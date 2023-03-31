# Modifed version of the one by zwer on stackoverflow (https://stackoverflow.com/a/44692178)
from typing import Optional

import logging


class DuplicateFilter(logging.Filter):
    def __init__(self, levelno: 'list[int]' = []):
        super().__init__()
        self._last_log: Optional[tuple[str, int, str]] = None
        self._levelno: 'list[int]' = levelno

    def filter(self, record: logging.LogRecord) -> bool:
        if record.levelno not in self._levelno:
            return True

        current_log = (record.module, record.levelno, record.msg)

        if current_log != self._last_log:
            self._last_log = current_log
            return True

        return False
