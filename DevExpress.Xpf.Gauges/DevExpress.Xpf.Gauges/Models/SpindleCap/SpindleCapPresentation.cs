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
	public abstract class SpindleCapPresentation : LayerPresentation {
	}
	public abstract class PredefinedSpindleCapPresentation : SpindleCapPresentation {
		public static readonly DependencyProperty FillProperty = DependencyPropertyManager.Register("Fill",
			typeof(Brush), typeof(PredefinedSpindleCapPresentation), new PropertyMetadata(null, FillPropertyChanged));
		[Category(Categories.Presentation)]
		public Brush Fill {
			get { return (Brush)GetValue(FillProperty); }
			set { SetValue(FillProperty, value); }
		}
		static void FillPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PredefinedSpindleCapPresentation presentation = d as PredefinedSpindleCapPresentation;
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
	public class DefaultSpindleCapPresentation : PredefinedSpindleCapPresentation {
		public override string PresentationName { get { return "Default Spindle Cap"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 85, 85, 85)); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new DefaultSpindleCapControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new DefaultSpindleCapPresentation();
		}
	}
	public class CleanWhiteSpindleCapPresentation : PredefinedSpindleCapPresentation {
		public override string PresentationName { get { return "Clean White Spindle Cap"; } }
		protected override Brush DefaultFill {
			get {
				RadialGradientBrush brush = new RadialGradientBrush() { Center = new Point(0.498, 1), GradientOrigin = new Point(0.498, 1), RadiusX = 1.003, RadiusY = 1.003 };
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(255, 205, 207, 214), Offset = 0.991 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(255, 244, 245, 246) });
				return brush;
			}
		}
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new CleanWhiteSpindleCapControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new CleanWhiteSpindleCapPresentation();
		}
	}
	public class CosmicSpindleCapPresentation : PredefinedSpindleCapPresentation {
		public override string PresentationName { get { return "Cosmic Spindle Cap"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 5, 136, 151)); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new CosmicSpindleCapControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new CosmicSpindleCapPresentation();
		}
	}
	public class SmartSpindleCapPresentation : PredefinedSpindleCapPresentation {
		public override string PresentationName { get { return "Smart Spindle Cap"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 54, 59, 76)); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new SmartSpindleCapControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new SmartSpindleCapPresentation();
		}
	}
	public class RedClockSpindleCapPresentation : PredefinedSpindleCapPresentation {
		public override string PresentationName { get { return "Red Clock Spindle Cap"; } }
		protected override Brush DefaultFill {
			get {
				RadialGradientBrush brush = new RadialGradientBrush() { Center = new Point(0.5, 0), GradientOrigin = new Point(0.5, 0), RadiusX = 1, RadiusY = 1 };
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(255, 165, 1, 1), Offset = 0.991 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(255, 250, 127, 115) });
				return brush;
			}
		}
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new RedClockSpindleCapControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new RedClockSpindleCapPresentation();
		}
	}
	public class ProgressiveSpindleCapPresentation : PredefinedSpindleCapPresentation {
		public override string PresentationName { get { return "Progressive Spindle Cap"; } }
		protected override Brush DefaultFill {
			get {
				RadialGradientBrush brush = new RadialGradientBrush() { Center = new Point(0.498, 1), GradientOrigin = new Point(0.498, 1), RadiusX = 1.003, RadiusY = 1.003 };
				brush.GradientStops.Add(new GradientStop() { Color = Colors.Black, Offset = 0.991 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(255, 6, 30, 43) });
				return brush;
			}
		}
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new ProgressiveSpindleCapControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ProgressiveSpindleCapPresentation();
		}
	}
	public class EcoSpindleCapPresentation : PredefinedSpindleCapPresentation {
		public override string PresentationName { get { return "Eco Spindle Cap"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 67, 65, 60)); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new EcoSpindleCapControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new EcoSpindleCapPresentation();
		}
	}
	public class FutureSpindleCapPresentation : PredefinedSpindleCapPresentation {
		public override string PresentationName { get { return "Future Spindle Cap"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 67, 65, 60)); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new FutureSpindleCapControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FutureSpindleCapPresentation();
		}
	}
	public class ClassicSpindleCapPresentation : PredefinedSpindleCapPresentation {
		public override string PresentationName { get { return "Classic Spindle Cap"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 42, 51, 72)); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new ClassicSpindleCapControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ClassicSpindleCapPresentation();
		}
	}
	public class IStyleSpindleCapPresentation : PredefinedSpindleCapPresentation {
		public override string PresentationName { get { return "IStyle Spindle Cap"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 26, 24, 26)); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new IStyleSpindleCapControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new IStyleSpindleCapPresentation();
		}
	}
	public class YellowSubmarineSpindleCapPresentation : PredefinedSpindleCapPresentation {
		public override string PresentationName { get { return "Yellow Submarine Spindle Cap"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 42, 51, 72)); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new YellowSubmarineSpindleCapControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new YellowSubmarineSpindleCapPresentation();
		}
	}
	public class MagicLightSpindleCapPresentation : PredefinedSpindleCapPresentation {
		public override string PresentationName { get { return "Magic Light Spindle Cap"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 42, 51, 72)); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new MagicLightSpindleCapControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new MagicLightSpindleCapPresentation();
		}
	}
	public class FlatLightSpindleCapPresentation : PredefinedSpindleCapPresentation {
		public override string PresentationName { get { return "Flat Light Spindle Cap"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.White); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new FlatLightSpindleCapControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FlatLightSpindleCapPresentation();
		}
	}
	public class FlatDarkSpindleCapPresentation : PredefinedSpindleCapPresentation {
		public override string PresentationName { get { return "Flat Dark Spindle Cap"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 78, 78, 78)); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new FlatDarkSpindleCapControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FlatDarkSpindleCapPresentation();
		}
	}
	public class CustomSpindleCapPresentation : SpindleCapPresentation {
		public static readonly DependencyProperty SpindleCapTemplateProperty = DependencyPropertyManager.Register("SpindleCapTemplate",
			typeof(ControlTemplate), typeof(CustomSpindleCapPresentation));
		[Category(Categories.Common)]
		public ControlTemplate SpindleCapTemplate {
			get { return (ControlTemplate)GetValue(SpindleCapTemplateProperty); }
			set { SetValue(SpindleCapTemplateProperty, value); }
		}
		public override string PresentationName { get { return "Custom Spindle Cap"; } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			CustomPresentationControl modelControl = new CustomPresentationControl();
			modelControl.SetBinding(CustomPresentationControl.TemplateProperty, new Binding("SpindleCapTemplate") { Source = this });
			return modelControl;
		}
		protected override GaugeDependencyObject CreateObject() {
			return new CustomSpindleCapPresentation();
		}
	}
}
