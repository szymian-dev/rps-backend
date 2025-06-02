from fastapi import APIRouter, Depends, File, UploadFile, HTTPException
from PIL import Image
from io import BytesIO  
import numpy as np

from ..auth.jwt_handler import validate_token
from ..config import settings
from ..models import PredictionResponseDto, GestureType
from ..aimodel.aimodels import ModelManager

router = APIRouter(
    prefix="/predictions",
)

tag = "Predictions"

@router.post("", tags=[tag], summary="Predict the gesture in the image", response_model=PredictionResponseDto)
async def predict(model_id: int, file: UploadFile = File(...), res = Depends(validate_token)) -> PredictionResponseDto:
    image = _read_image(file)

    model_manager = ModelManager()
    if model_id == -1:
        return _predict_with_all(model_manager, image)
    
    prediction = model_manager.predict(model_id, image)
    if prediction is None:
        return PredictionResponseDto(prediction=None)
    predicted_class = GestureType(np.argmax(prediction))

    return PredictionResponseDto(prediction=predicted_class)

def _read_image(file: UploadFile) -> Image.Image:
    try:
        content = file.file.read()  
        image = Image.open(BytesIO(content)) 
    except Exception as e:
        raise HTTPException(status_code=400, detail="Invalid image file")
    return image

def _predict_with_all(model_manager: ModelManager, image: Image.Image) -> PredictionResponseDto:
    predicted_class = model_manager.predict_all(image)
    return PredictionResponseDto(prediction=predicted_class)