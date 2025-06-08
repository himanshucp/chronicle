using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Chronicle.Web
{
    //public class ApplicationDbContext
    //{
    //    //private readonly string _connectionString;

    //    //public ApplicationDbContext(IConfiguration configuration)
    //    //{
    //    //    _connectionString = configuration.GetConnectionString("DefaultConnection");
    //    //}

    //    //// Creates and returns a new SQL connection
    //    //public IDbConnection CreateConnection()
    //    //{
    //    //    return new SqlConnection(_connectionString);
    //    //}
    //}

    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
    }
}
