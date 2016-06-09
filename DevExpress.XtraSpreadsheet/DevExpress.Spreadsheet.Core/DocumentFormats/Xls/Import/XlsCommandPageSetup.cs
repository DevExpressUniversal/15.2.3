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
using System.Reflection;
using System.Text;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using System.Drawing;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraExport.Xls;
using DevExpress.Compatibility.System.Drawing.Printing;
#if !SL
using System.Drawing.Printing;
#else
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
#endif
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsCommandPageHeader
	public class XlsCommandPageHeader : XlsCommandStringValueBase {
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			UpdateHeaderFooter(contentBuilder.CurrentSheet.Properties.HeaderFooter);
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder.CurrentChart != null)
				UpdateHeaderFooter(contentBuilder.CurrentChart.PrintSettings.HeaderFooter);
		}
		protected override int GetMaxLength() {
			return XlsDefs.MaxHeaderFooterLength;
		}
		void UpdateHeaderFooter(HeaderFooterOptions headerFooter) {
			headerFooter.BeginUpdate();
			try {
				headerFooter.OddHeader = Value;
			}
			finally {
				headerFooter.EndUpdate();
			}
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandPageFooter
	public class XlsCommandPageFooter : XlsCommandStringValueBase {
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			UpdateHeaderFooter(contentBuilder.CurrentSheet.Properties.HeaderFooter);
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			if(contentBuilder.CurrentChart != null)
				UpdateHeaderFooter(contentBuilder.CurrentChart.PrintSettings.HeaderFooter);
		}
		protected override int GetMaxLength() {
			return XlsDefs.MaxHeaderFooterLength;
		}
		void UpdateHeaderFooter(HeaderFooterOptions headerFooter) {
			headerFooter.BeginUpdate();
			try {
				headerFooter.OddFooter = Value;
			}
			finally {
				headerFooter.EndUpdate();
			}
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandPageHVCenter
	public class XlsCommandPageHCenter : XlsCommandBoolPropertyBase {
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			contentBuilder.CurrentSheet.PrintSetup.CenterHorizontally = Value;
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder.CurrentChart != null)
				contentBuilder.CurrentChart.PrintSettings.PageSetup.CenterHorizontally = Value;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	public class XlsCommandPageVCenter : XlsCommandBoolPropertyBase {
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			contentBuilder.CurrentSheet.PrintSetup.CenterVertically = Value;
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder.CurrentChart != null)
				contentBuilder.CurrentChart.PrintSettings.PageSetup.CenterVertically = Value;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsPageMarginsHelper
	public static class XlsPageMarginsHelper {
		public static int GetMargin(XlsContentBuilder contentBuilder, double margin, double defaultMargin) {
			if (margin < XlsDefs.MinMarginInInches || margin >= XlsDefs.MaxMarginInInches)
				margin = defaultMargin;
			return (int)Math.Round(contentBuilder.DocumentModel.UnitConverter.InchesToModelUnitsF((float)margin));
		}
	}
	#endregion
	#region XlsCommandPageMargins
	public class XlsCommandPageLeftMargin : XlsCommandDoublePropertyValueBase {
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			contentBuilder.CurrentSheet.Margins.Left = XlsPageMarginsHelper.GetMargin(contentBuilder, Value, XlsDefs.DefaultLeftRightMargin);
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			if(contentBuilder.CurrentChart != null)
				contentBuilder.CurrentChart.PrintSettings.PageMargins.Left = XlsPageMarginsHelper.GetMargin(contentBuilder, Value, XlsDefs.DefaultLeftRightMargin);
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	public class XlsCommandPageRightMargin : XlsCommandDoublePropertyValueBase {
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			contentBuilder.CurrentSheet.Margins.Right = XlsPageMarginsHelper.GetMargin(contentBuilder, Value, XlsDefs.DefaultLeftRightMargin);
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder.CurrentChart != null)
				contentBuilder.CurrentChart.PrintSettings.PageMargins.Right = XlsPageMarginsHelper.GetMargin(contentBuilder, Value, XlsDefs.DefaultLeftRightMargin);
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	public class XlsCommandPageTopMargin : XlsCommandDoublePropertyValueBase {
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			contentBuilder.CurrentSheet.Margins.Top = XlsPageMarginsHelper.GetMargin(contentBuilder, Value, XlsDefs.DefaultTopBottomMargin);
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			if(contentBuilder.CurrentChart != null)
				contentBuilder.CurrentChart.PrintSettings.PageMargins.Top = XlsPageMarginsHelper.GetMargin(contentBuilder, Value, XlsDefs.DefaultTopBottomMargin);
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	public class XlsCommandPageBottomMargin : XlsCommandDoublePropertyValueBase {
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			contentBuilder.CurrentSheet.Margins.Bottom = XlsPageMarginsHelper.GetMargin(contentBuilder, Value, XlsDefs.DefaultTopBottomMargin);
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder.CurrentChart != null)
				contentBuilder.CurrentChart.PrintSettings.PageMargins.Bottom = XlsPageMarginsHelper.GetMargin(contentBuilder, Value, XlsDefs.DefaultTopBottomMargin);
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandPageSetup
	public class XlsCommandPageSetup : XlsCommandBase {
		#region Fields
		const short dataSize = 34;
		float headerMargin;
		float footerMargin;
		PrintSetupInfo properties;
		#endregion
		public XlsCommandPageSetup() {
			this.properties = new PrintSetupInfo();
		}
		#region Properties
		public float HeaderMargin { get { return this.headerMargin; } set { this.headerMargin = value; } }
		public float FooterMargin { get { return this.footerMargin; } set { this.footerMargin = value; } }
		public PrintSetupInfo Properties { get { return this.properties; } }
		#endregion
		PaperKind ConvertToPaperKind(int value) {
			if (value < 0 || value > (int)PaperKind.PrcEnvelopeNumber10Rotated)
				return PaperKind.Custom;
			else
				return (PaperKind)value;
		}
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			Properties.PaperKind = ConvertToPaperKind(reader.ReadInt16());
			Properties.Scale = reader.ReadInt16();
			Properties.FirstPageNumber = reader.ReadInt16();
			Properties.FitToWidth = reader.ReadInt16();
			Properties.FitToHeight = reader.ReadInt16();
			short bitwiseField = reader.ReadInt16();
			bool NoPls = Convert.ToBoolean(bitwiseField & 0x04);
			Properties.PagePrintOrder = (PagePrintOrder)(bitwiseField & 0x01);
			if(Convert.ToBoolean(bitwiseField & 0x02))
				Properties.Orientation = ModelPageOrientation.Portrait;
			else
				Properties.Orientation = ModelPageOrientation.Landscape;
			Properties.UsePrinterDefaults = true; 
			Properties.BlackAndWhite = Convert.ToBoolean(bitwiseField & 0x08);
			Properties.Draft = Convert.ToBoolean(bitwiseField & 0x10);
			bool printCellComments = Convert.ToBoolean(bitwiseField & 0x20);
			if(Convert.ToBoolean(bitwiseField & 0x40))
				Properties.Orientation = ModelPageOrientation.Default;
			Properties.UseFirstPageNumber = Convert.ToBoolean(bitwiseField & 0x80);
			Properties.CommentsPrintMode = (ModelCommentsPrintMode)((bitwiseField & 0x0200) >> 9);
			Properties.ErrorsPrintMode = (ModelErrorsPrintMode)((bitwiseField & 0x0c00) >> 10);
			Properties.HorizontalDpi = reader.ReadInt16();
			Properties.VerticalDpi = reader.ReadInt16();
			double margin = reader.ReadDouble();
			if(margin < XlsDefs.MinMarginInInches || margin >= XlsDefs.MaxMarginInInches)
				margin = XlsDefs.DefaultHeaderFooterMargin;
			this.headerMargin = (float)margin;
			margin = reader.ReadDouble();
			if(margin < XlsDefs.MinMarginInInches || margin >= XlsDefs.MaxMarginInInches)
				margin = XlsDefs.DefaultHeaderFooterMargin;
			this.footerMargin = (float)margin;
			if(!printCellComments)
				Properties.CommentsPrintMode = ModelCommentsPrintMode.None;
			Properties.Copies = reader.ReadInt16();
			if(NoPls) {
				Properties.PaperKind = PaperKind.Letter;
				Properties.Scale = 100;
				Properties.HorizontalDpi = 600;
				Properties.VerticalDpi = 600;
				Properties.Copies = 1;
				Properties.Orientation = ModelPageOrientation.Default;
			}
			if(Properties.HorizontalDpi < 1)
				Properties.HorizontalDpi = 600;
			if(Properties.VerticalDpi == 0)
				Properties.VerticalDpi = Properties.HorizontalDpi;
			if(Properties.VerticalDpi < 1)
				Properties.VerticalDpi = 600;
			if(Properties.Copies < 1)
				Properties.Copies = 1;
			if(!Properties.UseFirstPageNumber)
				Properties.FirstPageNumber = 1;
			if(Properties.FirstPageNumber < 1)
				Properties.FirstPageNumber = 1;
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			WorksheetProperties sheetProperties = contentBuilder.CurrentSheet.Properties;
			ApplyPageSetup(sheetProperties.PrintSetup);
			ApplyHeaderFooterMargins(sheetProperties.Margins);
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder.CurrentChart == null)
				return;
			PrintSettings printSettings = contentBuilder.CurrentChart.PrintSettings;
			ApplyPageSetup(printSettings.PageSetup);
			ApplyHeaderFooterMargins(printSettings.PageMargins);
		}
		void ApplyPageSetup(PrintSetup pageSetup) {
			pageSetup.BeginUpdate();
			try {
				pageSetup.PaperKind = Properties.PaperKind;
				pageSetup.Scale = Properties.Scale;
				pageSetup.FirstPageNumber = Properties.FirstPageNumber;
				pageSetup.FitToWidth = Properties.FitToWidth;
				pageSetup.FitToHeight = Properties.FitToHeight;
				pageSetup.PagePrintOrder = Properties.PagePrintOrder;
				pageSetup.Orientation = Properties.Orientation;
				pageSetup.UsePrinterDefaults = Properties.UsePrinterDefaults;
				pageSetup.BlackAndWhite = Properties.BlackAndWhite;
				pageSetup.Draft = Properties.Draft;
				pageSetup.CommentsPrintMode = Properties.CommentsPrintMode;
				pageSetup.UseFirstPageNumber = Properties.UseFirstPageNumber;
				pageSetup.ErrorsPrintMode = Properties.ErrorsPrintMode;
				pageSetup.HorizontalDpi = Properties.HorizontalDpi;
				pageSetup.VerticalDpi = Properties.VerticalDpi;
				pageSetup.Copies = Properties.Copies;
			}
			finally {
				pageSetup.EndUpdate();
			}
		}
		void ApplyHeaderFooterMargins(Model.Margins margins) {
			margins.BeginUpdate();
			try {
				DocumentModelUnitConverter unitConverter = margins.DocumentModel.UnitConverter;
				margins.Header = (int)Math.Round(unitConverter.InchesToModelUnitsF(HeaderMargin));
				margins.Footer = (int)Math.Round(unitConverter.InchesToModelUnitsF(FooterMargin));
			}
			finally {
				margins.EndUpdate();
			}
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((short)Properties.PaperKind);
			writer.Write((short)Properties.Scale);
			writer.Write((short)Properties.FirstPageNumber);
			writer.Write((short)Properties.FitToWidth);
			writer.Write((short)Properties.FitToHeight);
			int bitwiseField = (short)Properties.PagePrintOrder;
			if(Properties.Orientation == ModelPageOrientation.Portrait)
				bitwiseField |= 0x02;
			if(Properties.BlackAndWhite)
				bitwiseField |= 0x08;
			if(Properties.Draft)
				bitwiseField |= 0x10;
			if(Properties.CommentsPrintMode != ModelCommentsPrintMode.None)
				bitwiseField |= 0x20;
			if(Properties.Orientation == ModelPageOrientation.Default)
				bitwiseField |= 0x40;
			if(Properties.UseFirstPageNumber)
				bitwiseField |= 0x80;
			bitwiseField |= ((short)Properties.CommentsPrintMode << 9);
			bitwiseField |= ((short)Properties.ErrorsPrintMode << 10);
			writer.Write((short)bitwiseField);
			writer.Write((short)Properties.HorizontalDpi);
			writer.Write((short)Properties.VerticalDpi);
			writer.Write((double)HeaderMargin);
			writer.Write((double)FooterMargin);
			writer.Write((short)Properties.Copies);
		}
		protected override short GetSize() {
			return dataSize;
		}
		public override IXlsCommand GetInstance() {
			return new XlsCommandPageSetup();
		}
	}
	#endregion
	#region XlsCommandHeaderFooter
	public class XlsCommandHeaderFooter : XlsCommandContentBase {
		XlsContentHeaderFooter content = new XlsContentHeaderFooter();
		#region Properties
		public Guid ViewId { get { return content.ViewId; } set { content.ViewId = value; } }
		public bool AlignWithMargins { get { return content.AlignWithMargins; } set { content.AlignWithMargins = value; } }
		public bool DifferentFirst { get { return content.DifferentFirst; } set { content.DifferentFirst = value; } }
		public bool DifferentOddEven { get { return content.DifferentOddEven; } set { content.DifferentOddEven = value; } }
		public bool ScaleWithDoc { get { return content.ScaleWithDoc; } set { content.ScaleWithDoc = value; } }
		public string EvenHeader { get { return content.EvenHeader; } set { content.EvenHeader = value; } }
		public string EvenFooter { get { return content.EvenFooter; } set { content.EvenFooter = value; } }
		public string FirstHeader { get { return content.FirstHeader; } set { content.FirstHeader = value; } }
		public string FirstFooter { get { return content.FirstFooter; } set { content.FirstFooter = value; } }
		#endregion
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			if(ViewId != Guid.Empty)
				return;
			UpdateHeaderFooter(contentBuilder.CurrentSheet.Properties.HeaderFooter);
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			if (ViewId != Guid.Empty || contentBuilder.CurrentChart == null)
				return;
			UpdateHeaderFooter(contentBuilder.CurrentChart.PrintSettings.HeaderFooter);
		}
		protected override void WriteCore(BinaryWriter writer) {
			content.RecordHeader.RecordTypeId = XlsCommandFactory.GetTypeCodeByType(GetType());
			base.WriteCore(writer);
		}
		void UpdateHeaderFooter(HeaderFooterOptions headerFooter) {
			headerFooter.BeginUpdate();
			try {
				headerFooter.AlignWithMargins = AlignWithMargins;
				headerFooter.DifferentFirst = DifferentFirst;
				headerFooter.DifferentOddEven = DifferentOddEven;
				headerFooter.ScaleWithDoc = ScaleWithDoc;
				headerFooter.EvenHeader = EvenHeader;
				headerFooter.EvenFooter = EvenFooter;
				headerFooter.FirstHeader = FirstHeader;
				headerFooter.FirstFooter = FirstFooter;
			}
			finally {
				headerFooter.EndUpdate();
			}
		}
		public override IXlsCommand GetInstance() {
			content = new XlsContentHeaderFooter();
			return this;
		}
		protected override IXlsContent GetContent() {
			return content;
		}
	}
	#endregion
}
