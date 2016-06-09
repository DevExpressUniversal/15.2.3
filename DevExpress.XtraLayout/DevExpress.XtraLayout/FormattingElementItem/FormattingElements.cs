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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraLayout.Customization;
using DevExpress.XtraLayout.Localization;
using DevExpress.XtraLayout.Utils;
namespace DevExpress.XtraLayout {
	public class EmptySpaceItem :LayoutControlItem, IFixedLayoutControlItem {
		public EmptySpaceItem(LayoutControlGroup parent) : base(parent) { }
		protected internal override void SetPropertiesDefault() {
			isTextVisible = false;
		}
		public EmptySpaceItem() : base(null) { TextVisible = false; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool AllowHotTrack {
			get { return false; }
			set { }
		}
		[XtraSerializableProperty(), DefaultValue(""), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string TypeName {
			get { return "EmptySpaceItem"; }
		}
		public override string GetDefaultText() {
			if(Name != "")
				return Name;
			String itemText = LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.EmptySpaceItemDefaultText);
			return itemText;
		}
		protected override ViewInfo.BaseLayoutItemViewInfo CreateViewInfo() {
			return new ViewInfo.EmptySpaceItemViewInfo(this);
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("EmptySpaceItemTextVisible"),
#endif
 DefaultValue(false)]
		public override bool TextVisible {
			get { return base.TextVisible; }
			set { base.TextVisible = value; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Control Control {
			get { return null; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override int TextToControlDistance {
			get { return 0; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Locations TextLocation {
			get { return Locations.Default; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Size ControlMinSize {
			get { return Size.Empty; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Size ControlMaxSize {
			get { return Size.Empty; }
		}
		internal protected override void SetSizeWithoutCorrection(Size size) {
			base.SetSizeWithoutCorrection(size);
		}
#if !SL
	[DevExpressXtraLayoutLocalizedDescription("EmptySpaceItemMinSize")]
#endif
		public override Size MinSize {
			get {
				if(SizeConstraintsType == SizeConstraintsType.Default)
					return EmptySpaceItemDefaultMinSize;
				else
					return base.MinSize;
			}
			set { base.MinSize = value; }
		}
#if !SL
	[DevExpressXtraLayoutLocalizedDescription("EmptySpaceItemMaxSize")]
#endif
		public override Size MaxSize {
			get {
				if(SizeConstraintsType == SizeConstraintsType.Default)
					return EmptySpaceItemDefaultMaxSize;
				else
					return base.MaxSize;
			}
			set { base.MaxSize = value; }
		}
		protected virtual Size EmptySpaceItemDefaultMaxSize {
			get { return Size.Empty; }
		}
		protected virtual Size EmptySpaceItemDefaultMinSize {
			get { return new Size(10, 10); }
		}
		Control IFixedLayoutControlItem.OnCreateControl() { return OnCreate(); }
		void IFixedLayoutControlItem.OnInitialize() { OnInitialize(); }
		void IFixedLayoutControlItem.OnDestroy() { OnDestroy(); }
		string IFixedLayoutControlItem.CustomizationName { get { return GetDisplayName(); } }
		Image IFixedLayoutControlItem.CustomizationImage { get { return GetDisplayImage(); } }
		bool IFixedLayoutControlItem.AllowClipText { get { return GetIgnoreTextSize(); } }
		bool IFixedLayoutControlItem.AllowChangeTextVisibility { get { return GetAllowShowHideText(); } }
		bool IFixedLayoutControlItem.AllowChangeTextLocation { get { return GetAllowChangeTextPosition(); } }
		protected virtual void OnInitialize() { }
		protected virtual Control OnCreate() { return null; }
		protected virtual void OnDestroy() { }
		protected virtual string GetDisplayName() {
			return LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.EmptySpaceItemDefaultText);
		}
		protected virtual Image GetDisplayImage() {
			return LayoutControlImageStorage.Default.DragHeaderPainter.Images[4];
		}
		protected virtual bool GetIgnoreTextSize() { return true; }
		protected virtual bool GetAllowShowHideText() { return false; }
		protected virtual bool GetAllowChangeTextPosition() { return false; }
		protected override Type GetDefaultWrapperType() {
			if(Parent != null) {
				switch(Parent.LayoutMode) {
					case LayoutMode.Flow:
						return typeof(FlowEmptySpaceItemWrapper);
					case LayoutMode.Table:
						return typeof(TableEmptySpaceItemWrapper);
				}
			}
			return typeof(EmptySpaceItemWrapper);
		}
	}
	public class SimpleLabelItem : EmptySpaceItem {
		protected override Control OnCreate() {
			isTextVisible = true;
			return base.OnCreate();
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool TextVisible {
			get { return true; }
			set { }
		}
		protected override string GetDisplayName() {
			return LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.SimpleLabelItemDefaultText);
		}
		public override string GetDefaultText() {
			if(Name != "") return GetDisplayName() + Name;
			else return GetDisplayName() + LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.DefaultItemText);
		}
		protected override Image GetDisplayImage() {
			return LayoutControlImageStorage.Default.DragHeaderPainter.Images[6];
		}
		protected override bool GetIgnoreTextSize() { return false; }
		[XtraSerializableProperty(), DefaultValue(""), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string TypeName {
			get { return "SimpleLabelItem"; }
		}
		public override Size MinSize {
			get { return (SizeConstraintsType == SizeConstraintsType.Default) ? CalcDefaultMinMaxSize(true, true) : base.MinSize; }
			set { base.MinSize = value; }
		}
		public override Size MaxSize {
			get { return (SizeConstraintsType == SizeConstraintsType.Default) ? CalcDefaultMinMaxSize(false, true) : base.MaxSize; }
			set { base.MaxSize = value; }
		}
		protected override Size GetContentMinMaxSize(bool getMin) { return Size.Empty; }
		protected override Size CalcSizeWithLabel(Size size, Size labelSize) {
			return new Size(0, labelSize.Height);
		}
		protected override Type GetDefaultWrapperType() {
			if(Parent != null) {
				switch(Parent.LayoutMode) {
					case LayoutMode.Flow:
						return typeof(FlowSimpleLabelItemWrapper);
					case LayoutMode.Table:
						return typeof(TableSimpleLabelItemWrapper);
				}
			}
			return typeof(SimpleLabelItemWrapper);
		}
	}
	public class SimpleSeparator : EmptySpaceItem {
		LayoutType layoutTypeCore = LayoutType.Vertical;
		Rectangle boundsCore = Rectangle.Empty;
		int separatorWidthCore = 6;
		public LayoutType LayoutType { get { return layoutTypeCore; } }
		public override Size Size {
			get { return base.Size; }
			set {
				base.Size = value;
				if(IsInitializing) UpdateLayoutType(Bounds);
			}
		}
		protected internal void UpdateLayoutType() {
			UpdateLayoutType(Bounds);
		}
		protected void UpdateLayoutType(Rectangle bounds) {
			layoutTypeCore = (bounds.Width > bounds.Height) ? LayoutType.Horizontal : LayoutType.Vertical;
			boundsCore = bounds;
		}
		protected internal override void SetBounds(Rectangle bounds) {
			base.SetBounds(bounds);
			UpdateLayoutType(bounds);
		}
		[XtraSerializableProperty(), DefaultValue(""), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string TypeName { get { return "SimpleSeparator"; } }
		protected override string GetDisplayName() {
			return LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.SimpleSeparatorItemDefaultText);
		}
		public override string GetDefaultText() {
			if(Name != "")
				return Name;
			String itemText = LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.SimpleSeparatorItemDefaultText);
			return itemText;
		}
		protected override Image GetDisplayImage() {
			return LayoutControlImageStorage.Default.DragHeaderPainter.Images[5];
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool TextVisible { get { return false; } set { } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		public override Size TextSize {
			get { return base.TextSize; }
			set { base.TextSize = value; }
		}
		protected internal SkinElementInfo SkinElementInfo {
			get { return (LayoutType == LayoutType.Horizontal) ? HorzSkinElementInfo : VertSkinElementInfo; }
		}
		protected SkinElementInfo HorzSkinElementInfo {
			get {
				if(Owner == null) return null;
				else return new SkinElementInfo(CommonSkins.GetSkin(Owner.LookAndFeel)[CommonSkins.SkinLabelLine], Rectangle.Empty);
			}
		}
		protected SkinElementInfo VertSkinElementInfo {
			get {
				if(Owner == null) return null;
				else return new SkinElementInfo(CommonSkins.GetSkin(Owner.LookAndFeel)[CommonSkins.SkinLabelLineVert], Rectangle.Empty);
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int SeparatorWidth {
			get {
				if(Owner == null)
					return separatorWidthCore;
				if(GetPaintingType() == PaintingType.Skinned) {
					if(this.layoutTypeCore == LayoutType.Horizontal)
						return HorzSkinElementInfo.Element.Size.MinSize.Height + Spacing.Height;
					else
						return VertSkinElementInfo.Element.Size.MinSize.Width + Spacing.Width;
				}
				else
					return separatorWidthCore;
			}
			set {
				StartChange();
				separatorWidthCore = value;
				ShouldUpdateConstraintsDoUpdate = true;
				ShouldResize = true;
				ComplexUpdate();
				EndChange();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override int BestFitWeight { get { return 1; } }
		protected internal override int BestFitCore() { return 1; }
		protected Size CalcSeparatorSizeCore(bool calcMax) {
			LayoutRectangle lREct = new LayoutRectangle(boundsCore, layoutTypeCore);
			lREct.Height = SeparatorWidth;
			if(calcMax) lREct.Width = 0;
			else
				lREct.Width = SeparatorWidth;
			return lREct.Rectangle.Size;
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Size MinSize { get { return CalcSeparatorSizeCore(false); } set { } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Size MaxSize { get { return CalcSeparatorSizeCore(true); } set { } }
		protected override Type GetDefaultWrapperType() {
			return typeof(SimpleSeparatorItemWrapper);
		}
	}
	public class EmptySpaceItemWrapper : LayoutWrapperBase {
		protected BaseLayoutItem Item {
			get { return WrappedObject as BaseLayoutItem; }
		}
		[Category("Name"), DefaultValue(true)]
		public virtual string Name { get { return Item.Name; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual AppearanceObject AppearanceItemCaption { get { return Item.AppearanceItemCaption; } }
		public virtual Point Location { get { return Item.Location; } set { Item.Location = value; OnSetValue(Item, value); } }
		public virtual Size Size { get { return Item.Size; } set { Item.Size = value; OnSetValue(Item, value); } }
		public virtual Size MaxSize { get { return Item.MaxSize; } set { Item.MaxSize = value; OnSetValue(Item, value); } }
		public virtual Size MinSize { get { return Item.MinSize; } set { Item.MinSize = value; OnSetValue(Item, value); } }
		public virtual DevExpress.XtraLayout.Utils.Padding Padding { get { return Item.Padding; } set { Item.Padding = value; OnSetValue(Item, value); } }
		public virtual DevExpress.XtraLayout.Utils.Padding Spacing { get { return Item.Spacing; } set { Item.Spacing = value; OnSetValue(Item, value); } }
		[DefaultValue(true)]
		public virtual bool AllowHide { get { return Item.AllowHide; } set { Item.AllowHide = value; OnSetValue(Item, value); } }
		[DefaultValue(LayoutVisibility.Always)]
		public virtual LayoutVisibility Visibility { get { return Item.Visibility; } set { Item.Visibility = value; OnSetValue(Item, value); } }
		[DefaultValue(true)]
		public virtual bool ShowInCustomizationForm { get { return Item.ShowInCustomizationForm; } set { Item.ShowInCustomizationForm = value; OnSetValue(Item, value); } }
		public override BasePropertyGridObjectWrapper Clone() {
			return new EmptySpaceItemWrapper();
		}
	}
	public class FlowEmptySpaceItemWrapper : EmptySpaceItemWrapper {
		[Category("OptionsFlowLayoutItem"), DefaultValue(false)]
		public virtual bool NewLineInFlowLayout { get { return Item.StartNewLine; } set { Item.StartNewLine = value; OnSetValue(Item, value); } }
		public override BasePropertyGridObjectWrapper Clone() {
			return new FlowEmptySpaceItemWrapper();
		}
	}
	public class TableEmptySpaceItemWrapper :EmptySpaceItemWrapper {
		[Category("OptionsTableLayoutItem"), DefaultValue(1)]
		public virtual int RowSpan { get { return Item.OptionsTableLayoutItem.RowSpan; } set { Item.OptionsTableLayoutItem.RowSpan = value; OnSetValue(Item, value); } }
		[Category("OptionsTableLayoutItem"), DefaultValue(1)]
		public virtual int ColumnSpan { get { return Item.OptionsTableLayoutItem.ColumnSpan; } set { Item.OptionsTableLayoutItem.ColumnSpan = value; OnSetValue(Item, value); } }
		[Category("OptionsTableLayoutItem")]
		public virtual int RowIndex { get { return Item.OptionsTableLayoutItem.RowIndex; } set { Item.OptionsTableLayoutItem.RowIndex = value; OnSetValue(Item, value); } }
		[Category("OptionsTableLayoutItem")]
		public virtual int ColumnIndex { get { return Item.OptionsTableLayoutItem.ColumnIndex; } set { Item.OptionsTableLayoutItem.ColumnIndex = value; OnSetValue(Item, value); } }
		public override BasePropertyGridObjectWrapper Clone() {
			return new TableEmptySpaceItemWrapper();
		}
	}
	public class SimpleSeparatorItemWrapper : EmptySpaceItemWrapper {
		public override BasePropertyGridObjectWrapper Clone() {
			return new SimpleSeparatorItemWrapper();
		}
	}
	public class SimpleLabelItemWrapper : EmptySpaceItemWrapper {
		[Category("Appearance"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
		public override AppearanceObject AppearanceItemCaption { get { return Item.AppearanceItemCaption; } }
		[Category("Text"), DefaultValue("")]
		public virtual string CustomizationFormText { get { return Item.CustomizationFormText; } set { Item.CustomizationFormText = value; OnSetValue(Item, value); } }
		[Category("Text"), DefaultValue("")]
		public virtual string Text { get { return Item.Text; } set { Item.Text = value; OnSetValue(Item, value); } }
		[Category("Text")]
		public virtual Size TextSize { get { return Item.TextSize; } set { Item.TextSize = value; OnSetValue(Item, value); } }
		[Category("ToolTip")]
		public virtual BaseLayoutItemOptionsToolTip OptionsToolTip { get { return Item.OptionsToolTip; } }
		public override BasePropertyGridObjectWrapper Clone() {
			return new SimpleLabelItemWrapper();
		}
	}
	public class FlowSimpleLabelItemWrapper : SimpleLabelItemWrapper {
		[Category("OptionsFlowLayoutItem"), DefaultValue(false)]
		public virtual bool NewLineInFlowLayout { get { return Item.StartNewLine; } set { Item.StartNewLine = value; OnSetValue(Item, value); } }
		public override BasePropertyGridObjectWrapper Clone() {
			return new FlowSimpleLabelItemWrapper();
		}
	}
	public class TableSimpleLabelItemWrapper :SimpleLabelItemWrapper {
		[Category("OptionsTableLayoutItem"), DefaultValue(1)]
		public virtual int RowSpan { get { return Item.OptionsTableLayoutItem.RowSpan; } set { Item.OptionsTableLayoutItem.RowSpan = value; OnSetValue(Item, value); } }
		[Category("OptionsTableLayoutItem"), DefaultValue(1)]
		public virtual int ColumnSpan { get { return Item.OptionsTableLayoutItem.ColumnSpan; } set { Item.OptionsTableLayoutItem.ColumnSpan = value; OnSetValue(Item, value); } }
		[Category("OptionsTableLayoutItem")]
		public virtual int RowIndex { get { return Item.OptionsTableLayoutItem.RowIndex; } set { Item.OptionsTableLayoutItem.RowIndex = value; OnSetValue(Item, value); } }
		[Category("OptionsTableLayoutItem")]
		public virtual int ColumnIndex { get { return Item.OptionsTableLayoutItem.ColumnIndex; } set { Item.OptionsTableLayoutItem.ColumnIndex = value; OnSetValue(Item, value); } }
		public override BasePropertyGridObjectWrapper Clone() {
			return new TableSimpleLabelItemWrapper();
		}
	}
}
