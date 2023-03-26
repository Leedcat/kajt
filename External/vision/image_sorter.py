from dataclasses import dataclass
from enum import IntEnum, auto

import logging
import os
import cv2
import pathlib
from tqdm import tqdm
import sys


class Classification(IntEnum):
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

    load_path = pathlib.Path(sys.argv[1] if len(
        sys.argv) > 1 else input("Load path: "))
    if not load_path.exists() or not load_path.is_dir():
        logging.error(f'\'{load_path}\' is not a folder')

    save_path = pathlib.Path(sys.argv[2] if len(
        sys.argv) > 2 else input("Save path: "))
    if not save_path.exists():
        os.mkdir(save_path)
    elif not save_path.is_dir():
        logging.error(f'\'{save_path}\' is not a folder')

    limit: 'int | None' = None
    if len(sys.argv) > 3:
        limit = int(sys.argv[3])

    logging.info("Getting images")
    image_names = list(map(lambda path: path.name, load_path.glob('*.png')))
    if limit is not None:
        image_names = image_names[:limit]
    images = list(map(lambda image_name: ImageData(
        str(image_name),
        Classification.UNKNOWN,
        read_image(pathlib.Path(load_path, image_name))),
        tqdm(image_names)))
    index = 0
    logging.info("Done")

    while True:
        index %= len(images)
        image = images[index]

        render_image(image, index, len(images))

        while True:
            key_code = cv2.waitKey()

            if key_code == ord('q'):
                return

            if key_code == ord('s'):
                save_images(save_path, load_path, images)
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


def read_image(image_path: pathlib.Path) -> cv2.Mat:
    image = cv2.imread(str(image_path), cv2.IMREAD_GRAYSCALE)
    return image


def render_image(image: ImageData, index: int, max_index: int):
    canvas = image.image.repeat(2, axis=0).repeat(2, axis=1)
    canvas = cv2.putText(canvas, f"{str(image.classification)}",  # nopep8 # pyright: ignore[reportUnknownMemberType]
                         (20, 20), cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 0, 0), 3)
    canvas = cv2.putText(canvas, f"{index}/{max_index}",  # nopep8 # pyright: ignore[reportUnknownMemberType]
                         (20, 40), cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 0, 0), 3)
    cv2.imshow("canvas", canvas)  # nopep8 # pyright: ignore[reportUnknownMemberType]


def save_images(save_path: pathlib.Path, load_path: pathlib.Path, images: 'list[ImageData]'):
    logging.info(
        f"Saving images")
    for image in tqdm(images):
        image_path = pathlib.Path(save_path, str(
            image.classification).lower(), image.name)

        old_image_path = pathlib.Path(load_path, image.name)

        if not image_path.parents[0].exists():
            os.mkdir(image_path.parents[0])

        os.rename(old_image_path, image_path)
        # if not cv2.imwrite(str(image_path), image.image):
        #     logging.error(
        #         f"Could not save image \"{image.name}\" to {image_path}")
        #     sys.exit()


if __name__ == "__main__":
    logging.basicConfig(
        level=logging.INFO,
        format='[%(asctime)s] [%(name)s] [%(levelname)s] %(message)s',
        datefmt='%Y-%m-%d %H:%M:%S')
    mainloop()
