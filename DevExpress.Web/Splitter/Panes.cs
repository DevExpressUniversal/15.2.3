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
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Design;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public class SplitterPane : ContentControlCollectionItem, IEnumerable, IHierarchyData {
		ASPxSplitter splitter;
		SplitterPaneCollection panes;
		SplitterSeparators separators;
		SplitterSeparators separator;
		SplitterPaneStyle style;
		SplitterPaneCollapsedStyle collapsedStyle;
		Unit designModeSize;
		public SplitterPane()
			: base() {
			this.panes = CreatePanesCollection();
			this.separators = new SplitterSeparators(this, false);
			this.separator = new SplitterSeparators(this, true);
			this.style = new SplitterPaneStyle();
			this.collapsedStyle = new SplitterPaneCollapsedStyle();
		}
		public SplitterPane(string name)
			: this() {
			this.Name = name;
		}
		protected internal SplitterPane(ASPxSplitter splitter)
			: this() {
			this.splitter = splitter;
		}
		protected internal ASPxSplitter Splitter {
			get {
				if(this.splitter != null)
					return this.splitter;
				if(Parent != null)
					return Parent.Splitter;
				return null;
			}
		}
		protected internal SplitterPaneCollection ParentCollection {
			get { return (SplitterPaneCollection)Collection; }
		}
		protected internal SplitterPane Parent {
			get { return (ParentCollection != null) ? ParentCollection.Owner : null; }
		}
		protected internal Orientation Orientation {
			get {
				if(Parent == null)
					return Splitter.ReverseOrientation(Splitter.Orientation);
				return ParentCollection.Orientation;
			}
		}
		protected internal bool IsVertical {
			get { return Orientation == Orientation.Vertical; }
		}
		protected internal Unit DesignModeSize {
			get { return designModeSize; }
			set { designModeSize = value; }
		}
		protected internal bool HasChildren {
			get { return Panes.Count > 0; }
		}
		protected internal bool HasVisibleChildren {
			get { return Panes.GetVisibleItemCount() > 0; }
		}
		protected internal ContentControl ContentControlInternal {
			get { return ContentControl; }
		}
		protected internal bool HasContentUrl {
			get { return !HasVisibleChildren && !string.IsNullOrEmpty(ContentUrl); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterPaneSeparators"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SplitterSeparators Separators {
			get { return separators; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterPaneSeparator"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SplitterSeparators Separator {
			get { return separator; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterPaneName"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(false)]
		public string Name {
			get { return GetStringProperty("Name", ""); }
			set { SetStringProperty("Name", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterPanePanes"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SplitterPaneCollection Panes {
			get { return panes; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterPaneVisible"),
#endif
		NotifyParentProperty(true), DefaultValue(true)]
		public bool Visible {
			get { return GetVisible(); }
			set {
				if (Visible == value)
					return;
				SetVisible(value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterPaneVisibleIndex"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int VisibleIndex {
			get { return GetVisibleIndex(); }
			set {
				if (VisibleIndex == value)
					return;
				SetVisibleIndex(value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterPaneEnabled"),
#endif
		NotifyParentProperty(true), DefaultValue(true)]
		public bool Enabled {
			get { return GetBoolProperty("Enabled", true); }
			set {
				if (Enabled == value)
					return;
				SetBoolProperty("Enabled", true, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterPaneSize"),
#endif
		NotifyParentProperty(true), DefaultValue(typeof(Unit), "")]
		public Unit Size {
			get { return GetUnitProperty("Size", Unit.Empty); }
			set {
				SplitterRenderHelper.CheckSizeType(value, false, false, true, "Size");
				CommonUtils.CheckNegativeValue(value.Value, "Size");
				SetUnitProperty("Size", Unit.Empty, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterPaneAutoWidth"),
#endif
		NotifyParentProperty(true), DefaultValue(false)]
		public bool AutoWidth
		{
			get { return GetBoolProperty("AutoWidth", false); }
			set { SetBoolProperty("AutoWidth", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterPaneAutoHeight"),
#endif
		NotifyParentProperty(true), DefaultValue(false)]
		public bool AutoHeight
		{
			get { return GetBoolProperty("AutoHeight", false); }
			set { SetBoolProperty("AutoHeight", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterPaneScrollBars"),
#endif
		NotifyParentProperty(true), DefaultValue(ScrollBars.None)]
		public ScrollBars ScrollBars {
			get { return (ScrollBars)GetEnumProperty("ScrollBars", ScrollBars.None); }
			set { SetEnumProperty("ScrollBars", ScrollBars.None, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterPaneMinSize"),
#endif
		NotifyParentProperty(true), DefaultValue(typeof(Unit), "")]
		public Unit MinSize {
			get { return GetUnitProperty("MinSize", Unit.Empty); }
			set {
				SplitterRenderHelper.CheckSizeType(value, false, false, false, "MinSize");
				CommonUtils.CheckNegativeValue(value.Value, "MinSize");
				SetUnitProperty("MinSize", Unit.Empty, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterPaneMaxSize"),
#endif
		NotifyParentProperty(true), DefaultValue(typeof(Unit), "")]
		public Unit MaxSize {
			get { return GetUnitProperty("MaxSize", Unit.Empty); }
			set {
				SplitterRenderHelper.CheckSizeType(value, false, false, false, "MaxSize");
				CommonUtils.CheckNegativeValue(value.Value, "MaxSize");
				if (!MinSize.IsEmpty && !value.IsEmpty)
					CommonUtils.CheckGreaterOrEqual(UnitUtils.ConvertToPixels(value).Value, UnitUtils.ConvertToPixels(MinSize).Value, "MaxSize", "MinSize");
				SetUnitProperty("MaxSize", Unit.Empty, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterPaneAllowResize"),
#endif
		NotifyParentProperty(true), DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean AllowResize {
			get { return GetDefaultBooleanProperty("AllowResize", DefaultBoolean.Default); }
			set { SetDefaultBooleanProperty("AllowResize", DefaultBoolean.Default, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterPaneShowSeparatorImage"),
#endif
		NotifyParentProperty(true), DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean ShowSeparatorImage {
			get { return GetDefaultBooleanProperty("ShowSeparatorImage", DefaultBoolean.Default); }
			set {
				if (ShowSeparatorImage == value)
					return;
				SetDefaultBooleanProperty("ShowSeparatorImage", DefaultBoolean.Default, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterPaneShowCollapseForwardButton"),
#endif
		NotifyParentProperty(true), DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean ShowCollapseForwardButton {
			get { return GetDefaultBooleanProperty("ShowCollapseForwardButton", DefaultBoolean.Default); }
			set {
				if (ShowCollapseForwardButton == value)
					return;
				SetDefaultBooleanProperty("ShowCollapseForwardButton", DefaultBoolean.Default, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterPaneShowCollapseBackwardButton"),
#endif
		NotifyParentProperty(true), DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean ShowCollapseBackwardButton {
			get { return GetDefaultBooleanProperty("ShowCollapseBackwardButton", DefaultBoolean.Default); }
			set {
				if (ShowCollapseBackwardButton == value)
					return;
				SetDefaultBooleanProperty("ShowCollapseBackwardButton", DefaultBoolean.Default, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterPaneCollapsed"),
#endif
		NotifyParentProperty(true), DefaultValue(false)]
		public bool Collapsed {
			get { return GetBoolProperty("Collapsed", false); }
			set { SetBoolProperty("Collapsed", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterPaneContentUrl"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(false),
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)), UrlProperty]
		public string ContentUrl {
			get { return GetStringProperty("ContentUrl", ""); }
			set {
				if (ContentUrl == value)
					return;
				SetStringProperty("ContentUrl", "", value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterPaneContentUrlIFrameName"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(false)]
		public string ContentUrlIFrameName {
			get { return GetStringProperty("ContentUrlIFrameName", ""); }
			set { SetStringProperty("ContentUrlIFrameName", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterPaneContentUrlIFrameTitle"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(true)]
		public string ContentUrlIFrameTitle {
			get { return GetStringProperty("ContentUrlIFrameTitle", ""); }
			set { SetStringProperty("ContentUrlIFrameTitle", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterPanePaneStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SplitterPaneStyle PaneStyle {
			get { return style; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterPaneCollapsedStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SplitterPaneCollapsedStyle CollapsedStyle {
			get { return collapsedStyle; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int ScrollTop {
			get { return GetIntProperty("ScrollTop", 0); }
			set {
				CommonUtils.CheckNegativeValue(value, "ScrollTop");
				SetIntProperty("ScrollTop", 0, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int ScrollLeft {
			get { return GetIntProperty("ScrollLeft", 0); }
			set {
				CommonUtils.CheckNegativeValue(value, "ScrollLeft");
				SetIntProperty("ScrollLeft", 0, value);
			}
		}
		public string GetPath() {
			return Splitter.RenderHelper.GetPaneVisiblePath(this);
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return new IStateManager[] { Panes, Separators, Separator, PaneStyle, CollapsedStyle };
		}
		public override void Assign(CollectionItem source) {
			if(source is SplitterPane) {
				SplitterPane src = (SplitterPane)source;
				Panes.Assign(src.Panes);
				Separators.Assign(src.Separators);
				Separator.Assign(src.Separator);
				Name = src.Name;
				Visible = src.Visible;
				Enabled = src.Enabled;
				Size = src.Size;
				AutoWidth = src.AutoWidth;
				AutoHeight = src.AutoHeight;
				MinSize = src.MinSize;
				MaxSize = src.MaxSize;
				ScrollBars = src.ScrollBars;
				ShowSeparatorImage = src.ShowSeparatorImage;
				ShowCollapseForwardButton = src.ShowCollapseForwardButton;
				ShowCollapseBackwardButton = src.ShowCollapseBackwardButton;
				Collapsed = src.Collapsed;
				ContentUrl = src.ContentUrl;
				ContentUrlIFrameName = src.ContentUrlIFrameName;
				ContentUrlIFrameTitle = src.ContentUrlIFrameTitle;
				AllowResize = src.AllowResize;
				PaneStyle.Assign(src.PaneStyle);
				CollapsedStyle.Assign(src.CollapsedStyle);
			}
			base.Assign(source);
		}
		protected virtual SplitterPaneCollection CreatePanesCollection() {
			return new SplitterPaneCollection(this);
		}
		protected override ContentControlCollection CreateContentControlCollection(Control ownerControl) {
			return new SplitterContentControlCollection(ownerControl);
		}
		protected override ContentControl CreateContentControl() {
			return new SplitterContentControl();
		}
		public override string ToString() {
			return (Name != "") ? Name : GetType().Name;
		}
		protected override bool IsLoading() {
			if(Splitter != null)
				return ((IWebControlObject)Splitter).IsLoading();
			return base.IsLoading();
		}
		protected override bool IsDesignMode() {
			if(Splitter != null)
				return ((IWebControlObject)Splitter).IsDesignMode();
			return base.IsDesignMode();
		}
		protected override bool IsRendering() {
			if(Splitter != null)
				return ((IWebControlObject)Splitter).IsRendering();
			return base.IsRendering();
		}
		protected override void LayoutChanged() {
			if(Splitter != null)
				((IWebControlObject)Splitter).LayoutChanged();
			else
				base.LayoutChanged();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return Panes.GetEnumerator();
		}
		bool IHierarchyData.HasChildren {
			get { return Panes.Count > 0; }
		}
		object IHierarchyData.Item {
			get { return this; }
		}
		string IHierarchyData.Path {
			get { return ""; }
		}
		string IHierarchyData.Type {
			get { return this.GetType().Name; }
		}
		IHierarchicalEnumerable IHierarchyData.GetChildren() {
			return Panes;
		}
		IHierarchyData IHierarchyData.GetParent() {
			return Parent;
		}
		protected override IList GetDesignTimeItems() {
			return (IList)Panes;
		}
		protected override string[] GetDesignTimeHiddenPropertyNames() {
			return new string[] { "Panes" };
		}
	}
	[Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
	public class SplitterPaneCollection : HierarchicalCollection<SplitterPane> {
		protected internal SplitterPaneCollection()
			: base() {
		}
		public SplitterPaneCollection(SplitterPane owner)
			: base(owner) {
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new SplitterPane Owner { get { return (SplitterPane)base.Owner; } }
		protected ASPxSplitter Splitter { get { return (Owner != null) ? Owner.Splitter : null; } }
		protected internal Orientation Orientation { get { return Splitter.ReverseOrientation(Owner.Orientation); } }
		public SplitterPane this[string name] { get { return FindByIndex(IndexOfName(name)); } }
		[Obsolete("This method is now obsolete. Use the GetVisibleItem(int index) method instead.")]
		public SplitterPane GetVisiblePane(int index) {
			return GetVisibleItem(index);
		}
		[Obsolete("This method is now obsolete. Use the GetVisibleItemCount() method instead.")]
		public int GetVisiblePaneCount() {
			return GetVisibleItemCount();
		}
		public SplitterPane Add() {
			return Add("");
		}
		public SplitterPane Add(string name) {
			return AddInternal(new SplitterPane(name));
		}
		public int IndexOfName(string name) {
			return IndexOf(delegate(SplitterPane pane) {
				return pane.Name == name;
			});
		}
		[Obsolete("This method is now obsolete. Use the Item[string name] property instead.")]
		public SplitterPane GetByName(string name) {
			return this[name];
		}
		protected internal SplitterPane FindByName(string name) {
			return FindRecursive(delegate(SplitterPane pane) {
				return pane.Name == name;
			});
		}
		protected override void OnChanged() {
			if(Splitter != null)
				Splitter.PanesChanged();
		}
	}
}
