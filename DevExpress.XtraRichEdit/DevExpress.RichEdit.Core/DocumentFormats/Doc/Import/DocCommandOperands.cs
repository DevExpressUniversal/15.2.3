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
using DevExpress.XtraRichEdit.Model;
using System.IO;
using DevExpress.Utils;
using DevExpress.Office;
using System.Drawing;
namespace DevExpress.XtraRichEdit.Import.Doc {
	#region TabsOperandBase
	public abstract class TabsOperandBase {
		protected const int PositionSize = 2;
		#region Fields
		byte deletedTabsCount;
		byte addedTabsCount;
		readonly List<int> deletedTabsPositions;
		readonly List<int> addedTabsPositions;
		readonly List<TabDescriptor> addedTabs;
		#endregion
		protected TabsOperandBase() {
			this.addedTabsPositions = new List<int>();
			this.deletedTabsPositions = new List<int>();
			this.addedTabs = new List<TabDescriptor>();
		}
		#region Properties
		public int DeletedTabsCount { get { return deletedTabsCount; } }
		public int AddedTabsCount { get { return addedTabsCount; } }
		public List<int> DeletedTabsPositions { get { return deletedTabsPositions; } }
		public List<int> AddedTabsPositions { get { return addedTabsPositions; } }
		public List<TabDescriptor> AddedTabs { get { return addedTabs; } }
		#endregion
		protected void Read(byte[] tabs) {
			this.deletedTabsCount = tabs[0];
			int currentPosition = 1;
			currentPosition += ReadDeletedTabs(tabs, currentPosition); 
			this.addedTabsCount = tabs[currentPosition];
			currentPosition++;
			ReadAddedTabs(tabs, currentPosition);
		}
		protected virtual int ReadDeletedTabs(byte[] tabs, int currentPosition) {
			for (int i = 0; i < DeletedTabsCount; i++)
				this.deletedTabsPositions.Add(BitConverter.ToInt16(tabs, (i * PositionSize) + currentPosition));
			return DeletedTabsCount * PositionSize;
		}
		protected virtual void ReadAddedTabs(byte[] tabs, int currentPosition) {
			for (int i = 0; i < AddedTabsCount; i++)
				this.addedTabsPositions.Add(BitConverter.ToInt16(tabs, (i * PositionSize) + currentPosition));
			currentPosition += AddedTabsCount * PositionSize;
			for (int i = 0; i < AddedTabsCount; i++)
				this.addedTabs.Add(new TabDescriptor(tabs[i + currentPosition]));
		}
		public void Write(BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			byte totalSize = (byte)(1 + DeletedTabsCount * PositionSize + 1 + AddedTabsCount * (1 + PositionSize));
			writer.Write(totalSize);
			writer.Write((byte)DeletedTabsCount);
			for (int i = 0; i < DeletedTabsCount; i++)
				writer.Write((short)Math.Min(DocConstants.MaxXASValue, DeletedTabsPositions[i]));
			writer.Write((byte)AddedTabsCount);
			for (int i = 0; i < AddedTabsCount; i++)
				writer.Write((short)Math.Min(DocConstants.MaxXASValue, AddedTabsPositions[i]));
			for (int i = 0; i < AddedTabsCount; i++)
				AddedTabs[i].Write(writer);
		}
		public void ConvertFromFormattingTabInfo(TabFormattingInfo info, DocumentModelUnitConverter unitConverter) {
			for (int i = 0; i < info.Count; i++)
				if (info[i].Deleted)
					this.deletedTabsPositions.Add(unitConverter.ModelUnitsToTwips(info[i].Position));
				else {
					this.addedTabsPositions.Add(unitConverter.ModelUnitsToTwips(info[i].Position));
					TabDescriptor tbd = new TabDescriptor(info[i].Alignment, info[i].Leader);
					this.addedTabs.Add(tbd);
				}
			this.deletedTabsCount = (byte)deletedTabsPositions.Count;
			this.addedTabsCount = (byte)addedTabs.Count;
		}
		public void AddTabs(TabFormattingInfo formattingInfo, DocumentModelUnitConverter unitConverter) {
			int count = this.DeletedTabsCount;
			for (int i = 0; i < count; i++) {
				TabInfo info = new TabInfo(unitConverter.TwipsToModelUnits(this.DeletedTabsPositions[i]), TabAlignmentType.Left, TabLeaderType.None, true);
				formattingInfo.Add(info);
			}
			count = this.AddedTabsCount;
			for (int i = 0; i < count; i++) {
				TabInfo info = new TabInfo(unitConverter.TwipsToModelUnits(this.AddedTabsPositions[i]), this.AddedTabs[i].Alignment, this.AddedTabs[i].Leader, false);
				formattingInfo.Add(info);
			}
		}
	}
	#endregion
	#region TabsOperand
	public class TabsOperand : TabsOperandBase {
		#region static
		public static TabsOperand FromByteArray(byte[] tabs) {
			TabsOperand result = new TabsOperand();
			result.Read(tabs);
			return result;
		}
		#endregion
	}
	#endregion
	#region TabsOperandClose
	public class TabsOperandClose : TabsOperandBase {
		#region static
		public static TabsOperandClose FromByteArray(byte[] tabs) {
			TabsOperandClose result = new TabsOperandClose();
			result.Read(tabs);
			return result;
		}
		#endregion
		readonly List<int> tolerances;
		public TabsOperandClose() {
			this.tolerances = new List<int>();
		}
		public List<int> Tolerances { get { return tolerances; } }
		protected override int ReadDeletedTabs(byte[] tabs, int currentPosition) {
			int result = base.ReadDeletedTabs(tabs, currentPosition);
			for (int i = 0; i < DeletedTabsCount; i++)
				this.tolerances.Add(BitConverter.ToInt16(tabs, (i * PositionSize) + currentPosition));
			result += DeletedTabsCount * PositionSize;
			return result;
		}
	}
	#endregion
	#region TabDescriptor
	public class TabDescriptor {
		#region static
		static TabDescriptor() {
			tabAlignments = new Dictionary<byte, TabAlignmentType>(4);
			tabAlignments.Add(0x00, TabAlignmentType.Left);
			tabAlignments.Add(0x01, TabAlignmentType.Center);
			tabAlignments.Add(0x02, TabAlignmentType.Right);
			tabAlignments.Add(0x03, TabAlignmentType.Decimal);
			tabLeaders = new Dictionary<byte, TabLeaderType>(6);
			tabLeaders.Add(0x00, TabLeaderType.None);
			tabLeaders.Add(0x01, TabLeaderType.Dots);
			tabLeaders.Add(0x02, TabLeaderType.Hyphens);
			tabLeaders.Add(0x03, TabLeaderType.Underline);
			tabLeaders.Add(0x04, TabLeaderType.ThickLine);
			tabLeaders.Add(0x05, TabLeaderType.MiddleDots);
		}
		#endregion
		#region Fields
		static readonly Dictionary<byte, TabAlignmentType> tabAlignments;
		static readonly Dictionary<byte, TabLeaderType> tabLeaders;
		TabAlignmentType tabAlignment;
		TabLeaderType tabLeader;
		#endregion
		public TabDescriptor(TabAlignmentType alignment, TabLeaderType leader) {
			this.tabAlignment = alignment;
			this.tabLeader = leader;
		}
		public TabDescriptor(byte tbd) {
			TabAlignmentType tabAlignment;
			TabLeaderType tabLeader;
			byte tabAlignmentCode = (byte)(tbd & 0x07);
			byte tabLeaderCode = (byte)((tbd & 0x38) >> 3);
			if (tabAlignments.TryGetValue(tabAlignmentCode, out tabAlignment))
				this.tabAlignment = tabAlignment;
			if (tabLeaders.TryGetValue(tabLeaderCode, out tabLeader))
				this.tabLeader = tabLeader;
		}
		public TabAlignmentType Alignment { get { return this.tabAlignment; } }
		public TabLeaderType Leader { get { return this.tabLeader; } }
		public void Write(BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			byte tbd = (byte)(CalcTabAlignmentCode() | CalcTabLeaderCode() << 3);
			writer.Write(tbd);
		}
		byte CalcTabAlignmentCode() {
			switch (Alignment) {
				case TabAlignmentType.Left:
					return 0x00;
				case TabAlignmentType.Center:
					return 0x01;
				case TabAlignmentType.Right:
					return 0x02;
				case TabAlignmentType.Decimal:
					return 0x03;
				default:
					return 0x00;
			}
		}
		byte CalcTabLeaderCode() {
			switch (Leader) {
				case TabLeaderType.EqualSign:
					return 0x00;
				case TabLeaderType.None:
					return 0x00;
				case TabLeaderType.Dots:
					return 0x01;
				case TabLeaderType.Hyphens:
					return 0x02;
				case TabLeaderType.Underline:
					return 0x03;
				case TabLeaderType.ThickLine:
					return 0x04;
				case TabLeaderType.MiddleDots:
					return 0x05;
				default:
					return 0x00;
			}
		}
	}
	#endregion
	#region TableCellDescriptor
	public class TableCellDescriptor {
		#region static
		public static TableCellDescriptor FromByteArray(byte[] data, int startIndex) {
			TableCellDescriptor result = new TableCellDescriptor();
			result.Read(data, startIndex);
			return result;
		}
		#endregion
		#region Fields
		public const int Size = 20;
		bool fitText;
		bool hideCellMark;
		MergingState horizontalMerging;
		bool noWrap;
		short preferredWidth;
		TextDirection textDirection;
		WidthUnitType type;
		VerticalAlignment verticalAlignment;
		MergingState verticalMerging;
		BorderDescriptor97 bottomBorder;
		BorderDescriptor97 leftBorder;
		BorderDescriptor97 rightBorder;
		BorderDescriptor97 topBorder;
		#endregion
		public TableCellDescriptor() {
			this.bottomBorder = new BorderDescriptor97();
			this.leftBorder = new BorderDescriptor97();
			this.rightBorder = new BorderDescriptor97();
			this.topBorder = new BorderDescriptor97();
		}
		#region Properties
		public bool FitText { get { return this.fitText; } }
		public bool HideCellMark { get { return this.hideCellMark; } }
		public MergingState HorizontalMerging { get { return this.horizontalMerging; } }
		public bool NoWrap { get { return this.noWrap; } }
		public short PreferredWidth { get { return this.preferredWidth; } }
		public TextDirection TextDirection { get { return this.textDirection; } }
		public WidthUnitType Type { get { return this.type; } }
		public VerticalAlignment VerticalAlignment { get { return this.verticalAlignment; } }
		public MergingState VerticalMerging { get { return this.verticalMerging; } }
		public BorderDescriptor97 BottomBorder { get { return this.bottomBorder; } }
		public BorderDescriptor97 LeftBorder { get { return this.leftBorder; } }
		public BorderDescriptor97 RightBorder { get { return this.rightBorder; } }
		public BorderDescriptor97 TopBorder { get { return this.topBorder; } }
		#endregion
		protected void Read(byte[] data, int startIndex) {
			Guard.ArgumentNotNull(data, "data");
			short bitwiseField = BitConverter.ToInt16(data, startIndex);
			this.horizontalMerging = MergingStateCalculator.CalcHorizontalMergingState((byte)(bitwiseField & 0x3));
			this.textDirection = TextDirectionCalculator.CalcTextDirection((byte)((bitwiseField & 0x1c) >> 2));
			this.verticalMerging = MergingStateCalculator.CalcVerticalMergingState((byte)((bitwiseField & 0x60) >> 5));
			this.verticalAlignment = AlignmentCalculator.CalcVerticalAlignment((byte)((bitwiseField & 0x0180) >> 7));
			this.type = WidthUnitCalculator.CalcWidthUnitType((byte)((bitwiseField & 0x0e00) >> 9));
			this.fitText = ((bitwiseField & 0x1000) == 0x1000);
			this.noWrap = ((bitwiseField & 0x2000) == 0x2000);
			this.hideCellMark = ((bitwiseField & 0x4000) == 0x4000);
			this.preferredWidth = BitConverter.ToInt16(data, startIndex + 2);
			this.topBorder = BorderDescriptor97.FromByteArray(data, startIndex + 4);
			this.leftBorder = BorderDescriptor97.FromByteArray(data, startIndex + 8);
			this.bottomBorder = BorderDescriptor97.FromByteArray(data, startIndex + 12);
			this.rightBorder = BorderDescriptor97.FromByteArray(data, startIndex + 16);
		}
		public TableCellDescriptor Clone() {
			TableCellDescriptor result = new TableCellDescriptor();
			result.fitText = this.fitText;
			result.hideCellMark = this.hideCellMark;
			result.horizontalMerging = this.horizontalMerging;
			result.noWrap = this.noWrap;
			result.preferredWidth = preferredWidth;
			result.textDirection = this.textDirection;
			result.type = this.type;
			result.verticalAlignment = this.verticalAlignment;
			result.verticalMerging = this.verticalMerging;
			result.bottomBorder = this.bottomBorder.Clone();
			result.leftBorder = this.leftBorder.Clone();
			result.rightBorder = this.rightBorder.Clone();
			result.topBorder = this.topBorder.Clone();
			return result;
		}
	}
	#endregion
	#region ColumnWidthOperand
	public class ColumnWidthOperand {
		public static ColumnWidthOperand FromByteArray(byte[] data) {
			ColumnWidthOperand result = new ColumnWidthOperand();
			result.Read(data);
			return result;
		}
		#region Fields
		byte startIndex;
		byte endIndex;
		short widthInTwips;
		#endregion
		#region Properties
		public byte StartIndex {
			get { return this.startIndex; }
			set { this.startIndex = value; }
		}
		public byte EndIndex {
			get { return this.endIndex; }
			set { this.endIndex = value; }
		}
		public short WidthInTwips {
			get { return this.widthInTwips; }
			set { this.widthInTwips = value; }
		}
		#endregion
		protected void Read(byte[] data) {
			this.startIndex = data[0];
			this.endIndex = (byte)(data[1] - 1);
			this.widthInTwips = BitConverter.ToInt16(data, 2);
		}
		public byte[] GetBytes() {
			byte[] result = new byte[4];
			result[0] = this.startIndex;
			result[1] = (byte)(this.endIndex + 1);
			Array.Copy(BitConverter.GetBytes(this.widthInTwips), 0, result, 2, 2);
			return result;
		}
		public ColumnWidthOperand Clone() {
			ColumnWidthOperand result = new ColumnWidthOperand();
			result.endIndex = this.endIndex;
			result.startIndex = this.startIndex;
			result.widthInTwips = this.widthInTwips;
			return result;
		}
	}
	#endregion
	#region WidthUnitOperand
	public class WidthUnitOperand {
		public static WidthUnitOperand FromByteArray(byte[] data, int startIndex) {
			WidthUnitOperand result = new WidthUnitOperand();
			result.Read(data, startIndex);
			return result;
		}
		public static WidthUnitOperand NonNegativeFromByteArray(byte[] data, int startIndex) {
			WidthUnitOperand result = new WidthUnitOperand();
			result.Read(data, startIndex);
			if (result.value < 0)
				result.value = 0;
			return result;
		}
		#region Fields
		WidthUnitType type;
		short value;
		#endregion
		#region Properties
		public WidthUnitType Type {
			get { return this.type; }
			set { this.type = value; }
		}
		public int Value {
			get { return this.value; }
			set { this.value = (short)value; }
		}
		#endregion
		protected void Read(byte[] data, int startIndex) {
			this.type = WidthUnitCalculator.CalcWidthUnitType(data[startIndex]);
			this.value = BitConverter.ToInt16(data, startIndex + 1);
		}
		public void ConvertFromWidthUnitInfo(WidthUnitInfo info, DocumentModelUnitConverter unitConverter) {
			this.type = info.Type;
			switch (this.type) {
				case WidthUnitType.Nil:
					this.value = 0;
					break;
				case WidthUnitType.ModelUnits:
					this.value = (short)Math.Min(unitConverter.ModelUnitsToTwips(info.Value), DocConstants.MaxXASValue);
					break;
				default:
					this.value = (short)info.Value;
					break;
			}
		}
		public byte[] GetBytes() {
			byte[] result = new byte[3];
			result[0] = WidthUnitCalculator.CalcWidthUnitTypeCode(this.type);
			Array.Copy(BitConverter.GetBytes(this.value), 0, result, 1, 2);
			return result;
		}
		public WidthUnitOperand Clone() {
			WidthUnitOperand result = new WidthUnitOperand();
			result.type = this.type;
			result.value = this.value;
			return result;
		}
	}
	#endregion
	#region TableCellWidthOperand
	public class TableCellWidthOperand {
		public static TableCellWidthOperand FromByteArray(byte[] data) {
			TableCellWidthOperand result = new TableCellWidthOperand();
			result.Read(data);			
			return result;
		}
		#region Fields
		byte startIndex;
		byte endIndex;
		WidthUnitOperand widthUnit;
		#endregion
		public TableCellWidthOperand() {
			this.widthUnit = new WidthUnitOperand();
		}
		#region Properties
		public byte StartIndex {
			get { return this.startIndex; }
			set { this.startIndex = value; }
		}
		public byte EndIndex {
			get { return this.endIndex; }
			set { this.endIndex = value; }
		}
		public WidthUnitOperand WidthUnit { get { return this.widthUnit; } }
		#endregion
		protected void Read(byte[] data) {
			this.startIndex = data[0];
			this.endIndex = (byte)(data[1] - 1);
			this.widthUnit = WidthUnitOperand.NonNegativeFromByteArray(data, 2);
		}
		public byte[] GetBytes() {
			byte[] result = new byte[5];
			result[0] = this.startIndex;
			result[1] = (byte)(this.endIndex + 1);
			Array.Copy(this.widthUnit.GetBytes(), 0, result, 2, 3);
			return result;
		}
		public TableCellWidthOperand Clone() {
			TableCellWidthOperand result = new TableCellWidthOperand();
			result.endIndex = this.endIndex;
			result.startIndex = this.startIndex;
			result.widthUnit = this.widthUnit.Clone();
			return result;
		}
	}
	#endregion
	#region InsertOperand
	public class InsertOperand {
		public static InsertOperand FromByteArray(byte[] data) {
			InsertOperand result = new InsertOperand();
			result.Read(data);
			return result;
		}
		#region Fields
		byte startIndex;
		byte count;
		short widthInTwips;
		#endregion
		#region Properties
		public byte StartIndex {
			get { return this.startIndex; }
			set { this.startIndex = value; }
		}
		public byte Count {
			get { return this.count; }
			set { this.count = value; }
		}
		public short WidthInTwips {
			get { return this.widthInTwips; }
			set { this.widthInTwips = value; }
		}
		#endregion
		protected void Read(byte[] data) {
			this.startIndex = data[0];
			this.count = data[1];
			this.widthInTwips = BitConverter.ToInt16(data, 2);
		}
		public byte[] GetBytes() {
			byte[] result = new byte[4];
			result[0] = this.startIndex;
			result[1] = this.count;
			Array.Copy(BitConverter.GetBytes(this.widthInTwips), 0, result, 2, 2);
			return result;
		}
		public InsertOperand Clone() {
			InsertOperand result = new InsertOperand();
			result.count = this.count;
			result.startIndex = this.startIndex;
			result.widthInTwips = this.widthInTwips;
			return result;
		}
	}
	#endregion
	#region TableCellBorders
	[Flags]
	public enum DocTableCellBorders {
		Top = 0x01,
		Left = 0x02,
		Bottom = 0x04,
		Right = 0x08,
		TopLeftToBottomRight = 0x10,
		TopRightToBottomLeft = 0x20,
		All = 0x0f
	}
	#endregion
	#region TableBordersOperand
	public class TableBordersOperand {
		#region static
		public static TableBordersOperand FromByteArray(byte[] data) {
			TableBordersOperand result = new TableBordersOperand();
			result.Read(data);
			return result;
		}
		#endregion
		#region Fields
		BorderDescriptor topBorder;
		BorderDescriptor leftBorder;
		BorderDescriptor bottomBorder;
		BorderDescriptor rightBorder;
		BorderDescriptor insideHorizontalBorder;
		BorderDescriptor insideVerticalBorder;
		#endregion
		public TableBordersOperand() {
			this.bottomBorder = new BorderDescriptor();
			this.insideHorizontalBorder = new BorderDescriptor();
			this.insideVerticalBorder = new BorderDescriptor();
			this.leftBorder = new BorderDescriptor();
			this.rightBorder = new BorderDescriptor();
			this.topBorder = new BorderDescriptor();
		}
		#region Properties
		public BorderDescriptor TopBorder { get { return this.topBorder; } }
		public BorderDescriptor LeftBorder { get { return this.leftBorder; } }
		public BorderDescriptor BottomBorder { get { return this.bottomBorder; } }
		public BorderDescriptor RightBorder { get { return this.rightBorder; } }
		public BorderDescriptor InsideHorizontalBorder { get { return this.insideHorizontalBorder; } }
		public BorderDescriptor InsideVerticalBorder { get { return this.insideVerticalBorder; } }
		#endregion
		protected void Read(byte[] data) {
			Guard.ArgumentNotNull(data, "data");
			int currentStartIndex = 0;
			this.topBorder = BorderDescriptor.FromByteArray(data, currentStartIndex);
			currentStartIndex += BorderDescriptor.BorderDescriptorSize;
			this.leftBorder = BorderDescriptor.FromByteArray(data, currentStartIndex);
			currentStartIndex += BorderDescriptor.BorderDescriptorSize;
			this.bottomBorder = BorderDescriptor.FromByteArray(data, currentStartIndex);
			currentStartIndex += BorderDescriptor.BorderDescriptorSize;
			this.rightBorder = BorderDescriptor.FromByteArray(data, currentStartIndex);
			currentStartIndex += BorderDescriptor.BorderDescriptorSize;
			this.insideHorizontalBorder = BorderDescriptor.FromByteArray(data, currentStartIndex);
			currentStartIndex += BorderDescriptor.BorderDescriptorSize;
			this.insideVerticalBorder = BorderDescriptor.FromByteArray(data, currentStartIndex);
		}
		public void Write(BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			this.topBorder.Write(writer);
			this.leftBorder.Write(writer);
			this.bottomBorder.Write(writer);
			this.rightBorder.Write(writer);
			this.insideHorizontalBorder.Write(writer);
			this.insideVerticalBorder.Write(writer);
		}
		public void ApplyProperties(TableBorders destination, DocumentModelUnitConverter unitConverter) {
			BottomBorder.ApplyProperties(destination.BottomBorder, unitConverter);
			InsideHorizontalBorder.ApplyProperties(destination.InsideHorizontalBorder, unitConverter);
			InsideVerticalBorder.ApplyProperties(destination.InsideVerticalBorder, unitConverter);
			LeftBorder.ApplyProperties(destination.LeftBorder, unitConverter);
			RightBorder.ApplyProperties(destination.RightBorder, unitConverter);
			TopBorder.ApplyProperties(destination.TopBorder, unitConverter);
		}
	}
	#endregion
	#region TableBordersOverrideOperand
	public class TableBordersOverrideOperand {
		#region static
		public static TableBordersOverrideOperand FromByteArray(byte[] data) {
			TableBordersOverrideOperand result = new TableBordersOverrideOperand();
			result.Read(data);
			return result;
		}
		#endregion
		#region Fields
		public const byte Size = 0x0b;
		byte startIndex;
		byte endIndex;
		DocTableCellBorders cellBorders;
		BorderDescriptor border;
		#endregion
		public TableBordersOverrideOperand() {
			this.border = new BorderDescriptor();
		}
		#region Properties
		public int StartIndex {
			get { return this.startIndex; }
			set { this.startIndex = (byte)value; }
		}
		public int EndIndex {
			get { return this.endIndex; }
			set { this.endIndex = (byte)value; }
		}
		public DocTableCellBorders CellBorders {
			get { return this.cellBorders; }
			set { this.cellBorders = value; }
		}
		public BorderDescriptor Border { get { return this.border; } }
		#endregion
		protected void Read(byte[] data) {
			Guard.ArgumentNotNull(data, "data");
			this.startIndex = data[0];
			this.endIndex = (byte)(data[1] - 1);
			this.cellBorders = (DocTableCellBorders)data[2];
			this.border = BorderDescriptor.FromByteArray(data, 3);
		}
		public void Write(BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			writer.Write(this.startIndex);
			writer.Write((byte)(this.endIndex + 1));
			writer.Write((byte)this.cellBorders);
			this.border.Write(writer);
		}
		public void ApplyProperties(TableCellBorders destination, DocumentModelUnitConverter unitConverter) {
			if ((CellBorders & DocTableCellBorders.Bottom) == DocTableCellBorders.Bottom)
				ApplyPropertiesCore(destination.BottomBorder, unitConverter);
			if ((CellBorders & DocTableCellBorders.Left) == DocTableCellBorders.Left)
				ApplyPropertiesCore(destination.LeftBorder, unitConverter);
			if ((CellBorders & DocTableCellBorders.Right) == DocTableCellBorders.Right)
				ApplyPropertiesCore(destination.RightBorder, unitConverter);
			if ((CellBorders & DocTableCellBorders.Top) == DocTableCellBorders.Top)
				ApplyPropertiesCore(destination.TopBorder, unitConverter);
		}
		void ApplyPropertiesCore(BorderBase border, DocumentModelUnitConverter unitConverter) {
			border.BeginUpdate();
			try {
				Border.ApplyProperties(border, unitConverter);
			}
			finally {
				border.EndUpdate();
			}
		}
		public TableBordersOverrideOperand Clone() {
			TableBordersOverrideOperand result = new TableBordersOverrideOperand();
			result.startIndex = this.startIndex;
			result.endIndex = this.endIndex;
			result.cellBorders = this.cellBorders;
			result.border = this.border.Clone();
			return result;
		}
	}
	#endregion
	#region CellSpacingOperand
	public class CellSpacingOperand {
		public static CellSpacingOperand FromByteArray(byte[] data) {
			CellSpacingOperand result = new CellSpacingOperand();
			result.Read(data);
			return result;
		}
		#region Fields
		public const byte Size = 0x06;
		byte startIndex;
		byte endIndex;
		DocTableCellBorders cellBorders;
		WidthUnitOperand widthUnit;
		#endregion
		public CellSpacingOperand() {
			this.widthUnit = new WidthUnitOperand();
		}
		#region Properties
		public int StartIndex {
			get { return this.startIndex; }
			set { this.startIndex = (byte)value; }
		}
		public int EndIndex {
			get { return this.endIndex; }
			set { this.endIndex = (byte)value; }
		}
		public DocTableCellBorders CellBorders {
			get { return this.cellBorders; }
			set { this.cellBorders = value; }
		}
		public WidthUnitOperand WidthUnit { get { return this.widthUnit; } }
		#endregion
		protected void Read(byte[] data) {
			Guard.ArgumentNotNull(data, "data");
			this.startIndex = data[0];
			this.endIndex = (byte)(data[1] - 1);
			this.cellBorders = (DocTableCellBorders)data[2];
			this.widthUnit = WidthUnitOperand.NonNegativeFromByteArray(data, 3);
		}
		public void Write(BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			writer.Write(this.startIndex);
			writer.Write((byte)(this.endIndex + 1));
			writer.Write((byte)this.cellBorders);
			writer.Write(this.widthUnit.GetBytes());
		}
		public void ApplyProperties(CellMargins destination, DocumentModelUnitConverter unitConverter) {
			if (WidthUnit.Type == WidthUnitType.ModelUnits)
				WidthUnit.Value = unitConverter.TwipsToModelUnits(WidthUnit.Value);
			if ((CellBorders & DocTableCellBorders.Bottom) == DocTableCellBorders.Bottom)
				ApplyPropertiesCore(destination.Bottom);				
			if ((CellBorders & DocTableCellBorders.Left) == DocTableCellBorders.Left)
				ApplyPropertiesCore(destination.Left);
			if ((CellBorders & DocTableCellBorders.Right) == DocTableCellBorders.Right)
				ApplyPropertiesCore(destination.Right);
			if ((CellBorders & DocTableCellBorders.Top) == DocTableCellBorders.Top)
				ApplyPropertiesCore(destination.Top);
		}
		void ApplyPropertiesCore(MarginUnitBase margin) {
			margin.BeginUpdate();
			try {
				margin.Type = WidthUnit.Type;
				margin.Value = WidthUnit.Value;
			}
			finally {
				margin.EndUpdate();
			}
		}
		public CellSpacingOperand Clone() {
			CellSpacingOperand result = new CellSpacingOperand();
			result.cellBorders = this.cellBorders;
			result.endIndex = this.endIndex;
			result.startIndex = this.startIndex;
			result.widthUnit = this.widthUnit;
			return result;
		}
	}
	#endregion
	#region CellRangeVerticalAlignmentOperand
	public class CellRangeVerticalAlignmentOperand {
		#region static
		public static CellRangeVerticalAlignmentOperand FromByteArray(byte[] data) {
			CellRangeVerticalAlignmentOperand result = new CellRangeVerticalAlignmentOperand();
			result.Read(data);
			return result;
		}
		#endregion
		#region Fields
		public const byte Size = 0x03;
		byte startIndex;
		byte endIndex;
		VerticalAlignment verticalAlignment;
		#endregion
		#region Properties
		public byte StartIndex {
			get { return this.startIndex; }
			set { this.startIndex = value; }
		}
		public byte EndIndex {
			get { return this.endIndex; }
			set { this.endIndex = value; }
		}
		public VerticalAlignment VerticalAlignment {
			get { return this.verticalAlignment; }
			set { this.verticalAlignment = value; }
		}
		#endregion
		protected void Read(byte[] data) {
			this.startIndex = data[0];
			this.endIndex = (byte)(data[1] - 1);
			this.verticalAlignment = AlignmentCalculator.CalcVerticalAlignment(data[2]);
		}
		public void Write(BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			writer.Write(this.startIndex);
			writer.Write((byte)(this.endIndex + 1));
			writer.Write(AlignmentCalculator.CalcVerticalAlignmentTypeCode(this.verticalAlignment));
		}
		public CellRangeVerticalAlignmentOperand Clone() {
			CellRangeVerticalAlignmentOperand result = new CellRangeVerticalAlignmentOperand();
			result.startIndex = this.startIndex;
			result.endIndex = this.endIndex;
			result.verticalAlignment = this.verticalAlignment;
			return result;
		}
	}
	#endregion
	#region DocCommandParagraphBorder97
	public abstract class DocCommandParagraphBorder97 : IDocCommand {
		#region Fields
		BorderDescriptor97 currentBorder;
		#endregion
		#region Properties
		public BorderDescriptor97 CurrentBorder { get { return this.currentBorder; } }
		#endregion
		protected DocCommandParagraphBorder97() {
		}
		#region IDocCommand Members
		public ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Paragraph; } }
		public void Read(byte[] data) {
			this.currentBorder = BorderDescriptor97.FromByteArray(data, 0);
		}
		public void Write(BinaryWriter writer) {
			this.currentBorder.Write(writer);
		}
		public virtual void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
		}
		protected internal void ApplyBorderProperties(BorderInfo border) {
			border.Width = CurrentBorder.Width;
			border.Style = DocBorderCalculator.MapToBorderLineStyle(CurrentBorder.Style);
			border.Color = CurrentBorder.BorderColor;
			border.Offset = CurrentBorder.Distance;
		}
		#endregion
	}
	#endregion
	#region DocCommandParagraphTopBorder
	public class DocCommandParagraphTopBorder : DocCommandParagraphBorder97 {
		public DocCommandParagraphTopBorder() {
		}
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			ParagraphInfo paragraphInfo = propertyContainer.ParagraphInfo;
			BorderInfo border = new BorderInfo();
			ApplyBorderProperties(border);
			paragraphInfo.FormattingInfo.TopBorder = border;
			paragraphInfo.FormattingOptions.UseTopBorder = true;
		}
	}
	#endregion
	#region DocCommandParagraphLeftBorder
	public class DocCommandParagraphLeftBorder : DocCommandParagraphBorder97 {
		public DocCommandParagraphLeftBorder() {
		}
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			ParagraphInfo paragraphInfo = propertyContainer.ParagraphInfo;
			BorderInfo border = new BorderInfo();
			ApplyBorderProperties(border);
			paragraphInfo.FormattingInfo.LeftBorder = border;
			paragraphInfo.FormattingOptions.UseLeftBorder = true;
		}
	}
	#endregion
	#region DocCommandParagraphBottomBorder
	public class DocCommandParagraphBottomBorder : DocCommandParagraphBorder97 {
		public DocCommandParagraphBottomBorder() {
		}
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			ParagraphInfo paragraphInfo = propertyContainer.ParagraphInfo;
			BorderInfo border = new BorderInfo();
			ApplyBorderProperties(border);
			paragraphInfo.FormattingInfo.BottomBorder = border;
			paragraphInfo.FormattingOptions.UseBottomBorder = true;
		}
	}
	#endregion
	#region DocCommandParagraphRightBorder
	public class DocCommandParagraphRightBorder : DocCommandParagraphBorder97 {
		public DocCommandParagraphRightBorder() {
		}
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			ParagraphInfo paragraphInfo = propertyContainer.ParagraphInfo;
			BorderInfo border = new BorderInfo();
			ApplyBorderProperties(border);
			paragraphInfo.FormattingInfo.RightBorder = border;
			paragraphInfo.FormattingOptions.UseRightBorder = true;
		}
	}
	#endregion
	#region DocCommandParagraphBorder
	public abstract class DocCommandParagraphBorder : IDocCommand {
		#region Fields
		const byte paragraphBorderOperandSize = 0x08;
		BorderDescriptor currentBorder;
		#endregion
		#region Properties
		public BorderDescriptor CurrentBorder { get { return this.currentBorder; } set { this.currentBorder = value; } }
		#endregion
		protected DocCommandParagraphBorder() {
			this.currentBorder = new BorderDescriptor();
		}
		#region IDocCommand Members
		public ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Paragraph; } }
		public void Read(byte[] data) {
			this.currentBorder = BorderDescriptor.FromByteArray(data, 0);
		}
		public void Write(BinaryWriter writer) {
			writer.Write(DocCommandFactory.GetOpcodeByType(this.GetType()));
			writer.Write(paragraphBorderOperandSize);
			this.currentBorder.Write(writer);
		}
		public virtual void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
		}
		protected internal void ApplyBorderProperties(BorderInfo border) {
			border.Width = CurrentBorder.Width;
			border.Style = DocBorderCalculator.MapToBorderLineStyle(CurrentBorder.Style);
			border.Color = CurrentBorder.Color;
			border.Offset = CurrentBorder.Offset;
		}
		#endregion
	}
	#endregion
	#region DocCommandParagraphTopBorderNew
	public class DocCommandParagraphTopBorderNew : DocCommandParagraphBorder {
		public DocCommandParagraphTopBorderNew()
			: base() {
		}
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			ParagraphInfo paragraphInfo = propertyContainer.ParagraphInfo;
			BorderInfo border = new BorderInfo();
			ApplyBorderProperties(border);
			paragraphInfo.FormattingInfo.TopBorder = border;
			paragraphInfo.FormattingOptions.UseTopBorder = true;
		}
	}
	#endregion
	#region DocCommandParagraphLeftBorderNew
	public class DocCommandParagraphLeftBorderNew : DocCommandParagraphBorder {
		public DocCommandParagraphLeftBorderNew()
			: base() {
		}
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			ParagraphInfo paragraphInfo = propertyContainer.ParagraphInfo;
			BorderInfo border = new BorderInfo();
			ApplyBorderProperties(border);
			paragraphInfo.FormattingInfo.LeftBorder = border;
			paragraphInfo.FormattingOptions.UseLeftBorder = true;
		}
	}
	#endregion
	#region DocCommandParagraphBottomBorderNew
	public class DocCommandParagraphBottomBorderNew : DocCommandParagraphBorder {
		public DocCommandParagraphBottomBorderNew()
			: base() {
		}
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			ParagraphInfo paragraphInfo = propertyContainer.ParagraphInfo;
			BorderInfo border = new BorderInfo();
			ApplyBorderProperties(border);
			paragraphInfo.FormattingInfo.BottomBorder = border;
			paragraphInfo.FormattingOptions.UseBottomBorder = true;
		}
	}
	#endregion
	#region DocCommandParagraphRightBorderNew
	public class DocCommandParagraphRightBorderNew : DocCommandParagraphBorder {
		public DocCommandParagraphRightBorderNew()
			: base() {
		}
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			ParagraphInfo paragraphInfo = propertyContainer.ParagraphInfo;
			BorderInfo border = new BorderInfo();
			ApplyBorderProperties(border);
			paragraphInfo.FormattingInfo.RightBorder = border;
			paragraphInfo.FormattingOptions.UseRightBorder = true;
		}
	}
	#endregion
	#region TableDefinitionOperand
	public class TableDefinitionOperand {
		#region static
		public static TableDefinitionOperand FromByteArray(byte[] data) {
			TableDefinitionOperand result = new TableDefinitionOperand();
			result.Read(data);
			return result;
		}
		#endregion
		#region Fields
		byte columnsCount;
		List<short> positions;
		List<TableCellDescriptor> cells;
		#endregion
		public TableDefinitionOperand() {
			this.positions = new List<short>();
			this.cells = new List<TableCellDescriptor>();
		}
		#region Properties
		public int ColumnsCount { 
			get { return this.columnsCount; }
			protected internal set { this.columnsCount = (byte)value; }
		}
		public List<short> Positions { get { return this.positions; } }
		public List<TableCellDescriptor> Cells { get { return this.cells; } }
		#endregion
		protected void Read(byte[] data) {
			Guard.ArgumentNotNull(data, "data");
			this.columnsCount = data[0];
			int count = this.columnsCount + 1;
			for (int i = 0; i < count; i++) {
				this.positions.Add(BitConverter.ToInt16(data, i * 2 + 1));
			}
			int currentPosition = 1 + (this.columnsCount + 1) * 2;
			count = (data.Length - currentPosition) / TableCellDescriptor.Size;
			for (int i = 0; i < count; i++) {
				this.cells.Add(TableCellDescriptor.FromByteArray(data, currentPosition + i * TableCellDescriptor.Size));
			}
		}
		public TableDefinitionOperand Clone() {
			TableDefinitionOperand result = new TableDefinitionOperand();
			result.columnsCount = this.columnsCount;
			int count = this.positions.Count;
			for (int i = 0; i < count; i++)
				result.positions.Add(this.positions[i]);
			count = this.cells.Count;
			for (int i = 0; i < count; i++)
				result.cells.Add(this.cells[i].Clone());
			return result;
		}
	}
	#endregion
	public class TLP {
		static Dictionary<short, string> knownStyles = GetKnownStyles();
		static Dictionary<short, string> GetKnownStyles() {
			Dictionary<short, string> result = new Dictionary<short, string>();
			result.Add(0, KnownStyleNames.NormalTable);
			result.Add(1, KnownStyleNames.TableSimple1);
			result.Add(2, KnownStyleNames.TableSimple2);
			result.Add(3, KnownStyleNames.TableSimple3);
			result.Add(4, KnownStyleNames.TableClassic1);
			result.Add(5, KnownStyleNames.TableClassic2);
			result.Add(6, KnownStyleNames.TableClassic3);
			result.Add(7, KnownStyleNames.TableClassic4);
			result.Add(8, KnownStyleNames.TableColorful1);
			result.Add(9, KnownStyleNames.TableColorful2);
			result.Add(10, KnownStyleNames.TableColorful3);
			result.Add(11, KnownStyleNames.TableColumns1);
			result.Add(12, KnownStyleNames.TableColumns2);
			result.Add(13, KnownStyleNames.TableColumns3);
			result.Add(14, KnownStyleNames.TableColumns4);
			result.Add(15, KnownStyleNames.TableColumns5);
			result.Add(16, KnownStyleNames.TableGrid1);
			result.Add(17, KnownStyleNames.TableGrid2);
			result.Add(18, KnownStyleNames.TableGrid3);
			result.Add(19, KnownStyleNames.TableGrid4);
			result.Add(20, KnownStyleNames.TableGrid5);
			result.Add(21, KnownStyleNames.TableGrid6);
			result.Add(22, KnownStyleNames.TableGrid7);
			result.Add(23, KnownStyleNames.TableGrid8);
			result.Add(24, KnownStyleNames.TableList1);
			result.Add(25, KnownStyleNames.TableList2);
			result.Add(26, KnownStyleNames.TableList3);
			result.Add(27, KnownStyleNames.TableList4);
			result.Add(28, KnownStyleNames.TableList5);
			result.Add(29, KnownStyleNames.TableList6);
			result.Add(30, KnownStyleNames.TableList7);
			result.Add(31, KnownStyleNames.TableList8);
			result.Add(32, KnownStyleNames.Table3DEffects1);
			result.Add(33, KnownStyleNames.Table3DEffects2);
			result.Add(34, KnownStyleNames.Table3DEffects3);
			result.Add(35, KnownStyleNames.TableContemporary);
			result.Add(36, KnownStyleNames.TableElegant);
			result.Add(37, KnownStyleNames.TableProfessional);
			result.Add(38, KnownStyleNames.TableSubtle1);
			result.Add(39, KnownStyleNames.TableSubtle2);
			return result;
		}
		public static TLP FromByteArray(byte[] data) {
			TLP result = new TLP();
			result.Read(data);
			return result;
		}
		public const byte Size = 0x03;
		short itl = -1;
		Fatl grfatl;
		protected internal Fatl TableStyleOptions { get { return grfatl; } set { grfatl = value; } }
		protected void Read(byte[] data) {
			Guard.ArgumentNotNull(data, "data");
			this.itl = BitConverter.ToInt16(data, 0);
			this.grfatl = (Fatl)BitConverter.ToInt16(data, 2);
		}
		public void Write(BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			writer.Write(this.itl);
			writer.Write((short)this.grfatl);
		}
		public void ApplyProperties(TableProperties properties) {
			TableStyle tableStyle = GetTableStyle(properties.DocumentModel);
			if (tableStyle == null) return;
			ApplyPropertiesCore(tableStyle, properties);
		}
		TableStyle GetTableStyle(DocumentModel documentModel) {
			string styleName;
			if (!knownStyles.TryGetValue(this.itl, out styleName))
				return null;
			return documentModel.TableStyles.GetStyleByName(styleName);
		}
		void ApplyPropertiesCore(TableStyle style, TableProperties properties) {
			properties.BeginInit();
			if ((grfatl & Fatl.UseBorders) == Fatl.UseBorders)
				properties.Borders.CopyFrom(style.TableProperties.Borders);
			properties.EndInit();
		}
	}
	[Flags]
	public enum Fatl : short {
		UseBorders = 0x0001,
		UseShading = 0x0002,
		UseFont = 0x0004,
		UseColor = 0x0008,
		UseBestFit = 0x0010,
		ApplyToHeaderRows = 0x0020,
		ApplyToLastRow = 0x0040,
		ApplyToHeaderColumns = 0x0080,
		ApplyToLastColumn = 0x0100
	}
	#region CellHideMarkOperand
	public class CellHideMarkOperand {
		#region static
		public static CellHideMarkOperand FromByteArray(byte[] data) {
			CellHideMarkOperand result = new CellHideMarkOperand();
			result.Read(data);
			return result;
		}
		#endregion
		#region Fields
		public const byte Size = 0x03;
		byte startIndex;
		byte endIndex;
		bool hideCellMark;
		#endregion
		#region Properties
		public byte StartIndex {
			get { return this.startIndex; }
			set { this.startIndex = value; }
		}
		public byte EndIndex {
			get { return this.endIndex; }
			set { this.endIndex = value; }
		}
		public bool HideCellMark {
			get { return this.hideCellMark; }
			set { this.hideCellMark = value; }
		}
		#endregion
		protected void Read(byte[] data) {
			this.startIndex = data[0];
			this.endIndex = (byte)(data[1] - 1);
			this.hideCellMark = data[2] == 1;
		}
		public void Write(BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			writer.Write(this.startIndex);
			writer.Write((byte)(this.endIndex + 1));
			writer.Write(this.hideCellMark);
		}
		public CellHideMarkOperand Clone() {
			CellHideMarkOperand result = new CellHideMarkOperand();
			result.startIndex = this.startIndex;
			result.endIndex = this.endIndex;
			result.hideCellMark = this.hideCellMark;
			return result;
		}
	}
	#endregion
}
