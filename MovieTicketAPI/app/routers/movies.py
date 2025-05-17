from fastapi import APIRouter, HTTPException, Depends
from sqlalchemy.orm import Session
from app.database import SessionLocal
from app.models import Movie
from app.schemas import MovieCreate, MovieResponse
from app.utils.vector_utils import generate_vector, save_vector_to_pinecone, delete_vector_from_pinecone

router = APIRouter()

def get_db():
    db = SessionLocal()
    try:
        yield db
    finally:
        db.close()

@router.post("/movies/", response_model=MovieResponse)
def create_movie(movie: MovieCreate, db: Session = Depends(get_db)):
    # Check if movie already exists
    existing_movie = db.query(Movie).filter(Movie.title == movie.title).first()
    if existing_movie:
        raise HTTPException(status_code=400, detail="Movie already exists")

    # Combine multiple fields to generate a vector
    combined_text = (
        f"{movie.title}. {movie.description}. Genre: {movie.genre}. "
        f"Release Date: {movie.release_date}. Price: {movie.price}. Rating: {movie.rating}."
        f" Duration: {movie.duration} minutes."
    )
    vector = generate_vector(combined_text)

    # Save movie to database
    db_movie = Movie(**movie.dict(), vector=str(vector))
    db.add(db_movie)
    db.commit()
    db.refresh(db_movie)

    # Save vector to Pinecone
    save_vector_to_pinecone(db_movie.id, vector)

    return db_movie

@router.get("/movies/{movie_id}", response_model=MovieResponse)
def get_movie(movie_id: int, db: Session = Depends(get_db)):
    movie = db.query(Movie).filter(Movie.id == movie_id).first()
    if not movie:
        raise HTTPException(status_code=404, detail="Movie not found")
    return movie

@router.get("/movies/", response_model=list[MovieResponse])
def list_movies(skip: int = 0, limit: int = 10, db: Session = Depends(get_db)):
    movies = db.query(Movie).offset(skip).limit(limit).all()
    return movies

# @router.put("/movies/{movie_id}", response_model=MovieResponse)
# def update_movie(movie_id: int, movie_update: MovieUpdate, db: Session = Depends(get_db)):
#     movie = db.query(Movie).filter(Movie.id == movie_id).first()
#     if not movie:
#         raise HTTPException(status_code=404, detail="Movie not found")

#     # Update movie fields
#     for key, value in movie_update.dict(exclude_unset=True).items():
#         setattr(movie, key, value)

#     # Regenerate vector if necessary
#     if any(field in movie_update.dict(exclude_unset=True) for field in ["title", "description", "genre", "release_date", "price", "rating", "duration"]):
#         combined_text = (
#             f"{movie.title}. {movie.description}. Genre: {movie.genre}. "
#             f"Release Date: {movie.release_date}. Price: {movie.price}. Rating: {movie.rating}."
#             f" Duration: {movie.duration} minutes."
#         )
#         vector = generate_vector(combined_text)
#         movie.vector = str(vector)
#         save_vector_to_pinecone(movie.id, vector)

#     db.commit()
#     db.refresh(movie)
#     return movie

@router.delete("/movies/{movie_id}", response_model=dict)
def delete_movie(movie_id: int, db: Session = Depends(get_db)):
    movie = db.query(Movie).filter(Movie.id == movie_id).first()
    if not movie:
        raise HTTPException(status_code=404, detail="Movie not found")

    # Delete vector from Pinecone
    delete_vector_from_pinecone(movie.id)

    # Delete movie from database
    db.delete(movie)
    db.commit()
    return {"message": "Movie deleted successfully"}