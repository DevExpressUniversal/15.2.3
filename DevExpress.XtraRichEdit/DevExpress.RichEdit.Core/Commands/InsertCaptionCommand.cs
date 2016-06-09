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
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Services.Implementation;
using DevExpress.XtraRichEdit.Utils;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Commands {
	public abstract class InsertCaptionCommandBase : TransactedInsertObjectCommand {
		protected InsertCaptionCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected internal override RichEditCommand InsertObjectCommand { get { return (RichEditCommand)Commands[2]; } }
		protected internal virtual string Prefix { get { return XtraRichEditLocalizer.GetString(PrefixStringId); } }
		protected internal abstract XtraRichEditStringId PrefixStringId { get; }
		protected internal override void CreateCommands() {
			base.CreateCommands();
			Debug.Assert(Commands.Count == 2);
			IInsertTextCommand command = (IInsertTextCommand)Control.CreateCommand(RichEditCommandId.InsertText);
			command.Text = Prefix + " ";
			Commands.Insert(1, (RichEditCommand)command);
		}
	}
	public class InsertEquationsCaptionCommand : InsertCaptionCommandBase {
		public InsertEquationsCaptionCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("InsertEquationsCaptionCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.InsertEquationsCaption; } }
		protected internal override XtraRichEditStringId PrefixStringId { get { return XtraRichEditStringId.Caption_CaptionPrefixEquation; } }
		protected internal override RichEditCommand CreateInsertObjectCommand() {
			return new InsertEquationsCaptionCoreCommand(Control);
		}
	}
	public class InsertFiguresCaptionCommand : InsertCaptionCommandBase {
		public InsertFiguresCaptionCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("InsertFiguresCaptionCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.InsertFiguresCaption; } }
		protected internal override XtraRichEditStringId PrefixStringId { get { return XtraRichEditStringId.Caption_CaptionPrefixFigure; } }
		protected internal override RichEditCommand CreateInsertObjectCommand() {
			return new InsertFiguresCaptionCoreCommand(Control);
		}
	}
	public class InsertTablesCaptionCommand : InsertCaptionCommandBase {
		public InsertTablesCaptionCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("InsertTablesCaptionCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.InsertTablesCaption; } }
		protected internal override XtraRichEditStringId PrefixStringId { get { return XtraRichEditStringId.Caption_CaptionPrefixTable; } }
		protected internal override RichEditCommand CreateInsertObjectCommand() {
			return new InsertTablesCaptionCoreCommand(Control);
		}
	}
}
namespace DevExpress.XtraRichEdit.Commands.Internal {
	#region InsertCaptionCoreBaseCommand (abstract class)
	public abstract class InsertCaptionCoreBaseCommand : InsertFieldCoreCommand {
		protected InsertCaptionCoreBaseCommand(IRichEditControl control, string fieldCode)
			: base(control, fieldCode) {
		}
	}
	#endregion
	#region InsertEquationsCaptionCoreCommand
	public class InsertEquationsCaptionCoreCommand : InsertCaptionCoreBaseCommand {
		public InsertEquationsCaptionCoreCommand(IRichEditControl control)
			: base(control, @"SEQ Equation \* ARABIC") {
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertEquationsCaption; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertEquationsCaptionDescription; } }
		public override string ImageName { get { return "InsertEquationCaption"; } }
		#endregion
	}
	#endregion
	#region InsertFiguresCaptionCoreCommand
	public class InsertFiguresCaptionCoreCommand : InsertCaptionCoreBaseCommand {
		public InsertFiguresCaptionCoreCommand(IRichEditControl control)
			: base(control, @"SEQ Figure \* ARABIC") {
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertFiguresCaption; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertFiguresCaptionDescription; } }
		public override string ImageName { get { return "InsertFigureCaption"; } }
		#endregion
	}
	#endregion
	#region InsertTablesCaptionCoreCommand
	public class InsertTablesCaptionCoreCommand : InsertCaptionCoreBaseCommand {
		public InsertTablesCaptionCoreCommand(IRichEditControl control)
			: base(control, @"SEQ Table \* ARABIC") {
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertTablesCaption; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertTablesCaptionDescription; } }
		public override string ImageName { get { return "InsertTableCaption"; } }
		#endregion
	}
	#endregion
	#region InsertCaptionPlaceholderCommand
	public class InsertCaptionPlaceholderCommand : RichEditMenuItemSimpleCommand, IPlaceholderCommand {
		public InsertCaptionPlaceholderCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertCaptionPlaceholder; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertCaptionPlaceholderDescription; } }
		public override RichEditCommandId Id { get { return RichEditCommandId.InsertCaptionPlaceholder; } }
		public override string ImageName { get { return "InsertCaption"; } }
		protected internal override void ExecuteCore() {
		}
		protected override void UpdateUIStateCore(DevExpress.Utils.Commands.ICommandUIState state) {
			if (state.Enabled)
				state.Enabled = ActivePieceTable.IsMain;
		}
	}
	#endregion
}
