using Continuity.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Sample.KlivaDesign.Views
{
	public sealed partial class AccountView : UserControl
	{
		public AccountView()
		{
			this.InitializeComponent();

			TestGrid.DoubleTapped += (s, e) =>
			{
				Shape.Visual().CenterPoint = new Vector3(320 + 120 - 40, 320 - 200 + 40, 0.0f);
				Shape.StartScaleAnimation(to: new Vector2(0.125f), duration: 400);
			};
		}
	}
}
