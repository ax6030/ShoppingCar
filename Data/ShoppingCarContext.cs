using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShoppingCar.Models;

namespace ShoppingCar.Data
{
    public class ShoppingCarContext : DbContext
    {
        public ShoppingCarContext (DbContextOptions<ShoppingCarContext> options)
            : base(options)
        {
        }
        
        public DbSet<ShoppingCar.Models.Products> Products { get; set; } = default!;
        public DbSet<ShoppingCar.Models.Category> Category { get; set; }
    }
}
