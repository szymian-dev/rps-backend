from pydantic import BaseModel
from enum import Enum
from typing import Optional

'''
 DTOs
'''
# General DTOs
class AiModelDto(BaseModel):
    id: int
    name: str
    description: Optional[str] = None
    
    class Config:
        from_attributes = True


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
    