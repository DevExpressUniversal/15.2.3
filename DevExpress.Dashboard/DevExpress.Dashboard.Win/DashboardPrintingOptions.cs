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

using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardWin.Native.Printing;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using System;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Drawing;
using System.Drawing.Design;
using DevExpress.DashboardExport;
namespace DevExpress.DashboardWin {
	public enum DashboardPrintingPageLayout { 
		Portrait, 
		Landscape 
	}
	public enum DashboardPrintingScaleMode { 
		None = 0, 
		UseScaleFactor = 1, 
		AutoFitToPageWidth = 2,
		AutoFitWithinOnePage = 3
	}
	public enum ChartPrintingSizeMode {
		None = 0,
		Stretch = 1,
		Zoom = 2
	}
	public enum RangeFilterPrintingSizeMode {
		None = 0,
		Stretch = 1,
		Zoom = 2
	}
	public enum MapPrintingSizeMode {
		None = 0,
		Zoom = 2
	}
	public enum DashboardPrintingFilterState {
		None = 0,
		Below = 1,
		SeparatePage = 2
	}
	public class DashboardPrintingOptions : OptionsBase {
		const PaperKind DefaultPaperKind = PaperKind.Letter;
		const DashboardPrintingPageLayout DefaultPageLayout = DashboardPrintingPageLayout.Portrait;
		const DashboardPrintingScaleMode DefaultScaleMode = DashboardPrintingScaleMode.AutoFitWithinOnePage;
		const float DefaultScaleFactor = 1.0f;
		const int DefaultAutoFitPageCount = 1;
		readonly DashboardItemPrintingOptions dashboardItemOptions = new DashboardItemPrintingOptions();
		readonly DocumentContentPrintingOptions documentContentOptions = new DocumentContentPrintingOptions();
		readonly ChartPrintingOptions chartOptions = new ChartPrintingOptions();
		readonly RangeFilterPrintingOptions rangeFilterOptions = new RangeFilterPrintingOptions();
		readonly GridPrintingOptions gridOptions = new GridPrintingOptions();
		readonly PivotPrintingOptions pivotOptions = new PivotPrintingOptions();
		readonly PiePrintingOptions pieOptions = new PiePrintingOptions();
		readonly GaugePrintingOptions gaugeOptions = new GaugePrintingOptions();
		readonly CardPrintingOptions cardOptions = new CardPrintingOptions();
		readonly MapPrintingOptions mapOptions = new MapPrintingOptions();
		readonly PrintingFontInfo fontInfo = new PrintingFontInfo();
		[DefaultValue(DefaultPaperKind), NotifyParentProperty(true)]
		public PaperKind PaperKind { get; set; }
		[DefaultValue(DefaultPageLayout), NotifyParentProperty(true)]
		public DashboardPrintingPageLayout PageLayout { get; set; }
		[DefaultValue(DefaultScaleMode), NotifyParentProperty(true)]
		public DashboardPrintingScaleMode ScaleMode { get; set; }
		[DefaultValue(DefaultScaleFactor), NotifyParentProperty(true)]
		public float ScaleFactor { get; set; }
		[DefaultValue(DefaultAutoFitPageCount), NotifyParentProperty(true)]
		public int AutoFitPageCount { get; set; }
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true),
		Obsolete("This property is now obsolete. Use DocumentContentOptions property instead.")
		]
		public DashboardItemPrintingOptions DashboardItemOptions {
			get { return dashboardItemOptions; }
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true)
		]
		public DocumentContentPrintingOptions DocumentContentOptions {
			get { return documentContentOptions; }
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true)
		]
		public ChartPrintingOptions ChartOptions {
			get { return chartOptions; }
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true)
		]
		public RangeFilterPrintingOptions RangeFilterOptions {
			get { return rangeFilterOptions; }
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true)
		]
		public GridPrintingOptions GridOptions {
			get { return gridOptions; }
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true)
		]
		public PivotPrintingOptions PivotOptions {
			get { return pivotOptions; }
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true)
		]
		public PiePrintingOptions PieOptions {
			get { return pieOptions; }
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true)
		]
		public GaugePrintingOptions GaugeOptions {
			get { return gaugeOptions; }
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true)
		]
		public CardPrintingOptions CardOptions {
			get { return cardOptions; }
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true)
		]
		public MapPrintingOptions MapOptions {
			get { return mapOptions; }
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true)
		]
		public PrintingFontInfo FontInfo { 
			get { return fontInfo; } 
		}
		internal DashboardPrintingOptions() {
			PaperKind = DefaultPaperKind;
			PageLayout = DefaultPageLayout;
			ScaleMode = DefaultScaleMode;
			ScaleFactor = DefaultScaleFactor;
			AutoFitPageCount = DefaultAutoFitPageCount;
		}
		internal ExtendedReportOptions GetOptions(string itemType) {
			ExtendedReportOptions opts = ExtendedReportOptions.Empty;
			opts.PaperOptions.PaperKind = PaperKind;
			opts.PaperOptions.Landscape = PageLayout == DashboardPrintingPageLayout.Landscape;
			ExtendedScalingOptions scalingOpts = new ExtendedScalingOptions();
			switch(ScaleMode) {
				case DashboardPrintingScaleMode.None:
					scalingOpts.ScaleMode = ExtendedScaleMode.None;
					break;
				case DashboardPrintingScaleMode.UseScaleFactor:
					scalingOpts.ScaleMode = ExtendedScaleMode.UseScaleFactor;
					scalingOpts.ScaleFactor = ScaleFactor;
					break;
				case DashboardPrintingScaleMode.AutoFitToPageWidth:
					scalingOpts.ScaleMode = ExtendedScaleMode.AutoFitToPageWidth;
					scalingOpts.AutoFitPageCount = AutoFitPageCount;
					break;
				case DashboardPrintingScaleMode.AutoFitWithinOnePage:
					scalingOpts.ScaleMode = ExtendedScaleMode.None;
					if(String.IsNullOrEmpty(itemType)) {
						opts.AutoPageOptions.AutoFitToPageSize = true;
						opts.AutoPageOptions.AutoRotate = true;
					}
					break;
			}
			opts.ScalingOptions = scalingOpts;
			opts.DocumentContentOptions.FilterStatePresentation = (FilterStatePresentation)DocumentContentOptions.FilterState;
			switch(itemType) {
				case DashboardItemType.Grid:
					opts.ItemContentOptions.SizeMode = GridOptions.FitToPageWidth ? ItemSizeMode.FitWidth : ItemSizeMode.None;
					opts.ItemContentOptions.HeadersOptions.PrintHeadersOnEveryPage = GridOptions.PrintHeadersOnEveryPage;
					break;
				case DashboardItemType.Pivot:
					opts.ItemContentOptions.HeadersOptions.PrintHeadersOnEveryPage = PivotOptions.PrintHeadersOnEveryPage;
					break;
				case DashboardItemType.Chart:
				case DashboardItemType.ScatterChart:
					opts.ItemContentOptions.SizeMode = (ItemSizeMode)ChartOptions.SizeMode;
					opts.AutoPageOptions.AutoRotate = ChartOptions.AutomaticPageLayout;
					break;
				case DashboardItemType.ChoroplethMap:
				case DashboardItemType.GeoPointMap:
				case DashboardItemType.BubbleMap:
				case DashboardItemType.PieMap:
					opts.ItemContentOptions.SizeMode = (ItemSizeMode)MapOptions.SizeMode;
					opts.AutoPageOptions.AutoRotate = MapOptions.AutomaticPageLayout;
					break;
				case DashboardItemType.RangeFilter:
					opts.ItemContentOptions.SizeMode = (ItemSizeMode)RangeFilterOptions.SizeMode;
					opts.AutoPageOptions.AutoRotate = RangeFilterOptions.AutomaticPageLayout;
					break;
				case DashboardItemType.Card:
					opts.ItemContentOptions.ArrangerOptions.AutoArrangeContent = CardOptions.AutoArrangeContent;
					break;
				case DashboardItemType.Gauge:
					opts.ItemContentOptions.ArrangerOptions.AutoArrangeContent = GaugeOptions.AutoArrangeContent;
					break;
				case DashboardItemType.Pie:
					opts.ItemContentOptions.ArrangerOptions.AutoArrangeContent = PieOptions.AutoArrangeContent;
					break;
			}
			return opts;
		}
	}
	public class DocumentContentPrintingOptions : OptionsBase {
		internal const DefaultBoolean DefaultShowTitle = DefaultBoolean.Default;
		const DashboardPrintingFilterState DefaultFilterState = DashboardPrintingFilterState.None;
		public DocumentContentPrintingOptions() {
			ShowTitle = DefaultShowTitle;
			FilterState = DefaultFilterState;
		}
		[DefaultValue(DefaultShowTitle), NotifyParentProperty(true)]
		public DefaultBoolean ShowTitle { get; set; }
		[DefaultValue(DefaultFilterState), NotifyParentProperty(true)]
		public DashboardPrintingFilterState FilterState { get; set; }
	}
	public class DashboardItemPrintingOptions : OptionsBase {
		const DefaultBoolean DefaultIncludeCaption = DefaultBoolean.Default;
		public DashboardItemPrintingOptions() {
			IncludeCaption = DefaultIncludeCaption;
		}
		[DefaultValue(DefaultIncludeCaption), NotifyParentProperty(true)]
		public DefaultBoolean IncludeCaption { get; set; }
	}
	public class ChartPrintingOptions : OptionsBase {
		const ChartPrintingSizeMode DefaultSizeMode = ChartPrintingSizeMode.Zoom;
		const bool DefaultAutomaticPageLayout = false;
		public ChartPrintingOptions() {
			SizeMode = DefaultSizeMode;
			AutomaticPageLayout = DefaultAutomaticPageLayout;
		}
		[DefaultValue(DefaultSizeMode), NotifyParentProperty(true)]
		public ChartPrintingSizeMode SizeMode { get; set; }
		[DefaultValue(DefaultAutomaticPageLayout), NotifyParentProperty(true)]
		public bool AutomaticPageLayout { get; set; }
	}
	public class RangeFilterPrintingOptions : OptionsBase {
		const RangeFilterPrintingSizeMode DefaultSizeMode = RangeFilterPrintingSizeMode.Stretch;
		const bool DefaultAutomaticPageLayout = false;
		public RangeFilterPrintingOptions() {
			SizeMode = DefaultSizeMode;
			AutomaticPageLayout = DefaultAutomaticPageLayout;
		}
		[DefaultValue(DefaultSizeMode), NotifyParentProperty(true)]
		public RangeFilterPrintingSizeMode SizeMode { get; set; }
		[DefaultValue(DefaultAutomaticPageLayout), NotifyParentProperty(true)]
		public bool AutomaticPageLayout { get; set; }
	}
	public class GridPrintingOptions : OptionsBase {
		const bool DefaultFitToPageWidth = true;
		const bool DefaultPrintHeadersOnEveryPage = true; 
		public GridPrintingOptions() {
			FitToPageWidth = DefaultFitToPageWidth;
			PrintHeadersOnEveryPage = DefaultPrintHeadersOnEveryPage; 
		}
		[DefaultValue(DefaultFitToPageWidth), NotifyParentProperty(true)]
		public bool FitToPageWidth { get; set; }
		[DefaultValue(DefaultPrintHeadersOnEveryPage), NotifyParentProperty(true)]
		public bool PrintHeadersOnEveryPage { get; set; }
	}
	public class PivotPrintingOptions : OptionsBase {
		const bool DefaultPrintHeadersOnEveryPage = true;
		public PivotPrintingOptions() {
			PrintHeadersOnEveryPage = DefaultPrintHeadersOnEveryPage;
		}
		[DefaultValue(DefaultPrintHeadersOnEveryPage), NotifyParentProperty(true)]
		public bool PrintHeadersOnEveryPage { get; set; }
	}
	public class PiePrintingOptions : OptionsBase {
		const bool DefaultAutoArrangeContent = true;
		public PiePrintingOptions() {
			AutoArrangeContent = DefaultAutoArrangeContent;
		}
		[DefaultValue(DefaultAutoArrangeContent), NotifyParentProperty(true)]
		public bool AutoArrangeContent { get; set; }
	}
	public class CardPrintingOptions : OptionsBase {
		const bool DefaultAutoArrangeContent = true;
		public CardPrintingOptions() {
			AutoArrangeContent = DefaultAutoArrangeContent;
		}
		[DefaultValue(DefaultAutoArrangeContent), NotifyParentProperty(true)]
		public bool AutoArrangeContent { get; set; }
	}
	public class GaugePrintingOptions : OptionsBase {
		const bool DefaultAutoArrangeContent = true;
		public GaugePrintingOptions() {
			AutoArrangeContent = DefaultAutoArrangeContent;
		}
		[DefaultValue(DefaultAutoArrangeContent), NotifyParentProperty(true)]
		public bool AutoArrangeContent { get; set; }
	}
	public class MapPrintingOptions : OptionsBase {
		const MapPrintingSizeMode DefaultSizeMode = MapPrintingSizeMode.Zoom;
		const bool DefaultAutomaticPageLayout = false;
		public MapPrintingOptions() {
			SizeMode = DefaultSizeMode;
			AutomaticPageLayout = DefaultAutomaticPageLayout;
		}
		[DefaultValue(DefaultSizeMode), NotifyParentProperty(true)]
		public MapPrintingSizeMode SizeMode { get; set; }
		[DefaultValue(DefaultAutomaticPageLayout), NotifyParentProperty(true)]
		public bool AutomaticPageLayout { get; set; }
	}
	public class OptionsBase {
		public override string ToString() {
			return OptionsHelper.GetObjectText(this);
		}
	}
	public class PrintingFontInfo {
		string fontName;
		byte gdiCharSet;
		bool useCustomFontInfo;
		public PrintingFontInfo()
			: base() {
		}
		[Editor("System.Drawing.Design.FontNameEditor, System.Drawing.Design", typeof(UITypeEditor))]
		[TypeConverterAttribute(typeof(FontConverter.FontNameConverter))]
		[NotifyParentProperty(true)]
		public string Name {
			get { return fontName; }
			set { fontName = value; }
		}
		[NotifyParentProperty(true)]
		public byte GdiCharSet {
			get { return gdiCharSet; }
			set { gdiCharSet = value; }
		}
		[DefaultValue(false), NotifyParentProperty(true)]
		public bool UseCustomFontInfo {
			get { return useCustomFontInfo; }
			set { useCustomFontInfo = value; }
		}
		public override string ToString() {
			return OptionsHelper.GetObjectText(this);
		}
		internal DashboardFontInfo GetDashboardFontInfo(){
			return useCustomFontInfo ? new DashboardFontInfo() { Name = fontName, GdiCharSet = gdiCharSet } : new DashboardFontInfo();
		}
	}
}
