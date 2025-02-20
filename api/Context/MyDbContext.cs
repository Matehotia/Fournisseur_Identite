using Microsoft.EntityFrameworkCore;
using App.Models;

namespace App.Context;

public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> o) : base(o){}

    public DbSet<User> Users {get; set;}

    public DbSet<NumberAttempt> NumberAttempts {get; set;}
    public DbSet<Attempt> Attempts {get; set;}

    public DbSet<PinExpiration> PinExpirations {get; set;}
    public DbSet<Authentification> Auths {get; set;}

    public DbSet<SessionExpiration> SessionExpirations {get; set;}
    public DbSet<Session> Sessions {get; set;} 
}
