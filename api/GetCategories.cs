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
using Database.Access;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        List<String> categories = new List<String>();
    
        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
        try {
            connection.Open();
            categories = DatabaseAccess.GetSoundClipCatagories(connection);
        } catch(Exception ex) {
            Response.Write(ex.Message.ToString());
            Console.WriteLine(ex.Message.ToString());
        } finally {
            if (connection.State == ConnectionState.Open) {
                connection.Close();
            }
        }
        string strJson = JsonConvert.SerializeObject(categories);
        Response.ContentType = "application/json; charset=utf-8";
        Response.Write("\n"+strJson+"\n\n");
        Response.End();
    }
}
