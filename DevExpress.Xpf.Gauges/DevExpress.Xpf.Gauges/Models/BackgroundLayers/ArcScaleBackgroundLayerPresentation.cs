#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.Xpf.Gauges.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Gauges {
	public abstract class ArcScaleLayerPresentation : LayerPresentation {
	}
	public abstract class PredefinedArcScaleLayerPresentation : ArcScaleLayerPresentation {
		public static readonly DependencyProperty FillProperty = DependencyPropertyManager.Register("Fill",
			typeof(Brush), typeof(PredefinedArcScaleLayerPresentation), new PropertyMetadata(null, FillPropertyChanged));
		[Category(Categories.Presentation)]
		public Brush Fill {
			get { return (Brush)GetValue(FillProperty); }
			set { SetValue(FillProperty, value); }
		}
		static void FillPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PredefinedArcScaleLayerPresentation presentation = d as PredefinedArcScaleLayerPresentation;
			if (presentation != null)
				presentation.ActualFillChanged();
		}
		protected abstract Brush DefaultFill { get; }
		[Category(Categories.Presentation)]
		public Brush ActualFill { get { return Fill != null ? Fill : DefaultFill; } }
		void ActualFillChanged() {
			NotifyPropertyChanged("ActualFill");
		}
	}
	public class DefaultArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Default Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 247, 247, 247)); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new DefaultArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new DefaultArcScaleBackgroundLayerPresentation();
		}
	}
	public class DefaultHalfTopArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Default Half Top Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(0xFF, 0xF7, 0xF7, 0xF7)); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new DefaultHalfTopArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new DefaultHalfTopArcScaleBackgroundLayerPresentation();
		}
	}
	public class DefaultQuarterTopLeftArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Default Quarter Top Left Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(0xFF, 0xF7, 0xF7, 0xF7)); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new DefaultQuarterTopLeftArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new DefaultQuarterTopLeftArcScaleBackgroundLayerPresentation();
		}
	}
	public class DefaultQuarterTopRightArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Default Quarter Top Right Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(0xFF, 0xF7, 0xF7, 0xF7)); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new DefaultQuarterTopRightArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new DefaultQuarterTopRightArcScaleBackgroundLayerPresentation();
		}
	}
	public class DefaultThreeQuartersArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Default Three Quarters Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(0xFF, 0xF7, 0xF7, 0xF7)); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new DefaultThreeQuartersArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new DefaultThreeQuartersArcScaleBackgroundLayerPresentation();
		}
	}
	public class CleanWhiteArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Clean White Background"; } }
		protected override Brush DefaultFill {
			get {
				LinearGradientBrush brush = new LinearGradientBrush() { StartPoint = new Point(0.5, 0), EndPoint = new Point(0.5, 1) };
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(255, 217, 220, 224), Offset = 0 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(255, 248, 248, 248), Offset = 1 });
				return brush;
			}
		}
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new CleanWhiteArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new CleanWhiteArcScaleBackgroundLayerPresentation();
		}
	}
	public class CleanWhiteHalfTopArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Clean White Half Top Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new CleanWhiteHalfTopArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new CleanWhiteHalfTopArcScaleBackgroundLayerPresentation();
		}
	}
	public class CleanWhiteQuarterTopRightArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Clean White Quarter Top Right Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush (Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new CleanWhiteQuarterTopRightArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new CleanWhiteQuarterTopRightArcScaleBackgroundLayerPresentation();
		}
	}
	public class CleanWhiteQuarterTopLeftArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Clean White Quarter Top Left Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush (Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new CleanWhiteQuarterTopLeftArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new CleanWhiteQuarterTopLeftArcScaleBackgroundLayerPresentation();
		}
	}
	public class CleanWhiteThreeQuartersArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Clean White Three Quarters Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush (Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new CleanWhiteThreeQuartersArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new CleanWhiteThreeQuartersArcScaleBackgroundLayerPresentation();
		}
	}
	public class CosmicArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Cosmic Background"; } }
		protected override Brush DefaultFill {
			get {
				RadialGradientBrush brush = new RadialGradientBrush();
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(255, 8, 210, 229), Offset = 0.229 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(255, 18, 56, 80), Offset = 1 });
				return brush;
			}
		}
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new CosmicArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new CosmicArcScaleBackgroundLayerPresentation();
		}
	}
	public class CosmicHalfTopArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Cosmic Half Top Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new CosmicHalfTopArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new CosmicHalfTopArcScaleBackgroundLayerPresentation();
		}
	}
	public class CosmicQuarterTopRightArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Cosmic Quarter Top Right Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new CosmicQuarterTopRightArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new CosmicQuarterTopRightArcScaleBackgroundLayerPresentation();
		}
	}
	public class CosmicQuarterTopLeftArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Cosmic Quarter Top Left Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new CosmicQuarterTopLeftArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new CosmicQuarterTopLeftArcScaleBackgroundLayerPresentation();
		}
	}
	public class CosmicThreeQuartersArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Cosmic Three Quarters Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new CosmicThreeQuartersArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new CosmicThreeQuartersArcScaleBackgroundLayerPresentation();
		}
	}
	public class SmartArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Smart Background"; } }
		protected override Brush DefaultFill {
			get {
				LinearGradientBrush brush = new LinearGradientBrush() { StartPoint = new Point(0.5, 0), EndPoint = new Point(0.5, 1) };
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(255, 211, 214, 222), Offset = 0 });
				brush.GradientStops.Add(new GradientStop() { Color = Colors.White, Offset = 1 });
				return brush;
			}
		}
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new SmartArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new SmartArcScaleBackgroundLayerPresentation();
		}
	}
	public class SmartHalfTopArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Smart Half Top Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new SmartHalfTopArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new SmartHalfTopArcScaleBackgroundLayerPresentation();
		}
	}
	public class SmartQuarterTopRightArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Smart Quarter Top Right Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new SmartQuarterTopRightArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new SmartQuarterTopRightArcScaleBackgroundLayerPresentation();
		}
	}
	public class SmartQuarterTopLeftArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Smart Quarter Top Left Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new SmartQuarterTopLeftArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new SmartQuarterTopLeftArcScaleBackgroundLayerPresentation();
		}
	}
	public class SmartThreeQuartersArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Smart Three Quarters Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new SmartThreeQuartersArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new SmartThreeQuartersArcScaleBackgroundLayerPresentation();
		}
	}
	public class RedClockArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Red Clock Background"; } }
		protected override Brush DefaultFill {
			get {
				LinearGradientBrush brush = new LinearGradientBrush() { StartPoint = new Point(0.5, 0), EndPoint = new Point(0.5, 1) };
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(255, 217, 220, 224), Offset = 0 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(255, 248, 248, 248), Offset = 1 });
				return brush;
			}
		}
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new RedClockArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new RedClockArcScaleBackgroundLayerPresentation();
		}
	}
	public class RedClockHalfTopArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Red Clock Half Top Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new RedClockHalfTopArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new RedClockHalfTopArcScaleBackgroundLayerPresentation();
		}
	}
	public class RedClockQuarterTopRightArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Red Clock Quarter Top Right Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new RedClockQuarterTopRightArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new RedClockQuarterTopRightArcScaleBackgroundLayerPresentation();
		}
	}
	public class RedClockQuarterTopLeftArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Red Clock Quarter Top Left Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new RedClockQuarterTopLeftArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new RedClockQuarterTopLeftArcScaleBackgroundLayerPresentation();
		}
	}
	public class RedClockThreeQuartersArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Red Clock Three Quarters Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new RedClockThreeQuartersArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new RedClockThreeQuartersArcScaleBackgroundLayerPresentation();
		}
	}
	public class ProgressiveArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Progressive Background"; } }
		protected override Brush DefaultFill {
			get {
				RadialGradientBrush brush = new RadialGradientBrush() { Center = new Point(0.5, 1), GradientOrigin = new Point(0.5, 1), RadiusX = 1, RadiusY = 1 };
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(255, 6, 28, 43), Offset = 0 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(255, 0, 2, 7), Offset = 1 });
				return brush;
			}
		}
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new ProgressiveArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ProgressiveArcScaleBackgroundLayerPresentation();
		}
	}
	public class ProgressiveHalfTopArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Progressive Half Top Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new ProgressiveHalfTopArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ProgressiveHalfTopArcScaleBackgroundLayerPresentation();
		}
	}
	public class ProgressiveQuarterTopRightArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Progressive Quarter Top Right Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new ProgressiveQuarterTopRightArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ProgressiveQuarterTopRightArcScaleBackgroundLayerPresentation();
		}
	}
	public class ProgressiveQuarterTopLeftArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Progressive Quarter Top Left Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new ProgressiveQuarterTopLeftArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ProgressiveQuarterTopLeftArcScaleBackgroundLayerPresentation();
		}
	}
	public class ProgressiveThreeQuartersArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Progressive Three Quarters Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new ProgressiveThreeQuartersArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ProgressiveThreeQuartersArcScaleBackgroundLayerPresentation();
		}
	}
	public class EcoArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Eco Background"; } }
		protected override Brush DefaultFill {
			get {
				RadialGradientBrush brush = new RadialGradientBrush() { RadiusY = 0.519, RadiusX = 0.519 };
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(64, 33, 29, 21), Offset = 1 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(00, 33, 29, 21), Offset = 0.9 });
				return brush;
			}
		}
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new EcoArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new EcoArcScaleBackgroundLayerPresentation();
		}
	}
	public class EcoHalfTopArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Eco Half Top Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new EcoHalfTopArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new EcoHalfTopArcScaleBackgroundLayerPresentation();
		}
	}
	public class EcoQuarterTopRightArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Eco Quarter Top Right Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new EcoQuarterTopRightArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new EcoQuarterTopRightArcScaleBackgroundLayerPresentation();
		}
	}
	public class EcoQuarterTopLeftArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Eco Quarter Top Left Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new EcoQuarterTopLeftArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new EcoQuarterTopLeftArcScaleBackgroundLayerPresentation();
		}
	}
	public class EcoThreeQuartersArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Eco Three Quarters Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new EcoThreeQuartersArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new EcoThreeQuartersArcScaleBackgroundLayerPresentation();
		}
	}
	public class FutureArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Future Background"; } }
		protected override Brush DefaultFill {
			get {
				RadialGradientBrush brush = new RadialGradientBrush() { Center = new Point(0.5, 1), GradientOrigin = new Point(0.5, 1), RadiusX = 1, RadiusY = 1 };
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(255, 50, 47, 89), Offset = 0 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(255, 9, 9, 14), Offset = 0.996 });
				return brush;
			}
		}
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new FutureArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FutureArcScaleBackgroundLayerPresentation();
		}
	}
	public class FutureHalfTopArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Future Half Top Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new FutureHalfTopArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FutureHalfTopArcScaleBackgroundLayerPresentation();
		}
	}
	public class FutureQuarterTopRightArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Future Quarter Top Right Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new FutureQuarterTopRightArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FutureQuarterTopRightArcScaleBackgroundLayerPresentation();
		}
	}
	public class FutureQuarterTopLeftArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Future Quarter Top Left Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new FutureQuarterTopLeftArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FutureQuarterTopLeftArcScaleBackgroundLayerPresentation();
		}
	}
	public class FutureThreeQuartersArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Future Three Quarters Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new FutureThreeQuartersArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FutureThreeQuartersArcScaleBackgroundLayerPresentation();
		}
	}
	public class ClassicArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Classic Background"; } }
		protected override Brush DefaultFill {
			get {
				RadialGradientBrush brush = new RadialGradientBrush() { Center = new Point(0.5, 1), GradientOrigin = new Point(0.5, 1), RadiusX = 1, RadiusY = 1 };
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(255, 55, 65, 87), Offset = 0 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(255, 28, 35, 55), Offset = 1 });
				return brush;
			}
		}
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new ClassicArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ClassicArcScaleBackgroundLayerPresentation();
		}
	}
	public class ClassicHalfTopArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Classic Half Top Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new ClassicHalfTopArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ClassicHalfTopArcScaleBackgroundLayerPresentation();
		}
	}
	public class ClassicQuarterTopRightArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Classic Quarter Top Right Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new ClassicQuarterTopRightArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ClassicQuarterTopRightArcScaleBackgroundLayerPresentation();
		}
	}
	public class ClassicQuarterTopLeftArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Classic Quarter Top Left Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new ClassicQuarterTopLeftArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ClassicQuarterTopLeftArcScaleBackgroundLayerPresentation();
		}
	}
	public class ClassicThreeQuartersArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Classic Three Quarters Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new ClassicThreeQuartersArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ClassicThreeQuartersArcScaleBackgroundLayerPresentation();
		}
	}
	public class IStyleArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "IStyle Background"; } }
		protected override Brush DefaultFill {
			get {
				LinearGradientBrush brush = new LinearGradientBrush() { StartPoint = new Point(0.5, 0), EndPoint = new Point(0.5, 0.846) };
				brush.GradientStops.Add(new GradientStop() { Color = Colors.White, Offset = 0 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(255, 231, 228, 231), Offset = 1 });
				return brush;
			}
		}
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new IStyleArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new IStyleArcScaleBackgroundLayerPresentation();
		}
	}
	public class IStyleHalfTopArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "IStyle Half Top Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new IStyleHalfTopArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new IStyleHalfTopArcScaleBackgroundLayerPresentation();
		}
	}
	public class IStyleQuarterTopRightArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "IStyle Quarter Top Right Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new IStyleQuarterTopRightArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new IStyleQuarterTopRightArcScaleBackgroundLayerPresentation();
		}
	}
	public class IStyleQuarterTopLeftArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "IStyle Quarter Top Left Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new IStyleQuarterTopLeftArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new IStyleQuarterTopLeftArcScaleBackgroundLayerPresentation();
		}
	}
	public class IStyleThreeQuartersArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "IStyle Three Quarters Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new IStyleThreeQuartersArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new IStyleThreeQuartersArcScaleBackgroundLayerPresentation();
		}
	}
	public class YellowSubmarineArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Yellow Submarine Background"; } }
		protected override Brush DefaultFill {
			get {
				RadialGradientBrush brush = new RadialGradientBrush() { Center = new Point(0.5, 1), GradientOrigin = new Point(0.5, 1), RadiusX = 1, RadiusY = 1 };
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(255, 255, 208, 119), Offset = 0 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(255, 255, 171, 7), Offset = 1 });
				return brush;
			}
		}
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new YellowSubmarineArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new YellowSubmarineArcScaleBackgroundLayerPresentation();
		}
	}
	public class YellowSubmarineHalfTopArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Yellow Submarine Half Top Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new YellowSubmarineHalfTopArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new YellowSubmarineHalfTopArcScaleBackgroundLayerPresentation();
		}
	}
	public class YellowSubmarineQuarterTopRightArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Yellow Submarine Quarter Top Right Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new YellowSubmarineQuarterTopRightArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new YellowSubmarineQuarterTopRightArcScaleBackgroundLayerPresentation();
		}
	}
	public class YellowSubmarineQuarterTopLeftArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Yellow Submarine Quarter Top Left Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new YellowSubmarineQuarterTopLeftArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new YellowSubmarineQuarterTopLeftArcScaleBackgroundLayerPresentation();
		}
	}
	public class YellowSubmarineThreeQuartersArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Yellow Submarine Three Quarters Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new YellowSubmarineThreeQuartersArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new YellowSubmarineThreeQuartersArcScaleBackgroundLayerPresentation();
		}
	}
	public class MagicLightArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Magic Light Background"; } }
		protected override Brush DefaultFill {
			get {
				RadialGradientBrush brush = new RadialGradientBrush() { Center = new Point(0.5, 1), GradientOrigin = new Point(0.5, 1), RadiusX = 1, RadiusY = 1 };
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(255, 24, 40, 49), Offset = 0 });
				brush.GradientStops.Add(new GradientStop() { Color = Colors.Black, Offset = 1 });
				return brush;
			}
		}
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new MagicLightArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new MagicLightArcScaleBackgroundLayerPresentation();
		}
	}
	public class MagicLightHalfTopArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Magic Light Half Top Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new MagicLightHalfTopArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new MagicLightHalfTopArcScaleBackgroundLayerPresentation();
		}
	}
	public class MagicLightQuarterTopRightArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Magic Light Quarter Top Right Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new MagicLightQuarterTopRightArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new MagicLightQuarterTopRightArcScaleBackgroundLayerPresentation();
		}
	}
	public class MagicLightQuarterTopLeftArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Magic Light Quarter Top Left Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new MagicLightQuarterTopLeftArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new MagicLightQuarterTopLeftArcScaleBackgroundLayerPresentation();
		}
	}
	public class MagicLightThreeQuartersArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "MagicLight Three Quarters Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new MagicLightThreeQuartersArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new MagicLightThreeQuartersArcScaleBackgroundLayerPresentation();
		}
	}
	public class FlatLightArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Flat Light Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.White); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new FlatLightArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FlatLightArcScaleBackgroundLayerPresentation();
		}
	}
	public class FlatLightHalfTopArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Flat Light Half Top Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.White); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new FlatLightHalfTopArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FlatLightHalfTopArcScaleBackgroundLayerPresentation();
		}
	}
	public class FlatLightQuarterTopRightArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Flat Light Quarter Top Right Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.White); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new FlatLightQuarterTopRightArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FlatLightQuarterTopRightArcScaleBackgroundLayerPresentation();
		}
	}
	public class FlatLightQuarterTopLeftArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Flat Light Quarter Top Left Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.White); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new FlatLightQuarterTopLeftArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FlatLightQuarterTopLeftArcScaleBackgroundLayerPresentation();
		}
	}
	public class FlatLightThreeQuartersArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Flat Light Three Quarters Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.White); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new FlatLightThreeQuartersArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FlatLightThreeQuartersArcScaleBackgroundLayerPresentation();
		}
	}
	public class FlatDarkArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Flat Dark Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 78, 78, 78));} }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new FlatDarkArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FlatDarkArcScaleBackgroundLayerPresentation();
		}
	}
	public class FlatDarkHalfTopArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Flat Dark Half Top Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 78, 78, 78)); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new FlatDarkHalfTopArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FlatDarkHalfTopArcScaleBackgroundLayerPresentation();
		}
	}
	public class FlatDarkQuarterTopRightArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Flat Dark Quarter Top Right Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 78, 78, 78)); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new FlatDarkQuarterTopRightArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FlatDarkQuarterTopRightArcScaleBackgroundLayerPresentation();
		}
	}
	public class FlatDarkQuarterTopLeftArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Flat Dark Quarter Top Left Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 78, 78, 78)); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new FlatDarkQuarterTopLeftArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FlatDarkQuarterTopLeftArcScaleBackgroundLayerPresentation();
		}
	}
	public class FlatDarkThreeQuartersArcScaleBackgroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Flat Dark Three Quarters Background"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 78, 78, 78)); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new FlatDarkThreeQuartersArcScaleBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FlatDarkThreeQuartersArcScaleBackgroundLayerPresentation();
		}
	}
	public class CustomArcScaleLayerPresentation : ArcScaleLayerPresentation {
		public static readonly DependencyProperty ScaleLayerTemplateProperty = DependencyPropertyManager.Register("ScaleLayerTemplate",
			typeof(ControlTemplate), typeof(CustomArcScaleLayerPresentation));
		[Category(Categories.Common)]
		public ControlTemplate ScaleLayerTemplate {
			get { return (ControlTemplate)GetValue(ScaleLayerTemplateProperty); }
			set { SetValue(ScaleLayerTemplateProperty, value); }
		}
		public override string PresentationName { get { return "Custom"; } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			CustomPresentationControl modelControl = new CustomPresentationControl();
			modelControl.SetBinding(CustomPresentationControl.TemplateProperty, new Binding("ScaleLayerTemplate") { Source = this });
			return modelControl;
		}
		protected override GaugeDependencyObject CreateObject() {
			return new CustomArcScaleLayerPresentation();
		}
	}
}
