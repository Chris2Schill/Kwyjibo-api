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

public partial class _Default : System.Web.UI.Page
{

    public struct SoundClipInfo
    {
        public string Error;
        public int Id;
        public string Name;
        public string CreatedBy;
        public string Genre;
        public string Location;
    };

    protected void Page_Load(object sender, EventArgs e)
    {

        SoundClipInfo[] soundClipInfo = null;
    
        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
        try
        {
            connection.Open();
            string query = String.Format("SELECT * FROM SoundClips");
            //String query = "SELECT * FROM SoundClips";
            SqlCommand command = new SqlCommand(query,connection);
            SqlDataReader reader = command.ExecuteReader();

            StringCollection id = new StringCollection();
            StringCollection name = new StringCollection();
            StringCollection createdBy = new StringCollection();
            StringCollection genre = new StringCollection();
            StringCollection location = new StringCollection();

            while (reader.Read())
            {
                id.Add(Convert.ToString(reader["id"]));
                name.Add(Convert.ToString(reader["name"]));
                createdBy.Add(Convert.ToString(reader["createdBy"]));
                genre.Add(Convert.ToString(reader["genre"]));
                location.Add(Convert.ToString(reader["location"]));
                //Label1.Text+=String.Format(string.Format("<br>{0} {1} {2}",reader["soundName"], reader["contributor"], reader["location"]));
            }
            reader.Close();

            soundClipInfo = new SoundClipInfo[id.Count];

            for (int i = 0; i < id.Count; i++)
            {
                soundClipInfo[i] = new SoundClipInfo();

                soundClipInfo[i].Id = Convert.ToInt32(id[i]);
                soundClipInfo[i].Name = name[i];
                soundClipInfo[i].CreatedBy = createdBy[i];
                soundClipInfo[i].Genre = genre[i];
                soundClipInfo[i].Location = location[i];
                soundClipInfo[i].Error = String.Empty;
            }

            

            
        }
        catch(Exception ex)
        {
            Response.Write(ex.Message.ToString());
        }
        finally
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }

            string strJson = JsonConvert.SerializeObject(soundClipInfo);
            Response.ContentType = "application/json; charset=utf-8";
            Response.Write(strJson);
            Response.End();
        }



    }
}