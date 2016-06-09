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

using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Internal;
#if WPF || SL
using DevExpress.Xpf.Scheduler.Drawing;
using DevExpress.Xpf.Scheduler.Commands;
#endif
namespace DevExpress.Xpf.Scheduler.Native {
	#region GanttViewFactoryHelper
	public class GanttViewFactoryHelper : ViewFactoryHelper {
		public override ViewGroupTypeStrategy CreateGroupByNoneStrategy() {
			return new TimelineViewGroupByNoneStrategy();
		}
		public override ViewGroupTypeStrategy CreateGroupByDateStrategy() {
			return new GanttViewGroupByDateStrategy();
		}
		public override ViewGroupTypeStrategy CreateGroupByResourceStrategy() {
			return new GanttViewGroupByDateStrategy();
		}
		public override AppointmentBaseLayoutCalculator CreateAppointmentLayoutCalculator(ISchedulerViewInfoBase viewInfo, bool alternate) {
			return new TimelineViewAppointmentLayoutCalculator(viewInfo);
		}
	}
	#endregion
	#region GanttViewGroupByNoneStrategy
	public class GanttViewGroupByNoneStrategy : ViewGroupTypeStrategy {
		public override IContinuousCellsInfosCalculator CreateVisuallyContinuousCellsInfosCalculator(bool alternate) {
			return new GanttViewGroupByNoneVisuallyContinuousCellsInfosCalculator();
		}
		public override ISchedulerViewInfoBase CreateViewInfo(SchedulerViewBase view) {
			return new GanttViewGroupByNone((GanttView)view);
		}
	}
	#endregion
	#region GanttViewGroupByNone
	public class GanttViewGroupByNone : TimelineViewInfoBase {
		public GanttViewGroupByNone(TimelineView view)
			: base(view) {
		}
		public override ResourceBaseCollection Resources { get { return WeekViewInfoBase.EmptyResourceCollection; } }
		protected internal override ResourceHeaderBase CreateResourceHeader(Resource resource, ResourceBrushes brushes) {
			return null;
		}
	}
	#endregion
	#region GanttViewGroupByNoneVisuallyContinuousCellsInfosCalculator
	public class GanttViewGroupByNoneVisuallyContinuousCellsInfosCalculator : ContinuousCellsInfosCalculator<GanttViewGroupByNone> {
		public override ResourceVisuallyContinuousCellsInfosCollection Calculate(ISchedulerViewInfoBase view, ResourceBaseCollection resources) {
			return base.Calculate(view, WeekViewInfoBase.EmptyResourceCollection);
		}
		protected internal override VisuallyContinuousCellsInfoCollection CreateGroup(GanttViewGroupByNone ganttView, int resourceIndex) {
			TimeCellCollection cells = ganttView.ResourcesCellContainers[0][0].Cells;
			return VisuallyContinuousCellsInfoHelper.CreateCollectionWithOneItem(cells);
		}
	}
	#endregion
	#region GanttViewGroupByDateStrategy
	public class GanttViewGroupByDateStrategy : ViewGroupTypeStrategy {
		public override IContinuousCellsInfosCalculator CreateVisuallyContinuousCellsInfosCalculator(bool alternate) {
			return new GanttViewGroupByDateVisuallyContinuousCellsInfosCalculator();
		}
		public override ISchedulerViewInfoBase CreateViewInfo(SchedulerViewBase view) {
			return new GanttViewGroupByDate((GanttView)view);
		}
	}
	#endregion
	#region GanttViewGroupByDateVisuallyContinuousCellsInfosCalculator
	public class GanttViewGroupByDateVisuallyContinuousCellsInfosCalculator : ContinuousCellsInfosCalculator<GanttViewGroupByDate> {
		protected internal override VisuallyContinuousCellsInfoCollection CreateGroup(GanttViewGroupByDate ganttView, int resourceIndex) {
			TimelineCellContainer element = ganttView.ResourcesCellContainers[resourceIndex][0];
			TimeCellCollection cells = element.Cells;
			return VisuallyContinuousCellsInfoHelper.CreateCollectionWithOneItem(cells);
		}
	}
	#endregion
	#region GanttViewGroupByDate
	public class GanttViewGroupByDate : TimelineViewInfoBase {
		public GanttViewGroupByDate(GanttView view)
			: base(view) {
		}
		protected internal override SingleResourceViewInfo CreateSingleResourceViewInfo() {
			return new GanttViewSingleResourceViewInfo(View);
		}
		protected internal override SingleResourceViewInfo CreateSingleResourceView(Resource resource, TimeIntervalCollection visibleIntervals, ResourceBrushes brushes) {
			GanttViewSingleResourceViewInfo result = (GanttViewSingleResourceViewInfo)base.CreateSingleResourceView(resource, visibleIntervals, brushes);
			result.NestingLevel = CalculateNesitngLevel(resource);
			result.IsExpanded = ((IInternalResource)resource).IsExpanded;
			result.ExpandCommand = CreateExpandCommand(resource);
			result.CollapseCommand = CreateCollapseCommand(resource);
			return result;
		}
		int CalculateNesitngLevel(Resource resource) {
			int result = 0;
			for (Resource childResource = resource; childResource.ParentId != null; result++)
				childResource = base.Resources.GetResourceById(childResource.ParentId);
			return result;
		}
		protected virtual SchedulerUICommandBase CreateExpandCommand(Resource resource) {
			return new ExpandResourceUICommand(resource);
		}
		protected virtual SchedulerUICommandBase CreateCollapseCommand(Resource resource) {
			return new CollapseResourceUICommand(resource);
		}
	}
	#endregion
}
