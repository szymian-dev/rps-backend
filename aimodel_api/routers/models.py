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
    models_db = db_session.scalars(
        select(Model).order_by(Model.id)
    ).all()
    return [AiModelDto.from_orm(model) for model in models_db]

@router.put("/statistics", tags=[tag], summary="Update the statistics of a model")
async def update_model_statistics(model_id: int, wrong_prediction: bool, db_session : db_dependency, res = Depends(validate_token)) -> bool:
    model = db_session.get(Model, model_id)
    if model is None:
        raise HTTPException(status_code=404, detail="Model not found")
    model.statistics.total_predictions += 1
    if wrong_prediction:
        model.statistics.wrong_predictions += 1
    db_session.commit()
    return True
