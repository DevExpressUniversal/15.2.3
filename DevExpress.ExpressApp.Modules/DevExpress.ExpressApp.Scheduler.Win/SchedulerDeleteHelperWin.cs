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
using DevExpress.XtraScheduler.Forms;
using DevExpress.XtraScheduler;
using System.Windows.Forms;
using System.ComponentModel;
namespace DevExpress.ExpressApp.Scheduler.Win {
	public class DeleteResultEventArgs : HandledEventArgs {
		private Appointment appointment;
		private DeleteResult deleteResult;
		public DeleteResultEventArgs(Appointment appointment) {
			this.appointment = appointment;
		}
		public Appointment Appointment {
			get {
				return appointment;
			}
		}
		public DeleteResult DeleteResult {
			get {
				return deleteResult;
			}
			set {
				deleteResult = value;
			}
		}
	}
	public class SchedulerDeleteHelperWin : SchedulerDeleteHelper {
		protected override void CreateException(Appointment patternAppointment, AppointmentType type, int index, CollectionSourceBase collectionSource) {
			((PersistentObjectStorage<Appointment>)schedulerEditor.StorageBase.Appointments).ShouldUpdateAfterInsert = false;
			((PersistentObjectStorage<DevExpress.XtraScheduler.Resource>)schedulerEditor.StorageBase.Resources).ShouldUpdateAfterInsert = false;
			Appointment exceptionAppointment = patternAppointment.CreateException(type, index);
			if(collectionSource.DataAccessMode == CollectionSourceDataAccessMode.DataView) {
				IEvent exceptionEvent = objectSpace.CreateObject(collectionSource.ObjectTypeInfo.Type) as IEvent;
				schedulerEditor.EventAssigner.AssignAppointment(exceptionEvent, exceptionAppointment, objectSpace);
			}
		}
		public SchedulerDeleteHelperWin(SchedulerListEditorBase schedulerEditor)
			: base(schedulerEditor) {
		}
		public override void ShowConfirmation(IEvent schedulerEvent) {
			DeleteResultEventArgs args = new DeleteResultEventArgs(schedulerEditor.GetAppointment(schedulerEvent, null));
			OnOccurrenceDeleting(args);
			if(!args.Handled) {
				RecurrentAppointmentDeleteForm deleteForm = new RecurrentAppointmentDeleteForm(schedulerEditor.GetAppointment(schedulerEvent));
				args.DeleteResult = DeleteResult.Cancel;
				if(deleteForm.ShowDialog() != DialogResult.Cancel) {
					switch(deleteForm.QueryResult) {
						case RecurrentAppointmentAction.Occurrence:
							args.DeleteResult = DeleteResult.Occurrence;
							break;
						case RecurrentAppointmentAction.Series:
							args.DeleteResult = DeleteResult.Series;
							break;
					}
				}
			}
			ProcessResult(args.DeleteResult);
		}
		protected void OnOccurrenceDeleting(DeleteResultEventArgs args) {
			if(OccurrenceDeleting != null) {
				OccurrenceDeleting(this, args);
			}
		}
		public event EventHandler<DeleteResultEventArgs> OccurrenceDeleting;
	}
}
