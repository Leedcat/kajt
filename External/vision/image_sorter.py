from dataclasses import dataclass
from enum import Enum, auto

import logging
import os
import sys
import cv2


class Classification(Enum):
    ATTACKING = auto()
    IDLING = auto()
    WALKING = auto()
    UNKNOWN = auto()

    def __str__(self) -> str:
        if self == Classification.ATTACKING:
            return "ATTACKING"
        if self == Classification.IDLING:
            return "IDLING"
        if self == Classification.WALKING:
            return "WALKING"
        if self == Classification.UNKNOWN:
            return "UNKNOWN"

        raise Exception(f"Unknown Classification {self.value}")


@dataclass
class ImageData:
    name: str
    classification: Classification
    image: cv2.Mat


def mainloop():
    if len(sys.argv) < 3:
        logging.error('Missing path arguments')
        sys.exit(1)

    load_path = sys.argv[1]
    if not os.path.exists(load_path) or not os.path.isdir(load_path):
        logging.error(f'\'{load_path}\' is not a folder')

    save_path = sys.argv[2]
    if not os.path.exists(save_path) or not os.path.isdir(save_path):
        logging.error(f'\'{save_path}\' is not a folder')

    image_names = os.listdir(load_path)
    images = list(map(lambda image_name: ImageData(image_name, Classification.UNKNOWN,
                  read_image(os.path.join(load_path, image_name))), image_names))
    index = 0

    while True:
        index %= len(images)
        image = images[index]

        render_image(image, index, len(images))

        while True:
            key_code = cv2.waitKey()

            if key_code == ord('q'):
                return

            if key_code == ord('s'):
                save_images(save_path, images)
                return

            if key_code == ord('a'):
                image.classification = Classification.ATTACKING
                index += 1
                break
            elif key_code == ord('i'):
                image.classification = Classification.IDLING
                index += 1
                break
            elif key_code == ord('w'):
                image.classification = Classification.WALKING
                index += 1
                break
            elif key_code == ord('u'):
                image.classification = Classification.UNKNOWN
                index += 1
                break
            elif key_code == ord('n'):
                index += 1
                break
            elif key_code == ord('p'):
                index -= 1
                break


def read_image(image_path: str) -> cv2.Mat:
    image = cv2.imread(image_path, cv2.IMREAD_GRAYSCALE)
    return image


def render_image(image: ImageData, index: int, max_index: int):
    canvas = image.image.repeat(2, axis=0).repeat(2, axis=1)
    canvas = cv2.putText(canvas, f"{str(image.classification)}",  # nopep8 # pyright: ignore[reportUnknownMemberType]
                         (20, 20), cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 0, 0), 3)
    canvas = cv2.putText(canvas, f"{index}/{max_index}",  # nopep8 # pyright: ignore[reportUnknownMemberType]
                         (20, 40), cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 0, 0), 3)


def save_images(save_path: str, images: 'list[ImageData]'):
    for i, image in enumerate(images):
        image_path = os.path.join(save_path, str(
            image.classification).lower(), image.name)
        if not cv2.imwrite(image_path, image.image):
            logging.error(
                f"Could not save image \"{image.name}\" to {image_path}")
        else:
            logging.info(
                f"Saved image {i+1}/{len(images)}# \"{image.name}\" as {str(image.classification)}")


if __name__ == "__main__":
    mainloop()
