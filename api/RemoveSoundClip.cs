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
        try{
			// Open the connection
            connection.Open();
			
			// Get the request parameters
            int stationId = Convert.ToInt32(Request["stationId"]);
			int soundClipId = Convert.ToInt32(Request["soundClipId"]);
			
			// Remove the clip 
            DatabaseAccess.RemoveSoundClipFromStation(connection, stationId, soundClipId);
			Response.Write(true);
			
        }catch(SqlException ex){

            Console.WriteLine(ex.Message.ToString());
			Response.Write(false);
        }finally{
            if (connection.State == ConnectionState.Open){
                connection.Close();
            }
        }
        

        Response.ContentType = "application/json; charset=utf-8";
        Response.End();
    }

}
