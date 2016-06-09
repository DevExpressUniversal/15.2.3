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
using DevExpress.Data.Filtering.Helpers;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;
namespace DevExpress.ExpressApp.ConditionalAppearance {
	public class AppearanceCustomizationListenerController : ViewController {
		void appearanceCustomizationController_CustomizeAppearance(object sender, CustomizeAppearanceEventArgs e) {
			if(Active) {
				HandleCustomizeAppearance(e);
			}
		}
		protected virtual void HandleCustomizeAppearance(CustomizeAppearanceEventArgs e) {
			AppearanceController controller = Frame.GetController<AppearanceController>();
			if(controller != null && e.Name != null && e.Item != null) {
				string itemName = ProcessItemName(e.Name);
				IViewInfo viewInfo;
				if(e.ViewInfo != null) {
					viewInfo = e.ViewInfo;
				}
				else {
					viewInfo = ViewInfo.FromView(View as ObjectView);
				}
				controller.RefreshItemAppearance(viewInfo, e.AppearanceItemType, itemName, e.Item, e.ContextObject, e.EvaluatorContextDescriptor);
			}
		}
		protected virtual string ProcessItemName(string itemName) {
			if(!string.IsNullOrEmpty(itemName)) {
				return itemName.TrimEnd('!'); 
			}
			return itemName;
		}
		protected override void OnFrameAssigned() {
			base.OnFrameAssigned();
			foreach(Controller c in Frame.Controllers) {
				ISupportAppearanceCustomization appearanceCustomizationController = c as ISupportAppearanceCustomization;
				if(appearanceCustomizationController != null) {
					appearanceCustomizationController.CustomizeAppearance += new EventHandler<CustomizeAppearanceEventArgs>(appearanceCustomizationController_CustomizeAppearance);
				}
			}
		}
	}
	public class ListViewItemAppearanceController : RefreshItemsAppearanceControllerBase<ListView> {
		private DataViewEvaluatorContextDescriptor dataViewEvaluatorContextDescriptor;
		private AppearanceItemsContainer items = new AppearanceItemsContainer();
		private void ListViewItemAppearanceController_CustomizeAppearance(Object sender, CustomizeAppearanceEventArgs e) {
			if((e.Name != null) && (e.Item != null)) {
				String itemName = e.Name.TrimEnd('!');
				EvaluatorContextDescriptor evaluatorContextDescriptor = e.EvaluatorContextDescriptor;
				if((evaluatorContextDescriptor == null) && (View.CollectionSource != null)
						&& (View.CollectionSource.DataAccessMode == CollectionSourceDataAccessMode.DataView)) {
					evaluatorContextDescriptor = GetDataViewEvaluatorContextDescriptor();
				}
				AppearanceController.RefreshItemAppearance(ViewInfo.FromView(View), AppearanceItemType, itemName, e.Item, e.ContextObject, evaluatorContextDescriptor);
			}
		}
		private EvaluatorContextDescriptor GetDataViewEvaluatorContextDescriptor() {
			if(dataViewEvaluatorContextDescriptor == null) {
				dataViewEvaluatorContextDescriptor = new DataViewEvaluatorContextDescriptor(ObjectSpace);
			}
			return dataViewEvaluatorContextDescriptor;
		}
		private void View_EditorChanged(object sender, EventArgs e) {
			SubscribeAppearanceRequest();
		}
		private void View_EditorChanging(object sender, EventArgs e) {
			UnsubscribeAppearanceRequest();
		}
		private void SubscribeAppearanceRequest() {
			if(View.Editor is ISupportAppearanceCustomization) {
				((ISupportAppearanceCustomization)(View.Editor)).CustomizeAppearance += new EventHandler<CustomizeAppearanceEventArgs>(ListViewItemAppearanceController_CustomizeAppearance);
			}
		}
		private void UnsubscribeAppearanceRequest() {
			if(View.Editor is ISupportAppearanceCustomization) {
				((ISupportAppearanceCustomization)(View.Editor)).CustomizeAppearance -= new EventHandler<CustomizeAppearanceEventArgs>(ListViewItemAppearanceController_CustomizeAppearance);
			}
		}
		private void ListViewItemAppearanceController_MasterObjectChanged(object sender, EventArgs e) {
			RefreshListEditorColumnsAppearance();
		}
		protected virtual void RefreshListEditorColumnsAppearance() {
			items.Clear();
			if(!CanManageColumns) {
				return;
			}
			ColumnsListEditor columnsListEditor = View.Editor as ColumnsListEditor;
			if(columnsListEditor != null && columnsListEditor.Control != null) {
				foreach(ColumnWrapper column in columnsListEditor.Columns) {
					if(!string.IsNullOrEmpty(column.PropertyName)) {
						items.AddItem(column.PropertyName, column);
						AppearanceController.RefreshItemAppearance(View, AppearanceItemType, column.PropertyName, column, null);
					}
				}
			}
		}
#if DebugTest
		internal void ViewControlsCreatedd() {
			OnViewControlsCreated();
		}
#endif
		protected override IList<AppearanceContainer> Items {
			get { return items.Items; }
		}
		protected override void OnViewControlsCreated() {
			base.OnViewControlsCreated();
			RefreshListEditorColumnsAppearance();
		}
		protected override void OnViewControlsDestroying() {
			base.OnViewControlsDestroying();
			items.Clear();
		}
		protected override void OnActivated() {
			SubscribeAppearanceRequest();
			View.EditorChanging += new EventHandler(View_EditorChanging);
			View.EditorChanged += new EventHandler(View_EditorChanged);
			if(View.CollectionSource is PropertyCollectionSource) {
				((PropertyCollectionSource)View.CollectionSource).MasterObjectChanged += new EventHandler(ListViewItemAppearanceController_MasterObjectChanged);
			}
			RefreshListEditorColumnsAppearance();
			base.OnActivated();
		}
		protected override void OnDeactivated() {
			dataViewEvaluatorContextDescriptor = null;
			items.Clear();
			View.EditorChanging -= new EventHandler(View_EditorChanging);
			View.EditorChanged -= new EventHandler(View_EditorChanged);
			if(View.CollectionSource is PropertyCollectionSource) {
				((PropertyCollectionSource)View.CollectionSource).MasterObjectChanged -= new EventHandler(ListViewItemAppearanceController_MasterObjectChanged);
			}
			UnsubscribeAppearanceRequest();
			base.OnDeactivated();
		}
		protected override string AppearanceItemType {
			get { return AppearanceController.AppearanceViewItemType; }
		}
		public ListViewItemAppearanceController() {
			CanManageColumns = true;
		}
		[DefaultValue(true)]
		public bool CanManageColumns { get; set; }
	}
	public abstract class RefreshItemsAppearanceControllerBase<T> : ViewController<T>, ISupportRefreshItemsAppearance where T : ObjectView {
		protected virtual AppearanceController AppearanceController {
			get { return Frame.GetController<AppearanceController>(); }
		}
		protected override void OnActivated() {
			base.OnActivated();
			AppearanceController.RegisterController(this);
		}
		protected override void OnDeactivated() {
			AppearanceController.UnRegisterController(this);
			base.OnDeactivated();
		}
#if (DebugTest)
		public IList<AppearanceContainer> ItemsForTest {
			get {
				return Items;
			}
		}
		public void DebugTest_RefreshViewItemsAppearanceCore(IList<AppearanceContainer> items, ViewInfo view, object currentObject) {
			RefreshViewItemsAppearanceCore(items, view, currentObject);
		}
#endif
		protected abstract IList<AppearanceContainer> Items { get; }
		protected abstract string AppearanceItemType { get; }
		public void RefreshViewItemsAppearance() {
			ISupportUpdate supportUpdate = null;
			if(View.LayoutManager != null) {
				supportUpdate = View.LayoutManager.Container as ISupportUpdate;
			}
			try {
				if(supportUpdate != null) {
					supportUpdate.BeginUpdate();
				}
				RefreshViewItemsAppearanceCore(Items, ViewInfo.FromView(View), View.CurrentObject);
			}
			finally {
				if(supportUpdate != null) {
					supportUpdate.EndUpdate();
				}
			}
		}
		private void RefreshViewItemsAppearanceCore(IList<AppearanceContainer> items, ViewInfo viewInfo, object currentObject) {
			foreach(AppearanceContainer item in items) {
				AppearanceController.RefreshItemAppearance(viewInfo, AppearanceItemType, item.Name, item.Item, currentObject, null);
			}
		}
	}
	public class DetailViewItemAppearanceController : RefreshItemsAppearanceControllerBase<DetailView> {
		private void SubscribeControlCreated() {
			foreach(ViewItem item in View.Items) {
				item.ControlCreated += new EventHandler<EventArgs>(viewItem_ControlCreated);
			}
		}
		private void UnSubscribeControlCreated() {
			foreach(ViewItem item in View.Items) {
				item.ControlCreated -= new EventHandler<EventArgs>(viewItem_ControlCreated);
			}
		}
		private void LayoutManager_LayoutCreated(object sender, EventArgs e) {
			RefreshViewItemsAppearance();
			UnSubscribeControlCreated();
			SubscribeControlCreated();
		}
		private void View_ItemsChanged(object sender, ViewItemsChangedEventArgs e) {
			if(e.ChangedType == ViewItemsChangedType.Added) {
				e.Item.ControlCreated += viewItem_ControlCreated;
			}
		}
		private void viewItem_ControlCreated(object sender, EventArgs e) {
			ViewItem viewItem = sender as ViewItem;
			if(viewItem != null) {
				viewItem.ControlCreated -= new EventHandler<EventArgs>(viewItem_ControlCreated);
				AppearanceController.RefreshItemAppearance(View, AppearanceItemType, viewItem.Id, viewItem, View.CurrentObject);
			}
		}
		protected override IList<AppearanceContainer> Items {
			get {
				AppearanceItemsContainer items = new AppearanceItemsContainer();
				foreach(ViewItem item in View.Items) {
					items.AddItem(item.Id, item);
				}
				return items.Items;
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			View.LayoutManager.LayoutCreated += new EventHandler(LayoutManager_LayoutCreated);
			View.ItemsChanged += new EventHandler<ViewItemsChangedEventArgs>(View_ItemsChanged);
		}
		protected override void OnDeactivated() {
			View.LayoutManager.LayoutCreated -= new EventHandler(LayoutManager_LayoutCreated);
			View.ItemsChanged -= new EventHandler<ViewItemsChangedEventArgs>(View_ItemsChanged);
			UnSubscribeControlCreated();
			base.OnDeactivated();
		}
		protected override string AppearanceItemType {
			get { return AppearanceController.AppearanceViewItemType; }
		}
	}
	public class DetailViewLayoutItemAppearanceController : RefreshItemsAppearanceControllerBase<DetailView> {
		AppearanceItemsContainer items = new AppearanceItemsContainer();
		private void LayoutManager_CustomizeAppearance(object sender, CustomizeAppearanceEventArgs e) {
			items.AddItem(e.Name, e.Item);
			AppearanceController.RefreshItemAppearance(View, AppearanceItemType, e.Name, e.Item, View.CurrentObject);
		}
		protected override void OnActivated() {
			base.OnActivated();
			((ISupportAppearanceCustomization)View.LayoutManager).CustomizeAppearance += new EventHandler<CustomizeAppearanceEventArgs>(LayoutManager_CustomizeAppearance);
		}
		protected override void OnDeactivated() {
			items.Clear();
			((ISupportAppearanceCustomization)View.LayoutManager).CustomizeAppearance -= new EventHandler<CustomizeAppearanceEventArgs>(LayoutManager_CustomizeAppearance);
			base.OnDeactivated();
		}
		protected override IList<AppearanceContainer> Items {
			get { return items.Items; }
		}
		protected override void OnViewControlsDestroying() {
			base.OnViewControlsDestroying();
			items.Clear();
		}
		protected override string AppearanceItemType {
			get { return AppearanceController.AppearanceLayoutItemType; }
		}
	}
	public struct AppearanceContainer {
		public string Name;
		public object Item;
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class AppearanceItemsContainer {
		private List<AppearanceContainer> items = new List<AppearanceContainer>();
		protected internal IList<AppearanceContainer> Items {
			get {
				return items;
			}
		}
#if DebugTest
		public IList<AppearanceContainer> DebugTest_Items {
			get { return Items; }
		}
		public void DebugTest_AddItem(string name, object item) {
			AddItem( name, item) ;
		}
		public void DebugTest_RemoveItem(string name) {
			RemoveItem(name) ;
		}
		public void DebugTest_Clear() {
			Clear();
		}
#endif
		protected internal void AddItem(string name, object item) {
			AppearanceContainer aContainer = new AppearanceContainer();
			aContainer.Name = name;
			aContainer.Item = item;
			items.Add(aContainer);
		}
		protected internal void RemoveItem(string name) {
			for(int counter = items.Count - 1; counter >= 0; counter--) {
				if(items[counter].Name == name) {
					items.RemoveAt(counter);
				}
			}
		}
		protected internal void Clear() {
			items.Clear();
		}
	}
}
