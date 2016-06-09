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
using System.Text;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	public class XlsCommandSelection : XlsCommandBase {
		#region Fields
		const int fixedPartSize = 9;
		const int variablePartItemSize = 6;
		ViewPaneType pane = ViewPaneType.TopLeft;
		CellPosition activeCell = new CellPosition(0, 0);
		int activeCellIndex;
		readonly List<CellRangeInfo> selectedCells = new List<CellRangeInfo>();
		#endregion
		#region Properties
		public ViewPaneType Pane { get { return pane; } set { pane = value; } }
		public CellPosition ActiveCell { get { return activeCell; } set { activeCell = value; } }
		public int ActiveCellIndex {
			get { return activeCellIndex; }
			set {
				Guard.ArgumentNonNegative(value, "ActiveCellIndex value");
				activeCellIndex = value;
			}
		}
		public IList<CellRangeInfo> SelectedCells { get { return selectedCells; } }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			Pane = ViewPaneTypeHelper.CodeToPaneType(reader.ReadByte());
			int rowIndex = reader.ReadUInt16();
			int columnIndex = Math.Min((int)reader.ReadUInt16(), XlsDefs.MaxColumnCount - 1);
			ActiveCell = new CellPosition(columnIndex, rowIndex);
			ActiveCellIndex = reader.ReadInt16();
			int count = reader.ReadUInt16();
			for(int i = 0; i < count; i++) {
				int firstRow = reader.ReadUInt16();
				int lastRow = reader.ReadUInt16();
				int firstColumn = reader.ReadByte();
				int lastColumn = reader.ReadByte();
				CellRangeInfo range = new CellRangeInfo(new CellPosition(firstColumn, firstRow), new CellPosition(lastColumn, lastRow));
				if(range.IsValid())
					SelectedCells.Add(range);
			}
			int bytesToRead = Size - (fixedPartSize + count * variablePartItemSize);
			if (bytesToRead > 0)
				reader.ReadBytes(bytesToRead);
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			ModelWorksheetView view = contentBuilder.CurrentSheet.ActiveView;
			if(Pane != view.ActivePaneType) return;
			contentBuilder.SelectionActiveCell = ActiveCell;
			contentBuilder.SelectionRanges.AddRange(SelectedCells);
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write(ViewPaneTypeHelper.PaneTypeToCode(Pane));
			writer.Write((ushort)this.activeCell.Row);
			writer.Write((ushort)this.activeCell.Column);
			writer.Write((short)this.activeCellIndex);
			int count = SelectedCells.Count;
			writer.Write((ushort)count);
			for(int i = 0; i < count; i++) {
				CellRangeInfo range = SelectedCells[i];
				writer.Write((ushort)range.First.Row);
				writer.Write((ushort)range.Last.Row);
				writer.Write((byte)range.First.Column);
				writer.Write((byte)range.Last.Column);
			}
		}
		protected override short GetSize() {
			return (short)(fixedPartSize + SelectedCells.Count * variablePartItemSize);
		}
		public override IXlsCommand GetInstance() {
			return new XlsCommandSelection();
		}
	}
}
