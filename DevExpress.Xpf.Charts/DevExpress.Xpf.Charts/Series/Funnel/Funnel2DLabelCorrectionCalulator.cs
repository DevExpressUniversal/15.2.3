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

using DevExpress.Charts.Native;
using System;
using System.Collections.Generic;
using System.Windows;
namespace DevExpress.Xpf.Charts.Native {
	class Funnel2DLabelCorrectionCalulator {
		private static double CalculateOffsetForColumnPosition(List<FunnelLableInfo> labelInfos, int connectorLength) {
			double maxOffset = 0;
			for (int i = 0; i < labelInfos.Count; i++) {
				FunnelLableInfo labelItem = labelInfos[i];
				maxOffset = Math.Max(maxOffset, labelItem.LabelSize.Width + connectorLength);
			}
			return maxOffset;
		}
		private static double CalculateOffsetForOutsidePosition(Rect viewport, AdditionalLengthCalculator lengthCalculator, List<FunnelLableInfo> labelInfos, int connectorLength, bool alignToCenter, bool heightToWidthRatioAuto) {
			lengthCalculator.Calculate(labelInfos, connectorLength);
			if (alignToCenter || !heightToWidthRatioAuto)
				return CalculateOffsetForAlignToCenterSeries(viewport, lengthCalculator, labelInfos, connectorLength);
			return CalculateOffsetForStretchSeries(viewport, lengthCalculator, labelInfos, connectorLength);
		}
		private static double CalculateOffsetForStretchSeries(Rect viewport, AdditionalLengthCalculator additionalLengthCalculator, List<FunnelLableInfo> labelInfos, int connectorLength) {
			double maxOffset = 0;
			for (int i = 0; i < labelInfos.Count; i++) {
				FunnelLableInfo labelItem = labelInfos[i];
				double funnelPointX = (0.5 + ((IFunnelPoint)labelItem.RefinedPoint).NormalizedValue / 2) * viewport.Width;
				double widthWithLabel = labelItem.LabelSize.Width + funnelPointX + connectorLength + additionalLengthCalculator.GetLength(labelItem.RefinedPoint);
				double offset = (widthWithLabel - viewport.Width) * viewport.Width / funnelPointX;
				if ((widthWithLabel > viewport.Width) && (maxOffset < offset))
					maxOffset = offset;
				if (maxOffset > viewport.Width) {
					maxOffset = viewport.Width;
					break;
				}
			}
			return maxOffset;
		}
		private static double CalculateOffsetForAlignToCenterSeries(Rect viewport, AdditionalLengthCalculator additionalLengthCalculator, List<FunnelLableInfo> labelInfos, int connectorLength) {
			double maxOffset = 0;
			for (int i = 0; i < labelInfos.Count; i++) {
				FunnelLableInfo labelItem = labelInfos[i];
				double halfTotalWidth = viewport.Width / 2;
				double halfPointWidth = ((IFunnelPoint)labelItem.RefinedPoint).NormalizedValue * halfTotalWidth;
				double halfWidthWithLabel = halfPointWidth + labelItem.LabelSize.Width + connectorLength + additionalLengthCalculator.GetLength(labelItem.RefinedPoint);
				double offset = (halfWidthWithLabel - halfTotalWidth) / ((IFunnelPoint)labelItem.RefinedPoint).NormalizedValue;
				if ((halfWidthWithLabel > halfTotalWidth) && (maxOffset < offset))
					maxOffset = offset;
				if (maxOffset > halfTotalWidth) {
					maxOffset = halfTotalWidth;
					break;
				}
			}
			return maxOffset;
		}
		public static Thickness CalculateMaxLabelPadding(Rect viewport, AdditionalLengthCalculator lengthCalculator, List<FunnelLableInfo> labelInfos, int connectorLength, bool alignToCenter, bool heightToWidthRatioAuto, Funnel2DLabelPosition labelPosition) {
			double halfTopLabelHeight = labelInfos.Count > 0 ? labelInfos[0].LabelSize.Height / 2 : 0;
			if (labelPosition == Funnel2DLabelPosition.Center)
				return new Thickness(0);
			else {
				double maxOffset = 0;
				if (labelPosition == Funnel2DLabelPosition.Left || labelPosition == Funnel2DLabelPosition.Right)
					maxOffset = CalculateOffsetForOutsidePosition(viewport, lengthCalculator, labelInfos, connectorLength, alignToCenter, heightToWidthRatioAuto);
				else
					maxOffset = CalculateOffsetForColumnPosition(labelInfos, connectorLength);
				if (alignToCenter)
					return new Thickness(maxOffset, halfTopLabelHeight, maxOffset, 0);
				if (labelPosition == Funnel2DLabelPosition.Left || labelPosition == Funnel2DLabelPosition.LeftColumn)
					return new Thickness(maxOffset, halfTopLabelHeight, 0, 0);
				return new Thickness(0, halfTopLabelHeight, maxOffset, 0);
			}
		}
	}
	class AdditionalLengthCalculator {
		Dictionary<RefinedPoint, int> connectorAdditionalLengths;
		public AdditionalLengthCalculator() {
			connectorAdditionalLengths = new Dictionary<RefinedPoint, int>();
		}
		void AddLength(RefinedPoint refinedPoint, int additionalConnectorLength) {
			if (connectorAdditionalLengths.ContainsKey(refinedPoint))
				connectorAdditionalLengths[refinedPoint] = Math.Max(connectorAdditionalLengths[refinedPoint], additionalConnectorLength);
			else
				connectorAdditionalLengths[refinedPoint] = additionalConnectorLength;
		}
		int CalculateLenght(int connectorLength, GRealPoint2D topPointOfPolygon, GRealPoint2D bottomPointOfPolygon, GRealSize2D labelSize, GRealPoint2D anhorLabelPoint, bool forTopPolygon) {
			int sign = forTopPolygon ? -1 : 1;
			GRealPoint2D rightLabelPoint = new GRealPoint2D(anhorLabelPoint.X - connectorLength, anhorLabelPoint.Y + sign * labelSize.Height / 2);
			GRealPoint2D? pointIntersection = Funnel2DLayoutCalculator.CalcIntersectionPoint(topPointOfPolygon, bottomPointOfPolygon, rightLabelPoint.Y);
			if (pointIntersection.HasValue && pointIntersection.Value.X < rightLabelPoint.X) 
				return (int)(rightLabelPoint.X - pointIntersection.Value.X) + 1;
			return 0;
		}
		public void Calculate(List<FunnelLableInfo> labelInfos, int connectorLength) {
			connectorAdditionalLengths.Clear();
			if (labelInfos == null)
				return;
			for (int i = 0; i < labelInfos.Count; i++) {
				GRealSize2D labelItemSize = labelInfos[i].LabelSize;
				FunnelPointInfo pointLayout = labelInfos[i].PointInfo;
				int length = 0;
				if (i > 0) {
					FunnelPointInfo prevLayout = labelInfos[i - 1].PointInfo;
					length = CalculateLenght(
						connectorLength,
						prevLayout.TopLeftPoint,
						prevLayout.BottomLeftPoint,
						labelItemSize,
						pointLayout.TopLeftPoint,
						true);
					AddLength(labelInfos[i].RefinedPoint, length);
				}
				length = CalculateLenght(
						connectorLength,
						pointLayout.TopLeftPoint,
						pointLayout.BottomLeftPoint,
						labelItemSize,
						pointLayout.TopLeftPoint,
						false);
				AddLength(labelInfos[i].RefinedPoint, length);
			}
		}
		public int GetLength(RefinedPoint refinedPoint) {
			if (connectorAdditionalLengths.ContainsKey(refinedPoint))
				return connectorAdditionalLengths[refinedPoint];
			return 0;
		}
	}
}
