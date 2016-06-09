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
using DevExpress.Utils.Commands;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Localization;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region ChangeActiveSheetCommandBase (abstract class)
	public abstract class ChangeActiveSheetCommandBase : SpreadsheetMenuItemSimpleCommand {
		protected ChangeActiveSheetCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected internal override void ExecuteCore() {
			DocumentModel.BeginUpdateFromUI();
			try {
				List<Worksheet> sheets = DocumentModel.GetVisibleSheets();
				int index = sheets.IndexOf(DocumentModel.ActiveSheet);
				if (index < 0)
					DocumentModel.ActiveSheet = sheets[0];
				else {
					Worksheet sheet = GetActualSheet(sheets, index);
					if (sheet != null)
						DocumentModel.ActiveSheet = sheet;
				}
			}
			finally {
				DocumentModel.EndUpdateFromUI();
			}
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
		}
		protected internal abstract Worksheet GetActualSheet(List<Worksheet> sheets, int currentIndex);
	}
	#endregion
	#region MoveToNextSheetCommand
	public class MoveToNextSheetCommand : ChangeActiveSheetCommandBase {
		public MoveToNextSheetCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.MoveToNextSheet; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_MoveToNextSheetDescription; } }
		#endregion
		protected internal override Worksheet GetActualSheet(List<Worksheet> sheets, int currentIndex) {
			return currentIndex != sheets.Count - 1 ? sheets[currentIndex + 1] : null;
		}
	}
	#endregion
}
