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
using System.Windows;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Xpf.Charts {
	public class BarStackedSeries2D : BarSeries2D, IStackedView {
		protected override bool ShouldAnimateClipBounds { get { return IsOverlapBars; } }
		protected override bool NeedSeriesInteraction { get { return true; } }
		protected override Type PointInterfaceType { get { return typeof(IStackedPoint); } }
		public BarStackedSeries2D() {
			DefaultStyleKey = typeof(BarStackedSeries2D);
		}
		bool IsOverlapBars { get { return Model != null ? Model.ActualOverlapBars : false; } }
		double GetMinValue(RefinedPoint pointInfo) {
			return 0;
		}
		double GetMaxValue(RefinedPoint pointInfo) {
			return ((IStackedPoint)pointInfo).TotalValue;
		}
		protected override double GetLowValue(RefinedPoint refinedPoint) {
			return ((IStackedPoint)refinedPoint).MinValue;
		}
		protected override double GetHighValue(RefinedPoint refinedPoint) {
			return ((IStackedPoint)refinedPoint).MaxValue;
		}
		protected override double GetBarWidth(RefinedPoint pointInfo) {
			return BarWidth;
		}
		protected override int GetFixedOffset(RefinedPoint pointInfo) {
			return 0;
		}
		protected override double GetDisplayOffset(RefinedPoint pointInfo) {
			return 0;
		}		
		protected override Series CreateObjectForClone() {
			return new BarStackedSeries2D();
		}
		protected override Rect CalculateSeriesPointBounds(PaneMapping mapping, RefinedPoint pointInfo) {
			return IsOverlapBars ? CalculateSeriesPointBounds(mapping, pointInfo, GetMinValue(pointInfo), GetMaxValue(pointInfo)) : base.CalculateSeriesPointBounds(mapping, pointInfo);
		}		
		protected override Rect? CalculateSeriesPointClipBounds(PaneMapping mapping, SeriesPointItem pointItem, Rect viewport, Rect barBounds) {
			if (IsOverlapBars) {
				RefinedPoint pointInfo = pointItem.RefinedPoint;
				Rect clipBounds = CalculateSeriesPointBounds(mapping, pointInfo, GetLowValue(pointInfo), GetHighValue(pointInfo));
				return new Rect(clipBounds.Left - 1, clipBounds.Top, clipBounds.Width + 2, clipBounds.Height);
			}
			return base.CalculateSeriesPointClipBounds(mapping, pointItem, viewport, barBounds);
		}
		protected override Bar2DLabelPosition GetSeriesLabelPosition() {
			return Bar2DLabelPosition.Center;
		}
		protected internal override SeriesPointAnimationBase CreateDefaultPointAnimation() {
			return new Bar2DDropInAnimation();
		}
		protected override double GetRefinedPointMax(RefinedPoint point) {
			return ((IStackedPoint)point).Value;
		}
		protected override double GetRefinedPointMin(RefinedPoint point) {
			return ((IStackedPoint)point).Value;
		}
		protected override double GetRefinedPointAbsMin(RefinedPoint point) {
			return Math.Abs(((IStackedPoint)point).Value);
		}
		protected override SeriesContainer CreateContainer() {
			return new StackedInteractionContainer(this);
		}
		protected override IEnumerable<double> GetCrosshairValues(RefinedPoint refinedPoint) {
			yield return ((IStackedPoint)refinedPoint).MaxValue;
		}
	}
}
