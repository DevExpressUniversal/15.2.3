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
using System.Windows.Data;
using DevExpress.Utils;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Charts;
namespace DevExpress.Charts.Designer.Native {
	public sealed partial class WpfChartModel : ChartModelElement {
		RibbonPageCategoryViewModel CreateAxisCategoryModel() {
			RibbonPageCategoryViewModel axisPageCategory = new RibbonPageCategoryViewModel(this, typeof(WpfChartAxisModel));
			axisPageCategory.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisCategoryTitle);
			RibbonPageViewModel page = new RibbonPageViewModel(this, RibbonPagesNames.AxisOptionsPage);
			page.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisPageCategoryTitle);
			axisPageCategory.Pages.Add(page);
			page.Groups.Add(CreateGeneralPageGroup());
			page.Groups.Add(CreateAxisAppearancePageGroup());
			page.Groups.Add(CreateAxisTitlePageGroup());
			page.Groups.Add(CreateAxisElementsPageGroup());
			page.Groups.Add(CreateRangePageGroup());
			return axisPageCategory;
		}
		#region General group
		RibbonPageGroupViewModel CreateGeneralPageGroup() {
			RibbonPageGroupViewModel generalPageGroup = new RibbonPageGroupViewModel();
			generalPageGroup.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_GeneralGroup);
			generalPageGroup.BarItems.Add(CreateGalleryDropDownButton_AxisKind());
			CreateButtons_AxisInterlaced(generalPageGroup);
			CreateButtons_AxisReverse(generalPageGroup);
			generalPageGroup.BarItems.Add(CreateGalleryDropDownButton_AxisLabels());
			return generalPageGroup;
		}
		BarDropDownButtonGalleryViewModel CreateGalleryDropDownButton_AxisKind() {
			BarDropDownButtonGalleryViewModel changeAxisKindButton = new BarDropDownButtonGalleryViewModel();
			changeAxisKindButton.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_AxisKind);
			changeAxisKindButton.ImageName = GlyphUtils.BarItemImages + "AxisOptions\\AxisKind";
			changeAxisKindButton.InitialVisibleColCount = 1;
			changeAxisKindButton.IsItemCaptionVisible = true;
			changeAxisKindButton.IsItemDescriptionVisible = true;
			changeAxisKindButton.IsGroupCaptionVisible = DefaultBoolean.False;
			changeAxisKindButton.ItemCheckMode = GalleryItemCheckMode.Single;
			GalleryGroupViewModel noneGalleryGroup = new GalleryGroupViewModel(hideGroupIfAllItemsAreDisabled: true);
			BarEditValueItemViewModel axisNoneGalleryItem = new BarEditValueItemViewModel(
				new AxisKindNoneCommand(this),
				this, BindingPathToSelectedModel, new AxisOptionsConverter());
			noneGalleryGroup.Items.Add(axisNoneGalleryItem);
			changeAxisKindButton.Groups.Add(noneGalleryGroup);
			GalleryGroupViewModel axisXKindsGalleryGroup = new GalleryGroupViewModel(hideGroupIfAllItemsAreDisabled: true);
			BarEditValueItemViewModel axisXNearNotReverseGalleryItem = new BarEditValueItemViewModel(
				new AxisXKindNearNotReverseCommand(this),
				this, BindingPathToSelectedModel, new AxisOptionsConverter());
			axisXKindsGalleryGroup.Items.Add(axisXNearNotReverseGalleryItem);
			BarEditValueItemViewModel axisXFarNotReverseGalleryItem = new BarEditValueItemViewModel(
				new AxisXKindFarNotReverseCommand(this),
				this, BindingPathToSelectedModel, new AxisOptionsConverter());
			axisXKindsGalleryGroup.Items.Add(axisXFarNotReverseGalleryItem);
			changeAxisKindButton.Groups.Add(axisXKindsGalleryGroup);
			GalleryGroupViewModel axisYKindsGalleryGroup = new GalleryGroupViewModel(hideGroupIfAllItemsAreDisabled: true);
			BarEditValueItemViewModel axisYNearNotReverseGalleryItem = new BarEditValueItemViewModel(
				new AxisYKindNearNotReverseCommand(this),
				this, BindingPathToSelectedModel, new AxisOptionsConverter());
			axisYKindsGalleryGroup.Items.Add(axisYNearNotReverseGalleryItem);
			BarEditValueItemViewModel axisYFarNotReverseGalleryItem = new BarEditValueItemViewModel(
				new AxisYKindFarNotReverseCommand(this),
				this, BindingPathToSelectedModel, new AxisOptionsConverter());
			axisYKindsGalleryGroup.Items.Add(axisYFarNotReverseGalleryItem);
			changeAxisKindButton.Groups.Add(axisYKindsGalleryGroup);
			GalleryGroupViewModel circularAxisYKindsGalleryGroup = new GalleryGroupViewModel(hideGroupIfAllItemsAreDisabled: true);
			BarEditValueItemViewModel circularAxisYNoneGalleryItem = new BarEditValueItemViewModel(
				new CircularAxisYKindNoneCommand(this),
				this, BindingPathToSelectedModel, new AxisOptionsConverter());
			circularAxisYKindsGalleryGroup.Items.Add(circularAxisYNoneGalleryItem);
			BarEditValueItemViewModel axisVisibleGalleryItem = new BarEditValueItemViewModel(
				new CircularAxisYKindVisibleCommand(this),
				this, BindingPathToSelectedModel, new AxisOptionsConverter());
			circularAxisYKindsGalleryGroup.Items.Add(axisVisibleGalleryItem);
			changeAxisKindButton.Groups.Add(circularAxisYKindsGalleryGroup);
			return changeAxisKindButton;
		}
		void CreateButtons_AxisInterlaced(RibbonPageGroupViewModel pageGroup) {
			BarCheckButtonViewModel interlacedAxisX2DModel = new BarCheckButtonViewModel(new AxisX2DInterlacedCommand(this), this, BindingPathToSelectedModel, new AxisOptionsConverter(), new HideBehavior());
			pageGroup.BarItems.Add(interlacedAxisX2DModel);
			BarCheckButtonViewModel interlacedAxisY2DModel = new BarCheckButtonViewModel(new AxisY2DInterlacedCommand(this), this, BindingPathToSelectedModel, new AxisOptionsConverter(), new HideBehavior());
			pageGroup.BarItems.Add(interlacedAxisY2DModel);
			BarCheckButtonViewModel interlacedAxisX3DModel = new BarCheckButtonViewModel(new AxisX3DInterlacedCommand(this), this, BindingPathToSelectedModel, new AxisOptionsConverter(), new HideBehavior());
			pageGroup.BarItems.Add(interlacedAxisX3DModel);
			BarCheckButtonViewModel interlacedAxisY3DModel = new BarCheckButtonViewModel(new AxisY3DInterlacedCommand(this), this, BindingPathToSelectedModel, new AxisOptionsConverter(), new HideBehavior());
			pageGroup.BarItems.Add(interlacedAxisY3DModel);
			BarCheckButtonViewModel interlacedCircularAxisX2DModel = new BarCheckButtonViewModel(new CircularAxisX2DInterlacedCommand(this), this, BindingPathToSelectedModel, new AxisOptionsConverter(), new HideBehavior());
			pageGroup.BarItems.Add(interlacedCircularAxisX2DModel);
			BarCheckButtonViewModel interlacedCircularAxisY2DModel = new BarCheckButtonViewModel(new CircularAxisY2DInterlacedCommand(this), this, BindingPathToSelectedModel, new AxisOptionsConverter(), new HideBehavior());
			pageGroup.BarItems.Add(interlacedCircularAxisY2DModel);
		}
		void CreateButtons_AxisReverse(RibbonPageGroupViewModel pageGroup) {
			BarCheckButtonViewModel reverseAxisX2DModel = new BarCheckButtonViewModel(new AxisX2DReverseCommand(this), this, BindingPathToSelectedModel, new AxisOptionsConverter(), new HideBehavior());
			pageGroup.BarItems.Add(reverseAxisX2DModel);
			BarCheckButtonViewModel reverseAxisY2DModel = new BarCheckButtonViewModel(new AxisY2DReverseCommand(this), this, BindingPathToSelectedModel, new AxisOptionsConverter(), new HideBehavior());
			pageGroup.BarItems.Add(reverseAxisY2DModel);
		}
		BarDropDownButtonGalleryViewModel CreateGalleryDropDownButton_AxisLabels() {
			BarDropDownButtonGalleryViewModel labelsOrientationButton = new BarDropDownButtonGalleryViewModel();
			labelsOrientationButton.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_LabelsOrientation);
			labelsOrientationButton.ImageName = GlyphUtils.BarItemImages + "AxisOptions\\LabelOrientation";
			labelsOrientationButton.InitialVisibleColCount = 1;
			labelsOrientationButton.InitialVisibleRowCount = 7;
			labelsOrientationButton.IsItemCaptionVisible = true;
			labelsOrientationButton.IsItemDescriptionVisible = true;
			labelsOrientationButton.IsGroupCaptionVisible = DefaultBoolean.False;
			labelsOrientationButton.ItemCheckMode = GalleryItemCheckMode.Single;
			GalleryGroupViewModel axisXLabelOrientationGalleryGroup = new GalleryGroupViewModel(hideGroupIfAllItemsAreDisabled: true);
			BarEditValueItemViewModel axisXNoneGalleryItem = new BarEditValueItemViewModel(
				new AxisXLabelOrientationNoneCommand(this),
				this, BindingPathToSelectedModel, new AxisOptionsConverter());
			axisXLabelOrientationGalleryGroup.Items.Add(axisXNoneGalleryItem);
			BarEditValueItemViewModel axisXLabelNormalGalleryItem = new BarEditValueItemViewModel(
				new AxisXLabelOrientationNormalCommand(this),
				this, BindingPathToSelectedModel, new AxisOptionsConverter());
			axisXLabelOrientationGalleryGroup.Items.Add(axisXLabelNormalGalleryItem);
			BarEditValueItemViewModel axisXLabelStaggeredGalleryItem = new BarEditValueItemViewModel(
				new AxisXLabelOrientationStaggeredCommand(this),
				this, BindingPathToSelectedModel, new AxisOptionsConverter());
			axisXLabelOrientationGalleryGroup.Items.Add(axisXLabelStaggeredGalleryItem);
			BarEditValueItemViewModel axisXLabelRotated90GalleryItem = new BarEditValueItemViewModel(
				new AxisXLabelOrientationRotated90Command(this),
				this, BindingPathToSelectedModel, new AxisOptionsConverter());
			axisXLabelOrientationGalleryGroup.Items.Add(axisXLabelRotated90GalleryItem);
			BarEditValueItemViewModel axisXLabelRotatedMinus90GalleryItem = new BarEditValueItemViewModel(
				new AxisXLabelOrientationRotatedMinus90Command(this),
				this, BindingPathToSelectedModel, new AxisOptionsConverter());
			axisXLabelOrientationGalleryGroup.Items.Add(axisXLabelRotatedMinus90GalleryItem);
			BarEditValueItemViewModel axisXLabelRotated45GalleryItem = new BarEditValueItemViewModel(
				new AxisXLabelOrientationRotated45Command(this),
				this, BindingPathToSelectedModel, new AxisOptionsConverter());
			axisXLabelOrientationGalleryGroup.Items.Add(axisXLabelRotated45GalleryItem);
			BarEditValueItemViewModel axisXLabelRotatedMinus45GalleryItem = new BarEditValueItemViewModel(
				new AxisXLabelOrientationRotatedMinus45Command(this),
				this, BindingPathToSelectedModel, new AxisOptionsConverter());
			axisXLabelOrientationGalleryGroup.Items.Add(axisXLabelRotatedMinus45GalleryItem);
			labelsOrientationButton.Groups.Add(axisXLabelOrientationGalleryGroup);
			GalleryGroupViewModel axisYLabelOrientationGalleryGroup = new GalleryGroupViewModel(hideGroupIfAllItemsAreDisabled: true);
			BarEditValueItemViewModel axisYNoneGalleryItem = new BarEditValueItemViewModel(
				new AxisYLabelOrientationNoneCommand(this),
				this, BindingPathToSelectedModel, new AxisOptionsConverter());
			axisYLabelOrientationGalleryGroup.Items.Add(axisYNoneGalleryItem);
			BarEditValueItemViewModel axisYLabelNormalGalleryItem = new BarEditValueItemViewModel(
				new AxisYLabelOrientationNormalCommand(this),
				this, BindingPathToSelectedModel, new AxisOptionsConverter());
			axisYLabelOrientationGalleryGroup.Items.Add(axisYLabelNormalGalleryItem);
			BarEditValueItemViewModel axisYLabelRotated90GalleryItem = new BarEditValueItemViewModel(
				new AxisYLabelOrientationRotated90Command(this),
				this, BindingPathToSelectedModel, new AxisOptionsConverter());
			axisYLabelOrientationGalleryGroup.Items.Add(axisYLabelRotated90GalleryItem);
			BarEditValueItemViewModel axisYLabelRotatedMinus90GalleryItem = new BarEditValueItemViewModel(
				new AxisYLabelOrientationRotatedMinus90Command(this),
				this, BindingPathToSelectedModel, new AxisOptionsConverter());
			axisYLabelOrientationGalleryGroup.Items.Add(axisYLabelRotatedMinus90GalleryItem);
			labelsOrientationButton.Groups.Add(axisYLabelOrientationGalleryGroup);
			GalleryGroupViewModel circularAxisXLabelOrientationGalleryGroup = new GalleryGroupViewModel(hideGroupIfAllItemsAreDisabled: true);
			BarEditValueItemViewModel circularAxisXNoneGalleryItem = new BarEditValueItemViewModel(
				new CircularAxisXLabelOrientationNoneCommand(this),
				this, BindingPathToSelectedModel, new AxisOptionsConverter());
			circularAxisXLabelOrientationGalleryGroup.Items.Add(circularAxisXNoneGalleryItem);
			BarEditValueItemViewModel circulatAxisXLabelNormalGalleryItem = new BarEditValueItemViewModel(
				new CircularAxisXLabelOrientationNormalCommand(this),
				this, BindingPathToSelectedModel, new AxisOptionsConverter());
			circularAxisXLabelOrientationGalleryGroup.Items.Add(circulatAxisXLabelNormalGalleryItem);
			labelsOrientationButton.Groups.Add(circularAxisXLabelOrientationGalleryGroup);
			GalleryGroupViewModel circularAxisYLabelOrientationGalleryGroup = new GalleryGroupViewModel(hideGroupIfAllItemsAreDisabled: true);
			BarEditValueItemViewModel circularAxisYNoneGalleryItem = new BarEditValueItemViewModel(
				new CircularAxisYLabelOrientationNoneCommand(this),
				this, BindingPathToSelectedModel, new AxisOptionsConverter());
			circularAxisYLabelOrientationGalleryGroup.Items.Add(circularAxisYNoneGalleryItem);
			BarEditValueItemViewModel circulatAxisYLabelNormalGalleryItem = new BarEditValueItemViewModel(
				new CircularAxisYLabelOrientationNormalCommand(this),
				this, BindingPathToSelectedModel, new AxisOptionsConverter());
			circularAxisYLabelOrientationGalleryGroup.Items.Add(circulatAxisYLabelNormalGalleryItem);
			labelsOrientationButton.Groups.Add(circularAxisYLabelOrientationGalleryGroup);
			return labelsOrientationButton;
		}
		#endregion
		#region Title group
		RibbonPageGroupViewModel CreateAxisTitlePageGroup() {
			RibbonPageGroupViewModel titlePageGroup = new RibbonPageGroupViewModel(hideGroupIfAllCommandsAreDisabled: true);
			titlePageGroup.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_TitleGroup);
			titlePageGroup.BarItems.Add(CreateGalleryDropDownButton_TitlePosition());
			titlePageGroup.BarItems.Add(new BarStaticTextViewModel(ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_TitleContent)));
			titlePageGroup.BarItems.Add(new BarEditValueItemViewModel(new SetAxisTitleContentCommand(this), this, BindingPathToSelectedModel, new AxisTitleDisplayNameConverter()));
			return titlePageGroup;
		}
		BarDropDownButtonGalleryViewModel CreateGalleryDropDownButton_TitlePosition() {
			BarDropDownButtonGalleryViewModel titlePositionButton = new BarDropDownButtonGalleryViewModel();
			titlePositionButton.RibbonStyle = RibbonItemStyles.Large;
			titlePositionButton.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_TitlePosition);
			titlePositionButton.ImageName = GlyphUtils.BarItemImages + "AxisOptions\\TitlePosition";
			titlePositionButton.InitialVisibleColCount = 1;
			titlePositionButton.IsItemCaptionVisible = true;
			titlePositionButton.IsItemDescriptionVisible = true;
			titlePositionButton.IsGroupCaptionVisible = DefaultBoolean.False;
			titlePositionButton.ItemCheckMode = GalleryItemCheckMode.Single;
			GalleryGroupViewModel noneGalleryGroup = new GalleryGroupViewModel(hideGroupIfAllItemsAreDisabled: true);
			BarEditValueItemViewModel axisNoneGalleryItem = new BarEditValueItemViewModel(
				new AxisTitlePositionNoneCommand(this),
				this, BindingPathToSelectedModel, new AxisOptionsConverter());
			noneGalleryGroup.Items.Add(axisNoneGalleryItem);
			titlePositionButton.Groups.Add(noneGalleryGroup);
			GalleryGroupViewModel axisXTitlePositionGalleryGroup = new GalleryGroupViewModel(hideGroupIfAllItemsAreDisabled: true);
			BarEditValueItemViewModel axisXTitleNearGalleryItem = new BarEditValueItemViewModel(
				new AxisXTitlePositionNearCommand(this),
				this, BindingPathToSelectedModel, new AxisOptionsConverter());
			axisXTitlePositionGalleryGroup.Items.Add(axisXTitleNearGalleryItem);
			BarEditValueItemViewModel axisXTitleCenterGalleryItem = new BarEditValueItemViewModel(
				new AxisXTitlePositionCenterCommand(this),
				this, BindingPathToSelectedModel, new AxisOptionsConverter());
			axisXTitlePositionGalleryGroup.Items.Add(axisXTitleCenterGalleryItem);
			BarEditValueItemViewModel axisXTitleFarGalleryItem = new BarEditValueItemViewModel(
				new AxisXTitlePositionFarCommand(this),
				this, BindingPathToSelectedModel, new AxisOptionsConverter());
			axisXTitlePositionGalleryGroup.Items.Add(axisXTitleFarGalleryItem);
			titlePositionButton.Groups.Add(axisXTitlePositionGalleryGroup);
			GalleryGroupViewModel axisYTitlePositionGalleryGroup = new GalleryGroupViewModel(hideGroupIfAllItemsAreDisabled: true);
			BarEditValueItemViewModel axisYTitleNearGalleryItem = new BarEditValueItemViewModel(
				new AxisYTitlePositionNearCommand(this),
				this, BindingPathToSelectedModel, new AxisOptionsConverter());
			axisYTitlePositionGalleryGroup.Items.Add(axisYTitleNearGalleryItem);
			BarEditValueItemViewModel axisYTitleCenterGalleryItem = new BarEditValueItemViewModel(
				new AxisYTitlePositionCenterCommand(this),
				this, BindingPathToSelectedModel, new AxisOptionsConverter());
			axisYTitlePositionGalleryGroup.Items.Add(axisYTitleCenterGalleryItem);
			BarEditValueItemViewModel axisYTitleFarGalleryItem = new BarEditValueItemViewModel(
				new AxisYTitlePositionFarCommand(this),
				this, BindingPathToSelectedModel, new AxisOptionsConverter());
			axisYTitlePositionGalleryGroup.Items.Add(axisYTitleFarGalleryItem);
			titlePositionButton.Groups.Add(axisYTitlePositionGalleryGroup);
			return titlePositionButton;
		}
		#endregion
		#region Axis Elements group
		RibbonPageGroupViewModel CreateAxisElementsPageGroup() {
			RibbonPageGroupViewModel elementsPageGroup = new RibbonPageGroupViewModel();
			elementsPageGroup.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_ElementsGroup);
			elementsPageGroup.BarItems.Add(CreateAddConstantLineXButton());
			elementsPageGroup.BarItems.Add(CreateAddConstantLineYButton());
			elementsPageGroup.BarItems.Add(CreateAddStripXButton());
			elementsPageGroup.BarItems.Add(CreateAddStripYButton());
			return elementsPageGroup;
		}
		RibbonItemViewModelBase CreateAddConstantLineXButton() {
			var command = new AddConstantLineToAxisXCommand(this);
			BarButtonViewModel addConstantLineXButton = new BarButtonViewModel(command, new HideBehavior());
			return addConstantLineXButton;
		}
		RibbonItemViewModelBase CreateAddConstantLineYButton() {
			var command = new AddConstantLineToAxisYCommand(this);
			BarButtonViewModel addConstantLineYButton = new BarButtonViewModel(command, new HideBehavior());
			return addConstantLineYButton;
		}
		RibbonItemViewModelBase CreateAddStripXButton() {
			var command = new AddStripToAxisXCommand(this);
			BarButtonViewModel addStripXButton = new BarButtonViewModel(command, new HideBehavior());
			return addStripXButton;
		}
		RibbonItemViewModelBase CreateAddStripYButton() {
			var command = new AddStripToAxisYCommand(this);
			BarButtonViewModel addStripYButton = new BarButtonViewModel(command, new HideBehavior());
			return addStripYButton;
		}
		#endregion
		#region Axis Appearance group
		RibbonPageGroupViewModel CreateAxisAppearancePageGroup() {
			RibbonPageGroupViewModel appearancePageGroup = new RibbonPageGroupViewModel();
			appearancePageGroup.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_AppearanceGroup);
			appearancePageGroup.BarItems.Add(CreateGridLinesStaticText());
			appearancePageGroup.BarItems.Add(CreateGridLinesVisibleCheckEdit());
			appearancePageGroup.BarItems.Add(CreateMinorGridlinesVisibleCheckEdit());
			appearancePageGroup.BarItems.Add(new BarSeparatorViewModel());
			appearancePageGroup.BarItems.Add(CreateTickmarksStaticText());
			appearancePageGroup.BarItems.Add(CreateMajorTickmarksCheckEdit());
			appearancePageGroup.BarItems.Add(CreateMinorTickmarksCheckEdit());
			appearancePageGroup.BarItems.Add(new BarSeparatorViewModel());
			return appearancePageGroup;
		}
		RibbonItemViewModelBase CreateGridLinesStaticText() {
			string gridlinesHeaderCaption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_GridlinesStaticText);
			BarStaticTextViewModel gridlinesHeader = new BarStaticTextViewModel(gridlinesHeaderCaption);
			return gridlinesHeader;
		}
		RibbonItemViewModelBase CreateGridLinesVisibleCheckEdit() {
			var command = new ToggleAxisGridLinesVisibleCommand(this);
			IValueConverter converter = new AxisGridLinesVisibleConverter();
			BarCheckEditViewModel majorGridlines = new BarCheckEditViewModel(command, this, BindingPathToSelectedModel, converter);
			majorGridlines.EditWidth = 15;
			return majorGridlines;
		}
		RibbonItemViewModelBase CreateMinorGridlinesVisibleCheckEdit() {
			var command = new ToggleAxisGridLinesMinorVisibledCommand(this);
			IValueConverter converter = new AxisGridLinesMinorVisibleConverter();
			BarCheckEditViewModel minorGridlines = new BarCheckEditViewModel(command, this, BindingPathToSelectedModel, converter);
			minorGridlines.EditWidth = 15;
			return minorGridlines;
		}
		RibbonItemViewModelBase CreateTickmarksStaticText() {
			string tickmarksHeaderCaption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_TickMarksStaticText);
			BarStaticTextViewModel tickmarksHeader = new BarStaticTextViewModel(tickmarksHeaderCaption);
			return tickmarksHeader;
		}
		RibbonItemViewModelBase CreateMajorTickmarksCheckEdit() {
			var command = new ToggleAxisTickmarksVisibleCommand(this);
			IValueConverter converter = new AxisTickMarksVisibleConverter();
			BarCheckEditViewModel majorTickmarks = new BarCheckEditViewModel(command, this, BindingPathToSelectedModel, converter);
			majorTickmarks.EditWidth = 15;
			return majorTickmarks;
		}
		RibbonItemViewModelBase CreateMinorTickmarksCheckEdit() {
			var command = new ToggleAxisTickmarksMinorVisibleCommand(this);
			IValueConverter converter = new ToggleAxisTickmarksMinorVisibleConverter();
			BarCheckEditViewModel minorTickmarks = new BarCheckEditViewModel(command, this, BindingPathToSelectedModel, converter);
			minorTickmarks.EditWidth = 15;
			return minorTickmarks;
		}
		#endregion
		#region Range group
		RibbonPageGroupViewModel CreateRangePageGroup() {
			RibbonPageGroupViewModel rangeGroup = new RibbonPageGroupViewModel(hideGroupIfAllCommandsAreDisabled: true);
			rangeGroup.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_RangeGroup);
			rangeGroup.BarItems.Add(CrateVisibleRangeHeader());
			rangeGroup.BarItems.Add(CreateQualitativeVisibleRangeMinValueEditor());
			rangeGroup.BarItems.Add(CreateNumericVisibleRangeMinValueEditor());
			rangeGroup.BarItems.Add(CreateDateTimeVisibleRangeMinValueEditor());
			rangeGroup.BarItems.Add(CreateQualitativeVisibleRangeMaxValueEditor());
			rangeGroup.BarItems.Add(CreateNumericVisibleRangeMaxValueEditor());
			rangeGroup.BarItems.Add(CreateDateTimeVisibleRangeMaxValueEditor());
			rangeGroup.BarItems.Add(new BarSeparatorViewModel());
			rangeGroup.BarItems.Add(CreateWholeRangeHeader());
			rangeGroup.BarItems.Add(CreateQualitativeWholeRangeMinValueEditor());
			rangeGroup.BarItems.Add(CreateNumericWholeRangeMinValueEditor());
			rangeGroup.BarItems.Add(CreateDateTimeWholeRangeMinValueEditor());
			rangeGroup.BarItems.Add(CreateQualitativeWholeRangeMaxValueEditor());
			rangeGroup.BarItems.Add(CreateNumericWholeRangeMaxValueEditor());
			rangeGroup.BarItems.Add(CreateDateTimeWholeRangeMaxValueEditor());
			return rangeGroup;
		}
		RibbonItemViewModelBase CrateVisibleRangeHeader() {
			string wholeRangeHeaderCaption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_VisibleRangeHeader);
			BarStaticTextViewModel visibleRangeHeader = new BarStaticTextViewModel(wholeRangeHeaderCaption);
			return visibleRangeHeader;
		}
		RibbonItemViewModelBase CreateQualitativeVisibleRangeMinValueEditor() {
			SetAxisVisualRangeMinValueCommand command = new SetAxisVisualRangeMinValueCommand(this, ScaleType.Qualitative);
			IValueConverter converter = new AxisModelToVisibleRangeMinValueConverter();
			BarEditValueItemViewModel editor = new BarEditValueItemViewModel(command, this, BindingPathToSelectedModel, converter, new HideBehavior());
			editor.NullText = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Auto);
			return editor;
		}
		RibbonItemViewModelBase CreateNumericVisibleRangeMinValueEditor() {
			SetAxisVisualRangeMinValueCommand setMinValueConmmandNumerical = new SetAxisVisualRangeMinValueCommand(this, ScaleType.Numerical);
			IValueConverter converter = new AxisModelToVisibleRangeMinValueConverter();
			var editor = new BarDoubleEditViewModel(setMinValueConmmandNumerical, this, BindingPathToSelectedModel, converter, new HideBehavior());
			editor.NullText = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Auto);
			return editor;
		}
		RibbonItemViewModelBase CreateDateTimeVisibleRangeMinValueEditor() {
			SetAxisVisualRangeMinValueCommand command = new SetAxisVisualRangeMinValueCommand(this, ScaleType.DateTime);
			IValueConverter converter = new AxisModelToVisibleRangeMinValueConverter();
			BarDateTimeEditViewModel editor = new BarDateTimeEditViewModel(command, this, BindingPathToSelectedModel, converter, new HideBehavior());
			editor.NullText = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Auto);
			return editor;
		}
		RibbonItemViewModelBase CreateQualitativeVisibleRangeMaxValueEditor() {
			SetAxisVisualRangeMaxValueCommand command = new SetAxisVisualRangeMaxValueCommand(this, ScaleType.Qualitative);
			IValueConverter converter = new AxisModelToVisibleRangeMaxValueConverter();
			BarEditValueItemViewModel editor = new BarEditValueItemViewModel(command, this, BindingPathToSelectedModel, converter, new HideBehavior());
			editor.NullText = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Auto);
			return editor;
		}
		RibbonItemViewModelBase CreateNumericVisibleRangeMaxValueEditor() {
			SetAxisVisualRangeMaxValueCommand setMaxValueConmmandNumerical = new SetAxisVisualRangeMaxValueCommand(this, ScaleType.Numerical);
			IValueConverter converter = new AxisModelToVisibleRangeMaxValueConverter();
			var editor = new BarDoubleEditViewModel(setMaxValueConmmandNumerical, this, BindingPathToSelectedModel, converter, new HideBehavior());
			editor.NullText = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Auto);
			return editor;
		}
		RibbonItemViewModelBase CreateDateTimeVisibleRangeMaxValueEditor() {
			SetAxisVisualRangeMaxValueCommand command = new SetAxisVisualRangeMaxValueCommand(this, ScaleType.DateTime);
			IValueConverter converter = new AxisModelToVisibleRangeMaxValueConverter();
			BarDateTimeEditViewModel editor = new BarDateTimeEditViewModel(command, this, BindingPathToSelectedModel, converter, new HideBehavior());
			editor.NullText = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Auto);
			return editor;
		}
		RibbonItemViewModelBase CreateWholeRangeHeader() {
			string visibleRangeHeaderCaption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_WholeRangeHeader);
			BarStaticTextViewModel wholeRangeHeader = new BarStaticTextViewModel(visibleRangeHeaderCaption);
			return wholeRangeHeader;
		}
		RibbonItemViewModelBase CreateQualitativeWholeRangeMinValueEditor() {
			var command = new SetAxisWholeRangeMinValueCommand(this, ScaleType.Qualitative);
			IValueConverter converter = new AxisModelToWholeRangeMinValueConverter();
			BarEditValueItemViewModel editor = new BarEditValueItemViewModel(command, this, BindingPathToSelectedModel, converter, new HideBehavior());
			editor.NullText = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Auto);
			return editor;
		}
		RibbonItemViewModelBase CreateNumericWholeRangeMinValueEditor() {
			var setMinValueConmmandNumerical = new SetAxisWholeRangeMinValueCommand(this, ScaleType.Numerical);
			IValueConverter converter = new AxisModelToWholeRangeMinValueConverter();
			var editor = new BarDoubleEditViewModel(setMinValueConmmandNumerical, this, BindingPathToSelectedModel, converter, new HideBehavior());
			editor.NullText = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Auto);
			return editor;
		}
		RibbonItemViewModelBase CreateDateTimeWholeRangeMinValueEditor() {
			var command = new SetAxisWholeRangeMinValueCommand(this, ScaleType.DateTime);
			IValueConverter converter = new AxisModelToWholeRangeMinValueConverter();
			BarDateTimeEditViewModel editor = new BarDateTimeEditViewModel(command, this, BindingPathToSelectedModel, converter, new HideBehavior());
			editor.NullText = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Auto);
			return editor;
		}
		RibbonItemViewModelBase CreateQualitativeWholeRangeMaxValueEditor() {
			var command = new SetAxisWholeRangeMaxValueCommand(this, ScaleType.Qualitative);
			IValueConverter converter = new AxisModelToWholeRangeMaxValueConverter();
			BarEditValueItemViewModel editor = new BarEditValueItemViewModel(command, this, BindingPathToSelectedModel, converter, new HideBehavior());
			editor.NullText = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Auto);
			return editor;
		}
		RibbonItemViewModelBase CreateNumericWholeRangeMaxValueEditor() {
			var setMaxValueConmmandNumerical = new SetAxisWholeRangeMaxValueCommand(this, ScaleType.Numerical);
			IValueConverter converter = new AxisModelToWholeRangeMaxValueConverter();
			var editor = new BarDoubleEditViewModel(setMaxValueConmmandNumerical, this, BindingPathToSelectedModel, converter, new HideBehavior());
			editor.NullText = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Auto);
			return editor;
		}
		RibbonItemViewModelBase CreateDateTimeWholeRangeMaxValueEditor() {
			var command = new SetAxisWholeRangeMaxValueCommand(this, ScaleType.DateTime);
			IValueConverter converter = new AxisModelToWholeRangeMaxValueConverter();
			BarDateTimeEditViewModel editor = new BarDateTimeEditViewModel(command, this, BindingPathToSelectedModel, converter, new HideBehavior());
			editor.NullText = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Auto);
			return editor;
		}
		#endregion
		#region Date&Time Options group
		#endregion
	}
}
