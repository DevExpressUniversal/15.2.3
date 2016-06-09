﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
using DevExpress.Export.Xl;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region FormatAlignmentMiddleCommand
	public class FormatAlignmentMiddleCommand : ChangeSelectedCellsAlignmentPropertiesCommand<bool> {
		public FormatAlignmentMiddleCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.FormatAlignmentMiddle; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FormatAlignmentMiddle; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FormatAlignmentMiddleDescription; } }
		protected override bool UseOfficeImage { get { return true; } }
		public override string ImageName { get { return "AlignMiddleCenter"; } }
		#endregion
		protected internal override void UpdateUIState(ICommandUIState state, bool activeCellValue) {
			state.Checked = activeCellValue;
		}
		protected internal override bool GetNewValue(IValueBasedCommandUIState<bool> state) {
			return !state.Checked;
		}
		protected internal override bool GetActiveCellValue(ICell cell) {
			return cell.ActualAlignment.Vertical == XlVerticalAlignment.Center;
		}
		protected internal override void SetValue(ICellAlignmentInfo accessor, bool value) {
			if (value)
				accessor.Vertical = XlVerticalAlignment.Center;
			else
				accessor.Vertical = XlVerticalAlignment.Bottom;
		}
	}
	#endregion
}
