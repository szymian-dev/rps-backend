import os
import tensorflow as tf
from sqlalchemy.orm import Session
from typing import Dict

from ..database.models import Model
from ..config import settings

loaded_models: Dict[int, tf.keras.models.Model] = {}


def _load_model(model_path: str) -> tf.keras.models.Model:
    path = os.path.join(settings.ai_models_folder, model_path)
    if not os.path.exists(path):
        raise Exception(f'Model file {path} not found')
    return tf.keras.models.load_model(path)


def load_models_from_db(db: Session):
    global loaded_models
    models = db.query(Model).all()
    for model in models:
        loaded_models[model.id] = _load_model(model.path_to_model)
    print(f"{len(loaded_models)} models loaded into memory.")

def get_model(model_id: int) -> tf.keras.models.Model:
    model = loaded_models.get(model_id)
    if not model:
        raise Exception(f"Model {model_id} not found.")
    return model
