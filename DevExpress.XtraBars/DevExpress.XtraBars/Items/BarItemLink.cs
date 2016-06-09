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
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Drawing;
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraBars.Ribbon.Handler;
using DevExpress.XtraBars.Ribbon.Helpers;
using DevExpress.XtraBars.Ribbon.Internal;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.XtraBars.Utils;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.Utils.Win;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Text;
using DevExpress.Utils.VisualEffects;
using DevExpress.Utils.Svg;
using DevExpress.Utils.Drawing;
namespace DevExpress.XtraBars {
	[Flags]
	public enum BarLinkUserDefines { None = 0, Caption = 1, Glyph = 2, PaintStyle = 4, Width = 8, KeyTip = 16, DropDownKeyTip = 32, RibbonStyle = 64, Alignment = 128, EditWidth = 256 };
	public enum BarLinkAction { CustomizeSelect, KeyClick, MouseClick, MouseDoubleClick, StartDrag, DropSelect, KeyboardHighlight, Highlight, UnHighlight, Press, PressArrow, MouseMove, Undefined};
	public interface ISpringLink { 
		bool SpringAllow { get ; }
		int SpringWidth { get; set; }
		int SpringMinWidth { get; }
		int SpringTempWidth { get; set; }
	}
	public class BarItemLink : IDisposable, ICloneable, IRibbonItem, IBarLinkTimer, ISupportRibbonKeyTip, IToolTipLookAndFeelProvider, ISupportAdornerElementBarItemLink {
		internal enum MouseTimerActionEnum {None, Open, Close, Check};
		RibbonItemViewInfo ribbonItemInfo;
		Bitmap userGlyph = null;
		BarManager manager;
		BarItemLinkReadOnlyCollection links;
		BarItem item;
		BarItemLink clonedFromLink;
		int clickCount, imageIndex, maxLinkTextWidth = 0;
		internal object linkedObject;
		object data;
		string userCaption;
		int userWidth, userEditWidth;
		RibbonItemStyles userRibbonStyle;
		BarItemLinkAlignment userAlignment;
		BarItemPaintStyle userPaintStyle;
		BarLinkUserDefines userDefine;
		string userKeyTip = string.Empty;
		string itemKeyTip = string.Empty;
		int firstIndex = 0;
		bool beginGroup, mostRecentlyUsed, visible, actAsButtonGroup;
		internal int recentIndex;
		internal object ownerControl;
		internal bool initialized, internalBeginGroup;
		static Size DefaultGlyphSize = new Size(16, 16);
		static Size DefaultLargeGlyphSize = new Size(32, 32);
		internal const 
			bool DefaultMostRecentlyUsed = true,
				 DefaultVisible = true,
				 DefaultBeginGroup = false;
		internal BarItemLink ClonedFromLink { get { return clonedFromLink; } }
		internal bool IsClonedLink { get { return ClonedFromLink != null; } }
		object ICloneable.Clone() {
			BarItemLink link = Item.CreateLink(Links, linkedObject);
			link.clonedFromLink = this;
			if(!GetType().Equals(link.GetType())) {
				throw new Exception(string.Format("Can't Clone() link ({0})!", GetType().ToString()));
			}
			link.Assign(this);
			link.ribbonItemInfo = RibbonItemInfo;
			link.beginGroup = this.beginGroup;
			link.actAsButtonGroup = this.actAsButtonGroup;
			link.imageIndex = this.imageIndex;
			link.recentIndex = this.recentIndex;
			link.data = this.data;
			link.manager = this.manager;
			link.maxLinkTextWidth = this.maxLinkTextWidth;
			link.userKeyTip = this.userKeyTip;
			return link;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), XtraSerializableProperty]
		public bool IsMerged {
			get {
				if(Item.Ribbon != null) {
					RibbonQuickToolbarItemLinkCollection coll = Links as RibbonQuickToolbarItemLinkCollection;
					if(coll != null)
						return coll.Ribbon == Item.Ribbon.MergeOwner;
					return false;
				}
				return false;
			}
		}
		Rectangle glyphBounds;
		protected internal Rectangle GlyphBounds { get { return glyphBounds; } set { glyphBounds = value; } }
		protected internal BaseRibbonViewInfo GetRibbonViewInfo() {
			if(RibbonItemInfo == null) return null;
			return RibbonItemInfo.ViewInfo;
		}
		protected internal RibbonItemViewInfo RibbonItemInfo { 
			get { return ribbonItemInfo; } 
			set { ribbonItemInfo = value; } 
		}
		internal int MaxLinkTextWidth {
			get { return maxLinkTextWidth; }
			set { maxLinkTextWidth = value; }
		}
		protected internal void Assign(BarItemLink link) {
			if(link == null) return;
			this.userGlyph = link.userGlyph;
			this.userCaption = link.userCaption;
			this.userWidth = link.userWidth;
			this.userEditWidth = link.userEditWidth;
			this.userPaintStyle = link.userPaintStyle;
			this.userDefine = link.userDefine;
			this.mostRecentlyUsed = link.mostRecentlyUsed;
			this.clickCount = link.clickCount;
			this.visible = link.visible;
			this.inplaceHolder = link.inplaceHolder;
		}
		protected BarItemLink(BarItemLinkReadOnlyCollection links, BarItem item, object linkedObject) {
			InitVars();
			this.links = links;
			this.item = item;
			this.linkedObject = linkedObject;
			this.initialized = true;
			UpdateLink();
		}
		protected virtual void InitVars() {
			this.userCaption = "";
			this.userWidth = 0;
			this.userEditWidth = 0;
			this.userDefine = BarLinkUserDefines.None;
			this.userPaintStyle = BarItemPaintStyle.Standard;
			this.userRibbonStyle = RibbonItemStyles.Default;
			this.userAlignment = BarItemLinkAlignment.Default;
			this.manager = null;
			this.data = null;
			this.internalBeginGroup = false;
			this.initialized = false;
			this.links = null;
			this.item = null;
			this.linkedObject = null;
			this.clickCount = 0;
			this.recentIndex = -1;
			this.ownerControl = null;
			this.visible = DefaultBeginGroup;
			this.beginGroup = DefaultBeginGroup;
			this.actAsButtonGroup = GetActAsButtonGroupDefault();
			this.mostRecentlyUsed = DefaultMostRecentlyUsed;
			this.enabledLinkCore = true;
		}
		internal virtual bool AllowDrawLinkDisabledInCustomizationMode { get { return false; } }
		DevExpress.Accessibility.BaseAccessible dxAccessible;
		protected internal virtual DevExpress.Accessibility.BaseAccessible DXAccessible { 
			get {
				if(dxAccessible == null) dxAccessible = CreateAccessibleInstance();
				return dxAccessible;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BarListItemLink OwnerListItemLink { get; set; }
		protected virtual DevExpress.Accessibility.BaseAccessible CreateAccessibleInstance() {
			return new DevExpress.XtraBars.Accessible.BaseLinkAccessible(this); 
		}
		[Browsable(false)]
		public bool IsLargeImageExist { get { return Item != null && Item.IsLargeImageExist; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual object Data { 
			get { return data; }
			set { data = value; }
		}
		[Browsable(false)]
		public object LinkedObject { get { return linkedObject; } }
		[Browsable(false), XtraSerializableProperty()]
		public virtual bool IsPageGroupContentToolbarButtonLink {
			get { return Item.Tag is RibbonPageGroup; }
			set { }
		}
		[Browsable(false), XtraSerializableProperty()]
		public virtual int GalleryLinkIndex {
			get {
				RibbonToolbarPopupItemLink link = this as RibbonToolbarPopupItemLink;
				if(IsGalleryToolbarItemLink) {
					return link.GalleryLink.Holder.ItemLinks.IndexOf(link.GalleryLink);
				}
				return -1;
			}
			set { }
		}
		[Browsable(false), XtraSerializableProperty()]
		public virtual string PageGroupName {
			get {
				RibbonPageGroup group = null;
				if(IsGalleryToolbarItemLink) {
					RibbonToolbarPopupItemLink link = this as RibbonToolbarPopupItemLink;
					RibbonPageGroupItemLinkCollection coll = link.GalleryLink.Holder as RibbonPageGroupItemLinkCollection;
					if(coll != null && coll.PageGroup != null) return coll.PageGroup.Name;
					return string.Empty;
				}
				group = Item.Tag as RibbonPageGroup;
				if(group == null) return string.Empty;
				return group.Name;
			}
			set { }
		}
		[Browsable(false), XtraSerializableProperty()]
		public virtual bool IsGalleryToolbarItemLink {
			get { return Item.Tag is RibbonGalleryBarItem; }
			set { }
		}
		[Browsable(false), XtraSerializableProperty()]
		public virtual string GalleryBarItemName {
			get {
				RibbonGalleryBarItem item = Item.Tag as RibbonGalleryBarItem;
				if(item == null) return string.Empty;
				return item.Name;
			}
			set { }
		}
		[Browsable(false), XtraSerializableProperty()]
		public virtual int ItemId {
			get { return Item != null ? Item.Id : -1; }
			set { }
		}
		[Browsable(false),  XtraSerializableProperty()]
		public int ClickCount { 
			get { return clickCount; }
			set { 
				if(ClickCount == value) return;
				clickCount = value;
			}
		}
		[Browsable(false),  XtraSerializableProperty()]
		public int UserWidth { 
			get { return userWidth; }
			set { 
				if(UserWidth == value) return;
				userWidth = value;
				this.userDefine |= BarLinkUserDefines.Width;
				OnLinkChanged();
			}
		}
		[Browsable(false), XtraSerializableProperty()]
		public int UserEditWidth {
			get { return userEditWidth; }
			set {
				if(UserEditWidth == value) return;
				userEditWidth = value;
				this.userDefine |= BarLinkUserDefines.EditWidth;
				OnLinkChanged();
			}
		}
		[Browsable(false),  XtraSerializableProperty()]
		public string UserCaption { 
			get { return userCaption; }
			set { 
				if(UserCaption == value) return;
				userCaption = value;
				this.userDefine |= BarLinkUserDefines.Caption;
				OnLinkChanged();
			}
		}
		[Browsable(false), XtraSerializableProperty()]
		public RibbonItemStyles UserRibbonStyle {
			get { return userRibbonStyle; }
			set {
				if(UserRibbonStyle == value) return;
				userRibbonStyle = value;
				this.userDefine |= BarLinkUserDefines.RibbonStyle;
				OnLinkChanged();
			}
		}
		[Browsable(false), XtraSerializableProperty()]
		public BarItemLinkAlignment UserAlignment {
			get { return userAlignment; }
			set {
				if(UserAlignment == value) return;
				userAlignment = value;
				this.userDefine |= BarLinkUserDefines.Alignment;
				OnLinkChanged();
			}
		}
		[Browsable(false),  XtraSerializableProperty()]
		public BarItemPaintStyle UserPaintStyle { 
			get { return userPaintStyle; }
			set { 
				if(UserPaintStyle == value) return;
				userPaintStyle = value;
				this.userDefine |= BarLinkUserDefines.PaintStyle;
				OnLinkChanged();
			}
		}
		protected internal bool IsUserDefine(BarLinkUserDefines userDefine) { return ((UserDefine & userDefine) != 0); }
		[Browsable(false),  XtraSerializableProperty(999)]
		public BarLinkUserDefines UserDefine { 
			get { return userDefine; }
			set { 
				if(UserDefine == value) return;
				userDefine = value;
				if(!IsUserDefine(BarLinkUserDefines.Caption)) this.userCaption = "";
				if(!IsUserDefine(BarLinkUserDefines.PaintStyle)) this.userPaintStyle = BarItemPaintStyle.Standard;
				if(!IsUserDefine(BarLinkUserDefines.Width)) this.userWidth = 0;
				if(!IsUserDefine(BarLinkUserDefines.EditWidth)) this.userEditWidth = 0;
				OnLinkChanged();
			}
		}
		public virtual int GetLinkHorzIndent() {
			if(Bar != null) return Bar.GetLinkHorzIndent();
			if(Manager != null) return Manager.DrawParameters.Constants.BarItemHorzIndent;
			return -1;
		}
		public virtual int GetLinkVertIndent() {
			if(Bar != null) return Bar.GetLinkVertIndent();
			if(Manager != null) return Manager.DrawParameters.Constants.BarItemVertIndent;
			return -1;
		}
		[Browsable(false)]
		public virtual bool IsImageExist {
			get {
				if(Manager != null && Manager.LargeIcons && IsLargeImageExist)
					return true;
				return Glyph != null || (Item != null && (ImageCollection.IsImageListImageExists(Item.Images, ImageIndex))) || (Item != null && Item.ImageUri != null && Item.ImageUri.HasImage);
			}
		}
		[Browsable(false)]
		public virtual bool CanSelectInCustomization { 
			get { 
				if(Manager != null && !Manager.IsDesignMode) {
					if((Bar != null && Bar.OptionsBar.DisableCustomization) || 
						Item.Visibility == BarItemVisibility.OnlyInRuntime) return false;
				}
				return !Item.IsPrivateItem && Manager == Item.Manager; 
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool CanResize { get { return false; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool CanDrag { get { return !Item.IsPrivateItem; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BarItemLinkReadOnlyCollection Links { get { return links; } }
		[Browsable(false)]
		public BarShortcut ItemShortcut { get { return Item.ItemShortcut; } }
		[Browsable(false)]
		public string ShortCutDisplayText { 
			get {
				if(!string.IsNullOrEmpty(Item.ShortcutKeyDisplayString))
					return Item.ShortcutKeyDisplayString;
				if(ItemShortcut != null && ItemShortcut != BarShortcut.Empty)
				return ItemShortcut.ToString(); 
				return null;
			} 
		}
		[Browsable(false)]
		public BarItem Item { get { return item; } }
		[Browsable(false)]
		public BarManager Manager { 
			get { 
				if(manager != null) return manager;
				RibbonControl r = GetRibbonCore();
				if(r != null) return r.Manager;
				if(Holder is BaseRibbonItemLinkCollection) return Holder.Manager;
				if(BarControl != null && BarControl.Manager != null) return BarControl.Manager;
				if(Item == null) return null;
				return Item.Manager;
			}
			internal set {
				manager = value;
			}
		}
		[Browsable(false)]
		public BarItemPaintStyle PaintStyle { 
			get { 
				if(IsUserDefine(BarLinkUserDefines.PaintStyle) || Item == null) return UserPaintStyle;
				return Item.PaintStyle; 
			} 
		}
		[Browsable(false)]
		public virtual bool IsLinkInMenu {
			get {
				if(LinkedObject is DevExpress.XtraBars.Customization.CustomizationForm) return true;
				if(BarControl == null) return false;
				if(BarControl.IsSubMenu) return true; 
				return false;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemLinkWidth"),
#endif
 XtraSerializableProperty()]
		public virtual int Width {
			get {
				if(IsUserDefine(BarLinkUserDefines.Width) && !Manager.IsDesignMode) return UserWidth;
				return Item.Width;
			}
			set {
				if(Manager.IsDesignMode)
					Item.Width = value;
				else
					UserWidth = value;
			}
		}
		[Browsable(false)]
		public virtual int MinWidth {
			get { return 0; }
		}
		[Browsable(false)]
		public Image Glyph {
			get { 
				if(IsUserDefine(BarLinkUserDefines.Glyph) || Item == null) return UserGlyph;
				return Item.Glyph; 
			} 
		}
		[Browsable(false)]
		public DxImageUri ImageUri {
			get { return Item != null ? Item.ImageUri : null; }
		}
		[Browsable(false)]
		public Image LargeGlyph {
			get {
				if(IsUserDefine(BarLinkUserDefines.Glyph) || Item == null) return UserGlyph;
				return Item.LargeGlyph;
			}
		}
		[Browsable(false)]
		public Bitmap UserGlyph {
			get { return userGlyph; }
			set { 
				if(UserGlyph == value) return;
				userGlyph = value;
				this.userDefine |= BarLinkUserDefines.Glyph;
				OnLinkChanged();
			}
		}
		[Browsable(false)]
		public Font Font { get { return Item.Font; } }
		protected internal bool HasFont { get { return Item.Appearance.Options.UseFont; } }
		[Browsable(false)]
		public virtual bool Enabled { 
			get {
				if(!EnabledLinkCore)
					return false;
				return Item.Enabled;  
			} 
		}
		bool enabledLinkCore;
		internal virtual bool EnabledLinkCore { 
			get { return Holder == null || enabledLinkCore && Holder.Enabled; }
			set { enabledLinkCore = value; }
		}
		[Browsable(false)]
		public virtual bool CanPress { get { return Enabled && Item.CanPress; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Bar Bar { get { return LinkedObject as Bar; } }
		[Browsable(false)]
		public virtual BarItem OwnerItem { get { return LinkedObject as BarItem; } }
		public virtual DefaultBoolean AllowHtmlText { get { return Item.AllowHtmlText; } }
		[Browsable(false)]
		public virtual bool IsAllowHtmlText {
			get {
				if(Bar != null && (Bar.DockStyle == BarDockStyle.Left || Bar.DockStyle == BarDockStyle.Right)) return false;
				return Item.IsAllowHtmlText;
			}
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarItemLinkAccelerator")]
#endif
		public virtual Keys Accelerator {
			get {
				switch(Item.CalcRealPaintStyle(this)) {
					case BarItemPaintStyle.Caption:
					case BarItemPaintStyle.CaptionGlyph:
						return ExtractAcceleratorKey(Caption);
					case BarItemPaintStyle.CaptionInMenu:
						if(IsLinkInMenu) return ExtractAcceleratorKey(Caption);
						break;
				}
				return Keys.None;
			}
		}
		protected internal virtual bool ContainsSubItemLink(BarItemLink link, int level) { return false; }
		public bool ContainsSubItemLink(BarItemLink link) {	return ContainsSubItemLink(link, 0); }
		public virtual void Focus() {
			if(BarControl == null && Ribbon == null) return;
			Manager.SelectLink(this);
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemLinkRecentIndex"),
#endif
 XtraSerializableProperty()]
		public virtual int RecentIndex {
			get { return recentIndex; }
			set { recentIndex = value; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemLinkVisible"),
#endif
 XtraSerializableProperty()]
		public virtual bool Visible {
			get { return visible; }
			set {
				if(Visible == value) return;
				visible = value;
				OnLinkChanged();
			}
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarItemLinkCaption")]
#endif
		public virtual string Caption {
			get {
				if(IsUserDefine(BarLinkUserDefines.Caption) || Item == null) return UserCaption;
				return Item.Caption;
			}
			set { UserCaption = value; }
		}
		protected internal string Description {
			get {
				if(Item == null) return string.Empty;
				return Item.Description;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemLinkMostRecentlyUsed"),
#endif
 XtraSerializableProperty()]
		public virtual bool MostRecentlyUsed {
			get { return mostRecentlyUsed; }
			set {
				if(MostRecentlyUsed == value) return;
				mostRecentlyUsed = value;
				CheckMostRecentlyUsed();
				OnLinkChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemLinkImageIndex"),
#endif
 XtraSerializableProperty()]
		public virtual int ImageIndex {
			get { return imageIndex; }
			set {
				if(ImageIndex == value) return;
				bool prevExist = IsImageExist;
				imageIndex = value;
				if(IsLoading) return;
				if(IsImageExist == prevExist)
					Invalidate();
				else
					OnLinkChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemLinkBeginGroup"),
#endif
 XtraSerializableProperty()]
		public virtual bool BeginGroup {
			get { return beginGroup; }
			set {
				if(value == BeginGroup) return;
				beginGroup = value;
				OnLinkChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemLinkActAsButtonGroup"),
#endif
 XtraSerializableProperty(), SupportedByBarManager(SupportedByBarManagerKind.NonSupported)]
		public virtual bool ActAsButtonGroup {
			get { return actAsButtonGroup; }
			set {
				if(value == ActAsButtonGroup) return;
				actAsButtonGroup = value;
				OnLinkChanged();
			}
		}
		[Browsable(false)]
		public bool IsDefaultActAsButtonGroup { get { return ActAsButtonGroup == GetActAsButtonGroupDefault(); } }
		[Browsable(false)]
		public virtual bool GetActAsButtonGroupDefault() { return false; }
		internal void SetBeginGroup(bool val) { 
			this.beginGroup = val;
		}
		[Browsable(false)]
		public bool GetBeginGroup() { return BeginGroup || internalBeginGroup; }
		[Browsable(false)]
		public virtual string DisplayCaption { get { return Caption; }	}
		[Browsable(false)]
		public virtual string DisplayHint {
			get {
				if(!IsAllowToolTip || Item.Hint == "") return "";
				string s = Item.Hint;
				if(Manager.ShowShortcutInScreenTips && ItemShortcut.IsExist) {
					s = s + " (" + ShortCutDisplayText + ")";
				}
				return s;
			}
		}
		protected internal virtual bool IsAllowToolTip {
			get {
				if(!IsLinkInMenu) return Manager.ShowScreenTipsInToolbars;
				if(Manager.ShowScreenTipsInMenus) return true;
				if(LinkViewInfo.DrawMode == BarLinkDrawMode.InMenuGallery) return true;
				if(LinkViewInfo.DrawMode == BarLinkDrawMode.InMenuLargeWithText || LinkViewInfo.DrawMode == BarLinkDrawMode.InMenuLarge) return true;
				return false;
			}
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarItemLinkCanShowSuperTip")]
#endif
public virtual bool CanShowSuperTip { get { return true; } }
		public SuperToolTip GetSuperTip() {
			if(!IsAllowToolTip) return null;
			if(Item.SuperTip != null && !item.SuperTip.IsEmpty) return item.SuperTip;
			if(Item.Hint == "") {
				if(LinkViewInfo == null || (LinkViewInfo.DrawParts & BarLinkParts.Caption) != 0)
					return null;
			}
			SuperToolTip res = new SuperToolTip();
			SuperToolTipSetupArgs args = new SuperToolTipSetupArgs();
			args.Title.Text = DisplayCaption;
			if(args.Title.Text != string.Empty && Manager.ShowShortcutInScreenTips && ItemShortcut.IsExist) {
				args.Title.Text += " (" + ShortCutDisplayText + ")";
			}
			if(DisplayCaption.Replace("&","") != Item.Hint)
				args.Contents.Text = Item.Hint;
			res.Setup(args);
			if(Item.IsAllowHtmlText) res.AllowHtmlText = DefaultBoolean.True;
			return res;
		}
		protected internal virtual ToolTipControlInfo GetToolTipInfo(RibbonHitInfo hi, Point point) {
			ToolTipControlInfo info = new ToolTipControlInfo();
			RibbonSplitButtonItemViewInfo itemInfo = RibbonItemInfo as RibbonSplitButtonItemViewInfo;
			if(itemInfo != null && itemInfo.DropButtonBounds.Contains(point) && Item.DropDownSuperTip != null && !Item.DropDownSuperTip.IsEmpty)
				info.SuperTip = Item.DropDownSuperTip;
			else
				info.SuperTip = GetSuperTip();
			info.Text = DisplayHint;
			info.Object = this;
			info.IconType = ToolTipIconType.None;
			return info;
		}
		protected internal virtual ToolTipControlInfo GetToolTipInfo(Point point) {
			return GetToolTipInfo(null, point);
		}
		public void ShowHint() {
			if(Manager != null && (BarControl != null || Ribbon != null) && !Bounds.IsEmpty) {
				Rectangle scBounds = ScreenBounds;
				Point center = scBounds.Location;
				center.X += (scBounds.Width / 2);
				center.Y += (scBounds.Height / 2);
				Rectangle area = Screen.GetWorkingArea(center);
				if(!area.Contains(center)) return;
				Cursor.Position = center;
				Manager.GetToolTipController().ShowHint(GetToolTipInfo(center));
			}
		}
		public void HideHint() {
			if(Manager != null) Manager.GetToolTipController().HideHint();
		}
		internal bool IsDisposed { get; set; }
		bool isDisposing = false;
		public virtual void Dispose() {
			if(isDisposing) return;
			IsDisposed = true;
			isDisposing = true;
			BarManager manager = Manager;
			if(Holder != null) Holder.RemoveLink(this);
			this.linkedObject = null;
			this.ownerControl = null;
			if(this.item != null && this.item.Links != null) {
				this.item.Links.Remove(this);
			}
			this.item = null;
			if(manager != null) manager.OnLinkDelete(this);
			isDisposing = false;
		}
		public virtual void Invalidate() {
			if(RibbonItemInfo != null) {
				RibbonItemInfo.Invalidate();
				return;
			}
			CustomControl bc = BarControl;
			if(bc != null) {
				if(bc is TabFormControlBase) {
					((TabFormControlBase)bc).ForceUpdateLinkInfo(this);
					return;
				}
				bc.Invalidate(Bounds);
			}
			if(RadialMenu != null && RadialMenu.Visible)
				RadialMenu.Window.Invalidate();
		}
		protected internal RadialMenu RadialMenu { get; set; }
		public virtual Point LinkPointToScreen(Point p) {
			if(RibbonItemInfo != null) return RibbonItemInfo.PointToScreen(p);
			if(IsRadialMenuWindowCreated) {
				return new Point(p.X + RadialMenu.Window.Location.X, p.Y + RadialMenu.Window.Location.Y);
			}
			if(BarControl == null || !BarControl.IsHandleCreated) return p;
			return BarControl.PointToScreen(p);
		}
		protected bool IsRadialMenuWindowCreated {
			get {
				if(RadialMenu == null) return false;
				BarManager radialMenuManager = RadialMenu.GetManager();
				if(radialMenuManager == null || !object.ReferenceEquals(Manager, radialMenuManager))
					return false;
				if(Manager.IsDesignMode && (RadialMenu.Window == null || !RadialMenu.Window.IsCreated))
					return false;
				return true;
			}
		}
		public virtual Rectangle RectangleToScreen(Rectangle rect) {
			if(RibbonItemInfo != null) return new Rectangle(RibbonItemInfo.PointToScreen(rect.Location), rect.Size);
			if(RadialMenu != null) return new Rectangle(rect.X - RadialMenu.Window.Location.X, rect.Y - RadialMenu.Window.Location.Y, rect.Width, rect.Height);
			if(BarControl == null || !BarControl.IsHandleCreated) return rect;
			return BarControl.RectangleToScreen(rect);
		}
		public virtual Point ScreenToLinkPoint(Point p) {
			if(RibbonItemInfo != null) return RibbonItemInfo.PointToClient(p);
			if(RadialMenu != null) return new Point(p.X - RadialMenu.Window.Location.X, p.Y - RadialMenu.Window.Location.Y);
			if(BarControl == null || !BarControl.IsHandleCreated) return p;
			return BarControl.PointToClient(p);
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarItemLinkCanVisible")]
#endif
		public virtual bool CanVisible {
			get {
				if(Manager == null) return false;
				if(!Manager.IsCustomizing && !Visible) return false;
				if(Item == null) return false;
				if(Holder != null && Holder.ItemLinks.IsMergedState) {
					if(InplaceHolder != null && !InplaceHolder.ItemLinks.CanVisibleMerged(this)) return false;
					if(!Holder.ItemLinks.CanVisibleMerged(this)) return false;
				}
				if(Item.Visibility == BarItemVisibility.Never) {
					return Manager.IsDesignMode;
				}
				if(Item.Visibility == BarItemVisibility.OnlyInRuntime) {
					return Manager.IsDesignMode || !Manager.IsCustomizing;
				}
				if(Item.Visibility == BarItemVisibility.OnlyInCustomizing) {
					return Manager.IsCustomizing || Manager.IsDesignMode;
				}
				if(Item.VisibleWhenVertical || Manager.IsDesignMode) return true;
				if(Bar != null) {
					if(Bar.OptionsBar.RotateWhenVertical && (Bar.DockStyle == BarDockStyle.Left || Bar.DockStyle == BarDockStyle.Right)) return false;
				}
				return true;
			}
		}
		internal BarLinksHolder Holder {
			get {
				if(LinkedObject is BarLinksHolder) return LinkedObject as BarLinksHolder;
				if(Bar != null) return Bar;
				return null;
			}
		}
		BarLinksHolder inplaceHolder;
		internal BarLinksHolder InplaceHolder {
			get { return inplaceHolder; }
			set { inplaceHolder = value; }
		}
		BarButtonItem lastCommandOwnerItem;
		protected internal BarButtonItem LastCommandOwnerItem {
			get { return lastCommandOwnerItem; }
			set { lastCommandOwnerItem = value; }
		}
		protected virtual bool ShouldCloseAllPopups { 
			get {
				if(Manager == null)
					return false;
				return Manager.SelectionInfo.OpenedPopups.Count > 0 && Bar != null && !(BarControl is QuickCustomizationBarControl); 
			} 
		}
		protected virtual void CloseAllPopups() {
			CloseAllPopups(BarMenuCloseType.All);
		}
		protected virtual void CloseAllPopups(BarMenuCloseType closeType) {
			Manager.SelectionInfo.CloseAllPopups(false, closeType);
		}
		protected internal bool IsLoading { get { return Item == null || Item.IsLoading; } }
		protected virtual internal bool AlwaysWorking { get { return false; } }
		protected internal virtual bool IsNeedMouseCapture { get { return true; } }
		protected internal virtual bool IsInterceptKey(KeyEventArgs e) { return true; }
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarItemLinkAlignment")]
#endif
		public virtual BarItemLinkAlignment Alignment {
			get {
				if(IsUserDefine(BarLinkUserDefines.Alignment) && UserAlignment != BarItemLinkAlignment.Default)
					return UserAlignment;
				if(Item == null) return BarItemLinkAlignment.Left;
				if(Bar != null &&
					(!Bar.OptionsBar.UseWholeRow || Bar.IsFloating)) return BarItemLinkAlignment.Left;
				if(Item.Alignment != BarItemLinkAlignment.Default)
					return Item.Alignment;
				BarLinkContainerExItem linkContainer = InplaceHolder as BarLinkContainerExItem;
				if(linkContainer != null)
					return linkContainer.Alignment;
				return Item.Alignment;
			}
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarItemLinkIsVertical")]
#endif
		public bool IsVertical {
			get {
				BarLinkViewInfo info = LinkViewInfo;
				if(info == null) return false;
				return info.IsDrawVerticalRotated || (info.BarControlInfo != null && info.BarControlInfo.IsVertical);
			}
		}
		protected internal virtual BarLinkViewInfo RequestLinkViewInfo() {
			if(LinkViewInfo != null) return LinkViewInfo;
			CustomControl bc = BarControl;
			if(bc != null && bc.ViewInfo != null && !bc.ViewInfo.IsReady) {
				bc.UpdateViewInfo();
			}
			return LinkViewInfo;
		}
		BarLinkViewInfo linkViewInfo = null;
		internal void SetLinkViewInfo(BarLinkViewInfo linkInfo) { 
			linkViewInfo = linkInfo;
			UpdateAnimatedLink(linkInfo);
		} 
		internal bool ShouldUpdateAnimatedItem(BarEditLinkViewInfo linkInfo, EditorAnimationInfo animInfo) {
			return animInfo != null && linkInfo.Link.Item.EditValue == (animInfo.ViewInfo as BaseEditViewInfo).EditValue;
		}
		protected internal void UpdateAnimatedLink(BarLinkViewInfo linkInfo) {
			if(BarControl == null)
				return;
			UpdateAnimatedLink(linkInfo, (ISupportXtraAnimation)BarControl, ((CustomLinksControl)BarControl).AnimationInvoker);
		}
		protected internal virtual void UpdateAnimatedLink(BarLinkViewInfo linkInfo, ISupportXtraAnimation animationHost, CustomAnimationInvoker invoker) {
			BarEditLinkViewInfo editInfo = linkInfo as BarEditLinkViewInfo;
			if(editInfo == null || BarControl == null) return;
			IAnimatedItem animItem = editInfo.EditViewInfo as IAnimatedItem;
			if(animItem == null) return;
			EditorAnimationInfo animInfo = XtraAnimator.Current.Get(BarControl as ISupportXtraAnimation, this) as EditorAnimationInfo;
			if(ShouldUpdateAnimatedItem(editInfo, animInfo)) {
				BarAnimatedItemsHelper.UpdateAnimatedViewInfo(animInfo, editInfo.EditViewInfo);
			} else {
				if(animInfo != null) XtraAnimator.Current.Animations.Remove(animInfo);
				BarAnimatedItemsHelper.AddAnimatedItem(animationHost, invoker, editInfo, this as BarEditItemLink);
			}
		}
		protected internal virtual BarLinkViewInfo LinkViewInfo {
			get {
				if(RibbonItemInfo != null) return RibbonItemInfo.GetLinkViewInfo();
				if(linkViewInfo != null) return linkViewInfo;
				CustomControl bc = BarControl;
				if(bc != null && bc.ViewInfo != null && bc.ViewInfo.IsReady) {
					CustomControlViewInfo cvi = bc.ViewInfo as CustomControlViewInfo;
					BarLinkViewInfo vi = cvi.GetLinkViewInfo(this, LinkViewInfoRange.Current);
					return vi;
				}
				if(RadialMenu != null) {
					return RadialMenu.Window.ViewInfo.GetLinkViewInfo(this);
				}
				return null;
			}
		}
		protected internal virtual bool NeedMouseCapture { get { return true; } }
		protected internal virtual bool IsStartGroup { get { return GetBeginGroup(); } }
		protected internal virtual bool AlwaysStartNewLine { get { return false; } }
		protected internal virtual CustomControl BarControl { 
			get { 
				if(ownerControl is CustomControl)
					return (ownerControl as CustomControl);
				if(Bar != null) return Bar.BarControl;
				if(LinkedObject is BarCustomContainerItem)  {
					return (CustomControl)ownerControl;
				}
				if(LinkedObject is PopupMenu)  {
					if(ownerControl is CustomControl)
						return (CustomControl)ownerControl;
					PopupMenu pm = LinkedObject as PopupMenu;
					return pm.SubControl;
				}
				return null;
			}
		}
		protected internal virtual void Assign(LinkPersistInfo info) {
			if(info == null) return;
			this.userDefine = info.UserDefine;
			this.userCaption = info.UserCaption;
			this.userGlyph = info.UserGlyph as Bitmap;
			this.userPaintStyle = info.UserPaintStyle;
			this.userWidth = info.UserWidth;
			this.userEditWidth = info.UserEditWidth;
			this.userRibbonStyle = info.UserRibbonStyle;
			this.userAlignment = info.UserAlignment;
			this.visible = info.Visible;
			this.userKeyTip = info.UserKeyTip;
			this.beginGroup = info.BeginGroup;
			this.mostRecentlyUsed = info.MostRecentlyUsed;
		}
		protected internal virtual void ProcessKey(KeyEventArgs e) {
			if(e.KeyData == Keys.Space || e.KeyData == Keys.Enter || e.KeyData == Accelerator || (e.KeyCode == Accelerator && e.Alt)) {
				OnLinkAction(BarLinkAction.KeyClick, e);
			}
			if(IsLinkInMenu && e.KeyData == Keys.F4) {
				CustomPopupBarControl menuBarControl = BarControl as CustomPopupBarControl;
				if(menuBarControl != null && Manager != null && Manager.SelectionInfo != null)
					Manager.SelectionInfo.ClosePopup(menuBarControl);
			}
		}
		protected internal virtual void ProcessKeyUp(KeyEventArgs e) { }
		protected internal virtual bool IsNeededKey(KeyEventArgs e) {
			if(!Enabled) return false;
			if(e.KeyData == Keys.Space || e.KeyData == Keys.Enter || e.KeyData == Accelerator || e.KeyData == Keys.F4) return true;
			return false;
		}	
		public virtual void Reset() {
			UserDefine = BarLinkUserDefines.None;
			this.imageIndex = Item.ImageIndex;
			this.visible = BarItemLink.DefaultVisible;
		}
		protected internal void UpdateLink() {
			this.imageIndex = Item.ImageIndex;
			this.visible = BarItemLink.DefaultVisible;
		}
		protected internal BarLinkContainerExItem ContainerEx { get { return LinkedObject as BarLinkContainerExItem; } }
		protected internal virtual void OnLinkChanged() { LayoutChanged(); }
		protected internal virtual void LayoutChanged() {
			CheckUpdateLinkState();
			if(Item != null) Item.RaiseLinkChanged();
			if(RibbonItemInfo != null)
				RibbonItemInfo.OnItemChanged();
			else if(Ribbon != null) {
				BaseRibbonItemLinkCollection coll = Links as BaseRibbonItemLinkCollection;
				RibbonStatusBarItemLinkCollection statusBarColl = coll as RibbonStatusBarItemLinkCollection;
				if(statusBarColl != null && Ribbon.StatusBar != null)
					Ribbon.StatusBar.Refresh();
				else if(coll != null)
					Ribbon.Refresh();
			}
			if(Bar != null && Bar.BarControl == BarControl) 
				Bar.OnLinkChanged(this);
			else {
				if(BarControl != null)
					BarControl.LayoutChanged();
				else {
					if(ContainerEx != null) 
						ContainerEx.OnItemChanged(false);
				}
			}
			if(RadialMenu != null)
				RadialMenu.OnLinkChanged(this);
			UpdateVisualEffects(UpdateAction.Update);
		}
		protected internal Size CalcLinkSize(Graphics g, object sourceObject) {
			BarLinkViewInfo linkInfo = CreateViewInfo();
			return linkInfo == null? Size.Empty: linkInfo.CalcLinkSize(g, sourceObject);
		}
		protected internal virtual BarLinkViewInfo CreateViewInfo() {
			if(Manager == null) return null;
			BarLinkViewInfo vInfo = Manager.Helper.CreateLinkViewInfo(this);
			if(BarControl != null)
				vInfo.ParentViewInfo = BarControl.ViewInfo;
			return vInfo;
		}
		protected internal void OnLinkActionCore(BarLinkAction action, object actionArgs) { 
			if(action == BarLinkAction.Highlight|| action == BarLinkAction.KeyboardHighlight) {
				if(BarControl != null) BarControl.MakeLinkVisible(this);
			}
			if(Item == null) return; 
			OnLinkAction(action, actionArgs); 
		}
		protected virtual bool ActDoubleClickAsSingle { get { return true; } }
		protected virtual void OnLinkAction(BarLinkAction action, object actionArgs) {
			if(action != BarLinkAction.MouseMove) Invalidate();
			switch(action) {
				case BarLinkAction.MouseDoubleClick:
					if(ActDoubleClickAsSingle) OnLinkClick();
					if(Item == null) return; 
					OnLinkDoubleClick();
					break;
				case BarLinkAction.Press:
					OnLinkPress();
					break;
				case BarLinkAction.KeyClick:
					OnLinkClick();
					break;
				case BarLinkAction.MouseClick : OnLinkClick(); 
					break;
				case BarLinkAction.Highlight:
				case BarLinkAction.KeyboardHighlight:
					if(ShouldCloseAllPopups) CloseAllPopups(BarMenuCloseType.AllExceptMiniToolbars);
					return;
			}
		}
		protected virtual void IncClickCount() {
			clickCount ++;
		}
		protected virtual void OnLinkPress() {
			Item.OnPress(this);
		}
		protected virtual void OnLinkDoubleClick() {
			Item.OnDoubleClick(this);
		}
		ArrayList SaveDXRibbonToolbars() {
			if(Ribbon == null)
				return null;
			ArrayList res = new ArrayList();
			foreach(RibbonMiniToolbar toolbar in Ribbon.MiniToolbars) {
				if(toolbar.Tag is DXRibbonMiniToolbar)
					res.Add(toolbar);
			}
			return res;
		}
		void CloseDXRibbonToolbars(ArrayList dxRibbonToolbars) {
			foreach(RibbonMiniToolbar toolbar in dxRibbonToolbars) {
				toolbar.Dispose();
			}   
		}
		BarButtonItemLink GetHolderItemLink() {
			var popupMenu = Holder as PopupMenu;
			if(popupMenu == null) return null;
			return popupMenu.Activator as BarButtonItemLink;
		}
		protected internal virtual bool ShouldRememberLastCommand {
			get {
				var itemLink = GetHolderItemLink();
				if(itemLink == null) return false;
				return itemLink.Item.RememberLastCommand;
			}
		}
		protected virtual void MakeOwnerRememberLastCommand() {
			var itemLink = GetHolderItemLink();
			if(itemLink == null) return;
			itemLink.Item.LastClickedLink = this;
		}
		protected virtual void OnLinkClick() {
			Manager.SelectionInfo.PressedLink = this;
			if(BarControl != null) BarControl.Update();
			if(ShouldRememberLastCommand) {
				MakeOwnerRememberLastCommand();
			}
			BarManager manager = Manager;
			CustomPopupBarControl popupControl = BarControl as CustomPopupBarControl;
			CustomPopupBarControl rootPopup = Manager.SelectionInfo.OpenedPopups.RootPopup == null ? null : Manager.SelectionInfo.OpenedPopups.RootPopup as CustomPopupBarControl;
			try {
				if(rootPopup != null) rootPopup.LockCloseUp();
				if(popupControl != null) popupControl.LockCloseUp();
				Manager.SelectionInfo.LockNullPressedChanging();
				try {
					BringToTopInRecentList(true);
					BarItem item = Item;
					ArrayList dxRibbonToolbars = null;
					if(item.CanCloseSubOnClick(this)) {
						dxRibbonToolbars = SaveDXRibbonToolbars();
						manager.SelectionInfo.OnCloseAll(BarMenuCloseType.AllExceptMiniToolbarsAndDXToolbars | BarMenuCloseType.KeepPopupContainer);
					}
					item.OnClick(this);
					if(item.CanCloseSubOnClick(this) && dxRibbonToolbars != null) {
						CloseDXRibbonToolbars(dxRibbonToolbars);
					}
				}
				finally {
					manager.SelectionInfo.UnLockNullPressedChanging();
					manager.SelectionInfo.PressedLink = null;
					manager.SelectionInfo.OnLinkClicked(this);
				}
			} finally {
				if(popupControl != null) popupControl.UnLockCloseUp();
				if(rootPopup != null) rootPopup.UnLockCloseUp();
			}
			if(ClonedFromLink != null && IsRequiredRecalcWhenCloned && BarControl != null) {
				BarControl.ViewInfo.ClearReady();
				BarControl.CheckDirty();
				BarControl.Invalidate();
			}
		}
		protected virtual bool IsRequiredRecalcWhenCloned { get { return false; } }
		protected internal virtual void CheckMostRecentlyUsed() {
			if(Manager == null) return;
			if(MostRecentlyUsed) {
				clickCount = Manager.GetController().PropertiesBar.MostRecentlyUsedClickCount;
				BringToTopInRecentList(false);
			}
			else {
				clickCount = 0;
				SendToBottomInRecentList();
			}
		}
		protected internal virtual void BringToTopInRecentList(bool incClickCount) {
			if(incClickCount)
				IncClickCount();
			if(Links != null)
				Links.UpdateRecentIndex(this, 0);
			BringParentToTopInRecentList();
		}
		protected virtual void BringParentToTopInRecentList() {
			if(IsLinkInMenu) {
				SubMenuBarControl subMenu = BarControl as SubMenuBarControl;
				if(subMenu != null && subMenu.ContainerLink != null)
					subMenu.ContainerLink.BringToTopInRecentList(true);
			}
		}
		protected virtual void SendToBottomInRecentList() {
			if(Links != null)
				Links.UpdateRecentIndex(this, Links.Count);
		}
		protected virtual Keys ExtractAcceleratorKey(string text) {
			char key = (char)0;
			for(int n = 0; n < text.Length - 1; n++) {
				if(text[n] == '&') {
					if(text[n + 1] != '&') {
						key = text[n + 1];
						break;
					} else {
						n++;
					}
				}
			}
			if(key == 0) return Keys.None;
			short scan = BarNativeMethods.VkKeyScan(key);
			if(scan == -1) return Keys.None;
			return (Keys)(scan & 0xff);
		}
		void CheckUpdateLinkStateInContainer(BarManager manager) {
			foreach(Bar bar in manager.Bars) {
				if(bar.BarControl == null)
					continue;
				foreach(BarControlRowViewInfo row in bar.BarControl.ViewInfo.Rows) {
					foreach(BarLinkViewInfo lv in row.Links) {
						if(lv.Link.Item == Item) {
							lv.UpdateLinkState();
							lv.BarControl.Invalidate();
						}
					}
				}
			}
		} 
		void CheckUpdateLinkStateInContainer() {
			CheckUpdateLinkStateInContainer(Manager);
			if(Manager.MergedOwner != null)
				CheckUpdateLinkStateInContainer(Manager.MergedOwner);
		}
		protected internal virtual void CheckUpdateLinkState() {
			if(LinkViewInfo == null) { 
				if(Manager == null || Item == null) return;
				if(!(LinkedObject is BarLinkContainerExItem))
					return;
				CheckUpdateLinkStateInContainer();   
				return;
			}
			if(LinkViewInfo.UpdateLinkState()) Invalidate();
		}
		protected internal virtual int LiveLinkLevel {
			get {
				if(BarControl == null) return 0;
				IPopup popup = BarControl as IPopup;
				if(popup == null) return 0;
				return Manager.SelectionInfo.OpenedPopups.IndexOf(popup) + 1;
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public Rectangle Bounds {
			get {
				BarLinkViewInfo vi = LinkViewInfo;
				return vi == null ? Rectangle.Empty : vi.Bounds;
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public Rectangle ScreenBounds {
			get {
				Rectangle res = Bounds;
				return res.IsEmpty ? res : new Rectangle(LinkPointToScreen(res.Location), res.Size);
			}
		}
		protected internal virtual Rectangle SourceRectangle {
			get {
				Rectangle sourceRect = Rectangle.Empty;
				BarLinkViewInfo vi = LinkViewInfo;
				if(vi == null) return sourceRect;
				sourceRect = vi.Bounds;
				Point p = LinkPointToScreen(vi.Bounds.Location);
				sourceRect.Location = p;
				sourceRect = CalcLinkOwnerRect(sourceRect);
				return sourceRect;
			}
		}
		protected virtual Rectangle CalcLinkOwnerRect(Rectangle sourceRect) {
			if(IsLinkInMenu) return Rectangle.Empty;
			return Manager.DrawParameters.CalcLinkSourceRectangle(sourceRect);
		}
		protected internal virtual void OnUpdateLinkProperty(BarLinkProperty property) {
			switch(property) {
				case BarLinkProperty.Enabled:
					break;
				case BarLinkProperty.Caption:
					break;
				case BarLinkProperty.ImageIndex:
					ImageIndex = Item.ImageIndex;
					break;
				case BarLinkProperty.Width:
					if(!IsLoading && Width != Item.Width) LayoutChanged();
					break;
			}
			OnLinkPropertyChanged(property);
		}
		protected virtual void OnLinkPropertyChanged(BarLinkProperty property) {
			if(IsLoading) return;
			if(property != BarLinkProperty.Enabled)
				return;
			if(RibbonItemInfo != null)
				RibbonItemInfo.Invalidate();
			else if(IsLinkInInplaceContainer) {
				Ribbon.Refresh();
				if(Ribbon.MergeOwner != null)
					Ribbon.MergeOwner.Refresh();
			}
		}
		bool IsLinkInInplaceContainer {
			get {
				return Ribbon != null && Ribbon.AllowInplaceLinks && Holder is BarLinkContainerExItem;
			}
		}
		#region IRibbonItem Members
		void IRibbonItem.OnViewInfoCreated(RibbonItemViewInfo viewInfo) { OnRibbonViewInfoCreated(viewInfo); }
		RibbonItemViewInfo IRibbonItem.GetCachedViewInfo() { return GetCachedRibbonItemViewInfo(); }
		DevExpress.XtraEditors.Repository.RepositoryItem IRibbonItem.Edit { get { return RibbonEdit; } }
		bool IRibbonItem.IsChecked { get { return RibbonIsChecked; } }
		bool IRibbonItem.IsDroppedDown { get { return RibbonIsDroppedDown; } }
		string IRibbonItem.Text { get { return DisplayCaption; } }
		bool IRibbonItem.IsButtonGroup { get { return RibbonIsButtonGroup; } }
		bool IRibbonItem.IsLargeButton { get { return RibbonIsLargeButton; } }
		RibbonItemStyles IRibbonItem.AllowedStyles { get { return RibbonAllowedStyles; } }
		bool IRibbonItem.BeginGroup {
			get { return BeginGroup; }
			set { BeginGroup = value; }
		}
		IRibbonItem[] IRibbonItem.GetChildren() {
			return RibbonGetButtonGroupChildren();
		}
		protected virtual RibbonItemStyles RibbonAllowedStyles { 
			get {
				if(IsUserDefine(BarLinkUserDefines.RibbonStyle) && UserRibbonStyle != RibbonItemStyles.Default)
					return UserRibbonStyle;
				return Item == null ? RibbonItemStyles.All : Item.GetRibbonAllowedStyles(RibbonItemInfo); 
			} 
		}
		protected virtual DevExpress.XtraEditors.Repository.RepositoryItem RibbonEdit { get { return null; } }
		protected virtual IRibbonItem[] RibbonGetButtonGroupChildren() { return null; }
		protected virtual bool RibbonIsLargeButton { 
			get {
				if(Ribbon != null && Ribbon.IsOfficeTablet)
					return false;
				return (RibbonAllowedStyles & RibbonItemStyles.Large) != 0; 
			} 
		}
		protected virtual bool RibbonIsButtonGroup { get { return false; } }
		protected virtual bool RibbonIsDroppedDown { get { return false; } }
		protected virtual bool RibbonIsChecked { get { return false; } }
		protected virtual RibbonItemViewInfo GetCachedRibbonItemViewInfo() { return null; }
		protected virtual void OnRibbonViewInfoCreated(RibbonItemViewInfo itemInfo) { }
		#endregion
		protected virtual void OnTimerOpen() { }
		protected virtual void OnTimerClose() { }
		protected virtual IPopup TimerPopup { get { return null; } }
		protected virtual bool TimerIsOpened { get { return false; } }
		MouseTimerActionEnum mouseTimerAction = MouseTimerActionEnum.None;
		protected virtual bool CanStartTimerCore { get { return false; } }
		bool IBarLinkTimer.CanStartTimer { get { return CanStartTimerCore; } }
		bool IBarLinkTimer.CanStopTimer { get { return BarControl == null; } }
		int IBarLinkTimer.TickInterval { get { return !TimerIsOpened ? Manager.SubMenuOpenCloseInterval : Manager.SubMenuOpenCloseInterval / 2; } }
		void IBarLinkTimer.OnTimerRun() { mouseTimerAction = MouseTimerActionEnum.Open; }
		bool IBarLinkTimer.OnTimerTick(bool sameLink) { return OnTimerTickCore(sameLink); }
		protected virtual bool OnTimerTickCore(bool sameLink) {
			if(BarControl == null) return false;
			if(mouseTimerAction == MouseTimerActionEnum.Open) {
				if(sameLink) {
					OnTimerOpen();
					this.mouseTimerAction = MouseTimerActionEnum.Close;
					return true;
				}
			}
			BarItemLink highlighted = Manager.SelectionInfo.HighlightedLink;
			if(mouseTimerAction == MouseTimerActionEnum.Close) {
				Point p = BarControl.PointToClient(Cursor.Position);
				if(!Manager.SelectionInfo.CanClosePopupsByTimer(TimerPopup) || sameLink || (highlighted != null && ContainsSubItemLink(highlighted)) || (highlighted == null && ChildPopupContains(Cursor.Position))
					|| (highlighted == null && !BarControl.ClientRectangle.Contains(p))) {
					mouseTimerAction = MouseTimerActionEnum.Close;
					int pc = Manager.SelectionInfo.OpenedPopups.Count - 1;
					int ti = Manager.SelectionInfo.OpenedPopups.IndexOf(BarControl);
					if(highlighted == null && ChildPopupContains(Cursor.Position)) return false;
					if(highlighted != null && highlighted == Manager.SelectionInfo.OpenedPopups.LastPopup.OwnerLink) return true;
					if(ti < pc - 1 && !IsRibbonCustomizationMenuBarControl(Manager.SelectionInfo.OpenedPopups[ti + 2] as IPopup))
						Manager.SelectionInfo.ClosePopup(Manager.SelectionInfo.OpenedPopups[ti + 2] as IPopup);
					return true;
				}
				OnTimerClose();
				return false;
			}
			mouseTimerAction = MouseTimerActionEnum.Close;
			return true;
		}
		protected bool ChildPopupContains(Point pt) {
			foreach(IPopup p in Manager.SelectionInfo.OpenedPopups) {
				if(p != BarControl && p.Bounds.Contains(pt) && IsChildOf(p, (IPopup)BarControl))
					return true;
			}
			return false;
		}
		protected bool IsChildOf(IPopup child, IPopup parent) {
			while(child.ParentPopup != null && child.ParentPopup != parent) {
				child = child.ParentPopup;
			}
			return child.ParentPopup == parent;
		}
		protected virtual bool IsRibbonCustomizationMenuBarControl(IPopup popup) {
			RibbonBarManager ribbonManager = Manager as RibbonBarManager;
			if(ribbonManager == null) return false;
			if(ribbonManager.Ribbon == null || ribbonManager.Ribbon.CustomizationPopupMenu == null) return false;
			return ribbonManager.Ribbon.CustomizationPopupMenu.SubControl == popup;
		}
		string ISupportRibbonKeyTip.ItemCaption { get { return Caption; } }
		string ISupportRibbonKeyTip.ItemKeyTip { get { return itemKeyTip; } set { itemKeyTip = value; } }
		string ISupportRibbonKeyTip.ItemUserKeyTip { 
			get { return userKeyTip; } 
			set { 
				if(value != null) userKeyTip = value.ToUpper();
				else userKeyTip = string.Empty;
			} 
		}
		int ISupportRibbonKeyTip.FirstIndex { get { return firstIndex; } set { firstIndex = value; } }
		void ISupportRibbonKeyTip.Click() {
			KeyTipItemClick();
		}
		bool ISupportRibbonKeyTip.KeyTipEnabled { get { return Enabled; } }
		bool ISupportRibbonKeyTip.KeyTipVisible { 
			get {
				if(!KeyTipVisibleCore) return false;
				if(IsLinkInMenu) {
					return BarControl.ViewInfo.Bounds.Contains(LinkViewInfo.Bounds);
				}
				if(RibbonItemInfo == null)
					return false;
				RibbonControl rc = RibbonItemInfo.OwnerControl as RibbonControl;
				return rc == null || rc.ClientRectangle.Contains(RibbonItemInfo.Bounds);
			} 
		}
		protected virtual bool KeyTipVisibleCore {
			get { return true; }
		}
		protected virtual int GetLinkRow() {
			if(Ribbon.IsOfficeTablet)
				return 2;
			RibbonHandler rhandler = Ribbon.Handler as RibbonHandler;
			if(RibbonItemInfo.IsLargeButton) {
				if(RibbonItemInfo is RibbonSplitButtonItemViewInfo) return 0;
				else return 2;
			}
			if(RibbonItemInfo is InRibbonGalleryRibbonItemViewInfo) return 2;
			int rowPos = Ribbon.NavigatableObjects.FindNavObjectByLink(this).Y - rhandler.ItemLinksBeginRow;
			if (rhandler.ItemLinksRowCount == 1) return 1;
			if (rhandler.ItemLinksRowCount == 2 && rowPos == 1) return 2;
			return rowPos;
		}
		protected internal virtual int GetKeyTipYPos() {
			if(RibbonItemInfo != null && RibbonItemInfo.OwnerButtonGroup != null && (RibbonItemInfo.OwnerButtonGroup.Item as BarItemLink).LinkedObject is RibbonQuickToolbarItemLinkCollection)
				return KeyTipItemBounds.Y + KeyTipItemBounds.Height / 2;
			return GetKeyTipYPos(GetLinkRow());
		}
		protected internal virtual int GetKeyTipYPos(int linkRow) {
			RibbonPageGroupItemLinkCollection pgColl= LinkedObject as RibbonPageGroupItemLinkCollection;
			if(RibbonItemInfo.OwnerButtonGroup != null) pgColl = (RibbonItemInfo.OwnerButtonGroup.Item as BarButtonGroupLink).LinkedObject as RibbonPageGroupItemLinkCollection;
			if(pgColl != null) return pgColl.PageGroup.GroupInfo.GetGroupKeyTipYPos(linkRow);
			if(RibbonItemInfo.Owner is RibbonPageGroupViewInfo) return ((RibbonPageGroupViewInfo)RibbonItemInfo.Owner).GetGroupKeyTipYPos(linkRow);
			return 0;
		}
		protected virtual Rectangle KeyTipItemBounds {
			get { 
				if(LinkViewInfo == null) return Rectangle.Empty;
				InRibbonGalleryRibbonItemViewInfo gallery = RibbonItemInfo as InRibbonGalleryRibbonItemViewInfo;
				if(gallery != null) {
					if(gallery.GalleryInfo.ButtonCommandBounds.IsEmpty) gallery.GalleryInfo.LayoutButtonsBounds();
					return (RibbonItemInfo as InRibbonGalleryRibbonItemViewInfo).GalleryInfo.ButtonCommandBounds;
				}
				return LinkViewInfo.Bounds;
			}
		}
		protected internal RibbonControl Ribbon { 
			get {
				RibbonControl r = GetRibbonCore();
				if(r != null) return r;
				RibbonBarManager man = Manager as RibbonBarManager;
				if(man != null) return man.Ribbon;
				return null;
			}
		}
		RibbonControl GetRibbonCore() {
			RibbonGroupItem grItem = LinkedObject as RibbonGroupItem;
			if(grItem != null) return grItem.PageGroup.Ribbon;
			RibbonPageGroupItemLinkCollection pgColl = LinkedObject as RibbonPageGroupItemLinkCollection;
			if(RibbonItemInfo != null && RibbonItemInfo.OwnerButtonGroup != null) pgColl = (RibbonItemInfo.OwnerButtonGroup.Item as BarButtonGroupLink).LinkedObject as RibbonPageGroupItemLinkCollection;
			if(pgColl != null) return pgColl.PageGroup.Ribbon;
			RibbonQuickToolbarItemLinkCollection tColl = LinkedObject as RibbonQuickToolbarItemLinkCollection;
			if(tColl != null) return tColl.Toolbar.Ribbon;
			return null;
		}
		Point ISupportRibbonKeyTip.ShowPoint { 
			get {
				if(LinkViewInfo == null) return Point.Empty;
				Point pt = Point.Empty;
				if(LinkedObject is RibbonPageGroupItemLinkCollection || LinkedObject is BarButtonGroup) {
					pt.Y = GetKeyTipYPos();
					pt.X = KeyTipItemBounds.X + KeyTipItemBounds.Width / 2;
				}
				else if(LinkedObject is BarCustomContainerItem || LinkedObject is PopupMenu) {
					pt.X = KeyTipItemBounds.X + KeyTipItemBounds.Height / 2;
					pt.Y = KeyTipItemBounds.Y + KeyTipItemBounds.Height / 2;
				}
				else if(LinkedObject is RibbonQuickToolbarItemLinkCollection) {
					pt.X = KeyTipItemBounds.X + KeyTipItemBounds.Width / 2;
					pt.Y = KeyTipItemBounds.Y + KeyTipItemBounds.Height * 2 / 3;
				}
				if(BarControl != null) pt = BarControl.PointToScreen(pt);
				else if(Ribbon != null) pt = Ribbon.PointToScreen(pt);
				return pt;
			} 
		}
		protected virtual bool IsCommandItemCore { get { return true; } }
		bool ISupportRibbonKeyTip.IsCommandItem { get { return IsCommandItemCore; } }
		public virtual void AssignKeyTip(BarItemLink link) {
			KeyTip = link.KeyTip;
		}
		ContentAlignment ISupportRibbonKeyTip.Alignment { 
			get {
				if((LinkedObject is BarCustomContainerItem && !(LinkedObject is BarButtonGroup)) || LinkedObject is PopupMenu) return ContentAlignment.TopLeft;
				else if(LinkedObject is RibbonQuickToolbarItemLinkCollection) return ContentAlignment.TopCenter;
				else if (LinkedObject is RibbonPageGroupItemLinkCollection || LinkedObject is BarButtonGroup)
				{
					if (RibbonItemInfo != null && RibbonItemInfo.OwnerButtonGroup != null && (RibbonItemInfo.OwnerButtonGroup.Item as BarItemLink).LinkedObject is RibbonQuickToolbarItemLinkCollection)
						return ContentAlignment.TopCenter;						  
					int rowPos = GetLinkRow();
					bool isCenter = ((IRibbonItem)this).IsLargeButton;
					if(rowPos == 0) {
						if(isCenter) return ContentAlignment.BottomCenter;
						return ContentAlignment.BottomLeft;
					}
					else if(rowPos == 1) {
						if(isCenter) return ContentAlignment.MiddleCenter;
						return ContentAlignment.MiddleLeft;
					}
					if(isCenter) return ContentAlignment.TopCenter;
					return ContentAlignment.TopLeft;
				}
				return ContentAlignment.MiddleCenter; 
			} 
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemLinkKeyTip"),
#endif
 XtraSerializableProperty()]
		public virtual string KeyTip {
			get { return (this as ISupportRibbonKeyTip).ItemUserKeyTip; }
			set {
				if((this as ISupportRibbonKeyTip).ItemUserKeyTip == value) return;
				(this as ISupportRibbonKeyTip).ItemUserKeyTip = value;
				this.userDefine |= BarLinkUserDefines.KeyTip;
				OnLinkChanged();
			}
		}
		bool ISupportRibbonKeyTip.HasDropDownButton { get { return HasDropDownButtonCore; } }
		protected virtual bool HasDropDownButtonCore { get { return false; } }
		protected internal virtual void KeyTipItemClickCore() {
			OnLinkAction(BarLinkAction.KeyClick, Keys.Space);
		}
		protected internal virtual void KeyTipItemClick() {
			if(Ribbon != null) 
				Ribbon.OnBeforeKeyTipClick();
			KeyTipItemClickCore();
		}
		public void Refresh() {
			Invalidate();
			if(RibbonItemInfo != null) {
				RibbonItemInfo.Refresh();
				return;
			}
			if(LinkViewInfo != null) {
				LinkViewInfo.UpdateViewInfo();
			}
			if(BarControl != null) BarControl.Update();
		}
		protected internal virtual void UpdateOwnerAppearance() {
			if(LinkViewInfo != null)
				LinkViewInfo.UpdateOwnerAppearance();
			if(RibbonItemInfo != null)
				RibbonItemInfo.UpdatePaintAppearance();
			else {
				BarItemLinkCollection coll = Links as BarItemLinkCollection;
				if(coll == null || !(coll.Owner is BarButtonGroup))
					return;
				foreach(BarItemLink link in ((BarButtonGroup)coll.Owner).Links) {
					if(link.RibbonItemInfo == null)
						continue;
					foreach(RibbonItemViewInfo info in ((RibbonButtonGroupItemViewInfo)link.RibbonItemInfo).Items) {
						if(((BarItemLink)info.Item).Item != Item)
							continue;
						info.UpdatePaintAppearance();
						info.Invalidate();
					}
				}
			}
		}
		protected internal virtual Image GetGlyph() {
			return GetGlyph(DefaultGlyphSize);
		}
		protected internal virtual Image GetGlyph(Size imageSize, ObjectState state = ObjectState.Normal) {
			if(ImageUri == null)
				return Item.Glyph;
			if(ImageUri.HasDefaultImage)
				return ImageUri.GetDefaultImage();
			if(ImageUri != null && ImageUri.HasImage) {
				if(ImageUri.HasSvgImage) {
					return GetSvgGlyph(imageSize, state);
				}
				return ImageUri.GetImage();
			}
			return Glyph;
		}
		ObjectState TrimState(ObjectState state) {
			if((state & ObjectState.Pressed) != 0)
				return ObjectState.Pressed;
			if((state & ObjectState.Hot) != 0)
				return ObjectState.Hot;
			if((state & ObjectState.Selected) != 0)
				return ObjectState.Selected;
			return state;
		}
		Image GetSvgGlyph(Size imageSize, ObjectState state) {
			if(Manager != null) {
				var lookAndFeel = Manager.GetController().LookAndFeel;
				var skin = DevExpress.Skins.CommonSkins.GetSkin(lookAndFeel);
				if(skin.SvgPalettes == null || skin.SvgPalettes.Count == 0) return ImageUri.GetSvgImage(imageSize, null);
				SvgPalette statePalette = null;
				state = TrimState(state);
				if(state != ObjectState.Normal && skin.SvgPalettes.ContainsKey(state))
					statePalette = skin.SvgPalettes[state];
				var palette = skin.SvgPalettes[DevExpress.Utils.Drawing.ObjectState.Normal];
				return ImageUri.GetSvgImage(imageSize, statePalette != null ? statePalette.Merge(palette) : palette);
			}
			return ImageUri.GetSvgImage(imageSize, null);
		}
		protected internal virtual Image GetLargeGlyph() {
			return GetLargeGlyph(DefaultLargeGlyphSize);
		}
		protected internal virtual Image GetLargeGlyph(Size size, ObjectState state = ObjectState.Normal) {
			if(ImageUri == null)
				return Item.LargeGlyph;
			if(ImageUri.HasDefaultImage)
				return ImageUri.GetDefaultImage();
			if(ImageUri.HasSvgImage && !size.IsEmpty) {
				return GetSvgGlyph(size, state);
			}
			if(Item.ImageUri.HasLargeImage) {
				return Item.ImageUri.GetLargeImage();
			}
			return Item.LargeGlyph;
		}
		protected internal virtual Size GetLargeGlyphSize() {
			if(Item.ImageUri != null && Item.ImageUri.HasLargeImage) return Item.ImageUri.GetLargeImage().Size;
			if(Item.LargeGlyph != null) return Item.LargeGlyph.Size;
			return Size.Empty;
		}
		protected internal virtual bool HasLargeGlyph {
			get { return (Item.LargeGlyph != null) || (Item.ImageUri != null && Item.ImageUri.HasLargeImage); }
		}
		#region IToolTipLookAndFeelProvider Members
		DevExpress.LookAndFeel.UserLookAndFeel IToolTipLookAndFeelProvider.LookAndFeel {
			get {
				if(Manager != null)
					return Manager.GetController().LookAndFeel;
				return UserLookAndFeel.Default;
			}
		}
		#endregion
		protected internal virtual void OnBeforeShowHint(ToolTipControllerShowEventArgs e) {
		}
		protected internal virtual bool AllowRibbonQATMenu {
			get { return true; }
		}
		#region ISupportAdornerElementBarItemLink Members
		UpdateActionEventHandler updateVisualEffects;
		event UpdateActionEventHandler ISupportAdornerElement.Changed {
			add { updateVisualEffects += value; }
			remove { updateVisualEffects -= value; }
		}
		protected internal void UpdateVisualEffects(UpdateAction action) {			
			if(updateVisualEffects == null) return;
			UpdateActionEvendArgs e = new UpdateActionEvendArgs(action);
			updateVisualEffects(this, e);
		}
		bool ISupportAdornerElement.IsVisible {
			get {
				IVisualEffectsHolder visualHolder = Holder as IVisualEffectsHolder;
				if(visualHolder == null) return false;
				return visualHolder.VisualEffectsVisible && Visible; 
			}
		}
		ISupportAdornerUIManager ISupportAdornerElement.Owner {
			get {
				IVisualEffectsHolder visualHolder = Holder as IVisualEffectsHolder;
				if(visualHolder == null) return null;
				return visualHolder.VisualEffectsOwner;
			}
		}
		#endregion
	}
}
