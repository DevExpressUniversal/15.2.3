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
using DevExpress.Export.Xl;
using DevExpress.Utils;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.PivotGrid.Export {
	public class PivotColumnsCreator : PivotExportItemsCreator<PivotColumnImplementer> {
		readonly bool fillFormatSettings;
		bool allowFixedColumns;
		protected override bool IsColumnCreator { get { return true; } }
		public PivotColumnsCreator(PivotGridData pivotData, IDataAwareExportOptions exportOptions, int rowAreaWidth, int lastAreaLevel, int areaCount)
			: base(pivotData, exportOptions, rowAreaWidth, lastAreaLevel, areaCount) {
			fillFormatSettings = pivotData.DataFieldArea == PivotDataArea.ColumnArea || pivotData.DataFieldArea == PivotDataArea.None;
			allowFixedColumns = exportOptions.AllowFixedColumns == DefaultBoolean.True;
		}
		PivotColumnSettings CreateColumnSettings(PivotFieldValueItem valueItem) {
			const int minWidth = 64;
			PivotFieldItemBase field = valueItem.Field ?? valueItem.DataField;
			PivotColumnSettings settings = new PivotColumnSettings();
			settings.Visible = true;
			settings.Appearance = new XlCellFormatting() { Font = new XlFont() };
			settings.Width = field != null && field.Width >= minWidth ? field.Width : minWidth;
			return settings;
		}
		protected override PivotColumnImplementer CreateAreaExportItem(int index) {
			PivotFieldValueItem item = VisualItems.GetUnpagedItem(false, 0, index);
			return new PivotColumnImplementer() {
				IsRowAreaColumn = true,
				Settings = CreateColumnSettings(item),
				FormatSettings = CreateGeneralFormatSettings(),
				IsFixedLeft = allowFixedColumns && index == FirstAreaMeasure - 1,
				LogicalPosition = index,
				ExportIndex = index
			};
		}
		protected override PivotColumnImplementer CreateGroupExportItem(PivotFieldValueItem item, int logicalPosition, int level, bool collapsed) {
			return new PivotColumnImplementer() {
				IsGroupColumn = true,
				IsCollapsed = collapsed,
				GroupHeader = item.DisplayText,
				Settings = CreateColumnSettings(item),
				FormatSettings = CreateGeneralFormatSettings(),
				LogicalPosition = logicalPosition,
				ExportIndex = item.Index,
				Level = level
			};
		}
		protected override PivotColumnImplementer CreateSingleExportItem(PivotFieldValueItem item, int logicalPosition, int exportIndex, int level) {
			return new PivotColumnImplementer() {
				Settings = CreateColumnSettings(item),
				FormatSettings = fillFormatSettings ? CreateFormatSettings(item) : CreateGeneralFormatSettings(),
				LogicalPosition = logicalPosition,
				ExportIndex = exportIndex,
				Level = level
			};
		}
	}
}
