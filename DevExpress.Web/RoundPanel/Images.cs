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

using System.ComponentModel;
using System.Collections.Generic;
using System.Web.UI;
using DevExpress.Web;
using DevExpress.Web.Internal;
using System.Web.UI.WebControls;
using DevExpress.Web.Localization;
namespace DevExpress.Web {
	public class CollapseButtonImageSpriteProperties : ImageSpriteProperties {
		[
#if !SL
	DevExpressWebLocalizedDescription("CollapseButtonImageSpritePropertiesHottrackedCssClass"),
#endif
		DefaultValue(""), NotifyParentProperty(true), AutoFormatEnable]
		public new string HottrackedCssClass {
			get { return base.HottrackedCssClass; }
			set { base.HottrackedCssClass = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CollapseButtonImageSpritePropertiesHottrackedLeft"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public new Unit HottrackedLeft {
			get { return base.HottrackedLeft; }
			set { base.HottrackedLeft = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CollapseButtonImageSpritePropertiesHottrackedTop"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public new Unit HottrackedTop {
			get { return base.HottrackedTop; }
			set { base.HottrackedTop = value; }
		}
		public CollapseButtonImageSpriteProperties()
			: base() {
		}
		public CollapseButtonImageSpriteProperties(IPropertiesOwner owner)
			: base(owner) {
		}
	}
	public class CollapseButtonImageProperties : ImagePropertiesBase {
		[
#if !SL
	DevExpressWebLocalizedDescription("CollapseButtonImagePropertiesSpriteProperties"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true), AutoFormatEnable]
		public CollapseButtonImageSpriteProperties SpriteProperties {
			get { return (CollapseButtonImageSpriteProperties)SpritePropertiesInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CollapseButtonImagePropertiesUrlHottracked"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)),
		UrlProperty, AutoFormatEnable, AutoFormatUrlProperty]
		public new string UrlHottracked {
			get { return base.UrlHottracked; }
			set { base.UrlHottracked = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CollapseButtonImagePropertiesUrlChecked"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)),
		UrlProperty, AutoFormatUrlProperty, AutoFormatEnable]
		public new string UrlChecked {
			get { return base.UrlChecked; }
			set { base.UrlChecked = value; }
		}
		public CollapseButtonImageProperties()
			: base() {
		}
		public CollapseButtonImageProperties(string url)
			: base(url) {
		}
		public CollapseButtonImageProperties(IPropertiesOwner owner)
			: base(owner) {
		}
		protected override ImageSpriteProperties CreateSpriteProperties() {
			return new CollapseButtonImageSpriteProperties(this);
		}
	}
	public class RoundPanelImages : ImagesBase {
		public const string HeaderImageName = "rpHeader",
							CollapseButtonImageName = "rpCollapseButton",
							ExpandButtonImageName = "rpExpandButton";
		public RoundPanelImages(ISkinOwner skinOwner)
			: base(skinOwner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RoundPanelImagesHeader"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ImageProperties Header {
			get { return GetImage(HeaderImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RoundPanelImagesCollapseButton"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual CollapseButtonImageProperties CollapseButton {
			get { return (CollapseButtonImageProperties)GetImageBase(CollapseButtonImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RoundPanelImagesExpandButton"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ImageProperties ExpandButton {
			get { return (ImageProperties)GetImageBase(ExpandButtonImageName); }
		}
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			base.PopulateImageInfoList(list);
			list.Add(new ImageInfo(HeaderImageName, ImageFlags.HasNoResourceImage));
			list.Add(new ImageInfo(CollapseButtonImageName, ImageFlags.IsPng, 15, 14,
				delegate() { return ASPxperienceLocalizer.GetString(ASPxperienceStringId.RoundPanel_CollapseButton); },
				typeof(CollapseButtonImageProperties), CollapseButtonImageName));
			list.Add(new ImageInfo(ExpandButtonImageName, ImageFlags.IsPng, 15, 14,
				delegate() { return ASPxperienceLocalizer.GetString(ASPxperienceStringId.RoundPanel_CollapseButton); },
				typeof(ImageProperties), CollapseButtonImageName));
		}
	}
}
