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
using DevExpress.Web.ASPxRichEdit.Localization;
using DevExpress.Web.Internal;
using DevExpress.XtraRichEdit.Localization;
namespace DevExpress.Web.ASPxRichEdit {
	public class RERCreateFieldCommand : RERButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.CreateField; } }
		protected override string DefaultText { get { return ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.MenuCmd_CreateField); } }
		protected override string DefaultToolTip { get { return ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.MenuCmd_CreateFieldDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.InsertDataField; } }
		public RERCreateFieldCommand()
			: base() { }
		public RERCreateFieldCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERInsertMergeFieldCommand : RERButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ShowInsertMergeFieldForm; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ShowInsertMergeFieldForm); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ShowInsertMergeFieldFormDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.InsertDataField; } }
		public RERInsertMergeFieldCommand()
			: base() { }
		public RERInsertMergeFieldCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERToggleViewMergedDataCommand : RERToggleButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ToggleViewMergedData; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleViewMergedData); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleViewMergedDataDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.ViewMergedData; } }
		public RERToggleViewMergedDataCommand()
			: base() { }
		public RERToggleViewMergedDataCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERToggleShowAllFieldCodesCommand : RERButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ShowAllFieldCodes; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ShowAllFieldCodes); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ShowAllFieldCodesDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.ShowAllFieldCodes; } }
		public RERToggleShowAllFieldCodesCommand()
			: base() { }
		public RERToggleShowAllFieldCodesCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERToggleShowAllFieldResultsCommand : RERButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ShowAllFieldResults; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ShowAllFieldResults); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ShowAllFieldResultsDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.ShowAllFieldResults; } }
		public RERToggleShowAllFieldResultsCommand()
			: base() { }
		public RERToggleShowAllFieldResultsCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERUpdateAllFieldsCommand : RERButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.UpdateAllFields; } }
		protected override string DefaultText { get { return ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.MenuCmd_UpdateAllFields); } }
		protected override string DefaultToolTip { get { return ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.MenuCmd_UpdateAllFieldsDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.UpdateField; } }
		public RERUpdateAllFieldsCommand()
			: base() { }
		public RERUpdateAllFieldsCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERFirstDataRecordCommand : RERButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.GoToFirstDataRecord; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_FirstDataRecord); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_FirstDataRecordDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.First; } }
		public RERFirstDataRecordCommand()
			: base() { }
		public RERFirstDataRecordCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERPreviousDataRecordCommand : RERButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.GoToPreviousDataRecord; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_PreviousDataRecord); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_PreviousDataRecordDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.Prev; } }
		public RERPreviousDataRecordCommand()
			: base() { }
		public RERPreviousDataRecordCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERNextDataRecordCommand : RERButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.GoToNextDataRecord; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_NextDataRecord); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_NextDataRecordDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.Next; } }
		public RERNextDataRecordCommand()
			: base() { }
		public RERNextDataRecordCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERLastDataRecordCommand : RERButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.GoToLastDataRecord; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_LastDataRecord); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_LastDataRecordDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.Last; } }
		public RERLastDataRecordCommand()
			: base() { }
		public RERLastDataRecordCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERFinishAndMergeCommand : RERButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ShowFinishAndMergeForm; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_MailMergeSaveDocumentAsCommand); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_MailMergeSaveDocumentAsCommandDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.MailMerge; } }
		public RERFinishAndMergeCommand()
			: base() { }
		public RERFinishAndMergeCommand(RibbonItemSize size)
			: base(size) { }
	}
}
