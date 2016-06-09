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
using System.Collections;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base.General;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Xml;
namespace DevExpress.ExpressApp.Scheduler {
	public abstract class SchedulerRecurrenceInfoControllerBase : ObjectViewController {
		private DeleteObjectsViewController deleteObjectsViewController;
		private PropertyEditor recurrenceInfoPropertyEditor;
		private void deleteObjectsViewController_Deleting(object sender, DeletingEventArgs e) {
			IRecurrentEvent patternEvent = DetailView.CurrentObject as IRecurrentEvent;
			if(patternEvent != null && e.Objects.Contains(patternEvent)) {
				foreach(IRecurrentEvent exceptionalEvent in GetExceptionalEvents(patternEvent)) {
					e.Objects.Add(exceptionalEvent);
				}
			}
		}
		private void View_ControlsCreated(object sender, EventArgs e) {
			recurrenceInfoPropertyEditor = GetRecurrenceInfoPropertyEditor();
			if(recurrenceInfoPropertyEditor != null) {
				recurrenceInfoPropertyEditor.ControlValueChanged += new EventHandler(recurrenceInfoPropertyEditor_ControlValueChanged);
			}
		}
		private void recurrenceInfoPropertyEditor_ControlValueChanged(object sender, EventArgs e) {
			DeleteExceptionalEvents();
		}
		private void DeleteExceptionalEvents() {
			IRecurrentEvent patternEvent = DetailView.CurrentObject as IRecurrentEvent;
			if(patternEvent != null) {
				IList exceptionalEvents = GetExceptionalEvents(patternEvent);
				if(exceptionalEvents.Count > 0) {
					ObjectSpace.Delete(exceptionalEvents);
				}
			}
		}
		private IList GetExceptionalEvents(IRecurrentEvent patternEvent) {
			return ObjectSpace.GetObjects(patternEvent.GetType(), new BinaryOperator("RecurrencePattern", patternEvent));
		}
		private void ObjectSpace_ObjectSaving(object sender, ObjectManipulatingEventArgs e) {
			if(e.Object == DetailView.CurrentObject) {
				UpdateEventDates();
			}
		}
		protected void UpdateEventDates() {
			IEvent schedulerEvent = (IEvent)DetailView.CurrentObject;
			if(schedulerEvent.Type == (int)AppointmentType.Pattern) {
				UpdateRecurrenceInfoDates();
			}
		}
		private void UpdateRecurrenceInfoDates() {
			IRecurrentEvent schedulerEvent = DetailView.CurrentObject as IRecurrentEvent;
			DevExpress.XtraScheduler.IRecurrenceInfo recurrenceInfo = RecurrenceInfoXmlPersistenceHelper.ObjectFromXml(schedulerEvent.RecurrenceInfoXml);
			if(recurrenceInfo != null) {
				recurrenceInfo.AllDay = schedulerEvent.AllDay;
				recurrenceInfo.Start = schedulerEvent.StartOn;
				schedulerEvent.RecurrenceInfoXml = recurrenceInfo.ToXml();
			}
		}
		protected abstract PropertyEditor GetRecurrenceInfoPropertyEditor();
		protected void RefreshRecurrenceInfoPropertyEditor() {
			PropertyEditor recurrenceInfoPropertyEditor = GetRecurrenceInfoPropertyEditor();
			if(recurrenceInfoPropertyEditor != null) {
				recurrenceInfoPropertyEditor.Refresh();
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			View.ControlsCreated += new EventHandler(View_ControlsCreated);
			deleteObjectsViewController = Frame.GetController<DeleteObjectsViewController>();
			if(deleteObjectsViewController != null) {
				deleteObjectsViewController.Deleting += new EventHandler<DeletingEventArgs>(deleteObjectsViewController_Deleting);
			}
			ObjectSpace.ObjectSaving += new EventHandler<ObjectManipulatingEventArgs>(ObjectSpace_ObjectSaving);
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			View.ControlsCreated -= new EventHandler(View_ControlsCreated);
			if(recurrenceInfoPropertyEditor != null) {
				recurrenceInfoPropertyEditor.ControlValueChanged -= new EventHandler(recurrenceInfoPropertyEditor_ControlValueChanged);
				recurrenceInfoPropertyEditor = null;
			}
			if(deleteObjectsViewController != null) {
				deleteObjectsViewController.Deleting -= new EventHandler<DeletingEventArgs>(deleteObjectsViewController_Deleting);
				deleteObjectsViewController = null;
			}
			ObjectSpace.ObjectSaving -= new EventHandler<ObjectManipulatingEventArgs>(ObjectSpace_ObjectSaving);
		}
		public SchedulerRecurrenceInfoControllerBase() {
			TargetObjectType = typeof(IRecurrentEvent);
			TargetViewType = ViewType.DetailView;
		}
		public DetailView DetailView {
			get { return (DetailView)View; }
		}
	}
}
