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
using DevExpress.Office.Localization;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region FormatFontSizeCommand
	public class FormatFontSizeCommand : FormatFontSizeCommandBase {
		public FormatFontSizeCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.FormatFontSize; } }
		public override OfficeStringId OfficeMenuCaptionStringId { get { return OfficeStringId.MenuCmd_ChangeFontSize; } }
		public override OfficeStringId OfficeDescriptionStringId { get { return OfficeStringId.MenuCmd_ChangeFontSizeDescription; } }
		protected override bool UseOfficeTextsAndImage { get { return true; } }
		#endregion
		protected internal override void SetValue(IRunFontInfo accessor, double value) {
			accessor.Size = ValidateFontSize(value);
		}
	}
	#endregion
	#region FormatFontSizeCommandBase (abstract class)
	public abstract class FormatFontSizeCommandBase : ChangeSelectedCellsFontPropertiesCommand<double> {
		protected FormatFontSizeCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected double ValidateFontSize(double value) {
			if (value <= 0)
				return DocumentModel.GetDefaultRunFontInfo().Size;
			else
				return value;
		}
		protected internal override double GetNewValue(IValueBasedCommandUIState<double> state) {
			return ValidateFontSize(base.GetNewValue(state));
		}
		protected internal override double GetActiveCellValue(ICell cell) {
			return cell.ActualFont.Size;
		}
	}
	#endregion
}
