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
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.XtraScheduler.Drawing {
	public class TimelineViewGroupByDateCellsLayoutCalculator : TimelineViewGroupCellsLayoutCalculator {
		public TimelineViewGroupByDateCellsLayoutCalculator(GraphicsCache cache, SchedulerViewInfoBase viewInfo, ViewInfoPainterBase painter)
			: base(cache, viewInfo, painter) {
		}
		protected internal override Rectangle CalculateTimelineBounds(Rectangle bounds, int index) {
			if (bounds == Rectangle.Empty)
				return bounds;
			Rectangle r = ViewInfo.ResourceHeaders[index].AnchorBounds; 
			Rectangle rect = AdjustWeekVerticallyDown(r.Right, r.Top, bounds.Right, r.Bottom);
			if (index == 0)  
				rect = Rectangle.FromLTRB(r.Right, r.Top, bounds.Right, r.Bottom);
			return rect;
		}
		protected internal override void CreateTimelines() {
			int count = ViewInfo.VisibleResources.Count;
			int shiftedIndexOnCollapedChildResources = 0;
			for (int i = 0; i < count; i++) {
				Resource resource = ViewInfo.VisibleResources[i];
				Timeline timeline = (Timeline)GetCellContainerByResourceAndInterval(resource, View.InnerVisibleIntervals.Interval);
				if (timeline == null)
					timeline = CreateTimeline(resource, View.InnerVisibleIntervals.Interval, GetColorSchema(resource, i + shiftedIndexOnCollapedChildResources));
				else
					timeline.SetViewInfo(ViewInfo);
				IInternalResource internalResource = (IInternalResource)resource;
				if (!internalResource.IsExpanded)
					shiftedIndexOnCollapedChildResources += internalResource.AllChildResourcesCount;
				Timelines.Add(timeline);
			}
		}
	}
}
