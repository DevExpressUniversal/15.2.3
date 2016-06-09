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
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraBars.Docking2010.Base;
using DevExpress.XtraBars.Docking2010.Customization;
namespace DevExpress.XtraBars.Docking2010.Views.Widget {
	public class WidgetView : BaseView {
		StackGroupProperties stackGroupPropertiesCore;
		StackGroupCollection stackGroupsCore;
		Orientation orientationCore;
		TableGroup tableGroupCore;
		FlowLayoutGroup flowLayoutGroupCore;
		FlowLayoutProperties flowLayoutPropertiesCore;
		DocumentAnimationProperties documentAnimationPropertiesCore;
		public WidgetView() : this(null) { }
		public WidgetView(IContainer container)
			: base(container) {
		}
		protected override void OnCreate() {
			base.OnCreate();
			orientationCore = Orientation.Vertical;
			layoutModeCore = Widget.LayoutMode.StackLayout;
			allowDocumentCaptionColorBlendingCore = true;
			stackGroupsCore = CreateStackGroups();
			StackGroups.CollectionChanged += OnStackGroupsCollectionChanged;
			stackGroupPropertiesCore = CreateStackGroupProperties();
			StackGroupProperties.Changed += OnStackGroupPropertiesChanged;
			tableGroupCore = CreateTableGroup();
			flowLayoutGroupCore = CreateFlowLayoutGroup();
			flowLayoutPropertiesCore = CreateFlowLayoutProperties(flowLayoutGroupCore);
			documentAnimationPropertiesCore = CreateDocumentAnimationProperties();
			allowDragDropWobbleAnimationCore = DefaultBoolean.Default;
		}
		protected virtual DocumentAnimationProperties CreateDocumentAnimationProperties() {
			return new DocumentAnimationProperties();
		}
		protected virtual FlowLayoutProperties CreateFlowLayoutProperties(FlowLayoutGroup flowLayoutGroup) {
			return new Widget.FlowLayoutProperties(flowLayoutGroup);
		}
		protected virtual FlowLayoutGroup CreateFlowLayoutGroup() {
			return new FlowLayoutGroup(this);
		}
		protected virtual TableGroup CreateTableGroup() {
			return new TableGroup(this);
		}
		protected internal TableGroup TableGroup {
			get { return tableGroupCore; }
		}
		protected internal FlowLayoutGroup FlowLayoutGroup {
			get { return flowLayoutGroupCore; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never)]
		public IDocumentGroup DocumentGroup {
			get {
				if(LayoutMode == LayoutMode.FlowLayout)
					return FlowLayoutGroup;
				if(LayoutMode == LayoutMode.TableLayout)
					return TableGroup;
				return null;
			}
		}
		protected override void LockComponentBeforeDisposing() {
			base.LockComponentBeforeDisposing();
			StackGroups.CollectionChanged -= OnStackGroupsCollectionChanged;
			StackGroupProperties.Changed -= OnStackGroupPropertiesChanged;
		}
		protected override void OnDispose() {
			Ref.Dispose(ref stackGroupsCore);
			Ref.Dispose(ref stackGroupPropertiesCore);
			Ref.Dispose(ref tableGroupCore);
			flowLayoutPropertiesCore = null;
			base.OnDispose();
		}
		void OnStackGroupPropertiesChanged(object sender, EventArgs e) {
			using(LockPainting()) {
				RequestInvokePatchActiveChild();
			}
		}
		protected override void OnLayoutChanged() {
			base.OnLayoutChanged();
			if(Manager != null && Manager.GetOwnerControl() is WidgetsHost)
				(Manager.GetOwnerControl() as WidgetsHost).LayoutChanged();
		}
		protected override void UpdateStyleCore() {
			base.UpdateStyleCore();
			foreach(var item in Documents) {
				item.UpdateStyle();
			}
		}
		protected override bool UseFloatingDocumentsHost {
			get { return false; }
		}
		protected internal override bool CanUseLoadingIndicator() {
			return UseLoadingIndicator == DefaultBoolean.True && (Site == null || !Site.DesignMode);
		}
		protected internal override bool CanShowOverlapWarning() {
			bool result = base.CanShowOverlapWarning();
			if(LayoutMode == LayoutMode.StackLayout)
				return result && StackGroups.Count == 0;
			if(LayoutMode == LayoutMode.TableLayout)
				return result && Columns.Count == 0 && Rows.Count == 0;
			return result;
		}
		void OnStackGroupsCollectionChanged(DevExpress.XtraBars.Docking2010.Base.CollectionChangedEventArgs<StackGroup> ea) {
			StackGroup group = ea.Element;
			if(ea.ChangedType == CollectionChangedType.ElementAdded) {
				OnStackGroupAdded(group);
				AddToContainer(group);
			}
			if(ea.ChangedType == CollectionChangedType.ElementRemoved) {
				OnStackGroupRemoved(group);
				RemoveFromContainer(group);
			}
			LayoutChanged();
			if(Manager == null) return;
			SetLayoutModified();
			Manager.LayoutChanged();
		}
		protected internal override bool OnSCMaximize(IntPtr hWnd) {
			if(Documents.GetDocument(hWnd) != null)
				return base.OnSCMaximize(hWnd);
			return false;
		}
		protected internal override bool OnSCRestore(IntPtr hWnd) {
			if(Documents.GetDocument(hWnd) != null) {
				return true;
			}
			return false;
		}
		static readonly object beginStackGroupDragging = new object();
		static readonly object endStackGroupDragging = new object();
		static readonly object beginDragging = new object();
		static readonly object dragging = new object();
		static readonly object endDragging = new object();
		static readonly object queryMaximizedControl = new object();
		static readonly object maximizedControlLoaded = new object();
		static readonly object maximizedControlReleasing = new object();
		static readonly object maximizedControlReleased = new object();
		[Category("Layout")]
		public event StackGroupDraggingEventHandler BeginStackGroupDragging {
			add { Events.AddHandler(beginStackGroupDragging, value); }
			remove { Events.RemoveHandler(beginStackGroupDragging, value); }
		}
		[Category("Layout")]
		public event StackGroupDraggingEventHandler EndStackGroupDragging {
			add { Events.AddHandler(endStackGroupDragging, value); }
			remove { Events.RemoveHandler(endStackGroupDragging, value); }
		}
		[Category("Layout")]
		public event EventHandler BeginDragging {
			add { Events.AddHandler(beginDragging, value); }
			remove { Events.RemoveHandler(beginDragging, value); }
		}
		[Category("Layout")]
		public event MouseEventHandler Dragging {
			add { Events.AddHandler(dragging, value); }
			remove { Events.RemoveHandler(dragging, value); }
		}
		[Category("Layout")]
		public event EventHandler EndDragging {
			add { Events.AddHandler(endDragging, value); }
			remove { Events.RemoveHandler(endDragging, value); }
		}
		protected internal void RaiseBeginDragging(Document document) {
			EventHandler handler = (EventHandler)Events[beginDragging];
			if(handler != null)
				handler(document, EventArgs.Empty);
		}
		protected internal void RaiseDragging(Document document, MouseEventArgs args) {
			MouseEventHandler handler = (MouseEventHandler)Events[dragging];
			if(handler != null)
				handler(document, args);
		}
		protected internal void RaiseEndDragging(Document document) {
			EventHandler handler = (EventHandler)Events[endDragging];
			if(handler != null)
				handler(document, EventArgs.Empty);
		}
		protected internal bool RaiseBeginStackGroupDragging(StackGroup group) {
			StackGroupDraggingEventHandler handler = (StackGroupDraggingEventHandler)Events[beginStackGroupDragging];
			StackGroupDraggingEventArgs args = new StackGroupDraggingEventArgs(group, null);
			if(handler != null)
				handler(this, args);
			return !args.Cancel;
		}
		protected internal bool RaiseEndStackGroupDragging(StackGroup group, StackGroup targetGroup) {
			StackGroupDraggingEventHandler handler = (StackGroupDraggingEventHandler)Events[endStackGroupDragging];
			StackGroupDraggingEventArgs args = new StackGroupDraggingEventArgs(group, targetGroup);
			if(handler != null)
				handler(this, args);
			return !args.Cancel;
		}
		[Category("Deferred Control Load Events")]
		public event QueryControlEventHandler QueryMaximizedControl {
			add { Events.AddHandler(queryMaximizedControl, value); }
			remove { Events.RemoveHandler(queryMaximizedControl, value); }
		}
		[Category("Deferred Control Load Events")]
		public event DeferredControlLoadEventHandler MaximizedControlLoaded {
			add { Events.AddHandler(maximizedControlLoaded, value); }
			remove { Events.RemoveHandler(maximizedControlLoaded, value); }
		}
		[Category("Deferred Control Load Events")]
		public event ControlReleasingEventHandler MaximizedControlReleasing {
			add { Events.AddHandler(maximizedControlReleasing, value); }
			remove { Events.RemoveHandler(maximizedControlReleasing, value); }
		}
		[Category("Deferred Control Load Events")]
		public event DeferredControlLoadEventHandler MaximizedControlReleased {
			add { Events.AddHandler(maximizedControlReleased, value); }
			remove { Events.RemoveHandler(maximizedControlReleased, value); }
		}
		protected internal Control RaiseQueryMaximizedControl(BaseDocument document) {
			QueryControlEventHandler handler = (QueryControlEventHandler)Events[queryMaximizedControl];
			QueryControlEventArgs ea = new QueryControlEventArgs(document);
			if(handler != null)
				handler(this, ea);
			return ea.Control;
		}
		protected internal void RaiseMaximizedControlLoaded(BaseDocument document, Control control) {
			DeferredControlLoadEventHandler handler = (DeferredControlLoadEventHandler)Events[maximizedControlLoaded];
			if(handler != null)
				handler(this, new DeferredControlLoadEventArgs(document, control));
		}
		protected internal bool RaiseMaximizedControlReleasing(BaseDocument document, out bool disposeControl) {
			return RaiseControlReleasing(document, true, true, out disposeControl);
		}
		protected internal bool RaiseMaximizedControlReleasing(BaseDocument document, bool keepControl, bool disposeControl, out bool disposeControlResult) {
			ControlReleasingEventHandler handler = (ControlReleasingEventHandler)Events[maximizedControlReleasing];
			ControlReleasingEventArgs e = new ControlReleasingEventArgs(document, keepControl, disposeControl);
			if(handler != null)
				handler(this, e);
			disposeControlResult = e.DisposeControl;
			return !e.Cancel;
		}
		protected void RaiseMaximizedControlReleased(BaseDocument document, Control control) {
			DeferredControlLoadEventHandler handler = (DeferredControlLoadEventHandler)Events[maximizedControlReleased];
			if(handler != null)
				handler(this, new DeferredControlLoadEventArgs(document, control));
		}
		public void ReleaseDeferredLoadMaximizedControl(Document document) {
			ReleaseDeferredLoadMaximizedControl(document, true, true);
		}
		public void ReleaseDeferredLoadMaximizedControl(Document document, bool keepControl) {
			ReleaseDeferredLoadMaximizedControl(document, keepControl, true);
		}
		public void ReleaseDeferredLoadMaximizedControl(Document document, bool keepControl, bool disposeControl) {
			using(LockPainting()) {
				document.ReleaseDeferredLoadMaximizedControl(this, keepControl, disposeControl);
				RequestInvokePatchActiveChild();
			}
		}
		public void ReleaseDeferredLoadMaximizedControls() {
			ReleaseDeferredLoadControls(true, true);
		}
		public void ReleaseDeferredLoadMaximizedControls(bool keepControls) {
			ReleaseDeferredLoadControls(keepControls, true);
		}
		public void ReleaseDeferredLoadMaximizedControls(bool keepControls, bool disposeControls) {
			using(BatchUpdate.Enter(Manager, true)) {
				using(LockPainting()) {
					foreach(Document document in Documents)
						document.ReleaseDeferredLoadMaximizedControl(this, keepControls, disposeControls);
					RequestInvokePatchActiveChild();
				}
			}
		}
		protected override void RegisterInfos() {
			if(IsDisposing) return;
			foreach(StackGroup group in StackGroups) {
				RegisterStackGroupInfo(group);
			}
			foreach(Document document in Documents) {
				RegisterDocumentInfo(document);
			}
		}
		protected override void UnregisterInfos() {
			if(IsDisposing) return;
			foreach(StackGroup group in StackGroups) {
				UnregisterStackGroupInfo(group);
			}
			foreach(Document document in Documents) {
				UnregisterDocumentInfo(document);
			}
		}
		protected virtual void OnStackGroupAdded(StackGroup group) {
			group.Properties.EnsureParentProperties(StackGroupProperties);
			group.SetManager(Manager);
			group.SetParent(this);
			group.LengthChanged += GroupLengthChanged;
			RegisterStackGroupInfo(group);
			InvalidateUIView();
		}
		void GroupLengthChanged(object sender, EventArgs e) {
			LayoutChanged();
		}
		protected virtual void OnStackGroupRemoved(StackGroup group) {
			UnregisterStackGroupInfo(group);
			InvalidateUIView();
			group.SetManager(null);
			group.SetParent(null);
			group.LengthChanged -= GroupLengthChanged;
			group = null;
		}
		protected internal void RegisterStackGroupInfo(StackGroup group) {
			if(ViewInfo != null)
				((WidgetViewInfo)ViewInfo).RegisterInfo(group);
		}
		protected internal void UnregisterStackGroupInfo(StackGroup group) {
			if(ViewInfo != null)
				((WidgetViewInfo)ViewInfo).UnregisterInfo(group);
		}
		protected internal void UnregisterDocumentInfo(Document document) {
			if(ViewInfo != null)
				((WidgetViewInfo)ViewInfo).UnregisterInfo(document);
		}
		protected internal void RegisterDocumentInfo(Document document) {
			if(ViewInfo != null)
				((WidgetViewInfo)ViewInfo).RegisterInfo(document);
		}
		protected virtual StackGroupCollection CreateStackGroups() {
			return new StackGroupCollection();
		}
		protected virtual StackGroupProperties CreateStackGroupProperties() {
			return new StackGroupProperties();
		}
		LayoutMode layoutModeCore;
		[XtraSerializableProperty,
#if !SL
	DevExpressXtraBarsLocalizedDescription("WidgetViewLayoutMode"),
#endif
		Category("Behavior"),
		DefaultValue(LayoutMode.StackLayout)]
		public LayoutMode LayoutMode {
			get { return layoutModeCore; }
			set { SetValue(ref layoutModeCore, value, OnLayoutModeChanged); }
		}
		void OnLayoutModeChanged() {
			PopulateTableGroupItems();
			if(Manager == null) return;
			var hostControl = Manager.GetOwnerControl() as WidgetsHost;
			if(hostControl != null) {
				hostControl.UpdateHandler();
			}
			LayoutChanged();
		}
		[ListBindable(false), TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter)),
#if !SL
	DevExpressXtraBarsLocalizedDescription("WidgetViewRows"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, true, true, 1),
		Category("Layout"),
		Editor("DevExpress.Utils.Design.DXCollectionEditorBase, " + AssemblyInfo.SRAssemblyDesign, typeof(System.Drawing.Design.UITypeEditor))]
		public RowDefinitionCollection Rows { get { return tableGroupCore != null ? tableGroupCore.Rows : null; } }
		[ListBindable(false), TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter)),
#if !SL
	DevExpressXtraBarsLocalizedDescription("WidgetViewColumns"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, true, true, 1),
		Category("Layout"),
		Editor("DevExpress.Utils.Design.DXCollectionEditorBase, " + AssemblyInfo.SRAssemblyDesign, typeof(System.Drawing.Design.UITypeEditor))]
		public ColumnDefinitionCollection Columns { get { return tableGroupCore != null ? tableGroupCore.Columns : null; } }
#if !SL
	[DevExpressXtraBarsLocalizedDescription("WidgetViewDocumentProperties")]
#endif
		[Category("Layout"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public new IDocumentProperties DocumentProperties {
			get { return base.DocumentProperties as IDocumentProperties; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("WidgetViewStackGroupProperties"),
#endif
		ListBindable(false), XtraSerializableProperty(XtraSerializationVisibility.Content),
		Category("Layout"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public IStackGroupProperties StackGroupProperties {
			get { return stackGroupPropertiesCore; }
		}
		[ListBindable(false), TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter)),
#if !SL
	DevExpressXtraBarsLocalizedDescription("WidgetViewStackGroups"),
#endif
		Category("Layout"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.Utils.Design.DXCollectionEditorBase, " + AssemblyInfo.SRAssemblyDesign, typeof(System.Drawing.Design.UITypeEditor))]
		public StackGroupCollection StackGroups {
			get { return stackGroupsCore; }
		}
		int documentSpacingCore;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("WidgetViewDocumentSpacing"),
#endif
		DefaultValue(0), Category("Layout"), XtraSerializableProperty]
		public int DocumentSpacing {
			get { return documentSpacingCore; }
			set { SetValue(ref documentSpacingCore, value); }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("WidgetViewFlowLayoutProperties"),
#endif
		Category("Behavior"), XtraSerializableProperty(XtraSerializationVisibility.Content),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FlowLayoutProperties FlowLayoutProperties {
			get { return flowLayoutPropertiesCore; }
		}
		public bool ShouldSerializeFlowLayoutProperties() {
			return flowLayoutPropertiesCore != null && flowLayoutPropertiesCore.ShouldSerialize();
		}
		public void ResetFlowLayoutProperties() {
			flowLayoutPropertiesCore.Reset();
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("WidgetViewDocumentAnimationProperties"),
#endif
		Category("Behavior"), XtraSerializableProperty(XtraSerializationVisibility.Content),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DocumentAnimationProperties DocumentAnimationProperties {
			get { return documentAnimationPropertiesCore; }
		}
		public bool ShouldSerializeDocumentAnimationProperties() {
			return documentAnimationPropertiesCore != null && DocumentAnimationProperties.ShouldSerialize();
		}
		public void ResetDocumentAnimationProperties() {
			DocumentAnimationProperties.Reset();
		}
		DefaultBoolean allowDragDropWobbleAnimationCore;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("WidgetViewAllowDragDropWobbleAnimation"),
#endif
		DefaultValue(DefaultBoolean.Default), Category("Behavior"), XtraSerializableProperty]
		public DefaultBoolean AllowDragDropWobbleAnimation {
			get { return allowDragDropWobbleAnimationCore; }
			set { SetValue(ref allowDragDropWobbleAnimationCore, value); }
		}
		DefaultBoolean allowDocumentStateChangeAnimationCore = DefaultBoolean.Default;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("WidgetViewAllowDocumentStateChangeAnimation"),
#endif
		Category("Behavior"), DefaultValue(DefaultBoolean.Default), XtraSerializableProperty]
		public virtual DefaultBoolean AllowDocumentStateChangeAnimation {
			get { return allowDocumentStateChangeAnimationCore; }
			set { allowDocumentStateChangeAnimationCore = value; }
		}
		DefaultBoolean allowStartupAnimationCore = DefaultBoolean.Default;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("WidgetViewAllowStartupAnimation"),
#endif
		Category("Behavior"), DefaultValue(DefaultBoolean.Default), XtraSerializableProperty]
		public virtual DefaultBoolean AllowStartupAnimation {
			get { return allowStartupAnimationCore; }
			set { allowStartupAnimationCore = value; }
		}
		DefaultBoolean allowResizeAnimationCore = DefaultBoolean.Default;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("WidgetViewAllowResizeAnimation"),
#endif
		Category("Behavior"), DefaultValue(DefaultBoolean.Default), XtraSerializableProperty]
		public virtual DefaultBoolean AllowResizeAnimation {
			get { return allowResizeAnimationCore; }
			set { allowResizeAnimationCore = value; }
		}
		[DefaultValue(Orientation.Vertical), Category("Layout"), XtraSerializableProperty, 
#if !SL
	DevExpressXtraBarsLocalizedDescription("WidgetViewOrientation")
#else
	Description("")
#endif
]
		public Orientation Orientation {
			get { return orientationCore; }
			set { SetValue(ref orientationCore, value, OnOrientationChanged); }
		}
		void OnOrientationChanged() {
			if(WidgetViewInfo != null) {
				((IWidgetViewControllerInternal)Controller).OnOrientationChanged();
			}
			if(Manager != null)
				Manager.InvokePatchActiveChildren();
			LayoutChanged();
		}
		public void ScrollToActiveDocument(Document document){
			var widgetHost = Manager.GetOwnerControl() as XtraEditors.XtraScrollableControl;
			var control =  Manager.GetChild(document);
			if(widgetHost != null && control != null) {
				widgetHost.ScrollControlIntoView(control);
			}
		}
		public void SuspendLayout() {
			BeginUpdate();
			if(Manager == null) return;
			var hostControl = Manager.GetOwnerControl() as WidgetsHost;
			if(hostControl != null) {
				hostControl.LockUpdateLayout();
			}
		}
		public void ResumeLayout() {
			EndUpdate();
			if(Manager == null) return;
			var hostControl = Manager.GetOwnerControl() as WidgetsHost;
			if(hostControl != null) {
				hostControl.UnlockUpdateLayout();
			}
		}
		public void BeginUpdateAnimation() {
			SuspendLayout();
		}
		public void EndUpdateAnimation() {
			if(LayoutMode == Widget.LayoutMode.FlowLayout && FlowLayoutGroup.Info != null) {
				FlowLayoutGroup.Info.AddAnimation();
			}
			if(LayoutMode == Widget.LayoutMode.TableLayout && TableGroup.Info != null) {
				TableGroup.Info.AddAnimation();
			}
			if(LayoutMode == Widget.LayoutMode.StackLayout) {
				foreach(StackGroup group in StackGroups) {
					if(group.Info != null)
						group.Info.AddAnimation();
				}
			}
			ResumeLayout();
		}
		internal WidgetViewInfo WidgetViewInfo { get { return (WidgetViewInfo)ViewInfo; } }
		public sealed override ViewType Type {
			get { return ViewType.Widget; }
		}
		protected sealed internal override Type GetUIElementKey() {
			return typeof(WidgetView);
		}
		[Browsable(false)]
		public new IWidgetViewController Controller {
			get { return base.Controller as IWidgetViewController; }
		}
		protected override IBaseViewController CreateController() {
			return new WidgetViewController(this);
		}
		protected override IBaseDocumentProperties CreateDocumentProperties() {
			return new DocumentProperties();
		}
		protected internal override bool AllowMdiLayout {
			get { return false; }
		}
		protected internal override bool AllowMdiSystemMenu {
			get { return false; }
		}
		protected internal override void PatchActiveChildren(Point offset) {
			var ownerControl = Manager.GetOwnerControl();
			for(int i = 0; i < Documents.Count; i++) {
				Document document = Documents[i] as Document;
				bool prev = document.IsControlLoadedByQueryControl;
				document.EnsureIsBoundToControl(this);
			}
			if(ownerControl is WidgetsHost) {
				(ownerControl as WidgetsHost).LayoutChanged();
			}
		}
		protected internal virtual StackGroup CreateStackGroup() {
			return new StackGroup(StackGroupProperties);
		}
		protected internal override void PatchBeforeActivateChild(System.Windows.Forms.Control activatedChild, Point offset) {
		}
		protected override void OnShowingDockGuidesCore(Customization.DockGuidesConfiguration configuration, BaseDocument document, BaseViewHitInfo hitInfo) {
			configuration.Disable(DockGuide.Center);
			configuration.Disable(DockGuide.CenterDock);
			configuration.DisableTabHint();
		}
		protected internal override void RegisterListeners(DevExpress.XtraBars.Docking2010.DragEngine.BaseUIView uiView) {
			uiView.RegisterUIServiceListener(new Dragging.DocumentManagerUIViewRegularDragListener());
			uiView.RegisterUIServiceListener(new Dragging.DocumentManagerUIViewDockingListener());
			uiView.RegisterUIServiceListener(new Dragging.DocumentManagerUIViewFloatingDragListener());
			uiView.RegisterUIServiceListener(new Dragging.DocumentManagerUIViewUIInteractionListener());
			uiView.RegisterUIServiceListener(new Dragging.WidgetUI.DocumentManagerUIViewReorderingListener(this));
		}
		protected override void OnDocumentAdded(BaseDocument document) {
			base.OnDocumentAdded(document);
			if(DocumentGroup != null && !DocumentGroup.Items.Contains(document as Document)) {
				DocumentGroup.Items.Add(document as Document);
			}
			if(!IsDocking || document is EmptyDocument)
				Controller.AddDocument(document);
			if(Manager != null)
				Manager.InvokePatchActiveChildren();
			RegisterDocumentInfo(document as Document);
		}
		protected override void OnDocumentRemoved(BaseDocument document) {
			base.OnDocumentRemoved(document);
			Controller.RemoveDocument(document);
			var widgetDocument = document as Document;
			if(TableGroup.Items.Contains(widgetDocument))
				TableGroup.Items.Remove(widgetDocument);
			if(FlowLayoutGroup.Items.Contains(widgetDocument)) {
				FlowLayoutGroup.Items.Remove(widgetDocument);
			}
			UnregisterDocumentInfo(document as Document);
		}
		protected override AppearanceObject GetActiveDocumentCaptionAppearance() {
			return AppearanceActiveDocumentCaption;
		}
		protected override AppearanceObject GetDocumentCaptionAppearance() {
			return AppearanceDocumentCaption;
		}
		protected virtual void PopulateTableGroupItems() {
			if(LayoutMode == Widget.LayoutMode.StackLayout) {
				TableGroup.Items.Clear();
				FlowLayoutGroup.Items.Clear();
				return;
			}
			foreach(var item in Documents) {
				if(item.IsVisible && !DocumentGroup.Items.Contains(item as Document))
					DocumentGroup.Items.Add(item as Document);
			}
		}
		#region Appearances
		protected override BaseViewAppearanceCollection CreateAppearanceCollection() {
			return new WidgetViewAppearanceCollection(this);
		}
		protected internal virtual void OnDeferredLoadDocumentMaximizedControlReleased(Control control, BaseDocument document) {
			Documents.UnregisterControl(control);
			RaiseMaximizedControlReleased(document, control);
		}
		protected internal override void OnDeferredLoadDocumentControlReleased(Control control, BaseDocument document) {
			base.OnDeferredLoadDocumentControlReleased(control, document);
			var widgetDocument = document as Document;
			Documents.UnregisterControl(widgetDocument.MaximizedControl);
		}
		bool allowDocumentCaptionColorBlendingCore;
		[Category("Appearance")]
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("WidgetViewAllowDocumentCaptionColorBlending"),
#endif
 DefaultValue(true)]
		[XtraSerializableProperty]
		public bool AllowDocumentCaptionColorBlending {
			get { return allowDocumentCaptionColorBlendingCore; }
			set { SetValue(ref allowDocumentCaptionColorBlendingCore, value); }
		}
		protected override bool CanBlendCaptionColor() {
			return AllowDocumentCaptionColorBlending;
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("WidgetViewAppearanceDocumentCaption"),
#endif
 Category("Appearance"),
		 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DevExpress.Utils.AppearanceObject AppearanceDocumentCaption {
			get { return ((WidgetViewAppearanceCollection)AppearanceCollection).DocumentCaption; }
		}
		bool ShouldSerializeAppearanceDocumentCaption() {
			return !IsDisposing && AppearanceCollection != null && AppearanceDocumentCaption.ShouldSerialize();
		}
		void ResetAppearanceDocumentCaption() {
			AppearanceDocumentCaption.Reset();
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("WidgetViewAppearanceActiveDocumentCaption"),
#endif
 Category("Appearance"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DevExpress.Utils.AppearanceObject AppearanceActiveDocumentCaption {
			get { return ((WidgetViewAppearanceCollection)AppearanceCollection).ActiveDocumentCaption; }
		}
		bool ShouldSerializeAppearanceActiveDocumentCaption() {
			return !IsDisposing && AppearanceCollection != null && AppearanceActiveDocumentCaption.ShouldSerialize();
		}
		void ResetAppearanceActiveDocumentCaption() {
			AppearanceActiveDocumentCaption.Reset();
		}
		#endregion Appearances
		#region XtraSerializable
		protected override void BeginRestoreLayout() {
			base.BeginRestoreLayout();
			NotRestoredDocuments = new List<Document>();
		}
		protected override void EndRestoreLayout() {
			RestoreLayoutCore();
			CheckNotRestoredDocuments();
			base.EndRestoreLayout();
		}
		void CheckNotRestoredDocuments() {
			foreach(Document document in NotRestoredDocuments) {
				Documents.Remove(document);
				Documents.Add(document);
				Controller.AddDocument(document);
			}
			NotRestoredDocuments.Clear();
			NotRestoredDocuments = null;
		}
		IList<Document> notRestoredDocumentsCore;
		IList<Document> NotRestoredDocuments {
			get { return notRestoredDocumentsCore; }
			set { notRestoredDocumentsCore = value; }
		}
		protected override object XtraFindItemsItem(XtraItemEventArgs e) {
			XtraPropertyInfo nameInfo = e.Item.ChildProperties["Name"];
			string name = nameInfo.Value as string;
			foreach(SerializableObjectInfo objectInfo in Items) {
				if(objectInfo.Name == name) {
					if(objectInfo is SerializableColumnDefinitionInfo || objectInfo is SerializableRowDefinitionInfo) continue;
					return objectInfo;
				}
			}
			return null;
		}
		protected void RestoreLayoutCore() {
			IDictionary<string, SerializableObjectInfo> itemsAndNames = new Dictionary<string, SerializableObjectInfo>();
			foreach(SerializableObjectInfo info in Items)
				itemsAndNames.Add(info.Name, info);
			List<StackGroup> groups = new List<StackGroup>();
			List<StackGroup> groupsToRemove = new List<StackGroup>();
			foreach(SerializableObjectInfo info in Items) {
				SerializableDocumentInfo documentInfo = info as SerializableDocumentInfo;
				if(documentInfo != null) {
					Document document = documentInfo.Source as Document;
					RestoreDocumentProperties(documentInfo, document);
					RestoreGroupItems(itemsAndNames, groups, documentInfo);
				}
				SerializableStackGroupInfo groupInfo = info as SerializableStackGroupInfo;
				if(groupInfo != null) {
					StackGroup group = groupInfo.Source as StackGroup;
					group.Caption = groupInfo.Caption;
					if(!groups.Contains(group)) {
						groups.Add(group);
					}
				}
			}
			RestoreItemsOrderAndSelection(groups);
			RestoreElementsLength(groups);
			StackGroups.AddRange(groups);
		}
		protected virtual void RestoreGroupItems(IDictionary<string, SerializableObjectInfo> itemsAndNames, List<StackGroup> groups, SerializableDocumentInfo documentInfo) {
			Document document = documentInfo.Source as Document;
			SerializableObjectInfo gInfo;
			bool restored = false;
			string stackGroupName = string.IsNullOrEmpty(documentInfo.StackGroupName) ? documentInfo.ParentName : documentInfo.StackGroupName;
			if(!string.IsNullOrEmpty(stackGroupName) && itemsAndNames.TryGetValue(stackGroupName, out gInfo)) {
				StackGroup group = gInfo.Source as StackGroup;
				if(!groups.Contains(group)) {
					RegisterStackGroupInfo(group);
					groups.Add(group);
				}
				group.Info.Register(document);
				group.Items.Add(document);
				restored = true;
			}
			if(string.Equals(documentInfo.ParentName, FlowLayoutGroup.Name)) {
				FlowLayoutGroup.Items.Add(document);
				restored = true;
			}
			if(string.Equals(documentInfo.ParentName, TableGroup.Name)) {
				TableGroup.Items.Add(document);
				restored = true;
			}
			if(!restored)
				NotRestoredDocuments.Add(document);
		}
		protected virtual void RestoreDocumentProperties(SerializableDocumentInfo docInfo, Document document) {
			document.RowIndex = docInfo.RowIndex;
			document.ColumnIndex = docInfo.ColumnIndex;
			document.RowSpan = docInfo.RowSpan;
			document.ColumnSpan = docInfo.ColumnSpan;
			document.Width = docInfo.Width;
			document.Height = docInfo.Height;
		}
		void RestoreElementsLength(IEnumerable<BaseRelativeLengthElement> elements) {
			foreach(var item in elements) {
				SerializableObjectInfo info;
				if(Infos.TryGetValue(item, out info)) {
					BaseSerializableRelativeLengthElementInfo groupInfo = info as BaseSerializableRelativeLengthElementInfo;
					item.Length = groupInfo.Length;
				}
				else continue;
			}
		}
		void RestoreItemsOrderAndSelection(List<StackGroup> groups) {
			groups.Sort(new SerializableRelativeLengthElementInfoIndexComparer(Infos));
			foreach(StackGroup group in groups) {
				group.Items.Sort(new SerializableDocumentInfoIndexComparer(Infos));
			}
			flowLayoutGroupCore.Items.Sort(new SerializableDocumentInfoIndexComparer(Infos));
		}
		protected override void PrepareSerializableObjectInfos() {
			base.PrepareSerializableObjectInfos();
			if(!IsDeserializing) {
				PrepareSerializableStackGroupInfos();
				if(LayoutMode == Widget.LayoutMode.FlowLayout) {
					PrepareSerializableFlowLayoutItemInfos();
				}
				if(LayoutMode == Widget.LayoutMode.TableLayout) {
					PrepareSerializableTableLayoutItemInfos();
				}
			}
			else
				StackGroups.Clear();
		}
		protected virtual void PrepareSerializableTableLayoutItemInfos() {
			SerializableObjectInfo documentInfo;
			foreach(var document in TableGroup.Items) {
				if(Infos.TryGetValue(document, out documentInfo)) {
					var widgetDocumentInfo = documentInfo as SerializableDocumentInfo;
					widgetDocumentInfo.ParentName = TableGroup.Name;
				}
			}
		}
		protected virtual void PrepareSerializableFlowLayoutItemInfos() {
			SerializableObjectInfo documentInfo;
			foreach(var document in FlowLayoutGroup.Items) {
				if(Infos.TryGetValue(document, out documentInfo)) {
					var widgetDocumentInfo = documentInfo as SerializableDocumentInfo;
					widgetDocumentInfo.Index = FlowLayoutGroup.Items.IndexOf(document as Document);
					widgetDocumentInfo.ParentName = FlowLayoutGroup.Name;
				}
			}
		}
		protected virtual void PrepareSerializableStackGroupInfos() {
			StackGroup[] stackGroups = StackGroups.ToArray();
			for(int i = 0; i < stackGroups.Length; i++) {
				StackGroup group = stackGroups[i];
				SerializableObjectInfo info = CreateSerializableStackGroupInfo(group);
				info.SetNameByIndex(i);
				RegisterSerializableObjectinfo(group, info);
				if(IsSerializing)
					UpdateChildrenParentNames(group, info);
			}
		}
		void UpdateChildrenParentNames(StackGroup group, SerializableObjectInfo info) {
			foreach(Document document in group.Items) {
				SerializableObjectInfo documentInfo;
				if(Infos.TryGetValue(document, out documentInfo)) {
					((SerializableDocumentInfo)documentInfo).StackGroupName = info.Name;
				}
			}
		}
		protected override BaseSerializableDocumentInfo CreateSerializableDocumentInfo(BaseDocument document) {
			return new SerializableDocumentInfo(document as Document);
		}
		protected virtual SerializableStackGroupInfo CreateSerializableStackGroupInfo(StackGroup group) {
			return new SerializableStackGroupInfo(group, StackGroups.IndexOf(group));
		}
		protected virtual BaseSerializableRelativeLengthElementInfo CreateSerializableRowDefinitionInfo(RowDefinition rowDefinition) {
			return new SerializableRowDefinitionInfo(rowDefinition, Rows.IndexOf(rowDefinition));
		}
		protected virtual BaseSerializableRelativeLengthElementInfo CreateSerializableColumnDefinitionInfo(ColumnDefinition columnDefinition) {
			return new SerializableColumnDefinitionInfo(columnDefinition, Columns.IndexOf(columnDefinition));
		}
		protected override SerializableObjectInfo CreateSerializableObjectInfo(string typeName) {
			switch(typeName) {
				case "StackGroup":
					StackGroup group = CreateStackGroup();
					group.SetManager(Manager);
					AddToContainer(group);
					SerializableStackGroupInfo result = CreateSerializableStackGroupInfo(group);
					RegisterSerializableObjectinfo(group, result);
					return result;
			}
			return base.CreateSerializableObjectInfo(typeName);
		}
		protected class BaseSerializableRelativeLengthElementInfo : SerializableObjectInfo {
			public BaseSerializableRelativeLengthElementInfo(BaseRelativeLengthElement element, int index)
				: base(element) {
				lengthCore = element.Length;
				Index = index;
			}
			protected override string GetTypeNameCore() {
				return "RelativeLengthElement";
			}
			int indexCore;
			[XtraSerializableProperty]
			public int Index {
				get { return indexCore; }
				set { indexCore = value; }
			}
			Length lengthCore;
			[XtraSerializableProperty(XtraSerializationVisibility.Content)]
			public Length Length {
				get { return lengthCore; }
				set { lengthCore = value; }
			}
		}
		protected class SerializableRowDefinitionInfo : BaseSerializableRelativeLengthElementInfo {
			public SerializableRowDefinitionInfo(BaseRelativeLengthElement element, int index)
				: base(element, index) {
			}
			protected override string GetTypeNameCore() {
				return "RowDefinition";
			}
		}
		protected class SerializableColumnDefinitionInfo : BaseSerializableRelativeLengthElementInfo {
			public SerializableColumnDefinitionInfo(BaseRelativeLengthElement element, int index)
				: base(element, index) {
			}
			protected override string GetTypeNameCore() {
				return "ColumnDefinition";
			}
		}
		protected class SerializableStackGroupInfo : BaseSerializableRelativeLengthElementInfo {
			IStackGroupDefaultProperties propertiesCore;
			string captionCore;
			public SerializableStackGroupInfo(StackGroup group, int index)
				: base(group, index) {
				propertiesCore = group.Properties;
				captionCore = group.Caption;
			}
			protected override string GetTypeNameCore() {
				return "StackGroup";
			}
			[XtraSerializableProperty(XtraSerializationVisibility.Visible)]
			public string Caption {
				get { return captionCore; }
				set { captionCore = value; }
			}
			[XtraSerializableProperty(XtraSerializationVisibility.Content)]
			public IStackGroupDefaultProperties Properties {
				get { return propertiesCore; }
			}
		}
		protected class SerializableDocumentInfo : BaseSerializableDocumentInfo {
			public SerializableDocumentInfo(Document document)
				: base(document) {
				if(!IsFloating) {
					if(document.Form != null)
						windowStateCore = document.Form.WindowState;
				}
				if(document.Parent != null)
					Index = document.Parent.Items.IndexOf(document);
				Width = document.Width;
				Height = document.Height;
				RowIndex = document.RowIndex;
				ColumnIndex = document.ColumnIndex;
				RowSpan = document.RowSpan;
				ColumnSpan = document.ColumnSpan;
			}
			string parentNameCore;
			[XtraSerializableProperty]
			public string ParentName {
				get { return parentNameCore; }
				set { parentNameCore = value; }
			}
			string stackGroupNameCore;
			[XtraSerializableProperty]
			public string StackGroupName {
				get { return stackGroupNameCore; }
				set { stackGroupNameCore = value; }
			}
			int indexCore;
			[XtraSerializableProperty]
			public int Index {
				get { return indexCore; }
				set { indexCore = value; }
			}
			FormWindowState windowStateCore;
			[XtraSerializableProperty]
			public FormWindowState WindowState {
				get { return windowStateCore; }
				set { windowStateCore = value; }
			}
			int widthCore;
			[XtraSerializableProperty]
			public int Width {
				get { return widthCore; }
				set { widthCore = value; }
			}
			int heightCore;
			[XtraSerializableProperty]
			public int Height {
				get { return heightCore; }
				set { heightCore = value; }
			}
			[XtraSerializableProperty]
			public int RowIndex { get; set; }
			[XtraSerializableProperty]
			public int ColumnIndex { get; set; }
			[XtraSerializableProperty]
			public int RowSpan { get; set; }
			[XtraSerializableProperty]
			public int ColumnSpan { get; set; }
		}
		class SerializableDocumentInfoIndexComparer : IComparer<Document> {
			IDictionary<object, SerializableObjectInfo> infos;
			public SerializableDocumentInfoIndexComparer(IDictionary<object, SerializableObjectInfo> infos) {
				this.infos = infos;
			}
			public int Compare(Document x, Document y) {
				if(x == y) return 0;
				SerializableObjectInfo xInfo; infos.TryGetValue(x, out xInfo);
				SerializableObjectInfo yInfo; infos.TryGetValue(y, out yInfo);
				return ((SerializableDocumentInfo)xInfo).Index.CompareTo(((SerializableDocumentInfo)yInfo).Index);
			}
		}
		class SerializableRelativeLengthElementInfoIndexComparer : IComparer<BaseRelativeLengthElement> {
			IDictionary<object, SerializableObjectInfo> infos;
			public SerializableRelativeLengthElementInfoIndexComparer(IDictionary<object, SerializableObjectInfo> infos) {
				this.infos = infos;
			}
			public int Compare(BaseRelativeLengthElement x, BaseRelativeLengthElement y) {
				if(x == y) return 0;
				SerializableObjectInfo xInfo; infos.TryGetValue(x, out xInfo);
				SerializableObjectInfo yInfo; infos.TryGetValue(y, out yInfo);
				return ((BaseSerializableRelativeLengthElementInfo)xInfo).Index.CompareTo(((BaseSerializableRelativeLengthElementInfo)yInfo).Index);
			}
		}
		protected virtual object XtraCreateColumnsItem(XtraItemEventArgs e) {
			ColumnDefinition columnDefinition = new ColumnDefinition();
			AddToContainer(columnDefinition);
			Columns.Add(columnDefinition);
			BaseSerializableRelativeLengthElementInfo columnDefinitionInfo = CreateSerializableColumnDefinitionInfo(columnDefinition);
			return columnDefinitionInfo;
		}
		protected virtual object XtraCreateRowsItem(XtraItemEventArgs e) {
			RowDefinition rowDefinition = new RowDefinition();
			AddToContainer(rowDefinition);
			Rows.Add(rowDefinition);
			BaseSerializableRelativeLengthElementInfo rowDefinitionInfo = CreateSerializableRowDefinitionInfo(rowDefinition);
			return rowDefinition;
		}
		#endregion XtraSerializable
	}
}
