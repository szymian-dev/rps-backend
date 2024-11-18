import os
import tensorflow as tf
from sqlalchemy.orm import Session
from typing import Dict

from ..database.models import Model
from ..config import settings

class LoadedModel:
    def __init__(self, model: tf.keras.models.Model, transforms: List[int]):
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
    @classmethod
    def load_models_from_db(cls, db: Session):
        models = db.query(Model).all()
        cls.loaded_models = {}
        for model in models:
            keras_model = cls._load_model(model.path_to_model)
            transforms = model.transforms
            cls.loaded_models[model.id] = LoadedModel(keras_model, transforms)
        print(f"{len(cls.loaded_models)} models loaded into memory.")

    def _apply_transforms(self, data):
        pass

    def _make_prediction(self, model: tf.keras.models.Model, data):
        pass

    def predict(self, model_id: int, data) -> tf.Tensor:
        model = self.loaded_models.get(model_id)
        if not model:
            raise Exception(f"Model {model_id} not found.")
        return self._make_prediction(model, data)