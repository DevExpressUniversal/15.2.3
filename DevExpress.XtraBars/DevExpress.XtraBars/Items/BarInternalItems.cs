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
using System.Windows.Forms;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Drawing;
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Localization;
using DevExpress.XtraEditors;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraBars.InternalItems;
using DevExpress.XtraBars.Ribbon;
using DevExpress.Skins;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraBars.Design;
namespace DevExpress.XtraBars.InternalItems {
	#region BarQBarCustomizationItem
	public class BarQBarCustomizationItem : BarCustomContainerItem {
		internal BarQBarCustomizationItem(BarManager barManager) : base(barManager) {
		}
		protected internal override BarItemPaintStyle CalcRealPaintStyle(BarItemLink link) {
			return BarItemPaintStyle.Caption;
		}
		protected internal override bool IsPrivateItem { get { return true; } }
	}
	public class BarQMenuAddRemoveButtonsItem : BarSubItem {
		internal BarQMenuAddRemoveButtonsItem(BarManager manager) {
			this.fIsPrivateItem = true;
			this.Manager = manager;
		}
	}
	#endregion
	#region BarQMenuCustomizationItem
	public class BarQMenuCustomizationItem : BarButtonItem {
		public Bar QBar;
		public virtual BarItemLink InnerLink { get { return Tag as BarItemLink; } }
		internal BarQMenuCustomizationItem(Bar bar, BarManager barManager, bool showChecks) : base(barManager, true) {
			this.QBar = bar;
			if(showChecks) {
				ButtonStyle = BarButtonStyle.Check; 
			}
		}
		public override bool CloseSubMenuOnClick {
			get { return !(Tag is BarItemLink); } 
		}
		protected internal override bool IsPrivateItem { get { return true; } }
		protected internal override void OnClick(BarItemLink link) {
			if(Tag is BarString) {
				BarString str = (BarString)Tag;
				if(str == BarString.CustomizeButton) {
					Manager.Customize();
				}
				if(str == BarString.ResetButton) {
					if(QBar != null) QBar.AskReset();
				}
			}
			base.OnClick(link);
		}
		public override bool Down {
			get {
				if(Tag is BarItemLink) {
					fDown = (Tag as BarItemLink).Visible;
				}
				return base.Down;
			}
			set {
				bool prevDown = fDown;
				base.Down = value;
				if(InnerLink != null) {
					InnerLink.Visible = !prevDown;
				}
			}
		}
	}
	#endregion
	#region BarRecentExpanderItem
	public class BarRecentExpanderItem : BarItem {
		public DevExpress.XtraBars.Controls.SubMenuBarControl FormBarControl;
		internal BarRecentExpanderItem(DevExpress.XtraBars.Controls.SubMenuBarControl formBarControl, BarManager barManager) : base(barManager) {
			this.FormBarControl = formBarControl;
		}
		protected internal override BarItemPaintStyle CalcRealPaintStyle(BarItemLink link) {
			return BarItemPaintStyle.Caption;
		}
		protected internal override bool IsPrivateItem { get { return true; } }
	}
	#endregion
	#region BarEmptyItem
	public class BarEmptyItemLink : BarStaticItemLink {
		public BarEmptyItemLink(BarItemLinkReadOnlyCollection ALinks, BarItem AItem, object linkedObject) : base(ALinks, AItem, linkedObject) { }
	}
	public class BarEmptyItem : BarStaticItem {
		internal BarEmptyItem(BarManager barManager) : base() {
			Manager = barManager;
		}
		protected internal override bool IsPrivateItem { get { return true; } }
	}
	#endregion
	public class BarScrollItem : BarItem {
		public DevExpress.XtraBars.Controls.SubMenuBarControl FormBarControl;
		internal BarScrollItem(BarManager barManager) : base(barManager) {
		}
		protected internal override BarItemPaintStyle CalcRealPaintStyle(BarItemLink link) {
			return BarItemPaintStyle.Caption;
		}
		protected internal override bool CanKeyboardSelect { get { return false; } }
		protected internal override bool IsPrivateItem { get { return true; } }
	}
	[ToolboxItem(false), DesignTimeVisible(false)]
	public class BarCloseItem : BarButtonItem {
		public DevExpress.XtraBars.Controls.SubMenuBarControl FormBarControl;
		internal BarCloseItem(BarManager barManager) : base(barManager, true) {
			Glyph = Manager.GetController().GetBitmap("CloseImage").Clone() as Bitmap;
		}
		protected internal override bool IsPrivateItem { get { return true; } }
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(Glyph != null) Glyph.Dispose();
				Glyph = null;
			}
			base.Dispose(disposing);
		}
	}
	[ToolboxItem(false), DesignTimeVisible(false),
	System.ComponentModel.Design.Serialization.DesignerSerializer("DevExpress.XtraBars.Design.Serialization.RibbonExpandCollapseItemCodeDomSerializer, " + AssemblyInfo.SRAssemblyBarsDesign, "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design")]
	public class RibbonExpandCollapseItem : BarButtonItem {
		internal RibbonExpandCollapseItem(RibbonControl ribbon) : base(ribbon.Manager, false) { }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override SuperToolTip SuperTip {
			get {
				if(base.SuperTip != null)
					return base.SuperTip;
				SuperToolTip sp = new SuperToolTip();
				ToolTipTitleItem header = new ToolTipTitleItem();
				BarString headerString = Ribbon.Minimized ? BarString.ExpandRibbonSuperTipHeader : BarString.CollapseRibbonSuperTipHeader;
				BarString textString = Ribbon.Minimized ? BarString.ExpandRibbonSuperTipText : BarString.CollapseRibbonSuperTipText;
				header.Text = BarLocalizer.Active.GetLocalizedString(headerString);
				sp.Items.Add(header);
				ToolTipItem item = new ToolTipItem();
				item.Text = BarLocalizer.Active.GetLocalizedString(textString);
				sp.Items.Add(item);
				return sp;
			}
			set {
				base.SuperTip = value;
			}
		}
		protected internal override DevExpress.XtraBars.Design.BarLinkInfoProvider CreateLinkInfoProvider(BarItemLink link) {
			return new RibbonExpandColldapeItemLinkInfoProvider(link);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Description {
			get { return base.Description; }
			set { base.Description = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BarShortcut ItemShortcut {
			get { return base.ItemShortcut; }
			set { base.ItemShortcut = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool ActAsDropDown {
			get { return base.ActAsDropDown; }
			set { base.ActAsDropDown = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool AllowAllUp {
			get { return base.AllowAllUp; }
			set { base.AllowAllUp = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BarItemAppearance Appearance {
			get { return base.Appearance; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BarItemAppearance AppearanceDisabled {
			get { return base.AppearanceDisabled; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BarButtonStyle ButtonStyle {
			get { return base.ButtonStyle; }
			set { base.ButtonStyle = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Caption {
			get { return base.Caption; }
			set { base.Caption = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool CloseSubMenuOnClick {
			get { return base.CloseSubMenuOnClick; }
			set { base.CloseSubMenuOnClick = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool Down {
			get { return base.Down; }
			set { base.Down = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override PopupControl DropDownControl {
			get { return base.DropDownControl; }
			set { base.DropDownControl = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool DropDownEnabled {
			get { return base.DropDownEnabled; }
			set { base.DropDownEnabled = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool Enabled {
			get { return base.Enabled; }
			set { base.Enabled = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Image Glyph {
			get { return base.Glyph; }
			set { base.Glyph = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Image GlyphDisabled {
			get { return base.GlyphDisabled; }
			set { base.GlyphDisabled = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DxImageUri ImageUri {
			get { return base.ImageUri; }
			set { base.ImageUri = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int GroupIndex {
			get { return base.GroupIndex; }
			set { base.GroupIndex = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Hint {
			get { return base.Hint; }
			set { base.Hint = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int ImageIndex {
			get { return base.ImageIndex; }
			set { base.ImageIndex = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int ImageIndexDisabled {
			get { return base.ImageIndexDisabled; }
			set { base.ImageIndexDisabled = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Image LargeGlyph {
			get { return base.LargeGlyph; }
			set { base.LargeGlyph = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Image LargeGlyphDisabled {
			get { return base.LargeGlyphDisabled; }
			set { base.LargeGlyphDisabled = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int LargeImageIndex {
			get { return base.LargeImageIndex; }
			set { base.LargeImageIndex = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int LargeImageIndexDisabled {
			get { return base.LargeImageIndexDisabled; }
			set { base.LargeImageIndexDisabled = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int LargeWidth {
			get { return base.LargeWidth; }
			set { base.LargeWidth = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int SmallWithoutTextWidth {
			get { return base.SmallWithoutTextWidth; }
			set { base.SmallWithoutTextWidth = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int SmallWithTextWidth {
			get { return base.SmallWithTextWidth; }
			set { base.SmallWithTextWidth = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BarItemVisibility Visibility {
			get { return base.Visibility; }
			set { base.Visibility = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new BarItemLinkAlignment Alignment {
			get { return base.Alignment; }
			set { base.Alignment = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new SuperToolTip DropDownSuperTip {
			get { return base.DropDownSuperTip; }
			set { base.DropDownSuperTip = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new BarMenuMerge MergeType {
			get { return base.MergeType; }
			set { base.MergeType = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new RibbonItemStyles RibbonStyle {
			get { return base.RibbonStyle; }
			set { base.RibbonStyle = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string ShortcutKeyDisplayString {
			get { return base.ShortcutKeyDisplayString; }
			set { base.ShortcutKeyDisplayString = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new BarItemPaintStyle PaintStyle {
			get { return base.PaintStyle; }
			set { base.PaintStyle = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new BarItemEventFireMode ItemClickFireMode {
			get { return base.ItemClickFireMode; }
			set { base.ItemClickFireMode = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string Name {
			get { return base.Name; }
			set { base.Name = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string AccessibleName {
			get { return base.AccessibleName; }
			set { base.AccessibleName = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string AccessibleDescription {
			get { return base.AccessibleDescription; }
			set { base.AccessibleDescription = value; }
		}
	}
	[ToolboxItem(false), DesignTimeVisible(false)]
	public class BarDesignTimeItem : BarButtonItem {
		internal BarDesignTimeItem(BarManager manager) : this() {
			this.fIsPrivateItem = true;
			try {
				this.Appearance.Font = new Font(Appearance.Font.FontFamily, 6, Appearance.Font.FontFamily.IsStyleAvailable(FontStyle.Bold) ? FontStyle.Bold : FontStyle.Regular);
			}
			catch {
				this.Appearance.Font = new Font(Appearance.Font.FontFamily, 6);
			}
			this.Caption = "[Add]";
			this.CloseSubMenuOnClick = false;
			this.Manager = manager;
		}
		public BarDesignTimeItem() {
		}
		protected internal override void OnPress(BarItemLink link) {
			Manager.Helper.CustomizationManager.ShowCreateItemDesigner(link as BarDesignTimeItemLink);
			Manager.SelectionInfo.UnPressLink(null);
		}
		public override bool Enabled {
			get {
				if(Manager is RibbonBarManager) return true;
				return Manager.Designer != null && !Manager.Designer.DebuggingState;
			}
			set {
			}
		}
	}
#region RibbonCollapsedPageItem
	[ToolboxItem(false), DesignTimeVisible(false),
	System.ComponentModel.Design.Serialization.DesignerSerializer("DevExpress.XtraBars.Design.Serialization.RibbonAutoHiddenPagesMenuItemCodeDomSerializer, " + AssemblyInfo.SRAssemblyBarsDesign, "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design")]
	public class AutoHiddenPagesMenuItem : BarButtonItem {
		internal AutoHiddenPagesMenuItem(RibbonControl ribbon) : base(ribbon.Manager, false) { }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override SuperToolTip SuperTip {
			get { return base.SuperTip; }
			set { base.SuperTip = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new int Id { get; set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Description {
			get { return base.Description; }
			set { base.Description = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BarShortcut ItemShortcut {
			get { return base.ItemShortcut; }
			set { base.ItemShortcut = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool ActAsDropDown {
			get { return base.ActAsDropDown; }
			set { base.ActAsDropDown = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool AllowAllUp {
			get { return base.AllowAllUp; }
			set { base.AllowAllUp = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BarItemAppearance Appearance {
			get { return base.Appearance; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BarItemAppearance AppearanceDisabled {
			get { return base.AppearanceDisabled; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BarButtonStyle ButtonStyle {
			get { return base.ButtonStyle; }
			set { base.ButtonStyle = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Caption {
			get { return base.Caption; }
			set { base.Caption = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool CloseSubMenuOnClick {
			get { return base.CloseSubMenuOnClick; }
			set { base.CloseSubMenuOnClick = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool Down {
			get { return base.Down; }
			set { base.Down = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override PopupControl DropDownControl {
			get { return base.DropDownControl; }
			set { base.DropDownControl = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool DropDownEnabled {
			get { return base.DropDownEnabled; }
			set { base.DropDownEnabled = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool Enabled {
			get { return base.Enabled; }
			set { base.Enabled = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Image Glyph {
			get { return base.Glyph; }
			set { base.Glyph = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Image GlyphDisabled {
			get { return base.GlyphDisabled; }
			set { base.GlyphDisabled = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DxImageUri ImageUri {
			get { return base.ImageUri; }
			set { base.ImageUri = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int GroupIndex {
			get { return base.GroupIndex; }
			set { base.GroupIndex = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Hint {
			get { return base.Hint; }
			set { base.Hint = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int ImageIndex {
			get { return base.ImageIndex; }
			set { base.ImageIndex = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int ImageIndexDisabled {
			get { return base.ImageIndexDisabled; }
			set { base.ImageIndexDisabled = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Image LargeGlyph {
			get { return base.LargeGlyph; }
			set { base.LargeGlyph = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Image LargeGlyphDisabled {
			get { return base.LargeGlyphDisabled; }
			set { base.LargeGlyphDisabled = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int LargeImageIndex {
			get { return base.LargeImageIndex; }
			set { base.LargeImageIndex = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int LargeImageIndexDisabled {
			get { return base.LargeImageIndexDisabled; }
			set { base.LargeImageIndexDisabled = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int LargeWidth {
			get { return base.LargeWidth; }
			set { base.LargeWidth = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int SmallWithoutTextWidth {
			get { return base.SmallWithoutTextWidth; }
			set { base.SmallWithoutTextWidth = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int SmallWithTextWidth {
			get { return base.SmallWithTextWidth; }
			set { base.SmallWithTextWidth = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BarItemVisibility Visibility {
			get { return base.Visibility; }
			set { base.Visibility = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new BarItemLinkAlignment Alignment {
			get { return base.Alignment; }
			set { base.Alignment = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new SuperToolTip DropDownSuperTip {
			get { return base.DropDownSuperTip; }
			set { base.DropDownSuperTip = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new BarMenuMerge MergeType {
			get { return base.MergeType; }
			set { base.MergeType = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new RibbonItemStyles RibbonStyle {
			get { return base.RibbonStyle; }
			set { base.RibbonStyle = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string ShortcutKeyDisplayString {
			get { return base.ShortcutKeyDisplayString; }
			set { base.ShortcutKeyDisplayString = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new BarItemPaintStyle PaintStyle {
			get { return base.PaintStyle; }
			set { base.PaintStyle = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new BarItemEventFireMode ItemClickFireMode {
			get { return base.ItemClickFireMode; }
			set { base.ItemClickFireMode = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string Name {
			get { return base.Name; }
			set { base.Name = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string AccessibleName {
			get { return base.AccessibleName; }
			set { base.AccessibleName = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string AccessibleDescription {
			get { return base.AccessibleDescription; }
			set { base.AccessibleDescription = value; }
		}
	}
#endregion
}
