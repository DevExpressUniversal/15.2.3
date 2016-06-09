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

extern alias Platform;
using System;
using Platform::DevExpress.Xpf.Core.Design;
namespace DevExpress.Xpf.RichEdit.Design {
	public static partial class BarInfos {
		#region Clipboard
		public static BarInfo Clipboard { get { return clipboard; } }
		static readonly BarInfo clipboard = new BarInfo(
			String.Empty,
			"Home",
			"Clipboard",
			new BarInfoItems(
				new string[] { "EditPaste", "EditCut", "EditCopy", "EditPasteSpecial" },
				new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, }
			),
			String.Empty,
			String.Empty,
			"Caption_PageHome",
			"Caption_GroupClipboard"
		);
		#endregion
		#region Font
		static readonly BarButtonGroupItemInfo FontButtonGroup = new BarButtonGroupItemInfo(
			new BarInfoItems(
				new string[] { "FormatFontName", "FormatFontSize" },
				new BarItemInfo[] { new BarFontNameItemInfo(), new BarFontSizeItemInfo() }
			)
		);
		static readonly BarButtonGroupItemInfo FontShapeButtonGroup = new BarButtonGroupItemInfo(
			new BarInfoItems(
				new string[] { "FormatFontBold", "FormatFontItalic", "FormatFontUnderline", "FormatFontDoubleUnderline", "FormatFontStrikeout", "FormatFontDoubleStrikeout", "FormatFontSuperscript", "FormatFontSubscript", "EditChangeCase" },
				new BarItemInfo[] {
					BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check,
					new BarSubItemInfo(new BarInfoItems(
						new string[] { "EditMakeUpperCase", "EditMakeLowerCase", "EditToggleCase" },
						new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button })
					),
				}
			)
		);
		static readonly BarButtonGroupItemInfo FontSizeButtonGroup = new BarButtonGroupItemInfo(
			new BarInfoItems(
				new string[] { "FormatIncreaseFontSize", "FormatDecreaseFontSize" },
				new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button }
			)
		);
		static readonly BarButtonGroupItemInfo FontColorButtonGroup = new BarButtonGroupItemInfo(
			new BarInfoItems(
				new string[] { "FormatFontBackColor", "FormatFontForeColor" },
				new BarItemInfo[] { new BarCharactersBackgroundColorSplitButtonItemInfo(), BarItemInfos.ForeColorSplitButton }
			)
		);
		static readonly BarButtonGroupItemInfo ClearFormattingButtonGroup = new BarButtonGroupItemInfo(
			new BarInfoItems(
				new string[] { "FormatClearFormatting" },
				new BarItemInfo[] { BarItemInfos.Button }
			)
		);
		static readonly BarButtonGroupItemInfo FormatFontButtonGroup = new BarButtonGroupItemInfo(
			new BarInfoItems(
				new string[] { "FormatFont" },
				new BarItemInfo[] { BarItemInfos.Button }
			)
		);
		public static BarInfo Font { get { return font; } }
		static readonly BarInfo font = new BarInfo(
			String.Empty,
			"Home",
			"Font",
			new BarInfoItems(
				new string[] { "Font", "FontSize", "ClearFormatting", "FontShape", "FontColor", "FormatFont" },
				new BarItemInfo[] { FontButtonGroup, FontSizeButtonGroup, ClearFormattingButtonGroup, FontShapeButtonGroup, FontColorButtonGroup, FormatFontButtonGroup }
			),
			"FormatFont",
			String.Empty,
			"Caption_PageHome",
			"Caption_GroupFont"
		);
		#endregion
		#region Paragraph
		static readonly BarButtonGroupItemInfo ParagraphAlignmentButtonGroup = new BarButtonGroupItemInfo(
			new BarInfoItems(
				new string[] { "FormatParagraphAlignLeft", "FormatParagraphAlignCenter", "FormatParagraphAlignRight", "FormatParagraphAlignJustify" },
				new BarItemInfo[] { BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check }
			)
		);
		static readonly BarButtonGroupItemInfo ParagraphNumberingListButtonGroup = new BarButtonGroupItemInfo(
			new BarInfoItems(
				new string[] { "FormatBulletedList", "FormatNumberingList", "FormatMultilevelList" },
				new BarItemInfo[] { BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check }
			)
		);
		static readonly BarButtonGroupItemInfo ParagraphIndentButtonGroup = new BarButtonGroupItemInfo(
			new BarInfoItems(
				new string[] { "FormatDecreaseIndent", "FormatIncreaseIndent" },
				new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button }
			)
		);
		static readonly BarButtonGroupItemInfo ShowWhitespaceButtonGroup = new BarButtonGroupItemInfo(
			new BarInfoItems(
				new string[] { "ViewShowWhitespace" },
				new BarItemInfo[] { BarItemInfos.Check }
			)
		);
		static readonly BarButtonGroupItemInfo FormatParagraphButtonGroup = new BarButtonGroupItemInfo(
			new BarInfoItems(
				new string[] { "FormatParagraph" },
				new BarItemInfo[] { BarItemInfos.Button }
			)
		);
		static readonly BarButtonGroupItemInfo ParagraphLineSpacingButtonGroup = new BarButtonGroupItemInfo(
			new BarInfoItems(
				new string[] { "FormatParagraphLineSpacing", "FormatParagraphBackColor" },
				new BarItemInfo[] {
					new BarSubItemInfo(new BarInfoItems(
						new string[] { "FormatParagraphLineSpacingSingle", "FormatParagraphLineSpacingSesquialteral", "FormatParagraphLineSpacingDouble", "FormatParagraphLineSpacingCustomize", "FormatParagraphAddSpacingBefore", "FormatParagraphRemoveSpacingBefore", "FormatParagraphAddSpacingAfter", "FormatParagraphRemoveSpacingAfter" },
						new BarItemInfo[] { BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button })
					),
					BarItemInfos.BackColorSplitButton
				}
			)
		);
		public static BarInfo Paragraph { get { return paragraph; } }
		static readonly BarInfo paragraph = new BarInfo(
			String.Empty,
			"Home",
			"Paragraph",
			new BarInfoItems(
				new string[] { "NumberingList", "ParagraphIndent", "ShowWhitespace", "ParagraphAlignment", "LineSpacing", "Paragraph" },
				new BarItemInfo[] {
					ParagraphNumberingListButtonGroup,
					ParagraphIndentButtonGroup,
					ShowWhitespaceButtonGroup,
					ParagraphAlignmentButtonGroup,
					ParagraphLineSpacingButtonGroup,
					FormatParagraphButtonGroup
				}
			),
			"FormatParagraph",
			String.Empty,
			"Caption_PageHome",
			"Caption_GroupParagraph"
		);
		#endregion
		#region Styles
		public static BarInfo Styles { get { return styles; } }
		static readonly BarInfo styles = new CompositeBarInfo(
			String.Empty,
			"Home",
			"Styles",
			new BarInfoItems(
				new string[] { "FormatFontStyle", "FormatEditFontStyle" },
				new BarItemInfo[] { new BarStyleItemInfo(), BarItemInfos.Button }
			),
			new BarInfoItems(
				new string[] { "FormatFontStyle", "FormatEditFontStyle" },
				new BarItemInfo[] { new GalleryStyleItemInfo(), BarItemInfos.Button }
			),
			"FormatEditFontStyle",
			String.Empty,
			"Caption_PageHome",
			"Caption_GroupStyles"
		);
		#endregion
		#region Editing
		public static BarInfo Editing { get { return editing; } }
		static readonly BarInfo editing = new BarInfo(
			String.Empty,
			"Home",
			"Editing",
			new BarInfoItems(
				new string[] { "EditFind", "EditReplace" },
				new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button }
			),
			String.Empty,
			String.Empty,
			"Caption_PageHome",
			"Caption_GroupEditing"
		);
		#endregion
	}
}
