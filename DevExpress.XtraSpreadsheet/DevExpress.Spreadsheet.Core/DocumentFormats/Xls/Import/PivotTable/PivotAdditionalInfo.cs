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
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Office.Utils;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region SxDataClass
	public enum SxDataClass {
		View = 0x00,
		Field = 0x01,
		Hierarchy = 0x02,
		Cache = 0x03,
		CacheField = 0x04,
		Qsi = 0x05,
		Query = 0x06,
		GroupLevel = 0x07,
		Group = 0x08,
		CacheItem = 0x09,
		Rule = 0x0c,
		Filter = 0x0d,
		OLAPDimensions = 0x10,
		AutoSort = 0x12,
		OLAPMeasureGroups = 0x13,
		OLAPMeasureGroup = 0x14,
		Field12 = 0x17,
		ConditionalFormattings = 0x1a,
		ConditionalFormattingRule = 0x1b,
		Filters12 = 0x1c,
		Filter12 = 0x1d
	}
	#endregion
	#region SxDataType
	public enum SxDataType {
		Id = 0x00,
		VerUpdInv = 0x01,
		Ver10Info = 0x02,
		CalcMember = 0x03,
		XMLSource = 0x04,
		SrcDataFile = 0x05,
		SrcCommFile = 0x06,
		ReconnCond = 0x07,
		Property = 0x05,
		GroupLevelInfo = 0x06,
		GroupInfo = 0x07,
		Member = 0x08,
		FilterMember = 0x09,
		CalcMemString = 0x0a,
		MemberCaption = 0x11,
		Rule = 0x13,
		Filter = 0x14,
		FilterItem = 0x15,
		VerSXMacro = 0x18,
		Ver12Info = 0x19,
		OLAPDimensionsMapping = 0x1a,
		Hierarchy = 0x1c,
		SetParentUnique = 0x1d,
		TableStyleClient = 0x1e,
		UserCaption = 0x1f,
		IconSet = 0x20,
		CompactRowHdr = 0x21,
		CompactColumnHdr = 0x22,
		OLAPMeasureGroupMapping = 0x23,
		MeasureGroup = 0x24,
		DisplayFolder = 0x25,
		ParentKPI = 0x26,
		ValueMetadataMapping = 0x26, 
		KPIValue = 0x27,
		KPIGoal = 0x28,
		KPIStatus = 0x29,
		KPITrend = 0x2a,
		KPIWeight = 0x2b,
		KPITime = 0x2c,
		ItemCount = 0x2d,
		DisplayName = 0x2e,
		Caption = 0x2f,
		MemberPropMap = 0x30,
		MemberPropMapCount = 0x31,
		ItemMemberPropMap = 0x32,
		ItemMemberPropMapCount = 0x33,
		InvRefreshReal = 0x34,
		CondFmtRule = 0x35,
		AutoShow = 0x37,
		SxFilter = 0x38,
		SxFilterDesc = 0x39,
		SxFilterValue1 = 0x3a,
		SxFilterValue2 = 0x3b,
		XlsFilter = 0x3c,
		XlsFilterValue1 = 0x3d,
		XlsFilterValue2 = 0x3e,
		FilterMember12 = 0x3f,
		PropName = 0x40,
		Info12 = 0x41,
		End = 0xff
	}
	#endregion
	#region IXlsPivotAddl
	public interface IXlsPivotAddl {
		SxDataClass DataClass { get; }
		SxDataType DataType { get; }
		void Read(XlsReader reader, XlsContentBuilder contentBuilder);
		void Execute(XlsContentBuilder contentBuilder);
		void Write(BinaryWriter writer);
		int GetSize();
	}
	#endregion
	#region XlsPivotAddlHeader
	public class XlsPivotAddlHeader : FutureRecordHeaderFlagsBase {
		public static XlsPivotAddlHeader FromStream(XlsReader reader) {
			XlsPivotAddlHeader result = new XlsPivotAddlHeader();
			result.Read(reader);
			return result;
		}
		public SxDataClass DataClass { get; set; }
		public SxDataType DataType { get; set; }
		protected override void ReadCore(XlsReader reader) {
			base.ReadCore(reader);
			DataClass = (SxDataClass)reader.ReadByte();
			DataType = (SxDataType)reader.ReadByte();
		}
		protected override void WriteCore(BinaryWriter writer) {
			base.WriteCore(writer);
			writer.Write((byte)DataClass);
			writer.Write((byte)DataType);
		}
		public override short GetSize() {
			return 6;
		}
	}
	#endregion
	#region XlsPivotAddlBase (abstract)
	public abstract class XlsPivotAddlBase : IXlsPivotAddl {
		public abstract SxDataClass DataClass { get; }
		public abstract SxDataType DataType { get; }
		public virtual void Read(XlsReader reader, XlsContentBuilder contentBuilder) {
			reader.ReadInt32(); 
			reader.ReadInt16(); 
		}
		public virtual void Write(BinaryWriter writer) {
			writer.Write((int)0); 
			writer.Write((short)0); 
		}
		public virtual void Execute(XlsContentBuilder contentBuilder) {
		}
		public virtual int GetSize() {
			return 6;
		}
	}
	#endregion
	#region XlsPivotAddlStringBase (abstract)
	public abstract class XlsPivotAddlStringBase : XlsPivotAddlBase {
		int totalCount;
		XLUnicodeString innerString = new XLUnicodeString();
		#region Properties
		public int TotalCount {
			get { return totalCount; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "TotalCount");
				totalCount = value;
			}
		}
		public string Value {
			get { return innerString.Value; }
			set {
				ValueChecker.CheckLength(value, 255, "Name");
				innerString.Value = value;
			}
		}
		#endregion
		public override void Read(XlsReader reader, XlsContentBuilder contentBuilder) {
			TotalCount = reader.ReadInt32();
			reader.ReadUInt16(); 
			innerString = XLUnicodeString.FromStream(reader);
		}
		public override void Write(BinaryWriter writer) {
			writer.Write(TotalCount);
			writer.Write((ushort)0);
			innerString.Write(writer);
		}
		public override int GetSize() {
			return innerString.Length + 6;
		}
	}
	#endregion
	#region XlsPivotAddlIntBase (abstract)
	public abstract class XlsPivotAddlIntBase : XlsPivotAddlBase {
		int innerValue;
		public int Value {
			get { return innerValue; }
			set {
				CheckValue(value);
				innerValue = value;
			}
		}
		public override void Read(XlsReader reader, XlsContentBuilder contentBuilder) {
			Value = reader.ReadInt32();
			reader.ReadUInt16(); 
		}
		public override void Write(BinaryWriter writer) {
			writer.Write(Value);
			writer.Write((ushort)0); 
		}
		protected virtual void CheckValue(int value) {
		}
	}
	#endregion
	#region XlsPivotAddlIdBase (abstract)
	public abstract class XlsPivotAddlIdBase : XlsPivotAddlStringBase {
		public override SxDataType DataType { get { return SxDataType.Id; } }
	}
	#endregion
	#region XlsPivotAddlEndBase (abstract)
	public abstract class XlsPivotAddlEndBase : XlsPivotAddlBase {
		public override SxDataType DataType { get { return SxDataType.End; } }
	}
	#endregion
	#region XlsPivotAddlVersionBase (abstract)
	public abstract class XlsPivotAddlVersionBase : XlsPivotAddlBase {
		public byte Version { get; set; }
		public override void Read(XlsReader reader, XlsContentBuilder contentBuilder) {
			Version = reader.ReadByte();
			reader.ReadByte(); 
			reader.ReadUInt16(); 
			reader.ReadUInt16(); 
		}
		public override void Write(BinaryWriter writer) {
			writer.Write((byte)Version);
			writer.Write((byte)0); 
			writer.Write((ushort)0); 
			writer.Write((ushort)0); 
		}
	}
	#endregion
	#region XlsPivotAddlVersionUpdateBase (abstract)
	public abstract class XlsPivotAddlVersionUpdateBase : XlsPivotAddlVersionBase {
		public override SxDataType DataType { get { return SxDataType.VerUpdInv; } }
	}
	#endregion
	#region XlsPivotAddlViewId
	public class XlsPivotAddlViewId : XlsPivotAddlIdBase {
		public override SxDataClass DataClass { get { return SxDataClass.View; } }
	}
	#endregion
	#region XlsPivotAddlViewVersionUpdate
	public class XlsPivotAddlViewVersionUpdate : XlsPivotAddlVersionUpdateBase {
		public override SxDataClass DataClass { get { return SxDataClass.View; } }
	}
	#endregion
	#region XlsPivotAddlViewVersion10
	public class XlsPivotAddlViewVersion10 : XlsPivotAddlBase {
		byte version;
		#region Properties
		public override SxDataClass DataClass { get { return SxDataClass.View; } }
		public override SxDataType DataType { get { return SxDataType.Ver10Info; } }
		public byte Version {
			get { return version; }
			set {
				ValueChecker.CheckValue(value, 0, 254, "Version");
				version = value;
			}
		}
		public bool DisplayImmediateItems { get; set; }
		public bool EnableDataEditing { get; set; }
		public bool DisableFieldList { get; set; }
		public bool ReenterOnLoadOnce { get; set; }
		public bool NotViewCalculatedMembers { get; set; }
		public bool NotVisualTotals { get; set; }
		public bool PageMultipleItemLabel { get; set; }
		public bool TensorFillColorValue { get; set; }
		public bool HideDropDownData { get; set; }
		#endregion
		public override void Read(XlsReader reader, XlsContentBuilder contentBuilder) {
			version = reader.ReadByte();
			ushort bitwiseField = reader.ReadUInt16();
			DisplayImmediateItems = (bitwiseField & 0x0001) != 0;
			EnableDataEditing = (bitwiseField & 0x0002) != 0;
			DisableFieldList = (bitwiseField & 0x0004) != 0;
			ReenterOnLoadOnce = (bitwiseField & 0x0008) != 0;
			NotViewCalculatedMembers = (bitwiseField & 0x0010) != 0;
			NotVisualTotals = (bitwiseField & 0x0020) != 0;
			PageMultipleItemLabel = (bitwiseField & 0x0040) != 0;
			TensorFillColorValue = (bitwiseField & 0x0080) != 0;
			HideDropDownData = (bitwiseField & 0x0100) != 0;
			reader.ReadByte(); 
			reader.ReadUInt16(); 
		}
		public override void Write(BinaryWriter writer) {
			writer.Write((byte)version);
			ushort bitwiseField = 0;
			if(DisplayImmediateItems)
				bitwiseField |= 0x0001;
			if(EnableDataEditing)
				bitwiseField |= 0x0002;
			if(DisableFieldList)
				bitwiseField |= 0x0004;
			if(ReenterOnLoadOnce)
				bitwiseField |= 0x0008;
			if(NotViewCalculatedMembers)
				bitwiseField |= 0x0010;
			if(NotVisualTotals)
				bitwiseField |= 0x0020;
			if(PageMultipleItemLabel)
				bitwiseField |= 0x0040;
			if(TensorFillColorValue)
				bitwiseField |= 0x0080;
			if(HideDropDownData)
				bitwiseField |= 0x0100;
			writer.Write(bitwiseField);
			writer.Write((byte)0); 
			writer.Write((ushort)0); 
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
			if (contentBuilder != null && contentBuilder.CurrentBuilderPivotView != null) {
				PivotTable pivotTable = contentBuilder.CurrentBuilderPivotView.PivotTable;
				pivotTable.CreatedVersion = Version;
				pivotTable.ItemPrintTitles = DisplayImmediateItems;
				pivotTable.EditData = EnableDataEditing;
				pivotTable.DisableFieldList = DisableFieldList;
				pivotTable.ShowCalcMbrs = !NotViewCalculatedMembers;
				pivotTable.VisualTotals = !NotVisualTotals;
				pivotTable.ShowMultipleLabel = PageMultipleItemLabel;
				pivotTable.ShowDataDropDown = !HideDropDownData;
			}
		}
	}
	#endregion
	#region XlsPivotAddlViewCalcMember
	public class XlsPivotAddlViewCalcMember : XlsPivotAddlBase {
		#region Fields
		XLUnicodeString name = new XLUnicodeString();
		XLUnicodeString mdxFormula = new XLUnicodeString();
		XLUnicodeString memberName = new XLUnicodeString();
		XLUnicodeString sourceHierarchy = new XLUnicodeString();
		XLUnicodeString parentUnique = new XLUnicodeString();
		#endregion
		#region Properties
		public override SxDataClass DataClass { get { return SxDataClass.View; } }
		public override SxDataType DataType { get { return SxDataType.CalcMember; } }
		public bool LongFormula { get; set; }
		public bool OLAPNamedSet { get; set; }
		public string Name {
			get { return name.Value; }
			set {
				ValueChecker.CheckLength(value, 255, "Name");
				name.Value = value;
			}
		}
		public string MDXFormula {
			get { return mdxFormula.Value; }
			set {
				ValueChecker.CheckLength(value, 255, "MDXFormula");
				mdxFormula.Value = value;
			}
		}
		public string MemberName {
			get { return memberName.Value; }
			set {
				ValueChecker.CheckLength(value, 255, "MemberName");
				memberName.Value = value;
			}
		}
		public string SourceHierarchy {
			get { return sourceHierarchy.Value; }
			set {
				ValueChecker.CheckLength(value, 255, "SourceHierarchy");
				sourceHierarchy.Value = value;
			}
		}
		public string ParentUnique {
			get { return parentUnique.Value; }
			set {
				ValueChecker.CheckLength(value, 255, "ParentUnique");
				parentUnique.Value = value;
			}
		}
		public int SolveOrder { get; set; }
		#endregion
		public override void Read(XlsReader reader, XlsContentBuilder contentBuilder) {
			ushort bitwiseField = reader.ReadUInt16();
			bool hasParentUnique = (bitwiseField & 0x0001) != 0;
			bool hasMemberName = (bitwiseField & 0x0002) != 0;
			bool hasSourceHeirarchy = (bitwiseField & 0x0004) != 0;
			LongFormula = (bitwiseField & 0x0008) != 0;
			OLAPNamedSet = (bitwiseField & 0x0100) != 0;
			reader.ReadInt32(); 
			name = XLUnicodeString.FromStream(reader);
			if (!LongFormula)
				mdxFormula = XLUnicodeString.FromStream(reader);
			else
				mdxFormula.Value = string.Empty;
			if (hasMemberName)
				memberName = XLUnicodeString.FromStream(reader);
			else
				memberName.Value = string.Empty;
			if (hasSourceHeirarchy)
				sourceHierarchy = XLUnicodeString.FromStream(reader);
			else
				sourceHierarchy.Value = string.Empty;
			if (hasParentUnique)
				parentUnique = XLUnicodeString.FromStream(reader);
			else
				parentUnique.Value = string.Empty;
			SolveOrder = reader.ReadInt32();
		}
		public override void Write(BinaryWriter writer) {
			bool hasParentUnique = !string.IsNullOrEmpty(parentUnique.Value);
			bool hasMemberName = !string.IsNullOrEmpty(memberName.Value);
			bool hasSourceHeirarchy = !string.IsNullOrEmpty(sourceHierarchy.Value);
			ushort bitwiseField = 0;
			if(hasParentUnique)
				bitwiseField |= 0x0001;
			if(hasMemberName)
				bitwiseField |= 0x0002;
			if(hasSourceHeirarchy)
				bitwiseField |= 0x0004;
			if(LongFormula)
				bitwiseField |= 0x0008;
			if(OLAPNamedSet)
				bitwiseField |= 0x0100;
			writer.Write(bitwiseField);
			writer.Write((int)0); 
			name.Write(writer);
			if (!LongFormula)
				mdxFormula.Write(writer);
			if (hasMemberName)
				memberName.Write(writer);
			if (hasSourceHeirarchy)
				sourceHierarchy.Write(writer);
			if (hasParentUnique)
				parentUnique.Write(writer);
			writer.Write(SolveOrder);
		}
		public override int GetSize() {
			bool hasParentUnique = !string.IsNullOrEmpty(parentUnique.Value);
			bool hasMemberName = !string.IsNullOrEmpty(memberName.Value);
			bool hasSourceHeirarchy = !string.IsNullOrEmpty(sourceHierarchy.Value);
			int result = 10 + name.Length;
			if (!LongFormula)
				result += mdxFormula.Length;
			if (hasMemberName)
				result += memberName.Length;
			if (hasSourceHeirarchy)
				result += sourceHierarchy.Length;
			if (hasParentUnique)
				result += parentUnique.Length;
			return result;
		}
	}
	#endregion
	#region XlsPivotAddlViewCalcMemString
	public class XlsPivotAddlViewCalcMemString : XlsPivotAddlStringBase {
		public override SxDataClass DataClass { get { return SxDataClass.View; } }
		public override SxDataType DataType { get { return SxDataType.CalcMemString; } }
	}
	#endregion
	#region XlsPivotAddlViewVersion12
	public class XlsPivotAddlViewVersion12 : XlsPivotAddlBase {
		#region Properties
		public override SxDataClass DataClass { get { return SxDataClass.View; } }
		public override SxDataType DataType { get { return SxDataType.Ver12Info; } }
		public bool DefaultCompact { get; set; }
		public bool DefaultOutline { get; set; }
		public bool OutlineData { get; set; }
		public bool CompactData { get; set; }
		public bool NewDropZones { get; set; }
		public bool Published { get; set; }
		public bool TurnOffImmersive { get; set; }
		public bool SingleFilterPerField { get; set; }
		public bool NonDefaultSortInFieldList { get; set; }
		public bool DontUseCustomLists { get; set; }
		public bool HideDrillIndicators { get; set; }
		public bool PrintDrillIndicators { get; set; }
		public bool MemPropsInTips { get; set; }
		public bool NoPivotTips { get; set; }
		public int IndentInc { get; set; }
		public bool NoHeaders { get; set; }
		#endregion
		public override void Read(XlsReader reader, XlsContentBuilder contentBuilder) {
			ushort bitwiseField = reader.ReadUInt16();
			DefaultCompact = (bitwiseField & 0x0001) != 0;
			DefaultOutline = (bitwiseField & 0x0002) != 0;
			OutlineData = (bitwiseField & 0x0004) != 0;
			CompactData = (bitwiseField & 0x0008) != 0;
			NewDropZones = (bitwiseField & 0x0010) != 0;
			Published = (bitwiseField & 0x0020) != 0;
			TurnOffImmersive = (bitwiseField & 0x0040) != 0;
			SingleFilterPerField = (bitwiseField & 0x0080) != 0;
			NonDefaultSortInFieldList = (bitwiseField & 0x0100) != 0;
			DontUseCustomLists = (bitwiseField & 0x0400) != 0;
			bitwiseField = reader.ReadUInt16();
			HideDrillIndicators = (bitwiseField & 0x0010) != 0;
			PrintDrillIndicators = (bitwiseField & 0x0020) != 0;
			MemPropsInTips = (bitwiseField & 0x0040) != 0;
			NoPivotTips = (bitwiseField & 0x0080) != 0;
			IndentInc = (bitwiseField & 0x7f00) >> 8;
			NoHeaders = (bitwiseField & 0x8000) != 0;
			reader.ReadUInt16(); 
		}
		public override void Write(BinaryWriter writer) {
			ushort bitwiseField = 0;
			if(DefaultCompact)
				bitwiseField |= 0x0001;
			if(DefaultOutline)
				bitwiseField |= 0x0002;
			if(OutlineData)
				bitwiseField |= 0x0004;
			if(CompactData)
				bitwiseField |= 0x0008;
			if(NewDropZones)
				bitwiseField |= 0x0010;
			if(Published)
				bitwiseField |= 0x0020;
			if(TurnOffImmersive)
				bitwiseField |= 0x0040;
			if(SingleFilterPerField)
				bitwiseField |= 0x0080;
			if(NonDefaultSortInFieldList)
				bitwiseField |= 0x0100;
			if(DontUseCustomLists)
				bitwiseField |= 0x0400;
			writer.Write(bitwiseField);
			bitwiseField = (ushort)((IndentInc & 0x07f) << 8);
			if(HideDrillIndicators)
				bitwiseField |= 0x0010;
			if(PrintDrillIndicators)
				bitwiseField |= 0x0020;
			if(MemPropsInTips)
				bitwiseField |= 0x0040;
			if(NoPivotTips)
				bitwiseField |= 0x0080;
			if(NoHeaders)
				bitwiseField |= 0x8000;
			writer.Write(bitwiseField);
			writer.Write((ushort)0); 
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
			if (contentBuilder != null && contentBuilder.CurrentBuilderPivotView != null) {
				PivotTable pivotTable = contentBuilder.CurrentBuilderPivotView.PivotTable;
				pivotTable.Compact = DefaultCompact;
				pivotTable.Outline = DefaultOutline;
				pivotTable.SetOutlineData(OutlineData);
				pivotTable.SetCompactData(CompactData);
				pivotTable.GridDropZones = !NewDropZones;
				pivotTable.Published = Published;
				pivotTable.Immersive = !TurnOffImmersive;
				pivotTable.MultipleFieldFilters = !SingleFilterPerField;
				pivotTable.FieldListSortAscending = NonDefaultSortInFieldList;
				pivotTable.CustomListSort = !DontUseCustomLists;
				pivotTable.SetShowDrill(!HideDrillIndicators);
				pivotTable.PrintDrill = PrintDrillIndicators;
				pivotTable.ShowMemberPropertyTips = MemPropsInTips;
				pivotTable.ShowDataTips = !NoPivotTips;
				if (IndentInc != 0)
					pivotTable.SetIndent(IndentInc);
				pivotTable.SetShowHeaders(!NoHeaders);
			}
		}
	}
	#endregion
	#region XlsPivotAddlViewTableStyleClient
	public class XlsPivotAddlViewTableStyleClient : XlsPivotAddlBase {
		LPWideString innerString = new LPWideString();
		#region Properties
		public override SxDataClass DataClass { get { return SxDataClass.View; } }
		public override SxDataType DataType { get { return SxDataType.TableStyleClient; } }
		public bool LastColumn { get; set; }
		public bool RowStrips { get; set; }
		public bool ColumnStrips { get; set; }
		public bool RowHeaders { get; set; }
		public bool ColumnHeaders { get; set; }
		public bool DefaultStyle { get; set; }
		public string Name {
			get { return innerString.Value; }
			set {
				ValueChecker.CheckLength(value, 255, "Name");
				innerString.Value = value;
			}
		}
		#endregion
		public override void Read(XlsReader reader, XlsContentBuilder contentBuilder) {
			base.Read(reader, contentBuilder);
			ushort bitwiseField = reader.ReadUInt16();
			LastColumn = (bitwiseField & 0x0002) != 0;
			RowStrips = (bitwiseField & 0x0004) != 0;
			ColumnStrips = (bitwiseField & 0x0008) != 0;
			RowHeaders = (bitwiseField & 0x0010) != 0;
			ColumnHeaders = (bitwiseField & 0x0020) != 0;
			DefaultStyle = (bitwiseField & 0x0040) != 0;
			innerString = LPWideString.FromStream(reader);
		}
		public override void Write(BinaryWriter writer) {
			base.Write(writer);
			ushort bitwiseField = 0;
			if(LastColumn)
				bitwiseField |= 0x0002;
			if(RowStrips)
				bitwiseField |= 0x0004;
			if(ColumnStrips)
				bitwiseField |= 0x0008;
			if(RowHeaders)
				bitwiseField |= 0x0010;
			if(ColumnHeaders)
				bitwiseField |= 0x0020;
			if(DefaultStyle)
				bitwiseField |= 0x0040;
			writer.Write(bitwiseField);
			innerString.Write(writer);
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
			if (contentBuilder.CurrentBuilderPivotView != null && contentBuilder.CurrentBuilderPivotView.PivotTable != null) {
				PivotTableStyleInfo styleInfo = contentBuilder.CurrentBuilderPivotView.PivotTable.StyleInfo;
				styleInfo.StyleName = Name;
				styleInfo.ShowColumnHeaders = ColumnHeaders;
				styleInfo.ShowColumnStripes = ColumnStrips;
				styleInfo.ShowRowHeaders = RowHeaders;
				styleInfo.ShowRowStripes = RowStrips;
				styleInfo.ShowLastColumn = LastColumn;
			}
		}
		public override int GetSize() {
			return 8 + innerString.Length;
		}
	}
	#endregion
	#region XlsPivotAddlViewCompactRowHeader
	public class XlsPivotAddlViewCompactRowHeader : XlsPivotAddlStringBase {
		public override SxDataClass DataClass { get { return SxDataClass.View; } }
		public override SxDataType DataType { get { return SxDataType.CompactRowHdr; } }
	}
	#endregion
	#region XlsPivotAddlViewCompactColumnHeader
	public class XlsPivotAddlViewCompactColumnHeader : XlsPivotAddlStringBase {
		public override SxDataClass DataClass { get { return SxDataClass.View; } }
		public override SxDataType DataType { get { return SxDataType.CompactColumnHdr; } }
	}
	#endregion
	#region XlsPivotAddlViewValueMetadataMapping
	public class XlsPivotAddlViewValueMetadataMapping : XlsPivotAddlBase {
		#region Properties
		public override SxDataClass DataClass { get { return SxDataClass.View; } }
		public override SxDataType DataType { get { return SxDataType.ValueMetadataMapping; } }
		public int EntryIndex { get; set; }
		public int RecordIndex { get; set; }
		#endregion
		public override void Read(XlsReader reader, XlsContentBuilder contentBuilder) {
			base.Read(reader, contentBuilder);
			EntryIndex = reader.ReadInt32();
			RecordIndex = reader.ReadInt32();
		}
		public override void Write(BinaryWriter writer) {
			base.Write(writer);
			writer.Write(EntryIndex);
			writer.Write(RecordIndex);
		}
		public override int GetSize() {
			return 14;
		}
	}
	#endregion
	#region XlsPivotAddlViewEnd
	public class XlsPivotAddlViewEnd : XlsPivotAddlEndBase {
		public override SxDataClass DataClass { get { return SxDataClass.View; } }
	}
	#endregion
	#region XlsPivotAddlFieldId
	public class XlsPivotAddlFieldId : XlsPivotAddlIdBase {
		public override SxDataClass DataClass { get { return SxDataClass.Field; } }
		public override void Execute(XlsContentBuilder contentBuilder) {
			if (contentBuilder != null && contentBuilder.CurrentBuilderPivotView != null) {
				contentBuilder.CurrentBuilderPivotView.SetPivotFieldByName(Value);
			}
		}
	}
	#endregion
	#region XlsPivotAddlFieldVersion10
	public class XlsPivotAddlFieldVersion10 : XlsPivotAddlBase {
		public override SxDataClass DataClass { get { return SxDataClass.Field; } }
		public override SxDataType DataType { get { return SxDataType.Ver10Info; } }
		public bool HideDropDown { get; set; }
		public override void Read(XlsReader reader, XlsContentBuilder contentBuilder) {
			ushort bitwiseField = reader.ReadUInt16();
			HideDropDown = (bitwiseField & 0x0001) != 0;
			reader.ReadInt32(); 
		}
		public override void Write(BinaryWriter writer) {
			ushort bitwiseField = 0;
			if(HideDropDown)
				bitwiseField |= 0x0001;
			writer.Write(bitwiseField);
			writer.Write((int)0); 
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
			if (contentBuilder != null && contentBuilder.CurrentBuilderPivotView != null) {
				contentBuilder.CurrentBuilderPivotView.PivotField.ShowDropDowns = !HideDropDown;
			}
		}
	}
	#endregion
	#region XlsPivotAddlFieldEnd
	public class XlsPivotAddlFieldEnd : XlsPivotAddlEndBase {
		public override SxDataClass DataClass { get { return SxDataClass.Field; } }
	}
	#endregion
	#region XlsPivotAddlField12Id
	public class XlsPivotAddlField12Id : XlsPivotAddlIdBase {
		public override SxDataClass DataClass { get { return SxDataClass.Field12; } }
		public override void Execute(XlsContentBuilder contentBuilder) {
			if (contentBuilder != null && contentBuilder.CurrentBuilderPivotView != null) {
				contentBuilder.CurrentBuilderPivotView.SetPivotFieldByName(Value);
			}
		}
	}
	#endregion
	#region XlsPivotAddlField12VersionUpdate
	public class XlsPivotAddlField12VersionUpdate : XlsPivotAddlVersionUpdateBase {
		public override SxDataClass DataClass { get { return SxDataClass.Field12; } }
	}
	#endregion
	#region XlsPivotAddlField12MemberCaption
	public class XlsPivotAddlField12MemberCaption : XlsPivotAddlStringBase {
		public override SxDataClass DataClass { get { return SxDataClass.Field12; } }
		public override SxDataType DataType { get { return SxDataType.MemberCaption; } }
	}
	#endregion
	#region XlsPivotAddlField12HierarchyIndex
	public class XlsPivotAddlField12HierarchyIndex : XlsPivotAddlIntBase {
		public override SxDataClass DataClass { get { return SxDataClass.Field12; } }
		public override SxDataType DataType { get { return SxDataType.Hierarchy; } }
	}
	#endregion
	#region XlsPivotAddlField12AutoShow
	public class XlsPivotAddlField12AutoShow : XlsPivotAddlBase {
		int innerValue = 1;
		public override SxDataClass DataClass { get { return SxDataClass.Field12; } }
		public override SxDataType DataType { get { return SxDataType.AutoShow; } }
		public int Value {
			get { return innerValue; }
			set {
				ValueChecker.CheckValue(value, 1, Int32.MaxValue, "Value");
				innerValue = value;
			}
		}
		public override void Read(XlsReader reader, XlsContentBuilder contentBuilder) {
			Value = reader.ReadInt32();
			reader.ReadUInt16(); 
		}
		public override void Write(BinaryWriter writer) {
			writer.Write(Value);
			writer.Write((ushort)0); 
		}
	}
	#endregion
	#region XlsPivotAddlField12Version12
	public class XlsPivotAddlField12Version12 : XlsPivotAddlBase {
		#region Properties
		public override SxDataClass DataClass { get { return SxDataClass.Field12; } }
		public override SxDataType DataType { get { return SxDataType.Ver12Info; } }
		public bool HiddenLevel { get; set; }
		public bool UseMemPropCaption { get; set; }
		public bool Compact { get; set; }
		public bool NotAutoSortDeffered { get; set; }
		public bool FilterInclusive { get; set; }
		#endregion
		public override void Read(XlsReader reader, XlsContentBuilder contentBuilder) {
			ushort bitwiseField = reader.ReadUInt16();
			HiddenLevel = (bitwiseField & 0x0002) != 0;
			UseMemPropCaption = (bitwiseField & 0x0004) != 0;
			Compact = (bitwiseField & 0x0008) != 0;
			NotAutoSortDeffered = (bitwiseField & 0x0010) != 0;
			FilterInclusive = (bitwiseField & 0x0020) != 0;
			reader.ReadUInt16(); 
			reader.ReadUInt16(); 
		}
		public override void Write(BinaryWriter writer) {
			ushort bitwiseField = 0;
			if(HiddenLevel)
				bitwiseField |= 0x0002;
			if(UseMemPropCaption)
				bitwiseField |= 0x0004;
			if(Compact)
				bitwiseField |= 0x0008;
			if(NotAutoSortDeffered)
				bitwiseField |= 0x0010;
			if(FilterInclusive)
				bitwiseField |= 0x0020;
			writer.Write(bitwiseField);
			writer.Write((ushort)0); 
			writer.Write((ushort)0); 
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
			if (contentBuilder != null && contentBuilder.CurrentBuilderPivotView != null && contentBuilder.CurrentBuilderPivotView.PivotField != null) {
				PivotField field = contentBuilder.CurrentBuilderPivotView.PivotField;
				field.HiddenLevel = HiddenLevel;
				field.ShowPropAsCaption = UseMemPropCaption;
				field.SetCompact(Compact);
				field.NonAutoSortDefault = NotAutoSortDeffered;
				field.IncludeNewItemsInFilter = !FilterInclusive;
			}
		}
	}
	#endregion
	#region XlsPivotAddlField12End
	public class XlsPivotAddlField12End : XlsPivotAddlEndBase {
		public override SxDataClass DataClass { get { return SxDataClass.Field12; } }
	}
	#endregion
	#region XlsPivotAddlCacheId
	public class XlsPivotAddlCacheId : XlsPivotAddlBase {
		public override SxDataClass DataClass { get { return SxDataClass.Cache; } }
		public override SxDataType DataType { get { return SxDataType.Id; } }
		public int Id { get; set; }
		public override void Read(XlsReader reader, XlsContentBuilder contentBuilder) {
			Id = reader.ReadInt32();
			reader.ReadUInt16(); 
		}
		public override void Write(BinaryWriter writer) {
			writer.Write(Id);
			writer.Write((ushort)0); 
		}
	}
	#endregion
	#region XlsPivotAddlCacheVersionUpdate
	public class XlsPivotAddlCacheVersionUpdate : XlsPivotAddlVersionUpdateBase {
		public override SxDataClass DataClass { get { return SxDataClass.Cache; } }
		public override void Execute(XlsContentBuilder contentBuilder) {
			if (contentBuilder == null || contentBuilder.CurrentPivotCacheBuilder.IsEmpty)
				return;
			XlsPivotCacheBuilderBase builder = contentBuilder.CurrentPivotCacheBuilder as XlsPivotCacheBuilderBase;
			builder.IgnoreAddlCache12Records = Version != 0xff && Version >= builder.VersionCacheLastRefresh;
		}
	}
	#endregion
	#region XlsPivotAddlCacheVersion10
	public class XlsPivotAddlCacheVersion10 : XlsPivotAddlBase {
		int maxUnusedItems = -1;
		#region Properties
		public override SxDataClass DataClass { get { return SxDataClass.Cache; } }
		public override SxDataType DataType { get { return SxDataType.Ver10Info; } }
		public int MaxUnusedItems {
			get { return maxUnusedItems; }
			set {
				ValueChecker.CheckValue(value, -1, 1048576, "MaxUnusedItems");
				maxUnusedItems = value;
			}
		}
		public byte VersionCacheLastRefresh { get; set; }
		public byte VersionCacheRefreshableMin { get; set; }
		public double DateLastRefreshed { get; set; }
		#endregion
		public override void Read(XlsReader reader, XlsContentBuilder contentBuilder) {
			base.Read(reader, contentBuilder);
			MaxUnusedItems = reader.ReadInt32();
			VersionCacheLastRefresh = reader.ReadByte();
			VersionCacheRefreshableMin = reader.ReadByte();
			DateLastRefreshed = reader.ReadDouble();
			reader.ReadUInt16(); 
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
			if (contentBuilder == null || contentBuilder.CurrentPivotCacheBuilder.IsEmpty)
				return;
			XlsPivotCacheBuilderBase builder = contentBuilder.CurrentPivotCacheBuilder as XlsPivotCacheBuilderBase;
			builder.MaxUnusedItems = MaxUnusedItems;
			builder.VersionCacheLastRefresh = VersionCacheLastRefresh;
			builder.VersionCacheRefreshableMin = VersionCacheRefreshableMin;
			builder.DateLastRefreshed = GetRefreshedDate(contentBuilder.DocumentModel.DataContext);
		}
		DateTime GetRefreshedDate(WorkbookDataContext context) {
			if (WorkbookDataContext.IsErrorDateTimeSerial(DateLastRefreshed, context.DateSystem))
				return DateLastRefreshed < 0 ? DateTime.MinValue : DateTime.MaxValue;
			else
				return context.FromDateTimeSerial(DateLastRefreshed);
		}
		public override void Write(BinaryWriter writer) {
			base.Write(writer);
			writer.Write(MaxUnusedItems);
			writer.Write(VersionCacheLastRefresh);
			writer.Write(VersionCacheRefreshableMin);
			writer.Write(DateLastRefreshed);
			writer.Write((ushort)0); 
		}
		public override int GetSize() {
			return 22;
		}
	}
	#endregion
	#region XlsPivotAddlCacheVerMacro
	public class XlsPivotAddlCacheVerMacro : XlsPivotAddlVersionBase {
		public override SxDataClass DataClass { get { return SxDataClass.Cache; } }
		public override SxDataType DataType { get { return SxDataType.VerSXMacro; } }
		public override void Execute(XlsContentBuilder contentBuilder) {
			if (contentBuilder == null || contentBuilder.CurrentPivotCacheBuilder.IsEmpty)
				return;
			((XlsPivotCacheBuilderBase)contentBuilder.CurrentPivotCacheBuilder).CreatedVersion = Version;
		}
	}
	#endregion
	#region XlsPivotAddlCacheInvRefreshReal
	public class XlsPivotAddlCacheInvRefreshReal : XlsPivotAddlBase {
		#region Properties
		public override SxDataClass DataClass { get { return SxDataClass.Cache; } }
		public override SxDataType DataType { get { return SxDataType.InvRefreshReal; } }
		public bool EnableRefresh { get; set; }
		public bool Invalid { get; set; }
		#endregion
		public override void Read(XlsReader reader, XlsContentBuilder contentBuilder) {
			ushort bitwiseField = reader.ReadUInt16();
			EnableRefresh = (bitwiseField & 0x0001) != 0;
			Invalid = (bitwiseField & 0x0002) != 0;
			reader.ReadInt32(); 
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
			if (contentBuilder == null || contentBuilder.CurrentPivotCacheBuilder.IsEmpty)
				return;
			XlsPivotCacheBuilderBase builder = contentBuilder.CurrentPivotCacheBuilder as XlsPivotCacheBuilderBase;
			if (builder.IgnoreAddlCache12Records)
				return;
			builder.Invalid = Invalid;
			builder.EnableRefresh = EnableRefresh;
		}
		public override void Write(BinaryWriter writer) {
			ushort bitwiseField = 0;
			if (EnableRefresh)
				bitwiseField |= 0x0001;
			if (Invalid)
				bitwiseField |= 0x0002;
			writer.Write(bitwiseField);
			writer.Write((int)0); 
		}
	}
	#endregion
	#region XlsPivotAddlCacheInfo12
	public class XlsPivotAddlCacheInfo12 : XlsPivotAddlBase {
		#region Properties
		public override SxDataClass DataClass { get { return SxDataClass.Cache; } }
		public override SxDataType DataType { get { return SxDataType.Info12; } }
		public bool SupportsAttributeDrillDown { get; set; }
		public bool SupportsSubQuery { get; set; }
		#endregion
		public override void Read(XlsReader reader, XlsContentBuilder contentBuilder) {
			ushort bitwiseField = reader.ReadUInt16();
			SupportsAttributeDrillDown = (bitwiseField & 0x0002) != 0;
			SupportsSubQuery = (bitwiseField & 0x0004) != 0;
			reader.ReadInt32(); 
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
			if (contentBuilder == null || contentBuilder.CurrentPivotCacheBuilder.IsEmpty)
				return;
			XlsPivotCacheBuilderBase builder = contentBuilder.CurrentPivotCacheBuilder as XlsPivotCacheBuilderBase;
			if (builder.IgnoreAddlCache12Records)
				return;
			builder.SupportsAttributeDrillDown = SupportsAttributeDrillDown;
			builder.SupportsSubQuery = SupportsSubQuery;
		}
		public override void Write(BinaryWriter writer) {
			ushort bitwiseField = 0;
			if (SupportsAttributeDrillDown)
				bitwiseField |= 0x0002;
			if (SupportsSubQuery)
				bitwiseField |= 0x0004;
			writer.Write(bitwiseField);
			writer.Write((int)0); 
		}
	}
	#endregion
	#region XlsPivotAddlCacheEnd
	public class XlsPivotAddlCacheEnd : XlsPivotAddlEndBase {
		public override SxDataClass DataClass { get { return SxDataClass.Cache; } }
	}
	#endregion
	#region XlsPivotAddlCacheFieldId
	public class XlsPivotAddlCacheFieldId : XlsPivotAddlIdBase {
		public override SxDataClass DataClass { get { return SxDataClass.CacheField; } }
	}
	#endregion
	#region XlsPivotAddlCacheFieldProperty
	public class XlsPivotAddlCacheFieldProperty : XlsPivotAddlIntBase {
		public override SxDataClass DataClass { get { return SxDataClass.CacheField; } }
		public override SxDataType DataType { get { return SxDataType.Property; } }
	}
	#endregion
	#region XlsPivotAddlCacheFieldItemCount
	public class XlsPivotAddlCacheFieldItemCount : XlsPivotAddlIntBase {
		public override SxDataClass DataClass { get { return SxDataClass.CacheField; } }
		public override SxDataType DataType { get { return SxDataType.ItemCount; } }
		protected override void CheckValue(int value) {
			ValueChecker.CheckValue(value, 0, 1048576, "Value");
		}
	}
	#endregion
	#region XlsPivotAddlCacheFieldCaption
	public class XlsPivotAddlCacheFieldCaption : XlsPivotAddlStringBase {
		public override SxDataClass DataClass { get { return SxDataClass.CacheField; } }
		public override SxDataType DataType { get { return SxDataType.Caption; } }
	}
	#endregion
	#region XlsPivotAddlCacheFieldPropName
	public class XlsPivotAddlCacheFieldPropName : XlsPivotAddlStringBase {
		public override SxDataClass DataClass { get { return SxDataClass.CacheField; } }
		public override SxDataType DataType { get { return SxDataType.PropName; } }
	}
	#endregion
	#region XlsPivotAddlCacheFieldEnd
	public class XlsPivotAddlCacheFieldEnd : XlsPivotAddlEndBase {
		public override SxDataClass DataClass { get { return SxDataClass.CacheField; } }
	}
	#endregion
	#region XlsPivotAddlRuleId
	public class XlsPivotAddlRuleId : XlsPivotAddlBase {
		public override SxDataClass DataClass { get { return SxDataClass.Rule; } }
		public override SxDataType DataType { get { return SxDataType.Id; } }
	}
	#endregion
	#region XlsPivotAddlRule
	public class XlsPivotAddlRule : XlsPivotAddlBase {
		int numberOfFilters;
		int refersTo;
		#region Properties
		public override SxDataClass DataClass { get { return SxDataClass.Rule; } }
		public override SxDataType DataType { get { return SxDataType.Rule; } }
		public PivotAreaType AreaType { get; set; }
		public bool IsPart { get; set; }
		public bool DataOnly { get; set; }
		public bool LabelOnly { get; set; }
		public bool GrandRow { get; set; }
		public bool GrandColumn { get; set; }
		public bool Fuzzy { get; set; }
		public bool OutlineMode { get; set; }
		public bool DrillOnly { get; set; }
		public byte FirstRowDelta { get; set; }
		public byte LastRowDelta { get; set; }
		public byte FirstColumnDelta { get; set; }
		public byte LastColumnDelta { get; set; }
		public int NumberOfFilters {
			get { return numberOfFilters; }
			set {
				Guard.ArgumentNonNegative(value, "NumberOfFilters");
				numberOfFilters = value;
			}
		}
		public int Position { get; set; }
		public int RefersTo {
			get { return refersTo; }
			set {
				ValueChecker.CheckValue(value, -2, 255, "RefersTo");
				refersTo = value;
			}
		}
		#endregion
		public override void Read(XlsReader reader, XlsContentBuilder contentBuilder) {
			base.Read(reader, contentBuilder);
			AreaType = (PivotAreaType)((reader.ReadByte() & 0xf0) >> 4);
			ushort bitwiseField = reader.ReadByte();
			IsPart = (bitwiseField & 0x0001) != 0;
			DataOnly = (bitwiseField & 0x0002) != 0;
			LabelOnly = (bitwiseField & 0x0004) != 0;
			GrandRow = (bitwiseField & 0x0008) != 0;
			GrandColumn = (bitwiseField & 0x0010) != 0;
			bitwiseField = reader.ReadUInt16();
			Fuzzy = (bitwiseField & 0x0001) != 0;
			bitwiseField = reader.ReadUInt16();
			OutlineMode = (bitwiseField & 0x0002) != 0;
			DrillOnly = (bitwiseField & 0x0020) != 0;
			FirstRowDelta = reader.ReadByte();
			LastRowDelta = reader.ReadByte();
			FirstColumnDelta = reader.ReadByte();
			LastColumnDelta = reader.ReadByte();
			NumberOfFilters = reader.ReadInt32();
			Position = reader.ReadInt32();
			RefersTo = reader.ReadInt32();
		}
		public override void Write(BinaryWriter writer) {
			base.Write(writer);
			writer.Write((byte)((int)AreaType << 4));
			ushort bitwiseField = 0;
			if(IsPart)
				bitwiseField |= 0x0001;
			if(DataOnly)
				bitwiseField |= 0x0002;
			if(LabelOnly)
				bitwiseField |= 0x0004;
			if(GrandRow)
				bitwiseField |= 0x0028;
			if(GrandColumn)
				bitwiseField |= 0x0090;
			writer.Write((byte)bitwiseField);
			writer.Write((ushort)(Fuzzy ? 1 : 0));
			bitwiseField = 0;
			if(OutlineMode)
				bitwiseField |= 0x0002;
			if(DrillOnly)
				bitwiseField |= 0x0020;
			writer.Write(bitwiseField);
			writer.Write(FirstRowDelta);
			writer.Write(LastRowDelta);
			writer.Write(FirstColumnDelta);
			writer.Write(LastColumnDelta);
			writer.Write(NumberOfFilters);
			writer.Write(Position);
			writer.Write(RefersTo);
		}
		public override int GetSize() {
			return 28;
		}
	}
	#endregion
	#region XlsPivotAddlRuleEnd
	public class XlsPivotAddlRuleEnd : XlsPivotAddlEndBase {
		public override SxDataClass DataClass { get { return SxDataClass.Rule; } }
	}
	#endregion
	#region XlsPivotAddlFilterId
	public class XlsPivotAddlFilterId : XlsPivotAddlBase {
		public override SxDataClass DataClass { get { return SxDataClass.Filter; } }
		public override SxDataType DataType { get { return SxDataType.Id; } }
	}
	#endregion
	#region XlsPivotAddlFilter
	public class XlsPivotAddlFilter : XlsPivotAddlBase {
		int position;
		int refersTo;
		int numberOfItems;
		#region Properties
		public override SxDataClass DataClass { get { return SxDataClass.Filter; } }
		public override SxDataType DataType { get { return SxDataType.Filter; } }
		public bool RowAxisRefered { get; set; }
		public bool ColumnAxisRefered { get; set; }
		public bool DataAxisRefered { get; set; }
		public bool Selected { get; set; }
		public bool SubtotalsDisplayed { get; set; }
		public int Position {
			get { return position; }
			set {
				ValueChecker.CheckValue(value, -1, 0x001f, "Position");
				position = value;
			}
		}
		public int RefersTo {
			get { return refersTo; }
			set {
				ValueChecker.CheckValue(value, -2, 0xff, "RefersTo");
				refersTo = value;
			}
		}
		public int NumberOfItems {
			get { return numberOfItems; }
			set {
				Guard.ArgumentNonNegative(value, "NumberOfItems");
				numberOfItems = value;
			}
		}
		#endregion
		public override void Read(XlsReader reader, XlsContentBuilder contentBuilder) {
			base.Read(reader, contentBuilder);
			ushort bitwiseField = reader.ReadUInt16();
			RowAxisRefered = (bitwiseField & 0x0001) != 0;
			ColumnAxisRefered = (bitwiseField & 0x0002) != 0;
			DataAxisRefered = (bitwiseField & 0x0008) != 0;
			Selected = (bitwiseField & 0x0010) != 0;
			SubtotalsDisplayed = reader.ReadInt16() != 0;
			Position = reader.ReadInt32();
			RefersTo = reader.ReadInt32();
			NumberOfItems = reader.ReadInt32();
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
			contentBuilder.NumberOfPivotFilterItems = NumberOfItems;
		}
		public override void Write(BinaryWriter writer) {
			base.Write(writer);
			ushort bitwiseField = 0;
			if(RowAxisRefered)
				bitwiseField |= 0x0001;
			if(ColumnAxisRefered)
				bitwiseField |= 0x0002;
			if(DataAxisRefered)
				bitwiseField |= 0x0008;
			if(Selected)
				bitwiseField |= 0x0010;
			writer.Write(bitwiseField);
			writer.Write((ushort)(SubtotalsDisplayed ? 1 : 0));
			writer.Write(Position);
			writer.Write(RefersTo);
			writer.Write(NumberOfItems);
		}
		public override int GetSize() {
			return 22;
		}
	}
	#endregion
	#region XlsPivotAddlFilterItems
	public class XlsPivotAddlFilterItems : XlsPivotAddlBase {
		readonly List<int> items = new List<int>();
		#region Properties
		public override SxDataClass DataClass { get { return SxDataClass.Filter; } }
		public override SxDataType DataType { get { return SxDataType.FilterItem; } }
		public List<int> Items { get { return items; } }
		#endregion
		public override void Read(XlsReader reader, XlsContentBuilder contentBuilder) {
			base.Read(reader, contentBuilder);
			Items.Clear();
			for (int i = 0; i < contentBuilder.NumberOfPivotFilterItems; i++)
				Items.Add(reader.ReadUInt16());
		}
		public override void Write(BinaryWriter writer) {
			base.Write(writer);
			for (int i = 0; i < Items.Count; i++)
				writer.Write((ushort)Items[i]);
		}
		public override int GetSize() {
			return 6 + Items.Count * 2;
		}
	}
	#endregion
	#region XlsPivotAddlFilterEnd
	public class XlsPivotAddlFilterEnd : XlsPivotAddlEndBase {
		public override SxDataClass DataClass { get { return SxDataClass.Filter; } }
	}
	#endregion
	#region XlsPivotAddlAutoSortId
	public class XlsPivotAddlAutoSortId : XlsPivotAddlBase {
		public override SxDataClass DataClass { get { return SxDataClass.AutoSort; } }
		public override SxDataType DataType { get { return SxDataType.Id; } }
		public bool AscendingSort { get; set; }
		public override void Read(XlsReader reader, XlsContentBuilder contentBuilder) {
			ushort bitwiseField = reader.ReadUInt16();
			AscendingSort = (bitwiseField & 0x0001) != 0;
			reader.ReadInt32(); 
		}
		public override void Write(BinaryWriter writer) {
			ushort bitwiseField = 0;
			if (AscendingSort)
				bitwiseField |= 0x0001;
			writer.Write(bitwiseField);
			writer.Write((int)0); 
		}
	}
	#endregion
	#region XlsPivotAddlAutoSortEnd
	public class XlsPivotAddlAutoSortEnd : XlsPivotAddlEndBase {
		public override SxDataClass DataClass { get { return SxDataClass.AutoSort; } }
	}
	#endregion
	#region XlsPivotAddlConditionalFormattingsId
	public class XlsPivotAddlConditionalFormattingsId : XlsPivotAddlIntBase {
		public override SxDataClass DataClass { get { return SxDataClass.ConditionalFormattings; } }
		public override SxDataType DataType { get { return SxDataType.Id; } }
		protected override void CheckValue(int value) {
			ValueChecker.CheckValue(value, 0, Int32.MaxValue, "Value");
		}
	}
	#endregion
	#region XlsPivotAddlConditionalFormattingsEnd
	public class XlsPivotAddlConditionalFormattingsEnd : XlsPivotAddlEndBase {
		public override SxDataClass DataClass { get { return SxDataClass.ConditionalFormattings; } }
	}
	#endregion
	#region XlsPivotCondFmtScope
	public enum XlsPivotCondFmtScope {
		Selection = 0x0000,
		Data = 0x0001,
		Field = 0x0002
	}
	#endregion
	#region XlsPivotCondFmtType
	public enum XlsPivotCondFmtType {
		None = 0x0000,
		EntireRange = 0x0001,
		EachRow = 0x0002,
		EachColumn = 0x0003
	}
	#endregion
	#region XlsPivotAddlConditionalFormattingRule
	public class XlsPivotAddlConditionalFormattingRule : XlsPivotAddlBase {
		int priority = 1;
		int ruleCount;
		#region Properties
		public override SxDataClass DataClass { get { return SxDataClass.ConditionalFormattingRule; } }
		public override SxDataType DataType { get { return SxDataType.CondFmtRule; } }
		public XlsPivotCondFmtScope Scope { get; set; }
		public XlsPivotCondFmtType FormatType { get; set; }
		public int Priority {
			get { return priority; }
			set {
				ValueChecker.CheckValue(value, 1, Int32.MaxValue, "Priority");
				priority = value;
			}
		}
		public int RuleCount {
			get { return ruleCount; }
			set {
				ValueChecker.CheckValue(value, 0, Int32.MaxValue, "RuleCount");
				ruleCount = value;
			}
		}
		#endregion
		public override void Read(XlsReader reader, XlsContentBuilder contentBuilder) {
			base.Read(reader, contentBuilder);
			Scope = (XlsPivotCondFmtScope)reader.ReadInt32();
			FormatType = (XlsPivotCondFmtType)reader.ReadInt32();
			Priority = reader.ReadInt32();
			RuleCount = reader.ReadInt32();
		}
		public override void Write(BinaryWriter writer) {
			base.Write(writer);
			writer.Write((int)Scope);
			writer.Write((int)FormatType);
			writer.Write(Priority);
			writer.Write(RuleCount);
		}
		public override int GetSize() {
			return 22;
		}
	}
	#endregion
	#region XlsPivotAddlConditionalFormattingRuleEnd
	public class XlsPivotAddlConditionalFormattingRuleEnd : XlsPivotAddlEndBase {
		public override SxDataClass DataClass { get { return SxDataClass.ConditionalFormattingRule; } }
	}
	#endregion
	#region XlsPivotAddlFilters12Id
	public class XlsPivotAddlFilters12Id : XlsPivotAddlIntBase {
		public override SxDataClass DataClass { get { return SxDataClass.Filters12; } }
		public override SxDataType DataType { get { return SxDataType.Id; } }
		protected override void CheckValue(int value) {
			ValueChecker.CheckValue(value, 0, Int32.MaxValue, "Value");
		}
	}
	#endregion
	#region XlsPivotAddlFilters12End
	public class XlsPivotAddlFilters12End : XlsPivotAddlEndBase {
		public override SxDataClass DataClass { get { return SxDataClass.Filters12; } }
	}
	#endregion
	#region XlsPivotAddlFilter12Id
	public class XlsPivotAddlFilter12Id : XlsPivotAddlIntBase {
		public override SxDataClass DataClass { get { return SxDataClass.Filter12; } }
		public override SxDataType DataType { get { return SxDataType.Id; } }
		public override void Execute(XlsContentBuilder contentBuilder) {
			if (contentBuilder != null && contentBuilder.CurrentBuilderPivotView != null) {
				XlsBuildPivotView builder = contentBuilder.CurrentBuilderPivotView;
				builder.FilterId = Value;
			}
		}
	}
	#endregion
	#region XlsPivotAddlFilter12Caption
	public class XlsPivotAddlFilter12Caption : XlsPivotAddlStringBase {
		public override SxDataClass DataClass { get { return SxDataClass.Filter12; } }
		public override SxDataType DataType { get { return SxDataType.Caption; } }
		public override void Execute(XlsContentBuilder contentBuilder) {
			if (contentBuilder != null && contentBuilder.CurrentBuilderPivotView != null) {
				PivotFilter filter = contentBuilder.CurrentBuilderPivotView.Filter;
				filter.SetNameCore(Value); 
			}
		}
	}
	#endregion
	#region XlsPivotAddlFilter12Filter
	public class XlsPivotAddlFilter12Filter : XlsPivotAddlBase {
		#region Properties
		public override SxDataClass DataClass { get { return SxDataClass.Filter12; } }
		public override SxDataType DataType { get { return SxDataType.SxFilter; } }
		public int FieldIndex { get; set; }
		public int MemberPropertyIndex { get; set; }
		public PivotFilterType FilterType { get; set; }
		public int DataItemIndex { get; set; }
		public int HierarchyIndex { get; set; }
		#endregion
		public override void Read(XlsReader reader, XlsContentBuilder contentBuilder) {
			base.Read(reader, contentBuilder);
			FieldIndex = reader.ReadInt32();
			MemberPropertyIndex = reader.ReadInt32();
			FilterType = (PivotFilterType)reader.ReadInt32();
			reader.ReadInt32(); 
			DataItemIndex = reader.ReadInt32();
			HierarchyIndex = reader.ReadInt32();
		}
		public override void Write(BinaryWriter writer) {
			base.Write(writer);
			writer.Write(FieldIndex);
			writer.Write(MemberPropertyIndex);
			writer.Write((int)FilterType);
			writer.Write((int)0); 
			writer.Write(DataItemIndex);
			writer.Write(HierarchyIndex);
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
			if (contentBuilder != null && contentBuilder.CurrentBuilderPivotView != null) {
				XlsBuildPivotView builder = contentBuilder.CurrentBuilderPivotView;
				builder.Filter = new PivotFilter(contentBuilder.DocumentModel);
				PivotFilter filter = builder.Filter;
				builder.PivotTable.Filters.Add(builder.Filter);
				filter.SetPivotFilterIdCore(builder.FilterId);
				filter.SetFieldIndexCore(FieldIndex);
				filter.FilterType = FilterType;
				if (!(FilterType < PivotFilterType.CaptionEqual || FilterType > PivotFilterType.CaptionNotBetween))
					filter.SetMemberPropertyFieldIdCore(MemberPropertyIndex);
				if ((FilterType < PivotFilterType.CaptionEqual || FilterType > PivotFilterType.CaptionNotBetween))
					if (DataItemIndex > -1)
						builder.Filter.SetMeasureFieldIndexCore(DataItemIndex);
				if (HierarchyIndex > 0)
					builder.Filter.SetMeasureIndexCore(HierarchyIndex);
			}
		}
		public override int GetSize() {
			return 30;
		}
	}
	#endregion
	#region XlsPivotAddlFilter12FilterDesc
	public class XlsPivotAddlFilter12FilterDesc : XlsPivotAddlStringBase {
		public override SxDataClass DataClass { get { return SxDataClass.Filter12; } }
		public override SxDataType DataType { get { return SxDataType.SxFilterDesc; } }
		public override void Execute(XlsContentBuilder contentBuilder) {
			if (contentBuilder != null && contentBuilder.CurrentBuilderPivotView != null) {
				PivotFilter filter = contentBuilder.CurrentBuilderPivotView.Filter;
				filter.SetDescriptionCore(Value);
			}
		}
	}
	#endregion
	#region XlsPivotAddlFilter12FilterValue1
	public class XlsPivotAddlFilter12FilterValue1 : XlsPivotAddlStringBase {
		public override SxDataClass DataClass { get { return SxDataClass.Filter12; } }
		public override SxDataType DataType { get { return SxDataType.SxFilterValue1; } }
		public override void Execute(XlsContentBuilder contentBuilder) {
			if (contentBuilder != null && contentBuilder.CurrentBuilderPivotView != null) {
				PivotFilter filter = contentBuilder.CurrentBuilderPivotView.Filter;
				filter.SetLabelPivotCore(Value);
			}
		}
	}
	#endregion
	#region XlsPivotAddlFilter12FilterValue2
	public class XlsPivotAddlFilter12FilterValue2 : XlsPivotAddlStringBase {
		public override SxDataClass DataClass { get { return SxDataClass.Filter12; } }
		public override SxDataType DataType { get { return SxDataType.SxFilterValue2; } }
		public override void Execute(XlsContentBuilder contentBuilder) {
			if (contentBuilder != null && contentBuilder.CurrentBuilderPivotView != null) {
				PivotFilter filter = contentBuilder.CurrentBuilderPivotView.Filter;
				filter.SetLabelPivotFilterCore(Value);
			}
		}
	}
	#endregion
	#region XlsPivotCustomFilterType
	public enum XlsPivotCustomFilterType {
		None = 0x000,
		Top10 = 0x0003,
		EqualDate = 0x0004,
		Before = 0x0005,
		After = 0x0006,
		BetweenDate = 0x0007,
		Tomorrow = 0x0008,
		Today = 0x0009,
		Yesterday = 0x000a,
		NextWeek = 0x000b,
		ThisWeek = 0x000c,
		LastWeek = 0x000d,
		NextMonth = 0x000e,
		ThisMonth = 0x000f,
		LastMonth = 0x0010,
		NextQuarter = 0x0011,
		ThisQuarter = 0x0012,
		LastQuarter = 0x0013,
		NextYear = 0x0014,
		ThisYear = 0x0015,
		LastYear = 0x0016,
		YearToDate = 0x0017,
		FirstQuarter = 0x0018,
		SecondQuarter = 0x0019,
		ThirdQuarter = 0x001a,
		FourthQuarter = 0x001b,
		January = 0x001c,
		February = 0x001d,
		March = 0x001e,
		April = 0x001f,
		May = 0x0020,
		June = 0x0021,
		July = 0x0022,
		August = 0x0023,
		September = 0x0024,
		October = 0x0025,
		November = 0x0026,
		December = 0x0027,
		NotEqualDate = 0x0028,
		BeforeOrEqual = 0x0029,
		AfterOrEqual = 0x002a,
		NotBetweenDate = 0x002b
	}
	#endregion
	#region XlsPivotFilterTop10Type
	public enum XlsPivotFilterTop10Type {
		None = 0x0000,
		Count = 0x0001,
		Percent = 0x0002,
		Sum = 0x0003
	}
	#endregion
	#region XlsPivotDataComparisonType
	public enum XlsPivotDataComparisonType {
		None = 0x00,
		Numeric = 0x04,
		String = 0x06,
		SpaceCharacters = 0x0c,
		NonSpaceCharacters = 0x0e
	}
	#endregion
	#region XlsPivotDataComparisonOperation
	public enum XlsPivotDataComparisonOperation {
		None = 0x00,
		Less = 0x01,
		Equal = 0x02,
		LessOrEqual = 0x03,
		Greater = 0x04,
		NotEqual = 0x05,
		GreaterOrEqual = 0x06
	}
	#endregion
	#region XlsPivotDataOperator
	public class XlsPivotDataOperator {
		public static XlsPivotDataOperator FromStream(XlsReader reader) {
			XlsPivotDataOperator result = new XlsPivotDataOperator();
			result.Read(reader);
			return result;
		}
		#region Properties
		public XlsPivotDataComparisonType ComparisonType { get; set; }
		public XlsPivotDataComparisonOperation ComparisonOperation { get; set; }
		public double Value { get; set; }
		public bool IsSimpleComparison { get; set; }
		#endregion
		protected void Read(XlsReader reader) {
			ComparisonType = (XlsPivotDataComparisonType)reader.ReadByte();
			ComparisonOperation = (XlsPivotDataComparisonOperation)reader.ReadByte();
			switch(ComparisonType){
				case XlsPivotDataComparisonType.Numeric:
					Value = reader.ReadDouble();
					break;
				case XlsPivotDataComparisonType.String:
					IsSimpleComparison = reader.ReadBytes(8)[0] == 1;
					break;
				default:
					reader.ReadBytes(8);
					break;
			}
		}
		public void Write(BinaryWriter writer) {
			writer.Write((byte)ComparisonType);
			writer.Write((byte)ComparisonOperation);
			if (ComparisonType == XlsPivotDataComparisonType.Numeric)
				writer.Write(Value);
			else if (ComparisonType == XlsPivotDataComparisonType.String) {
				writer.Write((int)(IsSimpleComparison ? 1 : 0));
				writer.Write((int)0); 
			}
			else {
				writer.Write((int)0); 
				writer.Write((int)0); 
			}
		}
	}
	#endregion
	#region XlsPivotOperatorJoinType
	public enum XlsPivotOperatorJoinType {
		None = 0x00,
		And = 0x01,
		Or = 0x02
	}
	#endregion
	#region XlsPivotFilterCriteria
	public class XlsPivotFilterCriteria {
		XlsPivotDataOperator first = new XlsPivotDataOperator();
		XlsPivotDataOperator second = new XlsPivotDataOperator();
		public static XlsPivotFilterCriteria FromStream(XlsReader reader) {
			XlsPivotFilterCriteria result = new XlsPivotFilterCriteria();
			result.Read(reader);
			return result;
		}
		#region Properties
		public XlsPivotDataOperator First { get { return first; } }
		public XlsPivotDataOperator Second { get { return second; } }
		public XlsPivotOperatorJoinType JoinType { get; set; }
		#endregion
		protected void Read(XlsReader reader) {
			first = XlsPivotDataOperator.FromStream(reader);
			second = XlsPivotDataOperator.FromStream(reader);
			JoinType = (XlsPivotOperatorJoinType)reader.ReadInt32();
		}
		public void Write(BinaryWriter writer) {
			first.Write(writer);
			second.Write(writer);
			writer.Write((int)JoinType);
			writer.Write((int)0);
		}
	}
	#endregion
	#region XlsPivotFilterTop10
	public class XlsPivotFilterTop10 {
		public static XlsPivotFilterTop10 FromStream(XlsReader reader) {
			XlsPivotFilterTop10 result = new XlsPivotFilterTop10();
			result.Read(reader);
			return result;
		}
		#region Properties
		public XlsPivotFilterTop10Type Top10Type { get; set; }
		public bool IsTop { get; set; }
		public double Value { get; set; }
		#endregion
		protected void Read(XlsReader reader) {
			Top10Type = (XlsPivotFilterTop10Type)reader.ReadInt32();
			ushort bitwiseField = reader.ReadUInt16();
			IsTop = (bitwiseField & 0x0001) != 0;
			Value = reader.ReadDouble();
			reader.ReadBytes(14); 
		}
		public void Write(BinaryWriter writer) {
			writer.Write((int)Top10Type);
			writer.Write((ushort)(IsTop ? 1 : 0));
			writer.Write(Value);
			writer.Write(new byte[14]); 
		}
	}
	#endregion
	#region XlsPivotAddlFilter12XlsFilter
	public class XlsPivotAddlFilter12XlsFilter : XlsPivotAddlBase {
		int numberOfCriteria;
		XlsPivotFilterTop10 top10 = null;
		XlsPivotFilterCriteria criteria = null;
		#region Properties
		public override SxDataClass DataClass { get { return SxDataClass.Filter12; } }
		public override SxDataType DataType { get { return SxDataType.XlsFilter; } }
		public PivotFilterType FilterType { get; set; }
		#region Filter Type Dictionary
		static Dictionary<XlsPivotCustomFilterType, PivotFilterType> filterTypes = new Dictionary<XlsPivotCustomFilterType, PivotFilterType> {
			{XlsPivotCustomFilterType.None, PivotFilterType.Unknown},
			{XlsPivotCustomFilterType.Top10, PivotFilterType.Count},
			{XlsPivotCustomFilterType.EqualDate, PivotFilterType.DateEqual},
			{XlsPivotCustomFilterType.NotEqualDate, PivotFilterType.DateNotEqual},
			{XlsPivotCustomFilterType.January, PivotFilterType.January},
			{XlsPivotCustomFilterType.February, PivotFilterType.February},
			{XlsPivotCustomFilterType.March, PivotFilterType.March},
			{XlsPivotCustomFilterType.April, PivotFilterType.April},
			{XlsPivotCustomFilterType.May, PivotFilterType.May},
			{XlsPivotCustomFilterType.June, PivotFilterType.June},
			{XlsPivotCustomFilterType.July, PivotFilterType.July},
			{XlsPivotCustomFilterType.August, PivotFilterType.August},
			{XlsPivotCustomFilterType.September, PivotFilterType.September},
			{XlsPivotCustomFilterType.October, PivotFilterType.October},
			{XlsPivotCustomFilterType.November, PivotFilterType.November},
			{XlsPivotCustomFilterType.December, PivotFilterType.December},
			{XlsPivotCustomFilterType.LastWeek, PivotFilterType.LastWeek},
			{XlsPivotCustomFilterType.LastMonth, PivotFilterType.LastMonth},
			{XlsPivotCustomFilterType.LastYear, PivotFilterType.LastYear},
			{XlsPivotCustomFilterType.LastQuarter, PivotFilterType.LastQuarter},
			{XlsPivotCustomFilterType.NextWeek, PivotFilterType.NextWeek},
			{XlsPivotCustomFilterType.NextMonth, PivotFilterType.NextMonth},
			{XlsPivotCustomFilterType.NextYear, PivotFilterType.NextYear},
			{XlsPivotCustomFilterType.NextQuarter, PivotFilterType.NextQuarter},
			{XlsPivotCustomFilterType.ThisWeek, PivotFilterType.ThisWeek},
			{XlsPivotCustomFilterType.ThisMonth, PivotFilterType.ThisMonth},
			{XlsPivotCustomFilterType.ThisYear, PivotFilterType.ThisYear},
			{XlsPivotCustomFilterType.ThisQuarter, PivotFilterType.ThisQuarter},
			{XlsPivotCustomFilterType.FirstQuarter, PivotFilterType.FirstQuarter},
			{XlsPivotCustomFilterType.SecondQuarter, PivotFilterType.SecondQuarter},
			{XlsPivotCustomFilterType.ThirdQuarter, PivotFilterType.ThirdQuarter},
			{XlsPivotCustomFilterType.FourthQuarter, PivotFilterType.FourthQuarter},
			{XlsPivotCustomFilterType.Today, PivotFilterType.Today},
			{XlsPivotCustomFilterType.Tomorrow, PivotFilterType.Tomorrow},
			{XlsPivotCustomFilterType.Yesterday, PivotFilterType.Yesterday},
			{XlsPivotCustomFilterType.BetweenDate, PivotFilterType.DateBetween},
			{XlsPivotCustomFilterType.NotBetweenDate, PivotFilterType.DateNotBetween},
			{XlsPivotCustomFilterType.YearToDate, PivotFilterType.YearToDate},
			{XlsPivotCustomFilterType.Before, PivotFilterType.DateNewerThan},
			{XlsPivotCustomFilterType.BeforeOrEqual, PivotFilterType.DateNewerThanOrEqual},
			{XlsPivotCustomFilterType.After, PivotFilterType.DateOlderThan},
			{XlsPivotCustomFilterType.AfterOrEqual, PivotFilterType.DateOlderThanOrEqual}
		};
		static Dictionary<PivotFilterType, XlsPivotCustomFilterType> XlsFilterTypes = RevertPivotFilterType();
		static Dictionary<PivotFilterType, XlsPivotCustomFilterType> RevertPivotFilterType() { 
			Dictionary<PivotFilterType, XlsPivotCustomFilterType> result = new Dictionary<PivotFilterType,XlsPivotCustomFilterType>();
			foreach (XlsPivotCustomFilterType key in filterTypes.Keys) 
				result.Add(filterTypes[key], key);
			result.Add(PivotFilterType.Percent, XlsPivotCustomFilterType.Top10);
			result.Add(PivotFilterType.Sum, XlsPivotCustomFilterType.Top10);
			return result;
		}
		#endregion
		PivotFilterType GetPivotFilterType(XlsPivotCustomFilterType xlsType) {
			return filterTypes[xlsType];
		}
		XlsPivotCustomFilterType GetXlsPivotCustomFilterType(PivotFilterType pivotFilterType) {
			if (XlsFilterTypes.ContainsKey(pivotFilterType))
				return XlsFilterTypes[pivotFilterType];
			return XlsPivotCustomFilterType.None;
		}
		public int NumberOfCriteria {
			get { return numberOfCriteria; }
			set {
				ValueChecker.CheckValue(value, 0, 2, "NumberOfCriteria");
				numberOfCriteria = value;
			}
		}
		public XlsPivotFilterTop10 Top10 { get { return top10; } set { top10 = value; } }
		public XlsPivotFilterCriteria Criteria { get { return criteria; } set { criteria = value; } }
		#endregion
		public override void Read(XlsReader reader, XlsContentBuilder contentBuilder) {
			base.Read(reader, contentBuilder);
			FilterType = GetPivotFilterType((XlsPivotCustomFilterType)reader.ReadInt32());
			NumberOfCriteria = reader.ReadInt32();
			if (FilterType > PivotFilterType.Unknown && FilterType < PivotFilterType.CaptionEqual){
				Top10 = XlsPivotFilterTop10.FromStream(reader);
				FilterType = (PivotFilterType)Top10.Top10Type;
			} 
			else
				Criteria = XlsPivotFilterCriteria.FromStream(reader);
		}
		public override void Write(BinaryWriter writer) {
			base.Write(writer);
			writer.Write((int)GetXlsPivotCustomFilterType(FilterType));
			writer.Write(NumberOfCriteria);
			if (FilterType > PivotFilterType.Unknown && FilterType < PivotFilterType.CaptionEqual)
				Top10.Write(writer);
			else
				Criteria.Write(writer);
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
			if (contentBuilder != null && contentBuilder.CurrentBuilderPivotView != null) {
				PivotFilter filter = contentBuilder.CurrentBuilderPivotView.Filter;
				AutoFilterColumn aFilterColumn = new AutoFilterColumn(contentBuilder.DocumentModel.ActiveSheet);
				filter.AutoFilter.FilterColumns.AddCore(aFilterColumn);
				contentBuilder.CurrentBuilderPivotView.AutoFilterColumn = aFilterColumn;
				if (Top10 != null) {
					if (Top10.Top10Type > XlsPivotFilterTop10Type.None)
						aFilterColumn.Top10FilterType = (Top10FilterType)((int)Top10.Top10Type - 1);
					filter.FilterType = (PivotFilterType)((int)Top10.Top10Type);
					aFilterColumn.FilterByTopOrder = Top10.IsTop;
					aFilterColumn.TopOrBottomDoubleValue = Top10.Value;
					aFilterColumn.FilterDoubleValue = Top10.Value;
				}
				else {
					if (filter.FilterType == PivotFilterType.Unknown)
						filter.FilterType = FilterType;
					if (numberOfCriteria > 0) {
						if (Criteria.First.ComparisonType == XlsPivotDataComparisonType.String){
							contentBuilder.CurrentBuilderPivotView.FirstValue = new CustomFilter();
							contentBuilder.CurrentBuilderPivotView.FirstValue.FilterOperator = (FilterComparisonOperator)((int)Criteria.First.ComparisonOperation);
						}
						else if (Criteria.First.ComparisonOperation != XlsPivotDataComparisonOperation.None) 
							aFilterColumn.CustomFilters.Add(GetCustomFilter(Criteria.First));
					}
					if (numberOfCriteria == 2) {
						if (Criteria.Second.ComparisonType == XlsPivotDataComparisonType.String){
							contentBuilder.CurrentBuilderPivotView.SecondValue = new CustomFilter();
							contentBuilder.CurrentBuilderPivotView.FirstValue.FilterOperator = (FilterComparisonOperator)((int)Criteria.Second.ComparisonOperation);
						}
						else if (Criteria.Second.ComparisonOperation != XlsPivotDataComparisonOperation.None)
							aFilterColumn.CustomFilters.Add(GetCustomFilter(Criteria.Second));
					}
					aFilterColumn.CustomFilters.CriterionAnd = Criteria.JoinType == XlsPivotOperatorJoinType.And;
				}
			}
		}
		CustomFilter GetCustomFilter(XlsPivotDataOperator dataOperation) {
			CustomFilter filter = new CustomFilter();
			filter.SetupFromInvariantValue(dataOperation.Value, false);
			filter.FilterOperator = (FilterComparisonOperator)((int)dataOperation.ComparisonOperation);
			return filter;
		}
		public override int GetSize() {
			return 42;
		}
	}
	#endregion
	#region XlsPivotAddlFilter12XlsFilterValue1
	public class XlsPivotAddlFilter12XlsFilterValue1 : XlsPivotAddlStringBase {
		public override SxDataClass DataClass { get { return SxDataClass.Filter12; } }
		public override SxDataType DataType { get { return SxDataType.XlsFilterValue1; } }
		public override void Execute(XlsContentBuilder contentBuilder) {
			if (contentBuilder != null && contentBuilder.CurrentBuilderPivotView != null) {
				CustomFilter filter = contentBuilder.CurrentBuilderPivotView.FirstValue;
				filter.Value = Value;
				filter.UpdateNumericValue(contentBuilder.CurrentSheet.DataContext, false);
				contentBuilder.CurrentBuilderPivotView.AutoFilterColumn.CustomFilters.Add(filter);
			}
		}
	}
	#endregion
	#region XlsPivotAddlFilter12XlsFilterValue2
	public class XlsPivotAddlFilter12XlsFilterValue2 : XlsPivotAddlStringBase {
		public override SxDataClass DataClass { get { return SxDataClass.Filter12; } }
		public override SxDataType DataType { get { return SxDataType.XlsFilterValue2; } }
		public override void Execute(XlsContentBuilder contentBuilder) {
			if (contentBuilder != null && contentBuilder.CurrentBuilderPivotView != null) {
				CustomFilter filter = contentBuilder.CurrentBuilderPivotView.SecondValue;
				filter.Value = Value;
				filter.UpdateNumericValue(contentBuilder.CurrentSheet.DataContext, false);
				contentBuilder.CurrentBuilderPivotView.AutoFilterColumn.CustomFilters.Add(filter);
			}
		}
	}
	#endregion
	#region XlsPivotAddlFilter12End
	public class XlsPivotAddlFilter12End : XlsPivotAddlEndBase {
		public override SxDataClass DataClass { get { return SxDataClass.Filter12; } }
	}
	#endregion
	#region XlsPivotAddlFactory
	public static class XlsPivotAddlFactory {
		#region AddlProduct
		class AddlProduct {
			public AddlProduct(int typeCode, Type productType) {
				TypeCode = typeCode;
				ProductType = productType;
			}
			public int TypeCode { get; private set; }
			public Type ProductType { get; private set; }
		}
		#endregion
		static List<AddlProduct> products;
		static Dictionary<int, ConstructorInfo> productTypes;
		static XlsPivotAddlFactory() {
			products = new List<AddlProduct>();
			productTypes = new Dictionary<int, ConstructorInfo>();
			CreateProducts();
			foreach (AddlProduct product in products) {
				productTypes.Add(product.TypeCode, product.ProductType.GetConstructor(new Type[] { }));
			}
		}
		static void CreateProducts() {
			AddProduct(SxDataClass.View, SxDataType.Id, typeof(XlsPivotAddlViewId));
			AddProduct(SxDataClass.View, SxDataType.VerUpdInv, typeof(XlsPivotAddlViewVersionUpdate));
			AddProduct(SxDataClass.View, SxDataType.Ver10Info, typeof(XlsPivotAddlViewVersion10));
			AddProduct(SxDataClass.View, SxDataType.CalcMember, typeof(XlsPivotAddlViewCalcMember));
			AddProduct(SxDataClass.View, SxDataType.CalcMemString, typeof(XlsPivotAddlViewCalcMemString));
			AddProduct(SxDataClass.View, SxDataType.Ver12Info, typeof(XlsPivotAddlViewVersion12));
			AddProduct(SxDataClass.View, SxDataType.TableStyleClient, typeof(XlsPivotAddlViewTableStyleClient));
			AddProduct(SxDataClass.View, SxDataType.CompactRowHdr, typeof(XlsPivotAddlViewCompactRowHeader));
			AddProduct(SxDataClass.View, SxDataType.CompactColumnHdr, typeof(XlsPivotAddlViewCompactColumnHeader));
			AddProduct(SxDataClass.View, SxDataType.ValueMetadataMapping, typeof(XlsPivotAddlViewValueMetadataMapping));
			AddProduct(SxDataClass.View, SxDataType.End, typeof(XlsPivotAddlViewEnd));
			AddProduct(SxDataClass.Field, SxDataType.Id, typeof(XlsPivotAddlFieldId));
			AddProduct(SxDataClass.Field, SxDataType.Ver10Info, typeof(XlsPivotAddlFieldVersion10));
			AddProduct(SxDataClass.Field, SxDataType.End, typeof(XlsPivotAddlFieldEnd));
			AddProduct(SxDataClass.Field12, SxDataType.Id, typeof(XlsPivotAddlField12Id));
			AddProduct(SxDataClass.Field12, SxDataType.VerUpdInv, typeof(XlsPivotAddlField12VersionUpdate));
			AddProduct(SxDataClass.Field12, SxDataType.MemberCaption, typeof(XlsPivotAddlField12MemberCaption));
			AddProduct(SxDataClass.Field12, SxDataType.Ver12Info, typeof(XlsPivotAddlField12Version12));
			AddProduct(SxDataClass.Field12, SxDataType.Hierarchy, typeof(XlsPivotAddlField12HierarchyIndex));
			AddProduct(SxDataClass.Field12, SxDataType.AutoShow, typeof(XlsPivotAddlField12AutoShow));
			AddProduct(SxDataClass.Field12, SxDataType.End, typeof(XlsPivotAddlField12End));
			AddProduct(SxDataClass.Cache, SxDataType.Id, typeof(XlsPivotAddlCacheId));
			AddProduct(SxDataClass.Cache, SxDataType.VerUpdInv, typeof(XlsPivotAddlCacheVersionUpdate));
			AddProduct(SxDataClass.Cache, SxDataType.Ver10Info, typeof(XlsPivotAddlCacheVersion10));
			AddProduct(SxDataClass.Cache, SxDataType.VerSXMacro, typeof(XlsPivotAddlCacheVerMacro));
			AddProduct(SxDataClass.Cache, SxDataType.InvRefreshReal, typeof(XlsPivotAddlCacheInvRefreshReal));
			AddProduct(SxDataClass.Cache, SxDataType.Info12, typeof(XlsPivotAddlCacheInfo12));
			AddProduct(SxDataClass.Cache, SxDataType.End, typeof(XlsPivotAddlCacheEnd));
			AddProduct(SxDataClass.CacheField, SxDataType.Id, typeof(XlsPivotAddlCacheFieldId));
			AddProduct(SxDataClass.CacheField, SxDataType.Property, typeof(XlsPivotAddlCacheFieldProperty));
			AddProduct(SxDataClass.CacheField, SxDataType.ItemCount, typeof(XlsPivotAddlCacheFieldItemCount));
			AddProduct(SxDataClass.CacheField, SxDataType.Caption, typeof(XlsPivotAddlCacheFieldCaption));
			AddProduct(SxDataClass.CacheField, SxDataType.PropName, typeof(XlsPivotAddlCacheFieldPropName));
			AddProduct(SxDataClass.CacheField, SxDataType.End, typeof(XlsPivotAddlCacheFieldEnd));
			AddProduct(SxDataClass.Rule, SxDataType.Id, typeof(XlsPivotAddlRuleId));
			AddProduct(SxDataClass.Rule, SxDataType.Rule, typeof(XlsPivotAddlRule));
			AddProduct(SxDataClass.Rule, SxDataType.End, typeof(XlsPivotAddlRuleEnd));
			AddProduct(SxDataClass.Filter, SxDataType.Id, typeof(XlsPivotAddlFilterId));
			AddProduct(SxDataClass.Filter, SxDataType.Filter, typeof(XlsPivotAddlFilter));
			AddProduct(SxDataClass.Filter, SxDataType.FilterItem, typeof(XlsPivotAddlFilterItems));
			AddProduct(SxDataClass.Filter, SxDataType.End, typeof(XlsPivotAddlFilterEnd));
			AddProduct(SxDataClass.AutoSort, SxDataType.Id, typeof(XlsPivotAddlAutoSortId));
			AddProduct(SxDataClass.AutoSort, SxDataType.End, typeof(XlsPivotAddlAutoSortEnd));
			AddProduct(SxDataClass.ConditionalFormattings, SxDataType.Id, typeof(XlsPivotAddlConditionalFormattingsId));
			AddProduct(SxDataClass.ConditionalFormattings, SxDataType.End, typeof(XlsPivotAddlConditionalFormattingsEnd));
			AddProduct(SxDataClass.ConditionalFormattingRule, SxDataType.CondFmtRule, typeof(XlsPivotAddlConditionalFormattingRule));
			AddProduct(SxDataClass.ConditionalFormattingRule, SxDataType.End, typeof(XlsPivotAddlConditionalFormattingRuleEnd));
			AddProduct(SxDataClass.Filters12, SxDataType.Id, typeof(XlsPivotAddlFilters12Id));
			AddProduct(SxDataClass.Filters12, SxDataType.End, typeof(XlsPivotAddlFilters12End));
			AddProduct(SxDataClass.Filter12, SxDataType.Id, typeof(XlsPivotAddlFilter12Id));
			AddProduct(SxDataClass.Filter12, SxDataType.Caption, typeof(XlsPivotAddlFilter12Caption));
			AddProduct(SxDataClass.Filter12, SxDataType.SxFilter, typeof(XlsPivotAddlFilter12Filter));
			AddProduct(SxDataClass.Filter12, SxDataType.SxFilterDesc, typeof(XlsPivotAddlFilter12FilterDesc));
			AddProduct(SxDataClass.Filter12, SxDataType.SxFilterValue1, typeof(XlsPivotAddlFilter12FilterValue1));
			AddProduct(SxDataClass.Filter12, SxDataType.SxFilterValue2, typeof(XlsPivotAddlFilter12FilterValue2));
			AddProduct(SxDataClass.Filter12, SxDataType.XlsFilter, typeof(XlsPivotAddlFilter12XlsFilter));
			AddProduct(SxDataClass.Filter12, SxDataType.XlsFilterValue1, typeof(XlsPivotAddlFilter12XlsFilterValue1));
			AddProduct(SxDataClass.Filter12, SxDataType.XlsFilterValue2, typeof(XlsPivotAddlFilter12XlsFilterValue2));
			AddProduct(SxDataClass.Filter12, SxDataType.End, typeof(XlsPivotAddlFilter12End));
		}
		static void AddProduct(SxDataClass dataClass, SxDataType dataType, Type productType) {
			AddlProduct product = new AddlProduct(GetTypeCode(dataClass, dataType), productType);
			products.Add(product);
		}
		static int GetTypeCode(SxDataClass dataClass, SxDataType dataType) {
			return (int)dataClass + ((int)dataType << 8);
		}
		public static IXlsPivotAddl CreateAdditionalData(SxDataClass dataClass, SxDataType dataType) {
			int typeCode = GetTypeCode(dataClass, dataType);
			if (!productTypes.ContainsKey(typeCode))
				return null;
			ConstructorInfo productConstructor = productTypes[typeCode];
			IXlsPivotAddl productInstance = productConstructor.Invoke(new object[] { }) as IXlsPivotAddl;
			return productInstance;
		}
	}
	#endregion
	#region XlsCommandPivotAddl
	public class XlsCommandPivotAddl : XlsCommandBase {
		const int headerSize = 6;
		XlsPivotAddlHeader header = new XlsPivotAddlHeader();
		IXlsPivotAddl data;
		#region Properties
		public IXlsPivotAddl Data {
			get { return data; }
			set {
				Guard.ArgumentNotNull(value, "Data");
				data = value;
			}
		}
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			header = XlsPivotAddlHeader.FromStream(reader);
			data = XlsPivotAddlFactory.CreateAdditionalData(header.DataClass, header.DataType);
			if (data != null) {
				data.Read(reader, contentBuilder);
			}
			else { 
				int bytesToRead = Size - headerSize;
				if (bytesToRead > 0)
					reader.ReadBytes(bytesToRead);
			}
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
			if (data != null)
				data.Execute(contentBuilder);
		}
		protected override void WriteCore(BinaryWriter writer) {
			header.RecordTypeId = XlsCommandFactory.GetTypeCodeByType(this.GetType());
			header.DataClass = data.DataClass;
			header.DataType = data.DataType;
			header.Write(writer);
			data.Write(writer);
		}
		protected override short GetSize() {
			return (short)(headerSize + data.GetSize());
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
}
