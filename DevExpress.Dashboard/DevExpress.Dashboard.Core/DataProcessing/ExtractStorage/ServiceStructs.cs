#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Runtime.InteropServices;
using DevExpress.Compatibility.System;
namespace DevExpress.DashboardCommon.DataProcessing {
	public enum CompressionType : int {
		Unknown = 1,
		Dictionary = 2,
	}
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1028")]
	public enum OffsetTableType : byte {
		Unknown = 1,
		TypedValues = 2,
		BLOB = 4,
	}
	#region service structs
	[StructLayout(LayoutKind.Sequential, Pack = 1), Serializable]
	public struct TableHeader {
		Int32 version;
		Int32 columnCount;
		Int32 rowCount;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		char[] name;
		public Int32 Version { get { return version; } set { version = value; } }
		public Int32 ColumnCount { get { return columnCount; } set { columnCount = value; } }
		public Int32 RowCount { get { return rowCount; } set { rowCount = value; } }
		public char[] Name { get { return name; } set { name = value; } }
	}
	[StructLayout(LayoutKind.Sequential, Size = 268, Pack = 1), Serializable]
	public struct ColumnHeader {
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		char[] name;
		Int32 uniqueCount;
		Int32 type;
		CompressionType compressionType;
		Int32 compressedBlocksCount;
		Int32 dataOffset;
		Int32 recordsCount;
		public char[] Name { get { return name; } set { name = value; } }
		public Int32 UniqueCount { get { return uniqueCount; } set { uniqueCount = value; } }
		public Int32 Type { get { return type; } set { type = value; } }
		public CompressionType CompressionType { get { return compressionType; } set { compressionType = value; } }
		public Int32 CompressedBlocksCount { get { return compressedBlocksCount; } set { compressedBlocksCount = value; } }
		public Int32 DataOffset { get { return dataOffset; } set { dataOffset = value; } }
		public Int32 RecordsCount { get { return recordsCount; } set { recordsCount = value; } }
	}
	[StructLayout(LayoutKind.Sequential, Pack = 1), Serializable]
	public struct OffsetRecord {
		Int32 minIndex;
		Int32 maxIndex;
		Int32 offset;
		Int32 length;
		public Int32 MinIndex { get { return minIndex; } set { minIndex = value; } }
		public Int32 MaxIndex { get { return maxIndex; } set { maxIndex = value; } }
		public Int32 Offset { get { return offset; } set { offset = value; } }
		public Int32 Length { get { return length; } set { length = value; } }
	}
	[StructLayout(LayoutKind.Sequential, Size = 1033, Pack = 1), Serializable]
	public struct OffsetTableStruct {
		Int32 nextTableOffset;
		OffsetTableType typeOfRecord;
		Int32 recordIndex;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		OffsetRecord[] records;
		public Int32 NextTableOffset { get { return nextTableOffset; } set { nextTableOffset = value; } }
		public OffsetTableType TypeOfRecord { get { return typeOfRecord; } set { typeOfRecord = value; } }
		public Int32 RecordIndex { get { return recordIndex; } set { recordIndex = value; } }
		public OffsetRecord[] Records { get { return records; } set { records = value; } }
	}
	#endregion
}
