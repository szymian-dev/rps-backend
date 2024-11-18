from PIL import Image
import numpy as np
import cv2
import mediapipe as mp

class Transformation:
    def apply(self, image):
        raise NotImplementedError("Each transformation must implement the 'apply' method.")

class RotateImage(Transformation):
    def __init__(self, angle=90):
        self.angle = angle

    def apply(self, image):
        if image.height > image.width:
            image.rotate(90, expand=True)

class GrayscaleImage(Transformation):
    def apply(self, image):
        return image.convert('L')

mp_hands = mp.solutions.hands

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
