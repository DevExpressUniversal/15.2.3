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
	public class ClassicLinearScaleForegroundLayerPresentation : PredefinedLinearScaleLayerPresentation {
		protected override Brush DefaultFill {
			get {
				RadialGradientBrush brush = new RadialGradientBrush() { Center = new Point(0.5, -0.186), GradientOrigin = new Point(0.5, -0.186), RadiusX = 1.281, RadiusY = 0.525 };
				brush.GradientStops.Add(new GradientStop() { Offset = 1 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0x60, 0xFF, 0xFF, 0xFF) });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0x0C, 0xFF, 0xFF, 0xFF), Offset = 0.9999 });
				return brush;
			}
		}
		public override string PresentationName { get { return "Classic Foreground"; } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new ClassicLinearScaleForegroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ClassicLinearScaleForegroundLayerPresentation();
		}
	}
	public class ProgressiveLinearScaleForegroundLayerPresentation : PredefinedLinearScaleLayerPresentation {
		protected override Brush DefaultFill {
			get {
				RadialGradientBrush brush = new RadialGradientBrush() { Center = new Point(0.501, 0.007), GradientOrigin = new Point(0.501, 0.007), RadiusX = 2.862, RadiusY = 1 };
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0x19, 0xC5, 0xD3, 0xFF), Offset = 0.35 });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0x7F, 0xC5, 0xD3, 0xFF) });
				brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0x00, 0xC5, 0xD3, 0xFF), Offset = 0.35001 });
				return brush;
			}
		}
		public override string PresentationName { get { return "Progressive Foreground"; } }
		protected internal override PresentationControl CreateLayerPresentationControl() {
			return new ProgressiveLinearScaleForegroundLayerControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ProgressiveLinearScaleForegroundLayerPresentation();
		}
	}
}
