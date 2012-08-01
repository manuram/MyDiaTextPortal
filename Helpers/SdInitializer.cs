using System.Data.Entity;
using System.Web.Security;

namespace SeniorDesign.Models
{
    // This class will not be called unless it is enabled in Global.asax.cs
    public class SdInitializer : DropCreateDatabaseIfModelChanges<SdContext>
    {
        protected override void Seed(SdContext context)
        {
            if (Membership.GetUser("Administrator") == null)
            {
                Membership.CreateUser("Administrator", "123reids");
                //Membership.CreateUser("Kara", "kara"); //Add'l default users
                //Membership.CreateUser("John", "john");
                //Membership.CreateUser("Val", "val");
                //Membership.CreateUser("Kathy", "kathy");
            }
        }
    }
}