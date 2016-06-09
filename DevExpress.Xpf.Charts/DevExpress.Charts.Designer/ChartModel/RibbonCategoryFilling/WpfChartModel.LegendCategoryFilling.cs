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
using System.Windows.Controls;
using DevExpress.Utils;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Charts;
using System.Collections.Generic;
namespace DevExpress.Charts.Designer.Native {
	public sealed partial class WpfChartModel : ChartModelElement {
		const string legendModelName = "LegendModel";
		RibbonPageCategoryViewModel CreateLegendCategoryModel() {
			RibbonPageCategoryViewModel legendOptionsCategory = new RibbonPageCategoryViewModel(this, typeof(WpfChartLegendModel));
			legendOptionsCategory.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.LegendOptionsRibbonCategoryTitle);
			RibbonPageViewModel selectedLegendPage = new RibbonPageViewModel(this, RibbonPagesNames.LegendOptionsPage);
			selectedLegendPage.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.LegendOptions_SelectedLegend);
			legendOptionsCategory.Pages.Add(selectedLegendPage);
			selectedLegendPage.Groups.Add(CreateLegendLayoutGroup());
			LegendFontFamilyCommand familyCommand = new LegendFontFamilyCommand(this);
			LegendFontSizeCommand sizeCommand = new LegendFontSizeCommand(this);
			LegendFontBoldCommand boldCommand = new LegendFontBoldCommand(this);
			LegendFontItalicCommand italicCommand = new LegendFontItalicCommand(this);
			RibbonPageGroupViewModel fontGroup = CreateFontGroup(legendModelName, familyCommand, sizeCommand, boldCommand, italicCommand);
			selectedLegendPage.Groups.Add(fontGroup);
			return legendOptionsCategory;
		}
		RibbonItemViewModelBase CreateGalleryDropDownButton_Orientation() {
			BarDropDownButtonGalleryViewModel changeLegendOrientationButton = new BarDropDownButtonGalleryViewModel();
			changeLegendOrientationButton.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.LegendOptions_Orientation);
			changeLegendOrientationButton.InitialVisibleColCount = 1;
			changeLegendOrientationButton.InitialVisibleRowCount = 2;
			changeLegendOrientationButton.ItemCheckMode = GalleryItemCheckMode.Single;
			changeLegendOrientationButton.IsItemCaptionVisible = true;
			changeLegendOrientationButton.IsItemDescriptionVisible = true;
			GalleryGroupViewModel galleryGroup = new GalleryGroupViewModel();
			changeLegendOrientationButton.Groups.Add(galleryGroup);
			foreach (Orientation orientation in Enum.GetValues(typeof(Orientation))) {
				ChartCommandBase command = new ChangeLegendOrientationCommand(this, orientation);
				BarEditValueItemViewModel galleryItem = new BarEditValueItemViewModel(command, this, legendModelName, new LegendOrientationToBoolConverter());
				galleryGroup.Items.Add(galleryItem);
			}
			return changeLegendOrientationButton;
		}
		RibbonPageGroupViewModel CreateLegendLayoutGroup() {
			RibbonPageGroupViewModel legendLayoutGroup = new RibbonPageGroupViewModel();
			legendLayoutGroup.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.LegendOptions_Layout);
			legendLayoutGroup.BarItems.Add(CreateLegendPositionGalleryDropDownButton(ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.LegendPosition), "LegendPosition"));
			legendLayoutGroup.BarItems.Add(new BarCheckButtonViewModel(new ToggleLegendReverseItemsCommand(this), this, legendModelName, new LegendIsReverseConverter()));
			legendLayoutGroup.BarItems.Add(CreateGalleryDropDownButton_Orientation());
			return legendLayoutGroup;
		}
		RibbonPageGroupViewModel CreateFontGroup(string modelName, FontFamilyCommand familyCommand, FontSizeCommand sizeCommand, FontBoldCommand boldCommand, FontItalicCommand italicCommand) {
			RibbonPageGroupViewModel fontGroup = new RibbonPageGroupViewModel();
			fontGroup.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Font);
			fontGroup.BarItems.Add(new BarFontFamilyEditViewModel(familyCommand, this, modelName, new FontFamilyConverter()));
			fontGroup.BarItems.Add(new BarComboBoxViewModel(sizeCommand, this, modelName, new FontSizeConverter()));
			fontGroup.BarItems.Add(new BarCheckButtonViewModel(boldCommand, this, modelName, new FontBoldToBoolConverter()));
			fontGroup.BarItems.Add(new BarCheckButtonViewModel(italicCommand, this, modelName, new FontItalicToBoolConverter()));
			return fontGroup;
		}	 
	}
}
