using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CCAuthServer.Context
{
    public class UserTenant
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public Guid UserDataId { get; set; }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        public virtual UserData UserData { get; set; }

        /// <summary>
        /// Gets or sets the tenant identifier.
        /// </summary>
        /// <value>
        /// The tenant identifier.
        /// </value>
        public Guid TenantId { get; set; }

        /// <summary>
        /// Gets or sets the tenant user identifier (the UserId on the database system, if not the same).
        /// </summary>
        /// <value>
        /// The tenant user identifier.
        /// </value>
        public Guid TenantUserId { get; set; }

        /// <summary>
        /// Configures the entity of type <typeparamref name="TEntity" />.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<UserTenant> builder)
        {
            builder.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            builder.HasIndex(k => new { k.UserDataId, k.TenantId }).HasName("IX_UserDataTenant");
            builder.HasOne(d => d.UserData)
                .WithMany(p => p.UserTenant)
                .HasForeignKey(d => d.UserDataId)
                .HasConstraintName("FK_dbo.UserData_dbo.UserTenant_UserDataId");
        }
    }
}
