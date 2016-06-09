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
using System.Collections;
using System.ComponentModel;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit;
namespace DevExpress.XtraRichEdit.Commands {
	#region PrevNextDataRecordCommandBase (abstract class)
	public abstract class PrevNextDataRecordCommandBase : RichEditMenuItemSimpleCommand {
		protected PrevNextDataRecordCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected int RecordCount {
			get {
#if SL
				IList source = Options.MailMerge.DataSource as IList;
				if (source == null)
					return 0;
				return source.Count;
#else
				if (DocumentModel.MailMergeDataController.IsReady)
					return DocumentModel.MailMergeDataController.ListSourceRowCount;
				else
					return 0;
#endif
			}
		}
		protected internal override void ExecuteCore() {
			int current = Options.MailMerge.ActiveRecord;
			if (!CanNavigateFrom(current))
				return;
			Options.MailMerge.ActiveRecord = CalculateNextRecordIndex(current);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = DocumentModel.MailMergeDataController.IsReady && CanNavigateFrom(Options.MailMerge.ActiveRecord);
		}
		protected internal abstract bool CanNavigateFrom(int recordIndex);
		protected internal abstract int CalculateNextRecordIndex(int recordIndex);
	}
	#endregion
	#region NextDataRecordCommand
	public class NextDataRecordCommand : PrevNextDataRecordCommandBase {
		public NextDataRecordCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("NextDataRecordCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_NextDataRecord; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("NextDataRecordCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_NextDataRecordDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("NextDataRecordCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.NextDataRecord; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("NextDataRecordCommandImageName")]
#endif
		public override string ImageName { get { return "Next"; } }
		#endregion
		protected internal override bool CanNavigateFrom(int recordIndex) {
			return recordIndex < RecordCount - 1;
		}
		protected internal override int CalculateNextRecordIndex(int recordIndex) {
			return recordIndex + 1;
		}
	}
	#endregion
	#region LastDataRecordCommand
	public class LastDataRecordCommand : PrevNextDataRecordCommandBase {
		public LastDataRecordCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("LastDataRecordCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_LastDataRecord; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("LastDataRecordCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_LastDataRecordDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("LastDataRecordCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.LastDataRecord; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("LastDataRecordCommandImageName")]
#endif
		public override string ImageName { get { return "Last"; } }
		#endregion
		protected internal override bool CanNavigateFrom(int recordIndex) {
			return recordIndex < RecordCount - 1;
		}
		protected internal override int CalculateNextRecordIndex(int recordIndex) {
			return RecordCount - 1;
		}
	}
	#endregion
	#region PreviousDataRecordCommand
	public class PreviousDataRecordCommand : PrevNextDataRecordCommandBase {
		public PreviousDataRecordCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("PreviousDataRecordCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_PreviousDataRecord; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("PreviousDataRecordCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_PreviousDataRecordDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("PreviousDataRecordCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.PreviousDataRecord; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("PreviousDataRecordCommandImageName")]
#endif
		public override string ImageName { get { return "Prev"; } }
		#endregion
		protected internal override bool CanNavigateFrom(int recordIndex) {
			return recordIndex > 0;
		}
		protected internal override int CalculateNextRecordIndex(int recordIndex) {
			return recordIndex - 1;
		}
	}
	#endregion
	#region FirstDataRecordCommand
	public class FirstDataRecordCommand : PrevNextDataRecordCommandBase {
		public FirstDataRecordCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("FirstDataRecordCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_FirstDataRecord; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("FirstDataRecordCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_FirstDataRecordDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("FirstDataRecordCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.FirstDataRecord; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("FirstDataRecordCommandImageName")]
#endif
		public override string ImageName { get { return "First"; } }
		#endregion
		protected internal override bool CanNavigateFrom(int recordIndex) {
			return recordIndex > 0;
		}
		protected internal override int CalculateNextRecordIndex(int recordIndex) {
			return 0;
		}
	}
	#endregion
}
