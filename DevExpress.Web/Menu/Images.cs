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
using System.Collections.Generic;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public class MenuItemImageSpriteProperties: ItemImageSpriteProperties {
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuItemImageSpritePropertiesCheckedCssClass"),
#endif
		DefaultValue(""), NotifyParentProperty(true), AutoFormatEnable]
		public new string CheckedCssClass {
			get { return base.CheckedCssClass; }
			set { base.CheckedCssClass = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuItemImageSpritePropertiesCheckedLeft"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public new Unit CheckedLeft {
			get { return base.CheckedLeft; }
			set { base.CheckedLeft = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuItemImageSpritePropertiesCheckedTop"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public new Unit CheckedTop {
			get { return base.CheckedTop; }
			set { base.CheckedTop = value; }
		}
		public MenuItemImageSpriteProperties()
			: base() {
		}
		public MenuItemImageSpriteProperties(IPropertiesOwner owner)
			: base(owner) {
		}
	}
	public class MenuItemImageProperties : ItemImagePropertiesBase {
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuItemImagePropertiesSpriteProperties"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true), AutoFormatEnable]
		public MenuItemImageSpriteProperties SpriteProperties {
			get { return (MenuItemImageSpriteProperties)SpritePropertiesInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuItemImagePropertiesUrlChecked"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)),
		UrlProperty, AutoFormatEnable, AutoFormatUrlProperty]
		public new string UrlChecked {
			get { return base.UrlChecked; }
			set { base.UrlChecked = value; }
		}
		public MenuItemImageProperties()
			: base() {
		}
		public MenuItemImageProperties(string url)
			: base(url) {
		}
		public MenuItemImageProperties(IPropertiesOwner owner)
			: base(owner) {
		}
		protected override ImageSpriteProperties CreateSpriteProperties() {
			return new MenuItemImageSpriteProperties(this);
		}
	}
	public class MenuScrollButtonImageSpriteProperties: ButtonImageSpriteProperties {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new string DisabledCssClass {
			get { return base.DisabledCssClass; }
			set { base.DisabledCssClass = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new Unit DisabledLeft {
			get { return base.DisabledLeft; }
			set { base.DisabledLeft = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new Unit DisabledTop {
			get { return base.DisabledTop; }
			set { base.DisabledTop = value; }
		}
		public MenuScrollButtonImageSpriteProperties()
			: base() {
		}
		public MenuScrollButtonImageSpriteProperties(IPropertiesOwner owner)
			: base(owner) {
		}
	}
	public class MenuScrollButtonImageProperties: ButtonImageProperties {
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuScrollButtonImagePropertiesSpriteProperties"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true), AutoFormatEnable]
		public new MenuScrollButtonImageSpriteProperties SpriteProperties {
			get { return (MenuScrollButtonImageSpriteProperties)SpritePropertiesInternal; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new string UrlDisabled {
			get { return base.UrlDisabled; }
			set { base.UrlDisabled = value; }
		}
		public MenuScrollButtonImageProperties()
			: base() {
		}
		public MenuScrollButtonImageProperties(string url)
			: base(url) {
		}
		public MenuScrollButtonImageProperties(IPropertiesOwner owner)
			: base(owner) {
		}
		protected override ImageSpriteProperties CreateSpriteProperties() {
			return new MenuScrollButtonImageSpriteProperties(this);
		}
	}
	public class MenuImages : ImagesBase {
		public const string
			AdaptiveMenuImageName = "mAdaptiveMenu",
			HorizontalPopOutImageName = "mHorizontalPopOut",
			VerticalPopOutImageName = "mVerticalPopOut",
			VerticalPopOutRtlImageName = "mVerticalPopOutRtl",
			ItemImageName = "mItem",
			SubMenuItemImageName = "mSubMenuItem",
			ScrollUpButtonImageName = "mScrollUp",
			ScrollDownButtonImageName = "mScrollDown",
			GutterImageName = "mPopupBack";
		public MenuImages(ASPxMenuBase menu)
			: base(menu) {
		}
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			base.PopulateImageInfoList(list);
			list.Add(new ImageInfo(AdaptiveMenuImageName, ImageFlags.Empty, Unit.Empty, Unit.Empty, "...",
				typeof(ItemImageProperties), AdaptiveMenuImageName));
			list.Add(new ImageInfo(HorizontalPopOutImageName, ImageFlags.Empty, Unit.Empty, Unit.Empty, "v", 
				typeof(ItemImageProperties), HorizontalPopOutImageName));
			list.Add(new ImageInfo(VerticalPopOutImageName, ImageFlags.Empty, Unit.Empty, Unit.Empty, ">",
				typeof(ItemImageProperties), VerticalPopOutImageName));
			list.Add(new ImageInfo(VerticalPopOutRtlImageName, ImageFlags.Empty, Unit.Empty, Unit.Empty, "<",
				typeof(ItemImageProperties), VerticalPopOutRtlImageName));
			list.Add(new ImageInfo(ItemImageName, ImageFlags.HasNoResourceImage, typeof(MenuItemImageProperties)));
			list.Add(new ImageInfo(SubMenuItemImageName, ImageFlags.HasCheckedState, 14,
				typeof(MenuItemImageProperties), SubMenuItemImageName));
			list.Add(new ImageInfo(ScrollUpButtonImageName, ImageFlags.Empty, Unit.Empty, Unit.Empty, "^",
				typeof(MenuScrollButtonImageProperties), ScrollUpButtonImageName));
			list.Add(new ImageInfo(ScrollDownButtonImageName, ImageFlags.Empty, Unit.Empty, Unit.Empty, "v",
				typeof(MenuScrollButtonImageProperties), ScrollDownButtonImageName));
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuImagesAdaptiveMenu"),
#endif
		AutoFormatEnable,
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ItemImageProperties AdaptiveMenu {
			get { return (ItemImageProperties)GetImageBase(AdaptiveMenuImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuImagesHorizontalPopOut"),
#endif
		AutoFormatEnable,
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ItemImageProperties HorizontalPopOut {
			get { return (ItemImageProperties)GetImageBase(HorizontalPopOutImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuImagesVerticalPopOut"),
#endif
		AutoFormatEnable,
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ItemImageProperties VerticalPopOut {
			get { return (ItemImageProperties)GetImageBase(VerticalPopOutImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuImagesVerticalPopOutRtl"),
#endif
		AutoFormatEnable,
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ItemImageProperties VerticalPopOutRtl {
			get { return (ItemImageProperties)GetImageBase(VerticalPopOutRtlImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuImagesItem"),
#endif
		AutoFormatEnable,
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public MenuItemImageProperties Item {
			get { return (MenuItemImageProperties)GetImageBase(ItemImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuImagesSubMenuItem"),
#endif
		AutoFormatEnable,
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public MenuItemImageProperties SubMenuItem {
			get { return (MenuItemImageProperties)GetImageBase(SubMenuItemImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuImagesScrollUpButton"),
#endif
		AutoFormatEnable,
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public MenuScrollButtonImageProperties ScrollUpButton {
			get { return (MenuScrollButtonImageProperties)GetImageBase(ScrollUpButtonImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuImagesScrollDownButton"),
#endif
		AutoFormatEnable,
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public MenuScrollButtonImageProperties ScrollDownButton {
			get { return (MenuScrollButtonImageProperties)GetImageBase(ScrollDownButtonImageName); }
		}
	}
}
