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

using System.Windows;
using System.Windows.Media;
namespace DevExpress.Xpf.Gauges {
	public class ProgressiveArcScaleForegroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Progressive Foreground"; } }
		protected override Brush DefaultFill {
			get {
				LinearGradientBrush brush = new LinearGradientBrush() { StartPoint = new Point(0.5, 0), EndPoint = new Point(0.5, 1) };
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(127, 195, 197, 255), Offset = 0 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0, 195, 197, 255), Offset = 1 });
				return brush;
			}
		}
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new ProgressiveArcScaleForegroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ProgressiveArcScaleForegroundLayerPresentation();
		}
	}
	public class ProgressiveHalfTopArcScaleForegroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Progressive Half Top Foreground"; } }
		protected override Brush DefaultFill {
			get {
				LinearGradientBrush brush = new LinearGradientBrush() { StartPoint = new Point(0.5, 0), EndPoint = new Point(0.5, 1) };
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(127, 197, 211, 255), Offset = 0 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0, 197, 211, 255), Offset = 1 });
				return brush;
			}
		}
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new ProgressiveHalfTopArcScaleForegroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ProgressiveHalfTopArcScaleForegroundLayerPresentation();
		}
	}
	public class ProgressiveQuarterTopLeftArcScaleForegroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Progressive Quarter Top Left Foreground"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new ProgressiveQuarterTopLeftArcScaleForegroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ProgressiveQuarterTopLeftArcScaleForegroundLayerPresentation();
		}
	}
	public class ProgressiveQuarterTopRightArcScaleForegroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Progressive Quarter Top Right Foreground"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new ProgressiveQuarterTopRightArcScaleForegroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ProgressiveQuarterTopRightArcScaleForegroundLayerPresentation();
		}
	}
	public class ProgressiveThreeQuartersArcScaleForegroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Progressive Three Quarters Foreground"; } }
		protected override Brush DefaultFill {
			get {
				LinearGradientBrush brush = new LinearGradientBrush() { StartPoint = new Point(0.5, 0), EndPoint = new Point(0.5, 1) };
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(127, 195, 197, 255), Offset = 0 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0, 195, 197, 255), Offset = 1 });
				return brush;
			}
		}
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new ProgressiveThreeQuartersArcScaleForegroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ProgressiveThreeQuartersArcScaleForegroundLayerPresentation();
		}
	}
	public class FutureArcScaleForegroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Future Foreground"; } }
		protected override Brush DefaultFill {
			get {
				LinearGradientBrush brush = new LinearGradientBrush() { StartPoint = new Point(0.5, 0), EndPoint = new Point(0.5, 1) };
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(255, 195, 202, 255), Offset = 0 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0, 212, 190, 255), Offset = 1 });
				return brush;
			}
		}
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new FutureArcScaleForegroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FutureArcScaleForegroundLayerPresentation();
		}
	}
	public class FutureHalfTopArcScaleForegroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Future Half Top Foreground"; } }
		protected override Brush DefaultFill {
			get {
				LinearGradientBrush brush = new LinearGradientBrush() { StartPoint = new Point(0.5, 0), EndPoint = new Point(0.5, 1) };
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(84, 195, 202, 255), Offset = 0 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0, 212, 190, 255), Offset = 1 });
				return brush;
			}
		}
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new FutureHalfTopArcScaleForegroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FutureHalfTopArcScaleForegroundLayerPresentation();
		}
	}
	public class FutureQuarterTopLeftArcScaleForegroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Future Quarter Top Left Foreground"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new FutureQuarterTopLeftArcScaleForegroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FutureQuarterTopLeftArcScaleForegroundLayerPresentation();
		}
	}
	public class FutureQuarterTopRightArcScaleForegroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Future Quarter Top Right Foreground"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new FutureQuarterTopRightArcScaleForegroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FutureQuarterTopRightArcScaleForegroundLayerPresentation();
		}
	}
	public class FutureThreeQuartersArcScaleForegroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Future Three Quarters Foreground"; } }
		protected override Brush DefaultFill {
			get {
				LinearGradientBrush brush = new LinearGradientBrush() { StartPoint = new Point(0.5, 0), EndPoint = new Point(0.5, 1) };
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(84, 195, 202, 255), Offset = 0 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0, 212, 190, 255), Offset = 1 });
				return brush;
			}
		}
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new FutureThreeQuartersArcScaleForegroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FutureThreeQuartersArcScaleForegroundLayerPresentation();
		}
	}
	public class MagicLightArcScaleForegroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Magic Light Foreground"; } }
		protected override Brush DefaultFill {
			get {
				LinearGradientBrush brush = new LinearGradientBrush() { StartPoint = new Point(0.5, 0), EndPoint = new Point(0.5, 1) };
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0x94, 0xEE, 0xFF), Offset = 0 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0x00, 0x94, 0xEE, 0xFF), Offset = 1 });
				return brush;
			}
		}
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new MagicLightArcScaleForegroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new MagicLightArcScaleForegroundLayerPresentation();
		}
	}
	public class MagicLightHalfTopArcScaleForegroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Magic Light Half Top Foreground"; } }
		protected override Brush DefaultFill {
			get {
				LinearGradientBrush brush = new LinearGradientBrush() { StartPoint = new Point(0.5, 0), EndPoint = new Point(0.5, 1) };
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(77, 148, 238, 255), Offset = 0 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0, 148, 238, 255), Offset = 1 });
				return brush;
			}
		}
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new MagicLightHalfTopArcScaleForegroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new MagicLightHalfTopArcScaleForegroundLayerPresentation();
		}
	}
	public class MagicLightQuarterTopLeftArcScaleForegroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Magic Light Quarter Top Left Foreground"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new MagicLightQuarterTopLeftArcScaleForegroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new MagicLightQuarterTopLeftArcScaleForegroundLayerPresentation();
		}
	}
	public class MagicLightQuarterTopRightArcScaleForegroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Magic Light Quarter Top Right Foreground"; } }
		protected override Brush DefaultFill { get { return new SolidColorBrush(Colors.Transparent); } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new MagicLightQuarterTopRightArcScaleForegroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new MagicLightQuarterTopRightArcScaleForegroundLayerPresentation();
		}
	}
	public class MagicLightThreeQuartersArcScaleForegroundLayerPresentation : PredefinedArcScaleLayerPresentation {
		public override string PresentationName { get { return "Magic Light Three Quarters Foreground"; } }
		protected override Brush DefaultFill {
			get {
				LinearGradientBrush brush = new LinearGradientBrush() { StartPoint = new Point(0.5, 0), EndPoint = new Point(0.5, 1) };
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0x4D, 0x94, 0xEE, 0xFF), Offset = 0 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0x00, 0x94, 0xEE, 0xFF), Offset = 1 });
				return brush;
			}
		}
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new MagicLightThreeQuartersArcScaleForegroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new MagicLightThreeQuartersArcScaleForegroundLayerPresentation();
		}
	}
}
