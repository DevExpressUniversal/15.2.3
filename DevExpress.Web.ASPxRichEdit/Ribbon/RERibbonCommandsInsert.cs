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

using DevExpress.Web.ASPxRichEdit.Internal;
using DevExpress.Web.Internal;
using DevExpress.Web.Office.Internal;
using DevExpress.XtraRichEdit.Localization;
using System.Web.UI;
namespace DevExpress.Web.ASPxRichEdit {
	public class RERInsertPageBreakCommand : RERButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.InsertPageBreak; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_InsertPageBreak2); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_InsertPageBreak2Description); } }
		protected override string ImageName { get { return RichEditRibbonImages.InsertPageBreak; } }
		public RERInsertPageBreakCommand() 
			: base() { }
		public RERInsertPageBreakCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERInsertTableCommand : RERDropDownCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ShowInsertTableForm; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_InsertTable); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_InsertTableDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.InsertTable; } }
		public RERInsertTableCommand()
			: base() { }
		public RERInsertTableCommand(RibbonItemSize size)
			: base(size) { }
		protected override void FillItems() {
			Items.Add(new RERInsertTableByGridHighlightingCommand());
		}
	}
	public class RERInsertTableByGridHighlightingCommand : RERDropDownCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.InsertTableCore; } }
		public RERInsertTableByGridHighlightingCommand()
			: base() {
			Template = new InsertTableTemplate(this, GetName());
		}
	}
	public class RERInsertPictureCommand : RERButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.InsertPicture; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_InsertPicture); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_InsertPictureDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.InsertImage; } }
		public RERInsertPictureCommand() 
			: base() { }
		public RERInsertPictureCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERShowBookmarksFormCommand : RERButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ShowBookmarkForm; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ShowBookmarkForm); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ShowBookmarkFormDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.Bookmark; } }
		public RERShowBookmarksFormCommand()
			: base() { }
		public RERShowBookmarksFormCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERShowHyperlinkFormCommand : RERButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ShowHyperlinkForm; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ShowHyperlinkForm); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ShowHyperlinkFormDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.Hyperlink; } }
		public RERShowHyperlinkFormCommand()
			: base() { }
		public RERShowHyperlinkFormCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class REREditPageHeaderCommand : RERButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.InsertHeader; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_EditPageHeader); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_EditPageHeaderDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.Header; } }
		public REREditPageHeaderCommand()
			: base() { }
		public REREditPageHeaderCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class REREditPageFooterCommand : RERButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.InsertFooter; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_EditPageFooter); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_EditPageFooterDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.Footer; } }
		public REREditPageFooterCommand()
			: base() { }
		public REREditPageFooterCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERInsertPageNumberFieldCommand : RERButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.InsertPageNumberField; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_InsertPageNumberField); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_InsertPageNumberFieldDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.InsertPageNumber; } }
		public RERInsertPageNumberFieldCommand()
			: base() { }
		public RERInsertPageNumberFieldCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERInsertPageCountFieldCommand : RERButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.InsertPageCountField; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_InsertPageCountField); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_InsertPageCountFieldDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.InsertPageCount; } }
		public RERInsertPageCountFieldCommand()
			: base() { }
		public RERInsertPageCountFieldCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERShowSymbolFormCommand : RERButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ShowSymbolForm; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_InsertSymbol); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_InsertSymbolDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.Symbol; } }
		public RERShowSymbolFormCommand()
			: base() { }
		public RERShowSymbolFormCommand(RibbonItemSize size)
			: base(size) { }
	}
}
