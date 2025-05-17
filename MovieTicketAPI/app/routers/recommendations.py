from fastapi import APIRouter, HTTPException, Depends
from sqlalchemy.orm import Session
from app.database import SessionLocal
from app.models import User, Movie, Ticket
from app.schemas import MovieResponse
from app.utils.vector_utils import pinecone_client, generate_vector

router = APIRouter()

def get_db():
    db = SessionLocal()
    try:
        yield db
    finally:
        db.close()

# Reccommandations normal with filter
# @router.get("/recommendations/{user_id}", response_model=list[MovieResponse])
# def get_recommendations(user_id: int, db: Session = Depends(get_db)):
#     # Check if user exists
#     user = db.query(User).filter(User.id == user_id).first()
#     if not user:
#         raise HTTPException(status_code=404, detail="User not found")

#     # Get user's preferences
#     preferences = user.preferences.split(",") if user.preferences else []

#     # Get movies from ticket history instead of purchase history
#     watched_movie_ids = db.query(Ticket.movie_id).filter(
#         Ticket.user_id == user_id,
#         # Ticket.status == "paid"  # Only consider paid tickets
#     ).all()
#     watched_movie_ids = [movie_id[0] for movie_id in watched_movie_ids]

#     # Recommend movies based on preferences and exclude already watched movies
#     recommended_movies = db.query(Movie).filter(
#         Movie.genre.in_(preferences),
#         ~Movie.id.in_(watched_movie_ids)
#     ).all()

#     return recommended_movies

# Get recommendations based on user preferences and exclude watched movies through vector search
@router.get("/recommendations/{user_id}", response_model=list[MovieResponse])
def get_recommendations(user_id: int, db: Session = Depends(get_db)):
    # Check if user exists
    user = db.query(User).filter(User.id == user_id).first()
    if not user:
        raise HTTPException(status_code=404, detail="User not found")

    # Get user's preferences vector
    user_preferences = user.preferences if user.preferences else ""
    print(f"User preferences: {user_preferences}")
    
    # Generate embedding for user preferences
    if user_preferences:
        preference_vector = generate_vector(user_preferences)  # Chuyển đổi chuỗi thành vector
    else:
        # Nếu không có sở thích, trả về danh sách rỗng hoặc xử lý khác
        # return []
        raise HTTPException(status_code=400, detail="User preferences are empty")
    print(f"Preference vector: {preference_vector}")

    # Get watched movies to exclude
    watched_movie_ids = db.query(Ticket.movie_id).filter(
        Ticket.user_id == user_id,
        # Ticket.status == "paid"
    ).all()
    watched_ids = [str(movie_id[0]) for movie_id in watched_movie_ids]
    print(f"Watched movie IDs: {watched_ids}")

    # Get index from Pinecone
    index = pinecone_client.Index("movie-recommendations")

    if len(watched_ids) == 0:
        # If no watched movies
        query_response = index.query(
            # namespace="movies",
            vector=preference_vector,
            top_k=4,
            include_metadata=True,
            include_values=False
        )
    else:   
        # Query Pinecone for similar movies using user preferences
        query_response = index.query(
            # namespace="movies",
            vector=preference_vector,  # Sử dụng vector thay vì chuỗi
            top_k=4,
            include_metadata=True,
            filter={
                "id": {
                    "$nin": watched_ids
                }
            }
        )

    # Get movie IDs from results
    movie_ids = [int(match['id']) for match in query_response['matches']]
    print(f"Recommended movie IDs: {movie_ids}")

    # Get full movie details from database
    recommended_movies = db.query(Movie).filter(
        Movie.id.in_(movie_ids)
    ).order_by(Movie.id.in_(movie_ids)).all()

    return recommended_movies[:4]

