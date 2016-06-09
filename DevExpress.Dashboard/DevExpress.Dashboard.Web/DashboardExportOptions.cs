#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Globalization;
using System.Web.UI;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardWeb.Native;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Web.Internal;
using DevExpress.XtraPrinting;
namespace DevExpress.DashboardWeb {
	public enum DashboardExportPageLayout {
		Portrait,
		Landscape
	}
	public enum DashboardExportScaleMode {
		None = 0,
		UseScaleFactor = 1,
		AutoFitToPageWidth = 2,
		AutoFitWithinOnePage = 3
	}
	public enum DashboardExportImageFormat {
		Png = 0,
		Gif = 1,
		Jpg = 2
	}
	public enum DashboardExportExcelFormat {
		Csv = 0,
		Xls = 1,
		Xlsx = 2
	}
	public enum ChartExportSizeMode {
		None = 0,
		Stretch = 1,
		Zoom = 2
	}
	public enum MapExportSizeMode {
		None = 0,
		Zoom = 2
	}
	public enum RangeFilterExportSizeMode {
		None = 0,
		Stretch = 1,
		Zoom = 2
	}
	public enum DashboardExportFilterState {
		None = 0,
		Below = 1,
		SeparatePage = 2
	}
	public class ImageFormatExportOptions : OptionsBase {
		const DashboardExportImageFormat DefaultFormat = DashboardExportImageFormat.Png;
		const int DefaultResolution = 96;
		public ImageFormatExportOptions() {
			Format = DefaultFormat;
			Resolution = DefaultResolution;
		}
		[DefaultValue(DefaultFormat), NotifyParentProperty(true)]
		public DashboardExportImageFormat Format { get; set; }
		[DefaultValue(DefaultResolution), NotifyParentProperty(true)]
		public int Resolution { get; set; }
		protected override void PrepareJSonPropertyInfo(List<JSONPropertyInfo> jSonPropertyInfo) {
			jSonPropertyInfo.Add(new JSONPropertyInfo("format", () => Format));
			jSonPropertyInfo.Add(new JSONPropertyInfo("resolution", () => Resolution));
		}
		public override void Assign(OptionsBase options) {
			ImageFormatExportOptions imageFormatItemOptions = options as ImageFormatExportOptions;
			if(imageFormatItemOptions != null) {
				Format = imageFormatItemOptions.Format;
				Resolution = imageFormatItemOptions.Resolution;
			}
		}
	}
	public class ExcelFormatExportOptions : OptionsBase {
		const DashboardExportExcelFormat DefaultFormat = DashboardExportExcelFormat.Xlsx;
		static string DefaultCsvValueSeparator = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
		public ExcelFormatExportOptions() {
			Format = DefaultFormat;
			CsvValueSeparator = DefaultCsvValueSeparator;
		}
		[DefaultValue(DefaultFormat), NotifyParentProperty(true)]
		public DashboardExportExcelFormat Format { get; set; }
		[NotifyParentProperty(true)]
		public string CsvValueSeparator { get; set; }
		protected override void PrepareJSonPropertyInfo(List<JSONPropertyInfo> jSonPropertyInfo) {
			jSonPropertyInfo.Add(new JSONPropertyInfo("format", () => Format));
			jSonPropertyInfo.Add(new JSONPropertyInfo("csvValueSeparator", () => CsvValueSeparator));
		}
		public override void Assign(OptionsBase options) {
			ExcelFormatExportOptions formatItemOptions = options as ExcelFormatExportOptions;
			if(formatItemOptions != null) {
				Format = formatItemOptions.Format;
				CsvValueSeparator = formatItemOptions.CsvValueSeparator;
			}
		}
		void ResetCsvValueSeparator() { CsvValueSeparator = DefaultCsvValueSeparator; }
		bool ShouldSerializeCsvValueSeparator() { return CsvValueSeparator != DefaultCsvValueSeparator; }
	}
	public class DocumentContentExportOptions : OptionsBase {
		internal const DefaultBoolean DefaultShowTitle = DefaultBoolean.Default;
		const DashboardExportFilterState DefaultFilterState = DashboardExportFilterState.None;
		public DocumentContentExportOptions() {
			ShowTitle = DefaultShowTitle;
			FilterState = DefaultFilterState;
		}
		[DefaultValue(DefaultShowTitle), NotifyParentProperty(true)]
		public DefaultBoolean ShowTitle { get; set; }
		[DefaultValue(DefaultFilterState), NotifyParentProperty(true)]
		public DashboardExportFilterState FilterState { get; set; }
		public override void Assign(OptionsBase options) {
			DocumentContentExportOptions opts = options as DocumentContentExportOptions;
			if(opts != null) {
				ShowTitle = opts.ShowTitle;
				FilterState = opts.FilterState;
			}
		}
	}
	public class DashboardItemExportOptions : OptionsBase {
		const DefaultBoolean DefaultIncludeCaption = DefaultBoolean.Default;
		readonly DocumentContentExportOptions options;
		public DashboardItemExportOptions(DocumentContentExportOptions options) {
			this.options = options;
			IncludeCaption = DefaultIncludeCaption;
		}
		[DefaultValue(DefaultIncludeCaption), NotifyParentProperty(true)]
		public DefaultBoolean IncludeCaption { get; set; }
		protected override void PrepareJSonPropertyInfo(List<JSONPropertyInfo> jSonPropertyInfo) {
			jSonPropertyInfo.Add(new JSONPropertyInfo("includeCaption", () => options.ShowTitle != DocumentContentExportOptions.DefaultShowTitle ? (options.ShowTitle != DefaultBoolean.False) : (IncludeCaption != DefaultBoolean.False)));
			jSonPropertyInfo.Add(new JSONPropertyInfo("caption", () => "Default"));
			FilterStatePresentation filterStatePresentation = (FilterStatePresentation)options.FilterState;
			jSonPropertyInfo.Add(new JSONPropertyInfo("filterStatePresentation", () => filterStatePresentation));
		}
		public override void Assign(OptionsBase options) {
			DashboardItemExportOptions dashboardItemOptions = options as DashboardItemExportOptions;
			if(dashboardItemOptions != null) {
				IncludeCaption = dashboardItemOptions.IncludeCaption;
			}
		}
	}
	public class ChartExportOptions : OptionsBase {
		const ChartExportSizeMode DefaultSizeMode = ChartExportSizeMode.Zoom;
		const bool DefaultAutomaticPageLayout = true;
		public ChartExportOptions() {
			SizeMode = DefaultSizeMode;
			AutomaticPageLayout = DefaultAutomaticPageLayout;
		}
		[DefaultValue(DefaultSizeMode), NotifyParentProperty(true)]
		public ChartExportSizeMode SizeMode { get; set; }
		[DefaultValue(DefaultAutomaticPageLayout), NotifyParentProperty(true)]
		public bool AutomaticPageLayout { get; set; }
		protected override void PrepareJSonPropertyInfo(List<JSONPropertyInfo> jSonPropertyInfo) {
			jSonPropertyInfo.Add(new JSONPropertyInfo("sizeMode", () => SizeMode));
			jSonPropertyInfo.Add(new JSONPropertyInfo("automaticPageLayout", () => AutomaticPageLayout));
		}
		public override void Assign(OptionsBase options) {
			ChartExportOptions chartOptions = options as ChartExportOptions;
			if(chartOptions != null) {
				SizeMode = chartOptions.SizeMode;
				AutomaticPageLayout = chartOptions.AutomaticPageLayout;
			}
		}
	}
	public class GridExportOptions : OptionsBase {
		const bool DefaultFitToPageWidth = true;
		const bool DefaultPrintHeadersOnEveryPage = true;
		public GridExportOptions() {
			FitToPageWidth = DefaultFitToPageWidth;
			PrintHeadersOnEveryPage = DefaultPrintHeadersOnEveryPage;
		}
		[DefaultValue(DefaultFitToPageWidth), NotifyParentProperty(true)]
		public bool FitToPageWidth { get; set; }
		[DefaultValue(DefaultPrintHeadersOnEveryPage), NotifyParentProperty(true)]
		public bool PrintHeadersOnEveryPage { get; set; }
		protected override void PrepareJSonPropertyInfo(List<JSONPropertyInfo> jSonPropertyInfo)
		{
			jSonPropertyInfo.Add(new JSONPropertyInfo("fitToPageWidth", () => FitToPageWidth));
			jSonPropertyInfo.Add(new JSONPropertyInfo("printHeadersOnEveryPage", () => PrintHeadersOnEveryPage));
		}
		public override void Assign(OptionsBase options) {
			GridExportOptions gridOptions = options as GridExportOptions;
			if(gridOptions != null) {
				PrintHeadersOnEveryPage = gridOptions.PrintHeadersOnEveryPage;
				FitToPageWidth = gridOptions.FitToPageWidth;
			}
		}
	}
	public class PivotExportOptions : OptionsBase {
		const bool DefaultPrintHeadersOnEveryPage = true;
		public PivotExportOptions() {
			PrintHeadersOnEveryPage = DefaultPrintHeadersOnEveryPage;
		}
		[DefaultValue(DefaultPrintHeadersOnEveryPage), NotifyParentProperty(true)]
		public bool PrintHeadersOnEveryPage { get; set; }
		protected override void PrepareJSonPropertyInfo(List<JSONPropertyInfo> jSonPropertyInfo) {
			jSonPropertyInfo.Add(new JSONPropertyInfo("printHeadersOnEveryPage", () => PrintHeadersOnEveryPage));
		}
		public override void Assign(OptionsBase options) {
			PivotExportOptions pivotOptions = options as PivotExportOptions;
			if(pivotOptions != null) {
				PrintHeadersOnEveryPage = pivotOptions.PrintHeadersOnEveryPage;
			}
		}
	}
	public class PieExportOptions : OptionsBase {
		const bool DefaultAutoArrangeContent = true;
		public PieExportOptions() {
			AutoArrangeContent = DefaultAutoArrangeContent;
		}
		[DefaultValue(DefaultAutoArrangeContent), NotifyParentProperty(true)]
		public bool AutoArrangeContent { get; set; }
		protected override void PrepareJSonPropertyInfo(List<JSONPropertyInfo> jSonPropertyInfo) {
			jSonPropertyInfo.Add(new JSONPropertyInfo("autoArrangeContent", () => AutoArrangeContent));
		}
		public override void Assign(OptionsBase options) {
			PieExportOptions pieOptions = options as PieExportOptions;
			if(pieOptions != null) {
				AutoArrangeContent = pieOptions.AutoArrangeContent;
			}
		}
	}
	public class CardExportOptions : OptionsBase {
		const bool DefaultAutoArrangeContent = true;
		public CardExportOptions() {
			AutoArrangeContent = DefaultAutoArrangeContent;
		}
		[DefaultValue(DefaultAutoArrangeContent), NotifyParentProperty(true)]
		public bool AutoArrangeContent { get; set; }
		protected override void PrepareJSonPropertyInfo(List<JSONPropertyInfo> jSonPropertyInfo) {
			jSonPropertyInfo.Add(new JSONPropertyInfo("autoArrangeContent", () => AutoArrangeContent));
		}
		public override void Assign(OptionsBase options) {
			CardExportOptions cardOptions = options as CardExportOptions;
			if(cardOptions != null) {
				AutoArrangeContent = cardOptions.AutoArrangeContent;
			}
		}
	}
	public class GaugeExportOptions : OptionsBase {
		const bool DefaultAutoArrangeContent = true;
		public GaugeExportOptions() {
			AutoArrangeContent = DefaultAutoArrangeContent;
		}
		[DefaultValue(DefaultAutoArrangeContent), NotifyParentProperty(true)]
		public bool AutoArrangeContent { get; set; }
		public override void Assign(OptionsBase options) {
			GaugeExportOptions gaugeOptions = options as GaugeExportOptions;
			if(gaugeOptions != null) {
				AutoArrangeContent = gaugeOptions.AutoArrangeContent;
			}
		}
		protected override void PrepareJSonPropertyInfo(List<JSONPropertyInfo> jSonPropertyInfo) {
			jSonPropertyInfo.Add(new JSONPropertyInfo("autoArrangeContent", () => AutoArrangeContent));
		}
	}
	public class MapExportOptions : OptionsBase {
		const MapExportSizeMode DefaultSizeMode = MapExportSizeMode.Zoom;
		const bool DefaultAutomaticPageLayout = true;
		public MapExportOptions() {
			SizeMode = DefaultSizeMode;
			AutomaticPageLayout = DefaultAutomaticPageLayout;
		}
		[DefaultValue(DefaultSizeMode), NotifyParentProperty(true)]
		public MapExportSizeMode SizeMode { get; set; }
		[DefaultValue(DefaultAutomaticPageLayout), NotifyParentProperty(true)]
		public bool AutomaticPageLayout { get; set; }
		protected override void PrepareJSonPropertyInfo(List<JSONPropertyInfo> jSonPropertyInfo) {
			jSonPropertyInfo.Add(new JSONPropertyInfo("sizeMode", () => SizeMode));
			jSonPropertyInfo.Add(new JSONPropertyInfo("automaticPageLayout", () => AutomaticPageLayout));
		}
		public override void Assign(OptionsBase options) {
			MapExportOptions mapOptions = options as MapExportOptions;
			if(mapOptions != null) {
				SizeMode = mapOptions.SizeMode;
				AutomaticPageLayout = mapOptions.AutomaticPageLayout;
			}
		}
	}
	public class RangeFilterExportOptions : OptionsBase {
		const RangeFilterExportSizeMode DefaultSizeMode = RangeFilterExportSizeMode.Stretch;
		const bool DefaultAutomaticPageLayout = true;
		public RangeFilterExportOptions() {
			SizeMode = DefaultSizeMode;
			AutomaticPageLayout = DefaultAutomaticPageLayout;
		}
		[DefaultValue(DefaultSizeMode), NotifyParentProperty(true)]
		public RangeFilterExportSizeMode SizeMode { get; set; }
		[DefaultValue(DefaultAutomaticPageLayout), NotifyParentProperty(true)]
		public bool AutomaticPageLayout { get; set; }
		protected override void PrepareJSonPropertyInfo(List<JSONPropertyInfo> jSonPropertyInfo) {
			jSonPropertyInfo.Add(new JSONPropertyInfo("sizeMode", () => SizeMode));
			jSonPropertyInfo.Add(new JSONPropertyInfo("automaticPageLayout", () => AutomaticPageLayout));
		}
		public override void Assign(OptionsBase options) {
			RangeFilterExportOptions rangeFilterOptions = options as RangeFilterExportOptions;
			if(rangeFilterOptions != null) {
				SizeMode = rangeFilterOptions.SizeMode;
				AutomaticPageLayout = rangeFilterOptions.AutomaticPageLayout;
			}
		}
	}
	public class ExportFontInfo {
		[DefaultValue(false), NotifyParentProperty(true)]
		public bool UseCustomFontInfo { get; set; }
		[DefaultValue(null), NotifyParentProperty(true)]
		public string Name { get; set; }
		[DefaultValue(null), NotifyParentProperty(true)]
		public byte? GdiCharSet { get; set; }
		public void Assign(ExportFontInfo fontInfo) {
			UseCustomFontInfo = fontInfo.UseCustomFontInfo;
			Name = fontInfo.Name;
			if(fontInfo.GdiCharSet.HasValue)
				GdiCharSet = fontInfo.GdiCharSet.Value;
		}
		internal DashboardFontInfo GetDashboardFontInfo() {
			DashboardFontInfo fontInfo = new DashboardFontInfo();
			if(UseCustomFontInfo) {
				if(Name != null)
					fontInfo.Name = Name;
				if(GdiCharSet.HasValue)
					fontInfo.GdiCharSet = GdiCharSet.Value;
			}
			return fontInfo;
		}
	}
	public abstract class OptionsBase : IJSONCustomObject {
		List<JSONPropertyInfo> jSonPropertyInfo;
		List<JSONPropertyInfo> JSonPropertyInfo {
			get {
				if(jSonPropertyInfo == null) {
					jSonPropertyInfo = new List<JSONPropertyInfo>();
					PrepareJSonPropertyInfo(jSonPropertyInfo);
				}
				return jSonPropertyInfo;
			}
		}
		public override string ToString() {
			return OptionsHelper.GetObjectText(this);
		}
		public abstract void Assign(OptionsBase options);
		protected virtual void PrepareJSonPropertyInfo(List<JSONPropertyInfo> jSonPropertyInfo) { }
		int IJSONCustomObject.PropertiesCount { get { return JSonPropertyInfo.Count; } }
		string IJSONCustomObject.GetPropertyName(int index) {
			return JSonPropertyInfo[index].Name;
		}
		object IJSONCustomObject.GetPropertyValue(int index) {
			return JSonPropertyInfo[index].Getter();
		}
	}
	public class DashboardExportOptions : OptionsBase {
		internal static DashboardReportOptions Parse(string itemType, Hashtable documentOptionsHash) {
			Hashtable imageFormatOptionsHash = (Hashtable)documentOptionsHash["imageFormatOptions"];
			Hashtable excelFormatOptionsHash = (Hashtable)documentOptionsHash["excelFormatOptions"];
			Hashtable commonOptionsHash = (Hashtable)documentOptionsHash["commonOptions"];
			Hashtable chartOptionsHash = (Hashtable)documentOptionsHash["chartOptions"];
			Hashtable gridOptionsHash = (Hashtable)documentOptionsHash["gridOptions"];
			Hashtable mapOptionsHash = (Hashtable)documentOptionsHash["mapOptions"];
			Hashtable rangeFilterOptionsHash = (Hashtable)documentOptionsHash["rangeFilterOptions"];
			DashboardExportScaleMode scaleMode = (DashboardExportScaleMode)Enum.Parse(typeof(DashboardExportScaleMode), (string)documentOptionsHash["scaleMode"], true);
			float scaleFactor = (float)Convert.ToDouble(documentOptionsHash["scaleFactor"]);
			int autoFitPageCount = Convert.ToInt32(documentOptionsHash["autoFitPageCount"]);
			DashboardReportOptions opts = new DashboardReportOptions {
				PageOptions = new PageOptions {
					PaperOptions = new PaperOptions {
						PaperKind = (PaperKind)Enum.Parse(typeof(PaperKind), (string)documentOptionsHash["paperKind"], true),
						Landscape = (string)documentOptionsHash["pageLayout"] == "Landscape",
						UseCustomMargins = false
					},
					ScalingOptions = new ScalingOptions()
				},
				FormatOptions = new FormatOptions {
					ImageOptions = new ImageExportOptions {
						Format = GetImageFormat(imageFormatOptionsHash["format"].ToString()),
						Resolution = Convert.ToInt32(imageFormatOptionsHash["resolution"])
					},
					PdfOptions = new PdfExportOptions(),
					ExcelOptions = new ExcelExportOptions() {
						Format = GetExcelFormat(excelFormatOptionsHash["format"].ToString()),
						CsvValueSeparator = excelFormatOptionsHash["csvValueSeparator"].ToString()
					}
				}
			};
			AutomaticPageOptions autoPageOpts = new AutomaticPageOptions();
			switch(scaleMode) {
				case DashboardExportScaleMode.None:
					scaleFactor = 1.0f;
					autoFitPageCount = 0;
					break;
				case DashboardExportScaleMode.UseScaleFactor:
					autoFitPageCount = 0;
					break;
				case DashboardExportScaleMode.AutoFitToPageWidth:
					scaleFactor = 1.0f;
					break;
				case DashboardExportScaleMode.AutoFitWithinOnePage:
					scaleFactor = 1.0f;
					autoFitPageCount = 0;
					if(String.IsNullOrEmpty(itemType) || itemType == "GROUP") {
						autoPageOpts.AutoFitToPageSize = true;
						autoPageOpts.AutoRotate = true;
					}
					break;
			}
			opts.PageOptions.ScalingOptions.ScaleFactor = scaleFactor;
			opts.PageOptions.ScalingOptions.AutoFitPageCount = autoFitPageCount;
			ItemContentOptions itemOpts = new ItemContentOptions {
					ArrangerOptions = new ArrangerOptions(),
					HeadersOptions = new HeadersOptions(),
					SizeMode = ItemSizeMode.None
			};
			DocumentContentOptions documentContentOpts = new DocumentContentOptions();
			switch(itemType) {
				case "CHART":
					itemOpts.SizeMode = (ItemSizeMode)Enum.Parse(typeof(ItemSizeMode), (string)(chartOptionsHash["sizeMode"]));
					autoPageOpts.AutoRotate = Convert.ToBoolean(chartOptionsHash["automaticPageLayout"]);
					break;
				case "GRID":
					itemOpts.SizeMode = Convert.ToBoolean(gridOptionsHash["fitToPageWidth"]) ?  ItemSizeMode.FitWidth : ItemSizeMode.None;
					itemOpts.HeadersOptions.PrintHeadersOnEveryPage = Convert.ToBoolean(gridOptionsHash["printHeadersOnEveryPage"]);
					break;
				case "PIVOT":
					itemOpts.HeadersOptions.PrintHeadersOnEveryPage = Convert.ToBoolean(((Hashtable)documentOptionsHash["pivotOptions"])["printHeadersOnEveryPage"]);
					break;
				case "CARD":
					itemOpts.ArrangerOptions.AutoArrangeContent = Convert.ToBoolean(((Hashtable)documentOptionsHash["cardOptions"])["autoArrangeContent"]);
					break;
				case "GAUGE":
					itemOpts.ArrangerOptions.AutoArrangeContent = Convert.ToBoolean(((Hashtable)documentOptionsHash["gaugeOptions"])["autoArrangeContent"]);
					break;
				case "PIE":
					itemOpts.ArrangerOptions.AutoArrangeContent = Convert.ToBoolean(((Hashtable)documentOptionsHash["pieOptions"])["autoArrangeContent"]);
					break;
				case "CHOROPLETHMAP":
				case "GEOPOINTMAP":
				case "BUBBLEMAP":
				case "PIEMAP":
					itemOpts.SizeMode = (ItemSizeMode)Enum.Parse(typeof(ItemSizeMode), (string)(mapOptionsHash["sizeMode"]));
					autoPageOpts.AutoRotate = Convert.ToBoolean(mapOptionsHash["automaticPageLayout"]);
					break;
				case "RANGEFILTER":
					itemOpts.SizeMode = (ItemSizeMode)Enum.Parse(typeof(ItemSizeMode), (string)(rangeFilterOptionsHash["sizeMode"]));
					autoPageOpts.AutoRotate = Convert.ToBoolean(rangeFilterOptionsHash["automaticPageLayout"]);
					break;
			}
			if(!String.IsNullOrEmpty(itemType)) {
				documentContentOpts.ShowTitle = Convert.ToBoolean(commonOptionsHash["includeCaption"]);
				documentContentOpts.Title = commonOptionsHash["caption"].ToString();
				documentContentOpts.FilterStatePresentation = (FilterStatePresentation)Enum.Parse(typeof(FilterStatePresentation), (string)(commonOptionsHash["filterStatePresentation"]));
			}
			else {
				documentContentOpts.ShowTitle = Convert.ToBoolean(documentOptionsHash["showTitle"]);
				documentContentOpts.Title = documentOptionsHash["title"].ToString();
				documentContentOpts.FilterStatePresentation = (FilterStatePresentation)Enum.Parse(typeof(FilterStatePresentation), (string)(commonOptionsHash["filterStatePresentation"]));
			}
			opts.DocumentContentOptions = documentContentOpts;
			opts.AutoPageOptions = autoPageOpts;
			opts.ItemContentOptions = itemOpts;
			return opts;
		}
		static ImageFormat GetImageFormat(string stringValue) {
			switch(stringValue) {
				case "Png":
					return ImageFormat.Png;
				case "Jpg":
					return ImageFormat.Jpeg;
				case "Gif":
					return ImageFormat.Gif;
				default:
					throw new NotSupportedException();
			}
		}
		static ExcelFormat GetExcelFormat(string stringValue) {
			switch(stringValue) {
				case "Csv":
					return ExcelFormat.Csv;
				case "Xls":
					return ExcelFormat.Xls;
				case "Xlsx":
					return ExcelFormat.Xlsx;
				default:
					throw new NotSupportedException();
			}
		}
		const PaperKind DefaultPaperKind = PaperKind.Letter;
		const DashboardExportPageLayout DefaultPageLayout = DashboardExportPageLayout.Portrait;
		const DashboardExportScaleMode DefaultScaleMode = DashboardExportScaleMode.AutoFitWithinOnePage;
		const float DefaultScaleFactor = 1.0f;
		const int DefaultAutoFitPageCount = 1;
		readonly ImageFormatExportOptions imageFormatOptions = new ImageFormatExportOptions();
		readonly ExcelFormatExportOptions excelFormatOptions = new ExcelFormatExportOptions();
		readonly DocumentContentExportOptions documentContentOptions = new DocumentContentExportOptions();
		readonly DashboardItemExportOptions dashboardItemOptions;
		readonly ChartExportOptions chartOptions = new ChartExportOptions();
		readonly GridExportOptions gridOptions = new GridExportOptions();
		readonly PivotExportOptions pivotOptions = new PivotExportOptions();
		readonly PieExportOptions pieOptions = new PieExportOptions();
		readonly GaugeExportOptions gaugeOptions = new GaugeExportOptions();
		readonly CardExportOptions cardOptions = new CardExportOptions();
		readonly MapExportOptions mapOptions = new MapExportOptions();
		readonly RangeFilterExportOptions rangeFilterOptions = new RangeFilterExportOptions();
		readonly ExportFontInfo fontInfo = new ExportFontInfo();
		protected override void PrepareJSonPropertyInfo(List<JSONPropertyInfo> jSonPropertyInfo) {
			jSonPropertyInfo.Add(new JSONPropertyInfo("autoFitPageCount", () => AutoFitPageCount));
			jSonPropertyInfo.Add(new JSONPropertyInfo("paperKind", () => PaperKind));
			jSonPropertyInfo.Add(new JSONPropertyInfo("pageLayout", () => PageLayout));
			jSonPropertyInfo.Add(new JSONPropertyInfo("scaleMode", () => ScaleMode));
			jSonPropertyInfo.Add(new JSONPropertyInfo("scaleFactor", () => ScaleFactor));
			jSonPropertyInfo.Add(new JSONPropertyInfo("imageFormatOptions", () => imageFormatOptions));
			jSonPropertyInfo.Add(new JSONPropertyInfo("excelFormatOptions", () => excelFormatOptions));
			jSonPropertyInfo.Add(new JSONPropertyInfo("commonOptions", () => dashboardItemOptions));
			jSonPropertyInfo.Add(new JSONPropertyInfo("chartOptions", () => chartOptions));
			jSonPropertyInfo.Add(new JSONPropertyInfo("gridOptions", () => gridOptions));
			jSonPropertyInfo.Add(new JSONPropertyInfo("pivotOptions", () => pivotOptions));
			jSonPropertyInfo.Add(new JSONPropertyInfo("pieOptions", () => pieOptions));
			jSonPropertyInfo.Add(new JSONPropertyInfo("gaugeOptions", () => gaugeOptions));
			jSonPropertyInfo.Add(new JSONPropertyInfo("cardOptions", () => cardOptions));
			jSonPropertyInfo.Add(new JSONPropertyInfo("mapOptions", () => mapOptions));
			jSonPropertyInfo.Add(new JSONPropertyInfo("rangeFilterOptions", () => rangeFilterOptions));
		}
		[
		DefaultValue(DefaultPaperKind),
		NotifyParentProperty(true)
		]
		public PaperKind PaperKind { get; set; }
		[
		DefaultValue(DefaultPageLayout),
		NotifyParentProperty(true)
		]
		public DashboardExportPageLayout PageLayout { get; set; }
		[
		DefaultValue(DefaultScaleMode),
		NotifyParentProperty(true)
		]
		public DashboardExportScaleMode ScaleMode { get; set; }
		[
		DefaultValue(DefaultScaleFactor),
		NotifyParentProperty(true)
		]
		public float ScaleFactor { get; set; }
		[
		DefaultValue(DefaultAutoFitPageCount),
		NotifyParentProperty(true)
		]
		public int AutoFitPageCount { get; set; }
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty),
		NotifyParentProperty(true)
		]
		public ImageFormatExportOptions ImageFormatOptions {
			get { return imageFormatOptions; }
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty),
		NotifyParentProperty(true)
		]
		public ExcelFormatExportOptions ExcelFormatOptions {
			get { return excelFormatOptions; }
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty),
		NotifyParentProperty(true),
		Obsolete("This property is now obsolete. Use DocumentContentOptions property instead.")
		]
		public DashboardItemExportOptions DashboardItemOptions {
			get { return dashboardItemOptions; }
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty),
		NotifyParentProperty(true)
		]
		public DocumentContentExportOptions DocumentContentOptions {
			get { return documentContentOptions; }
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty),
		NotifyParentProperty(true)
		]
		public ChartExportOptions ChartOptions {
			get { return chartOptions; }
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty),
		NotifyParentProperty(true)
		]
		public GridExportOptions GridOptions {
			get { return gridOptions; }
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty),
		NotifyParentProperty(true)
		]
		public PivotExportOptions PivotOptions {
			get { return pivotOptions; }
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty),
		NotifyParentProperty(true)
		]
		public PieExportOptions PieOptions {
			get { return pieOptions; }
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty),
		NotifyParentProperty(true)
		]
		public GaugeExportOptions GaugeOptions {
			get { return gaugeOptions; }
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty),
		NotifyParentProperty(true)
		]
		public CardExportOptions CardOptions {
			get { return cardOptions; }
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty),
		NotifyParentProperty(true)
		]
		public MapExportOptions MapOptions {
			get { return mapOptions; }
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty),
		NotifyParentProperty(true)
		]
		public RangeFilterExportOptions RangeFilterOptions {
			get { return rangeFilterOptions; }
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty),
		NotifyParentProperty(true)
		]
		public ExportFontInfo FontInfo {
			get { return fontInfo; }
		}
		public override void Assign(OptionsBase options) {
			DashboardExportOptions exportOptions = options as DashboardExportOptions;
			if(exportOptions != null) { 
				PaperKind = exportOptions.PaperKind;
				PageLayout = exportOptions.PageLayout;
				ScaleMode = exportOptions.ScaleMode;
				ScaleFactor = exportOptions.ScaleFactor;
				AutoFitPageCount = exportOptions.AutoFitPageCount;
				imageFormatOptions.Assign(exportOptions.ImageFormatOptions);
				excelFormatOptions.Assign(exportOptions.ExcelFormatOptions);
#pragma warning disable 0612, 0618 // Obsolete
				dashboardItemOptions.Assign(exportOptions.DashboardItemOptions);
#pragma warning restore 0612, 0618
				documentContentOptions.Assign(exportOptions.DocumentContentOptions);
				chartOptions.Assign(exportOptions.ChartOptions);
				gridOptions.Assign(exportOptions.GridOptions);
				pivotOptions.Assign(exportOptions.PivotOptions);
				pieOptions.Assign(exportOptions.PieOptions);
				gaugeOptions.Assign(exportOptions.GaugeOptions);
				cardOptions.Assign(exportOptions.CardOptions);
				mapOptions.Assign(exportOptions.MapOptions);
				rangeFilterOptions.Assign(exportOptions.RangeFilterOptions);
				fontInfo.Assign(exportOptions.FontInfo);
			}
		}
		public DashboardExportOptions() {
			this.dashboardItemOptions = new DashboardItemExportOptions(documentContentOptions);
			PaperKind = DefaultPaperKind;
			PageLayout = DefaultPageLayout;
			ScaleMode = DefaultScaleMode;
			ScaleFactor = DefaultScaleFactor;
			AutoFitPageCount = DefaultAutoFitPageCount;
		}
	}
}
