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
	#region XlsCommandSheetViewInformation
	public class XlsCommandSheetViewInformation : XlsCommandBase {
		#region Fields
		const short recordSize = 18;
		const int defaultGridlinesColorIndex = 64;
		int topRowIndex;
		int leftColumnIndex;
		int gridlinesColorIndex;
		int zoomScalePageBreakPreview;
		int zoomScaleNormalView;
		#endregion
		#region Properties
		public bool ShowFormulas { get; set; }
		public bool ShowGridlines { get; set; }
		public bool ShowRowColumnHeadings { get; set; }
		public bool Frozen { get; set; }
		public bool ShowZeroValues { get; set; }
		public bool GridlinesInDefaultColor { get { return gridlinesColorIndex == defaultGridlinesColorIndex; } }
		public bool RightToLeft { get; set; }
		public bool ShowOutlineSymbols { get; set; }
		public bool FrozenWithoutPaneSplit { get; set; }
		public bool SheetTabIsSelected { get; set; }
		public bool CurrentlyDisplayed { get; set; }
		public bool InPageBreakPreview { get; set; }
		public int TopRowIndex {
			get { return topRowIndex; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "TopRowIndex");
				topRowIndex = value;
			}
		}
		public int LeftColumnIndex {
			get { return leftColumnIndex; }
			set {
				ValueChecker.CheckValue(value, 0, byte.MaxValue, "LeftColumnIndex");
				leftColumnIndex = value;
			}
		}
		public int GridlinesColorIndex {
			get { return gridlinesColorIndex; }
			set {
				ValueChecker.CheckValue(value, 0, defaultGridlinesColorIndex, "GridlinesColorIndex");
				gridlinesColorIndex = value;
			}
		}
		public int ZoomScalePageBreakPreview {
			get { return zoomScalePageBreakPreview; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "ZoomScalePageBreakPreview");
				zoomScalePageBreakPreview = value;
			}
		}
		public int ZoomScaleNormalView {
			get { return zoomScaleNormalView; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "ZoomScaleNormalView");
				zoomScaleNormalView = value;
			}
		}
		public bool IsChartSheetCommand { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			ushort bitwiseField = reader.ReadUInt16();
			if(contentBuilder.ContentType != XlsContentType.Chart && contentBuilder.ContentType != XlsContentType.ChartSheet) {
				ShowFormulas = Convert.ToBoolean(bitwiseField & 0x0001);
				ShowGridlines = Convert.ToBoolean(bitwiseField & 0x0002);
				ShowRowColumnHeadings = Convert.ToBoolean(bitwiseField & 0x0004);
				Frozen = Convert.ToBoolean(bitwiseField & 0x0008);
				ShowZeroValues = Convert.ToBoolean(bitwiseField & 0x0010);
				RightToLeft = Convert.ToBoolean(bitwiseField & 0x0040);
				ShowOutlineSymbols = Convert.ToBoolean(bitwiseField & 0x0080);
				FrozenWithoutPaneSplit = Convert.ToBoolean(bitwiseField & 0x0100);
			}
			SheetTabIsSelected = Convert.ToBoolean(bitwiseField & 0x0200);
			if(contentBuilder.ContentType != XlsContentType.Chart && contentBuilder.ContentType != XlsContentType.ChartSheet) {
				CurrentlyDisplayed = Convert.ToBoolean(bitwiseField & 0x0400);
				InPageBreakPreview = Convert.ToBoolean(bitwiseField & 0x0800);
			}
			if(contentBuilder.ContentType == XlsContentType.Sheet || contentBuilder.ContentType == XlsContentType.MacroSheet) {
				TopRowIndex = reader.ReadUInt16();
				LeftColumnIndex = reader.ReadUInt16();
				GridlinesColorIndex = reader.ReadUInt16();
				if ((bitwiseField & 0x0020) != 0)
					GridlinesColorIndex = defaultGridlinesColorIndex;
				reader.ReadUInt16(); 
			}
			else {
				reader.Seek(8, SeekOrigin.Current);
			}
			if(Size == 10) return;
			if(contentBuilder.ContentType != XlsContentType.Chart && contentBuilder.ContentType != XlsContentType.ChartSheet) {
				this.zoomScalePageBreakPreview = reader.ReadUInt16();
				this.zoomScaleNormalView = reader.ReadUInt16();
				reader.ReadUInt16(); 
				reader.ReadUInt16(); 
			}
			else {
				reader.Seek(8, SeekOrigin.Current);
			}
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			DocumentModel documentModel = contentBuilder.DocumentModel;
			ModelWorksheetView view = contentBuilder.CurrentSheet.ActiveView;
			view.BeginUpdate();
			try {
				view.ShowFormulas = ShowFormulas;
				view.ShowGridlines = ShowGridlines;
				view.ShowRowColumnHeaders = ShowRowColumnHeadings;
				view.ShowZeroValues = ShowZeroValues;
				view.RightToLeft = RightToLeft;
				view.ShowOutlineSymbols = ShowOutlineSymbols;
				if(Frozen && FrozenWithoutPaneSplit)
					view.SplitState = ViewSplitState.Frozen;
				else if(Frozen)
					view.SplitState = ViewSplitState.FrozenSplit;
				else
					view.SplitState = ViewSplitState.Split;
				view.TabSelected = SheetTabIsSelected;
				view.ViewType = InPageBreakPreview ? SheetViewType.PageBreakPreview : SheetViewType.Normal;
				view.TopLeftCell = new CellPosition(LeftColumnIndex, TopRowIndex);
				if(!GridlinesInDefaultColor)
					view.GridlinesColor = documentModel.StyleSheet.Palette[GridlinesColorIndex];
				if(IsValidZoom(ZoomScalePageBreakPreview))
					view.ZoomScaleSheetLayoutView = ZoomScalePageBreakPreview;
				if(IsValidZoom(ZoomScaleNormalView))
					view.ZoomScaleNormal = ZoomScaleNormalView;
			}
			finally {
				view.EndUpdate();
			}
		}
		protected override void WriteCore(BinaryWriter writer) {
			ushort bitwiseField = 0;
			if(ShowFormulas) bitwiseField |= 0x0001;
			if(ShowGridlines) bitwiseField |= 0x0002;
			if(ShowRowColumnHeadings) bitwiseField |= 0x0004;
			if(Frozen) bitwiseField |= 0x0008;
			if(ShowZeroValues) bitwiseField |= 0x0010;
			if(GridlinesInDefaultColor) bitwiseField |= 0x0020;
			if(RightToLeft) bitwiseField |= 0x0040;
			if(ShowOutlineSymbols) bitwiseField |= 0x0080;
			if(FrozenWithoutPaneSplit) bitwiseField |= 0x0100;
			if(SheetTabIsSelected) bitwiseField |= 0x0200;
			if(CurrentlyDisplayed) bitwiseField |= 0x0400;
			if(InPageBreakPreview) bitwiseField |= 0x0800;
			writer.Write(bitwiseField);
			writer.Write((ushort)TopRowIndex);
			writer.Write((ushort)LeftColumnIndex);
			writer.Write((ushort)GridlinesColorIndex);
			writer.Write((ushort)0);
			if(!IsChartSheetCommand) {
				writer.Write((ushort)ZoomScalePageBreakPreview);
				writer.Write((ushort)ZoomScaleNormalView);
				writer.Write((ushort)0);
				writer.Write((ushort)0);
			}
		}
		protected override short GetSize() {
			return recordSize;
		}
		bool IsValidZoom(int value) {
			return (value == 0) || (value >= 10 && value <= 400);
		}
		public override IXlsCommand GetInstance() {
			return new XlsCommandSheetViewInformation();
		}
	}
	#endregion
	#region XlsCommandSheetViewScale
	public class XlsCommandSheetViewScale : XlsCommandBase {
		#region Fields
		int numerator = 1;
		int denominator = 1;
		#endregion
		#region Properties
		public int Numerator {
			get { return numerator; }
			set {
				ValueChecker.CheckValue(value, 1, Int16.MaxValue);
				numerator = value;
			}
		}
		public int Denominator {
			get { return denominator; }
			set {
				ValueChecker.CheckValue(value, 1, Int16.MaxValue);
				denominator = value;
			}
		}
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			this.numerator = reader.ReadInt16();
			this.denominator = reader.ReadInt16();
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			int scale = Numerator * 100 / Denominator;
			ModelWorksheetView view = contentBuilder.CurrentSheet.ActiveView;
			view.BeginUpdate();
			try {
				view.ZoomScale = scale;
				if(view.ViewType == SheetViewType.PageLayout)
					view.ZoomScalePageLayoutView = scale;
				else if(view.ViewType == SheetViewType.PageBreakPreview)
					view.ZoomScaleSheetLayoutView = scale;
				else
					view.ZoomScaleNormal = scale;
			}
			finally {
				view.EndUpdate();
			}
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((short)this.numerator);
			writer.Write((short)this.denominator);
		}
		protected override short GetSize() {
			return 4;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandPane
	public class XlsCommandPane : XlsCommandContentBase {
		XlsContentPane content = new XlsContentPane();
		#region Properties
		public int XPos { get { return content.XPos; } set { content.XPos = value; } }
		public int YPos { get { return content.YPos; } set { content.YPos = value; } }
		public int TopRow { get { return content.TopRow; } set { content.TopRow = value; } }
		public int LeftColumn { get { return content.LeftColumn; } set { content.LeftColumn = value; } }
		public ViewPaneType ActivePane {
			get { return ViewPaneTypeHelper.CodeToPaneType(content.ActivePane); }
			set { content.ActivePane = ViewPaneTypeHelper.PaneTypeToCode(value); }
		}
		#endregion
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			DocumentModel documentModel = contentBuilder.DocumentModel;
			ModelWorksheetView view = contentBuilder.CurrentSheet.ActiveView;
			view.BeginUpdate();
			try {
				view.ActivePaneType = ActivePane;
				view.SplitTopLeftCell = new CellPosition(LeftColumn, TopRow);
				if(view.SplitState == ViewSplitState.Split) {
					view.HorizontalSplitPosition = documentModel.UnitConverter.TwipsToModelUnits(XPos);
					view.VerticalSplitPosition = documentModel.UnitConverter.TwipsToModelUnits(YPos);
				}
				else {
					view.HorizontalSplitPosition = XPos;
					view.VerticalSplitPosition = YPos;
				}
			}
			finally {
				view.EndUpdate();
			}
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
		protected override IXlsContent GetContent() {
			return content;
		}
	}
	#endregion
}
