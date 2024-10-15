from pydantic import BaseModel
from enum import Enum

'''
 DTOs
'''

class PredictionResponseDto(BaseModel):
    prediction : 'GestureType'

'''
 Enums
'''
class GestureType(Enum):
    ROCK = 1
    PAPER = 0
    SCISSORS = 2
    
    
