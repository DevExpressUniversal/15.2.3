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

using DevExpress.Diagram.Core;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Diagram.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Diagram.Core.Themes;
using System.Windows.Shapes;
namespace DevExpress.Xpf.Diagram {
	[TemplatePart(Name = PART_Presenter, Type = typeof(ShapePresenter))]
	[TemplatePart(Name = PART_Text, Type = typeof(TextBlock))]
	[TemplatePart(Name = PART_Layout, Type = typeof(StackPanel))]
	[TemplatePart(Name = PART_Viewbox, Type = typeof(Viewbox))]
	public class ShapeToolboxPreview : Control<ShapeToolboxPreview> {
		#region DEBUGTEST
#if DEBUGTEST
		public ContentPresenter PresenterForTests { get { return presenter; } }
#endif
		#endregion
		const string PART_Presenter = "PART_ShapePresenter";
		const string PART_Text = "PART_TextBlock";
		const string PART_Layout = "PART_LayoutElement";
		const string PART_Viewbox= "PART_Viewbox";
		TextBlock textBlock;
		ContentPresenter presenter;
		StackPanel layoutElement;
		Viewbox viewbox;
		public static readonly DependencyProperty StrokeProperty;
		public static readonly DependencyProperty StrokeThicknessProperty;
		public static readonly DependencyProperty StrokeDashArrayProperty;
		public static readonly DependencyProperty ItemToolProperty;
		public static readonly DependencyProperty ThemeProperty;
		public static readonly DependencyProperty ViewModeProperty;
		public static readonly DependencyProperty IsCompactProperty;
		public static readonly DependencyProperty DiagramProperty;
		static ShapeToolboxPreview() {
			DependencyPropertyRegistrator<ShapeToolboxPreview>.New()
				.AddOwner(x => x.Stroke, out StrokeProperty, DiagramShape.StrokeProperty, null, FrameworkPropertyMetadataOptions.AffectsRender)
				.AddOwner(x => x.StrokeThickness, out StrokeThicknessProperty, DiagramShape.StrokeThicknessProperty, 1, FrameworkPropertyMetadataOptions.AffectsRender)
				.AddOwner(x => x.StrokeDashArray, out StrokeDashArrayProperty, DiagramShape.StrokeDashArrayProperty, null, FrameworkPropertyMetadataOptions.AffectsRender)
				.Register(x => x.ItemTool, out ItemToolProperty, null, x => x.UpdatePreview(), FrameworkPropertyMetadataOptions.AffectsRender)
				.Register(x => x.Theme, out ThemeProperty, null, x => x.UpdatePreview(), FrameworkPropertyMetadataOptions.AffectsRender)
				.Register(x => x.ViewMode, out ViewModeProperty, ShapeToolboxPreviewMode.IconsAndNames, x => x.OnViewModeChanged(), FrameworkPropertyMetadataOptions.AffectsRender)
				.Register(x => x.IsCompact, out IsCompactProperty, false, x => x.OnIsCompactChanged(), FrameworkPropertyMetadataOptions.AffectsRender)
				.Register(x => x.Diagram, out DiagramProperty, null, x => x.UpdatePreview())
				;
		}
		public ItemTool ItemTool {
			get { return (ItemTool)GetValue(ItemToolProperty); }
			set { SetValue(ItemToolProperty, value); }
		}
		public Brush Stroke {
			get { return (Brush)GetValue(StrokeProperty); }
			set { SetValue(StrokeProperty, value); }
		}
		public double StrokeThickness {
			get { return (double)GetValue(StrokeThicknessProperty); }
			set { SetValue(StrokeThicknessProperty, value); }
		}
		public DoubleCollection StrokeDashArray {
			get { return (DoubleCollection)GetValue(StrokeDashArrayProperty); }
			set { SetValue(StrokeDashArrayProperty, value); }
		}
		public DiagramTheme Theme {
			get { return (DiagramTheme)GetValue(ThemeProperty); }
			set { SetValue(ThemeProperty, value); }
		}
		public ShapeToolboxPreviewMode ViewMode {
			get { return (ShapeToolboxPreviewMode)GetValue(ViewModeProperty); }
			set { SetValue(ViewModeProperty, value); }
		}
		public bool IsCompact {
			get { return (bool)GetValue(IsCompactProperty); }
			set { SetValue(IsCompactProperty, value); }
		}
		public DiagramControl Diagram {
			get { return (DiagramControl)GetValue(DiagramProperty); }
			set { SetValue(DiagramProperty, value); }
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			textBlock = (TextBlock)GetTemplateChild(PART_Text);
			presenter = (ContentPresenter)GetTemplateChild(PART_Presenter);
			layoutElement = (StackPanel)GetTemplateChild(PART_Layout);
			viewbox = (Viewbox)GetTemplateChild(PART_Viewbox);
			UpdatePreview();
		}
		void UpdatePreview() {
			string name = ItemTool != null ? ItemTool.ToolName : null;
			if (textBlock != null)
				textBlock.Text = name;
			if (presenter != null && ItemTool != null && Diagram != null) {
				var diagramItem = ItemTool.CreateItem(Diagram);
				diagramItem.Size = ItemTool.DefaultItemSize;
				presenter.Content = diagramItem;
			}
			ToolTip = name;
			OnIsCompactChanged();
		}
		void OnViewModeChanged() {
			if (!IsCompact)
				SetViewMode(ViewMode);
		}
		void OnIsCompactChanged() {
			if (IsCompact)
				SetCompactView();
			else
				SetViewMode(ViewMode);
		}
		void SetViewMode(ShapeToolboxPreviewMode mode) {
			if (layoutElement == null || textBlock == null || viewbox == null) return;
			if (mode != ShapeToolboxPreviewMode.NamesUnderIcons) {
				layoutElement.Orientation = Orientation.Horizontal;
				textBlock.HorizontalAlignment = HorizontalAlignment.Left;
				textBlock.TextAlignment = TextAlignment.Left;
			} else {
				layoutElement.Orientation = Orientation.Vertical;
				textBlock.HorizontalAlignment = HorizontalAlignment.Center;
				textBlock.TextAlignment = TextAlignment.Center;
			}
			if (mode == ShapeToolboxPreviewMode.IconsOnly) {
				textBlock.Visibility = Visibility.Collapsed;
				viewbox.Visibility = Visibility.Visible;
			} else if (mode == ShapeToolboxPreviewMode.NamesOnly) {
				textBlock.Visibility = Visibility.Visible;
				viewbox.Visibility = Visibility.Collapsed;
			} else {
				textBlock.Visibility = Visibility.Visible;
				viewbox.Visibility = Visibility.Visible;
			}
		}
		void SetCompactView() {
			if (layoutElement != null && textBlock != null && viewbox != null) {
				textBlock.Visibility = Visibility.Collapsed;
				viewbox.Visibility = Visibility.Visible;
			}
		}
	}
	public class SelectionPreview : Control<SelectionPreview> {
	}
	public class DragPreview : Control<DragPreview> {
		readonly DiagramItem item;
#if DEBUGTEST
		public DiagramItem ItemForTests { get { return item; } }
#endif
		public DragPreview(DiagramItem item) {
			this.item = item;
			this.Background = new VisualBrush(item) { AutoLayoutContent = false };
		}
	}
	[TemplatePart(Name = PART_Line1, Type = typeof(Polyline))]
	[TemplatePart(Name = PART_Line2, Type = typeof(Line))]
	[TemplatePart(Name = PART_Line3, Type = typeof(Line))]
	[TemplatePart(Name = PART_Text, Type = typeof(TextBlock))]
	public class ShapeStylePresenter : ShapePresenter {
		const string PART_Line1 = "PART_Line1";
		const string PART_Line2 = "PART_Line2";
		const string PART_Line3 = "PART_Line3";
		const string PART_Text = "PART_Text";
		Polyline line1;
		Line line2;
		Line line3;
		TextBlock text;
		public static readonly DependencyProperty TextProperty;
		public static readonly DependencyProperty StyleIdProperty;
		public static readonly DependencyProperty DiagramProperty;
		public string Text {
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		public DiagramItemStyleId StyleId {
			get { return (DiagramItemStyleId)GetValue(StyleIdProperty); }
			set { SetValue(StyleIdProperty, value); }
		}
		public DiagramControl Diagram {
			get { return (DiagramControl)GetValue(DiagramProperty); }
			set { SetValue(DiagramProperty, value); }
		}
		static ShapeStylePresenter() {
			DependencyPropertyRegistrator<ShapeStylePresenter>.New()
				.Register(x => x.Text, out TextProperty, null, FrameworkPropertyMetadataOptions.AffectsRender)
				.Register(x => x.StyleId, out StyleIdProperty, null, (d, e) => ((ShapeStylePresenter)d).UpdateStyle())
				.Register(x => x.Diagram, out DiagramProperty, null, (d, e) => ((ShapeStylePresenter)d).UpdateStyle())
				.OverrideDefaultStyleKey()
				;
		}
		protected override Size MeasureOverride(Size constraint) {
			Shape = BasicShapes.RoundedRectangle.GetShape(new Size(constraint.Width, constraint.Height), BasicShapes.RoundedRectangle.DefaultParameters);
			return base.MeasureOverride(constraint);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			line1 = (Polyline)GetTemplateChild(PART_Line1);
			line2 = (Line)GetTemplateChild(PART_Line2);
			line3 = (Line)GetTemplateChild(PART_Line3);
			text = (TextBlock)GetTemplateChild(PART_Text);
			SetVisibility(Visibility.Collapsed, Visibility.Collapsed);
			UpdateStyle();
		}
		void UpdateStyle() {
			Style = (Diagram != null && Diagram.Theme != null) ? Diagram.Theme.CreateShapeStyle(StyleId) : null;
			if (Style == null || line1 == null || line2 == null || text == null) return;
			if (DiagramConnectorStyleId.Styles.FirstOrDefault(x => x == StyleId) != null) {
				Stroke = new SolidColorBrush(Colors.Transparent);
				Background = new SolidColorBrush(Colors.Transparent);   
				Brush linesStrokeBrush = GetStylePropertyValue<Brush>(Style, "Stroke");
				line1.Stroke = linesStrokeBrush;
				line2.Stroke = linesStrokeBrush;
				line3.Stroke = linesStrokeBrush;
				line1.StrokeDashArray = GetStylePropertyValue<DoubleCollection>(Style, "StrokeDashArray");
				SetVisibility(Visibility.Visible, Visibility.Collapsed);
			} else
				SetVisibility(Visibility.Collapsed, Visibility.Visible);
		}
		void SetVisibility(Visibility linesVisibility, Visibility textVisibility) {
			text.Visibility = textVisibility;
			line1.Visibility = linesVisibility;
			line2.Visibility = linesVisibility;
			line3.Visibility = linesVisibility;
		}
		T GetStylePropertyValue<T>(Style style, string name) {
			foreach (var item in style.Setters) {
				if (((Setter)item).Property.Name == name) {
					return (T)((Setter)item).Value;
				}
			}
			return default(T);
		}
	}
	[TemplatePart(Name = PART_RectangleVariant1, Type = typeof(Rectangle))]
	[TemplatePart(Name = PART_RectangleVariant2, Type = typeof(Rectangle))]
	[TemplatePart(Name = PART_RectangleVariant3, Type = typeof(Rectangle))]
	[TemplatePart(Name = PART_RectangleVariant4, Type = typeof(Rectangle))]
	[TemplatePart(Name = PART_Line1Variant1, Type = typeof(Polyline))]
	[TemplatePart(Name = PART_Line2Variant1, Type = typeof(Line))]
	[TemplatePart(Name = PART_Line3Variant1, Type = typeof(Line))]
	[TemplatePart(Name = PART_TextVariant1, Type = typeof(TextBlock))]
	public class ThemePresenter : Control<ThemePresenter> {
		const string PART_RectangleVariant1 = "PART_RectangleVariant1";
		const string PART_RectangleVariant2 = "PART_RectangleVariant2";
		const string PART_RectangleVariant3 = "PART_RectangleVariant3";
		const string PART_RectangleVariant4 = "PART_RectangleVariant4";
		const string PART_Line1Variant1 = "PART_Line1Variant1";
		const string PART_Line2Variant1 = "PART_Line2Variant1";
		const string PART_Line3Variant1 = "PART_Line3Variant1";
		const string PART_TextVariant1 = "PART_TextVariant1";
		Rectangle rectangleVariant1;
		Rectangle rectangleVariant2;
		Rectangle rectangleVariant3;
		Rectangle rectangleVariant4;
		Polyline line1Variant1;
		Line line2Variant1;
		Line line3Variant1;
		TextBlock textVariant1;
		public static readonly DependencyProperty DiagramThemeProperty;
		public static readonly DependencyProperty TextProperty;
		public DiagramTheme DiagramTheme {
			get { return (DiagramTheme)GetValue(DiagramThemeProperty); }
			set { SetValue(DiagramThemeProperty, value); }
		}
		public string Text {
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		static ThemePresenter() {
			DependencyPropertyRegistrator<ThemePresenter>.New()
				.Register(x => x.Text, out TextProperty, null, FrameworkPropertyMetadataOptions.AffectsRender)
				.Register(x => x.DiagramTheme, out DiagramThemeProperty, null, (d, e) => ((ThemePresenter)d).OnDiagramThemeChanged())
				;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			rectangleVariant1 = (Rectangle)GetTemplateChild(PART_RectangleVariant1);
			rectangleVariant2 = (Rectangle)GetTemplateChild(PART_RectangleVariant2);
			rectangleVariant3 = (Rectangle)GetTemplateChild(PART_RectangleVariant3);
			rectangleVariant4 = (Rectangle)GetTemplateChild(PART_RectangleVariant4);
			line1Variant1 = (Polyline)GetTemplateChild(PART_Line1Variant1);
			line2Variant1 = (Line)GetTemplateChild(PART_Line2Variant1);
			line3Variant1 = (Line)GetTemplateChild(PART_Line3Variant1);
			textVariant1 = (TextBlock)GetTemplateChild(PART_TextVariant1);
			OnDiagramThemeChanged();
		}
		void OnDiagramThemeChanged() {
			if (DiagramTheme == null) return;
			Style variant1 = DiagramTheme.CreateShapeStyle(DiagramShapeStyleId.Variant1);
			Style variant2 = DiagramTheme.CreateShapeStyle(DiagramShapeStyleId.Variant2);
			Style variant3 = DiagramTheme.CreateShapeStyle(DiagramShapeStyleId.Variant3);
			Style variant4 = DiagramTheme.CreateShapeStyle(DiagramShapeStyleId.Variant4);
			SetRectangleStyle(rectangleVariant1, variant1);
			SetRectangleStyle(rectangleVariant2, variant2);
			SetRectangleStyle(rectangleVariant3, variant3);
			SetRectangleStyle(rectangleVariant4, variant4);
			Brush linesStrokeBrush = (GetStylePropertyValue<Brush>(variant1, "Background").ToString() != "#FFFFFFFF")
				? GetStylePropertyValue<Brush>(variant1, "Background") : GetStylePropertyValue<Brush>(variant1, "Stroke");
			line1Variant1.Stroke = linesStrokeBrush;
			line2Variant1.Stroke = linesStrokeBrush;
			line3Variant1.Stroke = linesStrokeBrush;
			textVariant1.Foreground = GetStylePropertyValue<Brush>(variant1, "Foreground");
		}
		void SetRectangleStyle(Rectangle rectangle, Style style) {
			rectangle.Fill = GetStylePropertyValue<Brush>(style, "Background");
			rectangle.Stroke = GetStylePropertyValue<Brush>(style, "Stroke");
			rectangle.StrokeDashArray = GetStylePropertyValue<DoubleCollection>(style, "StrokeDashArray");
		}
		T GetStylePropertyValue<T>(Style style, string name) {
			foreach (var item in style.Setters) {
				if (((Setter)item).Property.Name == name) {
					return (T)((Setter)item).Value;
				}
			}
			return default(T);
		}
	}
}
