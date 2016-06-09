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
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base.General;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Internal.Implementations;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Xml;
namespace DevExpress.ExpressApp.Scheduler {
	public class EventAssigner {
		private SourceObjectHelperBase sourceObjectHelper;
		private SchedulerListEditorBase schedulerEditor;
		public EventAssigner(SourceObjectHelperBase sourceObjectHelper, SchedulerListEditorBase schedulerEditor) {
			this.sourceObjectHelper = sourceObjectHelper;
			this.schedulerEditor = schedulerEditor;
		}
		public void AssignAppointment(IEvent schedulerEvent, Appointment appointment, IObjectSpace exceptionEventObjectSpace) {
			AssignBaseProperties(schedulerEvent, appointment);
			foreach(CustomField property in appointment.CustomFields) {
				IMemberInfo customMemberInfo = schedulerEditor.CollectionSource.ObjectTypeInfo.FindMember(schedulerEditor.AppointmentsCustomFieldMappings[property.Name].Member);
				object propertyValue = property.Value;
				if(propertyValue == DBNull.Value) { 
					propertyValue = null;
				}
				customMemberInfo.SetValue(schedulerEvent, exceptionEventObjectSpace.GetObject(propertyValue));
			}
			AssignRecurrences(schedulerEvent, appointment, exceptionEventObjectSpace);
			AssignResources(schedulerEvent, appointment);
		}
		public void AssignBaseProperties(IEvent schedulerEvent, Appointment appointment) {
			schedulerEvent.Subject = appointment.Subject;
			schedulerEvent.Description = appointment.Description;
			schedulerEvent.Location = appointment.Location;
			schedulerEvent.AllDay = appointment.AllDay;
			schedulerEvent.StartOn = appointment.Start;
			schedulerEvent.EndOn = appointment.End;
			schedulerEvent.Label = (appointment.LabelKey is Int32) ? (Int32)appointment.LabelKey : 0;
			schedulerEvent.Status = (appointment.StatusKey is Int32) ? (Int32)appointment.StatusKey : 0;
			schedulerEvent.Type = (Int32)appointment.Type;
		}
		public void AssignRecurrences(IEvent schedulerEvent, Appointment appointment, IObjectSpace exceptionEventObjectSpace) {
			IRecurrentEvent recurrenceEvent = schedulerEvent as IRecurrentEvent;
			if(recurrenceEvent != null) {
				recurrenceEvent.RecurrencePattern = GetRecurrencePattern(appointment, exceptionEventObjectSpace);
				if(appointment.RecurrenceInfo != null) {
					if(appointment.Type == AppointmentType.Pattern) {
						recurrenceEvent.RecurrenceInfoXml = appointment.RecurrenceInfo.ToXml();
					}
					else {
						recurrenceEvent.RecurrenceInfoXml = String.Format(@"<RecurrenceInfo Id=""{0}"" Index=""{1}""/>", appointment.RecurrenceInfo.Id, appointment.RecurrenceIndex);
					}
				}
			}
		}
		public IRecurrentEvent GetRecurrencePattern(Appointment appointment, IObjectSpace exceptionEventObjectSpace) {
			if(appointment.RecurrencePattern != null) {
				return (IRecurrentEvent)(exceptionEventObjectSpace.GetObject(sourceObjectHelper.GetSourceObject(appointment.RecurrencePattern)));
			}
			return null;
		}
		public void AssignResources(IEvent schedulerEvent, Appointment appointment) {
			if(appointment.ResourceId != ResourceBase.Empty.Id) {
				AppointmentResourceIdCollectionXmlPersistenceHelper helper = new AppointmentResourceIdCollectionXmlPersistenceHelper(appointment.ResourceIds);
				schedulerEvent.ResourceId = helper.ToXml();
			}
		}
	}
}
