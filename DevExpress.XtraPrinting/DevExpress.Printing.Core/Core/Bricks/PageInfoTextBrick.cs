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

using DevExpress.XtraPrinting.Native;
using System;
using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
#if SL
using System.Windows.Media;
#else
using DevExpress.XtraPrinting.BrickExporters;
using DevExpress.DocumentView;
using System.Collections.Generic;
#endif
namespace DevExpress.XtraPrinting {
	public static class PageInfoExtensions {
		public static string GetDefaultStringFormat(this PageInfo pageInfo) {
			switch(pageInfo) {
				case PageInfo.DateTime:
					return "{0:D}";
				case PageInfo.NumberOfTotal:
					return "{0}/{1}";
				case PageInfo.None:
					return string.Empty;
				default:
					return "{0}";
			}
		}
		public static string GetText(this PageInfo pageInfo, int pageCount, int pageNumber, DateTime dateTime, string format, object textValue) {
			string result = string.Empty;
			try {
				if(!string.IsNullOrEmpty(format))
					result = pageInfo.GetTextCore(pageCount, pageNumber, dateTime, format, textValue);
			} catch { }
			return !string.IsNullOrEmpty(result) ? result :
				pageInfo.GetTextCore(pageCount, pageNumber, dateTime, pageInfo.GetDefaultStringFormat(), textValue);
		}
		static string GetTextCore(this PageInfo pageInfo, int pageCount, int pageNumber, DateTime dateTime, string format, object textValue) {
			switch(pageInfo) {
				case PageInfo.UserName:
					return String.Format(format, PrintingSystemBase.UserName, textValue);
				case PageInfo.DateTime:
					return String.Format(format, dateTime, textValue);
				case PageInfo.Number:
					return String.Format(format, pageNumber, textValue);
				case PageInfo.Total:
					return String.Format(format, pageCount, textValue);
				case PageInfo.NumberOfTotal:
					return String.Format(format, pageNumber, pageCount, textValue);
				case PageInfo.RomHiNumber:
					string hiRomNumber = PSConvert.ToRomanString(pageNumber);
					return String.Format(format, hiRomNumber, textValue);
				case PageInfo.RomLowNumber:
					string lowRomNumber = PSConvert.ToRomanString(pageNumber).ToLower();
					return String.Format(format, lowRomNumber, textValue);
				case PageInfo.None:
					return format;
			}
			return string.Empty;
		}
	}
#if !SL
	[BrickExporter(typeof(PageInfoTextBrickBaseExporter))]
#endif
	public abstract class PageInfoTextBrickBase : TextBrick {
		int startPageNumber = 1;
		string format = string.Empty;
		PageInfo pageInfo = PageInfo.None;
		protected readonly DateTime dateTime = DateTimeHelper.Now;
		public PageInfoTextBrickBase()
			: base() {
			this.XlsExportNativeFormat = DefaultBoolean.False;
		}
		public PageInfoTextBrickBase(IBrickOwner brickOwner)
			: base(brickOwner) {
			this.XlsExportNativeFormat = DefaultBoolean.False;
		}
		public PageInfoTextBrickBase(BorderSide sides, float borderWidth, Color borderColor, Color backColor, Color foreColor)
			: base(sides, borderWidth, borderColor, backColor, foreColor) {
			this.XlsExportNativeFormat = DefaultBoolean.False;
		}
		#region properties
		string ActualTextValue { get { return string.IsNullOrEmpty(TextValue as string) ? string.Empty : (string)TextValue; } }
		protected int StartPageIndex { get { return Math.Max(0, startPageNumber - 1); } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PageInfoTextBrickFormat"),
#endif
 DefaultValue("")]
		public virtual string Format { get { return format; } set { format = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PageInfoTextBrickPageInfo"),
#endif
 DefaultValue(PageInfo.None)]
		public virtual PageInfo PageInfo { get { return pageInfo; } set { pageInfo = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PageInfoTextBrickStartPageNumber"),
#endif
 DefaultValue(1)]
		public virtual int StartPageNumber {
			get { return startPageNumber; }
			set {
				if(value < 0)
					throw (new ArgumentOutOfRangeException("StartPageNumber"));
				startPageNumber = value;
			}
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PageInfoTextBrickBaseXlsExportNativeFormat"),
#endif
		XtraSerializableProperty,
		DefaultValue(DefaultBoolean.False),
		]
		public new DevExpress.Utils.DefaultBoolean XlsExportNativeFormat {
			get { return base.XlsExportNativeFormat; }
			set { base.XlsExportNativeFormat = value; }
		}
		#endregion
		protected string GetTextInfo(int pageCount, int pageNumber) {
			return PageInfo.GetText(pageCount, pageNumber, dateTime, Format, ActualTextValue);
		}
		internal abstract string GetTextInfo(PrintingSystemBase ps, IPageItem drawingPage);
		protected virtual bool AutoWidthCore {
			get { return false; }
		}
		protected internal override void PerformLayout(IPrintingSystemContext context) {
			Text = GetTextInfo(context.PrintingSystem, context.DrawingPage);
			if(AutoWidthCore && IsInitialized) {
				SizeF size = context.Measurer.MeasureString(Text, Font, GraphicsUnit.Document);
				Width = Padding.IsEmpty ? size.Width + GraphicsUnitConverter.PixelToDoc(1) * GetScaleFactor(context) : 
					Padding.InflateWidth(size.Width, GraphicsDpi.Document);
			}
			base.PerformLayout(context);
		}
		protected internal override bool AfterPrintOnPage(IList<int> indices, int pageIndex, int pageCount, Action<BrickBase> callback) {
			if(pageIndex <= PrintingSystem.Pages.Count)
				Text = GetTextInfo(PrintingSystem, PrintingSystem.Pages[pageIndex]);
			return base.AfterPrintOnPage(indices, pageIndex, pageCount, callback);
		}
	}
	public class PageInfoTextBrick : PageInfoTextBrickBase {
		#region static
		internal static int GetPageCountFromPS(PrintingSystemBase ps, int basePageNumber, DefaultBoolean continuousPageNumbering, IPageItem drawingPage) {
			int pageNumber = drawingPage != null ?
				(VisualBrick.ToBoolean(continuousPageNumbering, ps.ContinuousPageNumbering) ? drawingPage.PageCount : drawingPage.OriginalPageCount) :
				1;
			return ps != null ? pageNumber + basePageNumber : 1000;
		}
		#endregion
		DefaultBoolean continuousPageNumbering = DefaultBoolean.Default;
		public PageInfoTextBrick()
			: base() {
		}
		public PageInfoTextBrick(IBrickOwner brickOwner)
			: base(brickOwner) {
		}
		public PageInfoTextBrick(BorderSide sides, float borderWidth, Color borderColor, Color backColor, Color foreColor)
			: base(sides, borderWidth, borderColor, backColor, foreColor) {
		}
		#region properties
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PageInfoTextBrickContinuousPageNumbering"),
#endif
		XtraSerializableProperty,
		DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean ContinuousPageNumbering { get { return continuousPageNumbering; } set { continuousPageNumbering = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PageInfoTextBrickText"),
#endif
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		public override string Text { get { return base.Text; } set { base.Text = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PageInfoTextBrickFormat"),
#endif
XtraSerializableProperty]
		public override string Format { get { return base.Format; } set { base.Format = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PageInfoTextBrickPageInfo"),
#endif
XtraSerializableProperty]
		public override PageInfo PageInfo { get { return base.PageInfo; } set { base.PageInfo = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PageInfoTextBrickStartPageNumber"),
#endif
XtraSerializableProperty]
		public override int StartPageNumber {
			get { return base.StartPageNumber; }
			set { base.StartPageNumber = value; }
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("PageInfoTextBrickBrickType")]
#endif
		public override string BrickType { get { return BrickTypes.PageInfoText; } }
		#endregion
		int GetPageNumber(bool psContinuousPageNumbering, IPageItem drawingPage) {
			int drawingPageIndex = 0;
			if(drawingPage != null)
				drawingPageIndex = VisualBrick.ToBoolean(continuousPageNumbering, psContinuousPageNumbering) ? drawingPage.Index : drawingPage.OriginalIndex;
			return drawingPageIndex + StartPageNumber;
		}
		internal override string GetTextInfo(PrintingSystemBase ps, IPageItem drawingPage) {
			int pageCount = GetPageCountFromPS(ps, StartPageIndex, continuousPageNumbering, drawingPage);
			int pageNumber = GetPageNumber(ps.ContinuousPageNumbering, drawingPage);
			return GetTextInfo(pageCount, pageNumber);
		}
		protected internal override bool AfterPrintOnPage(IList<int> indices, int pageIndex, int pageCount, Action<BrickBase> callback) {
			bool result = base.AfterPrintOnPage(indices, pageIndex, pageCount, callback);
			PrintingSystem.Pages[pageIndex].PageNumberFormat = GetPageNumberFormat();
			PrintingSystem.Pages[pageIndex].PageNumberOffset = StartPageNumber;
			PrintingSystem.Pages[pageIndex].PageInfo = PageInfo;
			return result;
		}
		string GetPageNumberFormat() {
			var pageNumberFormatRegex = new System.Text.RegularExpressions.Regex("{0[^}]*}"); 
			string pageInfoFormat = !string.IsNullOrWhiteSpace(Format) ? Format : PageInfo.GetDefaultStringFormat();
			var pageNumberFormatMatch = pageNumberFormatRegex.Match(pageInfoFormat);
			return (pageNumberFormatMatch != null) ? pageNumberFormatMatch.Value : Page.DefaultPageNumberFormat;
		}
	}
}
