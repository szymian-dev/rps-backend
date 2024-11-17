from fastapi import FastAPI, Request, APIRouter
from fastapi.middleware.cors import CORSMiddleware
from fastapi.responses import JSONResponse
import logging

from .routers import predictions, models
from .config import settings
from .database.connection import SessionLocal, create_tables, populate_database_if_empty, get_db
from .aimodel.aimodels import load_models_from_db, loaded_models

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
def startup():
    create_tables()
    
    with get_db() as db:
        populate_database_if_empty(db)
        
    with get_db() as db:
        load_models_from_db(db)
        

    
api_router = APIRouter(prefix="/api/v1")
api_router.include_router(predictions.router)
app.include_router(api_router)
app.include_router(models.router)