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
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.UI;
using DevExpress.XtraScheduler.Localization;
namespace DevExpress.XtraScheduler.Extensions {
	public class AppointmentInfoBuilder {
		public virtual string GetAppointmentInfo(AppointmentFormController controller) {
			if (controller == null)
				return " ";
			string resultString = String.Empty;
			if (controller.EditedPattern != null)
				resultString += GetReccurrenceInfo(controller.EditedPattern, controller.InnerControl);
			string pastInfo = GetPastInfo(controller.EditedAppointmentCopy);
			if (!String.IsNullOrEmpty(pastInfo))
				resultString += pastInfo;
			else
				resultString += GetConflictInfo(controller);
			return resultString;
		}
		protected internal virtual string GetReccurrenceInfo(Appointment apt, InnerSchedulerControl control) {
			RecurrenceDescriptionBuilder recurrenceDescriptor = RecurrenceDescriptionBuilder.CreateInstance(apt, control.TimeZoneHelper, control.FirstDayOfWeek);
			return String.Format(SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.Msg_AppointmentOccurs), recurrenceDescriptor.BuildString());
		}
		protected internal virtual string GetPastInfo(Appointment apt) {
			if (IsAppointmentInPast(apt)) {
				if (apt.Type == AppointmentType.Pattern)
					return SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.Msg_AllOccurrencesInThePast);
				else
					return SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.Msg_AppointmentOccursInThePast);
			}
			else
				return String.Empty;
		}
		protected internal virtual string GetConflictInfo(AppointmentFormController controller) {
			Appointment apt = controller.EditedAppointmentCopy;
			int conflictCount = controller.CalculateConflictCount();
			if (conflictCount > 0) {
				if (apt.Type == AppointmentType.Pattern) {
					string instanceInfo;
					if ((apt.RecurrenceInfo.Range == RecurrenceRange.NoEndDate))
						instanceInfo = SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.Msg_Some);
					else
						instanceInfo = conflictCount.ToString();
					return String.Format(SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.Msg_PatternOccurrencesConflictsWithOtherAppointments), instanceInfo);
				}
				else
					return SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.Msg_AppointmentConflictsWithAnother);
			}
			else
				return String.Empty;
		}
		protected internal virtual bool IsAppointmentInPast(Appointment apt) {
			DateTime today = DateTime.Now;
			return IsAppointmentInPastCore(apt, today);
		}
		protected internal virtual bool IsAppointmentInPast(Appointment apt, DateTime today) {
			return IsAppointmentInPastCore(apt, today);
		}
		protected internal virtual bool IsAppointmentInPastCore(Appointment apt, DateTime today) {
			AppointmentBaseCollection appointments = new AppointmentBaseCollection();
			RecurringAppointmentsSeparator separator = new RecurringAppointmentsSeparator();
			PrevNextAppointmentCalculator calc = new PrevNextAppointmentCalculator(new EmptyAppointmentPredicate(), new EmptyOccurrenceInfoPredicate());
			appointments.Add(apt);
			separator.Process(appointments);
			TimeInterval next = calc.FindNearestAppointmentIntervalAfter((AppointmentBaseCollection)separator.DestinationCollection, separator.DestinationPatterns, today);
			return next == null;
		}
	}
}
