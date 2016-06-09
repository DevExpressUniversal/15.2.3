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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.Diagram.Core;
using DevExpress.Diagram.Core.Localization;
using DevExpress.Diagram.Core.Themes;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Utils;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Docking.Base;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Internal;
namespace DevExpress.Xpf.Diagram {
	[DXToolboxBrowsable(true)]
	[ToolboxTabName(AssemblyInfo.DXTabData)]
	[TemplatePart(Name = PART_ToolboxPane, Type = typeof(LayoutPanel))]
	[TemplatePart(Name = PART_PropertiesPane, Type = typeof(LayoutPanel))]
	public class DiagramDesignerControl : DiagramControl {
		const string PART_ToolboxPane = "PART_ToolboxPane";
		const string PART_PropertiesPane = "PART_PropertiesPane";
		LayoutPanel toolboxPane;
		LayoutPanel propertiesPane;
		static PageSizeInfo defaultPageSizeInfo = new PageSizeInfo("Letter", 8.5, 11.0, MeasureUnits.Inches);
		double initialToolboxPaneWidth = 0;
		DiagramToolsItem[] diagramToolsItems;
		PageSizeInfo[] pageSizes;
		CanvasSizeModeInfo[] canvasSizeModes;
		ReadOnlyCollection<DiagramItemStyleId> currentShapeStyles;
		DiagramColorPalette currentPalette;
		Locker colorChangeLock = new Locker();
		Locker toolboxViewModeLock = new Locker();
		Locker propertiesPaneLock = new Locker();
		Locker pageSizePropertiesLock = new Locker();
		DiagramItemEditUnit backgroundItemColor;
		DiagramItemEditUnit foregroundItemColor;
		DiagramItemEditUnit strokeItemColor;
		static DiagramDesignerControl() {
			DependencyPropertyRegistrator<DiagramDesignerControl>.New()
				.OverrideDefaultStyleKey();
		}
		public DiagramDesignerControl() {
			PageSize = defaultPageSizeInfo.Size;
			BindingOperations.SetBinding(this, DesignerThemeProperty, new Binding("Theme") { Source = this, Mode = BindingMode.OneWay });
			BindingOperations.SetBinding(this, SelectionToolsModelInternalProperty, new Binding("SelectionToolsModel") { Source = this, Mode = BindingMode.OneWay });
			BindingOperations.SetBinding(this, PageSizeInternalProperty, new Binding("PageSize") { Source = this, Mode = BindingMode.OneWay });
			BindingOperations.SetBinding(this, CanvasSizeModeInternalProperty, new Binding("CanvasSizeMode") { Source = this, Mode = BindingMode.OneWay });
			SetValue(SelectedSizeModeInfoProperty, CanvasSizeModesInfo.FirstOrDefault(x => x.CanvasSizeMode == CanvasSizeMode));
			currentShapeStyles = DiagramShapeStyleId.Styles;
			RefreshShapeStyles();
			currentPalette = ((DiagramTheme)GetValue(DesignerThemeProperty)).ColorPalette;
			SetValue(PalettesProperty, GetPalletes(currentPalette));
			backgroundItemColor = DiagramItemEditUnit.CreateEditUnit(DiagramItemEditUnit.BackgroundIdProperty, DiagramThemeColorId.Accent1_4);
			foregroundItemColor = DiagramItemEditUnit.CreateEditUnit(DiagramItemEditUnit.ForegroundIdProperty, DiagramThemeColorId.Accent1_4);
			strokeItemColor = DiagramItemEditUnit.CreateEditUnit(DiagramItemEditUnit.StrokeIdProperty, DiagramThemeColorId.Accent1_4);
			SetSplitButtonsColors();
			Loaded += DesignerControl_Loaded;
		}
		public PageSizeInfo[] PageSizesInfo {
			get {
				if (pageSizes == null)
					pageSizes = new[] { defaultPageSizeInfo, new PageSizeInfo("Legal", 8.5, 14.0, MeasureUnits.Inches),
					new PageSizeInfo("A3", 11.69, 16.54, MeasureUnits.Inches), new PageSizeInfo("A4", 8.27, 11.69, MeasureUnits.Inches), new PageSizeInfo("A5", 5.83, 8.27, MeasureUnits.Inches)};
				return pageSizes;
			}
		}
		public CanvasSizeModeInfo[] CanvasSizeModesInfo {
			get {
				if (canvasSizeModes == null)
					canvasSizeModes = new[] { new CanvasSizeModeInfo(CanvasSizeMode.AutoSize), new CanvasSizeModeInfo(CanvasSizeMode.None) };
				return canvasSizeModes;
			}
		}
		public DiagramToolsItem[] DiagramToolsItems {
			get {
				if (diagramToolsItems == null)
					diagramToolsItems = new DiagramToolsItem[] {
											new DiagramToolsItem(new DiagramTool[] { ActiveTool }),
											new DiagramToolsItem(new DiagramTool[] { new ConnectorTool() }),
											new DiagramToolsItem(new DiagramTool[] { new ShapeTool(BasicShapes.Rectangle), new ShapeTool(BasicShapes.Ellipse), new ShapeTool(BasicShapes.RightTriangle),
											new ShapeTool(BasicShapes.Hexagon) }, (s1, s2)=>SelectToolFromToolsItem(s1,s2))
					};
				return diagramToolsItems;
			}
		}
		public static readonly DependencyProperty ToolboxViewModeProperty =
			DependencyProperty.Register("ToolboxViewMode", typeof(ToolboxViewMode), typeof(DiagramDesignerControl),
				new PropertyMetadata(ToolboxViewMode.Normal, (d, e) => ((DiagramDesignerControl)d).OnToolboxViewModeChanged((ToolboxViewMode)e.OldValue, (ToolboxViewMode)e.NewValue)));
		public static readonly DependencyProperty PropertiesPanelVisibilityProperty =
			DependencyProperty.Register("PropertiesPanelVisibility", typeof(Visibility), typeof(DiagramDesignerControl),
				new PropertyMetadata(Visibility.Visible, (d, e) => ((DiagramDesignerControl)d).OnPropertiesPanelVisibilityChanged((Visibility)e.NewValue)));
		public static readonly DependencyProperty DefaultStencilsProperty =
			DependencyProperty.Register("DefaultStencils", typeof(StencilCollection), typeof(DiagramDesignerControl), new PropertyMetadata(new StencilCollection()));
		public ToolboxViewMode ToolboxViewMode {
			get { return (ToolboxViewMode)GetValue(ToolboxViewModeProperty); }
			set { SetValue(ToolboxViewModeProperty, value); }
		}
		public Visibility PropertiesPanelVisibility {
			get { return (Visibility)GetValue(PropertiesPanelVisibilityProperty); }
			set { SetValue(PropertiesPanelVisibilityProperty, value); }
		}
		public StencilCollection DefaultStencils {
			get { return (StencilCollection)GetValue(DefaultStencilsProperty); }
			set { SetValue(DefaultStencilsProperty, value); }
		}
		#region InternalProperties
		static readonly DependencyProperty SelectedToolsItemProperty =
			DependencyProperty.Register("SelectedToolsItem", typeof(DiagramToolsItem), typeof(DiagramDesignerControl),
				new PropertyMetadata(null, (d, e) => ((DiagramDesignerControl)d).OnSelectedToolsItemChanged(e)));
		static readonly DependencyProperty DesignerThemeProperty =
			DependencyProperty.Register("DesignerTheme", typeof(DiagramTheme), typeof(DiagramDesignerControl),
				new PropertyMetadata(DiagramThemes.Office, (d, e) => ((DiagramDesignerControl)d).OnDesignerThemeChanged(e)));
		static readonly DependencyProperty DesignerThemeInternalProperty =
			DependencyProperty.Register("DesignerThemeInternal", typeof(DiagramTheme), typeof(DiagramDesignerControl),
				new PropertyMetadata(DiagramThemes.Office, (d, e) => ((DiagramDesignerControl)d).OnDesignerThemeInternalChanged(e)));
		static readonly DependencyProperty ShapeStylesGalleryIsEnabledProperty =
			DependencyProperty.Register("ShapeStylesGalleryIsEnabled", typeof(bool), typeof(DiagramDesignerControl),
				new PropertyMetadata(false));
		static readonly DependencyProperty ShapeStylesProperty =
			DependencyProperty.Register("ShapeStyles", typeof(ReadOnlyCollection<DiagramItemStyleId>), typeof(DiagramDesignerControl),
				new PropertyMetadata(null));
		static readonly DependencyProperty SelectedPageSizeInfoProperty =
			DependencyProperty.Register("SelectedPageSizeInfo", typeof(PageSizeInfo), typeof(DiagramDesignerControl),
				new PropertyMetadata(defaultPageSizeInfo, (d, e) => ((DiagramDesignerControl)d).OnSelectedPageSizeInfoChanged(e)));
		static readonly DependencyProperty SelectionToolsModelInternalProperty =
			DependencyProperty.Register("SelectionToolsModelInternal", typeof(SelectionToolsModel<IDiagramItem>), typeof(DiagramDesignerControl),
				new PropertyMetadata(default(SelectionToolsModel<IDiagramItem>), (d, e) => ((DiagramDesignerControl)d).OnSelectionToolsModelInternalChanged(e)));
		static readonly DependencyProperty PalettesProperty =
			DependencyProperty.Register("Palettes", typeof(PaletteCollection), typeof(DiagramDesignerControl),
				new PropertyMetadata(null));
		static readonly DependencyProperty ShapeBackgroundColorProperty =
			DependencyProperty.Register("ShapeBackgroundColor", typeof(Color?), typeof(DiagramDesignerControl),
				new PropertyMetadata(null, (d, e) => ((DiagramDesignerControl)d).OnShapeBackgroundColorChanged(e)));
		static readonly DependencyProperty ShapeForegroundColorProperty =
			DependencyProperty.Register("ShapeForegroundColor", typeof(Color?), typeof(DiagramDesignerControl),
				new PropertyMetadata(null, (d, e) => ((DiagramDesignerControl)d).OnShapeForegroundColorChanged(e)));
		static readonly DependencyProperty ShapeStrokeColorProperty =
			DependencyProperty.Register("ShapeStrokeColor", typeof(Color?), typeof(DiagramDesignerControl),
				new PropertyMetadata(null, (d, e) => ((DiagramDesignerControl)d).OnShapeStrokeColorChanged(e)));
		static readonly DependencyProperty BackgroundPaletteNameProperty =
			DependencyProperty.Register("BackgroundPaletteName", typeof(string), typeof(DiagramDesignerControl), new PropertyMetadata(default(string)));
		static readonly DependencyProperty ForegroundPaletteNameProperty =
			DependencyProperty.Register("ForegroundPaletteName", typeof(string), typeof(DiagramDesignerControl), new PropertyMetadata(default(string)));
		static readonly DependencyProperty StrokePaletteNameProperty =
			DependencyProperty.Register("StrokePaletteName", typeof(string), typeof(DiagramDesignerControl), new PropertyMetadata(default(string)));
		static readonly DependencyProperty LastBackgroundColorProperty =
			DependencyProperty.Register("LastBackgroundColor", typeof(Color?), typeof(DiagramDesignerControl),
				new PropertyMetadata(null));
		static readonly DependencyProperty LastForegroundColorProperty =
			DependencyProperty.Register("LastForegroundColor", typeof(Color?), typeof(DiagramDesignerControl),
				new PropertyMetadata(null));
		static readonly DependencyProperty LastStrokeColorProperty =
			DependencyProperty.Register("LastStrokeColor", typeof(Color?), typeof(DiagramDesignerControl),
				new PropertyMetadata(null));
		static readonly DependencyProperty CollapsedButtonCheckedProperty =
			DependencyProperty.Register("CollapsedButtonChecked", typeof(bool), typeof(DiagramDesignerControl),
				new PropertyMetadata(false, (d, e) => ((DiagramDesignerControl)d).OnCollapsedButtonCheckedChanged(e)));
		static readonly DependencyProperty PropertiesPaneIsClosedProperty =
			DependencyProperty.Register("PropertiesPaneIsClosed", typeof(bool), typeof(DiagramDesignerControl),
				new PropertyMetadata(false, (d, e) => ((DiagramDesignerControl)d).OnPropertiesPaneIsClosedChanged((bool)e.NewValue)));
		static readonly DependencyProperty PropertiesPaneIsAutoHiddenProperty =
			DependencyProperty.Register("PropertiesPaneIsAutoHidden", typeof(bool), typeof(DiagramDesignerControl),
				new PropertyMetadata(false, (d, e) => ((DiagramDesignerControl)d).OnPropertiesPaneIsAutoHiddenChanged((bool)e.NewValue)));
		static readonly DependencyProperty CustomPageHeightProperty =
			DependencyProperty.Register("CustomPageHeight", typeof(decimal), typeof(DiagramDesignerControl),
				new PropertyMetadata((decimal)defaultPageSizeInfo.Size.Height));
		static readonly DependencyProperty CustomPageWidthProperty =
			DependencyProperty.Register("CustomPageWidth", typeof(decimal), typeof(DiagramDesignerControl),
				new PropertyMetadata((decimal)defaultPageSizeInfo.Size.Width));
		static readonly DependencyProperty PageSizeInternalProperty =
			DependencyProperty.Register("PageSizeInternal", typeof(Size), typeof(DiagramDesignerControl),
				new PropertyMetadata(default(Size), (d, e) => ((DiagramDesignerControl)d).OnPageSizeInternalChanged((Size)e.NewValue)));
		static readonly DependencyProperty PaperOrientationProperty =
			DependencyProperty.Register("PaperOrientation", typeof(Orientation), typeof(DiagramDesignerControl),
				new PropertyMetadata(Orientation.Vertical, (d, e) => ((DiagramDesignerControl)d).OnPaperOrientationChanged((Orientation)e.NewValue)));
		static readonly DependencyProperty SelectedSizeModeInfoProperty =
			DependencyProperty.Register("SelectedSizeModeInfo", typeof(CanvasSizeModeInfo), typeof(DiagramDesignerControl),
				new PropertyMetadata(null, (d, e) => ((DiagramDesignerControl)d).OnSelectedSizeModeInfoChanged((CanvasSizeModeInfo)e.NewValue)));
		static readonly DependencyProperty CanvasSizeModeInternalProperty =
			DependencyProperty.Register("CanvasSizeModeInternal", typeof(CanvasSizeMode), typeof(DiagramDesignerControl),
				new PropertyMetadata(CanvasSizeMode.AutoSize, (d, e) => ((DiagramDesignerControl)d).OnCanvasSizeModeInternalChanged((CanvasSizeMode)e.NewValue)));
		#endregion
		public IEnumerable TextHorizontalAlignment { get { return new[] { TextAlignment.Left, TextAlignment.Center, TextAlignment.Right, TextAlignment.Justify }; } }
		public IEnumerable TextVerticalAlignment { get { return Enum.GetValues(typeof(VerticalAlignment)).Cast<VerticalAlignment>().Except(new[] { VerticalAlignment.Stretch }); } }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			toolboxPane = (LayoutPanel)GetTemplateChild(PART_ToolboxPane);
			propertiesPane = (LayoutPanel)GetTemplateChild(PART_PropertiesPane);
			if (initialToolboxPaneWidth == 0) {
				initialToolboxPaneWidth = toolboxPane.ItemWidth.Value;
				if (ToolboxViewMode == ToolboxViewMode.Compact)
					toolboxPane.ItemWidth = new GridLength(75);
			}
			propertiesPaneLock.Lock();
			BindingOperations.ClearBinding(this, PropertiesPaneIsClosedProperty);
			BindingOperations.ClearBinding(this, PropertiesPaneIsAutoHiddenProperty);
			BindingOperations.SetBinding(this, PropertiesPaneIsClosedProperty, new Binding("IsClosed") { Source = propertiesPane, Mode = BindingMode.OneWay });
			BindingOperations.SetBinding(this, PropertiesPaneIsAutoHiddenProperty, new Binding("AutoHidden") { Source = propertiesPane, Mode = BindingMode.OneWay });
			propertiesPaneLock.Unlock();
			if (IsLoaded) {
				OnToolboxViewModeChanged(ToolboxViewMode.Normal, ToolboxViewMode);
				OnPropertiesPanelVisibilityChanged(PropertiesPanelVisibility);
			}
		}
		void DesignerControl_Loaded(object sender, RoutedEventArgs e) {
			OnToolboxViewModeChanged(ToolboxViewMode.Normal, ToolboxViewMode);
			OnPropertiesPanelVisibilityChanged(PropertiesPanelVisibility);
		}
		protected override void RaiseSelectionChanged() {
			base.RaiseSelectionChanged();
			if (SelectedItems == null || SelectedItems.Count() == 0) {
				SetValue(ShapeStylesGalleryIsEnabledProperty, false);
				return;
			}
			foreach (var item in SelectedItems) {
				if (item.Controller.GetDiagramItemStylesId() != null) {
					currentShapeStyles = item.Controller.GetDiagramItemStylesId();
					RefreshShapeStyles();
					SetValue(ShapeStylesGalleryIsEnabledProperty, true);
					return;
				}
			}
			SetValue(ShapeStylesGalleryIsEnabledProperty, false);
		}
		void OnSelectedToolsItemChanged(DependencyPropertyChangedEventArgs e) {
			if (e.NewValue != null && ((DiagramToolsItem)e.NewValue).SelectedTool != null)
				ActiveTool = ((DiagramToolsItem)e.NewValue).SelectedTool;
		}
		void OnPageSizeInternalChanged(Size newPageSize) {
			pageSizePropertiesLock.DoIfNotLocked(() => {
				ChangeCustomSizeSetters(newPageSize.Width, newPageSize.Height);
				SelectPageSizeInfo(newPageSize.Width, newPageSize.Height);
				SelectOrientation(newPageSize.Width, newPageSize.Height);
			});
		}
		void OnSelectedPageSizeInfoChanged(DependencyPropertyChangedEventArgs e) {
			if (e.NewValue == null) return;
			pageSizePropertiesLock.DoIfNotLocked(() => {
				double width = ((PageSizeInfo)e.NewValue).Size.Width;
				double height = ((PageSizeInfo)e.NewValue).Size.Height;
				SetRootPageSize(width, height);
				ChangeCustomSizeSetters(width, height);
				SelectOrientation(width, height);
			});
		}
		void OnPaperOrientationChanged(Orientation newOrientation) {
			pageSizePropertiesLock.DoIfNotLocked(() => {
				double width = PageSize.Width;
				double height = PageSize.Height;
				if ((width < height && newOrientation == Orientation.Horizontal) || (width >= height && newOrientation == Orientation.Vertical)) {
					SetRootPageSize(height, width);
					ChangeCustomSizeSetters(height, width);
					SelectPageSizeInfo(height, width);
				}
			});
		}
		void OnDesignerThemeChanged(DependencyPropertyChangedEventArgs e) {
			SetValue(DesignerThemeInternalProperty, (DiagramTheme)e.NewValue);
			RefreshShapeStyles(true);
			if (e.NewValue == null) return;
			currentPalette = ((DiagramTheme)e.NewValue).ColorPalette;
			SetValue(PalettesProperty, GetPalletes(currentPalette));
			SetSplitButtonsColors();
		}
		void OnDesignerThemeInternalChanged(DependencyPropertyChangedEventArgs e) {
			DiagramTheme theme = (DiagramTheme)e.NewValue;
			if (theme != this.Theme)
				RootToolsModel["Theme"] = theme;
		}
		void OnCollapsedButtonCheckedChanged(DependencyPropertyChangedEventArgs e) {
			if (this.IsInDesignMode())
				return;
			toolboxViewModeLock.Lock();
			ToolboxViewMode = (bool)e.NewValue ? ToolboxViewMode.Compact : ToolboxViewMode.Normal;
			toolboxViewModeLock.Unlock();
		}
		void OnToolboxViewModeChanged(ToolboxViewMode oldMode, ToolboxViewMode newMode) {
			if (toolboxPane == null) return;
			colorChangeLock.DoIfNotLocked(() => {
				switch (newMode) {
					case ToolboxViewMode.Normal:
						toolboxPane.Visibility = Visibility.Visible;
						SetValue(CollapsedButtonCheckedProperty, false);
						if (oldMode == ToolboxViewMode.Collapsed)
							toolboxPane.ItemWidth = new GridLength(initialToolboxPaneWidth);
						break;
					case ToolboxViewMode.Compact:
						toolboxPane.Visibility = Visibility.Visible;
						SetValue(CollapsedButtonCheckedProperty, true);
						break;
					case ToolboxViewMode.Collapsed:
						toolboxPane.Visibility = Visibility.Collapsed;
						break;
				}
			});
		}
		void OnPropertiesPaneIsClosedChanged(bool newState) {
			if (propertiesPaneLock.IsLocked) return;
			SetValue(PropertiesPanelVisibilityProperty, newState ? Visibility.Collapsed : Visibility.Visible);
		}
		void OnPropertiesPaneIsAutoHiddenChanged(bool newState) {
			if (propertiesPaneLock.IsLocked) return;
			if (newState)
				SetValue(PropertiesPanelVisibilityProperty, Visibility.Hidden);
			else if (propertiesPane != null && !propertiesPane.Closed)
				SetValue(PropertiesPanelVisibilityProperty, Visibility.Visible);
		}
		void OnPropertiesPanelVisibilityChanged(Visibility newVisibility) {
			if (propertiesPane == null) return;
			switch (newVisibility) {
				case Visibility.Visible:
					propertiesPane.Closed = false;
					propertiesPane.AutoHidden = false;
					break;
				case Visibility.Hidden:
					propertiesPane.Closed = false;
					propertiesPane.AutoHidden = true;
					propertiesPane.AutoHideExpandState = AutoHideExpandState.Hidden;
					break;
				case Visibility.Collapsed:
					propertiesPane.Closed = true;
					propertiesPane.AutoHidden = false;
					break;
			}
		}
		void OnSelectionToolsModelInternalChanged(DependencyPropertyChangedEventArgs e) {
			if (e.NewValue == null) return;
			var backgroundColor = SelectionToolsModel["BackgroundColor"] as DiagramItemEditUnit;
			var foregroundColor = SelectionToolsModel["ForegroundColor"] as DiagramItemEditUnit;
			var strokeColor = SelectionToolsModel["StrokeColor"] as DiagramItemEditUnit;
			colorChangeLock.Lock();
			SetValue(ShapeBackgroundColorProperty, GetBackgroundColor(backgroundColor));
			SetValue(ShapeForegroundColorProperty, GetForegroundColor(foregroundColor));
			SetValue(ShapeStrokeColorProperty, GetStrokeColor(strokeColor));
			colorChangeLock.Unlock();
		}
		void OnShapeBackgroundColorChanged(DependencyPropertyChangedEventArgs e) {
			OnColorChanged(LastBackgroundColorProperty, BackgroundPaletteNameProperty, ref backgroundItemColor, "BackgroundColor", e.NewValue, DiagramItemEditUnit.BackgroundProperty, DiagramItemEditUnit.BackgroundIdProperty);
		}
		void OnShapeForegroundColorChanged(DependencyPropertyChangedEventArgs e) {
			OnColorChanged(LastForegroundColorProperty, ForegroundPaletteNameProperty, ref foregroundItemColor, "ForegroundColor", e.NewValue, DiagramItemEditUnit.ForegroundProperty, DiagramItemEditUnit.ForegroundIdProperty);
		}
		void OnShapeStrokeColorChanged(DependencyPropertyChangedEventArgs e) {
			OnColorChanged(LastStrokeColorProperty, StrokePaletteNameProperty, ref strokeItemColor, "StrokeColor", e.NewValue, DiagramItemEditUnit.StrokeProperty, DiagramItemEditUnit.StrokeIdProperty);
		}
		void OnCanvasSizeModeInternalChanged(CanvasSizeMode mode) {
			SetValue(SelectedSizeModeInfoProperty, CanvasSizeModesInfo.FirstOrDefault(x => x.CanvasSizeMode == mode));
		}
		void OnSelectedSizeModeInfoChanged(CanvasSizeModeInfo info) {
			if (info.CanvasSizeMode != CanvasSizeMode)
				RootToolsModel["CanvasSizeMode"] = info.CanvasSizeMode;
		}
		protected override void ApplyBackgroundColor() {
			ApplyColor(ShapeBackgroundColorProperty, backgroundItemColor, "BackgroundColor", unit => GetBackgroundColor(unit));
		}
		protected override void ApplyForegroundColor() {
			ApplyColor(ShapeForegroundColorProperty, foregroundItemColor, "ForegroundColor", unit => GetForegroundColor(unit));
		}
		protected override void ApplyStrokeColor() {
			ApplyColor(ShapeStrokeColorProperty, strokeItemColor, "StrokeColor", unit => GetStrokeColor(unit));
		}
		protected override void DisplayItemProperties() {
			SetValue(PropertiesPanelVisibilityProperty, Visibility.Visible);
		}
		protected override void SetPageSize() {
			double customPageWidth = (double)(decimal)GetValue(CustomPageWidthProperty);
			double customPageHeight = (double)(decimal)GetValue(CustomPageHeightProperty);
			SetRootPageSize(customPageWidth, customPageHeight);
			SelectPageSizeInfo(customPageWidth, customPageHeight);
			SelectOrientation(customPageWidth, customPageHeight);
		}
		void ApplyColor(DependencyProperty shapeColorProperty, DiagramItemEditUnit itemColor, string toolsModelPropertyName, Func<DiagramItemEditUnit, Color?> getColorByEditUnit) {
			if (SelectionToolsModel == null) return;
			colorChangeLock.Lock();
			SelectionToolsModel[toolsModelPropertyName] = itemColor;
			SetValue(shapeColorProperty, getColorByEditUnit(itemColor));
			colorChangeLock.Unlock();
		}
		void OnColorChanged(DependencyProperty lastColorProperty, DependencyProperty paletteNameProperty, ref DiagramItemEditUnit itemColor, string toolsModelPropertyName, object newColorObject, PropertyDescriptor brushProperty, PropertyDescriptor colorIdProperty) {
			if (newColorObject == null) return;
			DiagramItemEditUnit color = null;
			colorChangeLock.DoIfNotLocked(() => {
				Color newColor = (Color)newColorObject;
				SetValue(lastColorProperty, newColor);
				DiagramThemeColorId? id = null;
				if ((string)GetValue(paletteNameProperty) == EditorLocalizer.GetString(EditorStringId.ColorEdit_ThemeColorsCaption))
					id = GetColorIdByColor(newColor);
				if (id == null)
					color = DiagramItemEditUnit.CreateEditUnit(brushProperty, DiagramItemStyleHelper.ColorToBrush(newColor));
				else
					color = DiagramItemEditUnit.CreateEditUnit(colorIdProperty, (DiagramThemeColorId)id);
				if (SelectionToolsModel != null)
					SelectionToolsModel[toolsModelPropertyName] = color;
			});
			if (color != null)
				itemColor = color;
		}
		void RefreshShapeStyles(bool createNew = false) {
			if (currentShapeStyles == null)
				return;
			if (createNew || GetValue(ShapeStylesProperty) == null || !Enumerable.SequenceEqual(currentShapeStyles, (ReadOnlyCollection<DiagramItemStyleId>)GetValue(ShapeStylesProperty))) {
				SetValue(ShapeStylesProperty, new ReadOnlyCollection<DiagramItemStyleId>(currentShapeStyles));
			}
		}
		void SelectToolFromToolsItem(DiagramToolsItem toolsItem, DiagramTool tool) {
			SetValue(SelectedToolsItemProperty, toolsItem);
			ActiveTool = tool;
		}
		PaletteCollection GetPalletes(DiagramColorPalette palette) {
			return new PaletteCollection("", new[] {
				new CustomPalette(EditorLocalizer.GetString(EditorStringId.ColorEdit_ThemeColorsCaption), palette.ThemeColors),
				new CustomPalette(EditorLocalizer.GetString(EditorStringId.ColorEdit_StandardColorsCaption), PredefinedColorCollections.Standard)});
		}
		DiagramThemeColorId? GetColorIdByColor(Color color) {
			if (currentPalette == null) return null;
			foreach (var enumValue in Enum.GetValues(typeof(DiagramThemeColorId)))
				if (currentPalette.GetColorByColorId((DiagramThemeColorId)enumValue) == color)
					return (DiagramThemeColorId?)enumValue;
			return null;
		}
		void SetSplitButtonsColors() {
			SetValue(LastBackgroundColorProperty, GetBackgroundColor(backgroundItemColor));
			SetValue(LastForegroundColorProperty, GetForegroundColor(foregroundItemColor));
			SetValue(LastStrokeColorProperty, GetStrokeColor(strokeItemColor));
		}
		Color? GetBackgroundColor(DiagramItemEditUnit editUnit) {
			return GetEditUnitColor(editUnit, DiagramItemEditUnit.BackgroundProperty, DiagramItemEditUnit.BackgroundIdProperty);
		}
		Color? GetForegroundColor(DiagramItemEditUnit editUnit) {
			return GetEditUnitColor(editUnit, DiagramItemEditUnit.ForegroundProperty, DiagramItemEditUnit.ForegroundIdProperty);
		}
		Color? GetStrokeColor(DiagramItemEditUnit editUnit) {
			return GetEditUnitColor(editUnit, DiagramItemEditUnit.StrokeProperty, DiagramItemEditUnit.StrokeIdProperty);
		}
		Color? GetEditUnitColor(DiagramItemEditUnit editUnit, PropertyDescriptor brushProperty, PropertyDescriptor colorIdProperty) {
			if (editUnit == null)
				return null;
			if (editUnit.Contains(brushProperty))
				return DiagramItemStyleHelper.BrushToColor((Brush)editUnit.GetValue(brushProperty));
			var colorId = (DiagramThemeColorId?)editUnit.GetValue(colorIdProperty);
			if (colorId.HasValue)
				return currentPalette.GetColorByColorId(colorId.Value);
			return Colors.Transparent;
		}
		void SetRootPageSize(double width, double height) {
			pageSizePropertiesLock.Lock();
			if (PageSize.Width != width || PageSize.Height != height)
				RootToolsModel["PageSize"] = new Size(width, height);
			pageSizePropertiesLock.Unlock();
		}
		void SelectPageSizeInfo(double width, double height) {
			pageSizePropertiesLock.Lock();
			SetValue(SelectedPageSizeInfoProperty, PageSizesInfo.FirstOrDefault(x => x.Size.Width == width && x.Size.Height == height));
			pageSizePropertiesLock.Unlock();
		}
		void SelectOrientation(double width, double height) {
			pageSizePropertiesLock.Lock();
			SetValue(PaperOrientationProperty, (width < height) ? Orientation.Vertical : Orientation.Horizontal);
			pageSizePropertiesLock.Unlock();
		}
		void ChangeCustomSizeSetters(double width, double height) {
			SetValue(CustomPageWidthProperty, (decimal)width);
			SetValue(CustomPageHeightProperty, (decimal)height);
		}
	}
	public enum ToolboxViewMode {
		Normal,
		Compact,
		Collapsed
	}
	public class DiagramToolsItem : ObservableObject {
		DiagramTool selectedTool;
		Action<DiagramToolsItem, DiagramTool> selectedAction;
		public DiagramToolsItem(IEnumerable<DiagramTool> tools, Action<DiagramToolsItem, DiagramTool> action = null) {
			Tools = new List<DiagramTool>();
			ToolsInfo = new List<DiagramToolInfo>();
			foreach (var tool in tools) {
				Tools.Add(tool);
				ToolsInfo.Add(new DiagramToolInfo(tool.ToolName, tool.ToolId, (s) => SelectTool(s)));
			}
			SelectedTool = Tools[0];
			ToolsInfo[0].IsSelected = true;
			selectedAction = action;
		}
		public List<DiagramTool> Tools { get; set; }
		public List<DiagramToolInfo> ToolsInfo { get; set; }
		public DiagramTool SelectedTool {
			get { return selectedTool; }
			set { SetPropertyValue("SelectedTool", ref selectedTool, value); }
		}
		void SelectTool(DiagramToolInfo toolInfo) {
			foreach (DiagramTool tool in Tools) {
				if (tool.ToolId == toolInfo.Id) {
					SelectedTool = tool;
					if (selectedAction != null)
						selectedAction(this, SelectedTool);
				}
			}
		}
	}
	public class CanvasSizeModeInfo {
		public string Name { get; private set; }
		public string Description { get; private set; }
		public CanvasSizeMode CanvasSizeMode { get; private set; }
		public CanvasSizeModeInfo(CanvasSizeMode mode) {
			CanvasSizeMode = mode;
			if (mode == CanvasSizeMode.AutoSize) {
				Name = DiagramControlLocalizer.GetString(DiagramControlStringId.Ribbon_CanvasAutoSize);
				Description = DiagramControlLocalizer.GetString(DiagramControlStringId.Ribbon_CanvasAutoSizeDescription);
			} else if (mode == CanvasSizeMode.None) {
				Name = DiagramControlLocalizer.GetString(DiagramControlStringId.Ribbon_CanvasNoneAutoSize);
				Description = DiagramControlLocalizer.GetString(DiagramControlStringId.Ribbon_CanvasNoneAutoSizeDescription);
			}
		}
	}
	public class PageSizeInfo {
		public string SizeId { get; private set; }
		public Size Size { get; private set; }
		public string Name { get; private set; }
		public string SizeString { get; private set; }
		public PageSizeInfo(string sizeId, double width, double height, MeasureUnit measureUnit = null) {
			SizeId = sizeId;
			double pixelWidth = Math.Round(measureUnit.ToPixels(width) * measureUnit.Multiplier);
			double pixelHeight = Math.Round(measureUnit.ToPixels(height) * measureUnit.Multiplier);
			Size = new Size(pixelWidth, pixelHeight);
			if (measureUnit.Name == DiagramControlLocalizer.GetString(DiagramControlStringId.MeasureUnit_Pixels)) {
				SizeString = string.Format("{0} x {1}", width, height);
			} else if (measureUnit.Name == DiagramControlLocalizer.GetString(DiagramControlStringId.MeasureUnit_Millimeters)) {
				SizeString = string.Format("{0} mm x {1} mm", width, height);
			} else if (measureUnit.Name == DiagramControlLocalizer.GetString(DiagramControlStringId.MeasureUnit_Inches)) {
				SizeString = string.Format("{0}\" x {1}\"", width, height);
			}
			DiagramControlStringId nameId;
			bool result = Enum.TryParse<DiagramControlStringId>(string.Format("PageSize_{0}", SizeId), true, out nameId);
			Name = (result) ? DiagramControlLocalizer.GetString(nameId) : Size.ToString();
		}
	}
	public class DiagramToolInfo : ObservableObject {
		bool isSelected;
		Action<DiagramToolInfo> selectedAction;
		public DiagramToolInfo(string name, string id, Action<DiagramToolInfo> action = null, bool isSelected = false) {
			Name = name;
			Id = id;
			selectedAction = action;
			IsSelected = isSelected;
		}
		public string Name { get; set; }
		public string Id { get; set; }
		public bool IsSelected {
			get { return isSelected; }
			set {
				if (IsSelected == value)
					return;
				if (value == true && selectedAction != null) {
					selectedAction(this);
				}
				isSelected = value;
				RaisePropertyChangedEvent("IsSelected");
			}
		}
	}
	public class DiagramToolsTemplateSelector : DataTemplateSelector {
		public override DataTemplate SelectTemplate(object item, DependencyObject container) {
			DiagramToolsItem toolsItem = item as DiagramToolsItem;
			var resourceKey = String.Empty;
			if (toolsItem.Tools == null || toolsItem.Tools.Count == 0 || toolsItem.Tools.Count == 1)
				resourceKey = "SingleTool";
			else
				resourceKey = "MultiplyTool";
			var element = container as BarItemSelector;
			return element.FindResource(resourceKey) as DataTemplate;
		}
	}
	public class ExtendedColorEdit : ColorEdit {
		public static readonly DependencyProperty EditColorPaletteNameProperty =
			DependencyProperty.Register("EditColorPaletteName", typeof(string), typeof(ExtendedColorEdit), new PropertyMetadata(default(string)));
		public string EditColorPaletteName {
			get { return (string)GetValue(EditColorPaletteNameProperty); }
			set { SetValue(EditColorPaletteNameProperty, value); }
		}
		protected override void SubscribeElementsEvents() {
			if (Gallery != null)
				Gallery.ItemClick += OnGalleryItemClick;
			if (ResetButton != null)
				ResetButton.ItemClick += OnResetButtonClick;
			if (MoreColorsButton != null)
				MoreColorsButton.ItemClick += OnMoreColorsButtonClick;
			if (NoColorButton != null)
				NoColorButton.ItemClick += OnNoColorButtonClick;
		}
		void OnGalleryItemClick(object sender, GalleryItemEventArgs e) {
			EditColorPaletteName = ((ColorGalleryItem)e.Item).Group.Caption.ToString();
			EditStrategy.OnGalleryColorChanged(((ColorGalleryItem)e.Item).Color);
			CloseOwnedPopup(true);
		}
		void OnResetButtonClick(object sender, ItemClickEventArgs e) {
			EditStrategy.OnResetButtonClick();
			CloseOwnedPopup(true);
		}
		void OnMoreColorsButtonClick(object sender, ItemClickEventArgs e) {
			EditColorPaletteName = EditorLocalizer.GetString(EditorStringId.ColorEdit_MoreColorsButtonCaption);
			IColorEdit actualOwner = OwnerPopupEdit != null ? (IColorEdit)OwnerPopupEdit : this;
			CloseOwnedPopup(false);
			ColorChooserDialog = ColorEditHelper.ShowColorChooserDialog(actualOwner);
		}
		void OnNoColorButtonClick(object sender, ItemClickEventArgs e) {
			EditStrategy.OnNoColorButtonClick();
			CloseOwnedPopup(true);
		}
	}
	public class BarMouseCheckOnlyItem : BarCheckItem {
		protected override void OnLoaded(object sender, RoutedEventArgs e) {
			base.OnLoaded(sender, e);
			this.ItemClick += itemClick;
		}
		protected override void OnUnloaded(object sender, RoutedEventArgs e) {
			this.ItemClick -= itemClick;
			base.OnUnloaded(sender, e);
		}
		void itemClick(object sender, ItemClickEventArgs e) {
			if (IsChecked == false)
				IsChecked = true;
		}
	}
	[TypeConverter(typeof(StencilCollectionConverter))]
	public class StencilCollection : ObservableCollection<string> {
		public StencilCollection() : base() { }
		public StencilCollection(IEnumerable<string> collection) : base(collection) { }
		public static StencilCollection Parse(string source) {
			var tokens = source.Split(',', ' ').Where(x => !string.IsNullOrEmpty(x));
			return new StencilCollection(tokens);
		}
	}
	public class StencilCollectionConverter : TypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if(sourceType == typeof(string))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			string source = value as string;
			if(source != null)
				return StencilCollection.Parse(source);
			return base.ConvertFrom(context, culture, value);
		}
	}
}
