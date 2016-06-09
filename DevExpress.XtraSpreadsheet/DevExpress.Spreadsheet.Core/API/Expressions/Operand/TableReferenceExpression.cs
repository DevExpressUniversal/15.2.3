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
using System.Text;
using DevExpress.Office;
using DevExpress.Office.Utils;
using System.ComponentModel;
using Model = DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.Spreadsheet.Formulas {
	#region TableRowEnum
	public enum TableRowsRestriction {
		None = 0x0000,
		Headers = 0x0001,
		Totals = 0x0002,
		Data = 0x0004,
		All = 0x0007,
		ThisRow = 0x0008,
	}
	#endregion
	#region TableReferenceExpression
	public class TableReferenceExpression : ReferenceExpression, ISupportsCopyFrom<TableReferenceExpression>, ICloneable<TableReferenceExpression> {
		#region Fields
		string tableName;
		TableRowsRestriction includedRows = TableRowsRestriction.None;
		string includedColumnStart;
		string includedColumnEnd;
		#endregion
		public TableReferenceExpression(string tableName) {
			this.tableName = tableName;
		}
		public TableReferenceExpression(string tableName, SheetReference sheetReference)
			: base(sheetReference) {
			this.tableName = tableName;
		}
		protected TableReferenceExpression() {
		}
		#region Properties
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("TableReferenceExpressionTableName")]
#endif
		public string TableName { get { return tableName; } set { tableName = value; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("TableReferenceExpressionColumnsDefined")]
#endif
		public bool ColumnsDefined { get { return !string.IsNullOrEmpty(includedColumnStart); } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("TableReferenceExpressionColumnStart")]
#endif
		public string ColumnStart { get { return includedColumnStart; } set { includedColumnStart = value; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("TableReferenceExpressionColumnEnd")]
#endif
		public string ColumnEnd { get { return includedColumnEnd; } set { includedColumnEnd = value; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("TableReferenceExpressionRowsRestriction")]
#endif
		public TableRowsRestriction RowsRestriction { get { return includedRows; } set { includedRows = value; } }
		#endregion
		public override void Visit(IExpressionVisitor visitor) {
			visitor.Visit(this);
		}
		protected override void BuildExpressionStringCore(StringBuilder result, IWorkbook workbook, Model.WorkbookDataContext context) {
			base.BuildExpressionStringCore(result, workbook, context);
			Model.ParsedThingTable modelThing = ToModelThing();
			modelThing.BuildExpressionStringCore(result, context);
		}
		internal Model.ParsedThingTable ToModelThing() {
			Model.ParsedThingTable modelThing = new Model.ParsedThingTable();
			modelThing.TableName = tableName;
			modelThing.ColumnStart = ColumnStart;
			modelThing.ColumnEnd = ColumnEnd;
			modelThing.IncludedRows = (Model.TableRowEnum)RowsRestriction;
			return modelThing;
		}
		#region ISupportsCopyFrom<TableReferenceExpression> Members
		public void CopyFrom(TableReferenceExpression value) {
			base.CopyFrom(value);
			this.tableName = value.tableName;
			this.includedRows = value.includedRows;
			this.includedColumnStart = value.includedColumnStart;
			this.includedColumnEnd = value.includedColumnEnd;
		}
		#endregion
		#region ICloneable<TableReferenceExpression> Members
		public TableReferenceExpression Clone() {
			TableReferenceExpression result = new TableReferenceExpression();
			result.CopyFrom(this);
			return result;
		}
		protected override IExpression CloneCore() {
			return Clone();
		}
		#endregion
	}
	#endregion
}
