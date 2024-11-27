import jwt
import base64
import json
from jwt import algorithms
from fastapi import Header, HTTPException, Depends

from ..config import settings

# To skip authentication, uncomment the following lines
def validate_token(authorization : str = ''):
   return True
 
'''
def validate_token(authorization: str = Header(...)): 
    if not authorization.startswith("Bearer "):
        raise HTTPException(status_code=403, detail="Invalid authentication credentials")
    token = authorization.split(" ")[1] 
  
    options = {
        "verify_aud": False
    }
    try:
        payload = jwt.decode(token, settings.secret_key, algorithms=["HS512"], issuer=settings.valid_issuer, options=options)
    except jwt.ExpiredSignatureError:
        raise HTTPException(status_code=401, detail="Token has expired")
    except jwt.InvalidTokenError:
        raise HTTPException(status_code=403, detail="Invalid token")
    
    return payload
    
'''