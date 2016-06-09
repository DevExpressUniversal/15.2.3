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
	#region XlsCommandPivotPageAxis -- SXPI --
	public class XlsCommandPivotPageAxis : XlsCommandRecordBase {
		#region Fields
		static short[] typeCodes = new short[] {
			0x003c 
		};
		readonly List<XlsPivotPageAxisReWriter> pivotPageAxis = new List<XlsPivotPageAxisReWriter>();
		#endregion
		#region Properties
		public List<XlsPivotPageAxisReWriter> PageAxis { get { return pivotPageAxis; } }
		#endregion
		#region Methods
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			XlsBuildPivotView builderPivotView = contentBuilder.CurrentBuilderPivotView;
			if (contentBuilder.ContentType == XlsContentType.Sheet && builderPivotView != null && builderPivotView.PivotPageCount != 0) {
				using (XlsCommandStream axisStream = new XlsCommandStream(contentBuilder, reader, typeCodes, Size)) {
					using (BinaryReader axisReader = new BinaryReader(axisStream)) {
						for (int i = 0; i < builderPivotView.PivotPageCount; i++)
							pivotPageAxis.Add(XlsPivotPageAxisReWriter.FromStream(axisReader));
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
				PivotPageFieldCollection pageFields = builder.PivotTable.PageFields;
				foreach (XlsPivotPageAxisReWriter element in PageAxis) {
					PivotPageField pElement = new PivotPageField(builder.PivotTable, element.PivotFieldIndex);
					if (element.PivotItemIndex < 0x7FFD)
						pElement.SetItemIndexCore(element.PivotItemIndex);
					pageFields.AddCore(pElement);
				}
				builder.CurrentIndexPageAxis = 0;
			}
		}
		public override IXlsCommand GetInstance() {
			pivotPageAxis.Clear();
			return this;
		}
		#endregion
	}
	public class XlsPivotPageAxisReWriter : IEquatable<XlsPivotPageAxisReWriter> {
		int pivotItemIndex;
		#region Properties
		public int PivotFieldIndex { get; set; }
		public int PivotItemIndex {
			get { return pivotItemIndex; }
			set {
				ValueChecker.CheckValue(value, 0, 0x7FFD, "PivotItemIndex");
				pivotItemIndex = value; 
			} 
		}
		public int DropDownId { get; set; }
		#endregion
		#region Methods
		public static XlsPivotPageAxisReWriter FromStream(BinaryReader reader) {
			XlsPivotPageAxisReWriter result = new XlsPivotPageAxisReWriter();
			result.Read(reader);
			return result;
		}
		protected void Read(BinaryReader reader) {
			PivotFieldIndex = reader.ReadInt16();
			PivotItemIndex = reader.ReadInt16();
			DropDownId = reader.ReadInt16();
		}
		public void Write(BinaryWriter writer) {
			writer.Write((short)PivotFieldIndex);
			writer.Write((short)PivotItemIndex);
			writer.Write((short)DropDownId);
		}
		public override bool Equals(Object other) {
			if (typeof(XlsPivotPageAxisReWriter) != other.GetType())
				return false;
			return this.Equals((XlsPivotPageAxisReWriter)other);
		}
		public bool Equals(XlsPivotPageAxisReWriter other) {
			if (this.PivotFieldIndex == other.PivotFieldIndex)
				if (this.PivotItemIndex == other.PivotItemIndex)
					if (this.DropDownId == other.DropDownId)
						return true;
			return false;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		#endregion
	}
	#endregion
}
