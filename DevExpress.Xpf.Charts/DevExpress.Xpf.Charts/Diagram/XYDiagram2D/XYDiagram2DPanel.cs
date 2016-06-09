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
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Xpf.Charts {
	public class XYDiagram2DPanel : Panel {
		struct LayoutItem {
			public readonly UIElement Element;
			public readonly Size Size;
			public LayoutItem(UIElement element, Size size) {
				Element = element;
				Size = size;
			}
			public override bool Equals(object obj) {
				LayoutItem item = (LayoutItem)obj;
				return Object.ReferenceEquals(Element, item.Element) && Size == item.Size;
			}
			public override int GetHashCode() {
				return Element.GetHashCode();
			}
		}
		const double defaultSize = 300.0;
		bool shouldUpdateLayout;
		List<LayoutItem> layoutItems;
		Pane Pane {
			get {
				PaneItemsControl parentItemsControl = LayoutHelper.FindParentObject<PaneItemsControl>(this);
				return parentItemsControl == null ? null : parentItemsControl.Pane;
			}
		}
		bool CanPerformLayout {
			get {
				Pane pane = Pane;
				if (pane == null)
					return false;
				XYDiagram2D diagram = pane.Diagram;
				return diagram != null;
			}
		}
		bool IsAxisLabelItemsMeasured {
			get {
				foreach (Axis2DItem axisItem in Pane.AxisItems) {
					IEnumerable<AxisLabelItem> items = axisItem.LabelItems;
					if (items != null)
						foreach (AxisLabelItem labelItem in items)
							if (labelItem.Size.IsEmpty)
								return false;
				}
				return true;
			}
		}
		IEnumerable<UIElement> LayoutElements {
			get {
				foreach (UIElement element in Children)
					if (XYDiagram2D.GetElementType(element) == XYDiagram2DElementType.None && (element is ILayoutElement))
						yield return element;
			}
		}
		IEnumerable<LayoutElementPresentation> ScrollBars {
			get {
				foreach (UIElement element in Children) {
					LayoutElementPresentation presentation = element as LayoutElementPresentation;
					if (presentation != null && (presentation.Content is ScrollBarPresentation))
						yield return presentation;
				}
			}
		}
		UIElement DiagramContent { get { return FindElementByType(XYDiagram2DElementType.DiagramContent); } }
		UIElement Mirror { get { return FindElementByType(XYDiagram2DElementType.Mirror); } }
		UIElement Pseudo3DBarSeriesContainer { get { return FindElementByType(XYDiagram2DElementType.Pseudo3DBarSeriesContainer); } }
		UIElement Pseudo3DMirror { get { return FindElementByType(XYDiagram2DElementType.Pseudo3DMirror); } }
		UIElement CrosshairContainer { get { return FindElementByType(XYDiagram2DElementType.CrosshairContainer); } }
		UIElement Selection { get { return FindElementByType(XYDiagram2DElementType.Selection); } }
		public XYDiagram2DPanel() {
			LayoutUpdated += delegate {
				try {
					if (shouldUpdateLayout) {
						Pane pane = Pane;
						if (pane != null) {
							XYDiagram2D diagram = pane.Diagram;
							if (diagram != null) {
								diagram.InvalidateMeasure();
								diagram.InvalidateArrange();
							}
						}
					}
				}
				finally {
					shouldUpdateLayout = false;
				}
			};
		}
		void InvalidateDiagram() {
			shouldUpdateLayout = true;			
		}
		UIElement FindElementByType(XYDiagram2DElementType elementType) {
			foreach (UIElement element in Children)
				if (XYDiagram2D.GetElementType(element) == elementType)
					return element;
			return null;
		}
		protected override Size MeasureOverride(Size availableSize) {
			if (!CanPerformLayout)
				return base.MeasureOverride(availableSize);
			List<LayoutItem> newLayoutItems = new List<LayoutItem>();
			Size constraint = new Size(MathUtils.ConvertInfinityToDefault(availableSize.Width, defaultSize), MathUtils.ConvertInfinityToDefault(availableSize.Height, defaultSize));
			foreach (UIElement element in LayoutElements) {
				element.Measure(constraint);
				newLayoutItems.Add(new LayoutItem(element, element.DesiredSize));
			}
			if (layoutItems == null || newLayoutItems.Count != layoutItems.Count)
				InvalidateDiagram();
			else
				for (int i = 0; i < newLayoutItems.Count; i++)
					if (!layoutItems[i].Equals(newLayoutItems[i])) {
						InvalidateDiagram();
						break;
					}
			layoutItems = newLayoutItems;
			Rect viewport = Pane.Viewport;
			foreach (UIElement scrollBar in ScrollBars)
				scrollBar.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
			UIElement diagramContent = DiagramContent;
			if (diagramContent != null)
				diagramContent.Measure(new Size(viewport.Width, viewport.Height));
			UIElement mirror = Mirror;
			if (mirror != null) {
				mirror.InvalidateArrange();
				mirror.Measure(new Size(viewport.Width, constraint.Height));
			}
			UIElement pseudo3DBarSeriesContainer = Pseudo3DBarSeriesContainer;
			if (pseudo3DBarSeriesContainer != null)
				pseudo3DBarSeriesContainer.Measure(new Size(viewport.Width, viewport.Height));
			UIElement pseudo3DMirror = Pseudo3DMirror;
			if (pseudo3DMirror != null) {
				pseudo3DMirror.InvalidateArrange();
				pseudo3DMirror.Measure(constraint);
			}
			UIElement crosshairContainer = CrosshairContainer;
			if (crosshairContainer != null)
				crosshairContainer.Measure(constraint);
			return constraint;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			if (!CanPerformLayout)
				return base.ArrangeOverride(finalSize);
			Pane pane = Pane;
			Rect viewport = pane.Viewport;
			bool isNavigationEnabled = pane.Diagram.IsNavigationEnabled;
			bool isSmallViewport = viewport.Width == 0 || viewport.Height == 0;
			foreach (UIElement element in LayoutElements) {
				Rect layoutElementRect = RectExtensions.Zero;
				ILayout layout = ((ILayoutElement)element).Layout;
				if (layout != null) {
					Rect clipBounds = layout.ClipBounds;
					element.Clip = clipBounds.IsEmpty ? null : new RectangleGeometry() { Rect = clipBounds };
					if (!isSmallViewport && layout.Visible)
						layoutElementRect = layout.Bounds.IsEmpty ? new Rect(0, 0, finalSize.Width, finalSize.Height) : new Rect(layout.Location, layout.Size);
				}
				element.Arrange(layoutElementRect);
			}
			IEnumerable<SeriesItem> seriesItemsCollection = pane.SeriesItems;
			if (seriesItemsCollection != null)
				foreach (SeriesItem seriesItem in seriesItemsCollection) {
					SeriesLabel label = seriesItem.Series.ActualLabel;
					if (label != null) {
						IEnumerable<SeriesLabelItem> items = label.Items;
						if (items != null)
							foreach (SeriesLabelItem labelItem in items)
								labelItem.UpdateConnectorCoordinates();
					}
				}
			UIElement diagramContent = DiagramContent;
			if (diagramContent != null) {
				diagramContent.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, viewport.Width, viewport.Height) };
				diagramContent.Arrange(viewport);
			}
			UIElement selection = Selection;
			if (selection != null)
				selection.Arrange(viewport);
			UIElement mirror = Mirror;
			if (mirror != null)
				mirror.Arrange(isSmallViewport ? RectExtensions.Zero : new Rect(new Point(viewport.Left, viewport.Bottom), mirror.DesiredSize));
			UIElement pseudo3DBarSeriesContainer = Pseudo3DBarSeriesContainer;
			if (pseudo3DBarSeriesContainer != null)
				pseudo3DBarSeriesContainer.Arrange(isSmallViewport ? RectExtensions.Zero : new Rect(new Point(0, 0), finalSize));
			UIElement pseudo3DMirror = Pseudo3DMirror;
			if (pseudo3DMirror != null)
				pseudo3DMirror.Arrange(isSmallViewport ? RectExtensions.Zero : new Rect(new Point(0, viewport.Bottom), pseudo3DMirror.DesiredSize));
			UIElement crosshairContainer = CrosshairContainer;
			if (crosshairContainer != null)
				crosshairContainer.Arrange(isSmallViewport ? RectExtensions.Zero : new Rect(new Point(0, 0), finalSize));
			return finalSize;
		}
	}
}
