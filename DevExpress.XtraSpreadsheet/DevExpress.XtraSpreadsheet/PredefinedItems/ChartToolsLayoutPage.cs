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
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.Gallery;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.Utils.Drawing;
namespace DevExpress.XtraSpreadsheet.UI {
	#region SpreadsheetChartsLayoutLabelsItemBuilder
	public class SpreadsheetChartsLayoutLabelsItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			PopulateChartTitleItems(items, creationContext);
			PopulateAxisTitlesItems(items, creationContext);
			PopulateChartLegendItems(items, creationContext);
			PopulateDataLabelsItems(items, creationContext);
		}
		void PopulateChartTitleItems(List<BarItem> items, BarCreationContextBase creationContext) {
			SpreadsheetCommandBarButtonGalleryDropDownItem dropDown = SpreadsheetCommandBarButtonGalleryDropDownItem.Create(SpreadsheetCommandId.ChartTitleCommandGroup);
			SpreadsheetCommandGalleryItemGroup group = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.ChartTitleCommandGroup);
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartTitleNone));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartTitleCenteredOverlay));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartTitleAbove));
			dropDown.GalleryDropDown.Gallery.Groups.Add(group);
			items.Add(dropDown);
			SetupVerticalGallery(dropDown.GalleryDropDown.Gallery, group.Items.Count);
		}
		void PopulateAxisTitlesItems(List<BarItem> items, BarCreationContextBase creationContext) {
			SpreadsheetCommandBarSubItem axesSubItem = SpreadsheetCommandBarSubItem.Create(SpreadsheetCommandId.ChartAxisTitlesCommandGroup);
			items.Add(axesSubItem);
			SpreadsheetCommandBarButtonGalleryDropDownItem primaryHorizontalAxisSubItem = SpreadsheetCommandBarButtonGalleryDropDownItem.Create(SpreadsheetCommandId.ChartPrimaryHorizontalAxisTitleCommandGroup);
			SpreadsheetCommandGalleryItemGroup horizontalGroup = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.ChartPrimaryHorizontalAxisTitleCommandGroup);
			horizontalGroup.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartPrimaryHorizontalAxisTitleNone));
			horizontalGroup.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartPrimaryHorizontalAxisTitleBelow));
			primaryHorizontalAxisSubItem.GalleryDropDown.Gallery.Groups.Add(horizontalGroup);
			axesSubItem.AddBarItem(primaryHorizontalAxisSubItem);
			SpreadsheetCommandBarButtonGalleryDropDownItem primaryVerticalAxisSubItem = SpreadsheetCommandBarButtonGalleryDropDownItem.Create(SpreadsheetCommandId.ChartPrimaryVerticalAxisTitleCommandGroup);
			SpreadsheetCommandGalleryItemGroup verticalGroup = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.ChartPrimaryVerticalAxisTitleCommandGroup);
			verticalGroup.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartPrimaryVerticalAxisTitleNone));
			verticalGroup.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartPrimaryVerticalAxisTitleRotated));
			verticalGroup.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartPrimaryVerticalAxisTitleVertical));
			verticalGroup.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartPrimaryVerticalAxisTitleHorizontal));
			primaryVerticalAxisSubItem.GalleryDropDown.Gallery.Groups.Add(verticalGroup);
			axesSubItem.AddBarItem(primaryVerticalAxisSubItem);
			SetupVerticalGallery(primaryHorizontalAxisSubItem.GalleryDropDown.Gallery, horizontalGroup.Items.Count);
			SetupVerticalGallery(primaryVerticalAxisSubItem.GalleryDropDown.Gallery, verticalGroup.Items.Count);
		}
		void PopulateChartLegendItems(List<BarItem> items, BarCreationContextBase creationContext) {
			SpreadsheetCommandBarButtonGalleryDropDownItem dropDown = SpreadsheetCommandBarButtonGalleryDropDownItem.Create(SpreadsheetCommandId.ChartLegendCommandGroup);
			SpreadsheetCommandGalleryItemGroup group = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.ChartLegendCommandGroup);
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartLegendNone));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartLegendAtRight));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartLegendAtTop));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartLegendAtLeft));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartLegendAtBottom));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartLegendOverlayAtRight));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartLegendOverlayAtLeft));
			dropDown.GalleryDropDown.Gallery.Groups.Add(group);
			items.Add(dropDown);
			SetupVerticalGallery(dropDown.GalleryDropDown.Gallery, group.Items.Count);
		}
		void PopulateDataLabelsItems(List<BarItem> items, BarCreationContextBase creationContext) {
			SpreadsheetCommandBarButtonGalleryDropDownItem dropDown = SpreadsheetCommandBarButtonGalleryDropDownItem.Create(SpreadsheetCommandId.ChartDataLabelsCommandGroup);
			SpreadsheetCommandGalleryItemGroup group = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.ChartDataLabelsCommandGroup);
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartDataLabelsNone));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartDataLabelsDefault));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartDataLabelsCenter));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartDataLabelsInsideEnd));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartDataLabelsInsideBase));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartDataLabelsOutsideEnd));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartDataLabelsBestFit));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartDataLabelsLeft));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartDataLabelsRight));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartDataLabelsAbove));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartDataLabelsBelow));
			dropDown.GalleryDropDown.Gallery.Groups.Add(group);
			items.Add(dropDown);
			SetupVerticalGallery(dropDown.GalleryDropDown.Gallery, group.Items.Count);
		}
		public static void SetupVerticalGallery(InDropDownGallery gallery, int rowCount) {
			gallery.ItemImageLocation = Locations.Left;
			gallery.ShowItemText = true;
			gallery.ItemImageLayout = ImageLayoutMode.MiddleLeft;
			gallery.ShowGroupCaption = false;
			gallery.ColumnCount = 1;
			gallery.RowCount = rowCount;
			gallery.AutoSize = GallerySizeMode.Both;
		}
	}
	#endregion
	#region SpreadsheetChartsLayoutAxesItemBuilder
	public class SpreadsheetChartsLayoutAxesItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			PopulateAxesItems(items, creationContext);
			PopulateGridlinesItems(items, creationContext);
		}
		void PopulateAxesItems(List<BarItem> items, BarCreationContextBase creationContext) {
			SpreadsheetCommandBarSubItem axesSubItem = SpreadsheetCommandBarSubItem.Create(SpreadsheetCommandId.ChartAxesCommandGroup);
			items.Add(axesSubItem);
			SpreadsheetCommandBarButtonGalleryDropDownItem primaryHorizontalAxisSubItem = SpreadsheetCommandBarButtonGalleryDropDownItem.Create(SpreadsheetCommandId.ChartPrimaryHorizontalAxisCommandGroup);
			SpreadsheetCommandGalleryItemGroup horizontalGroup = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.ChartPrimaryHorizontalAxisCommandGroup);
			horizontalGroup.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartHidePrimaryHorizontalAxis));
			horizontalGroup.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartPrimaryHorizontalAxisLeftToRight));
			horizontalGroup.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartPrimaryHorizontalAxisHideLabels));
			horizontalGroup.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartPrimaryHorizontalAxisRightToLeft));
			horizontalGroup.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartPrimaryHorizontalAxisDefault));
			horizontalGroup.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartPrimaryHorizontalAxisScaleThousands));
			horizontalGroup.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartPrimaryHorizontalAxisScaleMillions));
			horizontalGroup.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartPrimaryHorizontalAxisScaleBillions));
			horizontalGroup.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartPrimaryHorizontalAxisScaleLogarithm));
			primaryHorizontalAxisSubItem.GalleryDropDown.Gallery.Groups.Add(horizontalGroup);
			axesSubItem.AddBarItem(primaryHorizontalAxisSubItem);
			SpreadsheetCommandBarButtonGalleryDropDownItem primaryVerticalAxisSubItem = SpreadsheetCommandBarButtonGalleryDropDownItem.Create(SpreadsheetCommandId.ChartPrimaryVerticalAxisCommandGroup);
			SpreadsheetCommandGalleryItemGroup verticalGroup = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.ChartPrimaryVerticalAxisCommandGroup);
			verticalGroup.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartHidePrimaryVerticalAxis));
			verticalGroup.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartPrimaryVerticalAxisLeftToRight));
			verticalGroup.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartPrimaryVerticalAxisHideLabels));
			verticalGroup.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartPrimaryVerticalAxisRightToLeft));
			verticalGroup.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartPrimaryVerticalAxisDefault));
			verticalGroup.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartPrimaryVerticalAxisScaleThousands));
			verticalGroup.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartPrimaryVerticalAxisScaleMillions));
			verticalGroup.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartPrimaryVerticalAxisScaleBillions));
			verticalGroup.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartPrimaryVerticalAxisScaleLogarithm));
			primaryVerticalAxisSubItem.GalleryDropDown.Gallery.Groups.Add(verticalGroup);
			axesSubItem.AddBarItem(primaryVerticalAxisSubItem);
			SetupVerticalGallery(primaryHorizontalAxisSubItem.GalleryDropDown.Gallery, horizontalGroup.Items.Count);
			SetupVerticalGallery(primaryVerticalAxisSubItem.GalleryDropDown.Gallery, verticalGroup.Items.Count);
		}
		void PopulateGridlinesItems(List<BarItem> items, BarCreationContextBase creationContext) {
			SpreadsheetCommandBarSubItem gridlinesSubItem = SpreadsheetCommandBarSubItem.Create(SpreadsheetCommandId.ChartGridlinesCommandGroup);
			items.Add(gridlinesSubItem);
			SpreadsheetCommandBarButtonGalleryDropDownItem primaryHorizontalGridlinesSubItem = SpreadsheetCommandBarButtonGalleryDropDownItem.Create(SpreadsheetCommandId.ChartPrimaryHorizontalGridlinesCommandGroup);
			SpreadsheetCommandGalleryItemGroup horizontalGroup = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.ChartPrimaryHorizontalGridlinesCommandGroup);
			horizontalGroup.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartPrimaryHorizontalGridlinesNone));
			horizontalGroup.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartPrimaryHorizontalGridlinesMajor));
			horizontalGroup.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartPrimaryHorizontalGridlinesMinor));
			horizontalGroup.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartPrimaryHorizontalGridlinesMajorAndMinor));
			primaryHorizontalGridlinesSubItem.GalleryDropDown.Gallery.Groups.Add(horizontalGroup);
			gridlinesSubItem.AddBarItem(primaryHorizontalGridlinesSubItem);
			SpreadsheetCommandBarButtonGalleryDropDownItem primaryVerticalGridlinesSubItem = SpreadsheetCommandBarButtonGalleryDropDownItem.Create(SpreadsheetCommandId.ChartPrimaryVerticalGridlinesCommandGroup);
			SpreadsheetCommandGalleryItemGroup verticalGroup = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.ChartPrimaryVerticalGridlinesCommandGroup);
			verticalGroup.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartPrimaryVerticalGridlinesNone));
			verticalGroup.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartPrimaryVerticalGridlinesMajor));
			verticalGroup.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartPrimaryVerticalGridlinesMinor));
			verticalGroup.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartPrimaryVerticalGridlinesMajorAndMinor));
			primaryVerticalGridlinesSubItem.GalleryDropDown.Gallery.Groups.Add(verticalGroup);
			gridlinesSubItem.AddBarItem(primaryVerticalGridlinesSubItem);
			SetupVerticalGallery(primaryHorizontalGridlinesSubItem.GalleryDropDown.Gallery, horizontalGroup.Items.Count);
			SetupVerticalGallery(primaryVerticalGridlinesSubItem.GalleryDropDown.Gallery, verticalGroup.Items.Count);
		}
		void SetupVerticalGallery(InDropDownGallery gallery, int rowCount) {
			SpreadsheetChartsLayoutLabelsItemBuilder.SetupVerticalGallery(gallery, rowCount);
		}
	}
	#endregion
	#region SpreadsheetChartsLayoutAnalysisItemBuilder
	public class SpreadsheetChartsLayoutAnalysisItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			PopulateLinesItems(items, creationContext);
			PopulateUpDownBarsItems(items, creationContext);
			PopulateErrorBarsItems(items, creationContext);
		}
		void PopulateLinesItems(List<BarItem> items, BarCreationContextBase creationContext) {
			SpreadsheetCommandBarButtonGalleryDropDownItem dropDown = SpreadsheetCommandBarButtonGalleryDropDownItem.Create(SpreadsheetCommandId.ChartLinesCommandGroup);
			SpreadsheetCommandGalleryItemGroup group = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.ChartLinesCommandGroup);
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartLinesNone));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartShowDropLines));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartShowHighLowLines));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartShowDropLinesAndHighLowLines));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartShowSeriesLines));
			dropDown.GalleryDropDown.Gallery.Groups.Add(group);
			items.Add(dropDown);
			SetupVerticalGallery(dropDown.GalleryDropDown.Gallery, group.Items.Count);
		}
		void PopulateUpDownBarsItems(List<BarItem> items, BarCreationContextBase creationContext) {
			SpreadsheetCommandBarButtonGalleryDropDownItem dropDown = SpreadsheetCommandBarButtonGalleryDropDownItem.Create(SpreadsheetCommandId.ChartUpDownBarsCommandGroup);
			SpreadsheetCommandGalleryItemGroup group = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.ChartUpDownBarsCommandGroup);
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartHideUpDownBars));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartShowUpDownBars));
			dropDown.GalleryDropDown.Gallery.Groups.Add(group);
			items.Add(dropDown);
			SetupVerticalGallery(dropDown.GalleryDropDown.Gallery, group.Items.Count);
		}
		void PopulateErrorBarsItems(List<BarItem> items, BarCreationContextBase creationContext) {
			SpreadsheetCommandBarButtonGalleryDropDownItem dropDown = SpreadsheetCommandBarButtonGalleryDropDownItem.Create(SpreadsheetCommandId.ChartErrorBarsCommandGroup);
			SpreadsheetCommandGalleryItemGroup group = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.ChartErrorBarsCommandGroup);
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartErrorBarsNone));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartErrorBarsStandardError));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartErrorBarsPercentage));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ChartErrorBarsStandardDeviation));
			dropDown.GalleryDropDown.Gallery.Groups.Add(group);
			items.Add(dropDown);
			SetupVerticalGallery(dropDown.GalleryDropDown.Gallery, group.Items.Count);
		}
		void SetupVerticalGallery(InDropDownGallery gallery, int rowCount) {
			SpreadsheetChartsLayoutLabelsItemBuilder.SetupVerticalGallery(gallery, rowCount);
		}
	}
	#endregion
	#region SpreadsheetChartsLayoutLabelsBarCreator
	public class SpreadsheetChartsLayoutLabelsBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(ChartsLayoutRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(ChartsLayoutLabelsRibbonPageGroup); } }
		public override Type SupportedRibbonPageCategoryType { get { return typeof(ChartToolsRibbonPageCategory); } }
		public override Type SupportedBarType { get { return typeof(ChartsLayoutLabelsBar); } }
		public override int DockRow { get { return 2; } }
		public override int DockColumn { get { return 0; } }
		public override Bar CreateBar() {
			return new ChartsLayoutLabelsBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetChartsLayoutLabelsItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new ChartsLayoutRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new ChartsLayoutLabelsRibbonPageGroup();
		}
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new ChartToolsRibbonPageCategory();
		}
	}
	#endregion
	#region SpreadsheetChartsLayoutAxesBarCreator
	public class SpreadsheetChartsLayoutAxesBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(ChartsLayoutRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(ChartsLayoutAxesRibbonPageGroup); } }
		public override Type SupportedRibbonPageCategoryType { get { return typeof(ChartToolsRibbonPageCategory); } }
		public override Type SupportedBarType { get { return typeof(ChartsLayoutAxesBar); } }
		public override int DockRow { get { return 2; } }
		public override int DockColumn { get { return 0; } }
		public override Bar CreateBar() {
			return new ChartsLayoutAxesBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetChartsLayoutAxesItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new ChartsLayoutRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new ChartsLayoutAxesRibbonPageGroup();
		}
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new ChartToolsRibbonPageCategory();
		}
	}
	#endregion
	#region SpreadsheetChartsLayoutAnalysisBarCreator
	public class SpreadsheetChartsLayoutAnalysisBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(ChartsLayoutRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(ChartsLayoutAnalysisRibbonPageGroup); } }
		public override Type SupportedRibbonPageCategoryType { get { return typeof(ChartToolsRibbonPageCategory); } }
		public override Type SupportedBarType { get { return typeof(ChartsLayoutAnalysisBar); } }
		public override int DockRow { get { return 2; } }
		public override int DockColumn { get { return 0; } }
		public override Bar CreateBar() {
			return new ChartsLayoutAnalysisBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetChartsLayoutAnalysisItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new ChartsLayoutRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new ChartsLayoutAnalysisRibbonPageGroup();
		}
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new ChartToolsRibbonPageCategory();
		}
	}
	#endregion
	#region ChartsLayoutLabelsBar
	public class ChartsLayoutLabelsBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public ChartsLayoutLabelsBar() {
		}
		public ChartsLayoutLabelsBar(BarManager manager)
			: base(manager) {
		}
		public ChartsLayoutLabelsBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupChartsLayoutLabels); } }
	}
	#endregion
	#region ChartsLayoutAxesBar
	public class ChartsLayoutAxesBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public ChartsLayoutAxesBar() {
		}
		public ChartsLayoutAxesBar(BarManager manager)
			: base(manager) {
		}
		public ChartsLayoutAxesBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupChartsLayoutAxes); } }
	}
	#endregion
	#region ChartsLayoutAnalysisBar
	public class ChartsLayoutAnalysisBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public ChartsLayoutAnalysisBar() {
		}
		public ChartsLayoutAnalysisBar(BarManager manager)
			: base(manager) {
		}
		public ChartsLayoutAnalysisBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupChartsLayoutAnalysis); } }
	}
	#endregion
	#region ChartsLayoutRibbonPage
	public class ChartsLayoutRibbonPage : ControlCommandBasedRibbonPage {
		public ChartsLayoutRibbonPage() {
		}
		public ChartsLayoutRibbonPage(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PageChartsLayout); } }
	}
	#endregion
	#region ChartsLayoutLabelsRibbonPageGroup
	public class ChartsLayoutLabelsRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public ChartsLayoutLabelsRibbonPageGroup() {
		}
		public ChartsLayoutLabelsRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupChartsLayoutLabels); } }
	}
	#endregion
	#region ChartsLayoutAxesRibbonPageGroup
	public class ChartsLayoutAxesRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public ChartsLayoutAxesRibbonPageGroup() {
		}
		public ChartsLayoutAxesRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupChartsLayoutAxes); } }
	}
	#endregion
	#region ChartsLayoutAnalysisRibbonPageGroup
	public class ChartsLayoutAnalysisRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public ChartsLayoutAnalysisRibbonPageGroup() {
		}
		public ChartsLayoutAnalysisRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupChartsLayoutAnalysis); } }
	}
	#endregion
	#region ChartToolsRibbonPageCategory
	public class ChartToolsRibbonPageCategory : ControlCommandBasedRibbonPageCategory<SpreadsheetControl, SpreadsheetCommandId> {
		public ChartToolsRibbonPageCategory() {
			this.Visible = false;
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PageCategoryChartTools); } }
		protected override SpreadsheetCommandId EmptyCommandId { get { return SpreadsheetCommandId.None; } }
		public override SpreadsheetCommandId CommandId { get { return SpreadsheetCommandId.ToolsChartCommandGroup; } }
		protected override RibbonPageCategory CreateRibbonPageCategory() {
			return new ChartToolsRibbonPageCategory();
		}
	}
	#endregion
}
