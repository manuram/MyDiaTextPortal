using System;
using System.Collections.Generic;
using System.Data.Entity;
using SeniorDesign.Models;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Infrastructure;

namespace SeniorDesign.Models
{
    public class SdContext : DbContext
    {
        public DbSet<ResponseMessage> ResponseMessages { get; set; }
        public DbSet<SmsMessage> SmsMessages { get; set; }
        public DbSet<Rating> Ratings { get; set; }
    }
}