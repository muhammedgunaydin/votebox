using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using vote_box.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<PollModel> Polls { get; set; }
    public DbSet<OptionModel> Options { get; set; }
    public DbSet<VoteModel> Votes { get; set; }
    public DbSet<UserModel> Users { get; set; }
}