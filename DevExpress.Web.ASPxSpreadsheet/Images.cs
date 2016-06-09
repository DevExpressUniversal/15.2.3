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
using System.ComponentModel;
using System.Web.UI;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxSpreadsheet {
	public class SpreadsheetImages : ImagesBase {
		protected internal const string TouchResizeImageName = "TouchResize";
		protected internal const string CellsGridImageName = "CellsGrid";
		SpreadsheetIconImages iconImages;
		SpreadsheetFormulaBarButtonImageProperties formulaBarEnterButtonImage;
		SpreadsheetFormulaBarButtonImageProperties formulaBarCancelButtonImage;
		public SpreadsheetImages(ISkinOwner owner)
			: base(owner) {
		}
		[
		Category("Images"), NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public SpreadsheetFormulaBarButtonImageProperties FormulaBarEnterButtonImage {
			get {
				if(formulaBarEnterButtonImage == null)
					formulaBarEnterButtonImage = new SpreadsheetFormulaBarButtonImageProperties(this);
				return formulaBarEnterButtonImage;
			}
		}
		[
		Category("Images"), NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public SpreadsheetFormulaBarButtonImageProperties FormulaBarCancelButtonImage {
			get {
				if(formulaBarCancelButtonImage == null)
					formulaBarCancelButtonImage = new SpreadsheetFormulaBarButtonImageProperties(this);
				return formulaBarCancelButtonImage;
			}
		}
		[Category("Images"), AutoFormatEnable, NotifyParentProperty(true), DefaultValue(MenuIconSetType.NotSet)]
		public MenuIconSetType MenuIconSet {
			get { return IconImages.MenuIconSet; }
			set {
				IconImages.MenuIconSet = value;
				Changed();
			}
		}
		protected SpreadsheetIconImages IconImages {
			get {
				if(iconImages == null)
					iconImages = new SpreadsheetIconImages((ISkinOwner)Owner);
				return iconImages;
			}
		}
		public override string ToString() {
			return string.Empty;
		}
		public override void CopyFrom(ImagesBase source) {
			base.CopyFrom(source);
			SpreadsheetImages spreadsheetImages = source as SpreadsheetImages;
			if(spreadsheetImages != null) {
				MenuIconSet = spreadsheetImages.MenuIconSet;
				FormulaBarEnterButtonImage.CopyFrom(spreadsheetImages.FormulaBarEnterButtonImage);
				FormulaBarCancelButtonImage.CopyFrom(spreadsheetImages.FormulaBarCancelButtonImage);
			}
		}
		public override void Reset() {
			base.Reset();
			MenuIconSet = MenuIconSetType.NotSet;
		}
		public override ImageProperties GetImageProperties(Page page, string imageName, bool encode) {
			if(InfoIndex.ContainsKey(imageName))
				return base.GetImageProperties(page, imageName, encode);
			return IconImages.GetImageProperties(page, imageName, encode);
		}
		protected override Type GetResourceType() {
			return typeof(ASPxSpreadsheet);
		}
		protected override string GetResourceImagePath() {
			return ASPxSpreadsheet.SpreadsheetImageResourcePath;
		}
		protected override string GetDesignTimeResourceSpriteImagePath() {
			return ASPxSpreadsheet.SpreadsheetImageResourcePath + GetDesignTimeResourceSpriteImageDefaultName();
		}
		protected override string GetResourceSpriteCssPath() {
			return ASPxSpreadsheet.SpreadsheetSpriteCssResourceName;
		}
		protected internal void RegisterIconSpriteCssFile(Page page) {
			IconImages.RegisterIconSpriteCssFile(page);
		}
		protected override bool KeepDefaultSizes {
			get { return true; }
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { IconImages, FormulaBarEnterButtonImage, FormulaBarCancelButtonImage });
		}
	}
	public class SpreadsheetFormulaBarButtonImageProperties : MenuItemImageProperties {
		public SpreadsheetFormulaBarButtonImageProperties() : base() { }
		public SpreadsheetFormulaBarButtonImageProperties(IPropertiesOwner owner) : base(owner) { }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageSpriteProperties SpriteProperties {
			get { return base.SpriteProperties; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), 
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string UrlChecked {
			get { return base.UrlChecked; }
			set { base.UrlChecked = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string UrlSelected {
			get { return base.UrlSelected; }
			set { base.UrlSelected = value; }
		}
	}
	public class SpreadsheetIconImages : ImagesBase {
		internal readonly static Dictionary<MenuIconSetType, string> Categories = new Dictionary<MenuIconSetType, string>();
		internal readonly static Dictionary<MenuIconSetType, string> SpriteCssResourceNames = new Dictionary<MenuIconSetType, string>();
		internal readonly static Dictionary<MenuIconSetType, string> SpriteImageResourceNames = new Dictionary<MenuIconSetType, string>();
		public const string IconSpriteImageName = "ISprite";
		public const string GrayScaleIconSpriteImageName = "GISprite";
		public const string GrayScaleWithWhiteHottrackIconSpriteImageName = "GWISprite";
		private const int DefaultSmallSize = 16;
		private const int DefaultLargeSize = 32;
		private const int DefaultPresetSize = 48;
		#region ImageNames
		protected internal const string
								Copy = "Copy",
								Cut = "Cut",
								Delete_Hyperlink = "Delete_Hyperlink",
								Hyperlink = "Hyperlink",
								OpenHyperlink = "OpenHyperlink",
								Paste = "Paste",
								ChartGroupColumn = "ChartGroupColumn",
								ChartSelectData = "ChartSelectData",
								FloatingObjectBringForward = "FloatingObjectBringForward",
								FloatingObjectBringToFront = "FloatingObjectBringToFront",
								FloatingObjectSendBackward = "FloatingObjectSendBackward",
								FloatingObjectSendToBack = "FloatingObjectSendToBack",
								ChartGroupArea = "ChartGroupArea",
								ChartGroupBar = "ChartGroupBar",
								ChartGroupLine = "ChartGroupLine",
								ChartGroupOther = "ChartGroupOther",
								ChartGroupPie = "ChartGroupPie",
								ChartGroupScatter = "ChartGroupScatter",
								CreateArea3DChart = "CreateArea3DChart",
								CreateArea3DChartLarge = "CreateArea3DChartLarge",
								CreateAreaChart = "CreateAreaChart",
								CreateAreaChartLarge = "CreateAreaChartLarge",
								CreateBar3DChart = "CreateBar3DChart",
								CreateBar3DChartLarge = "CreateBar3DChartLarge",
								CreateBarChart = "CreateBarChart",
								CreateBarChartLarge = "CreateBarChartLarge",
								CreateBubbleChart = "CreateBubbleChart",
								CreateBubbleChartLarge = "CreateBubbleChartLarge",
								CreateConeBar3DChart = "CreateConeBar3DChart",
								CreateConeBar3DChartLarge = "CreateConeBar3DChartLarge",
								CreateConeFullStackedBar3DChart = "CreateConeFullStackedBar3DChart",
								CreateConeFullStackedBar3DChartLarge = "CreateConeFullStackedBar3DChartLarge",
								CreateConeManhattanBarChart = "CreateConeManhattanBarChart",
								CreateConeManhattanBarChartLarge = "CreateConeManhattanBarChartLarge",
								CreateConeStackedBar3DChart = "CreateConeStackedBar3DChart",
								CreateConeStackedBar3DChartLarge = "CreateConeStackedBar3DChartLarge",
								CreateCylinderBar3DChart = "CreateCylinderBar3DChart",
								CreateCylinderBar3DChartLarge = "CreateCylinderBar3DChartLarge",
								CreateCylinderFullStackedBar3DChart = "CreateCylinderFullStackedBar3DChart",
								CreateCylinderFullStackedBar3DChartLarge = "CreateCylinderFullStackedBar3DChartLarge",
								CreateCylinderManhattanBarChart = "CreateCylinderManhattanBarChart",
								CreateCylinderManhattanBarChartLarge = "CreateCylinderManhattanBarChartLarge",
								CreateCylinderStackedBar3DChart = "CreateCylinderStackedBar3DChart",
								CreateCylinderStackedBar3DChartLarge = "CreateCylinderStackedBar3DChartLarge",
								CreateDoughnutChart = "CreateDoughnutChart",
								CreateDoughnutChartLarge = "CreateDoughnutChartLarge",
								CreateFullStackedArea3DChart = "CreateFullStackedArea3DChart",
								CreateFullStackedArea3DChartLarge = "CreateFullStackedArea3DChartLarge",
								CreateFullStackedAreaChart = "CreateFullStackedAreaChart",
								CreateFullStackedAreaChartLarge = "CreateFullStackedAreaChartLarge",
								CreateFullStackedBar3DChart = "CreateFullStackedBar3DChart",
								CreateFullStackedBar3DChartLarge = "CreateFullStackedBar3DChartLarge",
								CreateFullStackedBarChart = "CreateFullStackedBarChart",
								CreateFullStackedBarChartLarge = "CreateFullStackedBarChartLarge",
								CreateFullStackedLineChart = "CreateFullStackedLineChart",
								CreateFullStackedLineChartLarge = "CreateFullStackedLineChartLarge",
								CreateLine3DChart = "CreateLine3DChart",
								CreateLine3DChartLarge = "CreateLine3DChartLarge",
								CreateLineChart = "CreateLineChart",
								CreateLineChartLarge = "CreateLineChartLarge",
								CreateManhattanBarChart = "CreateManhattanBarChart",
								CreateManhattanBarChartLarge = "CreateManhattanBarChartLarge",
								CreatePie3DChart = "CreatePie3DChart",
								CreatePie3DChartLarge = "CreatePie3DChartLarge",
								CreatePieChart = "CreatePieChart",
								CreatePieChartLarge = "CreatePieChartLarge",
								CreatePyramidBar3DChart = "CreatePyramidBar3DChart",
								CreatePyramidBar3DChartLarge = "CreatePyramidBar3DChartLarge",
								CreatePyramidFullStackedBar3DChart = "CreatePyramidFullStackedBar3DChart",
								CreatePyramidFullStackedBar3DChartLarge = "CreatePyramidFullStackedBar3DChartLarge",
								CreatePyramidManhattanBarChart = "CreatePyramidManhattanBarChart",
								CreatePyramidManhattanBarChartLarge = "CreatePyramidManhattanBarChartLarge",
								CreatePyramidStackedBar3DChart = "CreatePyramidStackedBar3DChart",
								CreatePyramidStackedBar3DChartLarge = "CreatePyramidStackedBar3DChartLarge",
								CreateRadarLineChart = "CreateRadarLineChart",
								CreateRadarLineChartLarge = "CreateRadarLineChartLarge",
								CreateRotatedBarChart = "CreateRotatedBarChart",
								CreateRotatedBarChartLarge = "CreateRotatedBarChartLarge",
								CreateRotatedFullStackedBarChart = "CreateRotatedFullStackedBarChart",
								CreateRotatedFullStackedBarChartLarge = "CreateRotatedFullStackedBarChartLarge",
								CreateRotatedStackedBarChart = "CreateRotatedStackedBarChart",
								CreateRotatedStackedBarChartLarge = "CreateRotatedStackedBarChartLarge",
								CreateStackedArea3DChart = "CreateStackedArea3DChart",
								CreateStackedArea3DChartLarge = "CreateStackedArea3DChartLarge",
								CreateStackedAreaChart = "CreateStackedAreaChart",
								CreateStackedAreaChartLarge = "CreateStackedAreaChartLarge",
								CreateStackedBar3DChart = "CreateStackedBar3DChart",
								CreateStackedBar3DChartLarge = "CreateStackedBar3DChartLarge",
								CreateStackedBarChart = "CreateStackedBarChart",
								CreateStackedBarChartLarge = "CreateStackedBarChartLarge",
								CreateStackedLineChart = "CreateStackedLineChart",
								CreateStackedLineChartLarge = "CreateStackedLineChartLarge",
								CreateStockChart = "CreateStockChart",
								CreateStockChartLarge = "CreateStockChartLarge",
								CreateRotatedBar3DChart = "CreateRotatedBar3DChart",
								CreateRotatedBar3DChartLarge = "CreateRotatedBar3DChartLarge",
								CreateRotatedStackedBar3DChart = "CreateRotatedStackedBar3DChart",
								CreateRotatedFullStackedBar3DChart = "CreateRotatedFullStackedBar3DChart",
								CreateRotatedCylinderBar3DChart = "CreateRotatedCylinderBar3DChart",
								CreateRotatedStackedCylinderBar3DChart = "CreateRotatedStackedCylinderBar3DChart",
								CreateRotatedFullStackedCylinderBar3DChar = "CreateRotatedFullStackedCylinderBar3DChart",
								CreateRotatedConeBar3DChart = "CreateRotatedConeBar3DChart",
								CreateRotatedStackedConeBar3DChart = "CreateRotatedStackedConeBar3DChart",
								CreateRotatedFullStackedConeBar3DChart = "CreateRotatedFullStackedConeBar3DChart",
								CreateRotatedPyramidBar3DChart = "CreateRotatedPyramidBar3DChart",
								CreateRotatedStackedPyramidBar3DChart = "CreateRotatedStackedPyramidBar3DChart",
								CreateRotatedFullStackedPyramidBar3DChart = "CreateRotatedFullStackedPyramidBar3DChart",
								CreateLineChartNoMarkers = "CreateLineChartNoMarkers",
								CreateStackedLineChartNoMarkers = "CreateStackedLineChartNoMarkers",
								CreateFullStackedLineChartNoMarkers = "CreateFullStackedLineChartNoMarkers",
								CreateScatterChartMarkersOnly = "CreateScatterChartMarkersOnly",
								CreateScatterChartLines = "CreateScatterChartLines",
								CreateScatterChartSmoothLines = "CreateScatterChartSmoothLines",
								CreateScatterChartLinesAndMarkers = "CreateScatterChartLinesAndMarkers",
								CreateScatterChartSmoothLinesAndMarkers = "CreateScatterChartSmoothLinesAndMarkers",
								CreateBubble3DChart = "CreateBubble3DChart",
								CreateRadarLineChartNoMarkers = "CreateRadarLineChartNoMarkers",
								CreateRadarLineChartFilled = "CreateRadarLineChartFilled",
								CreateStockChartOpenHighLowClose = "CreateStockChartOpenHighLowClose",
								CreateExplodedPieChart = "CreateExplodedPieChart",
								CreateExplodedPie3DChart = "CreateExplodedPie3DChart",
								CreateExplodedDoughnutChart = "CreateExplodedDoughnutChart",
								CreateStockChartHighLowClose = "CreateStockChartHighLowClose",
								CreateRotatedStackedBar3DChartLarge = "CreateRotatedStackedBar3DChartLarge",
								CreateRotatedFullStackedBar3DChartLarge = "CreateRotatedFullStackedBar3DChartLarge",
								CreateRotatedCylinderBar3DChartLarge = "CreateRotatedCylinderBar3DChartLarge",
								CreateRotatedStackedCylinderBar3DChartLarge = "CreateRotatedStackedCylinderBar3DChartLarge",
								CreateRotatedFullStackedCylinderBar3DCharLarge = "CreateRotatedFullStackedCylinderBar3DChartLarge",
								CreateRotatedConeBar3DChartLarge = "CreateRotatedConeBar3DChartLarge",
								CreateRotatedStackedConeBar3DChartLarge = "CreateRotatedStackedConeBar3DChartLarge",
								CreateRotatedFullStackedConeBar3DChartLarge = "CreateRotatedFullStackedConeBar3DChartLarge",
								CreateRotatedPyramidBar3DChartLarge = "CreateRotatedPyramidBar3DChartLarge",
								CreateRotatedStackedPyramidBar3DChartLarge = "CreateRotatedStackedPyramidBar3DChartLarge",
								CreateRotatedFullStackedPyramidBar3DChartLarge = "CreateRotatedFullStackedPyramidBar3DChartLarge",
								CreateLineChartNoMarkersLarge = "CreateLineChartNoMarkersLarge",
								CreateStackedLineChartNoMarkersLarge = "CreateStackedLineChartNoMarkersLarge",
								CreateFullStackedLineChartNoMarkersLarge = "CreateFullStackedLineChartNoMarkersLarge",
								CreateScatterChartMarkersOnlyLarge = "CreateScatterChartMarkersOnlyLarge",
								CreateScatterChartLinesLarge = "CreateScatterChartLinesLarge",
								CreateScatterChartSmoothLinesLarge = "CreateScatterChartSmoothLinesLarge",
								CreateScatterChartLinesAndMarkersLarge = "CreateScatterChartLinesAndMarkersLarge",
								CreateScatterChartSmoothLinesAndMarkersLarge = "CreateScatterChartSmoothLinesAndMarkersLarge",
								CreateBubble3DChartLarge = "CreateBubble3DChartLarge",
								CreateRadarLineChartNoMarkersLarge = "CreateRadarLineChartNoMarkersLarge",
								CreateRadarLineChartFilledLarge = "CreateRadarLineChartFilledLarge",
								CreateStockChartOpenHighLowCloseLarge = "CreateStockChartOpenHighLowCloseLarge",
								CreateExplodedPieChartLarge = "CreateExplodedPieChartLarge",
								CreateExplodedPie3DChartLarge = "CreateExplodedPie3DChartLarge",
								CreateExplodedDoughnutChartLarge = "CreateExplodedDoughnutChartLarge",
								CreateStockChartHighLowCloseLarge = "CreateStockChartHighLowCloseLarge",
								ChartPresetArea01 = "ChartPresetArea01",
								ChartPresetArea02 = "ChartPresetArea02",
								ChartPresetArea03 = "ChartPresetArea03",
								ChartPresetArea04 = "ChartPresetArea04",
								ChartPresetArea05 = "ChartPresetArea05",
								ChartPresetArea06 = "ChartPresetArea06",
								ChartPresetArea07 = "ChartPresetArea07",
								ChartPresetArea08 = "ChartPresetArea08",
								ChartPresetBarClustered01 = "ChartPresetBarClustered01",
								ChartPresetBarClustered02 = "ChartPresetBarClustered02",
								ChartPresetBarClustered03 = "ChartPresetBarClustered03",
								ChartPresetBarClustered04 = "ChartPresetBarClustered04",
								ChartPresetBarClustered05 = "ChartPresetBarClustered05",
								ChartPresetBarClustered06 = "ChartPresetBarClustered06",
								ChartPresetBarClustered07 = "ChartPresetBarClustered07",
								ChartPresetBarClustered08 = "ChartPresetBarClustered08",
								ChartPresetBarClustered09 = "ChartPresetBarClustered09",
								ChartPresetBarClustered10 = "ChartPresetBarClustered10",
								ChartPresetBarStacked01 = "ChartPresetBarStacked01",
								ChartPresetBarStacked02 = "ChartPresetBarStacked02",
								ChartPresetBarStacked03 = "ChartPresetBarStacked03",
								ChartPresetBarStacked04 = "ChartPresetBarStacked04",
								ChartPresetBarStacked05 = "ChartPresetBarStacked05",
								ChartPresetBarStacked06 = "ChartPresetBarStacked06",
								ChartPresetBarStacked07 = "ChartPresetBarStacked07",
								ChartPresetBarStacked08 = "ChartPresetBarStacked08",
								ChartPresetBarStacked09 = "ChartPresetBarStacked09",
								ChartPresetBarStacked10 = "ChartPresetBarStacked10",
								ChartPresetColumnClustered01 = "ChartPresetColumnClustered01",
								ChartPresetColumnClustered02 = "ChartPresetColumnClustered02",
								ChartPresetColumnClustered03 = "ChartPresetColumnClustered03",
								ChartPresetColumnClustered04 = "ChartPresetColumnClustered04",
								ChartPresetColumnClustered05 = "ChartPresetColumnClustered05",
								ChartPresetColumnClustered06 = "ChartPresetColumnClustered06",
								ChartPresetColumnClustered07 = "ChartPresetColumnClustered07",
								ChartPresetColumnClustered08 = "ChartPresetColumnClustered08",
								ChartPresetColumnClustered09 = "ChartPresetColumnClustered09",
								ChartPresetColumnClustered10 = "ChartPresetColumnClustered10",
								ChartPresetColumnClustered11 = "ChartPresetColumnClustered11",
								ChartPresetColumnStacked01 = "ChartPresetColumnStacked01",
								ChartPresetColumnStacked02 = "ChartPresetColumnStacked02",
								ChartPresetColumnStacked03 = "ChartPresetColumnStacked03",
								ChartPresetColumnStacked04 = "ChartPresetColumnStacked04",
								ChartPresetColumnStacked05 = "ChartPresetColumnStacked05",
								ChartPresetColumnStacked06 = "ChartPresetColumnStacked06",
								ChartPresetColumnStacked07 = "ChartPresetColumnStacked07",
								ChartPresetColumnStacked08 = "ChartPresetColumnStacked08",
								ChartPresetColumnStacked09 = "ChartPresetColumnStacked09",
								ChartPresetColumnStacked10 = "ChartPresetColumnStacked10",
								ChartPresetDoughnut01 = "ChartPresetDoughnut01",
								ChartPresetDoughnut02 = "ChartPresetDoughnut02",
								ChartPresetDoughnut03 = "ChartPresetDoughnut03",
								ChartPresetDoughnut04 = "ChartPresetDoughnut04",
								ChartPresetDoughnut05 = "ChartPresetDoughnut05",
								ChartPresetDoughnut06 = "ChartPresetDoughnut06",
								ChartPresetDoughnut07 = "ChartPresetDoughnut07",
								ChartPresetLine01 = "ChartPresetLine01",
								ChartPresetLine02 = "ChartPresetLine02",
								ChartPresetLine03 = "ChartPresetLine03",
								ChartPresetLine04 = "ChartPresetLine04",
								ChartPresetLine05 = "ChartPresetLine05",
								ChartPresetLine06 = "ChartPresetLine06",
								ChartPresetLine07 = "ChartPresetLine07",
								ChartPresetLine08 = "ChartPresetLine08",
								ChartPresetLine09 = "ChartPresetLine09",
								ChartPresetLine10 = "ChartPresetLine10",
								ChartPresetLine11 = "ChartPresetLine11",
								ChartPresetLine12 = "ChartPresetLine12",
								ChartPresetPie01 = "ChartPresetPie01",
								ChartPresetPie02 = "ChartPresetPie02",
								ChartPresetPie03 = "ChartPresetPie03",
								ChartPresetPie04 = "ChartPresetPie04",
								ChartPresetPie05 = "ChartPresetPie05",
								ChartPresetPie06 = "ChartPresetPie06",
								ChartPresetPie07 = "ChartPresetPie07",
								ChartPresetRadar01 = "ChartPresetRadar01",
								ChartPresetRadar02 = "ChartPresetRadar02",
								ChartPresetRadar03 = "ChartPresetRadar03",
								ChartPresetRadar04 = "ChartPresetRadar04",
								ChartPresetScatter01 = "ChartPresetScatter01",
								ChartPresetScatter02 = "ChartPresetScatter02",
								ChartPresetScatter03 = "ChartPresetScatter03",
								ChartPresetScatter04 = "ChartPresetScatter04",
								ChartPresetScatter05 = "ChartPresetScatter05",
								ChartPresetScatter06 = "ChartPresetScatter06",
								ChartPresetScatter07 = "ChartPresetScatter07",
								ChartPresetScatter08 = "ChartPresetScatter08",
								ChartPresetScatter09 = "ChartPresetScatter09",
								ChartPresetScatter10 = "ChartPresetScatter10",
								ChartPresetScatter11 = "ChartPresetScatter11",
								ChartPresetStock01 = "ChartPresetStock01",
								ChartPresetStock02 = "ChartPresetStock02",
								ChartPresetStock03 = "ChartPresetStock03",
								ChartPresetStock04 = "ChartPresetStock04",
								ChartPresetStock05 = "ChartPresetStock05",
								ChartPresetStock06 = "ChartPresetStock06",
								ChartPresetStock07 = "ChartPresetStock07",
								ChartSwitchRowColumn = "ChartSwitchRowColumn",
								ChartAxesGroup = "ChartAxesGroup",
								ChartAxisTitleGroup = "ChartAxisTitleGroup",
								ChartAxisTitleHorizontal = "ChartAxisTitleHorizontal",
								ChartAxisTitleHorizontal_None = "ChartAxisTitleHorizontal_None",
								ChartAxisTitleVertical = "ChartAxisTitleVertical",
								ChartAxisTitleVertical_HorizonlalText = "ChartAxisTitleVertical_HorizonlalText",
								ChartAxisTitleVertical_None = "ChartAxisTitleVertical_None",
								ChartAxisTitleVertical_RotatedText = "ChartAxisTitleVertical_RotatedText",
								ChartAxisTitleVertical_VerticalText = "ChartAxisTitleVertical_VerticalText",
								ChartHorizontalAxis_Billions = "ChartHorizontalAxis_Billions",
								ChartHorizontalAxis_Default = "ChartHorizontalAxis_Default",
								ChartHorizontalAxis_LeftToRight = "ChartHorizontalAxis_LeftToRight",
								ChartHorizontalAxis_LogScale = "ChartHorizontalAxis_LogScale",
								ChartHorizontalAxis_Millions = "ChartHorizontalAxis_Millions",
								ChartHorizontalAxis_None = "ChartHorizontalAxis_None",
								ChartHorizontalAxis_RightToLeft = "ChartHorizontalAxis_RightToLeft",
								ChartHorizontalAxis_Thousands = "ChartHorizontalAxis_Thousands",
								ChartHorizontalAxis_WithoutLabeling = "ChartHorizontalAxis_WithoutLabeling",
								ChartLabels_InsideBase = "ChartLabels_InsideBase",
								ChartLabels_InsideCenter = "ChartLabels_InsideCenter",
								ChartLabels_InsideEnd = "ChartLabels_InsideEnd",
								ChartLabels_LineAbove = "ChartLabels_LineAbove",
								ChartLabels_LineBelow = "ChartLabels_LineBelow",
								ChartLabels_LineCenter = "ChartLabels_LineCenter",
								ChartLabels_LineLeft = "ChartLabels_LineLeft",
								ChartLabels_LineNone = "ChartLabels_LineNone",
								ChartLabels_LineRight = "ChartLabels_LineRight",
								ChartLabels_None = "ChartLabels_None",
								ChartLabels_OutsideEnd = "ChartLabels_OutsideEnd",
								ChartLabels_Show = "ChartLabels_Show",
								ChartLegend_Bottom = "ChartLegend_Bottom",
								ChartLegend_Left = "ChartLegend_Left",
								ChartLegend_LeftOverlay = "ChartLegend_LeftOverlay",
								ChartLegend_None = "ChartLegend_None",
								ChartLegend_Right = "ChartLegend_Right",
								ChartLegend_RightOverlay = "ChartLegend_RightOverlay",
								ChartLegend_Top = "ChartLegend_Top",
								ChartTitleAbove = "ChartTitleAbove",
								ChartTitleCenteredOverlay = "ChartTitleCenteredOverlay",
								ChartTitleNone = "ChartTitleNone",
								ChartVerticalAxis_Billions = "ChartVerticalAxis_Billions",
								ChartVerticalAxis_BottomUp = "ChartVerticalAxis_BottomUp",
								ChartVerticalAxis_Default = "ChartVerticalAxis_Default",
								ChartVerticalAxis_LogScale = "ChartVerticalAxis_LogScale",
								ChartVerticalAxis_Millions = "ChartVerticalAxis_Millions",
								ChartVerticalAxis_None = "ChartVerticalAxis_None",
								ChartVerticalAxis_Thousands = "ChartVerticalAxis_Thousands",
								ChartVerticalAxis_TopDown = "ChartVerticalAxis_TopDown",
								ChartVerticallAxis_WithoutLabeling = "ChartVerticallAxis_WithoutLabeling",
								ChartGridlines = "ChartGridlines",
								ChartGridlinesHorizontal_Major = "ChartGridlinesHorizontal_Major",
								ChartGridlinesHorizontal_MajorMinor = "ChartGridlinesHorizontal_MajorMinor",
								ChartGridlinesHorizontal_Minor = "ChartGridlinesHorizontal_Minor",
								ChartGridlinesHorizontal_None = "ChartGridlinesHorizontal_None",
								ChartGridlinesVertical_Major = "ChartGridlinesVertical_Major",
								ChartGridlinesVertical_MajorMinor = "ChartGridlinesVertical_MajorMinor",
								ChartGridlinesVertical_Minor = "ChartGridlinesVertical_Minor",
								ChartGridlinesVertical_None = "ChartGridlinesVertical_None",
								FreezeFirstColumn = "FreezeFirstColumn",
								FreezeFirstColumnLarge = "FreezeFirstColumnLarge",
								FreezePanes = "FreezePanes",
								FreezePanesLarge = "FreezePanesLarge",
								FreezeTopRow = "FreezeTopRow",
								FreezeTopRowLarge = "FreezeTopRowLarge",
								UnfreezePanes = "UnfreezePanes",
								UnfreezePanesLarge = "UnfreezePanesLarge",
								DropDownButton = "DropDownButton",
								AutoFilter_Filtered = "AutoFilter_Filtered",
								AutoFilter_Ascending = "AutoFilter_Ascending",
								AutoFilter_Descending = "AutoFilter_Descending",
								AutoFilter_FilteredAndAscending = "AutoFilter_FilteredAndAscending",
								AutoFilter_FilteredAndDescending = "AutoFilter_FilteredAndDescending",
								ClearFilter = "ClearFilter",
								SortAsc = "SortAsc",
								SortDesc = "SortDesc",
								InsertSheet = "InsertSheet",
								RemoveSheet = "RemoveSheet",
								FormulaBarCancelButton = "FormulaBarCancelButton",
								FormulaBarEnterButton = "FormulaBarEnterButton"
								;
		#endregion
		static SpreadsheetIconImages() {
			Categories.Add(MenuIconSetType.NotSet, "Icons");
			Categories.Add(MenuIconSetType.Colored, "Icons");
			Categories.Add(MenuIconSetType.ColoredLight, "Icons");
			Categories.Add(MenuIconSetType.GrayScaled, "GIcons");
			Categories.Add(MenuIconSetType.GrayScaledWithWhiteHottrack, "GWIcons");
			SpriteCssResourceNames.Add(MenuIconSetType.NotSet, ASPxSpreadsheet.SpreadsheetIconSpriteCssResourceName);
			SpriteCssResourceNames.Add(MenuIconSetType.Colored, ASPxSpreadsheet.SpreadsheetIconSpriteCssResourceName);
			SpriteCssResourceNames.Add(MenuIconSetType.GrayScaled, ASPxSpreadsheet.SpreadsheetGrayScaleIconSpriteCssResourceName);
			SpriteCssResourceNames.Add(MenuIconSetType.GrayScaledWithWhiteHottrack, ASPxSpreadsheet.SpreadsheetGrayScaleWithWhiteHottrackIconSpriteCssResourceName);
			SpriteImageResourceNames.Add(MenuIconSetType.NotSet, ASPxSpreadsheet.SpreadsheetImageResourcePath + IconSpriteImageName + ".png");
			SpriteImageResourceNames.Add(MenuIconSetType.Colored, ASPxSpreadsheet.SpreadsheetImageResourcePath + IconSpriteImageName + ".png");
			SpriteImageResourceNames.Add(MenuIconSetType.GrayScaled, ASPxSpreadsheet.SpreadsheetImageResourcePath + GrayScaleIconSpriteImageName + ".png");
			SpriteImageResourceNames.Add(MenuIconSetType.GrayScaledWithWhiteHottrack, ASPxSpreadsheet.SpreadsheetImageResourcePath + GrayScaleWithWhiteHottrackIconSpriteImageName + ".png");
		}
		public SpreadsheetIconImages(ISkinOwner owner)
			: base(owner) {
		}
		public MenuIconSetType MenuIconSet {
			get { return (MenuIconSetType)GetEnumProperty("MenuIconSet", MenuIconSetType.NotSet); }
			set { SetEnumProperty("MenuIconSet", MenuIconSetType.NotSet, value); }
		}
		protected override Type GetResourceType() {
			return typeof(ASPxSpreadsheet);
		}
		protected override string GetResourceImagePath() {
			return ASPxSpreadsheet.SpreadsheetImageResourcePath;
		}
		protected override string GetDesignTimeResourceSpriteImagePath() {
			return SpriteImageResourceNames[MenuIconSet];
		}
		protected override string GetResourceSpriteCssPath() {
			return SpriteCssResourceNames[MenuIconSet];
		}
		protected override string GetImageCategory() {
			return Categories[MenuIconSet];
		}
		protected internal void RegisterIconSpriteCssFile(Page page) {
			base.RegisterDefaultSpriteCssFile(page);
		}
		protected override string GetCssPostFix() {
			return string.Empty;
		}
		protected override bool KeepDefaultSizes {
			get { return true; }
		}
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			base.PopulateImageInfoList(list);
			ImageFlags imageFlags = ImageFlags.IsPng;
			#region InitializeImages
			list.Add(new ImageInfo(Copy, imageFlags, DefaultSmallSize, Copy));
			list.Add(new ImageInfo(Cut, imageFlags, DefaultSmallSize, Cut));
			list.Add(new ImageInfo(Delete_Hyperlink, imageFlags, DefaultSmallSize, Delete_Hyperlink));
			list.Add(new ImageInfo(Hyperlink, imageFlags, DefaultSmallSize, Hyperlink));
			list.Add(new ImageInfo(OpenHyperlink, imageFlags, DefaultSmallSize, OpenHyperlink));
			list.Add(new ImageInfo(Paste, imageFlags, DefaultSmallSize, Paste));
			list.Add(new ImageInfo(ChartGroupColumn, imageFlags, DefaultSmallSize, ChartGroupColumn));
			list.Add(new ImageInfo(ChartSelectData, imageFlags, DefaultSmallSize, ChartSelectData));
			list.Add(new ImageInfo(FloatingObjectBringForward, imageFlags, DefaultSmallSize, FloatingObjectBringForward));
			list.Add(new ImageInfo(FloatingObjectBringToFront, imageFlags, DefaultSmallSize, FloatingObjectBringToFront));
			list.Add(new ImageInfo(FloatingObjectSendBackward, imageFlags, DefaultSmallSize, FloatingObjectSendBackward));
			list.Add(new ImageInfo(FloatingObjectSendToBack, imageFlags, DefaultSmallSize, FloatingObjectSendToBack));
			list.Add(new ImageInfo(ChartGroupArea, imageFlags, DefaultSmallSize, ChartGroupArea));
			list.Add(new ImageInfo(ChartGroupBar, imageFlags, DefaultSmallSize, ChartGroupBar));
			list.Add(new ImageInfo(ChartGroupLine, imageFlags, DefaultSmallSize, ChartGroupLine));
			list.Add(new ImageInfo(ChartGroupOther, imageFlags, DefaultSmallSize, ChartGroupOther));
			list.Add(new ImageInfo(ChartGroupPie, imageFlags, DefaultSmallSize, ChartGroupPie));
			list.Add(new ImageInfo(ChartGroupScatter, imageFlags, DefaultSmallSize, ChartGroupScatter));
			list.Add(new ImageInfo(CreateArea3DChart, imageFlags, DefaultSmallSize, CreateArea3DChart));
			list.Add(new ImageInfo(CreateArea3DChartLarge, imageFlags, DefaultLargeSize, CreateArea3DChartLarge));
			list.Add(new ImageInfo(CreateAreaChart, imageFlags, DefaultSmallSize, CreateAreaChart));
			list.Add(new ImageInfo(CreateAreaChartLarge, imageFlags, DefaultLargeSize, CreateAreaChartLarge));
			list.Add(new ImageInfo(CreateBar3DChart, imageFlags, DefaultSmallSize, CreateBar3DChart));
			list.Add(new ImageInfo(CreateBar3DChartLarge, imageFlags, DefaultLargeSize, CreateBar3DChartLarge));
			list.Add(new ImageInfo(CreateBarChart, imageFlags, DefaultSmallSize, CreateBarChart));
			list.Add(new ImageInfo(CreateBarChartLarge, imageFlags, DefaultLargeSize, CreateBarChartLarge));
			list.Add(new ImageInfo(CreateBubbleChart, imageFlags, DefaultSmallSize, CreateBubbleChart));
			list.Add(new ImageInfo(CreateBubbleChartLarge, imageFlags, DefaultLargeSize, CreateBubbleChartLarge));
			list.Add(new ImageInfo(CreateConeBar3DChart, imageFlags, DefaultSmallSize, CreateConeBar3DChart));
			list.Add(new ImageInfo(CreateConeBar3DChartLarge, imageFlags, DefaultLargeSize, CreateConeBar3DChartLarge));
			list.Add(new ImageInfo(CreateConeFullStackedBar3DChart, imageFlags, DefaultSmallSize, CreateConeFullStackedBar3DChart));
			list.Add(new ImageInfo(CreateConeFullStackedBar3DChartLarge, imageFlags, DefaultLargeSize, CreateConeFullStackedBar3DChartLarge));
			list.Add(new ImageInfo(CreateConeManhattanBarChart, imageFlags, DefaultSmallSize, CreateConeManhattanBarChart));
			list.Add(new ImageInfo(CreateConeManhattanBarChartLarge, imageFlags, DefaultLargeSize, CreateConeManhattanBarChartLarge));
			list.Add(new ImageInfo(CreateConeStackedBar3DChart, imageFlags, DefaultSmallSize, CreateConeStackedBar3DChart));
			list.Add(new ImageInfo(CreateConeStackedBar3DChartLarge, imageFlags, DefaultLargeSize, CreateConeStackedBar3DChartLarge));
			list.Add(new ImageInfo(CreateCylinderBar3DChart, imageFlags, DefaultSmallSize, CreateCylinderBar3DChart));
			list.Add(new ImageInfo(CreateCylinderBar3DChartLarge, imageFlags, DefaultLargeSize, CreateCylinderBar3DChartLarge));
			list.Add(new ImageInfo(CreateCylinderFullStackedBar3DChart, imageFlags, DefaultSmallSize, CreateCylinderFullStackedBar3DChart));
			list.Add(new ImageInfo(CreateCylinderFullStackedBar3DChartLarge, imageFlags, DefaultLargeSize, CreateCylinderFullStackedBar3DChartLarge));
			list.Add(new ImageInfo(CreateCylinderManhattanBarChart, imageFlags, DefaultSmallSize, CreateCylinderManhattanBarChart));
			list.Add(new ImageInfo(CreateCylinderManhattanBarChartLarge, imageFlags, DefaultLargeSize, CreateCylinderManhattanBarChartLarge));
			list.Add(new ImageInfo(CreateCylinderStackedBar3DChart, imageFlags, DefaultSmallSize, CreateCylinderStackedBar3DChart));
			list.Add(new ImageInfo(CreateCylinderStackedBar3DChartLarge, imageFlags, DefaultLargeSize, CreateCylinderStackedBar3DChartLarge));
			list.Add(new ImageInfo(CreateDoughnutChart, imageFlags, DefaultSmallSize, CreateDoughnutChart));
			list.Add(new ImageInfo(CreateDoughnutChartLarge, imageFlags, DefaultLargeSize, CreateDoughnutChartLarge));
			list.Add(new ImageInfo(CreateFullStackedArea3DChart, imageFlags, DefaultSmallSize, CreateFullStackedArea3DChart));
			list.Add(new ImageInfo(CreateFullStackedArea3DChartLarge, imageFlags, DefaultLargeSize, CreateFullStackedArea3DChartLarge));
			list.Add(new ImageInfo(CreateFullStackedAreaChart, imageFlags, DefaultSmallSize, CreateFullStackedAreaChart));
			list.Add(new ImageInfo(CreateFullStackedAreaChartLarge, imageFlags, DefaultLargeSize, CreateFullStackedAreaChartLarge));
			list.Add(new ImageInfo(CreateFullStackedBar3DChart, imageFlags, DefaultSmallSize, CreateFullStackedBar3DChart));
			list.Add(new ImageInfo(CreateFullStackedBar3DChartLarge, imageFlags, DefaultLargeSize, CreateFullStackedBar3DChartLarge));
			list.Add(new ImageInfo(CreateFullStackedBarChart, imageFlags, DefaultSmallSize, CreateFullStackedBarChart));
			list.Add(new ImageInfo(CreateFullStackedBarChartLarge, imageFlags, DefaultLargeSize, CreateFullStackedBarChartLarge));
			list.Add(new ImageInfo(CreateFullStackedLineChart, imageFlags, DefaultSmallSize, CreateFullStackedLineChart));
			list.Add(new ImageInfo(CreateFullStackedLineChartLarge, imageFlags, DefaultLargeSize, CreateFullStackedLineChartLarge));
			list.Add(new ImageInfo(CreateLine3DChart, imageFlags, DefaultSmallSize, CreateLine3DChart));
			list.Add(new ImageInfo(CreateLine3DChartLarge, imageFlags, DefaultLargeSize, CreateLine3DChartLarge));
			list.Add(new ImageInfo(CreateLineChart, imageFlags, DefaultSmallSize, CreateLineChart));
			list.Add(new ImageInfo(CreateLineChartLarge, imageFlags, DefaultLargeSize, CreateLineChartLarge));
			list.Add(new ImageInfo(CreateManhattanBarChart, imageFlags, DefaultSmallSize, CreateManhattanBarChart));
			list.Add(new ImageInfo(CreateManhattanBarChartLarge, imageFlags, DefaultLargeSize, CreateManhattanBarChartLarge));
			list.Add(new ImageInfo(CreatePie3DChart, imageFlags, DefaultSmallSize, CreatePie3DChart));
			list.Add(new ImageInfo(CreatePie3DChartLarge, imageFlags, DefaultLargeSize, CreatePie3DChartLarge));
			list.Add(new ImageInfo(CreatePieChart, imageFlags, DefaultSmallSize, CreatePieChart));
			list.Add(new ImageInfo(CreatePieChartLarge, imageFlags, DefaultLargeSize, CreatePieChartLarge));
			list.Add(new ImageInfo(CreatePyramidBar3DChart, imageFlags, DefaultSmallSize, CreatePyramidBar3DChart));
			list.Add(new ImageInfo(CreatePyramidBar3DChartLarge, imageFlags, DefaultLargeSize, CreatePyramidBar3DChartLarge));
			list.Add(new ImageInfo(CreatePyramidFullStackedBar3DChart, imageFlags, DefaultSmallSize, CreatePyramidFullStackedBar3DChart));
			list.Add(new ImageInfo(CreatePyramidFullStackedBar3DChartLarge, imageFlags, DefaultLargeSize, CreatePyramidFullStackedBar3DChartLarge));
			list.Add(new ImageInfo(CreatePyramidManhattanBarChart, imageFlags, DefaultSmallSize, CreatePyramidManhattanBarChart));
			list.Add(new ImageInfo(CreatePyramidManhattanBarChartLarge, imageFlags, DefaultLargeSize, CreatePyramidManhattanBarChartLarge));
			list.Add(new ImageInfo(CreatePyramidStackedBar3DChart, imageFlags, DefaultSmallSize, CreatePyramidStackedBar3DChart));
			list.Add(new ImageInfo(CreatePyramidStackedBar3DChartLarge, imageFlags, DefaultLargeSize, CreatePyramidStackedBar3DChartLarge));
			list.Add(new ImageInfo(CreateRadarLineChart, imageFlags, DefaultSmallSize, CreateRadarLineChart));
			list.Add(new ImageInfo(CreateRadarLineChartLarge, imageFlags, DefaultLargeSize, CreateRadarLineChartLarge));
			list.Add(new ImageInfo(CreateRotatedBarChart, imageFlags, DefaultSmallSize, CreateRotatedBarChart));
			list.Add(new ImageInfo(CreateRotatedBarChartLarge, imageFlags, DefaultLargeSize, CreateRotatedBarChartLarge));
			list.Add(new ImageInfo(CreateRotatedFullStackedBarChart, imageFlags, DefaultSmallSize, CreateRotatedFullStackedBarChart));
			list.Add(new ImageInfo(CreateRotatedFullStackedBarChartLarge, imageFlags, DefaultLargeSize, CreateRotatedFullStackedBarChartLarge));
			list.Add(new ImageInfo(CreateRotatedStackedBarChart, imageFlags, DefaultSmallSize, CreateRotatedStackedBarChart));
			list.Add(new ImageInfo(CreateRotatedStackedBarChartLarge, imageFlags, DefaultLargeSize, CreateRotatedStackedBarChartLarge));
			list.Add(new ImageInfo(CreateStackedArea3DChart, imageFlags, DefaultSmallSize, CreateStackedArea3DChart));
			list.Add(new ImageInfo(CreateStackedArea3DChartLarge, imageFlags, DefaultLargeSize, CreateStackedArea3DChartLarge));
			list.Add(new ImageInfo(CreateStackedAreaChart, imageFlags, DefaultSmallSize, CreateStackedAreaChart));
			list.Add(new ImageInfo(CreateStackedAreaChartLarge, imageFlags, DefaultLargeSize, CreateStackedAreaChartLarge));
			list.Add(new ImageInfo(CreateStackedBar3DChart, imageFlags, DefaultSmallSize, CreateStackedBar3DChart));
			list.Add(new ImageInfo(CreateStackedBar3DChartLarge, imageFlags, DefaultLargeSize, CreateStackedBar3DChartLarge));
			list.Add(new ImageInfo(CreateStackedBarChart, imageFlags, DefaultSmallSize, CreateStackedBarChart));
			list.Add(new ImageInfo(CreateStackedBarChartLarge, imageFlags, DefaultLargeSize, CreateStackedBarChartLarge));
			list.Add(new ImageInfo(CreateStackedLineChart, imageFlags, DefaultSmallSize, CreateStackedLineChart));
			list.Add(new ImageInfo(CreateStackedLineChartLarge, imageFlags, DefaultLargeSize, CreateStackedLineChartLarge));
			list.Add(new ImageInfo(CreateStockChart, imageFlags, DefaultSmallSize, CreateStockChart));
			list.Add(new ImageInfo(CreateStockChartLarge, imageFlags, DefaultLargeSize, CreateStockChartLarge));
			list.Add(new ImageInfo(CreateRotatedBar3DChart, imageFlags, DefaultSmallSize, CreateRotatedBar3DChart));
			list.Add(new ImageInfo(CreateRotatedBar3DChartLarge, imageFlags, DefaultLargeSize, CreateRotatedBar3DChartLarge));
			list.Add(new ImageInfo(CreateRotatedStackedBar3DChart, imageFlags, DefaultSmallSize, CreateRotatedStackedBar3DChart));
			list.Add(new ImageInfo(CreateRotatedFullStackedBar3DChart, imageFlags, DefaultSmallSize, CreateRotatedFullStackedBar3DChart));
			list.Add(new ImageInfo(CreateRotatedCylinderBar3DChart, imageFlags, DefaultSmallSize, CreateRotatedCylinderBar3DChart));
			list.Add(new ImageInfo(CreateRotatedStackedCylinderBar3DChart, imageFlags, DefaultSmallSize, CreateRotatedStackedCylinderBar3DChart));
			list.Add(new ImageInfo(CreateRotatedFullStackedCylinderBar3DChar, imageFlags, DefaultSmallSize, CreateRotatedFullStackedCylinderBar3DChar));
			list.Add(new ImageInfo(CreateRotatedConeBar3DChart, imageFlags, DefaultSmallSize, CreateRotatedConeBar3DChart));
			list.Add(new ImageInfo(CreateRotatedStackedConeBar3DChart, imageFlags, DefaultSmallSize, CreateRotatedStackedConeBar3DChart));
			list.Add(new ImageInfo(CreateRotatedFullStackedConeBar3DChart, imageFlags, DefaultSmallSize, CreateRotatedFullStackedConeBar3DChart));
			list.Add(new ImageInfo(CreateRotatedPyramidBar3DChart, imageFlags, DefaultSmallSize, CreateRotatedPyramidBar3DChart));
			list.Add(new ImageInfo(CreateRotatedStackedPyramidBar3DChart, imageFlags, DefaultSmallSize, CreateRotatedStackedPyramidBar3DChart));
			list.Add(new ImageInfo(CreateRotatedFullStackedPyramidBar3DChart, imageFlags, DefaultSmallSize, CreateRotatedFullStackedPyramidBar3DChart));
			list.Add(new ImageInfo(CreateLineChartNoMarkers, imageFlags, DefaultSmallSize, CreateLineChartNoMarkers));
			list.Add(new ImageInfo(CreateStackedLineChartNoMarkers, imageFlags, DefaultSmallSize, CreateStackedLineChartNoMarkers));
			list.Add(new ImageInfo(CreateFullStackedLineChartNoMarkers, imageFlags, DefaultSmallSize, CreateFullStackedLineChartNoMarkers));
			list.Add(new ImageInfo(CreateScatterChartMarkersOnly, imageFlags, DefaultSmallSize, CreateScatterChartMarkersOnly));
			list.Add(new ImageInfo(CreateScatterChartLines, imageFlags, DefaultSmallSize, CreateScatterChartLines));
			list.Add(new ImageInfo(CreateScatterChartSmoothLines, imageFlags, DefaultSmallSize, CreateScatterChartSmoothLines));
			list.Add(new ImageInfo(CreateScatterChartLinesAndMarkers, imageFlags, DefaultSmallSize, CreateScatterChartLinesAndMarkers));
			list.Add(new ImageInfo(CreateScatterChartSmoothLinesAndMarkers, imageFlags, DefaultSmallSize, CreateScatterChartSmoothLinesAndMarkers));
			list.Add(new ImageInfo(CreateBubble3DChart, imageFlags, DefaultSmallSize, CreateBubble3DChart));
			list.Add(new ImageInfo(CreateRadarLineChartNoMarkers, imageFlags, DefaultSmallSize, CreateRadarLineChartNoMarkers));
			list.Add(new ImageInfo(CreateRadarLineChartFilled, imageFlags, DefaultSmallSize, CreateRadarLineChartFilled));
			list.Add(new ImageInfo(CreateStockChartOpenHighLowClose, imageFlags, DefaultSmallSize, CreateStockChartOpenHighLowClose));
			list.Add(new ImageInfo(CreateExplodedPieChart, imageFlags, DefaultSmallSize, CreateExplodedPieChart));
			list.Add(new ImageInfo(CreateExplodedPie3DChart, imageFlags, DefaultSmallSize, CreateExplodedPie3DChart));
			list.Add(new ImageInfo(CreateExplodedDoughnutChart, imageFlags, DefaultSmallSize, CreateExplodedDoughnutChart));
			list.Add(new ImageInfo(CreateStockChartHighLowClose, imageFlags, DefaultSmallSize, CreateStockChartHighLowClose));
			list.Add(new ImageInfo(CreateRotatedStackedBar3DChartLarge, imageFlags, DefaultLargeSize, CreateRotatedStackedBar3DChartLarge));
			list.Add(new ImageInfo(CreateRotatedFullStackedBar3DChartLarge, imageFlags, DefaultLargeSize, CreateRotatedFullStackedBar3DChartLarge));
			list.Add(new ImageInfo(CreateRotatedCylinderBar3DChartLarge, imageFlags, DefaultLargeSize, CreateRotatedCylinderBar3DChartLarge));
			list.Add(new ImageInfo(CreateRotatedStackedCylinderBar3DChartLarge, imageFlags, DefaultLargeSize, CreateRotatedStackedCylinderBar3DChartLarge));
			list.Add(new ImageInfo(CreateRotatedFullStackedCylinderBar3DCharLarge, imageFlags, DefaultLargeSize, CreateRotatedFullStackedCylinderBar3DCharLarge));
			list.Add(new ImageInfo(CreateRotatedConeBar3DChartLarge, imageFlags, DefaultLargeSize, CreateRotatedConeBar3DChartLarge));
			list.Add(new ImageInfo(CreateRotatedStackedConeBar3DChartLarge, imageFlags, DefaultLargeSize, CreateRotatedStackedConeBar3DChartLarge));
			list.Add(new ImageInfo(CreateRotatedFullStackedConeBar3DChartLarge, imageFlags, DefaultLargeSize, CreateRotatedFullStackedConeBar3DChartLarge));
			list.Add(new ImageInfo(CreateRotatedPyramidBar3DChartLarge, imageFlags, DefaultLargeSize, CreateRotatedPyramidBar3DChartLarge));
			list.Add(new ImageInfo(CreateRotatedStackedPyramidBar3DChartLarge, imageFlags, DefaultLargeSize, CreateRotatedStackedPyramidBar3DChartLarge));
			list.Add(new ImageInfo(CreateRotatedFullStackedPyramidBar3DChartLarge, imageFlags, DefaultLargeSize, CreateRotatedFullStackedPyramidBar3DChartLarge));
			list.Add(new ImageInfo(CreateLineChartNoMarkersLarge, imageFlags, DefaultLargeSize, CreateLineChartNoMarkersLarge));
			list.Add(new ImageInfo(CreateStackedLineChartNoMarkersLarge, imageFlags, DefaultLargeSize, CreateStackedLineChartNoMarkersLarge));
			list.Add(new ImageInfo(CreateFullStackedLineChartNoMarkersLarge, imageFlags, DefaultLargeSize, CreateFullStackedLineChartNoMarkersLarge));
			list.Add(new ImageInfo(CreateScatterChartMarkersOnlyLarge, imageFlags, DefaultLargeSize, CreateScatterChartMarkersOnlyLarge));
			list.Add(new ImageInfo(CreateScatterChartLinesLarge, imageFlags, DefaultLargeSize, CreateScatterChartLinesLarge));
			list.Add(new ImageInfo(CreateScatterChartSmoothLinesLarge, imageFlags, DefaultLargeSize, CreateScatterChartSmoothLinesLarge));
			list.Add(new ImageInfo(CreateScatterChartLinesAndMarkersLarge, imageFlags, DefaultLargeSize, CreateScatterChartLinesAndMarkersLarge));
			list.Add(new ImageInfo(CreateScatterChartSmoothLinesAndMarkersLarge, imageFlags, DefaultLargeSize, CreateScatterChartSmoothLinesAndMarkersLarge));
			list.Add(new ImageInfo(CreateBubble3DChartLarge, imageFlags, DefaultLargeSize, CreateBubble3DChartLarge));
			list.Add(new ImageInfo(CreateRadarLineChartNoMarkersLarge, imageFlags, DefaultLargeSize, CreateRadarLineChartNoMarkersLarge));
			list.Add(new ImageInfo(CreateRadarLineChartFilledLarge, imageFlags, DefaultLargeSize, CreateRadarLineChartFilledLarge));
			list.Add(new ImageInfo(CreateStockChartOpenHighLowCloseLarge, imageFlags, DefaultLargeSize, CreateStockChartOpenHighLowCloseLarge));
			list.Add(new ImageInfo(CreateExplodedPieChartLarge, imageFlags, DefaultLargeSize, CreateExplodedPieChartLarge));
			list.Add(new ImageInfo(CreateExplodedPie3DChartLarge, imageFlags, DefaultLargeSize, CreateExplodedPie3DChartLarge));
			list.Add(new ImageInfo(CreateExplodedDoughnutChartLarge, imageFlags, DefaultLargeSize, CreateExplodedDoughnutChartLarge));
			list.Add(new ImageInfo(CreateStockChartHighLowCloseLarge, imageFlags, DefaultLargeSize, CreateStockChartHighLowCloseLarge));
			list.Add(new ImageInfo(ChartPresetArea01, imageFlags, DefaultPresetSize, ChartPresetArea01));
			list.Add(new ImageInfo(ChartPresetArea02, imageFlags, DefaultPresetSize, ChartPresetArea02));
			list.Add(new ImageInfo(ChartPresetArea03, imageFlags, DefaultPresetSize, ChartPresetArea03));
			list.Add(new ImageInfo(ChartPresetArea04, imageFlags, DefaultPresetSize, ChartPresetArea04));
			list.Add(new ImageInfo(ChartPresetArea05, imageFlags, DefaultPresetSize, ChartPresetArea05));
			list.Add(new ImageInfo(ChartPresetArea06, imageFlags, DefaultPresetSize, ChartPresetArea06));
			list.Add(new ImageInfo(ChartPresetArea07, imageFlags, DefaultPresetSize, ChartPresetArea07));
			list.Add(new ImageInfo(ChartPresetArea08, imageFlags, DefaultPresetSize, ChartPresetArea08));
			list.Add(new ImageInfo(ChartPresetBarClustered01, imageFlags, DefaultPresetSize, ChartPresetBarClustered01));
			list.Add(new ImageInfo(ChartPresetBarClustered02, imageFlags, DefaultPresetSize, ChartPresetBarClustered02));
			list.Add(new ImageInfo(ChartPresetBarClustered03, imageFlags, DefaultPresetSize, ChartPresetBarClustered03));
			list.Add(new ImageInfo(ChartPresetBarClustered04, imageFlags, DefaultPresetSize, ChartPresetBarClustered04));
			list.Add(new ImageInfo(ChartPresetBarClustered05, imageFlags, DefaultPresetSize, ChartPresetBarClustered05));
			list.Add(new ImageInfo(ChartPresetBarClustered06, imageFlags, DefaultPresetSize, ChartPresetBarClustered06));
			list.Add(new ImageInfo(ChartPresetBarClustered07, imageFlags, DefaultPresetSize, ChartPresetBarClustered07));
			list.Add(new ImageInfo(ChartPresetBarClustered08, imageFlags, DefaultPresetSize, ChartPresetBarClustered08));
			list.Add(new ImageInfo(ChartPresetBarClustered09, imageFlags, DefaultPresetSize, ChartPresetBarClustered09));
			list.Add(new ImageInfo(ChartPresetBarClustered10, imageFlags, DefaultPresetSize, ChartPresetBarClustered10));
			list.Add(new ImageInfo(ChartPresetBarStacked01, imageFlags, DefaultPresetSize, ChartPresetBarStacked01));
			list.Add(new ImageInfo(ChartPresetBarStacked02, imageFlags, DefaultPresetSize, ChartPresetBarStacked02));
			list.Add(new ImageInfo(ChartPresetBarStacked03, imageFlags, DefaultPresetSize, ChartPresetBarStacked03));
			list.Add(new ImageInfo(ChartPresetBarStacked04, imageFlags, DefaultPresetSize, ChartPresetBarStacked04));
			list.Add(new ImageInfo(ChartPresetBarStacked05, imageFlags, DefaultPresetSize, ChartPresetBarStacked05));
			list.Add(new ImageInfo(ChartPresetBarStacked06, imageFlags, DefaultPresetSize, ChartPresetBarStacked06));
			list.Add(new ImageInfo(ChartPresetBarStacked07, imageFlags, DefaultPresetSize, ChartPresetBarStacked07));
			list.Add(new ImageInfo(ChartPresetBarStacked08, imageFlags, DefaultPresetSize, ChartPresetBarStacked08));
			list.Add(new ImageInfo(ChartPresetBarStacked09, imageFlags, DefaultPresetSize, ChartPresetBarStacked09));
			list.Add(new ImageInfo(ChartPresetBarStacked10, imageFlags, DefaultPresetSize, ChartPresetBarStacked10));
			list.Add(new ImageInfo(ChartPresetColumnClustered01, imageFlags, DefaultPresetSize, ChartPresetColumnClustered01));
			list.Add(new ImageInfo(ChartPresetColumnClustered02, imageFlags, DefaultPresetSize, ChartPresetColumnClustered02));
			list.Add(new ImageInfo(ChartPresetColumnClustered03, imageFlags, DefaultPresetSize, ChartPresetColumnClustered03));
			list.Add(new ImageInfo(ChartPresetColumnClustered04, imageFlags, DefaultPresetSize, ChartPresetColumnClustered04));
			list.Add(new ImageInfo(ChartPresetColumnClustered05, imageFlags, DefaultPresetSize, ChartPresetColumnClustered05));
			list.Add(new ImageInfo(ChartPresetColumnClustered06, imageFlags, DefaultPresetSize, ChartPresetColumnClustered06));
			list.Add(new ImageInfo(ChartPresetColumnClustered07, imageFlags, DefaultPresetSize, ChartPresetColumnClustered07));
			list.Add(new ImageInfo(ChartPresetColumnClustered08, imageFlags, DefaultPresetSize, ChartPresetColumnClustered08));
			list.Add(new ImageInfo(ChartPresetColumnClustered09, imageFlags, DefaultPresetSize, ChartPresetColumnClustered09));
			list.Add(new ImageInfo(ChartPresetColumnClustered10, imageFlags, DefaultPresetSize, ChartPresetColumnClustered10));
			list.Add(new ImageInfo(ChartPresetColumnClustered11, imageFlags, DefaultPresetSize, ChartPresetColumnClustered11));
			list.Add(new ImageInfo(ChartPresetColumnStacked01, imageFlags, DefaultPresetSize, ChartPresetColumnStacked01));
			list.Add(new ImageInfo(ChartPresetColumnStacked02, imageFlags, DefaultPresetSize, ChartPresetColumnStacked02));
			list.Add(new ImageInfo(ChartPresetColumnStacked03, imageFlags, DefaultPresetSize, ChartPresetColumnStacked03));
			list.Add(new ImageInfo(ChartPresetColumnStacked04, imageFlags, DefaultPresetSize, ChartPresetColumnStacked04));
			list.Add(new ImageInfo(ChartPresetColumnStacked05, imageFlags, DefaultPresetSize, ChartPresetColumnStacked05));
			list.Add(new ImageInfo(ChartPresetColumnStacked06, imageFlags, DefaultPresetSize, ChartPresetColumnStacked06));
			list.Add(new ImageInfo(ChartPresetColumnStacked07, imageFlags, DefaultPresetSize, ChartPresetColumnStacked07));
			list.Add(new ImageInfo(ChartPresetColumnStacked08, imageFlags, DefaultPresetSize, ChartPresetColumnStacked08));
			list.Add(new ImageInfo(ChartPresetColumnStacked09, imageFlags, DefaultPresetSize, ChartPresetColumnStacked09));
			list.Add(new ImageInfo(ChartPresetColumnStacked10, imageFlags, DefaultPresetSize, ChartPresetColumnStacked10));
			list.Add(new ImageInfo(ChartPresetDoughnut01, imageFlags, DefaultPresetSize, ChartPresetDoughnut01));
			list.Add(new ImageInfo(ChartPresetDoughnut02, imageFlags, DefaultPresetSize, ChartPresetDoughnut02));
			list.Add(new ImageInfo(ChartPresetDoughnut03, imageFlags, DefaultPresetSize, ChartPresetDoughnut03));
			list.Add(new ImageInfo(ChartPresetDoughnut04, imageFlags, DefaultPresetSize, ChartPresetDoughnut04));
			list.Add(new ImageInfo(ChartPresetDoughnut05, imageFlags, DefaultPresetSize, ChartPresetDoughnut05));
			list.Add(new ImageInfo(ChartPresetDoughnut06, imageFlags, DefaultPresetSize, ChartPresetDoughnut06));
			list.Add(new ImageInfo(ChartPresetDoughnut07, imageFlags, DefaultPresetSize, ChartPresetDoughnut07));
			list.Add(new ImageInfo(ChartPresetLine01, imageFlags, DefaultPresetSize, ChartPresetLine01));
			list.Add(new ImageInfo(ChartPresetLine02, imageFlags, DefaultPresetSize, ChartPresetLine02));
			list.Add(new ImageInfo(ChartPresetLine03, imageFlags, DefaultPresetSize, ChartPresetLine03));
			list.Add(new ImageInfo(ChartPresetLine04, imageFlags, DefaultPresetSize, ChartPresetLine04));
			list.Add(new ImageInfo(ChartPresetLine05, imageFlags, DefaultPresetSize, ChartPresetLine05));
			list.Add(new ImageInfo(ChartPresetLine06, imageFlags, DefaultPresetSize, ChartPresetLine06));
			list.Add(new ImageInfo(ChartPresetLine07, imageFlags, DefaultPresetSize, ChartPresetLine07));
			list.Add(new ImageInfo(ChartPresetLine08, imageFlags, DefaultPresetSize, ChartPresetLine08));
			list.Add(new ImageInfo(ChartPresetLine09, imageFlags, DefaultPresetSize, ChartPresetLine09));
			list.Add(new ImageInfo(ChartPresetLine10, imageFlags, DefaultPresetSize, ChartPresetLine10));
			list.Add(new ImageInfo(ChartPresetLine11, imageFlags, DefaultPresetSize, ChartPresetLine11));
			list.Add(new ImageInfo(ChartPresetLine12, imageFlags, DefaultPresetSize, ChartPresetLine12));
			list.Add(new ImageInfo(ChartPresetPie01, imageFlags, DefaultPresetSize, ChartPresetPie01));
			list.Add(new ImageInfo(ChartPresetPie02, imageFlags, DefaultPresetSize, ChartPresetPie02));
			list.Add(new ImageInfo(ChartPresetPie03, imageFlags, DefaultPresetSize, ChartPresetPie03));
			list.Add(new ImageInfo(ChartPresetPie04, imageFlags, DefaultPresetSize, ChartPresetPie04));
			list.Add(new ImageInfo(ChartPresetPie05, imageFlags, DefaultPresetSize, ChartPresetPie05));
			list.Add(new ImageInfo(ChartPresetPie06, imageFlags, DefaultPresetSize, ChartPresetPie06));
			list.Add(new ImageInfo(ChartPresetPie07, imageFlags, DefaultPresetSize, ChartPresetPie07));
			list.Add(new ImageInfo(ChartPresetRadar01, imageFlags, DefaultPresetSize, ChartPresetRadar01));
			list.Add(new ImageInfo(ChartPresetRadar02, imageFlags, DefaultPresetSize, ChartPresetRadar02));
			list.Add(new ImageInfo(ChartPresetRadar03, imageFlags, DefaultPresetSize, ChartPresetRadar03));
			list.Add(new ImageInfo(ChartPresetRadar04, imageFlags, DefaultPresetSize, ChartPresetRadar04));
			list.Add(new ImageInfo(ChartPresetScatter01, imageFlags, DefaultPresetSize, ChartPresetScatter01));
			list.Add(new ImageInfo(ChartPresetScatter02, imageFlags, DefaultPresetSize, ChartPresetScatter02));
			list.Add(new ImageInfo(ChartPresetScatter03, imageFlags, DefaultPresetSize, ChartPresetScatter03));
			list.Add(new ImageInfo(ChartPresetScatter04, imageFlags, DefaultPresetSize, ChartPresetScatter04));
			list.Add(new ImageInfo(ChartPresetScatter05, imageFlags, DefaultPresetSize, ChartPresetScatter05));
			list.Add(new ImageInfo(ChartPresetScatter06, imageFlags, DefaultPresetSize, ChartPresetScatter06));
			list.Add(new ImageInfo(ChartPresetScatter07, imageFlags, DefaultPresetSize, ChartPresetScatter07));
			list.Add(new ImageInfo(ChartPresetScatter08, imageFlags, DefaultPresetSize, ChartPresetScatter08));
			list.Add(new ImageInfo(ChartPresetScatter09, imageFlags, DefaultPresetSize, ChartPresetScatter09));
			list.Add(new ImageInfo(ChartPresetScatter10, imageFlags, DefaultPresetSize, ChartPresetScatter10));
			list.Add(new ImageInfo(ChartPresetScatter11, imageFlags, DefaultPresetSize, ChartPresetScatter11));
			list.Add(new ImageInfo(ChartPresetStock01, imageFlags, DefaultPresetSize, ChartPresetStock01));
			list.Add(new ImageInfo(ChartPresetStock02, imageFlags, DefaultPresetSize, ChartPresetStock02));
			list.Add(new ImageInfo(ChartPresetStock03, imageFlags, DefaultPresetSize, ChartPresetStock03));
			list.Add(new ImageInfo(ChartPresetStock04, imageFlags, DefaultPresetSize, ChartPresetStock04));
			list.Add(new ImageInfo(ChartPresetStock05, imageFlags, DefaultPresetSize, ChartPresetStock05));
			list.Add(new ImageInfo(ChartPresetStock06, imageFlags, DefaultPresetSize, ChartPresetStock06));
			list.Add(new ImageInfo(ChartPresetStock07, imageFlags, DefaultPresetSize, ChartPresetStock07));
			list.Add(new ImageInfo(ChartSwitchRowColumn, imageFlags, DefaultSmallSize, ChartSwitchRowColumn));
			list.Add(new ImageInfo(FreezeFirstColumn, imageFlags, DefaultSmallSize, FreezeFirstColumn));
			list.Add(new ImageInfo(FreezeFirstColumnLarge, imageFlags, DefaultLargeSize, FreezeFirstColumnLarge));
			list.Add(new ImageInfo(FreezePanes, imageFlags, DefaultSmallSize, FreezePanes));
			list.Add(new ImageInfo(FreezePanesLarge, imageFlags, DefaultLargeSize, FreezePanesLarge));
			list.Add(new ImageInfo(FreezeTopRow, imageFlags, DefaultSmallSize, FreezeTopRow));
			list.Add(new ImageInfo(FreezeTopRowLarge, imageFlags, DefaultLargeSize, FreezeTopRowLarge));
			list.Add(new ImageInfo(UnfreezePanes, imageFlags, DefaultSmallSize, UnfreezePanes));
			list.Add(new ImageInfo(UnfreezePanesLarge, imageFlags, DefaultLargeSize, UnfreezePanesLarge));
			list.Add(new ImageInfo(ChartAxesGroup, imageFlags, DefaultSmallSize, ChartAxesGroup));
			list.Add(new ImageInfo(ChartAxisTitleGroup, imageFlags, DefaultSmallSize, ChartAxisTitleGroup));
			list.Add(new ImageInfo(ChartAxisTitleHorizontal, imageFlags, DefaultSmallSize, ChartAxisTitleHorizontal));
			list.Add(new ImageInfo(ChartAxisTitleHorizontal_None, imageFlags, DefaultSmallSize, ChartAxisTitleHorizontal_None));
			list.Add(new ImageInfo(ChartAxisTitleVertical, imageFlags, DefaultSmallSize, ChartAxisTitleVertical));
			list.Add(new ImageInfo(ChartAxisTitleVertical_HorizonlalText, imageFlags, DefaultSmallSize, ChartAxisTitleVertical_HorizonlalText));
			list.Add(new ImageInfo(ChartAxisTitleVertical_None, imageFlags, DefaultSmallSize, ChartAxisTitleVertical_None));
			list.Add(new ImageInfo(ChartAxisTitleVertical_RotatedText, imageFlags, DefaultSmallSize, ChartAxisTitleVertical_RotatedText));
			list.Add(new ImageInfo(ChartAxisTitleVertical_VerticalText, imageFlags, DefaultSmallSize, ChartAxisTitleVertical_VerticalText));
			list.Add(new ImageInfo(ChartHorizontalAxis_Billions, imageFlags, DefaultSmallSize, ChartHorizontalAxis_Billions));
			list.Add(new ImageInfo(ChartHorizontalAxis_Default, imageFlags, DefaultSmallSize, ChartHorizontalAxis_Default));
			list.Add(new ImageInfo(ChartHorizontalAxis_LeftToRight, imageFlags, DefaultSmallSize, ChartHorizontalAxis_LeftToRight));
			list.Add(new ImageInfo(ChartHorizontalAxis_LogScale, imageFlags, DefaultSmallSize, ChartHorizontalAxis_LogScale));
			list.Add(new ImageInfo(ChartHorizontalAxis_Millions, imageFlags, DefaultSmallSize, ChartHorizontalAxis_Millions));
			list.Add(new ImageInfo(ChartHorizontalAxis_None, imageFlags, DefaultSmallSize, ChartHorizontalAxis_None));
			list.Add(new ImageInfo(ChartHorizontalAxis_RightToLeft, imageFlags, DefaultSmallSize, ChartHorizontalAxis_RightToLeft));
			list.Add(new ImageInfo(ChartHorizontalAxis_Thousands, imageFlags, DefaultSmallSize, ChartHorizontalAxis_Thousands));
			list.Add(new ImageInfo(ChartHorizontalAxis_WithoutLabeling, imageFlags, DefaultSmallSize, ChartHorizontalAxis_WithoutLabeling));
			list.Add(new ImageInfo(ChartLabels_InsideBase, imageFlags, DefaultSmallSize, ChartLabels_InsideBase));
			list.Add(new ImageInfo(ChartLabels_InsideCenter, imageFlags, DefaultSmallSize, ChartLabels_InsideCenter));
			list.Add(new ImageInfo(ChartLabels_InsideEnd, imageFlags, DefaultSmallSize, ChartLabels_InsideEnd));
			list.Add(new ImageInfo(ChartLabels_LineAbove, imageFlags, DefaultSmallSize, ChartLabels_LineAbove));
			list.Add(new ImageInfo(ChartLabels_LineBelow, imageFlags, DefaultSmallSize, ChartLabels_LineBelow));
			list.Add(new ImageInfo(ChartLabels_LineCenter, imageFlags, DefaultSmallSize, ChartLabels_LineCenter));
			list.Add(new ImageInfo(ChartLabels_LineLeft, imageFlags, DefaultSmallSize, ChartLabels_LineLeft));
			list.Add(new ImageInfo(ChartLabels_LineNone, imageFlags, DefaultSmallSize, ChartLabels_LineNone));
			list.Add(new ImageInfo(ChartLabels_LineRight, imageFlags, DefaultSmallSize, ChartLabels_LineRight));
			list.Add(new ImageInfo(ChartLabels_None, imageFlags, DefaultSmallSize, ChartLabels_None));
			list.Add(new ImageInfo(ChartLabels_OutsideEnd, imageFlags, DefaultSmallSize, ChartLabels_OutsideEnd));
			list.Add(new ImageInfo(ChartLabels_Show, imageFlags, DefaultSmallSize, ChartLabels_Show));
			list.Add(new ImageInfo(ChartLegend_Bottom, imageFlags, DefaultSmallSize, ChartLegend_Bottom));
			list.Add(new ImageInfo(ChartLegend_Left, imageFlags, DefaultSmallSize, ChartLegend_Left));
			list.Add(new ImageInfo(ChartLegend_LeftOverlay, imageFlags, DefaultSmallSize, ChartLegend_LeftOverlay));
			list.Add(new ImageInfo(ChartLegend_None, imageFlags, DefaultSmallSize, ChartLegend_None));
			list.Add(new ImageInfo(ChartLegend_Right, imageFlags, DefaultSmallSize, ChartLegend_Right));
			list.Add(new ImageInfo(ChartLegend_RightOverlay, imageFlags, DefaultSmallSize, ChartLegend_RightOverlay));
			list.Add(new ImageInfo(ChartLegend_Top, imageFlags, DefaultSmallSize, ChartLegend_Top));
			list.Add(new ImageInfo(ChartTitleAbove, imageFlags, DefaultSmallSize, ChartTitleAbove));
			list.Add(new ImageInfo(ChartTitleCenteredOverlay, imageFlags, DefaultSmallSize, ChartTitleCenteredOverlay));
			list.Add(new ImageInfo(ChartTitleNone, imageFlags, DefaultSmallSize, ChartTitleNone));
			list.Add(new ImageInfo(ChartVerticalAxis_Billions, imageFlags, DefaultSmallSize, ChartVerticalAxis_Billions));
			list.Add(new ImageInfo(ChartVerticalAxis_BottomUp, imageFlags, DefaultSmallSize, ChartVerticalAxis_BottomUp));
			list.Add(new ImageInfo(ChartVerticalAxis_Default, imageFlags, DefaultSmallSize, ChartVerticalAxis_Default));
			list.Add(new ImageInfo(ChartVerticalAxis_LogScale, imageFlags, DefaultSmallSize, ChartVerticalAxis_LogScale));
			list.Add(new ImageInfo(ChartVerticalAxis_Millions, imageFlags, DefaultSmallSize, ChartVerticalAxis_Millions));
			list.Add(new ImageInfo(ChartVerticalAxis_None, imageFlags, DefaultSmallSize, ChartVerticalAxis_None));
			list.Add(new ImageInfo(ChartVerticalAxis_Thousands, imageFlags, DefaultSmallSize, ChartVerticalAxis_Thousands));
			list.Add(new ImageInfo(ChartVerticalAxis_TopDown, imageFlags, DefaultSmallSize, ChartVerticalAxis_TopDown));
			list.Add(new ImageInfo(ChartVerticallAxis_WithoutLabeling, imageFlags, DefaultSmallSize, ChartVerticallAxis_WithoutLabeling));
			list.Add(new ImageInfo(ChartGridlines, imageFlags, DefaultSmallSize, ChartGridlines));
			list.Add(new ImageInfo(ChartGridlinesHorizontal_Major, imageFlags, DefaultSmallSize, ChartGridlinesHorizontal_Major));
			list.Add(new ImageInfo(ChartGridlinesHorizontal_MajorMinor, imageFlags, DefaultSmallSize, ChartGridlinesHorizontal_MajorMinor));
			list.Add(new ImageInfo(ChartGridlinesHorizontal_Minor, imageFlags, DefaultSmallSize, ChartGridlinesHorizontal_Minor));
			list.Add(new ImageInfo(ChartGridlinesHorizontal_None, imageFlags, DefaultSmallSize, ChartGridlinesHorizontal_None));
			list.Add(new ImageInfo(ChartGridlinesVertical_Major, imageFlags, DefaultSmallSize, ChartGridlinesVertical_Major));
			list.Add(new ImageInfo(ChartGridlinesVertical_MajorMinor, imageFlags, DefaultSmallSize, ChartGridlinesVertical_MajorMinor));
			list.Add(new ImageInfo(ChartGridlinesVertical_Minor, imageFlags, DefaultSmallSize, ChartGridlinesVertical_Minor));
			list.Add(new ImageInfo(ChartGridlinesVertical_None, imageFlags, DefaultSmallSize, ChartGridlinesVertical_None));
			list.Add(new ImageInfo(DropDownButton, imageFlags, DefaultSmallSize, DropDownButton));
			list.Add(new ImageInfo(AutoFilter_Filtered, imageFlags, DefaultSmallSize, AutoFilter_Filtered));
			list.Add(new ImageInfo(AutoFilter_Ascending, imageFlags, DefaultSmallSize, AutoFilter_Ascending));
			list.Add(new ImageInfo(AutoFilter_Descending, imageFlags, DefaultSmallSize, AutoFilter_Descending));
			list.Add(new ImageInfo(AutoFilter_FilteredAndAscending, imageFlags, DefaultSmallSize, AutoFilter_FilteredAndAscending));
			list.Add(new ImageInfo(AutoFilter_FilteredAndDescending, imageFlags, DefaultSmallSize, AutoFilter_FilteredAndDescending));
			list.Add(new ImageInfo(SortAsc, imageFlags, DefaultSmallSize, SortAsc));
			list.Add(new ImageInfo(SortDesc, imageFlags, DefaultSmallSize, SortDesc));
			list.Add(new ImageInfo(ClearFilter, imageFlags, DefaultSmallSize, ClearFilter));
			list.Add(new ImageInfo(InsertSheet, imageFlags, DefaultSmallSize, InsertSheet));
			list.Add(new ImageInfo(RemoveSheet, imageFlags, DefaultSmallSize, RemoveSheet));
			list.Add(new ImageInfo(FormulaBarCancelButton, imageFlags | ImageFlags.HasDisabledState, DefaultSmallSize, FormulaBarCancelButton));
			list.Add(new ImageInfo(FormulaBarEnterButton, imageFlags | ImageFlags.HasDisabledState, DefaultSmallSize, FormulaBarEnterButton));
			#endregion
		}
	}
	public class SpreadsheetFileManagerImages : FileManagerImages {
		public SpreadsheetFileManagerImages(ISkinOwner skinOwner) : base(skinOwner) { }
	}
}
