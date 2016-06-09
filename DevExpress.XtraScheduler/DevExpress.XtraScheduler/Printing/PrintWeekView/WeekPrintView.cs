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
using DevExpress.XtraScheduler.Printing.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Printing {
	public class WeekPrintView : WeekView {
		ViewPart part;
		GraphicsInfo gInfo;
		PrintColorConverter colorConverter;
		Font headingsFont;
		Font appointmentFont;
		public WeekPrintView(SchedulerControl control, ViewPart part, GraphicsInfo gInfo, SchedulerPrintStyle printStyle)
			: base(control) {
			if (gInfo == null)
				Exceptions.ThrowArgumentNullException("gInfo");
			if (printStyle == null)
				Exceptions.ThrowArgumentNullException("printStyle");
			this.part = part;
			this.gInfo = gInfo;
			this.colorConverter = printStyle.ColorConverter;
			this.headingsFont = (Font)printStyle.HeadingsFont.Clone();
			this.appointmentFont = (Font)printStyle.AppointmentFont.Clone();
		}
		protected internal override bool HeaderAlternateEnabled { get { return false; } }
		internal ViewPart ViewPart { get { return part; } }
		internal Font HeadingsFont { get { return headingsFont; } }
		internal Font AppointmentFont { get { return appointmentFont; } }
		internal PrintColorConverter ColorConverter { get { return colorConverter; } }
		protected internal override bool DrawMoreButtonsOverAppointments { get { return false; } }
		protected internal override ViewPainterBase CreatePainter(SchedulerPaintStyle paintStyle) {
			WeekPrintViewPainter painter = new WeekPrintViewPainter(colorConverter);
			painter.Initialize();
			return painter;
		}
		protected internal override SchedulerViewInfoBase CreateViewInfoCore() {
			return new WeekPrintViewInfo(this, part, gInfo);
		}
		protected internal override BaseViewAppearance CreateAppearance() {
			WeekViewAppearance appearance = (WeekViewAppearance)base.CreateAppearance();
			appearance.HeaderCaption.Font = headingsFont;
			appearance.CellHeaderCaption.Font = headingsFont;
			appearance.Appointment.Font = appointmentFont;
			appearance.Appointment.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
			return appearance;
		}
		protected override void NotifyControlChanges(SchedulerControlChangeType change) {
		}
	}
}
