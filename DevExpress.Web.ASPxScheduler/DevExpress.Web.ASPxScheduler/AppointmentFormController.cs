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
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.UI;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.Web.ASPxScheduler.Internal {
	public class AppointmentFormController : AppointmentFormControllerBase {
		ASPxScheduler control;
		public AppointmentFormController(ASPxScheduler control, Appointment apt)
			: base(control.InnerControl, apt) {
			if (control == null)
				Exceptions.ThrowArgumentException("control", control);
			this.control = control;
			if (EditedPattern != null) {
				IRecurrenceInfo info = EditedPattern.RecurrenceInfo;
				UpdateRecurrenceInfoRange(info.Start, info.End, info.Range, info.OccurrenceCount);
			}
		}
		protected internal ASPxScheduler Control { get { return control; } }
		protected internal ASPxAppointmentStorage Appointments { get { return (ASPxAppointmentStorage)InnerAppointments; } }
		protected internal bool ShouldShowRecurrence {
			get { return !SourceAppointment.IsOccurrence && ShouldShowRecurrenceButton; }
		}
		protected internal virtual void UpdateRecurrenceInfoRange(DateTime start, DateTime end, RecurrenceRange rangeType, int occurrencesCount) {
			IInternalRecurrenceInfo internalRecurrenceInfo = EditedPattern.RecurrenceInfo as IInternalRecurrenceInfo;
			internalRecurrenceInfo.UpdateRange(start, end, rangeType, occurrencesCount, EditedPattern);
		}
	}
}
