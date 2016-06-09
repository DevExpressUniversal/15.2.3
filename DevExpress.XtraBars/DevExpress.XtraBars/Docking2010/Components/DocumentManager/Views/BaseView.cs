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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraBars.Docking2010.Base;
using DevExpress.XtraBars.Docking2010.Customization;
using DevExpress.XtraBars.Docking2010.DragEngine;
namespace DevExpress.XtraBars.Docking2010.Views {
	public enum ViewType {
		Tabbed,
		NativeMdi,
		WindowsUI,
		Widget,
		NoDocuments,
	}
	public enum FloatingReason {
		DoubleClick,
		ContextMenuAction,
		Dragging
	}
	public enum EndFloatingReason {
		DoubleClick,
		ContextMenuAction,
		Reposition,
		Docking
	}
	public enum FloatingDocumentContainer {
		Default,
		SingleDocument,
		DocumentsHost
	}
	public interface IDocumentClosedContext : IDisposable {
		void Flush();
	}
	public interface IMeasureContext : IDisposable {
		Graphics Graphics { get; }
	}
	[ToolboxItem(false), DesignTimeVisible(false), DevExpress.Utils.SerializationOrder(Order = 3)]
	public abstract class BaseView : BaseComponent, IUIElement,
		IXtraSerializable, IXtraSerializableLayout, ISupportXtraSerializer,
		IAppearanceCollectionAccessor, IDesignTimeSupport, DevExpress.Utils.ILogicalOwner, DevExpress.Utils.IXtraSerializableChildren, IDocumentCaptionAppearanceProvider,
		DevExpress.Utils.MVVM.Services.IDocumentAdapterFactory, DevExpress.Utils.MVVM.Services.IWindowedDocumentAdapterFactory {
		DocumentManager managerCore;
		BaseViewPainter painterCore;
		BaseViewInfo viewInfoCore;
		IBaseViewController controllerCore;
		IBaseDocumentProperties documentPropertiesCore;
		FloatingDocumentContainer floatingDocumentContainerCore;
		GraphicsInfo gInfo;
		OptionsLayout optionsLayoutCore;
		protected BaseView()
			: base(null) {
		}
		protected BaseView(IContainer container)
			: base(container) {
		}
		protected override void OnCreate() {
			base.OnCreate();
			optionsLayoutCore = CreateOptionsLayout();
			appearanceCollectionCore = CreateAppearanceCollection();
			AppearanceCollection.Changed += OnAppearanceChanged;
			floatingDocumentContainerCore = FloatingDocumentContainer.Default;
			this.gInfo = new GraphicsInfo();
			this.loadingIndicatorPropertiesCore = CreateLoadingIndicatorProperties();
			this.documentPropertiesCore = CreateDocumentProperties();
			this.documentSelectorPropertiesCore = CreateDocumentSelectorProperties();
			this.documentsCore = CreateDocumentCollection();
			this.floatDocumentsCore = CreateFloatDocumentCollection();
			this.floatPanelsCore = CreateFloatPanelCollection();
			this.controllerCore = CreateController();
			this.windowsDialogPropertiesCore = CreateWindowsDialogProperties();
			DocumentProperties.Changed += OnDocumentPropertiesChanged;
			DocumentSelectorProperties.Changed += OnDocumentSelectorPropertiesChanged;
			Documents.CollectionChanged += OnDocumentCollectionChanged;
			FloatDocuments.CollectionChanged += OnFloatDocumentCollectionChanged;
			FloatPanels.CollectionChanged += OnFloatPanelCollectionChanged;
			WindowsDialogProperties.Changed += OnWindowsDialogPropertiesChanged;
		}
		protected override void LockComponentBeforeDisposing() {
			base.LockComponentBeforeDisposing();
			DestroyAsyncAnimations();
			painting++;
			Documents.UnHookCore();
			FloatDocuments.UnHookCore();
			AppearanceCollection.Changed -= OnAppearanceChanged;
			DocumentProperties.Changed -= OnDocumentPropertiesChanged;
			DocumentSelectorProperties.Changed -= OnDocumentSelectorPropertiesChanged;
			FloatPanels.CollectionChanged -= OnFloatPanelCollectionChanged;
			FloatDocuments.CollectionChanged -= OnFloatDocumentCollectionChanged;
			Documents.CollectionChanged -= OnDocumentCollectionChanged;
			WindowsDialogProperties.Changed -= OnWindowsDialogPropertiesChanged;
		}
		protected override void OnDispose() {
			Unload();
			Ref.Dispose(ref waitScreenAdorner);
			Ref.Dispose(ref loadingIndicatorPropertiesCore);
			Ref.Dispose(ref appearanceCollectionCore);
			Ref.Dispose(ref documentPropertiesCore);
			Ref.Dispose(ref floatPanelsCore);
			Ref.Dispose(ref floatDocumentsCore);
			Ref.Dispose(ref documentsCore);
			Ref.Dispose(ref controllerCore);
			documentSelectorPropertiesCore = null;
			windowsDialogPropertiesCore = null;
			activeDocumentCore = null;
			activeFloatDocumentCore = null;
			managerCore = null;
			gInfo = null;
			base.OnDispose();
		}
		protected virtual IBaseDocumentProperties CreateDocumentProperties() {
			return new BaseDocumentProperties();
		}
		void OnDocumentPropertiesChanged(object sender, EventArgs e) {
			using(LockPainting()) {
				RequestInvokePatchActiveChild();
			}
		}
		protected virtual bool IsDocking {
			get { return Manager != null && Manager.IsDocking; }
		}
		[Browsable(false)]
		public abstract ViewType Type { get; }
		protected internal BaseViewPainter Painter {
			get { return painterCore; }
		}
		protected internal BaseViewInfo ViewInfo {
			get { return viewInfoCore; }
		}
		[Browsable(false)]
		public virtual bool IsFocused {
			get { return IsLoaded && focused > 0; }
		}
		int focused;
		protected virtual bool ShouldRecalculateLayoutOnFocusChange {
			get { return false; }
		}
		protected internal bool FocusEnter() {
			if(0 == focused++) {
				OnGotFocus();
				if(ShouldRecalculateLayoutOnFocusChange) {
					using(LockComponentChangeNotifications())
						LayoutChanged();
				}
				return true;
			}
			return false;
		}
		protected internal bool FocusLeave() {
			if(focused > 0) {
				OnLostFocus();
				focused = 0;
				if(Manager != null) {
					if(ActiveDocument != null) {
						if(ActiveDocument.IsDockPanel) {
							if(Manager.DockManager != null) {
								BarManager barManager = Manager.GetBarManager();
								if(CanCheckFocusedPanel(barManager))
									Manager.DockManager.InvokeCheckFocusedPanel();
							}
						}
						if(Manager.UIViewAdapter != null)
							Manager.UIViewAdapter.UIInteractionService.Reset();
						if(ShouldRecalculateLayoutOnFocusChange) {
							using(LockComponentChangeNotifications())
								LayoutChanged();
						}
					}
				}
				return true;
			}
			return false;
		}
		static bool CanCheckFocusedPanel(BarManager barManager) {
			if(barManager == null) return true;
			return (barManager.SelectionInfo.internalFocusLock == 0) && (barManager.EditorHelper.InternalFocusLock == 0);
		}
		protected virtual void OnGotFocus() {
			RaiseGotFocus();
		}
		protected virtual void OnLostFocus() {
			RaiseLostFocus();
		}
		[ThreadStatic]
		static int lockDeactivateApp = 0;
		protected void LockDeactivateApp() {
			lockDeactivateApp++;
		}
		protected void UnLockDeactivateApp() {
			lockDeactivateApp--;
		}
		protected internal void DeactivateApp() {
			if(lockDeactivateApp > 0) return;
			CancelDragOperation();
		}
		protected void CancelDragOperation() {
			if(!IsDisposing && Manager != null)
				Manager.CancelDragOperation();
		}
		int shownRequested = 0;
		protected bool IsShownRequested {
			get { return shownRequested > 0; }
		}
		protected virtual void OnLoading() {
		}
		protected virtual void OnLoaded() {
			RegisterFloatDocuments();
			RegisterInfos();
			shownRequested++;
		}
		protected virtual void OnUnloaded() {
			paint = 0;
			UnregisterInfos();
			UnRegisterFloatDocuments();
		}
		protected virtual void OnShown() {
			shownRequested = 0;
		}
		protected virtual void RegisterInfos() { }
		protected virtual void UnregisterInfos() { }
		void RegisterFloatDocuments() {
			if(IsDisposing || ViewInfo == null) return;
			ChangeContext.RegisterFloatForms(this);
			foreach(BaseDocument document in FloatDocuments)
				ViewInfo.RegisterFloatDocumentInfo(document);
		}
		void UnRegisterFloatDocuments() {
			if(IsDisposing || ViewInfo == null) return;
			foreach(BaseDocument document in FloatDocuments)
				ViewInfo.UnregisterFloatDocumentInfo(document);
			ChangeContext.UnregisterFloatForms(this);
		}
		#region ClosedContext
		protected internal IDocumentClosedContext BeginRaiseFloatDocumentClosed(BaseDocument document) {
			return !DocumentClosedContext.IsDocumentClosing(this) ? BeginRaiseDocumentClosed(document) : null;
		}
		protected internal IDocumentClosedContext BeginRaiseDocumentClosed(BaseDocument document) {
			return new DocumentClosedContext(this, document);
		}
		protected internal bool IsDocumentClosing() {
			return DocumentClosedContext.IsDocumentClosing(this);
		}
		protected internal bool IsDocumentClosing(BaseDocument document) {
			return DocumentClosedContext.IsDocumentClosing(this, document);
		}
		internal class DocumentClosedContext : IDocumentClosedContext {
			BaseView View;
			BaseDocument Document;
			static IDictionary<BaseView, DocumentClosedContext> contexts =
			   new Dictionary<BaseView, DocumentClosedContext>();
			int documentClosedRequest = 1;
			public DocumentClosedContext(BaseView view, BaseDocument document) {
				contexts.Add(view, this);
				View = view;
				Document = document;
			}
			public void RequestRaiseDocumentClosed() {
				documentClosedRequest++;
			}
			public static bool IsDocumentClosing(BaseView view) {
				return contexts.ContainsKey(view);
			}
			public static bool IsDocumentClosing(BaseView view, BaseDocument document) {
				DocumentClosedContext context;
				return contexts.TryGetValue(view, out context) && context.Document == document;
			}
			public static bool RequestRaiseDocumentClosed(BaseView view, BaseDocument document) {
				DocumentClosedContext context;
				if(contexts.TryGetValue(view, out context) && context.Document == document) {
					context.documentClosedRequest++;
					return true;
				}
				return false;
			}
			public void Flush() {
				if(documentClosedRequest > 0) {
					View.RaiseDocumentClosed(Document);
				}
				documentClosedRequest = 0;
			}
			public void Dispose() {
				Flush();
				contexts.Remove(View);
				View = null;
				Document = null;
			}
		}
		#endregion ClosedContext
		#region IMeasureContext
		protected internal IMeasureContext BeginMeasure() {
			return new MeasureContext(this, null);
		}
		protected internal IMeasureContext BeginMeasure(GraphicsCache cache) {
			return new MeasureContext(this, cache != null ? cache.Graphics : null);
		}
		class MeasureContext : IMeasureContext {
			BaseView view;
			Graphics graphicsCore;
			public MeasureContext(BaseView view, Graphics g) {
				this.view = view;
				if(view != null && view.gInfo != null)
					this.graphicsCore = view.gInfo.AddGraphics(g);
			}
			public Graphics Graphics {
				get { return graphicsCore; }
			}
			public void Dispose() {
				if(view != null && view.gInfo != null)
					view.gInfo.ReleaseGraphics();
				this.graphicsCore = null;
				this.view = null;
			}
		}
		#endregion
		#region ChangeContext
		internal class ChangeContext : IDisposable {
			Form[] floatForms;
			Form activeForm;
			BaseView from, to;
			static IDictionary<BaseView, ChangeContext> contexts =
				new Dictionary<BaseView, ChangeContext>();
			internal ChangeContext(BaseView from, BaseView to) {
				if(from != null)
					contexts.Add(from, this);
				if(to != null)
					contexts.Add(to, this);
				this.from = from;
				this.to = to;
			}
			public static void RegisterFloatForms(BaseView view) {
				ChangeContext context;
				if(contexts.TryGetValue(view, out context) && context.floatForms != null) {
					context.RegisterFloatFormsCore(view);
					if(context.activeForm != null) {
						BaseDocument activeDocument;
						if(view.FloatDocuments.TryGetValue(context.activeForm, out activeDocument))
							view.SetActiveFloatDocumentCore(activeDocument);
					}
				}
			}
			public static void UnregisterFloatForms(BaseView view) {
				ChangeContext context;
				if(contexts.TryGetValue(view, out context)) {
					context.activeForm = (view.ActiveFloatDocument != null) ?
						view.ActiveFloatDocument.Form : null;
					BaseDocument[] floatDocuments = view.FloatDocuments.CleanUp();
					context.UnregisterFloatFormsCore(floatDocuments);
					view.SetActiveFloatDocumentCore(null);
					for(int i = 0; i < floatDocuments.Length; i++) {
						floatDocuments[i].LockFormDisposing();
						floatDocuments[i].Dispose();
						floatDocuments[i].UnLockFormDisposing();
					}
				}
			}
			void UnregisterFloatFormsCore(BaseDocument[] floatDocuments) {
				floatForms = new Form[floatDocuments.Length];
				for(int i = 0; i < floatDocuments.Length; i++)
					floatForms[i] = floatDocuments[i].Form;
			}
			void RegisterFloatFormsCore(BaseView view) {
				using(view.FloatDocuments.LockCollectionChanged()) {
					BaseDocument[] floatDocuments = new BaseDocument[floatForms.Length];
					for(int i = 0; i < floatDocuments.Length; i++)
						floatDocuments[i] = ((IBaseViewControllerInternal)view.Controller).CreateAndInitializeDocument(floatForms[i]);
					view.FloatDocuments.AddRange(floatDocuments);
				}
			}
			public void Dispose() {
				if(from != null)
					contexts.Remove(from);
				if(to != null)
					contexts.Remove(to);
				this.activeForm = null;
				this.floatForms = null;
				this.from = null;
				this.to = null;
			}
		}
		#endregion ChangeContext
		protected internal void Unload() {
			ResetStyle();
			if(IsLoaded) {
				if(Manager != null && Manager.View == this)
					Manager.Depopulate();
				OnUnloaded();
				LayoutHelper.Unregister(this);
				loaded = 0;
			}
			loading = 0;
		}
		protected internal void Load() {
			if(0 == loading++)
				OnLoading();
			if(IsInitializing) return;
			if(0 == loaded++) {
				loading = 0;
				InitStyleCore();
				LayoutHelper.Register(this);
				OnLoaded();
			}
		}
		#region Painting
		int painting = 0;
		int patchActiveChildrenRequested = 0;
		class PaintLocker : IDisposable {
			BaseView View;
			public PaintLocker(BaseView view) {
				View = view;
				View.BeginUpdate();
				View.painting++;
			}
			public void Dispose() {
				View.CancelUpdate();
				if(--View.painting == 0) {
					if(View.IsLoaded)
						View.ResizeCore();
					if(View.patchActiveChildrenRequested > 0) {
						if(View.Manager != null)
							View.Manager.InvokePatchActiveChildren();
						View.patchActiveChildrenRequested = 0;
					}
					View.InvalidateUIView();
					if(View.IsLoaded)
						View.Invalidate();
				}
			}
		}
		protected internal IDisposable LockPainting() {
			return new PaintLocker(this);
		}
		protected internal bool IsPaintingLocked {
			get { return painting > 0; }
		}
		protected internal virtual bool IgnoreActiveFormOnActivation {
			get { return false; }
		}
		protected internal void RequestInvokePatchActiveChild() {
			patchActiveChildrenRequested++;
		}
		protected bool CanDraw() {
			return !IsDisposing && (ViewInfo != null && Painter != null) && !IsLayoutChangedTracking;
		}
		int paint = 0;
		protected internal void Draw(GraphicsCache cache, Rectangle clip) {
			if(painting > 0 || !CanDraw()) return;
			if(EnsureLayoutCalculated(cache)) {
				painting++;
				try {
					if(0 == paint++)
						OnBeforeFirstDraw();
					ViewInfo.UpdateAppearances();
					Painter.Draw(cache, clip);
				}
				finally {
					RaisePaintEvent(cache.PaintArgs.PaintArgs);
					painting--;
				}
			}
		}
		protected virtual void OnBeforeFirstDraw() { }
		#endregion Painting
		protected internal virtual bool CanShowContextMenu(BaseViewControllerMenu menu, Point point) {
			BaseViewHitInfo hitInfo = Manager.CalcHitInfo(point);
			return !RaisePopupMenuShowing(menu, hitInfo);
		}
		protected internal virtual bool CanShowOverlapWarning() {
			return (Documents.Count == 0) && IsDesignMode();
		}
		protected internal bool SetNextDocument(bool forward) {
			return SetNextDocument(ActiveDocument, forward);
		}
		protected internal bool SetNextDocument(BaseDocument document, bool forward) {
			if(CanSetNextDocument(document, forward)) {
				BaseDocument nextDocument = GetNextDocument(document, forward);
				if(!RaiseNextDocument(document, ref nextDocument, forward))
					return Controller.Activate(nextDocument);
			}
			return true;
		}
		protected virtual bool CanSetNextDocument(BaseDocument document, bool forward) {
			return true;
		}
		protected internal virtual BaseDocument GetNextDocument(BaseDocument document, bool forward) {
			return GetNextDocument(Documents.ToArray(), document, forward);
		}
		protected BaseDocument GetNextDocument(BaseDocument[] documents, BaseDocument document, bool forward) {
			int length = documents.Length;
			int index = Array.IndexOf(documents, document);
			if(forward)
				index = (++index % length);
			else
				index = (--index + length) % length;
			return documents[index];
		}
		int calculating = 0;
		protected internal bool EnsureLayoutCalculated(GraphicsCache cache) {
			if(!ViewInfo.IsReady && calculating == 0) {
				calculating++;
				try {
					using(IMeasureContext context = BeginMeasure(cache)) {
						ViewInfo.Calc(context.Graphics, Bounds);
					}
				}
				finally { calculating--; }
			}
			return ViewInfo.IsReady;
		}
		[Browsable(false)]
		public DocumentManager Manager {
			[System.Diagnostics.DebuggerStepThrough]
			get { return managerCore; }
		}
		protected abstract IBaseViewController CreateController();
		[Browsable(false)]
		public IBaseViewController Controller {
			[System.Diagnostics.DebuggerStepThrough]
			get { return controllerCore; }
		}
		protected virtual internal bool IsSkinPaintStyle {
			get { return ElementsLookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin; }
		}
		protected virtual internal UserLookAndFeel ElementsLookAndFeel {
			get { return (Manager != null) ? Manager.LookAndFeel : UserLookAndFeel.Default; }
		}
		protected virtual internal string PaintStyleName {
			get { return (Manager != null) ? Manager.PaintStyleName : null; }
		}
		protected virtual internal Color ParentBackColor {
			get { return (Manager != null) ? Manager.GetParentBackColor() : Color.Empty; }
		}
		protected virtual internal Color ParentForeColor {
			get { return (Manager != null) ? Manager.GetParentForeColor() : Color.Empty; }
		}
		protected virtual internal Image ParentBackgroundImage {
			get { return (Manager != null) ? Manager.GetParentBackgroundImage() : null; }
		}
		protected virtual internal ImageLayout ParentBackgroundImageLayout {
			get { return (Manager != null) ? Manager.GetParentBackgroundImageLayout() : ImageLayout.None; }
		}
		protected internal Color GetBackColor() {
			return (Painter != null) ? Painter.GetBackColor(ParentBackColor) : ParentBackColor;
		}
		protected virtual internal DocumentManagerAppearances Appearances {
			get { return (Manager != null) ? Manager.GetBarAndDockingController().AppearancesDocumentManager : null; }
		}
		protected internal void SetManager(DocumentManager manager) {
			if(managerCore == manager) return;
			if(manager == null)
				LayoutHelper.Unregister(this);
			managerCore = manager;
			OnManagerChanged();
		}
		Rectangle boundsCore = Rectangle.Empty;
		[Browsable(false)]
		public Rectangle Bounds {
			get { return boundsCore; }
		}
		protected internal virtual bool HasNCElements {
			get { return false; }
		}
		protected internal Rectangle CalcNCBounds(Rectangle bounds) {
			if(IsDisposing || !IsLoaded) return bounds;
			return CalcNCBoundsCore(bounds);
		}
		protected internal void Resize(Rectangle bounds) {
			if(IsDisposing || !(IsLoaded || ((IDesignTimeSupport)this).IsLoaded)) return;
			boundsCore = CalcBounds(bounds);
			ResizeCore();
			Invalidate();
		}
		protected virtual Rectangle CalcNCBoundsCore(Rectangle bounds) {
			return bounds;
		}
		protected virtual Rectangle CalcBounds(Rectangle bounds) {
			int margin = (Painter != null) ? Painter.GetRootMargin() : 0;
			return Rectangle.Inflate(bounds, -margin, -margin);
		}
		void ResizeCore() {
			if(patchChildrenInProgress > 0) return;
			if(!DesignMode)
				LayoutChanged();
			else
				ResetViewInfo();
			if(ViewInfo == null || Bounds.IsEmpty) return;
			if(IsShownRequested)
				OnShown();
			using(IMeasureContext context = BeginMeasure()) {
				ViewInfo.UpdateAppearances();
				ViewInfo.Calc(context.Graphics, Bounds);
			}
		}
		void ResetViewInfo() {
			if(IsUpdateLocked || IsInitializing || IsDisposing) return;
			RaiseLayout();
			if(ViewInfo != null)
				ViewInfo.SetDirty();
		}
		protected internal void Invalidate() {
			if(painting > 0) return;
			if(Manager != null)
				Manager.Invalidate();
		}
		protected internal void Invalidate(Rectangle rect) {
			if(painting > 0) return;
			if(Manager != null)
				Manager.Invalidate(rect);
		}
		protected internal void UpdateStyle() {
			if(!IsLoaded) return;
			using(LockPainting()) {
				UpdateStyleCore();
				UpdateUIChildrenStyle();
				UpdateFloatDocumentsStyle();
				if(ViewInfo != null)
					ViewInfo.SetAppearanceDirty();
				RequestInvokePatchActiveChild();
			}
		}
		protected internal virtual void OnPopulated() {
			Control activeMdiChild = Manager.GetActiveChild();
			if(activeMdiChild != null) {
				BaseDocument document;
				if(Documents.TryGetValue(activeMdiChild, out document)) {
					SetActiveDocumentCore(document);
					RequestInvokePatchActiveChild();
				}
			}
			if(ViewInfo != null)
				ViewInfo.SetDirty();
			UpdateStyle();
		}
		protected internal virtual void OnDepopulating() { }
		protected internal virtual void OnDepopulated() {
			ResetStyle();
			Documents.UnHookCore();
		}
		void UpdateFloatDocumentsStyle() {
			foreach(IFloatDocumentInfo info in ViewInfo.GetFloatDocumentInfos())
				info.UpdateStyle();
		}
		void UpdateUIChildrenStyle() {
			using(IUIElementEnumerator e = new IUIElementEnumerator(this)) {
				while(e.MoveNext()) {
					IBaseElementInfo info = e.Current as IBaseElementInfo;
					if(info != null)
						info.UpdateStyle();
				}
			}
		}
		protected void InitStyleCore() {
			IViewRegistrator registrator = ResolveViewRegistrator();
			if(registrator != null) {
				painterCore = registrator.CreatePainter(this);
				viewInfoCore = registrator.CreateViewInfo(this);
			}
		}
		protected virtual void UpdateStyleCore() {
			IViewRegistrator registrator = ResolveViewRegistrator();
			if(registrator != null) {
				painterCore = registrator.CreatePainter(this);
			}
		}
		protected void ResetStyle() {
			painterCore = null;
		}
		protected internal virtual IViewRegistrator ResolveViewRegistrator() {
			return Manager as IViewRegistrator;
		}
		protected virtual void OnManagerChanged() {
			ResetStyle();
			foreach(BaseDocument document in Documents)
				document.SetManager(Manager);
			foreach(BaseDocument document in FloatDocuments)
				document.SetManager(Manager);
			foreach(BaseDocument document in FloatPanels)
				document.SetManager(Manager);
		}
		protected override void OnLayoutChanged() {
			if(ViewInfo != null)
				ViewInfo.SetDirty();
			RaiseLayout();
			Invalidate();
		}
		protected override void OnInitialized() {
			if(loading > 0) Load();
		}
		protected internal virtual void OnVisibleChanged(bool visible) {
			if(ViewInfo != null)
				ViewInfo.SetDirty();
		}
		protected internal virtual bool OnBeginDocking(BaseDocument document) {
			if(document.IsDocumentsHost)
				return RaiseBeginDocumentsHostDocking(document);
			return RaiseBeginDocking(document);
		}
		protected internal virtual void OnEndDocking(BaseDocument document) {
			if(document.IsDocumentsHost)
				RaiseEndDocumentsHostDocking(document);
			else RaiseEndDocking(document);
		}
		protected internal virtual bool OnBeginFloating(BaseDocument document, FloatingReason reason) {
			return RaiseBeginFloating(document, reason);
		}
		protected internal virtual void OnFloating(BaseDocument document) {
			LockDeactivateApp();
			try {
				RaiseFloating(document);
				if(!UseFloatingDocumentsHost || document.IsDockPanel) return;
				IDocumentsHostWindow hostWindow = DocumentsHostContext.GetDocumentsHostWindow(Manager);
				if(hostWindow != null) {
					DocumentsHostContext context = Manager.EnsureDocumentsHostContext();
					bool isEmpty = hostWindow.DocumentManager.View.Documents.Count == 0;
					if(isEmpty && hostWindow.DestroyOnRemovingChildren)
						context.QueueDelete(document, hostWindow);
				}
			}
			finally { UnLockDeactivateApp(); }
		}
		protected internal virtual void OnEndFloating(BaseDocument document, EndFloatingReason reason) {
			RaiseEndFloating(document, reason);
			if(reason != EndFloatingReason.Docking && document.IsDockPanel) {
				Docking.DockPanel panel = document.GetDockPanel();
				if(panel != null && panel.DockManager != null)
					panel.DockManager.ActivePanel = panel;
			}
			if(!UseFloatingDocumentsHost || document.IsDockPanel) return;
			LockDeactivateApp();
			try {
				Form floatForm = document.Form;
				DocumentsHostContext context = Manager.EnsureDocumentsHostContext();
				if(reason == EndFloatingReason.Docking) {
					FloatDocuments.Remove(document);
					document.SetIsFloating(true);
					context.DequeueOnDocking(document);
				}
				else {
					IDocumentsHostWindow hostWindow = context.DequeueOnEndDragging(document, this);
					if(hostWindow != null) {
						if(Container != null)
							Container.Remove(document);
						FloatDocuments.Remove(document);
						using(BatchUpdate.Enter(hostWindow.DocumentManager)) {
							BaseView hostView = hostWindow.DocumentManager.View;
							hostView.LockDeactivateApp();
							try {
								hostView.Documents.Add(document);
								hostWindow.DocumentManager.DockFloatForm(document);
								hostWindow.Show();
							}
							finally {
								hostView.UnLockDeactivateApp();
							}
						}
					}
				}
			}
			finally { UnLockDeactivateApp(); }
		}
		int loading, loaded;
		[Browsable(false)]
		public bool IsLoaded {
			get { return loading == 0 && loaded > 0; }
		}
		[Browsable(false)]
		public bool IsLoading {
			get { return loading > 0; }
		}
		Image backgroundImageCore;
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BaseViewBackgroundImage")]
#endif
		[DefaultValue(null), Category("Appearance"), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(System.Drawing.Design.UITypeEditor))]
		public Image BackgroundImage {
			get { return backgroundImageCore; }
			set {
				if(BackgroundImage == value) return;
				backgroundImageCore = value;
				LayoutChanged();
			}
		}
		ImageLayoutMode backgroundImageLayoutModeCore = ImageLayoutMode.Default;
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BaseViewBackgroundImageLayoutMode")]
#endif
		[DefaultValue(ImageLayoutMode.Default), Category("Appearance")]
		public ImageLayoutMode BackgroundImageLayoutMode {
			get { return backgroundImageLayoutModeCore; }
			set {
				if(BackgroundImageLayoutMode == value) return;
				backgroundImageLayoutModeCore = value;
				LayoutChanged();
			}
		}
		Padding? backgroundImageStretchMarginsCore;
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BaseViewBackgroundImageStretchMargins")]
#endif
		[DefaultValue(null), Category("Appearance")]
		public Padding? BackgroundImageStretchMargins {
			get { return backgroundImageStretchMarginsCore; }
			set {
				if(BackgroundImageStretchMargins == value) return;
				backgroundImageStretchMarginsCore = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseViewAppearance"),
#endif
 Category("Appearance"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DevExpress.Utils.AppearanceObject Appearance {
			get { return AppearanceCollection.View; }
		}
		DevExpress.Utils.BaseAppearanceCollection IAppearanceCollectionAccessor.Appearances {
			get { return appearanceCollectionCore; }
		}
		BaseViewAppearanceCollection appearanceCollectionCore;
		protected BaseViewAppearanceCollection AppearanceCollection {
			get { return appearanceCollectionCore; }
		}
		protected virtual BaseViewAppearanceCollection CreateAppearanceCollection() {
			return new BaseViewAppearanceCollection(this);
		}
		bool ShouldSerializeAppearance() {
			return !IsDisposing && AppearanceCollection != null && Appearance.ShouldSerialize();
		}
		void ResetAppearance() {
			Appearance.Reset();
		}
		void OnAppearanceChanged(object sender, EventArgs e) {
			OnAppearanceChanged();
		}
		protected virtual void OnAppearanceChanged() {
			if(ViewInfo != null)
				ViewInfo.SetAppearanceDirty();
			LayoutChanged();
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BaseViewFloatingDocumentContainer")]
#endif
		[Category("Behavior"), DefaultValue(FloatingDocumentContainer.Default), XtraSerializableProperty]
		public virtual FloatingDocumentContainer FloatingDocumentContainer {
			get { return floatingDocumentContainerCore; }
			set { floatingDocumentContainerCore = value; }
		}
		protected virtual bool UseFloatingDocumentsHost {
			get { return FloatingDocumentContainer == FloatingDocumentContainer.DocumentsHost; }
		}
		protected internal bool CanCloseAll() {
			if(Documents.Count + GetFloatDocumentCount() == 0) return false;
			Predicate<BaseDocument> canClose = (d) => !d.IsDisposing && d.Properties.CanClose;
			return Documents.Exists(canClose) || FloatDocuments.Exists(canClose);
		}
		protected internal bool CanCloseAllButThis(BaseDocument document) {
			if(Documents.Count + GetFloatDocumentCount() <= 1) return false;
			Predicate<BaseDocument> canClose = (d) => d != document && d.Properties.CanClose;
			return (Documents.Exists(canClose) || FloatDocuments.Exists(canClose)) || HasDocumentHostsWithoutThisDocument(document);
		}
		protected bool HasDocumentHostsWithoutThisDocument(BaseDocument document) {
			var hostsContext = GetDocumentsHostContext();
			return (hostsContext != null) && hostsContext.CanCloseAllButThis(document, Manager);
		}
		protected internal bool CanFloatAll(BaseDocument[] documents) {
			if(!IsDisposing && !UseFloatingDocumentsHost || documents.Length <= 1) return false;
			return (Array.Find(documents, (d) => d.IsDockPanel) == null) && !DocumentsHostContext.IsChild(Manager);
		}
		protected internal bool CanFloatAll() {
			return CanFloatAll(Documents.ToArray());
		}
		protected internal bool CanFloat(BaseDocument document) {
			if(!UseFloatingDocumentsHost || !Documents.Contains(document)) return true;
			IDocumentsHostWindow hostWindow = DocumentsHostContext.GetDocumentsHostWindow(Manager);
			if(hostWindow == null) return true;
			return !hostWindow.DestroyOnRemovingChildren || Documents.Count > 1;
		}
		protected internal int GetFloatDocumentCount() {
			int floatDocumentsCount = (FloatDocuments != null) ? FloatDocuments.Count : 0;
			var hostsContext = GetDocumentsHostContext();
			if(hostsContext != null)
				floatDocumentsCount += hostsContext.GetSecondaryHostsCount();
			return floatDocumentsCount;
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BaseViewDocumentProperties")]
#endif
		[Category("Layout"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public IBaseDocumentProperties DocumentProperties {
			get { return documentPropertiesCore; }
		}
		bool ShouldSerializeDocumentProperties() {
			return DocumentProperties != null && DocumentProperties.ShouldSerialize();
		}
		void ResetDocumentProperties() {
			DocumentProperties.Reset();
		}
		ILoadingIndicatorProperties loadingIndicatorPropertiesCore;
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BaseViewLoadingIndicatorProperties")]
#endif
		[Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public ILoadingIndicatorProperties LoadingIndicatorProperties {
			get { return loadingIndicatorPropertiesCore; }
		}
		bool ShouldSerializeLoadingIndicatorProperties() {
			return LoadingIndicatorProperties != null && LoadingIndicatorProperties.ShouldSerialize();
		}
		void ResetLoadingIndicatorProperties() {
			LoadingIndicatorProperties.Reset();
		}
		protected virtual ILoadingIndicatorProperties CreateLoadingIndicatorProperties() {
			return new LoadingIndicatorProperties();
		}
		IBaseDocumentSelectorProperties documentSelectorPropertiesCore;
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BaseViewDocumentSelectorProperties")]
#endif
		[Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public IBaseDocumentSelectorProperties DocumentSelectorProperties {
			get { return documentSelectorPropertiesCore; }
		}
		bool ShouldSerializeDocumentSelectorProperties() {
			return !IsDisposing && DocumentSelectorProperties != null && DocumentSelectorProperties.ShouldSerialize();
		}
		void ResetDocumentSelectorProperties() {
			DocumentSelectorProperties.Reset();
		}
		protected virtual IBaseDocumentSelectorProperties CreateDocumentSelectorProperties() {
			return new DocumentSelectorProperties();
		}
		void OnDocumentSelectorPropertiesChanged(object sender, EventArgs e) {
		}
		IWindowsDialogProperties windowsDialogPropertiesCore;
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BaseViewWindowsDialogProperties")]
#endif
		[Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public IWindowsDialogProperties WindowsDialogProperties {
			get { return windowsDialogPropertiesCore; }
		}
		bool ShouldSerializeWindowsDialogProperties() {
			return !IsDisposing && WindowsDialogProperties != null && WindowsDialogProperties.ShouldSerialize();
		}
		void ResetWindowDialogProperties() {
			WindowsDialogProperties.Reset();
		}
		protected virtual IWindowsDialogProperties CreateWindowsDialogProperties() {
			return new WindowsDialogProperties();
		}
		void OnWindowsDialogPropertiesChanged(object sender, EventArgs e) {
		}
		DevExpress.Utils.DefaultBoolean useDocumentSelectorCore = DevExpress.Utils.DefaultBoolean.Default;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseViewUseDocumentSelector"),
#endif
	   Category("Behavior"), DefaultValue(DevExpress.Utils.DefaultBoolean.Default)]
		public virtual DevExpress.Utils.DefaultBoolean UseDocumentSelector {
			get { return useDocumentSelectorCore; }
			set { SetValueCore(ref useDocumentSelectorCore, value); }
		}
		DevExpress.Utils.DefaultBoolean allowHotkeyNavigationCore = DevExpress.Utils.DefaultBoolean.Default;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseViewAllowHotkeyNavigation"),
#endif
		Category("Behavior"), DefaultValue(DevExpress.Utils.DefaultBoolean.Default)]
		public virtual DevExpress.Utils.DefaultBoolean AllowHotkeyNavigation {
			get { return allowHotkeyNavigationCore; }
			set { SetValueCore(ref allowHotkeyNavigationCore, value); }
		}
		void SetValueCore(ref DevExpress.Utils.DefaultBoolean property, DevExpress.Utils.DefaultBoolean value) {
			if(property == value)
				return;
			if(Manager != null && CanUseDocumentSelector())
				Manager.DestroyDocumentSelector();
			property = value;
			if(Manager != null && CanUseDocumentSelector())
				Manager.CreateDocumentSelector();
		}
		protected internal virtual bool CanUseDocumentSelector() {
			return CanHotKeyNavigate() && (Site == null || !Site.DesignMode);
		}
		protected internal virtual bool CanHotKeyNavigate() {
			return UseDocumentSelector != DevExpress.Utils.DefaultBoolean.False || AllowHotkeyNavigation == DevExpress.Utils.DefaultBoolean.True;
		}
		[Browsable(false)]
		public bool IsDocumentSelectorVisible {
			get { return Manager != null && Manager.IsDocumentSelectorVisible; }
		}
		DevExpress.Utils.DefaultBoolean useLoadingIndicatorCore = DevExpress.Utils.DefaultBoolean.Default;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseViewUseLoadingIndicator"),
#endif
		Category("Behavior"), DefaultValue(DevExpress.Utils.DefaultBoolean.Default)]
		public virtual DevExpress.Utils.DefaultBoolean UseLoadingIndicator {
			get { return useLoadingIndicatorCore; }
			set { useLoadingIndicatorCore = value; }
		}
		protected internal virtual bool CanUseLoadingIndicator() {
			return UseLoadingIndicator != DevExpress.Utils.DefaultBoolean.False && (Site == null || !Site.DesignMode);
		}
		DevExpress.Utils.DefaultBoolean useSnappingEmulationCore = DevExpress.Utils.DefaultBoolean.Default;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseViewUseSnappingEmulation"),
#endif
		Category("Behavior"), DefaultValue(DevExpress.Utils.DefaultBoolean.Default)]
		public DevExpress.Utils.DefaultBoolean UseSnappingEmulation {
			get { return useSnappingEmulationCore; }
			set { useSnappingEmulationCore = value; }
		}
		protected internal virtual bool CanUseSnappingEmulation() {
			return UseSnappingEmulation != DevExpress.Utils.DefaultBoolean.False && Dragging.SnapSystemParameters.GetAllowDockMoving();
		}
		DevExpress.Utils.DefaultBoolean allowResetLayoutCore = DevExpress.Utils.DefaultBoolean.Default;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseViewAllowResetLayout"),
#endif
		Category("Behavior"), DefaultValue(DevExpress.Utils.DefaultBoolean.Default)]
		public virtual DevExpress.Utils.DefaultBoolean AllowResetLayout {
			get { return allowResetLayoutCore; }
			set { allowResetLayoutCore = value; }
		}
		protected internal virtual void AssignProperties(BaseView parentView) {
			this.allowHotkeyNavigationCore = parentView.AllowHotkeyNavigation;
			this.allowResetLayoutCore = parentView.AllowResetLayout;
			this.floatingDocumentContainerCore = parentView.FloatingDocumentContainer;
			this.showDockGuidesOnPressingShiftCore = parentView.ShowDockGuidesOnPressingShiftBase;
			this.useDocumentSelectorCore = parentView.UseDocumentSelector;
			this.useLoadingIndicatorCore = parentView.UseLoadingIndicator;
			this.useSnappingEmulationCore = parentView.UseSnappingEmulation;
			this.AppearanceCollection.AssignInternal(parentView.AppearanceCollection);
			this.DocumentProperties.Assign(parentView.DocumentProperties);
			this.DocumentSelectorProperties.Assign(parentView.DocumentSelectorProperties);
			this.LoadingIndicatorProperties.Assign(parentView.LoadingIndicatorProperties);
			this.OptionsLayout.Assign(parentView.OptionsLayout);
			this.WindowsDialogProperties.Assign(parentView.WindowsDialogProperties);
		}
		BaseDocument activeDocumentCore;
		BaseDocument activeFloatDocumentCore;
		BaseDocumentCollection documentsCore;
		[Browsable(false), Category("Layout"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BaseDocumentCollection Documents {
			get { return documentsCore; }
		}
		FloatDocumentCollection floatDocumentsCore;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public FloatDocumentCollection FloatDocuments {
			get { return floatDocumentsCore; }
		}
		FloatPanelCollection floatPanelsCore;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public FloatPanelCollection FloatPanels {
			get { return floatPanelsCore; }
		}
		[Browsable(false)]
		public BaseDocument ActiveDocument {
			get { return activeDocumentCore; }
		}
		[Browsable(false)]
		public BaseDocument ActiveFloatDocument {
			get { return activeFloatDocumentCore; }
		}
		[Browsable(false)]
		public bool IsEmpty {
			get { return (documentsCore == null) || Documents.Count == 0; }
		}
		protected internal abstract bool AllowMdiLayout { get; }
		protected internal abstract bool AllowMdiSystemMenu { get; }
		protected virtual BaseDocumentCollection CreateDocumentCollection() {
			return new BaseDocumentCollection(this);
		}
		protected virtual FloatDocumentCollection CreateFloatDocumentCollection() {
			return new FloatDocumentCollection(this);
		}
		protected virtual FloatPanelCollection CreateFloatPanelCollection() {
			return new FloatPanelCollection(this);
		}
		protected internal virtual BaseViewHitInfo CreateHitInfo() {
			BaseViewHitInfo result = null;
			IViewRegistrator registrator = ResolveViewRegistrator();
			if(registrator != null)
				result = registrator.CreateHitInfo(this);
			return result;
		}
		protected internal virtual BaseDocument CreateDocument(Control control) {
			BaseDocument result = null;
			IViewRegistrator registrator = ResolveViewRegistrator();
			if(registrator != null)
				result = registrator.CreateDocument(this, control);
			return result;
		}
		protected internal virtual DocumentContainer CreateDocumentContainer(BaseDocument document) {
			DocumentContainer result = null;
			IViewRegistrator registrator = ResolveViewRegistrator();
			if(registrator != null)
				result = registrator.CreateDocumentContainer(document);
			return result;
		}
		internal int lockActivateDocument = 0;
		public void ActivateDocument(Control control) {
			if(control == null || lockActivateDocument > 0) return;
			BaseDocument document = null;
			lockActivateDocument++;
			try {
				if(Documents.TryGetValue(control, out document)) {
					if(Controller.Activate(document)) {
						SetActiveDocumentCore(document);
					}
				}
				if(FloatDocuments.TryGetValue(control, out document)) {
					if(Controller.Activate(document)) {
						SetActiveFloatDocumentCore(document);
					}
				}
			}
			finally { lockActivateDocument--; }
		}
		protected internal void SetActiveDocumentCore(BaseDocument document) {
			if(ActiveDocument == document) return;
			BeforeActiveDocumentChanged(document);
			if(ActiveDocument != null)
				OnDocumentDeactivated(ActiveDocument);
			activeDocumentCore = document;
			if(ActiveDocument != null)
				OnDocumentActivated(ActiveDocument);
			using(LockComponentChangeNotifications())
				LayoutChanged();
		}
		protected void SetActiveFloatDocumentCore(BaseDocument document) {
			if(ActiveFloatDocument == document) return;
			if(ActiveFloatDocument != null)
				OnDocumentDeactivated(ActiveFloatDocument);
			activeFloatDocumentCore = document;
			if(ActiveFloatDocument != null)
				OnDocumentActivated(ActiveFloatDocument);
		}
		public BaseDocument AddFloatDocument(Control control) {
			if(IsDeserializing) return null;
			BaseDocument document;
			if(control != null && Documents.TryGetValue(control, out document)) {
				Controller.Float(document);
				return document;
			}
			else return ((IBaseViewControllerInternal)Controller).AddFloatDocument(control);
		}
		public BaseDocument AddDocument(string caption, string controlName) {
			BaseDocument result = null;
			IViewRegistrator registrator = ResolveViewRegistrator();
			if(registrator != null) {
				result = registrator.CreateDocument(this, null);
				result.MarkAsDeferredControlLoad();
				result.BeginUpdate();
				result.Caption = caption;
				result.ControlName = controlName;
				result.CancelUpdate();
				Documents.Add(result);
				try {
					if(result.Site != null && !string.IsNullOrEmpty(controlName) && !controlName.Contains("document")) {
						result.Site.Name = controlName.Substring(0, 1).ToLower() +
							controlName.Substring(1) + "Document";
					}
				}
				catch { }
			}
			return result;
		}
		public BaseDocument AddDocument(Control control) {
			if(IsDeserializing) return null;
			if(control is Form && (Manager != null) && !Manager.IsMdiStrategyInUse)
				throw new NotSupportedException(ContainerControlManagementStrategy.AddToDocumentHostError);
			BaseDocument floatDocument;
			if(control != null && TryGetFloatDocument(control, out floatDocument)) {
				Controller.Dock(floatDocument);
				return floatDocument;
			}
			else return ((IBaseViewControllerInternal)Controller).AddDocument(control);
		}
		bool TryGetFloatDocument(Control control, out BaseDocument document) {
			var hostsContext = GetDocumentsHostContext();
			if(hostsContext != null)
				return hostsContext.TryGetFloatDocument(control, out document);
			return FloatDocuments.TryGetValue(control, out document);
		}
		DocumentsHostContext GetDocumentsHostContext() {
			return (Manager != null) ? Manager.GetDocumentsHostContext() : null;
		}
		protected internal int releasingDeferredControlLoadDocument;
		protected internal bool IsReleasingDeferredControlLoadDocument {
			get { return releasingDeferredControlLoadDocument > 0; }
		}
		public bool AddFloatingDocumentsHost(BaseDocument[] documents) {
			if(!CanFloatAll(documents)) return false;
			LockDeactivateApp();
			try {
				DocumentsHostContext context = Manager.EnsureDocumentsHostContext();
				IDocumentsHostWindow hostWindow = null;
				for(int i = 0; i < documents.Length; i++) {
					BaseDocument document = documents[i];
					if(document.IsDeferredControlLoad) {
						if(!document.EnsureIsBoundToControl(this))
							return false;
					}
					if(i == 0) {
						document.Form.Location = GetFloatLocation(document);
						document.Form.Size = GetBounds(document).Size;
						hostWindow = context.DequeueOnEndDragging(document, this);
					}
					if(Container != null)
						Container.Remove(document);
					FloatDocuments.Remove(document);
					Documents.Remove(document);
					Manager.RemoveDocumentFromHost(document);
					hostWindow.DocumentManager.View.Documents.Add(document);
					hostWindow.DocumentManager.DockFloatForm(document);
				}
				if(hostWindow != null) {
					using(BatchUpdate.Enter(hostWindow.DocumentManager))
						hostWindow.Show();
				}
				return (hostWindow != null);
			}
			finally { UnLockDeactivateApp(); }
		}
		public bool AddFloatingDocumentsHost(BaseDocument document) {
			if(IsDisposing || document == null || document.IsDockPanel || !UseFloatingDocumentsHost) return false;
			LockDeactivateApp();
			try {
				if(Documents.Contains(document)) {
					if(document.IsDeferredControlLoad) {
						if(!document.EnsureIsBoundToControl(this))
							return false;
					}
					Point location = GetFloatLocation(document);
					Manager.RemoveDocumentFromHost(document);
					Documents.Remove(document);
					document.Form.Location = location;
				}
				FloatDocuments.Remove(document);
				return AddFloatingDocumentHostCore(document);
			}
			finally { UnLockDeactivateApp(); }
		}
		public bool AddFloatingDocumentsHost(Control control) {
			if(IsDisposing || control == null || control is Docking.DockPanel || !UseFloatingDocumentsHost) return false;
			LockDeactivateApp();
			try {
				BaseDocument document;
				if(Documents.TryGetValue(control, out document)) {
					Point location = GetFloatLocation(document);
					Manager.RemoveDocumentFromHost(document);
					Documents.Remove(document);
					document.Form.Location = location;
				}
				if(document == null && FloatDocuments.TryGetValue(control, out document))
					FloatDocuments.Remove(document);
				if(document == null)
					document = ((IBaseViewControllerInternal)Controller).CreateAndInitializeDocument(control);
				return AddFloatingDocumentHostCore(document);
			}
			finally { UnLockDeactivateApp(); }
		}
		bool AddFloatingDocumentHostCore(BaseDocument document) {
			if(Container != null)
				Container.Remove(document);
			DocumentsHostContext context = Manager.EnsureDocumentsHostContext();
			IDocumentsHostWindow hostWindow = context.DequeueOnEndDragging(document, this);
			if(hostWindow != null) {
				using(BatchUpdate.Enter(hostWindow.DocumentManager)) {
					hostWindow.DocumentManager.View.Documents.Add(document);
					hostWindow.DocumentManager.DockFloatForm(document);
					hostWindow.Show();
				}
			}
			return hostWindow != null;
		}
		int controlRemoving;
		public void RemoveDocument(Control control) {
			if(controlRemoving > 0) return;
			controlRemoving++;
			((IBaseViewControllerInternal)Controller).RemoveDocument(control);
			controlRemoving--;
		}
		void OnFloatDocumentCollectionChanged(CollectionChangedEventArgs<BaseDocument> ea) {
			BaseDocument document = ea.Element;
			CheckActiveFloatDocument(ea);
			if(ea.ChangedType == CollectionChangedType.ElementAdded)
				OnFloatDocumentAdded(document);
			if(ea.ChangedType == CollectionChangedType.ElementRemoved)
				OnFloatDocumentRemoved(document);
		}
		void OnFloatPanelCollectionChanged(CollectionChangedEventArgs<BaseDocument> ea) {
			BaseDocument document = ea.Element;
			if(ea.ChangedType == CollectionChangedType.ElementAdded)
				OnFloatPanelAdded(document);
			if(ea.ChangedType == CollectionChangedType.ElementRemoved)
				OnFloatPanelRemoved(document);
		}
		void OnDocumentCollectionChanged(CollectionChangedEventArgs<BaseDocument> ea) {
			using(var update = BatchUpdate.Enter(Manager)) {
				BaseDocument document = ea.Element;
				CheckActiveDocument(ea);
				if(ea.ChangedType == CollectionChangedType.ElementAdded) {
					OnDocumentAdded(document);
					RaiseDocumentAdded(document);
				}
				if(ea.ChangedType == CollectionChangedType.ElementRemoved) {
					OnDocumentRemoved(document);
					RaiseDocumentRemoved(document);
				}
				if(update != null && !IsLayoutModified)
					update.Cancel();
				LayoutChanged();
			}
		}
		void CheckActiveDocument(CollectionChangedEventArgs<BaseDocument> ea) {
			if(ea.ChangedType == CollectionChangedType.ElementDisposed || ea.ChangedType == CollectionChangedType.ElementRemoved) {
				if(ActiveDocument == ea.Element)
					SetActiveDocumentCore(null);
			}
		}
		void CheckActiveFloatDocument(CollectionChangedEventArgs<BaseDocument> ea) {
			if(ea.ChangedType == CollectionChangedType.ElementDisposed || ea.ChangedType == CollectionChangedType.ElementRemoved) {
				if(ActiveFloatDocument == ea.Element)
					SetActiveFloatDocumentCore(null);
			}
		}
		protected virtual void OnDocumentDeactivated(BaseDocument document) {
			RaiseDocumentDeactivated(document.SetIsActive(false));
		}
		protected virtual void BeforeActiveDocumentChanged(BaseDocument baseDocument) { }
		protected virtual void OnDocumentActivated(BaseDocument document) {
			RaiseDocumentActivated(document.SetIsActive(true));
		}
		protected virtual void OnFloatDocumentAdded(BaseDocument document) {
			document.SetIsFloating(true);
			if(ViewInfo != null) ViewInfo.RegisterFloatDocumentInfo(document);
		}
		protected virtual void OnFloatDocumentRemoved(BaseDocument document) {
			if(Manager != null)
				Manager.CheckDocumentSelectorVisibility();
			if(ViewInfo != null) ViewInfo.UnregisterFloatDocumentInfo(document);
			document.SetIsFloating(false);
		}
		protected virtual void OnFloatPanelAdded(BaseDocument document) {
			document.SetIsFloating(true);
			if(ViewInfo != null) ViewInfo.RegisterFloatPanelInfo(document);
		}
		protected virtual void OnFloatPanelRemoved(BaseDocument document) {
			if(ViewInfo != null) ViewInfo.UnregisterFloatPanelInfo(document);
			document.SetIsFloating(false);
		}
		protected internal virtual void OnDeferredLoadDocumentControlShown(Control control, BaseDocument document) {
			RaiseControlShown(document, control);
		}
		protected internal virtual void OnDeferredLoadDocumentControlLoaded(Control control, BaseDocument document) {
			Documents.RegisterControl(control, document);
			RaiseControlLoaded(document, control);
		}
		protected internal virtual void OnDeferredLoadDocumentControlReleased(Control control, BaseDocument document) {
			Documents.UnregisterControl(control);
			RaiseControlReleased(document, control);
		}
		protected virtual void OnDocumentAdded(BaseDocument document) {
			document.Properties.EnsureParentProperties(DocumentProperties);
			InvalidateUIView();
		}
		protected virtual void OnDocumentRemoved(BaseDocument document) {
			if(Manager != null)
				Manager.CheckDocumentSelectorVisibility();
			InvalidateUIView();
		}
		protected internal void AddToContainer(IComponent component) {
			if(Container != null && DesignMode) {
				if(ViewInfo != null)
					ViewInfo.SetDirty();
				if(!BaseDocument.IsInContainerChange(component as BaseDocument))
					Container.Add(component);
				INamed namedComponent = component as INamed;
				if(namedComponent != null) {
					if(string.IsNullOrEmpty(namedComponent.Name)) {
						namedComponent.Name = component.Site.Name;
					}
					else {
						if(IsDeserializing)
							component.Site.Name = namedComponent.Name;
					}
				}
			}
		}
		protected internal void RemoveFromContainer(IComponent component) {
			if(Container != null && DesignMode) {
				if(ViewInfo != null)
					ViewInfo.SetDirty();
				if(!BaseDocument.IsInContainerChange(component as BaseDocument))
					Container.Remove(component);
			}
		}
		protected internal virtual void OnFloatMDIChildActivated(Form form) {
			BaseDocument document;
			if(FloatDocuments.TryGetFloatDocument(form, out document)) {
				SetActiveFloatDocumentCore(document);
			}
		}
		protected internal virtual void OnFloatMDIChildDeactivated(Form form) {
			BaseDocument document;
			if(FloatDocuments.TryGetFloatDocument(form, out document)) {
				SetActiveFloatDocumentCore(null);
			}
		}
		protected void InvalidateUIView() {
			if(Manager != null && Manager.UIView != null)
				Manager.UIView.Invalidate();
		}
		protected internal int patchChildrenInProgress = 0;
		protected internal virtual bool ShouldRetryPatchActiveChildren(Size size) {
			return Bounds.Size != size;
		}
		protected internal abstract void PatchActiveChildren(Point offset);
		protected internal abstract void PatchBeforeActivateChild(Control activatedChild, Point offset);
		#region Events
		static readonly object layout = new object();
		static readonly object layoutUpgrade = new object();
		static readonly object queryControl = new object();
		static readonly object controlReleasing = new object();
		static readonly object controlShown = new object();
		static readonly object controlLoaded = new object();
		static readonly object controlReleased = new object();
		static readonly object documentAdded = new object();
		static readonly object documentRemoved = new object();
		static readonly object documentClosing = new object();
		static readonly object documentClosed = new object();
		static readonly object documentActivated = new object();
		static readonly object documentDeactivated = new object();
		static readonly object nextDocument = new object();
		static readonly object showingDockGuides = new object();
		static readonly object popupMenuShowing = new object();
		static readonly object beginFloating = new object();
		static readonly object floating = new object();
		static readonly object endFloating = new object();
		static readonly object beginDocking = new object();
		static readonly object endDocking = new object();
		static readonly object beginDocumentsHostDocking = new object();
		static readonly object endDocumentsHostDocking = new object();
		static readonly object gotFocus = new object();
		static readonly object lostFocus = new object();
		static readonly object documentSelectorShown = new object();
		static readonly object documentSelectorHidden = new object();
		static readonly object customDocumentsHostWindow = new object();
		static readonly object registerDocumentsHostWindow = new object();
		static readonly object unregisterDocumentsHostWindow = new object();
		static readonly object emptyDocumentsHostWindow = new object();
		static readonly object layoutReset = new object();
		static readonly object layoutResetting = new object();
		static readonly object customDrawBackground = new object();
		static readonly object beginSizing = new object();
		static readonly object endSizing = new object();
		static readonly object paintCore = new object();
		static readonly object documentSelectorCustomSortItems = new object();
		static readonly object loadingIndicatorShowing = new object();
		[Category(DevExpress.XtraEditors.CategoryName.CustomDraw)]
		public event PaintEventHandler Paint {
			add { Events.AddHandler(paintCore, value); }
			remove { Events.RemoveHandler(paintCore, value); }
		}
		[Category("Layout")]
		public event LayoutEndSizingEventHandler EndSizing {
			add { Events.AddHandler(endSizing, value); }
			remove { Events.RemoveHandler(endSizing, value); }
		}
		[Category("Layout")]
		public event LayoutBeginSizingEventHandler BeginSizing {
			add { Events.AddHandler(beginSizing, value); }
			remove { Events.RemoveHandler(beginSizing, value); }
		}
		[Category("Layout")]
		public event EventHandler Layout {
			add { Events.AddHandler(layout, value); }
			remove { Events.RemoveHandler(layout, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.CustomDraw)]
		public event CustomDrawBackgroundEventHandler CustomDrawBackground {
			add { Events.AddHandler(customDrawBackground, value); }
			remove { Events.RemoveHandler(customDrawBackground, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public event EventHandler DocumentSelectorShown {
			add { Events.AddHandler(documentSelectorShown, value); }
			remove { Events.RemoveHandler(documentSelectorShown, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public event EventHandler DocumentSelectorHidden {
			add { Events.AddHandler(documentSelectorHidden, value); }
			remove { Events.RemoveHandler(documentSelectorHidden, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Focus)]
		public event EventHandler GotFocus {
			add { Events.AddHandler(gotFocus, value); }
			remove { Events.RemoveHandler(gotFocus, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Focus)]
		public event EventHandler LostFocus {
			add { Events.AddHandler(lostFocus, value); }
			remove { Events.RemoveHandler(lostFocus, value); }
		}
		[Category("Layout")]
		public event DocumentEventHandler DocumentAdded {
			add { Events.AddHandler(documentAdded, value); }
			remove { Events.RemoveHandler(documentAdded, value); }
		}
		[Category("Layout")]
		public event DocumentEventHandler DocumentRemoved {
			add { Events.AddHandler(documentRemoved, value); }
			remove { Events.RemoveHandler(documentRemoved, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Events)]
		public event DocumentEventHandler DocumentActivated {
			add { Events.AddHandler(documentActivated, value); }
			remove { Events.RemoveHandler(documentActivated, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Events)]
		public event DocumentEventHandler DocumentDeactivated {
			add { Events.AddHandler(documentDeactivated, value); }
			remove { Events.RemoveHandler(documentDeactivated, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public event DocumentCancelEventHandler DocumentClosing {
			add { Events.AddHandler(documentClosing, value); }
			remove { Events.RemoveHandler(documentClosing, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public event DocumentEventHandler DocumentClosed {
			add { Events.AddHandler(documentClosed, value); }
			remove { Events.RemoveHandler(documentClosed, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public event NextDocumentEventHandler NextDocument {
			add { Events.AddHandler(nextDocument, value); }
			remove { Events.RemoveHandler(nextDocument, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public event ShowingDockGuidesEventHandler ShowingDockGuides {
			add { Events.AddHandler(showingDockGuides, value); }
			remove { Events.RemoveHandler(showingDockGuides, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public event PopupMenuShowingEventHandler PopupMenuShowing {
			add { Events.AddHandler(popupMenuShowing, value); }
			remove { Events.RemoveHandler(popupMenuShowing, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public event DocumentCancelEventHandler BeginFloating {
			add { Events.AddHandler(beginFloating, value); }
			remove { Events.RemoveHandler(beginFloating, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public event DocumentEventHandler Floating {
			add { Events.AddHandler(floating, value); }
			remove { Events.RemoveHandler(floating, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public event DocumentEventHandler EndFloating {
			add { Events.AddHandler(endFloating, value); }
			remove { Events.RemoveHandler(endFloating, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public event DocumentCancelEventHandler BeginDocking {
			add { Events.AddHandler(beginDocking, value); }
			remove { Events.RemoveHandler(beginDocking, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public event DocumentEventHandler EndDocking {
			add { Events.AddHandler(endDocking, value); }
			remove { Events.RemoveHandler(endDocking, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public event DocumentCancelEventHandler BeginDocumentsHostDocking {
			add { Events.AddHandler(beginDocumentsHostDocking, value); }
			remove { Events.RemoveHandler(beginDocumentsHostDocking, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public event DocumentEventHandler EndDocumentsHostDocking {
			add { Events.AddHandler(endDocumentsHostDocking, value); }
			remove { Events.RemoveHandler(endDocumentsHostDocking, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public event CustomDocumentsHostWindowEventHandler CustomDocumentsHostWindow {
			add { Events.AddHandler(customDocumentsHostWindow, value); }
			remove { Events.RemoveHandler(customDocumentsHostWindow, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public event DocumentsHostWindowEventHandler RegisterDocumentsHostWindow {
			add { Events.AddHandler(registerDocumentsHostWindow, value); }
			remove { Events.RemoveHandler(registerDocumentsHostWindow, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public event DocumentsHostWindowEventHandler UnregisterDocumentsHostWindow {
			add { Events.AddHandler(unregisterDocumentsHostWindow, value); }
			remove { Events.RemoveHandler(unregisterDocumentsHostWindow, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public event EmptyDocumentsHostWindowEventHandler EmptyDocumentsHostWindow {
			add { Events.AddHandler(emptyDocumentsHostWindow, value); }
			remove { Events.RemoveHandler(emptyDocumentsHostWindow, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Events)]
		public event LayoutResettingEventHandler LayoutResetting {
			add { Events.AddHandler(layoutResetting, value); }
			remove { Events.RemoveHandler(layoutResetting, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Events)]
		public event EventHandler LayoutReset {
			add { Events.AddHandler(layoutReset, value); }
			remove { Events.RemoveHandler(layoutReset, value); }
		}
		[Category("Deferred Control Load Events")]
		public event QueryControlEventHandler QueryControl {
			add { Events.AddHandler(queryControl, value); }
			remove { Events.RemoveHandler(queryControl, value); }
		}
		protected internal bool HasQueryControlSubscription {
			get { return !IsDisposing && Events[queryControl] != null; }
		}
		[Category("Deferred Control Load Events")]
		public event ControlReleasingEventHandler ControlReleasing {
			add { Events.AddHandler(controlReleasing, value); }
			remove { Events.RemoveHandler(controlReleasing, value); }
		}
		[Category("Deferred Control Load Events")]
		public event DeferredControlLoadEventHandler ControlLoaded {
			add { Events.AddHandler(controlLoaded, value); }
			remove { Events.RemoveHandler(controlLoaded, value); }
		}
		[Category("Deferred Control Load Events")]
		public event DeferredControlLoadEventHandler ControlShown {
			add { Events.AddHandler(controlShown, value); }
			remove { Events.RemoveHandler(controlShown, value); }
		}
		[Category("Deferred Control Load Events")]
		public event DeferredControlLoadEventHandler ControlReleased {
			add { Events.AddHandler(controlReleased, value); }
			remove { Events.RemoveHandler(controlReleased, value); }
		}
		[Category("Document Selector")]
		public event DocumentSelectorCustomSortItemsEventHandler DocumentSelectorCustomSortItems {
			add { Events.AddHandler(documentSelectorCustomSortItems, value); }
			remove { Events.RemoveHandler(documentSelectorCustomSortItems, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public event LoadingIndicatorShowingEventHandler LoadingIndicatorShowing {
			add { Events.AddHandler(loadingIndicatorShowing, value); }
			remove { Events.RemoveHandler(loadingIndicatorShowing, value); }
		}
		int layoutModified = 0;
		protected internal bool IsLayoutModified {
			get { return layoutModified > 0; }
		}
		protected internal void SetLayoutModified() {
			layoutModified++;
		}
		void RaisePaintEvent(PaintEventArgs args) {
			PaintEventHandler paintEvent = Events[paintCore] as PaintEventHandler;
			if(paintEvent == null) return;
			paintEvent.Invoke(this, args);
		}
		protected internal void RaiseLayout() {
			if(IsLayoutModified) {
				EventHandler handler = (EventHandler)Events[layout];
				if(handler != null)
					handler(this, EventArgs.Empty);
				layoutModified = 0;
			}
		}
		protected internal bool RaiseBeginSizing(IBaseSplitterInfo splitInfo) {
			LayoutBeginSizingEventHandler handler = (LayoutBeginSizingEventHandler)Events[beginSizing];
			LayoutBeginSizingEventArgs args = new LayoutBeginSizingEventArgs(splitInfo);
			if(handler != null)
				handler(this, args);
			return !args.Cancel;
		}
		protected internal bool RaiseEndSizing(int change, IBaseSplitterInfo splitInfo) {
			LayoutEndSizingEventHandler handler = (LayoutEndSizingEventHandler)Events[endSizing];
			LayoutEndSizingEventArgs args = new LayoutEndSizingEventArgs(change, splitInfo);
			if(handler != null)
				handler(this, args);
			return !args.Cancel;
		}
		protected internal void RaiseLayoutReset() {
			EventHandler handler = (EventHandler)Events[layoutReset];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected internal bool RaiseLayoutResetting() {
			LayoutResettingEventHandler handler = (LayoutResettingEventHandler)Events[layoutResetting];
			LayoutResettingEventArgs ea = new LayoutResettingEventArgs();
			if(handler != null) handler(this, ea);
			return !ea.Cancel;
		}
		protected internal void RaiseLayoutUpgrade(string previousVersion) {
			DevExpress.Utils.LayoutUpgradeEventHandler handler = (DevExpress.Utils.LayoutUpgradeEventHandler)this.Events[layoutUpgrade];
			DevExpress.Utils.LayoutAllowEventArgs ea = new DevExpress.Utils.LayoutAllowEventArgs(previousVersion);
			if(handler != null) handler(this, ea);
		}
		protected internal void RaiseCustomDrawBackground(GraphicsCache cache, Rectangle bounds) {
			CustomDrawBackgroundEventHandler handler = (CustomDrawBackgroundEventHandler)Events[customDrawBackground];
			CustomDrawBackgroundEventArgs ea = new CustomDrawBackgroundEventArgs(cache, bounds);
			if(handler != null)
				handler(this, ea);
		}
		protected internal Control RaiseQueryControl(BaseDocument document) {
			QueryControlEventHandler handler = (QueryControlEventHandler)Events[queryControl];
			QueryControlEventArgs ea = new QueryControlEventArgs(document);
			if(handler != null)
				handler(this, ea);
			return ea.Control;
		}
		protected internal bool RaiseControlReleasing(BaseDocument document, out bool disposeControl) {
			return RaiseControlReleasing(document, true, true, out disposeControl);
		}
		protected internal bool RaiseControlReleasing(BaseDocument document, bool keepControl, bool disposeControl, out bool disposeControlResult) {
			ControlReleasingEventHandler handler = (ControlReleasingEventHandler)Events[controlReleasing];
			ControlReleasingEventArgs e = new ControlReleasingEventArgs(document, keepControl, disposeControl);
			if(handler != null)
				handler(this, e);
			disposeControlResult = e.DisposeControl;
			return !e.Cancel;
		}
		protected void RaiseControlLoaded(BaseDocument document, Control control) {
			DeferredControlLoadEventHandler handler = (DeferredControlLoadEventHandler)Events[controlLoaded];
			if(handler != null)
				handler(this, new DeferredControlLoadEventArgs(document, control));
		}
		protected void RaiseControlShown(BaseDocument document, Control control) {
			DeferredControlLoadEventHandler handler = (DeferredControlLoadEventHandler)Events[controlShown];
			if(handler != null)
				handler(this, new DeferredControlLoadEventArgs(document, control));
		}
		protected void RaiseControlReleased(BaseDocument document, Control control) {
			DeferredControlLoadEventHandler handler = (DeferredControlLoadEventHandler)Events[controlReleased];
			if(handler != null)
				handler(this, new DeferredControlLoadEventArgs(document, control));
		}
		protected internal void RaiseDocumentSelectorShown() {
			EventHandler handler = (EventHandler)Events[documentSelectorShown];
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		protected internal void RaiseDocumentSelectorHidden() {
			EventHandler handler = (EventHandler)Events[documentSelectorHidden];
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		protected void RaiseGotFocus() {
			EventHandler handler = (EventHandler)Events[gotFocus];
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		protected void RaiseLostFocus() {
			EventHandler handler = (EventHandler)Events[lostFocus];
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		protected void RaiseDocumentAdded(BaseDocument document) {
			DocumentEventHandler handler = (DocumentEventHandler)Events[documentAdded];
			if(handler != null)
				handler(this, new DocumentEventArgs(document));
		}
		protected void RaiseDocumentRemoved(BaseDocument document) {
			DocumentEventHandler handler = (DocumentEventHandler)Events[documentRemoved];
			if(handler != null)
				handler(this, new DocumentEventArgs(document));
		}
		protected void RaiseDocumentActivated(BaseDocument document) {
			DocumentEventHandler handler = (DocumentEventHandler)Events[documentActivated];
			if(handler != null)
				handler(this, new DocumentEventArgs(document));
		}
		protected void RaiseDocumentDeactivated(BaseDocument document) {
			DocumentEventHandler handler = (DocumentEventHandler)Events[documentDeactivated];
			if(handler != null)
				handler(this, new DocumentEventArgs(document));
		}
		protected internal bool RaiseDocumentClosing(BaseDocument document) {
			DocumentCancelEventHandler handler = (DocumentCancelEventHandler)Events[documentClosing];
			DocumentCancelEventArgs e = new DocumentCancelEventArgs(document);
			if(handler != null)
				handler(this, e);
			return e.Cancel;
		}
		protected internal void RaiseDocumentClosed(BaseDocument document) {
			DocumentEventHandler handler = (DocumentEventHandler)Events[documentClosed];
			if(handler != null)
				handler(this, new DocumentEventArgs(document));
		}
		protected internal bool RaiseNextDocument(BaseDocument document, ref BaseDocument next, bool forward) {
			NextDocumentEventHandler handler = (NextDocumentEventHandler)Events[nextDocument];
			NextDocumentEventArgs e = new NextDocumentEventArgs(document, next, forward);
			if(handler != null) {
				handler(this, e);
				next = e.NextDocument;
			}
			return e.Handled;
		}
		protected void RaiseShowingDockGuides(BaseDocument document, DockGuidesConfiguration configuration, BaseViewHitInfo hitInfo) {
			ShowingDockGuidesEventHandler handler = (ShowingDockGuidesEventHandler)Events[showingDockGuides];
			if(handler != null)
				handler(this, new ShowingDockGuidesEventArgs(document, configuration, hitInfo));
		}
		protected internal bool RaisePopupMenuShowing(BaseViewControllerMenu menu, BaseViewHitInfo hitInfo) {
			PopupMenuShowingEventArgs e = new PopupMenuShowingEventArgs(menu, hitInfo);
			PopupMenuShowingEventHandler handler = (PopupMenuShowingEventHandler)Events[popupMenuShowing];
			if(handler != null)
				handler(this, e);
			return e.Cancel;
		}
		protected bool RaiseBeginFloating(BaseDocument document, FloatingReason reason) {
			bool cancelFloating = false;
			if(document.IsDockPanel) {
				if(Manager.DockManager != null) {
					Docking.DockPanelCancelEventArgs e = new Docking.DockPanelCancelEventArgs(document.GetDockPanel());
					Manager.DockManager.RaiseStartDocking(e);
					cancelFloating = e.Cancel;
				}
			}
			DocumentCancelEventHandler handler = (DocumentCancelEventHandler)Events[beginFloating];
			BeginFloatingEventArgs ea = new BeginFloatingEventArgs(document, cancelFloating, reason);
			if(handler != null)
				handler(this, ea);
			return !ea.Cancel;
		}
		protected void RaiseFloating(BaseDocument document) {
			DocumentEventHandler handler = (DocumentEventHandler)Events[floating];
			if(handler != null)
				handler(this, new DocumentEventArgs(document));
		}
		protected void RaiseEndFloating(BaseDocument document, EndFloatingReason reason) {
			DocumentEventHandler handler = (DocumentEventHandler)Events[endFloating];
			if(handler != null)
				handler(this, new EndFloatingEventArgs(document, reason));
		}
		protected bool RaiseBeginDocking(BaseDocument document) {
			DocumentCancelEventHandler handler = (DocumentCancelEventHandler)Events[beginDocking];
			DocumentCancelEventArgs ea = new DocumentCancelEventArgs(document);
			if(handler != null)
				handler(this, ea);
			return !ea.Cancel;
		}
		protected void RaiseEndDocking(BaseDocument document) {
			DocumentEventHandler handler = (DocumentEventHandler)Events[endDocking];
			if(handler != null)
				handler(this, new DocumentEventArgs(document));
		}
		protected bool RaiseBeginDocumentsHostDocking(BaseDocument document) {
			DocumentCancelEventHandler handler = (DocumentCancelEventHandler)Events[beginDocumentsHostDocking];
			DocumentCancelEventArgs ea = new DocumentCancelEventArgs(document);
			if(handler != null)
				handler(this, ea);
			return !ea.Cancel;
		}
		protected void RaiseEndDocumentsHostDocking(BaseDocument document) {
			DocumentEventHandler handler = (DocumentEventHandler)Events[endDocumentsHostDocking];
			if(handler != null)
				handler(this, new DocumentEventArgs(document));
		}
		protected internal void RaiseRegisterDocumentsHostWindow(IDocumentsHostWindow hostWindow) {
			DocumentsHostWindowEventHandler handler = (DocumentsHostWindowEventHandler)Events[registerDocumentsHostWindow];
			if(handler != null)
				handler(this, new DocumentsHostWindowEventArgs(hostWindow));
		}
		protected internal void RaiseUnregisterDocumentsHostWindow(IDocumentsHostWindow hostWindow) {
			DocumentsHostWindowEventHandler handler = (DocumentsHostWindowEventHandler)Events[unregisterDocumentsHostWindow];
			if(handler != null)
				handler(this, new DocumentsHostWindowEventArgs(hostWindow));
		}
		protected internal virtual IDocumentsHostWindow RaiseCustomDocumentsHostWindow(BaseDocument document) {
			CustomDocumentsHostWindowEventHandler handler = (CustomDocumentsHostWindowEventHandler)Events[customDocumentsHostWindow];
			DocumentsHostWindowConstructor defaultConstructor = Manager.IsMdiStrategyInUse ?
				new DocumentsHostWindowConstructor(() => new FloatMdiDocumentsHostWindow(Type, Manager)) :
				new DocumentsHostWindowConstructor(() => new FloatContainerControlDocumentsHostWindow(Type, Manager));
			CustomDocumentsHostWindowEventArgs args = new CustomDocumentsHostWindowEventArgs(document, defaultConstructor);
			if(handler != null) handler(this, args);
			DocumentsHostWindowConstructor constructor = args.Constructor ?? defaultConstructor;
			return !args.Cancel ? constructor() : null;
		}
		protected internal virtual bool RaiseEmptyDocumentsHostWindow(IDocumentsHostWindow hostWindow, bool keepOpen, bool documentDisposed) {
			var reason = documentDisposed ? EmptyDocumentsHostWindowReason.DocumentDisposed : EmptyDocumentsHostWindowReason.DocumentClosed;
			var e = new EmptyDocumentsHostWindowEventArgs(hostWindow, keepOpen, reason);
			EmptyDocumentsHostWindowEventHandler handler = (EmptyDocumentsHostWindowEventHandler)Events[emptyDocumentsHostWindow];
			if(handler != null)
				handler(this, e);
			return !e.KeepOpen;
		}
		protected internal virtual bool RaiseTabMouseActivating(BaseDocument document) { return false; }
		protected internal DocumentSelectorCustomSortItemsEventArgs RaiseDocumentSelectorCustomSortItems() {
			DocumentSelectorCustomSortItemsEventHandler handler = (DocumentSelectorCustomSortItemsEventHandler)Events[documentSelectorCustomSortItems];
			DocumentSelectorCustomSortItemsEventArgs ea = new DocumentSelectorCustomSortItemsEventArgs();
			if(handler != null)
				handler(this, ea);
			return ea;
		}
		#endregion Events
		#region DockOperation
		int dockOperationCounter = 0;
		protected internal IDockOperation BeginDockOperation(BaseDocument document) {
			return new DockOperation(this, document);
		}
		class DockOperation : IDockOperation {
			readonly BaseView View;
			readonly BaseDocument Document;
			bool canceledCore;
			public DockOperation(BaseView view, BaseDocument document) {
				View = view;
				Document = document;
				if(0 == View.dockOperationCounter++) {
					View.LockDeactivateApp();
					canceledCore = !View.OnBeginDocking(Document);
				}
			}
			public void Dispose() {
				if(--View.dockOperationCounter == 0) {
					if(!Canceled) {
						if(Document.IsDockPanel) {
							var panel = Document.GetDockPanel();
							if(panel != null)
								panel.dockedAsTabbedDocumentCore = true;
						}
						View.OnEndDocking(Document);
					}
					View.UnLockDeactivateApp();
				}
			}
			public bool Canceled {
				get { return canceledCore; }
			}
		}
		#endregion DockOperation
		#region IUIElement
		IUIElement IUIElement.Scope { get { return Manager; } }
		UIChildren uiChildren = new UIChildren();
		UIChildren IUIElement.Children {
			get { return uiChildren; }
		}
		#endregion IUIElement
		protected internal void SetCursor(Cursor cursor) {
			Control control = (Manager != null) ? Manager.GetOwnerControl() : null;
			if(control != null)
				control.Cursor = cursor;
		}
		internal Rectangle GetFloatingBounds(Rectangle bounds) {
			if(!IsDisposing && IsLoaded)
				return ViewInfo.GetFloatingBounds(bounds);
			return bounds;
		}
		internal Point GetFloatLocation(BaseDocument document) {
			if(!IsDisposing && IsLoaded) {
				Point viewPoint = ViewInfo.GetFloatLocation(document);
				return Manager.ClientToScreen(viewPoint);
			}
			return document.Form.Location;
		}
		DevExpress.Utils.DefaultBoolean showDockGuidesOnPressingShiftCore = DevExpress.Utils.DefaultBoolean.Default;
		internal DevExpress.Utils.DefaultBoolean ShowDockGuidesOnPressingShiftBase {
			get { return showDockGuidesOnPressingShiftCore; }
			set { showDockGuidesOnPressingShiftCore = value; }
		}
		internal bool CanShowDockGuidesOnPressingShift() {
			if(ShowDockGuidesOnPressingShiftBase == DevExpress.Utils.DefaultBoolean.True) 
				return Control.ModifierKeys == Keys.Shift;
			return true;
		}
		protected virtual bool HideDockGuidesInDesignMode() {
			return Site != null && Site.DesignMode;
		}
		internal void OnShowingDockGuides(DockGuidesConfiguration configuration, BaseDocument document, Point point) {
			if(document == null || !document.Properties.CanDock || !CanShowDockGuidesOnPressingShift()) {
				configuration.DisableAllGuides();
				if(document.IsDockPanel) {
					Docking.DockPanel panel = document.GetDockPanel();
					if(panel != null && !CanDockAsTabbedDocument(panel))
						configuration.DisableTabHint();
				}
				return;
			}
			if(HideDockGuidesInDesignMode()) {
				if(document.IsDockPanel || Manager.IsMdiStrategyInUse) {
					configuration.Disable(DockHint.Center);
					configuration.DisableCenterHints();
					if(document.IsDockPanel)
						configuration.DisableTabHint();
				}
			}
			if(IsEmpty)
				configuration.DisableCenterHints();
			if(!document.SupportSideDock) {
				configuration.DisableSideGuides();
				configuration.DisableSideHints();
			}
			else {
				if(document.IsDockPanel) {
					Docking.DockPanel panel = document.GetDockPanel();
					if(panel != null) {
						if(!panel.Options.AllowDockLeft) {
							configuration.Disable(DockHint.CenterSideLeft);
							configuration.Disable(DockGuide.Left);
						}
						if(!panel.Options.AllowDockTop) {
							configuration.Disable(DockHint.CenterSideTop);
							configuration.Disable(DockGuide.Top);
						}
						if(!panel.Options.AllowDockRight) {
							configuration.Disable(DockHint.CenterSideRight);
							configuration.Disable(DockGuide.Right);
						}
						if(!panel.Options.AllowDockBottom) {
							configuration.Disable(DockHint.CenterSideBottom);
							configuration.Disable(DockGuide.Bottom);
						}
						if(!CanDockAsTabbedDocument(panel)) {
							configuration.Disable(DockHint.Center);
							configuration.Disable(DockGuide.Center);
							configuration.Disable(DockGuide.CenterDock);
							configuration.DisableCenterHints();
							configuration.DisableTabHint();
						}
					}
				}
			}
			BaseViewHitInfo hitInfo = Manager.CalcHitInfo(point);
			OnShowingDockGuidesCore(configuration, document, hitInfo);
			if(!configuration.IsEmpty || configuration.IsTabHintEnabled)
				RaiseShowingDockGuides(document, configuration, hitInfo);
		}
		internal bool CanDockAsTabbedDocument(Docking.DockPanel panel) {
			bool allowDockAsTabbedDocument = panel.Options.AllowDockAsTabbedDocument;
			if(allowDockAsTabbedDocument && panel.HasChildren) {
				foreach(Docking.Helpers.DockLayout childLayout in panel.DockLayout) {
					if(childLayout != null) {
						allowDockAsTabbedDocument &= childLayout.Panel.Options.AllowDockAsTabbedDocument;
					}
				}
			}
			return allowDockAsTabbedDocument;
		}
		protected virtual void OnShowingDockGuidesCore(DockGuidesConfiguration configuration, BaseDocument document, BaseViewHitInfo hitInfo) { }
		#region XtraSerialization
		public void SaveLayoutToXml(string path) {
			SaveLayoutCore(new XmlXtraSerializer(), path);
		}
		public void RestoreLayoutFromXml(string path) {
			RestoreLayoutCore(new XmlXtraSerializer(), path);
		}
		public void SaveLayoutToXml(System.IO.Stream stream) {
			SaveLayoutCore(new XmlXtraSerializer(), stream);
		}
		public void RestoreLayoutFromXml(System.IO.Stream stream) {
			RestoreLayoutCore(new XmlXtraSerializer(), stream);
		}
		public void SaveLayoutToStream(System.IO.Stream stream) {
			SaveLayoutCore(new BinaryXtraSerializer(), stream);
		}
		public void RestoreLayoutFromStream(System.IO.Stream stream) {
			RestoreLayoutCore(new BinaryXtraSerializer(), stream);
		}
		public void SaveLayoutToStream(System.IO.Stream stream, bool binaryStream) {
			XtraSerializer serializer = binaryStream ?
				(XtraSerializer)new BinaryXtraSerializer() : (XtraSerializer)new XmlXtraSerializer();
			SaveLayoutCore(serializer, stream);
		}
		public void RestoreLayoutFromStream(System.IO.Stream stream, bool binaryStream) {
			XtraSerializer serializer = binaryStream ?
				(XtraSerializer)new BinaryXtraSerializer() : (XtraSerializer)new XmlXtraSerializer();
			RestoreLayoutCore(serializer, stream);
		}
		public void SaveLayoutToRegistry(string path) {
			SaveLayoutCore(new RegistryXtraSerializer(), path);
		}
		public void RestoreLayoutFromRegistry(string path) {
			RestoreLayoutCore(new RegistryXtraSerializer(), path);
		}
		BaseDocument restoreActiveDocument = null;
		protected void SetRestoreActiveDocument(BaseDocument document, bool force) {
			if(restoreActiveDocument == null || force)
				restoreActiveDocument = document;
		}
		protected virtual bool SaveLayoutCore(XtraSerializer serializer, object path) {
			System.IO.Stream stream = path as System.IO.Stream;
			if(stream != null)
				return serializer.SerializeObject(this, stream, GetType().Name, OptionsLayout);
			else
				return serializer.SerializeObject(this, path.ToString(), GetType().Name, OptionsLayout);
		}
		protected virtual void RestoreLayoutCore(XtraSerializer serializer, object path) {
			System.IO.Stream stream = path as System.IO.Stream;
			if(stream != null)
				serializer.DeserializeObject(this, stream, GetType().Name, OptionsLayout);
			else
				serializer.DeserializeObject(this, path.ToString(), GetType().Name, OptionsLayout);
		}
		int serializing, deserializing = 0;
		IList<SerializableObjectInfo> itemsCore;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true, true, false, 1)]
		public IList<SerializableObjectInfo> Items {
			get { return itemsCore; }
		}
		internal bool IsSerializing {
			get { return serializing > 0; }
		}
		internal bool IsDeserializing {
			get { return deserializing > 0; }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BaseViewOptionsLayout")]
#endif
		[Category("Data"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public OptionsLayout OptionsLayout {
			get { return optionsLayoutCore; }
		}
		protected virtual OptionsLayout CreateOptionsLayout() {
			return new OptionsLayout();
		}
		class SerializingBatchUpdate : IDisposable {
			IBatchUpdate managerUpdate;
			IBatchUpdate viewUpdate;
			public SerializingBatchUpdate(BaseView view) {
				managerUpdate = BatchUpdate.Enter(view.Manager, true);
				viewUpdate = BatchUpdate.Enter(view, true);
			}
			void IDisposable.Dispose() {
				Ref.Dispose(ref viewUpdate);
				Ref.Dispose(ref managerUpdate);
			}
		}
		IDisposable serializingBatch;
		void IXtraSerializable.OnStartSerializing() {
			serializingBatch = new SerializingBatchUpdate(this);
			serializing++;
			BeginSaveLayout();
		}
		void IXtraSerializable.OnEndSerializing() {
			EndSaveLayout();
			serializing--;
			Ref.Dispose(ref serializingBatch);
		}
		class DeserializingBatchUpdate : IDisposable {
			IBatchUpdate managerUpdate;
			IDisposable painting;
			Action beforePaintingComplete;
			Action beforeUpdateComplete;
			public DeserializingBatchUpdate(BaseView view, Action beforePaintingComplete, Action beforeUpdateComplete) {
				this.beforePaintingComplete = beforePaintingComplete;
				this.beforeUpdateComplete = beforeUpdateComplete;
				managerUpdate = BatchUpdate.Enter(view.Manager);
				painting = view.LockPainting();
			}
			void IDisposable.Dispose() {
				if(beforePaintingComplete != null)
					beforePaintingComplete();
				Ref.Dispose(ref painting);
				if(beforeUpdateComplete != null)
					beforeUpdateComplete();
				Ref.Dispose(ref managerUpdate);
			}
		}
		IDisposable deserializingBatch;
		void IXtraSerializable.OnStartDeserializing(DevExpress.Utils.LayoutAllowEventArgs e) {
			deserializingBatch = new DeserializingBatchUpdate(this, OnBeforeDeserializingComplete, OnDeserializingComplete);
			deserializing++;
			if(Manager != null) Manager.SetPatchActiveChildrenRequeryFlag();
			this.restoreActiveDocument = null;
			BeginRestoreLayout();
		}
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) {
			EndRestoreLayout();
			if(restoredVersion != OptionsLayout.LayoutVersion)
				RaiseLayoutUpgrade(restoredVersion);
			deserializing--;
			Ref.Dispose(ref deserializingBatch);
		}
		void OnBeforeDeserializingComplete() {
			if(ViewInfo != null)
				ViewInfo.SetDirty();
			SetLayoutModified();
			RequestInvokePatchActiveChild();
		}
		void OnDeserializingComplete() {
			if(restoreActiveDocument != null) {
				restoreActiveDocument.EnsureDeferredLoadControl(this);
				Manager.Activate(restoreActiveDocument);
				DocumentsHostContext.CheckHostFormActive(Manager);
				restoreActiveDocument = null;
			}
			if(Manager != null) Manager.PatchActiveChildrenRequery();
		}
		string IXtraSerializableLayout.LayoutVersion {
			get { return OptionsLayout.LayoutVersion; }
		}
		protected virtual void BeginSaveLayout() {
			PrepareSerializableObjectInfos();
		}
		protected virtual void EndSaveLayout() {
			ClearSerializableObjectInfos();
		}
		protected virtual void BeginRestoreLayout() {
			PrepareSerializableObjectInfos();
		}
		protected virtual void EndRestoreLayout() {
			ClearSerializableObjectInfos();
		}
		IDictionary<object, SerializableObjectInfo> infosCore;
		protected IDictionary<object, SerializableObjectInfo> Infos {
			get { return infosCore; }
		}
		protected virtual void PrepareSerializableObjectInfos() {
			infosCore = new Dictionary<object, SerializableObjectInfo>();
			itemsCore = new List<SerializableObjectInfo>();
			PrepareSerializableDocumentInfos();
		}
		protected virtual void ClearSerializableObjectInfos() {
			itemsCore = null;
			Ref.Clear(ref infosCore);
		}
		protected virtual void PrepareSerializableDocumentInfos() {
			BaseDocument[] documents = Documents.ToArray();
			for(int i = 0; i < documents.Length; i++) {
				BaseSerializableDocumentInfo info = CreateSerializableDocumentInfo(documents[i]);
				PrepareSerializableDocumentInfo(documents[i], info, i);
			}
			BaseDocument[] floatDocuments = FloatDocuments.ToArray();
			for(int i = 0; i < floatDocuments.Length; i++) {
				BaseSerializableDocumentInfo info = CreateSerializableDocumentInfo(floatDocuments[i]);
				PrepareSerializableDocumentInfo(floatDocuments[i], info, i);
			}
		}
		protected void PrepareSerializableDocumentInfo(BaseDocument document, BaseSerializableDocumentInfo info, int index) {
			string name = document.GetName();
			if(string.IsNullOrEmpty(name))
				info.SetNameByIndex(index);
			else info.SetName(name);
			RegisterSerializableObjectinfo(document, info);
		}
		protected void RegisterSerializableObjectinfo(object obj, SerializableObjectInfo info) {
			if(!Infos.ContainsKey(obj)) {
				Infos.Add(obj, info);
				Items.Add(info);
			}
		}
		protected void AddSerializableObjectinfo(SerializableObjectInfo info) {
			Items.Add(info);
		}
		protected abstract BaseSerializableDocumentInfo CreateSerializableDocumentInfo(BaseDocument document);
		protected virtual object XtraFindItemsItem(XtraItemEventArgs e) {
			XtraPropertyInfo nameInfo = e.Item.ChildProperties["Name"];
			string name = nameInfo.Value as string;
			foreach(SerializableObjectInfo objectInfo in Items) {
				if(objectInfo.Name == name)
					return objectInfo;
			}
			return null;
		}
		protected virtual object XtraCreateItemsItem(XtraItemEventArgs e) {
			XtraPropertyInfo typeNameInfo = e.Item.ChildProperties["TypeName"];
			SerializableObjectInfo info = CreateSerializableObjectInfo((string)typeNameInfo.Value);
			if(info != null)
				info.SetName((string)e.Item.ChildProperties["Name"].Value);
			return info;
		}
		protected virtual SerializableObjectInfo CreateSerializableObjectInfo(string typeName) {
			return null;
		}
		#region SerializableObjectInfo
		public abstract class SerializableObjectInfo {
			object sourceCore;
			public SerializableObjectInfo(object source) {
				sourceCore = source;
			}
			public object Source {
				get { return sourceCore; }
			}
			string nameCore;
			internal void SetName(string name) {
				nameCore = name;
			}
			internal void SetNameByIndex(int index) {
				nameCore = string.Format("{0}{1}", TypeName, index);
			}
			protected abstract string GetTypeNameCore();
			[XtraSerializableProperty]
			public string Name {
				get { return nameCore; }
			}
			[XtraSerializableProperty]
			public string TypeName {
				get { return GetTypeNameCore(); }
			}
		}
		protected abstract class BaseSerializableDocumentInfo : SerializableObjectInfo {
			IBaseDocumentDefaultProperties propertiesCore;
			public BaseSerializableDocumentInfo(BaseDocument document)
				: base(document) {
				propertiesCore = document.Properties;
				isFloatingCore = document.IsFloating;
				isActiveCore = document.IsActive;
				if(document.IsControlLoaded) {
					locationCore = document.FloatLocation.HasValue ? document.FloatLocation.Value :
						(document.Form.WindowState == FormWindowState.Normal ? document.Form.Location : document.Form.RestoreBounds.Location);
					sizeCore = document.FloatSize.HasValue ? document.FloatSize.Value :
						(document.Form.WindowState == FormWindowState.Normal ? document.Form.Size : document.Form.RestoreBounds.Size);
				}
			}
			[XtraSerializableProperty(XtraSerializationVisibility.Content)]
			public IBaseDocumentDefaultProperties Properties {
				get { return propertiesCore; }
			}
			protected override string GetTypeNameCore() {
				return "Document";
			}
			bool isFloatingCore;
			[XtraSerializableProperty]
			public bool IsFloating {
				get { return isFloatingCore; }
				set { isFloatingCore = value; }
			}
			Point locationCore;
			[XtraSerializableProperty]
			public Point Location {
				get { return locationCore; }
				set { locationCore = value; }
			}
			Size sizeCore;
			[XtraSerializableProperty]
			public Size Size {
				get { return sizeCore; }
				set { sizeCore = value; }
			}
			bool isActiveCore;
			[XtraSerializableProperty]
			public bool IsActive {
				get { return isActiveCore; }
				set { isActiveCore = value; }
			}
		}
		#endregion SerializableObjectInfo
		#endregion XtraSerialization
		#region SysCommand handler
		protected internal virtual bool CanProcessSysCommand(IntPtr hWnd, int cmd) {
			return Documents.GetDocument(hWnd) != null;
		}
		protected internal virtual bool ProcessSysCommand(IntPtr hWnd, int cmd) {
			if(IsDisposing) return false;
			switch(cmd) {
				case DevExpress.Utils.Drawing.Helpers.NativeMethods.SC.SC_CLOSE:
					return OnSCClose(hWnd);
				case DevExpress.Utils.Drawing.Helpers.NativeMethods.SC.SC_MAXIMIZE:
					return OnSCMaximize(hWnd);
				case DevExpress.Utils.Drawing.Helpers.NativeMethods.SC.SC_RESTORE:
					return OnSCRestore(hWnd);
			}
			return false;
		}
		protected internal virtual bool OnSCClose(IntPtr hWnd) {
			return Documents.OnSCClose(hWnd) ||
				FloatDocuments.OnSCClose(hWnd);
		}
		protected internal virtual bool OnSCMaximize(IntPtr hWnd) {
			throw new NotSupportedException();
		}
		protected internal virtual bool OnSCRestore(IntPtr hWnd) {
			throw new NotSupportedException();
		}
		#endregion SysCommand handler
		protected internal virtual DevExpress.Utils.ToolTipControlInfo GetToolTipControlInfo(BaseViewHitInfo hitInfo) {
			return null;
		}
		protected internal abstract void RegisterListeners(BaseUIView uiView);
		protected internal abstract Type GetUIElementKey();
		protected internal virtual bool CanProcessHooks() { return !IsDisposing; }
		public override string ToString() {
			return Site != null ? Site.Name : null;
		}
		#region IDesignTimeSupport Members
		int designerLoaded;
		bool IDesignTimeSupport.IsSerializing { get; set; }
		bool IDesignTimeSupport.IsLoaded {
			get { return designerLoaded > 0; }
		}
		void IDesignTimeSupport.Load() {
			if(0 == designerLoaded++) {
				if(ViewInfo != null)
					UpdateStyleCore();
				else
					InitStyleCore();
				if(!IsLoaded)
					RegisterInfos();
			}
		}
		void IDesignTimeSupport.Unload() {
			if(--designerLoaded == 0) {
				if(!IsLoaded)
					UnregisterInfos();
			}
		}
		protected internal bool IsDesignMode() {
			return IsDesignMode(this) || ((IDesignTimeSupport)this).IsLoaded ||
				DevExpress.Utils.Design.DesignTimeTools.IsDesignMode;
		}
		static bool IsDesignMode(IComponent component) {
			return (component != null) && (component.Site != null) && component.Site.DesignMode;
		}
		#endregion
		AsyncAdornerElementInfo waitScreenInfo;
		DevExpress.Utils.Animation.IAsyncAdorner waitScreenAdorner;
		[Browsable(false)]
		public bool IsLoadingAnimationInProgress {
			get {
				if(waitScreenInfo != null && waitScreenInfo.InfoArgs != null) {
					var args = waitScreenInfo.InfoArgs as WaitScreenAdornerInfoArgs;
					return (args.LoadingAnimator != null) && args.LoadingAnimator.AnimationInProgress;
				}
				return false;
			}
		}
		protected internal virtual bool ShowDocumentSelectorMenu() {
			return false;
		}
		protected internal DevExpress.Utils.Animation.IAsyncAdorner ShowLoadingIndicator(BaseDocument document) {
			if(!CanUseLoadingIndicator()) return null;
			bool hideIndicator = InNestedDocumentManager() || InInvisibleDueShowingStrategyForm();
			if(!RaiseLoadingIndicatorShowing(document, !hideIndicator)) return null;
			WaitScreenAdornerInfoArgs args = WaitScreenAdornerInfoArgs.EnsureInfoArgs(ref waitScreenInfo, this, document);
			return AsyncAdornerBootStrapper.Show(Manager.GetOwnerControlHandle(), waitScreenInfo);
		}
		protected virtual bool RaiseLoadingIndicatorShowing(BaseDocument document, bool show) {
			LoadingIndicatorShowingEventArgs e = new LoadingIndicatorShowingEventArgs(document, show);
			LoadingIndicatorShowingEventHandler handler = Events[loadingIndicatorShowing] as LoadingIndicatorShowingEventHandler;
			if(handler != null) handler(this, e);
			return !e.Cancel;
		}
		protected bool InInvisibleDueShowingStrategyForm() {
			var container = Manager.GetContainer();
			if(container != null) {
				var parentForm = container.FindForm() as XtraEditors.XtraForm;
				return XtraEditors.XtraForm.CheckInvisibleDueShowingStrategy(parentForm);
			}
			return false;
		}
		protected bool InNestedDocumentManager() {
			var container = Manager.GetContainer();
			if(container != null) {
				Control parentContainer = (Control)DocumentContainer.FromControl(container);
				if(parentContainer == null) {
					Form parentForm = container.FindForm();
					if(parentForm != null && parentForm.IsMdiChild)
						parentContainer = parentForm;
				}
				return (parentContainer != null) && (parentContainer.Left < 0 || parentContainer.Top < 0);
			}
			return false;
		}
		protected internal virtual Rectangle GetBounds(BaseDocument document) {
			if(!IsDisposing && IsLoaded)
				return ViewInfo.GetBounds(document);
			return Bounds;
		}
		protected virtual void DestroyAsyncAnimations() {
			if(waitScreenInfo != null && waitScreenInfo.InfoArgs != null)
				waitScreenInfo.InfoArgs.Destroy();
		}
		public void ReleaseDeferredLoadControl(BaseDocument document) {
			ReleaseDeferredLoadControl(document, true, true);
		}
		public void ReleaseDeferredLoadControl(BaseDocument document, bool keepControl) {
			ReleaseDeferredLoadControl(document, keepControl, true);
		}
		public void ReleaseDeferredLoadControl(BaseDocument document, bool keepControl, bool disposeControl) {
			using(LockPainting()) {
				document.ReleaseDeferredLoadControl(this, keepControl, disposeControl);
				RequestInvokePatchActiveChild();
			}
		}
		public void ReleaseDeferredLoadControls() {
			ReleaseDeferredLoadControls(true, true);
		}
		public void ReleaseDeferredLoadControls(bool keepControls) {
			ReleaseDeferredLoadControls(keepControls, true);
		}
		public void ReleaseDeferredLoadControls(bool keepControls, bool disposeControls) {
			using(BatchUpdate.Enter(Manager, true)) {
				using(LockPainting()) {
					foreach(BaseDocument document in Documents)
						document.ReleaseDeferredLoadControl(this, keepControls, disposeControls);
					RequestInvokePatchActiveChild();
				}
			}
		}
		protected override bool IsLayoutChangeRestricted {
			get {
				return base.IsLayoutChangeRestricted || (Manager != null && (Manager as IProcessRunningListener).IsRunning && DesignMode);
			}
		}
		internal static void PatchChild(Control ctrl, Rectangle client, Rectangle view) {
			PatchMaximized(ctrl);
			Rectangle ctrlBounds = ctrl.Bounds;
			if(!view.IntersectsWith(ctrlBounds))
				DevExpress.Utils.Mdi.MdiChildHelper.ResizeControlOutOfTheView(ctrl, client, ctrlBounds);
			else ctrl.Bounds = client;
			ctrl.Update();
		}
		internal static void PatchMaximized(Control child) {
			Form form = child as Form;
			if(form != null && form.WindowState != FormWindowState.Normal)
				form.WindowState = FormWindowState.Normal;
		}
		protected internal virtual bool AllowShowThumbnailsInTaskBar { get { return false; } }
		#region ILogicalOwner Members
		IEnumerable<Component> DevExpress.Utils.ILogicalOwner.GetLogicalChildren() {
			DevExpress.Utils.DefaultBoolean value = UseLoadingIndicator;
			UseLoadingIndicator = DevExpress.Utils.DefaultBoolean.False;
			try {
				foreach(var item in Documents) {
					if(!item.IsControlLoaded)
						item.EnsureDeferredLoadControl(this);
					yield return item;
				}
				foreach(var item in FloatDocuments) {
					if(!item.IsControlLoaded)
						item.EnsureDeferredLoadControl(this);
					yield return item;
				}
			}
			finally {
				UseLoadingIndicator = value;
			}
		}
		#endregion
		#region IXtraSerializableChildren Members
		string DevExpress.Utils.IXtraSerializableChildren.Name {
			get { return Type.ToString() + Manager.ViewCollection.IndexOf(this).ToString(); }
		}
		#endregion
		#region ICaptionAppearanceProvider Members
		DevExpress.Utils.AppearanceObject IDocumentCaptionAppearanceProvider.ActiveCaptionAppearance {
			get { return GetActiveDocumentCaptionAppearance(); }
		}
		DevExpress.Utils.AppearanceObject IDocumentCaptionAppearanceProvider.CaptionAppearance {
			get { return GetDocumentCaptionAppearance(); }
		}
		bool IDocumentCaptionAppearanceProvider.AllowCaptionColorBlending {
			get { return CanBlendCaptionColor(); }
		}
		protected virtual bool CanBlendCaptionColor() { return true; }
		#endregion
		protected virtual DevExpress.Utils.AppearanceObject GetDocumentCaptionAppearance() {
			return new DevExpress.Utils.AppearanceObject();
		}
		protected virtual DevExpress.Utils.AppearanceObject GetActiveDocumentCaptionAppearance() {
			return new DevExpress.Utils.AppearanceObject();
		}
		#region MVVM
		DevExpress.Utils.MVVM.Services.IDocumentAdapter DevExpress.Utils.MVVM.Services.IDocumentAdapterFactory.Create() {
			return new MVVM.Services.BaseDocumentAdapter(this);
		}
		DevExpress.Utils.MVVM.Services.IDocumentAdapter DevExpress.Utils.MVVM.Services.IWindowedDocumentAdapterFactory.Create() {
			return new MVVM.Services.BaseDocumentAdapter(this, true);
		}
		#endregion MVVM
	}
	[Editor("DevExpress.XtraBars.Design.DocumentManagerViewCollectionEditor, " + AssemblyInfo.SRAssemblyBarsDesign, typeof(System.Drawing.Design.UITypeEditor))]
	public class BaseViewCollection : BaseMutableListEx<BaseView> {
		DocumentManager ownerCore;
		public DocumentManager Owner {
			get { return ownerCore; }
		}
		public BaseViewCollection(DocumentManager manager) {
			ownerCore = manager;
		}
		protected override void OnDispose() {
			ownerCore = null;
			base.OnDispose();
		}
		protected override void OnElementAdded(BaseView element) {
			element.SetManager(Owner);
			base.OnElementAdded(element);
		}
		protected override void OnElementRemoved(BaseView element) {
			element.SetManager(null);
			base.OnElementRemoved(element);
		}
	}
	public enum PropertiesRestoreMode {
		Default, All, UI, None
	}
	public class OptionsLayout : DevExpress.Utils.OptionsLayoutBase {
		public const int UIProperty = 1;
		PropertiesRestoreMode propertiesRestoreModeCore;
		[DefaultValue(PropertiesRestoreMode.Default), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable]
		public PropertiesRestoreMode PropertiesRestoreMode {
			get { return propertiesRestoreModeCore; }
			set {
				if(PropertiesRestoreMode == value) return;
				PropertiesRestoreMode prevValue = PropertiesRestoreMode;
				propertiesRestoreModeCore = value;
				OnChanged(new DevExpress.Utils.Controls.BaseOptionChangedEventArgs("PropertiesRestoreMode", prevValue, value));
			}
		}
		public override void Assign(DevExpress.Utils.Controls.BaseOptions source) {
			BeginUpdate();
			try {
				base.Assign(source);
				OptionsLayout options = source as OptionsLayout;
				if(options == null) return;
				this.propertiesRestoreModeCore = options.PropertiesRestoreMode;
			}
			finally { EndUpdate(); }
		}
		protected internal bool RestoreAllProperties {
			get { return PropertiesRestoreMode == PropertiesRestoreMode.Default || PropertiesRestoreMode == PropertiesRestoreMode.All; }
		}
		protected internal bool RestoreUIProperties {
			get { return PropertiesRestoreMode == PropertiesRestoreMode.UI || PropertiesRestoreMode == PropertiesRestoreMode.All; }
		}
		protected internal bool CanRestoreProperty(string propertyName, int id) {
			if(RestoreAllProperties)
				return true;
			if(id == Views.OptionsLayout.UIProperty)
				return RestoreUIProperties;
			return PropertiesRestoreMode != Views.PropertiesRestoreMode.None;
		}
		protected internal static bool IsUIProperty(string propertyName, PropertyDescriptorCollection props) {
			XtraSerializablePropertyId xtraId = props[propertyName].Attributes[
				typeof(XtraSerializablePropertyId)] as XtraSerializablePropertyId;
			return (xtraId != null) && xtraId.Id == Views.OptionsLayout.UIProperty;
		}
	}
}
