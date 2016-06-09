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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
namespace DevExpress.DashboardExport {
	public class XRGridFooterPanel : XRPanel {
		static void PrepareBorders(XRControl control) {
			control.Borders = BorderSide.All;
			control.BorderWidth = 1;
		}
		readonly Font font;
		readonly GridViewerDataController dataController;
		public XRGridFooterPanel(Font font, GridViewerDataController dataController) {
			this.font = font;
			this.dataController = dataController;
			PrepareBorders(this);
			BackColor = Color.LightGray;
			BorderColor = Color.Gray;
		}
		XRControl CreateTotalItem(GridColumnTotalViewModel total) {
			XRTrimmingTextLabel label = new XRTrimmingTextLabel() {
				Text = string.Format(total.Caption, dataController.TryGetValueByColumnId(total.DataId)),
				TextAlignment = XtraPrinting.TextAlignment.MiddleRight,
				Padding = new PaddingInfo(0, 2, 0, 0),
				Font = font
			};
			return label;
		}
		public void Initialize(IList<GridColumnViewModel> columns, IList<int> columnWidths, int totalItemHeight, int topVisibleTotalItemIndex, int columnOffset) {
			const float cellHorizontalOffset = -0.5f;
			const float firstCellHorizontalOutreach = 0.5f;
			const int cellBoundsOutreach = 1;
			float x = cellHorizontalOffset;
			for(int columnIndex = columnOffset; columnIndex < columns.Count; columnIndex++) {
				int columnWidth = columnWidths[columnIndex];
				float width = columnWidth;
				width += columnIndex == columnOffset ? firstCellHorizontalOutreach : cellBoundsOutreach;
				float y = 0;
				GridColumnViewModel column = columns[columnIndex];
				for(int index = topVisibleTotalItemIndex; index < column.Totals.Count; index++) {
					GridColumnTotalViewModel total = column.Totals[index];
					XRControl item = CreateTotalItem(total);
					PrepareBorders(item);
					Controls.Add(item);
					item.BoundsF = new RectangleF(x, y, width, totalItemHeight + cellBoundsOutreach);
					y += totalItemHeight;
				}
				x += columnWidth;
			}
		}
	}
}
