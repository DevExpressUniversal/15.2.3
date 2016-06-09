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
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Import.Xls;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	public class XlsCommandMulRk : XlsCommandContentBase {
		XlsContentMulRk content = new XlsContentMulRk();
		#region Properties
		public int RowIndex { get { return content.RowIndex; } set { content.RowIndex = value; } }
		public int FirstColumnIndex { get { return content.FirstColumnIndex; } set { content.FirstColumnIndex = value; } }
		public int LastColumnIndex { get { return content.LastColumnIndex; } }
		public List<XlsRkRec> RkRecords { get { return content.RkRecords; } }
		#endregion
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			Row row = contentBuilder.CurrentSheet.Rows[RowIndex];
			int count = RkRecords.Count;
			for (int i = 0; i < count; i++) {
				XlsRkRec rec = RkRecords[i];
				ICell cell = row.Cells[FirstColumnIndex + i];
				ApplyCellValue(cell, rec.Rk.Value);
				cell.SetCellFormatIndex(contentBuilder.StyleSheet.GetCellFormatIndex(rec.FormatIndex));
			}
		}
		protected void ApplyCellValue(ICell cell, double value) {
			DocumentModel workbook = cell.Worksheet.Workbook;
			bool suppressCellValueAssignment = workbook.SuppressCellValueAssignment;
			workbook.SuppressCellValueAssignment = false;
			cell.AssignValueCore(value);
			workbook.SuppressCellValueAssignment = suppressCellValueAssignment;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
		protected override IXlsContent GetContent() {
			return content;
		}
	}	
}
