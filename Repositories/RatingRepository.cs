using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SeniorDesign.Models;

namespace SeniorDesign.Repository
{
    public class RatingRepository : IRatingRepository, IDisposable
    {
        private SdContext context;

        public RatingRepository(SdContext context)
        {
            this.context = context;
        }
        public IEnumerable<Rating> GetRatings()
        {
            return context.Ratings;
        }
        public IEnumerable<Rating> GetRatingsByUserName(string username)
        {
            return context.Ratings
                .Where(r => r.username == username);
        }
        //public IEnumerable<Rating> GetRatingsByPhone(string phone)
        //{
        //    // look up phone
        //    return GetRatingsByUserName(username);
        //}
        public void InsertRating(Rating rating)
        {
            context.Ratings.Add(rating);
        }
        public bool LogRating(string username, int rating)
        {
            try 
            {
                Rating r = new Rating(username, rating);
                InsertRating(r);
                Save();
                return true;
            }
            catch
            {
                return false;
            }
            
        }
        //public int GetNumRatingsByPhone(string phone)
        //{
        //    return context.Ratings;
        //}
        public int GetNumRatings(string username)
        {
            return context.Ratings
                        .Where(r => r.username == username)
                        .Count();
        }

        // Other
        public void Save()
        {
            context.SaveChanges();
        }
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


    }
}