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
	public abstract class LinearScaleMarkerPresentation : ValueIndicatorPresentation {
	}
	public abstract class PredefinedLinearScaleMarkerPresentation : LinearScaleMarkerPresentation {
		public static readonly DependencyProperty FillProperty = DependencyPropertyManager.Register("Fill",
		   typeof(Brush), typeof(PredefinedLinearScaleMarkerPresentation), new PropertyMetadata(null, FillPropertyChanged));
		[Category(Categories.Presentation)]
		public Brush Fill {
			get { return (Brush)GetValue(FillProperty); }
			set { SetValue(FillProperty, value); }
		}
		static void FillPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PredefinedLinearScaleMarkerPresentation presentation = d as PredefinedLinearScaleMarkerPresentation;
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
	public class DefaultLinearScaleMarkerPresentation : PredefinedLinearScaleMarkerPresentation {
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(0xFF, 0x55, 0x55, 0x55)); } }
		public override string PresentationName { get { return "Default Marker"; } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new DefaultLinearScaleMarkerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new DefaultLinearScaleMarkerPresentation();
		}
	}
	public class CleanWhiteLinearScaleMarkerPresentation : PredefinedLinearScaleMarkerPresentation {
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(0xFF, 0x83, 0x97, 0xD1)); } }
		public override string PresentationName { get { return "Clean White Marker"; } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new CleanWhiteLinearScaleMarkerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new CleanWhiteLinearScaleMarkerPresentation();
		}
	}
	public class CosmicLinearScaleMarkerPresentation : PredefinedLinearScaleMarkerPresentation {
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.White); } }
		public override string PresentationName { get { return "Cosmic Marker"; } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new CosmicLinearScaleMarkerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new CosmicLinearScaleMarkerPresentation();
		}
	}
	public class SmartLinearScaleMarkerPresentation : PredefinedLinearScaleMarkerPresentation {
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(0xFF, 0xBC, 0x3F, 0x33)); } }
		public override string PresentationName { get { return "Smart Marker"; } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new SmartLinearScaleMarkerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new SmartLinearScaleMarkerPresentation();
		}
	}
	public class ProgressiveLinearScaleMarkerPresentation : PredefinedLinearScaleMarkerPresentation {
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0x28, 0x43)); } }
		public override string PresentationName { get { return "Progressive Marker"; } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new ProgressiveLinearScaleMarkerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ProgressiveLinearScaleMarkerPresentation();
		}
	}
	public class EcoLinearScaleMarkerPresentation : PredefinedLinearScaleMarkerPresentation {
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(0xFF, 0x8D, 0xB7, 0x39)); } }
		public override string PresentationName { get { return "Eco Marker"; } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new EcoLinearScaleMarkerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new EcoLinearScaleMarkerPresentation();
		}
	}
	public class FutureLinearScaleMarkerPresentation : PredefinedLinearScaleMarkerPresentation {
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(0xFF, 0xB8, 0x76, 0xC4)); } }
		public override string PresentationName { get { return "Future Marker"; } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new FutureLinearScaleMarkerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FutureLinearScaleMarkerPresentation();
		}
	}
	public class ClassicLinearScaleMarkerPresentation : PredefinedLinearScaleMarkerPresentation {
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(0xFF, 0xA1, 0xBA, 0xFF)); } }
		public override string PresentationName { get { return "Classic Marker"; } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new ClassicLinearScaleMarkerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ClassicLinearScaleMarkerPresentation();
		}
	}
	public class IStyleLinearScaleMarkerPresentation : PredefinedLinearScaleMarkerPresentation {
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0x78, 0xA7)); } }
		public override string PresentationName { get { return "IStyle Marker"; } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new IStyleLinearScaleMarkerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new IStyleLinearScaleMarkerPresentation();
		}
	}
	public class YellowSubmarineLinearScaleMarkerPresentation : PredefinedLinearScaleMarkerPresentation {
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(0xFF, 0x3F, 0xAA, 0xD4)); } }
		public override string PresentationName { get { return "Yellow Submarine Marker"; } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new YellowSubmarineLinearScaleMarkerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new YellowSubmarineLinearScaleMarkerPresentation();
		}
	}
	public class MagicLightLinearScaleMarkerPresentation : PredefinedLinearScaleMarkerPresentation {
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(0xFF, 0xC9, 0xCD, 0xD3)); } }
		public override string PresentationName { get { return "Magic Light Marker"; } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new MagicLightLinearScaleMarkerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new MagicLightLinearScaleMarkerPresentation();
		}
	}
	public class RedThermometerLinearScaleMarkerPresentation : PredefinedLinearScaleMarkerPresentation {
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(0xFF, 0x83, 0x97, 0xD1)); } }
		public override string PresentationName { get { return "Red Thermometer Marker"; } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new RedThermometerLinearScaleMarkerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new RedThermometerLinearScaleMarkerPresentation();
		}
	}
	public class FlatLightLinearScaleMarkerPresentation : PredefinedLinearScaleMarkerPresentation {
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 153, 153, 153)); } }
		public override string PresentationName { get { return "Flat Light Marker"; } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new FlatLightLinearScaleMarkerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FlatLightLinearScaleMarkerPresentation();
		}
	}
	public class FlatDarkLinearScaleMarkerPresentation : PredefinedLinearScaleMarkerPresentation {
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 110, 110, 110)); } }
		public override string PresentationName { get { return "Flat Dark Marker"; } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new FlatDarkLinearScaleMarkerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FlatDarkLinearScaleMarkerPresentation();
		}
	}
	public class CustomLinearScaleMarkerPresentation : LinearScaleMarkerPresentation {
		public static readonly DependencyProperty MarkerTemplateProperty = DependencyPropertyManager.Register("MarkerTemplate",
			typeof(ControlTemplate), typeof(CustomLinearScaleMarkerPresentation));
		[Category(Categories.Common)]
		public ControlTemplate MarkerTemplate {
			get { return (ControlTemplate)GetValue(MarkerTemplateProperty); }
			set { SetValue(MarkerTemplateProperty, value); }
		}
		public override string PresentationName { get { return "Custom Marker"; } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			CustomValueIndicatorPresentationControl modelControl = new CustomValueIndicatorPresentationControl();
			modelControl.SetBinding(CustomValueIndicatorPresentationControl.TemplateProperty, new Binding("MarkerTemplate") { Source = this });
			return modelControl;
		}
		protected override GaugeDependencyObject CreateObject() {
			return new CustomLinearScaleMarkerPresentation();
		}
	}
}
