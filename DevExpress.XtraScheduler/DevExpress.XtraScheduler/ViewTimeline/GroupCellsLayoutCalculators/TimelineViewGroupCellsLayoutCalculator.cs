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
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.XtraScheduler.Drawing {
	public abstract class TimelineViewGroupCellsLayoutCalculator : SchedulerViewCellsLayoutCalculator {
		SchedulerViewCellContainerCollection timelines;
		protected TimelineViewGroupCellsLayoutCalculator(GraphicsCache cache, SchedulerViewInfoBase viewInfo, ViewInfoPainterBase painter)
			: base(cache, viewInfo, painter) {
			this.timelines = new SchedulerViewCellContainerCollection();
		}
		public SchedulerViewCellContainerCollection Timelines { get { return timelines; } }
		protected internal new TimelineView View { get { return (TimelineView)base.View; } }
		protected internal new TimelineViewInfo ViewInfo { get { return (TimelineViewInfo)base.ViewInfo; } }
		public override void CalcLayout(Rectangle bounds) {
			Timelines.Clear();
			Timelines.AddRange(ViewInfo.PreliminaryLayoutResult.CellContainers);
			CalcTimelinesLayout(bounds);
			XtraSchedulerDebug.Assert(ViewInfo.Timelines.Count == 0);
			ViewInfo.Timelines.AddRange(Timelines);
			SubscribeScrollContainerEvents();
		}
		protected virtual void SubscribeScrollContainerEvents() {
			int count = Timelines.Count;
			for (int i = 0; i < count; i++)
				ViewInfo.SubsribeScrollContainerEvents(Timelines[i]);
		}
		protected internal virtual void CalcTimelinesLayout(Rectangle bounds) {
			for (int i = 0; i < Timelines.Count; i++) {
				Timeline timeline = (Timeline)Timelines[i];
				InitializeContainerBorders(timeline);
				timeline.Bounds = CalculateTimelineBounds(bounds, i);
				TimelineViewPainter painter = (TimelineViewPainter)ViewInfo.Painter;
				timeline.CalcLayout(ViewInfo.SelectionBar.Cells, painter.TimelinePainter, ViewInfo.TimelinePaintAppearance);
			}
		}
		protected internal override void CalculatePreliminaryLayout() {
			CreateTimelines();
			CreateCells();
			ViewInfo.PreliminaryLayoutResult.CellContainers.Clear();
			ViewInfo.PreliminaryLayoutResult.CellContainers.AddRange(Timelines);
		}
		protected internal abstract Rectangle CalculateTimelineBounds(Rectangle bounds, int index);
		protected internal virtual Timeline CreateTimeline(Resource resource, TimeInterval interval, SchedulerColorSchema colorSchema) {
			return new Timeline(ViewInfo, resource, interval, colorSchema);
		}
		protected internal abstract void CreateTimelines();
		protected internal virtual void InitializeContainerBorders(Timeline container) {
			ResetContainerBorders(container);
		}
		void CreateCells() {
			foreach (Timeline timeline in Timelines) {
				timeline.Cells.Clear();
				SchedulerHeaderCollection headers = ViewInfo.PreliminaryLayoutResult.BaseLevel.Headers;
				int count = headers.Count;
				for (int i = 0; i < count; i++) {
					SchedulerHeader header = headers[i];
					TimeInterval interval = header.Interval.Clone();
					TimeCell cell = (TimeCell)timeline.CreateCell(interval);
					timeline.Cells.Add(cell);
				}
			}
		}
	}
}
