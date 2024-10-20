using Microsoft.EntityFrameworkCore;

namespace LIN.Notes.Persistence.Context;

/// <summary>
/// Nuevo contexto a la base de datos
/// </summary>
public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{

    /// <summary>
    /// Tabla de perfiles.
    /// </summary>
    public DbSet<ProfileModel> Profiles { get; set; }


    /// <summary>
    /// Notas.
    /// </summary>
    public DbSet<NoteDataModel> Notes { get; set; }


    /// <summary>
    /// Acceso a los Notes
    /// </summary>
    public DbSet<NoteAccessDataModel> AccessNotes { get; set; }


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

        modelBuilder.Entity<ProfileModel>().ToTable("profiles");
        modelBuilder.Entity<NoteDataModel>().ToTable("notes");
        modelBuilder.Entity<NoteAccessDataModel>().ToTable("note_access");
    }

}