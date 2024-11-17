from sqlalchemy import Integer, String, Text, JSON, ForeignKey
from sqlalchemy.orm import relationship, Mapped, mapped_column
from typing import Optional

from .connection import Base 


class Model(Base):
    __tablename__ = "models"

    id: Mapped[int] = mapped_column(Integer, primary_key=True)
    name: Mapped[str] = mapped_column(String(100), nullable=False)
    description: Mapped[Optional[str]] = mapped_column(Text, nullable=True)
    path_to_model: Mapped[str] = mapped_column(String(255), nullable=False)
    transformations: Mapped[list[int]] = mapped_column(JSON, nullable=False)

    # One-to-one relationship with model_statistics
    statistics: Mapped["ModelStatistics"] = relationship(
        "ModelStatistics", back_populates="model", uselist=False, cascade="all, delete-orphan"
    )

    def __repr__(self) -> str:
        return f"Model(id={self.id!r}, name={self.name!r})"

class ModelStatistics(Base):
    __tablename__ = "model_statistics"

    id: Mapped[int] = mapped_column(Integer, primary_key=True)
    model_id: Mapped[int] = mapped_column(ForeignKey("models.id"), nullable=False, unique=True)
    total_predictions: Mapped[int] = mapped_column(Integer, default=0)
    wrong_predictions: Mapped[int] = mapped_column(Integer, default=0)

    model: Mapped["Model"] = relationship("Model", back_populates="statistics")

    def __repr__(self) -> str:
        return f"ModelStatistics(id={self.id!r}, total_predictions={self.total_predictions!r}, wrong_predictions={self.wrong_predictions!r})"