using System;
using System.ComponentModel.DataAnnotations;

namespace SeniorDesign.Models
{
    public class Rating
    {
        public Rating()
        {
        }
        public Rating(string username, int points)
        {
            this.username = username;
            this.rating = points;
            this.timestamp = DateTime.Now.ToUniversalTime().Subtract(
                TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time").BaseUtcOffset);
        }

        [Key]
        public int ratingId { get; set; }
        public int rating { get; set; }
        public DateTime timestamp { get; set; }
        public string username { get; set; }
        public virtual string timeString 
        {
            get { return timestamp.ToShortDateString(); }
        }
    }
}