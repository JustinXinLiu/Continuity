using Continuity.Extensions;
using System;
using System.Numerics;
using Windows.Devices.Sensors;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Continuity.Controls
{
    /// <summary>
    /// TODO:
    /// 1. Create glow effect on each big star
    /// 2. Dispose stars that are outside of the window and recreate new ones
    /// 3. Hook up with Inclinometer (90%)
    /// </summary>
    public sealed partial class StarsBackground : UserControl
    {
        #region Fields

        private readonly Random _random = new Random((int)DateTime.Now.Ticks);

        private CompositionSurfaceBrush _circleBrush;
        private Compositor _compositor;
        private ContainerVisual _skyVisual;

        private Inclinometer _inclinometer;
        private uint _desiredReportInterval;
        private ExpressionAnimation _skyViusalOffsetExpressionAnimation;
        private CompositionPropertySet _reading;

        #endregion

        #region Constructor

        public StarsBackground()
        {
            InitializeComponent();

            SetupInclinometer();

            Loaded += OnLoaded;
        }

        #endregion

        #region Properties

        private float SkyVisualRadius => _skyVisual.Size.X / 2;

        public string StarUriString
        {
            get => (string)GetValue(StarUriStringProperty);
            set => SetValue(StarUriStringProperty, value);
        }

        public static readonly DependencyProperty StarUriStringProperty =
            DependencyProperty.Register("StarUriString", typeof(string), typeof(StarsBackground),
                new PropertyMetadata("ms-appx:///Assets/Star.png"));

        public float SkyVisualAreaRatio
        {
            get => (float)GetValue(SkyVisualAreaRatioProperty);
            set => SetValue(SkyVisualAreaRatioProperty, value);
        }

        public static readonly DependencyProperty SkyVisualAreaRatioProperty =
            DependencyProperty.Register("SkyVisualAreaRatio", typeof(float), typeof(StarsBackground), new PropertyMetadata(1.2f));

        public int NumberOfSmallTwinklingStars
        {
            get => (int)GetValue(NumberOfSmallTwinklingStarsProperty);
            set => SetValue(NumberOfSmallTwinklingStarsProperty, value);
        }

        public static readonly DependencyProperty NumberOfSmallTwinklingStarsProperty =
            DependencyProperty.Register("NumberOfSmallTwinklingStars", typeof(int), typeof(StarsBackground),
                new PropertyMetadata(600));

        public int NumberOfBigMovingStars
        {
            get => (int)GetValue(NumberOfBigMovingStarsProperty);
            set => SetValue(NumberOfBigMovingStarsProperty, value);
        }

        public static readonly DependencyProperty NumberOfBigMovingStarsProperty =
            DependencyProperty.Register("NumberOfBigMovingStars", typeof(int), typeof(StarsBackground),
                new PropertyMetadata(150));

        #endregion

        #region Handlers

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            //await HideStatusBar();

            InitializeCompositionVariables();
            CreateStarsVisuals();
            StartSkyCanvasStartupAnimations();
        }

        private void OnWindowVisibilityChanged(object sender, VisibilityChangedEventArgs e)
        {
            if (e.Visible)
            {
                RunInclinometer();
            }
            else
            {
                StopInclinometer();
            }
        }

        private async void OnInclinometerReadingChanged(Inclinometer sender, InclinometerReadingChangedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                _reading.InsertVector3("Offset", new Vector3(e.Reading.RollDegrees, e.Reading.PitchDegrees, 0.0f));
            });
        }

        #endregion

        #region Methods

        private void SetupInclinometer()
        {
            _inclinometer = Inclinometer.GetDefault();

            if (_inclinometer != null)
            {
                // Select a report interval that is both suitable for the purposes of the app and supported by the sensor.
                // This value will be used later to activate the sensor.
                uint minReportInterval = _inclinometer.MinimumReportInterval;
                _desiredReportInterval = minReportInterval > 16 ? minReportInterval : 16;
                // Establish the report interval.
                _inclinometer.ReportInterval = _desiredReportInterval;

                // Whenever the app is not present...
                Window.Current.VisibilityChanged += OnWindowVisibilityChanged;
            }
        }

        /// <summary>
        /// Restore the default report interval.
        /// </summary>
        private void RunInclinometer()
        {
            _inclinometer.ReportInterval = _desiredReportInterval;
            _inclinometer.ReadingChanged += OnInclinometerReadingChanged;
        }

        /// <summary>
        /// Reset the default report interval to release resources while the sensor is not in use.
        /// </summary>
        private void StopInclinometer()
        {
            _inclinometer.ReportInterval = 0;
            _inclinometer.ReadingChanged -= OnInclinometerReadingChanged;
        }

        //private async Task HideStatusBar()
        //{
        //    //if (ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1, 0))
        //    //{
        //    //    var statusBar = StatusBar.GetForCurrentView();
        //    //    await statusBar.HideAsync();
        //    //}
        //}

        private void InitializeCompositionVariables()
        {
            var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
            var maxWindowWidth = bounds.Width.ToFloat();
            var maxWindowHeight = bounds.Height.ToFloat();
            var maxWindowRadius = Math.Max(maxWindowWidth, maxWindowHeight);

            // Setup sky visual's size, position, opacity, etc.
            _skyVisual = Sky.ContainerVisual();
            _skyVisual.Size = new Vector2(maxWindowRadius * SkyVisualAreaRatio);
            _skyVisual.Offset = new Vector3(-SkyVisualRadius + maxWindowWidth / 2.0f, -SkyVisualRadius + maxWindowHeight / 2.0f, 0.0f);
            _skyVisual.CenterPoint = new Vector3(SkyVisualRadius, SkyVisualRadius, 0.0f);
            _skyVisual.Opacity = 0;

            _compositor = _skyVisual.Compositor;
            _reading = _compositor.CreatePropertySet();
            _reading.InsertVector3("Offset", new Vector3(0.0f, 0.0f, 0.0f));
            _circleBrush = _compositor.CreateSurfaceBrush();
            _circleBrush.Surface = LoadedImageSurface.StartLoadFromUri(new Uri(StarUriString));

            if (_inclinometer != null) SetupSkyVisualOffsetExpressionAnimation();
        }

        private void SetupSkyVisualOffsetExpressionAnimation()
        {
            // Kick off an expression animation that links the roll & pitch degress to the -offset of the sky canvas visual
            // TODO: Need to constraint the offset (total offset < dimension * SkyVisualAreaRatio) with
            // CreateConditionalExpressionAnimation once the next mobile build is available.
            _skyViusalOffsetExpressionAnimation = _compositor.CreateExpressionAnimation(
                "Vector3(SkyVisual.Offset.X - Reading.Offset.X * Sensitivity, SkyVisual.Offset.Y - Reading.Offset.Y * Sensitivity, 0.0f)");
            _skyViusalOffsetExpressionAnimation.SetReferenceParameter("SkyVisual", _skyVisual);
            _skyViusalOffsetExpressionAnimation.SetReferenceParameter("Reading", _reading);
            _skyViusalOffsetExpressionAnimation.SetScalarParameter("Sensitivity", 0.2f);
            //_skyViusalOffsetExpressionAnimation.SetScalarParameter("MaxDimension", SkyVisualRadius * 2 * SkyVisualAreaRatio);
            _skyVisual.StartAnimation("Offset", _skyViusalOffsetExpressionAnimation);
        }

        private void CreateStarsVisuals()
        {
            for (int i = 0; i < NumberOfSmallTwinklingStars; i++)
            {
                CreateSmallTwinklingStarVisual();
            }

            for (int i = 0; i < NumberOfBigMovingStars; i++)
            {
                CreateBigMovingStarVisual();
            }
        }

        private void StartSkyCanvasStartupAnimations()
        {
            _skyVisual.StartScaleAnimation(new Vector2(0.8f, 0.8f), new Vector2(1.0f, 1.0f), 1600);
            _skyVisual.StartOpacityAnimation(0.0f, 1.0f, 1600);
        }

        private void CreateSmallTwinklingStarVisual()
        {
            float z = _random.Create(-160, 160);
            float diameter = _random.Create(1, 3);
            var starVisual = CreateStarVisual(z, diameter);

            double duration = _random.Create(40000, 80000);
            StartStarVisualOffsetAnimation(starVisual, duration);
            StartSmallTwinklingStarVisualOpacityAnimation(starVisual);

            _skyVisual.Children.InsertAtTop(starVisual);
        }

        private void CreateBigMovingStarVisual()
        {
            float offsetZ = _random.Create(-240, 0);
            float diameter = _random.Create(3, 5, (value) => value > 4);
            var starVisual = CreateStarVisual(offsetZ, diameter);

            float finalOffsetZ = _random.Create(240, 400, (value) => value > 320);
            double duration = _random.Create(10000, 40000, (value) => value < 20000);
            StartStarVisualOffsetAnimation(starVisual, duration, finalOffsetZ);
            StartBigMovingStarVisualOpacityAnimation(starVisual);

            _skyVisual.Children.InsertAtTop(starVisual);
        }

        private SpriteVisual CreateStarVisual(float offsetZ, float diameter)
        {
            float offsetX = _random.Create(-SkyVisualRadius.ToInt(), SkyVisualRadius.ToInt());
            float offsetY = _random.Create(-SkyVisualRadius.ToInt(), SkyVisualRadius.ToInt());

            var starVisual = _compositor.CreateSpriteVisual();
            starVisual.Opacity = 0.0f;
            starVisual.Brush = _circleBrush;
            starVisual.Offset = new Vector3(SkyVisualRadius + offsetX, SkyVisualRadius + offsetY, offsetZ);
            starVisual.Size = new Vector2(diameter, diameter);
            return starVisual;
        }

        private void StartSmallTwinklingStarVisualOpacityAnimation(SpriteVisual starVisual)
        {
            float minOpacity = _random.Create(0, 60, (value) => value < 40);
            float maxOpacity = _random.Create(60, 100, (value) => value > 70);
            float duration = _random.Create(250, 500);

            starVisual.StartOpacityAnimation(minOpacity / 100, maxOpacity / 100, duration,
                iterationBehavior: AnimationIterationBehavior.Forever);
        }

        private void StartBigMovingStarVisualOpacityAnimation(SpriteVisual starVisual)
        {
            float maxOpacity = _random.Create(60, 100);
            float duration = _random.Create(500, 1000);

            starVisual.StartOpacityAnimation(null, maxOpacity / 100, duration);
        }

        private void StartStarVisualOffsetAnimation(SpriteVisual starVisual, double duration, float offsetZ = 0.0f)
        {
            float offsetX = _random.Create(-12, 12);
            float offsetY = _random.Create(-12, 12);
            var oldOffset = starVisual.Offset;

            starVisual.StartOffsetAnimation(null, new Vector3(oldOffset.X + offsetX, oldOffset.Y + offsetY, oldOffset.Z + offsetZ),
                duration,
                completed: () =>
                {
                    // TODO: Dispose and recreate the star

                    StartStarVisualOffsetAnimation(starVisual, duration, offsetZ);
                });
        }

        #endregion
    }
}
