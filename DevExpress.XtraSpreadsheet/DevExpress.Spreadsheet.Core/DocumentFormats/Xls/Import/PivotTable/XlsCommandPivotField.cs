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
	#region XlsCommandPivotField  -- Sxvd --
	public class XlsCommandPivotField : XlsCommandBase {
		#region Fields
		XLUnicodeStringNoCch fieldName = new XLUnicodeStringNoCch();
		#endregion
		#region Properties
		public PivotTableAxis FieldAxis { get; set; }
		public int CountSubtotal { get; set; }
		public int Subtotal { get; set; }
		public int CountItem { get; set; }
		public string FieldName {
			get { return fieldName.Value; }
			set {
				ValueChecker.CheckLength(value, 255, "FieldName");
				HasReadName = true;
				fieldName.Value = value;
			}
		}
		bool HasReadName { get; set; }
		#endregion
		#region Methods
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			FieldAxis = (PivotTableAxis)(reader.ReadUInt16() & 0x000F);
			CountSubtotal = reader.ReadUInt16();
			Subtotal = (reader.ReadUInt16() & 0x0FFF) << 1;
			CountItem = reader.ReadInt16();
			int lenFieldName = reader.ReadUInt16();
			HasReadName = false;
			if (lenFieldName != 0xFFFF){
				fieldName = XLUnicodeStringNoCch.FromStream(reader, lenFieldName);
				HasReadName = true;
			}
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			XlsBuildPivotView builder = contentBuilder.CurrentBuilderPivotView;
			if (builder != null) {
				PivotField pivotField = new PivotField(builder.PivotTable);
				builder.PivotTable.Fields.AddCore(pivotField);
				builder.SetPivotField(pivotField, CountItem);
				pivotField.Axis = (PivotTableAxis)FieldAxis;
				pivotField.IsDataField = ((short)FieldAxis & 8) > 0;
				pivotField.SetSubtotal((PivotFieldItemType)Subtotal);
				if (HasReadName)
					pivotField.SetNameCore(FieldName);
			}
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((ushort)FieldAxis);
			writer.Write((ushort)CountSubtotal);
			writer.Write((ushort)(Subtotal >> 1));
			writer.Write((short)CountItem);
			if (FieldName.Length != 0) {
				writer.Write((ushort)FieldName.Length);
				fieldName.Write(writer);
			}
			else{
			writer.Write((ushort)0xFFFF);
			}
		}
		protected override short GetSize() {
			int result = 10;
			if (FieldName.Length > 0)
				result += fieldName.Length;
			return (short)result;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
		#endregion
	}
	#endregion
}
