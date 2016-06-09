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
using DevExpress.XtraScheduler.Internal.Implementations;
namespace DevExpress.XtraScheduler.Drawing {
	public class WeekViewGroupByDateHeadersLayoutCalculator : WeekViewGroupHeadersLayoutCalculator {
		public WeekViewGroupByDateHeadersLayoutCalculator(GraphicsCache cache, SchedulerViewInfoBase viewInfo, SchedulerHeaderPainter painter)
			: base(cache, viewInfo, painter) {
		}
		protected override SchedulerHeaderCollection TopLevelHeaders { get { return ViewInfo.PreliminaryLayoutResult.DateHeaders; } }
		protected override SchedulerHeaderCollection SubLevelHeaders { get { return ViewInfo.PreliminaryLayoutResult.ResourceHeaders; } }
		public override void CalcLayout(Rectangle bounds) {
			if (bounds == Rectangle.Empty)
				return;
			ViewInfo.Corners.Clear();
			ViewInfo.ResourceHeaders.Clear();
			ViewInfo.GroupSeparators.Clear();
			PerformVerticalResourceHeadersAndSeparatorsLayout(bounds, new int[0]);
			SchedulerHeaderCollection corners = ViewInfo.PreliminaryLayoutResult.Corners;
			Rectangle topResourceHeaderBounds = SubLevelHeaders[0].Bounds;
			PerformUpperCornerLayout(corners, bounds.Location, topResourceHeaderBounds);
			ViewInfo.Corners.AddRange(corners);
			ViewInfo.WeekDaysHeaders.AddRange(TopLevelHeaders);
			int topHeadersHeight = CalculateHeadersHeight(TopLevelHeaders);
			Rectangle actualBounds = Rectangle.FromLTRB(topResourceHeaderBounds.Right - Painter.HorizontalOverlap, bounds.Top, bounds.Right, bounds.Top + topHeadersHeight);
			CalculateDateHeadersBounds(actualBounds, true);
		}
		protected internal override SchedulerHeaderCollection CreateDateHeaders(Resource resource) {
			SchedulerHeaderCollection result = base.CreateDateHeaders(resource);
			if (result.Count > 0)
				result[result.Count - 1].HasRightBorder = true;
			ApplyHeaderSeparators(result);
			return result;
		}
		protected internal override SchedulerHeaderCollection CreateTopLevelHeaders() {
			return CreateDateHeaders(ResourceBase.Empty);
		}
		protected override SchedulerHeaderCollection CreateGroupSeparators() {
			return CreateHorizontalGroupSeparators(ViewInfo.VisibleResources.Count - 1);
		}
		protected override SchedulerHeaderCollection CreateCorners() {
			SchedulerHeaderCollection result = new SchedulerHeaderCollection();
			result.Add(CreateUppreLeftCorner());
			return result;
		}
		protected internal override SchedulerHeaderCollection CreateSubLevelHeaders() {
			return CreateVerticalResourceHeaders();
		}
	}
}
