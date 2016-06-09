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
using System.ComponentModel;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.SystemModule {
	public class OrderProviderInitializer {
		private OrderProviderSource orderProviderSource;
		private XafApplication application;
		private Frame targetFrame;
		private void Application_ViewShown(Object sender, ViewShownEventArgs e) {
			if((application != null) && (e.TargetFrame == targetFrame)) {
				if(e.TargetFrame.GetController<RecordsNavigationController>() != null) {
					e.TargetFrame.GetController<RecordsNavigationController>().OrderProviderSource = orderProviderSource;
				}
				application.ViewShown -= new EventHandler<ViewShownEventArgs>(Application_ViewShown);
				application = null;
				targetFrame = null;
				orderProviderSource = null;
			}
		}
		public void Attach(XafApplication application, OrderProviderSource orderProviderSource, Frame targetFrame) {
			this.orderProviderSource = orderProviderSource;
			this.targetFrame = targetFrame;
			if(orderProviderSource != null) {
				this.application = application;
				application.ViewShown += new EventHandler<ViewShownEventArgs>(Application_ViewShown);
			}
		}
	}
	public class RecordsNavigationController : ViewController {
		[Flags]
		public enum MovementDirection {
			None = 0,
			Forward = 1,
			Previous = 2,
			All = 3
		};
		private SimpleAction previousObjectAction;
		private SimpleAction nextObjectAction;
		private System.ComponentModel.IContainer components;
		private OrderProviderSource orderProviderSource;
		private IBindingList linkListViewBindingList;
		private ObjectView ObjectView {
			get { return (ObjectView)View; }
		}
		private ListView ListView {
			get { return View as ListView; }
		}
		private void LinkListViewBindingList_ListChanged(Object sender, ListChangedEventArgs e) {
			if(e.ListChangedType == ListChangedType.ItemAdded
					|| e.ListChangedType == ListChangedType.ItemChanged
					|| e.ListChangedType == ListChangedType.ItemDeleted
					|| e.ListChangedType == ListChangedType.ItemMoved
					|| e.ListChangedType == ListChangedType.Reset) {
				UpdateActionState();
			}
		}
		private void LinkToListViewController_LinkChanged(Object sender, EventArgs e) {
			UnsubscribeFromLinkListViewBindingList();
			SubscribeToLinkListViewBindingList();
		}
		private void SubscribeToLinkListViewBindingList() {
			if(Frame != null) {
				LinkToListViewController linkToListViewController = Frame.GetController<LinkToListViewController>();
				if(linkToListViewController != null) {
					Link link = linkToListViewController.Link;
					if((link != null) && (link.ListView != null) && (link.ListView.CollectionSource != null)) {
						linkListViewBindingList = link.ListView.CollectionSource.List as IBindingList;
						if(linkListViewBindingList != null) {
							linkListViewBindingList.ListChanged += new ListChangedEventHandler(LinkListViewBindingList_ListChanged);
						}
					}
				}
			}
		}
		private void UnsubscribeFromLinkListViewBindingList() {
			if(linkListViewBindingList != null) {
				linkListViewBindingList.ListChanged -= new ListChangedEventHandler(LinkListViewBindingList_ListChanged);
				linkListViewBindingList = null;
			}
		}
		private void listViewEditor_FocusedObjectChanged(object sender, EventArgs e) {
			UpdateActionState();
		}
		private void listView_EditorChanged(object sender, EventArgs e) {
			SubscribeToListViewEditor();
		}
		private void listView_EditorChanging(object sender, EventArgs e) {
			UnsubscribeFromListViewEditor();
		}
		private void SubscribeToListViewEditor() {
			if(View is ListView) {
				ListView listView = (ListView)View;
				if(listView.Editor != null) {
					listView.Editor.FocusedObjectChanged += new EventHandler(listViewEditor_FocusedObjectChanged);
				}
			}
		}
		private void UnsubscribeFromListViewEditor() {
			if(View is ListView) {
				ListView listView = (ListView)View;
				if(listView.Editor != null) {
					listView.Editor.FocusedObjectChanged -= new EventHandler(listViewEditor_FocusedObjectChanged);
				}
			}
		}
		private void View_SelectionChanged(Object sender, EventArgs e) {
			UpdateActionState();
		}
		private void View_CurrentObjectChanged(Object sender, EventArgs e) {
			UpdateActionState();
		}
		private void previousObjectAction_OnExecute(Object sender, SimpleActionExecuteEventArgs e) {
			MoveToPrevious();
		}
		private void nextObjectAction_OnExecute(Object sender, SimpleActionExecuteEventArgs e) {
			MoveToNext();
		}
		private void Application_ViewShowing(Object sender, ViewShowingEventArgs e) {
			if(Active) {
				OnViewShowing(e);
			}
		}
		private void ObjectSpace_Committed(Object sender, EventArgs e) {
			UpdateActionState();
		}
		private Object GetObjectByIndex(Int32 index) {
			Object result = null;
			if(index >= 0) {
				IOrderProvider orderProvider = OrderProviderSource.OrderProvider;
				if(orderProvider is StandaloneOrderProvider) {
					result = ObjectSpace.GetObjectByKey(((ObjectView)View).ObjectTypeInfo.Type, ((StandaloneOrderProvider)orderProvider).GetObjectKeyByIndex(index));
				}
				else {
					Object obj = orderProvider.GetObjectByIndex(index);
					if(obj is XafDataViewRecord) {
						result = obj;
					}
					else {
						result = ObjectSpace.GetObject(obj);
					}
				}
			}
			return result;
		}
		private int GetCurrentObjectIndex() {
			int result = -1;
			if(OrderProviderSource.OrderProvider is IObjectOrderProvider) {
				result = ((IObjectOrderProvider)OrderProviderSource.OrderProvider).GetIndexByObject(ObjectView.CurrentObject);
			}
			if(OrderProviderSource.OrderProvider is IKeyOrderProvider) {
				result = ((IKeyOrderProvider)OrderProviderSource.OrderProvider).GetIndexByObjectKey(ObjectSpace.GetKeyValue(ObjectView.CurrentObject));
			}
			return result;
		}
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.previousObjectAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
			this.nextObjectAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
			this.previousObjectAction.Caption = "Previous Record";
			this.previousObjectAction.Category = "RecordsNavigation";
			this.previousObjectAction.ConfirmationMessage = "";
			this.previousObjectAction.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
			this.previousObjectAction.PaintStyle = Templates.ActionItemPaintStyle.Image;
			this.previousObjectAction.Id = "PreviousObject";
			this.previousObjectAction.ImageName = "MenuBar_Prev";
			this.previousObjectAction.Shortcut = "Control+PageUp";
			this.previousObjectAction.QuickAccess = true;
			this.previousObjectAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.previousObjectAction_OnExecute);
			this.nextObjectAction.Caption = "Next Record";
			this.nextObjectAction.Category = "RecordsNavigation";
			this.nextObjectAction.ConfirmationMessage = "";
			this.nextObjectAction.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
			this.nextObjectAction.PaintStyle = Templates.ActionItemPaintStyle.Image;
			this.nextObjectAction.Id = "NextObject";
			this.nextObjectAction.ImageName = "MenuBar_Next";
			this.nextObjectAction.Shortcut = "Control+PageDown";
			this.nextObjectAction.QuickAccess = true;
			this.nextObjectAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.nextObjectAction_OnExecute);
		}
		protected Boolean CanMoveToObject(Object nextObject) {
			return
				(View != null) && (nextObject != null) && (ObjectView.CurrentObject != null)
				&&
				(
					(ObjectSpace.GetObjectType(ObjectView.CurrentObject) == ObjectSpace.GetObjectType(nextObject))
					||
					(View is ListView)
				);
		}
		protected virtual void ProcessViewShowing(Frame targetFrame) {
			new OrderProviderInitializer().Attach(Application, OrderProviderSource, targetFrame);
		}
		protected virtual void OnViewShowing(ViewShowingEventArgs e) {
			if((e.TargetFrame is Window) && (e.SourceFrame == Frame)) {
				if((e.View is DetailView) && (View is ObjectView) && ((ObjectView)View).ObjectTypeInfo.IsAssignableFrom(((DetailView)e.View).ObjectTypeInfo)) {
					ProcessViewShowing(e.TargetFrame);
				}
			}
		}
		protected virtual void UpdateActionState() {
			String editorSelectionKey = "Editor doesn't support focused row selection.";
			String viewNestingKey = "ListView or root DetailView";
			previousObjectAction.Active.RemoveItem("ValidOrderProvider");
			nextObjectAction.Active.RemoveItem("ValidOrderProvider");
			if(orderProviderSource != null && orderProviderSource.OrderProvider is StandaloneOrderProvider) {
				previousObjectAction.Active["ValidOrderProvider"] = ((StandaloneOrderProvider)(orderProviderSource.OrderProvider)).Valid;
				nextObjectAction.Active["ValidOrderProvider"] = ((StandaloneOrderProvider)(orderProviderSource.OrderProvider)).Valid;
			}
			Boolean viewNestingValue =
				(View is ListView)
				||
				(View is DetailView) && ((DetailView)View).IsRoot;
			previousObjectAction.Active[viewNestingKey] = viewNestingValue;
			nextObjectAction.Active[viewNestingKey] = viewNestingValue;
			previousObjectAction.Active.RemoveItem(editorSelectionKey);
			nextObjectAction.Active.RemoveItem(editorSelectionKey);
			if(View is ListView) {
				ListView listView = (ListView)View;
				if(listView.Editor != null) {
					previousObjectAction.Active.SetItemValue(editorSelectionKey, (listView.Editor.SelectionType & SelectionType.FocusedObject) == SelectionType.FocusedObject);
					nextObjectAction.Active.SetItemValue(editorSelectionKey, (listView.Editor.SelectionType & SelectionType.FocusedObject) == SelectionType.FocusedObject);
				}
			}
			if(Active && !ObjectSpace.IsCommitting) {
				MovementDirection moveByType = CanMoveBy();
				previousObjectAction.Enabled.SetItemValue("Can move to previous", (moveByType & MovementDirection.Previous) == MovementDirection.Previous);
				nextObjectAction.Enabled.SetItemValue("Can move to next", (moveByType & MovementDirection.Forward) == MovementDirection.Forward);
			}
		}
		protected virtual IOrderProvider CreateDefaultOrderProvider(View view) {
			return new DefaultOrderProvider(view, Frame);
		}
		protected override void OnActivated() {
			base.OnActivated();
			ListView listView = View as ListView;
			IOrderProvider defaultOrderProvider = CreateDefaultOrderProvider(View);
			if(listView != null) {
				orderProviderSource = new OrderProviderSource();
				orderProviderSource.OrderProvider = new ListEditorOrderProvider(listView, defaultOrderProvider as IObjectOrderProvider);
			}
			else {
				if(orderProviderSource == null) {
					orderProviderSource = new OrderProviderSource();
					orderProviderSource.OrderProvider = defaultOrderProvider;
				}
			}
			ObjectView.SelectionChanged += new EventHandler(View_SelectionChanged);
			ObjectView.CurrentObjectChanged += new EventHandler(View_CurrentObjectChanged);
			ObjectSpace.Committed += new EventHandler(ObjectSpace_Committed);
			SubscribeToLinkListViewBindingList();
			if((Frame != null) && (View is DetailView)) {
				LinkToListViewController linkToListViewController = Frame.GetController<LinkToListViewController>();
				if(linkToListViewController != null) {
					linkToListViewController.LinkChanged += new EventHandler(LinkToListViewController_LinkChanged);
				}
			}
			SubscribeToListViewEditor();
			if(listView != null) {
				listView.EditorChanging += new EventHandler(listView_EditorChanging);
				listView.EditorChanged += new EventHandler(listView_EditorChanged);
			}
			UpdateActionState();
		}
		protected virtual IList GetOrderedObjects() {
			return orderProviderSource != null ? orderProviderSource.OrderProvider.GetOrderedObjects() : null;
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			ObjectSpace.Committed -= new EventHandler(ObjectSpace_Committed);
			ObjectView.SelectionChanged -= new EventHandler(View_SelectionChanged);
			ObjectView.CurrentObjectChanged -= new EventHandler(View_CurrentObjectChanged);
			if((orderProviderSource != null) && (View is ListView)) {
				orderProviderSource.OrderProvider =	new StandaloneOrderProvider(View.ObjectSpace, GetOrderedObjects());
			}
			if(Frame != null) {
				LinkToListViewController linkToListViewController = Frame.GetController<LinkToListViewController>();
				if(linkToListViewController != null) {
					linkToListViewController.LinkChanged -= new EventHandler(LinkToListViewController_LinkChanged);
				}
			}
			ListView listView = View as ListView;
			if(listView != null) {
				listView.EditorChanging -= new EventHandler(listView_EditorChanging);
				listView.EditorChanged -= new EventHandler(listView_EditorChanged);
			}
			UnsubscribeFromLinkListViewBindingList();
			UnsubscribeFromListViewEditor();
		}
		protected override void Dispose(bool disposing) {
			try {
				if(Frame != null && Frame.Application != null) {
					Frame.Application.ViewShowing -= new EventHandler<ViewShowingEventArgs>(Application_ViewShowing);
				}
				if(disposing && components != null) {
					components.Dispose();
				}
				UnsubscribeFromLinkListViewBindingList();
				UnsubscribeFromListViewEditor();
			}
			finally {
				base.Dispose(disposing);
			}
		}
		protected override void OnFrameAssigned() {
			base.OnFrameAssigned();
			if(Frame != null && Frame.Application != null) {
				Frame.Application.ViewShowing += new EventHandler<ViewShowingEventArgs>(Application_ViewShowing);
			}
		}
		protected virtual Boolean MoveToPrevious() {
			if(!View.IsRoot || ObjectView.CanChangeCurrentObject()) {
				int currentObjectIndex = GetCurrentObjectIndex();
				if(currentObjectIndex >= 0) {
					Object obj = GetObjectByIndex(currentObjectIndex - 1);
					if(CanMoveToObject(obj)) {
						if((ListView != null) && (ListView.CollectionSource.DataAccessMode == CollectionSourceDataAccessMode.DataView)) {
							ListView.CurrentObject = obj;
						}
						else {
							ObjectView.CurrentObject = ObjectSpace.GetObject(obj);
						}
						return true;
					}
				}
			}
			return false;
		}
		protected virtual Boolean MoveToNext() {
			if(!View.IsRoot || ObjectView.CanChangeCurrentObject()) {
				int currentObjectIndex = GetCurrentObjectIndex();
				if(currentObjectIndex >= 0) {
					Object obj = GetObjectByIndex(currentObjectIndex + 1);
					if(CanMoveToObject(obj)) {
						if((ListView != null) && (ListView.CollectionSource.DataAccessMode == CollectionSourceDataAccessMode.DataView)) {
							ListView.CurrentObject = obj;
						}
						else {
							ObjectView.CurrentObject = ObjectSpace.GetObject(obj);
						}
						return true;
					}
				}
			}
			return false;
		}
		public RecordsNavigationController()
			: base() {
			InitializeComponent();
			TypeOfView = typeof(ObjectView);
			RegisterActions(components);
		}
		public virtual MovementDirection CanMoveBy() {
			MovementDirection result = MovementDirection.None;
			if(View != null) {
				Object nextObject = null;
				Object prevObject = null;
				if((ObjectView.CurrentObject != null)) {
					int currentObjectIndex = GetCurrentObjectIndex();
					if(currentObjectIndex >= 0) {
						nextObject = GetObjectByIndex(currentObjectIndex + 1);
						prevObject = GetObjectByIndex(currentObjectIndex - 1);
					}
				}
				if(CanMoveToObject(nextObject)) {
					result |= MovementDirection.Forward;
				}
				if(CanMoveToObject(prevObject)) {
					result |= MovementDirection.Previous;
				}
			}
			return result;
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("RecordsNavigationControllerOrderProviderSource")]
#endif
		public OrderProviderSource OrderProviderSource {
			get { return orderProviderSource; }
			set {
				orderProviderSource = value;
				UpdateActionState();
			}
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("RecordsNavigationControllerPreviousObjectAction")]
#endif
		public SimpleAction PreviousObjectAction {
			get { return previousObjectAction; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("RecordsNavigationControllerNextObjectAction")]
#endif
		public SimpleAction NextObjectAction {
			get { return nextObjectAction; }
		}
	}
	public class OrderProviderSource {
		private IOrderProvider orderProvider;
		public IOrderProvider OrderProvider {
			get { return orderProvider; }
			set {
				if(value is IObjectOrderProvider || value is IKeyOrderProvider) {
					orderProvider = value;
				}
				else {
					throw new ArgumentOutOfRangeException("OrderProvider", value, "The OrderProvider has to support the IObjectOrderProvider or the IKeyOrderProvider interface");
				}
			}
		}
	}
	public interface IOrderProvider {
		Object GetObjectByIndex(Int32 index);
		IList GetOrderedObjects();
	}
	public interface IObjectOrderProvider : IOrderProvider {
		Int32 GetIndexByObject(Object obj);
	}
	public interface IKeyOrderProvider : IOrderProvider {
		Int32 GetIndexByObjectKey(Object objectKeyValue);
	}
	public interface IControlOrderProvider : IOrderProvider {
		Int32 GetIndexByObject(Object obj);
	}
	public class DefaultOrderProvider : IObjectOrderProvider {
		private View view;
		private Frame frame;
		public DefaultOrderProvider(View view, Frame frame) {
			this.view = view;
			this.frame = frame;
		}
		public int GetIndexByObject(Object obj) {
			ListView listView = view as ListView;
			if(IsListViewCollectionSourceAvailable(listView)) {
				Object currentObj = listView.ObjectSpace.GetObject(obj);
				if(currentObj != null) {
					return listView.CollectionSource.List.IndexOf(currentObj);
				}
			}
			return -1;
		}
		public Object GetObjectByIndex(int index) {
			ListView listView = view as ListView;
			if(IsListViewCollectionSourceAvailable(listView)) {
				if((index >= 0) && (index < listView.CollectionSource.List.Count)) {
					return listView.CollectionSource.List[index];
				}
			}
			return null;
		}
		public IList GetOrderedObjects() {
			ListView listView = view as ListView;
			if(IsListViewCollectionSourceAvailable(listView)) {
				return listView.CollectionSource.List;
			}
			return null;
		}
		private bool IsListViewCollectionSourceAvailable(ListView listView) {
			if((listView != null)
				&&
				((frame == null) || !frame.IsViewControllersActivation)
				) {
				return true;
			}
			return false;
		}
	}
	public class NullOrderProvider : IObjectOrderProvider {
		public Object GetObjectByIndex(int index) {
			return null;
		}
		public IList GetOrderedObjects() {
			return null;
		}
		public int GetIndexByObject(Object obj) {
			return -1;
		}
	}
	public class StandaloneOrderProvider : IKeyOrderProvider {
		private bool valid = true;
		private ArrayList objectKeys = new ArrayList();
		public StandaloneOrderProvider(IObjectSpace objectSpace, IList objects) {
			Initialize(objectSpace, objects);
		}
		private void Initialize(IObjectSpace objectSpace, IList objects) {
			if(objects != null) {
				foreach(Object obj in objects) {
					Object key = objectSpace.GetKeyValue(obj);
					objectKeys.Add(key);
					if(key == null) {
						valid = false;
					}
				}
			}
		}
		public int GetIndexByObjectKey(Object objectKey) {
			return objectKeys.IndexOf(objectKey);
		}
		public Object GetObjectKeyByIndex(int index) {
			if((index >= 0) && (index < objectKeys.Count)) {
				return objectKeys[index];
			}
			return null;
		}
		public Object GetObjectByIndex(int index) {
			return null;
		}
		public IList GetOrderedObjects() {
			return new ArrayList();
		}
		public bool Valid {
			get { return valid; }
		}
	}
	public class ListEditorOrderProvider : IObjectOrderProvider {
		private IObjectOrderProvider defaultOrderProvider;
		private ListView listView;
		private IControlOrderProvider ControlOrderProvider {
			get {
				if(listView != null) {
					return listView.Editor as IControlOrderProvider;
				}
				return null;
			}
		}
		public ListEditorOrderProvider(ListView listView, IObjectOrderProvider defaultOrderProvider) {
			this.defaultOrderProvider = defaultOrderProvider;
			if(this.defaultOrderProvider == null) {
				this.defaultOrderProvider = new NullOrderProvider();
			}
			this.listView = listView;
		}
		public int GetIndexByObject(Object obj) {
			if(ControlOrderProvider != null) {
				Object currentObj = null;
				currentObj = listView.GetObject(obj);
				if(currentObj != null) {
					return ControlOrderProvider.GetIndexByObject(currentObj);
				}
			}
			else { 
				return defaultOrderProvider.GetIndexByObject(obj);
			}
			return -1;
		}
		public Object GetObjectByIndex(int index) {
			if(ControlOrderProvider != null) {
				return ControlOrderProvider.GetObjectByIndex(index);
			}
			else {
				return defaultOrderProvider.GetObjectByIndex(index);
			}
		}
		public IList GetOrderedObjects() {
			if(ControlOrderProvider != null) {
				return ControlOrderProvider.GetOrderedObjects();
			}
			else {
				return defaultOrderProvider.GetOrderedObjects();
			}
		}
	}
}
