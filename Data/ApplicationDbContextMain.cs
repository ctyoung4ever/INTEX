using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace INTEX.Data
{
    public class ApplicationDbContextMain : IdentityDbContext
    {
        public ApplicationDbContextMain(DbContextOptions<ApplicationDbContextMain> options)
            : base(options)
        {
        }
    }
}
