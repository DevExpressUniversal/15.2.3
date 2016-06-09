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
using DevExpress.Utils;
using DevExpress.Export.Xl;
using System.Collections.Generic;
using DevExpress.XtraExport.Helpers;
namespace DevExpress.Printing.ExportHelpers {
	public class ColumnPositionConverter<TCol> : IXlColumnPositionConverter
	where TCol : class, IColumn {
		Dictionary<string, int> columnPositionTable;
		Dictionary<string, TCol> columnTable;
		public ColumnPositionConverter(Dictionary<string, int> columnPositionTable) {
			Guard.ArgumentNotNull(columnPositionTable, "columnTable");
			this.columnPositionTable = columnPositionTable;
		}
		public ColumnPositionConverter(Dictionary<string, TCol> columnTable) {
			Guard.ArgumentNotNull(columnTable, "columnTable");
			this.columnTable = columnTable;
		}
		public TCol GetColumnByFieldName(string fieldName) {
			TCol result;
			if(columnTable != null && columnTable.TryGetValue(fieldName, out result)) return result;
			return null;
		}
		public int GetColumnIndex(string name) {
			int result;
			if(columnPositionTable != null && columnPositionTable.TryGetValue(name, out result))
				return result;
			return -1;
		}
		public int GetTopRowForColumn(string name) {
			return 1;
		}
	}
	internal static class ColumnTableCreator<TCol> where TCol : class, IColumn {
		public static Dictionary<string, TValue> Create<TValue>(IEnumerable<TCol> gridColumns, Func<TCol, TValue> value) {
			var result = new Dictionary<string, TValue>();
			foreach(TCol col in gridColumns) {
				var addedValue = value(col);
				if(addedValue != null && !result.ContainsKey(col.FieldName)) result.Add(col.FieldName, value(col));
			}
			return result;
		}
	}
}
