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

        Station[] stationList = null;
        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
        try {
            connection.Open();
            stationList = DatabaseAccess.GetAllStations(connection);
        }catch(Exception ex){
            Response.Write(ex.Message.ToString() + "\n");
            Console.WriteLine(ex.Message.ToString());
        }finally{
            if (connection.State == ConnectionState.Open){
                connection.Close();
            }
        }
        String strJson = JsonConvert.SerializeObject(stationList);
        Response.ContentType = "application/json; charset=utf-8";
        Response.Write(strJson);
        Response.End();
    }
}