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
	public class PageBreakRecord {
		#region Fields
		public const int RecordSize = 6;
		int position;
		int start;
		int end;
		#endregion
		#region Properties
		public int Position {
			get { return position; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "Position");
				position = value;
			} 
		}
		public int Start {
			get { return start; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "Start");
				start = value;
			} 
		}
		public int End {
			get { return end; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "End");
				end = value;
			}
		}
		#endregion
		public static PageBreakRecord FromStream(XlsReader reader) {
			PageBreakRecord result = new PageBreakRecord();
			result.Read(reader);
			return result;
		}
		protected void Read(XlsReader reader) {
			Position = reader.ReadUInt16();
			Start = reader.ReadUInt16();
			End = reader.ReadUInt16();
		}
		public void Write(BinaryWriter writer) {
			writer.Write((ushort)Position);
			writer.Write((ushort)Start);
			writer.Write((ushort)End);
		}
		public override bool Equals(object obj)
		{
			PageBreakRecord other = obj as PageBreakRecord;
			if(other == null) return false;
 			return Position == other.Position &&
				Start == other.Start &&
				End == other.End;
		}
		public override int GetHashCode() {
			return Position;
		}
	}
	public abstract class XlsCommandPageBreaksBase : XlsCommandBase {
		List<PageBreakRecord> items = new List<PageBreakRecord>();
		public IList<PageBreakRecord> Items { get { return items; } }
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			int count = reader.ReadUInt16();
			if (count == 0)
				return;
			int breakRecordSize = (Size - 2) / count;
			if (breakRecordSize == 2) {
				for (int i = 0; i < count; i++) {
					int position = reader.ReadUInt16();
					Items.Add(new PageBreakRecord() { Position = position });
				}
			}
			else {
				for (int i = 0; i < count; i++) {
					Items.Add(PageBreakRecord.FromStream(reader));
				}
			}
		}
		protected override void WriteCore(BinaryWriter writer) {
			int count = Items.Count;
			writer.Write((ushort)count);
			for(int i = 0; i < count; i++) {
				Items[i].Write(writer);
			}
		}
		protected override short GetSize() {
			return (short)(sizeof(ushort) + Items.Count * PageBreakRecord.RecordSize);
		}
	}
	public class XlsCommandHorizontalPageBreaks : XlsCommandPageBreaksBase {
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			PageBreakCollection breaks = contentBuilder.CurrentSheet.RowBreaks;
			foreach(PageBreakRecord item in Items)
				breaks.TryInsert(item.Position);
		}
		public override IXlsCommand GetInstance() {
			return new XlsCommandHorizontalPageBreaks();
		}
	}
	public class XlsCommandVerticalPageBreaks : XlsCommandPageBreaksBase {
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			PageBreakCollection breaks = contentBuilder.CurrentSheet.ColumnBreaks;
			foreach(PageBreakRecord item in Items)
				breaks.TryInsert(item.Position);
		}
		public override IXlsCommand GetInstance() {
			return new XlsCommandVerticalPageBreaks();
		}
	}
}
