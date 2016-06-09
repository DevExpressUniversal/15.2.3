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
using System.IO;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsCommandSharedFeatureHeader
	public class XlsCommandSharedFeatureHeader : XlsCommandBase {
		#region Fields
		const int fixedPartSize = 19;
		XlsSharedFeatureType sharedFeatureType = XlsSharedFeatureType.Protection;
		XlsSharedFeatureEnhancedProtection protection = new XlsSharedFeatureEnhancedProtection();
		#endregion
		#region Properties
		public XlsSharedFeatureType SharedFeatureType { get { return sharedFeatureType; } set { sharedFeatureType = value; } }
		public bool HasHeaderData { get; set; }
		public XlsSharedFeatureEnhancedProtection Protection { get { return protection; } }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			FutureRecordHeader.FromStream(reader);
			SharedFeatureType = (XlsSharedFeatureType)reader.ReadInt16();
			reader.ReadByte(); 
			HasHeaderData = reader.ReadInt32() != 0;
			if(HasHeaderData) {
				if(SharedFeatureType == XlsSharedFeatureType.Protection)
					protection = XlsSharedFeatureEnhancedProtection.FromStream(reader);
				else {
					int bytesToRead = Size - fixedPartSize;
					if(bytesToRead > 0)
						reader.ReadBytes(bytesToRead);
				}
			}
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			if (SharedFeatureType == XlsSharedFeatureType.Protection) {
				WorksheetProtectionOptions protection = contentBuilder.CurrentSheet.Properties.Protection;
				protection.BeginUpdate();
				try {
					protection.AutoFiltersLocked = !Protection.AutoFilter;
					protection.DeleteColumnsLocked = !Protection.DeleteColumns;
					protection.DeleteRowsLocked = !Protection.DeleteRows;
					protection.FormatCellsLocked = !Protection.FormatCells;
					protection.FormatColumnsLocked = !Protection.FormatColumns;
					protection.FormatRowsLocked = !Protection.FormatRows;
					protection.InsertColumnsLocked = !Protection.InsertColumns;
					protection.InsertHyperlinksLocked = !Protection.InsertHyperlinks;
					protection.InsertRowsLocked = !Protection.InsertRows;
					protection.ObjectsLocked = !Protection.Objects;
					protection.PivotTablesLocked = !Protection.PivotTables;
					protection.ScenariosLocked = !Protection.Scenarios;
					protection.SelectLockedCellsLocked = !Protection.SetLockedCells;
					protection.SelectUnlockedCellsLocked = !Protection.SetUnlockedCells;
					protection.SortLocked = !Protection.Sort;
				}
				finally {
					protection.EndUpdate();
				}
			}
		}
		protected override void WriteCore(BinaryWriter writer) {
			FutureRecordHeader header = new FutureRecordHeader();
			header.RecordTypeId = XlsCommandFactory.GetTypeCodeByType(typeof(XlsCommandSharedFeatureHeader));
			header.Write(writer);
			writer.Write((short)SharedFeatureType);
			writer.Write((byte)1); 
			if(HasHeaderData && (SharedFeatureType == XlsSharedFeatureType.Protection)) {
				writer.Write((uint)0xffffffff);
				this.protection.Write(writer);
			}
			else
				writer.Write((uint)0);
		}
		protected override short GetSize() {
			int variablePartSize = 0;
			if(HasHeaderData && (SharedFeatureType == XlsSharedFeatureType.Protection))
				variablePartSize = Protection.GetSize();
			return (short)(fixedPartSize + variablePartSize);
		}
		public override IXlsCommand GetInstance() {
			return new XlsCommandSharedFeatureHeader();
		}
	}
	#endregion
	#region XlsCommandSharedFeatureHeader11
	public class XlsCommandSharedFeatureHeader11 : XlsCommandBase {
		#region Fields
		const short fixedSize = 29;
		XlsSharedFeatureType sharedFeatureType = XlsSharedFeatureType.List;
		#endregion
		#region Properties
		protected internal XlsSharedFeatureType SharedFeatureType { get { return sharedFeatureType; } set { sharedFeatureType = value; } }
		protected internal int IdListNext { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			FutureRecordHeader.FromStream(reader);
			SharedFeatureType = (XlsSharedFeatureType)reader.ReadInt16();
			reader.ReadByte();
			IdListNext = XlsSharedFeatureHeaderData.FromStream(reader).IdListNext;
		}
		protected override void WriteCore(BinaryWriter writer) {
			FutureRecordHeader header = new FutureRecordHeader();
			header.RecordTypeId = XlsCommandFactory.GetTypeCodeByType(typeof(XlsCommandSharedFeatureHeader11));
			header.Write(writer);
			writer.Write((short)SharedFeatureType);
			writer.Write((byte)1);
			XlsSharedFeatureHeaderData data = new XlsSharedFeatureHeaderData();
			data.IdListNext = IdListNext;
			data.Write(writer);
		}
		protected override short GetSize() {
			return fixedSize;
		}
		public override IXlsCommand GetInstance() {
			return new XlsCommandSharedFeatureHeader11();
		}
	}
	#endregion
	#region XlsSharedFeatureHeaderData
	public class XlsSharedFeatureHeaderData {
		protected internal int IdListNext { get; set; }
		public static XlsSharedFeatureHeaderData FromStream(XlsReader reader) {
			XlsSharedFeatureHeaderData result = new XlsSharedFeatureHeaderData();
			result.Read(reader);
			return result;
		}
		void Read(XlsReader reader) {
			reader.ReadInt32(); 
			reader.ReadInt32(); 
			IdListNext = reader.ReadInt32();
			XlsSharedFeatureFilterData.FromStream(reader);
		}
		public void Write(BinaryWriter writer) {
			writer.Write(0xFFFFFFFF); 
			writer.Write(0xFFFFFFFF); 
			writer.Write(IdListNext);
			XlsSharedFeatureFilterData filterData = new XlsSharedFeatureFilterData();
			filterData.Write(writer);
		}
	}
	#endregion
	#region XlsSharedFeatureFilterData
	public class XlsSharedFeatureFilterData {
		public static XlsSharedFeatureFilterData FromStream(XlsReader reader) {
			XlsSharedFeatureFilterData result = new XlsSharedFeatureFilterData();
			result.Read(reader);
			return result;
		}
		void Read(XlsReader reader) {
			XlsSharedFeatureFilterDataArray.FromStream(reader);
		}
		public void Write(BinaryWriter writer) {
			writer.Write((short)0);
		}
	}
	#endregion
	#region XlsSharedFeatureFilterDataArray
	public class XlsSharedFeatureFilterDataArray {
		public static XlsSharedFeatureFilterDataArray FromStream(XlsReader reader) {
			XlsSharedFeatureFilterDataArray result = new XlsSharedFeatureFilterDataArray();
			result.Read(reader);
			return result;
		}
		void Read(XlsReader reader) {
			int count = reader.ReadInt16();
			if (count == 0)
				return;
			SkipBytesFromFilterDataArray(reader, count);
		}
		void SkipBytesFromFilterDataArray(XlsReader reader, int count) {
			for (int i = 0; i < count; i++) {
				short variablePartSize = (short)reader.ReadInt32();
				reader.ReadInt16(); 
				reader.ReadBytes(variablePartSize);
			}
			reader.ReadInt64();
		}
	}
	#endregion 
	#region XlsSharedFeatureEnhancedProtection
	public class XlsSharedFeatureEnhancedProtection {
		#region Properties
		public bool Objects { get; set; }
		public bool Scenarios { get; set; }
		public bool FormatCells { get; set; }
		public bool FormatColumns { get; set; }
		public bool FormatRows { get; set; }
		public bool InsertColumns { get; set; }
		public bool InsertRows { get; set; }
		public bool InsertHyperlinks { get; set; }
		public bool DeleteColumns { get; set; }
		public bool DeleteRows { get; set; }
		public bool SetLockedCells { get; set; }
		public bool Sort { get; set; }
		public bool AutoFilter { get; set; }
		public bool PivotTables { get; set; }
		public bool SetUnlockedCells { get; set; }
		#endregion
		public static XlsSharedFeatureEnhancedProtection FromStream(XlsReader reader) {
			XlsSharedFeatureEnhancedProtection result = new XlsSharedFeatureEnhancedProtection();
			result.Read(reader);
			return result;
		}
		protected void Read(XlsReader reader) {
			uint bitwiseField = reader.ReadUInt32();
			Objects = Convert.ToBoolean(bitwiseField & 0x0001);
			Scenarios = Convert.ToBoolean(bitwiseField & 0x0002);
			FormatCells = Convert.ToBoolean(bitwiseField & 0x0004);
			FormatColumns = Convert.ToBoolean(bitwiseField & 0x0008);
			FormatRows = Convert.ToBoolean(bitwiseField & 0x0010);
			InsertColumns = Convert.ToBoolean(bitwiseField & 0x0020);
			InsertRows = Convert.ToBoolean(bitwiseField & 0x0040);
			InsertHyperlinks = Convert.ToBoolean(bitwiseField & 0x0080);
			DeleteColumns = Convert.ToBoolean(bitwiseField & 0x0100);
			DeleteRows = Convert.ToBoolean(bitwiseField & 0x0200);
			SetLockedCells = Convert.ToBoolean(bitwiseField & 0x0400);
			Sort = Convert.ToBoolean(bitwiseField & 0x0800);
			AutoFilter = Convert.ToBoolean(bitwiseField & 0x1000);
			PivotTables = Convert.ToBoolean(bitwiseField & 0x2000);
			SetUnlockedCells = Convert.ToBoolean(bitwiseField & 0x4000);
		}
		public void Write(BinaryWriter writer) {
			uint bitwiseField = 0;
			if(Objects)
				bitwiseField |= 0x0001;
			if(Scenarios)
				bitwiseField |= 0x0002;
			if(FormatCells)
				bitwiseField |= 0x0004;
			if(FormatColumns)
				bitwiseField |= 0x0008;
			if(FormatRows)
				bitwiseField |= 0x0010;
			if(InsertColumns)
				bitwiseField |= 0x0020;
			if(InsertRows)
				bitwiseField |= 0x0040;
			if(InsertHyperlinks)
				bitwiseField |= 0x0080;
			if(DeleteColumns)
				bitwiseField |= 0x0100;
			if(DeleteRows)
				bitwiseField |= 0x0200;
			if(SetLockedCells)
				bitwiseField |= 0x0400;
			if(Sort)
				bitwiseField |= 0x0800;
			if(AutoFilter)
				bitwiseField |= 0x1000;
			if(PivotTables)
				bitwiseField |= 0x2000;
			if(SetUnlockedCells)
				bitwiseField |= 0x4000;
			writer.Write(bitwiseField);
		}
		public int GetSize() {
			return 4;
		}
	}
	#endregion
}
