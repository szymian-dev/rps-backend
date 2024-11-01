from fastapi import FastAPI, Request
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

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)
@app.middleware("http")
async def log_requests(request: Request, call_next):
    logger.info(f"Request path: {request.url.path}")
    logger.info(f"Request method: {request.method}")
    logger.info(f"Request headers: {request.headers}")
    
    #if request.method == "POST":
    #    body = await request.body()
    #    logger.info(f"Request body: {body.decode()}")
    
    response = await call_next(request)
    return response

@app.on_event("startup")
async def startup_event():
    get_model()

app.include_router(predictions.router)