from pydantic import BaseModel
from enum import Enum
from typing import Optional

'''
 DTOs
'''
# Request DTOs

# Response DTOs
class PredictionResponseDto(BaseModel):
    prediction : Optional['GestureType'] = None

'''
 Enums
'''
class GestureType(Enum):
    ROCK = 1
    PAPER = 0
    SCISSORS = 2
    