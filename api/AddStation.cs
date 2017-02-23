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
using Logging;

public partial class _Default : System.Web.UI.Page 
{

    protected void Page_Load(object sender, EventArgs e)
    {
        //if (!UserAuthenticator.IsAuthenticated(Request["userId"], Request["authToken"])) {
         //   return;
       // }
       // o
        
        Logger.Log("AddStation() called");
        string stationName = Request["stationName"];
        Logger.Log("Got station Name");
        string createdBy = Request["createdBy"];
        Logger.Log("got created by");
        string genre = Request["genre"];
        Logger.Log("got genre");
        int maxNumClips = Convert.ToInt32(Request["maxNumClips"]);
        Logger.Log("got max clips");
        int bpm = Convert.ToInt32(Request["bpm"]);
        Logger.Log("got bpm");
        int timeSig = Convert.ToInt32(Request["timeSig"]);
        Logger.Log("got timeSig");

        Logger.Log(string.Format("INFO: stationName: {0}, createdBy: {1}, genre: {2}, maxNumClips: {3}, bpm: {4}, timeSig: {5}",
                    stationName, createdBy, genre, maxNumClips, bpm, timeSig));

        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
        try {
            connection.Open();
            DatabaseAccess.InsertStation(connection, stationName, createdBy, genre, maxNumClips, bpm, timeSig);
            Response.Write(JsonConvert.SerializeObject(true));
            Logger.Log("Add Station returning true");
        } catch(Exception ex) {
            Response.Write(false);
            Logger.Log(ex.Message);
        } finally {
            if (connection.State == ConnectionState.Open) {
                connection.Close();
            }
        }

        Response.ContentType = "application/json; charset=utf-8"; 
        Response.End();
    }
}
