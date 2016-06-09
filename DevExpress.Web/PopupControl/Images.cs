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
using DevExpress.Web.Localization;
namespace DevExpress.Web {
	public class HeaderButtonImageSpriteProperties : ImageSpriteProperties {
		[
#if !SL
	DevExpressWebLocalizedDescription("HeaderButtonImageSpritePropertiesHottrackedCssClass"),
#endif
		DefaultValue(""), NotifyParentProperty(true), AutoFormatEnable]
		public new string HottrackedCssClass {
			get { return base.HottrackedCssClass; }
			set { base.HottrackedCssClass = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("HeaderButtonImageSpritePropertiesHottrackedLeft"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public new Unit HottrackedLeft {
			get { return base.HottrackedLeft; }
			set { base.HottrackedLeft = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("HeaderButtonImageSpritePropertiesHottrackedTop"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public new Unit HottrackedTop {
			get { return base.HottrackedTop; }
			set { base.HottrackedTop = value; }
		}
		public HeaderButtonImageSpriteProperties()
			: base() {
		}
		public HeaderButtonImageSpriteProperties(IPropertiesOwner owner)
			: base(owner) {
		}
	}
	public class HeaderButtonImageProperties : ImagePropertiesBase {
		[
#if !SL
	DevExpressWebLocalizedDescription("HeaderButtonImagePropertiesSpriteProperties"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true), AutoFormatEnable]
		public HeaderButtonImageSpriteProperties SpriteProperties {
			get { return (HeaderButtonImageSpriteProperties)SpritePropertiesInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("HeaderButtonImagePropertiesUrlHottracked"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)),
		UrlProperty, AutoFormatEnable, AutoFormatUrlProperty]
		public new string UrlHottracked {
			get { return base.UrlHottracked; }
			set { base.UrlHottracked = value; }
		}
		public HeaderButtonImageProperties()
			: base() {
		}
		public HeaderButtonImageProperties(string url)
			: base(url) {
		}
		public HeaderButtonImageProperties(IPropertiesOwner owner)
			: base(owner) {
		}
		protected override ImageSpriteProperties CreateSpriteProperties() {
			return new HeaderButtonImageSpriteProperties(this);
		}
	}
	public class HeaderButtonCheckedImageProperties : HeaderButtonImageProperties {
		public HeaderButtonCheckedImageProperties()
			: base() {
		}
		public HeaderButtonCheckedImageProperties(string url)
			: base(url) {
		}
		public HeaderButtonCheckedImageProperties(IPropertiesOwner owner)
			: base(owner) {
		}
		protected override ImageSpriteProperties CreateSpriteProperties() {
			return new HeaderButtonImageSpriteProperties(this);
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("HeaderButtonCheckedImagePropertiesUrlChecked"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)),
		UrlProperty, AutoFormatUrlProperty, AutoFormatEnable]
		public new string UrlChecked
		{
			get { return base.UrlChecked; }
			set { base.UrlChecked = value; }
		}
	}
	public class PopupControlImages : ImagesBase {
		public const string 
			CloseButtonImageName = "pcCloseButton",
			PinButtonImageName = "pcPinButton",
			RefreshButtonImageName = "pcRefreshButton",
			CollapseButtonImageName = "pcCollapseButton",
			MaximizeButtonImageName = "pcMaximizeButton",
			SizeGripImageName = "pcSizeGrip",
			SizeGripRtlImageName = "pcSizeGripRtl",
			FooterImageName = "pcFooter",
			HeaderImageName = "pcHeader",
			ModalBackgroundImageName = "pcModalBack";
		public PopupControlImages(ASPxPopupControlBase popupControl)
			: base(popupControl) {
		}
		public PopupControlImages(ISkinOwner skinOwner)
			: base(skinOwner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupControlImagesCloseButton"),
#endif
		AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HeaderButtonImageProperties CloseButton {
			get { return (HeaderButtonImageProperties)GetImageBase(CloseButtonImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupControlImagesPinButton"),
#endif
		AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HeaderButtonCheckedImageProperties PinButton {
			get { return (HeaderButtonCheckedImageProperties)GetImageBase(PinButtonImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupControlImagesRefreshButton"),
#endif
		AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HeaderButtonImageProperties RefreshButton {
			get { return (HeaderButtonImageProperties)GetImageBase(RefreshButtonImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupControlImagesCollapseButton"),
#endif
		AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HeaderButtonCheckedImageProperties CollapseButton {
			get { return (HeaderButtonCheckedImageProperties)GetImageBase(CollapseButtonImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupControlImagesMaximizeButton"),
#endif
		AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HeaderButtonCheckedImageProperties MaximizeButton {
			get { return (HeaderButtonCheckedImageProperties)GetImageBase(MaximizeButtonImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupControlImagesSizeGrip"),
#endif
		AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties SizeGrip {
			get { return (ImageProperties)GetImageBase(SizeGripImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupControlImagesSizeGripRtl"),
#endif
		AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties SizeGripRtl {
			get { return (ImageProperties)GetImageBase(SizeGripRtlImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupControlImagesFooter"),
#endif
		AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties Footer {
			get { return (ImageProperties)GetImageBase(FooterImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupControlImagesHeader"),
#endif
		AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties Header {
			get { return (ImageProperties)GetImageBase(HeaderImageName); }
		}
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			base.PopulateImageInfoList(list);
			list.Add(new ImageInfo(CloseButtonImageName, ImageFlags.IsPng, 15, 14,
				delegate() { return ASPxperienceLocalizer.GetString(ASPxperienceStringId.PopupControl_CloseButton); },
				typeof(HeaderButtonImageProperties), CloseButtonImageName));
			list.Add(new ImageInfo(PinButtonImageName, ImageFlags.IsPng, 15, 14,
				delegate() { return ASPxperienceLocalizer.GetString(ASPxperienceStringId.PopupControl_PinButton); },
				typeof(HeaderButtonCheckedImageProperties), PinButtonImageName));
			list.Add(new ImageInfo(RefreshButtonImageName, ImageFlags.IsPng, 15, 14,
				delegate() { return ASPxperienceLocalizer.GetString(ASPxperienceStringId.PopupControl_RefreshButton); },
				typeof(HeaderButtonImageProperties), RefreshButtonImageName));
			list.Add(new ImageInfo(CollapseButtonImageName, ImageFlags.IsPng, 15, 14,
				delegate() { return ASPxperienceLocalizer.GetString(ASPxperienceStringId.PopupControl_CollapseButton); },
				typeof(HeaderButtonCheckedImageProperties), CollapseButtonImageName));
			list.Add(new ImageInfo(MaximizeButtonImageName, ImageFlags.IsPng, 15, 14,
				delegate() { return ASPxperienceLocalizer.GetString(ASPxperienceStringId.PopupControl_MaximizeButton); },
				typeof(HeaderButtonCheckedImageProperties), MaximizeButtonImageName));
			list.Add(new ImageInfo(SizeGripImageName, ImageFlags.IsPng, 16, 16,
				delegate() { return ASPxperienceLocalizer.GetString(ASPxperienceStringId.PopupControl_SizeGrip); },
				typeof(ImageProperties), SizeGripImageName));
			list.Add(new ImageInfo(SizeGripRtlImageName, ImageFlags.IsPng, 16, 16,
				delegate() { return ASPxperienceLocalizer.GetString(ASPxperienceStringId.PopupControl_SizeGrip); },
				typeof(ImageProperties), SizeGripRtlImageName));
			list.Add(new ImageInfo(FooterImageName, ImageFlags.HasNoResourceImage, typeof(ImageProperties)));
			list.Add(new ImageInfo(HeaderImageName, ImageFlags.HasNoResourceImage, typeof(ImageProperties)));
		}
	}
}
