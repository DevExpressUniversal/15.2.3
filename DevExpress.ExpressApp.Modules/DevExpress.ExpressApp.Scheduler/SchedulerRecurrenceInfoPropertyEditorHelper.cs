#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.ComponentModel;
using System.Globalization;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base.General;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Xml;
using DevExpress.XtraScheduler.Compatibility;
namespace DevExpress.ExpressApp.Scheduler {
	public enum RecurrenceResult { OK, Cancel, RemoveRecurrence }
	public class SchedulerRecurrenceInfoPropertyEditorHelper : IDisposable {
		private PropertyEditor propertyEditor;
		public static void AssignFromXml(XtraScheduler.IRecurrenceInfo dest, string recurrenceInfoXml) {
			if(!string.IsNullOrEmpty(recurrenceInfoXml)) {
				dest.FromXml(recurrenceInfoXml);
			}
		}
		public SchedulerRecurrenceInfoPropertyEditorHelper(PropertyEditor propertyEditor) {
			this.propertyEditor = propertyEditor;
		}
		private void SetReccurrenceInfo(IRecurrentEvent recurrentEvent, DevExpress.XtraScheduler.IRecurrenceInfo recurrenceInfo) {
			if(recurrentEvent.Type == (Int32)AppointmentType.Normal) {
				recurrentEvent.Type = (int)AppointmentType.Pattern;
			}
			SetRecurrenceInfo(recurrentEvent, recurrenceInfo.ToXml());
		}
		private void SetRecurrenceInfo(IRecurrentEvent recurrentEvent, string recurrenceInfo) {
			if(recurrentEvent.RecurrenceInfoXml != recurrenceInfo) {
				recurrentEvent.RecurrenceInfoXml = recurrenceInfo;
				OnRecurrenceInfoChanged();
			}
		}
		private void OnRecurrenceInfoChanged() {
			if(RecurrenceInfoChanged != null) {
				RecurrenceInfoChanged(this, EventArgs.Empty);
			}
		}
		public void ProcessResult(RecurrenceResult result, DevExpress.XtraScheduler.IRecurrenceInfo recurrenceInfo, DateTime startOn, DateTime endOn, bool allDay) {
			ProcessResult(OwnerEvent, result, recurrenceInfo, startOn, endOn, allDay);
		}
		public void ProcessResult(IRecurrentEvent recurrentEvent, RecurrenceResult result, DevExpress.XtraScheduler.IRecurrenceInfo recurrenceInfo, DateTime startOn, DateTime endOn, bool allDay) {
			if(result == RecurrenceResult.OK) {
				recurrentEvent.StartOn = startOn;
				recurrentEvent.EndOn = endOn;
				recurrentEvent.AllDay = allDay;
				SetReccurrenceInfo(recurrentEvent, recurrenceInfo);
			}
			if(result == RecurrenceResult.RemoveRecurrence) {
				SetRecurrenceInfo(recurrentEvent, String.Empty);
				recurrentEvent.Type = (int)AppointmentType.Normal;
			}
		}
		public bool TryGetRecurrenceInfoDescription(out string description) {
			bool result = true;
			description = String.Empty;
			if(OwnerEvent != null) {
				try {
					Appointment appointment = CreateEditingAppointment();
					if(!String.IsNullOrEmpty(OwnerEvent.RecurrenceInfoXml)) {
						description = RecurrenceInfo.GetDescription(appointment, DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek);
					}
				}
				catch(ArgumentException) {
					description = SchedulerModuleBaseLocalizer.Active.GetLocalizedString(SchedulerModuleBaseId.ObjectNotValid);
					result = false;
				}
			}
			return result;
		}
		public DevExpress.XtraScheduler.IRecurrenceInfo GetRecurrenceInfo() {
			if(!String.IsNullOrEmpty(OwnerEvent.RecurrenceInfoXml)) {
				return RecurrenceInfoXmlPersistenceHelper.ObjectFromXml(OwnerEvent.RecurrenceInfoXml);
			}
			else {
				return null;
			}
		}
		public Appointment CreateEditingAppointment() {
			return CreateEditingAppointment(OwnerEvent);
		}
		public Appointment CreateEditingAppointment(IRecurrentEvent recurrentEvent) {
			IEvent baseEvent = recurrentEvent;
			if(recurrentEvent.RecurrencePattern != null) {
				baseEvent = recurrentEvent.RecurrencePattern;
			}
			Appointment appointment = StaticAppointmentFactory.CreateAppointment(AppointmentType.Pattern, baseEvent.StartOn, baseEvent.EndOn);
			appointment.AllDay = baseEvent.AllDay; 
			AssignFromXml(appointment.RecurrenceInfo, recurrentEvent.RecurrenceInfoXml);
			return appointment;
		}
		public IRecurrentEvent OwnerEvent {
			get {
				IRecurrentEvent ownerEvent = null;
				IList<IMemberInfo> path = propertyEditor.MemberInfo.GetPath();
				if(path.Count > 1) { 
					for(int i = path.Count - 1; i >= 0; i--) {
						if(typeof(IRecurrentEvent).IsAssignableFrom(path[i].MemberType)) {
							ownerEvent = (IRecurrentEvent)path[i].GetValue(propertyEditor.CurrentObject);
							break;
						}
					}
				}
				else {
					ownerEvent = (IRecurrentEvent)propertyEditor.CurrentObject;
				}
				return ownerEvent;
			}
		}
		public bool IsEnabled {
			get {
				return OwnerEvent != null && SchedulerUtils.IsBaseType((AppointmentType)OwnerEvent.Type);
			}
		}
		public event EventHandler RecurrenceInfoChanged;
		public void Dispose() {
			RecurrenceInfoChanged = null;
			propertyEditor = null;
		}
		#region Obsolete 15.2
		[Obsolete("Use the IRecurrenceInfo.Assign method instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static void Assign(XtraScheduler.IRecurrenceInfo dest, XtraScheduler.IRecurrenceInfo source) {
			dest.Assign(source);
		}
		#endregion
	}
}
