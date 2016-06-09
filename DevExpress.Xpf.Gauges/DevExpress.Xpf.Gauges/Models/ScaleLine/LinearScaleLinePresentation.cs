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
	public abstract class LinearScaleLinePresentation : ScaleLinePresentation {
	}
	public abstract class PredefinedLinearScaleLinePresentation : LinearScaleLinePresentation {
		public static readonly DependencyProperty FillProperty = DependencyPropertyManager.Register("Fill",
			typeof(Brush), typeof(PredefinedLinearScaleLinePresentation), new PropertyMetadata(null, FillPropertyChanged));
		[Category(Categories.Presentation)]
		public Brush Fill {
			get { return (Brush)GetValue(FillProperty); }
			set { SetValue(FillProperty, value); }
		}
		static void FillPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PredefinedLinearScaleLinePresentation presentation = d as PredefinedLinearScaleLinePresentation;
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
	public class DefaultLinearScaleLinePresentation : PredefinedLinearScaleLinePresentation {
		public override string PresentationName { get { return "Default Scale Line"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 67, 67, 67)); } }
		protected internal override PresentationControl CreateLinePresentationControl() {
			return new DefaultLinearScaleLineControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new DefaultLinearScaleLinePresentation();
		}
	}
	public class CleanWhiteLinearScaleLinePresentation : PredefinedLinearScaleLinePresentation {
		public override string PresentationName { get { return "Clean White Scale Line"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 71, 71, 71)); } }
		protected internal override PresentationControl CreateLinePresentationControl() {
			return new CleanWhiteLinearScaleLineControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new CleanWhiteLinearScaleLinePresentation();
		}
	}
	public class CosmicLinearScaleLinePresentation : PredefinedLinearScaleLinePresentation {
		public override string PresentationName { get { return "Cosmic Scale Line"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 71, 71, 71)); } }
		protected internal override PresentationControl CreateLinePresentationControl() {
			return new CosmicLinearScaleLineControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new CosmicLinearScaleLinePresentation();
		}
	}
	public class SmartLinearScaleLinePresentation : PredefinedLinearScaleLinePresentation {
		public override string PresentationName { get { return "Smart Scale Line"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 89, 97, 111)); } }
		protected internal override PresentationControl CreateLinePresentationControl() {
			return new SmartLinearScaleLineControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new SmartLinearScaleLinePresentation();
		}
	}
	public class ProgressiveLinearScaleLinePresentation : PredefinedLinearScaleLinePresentation {
		public override string PresentationName { get { return "Progressive Scale Line"; } }
		protected override Brush DefaultFill {
			get {
				LinearGradientBrush brush = new LinearGradientBrush() { StartPoint = new Point(0.5, 0), EndPoint = new Point(0.5, 1)};
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0x65, 0xCB, 0xF6), Offset = 0 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0x36, 0xA4, 0xCB), Offset = 1 });
				return brush;
			}
		}
		protected internal override PresentationControl CreateLinePresentationControl() {
			return new ProgressiveLinearScaleLineControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ProgressiveLinearScaleLinePresentation();
		}
	}
	public class EcoLinearScaleLinePresentation : PredefinedLinearScaleLinePresentation {
		public override string PresentationName { get { return "Eco Scale Line"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 58, 56, 50)); } }
		protected internal override PresentationControl CreateLinePresentationControl() {
			return new EcoLinearScaleLineControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new EcoLinearScaleLinePresentation();
		}
	}
	public class FutureLinearScaleLinePresentation : PredefinedLinearScaleLinePresentation {
		public override string PresentationName { get { return "Future Scale Line"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 58, 56, 50)); } }
		protected internal override PresentationControl CreateLinePresentationControl() {
			return new FutureLinearScaleLineControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FutureLinearScaleLinePresentation();
		}
	}
	public class ClassicLinearScaleLinePresentation : PredefinedLinearScaleLinePresentation {
		public override string PresentationName { get { return "Classic Scale Line"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.White); } }
		protected internal override PresentationControl CreateLinePresentationControl() {
			return new ClassicLinearScaleLineControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ClassicLinearScaleLinePresentation();
		}
	}
	public class IStyleLinearScaleLinePresentation : PredefinedLinearScaleLinePresentation {
		public override string PresentationName { get { return "IStyle Scale Line"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.White); } }
		protected internal override PresentationControl CreateLinePresentationControl() {
			return new IStyleLinearScaleLineControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new IStyleLinearScaleLinePresentation();
		}
	}
	public class YellowSubmarineLinearScaleLinePresentation : PredefinedLinearScaleLinePresentation {
		public override string PresentationName { get { return "Yellow Submarine Scale Line"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.White); } }
		protected internal override PresentationControl CreateLinePresentationControl() {
			return new YellowSubmarineLinearScaleLineControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new YellowSubmarineLinearScaleLinePresentation();
		}
	}
	public class MagicLightLinearScaleLinePresentation : PredefinedLinearScaleLinePresentation {
		public override string PresentationName { get { return "Magic Light Scale Line"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.White); } }
		protected internal override PresentationControl CreateLinePresentationControl() {
			return new MagicLightLinearScaleLineControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new MagicLightLinearScaleLinePresentation();
		}
	}
	public class RedThermometerLinearScaleLinePresentation : PredefinedLinearScaleLinePresentation {
		public override string PresentationName { get { return "Red Thermometer Scale Line"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 58, 56, 50)); } }
		protected internal override PresentationControl CreateLinePresentationControl() {
			return new RedThermometerLinearScaleLineControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new MagicLightLinearScaleLinePresentation();
		}
	}
	public class FlatLightLinearScaleLinePresentation : PredefinedLinearScaleLinePresentation {
		public override string PresentationName { get { return "Flat Light Scale Line"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 153, 153, 153)); } }
		protected internal override PresentationControl CreateLinePresentationControl() {
			return new FlatLightLinearScaleLineControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FlatLightLinearScaleLinePresentation();
		}
	}
	public class FlatDarkLinearScaleLinePresentation : PredefinedLinearScaleLinePresentation {
		public override string PresentationName { get { return "Flat Dark Scale Line"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 78, 78, 78)); } }
		protected internal override PresentationControl CreateLinePresentationControl() {
			return new FlatDarkLinearScaleLineControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FlatDarkLinearScaleLinePresentation();
		}
	}
	public class CustomLinearScaleLinePresentation : LinearScaleLinePresentation {
		public static readonly DependencyProperty LineTemplateProperty = DependencyPropertyManager.Register("LineTemplate",
			typeof(ControlTemplate), typeof(CustomLinearScaleLinePresentation));
		[Category(Categories.Common)]
		public ControlTemplate LineTemplate {
			get { return (ControlTemplate)GetValue(LineTemplateProperty); }
			set { SetValue(LineTemplateProperty, value); }
		}
		public override string PresentationName { get { return "Custom Line"; } }
		protected internal override PresentationControl CreateLinePresentationControl() {
			CustomPresentationControl modelControl = new CustomPresentationControl();
			modelControl.SetBinding(CustomPresentationControl.TemplateProperty, new Binding("LineTemplate") { Source = this });
			return modelControl;
		}
		protected override GaugeDependencyObject CreateObject() {
			return new CustomLinearScaleLinePresentation();
		}
	}
}
