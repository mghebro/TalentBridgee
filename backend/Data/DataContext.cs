using Microsoft.EntityFrameworkCore;
using TalentBridge.Enums.OrganizationTypes;
using TalentBridge.Models;
using TalentBridge.Models.Auth;
using TalentBridge.Models.Roles;
using TalentBridge.Models.Recruitment;
using TalentBridge.Models.UserRelated;
using TalentBridge.Models.Communication;
using TalentBridge.Models.Analytics;
using TalentBridge.Models.Testing;

namespace TalentBridge.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }
    
    // Auth
    public DbSet<User> Users { get; set; }
    public DbSet<EmailVerification> EmailVerifications { get; set; }
    public DbSet<PasswordVerification> PasswordVerifications { get; set; }
    
    // Roles
    public DbSet<HRManager> HrManagers { get; set; }
    
    //Communication
    public DbSet<Message> Messages { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    
    // Recruitment
    public DbSet<Application> Applications { get; set; }
    public DbSet<ApplicationTimeline> ApplicationTimelines { get; set; }
    public DbSet<Vacancy> Vacancies { get; set; }
    public DbSet<SavedVacancy> SavedVacancies { get; set; }
    
    //testing
    public DbSet<Test> Tests { get; set; }
    public DbSet<TestAssignment> TestAssignments { get; set; }
    public DbSet<SubmissionAnswer> SubmissionAnswers { get; set; }
    public DbSet<TestSubmission> TestSubmissions { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<QuestionOption> QuestionOptions { get; set; }
    public DbSet<SelectedQuestionOption> SelectedQuestionOptions { get; set; }
    
    //UserRelated
    public DbSet<Education> Educations { get; set; }
    public DbSet<Experience> Experiences { get; set; }
    public DbSet<UserDetails> UserDetails { get; set; }
    
    //Analytics
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<VacancyView> VacancyViews { get; set; }
    
    //Organizations
    public DbSet<Organization> Organizations { get; set; } 
  
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Vacancy>()
            .HasOne(v => v.Test)
            .WithOne(t => t.Vacancy)
            .HasForeignKey<Test>(t => t.VacancyId);
            
        modelBuilder.Entity<Application>()
            .HasOne(a => a.TestAssignment)
            .WithOne(t => t.Application)
            .HasForeignKey<TestAssignment>(t => t.ApplicationId);

        modelBuilder.Entity<SelectedQuestionOption>()
            .HasKey(sqo => new { sqo.SubmissionAnswerId, sqo.QuestionOptionId });

        modelBuilder.Entity<SelectedQuestionOption>()
            .HasOne(sqo => sqo.SubmissionAnswer)
            .WithMany(sa => sa.SelectedOptions)
            .HasForeignKey(sqo => sqo.SubmissionAnswerId);

        modelBuilder.Entity<SelectedQuestionOption>()
            .HasOne(sqo => sqo.QuestionOption)
            .WithMany()
            .HasForeignKey(sqo => sqo.QuestionOptionId);
        
        foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                     .SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }
}