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
	public abstract class ArcScaleNeedlePresentation : ValueIndicatorPresentation {
	}
	public abstract class PredefinedArcScaleNeedlePresentation : ArcScaleNeedlePresentation {
		public static readonly DependencyProperty FillProperty = DependencyPropertyManager.Register("Fill",
			typeof(Brush), typeof(PredefinedArcScaleNeedlePresentation), new PropertyMetadata(null, FillPropertyChanged));
		[Category(Categories.Presentation)]
		public Brush Fill {
			get { return (Brush)GetValue(FillProperty); }
			set { SetValue(FillProperty, value); }
		}
		static void FillPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PredefinedArcScaleNeedlePresentation presentation = d as PredefinedArcScaleNeedlePresentation;
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
	public class DefaultArcScaleNeedlePresentation : PredefinedArcScaleNeedlePresentation {
		public override string PresentationName { get { return "Default Needle"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 85, 85, 85)); } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new DefaultArcScaleNeedleControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new DefaultArcScaleNeedlePresentation();
		}
	}
	public class CleanWhiteArcScaleNeedlePresentation : PredefinedArcScaleNeedlePresentation {
		public override string PresentationName { get { return "Clean White Needle"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 91, 98, 112)); } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new CleanWhiteArcScaleNeedleControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new CleanWhiteArcScaleNeedlePresentation();
		}
	}
	public class CosmicArcScaleNeedlePresentation : PredefinedArcScaleNeedlePresentation {
		public override string PresentationName { get { return "Cosmic Needle"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.White); } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new CosmicArcScaleNeedleControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new CosmicArcScaleNeedlePresentation();
		}
	}
	public class SmartArcScaleNeedlePresentation : PredefinedArcScaleNeedlePresentation {
		public override string PresentationName { get { return "SMart Needle"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 188, 63, 51)); } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new SmartArcScaleNeedleControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new SmartArcScaleNeedlePresentation();
		}
	}
	public class RedClockSecondArcScaleNeedlePresentation : PredefinedArcScaleNeedlePresentation {
		public override string PresentationName { get { return "Red Clock Second Needle"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 119, 1, 20)); } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new RedClockSecondArcScaleNeedleControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new RedClockSecondArcScaleNeedlePresentation();
		}
	}
	public class ProgressiveArcScaleNeedlePresentation : PredefinedArcScaleNeedlePresentation {
		public override string PresentationName { get { return "Progressive Needle"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 101, 210, 255)); } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new ProgressiveArcScaleNeedleControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ProgressiveArcScaleNeedlePresentation();
		}
	}
	public class EcoArcScaleNeedlePresentation : PredefinedArcScaleNeedlePresentation {
		public override string PresentationName { get { return "Eco Needle"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 141, 183, 57)); } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new EcoArcScaleNeedleControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new EcoArcScaleNeedlePresentation();
		}
	}
	public class FutureArcScaleNeedlePresentation : PredefinedArcScaleNeedlePresentation {
		public override string PresentationName { get { return "Future Needle"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 109, 148, 202)); } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new FutureArcScaleNeedleControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FutureArcScaleNeedlePresentation();
		}
	}
	public class ClassicArcScaleNeedlePresentation : PredefinedArcScaleNeedlePresentation {
		public override string PresentationName { get { return "Classic Needle"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 236, 239, 245)); } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new ClassicArcScaleNeedleControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ClassicArcScaleNeedlePresentation();
		}
	}
	public class IStyleArcScaleNeedlePresentation : PredefinedArcScaleNeedlePresentation {
		public override string PresentationName { get { return "IStyle Needle"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 35, 29, 37)); } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new IStyleArcScaleNeedleControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new IStyleArcScaleNeedlePresentation();
		}
	}
	public class YellowSubmarineArcScaleNeedlePresentation : PredefinedArcScaleNeedlePresentation {
		public override string PresentationName { get { return "Yellow Submarine Needle"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 82, 89, 103)); } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new YellowSubmarineArcScaleNeedleControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new YellowSubmarineArcScaleNeedlePresentation();
		}
	}
	public class MagicLightArcScaleNeedlePresentation : PredefinedArcScaleNeedlePresentation {
		public override string PresentationName { get { return "Magic Light Needle"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 88, 193, 212)); } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new MagicLightArcScaleNeedleControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new MagicLightArcScaleNeedlePresentation();
		}
	}
	public class FlatLightArcScaleNeedlePresentation : PredefinedArcScaleNeedlePresentation {
		public override string PresentationName { get { return "Flat Light Needle"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(85, 0, 0, 0)); } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new FlatLightArcScaleNeedleControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FlatLightArcScaleNeedlePresentation();
		}
	}
	public class FlatDarkArcScaleNeedlePresentation : PredefinedArcScaleNeedlePresentation {
		public override string PresentationName { get { return "Flat Dark Needle"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Color.FromArgb(255, 110, 110, 110)); } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			return new FlatDarkArcScaleNeedleControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FlatDarkArcScaleNeedlePresentation();
		}
	}
	public class CustomArcScaleNeedlePresentation : ArcScaleNeedlePresentation {
		public static readonly DependencyProperty NeedleTemplateProperty = DependencyPropertyManager.Register("NeedleTemplate",
			typeof(ControlTemplate), typeof(CustomArcScaleNeedlePresentation));
		[Category(Categories.Common)]
		public ControlTemplate NeedleTemplate {
			get { return (ControlTemplate)GetValue(NeedleTemplateProperty); }
			set { SetValue(NeedleTemplateProperty, value); }
		}
		public override string PresentationName { get { return "Custom Needle"; } }
		protected internal override PresentationControl CreateIndicatorPresentationControl() {
			CustomValueIndicatorPresentationControl modelControl = new CustomValueIndicatorPresentationControl();
			modelControl.SetBinding(CustomValueIndicatorPresentationControl.TemplateProperty, new Binding("NeedleTemplate") { Source = this });
			return modelControl;
		}
		protected override GaugeDependencyObject CreateObject() {
			return new CustomArcScaleNeedlePresentation();
		}
	}
}
