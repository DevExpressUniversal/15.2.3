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
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraTab;
namespace DevExpress.XtraBars.Docking2010.Views.Tabbed {
	[ToolboxItem(false)]
	public class Document : BaseDocument {
		IDocumentInfo infoCore;
		DocumentGroup parentCore;
		IPageAppearanceProvider appearanceProviderCore;
		TabbedViewPageAppearance appearanceCore;
		public Document() { }
		public Document(IContainer container)
			: base(container) {
		}
		public Document(IDocumentProperties parentProperties)
			: base(parentProperties) {
		}
		protected override void OnCreate() {
			base.OnCreate();
			appearanceProviderCore = CreateAppearanceProvider();
			appearanceCore = CreatePageAppearance();
			AppearanceProvider.Changed += OnAppearanceChanged;
		}
		protected override void LockComponentBeforeDisposing() {
			base.LockComponentBeforeDisposing();
			AppearanceProvider.Changed -= OnAppearanceChanged;
		}
		protected override Base.IComponentLayoutChangedTracker GetViewLayoutChangedTracker() {
			return (Info != null && Info.Owner != null) ? Info.Owner.TrackLayoutChanged() : base.GetViewLayoutChangedTracker();
		}
		protected override void OnDispose() {
			isSelectedCore = false;
			Ref.Dispose(ref appearanceCore);
			Ref.Dispose(ref appearanceProviderCore);
			Ref.Dispose(ref infoCore);
			base.OnDispose();
			parentCore = null;
		}
		protected internal override bool CanChangeControl() {
			bool canLoad = base.CanLoadControl();
			if(!canLoad && this.IsDockPanel && DesignMode)
				return true;
			return canLoad;
		}
		protected override bool CalcIsEnabled() {
			return (Manager != null && Manager.MdiParent != null && DesignMode) ? false : base.CalcIsEnabled();
		}
		protected virtual IPageAppearanceProvider CreateAppearanceProvider() {
			return new DocumentPageAppearanceProvider();
		}
		protected virtual TabbedViewPageAppearance CreatePageAppearance() {
			return new TabbedViewPageAppearance(AppearanceProvider);
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DocumentAppearance"),
#endif
 Category("Appearance"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TabbedViewPageAppearance Appearance {
			get { return appearanceCore; }
		}
		protected IPageAppearanceProvider AppearanceProvider {
			get { return appearanceProviderCore; }
		}
		bool ShouldSerializeAppearance() {
			return Appearance != null && Appearance.ShouldSerialize();
		}
		void ResetAppearance() {
			Appearance.Reset();
		}
		void OnAppearanceChanged(object sender, EventArgs e) {
			if(IsDisposing) return;
			IXtraTab tab = (Parent != null) ? Parent.GetTab() : null;
			if(tab != null && Info != null)
				tab.OnPageChanged(Info.TabPage);
		}
		protected override void OnImageChanged() {
			StopAnimation();
			StartAnimation();
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("DocumentProperties")]
#endif
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new IDocumentDefaultProperties Properties {
			get { return base.Properties as IDocumentDefaultProperties; }
		}
		protected override IBaseDocumentDefaultProperties CreateDefaultProperties(IBaseDocumentProperties parentProperties) {
			return new DocumentDefaultProperties(parentProperties as IDocumentProperties);
		}
		protected override void InitializePropertiesFromDockPanelCore(Docking.DockPanel panel) {
			base.InitializePropertiesFromDockPanelCore(panel);
			Properties.AllowFloatOnDoubleClick = CanFloatOnDoubleClick(panel) ? DevExpress.Utils.DefaultBoolean.True : DevExpress.Utils.DefaultBoolean.False;
		}
		protected override void UpdateDockPanelPropertiesCore(PropertyChangedEventArgs e, Docking.DockPanel dockPanel) {
			base.UpdateDockPanelPropertiesCore(e, dockPanel);
			if(e.PropertyName == "AllowFloatOnDoubleClick")
				dockPanel.Options.FloatOnDblClick = Properties.AllowFloatOnDoubleClick != DevExpress.Utils.DefaultBoolean.False;
		}
		protected override bool CanFloatOnDoubleClickCore() {
			return base.CanFloatOnDoubleClickCore() && Properties.CanFloatOnDoubleClick;
		}
		[Browsable(false)]
		public DocumentGroup Parent {
			get { return parentCore; }
		}
		[Browsable(false)]
		bool isSelectedCore;
		[Browsable(false)]
		public bool IsSelected {
			get { return isSelectedCore; }
		}
		protected internal IDocumentInfo Info {
			get { return infoCore; }
		}
		bool isAnimatedCore;
		[Browsable(false)]
		public bool IsAnimated {
			get { return isAnimatedCore; }
			internal set { isAnimatedCore = value; }
		}
		bool pinnedCore = false;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DocumentPinned"),
#endif
 Category("Layout"), DefaultValue(false)]
		public bool Pinned {
			get { return pinnedCore; }
			set { SetValue(ref pinnedCore, value, OnPinnedStateChanged); }
		}
		protected void OnPinnedStateChanged() {
			if(Parent != null) {
				Parent.SortPinnedItems();
				if(Pinned) Parent.RaisePinned(this);
				else Parent.RaiseUnpinned(this);
			}
		}
		public void StartAnimation() {
			if(Info != null)
				Info.StartAnimation();
		}
		public void StopAnimation() {
			if(Info != null)
				Info.StopAnimation();
		}
		protected internal void SetInfo(IDocumentInfo info) {
			infoCore = info;
		}
		bool IsDeserializing { get { return !IsDisposing && this.Info != null && this.Info.Owner.IsDeserializing; } }
		internal void SetParent(DocumentGroup parent) {
			if(parentCore == parent) return;
			parentCore = parent;
			if(parent == null && !IsDeserializing)
				isSelectedCore = false;
			OnParentChanged();
		}
		internal void SetIsSelected(bool value) {
			if(isSelectedCore == value) return;
			isSelectedCore = value;
			OnIsSelectedChanged();
		}
		string GetTextMdiTab(Control control) {
			DevExpress.XtraEditors.XtraForm xtraForm = control as DevExpress.XtraEditors.XtraForm;
			if(xtraForm != null && !string.IsNullOrEmpty(xtraForm.TextMdiTab))
				return xtraForm.TextMdiTab;
			return string.Empty;
		}
		protected internal override string GetCaptionFromControl(Control control) {
			string result = GetTextMdiTab(control);
			if(!string.IsNullOrEmpty(result)) return result;
			return base.GetCaptionFromControl(control);
		}
		protected override string InitializeCaptionFromForm() {
			string result = GetTextMdiTab(Form);
			if(!string.IsNullOrEmpty(result)) return result;
			return base.InitializeCaptionFromForm();
		}
		protected virtual void OnIsSelectedChanged() {
			RaiseIsSelectedChanged();
		}
		protected virtual void OnParentChanged() {
			RaiseParentChanged();
		}
		#region Events
		static readonly object parentChangedCore = new object();
		static readonly object isSelectedChangedCore = new object();
		[Category(DevExpress.XtraEditors.CategoryName.Layout)]
		public event DocumentEventHandler ParentChanged {
			add { Events.AddHandler(parentChangedCore, value); }
			remove { Events.RemoveHandler(parentChangedCore, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public event DocumentEventHandler IsSelectedChanged {
			add { Events.AddHandler(isSelectedChangedCore, value); }
			remove { Events.RemoveHandler(isSelectedChangedCore, value); }
		}
		protected void RaiseIsSelectedChanged() {
			DocumentEventHandler handler = (DocumentEventHandler)Events[isSelectedChangedCore];
			if(handler != null) handler(this, new DocumentEventArgs(this));
		}
		protected void RaiseParentChanged() {
			DocumentEventHandler handler = (DocumentEventHandler)Events[parentChangedCore];
			if(handler != null) handler(this, new DocumentEventArgs(this));
		}
		#endregion Events
		protected internal IXtraTabPage GetTabPage() {
			return Info.TabPage;
		}
		protected override void OnLayoutChanged() {
			TabChanged();
			base.OnLayoutChanged();
		}
		protected override void OnControlShown() {
			RequestTabChanged();
			base.OnControlShown();
		}
		protected override void OnControlHidden() {
			RequestTabChanged();
			base.OnControlHidden();
		}
		protected override DevExpress.Utils.AppearanceObject GetActiveDocumentCaptionAppearance() {
			return Appearance.HeaderActive;
		}
		protected override DevExpress.Utils.AppearanceObject GetDocumentCaptionAppearance() {
			return Appearance.Header;
		}
		protected void RequestTabChanged() {
			if(Info == null || Info.Owner == null) return;
			if(Info.Owner.IsUpdateLocked) return;
			TabChanged();
		}
		protected void TabChanged() {
			IXtraTab tab = (Parent != null) ? Parent.GetTab() : null;
			if(tab != null) tab.LayoutChanged();
		}
		#region Tooltip
		string tooltipCore;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DocumentTooltip"),
#endif
 Category("Tooltip")]
		[DefaultValue(null), Localizable(true), Editor(DevExpress.Utils.ControlConstants.MultilineStringEditor, typeof(System.Drawing.Design.UITypeEditor))]
		public virtual string Tooltip {
			get { return tooltipCore; }
			set { tooltipCore = value; }
		}
		DevExpress.Utils.SuperToolTip superTipCore;
		bool ShouldSerializeSuperTip() { return SuperTip != null && !SuperTip.IsEmpty; }
		void ResetSuperTip() { SuperTip = null; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DocumentSuperTip"),
#endif
 Category("Tooltip")]
		[Localizable(true), Editor("DevExpress.XtraEditors.Design.ToolTipContainerUITypeEditor, " + AssemblyInfo.SRAssemblyEditorsDesign, typeof(System.Drawing.Design.UITypeEditor))]
		public virtual DevExpress.Utils.SuperToolTip SuperTip {
			get { return superTipCore; }
			set { superTipCore = value; }
		}
		string tooltipTitleCore;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DocumentTooltipTitle"),
#endif
 Category("Tooltip")]
		[Localizable(true), DefaultValue(null)]
		public string TooltipTitle {
			get { return tooltipTitleCore; }
			set { tooltipTitleCore = value; }
		}
		DevExpress.Utils.ToolTipIconType toolTipIconTypeCore = DevExpress.Utils.ToolTipIconType.None;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DocumentTooltipIconType"),
#endif
 Category("Tooltip")]
		[Localizable(true), DefaultValue(DevExpress.Utils.ToolTipIconType.None)]
		public DevExpress.Utils.ToolTipIconType TooltipIconType {
			get { return toolTipIconTypeCore; }
			set { toolTipIconTypeCore = value; }
		}
		#endregion Tooltip
	}
	public class DocumentCollection : BaseDocumentCollection<Document, DocumentGroup> {
		public DocumentCollection(DocumentGroup owner)
			: base(owner) {
		}
		protected override void OnElementAdded(Document element) {
			element.SetParent(Owner);
			base.OnElementAdded(element);
		}
		Document selectedDocument;
		protected override void OnBeforeElementRemoved(Document element) {
			base.OnBeforeElementRemoved(element);
			selectedDocument = (Owner != null) ? Owner.SelectedDocument : null;
		}
		protected override void OnElementRemoved(Document element) {
			element.SetParent(null);
			if(element != selectedDocument && selectedDocument != null && Owner != null)
				Owner.SetSelected(selectedDocument);
			selectedDocument = null;
			base.OnElementRemoved(element);
		}
		protected override bool CanAdd(Document element) {
			return !Owner.IsFilledUp && base.CanAdd(element);
		}
		protected override void NotifyOwnerOnInsert(int index) {
			Owner.OnInsert(index);
		}
		protected override void NotifyOwnerOnMove(int index, Document element) {
			Owner.SortPinnedItems();
		}
	}
}
