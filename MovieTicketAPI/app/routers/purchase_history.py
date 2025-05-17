from fastapi import APIRouter, HTTPException, Depends
from sqlalchemy.orm import Session
from app.database import SessionLocal
from app.models import PurchaseHistory
from app.schemas import PurchaseHistoryResponse
from typing import List

router = APIRouter()

def get_db():
    db = SessionLocal()
    try:
        yield db
    finally:
        db.close()

# Get all purchase history
@router.get("/purchase-history/", response_model=List[PurchaseHistoryResponse])
def get_all_purchase_history(db: Session = Depends(get_db)):
    purchases = db.query(PurchaseHistory).all()
    return purchases

# Get purchase history by user ID
@router.get("/purchase-history/user/{user_id}", response_model=List[PurchaseHistoryResponse])
def get_user_purchase_history(user_id: int, db: Session = Depends(get_db)):
    purchases = db.query(PurchaseHistory).filter(PurchaseHistory.user_id == user_id).all()
    if not purchases:
        raise HTTPException(status_code=404, detail=f"No purchase history found for user {user_id}")
    return purchases

# Get purchase history by movie ID
@router.get("/purchase-history/movie/{movie_id}", response_model=List[PurchaseHistoryResponse])
def get_movie_purchase_history(movie_id: int, db: Session = Depends(get_db)):
    purchases = db.query(PurchaseHistory).filter(PurchaseHistory.movie_id == movie_id).all()
    if not purchases:
        raise HTTPException(status_code=404, detail=f"No purchase history found for movie {movie_id}")
    return purchases

# Delete purchase history
@router.delete("/purchase-history/{purchase_id}")
def delete_purchase_history(purchase_id: int, db: Session = Depends(get_db)):
    purchase = db.query(PurchaseHistory).filter(PurchaseHistory.id == purchase_id).first()
    if not purchase:
        raise HTTPException(status_code=404, detail="Purchase history not found")
    
    db.delete(purchase)
    db.commit()
    return {"message": "Purchase history deleted successfully"}