using System;
using System.Security.Cryptography;
using System.Text;

namespace Authentication{
	
	public class UserAuthenticator{

		public static string GenerateAuthToken(string username, string email){
			Random rand = new Random();
			int random = rand.Next(1, 51) * rand.Next(1, 51) * rand.Next(1, 51) * rand.Next(1, 51) * rand.Next(1, 51);
			return CalculateMD5Hash(username + random + email);
		}

		public static bool IsAuthenticated(string userId, string clientToken){
			string authToken = (string)(System.Web.HttpContext.Current.Cache.Get(userId));
			return clientToken.Equals(authToken);
		}
	
		public static string CalculateMD5Hash(string input){
			// step 1, calculate MD5 hash from input
			MD5 md5 = System.Security.Cryptography.MD5.Create();
			byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
			byte[] hash = md5.ComputeHash(inputBytes);

			// step 2, convert byte array to hex 
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < hash.Length; i++){
				sb.Append(hash[i].ToString("X2"));
			}
			return sb.ToString();
		}
	}
}