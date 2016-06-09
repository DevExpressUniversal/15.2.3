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
	#region XlsCommandPivotFormat  -- SxFormat --
	public class XlsCommandPivotFormat : XlsCommandBase {
		#region Fields
		int countDifferentialFormat;
		#endregion
		#region Properties
		public bool IsFormatApply { get; set; }
		public int DifferentialFormatLength {
			get { return countDifferentialFormat; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "DifferentialFormatLength");
				if (!IsFormatApply && value > 0)
					throw new ArgumentException("MUST be zero if rlType is zero");
				countDifferentialFormat = value;
			}
		}
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			IsFormatApply = (reader.ReadInt16() & 1) > 0;
			DifferentialFormatLength = reader.ReadUInt16();
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			XlsBuildPivotView builder = contentBuilder.CurrentBuilderPivotView;
			if (builder != null) {
				builder.IsPivotSelection = false;
				builder.IsPivotFormat = true;
				builder.DifferentialFormatLength = DifferentialFormatLength;
				PivotFormat format = new PivotFormat(contentBuilder.CurrentSheet.Workbook);
				builder.PivotTable.Formats.AddCore(format);
				builder.Format = format;
				if (IsFormatApply)
					format.FormatAction = FormatAction.Formatting;
				else
					format.FormatAction = FormatAction.Blank;
			}
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((short)(IsFormatApply ? 1 : 0));
			writer.Write((ushort)DifferentialFormatLength);
		}
		protected override short GetSize() {
			return (short)(4);
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
}
