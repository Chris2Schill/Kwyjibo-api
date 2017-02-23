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


public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string userId = Request.QueryString["userId"];
        string clientToken = Request.QueryString["authToken"];

        bool authenticated = UserAuthenticator.IsAuthenticated(userId, clientToken);
        string strJson = JsonConvert.SerializeObject(authenticated);
        Response.ContentType = "application/json; charset=utf-8";
        Response.Write(strJson);
        Response.End();
    }
}