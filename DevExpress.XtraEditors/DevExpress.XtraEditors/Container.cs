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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Win;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Mask;
using System.Collections.Generic;
using DevExpress.Skins;
namespace DevExpress.XtraEditors.Container {
	[ToolboxItem(false)]
	public abstract class EditorContainer : Control, ISupportInitialize, IEditorContainer, IMouseWheelSupport {
		private static readonly object editorKeyDown = new object();
		private static readonly object editorKeyPress = new object();
		private static readonly object editorKeyUp = new object();
		protected int fInitializing;
		bool _firstInit;
		IDXMenuManager menuManager = null;
		bool loadFired = false;
		ContainerHelper editorHelper;
		ToolTipController toolTipController;
		bool prevTabStop, firstPaint = true;
		int lockTabStop = 0;
		public EditorContainer() {
			this.fInitializing = 0;
			this._firstInit = true;
			this.editorHelper = CreateHelper();
			this.toolTipController = null;
			ToolTipController.DefaultController.AddClientControl(this);
		}
		bool isRightToLeft = false;
		protected void CheckRightToLeft() {
			bool newRightToLeft = WindowsFormsSettings.GetIsRightToLeft(this);
			if(newRightToLeft == this.isRightToLeft) return;
			this.isRightToLeft = newRightToLeft;
			OnRightToLeftChanged();
		}
		protected sealed override void OnRightToLeftChanged(EventArgs e) {
			base.OnRightToLeftChanged(e);
			CheckRightToLeft();
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			CheckRightToLeft();
		}
		protected virtual bool IsRightToLeft { get { return isRightToLeft; } }
		protected virtual void OnRightToLeftChanged() {
		}
		SizeF scaleFactor = new SizeF(1f, 1f);
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SizeF ScaleFactor { get { return scaleFactor; } }
		protected override void ScaleControl(SizeF factor, BoundsSpecified specified) {
			scaleFactor = factor;
			base.ScaleControl(factor, specified);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("EditorContainerMenuManager"),
#endif
 DefaultValue(null), DXCategory(CategoryName.BarManager)]
		public IDXMenuManager MenuManager {
			get { return menuManager; }
			set { menuManager = value; }
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("EditorContainerToolTipController"),
#endif
 DefaultValue(null)]
		public ToolTipController ToolTipController {
			get { return toolTipController; }
			set {
				if(value == ToolTipController.DefaultController) value = null;
				if(ToolTipController == value) return;
				if(ToolTipController != null) {
					ToolTipController.RemoveClientControl(this);
					if(ToolTipController != ToolTipController.DefaultController)
						ToolTipController.Disposed -= new EventHandler(OnToolTipControllerDisposed);
				}
				toolTipController = value;
				if(ToolTipController != null) { 
					ToolTipController.AddClientControl(this);
					if(ToolTipController != ToolTipController.DefaultController)
						ToolTipController.Disposed += new EventHandler(OnToolTipControllerDisposed);
					ToolTipController.DefaultController.RemoveClientControl(this);
				} else 
					ToolTipController.DefaultController.AddClientControl(this);
			}
		}
		public ToolTipController GetToolTipController() {
			if(ToolTipController == null) return ToolTipController.DefaultController;
			return ToolTipController;
		}
		protected void OnToolTipControllerDisposed(object sender, EventArgs e) {
			ToolTipController = null;
		}
		Control GetActiveControl() {
			if(!IsHandleCreated) return null;
			Form form = FindForm();
			return form != null ? form.ActiveControl : null;
		}
		protected sealed override void OnMouseWheel(MouseEventArgs ev) {
			if(XtraForm.ProcessSmartMouseWheel(this, ev)) return;
			OnMouseWheelCore(ev);
		}
		void IMouseWheelSupport.OnMouseWheel(MouseEventArgs e) {
			OnMouseWheelCore(e);
		}
		protected virtual void OnMouseWheelCore(MouseEventArgs ev) {
			if(ControlHelper.IsHMouseWheel(ev))
				OnMouseHWheel(ev);
			else 
			base.OnMouseWheel(ev);
		}
		protected virtual void OnMouseHWheel(MouseEventArgs ev) {
		}
		WeakReference prevFocusedControlRef;
		protected override void OnLeave(EventArgs e) {
			DestroyClearValidationTimer();
			this.prevFocusedControlRef = new WeakReference(GetActiveControl());
			base.OnLeave(e);
			if(!CausesValidation) OnContainerLeave(e);
		}
		protected override void OnValidated(EventArgs e) {
			base.OnValidated(e);
			OnContainerLeave(e);
		}
		protected override void OnEnter(EventArgs e) {
			base.OnEnter(e);
			OnContainerEnter(e);
		}
		Timer clearValidationTimer = null;
		void DestroyClearValidationTimer() {
			if(clearValidationTimer != null) clearValidationTimer.Dispose();
			this.clearValidationTimer = null;
		}
		protected virtual void OnContainerEnter(EventArgs e) {
			if(this.prevFocusedControlRef != null && prevFocusedControlRef.Target != null) {
				Control control = this.prevFocusedControlRef.Target as Control;
				DestroyClearValidationTimer();
				this.clearValidationTimer = new Timer();
				this.clearValidationTimer.Interval = 500;
				this.clearValidationTimer.Start();
				this.clearValidationTimer.Tick += new EventHandler(delegate(object sender, EventArgs ee) {
					if(this.IsHandleCreated && this.ContainsFocus && Control.MouseButtons != System.Windows.Forms.MouseButtons.None) {
						return;
					}
					((Timer)sender).Dispose();
					this.clearValidationTimer = null;
					DevExpress.Utils.Controls.ControlBase.ResetValidationCanceled(control);
				});
				this.prevFocusedControlRef = null;
			}
		}
		protected virtual void OnContainerLeave(EventArgs e) { }
		protected ContainerHelper EditorHelper { get { return editorHelper; } }
		protected abstract EditorContainerHelper CreateHelper();
		bool destroying = false;
		protected override void Dispose(bool disposing) {
			this.destroying = true;
			if(disposing) {
				DestroyClearValidationTimer();
				if(EditorHelper != null) {
					EditorHelper.Dispose();
				}
				ToolTipController = null;
				ToolTipController.DefaultController.RemoveClientControl(this);
				RemoveHookOnLoaded();
				this.prevFocusedControlRef = null;
			}
			base.Dispose(disposing);
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
#if DXWhidbey
, DevExpress.Utils.Design.InheritableCollection
#endif
]
		public virtual RepositoryItemCollection RepositoryItems { get { return EditorHelper.RepositoryItems; } }
		[DXCategory(CategoryName.Data), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("EditorContainerExternalRepository"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), DefaultValue(null)]
		public PersistentRepository ExternalRepository {
			get { return EditorHelper.ExternalRepository; }
			set { EditorHelper.ExternalRepository = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color ForeColor {
			get { return base.ForeColor; }
			set { base.ForeColor = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color BackColor {
			get { return base.BackColor; }
			set { base.BackColor = value; }
		}
		public virtual void BeginInit() {
			if(this._firstInit) {
				this._firstInit = false;
				OnFirstInit();
			}
			this.loadFired = false;
			this.fInitializing ++;
		}
		[Browsable(false)]
		public virtual bool IsLoading { get { return this.fInitializing != 0; } }
		public virtual void EndInit() {
			if(-- this.fInitializing == 0) OnEndInit();
		}
		protected virtual void OnFirstInit() { 
		}
		protected virtual void OnEndInit() {
			HookOnLoaded();
			if(IsHandleCreated && !DesignMode) {
				if(FireOnLoadOnPaint && !IsPainted && !Visible) return; 
				OnLoaded();
			}
		}
		protected override void OnVisibleChanged(EventArgs e) {
			base.OnVisibleChanged(e);
			if(!this.destroying && !DesignMode && Visible && IsHandleCreated && FireOnLoadOnPaint) OnLoaded();
		}
		protected override void OnCreateControl() {
			base.OnCreateControl();
			HookOnLoaded();
			if(!IsLoading && !DesignMode) {
				if(FireOnLoadOnPaint && !IsPainted) return; 
				OnLoaded();
			}
		}
		bool IsPainted { get { return !firstPaint; } }
		protected virtual bool FireOnLoadOnPaint { get { return false; } }
		protected override void OnPaint(PaintEventArgs e) {
			DevExpress.Utils.Mdi.ControlState.CheckPaintError(this);
			bool first = this.firstPaint;
			this.firstPaint = false;
			if(first && FireOnLoadOnPaint) {
				OnLoaded();
			}
		}
		IDesignerHost designerHost = null;
		protected virtual void RemoveHookOnLoaded() {
			if(designerHost != null) {
				designerHost.LoadComplete -= new EventHandler(OnDesignerHost_LoadComplete);
				designerHost.TransactionClosed -= new DesignerTransactionCloseEventHandler(OnDesignerHost_TransactionClosed);
			}
			this.designerHost = null;
		}
		protected virtual void HookOnLoaded() {
			RemoveHookOnLoaded();
			this.designerHost = GetService(typeof(IDesignerHost)) as IDesignerHost;
			if(designerHost != null) {
				this.designerHost.TransactionClosed += new DesignerTransactionCloseEventHandler(OnDesignerHost_TransactionClosed);
				designerHost.LoadComplete += new EventHandler(OnDesignerHost_LoadComplete);
			}
		}
		protected void OnDesignerHost_TransactionClosed(object sender, DesignerTransactionCloseEventArgs e) {
			if(this.designerHost != null && !this.designerHost.Loading
				&& (!this.designerHost.InTransaction || this.designerHost.TransactionDescription == null)) 
				OnLoaded();
		}
		protected void OnDesignerHost_LoadComplete(object sender, EventArgs e) {
			if(IsDisposed) return;
			OnLoaded();
		}
		protected virtual bool IsInitialized { get { return loadFired; } }
		protected virtual void OnLoaded() {
			if(IsLoading || IsInitialized) return;
			this.loadFired = true;
			EditorHelper.OnContainerLoaded();
		}
		int WM_CAPTURECHANGED = 0x215;
		protected override void WndProc(ref System.Windows.Forms.Message m) {
			if(m.Msg == WM_CAPTURECHANGED) {
				if(m.LParam == this.Handle) 
					OnGotCapture();
				else
					OnLostCapture();
			}
			if(m.Msg == DevExpress.Utils.Drawing.Helpers.MSG.WM_MOUSEHWHEEL) {
				DXMouseEventArgs me = ControlHelper.GenerateMouseHWheel(ref m, this);
				OnMouseWheel(me);
				if(me.Handled) {
					m.Result = new IntPtr(1);
					CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref m, this);
					return;
				}
			}
			base.WndProc(ref m);
			CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref m, this);
		}
		protected virtual void OnLostCapture() { }
		protected virtual void OnGotCapture() { }
		#region ContainerEvents
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("EditorContainerEditorKeyDown"),
#endif
 DXCategory(CategoryName.ContainerEvents)]
		public event KeyEventHandler EditorKeyDown {
			add { this.Events.AddHandler(editorKeyDown, value); }
			remove { this.Events.RemoveHandler(editorKeyDown, value); }
		}
		protected internal virtual void RaiseEditorKeyDown(KeyEventArgs e) {
			KeyEventHandler handler = (KeyEventHandler)this.Events[editorKeyDown];
			if(handler != null) handler(this, e);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("EditorContainerEditorKeyUp"),
#endif
 DXCategory(CategoryName.ContainerEvents)]
		public event KeyEventHandler EditorKeyUp {
			add { this.Events.AddHandler(editorKeyUp, value); }
			remove { this.Events.RemoveHandler(editorKeyUp, value); }
		}
		protected internal virtual void RaiseEditorKeyUp(KeyEventArgs e) {
			KeyEventHandler handler = (KeyEventHandler)this.Events[editorKeyUp];
			if(handler != null) handler(this, e);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("EditorContainerEditorKeyPress"),
#endif
 DXCategory(CategoryName.ContainerEvents)]
		public event KeyPressEventHandler EditorKeyPress {
			add { this.Events.AddHandler(editorKeyPress, value); }
			remove { this.Events.RemoveHandler(editorKeyPress, value); }
		}
		protected internal virtual void RaiseEditorKeyPress(KeyPressEventArgs e) {
			KeyPressEventHandler handler = (KeyPressEventHandler)this.Events[editorKeyPress];
			if(handler != null) handler(this, e);
		}
		PersistentRepository IEditorContainer.CreateInternalRepository() {
			return CreateInternalRepository();
		}
		protected virtual PersistentRepository CreateInternalRepository() { return new PersistentRepository(this); }
		void IEditorContainer.OnEditorKeyDown(KeyEventArgs e) {
			RaiseEditorKeyDown(e);
		}
		void IEditorContainer.OnEditorKeyUp(KeyEventArgs e) { 
			RaiseEditorKeyUp(e); 
		}
		void IEditorContainer.OnEditorKeyPress(KeyPressEventArgs e) {
			RaiseEditorKeyPress(e);
		}
		void IEditorContainer.OnRequireHideEditor() {
			RaiseRequireHideEditor();
		}
		void IEditorContainer.OnShowEditor(BaseEdit editor) {
			if(this.lockTabStop ++ == 0) {
				this.prevTabStop = TabStop;
			} else {
				this.lockTabStop --;
			}
			this.TabStop = false;
		}
		void IEditorContainer.OnHideEditor(BaseEdit editor) {
			this.TabStop = this.prevTabStop;
			this.lockTabStop --;
		}
		protected virtual void RaiseRequireHideEditor() {
		}
		#endregion ContainerEvents
	}
	public abstract class ContainerHelper : IDisposable {
		protected int fInternalFocusLock;
		protected BaseEdit fActiveEditor;
		int lockRepositoryItemUpdate = 0, allowHideException;
		DefaultEditorsRepository _defaultRepository;
		EditorsRepositoryBase _internalRepository;
		PersistentRepository _externalRepository;
		Hashtable editPainters;
		Hashtable editors;
		Component _owner;
		Dictionary<CacheItemKey, BaseEditViewInfo> repositoryViewInfoCache;
		public ContainerHelper(Component owner) {
			this.repositoryViewInfoCache = new Dictionary<CacheItemKey, BaseEditViewInfo>(new CacheItemKeyComparer());
			this.allowHideException = 0;
			this._owner = owner;
			this.fActiveEditor = null;
			this.fInternalFocusLock = 0;
			this._externalRepository = null;
			this.editPainters = new Hashtable();
			this.editors = new Hashtable();
			CreateRepositories();
		}
		protected virtual IDXMenuManager MenuManager { get { return null; } }
		protected Component Owner { get { return _owner; } }
		protected IEditorContainer OwnerContainer { get { return Owner as IEditorContainer; } }
		public virtual void Dispose() {
			HideHint();
			DestroyEditorsCache();
			DestroyRepositories();
			ClearViewInfoCache();
			ExternalRepository = null;
			this._owner = null;
		}
		List<RepositoryItem> notLoadedInternalRepositoryItems = new List<RepositoryItem>();
		List<RepositoryItem> notLoadedExternalRepositoryItems = new List<RepositoryItem>();
		protected List<RepositoryItem> NotLoadedInternalRepositoryItems { get { return notLoadedInternalRepositoryItems; } }
		protected List<RepositoryItem> NotLoadedExternalRepositoryItems { get { return notLoadedExternalRepositoryItems; } }
		[Obsolete, Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void InitializePosponedRespositories() {
			foreach(RepositoryItem item in NotLoadedInternalRepositoryItems)
				item.FireContainerLoaded();
			foreach(RepositoryItem item in NotLoadedExternalRepositoryItems)
				item.FireContainerLoaded();
		}
		public virtual void InitializePostponedRepositories() {
#pragma warning disable 612, 618
			InitializePosponedRespositories();
#pragma warning restore 612, 618
		}
		public virtual void FillPostponedRepositories() {
			for(int n = InternalRepository.Items.Count - 1; n >= 0; n--) {
				if(InternalRepository.Items[n].IsLoading && !NotLoadedInternalRepositoryItems.Contains(InternalRepository.Items[n]))
					NotLoadedInternalRepositoryItems.Add(InternalRepository.Items[n]);
			}
			if(ExternalRepository == null)
				return;
			for(int n = ExternalRepository.Items.Count - 1; n >= 0; n--) {
				if(ExternalRepository.Items[n].IsLoading && !NotLoadedExternalRepositoryItems.Contains(ExternalRepository.Items[n]))
					NotLoadedExternalRepositoryItems.Add(ExternalRepository.Items[n]);
			}
		}
		public virtual void OnContainerLoaded() {
			for(int n = InternalRepository.Items.Count - 1; n >= 0; n--) {
				if(InternalRepository.Items[n].IsLoading) {
					if(!NotLoadedInternalRepositoryItems.Contains(InternalRepository.Items[n]))
						NotLoadedInternalRepositoryItems.Add(InternalRepository.Items[n]);
				}
				else
					InternalRepository.Items[n].FireContainerLoaded();
			}
			if(ExternalRepository == null) return;
			for(int n = ExternalRepository.Items.Count - 1; n >= 0; n--) {
				if(ExternalRepository.Items[n].IsLoading) {
					if(!NotLoadedExternalRepositoryItems.Contains(ExternalRepository.Items[n]))
						NotLoadedExternalRepositoryItems.Add(ExternalRepository.Items[n]);
				}
				else
					ExternalRepository.Items[n].FireContainerLoaded();
			}
		}
		Dictionary<CacheItemKey, BaseEditViewInfo> RepositoryViewInfoCache { get { return repositoryViewInfoCache; } }
		public virtual void ClearViewInfoCache() {
			RepositoryViewInfoCache.Clear();
		}
		protected virtual DefaultEditorsRepository CreateDefaultEditorsRepository() {
			return new DefaultEditorsRepository();
		}
		protected virtual void CreateRepositories() {
			this._defaultRepository = CreateDefaultEditorsRepository();
			this._internalRepository = OwnerContainer.CreateInternalRepository();
			this._internalRepository.CollectionChanged += new CollectionChangeEventHandler(OnRepository_CollectionChanged);
			this._internalRepository.PropertiesChanged += new RepositoryItemChangedEventHandler(OnRepository_ItemChanged);
			this._internalRepository.RefreshRequired += new RepositoryItemChangedEventHandler(OnRepository_RefreshRequired);
			this._defaultRepository.CollectionChanged += new CollectionChangeEventHandler(OnRepository_CollectionChanged);
		}
		protected virtual void DestroyRepositories() {
			this._internalRepository.RefreshRequired -= new RepositoryItemChangedEventHandler(OnRepository_RefreshRequired);
			this._internalRepository.CollectionChanged -= new CollectionChangeEventHandler(OnRepository_CollectionChanged);
			this._internalRepository.PropertiesChanged -= new RepositoryItemChangedEventHandler(OnRepository_ItemChanged);
			this._defaultRepository.CollectionChanged -= new CollectionChangeEventHandler(OnRepository_CollectionChanged);
			InternalRepository.Dispose();
			DefaultRepository.Dispose();
		}
		public virtual void BeginAllowHideException() {
			this.allowHideException ++;
		}
		public virtual void EndAllowHideException() {
			this.allowHideException --;
		}
		bool lockHideException = false;
		public void LockHideException() { this.lockHideException = true; }
		public void UnlockHideException() { this.lockHideException = false; }
		public virtual bool AllowHideException { get { return allowHideException != 0 && !lockHideException; } }
		public virtual DefaultEditorsRepository DefaultRepository { get { return _defaultRepository; } }
		public virtual EditorsRepositoryBase InternalRepository { get { return _internalRepository; } }
		public virtual RepositoryItemCollection RepositoryItems { get { return InternalRepository.Items; } }
		public virtual PersistentRepository ExternalRepository {
			get { return _externalRepository; }
			set {
				if(ExternalRepository == value) return;
				if(ExternalRepository != null) {
					ExternalRepository.CollectionChanged -= new CollectionChangeEventHandler(OnRepository_CollectionChanged);
					ExternalRepository.Disposed -= new EventHandler(OnRepository_EditorsRepositoryDisposed);
					ExternalRepository.PropertiesChanged -= new RepositoryItemChangedEventHandler(OnRepository_ItemChanged);
				}
				_externalRepository = value;
				if(ExternalRepository != null) {
					ExternalRepository.PropertiesChanged += new RepositoryItemChangedEventHandler(OnRepository_ItemChanged);
					ExternalRepository.CollectionChanged += new CollectionChangeEventHandler(OnRepository_CollectionChanged);
					ExternalRepository.Disposed += new EventHandler(OnRepository_EditorsRepositoryDisposed);
				}
				OnEditorsRepositoryChanged();
			}
		}
		public int CalcDefaultMinHeight(Graphics graphics, AppearanceObject appearance) {
			int res = 10;
			if(DefaultRepository.Items.Count == 0) DefaultRepository.GetRepositoryItem(typeof(DateTime));
			for(int n = 0; n < DefaultRepository.Items.Count; n++) {
				RepositoryItem item = DefaultRepository.Items[n];
				using(BaseEditViewInfo info = item.CreateViewInfo()) {
					AppearanceHelper.Combine(info.PaintAppearance, new AppearanceObject[] { item.Appearance, appearance });
					res = Math.Max(res, info.CalcMinHeight(graphics)); 
				}
			}
			return res;
		}
		public virtual Hashtable EditPainters { get { return editPainters; } }
		public virtual Hashtable Editors { get { return editors; } }
		public virtual BaseEdit GetEditor(RepositoryItem item) {
			BaseEdit be = null;
			if(!Editors.ContainsKey(item)) {
				be = item.CreateEditor();
				be.SetEditorContainer(OwnerContainer);
				be.MenuManager = MenuManager;
				be.RightToLeft = RightToLeft.No;
				be.InplaceType = InplaceContainerType;
				Editors.Add(item, be);
			} else {
				be = (BaseEdit)Editors[item];
			}
			return be;
		}
		public abstract InplaceType InplaceContainerType { get ; }
		public void DestroyEditorsCache() { DestroyEditorsCache(null); }
		public virtual void DestroyEditorsCache(RepositoryItem item) {
			ClearViewInfoCache();
			if(ActiveEditor != null && (item == null || ActiveEditor.Tag == null || ActiveEditor.Tag == item)) {
				if(OwnerContainer != null) OwnerContainer.OnRequireHideEditor();
			}
			if(item != null) {
				BaseEdit be = Editors[item] as BaseEdit;
				if(be != null) {
					be.Dispose();
					Editors.Remove(item);
				}
				return;
			}
			foreach(DictionaryEntry entry in Editors) {
				BaseEdit be = entry.Value as BaseEdit;
				be.Dispose();
			}
			Editors.Clear();
		}
		public virtual BaseEditViewInfo GetCachedViewInfo(RepositoryItem item, string exHash, Rectangle bounds) {
			if(RepositoryViewInfoCache.Count == 0) return null;
			BaseEditViewInfo ret;
			RepositoryViewInfoCache.TryGetValue(GetHashCode(item, exHash, bounds), out ret);
			return ret;
		}
		const string divider = "---";
		class CacheItemKeyComparer : IEqualityComparer<CacheItemKey> {
			public bool Equals(CacheItemKey x, CacheItemKey y) {
				return x.Equals(y);
			}
			public int GetHashCode(CacheItemKey obj) {
				return obj.GetHashCode();
			}
		}
		struct CacheItemKey {
			int ItemHash;
			string ExHash;
			Rectangle Bounds;
			public bool Equals(CacheItemKey obj) {
				return ItemHash == obj.ItemHash && Bounds == obj.Bounds && ExHash == obj.ExHash;
			}
			public override bool Equals(object obj) {
				return obj is CacheItemKey ? Equals((CacheItemKey)obj) : false;
			}
			public override int GetHashCode() {
				return ItemHash ^ ExHash.GetHashCode() ^ Bounds.GetHashCode();
			}
			public CacheItemKey(int itemHash, string exHash, Rectangle bounds) {
				ItemHash = itemHash;
				ExHash = exHash;
				Bounds = bounds;
			}
		}
		CacheItemKey GetHashCode(RepositoryItem item, string exHash, Rectangle bounds) {
			return new CacheItemKey(item.GetHashCode(), exHash, bounds);
		}
		public virtual void AddCachedViewInfo(RepositoryItem item, string exHash, BaseEditViewInfo info) {
			if(info == null || !info.IsSupportFastViewInfo) return;
			info.Tag = null;
			if(RepositoryViewInfoCache.Count > 1000) RepositoryViewInfoCache.Clear();
			CacheItemKey key = GetHashCode(item, exHash, info.Bounds);
			if(RepositoryViewInfoCache.ContainsKey(key)) return;
			RepositoryViewInfoCache.Add(key, info);
		}
		public virtual BaseEditPainter GetPainter(RepositoryItem item) {
			DevExpress.XtraEditors.Drawing.BaseEditPainter result = null;
			if(!EditPainters.ContainsKey(item.EditorTypeName)) {
				EditPainters.Add(item.EditorTypeName, item.CreatePainter());
			}
			result = (BaseEditPainter)EditPainters[item.EditorTypeName];
			return result;
		}
		protected abstract bool IsLoading { get; }
		protected virtual void OnRepository_CollectionChanged(object sender, CollectionChangeEventArgs e) {
			RepositoryItem item = e.Element as RepositoryItem;
			if(e.Action == CollectionChangeAction.Add) {
				if(IsLoading && sender != DefaultRepository) return;
				OnRepositoryItemAdded(item);
			}
			if(e.Action == CollectionChangeAction.Remove) {
				if(IsLoading) return;
				OnRepositoryItemRemoved(item);
			}
		}
		public virtual RepositoryItem FindRepositoryItem(string name) {
			RepositoryItem ri = InternalRepository.Items[name];
			if(ri != null) return ri;
			if(ExternalRepository != null) return ExternalRepository.Items[name];
			return null;
		}
		protected virtual void OnRepository_RefreshRequired(object sender, RepositoryItemChangedEventArgs e) {
			if(this.lockRepositoryItemUpdate != 0 || IsLoading) return;
			OnRepositoryRefreshRequired(e.Item);
		}
		protected virtual void OnRepository_ItemChanged(object sender, RepositoryItemChangedEventArgs e) {
			if(this.lockRepositoryItemUpdate != 0 || IsLoading) return;
			OnRepositoryItemChanged(e.Item);
		}
		protected void OnRepository_EditorsRepositoryDisposed(object sender, EventArgs e) {
			this._externalRepository = null;
			OnEditorsRepositoryChanged();
		}
		protected virtual void OnEditorsRepositoryChanged() {
		}
		protected virtual void OnRepositoryItemAdded(RepositoryItem item) {
			ApplyDefaults(item);
			if(!IsLoading) item.FireContainerLoaded();
		}
		protected virtual void OnRepositoryItemChanged(RepositoryItem item) {
			DestroyEditorsCache(item);
		}
		protected virtual void OnRepositoryRefreshRequired(RepositoryItem item) {
		}
		protected virtual void OnRepositoryItemRemoved(RepositoryItem item) {
		}
		protected virtual void ApplyDefaults(RepositoryItem item) {
			this.lockRepositoryItemUpdate ++;
			try {
				OnSetRepositoryItemDefaults(item);
			}
			finally {
				this.lockRepositoryItemUpdate --;
			}
		}
		protected virtual void OnSetRepositoryItemDefaults(RepositoryItem item) {
			item.AutoHeight = false;
		}
		public void DrawCellEdit(GraphicsInfoArgs ginfo, RepositoryItem item, BaseEditViewInfo vi) {
			DrawCellEdit(ginfo, item, vi, null, Point.Empty);
		}
		public virtual void DrawCellEdit(GraphicsInfoArgs ginfo, RepositoryItem item, BaseEditViewInfo vi, AppearanceObject cellStyle, Point offset) {
			vi.PCount++;
			BaseEditPainter pb = GetPainter(item);
			AppearanceObject savedStyle = vi.PaintAppearance;
			if(cellStyle != null) {
				vi.PaintAppearance = cellStyle;
			}
			if(!offset.IsEmpty)
				vi.Offset(offset.X, offset.Y);
			try {
				pb.Draw(new ControlGraphicsInfoArgs(vi, ginfo, vi.Bounds));
			}
			finally {
				if(!offset.IsEmpty) 
					vi.Offset(-offset.X, -offset.Y);
				if(cellStyle != null)
					vi.PaintAppearance = savedStyle;
			}
		}
		static readonly object EmptyEditValue = new object();
		public virtual BaseEdit UpdateEditor(RepositoryItem ritem, UpdateEditorInfoArgs args) {
			BaseEdit be = GetEditor(ritem);
			if(be.IsDisposed) {
				Editors.Remove(ritem);
				be = GetEditor(ritem);
			}
			be.RightToLeft = args.RightToLeft ? System.Windows.Forms.RightToLeft.Yes : RightToLeft.No;
			if(ritem != null) be.Tag = ritem;
			AppearanceObject newAppearance = new AppearanceObject();
			AppearanceHelper.Combine(newAppearance, new AppearanceObject[] { ritem.Appearance, ritem.GetOverrideAppearance(), args.Appearance });
			if(newAppearance.BackColor == Color.Empty) {
				newAppearance.BackColor = be.InplaceType == InplaceType.Bars? BarSkins.GetSkin(args.LookAndFeel).GetSystemColor(SystemColors.Window): SystemColors.Window;
			}
			if(newAppearance.ForeColor == Color.Empty) {
				newAppearance.ForeColor = be.InplaceType == InplaceType.Bars ? BarSkins.GetSkin(args.LookAndFeel).GetSystemColor(SystemColors.WindowText) : SystemColors.WindowText;
			}
			newAppearance.BackColor = Color.FromArgb(255, newAppearance.BackColor);
			newAppearance.ForeColor = Color.FromArgb(255, newAppearance.ForeColor);
			((Control)be).TabStop = false;
			be.CausesValidation = false;
			be.Reset();
			be.Properties.BeginUpdate();
			be.Properties.AnnotationAttributes = ritem.AnnotationAttributes;
			try {
				be.Properties.ResetEvents();
				be.Properties.Assign(ritem);
				if(args.MakeReadOnly)
					be.Properties.ReadOnly = true;
				if(be.LookAndFeel.UseDefaultLookAndFeel) {
					be.LookAndFeel.Assign(args.LookAndFeel);
					be.LookAndFeel.SkinMaskColor = ((ISkinProviderEx)args.LookAndFeel).GetMaskColor();
					be.LookAndFeel.SkinMaskColor2 = ((ISkinProviderEx)args.LookAndFeel).GetMaskColor2();
				}
				be.Properties.AutoHeight = false; 
				be.Properties.Appearance.Assign(newAppearance);
				be.Location = args.Bounds.Location;
				be.Size = args.Bounds.Size;
				be.CreateControl();
				try {
					be.Properties.LockEvents();
					be.SetEmptyEditValue(EmptyEditValue);
					be.EditValue = args.EditValue;
					be.SetOldEditValue(args.EditValue);
					be.IsModified = false;
				}
				finally {
					be.Properties.UnLockEvents();
				}
				be.ErrorText = args.ErrorText;
				be.ErrorIcon = args.ErrorIcon;
			}
			finally {
				be.Properties.EndUpdate();
			}
			return be;
		}
		public void ShowEditor(BaseEdit edit, Control container) { ShowEditor(edit, container, false); }
		public virtual void ShowEditor(BaseEdit edit, Control container, bool checkForFocus) {
			this.fInternalFocusLock++;
			try {
				if(edit.Properties.AccessibleName == null) edit.Properties.AccessibleName = Localizer.Active.GetLocalizedString(StringId.ContainerAccessibleEditName); ;
				HideHint();
				container.Controls.Add(edit);
				edit.ToolTipController = RealToolTipController;;
				edit.Visible = true;
				if(checkForFocus) {
					if(edit.IsNeedFocus) edit.Focus();
				}
				else {
					Form form = container.FindForm();
					if(form is IFocusablePopupForm && !((IFocusablePopupForm)form).AllowFocus)
						XtraForm.SuppressDeactivation = true;
					edit.Focus();
					XtraForm.SuppressDeactivation = false;
				}
				if(OwnerContainer != null) OwnerContainer.OnShowEditor(edit);
				if(InplaceContainerType != InplaceType.Bars) edit.Enabled = true;
			}
			finally{
				this.fInternalFocusLock --;
			}
			this.fActiveEditor = edit;
		}
		public virtual int InternalFocusLock { get { return fInternalFocusLock; } }
		public virtual BaseEdit ActiveEditor { get { return fActiveEditor; } }
		public virtual void HideEditorCore(Control container, bool setFocus) {
			BaseEdit be = ActiveEditor;
			if(be == null) return;
			this.fActiveEditor = null;
			be.ToolTipController = null;
			HideHint();
			this.fInternalFocusLock ++;
			try {
				if(be.InplaceType == InplaceType.Bars) setFocus = true; 
				if(setFocus && container != null) container.Focus();
				be.Visible = false;
				ContainerControl form = FindContainer(be);
				if(be.IsHandleCreated) be.BeginInvoke(new MethodInvoker(() => {
					if(!be.Visible) be.DestroyHandleCore();
				}));
#if !DXWhidbey
				UpdateActiveControl(be, form);
#endif
				if(be.InplaceType != InplaceType.Bars) {
					be.Visible = false;
				} else {
					if (container != null) {
						SafeRemoveControl(container, be);
					}
				}
				UpdateUnvalidatedControl(be, form, be.InplaceType != InplaceType.Bars ?  container : null);
				if(OwnerContainer != null) OwnerContainer.OnHideEditor(be);
				CheckDestroyEditor(be);
			}
			finally {
				this.fInternalFocusLock --;
			}
		}
		protected void SafeRemoveControl(Control control, Control controlToRemove) {
			Form f = control.FindForm();
			Form activeMdiChild = null;
			if (f != null && f.IsMdiContainer) { activeMdiChild = f.ActiveMdiChild; }
			control.Controls.Remove(controlToRemove);
			if (f != null &&
				f.IsMdiContainer &&
				activeMdiChild != null &&
				activeMdiChild != f.ActiveMdiChild
				) { activeMdiChild.Activate(); }
		}
		protected void CheckDestroyEditor(BaseEdit be) {
			be.Tag = null; 
			be.Properties.ResetEvents();
		}
		protected ContainerControl FindContainer(Control ctrl) {
			Control parent = ctrl.Parent;
			if(parent is ContainerControl) return parent as ContainerControl;
			if(parent == null) return ctrl.FindForm();
			return FindContainer(parent);
		}
		public virtual HorzAlignment GetDefaultValueAlignment(RepositoryItem editor, Type columnType) {
			HorzAlignment result = HorzAlignment.Default;
			if(editor != null) result = editor.DefaultAlignment;
			if(result == HorzAlignment.Default) {
				result = HorzAlignment.Near;
				if(DevExpress.Data.Helpers.DefaultColumnAlignmentHelper.IsColumnFarAlignedByDefault(columnType))
					result = HorzAlignment.Far;
			}
			return result;
		}
		protected virtual BaseContainerValidateEditorEventArgs CreateValidateEventArgs(object fValue) {
			return new BaseContainerValidateEditorEventArgs(fValue);
		}
		public virtual bool ValidateEditor(IWin32Window owner) {
			if(ActiveEditor == null) return true;
			ActiveEditor.CompleteChanges();
			BaseContainerValidateEditorEventArgs va = CreateValidateEventArgs(EditingValue);
			try {
				RepositoryItem rItem = ActiveEditor.Properties;
				try {
					rItem.LockEvents();
					va.Valid = ActiveEditor.DoValidate();
					va.Value = EditingValue;
					va.TryValidateViaAnnotationAttributes(EditingValue, rItem.AnnotationAttributes);
					RaiseValidatingEditor(va);
					if(va.Valid) EditingValue = va.Value;
				}
				finally {
					rItem.UnLockEvents();
				}
			} catch(Exception e) {
				va.ErrorText = e.Message;
				OnInvalidValueException(owner, new EditorValueException(e, e.Message), va.Value);
				return false;
			}
			if(!va.Valid) {
				if(va.ErrorText == null || va.ErrorText.Length == 0) va.ErrorText = Localizer.Active.GetLocalizedString(StringId.InvalidValueText); 
				OnInvalidValueException(owner, new EditorValueException(new WarningException(va.ErrorText), va.ErrorText), va.Value);
			}
			return va.Valid;
		}
		public virtual void OnInvalidValueException(IWin32Window owner, Exception sourceException, object fValue) {
			bool isEditorException = sourceException is EditorValueException;
			string errorText =  isEditorException ? ((EditorValueException)sourceException).ErrorText : sourceException.Message;
			InvalidValueExceptionEventArgs e = new InvalidValueExceptionEventArgs(errorText, sourceException, fValue);
			RaiseInvalidValueException(e);
			if(e.ExceptionMode == ExceptionMode.ThrowException) throw e.Exception;
			if(e.ExceptionMode == ExceptionMode.DisplayError) {
				if(!isEditorException || !ShowEditorError(e.ErrorText))
					ShowError(owner, e.WindowCaption, e.ErrorText);
			}
			if(e.ExceptionMode == ExceptionMode.Ignore) return;
			if(AllowHideException) throw new HideException();
		}
		public virtual bool ShowEditorError(string errorText) {
			if(ActiveEditor == null) return false;
			if(errorText == null || errorText.Length == 0) errorText = Localizer.Active.GetLocalizedString(StringId.InvalidValueText); 
			ActiveEditor.ErrorText = errorText;
			ToolTipControllerShowEventArgs tool = RealToolTipController.CreateShowArgs();
			tool.SelectedControl = ActiveEditor;
			tool.IconType = ToolTipIconType.Error;
			tool.IconSize = ToolTipIconSize.Small;
			tool.ToolTip = errorText;
			RealToolTipController.ShowHint(tool, ActiveEditor);
			return true;
		}
		public virtual void ShowError(IWin32Window owner, string windowCaption, string errorText) {
			XtraMessageBox.Show(owner, errorText, windowCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		protected abstract void RaiseInvalidValueException(InvalidValueExceptionEventArgs e);
		protected abstract void RaiseValidatingEditor(BaseContainerValidateEditorEventArgs va);
		public virtual object EditingValue { 
			get { return ActiveEditor == null ? null : ActiveEditor.EditValue; } 
			set { if(ActiveEditor != null) ActiveEditor.EditValue = value; }
		}
		public abstract ToolTipController RealToolTipController {
			get; 
		}
		public virtual void HideHint() {
			if(RealToolTipController != null)
				RealToolTipController.HideHint();
		}
		public virtual void ShowHint(ToolTipControllerShowEventArgs eShow, Control controlClient) {
			RealToolTipController.ShowHint(eShow, controlClient);
		}
		public static void UpdateActiveControl(Control dock, ContainerControl form) {
			if(form == null) return;
			try {
				System.Reflection.FieldInfo[] fields = typeof(ContainerControl).GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
				if(fields == null) return;
				foreach(System.Reflection.FieldInfo fi in fields) {
					if(fi.Name == "activeControl") {
						Control control = fi.GetValue(form) as Control;
						if(control != null && (control == dock || dock.Contains(control)))
							fi.SetValue(form, null);
					}
				}
			}
			catch {
			}
		}
		public static void UpdateUnvalidatedControl(Control dock, ContainerControl form, Control container) {
			if(form == null) return;
			try {
				System.Reflection.FieldInfo[] fields = typeof(ContainerControl).GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
				if(fields == null) return;
				foreach(System.Reflection.FieldInfo fi in fields) {
					if(fi.Name == "unvalidatedControl" || fi.Name == "focusedControl") {
						Control control = fi.GetValue(form) as Control;
						if(control != null && ContainsControl(dock, control))
							fi.SetValue(form, container);
					}
				}
			}
			catch {
			}
		}
		public static void ClearUnvalidatedControl(Control dock, ContainerControl form) {
			if(form == null) return;
			try {
				System.Reflection.FieldInfo[] fields = typeof(ContainerControl).GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
				if(fields == null) return;
				foreach(System.Reflection.FieldInfo fi in fields) {
					if(fi.Name == "unvalidatedControl" || fi.Name == "focusedControl") {
						Control control = fi.GetValue(form) as Control;
						if(control != null && ContainsControl(dock, control)) 
							fi.SetValue(form, null);
					}
				}
			}
			catch {
			}
		}
		static bool ContainsControl(Control parent, Control child) {
			if(parent == null || child == null) return false;
			if(parent == child || parent.Contains(child)) return true;
			Control check = child;
			while(check.Parent != null) { 
				if(check == parent) return true; 
				check = check.Parent;
			}
			return false;
		}
	}
	public abstract class EditorContainerHelper : ContainerHelper {
		public EditorContainerHelper(EditorContainer owner) : base(owner) {
		}
		protected override IDXMenuManager MenuManager { get { return Owner.MenuManager; } }
		protected new EditorContainer Owner { get { return base.Owner as EditorContainer; } }
		protected override bool IsLoading { get { return Owner.IsLoading; } }
		public override ToolTipController RealToolTipController {
			get { return Owner == null || Owner.ToolTipController == null ? ToolTipController.DefaultController : Owner.ToolTipController; }
		}
		public override InplaceType InplaceContainerType { get { return InplaceType.Grid; } }
	}
	[ToolboxItem(false)]
	public abstract class ComponentEditorContainer : Component, ISupportInitialize, IEditorContainer {
		protected int fInitializing;
		bool _firstInit;
		bool loadFired = false;
		ContainerHelper editorHelper;
		ToolTipController toolTipController;
		private static readonly object editorKeyDown = new object();
		private static readonly object editorKeyPress = new object();
		private static readonly object editorKeyUp = new object();
		public ComponentEditorContainer() {
			this.fInitializing = 0;
			this._firstInit = true;
			this.editorHelper = CreateHelper();
			this.toolTipController = null;
		}
		SizeF IEditorContainer.ScaleFactor { get { return new SizeF(1, 1); } }
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ComponentEditorContainerToolTipController"),
#endif
 DefaultValue(null)]
		public ToolTipController ToolTipController {
			get { return toolTipController; }
			set {
				if(ToolTipController == value) return;
				OnChangeToolTipController(ToolTipController, value);
				toolTipController = value;
			}
		}
		public ToolTipController GetToolTipController() {
			if(ToolTipController == null) return ToolTipController.DefaultController;
			return ToolTipController;
		}
		protected virtual void OnChangeToolTipController(ToolTipController old, ToolTipController newController) {
		}
		protected ContainerHelper EditorHelper { get { return editorHelper; } }
		protected abstract ComponentEditorContainerHelper CreateHelper();
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(EditorHelper != null) {
					EditorHelper.Dispose();
				}
				ToolTipController = null;
			}
			base.Dispose(disposing);
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual RepositoryItemCollection RepositoryItems { get { return EditorHelper.RepositoryItems; } }
		[DXCategory(CategoryName.Data), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ComponentEditorContainerExternalRepository"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), DefaultValue(null)]
		public PersistentRepository ExternalRepository {
			get { return EditorHelper.ExternalRepository; }
			set { EditorHelper.ExternalRepository = value; }
		}
		public virtual void BeginInit() {
			if(this._firstInit) {
				this._firstInit = false;
				OnFirstInit();
			}
			this.loadFired = false;
			this.fInitializing ++;
		}
		[Browsable(false)]
		public virtual bool IsLoading { get { return this.fInitializing != 0; } }
		public virtual void EndInit() {
			if(-- this.fInitializing == 0) OnEndInit();
		}
		protected virtual void OnFirstInit() { 
		}
		protected virtual void OnEndInit() {
			OnLoaded();
		}
		protected virtual bool IsInitialized { get { return loadFired; } }
		protected virtual void OnLoaded() {
			if(IsLoading || IsInitialized) return;
			this.loadFired = true;
			EditorHelper.OnContainerLoaded();
		}
		#region ContainerEvents
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ComponentEditorContainerEditorKeyDown"),
#endif
 DXCategory(CategoryName.ContainerEvents)]
		public event KeyEventHandler EditorKeyDown {
			add { this.Events.AddHandler(editorKeyDown, value); }
			remove { this.Events.RemoveHandler(editorKeyDown, value); }
		}
		protected internal virtual void RaiseEditorKeyDown(KeyEventArgs e) {
			KeyEventHandler handler = (KeyEventHandler)this.Events[editorKeyDown];
			if(handler != null) handler(this, e);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ComponentEditorContainerEditorKeyUp"),
#endif
 DXCategory(CategoryName.ContainerEvents)]
		public event KeyEventHandler EditorKeyUp {
			add { this.Events.AddHandler(editorKeyUp, value); }
			remove { this.Events.RemoveHandler(editorKeyUp, value); }
		}
		protected internal virtual void RaiseEditorKeyUp(KeyEventArgs e) {
			KeyEventHandler handler = (KeyEventHandler)this.Events[editorKeyUp];
			if(handler != null) handler(this, e);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ComponentEditorContainerEditorKeyPress"),
#endif
 DXCategory(CategoryName.ContainerEvents)]
		public event KeyPressEventHandler EditorKeyPress {
			add { this.Events.AddHandler(editorKeyPress, value); }
			remove { this.Events.RemoveHandler(editorKeyPress, value); }
		}
		protected internal virtual void RaiseEditorKeyPress(KeyPressEventArgs e) {
			KeyPressEventHandler handler = (KeyPressEventHandler)this.Events[editorKeyPress];
			if(handler != null) handler(this, e);
		}
		PersistentRepository IEditorContainer.CreateInternalRepository() {
			return CreateInternalRepository();
		}
		protected virtual PersistentRepository CreateInternalRepository() { return new PersistentRepository(this); }
		void IEditorContainer.OnEditorKeyDown(KeyEventArgs e) {
			RaiseEditorKeyDown(e);
		}
		void IEditorContainer.OnEditorKeyUp(KeyEventArgs e) { 
			RaiseEditorKeyUp(e); 
		}
		void IEditorContainer.OnEditorKeyPress(KeyPressEventArgs e) {
			RaiseEditorKeyPress(e);
		}
		void IEditorContainer.OnShowEditor(BaseEdit editor) {
		}
		void IEditorContainer.OnHideEditor(BaseEdit editor) {
		}
		void IEditorContainer.OnRequireHideEditor() {
			RaiseRequireHideEditor();
		}
		protected virtual void RaiseRequireHideEditor() {
		}
		#endregion ContainerEvents
	}
	public abstract class ComponentEditorContainerHelper : ContainerHelper {
		public ComponentEditorContainerHelper(ComponentEditorContainer owner) : base(owner) {
		}
		protected new ComponentEditorContainer Owner { get { return base.Owner as ComponentEditorContainer; } }
		protected override bool IsLoading { get { return Owner.IsLoading; } }
		public override InplaceType InplaceContainerType { get { return InplaceType.Grid; } }
		public override ToolTipController RealToolTipController {
			get { return Owner == null || Owner.ToolTipController == null ? ToolTipController.DefaultController : Owner.ToolTipController; }
		}
	}
	[System.Runtime.InteropServices.ComVisible(false)]
	public interface IEditorContainer {
		void OnEditorKeyDown(KeyEventArgs e);
		void OnEditorKeyUp(KeyEventArgs e);
		void OnEditorKeyPress(KeyPressEventArgs e);
		void OnRequireHideEditor();
		void OnShowEditor(BaseEdit editor);
		void OnHideEditor(BaseEdit editor);
		PersistentRepository CreateInternalRepository();
		SizeF ScaleFactor { get; }
	}
	public class UpdateEditorInfoArgs {
		Rectangle bounds;
		AppearanceObject appearance;
		object editValue;
		UserLookAndFeel lookAndFeel;
		bool makeReadOnly;
		string errorText;
		Image errorIcon;
		bool rightToLeft;
		public UpdateEditorInfoArgs(bool makeReadOnly, Rectangle bounds, AppearanceObject appearance, object editValue, UserLookAndFeel lookAndFeel) :
			this(makeReadOnly, bounds, appearance, editValue, lookAndFeel, "") { }
		public UpdateEditorInfoArgs(bool makeReadOnly, Rectangle bounds, AppearanceObject appearance, object editValue, UserLookAndFeel lookAndFeel, string errorText) :
			this(makeReadOnly, bounds, appearance, editValue, lookAndFeel, errorText, null) { }
		public UpdateEditorInfoArgs(bool makeReadOnly, Rectangle bounds, AppearanceObject appearance, object editValue, UserLookAndFeel lookAndFeel, string errorText, Image errorIcon) :
			this(makeReadOnly, bounds, appearance, editValue, lookAndFeel, errorText, errorIcon, false) { }
		public UpdateEditorInfoArgs(bool makeReadOnly, Rectangle bounds, AppearanceObject appearance, object editValue, UserLookAndFeel lookAndFeel, string errorText, Image errorIcon, bool rightToLeft) {
			this.rightToLeft = rightToLeft;
			this.makeReadOnly = makeReadOnly;
			this.lookAndFeel = lookAndFeel;
			this.bounds = bounds;
			this.appearance = appearance;
			this.editValue = editValue;
			this.errorText = errorText;
			this.errorIcon = errorIcon;
		}
		public bool RightToLeft { get { return rightToLeft; } set { rightToLeft = value; } }
		public string ErrorText { get { return errorText; } }
		public Image ErrorIcon { get { return errorIcon; } }
		public bool MakeReadOnly { get { return makeReadOnly; } }
		public UserLookAndFeel LookAndFeel { get { return lookAndFeel; } }
		public Rectangle Bounds { get { return bounds; } }
		public AppearanceObject Appearance { get { return appearance; } }
		public object EditValue { get { return editValue; } }
	}
}
