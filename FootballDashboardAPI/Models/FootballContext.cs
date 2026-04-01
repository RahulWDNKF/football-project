using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FootballDashboardAPI.Models;

public partial class FootballContext : DbContext
{
    public FootballContext()
    {
    }

    public FootballContext(DbContextOptions<FootballContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Club> Clubs { get; set; }

    public virtual DbSet<ClubContact> ClubContacts { get; set; }

    public virtual DbSet<Document> Documents { get; set; }

    public virtual DbSet<Email> Emails { get; set; }

    public virtual DbSet<Note> Notes { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<Player1> Players1 { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<ReviewRating> ReviewRatings { get; set; }

    public virtual DbSet<ReviewSkillDetail> ReviewSkillDetails { get; set; }

    public virtual DbSet<Scout> Scouts { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<Template> Templates { get; set; }

    public virtual DbSet<User> Users { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Server=TORUS-S4\\SQLEXPRESS;Database=Football;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Club>(entity =>
        {
            entity.HasKey(e => e.ClubId).HasName("PK__clubs__BCAD3DD9B66CEB7C");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
        });

        modelBuilder.Entity<ClubContact>(entity =>
        {
            entity.HasKey(e => e.ClubContactId).HasName("PK__club_con__C96ED33340A70A6B");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Club).WithMany(p => p.ClubContacts).HasConstraintName("FK_club_contacts_club");
        });

        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.DocumentId).HasName("PK__document__9666E8AC94ADA730");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Club).WithMany(p => p.Documents)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_documents_club");

            entity.HasOne(d => d.Player).WithMany(p => p.Documents)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_documents_player");
        });

        modelBuilder.Entity<Email>(entity =>
        {
            entity.HasKey(e => e.EmailId).HasName("PK__emails__3FEF87667261BE34");

            entity.Property(e => e.SentAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Club).WithMany(p => p.Emails)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_emails_club");

            entity.HasOne(d => d.Player).WithMany(p => p.Emails)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_emails_player");

            entity.HasOne(d => d.SentByScout).WithMany(p => p.Emails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_emails_sent_by");
        });

        modelBuilder.Entity<Note>(entity =>
        {
            entity.HasKey(e => e.NoteId).HasName("PK__notes__CEDD0FA4F35B3DBF");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Club).WithMany(p => p.Notes)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_notes_club");

            entity.HasOne(d => d.CreatedByScout).WithMany(p => p.Notes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_notes_created_by");

            entity.HasOne(d => d.Player).WithMany(p => p.Notes)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_notes_player");
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__players__3213E83F515E58D2");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<Player1>(entity =>
        {
            entity.HasKey(e => e.PlayerId).HasName("PK__players__44DA120C27C34F14");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.AgentScout).WithMany(p => p.Player1s)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_players_agent_scout");

            entity.HasOne(d => d.CurrentClub).WithMany(p => p.Player1s).HasConstraintName("FK_players_current_club");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__reviews__60883D903C67CFE4");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Club1).WithMany(p => p.ReviewClub1s).HasConstraintName("FK_reviews_club1");

            entity.HasOne(d => d.Club2).WithMany(p => p.ReviewClub2s).HasConstraintName("FK_reviews_club2");

            entity.HasOne(d => d.Player).WithMany(p => p.Reviews).HasConstraintName("FK_reviews_player");

            entity.HasOne(d => d.Scout).WithMany(p => p.Reviews)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_reviews_scout");
        });

        modelBuilder.Entity<ReviewRating>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__review_r__60883D907EE36ADC");

            entity.HasOne(d => d.Review).WithOne(p => p.ReviewRating).HasConstraintName("FK_review_ratings_review");
        });

        modelBuilder.Entity<ReviewSkillDetail>(entity =>
        {
            entity.HasOne(d => d.Review).WithMany(p => p.ReviewSkillDetails).HasConstraintName("FK_review_skill_details_review");
        });

        modelBuilder.Entity<Scout>(entity =>
        {
            entity.HasKey(e => e.ScoutId).HasName("PK__scouts__FD9E70769C2D32C6");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PK__tasks__0492148D5F4D1B77");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.AssignedToScout).WithMany(p => p.Tasks)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tasks_assigned_to");

            entity.HasOne(d => d.Club).WithMany(p => p.Tasks)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_tasks_club");

            entity.HasOne(d => d.Player).WithMany(p => p.Tasks)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_tasks_player");
        });

        modelBuilder.Entity<Template>(entity =>
        {
            entity.HasKey(e => e.TemplateId).HasName("PK__template__BE44E07988181BD8");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__users__3213E83F72DFFB95");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
