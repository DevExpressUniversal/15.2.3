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
	public abstract class LinearScaleLevelBarPresentation : ValueIndicatorPresentation {
		protected internal abstract PresentationControl CreateForegroundPresentationControl();
	}
	public abstract class PredefinedLinearScaleLevelBarPresentation : LinearScaleLevelBarPresentation {
		public static readonly DependencyProperty FillProperty = DependencyPropertyManager.Register("Fill",
		  typeof(Brush), typeof(PredefinedLinearScaleLevelBarPresentation), new PropertyMetadata(null, FillPropertyChanged));
		[Category(Categories.Presentation)]
		public Brush Fill {
			get { return (Brush)GetValue(FillProperty); }
			set { SetValue(FillProperty, value); }
		}
		static void FillPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PredefinedLinearScaleLevelBarPresentation presentation = d as PredefinedLinearScaleLevelBarPresentation;
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
	public class DefaultLinearScaleLevelBarPresentation : PredefinedLinearScaleLevelBarPresentation {
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(0xFF, 0x93, 0xC7, 0xE2)); } }
		public override string PresentationName { get { return "Default Level Bar"; } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new DefaultLinearScaleLevelBarBackgroundControl();
		}
		protected internal override PresentationControl CreateForegroundPresentationControl() {
			return new DefaultLinearScaleLevelBarForegroundControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new DefaultLinearScaleLevelBarPresentation();
		}
	}
	public class CleanWhiteLinearScaleLevelBarPresentation : PredefinedLinearScaleLevelBarPresentation {
		protected override Brush DefaultFill {
			get {
				LinearGradientBrush brush = new LinearGradientBrush() { StartPoint = new Point(0.5, 1), EndPoint = new Point(0.5, 0) };
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0x6E, 0x75, 0x83), Offset = 0 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0x5A, 0x62, 0x70), Offset = 1 });
				return brush;
			}
		}
		public override string PresentationName { get { return "Clean White Level Bar"; } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new CleanWhiteLinearScaleLevelBarBackgroundControl();
		}
		protected internal override PresentationControl CreateForegroundPresentationControl() {
			return new CleanWhiteLinearScaleLevelBarForegroundControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new CleanWhiteLinearScaleLevelBarPresentation();
		}
	}
	public class CosmicLinearScaleLevelBarPresentation : PredefinedLinearScaleLevelBarPresentation {
		protected override Brush DefaultFill {
			get {
				LinearGradientBrush brush = new LinearGradientBrush() { StartPoint = new Point(0.5, 1), EndPoint = new Point(0.5, 0), MappingMode = BrushMappingMode.RelativeToBoundingBox };
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0x16, 0x1C, 0x1F) });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0x32, 0x41, 0x46), Offset = 1 });
				return brush;
			}
		}
		public override string PresentationName { get { return "Cosmic Level Bar"; } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new CosmicLinearScaleLevelBarBackgroundControl();
		}
		protected internal override PresentationControl CreateForegroundPresentationControl() {
			return new CosmicLinearScaleLevelBarForegroundControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new CosmicLinearScaleLevelBarPresentation();
		}
	}
	public class SmartLinearScaleLevelBarPresentation : PredefinedLinearScaleLevelBarPresentation {
		protected override Brush DefaultFill {
			get {
				LinearGradientBrush brush = new LinearGradientBrush() { StartPoint = new Point(0.5, 1), EndPoint = new Point(0.5, 0) };
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0xD2, 0x5D, 0x52), Offset = 0 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0xB6, 0x38, 0x2C), Offset = 1 });
				return brush;
			}
		}
		public override string PresentationName { get { return "Smart Level Bar"; } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new SmartLinearScaleLevelBarBackgroundControl();
		}
		protected internal override PresentationControl CreateForegroundPresentationControl() {
			return new SmartLinearScaleLevelBarForegroundControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new SmartLinearScaleLevelBarPresentation();
		}
	}
	public class ProgressiveLinearScaleLevelBarPresentation : PredefinedLinearScaleLevelBarPresentation {
		protected override Brush DefaultFill {
			get {
				LinearGradientBrush brush = new LinearGradientBrush() { StartPoint = new Point(0.5, 0), EndPoint = new Point(0.5, 1) };
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0x65, 0xCB, 0xF6), Offset = 0 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0x36, 0xA4, 0xCB), Offset = 1 });
				return brush;
			}
		}
		public override string PresentationName { get { return "Progressive Level Bar"; } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new ProgressiveLinearScaleLevelBarBackgroundControl();
		}
		protected internal override PresentationControl CreateForegroundPresentationControl() {
			return new ProgressiveLinearScaleLevelBarForegroundControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ProgressiveLinearScaleLevelBarPresentation();
		}
	}
	public class EcoLinearScaleLevelBarPresentation : PredefinedLinearScaleLevelBarPresentation {
		protected override Brush DefaultFill {
			get {
				LinearGradientBrush brush = new LinearGradientBrush() { StartPoint = new Point(0.5, 0), EndPoint = new Point(0.5, 1) };
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0x7A, 0x6F, 0x61), Offset = 0 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0x3D, 0x39, 0x38), Offset = 1 });
				return brush;
			}
		}
		public override string PresentationName { get { return "Eco Level Bar"; } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new EcoLinearScaleLevelBarBackgroundControl();
		}
		protected internal override PresentationControl CreateForegroundPresentationControl() {
			return new EcoLinearScaleLevelBarForegroundControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new EcoLinearScaleLevelBarPresentation();
		}
	}
	public class FutureLinearScaleLevelBarPresentation : PredefinedLinearScaleLevelBarPresentation {
		protected override Brush DefaultFill {
			get {
				LinearGradientBrush brush = new LinearGradientBrush() { StartPoint = new Point(0.5, 0), EndPoint = new Point(0.5, 1) };
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0x75, 0xA6, 0xDD), Offset = 0 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0x51, 0x70, 0xA9), Offset = 1 });
				return brush;
			}
		}
		public override string PresentationName { get { return "Future Level Bar"; } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new FutureLinearScaleLevelBarBackgroundControl();
		}
		protected internal override PresentationControl CreateForegroundPresentationControl() {
			return new FutureLinearScaleLevelBarForegroundControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FutureLinearScaleLevelBarPresentation();
		}
	}
	public class ClassicLinearScaleLevelBarPresentation : PredefinedLinearScaleLevelBarPresentation {
		protected override Brush DefaultFill {
			get {
				LinearGradientBrush brush = new LinearGradientBrush() { StartPoint = new Point(0.5, 0), EndPoint = new Point(0.5, 1) };
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0xA8, 0xB7, 0xE0), Offset = 0 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0x81, 0x93, 0xC6), Offset = 1 });
				return brush;
			}
		}
		public override string PresentationName { get { return "Classic Level Bar"; } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new ClassicLinearScaleLevelBarBackgroundControl();
		}
		protected internal override PresentationControl CreateForegroundPresentationControl() {
			return new ClassicLinearScaleLevelBarForegroundControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ClassicLinearScaleLevelBarPresentation();
		}
	}
	public class IStyleLinearScaleLevelBarPresentation : PredefinedLinearScaleLevelBarPresentation {
		protected override Brush DefaultFill {
			get {
				LinearGradientBrush brush = new LinearGradientBrush() { StartPoint = new Point(0.5, 0), EndPoint = new Point(0.5, 1) };
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0x2F, 0xA7, 0xC0), Offset = 0 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0x0A, 0x7A, 0xAF), Offset = 1 });
				return brush;
			}
		}
		public override string PresentationName { get { return "IStyle Level Bar"; } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new IStyleLinearScaleLevelBarBackgroundControl();
		}
		protected internal override PresentationControl CreateForegroundPresentationControl() {
			return new IStyleLinearScaleLevelBarForegroundControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new IStyleLinearScaleLevelBarPresentation();
		}
	}
	public class YellowSubmarineLinearScaleLevelBarPresentation : PredefinedLinearScaleLevelBarPresentation {
		protected override Brush DefaultFill {
			get {
				LinearGradientBrush brush = new LinearGradientBrush() { StartPoint = new Point(0.5, 0), EndPoint = new Point(0.5, 1) };
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0x39, 0x3D, 0x4B), Offset = 0 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0x26, 0x2F, 0x3F), Offset = 1 });
				return brush;
			}
		}
		public override string PresentationName { get { return "Yellow Submarine Level Bar"; } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new YellowSubmarineLinearScaleLevelBarBackgroundControl();
		}
		protected internal override PresentationControl CreateForegroundPresentationControl() {
			return new YellowSubmarineLinearScaleLevelBarForegroundControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new YellowSubmarineLinearScaleLevelBarPresentation();
		}
	}
	public class MagicLightLinearScaleLevelBarPresentation : PredefinedLinearScaleLevelBarPresentation {
		protected override Brush DefaultFill {
			get {
				LinearGradientBrush brush = new LinearGradientBrush() { StartPoint = new Point(0.5, 0), EndPoint = new Point(0.5, 1) };
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0x28, 0x42, 0x47), Offset = 0 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0x10, 0x1B, 0x21), Offset = 1 });
				return brush;
			}
		}
		public override string PresentationName { get { return "Magic Light Level Bar"; } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new MagicLightLinearScaleLevelBarBackgroundControl();
		}
		protected internal override PresentationControl CreateForegroundPresentationControl() {
			return new MagicLightLinearScaleLevelBarForegroundControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new MagicLightLinearScaleLevelBarPresentation();
		}
	}
	public class RedThermometerLinearScaleLevelBarPresentation : PredefinedLinearScaleLevelBarPresentation {
		protected override Brush DefaultFill {
			get {
				LinearGradientBrush brush = new LinearGradientBrush() { StartPoint = new Point(0.5, 0), EndPoint = new Point(0.5, 1) };
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0xBA, 0x15, 0x14), Offset = 0 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0xA9, 0x0A, 0x0A), Offset = 1 });
				return brush;
			}
		}
		public override string PresentationName { get { return "Red Thermometer Level Bar"; } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new RedThermometerLinearScaleLevelBarBackgroundControl();
		}
		protected internal override PresentationControl CreateForegroundPresentationControl() {
			return new RedThermometerLinearScaleLevelBarForegroundControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new RedThermometerLinearScaleLevelBarPresentation();
		}
	}
	public class FlatLightLinearScaleLevelBarPresentation : PredefinedLinearScaleLevelBarPresentation {
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 231, 49, 65)); } }
		public override string PresentationName { get { return "Flat Light Level Bar"; } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new FlatLightLinearScaleLevelBarBackgroundControl();
		}
		protected internal override PresentationControl CreateForegroundPresentationControl() {
			return new FlatLightLinearScaleLevelBarForegroundControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FlatLightLinearScaleLevelBarPresentation();
		}
	}
	public class FlatDarkLinearScaleLevelBarPresentation : PredefinedLinearScaleLevelBarPresentation {
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 231, 49, 65)); } }
		public override string PresentationName { get { return "Flat Dark Level Bar"; } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new FlatDarkLinearScaleLevelBarBackgroundControl();
		}
		protected internal override PresentationControl CreateForegroundPresentationControl() {
			return new FlatDarkLinearScaleLevelBarForegroundControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FlatDarkLinearScaleLevelBarPresentation();
		}
	}
	public class CustomLinearScaleLevelBarPresentation : LinearScaleLevelBarPresentation {
		public static readonly DependencyProperty LevelBarBackgroundTemplateProperty = DependencyPropertyManager.Register("LevelBarBackgroundTemplate",
			typeof(ControlTemplate), typeof(CustomLinearScaleLevelBarPresentation));
		public static readonly DependencyProperty LevelBarForegroundTemplateProperty = DependencyPropertyManager.Register("LevelBarForegroundTemplate",
			typeof(ControlTemplate), typeof(CustomLinearScaleLevelBarPresentation));
		[Category(Categories.Common)]
		public ControlTemplate LevelBarBackgroundTemplate {
			get { return (ControlTemplate)GetValue(LevelBarBackgroundTemplateProperty); }
			set { SetValue(LevelBarBackgroundTemplateProperty, value); }
		}
		[Category(Categories.Common)]
		public ControlTemplate LevelBarForegroundTemplate {
			get { return (ControlTemplate)GetValue(LevelBarForegroundTemplateProperty); }
			set { SetValue(LevelBarForegroundTemplateProperty, value); }
		}
		public override string PresentationName { get { return "Custom Level Bar"; } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			CustomValueIndicatorPresentationControl presentationControl = new CustomValueIndicatorPresentationControl();
			presentationControl.SetBinding(CustomValueIndicatorPresentationControl.TemplateProperty, new Binding("LevelBarBackgroundTemplate") { Source = this });
			return presentationControl;
		}
		protected internal override PresentationControl CreateForegroundPresentationControl() {
			CustomPresentationControl presentationControl = new CustomPresentationControl();
			presentationControl.SetBinding(CustomPresentationControl.TemplateProperty, new Binding("LevelBarForegroundTemplate") { Source = this });
			return presentationControl;
		}
		protected override GaugeDependencyObject CreateObject() {
			return new CustomLinearScaleLevelBarPresentation();
		}
	}
}
