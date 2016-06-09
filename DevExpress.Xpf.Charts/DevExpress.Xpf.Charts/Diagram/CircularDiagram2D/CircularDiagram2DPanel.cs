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
using System.Windows.Controls;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Charts.Native;
using System.Windows.Media;
namespace DevExpress.Xpf.Charts {
	public class CircularDiagram2DPanel : Panel {
		class CircularDiagram2DElements {
			readonly UIElementCollection elementCollection;
			public PointsContainer PointsContainer { get { return FindElementByType(typeof(PointsContainer)) as PointsContainer; } }
			public SeriesLabel Label { get { return FindElementByType(typeof(SeriesLabel)) as SeriesLabel; } }
			public IEnumerable<UIElement> LayoutElements {
				get {
					foreach (UIElement element in elementCollection)
						if (element is LayoutElementPresentation)
							yield return element;
				}
			}
			public CircularDiagram2DElements(UIElementCollection elementCollection) {
				this.elementCollection = elementCollection;
			}
			UIElement FindElementByType(Type elementType) {
				foreach (UIElement element in elementCollection) {
					if (element.GetType() == elementType)
						return element;
				}
				return null;
			}
		}
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
		const double defaultWidth = 300.0;
		const double defaultHeight = 300.0;
		List<LayoutItem> layoutItems;
		bool CanPerformLayout {
			get {
				CircularDiagram2D diagram = GetDiagram();
				return diagram != null;
			}
		}
		IEnumerable<UIElement> LayoutElements {
			get {
				foreach (UIElement element in Children)
					if (XYDiagram2D.GetElementType(element) == XYDiagram2DElementType.None && (element is ILayoutElement))
						yield return element;
			}
		}
		UIElement DiagramContent { get { return FindElementByType(XYDiagram2DElementType.DiagramContent); } }
		public CircularDiagram2DPanel() { }
		void InvalidateDiagram() {
			CircularDiagram2D diagram = GetDiagram();
			if (diagram != null && diagram.ChartControl != null)
				diagram.ChartControl.AddInvalidate(InvalidateMeasureFlags.MeasureDiagram);
		}
		CircularDiagram2D GetDiagram() {
			CircularDiagram2DItemsControl parentItemsControl = LayoutHelper.FindParentObject<CircularDiagram2DItemsControl>(this);
			return parentItemsControl != null ? parentItemsControl.Diagram : null;
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
			double constraintWidth = double.IsInfinity(availableSize.Width) ? defaultWidth : availableSize.Width;
			double constraintHeight = double.IsInfinity(availableSize.Height) ? defaultHeight : availableSize.Height;
			Size constraint = new Size(constraintWidth, constraintHeight);
			List<LayoutItem> newLayoutItems = new List<LayoutItem>();
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
			Rect viewport = GetDiagram().ActualViewport;
			UIElement diagramContent = DiagramContent;
			if (diagramContent != null)
				diagramContent.Measure(new Size(viewport.Width, viewport.Height));
			return constraint;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			if (!CanPerformLayout)
				return base.ArrangeOverride(finalSize);
			CircularDiagram2D diagram = GetDiagram();
			Rect viewport = diagram.ActualViewport;
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
			IEnumerable<SeriesItem> seriesItemsCollection = diagram.SeriesItems;
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
			if (diagramContent != null)
				diagramContent.Arrange(viewport);
			return finalSize;
		}
	}
}
