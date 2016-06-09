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
using System.Collections.Generic;
using System.Text;
using DevExpress.XtraScheduler.iCalendar;
using DevExpress.XtraScheduler.iCalendar.Components;
using DevExpress.XtraScheduler.Localization;
using System.ComponentModel;
namespace DevExpress.XtraScheduler.iCalendar {
	#region iCalendarStructureCreatedEventHandler
	public delegate void iCalendarStructureCreatedEventHandler(object sender, iCalendarStructureCreatedEventArgs e);
	#endregion
	#region iCalendarStructureCreatedEventArgs
	public class iCalendarStructureCreatedEventArgs : EventArgs {
		iCalendarContainer calendars;
		public iCalendarStructureCreatedEventArgs(iCalendarContainer calendars) {
			if (calendars == null)
				DevExpress.XtraScheduler.Native.Exceptions.ThrowArgumentNullException("calendars");
			this.calendars = calendars;
		}
		public iCalendarContainer Calendars { get { return calendars; } }
	}
	#endregion
	#region iCalendarAppointmentExportingEventArgs
	public class iCalendarAppointmentExportingEventArgs : AppointmentExportingEventArgs {
		VEvent ev;
		public iCalendarAppointmentExportingEventArgs(Appointment apt, VEvent ev)
			: base(apt) {
			if (ev == null)
				DevExpress.XtraScheduler.Native.Exceptions.ThrowArgumentException("ev", ev);
			this.ev = ev;
		}
		public VEvent VEvent { get { return ev; } }
	}
	#endregion
	#region iCalendarAppointmentExportedEventArgs
	public class iCalendarAppointmentExportedEventArgs : AppointmentExportedEventArgs {
		VEvent ev;
		public iCalendarAppointmentExportedEventArgs(Appointment apt, VEvent ev)
			: base(apt) {
			if (ev == null)
				DevExpress.XtraScheduler.Native.Exceptions.ThrowArgumentException("ev", ev);
			this.ev = ev;
		}
		public VEvent VEvent { get { return ev; } }
	}
	#endregion
	#region iCalendarAppointmentImportingEventArgs
	public class iCalendarAppointmentImportingEventArgs : AppointmentImportingEventArgs {
		VEvent ev;
		public iCalendarAppointmentImportingEventArgs(Appointment apt, VEvent ev)
			: base(apt) {
			if (ev == null)
				DevExpress.XtraScheduler.Native.Exceptions.ThrowArgumentException("ev", ev);
			this.ev = ev;
		}
		public VEvent VEvent { get { return ev; } }
	}
	#endregion
	#region iCalendarAppointmentImportedEventArgs
	public class iCalendarAppointmentImportedEventArgs : AppointmentImportedEventArgs {
		VEvent ev;
		public iCalendarAppointmentImportedEventArgs(Appointment apt, VEvent ev)
			: base(apt) {
			if (ev == null)
				DevExpress.XtraScheduler.Native.Exceptions.ThrowArgumentException("ev", ev);
			this.ev = ev;
		}
		public VEvent VEvent { get { return ev; } }
	}
	#endregion
	public enum iCalendarErrorType {
		NotValidFile = 1
	}
	#region iCalendarException
	public class iCalendarException : Exception {
		public iCalendarException() {
		}
		public iCalendarException(string message)
			: base(message) {
		}
	}
	#endregion
	#region iCalendarInvalidFileFormatException
	public class iCalendarInvalidFileFormatException : iCalendarException {
		public iCalendarInvalidFileFormatException()
			: base(SchedulerLocalizer.GetString(SchedulerStringId.Msg_iCalendar_NotValidFile)) {
		}
	}
	#endregion
	#region iCalendarEventImportException
	public class iCalendarEventImportException : iCalendarException {
		VEventCollection events;
		public iCalendarEventImportException(VEventCollection events)
			: base(SchedulerLocalizer.GetString(SchedulerStringId.Msg_iCalendar_AppointmentsImportWarning)) {
			this.events = events;
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("iCalendarEventImportExceptionEvents")]
#endif
		public VEventCollection Events { get { return events; } }
	}
	#endregion
	#region iCalendarParseErrorException
	public class iCalendarParseErrorException : iCalendarException {
		public iCalendarParseErrorException() {
		}
	}
	#endregion
	public class iCalendarExchangeExceptionEventArgs : ExchangeExceptionEventArgs {
		VEvent vEvent;
		int index;
		public iCalendarExchangeExceptionEventArgs(System.Exception originalException, VEvent ev, int index)
			: base(originalException) {
			this.vEvent = ev;
			this.index = index;
		}
		public VEvent VEvent { get { return vEvent; } }
		public int Index { get { return index; } }
	}
	public class iCalendarParseExceptionEventArgs : ExchangeExceptionEventArgs {
		int lineIndex;
		string lineText;
		public iCalendarParseExceptionEventArgs(System.Exception originalException, string lineText, int lineIndex)
			: base(originalException) {
			this.lineText = lineText;
			this.lineIndex = lineIndex;
		}
		public int LineIndex { get { return lineIndex; } }
		public string LineText { get { return lineText; } }
	}
}
namespace DevExpress.XtraScheduler.iCalendar.Native {
	#region iCalendarParseErrorEventArgs
	public class iCalendarParseErrorEventArgs : EventArgs {
		System.Exception originalException;
		iCalendarContentLine line;
		public iCalendarParseErrorEventArgs(iCalendarContentLine line, System.Exception originalException) {
			this.originalException = originalException;
			this.line = line;
		}
		public iCalendarContentLine Line { get { return line; } }
		public System.Exception OriginalException { get { return originalException; } }
	}
	#endregion
}
