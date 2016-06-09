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
using System.Text;
using DevExpress.Persistent.Base.General;
using System.Collections;
using DevExpress.ExpressApp.Utils;
using DevExpress.XtraScheduler;
namespace DevExpress.ExpressApp.Scheduler {
	public abstract class SchedulerDeleteHelper {
		private Dictionary<IEvent, List<Int32>> objectsToProcess;
		private Int32 processingIndex;
		private IList<IEvent> objectsToDelete;
		protected SchedulerListEditorBase schedulerEditor;
		protected IObjectSpace objectSpace;
		private IEnumerator<IEvent> appointmentsToAskEnumerator;
		public SchedulerDeleteHelper(SchedulerListEditorBase schedulerEditor) {
			Guard.ArgumentNotNull(schedulerEditor, "schedulerEditor");
			this.objectsToProcess = new Dictionary<IEvent, List<int>>();
			this.objectsToDelete = new List<IEvent>();
			this.schedulerEditor = schedulerEditor;
		}
		private void LoadObjects(IList events) {
			this.objectsToProcess.Clear();
			this.objectsToDelete.Clear();
			foreach(Object obj in events) {
				IEvent schedulerEvent = objectSpace.GetObject(obj) as IEvent;
				if(schedulerEvent != null) {
					if(schedulerEvent.Type == (int)AppointmentType.Normal) {
						this.objectsToDelete.Add(schedulerEvent);
					}
					else {
						IEvent patternEvent = GetPatternEvent(schedulerEvent);
						if(patternEvent != null && !this.objectsToProcess.ContainsKey(patternEvent)) {
							Appointment patternAppointment = schedulerEditor.GetAppointment(patternEvent, objectSpace);
							List<Int32> recurrenceIndexes = new List<Int32>();
							foreach(Appointment occurrence in schedulerEditor.SelectedAppointments)
								if(occurrence.RecurrencePattern == patternAppointment)
									recurrenceIndexes.Add(occurrence.RecurrenceIndex);
							this.objectsToProcess.Add(patternEvent, recurrenceIndexes);
						}
					}
				}
			}
		}
		private void AddToDeleting(IEvent schedulerEvent) {
			if(!objectsToDelete.Contains(schedulerEvent)) {
				objectsToDelete.Add(schedulerEvent);
			}
		}
		private IEvent GetPatternEvent(IEvent currentEvent) {
			IRecurrentEvent patternEvent = currentEvent as IRecurrentEvent;
			if(currentEvent.Type != (int)AppointmentType.Normal) {
				if(patternEvent.Type != (int)AppointmentType.Pattern) {
					patternEvent = patternEvent.RecurrencePattern;
				}
			}
			return patternEvent;
		}
		private bool IsAddedToDelete(IEvent refreshedEvent) {
			foreach(IEvent schedulerEvent in objectsToDelete) {
				if(objectSpace.GetObject(refreshedEvent) == objectSpace.GetObject(schedulerEvent)) {
					return true;
				}
			}
			return false;
		}
		private IEnumerable<IEvent> GetEventToAsk() {
			foreach(IEvent currentEvent in objectsToProcess.Keys) {
				foreach(Int32 currentIndex in objectsToProcess[currentEvent]) {
					if(IsAddedToDelete(currentEvent)) {
						break;
					}
					processingIndex = currentIndex;
					yield return objectSpace.GetObject(currentEvent);
				}
			}
			yield break;
		}
		private void ShowNextConfirmation() {
			if(appointmentsToAskEnumerator.MoveNext()) {
				ShowConfirmation(appointmentsToAskEnumerator.Current);
			}
			else {
				OnRecurrencesConfirmated();
			}
		}
		protected virtual void OnRecurrencesConfirmated() {
			if(RecurrencesConfirmated != null) {
				RecurrencesConfirmated(this, EventArgs.Empty);
			}
		}
		protected virtual void CreateException(Appointment patternAppointment, AppointmentType type, Int32 index, CollectionSourceBase collectionSource) {
			patternAppointment.CreateException(type, index);
		}
		public abstract void ShowConfirmation(IEvent schedulerEvent);
		public void StartProcessing(IList events, IObjectSpace objectSpace) {
			this.objectSpace = objectSpace;
			LoadObjects(events);
			appointmentsToAskEnumerator = GetEventToAsk().GetEnumerator();
			ShowNextConfirmation();
		}
		public void ProcessResult(DeleteResult result) {
			IEvent schedulerEvent = appointmentsToAskEnumerator.Current;
			IEvent processingEvent = objectSpace.GetObject(schedulerEvent);
			if(processingEvent.Type == (Int32)AppointmentType.Normal) {
				return;
			}
			Appointment patternAppointment = schedulerEditor.GetAppointment(processingEvent, objectSpace);
			if(patternAppointment.Type != AppointmentType.Pattern) {
				patternAppointment = patternAppointment.RecurrencePattern;
			}
			switch(result) {
				case DeleteResult.Occurrence:
					Appointment changedOccurence = patternAppointment.GetExceptions().Find(new Predicate<Appointment>(delegate(Appointment app) { return app.RecurrenceIndex == processingIndex; }));
					if(changedOccurence != null) {
						IEvent changedEvent = schedulerEditor.SourceObjectHelper.GetSourceObject(changedOccurence);
						changedEvent.Type = (int)AppointmentType.DeletedOccurrence;
					}
					else {
						CreateException(patternAppointment, AppointmentType.DeletedOccurrence, processingIndex, schedulerEditor.CollectionSource);
					}
					break;
				case DeleteResult.Series:
					AddToDeleting(processingEvent);
					foreach(Appointment exceptionAppointment in patternAppointment.GetExceptions()) {
						AddToDeleting(schedulerEditor.SourceObjectHelper.GetSourceObject(exceptionAppointment));
					}
					break;
			}
			ShowNextConfirmation();
		}
		public IList<IEvent> GetObjectsToDelete() {
			IList<IEvent> refreshedObjectsToDelete = new List<IEvent>();
			foreach(IEvent schedulerEvent in objectsToDelete) {
				refreshedObjectsToDelete.Add(objectSpace.GetObject(schedulerEvent));
			}
			return refreshedObjectsToDelete;
		}
		public event EventHandler RecurrencesConfirmated;
	}
}
