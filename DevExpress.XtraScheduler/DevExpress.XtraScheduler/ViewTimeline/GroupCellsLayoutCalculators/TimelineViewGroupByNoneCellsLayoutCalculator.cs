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
using DevExpress.XtraScheduler.Internal.Implementations;
namespace DevExpress.XtraScheduler.Drawing {
	public class TimelineViewGroupByNoneCellsLayoutCalculator : TimelineViewGroupCellsLayoutCalculator {
		public TimelineViewGroupByNoneCellsLayoutCalculator(GraphicsCache cache, SchedulerViewInfoBase viewInfo, ViewInfoPainterBase painter)
			: base(cache, viewInfo, painter) {
		}
		protected internal override int CalculateResourceColorIndex(int visibleIndex) {
			return 0;
		}
		protected internal override Rectangle CalculateTimelineBounds(Rectangle bounds, int index) {
			return Rectangle.FromLTRB(bounds.Left, ViewInfo.SelectionBar.Bounds.Bottom, bounds.Right, bounds.Bottom);
		}
		protected internal override void CreateTimelines() {
			Timeline timeline = (Timeline)GetCellContainerByResourceAndInterval(ResourceBase.Empty, View.InnerVisibleIntervals.Interval);
			if (timeline == null)
				timeline = CreateTimeline(ResourceBase.Empty, View.InnerVisibleIntervals.Interval, GetColorSchema(ResourceBase.Empty, 0));
			else
				timeline.SetViewInfo(ViewInfo);
			Timelines.Add(timeline);
		}
	}
}
