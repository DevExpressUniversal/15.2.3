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
using System.Linq;
using DevExpress.Export;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.PivotGrid.Export {
	public class PivotRowsCreator : PivotExportItemsCreator<PivotRowImplementer> {
		readonly bool fillFormatSettings;
		protected override bool IsColumnCreator { get { return false; } }
		public PivotRowsCreator(PivotGridData pivotData, IDataAwareExportOptions exportOptions, int columnAreaHeight, int lastAreaLevel, int areaCount)
			: base(pivotData, exportOptions, columnAreaHeight, lastAreaLevel, areaCount) {
			fillFormatSettings = pivotData.DataFieldArea == PivotDataArea.RowArea;
		}
		protected override PivotRowImplementer CreateAreaExportItem(int index) {
			return new PivotRowImplementer() {
				IsColumnAreaRow = true,
				FormatSettings = CreateGeneralFormatSettings(),
				LogicalPosition = index,
				ExportIndex = index
			};
		}
		protected override PivotRowImplementer CreateGroupExportItem(PivotFieldValueItem item, int logicalPosition, int level, bool collapsed) {
			return new PivotGroupRowImplementer() {
				ExportIndex = item.Index,
				LogicalPosition = logicalPosition,
				FormatSettings = CreateGeneralFormatSettings(),
				Level = level,
				IsCollapsed = collapsed,
				Caption = item.DisplayText
			};
		}
		protected override PivotRowImplementer CreateSingleExportItem(PivotFieldValueItem item, int logicalPosition, int exportIndex, int level) {
			return new PivotRowImplementer() {
				FormatSettings = fillFormatSettings ? CreateFormatSettings(item) : CreateGeneralFormatSettings(),
				LogicalPosition = logicalPosition,
				ExportIndex = exportIndex,
				Level = level
			};
		}
	}
}
