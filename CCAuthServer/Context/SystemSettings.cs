using Azure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Identity.Client;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace CCAuthServer.Context
{
    public class SystemSettings
    {
        public SystemSettings()
        {
        }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public int Key { get; set; }

        /// <summary>
        /// Gets or sets the distributor identifier.
        /// </summary>
        /// <value>
        /// The distributor identifier.
        /// </value>
        public int DistributorId { get; set; }

        /// <summary>
        /// Gets or sets the name of the internal.
        /// </summary>
        /// <value>
        /// The name of the internal.
        /// </value>
        public string InternalName { get; set; }

        /// <summary>
        /// Gets or sets the internal comment.
        /// </summary>
        /// <value>
        /// The internal comment.
        /// </value>
        public string InternalComment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is blocked.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is blocked; otherwise, <c>false</c>.
        /// </value>
        public bool IsBlocked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is hosted.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is hosted; otherwise, <c>false</c>.
        /// </value>
        public bool IsHosted { get; set; }

        /// <summary>
        /// Gets or sets the type of the system.
        /// </summary>
        /// <value>
        /// The type of the system.
        /// </value>
        public int SystemType { get; set; }

        /// <summary>
        /// Gets or sets the license identifier.
        /// </summary>
        /// <value>
        /// The license identifier.
        /// </value>
        public Guid LicenseId { get; set; }

        /// <summary>
        /// Gets or sets the last license check.
        /// </summary>
        /// <value>
        /// The last license check.
        /// </value>
        public DateTimeOffset? LastLicenseCheck { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        /// <value>
        /// The comment.
        /// </value>
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the address name1.
        /// </summary>
        /// <value>
        /// The address name1.
        /// </value>
        public string AddressName1 { get; set; }

        /// <summary>
        /// Gets or sets the address name2.
        /// </summary>
        /// <value>
        /// The address name2.
        /// </value>
        public string AddressName2 { get; set; }

        /// <summary>
        /// Gets or sets the address street.
        /// </summary>
        /// <value>
        /// The address street.
        /// </value>
        public string AddressStreet { get; set; }

        /// <summary>
        /// Gets or sets the address country.
        /// </summary>
        /// <value>
        /// The address country.
        /// </value>
        public string AddressCountry { get; set; }

        /// <summary>
        /// Gets or sets the state of the address.
        /// </summary>
        /// <value>
        /// The state of the address.
        /// </value>
        public string AddressState { get; set; }

        /// <summary>
        /// Gets or sets the address postal code.
        /// </summary>
        /// <value>
        /// The address postal code.
        /// </value>
        public string AddressPostalCode { get; set; }

        /// <summary>
        /// Gets or sets the address city.
        /// </summary>
        /// <value>
        /// The address city.
        /// </value>
        public string AddressCity { get; set; }

        /// <summary>
        /// Gets or sets the tax number.
        /// </summary>
        /// <value>
        /// The tax number.
        /// </value>
        public string TaxID { get; set; }


        /// <summary>
        /// Gets or sets the tax number.
        /// </summary>
        /// <value>
        /// The tax number.
        /// </value>
        public string TradeID { get; set; }

        /// <summary>
        /// Gets or sets the default time zone.
        /// </summary>
        /// <value>
        /// The default time zone.
        /// </value>
        public string DefaultTimeZone { get; set; }

        /// <summary>
        /// Gets or sets the maximum custom currency exchange rate difference.
        /// </summary>
        /// <value>
        /// The maximum custom currency exchange rate difference.
        /// </value>
        public short MaxCustomCurrencyExchangeRateDiff { get; set; }

        /// <summary>
        /// Gets or sets the minimum price unit.
        /// </summary>
        /// <value>
        /// The minimum price unit.
        /// </value>
        public decimal MinPriceUnit { get; set; }

        /// <summary>
        /// Gets or sets the default theme.
        /// </summary>
        /// <value>
        /// The default theme.
        /// </value>
        public string DefaultTheme { get; set; }

        /// <summary>
        /// Gets or sets the no delete range.
        /// </summary>
        /// <value>
        /// The no delete range.
        /// </value>
        public int NoDeleteRange { get; set; }

        /// <summary>
        /// Gets or sets the name of the mail sender.
        /// </summary>
        /// <value>
        /// The name of the mail sender.
        /// </value>
        public string MailSenderName { get; set; }

        /// <summary>
        /// Gets or sets the mail sender address.
        /// </summary>
        /// <value>
        /// The mail sender address.
        /// </value>
        public string MailSenderAddress { get; set; }

        /// <summary>
        /// Gets or sets the mail smtpserver.
        /// </summary>
        /// <value>
        /// The mail smtpserver.
        /// </value>
        public string MailSMTPServer { get; set; }

        /// <summary>
        /// Gets or sets the mail SMTP server port.
        /// </summary>
        /// <value>
        /// The mail SMTP server port.
        /// </value>
        public int? MailSMTPServerPort { get; set; }

        /// <summary>
        /// Gets or sets the mail SMTP log on.
        /// </summary>
        /// <value>
        /// The mail SMTP log on.
        /// </value>
        public bool? MailSMTPLogOn { get; set; }

        /// <summary>
        /// Gets or sets the mail SMTP enable SSL.
        /// </summary>
        /// <value>
        /// The mail SMTP enable SSL.
        /// </value>
        public bool? MailSMTPEnableSSL { get; set; }

        /// <summary>
        /// Gets or sets the name of the mail SMTP user.
        /// </summary>
        /// <value>
        /// The name of the mail SMTP user.
        /// </value>
        public string MailSMTPUserName { get; set; }

        /// <summary>
        /// Gets or sets the mail SMTP password.
        /// </summary>
        /// <value>
        /// The mail SMTP password.
        /// </value>
        public string MailSMTPPassword { get; set; }

        /// <summary>
        /// Gets or sets date when it was created.
        /// </summary>
        /// <value>
        /// The date when it was created.
        /// </value>
        public DateTimeOffset CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the user who has created the database entry
        /// </summary>
        /// <value>
        /// The user who has created the database entry
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
        /// Gets or sets the user who has made the last update.
        /// </summary>
        /// <value>
        /// The user who has made the last update.
        /// </value>
        public Guid LastUpdatedUser { get; set; }

        /// <summary>
        /// Gets or sets the system logo.
        /// </summary>
        /// <value>
        /// The system logo.
        /// </value>
        public string SystemLogo { get; set; }

        /// <summary>
        /// Gets or sets the branding title.
        /// </summary>
        /// <value>
        /// The branding title.
        /// </value>
        public string BrandingTitle { get; set; }

        /// <summary>
        /// Gets or sets the branding color as a css color string.
        /// </summary>
        /// <value>
        /// The branding title.
        /// </value>
        public string BrandingColor { get; set; }

        /// <summary>
        /// Gets or sets the maintenance level used to shut down several system features for a short or medium maintenance.
        /// At the moment only Revalue is prohibited when MaintenanceLevel is greater than 0.
        /// </summary>
        /// <value>
        /// The maintenance level.
        /// </value>
        public int MaintenanceLevel { get; set; }

        /// <summary>
        /// Gets or sets the contact user identifier.
        /// </summary>
        /// <value>
        /// The contact user identifier.
        /// </value>
        public Guid? ContactUserId { get; set; }


        /// <summary>
        /// Configures the entity of type <typeparamref name="TEntity" />.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<SystemSettings> builder)
        {
            builder.HasIndex(e => e.ContactUserId)
                .HasName("IX_ContactUserId");

            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.Property(e => e.AddressCity)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(e => e.AddressCountry).HasMaxLength(10);

            builder.Property(e => e.AddressName1)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(e => e.AddressName2).HasMaxLength(256);

            builder.Property(e => e.AddressPostalCode)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(e => e.AddressState).HasMaxLength(50);

            builder.Property(e => e.AddressStreet)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(e => e.DefaultTheme).HasMaxLength(50);

            builder.Property(e => e.DefaultTimeZone)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.InternalName)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(e => e.MailSenderAddress).HasMaxLength(50);

            builder.Property(e => e.MailSenderName).HasMaxLength(50);

            builder.Property(e => e.MailSMTPPassword)
                .HasMaxLength(50);

            builder.Property(e => e.MailSMTPServer)
                .HasMaxLength(50);

            builder.Property(e => e.MailSMTPUserName)
                .HasMaxLength(50);

            builder.Property(e => e.MinPriceUnit).HasColumnType("money");

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(256);


            builder.Property(d => d.TaxID).HasMaxLength(50);
            builder.Property(d => d.TradeID).HasMaxLength(50);
        }
    }
}
