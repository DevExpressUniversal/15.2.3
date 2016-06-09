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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.Gallery;
using DevExpress.XtraCharts.Commands;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts.UI {
	#region ChartTypesItemBuilder
	public class ChartTypesItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			CreateBarBaseItem createBarBaseItem = new CreateBarBaseItem();
			ChartControlCommandGalleryItemGroup2DColumn group2DColumns = new ChartControlCommandGalleryItemGroup2DColumn();
			ChartControlCommandGalleryItemGroup3DColumn group3DColumns = new ChartControlCommandGalleryItemGroup3DColumn();
			ChartControlCommandGalleryItemGroupCylinderColumn groupCylinderColumns = new ChartControlCommandGalleryItemGroupCylinderColumn();
			ChartControlCommandGalleryItemGroupConeColumn groupConeColumns = new ChartControlCommandGalleryItemGroupConeColumn();
			ChartControlCommandGalleryItemGroupPyramidColumn groupPyramidColumns = new ChartControlCommandGalleryItemGroupPyramidColumn();
			group2DColumns.Items.Add(new CreateBarChartItem());
			group2DColumns.Items.Add(new CreateFullStackedBarChartItem());
			group2DColumns.Items.Add(new CreateSideBySideFullStackedBarChartItem());
			group2DColumns.Items.Add(new CreateSideBySideStackedBarChartItem());
			group2DColumns.Items.Add(new CreateStackedBarChartItem());
			group3DColumns.Items.Add(new CreateBar3DChartItem());
			group3DColumns.Items.Add(new CreateFullStackedBar3DChartItem());
			group3DColumns.Items.Add(new CreateManhattanBarChartItem());
			group3DColumns.Items.Add(new CreateSideBySideFullStackedBar3DChartItem());
			group3DColumns.Items.Add(new CreateSideBySideStackedBar3DChartItem());
			group3DColumns.Items.Add(new CreateStackedBar3DChartItem());
			groupCylinderColumns.Items.Add(new CreateCylinderBar3DChartItem());
			groupCylinderColumns.Items.Add(new CreateCylinderFullStackedBar3DChartItem());
			groupCylinderColumns.Items.Add(new CreateCylinderManhattanBarChartItem());
			groupCylinderColumns.Items.Add(new CreateCylinderSideBySideFullStackedBar3DChartItem());
			groupCylinderColumns.Items.Add(new CreateCylinderSideBySideStackedBar3DChartItem());
			groupCylinderColumns.Items.Add(new CreateCylinderStackedBar3DChartItem());
			groupConeColumns.Items.Add(new CreateConeBar3DChartItem());
			groupConeColumns.Items.Add(new CreateConeFullStackedBar3DChartItem());
			groupConeColumns.Items.Add(new CreateConeManhattanBarChartItem());
			groupConeColumns.Items.Add(new CreateConeSideBySideFullStackedBar3DChartItem());
			groupConeColumns.Items.Add(new CreateConeSideBySideStackedBar3DChartItem());
			groupConeColumns.Items.Add(new CreateConeStackedBar3DChartItem());
			groupPyramidColumns.Items.Add(new CreatePyramidBar3DChartItem());
			groupPyramidColumns.Items.Add(new CreatePyramidFullStackedBar3DChartItem());
			groupPyramidColumns.Items.Add(new CreatePyramidManhattanBarChartItem());
			groupPyramidColumns.Items.Add(new CreatePyramidSideBySideFullStackedBar3DChartItem());
			groupPyramidColumns.Items.Add(new CreatePyramidSideBySideStackedBar3DChartItem());
			groupPyramidColumns.Items.Add(new CreatePyramidStackedBar3DChartItem());
			createBarBaseItem.GalleryDropDown.Gallery.Groups.Add(group2DColumns);
			createBarBaseItem.GalleryDropDown.Gallery.Groups.Add(group3DColumns);
			createBarBaseItem.GalleryDropDown.Gallery.Groups.Add(groupCylinderColumns);
			createBarBaseItem.GalleryDropDown.Gallery.Groups.Add(groupConeColumns);
			createBarBaseItem.GalleryDropDown.Gallery.Groups.Add(groupPyramidColumns);
			items.Add(createBarBaseItem);
			CreateLineBaseItem createLineBaseItem = new CreateLineBaseItem();
			ChartControlCommandGalleryItemGroup2DLine group2DLines = new ChartControlCommandGalleryItemGroup2DLine();
			ChartControlCommandGalleryItemGroup3DLine group3DLines = new ChartControlCommandGalleryItemGroup3DLine();
			group2DLines.Items.Add(new CreateLineChartItem());
			group2DLines.Items.Add(new CreateFullStackedLineChartItem());
			group2DLines.Items.Add(new CreateScatterLineChartItem());
			group2DLines.Items.Add(new CreateSplineChartItem());
			group2DLines.Items.Add(new CreateStackedLineChartItem());
			group2DLines.Items.Add(new CreateStepLineChartItem());
			group3DLines.Items.Add(new CreateLine3DChartItem());
			group3DLines.Items.Add(new CreateFullStackedLine3DChartItem());
			group3DLines.Items.Add(new CreateSpline3DChartItem());
			group3DLines.Items.Add(new CreateStackedLine3DChartItem());
			group3DLines.Items.Add(new CreateStepLine3DChartItem());
			createLineBaseItem.GalleryDropDown.Gallery.Groups.Add(group2DLines);
			createLineBaseItem.GalleryDropDown.Gallery.Groups.Add(group3DLines);
			items.Add(createLineBaseItem);
			CreatePieBaseItem createPieBaseItem = new CreatePieBaseItem();
			ChartControlCommandGalleryItemGroup2DPie group2DPie = new ChartControlCommandGalleryItemGroup2DPie();
			ChartControlCommandGalleryItemGroup3DPie group3DPie = new ChartControlCommandGalleryItemGroup3DPie();
			group2DPie.Items.Add(new CreatePieChartItem());
			group2DPie.Items.Add(new CreateDoughnutChartItem());
			group2DPie.Items.Add(new CreateNestedDoughnutChartItem());
			group3DPie.Items.Add(new CreatePie3DChartItem());
			group3DPie.Items.Add(new CreateDoughnut3DChartItem());
			createPieBaseItem.GalleryDropDown.Gallery.Groups.Add(group2DPie);
			createPieBaseItem.GalleryDropDown.Gallery.Groups.Add(group3DPie);
			items.Add(createPieBaseItem);
			CreateRotatedBarBaseItem createRotatedBarBaseItem = new CreateRotatedBarBaseItem();
			ChartControlCommandGalleryItemGroup2DBar group2DBar = new ChartControlCommandGalleryItemGroup2DBar();
			group2DBar.Items.Add(new CreateRotatedBarChartItem());
			group2DBar.Items.Add(new CreateRotatedFullStackedBarChartItem());
			group2DBar.Items.Add(new CreateRotatedSideBySideFullStackedBarChartItem());
			group2DBar.Items.Add(new CreateRotatedSideBySideStackedBarChartItem());
			group2DBar.Items.Add(new CreateRotatedStackedBarChartItem());
			createRotatedBarBaseItem.GalleryDropDown.Gallery.Groups.Add(group2DBar);
			items.Add(createRotatedBarBaseItem);
			CreateAreaBaseItem createAreaBaseItem = new CreateAreaBaseItem();
			ChartControlCommandGalleryItemGroup2DArea group2DArea = new ChartControlCommandGalleryItemGroup2DArea();
			ChartControlCommandGalleryItemGroup3DArea group3DArea = new ChartControlCommandGalleryItemGroup3DArea();
			group2DArea.Items.Add(new CreateAreaChartItem());
			group2DArea.Items.Add(new CreateFullStackedAreaChartItem());
			group2DArea.Items.Add(new CreateFullStackedSplineAreaChartItem());
			group2DArea.Items.Add(new CreateSplineAreaChartItem());
			group2DArea.Items.Add(new CreateStackedAreaChartItem());
			group2DArea.Items.Add(new CreateStackedSplineAreaChartItem());
			group2DArea.Items.Add(new CreateStepAreaChartItem());
			group3DArea.Items.Add(new CreateArea3DChartItem());
			group3DArea.Items.Add(new CreateFullStackedArea3DChartItem());
			group3DArea.Items.Add(new CreateFullStackedSplineArea3DChartItem());
			group3DArea.Items.Add(new CreateSplineArea3DChartItem());
			group3DArea.Items.Add(new CreateStackedArea3DChartItem());
			group3DArea.Items.Add(new CreateStackedSplineArea3DChartItem());
			group3DArea.Items.Add(new CreateStepArea3DChartItem());
			createAreaBaseItem.GalleryDropDown.Gallery.Groups.Add(group2DArea);
			createAreaBaseItem.GalleryDropDown.Gallery.Groups.Add(group3DArea);
			items.Add(createAreaBaseItem);
			CreateOtherSeriesTypesBaseItem createOtherSeriesTypesBaseItem = new CreateOtherSeriesTypesBaseItem();
			ChartControlCommandGalleryItemGroupPoint groupPoint = new ChartControlCommandGalleryItemGroupPoint();
			ChartControlCommandGalleryItemGroupFunnel groupFunnel = new ChartControlCommandGalleryItemGroupFunnel();
			ChartControlCommandGalleryItemGroupFinancial groupFinancial = new ChartControlCommandGalleryItemGroupFinancial();
			ChartControlCommandGalleryItemGroupRadar groupRadar = new ChartControlCommandGalleryItemGroupRadar();
			ChartControlCommandGalleryItemGroupPolar groupPolar = new ChartControlCommandGalleryItemGroupPolar();
			ChartControlCommandGalleryItemGroupRange groupRange = new ChartControlCommandGalleryItemGroupRange();
			ChartControlCommandGalleryItemGroupGantt groupGantt = new ChartControlCommandGalleryItemGroupGantt();
			groupPoint.Items.Add(new CreatePointChartItem());
			groupPoint.Items.Add(new CreateBubbleChartItem());
			groupFunnel.Items.Add(new CreateFunnelChartItem());
			groupFunnel.Items.Add(new CreateFunnel3DChartItem());
			groupFinancial.Items.Add(new CreateStockChartItem());
			groupFinancial.Items.Add(new CreateCandleStickChartItem());
			groupRadar.Items.Add(new CreateRadarPointChartItem());
			groupRadar.Items.Add(new CreateRadarLineChartItem());
			groupRadar.Items.Add(new CreateRadarAreaChartItem());
			groupRadar.Items.Add(new CreateScatterRadarLineChartItem());
			groupPolar.Items.Add(new CreatePolarPointChartItem());
			groupPolar.Items.Add(new CreatePolarLineChartItem());
			groupPolar.Items.Add(new CreatePolarAreaChartItem());
			groupPolar.Items.Add(new CreateScatterPolarLineChartItem());
			groupRange.Items.Add(new CreateRangeBarChartItem());
			groupRange.Items.Add(new CreateSideBySideRangeBarChartItem());
			groupRange.Items.Add(new CreateRangeAreaChartItem());
			groupRange.Items.Add(new CreateRangeArea3DChartItem());
			groupGantt.Items.Add(new CreateGanttChartItem());
			groupGantt.Items.Add(new CreateSideBySideGanttChartItem());
			createOtherSeriesTypesBaseItem.GalleryDropDown.Gallery.Groups.Add(groupPoint);
			createOtherSeriesTypesBaseItem.GalleryDropDown.Gallery.Groups.Add(groupFunnel);
			createOtherSeriesTypesBaseItem.GalleryDropDown.Gallery.Groups.Add(groupFinancial);
			createOtherSeriesTypesBaseItem.GalleryDropDown.Gallery.Groups.Add(groupRadar);
			createOtherSeriesTypesBaseItem.GalleryDropDown.Gallery.Groups.Add(groupPolar);
			createOtherSeriesTypesBaseItem.GalleryDropDown.Gallery.Groups.Add(groupRange);
			createOtherSeriesTypesBaseItem.GalleryDropDown.Gallery.Groups.Add(groupGantt);
			items.Add(createOtherSeriesTypesBaseItem);
		}
	}
	#endregion
	#region ChartAppearanceItemBuilder
	public class ChartAppearanceItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			ChangePaletteGalleryBaseItem changePaletteGalleryBaseItem = new ChangePaletteGalleryBaseItem();
			items.Add(changePaletteGalleryBaseItem);
			BarItem createAppearanceGalleryBaseItem;
			if (creationContext.IsRibbon)
				createAppearanceGalleryBaseItem = new ChangeAppearanceGalleryBaseItem();
			else
				createAppearanceGalleryBaseItem = new ChangeAppearanceGalleryBaseBarManagerItem();
			items.Add(createAppearanceGalleryBaseItem);
		}
	}
	#endregion
	#region ChartControlCommandGalleryItemGroup2DColumn
	public class ChartControlCommandGalleryItemGroup2DColumn : ChartControlCommandGalleryItemGroup {
		public ChartControlCommandGalleryItemGroup2DColumn()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.Column2DGroupPlaceHolder; } }
	}
	#endregion
	#region ChartControlCommandGalleryItemGroup3DColumn
	public class ChartControlCommandGalleryItemGroup3DColumn : ChartControlCommandGalleryItemGroup {
		public ChartControlCommandGalleryItemGroup3DColumn()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.Column3DGroupPlaceHolder; } }
	}
	#endregion
	#region ChartControlCommandGalleryItemGroupCylinderColumn
	public class ChartControlCommandGalleryItemGroupCylinderColumn : ChartControlCommandGalleryItemGroup {
		public ChartControlCommandGalleryItemGroupCylinderColumn()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.ColumnCylinderGroupPlaceHolder; } }
	}
	#endregion
	#region ChartControlCommandGalleryItemGroupConeColumn
	public class ChartControlCommandGalleryItemGroupConeColumn : ChartControlCommandGalleryItemGroup {
		public ChartControlCommandGalleryItemGroupConeColumn()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.ColumnConeGroupPlaceHolder; } }
	}
	#endregion
	#region ChartControlCommandGalleryItemGroupPyramidColumn
	public class ChartControlCommandGalleryItemGroupPyramidColumn : ChartControlCommandGalleryItemGroup {
		public ChartControlCommandGalleryItemGroupPyramidColumn()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.ColumnPyramidGroupPlaceHolder; } }
	}
	#endregion
	#region ChartControlCommandGalleryItemGroup2DLine
	public class ChartControlCommandGalleryItemGroup2DLine : ChartControlCommandGalleryItemGroup {
		public ChartControlCommandGalleryItemGroup2DLine()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.Line2DGroupPlaceHolder; } }
	}
	#endregion
	#region ChartControlCommandGalleryItemGroup3DLine
	public class ChartControlCommandGalleryItemGroup3DLine : ChartControlCommandGalleryItemGroup {
		public ChartControlCommandGalleryItemGroup3DLine()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.Line3DGroupPlaceHolder; } }
	}
	#endregion
	#region ChartControlCommandGalleryItemGroup2DPie
	public class ChartControlCommandGalleryItemGroup2DPie : ChartControlCommandGalleryItemGroup {
		public ChartControlCommandGalleryItemGroup2DPie()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.Pie2DGroupPlaceHolder; } }
	}
	#endregion
	#region ChartControlCommandGalleryItemGroup3DPie
	public class ChartControlCommandGalleryItemGroup3DPie : ChartControlCommandGalleryItemGroup {
		public ChartControlCommandGalleryItemGroup3DPie()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.Pie3DGroupPlaceHolder; } }
	}
	#endregion
	#region ChartControlCommandGalleryItemGroup2DBar
	public class ChartControlCommandGalleryItemGroup2DBar : ChartControlCommandGalleryItemGroup {
		public ChartControlCommandGalleryItemGroup2DBar()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.Bar2DGroupPlaceHolder; } }
	}
	#endregion
	#region ChartControlCommandGalleryItemGroup2DArea
	public class ChartControlCommandGalleryItemGroup2DArea : ChartControlCommandGalleryItemGroup {
		public ChartControlCommandGalleryItemGroup2DArea()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.Area2DGroupPlaceHolder; } }
	}
	#endregion
	#region ChartControlCommandGalleryItemGroup3DArea
	public class ChartControlCommandGalleryItemGroup3DArea : ChartControlCommandGalleryItemGroup {
		public ChartControlCommandGalleryItemGroup3DArea()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.Area3DGroupPlaceHolder; } }
	}
	#endregion
	#region ChartControlCommandGalleryItemGroupPoint
	public class ChartControlCommandGalleryItemGroupPoint : ChartControlCommandGalleryItemGroup {
		public ChartControlCommandGalleryItemGroupPoint()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.PointGroupPlaceHolder; } }
	}
	#endregion
	#region ChartControlCommandGalleryItemGroupFunnel
	public class ChartControlCommandGalleryItemGroupFunnel : ChartControlCommandGalleryItemGroup {
		public ChartControlCommandGalleryItemGroupFunnel()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.FunnelGroupPlaceHolder; } }
	}
	#endregion
	#region ChartControlCommandGalleryItemGroupFinancial
	public class ChartControlCommandGalleryItemGroupFinancial : ChartControlCommandGalleryItemGroup {
		public ChartControlCommandGalleryItemGroupFinancial()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.FinancialGroupPlaceHolder; } }
	}
	#endregion
	#region ChartControlCommandGalleryItemGroupRadar
	public class ChartControlCommandGalleryItemGroupRadar : ChartControlCommandGalleryItemGroup {
		public ChartControlCommandGalleryItemGroupRadar()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.RadarGroupPlaceHolder; } }
	}
	#endregion
	#region ChartControlCommandGalleryItemGroupPolar
	public class ChartControlCommandGalleryItemGroupPolar : ChartControlCommandGalleryItemGroup {
		public ChartControlCommandGalleryItemGroupPolar()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.PolarGroupPlaceHolder; } }
	}
	#endregion
	#region ChartControlCommandGalleryItemGroupRange
	public class ChartControlCommandGalleryItemGroupRange : ChartControlCommandGalleryItemGroup {
		public ChartControlCommandGalleryItemGroupRange()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.RangeGroupPlaceHolder; } }
	}
	#endregion
	#region ChartControlCommandGalleryItemGroupGantt
	public class ChartControlCommandGalleryItemGroupGantt : ChartControlCommandGalleryItemGroup {
		public ChartControlCommandGalleryItemGroupGantt()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.GanttGroupPlaceHolder; } }
	}
	#endregion
	#region CreateBarBaseItem
	public class CreateBarBaseItem : ChartCommandDropDownGalleryBarItem {
		static readonly int galleryColumnCount = 4;
		static readonly int galleryRowCount = 10;
		public CreateBarBaseItem()
			: base() {
		}
		public CreateBarBaseItem(BarManager manager)
			: base(manager) {
		}
		public CreateBarBaseItem(string caption)
			: base(caption) {
		}
		public CreateBarBaseItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateBarChartPlaceHolder; } }
		protected override int GalleryColumnCount { get { return galleryColumnCount; } }
		protected override int GalleryRowCount { get { return galleryRowCount; } }
	}
	#endregion
	#region CreateBarChartItem
	public class CreateBarChartItem : ChartCommandGalleryItem {
		public CreateBarChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateBarChart; } }
	}
	#endregion
	#region CreateBar3DChartItem
	public class CreateBar3DChartItem : ChartCommandGalleryItem {
		public CreateBar3DChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateBar3DChart; } }
	}
	#endregion
	#region CreateFullStackedBarChartItem
	public class CreateFullStackedBarChartItem : ChartCommandGalleryItem {
		public CreateFullStackedBarChartItem() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateFullStackedBarChart; } }
	}
	#endregion
	#region CreateFullStackedBar3DChartItem
	public class CreateFullStackedBar3DChartItem : ChartCommandGalleryItem {
		public CreateFullStackedBar3DChartItem() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateFullStackedBar3DChart; } }
	}
	#endregion
	#region CreateManhattanBarChartItem
	public class CreateManhattanBarChartItem : ChartCommandGalleryItem {
		public CreateManhattanBarChartItem() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateManhattanBarChart; } }
	}
	#endregion
	#region CreateSideBySideFullStackedBarChartItem
	public class CreateSideBySideFullStackedBarChartItem : ChartCommandGalleryItem {
		public CreateSideBySideFullStackedBarChartItem() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateSideBySideFullStackedBarChart; } }
	}
	#endregion
	#region CreateSideBySideFullStackedBar3DChartItem
	public class CreateSideBySideFullStackedBar3DChartItem : ChartCommandGalleryItem {
		public CreateSideBySideFullStackedBar3DChartItem() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateSideBySideFullStackedBar3DChart; } }
	}
	#endregion
	#region CreateSideBySideStackedBarChartItem
	public class CreateSideBySideStackedBarChartItem : ChartCommandGalleryItem {
		public CreateSideBySideStackedBarChartItem() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateSideBySideStackedBarChart; } }
	}
	#endregion
	#region CreateSideBySideStackedBar3DChartItem
	public class CreateSideBySideStackedBar3DChartItem : ChartCommandGalleryItem {
		public CreateSideBySideStackedBar3DChartItem() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateSideBySideStackedBar3DChart; } }
	}
	#endregion
	#region CreateStackedBarChartItem
	public class CreateStackedBarChartItem : ChartCommandGalleryItem {
		public CreateStackedBarChartItem() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateStackedBarChart; } }
	}
	#endregion
	#region CreateStackedBar3DChartItem
	public class CreateStackedBar3DChartItem : ChartCommandGalleryItem {
		public CreateStackedBar3DChartItem() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateStackedBar3DChart; } }
	}
	#endregion
	#region CreateConeBar3DChartItem
	public class CreateConeBar3DChartItem : ChartCommandGalleryItem {
		public CreateConeBar3DChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateConeBar3DChart; } }
	}
	#endregion
	#region CreateConeFullStackedBar3DChartItem
	public class CreateConeFullStackedBar3DChartItem : ChartCommandGalleryItem {
		public CreateConeFullStackedBar3DChartItem() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateConeFullStackedBar3DChart; } }
	}
	#endregion
	#region CreateConeManhattanBarChartItem
	public class CreateConeManhattanBarChartItem : ChartCommandGalleryItem {
		public CreateConeManhattanBarChartItem() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateConeManhattanBarChart; } }
	}
	#endregion
	#region CreateConeSideBySideFullStackedBar3DChartItem
	public class CreateConeSideBySideFullStackedBar3DChartItem : ChartCommandGalleryItem {
		public CreateConeSideBySideFullStackedBar3DChartItem() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateConeSideBySideFullStackedBar3DChart; } }
	}
	#endregion
	#region CreateConeSideBySideStackedBar3DChartItem
	public class CreateConeSideBySideStackedBar3DChartItem : ChartCommandGalleryItem {
		public CreateConeSideBySideStackedBar3DChartItem() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateConeSideBySideStackedBar3DChart; } }
	}
	#endregion
	#region CreateConeStackedBar3DChartItem
	public class CreateConeStackedBar3DChartItem : ChartCommandGalleryItem {
		public CreateConeStackedBar3DChartItem() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateConeStackedBar3DChart; } }
	}
	#endregion
	#region CreatePyramidBar3DChartItem
	public class CreatePyramidBar3DChartItem : ChartCommandGalleryItem {
		public CreatePyramidBar3DChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreatePyramidBar3DChart; } }
	}
	#endregion
	#region CreatePyramidFullStackedBar3DChartItem
	public class CreatePyramidFullStackedBar3DChartItem : ChartCommandGalleryItem {
		public CreatePyramidFullStackedBar3DChartItem() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreatePyramidFullStackedBar3DChart; } }
	}
	#endregion
	#region CreatePyramidManhattanBarChartItem
	public class CreatePyramidManhattanBarChartItem : ChartCommandGalleryItem {
		public CreatePyramidManhattanBarChartItem() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreatePyramidManhattanBarChart; } }
	}
	#endregion
	#region CreatePyramidSideBySideFullStackedBar3DChartItem
	public class CreatePyramidSideBySideFullStackedBar3DChartItem : ChartCommandGalleryItem {
		public CreatePyramidSideBySideFullStackedBar3DChartItem() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreatePyramidSideBySideFullStackedBar3DChart; } }
	}
	#endregion
	#region CreatePyramidSideBySideStackedBar3DChartItem
	public class CreatePyramidSideBySideStackedBar3DChartItem : ChartCommandGalleryItem {
		public CreatePyramidSideBySideStackedBar3DChartItem() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreatePyramidSideBySideStackedBar3DChart; } }
	}
	#endregion
	#region CreatePyramidStackedBar3DChartItem
	public class CreatePyramidStackedBar3DChartItem : ChartCommandGalleryItem {
		public CreatePyramidStackedBar3DChartItem() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreatePyramidStackedBar3DChart; } }
	}
	#endregion
	#region CreateCylinderBar3DChartItem
	public class CreateCylinderBar3DChartItem : ChartCommandGalleryItem {
		public CreateCylinderBar3DChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateCylinderBar3DChart; } }
	}
	#endregion
	#region CreateCylinderFullStackedBar3DChartItem
	public class CreateCylinderFullStackedBar3DChartItem : ChartCommandGalleryItem {
		public CreateCylinderFullStackedBar3DChartItem() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateCylinderFullStackedBar3DChart; } }
	}
	#endregion
	#region CreateCylinderManhattanBarChartItem
	public class CreateCylinderManhattanBarChartItem : ChartCommandGalleryItem {
		public CreateCylinderManhattanBarChartItem() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateCylinderManhattanBarChart; } }
	}
	#endregion
	#region CreateCylinderSideBySideFullStackedBar3DChartItem
	public class CreateCylinderSideBySideFullStackedBar3DChartItem : ChartCommandGalleryItem {
		public CreateCylinderSideBySideFullStackedBar3DChartItem() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateCylinderSideBySideFullStackedBar3DChart; } }
	}
	#endregion
	#region CreateCylinderSideBySideStackedBar3DChartItem
	public class CreateCylinderSideBySideStackedBar3DChartItem : ChartCommandGalleryItem {
		public CreateCylinderSideBySideStackedBar3DChartItem() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateCylinderSideBySideStackedBar3DChart; } }
	}
	#endregion
	#region CreateCylinderStackedBar3DChartItem
	public class CreateCylinderStackedBar3DChartItem : ChartCommandGalleryItem {
		public CreateCylinderStackedBar3DChartItem() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateCylinderStackedBar3DChart; } }
	}
	#endregion
	#region CreatePieBaseItem
	public class CreatePieBaseItem : ChartCommandDropDownGalleryBarItem {
		static readonly int galleryColumnCount = 3;
		static readonly int galleryRowCount = 2;
		public CreatePieBaseItem()
			: base() {
		}
		public CreatePieBaseItem(BarManager manager)
			: base(manager) {
		}
		public CreatePieBaseItem(string caption)
			: base(caption) {
		}
		public CreatePieBaseItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreatePieChartPlaceHolder; } }
		protected override int GalleryColumnCount { get { return galleryColumnCount; } }
		protected override int GalleryRowCount { get { return galleryRowCount; } }
	}
	#endregion
	#region CreatePieChartItem
	public class CreatePieChartItem : ChartCommandGalleryItem {
		public CreatePieChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreatePieChart; } }
	}
	#endregion
	#region CreatePie3DChartItem
	public class CreatePie3DChartItem : ChartCommandGalleryItem {
		public CreatePie3DChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreatePie3DChart; } }
	}
	#endregion
	#region CreateDoughnutChartItem
	public class CreateDoughnutChartItem : ChartCommandGalleryItem {
		public CreateDoughnutChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateDoughnutChart; } }
	}
	#endregion
	#region CreateNestedDoughnutChartItem
	public class CreateNestedDoughnutChartItem : ChartCommandGalleryItem {
		public CreateNestedDoughnutChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateNestedDoughnutChart; } }
	}
	#endregion
	#region CreateDoughnut3DChartItem
	public class CreateDoughnut3DChartItem : ChartCommandGalleryItem {
		public CreateDoughnut3DChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateDoughnut3DChart; } }
	}
	#endregion
	#region CreateAreaBaseItem
	public class CreateAreaBaseItem : ChartCommandDropDownGalleryBarItem {
		static readonly int galleryColumnCount = 4;
		static readonly int galleryRowCount = 4;
		public CreateAreaBaseItem()
			: base() {
		}
		public CreateAreaBaseItem(BarManager manager)
			: base(manager) {
		}
		public CreateAreaBaseItem(string caption)
			: base(caption) {
		}
		public CreateAreaBaseItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateAreaChartPlaceHolder; } }
		protected override int GalleryColumnCount { get { return galleryColumnCount; } }
		protected override int GalleryRowCount { get { return galleryRowCount; } }
	}
	#endregion
	#region CreateAreaChartItem
	public class CreateAreaChartItem : ChartCommandGalleryItem {
		public CreateAreaChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateAreaChart; } }
	}
	#endregion
	#region CreateArea3DChartItem
	public class CreateArea3DChartItem : ChartCommandGalleryItem {
		public CreateArea3DChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateArea3DChart; } }
	}
	#endregion
	#region CreateFullStackedAreaChartItem
	public class CreateFullStackedAreaChartItem : ChartCommandGalleryItem {
		public CreateFullStackedAreaChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateFullStackedAreaChart; } }
	}
	#endregion
	#region CreateFullStackedArea3DChartItem
	public class CreateFullStackedArea3DChartItem : ChartCommandGalleryItem {
		public CreateFullStackedArea3DChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateFullStackedArea3DChart; } }
	}
	#endregion
	#region CreateFullStackedSplineAreaChartItem
	public class CreateFullStackedSplineAreaChartItem : ChartCommandGalleryItem {
		public CreateFullStackedSplineAreaChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateFullStackedSplineAreaChart; } }
	}
	#endregion
	#region CreateFullStackedSplineArea3DChartItem
	public class CreateFullStackedSplineArea3DChartItem : ChartCommandGalleryItem {
		public CreateFullStackedSplineArea3DChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateFullStackedSplineArea3DChart; } }
	}
	#endregion
	#region CreateSplineAreaChartItem
	public class CreateSplineAreaChartItem : ChartCommandGalleryItem {
		public CreateSplineAreaChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateSplineAreaChart; } }
	}
	#endregion
	#region CreateSplineArea3DChartItem
	public class CreateSplineArea3DChartItem : ChartCommandGalleryItem {
		public CreateSplineArea3DChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateSplineArea3DChart; } }
	}
	#endregion
	#region CreateStackedAreaChartItem
	public class CreateStackedAreaChartItem : ChartCommandGalleryItem {
		public CreateStackedAreaChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateStackedAreaChart; } }
	}
	#endregion
	#region CreateStackedArea3DChartItem
	public class CreateStackedArea3DChartItem : ChartCommandGalleryItem {
		public CreateStackedArea3DChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateStackedArea3DChart; } }
	}
	#endregion
	#region CreateStackedSplineAreaChartItem
	public class CreateStackedSplineAreaChartItem : ChartCommandGalleryItem {
		public CreateStackedSplineAreaChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateStackedSplineAreaChart; } }
	}
	#endregion
	#region CreateStackedSplineArea3DChartItem
	public class CreateStackedSplineArea3DChartItem : ChartCommandGalleryItem {
		public CreateStackedSplineArea3DChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateStackedSplineArea3DChart; } }
	}
	#endregion
	#region CreateStepAreaChartItem
	public class CreateStepAreaChartItem : ChartCommandGalleryItem {
		public CreateStepAreaChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateStepAreaChart; } }
	}
	#endregion
	#region CreateStepArea3DChartItem
	public class CreateStepArea3DChartItem : ChartCommandGalleryItem {
		public CreateStepArea3DChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateStepArea3DChart; } }
	}
	#endregion
	#region CreateLineBaseItem
	public class CreateLineBaseItem : ChartCommandDropDownGalleryBarItem {
		static readonly int galleryColumnCount = 3;
		static readonly int galleryRowCount = 4;
		public CreateLineBaseItem()
			: base() {
		}
		public CreateLineBaseItem(BarManager manager)
			: base(manager) {
		}
		public CreateLineBaseItem(string caption)
			: base(caption) {
		}
		public CreateLineBaseItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateLineChartPlaceHolder; } }
		protected override int GalleryColumnCount { get { return galleryColumnCount; } }
		protected override int GalleryRowCount { get { return galleryRowCount; } }
	}
	#endregion
	#region CreateLineChartItem
	public class CreateLineChartItem : ChartCommandGalleryItem {
		public CreateLineChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateLineChart; } }
	}
	#endregion
	#region CreateLine3DChartItem
	public class CreateLine3DChartItem : ChartCommandGalleryItem {
		public CreateLine3DChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateLine3DChart; } }
	}
	#endregion
	#region CreateFullStackedLineChartItem
	public class CreateFullStackedLineChartItem : ChartCommandGalleryItem {
		public CreateFullStackedLineChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateFullStackedLineChart; } }
	}
	#endregion
	#region CreateFullStackedLine3DChartItem
	public class CreateFullStackedLine3DChartItem : ChartCommandGalleryItem {
		public CreateFullStackedLine3DChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateFullStackedLine3DChart; } }
	}
	#endregion
	#region CreateScatterLineChartItem
	public class CreateScatterLineChartItem : ChartCommandGalleryItem {
		public CreateScatterLineChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateScatterLineChart; } }
	}
	#endregion
	#region CreateSplineChartItem
	public class CreateSplineChartItem : ChartCommandGalleryItem {
		public CreateSplineChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateSplineChart; } }
	}
	#endregion
	#region CreateSpline3DChartItem
	public class CreateSpline3DChartItem : ChartCommandGalleryItem {
		public CreateSpline3DChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateSpline3DChart; } }
	}
	#endregion
	#region CreateStackedLineChartItem
	public class CreateStackedLineChartItem : ChartCommandGalleryItem {
		public CreateStackedLineChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateStackedLineChart; } }
	}
	#endregion
	#region CreateStackedLine3DChartItem
	public class CreateStackedLine3DChartItem : ChartCommandGalleryItem {
		public CreateStackedLine3DChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateStackedLine3DChart; } }
	}
	#endregion
	#region CreateStepLineChartItem
	public class CreateStepLineChartItem : ChartCommandGalleryItem {
		public CreateStepLineChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateStepLineChart; } }
	}
	#endregion
	#region CreateStepLine3DChartItem
	public class CreateStepLine3DChartItem : ChartCommandGalleryItem {
		public CreateStepLine3DChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateStepLine3DChart; } }
	}
	#endregion
	#region CreateOtherSeriesTypesBaseItem
	public class CreateOtherSeriesTypesBaseItem : ChartCommandDropDownGalleryBarItem {
		static readonly int galleryColumnCount = 4;
		static readonly int galleryRowCount = 7;
		public CreateOtherSeriesTypesBaseItem()
			: base() {
		}
		public CreateOtherSeriesTypesBaseItem(BarManager manager)
			: base(manager) {
		}
		public CreateOtherSeriesTypesBaseItem(string caption)
			: base(caption) {
		}
		public CreateOtherSeriesTypesBaseItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateOtherSeriesTypesChartPlaceHolder; } }
		protected override int GalleryColumnCount { get { return galleryColumnCount; } }
		protected override int GalleryRowCount { get { return galleryRowCount; } }
	}
	#endregion
	#region CreatePointChartItem
	public class CreatePointChartItem : ChartCommandGalleryItem {
		public CreatePointChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreatePointChart; } }
	}
	#endregion
	#region CreateBubbleChartItem
	public class CreateBubbleChartItem : ChartCommandGalleryItem {
		public CreateBubbleChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateBubbleChart; } }
	}
	#endregion
	#region CreateFunnelChartItem
	public class CreateFunnelChartItem : ChartCommandGalleryItem {
		public CreateFunnelChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateFunnelChart; } }
	}
	#endregion
	#region CreateFunnel3DChartItem
	public class CreateFunnel3DChartItem : ChartCommandGalleryItem {
		public CreateFunnel3DChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateFunnel3DChart; } }
	}
	#endregion
	#region CreateRangeBarChartItem
	public class CreateRangeBarChartItem : ChartCommandGalleryItem {
		public CreateRangeBarChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateRangeBarChart; } }
	}
	#endregion
	#region CreateSideBySideRangeBarChartItem
	public class CreateSideBySideRangeBarChartItem : ChartCommandGalleryItem {
		public CreateSideBySideRangeBarChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateSideBySideRangeBarChart; } }
	}
	#endregion
	#region CreateRangeAreaChartItem
	public class CreateRangeAreaChartItem : ChartCommandGalleryItem {
		public CreateRangeAreaChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateRangeAreaChart; } }
	}
	#endregion
	#region CreateRangeArea3DChartItem
	public class CreateRangeArea3DChartItem : ChartCommandGalleryItem {
		public CreateRangeArea3DChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateRangeArea3DChart; } }
	}
	#endregion
	#region CreateRadarPointChartItem
	public class CreateRadarPointChartItem : ChartCommandGalleryItem {
		public CreateRadarPointChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateRadarPointChart; } }
	}
	#endregion
	#region CreateRadarLineChartItem
	public class CreateRadarLineChartItem : ChartCommandGalleryItem {
		public CreateRadarLineChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateRadarLineChart; } }
	}
	#endregion
	#region CreateScatterRadarLineChartItem
	public class CreateScatterRadarLineChartItem : ChartCommandGalleryItem {
		public CreateScatterRadarLineChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateScatterRadarLineChart; } }
	}
	#endregion
	#region CreateRadarAreaChartItem
	public class CreateRadarAreaChartItem : ChartCommandGalleryItem {
		public CreateRadarAreaChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateRadarAreaChart; } }
	}
	#endregion
	#region CreatePolarPointChartItem
	public class CreatePolarPointChartItem : ChartCommandGalleryItem {
		public CreatePolarPointChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreatePolarPointChart; } }
	}
	#endregion
	#region CreatePolarLineChartItem
	public class CreatePolarLineChartItem : ChartCommandGalleryItem {
		public CreatePolarLineChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreatePolarLineChart; } }
	}
	#endregion
	#region CreateScatterPolarLineChartItem
	public class CreateScatterPolarLineChartItem : ChartCommandGalleryItem {
		public CreateScatterPolarLineChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateScatterPolarLineChart; } }
	}
	#endregion
	#region CreatePolarAreaChartItem
	public class CreatePolarAreaChartItem : ChartCommandGalleryItem {
		public CreatePolarAreaChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreatePolarAreaChart; } }
	}
	#endregion
	#region CreateStockChartItem
	public class CreateStockChartItem : ChartCommandGalleryItem {
		public CreateStockChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateStockChart; } }
	}
	#endregion
	#region CreateCandleStickChartItem
	public class CreateCandleStickChartItem : ChartCommandGalleryItem {
		public CreateCandleStickChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateCandleStickChart; } }
	}
	#endregion
	#region CreateGanttChartItem
	public class CreateGanttChartItem : ChartCommandGalleryItem {
		public CreateGanttChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateGanttChart; } }
	}
	#endregion
	#region CreateSideBySideGanttChartItem
	public class CreateSideBySideGanttChartItem : ChartCommandGalleryItem {
		public CreateSideBySideGanttChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateSideBySideGanttChart; } }
	}
	#endregion
	#region CreateRotatedBarBaseItem
	public class CreateRotatedBarBaseItem : ChartCommandDropDownGalleryBarItem {
		static readonly int galleryColumnCount = 3;
		static readonly int galleryRowCount = 2;
		public CreateRotatedBarBaseItem()
			: base() {
		}
		public CreateRotatedBarBaseItem(BarManager manager)
			: base(manager) {
		}
		public CreateRotatedBarBaseItem(string caption)
			: base(caption) {
		}
		public CreateRotatedBarBaseItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateRotatedBarChartPlaceHolder; } }
		protected override int GalleryColumnCount { get { return galleryColumnCount; } }
		protected override int GalleryRowCount { get { return galleryRowCount; } }
	}
	#endregion
	#region CreateRotatedBarChartItem
	public class CreateRotatedBarChartItem : ChartCommandGalleryItem {
		public CreateRotatedBarChartItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateRotatedBarChart; } }
	}
	#endregion
	#region CreateRotatedFullStackedBarChartItem
	public class CreateRotatedFullStackedBarChartItem : ChartCommandGalleryItem {
		public CreateRotatedFullStackedBarChartItem() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateRotatedFullStackedBarChart; } }
	}
	#endregion
	#region CreateRotatedSideBySideFullStackedBarChartItem
	public class CreateRotatedSideBySideFullStackedBarChartItem : ChartCommandGalleryItem {
		public CreateRotatedSideBySideFullStackedBarChartItem() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateRotatedSideBySideFullStackedBarChart; } }
	}
	#endregion
	#region CreateRotatedSideBySideStackedBarChartItem
	public class CreateRotatedSideBySideStackedBarChartItem : ChartCommandGalleryItem {
		public CreateRotatedSideBySideStackedBarChartItem() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateRotatedSideBySideStackedBarChart; } }
	}
	#endregion
	#region CreateRotatedStackedBarChartItem
	public class CreateRotatedStackedBarChartItem : ChartCommandGalleryItem {
		public CreateRotatedStackedBarChartItem() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.CreateRotatedStackedBarChart; } }
	}
	#endregion
	#region ChangeAppearanceGalleryBaseItem
	public class ChangeAppearanceGalleryBaseItem : ChartCommandGalleryBarItem {
		static readonly int galleryColumnCount = 7;
		static readonly int galleryRowCount = 4;
		protected override ChartCommandId CommandId { get { return ChartCommandId.ChangeAppearance; } }
		protected override Size GalleryImageSize { get { return new Size(80, 50); } }
		public ChangeAppearanceGalleryBaseItem()
			: base() {
			Gallery.ColumnCount = galleryColumnCount;
			Gallery.RowCount = galleryRowCount;
		}
		ICommandUIState CreateCommandUIState(Command command) {
			DefaultValueBasedCommandUIState<ChartAppearanceInfo> state = new DefaultValueBasedCommandUIState<ChartAppearanceInfo>();
			state.Value = SelectedItem.Tag as ChartAppearanceInfo;
			return state;
		}
		void PopulateGalleryItems() {
			if (DesignMode || Control == null)
				return;
			Gallery.BeginUpdate();
			if (Gallery.Groups.Count == 0)
				Gallery.Groups.Add(new GalleryItemGroup());
			GalleryItemCollection galleryItems = Gallery.Groups[0].Items;
			galleryItems.Clear();
			Chart chart = Control.Chart;
			Palette currentPalette = chart.PaletteRepository[Control.Chart.PaletteName];
			ChartAppearanceInfo currentApperarnceInfo = new ChartAppearanceInfo(chart.AppearanceRepository[chart.AppearanceName], chart.PaletteBaseColorNumber);
			ViewType viewType = chart.Series.Count == 0 ? ViewType.Bar : SeriesViewFactory.GetViewType(chart.Series[0].View);
			bool isBar3D = (chart.Series.Count != 0 && chart.Series[0].View is Bar3DSeriesView);
			foreach(ChartAppearance appearance in chart.AppearanceRepository) {
				if (!currentPalette.Predefined || appearance == chart.AppearanceRepository[chart.AppearanceName] || String.IsNullOrEmpty(appearance.PaletteName) || appearance.PaletteName == currentPalette.Name)
					for (int i = 0; i <= currentPalette.Count; i++) {
						GalleryItem galleryItem = new GalleryItem();
						if (isBar3D)
							galleryItem.Image = AppearanceImageHelper.CreateImage(viewType, appearance, currentPalette, i, (chart.Series[0].View as Bar3DSeriesView).Model);
						else
							galleryItem.Image = AppearanceImageHelper.CreateImage(viewType, appearance, currentPalette, i);
						ChartAppearanceInfo apperarnceInfo = new ChartAppearanceInfo(appearance, i);
						galleryItem.Tag = apperarnceInfo;
						galleryItems.Add(galleryItem);
						if (currentApperarnceInfo.Equals(apperarnceInfo)) {
							galleryItem.Checked = true;
							Gallery.MakeVisible(galleryItem);
						}
					}
			}
			Gallery.EndUpdate();
		}
		protected override void InvokeCommand() {
			Command command = CreateCommand();
			if (command != null) {
				ICommandUIState state = CreateCommandUIState(command);
				if (command.CanExecute())
					command.ForceExecute(state);
			}
		}
		protected override void OnControlChanged() {
			base.OnControlChanged();
			PopulateGalleryItems();
		}
		protected override void OnControlUpdateUI(object sender, EventArgs e) {
			base.OnControlUpdateUI(sender, e);
			PopulateGalleryItems();
		}
	}
	#endregion
	#region ChangePaletteGalleryBaseItem
	public class ChangePaletteGalleryBaseItem : ChartCommandDropDownGalleryBarItem {
		const int PaletteImageSize = 10;
		static readonly int galleryColumnCount = 1;
		static readonly int galleryRowCount = 10;
		static readonly int vertPadding = -3;
		protected override ChartCommandId CommandId { get { return ChartCommandId.ChangePalettePlaceHolder; } }
		protected override System.Drawing.Size GalleryImageSize { get { return new System.Drawing.Size(160, 10); } }
		protected override int GalleryColumnCount { get { return galleryColumnCount; } }
		protected override int GalleryRowCount { get { return galleryRowCount; } }
		protected override bool ShowGroupCaption { get { return false; } }
		protected override bool ShowItemText { get { return true; } }
		public ChangePaletteGalleryBaseItem()
			: base() {
		}
		public ChangePaletteGalleryBaseItem(BarManager manager)
			: base(manager) {
		}
		public ChangePaletteGalleryBaseItem(string caption)
			: base(caption) {
		}
		public ChangePaletteGalleryBaseItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		void PopulateGalleryItems() {
			if (DesignMode || Control == null)
				return;
			InDropDownGallery gallery = GalleryDropDown.Gallery;
			gallery.BeginUpdate();
			if (gallery.Groups.Count == 0)
				gallery.Groups.Add(new GalleryItemGroup());
			GalleryItemCollection galleryItems = gallery.Groups[0].Items;
			foreach (GalleryItem item in galleryItems) {
				ChangePaletteGalleryItem changePaletteGalleryItem = item as ChangePaletteGalleryItem;
				changePaletteGalleryItem.UnsubscribeEvents();
				changePaletteGalleryItem.Dispose();
			}
			galleryItems.Clear();
			Chart chart = Control.Chart;
			foreach (string paletteName in chart.PaletteRepository.PaletteNames) {
				Palette palette = chart.PaletteRepository[paletteName];
				ChangePaletteGalleryItem galleryItem = new ChangePaletteGalleryItem();
				galleryItem.Tag = palette.Name;
				galleryItem.Caption = palette.DisplayName;
				galleryItem.Image = CreatePaletteImage(palette);
				galleryItems.Add(galleryItem);
				galleryItem.Checked = chart.PaletteName == palette.Name;
				galleryItem.Control = Control;
			}
			gallery.EndUpdate();
		}
		Image CreatePaletteImage(Palette palette){
			Bitmap image = new Bitmap(palette.Count * (PaletteImageSize + 1) - 1, PaletteImageSize);
			using (Graphics g = Graphics.FromImage(image)) {
				Rectangle rect = new Rectangle(Point.Empty, new Size(PaletteImageSize, PaletteImageSize));
				for (int i = 0; i < palette.Count; i++, rect.X += PaletteImageSize + 1) {
					using (Brush brush = new SolidBrush(palette[i].Color))
						g.FillRectangle(brush, rect);
					Rectangle penRect = rect;
					penRect.Width--;
					penRect.Height--;
					using (Pen pen = new Pen(Color.Gray))
						g.DrawRectangle(pen, penRect);
				}
			}
			return image;
		}
		protected override void OnControlChanged() {
			base.OnControlChanged();
			PopulateGalleryItems();
		}
		protected override void OnControlUpdateUI(object sender, EventArgs e) {
			base.OnControlUpdateUI(sender, e);
			PopulateGalleryItems();
		}
		protected override void InitializeDropDownGallery(DevExpress.XtraBars.Ribbon.Gallery.InDropDownGallery gallery) {
			gallery.ItemImageLocation = DevExpress.Utils.Locations.Right;
			gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.MiddleLeft;
			gallery.Appearance.ItemCaptionAppearance.Normal.Options.UseTextOptions = true;
			gallery.Appearance.ItemCaptionAppearance.Normal.Options.UseFont = true;
			gallery.Appearance.ItemCaptionAppearance.Normal.Font = new Font(gallery.Appearance.ItemCaptionAppearance.Normal.Font, FontStyle.Regular);
			gallery.Appearance.ItemCaptionAppearance.Normal.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
			gallery.Appearance.ItemDescriptionAppearance.Normal.Options.UseTextOptions = true;
			gallery.Appearance.ItemDescriptionAppearance.Normal.Options.UseFont = true;
			gallery.Appearance.ItemDescriptionAppearance.Normal.Font = new Font(gallery.Appearance.ItemDescriptionAppearance.Normal.Font, FontStyle.Regular);
			gallery.Appearance.ItemDescriptionAppearance.Normal.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
			gallery.Appearance.ItemCaptionAppearance.Hovered.Options.UseTextOptions = true;
			gallery.Appearance.ItemCaptionAppearance.Hovered.Options.UseFont = true;
			gallery.Appearance.ItemCaptionAppearance.Hovered.Font = new Font(gallery.Appearance.ItemCaptionAppearance.Hovered.Font, FontStyle.Regular);
			gallery.Appearance.ItemCaptionAppearance.Hovered.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
			gallery.Appearance.ItemDescriptionAppearance.Hovered.Options.UseTextOptions = true;
			gallery.Appearance.ItemDescriptionAppearance.Hovered.Options.UseFont = true;
			gallery.Appearance.ItemDescriptionAppearance.Hovered.Font = new Font(gallery.Appearance.ItemDescriptionAppearance.Hovered.Font, FontStyle.Regular);
			gallery.Appearance.ItemDescriptionAppearance.Hovered.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
			gallery.Appearance.ItemCaptionAppearance.Pressed.Options.UseTextOptions = true;
			gallery.Appearance.ItemCaptionAppearance.Pressed.Options.UseFont = true;
			gallery.Appearance.ItemCaptionAppearance.Pressed.Font = new Font(gallery.Appearance.ItemCaptionAppearance.Pressed.Font, FontStyle.Regular);
			gallery.Appearance.ItemCaptionAppearance.Pressed.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
			gallery.Appearance.ItemDescriptionAppearance.Pressed.Options.UseTextOptions = true;
			gallery.Appearance.ItemDescriptionAppearance.Pressed.Options.UseFont = true;
			gallery.Appearance.ItemDescriptionAppearance.Pressed.Font = new Font(gallery.Appearance.ItemDescriptionAppearance.Pressed.Font, FontStyle.Regular);
			gallery.Appearance.ItemDescriptionAppearance.Pressed.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
			DevExpress.Skins.SkinPaddingEdges currentImagePadding = gallery.ItemImagePadding;
			DevExpress.Skins.SkinPaddingEdges currentTextPadding = gallery.ItemTextPadding;
			gallery.ItemImagePadding = new DevExpress.Skins.SkinPaddingEdges(currentImagePadding.Left, vertPadding, currentImagePadding.Right, vertPadding);
			gallery.ItemTextPadding = new DevExpress.Skins.SkinPaddingEdges(currentTextPadding.Left, vertPadding, currentTextPadding.Right, vertPadding);
			base.InitializeDropDownGallery(gallery);
		}
	}
	#endregion
	#region ChangePaletteGalleryItem
	public class ChangePaletteGalleryItem : ChartCommandGalleryItem {
		public ChangePaletteGalleryItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.ChangePalette; } }
		ICommandUIState CreatePaletteCommandUIState(Command command) {
			string paletteName = Tag != null ? Tag as string : null;
			if (paletteName == null)
				paletteName = Palettes.Office.Name;
			DefaultValueBasedCommandUIState<string> state = new DefaultValueBasedCommandUIState<string>();
			state.Value = paletteName;
			return state;
		}
		protected override void InvokeCommand() {
			Command command = CreateCommand();
			if (command != null) {
				ICommandUIState state = CreatePaletteCommandUIState(command);
				if (command.CanExecute())
					command.ForceExecute(state);
			}
		}
		protected override SuperToolTip GetSuperTip() {
			return null;
		}
		internal void UnsubscribeEvents() {
			if (Control != null)
				UnsubscribeControlEvents();
		}
	}
	#endregion
	#region ChartTypesBarCreator
	public class ChartTypesBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(CreateChartRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(ChartTypeRibbonPageGroup); } }
		public override Type SupportedRibbonPageCategoryType { get { return typeof(ChartRibbonPageCategory); } }
		public override Type SupportedBarType { get { return typeof(ChartTypeBar); } }
		public override int DockRow { get { return 0; } }
		public override int DockColumn { get { return 0; } }
		public override Bar CreateBar() {
			return new ChartTypeBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new ChartTypesItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new CreateChartRibbonPage();
		}
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new ChartRibbonPageCategory();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new ChartTypeRibbonPageGroup();
		}
	}
	#endregion
	#region ChartAppearanceBarCreator
	public class ChartAppearanceBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(CreateChartRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(ChartAppearanceRibbonPageGroup); } }
		public override Type SupportedRibbonPageCategoryType { get { return typeof(ChartRibbonPageCategory); } }
		public override Type SupportedBarType { get { return typeof(ChartAppearanceBar); } }
		public override int DockRow { get { return 0; } }
		public override int DockColumn { get { return 1; } }
		public override Bar CreateBar() {
			return new ChartAppearanceBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new ChartAppearanceItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new CreateChartRibbonPage();
		}
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new ChartRibbonPageCategory();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new ChartAppearanceRibbonPageGroup();
		}
	}
	#endregion
	#region ChartTypeBar
	public class ChartTypeBar : ControlCommandBasedBar<IChartContainer, ChartCommandId> {
		public ChartTypeBar() {
		}
		public ChartTypeBar(BarManager manager)
			: base(manager) {
		}
		public ChartTypeBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return ChartLocalizer.GetString(ChartStringId.RibbonTypesGroupCaption); } }
	}
	#endregion
	#region ChartAppearanceBar
	public class ChartAppearanceBar : ControlCommandBasedBar<IChartContainer, ChartCommandId> {
		public ChartAppearanceBar() {
		}
		public ChartAppearanceBar(BarManager manager)
			: base(manager) {
		}
		public ChartAppearanceBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return ChartLocalizer.GetString(ChartStringId.RibbonAppearanceGroupCaption); } }
	}
	#endregion
	#region CreateChartRibbonPage
	public class CreateChartRibbonPage : ControlCommandBasedRibbonPage {
		public CreateChartRibbonPage() {
		}
		public CreateChartRibbonPage(string text)
			: base(text) {
		}
		public override string DefaultText { get { return ChartLocalizer.GetString(ChartStringId.RibbonDesignPageCaption); } }
	}
	#endregion
	#region ChartTypeRibbonPageGroup
	public class ChartTypeRibbonPageGroup : ChartRibbonPageGroup {
		public ChartTypeRibbonPageGroup() {
		}
		public ChartTypeRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return ChartLocalizer.GetString(ChartStringId.RibbonTypesGroupCaption); } }
	}
	#endregion
	#region ChartAppearanceRibbonPageGroup
	public class ChartAppearanceRibbonPageGroup : ChartRibbonPageGroup {
		public ChartAppearanceRibbonPageGroup() {
		}
		public ChartAppearanceRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return ChartLocalizer.GetString(ChartStringId.RibbonAppearanceGroupCaption); } }
	}
	#endregion
	#region ChartRibbonPageCategory
	public class ChartRibbonPageCategory : ControlCommandBasedRibbonPageCategory<IChartContainer, ChartCommandId> {
		public override string DefaultText { get { return ChartLocalizer.GetString(ChartStringId.RibbonPageCategoryCaption); } }
		protected override ChartCommandId EmptyCommandId { get { return ChartCommandId.None; } }
		protected override RibbonPageCategory CreateRibbonPageCategory() {
			return new ChartRibbonPageCategory();
		}
	}
	#endregion
}
