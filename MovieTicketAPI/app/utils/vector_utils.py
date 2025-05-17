import os
from pinecone import Pinecone, ServerlessSpec
from transformers import AutoTokenizer, AutoModel
import torch

# Load environment variables
from dotenv import load_dotenv
load_dotenv()

# Initialize Pinecone
pinecone_client = Pinecone(api_key=os.getenv("PINECONE_API_KEY"))

# Define Pinecone index
INDEX_NAME = os.getenv("PINECONE_INDEX_NAME", "movie-tickets-index")
if INDEX_NAME not in pinecone_client.list_indexes().names():
    pinecone_client.create_index(
        name=INDEX_NAME,
        dimension=512,  # Correct dimensions for Hugging Face embeddings
        metric="cosine",
        spec=ServerlessSpec(cloud="aws", region=os.getenv("PINECONE_ENVIRONMENT"))
    )
index = pinecone_client.Index(INDEX_NAME)

# Load Hugging Face model and tokenizer
MODEL_NAME = "sentence-transformers/all-MiniLM-L6-v2"
tokenizer = AutoTokenizer.from_pretrained(MODEL_NAME)
model = AutoModel.from_pretrained(MODEL_NAME)

def generate_vector(text: str) -> list:
    """Generate a vector embedding for the given text using Hugging Face."""
    tokens = tokenizer(text, return_tensors="pt", truncation=True, padding=True, max_length=512)
    with torch.no_grad():
        embeddings = model(**tokens).last_hidden_state.mean(dim=1)
    return embeddings.squeeze().tolist()

def save_vector_to_pinecone(movie_id: int, vector: list):
    """Save the vector to Pinecone with the movie ID as the unique key."""
    index.upsert([(str(movie_id), vector)])

def delete_vector_from_pinecone(movie_id: int):
    """Delete the vector from Pinecone using the movie ID as the unique key."""
    index.delete(ids=[str(movie_id)])