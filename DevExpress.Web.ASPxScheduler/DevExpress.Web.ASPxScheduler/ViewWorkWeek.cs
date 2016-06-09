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
using System.ComponentModel;
using DevExpress.Utils.Serializing;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using DevExpress.Web.ASPxScheduler.Drawing;
using DevExpress.Web.ASPxScheduler.Internal;
using DevExpress.Web.ASPxScheduler.Rendering;
using System.Web.UI;
namespace DevExpress.Web.ASPxScheduler.Internal {
	public abstract class WeekViewBase : DayView {
		protected WeekViewBase(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		#region Styles
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DayViewStyles DayViewStyles { get { return base.DayViewStyles; } }
		#endregion
		#endregion
		protected internal override SchedulerTemplates CreateTemplates() {
			return new WorkWeekViewTemplates();
		}
		public override void Assign(SchedulerViewBase source) {
			base.Assign(source);
			WeekViewBase workWeekView = source as WeekViewBase;
			if (workWeekView != null) {
				Styles.CopyFrom(workWeekView.Styles);				
			}
		}
	}
}
namespace DevExpress.Web.ASPxScheduler {
	#region WorkWeekView
	public class WorkWeekView : WeekViewBase {
		public WorkWeekView(ASPxScheduler control) : base(control) {
		}
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override SchedulerViewType Type { get { return SchedulerViewType.WorkWeek; } }
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("WorkWeekViewShowFullWeek"),
#endif
		DefaultValue(InnerWorkWeekView.DefaultShowFullWeek), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatEnable()]
		public virtual bool ShowFullWeek {
			get {
				if(InnerView != null) {
					InnerWorkWeekView innerView = (InnerWorkWeekView)InnerView;
					return innerView.ShowFullWeek;
				}
				else
					return false;
			}
			set {
				if(InnerView != null) {
					InnerWorkWeekView innerView = (InnerWorkWeekView)InnerView;
					innerView.ShowFullWeek = value;
				}
			}
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("WorkWeekViewStyles"),
#endif
		Category("Styles"), NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new WorkWeekViewStyles Styles { get { return (WorkWeekViewStyles)InnerStyles; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public WorkWeekViewStyles WorkWeekViewStyles { get { return this.Styles; } }
		#endregion
		protected internal override ViewFactoryHelper CreateFactoryHelper() {
			return new WorkWeekViewFactoryHelper();
		}
		protected internal override InnerSchedulerViewBase CreateInnerView() {
			return new InnerWorkWeekView(this, new WorkWeekViewProperties());
		}
		protected internal override ASPxSchedulerStylesBase CreateStyles() {
			return new WorkWeekViewStyles(Control);
		}
		public override void Assign(SchedulerViewBase source) {
			base.Assign(source);
			WorkWeekView workWeekView = source as WorkWeekView;
			if (workWeekView != null) {
				WorkWeekViewStyles.CopyFrom(workWeekView.WorkWeekViewStyles);
				ShowFullWeek = workWeekView.ShowFullWeek;
			}
		}
	}
	#endregion
}
namespace DevExpress.Web.ASPxScheduler.Internal {
	#region WorkWeekViewFactoryHelper
	public class WorkWeekViewFactoryHelper : DayViewFactoryHelper {
		public override ViewGroupTypeStrategy CreateGroupByNoneStrategy() {
			return new WorkWeekViewGroupByNoneStrategy();
		}
		public override ViewGroupTypeStrategy CreateGroupByDateStrategy() {
			return new WorkWeekViewGroupByDateStrategy();
		}
		public override ViewGroupTypeStrategy CreateGroupByResourceStrategy() {
			return new WorkWeekViewGroupByResourceStrategy();
		}
		public override AppointmentBaseLayoutCalculator CreateAppointmentLayoutCalculator(ISchedulerWebViewInfoBase viewInfo, AppointmentContentLayoutCalculator contentCalculator, bool alternate) {
			if (alternate)
				return new WorkWeekViewLongAppointmentLayoutCalculator(viewInfo, contentCalculator);
			else
				return new WorkWeekViewShortAppointmentLayoutCalculator(viewInfo, contentCalculator);		
		}		
		public override AppointmentContentLayoutCalculator CreateAppointmentContentLayoutCalculator(ISchedulerWebViewInfoBase viewInfo, bool alternate) {
			if (alternate)
				return new DayViewLongAppointmentContentLayoutCalculator(viewInfo);
			else
				return new DayViewShortAppointmentContentLayoutCalculator(viewInfo);
		}	   
	}
	#endregion
	#region WorkWeekViewGroupByNoneStrategy
	public class WorkWeekViewGroupByNoneStrategy : ViewGroupTypeStrategy {
		public override IContinuousCellsInfosCalculator CreateVisuallyContinuousCellsInfosCalculator(bool alternate) {
			if(alternate)
				return new WorkWeekViewGroupByNoneAllDayAreaVisuallyContinuousCellsInfosCalculator();
			else
				return new WorkWeekViewGroupByNoneVisuallyContinuousCellsInfosCalculator();
		}
		public override ISchedulerWebViewInfoBase CreateWebViewInfo(SchedulerViewBase view) {
			return new WebWorkWeekViewGroupByNone((WorkWeekView)view);
		}
	}
	#endregion
	#region WorkWeekViewGroupByDateStrategy
	public class WorkWeekViewGroupByDateStrategy : ViewGroupTypeStrategy {
		public override IContinuousCellsInfosCalculator CreateVisuallyContinuousCellsInfosCalculator(bool alternate) {
			if(alternate)
				return new WorkWeekViewGroupByDateAllDayAreaVisuallyContinuousCellsInfosCalculator();
			else
				return new WorkWeekViewGroupByDateVisuallyContinuousCellsInfosCalculator();
		}
		public override ISchedulerWebViewInfoBase CreateWebViewInfo(SchedulerViewBase view) {
			return new WebWorkWeekViewGroupByDate((WorkWeekView)view);
		}
	}
	#endregion
	#region WorkWeekViewGroupByResourceStrategy
	public class WorkWeekViewGroupByResourceStrategy : ViewGroupTypeStrategy {
		public override IContinuousCellsInfosCalculator CreateVisuallyContinuousCellsInfosCalculator(bool alternate) {
			if(alternate)
				return new WorkWeekViewGroupByResourceAllDayAreaVisuallyContinuousCellsInfosCalculator();
			else
				return new WorkWeekViewGroupByResourceVisuallyContinuousCellsInfosCalculator();
		}
		public override ISchedulerWebViewInfoBase CreateWebViewInfo(SchedulerViewBase view) {
			return new WebWorkWeekViewGroupByResource((WorkWeekView)view);
		}
	}
	#endregion
}
