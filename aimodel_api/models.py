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
    ROCK = 0
    PAPER = 1
    SCISSORS = 2
    
    
