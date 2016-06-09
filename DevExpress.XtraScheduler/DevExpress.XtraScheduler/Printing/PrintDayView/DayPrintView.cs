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

using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Printing {
	public class DayPrintView : DayView {
		#region Fields
		bool showExtendedCells;
		GraphicsInfo gInfo;
		PrintColorConverter colorConverter;
		Font headingsFont;
		Font appointmentFont;
		TextOptions appointmentTextOptions;
		#endregion
		public DayPrintView(SchedulerControl control, GraphicsInfo gInfo, SchedulerPrintStyle printStyle)
			: base(control) {
			if (gInfo == null)
				Exceptions.ThrowArgumentNullException("gInfo");
			if (printStyle == null)
				Exceptions.ThrowArgumentNullException("printStyle");
			this.gInfo = gInfo;
			this.colorConverter = (PrintColorConverter)printStyle.ColorConverter.Clone();
			this.headingsFont = (Font)printStyle.HeadingsFont.Clone();
			this.appointmentFont = (Font)printStyle.AppointmentFont.Clone();
			DailyPrintStyle dailyPrintStyle = printStyle as DailyPrintStyle;
			WeeklyPrintStyle weeklyPrintStyle = printStyle as WeeklyPrintStyle;
			showExtendedCells = dailyPrintStyle != null ? dailyPrintStyle.PrintAllAppointments : (weeklyPrintStyle != null ? weeklyPrintStyle.PrintAllAppointments : true);
			appointmentTextOptions = Control.DayView.Appearance.Appointment.TextOptions;
		}
		#region Properties
		protected internal override bool ShowExtendedCells { get { return showExtendedCells; } }
		protected internal override bool HeaderAlternateEnabled { get { return false; } }
		internal Font HeadingsFont { get { return headingsFont; } }
		internal Font AppointmentFont { get { return appointmentFont; } }
		public new DayPrintViewInfo ViewInfo { get { return (DayPrintViewInfo)base.ViewInfo; } }
		public PrintColorConverter ColorConverter { get { return colorConverter; } }
		#endregion
		protected internal override void Initialize() {
			base.Initialize();
			DayViewAppointmentDisplayOptions appointmentDisplayOptions = (DayViewAppointmentDisplayOptions)AppointmentDisplayOptions;
			appointmentDisplayOptions.ShowShadows = false;
			this.TimeSlots.BeginUpdate();
			this.TimeSlots.Clear();
			this.TimeSlots.AddRange(GetActiveStyleTimeSlots());
			this.TimeSlots.EndUpdate();
		}
		internal TimeSlotCollection GetActiveStyleTimeSlots() {
			DailyPrintStyle dailyPrintStyle = this.Control.ActivePrintStyle as DailyPrintStyle;
			WeeklyPrintStyle weeklyPrintStyle = this.Control.ActivePrintStyle as WeeklyPrintStyle;
			return dailyPrintStyle != null ? dailyPrintStyle.TimeSlots : (weeklyPrintStyle != null ? weeklyPrintStyle.TimeSlots : null);
		}
		protected internal override ViewPainterBase CreatePainter(SchedulerPaintStyle paintStyle) {
			DayPrintViewPainter painter = new DayPrintViewPainter(colorConverter);
			painter.Initialize();
			return painter;
		}
		protected internal override SchedulerViewInfoBase CreateViewInfoCore() {
			return new DayPrintViewInfo(this, gInfo);
		}
		protected internal override BaseViewAppearance CreateAppearance() {
			DayViewAppearance appearance = (DayViewAppearance)base.CreateAppearance();
			appearance.TimeRuler.Font = headingsFont;
			appearance.Appointment.Font = appointmentFont;
			appearance.Appointment.TextOptions.Assign(appointmentTextOptions);
			return appearance;
		}
		protected override void NotifyControlChanges(SchedulerControlChangeType change) {
		}
	}
}
