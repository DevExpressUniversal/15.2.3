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
using System.Drawing;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.XtraEditors;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Services;
using DevExpress.XtraScheduler.Services.Internal;
namespace DevExpress.XtraScheduler {
	public class WeekView : SchedulerViewBase {
		public WeekView(SchedulerControl control)
			: base(control) {
		}
		#region Properties
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("WeekViewAppearance"),
#endif
 Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new WeekViewAppearance Appearance { get { return (WeekViewAppearance)base.Appearance; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override SchedulerViewType Type { get { return SchedulerViewType.Week; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal virtual DayOfWeek FirstDayOfWeek { get { return Control.FirstDayOfWeek; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new WeekViewInfo ViewInfo { get { return (WeekViewInfo)base.ViewInfo; } }
		#region MenuItemId
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override SchedulerMenuItemId MenuItemId { get { return SchedulerMenuItemId.SwitchToWeekView; } }
		#endregion
		#region AppointmentDisplayOptions
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("WeekViewAppointmentDisplayOptions"),
#endif
 Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public new WeekViewAppointmentDisplayOptions AppointmentDisplayOptions { get { return (WeekViewAppointmentDisplayOptions)base.AppointmentDisplayOptions; } }
		#endregion
		#region DeferredScrolling
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("WeekViewDeferredScrolling"),
#endif
		XtraSerializableProperty()]
		public SchedulerDeferredScrollingOption DeferredScrolling { get { return (SchedulerDeferredScrollingOption)InnerView.DeferredScrolling; } }
		#endregion
		#endregion
		protected internal override InnerSchedulerViewBase CreateInnerView() {
			return new InnerWeekView(this, new WeekViewProperties());
		}
		protected internal override ViewDateTimeScrollController CreateDateTimeScrollController() {
			return new WeekViewDateTimeScrollController(this);
		}
		protected internal override BaseViewAppearance CreateAppearance() {
			return new WeekViewAppearance();
		}
		protected internal override ViewPainterBase CreatePainter(SchedulerPaintStyle paintStyle) {
			if (paintStyle == null)
				Exceptions.ThrowArgumentException("paintStyle", paintStyle);
			return paintStyle.CreateWeekViewPainter();
		}
		protected internal override AppointmentDisplayOptions CreateAppointmentDisplayOptionsCore() {
			return new WeekViewAppointmentDisplayOptions();
		}
		protected internal override ViewFactoryHelper CreateFactoryHelper() {
			return new WeekViewFactoryHelper();
		}
		protected internal override SchedulerViewInfoBase CreateViewInfoCore() {
			return new WeekViewInfo(this);
		}
		protected internal override void InitializeViewInfo(SchedulerViewInfoBase viewInfo) {
			WeekViewInfo weekViewInfo = (WeekViewInfo)viewInfo;
			weekViewInfo.FirstDayOfWeek = this.FirstDayOfWeek;
			InnerWeekView innerView = (InnerWeekView)this.InnerView;
			weekViewInfo.CompressWeekend = innerView.CompressWeekendInternal;
		}
		protected internal override bool ChangeResourceScrollBarOrientationIfNeeded(ResourceNavigator navigator) {
			bool oldVertical = navigator.Vertical;
			bool newVertical = CanShowResources() && GroupType == SchedulerGroupType.Date;
			navigator.Vertical = newVertical;
			return oldVertical != newVertical;
		}
		protected internal override bool ChangeDateTimeScrollBarOrientationIfNeeded(DateTimeScrollBar scrollBar) {
			ScrollBarType oldScrollBarType = scrollBar.ScrollBarType;
			ScrollBarType newScrollBarType;
			if (CanShowResources() && GroupType == SchedulerGroupType.Date)
				newScrollBarType = ScrollBarType.Horizontal;
			else
				newScrollBarType = ScrollBarType.Vertical;
			scrollBar.ScrollBarType = newScrollBarType;
			return newScrollBarType != oldScrollBarType;
		}
	}
}
