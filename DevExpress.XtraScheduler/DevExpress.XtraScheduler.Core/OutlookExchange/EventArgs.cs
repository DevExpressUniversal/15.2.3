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
using DevExpress.XtraScheduler.Outlook.Interop;
namespace DevExpress.XtraScheduler.Outlook {
	#region OutlookAppointmentImportingEventArgs
	[CLSCompliant(false)]
	public class OutlookAppointmentImportingEventArgs : AppointmentImportingEventArgs {
		_AppointmentItem olApt;
		public OutlookAppointmentImportingEventArgs(Appointment apt, _AppointmentItem olApt)
			: base(apt) {
			if (olApt == null)
				DevExpress.XtraScheduler.Native.Exceptions.ThrowArgumentException("olApt", olApt);
			this.olApt = olApt;
		}
		public _AppointmentItem OutlookAppointment { get { return olApt; } }
	}
	#endregion
	#region OutlookAppointmentImportedEventArgs
	[CLSCompliant(false)]
	public class OutlookAppointmentImportedEventArgs : AppointmentImportedEventArgs {
		_AppointmentItem olApt;
		public OutlookAppointmentImportedEventArgs(Appointment apt, _AppointmentItem olApt)
			: base(apt) {
			if (olApt == null)
				DevExpress.XtraScheduler.Native.Exceptions.ThrowArgumentException("olApt", olApt);
			this.olApt = olApt;
		}
		public _AppointmentItem OutlookAppointment { get { return olApt; } }
	}
	#endregion
	#region OutlookAppointmentExportingEventArgs
	[CLSCompliant(false)]
	public class OutlookAppointmentExportingEventArgs : AppointmentExportingEventArgs {
		_AppointmentItem olApt;
		public OutlookAppointmentExportingEventArgs(Appointment apt, _AppointmentItem olApt)
			: base(apt) {
			if (olApt == null)
				DevExpress.XtraScheduler.Native.Exceptions.ThrowArgumentException("olApt", olApt);
			this.olApt = olApt;
		}
		public _AppointmentItem OutlookAppointment { get { return olApt; } }
	}
	#endregion
	#region OutlookAppointmentExportedEventArgs
	[CLSCompliant(false)]
	public class OutlookAppointmentExportedEventArgs : AppointmentExportedEventArgs {
		_AppointmentItem olApt;
		public OutlookAppointmentExportedEventArgs(Appointment apt, _AppointmentItem olApt)
			: base(apt) {
			if (olApt == null)
				DevExpress.XtraScheduler.Native.Exceptions.ThrowArgumentException("olApt", olApt);
			this.olApt = olApt;
		}
		public _AppointmentItem OutlookAppointment { get { return olApt; } }
	}
	#endregion
	#region OutlookAppointmentSynchronizingEventArgs
	[CLSCompliant(false)]
	public class OutlookAppointmentSynchronizingEventArgs : AppointmentSynchronizingEventArgs {
		_AppointmentItem olApt;
		public OutlookAppointmentSynchronizingEventArgs(_AppointmentItem olApt) {
			this.olApt = olApt;
		}
		public OutlookAppointmentSynchronizingEventArgs(Appointment apt, _AppointmentItem olApt)
			: base(apt) {
			this.olApt = olApt;
		}
		public _AppointmentItem OutlookAppointment { get { return olApt; } }
	}
	#endregion
	#region OutlookAppointmentSynchronizedEventArgs
	[CLSCompliant(false)]
	public class OutlookAppointmentSynchronizedEventArgs : AppointmentSynchronizedEventArgs {
		_AppointmentItem olApt;
		public OutlookAppointmentSynchronizedEventArgs(_AppointmentItem olApt) {
			this.olApt = olApt;
		}
		public OutlookAppointmentSynchronizedEventArgs(Appointment apt, _AppointmentItem olApt)
			: base(apt) {
			this.olApt = olApt;
		}
		public _AppointmentItem OutlookAppointment { get { return olApt; } }
	}
	#endregion
	[CLSCompliant(false)]
	public class OutlookExchangeExceptionEventArgs : ExchangeExceptionEventArgs {
		Appointment apt;
		_AppointmentItem olApt;
		public OutlookExchangeExceptionEventArgs(System.Exception originalException, Appointment apt, _AppointmentItem olApt)
			: base(originalException) {
			this.olApt = olApt;
			this.apt = apt;
		}
		public _AppointmentItem OutlookAppointment { get { return olApt; } }
		public Appointment Appointment { get { return apt; } }
	}
}
