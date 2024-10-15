from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware

from .routers import predictions
from .config import settings

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

app.include_router(predictions.router)