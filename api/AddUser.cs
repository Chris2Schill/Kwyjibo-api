using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Text;
using Database.Models;
using Database.Access;
using Authentication;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        SessionInfo info = new SessionInfo();
        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);

        try {
            connection.Open();
            info = DatabaseAccess.InsertUser(connection, Request["username"], Request["email"], UserAuthenticator.CalculateMD5Hash(Request["password"]) );
        } catch(Exception ex) {
            Console.WriteLine(ex.ToString());
            info.USER_ID = String.Empty;
            info.USER_NAME = String.Empty;
            info.USER_EMAIL = String.Empty;
            info.AUTH_TOKEN = String.Empty;
            info.IS_AUTHENTICATED = false;
            info.ERROR = ex.Message.ToString();
        } finally {
            if (connection.State == ConnectionState.Open) {
                connection.Close();
            }
        }

        string strJson = JsonConvert.SerializeObject(info);
        Response.ContentType = "application/json; charset=utf-8";
        Response.Write(strJson);
        Response.End();
    }
}