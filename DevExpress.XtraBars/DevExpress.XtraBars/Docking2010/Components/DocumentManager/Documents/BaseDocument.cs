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
using DevExpress.Utils;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.XtraBars.Docking2010.Base;
namespace DevExpress.XtraBars.Docking2010.Views {
	[ToolboxItem(false), DesignTimeVisible(false)]
	public abstract class BaseDocument : BaseComponent, ILogicalOwner, IXtraSerializableChildren, IDocumentCaptionAppearanceProvider {
		string captionCore;
		string headerCore;
		string footerCore;
		Image imageCore;
		int imageIndexCore = -1;
		internal Control controlCore;
		Form formCore;
		DocumentManager managerCore;
		IBaseDocumentDefaultProperties propertiesCore;
		protected BaseDocument()
			: base(null) {
			InitProperties(null);
		}
		protected BaseDocument(IContainer container)
			: base(container) {
			InitProperties(null);
		}
		protected BaseDocument(IBaseDocumentProperties parentProperties)
			: base(null) {
			InitProperties(parentProperties);
		}
		protected override void OnDispose() {
			isActiveCore = false;
			if(IsDeferredControlLoad && IsControlLoadedByQueryControl) {
				BaseView view = Manager != null ? Manager.View : null;
				ReleaseDeferredLoadControl(view);
			}
			if(Properties != null)
				Properties.Changed -= OnPropertiesChanged;
			BaseDocumentSettings.Detach(Control);
			Ref.Dispose(ref propertiesCore);
			ReleaseControl();
			managerCore = null;
			base.OnDispose();
		}
		protected void ReleaseControl() {
			object target = (subscription != null) ? subscription.Target : null;
			UnsubscribeControl();
			if(Form != null) {
				if(!IsFormDisposingLocked && !Form.IsDisposed) {
					Docking.FloatForm floatForm = Form as Docking.FloatForm;
					FloatDocumentForm floatDocumentForm = Form as FloatDocumentForm;
					if(floatForm != null || floatDocumentForm != null) {
						if(floatForm != null && !floatForm.IsDisposing) {
							if(Manager != null && !Manager.IsDisposing) {
								if(Manager.UIViewAdapter != null)
									Manager.UnregisterFloatPanel(floatForm);
								if(target != null && floatForm.FloatLayout.Panel == target)
									Form.Dispose();
							}
						}
						if(floatDocumentForm != null && !floatDocumentForm.IsDisposing)
							Form.Dispose();
					}
					else {
						if(!Form.Disposing && !IsFormClose)
							Form.Dispose();
					}
				}
				formCore = null;
			}
			controlCore = null;
		}
		IDisposable viewLayoutChangedTracker;
		protected override void LockComponentBeforeDisposing() {
			viewLayoutChangedTracker = GetViewLayoutChangedTracker();
			base.LockComponentBeforeDisposing();
		}
		protected override void AfterComponentDisposing() {
			base.AfterComponentDisposing();
			Ref.Dispose(ref viewLayoutChangedTracker);
		}
		protected virtual IComponentLayoutChangedTracker GetViewLayoutChangedTracker() {
			return (Manager != null && Manager.View != null) ? Manager.View.TrackLayoutChanged() : null;
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BaseDocumentProperties")]
#endif
		[Category(DevExpress.XtraEditors.CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public IBaseDocumentDefaultProperties Properties {
			get { return propertiesCore; }
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShouldSerializeProperties() {
			return Properties != null && Properties.ShouldSerialize();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void ResetProperties() {
			Properties.Reset();
		}
		void InitProperties(IBaseDocumentProperties parentProperties) {
			propertiesCore = CreateDefaultProperties(parentProperties);
			Properties.Changed += OnPropertiesChanged;
		}
		void OnPropertiesChanged(object sender, EventArgs e) {
			PropertyChangedEventArgs pce = e as PropertyChangedEventArgs;
			if(IsDockPanel && pce != null)
				UpdateDockPanelProperties(pce);
			LayoutChanged();
		}
		protected virtual IBaseDocumentDefaultProperties CreateDefaultProperties(IBaseDocumentProperties parentProperties) {
			return new BaseDocumentDefaultProperties(parentProperties);
		}
		WeakReference subscription;
		protected virtual void SubscribeControl() {
			Control target = Control;
			if(IsDockPanel)
				target = GetDockPanel();
			if(target != null) {
				subscription = new WeakReference(target);
				SubscribeCore(target);
			}
		}
		protected internal Docking.DockPanel GetDockPanel() {
			Docking.FloatForm form = Form as Docking.FloatForm;
			if(form != null && form.FloatLayout != null)
				return form.FloatLayout.Panel;
			return null;
		}
		protected virtual void UnsubscribeControl() {
			if(subscription != null) {
				Control target = subscription.Target as Control;
				UnsubscribeCore(target);
				subscription = null;
			}
		}
		protected virtual void SubscribeCore(Control control) {
			if(control == null) return;
			control.VisibleChanged += OnControlVisibleChanged;
			control.TextChanged += OnControlTextChanged;
			control.Disposed += OnControlDisposed;
		}
		protected virtual void UnsubscribeCore(Control control) {
			if(control == null) return;
			control.VisibleChanged -= OnControlVisibleChanged;
			control.TextChanged -= OnControlTextChanged;
			control.Disposed -= OnControlDisposed;
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BaseDocumentCaption")]
#endif
		[Category("Appearance"), DefaultValue(null), Localizable(true)]
		public string Caption {
			get { return captionCore; }
			set { SetValue(ref captionCore, value, OnCaptionChanged); }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BaseDocumentHeader")]
#endif
		[Category("Appearance"), DefaultValue(null), Localizable(true)]
		public string Header {
			get { return headerCore; }
			set { headerCore = value; }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BaseDocumentFooter")]
#endif
		[Category("Appearance"), DefaultValue(null), Localizable(true)]
		public string Footer {
			get { return footerCore; }
			set { footerCore = value; }
		}
		Size? floatSizeCore;
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BaseDocumentFloatSize")]
#endif
		[Category("Layout"), DefaultValue(null)]
		public Size? FloatSize {
			get {
				if(floatSizeCore == null) {
					if(IsControlLoaded) {
						var settings = BaseDocumentSettings.GetProvider<IBaseDocumentSettings>(Control);
						if(settings != null)
							floatSizeCore = settings.FloatSize;
					}
				}
				if(IsFloating && !IsMaximized) floatSizeCore = formCore.Size;
				return floatSizeCore;
			}
			set { floatSizeCore = value; }
		}
		Point? floatLocationCore;
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BaseDocumentFloatLocation")]
#endif
		[Category("Layout"), DefaultValue(null)]
		public Point? FloatLocation {
			get {
				if(floatLocationCore == null) {
					if(IsControlLoaded) {
						var settings = BaseDocumentSettings.GetProvider<IBaseDocumentSettings>(Control);
						if(settings != null)
							floatLocationCore = settings.FloatLocation;
					}
				}
				if(IsFloating && !IsMaximized) floatLocationCore = formCore.Location;
				return floatLocationCore;
			}
			set { floatLocationCore = value; }
		}
		protected virtual internal void SetBoundsCore(Rectangle bounds) { }
		protected virtual internal Rectangle GetBoundsCore() { return Rectangle.Empty; }
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BaseDocumentImage")]
#endif
		[Category("Appearance"), DefaultValue(null), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(System.Drawing.Design.UITypeEditor))]
		public Image Image {
			get { return imageCore; }
			set {
				SetValue(ref imageCore, value);
				OnImageChanged();
			}
		}
		protected virtual void OnImageChanged() { }
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BaseDocumentImageIndex")]
#endif
		[Category("Appearance"), DefaultValue(-1)]
		public int ImageIndex {
			get { return imageIndexCore; }
			set { SetValue(ref imageIndexCore, value); }
		}
		DxImageUri imageUriCore = new DxImageUri();		
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseDocumentImageUri"),
#endif
 Category("Appearance"), DefaultValue(null)]
		[TypeConverter(typeof(ExpandableObjectConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DxImageUri ImageUri {
			get { return imageUriCore; }
			set { SetValue(ref imageUriCore, value); }
		}
		[Browsable(false)]
		public Form Form {
			get { return formCore; }
		}
		object tagCore;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseDocumentTag"),
#endif
 DefaultValue(null), Category("Data"),
		Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))]
		public object Tag {
			get { return tagCore; }
			set { tagCore = value; }
		}
		protected internal Size InitializedControlSize { get; set; }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Control Control {
			get { return controlCore; }
			internal set {
				if(controlCore == value) return;
				UnsubscribeControl();
				controlCore = value;
				formCore = value as Form;
				if(value is Docking.FloatForm) {
					MarkAsDockPanel();
					InitializePropertiesFromDockPanel();
				}
				if(value != null) {
					InitializedControlSize = value.Size;
					if(formCore == null) {
						formCore = CreateFloatDocumentForm();
						value.Dock = DockStyle.Fill;
						if(IsNonMdi)
							value.Parent = Manager.View.CreateDocumentContainer(this);
						else value.Parent = Form;
					}
					var settings = BaseDocumentSettings.GetProvider<IBaseDocumentSettings>(value);
					if(settings != null)
						ApplySettingsCore(settings);
					if(formCore != null)
						InitializeFromForm();
				}
				SubscribeControl();
				LayoutChanged();
			}
		}
		protected virtual FloatDocumentForm CreateFloatDocumentForm() {
			return new FloatDocumentForm(this);
		}
		protected virtual void ApplySettingsCore(IBaseDocumentSettings settings) {
			if(string.IsNullOrEmpty(captionCore))
				this.captionCore = settings.Caption;
			if(imageCore == null)
				this.imageCore = settings.Image;
		}
		[Browsable(false)]
		public bool IsEnabled {
			get { return CalcIsEnabled(); }
		}
		[Browsable(false)]
		public bool IsVisible {
			get { return CalcIsVisible(); }
		}
		protected internal bool CanActivate() {
			return !IsDisposing && IsEnabled && Properties.CanActivate;
		}
		protected internal bool CanFloat() {
			return !IsDisposing && IsEnabled && Properties.CanFloat;
		}
		protected internal bool CanFloat(BaseView view) {
			return CanFloat() && view.CanFloat(this);
		}
		protected virtual bool CalcIsEnabled() {
			return IsControlLoaded ? Control.Enabled : IsDeferredControlLoad;
		}
		protected virtual bool CalcIsVisible() {
			return IsControlLoaded ? Control.Visible : IsDeferredControlLoad;
		}
		protected internal virtual bool Borderless {
			get { return true; }
		}
		protected internal virtual bool HasCloseButton() {
			return Properties.CanClose;
		}
		protected internal virtual bool HasMaximizeButton() {
			return true;
		}
		protected internal virtual bool CanMaximize() {
			return true;
		}
		protected internal string GetName() {
			if(IsDeferredControlLoad)
				return ControlName;
			if(IsDockPanel) {
				Docking.DockPanel panel = GetDockPanel();
				if(panel != null) {
					if(!string.IsNullOrEmpty(panel.Name))
						return panel.Name;
					return panel.ID.ToString();
				}
			}
			return (Control != null) ? Control.Name : null;
		}
		int lockFormDisposingCore = 0;
		protected internal bool IsFormDisposingLocked {
			get { return lockFormDisposingCore > 0; }
		}
		bool isFormCloseCore;
		protected internal bool IsFormClose {
			get { return isFormCloseCore; }
		}
		protected internal void SetIsFormClose() {
			isFormCloseCore = true;
		}
		protected internal void LockFormDisposing() {
			lockFormDisposingCore++;
		}
		protected internal void UnLockFormDisposing() {
			lockFormDisposingCore--;
		}
		internal int controlDispose;
		protected internal bool IsControlDisposeInProgress {
			get { return controlDispose > 0; }
		}
		protected internal bool IsFormDisposing {
			get { return (formCore != null) && formCore.Disposing; }
		}
		void OnControlDisposed(object sender, EventArgs e) {
			controlDispose++;
			Dispose();
			controlDispose--;
		}
		void OnControlTextChanged(object sender, EventArgs e) {
			if(IsInitializing) return;
			Caption = GetCaptionFromControl((Control)sender);
		}
		bool? visibleCore;
		void OnControlVisibleChanged(object sender, EventArgs e) {
			if(IsInitializing) return;
			bool visibility = visibleCore.GetValueOrDefault(true);
			bool actualVisibility = IsVisible;
			if(actualVisibility != visibility) {
				if(visibility)
					OnControlHidden();
				else {
					if(actualVisibility)
						OnControlShown();
				}
				visibleCore = !visibility;
			}
		}
		protected virtual void OnControlShown() { }
		protected virtual void OnControlHidden() {
			if(Manager == null || Manager.UIView == null) return;
			if(!IsFloating && !Manager.IsFloating)
				Manager.UIView.Invalidate();
		}
		protected override void OnLayoutChanged() {
			if(IsFloating && IsFloatDocument)
				DevExpress.Skins.XtraForm.FormPainter.InvalidateNC(Form);
		}
		protected internal virtual string GetCaptionFromControl(Control control) {
			return control.Text;
		}
		[Browsable(false)]
		public DocumentManager Manager {
			get { return managerCore; }
		}
		bool isDeferredControlLoadCore;
		[Browsable(false)]
		public bool IsDeferredControlLoad {
			get { return isDeferredControlLoadCore; }
		}
		protected internal bool IsControlLoaded {
			get { return controlCore != null; }
		}
		protected internal bool IsControlLoading {
			get { return controlLoading > 0; }
		}
		protected internal bool IsControlHandleCreated {
			get { return controlCore != null && Control.IsHandleCreated; }
		}
		protected internal bool IsControlLoadedByQueryControl { get; private set; }
		protected internal bool IsDesignMode(BaseView view) {
			return (view != null) && view.IsDesignMode();
		}
		protected internal bool EnsureDeferredLoadControl(BaseView view) {
			if(view == null || !IsDeferredControlLoad || IsControlLoadedByQueryControl)
				return false;
			return EnsureDeferredLoadControlCore(view, null);
		}
		protected internal bool EnsureDeferredLoadControlOnPopulate(BaseView view) {
			if(view == null || !IsDeferredControlLoad || IsControlLoadedByQueryControl)
				return false;
			bool result = EnsureDeferredLoadControlCore(view, depopulatedControlReference);
			if(result)
				depopulatedControlReference = null;
			return result;
		}
		int controlLoading = 0;
		bool EnsureDeferredLoadControlCore(BaseView view, WeakReference controlReference) {
			using(view.ShowLoadingIndicator(this)) {
				using(BatchUpdate.Enter(view.Manager, true)) {
					using(BatchUpdate.Enter(this, true)) {
						Control controlFromReference = (controlReference != null) ?
							controlReference.Target as Control : null;
						Control control = controlFromReference ?? view.RaiseQueryControl(this);
						if(control == null && IsDesignMode(view))
							control = new DesignModeContent() { Text = Caption };
						if(control != null) {
							BaseDocument boundDocument;
							if(view.Documents.TryGetValue(control, out boundDocument)) {
								if(boundDocument.IsControlLoadedByQueryControl)
									boundDocument.ReleaseDeferredLoadControlCore(view, false);
							}
							controlLoading++;
							try {
								if(view.Manager != null)
									view.Manager.EnsureDocument(this);
								view.OnDeferredLoadDocumentControlLoaded(control, this);
								if(string.IsNullOrEmpty(Caption) && !string.IsNullOrEmpty(control.Text))
									Caption = control.Text;
								Control = control;
								IsControlLoadedByQueryControl = true;
								if(view.Manager != null)
									view.Manager.AddDocumentToHost(this);
								EnsureFormManager(view.Manager);
								OnDeferredLoadControlComplete();
							}
							finally {
								view.OnDeferredLoadDocumentControlShown(control, this);
								controlLoading--;
							}
						}
						return control != null;
					}
				}
			}
		}
		protected virtual void OnDeferredLoadControlComplete() { }
		protected internal void ReleaseDeferredLoadControl(BaseView view) {
			ReleaseDeferredLoadControl(view, true, true);
		}
		protected internal void ReleaseDeferredLoadControl(BaseView view, bool keepControl, bool disposeControl) {
			if(view == null || !IsControlLoadedByQueryControl) return;
			if(IsDesignMode(view) && controlCore is DesignModeContent) return;
			ReleaseDeferredLoadControlCore(view, keepControl, disposeControl);
		}
		WeakReference depopulatedControlReference;
		protected internal void ReleaseDeferredLoadControlOnDepopulate(BaseView view) {
			if(view == null || (!IsControlLoadedByQueryControl && !CanReleaseDockPanel)) return;
			using(ReleaseDockPanel()) {
				bool isDesignModeContent = (IsDesignMode(view) && controlCore is DesignModeContent);
				if(!isDesignModeContent && !view.IsDisposing)
					depopulatedControlReference = new WeakReference(controlCore);
				ReleaseDeferredLoadControlCore(view, false, isDesignModeContent || view.IsDisposing);
			}
		}
		int lockReleaseDeferredLoadControl = 0;
		protected void ReleaseDeferredLoadControlCore(BaseView view, bool keepControl, bool disposeControl) {
			if(lockReleaseDeferredLoadControl > 0) return;
			lockReleaseDeferredLoadControl++;
			try {
				bool disposeControlResult;
				if(!view.RaiseControlReleasing(this, keepControl, disposeControl, out disposeControlResult)) return;
				using(BatchUpdate.Enter(view.Manager, true)) {
					if(!disposeControlResult)
						LockFormDisposing();
					ReleaseDeferredLoadControlCore(view, disposeControlResult);
					if(!disposeControlResult)
						UnLockFormDisposing();
				}
			}
			finally { lockReleaseDeferredLoadControl--; }
		}
		protected void ReleaseDeferredLoadControlCore(BaseView view, bool disposeControl) {
			using(BatchUpdate.Enter(this, true)) {
				Form.Visible = false;
				if(!IsDisposing && view.Manager != null) {
					view.releasingDeferredControlLoadDocument++;
					view.Manager.RemoveDocumentFromHost(this);
					view.releasingDeferredControlLoadDocument--;
				}
				Control control = Control;
				if(control != Form) {
					DocumentContainer container = control.Parent as DocumentContainer;
					if(container != null)
						container.LockDocumentDisposing();
					control.Parent = null;
				}
				ReleaseControl();
				view.OnDeferredLoadDocumentControlReleased(control, this);
				if(disposeControl) {
					BaseDocumentSettings.Detach(control);
					if(!control.IsDisposed)
						Ref.Dispose(ref control);
				}
				IsControlLoadedByQueryControl = false;
			}
		}
		protected internal bool IsDesignModeContent {
			get { return IsDeferredControlLoad && controlCore is DesignModeContent; }
		}
		string controlTypeNameCore;
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BaseDocumentControlTypeName")]
#endif
		[Category("Deferred Control Load Properties"), DefaultValue(null)]
		public string ControlTypeName {
			get { return controlTypeNameCore; }
			set {
				if(IsInitialized && !CanLoadControl()) return;
				controlTypeNameCore = value;
			}
		}
		string controlNameCore;
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BaseDocumentControlName")]
#endif
		[Category("Deferred Control Load Properties"), DefaultValue(null)]
		public string ControlName {
			get { return controlNameCore; }
			set {
				if(IsInitialized && !CanChangeControl()) return;
				controlNameCore = value;
			}
		}
		protected internal virtual bool EnsureIsBoundToControl(BaseView view) {
			if(CanLoadControl()) {
				if(!EnsureDeferredLoadControl(view))
					throw new DeferredLoadingException();
			}
			InitializePropertiesFromDockPanel();
			return controlCore != null;
		}
		protected internal virtual bool CanChangeControl() {
			return CanLoadControl();
		}
		protected internal virtual bool CanLoadControl() {
			return IsDeferredControlLoad && !IsControlLoaded;
		}
		protected internal bool CanLoadControlByName(string controlName) {
			return CanLoadControl() &&
				(!string.IsNullOrEmpty(controlName)) && (ControlName == controlName);
		}
		protected internal bool CanLoadControlByType(Type controlType) {
			return CanLoadControl() && (controlType != null) &&
				(ControlTypeName == controlType.FullName || ControlTypeName == controlType.Name);
		}
		protected internal bool CanLoadControl(Control control) {
			return CanLoadControl() && (control != null) &&
				(string.IsNullOrEmpty(ControlName) && string.IsNullOrEmpty(ControlTypeName));
		}
		bool isActiveCore;
		[Browsable(false)]
		public bool IsActive {
			get { return isActiveCore; }
		}
		bool isFloatingCore;
		[Browsable(false)]
		public bool IsFloating {
			get { return isFloatingCore; }
		}
		[Browsable(false)]
		public virtual bool IsMaximized {
			get { return Form != null && Form.WindowState == FormWindowState.Maximized; }
		}
		bool isDockPanelCore;
		[Browsable(false)]
		public bool IsDockPanel {
			get { return isDockPanelCore; }
		}
		protected internal bool IsMdiForm {
			get { return controlCore is Form; }
		}
		protected internal bool IsFloatDocument {
			get { return formCore is FloatDocumentForm; }
		}
		bool isNonMdiCore;
		protected internal bool IsNonMdi {
			get { return isNonMdiCore; }
		}
		bool isDocumentsHostCore;
		protected internal bool IsDocumentsHost {
			get { return isDocumentsHostCore; }
		}
		bool isContainerCore;
		protected internal bool IsContainer {
			get { return isContainerCore || isDocumentsHostCore; }
		}
		protected internal bool SupportSideDock {
			get { return !IsContainer; }
		}
		internal void MarkAsNonMDI() {
			isNonMdiCore = true;
		}
		internal void MarkAsDockPanel() {
			isDockPanelCore = true;
		}
		internal void MarkAsDocumentsHost() {
			isDocumentsHostCore = true;
		}
		internal bool CanFloatOnDoubleClick() {
			return CanFloatOnDoubleClick(GetDockPanel()) && CanFloatOnDoubleClickCore();
		}
		protected bool CanFloatOnDoubleClick(Docking.DockPanel panel) {
			if(panel == null || panel.DockManager == null) return true;
			return CanFloatOnDoubleClick(panel.DockManager, panel);
		}
		bool CanFloatOnDoubleClick(Docking.DockManager dockManager, Docking.DockPanel panel) {
			return dockManager.DockingOptions.FloatOnDblClick && panel.Options.AllowFloating && panel.Options.FloatOnDblClick;
		}
		protected virtual bool CanFloatOnDoubleClickCore() {
			return IsEnabled;
		}
		protected void InitializePropertiesFromDockPanel() {
			Docking.DockPanel panel = GetDockPanel();
			if(panel != null) 
				InitializePropertiesFromDockPanel(panel);
		}
		protected void UpdateDockPanelProperties(PropertyChangedEventArgs e) {
			Docking.DockPanel panel = GetDockPanel();
			if(panel != null && e != null)
				UpdateDockPanekProperties(e, panel);
		}
		protected internal void InitializePropertiesFromDockPanel(Docking.DockPanel panel) {
			Properties.BeginUpdate();
			InitializePropertiesFromDockPanelCore(panel);
			Properties.CancelUpdate();
		}
		protected virtual void InitializePropertiesFromDockPanelCore(Docking.DockPanel panel) {
			Properties.AllowFloat = CanFloat(panel) ? DefaultBoolean.True : DefaultBoolean.False;
			Properties.AllowClose = CanClose(panel) ? DefaultBoolean.True : DefaultBoolean.False;
			if(string.IsNullOrEmpty(controlNameCore) && !string.IsNullOrEmpty(panel.Name)) {
				controlNameCore = panel.Name;
			}
		}
		protected void UpdateDockPanekProperties(PropertyChangedEventArgs e, Docking.DockPanel panel) {
			panel.Options.BeginUpdate();
			UpdateDockPanelPropertiesCore(e, panel);
			panel.Options.CancelUpdate();
		}
		protected virtual void UpdateDockPanelPropertiesCore(PropertyChangedEventArgs e, Docking.DockPanel dockPanel) {
			if(e.PropertyName == "AllowClose")
				dockPanel.Options.ShowCloseButton = Properties.AllowClose != DefaultBoolean.False;
			if(e.PropertyName == "AllowFloat")
				dockPanel.Options.AllowFloating = Properties.AllowFloat != DefaultBoolean.False;
		}
		protected bool CanFloat(Docking.DockPanel panel) {
			return (panel != null) && panel.Options.AllowFloating;
		}
		protected bool CanClose(Docking.DockPanel panel) {
			if(panel == null) return true;
			return CanClose(panel.DockManager, panel);
		}
		bool CanClose(Docking.DockManager dockManager, Docking.DockPanel panel) {
			if(dockManager == null)
				return panel.Options.ShowCloseButton;
			return dockManager.DockingOptions.ShowCloseButton && panel.Options.ShowCloseButton;
		}
		List<BaseDocument> children;
		internal void AddChild(BaseDocument child) {
			if(!isContainerCore) {
				isContainerCore = true;
				UnsubscribeControl();
				controlCore = null;
				formCore = null;
				isFloatingCore = false;
				children = new List<BaseDocument>();
			}
			children.Add(child);
		}
		internal BaseDocument[] GetChildren() {
			return IsContainer ? children.ToArray() : new BaseDocument[] { };
		}
		internal DocumentContainer ContainerControl {
			get { return Control != null ? Control.Parent as DocumentContainer : null; }
		}
		internal BaseDocument SetIsActive(bool value) {
			if(isActiveCore == value) 
				return this;
			isActiveCore = value;
			OnIsActiveChanged();
			return this;
		}
		internal void MarkAsDeferredControlLoad() {
			isDeferredControlLoadCore = true;
		}
		internal void SetIsFloating(bool value) {
			if(isFloatingCore == value) return;
			isFloatingCore = value;
			OnIsFloatingChanged();
		}
		internal void SetManager(DocumentManager manager) {
			EnsureFormManager(manager);
			if(managerCore == manager) return;
			managerCore = manager;
			OnManagerChanged();
		}
		protected virtual void InitializeFromForm() {
			bool useFormImage = Properties.CanUseFormIconAsDocumentImage;
			if(IsDockPanel) {
				Docking.FloatForm fForm = (Docking.FloatForm)Form;
				if(string.IsNullOrEmpty(captionCore))
					captionCore = fForm.FloatLayout.Panel.Text;
				if(imageCore == null && useFormImage)
					imageCore = fForm.FloatLayout.Image;
			}
			else {
				if(string.IsNullOrEmpty(captionCore))
					captionCore = InitializeCaptionFromForm();
				if(imageCore == null && useFormImage) {
					Icon icon = Form.Icon;
					if(icon != null)
						imageCore = ImageFromIcon(icon);
				}
			}
		}
		static System.Reflection.FieldInfo fi_defaultIcon, fi_smallIcon;
		Image ImageFromIcon(Icon formIcon) {
			if(fi_defaultIcon == null) {
				fi_defaultIcon = typeof(Form).GetField("defaultIcon",
					System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
			}
			if(fi_smallIcon == null) {
				fi_smallIcon = typeof(Form).GetField("smallIcon",
					System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
			}
			object defaultIcon = fi_defaultIcon.GetValue(null);
			if(object.Equals(formIcon, defaultIcon)) {
				Icon smallIcon = fi_smallIcon.GetValue(Form) as Icon;
				if(smallIcon != null)
					return smallIcon.ToBitmap();
				using(smallIcon = new Icon(formIcon, SystemInformation.SmallIconSize)) {
					return smallIcon.ToBitmap();
				}
			}
			return formIcon.ToBitmap();
		}
		protected virtual string InitializeCaptionFromForm() {
			return Control.Text;
		}
		protected virtual void OnIsActiveChanged() { }
		protected virtual void OnIsFloatingChanged() { }
		protected virtual void OnManagerChanged() { }
		protected void EnsureFormManager(DocumentManager manager) {
			FloatDocumentForm fForm = formCore as FloatDocumentForm;
			if(fForm != null) fForm.SetManager(manager);
			DocumentContainer container = (controlCore != null)
				? Control.Parent as DocumentContainer : null;
			if(container != null) container.SetManager(manager);
		}
		protected virtual void OnCaptionChanged() {
			if(IsLayoutChangeRestricted) return;
			FloatDocumentForm fForm = formCore as FloatDocumentForm;
			if(fForm != null) {
				fForm.Text = Caption;
				fForm.InvalidateNC();
			}
			DocumentContainer container = (controlCore != null)
				? Control.Parent as DocumentContainer : null;
			if(container != null)
				container.InvalidateNC();
			LayoutChanged();
		}
		internal static BaseDocument GetDocument(IBaseElementInfo info) {
			IBaseDocumentInfo documentInfo = info as IBaseDocumentInfo;
			return (documentInfo != null) ? documentInfo.BaseDocument : null;
		}
		protected internal void UpdateStyle() {
			FloatDocumentForm fForm = formCore as FloatDocumentForm;
			if(fForm != null) fForm.UpdateStyle();
			DocumentContainer container = (controlCore != null)
				? Control.Parent as DocumentContainer : null;
			if(container != null) container.UpdateStyle();
		}
		protected internal virtual Image GetActualImage() {
			if(ImageUri != null && ImageUri.HasImage)
				return ImageUri.GetImage();
			return Image;
		}
		protected internal virtual Image GetImageForAnimation() {
			if(ImageUri != null && ImageUri.HasImage)
				return null;
			return Image;
		}
		protected internal Rectangle GetRestoreBounds() {
			Docking.DockPanel panel = GetDockPanel();
			return (panel != null) ? new Rectangle(panel.FloatLocation, panel.FloatSize) : Form.Bounds;
		}
		protected internal object GetID() {
			Docking.DockPanel panel = GetDockPanel();
			return (panel != null) ? (object)panel.ID : (object)Form.Handle;
		}
		protected internal void RestoreBounds(Rectangle size) {
			Docking.DockPanel panel = GetDockPanel();
			if(panel != null) {
				panel.FloatLocation = size.Location;
				panel.FloatSize = size.Size;
			}
			else Form.Bounds = size;
		}
		protected internal void EnsureFloatingBounds() {
			if(IsFloating && Form != null) {
				floatSizeCore = Form.Size;
				floatLocationCore = Form.Location;
				var settings = BaseDocumentSettings.GetProvider<IBaseDocumentSettings>(Control);
				if(settings != null) {
					settings.FloatSize = FloatSize;
					settings.FloatLocation = FloatLocation;
				}
			}
		}
		[ToolboxItem(false)]
		class DesignModeContent : Panel {
			static int currentColor;
			static Color[] forecolors = new Color[] { 
				Color.Red, Color.Blue, Color.Green
			};
			SolidBrush sb;
			Pen pen;
			StringFormat sf;
			static Font font = Views.WindowsUI.SegoeUIFontsCache.GetFont("Segoe UI", 24f);
			public DesignModeContent() {
				sf = new StringFormat()
				{
					Alignment = StringAlignment.Center,
					LineAlignment = StringAlignment.Center,
					Trimming = StringTrimming.EllipsisCharacter,
					FormatFlags = StringFormatFlags.NoWrap
				};
				DoubleBuffered = true;
				ForeColor = forecolors[(currentColor++) % forecolors.Length];
				SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
				SetStyle(ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
			}
			protected override void Dispose(bool disposing) {
				if(disposing) {
					Ref.Dispose(ref sb);
					Ref.Dispose(ref pen);
					Ref.Dispose(ref sf);
				}
				base.Dispose(disposing);
			}
			protected override void OnPaint(PaintEventArgs e) {
				if(sb == null)
					sb = new SolidBrush(Color.FromArgb(100, ForeColor));
				if(pen == null)
					pen = new Pen(Color.FromArgb(200, 221, 223, 223));
				e.Graphics.FillRectangle(sb, ClientRectangle);
				e.Graphics.DrawString(Text, font, sb, ClientRectangle, sf);
			}
		}
		protected internal bool DoValidate() {
			if(IsControlLoaded) {
				ContainerControl container = controlCore as ContainerControl;
				if(container == null && controlCore != null)
					container = controlCore.GetContainerControl() as ContainerControl;
				if(container != null)
					return container.Validate() || AllowFocusChangeOnValidation(container);
			}
			return true;
		}
		bool AllowFocusChangeOnValidation(ContainerControl container) {
			return (container.AutoValidate != AutoValidate.EnablePreventFocusChange);
		}
		#region ReleaseDockPanel
		internal class ReleaseDockPanelContext : IDisposable {
			Docking.DockPanel panel;
			Form form;
			public ReleaseDockPanelContext(BaseDocument document) {
				panel = document.GetDockPanel();
				form = document.Form;
				if(CanRelease)
					panel.DockLayout.SavedInfo.SaveSettings(panel.DockLayout);
			}
			bool CanRelease { get { return panel != null && panel.DockLayout != null; } }
			public void Dispose() {
				if(CanRelease) {
					panel.dockedAsTabbedDocumentCore = false;
					if(!form.IsDisposed) {
						if(panel.DockLayout != null)
							panel.DockLayout.LayoutChanged();
						form.Visible = true;
					}
				}
				form = null;
				panel = null;
			}
		}
		internal ReleaseDockPanelContext ReleaseDockPanel() {
			return new ReleaseDockPanelContext(this);
		}
		bool CanReleaseDockPanel { get { return IsDockPanel && Control != null; } }
		#endregion ReleaseDockPanel
		#region ILogicalOwner Members
		public IEnumerable<Component> GetLogicalChildren() {
			return new Component[] { Control, Form };
		}
		#endregion
		#region IXtraSerializableChildren Members
		string IXtraSerializableChildren.Name {
			get { return string.IsNullOrEmpty(ControlName) && Control != null ? Control.Name : ControlName; }
		}
		#endregion
		#region ICaptionAppearanceProvider Members
		AppearanceObject IDocumentCaptionAppearanceProvider.ActiveCaptionAppearance {
			get { return GetActiveDocumentCaptionAppearance(); }
		}
		AppearanceObject IDocumentCaptionAppearanceProvider.CaptionAppearance {
			get { return GetDocumentCaptionAppearance(); }
		}
		bool IDocumentCaptionAppearanceProvider.AllowCaptionColorBlending { get { return CanBlendCaptionColor(); } }
		protected virtual bool CanBlendCaptionColor() { return true; }
		#endregion
		protected virtual AppearanceObject GetActiveDocumentCaptionAppearance() {
			return new AppearanceObject();
		}
		protected virtual AppearanceObject GetDocumentCaptionAppearance() {
			return new AppearanceObject();
		}
		#region Change Container
		internal IDisposable TrackContainerChange(IContainer container) {
			return new ChangeContainerContext(this, container);
		}
		internal static bool IsInContainerChange(BaseDocument baseDocument) {
			return ChangeContainerContext.IsInContainerChange(baseDocument);
		}
		sealed class ChangeContainerContext : IDisposable {
			#region static
			static IDictionary<BaseDocument, ChangeContainerContext> contexts = new Dictionary<BaseDocument, ChangeContainerContext>();
			internal static bool IsInContainerChange(BaseDocument document) {
				return (document != null) && contexts.ContainsKey(document);
			}
			#endregion
			BaseDocument document;
			IContainer container;
			bool isContainerChange;
			string siteName;
			internal ChangeContainerContext(BaseDocument document, IContainer container) {
				this.document = document;
				this.container = container;
				contexts.Add(document, this);
				isContainerChange = (document.Container != container);
				if(isContainerChange) {
					if(document.Site != null)
						siteName = document.Site.Name;
					if(document.Container != null)
						document.Container.Remove(document);
				}
			}
			void IDisposable.Dispose() {
				if(isContainerChange) {
					if(container != null)
						container.Add(document);
					if(!string.IsNullOrEmpty(siteName) && document.Site != null) {
						try { document.Site.Name = siteName; }
						catch { }
					}
				}
				contexts.Remove(document);
				this.container = null;
				this.document = null;
			}
		}
		#endregion Change Container
	}
}
namespace DevExpress.XtraBars.MVVM.Services {
	using DevExpress.Utils.MVVM.Services;
	using DevExpress.XtraBars.Docking2010.Views;
	class BaseDocumentAdapter : IDocumentAdapter, IDocumentContentProvider {
		BaseDocument hostDocument;
		BaseView hostView;
		bool addAsFloat;
		public BaseDocumentAdapter(BaseView hostView, bool addAsFloat = false) {
			this.hostView = hostView;
			this.addAsFloat = addAsFloat;
			hostView.DocumentClosing += hostView_DocumentClosing;
			hostView.DocumentClosed += hostView_DocumentClosed;
		}
		public void Dispose() {
			hostView.DocumentClosing -= hostView_DocumentClosing;
			hostView.DocumentClosed -= hostView_DocumentClosed;
			hostDocument = null;
		}
		void hostView_DocumentClosing(object sender, DocumentCancelEventArgs e) {
			if(e.Document == hostDocument)
				RaiseClosing(e);
		}
		void hostView_DocumentClosed(object sender, DocumentEventArgs e) {
			if(e.Document == hostDocument) {
				RaiseClosed(e);
				Dispose();
			}
		}
		void RaiseClosed(DocumentEventArgs e) {
			if(Closed != null) Closed(hostView, e);
		}
		void RaiseClosing(DocumentCancelEventArgs e) {
			if(Closing != null) Closing(hostView, e);
		}
		public void Show(Control control) {
			if(hostView.Manager.GetDocument(control) == null)
				hostDocument = addAsFloat ? hostView.AddFloatDocument(control) : hostView.AddDocument(control);
			hostView.ActivateDocument(control);
		}
		public void Close(Control control, bool force = true) {
			if(force)
				hostView.DocumentClosing -= hostView_DocumentClosing;
			hostView.Controller.Close(hostDocument);
		}
		public event EventHandler Closed;
		public event CancelEventHandler Closing;
		public bool CanAddContent {
			get { return hostView.HasQueryControlSubscription; }
		}
		public void AddContent(string caption, string viewName) {
			hostDocument = hostView.AddDocument(caption, viewName);
		}
		public object Resolve(string name, params object[] parameters) {
			if(hostDocument == null) return null;
			return hostDocument.EnsureIsBoundToControl(hostView) ? hostDocument.Control : null;
		}
	}
}
