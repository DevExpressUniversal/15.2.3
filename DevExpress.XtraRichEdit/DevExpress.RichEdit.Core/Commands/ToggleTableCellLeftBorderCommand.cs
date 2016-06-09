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
using System.ComponentModel;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Tables.Native;
namespace DevExpress.XtraRichEdit.Commands {
	public class ToggleTableCellsLeftBorderCommand : ToggleTableCellsBordersCommandBase {
		public ToggleTableCellsLeftBorderCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleTableCellsLeftBorderCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleTableCellsLeftBorder; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleTableCellsLeftBorderCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleTableCellsLeftBorderDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleTableCellsLeftBorderCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.ToggleTableCellsLeftBorder; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleTableCellsLeftBorderCommandImageName")]
#endif
		public override string ImageName { get { return "BorderLeft"; } }
		protected internal override void CollectBorders(ITableCellBorders cellBorders, SelectedTableCellPositionInfo cellPositionInfo, List<BorderBase> borders) {
			if (cellPositionInfo.FirstSelectedColumn)
				borders.Add(cellBorders.LeftBorder);
		}
		protected override void CollectTableBorders(TableBorders tableBorders, List<BorderBase> borders) {
			borders.Add(tableBorders.LeftBorder);
		}
		protected internal override void CollectRightNeighbourBorders(SelectedCellsIntervalInRow cellsInRow, List<BorderBase> borders) {
		}
		protected internal override void CollectTopNeighbourBorders(SelectedCellsIntervalInRow cellsInRow, List<BorderBase> borders) {
		}
		protected internal override void CollectBottomNeighbourBorders(SelectedCellsIntervalInRow cellsInRow, List<BorderBase> borders) {
		}
	}
}
