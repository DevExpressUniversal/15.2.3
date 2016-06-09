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
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.XtraScheduler {
	#region MonthView
	public class MonthView : WeekView {
		#region Fields
		#endregion
		public MonthView(SchedulerControl control)
			: base(control) {
		}
		#region Properties
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("MonthViewAppearance"),
#endif
Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new MonthViewAppearance Appearance { get { return (MonthViewAppearance)base.Appearance; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override SchedulerViewType Type { get { return SchedulerViewType.Month; } }
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("MonthViewWeekCount"),
#endif
DefaultValue(InnerMonthView.defaultWeekCount), XtraSerializableProperty()]
		public int WeekCount {
			get {
				if (InnerView != null) {
					InnerMonthView innerView = (InnerMonthView)InnerView;
					return innerView.WeekCount;
				}
				else
					return 0;
			}
			set {
				if (InnerView != null) {
					InnerMonthView innerView = (InnerMonthView)InnerView;
					innerView.WeekCount = value;
				}
			}
		}
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("MonthViewCompressWeekend"),
#endif
DefaultValue(InnerMonthView.defaultCompressWeekend), XtraSerializableProperty()]
		public bool CompressWeekend {
			get {
				if (InnerView != null) {
					InnerMonthView innerView = (InnerMonthView)InnerView;
					return innerView.CompressWeekend;
				}
				else
					return false;
			}
			set {
				if (InnerView != null) {
					InnerMonthView innerView = (InnerMonthView)InnerView;
					innerView.CompressWeekend = value;
				}
			}
		}
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("MonthViewShowWeekend"),
#endif
DefaultValue(InnerMonthView.defaultShowWeekend), XtraSerializableProperty()]
		public bool ShowWeekend {
			get {
				if (InnerView != null) {
					InnerMonthView innerView = (InnerMonthView)InnerView;
					return innerView.ShowWeekend;
				}
				else
					return false;
			}
			set {
				if (InnerView != null) {
					InnerMonthView innerView = (InnerMonthView)InnerView;
					innerView.ShowWeekend = value;
				}
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override SchedulerMenuItemId MenuItemId { get { return SchedulerMenuItemId.SwitchToMonthView; } }
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("MonthViewAppointmentDisplayOptions"),
#endif
Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public new MonthViewAppointmentDisplayOptions AppointmentDisplayOptions { get { return (MonthViewAppointmentDisplayOptions)base.AppointmentDisplayOptions; } }
		#endregion
		protected internal override InnerSchedulerViewBase CreateInnerView() {
			return new InnerMonthView(this, new MonthViewProperties());
		}
		protected internal override ViewDateTimeScrollController CreateDateTimeScrollController() {
			return new MonthViewDateTimeScrollController(this);
		}
		protected internal override BaseViewAppearance CreateAppearance() {
			return new MonthViewAppearance();
		}
		protected internal override AppointmentDisplayOptions CreateAppointmentDisplayOptionsCore() {
			return new MonthViewAppointmentDisplayOptions();
		}
		protected internal override ViewPainterBase CreatePainter(SchedulerPaintStyle paintStyle) {
			if (paintStyle == null)
				Exceptions.ThrowArgumentException("paintStyle", paintStyle);
			return paintStyle.CreateMonthViewPainter();
		}
		protected internal override ViewFactoryHelper CreateFactoryHelper() {
			return new MonthViewFactoryHelper();
		}
		protected internal override SchedulerViewInfoBase CreateViewInfoCore() {
			return new MonthViewInfo(this);
		}
		protected internal override void InitializeViewInfo(SchedulerViewInfoBase viewInfo) {
			base.InitializeViewInfo(viewInfo);
			MonthViewInfo monthViewInfo = (MonthViewInfo)viewInfo;
			monthViewInfo.ShowWeekend = this.ShowWeekend;
		}
	}
	#endregion
}
