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
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Compatibility;
namespace DevExpress.ExpressApp.Scheduler {
	public enum DeleteResult { Occurrence, Series, Cancel }
	public abstract class SchedulerListViewControllerBase : ObjectViewController {
		private NewActionEventArgs newActionEventArgs;
		protected DeleteObjectsViewController deleteObjectsViewController;
		private int suppressCount = 0;
		private CriteriaOperator visibleIntervalCriteria;
		private void deleteObjectsViewController_Deleting(object sender, DeletingEventArgs e) {
			OnDeleting(e.Objects);
		}
		private void SchedulerListViewControllerBase_FilterDataSource(object sender, FilterDataSourceEventArgs e) {
			OnFilterDataSource(e);
		}
		private void CollectionSource_CollectionChanged(Object sender, EventArgs e) {
			AddExpression();
		}
		private void AddExpression() {
			XafDataView dataView = ((ListView)View).CollectionSource.Collection as XafDataView;
			if((dataView != null) && dataView.Expressions.FirstOrDefault((DataViewExpression expression) => expression.Name == "AppointmentId") == null) {
				List<DataViewExpression> expressions = new List<DataViewExpression>();
				expressions.AddRange(dataView.Expressions);
				string id = ObjectSpace.GetKeyPropertyName(dataView.ObjectType);
				expressions.Add(new DataViewExpression("AppointmentId", id));
				expressions.Add(new DataViewExpression("ResourceId", "'<ResourceIds></ResourceIds>'"));
				dataView.Expressions = expressions;
			}
		}
		protected void Refill(IList collection, ICollection<IEvent> fromCollection) {
			collection.Clear();
			foreach(IEvent obj in fromCollection) {
				collection.Add(obj);
			}
			fromCollection.Clear();
		}
		private void SubscribeToEventsIfEditorIsSchedulerListEditor() {
			schedulerEditor = ((ListView)View).Editor as SchedulerListEditorBase;
			if(schedulerEditor != null) {
				View.ControlsCreated += new EventHandler(View_ControlsCreated);
				View.CurrentObjectChanged += new EventHandler(View_CurrentObjectChanged);
				SubscribeToSchedulerListEditorEvents();
				deleteObjectsViewController = Frame.GetController<DeleteObjectsViewController>();
				if(deleteObjectsViewController != null) {
					deleteObjectsViewController.Deleting += new EventHandler<DeletingEventArgs>(deleteObjectsViewController_Deleting);
				}
			}
		}
		private void View_CurrentObjectChanged(object sender, EventArgs e) {
			OnCurrentObjectChanged();
		}
		private void UnsubscribeFromEventsIfEditorIsSchedulerListEditor() {
			if(schedulerEditor != null) {
				View.ControlsCreated -= new EventHandler(View_ControlsCreated);
				View.CurrentObjectChanged -= new EventHandler(View_CurrentObjectChanged);
				UnsubscribeFromSchedulerListEditorEvents();
				if(deleteObjectsViewController != null) {
					deleteObjectsViewController.Deleting -= new EventHandler<DeletingEventArgs>(deleteObjectsViewController_Deleting);
				}
			}
		}
		protected virtual void OnFilterDataSource(FilterDataSourceEventArgs e) {
			if(((ListView)View).CollectionSource != null) {
				PropertyCollectionSource collectionSource = ((ListView)View).CollectionSource as PropertyCollectionSource;
				if(collectionSource != null) {
					if(collectionSource.MasterObject != null && typeof(DevExpress.XtraScheduler.Resource).IsAssignableFrom(collectionSource.MasterObjectType) && collectionSource.MemberInfo.AssociatedMemberInfo != null) {
						if(collectionSource.MemberInfo.AssociatedMemberInfo.IsList) {
							FilterDataSourceUsingMasterObjectAsList(collectionSource.MemberInfo.AssociatedMemberInfo.Name, collectionSource.MasterObject);
						}
						else {
							FilterDataSourceUsingMasterObject(collectionSource.MemberInfo.AssociatedMemberInfo.Name, collectionSource.MasterObject);
						}
					}
					else {
						RemoveDataSourceFilter();
					}
				}
				else {
					FilterDataSourceUsingVisibleInterval(e.StartDate, e.EndDate);
				}
			}
		}
		protected void RemoveDataSourceFilter() {
			((ListView)View).CollectionSource.Criteria["ActiveViewFilter"] = null;
		}
		protected void FilterDataSourceUsingMasterObjectAsList(string collectionProperty, object masterObject) {
			ContainsOperator containsOperator = ((ListView)View).CollectionSource.Criteria["ActiveViewFilter"] as ContainsOperator;
			object keyValue = View.ObjectSpace.GetKeyValue(masterObject);
			if(ReferenceEquals(containsOperator, null) ||
				(((OperandValue)((BinaryOperator)containsOperator.Condition).RightOperand).Value != keyValue)) {
				((ListView)View).CollectionSource.Criteria["ActiveViewFilter"] =
					new ContainsOperator(collectionProperty, new BinaryOperator(View.ObjectSpace.GetKeyPropertyName(masterObject.GetType()), keyValue));
			}
		}
		protected void FilterDataSourceUsingMasterObject(string masterObjectProperty, object masterObject) {
			BinaryOperator binaryOperator = ((ListView)View).CollectionSource.Criteria["ActiveViewFilter"] as BinaryOperator;
			object keyValue = View.ObjectSpace.GetKeyValue(masterObject);
			if(ReferenceEquals(binaryOperator, null) ||
				(((OperandValue)binaryOperator.RightOperand).Value != keyValue)) {
				((ListView)View).CollectionSource.Criteria["ActiveViewFilter"] =
					new BinaryOperator(masterObjectProperty + "." + View.ObjectSpace.GetKeyPropertyName(masterObject.GetType()), keyValue);
			}
		}
		protected void FilterDataSourceUsingVisibleInterval(DateTime startDate, DateTime endDate) {
			GroupOperator groupOperator = visibleIntervalCriteria as GroupOperator;
			BetweenOperator betweenOperator = null;
			if(!ReferenceEquals(groupOperator, null)) {
				betweenOperator = ((BetweenOperator)groupOperator.Operands[0]);
			}
			bool updateOnlyInitialCriteria = true;
			if(ReferenceEquals(groupOperator, null)
				|| (startDate < (DateTime)((OperandValue)betweenOperator.BeginExpression).Value)
				|| (endDate > (DateTime)((OperandValue)betweenOperator.EndExpression).Value)) {
				startDate = startDate - TimeSpan.FromDays(35);
				endDate = endDate + TimeSpan.FromDays(35);
				visibleIntervalCriteria =
					CriteriaOperator.Or(
						new BetweenOperator(SchedulerEditor.AppointmentsMappings.Start, startDate, endDate),
						new BetweenOperator(SchedulerEditor.AppointmentsMappings.End, startDate, endDate),
						CriteriaOperator.And(
							new BinaryOperator(SchedulerEditor.AppointmentsMappings.Start, startDate, BinaryOperatorType.LessOrEqual),
							new BinaryOperator(SchedulerEditor.AppointmentsMappings.End, endDate, BinaryOperatorType.GreaterOrEqual)
						),
						CriteriaOperator.And(
							new BinaryOperator(SchedulerEditor.AppointmentsMappings.Type, (int)AppointmentType.Pattern),
							new BinaryOperator(SchedulerEditor.AppointmentsMappings.Start, endDate, BinaryOperatorType.LessOrEqual)
						),
						new BinaryOperator(SchedulerEditor.AppointmentsMappings.Type, (int)AppointmentType.ChangedOccurrence) 
					);
				updateOnlyInitialCriteria = false;
			}
			ApplyVisibleIntervalCriteria(updateOnlyInitialCriteria);
		}
		protected void ApplyVisibleIntervalCriteria(bool updateOnlyInitialCriteria) {
			if(CanApplyCriteria && !ReferenceEquals(visibleIntervalCriteria, null)) {
				CriteriaOperator currentCriteria = ((ListView)View).CollectionSource.Criteria["ActiveViewFilter"];
				if(!updateOnlyInitialCriteria || SelectNothingCriteria.Equals(currentCriteria)) {
					((ListView)View).CollectionSource.Criteria["ActiveViewFilter"] = visibleIntervalCriteria;
				}
			}
		}
		protected void schedulerListEditor_DataSourceChanged(object sender, EventArgs e) {
			OnDataSourceChanged();
		}
		protected void schedulerListEditor_NewAction(object sender, NewActionEventArgs e) {
			newActionEventArgs = e;
			OnNewAction(e);
		}
		protected void NewObjectViewController_ObjectCreating(object sender, ObjectCreatingEventArgs e) {
			if(newActionEventArgs == null && typeof(IEvent).IsAssignableFrom(e.ObjectType)) {
				if(schedulerEditor != null && schedulerEditor.SelectedInterval != null) {
					Appointment app = StaticAppointmentFactory.CreateAppointment(AppointmentType.Normal, schedulerEditor.SelectedInterval.Start, schedulerEditor.SelectedInterval.End);
					if(schedulerEditor.SelectedResource != null) {
						app.ResourceId = schedulerEditor.SelectedResource.Id;
					}
					newActionEventArgs = new NewActionEventArgs(app);
				}
			}
		}
		protected void NewObjectViewController_ObjectCreated(object sender, ObjectCreatedEventArgs e) {
			if(newActionEventArgs != null) {
				IEvent schedulerEvent = (IEvent)e.CreatedObject;
				Appointment appointment = newActionEventArgs.Appointment;
				schedulerEditor.EventAssigner.AssignAppointment(schedulerEvent, appointment, e.ObjectSpace);
				IEvent patternEvent = schedulerEditor.EventAssigner.GetRecurrencePattern(appointment, e.ObjectSpace);
				if(patternEvent != null) {
					schedulerEditor.RaiseExceptionEventCreated(patternEvent, schedulerEvent);
				}
				newActionEventArgs = null;
			}
		}
		protected void schedulerListEditor_ObjectChanged(object sender, EventArgs e) {
			bool isShowNotifications = false;
			ObjectSpace.SetModified(ObjectSpace.GetObject(SchedulerEditor.FocusedObject)); 
			if(ObjectSpace.ModifiedObjects.Count > 0 && !(((ListView)View).CollectionSource is PropertyCollectionSource)) {
				foreach(object obj in ObjectSpace.ModifiedObjects) {
					if(obj as IEvent != null && obj as IReminderEvent != null && ((IReminderEvent)obj).AlarmTime.HasValue && ((IReminderEvent)obj).AlarmTime <= DateTime.Now) {
						isShowNotifications = true;
						break;
					}
				}
				ObjectSpace.CommitChanges();
				if(isShowNotifications) {
					ShowNotifications(this.Application.Modules.FirstOrDefault(m => m is INotificationsServiceOwner) as INotificationsServiceOwner);
				}
			}
		}
		protected virtual void ShowNotifications(INotificationsServiceOwner storage) {
		}
		protected void View_ControlsCreated(object sender, EventArgs e) {
			UnsubscribeFromSchedulerListEditorEvents();
			SubscribeToSchedulerListEditorEvents();
			OnControlsCreated();
		}
		private void View_InfoChanged(object sender, EventArgs e) {
			OnViewInfoChanged();
		}
		private void View_InfoChanging(object sender, EventArgs e) {
			OnViewInfoChanging();
		}
		private bool IsNotLookupListView {
			get { return View.IsRoot || ((Frame is NestedFrame) && ((NestedFrame)Frame).ViewItem is ListPropertyEditor); }
		}
		private SchedulerListEditorBase schedulerEditor;
		protected SchedulerListEditorBase SchedulerEditor {
			get { return schedulerEditor; }
		}
		protected virtual void OnCurrentObjectChanged() {
		}
		protected virtual void OnDataSourceChanged() {
		}
		protected virtual void OnNewAction(NewActionEventArgs e) {
			NewObjectViewController newObjectController = Frame.GetController<NewObjectViewController>();
			if(newObjectController != null) {
				SingleChoiceAction newObjectAction = newObjectController.NewObjectAction;
				if(newObjectAction.Active && newObjectAction.Enabled) {
					newObjectAction.DoExecute(new ChoiceActionItem("", ((ListView)View).ObjectTypeInfo.Type));
				}
				else {
					Tracing.Tracer.LogWarning(GetType().FullName + ".OnNewAction, newObjectAction: " +
						DiagnosticInfoProviderBase.XmlNodeToString(ActionsInfoController.CreateActionNode(new XmlDocument(), newObjectAction)));
				}
			}
			else {
				Tracing.Tracer.LogWarning(GetType().FullName + ".OnNewAction, Frame.GetController<NewObjectViewController>() returns null");
			}
		}
		protected virtual void OnControlsCreated() {
		}
		protected virtual void OnViewInfoChanging() {
			UnsubscribeFromEventsIfEditorIsSchedulerListEditor();
		}
		protected virtual void OnViewInfoChanged() {
			SubscribeToEventsIfEditorIsSchedulerListEditor();
		}
		protected virtual void SubscribeToSchedulerListEditorEvents() {
			SchedulerEditor.FilterDataSource += new EventHandler<FilterDataSourceEventArgs>(SchedulerListViewControllerBase_FilterDataSource);
		}
		protected virtual void UnsubscribeFromSchedulerListEditorEvents() {
			SchedulerEditor.FilterDataSource -= new EventHandler<FilterDataSourceEventArgs>(SchedulerListViewControllerBase_FilterDataSource);
		}
		protected virtual void OnDeleting(IList objectsToDelete) {
		}
		protected override void OnActivated() {
			base.OnActivated();
			AddExpression();
			((ListView)View).CollectionSource.CollectionChanged += CollectionSource_CollectionChanged;
			View.ModelChanging += new EventHandler(View_InfoChanging);
			View.ModelChanged += new EventHandler(View_InfoChanged);
			SubscribeToEventsIfEditorIsSchedulerListEditor();
			ListView listView = (ListView)View;
			if(listView.Editor is SchedulerListEditorBase && IsNotLookupListView) {
				listView.CollectionSource.Criteria["ActiveViewFilter"] = SelectNothingCriteria;
			}
		}
		protected override void OnDeactivated() {
			UnsubscribeFromEventsIfEditorIsSchedulerListEditor();
			View.ModelChanging -= new EventHandler(View_InfoChanging);
			View.ModelChanged -= new EventHandler(View_InfoChanged);
			newActionEventArgs = null;
			deleteObjectsViewController = null;
			suppressCount = 0;
			visibleIntervalCriteria = null;
			this.schedulerEditor = null;
			base.OnDeactivated();
		}
		public SchedulerListViewControllerBase() {
			this.TargetViewType = ViewType.ListView;
			this.TargetObjectType = typeof(IEvent);
		}
		public void SuppressConfirmationShowing() {
			suppressCount++;
		}
		public void RestoreConfirmationShowing() {
			suppressCount--;
		}
		private CriteriaOperator SelectNothingCriteria {
			get { return CriteriaOperator.Parse("1=2"); }
		}
		protected virtual bool CanApplyCriteria {
			get { return true; }
		}
		public bool ShowConfirmations {
			get { return suppressCount == 0; }
		}
#if DebugTest
		public CriteriaOperator DebugTest_VisibleIntervalCriteria {
			get { return visibleIntervalCriteria; }
			set { visibleIntervalCriteria = value; }
		}
#endif
	}
}
