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
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core.Native;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
namespace DevExpress.Xpf.Charts.Native {
	public abstract class PanesPanelLayoutManager {
		public static PanesPanelLayoutManager Create(XYDiagram2D diagram) {
			Panel rootPanel = diagram.PanesRootPanel;
			if (rootPanel == null)
				return null;
			if (rootPanel is XYDiagram2DPanesPanel) {
				XYDiagram2DPanesPanel panesPanel = (XYDiagram2DPanesPanel)rootPanel;
				return new StackPanesPanelLayoutManager(diagram, panesPanel.Orientation);
			}
			if (rootPanel is StackPanel) {
				StackPanel stackPanel = (StackPanel)rootPanel;
				return new StackPanesPanelLayoutManager(diagram, stackPanel.Orientation);
			}
			if (rootPanel is Grid)
				return new GridPanesPanelLayoutManager(diagram);
			if (rootPanel is UniformGrid)
				return new UniformGridPanesPanelLayoutManager(diagram, (UniformGrid)rootPanel);
			return null;
		}
		class PaneLineCollection : List<Pane> {
			readonly bool isVertical;
			public bool IsVertical { get { return isVertical; } }
			public PaneLineCollection(List<Pane> panesLine, bool isVerticalLine) : base(panesLine) {
				this.isVertical = isVerticalLine;
			}
		}
		readonly List<Pane> visiblePanes;
		readonly List<PaneLineCollection> paneLines;
		protected List<Pane> VisiblePanes { get { return visiblePanes; } }
		protected PanesPanelLayoutManager(XYDiagram2D diagram) {
			this.visiblePanes = new List<Pane>();
			foreach (Pane pane in diagram.ActualPanes) {
				if (pane.Visibility == Visibility.Visible)
					this.visiblePanes.Add(pane);
			}
			this.paneLines = new List<PaneLineCollection>();
		}
		protected void AddPanesLine(List<Pane> panesLine, bool isVerticalLine) {
			paneLines.Add(new PaneLineCollection(panesLine, isVerticalLine));
		}
		protected abstract void CollectPaneLines();
		public void CorrectLayout() {
			CollectPaneLines();
			foreach (PaneLineCollection line in paneLines) {
				if (line.IsVertical)
					PanesLayoutCorrector.CorrectByHorizontal(line);
				else
					PanesLayoutCorrector.CorrectByVertical(line);
			}
		}
		public List<Pane> GetPaneLine(Pane pane, bool isVerticalLines) {
			foreach (PaneLineCollection line in paneLines) {
				if (line.IsVertical == isVerticalLines && line.Contains(pane))
					return line;
			}
			return new List<Pane>() { pane };
		}
	}
	public class StackPanesPanelLayoutManager : PanesPanelLayoutManager {
		readonly Orientation panelOrientation;
		internal StackPanesPanelLayoutManager(XYDiagram2D diagram, Orientation stackPanelOrientation) : base(diagram) {
			this.panelOrientation = stackPanelOrientation;
		}
		protected override void CollectPaneLines() {
			AddPanesLine(VisiblePanes, panelOrientation == Orientation.Vertical);
		}
	}
	public abstract class GridPanesPanelLayoutManagerBase : PanesPanelLayoutManager {
		internal GridPanesPanelLayoutManagerBase(XYDiagram2D diagram) : base(diagram) {
		}
		protected abstract int GetPaneIndex(Pane pane, bool isIndexVertical);
		protected abstract int GetPaneSpan(Pane pane, bool isSpanVertical);
		void CollectPaneLines(bool isVertical) {
			List<int> indexes = new List<int>();
			List<int> spans = new List<int>();
			foreach (Pane pane in VisiblePanes) {
				int index = GetPaneIndex(pane, isVertical);
				if (!indexes.Contains(index))
					indexes.Add(index);
				int span = GetPaneSpan(pane, isVertical);
				if (!spans.Contains(span))
					spans.Add(span);
			}
			foreach (int index in indexes) {
				foreach (int span in spans) {
					List<Pane> panesForCorrection = new List<Pane>();
					foreach (Pane pane in VisiblePanes) {
						if (GetPaneIndex(pane, isVertical) == index && GetPaneSpan(pane, isVertical) == span)
							panesForCorrection.Add(pane);
					}
					if (panesForCorrection.Count > 0)
						AddPanesLine(panesForCorrection, !isVertical);
				}
			}
		}
		protected override void CollectPaneLines() {
			CollectPaneLines(true);
			CollectPaneLines(false);
		}
	}
	public class GridPanesPanelLayoutManager : GridPanesPanelLayoutManagerBase {
		internal GridPanesPanelLayoutManager(XYDiagram2D diagram) : base(diagram) {
		}
		protected override int GetPaneIndex(Pane pane, bool isIndexVertical) {
			return (int)pane.GetValue(isIndexVertical ? Grid.RowProperty : Grid.ColumnProperty);
		}
		protected override int GetPaneSpan(Pane pane, bool isSpanVertical) {
			return (int)pane.GetValue(isSpanVertical ? Grid.RowSpanProperty : Grid.ColumnSpanProperty);
		}
	}
	public class UniformGridPanesPanelLayoutManager : GridPanesPanelLayoutManagerBase {
		readonly int columns;
		readonly int firstColumn;
		static int CalculateUniformGridColumns(XYDiagram2D diagram, UniformGrid uniformGrid) {
			int columns = uniformGrid.Columns;
			int firstColumn = uniformGrid.FirstColumn;
			int rows = uniformGrid.Rows;
			if (rows == 0 || columns == 0) {
				int visiblePanesCount = 0;
				foreach (Pane pane in diagram.Panes) {
					if (pane.Visibility != Visibility.Collapsed)
						visiblePanesCount++;
				}
				if (visiblePanesCount != 0) {
					if (rows == 0) {
						if (columns == 0) {
							columns = (int)Math.Sqrt((double)visiblePanesCount);
							if (columns * columns < visiblePanesCount)
								columns++;
						}
					}
					else {
						if (columns == 0)
							columns = (visiblePanesCount + rows - 1) / rows;
					}
				}
			}
			return columns;
		}
		internal UniformGridPanesPanelLayoutManager(XYDiagram2D diagram, UniformGrid uniformGrid) : base(diagram) {
			this.firstColumn = uniformGrid.FirstColumn;
			this.columns = CalculateUniformGridColumns(diagram, uniformGrid);
		}
		protected override int GetPaneIndex(Pane pane, bool isIndexVertical) {
			int paneIndex = VisiblePanes.IndexOf(pane) + firstColumn;
			return isIndexVertical ? paneIndex / columns : paneIndex % columns;
		}
		protected override int GetPaneSpan(Pane pane, bool isSpanVertical) {
			return 1;
		}
	}	
	public class PanesLayoutCorrector {
		static List<Axis2DItem> GetVisibleAxisItemsOnPosition(Pane pane, AxisPosition axisPosition) {
			List<Axis2DItem> visibleAxisItems = new List<Axis2DItem>();
			foreach (Axis2DItem axisItem in pane.AxisItems) {
				Axis2D axis = axisItem.Axis as Axis2D;
				if (axis.Position == axisPosition && axisItem.Layout != null && axisItem.Layout.Visible)
					visibleAxisItems.Add(axisItem);
			}
			return visibleAxisItems;
		}
		static double CalculateTitleIndent(AxisPosition position, Rect viewport, Rect titleRect) {
			switch (position) {
				case AxisPosition.Left:
					return viewport.Left - titleRect.Right;
				case AxisPosition.Top:
					return viewport.Top - titleRect.Bottom;
				case AxisPosition.Right:
					return titleRect.Left - viewport.Right;
				case AxisPosition.Bottom:
					return titleRect.Top - viewport.Bottom;
				default:
					return 0.0;
			}
		}
		public static void CorrectByHorizontal(IList<Pane> panesForCorrection) {
			PanesLayoutCorrector Manager = new PanesLayoutCorrector(panesForCorrection);
			Manager.CorrectTitleIndentsByHorizontal();
			Manager.CorrectViewportsByHorizontal();
		}
		public static void CorrectByVertical(IList<Pane> panesForCorrection) {
			PanesLayoutCorrector Manager = new PanesLayoutCorrector(panesForCorrection);
			Manager.CorrectTitleIndentsByVertical();
			Manager.CorrectViewportsByVertical();
		}
		readonly IList<Pane> panesForCorrection;
		protected PanesLayoutCorrector(IList<Pane> panesForCorrection) {
			this.panesForCorrection = panesForCorrection;
		}
		void CorrectTitleIndent(AxisPosition axisPosition) {
			Dictionary<Pane, List<Axis2DItem>> visibleAxisItemsForPanes = new Dictionary<Pane, List<Axis2DItem>>();
			foreach (Pane pane in panesForCorrection)
				visibleAxisItemsForPanes.Add(pane, GetVisibleAxisItemsOnPosition(pane, axisPosition));
			int maxAxisItemsCount = 0;
			foreach (List<Axis2DItem> axisItems in visibleAxisItemsForPanes.Values)
				maxAxisItemsCount = Math.Max(axisItems.Count, maxAxisItemsCount);
			for (int i = 0; i < maxAxisItemsCount; i++) {
				Dictionary<Pane, Axis2DItem> axisItemsOnLevelForAlign = new Dictionary<Pane, Axis2DItem>();
				foreach (KeyValuePair<Pane, List<Axis2DItem>> pair in visibleAxisItemsForPanes) {
					Pane pane = pair.Key;
					List<Axis2DItem> axisItems = pair.Value;
					if (i < axisItems.Count) {
						Axis2DItem axisItem = axisItems[i];
						AxisTitleItem axisTitleItem = axisItem.TitleItem;
						if (axisTitleItem != null && axisTitleItem.Layout != null && axisTitleItem.Layout.Visible)
							axisItemsOnLevelForAlign.Add(pane, axisItem);
					}
				}
				double extremeTitleIndent = double.NaN;
				foreach (KeyValuePair<Pane, Axis2DItem> pair in axisItemsOnLevelForAlign) {
					Pane pane = pair.Key;
					Axis2DItem axisItem = pair.Value;
					double axisTitleIndent = CalculateTitleIndent(axisPosition, pane.Viewport, axisItem.TitleItem.Layout.Bounds);
					extremeTitleIndent = double.IsNaN(extremeTitleIndent) ? axisTitleIndent : Math.Max(extremeTitleIndent, axisTitleIndent);
				}
				foreach (KeyValuePair<Pane, Axis2DItem> pair in axisItemsOnLevelForAlign) {
					Pane pane = pair.Key;
					Axis2DItem axisItem = pair.Value;
					double axisTitleIndent = CalculateTitleIndent(axisPosition, pane.Viewport, axisItem.TitleItem.Layout.Bounds);
					axisItem.TitleIndent = extremeTitleIndent - axisTitleIndent;
				}
			}
		}
		void CorrectViewportsByHorizontal() {
			double viewportLeft = double.NaN;
			double viewportRight = double.NaN;
			foreach (Pane pane in panesForCorrection) {
				viewportLeft = double.IsNaN(viewportLeft) ? pane.Viewport.Left : Math.Max(viewportLeft, pane.Viewport.Left);
				viewportRight = double.IsNaN(viewportRight) ? pane.Viewport.Right : Math.Min(viewportRight, pane.Viewport.Right);
			}
			if (!double.IsNaN(viewportLeft) && !double.IsNaN(viewportRight)) {
				foreach (Pane pane in panesForCorrection)
					pane.Viewport = new Rect(new Point(viewportLeft, pane.Viewport.Top), new Point(viewportRight, pane.Viewport.Bottom));
			}
		}
		void CorrectViewportsByVertical() {
			double viewportTop = double.NaN;
			double viewportBottom = double.NaN;
			foreach (Pane pane in panesForCorrection) {
				viewportTop = double.IsNaN(viewportTop) ? pane.Viewport.Top : Math.Max(viewportTop, pane.Viewport.Top);
				viewportBottom = double.IsNaN(viewportBottom) ? pane.Viewport.Bottom : Math.Min(viewportBottom, pane.Viewport.Bottom);
			}
			if (!double.IsNaN(viewportTop) && !double.IsNaN(viewportBottom)) {
				foreach (Pane pane in panesForCorrection)
					pane.Viewport = new Rect(new Point(pane.Viewport.Left, viewportTop), new Point(pane.Viewport.Right, viewportBottom));
			}
		}
		void CorrectTitleIndentsByHorizontal() {
			CorrectTitleIndent(AxisPosition.Left);
			CorrectTitleIndent(AxisPosition.Right);
		}
		void CorrectTitleIndentsByVertical() {
			CorrectTitleIndent(AxisPosition.Top);
			CorrectTitleIndent(AxisPosition.Bottom);
		}
	}
}
