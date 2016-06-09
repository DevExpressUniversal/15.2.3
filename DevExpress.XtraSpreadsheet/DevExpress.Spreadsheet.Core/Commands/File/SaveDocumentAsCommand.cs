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
using DevExpress.Utils.Commands;
using DevExpress.Office.Localization;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region SaveDocumentAsCommandBase (abstract class)
	public abstract class SaveDocumentAsCommandBase : SpreadsheetCommand {
		protected SaveDocumentAsCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override OfficeStringId OfficeMenuCaptionStringId { get { return OfficeStringId.MenuCmd_SaveDocumentAs; } }
		public override OfficeStringId OfficeDescriptionStringId { get { return OfficeStringId.MenuCmd_SaveDocumentAsDescription; } }
		protected override bool UseOfficeTextsAndImage { get { return true; } }
		public override string ImageName { get { return "SaveAs"; } }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			CheckExecutedAtUIThread();
			NotifyBeginCommandExecution(state);
			try {
				if (ExecuteCore(state))
					DocumentServer.Modified = false;
			}
			finally {
				NotifyEndCommandExecution(state);
			}
		}
		protected internal abstract bool ExecuteCore(ICommandUIState state);
		protected override void UpdateUIStateCore(ICommandUIState state) {
			CheckExecutedAtUIThread();
			ApplyCommandsRestriction(state, Options.InnerBehavior.SaveAs);
		}
	}
	#endregion
	#region SaveDocumentAsCommand
	public class SaveDocumentAsCommand : SaveDocumentAsCommandBase {
		public SaveDocumentAsCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.FileSaveAs; } }
		protected override bool ShouldBeExecutedOnKeyUpInSilverlightEnvironment { get { return true; } }
		public override bool ShowsModalDialog { get { return true; } }
		#endregion
		protected internal override bool ExecuteCore(ICommandUIState state) {
			if (!SaveDocumentCommand.CloseInplaceEditor(InnerControl))
				return false;
			return InnerControl.SaveDocumentAs();
		}
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	#region SaveDocumentAsSilentlyCommand
	public class SaveDocumentAsSilentlyCommand : SaveDocumentAsCommandBase {
		public SaveDocumentAsSilentlyCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.FileSaveAsSilently; } }
		#endregion
		protected internal override bool ExecuteCore(ICommandUIState state) {
			IValueBasedCommandUIState<string> valueBasedState = state as IValueBasedCommandUIState<string>;
			if (valueBasedState == null)
				return false;
			InnerControl.SaveDocument(valueBasedState.Value);
			return true;
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			return new DefaultValueBasedCommandUIState<string>();
		}
	}
	#endregion
}
