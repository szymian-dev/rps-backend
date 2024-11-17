from sqlalchemy import create_engine, text
from sqlalchemy.ext.declarative import declarative_base
from sqlalchemy.orm import sessionmaker, Session
import os
from contextlib import contextmanager

SQLALCHEMY_DATABASE_URL = "sqlite:///./aimodels.db"

engine = create_engine(SQLALCHEMY_DATABASE_URL, echo=True)
SessionLocal = sessionmaker(autocommit=False, autoflush=False, bind=engine)

Base = declarative_base()

@contextmanager
def get_db():
    db = SessionLocal()  
    try:
        yield db 
    finally:
        db.close() 

def create_tables():
    from .models import Model, ModelStatistics
    Base.metadata.create_all(engine)

def populate_database_if_empty(dbSession : Session):
    from .populate_models import add_models_and_statistics
    from .models import Model
        
    count = dbSession.query(Model).count()

    if count == 0:
        print("Database is empty. Populating with initial data.")
        add_models_and_statistics(dbSession)
    else:
        print("Database is not empty. Skipping population.")

#create_tables()
#populate_database_if_empty()
