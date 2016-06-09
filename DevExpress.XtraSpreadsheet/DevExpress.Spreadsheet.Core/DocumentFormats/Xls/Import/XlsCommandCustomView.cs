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
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	public class XlsCommandCustomViewBegin : XlsCommandBase {
		Guid customViewIndentity = new Guid();
		CellRangeInfo topLeftVisibleArea = new CellRangeInfo();
		public Guid CustomViewIdentity {
			get { return customViewIndentity; }
			set { customViewIndentity = value; }
		}
		public int TabId { get; set; }
		public int ZoomLevel { get; set; }
		public int GridlinesColorIndex { get; set; }
		public ViewPaneType Pane { get; set; }
		public bool ShowPageBreaks { get; set; }
		public bool ShowFormulas { get; set; }
		public bool ShowGridlines { get; set; }
		public bool ShowRowColumnHeadings { get; set; }
		public bool ShowOutlineSymbols { get; set; }
		public bool ShowZeroValues { get; set; }
		public bool CenterHorizontal { get; set; }
		public bool CenterVertical { get; set; }
		public bool PrintRowColumnHeadings { get; set; }
		public bool PrintGridlines { get; set; }
		public bool FitToPage { get; set; }
		public bool PrintArea { get; set; }
		public bool OnePrintArea { get; set; }
		public bool InFilterMode { get; set; }
		public bool AutoFilterIconShown { get; set; }
		public bool Frozen { get; set; }
		public bool FrozenWithoutPaneSplit { get; set; }
		public bool IsSplitVertically { get; set; }
		public bool IsSplitHorizontally { get; set; }
		public bool HiddenRowsPresent { get; set; }
		public bool HiddenColumnsPresent { get; set; }
		public bool FilterUnique { get; set; }
		public bool InPageBreakPreview { get; set; }
		public bool InPageLayoutView { get; set; }
		public bool ShowRullers { get; set; }
		public CellRangeInfo TopLeftVisibleArea {
			get { return topLeftVisibleArea; }
			set {
				Guard.ArgumentNotNull(value, "TopLeftVisibleArea");
				topLeftVisibleArea = value;
			}
		}
		public double SplitXPosition { get; set; }
		public double SplitYPosition { get; set; }
		public int RightPaneFirstColumn { get; set; }
		public int BottomPaneFirstRow { get; set; }
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			byte[] guidBytes = reader.ReadBytes(16);
			CustomViewIdentity = new Guid(guidBytes);
			TabId = reader.ReadUInt16();
			reader.ReadUInt16(); 
			ZoomLevel = reader.ReadInt32();
			GridlinesColorIndex = reader.ReadUInt16();
			reader.ReadUInt16(); 
			Pane = ViewPaneTypeHelper.CodeToPaneType(reader.ReadByte());
			reader.ReadUInt16(); 
			reader.ReadByte(); 
			uint bitwiseField = reader.ReadUInt32();
			ShowPageBreaks = Convert.ToBoolean(bitwiseField & 0x00000001);
			ShowFormulas = Convert.ToBoolean(bitwiseField & 0x00000002);
			ShowGridlines = Convert.ToBoolean(bitwiseField & 0x00000004);
			ShowRowColumnHeadings = Convert.ToBoolean(bitwiseField & 0x00000008);
			ShowOutlineSymbols = Convert.ToBoolean(bitwiseField & 0x00000010);
			ShowZeroValues = !Convert.ToBoolean(bitwiseField & 0x000000020);
			CenterHorizontal = Convert.ToBoolean(bitwiseField & 0x000000040);
			CenterVertical = Convert.ToBoolean(bitwiseField & 0x000000080);
			PrintRowColumnHeadings = Convert.ToBoolean(bitwiseField & 0x00000100);
			PrintGridlines = Convert.ToBoolean(bitwiseField & 0x00000200);
			FitToPage = Convert.ToBoolean(bitwiseField & 0x00000400);
			PrintArea = Convert.ToBoolean(bitwiseField & 0x00000800);
			OnePrintArea = Convert.ToBoolean(bitwiseField & 0x00001000);
			InFilterMode = Convert.ToBoolean(bitwiseField & 0x00002000);
			AutoFilterIconShown = Convert.ToBoolean(bitwiseField & 0x00004000);
			Frozen = Convert.ToBoolean(bitwiseField & 0x00008000);
			FrozenWithoutPaneSplit = Convert.ToBoolean(bitwiseField & 0x00010000);
			IsSplitVertically = Convert.ToBoolean(bitwiseField & 0x00020000);
			IsSplitHorizontally = Convert.ToBoolean(bitwiseField & 0x00040000);
			HiddenRowsPresent = !Convert.ToBoolean(bitwiseField & 0x00080000);
			HiddenColumnsPresent = Convert.ToBoolean(bitwiseField & 0x00100000);
			FilterUnique = Convert.ToBoolean(bitwiseField & 0x02000000);
			InPageBreakPreview = Convert.ToBoolean(bitwiseField & 0x04000000);
			InPageLayoutView = Convert.ToBoolean(bitwiseField & 0x08000000);
			ShowRullers = Convert.ToBoolean(bitwiseField & 0x20000000);
			int firstRow = reader.ReadUInt16();
			int lastRow = reader.ReadUInt16();
			int firstColumn = reader.ReadUInt16();
			int lastColumn = reader.ReadUInt16();
			TopLeftVisibleArea = new CellRangeInfo(new CellPosition(firstColumn, firstRow), new CellPosition(lastColumn, lastRow));
			SplitXPosition = reader.ReadDouble();
			SplitYPosition =  reader.ReadDouble();
			RightPaneFirstColumn = reader.ReadUInt16();
			BottomPaneFirstRow = reader.ReadUInt16();
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
			if(contentBuilder.ContentType == XlsContentType.Sheet)
				contentBuilder.StartContent(XlsContentType.SheetCustomView);
			else if(contentBuilder.ContentType == XlsContentType.Chart)
				contentBuilder.StartContent(XlsContentType.ChartCustomView);
			else if(contentBuilder.ContentType == XlsContentType.ChartSheet)
				contentBuilder.StartContent(XlsContentType.ChartSheetCustomView);
			else if(contentBuilder.ContentType == XlsContentType.MacroSheet)
				contentBuilder.StartContent(XlsContentType.MacroSheetCustomView);
			else if (contentBuilder.ContentType == XlsContentType.VisualBasicModule)
				contentBuilder.StartContent(XlsContentType.VisualBasicModuleCustomView);
			else
				contentBuilder.ThrowInvalidFile("Custom view wrong substream!");
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write(CustomViewIdentity.ToByteArray());
			writer.Write((ushort)TabId);
			writer.Write((ushort)0);
			writer.Write(ZoomLevel);
			writer.Write((ushort)GridlinesColorIndex);
			writer.Write((ushort)0);
			writer.Write(ViewPaneTypeHelper.PaneTypeToCode(Pane));
			writer.Write((ushort)0);
			writer.Write((byte)0);
			uint bitwiseField = 0;
			if(ShowPageBreaks) bitwiseField |= 0x00000001;
			if(ShowFormulas) bitwiseField |= 0x00000002;
			if(ShowGridlines) bitwiseField |= 0x00000004;
			if(ShowRowColumnHeadings) bitwiseField |= 0x00000008;
			if(ShowOutlineSymbols) bitwiseField |= 0x00000010;
			if(!ShowZeroValues) bitwiseField |= 0x000000020;
			if(CenterHorizontal) bitwiseField |= 0x000000040;
			if(CenterVertical) bitwiseField |= 0x000000080;
			if(PrintRowColumnHeadings) bitwiseField |= 0x00000100;
			if(PrintGridlines) bitwiseField |= 0x00000200;
			if(FitToPage) bitwiseField |= 0x00000400;
			if(PrintArea) bitwiseField |= 0x00000800;
			if(OnePrintArea) bitwiseField |= 0x00001000;
			if(InFilterMode) bitwiseField |= 0x00002000;
			if(AutoFilterIconShown) bitwiseField |= 0x00004000;
			if(Frozen) bitwiseField |= 0x00008000;
			if(FrozenWithoutPaneSplit) bitwiseField |= 0x00010000;
			if(IsSplitVertically) bitwiseField |= 0x00020000;
			if(IsSplitHorizontally) bitwiseField |= 0x00040000;
			if(!HiddenRowsPresent) bitwiseField |= 0x00080000;
			if(HiddenColumnsPresent) bitwiseField |= 0x00100000;
			if(FilterUnique) bitwiseField |= 0x02000000;
			if(InPageBreakPreview) bitwiseField |= 0x04000000;
			if(InPageLayoutView) bitwiseField |= 0x08000000;
			if(ShowRullers) bitwiseField |= 0x20000000;
			writer.Write(bitwiseField);
			writer.Write((ushort)TopLeftVisibleArea.First.Row);
			writer.Write((ushort)TopLeftVisibleArea.Last.Row);
			writer.Write((ushort)TopLeftVisibleArea.First.Column);
			writer.Write((ushort)TopLeftVisibleArea.Last.Column);
			writer.Write(SplitXPosition);
			writer.Write(SplitYPosition);
			writer.Write((ushort)RightPaneFirstColumn);
			writer.Write((ushort)BottomPaneFirstRow);
		}
		protected override short GetSize() {
			return 64;
		}
		public override IXlsCommand GetInstance() {
			return new XlsCommandCustomViewBegin();
		}
	}
	public class XlsCommandCustomViewEnd : XlsCommandBase {
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			reader.ReadUInt16();
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
			contentBuilder.EndContent();
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((ushort)1);
		}
		protected override short GetSize() {
			return 2;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
}
