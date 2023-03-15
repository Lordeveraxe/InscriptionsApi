using System;
using System.Collections.Generic;
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

    public virtual DbSet<Inscription> Inscriptions { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Inscription>(entity =>
        {
            entity.HasKey(e => e.IncriptionId).HasName("PK__Inscript__F00DD83E4FC8C55F");

            entity.Property(e => e.IncriptionId).HasColumnName("incription_id");
            entity.Property(e => e.IncriptionDate)
                .HasColumnType("smalldatetime")
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
            entity.HasKey(e => e.StudentId).HasName("PK__Students__2A33069A970B06CC");

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
            entity.Property(e => e.StudentStatus).HasColumnName("student_status");
            entity.Property(e => e.TypeDocStudent)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("type_doc_student");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.SubjectId).HasName("PK__Subjects__5004F66050DB088D");

            entity.Property(e => e.SubjectId).HasColumnName("subject_id");
            entity.Property(e => e.SubjectCapacity).HasColumnName("subject_capacity");
            entity.Property(e => e.SubjectName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("subject_name");
            entity.Property(e => e.SubjectStatus).HasColumnName("subject_status");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
