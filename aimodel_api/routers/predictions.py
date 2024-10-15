from fastapi import APIRouter, Depends, File, UploadFile, HTTPException
from PIL import Image
from io import BytesIO  
import numpy as np

from ..auth.jwt_handler import validate_token
from ..config import settings
from ..utils.image_processing import prepare_image_for_prediction
from ..models import PredictionResponseDto, GestureType
from ..aimodel.aimodel import get_model  

router = APIRouter(
    prefix="/predictions",
)

@router.post("/")
async def predict(file: UploadFile = File(...), res = Depends(validate_token), model = Depends(get_model)) -> PredictionResponseDto:
    try:
        content = await file.read()  
        image = Image.open(BytesIO(content)) 
    except Exception as e:
        raise HTTPException(status_code=400, detail="Invalid image file")

    image = _process_image(image)
    predicted_class = _predict(image, model)


    return PredictionResponseDto(prediction=predicted_class)

def _process_image(image):
    image = prepare_image_for_prediction(image)
    return image

def _predict(image, model):
    try:
        prediction = model.predict(image)
    except Exception as e:
        raise HTTPException(status_code=500, detail="Error predicting image")

    predicted_class = GestureType(int(np.argmax(prediction)))
    return predicted_class