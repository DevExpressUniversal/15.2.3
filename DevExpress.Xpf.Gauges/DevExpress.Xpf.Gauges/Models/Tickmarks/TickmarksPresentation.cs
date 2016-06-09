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
	public abstract class TickmarksPresentation : PresentationBase {
		protected internal abstract PresentationControl CreateMinorTickPresentationControl();
		protected internal abstract PresentationControl CreateMajorTickPresentationControl();
	}
	public abstract class PredefinedTickmarksPresentation : TickmarksPresentation {
		public static readonly DependencyProperty MajorTickBrushProperty = DependencyPropertyManager.Register("MajorTickBrush",
		   typeof(Brush), typeof(PredefinedTickmarksPresentation), new PropertyMetadata(null, MajorTickBrushPropertyChanged));
		public static readonly DependencyProperty MinorTickBrushProperty = DependencyPropertyManager.Register("MinorTickBrush",
		   typeof(Brush), typeof(PredefinedTickmarksPresentation), new PropertyMetadata(null, MinorTickBrushPropertyChanged));
		[Category(Categories.Presentation)]
		public Brush MajorTickBrush {
			get { return (Brush)GetValue(MajorTickBrushProperty); }
			set { SetValue(MajorTickBrushProperty, value); }
		}
		[Category(Categories.Presentation)]
		public Brush MinorTickBrush {
			get { return (Brush)GetValue(MinorTickBrushProperty); }
			set { SetValue(MinorTickBrushProperty, value); }
		}
		static void MajorTickBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PredefinedTickmarksPresentation presentation = d as PredefinedTickmarksPresentation;
			if (presentation != null)
				presentation.ActualMajorTickBrushChanged();
		}
		static void MinorTickBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PredefinedTickmarksPresentation presentation = d as PredefinedTickmarksPresentation;
			if (presentation != null)
				presentation.ActualMinorTickBrushChanged();
		}
		protected abstract Brush DefaultMajorTickBrush { get; }
		protected abstract Brush DefaultMinorTickBrush { get; }
		[Category(Categories.Presentation)]
		public Brush ActualMajorTickBrush { get { return MajorTickBrush != null ? MajorTickBrush : DefaultMajorTickBrush; } }
		[Category(Categories.Presentation)]
		public Brush ActualMinorTickBrush { get { return MinorTickBrush != null ? MinorTickBrush : DefaultMinorTickBrush; } }
		void ActualMinorTickBrushChanged() {
			NotifyPropertyChanged("ActualMinorTickBrush");
		}
		void ActualMajorTickBrushChanged() {
			NotifyPropertyChanged("ActualMajorTickBrush");
		}
	}
	public class DefaultTickmarksPresentation : PredefinedTickmarksPresentation {
		public override string PresentationName { get { return "Default Tickmarks"; } }
		protected override Brush DefaultMajorTickBrush { get { return new SolidColorBrush(Color.FromArgb(255, 67, 67, 67)); } }
		protected override Brush DefaultMinorTickBrush { get { return new SolidColorBrush(Color.FromArgb(255, 67, 67, 67)); } }
		protected internal override PresentationControl CreateMinorTickPresentationControl() {
			return new DefaultMinorTickmarkControl();
		}
		protected internal override PresentationControl CreateMajorTickPresentationControl() {
			return new DefaultMajorTickmarkControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new DefaultTickmarksPresentation();
		}
	}
	public class CleanWhiteTickmarksPresentation : PredefinedTickmarksPresentation {
		public override string PresentationName { get { return "Clean White Tickmarks"; } }
		protected override Brush DefaultMajorTickBrush { get { return new SolidColorBrush(Color.FromArgb(255, 72, 78, 90)); } }
		protected override Brush DefaultMinorTickBrush { get { return new SolidColorBrush(Color.FromArgb(255, 72, 78, 90)); } }
		protected internal override PresentationControl CreateMinorTickPresentationControl() {
			return new CleanWhiteMinorTickmarkControl();
		}
		protected internal override PresentationControl CreateMajorTickPresentationControl() {
			return new CleanWhiteMajorTickmarkControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new CleanWhiteTickmarksPresentation();
		}
	}
	public class CosmicTickmarksPresentation : PredefinedTickmarksPresentation {
		public override string PresentationName { get { return "Cosmic Tickmarks"; } }
		protected override Brush DefaultMajorTickBrush { get { return new SolidColorBrush(Color.FromArgb(255, 3, 23, 29)); } }
		protected override Brush DefaultMinorTickBrush { get { return new SolidColorBrush(Color.FromArgb(255, 3, 23, 29)); } }
		protected internal override PresentationControl CreateMinorTickPresentationControl() {
			return new CosmicMinorTickmarkControl();
		}
		protected internal override PresentationControl CreateMajorTickPresentationControl() {
			return new CosmicMajorTickmarkControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new CosmicTickmarksPresentation();
		}
	}
	public class SmartTickmarksPresentation : PredefinedTickmarksPresentation {
		public override string PresentationName { get { return "Smart Tickmarks"; } }
		protected override Brush DefaultMajorTickBrush { get { return new SolidColorBrush(Color.FromArgb(255, 89, 97, 111)); } }
		protected override Brush DefaultMinorTickBrush { get { return new SolidColorBrush(Color.FromArgb(255, 89, 97, 111)); } }
		protected internal override PresentationControl CreateMinorTickPresentationControl() {
			return new SmartMinorTickmarkControl();
		}
		protected internal override PresentationControl CreateMajorTickPresentationControl() {
			return new SmartMajorTickmarkControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new SmartTickmarksPresentation();
		}
	}
	public class ProgressiveTickmarksPresentation : PredefinedTickmarksPresentation {
		public override string PresentationName { get { return "Progressive Tickmarks"; } }
		protected override Brush DefaultMajorTickBrush { get { return new SolidColorBrush(Color.FromArgb(255, 1, 8, 14)); } }
		protected override Brush DefaultMinorTickBrush { get { return new SolidColorBrush(Color.FromArgb(255, 1, 8, 14)); } }
		protected internal override PresentationControl CreateMinorTickPresentationControl() {
			return new ProgressiveMinorTickmarkControl();
		}
		protected internal override PresentationControl CreateMajorTickPresentationControl() {
			return new ProgressiveMajorTickmarkControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ProgressiveTickmarksPresentation();
		}
	}
	public class EcoTickmarksPresentation : PredefinedTickmarksPresentation {
		public override string PresentationName { get { return "Eco Tickmarks"; } }
		protected override Brush DefaultMajorTickBrush { get { return new SolidColorBrush(Color.FromArgb(255, 58, 56, 50)); } }
		protected override Brush DefaultMinorTickBrush { get { return new SolidColorBrush(Color.FromArgb(255, 58, 56, 50)); } }
		protected internal override PresentationControl CreateMinorTickPresentationControl() {
			return new EcoMinorTickmarkControl();
		}
		protected internal override PresentationControl CreateMajorTickPresentationControl() {
			return new EcoMajorTickmarkControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new EcoTickmarksPresentation();
		}
	}
	public class FutureTickmarksPresentation : PredefinedTickmarksPresentation {
		public override string PresentationName { get { return "Future Tickmarks"; } }
		protected override Brush DefaultMajorTickBrush { get { return new SolidColorBrush(Color.FromArgb(255, 173, 166, 200)); } }
		protected override Brush DefaultMinorTickBrush { get { return new SolidColorBrush(Color.FromArgb(255, 173, 166, 200)); } }
		protected internal override PresentationControl CreateMinorTickPresentationControl() {
			return new FutureMinorTickmarkControl();
		}
		protected internal override PresentationControl CreateMajorTickPresentationControl() {
			return new FutureMajorTickmarkControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FutureTickmarksPresentation();
		}
	}
	public class ClassicTickmarksPresentation : PredefinedTickmarksPresentation {
		public override string PresentationName { get { return "Classic Tickmarks"; } }
		protected override Brush DefaultMajorTickBrush { get { return new SolidColorBrush(Colors.White); } }
		protected override Brush DefaultMinorTickBrush { get { return new SolidColorBrush(Colors.White); } }
		protected internal override PresentationControl CreateMinorTickPresentationControl() {
			return new ClassicMinorTickmarkControl();
		}
		protected internal override PresentationControl CreateMajorTickPresentationControl() {
			return new ClassicMajorTickmarkControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ClassicTickmarksPresentation();
		}
	}
	public class IStyleTickmarksPresentation : PredefinedTickmarksPresentation {
		public override string PresentationName { get { return "IStyle Tickmarks"; } }
		protected override Brush DefaultMajorTickBrush { get { return new SolidColorBrush(Color.FromArgb(255, 89, 97, 111)); } }
		protected override Brush DefaultMinorTickBrush { get { return new SolidColorBrush(Color.FromArgb(255, 89, 97, 111)); } }
		protected internal override PresentationControl CreateMinorTickPresentationControl() {
			return new IStyleMinorTickmarkControl();
		}
		protected internal override PresentationControl CreateMajorTickPresentationControl() {
			return new IStyleMajorTickmarkControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new IStyleTickmarksPresentation();
		}
	}
	public class YellowSubmarineTickmarksPresentation : PredefinedTickmarksPresentation {
		public override string PresentationName { get { return "Yellow Submarine Tickmarks"; } }
		protected override Brush DefaultMajorTickBrush { get { return new SolidColorBrush(Color.FromArgb(255, 53, 59, 74)); } }
		protected override Brush DefaultMinorTickBrush { get { return new SolidColorBrush(Color.FromArgb(255, 53, 59, 74)); } }
		protected internal override PresentationControl CreateMinorTickPresentationControl() {
			return new YellowSubmarineMinorTickmarkControl();
		}
		protected internal override PresentationControl CreateMajorTickPresentationControl() {
			return new YellowSubmarineMajorTickmarkControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new YellowSubmarineTickmarksPresentation();
		}
	}
	public class MagicLightTickmarksPresentation : PredefinedTickmarksPresentation {
		public override string PresentationName { get { return "Magic Light Tickmarks"; } }
		protected override Brush DefaultMajorTickBrush {
			get {
				RadialGradientBrush brush = new RadialGradientBrush();
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0x60, 0xD2, 0xE2) });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0x00, 0x60, 0xD2, 0xE2), Offset = 1 });
				return brush;
			}
		}
		protected override Brush DefaultMinorTickBrush {
			get {
				RadialGradientBrush brush = new RadialGradientBrush();
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0x60, 0xD2, 0xE2) });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0x00, 0x60, 0xD2, 0xE2), Offset = 1 });
				return brush;
			}
		}
		protected internal override PresentationControl CreateMinorTickPresentationControl() {
			return new MagicLightMinorTickmarkControl();
		}
		protected internal override PresentationControl CreateMajorTickPresentationControl() {
			return new MagicLightMajorTickmarkControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new MagicLightTickmarksPresentation();
		}
	}
	public class FlatLightTickmarksPresentation : PredefinedTickmarksPresentation {
		public override string PresentationName { get { return "Flat Light Tickmarks"; } }
		protected override Brush DefaultMajorTickBrush { get { return new SolidColorBrush(Color.FromArgb(255, 153, 153, 153)); } }
		protected override Brush DefaultMinorTickBrush { get { return new SolidColorBrush(Color.FromArgb(255, 153, 153, 153)); } }
		protected internal override PresentationControl CreateMinorTickPresentationControl() {
			return new FlatLightMinorTickmarkControl();
		}
		protected internal override PresentationControl CreateMajorTickPresentationControl() {
			return new FlatLightMajorTickmarkControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FlatLightTickmarksPresentation();
		}
	}
	public class FlatDarkTickmarksPresentation : PredefinedTickmarksPresentation {
		public override string PresentationName { get { return "Flat Dark Tickmarks"; } }
		protected override Brush DefaultMajorTickBrush { get { return new SolidColorBrush(Color.FromArgb(255, 77, 77, 77)); } }
		protected override Brush DefaultMinorTickBrush { get { return new SolidColorBrush(Color.FromArgb(255, 77, 77, 77)); } }
		protected internal override PresentationControl CreateMinorTickPresentationControl() {
			return new FlatDarkMinorTickmarkControl();
		}
		protected internal override PresentationControl CreateMajorTickPresentationControl() {
			return new FlatDarkMajorTickmarkControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FlatDarkTickmarksPresentation();
		}
	}
	public class CustomTickmarksPresentation : TickmarksPresentation {
		public static readonly DependencyProperty MinorTickmarkTemplateProperty = DependencyPropertyManager.Register("MinorTickmarkTemplate",
			typeof(ControlTemplate), typeof(CustomTickmarksPresentation));
		public static readonly DependencyProperty MajorTickmarkTemplateProperty = DependencyPropertyManager.Register("MajorTickmarkTemplate",
			typeof(ControlTemplate), typeof(CustomTickmarksPresentation));
		[Category(Categories.Common)]
		public ControlTemplate MinorTickmarkTemplate {
			get { return (ControlTemplate)GetValue(MinorTickmarkTemplateProperty); }
			set { SetValue(MinorTickmarkTemplateProperty, value); }
		}
		[Category(Categories.Common)]
		public ControlTemplate MajorTickmarkTemplate {
			get { return (ControlTemplate)GetValue(MajorTickmarkTemplateProperty); }
			set { SetValue(MajorTickmarkTemplateProperty, value); }
		}
		public override string PresentationName { get { return "Custom Tickmarks"; } }
		protected internal override PresentationControl CreateMajorTickPresentationControl() {
			CustomPresentationControl modelControl = new CustomPresentationControl();
			modelControl.SetBinding(CustomPresentationControl.TemplateProperty, new Binding("MajorTickmarkTemplate") { Source = this });
			return modelControl;
		}
		protected internal override PresentationControl CreateMinorTickPresentationControl() {
			CustomPresentationControl modelControl = new CustomPresentationControl();
			modelControl.SetBinding(CustomPresentationControl.TemplateProperty, new Binding("MinorTickmarkTemplate") { Source = this });
			return modelControl;
		}
		protected override GaugeDependencyObject CreateObject() {
			return new CustomTickmarksPresentation();
		}
	}
}
