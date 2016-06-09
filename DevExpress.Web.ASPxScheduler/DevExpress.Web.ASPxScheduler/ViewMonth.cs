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
namespace DevExpress.Web.ASPxScheduler {
	#region MonthView
	public class MonthView : WeekView {
		public MonthView(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override SchedulerViewType Type { get { return SchedulerViewType.Month; } }
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("MonthViewWeekCount"),
#endif
DefaultValue(InnerMonthView.defaultWeekCount), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatEnable()]
		public int WeekCount {
			get {
				if(InnerView != null) {
					InnerMonthView innerView = (InnerMonthView)InnerView;
					return innerView.WeekCount;
				}
				else
					return 0;
			}
			set {
				if(InnerView != null) {
					InnerMonthView innerView = (InnerMonthView)InnerView;
					innerView.WeekCount = value;
				}
			}
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("MonthViewCompressWeekend"),
#endif
DefaultValue(InnerMonthView.defaultCompressWeekend), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatEnable()]
		public bool CompressWeekend {
			get {
				if(InnerView != null) {
					InnerMonthView innerView = (InnerMonthView)InnerView;
					return innerView.CompressWeekend;
				}
				else
					return false;
			}
			set {
				if(InnerView != null) {
					InnerMonthView innerView = (InnerMonthView)InnerView;
					innerView.CompressWeekend = value;
				}
			}
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("MonthViewShowWeekend"),
#endif
DefaultValue(InnerMonthView.defaultShowWeekend), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatEnable()]
		public bool ShowWeekend {
			get {
				if(InnerView != null) {
					InnerMonthView innerView = (InnerMonthView)InnerView;
					return innerView.ShowWeekend;
				}
				else
					return false;
			}
			set {
				if(InnerView != null) {
					InnerMonthView innerView = (InnerMonthView)InnerView;
					innerView.ShowWeekend = value;
				}
			}
		}
		#region Styles
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("MonthViewStyles"),
#endif
Category("Styles"), NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new MonthViewStyles Styles { get { return (MonthViewStyles)InnerStyles; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override WeekViewStyles WeekViewStyles { get { return base.WeekViewStyles; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public MonthViewStyles MonthViewStyles { get { return this.Styles; } }
		#endregion
		#endregion
		public override void Assign(SchedulerViewBase source) {
			base.Assign(source);
			MonthView monthView = source as MonthView;
			if (monthView != null) {
				WeekCount = monthView.WeekCount;
				CompressWeekend = monthView.CompressWeekend;
				ShowWeekend = monthView.ShowWeekend;
			}
		}
		protected internal override InnerSchedulerViewBase CreateInnerView() {
			return new InnerMonthView(this, new MonthViewProperties());
		}
		protected internal override AppointmentDisplayOptions CreateAppointmentDisplayOptionsCore() {
			return new MonthViewAppointmentDisplayOptions();
		}
		protected internal override ViewFactoryHelper CreateFactoryHelper() {
			return new MonthViewFactoryHelper();
		}
		protected internal override ASPxSchedulerStylesBase CreateStyles() {
			return new MonthViewStyles(Control);
		}
		protected internal override SchedulerTemplates CreateTemplates() {
			return new MonthViewTemplates();
		}
	}
	#endregion
}
namespace DevExpress.Web.ASPxScheduler.Internal {
	#region MonthViewFactoryHelper
	public class MonthViewFactoryHelper : ViewFactoryHelper {
		public override ViewGroupTypeStrategy CreateGroupByNoneStrategy() {
			return new MonthViewGroupByNoneStrategy();
		}
		public override ViewGroupTypeStrategy CreateGroupByDateStrategy() {
			return new MonthViewGroupByDateStrategy();
		}
		public override ViewGroupTypeStrategy CreateGroupByResourceStrategy() {
			return new MonthViewGroupByResourceStrategy();
		}
		public override AppointmentBaseLayoutCalculator CreateAppointmentLayoutCalculator(ISchedulerWebViewInfoBase viewInfo, AppointmentContentLayoutCalculator contentCalculator, bool alternate) {
			return new MonthViewAppointmentLayoutCalculator(viewInfo, contentCalculator);
		}		
		public override AppointmentContentLayoutCalculator CreateAppointmentContentLayoutCalculator(ISchedulerWebViewInfoBase viewInfo, bool alternate) {
			return new HorizontalAppointmentContentLayoutCalculator(viewInfo);
		}
	}
	#endregion
	#region MonthViewGroupByNoneStrategy
	public class MonthViewGroupByNoneStrategy : ViewGroupTypeStrategy {
		public override IContinuousCellsInfosCalculator CreateVisuallyContinuousCellsInfosCalculator(bool alternate) {
			return new MonthViewGroupByNoneVisuallyContinuousCellsInfosCalculator();
		}
		public override ISchedulerWebViewInfoBase CreateWebViewInfo(SchedulerViewBase view) {
			return new WebMonthViewGroupByNone((MonthView)view);
		}
	}
	#endregion
	#region MonthViewGroupByDateStrategy
	public class MonthViewGroupByDateStrategy : ViewGroupTypeStrategy {
		public override IContinuousCellsInfosCalculator CreateVisuallyContinuousCellsInfosCalculator(bool alternate) {
			return new MonthViewGroupByDateVisuallyContinuousCellsInfosCalculator();
		}
		public override ISchedulerWebViewInfoBase CreateWebViewInfo(SchedulerViewBase view) {
			return new WebMonthViewGroupByDate((MonthView)view);
		}
	}
	#endregion
	#region MonthViewGroupByResourceStrategy
	public class MonthViewGroupByResourceStrategy : ViewGroupTypeStrategy {
		public override IContinuousCellsInfosCalculator CreateVisuallyContinuousCellsInfosCalculator(bool alternate) {
			return new MonthViewGroupByResourceVisuallyContinuousCellsInfosCalculator();
		}
		public override ISchedulerWebViewInfoBase CreateWebViewInfo(SchedulerViewBase view) {			
			return new WebMonthViewGroupByResource((MonthView)view);
		}
	}
	#endregion
}
