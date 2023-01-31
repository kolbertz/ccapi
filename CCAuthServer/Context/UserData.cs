using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CCAuthServer.Context
{
    public class UserData
    {
        public UserData()
        {
            UserTenant = new HashSet<UserTenant>();
        }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>
        /// The language.
        /// </value>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the culture.
        /// </summary>
        /// <value>
        /// The culture.
        /// </value>
        public string Culture { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        /// <value>
        /// The comment.
        /// </value>
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the user image.
        /// </summary>
        /// <value>
        /// The user image.
        /// </value>
        public string UserImage { get; set; }

        /// <summary>
        /// Gets or sets the is approved.
        /// </summary>
        /// <value>
        /// The is approved.
        /// </value>
        public bool? IsApproved { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is blocked.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is blocked; otherwise, <c>false</c>.
        /// </value>
        public bool IsBlocked { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating that the user is inactive (normally "deleted").
        /// </summary>
        /// <value>
        /// The is inactive.
        /// </value>
        public bool? IsInactive { get; set; }

        /// <summary>
        /// Gets or sets the display name for the user from real name to email down to card id.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the e-mail-address (also login name).
        /// </summary>
        /// <value>
        /// The e mail.
        /// </value>
        public string EMail { get; set; }

        /// <summary>
        /// Gets or sets the encoded password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the old salt key (for transfers of users from old systems).
        /// </summary>
        /// <value>
        /// The old salt key.
        /// </value>
        public string OldSaltKey { get; set; }

        /// <summary>
        /// Gets or sets date when it was created.
        /// </summary>
        /// <value>
        /// The date when it was created.
        /// </value>
        public DateTimeOffset CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the user which has created the database entry
        /// </summary>
        /// <value>
        /// The user which has created the database entry
        /// </value>
        public Guid CreatedUser { get; set; }

        /// <summary>
        /// Gets or sets the date of the last update
        /// </summary>
        /// <value>
        /// The date of the last update
        /// </value>
        public DateTimeOffset LastUpdatedDate { get; set; }

        /// <summary>
        /// Gets or sets the user which has made the last update.
        /// </summary>
        /// <value>
        /// The user which has made the last update.
        /// </value>
        public Guid LastUpdatedUser { get; set; }

        /// <summary>
        /// Gets or sets the last log on date.
        /// </summary>
        /// <value>
        /// The last log on date.
        /// </value>
        public DateTimeOffset? LastLogOnDate { get; set; }

        /// <summary>
        /// Gets or sets the failed password attempt count.
        /// </summary>
        /// <value>
        /// The failed password attempt count.
        /// </value>
        public int? FailedPasswordAttemptCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is locked out.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is locked out; otherwise, <c>false</c>.
        /// </value>
        public bool? IsLockedOut { get; set; }

        /// <summary>
        /// Gets or sets the last lockout date.
        /// </summary>
        /// <value>
        /// The last lockout date.
        /// </value>
        public DateTimeOffset? LastLockoutDate { get; set; }

        /// <summary>
        /// Gets or sets the password token.
        /// </summary>
        /// <value>
        /// The password token.
        /// </value>
        public string PasswordToken { get; set; }

        /// <summary>
        /// Gets or sets the password token expiration.
        /// </summary>
        /// <value>
        /// The password token expiration.
        /// </value>
        public DateTimeOffset? PasswordTokenExpiration { get; set; }

        /// <summary>
        /// Gets or sets the user tenants (systems the user has access to).
        /// </summary>
        /// <value>
        /// The user tenants.
        /// </value>
        public virtual ICollection<UserTenant> UserTenant { get; set; }

        /// <summary>
        /// Configures the entity of type <typeparamref name="TEntity" />.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<UserData> builder)
        {
            builder.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");

            builder.Property(e => e.Culture).HasMaxLength(10);

            builder.Property(e => e.FirstName).HasMaxLength(50);

            builder.Property(e => e.Language).HasMaxLength(10);

            builder.Property(e => e.LastName).HasMaxLength(50);

            builder.Property(e => e.UserImage).IsUnicode(false);
        }
    }
}
