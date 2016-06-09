#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using System.Collections.Generic;
using System.Drawing;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
namespace DevExpress.DashboardWin.Native {
	public class ChartSeriesDragSection : ChartSeriesDragSectionBase<ChartSeries>, IDragAreaElementWithButton {
		readonly ChartRemovePaneButton paneRemoveButton;
		ChartDashboardItem DashboardItem { get { return (ChartDashboardItem)Area.DashboardItem; } }
		public ChartSeriesDragSection(DragArea area, string caption, string itemName, ChartPaneDescription paneDescription)
			: base(area, caption, itemName, paneDescription) {
			ChartPane pane = paneDescription.Pane;
			if(pane == null) {
				pane = DashboardItem.CreateDefaultChartPane();
				SetCaption(ChartDashboardItem.CreateChartPaneDescriptionCaption(pane));
				SetHolders(pane.Series);
			}
			if(DashboardItem.Panes.Count > 1)
				paneRemoveButton = new ChartRemovePaneButton(pane);
		}
		void IDragAreaElementWithButton.SetButtonState(DragAreaButtonState state) {
			paneRemoveButton.SetOptionsButtonState(state);
		}
		void IDragAreaElementWithButton.ExecuteButtonClick(DragAreaControl dragArea) {
			paneRemoveButton.Execute(dragArea);
		}
		protected override DragAreaSelection GetSelectionInternal(Point point) {
			if (paneRemoveButton != null && paneRemoveButton.Bounds.Contains(point)) {
				DragAreaSelection selection = new DragAreaSelection(DragAreaSelectionType.ImageButton);
				selection.ElementWithButton = this;
				selection.ImageButton = paneRemoveButton;
				return selection;
			}
			return base.GetSelectionInternal(point);
		}
		protected override void ArrangeContent(DragAreaDrawingContext drawingContext, GraphicsCache cache, Point location) {
			base.ArrangeContent(drawingContext, cache, location);
			if(paneRemoveButton != null)
				paneRemoveButton.CalculateBounds(drawingContext, location);
		}
		protected override void PaintContent(DragAreaDrawingContext drawingContext, GraphicsCache cache) {
			base.PaintContent(drawingContext, cache);
			if(paneRemoveButton != null)
				paneRemoveButton.Paint(drawingContext.Appearances.ChartPaneRemoveButtonColor, cache);
		}
		protected override XtraForm CreateOptionsForm(ChartSelectorContext context, IEnumerable<SeriesViewGroup> seriesViewGroups) {
			return new ChartSeriesOptionsForm(context, seriesViewGroups);
		}
	}
}
