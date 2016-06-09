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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public abstract class HighlightedPointLayout {
	}
	public class HighlightedPolygonPointLayout : HighlightedPointLayout {
		readonly IPolygon polygon;
		readonly Color color;
		readonly Color borderColor;
		public IPolygon Polygon { get { return polygon; } }
		public Color Color { get { return color; } }
		public Color BorderColor { get { return borderColor; } }
		public HighlightedPolygonPointLayout(IPolygon polygon, Color color, Color borderColor) {
			this.polygon = polygon;
			this.color = color;
			this.borderColor = borderColor;
		}
	}
	public class HighlightedRangePointLayout : HighlightedPolygonPointLayout {
		readonly IPolygon polygon2;
		public IPolygon Polygon2 { get { return polygon2; } }
		public HighlightedRangePointLayout(IPolygon polygon, IPolygon polygon2, Color color, Color borderColor) : base(polygon, color, borderColor) {
			this.polygon2 = polygon2;
		}
	}
	public class HighlightedBarPointLayout : HighlightedPointLayout {
		readonly RectangleF rectangle;
		public RectangleF Rectangle { get { return rectangle; } }
		public HighlightedBarPointLayout(RectangleF rectangle) {
			this.rectangle = rectangle;
		}
	}
	public class HighlightedStockPointLayout : HighlightedPointLayout {
		readonly Color color;
		readonly RectangleF stockRect;
		readonly RectangleF openRect;
		readonly RectangleF closeRect;
		public Color Color { get { return color; } }
		public RectangleF StockRect { get { return stockRect; } }
		public RectangleF OpenRect { get { return openRect; } }
		public RectangleF CloseRect { get { return closeRect; } }
		public HighlightedStockPointLayout(Color color, RectangleF stockRect, RectangleF openRect, RectangleF closeRect) {
			this.color = color;
			this.stockRect = stockRect;
			this.openRect = openRect;
			this.closeRect = closeRect;
		}
	}
	public class HighlightedCandleStickPointLayout : HighlightedPointLayout {
		readonly Color color;
		readonly RectangleF stockLowRect;
		readonly RectangleF stockHighRect;
		readonly RectangleF[] bodyRects;
		public Color Color { get { return color; } }
		public RectangleF StockLowRect { get { return stockLowRect; } }
		public RectangleF StockHighRect { get { return stockHighRect; } }
		public RectangleF[] BodyRects { get { return bodyRects; } }
		public HighlightedCandleStickPointLayout(Color color, RectangleF stockLowRect, RectangleF stockHighRect, RectangleF[] bodyRects) {
			this.color = color;
			this.stockLowRect = stockLowRect;
			this.stockHighRect = stockHighRect;
			this.bodyRects = bodyRects;
		}
	}
}
