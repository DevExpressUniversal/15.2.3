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

using System.Drawing;
using DevExpress.Utils.Drawing;
namespace DevExpress.DashboardWin.Native {
	public class ChartHeaderDragSection : HeaderDragSection, IDragAreaElementWithButton {
		readonly ChartAddPaneButton addChartPaneButton = new ChartAddPaneButton();
		public ChartHeaderDragSection(DragArea area, string header)
			: base(area, header) {
		}
		void IDragAreaElementWithButton.SetButtonState(DragAreaButtonState state) {
			addChartPaneButton.SetOptionsButtonState(state);
		}
		void IDragAreaElementWithButton.ExecuteButtonClick(DragAreaControl dragArea) {
			addChartPaneButton.Execute(dragArea);
		}
		protected override DragAreaSelection GetSelectionInternal(Point point) {
			if (addChartPaneButton != null && addChartPaneButton.Bounds.Contains(point)) {
				DragAreaSelection selection = new DragAreaSelection(DragAreaSelectionType.ImageButton);
				selection.ElementWithButton = this;
				selection.ImageButton = addChartPaneButton;
				return selection;
			}
			return base.GetSelectionInternal(point);
		}
		protected override void ArrangeContent(DragAreaDrawingContext drawingContext, GraphicsCache cache, Point location) {
			base.ArrangeContent(drawingContext, cache, location);
			addChartPaneButton.CalculateBounds(drawingContext, location);
		}
		protected override void PaintContent(DragAreaDrawingContext drawingContext, GraphicsCache cache) {
			base.PaintContent(drawingContext, cache);
			addChartPaneButton.Paint(drawingContext.Appearances.ChartPaneRemoveButtonColor, cache);
		}
	}
}
