using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UNIOOP.App.Models;

namespace UNIOOP.APP.Data.Configurations
{
    public class GovernmentOfficerConfiguration : IEntityTypeConfiguration<GovernmentOfficer>
    {
        public void Configure(EntityTypeBuilder<GovernmentOfficer> builder)
        {
            builder.ToTable("GovernmentOfficer");

            builder.Property(o => o.OfficerID).UseIdentityByDefaultColumn();
            builder.HasIndex(o => o.OfficerID).IsUnique().HasDatabaseName("UX_GovernmentOfficer_OfficerID");
        }
    }
}