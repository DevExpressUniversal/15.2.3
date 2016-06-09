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
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Charts.Native;
namespace DevExpress.Xpf.Charts.Native {
	class Pie2DCalulator {
		static double CalculatePointToLineDistance(Point linePoint1, Point linePoint2, Point point) {
			double distance;
			if (linePoint2.X == linePoint1.X) {
				distance = Math.Abs(point.X - linePoint1.X);
			}
			else {
				double a1 = (linePoint2.Y - linePoint1.Y) / (linePoint2.X - linePoint1.X);
				double c1 = linePoint1.Y - linePoint1.X * a1;
				distance = Math.Abs(a1 * point.X - point.Y + c1) / Math.Sqrt(a1 * a1 + 1);
			}
			return distance;
		}
		public static Thickness CalculateMaxLabelPadding(Rect viewport, PieSeries2D series, SeriesLabel seriesLabel) {
			double maxRadius = Math.Min(0.5 * viewport.Width, 0.5 * viewport.Height);
			double maxHoleRadius = maxRadius * series.HoleRadiusPercent / 100.0;
			double maxLabelHeight = 0;
			double maxLabelWidth = 0;
			if (seriesLabel.Items != null) {
				foreach (SeriesLabelItem lebelItem in seriesLabel.Items) {
					if (maxLabelHeight < lebelItem.LabelSize.Height)
						maxLabelHeight = lebelItem.LabelSize.Height;
					if (maxLabelWidth < lebelItem.LabelSize.Width)
						maxLabelWidth = lebelItem.LabelSize.Width;
				}
			}
			if (seriesLabel.Items == null || seriesLabel.Items.Count == 0) 
				return new Thickness(0);
			switch (PieSeries.GetLabelPosition(seriesLabel)) {
				case PieLabelPosition.Outside:
					maxLabelHeight += seriesLabel.Indent;
					maxLabelWidth += seriesLabel.Indent;
					double maxLabelPadding = Math.Max(maxRadius + maxLabelWidth - viewport.Width / 2,
						maxRadius + maxLabelHeight - viewport.Height / 2);
					return maxLabelPadding > 0 ? new Thickness(maxLabelPadding) : new Thickness(0);
				case PieLabelPosition.TwoColumns:
					maxLabelHeight = 0.5 * maxLabelHeight + seriesLabel.Indent;
					maxLabelWidth += seriesLabel.Indent;
					return new Thickness(maxLabelWidth, maxLabelHeight, maxLabelWidth, maxLabelHeight);
				case PieLabelPosition.Inside:
					maxLabelHeight /= 2;
					maxLabelWidth /= 2;
					double labelPosition = (maxRadius - maxHoleRadius) / 2 + maxHoleRadius;
					maxLabelPadding = Math.Max(labelPosition + maxLabelWidth - viewport.Width / 2,
						labelPosition + maxLabelHeight - viewport.Height / 2);
					return maxLabelPadding > 0 ? new Thickness(maxLabelPadding) : new Thickness(0);
				default:
					ChartDebug.Fail("Unknown PieLabelPosition value.");
					return new Thickness(0);
			}
		} 
	}
}
