from PIL import Image
import os
from tensorflow.keras.preprocessing import image
import numpy as np
#from rembg import remove


def prepare_image_for_prediction(img, target_size=(224, 224), grayscale=True):
    if img.height > img.width:
        img = img.rotate(90, expand=True)
    
    if grayscale:
        img = img.convert('L')
    
    img = img.resize(target_size, Image.Resampling.LANCZOS)
    img_array = image.img_to_array(img)
    
    if grayscale:
        img_array = np.expand_dims(img_array, axis=-1)
    
    img_array = np.expand_dims(img_array, axis=0)
    img_array /= 255.0
    return img_array