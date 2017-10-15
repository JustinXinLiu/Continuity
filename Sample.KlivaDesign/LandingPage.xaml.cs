using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Continuity.Extensions;
using Sample.KlivaDesign.Models;

namespace Sample.KlivaDesign
{
    public sealed partial class LandingPage : Page
    {
        public ObservableCollection<ActivitySummary> ActivitySummaries { get; } = new ObservableCollection<ActivitySummary>();

        public LandingPage()
        {
            InitializeComponent();

            SetupMapStyle();
            ShowMapAndPolyline();
            PopulateActivities();

            void SetupMapStyle()
            {
                ActivityMap.StyleSheet = MapStyleSheet.ParseFromJson(@"
                {
                    ""version"": ""1.*"",
                    ""settings"": {
                        ""landColor"": ""#FF101416"",
                        ""spaceColor"": ""#FF101416"",
                        ""shadedReliefVisible"": false,
                        ""atmosphereVisible"": false,
                        ""logosVisible"": false
                    },
                    ""elements"": {
                        ""mapElement"": {
                            ""labelVisible"": true,
                            ""labelColor"": ""#FF3E4244"",
                            ""labelOutlineColor"": ""#FF0B0F11"",
                            ""strokeColor"": ""#FF0B0F11"",
                            ""strokeWidthScale"": 1                                             
                        },
                        ""political"": {
                            ""borderStrokeColor"": ""#FF1E3B49"",
                            ""borderOutlineColor"": ""#00000000""
                        },
                        ""point"": {
                            ""iconColor"": ""#00000000"",
                            ""fillColor"": ""#00000000"",
                            ""strokeColor"": ""#00000000""
                        },
                        ""roadShield"": {
                            ""scale"": 0
                        },

                        ""transportation"": {
                            ""strokeColor"": ""#00000000"",
                            ""fillColor"": ""#FF0B0F11""
                        },

                        ""road"": {
                            ""strokeColor"": ""#00000000"",
                            ""fillColor"": ""#FF0B0F11""
                        },
                        ""controlledAccessHighway"": {
                            ""strokeColor"": ""#00000000"",
                            ""fillColor"": ""#FF0B0F11""
                        },
                        ""highSpeedRamp"": {
                            ""strokeColor"": ""#00000000"",
                            ""fillColor"": ""#FF0B0F11""
                        },
                        ""highway"": {
                            ""strokeColor"": ""#00000000"",
                            ""fillColor"": ""#FF0B0F11""
                        },
                        ""majorRoad"": {
                            ""strokeColor"": ""#00000000"",
                            ""fillColor"": ""#FF060809""
                        },
                        ""arterialRoad"": {
                            ""strokeColor"": ""#00000000"",
                            ""fillColor"": ""#FF0B0F11""
                        },
                        ""street"": {
                            ""strokeColor"": ""#00000000"",
                            ""fillColor"": ""#FF0B0F11""
                        },
                        ""ramp"": {
                            ""strokeColor"": ""#00000000"",
                            ""fillColor"": ""#FF0B0F11""
                        },
                        ""unpavedStreet"": {
                            ""strokeColor"": ""#00000000"",
                            ""fillColor"": ""#FF0B0F11""
                        },
                        ""tollRoad"": {
                            ""strokeColor"": ""#00000000"",
                            ""fillColor"": ""#FF0B0F11""
                        },
                        ""railway"": {
                            ""strokeColor"": ""#00000000"",
                            ""fillColor"": ""#FF0B0F11""
                        },
                        ""trail"": {
                            ""strokeColor"": ""#00000000"",
                            ""fillColor"": ""#FF0B0F11""
                        },
                        ""waterRoute"": {
                            ""strokeColor"": ""#00000000"",
                            ""fillColor"": ""#FF182127""
                        },
                        ""water"": {
                            ""strokeColor"": ""#00000000"",
                            ""fillColor"": ""#FF182127""
                        },
                        ""river"": {
                            ""strokeColor"": ""#00000000"",
                            ""fillColor"": ""#FF182127""
                        },

                        ""structure"": {
                            ""fillColor"": ""#FF0B0F11""
                        },
                        ""area"": {
                            ""fillColor"": ""#FF0B0F11""
                        }
                    }
                }
            ");
            }

            void ShowMapAndPolyline()
            {
                ActivityMap.Visual().Opacity = 0;
                ActivityMap.LoadingStatusChanged += ActivityMapLoadingStatusChanged;

                async void ActivityMapLoadingStatusChanged(MapControl sender, object args)
                {
                    ActivityMap.LoadingStatusChanged -= ActivityMapLoadingStatusChanged;
                    RemoveMapServiceTokenWarning();

                    ActivityMap.StartOpacityAnimation();
                    await DrawPolylineAsync();
                }
            }

            void RemoveMapServiceTokenWarning()
            {
                var mapGrid = ActivityMap.GetChildByName<Grid>("MapGrid");
                var border = mapGrid.Children().OfType<Border>().Last();
                border.Visibility = Visibility.Collapsed;
            }
        }

        private async Task DrawPolylineAsync()
        {
            var geoPositions = new List<BasicGeoposition>
            {
                new BasicGeoposition { Latitude = -37.88385, Longitude = 145.18219 },
                new BasicGeoposition { Latitude = -37.88334, Longitude = 145.17769 },
                new BasicGeoposition { Latitude = -37.88385, Longitude = 145.17745 },
                new BasicGeoposition { Latitude = -37.88336, Longitude = 145.17674 },
                new BasicGeoposition { Latitude = -37.88268, Longitude = 145.17029 },
                new BasicGeoposition { Latitude = -37.88148, Longitude = 145.17013 },
                new BasicGeoposition { Latitude = -37.88081, Longitude = 145.16776 },
                new BasicGeoposition { Latitude = -37.88066, Longitude = 145.16563 },
                new BasicGeoposition { Latitude = -37.87992, Longitude = 145.16356 }
            };

            var polyLine = new MapPolyline
            {
                Path = new Geopath(geoPositions),
                StrokeThickness = 4,
                StrokeDashed = false,
                StrokeColor = Color.FromArgb(200, 245, 51, 39)
            }; 
            ActivityMap.MapElements.Add(polyLine);

            var startPoint = new Button { Style = (Style)App.Current.Resources["ButtonMapPointStyle"] };
            ActivityMap.Children.Add(startPoint);
            MapControl.SetLocation(startPoint, new Geopoint(geoPositions.First()));
            MapControl.SetNormalizedAnchorPoint(startPoint, new Point(0.5, 0.5));

            var endPoint = new Button { Style = (Style)App.Current.Resources["ButtonMapPinStyle"] };
            ActivityMap.Children.Add(endPoint);
            MapControl.SetLocation(endPoint, new Geopoint(geoPositions.Last()));
            MapControl.SetNormalizedAnchorPoint(endPoint, new Point(0.5, 0.5));

            await ActivityMap.TrySetViewBoundsAsync(GeoboundingBox.TryCompute(geoPositions),
                new Thickness(ActualWidth / 4, ActualHeight / 3, 0, 0), MapAnimationKind.Bow);
        }

        private void PopulateActivities()
        {
            ActivitySummaries.Add(new ActivitySummary
            {
                FullName = "Justin",
                ProfileMediumFormatted = "Assets/Avatars/justin.jpg",
                TypeImage = "",
                Name = "Morning Ride",
                StartDate = "34 minutes ago",
                Distance = 12.3,
                ElevationGain = 37,
                CommentCount = 1,
                KudosCount = 0
            });

            ActivitySummaries.Add(new ActivitySummary
            {
                FullName = "Bart",
                ProfileMediumFormatted = "Assets/Avatars/bart.jpg",
                TypeImage = "",
                Name = "Morning Ride",
                StartDate = "6 hours ago",
                Distance = 16.1,
                ElevationGain = 44,
                CommentCount = 3,
                KudosCount = 5
            });

            ActivitySummaries.Add(new ActivitySummary
            {
                FullName = "Glenn",
                ProfileMediumFormatted = "Assets/Avatars/glenn.jpg",
                TypeImage = "",
                Name = "Night Ride",
                StartDate = "1 day ago",
                Distance = 72.9,
                ElevationGain = 121,
                CommentCount = 13,
                KudosCount = 27
            });

            ActivitySummaries.Add(new ActivitySummary
            {
                FullName = "Glenn",
                ProfileMediumFormatted = "Assets/Avatars/glenn.jpg",
                TypeImage = "",
                Name = "Night Ride",
                StartDate = "1 day ago",
                Distance = 2.9,
                ElevationGain = 77,
                CommentCount = 2,
                KudosCount = 33
            });

            ActivitySummaries.Add(new ActivitySummary
            {
                FullName = "Bart",
                ProfileMediumFormatted = "Assets/Avatars/bart.jpg",
                TypeImage = "",
                Name = "Night Ride",
                StartDate = "1 day ago",
                Distance = 67.8,
                ElevationGain = 99,
                CommentCount = 12,
                KudosCount = 7
            });

            ActivitySummaries.Add(new ActivitySummary
            {
                FullName = "Justin",
                ProfileMediumFormatted = "Assets/Avatars/justin.jpg",
                TypeImage = "",
                Name = "Night Ride",
                StartDate = "2 days ago",
                Distance = 7.9,
                ElevationGain = 23,
                CommentCount = 55,
                KudosCount = 12
            });

            ActivitySummaries.Add(new ActivitySummary
            {
                FullName = "Bart",
                ProfileMediumFormatted = "Assets/Avatars/bart.jpg",
                TypeImage = "",
                Name = "Night Ride",
                StartDate = "4 days ago",
                Distance = 143.8,
                ElevationGain = 212,
                CommentCount = 1,
                KudosCount = 3
            });
        }
    }
}
