using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using Newtonsoft.Json;
using System.Collections.Specialized;

using Authentication;
using Database.Models;
using Database.Access;
using BeatGeneration;
using Logging;
using Sox;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string[] soundClipIds = Request.QueryString.GetValues("soundClipIds");
        int bpm = Convert.ToInt32(Request["bpm"]);
        int timeSignature = Convert.ToInt32(Request["timeSignature"]);

        Random r = new Random();

        int tempId = r.Next(1000,10000);
        string tempFilepath = string.Format("C:\\musicgroup\\temp\\temp_{0}.wav", tempId);

        SoundClipInfo[] clips = GetSoundClips(soundClipIds).ToArray();

        double[] beatPattern = new BeatGenerator().GenerateBeat(bpm, timeSignature, soundClipIds.Count());
        Logger.Log("# Sound Clips: " + clips.Length);

        SoxUtils.Construct(beatPattern, clips, tempFilepath);
        
        string strJson = JsonConvert.SerializeObject(beatPattern);
        Response.ContentType = "Content-Type: audio/mpeg";
        Response.TransmitFile(tempFilepath);
        //Response.Write(strJson);
        Response.End();
    }

    private List<SoundClipInfo> GetSoundClips(string[] ids){
        List<SoundClipInfo> clips = new List<SoundClipInfo>();
        using (var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString)){
            sqlConnection.Open();
            string query = "select * from SoundClips where Id in " + GetIdString(ids);
            using (var sqlCommand = new SqlCommand(query, sqlConnection)){
                using (var dataReader = sqlCommand.ExecuteReader()){
                    while (dataReader.Read()){

                        var row = new SoundClipInfo();
                        row.Id = Convert.ToInt32(dataReader["Id"]);
                        row.Name = Convert.ToString(dataReader["Name"]);
                        row.CreatedById = Convert.ToInt32(dataReader["CreatedById"]);
                        row.Location = Convert.ToString(dataReader["Location"]);
                        row.Category = Convert.ToInt32(dataReader["Category"]);
                        row.Filepath = Convert.ToString(dataReader["Filepath"]);
                        row.Length = Convert.ToDecimal(dataReader["Length"]);
                        row.UploadDate = Convert.ToDateTime(dataReader["UploadDate"]);
                        row.Error = string.Empty;
                        clips.Add(row);
                    }
                }
            }
        }
        return clips;
    }

    private string GetIdString(string[] array){
        StringBuilder sb = new StringBuilder();
        sb.Append("(");
        foreach (string i in array){
            sb.Append(Convert.ToInt32(i) + ", "); 
        }
        sb.Append(0 + ")");
        return sb.ToString();

    }
}
