from fastapi import FastAPI, Request, APIRouter
from fastapi.middleware.cors import CORSMiddleware
from fastapi.responses import JSONResponse
import logging

from .routers import predictions
from .config import settings
from .aimodel.aimodel import get_model

app = FastAPI(
    title=settings.app_name,
    debug =settings.debug,
)

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

@app.on_event("startup")
async def startup_event():
    get_model()
    
api_router = APIRouter(prefix="/api/v1")
api_router.include_router(predictions.router)
app.include_router(api_router)