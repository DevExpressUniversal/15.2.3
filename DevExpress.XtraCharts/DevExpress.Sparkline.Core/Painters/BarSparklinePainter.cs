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

using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Windows.Forms;
using System;
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.Sparkline.Core {
	public class BarSparklinePainter : BaseSparklinePainter {
		BarSparklineViewBase BarView { get { return (BarSparklineViewBase)View; } }
		protected override bool EnableAntialiasing { get { return false; } }
		public override SparklineViewType SparklineType { get { return SparklineViewType.Bar; } }
		protected override Padding GetMarkersPadding() {
			return new Padding();
		}
		protected override void DrawInternal(Graphics graphics) {
			int barWidth = Math.Max(1, SparklineMathUtils.Round(Mapping.MinPointsDistancePx) - BarView.BarDistance);
			int halfBarWidth = (int)Math.Round((double)barWidth / 2);
			int startIndex = GetIndexOfFirstPointForDrawing();
			int endIndex = GetIndexOfLastPointForDrawing();
			for (int i = startIndex; i <= endIndex; i++) {
				SparklinePoint point = DataProvider.SortedPoints[i];
				if (!SparklineMathUtils.IsValidDouble(point.Value) || !SparklineMathUtils.IsValidDouble(point.Argument))
					continue;
				Point screenPoint = Mapping.MapPoint(point.Argument, point.Value);
				screenPoint.X -= halfBarWidth;
				int height = Math.Abs(screenPoint.Y - Mapping.ScreenYZeroValue);
				PointPresentationType pointType = GetPointPresentationType(i);
				Color color = GetPointColor(pointType);
				graphics.FillRectangle(GetSolidBrush(color), screenPoint.X, Math.Min(screenPoint.Y, Mapping.ScreenYZeroValue), barWidth, height);
			}
		}
	}
}
