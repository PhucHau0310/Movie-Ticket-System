from fastapi import APIRouter, HTTPException, Depends, Request
from sqlalchemy.orm import Session
from app.database import SessionLocal
from app.models import Ticket, Movie, User
from app.schemas import TicketCreate, TicketResponse
from datetime import datetime
from app.utils.vnpay_utils import generate_vnpay_payment_url, verify_vnpay_response

router = APIRouter()

def get_db():
    db = SessionLocal()
    try:
        yield db
    finally:
        db.close()

# Get tickets by user ID
@router.get("/tickets/user/{user_id}", response_model=list[TicketResponse])
def get_tickets_by_user(user_id: int, db: Session = Depends(get_db)):
    # Check if user exists
    user = db.query(User).filter(User.id == user_id).first()
    if not user:
        raise HTTPException(status_code=404, detail="User not found")
    
    # Get all tickets for the user
    tickets = db.query(Ticket).filter(Ticket.user_id == user_id).all()
    return tickets


# Create a new ticket with VNPay integration
@router.post("/tickets/", response_model=dict)
def create_ticket(ticket: TicketCreate, db: Session = Depends(get_db)):
    # Check if the movie exists
    movie = db.query(Movie).filter(Movie.id == ticket.movie_id).first()
    if not movie:
        raise HTTPException(status_code=404, detail="Movie not found")

    # Check if the user exists
    user = db.query(User).filter(User.id == ticket.user_id).first()
    if not user:
        raise HTTPException(status_code=404, detail="User not found")

    # Calculate total price
    seat_count = len(ticket.seat_numbers.split(","))
    total_price = seat_count * movie.price

    # Save ticket to database (status: pending)
    db_ticket = Ticket(
        seat_numbers=ticket.seat_numbers,
        user_id=ticket.user_id,
        movie_id=ticket.movie_id,
        total_price=total_price,
        purchase_date=datetime.now(),
        status="pending"  # Add a status field to track payment
    )
    db.add(db_ticket)
    db.commit()
    db.refresh(db_ticket)

    order_info = f"Thanh toan ve xem phim {movie.title}".replace(" ", "-")
    payment_url = generate_vnpay_payment_url(
        order_id=db_ticket.id,
        amount=int(total_price),
        order_info=order_info
    )

    return {"payment_url": payment_url, "ticket_id": db_ticket.id}

# Get all tickets
@router.get("/tickets/", response_model=list[TicketResponse])
def get_tickets(db: Session = Depends(get_db)):
    tickets = db.query(Ticket).all()
    return tickets

# Get a ticket by ID
@router.get("/tickets/{ticket_id}", response_model=TicketResponse)
def get_ticket(ticket_id: int, db: Session = Depends(get_db)):
    ticket = db.query(Ticket).filter(Ticket.id == ticket_id).first()
    if not ticket:
        raise HTTPException(status_code=404, detail="Ticket not found")
    return ticket

# Update a ticket
@router.put("/tickets/{ticket_id}", response_model=TicketResponse)
def update_ticket(ticket_id: int, ticket: TicketCreate, db: Session = Depends(get_db)):
    db_ticket = db.query(Ticket).filter(Ticket.id == ticket_id).first()
    if not db_ticket:
        raise HTTPException(status_code=404, detail="Ticket not found")

    # Check if the movie exists
    movie = db.query(Movie).filter(Movie.id == ticket.movie_id).first()
    if not movie:
        raise HTTPException(status_code=404, detail="Movie not found")

    # Check if the user exists
    user = db.query(User).filter(User.id == ticket.user_id).first()
    if not user:
        raise HTTPException(status_code=404, detail="User not found")

    # Calculate total price
    seat_count = len(ticket.seat_numbers.split(","))
    total_price = seat_count * movie.price

    # Update ticket details
    db_ticket.seat_numbers = ticket.seat_numbers
    db_ticket.user_id = ticket.user_id
    db_ticket.movie_id = ticket.movie_id
    db_ticket.total_price = total_price
    db.commit()
    db.refresh(db_ticket)
    return db_ticket

# Delete a ticket
@router.delete("/tickets/{ticket_id}")
def delete_ticket(ticket_id: int, db: Session = Depends(get_db)):
    db_ticket = db.query(Ticket).filter(Ticket.id == ticket_id).first()
    if not db_ticket:
        raise HTTPException(status_code=404, detail="Ticket not found")

    db.delete(db_ticket)
    db.commit()
    return {"message": "Ticket deleted successfully"}

# VNPay return endpoint
@router.get("/tickets/vnpay_return/")
def vnpay_return(request: Request, db: Session = Depends(get_db)):
    params = dict(request.query_params)
    is_valid = verify_vnpay_response(params)

    if not is_valid:
        raise HTTPException(status_code=400, detail="Invalid VNPay response")

    ticket_id = int(params.get("vnp_TxnRef"))
    ticket = db.query(Ticket).filter(Ticket.id == ticket_id).first()

    if not ticket:
        raise HTTPException(status_code=404, detail="Ticket not found")

    # Update ticket status based on VNPay response
    if params.get("vnp_ResponseCode") == "00":  # Success
        ticket.status = "paid"
    else:
        ticket.status = "failed"

    db.commit()
    return {"message": "Payment status updated", "status": ticket.status}