from PIL import Image
import numpy as np
import cv2
import mediapipe as mp
import tensorflow as tf
import os
from skimage.morphology import remove_small_holes, remove_small_objects
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
            try:
                image.save("tr_rotated_image.jpg")
            except Exception as e:
                print("Could not save rotated image:", e)
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
            try:
                image.save("tr_grayscale_image.jpg")
            except Exception as e:
                print("Could not save grayscale image:", e)
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
            try:
                image.save("tr_resized_image.jpg")
            except Exception as e:
                print("Could not save resized image:", e)
    
        return image
    
# Normalizes the image array by dividing it by 255.0 to scale the pixel values to the range [0, 1]
class Normalize(Transformation):
    def __init__(self, verbose=False):
        self.verbose = verbose
    
    def apply(self, image):
        if isinstance(image, Image.Image):
            image = np.array(image)
        if not isinstance(image, np.ndarray):
            raise ValueError("NormalizeTransform: Image must be a PIL Image or a numpy array.")
        
        image = image.astype(np.float32)
        image = image / 255.0
        
        if self.verbose:
            print(f"Normalized image array to range [0, 1]. Min value: {image.min()}, Max value: {image.max()}")
        return image
    
# Adds a channel to the image array to make it compatible with grayscale models that expect a channel dimension (e.g. (224, 224, 1))
class AddGrayscaleChannel(Transformation):
    def __init__(self, verbose=False):
        self.verbose = verbose
    
    def apply(self, image):
        if isinstance(image, Image.Image):
            image = np.array(image)
        if not isinstance(image, np.ndarray):
            raise ValueError("AddGrayscaleChannel: Image must be a PIL Image or a numpy array.")
        if len(image.shape) != 2:
            raise ValueError("AddGrayscaleChannel: Image must be a 2D array.")
        
        image = np.expand_dims(image, axis=-1)
        
        if self.verbose:
            print("Added grayscale channel to image array. New shape:", image.shape)
        return image
    
# Adds a batch dimension to the image array. image must be a PIL Image object or a numpy array
class AddBatchDim(Transformation):
    def __init__(self, verbose=False):
        self.verbose = verbose
    
    def apply(self, image):
        if isinstance(image, Image.Image):
            image = np.array(image)
        if not isinstance(image, np.ndarray):
            raise ValueError("AddBatchDim: Image must be a PIL Image or a numpy array.")
        if len(image.shape) != 3:
            raise ValueError("AddBatchDim: Image must be a 3D array.")
        
        image = np.expand_dims(image, axis=0)
        
        if self.verbose:
            print("Added batch dimension to image array. New shape:", image.shape)
        return image
        
mp_hands = mp.solutions.hands
# Detects hands in the image using the MediaPipe Hands model. 
# Returns a mask with the detected hand filled in white, None if no hand was detected. image must be a PIL Image object
class HandDetection(Transformation):
    def __init__(self, min_detection_confidence=0.5, output_shape=(64, 64), verbose=False):
        self.verbose = verbose
        self.hands = mp_hands.Hands(static_image_mode=True, max_num_hands=1, min_detection_confidence=min_detection_confidence)
        self.output_shape = output_shape

    def apply(self, image):
        if isinstance(image, Image.Image):
            image = np.array(image)
        if not isinstance(image, np.ndarray):
            raise ValueError("HandDetection: Image must be a PIL Image or a numpy array.")
        
        results = self.hands.process(image)
        
        if not results.multi_hand_landmarks:
            if self.verbose:
                print("Mediapipe: No hands detected.")
            return None
        
        mask = np.zeros(image.shape[:2], dtype=np.uint8)
        for hand_landmarks in results.multi_hand_landmarks:
            points = [(int(lm.x * image.shape[1]), int(lm.y * image.shape[0])) for lm in hand_landmarks.landmark]
            cv2.fillPoly(mask, [np.array(points, dtype=np.int32)], 255)
            
        im = Image.fromarray(mask)
        im = im.resize(self.output_shape, Image.Resampling.LANCZOS)
        if im.mode != 'L':
            im = im.convert('L')
        
        if self.verbose:
            try:
                im.save("tr_hand_mask.png")
            except Exception as e:
                print("Could not save hand mask:", e)
        return im

# Segments the image using a U-Net model. image must be a PIL Image object or a numpy array
# Return a binary mask with the segmented hand, None if no hand was detected
class UnetSegmentation(Transformation):
    def __init__(self, model_filename, img_shape=(128, 128), verbose=False):
        self.model_filename = model_filename
        self.img_shape = img_shape
        self.verbose = verbose
        
        unet_model_path = os.path.join(settings.ai_models_folder, model_filename)
        if not os.path.exists(unet_model_path):
            raise ValueError(f"UnetSegmentation: Model file {unet_model_path} does not exist.")
        
        self.unet_model = tf.keras.models.load_model(unet_model_path)
    
    @staticmethod   
    def check_if_mask_almost_empty(mask):
        count_non_zero = np.count_nonzero(mask)
        if count_non_zero < 900:
            return True
        
    def apply(self, image):
        if isinstance(image, np.ndarray):
            image = Image.fromarray(image)
        if not isinstance(image, Image.Image):
            raise ValueError("UnetSegmentation: Image must be a PIL Image or a numpy array.")
        if image.mode != 'RGB':
            raise ValueError("UnetSegmentation: Input image must be in RGB format")
        
        # Preprocess image before segmentation with U-Net
        image = image.resize(self.img_shape, Image.Resampling.LANCZOS)
        image = np.array(image)
        image = image / 255.0
        image = np.expand_dims(image, axis=0)
        
        result = self.unet_model.predict(image)
        
        # Convert the output to a binary mask
        pred_mask = (result.squeeze() > 0.5).astype(np.uint8) 
        mask_bool = pred_mask.astype(bool)
        processed_mask = remove_small_objects(mask_bool, min_size=128)
        processed_mask = remove_small_holes(processed_mask, area_threshold=256).astype(np.uint8)
        
        if self.check_if_mask_almost_empty(processed_mask):
            if self.verbose:
                print("UnetSegmentation: No hands detected.")
            return None
        
        assert np.all(np.isin(processed_mask, [0, 1])), "UnetSegmentation: Mask contains values other than 0 and 1."
        if self.verbose:
            try:
                im = Image.fromarray(processed_mask * 255)
                im.save("tr_unet_mask.png")
            except Exception as e:
                print("Could not save U-Net mask:", e)
                
        return processed_mask
    
# Preprocesses the image array to match the input requirements of the ResNet model
class ResnetPreprocess(Transformation):
    def __init__(self, verbose=False):
        self.verbose = verbose
    
    def apply(self, image):
        if isinstance(image, Image.Image):
            image = np.array(image)
        if not isinstance(image, np.ndarray):
            raise ValueError("ResnetPreprocess: Image must be a PIL Image or a numpy array.")
        if image.shape != (224, 224, 3):
            raise ValueError("ResnetPreprocess: Image must be of shape (224, 224, 3).")
        
        image = image.astype(np.float32)
        image = tf.keras.applications.resnet50.preprocess_input(image)
        
        if self.verbose:
            print("Preprocessed image array for ResNet model.")
            try:
                im = Image.fromarray(image.astype(np.uint8))
                im.save("tr_resnet_preprocessed_image.jpg")
            except Exception as e:
                print("Could not save ResNet preprocessed image:", e)
        return image

class TransformationType(Enum):
    ROTATE = 1
    RESIZE = 2
    GRAYSCALE = 3
    MP_HANDS = 4
    U_NET = 5
    NORMALIZE = 6
    ADD_GRAYSCALE_CHANNEL = 7
    RESIZE128X128 = 8
    ADD_BATCH_DIM = 9
    RESNET_PREPROCESS = 10
    

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
                TransformationType.U_NET: UnetSegmentation(model_filename=settings.unet_model_name, verbose=settings.debug),
                TransformationType.NORMALIZE: Normalize(verbose=settings.debug),
                TransformationType.ADD_GRAYSCALE_CHANNEL: AddGrayscaleChannel(verbose=settings.debug),
                TransformationType.RESIZE128X128: ResizeImage(target_size=(128, 128), verbose=settings.debug),
                TransformationType.ADD_BATCH_DIM: AddBatchDim(verbose=settings.debug),
                TransformationType.RESNET_PREPROCESS: ResnetPreprocess(verbose=settings.debug)
            }

    @classmethod
    def get_transform_by_id(cls, transform_id):
        cls._initialize_transformations()
        return cls._transformations.get(transform_id)
    
# grayscale: 1- rotate, 2- resize, 3- grayscale, 6- normalize, 7- add_grayscale_channel, 9- add_batch_dim
# mediapipe: 1- rotate, 2- resize, 4- mediapipe, 6- normalize, 7- add_grayscale_channel, 9- add_batch_dim
# unet: 1- rotate, 5- unet, 7- add_grayscale_channel, 9- add_batch_dim
# rgb128x128: 1- rotate, 8- resize128x128, 6- normalize, 9- add_batch_dim
# resnet: 1- rotate, 2- resize, 10-resnet_preprocess, 9- add_batch_dim