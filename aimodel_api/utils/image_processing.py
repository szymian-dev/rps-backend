from PIL import Image
import os
from tensorflow.keras.preprocessing import image
import numpy as np
#from rembg import remove


def prepare_image_for_prediction(img, target_size=(224, 224)):
    img = img.resize(target_size)
    img_array = image.img_to_array(img)
    # Expand dimensions (1, 224, 224, 3) because the model expects a batch of images
    img_array = np.expand_dims(img_array, axis=0)
    # Normalization of the image because the model was trained on normalized images
    img_array /= 255.0
    return img_array