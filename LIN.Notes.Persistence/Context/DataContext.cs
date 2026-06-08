using Microsoft.EntityFrameworkCore;

namespace LIN.Notes.Persistence.Context;

/// <summary>
/// Nuevo contexto a la base de datos
/// </summary>
public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<ProfileModel> Profiles { get; set; }
    public DbSet<NoteDataModel> Notes { get; set; }
    public DbSet<NoteAccessDataModel> AccessNotes { get; set; }
    public DbSet<MovementDataModel> Movements { get; set; }
    public DbSet<TaskDataModel> Tasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        // Indices y identidad
        modelBuilder.Entity<ProfileModel>()
           .HasIndex(e => e.AccountId)
           .IsUnique();

        modelBuilder.Entity<NoteAccessDataModel>()
                          .HasOne(t => t.Note)
                          .WithMany(t => t.UsersAccess)
                          .HasForeignKey(y => y.NoteId)
                          .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<NoteAccessDataModel>()
                        .HasOne(t => t.Profile)
                        .WithMany()
                        .HasForeignKey(y => y.ProfileID)
                        .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<NoteDataModel>()
                        .HasMany(t => t.Tasks)
                        .WithOne(t => t.Note)
                        .HasForeignKey(y => y.NoteId)
                        .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<MovementDataModel>()
                       .HasOne(t => t.Profile)
                       .WithMany()
                       .HasForeignKey(y => y.ProfileId)
                       .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<TaskDataModel>()
                    .HasOne(t => t.Note)
                    .WithMany(t => t.Tasks)
                    .HasForeignKey(y => y.NoteId)
                    .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<ProfileModel>().ToTable("profiles");
        modelBuilder.Entity<NoteDataModel>().ToTable("notes");
        modelBuilder.Entity<NoteAccessDataModel>().ToTable("note_access");
    }
}