using System;

namespace Sample.Main.Models
{
    public class Player : Person, IPlayer
    {
        public string AvatarUrl { get; set; }

        public int CareerStartYear { get; set; }

        public int? CareerEndYear { get; set; }

        public Position Position { get; set; }

        public double PointsPerGame { get; set; }

        public double ReboundsPerGame { get; set; }

        public double AssistsPerGame { get; set; }

        public double PlayerImpactEstimate { get; set; }
    }
}
