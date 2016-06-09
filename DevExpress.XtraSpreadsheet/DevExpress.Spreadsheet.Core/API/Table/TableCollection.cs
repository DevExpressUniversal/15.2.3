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
using DevExpress.Office;
using System.Collections.Generic;
using System.ComponentModel;
namespace DevExpress.Spreadsheet {
	public interface TableCollection : ISimpleCollection<Table> {
		Table Add(Range range, bool hasHeaders);
		Table Add(Range range, string name, bool hasHeaders);
		void Remove(Table table);
		void RemoveAt(int index);
		void Clear();
		bool Contains(Table table);
		int IndexOf(Table table);
		IList<Table> GetTables(Range range);
	}
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Office.Utils;
	using DevExpress.XtraSpreadsheet.Localization;
	using DevExpress.XtraSpreadsheet.Utils;
	using ModelHyperlink = DevExpress.XtraSpreadsheet.Model.ModelHyperlink;
	using DevExpress.Spreadsheet;
	using System.Collections;
	using DevExpress.Utils;
	partial class NativeTableCollection : NativeCollectionBase<Table, NativeTable, Model.Table>, TableCollection {
		public NativeTableCollection(NativeWorksheet worksheet)
			: base(worksheet) {
		}
		public override IEnumerable<Model.Table> GetModelItemEnumerable() { 
			return ModelWorksheet.Tables;
		}
		public override int ModelCollectionCount { get { return ModelWorksheet.Tables.Count; } }
		protected override void ClearModelObjects() {
			int count = Count;
			for (int i = count - 1; i >= 0; i--)
				RemoveModelObjectAt(i);
		}
		protected override NativeTable CreateNativeObject(Model.Table modelObject) {
			return new NativeTable(Worksheet, modelObject);
		}
		protected override void RemoveModelObjectAt(int index) {
			NativeTable table = this[index] as NativeTable;
			if (table == null)
				return;
			Model.TableRemoveApiCommand command = new Model.TableRemoveApiCommand(ApiErrorHandler.Instance, table.ModelTable);
			command.Execute();
		}
		protected override void InvalidateItem(NativeTable item) {
			item.Invalidate();
		}
		public Table Add(Range range, bool hasHeaders) {
			return Add(range, string.Empty, hasHeaders);
		}
		public Table Add(Range range, string name, bool hasHeaders) {
			Worksheet.ValidateRange(range);
			Model.InsertTableCommand command = new Model.InsertTableCommand(ApiErrorHandler.Instance, Worksheet.ModelWorksheet, name, hasHeaders, true);
			command.Range = Worksheet.GetModelSingleRange(range);
			command.Execute();
			return InnerList[Count - 1];
		}
		#region GetTables
		public IList<Table> GetTables(Range range) {
			List<Table> result = new List<Table>();
			Model.CellRangeBase modelRange = Worksheet.GetModelRange(range);
			for (int i = Count - 1; i >= 0; i--) {
				NativeTable table = InnerList[i] as NativeTable;
				Model.CellRange tableRange = table.ModelTable.Range;
				if (modelRange.Intersects(tableRange))
					result.Add(table);
			}
			return result;
		}
		#endregion
	}
}
