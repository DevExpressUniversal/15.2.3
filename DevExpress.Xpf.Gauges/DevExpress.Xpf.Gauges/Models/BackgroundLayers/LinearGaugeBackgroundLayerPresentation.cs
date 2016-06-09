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
	public abstract class LinearGaugeLayerPresentation : LayerPresentation {
	}
	public abstract class PredefinedLinearGaugeLayerPresentation : LinearGaugeLayerPresentation {
		public static readonly DependencyProperty FillProperty = DependencyPropertyManager.Register("Fill",
			typeof(Brush), typeof(PredefinedLinearGaugeLayerPresentation), new PropertyMetadata(null, FillPropertyChanged));
		[Category(Categories.Presentation)]
		public Brush Fill {
			get { return (Brush)GetValue(FillProperty); }
			set { SetValue(FillProperty, value); }
		}
		static void FillPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PredefinedLinearGaugeLayerPresentation presentation = d as PredefinedLinearGaugeLayerPresentation;
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
	public class DefaultLinearGaugeBackgroundLayerPresentation : PredefinedLinearGaugeLayerPresentation {
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(0xFF, 0xF7, 0xF7, 0xF7)); } }
		public override string PresentationName { get { return "Default Background"; } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new DefaultGaugeBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new DefaultLinearGaugeBackgroundLayerPresentation();
		}
	}
	public class CleanWhiteLinearGaugeBackgroundLayerPresentation : PredefinedLinearGaugeLayerPresentation {
		protected override Brush DefaultFill {
			get {
				RadialGradientBrush brush = new RadialGradientBrush() { Center = new Point(0.5, 0), GradientOrigin = new Point(0.5, 0), RadiusX = 2, RadiusY = 1 };
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0xF3, 0xF5, 0xF8), Offset = 1 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0xF8, 0xF8, 0xF9), Offset = 0 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0xEA, 0xEA, 0xED), Offset = 0.3801 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0xF0, 0xF1, 0xF3), Offset = 0.38 });
				return brush;
			}
		}
		public override string PresentationName { get { return "Clean White Background"; } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new CleanWhiteGaugeBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new CleanWhiteLinearGaugeBackgroundLayerPresentation();
		}
	}
	public class CosmicLinearGaugeBackgroundLayerPresentation : PredefinedLinearGaugeLayerPresentation {
		protected override Brush DefaultFill {
			get {
				RadialGradientBrush brush = new RadialGradientBrush() { Center = new Point(0.5, 0), GradientOrigin = new Point(0.5, 0), RadiusX = 2, RadiusY = 1 };
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0x11, 0x4B, 0x63), Offset = 1 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0x0C, 0x9C, 0xB1), Offset = 0 });
				return brush;
			}
		}
		public override string PresentationName { get { return "Cosmic Background"; } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new CosmicGaugeBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new CosmicLinearGaugeBackgroundLayerPresentation();
		}
	}
	public class SmartLinearGaugeBackgroundLayerPresentation : PredefinedLinearGaugeLayerPresentation {
		protected override Brush DefaultFill {
			get {
				RadialGradientBrush brush = new RadialGradientBrush() { Center = new Point(0.5, 0), GradientOrigin = new Point(0.5, 0), RadiusX = 2.862, RadiusY = 1 };
				brush.GradientStops.Add(new GradientStop() { Color = Colors.White, Offset = 1 });
				brush.GradientStops.Add(new GradientStop() { Color = Colors.White, Offset = 0 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0xDC, 0xDE, 0xE5), Offset = 0.32 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0xD3, 0xD6, 0xDE), Offset = 0.32001 });
				return brush;
			}
		}
		public override string PresentationName { get { return "Smart Background"; } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new SmartGaugeBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new SmartLinearGaugeBackgroundLayerPresentation();
		}
	}
	public class ProgressiveLinearGaugeBackgroundLayerPresentation : PredefinedLinearGaugeLayerPresentation {
		protected override Brush DefaultFill {
			get {
				RadialGradientBrush brush = new RadialGradientBrush() { Center = new Point(0.5, 0), GradientOrigin = new Point(0.5, 0), RadiusX = 2.862, RadiusY = 1 };
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0x06, 0x1C, 0x2B), Offset = 1 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0x5D, 0x66, 0x7D), Offset = 0.012 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0x11, 0x1B, 0x25), Offset = 0.32 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0x00, 0x02, 0x07), Offset = 0.32001 });
				return brush;
			}
		}
		public override string PresentationName { get { return "Progressive Background"; } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new ProgressiveGaugeBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ProgressiveLinearGaugeBackgroundLayerPresentation();
		}
	}
	public class EcoLinearGaugeBackgroundLayerPresentation : PredefinedLinearGaugeLayerPresentation {
		protected override Brush DefaultFill {
			get {
				RadialGradientBrush brush = new RadialGradientBrush() { Center = new Point(0.5, 0), GradientOrigin = new Point(0.5, 0), RadiusX = 2.862, RadiusY = 1 };
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0xFA, 0xF8, 0xEE), Offset = 1 });
				brush.GradientStops.Add(new GradientStop() { Color = Colors.White, Offset = 0 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0xF7, 0xF4, 0xE7), Offset = 0.35 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0xF0, 0xEB, 0xD3), Offset = 0.35001 });
				return brush;
			}
		}
		public override string PresentationName { get { return "Eco Background"; } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new EcoGaugeBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new EcoLinearGaugeBackgroundLayerPresentation();
		}
	}
	public class FutureLinearGaugeBackgroundLayerPresentation : PredefinedLinearGaugeLayerPresentation {
		protected override Brush DefaultFill {
			get {
				RadialGradientBrush brush = new RadialGradientBrush() { Center = new Point(0.5, 0), GradientOrigin = new Point(0.5, 0), RadiusX = 2.862, RadiusY = 1 };
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0x32, 0x2F, 0x57), Offset = 1 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0x39, 0x40, 0x64), Offset = 0 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0x18, 0x1B, 0x2B), Offset = 0.32 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0x12, 0x11, 0x1B), Offset = 0.32001 });
				return brush;
			}
		}
		public override string PresentationName { get { return "Future Background"; } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new FutureGaugeBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FutureLinearGaugeBackgroundLayerPresentation();
		}
	}
	public class ClassicLinearGaugeBackgroundLayerPresentation : PredefinedLinearGaugeLayerPresentation {
		protected override Brush DefaultFill {
			get {
				RadialGradientBrush brush = new RadialGradientBrush() { Center = new Point(0.5, 1.001), GradientOrigin = new Point(0.5, 1.001), RadiusX = 2.862, RadiusY = 1 };
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0x1C, 0x23, 0x37), Offset = 1 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0x37, 0x41, 0x57) });
				return brush;
			}
		}
		public override string PresentationName { get { return "Classic Background"; } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new ClassicGaugeBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ClassicLinearGaugeBackgroundLayerPresentation();
		}
	}
	public class IStyleLinearGaugeBackgroundLayerPresentation : PredefinedLinearGaugeLayerPresentation {
		protected override Brush DefaultFill {
			get {
				RadialGradientBrush brush = new RadialGradientBrush() { Center = new Point(0.5, 0), GradientOrigin = new Point(0.5, 0), RadiusX = 2.862, RadiusY = 1 };
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0xE7, 0xE4, 0xE7), Offset = 1 });
				brush.GradientStops.Add(new GradientStop() { Color = Colors.White, Offset = 0 });
				return brush;
			}
		}
		public override string PresentationName { get { return "IStyle Background"; } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new IStyleGaugeBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new IStyleLinearGaugeBackgroundLayerPresentation();
		}
	}
	public class YellowSubmarineLinearGaugeBackgroundLayerPresentation : PredefinedLinearGaugeLayerPresentation {
		protected override Brush DefaultFill {
			get {
				RadialGradientBrush brush = new RadialGradientBrush() { Center = new Point(0.5, 1.001), GradientOrigin = new Point(0.5, 1.001), RadiusX = 2.862, RadiusY = 1 };
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0xFF, 0xAB, 0x07), Offset = 1 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0xFF, 0xD0, 0x77), Offset = 0 });
				return brush;
			}
		}
		public override string PresentationName { get { return "Yellow Submarine Background"; } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new YellowSubmarineGaugeBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new YellowSubmarineLinearGaugeBackgroundLayerPresentation();
		}
	}
	public class MagicLightLinearGaugeBackgroundLayerPresentation : PredefinedLinearGaugeLayerPresentation {
		protected override Brush DefaultFill {
			get {
				RadialGradientBrush brush = new RadialGradientBrush() { Center = new Point(0.5, 0), GradientOrigin = new Point(0.5, 0), RadiusX = 2.862, RadiusY = 1 };
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0x18, 0x28, 0x31), Offset = 1 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0x2B, 0x46, 0x4B), Offset = 0 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0x11, 0x1D, 0x20), Offset = 0.35 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0x08, 0x0D, 0x10), Offset = 0.3501 });
				return brush;
			}
		}
		public override string PresentationName { get { return "Magic Light Background"; } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new MagicLightGaugeBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new MagicLightLinearGaugeBackgroundLayerPresentation();
		}
	}
	public class RedThermometerLinearGaugeBackgroundLayerPresentation : PredefinedLinearGaugeLayerPresentation {
		protected override Brush DefaultFill {
			get {
				RadialGradientBrush brush = new RadialGradientBrush() { Center = new Point(0.5, 0), GradientOrigin = new Point(0.5, 0), RadiusX = 2, RadiusY = 1 };
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0x99, 0xFF, 0xFF, 0xFF) });
				brush.GradientStops.Add(new GradientStop() { Color = Colors.Transparent, Offset = 0.3801 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0x4C, 0xFF, 0xFF, 0xFF), Offset = 0.38 });
				return brush;
			}
		}
		public override string PresentationName { get { return "Red Thermometer Background"; } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new RedThermometerGaugeBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new RedThermometerLinearGaugeBackgroundLayerPresentation();
		}
	}
	public class FlatLightLinearGaugeBackgroundLayerPresentation : PredefinedLinearGaugeLayerPresentation {
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.White); } }
		public override string PresentationName { get { return "Flat Light Background"; } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new FlatLightGaugeBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FlatLightLinearGaugeBackgroundLayerPresentation();
		}
	}
	public class FlatDarkLinearGaugeBackgroundLayerPresentation : PredefinedLinearGaugeLayerPresentation {
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Black); } }
		public override string PresentationName { get { return "Flat Dark Background"; } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new FlatDarkGaugeBackgroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FlatDarkLinearGaugeBackgroundLayerPresentation();
		}
	}
	public class CustomLinearGaugeLayerPresentation : LinearGaugeLayerPresentation {
		public static readonly DependencyProperty GaugeLayerTemplateProperty = DependencyPropertyManager.Register("GaugeLayerTemplate",
			typeof(ControlTemplate), typeof(CustomLinearGaugeLayerPresentation));
		[Category(Categories.Common)]
		public ControlTemplate GaugeLayerTemplate {
			get { return (ControlTemplate)GetValue(GaugeLayerTemplateProperty); }
			set { SetValue(GaugeLayerTemplateProperty, value); }
		}
		public override string PresentationName { get { return "Custom"; } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			CustomPresentationControl modelControl = new CustomPresentationControl();
			modelControl.SetBinding(CustomPresentationControl.TemplateProperty, new Binding("GaugeLayerTemplate") { Source = this });
			return modelControl;
		}
		protected override GaugeDependencyObject CreateObject() {
			return new CustomLinearGaugeLayerPresentation();
		}
	}
}
