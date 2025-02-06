using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AquariumForum_2.Models;

namespace AquariumForum_2.Data
{
    public class AquariumForum_2Context : DbContext
    {
        public AquariumForum_2Context (DbContextOptions<AquariumForum_2Context> options)
            : base(options)
        {
        }

        public DbSet<AquariumForum_2.Models.Comment> Comment { get; set; } = default!;
        public DbSet<AquariumForum_2.Models.Discussion> Discussion { get; set; } = default!;
    }
}
