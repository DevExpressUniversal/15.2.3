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
using DevExpress.Office.Localization;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Utils.Commands;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region FormatIncreaseFontSizeCommand
	public class FormatIncreaseFontSizeCommand : FormatFontSizeCommandBase {
		public FormatIncreaseFontSizeCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.FormatIncreaseFontSize; } }
		public override OfficeStringId OfficeMenuCaptionStringId { get { return OfficeStringId.MenuCmd_IncreaseFontSize; } }
		public override OfficeStringId OfficeDescriptionStringId { get { return OfficeStringId.MenuCmd_IncreaseFontSizeDescription; } }
		protected override bool UseOfficeTextsAndImage { get { return true; } }
		public override string ImageName { get { return "FontSizeIncrease"; } }
		#endregion
		protected internal override void ModifyDocumentModelCore(ICommandUIState state) {
			IValueBasedCommandUIState<double> valueBasedState = state as IValueBasedCommandUIState<double>;
			if (valueBasedState == null)
				return;
			valueBasedState.Value = InnerControl.PredefinedFontSizeCollection.CalculateNextFontSize((int)Math.Round(GetActiveCell().ActualFont.Size));
			base.ModifyDocumentModelCore(state);
		}
		protected internal override void SetValue(IRunFontInfo accessor, double value) {
			accessor.Size = ValidateFontSize(value);
		}
	}
	#endregion
}
