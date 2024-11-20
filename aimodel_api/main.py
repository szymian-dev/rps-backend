from fastapi import FastAPI, Request, APIRouter
from fastapi.middleware.cors import CORSMiddleware
from fastapi.responses import JSONResponse
import logging

from .routers import predictions, models
from .config import settings
from .database.connection import SessionLocal, create_tables, populate_database_if_empty, get_db
from .aimodel.aimodels import ModelManager

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
    db: Session = next(get_db())
    try:
        create_tables()
        populate_database_if_empty(db)
        mm = ModelManager()
        mm.load_models_from_db(db)
    finally:
        db.close() 
        

    
api_router = APIRouter(prefix="/api/v1")
api_router.include_router(predictions.router)
api_router.include_router(models.router)
app.include_router(api_router)