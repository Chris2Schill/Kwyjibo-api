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

using Authentication;
using Database.Access;
using Database.Models;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        /*if (!UserAuthenticator.IsAuthenticated(Request["userId"], Request["authToken"])) {
            Response.Write("User not authenticated.");
            Response.End();
            return;
        }*/

        // Get Request params
        int clipId = Convert.ToInt32(Request["clipId"]);

        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
        connection.Open();

        // Get the filepath for the soundclip
		string filepath = DatabaseAccess.GetSoundClipFilepath(connection, clipId);

        // Get the clip record so we can return the name. The android client needs it to display to the user
        SoundClipInfo clip = DatabaseAccess.GetSoundClip(connection, clipId); 

        Response.ContentType = "Content-Type: audio/mpeg";
		Response.AddHeader("Content-Disposition", "attachment;filename=\""+filepath.Substring(filepath.LastIndexOf("\\")) + "\"");
        Response.TransmitFile(filepath);
        Response.Write(JsonConvert.SerializeObject(clip));
        Response.End();
    }

}
