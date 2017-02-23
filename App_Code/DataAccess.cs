using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Collections.Specialized;

using Authentication;
using Database.Models;

namespace Database.Access
{
    public class DatabaseAccess
    {

        private static string logFile = "C:\\musicgroup\\stations-songs\\log.txt";

        /// <summary>
        ///     Inserts a record into the StationSoundJunction table.
        /// </summary>
        /// <param name="connection">The SqlConnection object used to access the database. Must be in ConnectionState.Open</param>
        /// <param name="stationName">The 'Name' of the station that the sound clip is being added to.</param>
        /// <param name="clipId">The 'Id' of the uploaded sound clip.</param>
        /// <returns>Returns true if successful. Throws an exception otherwise.</returns>
        public static bool InsertStationSoundJunction(SqlConnection connection, int stationId, int clipId)
        {
            if (connection.State == ConnectionState.Open)
            {
                try
                {
                    string query = "insert into StationSoundJunction (Station_Id, SoundClip_Id) Values (@Station_Id, @SoundClip_Id)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.Add("@Station_Id", SqlDbType.Int).Value = stationId;
                    command.Parameters.Add("@SoundClip_Id", SqlDbType.Int).Value = clipId;
                    command.ExecuteNonQuery();
                }
                catch (SqlException e)
                {
                    Debug.WriteLine(e.ToString());
                    throw;
                }
            }
            return true;
        }
		
		///
		public static string GetUsernameFromId(SqlConnection connection, int userId){
            string name = string.Empty;
			if (connection.State == ConnectionState.Open){
				try{
					string query = "select Username from Users where Id=@Id";
					SqlCommand command = new SqlCommand(query, connection);
					command.Parameters.Add("@Id", SqlDbType.Int).Value = userId;
					SqlDataReader reader = command.ExecuteReader();
					if (reader.Read()){
						name = Convert.ToString(reader["Username"]);
					}
					reader.Close();
				}catch(SqlException e){
					Debug.WriteLine(e.Message);
				}
				
			}
			return name;
		}

        /// <summary>
        ///     Creates a filepath string for an uploaded sound clip so it can be saved locally.
        /// </summary>
        /// <param name="clipName">The 'Name' of the uploaded sound clip.</param>
        /// <param name="categoryName">The category 'Name' of the uploaded sound clip.</param>
        /// <param name="filetype">The filetype of the uploaded file including the prepended period. (".mp3", ".wav", ".3gp")</param>
		/// <param name="id">The id of the user who uploaded the sound clip named by clipName</param>
        /// <returns>A filepath for the local storage of the sound clip.</returns>
        public static string CreateLocalFilePathFor(string clipName, string categoryName, int id, string filetype)
        {
            string filepath = Path.Combine(HttpContext.Current.Server.MapPath("/soundclips/" + categoryName + "/"),
                                        clipName.Replace(" ", "_") + string.Format("_{0}{1}", id, filetype));
            return filepath.ToLower();
        }

        /// <summary>
        ///     Converts a category 'Id' into a category 'Name'.
        /// </summary>
        /// <param name="connection">The SqlConnection object used to access the database. Must be in ConnectionState.Open</param>
        /// <param name="id">The category 'Id' you wish to get the 'Name' of.</param>
        /// <returns>
        ///     Returns the 'Name' field from the Categories table corresponding to the id parameter if it exists and 
        ///     throws an exception otherwise.
        /// </returns>
        public static string GetCategoryName(SqlConnection connection, int id)
        {
            string category = null;
            if (connection.State == ConnectionState.Open)
            {
                try
                {
                    string query = "SELECT Name FROM Categories WHERE Id=@Id";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        category = (string)reader["Name"];
                    }
                    reader.Close();

                }
                catch (SqlException e)
                {
                    Console.WriteLine(e.ToString());
                    throw;
                }
            }
            return category;
        }


        /// <summary>
        ///     Increments the 'NumCurrentClips' field of the station specified by the station parameter by 1.
        /// </summary>
        /// <param name="conection">The SqlConnection object used to access the database. Must be in ConnectionState.Open</param>
        /// <param name="stationName">The 'Name' of the station that needs its 'NumCurrentClips' incremented.</param>
        /// <returns>
        ///     Returns true if successful. Throws an exception if otherwise.
        /// </returns>
        public static bool IncrementStationNumClips(SqlConnection connection, int stationId)
        {
            if (connection.State == ConnectionState.Open)
            {
                try
                {
                    string query = "UPDATE Stations SET NumCurrentClips=NumCurrentClips+1 WHERE Id=@StationId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.Add("@StationId", SqlDbType.Int).Value = stationId;
                    command.ExecuteNonQuery();
                }
                catch (SqlException e)
                {
                    Console.WriteLine(e.ToString());
                    throw;
                }
            }
            return true;
        }
		
		public static void DecrementStationNumClip(SqlConnection connection, int stationId)
		{
			if (connection.State == ConnectionState.Open){
				
				try{
					string query = "UPDATE Stations SET NumCurrentClips=NumCurrentClips-1 WHERE Id=@Id";
					SqlCommand command = new SqlCommand(query, connection);
					command.Parameters.Add("@Id", SqlDbType.Int).Value = stationId;
					command.ExecuteNonQuery();
				}catch(SqlException e){
					Debug.WriteLine(e.Message);
				}
				
			}
		}

        /// <summary>
        ///     Inserts a record into the SoundClips table.
        /// </summary>
        /// <param name="connection">The SqlConnection object used to access the database. Must be in ConnectionState.Open</param>
        /// <param name="clipname">The 'Name' of the sound clip being inserted.</param>
        /// <param name="username">The username of the person who is trying to insert the sound clip.</param>
        /// <param name="location">The 'Location' of the user when they are uploading the sound clip.</param>
        /// <param name="categoryId">The 'Id' of the category of the sound clip being uploaded.</param>
        /// <param name="filepath">The filepath for where the sound clip will be stored on disk.</param>
        /// <returns>
        ///     Returns the SoundClipInfo record that was just inserted if successful. Throws an exception otherwise.
        /// </returns>
        public static  SoundClipInfo InsertSoundClipInfo(SqlConnection connection, string clipname, int createdById,
                                                    string location, int categoryId, Decimal length, string filepath)
        {
            SoundClipInfo clip = new SoundClipInfo();
            string query = "INSERT INTO SoundClips (Name, CreatedById, Location, Category, Filepath, Length, UploadDate) "
                        + "VALUES (@Name, @CreatedById, @Location, @Category, @Filepath, @Length, @UploadDate); "
                        + "SELECT * FROM SoundClips WHERE Id=SCOPE_IDENTITY();";

            if (connection.State == ConnectionState.Open)
            {
                try
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.Add("@Name", SqlDbType.VarChar, 50).Value = clipname;
                    command.Parameters.Add("@CreatedById", SqlDbType.VarChar, 50).Value = createdById;
                    command.Parameters.Add("@Location", SqlDbType.VarChar, 50).Value = location;
                    command.Parameters.Add("@Category", SqlDbType.VarChar, 50).Value = categoryId;
                    command.Parameters.Add("@Filepath", SqlDbType.VarChar, 256).Value = filepath;
                    command.Parameters.Add("@Length", SqlDbType.Decimal, 18).Value = length;
                    command.Parameters.Add("@UploadDate", SqlDbType.Date, 50).Value = DateTime.Now.ToString();

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        clip.Id = Convert.ToInt32(reader["Id"]);
                        clip.Name = Convert.ToString(reader["Name"]);
                        clip.CreatedById = Convert.ToInt32(reader["CreatedById"]);
                        clip.Location = Convert.ToString(reader["Location"]);
                        clip.Category = Convert.ToInt32(reader["Category"]);
                        clip.Filepath = Convert.ToString(reader["Filepath"]);
                        clip.Length = Convert.ToDecimal(reader["Length"]);
                        clip.UploadDate = Convert.ToDateTime(reader["UploadDate"]);
                    }
                    reader.Close();
                    clip.CreatedByName = GetUsernameFromId(connection, createdById);
                }
                catch (SqlException ex)
                {

                    using (StreamWriter stream = File.AppendText(logFile))
                    {
                        stream.WriteLine(ex.Message);
                    }
                    Console.WriteLine(ex.ToString());
                }
            }
            return clip;
        }

        /// <summary>
        ///     Inserts a new station into the Stations table.
        /// </summary>
        /// <param name="connection">The SqlConnection object used to access the database. Must be in ConnectionState.Open</param>
        /// <param name="stationName">The name of the station you wish to insert.</param>
        /// <param name="createdBy">The username of the user who is inserting the station.</param>
        /// <param name="genre">The genre of the station you wish to insert.</param>
        /// <returns>
        ///     Returns true if successful. Returns false if otherwise."
        /// </returns>
        public static bool InsertStation(SqlConnection connection, string stationName, string createdBy, string genre, int maxNumClips, int bpm, int timeSignature)
        {
            if (connection.State == ConnectionState.Open)
            {
                string query = String.Format("INSERT INTO Stations (Name, CreatedBy, Genre, MaxNumClips, BPM, TimeSignature) "
                                        + "VALUES (@StationName, @CreatedBy, @Genre, @MaxNumClips, @BPM, @TimeSignature); Select Id from Stations where Id=SCOPE_IDENTITY()" );
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.Add("@StationName", SqlDbType.VarChar, -1).Value = stationName;
                command.Parameters.Add("@CreatedBy", SqlDbType.VarChar, -1).Value = createdBy;
                command.Parameters.Add("@Genre", SqlDbType.VarChar, -1).Value = genre;
				command.Parameters.Add("@MaxNumClips", SqlDbType.Int).Value = maxNumClips;
				command.Parameters.Add("@BPM", SqlDbType.Int).Value = bpm;
                command.Parameters.Add("@TimeSignature", SqlDbType.Int).Value = timeSignature;
                SqlDataReader reader = command.ExecuteReader();
				
				int id = 0;
				if (reader.Read()){
					id = Convert.ToInt32(reader["Id"]);
				}
				reader.Close();
				
                // Append to file making the file longer over time if it is not deleted.
                using (StreamWriter stream = File.AppendText(logFile))
                {
                    stream.WriteLine("Id: " + id.ToString());
                }
				
				
				UpdateSongFilepath(connection, id, string.Format("C:\\musicgroup\\stations-songs\\{0}_{1}.wav", stationName.Replace(" ","_"), id ));
                return true;
            }
            return false;
        }
		
		public static void UpdateSongFilepath(SqlConnection connection, int stationId, string songFilepath){
			if (connection.State == ConnectionState.Open){
				try{
					string query = "UPDATE Stations SET SongFilepath=@SongFilepath WHERE Id=@Id";
					SqlCommand command = new SqlCommand(query, connection);
					command.Parameters.Add("@SongFilepath", SqlDbType.VarChar, -1).Value = songFilepath;
					command.Parameters.Add("@Id", SqlDbType.Int).Value = stationId;
					command.ExecuteNonQuery();
				}catch(SqlException e){
					Debug.WriteLine(e.Message);
				}
				

			}
		}

        /// <summary>
        ///     Inserts a user into the database.
        /// </summary>
        /// <param name="connection">The SqlConnection object used to access the database. Must be in ConnectionState.Open</param>
        /// <param name="username">The 'Username' of the user who is signing up.</param>
        /// <param name="email">The 'Email' of the user who is signing up.</param>
        /// <param name="passwordHash">The hash of the password of the user who is signing up.</param>
        /// <returns>
        ///     Returns the inserted user record as a SessionInfo object if successful. Throws an exception otherwise.
        /// </returns>
        public static SessionInfo InsertUser(SqlConnection connection, string username, string email, string passwordHash)
        {
            SessionInfo sessionInfo = new SessionInfo();
            if (connection.State == ConnectionState.Open)
            {
                try
                {
                    string query = "INSERT INTO Users (Username, Email, Password) "
                                + "VALUES (@Username, @Email, @Password);"
                                + "SELECT * FROM Users WHERE Id=SCOPE_IDENTITY()";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.Add("@Username", SqlDbType.VarChar, 50).Value = username;
                    command.Parameters.Add("@Email", SqlDbType.VarChar, 50).Value = email;
                    command.Parameters.Add("@Password", SqlDbType.VarChar, 50).Value = passwordHash;
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        sessionInfo.USER_ID = Convert.ToString(reader["Id"]);
                        sessionInfo.USER_NAME = Convert.ToString(reader["Username"]);
                        sessionInfo.USER_EMAIL = Convert.ToString(reader["Email"]);
                        sessionInfo.AUTH_TOKEN = UserAuthenticator.GenerateAuthToken(sessionInfo.USER_NAME, sessionInfo.USER_EMAIL);
                        sessionInfo.IS_AUTHENTICATED = true;
                        HttpContext.Current.Cache.Insert(sessionInfo.USER_ID, sessionInfo.AUTH_TOKEN, null,
                                System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(20));

                    }
                    reader.Close();

                }
                catch (SqlException e)
                {
                    Console.WriteLine(e.ToString());
                    throw;
                }
            }
            return sessionInfo;
        }

        /// <summary>Gets all the rows of the 'SoundClips' table.</summary>
        /// <param name="connection">The SqlConnection object used to access the database. must be in ConnectionState.Open</param>
        /// <returns>Returns all the data stored within the 'SoundClips' table </returns>
        public static SoundClipInfo[] SelectAllFromSoundClips(SqlConnection connection)
        {
            List<SoundClipInfo> clips = new List<SoundClipInfo>();
            if (connection.State == ConnectionState.Open)
            {
                String query = "SELECT * FROM SoundClips";
                SqlCommand command = new SqlCommand(query, connection);

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    SoundClipInfo clipInfo = new SoundClipInfo();

                    clipInfo.Id = Convert.ToInt32(reader["Id"]);
                    clipInfo.Name = Convert.ToString(reader["Name"]);
                    clipInfo.CreatedById = Convert.ToInt32(reader["CreatedById"]);
                    clipInfo.Location = Convert.ToString(reader["Location"]);
                    clipInfo.Category = Convert.ToInt32(reader["Category"]);
                    clipInfo.Filepath = Convert.ToString(reader["Filepath"]);
					clipInfo.Length = Convert.ToDecimal(reader["Length"]);
                    clipInfo.UploadDate = Convert.ToDateTime(reader["UploadDate"]);
                    clipInfo.CreatedByName = GetUsernameFromId(connection, clipInfo.CreatedById);
                    clipInfo.Error = String.Empty;
					clips.Add(clipInfo);
                }
                reader.Close();  
            }
            return clips.ToArray();
        }

        public static SoundClipInfo GetSoundClip(SqlConnection connection, int clipId){
            SoundClipInfo clip = new SoundClipInfo();
            if (connection.State == ConnectionState.Open){
                string query = "select * from SoundClips where Id=@Id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.Add("@Id", SqlDbType.Int).Value = clipId;

                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read()){
                    clip.Id = Convert.ToInt32(reader["Id"]);
                    clip.Name = Convert.ToString(reader["Name"]);
                    clip.CreatedById = Convert.ToInt32(reader["CreatedById"]);
                    clip.Location = Convert.ToString(reader["Location"]);
                    clip.Category = Convert.ToInt32(reader["Category"]);
                    clip.Filepath = Convert.ToString(reader["Filepath"]);
                    clip.Length = Convert.ToDecimal(reader["Length"]);
                    clip.UploadDate = Convert.ToDateTime(reader["UploadDate"]);
                    clip.CreatedByName = GetUsernameFromId(connection, clip.CreatedById);
                    clip.Error = String.Empty;
                }
                reader.Close();


            }
            return clip;

        }



        /// <summary>Gets the 'Name' field of all the categories in the 'Categories' table.</summary>
        /// <param name="connection">The SqlConnection object used to access the database. must be in ConnectionState.Open</param>
        /// <returns>Returns the names of the categories stored within the 'Categories' table </returns>
        public static List<string> GetSoundClipCatagories(SqlConnection connection)
        {
            List<string> categories = new List<string>();
            if (connection.State == ConnectionState.Open)
            {
                string query = String.Format("SELECT Name FROM Categories");
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    categories.Add((string)reader["Name"]);
                }
                reader.Close();
            }
            return categories;
        }

        /// <summary>Finds the matching sound clip in the DB and returns its 'Filepath' field.</summary>
        /// <param name="connection">The SqlConnection object used to access the database. must be in ConnectionState.Open</param>
        /// <returns>Returns the 'Filepath' field.</returns>
        public static string GetSoundClipFilepath(SqlConnection connection, int clipId)
        {
            String filepath = null;
            if (connection.State == ConnectionState.Open)
            {
                string query = "SELECT Filepath FROM SoundClips WHERE Id=@Id";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.Add("@Id", SqlDbType.Int).Value = clipId;

                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    filepath = Convert.ToString(reader["Filepath"]);
                }
                reader.Close();
            }
            return filepath;
        }

        /// <summary>Gets all the rows of the 'Stations' table.</summary>
        /// <param name="connection">The SqlConnection object used to access the database.</param>
        /// <returns>Returns all the rows of the 'Stations' table</returns>
        public static Station[] GetAllStations(SqlConnection connection)
        {				
			List<Station> stationList = new List<Station>();
            if (connection.State == ConnectionState.Open)
            {
                string query = String.Format("SELECT * FROM Stations");
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
					Station station = new Station();
	
					station = new Station();
                    station.Id = Convert.ToInt32(reader["Id"]);
                    station.Name = Convert.ToString(reader["Name"]);
                    station.CreatedBy = Convert.ToString(reader["CreatedBy"]);
                    station.Genre = Convert.ToString(reader["Genre"]);
                    station.NumCurrentClips = Convert.ToInt32(reader["NumCurrentClips"]);
					station.BPM = Convert.ToInt32(reader["BPM"]);
                    station.TimeSignature = Convert.ToInt32(reader["TimeSignature"]);
					
					stationList.Add(station);
				}
                reader.Close();
            }
            return stationList.ToArray();
        }

        /// <summary>Gets all the rows of the 'Stations' table.</summary>
        /// <param name="connection">The SqlConnection object used to access the database.</param>
        /// <param name="stationName">The name of the station that contains the sound clips you want.</param>
        /// <returns>Returns the 'SoundClipInfo' objects of the sound clips that are associated with the sepcified station.</returns>
        public static SoundClipInfo[] GetStationSoundClips(SqlConnection connection, int stationId)
        {
            List<SoundClipInfo> clips = new List<SoundClipInfo>();
            if (connection.State == ConnectionState.Open)
            {
                /*String query = "SELECT * FROM SoundClips WHERE Id IN"
                            + "    (SELECT SoundClip_Id FROM StationSoundJunction WHERE Station_Id ="
                            + "        (SELECT Id FROM Stations WHERE Name=@StationName"
                            + "        )"
                            + "    );";*/
							
				string query = "SELECT * FROM SoundClips WHERE Id IN (SELECT SoundClip_Id FROM StationSoundJunction WHERE Station_Id=@Station_Id)";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.Add("@Station_Id", SqlDbType.Int).Value = stationId;
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    SoundClipInfo clipInfo = new SoundClipInfo();
					
					clipInfo.Id = Convert.ToInt32(reader["Id"]);
                    clipInfo.Name = (string)reader["Name"];
                    clipInfo.CreatedById = Convert.ToInt32(reader["CreatedById"]);
                    clipInfo.Location = (string)reader["Location"];
                    clipInfo.Category = Convert.ToInt32(reader["Category"]);
                    clipInfo.Filepath = (string)reader["Filepath"];
					clipInfo.Length = Convert.ToDecimal(reader["Length"]);
                    clipInfo.UploadDate = Convert.ToDateTime(reader["UploadDate"]);
                    clipInfo.Error = String.Empty;
                    clipInfo.CreatedByName = GetUsernameFromId(connection, clipInfo.CreatedById);
					
					clips.Add(clipInfo);
                }
                reader.Close();

   
            }
            return clips.ToArray();
        }



        /// <summary>
        ///     Checks to see if the username and password match a record in the database.
        ///     Returns the record as a SessionInfo object if one exists. Returns an empty SessionInfo if not.
        /// </summary>
        /// <param name="connection">The SqlConnection object used to access the database.</param>
        /// <param name="username">The username of the account you wish to verify.</param>
        /// <param name="passwordHash">The hashed password of the account you wish to verify.</param>
        /// <returns>Returns the 'SoundClipInfo' objects of the sound clips that are associated with the specified station.</returns>
        public static SessionInfo ValidateUserCredentials(SqlConnection connection, string username, string passwordHash)
        {
            SessionInfo user = new SessionInfo();
            string query = "SELECT Id, Username, Email, Password FROM Users WHERE Username=@Username AND [Password]=@PasswordHash";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.Add("@Username", SqlDbType.VarChar, 50).Value = username;
            command.Parameters.Add("@PasswordHash", SqlDbType.VarChar, 50).Value = passwordHash;

            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {// This will throw an exception if no match is found
                user.USER_ID = Convert.ToString(reader["Id"]);
                user.USER_NAME = Convert.ToString(reader["Username"]);
                user.USER_EMAIL = Convert.ToString(reader["Email"]);
                user.AUTH_TOKEN = UserAuthenticator.GenerateAuthToken(user.USER_NAME, user.USER_EMAIL);
                user.IS_AUTHENTICATED = true;
                HttpContext.Current.Cache.Insert(user.USER_ID, user.AUTH_TOKEN, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(20));
            }
            reader.Close();

            return user;
        }
		
		
		public static void RemoveSoundClipFromStation(SqlConnection connection, int stationId, int soundClipId){
			if (connection.State == ConnectionState.Open){
				try{
					string query = "DELETE FROM StationSoundJunction WHERE Station_Id=@StationId AND SoundClip_Id=@SoundClipId";
					SqlCommand command = new SqlCommand(query, connection);
					command.Parameters.Add("@StationId", SqlDbType.Int).Value = stationId;
					command.Parameters.Add("@SoundClipId", SqlDbType.Int).Value = soundClipId;
					command.ExecuteNonQuery();
					
					DecrementStationNumClip(connection, stationId);
				
				}catch(SqlException e){
					using (StreamWriter stream = File.AppendText("/musicgroup/log.txt")){
						stream.WriteLine(DateTime.Now.ToString("mm/dd/yyyy")+ ": " + e.Message);
					}
				}
			}
		}
		
		public static string GetStationSongFilepath(SqlConnection connection, int stationId){
			string filepath = String.Empty;
			if (connection.State == ConnectionState.Open){
				try{
					string query = "select SongFilepath from Stations Where Id=@StationId";
					SqlCommand command = new SqlCommand(query, connection);
					command.Parameters.Add("@StationId", SqlDbType.Int).Value = stationId;
					SqlDataReader reader = command.ExecuteReader();
					if (reader.Read()){
						filepath = Convert.ToString(reader["SongFilepath"]);
					}
					reader.Close();
					
				}catch(SqlException e){
					using (StreamWriter stream = File.AppendText("/musicgroup/log.txt")){
						stream.WriteLine(DateTime.Now.ToString("mm/dd/yyyy")+ ": " + e.Message);
					}
				}
			}
			return filepath;
		}
		
		public static void DeleteSoundClip(SqlConnection connection, int soundClipId){
			if (connection.State == ConnectionState.Open){
				try{
					// Remove all entries in SoundStationJunction table
					string query = "delete from StationSoundJunction where SoundClip_Id=@SoundClipId";
					SqlCommand command = new SqlCommand(query, connection);
					command.Parameters.Add("@SoundClipId", SqlDbType.Int).Value = soundClipId;
					command.ExecuteNonQuery();
					
					// Get the local filepath so that we can delete it from the disk
					string filepath = string.Empty;
					query = "select Filepath from SoundClips where Id=@SoundClipId";
					command = new SqlCommand(query, connection);
					command.Parameters.Add("@SoundClipId", SqlDbType.Int).Value = soundClipId;
					SqlDataReader reader = command.ExecuteReader();
					if (reader.Read()){
						filepath = Convert.ToString(reader["Filepath"]);
					}
					reader.Close();
					
					// Remove from SoundClips table
					query = "delete from SoundClips where Id=@SoundClipId";
					command = new SqlCommand(query, connection);
					command.Parameters.Add("@SoundClipId", SqlDbType.Int).Value = soundClipId;
					command.ExecuteNonQuery();
					
					// Delete File from disk
					File.Delete(filepath);
 
	
				}catch(SqlException e){
					using (StreamWriter stream = File.AppendText("/musicgroup/log.txt")){
						stream.WriteLine(DateTime.Now.ToString("mm/dd/yyyy")+ ": " + e.Message);
					}
				}
			}
		}

        public static void DeleteStation(SqlConnection connection, int stationId){
			if (connection.State == ConnectionState.Open){
				try{
                    // Remove from the Stations table
					string query = "delete from Stations where Id=@StationId";
					SqlCommand command = new SqlCommand(query, connection);
					command.Parameters.Add("@StationId", SqlDbType.Int).Value = stationId;
					command.ExecuteNonQuery();

                    // Remove all entries in StationSoundJunction
					query = "delete from StationSoundJunction where Station_Id=@StationId";
					command = new SqlCommand(query, connection);
					command.Parameters.Add("@StationId", SqlDbType.Int).Value = stationId;
                    command.ExecuteNonQuery();
				}catch(SqlException e){
					using (StreamWriter stream = File.AppendText("/musicgroup/log.txt")){
						stream.WriteLine(DateTime.Now.ToString("mm/dd/yyyy")+ ": " + e.Message);
					}
				}
			}
        }
		
    }

	
}
