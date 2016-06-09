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
	#region XlsCommandPivotViewExt9  -- SXViewEx9 --
	public class XlsCommandPivotViewExt9 : XlsCommandBase {
		#region Fields
		private enum FirstViewExt{
			FrtAlert = 1,
		}
		private enum SecondViewExt {
			PrintTitles = 1, 
			LineMode = 2, 
			RepeatItemsOnEachPrintedPage = 5, 
		}
		BitwiseContainer first = new BitwiseContainer(16);
		BitwiseContainer second = new BitwiseContainer(32);
		XLUnicodeString grandName = new XLUnicodeString();
		#endregion
		#region Properties
		public int CommandId {
			get { return 0x0810; }
			set {
				if (value != 0x0810)
					throw new ArgumentException("The value MUST be 0x0810");
			}
		}
		public bool IsSupport {
			get { return first.GetBoolValue((int)FirstViewExt.FrtAlert); }
			set { first.SetBoolValue((int)FirstViewExt.FrtAlert, value); }
		}
		public bool IsPrintTitles {
			get { return second.GetBoolValue((int)SecondViewExt.PrintTitles); }
			set { second.SetBoolValue((int)SecondViewExt.PrintTitles, value); }
		}
		public bool IsLineMode {
			get { return second.GetBoolValue((int)SecondViewExt.LineMode); }
			set { second.SetBoolValue((int)SecondViewExt.LineMode, value); }
		}
		public bool IsPrintItemTitles {
			get { return second.GetBoolValue((int)SecondViewExt.RepeatItemsOnEachPrintedPage); }
			set { second.SetBoolValue((int)SecondViewExt.RepeatItemsOnEachPrintedPage, value); }
		}
		public int AutoFormat { get; set; }
		public string GrandName {
			get { return grandName.Value; }
			set {
				ValueChecker.CheckLength(value, 255, "GrandName");
				grandName.Value = value;
			}
		}
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			CommandId = reader.ReadInt16();
			first.ShortContainer = reader.ReadInt16();
			reader.ReadInt32(); 
			second.IntContainer = reader.ReadInt32();
			AutoFormat = reader.ReadInt16();
			grandName = XLUnicodeString.FromStream(reader);
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			XlsBuildPivotView builder = contentBuilder.CurrentBuilderPivotView;
			if (builder != null) {
				PivotTable table = builder.PivotTable;
				table.FieldPrintTitles = IsPrintTitles;
				table.Outline = IsLineMode;
				table.ItemPrintTitles = IsPrintItemTitles;
				table.AutoFormatId = AutoFormat;
				table.SetGrandTotalCaptionCore(GrandName);
			}
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((ushort)CommandId);
			writer.Write((short)first.ShortContainer);
			writer.Write((int)0); 
			writer.Write((int)second.IntContainer);
			writer.Write((short)AutoFormat);
			grandName.Write(writer);
		}
		protected override short GetSize() {
			int result = 14;
			result += grandName.Length;
			return (short) result;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
}
