namespace Sample.Main.Models
{
    public interface IPlayer
    {
        string AvatarUrl { get; set; }

        int CareerStartYear { get; set; }

        int? CareerEndYear { get; set; }

        Position Position { get; set; }

        string Bio { get; set; }

        double PointsPerGame { get; set; }

        double ReboundsPerGame { get; set; }

        double AssistsPerGame { get; set; }

        double PlayerImpactEstimate { get; set; }
    }
}
