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
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region IndexRecord
	#endregion
	#region IndexRecordStub
	public class IndexRecordStub : XlsContentIndex {
		int rowBlocksCount;
		long offset;
		public IndexRecordStub(int rowBlocksCount) {
			this.rowBlocksCount = rowBlocksCount;
		}
		public void ReplaceStub(BinaryWriter writer, XlsContentIndex indexRecord) {
			long savedPosition = writer.BaseStream.Position;
			writer.BaseStream.Seek(this.offset, SeekOrigin.Begin);
			indexRecord.Write(writer);
			writer.BaseStream.Seek(savedPosition, SeekOrigin.Begin);
		}
		public override void Write(BinaryWriter writer) {
			this.offset = writer.BaseStream.Position;
			writer.BaseStream.Seek(GetSize(), SeekOrigin.Current);
		}
		public override int GetSize() {
			return FixedPartSize + this.rowBlocksCount * VariablePartElementSize;
		}
	}
	#endregion
	#region FutureRecordHeaderOld
	public class FutureRecordHeaderOld : FutureRecordHeaderFlagsBase {
		public static FutureRecordHeaderOld FromStream(XlsReader reader) {
			FutureRecordHeaderOld result = new FutureRecordHeaderOld();
			result.Read(reader);
			return result;
		}
		public override short GetSize() {
			return 4;
		}
	}
	#endregion
	#region FutureRecordHeaderRef
	public class FutureRecordHeaderRef : FutureRecordHeaderFlagsBase {
		#region Static Members
		public static FutureRecordHeaderRef FromStream(XlsReader reader) {
			FutureRecordHeaderRef result = new FutureRecordHeaderRef();
			result.Read(reader);
			return result;
		}
		public static FutureRecordHeaderRef Create(CellRangeInfo cellRangeInfo, short recordTypeId) {
			FutureRecordHeaderRef result = new FutureRecordHeaderRef();
			result.Range = cellRangeInfo;
			result.RangeOfCells = true;
			result.RecordTypeId = recordTypeId;
			return result;
		}
		#endregion
		#region Fields
		CellRangeInfo range = new CellRangeInfo();
		#endregion
		#region Fields
		protected internal CellRangeInfo Range {
			get { return range; }
			set {
				if(value != null)
					range = value;
				else
					range = new CellRangeInfo();
			}
		}
		#endregion
		protected override void ReadCore(XlsReader reader) {
			base.ReadCore(reader);
			Ref8U ref8Info = Ref8U.FromStream(reader);
			this.range = ref8Info.CellRangeInfo;
		}
		protected override void WriteCore(BinaryWriter writer) {
			base.WriteCore(writer);
			Ref8U ref8Info = new Ref8U();
			ref8Info.CellRangeInfo = this.range;
			ref8Info.Write(writer);
		}
		public override short GetSize() {
			return 12;
		}
	}
	#endregion
	#region XlsFutureRecordHeaderFactory
	public static class XlsFutureRecordHeaderFactory {
		readonly static Dictionary<int, Type> products;
		static XlsFutureRecordHeaderFactory() {
			products = new Dictionary<int, Type>();
			Initialize();
		}
		static void Initialize() {
			AddProduct(0x0812, typeof(FutureRecordHeaderOld));
			AddProduct(0x0875, typeof(FutureRecordHeader));
			AddProduct(0x087f, typeof(TableFutureRecordHeader));
			AddProduct(0x089f, typeof(FutureRecordHeader));
		}
		static void AddProduct(int typeCode, Type prodType) {
			products.Add(typeCode, prodType);
		}
		public static FutureRecordHeaderBase Create(short typeCode) {
			try {
				if(!products.ContainsKey(typeCode))
					return null;
				Type prodType = products[typeCode];
				ConstructorInfo prodConstructor = prodType.GetConstructor(new Type[] { });
				FutureRecordHeaderBase result = prodConstructor.Invoke(new object[] { }) as FutureRecordHeaderBase;
				result.RecordTypeId = typeCode;
				return result;
			}
			catch { }
			return null;
		}
		public static FutureRecordHeaderBase FromStream(short typeCode, XlsReader reader) {
			Guard.ArgumentNotNull(reader, "reader");
			try {
				if(!products.ContainsKey(typeCode))
					return null;
				Type prodType = products[typeCode];
				MethodInfo method = prodType.GetMethod("FromStream", new Type[] { typeof(XlsReader) });
				if(method != null && method.IsStatic)
					return method.Invoke(null, new object[] { reader }) as FutureRecordHeaderBase;
			}
			catch { }
			return null;
		}
	}
	#endregion
}
