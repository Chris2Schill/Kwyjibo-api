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
using System.Diagnostics;

using Authentication;
using Database.Models;
using Database.Access;
using Sox;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Authenticate User
        /*if (!UserAuthenticator.IsAuthenticated(Request["userId"], Request["authToken"])) {
            Response.Write("User not authenticated.");
            Response.End();
            return;
        }*/

        // Get Request Params
        string clipName = Request["clipName"];
        int category = Convert.ToInt32(Request["category"]);
        string location = Request["location"];
        int createdById = Convert.ToInt32(Request["createdById"]);
        int stationId = Convert.ToInt32(Request["stationId"]);
        int toStation = Convert.ToInt32(Request["toStation"]);

        


        // Make the object we will return in the response
        SoundClipInfo insertedClip = new SoundClipInfo();

        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
        try { 
            // Open the connection to db
			connection.Open();

            // Save uploaded sound clip to disk
            string categoryName = DatabaseAccess.GetCategoryName(connection, category);
            string localFilepath = DatabaseAccess.CreateLocalFilePathFor(clipName, categoryName, createdById, ".wav");
            foreach (string fileStr in Request.Files) {
				HttpPostedFile file = Request.Files[fileStr];
				file.SaveAs(localFilepath);

                using (StreamWriter stream = File.AppendText("C:\\musicgroup\\log.txt"))
                {
                    stream.WriteLine("Writing file: " +localFilepath);
                }
			}
            SoxUtils.Trim(localFilepath);


            Decimal length = SoxUtils.GetClipLength(localFilepath);
            insertedClip = DatabaseAccess.InsertSoundClipInfo(connection, clipName, createdById, location, category, length, localFilepath);

            if (toStation == 1){
                DatabaseAccess.IncrementStationNumClips(connection, stationId);
                DatabaseAccess.InsertStationSoundJunction(connection, stationId, insertedClip.Id);
            }
			
            
        } catch(SqlException se) {
            Debug.WriteLine(se.Message.ToString());
            insertedClip.Error = se.Message.ToString();

        } catch(HttpException he){
            Debug.WriteLine(he.Message.ToString());
            insertedClip.Error = he.Message.ToString();

        } catch(Exception ex){
            Debug.WriteLine(ex.Message.ToString());
            insertedClip.Error = ex.Message.ToString();    
        }finally {
            if (connection.State == ConnectionState.Open) {
                connection.Close();
            }
        }

        String strJson = JsonConvert.SerializeObject(insertedClip);
        Response.ContentType = "application/json; charset=utf-8";
        Response.Write(strJson);
        Response.End();
    }

}
