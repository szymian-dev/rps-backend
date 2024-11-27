import jwt
import base64
import json
from jwt import algorithms
from fastapi import Header, HTTPException, Depends
from typing import Optional

from ..config import settings

def validate_token(authorization: Optional[str] = Header(default=None)): 
    if settings.skip_auth:
        return True
    
    if authorization is None or not authorization.startswith("Bearer "):
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
    