from pydantic import BaseModel
from typing import Optional, List
from datetime import datetime

class UserLogin(BaseModel):
    username: str
    password: str

class UserRegister(BaseModel):
    username: str
    password: str
    role: str = "customer"  # Default role for new registrations
    preferences: Optional[str] = None
    
    class Config:
        json_schema_extra = {
            "example": {
                "username": "newuser",
                "password": "password123",
                "preferences": "action,comedy"
            }
        }

class UpdatePreferences(BaseModel):
    preferences: str  # Comma-separated list of genres

    class Config:
        json_schema_extra = {
            "example": {
                "preferences": "action,comedy"
            }
        }

# User schema
class UserBase(BaseModel):
    username: str
    role: Optional[str] = "user"
    preferences: Optional[str] = None  # User preferences (e.g., genres)

class UserCreate(UserBase):
    password: str

class UserResponse(UserBase):
    id: int

    class Config:
        orm_mode = True

# Movie schema
class MovieBase(BaseModel):
    title: str
    description: str
    genre: str
    duration: int
    release_date: str
    price: float
    rating: Optional[float] = None
    image_url: Optional[str] = None  # URL of the movie poster

class MovieCreate(MovieBase):
    pass

class MovieResponse(MovieBase):
    id: int
    vector: Optional[str] = None  # Include vector if needed in the response

    class Config:
        orm_mode = True

# Ticket schema
class TicketBase(BaseModel):
    seat_numbers: str  # Comma-separated seat numbers
    user_id: int
    movie_id: int

class TicketCreate(TicketBase):
    pass

class TicketResponse(TicketBase):
    id: int
    total_price: float
    purchase_date: datetime

    class Config:
        orm_mode = True

# PurchaseHistory schema
class PurchaseHistoryBase(BaseModel):
    user_id: int
    movie_id: int

class PurchaseHistoryCreate(PurchaseHistoryBase):
    pass

class PurchaseHistoryResponse(PurchaseHistoryBase):
    id: int
    purchase_date: datetime

    class Config:
        orm_mode = True