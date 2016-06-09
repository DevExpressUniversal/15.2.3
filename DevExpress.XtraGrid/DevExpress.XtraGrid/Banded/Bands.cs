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
using System.Data;
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Utils.Controls;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.XtraGrid.Design;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraEditors.Repository;
using DevExpress.Utils.Serializing;
using DevExpress.XtraEditors;
using System.Collections.Generic;
namespace DevExpress.XtraGrid.Views.BandedGrid {
	public class OptionsBand : ViewBaseOptions {
		bool allowMove, allowSize, fixedWidth, 
			showInCustomizationForm, showCaption, allowPress, allowHotTrack;
		public OptionsBand() {
			this.allowPress = this.allowHotTrack = true;
			this.showCaption = true;
			this.fixedWidth = false;
			this.showInCustomizationForm = true;
			this.allowMove = true;
			this.allowSize = true;
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("OptionsBandShowCaption"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool ShowCaption {
			get { return showCaption; }
			set {
				if(ShowCaption == value) return;
				bool prevValue = ShowCaption;
				showCaption = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowCaption", prevValue, ShowCaption));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("OptionsBandShowInCustomizationForm"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool ShowInCustomizationForm {
			get { return showInCustomizationForm; }
			set {
				if(ShowInCustomizationForm == value) return;
				bool prevValue = ShowInCustomizationForm;
				showInCustomizationForm = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowInCustomizationForm", prevValue, ShowInCustomizationForm));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("OptionsBandFixedWidth"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public bool FixedWidth {
			get { return fixedWidth; }
			set {
				if(FixedWidth == value) return;
				bool prevValue = FixedWidth;
				fixedWidth = value;
				OnChanged(new BaseOptionChangedEventArgs("FixedWidth", prevValue, FixedWidth));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("OptionsBandAllowSize"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool AllowSize {
			get { return allowSize; }
			set {
				if(AllowSize == value) return;
				bool prevValue = AllowSize;
				allowSize = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowSize", prevValue, AllowSize));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("OptionsBandAllowMove"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool AllowMove {
			get { return allowMove; }
			set {
				if(AllowMove == value) return;
				bool prevValue = AllowMove;
				allowMove = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowMove", prevValue, AllowMove));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("OptionsBandAllowHotTrack"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool AllowHotTrack {
			get { return allowHotTrack; }
			set {
				if(AllowHotTrack == value) return;
				bool prevValue = AllowHotTrack;
				allowHotTrack = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowHotTrack", prevValue, AllowHotTrack));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("OptionsBandAllowPress"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool AllowPress {
			get { return allowPress; }
			set {
				if(AllowPress == value) return;
				bool prevValue = AllowPress;
				allowPress = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowPress", prevValue, AllowPress));
			}
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				OptionsBand opt = options as OptionsBand;
				if(opt == null) return;
				this.showCaption = opt.ShowCaption;
				this.allowMove = opt.AllowMove;
				this.allowSize = opt.AllowSize;
				this.fixedWidth = opt.FixedWidth;
				this.showInCustomizationForm = opt.ShowInCustomizationForm;
				this.allowPress = opt.AllowPress;
				this.allowHotTrack = opt.AllowHotTrack;
			}
			finally {
				EndUpdate();
			}
		}
		internal void AssignInternal(BaseOptions options) {
			BeginUpdate();
			try {
				Assign(options);			
			}
			finally {
				CancelUpdate();
			}
		}
	}
	public class GridBandConverter : ComponentConverter {
		public GridBandConverter(Type type) : base(type) { }
	}
	[DesignTimeVisible(false), ToolboxItem(false), TypeConverter(typeof(GridBandConverter)),
	Designer("DevExpress.XtraGrid.Design.GridBandDesigner, " + AssemblyInfo.SRAssemblyGridDesign, typeof(System.ComponentModel.Design.IDesigner))]
	public class GridBand : Component, IAppearanceOwner, IXtraSerializableLayoutEx {
		protected const int LayoutIdAppearance = 1, LayoutIdData = 2, LayoutIdLayout = 3;
		FixedStyle _fixed;
		int rowCount, width, minWidth, visibleWidth, imageIndex;
		Image image;
		StringAlignment imageAlignment;
		GridBandCollection collection, children;
		GridBandColumnCollection fColumns;
		string caption, name, headerStyleName, toolTip, customizationCaption;
		GridBand parentBand;
		OptionsBand optionsBand;
		bool visible, autoFillDown, isBandDisposing;
		AppearanceObject appearanceHeader;
		object tag;
		public GridBand() {
			this.appearanceHeader = new AppearanceObject(this, false);
			this.appearanceHeader.Changed += new EventHandler(OnAppearanceChanged);
			this.optionsBand = CreateOptionsBand();
			this.optionsBand.Changed += new BaseOptionChangedEventHandler(OnOptionsChanged);
			this.isBandDisposing = false;
			this.imageIndex = -1;
			this.image = null;
			this.imageAlignment = StringAlignment.Near;
			this.autoFillDown = true;
			this._fixed = FixedStyle.None;
			this.minWidth = 10;
			this.visibleWidth = this.width = 70;
			this.children = CreateBandCollection();
			this.children.CollectionChanged += new CollectionChangeEventHandler(OnChildrenBandCollectionChanged);
			this.fColumns = new GridBandColumnCollection(this);
			this.fColumns.CollectionChanged += new CollectionChangeEventHandler(OnBandColumnCollectionChanged);
			this.rowCount = 1;
			this.visible = true;
			this.customizationCaption = this.toolTip = this.headerStyleName = this.name = this.caption = "";
			this.collection = null;
			this.parentBand = null;
		}
		protected virtual OptionsBand CreateOptionsBand() { return new OptionsBand(); }
		protected virtual GridBandCollection CreateBandCollection() { return new GridBandCollection(null, this); }
		protected virtual bool IsBandDisposing { get { return isBandDisposing; } }
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(Container != null) Container.Remove(this);
				this.optionsBand.Changed += new BaseOptionChangedEventHandler(OnOptionsChanged);
				this.isBandDisposing = true;
				Columns.Clear();
				if(Children != null) Children.Clear();
				if(Collection != null) {
					GridBandCollection coll = Collection;
					this.collection = null;
					coll.Remove(this);
				}
				this.appearanceHeader.Changed -= new EventHandler(OnAppearanceChanged);
			}
			base.Dispose(disposing);
		}
		bool ShouldSerializeAppearanceHeader() { return AppearanceHeader.ShouldSerialize(); } 
		void ResetAppearanceHeader() { AppearanceHeader.Reset(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridBandAppearanceHeader"),
#endif
 DXCategory(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), XtraSerializablePropertyId(LayoutIdAppearance)
		]
		public AppearanceObject AppearanceHeader {
			get { return appearanceHeader; }
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraGridLocalizedDescription("GridBandImageAlignment"),
#endif
 DefaultValue(StringAlignment.Near), XtraSerializableProperty(), Localizable(true)]
		public StringAlignment ImageAlignment {
			get { return imageAlignment; }
			set {
				if(ImageAlignment != value) {
					imageAlignment = value;
					OnChanged();
				}
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraGridLocalizedDescription("GridBandImageIndex"),
#endif
 DefaultValue(-1),
		Editor(typeof(DevExpress.Utils.Design.ImageIndexesEditor), typeof(System.Drawing.Design.UITypeEditor)),
		DevExpress.Utils.ImageList("Images"), XtraSerializableProperty(), Localizable(true)]
		public int ImageIndex {
			get { return imageIndex; }
			set {
				if(ImageIndex != value) {
					imageIndex = value;
					OnChanged();
				}
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraGridLocalizedDescription("GridBandImage"),
#endif
 DefaultValue(null),
		Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(System.Drawing.Design.UITypeEditor))]
		public Image Image {
			get { return image; }
			set {
				if(Image != value) {
					image = value;
					OnChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridBandTag"),
#endif
 DXCategory(CategoryName.Data), DefaultValue(null),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))
		]
		public virtual object Tag {
			get { return tag; }
			set { tag = value; }
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraGridLocalizedDescription("GridBandFixed"),
#endif
 DefaultValue(FixedStyle.None), XtraSerializableProperty(), XtraSerializablePropertyId(LayoutIdLayout), Localizable(true)]
		public virtual FixedStyle Fixed {
			get { return _fixed; }
			set {
				if(Fixed == value) return;
				_fixed = value;
				if(View != null)
					View.SetBandFixedStyle(this, value);
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraGridLocalizedDescription("GridBandWidth"),
#endif
 DefaultValue(70), XtraSerializableProperty(), XtraSerializablePropertyId(LayoutIdLayout), Localizable(true)]
		public virtual int Width {
			get { return width; }
			set {
				int w = MinWidth;
				if(value < w) value = w;
				if(Width == value) return;
				width = value;
				if(IsLoading) return;
				View.OnBandSizeChanged(this);
			}
		}
		public void Resize(int newWidth) {
			if(Width == newWidth) return;
			Width = newWidth;
			if(View != null) View.OnBandWidthChanged(this);
		}
		internal void SetWidthCore(int newWidth) { this.width = newWidth; }
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraGridLocalizedDescription("GridBandAutoFillDown"),
#endif
 DefaultValue(true), XtraSerializableProperty(), XtraSerializablePropertyId(LayoutIdLayout), Localizable(true)]
		public virtual bool AutoFillDown {
			get { return autoFillDown; }
			set {
				if(AutoFillDown == value) return;
				autoFillDown = value;
				OnChanged();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual object Images {
			get { return View == null ? null : View.Images; }
		}
		[Browsable(false)]
		public virtual int VisibleWidth { 
			get { return visibleWidth; } 
		}
		[Browsable(false)]
		public virtual int VisibleIndex {
			get {
				if(!Visible || Collection == null) return -1;
				int res = 0;
				for(int n = 0; n < Collection.Count; n++) {
					GridBand band = Collection[n];
					if(band == this) return res;
					if(band.Visible) res ++;
				}
				return res;
			}
			set { }
		}
		internal void SetVisibleWidthCore(int newWidth) { this.visibleWidth = newWidth; }
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraGridLocalizedDescription("GridBandMinWidth"),
#endif
 DefaultValue(10), XtraSerializableProperty(), Localizable(true)]
		public virtual int MinWidth {
			get { return minWidth; }
			set {
				if(value < 10) value = 10;
				if(MinWidth == value) return;
				minWidth = value;
				if(!CheckWidth()) OnChanged();
			}
		}
		internal void XtraClearColumns(XtraItemEventArgs e) {	
			OptionsLayoutGrid optGrid = e.Options as OptionsLayoutGrid;
			bool addNewColumns = (optGrid != null && optGrid.Columns.AddNewColumns);
			if(e.Item.ChildProperties == null || e.Item.ChildProperties.Count == 0) {
				if(!addNewColumns) Columns.Clear();
				return;
			}
			ArrayList list = new ArrayList();
			foreach(DevExpress.Utils.Serializing.Helpers.XtraPropertyInfo xp in e.Item.ChildProperties) {
				object col = XtraFindColumnsItem(new XtraItemEventArgs(this, Columns, xp));
				if(col != null) list.Add(col);
			}
			for(int n = Columns.Count - 1; n >= 0; n--) {
				GridColumn col = Columns[n];
				if(!list.Contains(col)) {
					if(addNewColumns) continue;
					Columns.RemoveAt(n);
				} else
					Columns.RemoveAt(n);
			}
		}
		internal object XtraCreateColumnsItem(XtraItemEventArgs e) { return null; }
		internal object XtraFindColumnsItem(XtraItemEventArgs e) { 
			if(e.Item.Value == null) return null;
			string name = e.Item.Value.ToString();
			if(name == string.Empty) return null;
			return View.Columns.ColumnByName(name);
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), 
		XtraSerializableProperty(XtraSerializationVisibility.NameCollection, true, true, true), XtraSerializablePropertyId(LayoutIdLayout)]
		public virtual GridBandColumnCollection Columns { get { return fColumns; } }
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraGridLocalizedDescription("GridBandRowCount"),
#endif
 DefaultValue(1), XtraSerializableProperty(), XtraSerializablePropertyId(LayoutIdLayout), Localizable(true)]
		public virtual int RowCount {
			get { return rowCount; }
			set {
				if(value < 1) value = 1;
				if(value > 10) value = 10;
				if(RowCount == value) return;
				rowCount = value;
				OnChanged();
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraGridLocalizedDescription("GridBandVisible"),
#endif
 DefaultValue(true), XtraSerializableProperty(), XtraSerializablePropertyId(LayoutIdLayout), Localizable(true)]
		public virtual bool Visible {
			get { return visible; }
			set {
				if(Visible == value) return;
				visible = value;
				OnChanged();
			}
		}
		[Browsable(false)]
		public virtual bool ReallyVisible {
			get {
				if(!Visible) return false;
				GridBand parentBand = ParentBand;
				while(parentBand != null) { 
					if(!parentBand.Visible) return false;
					parentBand = parentBand.ParentBand;
				}
				return true;
			}
		}
		[Browsable(false)]
		public virtual bool HasChildren { get { return Children != null && Children.Count > 0; } }
		internal void XtraClearChildren(XtraItemEventArgs e) {	Children.ClearBandItems(e); }
		internal object XtraCreateChildrenItem(XtraItemEventArgs e) { return Children.CreateBandItem(e); }
		internal object XtraFindChildrenItem(XtraItemEventArgs e) { return Children.FindBandItem(e); }
		internal void XtraSetIndexChildrenItem(XtraSetItemIndexEventArgs e) { Children.SetBandItemIndex(e); }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), 
		XtraSerializableProperty(true, true, true), XtraSerializablePropertyId(LayoutIdLayout)]
		public virtual GridBandCollection Children { get { return children; } }
		[Browsable(false)]
		public virtual GridBandCollection Collection { get { return collection; } }
		internal void SetCollectionCore(GridBandCollection bands) { 
			bool isNullCollection = this.collection == null;
			this.collection = bands; 
			if(bands == null) SetParentBandCore(null);
			else SetParentBandCore(bands.OwnerBand);
			if(!IsLoading && View != null && View.Container != null && Site == null) {
					if(View.Container != null) {
						try {
							View.Container.Add(this);
						} catch {
						}
					}
			}
			if(isNullCollection && !IsLoading && View != null && !View.IsDeserializing) {
				if(string.IsNullOrEmpty(Caption)) Caption = Name;
			}
		}
		[Browsable(false)]
		public virtual GridBand ParentBand { get { return parentBand; } }
		[Browsable(false)]
		public virtual GridBand RootBand { 
			get { 
				GridBand band = this;
				while(band.ParentBand != null) band = band.ParentBand;
				return band;
			}
		}
		internal void SetParentBandCore(GridBand band) { this.parentBand = band; } 
		[Browsable(false)]
		public virtual BandedGridView View { get { return Collection == null ? null : Collection.View; } }
		internal void SetViewCore(BandedGridView view) {
			if(children != null) children.SetViewCore(view);
		}
		[Browsable(false)]
		public virtual int Index {
			get { 
				if(Collection == null) return -1;
				return Collection.IndexOf(this);
			}
		}
		[Browsable(false)]
		public virtual int BandLevel {
			get {
				GridBand band = this;
				int level = 0;
				while(band.ParentBand != null) { level ++; band = band.ParentBand; }
				return level;
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraGridLocalizedDescription("GridBandCaption"),
#endif
 DefaultValue(""), XtraSerializableProperty(), Localizable(true)]
		public virtual string Caption {
			get { return caption; }
			set {
				if(value == null) value = string.Empty;
				if(Caption == value) return;
				caption = value;
				OnChanged();
			}
		}
		public string GetTextCaption() {
			if(View == null) return Caption;
			return View.GetNonFormattedCaption(Caption);
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraGridLocalizedDescription("GridBandCustomizationCaption"),
#endif
 DefaultValue(""), XtraSerializableProperty(), Localizable(true)]
		public string CustomizationCaption {
			get { return customizationCaption; }
			set {
				if(value == null) value = string.Empty;
				if(CustomizationCaption != value) {
					customizationCaption = value;
					OnChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridBandToolTip"),
#endif
 DXCategory(CategoryName.Appearance), DefaultValue(""), XtraSerializableProperty(), Localizable(true)]
		public virtual string ToolTip {
			get { return toolTip; }
			set {
				if(ToolTip == value) return;
				toolTip = value;
				OnChanged();
			}
		}
		[Browsable(false), DefaultValue(""), XtraSerializableProperty(), XtraSerializablePropertyId(-1), Localizable(true)]
		public virtual string Name {
			get { 
				if(this.Site != null) name = this.Site.Name;
				return name;
			}
			set {
				if(value == null) value = string.Empty;
				name = value;
				if(Site != null) Site.Name = name;
			}
		}
		internal bool ShouldSerializeOptionsBand() { return OptionsBand.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridBandOptionsBand"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public OptionsBand OptionsBand { get { return optionsBand; } }
		protected virtual bool CheckWidth() {
			int w = Math.Max(MinWidth, Width);
			if(w == Width) return false;
			this.width = w;
			OnChanged();
			return true;
		}
		internal void AssignCore(GridBand band, bool synchronize) { Assign(band, synchronize); }
		protected virtual void Assign(GridBand band, bool synchronize) {
			this.tag = band.Tag;
			this.customizationCaption = band.CustomizationCaption;
			this.autoFillDown = band.AutoFillDown;
			this.caption = band.Caption;
			this.toolTip = band.ToolTip;
			this._fixed = band.Fixed;
			this.imageAlignment = band.ImageAlignment;
			this.imageIndex = band.ImageIndex;
			this.image = band.Image;
			this.minWidth = band.MinWidth;
			try {
				this.Name = band.Name;
			}
			catch { }
			this.rowCount = band.RowCount;
			this.visible = band.Visible;
			this.width = band.Width;
			this.appearanceHeader.AssignInternal(band.AppearanceHeader);
			this.OptionsBand.AssignInternal(band.OptionsBand);
			this.Children.SetViewCore(View);
			if(synchronize) 
				this.Children.SynchronizeCore(band.Children);
			else
				this.Children.AssignCore(band.Children);
			this.Columns.Assign(band.Columns);
		}
		public override string ToString() {
			if(string.IsNullOrEmpty(Caption)) return Name;
			return Caption;
		}
		protected virtual void OnOptionsChanged(object sender, BaseOptionChangedEventArgs e) {
			OnChanged();
		}
		protected virtual void OnChanged() {
			if(View == null) return;
			View.OnBandChanged(this);
		}
		protected internal string GetCustomizationCaption() {
			if(CustomizationCaption == string.Empty) return Caption;
			return CustomizationCaption;
		}
		protected virtual void OnBandColumnCollectionChanged(object sender, CollectionChangeEventArgs e) {
			if(View != null) View.OnBandColumnCollectionChanged(this, e);
		}
		protected virtual void OnChildrenBandCollectionChanged(object sender, CollectionChangeEventArgs e) {
			GridBand band = e.Element as GridBand;
			if(e.Action == CollectionChangeAction.Add) {
				band.SetParentBandCore(this);
				if(!band.HasChildren && band.Columns.Count == 0 && (View != null && View.OptionsView.ColumnAutoWidth)) band.SetWidthCore(0);
				if(Columns.Count > 0) {
					MoveColumnsTo(band);
				}
			}
			if(e.Action == CollectionChangeAction.Remove) {
				band.SetParentBandCore(null);
				if(IsBandDisposing) band.Dispose();
			}
			if(!IsLoading && e.Action == CollectionChangeAction.Add) {
				CheckWidth();
			}
			if(View != null) View.OnBandCollectionChanged(sender, e);
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool IsLoading { get { return View == null || View.IsLoading; } }
		protected virtual void MoveColumnsTo(GridBand band) {
			if(Columns.Count == 0) return;
			BandedGridColumn[] cols = new BandedGridColumn[Columns.Count];
			(Columns as IList).CopyTo(cols, 0);
			foreach(BandedGridColumn col in cols) {
				band.Columns.Add(col);
			}
		}
		protected void OnAppearanceChanged(object sender, EventArgs e) {
			OnChanged();
		}
		void IXtraSerializableLayoutEx.ResetProperties(OptionsLayoutBase options) {
			OnResetSerializationProperties(options);
		}
		bool IXtraSerializableLayoutEx.AllowProperty(OptionsLayoutBase options, string propertyName, int id) {
			return OnAllowSerializationProperty(options, propertyName, id);
		}
		protected virtual bool OnAllowSerializationProperty(OptionsLayoutBase options, string propertyName, int id) {
			OptionsLayoutGrid optGrid = options as OptionsLayoutGrid;
			if(optGrid == null) return true;
			if(id == LayoutIdAppearance) return optGrid.Columns.StoreAppearance;
			if(optGrid.Columns.StoreLayout && id == LayoutIdLayout) return true;
			if(optGrid.Columns.StoreAllOptions) return true;
			return false;
		}
		protected virtual void OnResetSerializationProperties(OptionsLayoutBase options) {
			OptionsLayoutGrid optGrid = options as OptionsLayoutGrid;
			if(optGrid == null || optGrid.Columns.StoreAppearance) {
				AppearanceHeader.Reset();
			}
			if(optGrid != null && optGrid.Columns.StoreAllOptions) optGrid = null;
			if(optGrid == null || optGrid.Columns.StoreLayout) {
				this.visible = true;
				this.minWidth = 10;
				this.visibleWidth = width = 70;
			}
			if(optGrid == null || optGrid.Columns.StoreAllOptions) {
				OptionsBand.Reset();
				this.ImageIndex = -1;
				this.ImageAlignment = StringAlignment.Near;
				this.AutoFillDown = true;
				this.Fixed = FixedStyle.None;
				this.rowCount = 1;
				this.toolTip = this.headerStyleName = this.caption = "";
			}
		}
	}
	[ToolboxItem(false), ListBindable(false)]
	public class GridBandCollection : CollectionBase, IEnumerable<GridBand> {
		protected BandedGridView fView;
		GridBand ownerBand;
		internal CollectionChangeEventHandler CollectionChanged;
		public GridBandCollection(BandedGridView view, GridBand ownerBand) {
			this.fView = view;
			this.ownerBand = ownerBand;
		}
		[Browsable(false)] 
		public GridBand OwnerBand { get { return ownerBand; } }
		[Browsable(false)]
		public BandedGridView View { get { return fView; } }
		internal void SetViewCore(BandedGridView view) { 
			this.fView = view; 
			for(int n = Count - 1; n >= 0; n--) this[n].SetViewCore(view);
		}
#if !SL
	[DevExpressXtraGridLocalizedDescription("GridBandCollectionItem")]
#endif
		public GridBand this[string name] { 
			get { 
				for(int n = Count - 1; n >= 0; n--) {
					GridBand band = this[n];
					if(band.Name == name) return band;
				}
				return null;
			} 
		}
		protected internal bool EqualsCore(IList list) {
			if(list.Count != Count) return false;
			for(int n = 0; n < Count; n++) {
				if(this[n] != list[n]) return false;
			}
			return true;
		}
		internal void InternalAdd(IList list) {
			InnerList.AddRange(list);
		}
		internal void InternalCopy(IList list) {
			InnerList.Clear();
			InnerList.AddRange(list);
		}
#if !SL
	[DevExpressXtraGridLocalizedDescription("GridBandCollectionItem")]
#endif
		public GridBand this[int index] { get { return (GridBand)List[index]; }  }
		internal void AssignCore(GridBandCollection bands) { Assign(bands); }
		protected virtual void Assign(GridBandCollection bands) {
			InnerList.Clear();
			for(int n = 0; n < bands.Count; n++) {
				GridBand band = CreateBand();
				InnerList.Add(band);
				OnInternalInsertComplete(n, band, false);
			}
			for(int n = 0; n < bands.Count; n++) {
				this[n].AssignCore(bands[n], false);
			}
		}
		public virtual void AddRange(GridBand[] bands) {
			foreach(GridBand band in bands) {
				List.Add(band);
			}
		}
		public virtual GridBand Insert(int index) {
			if(index < 0) index = 0;
			if(index >= Count) return Add();
			GridBand band = CreateBand();
			List.Insert(index, band);
			return band;
		}
		public virtual GridBand Add() { return AddBand(""); }
		public virtual GridBand AddBand(string caption) {
			GridBand band = CreateBand();
			band.Caption = caption;
			List.Add(band);
			return band;
		}
		public virtual GridBand Add(GridBand band) {
			List.Add(band);
			return band;
		}
		public virtual void Insert(int position, GridBand band) {
			List.Insert(position, band);
		}
		public virtual void Remove(GridBand band) {
			if(List.Contains(band))
				List.Remove(band);
		}
		public virtual void InsertRoot(GridBand band) {
			if(band.Collection != null)
				band.Collection.InternalRemove(band);
			band.SetCollectionCore(null);
			GridBand[] bands = new GridBand[Count];
			InnerList.CopyTo(bands, 0);
			InnerList.Clear();
			band.Children.AddRange(bands);
			List.Insert(0, band);
		}
		internal void InternalRemove(GridBand band) { InnerList.Remove(band); }
		public virtual void MoveTo(int newIndex, GridBand band) {
			if(!Contains(band)) return;
			int prevIndex = IndexOf(band);
			if(newIndex < 0) newIndex = -1;
			if(newIndex > Count) newIndex = Count;
			if(prevIndex == newIndex) return;
			if(newIndex == -1) Remove(band);
			else {
				InnerList.Remove(band);
				if(newIndex > prevIndex) newIndex --;
				List.Insert(newIndex, band);
			}
		}
		public virtual bool CanAdd(GridBand band) {
			if(band == null || Contains(band)) return false;
			if(band == OwnerBand) return false;
			if(band.HasChildren && band.Children.ContainsEx(OwnerBand)) return false;
			return true;
		}
		public virtual bool Contains(GridBand band) { return List.Contains(band); }
		public virtual bool ContainsEx(GridBand band) {
			if(Contains(band)) return true;
			for(int n = 0; n < Count; n++) {
				GridBand b = this[n];
				if(b.Children.ContainsEx(band)) return true;
			}
			return false;
		}
		public virtual int IndexOf(GridBand band) { return List.IndexOf(band); }
		public virtual GridBand CreateBand() { return new GridBand(); }
		protected override void OnInsert(int position, object obj) {
			GridBand band = obj as GridBand;
			if(!CanAdd(band)) throw new ArgumentException("bad band");
			base.OnInsert(position, obj);
		}
		protected override void OnInsertComplete(int index, object obj) {
			base.OnInsertComplete(index, obj);
			GridBand band = obj as GridBand;
			if(band == null ) return;
			if(band.Collection != this ) {
				if(band.Collection != null) {
					band.Collection.InternalRemove(band);
				}
			}
			OnInternalInsertComplete(index, band, !synchronizing);
		}
		protected virtual void OnInternalInsertComplete(int index, GridBand band, bool fireEvents) {
			band.SetCollectionCore(this);
			if(fireEvents)
				RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, band));
		}
		protected override void OnRemoveComplete(int index, object obj) {
			base.OnRemoveComplete(index, obj);
			RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, obj));
		}
		protected override void OnClear() {
			if(View != null) View.BeginUpdate();
			try {
				for(int n = Count - 1; n >= 0; n --) {
					List.RemoveAt(n);
				}
			}
			finally {
				if(View != null) View.EndUpdate();
			}
		}
		protected internal virtual void DestroyBands() {
			ArrayList list = new ArrayList(List);
			InnerList.Clear();
			foreach(GridBand band in list) {
				band.Dispose();
			}
		}
		protected virtual void RaiseCollectionChanged(CollectionChangeEventArgs e) {
			if(CollectionChanged != null) CollectionChanged(this, e);
		}
		internal void OnChangedCore(GridBand band) { OnChanged(band); }
		protected virtual void OnChanged(GridBand band) {
		}
		internal void SetItemIndexCore(GridBand band, int value) { SetItemIndex(band, value); }
		protected virtual void SetItemIndex(GridBand band, int value) {
			if(value < 0) value = 0;
			if(value > Count) value = Count;
			int prevIndex = List.IndexOf(band);
			if(prevIndex < 0 || prevIndex == value) return;
			InnerList.RemoveAt(prevIndex);
			if(value > Count) value = Count;
			InnerList.Insert(value, band);
			OnChanged(band);
		}
#if !SL
	[DevExpressXtraGridLocalizedDescription("GridBandCollectionLastVisibleBand")]
#endif
		public virtual GridBand LastVisibleBand {
			get {
				for(int n = Count - 1; n >= 0; n--) {
					GridBand band = this[n];
					if(band.Visible) return band;
				}
				return null;
			}
		}
#if !SL
	[DevExpressXtraGridLocalizedDescription("GridBandCollectionVisibleBandCount")]
#endif
		public virtual int VisibleBandCount {
			get {
				int res = 0;
				for(int n = 0; n < Count; n++) {
					GridBand band = this[n];
					if(band.Visible) res++;
				}
				return res;
			}
		}
		public virtual GridBand GetVisibleBand(GridBand band) {
			int index = band.Index;
			while(band != null) {
				if(band.Visible) return band;
				if(++index >= Count) return null;
				band = this[index];
			}
			return null;
		}
		public virtual GridBand GetVisibleBand(int index) {
			int res = 0;
			for(int n = 0; n < Count; n++) {
				GridBand band = this[n];
				if(band.Visible) {
					if(res == index) return band;
					res ++;
				}
			}
			return null;
		}
#if !SL
	[DevExpressXtraGridLocalizedDescription("GridBandCollectionFirstVisibleBand")]
#endif
		public virtual GridBand FirstVisibleBand {
			get {
				for(int n = 0; n < Count; n++) {
					GridBand band = this[n];
					if(band.Visible) return band;
				}
				return null;
			}
		}
		bool synchronizing = false;
		internal void SynchronizeCore(GridBandCollection sourceBands) { Synchronize(sourceBands); }
		protected virtual void Synchronize(GridBandCollection sourceBands) {
			this.synchronizing = true;
			try {
				GridBand[] bands = new GridBand[InnerList.Count];
				InnerList.CopyTo(bands);
				InnerList.Clear();
				foreach(GridBand band in sourceBands) {
					GridBand destBand = null;
					if(band.Name != "") {
						destBand = FindBand(band.Name, bands);
					}
					if(destBand == null) destBand = Add();
					else InnerList.Add(destBand);
					OnInternalInsertComplete(-1, destBand, false);
					destBand.AssignCore(band, true);
				}
			} finally {
				this.synchronizing = false;
			}
		}
		protected virtual GridBand FindBand(string name, ICollection cols) {
			foreach(GridBand band in cols) {
				if(band.Name == name) return band;
			}
			return null;
		}
		protected internal void ClearBandItems(XtraItemEventArgs e) {
			if(e.Item.ChildProperties == null || e.Item.ChildProperties.Count == 0) {
				Clear();
				return;
			}
			OptionsLayoutGrid optGrid = e.Options as OptionsLayoutGrid;
			ArrayList list = new ArrayList();
			foreach(DevExpress.Utils.Serializing.Helpers.XtraPropertyInfo xp in e.Item.ChildProperties) {
				object col = FindBandItem(new XtraItemEventArgs(this, this, xp));
				if(col != null) list.Add(col);
			}
			for(int n = Count - 1; n >= 0; n--) {
				GridBand band = this[n];
				if(optGrid != null && optGrid.Columns.AddNewColumns) continue;
				if(!list.Contains(band)) band.Dispose();
			}
		}
		internal object CreateBandItem(XtraItemEventArgs e) {
			OptionsLayoutGrid optGrid = e.Options as OptionsLayoutGrid;
			if(optGrid != null) {
				if(optGrid.Columns.RemoveOldColumns) return null;
				if(!optGrid.Columns.StoreAllOptions) return null;
			}
			GridBand band = Add();
			return band;
		}
		internal object FindBandItem(XtraItemEventArgs e) {
			if(e.Item.ChildProperties == null) return null;
			string name = null;
			DevExpress.Utils.Serializing.Helpers.XtraPropertyInfo xp = e.Item.ChildProperties["Name"];
			if(xp != null && xp.Value != null) name = xp.Value.ToString();
			if(name == null || name == string.Empty) return null;
			return this[name];
		}
		internal void SetBandItemIndex(XtraSetItemIndexEventArgs e) {
			if(e.Item == null) return;
			int index = this.List.IndexOf(e.Item.Value);
			if(index == -1 || index == e.NewIndex) return;
			MoveTo(e.NewIndex, e.Item.Value as GridBand);
		}
		IEnumerator<GridBand> IEnumerable<GridBand>.GetEnumerator() {
			foreach(GridBand gridBand in InnerList)
				yield return gridBand;
		}
	}
	[ListBindable(false)]
	public class GridBandColumnCollection : CollectionBase {
		GridBand band;
		internal CollectionChangeEventHandler CollectionChanged;
		public GridBandColumnCollection(GridBand band) {
			this.band = band;
		}
#if !SL
	[DevExpressXtraGridLocalizedDescription("GridBandColumnCollectionVisibleColumnCount")]
#endif
		public virtual int VisibleColumnCount {
			get {
				int res = 0;
				for(int n = 0; n < Count; n++) {
					if(this[n].Visible) res++;
				}
				return res;
			}
		}
		protected internal virtual void Assign(GridBandRowCollection rows) {
			InnerList.Clear();
			foreach(GridBandRow row in rows) {
				InnerList.AddRange(row.Columns);
			}
		}
		protected internal virtual void Assign(GridBandColumnCollection columns) {
			InnerList.Clear();
			for(int n = 0; n < columns.Count; n++) {
				BandedGridColumn col = columns[n];
				BandedGridColumn currentColumn = View.Columns[col.AbsoluteIndex];
				currentColumn.SetOwnerBandCore(Band);
				currentColumn.SeColIndexCore(col.ColIndex);
				InnerList.Add(currentColumn);
			}
			ResetIndexes();
		}
#if !SL
	[DevExpressXtraGridLocalizedDescription("GridBandColumnCollectionBand")]
#endif
		public virtual GridBand Band { get { return band; } }
#if !SL
	[DevExpressXtraGridLocalizedDescription("GridBandColumnCollectionView")]
#endif
		public BandedGridView View { get { return Band == null ? null : Band.View; } }
#if !SL
	[DevExpressXtraGridLocalizedDescription("GridBandColumnCollectionItem")]
#endif
		public BandedGridColumn this[int index] { get { return List[index] as BandedGridColumn; } }
#if !SL
	[DevExpressXtraGridLocalizedDescription("GridBandColumnCollectionItem")]
#endif
		public BandedGridColumn this[string name] { 
			get {
				for(int n = Count - 1; n >=0; n--) {
					BandedGridColumn col = this[n];
					if(col.Name == name) return col;
				}
				return null;
			}
		}
		public virtual void Add(BandedGridColumn column) {
			if(!CanAddColumn(column)) return;
			column.OwnerBand = null;
			List.Add(column);
		}
		public virtual void Insert(int index, BandedGridColumn column) {
			if(!CanAddColumn(column)) {
				return;
			}
			column.OwnerBand = null;
			List.Insert(index, column);
		}
		public virtual void MoveTo(int newIndex, BandedGridColumn column) {
			if(!Contains(column)) return;
			int prevIndex = IndexOf(column);
			if(newIndex < 0) newIndex = -1;
			if(newIndex > Count) newIndex = Count;
			if(prevIndex == newIndex) return;
			if(newIndex == -1) Remove(column);
			else {
				InnerList.Remove(column);
				if(newIndex > prevIndex) newIndex --;
				try {
					column.WidthLocked = true;
					List.Insert(newIndex, column);
				}
				finally {
					column.WidthLocked = false;
				}
			}
		}
		public virtual void Remove(BandedGridColumn column) {
			if(List.Contains(column))
				List.Remove(column);
		}
		protected override void OnInsertComplete(int index, object obj) {
			base.OnInsertComplete(index, obj);
			ResetIndexes();
			BandedGridColumn col = obj as BandedGridColumn;
			if(col.OwnerBand != Band) {
				col.OwnerBand = Band;
				if(View != null) View.UpdateBandColumnsRowValues(Band);
			}
			RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, col));
		}
		protected override void OnRemoveComplete(int index, object obj) {
			base.OnRemoveComplete(index, obj);
			ResetIndexes();
			BandedGridColumn col = obj as BandedGridColumn;
			col.OwnerBand = null;
			if(View != null && !this.isClearing) View.UpdateBandColumnsRowValues(Band);
			RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, obj));
		}
		protected virtual void RaiseCollectionChanged(CollectionChangeEventArgs e) {
			if(CollectionChanged != null) CollectionChanged(this, e);
		}
		protected virtual bool IsLoading {
			get { return View == null || View.IsLoading; }
		}
		protected virtual bool CanAddColumn(BandedGridColumn column) {
			if(column == null || Contains(column)) return false;
			if(Band.Children.Count > 0) return false;
			if(column.View != null && column.View != View && !IsLoading) return false;
			return true;
		}
		public virtual bool Contains(BandedGridColumn column) { return List.Contains(column); }
		public virtual int IndexOf(BandedGridColumn column) { return List.IndexOf(column); } 
		protected internal virtual void ResetIndexes() {
			for(int n = 0; n < this.Count; n++) this[n].SeColIndexCore(n);
		}
		internal void SortCore(IComparer comparer) {
			InternalSort(comparer);
		}
		protected virtual void InternalSort(IComparer comparer) {
			ResetIndexes();
			InnerList.Sort(comparer);
			ResetIndexes();
		}
		bool isClearing = false;
		protected override void OnClear() {
			this.isClearing = true;
			try {
				if(View != null) View.BeginUpdate();
				try {
					for(int n = Count - 1; n >= 0; n--) {
						List.RemoveAt(n);
					}
				}
				finally {
					if(View != null) View.EndUpdate();
				}
			}
			finally {
				this.isClearing = false;
			}
			if(View != null) View.UpdateBandColumnsRowValues(Band);
		}
	}
	public class GridBandRow {
		BandedGridColumnReadOnlyCollection columns;
		public GridBandRow() {
			this.columns = new BandedGridColumnReadOnlyCollection(null);
		}
		public BandedGridColumnReadOnlyCollection Columns { get { return columns; } }
		public int MaxColumnRowCount {
			get {
				int res = 0;
				foreach(BandedGridColumn col in Columns) res = Math.Max(res, col.RowCount);
				return res;
			}
		}
		public bool IsColumnRowCountEquals {
			get {
				int res = -1;
				foreach(BandedGridColumn col in Columns) {
					if(res == -1) res = col.RowCount; 
					else {
						if(res == col.RowCount) continue;
						res = -2;
						break;
					}
				}
				return res != -2;
			}
		}
		public int CanFocusedColumnCount {
			get {
				int res = 0;
				foreach(BandedGridColumn col in Columns) {
					if(col.OptionsColumn.AllowFocus) res++;
				}
				return res;
			}
		}
	}
	[ListBindable(false)]
	public class GridBandRowCollection : CollectionBase {
		public GridBandRow this[int index] { get { return List[index] as GridBandRow; } }
		public int Add(GridBandRow row) {
			int index = List.IndexOf(row);
			if(index > -1) return index;
			return List.Add(row);
		}
		public void Insert(int index, GridBandRow row) {
			List.Insert(index, row);
		}
		public int IndexOf(GridBandRow row) { return List.IndexOf(row); }
		public GridBandRow FindRow(BandedGridColumn column) {
			foreach(GridBandRow row in this) {
				if(row.Columns.Contains(column)) return row;
			}
			return null;
		}
	}
}
