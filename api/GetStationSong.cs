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
using System.Diagnostics;
using Authentication;
using Database.Models;
using Database.Access;


public partial class _Default : System.Web.UI.Page{

    protected void Page_Load(object sender, EventArgs e){
        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);

        String filepath = null;
        try{
            connection.Open();
            filepath = DatabaseAccess.GetStationSongFilepath(connection, Convert.ToInt32(Request["stationId"]));
        }catch(SqlException ex){
			Debug.WriteLine(ex.Message);
			Response.ContentType = "application/json charset=utf-8";
			Response.Write(ex.Message);
			Response.End();
			return;
        }finally{
            if (connection.State == ConnectionState.Open){
                connection.Close();
            }
		}

    Response.ContentType = "Content-Type: audio/mpeg";
		Response.AddHeader("Content-Disposition", "attachment;filename=\""+filepath.Substring(filepath.LastIndexOf("\\")+1) + "\"");
		Response.TransmitFile(filepath);
        Response.End();
    }

}
