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

using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Threading;
using System.Linq;
using System.Windows.Media;
using DevExpress.Mvvm;
using DevExpress.Mvvm.UI;
using DevExpress.Data.Utils;
using DevExpress.Utils;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Core.Serialization;
using System.Windows.Input;
namespace DevExpress.Xpf.Core.Native {
	public abstract class TabbedDocumentUIServiceBase : DocumentUIServiceBase {
		#region Properties
		public static readonly DependencyProperty TargetProperty = DependencyProperty.Register("Target", typeof(DXTabControl), typeof(TabbedDocumentUIServiceBase), new PropertyMetadata(null, 
			(d, e) => ((TabbedDocumentUIServiceBase)d).OnTargetChanged((DXTabControl)e.OldValue, (DXTabControl)e.NewValue)));
		public static readonly DependencyProperty ActiveDocumentProperty = DependencyProperty.Register("ActiveDocument", typeof(IDocument), typeof(TabbedDocumentUIServiceBase), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
			(d, e) => ((TabbedDocumentUIServiceBase)d).OnActiveDocumentChanged((IDocument)e.OldValue, (IDocument)e.NewValue)));
		public static readonly DependencyProperty ShowNewItemOnStartupProperty = DependencyProperty.Register("ShowNewItemOnStartup", typeof(bool), typeof(TabbedDocumentUIServiceBase), new PropertyMetadata(true));
		public static readonly DependencyProperty NewItemTemplateProperty = DependencyProperty.Register("NewItemTemplate", typeof(DataTemplate), typeof(TabbedDocumentUIServiceBase), new PropertyMetadata(null));
		public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(TabbedDocumentUIServiceBase), new PropertyMetadata(null));
		public DXTabControl Target { get { return (DXTabControl)GetValue(TargetProperty); } set { SetValue(TargetProperty, value); } }
		public IDocument ActiveDocument { get { return (IDocument)GetValue(ActiveDocumentProperty); } set { SetValue(ActiveDocumentProperty, value); } }
		public bool ShowNewItemOnStartup { get { return (bool)GetValue(ShowNewItemOnStartupProperty); } set { SetValue(ShowNewItemOnStartupProperty, value); } }
		public DataTemplate NewItemTemplate { get { return (DataTemplate)GetValue(NewItemTemplateProperty); } set { SetValue(NewItemTemplateProperty, value); } }
		public DataTemplate ItemTemplate { get { return (DataTemplate)GetValue(ItemTemplateProperty); } set { SetValue(ItemTemplateProperty, value); } }
		public event ActiveDocumentChangedEventHandler ActiveDocumentChanged;
		#endregion Properties
		protected DXTabControl ActualTarget { get { return Target ?? AssociatedObject as DXTabControl; } }
		protected DXTabControl CurrentTarget {
			get { return currentTarget ?? ActualTarget; }
			set {
				var oldValue = CurrentTarget;
				currentTarget = value;
				var newValue = CurrentTarget;
				if(oldValue != newValue)
					OnCurrentTargetChanged(oldValue, newValue);
			}
		}
		DXTabControl currentTarget = null;
		protected DXTabItem CreateNewTabItem() {
			DXTabItem res = null;
			if(NewItemTemplate != null) {
				object newItem = NewItemTemplate.LoadContent();
				if(newItem is DXTabItem) {
					res = (DXTabItem)newItem;
				} else if((newItem as ContentControl).With(x => x.Content) is DXTabItem) {
					var tab = ((ContentControl)newItem).Content as DXTabItem;
					((ContentControl)newItem).Content = null;
					res = tab;
				}
			}
			if(res == null)
				res = new DXTabItem() { Header = "NewItem" };
			res.SetCurrentValue(DXTabItem.IsNewProperty, true);
			return res;
		}
		protected DXTabItem CreateTabItem() {
			if(ItemTemplate != null) {
				object item = ItemTemplate.LoadContent();
				if(item is DXTabItem)
					return (DXTabItem)item;
				if(item is ContentControl) {
					object res = ((ContentControl)item).Content;
					if(res is DXTabItem) {
						((ContentControl)item).Content = null;
						return (DXTabItem)res;
					}
				}
				throw new InvalidOperationException("NewItemTemplate is invalid.");
			}
			return new DXTabItem() { Header = "Item" };
		}
		void OnTargetChanged(DXTabControl oldValue, DXTabControl newValue) {
			Initialize();
		}
		void OnAssociatedObjectLoaded(object sender, RoutedEventArgs e) {
			AssociatedObject.Loaded -= OnAssociatedObjectLoaded;
			Initialize();
		}
		void SubscribeTabControl(DXTabControl tabControl) {
			tabControl.TabHiding +=
				new WeakEventHandler<TabbedDocumentUIServiceBase, TabControlTabHidingEventArgs, TabControlTabHidingEventHandler>(
					this,
					(owner, sender, args) => owner.OnTabControlTabHiding(sender, args),
					(wh, o) => ((DXTabControl)o).TabHiding -= wh.Handler,
					(wh) => wh.OnEvent).Handler;
			tabControl.TabHidden +=
				new WeakEventHandler<TabbedDocumentUIServiceBase, TabControlTabHiddenEventArgs, TabControlTabHiddenEventHandler>(
					this,
					(owner, sender, args) => owner.OnTabControlTabHidden(sender, args),
					(wh, o) => ((DXTabControl)o).TabHidden -= wh.Handler,
					(wh) => wh.OnEvent).Handler;
			tabControl.TabShowing +=
				new WeakEventHandler<TabbedDocumentUIServiceBase, TabControlTabShowingEventArgs, TabControlTabShowingEventHandler>(
					this,
					(owner, sender, args) => owner.OnTabControlTabShowing(sender, args),
					(wh, o) => ((DXTabControl)o).TabShowing -= wh.Handler,
					(wh) => wh.OnEvent).Handler;
			tabControl.TabShown +=
				new WeakEventHandler<TabbedDocumentUIServiceBase, TabControlTabShownEventArgs, TabControlTabShownEventHandler>(
					this,
					(owner, sender, args) => owner.OnTabControlTabShown(sender, args),
					(wh, o) => ((DXTabControl)o).TabShown -= wh.Handler,
					(wh) => wh.OnEvent).Handler;
			tabControl.TabRemoving +=
				new WeakEventHandler<TabbedDocumentUIServiceBase, TabControlTabRemovingEventArgs, TabControlTabRemovingEventHandler>(
					this,
					(owner, sender, args) => owner.OnTabControlTabRemoving(sender, args),
					(wh, o) => ((DXTabControl)o).TabRemoving -= wh.Handler,
					(wh) => wh.OnEvent).Handler;
			tabControl.TabRemoved +=
			   new WeakEventHandler<TabbedDocumentUIServiceBase, TabControlTabRemovedEventArgs, TabControlTabRemovedEventHandler>(
				   this,
				   (owner, sender, args) => owner.OnTabControlTabRemoved(sender, args),
				   (wh, o) => ((DXTabControl)o).TabRemoved -= wh.Handler,
				   (wh) => wh.OnEvent).Handler;
			tabControl.SelectionChanged +=
			   new WeakEventHandler<TabbedDocumentUIServiceBase, TabControlSelectionChangedEventArgs, TabControlSelectionChangedEventHandler>(
				   this,
				   (owner, sender, args) => owner.OnTabControlSelectionChanged(sender, args.OldSelectedIndex, args.NewSelectedIndex),
				   (wh, o) => ((DXTabControl)o).SelectionChanged -= wh.Handler,
				   (wh) => wh.OnEvent).Handler;
			tabControl.TabAdding +=
				new WeakEventHandler<TabbedDocumentUIServiceBase, TabControlTabAddingEventArgs, TabControlTabAddingEventHandler>(
				   this,
					(owner, sender, args) => owner.OnTabControlTabAdding(sender, args),
					(wh, o) => ((DXTabControl)o).TabAdding -= wh.Handler,
					(wh) => wh.OnEvent).Handler;
			tabControl.NewTabbedWindow +=
			   new WeakEventHandler<TabbedDocumentUIServiceBase, TabControlNewTabbedWindowEventArgs, TabControlNewTabbedWindowEventHandler>(
				   this,
				   (owner, sender, args) => {
					   SubscribeTabControl(args.NewTabControl);
					   SubscribeWindow(args.NewWindow);
				   },
				   (wh, o) => ((DXTabControl)o).NewTabbedWindow -= wh.Handler,
				   (wh) => wh.OnEvent).Handler;
			DXSerializer.AddDeserializePropertyHandler(tabControl,
				new WeakEventHandler<TabbedDocumentUIServiceBase, XtraPropertyInfoEventArgs, XtraPropertyInfoEventHandler>(
				   this,
				   (owner, sender, args) => owner.OnTabControlDeserialized((DXTabControl)sender),
				   (wh, o) => DXSerializer.RemoveDeserializePropertyHandler(((DXTabControl)o), wh.Handler),
				   (wh) => wh.OnEvent).Handler);
		}
		void SubscribeWindow(Window window) {
			if(window == null) return;
			window.Activated +=
				new WeakEventHandler<TabbedDocumentUIServiceBase, EventArgs, EventHandler>(
					this,
					(owner, sender, args) => owner.OnTabbedWindowActivated(sender, args),
					(wh, o) => ((Window)o).Activated -= wh.Handler,
					(wh) => wh.OnEvent).Handler;
			window.Closing +=
				new WeakEventHandler<TabbedDocumentUIServiceBase, CancelEventArgs, CancelEventHandler>(
					this,
					(owner, sender, args) => owner.OnTabbedWindowClosing(sender, args),
					(wh, o) => ((Window)o).Closing -= wh.Handler,
					(wh) => wh.OnEvent).Handler;
			window.Closed +=
				new WeakEventHandler<TabbedDocumentUIServiceBase, EventArgs, EventHandler>(
					this,
					(owner, sender, args) => owner.OnTabbedWindowClosed(sender, args),
					(wh, o) => ((Window)o).Closed -= wh.Handler,
					(wh) => wh.OnEvent).Handler;
			window.Loaded += new WeakEventHandler<TabbedDocumentUIServiceBase, RoutedEventArgs, RoutedEventHandler>(
					this,
					(owner, sender, args) => owner.OnTabbedWindowShown(sender, args),
					(wh, o) => ((Window)o).Loaded -= wh.Handler,
					(wh) => wh.OnEvent).Handler;
		}
		void OnTabControlLayoutUpdated(object sender, EventArgs e) {
			var ownerWindow = Window.GetWindow(ActualTarget);
			if(ownerWindow == null) return;
			ActualTarget.LayoutUpdated -= OnTabControlLayoutUpdated;
			SubscribeWindow(ownerWindow);
		}
		protected override void OnAttached() {
			base.OnAttached();
			if(isInitialized) return;
			if(AssociatedObject.IsLoaded)
				Initialize();
			else AssociatedObject.Loaded += OnAssociatedObjectLoaded;
		}
		protected override void OnDetaching() {
			isInitialized = false;
			AssociatedObject.Loaded -= OnAssociatedObjectLoaded;
			base.OnDetaching();
		}
		bool isInitialized = false;
		protected virtual void Initialize() {
			isInitialized = true;
			CurrentTarget = ActualTarget;
			SubscribeTabControl(ActualTarget);
			var ownerWindow = Window.GetWindow(ActualTarget);
			if(ownerWindow != null)
				SubscribeWindow(Window.GetWindow(ActualTarget));
			else ActualTarget.LayoutUpdated += OnTabControlLayoutUpdated;
			OnTabControlDeserialized(ActualTarget);
			if(ShowNewItemOnStartup && ActualTarget.Items.Count == 0) ActualTarget.AddNewTabItem();
		}
		protected virtual void OnActiveDocumentChanged(IDocument oldValue, IDocument newValue) {
			if(!lockActiveDocumentChanged) {
				lockTabSelectionChanged = true;
				newValue.Do(x => x.Show());
				lockTabSelectionChanged = false;
			}
			ActiveDocumentChanged.Do(x => x(this, new ActiveDocumentChangedEventArgs(oldValue, newValue)));
		}
		void OnCurrentTargetChanged(DXTabControl oldValue, DXTabControl newValue) {
			if(newValue == null) {
				ActiveDocument = null;
				return;
			}
			IDocument doc = newValue.SelectedContainer != null ? GetDocument(newValue.SelectedContainer) : null;
			doc.Do(x => ActiveDocument = x);
		}
		void OnTabControlSelectionChanged(object sender, int oldSelectedIndex, int newSelectedIndex) {
			if(lockTabSelectionChanged) return;
			DXTabControl tabControl = (DXTabControl)sender;
			if(CurrentTarget != tabControl) return;
			DXTabItem item = tabControl.GetTabItem(newSelectedIndex);
			IDocument doc = item != null ? GetDocument(item) : null;
			doc.Do(x => ActiveDocument = x);
		}
		bool lockTabSelectionChanged = false;
		bool lockActiveDocumentChanged = false;
		protected virtual void OnTabControlTabHiding(object sender, TabControlTabHidingEventArgs e) { }
		protected virtual void OnTabControlTabHidden(object sender, TabControlTabHiddenEventArgs e) { }
		protected virtual void OnTabControlTabShowing(object sender, TabControlTabShowingEventArgs e) { }
		protected virtual void OnTabControlTabShown(object sender, TabControlTabShownEventArgs e) { }
		protected virtual void OnTabControlTabRemoving(object sender, TabControlTabRemovingEventArgs e) { }
		protected virtual void OnTabControlTabRemoved(object sender, TabControlTabRemovedEventArgs e) { }
		protected virtual void OnTabControlTabAdding(object sender, TabControlTabAddingEventArgs e) {
			e.Item = CreateNewTabItem();
		}
		protected virtual void OnTabControlDeserialized(DXTabControl tabControl) {
			List<KeyValuePair<int,DXTabItem>> itemsToReplace = new List<KeyValuePair<int,DXTabItem>>();
			for(int i = 0; i < tabControl.Items.Count; i++) {
				var item = tabControl.Items[i] as DXTabItem;
				if(item == null) continue;
				if(item.IsNew) itemsToReplace.Add(new KeyValuePair<int,DXTabItem>(i, item));
			}
			foreach(var item in itemsToReplace)
				tabControl.Items[item.Key] = CreateNewTabItem();
			OnTabControlSelectionChanged(tabControl, tabControl.SelectedIndex, tabControl.SelectedIndex);
		}
		protected virtual void OnTabbedWindowActivated(object sender, EventArgs e) {
			lockActiveDocumentChanged = true;
			SetCurrentTarget((Window)sender);
			lockActiveDocumentChanged = false;
		}
		protected virtual void OnTabbedWindowClosing(object sender, CancelEventArgs e) { }
		protected virtual void OnTabbedWindowClosed(object sender, EventArgs e) { }
		protected virtual void OnTabbedWindowShown(object sender, EventArgs e) { }
		void SetCurrentTarget(Window w) {
			if(w is DXTabbedWindow)
				CurrentTarget = ((DXTabbedWindow)w).TabControl;
			else CurrentTarget = LayoutTreeHelper.GetVisualChildren(w).OfType<DXTabControl>().FirstOrDefault();
		}
	}
}
namespace DevExpress.Xpf.Core {
	[TargetTypeAttribute(typeof(Window))]
	[TargetTypeAttribute(typeof(UserControl))]
	[TargetTypeAttribute(typeof(DXTabControl))]
	public class TabbedWindowDocumentUIService : TabbedDocumentUIServiceBase, IGroupedDocumentManagerService, IDocumentManagerService, IDocumentOwner {
		#region Document
		public class TabbedWindowDocument : ViewModelBase, IDocument, IDocumentInfo, IDisposable {
			public TabbedWindowDocumentUIService Owner { get; private set; }
			public object View { get; private set; }
			public Window Window { get { return TabControl.With(Window.GetWindow); } }
			public DXTabControl TabControl { get { return Tab.With(x => x.Owner); } }
			public DXTabItem Tab { get; private set; }
			public object Id { get; set; }
			public object Title { get { return GetProperty(() => Title); } set { SetProperty(() => Title, value, OnTitleChanged); } }
			public object Content { get { return ViewHelper.GetViewModelFromView(View); } }
			public bool DestroyOnClose { get; set; }
			public string DocumentType { get; private set; }
			public DocumentState State { get; private set; }
			public TabbedWindowDocument(TabbedWindowDocumentUIService owner, object view, string documentType) {
				Owner = owner;
				View = view;
				DocumentType = documentType;
				Owner.documents.Add(this);
				State = DocumentState.Hidden;
			}
			void OnTitleChanged() {
				Tab.Do(x => x.Header = Title);
			}
			bool lockShow = false;
			public void Show() {
				if(lockShow) return;
				lockShow = true;
				Owner.lockTabShowing = true;
				if(Tab == null || Tab.Owner == null) {
					CreateOrReplaceTab();
					PrepareTab();
				}
				Window.GetWindow(Tab.Owner).Do(x => x.Activate());
				Tab.Owner.ShowTabItem(Tab);
				State = DocumentState.Visible;
				Owner.lockTabShowing = false;
				lockShow = false;
			}
			void CreateOrReplaceTab() {
				DXTabControl tabControl = Owner.CurrentTarget;
				int index = -1;
				if(tabControl.SelectedContainer.Return(x => x.IsNew, () => false)) {
					index = tabControl.SelectedIndex;
					Owner.lockTabHidding = true;
					tabControl.RemoveTabItem(tabControl.SelectedContainer);
					Owner.lockTabHidding = false;
				}
				if(Tab == null) {
					Tab = Owner.CreateTabItem();
					SetDocument(Tab, this);
					Tab.Content = View;
					Owner.InitializeDocumentContainer(Tab, DXTabItem.ContentProperty, null);
					if(index != -1) tabControl.Items.Insert(index, Tab);
					else tabControl.Items.Add(Tab);
					return;
				} 
				if(index != -1) {
					Tab.Remove();
					Tab.Insert(tabControl, index);
				}
			}
			void PrepareTab() {
				SetDocument(Tab, this);
				Tab.SetCurrentValue(DXTabItem.IsNewProperty, false);
				if(Tab.Owner == null) Owner.CurrentTarget.Items.Add(Tab);
				SetTitleBinding(View, DXTabItem.HeaderProperty, Tab, false);
				if(Title != null) OnTitleChanged();
				if(string.IsNullOrEmpty(Tab.Name) && Id != null) Tab.Name = Id.ToString();
			}
			public void Close(bool force = true) {
				CloseCore(force, DestroyOnClose);
			}
			internal void CloseCore(bool force, bool dispose) {
				if(State == DocumentState.Destroyed) return;
				CancelEventArgs args = new CancelEventArgs();
				if(!force)
					DocumentViewModelHelper.OnClose(Content, args);
				if(args.Cancel) return;
				Owner.lockTabHidding = true;
				Tab.Close();
				Owner.lockTabHidding = false;
				Tab.Owner.Do(x => x.RemoveTabItem(Tab));
				State = DocumentState.Hidden;
				if(dispose) Dispose();
			}
			public void Hide() {
				if(State != DocumentState.Visible) return;
				var owner = Owner;
				owner.lockTabHidding = true;
				Tab.Close();
				State = DocumentState.Hidden;
				owner.lockTabHidding = false;
			}
			public void Dispose() {
				Tab.With(x => x.Owner).Do(x => x.RemoveTabItem(Tab));
				DocumentViewModelHelper.OnDestroy(Content);
				Owner.documents.Remove(this);
				Owner = null;
				Tab = null;
				View = null;
				State = DocumentState.Destroyed;
			}
		}
		public class TabbedWindowDocumentGroup : IDocumentGroup {
			public IEnumerable<IDocument> Documents { get; private set; }
			public TabbedWindowDocumentGroup(IEnumerable<IDocument> documents) {
				Documents = documents;
			}
		}
		#endregion Document
		public static readonly DependencyProperty WindowShownCommandProperty = DependencyProperty.Register("WindowShownCommand", typeof(ICommand), typeof(TabbedWindowDocumentUIService), new PropertyMetadata(null));
		public static readonly DependencyProperty WindowClosingCommandProperty = DependencyProperty.Register("WindowClosingCommand", typeof(ICommand), typeof(TabbedWindowDocumentUIService), new PropertyMetadata(null));
		public static readonly DependencyProperty SingleWindowClosingCommandProperty = DependencyProperty.Register("SingleWindowClosingCommand", typeof(ICommand), typeof(TabbedWindowDocumentUIService), new PropertyMetadata(null));
		public ICommand WindowShownCommand { get { return (ICommand)GetValue(WindowShownCommandProperty); } set { SetValue(WindowShownCommandProperty, value); } }
		public ICommand WindowClosingCommand { get { return (ICommand)GetValue(WindowClosingCommandProperty); } set { SetValue(WindowClosingCommandProperty, value); } }
		public ICommand SingleWindowClosingCommand { get { return (ICommand)GetValue(SingleWindowClosingCommandProperty); } set { SetValue(SingleWindowClosingCommandProperty, value); } }
		public IEnumerable<IDocument> Documents { get { return documents; } }
		public IDocumentGroup ActiveGroup { get { return new TabbedWindowDocumentGroup(GetDocuments(CurrentTarget)); } }
		public IEnumerable<IDocumentGroup> Groups { get { return GetGroups(); } }
		protected override void OnTabbedWindowClosing(object sender, CancelEventArgs e) {
			base.OnTabbedWindowClosing(sender, e);
			var docs = documents.Where(x => x.Window == sender).ToList();
			DocumentsClosingEventArgs args = new DocumentsClosingEventArgs(docs) { Cancel = e.Cancel };
			WindowClosingCommand.If(x => x.CanExecute(args)).Do(x => x.Execute(args));
			if(Groups.Count() == 1) SingleWindowClosingCommand.If(x => x.CanExecute(args)).Do(x => x.Execute(args));
			e.Cancel = args.Cancel;
		}
		protected override void OnTabbedWindowShown(object sender, EventArgs e) {
			base.OnTabbedWindowShown(sender, e);
			WindowShownCommand.If(x => x.CanExecute(e)).Do(x => x.Execute(e));
		}
		protected override void OnTabbedWindowClosed(object sender, EventArgs e) {
			base.OnTabbedWindowClosed(sender, e);
			var docs = documents.Where(x => x.Window == sender).ToList();
			foreach(var doc in docs)
				if(doc.State == DocumentState.Visible) doc.Close(true);
		}
		protected override void OnTabControlTabHiding(object sender, TabControlTabHidingEventArgs e) {
			base.OnTabControlTabHiding(sender, e);
			if(lockTabHidding) return;
			DXTabControl tabControl = (DXTabControl)sender;
			DXTabItem item = tabControl.GetTabItem(e.Item);
			TabbedWindowDocument document = (TabbedWindowDocument)(item != null ? GetDocument(item) : null);
			if(document == null) return;
			e.Cancel = true;
			if(tabControl.View.RemoveTabItemsOnHiding)
				document.Close(false);
			else document.Hide();
		}
		protected override void OnTabControlTabShowing(object sender, TabControlTabShowingEventArgs e) {
			base.OnTabControlTabShowing(sender, e);
			if(lockTabShowing) return;
			DXTabControl tabControl = (DXTabControl)sender;
			DXTabItem item = tabControl.GetTabItem(e.Item);
			TabbedWindowDocument document = (TabbedWindowDocument)(item != null ? GetDocument(item) : null);
			if(document == null) return;
			e.Cancel = true;
			document.Show();
		}
		bool lockTabShowing = false;
		bool lockTabHidding = false;
		List<TabbedWindowDocument> documents = new List<TabbedWindowDocument>();
		void IDocumentOwner.Close(IDocumentContent documentContent, bool force) {
			CloseDocument(this, documentContent, force);
		}
		IDocument IDocumentManagerService.CreateDocument(string documentType, object viewModel, object parameter, object parentViewModel) {
			object view = CreateAndInitializeView(documentType, viewModel, parameter, parentViewModel, this);
			DXSerializer.SetEnabled((DependencyObject)view, false);
			return new TabbedWindowDocument(this, view, documentType);
		}
		IEnumerable<IDocumentGroup> GetGroups() {
			List<TabbedWindowDocumentGroup> res = new List<TabbedWindowDocumentGroup>();
			if(CurrentTarget == null) return res;
			if(CurrentTarget.View.StretchView == null) {
				res.Add(new TabbedWindowDocumentGroup(Documents));
				return res;
			}
			var tabControls = DragDropRegionManager.GetDragDropControls(CurrentTarget.View.StretchView.DragDropRegion).OfType<DXTabControl>();
			foreach(var tabControl in tabControls) {
				var currentGroup = GetDocuments(tabControl);
				if(currentGroup.Count() > 0)
					res.Add(new TabbedWindowDocumentGroup(currentGroup));
			}
			return res;
		}
		IEnumerable<IDocument> GetDocuments(DXTabControl tabControl) {
			List<IDocument> currentGroup = new List<IDocument>();
			if(tabControl == null) return currentGroup;
			tabControl.ForEachTabItem(x => {
				var doc = GetDocument(x);
				if(doc != null) currentGroup.Add(doc);
			});
			return currentGroup;
		}
	}
}
