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
using System.Web.UI;
using DevExpress.Web.ASPxScheduler.Drawing;
using DevExpress.Web.ASPxScheduler.Internal;
using DevExpress.Web.ASPxScheduler.Rendering;
using DevExpress.Utils.Serializing;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
namespace DevExpress.Web.ASPxScheduler {
	#region WeekView
	public class WeekView : SchedulerViewBase {
		ASPxSchedulerOptionsCellAutoHeight cellAutoHeight;
		public WeekView(ASPxScheduler control)
			: base(control) {
				this.cellAutoHeight = new ASPxSchedulerOptionsCellAutoHeight();
		}
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override SchedulerViewType Type { get { return SchedulerViewType.Week; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal virtual DayOfWeek FirstDayOfWeek { get { return Control.FirstDayOfWeek; } }
		protected internal virtual bool DrawMoreButtonsOverAppointments { get { return true; } }
		protected internal new InnerWeekView InnerView { get { return (InnerWeekView)base.InnerView; } }
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("WeekViewAppointmentDisplayOptions"),
#endif
		Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
		public WeekViewAppointmentDisplayOptions AppointmentDisplayOptions { get { return (WeekViewAppointmentDisplayOptions)base.AppointmentDisplayOptionsInternal; } }
		#region Styles
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("WeekViewStyles"),
#endif
Category("Styles"), NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public WeekViewStyles Styles { get { return (WeekViewStyles)InnerStyles; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual WeekViewStyles WeekViewStyles { get { return this.Styles; } }
		#endregion
		#region Templates
		[
		Browsable(false),
		PersistenceMode(PersistenceMode.InnerProperty),
		NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public WeekViewTemplates Templates { get { return (WeekViewTemplates)InnerTemplates; } }
		#endregion
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("WeekViewCellAutoHeightOptions"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
		public ASPxSchedulerOptionsCellAutoHeight CellAutoHeightOptions { get { return cellAutoHeight; } }
		#endregion
		public override void Assign(SchedulerViewBase source) {
			base.Assign(source);
			WeekView weekView = source as WeekView;
			if (weekView != null) {
				Styles.CopyFrom(weekView.Styles);
				WeekViewStyles.CopyFrom(weekView.WeekViewStyles);
				AppointmentDisplayOptions.Assign(weekView.AppointmentDisplayOptions);
				CellAutoHeightOptions.Assign(weekView.CellAutoHeightOptions);
			}
		}
		protected internal override InnerSchedulerViewBase CreateInnerView() {
			return new InnerWeekView(this, new WeekViewProperties());
		}
		protected internal override AppointmentDisplayOptions CreateAppointmentDisplayOptionsCore() {
			return new WeekViewAppointmentDisplayOptions();
		}
		protected internal override ViewFactoryHelper CreateFactoryHelper() {
			return new WeekViewFactoryHelper();
		}
		protected internal override ASPxSchedulerStylesBase CreateStyles() {
			return new WeekViewStyles(Control);
		}
		protected internal override SchedulerTemplates CreateTemplates() {
			return new WeekViewTemplates();
		}
	}
	#endregion
}
namespace DevExpress.Web.ASPxScheduler.Internal {
	#region WeekViewFactoryHelper
	public class WeekViewFactoryHelper : ViewFactoryHelper {
		public override ViewGroupTypeStrategy CreateGroupByNoneStrategy() {
			return new WeekViewGroupByNoneStrategy();
		}
		public override ViewGroupTypeStrategy CreateGroupByDateStrategy() {
			return new WeekViewGroupByDateStrategy();
		}
		public override ViewGroupTypeStrategy CreateGroupByResourceStrategy() {
			return new WeekViewGroupByResourceStrategy();
		}
		public override AppointmentBaseLayoutCalculator CreateAppointmentLayoutCalculator(ISchedulerWebViewInfoBase viewInfo, AppointmentContentLayoutCalculator contentCalculator, bool alternate) {
			return new WeekViewAppointmentLayoutCalculator(viewInfo, contentCalculator);
		}
		public override AppointmentContentLayoutCalculator CreateAppointmentContentLayoutCalculator(ISchedulerWebViewInfoBase viewInfo, bool alternate) {
			return new WeekViewApointmnetContentLayoutCalculator(viewInfo);
		}
	}
	#endregion
	#region WeekViewGroupByNoneStrategy
	public class WeekViewGroupByNoneStrategy : ViewGroupTypeStrategy {
		public override IContinuousCellsInfosCalculator CreateVisuallyContinuousCellsInfosCalculator(bool alternate) {
			return new WeekViewGroupByNoneVisuallyContinuousCellsInfosCalculator();
		}
		public override ISchedulerWebViewInfoBase CreateWebViewInfo(SchedulerViewBase view) {
			return new WebWeekViewGroupByNone((WeekView)view);
		}
	}
	#endregion
	#region WeekViewGroupByDateStrategy
	public class WeekViewGroupByDateStrategy : ViewGroupTypeStrategy {
		public override IContinuousCellsInfosCalculator CreateVisuallyContinuousCellsInfosCalculator(bool alternate) {
			return new WeekViewGroupByDateVisuallyContinuousCellsInfosCalculator();
		}
		public override ISchedulerWebViewInfoBase CreateWebViewInfo(SchedulerViewBase view) {
			return new WebWeekViewGroupByDate((WeekView)view);
		}
	}
	#endregion
	#region WeekViewGroupByResourceStrategy
	public class WeekViewGroupByResourceStrategy : ViewGroupTypeStrategy {
		public override IContinuousCellsInfosCalculator CreateVisuallyContinuousCellsInfosCalculator(bool alternate) {
			return new WeekViewGroupByResourceVisuallyContinuousCellsInfosCalculator();
		}
		public override ISchedulerWebViewInfoBase CreateWebViewInfo(SchedulerViewBase view) {
			return new WebWeekViewGroupByResource((WeekView)view);
		}
	}
	#endregion
}
