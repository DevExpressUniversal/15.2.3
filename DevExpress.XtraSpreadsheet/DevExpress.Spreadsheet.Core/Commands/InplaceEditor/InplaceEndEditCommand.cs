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
namespace DevExpress.XtraSpreadsheet.Commands {
	#region InplaceEndEditCommand
	public class InplaceEndEditCommand : InplaceEditorSimpleCommand {
		bool commitSuccessfull;
		public InplaceEndEditCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.InplaceEndEdit; } }
		public bool CommitSuccessfull { get { return commitSuccessfull; } }
		#endregion
		protected internal override void ExecuteCore() {
			CheckExecutedAtUIThread();
			this.commitSuccessfull = InplaceEditor.CanClose();
			if (!commitSuccessfull)
				return;
			this.commitSuccessfull = InplaceEditor.Commit(false, false);
			if (commitSuccessfull)
				InplaceEditor.Deactivate(true);
		}
	}
	#endregion
	#region InplaceEndEditEnterToMultipleCellsCommand
	public class InplaceEndEditEnterToMultipleCellsCommand : InplaceEditorSimpleCommand {
		public InplaceEndEditEnterToMultipleCellsCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.InplaceEndEditEnterToMultipleCells; } }
		#endregion
		protected internal override void ExecuteCore() {
			CheckExecutedAtUIThread();
			if (!InplaceEditor.CanClose())
				return;
			if (InplaceEditor.Commit(true, false))
				InplaceEditor.Deactivate(true);
		}
	}
	#endregion
	#region InplaceEndEditEnterArrayFormula
	public class InplaceEndEditEnterArrayFormulaCommand : InplaceEditorSimpleCommand {
		public InplaceEndEditEnterArrayFormulaCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.InplaceEndEditEnterArrayFormula; } }
		#endregion
		protected internal override void ExecuteCore() {
			CheckExecutedAtUIThread();
			if (!InplaceEditor.CanClose())
				return;
			if (InplaceEditor.Commit(true, true))
				InplaceEditor.Deactivate(true);
		}
	}
	#endregion
}
