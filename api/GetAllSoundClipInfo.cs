using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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
        /*if (!UserAuthenticator.IsAuthenticated(Request["userId"], Request["authToken"])){
            Response.Write("User not authenticated.");
            Response.End();
            return;
        }*/

		SoundClipInfo[] clips = null;
        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
        try{
            connection.Open();
            clips = DatabaseAccess.SelectAllFromSoundClips(connection);
        }catch(SqlException ex){
            Console.WriteLine(ex.Message.ToString());            
            clips = new SoundClipInfo[1];
            clips[0].Error = ex.Message.ToString();
        }finally{
            if (connection.State == ConnectionState.Open){
                connection.Close();
            }
        }
			
        String strJson = JsonConvert.SerializeObject(clips);
        Response.ContentType = "application/json; charset=utf-8";
        Response.Write(strJson);
        Response.End();
    }
}
