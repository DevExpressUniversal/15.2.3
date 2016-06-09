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
using System.Linq;
using System.Windows.Forms;
using DevExpress.Utils.Serializing;
using DevExpress.XtraBars.Docking2010.Base;
using DevExpress.XtraBars.Docking2010.Customization;
namespace DevExpress.XtraBars.Docking2010.Views.Tabbed {
	public class TabbedView : BaseView, IThumbnailViewClient {
		Orientation orientationCore;
		DocumentGroupCollection documentGroupsCore;
		IDocumentGroupProperties documentGroupPropertiesCore;
		public TabbedView() { }
		public TabbedView(IContainer container)
			: base(container) {
		}
		public sealed override ViewType Type {
			get { return ViewType.Tabbed; }
		}
		protected sealed internal override Type GetUIElementKey() {
			return typeof(TabbedView);
		}
		protected sealed internal override bool AllowMdiLayout {
			get { return false; }
		}
		protected sealed internal override bool AllowMdiSystemMenu {
			get { return false; }
		}
		protected sealed override bool ShouldRecalculateLayoutOnFocusChange {
			get { return true; }
		}
		protected override void OnCreate() {
			base.OnCreate();
			documentGroupsCore = CreateDocumentGroups();
			DocumentGroups.CollectionChanged += OnDocumentGroupsCollectionChanged;
			documentGroupPropertiesCore = CreateDocumentGroupProperties();
			DocumentGroupProperties.Changed += OnDocumentGroupPropertiesChanged;
			orientationCore = Orientation.Horizontal;
		}
		protected override void LockComponentBeforeDisposing() {
			base.LockComponentBeforeDisposing();
			DocumentGroups.CollectionChanged -= OnDocumentGroupsCollectionChanged;
			DocumentGroupProperties.Changed -= OnDocumentGroupPropertiesChanged;
		}
		protected override void OnDispose() {
			Unload();
			Ref.Dispose(ref documentGroupsCore);
			Ref.Dispose(ref documentGroupPropertiesCore);
			visibleDocumentGroupsCore = null;
			base.OnDispose();
		}
		protected override void OnManagerChanged() {
			base.OnManagerChanged();
			foreach(DocumentGroup group in DocumentGroups)
				group.SetManager(Manager);
		}
		[Browsable(false)]
		public new ITabbedViewController Controller {
			get { return base.Controller as ITabbedViewController; }
		}
		protected override IBaseViewController CreateController() {
			return new TabbedViewController(this);
		}
		void OnDocumentGroupsCollectionChanged(CollectionChangedEventArgs<DocumentGroup> ea) {
			DocumentGroup group = ea.Element;
			if(ea.ChangedType == CollectionChangedType.ElementAdded) {
				AddToContainer(group);
				OnDocumentGroupAdded(group);
			}
			if(ea.ChangedType == CollectionChangedType.ElementRemoved) {
				OnDocumentGroupRemoved(group);
				RemoveFromContainer(group);
			}
			visibleDocumentGroupsCore = null;
			SetLayoutModified();
			LayoutChanged();
		}
		void OnDocumentGroupPropertiesChanged(object sender, EventArgs e) {
			using(LockPainting()) {
				UpdateGroupHeaderButton(e);
				RequestInvokePatchActiveChild();
			}
		}
		void UpdateGroupHeaderButton(EventArgs e) {
			PropertyChangedEventArgs args = e as PropertyChangedEventArgs;
			if(args == null) return;
			switch(args.PropertyName) {
				case "ShowDocumentSelectorButton":
					foreach(DocumentGroup documentGroup in DocumentGroups) {
						if(documentGroup.Info == null) continue;
						documentGroup.Info.CheckDropDownButton();
					}
					break;
				case "CustomHeaderButtons":
					foreach(DocumentGroup documentGroup in DocumentGroups) {
						if(documentGroup.Info == null) continue;
						documentGroup.Info.UpdateCustomHeaderButtons(documentGroup);
					}
					break;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("TabbedViewShowDockGuidesOnPressingShift"),
#endif
	  Category("Behavior"), DefaultValue(DevExpress.Utils.DefaultBoolean.Default)]
		public DevExpress.Utils.DefaultBoolean ShowDockGuidesOnPressingShift {
			get { return ShowDockGuidesOnPressingShiftBase; }
			set { ShowDockGuidesOnPressingShiftBase = value; }
		}
		DevExpress.Utils.DefaultBoolean showDocumentSelectorMenuOnCtrlAltDownArrowCore = DevExpress.Utils.DefaultBoolean.Default;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("TabbedViewShowDocumentSelectorMenuOnCtrlAltDownArrow"),
#endif
	  Category("Behavior"), DefaultValue(DevExpress.Utils.DefaultBoolean.Default)]
		public DevExpress.Utils.DefaultBoolean ShowDocumentSelectorMenuOnCtrlAltDownArrow {
			get { return showDocumentSelectorMenuOnCtrlAltDownArrowCore; }
			set { showDocumentSelectorMenuOnCtrlAltDownArrowCore = value; }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("TabbedViewDocumentProperties")]
#endif
		[Category("Layout"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public new IDocumentProperties DocumentProperties {
			get { return base.DocumentProperties as IDocumentProperties; }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("TabbedViewOrientation")]
#endif
		[Category("Layout"), DefaultValue(Orientation.Horizontal), XtraSerializableProperty]
		public Orientation Orientation {
			get { return orientationCore; }
			set {
				if(orientationCore == value) return;
				orientationCore = value;	   
				OnOrientationChanged();
			}
		}
		protected internal override void AssignProperties(DevExpress.XtraBars.Docking2010.Views.BaseView parentView) {
			base.AssignProperties(parentView);
			TabbedView parentTabbedView = parentView as TabbedView;
			if(parentTabbedView!=null){
				this.orientationCore = parentTabbedView.Orientation;
				this.showDocumentSelectorMenuOnCtrlAltDownArrowCore = parentTabbedView.ShowDocumentSelectorMenuOnCtrlAltDownArrow;
				this.DocumentGroupProperties.Assign(this.DocumentGroupProperties);
			}
		}
		protected override BaseViewAppearanceCollection CreateAppearanceCollection() {
			return new TabbedViewAppearanceCollection(this);
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("TabbedViewAppearancePage"),
#endif
 Category("Appearance"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TabbedViewPageAppearance AppearancePage {
			get { return ((TabbedViewAppearanceCollection)AppearanceCollection).AppearancePage; }
		}
		bool ShouldSerializeAppearancePage() {
			return !IsDisposing && AppearanceCollection != null && AppearancePage.ShouldSerialize();
		}
		void ResetAppearancePage() {
			AppearancePage.Reset();
		}
		#region Events
		static readonly object orientationChanged = new object();
		static readonly object customHeaderButtonClick = new object();
		static readonly object tabMouseActivating = new object();
		static readonly object documentPinned = new object();
		static readonly object documentUnpinned = new object();
		[Category("Layout")]
		public event EventHandler OrientationChanged {
			add { Events.AddHandler(orientationChanged, value); }
			remove { Events.RemoveHandler(orientationChanged, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public event CustomHeaderButtonEventHandler CustomHeaderButtonClick {
			add { Events.AddHandler(customHeaderButtonClick, value); }
			remove { Events.RemoveHandler(customHeaderButtonClick, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public event DocumentCancelEventHandler TabMouseActivating {
			add { Events.AddHandler(tabMouseActivating, value); }
			remove { Events.RemoveHandler(tabMouseActivating, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Layout)]
		public event DocumentEventHandler DocumentPinned {
			add { Events.AddHandler(documentPinned, value); }
			remove { Events.RemoveHandler(documentPinned, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Layout)]
		public event DocumentEventHandler DocumentUnpinned {
			add { Events.AddHandler(documentUnpinned, value); }
			remove { Events.RemoveHandler(documentUnpinned, value); }
		}
		protected void RaiseOrientationChanged() {
			EventHandler handler = (EventHandler)Events[orientationChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected void RaiseCustomHeaderButtonClick(CustomHeaderButtonEventArgs e) {
			CustomHeaderButtonEventHandler handler = (CustomHeaderButtonEventHandler)Events[customHeaderButtonClick];
			if(handler != null) handler(this, e);
		}
		protected internal void RaiseCustomHeaderButtonClick(XtraTab.Buttons.CustomHeaderButton button, Document document) {
			RaiseCustomHeaderButtonClick(new CustomHeaderButtonEventArgs(button, document));
		}
		protected internal override bool RaiseTabMouseActivating(BaseDocument document) {
			bool result = false;
			if(document is Document && ((Document)document).Parent != null)
				result = ((Document)document).Parent.RaiseTabMouseActivating(document);
			DocumentCancelEventArgs ea = new DocumentCancelEventArgs(document);
			DocumentCancelEventHandler handler = (DocumentCancelEventHandler)Events[tabMouseActivating];
			if(handler != null) handler(this, ea);
			result |= ea.Cancel;
			return result;
		}
		protected internal void RaiseDocumentPinned(Document document ){
			DocumentEventHandler handler = (DocumentEventHandler)Events[documentPinned];
			DocumentEventArgs ea = new DocumentEventArgs(document);
			if(handler != null) handler(this, ea);
		}
		protected internal void RaiseDocumentUnpinned(Document document) {
			DocumentEventHandler handler = (DocumentEventHandler)Events[documentUnpinned];
			DocumentEventArgs ea = new DocumentEventArgs(document);
			if(handler != null) handler(this, ea);
		}
		#endregion Events
		protected virtual void OnOrientationChanged() {
			using(LockPainting()) {
				RootContainer.Orientation = Orientation;
				RootContainer.RepopulateSplitters();
				SetLayoutModified();
				RequestInvokePatchActiveChild();
				RaiseOrientationChanged();
			}
		}
		[Browsable(false)]
		public bool IsHorizontal {
			get { return Orientation == Orientation.Horizontal; }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("TabbedViewDocumentGroups")]
#endif
		[Category("Layout"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Editor("DevExpress.Utils.Design.DXCollectionEditorBase, " + AssemblyInfo.SRAssemblyDesign, typeof(System.Drawing.Design.UITypeEditor))]
		public DocumentGroupCollection DocumentGroups {
			get { return documentGroupsCore; }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("TabbedViewDocumentGroupProperties")]
#endif
		[Category("Layout"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public IDocumentGroupProperties DocumentGroupProperties {
			get { return documentGroupPropertiesCore; }
		}
		bool ShouldSerializeDocumentGroupProperties() {
			return !IsDisposing && DocumentGroupProperties != null && DocumentGroupProperties.ShouldSerialize();
		}
		void ResetDocumentGroupProperties() {
			DocumentGroupProperties.Reset();
		}
		DevExpress.Utils.DefaultBoolean enableFreeLayoutModeCore = DevExpress.Utils.DefaultBoolean.Default;
#if !SL
	[DevExpressXtraBarsLocalizedDescription("TabbedViewEnableFreeLayoutMode")]
#endif
		[Category("Behavior"), DefaultValue(DevExpress.Utils.DefaultBoolean.Default), XtraSerializableProperty(XtraSerializationVisibility.Visible)]
		public DevExpress.Utils.DefaultBoolean EnableFreeLayoutMode {
			get { return enableFreeLayoutModeCore; }
			set { SetValue(ref enableFreeLayoutModeCore, value, LayoutChanged); }
		}
		protected internal virtual bool AllowFreeLayoutMode() {
			return EnableFreeLayoutMode == DevExpress.Utils.DefaultBoolean.True;
		}
		protected virtual DocumentGroupCollection CreateDocumentGroups() {
			return new DocumentGroupCollection();
		}
		protected internal virtual DocumentGroup CreateDocumentGroup() {
			return new DocumentGroup(DocumentGroupProperties);
		}
		protected override IBaseDocumentProperties CreateDocumentProperties() {
			return new DocumentProperties();
		}
		protected virtual IDocumentGroupProperties CreateDocumentGroupProperties() {
			return new DocumentGroupProperties();
		}
		protected override void BeforeActiveDocumentChanged(BaseDocument baseDocument) {
			base.BeforeActiveDocumentChanged(baseDocument);
			Document document = baseDocument as Document;
			if(document != null && document.Parent != null)
				document.Parent.SetSelected(document);
		}
		protected override void OnLoaded() {
			base.OnLoaded();
			UpdateTree(RootContainer);
		}
		protected override void OnInitialized() {
			base.OnInitialized();
			if(RootContainer.Nodes.Count == 0) {
				RootContainer.Orientation = Orientation;
				foreach(var item in DocumentGroups) {
					RootContainer.TabbedViewInfo = ((TabbedViewInfo)ViewInfo);
					RootContainer.AddGroup(item);
				}
			}
			UpdateTree(RootContainer);
		}
		void UpdateTree(DockingContainer rootNode) {
			rootNode.TabbedViewInfo = ViewInfo as TabbedViewInfo;
			rootNode.RepopulateSplitters();
			if(rootNode.Parent != null && rootNode.Element == null && rootNode.Nodes.Count == 0) {
				rootNode.Parent.Nodes.Remove(rootNode);
			}
			for(int i = rootNode.Nodes.Count - 1; i >= 0; i--) {
				DockingContainer item = rootNode.Nodes[i];
				UpdateTree(item);
			}
		}
		internal DockingContainer rootDockingContainerCore = new DockingContainer(); 
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
#if !SL
	[DevExpressXtraBarsLocalizedDescription("TabbedViewRootContainer")]
#endif
		public DockingContainer RootContainer { get { return rootDockingContainerCore; } }
		protected virtual void OnDocumentGroupAdded(DocumentGroup group) {
			if(!IsInitializing) {
				RootContainer.TabbedViewInfo = ((TabbedViewInfo)ViewInfo);
				RootContainer.AddGroup(group);
			}
			group.IsLoaded = true;
			group.Properties.EnsureParentProperties(DocumentGroupProperties);
			group.SetManager(Manager);
			RegisterDocumentGroupInfo(group);
			group.VisibilityChanged += OnDocumentGroupVisibilityChanged;
			visibleDocumentGroupsCore = null;
			InvalidateUIView();
		}
		protected virtual void OnDocumentGroupRemoved(DocumentGroup group) {
			RootContainer.TabbedViewInfo = ((TabbedViewInfo)ViewInfo);
			DockingContainerHelper.RemoveDocumentGroup(group, RootContainer);
			UnregisterDocumentGroupInfo(group);
			group.VisibilityChanged -= OnDocumentGroupVisibilityChanged;
			visibleDocumentGroupsCore = null;
			InvalidateUIView();
			group.SetManager(null);
			group.IsLoaded = false;
		}
		public void SetGroupsVisibility(IEnumerable<DocumentGroup> groups, bool value) {
			using(BatchUpdate.Enter(Manager)) {
				using(LockPainting()) {
					foreach(DocumentGroup group in groups) {
						group.Visible = value;
					}
				}
			}
		}
		protected internal override void OnDeferredLoadDocumentControlLoaded(Control control, BaseDocument document) {
			base.OnDeferredLoadDocumentControlLoaded(control, document);
			visibleDocumentGroupsCore = null;
		}
		protected override void OnDocumentAdded(BaseDocument baseDocument) {
			visibleDocumentGroupsCore = null;
			base.OnDocumentAdded(baseDocument);
			Document document = baseDocument as Document;
			if(document != null) {
				RegisterDocumentInfo(document);
				if(!IsInitializing && !IsDocking && !IsDeserializing)
					Controller.AddDocument(document);
				if(IsDesignMode()) groupsToRestore.Remove(document);
			}
		}
		protected internal override void OnEndFloating(BaseDocument document, EndFloatingReason reason) {
			base.OnEndFloating(document, reason);
			if(IsDesignMode() && !document.IsDockPanel && reason != EndFloatingReason.Docking) {
				DevExpress.XtraBars.Docking2010.Views.Tabbed.DocumentGroup doc = null;
				groupsToRestore.TryGetValue((DevExpress.XtraBars.Docking2010.Views.Tabbed.Document)document, out doc);
				if(DocumentGroups.Contains(doc))
					Controller.Dock((DevExpress.XtraBars.Docking2010.Views.Tabbed.Document)document, doc);
				else
					Controller.CreateNewDocumentGroup((Document)document, Orientation == Orientation.Vertical ? Orientation.Horizontal : Orientation.Vertical);
			}
		}
		internal Dictionary<Document, DocumentGroup> groupsToRestore = new Dictionary<Document, DocumentGroup>();
		protected override void OnDocumentRemoved(BaseDocument baseDocument) {
			base.OnDocumentRemoved(baseDocument);
			Document document = baseDocument as Document;
			if(document != null) {
				Controller.RemoveDocument(document);
				UnregisterDocumentInfo(document);
			}
		}
		public void StartAnimation() {
			foreach(Document document in Documents) {
				document.StartAnimation();
			}
		}
		public void StopAnimation() {
			foreach(Document document in Documents) {
				document.StopAnimation();
			}
		}
		protected override void RegisterInfos() {
			if(IsDisposing) return;
			foreach(Document document in Documents)
				RegisterDocumentInfo(document);
			foreach(DocumentGroup group in DocumentGroups) {
				RegisterDocumentGroupInfo(group);
				foreach(Document document in group.Items) {
					RegisterDocumentInfo(document, group);
				}
			}
		}
		protected override void UnregisterInfos() {
			if(IsDisposing) return;
			foreach(DocumentGroup group in DocumentGroups) {
				foreach(Document document in group.Items) {
					UnregisterDocumentInfo(document, group);
				}
				UnregisterDocumentGroupInfo(group);
			}
			foreach(Document document in Documents)
				UnregisterDocumentInfo(document);
		}
		protected internal void RegisterDocumentInfo(Document document) {
			if(ViewInfo != null)
				((TabbedViewInfo)ViewInfo).RegisterInfo(document);
		}
		protected internal void UnregisterDocumentInfo(Document document) {
			if(ViewInfo != null)
				((TabbedViewInfo)ViewInfo).UnregisterInfo(document);
		}
		protected internal void RegisterDocumentInfo(Document document, DocumentGroup group) {
			if(ViewInfo != null)
				((TabbedViewInfo)ViewInfo).RegisterInfo(document, group);
		}
		protected internal void UnregisterDocumentInfo(Document document, DocumentGroup group) {
			if(ViewInfo != null)
				((TabbedViewInfo)ViewInfo).UnregisterInfo(document, group);
		}
		protected internal void RegisterDocumentGroupInfo(DocumentGroup group) {
			if(ViewInfo != null)
				((TabbedViewInfo)ViewInfo).RegisterInfo(group);
		}
		protected internal void UnregisterDocumentGroupInfo(DocumentGroup group) {
			if(ViewInfo != null)
				((TabbedViewInfo)ViewInfo).UnregisterInfo(group);
		}
		protected internal override void PatchActiveChildren(Point offset) {
			for(int i = 0; i < DocumentGroups.Count; i++) {
				DocumentGroup group = DocumentGroups[i];
				Rectangle clientRect = group.Info.Client;
				clientRect.Offset(offset);
				group.PatchActiveChild(clientRect, Manager.Bounds);
			}
			for(int i = 0; i < DocumentGroups.Count; i++) {
				DocumentGroup group = DocumentGroups[i];
				if(!group.Visible) {
					var activeChild = group.GetActiveChild();
					Rectangle clientRect = new Rectangle(new Point(-10000, -10000), activeChild != null ? activeChild.Size : Bounds.Size);
					group.PatchActiveChild(clientRect, Manager.Bounds);
				}
			}
		}
		List<DocumentGroup> visibleDocumentGroupsCore;
		internal List<DocumentGroup> VisibleDocumentGroups {
			get {
				if(visibleDocumentGroupsCore == null) {
					visibleDocumentGroupsCore = new List<DocumentGroup>();
					foreach(DocumentGroup group in DocumentGroups) {
						if(group.Manager != null && group.Visible)
							visibleDocumentGroupsCore.Add(group);
					}
				}
				return visibleDocumentGroupsCore;
			}
		}
		void OnDocumentGroupVisibilityChanged(object sender, EventArgs e) {
			visibleDocumentGroupsCore = null;
			if(AllDocumentGroupIsInvisible())
				Manager.InvokePatchActiveChildren();
		}
		bool AllDocumentGroupIsInvisible() {
			return DocumentGroups.FindFirst((documentGrop) => documentGrop.Visible) == null;
		}
		protected internal override void PatchBeforeActivateChild(Control activatedChild, Point offset) {
			BaseDocument document;
			if(Documents.TryGetValue(activatedChild, out document)) {
				Control activeChild = Manager.GetActiveChild();
				for(int i = 0; i < VisibleDocumentGroups.Count; i++) {
					DocumentGroup group = VisibleDocumentGroups[i];
					if(group.Items.Contains(document as Document)) {
						Rectangle groupBounds = ViewInfo.VisibleMdiChildren[i];
						groupBounds.Offset(offset);
						group.PathBeforeActivateChild(activatedChild, activeChild, groupBounds);
						return;
					}
				}
			}
			for(int i = 0; i < DocumentGroups.Count; i++) {
				DocumentGroup group = DocumentGroups[i];
				if(!group.Visible) {
					Rectangle clientRect = new Rectangle(-10000, -10000, 100, 100);
					group.PatchActiveChild(clientRect, Manager.Bounds);
				}
			}
		}
		bool CanShowDocumentSelectorMenu() {
			return ShowDocumentSelectorMenuOnCtrlAltDownArrow != DevExpress.Utils.DefaultBoolean.False;
		}
		protected internal override bool ShowDocumentSelectorMenu() {
			var document = ActiveDocument as Views.Tabbed.Document;
			if(document != null && CanShowDocumentSelectorMenu())
				return Controller.ShowDocumentSelectorMenu(document.Parent);
			return false;
		}
		protected override bool CanSetNextDocument(BaseDocument document, bool forward) {
			Document activeDocument = document as Document;
			if(activeDocument == null || activeDocument.Parent == null) return false;
			return activeDocument.Parent.Properties.HasTabHeader;
		}
		protected internal override BaseDocument GetNextDocument(BaseDocument document, bool forward) {
			return GetNextDocument(GetDocuments(), document, forward);
		}
		protected internal Document[] GetDocuments() {
			List<Document> documents = new List<Document>();
			for(int i = 0; i < DocumentGroups.Count; i++) {
				DocumentGroup group = DocumentGroups[i];
				documents.AddRange(group.Items.ToArray());
			}
			return documents.ToArray();
		}
		protected internal int GetPinnedDocumentCount() {
			int result = 0;
			foreach(Document document in GetDocuments()) {
				if(!document.IsDisposing && !document.Properties.IsDisposing && document.Properties.CanPin && document.Pinned)
					result++;
			}
			return result;
		}
		protected internal override DevExpress.Utils.ToolTipControlInfo GetToolTipControlInfo(BaseViewHitInfo hitInfo) {
			IDocumentGroupInfo groupInfo = Dragging.InfoHelper.GetDocumentGroupInfo(hitInfo.Info.Element);
			if(groupInfo != null)
				return groupInfo.GetToolTipControlInfo(hitInfo);
			return base.GetToolTipControlInfo(hitInfo);
		}
		protected override bool HideDockGuidesInDesignMode() {
			return base.HideDockGuidesInDesignMode() && Manager.ContainerControl == null;
		}
		protected override DevExpress.Utils.AppearanceObject GetActiveDocumentCaptionAppearance() {
			return AppearancePage.HeaderActive;
		}
		protected override DevExpress.Utils.AppearanceObject GetDocumentCaptionAppearance() {
			return AppearancePage.Header;
		}
		protected override void OnShowingDockGuidesCore(DockGuidesConfiguration configuration, BaseDocument document, BaseViewHitInfo hitInfo) {
			Document tabbedDocument = document as Document;
			if(!tabbedDocument.Properties.CanDockFill)
				configuration.Disable(DockHint.Center);
			if(DocumentGroups.Count == 0) return;
			if(hitInfo.IsEmpty) return;
			IDocumentGroupInfo groupInfo = Dragging.InfoHelper.GetDocumentGroupInfo(hitInfo.Info.Element);
			if(groupInfo != null) {
				if(groupInfo.Group.IsFilledUp || groupInfo.Group == tabbedDocument.Parent)
					configuration.Disable(DockHint.Center);
				if(DocumentGroups.Count > 1 && groupInfo.Group == tabbedDocument.Parent && tabbedDocument.Parent.Items.Count < 2)
					configuration.DisableCenterHints();
			}
			ISplitterInfo splitterInfo = Dragging.InfoHelper.GetSplitterInfo(hitInfo.Info.Element);
			if(splitterInfo != null) {
				configuration.DisableTabHint();
				configuration.Disable(DockGuide.CenterDock);
			}
			if(!AllowFreeLayoutMode())
				DisableFreeLayoutDockHints(configuration);  
		}
		void DisableFreeLayoutDockHints(DockGuidesConfiguration configuration) {
			if(RootContainer.Orientation == Orientation.Horizontal) {
				if(RootContainer.Nodes.Count > 1) {
					configuration.Disable(DockHint.CenterTop);
					configuration.Disable(DockHint.CenterBottom);
				}
				else if(RootContainer.Nodes.Count == 1 && RootContainer.Nodes[0].Nodes.Count > 1) {
					configuration.Disable(DockHint.CenterLeft);
					configuration.Disable(DockHint.CenterRight);
				}
			}
			else {
				if(RootContainer.Nodes.Count > 1) {
					configuration.Disable(DockHint.CenterLeft);
					configuration.Disable(DockHint.CenterRight);
				}
				else if(RootContainer.Nodes.Count == 1 && RootContainer.Nodes[0].Nodes.Count > 1) {
					configuration.Disable(DockHint.CenterTop);
					configuration.Disable(DockHint.CenterBottom);
				}
			}
		}
		#region XtraSerializable
		IDisposable documentGroupsLockObj;
		protected override void BeginRestoreLayout() {
			base.BeginRestoreLayout();
			documentGroupsLockObj = CleanUpDocumentGroups();
			NotRestoredDocuments = new List<Document>();
		}
		protected override void EndRestoreLayout() {
			RestoreDocumentGroups();
			ResotreNodes();
			RestoreGroupLengths(DocumentGroups.ToList());
			Ref.Dispose(ref documentGroupsLockObj);
			CheckNotRestoredDocuments();
			visibleDocumentGroupsCore = null;
			base.EndRestoreLayout();
		}
		void ResotreNodes() {
			var info = Items.FirstOrDefault(x => (x is SerializableDockingContainerInfo) && (x as SerializableDockingContainerInfo).Parent == "Root") as SerializableDockingContainerInfo;
			if(info == null) return;
			RootContainer.Nodes.Clear();
			ResotreNodes(RootContainer, info);
			RemoveUnnecessaryNodesFromTree(RootContainer);
		}
		protected internal bool RemoveUnnecessaryNodesFromTree(DockingContainer root) {
			for(int i = 0; i < root.Nodes.Count; i++) {
				DockingContainer watchingNode = root.Nodes[i];
				if(RemoveUnnecessaryNodesFromTree(watchingNode) && i >= 0) i--;
			}
			if(root.Parent != null && root.Nodes.Count > 0 && root.Orientation == root.Parent.Orientation) {
				DockingContainerHelper.ReplaceContainerOnChildren(root);
				return true;
			}
			else
				return false;
		}
		void ResotreNodes(DockingContainer node, SerializableDockingContainerInfo info) {
			DockingContainer chidlNode = new DockingContainer()
			{
				Length = info.Length,
				Orientation = info.Orientation,
				TabbedViewInfo = ViewInfo as TabbedViewInfo
			};
		   var groupInfo =  Items.FirstOrDefault(x => x.Name == info.Element) as SerializableDocumentGroupInfo;
		   if(groupInfo != null) {
			   chidlNode.Element = groupInfo.Source as DocumentGroup;
		   }
			var child = Items.Select(x => x).Where(x => (x is SerializableDockingContainerInfo) && (x as SerializableDockingContainerInfo).Parent == info.Name).OrderBy(x => (x as SerializableDockingContainerInfo).Index).ToList();
			if(child.Count() != 0 || chidlNode.Element != null)
				node.Nodes.Add(chidlNode);
			foreach(SerializableDockingContainerInfo item in child) {
				ResotreNodes(chidlNode, item);
			}
			if(node.Nodes.Count == 0 && node.Element == null && node.Parent != null)
				node.Parent.Nodes.Remove(node);
			node.RepopulateSplitters();
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
		protected IDisposable CleanUpDocumentGroups() {
			IDisposable result = DocumentGroups.LockCollectionChanged();
			DocumentGroup[] groups = DocumentGroups.CleanUp();
			for(int i = 0; i < groups.Length; i++) {
				DocumentGroup group = groups[i];
				group.VisibilityChanged -= OnDocumentGroupVisibilityChanged;
				Document[] documents = group.Items.CleanUp();
				foreach(Document document in documents) {
					document.SetIsSelected(false);
					UnregisterDocumentInfo(document, group);
					UnregisterDocumentInfo(document);
				}
				UnregisterDocumentGroupInfo(group);
			}
			return result;
		}
		protected void RestoreDocumentGroups() {
			IDictionary<string, SerializableObjectInfo> itemsAndNames = new Dictionary<string, SerializableObjectInfo>();
			foreach(SerializableObjectInfo info in Items)
				itemsAndNames.Add(info.Name, info);
			List<DocumentGroup> groups = new List<DocumentGroup>();
			List<DocumentGroup> groupsToRemove = new List<DocumentGroup>();
			foreach(SerializableObjectInfo info in Items) {
				SerializableDocumentInfo dockInfo = info as SerializableDocumentInfo;
				if(dockInfo != null) {
					Document document = dockInfo.Source as Document;
					if(dockInfo.IsFloating) {
						Controller.Float(document, dockInfo.Location, dockInfo.Size);
						document.Form.WindowState = dockInfo.WindowState;
						continue;
					}
					SerializableObjectInfo gInfo;
					if(!string.IsNullOrEmpty(dockInfo.ParentName) && itemsAndNames.TryGetValue(dockInfo.ParentName, out gInfo)) {
						DocumentGroup group = gInfo.Source as DocumentGroup;
						if(document.IsFloating) {
							Control control = document.Control;
							FloatDocuments.Remove(document);
							Manager.DockFloatForm(document);
							Documents.Add(document);
							((IBaseViewControllerInternal)Controller).AddDocument(control);
						}
						else RegisterDocumentInfo(document);
						if(!groups.Contains(group)) {
							groups.Add(group);
							RegisterDocumentGroupInfo(group);
						}
						using(group.Items.LockCollectionChanged()) {
							RegisterDocumentInfo(document, group);
							group.Items.Add(document);
						}
						document.Pinned = dockInfo.Pinned;
						if(dockInfo.IsActive)
							SetRestoreActiveDocument(document, true);
						continue;
					}
					else NotRestoredDocuments.Add(document);
				}
				SerializableDocumentGroupInfo groupInfo = info as SerializableDocumentGroupInfo;
				if(groupInfo != null) {
					DocumentGroup group = groupInfo.Source as DocumentGroup;
					if(!groups.Contains(group)) {
						if(groupInfo.IsNew) {
							groups.Add(group);
							RegisterDocumentGroupInfo(group);
						}
						else groupsToRemove.Add(group);
					}
				}
			}
			RemoveEmptyGroups(groups);
			RestoreItemsOrderAndSelection(groups);
			DocumentGroups.AddRange(groups);
			UpdateDockingContainers();
			RemoveRedundantGroups(groupsToRemove);
			if(Manager.DockManager != null && Manager.DockManager.ActivePanel != null)
				Manager.DockManager.ActivePanel = null;
		}
		void UpdateDockingContainers() {
			foreach(var group in DocumentGroups) {
				if(DockingContainerHelper.GetTargetNode(group, RootContainer) == null) {
					RootContainer.AddGroup(group);
				}
			}
		}
		void RestoreGroupLengths(List<DocumentGroup> groups) {
			if(!Bounds.IsEmpty)
				(ViewInfo as TabbedViewInfo).CalcContainers();
			int[] lengths = new int[groups.Count];
			for(int i = 0; i < lengths.Length; i++) {
				SerializableObjectInfo info;
				if(Infos.TryGetValue(groups[i], out info)) {
					SerializableDocumentGroupInfo groupInfo = info as SerializableDocumentGroupInfo;
					if(groupInfo.Length == 0)
						return;
					lengths[i] = groupInfo.Length;
				}
				else return;
			}
			for(int i = 0; i < lengths.Length; i++) {
				groups[i].SetGroupLengthCore(lengths[i]);
			}
		}
		void RestoreItemsOrderAndSelection(List<DocumentGroup> groups) {
			Control hostControl = Manager.GetOwnerControl();
			if(hostControl != null && !hostControl.Visible) {
				if(selectedDocumentsDelayed == null)
					selectedDocumentsDelayed = new Dictionary<DocumentGroup, Document>();
			}
			groups.Sort(new SerializableDocumentGroupInfoIndexComparer(Infos));
			foreach(DocumentGroup group in groups) {
				group.Items.Sort(new SerializableDocumentInfoIndexComparer(Infos));
				RestoreSelectedItem(group);
				group.VisibilityChanged += OnDocumentGroupVisibilityChanged;
			}
		}
		IDictionary<DocumentGroup, Document> selectedDocumentsDelayed;
		void RestoreSelectedItem(DocumentGroup group) {
			foreach(Document document in group.Items) {
				SerializableObjectInfo dInfo;
				if(Infos.TryGetValue(document, out dInfo) && ((SerializableDocumentInfo)dInfo).IsSelected) {
					if(selectedDocumentsDelayed != null) {
						if(!selectedDocumentsDelayed.ContainsKey(group))
							selectedDocumentsDelayed.Add(group, document);
						else selectedDocumentsDelayed[group] = document;
					}
					if(selectedDocumentsDelayed == null)
						SetRestoreActiveDocument(document, false);
					group.SetSelected(document);
					return;
				}
			}
			if(group.Items.Count > 0 && group.SelectedDocument == null)
				group.SetSelected(group.Items[0]);
		}
		protected internal override void OnVisibleChanged(bool visible) {
			base.OnVisibleChanged(visible);
			if(visible && selectedDocumentsDelayed != null)
				RestoreDelayedSelection();
		}
		protected void RestoreDelayedSelection() {
			Control mdiClient = Manager.GetOwnerControl();
			if(mdiClient != null && selectedDocumentsDelayed != null) {
				foreach(KeyValuePair<DocumentGroup, Document> pair in selectedDocumentsDelayed) {
					pair.Key.SetSelected(pair.Value);
					mdiClient.BeginInvoke(new MethodInvoker(delegate() {
						DevExpress.Utils.Drawing.Helpers.NativeMethods.SendMessage(
							mdiClient.Handle, 0x0222, pair.Value.Form.Handle, IntPtr.Zero);
					}));
				}
				Ref.Clear(ref selectedDocumentsDelayed);
			}
		}
		void RemoveEmptyGroups(List<DocumentGroup> allGroups) {
			DocumentGroup[] groupArray = allGroups.ToArray();
			foreach(DocumentGroup group in groupArray) {
				if(group.Items.Count == 0 && group.Properties.ActualDestroyOnRemovingChildren) {
					UnregisterDocumentGroupInfo(group);
					allGroups.Remove(group);
					group.Dispose();
				}
			}
		}
		void RemoveRedundantGroups(List<DocumentGroup> groupsToRemove) {
			foreach(DocumentGroup group in groupsToRemove) {
				if(group.Properties.ActualDestroyOnRemovingChildren)
					group.Dispose();
			}
		}
		protected override void PrepareSerializableObjectInfos() {
			base.PrepareSerializableObjectInfos();
			PrepareSerializableDocumentGroupInfos();
		}
		protected virtual void PrepareSerializableDocumentGroupInfos() {
			PrepareSerializableDockingContainerInfos(RootContainer, "Root", 0);
			DocumentGroup[] documentGroups = DocumentGroups.ToArray();
			for(int i = 0; i < documentGroups.Length; i++) {
				DocumentGroup group = documentGroups[i];
				SerializableObjectInfo info = CreateSerializableDocumentGroupInfo(group);
				info.SetNameByIndex(i);
				RegisterSerializableObjectinfo(group, info);
				if(IsSerializing)
					UpdateChildrenParentNames(group, info);
			}
		}
		void PrepareSerializableDockingContainerInfos(DockingContainer node, string parent, int level) {
			if(IsDeserializing) return;
			SerializableDockingContainerInfo info = new SerializableDockingContainerInfo(node);
			info.Index = node.Parent != null ? node.Parent.Nodes.IndexOf(node) : 0;
			info.Length = node.Length;
			info.Parent = parent;
			info.Orientation = node.Orientation;
			info.SetName(node.GetHashCode().ToString());
			if(node.Element != null) {
				info.Element = "DocumentGroup" + DocumentGroups.IndexOf(node.Element as DocumentGroup).ToString();
			}
			RegisterSerializableObjectinfo(node, info);
			DockingContainer[] nodes = node.Nodes.ToArray();
			foreach(var item in nodes) {
				PrepareSerializableDockingContainerInfos(item, info.Name, level + 1);
			}
		}
		void UpdateChildrenParentNames(DocumentGroup group, SerializableObjectInfo info) {
			foreach(Document document in group.Items) {
				SerializableObjectInfo documentInfo;
				if(Infos.TryGetValue(document, out documentInfo)) {
					((SerializableDocumentInfo)documentInfo).ParentName = info.Name;
				}
			}
		}
		protected override BaseSerializableDocumentInfo CreateSerializableDocumentInfo(BaseDocument document) {
			return new SerializableDocumentInfo(document as Document);
		}
		protected virtual SerializableDocumentGroupInfo CreateSerializableDocumentGroupInfo(DocumentGroup group) {
			return new SerializableDocumentGroupInfo(group, DocumentGroups.IndexOf(group));
		}
		protected override SerializableObjectInfo CreateSerializableObjectInfo(string typeName) {
			switch(typeName) {
				case "DocumentGroup":
					DocumentGroup group = CreateDocumentGroup();
					group.SetManager(Manager);
					group.IsLoaded = true;
					AddToContainer(group);
					SerializableDocumentGroupInfo result = CreateSerializableDocumentGroupInfo(group);
					result.IsNew = true;
					RegisterSerializableObjectinfo(group, result);
					return result;
				case "DockingContainer":
					var info = new SerializableDockingContainerInfo(null);
					AddSerializableObjectinfo(info);
					return info;
			}
			return base.CreateSerializableObjectInfo(typeName);
		}
		protected class SerializableDocumentGroupInfo : SerializableObjectInfo {
			IDocumentGroupDefaultProperties propertiesCore;
			public SerializableDocumentGroupInfo(DocumentGroup group, int index)
				: base(group) {
				propertiesCore = group.Properties;
				lengthCore = group.GroupLength;
				indexCore = index;
			}
			protected override string GetTypeNameCore() {
				return "DocumentGroup";
			}
			[XtraSerializableProperty(XtraSerializationVisibility.Content)]
			public IDocumentGroupDefaultProperties Properties {
				get { return propertiesCore; }
			}
			int lengthCore;
			[XtraSerializableProperty]
			public int Length {
				get { return lengthCore; }
				set { lengthCore = value; }
			}
			bool isNewCore;
			public bool IsNew {
				get { return isNewCore; }
				internal set { isNewCore = value; }
			}
			int indexCore;
			[XtraSerializableProperty]
			public int Index {
				get { return indexCore; }
				set { indexCore = value; }
			}
		}
		protected class SerializableDockingContainerInfo : SerializableObjectInfo {
			DockingContainer nodeCore;
			public SerializableDockingContainerInfo(DockingContainer node)
				: base(node) {
				nodeCore = node;
				Length = new Widget.Length();
			}
			[XtraSerializableProperty]
			public string Parent { get; set; }
			[XtraSerializableProperty]
			public string Element { get; set; }
			[XtraSerializableProperty]
			public Orientation Orientation { get; set; }
			[XtraSerializableProperty]
			public int Index { get; set; }
			[XtraSerializableProperty(XtraSerializationVisibility.Content)]
			public DevExpress.XtraBars.Docking2010.Views.Widget.Length Length { get; set; }
			protected override string GetTypeNameCore() {
				return "DockingContainer";
			}
		}
		protected class SerializableDocumentInfo : BaseSerializableDocumentInfo {
			public SerializableDocumentInfo(Document document)
				: base(document) {
				if(!IsFloating) {
					isSelectedCore = document.IsSelected;
					indexCore = document.Parent.Items.IndexOf(document);
					if(document.Form != null)
						windowStateCore = document.Form.WindowState;
				}
				pinnedCore = document.Pinned;
			}
			string parentNameCore;
			[XtraSerializableProperty]
			public string ParentName {
				get { return parentNameCore; }
				set { parentNameCore = value; }
			}
			bool isSelectedCore;
			[XtraSerializableProperty]
			public bool IsSelected {
				get { return isSelectedCore; }
				set { isSelectedCore = value; }
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
			bool pinnedCore;
			[XtraSerializableProperty]
			public bool Pinned {
				get { return pinnedCore; }
				set { pinnedCore = value; }
			}
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
		class SerializableDocumentGroupInfoIndexComparer : IComparer<DocumentGroup> {
			IDictionary<object, SerializableObjectInfo> infos;
			public SerializableDocumentGroupInfoIndexComparer(IDictionary<object, SerializableObjectInfo> infos) {
				this.infos = infos;
			}
			public int Compare(DocumentGroup x, DocumentGroup y) {
				if(x == y) return 0;
				SerializableObjectInfo xInfo; infos.TryGetValue(x, out xInfo);
				SerializableObjectInfo yInfo; infos.TryGetValue(y, out yInfo);
				return ((SerializableDocumentGroupInfo)xInfo).Index.CompareTo(((SerializableDocumentGroupInfo)yInfo).Index);
			}
		}
		#endregion XtraSerializable
		#region SysCommand handler
		protected internal override bool OnSCMaximize(IntPtr hWnd) {
			if(Documents.GetDocument(hWnd) != null)
				return base.OnSCMaximize(hWnd);
			return false;
		}
		protected internal override bool OnSCRestore(IntPtr hWnd) {
			if(Documents.GetDocument(hWnd) != null)
				return base.OnSCMaximize(hWnd);
			return false;
		}
		#endregion SysCommand handler
		protected internal override void RegisterListeners(DragEngine.BaseUIView uiView) {
			uiView.RegisterUIServiceListener(new Dragging.DocumentManagerUIViewRegularDragListener());
			uiView.RegisterUIServiceListener(new Dragging.DocumentManagerUIViewReorderingListener());
			uiView.RegisterUIServiceListener(new Dragging.DocumentManagerUIViewResizingListener());
			uiView.RegisterUIServiceListener(new Dragging.DocumentManagerUIViewDockingListener());
			uiView.RegisterUIServiceListener(new Dragging.DocumentManagerUIViewFloatingDragListener());
			uiView.RegisterUIServiceListener(new Dragging.DocumentManagerUIViewUIInteractionListener());
#if DEBUGTEST
			uiView.RegisterUIServiceListener(new Dragging.DocumentManagerUIViewUIInteractionListenerForResizeAssistent());
#endif
		}
		protected internal override bool AllowShowThumbnailsInTaskBar { get { return true; } }
#region IThumbnailViewClient Members
		DocumentGroup GetGroupFromDocument(BaseDocument document) {
			foreach(DocumentGroup group in DocumentGroups) {
				if(group.Items.Contains(document as Document))
					return group;
			}
			return null;
		}
		void IThumbnailViewClient.RedrawThumbnailChild(BaseDocument document) {
			if(VisibleDocumentGroups == null || VisibleDocumentGroups.Count <= 0) return;
			DocumentGroup group = GetGroupFromDocument(document);
			if(group == null || !VisibleDocumentGroups.Contains(group)) return;
			for(int i = 0; i < VisibleDocumentGroups.Count; i++) {
				if(group == VisibleDocumentGroups[i]) {
					Rectangle clientRect = ViewInfo.VisibleMdiChildren[i];
					Control control = Manager.GetChild(document);
					if(control.Bounds == clientRect) return;
					group.RedrawThumbnailChild(control, clientRect.Size, Manager.Bounds);
				}
			}
		}
#endregion
	}
}
