﻿using Microsoft.EntityFrameworkCore;


namespace OrderService
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

    }
}
