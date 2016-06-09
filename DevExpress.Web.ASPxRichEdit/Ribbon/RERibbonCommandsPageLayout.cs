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

using DevExpress.Utils.Internal;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxRichEdit.Export;
using DevExpress.Web.ASPxRichEdit.Internal;
using DevExpress.Web.ASPxRichEdit.Localization;
using DevExpress.XtraRichEdit.Commands;
using System.Drawing.Printing;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.Office;
using DevExpress.Office.Design.Internal;
using System;
using System.Drawing;
namespace DevExpress.Web.ASPxRichEdit {
	public class RERPageMarginsCommand : RERDropDownCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.None; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ChangeSectionPageMargins); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ChangeSectionPageMarginsDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.PageMargins; } }
		protected override bool DefaultDropDownMode { get { return false; } }
		public RERPageMarginsCommand() 
			: base() { }
		public RERPageMarginsCommand(RibbonItemSize size)
			: base(size) { }
		protected override void FillItems() {
			Items.Add(new RERSetNormalSectionPageMarginsCommand());
			Items.Add(new RERSetNarrowSectionPageMarginsCommand());
			Items.Add(new RERSetModerateSectionPageMarginsCommand());
			Items.Add(new RERSetWideSectionPageMarginsCommand());
			Items.Add(new RERShowPageMarginsSetupFormCommand());
		}
	}
	public class RERSetNormalSectionPageMarginsCommand : RERDropDownToggleCommandBase {
		const int Left = 1700;
		const int Right = 850;
		const int Top = 1133;
		const int Bottom = 1133;
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.SetNormalSectionPageMargins; } }
		protected override string DefaultText {
			get { return ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.MarginsNormal); }
		}
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_SetNormalSectionPageMarginsDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.PageMarginsNormal; } }
		public RERSetNormalSectionPageMarginsCommand() 
			: base() { }
		public RERSetNormalSectionPageMarginsCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERSetNarrowSectionPageMarginsCommand : RERDropDownToggleCommandBase {
		const int Left = 720;
		const int Right = 720;
		const int Top = 720;
		const int Bottom = 720;
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.SetNarrowSectionPageMargins; } }
		protected override string DefaultText {
			get { return ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.MarginsNarrow); }
		}
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_SetNarrowSectionPageMarginsDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.PageMarginsNarrow; } }
		public RERSetNarrowSectionPageMarginsCommand() 
			: base() { }
		public RERSetNarrowSectionPageMarginsCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERSetModerateSectionPageMarginsCommand : RERDropDownToggleCommandBase {
		const int Left = 1080;
		const int Right = 1080;
		const int Top = 1440;
		const int Bottom = 1440;
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.SetModerateSectionPageMargins; } }
		protected override string DefaultText {
			get { return ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.MarginsModerate); }
		}
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_SetModerateSectionPageMarginsDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.PageMarginsModerate; } }
		public RERSetModerateSectionPageMarginsCommand() 
			: base() { }
		public RERSetModerateSectionPageMarginsCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERSetWideSectionPageMarginsCommand : RERDropDownToggleCommandBase {
		const int Left = 2880;
		const int Right = 2880;
		const int Top = 1440;
		const int Bottom = 1440;
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.SetWideSectionPageMargins; } }
		protected override string DefaultText {
			get { return ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.MarginsWide); }
		}
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_SetWideSectionPageMarginsDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.PageMarginsWide; } }
		public RERSetWideSectionPageMarginsCommand() 
			: base() { }
		public RERSetWideSectionPageMarginsCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERShowPageMarginsSetupFormCommand : RERDropDownCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ShowPageMarginsSetupForm; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ShowPageMarginsSetupForm); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ShowPageMarginsSetupFormDescription); } }
		public RERShowPageMarginsSetupFormCommand()
			: base() { BeginGroup = true; }
		public RERShowPageMarginsSetupFormCommand(RibbonItemSize size)
			: base(size) { BeginGroup = true; }
	}
	public class RERChangeSectionPageOrientationCommand : RERDropDownCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.None; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ChangeSectionPageOrientation); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ChangeSectionPageOrientationDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.PageOrientation; } }
		protected override bool DefaultDropDownMode { get { return false; } }
		public RERChangeSectionPageOrientationCommand() 
			: base() { }
		public RERChangeSectionPageOrientationCommand(RibbonItemSize size)
			: base(size) { }
		protected override void FillItems() {
			Items.Add(new RERSetPortraitPageOrientationCommand());
			Items.Add(new RERSetLandscapePageOrientationCommand());
		}
	}
	public class RERSetPortraitPageOrientationCommand : RERDropDownToggleCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.SetPortraitPageOrientation; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_SetPortraitPageOrientation); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_SetPortraitPageOrientationDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.PageOrientationPortrait; } }
		public RERSetPortraitPageOrientationCommand() 
			: base() { }
		public RERSetPortraitPageOrientationCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERSetLandscapePageOrientationCommand : RERDropDownToggleCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.SetLandscapePageOrientation; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_SetLandscapePageOrientation); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_SetLandscapePageOrientationDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.PageOrientationLandscape; } }
		public RERSetLandscapePageOrientationCommand() 
			: base() { }
		public RERSetLandscapePageOrientationCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERChangeSectionPaperKindCommand : RERDropDownCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.None; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ChangeSectionPagePaperKind); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ChangeSectionPagePaperKindDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.PaperSize; } }
		protected override bool DefaultDropDownMode { get { return false; } }
		public RERChangeSectionPaperKindCommand() 
			: base() { }
		public RERChangeSectionPaperKindCommand(RibbonItemSize size)
			: base(size) { }
		protected override void FillItems() {
			Items.Add(new RERSectionLetterPaperKind());
			Items.Add(new RERSectionLegalPaperKind());
			Items.Add(new RERSectionFolioPaperKind());
			Items.Add(new RERSectionA4PaperKind());
			Items.Add(new RERSectionB5PaperKind());
			Items.Add(new RERSectionExecutivePaperKind());
			Items.Add(new RERSectionA5PaperKind());
			Items.Add(new RERSectionA6PaperKind());
			Items.Add(new RERShowPagePaperSetupFormCommand());
		}
	}
	public class RERSectionLetterPaperKind : RERDropDownToggleCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.SetSectionLetterPaperKind; } }
		protected override string DefaultText { get { return RichEditLocalization.GetPaperKindDisplayName(PaperKind.Letter); } }
		public RERSectionLetterPaperKind() 
			: base() { }
		public RERSectionLetterPaperKind(RibbonItemSize size)
			: base(size) { }
	}
	public class RERSectionLegalPaperKind : RERDropDownToggleCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.SetSectionLegalPaperKind; } }
		protected override string DefaultText { get { return RichEditLocalization.GetPaperKindDisplayName(PaperKind.Legal); } }
		public RERSectionLegalPaperKind() 
			: base() { }
		public RERSectionLegalPaperKind(RibbonItemSize size)
			: base(size) { }
	}
	public class RERSectionFolioPaperKind : RERDropDownToggleCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.SetSectionFolioPaperKind; } }
		protected override string DefaultText { get { return RichEditLocalization.GetPaperKindDisplayName(PaperKind.Folio); } }
		public RERSectionFolioPaperKind() 
			: base() { }
		public RERSectionFolioPaperKind(RibbonItemSize size)
			: base(size) { }
	}
	public class RERSectionA4PaperKind : RERDropDownToggleCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.SetSectionA4PaperKind; } }
		protected override string DefaultText { get { return RichEditLocalization.GetPaperKindDisplayName(PaperKind.A4); } }
		public RERSectionA4PaperKind() 
			: base() { }
		public RERSectionA4PaperKind(RibbonItemSize size)
			: base(size) { }
	}
	public class RERSectionB5PaperKind : RERDropDownToggleCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.SetSectionB5PaperKind; } }
		protected override string DefaultText { get { return RichEditLocalization.GetPaperKindDisplayName(PaperKind.B5); } }
		public RERSectionB5PaperKind() 
			: base() { }
		public RERSectionB5PaperKind(RibbonItemSize size)
			: base(size) { }
	}
	public class RERSectionExecutivePaperKind : RERDropDownToggleCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.SetSectionExecutivePaperKind; } }
		protected override string DefaultText { get { return RichEditLocalization.GetPaperKindDisplayName(PaperKind.Executive); } }
		public RERSectionExecutivePaperKind() 
			: base() { }
		public RERSectionExecutivePaperKind(RibbonItemSize size)
			: base(size) { }
	}
	public class RERSectionA5PaperKind : RERDropDownToggleCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.SetSectionA5PaperKind; } }
		protected override string DefaultText { get { return RichEditLocalization.GetPaperKindDisplayName(PaperKind.A5); } }
		public RERSectionA5PaperKind() 
			: base() { }
		public RERSectionA5PaperKind(RibbonItemSize size)
			: base(size) { }
	}
	public class RERSectionA6PaperKind : RERDropDownToggleCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.SetSectionA6PaperKind; } }
		protected override string DefaultText { get { return RichEditLocalization.GetPaperKindDisplayName(PaperKind.A6); } }
		public RERSectionA6PaperKind() 
			: base() { }
		public RERSectionA6PaperKind(RibbonItemSize size)
			: base(size) { }
	}
	public class RERShowPagePaperSetupFormCommand : RERDropDownCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ShowPagePaperSetupForm; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ShowPagePaperSetupForm); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ShowPagePaperSetupFormDescription); } }
		public RERShowPagePaperSetupFormCommand()
			: base() { BeginGroup = true; }
		public RERShowPagePaperSetupFormCommand(RibbonItemSize size)
			: base(size) { BeginGroup = true; }
	}
	public class RERSetSectionColumnsCommand : RERDropDownCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.None; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_SetSectionColumns); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_SetSectionColumnsDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.ColumnsTwo; } }
		protected override bool DefaultDropDownMode { get { return false; } }
		public RERSetSectionColumnsCommand() 
			: base() { }
		public RERSetSectionColumnsCommand(RibbonItemSize size)
			: base(size) { }
		protected override void FillItems() {
			Items.Add(new RERSetSectionOneColumnCommand());
			Items.Add(new RERSetSectionTwoColumnsCommand());
			Items.Add(new RERSetSectionThreeColumnsCommand());
			Items.Add(new RERShowColumnsSetupFormCommand());
		}
	}
	public class RERSetSectionOneColumnCommand : RERDropDownToggleCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.SetSectionOneColumn; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_SetSectionOneColumn); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_SetSectionOneColumnDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.ColumnsOne; } }
		public RERSetSectionOneColumnCommand() 
			: base() { }
		public RERSetSectionOneColumnCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERSetSectionTwoColumnsCommand : RERDropDownToggleCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.SetSectionTwoColumns; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_SetSectionTwoColumns); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_SetSectionTwoColumnsDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.ColumnsTwo; } }
		public RERSetSectionTwoColumnsCommand() 
			: base() { }
		public RERSetSectionTwoColumnsCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERSetSectionThreeColumnsCommand : RERDropDownToggleCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.SetSectionThreeColumns; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_SetSectionThreeColumns); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_SetSectionThreeColumnsDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.ColumnsThree; } }
		public RERSetSectionThreeColumnsCommand() 
			: base() { }
		public RERSetSectionThreeColumnsCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERShowColumnsSetupFormCommand : RERDropDownCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ShowColumnsSetupForm; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ShowColumnsSetupForm); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ShowColumnsSetupFormDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.Columns; } }
		public RERShowColumnsSetupFormCommand()
			: base() { BeginGroup = true; }
		public RERShowColumnsSetupFormCommand(RibbonItemSize size)
			: base(size) { BeginGroup = true; }
	}
	public class RERInsertBreakCommand : RERDropDownCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.None; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_InsertBreak); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_InsertBreakDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.InsertPageBreak; } }
		protected override bool DefaultDropDownMode { get { return false; } }
		public RERInsertBreakCommand() 
			: base() { }
		public RERInsertBreakCommand(RibbonItemSize size)
			: base(size) { }
		protected override void FillItems() {
			Items.Add(new RERInsertPageBreak2Command());
			Items.Add(new RERInsertColumnBreakCommand());
			Items.Add(new RERInsertSectionBreakNextPageCommand());
			Items.Add(new RERInsertSectionBreakEvenPageCommand());
			Items.Add(new RERInsertSectionBreakOddPageCommand());
		}
	}
	public class RERInsertPageBreak2Command : RERDropDownCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.InsertPageBreak; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_InsertPageBreak); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_InsertPageBreakDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.InsertPageBreak; } }
		public RERInsertPageBreak2Command() 
			: base() { }
		public RERInsertPageBreak2Command(RibbonItemSize size)
			: base(size) { }
	}
	public class RERInsertColumnBreakCommand : RERDropDownCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.InsertColumnBreak; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_InsertColumnBreak); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_InsertColumnBreakDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.InsertColumnBreak; } }
		public RERInsertColumnBreakCommand() 
			: base() { }
		public RERInsertColumnBreakCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERInsertSectionBreakNextPageCommand : RERDropDownCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.InsertSectionBreakNextPage; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_InsertSectionBreakNextPage); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_InsertSectionBreakNextPageDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.InsertSectionBreakNextPage; } }
		public RERInsertSectionBreakNextPageCommand() 
			: base() { }
		public RERInsertSectionBreakNextPageCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERInsertSectionBreakEvenPageCommand : RERDropDownCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.InsertSectionBreakEvenPage; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_InsertSectionBreakEvenPage); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_InsertSectionBreakEvenPageDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.InsertSectionBreakEvenPage; } }
		public RERInsertSectionBreakEvenPageCommand() 
			: base() { }
		public RERInsertSectionBreakEvenPageCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERInsertSectionBreakOddPageCommand : RERDropDownCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.InsertSectionBreakOddPage; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_InsertSectionBreakOddPage); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_InsertSectionBreakOddPageDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.InsertSectionBreakOddPage; } }
		public RERInsertSectionBreakOddPageCommand() 
			: base() { }
		public RERInsertSectionBreakOddPageCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERChangePageColorCommand : RERColorCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ChangePageColor; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ChangePageColor); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ChangePageColorDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.PageColor; } }
		protected override string DefaultAutomaticColorItemCaption { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_NoColor); } }
		protected override Color DefaultAutomaticColor { get { return Color.Transparent; } }
		protected override string DefaultAutomaticColorItemValue { get { return Color.Transparent.ToArgb().ToString(); } }
		public RERChangePageColorCommand() 
			: base() { }
		public RERChangePageColorCommand(RibbonItemSize size)
			: base(size) { }
	}
}
