from __future__ import annotations
from typing import Optional, NamedTuple

from math import floor
import logging
from win32gui import FindWindow, GetWindowRect
import dxcam  # pyright: ignore[reportMissingTypeStubs]
import cv2

from duplicatefilter import DuplicateFilter

if not 'logger' in locals():
    logger = logging.getLogger(__name__)
    logger.addFilter(DuplicateFilter([logging.WARNING]))


class RECT(NamedTuple):
    left: int
    top: int
    right: int
    bottom: int

    def size(self) -> SIZE:
        return SIZE(self.right - self.left, self.bottom - self.top)


class SIZE(NamedTuple):
    width: int
    height: int


class POINT(NamedTuple):
    x: int
    y: int


IntPtr = int

capturer: Optional[dxcam.DXCamera] = None
window_title: str = 'League of Legends (TM) Client'


def create_capturer():
    global capturer
    if capturer is not None:
        raise Exception('Cannot create multiple capturers')

    capturer = dxcam.create()  # nopep8 #pyright: ignore[reportUnknownMemberType, reportUnknownVariableType]


def get_window_handle() -> Optional[IntPtr]:
    logger.debug(f'Attempting to find window \'{window_title}\'')
    window_handle = FindWindow(None, window_title)
    return window_handle or None


def get_window_handle_blocking() -> IntPtr:
    logger.debug('Blocking until window handle is found')

    window_handle: Optional[IntPtr] = None
    while window_handle is None:
        window_handle = get_window_handle()

    logger.debug(f'Found window handle')
    return window_handle


def get_window_rect(window_handle: IntPtr) -> Optional[RECT]:
    logger.debug('Getting window RECT')
    window_rect = RECT(*GetWindowRect(window_handle))

    if not validate_window_rect(window_rect):
        return None

    return RECT(*window_rect)


def validate_window_rect(window_rect: RECT) -> bool:
    logger.debug('Validating window RECT')

    if window_rect.left < 0 or \
       window_rect.top < 0 or \
       window_rect.right < 0 or \
       window_rect.bottom < 0:
        logger.warning(
            'Window RECT has elements that are below zero, the window might be minimized')
        return False

    window_rect_size = window_rect.size()

    if window_rect_size.width < 0 or window_rect_size.height < 0:
        logger.warning(
            'Window RECT size is below zero, the window might be minimized')
        return False

    return True


def get_player_rect(window_rect: RECT) -> RECT:
    logger.debug('Getting player RECT')
    window_rect_size = window_rect.size()

    player_rect_center = POINT(
        window_rect.left + floor(window_rect_size.width * 0.45),
        window_rect.top + floor(window_rect_size.height * 0.45)
    )

    player_rect = RECT(
        player_rect_center.x - floor(window_rect_size.width * 0.2 / 2),
        player_rect_center.y - floor(window_rect_size.height * 0.2 / 2),
        player_rect_center.x + floor(window_rect_size.width * 0.2 / 2),
        player_rect_center.y + floor(window_rect_size.height * 0.2 / 2)
    )

    return player_rect


def get_raw_image(player_rect: RECT) -> Optional[cv2.Mat]:
    if capturer is None:
        raise Exception(
            'Cannot get an image if the capturer is not created')

    return capturer.grab(region=player_rect)  # nopep8 # pyright: ignore[reportUnknownVariableType]


def get_processed_image(raw_image: cv2.Mat) -> cv2.Mat:
    resized_image = cv2.resize(raw_image, (256, 256))
    gray_image = cv2.cvtColor(resized_image, cv2.COLOR_RGB2GRAY)
    return gray_image


def get_image(blocking: bool) -> Optional[cv2.Mat]:
    window_handle = get_window_handle_blocking() if blocking else get_window_handle()
    if window_handle is None:
        return None

    window_rect = get_window_rect(window_handle)
    if window_rect is None:
        return None

    player_rect = get_player_rect(window_rect)
    raw_image = get_raw_image(player_rect)
    if raw_image is None:
        return None

    processed_image = get_processed_image(raw_image)

    return processed_image
