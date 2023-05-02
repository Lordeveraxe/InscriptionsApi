using System;
using System.Collections.Generic;
using InscriptionsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace InscriptionsApiLocal.Models;

public partial class InscriptionsUniversityContext : DbContext
{
    public InscriptionsUniversityContext()
    {
    }

    public InscriptionsUniversityContext(DbContextOptions<InscriptionsUniversityContext> options)
        : base(options)
    {
    }
    public virtual DbSet<Credential> Credentials { get; set; }

    public virtual DbSet<Inscription> Inscriptions { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Credential>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Credenti__B9BE370F4283B2B9");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            entity.Property(e => e.CredentialSalt)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("credential_salt");
            entity.Property(e => e.UserPassword)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("user_password");

            entity.HasOne(d => d.User).WithOne(p => p.Credential)
                .HasForeignKey<Credential>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Credentia__user___4F7CD00D");
        });

        modelBuilder.Entity<Inscription>(entity =>
        {
            entity.HasKey(e => e.IncriptionId).HasName("PK__Inscript__F00DD83EA5FF4E37");

            entity.Property(e => e.IncriptionId).HasColumnName("incription_id");
            entity.Property(e => e.IncriptionDate)
                .HasColumnType("datetime")
                .HasColumnName("incription_date");
            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.SubjectId).HasColumnName("subject_id");

            entity.HasOne(d => d.Student).WithMany(p => p.Inscriptions)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inscripti__stude__3E52440B");

            entity.HasOne(d => d.Subject).WithMany(p => p.Inscriptions)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inscripti__subje__3F466844");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__Students__2A33069A6AD7EE57");

            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.StudentDoc)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("student_doc");
            entity.Property(e => e.StudentGenre)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("student_genre");
            entity.Property(e => e.StudentLn)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("student_ln");
            entity.Property(e => e.StudentName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("student_name");
            entity.Property(e => e.StudentPhoto)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("student_photo");
            entity.Property(e => e.StudentStatus).HasColumnName("student_status");
            entity.Property(e => e.TypeDocStudent)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("type_doc_student");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.SubjectId).HasName("PK__Subjects__5004F6605C22DA83");

            entity.Property(e => e.SubjectId).HasColumnName("subject_id");
            entity.Property(e => e.SubjectCapacity).HasColumnName("subject_capacity");
            entity.Property(e => e.SubjectName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("subject_name");
            entity.Property(e => e.SubjectStatus).HasColumnName("subject_status");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__B9BE370FD67A12EA");

            entity.HasIndex(e => e.UserName, "uc_Users").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.UserEmail)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("user_email");
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("user_name");
            entity.Property(e => e.UserState).HasColumnName("user_state");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
