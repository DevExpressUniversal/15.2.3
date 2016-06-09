#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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
using System.Drawing;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Export;
using DevExpress.XtraPrinting.Export.Text;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Native.DrillDown;
namespace DevExpress.XtraReports.Web.WebDocumentViewer.Native {
	public class MapBuilder : MapBuilderBase<IWebDocumentViewerMapNode> {
		public int[] ColumnWidthArray { get; private set; }
		readonly LayoutControlCollection layoutControls = new LayoutControlCollection();
		protected override IWebDocumentViewerMapNode CreateNode(RectangleF rect, BrickBase brick, string indexes, RectangleF absoluteBrick) {
			VisualBrick visualBrick = brick as VisualBrick;
			string text = visualBrick != null ? visualBrick.Text : null;
			var mapNode = new MapNode(rect, brick, indexes, text);
			BrickStyle brickStyle = null;
			if(visualBrick != null) {
				mapNode.DrillDownKey = visualBrick.Value is DrillDownKey ? visualBrick.Value.ToString() : null;
				mapNode.NavigationUrl = string.Equals(visualBrick.Url, SR.BrickEmptyUrl, StringComparison.OrdinalIgnoreCase) ? "" : visualBrick.Url;
				mapNode.NavigationPageIndex = visualBrick.NavigationPageIndex;
				mapNode.NavigationIndexes = visualBrick.NavigationBrickIndices;
				mapNode.Target = visualBrick.Target;
				brickStyle = visualBrick.Style;
			}
			if(brick.InnerBrickList == null || brick.InnerBrickList.Count == 0) {
				RectangleF rectOffset = RectF.Offset(rect, absoluteBrick.X, absoluteBrick.Y);
				var textViewData = new EnumTextBrcikViewData(brickStyle, rectOffset, brick as ITableCell, mapNode) { Texts = new SimpleTextLayoutTable(text) };
				layoutControls.Add(textViewData);
			} else {
				mapNode.GeneralBrickIndex = -1;
			}
			return mapNode;
		}
		protected override void EnumerateBricks() {
			base.EnumerateBricks();
			if(this.layoutControls == null)
				return;
			TextLayoutMapX mapX = new TextLayoutMapX(layoutControls);
			TextLayoutMapY mapY = new TextLayoutMapY(layoutControls);
			ColumnWidthArray = mapX.ColumnWidthArray;
			int commonIndex = 0;
			for(int y = 0; y < mapY.Count; y++) {
				for(int x = 0; x < mapY[y].Count; x++) {
					var brickViewData = mapY[y][x].Owner as EnumTextBrcikViewData;
					if(brickViewData != null && brickViewData.MapNode.GeneralBrickIndex == 0) {
						int index = mapX.FindAndRemove(brickViewData);
						brickViewData.MapNode.Column = index;
						brickViewData.MapNode.Row = y;
						brickViewData.MapNode.GeneralBrickIndex = commonIndex++;
					}
				}
			}
		}
	}
}
