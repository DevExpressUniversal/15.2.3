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
using DevExpress.Export;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.XtraPivotGrid {
	public delegate void CustomizePivotCellEventHandler(CustomizePivotCellEventArgs e);
	public class PivotFieldValueExportEventArgs : PivotFieldValueEventArgsBase<PivotGridField> {
		public PivotFieldValueExportEventArgs(PivotFieldValueItem item)
			: base(item) {
		}
		public PivotFieldValueExportEventArgs(PivotGridField field)
			: base(field) {
		}
	}
	public class PivotCellValueExportEventArgs : PivotCellEventArgsBase<PivotGridField, PivotGridViewInfoData, PivotGridCustomTotal> {
		public PivotCellValueExportEventArgs(PivotGridCellItem cellItem)
			: base(cellItem) {
		}
	}
	public class CustomizePivotCellEventArgs {
		readonly CustomizePivotCellEventArgsCore coreArgs;
		readonly PivotCellValueExportEventArgs cellItemInfo;
		readonly PivotFieldValueExportEventArgs valueItemInfo;
		public ExportCellType ColumnType { get { return coreArgs.ColumnType; } }
		public ExportCellType RowType { get { return coreArgs.RowType; } }
		public PivotExportArea ExportArea { get { return coreArgs.ExportArea; } }
		public PivotCellValueExportEventArgs CellItemInfo { get { return cellItemInfo; } }
		public PivotFieldValueExportEventArgs ValueItemInfo { get { return valueItemInfo; } }
		public object Value { get { return coreArgs.Value; } set { coreArgs.Value = value; } }
		public XlFormattingObject Formatting { get { return coreArgs.Formatting; } set { coreArgs.Formatting = value; } }
		public bool Handled { get { return coreArgs.Handled; } set { coreArgs.Handled = value; } }
		internal CustomizePivotCellEventArgs(CustomizePivotCellEventArgsCore coreArgs) {
			this.coreArgs = coreArgs;
			if(coreArgs.CellItem != null)
				cellItemInfo = new PivotCellValueExportEventArgs(coreArgs.CellItem);
			if(coreArgs.ValueItem != null)
				valueItemInfo = new PivotFieldValueExportEventArgs(coreArgs.ValueItem);
		}
	}
}
