using Kunlun.LPS.Worker.Core.Domain.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kunlun.LPS.Worker.Data.Mapping
{
    class RegisterPointsRuleMembershipCardLevelDetailMap : IEntityTypeConfiguration<RegisterPointsRuleMembershipCardLevelDetail>
    {
        public void Configure(EntityTypeBuilder<RegisterPointsRuleMembershipCardLevelDetail> builder)
        {
            builder.ToTable("LPS_RegisterPointsRuleCardLevelDetail");

            builder.HasKey(c => c.Id);

            builder.Property(t => t.InsertUser).HasColumnName("Insert_User");
            builder.Property(t => t.InsertDate).HasColumnName("Insert_Date");
            builder.Property(t => t.UpdateUser).HasColumnName("Update_User");
            builder.Property(t => t.UpdateDate).HasColumnName("Update_Date");

            builder.Property(t => t.Points).HasColumnType("decimal(18,2)");
        }
    }
}
