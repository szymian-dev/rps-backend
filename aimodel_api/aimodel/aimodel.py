import os
os.environ['TF_ENABLE_ONEDNN_OPTS'] = '0'
os.environ['TF_CPP_MIN_LOG_LEVEL'] = '2' 
import tensorflow as tf

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
        model = _load_model(settings.ai_model_path)
    return model