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
using System.Windows.Forms;
using System.Windows.Media;
namespace DevExpress.Xpf.Editors.Internal {
	public class BarSparklinePainter : BaseSparklinePainter {
		BarSparklineControlBase BarView { get { return (BarSparklineControlBase)View; } }
		protected override bool EnableAntialiasing { get { return false; } }
		public override SparklineViewType SparklineType { get { return SparklineViewType.Bar; } }
		protected virtual double GetBarValue(double value) {
			return value;
		}
		protected override void DrawInternal(DrawingContext drawingContext) {
			double barWidth = Math.Max(1, Mapping.MinPointsDistancePx - BarView.ActualBarDistance - 1);
			int halfBarWidth = Convert.ToInt32(barWidth / 2);
			Point lastInvisible = new Point();
			int lastInvisibleIndex = -1;
			bool hasDrawingPoints = false;
			for (int i = 0; i < Data.Count; i++) {
				double value = Data[i].Value;
				if(!SparklineMathUtils.IsValidDouble(value))
					continue;
				value = GetBarValue(value);
				double x = Mapping.MapXValueToScreen(Data[i].Argument) - halfBarWidth;
				double y = Mapping.MapYValueToScreen(value);
				if (!Mapping.isPointVisible(Data[i].Argument) && !hasDrawingPoints) {
					lastInvisible = new Point(x, y);
					lastInvisibleIndex = i;
					continue;
				}
				if (lastInvisibleIndex >= 0) {
					DrawBar(drawingContext, lastInvisible.X, lastInvisible.Y, barWidth, lastInvisibleIndex);
					lastInvisibleIndex = -1;
				}
				hasDrawingPoints = true;
				DrawBar(drawingContext, x, y, barWidth, i);
				if (!Mapping.isPointVisible(Data[i].Argument))
					break;
			}
		}
		void DrawBar(DrawingContext drawingContext, double x, double y, double barWidth, int i) {
			double height = Math.Abs(y - Mapping.ScreenYZeroValue);
			if (height == 0)
				height = 1;
			PointPresentationType pointType = GetPointPresentationType(i);
			SolidColorBrush brush = GetPointBrush(pointType);
			drawingContext.DrawRectangle(brush, GetPen(brush, 1), new System.Windows.Rect(x, Math.Min(y, Mapping.ScreenYZeroValue), barWidth, height));
		}
	}
}
