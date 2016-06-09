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
using System.Windows;
using System.ComponentModel;
using DevExpress.Xpf.Charts.Localization;
using DevExpress.Charts.Native;
namespace DevExpress.Xpf.Charts {
	public enum ControlCoordinatesVisibility {
		Visible,
		Hidden,
		Undefined
	}
	public class ControlCoordinates {
		readonly Pane pane;
		readonly AxisX2D axisX;
		readonly AxisY2D axisY;
		readonly ControlCoordinatesVisibility visibility;
		readonly Point point;
#if !SL
	[DevExpressXpfChartsLocalizedDescription("ControlCoordinatesPane")]
#endif
		public Pane Pane { get { return pane; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("ControlCoordinatesAxisX")]
#endif
		public AxisX2D AxisX { get { return axisX; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("ControlCoordinatesAxisY")]
#endif
		public AxisY2D AxisY { get { return axisY; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("ControlCoordinatesVisibility")]
#endif
		public ControlCoordinatesVisibility Visibility { get { return visibility; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("ControlCoordinatesPoint")]
#endif
		public Point Point { get { return point; } }
		internal ControlCoordinates(Pane pane, AxisX2D axisX, AxisY2D axisY, ControlCoordinatesVisibility visibility, Point point)
			: this(pane, visibility) {
			this.point = point;
			this.axisX = axisX;
			this.axisY = axisY;
		}
		internal ControlCoordinates(Pane pane, ControlCoordinatesVisibility visibility) {
			this.pane = pane;
			this.visibility = visibility;
		}
		internal ControlCoordinates(Pane pane)
			: this(pane, ControlCoordinatesVisibility.Undefined) {
		}
	}
}
namespace DevExpress.Xpf.Charts.Native {
	public static class DiagramToPointUtils {
		static object CheckValue(object value, ActualScaleType scaleType, ActualScaleType incorrectScaleType) {
			if (scaleType == incorrectScaleType)
				throw new ArgumentException(String.Format(ChartLocalizer.GetString(ChartStringId.MsgDiagramToPointIncorrectValue), value.GetType().ToString(), scaleType.ToString()));
			if (scaleType == ActualScaleType.Qualitative)
				return value.ToString();
			return value;
		}
		public static object CheckValue(string value, ActualScaleType scaleType) {
			object convertedArgument = value;
			try {
				if (scaleType == ActualScaleType.DateTime)
					convertedArgument = Convert.ToDateTime(value);
				else if (scaleType == ActualScaleType.Numerical)
					convertedArgument = Convert.ToDouble(value);
			}
			catch (Exception e) {
				throw new ArgumentException(String.Format(ChartLocalizer.GetString(ChartStringId.MsgDiagramToPointIncorrectValue), value.GetType().ToString(), scaleType.ToString()), e);
			}
			return convertedArgument;
		}
		public static object CheckValue(double value, ActualScaleType scaleType) {
			return CheckValue(value, scaleType, ActualScaleType.DateTime);
		}
		public static object CheckValue(DateTime value, ActualScaleType scaleType) {
			return CheckValue(value, scaleType, ActualScaleType.Numerical);
		}
		public static object CheckValue(object value, ActualScaleType scaleType) {
			if (value is string)
				return CheckValue((string)value, scaleType);
			else if (value is DateTime)
				return CheckValue((DateTime)value, scaleType);
			return CheckValue((double)value, scaleType);
		}
	}
}
