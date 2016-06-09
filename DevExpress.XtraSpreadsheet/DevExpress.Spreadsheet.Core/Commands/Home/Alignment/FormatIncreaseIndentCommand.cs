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
using System.Collections.Generic;
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Localization;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region FormatIncreaseIndentCommand
	public class FormatIncreaseIndentCommand : FormatChangeIndentCommand {
		public FormatIncreaseIndentCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.FormatIncreaseIndent; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FormatIncreaseIndent; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FormatIncreaseIndentDescription; } }
		protected override bool UseOfficeImage { get { return true; } }
		public override string ImageName { get { return "IndentIncrease"; } }
		protected override int Delta { get { return 1; } }
		#endregion
	}
	#endregion
	#region FormatChangeIndentCommand (abstract class)
	public abstract class FormatChangeIndentCommand : ChangeSelectedCellsAlignmentPropertiesCommand<int> {
		protected FormatChangeIndentCommand(ISpreadsheetControl control)
			: base(control) {
		}
		protected abstract int Delta { get; }
		protected internal override int GetActiveCellValue(ICell cell) {
			return cell.ActualAlignment.Indent;
		}
		int ValidateIndentValue(int value) {
			return Math.Min(250, Math.Max(0, value));
		}
		protected internal override int GetNewValue(IValueBasedCommandUIState<int> state) {
			return ValidateIndentValue(base.GetNewValue(state));
		}
		protected internal override void SetValue(ICellAlignmentInfo accessor, int value) {
			IActualCellAlignmentInfo actualAlignment = accessor as IActualCellAlignmentInfo;
			if (actualAlignment != null)
				accessor.Indent = (byte)ValidateIndentValue(actualAlignment.Indent + Delta);
		}
	}
	#endregion
}
