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
using DevExpress.Web.ASPxRichEdit.Export;
using DevExpress.Web.ASPxRichEdit.Internal;
using DevExpress.Web.Internal;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Localization;
namespace DevExpress.Web.ASPxRichEdit {
	public class RERGoToPageHeaderCommand : RERButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.GoToPageHeader; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_GoToPageHeader); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_GoToPageHeaderDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.GoToHeader; } }
		public RERGoToPageHeaderCommand() 
			: base() { }
		public RERGoToPageHeaderCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERGoToPageFooterCommand : RERButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.GoToPageFooter; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_GoToPageFooter); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_GoToPageFooterDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.GoToFooter; } }
		public RERGoToPageFooterCommand()
			: base() { }
		public RERGoToPageFooterCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERGoToNextPageHeaderFooterCommand : RERButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.GoToNextPageHeaderFooter; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_GoToNextHeaderFooter); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_GoToNextHeaderFooterDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.GoToNextHeaderFooter; } }
		public RERGoToNextPageHeaderFooterCommand()
			: base() { }
		public RERGoToNextPageHeaderFooterCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERGoToPreviousPageHeaderFooterCommand : RERButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.GoToPreviousPageHeaderFooter; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_GoToPreviousHeaderFooter); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_GoToPreviousHeaderFooterDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.GoToPreviousHeaderFooter; } }
		public RERGoToPreviousPageHeaderFooterCommand()
			: base() { }
		public RERGoToPreviousPageHeaderFooterCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERToggleHeaderFooterLinkToPreviousCommand : RERToggleButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.LinkHeaderFooterToPrevious; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleHeaderFooterLinkToPrevious); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleHeaderFooterLinkToPreviousDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.LinkToPrevious; } }
		public RERToggleHeaderFooterLinkToPreviousCommand()
			: base() { }
		public RERToggleHeaderFooterLinkToPreviousCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERToggleDifferentFirstPageCommand : RERToggleButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ToggleDifferentFirstPage; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleDifferentFirstPage); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleDifferentFirstPageDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.DifferentFirstPage; } }
		public RERToggleDifferentFirstPageCommand()
			: base() { }
		public RERToggleDifferentFirstPageCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERToggleDifferentOddAndEvenPagesCommand : RERToggleButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ToggleDifferentOddAndEvenPages; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleDifferentOddAndEvenPages); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleDifferentOddAndEvenPagesDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.DifferentOddEvenPages; } }
		public RERToggleDifferentOddAndEvenPagesCommand()
			: base() { }
		public RERToggleDifferentOddAndEvenPagesCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERClosePageHeaderFooterCommand : RERButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ClosePageHeaderFooter; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ClosePageHeaderFooter); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ClosePageHeaderFooterDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.CloseHeaderAndFooter; } }
		public RERClosePageHeaderFooterCommand()
			: base() { }
		public RERClosePageHeaderFooterCommand(RibbonItemSize size)
			: base(size) { }
	}
}
