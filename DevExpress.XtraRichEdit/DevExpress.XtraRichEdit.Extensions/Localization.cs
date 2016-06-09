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
using System.Resources;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
using DevExpress.XtraRichEdit.Localization.Internal;
namespace DevExpress.XtraRichEdit.Localization {
	#region RichEditExtensionsStringId
	public enum RichEditExtensionsStringId {
		Caption_PageCategoryHeaderFooterTools,
		Caption_PageCategoryFloatingPictureTools,
		Caption_PageFile,
		Caption_PageHome,
		Caption_PageInsert,
		Caption_PageLayout,
		Caption_PageMailings,
		Caption_PageView,
		Caption_PageHeaderFooterToolsDesign,
		Caption_PagePageLayout,
		Caption_PageReview,
		Caption_PageReferences,
		Caption_PageFloatingObjectPictureToolsFormat,
		Caption_Formatting,
		Caption_GroupFont,
		Caption_GroupParagraph,
		Caption_GroupClipboard,
		Caption_GroupCommon,
		Caption_GroupEditing,
		Caption_GroupStyles,
		Caption_GroupZoom,
		Caption_GroupShow,
		Caption_GroupIllustrations,
		Caption_GroupText,
		Caption_GroupTables,
		Caption_GroupSymbols,
		Caption_GroupLinks,
		Caption_GroupPages,
		Caption_GroupHeaderFooter,
		Caption_GroupHeaderFooterToolsDesignClose,
		Caption_GroupHeaderFooterToolsDesignNavigation,
		Caption_GroupMailMerge,
		Caption_GroupDocumentViews,
		Caption_GroupHeaderFooterToolsDesignOptions,
		Caption_GroupPageSetup,
		Caption_GroupDocumentProtection,
		Caption_GroupDocumentProofing,
		Caption_GroupTableOfContents,
		Caption_GroupFloatingPictureToolsArrange,
		Caption_GroupFloatingPictureToolsShapeStyles,
		Caption_GroupCaptions,
		Caption_GroupPageBackground,
		Caption_GroupDocumentLanguage,
		Caption_GroupDocumentComment,
		Caption_GroupDocumentTracking,
		Caption_ColorAutomatic,
		Caption_NoColor,
		Caption_NoFill,
		Caption_NoOutline,
	}
	#endregion
	#region RichEditExtensionsResLocalizer
	public class RichEditExtensionsResLocalizer : XtraResXLocalizer<RichEditExtensionsStringId> {
		public RichEditExtensionsResLocalizer()
			: base(new RichEditExtensionsLocalizer()) {
		}
		protected override ResourceManager CreateResourceManagerCore() {
			return new ResourceManager("DevExpress.XtraRichEdit.LocalizationRes", typeof(RichEditExtensionsResLocalizer).Assembly);
		}
	}
	#endregion
	#region RichEditExtensionsLocalizer
	public class RichEditExtensionsLocalizer : RichEditLocalizerBase<RichEditExtensionsStringId> {
		static RichEditExtensionsLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<RichEditExtensionsStringId>(CreateDefaultLocalizer()));
		}
		#region PopulateStringTable
		protected override void PopulateStringTable() {
			AddString(RichEditExtensionsStringId.Caption_PageCategoryHeaderFooterTools, "Header & Footer Tools");
			AddString(RichEditExtensionsStringId.Caption_PageCategoryFloatingPictureTools, "Picture Tools");
			AddString(RichEditExtensionsStringId.Caption_PageFile, "File");
			AddString(RichEditExtensionsStringId.Caption_PageHome, "Home");
			AddString(RichEditExtensionsStringId.Caption_PageInsert, "Insert");
			AddString(RichEditExtensionsStringId.Caption_PageLayout, "Layout");
			AddString(RichEditExtensionsStringId.Caption_PageMailings, "Mail Merge");
			AddString(RichEditExtensionsStringId.Caption_PageView, "View");
			AddString(RichEditExtensionsStringId.Caption_Formatting, "Formatting");
			AddString(RichEditExtensionsStringId.Caption_PageHeaderFooterToolsDesign, "Design");
			AddString(RichEditExtensionsStringId.Caption_PagePageLayout, "Page Layout");
			AddString(RichEditExtensionsStringId.Caption_PageReview, "Review");
			AddString(RichEditExtensionsStringId.Caption_PageReferences, "References");
			AddString(RichEditExtensionsStringId.Caption_PageFloatingObjectPictureToolsFormat, "Format");
			AddString(RichEditExtensionsStringId.Caption_GroupFont, "Font");
			AddString(RichEditExtensionsStringId.Caption_GroupParagraph, "Paragraph");
			AddString(RichEditExtensionsStringId.Caption_GroupClipboard, "Clipboard");
			AddString(RichEditExtensionsStringId.Caption_GroupEditing, "Editing");
			AddString(RichEditExtensionsStringId.Caption_GroupCommon, "Common");
			AddString(RichEditExtensionsStringId.Caption_GroupStyles, "Styles");
			AddString(RichEditExtensionsStringId.Caption_GroupZoom, "Zoom");
			AddString(RichEditExtensionsStringId.Caption_GroupShow, "Show");
			AddString(RichEditExtensionsStringId.Caption_GroupIllustrations, "Illustrations");
			AddString(RichEditExtensionsStringId.Caption_GroupText, "Text");
			AddString(RichEditExtensionsStringId.Caption_GroupTables, "Tables");
			AddString(RichEditExtensionsStringId.Caption_GroupSymbols, "Symbols");
			AddString(RichEditExtensionsStringId.Caption_GroupLinks, "Links");
			AddString(RichEditExtensionsStringId.Caption_GroupPages, "Pages");
			AddString(RichEditExtensionsStringId.Caption_GroupHeaderFooter, "Header && Footer");
			AddString(RichEditExtensionsStringId.Caption_GroupHeaderFooterToolsDesignClose, "Close");
			AddString(RichEditExtensionsStringId.Caption_GroupHeaderFooterToolsDesignNavigation, "Navigation");
			AddString(RichEditExtensionsStringId.Caption_GroupMailMerge, "Mail Merge");
			AddString(RichEditExtensionsStringId.Caption_GroupDocumentViews, "Document Views");
			AddString(RichEditExtensionsStringId.Caption_GroupHeaderFooterToolsDesignOptions, "Options");
			AddString(RichEditExtensionsStringId.Caption_GroupPageSetup, "Page Setup");
			AddString(RichEditExtensionsStringId.Caption_GroupDocumentProtection, "Protect");
			AddString(RichEditExtensionsStringId.Caption_GroupDocumentProofing, "Proofing");
			AddString(RichEditExtensionsStringId.Caption_GroupDocumentComment, "Comment");
			AddString(RichEditExtensionsStringId.Caption_GroupDocumentTracking, "Tracking");
			AddString(RichEditExtensionsStringId.Caption_GroupTableOfContents, "Table of Contents");
			AddString(RichEditExtensionsStringId.Caption_GroupFloatingPictureToolsArrange, "Arrange");
			AddString(RichEditExtensionsStringId.Caption_GroupFloatingPictureToolsShapeStyles, "Shape Styles");
			AddString(RichEditExtensionsStringId.Caption_GroupCaptions, "Captions");
			AddString(RichEditExtensionsStringId.Caption_GroupPageBackground, "Background");
			AddString(RichEditExtensionsStringId.Caption_GroupDocumentLanguage, "Language");
			AddString(RichEditExtensionsStringId.Caption_ColorAutomatic, "Automatic");
			AddString(RichEditExtensionsStringId.Caption_NoColor, "No Color");
			AddString(RichEditExtensionsStringId.Caption_NoFill, "No Fill");
			AddString(RichEditExtensionsStringId.Caption_NoOutline, "No Outline");
		}
		#endregion
		public static XtraLocalizer<RichEditExtensionsStringId> CreateDefaultLocalizer() {
			return new RichEditExtensionsResLocalizer();
		}
		public static string GetString(RichEditExtensionsStringId id) {
			return Active.GetLocalizedString(id);
		}
		public override XtraLocalizer<RichEditExtensionsStringId> CreateResXLocalizer() {
			return new RichEditExtensionsResLocalizer();
		}
	}
	#endregion
}
