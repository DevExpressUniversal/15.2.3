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
using System.Collections.Generic;
using DevExpress.XtraBars.Docking2010.Base;
namespace DevExpress.XtraBars.Docking2010.Views {
	public enum RibbonAndBarsMergeStyle {
		Default,
		Always,
		WhenNotFloating
	}
	public enum DocumentActivationScope {
		Default,
		DocumentsHost,
		AllDocuments
	}
	public interface IActivationInfo : IDisposable {
		BaseDocument ActiveDocument { get; }
		event DocumentEventHandler ActiveDocumentChanged;
		IEnumerable<BaseDocument> DocumentActivationList { get; }
		IEnumerable<Docking.DockPanel> PanelActivationList { get; }
		void Attach(IDocumentsHostWindow hostWindow);
		void Detach(IDocumentsHostWindow hostWindow);
		void Attach(Docking.DockManager dockManager);
		void Detach(Docking.DockManager dockManager);
		void Attach(BaseView view);
		void Detach(BaseView view);
		void Detach(DocumentManager manager);
		IDisposable LockActiveDocumentChanging();
		Ribbon.IRibbonMergingContext GetRibbonMergingContext();
		void OnMaximize();
		void OnRestore();
	}
	class ActivationInfo : IActivationInfo, ISharedRefTarget<IActivationInfo>, IThumbnailManagerClient {
		readonly DocumentManager Manager;
		readonly System.ComponentModel.EventHandlerList Events;
		public ActivationInfo(DocumentManager manager) {
			this.Manager = manager;
			this.Events = new System.ComponentModel.EventHandlerList();
			this.documentActivationListCore = new List<BaseDocument>();
			this.panelActivationListCore = new List<Docking.DockPanel>();
			this.dockManagers = new List<Docking.DockManager>();
			this.views = new List<BaseView>();
			this.hostWindows = new List<IDocumentsHostWindow>();
			this.hostForms = new List<System.Windows.Forms.Form>();
		}
		bool isDisposing;
		void IDisposable.Dispose() {
			if(!isDisposing) {
				isDisposing = true;
				OnDispose();
			}
			GC.SuppressFinalize(this);
		}
		void OnDispose() {
			Events.Dispose();
			System.Windows.Forms.Form[] hostForms = this.hostForms.ToArray();
			for(int i = 0; i < hostForms.Length; i++)
				Detach(hostForms[i]);
			Docking.DockManager[] managers = this.dockManagers.ToArray();
			for(int i = 0; i < managers.Length; i++)
				UnsubscribeDockManagerEvents(managers[i]);
			BaseView[] views = this.views.ToArray();
			for(int i = 0; i < views.Length; i++)
				UnsubscribeViewEvents(views[i]);
			IDocumentsHostWindow[] hostWindows = this.hostWindows.ToArray();
			for(int i = 0; i < hostWindows.Length; i++)
				UnubscribeHostWindowEvents(hostWindows[i]);
			ReleaseActiveDocument();
			documentActivationListCore.Clear();
			panelActivationListCore.Clear();
		}
		bool ISharedRefTarget<IActivationInfo>.CanShare(IActivationInfo target) {
			ActivationInfo targetInfo = target as ActivationInfo;
			return (targetInfo != null) && Manager.IsShareActivationScope(targetInfo.Manager);
		}
		void ISharedRefTarget<IActivationInfo>.Share(IActivationInfo target) {
			ActivationInfo targetInfo = target as ActivationInfo;
			if(targetInfo != null) {
				Docking.DockManager[] targetDockManagers = targetInfo.dockManagers.ToArray();
				for(int i = 0; i < targetDockManagers.Length; i++)
					Attach(targetDockManagers[i]);
				BaseView[] targetViews = targetInfo.views.ToArray();
				for(int i = 0; i < targetViews.Length; i++)
					Attach(targetViews[i]);
				var targetHostForms = this.hostForms.ToArray();
				for(int i = 0; i < targetHostForms.Length; i++)
					Attach(targetHostForms[i]);
				IDocumentsHostWindow hostWindow = DocumentsHostContext.GetDocumentsHostWindow(targetInfo.Manager);
				if(hostWindow != null)
					Attach(hostWindow);
				var hostForm = DocumentsHostContext.GetForm(Manager);
				if(hostForm != null)
					Attach(hostForm);
				Delegate handler = targetInfo.Events[activeDocumentChanged];
				while(handler != null) {
					Events.AddHandler(activeDocumentChanged, handler);
					targetInfo.Events.RemoveHandler(activeDocumentChanged, handler);
					handler = targetInfo.Events[activeDocumentChanged];
				}
			}
		}
		void Attach(System.Windows.Forms.Form hostForm) {
			if(!hostForms.Contains(hostForm)) {
				hostForm.Activated += HostWindow_Activated;
				hostForms.Add(hostForm);
			}
		}
		void Detach(System.Windows.Forms.Form hostForm) {
			if(hostForms.Contains(hostForm)) {
				hostForm.Activated -= HostWindow_Activated;
				hostForms.Remove(hostForm);
			}
		}
		public void Detach(DocumentManager manager) {
			var hostForm = DocumentsHostContext.GetForm(manager);
			if(hostForm != null)
				Detach(hostForm);
			var hostWindow = DocumentsHostContext.GetDocumentsHostWindow(manager);
			if(hostWindow != null)
				Detach(hostWindow);
		}
		public void Attach(IDocumentsHostWindow hostWindow) {
			if(!hostWindows.Contains(hostWindow))
				SubscribeHostWindowEvents(hostWindow);
		}
		public void Detach(IDocumentsHostWindow hostWindow) {
			if(hostWindows.Contains(hostWindow))
				UnubscribeHostWindowEvents(hostWindow);
		}
		List<Docking.DockManager> dockManagers;
		List<BaseView> views;
		List<IDocumentsHostWindow> hostWindows;
		List<System.Windows.Forms.Form> hostForms;
		public void Attach(Docking.DockManager dockManager) {
			if(!dockManagers.Contains(dockManager))
				SubscribeDockManagerEvents(dockManager);
		}
		public void Detach(Docking.DockManager dockManager) {
			if(dockManagers.Contains(dockManager))
				UnsubscribeDockManagerEvents(dockManager);
		}
		public void Attach(BaseView view) {
			if(!views.Contains(view))
				SubscribeViewEvents(view);
		}
		public void Detach(BaseView view) {
			if(views.Contains(view))
				UnsubscribeViewEvents(view);
		}
		void SubscribeViewEvents(BaseView view) {
			if(view != null) {
				view.GotFocus += View_GotFocus;
				view.DocumentActivated += View_DocumentActivated;
				view.ControlShown += View_ControlShown;
				view.Paint += View_Paint;
				if(view.Documents != null)
					view.Documents.CollectionChanged += Documents_CollectionChanged;
				if(view.FloatDocuments != null)
					view.FloatDocuments.CollectionChanged += FloatDocuments_CollectionChanged;
				NativeMdi.NativeMdiView nv = view as NativeMdi.NativeMdiView;
				views.Add(view);
			}
		}
		void View_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
			RaiseUpdateTabbedThumbnails();
		}
		void UnsubscribeViewEvents(BaseView view) {
			if(view != null) {
				views.Remove(view);
				view.GotFocus -= View_GotFocus;
				view.DocumentActivated -= View_DocumentActivated;
				view.ControlShown -= View_ControlShown;
				view.Paint -= View_Paint;
				if(view.Documents != null)
					view.Documents.CollectionChanged -= Documents_CollectionChanged;
				if(view.FloatDocuments != null)
					view.FloatDocuments.CollectionChanged -= FloatDocuments_CollectionChanged;
			}
		}
		void SubscribeHostWindowEvents(IDocumentsHostWindow window) {
			if(window != null) {
				window.Closed += HostWindow_Closed;
				var hostForm = window as System.Windows.Forms.Form;
				if(hostForm != null)
					hostForm.Activated += HostWindow_Activated;
				this.hostWindows.Add(window);
			}
		}
		void UnubscribeHostWindowEvents(IDocumentsHostWindow window) {
			if(window != null) {
				this.hostWindows.Remove(window);
				var hostForm = window as System.Windows.Forms.Form;
				if(hostForm != null)
					hostForm.Activated -= HostWindow_Activated;
				window.Closed -= HostWindow_Closed;
			}
		}
		void SubscribeDockManagerEvents(Docking.DockManager dockManager) {
			if(dockManager != null) {
				dockManager.ActivePanelChanged += DockManager_ActivePanelChanged;
				dockManager.ClosedPanel += DockManager_ClosedPanel;
				this.dockManagers.Add(dockManager);
			}
		}
		void UnsubscribeDockManagerEvents(Docking.DockManager dockManager) {
			if(dockManager != null) {
				this.dockManagers.Remove(dockManager);
				dockManager.ActivePanelChanged -= DockManager_ActivePanelChanged;
				dockManager.ClosedPanel -= DockManager_ClosedPanel;
			}
		}
		void DockManager_ActivePanelChanged(object sender, Docking.ActivePanelChangedEventArgs e) {
			if(e.Panel == null) return;
			AddElementToActivationList(e.Panel);
		}
		void DockManager_ClosedPanel(object sender, Docking.DockPanelEventArgs e) {
			RemoveElementFromActivationList(e.Panel);
		}
		void HostWindow_Closed(object sender, EventArgs e) {
			IDocumentsHostWindow hostWindow = sender as IDocumentsHostWindow;
			if(hostWindow != null)
				UnubscribeHostWindowEvents(hostWindow);
			else Detach((System.Windows.Forms.Form)sender);
		}
		void HostWindow_Activated(object sender, EventArgs e) {
			IDocumentsHostWindow hostWindow = sender as IDocumentsHostWindow;
			if(hostWindow != null) {
				BaseView view = hostWindow.DocumentManager.View;
				SetActiveDocumentFromFocusedView(view);
			}
			else {
				if(sender == DocumentsHostContext.GetForm(Manager))
					SetActiveDocumentFromFocusedView(Manager.View);
			}
		}
		void SetActiveDocumentFromFocusedView(BaseView view) {
			if(view != null && view.IsFocused && view.ActiveDocument != null) {
				SetActiveDocument(view.ActiveDocument);
				AddDocumentToActivationList(view.ActiveDocument);
			}
		}
		void View_GotFocus(object sender, EventArgs e) {
			BaseView view = sender as BaseView;
			if(view.ActiveDocument != null)
				SetActiveDocument(view.ActiveDocument);
			if(Manager != null)
				Manager.UpdateTaskbarThumbnails();
		}
		void View_DocumentActivated(object sender, DocumentEventArgs e) {
			AddElementToActivationList(e.Document);
			SetActiveDocument(e.Document);
		}
		void View_ControlShown(object sender, DeferredControlLoadEventArgs e) {
			if(e.Document == ActiveDocument)
				EnsureMergedBarsAndRibbons();
		}
		void Documents_CollectionChanged(CollectionChangedEventArgs<BaseDocument> ea) {
			RaiseTabbedThumbnailsChanged(new DocumentEventArgs(ea.Element));
			if(IsActiveDocumentChangingLocked) return;
			if(ea.ChangedType == CollectionChangedType.ElementAdded)
				AddElementToActivationList(ea.Element);
			if(ea.ChangedType == CollectionChangedType.ElementRemoved) {
				RemoveElementFromActivationList(ea.Element);
				CheckActiveDocument(ea.Element);
			}
		}
		void FloatDocuments_CollectionChanged(CollectionChangedEventArgs<BaseDocument> ea) {
			RaiseTabbedThumbnailsChanged(new DocumentEventArgs(ea.Element));
			if(IsActiveDocumentChangingLocked) return;
			if(ea.ChangedType == CollectionChangedType.ElementAdded)
				AddElementToActivationList(ea.Element);
			if(ea.ChangedType == CollectionChangedType.ElementRemoved) {
				RemoveElementFromActivationList(ea.Element);
				CheckActiveFloatDocument(ea.Element);
			}
		}
		int lockCounter;
		bool IsActiveDocumentChangingLocked {
			get { return lockCounter > 0; }
		}
		IDisposable IActivationInfo.LockActiveDocumentChanging() {
			return new Locker(this);
		}
		void IActivationInfo.OnMaximize() {
			RefreshRibbon(GetParentBarManager());
		}
		void IActivationInfo.OnRestore() {
			RefreshRibbon(GetParentBarManager());
		}
		class Locker : IDisposable {
			ActivationInfo info;
			public Locker(ActivationInfo info) {
				this.info = info;
				info.lockCounter++;
			}
			void IDisposable.Dispose() {
				info.lockCounter--;
			}
		}
		void CheckActiveDocument(BaseDocument document) {
			if(ActiveDocument == document && documentActivationListCore.Count == 0) {
				BaseView view = GetView(document);
				if((view != null) && view.IsDocumentClosing(document))
					SetActiveDocument(null);
			}
		}
		void CheckActiveFloatDocument(BaseDocument document) {
			if(ActiveDocument == document && documentActivationListCore.Count == 0) 
				SetActiveDocument(null);
		}
		BaseView GetView(BaseDocument document) {
			BaseView view = Manager.View ?? 
				((document != null && document.Manager != null) ? document.Manager.View : null);
			return (view != null && view.Controller != null) ? view.Controller.GetView(document) : null;
		}
		void ReleaseActiveDocument() {
			if(ActiveDocument != null) 
				UnubscribeActiveDocument();
			activeDocumentCore = null;
		}
		BaseDocument activeDocumentCore;
		public BaseDocument ActiveDocument {
			get { return activeDocumentCore; }
		}
		readonly static object activeDocumentChanged = new object();
		public event DocumentEventHandler ActiveDocumentChanged {
			add { Events.AddHandler(activeDocumentChanged, value); }
			remove { Events.RemoveHandler(activeDocumentChanged, value); }
		}
		void RaiseActiveDocumentChanged() {
			DocumentEventHandler handler = Events[activeDocumentChanged] as DocumentEventHandler;
			if(handler != null)
				handler(Manager, new DocumentEventArgs(ActiveDocument));
		}
		IList<BaseDocument> documentActivationListCore;
		public IEnumerable<BaseDocument> DocumentActivationList {
			get { return documentActivationListCore; }
		}
		IList<Docking.DockPanel> panelActivationListCore;
		public IEnumerable<Docking.DockPanel> PanelActivationList {
			get { return panelActivationListCore; }
		}
		void AddElementToActivationList(object obj) {
			BaseDocument document = obj as BaseDocument;
			if(document != null) {
				AddDocumentToActivationList(document);
				if(ActiveDocument == null)
					SetActiveDocument(document);
			}
			Docking.DockPanel panel = obj as Docking.DockPanel;
			if(panel != null)
				AddPanelToActivationList(panel);
		}
		void AddDocumentToActivationList(BaseDocument document) {
			documentActivationListCore.Remove(document);
			documentActivationListCore.Insert(0, document);
		}
		void AddPanelToActivationList(Docking.DockPanel panel) {
			panelActivationListCore.Remove(panel);
			panelActivationListCore.Insert(0, panel);
		}
		void RemoveElementFromActivationList(object obj) {
			BaseDocument document = obj as BaseDocument;
			if(document != null) {
				documentActivationListCore.Remove(document);
				if(document.IsDockPanel && ActiveDocument == document) {
					ReleaseActiveDocument();
					AddPanelToActivationList(document.GetDockPanel());
				}
			}
			Docking.DockPanel panel = obj as Docking.DockPanel;
			if(panel != null)
				panelActivationListCore.Remove(panel);			
		}
		void SetActiveDocument(BaseDocument document) {
			if(ActiveDocument == document) {
				if(document != null) 
					EnsureMergedBarsAndRibbons();
				return;
			}
			using(GetRibbonMergingContext()) {
				if(ActiveDocument != null) {
					UnubscribeActiveDocument();
					OnDocumentDeactivated();
				}
				activeDocumentCore = document;
				if(ActiveDocument != null) {
					OnDocumentActivated();
					SubscribeActiveDocument();
				}
			}
			RaiseActiveDocumentChanged();
		}
		bool AllowMerging() {
			return Manager != null && Manager.CanMergeOnDocumentActivate();
		}
		void OnDocumentActivated() {
			if(Manager != null)
				Manager.UpdateTaskbarThumbnails();
			if(!AllowMerging()) return;
			BarManager parentManager = GetParentBarManager();
			if(parentManager != null && ActiveDocument.IsControlLoaded) {
				BarManager childManager = BarManager.FindManager(ActiveDocument.Control);
				if(childManager != null) 
					MergeBarsAndRibbons(parentManager, childManager);
			}
		}
		void OnDocumentDeactivated() {
			if(!AllowMerging()) return;
			UnmergeBarsAndRibbons(BarManager.FindManager(Manager.GetContainer()));
		}
		BarManager GetParentBarManager() {
			BarManager parentManager = BarManager.FindManager(Manager.GetContainer());
			if(parentManager == null && Manager.IsInitializing) {
				BarManager menuManager = Manager.MenuManager as BarManager;
				if(menuManager != null && menuManager.Form == null)
					parentManager = menuManager;
				Ribbon.RibbonControl ribbon = Manager.MenuManager as Ribbon.RibbonControl;
				if(ribbon != null)
					parentManager = ribbon.Manager;
			}
			return parentManager;
		}
		WeakReference MergedParentManager;
		WeakReference MergedChildManager;
		BarManager GetMergedParentManager() {
			return (MergedParentManager != null) ? MergedParentManager.Target as BarManager : null;
		}
		BarManager GetMergedChildManager() {
			return (MergedChildManager != null) ? MergedChildManager.Target as BarManager : null;
		}
		public Ribbon.IRibbonMergingContext GetRibbonMergingContext() {
			if(!AllowMerging()) return null;
			return GetRibbonMergingContext(BarManager.FindManager(Manager.GetContainer()));
		}
		Ribbon.IRibbonMergingContext GetRibbonMergingContext(BarManager parentManager) {
			Ribbon.IRibbonMergingContext mergingContext = null;
			Ribbon.RibbonBarManager ribbonManager = parentManager as Ribbon.RibbonBarManager;
			if(ribbonManager != null)
				mergingContext = ribbonManager.Ribbon.CreateRibbonMergingContext();
			return mergingContext;
		}
		void EnsureMergedBarsAndRibbons() {
			var parentManager = GetMergedParentManager();
			if(Coalesce(ref parentManager, GetParentBarManager) && AllowMerging())
				EnsureMergedBarsAndRibbons(parentManager);
			else if(Manager.RibbonAndBarsMergeStyle == RibbonAndBarsMergeStyle.WhenNotFloating && parentManager != null && !parentManager.IsDestroying)
				UnmergeBarsAndRibbons(parentManager);
		}
		void EnsureMergedBarsAndRibbons(BarManager parentManager) {
			BarManager childManager = BarManager.FindManager(ActiveDocument.Control);
			if(GetMergedChildManager() != childManager) {
				using(GetRibbonMergingContext(parentManager)) {
					UnmergeBarsAndRibbons(parentManager);
					MergeBarsAndRibbons(parentManager, childManager);
				}
			}
		}
		static bool Coalesce<T>(ref T value, Func<T> calcValue) where T : class {
			return (value != null) || (value == null && ((value = calcValue()) != null));
		}
		void MergeBarsAndRibbons(BarManager parentManager, BarManager childManager) {
			MergedParentManager = new WeakReference(parentManager);
			MergedChildManager = new WeakReference(childManager);
			MergeBarsAndRibbonsCore(parentManager, childManager, CanMergeBarManager, CanMergeRibbon);
		}
		void UnmergeBarsAndRibbons(BarManager parentManager) {
			MergedParentManager = new WeakReference(parentManager);
			UnmergeBarsAndRibbonsCore(parentManager, CanMergeBarManager, CanMergeRibbon);
		}
		void RefreshRibbon(BarManager parentManager) {
			if(parentManager is Ribbon.RibbonBarManager) {
				var parentRibbon = ((Ribbon.RibbonBarManager)parentManager).Ribbon;
				if(parentRibbon != null && parentRibbon.GetMdiMergeStyle() == Ribbon.RibbonMdiMergeStyle.Always)
					parentRibbon.Refresh();
			}
		}
		void MergeBarsAndRibbonsCore(BarManager parentManager, BarManager childManager, Predicate<BarManager> canMergeBarManager, Predicate<Ribbon.RibbonControl> canMergeRibbon) {
			if(childManager != null && parentManager != null) {
				if(!childManager.Helper.LoadHelper.Loaded)
					childManager.Helper.LoadHelper.Load();
				if(parentManager is Ribbon.RibbonBarManager && childManager is Ribbon.RibbonBarManager) {
					var parentRibbon = ((Ribbon.RibbonBarManager)parentManager).Ribbon;
					var childRibbon = ((Ribbon.RibbonBarManager)childManager).Ribbon;
					if(parentRibbon != null && childRibbon != null) {
						if(canMergeRibbon(parentRibbon)) {
							if(!childRibbon.IsHandleCreated)
								childRibbon.CreateControl();
							var childForm = childManager.GetForm();
							if(Manager.IsMdiStrategyInUse) {
								parentRibbon.MDIMinimizeItem.ChildForm = childForm;
								parentRibbon.MDIRestoreItem.ChildForm = childForm;
								parentRibbon.MDICloseItem.ChildForm = childForm;
							}
							parentRibbon.MergeRibbon(childRibbon);
						}
					}
				}
				else {
					if(canMergeBarManager(parentManager))
						parentManager.Helper.MdiHelper.MergeManager(childManager);
				}
				MergedChildManager = new WeakReference(childManager);
			}
			else MergedChildManager = null;
		}
		void UnmergeBarsAndRibbonsCore(BarManager parentManager, Predicate<BarManager> canMergeBarManager, Predicate<Ribbon.RibbonControl> canMergeRibbon) {
			if(parentManager != null) {
				if(parentManager is Ribbon.RibbonBarManager) {
					var parentRibbon = ((Ribbon.RibbonBarManager)parentManager).Ribbon;
					if(parentRibbon != null) {
						if(canMergeRibbon(parentRibbon)) 
							parentRibbon.UnMergeRibbon();
					}
				}
				else {
					if(canMergeBarManager(parentManager))
						parentManager.Helper.MdiHelper.UnMergeManager();
				}
			}
			MergedChildManager = null;
		}
		bool CanMergeBarManager(BarManager parentManager) {
			return parentManager.MdiMenuMergeStyle == BarMdiMenuMergeStyle.Always || parentManager.MdiMenuMergeStyle == BarMdiMenuMergeStyle.WhenChildActivated;
		}
		bool CanMergeRibbon(Ribbon.RibbonControl parentRibbon) {
			return parentRibbon.GetMdiMergeStyle() == Ribbon.RibbonMdiMergeStyle.Always;
		}
		void SubscribeActiveDocument() {
			ActiveDocument.Disposed += ActiveDocument_Disposed;
		}
		void UnubscribeActiveDocument() {
			ActiveDocument.Disposed -= ActiveDocument_Disposed;
		}
		void ActiveDocument_Disposed(object sender, EventArgs e) {
			BaseDocument document = sender as BaseDocument;
			ReleaseActiveDocument();
			documentActivationListCore.Remove(document);
			if(documentActivationListCore.Count == 0)
				OnDocumentDeactivated();
		}
		#region IThumbnailManagerClient
		readonly static object tabbedThumbnailsChanged = new object();
		readonly static object updateTabbedThumbnailsEvent = new object();
		IEnumerable<System.Windows.Forms.Control> IThumbnailManagerClient.Children {
			get {
				if(isDisposing || !Manager.CanUseTaskbarThumbnails())
					yield break;
				int counter = 0;
				foreach(BaseDocument document in DocumentActivationList) {
					if(counter >= Manager.MaxThumbnailCount)
						yield break;
					if(document.IsControlHandleCreated) {
						counter++;
						yield return document.Control;
					}
				}
			}
		}
		object IThumbnailManagerClient.GetObject(System.Windows.Forms.Control control) {
			foreach(BaseDocument document in DocumentActivationList)
				if(document.IsControlHandleCreated && control == document.Control) return document;
			return null;
		}
		event DocumentEventHandler IThumbnailManagerClient.TabbedThumbnailsChanged {
			add { Events.AddHandler(tabbedThumbnailsChanged, value); }
			remove { Events.RemoveHandler(tabbedThumbnailsChanged, value); }
		}
		event EventHandler IThumbnailManagerClient.UpdateTabbedThumbnails {
			add { Events.AddHandler(updateTabbedThumbnailsEvent, value); }
			remove { Events.RemoveHandler(updateTabbedThumbnailsEvent, value); }
		}
		void RaiseUpdateTabbedThumbnails() {
			if(!CanRaiseEvents()) return;
			EventHandler handler = Events[updateTabbedThumbnailsEvent] as EventHandler;
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		void RaiseTabbedThumbnailsChanged(DocumentEventArgs ea) {
			if(!CanRaiseEvents()) return;
			DocumentEventHandler handler = Events[tabbedThumbnailsChanged] as DocumentEventHandler;
			if(handler != null)
				handler(this, ea);
		}
		bool CanRaiseEvents() {
			return !(isDisposing || Manager.View == null || Manager.View.IsDesignMode());
		}
		#endregion IThumbnailManagerClient
	}
}
