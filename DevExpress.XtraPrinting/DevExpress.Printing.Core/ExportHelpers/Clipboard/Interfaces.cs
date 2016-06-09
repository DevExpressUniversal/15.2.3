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
using System.Windows.Forms;
using System.Collections.Generic;
using DevExpress.Export.Xl;
using DevExpress.Compatibility.System.Windows.Forms;
using DevExpress.Export;
namespace DevExpress.XtraExport.Helpers {
	public interface IClipboardExporter<TCol, TRow>
		where TRow : IRowBase
		where TCol : IColumn {
		void BeginExport();
		void EndExport();
		void AddHeaders(IEnumerable<TCol> selectedColumns, IEnumerable<XlCellFormatting> appearance);
		void AddGroupHeader(string groupHeader, XlCellFormatting appearance, int columnsCount);
		void AddRow(IEnumerable<ClipboardCellInfo> rowInfo);
		void SetDataObject(DataObject data);
	}
	public interface IClipboardGridView<TCol, TRow> :IGridView<TCol, TRow>
		where TRow : IRowBase
		where TCol : IColumn {
		IEnumerable<TCol> GetSelectedColumns();
		IEnumerable<TRow> GetSelectedRows();
		string GetRowCellDisplayText(TRow row, string columnName);
		XlCellFormatting GetCellAppearance(TRow row, TCol col);
		bool CanCopyToClipboard();
		int GetSelectedCellsCount();
		bool UseHierarchyIndent(TRow row, TCol col);
		void ProgressBarCallBack(int progress);
	}
	public interface IClipboardGroupRow<out TRow> :IGroupRow<TRow>
		where TRow : IRowBase {
		IEnumerable<TRow> GetSelectedRows();
		bool IsTreeListGroupRow();
	}
	public interface IClipboardManager<TCol, TRow>
		where TRow : IRowBase
		where TCol : IColumn {
		void SetClipboardData(DataObject dataObject);
		void AssignOptions(ClipboardOptions options);
	}
}
