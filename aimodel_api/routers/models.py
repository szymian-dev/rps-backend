from fastapi import APIRouter, Depends, HTTPException
from sqlalchemy import select
from sqlalchemy.orm import Session

from ..database.connection import db_dependency
from ..database.models import Model
from ..models import AiModelDto
from ..auth.jwt_handler import validate_token

router = APIRouter(
    prefix="/models",
)

tag = "Models"

@router.get("", tags=[tag], summary="Get all models")
async def get_models(db_session : db_dependency, res = Depends(validate_token)) -> list[AiModelDto]:
    print(db_session)
    
    models_db = db_session.scalars(
        select(Model).order_by(Model.id)
    ).all()
    return [AiModelDto.from_orm(model) for model in models_db]