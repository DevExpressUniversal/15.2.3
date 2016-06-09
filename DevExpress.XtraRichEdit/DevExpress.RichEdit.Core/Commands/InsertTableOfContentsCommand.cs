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
using System.ComponentModel;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Services.Implementation;
namespace DevExpress.XtraRichEdit.Commands {
	#region InsertTableOfContentsCommand
	public class InsertTableOfContentsCommand : TransactedInsertObjectCommand {
		public InsertTableOfContentsCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("InsertTableOfContentsCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.InsertTableOfContents; } }
		protected internal override RichEditCommand CreateInsertObjectCommand() {
			return new InsertTableOfContentsCoreCommand(Control);
		}
	}
	#endregion
	#region InsertTableOfEquationCommand
	public class InsertTableOfEquationsCommand : TransactedInsertObjectCommand {
		public InsertTableOfEquationsCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("InsertTableOfEquationsCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.InsertTableOfEquations; } }
		protected internal override RichEditCommand CreateInsertObjectCommand() {
			return new InsertTableOfEquationsCoreCommand(Control);
		}
	}
	#endregion
	#region InsertTableOfFiguresCommand
	public class InsertTableOfFiguresCommand : TransactedInsertObjectCommand {
		public InsertTableOfFiguresCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("InsertTableOfFiguresCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.InsertTableOfFigures; } }
		protected internal override RichEditCommand CreateInsertObjectCommand() {
			return new InsertTableOfFiguresCoreCommand(Control);
		}
	}
	#endregion
	#region InsertTableOfTablesCommand
	public class InsertTableOfTablesCommand : TransactedInsertObjectCommand {
		public InsertTableOfTablesCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("InsertTableOfTablesCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.InsertTableOfTables; } }
		protected internal override RichEditCommand CreateInsertObjectCommand() {
			return new InsertTableOfTablesCoreCommand(Control);
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Commands.Internal {
	#region InsertTableOfContentCoreBaseCommand (abstract class)
	public abstract class InsertTableOfContentsCoreBaseCommand : InsertFieldCoreCommand {
		protected InsertTableOfContentsCoreBaseCommand(IRichEditControl control, string fieldCode)
			: base(control, fieldCode) {
		}
		protected internal override void UpdateField(Field field) {
			DocumentModel.ResetTemporaryLayout();
			base.UpdateField(field);
			DocumentModel.ResetTemporaryLayout();
			base.UpdateField(field);
			DocumentModel.ResetTemporaryLayout();
		}
		protected override void UpdateUIStateCore(DevExpress.Utils.Commands.ICommandUIState state) {
			base.UpdateUIStateCore(state);
			if (state.Enabled)
				state.Enabled = ActivePieceTable.IsMain;
		}
	}
	#endregion
	#region InsertTableOfContentCoreCommand
	public class InsertTableOfContentsCoreCommand : InsertTableOfContentsCoreBaseCommand {
		public InsertTableOfContentsCoreCommand(IRichEditControl control)
			: base(control, @"TOC \h") {
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertTableOfContents; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertTableOfContentsDescription; } }
		public override string ImageName { get { return "InsertTableOfContents"; } }
		#endregion
	}
	#endregion
	#region InsertTableOfEquationsCoreCommand
	public class InsertTableOfEquationsCoreCommand : InsertTableOfContentsCoreBaseCommand {
		public InsertTableOfEquationsCoreCommand(IRichEditControl control)
			: base(control, @"TOC \h \c ""Equation""") {
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertTableOfEquations; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertTableOfEquationsDescription; } }
		public override string ImageName { get { return "InsertTableOfEquations"; } }
		#endregion
	}
	#endregion
	#region InsertTableOfFiguresCoreCommand
	public class InsertTableOfFiguresCoreCommand : InsertTableOfContentsCoreBaseCommand {
		public InsertTableOfFiguresCoreCommand(IRichEditControl control)
			: base(control, @"TOC \h \c ""Figure""") {
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertTableOfFigures; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertTableOfFiguresDescription; } }
		public override string ImageName { get { return "InsertTableOfFigures"; } }
		#endregion
	}
	#endregion
	#region InsertTableOfTablesCoreCommand
	public class InsertTableOfTablesCoreCommand : InsertTableOfContentsCoreBaseCommand {
		public InsertTableOfTablesCoreCommand(IRichEditControl control)
			: base(control, @"TOC \h \c ""Table""") {
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertTableOfTables; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertTableOfTablesDescription; } }
		public override string ImageName { get { return "InsertTableCaption"; } }
		#endregion
	}
	#endregion
	#region InsertTableOfFiguresPlaceholderCommand
	public class InsertTableOfFiguresPlaceholderCommand : RichEditMenuItemSimpleCommand, IPlaceholderCommand {
		public InsertTableOfFiguresPlaceholderCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertTableOfFiguresPlaceholder; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertTableOfFiguresPlaceholderDescription; } }
		public override RichEditCommandId Id { get { return RichEditCommandId.InsertTableOfFiguresPlaceholder; } }
		public override string ImageName { get { return "InsertTableOfCaptions"; } }
		protected internal override void ExecuteCore() {
		}
		protected override void UpdateUIStateCore(DevExpress.Utils.Commands.ICommandUIState state) {
			state.Enabled = ActivePieceTable.IsMain;
		}
	}
	#endregion
}
