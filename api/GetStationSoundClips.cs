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

using Authentication;
using Database.Models;
using Database.Access;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        SoundClipInfo[] clips = null;
        int stationId = Convert.ToInt32(Request["stationId"]);   
    
        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
       // try{
            connection.Open();
            clips = DatabaseAccess.GetStationSoundClips(connection, stationId);
      //  }catch(Exception ex){
       //     Console.WriteLine(ex.Message.ToString());
       //     clips = new SoundClipInfo[1];
       //     clips[0].Error = ex.Message;
      //  }finally{
            if (connection.State == ConnectionState.Open) {
                connection.Close();
            }
//}
        string strJson = JsonConvert.SerializeObject(clips);
        Response.ContentType = "application/json; charset=utf-8";
        Response.Write(strJson);
        Response.End();
    }
}