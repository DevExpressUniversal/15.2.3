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
	#region XlsCommandPivotViewDataItem -- SXDI --
	public class XlsCommandPivotViewDataItem : XlsCommandBase {
		static Dictionary<int, PivotShowDataAs> listPivotShowDataAs = CreatePivotShowDataAsList();
		public static Dictionary<PivotShowDataAs, int> listRevertPivotShowDataAs = CreateRevertPivotShowDataAsList(listPivotShowDataAs);
		#region Fields
		enum ConFunction {
		Sum = 0,
		Count = 1,
		Average = 2,
		Max = 3,
		Min = 4,
		Product = 5,
		CountNumbers = 6,
		StdDev = 7,
		StdDevp = 8,
		Var = 9,
		Varp = 10
		}
		XLUnicodeStringNoCch dataItemName = new XLUnicodeStringNoCch();
		int baseField;
		int baseItem;
		#endregion
		#region Properties
		public int PivotFieldIndex { get; set; }
		public PivotDataConsolidateFunction DataItemFunction { get; set; }
		public PivotShowDataAs ShowDataAs { get; set; }
		public bool IsBaseField { get; set; }
		public bool IsBaseItem { get; set; }
		public int BaseField {
			get { return baseField; } 
			set {
				IsBaseField = ((int)ShowDataAs < 5 && (int)ShowDataAs > 0) ? true : false;
				if (IsBaseField)
					baseField = value;
			} 
		}
		public int BaseItem {
			get { return baseItem; }
			set {
				switch(ShowDataAs){
					case PivotShowDataAs.Difference:
					case PivotShowDataAs.Percent:
					case PivotShowDataAs.PercentDifference:
						IsBaseItem = true;
						baseItem = value;
						break;
				}
			} 
		}
		public int ItemFormat { get; set; }
		public string DataItemName {
			get { return dataItemName.Value; }
			set {
				ValueChecker.CheckLength(value, 255, "ItemName");
				dataItemName.Value = value;
			}
		}
		#endregion
		#region Methods
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			PivotFieldIndex = reader.ReadInt16();
			DataItemFunction = ToConsolidateFunction(reader.ReadInt16());
			ShowDataAs = listPivotShowDataAs[reader.ReadInt16()];
			BaseField = reader.ReadInt16();
			BaseItem = reader.ReadInt16();
			ItemFormat = reader.ReadInt16();
			int lenDataItemName = reader.ReadUInt16();
			if (lenDataItemName != 0xFFFF)
				dataItemName = XLUnicodeStringNoCch.FromStream(reader, lenDataItemName);
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			XlsBuildPivotView builder = contentBuilder.CurrentBuilderPivotView;
			if (builder != null && builder.PivotTable != null) {
				PivotDataField dataField = new PivotDataField(builder.PivotTable, PivotFieldIndex);
				dataField.SetNameCore(DataItemName);
				dataField.SetSubtotalCore(DataItemFunction);
				dataField.SetShowDataAsCore(ShowDataAs);
				if (IsBaseField)
					dataField.SetBaseFieldCore(BaseField);
				if (IsBaseItem)
					dataField.SetBaseItemCore(BaseItem);
				dataField.SetNumberFormatIndexCore(contentBuilder.StyleSheet.GetNumberFormatIndex(ItemFormat));
				builder.PivotTable.DataFields.AddCore(dataField);
			}
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((short)PivotFieldIndex);
			writer.Write((short)ToShortValue(DataItemFunction));
			writer.Write((short)listRevertPivotShowDataAs[ShowDataAs]);
			writer.Write((short)BaseField);
			writer.Write((short)BaseItem);
			writer.Write((short)ItemFormat);
			if (DataItemName.Length != 0) {
				writer.Write((ushort)DataItemName.Length);
				dataItemName.Write(writer);
			}
			else {
				writer.Write((ushort)0xFFFF);
			}
		}
		protected override short GetSize() {
			int result = 14;
			if (DataItemName.Length > 0)
				result += dataItemName.Length;
			return (short)result;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
		PivotDataConsolidateFunction ToConsolidateFunction(short index){
			switch ((ConFunction)index) {
				case ConFunction.Sum: return PivotDataConsolidateFunction.Sum;
				case ConFunction.Count: return PivotDataConsolidateFunction.Count;
				case ConFunction.Average: return PivotDataConsolidateFunction.Average;
				case ConFunction.Max: return PivotDataConsolidateFunction.Max;
				case ConFunction.Min: return PivotDataConsolidateFunction.Min;
				case ConFunction.Product: return PivotDataConsolidateFunction.Product;
				case ConFunction.CountNumbers: return PivotDataConsolidateFunction.CountNums;
				case ConFunction.StdDev: return PivotDataConsolidateFunction.StdDev;
				case ConFunction.StdDevp: return PivotDataConsolidateFunction.StdDevp;
				case ConFunction.Var: return PivotDataConsolidateFunction.Var;
				case ConFunction.Varp: return PivotDataConsolidateFunction.Varp;
			}
			return PivotDataConsolidateFunction.Sum;
		}
		short ToShortValue(PivotDataConsolidateFunction func) {
			switch (func) {
				case PivotDataConsolidateFunction.Sum: return (short)ConFunction.Sum;
				case PivotDataConsolidateFunction.Count: return (short)ConFunction.Count;
				case PivotDataConsolidateFunction.Average: return (short)ConFunction.Average;
				case PivotDataConsolidateFunction.Max: return (short)ConFunction.Max;
				case PivotDataConsolidateFunction.Min: return (short)ConFunction.Min;
				case PivotDataConsolidateFunction.Product: return (short)ConFunction.Product;
				case PivotDataConsolidateFunction.CountNums: return (short)ConFunction.CountNumbers;
				case PivotDataConsolidateFunction.StdDev: return (short)ConFunction.StdDev;
				case PivotDataConsolidateFunction.StdDevp: return (short)ConFunction.StdDevp;
				case PivotDataConsolidateFunction.Var: return (short)ConFunction.Var;
				case PivotDataConsolidateFunction.Varp: return (short)ConFunction.Varp;
			}
			return (short)ConFunction.Sum;
		}
		#endregion
		static Dictionary<int, PivotShowDataAs> CreatePivotShowDataAsList() {
			Dictionary<int, PivotShowDataAs> list = new Dictionary<int, PivotShowDataAs>();
			list.Add(0, PivotShowDataAs.Normal);
			list.Add(1, PivotShowDataAs.Difference);
			list.Add(2, PivotShowDataAs.Percent);
			list.Add(3, PivotShowDataAs.PercentDifference);
			list.Add(4, PivotShowDataAs.RunningTotal);
			list.Add(5, PivotShowDataAs.PercentOfRow);
			list.Add(6, PivotShowDataAs.PercentOfColumn);
			list.Add(7, PivotShowDataAs.PercentOfTotal);
			list.Add(8, PivotShowDataAs.Index);
			return list;
		}
		static Dictionary<PivotShowDataAs, int> CreateRevertPivotShowDataAsList(Dictionary<int, PivotShowDataAs> parent) {
			Dictionary<PivotShowDataAs, int> list = new Dictionary<PivotShowDataAs, int>();
			foreach (int key in parent.Keys)
				list.Add(parent[key], key);
			return list;
		}
	}
	#endregion
}
