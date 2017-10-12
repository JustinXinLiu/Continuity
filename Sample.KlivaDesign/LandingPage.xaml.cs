using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Continuity.Extensions;

namespace Sample.KlivaDesign
{
    public sealed partial class LandingPage : Page
    {
        public LandingPage()
        {
            InitializeComponent();

            SetupMapStyle();

            Loaded += (s, e) => RemoveMapServiceTokenWarning();

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

            void RemoveMapServiceTokenWarning()
            {
                var mapGrid = ActivityMap.GetChildByName<Grid>("MapGrid");
                var border = mapGrid.Children().OfType<Border>().Last();
                border.Visibility = Visibility.Collapsed;
            }
        }
    }
}
