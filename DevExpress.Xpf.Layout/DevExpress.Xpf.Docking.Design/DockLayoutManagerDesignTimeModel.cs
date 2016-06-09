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
using System.Collections.ObjectModel;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Interaction;
using DevExpress.Xpf.Docking.Base;
using DevExpress.Design.UI;
using DevExpress.Xpf.Docking.Platform;
using DevExpress.Xpf.Layout.Core;
using DevExpress.Xpf.Docking.VisualElements;
using LayoutCore = System;
namespace DevExpress.Xpf.Docking.Design {
	public class DockLayoutManagerDesignTimeModel : DependencyObject, LayoutCore.IObserver<NotificationEventArgs> {
		#region static
		public static readonly DependencyProperty IsAdornerPanelExpandedProperty;
		public static readonly DependencyProperty IsLayoutTreeExpanedProperty;
		public static readonly DependencyProperty SelectedModelItemProperty;
		public static readonly DependencyProperty SelectedTypeProperty;
		public static readonly DependencyProperty AvailableLayoutItemsProperty;
		public static readonly DependencyProperty SelectedLayoutItemProperty;
		public static readonly DependencyProperty IsLayoutItemSelectedProperty;
		public static readonly DependencyProperty IsDockItemSelectedProperty;
		public static readonly DependencyProperty DockItemTypeProperty;
		public static readonly DependencyProperty LayoutItemTypeProperty;
		static DockLayoutManagerDesignTimeModel() {
			Type ownerType = typeof(DockLayoutManagerDesignTimeModel);
			var dProp = new DependencyPropertyRegistrator<DockLayoutManagerDesignTimeModel>();
			dProp.Register("SelectedModelItem", ref SelectedModelItemProperty, (ModelItem)null,
				(dObj, e) => ((DockLayoutManagerDesignTimeModel)dObj).OnSelectedModelItemChanged((ModelItem)e.NewValue));
			dProp.Register("SelectedLayoutItem", ref SelectedLayoutItemProperty, (BaseLayoutItem)null,
				(dObj, e) => ((DockLayoutManagerDesignTimeModel)dObj).OnSelectedLayoutItemChanged((BaseLayoutItem)e.NewValue));
			dProp.Register("SelectedType", ref SelectedTypeProperty, LayoutTypes.LayoutPanel,
				(dObj, e) => ((DockLayoutManagerDesignTimeModel)dObj).OnSelectedTypeChanged((LayoutTypes)e.NewValue));
			dProp.Register("IsAdornerPanelExpanded", ref IsAdornerPanelExpandedProperty, true);
			dProp.Register("IsLayoutTreeExpanded", ref IsLayoutTreeExpanedProperty, true);
			dProp.Register("AvailableLayoutItems", ref AvailableLayoutItemsProperty, (ObservableCollection<LayoutItemTypeInfo>)null);
			dProp.Register("IsLayoutItemSelected", ref IsLayoutItemSelectedProperty, false);
			dProp.Register("IsDockItemSelected", ref IsDockItemSelectedProperty, false,
				(dObj, e) => ((DockLayoutManagerDesignTimeModel)dObj).OnIsDockItemSelectedChanged((bool)e.NewValue),
				(dObj, value) => ((DockLayoutManagerDesignTimeModel)dObj).CoerceIsDockItemSelected((bool)value));
			dProp.Register("DockItemType", ref DockItemTypeProperty, LayoutTypes.LayoutPanel);
			dProp.Register("LayoutItemType", ref LayoutItemTypeProperty, LayoutTypes.ControlItem);
		}
		#endregion static
		public void ActualizeModel(EditingContext context, ModelItem adornedElement, DockLayoutManager manager) {
			Context = context;
			Manager = manager;
			AdornedElement = adornedElement;
		}
		public EditingContext Context { get; private set; }
		public DockLayoutManager Manager { get; private set; }
		public ModelItem AdornedElement { get; private set; }
		public ModelItem SelectedModelItem {
			get { return (ModelItem)GetValue(SelectedModelItemProperty); }
			set { SetValue(SelectedModelItemProperty, value); }
		}
		public LayoutTypes SelectedType {
			get { return (LayoutTypes)GetValue(SelectedTypeProperty); }
			set { SetValue(SelectedTypeProperty, value); }
		}
		public ObservableCollection<LayoutItemTypeInfo> AvailableLayoutItems {
			get { return (ObservableCollection<LayoutItemTypeInfo>)GetValue(AvailableLayoutItemsProperty); }
			private set { SetValue(AvailableLayoutItemsProperty, value); }
		}
		public bool IsLayoutItemSelected {
			get { return (bool)GetValue(IsLayoutItemSelectedProperty); }
			private set { SetValue(IsLayoutItemSelectedProperty, value); }
		}
		public bool IsAdornerPanelExpanded {
			get { return (bool)GetValue(IsAdornerPanelExpandedProperty); }
			set { SetValue(IsAdornerPanelExpandedProperty, value); }
		}
		public bool IsLayoutTreeExpaned {
			get { return (bool)GetValue(IsLayoutTreeExpanedProperty); }
			set { SetValue(IsLayoutTreeExpanedProperty, value); }
		}
		public BaseLayoutItem SelectedLayoutItem {
			get { return (BaseLayoutItem)GetValue(SelectedLayoutItemProperty); }
			set { SetValue(SelectedLayoutItemProperty, value); }
		}
		public LayoutTypes DockItemType {
			get { return (LayoutTypes)GetValue(DockItemTypeProperty); }
			set { SetValue(DockItemTypeProperty, value); }
		}
		public LayoutTypes LayoutItemType {
			get { return (LayoutTypes)GetValue(LayoutItemTypeProperty); }
			set { SetValue(LayoutItemTypeProperty, value); }
		}
		public event SelectedModelItemChangedEventHanler SelectedModelItemChanged;
		public event SelectedLayoutItemChangedEventHanler SelectedLayoutItemChanged;
		bool IsTabbed(ModelItem model) {
			return model.Is<LayoutGroup>() && (model.Is<TabbedGroup>() || object.Equals(model.Properties["GroupBorderStyle"].ComputedValue, GroupBorderStyle.Tabbed));
		}
		protected virtual void OnSelectedModelItemChanged(ModelItem value) {
			if(SelectedModelItemChanged != null)
				SelectedModelItemChanged(this, value);
			if(value != null) {
				SelectedLayoutItem = value.As<BaseLayoutItem>();
				IsLayoutItemSelected = value.Is<BaseLayoutItem>();
				Dispatcher.BeginInvoke(new Action(() =>
				{
					if(IsTabbed(value.Parent))
						LayoutGroupDesignModeValueProvider.SetDesignTimeSelectedTabIndex(value.Parent, value.Parent.ItemsProperty().IndexOf(value));
				}));
			}
			else {
				SelectedLayoutItem = null;
				IsLayoutItemSelected = false;
			}
		}
		protected virtual void OnSelectedLayoutItemChanged(BaseLayoutItem item) {
			if(SelectedLayoutItemChanged != null)
				SelectedLayoutItemChanged(this, item);
			CoerceValue(IsDockItemSelectedProperty);
		}
		protected virtual void OnIsDockItemSelectedChanged(bool value) {
			AvailableLayoutItems.Clear();
			if(!value) {
				AvailableLayoutItems.Add(new LayoutItemTypeInfo(SetLayoutItemTypeCommand, LayoutTypes.ControlItem));
				AvailableLayoutItems.Add(new LayoutItemTypeInfo(SetLayoutItemTypeCommand, LayoutTypes.Label));
				AvailableLayoutItems.Add(new LayoutItemTypeInfo(SetLayoutItemTypeCommand, LayoutTypes.EmptySpace));
				AvailableLayoutItems.Add(new LayoutItemTypeInfo(SetLayoutItemTypeCommand, LayoutTypes.Separator));
				AvailableLayoutItems.Add(new LayoutItemTypeInfo(SetLayoutItemTypeCommand, LayoutTypes.Splitter));
				SelectedType = LayoutItemType;
			}
			else {
				AvailableLayoutItems.Add(new LayoutItemTypeInfo(SetLayoutItemTypeCommand, LayoutTypes.LayoutPanel));
				AvailableLayoutItems.Add(new LayoutItemTypeInfo(SetLayoutItemTypeCommand, LayoutTypes.DocumentPanel));
				SelectedType = DockItemType;
			}
		}
		protected virtual object CoerceIsDockItemSelected(bool value) {
			return !SelectedLayoutItem.IsLayoutItem();
		}
		protected virtual void OnSelectedTypeChanged(LayoutTypes type) {
			DependencyProperty targetProperty = SelectedLayoutItem.IsLayoutItem() ?
				LayoutItemTypeProperty : DockItemTypeProperty;
			SetValue(targetProperty, type);
		}
		public ICommand RemoveItemCommand { get; private set; }
		public ICommand LoadLayoutCommand { get; private set; }
		public ICommand SaveLayoutCommand { get; private set; }
		public ICommand SetLayoutItemTypeCommand { get; private set; }
		Dictionary<Notification, Action<NotificationEventArgs>> handlers;
		public DockLayoutManagerDesignTimeModel() {
			AvailableLayoutItems = new ObservableCollection<LayoutItemTypeInfo>() { 
				new LayoutItemTypeInfo(SetLayoutItemTypeCommand, LayoutTypes.LayoutPanel),
				new LayoutItemTypeInfo(SetLayoutItemTypeCommand, LayoutTypes.DocumentPanel)
			};
			InitializeNotificationHandling();
			InitializeCommands();
			CoerceValue(IsDockItemSelectedProperty);
		}
		protected virtual void InitializeNotificationHandling() {
			handlers = new Dictionary<Notification, Action<NotificationEventArgs>>();
			handlers.Add(Notification.Updating, OnUpdating);
			handlers.Add(Notification.Updated, OnUpdated);
			handlers.Add(Notification.Action, OnAction);
		}
		protected virtual void InitializeCommands() {
			RemoveItemCommand = (ICommand)new WpfDelegateCommand<object>(obj => RemoveItem(), null, false);
			SaveLayoutCommand = (ICommand)new WpfDelegateCommand<object>(obj => SaveLayout(), null, false);
			LoadLayoutCommand = (ICommand)new WpfDelegateCommand<object>(obj => LoadLayout(), null, false);
			SetLayoutItemTypeCommand = (ICommand)new WpfDelegateCommand<object>((type) => SetLayoutItemType((LayoutTypes)type), null, false);
		}
		public void SelectModelItemInContext(EditingContext context, ModelItem modelItem) {
			if(context != null && modelItem != null) {
				SelectionOperations.Select(context, modelItem);
			}
		}
		public object Select(BaseLayoutItem item) {
			if(item == null) return null;
			SelectLayoutItemCore(item);
			return item;
		}
		public object Select(LayoutElementHitInfo hitInfo) {
			if(hitInfo.IsEmpty || hitInfo.Element is SplitterElement) return null;
			BaseLayoutItem item = ((IDockLayoutElement)hitInfo.Element).Item;
			if(item == null) return null;
			if(item is ContentItem && hitInfo.InContent) {
				var content = ((ContentItem)item).Content;
				if(content != null) {
					ModelItem contentModel = ModelServiceHelper.FindModelItem<object>(Context, content);
					SelectModelItemInContext(Context, contentModel);
					return content;
				}
				else {
					SelectLayoutItemCore(item);
				}
			}
			SelectLayoutItemCore(item);
			return item;
		}
		void SelectLayoutItemCore(BaseLayoutItem item) {
			if(item.IsTabPage) {
				Action selectAction = new Action(() =>
				{
					ModelItem groupItem = ModelServiceHelper.FindModelItem(Context, item.Parent);
					ModelItem selected = ModelServiceHelper.FindModelItem(Context, item);
					LayoutGroupDesignModeValueProvider.SetDesignTimeSelectedTabIndex(groupItem, groupItem.ItemsProperty().IndexOf(selected));
				});
				Dispatcher.BeginInvoke(selectAction);
			}
			ModelItem layoutItem = ModelServiceHelper.FindModelItem(Context, item);
			SelectModelItemInContext(Context, layoutItem);
		}
		#region IObserver<DockLayoutManagerActionEventArgs> Members
		public void OnCompleted() {
			ResetUpdates();
		}
		public void OnError(Exception error) {
			ResetUpdates();
		}
		public void OnNext(NotificationEventArgs value) {
			Action<NotificationEventArgs> action;
			if(handlers.TryGetValue(value.Notification, out action)) {
				action(value);
			}
		}
		#endregion
		internal bool InUpdating {
			get { return (updates != null); }
		}
		internal void ResetUpdates() {
			if(updates != null) {
				updates.Clear();
				updates = null;
			}
		}
		internal ModelUpdate[] GetUpdatesToExecute() {
			if(updates == null) return new ModelUpdate[0];
			RemoveDuplicateUpdates();
			return updates.ToArray();
		}
		void RemoveDuplicateUpdates() {
			ModelUpdate[] updatesToCheck = updates.ToArray();
			bool fHasUpdate = false;
			foreach(ModelUpdate update in updatesToCheck) {
				if(update is DockLayoutManagerModelUpdate) {
					if(fHasUpdate)
						updates.Remove(update);
					else fHasUpdate = true;
				}
			}
		}
		List<ModelUpdate> updates;
		void OnUpdating(NotificationEventArgs args) {
			var lockService = AdornedElement.Context.Services.GetService<LockService>();
			lockService.Lock();
			updates = new List<ModelUpdate>();
		}
		void OnUpdated(NotificationEventArgs args) {
			if(!InUpdating) return;
			using(ModelEditingScope layouting = AdornedElement.Root.BeginEdit(DockingLocalizer.GetString(DockingStringId.DockingOperations))) {
				Array.ForEach(GetUpdatesToExecute(),
						(update) => update.Execute()
					);
				layouting.Complete();
			}
			ResetUpdates();
			var lockService = AdornedElement.Context.Services.GetService<LockService>();
			lockService.UnLock();
		}
		void OnAction(NotificationEventArgs args) {
			if(!InUpdating) return;
			if(!IsUpdateRegistered(args)) {
				RegisterUpdate(args);
			}
		}
		void RegisterUpdate(NotificationEventArgs args) {
			ModelItem root = ModelServiceHelper.FindModelItem<DockLayoutManager>(Context, Manager);
			if(root != null) {
				ModelUpdate update = ModelUpdateFactory.Create(root, args, Manager);
				if(update != null) updates.Add(update);
			}
		}
		bool IsUpdateRegistered(NotificationEventArgs args) {
			return Array.Exists(updates.ToArray(),
				(update) => (update.Args.ActionTarget == args.ActionTarget) && (update.Args.Property == args.Property));
		}
		public void AddItem(DockTypeValue value) {
			AddItem(SelectedType, value);
		}
		public void AddItem(LayoutTypes type, DockTypeValue dockTypeValue) {
			if(Context == null) return;
			DockLayoutManagerDesignService service = Context.Services.GetService<DockLayoutManagerDesignService>();
			if(service != null) {
				if(SelectedModelItem != null)
					service.AddItem(SelectedModelItem, dockTypeValue, type);
				else
					service.AddItem(ModelServiceHelper.FindModelItem<DockLayoutManager>(Context, Manager), dockTypeValue, type);
			}
		}
		public void RemoveItem() {
			if(Context == null) return;
			DockLayoutManagerDesignService service = Context.Services.GetService<DockLayoutManagerDesignService>();
			if(service != null)
				service.RemoveItem(SelectedModelItem);
		}
		public void SaveLayout() {
			if(Context == null) return;
			DockLayoutManagerDesignService service = Context.Services.GetService<DockLayoutManagerDesignService>();
			if(service != null)
				service.SaveLayout(Manager);
		}
		public void LoadLayout() {
			if(Context == null) return;
			DockLayoutManagerDesignService service = Context.Services.GetService<DockLayoutManagerDesignService>();
			if(service != null)
				service.RestoreLayout(AdornedElement);
			SelectedModelItem = null;
		}
		public void SetLayoutItemType(LayoutTypes type) {
			SelectedType = type;
		}
	}
	public delegate void SelectedModelItemChangedEventHanler (object sender, ModelItem modelItem);
	public delegate void SelectedLayoutItemChangedEventHanler(object sender, BaseLayoutItem item);
	public enum LayoutTypes {
		Group, LayoutPanel, DocumentPanel, DocumentGroup, TabbedGroup,
		GroupBox, LayoutTabbedGroup, LayoutTab,
		ControlItem, Label, Separator, EmptySpace, Splitter
	}
	public class LayoutItemTypeInfo : DependencyObject {
		#region static
		public static LayoutItemTypeInfo GroupTypeInfo {
			get { return new LayoutItemTypeInfo(null, LayoutTypes.Group, ImageUriHelper.GetImageUri(ImageUriHelper.NewGroup), ToolTipHelper.AddGroup); }
		}
		public static LayoutItemTypeInfo PanelTypeInfo {
			get { return new LayoutItemTypeInfo(null, LayoutTypes.LayoutPanel, ImageUriHelper.GetImageUri(ImageUriHelper.NewPanel), ToolTipHelper.AddPanel); }
		}
		public static LayoutItemTypeInfo DocumentGroupTypeInfo {
			get { return new LayoutItemTypeInfo(null, LayoutTypes.DocumentGroup, ImageUriHelper.GetImageUri(ImageUriHelper.NewTab), ToolTipHelper.AddDocumentGroup); }
		}
		public static LayoutItemTypeInfo DocumentTabTypeInfo {
			get { return new LayoutItemTypeInfo(null, LayoutTypes.DocumentPanel, ImageUriHelper.GetImageUri(ImageUriHelper.NewTabbedGroup), ToolTipHelper.AddDocument); }
		}
		public static LayoutItemTypeInfo PanelTabTypeInfo {
			get { return new LayoutItemTypeInfo(null, LayoutTypes.LayoutPanel, ImageUriHelper.GetImageUri(ImageUriHelper.NewTabbedGroup), ToolTipHelper.AddPanel); }
		}
		public static LayoutItemTypeInfo ControlItemTypeInfo {
			get { return new LayoutItemTypeInfo(null, LayoutTypes.ControlItem, ImageUriHelper.GetImageUri(ImageUriHelper.NewItem), ToolTipHelper.AddItem); }
		}
		public static LayoutItemTypeInfo GroupboxTypeInfo {
			get { return new LayoutItemTypeInfo(null, LayoutTypes.GroupBox, ImageUriHelper.GetImageUri(ImageUriHelper.NewGroup), ToolTipHelper.AddGroup); }
		}
		public static LayoutItemTypeInfo LayoutTabbedGroupTypeInfo {
			get { return new LayoutItemTypeInfo(null, LayoutTypes.LayoutTabbedGroup, ImageUriHelper.GetImageUri(ImageUriHelper.NewTab), ToolTipHelper.AddLayoutTab); }
		}
		public static LayoutItemTypeInfo LayoutTabTypeInfo {
			get { return new LayoutItemTypeInfo(null, LayoutTypes.LayoutTab, ImageUriHelper.GetImageUri(ImageUriHelper.NewTabbedGroup), ToolTipHelper.AddGroup); }
		}
		public static readonly DependencyProperty IsEnabledProperty;
		static LayoutItemTypeInfo() {
			var dProp = new DependencyPropertyRegistrator<LayoutItemTypeInfo>();
			dProp.Register("IsEnabled", ref IsEnabledProperty, true);
		}
		#endregion static
		public LayoutItemTypeInfo(ICommand command, LayoutTypes type)
			: this(command, type, null, string.Empty) {
		}
		public LayoutItemTypeInfo(ICommand command, LayoutTypes type, Uri imageUri, string toolTip) {
			Command = command;
			Type = type;
			ImageUri = imageUri;
			if(imageUri != null)
				Image = new System.Windows.Media.Imaging.BitmapImage(imageUri);
			ToolTip = toolTip;
		}
		public bool IsEnabled {
			get { return (bool)GetValue(IsEnabledProperty); }
			set { SetValue(IsEnabledProperty, value); }
		}
		public ICommand Command { get; private set; }
		public LayoutTypes Type { get; private set; }
		public Uri ImageUri { get; private set; }
		public ImageSource Image { get; private set; }
		public string ToolTip { get; private set; }
	}
}
