namespace Sample.KlivaDesign.Models
{
    public class ActivitySummary
    {
        public string FullName { get; set; }
        public string ProfileMediumFormatted { get; set; }

        public string Name { get; set; }
        public string TypeImage { get; set; }
        public string StartDate { get; set; }
        public double Distance { get; set; }
        public double ElevationGain { get; set; }
        public int CommentCount { get; set; }
        public int KudosCount { get; set; }
	    public int AchievementCount { get; set; }
	    public bool AchievementVisible => AchievementCount > 0;
    }
}
