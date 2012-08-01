using System;
using System.Collections.Generic;
using SeniorDesign.Models;

namespace SeniorDesign.Repository
{
    public interface IRatingRepository : IDisposable
    {
        // Get
        IEnumerable<Rating> GetRatings();
        IEnumerable<Rating> GetRatingsByUserName(string user);
        //IEnumerable<Rating> GetRatingsByPhone(string phone);
        void InsertRating(Rating rating);
        bool LogRating(string username, int rating);
        //int GetNumRatingsByPhone(string phone);
        int GetNumRatings(string username);

        // Other
        void Save();         
    }
}