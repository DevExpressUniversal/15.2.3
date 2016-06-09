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
using System.ComponentModel;
using System.Web.UI;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxHtmlEditor {
	public class HtmlEditorRoundPanelContent : DevExpress.Web.PanelContent {
		public HtmlEditorRoundPanelContent() { }
	}
	[ToolboxItem(false)
]
	public class ASPxHtmlEditorRoundPanel : DevExpress.Web.ASPxRoundPanel {
		public ASPxHtmlEditorRoundPanel()
			: base(null) {
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorRoundPanelShowHeader"),
#endif
DefaultValue(false)]
		public override bool ShowHeader
		{
			get { return GetBoolProperty("ShowHeader", false); }
			set
			{
				SetBoolProperty("ShowHeader", false, value);
				LayoutChanged();
			}
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			CssClass = RenderUtils.CombineCssClasses(CssClass, HtmlEditorRoundPanelStyles.ControlCssClass);
		}
		protected override StylesBase CreateStyles() {
			return new HtmlEditorRoundPanelStyles(this);
		}
		protected override ImagesBase CreateImages() {
			return new HtmlEditorRoundPanelImages(this);
		}
		protected override string GetSkinControlName() {
			return "HtmlEditor";
		}
	}
	public class HtmlEditorRoundPanelImages : RoundPanelImages {
		internal const string Category = "RoundPanel";
		public const string HEHeaderImageName = "herpHeader";
		public HtmlEditorRoundPanelImages(ISkinOwner skinOwner)
			: base(skinOwner) {
		}
		[NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatEnable]
		public override ImageProperties Header {
			get { return GetImage(HEHeaderImageName); }
		}
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			base.PopulateImageInfoList(list);
			list.Add(new ImageInfo(HEHeaderImageName, ImageFlags.HasNoResourceImage));
		}
		protected override string GetImageCategory() {
			return Category;
		}
		protected override Type GetResourceType() {
			return typeof(ASPxHtmlEditor);
		}
		protected override string GetResourceImagePath() {
			return ASPxHtmlEditor.HtmlEditorImagesResourcePath;
		}
		protected override string GetDesignTimeResourceSpriteImagePath() {
			return ASPxHtmlEditor.HtmlEditorImagesResourcePath + GetDesignTimeResourceSpriteImageDefaultName();
		}
		protected override string GetResourceSpriteCssPath() {
			return ASPxHtmlEditor.HtmlEditorSpriteCssResourceName;
		}
	}
	public class HtmlEditorRoundPanelStyles : RoundPanelStyles {
		public const string ControlStyleStyleName = "ControlStyle";
		public const string ControlCssClass = "dxheRP";
		public HtmlEditorRoundPanelStyles(ISkinOwner owner)
			: base(owner) {
		}
		[Browsable(false)]
		public new string CssFilePath {
			get { return base.CssFilePath; }
			set { base.CssFilePath = value; }
		}
		[Browsable(false)]
		public override string CssPostfix {
			get { return base.CssPostfix; }
			set { base.CssPostfix = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorRoundPanelStylesControlStyle"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public AppearanceStyleBase ControlStyle {
			get { return GetStyle(ControlStyleStyleName); }
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(ControlStyleStyleName, delegate() { return new AppearanceStyleBase(); }));
		}
	}
}
