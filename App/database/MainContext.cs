﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vietjet_series_booking_dotnet.Modules.Intelisys.Entities;


namespace vietjet_series_booking_dotnet.App.database
{
    public class MainContext : DbContext
    {
        public MainContext(DbContextOptions<MainContext> options) : base(options)
        {

        }
        public DbSet<User_Permissions> user_permissions { get; set; }
        public DbSet<Token> tokens { get; set; }
        public DbSet<Log> logs { get; set; }
       
    }
}
