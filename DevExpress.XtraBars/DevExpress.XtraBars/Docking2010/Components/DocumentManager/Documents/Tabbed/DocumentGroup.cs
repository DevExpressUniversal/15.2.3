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
using DevExpress.Utils.Base;
using DevExpress.Utils.Mdi;
using DevExpress.Utils.Serializing;
using DevExpress.XtraBars.Docking2010.Base;
namespace DevExpress.XtraBars.Docking2010.Views.Tabbed {
	public interface IDocumentGroupProperties : IBaseProperties {
		int MaxDocuments { get; set; }
		bool DestroyOnRemovingChildren { get; set; }
		bool ShowTabHeader { get; set; }
		bool ShowDocumentSelectorButton { get; set; }
		DevExpress.XtraTab.TabHeaderLocation HeaderLocation { get; set; }
		DevExpress.XtraTab.TabOrientation HeaderOrientation { get; set; }
		DevExpress.XtraTab.TabPageImagePosition PageImagePosition { get; set; }
		DevExpress.XtraTab.ClosePageButtonShowMode ClosePageButtonShowMode { get; set; }
		DevExpress.XtraTab.PinPageButtonShowMode PinPageButtonShowMode { get; set; }
		DevExpress.XtraTab.Buttons.CustomHeaderButtonCollection CustomHeaderButtons { get; }
		DevExpress.XtraTabbedMdi.CloseTabOnMiddleClick CloseTabOnMiddleClick { get; set; }
		DevExpress.XtraTab.TabButtons HeaderButtons { get; set; }
		DevExpress.XtraTab.TabButtonShowMode HeaderButtonsShowMode { get; set; }
	}
	public interface IDocumentGroupDefaultProperties : IBaseDefaultProperties {
		int? MaxDocuments { get; set; }
		DevExpress.Utils.DefaultBoolean DestroyOnRemovingChildren { get; set; }
		DevExpress.Utils.DefaultBoolean ShowTabHeader { get; set; }
		DevExpress.Utils.DefaultBoolean ShowDocumentSelectorButton { get; set; }
		DevExpress.XtraTab.TabHeaderLocation? HeaderLocation { get; set; }
		DevExpress.XtraTab.TabOrientation HeaderOrientation { get; set; }
		DevExpress.XtraTab.TabPageImagePosition? PageImagePosition { get; set; }
		DevExpress.XtraTab.ClosePageButtonShowMode ClosePageButtonShowMode { get; set; }
		DevExpress.XtraTab.PinPageButtonShowMode PinPageButtonShowMode { get; set; }
		DevExpress.XtraTab.Buttons.CustomHeaderButtonCollection CustomHeaderButtons { get; }
		DevExpress.XtraTabbedMdi.CloseTabOnMiddleClick CloseTabOnMiddleClick { get; set; }
		int TabWidth { get; set; }
		DevExpress.XtraTab.TabButtons HeaderButtons { get; set; }
		DevExpress.XtraTab.TabButtonShowMode HeaderButtonsShowMode { get; set; }
		int ActualMaxDocuments { get; }
		bool ActualDestroyOnRemovingChildren { get; }
		bool HasTabHeader { get; }
		bool HasDocumentSelectorButton { get; }
		DevExpress.XtraTab.TabHeaderLocation ActualHeaderLocation { get; }
		DevExpress.XtraTab.TabOrientation ActualHeaderOrientation { get; }
		DevExpress.XtraTab.TabPageImagePosition ActualPageImagePosition { get; }
		DevExpress.XtraTab.ClosePageButtonShowMode ActualClosePageButtonShowMode { get; }
		DevExpress.XtraTab.PinPageButtonShowMode ActualPinPageButtonShowMode { get; }
		DevExpress.XtraTabbedMdi.CloseTabOnMiddleClick ActualCloseTabOnMiddleClick { get; }
		IEnumerable<DevExpress.XtraTab.Buttons.CustomHeaderButton> GetActualCustomHeaderButtons();
		DevExpress.XtraTab.TabButtons GetActualHeaderButtons { get; }
		DevExpress.XtraTab.TabButtonShowMode ActualHeaderButtonsShowMode { get; }
	}
	[ToolboxItem(false), DesignTimeVisible(false)]
	public class DocumentGroup : BaseComponent, IRelativeLengthElement {
		IDocumentGroupDefaultProperties propertiesCore;
		IDocumentGroupInfo infoCore;
		DocumentManager managerCore;
		DocumentCollection itemsCore;
		int groupLengthCore;
		object tagCore;
		public DocumentGroup()
			: base(null) {
			InitProperties(null);
		}
		public DocumentGroup(IContainer container)
			: base(container) {
			InitProperties(null);
		}
		public DocumentGroup(IDocumentGroupProperties defaultProperties)
			: base(null) {
			InitProperties(defaultProperties);
		}
		public bool Contains(Control control) {
			if(Manager == null) {
				foreach(Document document in Items) {
					if(document.Control == control)
						return true;
				}
				return false;
			}
			else {
				BaseDocument document = Manager.GetDocument(control);
				if(document == null) return false;
				return (((Document)document).Parent == this);
			}
		}
		[Browsable(false)]
		public DocumentManager Manager {
			get { return managerCore; }
		}
		protected internal IDocumentGroupInfo Info {
			get { return infoCore; }
		}
		protected internal bool IsLoaded { get; set; }
		protected internal void SetManager(DocumentManager manager) {
			managerCore = manager;
		}
		protected internal void SetInfo(IDocumentGroupInfo info) {
			infoCore = info;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DocumentCollection Items {
			get { return itemsCore; }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("DocumentGroupProperties")]
#endif
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Properties")]
		public IDocumentGroupDefaultProperties Properties {
			get { return propertiesCore; }
		}
		internal void SetGroupLength(int value) {
			value = Math.Max(0, value);
			groupLengthCore = value;
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("DocumentGroupGroupLength")]
#endif
		[Category("Layout"), DefaultValue(0), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int GroupLength {
			get { return actualLengthCore; }
			set {
				value = Math.Max(0, value);
				if(actualLengthCore == value) return;
				SetGroupLengthCore(value);
				LayoutChanged();
			}
		}
		internal void SetGroupLengthCore(int value) {
			if(actualLengthCore == 0) return;
			value = value == 0 ? 1 : value;
			lengthCore.UnitValue *=  (value / (double)actualLengthCore);
			TabbedView view = GetTabbedView();
			if(view == null) return;
			DockingContainer container = DockingContainerHelper.GetTargetNode(this, view.RootContainer);
			if(container == null || container.Parent == null) return;
			DockingContainerHelper.CorrectContainerNodesRelativeLength(container.Parent);
		}
		bool visibleCore;
		internal bool SetGroupVisibility(bool value) {
			if(visibleCore == value) return false;
			visibleCore = value;
			RaiseVisibilityChanged();
			return true;
		}
		static readonly object visibilityChangedCore = new object();
		[Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public event EventHandler VisibilityChanged {
			add { Events.AddHandler(visibilityChangedCore, value); }
			remove { Events.RemoveHandler(visibilityChangedCore, value); }
		}
		static readonly object tabMouseActivating = new object();
		[Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public event DocumentCancelEventHandler TabMouseActivating {
			add { Events.AddHandler(tabMouseActivating, value); }
			remove { Events.RemoveHandler(tabMouseActivating, value); }
		}
		protected void RaiseVisibilityChanged() {
			EventHandler handler = (EventHandler)Events[visibilityChangedCore];
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		protected internal bool RaiseTabMouseActivating(BaseDocument document) {
			DocumentCancelEventArgs ea = new DocumentCancelEventArgs(document);
			DocumentCancelEventHandler handler = (DocumentCancelEventHandler)Events[tabMouseActivating];
			if(handler != null) handler(this, ea);
			return ea.Cancel;
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("DocumentGroupVisible")]
#endif
		[Category("Layout"), DefaultValue(true)]
		public bool Visible {
			get { return visibleCore; }
			set {
				if(SetGroupVisibility(value))
					LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DocumentGroupTag"),
#endif
 DefaultValue(null)]
		[Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor))]
		[TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter)), Category("Data")]
		public object Tag {
			get { return tagCore; }
			set { tagCore = value; }
		}
		bool ShouldSerializeProperties() {
			return Properties != null && Properties.ShouldSerialize();
		}
		void ResetProperties() {
			Properties.Reset();
		}
		protected override void OnCreate() {
			itemsCore = CreateItems();
			Items.CollectionChanged += OnItemsCollectionChanged;
			(this as IRelativeLengthElement).Length = new Widget.Length();
			base.OnCreate();
		}
		public void DockTo(DocumentGroup targetGroup, Orientation orientation) {
			DockingContainerHelper.AddToGroup(targetGroup, this, GetTabbedView().RootContainer, orientation);
		}
		public void DockTo(DocumentGroup targetGroup, Orientation orientation, bool afterGroup) {
			DockingContainerHelper.AddToGroup(targetGroup, this, GetTabbedView().RootContainer, orientation, afterGroup);
		}
		void InitProperties(IDocumentGroupProperties parentProperties) {
			visibleCore = true;
			propertiesCore = CreateDefaultProperties(parentProperties);
			Properties.Changed += OnPropertiesChanged;
		}
		protected override void OnDispose() {
			Ref.Dispose(ref infoCore);
			if(Properties != null)
				Properties.Changed -= OnPropertiesChanged;
			if(Items != null)
				Items.CollectionChanged -= OnItemsCollectionChanged;
			Ref.Dispose(ref propertiesCore);
			Ref.Dispose(ref itemsCore);
			managerCore = null;
			base.OnDispose();
		}
		protected virtual IDocumentGroupDefaultProperties CreateDefaultProperties(IDocumentGroupProperties parentProperties) {
			return new DocumentGroupDefaultProperties(parentProperties);
		}
		protected virtual DocumentCollection CreateItems() {
			return new DocumentCollection(this);
		}
		void OnPropertiesChanged(object sender, EventArgs e) {
			PropertyChangedEventArgs args = e as PropertyChangedEventArgs;
			if(args == null || Info == null) return;
			switch(args.PropertyName) {
				case "ShowDocumentSelectorButton":
					Info.CheckDropDownButton();
					break;
				case "CustomHeaderButtons":
					Info.UpdateCustomHeaderButtons(this);
					break;
			}
			LayoutChanged();
		}
		void OnItemsCollectionChanged(CollectionChangedEventArgs<Document> ea) {
			if(ea.ChangedType == CollectionChangedType.ElementAdded) {
				if(selectedItemIndexCore == -1)
					SetSelected(ea.Element);
				SortPinnedItems();
			}
			if(ea.ChangedType == CollectionChangedType.ElementDisposed) {
				if(IsLoaded && CanDestroyAutomatically(ea.Element))
					Dispose();
			}
			if(ea.ChangedType == CollectionChangedType.ElementRemoved) {
				if(IsLoaded && CanDestroyAutomatically()) {
					Dispose();
				}
				else CheckSelectedIndex();
			}
			LayoutChanged();
		}
		protected virtual bool CanDestroyAutomatically() {
			return Items.Count == 0 && Properties.ActualDestroyOnRemovingChildren;
		}
		protected virtual bool CanDestroyAutomatically(Document disposedDocument) {
			return Items.Count == 1 && Items.Contains(disposedDocument) && Properties.ActualDestroyOnRemovingChildren;
		}
		protected override void OnLayoutChanged() {
			base.OnLayoutChanged();
			if(Info != null && Info.Owner != null)
				Info.Owner.SetLayoutModified();
			if(Manager != null)
				Manager.LayoutChanged();
		}
		[Browsable(false)]
		public bool IsFilledUp {
			get {
				int maxDocument = Properties.ActualMaxDocuments;
				return maxDocument > 0 && Items.Count >= maxDocument;
			}
		}
		int selectedItemIndexCore = -1;
		[Browsable(false)]
		public int SelectedItemIndex {
			get { return selectedItemIndexCore; }
		}
		[Browsable(false)]
		public Document SelectedDocument {
			get { return GetDocument(SelectedItemIndex); }
		}
		protected Document GetDocument(int index) {
			return (index >= 0 && index < Items.Count) ? Items[index] : null;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void SetSelected(Document document) {
			Document prevDocument = GetDocument(SelectedItemIndex);
			selectedItemIndexCore = Items.IndexOf(document);
			if(prevDocument != document) {
				if(prevDocument != null) {
					prevDocument.SetIsSelected(false);
					BaseView view = Info != null ? Info.Owner :
						(Manager != null ? Manager.View : null);
					if(view != null)
						prevDocument.ReleaseDeferredLoadControl(view);
				}
				if(document != null)
					document.SetIsSelected(true);
				RaiseSelectionChanged(document);
			}
			else {
				if(document != null)
					document.SetIsSelected(true);
			}
		}
		static readonly object selectionChangedCore = new object();
		[Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public event DocumentEventHandler SelectionChanged {
			add { Events.AddHandler(selectionChangedCore, value); }
			remove { Events.RemoveHandler(selectionChangedCore, value); }
		}
		protected internal void RaiseSelectionChanged(Document document) {
			DocumentEventHandler handler = (DocumentEventHandler)Events[selectionChangedCore];
			if(handler != null) handler(this, new DocumentEventArgs(document));
		}
		protected internal void RaisePinned(Document document) {
			var view = GetTabbedView();
			if(view != null)
				view.RaiseDocumentPinned(document);
		}
		protected internal void RaiseUnpinned(Document document) {
			var view = GetTabbedView();
			if(view != null)
				view.RaiseDocumentUnpinned(document);
		}
		TabbedView GetTabbedView() {
			var view = Info != null ? Info.Owner : (Manager != null ? Manager.View : null);
			return view as TabbedView;
		}
		internal void OnInsert(int index) {
			if(index <= selectedItemIndexCore)
				selectedItemIndexCore++;
		}
		void SetSelectedCore(int index, bool value) {
			Document prevDocument = GetDocument(SelectedItemIndex);
			Document document = GetDocument(index);
			if(document != null)
				document.SetIsSelected(value);
			if(document != prevDocument)
				RaiseSelectionChanged(document);
		}
		void CheckSelectedIndex() {
			if(selectedItemIndexCore == Items.Count) {
				selectedItemIndexCore--;
				SetSelectedCore(SelectedItemIndex, true);
			}
		}
		protected internal Control GetActiveChild() {
			if(SelectedDocument == null) return null;
			if(Visible && Info != null)
				SelectedDocument.EnsureIsBoundToControl(Info.Owner);
			return Manager.GetChild(SelectedDocument);
		}
		protected Control[] GetChildren() {
			Control[] children = new Control[Items.Count];
			for(int i = 0; i < children.Length; i++)
				if(Items[i].IsControlLoaded)
					children[i] = Manager.GetChild(Items[i]);
			return children;
		}
		internal void RedrawThumbnailChild(Control control, Size client, Rectangle view) {
			if(control == null) return;
			BaseView.PatchChild(control, new Rectangle(new Point(-view.Width, -view.Height), client), view);
		}
		protected internal void PatchActiveChild(Rectangle client, Rectangle view) {
			Control activeChild = GetActiveChild();
			if(activeChild != null) {
				BaseView.PatchChild(activeChild, client, view);
				Manager.LayeredWindowsNotifyHidden(activeChild);
			}
			Control[] children = GetChildren();
			foreach(Control child in children) {
				if(child == null || child == activeChild) continue;
				Rectangle childBounds = child.Bounds;
				if(view.IntersectsWith(childBounds)) {
					child.Location = new Point(
							view.X - Math.Max(childBounds.Width, client.Width),
							view.Y - Math.Max(childBounds.Height, client.Height)
						);
					Manager.LayeredWindowsNotifyHidden(child);
				}
			}
		}
		protected internal void PathBeforeActivateChild(Control activatedChild, Control activeChild, Rectangle bounds) {
			MdiChildHelper.PatchBeforeActivateChild(activatedChild, activeChild, bounds);
		}
		protected internal DevExpress.XtraTab.IXtraTab GetTab() {
			return Info != null ? Info.Tab : null;
		}
		protected internal DevExpress.XtraTab.IXtraTabPage GetSelectedTabPage() {
			return (SelectedDocument != null) ? SelectedDocument.GetTabPage() : null;
		}
		protected internal Size MinSize {
			get {
				Document document = GetDocument(SelectedItemIndex);
				Control child = (document != null && document.Control != null) ? Manager.GetChild(document) : null;
				Size documentMinSize = (child != null) ? child.MinimumSize : Size.Empty;
				Size borderMinSize = (Info != null) ? new Size(Info.Bounds.Width - Info.Client.Width,
						Info.Bounds.Height - Info.Client.Height) : Size.Empty;
				return Size.Add(documentMinSize, borderMinSize);
			}
		}
		internal void SortPinnedItems() {
			if(Items != null || Items.Count > 1) {
				Document savedSelectedDocumet = SelectedDocument;
				Items.BeginUpdate();
				int i = 0;
				foreach(Document item in GetPinnedDocuments()) {
					Items.List.Remove(item);
					Items.List.Insert(i, item);
					i++;
				}
				Items.EndUpdate();
				if(savedSelectedDocumet != null && SelectedItemIndex != Items.IndexOf(savedSelectedDocumet))
					selectedItemIndexCore = Items.IndexOf(savedSelectedDocumet);
			}
			LayoutChanged();
		}
		List<Document> GetPinnedDocuments() {
			List<Document> result = new List<Document>();
			foreach(Document document in Items) {
				if(document.Pinned)
					result.Add(document);
			}
			return result;
		}
		bool IRelativeLengthElement.Visible { get; set; }
		Widget.Length lengthCore;
		Widget.Length IRelativeLengthElement.Length { get { return lengthCore; } set { lengthCore = value; } }
		int actualLengthCore;
		int IRelativeLengthElement.ActualLength { get { return actualLengthCore; } set { actualLengthCore = value; } }
		Orientation IRelativeLengthElement.Orientation { get; set; }
		Rectangle IRelativeLengthElement.LayoutRect { get; set; }
		void IRelativeLengthElement.CaclLayout(Graphics g, Rectangle bounds) {
			if(infoCore != null)
				infoCore.Calc(g, bounds);
		}
	}
	public class DocumentGroupCollection : BaseMutableListEx<DocumentGroup> {
		public bool Insert(int index, DocumentGroup group) {
			return InsertCore(index, group);
		}
	}
	public class DocumentGroupProperties : DevExpress.XtraBars.Docking2010.Base.BaseProperties, IDocumentGroupProperties {
		public DocumentGroupProperties() {
			headerButtonsCore = new DevExpress.XtraTab.Buttons.CustomHeaderButtonCollection();
			CustomHeaderButtons.CollectionChanged += OnCustomHeaderButtonsCollectionChanged;
			SetDefaultValueCore("DestroyOnRemovingChildren", true);
			SetDefaultValueCore("ShowTabHeader", true);
			SetDefaultValueCore("ShowDocumentSelectorButton", true);
			SetDefaultValueCore("HeaderButtons", DevExpress.XtraTab.TabButtons.Default);
			SetDefaultValueCore("HeaderButtonsShowMode", DevExpress.XtraTab.TabButtonShowMode.Default);
			SetDefaultValueCore("HeaderLocation", DevExpress.XtraTab.TabHeaderLocation.Top);
		}
		protected override void OnDispose() {
			CustomHeaderButtons.CollectionChanged -= OnCustomHeaderButtonsCollectionChanged;
			CustomHeaderButtons.Clear();
			base.OnDispose();
		}
		void OnCustomHeaderButtonsCollectionChanged(object sender, CollectionChangeEventArgs e) {
			PropertiesChanged("CustomHeaderButtons");
		}
		[DefaultValue(0), RefreshProperties(RefreshProperties.Repaint)]
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Layout")]
		public int MaxDocuments {
			get { return GetValueCore<int>("MaxDocuments"); }
			set { SetValueCore("MaxDocuments", value); }
		}
		[DefaultValue(true), RefreshProperties(RefreshProperties.Repaint)]
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(XtraEditors.CategoryName.Behavior)]
		public bool DestroyOnRemovingChildren {
			get { return GetValueCore<bool>("DestroyOnRemovingChildren"); }
			set { SetValueCore("DestroyOnRemovingChildren", value); }
		}
		[DefaultValue(true), RefreshProperties(RefreshProperties.Repaint)]
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		[XtraSerializablePropertyId(OptionsLayout.UIProperty), Category(XtraEditors.CategoryName.Appearance)]
		public bool ShowTabHeader {
			get { return GetValueCore<bool>("ShowTabHeader"); }
			set { SetValueCore("ShowTabHeader", value); }
		}
		[DefaultValue(true), RefreshProperties(RefreshProperties.Repaint)]
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		[XtraSerializablePropertyId(OptionsLayout.UIProperty), Category(XtraEditors.CategoryName.Appearance)]
		public bool ShowDocumentSelectorButton {
			get { return GetValueCore<bool>("ShowDocumentSelectorButton"); }
			set { SetValueCore("ShowDocumentSelectorButton", value); }
		}
		[DefaultValue(DevExpress.XtraTab.TabHeaderLocation.Top), RefreshProperties(RefreshProperties.Repaint)]
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		[XtraSerializablePropertyId(OptionsLayout.UIProperty), Category(XtraEditors.CategoryName.Appearance)]
		public DevExpress.XtraTab.TabHeaderLocation HeaderLocation {
			get { return GetValueCore<DevExpress.XtraTab.TabHeaderLocation>("HeaderLocation"); }
			set { SetValueCore("HeaderLocation", value); }
		}
		[DefaultValue(DevExpress.XtraTab.TabOrientation.Default), RefreshProperties(RefreshProperties.Repaint)]
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		[XtraSerializablePropertyId(OptionsLayout.UIProperty), Category(XtraEditors.CategoryName.Appearance)]
		public DevExpress.XtraTab.TabOrientation HeaderOrientation {
			get { return GetValueCore<DevExpress.XtraTab.TabOrientation>("HeaderOrientation"); }
			set { SetValueCore("HeaderOrientation", value); }
		}
		[DefaultValue(DevExpress.XtraTab.TabPageImagePosition.Near), RefreshProperties(RefreshProperties.Repaint)]
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		[XtraSerializablePropertyId(OptionsLayout.UIProperty), Category(XtraEditors.CategoryName.Appearance)]
		public DevExpress.XtraTab.TabPageImagePosition PageImagePosition {
			get { return GetValueCore<DevExpress.XtraTab.TabPageImagePosition>("PageImagePosition"); }
			set { SetValueCore("PageImagePosition", value); }
		}
		[DefaultValue(DevExpress.XtraTab.ClosePageButtonShowMode.Default), RefreshProperties(RefreshProperties.Repaint)]
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(XtraEditors.CategoryName.Appearance)]
		public DevExpress.XtraTab.ClosePageButtonShowMode ClosePageButtonShowMode {
			get { return GetValueCore<DevExpress.XtraTab.ClosePageButtonShowMode>("ClosePageButtonShowMode"); }
			set { SetValueCore("ClosePageButtonShowMode", value); }
		}
		[DefaultValue(DevExpress.XtraTab.PinPageButtonShowMode.Default), RefreshProperties(RefreshProperties.Repaint)]
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(XtraEditors.CategoryName.Appearance)]
		public DevExpress.XtraTab.PinPageButtonShowMode PinPageButtonShowMode {
			get { return GetValueCore<DevExpress.XtraTab.PinPageButtonShowMode>("PinPageButtonShowMode"); }
			set { SetValueCore("PinPageButtonShowMode", value); }
		}
		[DefaultValue(DevExpress.XtraTabbedMdi.CloseTabOnMiddleClick.Default), RefreshProperties(RefreshProperties.Repaint)]
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(XtraEditors.CategoryName.Behavior)]
		public DevExpress.XtraTabbedMdi.CloseTabOnMiddleClick CloseTabOnMiddleClick {
			get { return GetValueCore<DevExpress.XtraTabbedMdi.CloseTabOnMiddleClick>("CloseTabOnMiddleClick"); }
			set { SetValueCore("CloseTabOnMiddleClick", value); }
		}
		[DefaultValue(DevExpress.XtraTab.TabButtons.Default), RefreshProperties(RefreshProperties.Repaint)]
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(XtraEditors.CategoryName.Appearance)]
		[Editor(typeof(DevExpress.Utils.Editors.AttributesEditor), typeof(System.Drawing.Design.UITypeEditor))]
		[XtraSerializablePropertyId(OptionsLayout.UIProperty)]
		public DevExpress.XtraTab.TabButtons HeaderButtons {
			get { return GetValueCore<DevExpress.XtraTab.TabButtons>("HeaderButtons"); }
			set { SetValueCore("HeaderButtons", value); }
		}
		[DefaultValue(DevExpress.XtraTab.TabButtonShowMode.Default), RefreshProperties(RefreshProperties.Repaint)]
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		[XtraSerializablePropertyId(OptionsLayout.UIProperty), Category(XtraEditors.CategoryName.Appearance)]
		public DevExpress.XtraTab.TabButtonShowMode HeaderButtonsShowMode {
			get { return GetValueCore<DevExpress.XtraTab.TabButtonShowMode>("HeaderButtonsShowMode"); }
			set { SetValueCore("HeaderButtonsShowMode", value); }
		}
		DevExpress.XtraTab.Buttons.CustomHeaderButtonCollection headerButtonsCore;
		[RefreshProperties(RefreshProperties.All)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DevExpress.XtraTab.Buttons.CustomHeaderButtonCollection CustomHeaderButtons {
			get { return headerButtonsCore; }
		}
		protected bool ShouldSerializeCustomHeaderButtons() {
			return CustomHeaderButtons != null && CustomHeaderButtons.Count > 0;
		}
		protected void ResetCustomHeaderButtons() {
			CustomHeaderButtons.Clear();
		}
		protected override IBaseProperties CloneCore() {
			return new DocumentGroupProperties();
		}
		protected override bool ShouldSerializeCore() {
			return base.ShouldSerializeCore() || ShouldSerializeCustomHeaderButtons();
		}
		protected override void ResetCore() {
			ResetCustomHeaderButtons();
			base.ResetCore();
		}
		protected override void AssignCore(IPropertiesProvider source) {
			base.AssignCore(source);
			IDocumentGroupProperties properties = source as IDocumentGroupProperties;
			if(properties != null) {
				CustomHeaderButtons.Assign(properties.CustomHeaderButtons);
			}
		}
	}
	public class DocumentGroupDefaultProperties : DevExpress.XtraBars.Docking2010.Base.BaseDefaultProperties, IDocumentGroupDefaultProperties {
		public DocumentGroupDefaultProperties(IDocumentGroupProperties parentProperties)
			: base(parentProperties) {
			headerButtonsCore = new DevExpress.XtraTab.Buttons.CustomHeaderButtonCollection();
			CustomHeaderButtons.CollectionChanged += OnCustomHeaderButtonsCollectionChanged;
			SetDefaultValueCore("DestroyOnRemovingChildren", DevExpress.Utils.DefaultBoolean.Default);
			SetDefaultValueCore("ShowTabHeader", DevExpress.Utils.DefaultBoolean.Default);
			SetDefaultValueCore("ShowDocumentSelectorButton", DevExpress.Utils.DefaultBoolean.Default);
			SetDefaultValueCore("HeaderButtons", DevExpress.XtraTab.TabButtons.Default);
			SetConverter("DestroyOnRemovingChildren", GetDefaultBooleanConverter(true));
			SetConverter("ShowTabHeader", GetDefaultBooleanConverter(true));
			SetConverter("ShowDocumentSelectorButton", GetDefaultBooleanConverter(true));
			SetConverter("HeaderButtons", GetNullableValueConverter(DevExpress.XtraTab.TabButtons.Default));
			SetConverter("HeaderLocation", GetNullableValueConverter(DevExpress.XtraTab.TabHeaderLocation.Top));
		}
		protected override void OnDispose() {
			CustomHeaderButtons.CollectionChanged -= OnCustomHeaderButtonsCollectionChanged;
			CustomHeaderButtons.Clear();
			base.OnDispose();
		}
		void OnCustomHeaderButtonsCollectionChanged(object sender, CollectionChangeEventArgs e) {
			PropertiesChanged("CustomHeaderButtons");
		}
		[DefaultValue(null), RefreshProperties(RefreshProperties.Repaint)]
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Layout")]
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public int? MaxDocuments {
			get { return GetValueCore<int?>("MaxDocuments"); }
			set { SetValueCore("MaxDocuments", value); }
		}
		[DefaultValue(DevExpress.Utils.DefaultBoolean.Default), RefreshProperties(RefreshProperties.Repaint)]
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(XtraEditors.CategoryName.Behavior)]
		public DevExpress.Utils.DefaultBoolean DestroyOnRemovingChildren {
			get { return GetValueCore<DevExpress.Utils.DefaultBoolean>("DestroyOnRemovingChildren"); }
			set { SetValueCore("DestroyOnRemovingChildren", value); }
		}
		[DefaultValue(DevExpress.Utils.DefaultBoolean.Default), RefreshProperties(RefreshProperties.Repaint)]
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		[XtraSerializablePropertyId(OptionsLayout.UIProperty), Category(XtraEditors.CategoryName.Appearance)]
		public DevExpress.Utils.DefaultBoolean ShowTabHeader {
			get { return GetValueCore<DevExpress.Utils.DefaultBoolean>("ShowTabHeader"); }
			set { SetValueCore("ShowTabHeader", value); }
		}
		[DefaultValue(DevExpress.Utils.DefaultBoolean.Default), RefreshProperties(RefreshProperties.Repaint)]
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		[XtraSerializablePropertyId(OptionsLayout.UIProperty), Category(XtraEditors.CategoryName.Appearance)]
		public DevExpress.Utils.DefaultBoolean ShowDocumentSelectorButton {
			get { return GetValueCore<DevExpress.Utils.DefaultBoolean>("ShowDocumentSelectorButton"); }
			set { SetValueCore("ShowDocumentSelectorButton", value); }
		}
		[DefaultValue(null), RefreshProperties(RefreshProperties.Repaint)]
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		[XtraSerializablePropertyId(OptionsLayout.UIProperty), Category(XtraEditors.CategoryName.Appearance)]
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public DevExpress.XtraTab.TabHeaderLocation? HeaderLocation {
			get { return GetValueCore<DevExpress.XtraTab.TabHeaderLocation?>("HeaderLocation"); }
			set { SetValueCore("HeaderLocation", value); }
		}
		[DefaultValue(DevExpress.XtraTab.TabOrientation.Default), RefreshProperties(RefreshProperties.Repaint)]
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		[XtraSerializablePropertyId(OptionsLayout.UIProperty), Category(XtraEditors.CategoryName.Appearance)]
		public DevExpress.XtraTab.TabOrientation HeaderOrientation {
			get { return GetValueCore<DevExpress.XtraTab.TabOrientation>("HeaderOrientation"); }
			set { SetValueCore("HeaderOrientation", value); }
		}
		[DefaultValue(null), RefreshProperties(RefreshProperties.Repaint)]
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		[XtraSerializablePropertyId(OptionsLayout.UIProperty), Category(XtraEditors.CategoryName.Appearance)]
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public DevExpress.XtraTab.TabPageImagePosition? PageImagePosition {
			get { return GetValueCore<DevExpress.XtraTab.TabPageImagePosition?>("PageImagePosition"); }
			set { SetValueCore("PageImagePosition", value); }
		}
		[DefaultValue(DevExpress.XtraTab.ClosePageButtonShowMode.Default), RefreshProperties(RefreshProperties.Repaint)]
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(XtraEditors.CategoryName.Appearance)]
		public DevExpress.XtraTab.ClosePageButtonShowMode ClosePageButtonShowMode {
			get { return GetValueCore<DevExpress.XtraTab.ClosePageButtonShowMode>("ClosePageButtonShowMode"); }
			set { SetValueCore("ClosePageButtonShowMode", value); }
		}
		[DefaultValue(DevExpress.XtraTab.PinPageButtonShowMode.Default), RefreshProperties(RefreshProperties.Repaint)]
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(XtraEditors.CategoryName.Appearance)]
		public DevExpress.XtraTab.PinPageButtonShowMode PinPageButtonShowMode {
			get { return GetValueCore<DevExpress.XtraTab.PinPageButtonShowMode>("PinPageButtonShowMode"); }
			set { SetValueCore("PinPageButtonShowMode", value); }
		}
		[DefaultValue(DevExpress.XtraTabbedMdi.CloseTabOnMiddleClick.Default), RefreshProperties(RefreshProperties.Repaint)]
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(XtraEditors.CategoryName.Behavior)]
		public DevExpress.XtraTabbedMdi.CloseTabOnMiddleClick CloseTabOnMiddleClick {
			get { return GetValueCore<DevExpress.XtraTabbedMdi.CloseTabOnMiddleClick>("CloseTabOnMiddleClick"); }
			set { SetValueCore("CloseTabOnMiddleClick", value); }
		}
		[DefaultValue(0), RefreshProperties(RefreshProperties.Repaint)]
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		[XtraSerializablePropertyId(OptionsLayout.UIProperty), Category("Layout")]
		public int TabWidth {
			get { return GetValueCore<int>("TabWidth"); }
			set { SetValueCore("TabWidth", value); }
		}
		[DefaultValue(DevExpress.XtraTab.TabButtons.Default), RefreshProperties(RefreshProperties.Repaint)]
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		[XtraSerializablePropertyId(OptionsLayout.UIProperty), Category(XtraEditors.CategoryName.Appearance)]
		public DevExpress.XtraTab.TabButtons HeaderButtons {
			get { return GetValueCore<DevExpress.XtraTab.TabButtons>("HeaderButtons"); }
			set { SetValueCore("HeaderButtons", value); }
		}
		[DefaultValue(DevExpress.XtraTab.TabButtonShowMode.Default), RefreshProperties(RefreshProperties.Repaint)]
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		[XtraSerializablePropertyId(OptionsLayout.UIProperty), Category(XtraEditors.CategoryName.Appearance)]
		public DevExpress.XtraTab.TabButtonShowMode HeaderButtonsShowMode {
			get { return GetValueCore<DevExpress.XtraTab.TabButtonShowMode>("HeaderButtonsShowMode"); }
			set { SetValueCore("HeaderButtonsShowMode", value); }
		}
		DevExpress.XtraTab.Buttons.CustomHeaderButtonCollection headerButtonsCore;
		[RefreshProperties(RefreshProperties.All)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DevExpress.XtraTab.Buttons.CustomHeaderButtonCollection CustomHeaderButtons {
			get { return headerButtonsCore; }
		}
		protected bool ShouldSerializeCustomHeaderButtons() {
			return CustomHeaderButtons != null && CustomHeaderButtons.Count > 0;
		}
		protected void ResetCustomHeaderButtons() {
			CustomHeaderButtons.Clear();
		}
		protected override IBaseProperties CloneCore() {
			return new DocumentGroupDefaultProperties(ParentProperties as IDocumentGroupProperties);
		}
		protected override bool ShouldSerializeCore() {
			return base.ShouldSerializeCore() || ShouldSerializeCustomHeaderButtons();
		}
		protected override void ResetCore() {
			ResetCustomHeaderButtons();
			base.ResetCore();
		}
		protected override void AssignCore(IPropertiesProvider source) {
			base.AssignCore(source);
			IDocumentGroupDefaultProperties properties = source as IDocumentGroupDefaultProperties;
			if(properties != null) {
				CustomHeaderButtons.Assign(properties.CustomHeaderButtons);
			}
		}
		[Browsable(false)]
		public bool ActualDestroyOnRemovingChildren {
			get { return GetActualValue<DevExpress.Utils.DefaultBoolean, bool>("DestroyOnRemovingChildren"); }
		}
		[Browsable(false)]
		public int ActualMaxDocuments {
			get { return GetActualValueFromNullable<int>("MaxDocuments"); }
		}
		[Browsable(false)]
		public bool HasTabHeader {
			get { return GetActualValue<DevExpress.Utils.DefaultBoolean, bool>("ShowTabHeader"); }
		}
		[Browsable(false)]
		public DevExpress.XtraTab.TabHeaderLocation ActualHeaderLocation {
			get { return GetActualValueFromNullable<DevExpress.XtraTab.TabHeaderLocation>("HeaderLocation"); }
		}
		[Browsable(false)]
		public DevExpress.XtraTab.TabOrientation ActualHeaderOrientation {
			get { return GetActualValue<DevExpress.XtraTab.TabOrientation, DevExpress.XtraTab.TabOrientation>("HeaderOrientation"); }
		}
		[Browsable(false)]
		public DevExpress.XtraTab.TabPageImagePosition ActualPageImagePosition {
			get { return GetActualValue<DevExpress.XtraTab.TabPageImagePosition, DevExpress.XtraTab.TabPageImagePosition>("PageImagePosition"); }
		}
		[Browsable(false)]
		public DevExpress.XtraTab.ClosePageButtonShowMode ActualClosePageButtonShowMode {
			get { return GetActualValue<DevExpress.XtraTab.ClosePageButtonShowMode, DevExpress.XtraTab.ClosePageButtonShowMode>("ClosePageButtonShowMode"); }
		}
		[Browsable(false)]
		public DevExpress.XtraTab.PinPageButtonShowMode ActualPinPageButtonShowMode {
			get { return GetActualValue<DevExpress.XtraTab.PinPageButtonShowMode, DevExpress.XtraTab.PinPageButtonShowMode>("PinPageButtonShowMode"); }
		}
		[Browsable(false)]
		public DevExpress.XtraTabbedMdi.CloseTabOnMiddleClick ActualCloseTabOnMiddleClick {
			get { return GetActualValue<DevExpress.XtraTabbedMdi.CloseTabOnMiddleClick, DevExpress.XtraTabbedMdi.CloseTabOnMiddleClick>("CloseTabOnMiddleClick"); }
		}
		[Browsable(false)]
		public bool HasDocumentSelectorButton {
			get { return GetActualValue<DevExpress.Utils.DefaultBoolean, bool>("ShowDocumentSelectorButton"); }
		}
		[Browsable(false)]
		public IEnumerable<DevExpress.XtraTab.Buttons.CustomHeaderButton> GetActualCustomHeaderButtons() {
			IList<DevExpress.XtraTab.Buttons.CustomHeaderButton> buttons = new List<DevExpress.XtraTab.Buttons.CustomHeaderButton>();
			IDocumentGroupProperties parentProperties = ParentProperties as IDocumentGroupProperties;
			foreach(DevExpress.XtraTab.Buttons.CustomHeaderButton button in CustomHeaderButtons) {
				buttons.Add(button);
			}
			if(parentProperties != null) {
				foreach(DevExpress.XtraTab.Buttons.CustomHeaderButton button in parentProperties.CustomHeaderButtons) {
					buttons.Add(button);
				}
			}
			return buttons;
		}
		[Browsable(false)]
		public DevExpress.XtraTab.TabButtons GetActualHeaderButtons {
			get { return GetActualValue<DevExpress.XtraTab.TabButtons, DevExpress.XtraTab.TabButtons>("HeaderButtons"); }
		}
		[Browsable(false)]
		public DevExpress.XtraTab.TabButtonShowMode ActualHeaderButtonsShowMode {
			get { return GetActualValue<DevExpress.XtraTab.TabButtonShowMode, DevExpress.XtraTab.TabButtonShowMode>("HeaderButtonsShowMode"); }
		}
	}
}
