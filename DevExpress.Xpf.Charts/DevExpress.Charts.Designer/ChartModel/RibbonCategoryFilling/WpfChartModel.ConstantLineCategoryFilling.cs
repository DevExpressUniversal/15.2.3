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

using DevExpress.Utils;
using DevExpress.Xpf.Bars;
namespace DevExpress.Charts.Designer.Native {
	public sealed partial class WpfChartModel : ChartModelElement {
		RibbonPageCategoryViewModel CreateConstantLineCategoryViewModel() {
			RibbonPageCategoryViewModel constantLineOptionsCategory = new RibbonPageCategoryViewModel(this, typeof(WpfChartConstantLineModel));
			constantLineOptionsCategory.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ConstantLineCategoryTitle);
			constantLineOptionsCategory.Pages.Add(CreateConstantLineRibbonPageModel());
			return constantLineOptionsCategory;
		}
		RibbonPageViewModel CreateConstantLineRibbonPageModel() {
			RibbonPageViewModel page = new RibbonPageViewModel(this, RibbonPagesNames.ConstantLineOptionsPage);
			page.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ConstantLinePageCategoryTitle);
			RibbonPageGroupViewModel generalGroup = new RibbonPageGroupViewModel();
			page.Groups.Add(generalGroup);
			generalGroup.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ConstantLine_GeneralGroupTitle);
			generalGroup.BarItems.Add(CreateTextEditor_Value());
			generalGroup.BarItems.Add(CreateTextEditor_Thickness());
			generalGroup.BarItems.Add(CreateTextEditor_ConstntLineLegendText());
			generalGroup.BarItems.Add(CreateColorEditor_ConstantLineColor());
			RibbonPageGroupViewModel titleGroup = new RibbonPageGroupViewModel();
			page.Groups.Add(titleGroup);
			titleGroup.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ConstantLine_TitleGroupTitle);
			titleGroup.BarItems.Add(CreateConstantLineTitleGallery());
			titleGroup.BarItems.Add(new BarStaticTextViewModel(ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ConstantLine_TitleText)));
			titleGroup.BarItems.Add(CreateTextEditor_ConstantLineTitleText());
			titleGroup.BarItems.Add(CreateColorEditor_ConstantLineTitleForeground());
			ConstantLineTitleFontFamilyCommand familyCommand = new ConstantLineTitleFontFamilyCommand(this);
			ConstantLineTitleFontSizeCommand sizeCommand = new ConstantLineTitleFontSizeCommand(this);
			ConstantLineTitleFontBoldCommand boldCommand = new ConstantLineTitleFontBoldCommand(this);
			ConstantLineTitleFontItalicCommand italicCommand = new ConstantLineTitleFontItalicCommand(this);
			RibbonPageGroupViewModel fontGroup = CreateFontGroup(BindingPathToSelectedModel, familyCommand, sizeCommand, boldCommand, italicCommand);
			page.Groups.Add(fontGroup);
			return page;
		}
		RibbonItemViewModelBase CreateTextEditor_ConstntLineLegendText() {
			ChangeConstantLineLegendTextCommand command = new ChangeConstantLineLegendTextCommand(this);
			BarEditValueItemViewModel editor = new BarEditValueItemViewModel(command, this, BindingPathToSelectedModel, new ConstantLineLegendTextConverter());
			return editor;
		}
		RibbonItemViewModelBase CreateConstantLineTitleGallery() {
			BarDropDownButtonGalleryViewModel constantLineTitleBtn = new BarDropDownButtonGalleryViewModel();
			constantLineTitleBtn.RibbonStyle = RibbonItemStyles.Large;
			constantLineTitleBtn.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ConstantLine_TitlePosition);
			constantLineTitleBtn.InitialVisibleColCount = 1;
			constantLineTitleBtn.InitialVisibleRowCount = 5;
			constantLineTitleBtn.IsGroupCaptionVisible = DefaultBoolean.False;
			constantLineTitleBtn.ItemCheckMode = GalleryItemCheckMode.Single;
			constantLineTitleBtn.IsItemCaptionVisible = true;
			constantLineTitleBtn.IsItemDescriptionVisible = true;
			GalleryGroupViewModel galleryGroup = new GalleryGroupViewModel(hideGroupIfAllItemsAreDisabled: true);
			constantLineTitleBtn.Groups.Add(galleryGroup);
			BarEditValueItemViewModel noneItem = new BarEditValueItemViewModel(new ChangeConstantLineTitlePositionNone(this), this, BindingPathToSelectedModel, new ConstantLineTitlePositionToBoolConverter());
			galleryGroup.Items.Add(noneItem);
			GalleryGroupViewModel verticalGroup = new GalleryGroupViewModel(hideGroupIfAllItemsAreDisabled: true);
			constantLineTitleBtn.Groups.Add(verticalGroup);
			BarEditValueItemViewModel nearAboveVerticalItem = new BarEditValueItemViewModel(new ChangeConstantLineTitlePositionNearAboveVertical(this), this, BindingPathToSelectedModel, new ConstantLineTitlePositionToBoolConverter());
			verticalGroup.Items.Add(nearAboveVerticalItem);
			BarEditValueItemViewModel nearBelowVerticalItem = new BarEditValueItemViewModel(new ChangeConstantLineTitlePositionNearBelowVertical(this), this, BindingPathToSelectedModel, new ConstantLineTitlePositionToBoolConverter());
			verticalGroup.Items.Add(nearBelowVerticalItem);
			BarEditValueItemViewModel farAboveVerticalItem = new BarEditValueItemViewModel(new ChangeConstantLineTitlePositionFarAboveVertical(this), this, BindingPathToSelectedModel, new ConstantLineTitlePositionToBoolConverter());
			verticalGroup.Items.Add(farAboveVerticalItem);
			BarEditValueItemViewModel farBelowVerticalItem = new BarEditValueItemViewModel(new ChangeConstantLineTitlePositionFarBelowVertical(this), this, BindingPathToSelectedModel, new ConstantLineTitlePositionToBoolConverter());
			verticalGroup.Items.Add(farBelowVerticalItem);
			GalleryGroupViewModel horizontalGroup = new GalleryGroupViewModel(hideGroupIfAllItemsAreDisabled: true);
			constantLineTitleBtn.Groups.Add(horizontalGroup);
			BarEditValueItemViewModel nearAboveHorizontalItem = new BarEditValueItemViewModel(new ChangeConstantLineTitlePositionNearAboveHorizontal(this), this, BindingPathToSelectedModel, new ConstantLineTitlePositionToBoolConverter());
			horizontalGroup.Items.Add(nearAboveHorizontalItem);
			BarEditValueItemViewModel nearBelowHorizontalItem = new BarEditValueItemViewModel(new ChangeConstantLineTitlePositionNearBelowHorizontal(this), this, BindingPathToSelectedModel, new ConstantLineTitlePositionToBoolConverter());
			horizontalGroup.Items.Add(nearBelowHorizontalItem);
			BarEditValueItemViewModel farAboveHorizontalItem = new BarEditValueItemViewModel(new ChangeConstantLineTitlePositionFarAboveHorizontal(this), this, BindingPathToSelectedModel, new ConstantLineTitlePositionToBoolConverter());
			horizontalGroup.Items.Add(farAboveHorizontalItem);
			BarEditValueItemViewModel farBelowHorizontalItem = new BarEditValueItemViewModel(new ChangeConstantLineTitlePositionFarBelowHorizontal(this), this, BindingPathToSelectedModel, new ConstantLineTitlePositionToBoolConverter());
			horizontalGroup.Items.Add(farBelowHorizontalItem);
			return constantLineTitleBtn;
		}
		RibbonItemViewModelBase CreateTextEditor_ConstantLineTitleText() {
			ChangeConstantLineTitleTextCommand command = new ChangeConstantLineTitleTextCommand(this);
			BarEditValueItemViewModel titleTextEditor = new BarEditValueItemViewModel(command, this, BindingPathToSelectedModel, new ConstantLineTitleTextConverter());
			return titleTextEditor;
		}
		RibbonItemViewModelBase CreateTextEditor_Thickness() {
			ChangeConstantLineThicknessCommand command = new ChangeConstantLineThicknessCommand(this);
			RibbonItemViewModelBase result = new BarThicknessEditViewModel(command, this, BindingPathToSelectedModel, 1, 1, 20, new ConstantLineThicknessConverter());
			return result;
		}
		RibbonItemViewModelBase CreateColorEditor_ConstantLineColor() {
			ChangeConstantLineColorCommand command = new ChangeConstantLineColorCommand(this);
			BarColorEditViewModel colorEditor = new BarColorEditViewModel(command, this, BindingPathToSelectedModel, new ConstantLineColorConverter());
			return colorEditor;
		}
		RibbonItemViewModelBase CreateColorEditor_ConstantLineTitleForeground() {
			ChangeConstantLineTitleForegroundCommand command = new ChangeConstantLineTitleForegroundCommand(this);
			BarColorEditViewModel colorEditor = new BarColorEditViewModel(command, this, BindingPathToSelectedModel, new ConstantLineTitleForegroundConverter());
			colorEditor.RibbonStyle = RibbonItemStyles.Large;
			return colorEditor;
		}
		RibbonItemViewModelBase CreateTextEditor_Value() {
			ChangeConstantLineValueCommand command = new ChangeConstantLineValueCommand(this);
			BarEditValueItemViewModel valueEditor = new BarEditValueItemViewModel(command, this, BindingPathToSelectedModel, new ConstantLineValueConverter());
			command.EditorModel = valueEditor;
			return valueEditor;
		}
	}
}
