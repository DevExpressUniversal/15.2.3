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
	#region XlsCommandPivotAxis -- SxIvd --
	public class XlsCommandPivotAxis : XlsCommandRecordBase {
		#region Fields
		static short[] typeCodes = new short[] {
			0x003c 
		};
		readonly List<XlsPivotAxisReWriter> pivotAxis = new List<XlsPivotAxisReWriter>();
		#endregion
		#region Properties
		public List<XlsPivotAxisReWriter> Axis { get { return pivotAxis; } }
		#endregion
		#region Methods
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			XlsBuildPivotView builderPivotView = contentBuilder.CurrentBuilderPivotView;
			if (contentBuilder.ContentType == XlsContentType.Sheet && builderPivotView != null && builderPivotView.PivotAxisType != XlsPivotLinesType.None) {
				using (XlsCommandStream axisStream = new XlsCommandStream(contentBuilder, reader, typeCodes, Size)) {
					using (BinaryReader axisReader = new BinaryReader(axisStream)) {
						int count = builderPivotView.PivotAxisType == XlsPivotLinesType.Rows ? builderPivotView.PivotItemRowCount : builderPivotView.PivotItemColumnCount;
						for (int i = 0; i < count; i++)
							pivotAxis.Add(XlsPivotAxisReWriter.FromStream(axisReader));
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
			if (builder != null) {
				PivotTableColumnRowFieldIndices listField = null;
				if (builder.PivotAxisType == XlsPivotLinesType.Rows) {
					listField = builder.PivotTable.RowFields;
					builder.PivotAxisType = builder.PivotItemColumnCount != 0 ? XlsPivotLinesType.Columns : XlsPivotLinesType.None;
				}
				else if (builder.PivotAxisType == XlsPivotLinesType.Columns) {
					listField = builder.PivotTable.ColumnFields;
					builder.PivotAxisType = XlsPivotLinesType.None;
				}
				foreach (XlsPivotAxisReWriter element in pivotAxis)
					listField.AddCore(new PivotFieldReference(element.Value));
			}
		}
		public override IXlsCommand GetInstance() {
			pivotAxis.Clear();
			return this;
		}
		#endregion
	}
	public class XlsPivotAxisReWriter : IEquatable<XlsPivotAxisReWriter> {
		int value = 0;
		public int Value { 
			get {return value;}
			set {
				ValueChecker.CheckValue(value, -2, short.MaxValue, "XlsPivotAxisReWriter Value");
				this.value = value; 
			}
		}
		public static XlsPivotAxisReWriter FromStream(BinaryReader reader) {
			XlsPivotAxisReWriter result = new XlsPivotAxisReWriter();
			result.Read(reader);
			return result;
		}
		protected void Read(BinaryReader reader) {
			Value = reader.ReadInt16();
		}
		public void Write(BinaryWriter writer) {
			writer.Write((short)Value);
		}
		public override bool Equals(Object other) {
			if (typeof(XlsPivotAxisReWriter) != other.GetType())
				return false;
			return this.value == ((XlsPivotAxisReWriter)other).value;
		}
		public bool Equals(XlsPivotAxisReWriter other) {
			return this.value == other.value;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	#endregion
}
