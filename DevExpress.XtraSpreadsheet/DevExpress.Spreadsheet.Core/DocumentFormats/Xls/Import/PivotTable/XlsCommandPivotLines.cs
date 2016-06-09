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
	#region XlsCommandPivotLines -- SXLI --
	public class XlsCommandPivotLines : XlsCommandRecordBase {
		#region Fields
		static short[] typeCodes = new short[] {
			0x003c, 
		};
		readonly List<XlsPivotLineItem> items = new List<XlsPivotLineItem>();
		#endregion
		#region Properties
		public List<XlsPivotLineItem> Items { get { return items; } }
		#endregion
		#region Methods
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			XlsBuildPivotView builderPivotView = contentBuilder.CurrentBuilderPivotView;
			if (contentBuilder.ContentType == XlsContentType.Sheet && builderPivotView != null && builderPivotView.PivotLinesType != XlsPivotLinesType.None) {
				using (XlsCommandStream itemStream = new XlsCommandStream(contentBuilder, reader, typeCodes, Size)) {
					using (BinaryReader itemReader = new BinaryReader(itemStream)) {
						int count = builderPivotView.PivotLinesType == XlsPivotLinesType.Rows ? builderPivotView.PivotLinesRowCount : builderPivotView.PivotLinesColumnCount;
						int countIndex = builderPivotView.PivotLinesType == XlsPivotLinesType.Rows ? builderPivotView.PivotItemRowCount : builderPivotView.PivotItemColumnCount;
						for (int i = 0; i < count; i++)
							items.Add(XlsPivotLineItem.FromStream(itemReader, countIndex));
					}
				}
				return;
			}
			base.ReadCore(reader, contentBuilder);
		}
		protected override void CheckPosition(XlsReader reader, XlsContentBuilder contentBuilder, long initialPosition, long expectedPosition) {
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			XlsBuildPivotView builder = contentBuilder.CurrentBuilderPivotView;
			if (builder != null && builder.PivotTable != null) { 
				if (builder.PivotLinesType == XlsPivotLinesType.Rows) {
					foreach(XlsPivotLineItem element in Items){
						bool ignoreFlag = (element.IsGrand || element.ItemType == PivotFieldItemType.Blank);
						builder.PivotTable.RowItems.Add(PivotLayoutItemFactory.CreateInstance(
							ignoreFlag ? new int[0] : ParseLineEntry(element), 
							element.PivotItemIndexCount, 
							element.ItemData, 
							element.ItemType)	
						);
					}
					builder.PivotLinesType = XlsPivotLinesType.Columns;
				}
				else if (builder.PivotLinesType == XlsPivotLinesType.Columns) {
					foreach (XlsPivotLineItem element in Items) {
						bool ignoreFlag = (element.IsGrand || element.ItemType == PivotFieldItemType.Blank);
						builder.PivotTable.ColumnItems.Add(PivotLayoutItemFactory.CreateInstance(
							ignoreFlag ? new int[0] : ParseLineEntry(element),
							element.PivotItemIndexCount,
							element.ItemData,
							element.ItemType)
						);
					}
					builder.PivotLinesType = XlsPivotLinesType.None;
				}
			}
		}
		int[] ParseLineEntry(XlsPivotLineItem element) {
			List<int> result = new List<int>();
			for (int index = 0; index < element.LineEntry.Count; index++) 
				if (index >= element.PivotItemIndexCount && index < element.CountLineEntry)
					result.Add(element.LineEntry[index]);
			return result.ToArray();
		}
		public override IXlsCommand GetInstance() {
			items.Clear();
			return this;
		}
		#endregion
	}
	#endregion
	#region XlsPivotLineItem
	public class XlsPivotLineItem : IEquatable<XlsPivotLineItem> {
		#region Fields
		private enum PivotLineFlags {
			IsMultiDataName = 0, 
			ItemData = 1, 
			IsSubtotal = 2, 
			IsBlock = 3, 
			IsGrand = 4, 
			IsMultiDataOnAxis = 5, 
			IsEmpty = 7, 
		}
		BitwiseContainer lineFlags = new BitwiseContainer(16, new int[2] { 1, 8 });
		List<int> lineEntry = new List<int>();
		#endregion
		#region Properties
		public int PivotItemIndexCount { get; set; }
		public PivotFieldItemType ItemType { get; set; }
		public int CountLineEntry { get; set; }
		public bool IsMultiDataName {
			get { return lineFlags.GetBoolValue((int)PivotLineFlags.IsMultiDataName); }
			set { lineFlags.SetBoolValue((int)PivotLineFlags.IsMultiDataName, value); }
		}
		public bool IsSubtotal {
			get { return lineFlags.GetBoolValue((int)PivotLineFlags.IsSubtotal); }
			set { lineFlags.SetBoolValue((int)PivotLineFlags.IsSubtotal, value); }
		}
		public bool IsBlock {
			get { return lineFlags.GetBoolValue((int)PivotLineFlags.IsBlock); }
			set { lineFlags.SetBoolValue((int)PivotLineFlags.IsBlock, value); }
		}
		public bool IsGrand {
			get { return lineFlags.GetBoolValue((int)PivotLineFlags.IsGrand); }
			set { lineFlags.SetBoolValue((int)PivotLineFlags.IsGrand, value); }
		}
		public bool IsMultiDataOnAxis {
			get { return lineFlags.GetBoolValue((int)PivotLineFlags.IsMultiDataOnAxis); }
			set { lineFlags.SetBoolValue((int)PivotLineFlags.IsMultiDataOnAxis, value); }
		}
		public int ItemData {
			get { return lineFlags.GetIntValue((int)PivotLineFlags.ItemData); }
			set { lineFlags.SetIntValue((int)PivotLineFlags.ItemData, value); }
		}
		public bool IsEmptyItem {
			get { return lineFlags.GetBoolValue((int)PivotLineFlags.IsEmpty); }
			set { lineFlags.SetBoolValue((int)PivotLineFlags.IsEmpty, value); }
		}
		public List<int> LineEntry { get { return lineEntry; } }
		#endregion
		static Dictionary<int, PivotFieldItemType> itemType = InitItemType();
		static Dictionary<PivotFieldItemType, int> revertItemType = InitRevertItemType(itemType);
		#region Methods
		static Dictionary<int, PivotFieldItemType> InitItemType() {
			Dictionary<int, PivotFieldItemType> types = new Dictionary<int, PivotFieldItemType>();
			types.Add(0x0000, PivotFieldItemType.Data);
			types.Add(0x0001, PivotFieldItemType.DefaultValue);
			types.Add(0x0002, PivotFieldItemType.Sum);
			types.Add(0x0003, PivotFieldItemType.CountA);
			types.Add(0x0004, PivotFieldItemType.Count);
			types.Add(0x0005, PivotFieldItemType.Avg);
			types.Add(0x0006, PivotFieldItemType.Max);
			types.Add(0x0007, PivotFieldItemType.Min);
			types.Add(0x0008, PivotFieldItemType.Product);
			types.Add(0x0009, PivotFieldItemType.StdDev);
			types.Add(0x000A, PivotFieldItemType.StdDevP);
			types.Add(0x000B, PivotFieldItemType.Var);
			types.Add(0x000C, PivotFieldItemType.VarP);
			types.Add(0x000D, PivotFieldItemType.Grand);
			types.Add(0x000E, PivotFieldItemType.Blank);
			return types;
		}
		static Dictionary<PivotFieldItemType, int> InitRevertItemType(Dictionary<int, PivotFieldItemType> types) {
			Dictionary<PivotFieldItemType, int> revert = new Dictionary<PivotFieldItemType, int>();
			foreach (int key in types.Keys)
				revert.Add(types[key], key);
			return revert;
		}
		public static XlsPivotLineItem FromStream(BinaryReader reader, int countElement) {
			XlsPivotLineItem result = new XlsPivotLineItem();
			result.Read(reader, countElement);
			return result;
		}
		protected void Read(BinaryReader reader, int countElement) {
			PivotItemIndexCount = reader.ReadInt16();
			ItemType = CalculatedTypeEnum(reader.ReadUInt16());
			CountLineEntry = reader.ReadInt16();
			lineFlags.ShortContainer = (short)reader.ReadUInt16();
			for (int index = 0; index < countElement; index++)
				lineEntry.Add(reader.ReadInt16());
		}
		public void Write(BinaryWriter writer) {
			writer.Write((short)PivotItemIndexCount);
			writer.Write((ushort)CalculatedTypeNumber(ItemType));
			writer.Write((short)CountLineEntry);
			writer.Write((ushort)lineFlags.ShortContainer);
			foreach (int element in LineEntry)
				writer.Write((short)element);
		}
		PivotFieldItemType CalculatedTypeEnum(int type) {
			type &= 0x7FFF;
			if (itemType.ContainsKey(type))
				return itemType[type];
			return PivotFieldItemType.Blank;
		}
		int CalculatedTypeNumber(PivotFieldItemType type) {
			return revertItemType[type];
		}
		public override bool Equals(Object other) {
			if (typeof(XlsPivotLineItem) != other.GetType())
				return false;
			return this.Equals((XlsPivotLineItem)other);
		}
		public bool Equals(XlsPivotLineItem other) {
			if (this.lineFlags.ShortContainer == other.lineFlags.ShortContainer)
				if (this.lineEntry.Count == other.lineEntry.Count) {
					for (int i = 0; i < this.lineEntry.Count; i++)
						if (this.lineEntry[i] != other.lineEntry[i])
							return false;
					return true;
				}
			return false;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		protected internal int GetItemSize() {
			return 8 + LineEntry.Count * 2;
		}
		#endregion
	}
	#endregion
}
