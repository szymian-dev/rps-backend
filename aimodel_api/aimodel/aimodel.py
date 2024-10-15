import tensorflow as tf
import os

from ..config import settings

def _load_model(model_path: str) -> tf.keras.models.Model:
    if not os.path.exists(model_path):
        raise Exception(f'Model file {model_path} not found')
    
    model = tf.keras.models.load_model(model_path)
    return model

# Instantiate the model when the module is loaded
model = _load_model(settings.model_path)
