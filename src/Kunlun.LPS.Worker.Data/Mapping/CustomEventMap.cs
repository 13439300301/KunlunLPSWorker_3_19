using Kunlun.LPS.Worker.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Data.Mapping
{
    class CustomEventMap : IEntityTypeConfiguration<CustomEvent>
    {
        public void Configure(EntityTypeBuilder<CustomEvent> builder)
        {
            builder.ToTable("custom_event");

            builder.HasKey(c => c.Id);

            builder.Property(t => t.EventCode).HasColumnName("event_code");
            builder.Property(t => t.TemplateId).HasColumnName("template_id");
            builder.Property(t => t.StepId).HasColumnName("step_id");
            builder.Property(t => t.LanguageCode).HasColumnName("language_code");

            builder.Property(t => t.InsertUser).HasColumnName("Insert_User");
            builder.Property(t => t.InsertDate).HasColumnName("Insert_Date");
            builder.Property(t => t.UpdateUser).HasColumnName("Update_User");
            builder.Property(t => t.UpdateDate).HasColumnName("Update_Date");

            builder.HasMany(t => t.CustomEventDetail).WithOne(t => t.CustomEvent).HasForeignKey(t => t.EventId);
        }
    }
}
