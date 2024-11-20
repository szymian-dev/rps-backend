from PIL import Image
import numpy as np
import cv2
import mediapipe as mp
import tensorflow as tf
from enum import Enum

from ..config import settings


class Transformation:
    def apply(self, image):
        raise NotImplementedError("Each transformation must implement the 'apply' method.")

# Rotates the image by the specified angle. image must be a PIL Image object
class RotateImageIfVertical(Transformation):
    def __init__(self, angle=90, verbose=False):
        self.angle = angle
        self.verbose = verbose

    def apply(self, image):
        if not isinstance(image, Image.Image):
            raise ValueError("RotateTransform: Image must be a PIL Image.")
        
        if image.height > image.width:
            image = image.rotate(90, expand=True)
            
        if self.verbose:
            image.save("tr_rotated_image.jpg")
        return image

# Converts the image to grayscale using the L mode. image must be a PIL Image object
class GrayscaleImage(Transformation):
    def __init__(self, verbose=False):
        self.verbose = verbose
    
    def apply(self, image):
        if not isinstance(image, Image.Image):
            raise ValueError("GrayscaleTransform: Image must be a PIL Image.")
        
        image = image.convert('L')
        if self.verbose:
            image.save("tr_grayscale_image.jpg")
        return image

# Resizes the image to the target size using the Lanczos resampling method. image must be a PIL Image object
class ResizeImage(Transformation):
    def __init__(self, target_size=(224, 224), verbose=False):
        self.target_size = target_size
        self.verbose = verbose

    def apply(self, image):
        if isinstance(image, np.ndarray):
            image = Image.fromarray(image)
        
        if not isinstance(image, Image.Image):
            raise ValueError("ResizeTransform: Image must be a PIL Image.")
        
        image = image.resize(self.target_size, Image.Resampling.LANCZOS)
        if self.verbose:
            image.save("tr_resized_image.jpg")
        
        return image
    
# Normalizes the image array by dividing it by 255.0 to scale the pixel values to the range [0, 1], adds batch dimension
# Should always be the last transformation in the pipeline
class NormalizeImage(Transformation):
    def __init__(self, verbose=False):
        self.verbose = verbose
    
    def apply(self, image):
        if isinstance(image, Image.Image):
            image_array = np.array(image)
        elif isinstance(image, np.ndarray):
            image_array = image
        else:
            raise ValueError("NormalizeTransform: Image must be a PIL Image or a numpy array.")
        
        if len(image_array.shape) == 4:
            raise ValueError("NormalizeTransform: Image must have shape (height, width, channels). Why are you normalizing a batch of images?")
        if len(image_array.shape) == 2:
            raise ValueError("NormalizeTransform: Image has just 2 dimensions. Did you forget to add a channel dimension?")
        
        image_array = np.expand_dims(image_array, axis=0)
        image_array = image_array.astype(np.float32)
        image_array /= 255.0
        
        if self.verbose:
            print("Normalized image array shape:", image_array.shape)
        return image_array
    
# Adds a channel to the image array to make it compatible with grayscale models that expect a channel dimension (e.g. (224, 224, 1))
class AddChannel(Transformation):
    def __init__(self, verbose=False):
        self.verbose = verbose
    
    def apply(self, image):
        if isinstance(image, Image.Image):
            image = np.array(image)
        if not isinstance(image, np.ndarray):
            raise ValueError("AddChannel: Image must be a PIL Image or a numpy array.")
        
        if len(image.shape) == 3:
            return image
        image = np.expand_dims(image, axis=-1)
        
        if self.verbose:
            print("Added channel to image array. New shape:", image.shape)
        return image
    
    
mp_hands = mp.solutions.hands

# Detects hands in the image using the MediaPipe Hands model. Returns a mask with the detected hand filled in white, None if no hand was detected. image must be a PIL Image object
class HandDetection(Transformation):
    def __init__(self, min_detection_confidence=0.5, verbose=False):
        self.verbose = verbose
        self.hands = mp_hands.Hands(static_image_mode=True, max_num_hands=1, min_detection_confidence=min_detection_confidence)

    def apply(self, image):
        if isinstance(image, Image.Image):
            image = np.array(image)
        if not isinstance(image, np.ndarray):
            raise ValueError("HandDetection: Image must be a PIL Image or a numpy array.")
        
        results = self.hands.process(image)
        
        if not results.multi_hand_landmarks:
            if self.verbose:
                print("No hands detected.")
            return None
        
        mask = np.zeros(image.shape[:2], dtype=np.uint8)
        for hand_landmarks in results.multi_hand_landmarks:
            points = [(int(lm.x * image.shape[1]), int(lm.y * image.shape[0])) for lm in hand_landmarks.landmark]
            cv2.fillPoly(mask, [np.array(points, dtype=np.int32)], 255)
        
        if self.verbose:
            cv2.imwrite("tr_hand_mask.jpg", mask)
        return mask

# Placeholder class for U-Net segmentation. Do not use.
# Segments the image using a U-Net model. image must be a PIL Image object or a numpy array
class UnetSegmentation(Transformation):
    def __init__(self, model, verbose=False):
        self.model = model
        self.verbose = verbose

    def apply(self, image):
        if isinstance(image, Image.Image):
            image = np.array(image)
        if not isinstance(image, np.ndarray):
            raise ValueError("UnetSegmentation: Image must be a PIL Image or a numpy array.")
        
        raise NotImplementedError("U-Net segmentation is not implemented yet.")


class TransformationType(Enum):
    ROTATE = 1
    RESIZE = 2
    GRAYSCALE = 3
    MP_HANDS = 4
    U_NET = 5
    NORMALIZE = 6
    ADD_CHANNEL = 7
    RESIZE400X300 = 8
    

class Transformations:
    _transformations = None  # Zmienna klasowa przechowująca dict transformacji

    def __new__(cls, *args, **kwargs):
        raise TypeError(f"{cls.__name__} is a static class and cannot be instantiated.")

    @classmethod
    def _initialize_transformations(cls):
        if cls._transformations is None:
            cls._transformations = {
                TransformationType.ROTATE: RotateImageIfVertical(angle=90, verbose=settings.debug),
                TransformationType.RESIZE: ResizeImage(target_size=(224, 224), verbose=settings.debug),
                TransformationType.GRAYSCALE: GrayscaleImage(verbose=settings.debug),
                TransformationType.MP_HANDS: HandDetection(verbose=settings.debug),
                TransformationType.U_NET: UnetSegmentation(model=None, verbose=settings.debug),  # Placeholder, do not use
                TransformationType.NORMALIZE: NormalizeImage(verbose=settings.debug),
                TransformationType.ADD_CHANNEL: AddChannel(verbose=settings.debug),
                TransformationType.RESIZE400X300: ResizeImage(target_size=(400, 300), verbose=settings.debug)
            }

    @classmethod
    def get_transform_by_id(cls, transform_id):
        cls._initialize_transformations()
        return cls._transformations.get(transform_id)
    
# grayscle: rotate, resize, grayscale, add_channel, normalize
# hands: rotate, resize400x300, hands, resize, add_channel, normalize
# unet: rotate, resize, unet, add_channel, normalize
# rgb: rotate, resize, normalize