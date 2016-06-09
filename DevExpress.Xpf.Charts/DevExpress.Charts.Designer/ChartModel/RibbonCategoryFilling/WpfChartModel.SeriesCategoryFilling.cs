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

using System.Collections.Generic;
using System.Windows.Data;
using DevExpress.Utils;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Charts;
namespace DevExpress.Charts.Designer.Native {
	public sealed partial class WpfChartModel : ChartModelElement {
		RibbonPageCategoryViewModel CreateSeriesCategoryModel() {
			RibbonPageCategoryViewModel seriesCategory = new RibbonPageCategoryViewModel(this, typeof(WpfChartSeriesModel));
			seriesCategory.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptionsRibbonCategoryTitle);
			seriesCategory.Pages.Add(CreateSeriesOptionsPage());
			seriesCategory.Pages.Add(CreateSeriesDataPage());
			return seriesCategory; 
		}
		#region Series Options page
		RibbonPageViewModel CreateSeriesOptionsPage() {
			RibbonPageViewModel seriesOptionsPage = new RibbonPageViewModel(this, RibbonPagesNames.SeriesOptionsMainPage);
			seriesOptionsPage.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_SelectedSeries);
			seriesOptionsPage.Groups.Add(CreateSeriesOptionsPageMainGroup());
			seriesOptionsPage.Groups.Add(CreateSeriesOptionsPageLabelsGroup());
			seriesOptionsPage.Groups.Add(GenerageMarkerOptions());
			seriesOptionsPage.Groups.Add(CreateSeriesOptionsPageLayoutGroup());
			seriesOptionsPage.Groups.Add(CreateSeriesOptionsPageAddIndicatorGroup());
			seriesOptionsPage.Groups.Add(CreateSeriesOptionsChangeSeriesViewGroup());
			seriesOptionsPage.Groups.Add(CreateSeriesOptionsPieDoughnutOptionsGroup());
			seriesOptionsPage.Groups.Add(CreateSeriesOptionsFunnelOptionsGroup());
			seriesOptionsPage.Groups.Add(CreateNestedDonut2DOptionsGroup());
			return seriesOptionsPage;
		}
		#region Main group
		RibbonPageGroupViewModel CreateSeriesOptionsPageMainGroup() {
			RibbonPageGroupViewModel mainGroup = new RibbonPageGroupViewModel();
			mainGroup.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Main);
			mainGroup.BarItems.Add(new BarStaticTextViewModel(ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_SeriesName)));
			mainGroup.BarItems.Add(CreateSeriesNameEditor());
			return mainGroup;
		}
		RibbonItemViewModelBase CreateSeriesNameEditor() {
			var command = new ChangeSeriesNameCommand(this);
			IValueConverter converter = new SeriesNameConverter();
			var editor = new BarEditValueItemViewModel(command, this, BindingPathToSelectedModel, converter);
			return editor;
		}
		#endregion   
		#region Labels group
		RibbonPageGroupViewModel CreateSeriesOptionsPageLabelsGroup() {
			var labelsGroup = new RibbonPageGroupViewModel();
			labelsGroup.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_LabelsGroupTitle);
			labelsGroup.BarItems.Add(CreateLabelOrientationGallery());
			labelsGroup.BarItems.Add(CreateSeriesLabelConnectorThicknessEditor());
			labelsGroup.BarItems.Add(CreateSeriesLabelIndentEditor());
			return labelsGroup;
		}
		RibbonItemViewModelBase CreateSeriesLabelConnectorThicknessEditor() {
			var command = new SetSeriesLabelConnectorThicknessCommand(this);
			IValueConverter converter = new SeriesLabelConectorThicknessConverter();
			int startThickness = 0;
			int step = 1;
			int endThickness = 10;
			var editor = new BarThicknessEditViewModel(command, this, BindingPathToSelectedModel, startThickness, step, endThickness, converter);
			editor.EditWidth = 60;
			return editor;
		}
		RibbonItemViewModelBase CreateSeriesLabelIndentEditor() {
			var command = new SetSeriesLabelConnectorIndentCommand(this);
			IValueConverter converter = new SeriesLabelIndentConverter();
			var editor = new BarSpinEditViewModel(command, this, BindingPathToSelectedModel, converter);
			editor.EditValue = 0;
			editor.EditWidth = 60;
			editor.MaxValue = 1000;
			return editor;
		}
		RibbonItemViewModelBase CreateLabelOrientationGallery() {
			var gallery = new BarDropDownButtonGalleryViewModel();
			gallery.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_SeriesLabelsPosition);
			gallery.IsItemCaptionVisible = true;
			gallery.IsGroupCaptionVisible = DefaultBoolean.False;
			gallery.IsItemDescriptionVisible = true;
			gallery.InitialVisibleColCount = 1;
			gallery.InitialVisibleRowCount = 20;
			gallery.ItemCheckMode = GalleryItemCheckMode.Single;
			var group = new GalleryGroupViewModel();
			gallery.Groups.Add(group);
			const bool falseBindingFailureValue = false;
			var hideWhenDiabled = new HideBehavior();
			group.Items.Add(new BarEditValueItemViewModel(new SetNoneLabelPositionCommand(this), this, BindingPathToSelectedModel, falseBindingFailureValue, hideWhenDiabled));
			group.Items.Add(new BarEditValueItemViewModel(new SetFinancialSeriesLabelEnabledCommand(this), this, BindingPathToSelectedModel, falseBindingFailureValue, hideWhenDiabled));
			group.Items.Add(new BarEditValueItemViewModel(new SetBarSeries3DLabelEnabledCommand(this), this, BindingPathToSelectedModel, falseBindingFailureValue, hideWhenDiabled));
			group.Items.Add(new BarEditValueItemViewModel(new SetArea3DLabelEnabledCommand(this), this, BindingPathToSelectedModel, falseBindingFailureValue, hideWhenDiabled));
			group.Items.Add(new BarEditValueItemViewModel(new SetBar2DCenterLabelPositionCommand(this), this, BindingPathToSelectedModel, falseBindingFailureValue, hideWhenDiabled));
			group.Items.Add(new BarEditValueItemViewModel(new SetBar2DOutsideLabelPositionCommand(this), this, BindingPathToSelectedModel, falseBindingFailureValue, hideWhenDiabled));
			group.Items.Add(new BarEditValueItemViewModel(new SetPieInsideLabelPositionCommand(this), this, BindingPathToSelectedModel, falseBindingFailureValue, hideWhenDiabled));
			group.Items.Add(new BarEditValueItemViewModel(new SetPieOutsideLabelPositionCommand(this), this, BindingPathToSelectedModel, falseBindingFailureValue, hideWhenDiabled));
			group.Items.Add(new BarEditValueItemViewModel(new SetPieTwoColumnsLabelPositionCommand(this), this, BindingPathToSelectedModel, falseBindingFailureValue, hideWhenDiabled));
			group.Items.Add(new BarEditValueItemViewModel(new SetFunnelLeftLabelPositionCommand(this), this, BindingPathToSelectedModel, falseBindingFailureValue, hideWhenDiabled));
			group.Items.Add(new BarEditValueItemViewModel(new SetFunnelLeftColumnLabelPositionCommand(this), this, BindingPathToSelectedModel, falseBindingFailureValue, hideWhenDiabled));
			group.Items.Add(new BarEditValueItemViewModel(new SetFunnelRightLabelPositionCommand(this), this, BindingPathToSelectedModel, falseBindingFailureValue, hideWhenDiabled));
			group.Items.Add(new BarEditValueItemViewModel(new SetFunnelRightColumnLabelPositionCommand(this), this, BindingPathToSelectedModel, falseBindingFailureValue, hideWhenDiabled));
			group.Items.Add(new BarEditValueItemViewModel(new SetFunnelCenterLabelPositionCommand(this), this, BindingPathToSelectedModel, falseBindingFailureValue, hideWhenDiabled));
			group.Items.Add(new BarEditValueItemViewModel(new SetBubble2DCenterLabelPosition(this), this, BindingPathToSelectedModel, falseBindingFailureValue, hideWhenDiabled));
			group.Items.Add(new BarEditValueItemViewModel(new SetMarker3DCenterLabelPosition(this), this, BindingPathToSelectedModel, falseBindingFailureValue, hideWhenDiabled));
			group.Items.Add(new BarEditValueItemViewModel(new SetMarker2DLabelPositionAtAngle0(this), this, BindingPathToSelectedModel, falseBindingFailureValue, hideWhenDiabled));
			group.Items.Add(new BarEditValueItemViewModel(new SetLabelPositionAtAngle45(this), this, BindingPathToSelectedModel, falseBindingFailureValue, hideWhenDiabled));
			group.Items.Add(new BarEditValueItemViewModel(new SetMarker3DTopLabelPosition(this), this, BindingPathToSelectedModel, falseBindingFailureValue, hideWhenDiabled));
			group.Items.Add(new BarEditValueItemViewModel(new SetLabelPositionAtAngle90(this), this, BindingPathToSelectedModel, falseBindingFailureValue, hideWhenDiabled));
			group.Items.Add(new BarEditValueItemViewModel(new SetStackedBar2DCenterLabelPosition(this), this, BindingPathToSelectedModel, falseBindingFailureValue, hideWhenDiabled));
			group.Items.Add(new BarEditValueItemViewModel(new SetLabelPositionAtAngle135(this), this, BindingPathToSelectedModel, falseBindingFailureValue, hideWhenDiabled));
			group.Items.Add(new BarEditValueItemViewModel(new SetLabelPositionAtAngle180(this), this, BindingPathToSelectedModel, falseBindingFailureValue, hideWhenDiabled));
			group.Items.Add(new BarEditValueItemViewModel(new SetLabelPositionAtAngle225(this), this, BindingPathToSelectedModel, falseBindingFailureValue, hideWhenDiabled));
			group.Items.Add(new BarEditValueItemViewModel(new SetPositionAtAngle270(this), this, BindingPathToSelectedModel, falseBindingFailureValue, hideWhenDiabled));
			group.Items.Add(new BarEditValueItemViewModel(new SetLabelPositionAtAngle315(this), this, BindingPathToSelectedModel, falseBindingFailureValue, hideWhenDiabled));
			group.Items.Add(new BarEditValueItemViewModel(new SetRangeBarMinValueLabelPositionCommand(this), this, BindingPathToSelectedModel, falseBindingFailureValue, hideWhenDiabled));
			group.Items.Add(new BarEditValueItemViewModel(new SetRangeBarOneLabelPositionCommand(this), this, BindingPathToSelectedModel, falseBindingFailureValue, hideWhenDiabled));
			group.Items.Add(new BarEditValueItemViewModel(new SetRangeBarTwoLabelsLabelPositionCommand(this), this, BindingPathToSelectedModel, falseBindingFailureValue, hideWhenDiabled));
			group.Items.Add(new BarEditValueItemViewModel(new SetRangeArea2DMaxValueLabelPositionCommand(this), this, BindingPathToSelectedModel, falseBindingFailureValue, hideWhenDiabled));
			group.Items.Add(new BarEditValueItemViewModel(new SetRangeArea2DMinValueLabelPositionCommand(this), this, BindingPathToSelectedModel, falseBindingFailureValue, hideWhenDiabled));
			group.Items.Add(new BarEditValueItemViewModel(new SetRangeArea2DOneLabelPositionCommand(this), this, BindingPathToSelectedModel, falseBindingFailureValue, hideWhenDiabled));
			group.Items.Add(new BarEditValueItemViewModel(new SetRangeArea2DTwoLabelsPositionCommand(this), this, BindingPathToSelectedModel, falseBindingFailureValue, hideWhenDiabled));
			group.Items.Add(new BarEditValueItemViewModel(new SetRangeArea2DValue1LabelPositionCommand(this), this, BindingPathToSelectedModel, falseBindingFailureValue, hideWhenDiabled));
			group.Items.Add(new BarEditValueItemViewModel(new SetRangeArea2DValue2LabelPositionCommand(this), this, BindingPathToSelectedModel, falseBindingFailureValue, hideWhenDiabled));
			return gallery;
		}
		#endregion
		#region Markers group
		RibbonPageGroupViewModel GenerageMarkerOptions() {
			var markersGroup = new RibbonPageGroupViewModel(hideGroupIfAllCommandsAreDisabled: true);
			markersGroup.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_MarkersCaption);
			var gallery = new BarDropDownButtonGalleryViewModel();
			gallery.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Model);
			gallery.IsItemCaptionVisible = true;
			gallery.IsGroupCaptionVisible = DefaultBoolean.False;
			gallery.ImageName = GlyphUtils.GalleryItemImages + "SeriesPointAndSeriesModels\\Models";
			gallery.InitialVisibleColCount = 1;
			gallery.ItemCheckMode = GalleryItemCheckMode.Single;
			gallery.Groups.Add(CreateNoModelGroup());
			gallery.Groups.Add(CreateBar2DModelsGroup());
			gallery.Groups.Add(CreateBar3DModelsGroup());
			gallery.Groups.Add(CreateRangeBar2DModelsGroup());
			gallery.Groups.Add(CreateMarker2DModelsGroup());
			gallery.Groups.Add(CreateMarker3DModelsGroup());
			gallery.Groups.Add(CreateStock2DModelsGroup());
			gallery.Groups.Add(CreateCandleStick2DModelsGroup());
			gallery.Groups.Add(CreatePie2DModelsGroup());
			gallery.Groups.Add(CreatePie3DModelsGroup());
			markersGroup.BarItems.Add(gallery);
			return markersGroup;
		}
		GalleryGroupViewModel CreateNoModelGroup() {
			GalleryGroupViewModel group = new GalleryGroupViewModel(hideGroupIfAllItemsAreDisabled: true);
			group.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_NoModelString);
			group.Items.Add(new BarEditValueItemViewModel(new DisableMarkerCommand(this), this, BindingPathToSelectedModel));
			return group;
		}
		GalleryGroupViewModel CreatePie3DModelsGroup() {
			GalleryGroupViewModel group = new GalleryGroupViewModel(hideGroupIfAllItemsAreDisabled: true);
			var kinds = Pie3DModel.GetPredefinedKinds();
			foreach (Pie3DKind kind in kinds)
				group.Items.Add(new BarEditValueItemViewModel(new Pie3DModelCommand(this, kind), this, BindingPathToSelectedModel));
			return group;
		}
		GalleryGroupViewModel CreatePie2DModelsGroup() {
			GalleryGroupViewModel group = new GalleryGroupViewModel(hideGroupIfAllItemsAreDisabled: true);
			var kinds = Pie2DModel.GetPredefinedKinds();
			foreach (Pie2DKind kind in kinds)
				group.Items.Add(new BarEditValueItemViewModel(new Pie2DModelCommand(this, kind), this, BindingPathToSelectedModel));
			return group;
		}
		GalleryGroupViewModel CreateCandleStick2DModelsGroup() {
			GalleryGroupViewModel group = new GalleryGroupViewModel(hideGroupIfAllItemsAreDisabled: true);
			var kinds = CandleStick2DModel.GetPredefinedKinds();
			foreach (CandleStick2DKind kind in kinds)
				group.Items.Add(new BarEditValueItemViewModel(new CandleStick2DModelCommand(this, kind), this, BindingPathToSelectedModel));
			return group;
		}
		GalleryGroupViewModel CreateStock2DModelsGroup() {
			GalleryGroupViewModel group = new GalleryGroupViewModel(hideGroupIfAllItemsAreDisabled: true);
			var kinds = Stock2DModel.GetPredefinedKinds();
			foreach (Stock2DKind kind in kinds)
				group.Items.Add(new BarEditValueItemViewModel(new Stock2DModelCommand(this, kind), this, BindingPathToSelectedModel));
			return group;
		}
		GalleryGroupViewModel CreateMarker3DModelsGroup() {
			var group = new GalleryGroupViewModel(hideGroupIfAllItemsAreDisabled: true);
			var kinds = Marker3DModel.GetPredefinedKinds();
			foreach (Marker3DKind kind in kinds)
				group.Items.Add(new BarEditValueItemViewModel(new Marker3DModelCommand(this, kind), this, BindingPathToSelectedModel));
			return group;
		}
		GalleryGroupViewModel CreateMarker2DModelsGroup() {
			GalleryGroupViewModel group = new GalleryGroupViewModel(hideGroupIfAllItemsAreDisabled: true);
			group.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_MarkersCaption);
			var kinds = Marker2DModel.GetPredefinedKinds();
			foreach (Marker2DKind kind in kinds)
				group.Items.Add(new BarEditValueItemViewModel(new Marker2DModelCommand(this, kind), this, BindingPathToSelectedModel));
			return group;
		}
		GalleryGroupViewModel CreateBar3DModelsGroup() {
			GalleryGroupViewModel group = new GalleryGroupViewModel(hideGroupIfAllItemsAreDisabled: true);
			var kinds = Bar3DModel.GetPredefinedKinds();
			foreach (Bar3DKind kind in kinds)
				group.Items.Add(new BarEditValueItemViewModel(new Bar3DModelCommand(this, kind), this, BindingPathToSelectedModel));
			return group;
		}
		GalleryGroupViewModel CreateBar2DModelsGroup() {
			GalleryGroupViewModel group = new GalleryGroupViewModel(hideGroupIfAllItemsAreDisabled: true);
			var kinds = Bar2DModel.GetPredefinedKinds();
			foreach (Bar2DKind kind in kinds)
				group.Items.Add(new BarEditValueItemViewModel(new Bar2DModelCommand(this, kind), this, BindingPathToSelectedModel));
			return group;
		}
		GalleryGroupViewModel CreateRangeBar2DModelsGroup() {
			GalleryGroupViewModel group = new GalleryGroupViewModel(hideGroupIfAllItemsAreDisabled: true);
			var kinds = RangeBar2DModel.GetPredefinedKinds();
			foreach (RangeBar2DKind kind in kinds)
				group.Items.Add(new BarEditValueItemViewModel(new RangeBar2DModelCommand(this, kind), this, BindingPathToSelectedModel));
			return group;
		}
		#endregion
		#region Layout group
		RibbonPageGroupViewModel CreateSeriesOptionsPageLayoutGroup() {
			var layoutGroup = new RibbonPageGroupViewModel(hideGroupIfAllCommandsAreDisabled: true);
			layoutGroup.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_LayoutGroup);
			layoutGroup.BarItems.Add(CreateSeriesPaneEditor());
			layoutGroup.BarItems.Add(CreateSeriesAxisXEditor());
			layoutGroup.BarItems.Add(CreateSeriesAxisYEditor());
			return layoutGroup;
		}
		RibbonItemViewModelBase CreateSeriesPaneEditor() {
			var command = new ChangeSeriesPaneCommand(this);
			const string comboBoxItemsSourcePath = "DiagramModel.AllPanes";
			IValueConverter converter = new SeriesPaneConverter();
			var editor = new BarDynamicComboBoxViewModel(command, this, BindingPathToSelectedModel, comboBoxItemsSourcePath, converter);
			return editor;
		}
		RibbonItemViewModelBase CreateSeriesAxisXEditor() {
			var command = new ChangeSeriesAxisXCommand(this);
			const string comboBoxItemsSourcePath = "DiagramModel.AllAxesX";
			IValueConverter converter = new SeriesAxisXConverter();
			var editor = new BarDynamicComboBoxViewModel(command, this, BindingPathToSelectedModel, comboBoxItemsSourcePath, converter);
			return editor;
		}
		RibbonItemViewModelBase CreateSeriesAxisYEditor() {
			var command = new ChangeSeriesAxisYCommand(this);
			const string comboBoxItemsSourcePath = "DiagramModel.AllAxesY";
			IValueConverter converter = new SeriesAxisYConverter();
			var editor = new BarDynamicComboBoxViewModel(command, this, BindingPathToSelectedModel, comboBoxItemsSourcePath, converter);
			return editor;
		}
		#endregion
		#region Add Indicator group
		RibbonPageGroupViewModel CreateSeriesOptionsPageAddIndicatorGroup() {
			var indicatorsGroup = new RibbonPageGroupViewModel(hideGroupIfAllCommandsAreDisabled: true);
			indicatorsGroup.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_AddIndicatorsGroupTitle);
			indicatorsGroup.BarItems.Add(CreateGalleryForAddingIndicator());
			return indicatorsGroup;
		}
		#region CreateAddIndicatorGallery()
		BarSplitButtonGalleryViewModel CreateGalleryForAddingIndicator() {
			var gallery = new BarSplitButtonGalleryViewModel(new AddIndicatorCommand<RegressionLine>(this));
			gallery.IsGroupCaptionVisible = DefaultBoolean.True;
			gallery.Groups.Add(CreateSimpleIndicatorsGalleryGroupForSplitButtonGallery(gallery));
			gallery.Groups.Add(CreateFibonacciIndicatorsGalleryGroupForSplitButtonGallery(gallery));
			gallery.Groups.Add(CreateMovingAverageIndicatorsGalleryGroupForSplitButtonGallery(gallery));
			return gallery;
		}
		GalleryGroupViewModel CreateSimpleIndicatorsGalleryGroupForSplitButtonGallery(BarSplitButtonGalleryViewModel gallery) {
			GalleryGroupViewModel simpleIndicatorsGroup = new GalleryGroupViewModel();
			simpleIndicatorsGroup.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Elements_AddIndicator_SimpleIndicatorsGalleryGroup);
			simpleIndicatorsGroup.Items.Add(new SplitButtonGalleryItemViewModel(new AddIndicatorCommand<RegressionLine>(this), gallery));
			simpleIndicatorsGroup.Items.Add(new SplitButtonGalleryItemViewModel(new AddIndicatorCommand<TrendLine>(this), gallery));
			return simpleIndicatorsGroup;
		}
		GalleryGroupViewModel CreateFibonacciIndicatorsGalleryGroupForSplitButtonGallery(BarSplitButtonGalleryViewModel gallery) {
			GalleryGroupViewModel fibonacciIndicatorsGroup = new GalleryGroupViewModel();
			fibonacciIndicatorsGroup.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Elements_AddIndicator_FibonacciIndicatorsGalleryGroup);
			fibonacciIndicatorsGroup.Items.Add(new SplitButtonGalleryItemViewModel(new AddIndicatorCommand<FibonacciArcs>(this), gallery));
			fibonacciIndicatorsGroup.Items.Add(new SplitButtonGalleryItemViewModel(new AddIndicatorCommand<FibonacciFans>(this), gallery));
			fibonacciIndicatorsGroup.Items.Add(new SplitButtonGalleryItemViewModel(new AddIndicatorCommand<FibonacciRetracement>(this), gallery));
			return fibonacciIndicatorsGroup;
		}
		GalleryGroupViewModel CreateMovingAverageIndicatorsGalleryGroupForSplitButtonGallery(BarSplitButtonGalleryViewModel gallery) {
			GalleryGroupViewModel movingAverageIndicatorsGroup = new GalleryGroupViewModel();
			movingAverageIndicatorsGroup.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Elements_AddIndicator_MovingAverageIndicatorsGalleryGroup);
			movingAverageIndicatorsGroup.Items.Add(new SplitButtonGalleryItemViewModel(new AddIndicatorCommand<SimpleMovingAverage>(this), gallery));
			movingAverageIndicatorsGroup.Items.Add(new SplitButtonGalleryItemViewModel(new AddIndicatorCommand<WeightedMovingAverage>(this), gallery));
			movingAverageIndicatorsGroup.Items.Add(new SplitButtonGalleryItemViewModel(new AddIndicatorCommand<ExponentialMovingAverage>(this), gallery));
			movingAverageIndicatorsGroup.Items.Add(new SplitButtonGalleryItemViewModel(new AddIndicatorCommand<TriangularMovingAverage>(this), gallery));
			return movingAverageIndicatorsGroup;
		}
		#endregion //CreateAddIndicatorGallery()
		#endregion //Add Indicator group
		#region Change Series View group
		RibbonPageGroupViewModel CreateSeriesOptionsChangeSeriesViewGroup() {
			RibbonPageGroupViewModel mainGroup = new RibbonPageGroupViewModel(hideGroupIfAllCommandsAreDisabled: true);
			mainGroup.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_ChangeSeriesView);
			mainGroup.BarItems.Add(GenerateChangeSeriesViewGallery());
			return mainGroup;
		}
		RibbonItemViewModelBase GenerateChangeSeriesViewGallery() {
			var changeSeriesViewGallery = new InRibbonGalleryViewModel();
			changeSeriesViewGallery.ColCount = 7;
			ChangeSeriesViewGalleryCreator galleryCreator = new ChangeSeriesViewGalleryCreator(hideItemsWhenDesabled: true);
			galleryCreator.CreateSeriesViewGallery(this, changeSeriesViewGallery);
			return changeSeriesViewGallery;
		}
		#endregion
		#region Pie/Doughnut Options group
		RibbonPageGroupViewModel CreateSeriesOptionsPieDoughnutOptionsGroup() {
			RibbonPageGroupViewModel mainGroup = new RibbonPageGroupViewModel(hideGroupIfAllCommandsAreDisabled: true);
			mainGroup.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_PieDoughnutOptionsGroup);
			mainGroup.BarItems.Add(new BarStaticTextViewModel(ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_HoleRadiusPercent)));
			mainGroup.BarItems.Add(CreateHoleRadiusPercentEditor());
			return mainGroup;
		}
		RibbonItemViewModelBase CreateHoleRadiusPercentEditor() {
			var command = new ChangePieHoleRadiusPercent(this);
			IValueConverter converter = new SelectedModelToPieHoleRadiusPercent();
			var spin = new BarSpinEditViewModel(command, this, BindingPathToSelectedModel, converter);
			spin.MinValue = 0;
			spin.MaxValue = 100;
			spin.EditWidth = 75;
			spin.IsFloatValue = true;
			return spin;
		}
		#endregion
		#region Funnel Options group
		RibbonPageGroupViewModel CreateSeriesOptionsFunnelOptionsGroup() {
			RibbonPageGroupViewModel mainGroup = new RibbonPageGroupViewModel(hideGroupIfAllCommandsAreDisabled: true);
			mainGroup.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_FunnelOptionsGroup);
			mainGroup.BarItems.Add(new BarSeparatorViewModel());
			mainGroup.BarItems.Add(new BarCheckButtonViewModel(new ChangeFunnelAlignToCenterCommand(this), this, BindingPathToSelectedModel, new SelectedModelToFunnelAlignToCenterConverter()) { RibbonStyle = RibbonItemStyles.Large });
			mainGroup.BarItems.Add(new BarCheckButtonViewModel(new ChangeFunnelRationAutoCommand(this), this, BindingPathToSelectedModel, new SelectedModelToRatioAutoConverter()) { RibbonStyle = RibbonItemStyles.Large });
			mainGroup.BarItems.Add(CreateRatioEditor());
			mainGroup.BarItems.Add(CreatePointDistanceEditor());
			return mainGroup;
		}
		RibbonItemViewModelBase CreatePointDistanceEditor() {
			var command = new ChangeFunnelPointDistanceCommand(this);
			IValueConverter converter = new SelectedModelToFunnelPointDistanceConverter();
			var spin = new BarSpinEditViewModel(command, this, BindingPathToSelectedModel, converter);
			spin.MinValue = 0;
			spin.MaxValue = 20;
			spin.EditWidth = 75;
			spin.IsFloatValue = false;
			return spin;
		}
		RibbonItemViewModelBase CreateRatioEditor() {
			var command = new ChangeFunnelRatioCommand(this);
			IValueConverter converter = new SelectedModelToFunnelRatioConverter();
			var spin = new BarSpinEditViewModel(command, this, BindingPathToSelectedModel, converter);
			spin.MinValue = (decimal)0.2;
			spin.MaxValue = 2;
			spin.EditValue = 1;
			spin.Increment = (decimal)0.1;
			spin.EditWidth = 75;
			spin.IsFloatValue = true;
			return spin;
		}
		#endregion
		#region NestedDonut2D options group
		RibbonPageGroupViewModel CreateNestedDonut2DOptionsGroup() {
			RibbonPageGroupViewModel nestedDonutGroup = new RibbonPageGroupViewModel(hideGroupIfAllCommandsAreDisabled: true);
			nestedDonutGroup.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_NestedDonut2DOptionsGroup);
			nestedDonutGroup.BarItems.Add(CreatedGroupPropertyEditor());
			nestedDonutGroup.BarItems.Add(CreatedWeightEditor());
			nestedDonutGroup.BarItems.Add(new BarSeparatorViewModel());
			nestedDonutGroup.BarItems.Add(CreateInnerIndentEditor());
			nestedDonutGroup.BarItems.Add(CreateHoleRadiusOfNestedDonutPercentEditor());
			return nestedDonutGroup;
		}
		RibbonItemViewModelBase CreateHoleRadiusOfNestedDonutPercentEditor() {
			var command = new ChangeNestedDonutHoleRadiusPercent(this);
			IValueConverter converter = new SelectedModelToPieHoleRadiusPercent();
			var spin = new BarSpinEditViewModel(command, this, BindingPathToSelectedModel, converter);
			spin.MinValue = 0;
			spin.MaxValue = 100;
			spin.IsFloatValue = true;
			return spin;
		}
		RibbonItemViewModelBase CreatedGroupPropertyEditor() {
			var command = new ChangeNestedDonutGroupCommand(this);
			IValueConverter converter = new NestedDonutGroupConverter();
			var editor = new BarEditValueItemViewModel(command, this, BindingPathToSelectedModel, converter);
			editor.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_NestedDonutGroup);
			return editor;
		}
		RibbonItemViewModelBase CreateInnerIndentEditor() {
			var command = new SetNestedDonutInnerIndentCommand(this);
			IValueConverter converter = new NestedDonutInnerIndentIndentConverter();
			var editor = new BarSpinEditViewModel(command, this, BindingPathToSelectedModel, converter);
			editor.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_NestedDonutInnerIndent);
			editor.EditValue = 0;
			editor.MaxValue = 1000;
			return editor;
		}
		RibbonItemViewModelBase CreatedWeightEditor() {
			var command = new SetNestedDonutWeightCommand(this);
			IValueConverter converter = new NestedDonutWeightConverter();
			var editor = new BarSpinEditViewModel(command, this, BindingPathToSelectedModel, converter);
			editor.IsFloatValue = true;
			editor.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_NestedDonutWeight);
			editor.EditValue = 0;
			editor.MaxValue = 30;
			editor.Increment = 0.25M;
			return editor;
		}
		#endregion
		#endregion //Series Options page
		#region Series Data page
		RibbonPageViewModel CreateSeriesDataPage() {
			var page =  new RibbonPageViewModel(this, RibbonPagesNames.SeriesOptionsDataPage);
			page.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_DataPage);
			page.Groups.Add(CreateSeriesDataSourcePageGroup());
			page.Groups.Add(CreateSeriesDataMembersPageGroup());
			return page;
		}
		#region Data Source group
		RibbonPageGroupViewModel CreateSeriesDataSourcePageGroup() {
			var dataSourceGroup = new RibbonPageGroupViewModel();
			dataSourceGroup.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_DataSourcePageGroup);
			dataSourceGroup.BarItems.Add(CreateChangeSeriesDataSourceGallery());
			return dataSourceGroup;
		}
		RibbonItemViewModelBase CreateChangeSeriesDataSourceGallery() {
			var changeDataSourceSplitButton = new BarDropDownButtonGalleryViewModel();
			changeDataSourceSplitButton.RibbonStyle = RibbonItemStyles.Large;
			changeDataSourceSplitButton.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_DataSourcePageGroup);
			changeDataSourceSplitButton.InitialVisibleRowCount = 4;
			changeDataSourceSplitButton.InitialVisibleColCount = 1;
			changeDataSourceSplitButton.SizeMode = GallerySizeMode.Vertical;
			changeDataSourceSplitButton.ItemCheckMode = GalleryItemCheckMode.Single;
			changeDataSourceSplitButton.IsItemCaptionVisible = true;
			var galleryGroupDataSources = new GalleryGroupViewModel();
			changeDataSourceSplitButton.Groups.Add(galleryGroupDataSources);
			galleryGroupDataSources.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ChartViewGallery_BarSeries);
			var noneItem = new BarEditValueItemViewModel(new SelectDataSourceNoneCommand(this), this, BindingPathToSelectedModel, new SelectedDataSourceConverter());
			galleryGroupDataSources.Items.Add(noneItem);
			var chartDataSourceItem = new BarEditValueItemViewModel(new SelectChartDataSourceCommand(this), this, BindingPathToSelectedModel, new SelectedDataSourceConverter());
			galleryGroupDataSources.Items.Add(chartDataSourceItem);
			var seriesDataSourceItem = new BarEditValueItemViewModel(new SelectSeriesDataSourceCommand(this), this, BindingPathToSelectedModel, new SelectedDataSourceConverter());
			galleryGroupDataSources.Items.Add(seriesDataSourceItem);
			return changeDataSourceSplitButton;
		}
		#endregion
		#region Data Members group
		RibbonPageGroupViewModel CreateSeriesDataMembersPageGroup() {
			var dataMembersGroup = new RibbonPageGroupViewModel();
			dataMembersGroup.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_DataMembersPageGroup);
			dataMembersGroup.BarItems.Add(CreateSeriesArgumentDataMemberEditor());
			dataMembersGroup.BarItems.Add(CreateSeriesValueDataMemberEditor());
			dataMembersGroup.BarItems.Add(CreateSeriesValue2DataMemberEditor());
			dataMembersGroup.BarItems.Add(CreateSeriesWeightDataMemberEditor());
			dataMembersGroup.BarItems.Add(CreateSeriesLowValueDataMemberEditor());
			dataMembersGroup.BarItems.Add(CreateSeriesHighValueDataMemberEditor());
			dataMembersGroup.BarItems.Add(CreateSeriesOpenValueDataMemberEditor());
			dataMembersGroup.BarItems.Add(CreateSeriesCloseValueDataMemberEditor());
			dataMembersGroup.BarItems.Add(CreateSelectedSeriesDataMemberEditor());
			return dataMembersGroup;
		}
		RibbonItemViewModelBase CreateSeriesArgumentDataMemberEditor() {
			var command = new SelectArgumentDataMemberCommand(this);
			IValueConverter converter = new DataMemberConverter();
			bool isVlaueDataMember = false;
			var editor = new BarPopupDataBrowserEditViewModel(command, this, BindingPathToSelectedModel, converter, isVlaueDataMember);
			return editor;
		}
		RibbonItemViewModelBase CreateSeriesValueDataMemberEditor() {
			var command = new SelectValueDataMemberCommand(this);
			IValueConverter converter = new DataMemberConverter();
			bool isValueDataMember = true;
			IRibbonBehavior hideWhenDisabled = new HideBehavior();
			var editor = new BarPopupDataBrowserEditViewModel(command, this, BindingPathToSelectedModel, converter, hideWhenDisabled, isValueDataMember);
			return editor;
		}
		RibbonItemViewModelBase CreateSeriesValue2DataMemberEditor() {
			var command = new SelectValue2DataMemberCommand(this);
			IValueConverter converter = new DataMemberConverter();
			bool isValueDataMember = true;
			IRibbonBehavior hideWhenDisabled = new HideBehavior();
			var editor = new BarPopupDataBrowserEditViewModel(command, this, BindingPathToSelectedModel, converter, hideWhenDisabled, isValueDataMember);
			return editor;
		}
		RibbonItemViewModelBase CreateSeriesWeightDataMemberEditor() {
			var command = new SelectWeightDataMemberCommand(this);
			IValueConverter converter = new DataMemberConverter();
			bool isValueDataMember = true;
			IRibbonBehavior hideWhenDisabled = new HideBehavior();
			var editor = new BarPopupDataBrowserEditViewModel(command, this, BindingPathToSelectedModel, converter, hideWhenDisabled, isValueDataMember);
			return editor;
		}
		RibbonItemViewModelBase CreateSeriesLowValueDataMemberEditor() {
			var command = new SelectLowValueDataMemberCommand(this);
			IValueConverter converter = new DataMemberConverter();
			bool isValueDataMember = true;
			IRibbonBehavior hideWhenDisabled = new HideBehavior();
			var editor = new BarPopupDataBrowserEditViewModel(command, this, BindingPathToSelectedModel, converter, hideWhenDisabled, isValueDataMember);
			return editor;
		}
		RibbonItemViewModelBase CreateSeriesHighValueDataMemberEditor() {
			var command = new SelectHighValueDataMemberCommand(this);
			IValueConverter converter = new DataMemberConverter();
			bool isValueDataMember = true;
			IRibbonBehavior hideWhenDisabled = new HideBehavior();
			var editor = new BarPopupDataBrowserEditViewModel(command, this, BindingPathToSelectedModel, converter, hideWhenDisabled, isValueDataMember);
			return editor;
		}
		RibbonItemViewModelBase CreateSeriesOpenValueDataMemberEditor() {
			var command = new SelectOpenValueDataMemberCommand(this);
			IValueConverter converter = new DataMemberConverter();
			bool isValueDataMember = true;
			IRibbonBehavior hideWhenDisabled = new HideBehavior();
			var editor = new BarPopupDataBrowserEditViewModel(command, this, BindingPathToSelectedModel, converter, hideWhenDisabled, isValueDataMember);
			return editor;
		}
		RibbonItemViewModelBase CreateSeriesCloseValueDataMemberEditor() {
			var command = new SelectCloseValueDataMemberCommand(this);
			IValueConverter converter = new DataMemberConverter();
			bool isValueDataMember = true;
			IRibbonBehavior hideWhenDisabled = new HideBehavior();
			var editor = new BarPopupDataBrowserEditViewModel(command, this, BindingPathToSelectedModel, converter, hideWhenDisabled, isValueDataMember);
			return editor;
		}
		RibbonItemViewModelBase CreateSelectedSeriesDataMemberEditor() {
			var command = new SelectSeriesDataMemberCommand(this);
			IValueConverter converter = new DataMemberConverter();
			bool isValueDataMember = false;
			IRibbonBehavior hideWhenDisabled = new HideBehavior();
			var editor = new BarPopupDataBrowserEditViewModel(command, this, BindingPathToSelectedModel, converter, hideWhenDisabled, isValueDataMember);
			return editor;
		}
		#endregion //Data Members group
		#endregion //Series Data page
	}
}
