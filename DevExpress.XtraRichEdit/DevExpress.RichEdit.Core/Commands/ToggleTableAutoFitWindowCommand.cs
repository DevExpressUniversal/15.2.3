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
namespace DevExpress.XtraRichEdit.Commands {
	#region ToggleTableAutoFitWindowCommand
	public class ToggleTableAutoFitWindowCommand : ToggleTableAutoFitCommandBase {
		const int hundredPercent = 5000;
		public ToggleTableAutoFitWindowCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleTableAutoFitWindowCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.ToggleTableAutoFitWindow; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleTableAutoFitWindowCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleTableAutoFitWindow; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleTableAutoFitWindowCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleTableAutoFitWindowDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleTableAutoFitWindowCommandImageName")]
#endif
		public override string ImageName { get { return "TableAutoFitWindow"; } }
		#endregion
		protected override WidthUnitInfo GetTableWidthInfo() {
			return new WidthUnitInfo(WidthUnitType.FiftiethsOfPercent, hundredPercent);
		}
		protected override WidthUnitInfo GetTableCellWidthInfo(TableCell cell, double rowWidth) {
			int value = (int)Math.Ceiling(hundredPercent * cell.PreferredWidth.Value / rowWidth);
			return new WidthUnitInfo(WidthUnitType.FiftiethsOfPercent, value);
		}
		protected override int CalculateRowWidth(TableRow row) {
			int result = row.WidthAfter.Value + row.WidthBefore.Value;
			TableCellCollection cells = row.Cells;
			int cellCount = cells.Count;
			for (int i = 0; i < cellCount; i++) {
				result += cells[i].PreferredWidth.Value;
			}
			return result;
		}
	}
	#endregion
}
