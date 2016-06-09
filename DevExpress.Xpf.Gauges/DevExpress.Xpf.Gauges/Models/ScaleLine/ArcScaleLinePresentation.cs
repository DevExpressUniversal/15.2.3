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
	public abstract class ScaleLinePresentation : PresentationBase {
		protected internal abstract PresentationControl CreateLinePresentationControl();
	}
	public abstract class ArcScaleLinePresentation : ScaleLinePresentation {
	}
	public abstract class PredefinedArcScaleLinePresentation : ArcScaleLinePresentation {
		public static readonly DependencyProperty FillProperty = DependencyPropertyManager.Register("Fill",
			typeof(Brush), typeof(PredefinedArcScaleLinePresentation), new PropertyMetadata(null, FillPropertyChanged));
		[Category(Categories.Presentation)]
		public Brush Fill {
			get { return (Brush)GetValue(FillProperty); }
			set { SetValue(FillProperty, value); }
		}
		static void FillPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PredefinedArcScaleLinePresentation presentation = d as PredefinedArcScaleLinePresentation;
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
	public class DefaultArcScaleLinePresentation : PredefinedArcScaleLinePresentation {
		public override string PresentationName { get { return "Default Scale Line"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 67, 67, 67)); } }
		protected internal override PresentationControl CreateLinePresentationControl() {
			return new DefaultArcScaleLineControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new DefaultArcScaleLinePresentation();
		}
	}
	public class CleanWhiteArcScaleLinePresentation : PredefinedArcScaleLinePresentation {
		public override string PresentationName { get { return "Clean White Scale Line"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 71, 71, 71)); } }
		protected internal override PresentationControl CreateLinePresentationControl() {
			return new CleanWhiteArcScaleLineControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new CleanWhiteArcScaleLinePresentation();
		}
	}
	public class CosmicArcScaleLinePresentation : PredefinedArcScaleLinePresentation {
		public override string PresentationName { get { return "Cosmic Scale Line"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 71, 71, 71)); } }
		protected internal override PresentationControl CreateLinePresentationControl() {
			return new CosmicArcScaleLineControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new CosmicArcScaleLinePresentation();
		}
	}
	public class SmartArcScaleLinePresentation : PredefinedArcScaleLinePresentation {
		public override string PresentationName { get { return "Smart Scale Line"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 89, 97, 111)); } }
		protected internal override PresentationControl CreateLinePresentationControl() {
			return new SmartArcScaleLineControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new SmartArcScaleLinePresentation();
		}
	}
	public class ProgressiveArcScaleLinePresentation : PredefinedArcScaleLinePresentation {
		public override string PresentationName { get { return "Progressive Scale Line"; } }
		protected override Brush DefaultFill { get { 
		RadialGradientBrush brush = new RadialGradientBrush() { Center = new Point(0.5, 0), GradientOrigin = new Point(0.5, 0), RadiusX = 1, RadiusY = 1 };
				brush.GradientStops.Add(new GradientStop() {Color=Color.FromArgb(255, 101, 203, 246)});
				brush.GradientStops.Add(new GradientStop() {Color=Color.FromArgb(255, 43, 90, 112), Offset=1});				
				return brush;
			 } }
		protected internal override PresentationControl CreateLinePresentationControl() {
			return new ProgressiveArcScaleLineControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ProgressiveArcScaleLinePresentation();
		}
	}
	public class EcoArcScaleLinePresentation : PredefinedArcScaleLinePresentation {
		public override string PresentationName { get { return "Eco Scale Line"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 58, 56, 50)); } }
		protected internal override PresentationControl CreateLinePresentationControl() {
			return new EcoArcScaleLineControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new EcoArcScaleLinePresentation();
		}
	}
	public class FutureArcScaleLinePresentation : PredefinedArcScaleLinePresentation {
		public override string PresentationName { get { return "Future Scale Line"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 58, 56, 50)); } }
		protected internal override PresentationControl CreateLinePresentationControl() {
			return new FutureArcScaleLineControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FutureArcScaleLinePresentation();
		}
	}
	public class ClassicArcScaleLinePresentation : PredefinedArcScaleLinePresentation {
		public override string PresentationName { get { return "Classic Scale Line"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.White); } }
		protected internal override PresentationControl CreateLinePresentationControl() {
			return new ClassicArcScaleLineControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ClassicArcScaleLinePresentation();
		}
	}
	public class IStyleArcScaleLinePresentation : PredefinedArcScaleLinePresentation {
		public override string PresentationName { get { return "IStyle Scale Line"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.White); } }
		protected internal override PresentationControl CreateLinePresentationControl() {
			return new IStyleArcScaleLineControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new IStyleArcScaleLinePresentation();
		}
	}
	public class YellowSubmarineArcScaleLinePresentation : PredefinedArcScaleLinePresentation {
		public override string PresentationName { get { return "Yellow Submarine Scale Line"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.White); } }
		protected internal override PresentationControl CreateLinePresentationControl() {
			return new YellowSubmarineArcScaleLineControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new YellowSubmarineArcScaleLinePresentation();
		}
	}
	public class MagicLightArcScaleLinePresentation : PredefinedArcScaleLinePresentation {
		public override string PresentationName { get { return "Magic Light Scale Line"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.White); } }
		protected internal override PresentationControl CreateLinePresentationControl() {
			return new MagicLightArcScaleLineControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new MagicLightArcScaleLinePresentation();
		}
	}
	public class FlatLightArcScaleLinePresentation : PredefinedArcScaleLinePresentation {
		public override string PresentationName { get { return "Flat Light Scale Line"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 153, 153, 153)); } }
		protected internal override PresentationControl CreateLinePresentationControl() {
			return new FlatLightArcScaleLineControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FlatLightArcScaleLinePresentation();
		}
	}
	public class FlatDarkArcScaleLinePresentation : PredefinedArcScaleLinePresentation {
		public override string PresentationName { get { return "Flat Dark Scale Line"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 78, 78, 78)); } }
		protected internal override PresentationControl CreateLinePresentationControl() {
			return new FlatDarkArcScaleLineControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FlatDarkArcScaleLinePresentation();
		}
	}
	public class CustomArcScaleLinePresentation : ArcScaleLinePresentation {
		public static readonly DependencyProperty LineTemplateProperty = DependencyPropertyManager.Register("LineTemplate",
			typeof(ControlTemplate), typeof(CustomArcScaleLinePresentation));
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
			return new CustomArcScaleLinePresentation();
		}
	}
}
