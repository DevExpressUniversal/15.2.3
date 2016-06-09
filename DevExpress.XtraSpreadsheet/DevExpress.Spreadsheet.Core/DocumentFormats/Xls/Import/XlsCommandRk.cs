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
using System.Runtime.InteropServices;
using System.Text;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	public class XlsCommandRk : XlsCommandCellBase {
		XlsContentRk content = new XlsContentRk();
		#region Properties
		public double Value { get { return content.Value; } set { content.Value = value; } }
		#endregion
		protected override void ApplyCellValueCore(ICell cell) {
			cell.AssignValueCore(Value);
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
		protected override IXlsContent GetContent() {
			return content;
		}
	}
	#region RcRec
	public class RkRec {
		RkNumber rk = new RkNumber(0);
		#region Properties
		public int FormatIndex { get; set; }
		public RkNumber Rk {
			get { return rk; }
			set {
				if(value == null)
					rk = new RkNumber(0);
				else
					rk = value;
			} 
		}
		#endregion
		public static RkRec Read(XlsReader reader) {
			RkRec result = new RkRec();
			result.FormatIndex = reader.ReadUInt16();
			result.Rk = new RkNumber(reader.ReadInt32());
			return result;
		}
		public void Write(BinaryWriter writer) {
			writer.Write((ushort)FormatIndex);
			writer.Write(Rk.GetRawValue());
		}
	}
	#endregion
	#region RkNumber
	public class RkNumber {
		#region DoubleInt64Union
		[StructLayout(LayoutKind.Explicit)]
		struct DoubleInt64Union {
			[FieldOffset(0)]
			public double DoubleValue;
			[FieldOffset(0)]
			public Int64 Int64Value;
		}
		#endregion
		#region Fields
		const long valueMask = 0xfffffffc;
		const long low34BitsMask = 0x3ffffffff;
		const int int30MinValue = -536870912; 
		const int int30MaxValue = 536870911; 
		int rkType;
		double value;
		#endregion
		public RkNumber(Int32 rawValue) {
			this.rkType = rawValue & 0x03;
			DoubleInt64Union helper = new DoubleInt64Union();
			if (!IsInt) 
				helper.Int64Value = (rawValue & valueMask) << 32;
			else
				helper.DoubleValue = (double)((int)((rawValue & valueMask)) >> 2);
			this.value = (X100) ? helper.DoubleValue / 100 : helper.DoubleValue;
		}
		#region Properties
		public double Value {
			get { return this.value; }
			set {
				if(!IsRkValue(value))
					throw new ArgumentException("value can't be represented as RkNumber");
				this.value = value;
				this.rkType = GetRkType(value);
			}
		}
		protected internal bool X100 { get { return (rkType & 0x01) != 0; } }
		protected internal bool IsInt { get { return (rkType & 0x02) != 0; } }
		#endregion
		public Int32 GetRawValue() {
			DoubleInt64Union helper = new DoubleInt64Union();
			if (X100)
				helper.DoubleValue = Value * 100;
			else
				helper.DoubleValue = Value;
			int result;
			if (!IsInt) 
				result = (int)((helper.Int64Value >> 32) & valueMask);
			else
				result = ((int)helper.DoubleValue) << 2;
			result |= this.rkType;
			return result;
		}
		public static bool IsRkValue(double value) {
			return GetRkType(value) != -1;
		}
		static int GetRkType(double value) {
			DoubleInt64Union helper = new DoubleInt64Union();
			helper.DoubleValue = value;
			if((helper.Int64Value & low34BitsMask) == 0) 
				return 0;
			if(CanBePresentedAsInt30(value))
				return 2;
			try {
				helper.DoubleValue = (double)(value * 100);
				if((helper.Int64Value & low34BitsMask) == 0)
					return 1;
				if(CanBePresentedAsInt30(value * 100))
					return 3;
			}
			catch(OverflowException) { }
			return -1;
		}
		static bool CanBePresentedAsInt30(double value) {
			double truncated = WorksheetFunctionBase.Truncate(value);
			return truncated == value && truncated >= int30MinValue && truncated <= int30MaxValue;
		}
	}
	#endregion
}
