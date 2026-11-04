using Hpuni.Domain;
using Microsoft.EntityFrameworkCore;

namespace Hpuni.Infrastructure;

public static class NotificationsConfiguration
{
    public static void ConfigureNotifications(this ModelBuilder b)
    {
        b.Entity<NotificationReadState>(e=>{
            e.ToTable("NotificationReadStates");e.HasKey(x=>new{x.ActorId,x.SourceKey});
            e.Property(x=>x.SourceKey).HasMaxLength(120);
            e.Property(x=>x.SourceVersion).HasMaxLength(8).IsRequired();
            e.HasOne<ApplicationUser>().WithMany().HasForeignKey(x=>x.ActorId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
