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
using System.IO;
using System.Reflection;
using System.Text;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	public class XlsCommandMergeCells : XlsCommandBase {
		readonly List<CellRangeInfo> mergedCells = new List<CellRangeInfo>();
		public IList<CellRangeInfo> MergedCells { get { return mergedCells; } }
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			int count = reader.ReadUInt16();
			for(int i = 0; i < count; i++) {
				int firstRow = reader.ReadUInt16();
				int lastRow = reader.ReadUInt16();
				int firstColumn = Math.Min((int)reader.ReadUInt16(), 0x00ff);
				int lastColumn = Math.Min((int)reader.ReadUInt16(), 0x00ff);
				CellRangeInfo range = new CellRangeInfo(new CellPosition(firstColumn, firstRow), new CellPosition(lastColumn, lastRow));
				if(range.IsValid() && contentBuilder.IsNotOverlapped(range))
					MergedCells.Add(range);
			}
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			Worksheet sheet = contentBuilder.CurrentSheet;
			int count = MergedCells.Count;
			for(int i = 0; i < count; i++) {
				CellRangeInfo item = MergedCells[i];
				CellRange range = XlsCellRangeFactory.CreateCellRange(sheet, item.First, item.Last) as CellRange;
				if(range.CellCount > 1)
					sheet.MergedCells.Add(range);
			}
		}
		protected override void WriteCore(BinaryWriter writer) {
			int count = Math.Min(MergedCells.Count, XlsDefs.MaxMergeCellCount);
			writer.Write((ushort)count);
			for(int i = 0; i < count; i++) {
				CellRangeInfo range = MergedCells[i];
				writer.Write((ushort)range.First.Row);
				writer.Write((ushort)range.Last.Row);
				writer.Write((ushort)range.First.Column);
				writer.Write((ushort)range.Last.Column);
			}
		}
		protected override short GetSize() {
			int count = Math.Min(MergedCells.Count, XlsDefs.MaxMergeCellCount);
			return (short)(count * 8 + 2);
		}
		public override IXlsCommand GetInstance() {
			this.mergedCells.Clear();
			return this;
		}
	}
}
