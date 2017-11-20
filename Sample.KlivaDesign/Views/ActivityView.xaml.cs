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
using Windows.UI.Xaml.Media;
using Continuity.Extensions;
using Microsoft.Toolkit.Uwp.Helpers;
using Sample.KlivaDesign.Models;
using System.Numerics;
using Windows.UI.Xaml.Controls.Primitives;
using Continuity;

namespace Sample.KlivaDesign.Views
{
	public sealed partial class ActivityView : UserControl
	{
		public ObservableCollection<ActivitySummary> ActivitySummaries { get; } = new ObservableCollection<ActivitySummary>();
		public ObservableCollection<Segment> Segments { get; } = new ObservableCollection<Segment>();

		public ActivityView()
		{
			InitializeComponent();

			SetupMapStyle();
			EnableAnimations();

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

			void EnableAnimations()
			{
				ActionsPanel.EnableFluidVisibilityAnimation(AnimationAxis.Y, -174.0f, -174.0f, showDuration: 400, hideDuration: 400);
				ActivityType.EnableFluidVisibilityAnimation(AnimationAxis.Y, 12.0f, -12.0f, showDuration: 400, hideDuration: 400, showDelay: 800);

				LeftBladeToggle.EnableFluidVisibilityAnimation(centerPoint: new Vector3(20.0f, 20.0f, 0.0f), showFromScale: 0.2f, hideToScale: 0.2f, showDuration: 400, hideDuration: 400);
				RightBladeToggle.EnableFluidVisibilityAnimation(centerPoint: new Vector3(20.0f, 20.0f, 0.0f), showFromScale: 0.2f, hideToScale: 0.2f, showDuration: 400, hideDuration: 400);
				LeftBladeContent.EnableFluidVisibilityAnimation(showFromScale: 0.0f, hideToScale: 0.0f, showDuration: 400, hideDuration: 400);
				RightBladeContent.EnableFluidVisibilityAnimation(showFromScale: 0.0f, hideToScale: 0.0f, showDuration: 400, hideDuration: 400, showDelay: 100);

				RightBladeToggle.EnableImplicitAnimation(VisualPropertyType.Offset, 400, 100);
			}
		}

		public async Task InitializeAsync()
		{
			ShowMapAndPolyline();

			await Task.Delay(1000);

			PopulateActivities();
			PopulateSegments();

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
					AnimateMapProperties();

					void RemoveMapServiceTokenWarning()
					{
						var mapGrid = ActivityMap.GetChildByName<Grid>("MapGrid");
						var border = mapGrid.Children().OfType<Border>().Last();
						border.Visibility = Visibility.Collapsed;
					}

					void AnimateMapProperties()
					{
						ActivityMap.Animate(null, 45.0d, nameof(ActivityMap.Heading), 800, enableDependentAnimation: true);
						ActivityMap.Animate(null, 75.0d, nameof(ActivityMap.DesiredPitch), 800, enableDependentAnimation: true);
						ActivityMap.Animate(null, 14.5d, nameof(ActivityMap.ZoomLevel), 800, enableDependentAnimation: true);

					}
				}
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

			var margin = ActualWidth / 4;
			await ActivityMap.TrySetViewBoundsAsync(GeoboundingBox.TryCompute(geoPositions),
				new Thickness(margin, margin, margin, margin), MapAnimationKind.Bow);
		}

		private void PopulateActivities()
		{
			ActionsPanel.Visibility = Visibility.Visible;
			ActivityType.Visibility = Visibility.Visible;

			ActivitySummaries.Add(new ActivitySummary
			{
				FullName = "Justin",
				ProfileMediumFormatted = "../Assets/Avatars/justin.jpg",
				TypeImage = "",
				Name = "Morning Ride",
				StartDate = "34 minutes ago",
				Distance = 12.3,
				ElevationGain = 37,
				CommentCount = 1,
				KudosCount = 0,
				AchievementCount = 0
			});

			ActivitySummaries.Add(new ActivitySummary
			{
				FullName = "Bart",
				ProfileMediumFormatted = "../Assets/Avatars/bart.jpg",
				TypeImage = "",
				Name = "Morning Ride",
				StartDate = "6 hours ago",
				Distance = 16.1,
				ElevationGain = 44,
				CommentCount = 3,
				KudosCount = 5,
				AchievementCount = 3
			});

			ActivitySummaries.Add(new ActivitySummary
			{
				FullName = "Glenn",
				ProfileMediumFormatted = "../Assets/Avatars/glenn.jpg",
				TypeImage = "",
				Name = "Night Ride",
				StartDate = "1 day ago",
				Distance = 72.9,
				ElevationGain = 121,
				CommentCount = 13,
				KudosCount = 27,
				AchievementCount = 51
			});

			ActivitySummaries.Add(new ActivitySummary
			{
				FullName = "Glenn",
				ProfileMediumFormatted = "../Assets/Avatars/glenn.jpg",
				TypeImage = "",
				Name = "Night Ride",
				StartDate = "1 day ago",
				Distance = 2.9,
				ElevationGain = 77,
				CommentCount = 2,
				KudosCount = 33,
				AchievementCount = 0
			});

			ActivitySummaries.Add(new ActivitySummary
			{
				FullName = "Bart",
				ProfileMediumFormatted = "../Assets/Avatars/bart.jpg",
				TypeImage = "",
				Name = "Night Ride",
				StartDate = "1 day ago",
				Distance = 67.8,
				ElevationGain = 99,
				CommentCount = 12,
				KudosCount = 7,
				AchievementCount = 0
			});

			ActivitySummaries.Add(new ActivitySummary
			{
				FullName = "Justin",
				ProfileMediumFormatted = "../Assets/Avatars/justin.jpg",
				TypeImage = "",
				Name = "Night Ride",
				StartDate = "2 days ago",
				Distance = 7.9,
				ElevationGain = 23,
				CommentCount = 55,
				KudosCount = 12,
				AchievementCount = 0
			});

			ActivitySummaries.Add(new ActivitySummary
			{
				FullName = "Bart",
				ProfileMediumFormatted = "../Assets/Avatars/bart.jpg",
				TypeImage = "",
				Name = "Night Ride",
				StartDate = "4 days ago",
				Distance = 143.8,
				ElevationGain = 212,
				CommentCount = 1,
				KudosCount = 3,
				AchievementCount = 121
			});

			ActivitySummaries.Add(new ActivitySummary
			{
				FullName = "Glenn",
				ProfileMediumFormatted = "../Assets/Avatars/glenn.jpg",
				TypeImage = "",
				Name = "Afternoon Ride",
				StartDate = "1 day ago",
				Distance = 0.6,
				ElevationGain = 12,
				CommentCount = 0,
				KudosCount = 1,
				AchievementCount = 0
			});

			ActivitySummaries.Add(new ActivitySummary
			{
				FullName = "Glenn",
				ProfileMediumFormatted = "../Assets/Avatars/glenn.jpg",
				TypeImage = "",
				Name = "Night Ride",
				StartDate = "1 day ago",
				Distance = 12,
				ElevationGain = 50,
				CommentCount = 2,
				KudosCount = 7,
				AchievementCount = 2
			});
		}

		private void PopulateSegments()
		{
			Segments.Add(new Segment
			{
				Name = "166th down",
				Distance = 0.5,
				Time = "0:40",
				Speed = 46.2
			});

			Segments.Add(new Segment
			{
				Name = "60 Acres Park to Wilmot Gateway Park (SRT)",
				Distance = 6.1,
				Time = "15:50",
				Speed = 23.3
			});

			Segments.Add(new Segment
			{
				Name = "NE 116th St to NE 145th St (SRT)",
				Distance = 3.2,
				Time = "10:25",
				Speed = 18.8
			});

			Segments.Add(new Segment
			{
				Name = "116 to 124",
				Distance = 0.7,
				Time = "11:28",
				Speed = 8.1
			});

			Segments.Add(new Segment
			{
				Name = "124th to Bothell",
				Distance = 7.3,
				Time = "5:50",
				Speed = 30.9
			});

			Segments.Add(new Segment
			{
				Name = "Full Moon TT - Sprint 01",
				Distance = 1.6,
				Time = "14:15",
				Speed = 32.8
			});

			Segments.Add(new Segment
			{
				Name = "BrewWood",
				Distance = 2.7,
				Time = "2:50",
				Speed = 32
			});

			Segments.Add(new Segment
			{
				Name = "Full Moon TT - Sprint 02",
				Distance = 1.5,
				Time = "22:44",
				Speed = 33.6
			});

			Segments.Add(new Segment
			{
				Name = "522 Boogie",
				Distance = 1.6,
				Time = "5:48",
				Speed = 29.8
			});

			Segments.Add(new Segment
			{
				Name = "N Creek Trail Climb",
				Distance = 0.8,
				Time = "4:25",
				Speed = 27.4
			});

			Segments.Add(new Segment
			{
				Name = "522 to the powerlines on SRT TT",
				Distance = 10.05,
				Time = "13:02",
				Speed = 27.6
			});

			Segments.Add(new Segment
			{
				Name = "405 to Wilmot",
				Distance = 1.4,
				Time = "3:17",
				Speed = 28.5
			});

			Segments.Add(new Segment
			{
				Name = "Wilmot Gateway Park to Leary Way NE (SRT)",
				Distance = 6.1,
				Time = "13:05",
				Speed = 28.2
			});

			Segments.Add(new Segment
			{
				Name = "WoodBrew",
				Distance = 2.6,
				Time = "5:48",
				Speed = 27.9
			});
		}

		#region Blades event handlers

		private void OnSegmentsContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
		{
			if (args.InRecycleQueue || args.ItemContainer == null) return;

			if (args.ItemIndex % 2 == 0)
			{
				args.ItemContainer.Background = new SolidColorBrush("#02FFEBEA".ToColor());
			}
			else
			{
				args.ItemContainer.Background = new SolidColorBrush("#05FFEBEA".ToColor());
			}
		}

		private void OnLeftBladeContentSizeChanged(object sender, SizeChangedEventArgs e) =>
			LeftBladeContent.Visual().CenterPoint = new Vector3(0.0f, (LeftBladeToggle.Margin.Top + LeftBladeToggle.ActualHeight / 2).ToFloat(), 0.0f);

		private void OnLeftBladeToggleChecked(object sender, RoutedEventArgs e) =>
			LeftBladeContent.Visibility = Visibility.Visible;

		private void OnLeftBladeToggleUnchecked(object sender, RoutedEventArgs e) =>
			LeftBladeContent.Visibility = Visibility.Collapsed;

		private void OnRightBladeContentSizeChanged(object sender, SizeChangedEventArgs e) =>
			RightBladeContent.Visual().CenterPoint = new Vector3(RightBladeContent.ActualWidth.ToFloat(), (RightBladeToggle.Margin.Top + RightBladeToggle.ActualHeight / 2).ToFloat(), 0.0f);

		private void OnRightBladeToggleChecked(object sender, RoutedEventArgs e) =>
			RightBladeContent.Visibility = Visibility.Visible;

		private void OnRightBladeToggleUnchecked(object sender, RoutedEventArgs e) =>
			RightBladeContent.Visibility = Visibility.Collapsed;

		#endregion

		#region ActivityList event handlers

		private void OnActivityListContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
		{
			if (!args.InRecycleQueue)
			{
				args.ItemContainer.Loaded += OnItemContainerLoaded;
			}

			void OnItemContainerLoaded(object s, RoutedEventArgs e)
			{
				args.ItemContainer.Loaded -= OnItemContainerLoaded;

				// Don't animate if we're not in the visible viewport
				if (ActivityList.ItemsPanelRoot is ItemsStackPanel itemsPanel &&
					args.ItemContainer.Children().OfType<ListViewItemPresenter>().SingleOrDefault() is ListViewItemPresenter itemPresenter &&
					args.ItemIndex >= itemsPanel.FirstVisibleIndex && args.ItemIndex <= itemsPanel.LastVisibleIndex)
				{
					var itemVisual = itemPresenter.Visual();
					itemVisual.Opacity = 0.0f;
					itemVisual.CenterPoint = new Vector3(itemPresenter.RenderSize.ToVector2() / 2, 0f);
					var delay = (args.ItemIndex - itemsPanel.FirstVisibleIndex) * 100;

					itemVisual.StartOpacityAnimation(delay: delay);
					itemVisual.StartScaleAnimation(new Vector2(0.6f), Vector2.One, delay: delay);
				}
			}
		}

		private void OnActivityListItemClick(object sender, ItemClickEventArgs e)
		{
			LeftBladeToggle.Visibility = Visibility.Visible;
			RightBladeToggle.Visibility = Visibility.Visible;

			RightBladeToggle.IsChecked = true;
			LeftBladeToggle.IsChecked = true;
		}

		#endregion
	}
}
