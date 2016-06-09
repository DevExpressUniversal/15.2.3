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
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Effects;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public class CrosshairPanel : Panel {
		XYDiagram2D Diagram { get { return DataContext as XYDiagram2D; } }
		protected override Size MeasureOverride(Size availableSize) {
			foreach (UIElement child in Children)
				child.Measure(availableSize);
			return MathUtils.ConvertInfinityToDefault(availableSize, 0.0);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			XYDiagram2D diagram = Diagram;
			if (diagram != null)
				diagram.CompleteCrosshairLayout();
			foreach (UIElement child in Children)
				child.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
			return finalSize;
		}
	}
	public class CrosshairAxisLabelsPanel : Panel {
		protected override Size MeasureOverride(Size constraint) {
			foreach (UIElement child in Children) {
				ContentPresenter presenter = child as ContentPresenter;
				if (presenter != null) {
					ICrosshairLabelItem item = presenter.Content as ICrosshairLabelItem;
					if (item != null) {
						presenter.Measure(constraint);
						item.Size = presenter.DesiredSize;
					}
				}
			}
			return new Size(MathUtils.ConvertInfinityToDefault(constraint.Width, 0.0), MathUtils.ConvertInfinityToDefault(constraint.Height, 0.0));
		}
		protected override Size ArrangeOverride(Size finalSize) {
			foreach (UIElement child in Children) {
				ContentPresenter presenter = child as ContentPresenter;
				if (presenter != null) {
					ICrosshairLabelItem item = presenter.Content as ICrosshairLabelItem;
					if (item != null) {
						if (item.Size != item.Bounds.Size)
							presenter.Arrange(new Rect(item.Bounds.X, item.Bounds.Y, item.Size.Width, item.Size.Height));
						else
							presenter.Arrange(item.Bounds);
					}
				}
			}
			return finalSize;
		}
	}
	public class CrosshairLabelsPanel : Panel {
		protected override Size MeasureOverride(Size constraint) {
			foreach (UIElement child in Children) {
				ContentPresenter presenter = child as ContentPresenter;
				if (presenter != null) {
					ICrosshairLabelItem item = presenter.Content as ICrosshairLabelItem;
					if (item != null) {
						presenter.Measure(constraint);
						Popup popup = LayoutHelper.FindElementByName(presenter, "PART_Popup") as Popup;
						if (popup != null) {
							popup.Child.InvalidateMeasure();
							popup.Child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
							item.Size = popup.Child.DesiredSize;
						}
					}
				}
			}
			return new Size(MathUtils.ConvertInfinityToDefault(constraint.Width, 0.0), MathUtils.ConvertInfinityToDefault(constraint.Height, 0.0));
		}
		protected override Size ArrangeOverride(Size finalSize) {
			foreach (UIElement child in Children) {
				ContentPresenter presenter = child as ContentPresenter;
				if (presenter != null) {
					Popup popup = LayoutHelper.FindElementByName(presenter, "PART_Popup") as Popup;
					if (popup != null)
						popup.Child.InvalidateArrange();
				}
			}
			return base.ArrangeOverride(finalSize);
		}
	}
	public interface ICrosshairLabelItem {
		Size Size { get; set; }
		Rect Bounds { get; }
	}
	[NonCategorized]
	public abstract class CrosshairLablePresentationData : NotifyPropertyChangedObject {
		string text;
		Brush foreground;
		FontFamily fontFamily;
		double fontSize;
		FontStretch fontStretch;
		FontStyle fontStyle;
		FontWeight fontWeight;
		string headerText;
		string footerText;
		Visibility headerTextVisibility;
		Visibility footerTextVisibility;
		public string Text {
			get { return text; }
			set {
				text = value;
				OnPropertyChanged("Text");
			}
		}
		public Brush Foreground {
			get { return foreground; }
			set {
				foreground = value;
				OnPropertyChanged("Foreground");
			}
		}
		public FontFamily FontFamily {
			get { return fontFamily; }
			set {
				fontFamily = value;
				OnPropertyChanged("FontFamily");
			}
		}
		public double FontSize {
			get { return fontSize; }
			set {
				fontSize = value;
				OnPropertyChanged("FontSize");
			}
		}
		public FontStretch FontStretch {
			get { return fontStretch; }
			set {
				fontStretch = value;
				OnPropertyChanged("FontStretch");
			}
		}
		public FontStyle FontStyle {
			get { return fontStyle; }
			set {
				fontStyle = value;
				OnPropertyChanged("FontStyle");
			}
		}
		public FontWeight FontWeight {
			get { return fontWeight; }
			set {
				fontWeight = value;
				OnPropertyChanged("FontWeight");
			}
		}
		public string HeaderText {
			get { return headerText; }
			set {
				headerText = value;
				OnPropertyChanged("HeaderText");
			}
		}
		public string FooterText {
			get { return footerText; }
			set {
				footerText = value;
				OnPropertyChanged("FooterText");
			}
		}
		public Visibility HeaderTextVisibility {
			get { return headerTextVisibility; }
			set {
				headerTextVisibility = value;
				OnPropertyChanged("HeaderTextVisibility");
			}
		}
		public Visibility FooterTextVisibility {
			get { return footerTextVisibility; }
			set {
				footerTextVisibility = value;
				OnPropertyChanged("FooterTextVisibility");
			}
		}
	}
	[NonCategorized]
	public class CrosshairAxisLabelPresentationData : CrosshairLablePresentationData {
		Axis2D axis;
		Brush background;
		DataTemplate crosshairAxisLabelTemplate;
		public Brush Background {
			get { return background; }
			set {
				background = value;
				OnPropertyChanged("Background");
			}
		}
		public Axis2D Axis {
			get { return axis; }
			set {
				axis = value;
				OnPropertyChanged("Axis");
			}
		}
		public DataTemplate CrosshairAxisLabelTemplate {
			get {
				return crosshairAxisLabelTemplate;
			}
			set {
				crosshairAxisLabelTemplate = value;
				OnPropertyChanged("CrosshairAxisLabelTemplate");
			}
		}
	}
	[NonCategorized]
	public class CrosshairLinePresentationData : NotifyPropertyChangedObject {
		double x1;
		double x2;
		double y1;
		double y2;
		LineStyle lineStyle;
		Brush brush;
		public Brush Brush {
			get {
				return brush;
			}
			set {
				brush = value;
				OnPropertyChanged("Brush");
			}
		}
		public LineStyle LineStyle {
			get {
				return lineStyle;
			}
			set {
				lineStyle = value;
				OnPropertyChanged("LineStyle");
			}
		}
		public double X1 {
			get {
				return x1;
			}
			set {
				x1 = value;
				OnPropertyChanged("X1");
			}
		}
		public double X2 {
			get { return x2; }
			set {
				x2 = value;
				OnPropertyChanged("X2");
			}
		}
		public double Y1 {
			get { return y1; }
			set {
				y1 = value;
				OnPropertyChanged("Y1");
			}
		}
		public double Y2 {
			get { return y2; }
			set {
				y2 = value;
				OnPropertyChanged("Y2");
			}
		}
	}
	[NonCategorized]
	public class CrosshairAxisLabelItem : NotifyPropertyChangedObject, ICrosshairLabelItem {
		readonly CrosshairAxisLabelPresentationData presentatinoData = new CrosshairAxisLabelPresentationData();
		CrosshairAxisInfo labelInfo;
		Size size;
		Rect bounds;
		public CrosshairAxisLabelPresentationData PresentationData { get { return presentatinoData; } }
		public CrosshairAxisInfo LabelInfo { get { return labelInfo; } set { labelInfo = value; } }
		public Size Size { get { return size; } set { size = value; } }
		public Rect Bounds { get { return bounds; } set { bounds = value; } }
	}
	[NonCategorized]
	public class CrosshairSeriesLabelPresentationData : CrosshairLablePresentationData {
		XYSeries2D series;
		CrosshairMarkerItem markerItem;
		DataTemplate crosshairSeriesLabelTemplate;
		Visibility textVisibility;
		Visibility markerVisibility;
		HorizontalAlignment elementAlignment;
		SeriesPoint seriesPoint;
		public XYSeries2D Series {
			get { return series; }
			set {
				series = value;
				OnPropertyChanged("Series");
			}
		}
		public CrosshairMarkerItem MarkerItem {
			get { return markerItem; }
			set {
				markerItem = value;
				OnPropertyChanged("MarkerItem");
			}
		}
		public DataTemplate CrosshairSeriesLabelTemplate {
			get {
				return crosshairSeriesLabelTemplate;
			}
			set {
				crosshairSeriesLabelTemplate = value;
				OnPropertyChanged("CrosshairSeriesLabelTemplate");
			}
		}
		public Visibility TextVisibility {
			get { return textVisibility; }
			set {
				textVisibility = value;
				OnPropertyChanged("TextVisibility");
			}
		}
		public Visibility MarkerVisibility {
			get { return markerVisibility; }
			set {
				markerVisibility = value;
				OnPropertyChanged("MarkerVisibility");
			}
		}
		public HorizontalAlignment ElementAlignment {
			get { return elementAlignment; }
			set {
				elementAlignment = value;
				OnPropertyChanged("ElementAlignment");
			}
		}
		public SeriesPoint SeriesPoint {
			get { return seriesPoint; }
			internal set {
				seriesPoint = value;
				OnPropertyChanged("SeriesPoint");
			}
		}
	}
	[NonCategorized]
	public class CrosshairSeriesLabelItem : AnnotationItem, ICrosshairLabelItem {
		ObservableCollection<CrosshairSeriesLabelPresentationData> presentatinoData;
		Size size;
		Rect bounds;
		public ObservableCollection<CrosshairSeriesLabelPresentationData> PresentationData {
			get { return presentatinoData; }
			set {
				presentatinoData = value;
				OnPropertyChanged("PresentationData");
			}
		}
		public Size Size { get { return size; } set { size = value; } }
		public Rect Bounds {
			get { return bounds; }
			set {
				bounds = value;
				OnPropertyChanged("Bounds");
			}
		}
	}
	[NonCategorized]
	public class AnnotationItem : NotifyPropertyChangedObject {
		const double beakHeight = 32;
		protected static Effect CreateDefaultShadow() {
			return new DropShadowEffect() { Direction = -90, BlurRadius = 16, Opacity = 0.25, ShadowDepth = 4 };
		}
		IAnnotationLayout layout;
		Effect shadow = CreateDefaultShadow();
		Visibility beakVisibility = Visibility.Visible;
		AnnotationLocation location;
		public IAnnotationLayout Layout {
			get { return layout; }
			set {
				layout = value;
				UpdateLayoutProperties();
			}
		}
		public AnnotationLocation Location {
			get { return location; }
			set {
				location = value;
				OnPropertyChanged("Location");
			}
		}
		public Visibility BeakVisibility {
			get { return beakVisibility; }
			set {
				beakVisibility = value;
				OnPropertyChanged("BeakVisibility");
			}
		}
		public Effect Shadow {
			get { return shadow; }
			set {
				shadow = value;
				OnPropertyChanged("Shadow");
			}
		}
		public double BeakHeight { get { return beakVisibility == Visibility.Visible ? beakHeight : 0; } }
		public void UpdateLayoutProperties() {
			Location = layout.Location;
			BeakVisibility = layout.ShowTail ? Visibility.Visible : Visibility.Collapsed;
		}
	}
	public enum AnnotationElementType {
		None,
		Content,
		Beak,
	}
	[NonCategorized]
	public class AnnotationPanel : Panel {
		internal const double PopupContentShadowPadding = 16;
		public static readonly DependencyProperty AnnotationItemProperty = DependencyPropertyManager.Register("AnnotationItem",
			typeof(AnnotationItem), typeof(AnnotationPanel), new PropertyMetadata(null, AnnotationItemChanged));
		public static readonly DependencyProperty ElementTypeProperty = DependencyPropertyManager.RegisterAttached("ElementType",
			typeof(AnnotationElementType), typeof(AnnotationPanel), new PropertyMetadata(AnnotationElementType.None));
		public AnnotationItem AnnotationItem {
			get { return (AnnotationItem)GetValue(AnnotationItemProperty); }
			set { SetValue(AnnotationItemProperty, value); }
		}
		public static AnnotationElementType GetElementType(UIElement element) {
			return (AnnotationElementType)element.GetValue(ElementTypeProperty);
		}
		public static void SetElementType(UIElement element, AnnotationElementType elementType) {
			element.SetValue(ElementTypeProperty, elementType);
		}
		static void AnnotationItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			AnnotationPanel panel = d as AnnotationPanel;
			if (panel != null)
				panel.InvalidateMeasure();
		}
		Size maxMeasureSize = new Size(0, 0);
		Size maxArrangeSize = new Size(0, 0);
		Panel parentPanel;
		Panel ParentPanel {
			get {
				if (parentPanel == null) {
					parentPanel = LayoutHelper.FindLayoutOrVisualParentObject<ToolTipPanel>(this, true);
					if (parentPanel == null)
						parentPanel = LayoutHelper.FindLayoutOrVisualParentObject<CrosshairLabelsPanel>(this, true);
				}
				return parentPanel;
			}
		}
		internal Point CalculatePopupOffset() {
			double xOffset = -AnnotationPanel.PopupContentShadowPadding;
			double yOffset = AnnotationPanel.PopupContentShadowPadding;
			if (AnnotationItem != null && AnnotationItem.Layout != null) {
				if (AnnotationItem.Layout.Location == AnnotationLocation.TopLeft || AnnotationItem.Layout.Location == AnnotationLocation.BottomLeft)
					xOffset *= -1;
				if (AnnotationItem.Layout.Location == AnnotationLocation.BottomLeft || AnnotationItem.Layout.Location == AnnotationLocation.BottomRight)
					yOffset *= -1;
			}
			return new Point(xOffset, yOffset);
		}
		protected override Size MeasureOverride(Size availableSize) {
			AnnotationItem item = AnnotationItem;
			double maxBeakHeight = 0, maxContentHeight = 0, desiredWidth = 0;
			if (item != null) {
				foreach (UIElement child in Children) {
					switch (GetElementType(child)) {
						case AnnotationElementType.Beak:
							child.Measure(new Size(availableSize.Width, item.BeakHeight));
							maxBeakHeight = Math.Max(maxBeakHeight, child.DesiredSize.Height);
							break;
						case AnnotationElementType.Content:
							double contentHeight = availableSize.Height - item.BeakHeight;
							if (contentHeight >= 0.0)
								child.Measure(new Size(availableSize.Width, contentHeight));
							maxContentHeight = Math.Max(maxContentHeight, child.DesiredSize.Height);
							break;
						default:
							child.Measure(availableSize);
							break;
					}
					desiredWidth = Math.Max(desiredWidth, child.DesiredSize.Width);
				}
				if (ParentPanel != null)
					ParentPanel.InvalidateArrange();
			}
			Size newSize = new Size(desiredWidth + PopupContentShadowPadding * 2, maxContentHeight + maxBeakHeight + PopupContentShadowPadding * 2);
			if (newSize.Height > maxMeasureSize.Height)
				maxMeasureSize.Height = newSize.Height;
			if (newSize.Width > maxMeasureSize.Width)
				maxMeasureSize.Width = newSize.Width;
			return maxMeasureSize;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			AnnotationItem item = AnnotationItem;
			if (item != null && item.Layout != null) {
				AnnotationLocation location = item.Layout.Location;
				foreach (UIElement child in Children) {
					switch (GetElementType(child)) {
						case AnnotationElementType.Beak:
							double beakOffsetX = 0, beakOffsetY = 0;
							if (location == AnnotationLocation.TopLeft || location == AnnotationLocation.TopRight)
								beakOffsetY = (DesiredSize.Height - PopupContentShadowPadding * 2) - item.BeakHeight;
							if (location == AnnotationLocation.TopRight || location == AnnotationLocation.BottomRight)
								beakOffsetX = -item.Layout.Offset.X;
							else
								beakOffsetX = (DesiredSize.Width - PopupContentShadowPadding * 2) + item.Layout.Offset.X - child.DesiredSize.Width;
							child.Arrange(new Rect(beakOffsetX + PopupContentShadowPadding, beakOffsetY + PopupContentShadowPadding, child.DesiredSize.Width, child.DesiredSize.Height));
							break;
						case AnnotationElementType.Content:
							double contentYOffset = 0;
							if (location == AnnotationLocation.BottomLeft || location == AnnotationLocation.BottomRight)
								contentYOffset = item.BeakHeight;
							child.Arrange(new Rect(PopupContentShadowPadding, contentYOffset + PopupContentShadowPadding, child.DesiredSize.Width, child.DesiredSize.Height));
							break;
						default:
							child.Arrange(new Rect(0, 0, DesiredSize.Width, DesiredSize.Height));
							break;
					}
				}
			}
			if (DesiredSize.Height > maxArrangeSize.Height)
				maxArrangeSize.Height = DesiredSize.Height;
			if (DesiredSize.Width > maxArrangeSize.Width)
				maxArrangeSize.Width = DesiredSize.Width;
			return maxArrangeSize;
		}
	}
	[NonCategorized]
	public class CrosshairElementDrawInfo {
		readonly CrosshairElement element;
		readonly CrosshairLine elementLine;
		readonly CrosshairAxisInfo elementAxisInfo;
		public CrosshairElement Element { get { return element; } }
		public CrosshairLine ElementLine { get { return elementLine; } }
		public CrosshairAxisInfo ElementAxisInfo { get { return elementAxisInfo; } }
		public CrosshairElementDrawInfo(CrosshairElement element, CrosshairLine elementLine, CrosshairAxisInfo elementAxisInfo) {
			this.element = element;
			this.elementLine = elementLine;
			this.elementAxisInfo = elementAxisInfo;
		}
	}
	[NonCategorized]
	public class CrosshairPaneDrawInfo {
		readonly Pane pane;
		readonly CrosshairLine cursorLine;
		readonly Dictionary<CrosshairAxisLabelElement, CrosshairAxisInfo> cursorAxesInfo;
		readonly List<CrosshairElementDrawInfo> elementsDrawInfo;
		public Pane Pane { get { return pane; } }
		public CrosshairLine CursorLine { get { return cursorLine; } }
		public Dictionary<CrosshairAxisLabelElement, CrosshairAxisInfo> CursorAxesInfo { get { return cursorAxesInfo; } }
		public List<CrosshairElementDrawInfo> ElementsDrawInfo { get { return elementsDrawInfo; } }
		public CrosshairPaneDrawInfo(Pane pane, CrosshairLine cursorLine) {
			this.pane = pane;
			this.cursorLine = cursorLine;
			this.cursorAxesInfo = new Dictionary<CrosshairAxisLabelElement, CrosshairAxisInfo>();
			this.elementsDrawInfo = new List<CrosshairElementDrawInfo>();
		}
	}
	[NonCategorized]
	public class CrosshairDrawInfo {
		readonly XYDiagram2D diagram;
		readonly CrosshairOptions crosshairOptions;
		readonly CrosshairLineElement cursorLineElement;
		readonly List<CrosshairElementGroup> crosshairElementGroups;
		readonly List<CrosshairPaneDrawInfo> paneElements;
		readonly List<CrosshairAxisLabelElement> cursorAxisLabelElements;
		readonly Dictionary<CrosshairElementGroup, CrosshairLabelInfoEx> groupsLabelsMapping;
		public CrosshairLineElement CursorLineElement { get { return cursorLineElement; } }
		public List<CrosshairElementGroup> CrosshairElementGroups { get { return crosshairElementGroups; } }
		public List<CrosshairPaneDrawInfo> PaneElements { get { return paneElements; } }
		public List<CrosshairAxisLabelElement> CrosshairAxisLabelElements { get { return cursorAxisLabelElements; } }
		public CrosshairDrawInfo(CrosshairOptions crosshairOptions, XYDiagram2D diagram) {
			this.diagram = diagram;
			this.crosshairOptions = crosshairOptions;
			this.crosshairElementGroups = new List<CrosshairElementGroup>();
			this.paneElements = new List<CrosshairPaneDrawInfo>();
			this.cursorAxisLabelElements = new List<CrosshairAxisLabelElement>();
			this.cursorLineElement = CreateCrosshairLineElement(crosshairOptions.SnapMode == CrosshairSnapMode.NearestValue);
			this.groupsLabelsMapping = new Dictionary<CrosshairElementGroup, CrosshairLabelInfoEx>();
		}
		List<SeriesPoint> PopulateSeriesPoints(CrosshairPointsGroup pointsGroup) {
			List<SeriesPoint> points = new List<SeriesPoint>();
			foreach (CrosshairSeriesTextEx text in pointsGroup.SeriesTexts) {
				points.Add(SeriesPoint.GetSeriesPoint(text.RefinedPoint.SeriesPoint));
			}
			return points;
		}
		CrosshairGroupHeaderElement CreateGroupHeaderElement(CrosshairPointsGroup group) {
			DataTemplate headerTemplate = null;
			XYSeries2D series = group.SeriesTexts[0].RefinedSeries.Series as XYSeries2D;
			if (series != null)
				headerTemplate = series.CrosshairLabelTemplate;
			return new CrosshairGroupHeaderElement(PopulateSeriesPoints(group), group.HeaderText, diagram.CrosshairSeriesLabelForeground, diagram.FontFamily, diagram.FontSize, diagram.FontStretch,
				diagram.FontStyle, diagram.FontWeight, headerTemplate);
		}
		Brush GetLineColor(bool isValueLine) {
			return isValueLine ? crosshairOptions.ActualValueLineBrush : crosshairOptions.ActualArgumentLineBrush;
		}
		LineStyle CreateCopy(LineStyle lineStyle) {
			if (lineStyle == null)
				return new LineStyle();
			return (LineStyle)lineStyle.Clone();
		}
		LineStyle GetLineStyle(bool isValueLine) {
			return isValueLine ? CreateCopy(crosshairOptions.ValueLineStyle) : CreateCopy(crosshairOptions.ArgumentLineStyle);
		}
		CrosshairLineElement CreateCrosshairLineElement(bool isValueLine) {
			return new CrosshairLineElement(isValueLine ? crosshairOptions.ShowValueLine : crosshairOptions.ShowArgumentLine, GetLineColor(isValueLine), GetLineStyle(isValueLine));
		}
		bool GetCrosshairAxisLabelVisibility(bool isValuesAxis, CrosshairAxisLabelOptions axisLabelOptions) {
			if (axisLabelOptions.Visibility.HasValue)
				return axisLabelOptions.Visibility.Value;
			return isValuesAxis ? crosshairOptions.ShowValueLabels : crosshairOptions.ShowArgumentLabels;
		}
		CrosshairAxisLabelElement CreateCrosshairAxisLabelElement(CrosshairAxisInfo crosshairAxisInfo) {
			Axis2D axis2D = (Axis2D)crosshairAxisInfo.Axis;
			CrosshairAxisLabelOptions axisLabelOptions = axis2D.ActualCrosshairAxisLabelOptions;
			return new CrosshairAxisLabelElement(crosshairAxisInfo.Text, axisLabelOptions.ActualBackground, axisLabelOptions.ActualForeground,
				axisLabelOptions.ActualFontFamily, axisLabelOptions.ActualFontSize, axisLabelOptions.ActualFontStretch, axisLabelOptions.ActualFontStyle,
				axisLabelOptions.ActualFontWeight, GetCrosshairAxisLabelVisibility(axis2D.IsValuesAxis, axisLabelOptions), crosshairAxisInfo.Value, axis2D.CrosshairLabelTemplate);
		}
		CrosshairLabelElement CreateCrosshairLabelElement(CrosshairSeriesPointEx crosshairPoint) {
			CrosshairSeriesTextEx crosshairSeriesText = crosshairPoint.CrosshairSeriesText;
			IRefinedSeries refinedSeries = crosshairSeriesText.RefinedSeries;
			XYSeries2D series = (XYSeries2D)refinedSeries.Series;
			CrosshairMarkerItem markerItem = series.CreateCrosshairMarkerItem(refinedSeries, crosshairPoint.RefinedPoint);
			return new CrosshairLabelElement(crosshairSeriesText.Text, diagram.CrosshairSeriesLabelForeground, diagram.FontFamily, diagram.FontSize, diagram.FontStretch, diagram.FontStyle, diagram.FontWeight,
				series.ActualCrosshairLabelVisible, series.CrosshairLabelTemplate, markerItem.MarkerBrush);
		}
		public void AddCrosshairPaneInfo(CrosshairPaneInfoEx crosshairInfo, Pane pane) {
			CrosshairPaneDrawInfo paneElement = new CrosshairPaneDrawInfo(pane, crosshairInfo.CursorCrossLine);
			foreach (CrosshairAxisInfo crosshairAxisInfo in crosshairInfo.CursorAxesInfo) {
				CrosshairAxisLabelElement axisLabelElement = CreateCrosshairAxisLabelElement(crosshairAxisInfo);
				paneElement.CursorAxesInfo.Add(axisLabelElement, crosshairAxisInfo);
				cursorAxisLabelElements.Add(axisLabelElement);
			}
			foreach (CrosshairLabelInfoEx labelInfo in crosshairInfo.LabelsInfo) {
				foreach (CrosshairPointsGroup pointsGroup in labelInfo.PointGroups) {
					CrosshairGroupHeaderElement headerElement = CreateGroupHeaderElement(pointsGroup);
					List<CrosshairElement> elements = new List<CrosshairElement>();
					foreach (CrosshairSeriesPointEx crosshairPoint in pointsGroup.SeriesPoints) {
						CrosshairLineElement lineElement = CreateCrosshairLineElement(crosshairPoint.CrosshairLine.IsValueLine);
						CrosshairAxisLabelElement axisLabelElement = CreateCrosshairAxisLabelElement(crosshairPoint.CrosshairAxisInfo);
						CrosshairLabelElement crosshairLabelElement = CreateCrosshairLabelElement(crosshairPoint);
						CrosshairElement crosshairElement = new CrosshairElement(SeriesPoint.GetSeriesPoint(crosshairPoint.RefinedPoint.SeriesPoint), lineElement, axisLabelElement, crosshairLabelElement, crosshairPoint.RefinedSeries, crosshairPoint.RefinedPoint);
						elements.Add(crosshairElement);
						paneElement.ElementsDrawInfo.Add(new CrosshairElementDrawInfo(crosshairElement, crosshairPoint.CrosshairLine, crosshairPoint.CrosshairAxisInfo));
					}
					CrosshairElementGroup elementGroup = new CrosshairElementGroup(headerElement, elements);
					crosshairElementGroups.Add(elementGroup);
					groupsLabelsMapping.Add(elementGroup, labelInfo);
				}
			}
			paneElements.Add(paneElement);
		}
		public CrosshairLabelInfoEx GetGroupLabelInfo(CrosshairElementGroup group) {
			CrosshairLabelInfoEx labelInfo = null;
			groupsLabelsMapping.TryGetValue(group, out labelInfo);
			return labelInfo;
		}
	}
}
