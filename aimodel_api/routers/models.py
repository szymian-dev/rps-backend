from fastapi import APIRouter, Depends, HTTPException
from sqlalchemy.orm import Session

from ..database.connection import get_db

router = APIRouter(
    prefix="/models",
)

tag = "Models"

@router.get("", tags=[tag], summary="Get all models")
async def get_models(db : Session = Depends(get_db)):
    return {"message": "Get all models"}