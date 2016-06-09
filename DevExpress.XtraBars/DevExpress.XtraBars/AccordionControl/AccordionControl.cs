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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using DevExpress.Data.Filtering;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Filtering;
using DevExpress.Utils.Gesture;
using DevExpress.Utils.Serializing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
namespace DevExpress.XtraBars.Navigation {
	public enum AnimationType { None, Simple, Spline }
	public enum TextPosition { AfterImage, BeforeImage }
	public enum ImageLayoutMode { OriginalSize, Squeeze, Stretch }
	public enum ScrollBarMode { Touch, Default, Hidden, Auto, [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)] AutoCollapse }
	public enum ShowFilterControl { Always, Never, Auto }
	public enum ExpandElementMode { Single, Multiple, SingleItem }
	public enum AccordionElementState { Expanded, Collapsed }
	[Designer("DevExpress.XtraBars.Navigation.Design.AccordionControlDesigner, " + AssemblyInfo.SRAssemblyBarsDesign, typeof(System.ComponentModel.Design.IDesigner)),
	 ToolboxTabName(AssemblyInfo.DXTabNavigation), ToolboxBitmap(typeof(ToolboxIcons.ToolboxIconsRootNS), "AccordionControl"), DXToolboxItem(DXToolboxItemKind.Regular), Description("A NavBar control allowing you to create an advanced hierarchical navigation menu.")]
	public class AccordionControl : BaseStyleControl, IContextItemCollectionOwner, IContextItemCollectionOptionsOwner, ISearchControlClient,
		IElementCollectionContainer, ISupportInitialize, IXtraSerializableLayout, IToolTipControlClient, IMouseWheelSupport, IContextItemProvider,
		IChildControlsIgnoreMouseWheel, IFilteringUIProvider {
		private static readonly object contextButtonClick = new object();
		private static readonly object contextButtonCustomize = new object();
		private static readonly object customContextButtonToolTip = new object();
		private static readonly object getContentContainer = new object();
		private static readonly object hasContentContainer = new object();
		private static readonly object itemContentContainerHidden = new object();
		private static readonly object elementClick = new object();
		private static readonly object selectedElementChanged = new object();
		private static readonly object customDrawElement = new object();
		private static readonly object customElementText = new object();
		private static readonly object filterContent = new object();
		private static readonly object expandStateChanged = new object();
		private static readonly object expandStateChanging = new object();
		private static readonly object elementDragOver = new object();
		private static readonly object startElementDragging = new object();
		private static readonly object endElementDragging = new object();
		AccordionControlDesignTimeManager designManager;
		AccordionControlAppearances appearance;
		bool allowItemSelection, expandGroupOnHeaderClick, allowElementDragging;
		DefaultBoolean allowGlyphSkinning;
		OptionsMinimizing optionsMinimizing;
		public AccordionControl()
			: base() {
			SetStyle(ControlConstants.DoubleBuffer, true);
			this.itemHeight = this.groupHeight = 0;
			this.designManager = CreateDesignManager();
			this.showGroupExpandButtons = this.expandGroupOnHeaderClick = true;
			this.allowElementDragging = this.allowItemSelection = this.useDefaultFilterControl = false;
			this.showFilterControl = ShowFilterControl.Never;
			this.appearance = new AccordionControlAppearances(this);
			this.allowGlyphSkinning = DefaultBoolean.Default;
			this.expandElementMode = ExpandElementMode.SingleItem;
			BorderStyle = BorderStyles.NoBorder;
		}
		AccordionControlElement selectedElement;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DefaultValue(null)]
		public AccordionControlElement SelectedElement {
			get {
				return selectedElement;
			}
			set {
				SetSelectedElement(value);
			}
		}
		public void SelectElement(AccordionControlElement element) {
			SelectedElement = element;
		}
		protected internal void SetSelectedElement(AccordionControlElement element) {
			if(!AllowItemSelection || (element != null && element.Style != ElementStyle.Item) || this.selectedElement == element)
				return;
			KeyNavHelper.SelectedElement = null;
			this.selectedElement = element;
			ControlInfo.ClearPaintAppearance();
			Invalidate();
			SelectedElementChangedEventArgs eventArgs = new SelectedElementChangedEventArgs(element);
			RaiseSelectedElementChanged(eventArgs);
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override BorderStyles BorderStyle {
			get { return base.BorderStyle; }
			set { base.BorderStyle = value; }
		}
		protected override void OnLookAndFeelChanged(object sender, EventArgs e) {
			base.OnLookAndFeelChanged(sender, e);
			ControlInfo.ClearPaintAppearance();
			ForceRefreshViewInfo();
			if(OptionsMinimizing.AllowMinimizing && OptionsMinimizing.State == AccordionControlState.Minimized)
				Width = GetMinimizedWidth();
			ControlInfo.Scrollers.VScrollBar.OnAction(DevExpress.XtraEditors.ScrollNotifyAction.Hide);
		}
		protected internal virtual void OnAppearanceChanged() {
			ControlInfo.ClearPaintAppearance();
			ForceRefreshViewInfo();
			if(GetFilterControl() != null)
				GetFilterControl().UpdateLayout();
			if(!ControlInfo.IsInAnimation) Refresh();
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			ForceRefresh();
			ControlInfo.Scrollers.VScrollBar.OnAction(DevExpress.XtraEditors.ScrollNotifyAction.Resize);
			UpdateFilterControlWidth();
		}
		private void UpdateFilterControlWidth() {
			if(GetFilterControl() != null)
				GetFilterControl().Width = Width;
		}
		protected override Size DefaultSize { get { return new Size(260, 300); } }
		protected override Size CalcSizeableMaxSize() {
			return Size.Empty;
		}
		protected internal new bool IsRightToLeft { get { return base.IsRightToLeft; } }
		protected override DevExpress.XtraEditors.Drawing.BaseControlPainter CreatePainter() {
			return new AccordionControlPainter();
		}
		protected override DevExpress.XtraEditors.ViewInfo.BaseStyleControlViewInfo CreateViewInfo() {
			return new AccordionControlViewInfo(this);
		}
		public virtual void OnElementCollectionChanged() {
			LayoutChanged();
			FireChanged();
		}
		AccordionContextItemCollectionOptions contextButtonsOptions;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlContextButtonsOptions"),
#endif
 Category(CategoryName.Appearance),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof(ExpandableObjectConverter))]
		public AccordionContextItemCollectionOptions ContextButtonsOptions {
			get {
				if(contextButtonsOptions == null) {
					contextButtonsOptions = CreateContextButtonOptions();
				}
				return contextButtonsOptions;
			}
		}
		protected virtual AccordionContextItemCollectionOptions CreateContextButtonOptions() {
			return new AccordionContextItemCollectionOptions(this);
		}
		void IContextItemCollectionOptionsOwner.OnOptionsChanged(string propertyName, object oldValue, object newValue) {
			OnPropertiesChanged();
		}
		ContextAnimationType IContextItemCollectionOptionsOwner.AnimationType { get { return ContextAnimationType.OpacityAnimation; } }
		ContextItemCollection groupContextButtons;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlGroupContextButtons"),
#endif
 Category(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ContextItemCollection GroupContextButtons {
			get {
				if(groupContextButtons == null)
					groupContextButtons = new ContextItemCollection(this);
				groupContextButtons.Options = ContextButtonsOptions;
				return groupContextButtons;
			}
		}
		protected AccordionControlViewInfo ControlViewInfo { get { return ViewInfo as AccordionControlViewInfo; } }
		public AccordionControlElement AddGroup() {
			AccordionControlElement group = new AccordionControlElement(ElementStyle.Group);
			Elements.Add(group);
			return group;
		}
		protected virtual AccordionControlDesignTimeManager CreateDesignManager() {
			return new AccordionControlDesignTimeManager(this);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public AccordionControlDesignTimeManager DesignManager { get { return designManager; } }
		ContextItemCollection itemContextButtons;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlItemContextButtons"),
#endif
 Category(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ContextItemCollection ItemContextButtons {
			get {
				if(itemContextButtons == null)
					itemContextButtons = new ContextItemCollection(this);
				itemContextButtons.Options = ContextButtonsOptions;
				return itemContextButtons;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlContextButtonCustomize"),
#endif
 Category(CategoryName.Events)]
		public event AccordionControlContextButtonCustomizeEventHandler ContextButtonCustomize {
			add { Events.AddHandler(contextButtonCustomize, value); }
			remove { Events.RemoveHandler(contextButtonCustomize, value); }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlContextButtonClick"),
#endif
 Category(CategoryName.Events)]
		public event ContextItemClickEventHandler ContextButtonClick {
			add { Events.AddHandler(contextButtonClick, value); }
			remove { Events.RemoveHandler(contextButtonClick, value); }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlCustomContextButtonToolTip"),
#endif
 Category(CategoryName.Events)]
		public event AccordionControlContextButtonToolTipEventHandler CustomContextButtonToolTip {
			add { Events.AddHandler(customContextButtonToolTip, value); }
			remove { Events.RemoveHandler(customContextButtonToolTip, value); }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlElementClick"),
#endif
 Category(CategoryName.Events)]
		public event ElementClickEventHandler ElementClick {
			add { Events.AddHandler(elementClick, value); }
			remove { Events.RemoveHandler(elementClick, value); }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlSelectedElementChanged"),
#endif
 Category(CategoryName.Events)]
		public event SelectedElementChangedEventHandler SelectedElementChanged {
			add { Events.AddHandler(selectedElementChanged, value); }
			remove { Events.RemoveHandler(selectedElementChanged, value); }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlGetContentContainer"),
#endif
 Category(CategoryName.Events)]
		public event GetContentContainerEventHandler GetContentContainer {
			add { Events.AddHandler(getContentContainer, value); }
			remove { Events.RemoveHandler(getContentContainer, value); }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlHasContentContainer"),
#endif
 Category(CategoryName.Events)]
		public event HasContentContainerEventHandler HasContentContainer {
			add { Events.AddHandler(hasContentContainer, value); }
			remove { Events.RemoveHandler(hasContentContainer, value); }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlItemContentContainerHidden"),
#endif
 Category(CategoryName.Events)]
		public event ItemContentContainerHiddenEventHandler ItemContentContainerHidden {
			add { Events.AddHandler(itemContentContainerHidden, value); }
			remove { Events.RemoveHandler(itemContentContainerHidden, value); }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlFilterContent"),
#endif
 Category(CategoryName.Events)]
		public event FilterContentEventHandler FilterContent {
			add { Events.AddHandler(filterContent, value); }
			remove { Events.RemoveHandler(filterContent, value); }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlCustomDrawElement"),
#endif
 Category(CategoryName.Events)]
		public event CustomDrawElementEventHandler CustomDrawElement {
			add { Events.AddHandler(customDrawElement, value); }
			remove { Events.RemoveHandler(customDrawElement, value); }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlCustomElementText"),
#endif
 Category(CategoryName.Events)]
		public event CustomElementTextEventHandler CustomElementText {
			add { Events.AddHandler(customElementText, value); }
			remove { Events.RemoveHandler(customElementText, value); }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlExpandStateChanged"),
#endif
 Category(CategoryName.Events)]
		public event ExpandStateChangedEventHandler ExpandStateChanged {
			add { Events.AddHandler(expandStateChanged, value); }
			remove { Events.RemoveHandler(expandStateChanged, value); }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlExpandStateChanging"),
#endif
 Category(CategoryName.Events)]
		public event ExpandStateChangingEventHandler ExpandStateChanging {
			add { Events.AddHandler(expandStateChanging, value); }
			remove { Events.RemoveHandler(expandStateChanging, value); }
		}
		protected internal virtual void RaiseExpandStateChanging(ExpandStateChangingEventArgs e) {
			ExpandStateChangingEventHandler handler = Events[expandStateChanging] as ExpandStateChangingEventHandler;
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseExpandStateChanged(ExpandStateChangedEventArgs e) {
			ExpandStateChangedEventHandler handler = Events[expandStateChanged] as ExpandStateChangedEventHandler;
			if(handler != null) handler(this, e);
		}
		protected internal virtual string RaiseCustomElementText(CustomElementTextEventArgs e) {
			CustomElementTextEventHandler handler = Events[customElementText] as CustomElementTextEventHandler;
			if(handler != null) handler(this, e);
			return e.Text;
		}
		[ Category(CategoryName.Events)]
		public event StartAccordionElementDraggingEventHandler StartElementDragging {
			add { Events.AddHandler(startElementDragging, value); }
			remove { Events.RemoveHandler(startElementDragging, value); }
		}
		[ Category(CategoryName.Events)]
		public event AccordionElementDragOverEventHandler ElementDragOver {
			add { Events.AddHandler(elementDragOver, value); }
			remove { Events.RemoveHandler(elementDragOver, value); }
		}
		[ Category(CategoryName.Events)]
		public event EndAccordionElementDraggingEventHandler EndElementDragging {
			add { Events.AddHandler(endElementDragging, value); }
			remove { Events.RemoveHandler(endElementDragging, value); }
		}
		protected internal virtual void RaiseCustomDrawElement(CustomDrawElementEventArgs e) {
			CustomDrawElementEventHandler handler = Events[customDrawElement] as CustomDrawElementEventHandler;
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseStartElementDragging(StartAccordionElementDraggingEventArgs e) {
			StartAccordionElementDraggingEventHandler handler = Events[startElementDragging] as StartAccordionElementDraggingEventHandler;
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseElementDragOver(AccordionElementDragOverEventArgs e) {
			AccordionElementDragOverEventHandler handler = Events[elementDragOver] as AccordionElementDragOverEventHandler;
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseEndElementDragging(EndAccordionElementDraggingEventArgs e) {
			EndAccordionElementDraggingEventHandler handler = Events[endElementDragging] as EndAccordionElementDraggingEventHandler;
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseItemContentContainerHidden(ItemContentContainerHiddenEventArgs e) {
			ItemContentContainerHiddenEventHandler handler = Events[itemContentContainerHidden] as ItemContentContainerHiddenEventHandler;
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseGetContentContainer(GetContentContainerEventArgs e) {
			GetContentContainerEventHandler handler = Events[getContentContainer] as GetContentContainerEventHandler;
			if(handler != null) handler(this, e);
		}
		protected internal virtual bool RaiseHasContentContainer(HasContentContainerEventArgs e) {
			HasContentContainerEventHandler handler = Events[hasContentContainer] as HasContentContainerEventHandler;
			if(handler != null) handler(this, e);
			return e.HasContentContainer;
		}
		protected internal virtual bool RaiseElementClick(ElementClickEventArgs e) {
			ElementClickEventHandler handler = Events[elementClick] as ElementClickEventHandler;
			if(handler != null) handler(this, e);
			return e.Handled;
		}
		protected internal virtual void RaiseSelectedElementChanged(SelectedElementChangedEventArgs e) {
			SelectedElementChangedEventHandler handler = Events[selectedElementChanged] as SelectedElementChangedEventHandler;
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseFilterContent(FilterContentEventArgs e) {
			FilterContentEventHandler handler = Events[filterContent] as FilterContentEventHandler;
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseCustomContextButtonToolTip(AccordionControlContextButtonToolTipEventArgs e) {
			AccordionControlContextButtonToolTipEventHandler handler = (AccordionControlContextButtonToolTipEventHandler)Events[customContextButtonToolTip];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseContextButtonCustomize(AccordionControlContextButtonCustomizeEventArgs e) {
			AccordionControlContextButtonCustomizeEventHandler handler = (AccordionControlContextButtonCustomizeEventHandler)Events[contextButtonCustomize];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseContextItemClick(ContextItemClickEventArgs e) {
			ContextItemClickEventHandler handler = Events[contextButtonClick] as ContextItemClickEventHandler;
			if(handler != null)
				handler(this, e);
		}
		AccordionControlElementCollection elements;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlElements"),
#endif
 Category(CategoryName.Data), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(true, true, false, 999)]
		public AccordionControlElementCollection Elements {
			get {
				if(elements == null)
					elements = CreateElements();
				return elements;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlAllowItemSelection"),
#endif
 Category(CategoryName.Appearance), DefaultValue(false)]
		public bool AllowItemSelection {
			get { return allowItemSelection; }
			set {
				if(AllowItemSelection == value)
					return;
				allowItemSelection = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlExpandGroupOnHeaderClick"),
#endif
 Category(CategoryName.Appearance), DefaultValue(true)]
		public bool ExpandGroupOnHeaderClick {
			get { return expandGroupOnHeaderClick; }
			set {
				if(ExpandGroupOnHeaderClick == value)
					return;
				expandGroupOnHeaderClick = value;
				OnPropertiesChanged();
			}
		}
		[ Category(CategoryName.Appearance), DefaultValue(false)]
		public bool AllowElementDragging {
			get { return allowElementDragging; }
			set {
				if(AllowElementDragging == value)
					return;
				allowElementDragging = value;
			}
		}
		IDesignerHost DesignerHost { get { return GetService(typeof(IDesignerHost)) as IDesignerHost; } }
		internal object XtraCreateElementsItem(XtraItemEventArgs e) {
			return XtraCreateElementsItemCore(Elements);
		}
		protected internal object XtraCreateElementsItemCore(AccordionControlElementCollection col) {
			AccordionControlElement elem = col.Add();
			if(IsDesignMode && DesignerHost != null) DesignerHost.Container.Add(elem);
			return elem;
		}
		internal object XtraFindElementsItem(XtraItemEventArgs e) {
			return XtraFindElementsItemCore(e, Elements);
		}
		protected internal object XtraFindElementsItemCore(XtraItemEventArgs e, AccordionControlElementCollection col) {
			DevExpress.Utils.Serializing.Helpers.XtraPropertyInfo pi = e.Item.ChildProperties["Name"];
			if(pi == null) return null;
			string name = pi.Value.ToString();
			if(string.Equals(name, string.Empty)) return null;
			for(int i = 0; i < this.originalElements.Count; i++) {
				if(string.Equals(name, this.originalElements[i].Name)) {
					AccordionControlElement elem = this.originalElements[i];
					UpdateElementOwner(e.Owner, elem);
					this.originalElements.Remove(elem);
					return elem;
				}
			}
			return null;
		}
		protected void UpdateElementOwner(object newOwner, AccordionControlElement elem) {
			elem.OwnerElements.Remove(elem);
			if(newOwner is AccordionControl)
				((AccordionControl)newOwner).Elements.Add(elem);
			if(newOwner is AccordionControlElement)
				((AccordionControlElement)newOwner).Elements.Add(elem);
		}
		protected virtual AccordionControlElementCollection CreateElements() {
			return new AccordionControlElementCollection(this);
		}
		int distanceBetweenRootGroups = -1;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlDistanceBetweenRootGroups"),
#endif
 Category(CategoryName.Appearance), DefaultValue(-1)]
		public int DistanceBetweenRootGroups {
			get { return distanceBetweenRootGroups; }
			set {
				if(DistanceBetweenRootGroups == value)
					return;
				if(value < 0) value = -1;
				distanceBetweenRootGroups = value;
				OnPropertiesChanged();
			}
		}
		int itemHeight;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlItemHeight"),
#endif
 Category(CategoryName.Appearance), XtraSerializableProperty, DefaultValue(0)]
		public int ItemHeight {
			get { return itemHeight; }
			set {
				if(ItemHeight == value)
					return;
				itemHeight = value;
				ForceRefreshViewInfo();
				OnPropertiesChanged();
			}
		}
		int groupHeight;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlGroupHeight"),
#endif
 Category(CategoryName.Appearance), XtraSerializableProperty, DefaultValue(0)]
		public int GroupHeight {
			get { return groupHeight; }
			set {
				if(GroupHeight == value)
					return;
				groupHeight = value;
				ForceRefreshViewInfo();
				OnPropertiesChanged();
			}
		}
		[Obsolete("Use ExpandElementMode instead of AllowMultipleItemExpansion"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool AllowMultipleItemExpansion {
			get { return ExpandElementMode == ExpandElementMode.Multiple; }
			set {
				if(value) ExpandElementMode = ExpandElementMode.Multiple;
				else ExpandElementMode = ExpandElementMode.SingleItem;
			}
		}
		ExpandElementMode expandElementMode;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlExpandElementMode"),
#endif
 Category(CategoryName.Behavior), XtraSerializableProperty, DefaultValue(ExpandElementMode.SingleItem)]
		public ExpandElementMode ExpandElementMode {
			get { return expandElementMode; }
			set {
				if(ExpandElementMode == value)
					return;
				expandElementMode = value;
				OnPropertiesChanged();
			}
		}
		protected internal ExpandElementMode GetExpandElementMode() {
			if(IsDesignMode) return ExpandElementMode.Multiple;
			return ExpandElementMode;
		}
		ScrollBarMode scrollBarMode = ScrollBarMode.Default;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlScrollBarMode"),
#endif
 Category(CategoryName.Appearance), DefaultValue(ScrollBarMode.Default)]
		public ScrollBarMode ScrollBarMode {
			get { return scrollBarMode; }
			set {
				if(ScrollBarMode == value)
					return;
				scrollBarMode = value;
				ControlInfo.Scrollers.ApplyScrollBarMode();
				OnPropertiesChanged();
			}
		}
		AnimationType animationType = AnimationType.Spline;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlAnimationType"),
#endif
 Category(CategoryName.Appearance), XtraSerializableProperty, DefaultValue(AnimationType.Spline)]
		public AnimationType AnimationType {
			get { return animationType; }
			set {
				if(AnimationType == value)
					return;
				animationType = value;
				OnPropertiesChanged();
			}
		}
		object images;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlImages"),
#endif
 Category(CategoryName.Appearance),
		TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter)), DefaultValue(null)]
		public virtual object Images {
			get { return images; }
			set {
				if(images == value) return;
				images = value;
				ForceRefreshViewInfo();
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlAllowGlyphSkinning"),
#endif
 DefaultValue(DefaultBoolean.Default), Category("Appearance")]
		public virtual DefaultBoolean AllowGlyphSkinning {
			get { return allowGlyphSkinning; }
			set {
				if(AllowGlyphSkinning == value)
					return;
				allowGlyphSkinning = value;
				ForceRefreshViewInfo();
				OnPropertiesChanged();
			}
		}
		bool showGroupExpandButtons;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlShowGroupExpandButtons"),
#endif
 Category(CategoryName.Appearance), DefaultValue(true)]
		public bool ShowGroupExpandButtons {
			get { return showGroupExpandButtons; }
			set {
				if(showGroupExpandButtons == value) return;
				showGroupExpandButtons = value;
				ForceRefreshViewInfo();
				OnPropertiesChanged();
			}
		}
		bool searchMode = false;
		ShowFilterControl showFilterControl;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlShowFilterControl"),
#endif
 Category(CategoryName.Appearance), DefaultValue(ShowFilterControl.Never)]
		public ShowFilterControl ShowFilterControl {
			get { return showFilterControl; }
			set {
				if(showFilterControl == value) return;
				showFilterControl = value;
				UpdateFilterControl();
				CheckFilterControlVisibility();
				OnPropertiesChanged();
			}
		}
		IFilterContent filterControl;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlFilterControl"),
#endif
 Category(CategoryName.Appearance), DefaultValue(null)]
		public IFilterContent FilterControl {
			get { return filterControl; }
			set {
				if(filterControl == value) return;
				OnFilterControlChanging(value);
				filterControl = value;
				UpdateFilterControl();
			}
		}
		protected virtual OptionsMinimizing CreateOptionsMinimizing() {
			return new OptionsMinimizing(this);
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlOptionsMinimizing"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]
		public OptionsMinimizing OptionsMinimizing {
			get {
				if(optionsMinimizing == null) {
					optionsMinimizing = CreateOptionsMinimizing();
				}
				return optionsMinimizing;
			}
			set { optionsMinimizing = value; }
		}
		protected internal int GetMinimizedWidth() {
			if(OptionsMinimizing.MinimizedWidth >= 0) return OptionsMinimizing.MinimizedWidth;
			return ControlInfo.CalcCollapsedBestWidth();
		}
		protected internal IFilterContent GetFilterControl() {
			if(FilterControl != null) return FilterControl;
			return this.defaultFilterControl;
		}
		IFilterContent defaultFilterControl;
		protected void CheckDefaultFilterControl() {
			if(FilterControl != null || ShowFilterControl == ShowFilterControl.Never) {
				if(this.defaultFilterControl != null) {
					DisposeDefaultFilterControl();
				}
				return;
			}
			if(this.defaultFilterControl != null) return;
			this.defaultFilterControl = new AccordionSearchControl(this);
			this.useDefaultFilterControl = true;
		}
		protected void DisposeDefaultFilterControl() {
			this.defaultFilterControl.FilterValueChanged -= FilterValueChanged;
			((Control)this.defaultFilterControl).Dispose();
			this.defaultFilterControl = null;
		}
		protected void DefaultFilterContent(FilterContentEventArgs args) {
			string text = args.Element.Text.ToLower();
			string searchText = GetFilterControl().FilterValue == null ? string.Empty : GetFilterControl().FilterValue.ToString().ToLower();
			args.Visible = text.Contains(searchText);
		}
		bool useDefaultFilterControl;
		protected void UpdateFilterControl() {
			CheckDefaultFilterControl();
			if(GetFilterControl() != null && GetFilterControl().Control != null) {
				Controls.Add(GetFilterControl().Control);
				GetFilterControl().FilterValueChanged += FilterValueChanged;
				if(GetFilterControl().LookAndFeel != null)
					GetFilterControl().LookAndFeel.ParentLookAndFeel = LookAndFeel;
				GetFilterControl().Control.BringToFront();
			}
			CheckFilterControlVisibility();
			OnPropertiesChanged();
		}
		private void OnFilterControlChanging(IFilterContent newValue) {
			if(FilterControl != null && FilterControl.LookAndFeel != null) {
				FilterControl.LookAndFeel.ParentLookAndFeel = null;
				FilterControl.FilterValueChanged -= FilterValueChanged;
				Controls.Remove(FilterControl as Control);
			}
			if(this.useDefaultFilterControl && newValue != null) {
				DisposeDefaultFilterControl();
				this.useDefaultFilterControl = false;
			}
		}
		bool allowHtmlText = true;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlAllowHtmlText"),
#endif
 Category(CategoryName.Behavior), DefaultValue(true)]
		public bool AllowHtmlText {
			get {
				return allowHtmlText;
			}
			set {
				if(allowHtmlText == value)
					return;
				allowHtmlText = value;
				ForceRefreshViewInfo();
				OnPropertiesChanged();
			}
		}
		public void ExpandElement(AccordionControlElement element) {
			AccordionControlElement currentElement = element;
			AccordionControlElement firstCollapsedElement = null;
			while(currentElement != null) {
				if(!currentElement.Expanded) firstCollapsedElement = currentElement;
				currentElement = currentElement.OwnerElement;
			}
			currentElement = element;
			ControlInfo.ForceExpanding = true;
			try {
				while(currentElement != firstCollapsedElement) {
					if(!currentElement.Expanded) currentElement.InvertExpanded();
					currentElement = currentElement.OwnerElement;
				}
			}
			finally {
				ControlInfo.ForceExpanding = false;
				if(firstCollapsedElement != null) firstCollapsedElement.InvertExpanded();
			}
		}
		public void CollapseElement(AccordionControlElement element) {
			if(!element.Expanded) return;
			element.InvertExpanded();
		}
		public bool ExpandAll() {
			if(ExpandElementMode != ExpandElementMode.Multiple)
				return false;
			BeginUpdate();
			AccordionControlHelper.ForEachElement(c => { c.Expanded = true; }, Elements, false);
			EndUpdate();
			return true;
		}
		public void CollapseAll() {
			BeginUpdate();
			AccordionControlHelper.ForEachElement(c => { c.Expanded = false; }, Elements, false);
			EndUpdate();
		}
		public void MakeElementVisible(AccordionControlElement element) {
			AccordionControlHelper.MakeElementVisible(element, this);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				ControlInfo.DisposeExpandButton();
				if(GetFilterControl() != null)
					GetFilterControl().FilterValueChanged -= FilterValueChanged;
				if(Elements != null) {
					while(Elements.Count > 0) {
						Elements[0].Dispose();
					}
				}
			}
			base.Dispose(disposing);
		}
		void FilterValueChanged(object sender, EventArgs e) {
			ApplyFilterValue();
		}
		protected internal void CheckFilterControlVisibility() {
			if(GetFilterControl() == null) return;
			GetFilterControl().Visible = ShouldShowFilterControl();
		}
		protected internal bool ShouldShowFilterControl() {
			if(GetFilterControl() == null) return false;
			if(ShowFilterControl == ShowFilterControl.Always) return true;
			return ShowFilterControl == ShowFilterControl.Auto && searchMode;
		}
		AccordionControlHandler handler;
		protected internal AccordionControlHandler Handler {
			get {
				if(handler == null)
					handler = CreateHandler();
				return handler;
			}
		}
		bool shouldOptimizeAnimation = true;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DefaultValue(true)]
		public bool ShouldOptimizeAnimation {
			get { return shouldOptimizeAnimation; }
			set { shouldOptimizeAnimation = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Color GetBackColor() {
			return ControlInfo.GetBackColor();
		}
		bool locked = false;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void LockUpdate() {
			this.locked = true;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void UnlockUpdate() {
			this.locked = false;
		}
		protected internal void ForceRefresh() {
			if(this.locked || IsUpdate) return;
			ControlInfo.CheckContentTop();
			Refresh();
			ControlInfo.UpdateScrollBars();
		}
		protected AccordionControlHandler CreateHandler() {
			return new AccordionControlHandler(ControlInfo);
		}
		GestureHelper gestureHelper;
		protected internal GestureHelper GestureHelper {
			get {
				if(gestureHelper == null) {
					gestureHelper = new GestureHelper(Handler);
					gestureHelper.PanWithGutter = false;
				}
				return gestureHelper;
			}
		}
		protected override void WndProc(ref System.Windows.Forms.Message msg) {
			if(ProcessTouchEvents(ref msg)) return;
			base.WndProc(ref msg);
			CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref msg, this);
		}
		internal bool ProcessTouchEvents(ref System.Windows.Forms.Message msg) {
			return GestureHelper.WndProc(ref msg);
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			Handler.OnMouseLeave(e);
		}
		protected override void OnMouseEnter(EventArgs e) {
			base.OnMouseEnter(e);
			Handler.OnMouseEnter(e);
		}
		protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e) {
			base.OnMouseDown(e);
			Handler.OnMouseDown(e);
		}
		protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e) {
			base.OnMouseUp(e);
			Handler.OnMouseUp(e);
		}
		protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e) {
			base.OnMouseMove(e);
			Handler.OnMouseMove(e);
		}
		protected override void OnMouseWheel(System.Windows.Forms.MouseEventArgs e) {
			base.OnMouseWheel(e);
			if(XtraForm.ProcessSmartMouseWheel(this, e)) return;
			Handler.OnMouseWheel(e);
		}
		void IMouseWheelSupport.OnMouseWheel(System.Windows.Forms.MouseEventArgs e) {
			Handler.OnMouseWheel(e);
		}
		bool IContextItemCollectionOwner.IsDesignMode {
			get { return IsDesignMode; }
		}
		bool IContextItemCollectionOwner.IsRightToLeft {
			get { return IsRightToLeft; }
		}
		bool SuppressCollectionChangedEvent { get; set; }
		void IContextItemCollectionOwner.OnCollectionChanged() {
			if(!IsHandleCreated || IsLoading || SuppressCollectionChangedEvent)
				return;
			ViewInfo.IsReady = false;
			Refresh();
		}
		void IContextItemCollectionOwner.OnItemChanged(ContextItem item, string propertyName, object oldValue, object newValue) {
			if(!IsHandleCreated || IsLoading)
				return;
			if(propertyName == "Visibility") {
				Invalidate();
				Update();
			}
			else {
				ViewInfo.IsReady = false;
				Refresh();
			}
		}
		ToolTipControlInfo IToolTipControlClient.GetObjectInfo(Point point) {
			if(ControlInfo.HoverInfo.ItemInfo != null) {
				ToolTipControlInfo contextBtnInfo = ControlInfo.HoverInfo.ItemInfo.ContextButtonsViewInfo.GetToolTipInfo(point);
				if(contextBtnInfo != null) return contextBtnInfo;
			}
			return Handler == null ? null : Handler.GetTooltipObjectInfo();
		}
		protected internal AccordionControlViewInfo ControlInfo { get { return ViewInfo as AccordionControlViewInfo; } }
		protected internal void ForceRefreshViewInfo() {
			AccordionControlHelper.ForEachElementInfo(c => { c.UnsubscribeOnElementEvents(); }, ControlInfo.ElementsInfo);
			if(ControlInfo.ElementsInfo != null)
				ControlInfo.ElementsInfo.Clear();
			ForceRefresh();
		}
		public virtual AccordionControlHitInfo CalcHitInfo(Point p) {
			return ControlInfo.CalcHitInfo(p);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void ForceFireChanged() {
			FireChanged();
		}
		protected override bool ProcessDialogKey(Keys keyData) {
			if(keyData == Keys.Tab && KeyNavHelper.SelectedElement != null) return false;
			if(keyData == Keys.Down || keyData == Keys.Up || keyData == Keys.Left || keyData == Keys.Right || keyData == Keys.Home || keyData == Keys.End || keyData == Keys.Space) return false;
			return base.ProcessDialogKey(keyData);
		}
		protected override void OnLostFocus(EventArgs e) {
			if(KeyNavHelper.SelectedElement != null) {
				KeyNavHelper.SelectedElement = null;
				Invalidate();
			}
			base.OnLostFocus(e);
		}
		protected override void OnKeyUp(KeyEventArgs e) {
			if(KeyNavHelper.ProcessKey(e)) e.Handled = true;
			base.OnKeyUp(e);
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			if(e.KeyCode == Keys.F && e.Control) {
				if(ShowFilterControl == ShowFilterControl.Auto && !searchMode) {
					UpdateFilterControlState(true);
					return;
				}
				if(ShowFilterControl == ShowFilterControl.Always && GetFilterControl() != null) {
					GetFilterControl().Control.Focus();
					return;
				}
				if(CheckAttachedToSearchControl()) {
					searchControl.Focus();
					return;
				}
			}
			base.OnKeyDown(e);
		}
		protected internal void UpdateFilterControlState(bool visible) {
			this.searchMode = visible;
			CheckFilterControlVisibility();
			if(visible) {
				if(GetFilterControl() != null)
					GetFilterControl().Control.Focus();
			}
			else Focus();
			ForceRefresh();
		}
		protected override void OnKeyPress(KeyPressEventArgs e) {
			if(ShouldShowFilterControl()) {
				if(AccordionControlHelper.IsLetterOrNumberKey(e)) {
					NativeMethods.SendMessage(GetFilterControl().Control.Handle, MSG.WM_CHAR, (IntPtr)e.KeyChar, IntPtr.Zero);
					return;
				}
			}
			base.OnKeyPress(e);
		}
		KeyboardNavigationHelper keyNavHelper;
		protected internal KeyboardNavigationHelper KeyNavHelper {
			get {
				if(keyNavHelper == null) {
					keyNavHelper = new KeyboardNavigationHelper(this);
				}
				return keyNavHelper;
			}
		}
		#region ISearchControlClient Members
		bool ApplyFindFilterInItems(string searchString, AccordionControlElementCollection items) {
			bool found = false;
			foreach(AccordionControlElement childElement in items) {
				string lowerText = childElement.Text.ToLower();
				if(lowerText.Contains(searchString)) {
					childElement.SetRealVisible(true, true);
					found = true;
				}
				else {
					if(childElement.Style == ElementStyle.Group) {
						bool isVisible = ApplyFindFilterInItems(searchString, childElement.Elements);
						if(!isVisible) continue;
						childElement.SetRealVisible(true, false);
						found = true;
					}
				}
			}
			return found;
		}
		void ISearchControlClient.ApplyFindFilter(SearchInfoBase searchInfo) {
			SearchCriteriaInfo info = searchInfo as SearchCriteriaInfo;
			GroupOperator groupOperator = info.CriteriaOperator as GroupOperator;
			if(!ReferenceEquals(groupOperator, null)) {
				foreach(AccordionControlElement group in Elements)
					group.SetRealVisible(false, true);
				foreach(var operand in groupOperator.Operands) {
					FunctionOperator fOperator = operand as FunctionOperator;
					if(!ReferenceEquals(fOperator, null)) ApplyFindFilterCore(fOperator);
				}
				return;
			}
			FunctionOperator functionOperator = info.CriteriaOperator as FunctionOperator;
			if(ReferenceEquals(functionOperator, null)) {
				ResetFilter();
			}
			else {
				foreach(AccordionControlElement group in Elements)
					group.SetRealVisible(false, true);
				ApplyFindFilterCore(functionOperator);
			}
			LayoutChanged();
		}
		void ApplyFindFilterCore(FunctionOperator functionOperator) {
			string searchString = functionOperator.Operands[1].ToString();
			searchString = searchString.Substring(1, searchString.Length - 2);
			ApplyFilterValue(searchString);
		}
		protected bool IsElementVisible(AccordionControlElement elem) {
			if(GetFilterControl() == null) return true;
			FilterContentEventArgs e = new FilterContentEventArgs(elem, GetFilterControl().FilterValue);
			RaiseFilterContent(e);
			if(!e.Handled) DefaultFilterContent(e);
			return e.Visible;
		}
		protected void ApplyFilterValue() {
			foreach(AccordionControlElement group in Elements) {
				bool visible = IsElementVisible(group);
				if(visible) group.SetRealVisible(true, true);
				else group.SetRealVisible(ApplyFindFilterInItems(group.Elements), false);
			}
			ApplyFilterValueCore();
		}
		protected void ApplyFilterValueCore() {
			ControlInfo.ShowVisibleControls();
			ControlInfo.CheckContentTop();
		}
		protected void ApplyFilterValue(string searchString) {
			foreach(AccordionControlElement group in Elements) {
				string lowerText = group.Text.ToLower();
				if(lowerText.Contains(searchString)) group.SetRealVisible(true, true);
				else {
					bool isVisible = ApplyFindFilterInItems(searchString, group.Elements);
					if(!isVisible) continue;
					group.SetRealVisible(true, false);
				}
			}
			ApplyFilterValueCore();
		}
		bool ApplyFindFilterInItems(AccordionControlElementCollection items) {
			bool found = false;
			foreach(AccordionControlElement childElement in items) {
				bool visible = IsElementVisible(childElement);
				if(visible) {
					childElement.SetRealVisible(true, true);
					found = true;
				}
				else {
					if(childElement.Style == ElementStyle.Group) {
						childElement.SetRealVisible(ApplyFindFilterInItems(childElement.Elements), false);
						if(childElement.RealVisible) found = true;
					}
					else childElement.SetRealVisible(false, true);
				}
			}
			return found;
		}
		protected internal void ResetFilter() {
			foreach(AccordionControlElement group in Elements)
				group.SetRealVisible(true, true);
		}
		SearchControlProviderBase ISearchControlClient.CreateSearchProvider() {
			return new AccordionControlCriteriaProvider();
		}
		bool ISearchControlClient.IsAttachedToSearchControl { get { return CheckAttachedToSearchControl(); } }
		bool CheckAttachedToSearchControl() { return searchControl != null; }
		void ISearchControlClient.SetSearchControl(ISearchControl searchControl) {
			if(this.searchControl == searchControl) return;
			this.searchControl = searchControl;
			ResetFilter();
		}
		ISearchControl searchControl;
		#endregion
		public void BeginInit() {
			this.update++;
		}
		public void EndInit() {
			UpdateFilterControl();
			if(GetFilterControl() != null)
				GetFilterControl().Location = Point.Empty;
			CheckExpandElementState(Elements);
			ControlInfo.Helper.CheckControlsVisibility(Elements);
			AccordionControlHelper.ForEachElement(c =>
			{
				if(c.ImageUri != null) c.ImageUri.SetClient(c);
			}, Elements, false);
			this.update--;
			LayoutChanged();
		}
		int update = 0;
		protected internal bool IsUpdate { get { return this.update != 0; } }
		protected void CheckExpandElementState(AccordionControlElementCollection elements) {
			if(GetExpandElementMode() == ExpandElementMode.Single) {
				CheckSingleModeExpandElementState(elements);
			}
			if(GetExpandElementMode() == ExpandElementMode.SingleItem) {
				CheckSingleItemModeExpandElementState(elements, false);
			}
		}
		protected void CheckSingleModeExpandElementState(AccordionControlElementCollection elements) {
			bool hasExpanded = false;
			for(int i = 0; i < elements.Count; i++) {
				if(elements[i].Expanded) {
					if(hasExpanded) elements[i].Expanded = false;
					hasExpanded = true;
				}
				if(elements[i].Elements != null) CheckSingleModeExpandElementState(elements[i].Elements);
			}
		}
		protected void CheckSingleItemModeExpandElementState(AccordionControlElementCollection elements, bool hasExpanded) {
			for(int i = 0; i < elements.Count; i++) {
				if(elements[i].Expanded && elements[i].Style == ElementStyle.Item) {
					if(hasExpanded) elements[i].Expanded = false;
					hasExpanded = true;
				}
				if(elements[i].Elements != null) CheckSingleItemModeExpandElementState(elements[i].Elements, hasExpanded);
			}
		}
		int lockLayout = 0;
		protected internal bool IsLockLayout { get { return lockLayout != 0; } }
		public virtual void BeginUpdate() {
			this.lockLayout++;
			this.update++;
		}
		public virtual void EndUpdate() {
			if(--this.lockLayout == 0) {
				LayoutChanged();
				CheckExpandElementState(Elements);
				ControlInfo.Helper.CheckControlsVisibility(Elements);
			}
			this.update--;
		}
		public void ForEachElement(Action<AccordionControlElement> handler) {
			AccordionControlHelper.ForEachElement(handler, Elements, false);
		}
		public void ForEachVisibleElement(Action<AccordionControlElement> handler) {
			AccordionControlHelper.ForEachElement(handler, Elements, true);
		}
		public List<AccordionControlElement> GetElements() {
			return AccordionControlHelper.GetElements(Elements, false);
		}
		public List<AccordionControlElement> GetVisibleElements() {
			return AccordionControlHelper.GetElements(Elements, true);
		}
		protected override void OnPaint(PaintEventArgs e) {
			if(IsLockLayout) return;
			base.OnPaint(e);
		}
		protected override void LayoutChanged() {
			if(IsLockLayout) return;
			base.LayoutChanged();
		}
		internal void ForceLayoutChanged() {
			LayoutChanged();
		}
		void ResetAppearance() { Appearance.Reset(); }
		bool ShouldSerializeAppearance() { return Appearance.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlAppearance"),
#endif
 Category(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new virtual AccordionControlAppearances Appearance {
			get { return appearance; }
		}
		internal void SetAppearance(AccordionControlAppearances appearance) {
			this.appearance = appearance;
		}
		public virtual void SaveToXml(string xmlFile) {
			SaveLayoutCore(new XmlXtraSerializer(), xmlFile);
		}
		public virtual void RestoreFromXml(string xmlFile) {
			RestoreLayoutCore(new XmlXtraSerializer(), xmlFile);
		}
		public virtual bool SaveToRegistry(string path) {
			return SaveLayoutCore(new RegistryXtraSerializer(), path);
		}
		public virtual void RestoreFromRegistry(string path) {
			RestoreLayoutCore(new RegistryXtraSerializer(), path);
		}
		public virtual void SaveToStream(System.IO.Stream stream) {
			SaveLayoutCore(new XmlXtraSerializer(), stream);
		}
		public virtual void RestoreFromStream(System.IO.Stream stream) {
			RestoreLayoutCore(new XmlXtraSerializer(), stream);
		}
		protected virtual bool SaveLayoutCore(XtraSerializer serializer, object path) {
			System.IO.Stream stream = path as System.IO.Stream;
			if(stream != null)
				return serializer.SerializeObject(this, stream, this.GetType().Name);
			else
				return serializer.SerializeObject(this, path.ToString(), this.GetType().Name);
		}
		List<AccordionControlElement> originalElements;
		protected virtual void RestoreLayoutCore(XtraSerializer serializer, object path) {
			BeginUpdate();
			try {
				this.originalElements = CreateOriginalElementList();
				Deserialize(serializer, path);
				ForceRefreshViewInfo();
				while(this.originalElements.Count > 0) {
					AccordionControlElement elem = this.originalElements[0];
					this.originalElements.Remove(elem);
					elem.Dispose();
				}
			}
			finally { EndUpdate(); }
		}
		protected List<AccordionControlElement> CreateOriginalElementList() {
			List<AccordionControlElement> list = new List<AccordionControlElement>();
			AccordionControlHelper.ForEachElement(elem => list.Add(elem), Elements, false);
			this.elements = null;
			return list;
		}
		protected void Deserialize(XtraSerializer serializer, object path) {
			System.IO.Stream stream = path as System.IO.Stream;
			if(stream != null)
				serializer.DeserializeObject(this, stream, this.GetType().Name);
			else
				serializer.DeserializeObject(this, path.ToString(), this.GetType().Name);
		}
		string IXtraSerializableLayout.LayoutVersion {
			get { return string.Empty; }
		}
		ContextItem IContextItemProvider.CreateContextItem(Type type) {
			ContextItem item = CreateContextItemCore(type);
			if(item == null) return null;
			item.Name = item.GetType().Name;
			return item;
		}
		ContextItem CreateContextItemCore(Type type) {
			if(type == typeof(ContextButton)) return new AccordionContextButton();
			if(type == typeof(RatingContextButton)) return new AccordionRatingContextButton();
			if(type == typeof(CheckContextButton)) return new AccordionCheckContextButton();
			if(type == typeof(TrackBarContextButton)) return new AccordionTrackBarContextButton();
			return null;
		}
		bool IChildControlsIgnoreMouseWheel.Ignore {
			get { return Focused; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override SuperToolTip SuperTip {
			get { return base.SuperTip; }
			set { base.SuperTip = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override string ToolTipTitle {
			get { return base.ToolTipTitle; }
			set { base.ToolTipTitle = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override ToolTipIconType ToolTipIconType {
			get { return base.ToolTipIconType; }
			set { base.ToolTipIconType = value; }
		}
		#region IFilteringUIProvider Members
		void IFilteringUIProvider.RetrieveFields(object dataSource, Type type) {
			EditorGenerator generator = new EditorGenerator(new AccordionControlLayoutCreator(this));
			generator.ControlCreator.DataSource = dataSource;
			generator.CreateLayout(type);
		}
		#endregion
	}
	public enum ElementStyle { Group, Item }
	[
	DesignTimeVisible(false), ToolboxItem(false),
	SmartTagSupport(typeof(AccordionControlGroupDesignTimeBoundsProvider), SmartTagSupportAttribute.SmartTagCreationMode.UseComponentDesigner),
	Designer("DevExpress.XtraBars.Navigation.Design.AccordionControlElementDesigner, " + AssemblyInfo.SRAssemblyBarsDesign, typeof(IDesigner))
	]
	public class AccordionControlElement : AccordionControlElementBase, IElementCollectionContainer, IXtraSerializableLayout, DevExpress.Utils.MVVM.ISupportCommandBinding {
		private static readonly object styleChanged = new object();
		AccordionControlElementAppearances appearance;
		public AccordionControlElement() : this(ElementStyle.Group) { }
		public AccordionControlElement(ElementStyle style)
			: base() {
			this.style = style;
			this.appearance = new AccordionControlElementAppearances(this);
			this.appearance.Changed += OnAppearanceChanged;
		}
		ElementStyle style;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlElementStyle"),
#endif
 Category(CategoryName.Behavior), XtraSerializableProperty, DefaultValue(ElementStyle.Group)]
		public ElementStyle Style {
			get {
				return style;
			}
			set {
				if(style == value) return;
				style = value;
				CheckContentContainerVisible();
				RefreshViewInfo();
				RaiseStyleChanged();
			}
		}
		bool? hasContentContainer;
		protected internal bool HasContentContainer {
			get {
				if(ContentContainer != null) return true;
				if(DesignMode || Style == ElementStyle.Group) return false;
				if(this.hasContentContainer == null) {
					HasContentContainerEventArgs args = new HasContentContainerEventArgs(this);
					this.hasContentContainer = AccordionControl.RaiseHasContentContainer(args);
				}
				return this.hasContentContainer == true;
			}
		}
		protected internal bool CanExpandElement {
			get {
				if(AccordionControl == null) return true;
				if(AccordionControl.ControlInfo.IsMinimized) return false;
				if(!AccordionControl.ControlInfo.PressedInfo.IsInElement || Style == ElementStyle.Item)
					return true;
				return AccordionControl.ExpandGroupOnHeaderClick || AccordionControl.ControlInfo.PressedInfo.HitTest == AccordionControlHitTest.Button;
			}
		}
		protected internal override void InvertExpanded() {
			if(AccordionControl != null) {
				AccordionControl.ControlInfo.ResetElementsToExpandCollapse();
			}
			InvertExpandedCore();
		}
		protected internal void InvertExpandedCore() {
			if(!GetEnabled()) return;
			if(AccordionControl == null || AccordionControl.IsUpdate) {
				SetExpanded(!Expanded);
				return;
			}
			if(AccordionControl.IsHandleCreated) {
				if(AccordionControl.AnimationType != AnimationType.None && !DesignMode) {
					InvertExpandedWithAnimation();
				}
				else UpdateExpanded();
				return;
			}
			InvertExpandedImmediately();
		}
		protected internal void CheckContentContainerVisible() {
			if(ContentContainer == null) return;
			ContentContainer.Visible = IsContentContainerVisible;
		}
		protected internal bool IsContentContainerVisible {
			get {
				if(AccordionControl != null && AccordionControl.ControlInfo.IsMinimized)
					return false;
				return Style != ElementStyle.Group && Expanded && IsAllParentExpanded && GetVisible();
			}
		}
		protected internal bool IsAllParentExpanded {
			get {
				if(OwnerElement == null) return true;
				return OwnerElement.Expanded && OwnerElement.IsAllParentExpanded;
			}
		}
		protected void InvertExpandedWithAnimation() {
			if(!AccordionControl.IsLockLayout)
				AccordionControl.ControlInfo.Scrollers.LockUpdateScrollBar();
			DrawingLocker.LockDrawing(AccordionControl.Handle);
			try {
				AccordionControl.ControlInfo.Helper.ClearHeaderControlImages();
				if(Style == ElementStyle.Group && Expanded) {
					AccordionControl.ControlInfo.Helper.CreateImageFormHeaderControl(this);
				}
				UpdateExpanded();
			}
			finally { DrawingLocker.UnlockDrawing(AccordionControl.Handle); }
		}
		protected void InvertExpandedImmediately() {
			if(AccordionControl != null && Style == ElementStyle.Item && !Expanded) {
				if(AccordionControl.GetExpandElementMode() == ExpandElementMode.Single || AccordionControl.GetExpandElementMode() == ExpandElementMode.SingleItem) {
					AccordionControlHelper.ForEachElement(c => { CollapseItem(c); }, AccordionControl.Elements, false);
				}
			}
			UpdateExpanded();
		}
		protected internal void UpdateExpanded() {
			ExpandCollapseElementList list = AccordionControl.ControlInfo.ElementsToExpandCollapse;
			UpdateElementsToExpandCollapse(list);
			if(list.Count == 0) return;
			if(list[0].Element != this) {
				list[0].Element.InvertExpandedCore();
				return;
			}
			BeginUpdate();
			try {
				SetExpanded(!Expanded);
			}
			finally { EndUpdate(); }
		}
		protected void UpdateElementsToExpandCollapse(ExpandCollapseElementList list) {
			bool contains = false;
			foreach(QueryExpandStateElement item in list) {
				if(item.Element.Equals(this)) {
					contains = true;
					break;
				}
			}
			if(!contains) {
				AccordionElementState newState = Expanded ? AccordionElementState.Collapsed : AccordionElementState.Expanded;
				list.Add(new QueryExpandStateElement(this, newState));
			}
			AccordionControl.RaiseExpandStateChanging(new ExpandStateChangingEventArgs(this, list));
		}
		protected void CollapseItem(AccordionControlElement elem) {
			if(elem.Style == ElementStyle.Group) return;
			if(elem != this && elem.Expanded) elem.InvertExpanded();
		}
		protected internal bool RaiseElementClick() {
			ElementClickEventArgs args = new ElementClickEventArgs(this);
			RaiseClick();
			if(AccordionControl != null)
				return AccordionControl.RaiseElementClick(args);
			return false;
		}
		AccordionControlElementCollection elements;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlElementElements"),
#endif
 Category(CategoryName.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(true, true, false, 999)]
		public AccordionControlElementCollection Elements {
			get {
				if(elements == null)
					elements = CreateElements();
				return elements;
			}
		}
		internal object XtraCreateElementsItem(XtraItemEventArgs e) {
			return AccordionControl.XtraCreateElementsItemCore(Elements);
		}
		internal object XtraFindElementsItem(XtraItemEventArgs e) {
			return AccordionControl.XtraFindElementsItemCore(e, Elements);
		}
		protected virtual AccordionControlElementCollection CreateElements() {
			return new AccordionControlElementCollection(this);
		}
		[Browsable(false)]
		public override AccordionControl AccordionControl {
			get {
				if(OwnerElements == null) return null;
				if(OwnerElements.Element != null) return OwnerElements.Element.AccordionControl;
				return OwnerElements.AccordionControl;
			}
		}
		[Browsable(false)]
		public bool IsVisible { get { return GetVisible() && IsAllParentExpanded; } }
		[Browsable(false)]
		public int Level {
			get {
				int level = 0;
				AccordionControlElement parent = OwnerElement;
				while(parent != null) {
					parent = parent.OwnerElement;
					level++;
				}
				return level;
			}
		}
		protected internal override void SetRealVisible(bool value, bool applyForChildren) {
			base.SetRealVisible(value, applyForChildren);
			if(Style == ElementStyle.Item) return;
			if(applyForChildren) {
				foreach(AccordionControlElement item in Elements) {
					item.SetRealVisible(value, applyForChildren);
				}
			}
		}
		protected override void OnVisibleChanged() {
			if(AccordionControl != null)
				AccordionControl.ControlInfo.Helper.CheckElementControlsVisibility(this);
			base.OnVisibleChanged();
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public AccordionControlElement OwnerElement { get { return OwnerElements == null ? null : OwnerElements.Element; } }
		protected internal override bool IsInAnimation {
			get {
				if(Style == ElementStyle.Item && OwnerElement != null && OwnerElement.IsInAnimation) return true;
				return base.IsInAnimation;
			}
			set {
				base.IsInAnimation = value;
			}
		}
		protected internal override void SetExpanded(bool value) {
			if(AccordionControl != null && Style == ElementStyle.Item && value) {
				if(!HasContentContainer) {
					AccordionControl.ControlInfo.Scrollers.UnlockUpdateScrollBar();
					return;
				}
				if(ContentContainer == null) {
					AccordionControl.RaiseGetContentContainer(new GetContentContainerEventArgs(this));
				}
				if(ContentContainer == null) {
					AccordionControl.ControlInfo.Scrollers.UnlockUpdateScrollBar();
					return;
				}
			}
			base.SetExpanded(value);
		}
		protected internal override void CheckHeaderControlVisible() {
			if(AccordionControl != null && AccordionControl.ControlInfo.IsMinimized && HeaderControl != null) {
				HeaderControl.Visible = false;
				return;
			}
			if(Style == ElementStyle.Group) {
				base.CheckHeaderControlVisible();
				return;
			}
			if(HeaderControl == null) return;
			if(OwnerElement == null) HeaderControl.Visible = HeaderVisible && GetVisible();
			else HeaderControl.Visible = IsAllParentExpanded && HeaderVisible && GetVisible();
		}
		public void OnElementCollectionChanged() {
			if(AccordionControl != null) {
				AccordionControl.OnElementCollectionChanged();
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				UnsubscribeOnHeaderControlEvents();
				if(HeaderControl != null) HeaderControl.Dispose();
				if(ContentContainer != null) ContentContainer.Dispose();
				if(Appearance != null) Appearance.Changed -= OnAppearanceChanged;
				if(OwnerElements != null) OwnerElements.Remove(this);
				if(Elements != null) {
					while(Elements.Count > 0) {
						Elements[0].Dispose();
					}
				}
			}
			base.Dispose(disposing);
		}
		void ResetAppearance() { Appearance.Reset(); }
		bool ShouldSerializeAppearance() { return Appearance.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlElementAppearance"),
#endif
 Category(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AccordionControlElementAppearances Appearance {
			get { return appearance; }
		}
		void OnAppearanceChanged(object sender, EventArgs e) {
			if(AccordionControl != null)
				AccordionControl.Refresh();
		}
		string IXtraSerializableLayout.LayoutVersion {
			get { return string.Empty; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlElementStyleChanged"),
#endif
 Category(CategoryName.Events)]
		public event EventHandler StyleChanged {
			add { Events.AddHandler(styleChanged, value); }
			remove { Events.RemoveHandler(styleChanged, value); }
		}
		protected void RaiseStyleChanged() {
			EventHandler handler = Events[styleChanged] as EventHandler;
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		#region Commands
		public IDisposable BindCommand(object command, Func<object> queryCommandParameter = null) {
			return DevExpress.Utils.MVVM.CommandHelper.Bind(this,
				(element, execute) => element.Click += (s, e) => execute(),
				(element, canExecute) => element.Enabled = canExecute(),
				command, queryCommandParameter);
		}
		public IDisposable BindCommand(System.Linq.Expressions.Expression<Action> commandSelector, object source, Func<object> queryCommandParameter = null) {
			return DevExpress.Utils.MVVM.CommandHelper.Bind(this,
				(element, execute) => element.Click += (s, e) => execute(),
				(element, canExecute) => element.Enabled = canExecute(),
				commandSelector, source, queryCommandParameter);
		}
		public IDisposable BindCommand<T>(System.Linq.Expressions.Expression<Action<T>> commandSelector, object source, Func<T> queryCommandParameter = null) {
			return DevExpress.Utils.MVVM.CommandHelper.Bind(this,
				(element, execute) => element.Click += (s, e) => execute(),
				(element, canExecute) => element.Enabled = canExecute(),
				commandSelector, source, () => (queryCommandParameter != null) ? queryCommandParameter() : default(T));
		}
		#endregion Commands
		internal AccordionControlElement Clone() {
			AccordionControlElement elem = new AccordionControlElement();
			elem.appearance = Appearance.Clone();
			elem.Style = Style;
			elem.Image = Image;
			elem.ImageIndex = ImageIndex;
			elem.ImageLayoutMode = ImageLayoutMode;
			elem.AllowGlyphSkinning = AllowGlyphSkinning;
			elem.Enabled = Enabled;
			elem.HeaderVisible = HeaderVisible;
			elem.Height = Height;
			elem.Hint = Hint;
			elem.ImageUri = ImageUri;
			elem.Text = Text;
			elem.SuperTip = SuperTip;
			elem.TextPosition = TextPosition;
			elem.TextToImageDistance = TextToImageDistance;
			elem.Visible = elem.Visible;
			elem.TagInternal = this;
			return elem;
		}
	}
	public abstract class AccordionControlElementBase : Component, IDXImageUriClient {
		private static readonly object expandedChanged = new object();
		private static readonly object visibleChanged = new object();
		private static readonly object enabledChanged = new object();
		private static readonly object dragDrop = new object();
		private static readonly object click = new object();
		string hint, name;
		DefaultBoolean allowGlyphSkinning;
		DxImageUri imageUri;
		public abstract AccordionControl AccordionControl { get; }
		public AccordionControlElementBase() {
			this.realVisible = true;
			this.hint = string.Empty;
			this.height = 0;
			this.allowGlyphSkinning = DefaultBoolean.Default;
			Bounds = Rectangle.Empty;
			this.imageUri = CreateImageUriInstance();
			this.imageUri.Changed += ImageUriChanged;
			this.name = string.Empty;
		}
		bool headerVisible = true;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlElementBaseHeaderVisible"),
#endif
 Category(CategoryName.Appearance), DefaultValue(true), XtraSerializableProperty]
		public bool HeaderVisible {
			get { return headerVisible; }
			set {
				headerVisible = value;
				CheckHeaderControlVisible();
				OnPropertiesChanged();
			}
		}
		protected internal Rectangle Bounds { get; set; }
		AccordionControlElementCollection ownerElements;
		protected internal AccordionControlElementCollection OwnerElements {
			get {
				return ownerElements;
			}
			set {
				if(ownerElements == value)
					return;
				if(ownerElements != null && value != null)
					ownerElements.Remove(this as AccordionControlElement);
				ownerElements = value;
				CheckHeaderControlVisible();
			}
		}
		Control headerControl;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlElementBaseHeaderControl"),
#endif
 Category(CategoryName.Appearance), DefaultValue(null)]
		public Control HeaderControl {
			get { return headerControl; }
			set {
				if(HeaderControl == value) return;
				UnsubscribeOnHeaderControlEvents();
				RemoveControlFromAccordionControls(HeaderControl);
				headerControl = value;
				SubscribeOnHeaderControlEvents();
				if(HeaderControl != null) {
					if(AccordionControl != null)
						AccordionControl.Controls.Add(HeaderControl);
					CheckHeaderControlVisible();
					CheckHeaderControlEnabled();
				}
				RefreshViewInfo();
				OnPropertiesChanged();
			}
		}
		protected void RemoveControlFromAccordionControls(Control ctrl) {
			if(ctrl != null && ctrl.Parent == AccordionControl) {
				ctrl.Parent = null;
				ctrl.Dispose();
			}
		}
		protected virtual void SubscribeOnHeaderControlEvents() {
			if(HeaderControl != null) {
				HeaderControl.MouseLeave += HeaderControl_MouseLeave;
			}
		}
		void HeaderControl_MouseLeave(object sender, EventArgs e) {
			AccordionControl.ControlInfo.ResetHoverInfo();
		}
		protected virtual void UnsubscribeOnHeaderControlEvents() {
			if(HeaderControl != null) {
				HeaderControl.MouseLeave -= HeaderControl_MouseLeave;
			}
		}
		protected void CheckHeaderControlEnabled() {
			if(HeaderControl == null) return;
			HeaderControl.Enabled = GetEnabled();
		}
		protected internal int InnerContentHeight { get; set; }
		AccordionContentContainer contentContainer;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlElementBaseContentContainer"),
#endif
 Category(CategoryName.Appearance), DefaultValue(null)]
		public AccordionContentContainer ContentContainer {
			get { return contentContainer; }
			set {
				if(ContentContainer == value) return;
				RemoveControlFromAccordionControls(ContentContainer);
				contentContainer = value;
				ClearPaintAppearance();
				if(contentContainer != null) {
					if(AccordionControl != null) {
						contentContainer.Width = AccordionControl.Width;
						AccordionControl.Controls.Add(contentContainer);
					}
					contentContainer.SetOwnerElement(this);
					contentContainer.Visible = Expanded;
				}
				OnPropertiesChanged();
			}
		}
		protected internal void ResetContentContainer() {
			this.contentContainer = null;
			ClearPaintAppearance();
		}
		protected void ClearPaintAppearance() {
			if(AccordionControl != null)
				AccordionControl.ControlInfo.ClearPaintAppearance();
		}
		Image contentContainerImage = null;
		protected internal Image ContentContainerImage {
			get { return contentContainerImage; }
			set { contentContainerImage = value; }
		}
		protected internal void CreateImageFromContentContainer() {
			if(ContentContainer == null) return;
			ContentContainerImage = AccordionControlPainter.GetControlImage(ContentContainer);
		}
		protected internal virtual void CheckHeaderControlVisible() {
			if(HeaderControl == null) return;
			HeaderControl.Visible = HeaderVisible && GetVisible();
		}
		Image headerControlImage = null;
		protected internal Image HeaderControlImage {
			get { return headerControlImage; }
			set { headerControlImage = value; }
		}
		bool expanded = false;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlElementBaseExpanded"),
#endif
 DefaultValue(false), Category(CategoryName.Behavior), XtraSerializableProperty]
		public bool Expanded {
			get {
				if(!HeaderVisible) return GetVisible();
				return expanded;
			}
			set {
				if(expanded == value) return;
				InvertExpanded();
			}
		}
		protected internal virtual void SetExpanded(bool value) {
			this.expanded = value;
			if(!isInUpdate && AccordionControl != null && !AccordionControl.IsUpdate)
				AccordionControl.ControlInfo.ShowVisibleControls();
			OnExpandedChanged();
		}
		protected internal virtual void InvertExpanded() {
			SetExpanded(!Expanded);
		}
		SuperToolTip superTip;
		internal bool ShouldSerializeSuperTip() { return SuperTip != null && !SuperTip.IsEmpty; }
		public void ResetSuperTip() { SuperTip = null; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlElementBaseSuperTip"),
#endif
 Localizable(true), Category(CategoryName.ToolTip),
		Editor("DevExpress.XtraEditors.Design.ToolTipContainerUITypeEditor, " + AssemblyInfo.SRAssemblyEditorsDesign, typeof(System.Drawing.Design.UITypeEditor))]
		public virtual SuperToolTip SuperTip {
			get { return superTip; }
			set { superTip = value; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlElementBaseHint"),
#endif
 DefaultValue(""), Category(CategoryName.Appearance), Localizable(true)]
		public virtual string Hint {
			get { return hint; }
			set {
				if(value == null) value = string.Empty;
				hint = value;
			}
		}
		bool isInAnimation = false;
		protected internal virtual bool IsInAnimation {
			get { return isInAnimation; }
			set { isInAnimation = value; }
		}
		bool isInUpdate = false;
		protected internal void BeginUpdate() {
			this.isInUpdate = true;
		}
		protected internal void EndUpdate() {
			this.isInUpdate = false;
		}
		protected internal event EventHandler ExpandedChanged {
			add { Events.AddHandler(expandedChanged, value); }
			remove { Events.RemoveHandler(expandedChanged, value); }
		}
		protected internal bool LockAnimation { get; set; }
		void RaiseExpandedChanged() {
			EventHandler handler = Events[expandedChanged] as EventHandler;
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlElementBaseClick"),
#endif
 Category(CategoryName.Events)]
		public event EventHandler Click {
			add { Events.AddHandler(click, value); }
			remove { Events.RemoveHandler(click, value); }
		}
		protected void RaiseClick() {
			EventHandler handler = Events[click] as EventHandler;
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlElementBaseVisibleChanged"),
#endif
 Category(CategoryName.Events)]
		public event EventHandler VisibleChanged {
			add { Events.AddHandler(visibleChanged, value); }
			remove { Events.RemoveHandler(visibleChanged, value); }
		}
		void RaiseVisibleChanged() {
			EventHandler handler = Events[visibleChanged] as EventHandler;
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlElementBaseEnabledChanged"),
#endif
 Category(CategoryName.Events)]
		public event EventHandler EnabledChanged {
			add { Events.AddHandler(enabledChanged, value); }
			remove { Events.RemoveHandler(enabledChanged, value); }
		}
		void RaiseEnabledChanged() {
			EventHandler handler = Events[enabledChanged] as EventHandler;
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public event EventHandler DragDrop {
			add { Events.AddHandler(dragDrop, value); }
			remove { Events.RemoveHandler(dragDrop, value); }
		}
		protected internal void RaiseDragDrop() {
			EventHandler handler = Events[dragDrop] as EventHandler;
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		ImageLayoutMode imageLayoutMode = ImageLayoutMode.OriginalSize;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlElementBaseImageLayoutMode"),
#endif
 Category(CategoryName.Appearance), XtraSerializableProperty, DefaultValue(ImageLayoutMode.OriginalSize)]
		public ImageLayoutMode ImageLayoutMode {
			get { return imageLayoutMode; }
			set {
				if(ImageLayoutMode == value) return;
				imageLayoutMode = value;
				RefreshViewInfo();
				OnPropertiesChanged();
			}
		}
		TextPosition textPosition = TextPosition.AfterImage;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlElementBaseTextPosition"),
#endif
 Category(CategoryName.Appearance), XtraSerializableProperty, DefaultValue(TextPosition.AfterImage)]
		public TextPosition TextPosition {
			get { return textPosition; }
			set {
				if(TextPosition == value) return;
				textPosition = value;
				OnPropertiesChanged();
			}
		}
		int textToImageDistance = 5;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlElementBaseTextToImageDistance"),
#endif
 Category(CategoryName.Appearance), DefaultValue(5), XtraSerializableProperty]
		public int TextToImageDistance {
			get { return textToImageDistance; }
			set {
				if(TextToImageDistance == value) return;
				textToImageDistance = value;
				OnPropertiesChanged();
			}
		}
		int headerControlToContextButtonsDistance = 5;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlElementBaseHeaderControlToContextButtonsDistance"),
#endif
 Category(CategoryName.Appearance), DefaultValue(5), XtraSerializableProperty]
		public int HeaderControlToContextButtonsDistance {
			get { return headerControlToContextButtonsDistance; }
			set {
				if(HeaderControlToContextButtonsDistance == value) return;
				headerControlToContextButtonsDistance = value;
				OnPropertiesChanged();
			}
		}
		protected virtual void OnExpandedChanged() {
			if(ShouldRefreshBeforeAnimation())
				OnPropertiesChanged();
			RaiseExpandedChanged();
		}
		bool ShouldRefreshBeforeAnimation() {
			return AccordionControl == null || AccordionControl.IsDesignMode || AccordionControl.AnimationType != AnimationType.None;
		}
		bool visible = true;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlElementBaseVisible"),
#endif
 Category(CategoryName.Appearance), DefaultValue(true), XtraSerializableProperty]
		public bool Visible {
			get {
				if(OwnerElements != null && OwnerElements.Element != null && OwnerElements.Element.Style == ElementStyle.Item)
					return false;
				if(!RealVisible) return false;
				return visible;
			}
			set {
				if(Visible == value)
					return;
				visible = value;
				OnVisibleChanged();
			}
		}
		internal bool GetVisible() {
			if(AccordionControl != null && AccordionControl.IsDesignMode)
				return true;
			return Visible;
		}
		bool realVisible = true;
		protected internal bool RealVisible {
			get { return realVisible; }
		}
		protected internal virtual void SetRealVisible(bool value, bool applyForChildren) {
			this.realVisible = value;
		}
		protected virtual void OnVisibleChanged() {
			RaiseVisibleChanged();
			OnPropertiesChanged();
		}
		bool enabled = true;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlElementBaseEnabled"),
#endif
 Category(CategoryName.Appearance), DefaultValue(true)]
		public bool Enabled {
			get { return enabled; }
			set {
				if(Enabled == value)
					return;
				enabled = value;
				CheckHeaderControlEnabled();
				OnEnabledChanged();
			}
		}
		internal bool GetEnabled() {
			if(AccordionControl != null && AccordionControl.IsDesignMode)
				return true;
			return Enabled;
		}
		private void OnEnabledChanged() {
			RaiseEnabledChanged();
			RefreshViewInfo();
			OnPropertiesChanged();
		}
		string text;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlElementBaseText"),
#endif
 Category(CategoryName.Appearance), DefaultValue(null), XtraSerializableProperty, Localizable(true)]
		public string Text {
			get { return text; }
			set {
				if(Text == value)
					return;
				text = value;
				RefreshViewInfo();
				OnPropertiesChanged();
			}
		}
		[Browsable(false), XtraSerializableProperty]
		public string Name {
			get {
				if(Site != null) return Site.Name;
				return name;
			}
			set {
				if(value == null) return;
				name = value;
			}
		}
		Image image;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlElementBaseImage"),
#endif
 Category(CategoryName.Appearance), DefaultValue(null), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public Image Image {
			get { return image; }
			set {
				if(Image == value)
					return;
				image = value;
				RefreshViewInfo();
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlElementBaseAllowGlyphSkinning"),
#endif
 DefaultValue(DefaultBoolean.Default), Category("Appearance")]
		public virtual DefaultBoolean AllowGlyphSkinning {
			get { return allowGlyphSkinning; }
			set {
				if(AllowGlyphSkinning == value)
					return;
				allowGlyphSkinning = value;
				RefreshViewInfo();
				OnPropertiesChanged();
			}
		}
		protected internal virtual bool ShouldSerializeImageUri() {
			return ImageUri.ShouldSerialize();
		}
		protected internal virtual void ResetImageUri() {
			ImageUri.Reset();
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlElementBaseImageUri"),
#endif
 Category("Appearance"), TypeConverter(typeof(ExpandableObjectConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual DxImageUri ImageUri {
			get { return imageUri; }
			set {
				if(value == null || ImageUri.Equals(value)) return;
				DxImageUri prev = ImageUri;
				this.imageUri = value;
				OnImageUriChanged(prev, value);
			}
		}
		private void OnImageUriChanged(DxImageUri prev, DxImageUri next) {
			if(prev != null) {
				prev.Changed -= ImageUriChanged;
			}
			if(next != null) {
				next.Changed += ImageUriChanged;
				next.SetClient(this);
			}
			RefreshViewInfo();
			OnPropertiesChanged();
		}
		protected void ImageUriChanged(object sender, EventArgs e) {
			RefreshViewInfo();
			OnPropertiesChanged();
		}
		protected virtual DxImageUri CreateImageUriInstance() {
			return new DxImageUri();
		}
		protected internal bool GetAllowGlyphSkinning() {
			if(AllowGlyphSkinning == DefaultBoolean.True) return true;
			if(AllowGlyphSkinning == DefaultBoolean.False) return false;
			return AccordionControl != null && AccordionControl.AllowGlyphSkinning == DefaultBoolean.True;
		}
		protected void RefreshViewInfo() {
			if(AccordionControl == null) return;
			AccordionControl.ForceRefreshViewInfo();
		}
		protected internal Image GetOriginalImage() {
			if(ImageUri != null) {
				if(ImageUri.HasLargeImage) return ImageUri.GetLargeImage();
				if(ImageUri.HasImage) return ImageUri.GetImage();
			}
			if(Image != null) return Image;
			if(Images != null)
				return ImageCollection.GetImageListImage(Images, ImageIndex);
			return null;
		}
		[Browsable(false)]
		public object Images {
			get {
				if(AccordionControl == null) return null;
				return AccordionControl.Images;
			}
		}
		int imageIndex = -1;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlElementBaseImageIndex"),
#endif
 Category(CategoryName.Appearance), DefaultValue(-1),
		Editor(typeof(ImageIndexesEditor), typeof(UITypeEditor)), ImageList("Images")]
		public int ImageIndex {
			get { return imageIndex; }
			set {
				if(ImageIndex == value)
					return;
				imageIndex = value;
				RefreshViewInfo();
				OnPropertiesChanged();
			}
		}
		int height;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlElementBaseHeight"),
#endif
 Category(CategoryName.Appearance), XtraSerializableProperty, DefaultValue(0)]
		public int Height {
			get {
				return height;
			}
			set {
				if(Height == value)
					return;
				height = value;
				RefreshViewInfo();
				OnPropertiesChanged();
			}
		}
		protected internal bool IsHeightUpdated { get { return Height != 0; } }
		protected void OnPropertiesChanged() {
			if(AccordionControl != null && !AccordionControl.IsUpdate)
				AccordionControl.Refresh();
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AccordionControlElementBaseTag"),
#endif
 Category(CategoryName.Data), DefaultValue(null),
		Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))]
		public object Tag { get; set; }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public object TagInternal { get; set; }
		bool IDXImageUriClient.IsDesignMode {
			get { return DesignMode; }
		}
		UserLookAndFeel IDXImageUriClient.LookAndFeel {
			get {
				if(AccordionControl == null) return null;
				return AccordionControl.LookAndFeel;
			}
		}
		void IDXImageUriClient.SetGlyphSkinningValue(bool value) {
			AllowGlyphSkinning = (value ? DefaultBoolean.True : DefaultBoolean.False);
		}
		bool IDXImageUriClient.SupportsGlyphSkinning {
			get { return true; }
		}
		bool IDXImageUriClient.SupportsLookAndFeel {
			get { return true; }
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(this.imageUri != null) {
					this.imageUri.Changed -= ImageUriChanged;
					this.imageUri.Dispose();
				}
				this.imageUri = null;
			}
			base.Dispose(disposing);
		}
	}
	public delegate void GetContentContainerEventHandler(object sender, GetContentContainerEventArgs e);
	public class GetContentContainerEventArgs : EventArgs {
		public GetContentContainerEventArgs(AccordionControlElement element) {
			this.element = element;
		}
		AccordionControlElement element;
		public AccordionControlElement Element { get { return element; } }
		public AccordionContentContainer ContentContainer {
			get { return element.ContentContainer; }
			set { element.ContentContainer = value; }
		}
	}
	public delegate void HasContentContainerEventHandler(object sender, HasContentContainerEventArgs e);
	public class HasContentContainerEventArgs : EventArgs {
		public HasContentContainerEventArgs(AccordionControlElement element) {
			this.element = element;
			this.hasContentContainer = false;
		}
		AccordionControlElement element;
		public AccordionControlElement Element { get { return element; } }
		bool hasContentContainer;
		public bool HasContentContainer {
			get { return hasContentContainer; }
			set { hasContentContainer = value; }
		}
	}
	public class AccordionControlElementEventArgs : EventArgs {
		public AccordionControlElementEventArgs(AccordionControlElement element) {
			this.element = element;
		}
		AccordionControlElement element;
		public AccordionControlElement Element { get { return element; } }
	}
	public delegate void ElementClickEventHandler(object sender, ElementClickEventArgs e);
	public class ElementClickEventArgs : AccordionControlElementEventArgs {
		bool handled;
		public ElementClickEventArgs(AccordionControlElement element)
			: base(element) {
			this.handled = false;
		}
		public bool Handled {
			get { return handled; }
			set { handled = value; }
		}
	}
	public delegate void SelectedElementChangedEventHandler(object sender, SelectedElementChangedEventArgs e);
	public class SelectedElementChangedEventArgs : AccordionControlElementEventArgs {
		public SelectedElementChangedEventArgs(AccordionControlElement element)
			: base(element) {
		}
	}
	public delegate void ItemContentContainerHiddenEventHandler(object sender, ItemContentContainerHiddenEventArgs e);
	public class ItemContentContainerHiddenEventArgs : AccordionControlElementEventArgs {
		public ItemContentContainerHiddenEventArgs(AccordionControlElement element)
			: base(element) {
		}
	}
	public delegate void FilterContentEventHandler(object sender, FilterContentEventArgs e);
	public class FilterContentEventArgs : EventArgs {
		AccordionControlElement element;
		object filterValue;
		public FilterContentEventArgs(AccordionControlElement element, object filterValue) {
			this.element = element;
			Visible = true;
			Handled = false;
			this.filterValue = filterValue;
		}
		public AccordionControlElement Element { get { return element; } }
		public bool Visible { get; set; }
		public bool Handled { get; set; }
		public object FilterValue { get { return filterValue; } }
	}
	public interface IElementCollectionContainer {
		void OnElementCollectionChanged();
	}
	[Editor("DevExpress.XtraBars.Navigation.Design.AccordionControlElementCollectionEditor, " + AssemblyInfo.SRAssemblyBarsDesign, typeof(System.Drawing.Design.UITypeEditor))]
	public class AccordionControlElementCollection : Collection<AccordionControlElement> {
		public AccordionControlElementCollection() { }
		public AccordionControlElementCollection(IElementCollectionContainer container) {
			SetContainer(container);
		}
		public IElementCollectionContainer Container { get; private set; }
		public AccordionControl AccordionControl { get { return Container as AccordionControl; } }
		public AccordionControlElement Element { get { return Container as AccordionControlElement; } }
		public virtual void AddRange(AccordionControlElement[] elements) {
			foreach(AccordionControlElement element in elements) Add(element);
		}
		public virtual AccordionControlElement Add() {
			AccordionControlElement elem = new AccordionControlElement();
			Add(elem);
			return elem;
		}
		public AccordionControlElement this[string name] {
			get {
				foreach(AccordionControlElement item in Items) {
					if(item.Site != null && item.Site.Name == name) return item;
				}
				return null;
			}
		}
		AccordionControl GetAccordionControl() {
			if(AccordionControl != null)
				return AccordionControl;
			if(Element != null)
				return Element.AccordionControl;
			return null;
		}
		void AddElementControls(AccordionControlElement elem) {
			AccordionControl accordion = GetAccordionControl();
			if(accordion == null || accordion.IsDesignMode)
				return;
			if(elem.ContentContainer != null && !accordion.Controls.Contains(elem.ContentContainer))
				accordion.Controls.Add(elem.ContentContainer);
			if(elem.HeaderControl != null && !accordion.Controls.Contains(elem.HeaderControl))
				accordion.Controls.Add(elem.HeaderControl);
			foreach(AccordionControlElement child in elem.Elements) {
				AddElementControls(child);
			}
		}
		void RemoveElementControls(AccordionControlElement elem) {
			AccordionControl accordion = GetAccordionControl();
			if(accordion == null)
				return;
			if(elem.ContentContainer != null && accordion.Controls.Contains(elem.ContentContainer))
				accordion.Controls.Remove(elem.ContentContainer);
			if(elem.HeaderControl != null && accordion.Controls.Contains(elem.HeaderControl))
				accordion.Controls.Remove(elem.HeaderControl);
			foreach(AccordionControlElement child in elem.Elements) {
				RemoveElementControls(child);
			}
		}
		protected override void InsertItem(int index, AccordionControlElement item) {
			base.InsertItem(index, item);
			item.OwnerElements = this;
			AddElementControls(item);
			CheckExpandedState(item);
			if(Container != null)
				Container.OnElementCollectionChanged();
		}
		protected override void RemoveItem(int index) {
			AccordionControlElement item = this[index];
			if(item != null)
				item.OwnerElements = null;
			base.RemoveItem(index);
			RemoveElementControls(item);
			if(Container != null)
				Container.OnElementCollectionChanged();
		}
		protected override void SetItem(int index, AccordionControlElement item) {
			AccordionControlElement group = this[index];
			if(group != null)
				group.OwnerElements = null;
			item.OwnerElements = this;
			base.SetItem(index, item);
			CheckExpandedState(item);
			if(Container != null)
				Container.OnElementCollectionChanged();
		}
		public override string ToString() {
			return string.Empty;
		}
		public void CheckExpandedState(AccordionControlElement element) {
			if(AccordionControl == null || AccordionControl.GetExpandElementMode() == ExpandElementMode.Multiple || !element.Expanded) return;
			foreach(AccordionControlElement item in this.Items) {
				if(item.Expanded && !item.Equals(element)) {
					element.Expanded = false;
					return;
				}
			}
		}
		internal void SetContainer(IElementCollectionContainer container) {
			Container = container;
		}
		public AccordionControlElementCollection Clone() {
			AccordionControlElementCollection col = new AccordionControlElementCollection();
			for(int i = 0; i < Count; i++) {
				AccordionControlElement elem = this[i].Clone();
				AccordionControlElementCollection childElements = this[i].Elements.Clone();
				col.Add(elem);
				while(childElements.Count > 0) {
					elem.Elements.Add(childElements[0]);
				}
			}
			return col;
		}
	}
	public enum AccordionControlHitTest { None, Group, Item, ContextButtons, ScrollBar, Button, ExpandButton }
	public class AccordionControlHitInfo {
		static AccordionControlHitInfo empty;
		public static AccordionControlHitInfo Empty {
			get {
				if(empty == null)
					empty = new AccordionControlHitInfo();
				return empty;
			}
		}
		public Point HitPoint { get; internal set; }
		public AccordionControlHitTest HitTest { get; internal set; }
		public AccordionElementBaseViewInfo ItemInfo { get; internal set; }
		public bool IsInElement {
			get {
				return HitTest == AccordionControlHitTest.Item || HitTest == AccordionControlHitTest.Group || HitTest == AccordionControlHitTest.Button;
			}
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			AccordionControlHitInfo info = obj as AccordionControlHitInfo;
			if(info == null)
				return false;
			return info.HitTest == HitTest && info.ItemInfo == ItemInfo;
		}
	}
	public class ExpandCollapseElementList : List<QueryExpandStateElement> {
		public void Add(AccordionControlElement element, AccordionElementState newState) {
			if(element == null) return;
			Add(new QueryExpandStateElement(element, newState));
		}
		public void Insert(int index, AccordionControlElement element, AccordionElementState newState) {
			if(element == null) return;
			Insert(index, new QueryExpandStateElement(element, newState));
		}
	}
	public class QueryExpandStateElement {
		public QueryExpandStateElement(AccordionControlElement element, AccordionElementState newState) {
			this.element = element;
			this.newState = newState;
		}
		AccordionControlElement element;
		public AccordionControlElement Element {
			get { return element; }
		}
		AccordionElementState newState;
		public AccordionElementState NewState {
			get { return newState; }
		}
	}
	public delegate void AccordionControlContextButtonCustomizeEventHandler(object sender, AccordionControlContextButtonCustomizeEventArgs e);
	public class AccordionControlContextButtonCustomizeEventArgs : EventArgs {
		public AccordionControlContextButtonCustomizeEventArgs(AccordionControlElement element, ContextItem contextItem) {
			this.element = element;
			ContextItem = contextItem;
		}
		AccordionControlElement element;
		public ContextItem ContextItem { get; private set; }
		public AccordionControlElement Element { get { return element; } }
	}
	public delegate void AccordionControlContextButtonToolTipEventHandler(object sender, AccordionControlContextButtonToolTipEventArgs e);
	public class AccordionControlContextButtonToolTipEventArgs : EventArgs {
		ContextButtonToolTipEventArgs contextToolTipArgs;
		public AccordionControlContextButtonToolTipEventArgs(AccordionControlElement element, ContextButtonToolTipEventArgs contextToolTipArgs) {
			this.element = element;
			this.contextToolTipArgs = contextToolTipArgs;
		}
		AccordionControlElement element;
		public AccordionControlElement Element { get { return element; } }
		public ContextItem ContextItem { get { return contextToolTipArgs.Item; } }
		public object Value { get { return contextToolTipArgs.Value; } }
		public string Text {
			get { return contextToolTipArgs.Text; }
			set { contextToolTipArgs.Text = value; }
		}
	}
	public class CustomDrawElementEventArgs {
		public CustomDrawElementEventArgs(GraphicsCache cache, AccordionElementBaseViewInfo elementInfo, AccordionControlPainter painter) {
			this.cache = cache;
			this.elementInfo = elementInfo;
			this.painter = painter;
		}
		bool handled;
		AccordionElementBaseViewInfo elementInfo;
		GraphicsCache cache;
		AccordionControlPainter painter;
		public AccordionElementBaseViewInfo ObjectInfo {
			get { return elementInfo; }
		}
		public virtual AppearanceObject Appearance {
			get { return elementInfo.PaintAppearance; }
		}
		public virtual bool Handled {
			get { return handled; }
			set { handled = value; }
		}
		public virtual Graphics Graphics {
			get { return cache.Graphics; }
		}
		public virtual GraphicsCache Cache {
			get { return cache; }
		}
		public void DrawHeaderBackground() {
			this.painter.DrawHeaderBackground(Cache, ObjectInfo);
		}
		public void DrawExpandCollapseButton() {
			this.painter.DrawExpandCollapseButton(Cache, ObjectInfo);
		}
		public void DrawImage() {
			this.painter.DrawImage(Cache, ObjectInfo);
		}
		public void DrawContextButtons() {
			this.painter.DrawContextButtons(Cache, ObjectInfo);
		}
		public void DrawText() {
			this.painter.DrawText(Cache, ObjectInfo);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void DrawElementSelection() { }
	}
	public delegate void CustomDrawElementEventHandler(object sender, CustomDrawElementEventArgs e);
	public class CustomElementTextEventArgs {
		public CustomElementTextEventArgs(string text, AccordionElementBaseViewInfo elementInfo) {
			this.text = text;
			this.elementInfo = elementInfo;
		}
		AccordionElementBaseViewInfo elementInfo;
		string text;
		public string Text {
			get { return text; }
			set { text = value; }
		}
		public AccordionElementBaseViewInfo ObjectInfo {
			get { return elementInfo; }
		}
	}
	public delegate void CustomElementTextEventHandler(object sender, CustomElementTextEventArgs e);
	public class ExpandStateChangingEventArgs {
		public ExpandStateChangingEventArgs(AccordionControlElement element, ExpandCollapseElementList elementsToExpandCollapse) {
			this.element = element;
			this.elementsToExpandCollapse = elementsToExpandCollapse;
		}
		AccordionControlElement element;
		ExpandCollapseElementList elementsToExpandCollapse;
		public AccordionControlElement Element {
			get { return element; }
		}
		public AccordionElementState NewState {
			get { return Element.Expanded ? AccordionElementState.Collapsed : AccordionElementState.Expanded; }
		}
		public ExpandCollapseElementList ElementsToExpandCollapse {
			get { return elementsToExpandCollapse; }
		}
	}
	public delegate void ExpandStateChangingEventHandler(object sender, ExpandStateChangingEventArgs e);
	public delegate void ExpandStateChangedEventHandler(object sender, ExpandStateChangedEventArgs e);
	public class ExpandStateChangedEventArgs : AccordionControlElementEventArgs {
		public ExpandStateChangedEventArgs(AccordionControlElement element)
			: base(element) {
		}
	}
	public class AccordionElementDragOverEventArgs : EventArgs {
		AccordionControlElement element;
		bool canDrop;
		object targetOwner;
		public AccordionElementDragOverEventArgs(AccordionControlElement element, object targetOwner, bool canDrop) {
			this.element = element;
			this.canDrop = canDrop;
			this.targetOwner = targetOwner;
		}
		public AccordionControlElement Element { get { return element; } }
		public object TargetOwner { get { return targetOwner; } }
		public bool CanDrop {
			get { return canDrop; }
			set { canDrop = value; }
		}
	}
	public delegate void AccordionElementDragOverEventHandler(object sender, AccordionElementDragOverEventArgs e);
	public class StartAccordionElementDraggingEventArgs : EventArgs {
		AccordionControlElement element;
		Image dragImage;
		bool cancel;
		public StartAccordionElementDraggingEventArgs(AccordionControlElement element) {
			this.dragImage = null;
			this.cancel = false;
			this.element = element;
		}
		public AccordionControlElement Element { get { return element; } }
		public Image DragImage {
			get { return dragImage; }
			set { dragImage = value; }
		}
		public bool Cancel {
			get { return cancel; }
			set { cancel = value; }
		}
	}
	public delegate void StartAccordionElementDraggingEventHandler(object sender, StartAccordionElementDraggingEventArgs e);
	public class EndAccordionElementDraggingEventArgs : EventArgs {
		AccordionControlElement element;
		object targetOwner;
		int insertIndex;
		bool cancel;
		public EndAccordionElementDraggingEventArgs(AccordionControlElement element, object targetOwner, int insertIndex) {
			this.cancel = false;
			this.element = element;
			this.insertIndex = insertIndex;
			this.targetOwner = targetOwner;
		}
		public AccordionControlElement Element { get { return element; } }
		public object TargetOwner { get { return targetOwner; } }
		public int InsertIndex { get { return insertIndex; } }
		public bool Cancel {
			get { return cancel; }
			set { cancel = value; }
		}
	}
	public delegate void EndAccordionElementDraggingEventHandler(object sender, EndAccordionElementDraggingEventArgs e);
}
