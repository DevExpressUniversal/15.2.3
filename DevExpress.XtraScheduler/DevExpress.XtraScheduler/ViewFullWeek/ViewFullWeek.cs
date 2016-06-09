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

using DevExpress.Utils.Drawing;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Native;
using System.ComponentModel;
namespace DevExpress.XtraScheduler {
	public class FullWeekView : DayView {
		public FullWeekView(SchedulerControl control)
			: base(control) {
		}
		#region Properties
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("FullWeekViewAppearance"),
#endif
 Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new WorkWeekViewAppearance Appearance { get { return (WorkWeekViewAppearance)base.Appearance; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override SchedulerViewType Type { get { return SchedulerViewType.FullWeek; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override SchedulerMenuItemId MenuItemId { get { return SchedulerMenuItemId.SwitchToFullWeekView; } }
		[DefaultValue(false)]
		public override bool Enabled {
			get { return base.Enabled; }
			set { base.Enabled = value; }
		}
		#endregion
		protected internal override InnerSchedulerViewBase CreateInnerView() {
			return new InnerFullWeekView(this, new FullWeekViewProperties());
		}
		protected internal override ViewDateTimeScrollController CreateDateTimeScrollController() {
			return new FullWeekViewDateTimeScrollController(this);
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
			return new FullWeekViewFactoryHelper();
		}
		protected internal override SchedulerViewInfoBase CreateViewInfoCore() {
			return new FullWeekViewInfo(this);
		}
	}
}
namespace DevExpress.XtraScheduler.Drawing {
	public class FullWeekViewInfo : DayViewInfo {
		public FullWeekViewInfo(FullWeekView view)
			: base(view) {
		}
		#region Properties
		public new FullWeekView View { get { return (FullWeekView)base.View; } }
		#endregion
	}
	#region FullWeekViewFactoryHelper
	public class FullWeekViewFactoryHelper : ViewFactoryHelper {
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
			return new FullWeekViewDispatchAppointmentsLayoutStrategy((FullWeekView)view);
		}
	}
	#endregion
	#region FullWeekViewDispatchAppointmentsLayoutStrategy
	public class FullWeekViewDispatchAppointmentsLayoutStrategy : DayViewDispatchAppointmentsLayoutStrategy {
		public FullWeekViewDispatchAppointmentsLayoutStrategy(FullWeekView view)
			: base(view) {
		}
		public new FullWeekView View { get { return (FullWeekView)base.View; } }
		protected internal override DayViewAllDayAppointmentsLayoutStrategy CreateAllDayLayoutStrategy(SchedulerViewBase view) {
			return new FullWeekViewAllDayAppointmentsLayoutStrategy(view);
		}
		protected internal override DayViewTimeCellAppointmentsLayoutStrategy CreateTimeCellLayoutStrategy(SchedulerViewBase view) {
			return new FullWeekViewTimeCellAppointmentsLayoutStrategy(View);
		}
	}
	#endregion
	#region FullWeekViewTimeCellAppointmentsLayoutStrategy
	public class FullWeekViewTimeCellAppointmentsLayoutStrategy : DayViewTimeCellAppointmentsLayoutStrategy {
		public FullWeekViewTimeCellAppointmentsLayoutStrategy(FullWeekView view)
			: base(view) {
		}
		protected internal override AppointmentContentLayoutCalculator CreateAppointmentContentLayoutCalculator(AppointmentPainter painter) {
			return new FullWeekViewTimeCellAppointmentContentLayoutCalculator((FullWeekViewInfo)ViewInfo, painter);
		}
		protected internal override AppointmentBaseLayoutCalculator CreateAppointmentLayoutCalculator(GraphicsCache cache) {
			AppointmentPainter painter = ((DayViewPainter)ViewInfo.Painter).TimeCellsAppointmentPainter;
			AppointmentContentLayoutCalculator contentCalculator = CreateAppointmentContentLayoutCalculator(painter);
			return new FullWeekViewTimeCellAppointmentLayoutCalculator((FullWeekViewInfo)ViewInfo, contentCalculator, cache, painter);
		}
	}
	#endregion
	#region FullWeekViewAllDayAppointmentsLayoutStrategy
	public class FullWeekViewAllDayAppointmentsLayoutStrategy : DayViewAllDayAppointmentsLayoutStrategy {
		public FullWeekViewAllDayAppointmentsLayoutStrategy(SchedulerViewBase view)
			: base(view) {
		}
		protected internal override AppointmentContentLayoutCalculator CreateAppointmentContentLayoutCalculator() {
			return new WorkWeekViewAllDayAppointmentContentLayoutCalculator((FullWeekViewInfo)ViewInfo, ViewInfo.Painter.AppointmentPainter);
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
}
