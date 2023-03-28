from datetime import datetime
import logging
import os
import sys
import cv2
import keyboard
import libs.capturer as capturer


image_path = input('Save path: ')
total_saved = 0


def mainloop():
    capturer.create_capturer()

    toggle_save = False

    while True:
        key_code = cv2.waitKey(1)
        if key_code != -1:
            logging.debug(f'Got key_code {chr(key_code)}')

        if key_code == ord('q'):
            break

        image = capturer.get_image(True)
        if image is None:
            continue

        if keyboard.is_pressed('o'):
            toggle_save = not toggle_save

        is_saving_image = key_code == ord(
            'i') or keyboard.is_pressed('i') or toggle_save

        render_image(image, is_saving_image)

        if is_saving_image:
            save_image(image)


def render_image(image: cv2.Mat, is_saving_image: bool):
    canvas = image.repeat(2, axis=0).repeat(2, axis=1)

    if is_saving_image:
        cv2.circle(canvas, (20, 20), 20, (255, 255, 255), -1)  # nopep8 # pyright: ignore[reportUnknownMemberType]

    cv2.imshow('canvas', canvas)  # pyright: ignore[reportUnknownMemberType]


def save_image(image: cv2.Mat):
    global image_path, total_saved
    image_name = f'{datetime.now()}.png'.replace(' ', '_').replace(':', '-')
    logging.debug(f'Attempting to save image {image_name}')

    if not os.path.exists(image_path):
        os.mkdir(image_path)
        logging.info(f'Created folder \'{image_path}\'')
    elif os.path.isfile(image_path):
        logging.error(f'Save path \'{image_path}\' is a file')
        sys.exit()

    if not cv2.imwrite(os.path.join(image_path, image_name), image):
        logging.error('The image could not be saved for some reason')
        sys.exit(1)

    total_saved += 1
    logging.info(f'Saved image {total_saved}# {image_name}')


if __name__ == '__main__':
    logging.basicConfig(
        level=logging.INFO,
        format='[%(asctime)s] [%(name)s] [%(levelname)s] %(message)s',
        datefmt='%Y-%m-%d %H:%M:%S')
    mainloop()
