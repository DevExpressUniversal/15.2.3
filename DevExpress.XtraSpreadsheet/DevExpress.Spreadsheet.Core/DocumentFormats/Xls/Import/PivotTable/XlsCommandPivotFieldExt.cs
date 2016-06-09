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
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsCommandPivotFieldExt -- SXVDEx --
	public class XlsCommandPivotFieldExt : XlsCommandBase {
		#region Fields
		int dItemAutoSort;
		private enum FieldFlags {
			ShowAllItems = 0,
			DragToRow = 1,
			DragToColumn = 2,
			DragToPage = 3,
			DragToHide = 4,
			NotDragToData = 5,
			ServerBased = 7,
			AutoSort = 9,
			AscendSort = 10,
			AutoShow = 11,
			TopAutoShow = 12,
			CalculatedField = 13,
			PageBreaksBetweenItems = 14,
			HideNewItems = 15,
			Outline = 21,
			InsertBlankRow = 22,
			SubtotalAtTop = 23
		}
		BitwiseContainer pivotFieldFlags = new BitwiseContainer(32);
		XLUnicodeStringNoCch subName = new XLUnicodeStringNoCch();
		#endregion
		#region Properties
		public bool ShowAllItems {
			get { return pivotFieldFlags.GetBoolValue((int)FieldFlags.ShowAllItems); }
			set { pivotFieldFlags.SetBoolValue((int)FieldFlags.ShowAllItems, value); }
		}
		public bool DragToRow {
			get { return pivotFieldFlags.GetBoolValue((int)FieldFlags.DragToRow); }
			set { pivotFieldFlags.SetBoolValue((int)FieldFlags.DragToRow, value); }
		}
		public bool DragToColumn {
			get { return pivotFieldFlags.GetBoolValue((int)FieldFlags.DragToColumn); }
			set { pivotFieldFlags.SetBoolValue((int)FieldFlags.DragToColumn, value); }
		}
		public bool DragToPage {
			get { return pivotFieldFlags.GetBoolValue((int)FieldFlags.DragToPage); }
			set { pivotFieldFlags.SetBoolValue((int)FieldFlags.DragToPage, value); }
		}
		public bool DragToHide {
			get { return pivotFieldFlags.GetBoolValue((int)FieldFlags.DragToHide); }
			set { pivotFieldFlags.SetBoolValue((int)FieldFlags.DragToHide, value); }
		}
		public bool NotDragToData {
			get { return pivotFieldFlags.GetBoolValue((int)FieldFlags.NotDragToData); }
			set { pivotFieldFlags.SetBoolValue((int)FieldFlags.NotDragToData, value); }
		}
		public bool ServerBased {
			get { return pivotFieldFlags.GetBoolValue((int)FieldFlags.ServerBased); }
			set { pivotFieldFlags.SetBoolValue((int)FieldFlags.ServerBased, value); }
		}
		public bool AutoSort {
			get { return pivotFieldFlags.GetBoolValue((int)FieldFlags.AutoSort); }
			set { pivotFieldFlags.SetBoolValue((int)FieldFlags.AutoSort, value); }
		}
		public bool AscendSort {
			get { return pivotFieldFlags.GetBoolValue((int)FieldFlags.AscendSort); }
			set { pivotFieldFlags.SetBoolValue((int)FieldFlags.AscendSort, value); }
		}
		public bool AutoShow {
			get { return pivotFieldFlags.GetBoolValue((int)FieldFlags.AutoShow); }
			set { pivotFieldFlags.SetBoolValue((int)FieldFlags.AutoShow, value); }
		}
		public bool TopAutoShow {
			get { return pivotFieldFlags.GetBoolValue((int)FieldFlags.TopAutoShow); }
			set { pivotFieldFlags.SetBoolValue((int)FieldFlags.TopAutoShow, value); }
		}
		public bool CalculatedField {
			get { return pivotFieldFlags.GetBoolValue((int)FieldFlags.CalculatedField); }
			set { pivotFieldFlags.SetBoolValue((int)FieldFlags.CalculatedField, value); }
		}
		public bool PageBreaksBetweenItems {
			get { return pivotFieldFlags.GetBoolValue((int)FieldFlags.PageBreaksBetweenItems); }
			set { pivotFieldFlags.SetBoolValue((int)FieldFlags.PageBreaksBetweenItems, value); }
		}
		public bool HideNewItems {
			get { return pivotFieldFlags.GetBoolValue((int)FieldFlags.HideNewItems); }
			set { pivotFieldFlags.SetBoolValue((int)FieldFlags.HideNewItems, value); }
		}
		public bool Outline {
			get { return pivotFieldFlags.GetBoolValue((int)FieldFlags.Outline); }
			set { pivotFieldFlags.SetBoolValue((int)FieldFlags.Outline, value); }
		}
		public bool InsertBlankRow {
			get { return pivotFieldFlags.GetBoolValue((int)FieldFlags.InsertBlankRow); }
			set { pivotFieldFlags.SetBoolValue((int)FieldFlags.InsertBlankRow, value); }
		}
		public bool SubtotalAtTop {
			get { return pivotFieldFlags.GetBoolValue((int)FieldFlags.SubtotalAtTop); }
			set { pivotFieldFlags.SetBoolValue((int)FieldFlags.SubtotalAtTop, value); }
		}
		public int NumberAutoPivotItems {
			get { return pivotFieldFlags.Container[3]; }
			set {
				if (AutoShow)
					ValueChecker.CheckValue(value, 1, 255, "NumberAutoPivotItems");
				else
					ValueChecker.CheckValue(value, 0, 255, "NumberAutoPivotItems");
				byte[] array = pivotFieldFlags.Container;
				array[3] = (byte)value;
				pivotFieldFlags.Container = array;
			}
		}
		public int DataItemAutoSort {
			get { return dItemAutoSort; } 
			set {
				ValueChecker.CheckValue(value, -1, short.MaxValue, "DataItemAutoSort");
				dItemAutoSort = value;
			} 
		}
		public int RanksAutoShow { get; set; }
		public int NumberFormatId { get; set; }
		public string SubName {
			get { return subName.Value; }
			set {
				ValueChecker.CheckLength(value, 255, "PivotFieldSubName");
				subName.Value = value;
			}
		}
		#endregion
		#region Methods
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			pivotFieldFlags.IntContainer = reader.ReadInt32();
			DataItemAutoSort = reader.ReadInt16();
			RanksAutoShow = reader.ReadInt16();
			NumberFormatId = reader.ReadInt16();
			if (this.Size >= 20) {
				int subNameLength = reader.ReadUInt16();
				reader.ReadInt64();
				if (subNameLength != 0xFFFF)
					subName = XLUnicodeStringNoCch.FromStream(reader, subNameLength);
			}
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			XlsBuildPivotView builder = contentBuilder.CurrentBuilderPivotView;
			if (builder != null) {
				PivotField pivotField = builder.PivotField;
				pivotField.BeginUpdate();
				try {
					pivotField.SetShowItemsWithNoData(ShowAllItems);
					pivotField.DragToRow = DragToRow;
					pivotField.DragToCol = DragToColumn;
					pivotField.DragToPage = DragToPage;
					pivotField.DragOff = DragToHide;
					pivotField.DragToData = !NotDragToData;
					pivotField.ServerField = ServerBased;
					if (AutoSort)
						pivotField.SetSortType(AscendSort ? PivotTableSortTypeField.Ascending : PivotTableSortTypeField.Descending);
					else
						pivotField.SetSortType(PivotTableSortTypeField.Manual);
					pivotField.AutoShow = AutoShow;
					pivotField.TopAutoShow = TopAutoShow;
					pivotField.InsertPageBreak = PageBreaksBetweenItems;
					pivotField.HideNewItems = HideNewItems;
					pivotField.SetOutline(Outline);
					pivotField.SetInsertBlankRow(InsertBlankRow);
					pivotField.SetSubtotalTop(SubtotalAtTop);
					pivotField.ItemPageCount = NumberAutoPivotItems;
					pivotField.RankBy = RanksAutoShow;
					pivotField.SetSubtotalCaptionCore(SubName);
					pivotField.SetNumberFormatIndex(contentBuilder.StyleSheet.GetNumberFormatIndex(NumberFormatId));
				}
				finally {
					pivotField.EndUpdate();
				}
			}
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((int)pivotFieldFlags.IntContainer);
			writer.Write((short)DataItemAutoSort);
			writer.Write((short)RanksAutoShow);
			writer.Write((short)NumberFormatId);
			if (SubName.Length > 0) {
				writer.Write((ushort)SubName.Length);
				writer.Write((long)0);
				subName.Write(writer);
			}
			else {
				writer.Write((ushort)0xFFFF);
				writer.Write((long)0);
			}
		}
		protected override short GetSize() {
			int result = 20;
			if (SubName.Length > 0)
				result += subName.Length;
			return (short)result;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
		#endregion
	}
	#endregion
}
