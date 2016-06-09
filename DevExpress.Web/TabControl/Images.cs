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
	public class TabImageSpriteProperties: ImageSpriteProperties {
		[
#if !SL
	DevExpressWebLocalizedDescription("TabImageSpritePropertiesDisabledCssClass"),
#endif
		DefaultValue(""), NotifyParentProperty(true), AutoFormatEnable]
		public new string DisabledCssClass {
			get { return base.DisabledCssClass; }
			set { base.DisabledCssClass = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabImageSpritePropertiesDisabledLeft"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public new Unit DisabledLeft {
			get { return base.DisabledLeft; }
			set { base.DisabledLeft = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabImageSpritePropertiesDisabledTop"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public new Unit DisabledTop {
			get { return base.DisabledTop; }
			set { base.DisabledTop = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabImageSpritePropertiesHottrackedCssClass"),
#endif
		DefaultValue(""), NotifyParentProperty(true), AutoFormatEnable]
		public new string HottrackedCssClass {
			get { return base.HottrackedCssClass; }
			set { base.HottrackedCssClass = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabImageSpritePropertiesHottrackedLeft"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public new Unit HottrackedLeft {
			get { return base.HottrackedLeft; }
			set { base.HottrackedLeft = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabImageSpritePropertiesHottrackedTop"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public new Unit HottrackedTop {
			get { return base.HottrackedTop; }
			set { base.HottrackedTop = value; }
		}
		public TabImageSpriteProperties()
			: base() {
		}
		public TabImageSpriteProperties(IPropertiesOwner owner)
			: base(owner) {
		}
	}
	public class TabImageProperties: ImagePropertiesBase {
		[
#if !SL
	DevExpressWebLocalizedDescription("TabImagePropertiesSpriteProperties"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true), AutoFormatEnable]
		public TabImageSpriteProperties SpriteProperties {
			get { return (TabImageSpriteProperties)SpritePropertiesInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabImagePropertiesUrlDisabled"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)),
		UrlProperty, AutoFormatEnable, AutoFormatUrlProperty]
		public new string UrlDisabled {
			get { return base.UrlDisabled; }
			set { base.UrlDisabled = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabImagePropertiesUrlHottracked"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)),
		UrlProperty, AutoFormatEnable, AutoFormatUrlProperty]
		public new string UrlHottracked {
			get { return base.UrlHottracked; }
			set { base.UrlHottracked = value; }
		}
		public TabImageProperties()
			: base() {
		}
		public TabImageProperties(string url)
			: base(url) {
		}
		public TabImageProperties(IPropertiesOwner owner)
			: base(owner) {
		}
		protected override ImageSpriteProperties CreateSpriteProperties() {
			return new TabImageSpriteProperties(this);
		}
	}
	public class TabControlImages : ImagesBase {
		public const string ActiveTabImageName = "tcActiveTab",
							TabImageName = "tcTab",
							ScrollLeftButtonImageName = "tcScrollLeft",
							ScrollRightButtonImageName = "tcScrollRight",
							ScrollButtonBackgroundImageName = "tcScrollBtnBack",
							ScrollButtonHoverBackgroundImageName = "tcScrollBtnHoverBack",
							ScrollButtonPressedBackgroundImageName = "tcScrollBtnPressedBack",
							ScrollButtonDisableBackgroundImageName = "tcScrollBtnDisabledBack";
		public TabControlImages(ASPxTabControlBase tabControl)
			: base(tabControl) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabControlImagesActiveTab"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TabImageProperties ActiveTab {
			get { return (TabImageProperties)GetImageBase(ActiveTabImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabControlImagesTab"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TabImageProperties Tab {
			get { return (TabImageProperties)GetImageBase(TabImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabControlImagesScrollLeftButton"),
#endif
		AutoFormatEnable,
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonImageProperties ScrollLeftButton {
			get { return (ButtonImageProperties)GetImageBase(ScrollLeftButtonImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabControlImagesScrollRightButton"),
#endif
		AutoFormatEnable,
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonImageProperties ScrollRightButton {
			get { return (ButtonImageProperties)GetImageBase(ScrollRightButtonImageName); }
		}
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			base.PopulateImageInfoList(list);
			list.Add(new ImageInfo(ActiveTabImageName, ImageFlags.HasNoResourceImage, typeof(TabImageProperties)));
			list.Add(new ImageInfo(TabImageName, ImageFlags.HasNoResourceImage, typeof(TabImageProperties)));
			list.Add(new ImageInfo(ScrollLeftButtonImageName, ImageFlags.HasDisabledState | ImageFlags.HasHottrackState | ImageFlags.HasPressedState,
				Unit.Empty, Unit.Empty, "<", typeof(ButtonImageProperties), ScrollLeftButtonImageName));
			list.Add(new ImageInfo(ScrollRightButtonImageName, ImageFlags.HasDisabledState | ImageFlags.HasHottrackState | ImageFlags.HasPressedState,
				Unit.Empty, Unit.Empty, ">", typeof(ButtonImageProperties), ScrollRightButtonImageName));
		}
	}
}
