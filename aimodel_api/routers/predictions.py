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
    print(f"Received file: {file.filename} with content type: {file.content_type}")
    image = _read_image(file)

    model_manager = ModelManager()
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