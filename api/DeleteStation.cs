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
using Database.Access;
using Authentication;

public partial class _Default : System.Web.UI.Page 
{

    protected void Page_Load(object sender, EventArgs e)
    {
        //if (!UserAuthenticator.IsAuthenticated(Request["userId"], Request["authToken"])) {
          //  return;
      //  }
        
        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
        try {
            connection.Open();

            // Get request parameters
            int stationId = Convert.ToInt32(Request["stationId"]);

            // Delete the station
            DatabaseAccess.DeleteStation(connection, stationId);

            Response.Write(JsonConvert.SerializeObject(true));
        } catch(Exception ex) {
            Response.Write(JsonConvert.SerializeObject(false) + "<br />");
            Response.Write(ex.Message.ToString());
        } finally {
            if (connection.State == ConnectionState.Open) {
                connection.Close();
            }
        }

        Response.ContentType = "application/json; charset=utf-8"; 
        Response.End();
    }
}
