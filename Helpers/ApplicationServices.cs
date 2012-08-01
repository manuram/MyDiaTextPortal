using System;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Web.Management;

namespace SeniorDesign.Models
{
    public static class ApplicationServices
    {
        readonly static string connectionString =
              WebConfigurationManager.ConnectionStrings["SdContext"].ConnectionString;
        readonly static SqlConnectionStringBuilder myBuilder =
              new SqlConnectionStringBuilder(connectionString);

        public static void InstallServices(SqlFeatures sqlFeatures)
        {
            SqlServices.Install(myBuilder.InitialCatalog, sqlFeatures, connectionString);
        }

        public static void UninstallServices(SqlFeatures sqlFeatures)
        {
            SqlServices.Uninstall(myBuilder.InitialCatalog, sqlFeatures, connectionString);
        }
    }
}