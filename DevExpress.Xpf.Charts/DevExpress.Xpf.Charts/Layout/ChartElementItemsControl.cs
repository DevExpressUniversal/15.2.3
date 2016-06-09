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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public class ChartElementItemsControl : ChartItemsControl {
		public static readonly DependencyProperty DiagramProperty = DependencyPropertyManager.Register("Diagram", 
			typeof(Diagram), typeof(ChartElementItemsControl), new PropertyMetadata(null, PropertyChanged));
		public static readonly DependencyProperty LegendProperty = DependencyPropertyManager.Register("Legend", 
			typeof(Legend), typeof(ChartElementItemsControl), new PropertyMetadata(null, PropertyChanged));
		[
		Category(Categories.Common)
		]
		public Diagram Diagram {
			get { return (Diagram)GetValue(DiagramProperty); }
			set { SetValue(DiagramProperty, value); }
		}
		[
		Category(Categories.Common)
		]
		public Legend Legend {
			get { return (Legend)GetValue(LegendProperty); }
			set { SetValue(LegendProperty, value); }
		}
		static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ChartElementItemsControl itemsControl = d as ChartElementItemsControl;
			if (itemsControl != null)
				itemsControl.Update();
		}
		void Update() {
			ObservableCollection<Control> items = new ObservableCollection<Control>();
			if (Diagram != null)
				items.Add(Diagram);
			if (Legend != null)
				items.Add(Legend);
			ItemsSource = items;
		}
	}
	public class ChartElementPanel : Panel {
		Diagram diagram = null;
		Legend legend = null;
		bool IsLegendInsideDiagram {
			get {
				return ActualLegendVerticalPosition != VerticalPosition.TopOutside && ActualLegendVerticalPosition != VerticalPosition.BottomOutside &&
					ActualLegendHorizontalPosition != HorizontalPosition.LeftOutside && ActualLegendHorizontalPosition != HorizontalPosition.RightOutside;
			}
		}
		HorizontalPosition ActualLegendHorizontalPosition {
			get { return legend != null ? legend.HorizontalPosition : HorizontalPosition.Center; }
		}
		VerticalPosition ActualLegendVerticalPosition {
			get { return legend != null ? legend.VerticalPosition : VerticalPosition.Center; }
		}
		Thickness ActualLegendIndentFromDiagram {
			get { return legend != null ? legend.IndentFromDiagram : new Thickness(0); }
		}
		bool LegendHasItems {
			get { return legend != null && legend.Items != null && legend.Items.Count > 0; }
		}
		Rect CalcLegendBounds() {
			if (diagram == null)
				return RectExtensions.Zero;
			return new Rect (diagram.ActualViewport.Left + diagram.Margin.Left + diagram.BorderThickness.Left + diagram.Padding.Left, 
				diagram.ActualViewport.Top + diagram.Margin.Top + diagram.BorderThickness.Top + diagram.Padding.Top, diagram.ActualViewport.Width, diagram.ActualViewport.Height);			
		}
		UIElement FindControlByType(Type type) {
			foreach(UIElement control in Children)
				if (control.GetType() == type || control.GetType().IsSubclassOf(type))
					return control;
			return null;
		}
		void EnsureChildrenControls() {
			diagram = FindControlByType(typeof(Diagram)) as Diagram;
			legend = FindControlByType(typeof(Legend)) as Legend;			
		}
		Size MeasureLegend(Size availableSize) {
			legend.Measure(availableSize);
			if (legend.Items != null && legend.Items.Count > 0)
				return legend.DesiredSize;
			return new Size(0, 0);
		}
		Size MeasureLegendInsideDiagram(Size availableSize) {
			diagram.InvalidateMeasure();
			diagram.Measure(availableSize);
			Rect legendBounds = CalcLegendBounds();
			double insideWidth = Math.Max(legendBounds.Width - ActualLegendIndentFromDiagram.Left - ActualLegendIndentFromDiagram.Right, 0);
			double insideHeight = Math.Max(legendBounds.Height - ActualLegendIndentFromDiagram.Top - ActualLegendIndentFromDiagram.Bottom, 0);
			MeasureLegend(new Size(insideWidth, insideHeight));
			return diagram.DesiredSize;
		}
		Size MeasureLegendOutsideDiagram(Size availableSize) {
			double legendAvailableHeight = Math.Max(availableSize.Height-diagram.Margin.Top-diagram.Margin.Bottom, 0);
			double legendAvailableWidth = Math.Max(availableSize.Width-diagram.Margin.Left-diagram.Margin.Right, 0);
			Size legendSize = MeasureLegend(new Size(legendAvailableWidth, legendAvailableHeight));
			double diagramWidth = availableSize.Width;
			double diagramHeight = availableSize.Height;
			if (legendSize.Width > 0) {
				switch (ActualLegendHorizontalPosition) {
					case HorizontalPosition.LeftOutside:
						diagramWidth -= Math.Round(legendSize.Width + ActualLegendIndentFromDiagram.Right);
						break;
					case HorizontalPosition.RightOutside:
						diagramWidth -= Math.Round(legendSize.Width + ActualLegendIndentFromDiagram.Left);
						break;
				}
			}
			if (legendSize.Height > 0) {
				switch (ActualLegendVerticalPosition) {
					case VerticalPosition.TopOutside:
						diagramHeight -= Math.Round(legendSize.Height + ActualLegendIndentFromDiagram.Bottom);
						break;
					case VerticalPosition.BottomOutside:
						diagramHeight -= Math.Round(legendSize.Height + ActualLegendIndentFromDiagram.Top);
						break;
				}
			}
			diagramWidth = Math.Max(diagramWidth, 0);
			diagramHeight = Math.Max(diagramHeight, 0);
			diagram.Measure(new Size(diagramWidth, diagramHeight));
			return availableSize;
		}
		Size ArrangeLegendOutsideDiagram(Size finalSize) {
			Point location = new Point(0, 0);
			if (LegendHasItems) { 
				if (ActualLegendHorizontalPosition == HorizontalPosition.LeftOutside)
					location.X = Math.Round(legend.DesiredSize.Width + ActualLegendIndentFromDiagram.Right);
				if (ActualLegendVerticalPosition == VerticalPosition.TopOutside)
					location.Y = Math.Round(legend.DesiredSize.Height + ActualLegendIndentFromDiagram.Bottom);			
			}
			diagram.Arrange(new Rect(location, diagram.DesiredSize));
			Size legendSize = LegendHasItems ? legend.DesiredSize : new Size(0, 0);
			double legendX;
			switch (ActualLegendHorizontalPosition) {
				case HorizontalPosition.LeftOutside: 
					legendX = 0; 
					break;
				case HorizontalPosition.Left:
					legendX = diagram.Margin.Left;
					break;
				case HorizontalPosition.Center:
					legendX = diagram.Margin.Left + (diagram.DesiredSize.Width - diagram.Margin.Left - diagram.Margin.Right - legendSize.Width) / 2;
					break;
				case HorizontalPosition.Right:
					legendX = diagram.DesiredSize.Width - diagram.Margin.Right - legendSize.Width;
					break;
				case HorizontalPosition.RightOutside:
					legendX = finalSize.Width - legendSize.Width;
					break;
				default:
					ChartDebug.Fail("Unkown HorizontalPosition value.");
					goto case HorizontalPosition.RightOutside;
			}
			double legendY;
			switch (ActualLegendVerticalPosition) {
				case VerticalPosition.TopOutside:
					legendY = 0;
					break;
				case VerticalPosition.Top:
					legendY = diagram.Margin.Top;
					break;
				case VerticalPosition.Center:
					legendY = diagram.Margin.Top + (diagram.DesiredSize.Height - diagram.Margin.Top - diagram.Margin.Bottom - legendSize.Height) / 2;
					break;
				case VerticalPosition.Bottom:
					legendY = diagram.DesiredSize.Height - diagram.Margin.Bottom - legendSize.Height;
					break;
				case VerticalPosition.BottomOutside:
					legendY = finalSize.Height - legendSize.Height;
					break;
				default:
					ChartDebug.Fail("Unkown VerticalPosition value.");
					goto case VerticalPosition.Top;
			}
			legend.Arrange(new Rect(new Point(legendX, legendY), legendSize));
			return finalSize;
		}
		Size ArrangeLegendInsideDiagram(Size finalSize) {
			diagram.Arrange(new Rect(new Point(0, 0), diagram.DesiredSize));
			Rect legendBounds = CalcLegendBounds();
			Size legendSize = LegendHasItems ? legend.DesiredSize : new Size(0, 0);
			double legendX;
			switch (ActualLegendHorizontalPosition) {
				case HorizontalPosition.Left:
					legendX = legendBounds.Left + ActualLegendIndentFromDiagram.Left;
					break;
				case HorizontalPosition.Center:
					legendX = legendBounds.Left + Math.Max((legendBounds.Width - legendSize.Width) / 2, ActualLegendIndentFromDiagram.Left);
					break;
				case HorizontalPosition.Right:
					legendX = legendBounds.Right - legendSize.Width - ActualLegendIndentFromDiagram.Right;
					break;
				default:
					ChartDebug.Fail("Unkown HorizontalPosition value.");
					goto case HorizontalPosition.Right;
			}
			double legendY;
			switch (ActualLegendVerticalPosition) {
				case VerticalPosition.Top:
					legendY = legendBounds.Top + ActualLegendIndentFromDiagram.Top;
					break;
				case VerticalPosition.Center:
					legendY = legendBounds.Top + Math.Max((legendBounds.Height - legendSize.Height) / 2, ActualLegendIndentFromDiagram.Top);
					break;
				case VerticalPosition.Bottom:
					legendY = legendBounds.Bottom - legendSize.Height - ActualLegendIndentFromDiagram.Bottom;
					break;
				default:
					ChartDebug.Fail("Unkown VerticalPosition value.");
					goto case VerticalPosition.Top;
			}
			legend.Arrange(new Rect(new Point(legendX, legendY), legendSize));
			return finalSize;
		}
		Size ArrangeLegendWithoutDiagram(Size finalSize) {
			Size legendSize = LegendHasItems ? legend.DesiredSize : new Size(0, 0);
			double legendX;
			switch (ActualLegendHorizontalPosition) {
				case HorizontalPosition.LeftOutside:
				case HorizontalPosition.Left:
					legendX = 0;
					break;
				case HorizontalPosition.Center:
					legendX = (finalSize.Width - legendSize.Width) / 2;
					break;
				case HorizontalPosition.Right:
				case HorizontalPosition.RightOutside:
					legendX = finalSize.Width - legendSize.Width;
					break;
				default:
					ChartDebug.Fail("Unkown HorizontalPosition value.");
					goto case HorizontalPosition.RightOutside;
			}
			double legendY;
			switch (ActualLegendVerticalPosition) {
				case VerticalPosition.TopOutside:
				case VerticalPosition.Top:
					legendY = 0;
					break;
				case VerticalPosition.Center:
					legendY = (finalSize.Height - legendSize.Height) / 2;
					break;
				case VerticalPosition.Bottom:
				case VerticalPosition.BottomOutside:
					legendY = finalSize.Height - legendSize.Height;
					break;
				default:
					ChartDebug.Fail("Unkown VerticalPosition value.");
					goto case VerticalPosition.Top;
			}
			legend.Arrange(new Rect(new Point(legendX, legendY), legendSize));
			return legendSize;
		}
		protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved) {
			base.OnVisualChildrenChanged(visualAdded, visualRemoved);
			diagram = null;
			legend = null;
		}
		protected override Size MeasureOverride(Size availableSize) {
			EnsureChildrenControls();
			if (legend == null) {
				if(diagram != null)
					diagram.Measure(availableSize);
				return availableSize;
			}
			if (diagram == null) {
				MeasureLegend(availableSize);
				return availableSize;
			}
			if (IsLegendInsideDiagram)
				return MeasureLegendInsideDiagram(availableSize);
			else
				return MeasureLegendOutsideDiagram(availableSize);				
		}
		protected override Size ArrangeOverride(Size finalSize) {
			EnsureChildrenControls();
			if (diagram == null) {
				if (legend != null)
					ArrangeLegendWithoutDiagram(finalSize);
				return finalSize;
			}
			if (legend == null) {				
				diagram.Arrange(new Rect(new Point(0, 0), diagram.DesiredSize));
				return finalSize;
			}
			if (IsLegendInsideDiagram)
				return ArrangeLegendInsideDiagram(finalSize);
			else
				return ArrangeLegendOutsideDiagram(finalSize);
		}
	}
}
