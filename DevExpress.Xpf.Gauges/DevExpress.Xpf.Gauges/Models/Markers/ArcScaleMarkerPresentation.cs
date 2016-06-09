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
	public abstract class ArcScaleMarkerPresentation : ValueIndicatorPresentation {		
	}
	public abstract class PredefinedArcScaleMarkerPresentation : ArcScaleMarkerPresentation {
		public static readonly DependencyProperty FillProperty = DependencyPropertyManager.Register("Fill",
		   typeof(Brush), typeof(PredefinedArcScaleMarkerPresentation), new PropertyMetadata(null, FillPropertyChanged));
		[Category(Categories.Presentation)]
		public Brush Fill {
			get { return (Brush)GetValue(FillProperty); }
			set { SetValue(FillProperty, value); }
		}
		static void FillPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PredefinedArcScaleMarkerPresentation presentation = d as PredefinedArcScaleMarkerPresentation;
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
	public class DefaultArcScaleMarkerPresentation : PredefinedArcScaleMarkerPresentation {
		public override string PresentationName { get { return "Default Marker"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 85, 85, 85)); } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new DefaultArcScaleMarkerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new DefaultArcScaleMarkerPresentation();
		}
	}
	public class CleanWhiteArcScaleMarkerPresentation : PredefinedArcScaleMarkerPresentation {
		public override string PresentationName { get { return "CLean White Marker"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 131, 151, 209)); } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new CleanWhiteArcScaleMarkerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new CleanWhiteArcScaleMarkerPresentation();
		}
	}
	public class CosmicArcScaleMarkerPresentation : PredefinedArcScaleMarkerPresentation {
		public override string PresentationName { get { return "Cosmic Marker"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.White); } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new CosmicArcScaleMarkerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new CosmicArcScaleMarkerPresentation();
		}
	}
	public class SmartArcScaleMarkerPresentation : PredefinedArcScaleMarkerPresentation {
		public override string PresentationName { get { return "Smart Marker"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 188, 63, 51)); } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new SmartArcScaleMarkerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new SmartArcScaleMarkerPresentation();
		}
	}
	public class ProgressiveArcScaleMarkerPresentation : PredefinedArcScaleMarkerPresentation {
		public override string PresentationName { get { return "Progressive Marker"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 0, 40, 67)); } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new ProgressiveArcScaleMarkerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ProgressiveArcScaleMarkerPresentation();
		}
	}
	public class EcoArcScaleMarkerPresentation : PredefinedArcScaleMarkerPresentation {
		public override string PresentationName { get { return "Eco Marker"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 141, 183, 103)); } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new EcoArcScaleMarkerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new EcoArcScaleMarkerPresentation();
		}
	}
	public class FutureArcScaleMarkerPresentation : PredefinedArcScaleMarkerPresentation {
		public override string PresentationName { get { return "Future Marker"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 184, 118, 196)); } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new FutureArcScaleMarkerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FutureArcScaleMarkerPresentation();
		}
	}
	public class ClassicArcScaleMarkerPresentation : PredefinedArcScaleMarkerPresentation {
		public override string PresentationName { get { return "Classic Marker"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 161, 186, 255)); } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new ClassicArcScaleMarkerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ClassicArcScaleMarkerPresentation();
		}
	}
	public class IStyleArcScaleMarkerPresentation : PredefinedArcScaleMarkerPresentation {
		public override string PresentationName { get { return "IStyle Marker"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 00, 120, 167)); } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new IStyleArcScaleMarkerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new IStyleArcScaleMarkerPresentation();
		}
	}
	public class YellowSubmarineArcScaleMarkerPresentation : PredefinedArcScaleMarkerPresentation {
		public override string PresentationName { get { return "Yellow Submarine Marker"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 63, 170, 212)); } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new YellowSubmarineArcScaleMarkerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new YellowSubmarineArcScaleMarkerPresentation();
		}
	}
	public class MagicLightArcScaleMarkerPresentation : PredefinedArcScaleMarkerPresentation {
		public override string PresentationName { get { return "Magic Light Marker"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 88, 193, 212)); } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new MagicLightArcScaleMarkerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new MagicLightArcScaleMarkerPresentation();
		}
	}
	public class FlatLightArcScaleMarkerPresentation : PredefinedArcScaleMarkerPresentation {
		public override string PresentationName { get { return "Flat Light Marker"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 153, 153, 153)); } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new FlatLightArcScaleMarkerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FlatLightArcScaleMarkerPresentation();
		}
	}
	public class FlatDarkArcScaleMarkerPresentation : PredefinedArcScaleMarkerPresentation {
		public override string PresentationName { get { return "Flat Dark Marker"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 110, 110, 110)); } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new FlatDarkArcScaleMarkerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FlatDarkArcScaleMarkerPresentation();
		}
	}
	public class CustomArcScaleMarkerPresentation : ArcScaleMarkerPresentation {
		public static readonly DependencyProperty MarkerTemplateProperty = DependencyPropertyManager.Register("MarkerTemplate",
			typeof(ControlTemplate), typeof(CustomArcScaleMarkerPresentation));
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
			return new CustomArcScaleMarkerPresentation();
		}
	}
}
