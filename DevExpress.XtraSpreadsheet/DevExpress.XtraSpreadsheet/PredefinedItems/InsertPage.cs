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
using DevExpress.XtraBars;
using System.Collections.Generic;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraSpreadsheet.Localization;
namespace DevExpress.XtraSpreadsheet.UI {
	#region SpreadsheetTablesItemBuilder
	public class SpreadsheetTablesItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.InsertPivotTable));
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.InsertTable));
		}
	}
	#endregion
	#region SpreadsheetIllustrationsItemBuilder
	public class SpreadsheetIllustrationsItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.InsertPicture));
		}
	}
	#endregion
	#region SpreadsheetChartsItemBuilder
	public class SpreadsheetChartsItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			PopulateColumnChartsItems(items, creationContext);
			PopulateLineChartsItems(items, creationContext);
			PopulatePieChartsItems(items, creationContext);
			PopulateBarChartsItems(items, creationContext);
			PopulateAreaChartsItems(items, creationContext);
			PopulateScatterChartsItems(items, creationContext);
			PopulateOtherChartsItems(items, creationContext);
		}
		void PopulateColumnChartsItems(List<BarItem> items, BarCreationContextBase creationContext) {
			SpreadsheetCommandBarButtonGalleryDropDownItem dropDown = SpreadsheetCommandBarButtonGalleryDropDownItem.Create(SpreadsheetCommandId.InsertChartColumnCommandGroup);
			dropDown.GalleryDropDown.Gallery.ColumnCount = 4;
			dropDown.GalleryDropDown.Gallery.RowCount = 5;
			SpreadsheetCommandGalleryItemGroup group2D = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.InsertChartColumn2DCommandGroup);
			group2D.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartColumnClustered2D));
			group2D.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartColumnStacked2D));
			group2D.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartColumnPercentStacked2D));
			dropDown.GalleryDropDown.Gallery.Groups.Add(group2D);
			SpreadsheetCommandGalleryItemGroup group3D = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.InsertChartColumn3DCommandGroup);
			group3D.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartColumnClustered3D));
			group3D.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartColumnStacked3D));
			group3D.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartColumnPercentStacked3D));
			group3D.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartColumn3D));
			dropDown.GalleryDropDown.Gallery.Groups.Add(group3D);
			SpreadsheetCommandGalleryItemGroup groupCylinder = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.InsertChartCylinderCommandGroup);
			groupCylinder.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartCylinderClustered));
			groupCylinder.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartCylinderStacked));
			groupCylinder.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartCylinderPercentStacked));
			groupCylinder.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartCylinder));
			dropDown.GalleryDropDown.Gallery.Groups.Add(groupCylinder);
			SpreadsheetCommandGalleryItemGroup groupCone = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.InsertChartConeCommandGroup);
			groupCone.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartConeClustered));
			groupCone.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartConeStacked));
			groupCone.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartConePercentStacked));
			groupCone.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartCone));
			dropDown.GalleryDropDown.Gallery.Groups.Add(groupCone);
			SpreadsheetCommandGalleryItemGroup groupPyramid = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.InsertChartPyramidCommandGroup);
			groupPyramid.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartPyramidClustered));
			groupPyramid.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartPyramidStacked));
			groupPyramid.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartPyramidPercentStacked));
			groupPyramid.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartPyramid));
			dropDown.GalleryDropDown.Gallery.Groups.Add(groupPyramid);
			items.Add(dropDown);
		}
		void PopulateBarChartsItems(List<BarItem> items, BarCreationContextBase creationContext) {
			SpreadsheetCommandBarButtonGalleryDropDownItem dropDown = SpreadsheetCommandBarButtonGalleryDropDownItem.Create(SpreadsheetCommandId.InsertChartBarCommandGroup);
			dropDown.GalleryDropDown.Gallery.ColumnCount = 3;
			dropDown.GalleryDropDown.Gallery.RowCount = 5;
			SpreadsheetCommandGalleryItemGroup group2D = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.InsertChartBar2DCommandGroup);
			group2D.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartBarClustered2D));
			group2D.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartBarStacked2D));
			group2D.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartBarPercentStacked2D));
			dropDown.GalleryDropDown.Gallery.Groups.Add(group2D);
			SpreadsheetCommandGalleryItemGroup group3D = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.InsertChartBar3DCommandGroup);
			group3D.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartBarClustered3D));
			group3D.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartBarStacked3D));
			group3D.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartBarPercentStacked3D));
			dropDown.GalleryDropDown.Gallery.Groups.Add(group3D);
			SpreadsheetCommandGalleryItemGroup groupCylinder = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.InsertChartHorizontalCylinderCommandGroup);
			groupCylinder.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartHorizontalCylinderClustered));
			groupCylinder.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartHorizontalCylinderStacked));
			groupCylinder.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartHorizontalCylinderPercentStacked));
			dropDown.GalleryDropDown.Gallery.Groups.Add(groupCylinder);
			SpreadsheetCommandGalleryItemGroup groupCone = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.InsertChartHorizontalConeCommandGroup);
			groupCone.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartHorizontalConeClustered));
			groupCone.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartHorizontalConeStacked));
			groupCone.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartHorizontalConePercentStacked));
			dropDown.GalleryDropDown.Gallery.Groups.Add(groupCone);
			SpreadsheetCommandGalleryItemGroup groupPyramid = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.InsertChartHorizontalPyramidCommandGroup);
			groupPyramid.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartHorizontalPyramidClustered));
			groupPyramid.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartHorizontalPyramidStacked));
			groupPyramid.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartHorizontalPyramidPercentStacked));
			dropDown.GalleryDropDown.Gallery.Groups.Add(groupPyramid);
			items.Add(dropDown);
		}
		void PopulateLineChartsItems(List<BarItem> items, BarCreationContextBase creationContext) {
			SpreadsheetCommandBarButtonGalleryDropDownItem dropDown = SpreadsheetCommandBarButtonGalleryDropDownItem.Create(SpreadsheetCommandId.InsertChartLineCommandGroup);
			dropDown.GalleryDropDown.Gallery.ColumnCount = 3;
			dropDown.GalleryDropDown.Gallery.RowCount = 3;
			SpreadsheetCommandGalleryItemGroup group2D = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.InsertChartLine2DCommandGroup);
			group2D.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartLine));
			group2D.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartStackedLine));
			group2D.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartPercentStackedLine));
			group2D.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartLineWithMarkers));
			group2D.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartStackedLineWithMarkers));
			group2D.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartPercentStackedLineWithMarkers));
			dropDown.GalleryDropDown.Gallery.Groups.Add(group2D);
			SpreadsheetCommandGalleryItemGroup group3D = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.InsertChartLine3DCommandGroup);
			group3D.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartLine3D));
			dropDown.GalleryDropDown.Gallery.Groups.Add(group3D);
			items.Add(dropDown);
		}
		void PopulatePieChartsItems(List<BarItem> items, BarCreationContextBase creationContext) {
			SpreadsheetCommandBarButtonGalleryDropDownItem dropDown = SpreadsheetCommandBarButtonGalleryDropDownItem.Create(SpreadsheetCommandId.InsertChartPieCommandGroup);
			dropDown.GalleryDropDown.Gallery.ColumnCount = 2;
			dropDown.GalleryDropDown.Gallery.RowCount = 3;
			SpreadsheetCommandGalleryItemGroup group2D = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.InsertChartPie2DCommandGroup);
			group2D.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartPie2D));
			group2D.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartPieExploded2D));
			dropDown.GalleryDropDown.Gallery.Groups.Add(group2D);
			SpreadsheetCommandGalleryItemGroup group3D = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.InsertChartPie3DCommandGroup);
			group3D.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartPie3D));
			group3D.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartPieExploded3D));
			dropDown.GalleryDropDown.Gallery.Groups.Add(group3D);
			SpreadsheetCommandGalleryItemGroup doughnutGroup2D = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.InsertChartDoughnut2DCommandGroup);
			doughnutGroup2D.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartDoughnut2D));
			doughnutGroup2D.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartDoughnutExploded2D));
			dropDown.GalleryDropDown.Gallery.Groups.Add(doughnutGroup2D);
			items.Add(dropDown);
		}
		void PopulateAreaChartsItems(List<BarItem> items, BarCreationContextBase creationContext) {
			SpreadsheetCommandBarButtonGalleryDropDownItem dropDown = SpreadsheetCommandBarButtonGalleryDropDownItem.Create(SpreadsheetCommandId.InsertChartAreaCommandGroup);
			dropDown.GalleryDropDown.Gallery.ColumnCount = 3;
			dropDown.GalleryDropDown.Gallery.RowCount = 2;
			SpreadsheetCommandGalleryItemGroup group2D = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.InsertChartArea2DCommandGroup);
			group2D.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartArea));
			group2D.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartStackedArea));
			group2D.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartPercentStackedArea));
			dropDown.GalleryDropDown.Gallery.Groups.Add(group2D);
			SpreadsheetCommandGalleryItemGroup group3D = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.InsertChartArea3DCommandGroup);
			group3D.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartArea3D));
			group3D.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartStackedArea3D));
			group3D.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartPercentStackedArea3D));
			dropDown.GalleryDropDown.Gallery.Groups.Add(group3D);
			items.Add(dropDown);
		}
		void PopulateScatterChartsItems(List<BarItem> items, BarCreationContextBase creationContext) {
			SpreadsheetCommandBarButtonGalleryDropDownItem dropDown = SpreadsheetCommandBarButtonGalleryDropDownItem.Create(SpreadsheetCommandId.InsertChartScatterCommandGroup);
			dropDown.GalleryDropDown.Gallery.ColumnCount = 2;
			dropDown.GalleryDropDown.Gallery.RowCount = 4;
			SpreadsheetCommandGalleryItemGroup groupScatter = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.InsertChartScatterCommandGroup);
			groupScatter.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartScatterMarkers));
			groupScatter.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartScatterSmoothLinesAndMarkers));
			groupScatter.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartScatterSmoothLines));
			groupScatter.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartScatterLinesAndMarkers));
			groupScatter.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartScatterLines));
			dropDown.GalleryDropDown.Gallery.Groups.Add(groupScatter);
			SpreadsheetCommandGalleryItemGroup groupBubbles = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.InsertChartBubbleCommandGroup);
			groupBubbles.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartBubble));
			groupBubbles.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartBubble3D));
			dropDown.GalleryDropDown.Gallery.Groups.Add(groupBubbles);
			items.Add(dropDown);
		}
		void PopulateOtherChartsItems(List<BarItem> items, BarCreationContextBase creationContext) {
			SpreadsheetCommandBarButtonGalleryDropDownItem dropDown = SpreadsheetCommandBarButtonGalleryDropDownItem.Create(SpreadsheetCommandId.InsertChartOtherCommandGroup);
			dropDown.GalleryDropDown.Gallery.ColumnCount = 4;
			dropDown.GalleryDropDown.Gallery.RowCount = 2;
			SpreadsheetCommandGalleryItemGroup groupStock = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.InsertChartStockCommandGroup);
			groupStock.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartStockHighLowClose));
			groupStock.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartStockOpenHighLowClose));
			groupStock.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartStockVolumeHighLowClose));
			groupStock.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartStockVolumeOpenHighLowClose));
			dropDown.GalleryDropDown.Gallery.Groups.Add(groupStock);
			SpreadsheetCommandGalleryItemGroup groupRadar = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.InsertChartRadarCommandGroup);
			groupRadar.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartRadar));
			groupRadar.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartRadarWithMarkers));
			groupRadar.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartRadarFilled));
			dropDown.GalleryDropDown.Gallery.Groups.Add(groupRadar);
			items.Add(dropDown);
		}
	}
	#endregion
	#region SpreadsheetLinksItemBuilder
	public class SpreadsheetLinksItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.InsertHyperlink));
		}
	}
	#endregion
	#region SpreadsheetSymbolsItemBuilder
	public class SpreadsheetSymbolsItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.InsertSymbol));
		}
	}
	#endregion
	#region SpreadsheetTablesBarCreator
	public class SpreadsheetTablesBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(InsertRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(TablesRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(TablesBar); } }
		public override int DockRow { get { return 2; } }
		public override int DockColumn { get { return 0; } }
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetTablesItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new InsertRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new TablesRibbonPageGroup();
		}
		public override Bar CreateBar() {
			return new TablesBar();
		}
	}
	#endregion
	#region SpreadsheetIllustrationsBarCreator
	public class SpreadsheetIllustrationsBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(InsertRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(IllustrationsRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(IllustrationsBar); } }
		public override int DockRow { get { return 2; } }
		public override int DockColumn { get { return 1; } }
		public override Bar CreateBar() {
			return new IllustrationsBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetIllustrationsItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new InsertRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new IllustrationsRibbonPageGroup();
		}
	}
	#endregion
	#region SpreadsheetChartsBarCreator
	public class SpreadsheetChartsBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(InsertRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(ChartsRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(ChartsBar); } }
		public override int DockRow { get { return 2; } }
		public override int DockColumn { get { return 1; } }
		public override Bar CreateBar() {
			return new ChartsBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetChartsItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new InsertRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new ChartsRibbonPageGroup();
		}
	}
	#endregion
	#region SpreadsheetLinksBarCreator
	public class SpreadsheetLinksBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(InsertRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(LinksRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(LinksBar); } }
		public override int DockRow { get { return 2; } }
		public override int DockColumn { get { return 2; } }
		public override Bar CreateBar() {
			return new LinksBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetLinksItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new InsertRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new LinksRibbonPageGroup();
		}
	}
	#endregion
	#region SpreadsheetSymbolsBarCreator
	public class SpreadsheetSymbolsBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(InsertRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(SymbolsRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(SymbolsBar); } }
		public override int DockRow { get { return 2; } }
		public override int DockColumn { get { return 2; } }
		public override Bar CreateBar() {
			return new SymbolsBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetSymbolsItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new InsertRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new SymbolsRibbonPageGroup();
		}
	}
	#endregion
	#region TablesBar
	public class TablesBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public TablesBar() {
		}
		public TablesBar(BarManager manager)
			: base(manager) {
		}
		public TablesBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupTables); } }
	}
	#endregion
	#region IllustrationsBar
	public class IllustrationsBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public IllustrationsBar() {
		}
		public IllustrationsBar(BarManager manager)
			: base(manager) {
		}
		public IllustrationsBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupIllustrations); } }
	}
	#endregion
	#region ChartsBar
	public class ChartsBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public ChartsBar() {
		}
		public ChartsBar(BarManager manager)
			: base(manager) {
		}
		public ChartsBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupCharts); } }
	}
	#endregion
	#region LinksBar
	public class LinksBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public LinksBar() {
		}
		public LinksBar(BarManager manager)
			: base(manager) {
		}
		public LinksBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupLinks); } }
	}
	#endregion
	#region SymbolsBar
	public class SymbolsBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public SymbolsBar() {
		}
		public SymbolsBar(BarManager manager)
			: base(manager) {
		}
		public SymbolsBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupSymbols); } }
	}
	#endregion
	#region InsertRibbonPage
	public class InsertRibbonPage : ControlCommandBasedRibbonPage {
		public InsertRibbonPage() {
		}
		public InsertRibbonPage(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PageInsert); } }
	}
	#endregion
	#region TablesRibbonPageGroup
	public class TablesRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public TablesRibbonPageGroup() {
		}
		public TablesRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupTables); } }
	}
	#endregion
	#region IllustrationsRibbonPageGroup
	public class IllustrationsRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public IllustrationsRibbonPageGroup() {
		}
		public IllustrationsRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupIllustrations); } }
	}
	#endregion
	#region ChartsRibbonPageGroup
	public class ChartsRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public ChartsRibbonPageGroup() {
		}
		public ChartsRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupCharts); } }
	}
	#endregion
	#region LinksRibbonPageGroup
	public class LinksRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public LinksRibbonPageGroup() {
		}
		public LinksRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupLinks); } }
	}
	#endregion
	#region SymbolsRibbonPageGroup
	public class SymbolsRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public SymbolsRibbonPageGroup() {
		}
		public SymbolsRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupSymbols); } }
	}
	#endregion
}
