from PIL import Image
import numpy as np
import cv2
import mediapipe as mp
import tensorflow as tf
from enum import Enum

class Transformation:
    def apply(self, image):
        raise NotImplementedError("Each transformation must implement the 'apply' method.")

# Rotates the image by the specified angle. image must be a PIL Image object
class RotateImage(Transformation):
    def __init__(self, angle=90):
        self.angle = angle

    def apply(self, image):
        if image.height > image.width:
            image.rotate(90, expand=True)
        return image

# Converts the image to grayscale using the L mode. image must be a PIL Image object
class GrayscaleImage(Transformation):
    def apply(self, image):
        return image.convert('L')

# Resizes the image to the target size using the Lanczos resampling method. image must be a PIL Image object
class ResizeImage(Transformation):
    def __init__(self, target_size=(224, 224)):
        self.target_size = target_size

    def apply(self, image):
        return image.resize(self.target_size, Image.Resampling.LANCZOS)
    
# Normalizes the image array by dividing it by 255.0 to scale the pixel values to the range [0, 1], adds batch dimension
class NormalizeImage(Transformation):
    def apply(self, image):
        if isinstance(image, Image.Image):
            image_array = np.array(image)
        else:
            image_array = image
        image_array = np.array(image)
        image_array = np.expand_dims(image_array, axis=0)
        image_array /= 255.0
        return image_array
    
# Adds a channel to the image array to make it compatible with grayscale models that expect a channel dimension (e.g. (224, 224, 1))
class AddChannel(Transformation):
    def apply(self, image):
        return np.expand_dims(image, axis=-1)

mp_hands = mp.solutions.hands

# Detects hands in the image using the MediaPipe Hands model. Returns a mask with the detected hand filled in white, None if no hand was detected. image must be a PIL Image object
class HandDetection(Transformation):
    def __init__(self):
        self.hands = mp_hands.Hands(static_image_mode=True, max_num_hands=1, min_detection_confidence=0.5)

    def apply(self, image):
        image_rgb = cv2.cvtColor(np.array(image), cv2.COLOR_BGR2RGB)
        results = self.hands.process(image_rgb)
        
        if not results.multi_hand_landmarks:
            return None
        
        mask = np.zeros(image.size[::-1], dtype=np.uint8)
        for hand_landmarks in results.multi_hand_landmarks:
            points = [(int(lm.x * image.width), int(lm.y * image.height)) for lm in hand_landmarks.landmark]
            cv2.fillPoly(mask, [np.array(points, dtype=np.int32)], 255)
        
        return mask

# class UnetSegmentation(Transformation):
#     def __init__(self, model_path='./unet_model/unet.keras'):
#         self.model = tf.keras.models.load_model(model_path)

#     def apply(self, image: Image.Image) -> np.ndarray:
#         image_array = np.array(image)  
#         image_array = np.expand_dims(image_array, axis=0) 
#         prediction = self.model.predict(image_array)
#         return prediction[0]  


# grayscale: conv grayscae, resize, normalize
# mp_hands: resize, mp_hands, normalize
# u_net: resize, u_net, normalize


class TransformationType(Enum):
    GRAYSCALE = 1
    MP_HANDS = 2
    U_NET = 3
    RESIZE = 4
    NORMALIZE = 5
    ADD_CHANNEL = 6
    ROTATE = 7
    

class Transformations:
    @staticmethod
    def get_transform_by_id(transform_id):
        transformations = {
            TransformationType.GRAYSCALE: GrayscaleImage(),
            TransformationType.MP_HANDS: HandDetection(),
            TransformationType.U_NET: UnetSegmentation(),
            TransformationType.RESIZE: ResizeImage(),
            TransformationType.NORMALIZE: NormalizeImage(),
            TransformationType.ADD_CHANNEL: AddChannel(),
            TransformationType.ROTATE: RotateImage()
        }
        return transformations.get(transform_id)