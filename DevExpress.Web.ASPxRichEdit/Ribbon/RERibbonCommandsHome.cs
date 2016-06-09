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

using System.Collections.Generic;
using DevExpress.Utils.Internal;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxRichEdit.Export;
using DevExpress.Web.ASPxRichEdit.Internal;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.Web.ASPxRichEdit.Localization;
using System.Drawing;
namespace DevExpress.Web.ASPxRichEdit {
	#region Undo Group
	public class RERUndoCommand : RERButtonCommandBase {
		protected override bool ShowText { get { return false; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.Undo; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_Undo); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_UndoDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.Undo; } }
		public RERUndoCommand() 
			: base() { }
		public RERUndoCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERRedoCommand : RERButtonCommandBase {
		protected override bool ShowText { get { return false; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.Redo; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_Redo); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_RedoDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.Redo; } }
		public RERRedoCommand() 
			: base() { }
		public RERRedoCommand(RibbonItemSize size)
			: base(size) { }
	}
	#endregion
	#region Clipboard Group
	public class RERPasteCommand : RERButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.PasteSelection; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_Paste); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_PasteDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.Paste; } }
		public RERPasteCommand() 
			: base() { }
		public RERPasteCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERCopyCommand : RERButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.CopySelection; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_CopySelection); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_CopySelectionDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.Copy; } }
		public RERCopyCommand() 
			: base() { }
		public RERCopyCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERCutCommand : RERButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.CutSelection; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_CutSelection); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_CutSelectionDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.Cut; } }
		public RERCutCommand() 
			: base() { }
		public RERCutCommand(RibbonItemSize size)
			: base(size) { }
	}
	#endregion
	#region Font Group
	public class RERFontNameCommand : RERComboBoxCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ChangeFontName; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ChangeFontName); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ChangeFontNameDescription); } }
		public RERFontNameCommand() 
			: base() { }
		protected override ListEditItemCollection DefaultItems {
			get {
				if((base.Items == null) || (base.Items.Count == 0))
					return GetItems();
				return null;
			}
		}
		protected override int DefaultWidth { get { return 150; } }
		protected ListEditItemCollection GetItems() {
			ListEditItemCollection fontCollection = new ListEditItemCollection();
			for(int i = 0; i < WebFontInfoCache.DefaultFonts.Length; i++) {
				fontCollection.Add(WebFontInfoCache.DefaultFonts[i].Name, i);
			}
			return fontCollection;
		}
		protected override Dictionary<int, string> GetHtmlTextItemsDictionary() {
			Dictionary<int, string> result = new Dictionary<int, string>();
			foreach(ListEditItem item in Items)
				result.Add(item.Index, string.Format("<span style=\"font-family: {0};\">{1}</span>", item.Text, item.Text));
			return result;
		}
	}
	public class RERFontSizeCommand : RERComboBoxCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ChangeFontSize; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ChangeFontSize); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ChangeFontSizeDescription); } }
		protected override int DefaultWidth { get { return 60; } }
		public RERFontSizeCommand() 
			: base() { }
		protected override ListEditItemCollection DefaultItems {
			get {
				if((base.Items == null) || (base.Items.Count == 0))
					return GetItems();
				return null;
			}
		}
		protected ListEditItemCollection GetItems() {
			ListEditItemCollection fontSizeCollection = new ListEditItemCollection();
			foreach(int fontSize in FontManager.GetPredefinedFontSizes())
				fontSizeCollection.Add(fontSize.ToString());
			return fontSizeCollection;
		}
	}
	public class RERIncreaseFontSizeCommand : RERButtonCommandBase {
		protected override bool ShowText { get { return false; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.IncreaseFontSize; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_IncreaseFontSize); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_IncreaseFontSizeDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.FontSizeIncrease; } }
		protected override string GetSubGroupName() { return "Size"; }
		public RERIncreaseFontSizeCommand() 
			: base() { }
		public RERIncreaseFontSizeCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERDecreaseFontSizeCommand : RERButtonCommandBase {
		protected override bool ShowText { get { return false; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.DecreaseFontSize; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_DecreaseFontSize); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_DecreaseFontSizeDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.FontSizeDecrease; } }
		protected override string GetSubGroupName() { return "Size"; }
		public RERDecreaseFontSizeCommand() 
			: base() { }
		public RERDecreaseFontSizeCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERChangeCaseCommand : RERDropDownCommandBase {
		protected override bool ShowText { get { return false; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.None; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ChangeTextCase); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ChangeTextCaseDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.ChangeTextCase; } }
		protected override bool DefaultDropDownMode { get { return false; } }
		public RERChangeCaseCommand() 
			: base() { }
		public RERChangeCaseCommand(RibbonItemSize size)
			: base(size) { }
		protected override void FillItems() {
			Items.Add(new RERSentenceCaseCommand());
			Items.Add(new RERUpperCaseCommand());
			Items.Add(new RERLowerCaseCommand());
			Items.Add(new RERCapitalizeEachWordCommand());
			Items.Add(new RERToggleCaseCommand());
		}
	}
	public class RERSentenceCaseCommand : RERDropDownCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.SentenceCase; } }
		protected override string DefaultText { get { return ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.MenuCmd_MakeTextSentenceCase); } }
		protected override string DefaultToolTip { get { return ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.MenuCmd_MakeTextSentenceCaseDescription); } }
		public RERSentenceCaseCommand()
			: base() { }
		public RERSentenceCaseCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERUpperCaseCommand : RERDropDownCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.MakeTextUpperCase; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_MakeTextUpperCase); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_MakeTextUpperCaseDescription); } }
		public RERUpperCaseCommand() 
			: base() { }
		public RERUpperCaseCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERLowerCaseCommand : RERDropDownCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.MakeTextLowerCase; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_MakeTextLowerCase); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_MakeTextLowerCaseDescription); } }
		public RERLowerCaseCommand() 
			: base() { }
		public RERLowerCaseCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERCapitalizeEachWordCommand : RERDropDownCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.CapitalizeEachWordTextCase; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_CapitalizeEachWordTextCase); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_CapitalizeEachWordTextCaseDescription); } }
		public RERCapitalizeEachWordCommand() 
			: base() { }
		public RERCapitalizeEachWordCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERToggleCaseCommand : RERDropDownCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ToggleTextCase; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleTextCase); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleTextCaseDescription); } }
		public RERToggleCaseCommand() 
			: base() { }
		public RERToggleCaseCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERFontBoldCommand : RERToggleButtonCommandBase {
		protected override bool ShowText { get { return false; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ToggleFontBold; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleFontBold); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleFontBoldDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.Bold; } }
		protected override string GetSubGroupName() { return "Style"; }
		public RERFontBoldCommand() 
			: base() { }
		public RERFontBoldCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERFontItalicCommand : RERToggleButtonCommandBase {
		protected override bool ShowText { get { return false; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ToggleFontItalic; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleFontItalic); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleFontItalicDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.Italic; } }
		protected override string GetSubGroupName() { return "Style"; }
		public RERFontItalicCommand() 
			: base() { }
		public RERFontItalicCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERFontUnderlineCommand : RERToggleButtonCommandBase {
		protected override bool ShowText { get { return false; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ToggleFontUnderline; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleFontUnderline); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleFontUnderlineDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.Underline; } }
		protected override string GetSubGroupName() { return "Style"; }
		public RERFontUnderlineCommand() 
			: base() { }
		public RERFontUnderlineCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERFontStrikeoutCommand : RERToggleButtonCommandBase {
		protected override bool ShowText { get { return false; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ToggleFontStrikeout; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleFontStrikeout); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleFontStrikeoutDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.Strikeout; } }
		protected override string GetSubGroupName() { return "Style"; }
		public RERFontStrikeoutCommand() 
			: base() { }
		public RERFontStrikeoutCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERFontSuperscriptCommand : RERToggleButtonCommandBase {
		protected override bool ShowText { get { return false; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ToggleFontSuperscript; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_FontSuperscript); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_FontSuperscriptDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.Superscript; } }
		protected override string GetSubGroupName() { return "Script"; }
		public RERFontSuperscriptCommand() 
			: base() { }
		public RERFontSuperscriptCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERFontSubscriptCommand : RERToggleButtonCommandBase {
		protected override bool ShowText { get { return false; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ToggleFontSubscript; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_FontSubscript); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_FontSubscriptDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.Subscript; } }
		protected override string GetSubGroupName() { return "Script"; }
		public RERFontSubscriptCommand() 
			: base() { }
		public RERFontSubscriptCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERFontColorCommand : RERColorCommandBase {
		protected override bool ShowText { get { return false; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ChangeFontForeColor; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ChangeFontColor); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ChangeFontColorDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.FontColor; } }
		protected override string GetSubGroupName() { return "Color"; }
		protected override string DefaultAutomaticColorItemCaption { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_ColorAutomatic); } }
		protected override Color DefaultAutomaticColor { get { return Color.Black ; } }
		protected override string DefaultAutomaticColorItemValue { get { return ColorTranslator.ToOle(Color.Empty).ToString(); } }
		public RERFontColorCommand() 
			: base() { }
		public RERFontColorCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERFontBackColorCommand : RERColorCommandBase {
		protected override bool ShowText { get { return false; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ChangeFontBackColor; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_HighlightText); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_HighlightTextDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.Highlight; } }
		protected override string GetSubGroupName() { return "Color"; }
		protected override string DefaultAutomaticColorItemCaption { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_NoColor); } }
		protected override string DefaultAutomaticColorItemValue { get { return ColorTranslator.ToOle(Color.Transparent).ToString(); } }
	}
	public class RERClearFormattingCommand : RERButtonCommandBase {
		protected override bool ShowText { get { return false; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ClearFormatting; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ClearFormatting); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ClearFormattingDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.ClearFormatting; } }
		public RERClearFormattingCommand() 
			: base() { }
		public RERClearFormattingCommand(RibbonItemSize size)
			: base(size) { }
	}
	#endregion
	#region Paragraph Group
	public class RERBulletedListCommand : RERToggleButtonCommandBase {
		protected override bool ShowText { get { return false; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ToggleBulletedListItem; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_InsertBulletList); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_InsertBulletListDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.ListBullets; } }
		public RERBulletedListCommand()
			: base() { }
		public RERBulletedListCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERNumberingListCommand : RERToggleButtonCommandBase {
		protected override bool ShowText { get { return false; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ToggleNumberingListItem; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_InsertSimpleList); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_InsertSimpleListDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.ListNumbers; } }
		public RERNumberingListCommand()
			: base() { }
		public RERNumberingListCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERMultilevelListCommand : RERToggleButtonCommandBase {
		protected override bool ShowText { get { return false; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ToggleMultilevelListItem; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_InsertMultilevelList); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_InsertMultilevelListDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.ListMultilevel; } }
		public RERMultilevelListCommand()
			: base() { }
		public RERMultilevelListCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERDecreaseIndentCommand : RERButtonCommandBase {
		protected override bool ShowText { get { return false; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.DecreaseIndent; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_DecrementIndent); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_DecrementIndentDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.IndentDecrease; } }
		protected override string GetSubGroupName() { return "Indent"; }
		public RERDecreaseIndentCommand() 
			: base() { }
		public RERDecreaseIndentCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERIncreaseIndentCommand : RERButtonCommandBase {
		protected override bool ShowText { get { return false; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.IncreaseIndent; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_IncrementIndent); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_IncrementIndentDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.IndentIncrease; } }
		protected override string GetSubGroupName() { return "Indent"; }
		public RERIncreaseIndentCommand() 
			: base() { }
		public RERIncreaseIndentCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERShowWhitespaceCommand : RERToggleButtonCommandBase {
		protected override bool ShowText { get { return false; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ToggleShowWhitespace; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleWhitespace); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleWhitespaceDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.ShowHidden; } }
		public RERShowWhitespaceCommand()
			: base() { }
		public RERShowWhitespaceCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERAlignLeftCommand : RERToggleButtonCommandBase {
		protected override bool ShowText { get { return false; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ToggleParagraphAlignmentLeft; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ParagraphAlignmentLeft); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ParagraphAlignmentLeftDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.AlignLeft; } }
		protected override string GetSubGroupName() { return "Align"; }
		public RERAlignLeftCommand() 
			: base() { }
		public RERAlignLeftCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERAlignCenterCommand : RERToggleButtonCommandBase {
		protected override bool ShowText { get { return false; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ToggleParagraphAlignmentCenter; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ParagraphAlignmentCenter); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ParagraphAlignmentCenterDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.AlignCenter; } }
		protected override string GetSubGroupName() { return "Align"; }
		public RERAlignCenterCommand() 
			: base() { }
		public RERAlignCenterCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERAlignRightCommand : RERToggleButtonCommandBase {
		protected override bool ShowText { get { return false; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ToggleParagraphAlignmentRight; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ParagraphAlignmentRight); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ParagraphAlignmentRightDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.AlignRight; } }
		protected override string GetSubGroupName() { return "Align"; }
		public RERAlignRightCommand() 
			: base() { }
		public RERAlignRightCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERAlignJustifyCommand : RERToggleButtonCommandBase {
		protected override bool ShowText { get { return false; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ToggleParagraphAlignmentJustify; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ParagraphAlignmentJustify); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ParagraphAlignmentJustifyDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.AlignJustify; } }
		protected override string GetSubGroupName() { return "Align"; }
		public RERAlignJustifyCommand() 
			: base() { }
		public RERAlignJustifyCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERParagraphLineSpacingCommand : RERDropDownCommandBase {
		protected override bool ShowText { get { return false; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.None; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ChangeParagraphLineSpacing); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ChangeParagraphLineSpacingDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.LineSpacing; } }
		protected override bool DefaultDropDownMode { get { return false; } }
		public RERParagraphLineSpacingCommand() 
			: base() { }
		public RERParagraphLineSpacingCommand(RibbonItemSize size)
			: base(size) { }
		protected override void FillItems() {
			Items.Add(new RERSetSingleParagraphSpacingCommand());
			Items.Add(new RERSetSesquialteralParagraphSpacingCommand());
			Items.Add(new RERSetDoubleParagraphSpacingCommand());
			Items.Add(new RERAddSpacingBeforeParagraphCommand());
			Items.Add(new RERAddSpacingAfterParagraphCommand());
			Items.Add(new RERRemoveSpacingBeforeParagraphCommand());
			Items.Add(new RERRemoveSpacingAfterParagraphCommand());
		}
	}
	public class RERSetSingleParagraphSpacingCommand : RERDropDownToggleCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.SetSingleParagraphSpacing; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_SetSingleParagraphSpacing); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_SetSingleParagraphSpacingDescription); } }
		public RERSetSingleParagraphSpacingCommand() 
			: base() { }
		public RERSetSingleParagraphSpacingCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERSetSesquialteralParagraphSpacingCommand : RERDropDownToggleCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.SetSesquialteralParagraphSpacing; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_SetSesquialteralParagraphSpacing); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_SetSesquialteralParagraphSpacingDescription); } }
		public RERSetSesquialteralParagraphSpacingCommand() 
			: base() { }
		public RERSetSesquialteralParagraphSpacingCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERSetDoubleParagraphSpacingCommand : RERDropDownToggleCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.SetDoubleParagraphSpacing; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_SetDoubleParagraphSpacing); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_SetDoubleParagraphSpacingDescription); } }
		public RERSetDoubleParagraphSpacingCommand() 
			: base() { }
		public RERSetDoubleParagraphSpacingCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERAddSpacingBeforeParagraphCommand : RERDropDownCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.AddSpacingBeforeParagraph; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_AddSpacingBeforeParagraph); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_AddSpacingBeforeParagraphDescription); } }
		public RERAddSpacingBeforeParagraphCommand() 
			: base() { }
		public RERAddSpacingBeforeParagraphCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERAddSpacingAfterParagraphCommand : RERDropDownCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.AddSpacingAfterParagraph; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_AddSpacingAfterParagraph); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_AddSpacingAfterParagraphDescription); } }
		public RERAddSpacingAfterParagraphCommand() 
			: base() { }
		public RERAddSpacingAfterParagraphCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERRemoveSpacingBeforeParagraphCommand : RERDropDownCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.RemoveSpacingBeforeParagraph; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_RemoveSpacingBeforeParagraph); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_RemoveSpacingBeforeParagraphDescription); } }
		public RERRemoveSpacingBeforeParagraphCommand() 
			: base() { }
		public RERRemoveSpacingBeforeParagraphCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERRemoveSpacingAfterParagraphCommand : RERDropDownCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.RemoveSpacingAfterParagraph; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_RemoveSpacingAfterParagraph); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_RemoveSpacingAfterParagraphDescription); } }
		public RERRemoveSpacingAfterParagraphCommand() 
			: base() { }
		public RERRemoveSpacingAfterParagraphCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERParagraphBackColorCommand : RERColorCommandBase {
		protected override bool ShowText { get { return false; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ChangeParagraphBackColor; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ChangeParagraphBackColor); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ChangeParagraphBackColorDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.Shading; } }
		protected override string DefaultAutomaticColorItemCaption { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_NoColor); } }
		protected override string DefaultAutomaticColorItemValue { get { return ColorTranslator.ToOle(Color.Transparent).ToString(); } }
		public RERParagraphBackColorCommand() 
			: base() { }
		public RERParagraphBackColorCommand(RibbonItemSize size)
			: base(size) { }
	}
	#endregion
	#region Editing Group
	public class RERSelectAllCommand : RERButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.SelectAll; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_SelectAll); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_SelectAllDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.SelectAll; } }
		public RERSelectAllCommand()
			: base() { }
		public RERSelectAllCommand(RibbonItemSize size)
			: base(size) { }
	}
	#endregion
	#region Styles Group
	public class RERChangeStyleCommand : RERGalleryBarCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ChangeStyle; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ChangeStyle); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ChangeStyleDescription); } }
		public RERChangeStyleCommand()
			: base() { }
	}
	#endregion
}
