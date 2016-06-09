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

using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Bars;
namespace DevExpress.Charts.Designer.Native {
	public sealed partial class WpfChartModel : ChartModelElement {
		RibbonPageCategoryViewModel CreateChartTitleCategoryModel() {
			RibbonPageCategoryViewModel chartTitleOptionsCategory = new RibbonPageCategoryViewModel(this, typeof(WpfChartTitleModel));
			chartTitleOptionsCategory.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ChartTitleOptionsRibbonCategoryTitle);
			RibbonPageViewModel selectedTitlePage = new RibbonPageViewModel(this, RibbonPagesNames.ChartTitleOptionsPage);
			selectedTitlePage.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ChartTitleOptions_SelectedTitle);
			chartTitleOptionsCategory.Pages.Add(selectedTitlePage);
			selectedTitlePage.Groups.Add(CreateTitleGeneralGroup());
			TitleFontFamilyCommand familyCommand = new TitleFontFamilyCommand(this);
			TitleFontSizeCommand sizeCommand = new TitleFontSizeCommand(this);
			TitleFontBoldCommand boldCommand = new TitleFontBoldCommand(this);
			TitleFontItalicCommand italicCommand = new TitleFontItalicCommand(this);
			RibbonPageGroupViewModel fontGroup = CreateFontGroup(BindingPathToSelectedModel, familyCommand, sizeCommand, boldCommand, italicCommand);
			selectedTitlePage.Groups.Add(fontGroup);
			return chartTitleOptionsCategory;
		}
		BarDropDownButtonGalleryViewModel CreateGalleryDropDownButton_СhartTitlePosition() {
			BarDropDownButtonGalleryViewModel chartTitleButton = new BarDropDownButtonGalleryViewModel();
			chartTitleButton.InitialVisibleColCount = 3;
			chartTitleButton.InitialVisibleRowCount = 3;
			chartTitleButton.ItemCheckMode = GalleryItemCheckMode.Single;
			chartTitleButton.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ChartTitleOptions_Position);
			chartTitleButton.ImageName = GlyphUtils.ElementsPageImages + "AddTitle";
			GalleryGroupViewModel galleryGroup = new GalleryGroupViewModel();
			ChartTitlePositionToBoolConverter converter = new ChartTitlePositionToBoolConverter();
			string itemGlyphPath = GlyphUtils.GalleryItemImages + @"AddChartTitle\";
			BarButtonViewModel topNearItem = new BarButtonViewModel(new AddChartTitleCommand(this, Dock.Top, HorizontalAlignment.Left));
			galleryGroup.Items.Add(new BarEditValueItemViewModel(
				new ChartTitlePositionCommand(this, Dock.Top, HorizontalAlignment.Left), this, BindingPathToSelectedModel, converter) {
					Glyph = GlyphUtils.GetGlyphByPath(itemGlyphPath + "TopNear"),
					LargeGlyph = GlyphUtils.GetLargeGlyphByPath(itemGlyphPath + "TopNear")
				}
			);
			galleryGroup.Items.Add(
				new BarEditValueItemViewModel(new ChartTitlePositionCommand(this, Dock.Top, HorizontalAlignment.Center), this, BindingPathToSelectedModel, converter) {
					Glyph = GlyphUtils.GetGlyphByPath(itemGlyphPath + "TopCenter"),
					LargeGlyph = GlyphUtils.GetLargeGlyphByPath(itemGlyphPath + "TopCenter")
				}
			);
			galleryGroup.Items.Add(
				new BarEditValueItemViewModel(new ChartTitlePositionCommand(this, Dock.Top, HorizontalAlignment.Right), this, BindingPathToSelectedModel, converter) {
					Glyph = GlyphUtils.GetGlyphByPath(itemGlyphPath + "TopRight"),
					LargeGlyph = GlyphUtils.GetLargeGlyphByPath(itemGlyphPath + "TopRight")
				}
			);
			galleryGroup.Items.Add(
				new BarEditValueItemViewModel(new ChartTitlePositionCommand(this, Dock.Left, HorizontalAlignment.Center), this, BindingPathToSelectedModel, converter) {
					Glyph = GlyphUtils.GetGlyphByPath(itemGlyphPath + "LeftCenter"),
					LargeGlyph = GlyphUtils.GetLargeGlyphByPath(itemGlyphPath + "LeftCenter")
				}
			);
			galleryGroup.Items.Add(new BarEditValueItemViewModel(new EmptyChartCommand(this), this, BindingPathToSelectedModel, converter));
			galleryGroup.Items.Add(
				new BarEditValueItemViewModel(new ChartTitlePositionCommand(this, Dock.Right, HorizontalAlignment.Center), this, BindingPathToSelectedModel, converter) {
					Glyph = GlyphUtils.GetGlyphByPath(itemGlyphPath + "RightCenter"),
					LargeGlyph = GlyphUtils.GetLargeGlyphByPath(itemGlyphPath + "RightCenter")
				}
			);
			galleryGroup.Items.Add(
				new BarEditValueItemViewModel(new ChartTitlePositionCommand(this, Dock.Bottom, HorizontalAlignment.Left), this, BindingPathToSelectedModel, converter) {
					Glyph = GlyphUtils.GetGlyphByPath(itemGlyphPath + "BottomLeft"),
					LargeGlyph = GlyphUtils.GetLargeGlyphByPath(itemGlyphPath + "BottomLeft")
				}
			);
			galleryGroup.Items.Add(
				new BarEditValueItemViewModel(new ChartTitlePositionCommand(this, Dock.Bottom, HorizontalAlignment.Center), this, BindingPathToSelectedModel, converter) {
					Glyph = GlyphUtils.GetGlyphByPath(itemGlyphPath + "BottomCenter"),
					LargeGlyph = GlyphUtils.GetLargeGlyphByPath(itemGlyphPath + "BottomCenter")
				}
			);
			galleryGroup.Items.Add(new BarEditValueItemViewModel(new ChartTitlePositionCommand(this, Dock.Bottom, HorizontalAlignment.Right), this, BindingPathToSelectedModel, converter) {
					Glyph = GlyphUtils.GetGlyphByPath(itemGlyphPath + "BottomRight"),
					LargeGlyph = GlyphUtils.GetLargeGlyphByPath(itemGlyphPath + "BottomRight")
				}
			);
			chartTitleButton.Groups.Add(galleryGroup);
			return chartTitleButton;
		}
		RibbonPageGroupViewModel CreateTitleGeneralGroup() {
			RibbonPageGroupViewModel generalGroup = new RibbonPageGroupViewModel();
			generalGroup.BarItems.Add(CreateGalleryDropDownButton_СhartTitlePosition());
			generalGroup.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ChartTitleOptions_General);
			generalGroup.BarItems.Add(new BarStaticTextViewModel(ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ChartTitleOptions_Text)));
			generalGroup.BarItems.Add(new BarEditValueItemViewModel(new ChartTitleTextCommand(this), this, BindingPathToSelectedModel, new ChartTitleContentConverter()));
			return generalGroup;
		}
	}
}
