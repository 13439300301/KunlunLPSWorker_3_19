using Kunlun.LPS.Worker.Core.Domain.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kunlun.LPS.Worker.Data.Mapping
{
    class RegisterPointsRuleMemberSourceMapMap : IEntityTypeConfiguration<RegisterPointsRuleMemberSourceMap>
    {
        public void Configure(EntityTypeBuilder<RegisterPointsRuleMemberSourceMap> builder)
        {
            builder.ToTable("LPS_RegisterPointsRule_Source_Map");

            builder.HasKey(c => c.Id);

            builder.Property(t => t.InsertUser).HasColumnName("Insert_User");
            builder.Property(t => t.InsertDate).HasColumnName("Insert_Date");
            builder.Property(t => t.UpdateUser).HasColumnName("Update_User");
            builder.Property(t => t.UpdateDate).HasColumnName("Update_Date");
        }
    }
}
