using CCUserService.DTOs;

namespace CCUserService.Data
{
    public class InternalUser
    {
        public Guid Id { get; set; } 

        public DateTimeOffset CreatedDate { get; set; }

        public Guid CreatedUser { get; set; }

        public DateTimeOffset LastUpdatedDate { get; set; }

        public Guid LastUpdatedUser { get; set; }

        public Guid UserGroupId { get; set; }

      

        public InternalUser(User user) :base()
        {
            MergeUser(user);
            
        }

        public void MergeUser(User user)
        {
            Id = user.Id;
            UserGroupId = user.UserGroupId;
        }


    }
}
