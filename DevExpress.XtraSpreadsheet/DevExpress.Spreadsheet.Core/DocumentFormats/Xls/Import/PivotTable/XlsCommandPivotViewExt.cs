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
	#region XlsCommandPivotViewExt  -- SXViewEx --
	public class XlsCommandPivotViewExt : XlsCommandBase {
		#region Fields
		int hierarchyRecords;
		int extPageAxisRecords;
		int extPivotFieldRecords;
		#endregion
		#region Properties
		public int OldHeader {
			get { return 0x80C; }
			set {
				if (value != 0x80C)
					throw new ArgumentException("The value of the frtHeaderOld.rt field MUST be 0x80C");
			}
		}
		public int CountHierarchyRecords {
			get { return hierarchyRecords; }
			set {
				ValueChecker.CheckValue(value, 1, int.MaxValue, "NumberHierarchyRecords");
				hierarchyRecords = value;
			}
		}
		public int CountExtPageAxisRecords {
			get { return extPageAxisRecords; }
			set {
				ValueChecker.CheckValue(value, 0, int.MaxValue, "NumberExtPageAxisRecords");
				extPageAxisRecords = value;
			}
		}
		public int CountExtPivotFieldRecords {
			get { return extPivotFieldRecords; }
			set {
				ValueChecker.CheckValue(value, 0, int.MaxValue, "NumberExtPivotFieldRecords");
				extPivotFieldRecords = value;
			}
		}
		public byte[] GBFuture { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			OldHeader = reader.ReadInt32();
			CountHierarchyRecords = reader.ReadInt32();
			CountExtPageAxisRecords = reader.ReadInt32();
			CountExtPivotFieldRecords = reader.ReadInt32();
			int countFutureByte = (int)reader.ReadUInt32();
			if (countFutureByte > 0)
				GBFuture = reader.ReadBytes(countFutureByte);
			else
				GBFuture = new byte[0];
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			XlsBuildPivotView builder = contentBuilder.CurrentBuilderPivotView;
			if (builder != null) {
				builder.PivotHierarchyCount = CountHierarchyRecords;
				builder.PivotPageAxisExtCount = CountExtPageAxisRecords;
				builder.PivotFieldOLAPExtCount = CountExtPivotFieldRecords;
			}
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write(OldHeader);
			writer.Write(CountHierarchyRecords);
			writer.Write(CountExtPageAxisRecords);
			writer.Write(CountExtPivotFieldRecords);
			if (GBFuture != null && GBFuture.Length > 0) {
				writer.Write((uint)GBFuture.Length);
				writer.Write(GBFuture);
			}
			else
				writer.Write((uint)0);
		}
		protected override short GetSize() {
			int result = 20;
			if (GBFuture != null)
				result += GBFuture.Length;
			return (short)result;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
}
