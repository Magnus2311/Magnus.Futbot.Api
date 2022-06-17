using System.Security.Principal;
using MongoDB.Bson;

namespace Magnus.Futbot.Api.Helpers
{
    public class CustomPrincipal : IPrincipal
    {
        public IIdentity? Identity { get; private set; }

        public CustomPrincipal(string userId)
        {
            this.Identity = new GenericIdentity(userId);
            UserId = new ObjectId(userId);
        }

        public bool IsInRole(string role) => false;

        public ObjectId UserId { get; private set; }
    }
}