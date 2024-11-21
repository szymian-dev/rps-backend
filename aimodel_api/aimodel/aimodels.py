import os
import tensorflow as tf
from sqlalchemy.orm import Session
from typing import Dict
from PIL import Image
from fastapi import HTTPException

from ..database.models import Model
from ..config import settings
from .transformations import Transformations, TransformationType
from ..config import settings

import numpy as np

class LoadedModel:
    def __init__(self, model: tf.keras.models.Model, transforms: list[int]):
        self.model = model
        self.transforms = transforms

class ModelManager:
    _instance = None
    loaded_models: Dict[int, LoadedModel] = {}

    # Singleton
    def __new__(cls, *args, **kwargs):
        if cls._instance is None:
            cls._instance = super(ModelManager, cls).__new__(cls, *args, **kwargs)
            cls._instance.loaded_models = {}  
        return cls._instance

    @staticmethod
    def _load_model(model_path: str) -> tf.keras.models.Model:
        path = os.path.join(settings.ai_models_folder, model_path)
        if not os.path.exists(path):
            raise Exception(f'Model file {path} not found')
        return tf.keras.models.load_model(path)

    # Load models from database
    def load_models_from_db(self, db: Session):
        models = db.query(Model).all()
        self.loaded_models = {}
        for model in models:
            keras_model = self._load_model(model.path_to_model)
            transforms = model.transformations
            self.loaded_models[model.id] = LoadedModel(keras_model, transforms)
        print(f"{len(self.loaded_models)} models loaded into memory.")

    # Apply transformations to image from list of transformation ids
    def _apply_transforms(self, image: Image.Image, transforms: list[int]):
        if image.mode == "RGBA":
            image = image.convert("RGB")
            print("Converted image from RGBA to RGB")
        
        for transform_id in transforms:
            try:
                transformation_type = TransformationType(transform_id)
            except ValueError:
                raise ValueError(f"Invalid transformation id {transform_id}")
            transform = Transformations.get_transform_by_id(transformation_type)
            if not transform:
                raise ValueError(f"Transformation {transform_id} not found.")
            image = transform.apply(image)
            if image is None:
                return None
        return image


    def predict(self, model_id: int, image: Image.Image) -> tf.Tensor:
        model_data = self.loaded_models.get(model_id)
        if not model_data:
            raise HTTPException(status_code=404, detail=f"Model {model_id} not found.")
        
        image = self._apply_transforms(image, model_data.transforms)
        if image is None:
            return None
        
        try:
            prediction = model_data.model.predict(image)
            
            if settings.debug:
                try:
                    image_array = image.squeeze() 
                    if image_array.dtype != np.uint8:
                        image_array = (image_array * 255).astype(np.uint8)  
                    
                    i = Image.fromarray(image_array)  
                    i.save("tr_test.jpg") 
                except Exception as e:
                    print(f"Error saving transformed image: {str(e)}. Skipping, don't worry.")
        except Exception as e:
            raise Exception(f"Error predicting with model {model_id}: {str(e)}")
        return prediction
        