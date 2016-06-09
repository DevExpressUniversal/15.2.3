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

using System.Drawing;
using DevExpress.Utils.Drawing;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.XtraScheduler.Drawing {
	public class TimelineViewGroupByDateHeadersLayoutCalculator : TimelineViewGroupHeadersLayoutCalculator {
		internal static int LayoutCorrection = 1;
		public TimelineViewGroupByDateHeadersLayoutCalculator(GraphicsCache cache, SchedulerViewInfoBase viewInfo, SchedulerHeaderPainter painter)
			: base(cache, viewInfo, painter) {
		}
		public override void CalcLayout(Rectangle bounds) {
			CalcLayout(bounds, new int[0]);
		}
		public void CalcLayout(Rectangle bounds, int[] headersHeight) {
			if (bounds == Rectangle.Empty)
				return;
			ViewInfo.Corners.Clear();
			ViewInfo.ResourceHeaders.Clear();
			ViewInfo.GroupSeparators.Clear();
			PerformVerticalResourceHeadersAndSeparatorsLayout(bounds, headersHeight);
			SchedulerHeaderCollection corners = ViewInfo.PreliminaryLayoutResult.Corners;
			Rectangle topResourceHeaderBounds = SubLevelHeaders[0].Bounds;
			PerformUpperCornerLayout(corners, bounds.Location, topResourceHeaderBounds);
			ViewInfo.Corners.AddRange(corners);
			Rectangle actualBounds = Rectangle.FromLTRB(topResourceHeaderBounds.Right - Painter.HorizontalOverlap, bounds.Top, bounds.Right, bounds.Bottom);
			PerformScaleHeadersLayout(actualBounds);
		}
		public override int CalculateResourceHeadersWidht(Rectangle bounds) {
			SchedulerHeaderCollection resourceHeaders = CreateSubLevelHeaders();
			SchedulerHeaderCollection groupSeparators = CreateGroupSeparators();
			if (resourceHeaders.Count == 0)
				return 0;
			TimeScaleLevelsCalculator calc = new TimeScaleLevelsCalculator();
			TimeScaleLevelCollection levels = calc.Calculate(ViewInfo.Scales, View.InnerVisibleIntervals.Start, 1);
			SchedulerHeaderCollection baseHeaders = CreateLevelHeaders(levels[levels.Count - 1], true);
			int topHeadersHeight = CalculateHeadersHeight(baseHeaders);
			Rectangle resourceHeadersBounds = new Rectangle(0, 0, bounds.Height - topHeadersHeight + Painter.VerticalOverlap, bounds.Width);
			CalculateResourceHeadersBounds(resourceHeadersBounds, resourceHeaders, groupSeparators, true, new int[0]);
			return resourceHeaders[0].Bounds.Height;
		}
		protected internal override void AdjustHeadersWidth(int[] headerAnchorsWidth, bool groupSeparatorBeforeHeaders, bool groupSeparatorAfterHeaders) {
		}
		protected internal override Rectangle[] CalculateInitialHeadersEqualRectangles(Rectangle bounds, SchedulerHeaderCollection headers, int groupSeparatorWidth) {
			Rectangle extendedBounds = bounds;
			extendedBounds.Width += groupSeparatorWidth;
			ResourceHeadersHeightCalculator calculator = new ResourceHeadersHeightCalculator();
			int[] widths = calculator.Calculate(headers, extendedBounds.Width, groupSeparatorWidth, View.CollapsedResourceHeight);
			return CalculateInitialHeadersEqualRectanglesCore(headers, extendedBounds, widths);
		}
		protected internal override int CalculateHeadersHeight(SchedulerHeaderCollection headers) {
			int result = ViewInfo.CalculateSelectionBarHeight();
			int count = ViewInfo.Scales.Count;
			XtraSchedulerDebug.Assert(count > 0);
			SchedulerHeaderPreliminaryLayoutResultCollection preliminaryResults = CalculateHeadersPreliminaryLayout(headers);
			int levelHeight = CalculateHeadersHeight(preliminaryResults);
			for (int i = 0; i < count; i++) {
				TimeScale item = ViewInfo.Scales[i];
				if (item.Visible)
					result += (levelHeight - Painter.VerticalOverlap);
			}
			result += LayoutCorrection;
			return result;
		}
		protected override SchedulerHeaderCollection TopLevelHeaders {
			get { return ViewInfo.PreliminaryLayoutResult.BaseLevel.Headers; }
		}
		protected override SchedulerHeaderCollection SubLevelHeaders {
			get { return ViewInfo.PreliminaryLayoutResult.ResourceHeaders; }
		}
		protected override SchedulerHeaderCollection CreateGroupSeparators() {
			return CreateHorizontalGroupSeparators(ViewInfo.VisibleResources.Count - 1);
		}
		protected internal override SchedulerHeaderCollection CreateSubLevelHeaders() {
			return CreateVerticalResourceHeaders();
		}
		protected override SchedulerHeaderCollection CreateCorners() {
			SchedulerHeaderCollection result = new SchedulerHeaderCollection();
			result.Add(CreateUppreLeftCorner());
			return result;
		}
		Rectangle[] CalculateInitialHeadersEqualRectanglesCore(SchedulerHeaderCollection headers, Rectangle bounds, int[] widths) {
			int count = headers.Count;
			if (count <= 0)
				return new Rectangle[] { bounds };
			XtraSchedulerDebug.Assert(headers.Count == widths.Length);
			Rectangle[] result = new Rectangle[count];
			int offset = bounds.X;
			for (int i = 0; i < count; i++) {
				result[i] = new Rectangle(offset, bounds.Y, widths[i], bounds.Height);
				offset += widths[i];
			}
			return result;
		}
	}
}
