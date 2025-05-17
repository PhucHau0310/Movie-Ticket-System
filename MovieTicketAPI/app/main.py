from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware
from app.routers import movies, users, tickets, recommendations, purchase_history

app = FastAPI(
    title="Movie Ticket System API",
    version="1.0.0",
    description="API for Movie Ticket System",
    contact={
        "name": "Nguyễn Phúc Hậu",
        "url": "https://my-portfolio.hau.io.vn",
        "email": "haunhpr024@gmail.com"
    }
)

origins = ["http://localhost:3000"]

app.add_middleware(
CORSMiddleware,
allow_origins=origins,
allow_credentials=True,
allow_methods=["*"],
allow_headers=["*"],
)

# Root endpoint
@app.get("/", 
    summary="API Home",
    description="Endpoint to check if API is running")
async def read_root():
    return {
        "message": "Movie Ticket System API",
        "version": "1.0.0",
        "status": "running",
        "endpoints": {
            "management account": "/api/v1/account",
            "management movie": "/api/v1/movie",
            "management ticket": "/api/v1/ticket",
        }
    }

# Include routers
app.include_router(users.router, prefix="/api/v1", tags=["users"])
app.include_router(movies.router, prefix="/api/v1", tags=["movies"])
app.include_router(tickets.router, prefix="/api/v1", tags=["tickets"])
app.include_router(recommendations.router, prefix="/api/v1", tags=["recommendations"])
app.include_router(purchase_history.router, prefix="/api/v1", tags=["purchase-history"])
