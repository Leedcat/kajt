import logging
import cv2
import numpy as np
import socket
import signal
import tensorflow as tf

import libs.capturer as capturer
from image_sorter import Classification


class_names = ['attacking', 'idling', 'walking']
client: 'socket.socket | None' = None
host = 'localhost'
port = 9000

# model = tf.keras.models.load_model('model.hdf5')
interpreter = tf.lite.Interpreter(model_path='model.tflite')
classify_lite = interpreter.get_signature_runner('serving_default')


def mainloop():
    global client, host, port
    capturer.create_capturer()

    client = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    logging.info(f'Trying to connect to {host}:{port}')
    client.connect((host, port))

    while True:
        image = capturer.get_image(blocking=True)

        if image is None:
            logging.error("Could not get image")
            # sys.exit(1)
            continue

        image_class, certainty = classify_image(image)

        logging.info(
            f"Classified image as {image_class} with {certainty*100:.2f}% certainty")

        data = (image_class).to_bytes(1, 'big')
        client.send(data)
        logging.debug(f"Sent data: {data}")

        cv2.imshow("canvas", image)  # nopep8 # pyright: ignore[reportUnknownMemberType]
        cv2.waitKey(1)


def classify_image(image: cv2.Mat) -> 'tuple[Classification, float]':
    global classify_lite
    image_batch = np.expand_dims(image, axis=(0, 3)).astype('float32')  # nopep8 # pyright: ignore[reportUnknownMemberType]
    predictions = classify_lite(rescaling_input=image_batch)['outputs']
    score = tf.nn.softmax(predictions[0])
    image_class = Classification(np.argmax(score) + 1)  # nopep8 # pyright: ignore[reportUnknownMemberType]
    certainty = np.max(score)   # nopep8 # pyright: ignore[reportUnknownMemberType]

    return (image_class, certainty)


if __name__ == '__main__':
    logging.basicConfig(
        level=logging.INFO,
        format='[%(asctime)s] [%(name)s] [%(levelname)s] %(message)s',
        datefmt='%Y-%m-%d %H:%M:%S')

    signal.signal(signal.SIGINT,
                  lambda signal, frame: client.close() if client is not None else None)

    mainloop()
