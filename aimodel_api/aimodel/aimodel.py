import tensorflow as tf
import os
from ..config import settings

# Global model variable to avoid loading the model multiple times
model = None

def _load_model(model_path: str) -> tf.keras.models.Model:
    if not os.path.exists(model_path):
        raise Exception(f'Model file {model_path} not found')
    
    model = tf.keras.models.load_model(model_path)
    return model

def get_model() -> tf.keras.models.Model:
    global model
    if model is None:
        model = _load_model(settings.model_path)
    return model