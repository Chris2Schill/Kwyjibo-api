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
using Authentication;
using Database.Models;
using Database.Access;


public partial class _Default : System.Web.UI.Page{

    protected void Page_Load(object sender, EventArgs e){
        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);

        SessionInfo user = new SessionInfo();
        try{
            connection.Open();
            string username = Request["username"];
            string passwordHash = UserAuthenticator.CalculateMD5Hash(Request["password"]);
            user = DatabaseAccess.ValidateUserCredentials(connection, username, passwordHash);
        }catch(SqlException ex){
            user.USER_ID = String.Empty;
            user.USER_NAME = String.Empty;
            user.USER_EMAIL = String.Empty;
            user.AUTH_TOKEN = String.Empty;
            user.IS_AUTHENTICATED = false;
			user.ERROR = ex.Message;
            Console.WriteLine(ex.Message.ToString());
        }finally{
            if (connection.State == ConnectionState.Open){
                connection.Close();
            }
        }
        
        string localFilepath = "C:\\musicgroup\\soundclips\\Percussive\\Sample.mp3";
 

        String strJson = JsonConvert.SerializeObject(user);
        Response.ContentType = "application/json; charset=utf-8";
        Response.Write("\n"+strJson+"\n\n");
        Response.End();
    }

}
