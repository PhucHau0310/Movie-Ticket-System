using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MovieTicketApp.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Genre { get; set; }
        public int Duration { get; set; }
        [JsonPropertyName("release_date")]
        public string ReleaseDate { get; set; }
        public float Price { get; set; }
        public float? Rating { get; set; }
        [JsonPropertyName("image_url")]
        public string ImageUrl { get; set; }
    }
}
