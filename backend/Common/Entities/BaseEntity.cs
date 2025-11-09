namespace TalentBridge.Common.Entities;

// ეს პარამეტრები მაინც ყველგან საჭიროა (სტანდარტია) ამიტომ გლობალურად იქმნება. 
public class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}