from sqlalchemy import Column, Integer, String, Float, ForeignKey, DateTime
from sqlalchemy.orm import relationship
from .database import Base
from datetime import datetime
from zoneinfo import ZoneInfo

class User(Base):
    __tablename__ = "users"
    id = Column(Integer, primary_key=True, index=True)
    username = Column(String, unique=True, index=True, nullable=False)
    password = Column(String, nullable=False)
    role = Column(String, default="user", nullable=False)
    preferences = Column(String, nullable=True)  # User preferences (e.g., genres)

    # Relationship to tickets
    tickets = relationship("Ticket", back_populates="user")
    purchase_history = relationship("PurchaseHistory", back_populates="user")

class Movie(Base):
    __tablename__ = "movies"
    id = Column(Integer, primary_key=True, index=True)
    title = Column(String, index=True, nullable=False)
    description = Column(String, nullable=False)
    genre = Column(String, nullable=False)
    duration = Column(Integer, nullable=False)  # Duration in minutes
    release_date = Column(String, nullable=False)
    vector = Column(String)  # Assuming vector is stored as a string representation of a list
    price = Column(Float, nullable=False)  # Price of the movie ticket
    rating = Column(Float, nullable=True)  # Average rating of the movie (optional)
    image_url = Column(String, nullable=True)  # URL of the movie poster

    # Relationship to tickets
    tickets = relationship("Ticket", back_populates="movie")

class Ticket(Base):
    __tablename__ = "tickets"
    id = Column(Integer, primary_key=True, index=True)
    seat_numbers = Column(String, nullable=False)  # Store multiple seat numbers as a comma-separated string
    user_id = Column(Integer, ForeignKey("users.id"), nullable=False)
    movie_id = Column(Integer, ForeignKey("movies.id"), nullable=False)
    total_price = Column(Float, nullable=False)  # Total price for the tickets
    purchase_date = Column(DateTime, default=lambda: datetime.now(ZoneInfo("Asia/Ho_Chi_Minh")))  # Default to current UTC time
    status = Column(String, default="pending")  # Add this field

    # Relationships
    user = relationship("User", back_populates="tickets")
    movie = relationship("Movie", back_populates="tickets")

class PurchaseHistory(Base):
    __tablename__ = "purchase_history"
    id = Column(Integer, primary_key=True, index=True)
    user_id = Column(Integer, ForeignKey("users.id"), nullable=False)
    movie_id = Column(Integer, ForeignKey("movies.id"), nullable=False)
    purchase_date = Column(DateTime, default=lambda: datetime.now(ZoneInfo("Asia/Ho_Chi_Minh")))

    # Relationships
    user = relationship("User", back_populates="purchase_history")
    movie = relationship("Movie")
