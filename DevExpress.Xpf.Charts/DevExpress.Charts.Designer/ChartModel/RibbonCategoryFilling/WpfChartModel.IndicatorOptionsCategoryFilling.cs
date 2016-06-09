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
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Charts;
namespace DevExpress.Charts.Designer.Native {
	public sealed partial class WpfChartModel : ChartModelElement {
		RibbonPageCategoryViewModel CreateIndicatorOptionsCategory() {
			RibbonPageCategoryViewModel indicatorCategory = new RibbonPageCategoryViewModel(this, typeof(WpfChartIndicatorModel));
			indicatorCategory.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.IndicatorCategoryTitle);
			indicatorCategory.Pages.Add(CreateIndicatorPage());
			return indicatorCategory;
		}
		RibbonPageViewModel CreateIndicatorPage() {
			RibbonPageViewModel indicatorPage = new RibbonPageViewModel(this, RibbonPagesNames.IndicatorOptionsPage);
			indicatorPage.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Indicator_IndicatorPageTitle);
			indicatorPage.Groups.Add(CreateIndicatorGeneralGroup());
			indicatorPage.Groups.Add(CreateRegressionLineGeneralGroup());
			indicatorPage.Groups.Add(CreateIndicatorPresentationGroup());
			indicatorPage.Groups.Add(CreateIndicatorAdvancedGroup());
			indicatorPage.Groups.Add(CreateIndicatorAdvancedGroup2());
			indicatorPage.Groups.Add(CreateSeparatePaneIndicatorGroup());
			return indicatorPage;
		}
		#region General group
		RibbonPageGroupViewModel CreateIndicatorGeneralGroup() {
			RibbonPageGroupViewModel group = new RibbonPageGroupViewModel(hideGroupIfAllCommandsAreDisabled: true);
			group.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Indicator_GeneralGroupCaption);
			group.BarItems.Add(CreateValueLevelEditor());
			group.BarItems.Add(CreatePointsCountEditor());
			group.BarItems.Add(CreateEnvelopePercentEditor());
			group.BarItems.Add(CreateArg1ValLevel1Header());
			group.BarItems.Add(CreateNumericArgument1Editor());
			group.BarItems.Add(CreateQualitativeArgument1Editor());
			group.BarItems.Add(CreateDateTimeArgument1Editor());
			group.BarItems.Add(CreateValueLevel1Editor());
			group.BarItems.Add(new BarSeparatorViewModel());
			group.BarItems.Add(CreateArg2ValLevel2Header());
			group.BarItems.Add(CreateNumericArgument2Editor());
			group.BarItems.Add(CreateQualitativeArgument2Editor());
			group.BarItems.Add(CreateDateTimeArgument2Editor());
			group.BarItems.Add(CreateValueLevel2Editor());
			return group;
		}
		RibbonItemViewModelBase CreateValueLevelEditor() {
			var command = new ChangeMovingAverageValueLevelCommand(this);
			IValueConverter converter = new SelectedModelToRegressionLineOrMovingAverageValueLevelConverter();
			var combo = new BarDynamicComboBoxViewModel(command, this, BindingPathToSelectedModel, "SelectedIndicatorAcceptedValueLevels", converter, new HideBehavior());
			return combo;
		}
		RibbonItemViewModelBase CreateArg1ValLevel1Header() {
			string headerCaption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Indicator_Arg1ValLevel1Header);
			var header = new BarStaticTextViewModel(headerCaption);
			return header;
		}
		RibbonItemViewModelBase CreateNumericArgument1Editor() {
			var command = new ChangeFinancialIndicatorArgument1Command(this, ScaleType.Numerical);
			IValueConverter converter = new SelectedModelToFinIndicatorNumericalArgument1Converter();
			var doubleEditor = new BarDoubleEditViewModel(command, this, BindingPathToSelectedModel, converter, new HideBehavior());
			return doubleEditor;
		}
		RibbonItemViewModelBase CreateQualitativeArgument1Editor() {
			var command = new ChangeFinancialIndicatorArgument1Command(this, ScaleType.Qualitative);
			IValueConverter converter = new SelectedModelToFinIndicatorNumericalArgument1Converter();
			var textEditor = new BarEditValueItemViewModel(command, this, BindingPathToSelectedModel, converter, new HideBehavior());
			return textEditor;
		}
		RibbonItemViewModelBase CreateDateTimeArgument1Editor() {
			var command = new ChangeFinancialIndicatorArgument1Command(this, ScaleType.DateTime);
			IValueConverter converter = new SelectedModelToFinIndicatorNumericalArgument1Converter();
			var dateEditor = new BarDateTimeEditViewModel(command, this, BindingPathToSelectedModel, converter, new HideBehavior());
			dateEditor.EditWidth = 130;
			return dateEditor;
		}
		RibbonItemViewModelBase CreateValueLevel1Editor() {
			var command = new ChangeFinancialIndicatorValueLevel1(this);
			IValueConverter converter = new SelectedModelToValueLevel1Converter();
			var combo = new BarDynamicComboBoxViewModel(command, this, BindingPathToSelectedModel, "SelectedIndicatorAcceptedValueLevels", converter, new HideBehavior());
			return combo;
		}
		RibbonItemViewModelBase CreateArg2ValLevel2Header() {
			string headerCaption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Indicator_Arg2ValLevel2Header);
			var header = new BarStaticTextViewModel(headerCaption);
			return header;
		}
		RibbonItemViewModelBase CreateNumericArgument2Editor() {
			var command = new ChangeFinancialIndicatorArgument2Command(this, ScaleType.Numerical);
			IValueConverter converter = new SelectedModelToFinIndicatorNumericalArgument2Converter();
			var doubleEditor = new BarDoubleEditViewModel(command, this, BindingPathToSelectedModel, converter, new HideBehavior());
			return doubleEditor;
		}
		RibbonItemViewModelBase CreateQualitativeArgument2Editor() {
			var command = new ChangeFinancialIndicatorArgument2Command(this, ScaleType.Qualitative);
			IValueConverter converter = new SelectedModelToFinIndicatorNumericalArgument2Converter();
			var textEditor = new BarEditValueItemViewModel(command, this, BindingPathToSelectedModel, converter, new HideBehavior());
			return textEditor;
		}
		RibbonItemViewModelBase CreateDateTimeArgument2Editor() {
			var command = new ChangeFinancialIndicatorArgument2Command(this, ScaleType.DateTime);
			IValueConverter converter = new SelectedModelToFinIndicatorNumericalArgument2Converter();
			var dateEditor = new BarDateTimeEditViewModel(command, this, BindingPathToSelectedModel, converter, new HideBehavior());
			dateEditor.EditWidth = 130;
			return dateEditor;
		}
		RibbonItemViewModelBase CreateValueLevel2Editor() {
			var command = new ChangeFinancialIndicatorValueLevel2(this);
			IValueConverter converter = new SelectedModelToValueLevel2Converter();
			var combo = new BarDynamicComboBoxViewModel(command, this, BindingPathToSelectedModel, "SelectedIndicatorAcceptedValueLevels", converter, new HideBehavior());
			return combo;
		}
		RibbonItemViewModelBase CreatePointsCountEditor() {
			var command = new ChangeMovingAveragePointsCountCommand(this);
			IValueConverter converter = new SelectedModelToMovingAveragePointsCountConverter();
			var editor = new BarSpinEditViewModel(command, this, BindingPathToSelectedModel, converter, new HideBehavior());
			editor.MinValue = 1;
			editor.MaxValue = int.MaxValue;
			return editor;
		}
		RibbonItemViewModelBase CreateEnvelopePercentEditor() {
			var command = new ChangeMovingAverageEnvelopePercentCommand(this);
			IValueConverter converter = new SelectedModelToMovingAverageEnvelopePercentConverter();
			var editor = new BarSpinEditViewModel(command, this, BindingPathToSelectedModel, converter, new HideBehavior());
			editor.MinValue = 0;
			editor.MaxValue = 100;
			return editor;
		}
		#endregion
		#region General group for RegressionLine
		RibbonPageGroupViewModel CreateRegressionLineGeneralGroup() {
			RibbonPageGroupViewModel group = new RibbonPageGroupViewModel(hideGroupIfAllCommandsAreDisabled: true);
			group.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Indicator_GeneralGroupCaption);
			group.BarItems.Add(new BarStaticTextViewModel(ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Indicator_VlaueLevelForRegressionLine)));
			group.BarItems.Add(CreateValueLevelEditorForRegressionLine());
			return group;
		}
		RibbonItemViewModelBase CreateValueLevelEditorForRegressionLine() {
			var command = new ChangeRegressionLineValueLevelCommand(this);
			IValueConverter converter = new SelectedModelToRegressionLineOrMovingAverageValueLevelConverter();
			var combo = new BarDynamicComboBoxViewModel(command, this, BindingPathToSelectedModel, "SelectedIndicatorAcceptedValueLevels", converter, new HideBehavior());
			return combo;
		}
		#endregion
		#region Presentation group
		RibbonPageGroupViewModel CreateIndicatorPresentationGroup() {
			RibbonPageGroupViewModel group = new RibbonPageGroupViewModel();
			group.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Indicator_PresentationGroupCaption);
			group.BarItems.Add(CreateIndicatorColorEditor());
			group.BarItems.Add(CreateIndicatorThicknessEditor());
			group.BarItems.Add(CreateIndicatorLegendTextEditor());
			return group;
		}
		RibbonItemViewModelBase CreateIndicatorColorEditor() {
			var command = new ChangeIndicatorColorCommand(this);
			BarColorEditViewModel colorEditor = new BarColorEditViewModel(command, this, BindingPathToSelectedModel, new SelectedModelToIndicatorColorConverter());
			return colorEditor;
		}
		RibbonItemViewModelBase CreateIndicatorThicknessEditor() {
			var command = new ChangeIndicatorThicknessCommand(this);
			IValueConverter converter = new SelectedModelToIndicatorThicknessConverter();
			BarThicknessEditViewModel thicknessEditor = new BarThicknessEditViewModel(command, this, BindingPathToSelectedModel, 1, 1, 20, converter);
			return thicknessEditor;
		}
		RibbonItemViewModelBase CreateIndicatorLegendTextEditor() {
			var command = new ChangeIndicatorLegendTextCommand(this);
			IValueConverter converter = new SelectedModelToIndicatorLegendTextConverter();
			BarEditValueItemViewModel textEditor = new BarEditValueItemViewModel(command, this, BindingPathToSelectedModel, converter);
			return textEditor;
		}
		#endregion
		#region Advanced group
		RibbonPageGroupViewModel CreateIndicatorAdvancedGroup() {
			RibbonPageGroupViewModel group = new RibbonPageGroupViewModel(hideGroupIfAllCommandsAreDisabled: true);
			group.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Indicator_AdvancedGroupCaption);
			group.BarItems.Add(CreateTrendLineExtrapolateToInfinityCheckButton());
			group.BarItems.Add(CreateFibonacciShowLevel23_6CheckBox());
			group.BarItems.Add(CreateFibonacciShowLevel76_4CheckBox());
			group.BarItems.Add(CreateFibonacciArcsShowLevel100CheckBox());
			group.BarItems.Add(CreateFibonacciFansShowLevel0CheckBox());
			group.BarItems.Add(CreateFibonacciRetracementShowAdditionalLevelsCheckBox());
			return group;
		}
		RibbonItemViewModelBase CreateTrendLineExtrapolateToInfinityCheckButton() {
			var command = new ToggleTrendLineExtrapolateToInfinityCommand(this);
			IValueConverter converter = new SelectedModelToTrenLineExtrapolateToInfinityConverter();
			var checkButton = new BarCheckButtonViewModel(command, this, BindingPathToSelectedModel, converter, new HideBehavior());
			return checkButton;
		}
		RibbonItemViewModelBase CreateFibonacciShowLevel23_6CheckBox() {
			var command = new ToggleFibonacciIndicatorShowLevel23_6Command(this);
			IValueConverter converter = new SelectedModelToFibonacciShowLevel23_6Converter();
			var checkEdit = new BarCheckEditViewModel(command, this, BindingPathToSelectedModel, converter, new HideBehavior());
			return checkEdit;
		}
		RibbonItemViewModelBase CreateFibonacciShowLevel76_4CheckBox() {
			var command = new ToggleFibonacciIndicatorShowLevel76_4Command(this);
			IValueConverter converter = new SelectedModelToFibonacciShowLevel76_4Converter();
			var checkEdit = new BarCheckEditViewModel(command, this, BindingPathToSelectedModel, converter, new HideBehavior());
			return checkEdit;
		}
		RibbonItemViewModelBase CreateFibonacciArcsShowLevel100CheckBox() {
			var command = new ToggleFibonacciArcsShowLevel100Command(this);
			IValueConverter converter = new SelectedModelToFibonacciArcsShowLevel100Converter();
			var checkEdit = new BarCheckEditViewModel(command, this, BindingPathToSelectedModel, converter, new HideBehavior());
			return checkEdit;
		}
		RibbonItemViewModelBase CreateFibonacciFansShowLevel0CheckBox() {
			var command = new ToggleFibonacciFansShowLevel0Command(this);
			IValueConverter converter = new SelectedModelToFibonacciFansShowLevel0Converter();
			var checkEdit = new BarCheckEditViewModel(command, this, BindingPathToSelectedModel, converter, new HideBehavior());
			return checkEdit;
		}
		RibbonItemViewModelBase CreateFibonacciRetracementShowAdditionalLevelsCheckBox() {
			var command = new ToggleFibonacciRetracementShowAdditionalLevelsCommand(this);
			IValueConverter converter = new SelectedModelToFibonacciRetracementShowAdditionalLevelsConverter();
			var checkEdit = new BarCheckEditViewModel(command, this, BindingPathToSelectedModel, converter, new HideBehavior());
			return checkEdit;
		}
		#endregion
		#region Advanced group #2 (created to hide gallery button)
		RibbonPageGroupViewModel CreateIndicatorAdvancedGroup2() {
			RibbonPageGroupViewModel group = new RibbonPageGroupViewModel(hideGroupIfAllCommandsAreDisabled: true);
			group.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Indicator_AdvancedGroupCaption);
			group.BarItems.Add(CreateMovingAverageKindGallery());
			return group;
		}
		RibbonItemViewModelBase CreateMovingAverageKindGallery() {
			var gallery = new BarDropDownButtonGalleryViewModel();
			gallery.InitialVisibleColCount = 1;
			gallery.InitialVisibleRowCount = 3;
			gallery.IsItemCaptionVisible = true;
			gallery.IsItemDescriptionVisible = true;
			gallery.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Indicator_MovingAverageKind);
			var group = new GalleryGroupViewModel();
			gallery.Groups.Add(group);
			var movingAverageCommand = new ChangeMovingAverageKindComand(this, MovingAverageKind.MovingAverage);
			IValueConverter converter = new SelectedViewModelToBoolMovingAverageKindConverter();
			group.Items.Add(new BarEditValueItemViewModel(movingAverageCommand, this, BindingPathToSelectedModel, converter));
			var envelopeCommand = new ChangeMovingAverageKindComand(this, MovingAverageKind.Envelope);
			group.Items.Add(new BarEditValueItemViewModel(envelopeCommand, this, BindingPathToSelectedModel, converter));
			var movingAverageAndEnvelopeCommand = new ChangeMovingAverageKindComand(this, MovingAverageKind.MovingAverageAndEnvelope);
			group.Items.Add(new BarEditValueItemViewModel(movingAverageAndEnvelopeCommand, this, BindingPathToSelectedModel, converter));
			return gallery;
		}
		#endregion
		#region SeparatePane group
		RibbonPageGroupViewModel CreateSeparatePaneIndicatorGroup() {
			RibbonPageGroupViewModel group = new RibbonPageGroupViewModel(hideGroupIfAllCommandsAreDisabled: true);
			group.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Indicator_SeparatePaneGroupCaption);
			group.BarItems.Add(CreateIndicatorPaneEditor());
			group.BarItems.Add(CreateIndicatorAxisYEditor());
			return group;
		}
		RibbonItemViewModelBase CreateIndicatorPaneEditor() {
			var command = new ChangeIndicatorPaneCommand(this);
			const string comboBoxItemsSourcePath = "DiagramModel.AllPanes";
			IValueConverter converter = new IndicatorPaneConverter();
			var editor = new BarDynamicComboBoxViewModel(command, this, BindingPathToSelectedModel, comboBoxItemsSourcePath, converter);
			return editor;
		}
		RibbonItemViewModelBase CreateIndicatorAxisYEditor() {
			var command = new ChangeIndicatorAxisYCommand(this);
			const string comboBoxItemsSourcePath = "DiagramModel.AllAxesY";
			IValueConverter converter = new IndicatorAxisYConverter();
			var editor = new BarDynamicComboBoxViewModel(command, this, BindingPathToSelectedModel, comboBoxItemsSourcePath, converter);
			return editor;
		}
		#endregion
	}
}
