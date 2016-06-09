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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
namespace DevExpress.XtraScheduler {
	#region WorkWeekView
	public class WorkWeekView : DayView {
		public WorkWeekView(SchedulerControl control)
			: base(control) {
		}
		#region Properties
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("WorkWeekViewAppearance"),
#endif
 Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new WorkWeekViewAppearance Appearance { get { return (WorkWeekViewAppearance)base.Appearance; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override SchedulerViewType Type { get { return SchedulerViewType.WorkWeek; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override SchedulerMenuItemId MenuItemId { get { return SchedulerMenuItemId.SwitchToWorkWeekView; } }
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("WorkWeekViewShowFullWeek"),
#endif
 DefaultValue(InnerWorkWeekView.DefaultShowFullWeek), XtraSerializableProperty()]
		public bool ShowFullWeek {
			get {
				if (InnerView != null) {
					InnerWorkWeekView innerView = (InnerWorkWeekView)InnerView;
					return innerView.ShowFullWeek;
				}
				else
					return false;
			}
			set {
				if (InnerView != null) {
					InnerWorkWeekView innerView = (InnerWorkWeekView)InnerView;
					innerView.ShowFullWeek = value;
				}
			}
		}
		#endregion
		protected internal override InnerSchedulerViewBase CreateInnerView() {
			return new InnerWorkWeekView(this, new WorkWeekViewProperties());
		}
		protected internal override ViewDateTimeScrollController CreateDateTimeScrollController() {
			return new WorkWeekViewDateTimeScrollController(this);
		}
		protected internal override BaseViewAppearance CreateAppearance() {
			return new WorkWeekViewAppearance();
		}
		protected internal override ViewPainterBase CreatePainter(SchedulerPaintStyle paintStyle) {
			if (paintStyle == null)
				Exceptions.ThrowArgumentException("paintStyle", paintStyle);
			return paintStyle.CreateWorkWeekViewPainter();
		}
		protected internal override ViewFactoryHelper CreateFactoryHelper() {
			return new WorkWeekViewFactoryHelper();
		}
		protected internal override SchedulerViewInfoBase CreateViewInfoCore() {
			return new WorkWeekViewInfo(this);
		}
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Drawing {
	public class WorkWeekViewInfo : DayViewInfo {
		public WorkWeekViewInfo(WorkWeekView view)
			: base(view) {
		}
		#region Properties
		public new WorkWeekView View { get { return (WorkWeekView)base.View; } }
		#endregion
	}
	#region WorkWeekViewFactoryHelper
	public class WorkWeekViewFactoryHelper : ViewFactoryHelper {
		public override ViewGroupTypeStrategy CreateGroupByNoneStrategy() {
			return new WorkWeekViewGroupByNoneStrategy();
		}
		public override ViewGroupTypeStrategy CreateGroupByDateStrategy() {
			return new WorkWeekViewGroupByDateStrategy();
		}
		public override ViewGroupTypeStrategy CreateGroupByResourceStrategy() {
			return new WorkWeekViewGroupByResourceStrategy();
		}
		public override AppointmentsBaseLayoutStrategy CreateAppointmentsLayoutStrategy(SchedulerViewBase view) {
			return new WorkWeekViewDispatchAppointmentsLayoutStrategy((WorkWeekView)view);
		}
	}
	#endregion
	#region WorkWeekViewGroupByNoneStrategy
	public class WorkWeekViewGroupByNoneStrategy : ViewGroupTypeStrategy {
		public override SchedulerViewHeadersLayoutCalculator CreateHeadersLayoutCalculator(GraphicsCache cache, SchedulerViewInfoBase viewInfo, SchedulerHeaderPainter painter) {
			return new WorkWeekViewGroupByNoneHeadersLayoutCalculator(cache, viewInfo, painter);
		}
		public override SchedulerViewCellsLayoutCalculator CreateCellsLayoutCalculator(GraphicsCache cache, SchedulerViewInfoBase viewInfo, ViewInfoPainterBase painter) {
			return new WorkWeekViewGroupByNoneCellsLayoutCalculator(cache, viewInfo, painter);
		}
		public override VisuallyContinuousCellsInfosCalculator CreateVisuallyContinuousCellsInfosCalculator(SchedulerViewInfoBase viewInfo, bool alternate) {
			if (alternate)
				return new WorkWeekViewGroupByNoneAllDayAreaVisuallyContinuousCellsInfosCalculator();
			else
				return new WorkWeekViewGroupByNoneVisuallyContinuousCellsInfosCalculator();
		}
	}
	#endregion
	#region WorkWeekViewGroupByDateStrategy
	public class WorkWeekViewGroupByDateStrategy : ViewGroupTypeStrategy {
		public override SchedulerViewHeadersLayoutCalculator CreateHeadersLayoutCalculator(GraphicsCache cache, SchedulerViewInfoBase viewInfo, SchedulerHeaderPainter painter) {
			return new WorkWeekViewGroupByDateHeadersLayoutCalculator(cache, viewInfo, painter);
		}
		public override SchedulerViewCellsLayoutCalculator CreateCellsLayoutCalculator(GraphicsCache cache, SchedulerViewInfoBase viewInfo, ViewInfoPainterBase painter) {
			return new WorkWeekViewGroupByDateCellsLayoutCalculator(cache, viewInfo, painter);
		}
		public override VisuallyContinuousCellsInfosCalculator CreateVisuallyContinuousCellsInfosCalculator(SchedulerViewInfoBase viewInfo, bool alternate) {
			if (alternate)
				return new WorkWeekViewGroupByDateAllDayAreaVisuallyContinuousCellsInfosCalculator();
			else
				return new WorkWeekViewGroupByDateVisuallyContinuousCellsInfosCalculator();
		}
	}
	#endregion
	#region WorkWeekViewGroupByResourceStrategy
	public class WorkWeekViewGroupByResourceStrategy : ViewGroupTypeStrategy {
		public override SchedulerViewHeadersLayoutCalculator CreateHeadersLayoutCalculator(GraphicsCache cache, SchedulerViewInfoBase viewInfo, SchedulerHeaderPainter painter) {
			return new WorkWeekViewGroupByResourceHeadersLayoutCalculator(cache, viewInfo, painter);
		}
		public override SchedulerViewCellsLayoutCalculator CreateCellsLayoutCalculator(GraphicsCache cache, SchedulerViewInfoBase viewInfo, ViewInfoPainterBase painter) {
			return new WorkWeekViewGroupByResourceCellsLayoutCalculator(cache, viewInfo, painter);
		}
		public override VisuallyContinuousCellsInfosCalculator CreateVisuallyContinuousCellsInfosCalculator(SchedulerViewInfoBase viewInfo, bool alternate) {
			if (alternate)
				return new WorkWeekViewGroupByResourceAllDayAreaVisuallyContinuousCellsInfosCalculator();
			else
				return new WorkWeekViewGroupByResourceVisuallyContinuousCellsInfosCalculator();
		}
	}
	#endregion
	#region WorkWeekViewGroupByNoneHeadersLayoutCalculator
	public class WorkWeekViewGroupByNoneHeadersLayoutCalculator : DayViewGroupByNoneHeadersLayoutCalculator {
		public WorkWeekViewGroupByNoneHeadersLayoutCalculator(GraphicsCache cache, SchedulerViewInfoBase viewInfo, SchedulerHeaderPainter painter)
			: base(cache, viewInfo, painter) {
		}
	}
	#endregion
	#region WorkWeekViewGroupByDateHeadersLayoutCalculator
	public class WorkWeekViewGroupByDateHeadersLayoutCalculator : DayViewGroupByDateHeadersLayoutCalculator {
		public WorkWeekViewGroupByDateHeadersLayoutCalculator(GraphicsCache cache, SchedulerViewInfoBase viewInfo, SchedulerHeaderPainter painter)
			: base(cache, viewInfo, painter) {
		}
	}
	#endregion
	#region WorkWeekViewGroupByResourceHeadersLayoutCalculator
	public class WorkWeekViewGroupByResourceHeadersLayoutCalculator : DayViewGroupByResourceHeadersLayoutCalculator {
		public WorkWeekViewGroupByResourceHeadersLayoutCalculator(GraphicsCache cache, SchedulerViewInfoBase viewInfo, SchedulerHeaderPainter painter)
			: base(cache, viewInfo, painter) {
		}
	}
	#endregion
	#region WorkWeekViewGroupByNoneCellsLayoutCalculator
	public class WorkWeekViewGroupByNoneCellsLayoutCalculator : DayViewGroupByNoneCellsLayoutCalculator {
		public WorkWeekViewGroupByNoneCellsLayoutCalculator(GraphicsCache cache, SchedulerViewInfoBase viewInfo, ViewInfoPainterBase painter)
			: base(cache, viewInfo, painter) {
		}
	}
	#endregion
	#region WorkWeekViewGroupByDateCellsLayoutCalculator
	public class WorkWeekViewGroupByDateCellsLayoutCalculator : DayViewGroupByDateCellsLayoutCalculator {
		public WorkWeekViewGroupByDateCellsLayoutCalculator(GraphicsCache cache, SchedulerViewInfoBase viewInfo, ViewInfoPainterBase painter)
			: base(cache, viewInfo, painter) {
		}
	}
	#endregion
	#region WorkWeekViewGroupByResourceCellsLayoutCalculator
	public class WorkWeekViewGroupByResourceCellsLayoutCalculator : DayViewGroupByResourceCellsLayoutCalculator {
		public WorkWeekViewGroupByResourceCellsLayoutCalculator(GraphicsCache cache, SchedulerViewInfoBase viewInfo, ViewInfoPainterBase painter)
			: base(cache, viewInfo, painter) {
		}
	}
	#endregion
	#region WorkWeekViewDispatchAppointmentsLayoutStrategy
	public class WorkWeekViewDispatchAppointmentsLayoutStrategy : DayViewDispatchAppointmentsLayoutStrategy {
		public WorkWeekViewDispatchAppointmentsLayoutStrategy(WorkWeekView view)
			: base(view) {
		}
		public new WorkWeekView View { get { return (WorkWeekView)base.View; } }
		protected internal override DayViewAllDayAppointmentsLayoutStrategy CreateAllDayLayoutStrategy(SchedulerViewBase view) {
			return new WorkWeekViewAllDayAppointmentsLayoutStrategy(view);
		}
		protected internal override DayViewTimeCellAppointmentsLayoutStrategy CreateTimeCellLayoutStrategy(SchedulerViewBase view) {
			return new WorkWeekViewTimeCellAppointmentsLayoutStrategy(View);
		}
	}
	#endregion
	#region WorkWeekViewAllDayAppointmentsLayoutStrategy
	public class WorkWeekViewAllDayAppointmentsLayoutStrategy : DayViewAllDayAppointmentsLayoutStrategy {
		public WorkWeekViewAllDayAppointmentsLayoutStrategy(SchedulerViewBase view)
			: base(view) {
		}
		protected internal override AppointmentContentLayoutCalculator CreateAppointmentContentLayoutCalculator() {
			return new WorkWeekViewAllDayAppointmentContentLayoutCalculator((WorkWeekViewInfo)ViewInfo, ViewInfo.Painter.AppointmentPainter);
		}
		protected internal override HorizontalAppointmentLayoutCalculator CreateAppointmentLayoutCalculator(GraphicsCache cache) {
			AppointmentContentLayoutCalculator contentCalculator = CreateAppointmentContentLayoutCalculator();
			AppointmentPainter painter = ViewInfo.Painter.AppointmentPainter;
			if (View.AppointmentDisplayOptions.AppointmentAutoHeight)
				return new WorkWeekViewAllDayAppointmentAutoHeightLayoutCalculator(ViewInfo, contentCalculator, cache, painter);
			else
				return new WorkWeekViewAllDayAppointmentFixedHeightLayoutCalculator(ViewInfo, contentCalculator, cache, painter);
		}
	}
	#endregion
	#region WorkWeekViewTimeCellAppointmentsLayoutStrategy
	public class WorkWeekViewTimeCellAppointmentsLayoutStrategy : DayViewTimeCellAppointmentsLayoutStrategy {
		public WorkWeekViewTimeCellAppointmentsLayoutStrategy(WorkWeekView view)
			: base(view) {
		}
		protected internal override AppointmentContentLayoutCalculator CreateAppointmentContentLayoutCalculator(AppointmentPainter painter) {
			return new WorkWeekViewTimeCellAppointmentContentLayoutCalculator((WorkWeekViewInfo)ViewInfo, painter);
		}
		protected internal override AppointmentBaseLayoutCalculator CreateAppointmentLayoutCalculator(GraphicsCache cache) {
			AppointmentPainter painter = ((DayViewPainter)ViewInfo.Painter).TimeCellsAppointmentPainter;
			AppointmentContentLayoutCalculator contentCalculator = CreateAppointmentContentLayoutCalculator(painter);
			return new WorkWeekViewTimeCellAppointmentLayoutCalculator((WorkWeekViewInfo)ViewInfo, contentCalculator, cache, painter);
		}
	}
	#endregion
}
