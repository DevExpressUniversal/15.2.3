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
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.Utils;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Charts;
namespace DevExpress.Charts.Designer.Native {
	public sealed partial class WpfChartModel : ChartModelElement {
		RibbonPageCategoryViewModel CreateDefaultCategoryModel() {
			RibbonPageCategoryViewModel defaultCategory = new RibbonPageCategoryViewModel(this);
			defaultCategory.Pages.Add(CreateChartPage());
			defaultCategory.Pages.Add(CreatePage_Elements());
			return defaultCategory;
		}
		#region Chart page
		RibbonPageViewModel CreateChartPage() {
			RibbonPageViewModel chartPage = new RibbonPageViewModel(this, RibbonPagesNames.DefaultCategoryChartPage);
			chartPage.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_MainPageTitle);
			chartPage.Groups.Add( CreateAddSeriesGroup());
			chartPage.Groups.Add(CreateChangeChartTypeGroup());
			chartPage.Groups.Add(CreatePalletteGroup());
			chartPage.Groups.Add(CreateXYDiagram2DOptionsGroup());
			chartPage.Groups.Add(CreateSimpleDiagram2DOptionsGroup());
			chartPage.Groups.Add(CreateCircularDiagram2DOptionsGroup());
			chartPage.Groups.Add(CreateDiagram3DOptionsGroup());
			return chartPage;
		}
		#region Add Series group
		RibbonPageGroupViewModel CreateAddSeriesGroup() {
			RibbonPageGroupViewModel pageGroupAddSeries = new RibbonPageGroupViewModel();
			pageGroupAddSeries.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Main_AddSeriesManualyGroupTitle);
			pageGroupAddSeries.BarItems.Add(CreateAddSeriesGallerySplitButton());
			return pageGroupAddSeries;
		}
		RibbonItemViewModelBase CreateAddSeriesGallerySplitButton() {
			BarSplitButtonGalleryViewModel addSeriesGallerySplitButton = new BarSplitButtonGalleryViewModel(null);
			addSeriesGallerySplitButton.InitialVisibleRowCount = 9;
			addSeriesGallerySplitButton.InitialVisibleColCount = 7;
			addSeriesGallerySplitButton.IsGroupCaptionVisible = DefaultBoolean.True;
			addSeriesGallerySplitButton.SizeMode = GallerySizeMode.Vertical;
			AddSeriesGalleryCreator galleryCreator = new AddSeriesGalleryCreator();
			galleryCreator.CreateSeriesViewGallery(this, addSeriesGallerySplitButton);
			return addSeriesGallerySplitButton;
		}
		#endregion
		#region Chnge Chart Type group
		RibbonPageGroupViewModel CreateChangeChartTypeGroup() {
			RibbonPageGroupViewModel pageGroupChangeChartType = new RibbonPageGroupViewModel();
			pageGroupChangeChartType.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Main_ChangeChartTypeGroupTitle);
			pageGroupChangeChartType.BarItems.Add(CreateChangeChartTypeInRibbonGallery());
			return pageGroupChangeChartType;
		}
		InRibbonGalleryViewModel CreateChangeChartTypeInRibbonGallery() {
			InRibbonGalleryViewModel changeChartTypeInRibbonGallery = new InRibbonGalleryViewModel();
			changeChartTypeInRibbonGallery.ColCount = 7;
			changeChartTypeInRibbonGallery.DropDownGalleryRowCount = 9;
			ChangeChartTypeGalleryCreator galleryCreator = new ChangeChartTypeGalleryCreator();
			galleryCreator.CreateSeriesViewGallery(this, changeChartTypeInRibbonGallery);
			return changeChartTypeInRibbonGallery;
		}
		#endregion
		#region Palette group
		RibbonPageGroupViewModel CreatePalletteGroup() {
			RibbonPageGroupViewModel pageGroupPalette = new RibbonPageGroupViewModel();
			pageGroupPalette.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Main_Palette);
			pageGroupPalette.BarItems.Add(CreatePaletteInRibbonGallery());
			return pageGroupPalette;
		}
		InRibbonGalleryViewModel CreatePaletteInRibbonGallery() {
			InRibbonGalleryViewModel inRibbonGalleryPalette = new InRibbonGalleryViewModel();
			inRibbonGalleryPalette.ColCount = 1;
			inRibbonGalleryPalette.DropDownGalleryRowCount = 9;
			FillPaletteInRibbonGallery(inRibbonGalleryPalette);
			return inRibbonGalleryPalette;
		}
		void FillPaletteInRibbonGallery(InRibbonGalleryViewModel inRibbonGalleryPalette) {
			const double indent = 3.0;
			Size squareSize = new Size(32, 54);
			GalleryGroupViewModel galleryGroup = new GalleryGroupViewModel();
			inRibbonGalleryPalette.Groups.Add(galleryGroup);
			var paletteTypes = Assembly.GetAssembly(typeof(ChartControl)).GetTypes().Where(x => (x.IsSubclassOf(typeof(PredefinedPalette)) && !x.IsAbstract));
			foreach (Type paletteType in paletteTypes) {
				PredefinedPalette palette = (PredefinedPalette)Activator.CreateInstance(paletteType);
				DrawingGroup drawing = new DrawingGroup();
				for (int i = 0; i < palette.Count; i++) {
					Color color = palette[i];
					double x = (i + 1) * indent + (i + 1) * squareSize.Width;
					Rect rect = new Rect(new Point(x, indent), squareSize);
					RectangleGeometry geometry = new RectangleGeometry(rect);
					GeometryDrawing square = new GeometryDrawing(new SolidColorBrush(color), new Pen(), geometry);
					drawing.Children.Add(square);
				}
				DrawingImage paletteImage = new DrawingImage(drawing);
				paletteImage.Freeze();
				BarButtonViewModel galleryItem = new BarButtonViewModel(new ChangeChartPalette(this, palette));
				galleryItem.Caption = palette.PaletteName;
				galleryItem.LargeGlyph = paletteImage;
				galleryGroup.Items.Add(galleryItem);
			}
		}
		#endregion
		#region XYDiagram2D Options group
		RibbonPageGroupViewModel CreateXYDiagram2DOptionsGroup() {
			RibbonPageGroupViewModel xyDiagram2DPropertiesGroup = new RibbonPageGroupViewModel(hideGroupIfAllCommandsAreDisabled: true);
			xyDiagram2DPropertiesGroup.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Main_XYDiagram2DPropertiesGrop);
			xyDiagram2DPropertiesGroup.BarItems.Add(CreateRotatedCheckButton());
			xyDiagram2DPropertiesGroup.BarItems.Add(CreatePaneOrientationGallery());
			xyDiagram2DPropertiesGroup.BarItems.Add(new BarSeparatorViewModel());
			xyDiagram2DPropertiesGroup.BarItems.Add(CreateNavigationHeader());
			xyDiagram2DPropertiesGroup.BarItems.Add(CreateEnableAxisXNavigation());
			xyDiagram2DPropertiesGroup.BarItems.Add(CrateEnableAxisYNavigation());
			return xyDiagram2DPropertiesGroup;
		}
		RibbonItemViewModelBase CreateRotatedCheckButton() {
			ToggleXYDiagram2DRotatedCommand command = new ToggleXYDiagram2DRotatedCommand(this);
			BarCheckButtonViewModel xyDiagram2DRotatedCheckButtonVM = new BarCheckButtonViewModel(command, this, "DiagramModel", new DiagramReversedConverter());
			return xyDiagram2DRotatedCheckButtonVM;
		}
		RibbonItemViewModelBase CreatePaneOrientationGallery() {
			BarDropDownButtonGalleryViewModel gallery = new BarDropDownButtonGalleryViewModel();
			gallery.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Main_PaneOrientationButtonCaption);
			gallery.InitialVisibleColCount = 1;
			gallery.InitialVisibleRowCount = 2;
			gallery.IsItemCaptionVisible = true;
			gallery.ItemCheckMode = GalleryItemCheckMode.Single;
			GalleryGroupViewModel group = new GalleryGroupViewModel();
			gallery.Groups.Add(group);
			BarEditValueItemViewModel horizontal = new BarEditValueItemViewModel(new ChangePanesOrientationCommand(this, Orientation.Horizontal), this, "DiagramModel", new DiagramModelToBoolConverter());
			group.Items.Add(horizontal);
			BarEditValueItemViewModel vertical = new BarEditValueItemViewModel(new ChangePanesOrientationCommand(this, Orientation.Vertical), this, "DiagramModel", new DiagramModelToBoolConverter());
			group.Items.Add(vertical);
			return gallery;
		}
		RibbonItemViewModelBase CreateNavigationHeader() {
			string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Main_NavigationHeader);
			BarStaticTextViewModel header = new BarStaticTextViewModel(caption);
			return header;
		}
		RibbonItemViewModelBase CreateEnableAxisXNavigation() {
			IValueConverter converter = new DiagramEnableAxisNavigationConverter(ChangeDiagramAxisNavigationEnabledCommand.AxisKind.X);
			BarCheckEditViewModel enableAxisXNavigationCheck = new BarCheckEditViewModel(new ChangeDiagramAxisNavigationEnabledCommand(this, ChangeDiagramAxisNavigationEnabledCommand.AxisKind.X), this, "DiagramModel", converter);
			enableAxisXNavigationCheck.EditWidth = 15;
			return enableAxisXNavigationCheck;
		}
		RibbonItemViewModelBase CrateEnableAxisYNavigation() {
			IValueConverter converter = new DiagramEnableAxisNavigationConverter(ChangeDiagramAxisNavigationEnabledCommand.AxisKind.Y);
			BarCheckEditViewModel enableAxisYNavigationCheck = new BarCheckEditViewModel(new ChangeDiagramAxisNavigationEnabledCommand(this, ChangeDiagramAxisNavigationEnabledCommand.AxisKind.Y), this, "DiagramModel", converter);
			enableAxisYNavigationCheck.EditWidth = 15;
			return enableAxisYNavigationCheck;
		}
		#endregion
		#region SimpleDiagram2D Options group
		RibbonPageGroupViewModel CreateSimpleDiagram2DOptionsGroup() {
			RibbonPageGroupViewModel simpleDiagram2DOptionsGroup = new RibbonPageGroupViewModel(hideGroupIfAllCommandsAreDisabled: true);
			simpleDiagram2DOptionsGroup.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Main_SimpleDiagramPropertiesGrop);
			simpleDiagram2DOptionsGroup.BarItems.Add(CreateLayoutDirectionGallery<SimpleDiagram2D>());
			simpleDiagram2DOptionsGroup.BarItems.Add(CreateSimpleDiagramLayoutDimensionTitle());
			simpleDiagram2DOptionsGroup.BarItems.Add(CreateDimensionEdit<SimpleDiagram2D>());
			return simpleDiagram2DOptionsGroup;
		}
		RibbonItemViewModelBase CreateLayoutDirectionGallery<DiagramType>() where DiagramType : Diagram {
			BarDropDownButtonGalleryViewModel gallery = new BarDropDownButtonGalleryViewModel(1, 2, true, false, GalleryItemCheckMode.Single);
			gallery.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Main_SimpleDiagramLayoutDirection);
			GalleryGroupViewModel group = new GalleryGroupViewModel();
			gallery.Groups.Add(group);
			ChartCommandBase horizontalLayoutCommand = new SetSimpleDiagramLayoutDirectionCommand<DiagramType>(this, LayoutDirection.Horizontal);
			group.Items.Add(new BarEditValueItemViewModel(horizontalLayoutCommand, this, "DiagramModel", new LayoutDirectionToBoolConverter<DiagramType>()));
			ChartCommandBase verticalLayoutCommand = new SetSimpleDiagramLayoutDirectionCommand<DiagramType>(this, LayoutDirection.Vertical);
			group.Items.Add(new BarEditValueItemViewModel(verticalLayoutCommand, this, "DiagramModel", new LayoutDirectionToBoolConverter<DiagramType>()));
			return gallery;
		}
		RibbonItemViewModelBase CreateSimpleDiagramLayoutDimensionTitle() {
			string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Main_SimpleDiagramLayoutDimensionWithoutColon);
			RibbonItemViewModelBase header = new BarStaticTextViewModel(caption);
			return header;
		}
		RibbonItemViewModelBase CreateDimensionEdit<DiagramType>() where DiagramType : Diagram {
			var command = new SetSimpleDiagramLayoutDimensionCommand<DiagramType>(this);
			IValueConverter converter = new DiagramModelToSimpleDiagramDimentionConverter();
			BarSpinEditViewModel edit = new BarSpinEditViewModel(command, this, "DiagramModel", converter);
			edit.EditWidth = 65;
			edit.MinValue = 1;
			edit.MaxValue = 50;
			return edit;
		}
		#endregion
		#region CircularDiagram2D Options group
		RibbonPageGroupViewModel CreateCircularDiagram2DOptionsGroup() {
			RibbonPageGroupViewModel circularDiagramOptionsGroup = new RibbonPageGroupViewModel(hideGroupIfAllCommandsAreDisabled: true);
			circularDiagramOptionsGroup.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Main_CircularDiagramOptionsGroup);
			circularDiagramOptionsGroup.BarItems.Add(CreateShapeStyleGallery());
			circularDiagramOptionsGroup.BarItems.Add(CreateRotationDirectionGallery());
			circularDiagramOptionsGroup.BarItems.Add(new BarSeparatorViewModel());
			circularDiagramOptionsGroup.BarItems.Add(CreateCircularDiagramStartAngleTitle());
			circularDiagramOptionsGroup.BarItems.Add(CreateStartAngleSpinEditor());
			return circularDiagramOptionsGroup;
		}
		RibbonItemViewModelBase CreateShapeStyleGallery() {
			BarDropDownButtonGalleryViewModel gallery = new BarDropDownButtonGalleryViewModel(1, 2, true, false, GalleryItemCheckMode.Single);
			gallery.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Main_CircularDiagramShapeStyle);
			GalleryGroupViewModel group = new GalleryGroupViewModel();
			gallery.Groups.Add(group);
			var setCircleCommand = new ChangeCircularDiagramShapeStyle(this, CircularDiagramShapeStyle.Circle);
			group.Items.Add(new BarEditValueItemViewModel(setCircleCommand, this, "DiagramModel", new DiagramModelToBoolShapeStyleConverter()));
			var setPolygonCommand = new ChangeCircularDiagramShapeStyle(this, CircularDiagramShapeStyle.Polygon);
			group.Items.Add(new BarEditValueItemViewModel(setPolygonCommand, this, "DiagramModel", new DiagramModelToBoolShapeStyleConverter()));
			return gallery;
		}
		RibbonItemViewModelBase CreateRotationDirectionGallery() {
			BarDropDownButtonGalleryViewModel gallery = new BarDropDownButtonGalleryViewModel(1, 2, true, false, GalleryItemCheckMode.Single);
			gallery.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Main_CircularDiagramRotationDirection);
			GalleryGroupViewModel group = new GalleryGroupViewModel();
			gallery.Groups.Add(group);
			var setClockwiseCommand = new ChangeCircularDiagramRotationDirection(this, CircularDiagramRotationDirection.Clockwise);
			BarEditValueItemViewModel clockwise = new BarEditValueItemViewModel(setClockwiseCommand, this, "DiagramModel", new DiagramModelToBoolRotationDirectionConverter());
			group.Items.Add(clockwise);
			var setCounterclockwiseCommand = new ChangeCircularDiagramRotationDirection(this, CircularDiagramRotationDirection.Counterclockwise);
			BarEditValueItemViewModel counterclockwise = new BarEditValueItemViewModel(setCounterclockwiseCommand, this, "DiagramModel", new DiagramModelToBoolRotationDirectionConverter());
			group.Items.Add(counterclockwise);
			return gallery;
		}
		RibbonItemViewModelBase CreateCircularDiagramStartAngleTitle() {
			string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Main_CircularDiagramStartAngle);
			RibbonItemViewModelBase header = new BarStaticTextViewModel(caption);
			return header;
		}
		RibbonItemViewModelBase CreateStartAngleSpinEditor() {
			var command = new ChangeCircularDiagramStartAngleCommand(this);
			BarSpinEditViewModel spin = new BarSpinEditViewModel(command, this, "DiagramModel", new DiagramModelToDoubleStartAngleConverter());
			spin.IsFloatValue = true;
			spin.MinValue = -360M;
			spin.MaxValue = 360M;
			spin.Increment = 10M;
			return spin;
		}
		#endregion
		#region Diagram3D Options group
		RibbonPageGroupViewModel CreateDiagram3DOptionsGroup() {
			RibbonPageGroupViewModel diagram3dOptionsGroup = new RibbonPageGroupViewModel(hideGroupIfAllCommandsAreDisabled: true);
			diagram3dOptionsGroup.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Main_Diagram3DOptionsGroup);
			diagram3dOptionsGroup.BarItems.Add(CreatePerspectiveAngleEditor());
			diagram3dOptionsGroup.BarItems.Add(CreateZoomPercentEditor());
			diagram3dOptionsGroup.BarItems.Add(CreateDimensionEdit<SimpleDiagram3D>());
			diagram3dOptionsGroup.BarItems.Add(CreateLayoutDirectionGallery<SimpleDiagram3D>());
			return diagram3dOptionsGroup;
		}
		RibbonItemViewModelBase CreatePerspectiveAngleEditor() {
			var command = new ChangeDiagram3DPerspectiveAngleCommand(this);
			BarSpinEditViewModel spin = new BarSpinEditViewModel(command, this, "DiagramModel", new DiagramModelToDoublePerspectiveAngleConverter());
			spin.EditWidth = 65;
			spin.IsFloatValue = true;
			spin.MinValue = 0M;
			spin.MaxValue = 175M;
			spin.Increment = 5M;
			return spin;
		}
		RibbonItemViewModelBase CreateZoomPercentEditor() {
			var command = new ChangeDiagram3DZoomPercentCommand(this);
			BarSpinEditViewModel spin = new BarSpinEditViewModel(command, this, "DiagramModel", new DiagramModelToDoubleZoomPercentConverter());
			spin.EditWidth = 65;
			spin.IsFloatValue = true;
			spin.MinValue = 10M;
			spin.MaxValue = 500M;
			spin.Increment = 10M;
			return spin;
		}
		#endregion
		#endregion
		#region Elements page
		RibbonPageViewModel CreatePage_Elements() {
			RibbonPageViewModel elementsPage = new RibbonPageViewModel(this, RibbonPagesNames.DefaultCategoryElementsPage);
			elementsPage.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_ElementsPageTitle);
			elementsPage.Groups.Add(CreateChartAreaElementsGroup());
			elementsPage.Groups.Add(CreateAxesGroup());
			elementsPage.Groups.Add(CreateAxisElementsGroup());
			elementsPage.Groups.Add(CreateIndicatorsGroup());
			return elementsPage;
		}
		#region Chart Area Elements group
		RibbonPageGroupViewModel CreateChartAreaElementsGroup() {
			RibbonPageGroupViewModel chartAreaElementsGroup = new RibbonPageGroupViewModel();
			chartAreaElementsGroup.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Elements_ChartAreaElementsGroupTitle);
			chartAreaElementsGroup.BarItems.Add(CreateLegendPositionGalleryDropDownButton());
			chartAreaElementsGroup.BarItems.Add(CreateAddPaneGallerySplitButton());
			chartAreaElementsGroup.BarItems.Add(CreateAddChartTitleGalleryDropDownButton());
			return chartAreaElementsGroup;
		}
		RibbonItemViewModelBase CreateLegendPositionGalleryDropDownButton() {
			string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Elements_Legend);
			const string glyphName = "Legend";
			return CreateLegendPositionGalleryDropDownButton(caption, glyphName);
		}
		RibbonItemViewModelBase CreateLegendPositionGalleryDropDownButton(string caption, string glyphName) {
			BarDropDownButtonGalleryViewModel changeLegendPositionButton = new BarDropDownButtonGalleryViewModel();
			changeLegendPositionButton.Caption = caption;
			changeLegendPositionButton.ImageName = GlyphUtils.BarItemImages + glyphName;
			changeLegendPositionButton.InitialVisibleColCount = 5;
			changeLegendPositionButton.InitialVisibleRowCount = 5;
			changeLegendPositionButton.ItemCheckMode = GalleryItemCheckMode.Single;
			changeLegendPositionButton.AllowToolTips = false;
			GalleryGroupViewModel galleryGroup = new GalleryGroupViewModel();
			changeLegendPositionButton.Groups.Add(galleryGroup);
			for (int i = 0; i < 5; i++) {
				VerticalPosition vertPosition = (VerticalPosition)i;
				for (int j = 0; j < 5; j++) {
					HorizontalPosition horizontalPosition = (HorizontalPosition)j;
					ChartCommandBase command = new ChangeLegendPositionCommand(this, horizontalPosition, vertPosition);
					BarEditValueItemViewModel galleryItem = new BarEditValueItemViewModel(command, this, "LegendModel", new LegendPositionToBoolConverter());
					galleryGroup.Items.Add(galleryItem);
				}
			}
			return changeLegendPositionButton;
		}
		RibbonItemViewModelBase CreateAddPaneGallerySplitButton() {
			BarSplitButtonGalleryViewModel addPaneGalleryButton = new BarSplitButtonGalleryViewModel(new AddPaneVerticalCommand(this));
			addPaneGalleryButton.InitialVisibleColCount = 1;
			addPaneGalleryButton.InitialVisibleRowCount = 2;
			addPaneGalleryButton.IsItemCaptionVisible = true;
			addPaneGalleryButton.IsItemDescriptionVisible = true;
			GalleryGroupViewModel galleryGroup = new GalleryGroupViewModel();
			addPaneGalleryButton.Groups.Add(galleryGroup);
			SplitButtonGalleryItemViewModel verticalItem = new SplitButtonGalleryItemViewModel(new AddPaneVerticalCommand(this), addPaneGalleryButton);
			SplitButtonGalleryItemViewModel horizontalItem = new SplitButtonGalleryItemViewModel(new AddPaneHorizontalCommand(this), addPaneGalleryButton);
			galleryGroup.Items.Add(verticalItem);
			galleryGroup.Items.Add(horizontalItem);
			return addPaneGalleryButton;
		}
		RibbonItemViewModelBase CreateAddChartTitleGalleryDropDownButton() {
			BarSplitButtonGalleryViewModel addChartTitleBtn = new BarSplitButtonGalleryViewModel(new AddChartTitleCommand(this, Dock.Top, HorizontalAlignment.Center));
			addChartTitleBtn.InitialVisibleColCount = 3;
			addChartTitleBtn.InitialVisibleRowCount = 3;
			addChartTitleBtn.AllowToolTips = false;
			addChartTitleBtn.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Elements_AddChartTitle);
			GalleryGroupViewModel galleryGroup = new GalleryGroupViewModel();
			addChartTitleBtn.Groups.Add(galleryGroup);
			string glyphPath = GlyphUtils.GalleryItemImages + @"AddChartTitle\";
			BarButtonViewModel topNearItem = new BarButtonViewModel(new AddChartTitleCommand(this, Dock.Top, HorizontalAlignment.Left));
			topNearItem.Glyph = GlyphUtils.GetGlyphByPath(glyphPath + "TopNear");
			topNearItem.LargeGlyph = GlyphUtils.GetLargeGlyphByPath(glyphPath + "TopNear");
			galleryGroup.Items.Add(topNearItem);
			BarButtonViewModel topCenterItem = new BarButtonViewModel(new AddChartTitleCommand(this, Dock.Top, HorizontalAlignment.Center));
			topCenterItem.Glyph = GlyphUtils.GetGlyphByPath(glyphPath + "TopCenter");
			topCenterItem.LargeGlyph = GlyphUtils.GetLargeGlyphByPath(glyphPath + "TopCenter");
			galleryGroup.Items.Add(topCenterItem);
			BarButtonViewModel topFarItem = new BarButtonViewModel(new AddChartTitleCommand(this, Dock.Top, HorizontalAlignment.Right));
			topFarItem.Glyph = GlyphUtils.GetGlyphByPath(glyphPath + "TopRight");
			topFarItem.LargeGlyph = GlyphUtils.GetLargeGlyphByPath(glyphPath + "TopRight");
			galleryGroup.Items.Add(topFarItem);
			BarButtonViewModel leftCenterItem = new BarButtonViewModel(new AddChartTitleCommand(this, Dock.Left, VerticalAlignment.Center));
			leftCenterItem.Glyph = GlyphUtils.GetGlyphByPath(glyphPath + "LeftCenter");
			leftCenterItem.LargeGlyph = GlyphUtils.GetLargeGlyphByPath(glyphPath + "LeftCenter");
			galleryGroup.Items.Add(leftCenterItem);
			BarButtonViewModel noneItem = new BarButtonViewModel(new EmptyChartCommand(this));
			galleryGroup.Items.Add(noneItem);
			BarButtonViewModel rightCenterItem = new BarButtonViewModel(new AddChartTitleCommand(this, Dock.Right, VerticalAlignment.Center));
			rightCenterItem.Glyph = GlyphUtils.GetGlyphByPath(glyphPath + "RightCenter");
			rightCenterItem.LargeGlyph = GlyphUtils.GetLargeGlyphByPath(glyphPath + "RightCenter");
			galleryGroup.Items.Add(rightCenterItem);
			BarButtonViewModel bottomNearItem = new BarButtonViewModel(new AddChartTitleCommand(this, Dock.Bottom, HorizontalAlignment.Left));
			bottomNearItem.Glyph = GlyphUtils.GetGlyphByPath(glyphPath + "BottomLeft");
			bottomNearItem.LargeGlyph = GlyphUtils.GetLargeGlyphByPath(glyphPath + "BottomLeft");
			galleryGroup.Items.Add(bottomNearItem);
			BarButtonViewModel bottomCenterItem = new BarButtonViewModel(new AddChartTitleCommand(this, Dock.Bottom, HorizontalAlignment.Center));
			bottomCenterItem.Glyph = GlyphUtils.GetGlyphByPath(glyphPath + "BottomCenter");
			bottomCenterItem.LargeGlyph = GlyphUtils.GetLargeGlyphByPath(glyphPath + "BottomCenter");
			galleryGroup.Items.Add(bottomCenterItem);
			BarButtonViewModel bottomFarItem = new BarButtonViewModel(new AddChartTitleCommand(this, Dock.Bottom, HorizontalAlignment.Right));
			bottomFarItem.Glyph = GlyphUtils.GetGlyphByPath(glyphPath + "BottomRight");
			bottomFarItem.LargeGlyph = GlyphUtils.GetLargeGlyphByPath(glyphPath + "BottomRight");
			galleryGroup.Items.Add(bottomFarItem);
			return addChartTitleBtn;
		}
		#endregion
		#region Axes group
		RibbonPageGroupViewModel CreateAxesGroup() {
			RibbonPageGroupViewModel axesGroup = new RibbonPageGroupViewModel();
			axesGroup.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Elements_AxesGroupTitle);
			axesGroup.BarItems.Add(CreateAddAxisXButton());
			axesGroup.BarItems.Add(CreateAddAxisYButton());
			return axesGroup;
		}
		RibbonItemViewModelBase CreateAddAxisXButton() {
			BarButtonViewModel addAxisXButton = new BarButtonViewModel(new AddSecondaryAxisX(this));
			return addAxisXButton;
		}
		RibbonItemViewModelBase CreateAddAxisYButton() {
			BarButtonViewModel addAxisYButton = new BarButtonViewModel(new AddSecondaryAxisY(this));
			return addAxisYButton;
		}
		#endregion
		#region Axis Elements group
		RibbonPageGroupViewModel CreateAxisElementsGroup() {
			RibbonPageGroupViewModel axesElementsGroup = new RibbonPageGroupViewModel();
			axesElementsGroup.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Elements_AxesElementsGroupTitle);
			axesElementsGroup.BarItems.Add(CreateAddConstantLineGallerySplitButton());
			axesElementsGroup.BarItems.Add(CreateAddStripGallerySplitButton());
			return axesElementsGroup;
		}
		RibbonItemViewModelBase CreateAddConstantLineGallerySplitButton() {
			BarSplitButtonGalleryViewModel galleryButton = new BarSplitButtonGalleryViewModel(new AddConstantLineYCommand(this));
			galleryButton.InitialVisibleColCount = 1;
			galleryButton.InitialVisibleRowCount = 2;
			galleryButton.IsItemCaptionVisible = true;
			galleryButton.IsItemDescriptionVisible = true;
			GalleryGroupViewModel group = new GalleryGroupViewModel();
			galleryButton.Groups.Add(group);
			SplitButtonGalleryItemViewModel constantLineYItem = new SplitButtonGalleryItemViewModel(new AddConstantLineYCommand(this), galleryButton);
			group.Items.Add(constantLineYItem);
			SplitButtonGalleryItemViewModel constantLineXItem = new SplitButtonGalleryItemViewModel(new AddConstantLineXCommand(this), galleryButton);
			group.Items.Add(constantLineXItem);
			return galleryButton;
		}
		RibbonItemViewModelBase CreateAddStripGallerySplitButton() {
			BarSplitButtonGalleryViewModel galleryBtn = new BarSplitButtonGalleryViewModel(new AddStripYCommand(this));
			galleryBtn.InitialVisibleColCount = 1;
			galleryBtn.InitialVisibleRowCount = 2;
			galleryBtn.IsItemCaptionVisible = true;
			galleryBtn.IsItemDescriptionVisible = true;
			GalleryGroupViewModel group = new GalleryGroupViewModel();
			galleryBtn.Groups.Add(group);
			BarButtonViewModel addStripXCommand = new BarButtonViewModel(new AddStripXCommand(this));
			group.Items.Add(addStripXCommand);
			BarButtonViewModel addStripYCommand = new BarButtonViewModel(new AddStripYCommand(this));
			group.Items.Add(addStripYCommand);
			return galleryBtn;
		}
		#endregion
		#region Indicators group
		RibbonPageGroupViewModel CreateIndicatorsGroup() {
			RibbonPageGroupViewModel indicatorsGroup = new RibbonPageGroupViewModel();
			indicatorsGroup.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Elements_IndicatorsGroup);
			indicatorsGroup.BarItems.Add(CreateAddIndicatorGallery());
			return indicatorsGroup;
		}
		RibbonItemViewModelBase CreateAddIndicatorGallery() {
			BarDropDownButtonGalleryWithComboViewModel gallery = new BarDropDownButtonGalleryWithComboViewModel(this, "DiagramModel.Diagram.Series", new SeriesCollectionToNonVisualSeriesPresentationArrayConverter());
			gallery.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Elements_AddIndicator);
			gallery.ImageName = GlyphUtils.BarItemImages + "AddIndicator";
			gallery.InitialVisibleColCount = 1;
			gallery.IsItemCaptionVisible = true;
			gallery.SizeMode = GallerySizeMode.Vertical;
			gallery.IsGroupCaptionVisible = DefaultBoolean.True;
			gallery.SelectedComboBoxItemToGalleryItemCommandParameterConverter = new NonVisualSeriesPresentationToSeriesConverter();
			gallery.Groups.Add(CreateSimpleIndicatorsGalleryGroup());
			gallery.Groups.Add(CreateFibonacciIndicatorsGalleryGroup());
			gallery.Groups.Add(CreateMovingAverageIndicatorsGalleryGroup());
			gallery.Groups.Add(CreateOscillatorIndicatorsGalleryGroup());
			gallery.Groups.Add(CreateTrendIndicatorsGalleryGroup());
			gallery.Groups.Add(CreatePriceIndicatorsGalleryGroup());
			return gallery;
		}
		GalleryGroupViewModel CreateSimpleIndicatorsGalleryGroup() {
			GalleryGroupViewModel simpleIndicatorsGroup = new GalleryGroupViewModel();
			simpleIndicatorsGroup.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Elements_AddIndicator_SimpleIndicatorsGalleryGroup);
			simpleIndicatorsGroup.Items.Add(new BarButtonViewModel(new AddIndicatorCommand<RegressionLine>(this)));
			simpleIndicatorsGroup.Items.Add(new BarButtonViewModel(new AddIndicatorCommand<TrendLine>(this)));
			return simpleIndicatorsGroup;
		}
		GalleryGroupViewModel CreateFibonacciIndicatorsGalleryGroup() {
			GalleryGroupViewModel fibonacciIndicatorsGroup = new GalleryGroupViewModel();
			fibonacciIndicatorsGroup.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Elements_AddIndicator_FibonacciIndicatorsGalleryGroup);
			fibonacciIndicatorsGroup.Items.Add(new BarButtonViewModel(new AddIndicatorCommand<FibonacciArcs>(this)));
			fibonacciIndicatorsGroup.Items.Add(new BarButtonViewModel(new AddIndicatorCommand<FibonacciFans>(this)));
			fibonacciIndicatorsGroup.Items.Add(new BarButtonViewModel(new AddIndicatorCommand<FibonacciRetracement>(this)));
			return fibonacciIndicatorsGroup;
		}
		GalleryGroupViewModel CreateMovingAverageIndicatorsGalleryGroup() {
			GalleryGroupViewModel movingAverageIndicatorsGroup = new GalleryGroupViewModel();
			movingAverageIndicatorsGroup.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Elements_AddIndicator_MovingAverageIndicatorsGalleryGroup);
			movingAverageIndicatorsGroup.Items.Add(new BarButtonViewModel(new AddIndicatorCommand<SimpleMovingAverage>(this)));
			movingAverageIndicatorsGroup.Items.Add(new BarButtonViewModel(new AddIndicatorCommand<WeightedMovingAverage>(this)));
			movingAverageIndicatorsGroup.Items.Add(new BarButtonViewModel(new AddIndicatorCommand<ExponentialMovingAverage>(this)));
			movingAverageIndicatorsGroup.Items.Add(new BarButtonViewModel(new AddIndicatorCommand<TriangularMovingAverage>(this)));
			movingAverageIndicatorsGroup.Items.Add(new BarButtonViewModel(new AddIndicatorCommand<TripleExponentialMovingAverageTema>(this)));
			return movingAverageIndicatorsGroup;
		}
		GalleryGroupViewModel CreateOscillatorIndicatorsGalleryGroup() {
			GalleryGroupViewModel oscillatorsGroup = new GalleryGroupViewModel();
			oscillatorsGroup.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Elements_AddIndicator_OscillatorsGalleryGroup);
			oscillatorsGroup.Items.Add(new BarButtonViewModel(new AddIndicatorCommand<AverageTrueRange>(this)));
			oscillatorsGroup.Items.Add(new BarButtonViewModel(new AddIndicatorCommand<CommodityChannelIndex>(this)));
			oscillatorsGroup.Items.Add(new BarButtonViewModel(new AddIndicatorCommand<DetrendedPriceOscillator>(this)));
			oscillatorsGroup.Items.Add(new BarButtonViewModel(new AddIndicatorCommand<MovingAverageConvergenceDivergence>(this)));
			oscillatorsGroup.Items.Add(new BarButtonViewModel(new AddIndicatorCommand<RateOfChange>(this)));
			oscillatorsGroup.Items.Add(new BarButtonViewModel(new AddIndicatorCommand<RelativeStrengthIndex>(this)));
			oscillatorsGroup.Items.Add(new BarButtonViewModel(new AddIndicatorCommand<TripleExponentialMovingAverageTrix>(this)));
			oscillatorsGroup.Items.Add(new BarButtonViewModel(new AddIndicatorCommand<ChaikinsVolatility>(this)));
			oscillatorsGroup.Items.Add(new BarButtonViewModel(new AddIndicatorCommand<WilliamsR>(this)));
			return oscillatorsGroup;
		}
		GalleryGroupViewModel CreateTrendIndicatorsGalleryGroup() {
			GalleryGroupViewModel trendIndicatorsGroup = new GalleryGroupViewModel();
			trendIndicatorsGroup.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Elements_AddIndicator_TrendIndicatorsGalleryGroup);
			trendIndicatorsGroup.Items.Add(new BarButtonViewModel(new AddIndicatorCommand<BollingerBands>(this)));
			trendIndicatorsGroup.Items.Add(new BarButtonViewModel(new AddIndicatorCommand<MassIndex>(this)));
			trendIndicatorsGroup.Items.Add(new BarButtonViewModel(new AddIndicatorCommand<StandardDeviation>(this)));
			return trendIndicatorsGroup;
		}
		GalleryGroupViewModel CreatePriceIndicatorsGalleryGroup() {
			GalleryGroupViewModel priceIndicatorsGroup = new GalleryGroupViewModel();
			priceIndicatorsGroup.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Elements_AddIndicator_PriceIndicatorsGalleryGroup);
			priceIndicatorsGroup.Items.Add(new BarButtonViewModel(new AddIndicatorCommand<MedianPrice>(this)));
			priceIndicatorsGroup.Items.Add(new BarButtonViewModel(new AddIndicatorCommand<TypicalPrice>(this)));
			priceIndicatorsGroup.Items.Add(new BarButtonViewModel(new AddIndicatorCommand<WeightedClose>(this)));
			return priceIndicatorsGroup;
		}
		#endregion
		#endregion
	}
}
