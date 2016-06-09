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

using DevExpress.Web.ASPxScheduler.Drawing;
using DevExpress.Web.ASPxScheduler.Internal;
using DevExpress.Web.ASPxScheduler.Rendering;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.UI;
namespace DevExpress.Web.ASPxScheduler {
	public class FullWeekView : WeekViewBase {
		public FullWeekView(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		#region Enabled
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("SchedulerViewBaseEnabled"),
#endif
 DefaultValue(false), NotifyParentProperty(true), AutoFormatEnable()]
		public new bool Enabled {
			get {
				return base.Enabled;
			}
			set {
				base.Enabled = value;				
			}
		}
		#endregion
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override SchedulerViewType Type { get { return SchedulerViewType.FullWeek; } }
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("FullWeekViewStyles"),
#endif
		Category("Styles"), NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new FullWeekViewStyles Styles { get { return (FullWeekViewStyles)InnerStyles; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FullWeekViewStyles FullWeekViewStyles { get { return this.Styles; } }
		#endregion
		protected internal override ASPxSchedulerStylesBase CreateStyles() {
			return new FullWeekViewStyles(Control);
		}
		protected internal override XtraScheduler.Native.InnerSchedulerViewBase CreateInnerView() {
			return new InnerFullWeekView(this, new FullWeekViewProperties());
		}
		protected internal override ViewFactoryHelper CreateFactoryHelper() {
			return new FullWeekViewFactoryHelper();
		}
		public override void Assign(SchedulerViewBase source) {
			base.Assign(source);
			FullWeekView fullWeekView = source as FullWeekView;
			if (fullWeekView != null) 
				FullWeekViewStyles.CopyFrom(fullWeekView.FullWeekViewStyles);
		}
	}
}
namespace DevExpress.Web.ASPxScheduler {
	public class FullWeekViewFactoryHelper : DayViewFactoryHelper {
		public override ViewGroupTypeStrategy CreateGroupByNoneStrategy() {
			return new FullWeekViewGroupByNoneStrategy();
		}
		public override ViewGroupTypeStrategy CreateGroupByDateStrategy() {
			return new FullWeekViewGroupByDateStrategy();
		}
		public override ViewGroupTypeStrategy CreateGroupByResourceStrategy() {
			return new FullWeekViewGroupByResourceStrategy();
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
	public class FullWeekViewGroupByNoneStrategy : WorkWeekViewGroupByNoneStrategy {
		public override ISchedulerWebViewInfoBase CreateWebViewInfo(SchedulerViewBase view) {
			return new WebDayViewGroupByNone((FullWeekView)view);
		}
	}
	public class FullWeekViewGroupByDateStrategy : WorkWeekViewGroupByDateStrategy {
		public override ISchedulerWebViewInfoBase CreateWebViewInfo(SchedulerViewBase view) {
			return new WebDayViewGroupByDate((FullWeekView)view);
		}
	}
	public class FullWeekViewGroupByResourceStrategy : WorkWeekViewGroupByResourceStrategy {
		public override ISchedulerWebViewInfoBase CreateWebViewInfo(SchedulerViewBase view) {
			return new WebDayViewGroupByResource((FullWeekView)view);
		}
	}
}
