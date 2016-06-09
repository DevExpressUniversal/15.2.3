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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Base;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.XtraBars.Docking.Helpers;
using DevExpress.XtraBars.Docking2010.Base;
using DevExpress.XtraEditors.ButtonPanel;
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	public interface IContentContainer : IBaseObject, IComponent, ISupportBatchUpdate, INamed {
		[Browsable(false)]
		DocumentManager Manager { get; }
		IContentContainerDefaultProperties Properties { get; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		IContentContainerInfo Info { get; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		bool IsAutoCreated { get; }
		bool IsActive { get; }
		Image Image { get; set; }
		string Caption { get; set; }
		string Subtitle { get; set; }
		object Tag { get; set; }
		AppearanceObject AppearanceSubtitle { get; }
		IContentContainer Parent { get; set; }
		IEnumerable<IContentContainerAction> Actions { get; }
		IEnumerable<IButton> Buttons { get; }
		bool Contains(Document document);
		IEnumerator<IContentContainer> GetEnumerator();
		IEnumerator<IContentContainer> GetEnumerator(int level);
	}
	public interface IDocumentContainer : IContentContainer {
		Document Document { get; }
	}
	public interface IDocumentSelector : IContentContainer {
		Document SelectedDocument { get; }
		void SetSelected(Document document);
	}
	internal interface IContentContainerInternal : IContentContainer, ICustomButtonsOwner, IContainerRemove {
		bool IsLoaded { get; set; }
		void EnsureParentProperties(WindowsUIView view);
		void SetManager(DocumentManager manager);
		void SetIsAutoCreated(bool value);
		void Activate(WindowsUIView view);
		void Deactivate();
		void PatchChildren(Rectangle view, bool active);
		int Count { get; }
		Document[] GetDocuments();
		IList<IContentContainer> Children { get; }
		IEnumerable<IContentContainerAction> GetActualActions();
		event EventHandler ActualActionsChanged;
		IContentContainer GetDetailContainer(BaseDocument document);
		IContentContainer GetOverviewContainer();
		ContextualZoomLevel ZoomLevel { get; }
		void ActivateDocument(Document document);
		void RequestContextActionBarActivation();
		void UpdateDocumentActions();
	}
	public interface IContentContainerProperties : IBaseProperties {
		Orientation Orientation { get; set; }
		Padding Margin { get; set; }
		int Capacity { get; set; }
		int HeaderOffset { get; set; }
		bool ShowCaption { get; set; }
		bool DestroyOnRemovingChildren { get; set; }
		bool ShowContextActionBarOnActivating { get; set; }
	}
	public interface IContentContainerDefaultProperties : IBaseDefaultProperties {
		Orientation? Orientation { get; set; }
		Padding? Margin { get; set; }
		int? Capacity { get; set; }
		int? HeaderOffset { get; set; }
		DevExpress.Utils.DefaultBoolean ShowCaption { get; set; }
		DevExpress.Utils.DefaultBoolean DestroyOnRemovingChildren { get; set; }
		DevExpress.Utils.DefaultBoolean ShowContextActionBarOnActivating { get; set; }
		Orientation ActualOrientation { get; }
		int ActualCapacity { get; }
		bool ActualDestroyOnRemovingChildren { get; }
		bool CanShowContextActionBarOnActivating { get; }
		bool CanShowCaption { get; }
		bool HasMargin { get; }
		Padding ActualMargin { get; }
		bool HasHeaderOffset { get; }
		int ActualHeaderOffset { get; }
	}
	public interface ICustomButtonsOwner {
		WindowsUIButtonsPanel ButtonsPanel { get; }
	}
	[ToolboxItem(false), DesignTimeVisible(false)]
	public abstract class BaseContentContainer : BaseComponent, IContentContainerInternal, DevExpress.XtraEditors.ButtonsPanelControl.IButtonPanelControlAppearanceOwner, IWindowsUIButtonPanelOwner, Customization.ISearchContainer {
		DocumentManager managerCore;
		IContentContainerDefaultProperties propertiesCore;
		IContentContainerDefaultProperties detailContainerPropertiesCore;
		IContentContainerInfo infoCore;
		ContentContainerActionCollection actionsCore;
		DevExpress.XtraEditors.ButtonsPanelControl.ButtonsPanelControlAppearance appearanceButtonCore;
		WindowsUIButtonsPanel buttonsPanelCore;
		object buttonBackgroundImagesCore;
		protected BaseContentContainer(IContainer container)
			: base(container) {
			InitProperties(null);
		}
		protected BaseContentContainer(IContentContainerProperties defaultProperties)
			: base(null) {
			InitProperties(defaultProperties);
		}
		protected override void OnCreate() {
			base.OnCreate();
			childrenCore = new List<IContentContainer>();
			actionsCore = CreateActions();
			Actions.CollectionChanged += OnActionsCollectionChanged;
			appearanceSubtitleCore = new AppearanceObject();
			AppearanceSubtitle.Changed += OnAppearanceSubtitleChanged;
			appearanceButtonCore = new XtraEditors.ButtonsPanelControl.ButtonsPanelControlAppearance(this);
			AppearanceButton.Changed += OnAppearanceButtonChanged;
			buttonsPanelCore = CreateButtonsPanel();
			SubscribeButtonPanel();
		}
		protected override void LockComponentBeforeDisposing() {
			base.LockComponentBeforeDisposing();
			UnsubscribeButtonPanel();
			Properties.Changed -= OnPropertiesChanged;
			AppearanceSubtitle.Changed -= OnAppearanceSubtitleChanged;
			AppearanceButton.Changed -= OnAppearanceButtonChanged;
			Actions.CollectionChanged -= OnActionsCollectionChanged;
			Ref.Dispose(ref infoCore);
		}
		protected override void OnDispose() {
			Ref.Dispose(ref propertiesCore);
			Ref.Dispose(ref actionsCore);
			UnsubscribeButtonPanel();
			Ref.Dispose(ref buttonsPanelCore);
			base.OnDispose();
			childrenCore.Clear();
			Ref.Dispose(ref appearanceSubtitleCore);
			Ref.Dispose(ref appearanceButtonCore);
			managerCore = null;
		}
		protected WindowsUIButtonsPanel CreateButtonsPanel() {
			return new CustomHeaderButtonsPanel(this);
		}
		protected void SubscribeButtonPanel() {
			ButtonsPanel.ContentAlignment = ContentAlignment.MiddleRight;
			ButtonsPanel.ButtonInterval = 21;
			ButtonsPanel.Changed += OnButtonsPanelChanged;
			ButtonsPanel.ButtonClick += OnButtonClick;
			ButtonsPanel.ButtonChecked += OnButtonChecked;
			ButtonsPanel.ButtonUnchecked += OnButtonUnchecked;
		}
		protected void UnsubscribeButtonPanel() {
			ButtonsPanel.Changed -= OnButtonsPanelChanged;
			ButtonsPanel.ButtonClick -= OnButtonClick;
			ButtonsPanel.ButtonChecked -= OnButtonChecked;
			ButtonsPanel.ButtonUnchecked -= OnButtonUnchecked;
		}
		void OnButtonsPanelChanged(object sender, EventArgs e) {
			LayoutChanged();
		}
		protected IContentContainerDefaultProperties DetailContainerPropertiesCore {
			get { return detailContainerPropertiesCore; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string Name { get; set; }
		string captionCore;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseContentContainerCaption"),
#endif
		DefaultValue(null), Category("Appearance"), SmartTagProperty("Caption", ""), Localizable(true)]
		public virtual string Caption {
			get { return captionCore; }
			set { SetValue(ref captionCore, value); }
		}
		string subtitleCore;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseContentContainerSubtitle"),
#endif
		DefaultValue(null), Category("Appearance"), SmartTagProperty("Subtitle", ""), Localizable(true)]
		public virtual string Subtitle {
			get { return subtitleCore; }
			set { SetValue(ref subtitleCore, value); }
		}
		AppearanceObject appearanceSubtitleCore;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseContentContainerAppearanceSubtitle"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]
		public virtual AppearanceObject AppearanceSubtitle {
			get { return appearanceSubtitleCore; }
		}
		bool ShouldSerializeAppearanceSubtitle() {
			return !IsDisposing && AppearanceSubtitle.ShouldSerialize();
		}
		void ResetAppearanceSubtitle() {
			AppearanceSubtitle.Reset();
		}
		void OnAppearanceSubtitleChanged(object sender, EventArgs e) {
			LayoutChanged();
		}
		Image imageCore;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseContentContainerImage"),
#endif
		DefaultValue(null), Category("Appearance"), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(System.Drawing.Design.UITypeEditor))]
		public virtual Image Image {
			get { return imageCore; }
			set { SetValue(ref imageCore, value); }
		}
		DxImageUri imageUriCore = new DxImageUri();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseContentContainerImageUri"),
#endif
 Category("Appearance"), DefaultValue(null)]
		[TypeConverter(typeof(ExpandableObjectConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DxImageUri ImageUri {
			get { return imageUriCore; }
			set { SetValue(ref imageUriCore, value); }
		}
		protected internal Image GetActualImage() {
			if(ImageUri != null && ImageUri.HasImage)
				return ImageUri.GetImage();
			return Image;
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public ContentContainerActionCollection Actions {
			get { return actionsCore; }
		}
		protected virtual ContentContainerActionCollection CreateActions() {
			return new ContentContainerActionCollection(this);
		}
		void OnActionsCollectionChanged(DevExpress.XtraBars.Docking2010.Base.CollectionChangedEventArgs<IContentContainerAction> ea) {
			LayoutChanged();
		}
		protected abstract IContentContainerInfo CreateContentContainerInfo(WindowsUIView view);
		void InitProperties(IContentContainerProperties parentProperties) {
			propertiesCore = CreateDefaultProperties(parentProperties);
			detailContainerPropertiesCore = CreateDetailContainerProperties();
			Properties.Changed += OnPropertiesChanged;
		}
		protected virtual IContentContainerDefaultProperties CreateDetailContainerProperties() {
			return new DetailContainerDefaultProperties(null);
		}
		void OnPropertiesChanged(object sender, EventArgs e) {
			PropertyChangedEventArgs pea = e as PropertyChangedEventArgs;
			if(pea != null)
				OnPropertiesPropertyChanged(pea.PropertyName);
			LayoutChanged();
		}
		protected virtual void OnPropertiesPropertyChanged(string propertyName) { }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseContentContainerProperties"),
#endif
 Category("Properties"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public IContentContainerDefaultProperties Properties {
			get { return propertiesCore; }
		}
		object tagCore;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseContentContainerTag"),
#endif
 DefaultValue(null), Category("Data"),
		Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))]
		public object Tag {
			get { return tagCore; }
			set { tagCore = value; }
		}
		bool ShouldSerializeProperties() {
			return !IsDisposing && Properties != null && Properties.ShouldSerialize();
		}
		void ResetProperties() {
			Properties.Reset();
		}
		protected abstract IContentContainerDefaultProperties CreateDefaultProperties(IContentContainerProperties parentProperties);
		[Browsable(false)]
		public DocumentManager Manager {
			get { return managerCore; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public IContentContainerInfo Info {
			get { return infoCore; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsAutoCreated { get; private set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsLoaded { get; private set; }
		ContextualZoomLevel IContentContainerInternal.ZoomLevel { get { return GetZoomLevel(); } }
		protected virtual ContextualZoomLevel GetZoomLevel() { return ContextualZoomLevel.Normal; }
		void IContainerRemove.ContainerRemoved() {
			ClearParentOnContainerRemoved();
		}
		bool IContentContainerInternal.IsLoaded {
			get { return IsLoaded; }
			set { IsLoaded = value; }
		}
		void IContentContainerInternal.SetManager(DocumentManager manager) {
			managerCore = manager;
		}
		void IContentContainerInternal.SetIsAutoCreated(bool value) {
			IsAutoCreated = value;
		}
		IContentContainer parentCore;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseContentContainerParent"),
#endif
 DefaultValue(null), Category("Layout")]
		public IContentContainer Parent {
			get { return parentCore; }
			set {
				if(parentCore == value || IsChild(value, this) || value == this) return;
				if(Parent != null)
					((IContentContainerInternal)Parent).Children.Remove(this);
				parentCore = value;
				if(Parent != null)
					((IContentContainerInternal)Parent).Children.Add(this);
				RaiseHierarchyChanged();
				LayoutChanged();
			}
		}
		protected internal void ClearParentOnContainerRemoved() {
			TileContainer tileContainer = parentCore as TileContainer;
			Parent = null;
			if(tileContainer != null) {
				if(tileContainer.ActivationTarget == this)
					tileContainer.ActivationTarget = null;
				if(tileContainer.Items != null) {
					foreach(Tile tile in tileContainer.Items)
						if(tile.ActivationTarget == this) tile.ActivationTarget = null;
				}
			}
		}
		IList<IContentContainer> childrenCore;
		IList<IContentContainer> IContentContainerInternal.Children {
			get { return childrenCore; }
		}
		public static IContentContainer GetRoot(IContentContainer container) {
			if(container == null) return null;
			if(container.Parent == null) return container;
			IContentContainer root = container;
			while(root.Parent != null)
				root = root.Parent;
			return root;
		}
		public static bool IsChild(IContentContainer container, IContentContainer parent) {
			if(container != null && parent != null) {
				IContentContainer node = container.Parent;
				while(node != null) {
					if(node == parent) return true;
					node = node.Parent;
				}
			}
			return false;
		}
		protected override void OnLayoutChanged() {
			base.OnLayoutChanged();
			if(Manager != null)
				Manager.LayoutChanged();
		}
		protected void RaiseHierarchyChanged() {
			WindowsUIView view = GetView();
			if(view != null)
				view.RaiseHierarchyChanged();
		}
		protected WindowsUIView GetView() {
			if(Info != null && Info.Owner != null)
				return Info.Owner as WindowsUIView;
			return (Manager != null) ?
				Manager.View as WindowsUIView : null;
		}
		[Browsable(false)]
		public bool IsHorizontal {
			get { return Properties.ActualOrientation == Orientation.Horizontal; }
		}
		IEnumerable<IContentContainerAction> IContentContainer.Actions {
			get { return Actions; }
		}
		IEnumerable<IButton> IContentContainer.Buttons {
			get {
				foreach(IButton button in ButtonsPanel.Buttons) {
					yield return button;
				}
			}
		}
		bool IContentContainer.Contains(Document document) {
			return ContainsCore(document);
		}
		int IContentContainerInternal.Count { get { return Count; } }
		Document[] IContentContainerInternal.GetDocuments() {
			return GetDocumentsCore();
		}
		void IContentContainerInternal.ActivateDocument(Document document) {
			ActivateDocumentCore(document);
		}
		protected virtual void ActivateDocumentCore(Document document) { }
		protected abstract bool ContainsCore(Document document);
		protected abstract Document[] GetDocumentsCore();
		protected abstract int Count { get; }
		[Browsable(false)]
		public bool IsFilledUp {
			get {
				int capacity = Properties.ActualCapacity;
				return capacity > 0 && Count >= capacity;
			}
		}
		protected internal bool IsDesignMode {
			get { return Manager != null && Manager.Site != null && Manager.Site.DesignMode; }
		}
		protected virtual bool CanDestroyAutomatically {
			get { return Count == 0 && !IsDesignMode && (IsAutoCreated || Properties.ActualDestroyOnRemovingChildren); }
		}
		protected bool CheckDestroyAutomatically() {
			if(IsLoaded && CanDestroyAutomatically) {
				if(IsActive)
					DeactivateCore();
				Dispose();
			}
			return IsDisposing;
		}
		[Browsable(false)]
		public bool IsActive {
			get { return infoCore != null; }
		}
		void IContentContainerInternal.Activate(WindowsUIView view) {
			OnBeforeActivate(view);
			infoCore = CreateContentContainerInfo(view);
			ColoredElementsCache.Reset();
			OnActivated();
			Buttons.UpdateButtonsVisibility();
		}
		void IContentContainerInternal.Deactivate() {
			if(IsActive)
				DeactivateCore();
			ColoredElementsCache.Reset();
		}
		int deactivating = 0;
		void DeactivateCore() {
			if(deactivating > 0) return;
			deactivating++;
			var view = GetView();
			OnBeforeDeactivate(view);
			bool deferredNavigation = (this is Flyout && view.ActiveContentContainer == null);
			if(!deferredNavigation)
				NotifyNavigatedFrom();
			ReleaseDeferredControlLoadDocuments();
			if(deferredNavigation)
				NotifyNavigatedFrom();
			OnDeactivated();
			Ref.Dispose(ref infoCore);
			deactivating--;
		}
		int contextActionBarActivationRequest;
		void IContentContainerInternal.RequestContextActionBarActivation() {
			contextActionBarActivationRequest++;
		}
		void IContentContainerInternal.PatchChildren(Rectangle view, bool active) {
			EnsureDeferredControlLoadDocuments();
			WindowsUIView owner = GetView();
			NotifyNavigatedTo();
			if(owner.retryPatchActiveChildrenFlag) {
				if(owner.ViewInfo != null)
					owner.ViewInfo.SetDirty();
				return;
			}
			if(!IsDisposing && CanUpdateActionsOnActivation()) {
				if(documentForDelayedButtonsMerging != null) {
					var actionsArgs = owner.GetDocumentActions(this, (Document)owner.ActiveDocument);
					OnDocumentActionsLoaded(actionsArgs);
					Buttons.Merge(actionsArgs.DocumentActions);
					if(owner.ViewInfo != null) {
						owner.ViewInfo.SetDirty();
						owner.Invalidate(Info.Header);
					}
					this.documentForDelayedButtonsMerging = null;
				}
				if(contextActionBarActivationRequest > 0) {
					try {
						owner.contentContainerContextActionBarActivating++;
						owner.ShowNavigationAdorner();
					}
					finally { owner.contentContainerContextActionBarActivating--; }
					contextActionBarActivationRequest = 0;
				}
			}
			PatchChildrenCore(view, active);
		}
		protected virtual void OnDocumentActionsLoaded(IDocumentActionsArgs actionsArgs) { }
		protected virtual bool CanUpdateActionsOnActivation() {
			return IsLoaded;
		}
		void IContentContainerInternal.EnsureParentProperties(WindowsUIView view) {
			EnsureParentPropertiesCore(view);
		}
		protected virtual void EnsureParentPropertiesCore(WindowsUIView view) {
			Properties.EnsureParentProperties(GetParentProperties(view));
			DetailContainerPropertiesCore.EnsureParentProperties(GetDetailContainerParentProperties(view));
		}
		IEnumerable<IContentContainerAction> IContentContainerInternal.GetActualActions() {
			var actions = new List<IContentContainerAction>(Actions);
			GetActualActionsCore(actions);
			OnActionCustomization(actions);
			var result = actions.OrderBy((x => x), new WindowsUI.ContentContainerAction.ContentContainerActionComparer(this));
			return result.ToArray();
		}
		IContentContainer IContentContainerInternal.GetDetailContainer(BaseDocument document) {
			IContentContainer detailContainer = CreateDetailContainer((Document)document);
			detailContainer.Properties.EnsureParentProperties(detailContainerPropertiesCore.ParentProperties);
			detailContainer.Properties.Assign(detailContainerPropertiesCore);
			return detailContainer;
		}
		IContentContainer IContentContainerInternal.GetOverviewContainer() {
			return CreateOverviewContainer();
		}
		protected virtual IDocumentContainer CreateDetailContainer(Document document) {
			return new DetailContainer(document, this);
		}
		protected virtual IContentContainer CreateOverviewContainer() {
			return new OverviewContainer(this);
		}
		public virtual void UpdateDocumentActions() {
			WindowsUIView view = GetView();
			if(view != null && view.ActiveDocument != null)
				UpdateDocumentActions(view.ActiveDocument as Document);
		}
		protected void UpdateDocumentActions(Document document) {
			WindowsUIView view = GetView();
			if(view != null && document != null) {
				Buttons.Unmerge(view.GetCurrentlyUsedDocumentActions());
				if(document.IsControlLoaded) {
					var actionsArgs = view.GetDocumentActions(this, document);
					OnDocumentActionsLoaded(actionsArgs);
					Buttons.Merge(actionsArgs.DocumentActions);
				}
				if(view.ViewInfo != null)
					view.ViewInfo.SetDirty();
				LayoutChanged();
			}
		}
		protected virtual void NotifyNavigatedTo() { }
		protected void NotifyNavigatedTo(Document document) {
			WindowsUIView view = GetView();
			if(view != null)
				view.NotifyNavigatedTo(document);
		}
		protected virtual void NotifyNavigatedFrom() { }
		protected void NotifyNavigatedFrom(Document document) {
			WindowsUIView view = GetView();
			if(view != null)
				view.NotifyNavigatedFrom(document);
		}
		protected virtual void GetActualActionsCore(IList<IContentContainerAction> actions) {
			actions.Add(ContentContainerAction.Back);
			actions.Add(ContentContainerAction.Home);
			if(WindowsUIViewController.IsFullScreenMode(Manager))
				actions.Add(ContentContainerAction.Exit);
			if(GetZoomLevel() == ContextualZoomLevel.Normal)
				actions.Add(ContentContainerAction.Overview);
			if(Info != null && Info.Owner != null) {
				foreach(var commonAction in ((WindowsUIView)Info.Owner).ContentContainerActions)
					actions.Add(commonAction);
			}
		}
		static readonly object actualActionsChanged = new object();
		[Category(DevExpress.XtraEditors.CategoryName.Appearance)]
		public event EventHandler ActualActionsChanged {
			add { this.Events.AddHandler(actualActionsChanged, value); }
			remove { this.Events.RemoveHandler(actualActionsChanged, value); }
		}
		protected internal void RaiseActualActionsChanged() {
			EventHandler handler = (EventHandler)this.Events[actualActionsChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		static readonly object actionCustomization = new object();
		[Category(DevExpress.XtraEditors.CategoryName.Appearance)]
		public event ContentContainerActionCustomizationEventHandler ActionCustomization {
			add { this.Events.AddHandler(actionCustomization, value); }
			remove { this.Events.RemoveHandler(actionCustomization, value); }
		}
		void OnActionCustomization(IList<IContentContainerAction> actions) {
			ContentContainerActionCustomizationEventArgs args = new ContentContainerActionCustomizationEventArgs(this, actions);
			WindowsUIView view = (Info != null) ? Info.Owner as WindowsUIView : null;
			if(view != null)
				view.RaiseContentContainerActionCustomization(args);
			RaiseActionCustomization(args);
		}
		protected void RaiseActionCustomization(ContentContainerActionCustomizationEventArgs args) {
			ContentContainerActionCustomizationEventHandler handler = (ContentContainerActionCustomizationEventHandler)this.Events[actionCustomization];
			if(handler != null)
				handler(this, args);
		}
		protected virtual IBaseProperties GetDetailContainerParentProperties(WindowsUIView view) {
			return view.DetailContainerProperties;
		}
		protected abstract IBaseProperties GetParentProperties(WindowsUIView view);
		protected abstract void ReleaseDeferredControlLoadDocuments();
		protected abstract void EnsureDeferredControlLoadDocuments();
		protected abstract void PatchChildrenCore(Rectangle view, bool active);
		protected virtual void OnBeforeActivate(WindowsUIView view) { }
		protected virtual void OnActivated() { }
		protected virtual void OnBeforeDeactivate(WindowsUIView view) { }
		protected virtual void OnDeactivated() {
			DeactivateDocumentInView(null);
		}
		protected IEnumerable<BaseDocument> GetLayoutDocuments() {
			WindowsUIView owner = GetView();
			var documents = owner.Documents.ToArray();
			BaseDocument activeFlyoutDocument = null;
			var activeFlyoutDocumentContainer = owner.ActiveFlyoutContainer as IDocumentContainer;
			if(activeFlyoutDocumentContainer != null)
				activeFlyoutDocument = activeFlyoutDocumentContainer.Document;
			return System.Linq.Enumerable.Where(documents, (d) => !object.ReferenceEquals(d, activeFlyoutDocument));
		}
		Document documentForDelayedButtonsMerging;
		protected void ActivateDocumentInView(Document document) {
			WindowsUIView view = GetView();
			if(view != null && document != null) {
				Buttons.Unmerge(view.GetCurrentlyUsedDocumentActions());
				view.ActivateDocument(document);
				this.documentForDelayedButtonsMerging = null;
				if(document.IsControlLoaded) {
					var actionsArgs = view.GetDocumentActions(this, document);
					OnDocumentActionsLoaded(actionsArgs);
					Buttons.Merge(actionsArgs.DocumentActions);
				}
				else this.documentForDelayedButtonsMerging = document;
			}
		}
		protected void DeactivateDocumentInView(Document lastDocument) {
			WindowsUIView view = GetView();
			if(view != null) {
				Buttons.Unmerge(view.GetCurrentlyUsedDocumentActions());
				if(ContainsCore((Document)view.ActiveDocument)) {
					view.ActivateDocument(lastDocument);
				}
			}
		}
		public IEnumerator<IContentContainer> GetEnumerator() {
			return GetEnumerator(this, null, -1);
		}
		public IEnumerator<IContentContainer> GetEnumerator(int level) {
			return GetEnumerator(this, null, level);
		}
		public IEnumerator<IContentContainer> GetEnumerator(Predicate<IContentContainer> filter) {
			return GetEnumerator(this, filter, -1);
		}
		public static IEnumerator<IContentContainer> GetEnumerator(IContentContainer container, Predicate<IContentContainer> filter, int level) {
			return new IContentContainerEnumerator(container, filter, level);
		}
		static readonly object buttonClick = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseContentContainerButtonClick"),
#endif
 Category(DockConsts.BehaviorCategory)]
		public virtual event ButtonEventHandler ButtonClick {
			add { this.Events.AddHandler(buttonClick, value); }
			remove { this.Events.RemoveHandler(buttonClick, value); }
		}
		static readonly object buttonUnchecked = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseContentContainerButtonUnchecked"),
#endif
 Category(DockConsts.BehaviorCategory)]
		public virtual event ButtonEventHandler ButtonUnchecked {
			add { this.Events.AddHandler(buttonUnchecked, value); }
			remove { this.Events.RemoveHandler(buttonUnchecked, value); }
		}
		static readonly object buttonChecked = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseContentContainerButtonChecked"),
#endif
 Category(DockConsts.BehaviorCategory)]
		public virtual event ButtonEventHandler ButtonChecked {
			add { this.Events.AddHandler(buttonChecked, value); }
			remove { this.Events.RemoveHandler(buttonChecked, value); }
		}
		protected virtual void OnButtonChecked(object sender, ButtonEventArgs e) {
			if(e.Button is WindowsUIActionButton) {
				WindowsUIActionButton button = e.Button as WindowsUIActionButton;
				var tag = button.Tag as ContentContainerAction.Tag;
				tag.Args.State = true;
				ContentContainerAction.Execute(tag.View, tag.Args);
				Buttons.UpdateButtonsVisibility();
			}
			else RaiseButtonChecked(e);
		}
		protected virtual void OnButtonUnchecked(object sender, ButtonEventArgs e) {
			if(e.Button is WindowsUIActionButton) {
				WindowsUIActionButton button = e.Button as WindowsUIActionButton;
				var tag = button.Tag as ContentContainerAction.Tag;
				tag.Args.State = false;
				ContentContainerAction.Execute(tag.View, tag.Args);
				Buttons.UpdateButtonsVisibility();
			}
			else RaiseButtonUnchecked(e);
		}
		protected virtual void OnButtonClick(object sender, ButtonEventArgs e) {
			if(e.Button is WindowsUIActionButton) {
				WindowsUIActionButton button = e.Button as WindowsUIActionButton;
				var tag = button.Tag as ContentContainerAction.Tag;
				ContentContainerAction.Execute(tag.View, tag.Args);
				Buttons.UpdateButtonsVisibility();
			}
			else RaiseButtonClick(e);
		}
		protected void RaiseButtonChecked(ButtonEventArgs e) {
			ButtonEventHandler handler = (ButtonEventHandler)this.Events[buttonChecked];
			if(handler != null) handler(this, e);
		}
		protected void RaiseButtonUnchecked(ButtonEventArgs e) {
			ButtonEventHandler handler = (ButtonEventHandler)this.Events[buttonUnchecked];
			if(handler != null) handler(this, e);
		}
		protected void RaiseButtonClick(ButtonEventArgs e) {
			ButtonEventHandler handler = (ButtonEventHandler)this.Events[buttonClick];
			if(handler != null) handler(this, e);
			WindowsUIButton button = e.Button as WindowsUIButton;
			if(button != null) button.RaiseClick();
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseContentContainerButtonBackgroundImages"),
#endif
		DefaultValue(null), Category("Buttons"), TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter))]
		public virtual object ButtonBackgroundImages {
			get { return buttonBackgroundImagesCore; }
			set {
				if(ButtonBackgroundImages == value) return;
				buttonBackgroundImagesCore = value;
				ColoredElementsCache.Reset();
				if(Manager != null)
					Manager.LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseContentContainerAppearanceButton"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Buttons")]
		public virtual DevExpress.XtraEditors.ButtonsPanelControl.ButtonsPanelControlAppearance AppearanceButton {
			get { return appearanceButtonCore; }
		}
		void OnAppearanceButtonChanged(object sender, EventArgs e) {
			LayoutChanged();
		}
		void ResetAppearanceButton() { AppearanceButton.Reset(); }
		bool ShouldSerializeAppearanceButton() { return !IsDisposing && AppearanceButton.ShouldSerialize(); }
		[ListBindable(false), TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter)),
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseContentContainerButtons"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraBars.Design.WindowsUIButtonCollectionEditor, " + AssemblyInfo.SRAssemblyBarsDesign,
		 typeof(System.Drawing.Design.UITypeEditor)), Category("Buttons"), Localizable(true)]
		public virtual ContentContainerButtonCollection Buttons {
			get { return ButtonsPanel.Buttons as ContentContainerButtonCollection; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseContentContainerButtonInterval"),
#endif
 DefaultValue(21), Category("Buttons")]
		public virtual int ButtonInterval {
			get { return ButtonsPanel.ButtonInterval; }
			set { ButtonsPanel.ButtonInterval = value; }
		}
		WindowsUIButtonsPanel ICustomButtonsOwner.ButtonsPanel {
			get { return ButtonsPanel; }
		}
		protected WindowsUIButtonsPanel ButtonsPanel { get { return buttonsPanelCore; } }
		#region IWindowsUIButtonPanelAppearanceOwner Members
		DevExpress.XtraEditors.ButtonsPanelControl.IButtonsPanelControlAppearanceProvider DevExpress.XtraEditors.ButtonsPanelControl.IButtonPanelControlAppearanceOwner.CreateAppearanceProvider() {
			return new DevExpress.XtraEditors.ButtonsPanelControl.ButtonsPanelControlAppearanceProvider();
		}
		#endregion
		#region IAppearanceOwner Members
		bool DevExpress.Utils.IAppearanceOwner.IsLoading {
			get { return false; }
		}
		#endregion
		#region IButtonsPanelOwner Members
		bool IButtonsPanelOwner.Enabled { get { return true; } }
		bool IWindowsUIButtonPanelOwner.EnableImageTransparency { get { return false; } }
		bool IButtonsPanelOwner.AllowHtmlDraw { get { return false; } }
		bool IButtonsPanelOwner.AllowGlyphSkinning { get { return false; } }
		ObjectPainter IButtonsPanelOwner.GetPainter() {
			return new CustomHeaderButtonsPanelSkinPainter(Manager.GetBarAndDockingController().LookAndFeel);
		}
		object IButtonsPanelOwner.Images {
			get { return Manager != null ? Manager.Images : null; }
		}
		void IButtonsPanelOwner.Invalidate() {
			if(Manager != null && ButtonsPanel.ViewInfo != null)
				Manager.Invalidate(ButtonsPanel.ViewInfo.Bounds);
		}
		bool IButtonsPanelOwner.IsSelected {
			get { return false; }
		}
		object IButtonsPanelOwner.ButtonBackgroundImages {
			get {
				if(ButtonBackgroundImages != null)
					return ButtonBackgroundImages;
				var view = GetView();
				return (view != null) ? view.ActionButtonBackgroundImages : null;
			}
		}
		bool IWindowsUIButtonPanelOwner.UseButtonBackgroundImages { get { return true; } }
		#endregion
		#region ISearchObject Members
		string[] searchTagsCore;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseContentContainerSearchTags"),
#endif
		Category("Search Properties"), DefaultValue(null), Localizable(true)]
		public string[] SearchTags {
			get { return searchTagsCore; }
			set {
				if(searchTagsCore == value) return;
				searchTagsCore = value;
			}
		}
		string Customization.ISearchObject.SearchText { get { return GetSearchText(); } }
		string Customization.ISearchObject.SearchTag { get { return GetSearchTag(); } }
		protected virtual string GetSearchText() {
			return string.IsNullOrEmpty(this.Caption) ? this.Name : this.Caption;
		}
		protected virtual string GetSearchTag() {
			string searchTags = string.Empty;
			if(SearchTags == null || SearchTags.Length <= 0) return GetSearchText();
			foreach(string tag in SearchTags) {
				if(string.IsNullOrEmpty(tag)) continue;
				searchTags += tag;
			}
			return searchTags;
		}
		bool excludeFromSearchCore;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseContentContainerExcludeFromSearch"),
#endif
 Category("Search Properties"), DefaultValue(false)]
		public bool ExcludeFromSearch {
			get { return excludeFromSearchCore; }
			set {
				if(ExcludeFromSearch == value) return;
				excludeFromSearchCore = value;
			}
		}
		bool Customization.ISearchObject.EnabledInSearch { get { return EnabledInSearch; } }
		protected virtual bool EnabledInSearch { get { return !this.IsDisposing && !ExcludeFromSearch; } }
		#endregion
		#region ISearchContainer Members
		IEnumerable<Customization.ISearchContainer> Customization.ISearchContainer.SearchChildList {
			get { return GetSearchChildList(); }
		}
		protected virtual IEnumerable<Customization.ISearchContainer> GetSearchChildList() {
			IList<Customization.ISearchContainer> list = new List<Customization.ISearchContainer>();
			foreach(Customization.ISearchContainer searchContainer in this)
				if(searchContainer != this)
					list.Add(searchContainer);
			return list;
		}
		IEnumerable<Customization.ISearchObject> Customization.ISearchContainer.SearchObjectList {
			get { return GetSearchObjectList(); }
		}
		protected virtual IEnumerable<Customization.ISearchObject> GetSearchObjectList() { return null; }
		#endregion
		Image IContentContainer.Image {
			get { return GetActualImage(); }
			set { imageCore = value; }
		}
	}
	public class ContentContainerCollection : BaseMutableListEx<IContentContainer, WindowsUIView> {
		public ContentContainerCollection(WindowsUIView owner)
			: base(owner) {
		}
		protected override void NotifyOwnerOnInsert(int index) {
		}
		protected override void OnBeforeElementAdded(IContentContainer element) {
			Owner.BeginUpdate();
			((IContentContainerInternal)element).SetManager(Owner.Manager);
			base.OnBeforeElementAdded(element);
		}
		protected override void OnElementAdded(IContentContainer element) {
			Owner.AddToContainer(element);
			base.OnElementAdded(element);
			Owner.EndUpdate();
		}
		protected override void OnBeforeElementRemoved(IContentContainer element) {
			Owner.BeginUpdate();
			base.OnBeforeElementRemoved(element);
		}
		protected override void OnElementRemoved(IContentContainer element) {
			base.OnElementRemoved(element);
			Owner.RemoveFromContainer(element);
			((IContentContainerInternal)element).SetManager(null);
			Owner.EndUpdate();
		}
		public IContentContainer this[string name] {
			get { return FindFirst((contentContainer) => (!string.IsNullOrEmpty(contentContainer.Name) && contentContainer.Name.Equals(name))); }
		}
	}
	public abstract class ContentContainerProperties : DevExpress.XtraBars.Docking2010.Base.BaseProperties, IContentContainerProperties {
		public ContentContainerProperties() {
			SetDefaultValueCore("DestroyOnRemovingChildren", true);
			SetDefaultValueCore("ShowCaption", true);
		}
		[DefaultValue(0), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Layout")]
		public int Capacity {
			get { return GetValueCore<int>("Capacity"); }
			set { SetValueCore("Capacity", value); }
		}
		[DefaultValue(0), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Layout")]
		public int HeaderOffset {
			get { return GetValueCore<int>("HeaderOffset"); }
			set { SetValueCore("HeaderOffset", value); }
		}
		[DefaultValue(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public bool ShowCaption {
			get { return GetValueCore<bool>("ShowCaption"); }
			set { SetValueCore("ShowCaption", value); }
		}
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public bool DestroyOnRemovingChildren {
			get { return GetValueCore<bool>("DestroyOnRemovingChildren"); }
			set { SetValueCore("DestroyOnRemovingChildren", value); }
		}
		bool ShouldSerializeDestroyOnRemovingChildren() { return !IsDefault("DestroyOnRemovingChildren"); }
		void ResetDestroyOnRemovingChildren() { Reset("DestroyOnRemovingChildren"); }
		[DefaultValue(false), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public bool ShowContextActionBarOnActivating {
			get { return GetValueCore<bool>("ShowContextActionBarOnActivating"); }
			set { SetValueCore("ShowContextActionBarOnActivating", value); }
		}
		[DefaultValue(Orientation.Horizontal), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Layout")]
		public virtual Orientation Orientation {
			get { return GetValueCore<Orientation>("Orientation"); }
			set { SetValueCore("Orientation", value); }
		}
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Layout")]
		public Padding Margin {
			get { return GetValueCore<Padding>("Margin"); }
			set { SetValueCore("Margin", value); }
		}
		bool ShouldSerializeMargin() { return !IsDefault("Margin"); }
		void ResetMargin() { Reset("Margin"); }
	}
	public abstract class ContentContainerDefaultProperties : DevExpress.XtraBars.Docking2010.Base.BaseDefaultProperties, IContentContainerDefaultProperties {
		public ContentContainerDefaultProperties(IContentContainerProperties parentProperties)
			: base(parentProperties) {
			SetDefaultValueCore("DestroyOnRemovingChildren", DevExpress.Utils.DefaultBoolean.Default);
			SetDefaultValueCore("ShowContextActionBarOnActivating", DevExpress.Utils.DefaultBoolean.Default);
			SetDefaultValueCore("ShowCaption", DevExpress.Utils.DefaultBoolean.Default);
			SetConverter("DestroyOnRemovingChildren", GetDefaultBooleanConverter(true));
			SetConverter("ShowContextActionBarOnActivating", GetDefaultBooleanConverter(false));
			SetConverter("ShowCaption", GetDefaultBooleanConverter(true));
		}
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Layout")]
		[TypeConverter(typeof(NullableTypeConverter))]
		public int? Capacity {
			get { return GetValueCore<int?>("Capacity"); }
			set { SetValueCore("Capacity", value); }
		}
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Layout")]
		[TypeConverter(typeof(NullableTypeConverter))]
		public int? HeaderOffset {
			get { return GetValueCore<int?>("HeaderOffset"); }
			set { SetValueCore("HeaderOffset", value); }
		}
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Layout")]
		public DevExpress.Utils.DefaultBoolean ShowCaption {
			get { return GetValueCore<DevExpress.Utils.DefaultBoolean>("ShowCaption"); }
			set { SetValueCore("ShowCaption", value); }
		}
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public DevExpress.Utils.DefaultBoolean DestroyOnRemovingChildren {
			get { return GetValueCore<DevExpress.Utils.DefaultBoolean>("DestroyOnRemovingChildren"); }
			set { SetValueCore("DestroyOnRemovingChildren", value); }
		}
		bool ShouldSerializeDestroyOnRemovingChildren() { return !IsDefault("DestroyOnRemovingChildren"); }
		void ResetDestroyOnRemovingChildren() { Reset("DestroyOnRemovingChildren"); }
		[DefaultValue(DevExpress.Utils.DefaultBoolean.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public DevExpress.Utils.DefaultBoolean ShowContextActionBarOnActivating {
			get { return GetValueCore<DevExpress.Utils.DefaultBoolean>("ShowContextActionBarOnActivating"); }
			set { SetValueCore("ShowContextActionBarOnActivating", value); }
		}
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Layout")]
		[TypeConverter(typeof(NullableTypeConverter))]
		public Orientation? Orientation {
			get { return GetValueCore<Orientation?>("Orientation"); }
			set { SetValueCore("Orientation", value); }
		}
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Layout")]
		[TypeConverter(typeof(NullableTypeConverter))]
		public Padding? Margin {
			get { return GetValueCore<Padding?>("Margin"); }
			set { SetValueCore("Margin", value); }
		}
		[Browsable(false)]
		public bool ActualDestroyOnRemovingChildren {
			get { return GetActualValue<DevExpress.Utils.DefaultBoolean, bool>("DestroyOnRemovingChildren"); }
		}
		[Browsable(false)]
		public bool CanShowContextActionBarOnActivating {
			get { return GetActualValue<DevExpress.Utils.DefaultBoolean, bool>("ShowContextActionBarOnActivating"); }
		}
		[Browsable(false)]
		public int ActualCapacity {
			get { return GetActualValueFromNullable<int>("Capacity"); }
		}
		[Browsable(false)]
		public Orientation ActualOrientation {
			get { return GetActualValueFromNullable<Orientation>("Orientation"); }
		}
		[Browsable(false)]
		public Padding ActualMargin {
			get { return GetActualValueFromNullable<Padding>("Margin"); }
		}
		[Browsable(false)]
		public int ActualHeaderOffset {
			get { return GetActualValueFromNullable<int>("HeaderOffset"); }
		}
		[Browsable(false)]
		public bool HasMargin {
			get { return HasValue("Margin"); }
		}
		[Browsable(false)]
		public bool HasHeaderOffset {
			get { return HasValue("HeaderOffset"); }
		}
		[Browsable(false)]
		public bool CanShowCaption {
			get { return GetActualValue<DevExpress.Utils.DefaultBoolean, bool>("ShowCaption"); }
		}
	}
	class IContentContainerEnumerator : IEnumerator<IContentContainer> {
		IContentContainer Root;
		Stack<IContentContainer> Stack;
		Predicate<IContentContainer> Filter;
		public IContentContainerEnumerator(IContentContainer element)
			: this(element, null, -1) {
		}
		public IContentContainerEnumerator(IContentContainer element, Predicate<IContentContainer> filter, int level) {
			Stack = new Stack<IContentContainer>(8);
			Root = element;
			Filter = filter;
			Level = level;
		}
		public void Dispose() {
			Reset();
			Root = null;
			Stack = null;
			Filter = null;
			GC.SuppressFinalize(this);
		}
		#region IEnumerator Members
		IContentContainer current;
		object IEnumerator.Current {
			get { return current; }
		}
		public IContentContainer Current {
			get { return current; }
		}
		int currentLevel;
		public int Level { get; set; }
		public bool MoveNext() {
			if(current == null) {
				current = Root;
				currentLevel = 0;
			}
			else {
				bool canGetNext = Level > 0;
				canGetNext = canGetNext ? currentLevel < Level : true;
				if(current is IContentContainerInternal && canGetNext) {
					var children = ((IContentContainerInternal)current).Children;
					if(children.Count > 0) {
						for(int i = 0; i < children.Count; i++) {
							IContentContainer child = children[(children.Count - 1) - i];
							if(Filter == null || Filter(child)) {
								Stack.Push(child);
							}
						}
					}
				}
				current = Stack.Count > 0 ? Stack.Pop() : null;
				currentLevel++;
			}
			return current != null;
		}
		public void Reset() {
			if(Stack != null)
				Stack.Clear();
			current = null;
		}
		#endregion
	}
	class WindowsUIActionButton : WindowsUIButton, IButton {
		IUIActionProperties action;
		public WindowsUIActionButton(IUIActionProperties action) {
			this.action = action;
		}
		protected override DevExpress.XtraEditors.ButtonPanel.ImageLocation ImageLocationCore {
			get { return DevExpress.XtraEditors.ButtonPanel.ImageLocation.AboveText; }
		}
		public override string Caption {
			get { return action.Caption; }
		}
		public override string ToolTip {
			get { return action.Description; }
		}
		public new bool IsLeft {
			get { return GetIsLeft(); }
			set { SetIsLeft(value); }
		}
		protected internal bool Belong(IEnumerable<IUIActionProperties> actions) {
			return System.Linq.Enumerable.Contains(actions, action);
		}
	}
	public class ContentContainerButtonCollection : BaseButtonCollection {
		public ContentContainerButtonCollection(IButtonsPanel panel)
			: base(panel) {
		}
		bool CanAddAction() {
			return Owner.Owner is IContentContainer;
		}
		public void AddAction(IContentContainerAction action) {
			if(!CanAddAction()) return;
			Add(CreateActionButton((Owner.Owner as IContentContainer), action));
		}
		public void AddAction(IDocumentAction action) {
			if(!CanAddAction()) return;
			Add(CreateActionButton((Owner.Owner as IContentContainer), action));
		}
		public void AddActions(IEnumerable<IContentContainerAction> actions) {
			if(!CanAddAction() || actions == null) return;
			BeginUpdate();
			foreach(IContentContainerAction action in actions) {
				Add(CreateActionButton(Owner.Owner as IContentContainer, action));
			}
			EndUpdate();
		}
		public void AddActions(IEnumerable<IDocumentAction> actions) {
			if(!CanAddAction() || actions == null) return;
			BeginUpdate();
			foreach(IDocumentAction action in actions) {
				Add(CreateActionButton(Owner.Owner as IContentContainer, action));
			}
			EndUpdate();
		}
		protected internal void UpdateButtonsVisibility() {
			BeginUpdate();
			foreach(IBaseButton btn in this) {
				WindowsUIActionButton button = btn as WindowsUIActionButton;
				if(button != null)
					button.Visible = ContentContainerAction.CanExecute(GetArgs(button));
			}
			CancelUpdate();
		}
		protected internal void Merge(IEnumerable<IDocumentAction> actions) {
			if(!CanAddAction() || actions == null) return;
			BeginUpdate();
			foreach(IDocumentAction action in actions) {
				Add(CreateActionButton(Owner.Owner as IContentContainer, action));
			}
			if(Owner.ViewInfo != null)
				Owner.ViewInfo.SetDirty();
			CancelUpdate();
		}
		protected internal void Unmerge(IEnumerable<IDocumentAction> actions) {
			if(actions == null) return;
			var buttons = ToArray();
			BeginUpdate();
			foreach(IButton btn in buttons) {
				WindowsUIActionButton button = btn as WindowsUIActionButton;
				if(button != null && button.Belong(actions))
					Remove(button);
			}
			CancelUpdate();
		}
		protected internal IButton CreateActionButton(IContentContainer container, IContentContainerAction action) {
			ContentContainerAction.Args args = new ContentContainerAction.Args(action, container);
			WindowsUIActionButton button = new WindowsUIActionButton(action);
			button.Tag = new ContentContainerAction.Tag(GetWindowsUIView, args);
			button.IsLeft = ContentContainerAction.GetActionLayout(action).Edge == ActionEdge.Left;
			button.Image = action.Image ?? Resources.ContentContainterActionResourceLoader.GetImage("Default");
			return button;
		}
		protected internal IButton CreateActionButton(IContentContainer container, IDocumentAction action) {
			ContentContainerAction.Args args = new ContentContainerAction.Args(action, container);
			WindowsUIActionButton button = new WindowsUIActionButton(action);
			button.Tag = new ContentContainerAction.Tag(GetWindowsUIView, args);
			button.Style = (ContentContainerAction.GetActionStyle(action).Style == ActionStyle.CheckAction) ?
				ButtonStyle.CheckButton : ButtonStyle.PushButton;
			if(button.Style == ButtonStyle.CheckButton) {
				args.State = ContentContainerAction.GetActionStyle(action).InitialState;
				button.Checked = object.Equals(args.State, true);
			}
			button.Visible = ContentContainerAction.CanExecute(args);
			button.Image = action.Image ?? Resources.ContentContainterActionResourceLoader.GetImage("Default");
			return button;
		}
		protected internal WindowsUIView GetWindowsUIView() {
			if(!(Owner.Owner is IContentContainer)) return null;
			IContentContainer container = Owner.Owner as IContentContainer;
			if(container.Info == null || !(container.Info.Owner is WindowsUIView)) return null;
			return container.Info.Owner as WindowsUIView;
		}
		ContentContainerAction.Args GetArgs(WindowsUIActionButton button) {
			return (button.Tag as ContentContainerAction.Tag).Args;
		}
	}
}
