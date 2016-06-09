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
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp {
	public abstract class CompositeView : View {
		private static Dictionary<String, Int32> recursionBreak;
		public static Int32 MaxRecursionDeep = 50;
		private LayoutManager layoutManager;
		private XafApplication application;
		private ViewItemsCollection items;
		private Boolean delayedItemsInitialization;
		private IObjectSpace objectSpace;
		private Boolean skipObjectSpaceDisposing;
		private void SetLayoutManagerDelayedItemsInitialization() {
			if(layoutManager != null) {
				layoutManager.DelayedItemsInitialization = delayedItemsInitialization;
			}
		}
		private void ErrorMessages_MessagesChanged(Object sender, EventArgs e) {
			UpdateControlErrorMessages();
		}
		private void DisposeObjectSpace() {
			if(objectSpace != null) {
				if(IsRoot) {
					objectSpace.Owner = null;
					if(!skipObjectSpaceDisposing) {
						objectSpace.Dispose();
					}
				}
				objectSpace = null;
			}
		}
		protected ViewItemsCollection ItemsCollection {
			get { return items; }
		}
		protected XafApplication Application {
			get { return application; }
		}
		protected virtual Boolean IsItemsChangingSupported() {
			return true;
		}
		protected void CreateViewItemControls() {
			foreach(ViewItem item in items) {
				if(item.Control == null) {
					item.CreateControl();
				}
			}
		}
		protected virtual ViewItem CreateItem(IModelViewItem info) {
			if(Application != null) {
				return Application.EditorFactory.CreateDetailViewEditor(false, info, null, Application, ObjectSpace);
			}
			else {
				return null;
			}
		}
		protected void ClearItems() {
			SafeExecutor executor = new SafeExecutor(this);
			foreach(ViewItem item in new List<ViewItem>(items)) {
				executor.Dispose(item, item.Id);
			}
			items.Clear();
		}
		protected override void RefreshCore() {
			foreach(ViewItem item in items) {
				item.Refresh();
			}
		}
		protected virtual void InitializeItem(ViewItem item) {
			if(!delayedItemsInitialization) {
				if(IsControlCreated && (item.Control == null)) {
					item.CreateControl();
				}
				else if(item is IFrameContainer) {
					((IFrameContainer)item).InitializeFrame();
				}
			}
		}
		protected virtual void OnItemsChanged(ViewItem item, ViewItemsChangedType changedType) {
			ViewItemsChangedEventArgs e = new ViewItemsChangedEventArgs(changedType, item);
			ItemChangedInternal(e.Item, e.ChangedType);
			if(ItemsChanged != null) {
				ItemsChanged(this, e);
			}
		}
		protected virtual void ItemChangedInternal(ViewItem item, ViewItemsChangedType changeType) { }
		protected override void DisposeCore() {
			ItemsChanged = null;
			if(ErrorMessages != null) {
				ErrorMessages.MessagesChanged -= new EventHandler(ErrorMessages_MessagesChanged);
			}
			if((layoutManager != null) && (layoutManager.Container is ISupportUpdate)) {
				((ISupportUpdate)layoutManager.Container).BeginUpdate();
			}
			ClearItems();
			if((layoutManager != null) && (layoutManager.Container is ISupportUpdate)) {
				layoutManager.ClearLayoutItems();
				((ISupportUpdate)layoutManager.Container).EndUpdate();
			}
			SafeExecutor executor = new SafeExecutor(this);
			if(layoutManager != null) {
				executor.Dispose(layoutManager);
				layoutManager = null;
			}
			executor.ThrowExceptionIfAny();
			DisposeObjectSpace();
			base.DisposeCore();
		}
		protected virtual Boolean GetIsLayoutSimple() {
			return false;
		}
		protected override void SaveModelCore() {
			if(LayoutManager != null) {
				LayoutManager.SaveModel();
			}
			foreach(ViewItem item in Items) {
				item.SaveModel();
			}
		}
		protected override void LoadModelCore() {
			if(Model is IModelCompositeView) {
				ClearItems();
				lock(DevExpress.ExpressApp.DC.TypesInfo.lockObject) {
					Boolean isFirstEntry = (recursionBreak == null);
					try {
						if(isFirstEntry) {
							recursionBreak = new Dictionary<string, int>();
						}
						lock (DevExpress.ExpressApp.DC.TypesInfo.lockObject) {
							if(!recursionBreak.ContainsKey(Id)) {
								recursionBreak.Add(Id, 1);
							}
							else {
								if(recursionBreak[Id] > MaxRecursionDeep) {
									throw new InfiniteRecursionException(string.Format(
										SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.InfiniteRecursionDetected),
										Id, recursionBreak[Id]));
								}
								recursionBreak[Id] = recursionBreak[Id] + 1;
							}
							foreach(IModelViewItem editorInfo in ((IModelCompositeView)Model).Items) {
								AddItem(editorInfo);
							}
						}
					}
					finally {
						if(isFirstEntry) {
							recursionBreak = null;
						}
					}
				}
			}
		}
		protected virtual void UpdateControlErrorMessages() {
			foreach(ViewItem item in Items) {
				item.UpdateErrorMessage(ErrorMessages);
			}
		}
		protected override Object CreateControlsCore() {
			if(LayoutManager != null) {
				if(!delayedItemsInitialization) {
					CreateViewItemControls();
				}
				if(!ErrorMessages.IsEmpty) {
					UpdateControlErrorMessages();
				}
				if(Model is IModelCompositeView) {
					return LayoutManager.LayoutControls(((IModelCompositeView)Model).Layout, ItemsCollection);
				}
				else if(Model is IModelListView) {
					return LayoutManager.LayoutControls(((IModelListView)Model).SplitLayout, ItemsCollection);
				}
			}
			return null;
		}
		protected CompositeView(IObjectSpace objectSpace, XafApplication application, Boolean isRoot)
			: base(isRoot) {
			this.application = application;
			items = new ViewItemsCollection();
			if(application != null) {
				layoutManager = application.CreateLayoutManager(GetIsLayoutSimple());
			}
			SetLayoutManagerDelayedItemsInitialization();
			ErrorMessages.MessagesChanged += new EventHandler(ErrorMessages_MessagesChanged);
			this.objectSpace = objectSpace;
			if(objectSpace != null && isRoot) {
				if(objectSpace.Owner == null) {
					objectSpace.Owner = this;
				}
				else {
					throw new ArgumentException(
						SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.AlreadyUsedObjectSpace, ((View)objectSpace.Owner).ToString()),
						"objectSpace");
				}
			}
		}
		public CompositeView(XafApplication application, Boolean isRoot)
			: this(null, application, isRoot) { }
		public ViewItem AddItem(IModelViewItem info) {
			if(IsItemsChangingSupported()) {
				ViewItem item = CreateItem(info);
				AddItem(item);
				return item;
			}
			else {
				return null;
			}
		}
		public void AddItem(ViewItem item) {
			if(IsItemsChangingSupported()) {
				items.Add(item);
				item.View = this;
				InitializeItem(item);
				OnItemsChanged(item, ViewItemsChangedType.Added);
			}
		}
		public ViewItem InsertItem(Int32 index, IModelViewItem info) {
			if(IsItemsChangingSupported()) {
				ViewItem item = CreateItem(info);
				InsertItem(index, item);
				return item;
			}
			else {
				return null;
			}
		}
		public void InsertItem(Int32 index, ViewItem item) {
			if(IsItemsChangingSupported()) {
				items.Insert(index, item.Id, item);
				item.View = this;
				InitializeItem(item);
				OnItemsChanged(item, ViewItemsChangedType.Added);
			}
		}
		public void RemoveItem(String id) {
			if(IsItemsChangingSupported()) {
				id = LayoutManager.RemoveUniqueSuffix(id);
				ViewItem item = items[id];
				if(item != null) {
					items.Remove(id);
					OnItemsChanged(item, ViewItemsChangedType.Removed);
					item.Dispose();
				}
				if(Model != null) {
					IModelViewItem itemToRemove = ((IModelCompositeView)Model).Items[id];
					if(itemToRemove != null) {
						itemToRemove.Remove();
					}
				}
			}
		}
		public void UpdateItem(ViewItem newItem) {
			if(IsItemsChangingSupported()) {
				string id = LayoutManager.RemoveUniqueSuffix(newItem.Id);
				ViewItem item = items[id];
				if(item != null) {
					items.Remove(id);
					OnItemsChanged(item, ViewItemsChangedType.Removed);
				}
				AddItem(newItem);
				if((newItem.Control == null) && delayedItemsInitialization && (item.Control != null)) {
					newItem.CreateControl();
				}
				if(item != null) {
					LayoutManager.UpdateViewItem(newItem);
					item.Dispose();
				}
			}
		}
		public ViewItem FindItem(String id) {
			id = LayoutManager.RemoveUniqueSuffix(id);
			return items[id];
		}
		public IList<T> GetItems<T>() where T : class {
			List<T> result = new List<T>();
			foreach(ViewItem item in items) {
				T typedItem = item as T;
				if(typedItem != null) {
					result.Add(typedItem);
				}
			}
			return result.AsReadOnly();
		}
		public override void BreakLinksToControls() {
			foreach(ViewItem item in items) {
				item.BreakLinksToControl(false);
			}
			if(layoutManager != null) {
				layoutManager.BreakLinksToControls();
			}
			base.BreakLinksToControls();
		}
		public LayoutManager LayoutManager {
			get { return layoutManager; }
		}
		public IList<ViewItem> Items {
			get { return items.GetValues().AsReadOnly(); }
		}
		public Boolean DelayedItemsInitialization {
			get { return delayedItemsInitialization; }
			set {
				delayedItemsInitialization = value;
				SetLayoutManagerDelayedItemsInitialization();
			}
		}
		public override IObjectSpace ObjectSpace {
			get { return objectSpace; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Boolean SkipObjectSpaceDisposing {
			get { return skipObjectSpaceDisposing; }
			set { skipObjectSpaceDisposing = value; }
		}
		public event EventHandler<ViewItemsChangedEventArgs> ItemsChanged;
	}
}
