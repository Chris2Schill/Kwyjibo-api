using System;
namespace Database.Models{
	
	public struct Station {
        public int Id;
        public string Name;
        public string CreatedBy;
        public string Genre;
        public int NumCurrentClips;
		public int BPM, TimeSignature;
    };
	
	public struct SoundClipInfo {
        public int Id;
        public string Name;
        // We include both values here becuase the android client wants them both and it's easier serving it this way.
        public int CreatedById;
        public string CreatedByName;
        public string Location;
        public int Category;
        public string Filepath;
        public decimal Length;
        public DateTime UploadDate;
        public string Error;
    };
	
	public struct SessionInfo{
		public string USER_ID;
		public string USER_NAME;
		public string USER_EMAIL;
		public string AUTH_TOKEN;
		public bool IS_AUTHENTICATED;
        public string ERROR;
	};
}
