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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Editors.Helpers {
	public static class BrushHelper {
		public static SolidColorBrush ToSolidColorBrush(this object value) {
			if (value is Color)
				return new SolidColorBrush((Color)value);
			if (value is SolidColorBrush)
				return (SolidColorBrush)value;
			if (value is GradientBrush) {
				Color gc = ((GradientBrush)value).GradientStops.If(x => x.Count > 0).Return(x => x.First().Color, () => Text2ColorHelper.DefaultColor);
				return new SolidColorBrush(gc);
			}
			return new SolidColorBrush(Text2ColorHelper.DefaultColor);
		}
		public static LinearGradientBrush ToLinearGradientBrush(this object value) {
			if (value is LinearGradientBrush)
				return (LinearGradientBrush)value;
			Color startColor = Colors.White;
			if (value is Color)
				startColor = (Color)value;
			else if (value is SolidColorBrush)
				startColor = ((SolidColorBrush)value).Color;
			GradientStopCollection gc = new GradientStopCollection();
			var rgb = value as RadialGradientBrush;
			if (rgb != null)
				gc = rgb.GradientStops ?? new GradientStopCollection();
			if (gc.Count == 0) {
				gc.Add(new GradientStop(startColor, 0));
				gc.Add(new GradientStop(Colors.Black, 1));
			}
			else if (gc.Count == 1) {
				gc.Add(new GradientStop(Colors.Black, 1));
			}
			var lgb = new LinearGradientBrush(gc);
			if (rgb != null) {
				lgb.ColorInterpolationMode = rgb.ColorInterpolationMode;
				lgb.MappingMode = rgb.MappingMode;
				lgb.SpreadMethod = rgb.SpreadMethod;
			}
			return lgb;
		}
		public static RadialGradientBrush ToRadialGradientBrush(this object value) {
			if (value is RadialGradientBrush)
				return (RadialGradientBrush)value;
			Color startColor = Colors.White;
			if (value is Color)
				startColor = (Color)value;
			else if (value is SolidColorBrush)
				startColor = ((SolidColorBrush)value).Color;
			GradientStopCollection gc = new GradientStopCollection();
			var lgb = value as LinearGradientBrush;
			if (lgb != null) 
				gc = lgb.GradientStops ?? new GradientStopCollection();
			if (gc.Count == 0) {
				gc.Add(new GradientStop(startColor, 0));
				gc.Add(new GradientStop(Colors.Black, 1));
			}
			else if (gc.Count == 1) {
				gc.Add(new GradientStop(Colors.Black, 1));
			}
			var rgb = new RadialGradientBrush(gc);
			if (lgb != null) {
				rgb.ColorInterpolationMode = lgb.ColorInterpolationMode;
				rgb.MappingMode = lgb.MappingMode;
				rgb.SpreadMethod = lgb.SpreadMethod;
			}
			return rgb;
		}
	}
}
