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

using System.Collections.Generic;
using System.Drawing;
using System;
using DevExpress.XtraMap.Drawing;
using System.ComponentModel;
namespace DevExpress.XtraMap {
	public class GraphColorizer : GenericColorizer<ColorizerColorItem> {
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("GraphColorizerColorItems"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new GenericColorizerItemCollection<ColorizerColorItem> ColorItems {
			get { return (GenericColorizerItemCollection<ColorizerColorItem>)base.ColorItems; }
		}
		Color GetColor(IColorizerElement element) {
			Dictionary<Color, int> intersect = new Dictionary<Color, int>();
			MapShape mapShape = element as MapShape;
			MapRect[] targetRects = GetShapeBounds(mapShape);
			foreach(MapRect targetRect in targetRects) {
				foreach(MapItem item in mapShape.Layer.DataItems) {
					IColorizerElement shp = item as IColorizerElement;
					if(shp == null || shp.ColorizerColor == Color.Transparent)
						continue;
					MapRect[] rects = GetShapeBounds(shp as MapShape);
					foreach(MapRect shpRect in rects) {
						if(MapRect.IsIntersected(shpRect, targetRect)) {
							if(intersect.ContainsKey(shp.ColorizerColor))
								intersect[shp.ColorizerColor]++;
							else
								intersect.Add(shp.ColorizerColor, 1);
						}
					}
				}
			}
			int actualUsedColorsCount = ActualColorItems != null && ActualColorItems.Count > 1 ? ActualColorItems.Count - 1 : 0;
			for(int i = 0; i < actualUsedColorsCount; i++) {
				if(!intersect.ContainsKey(ActualColorItems[i].Color))
					return ActualColorItems[i].Color;
			}
			return ActualColorItems[actualUsedColorsCount].Color;
		}
		MapRect[] GetShapeBounds(MapShape shape) {
			IRenderItemContainer container = shape as IRenderItemContainer;
			if(container != null) {
				List<MapRect> bounds = new List<MapRect>();
				foreach(IRenderItem part in container.Items)
					bounds.Add(part.Geometry.Bounds);
				return bounds.ToArray();
			}
			IRenderItem item = shape as IRenderItem;
			if (item != null)
				item.PrepareGeometry();
			return new MapRect[] { shape.Bounds };
		}
		public override string ToString() {
			return "(GraphColorizer)";
		}
		public override void ColorizeElement(IColorizerElement element) {
			if(element == null || ActualColorItems == null || ActualColorItems.Count < 2)
				return;
			Color color = GetColor(element);
			if(color == Color.Transparent)
				return;
			element.ColorizerColor = color;
		}
		protected override ColorizerColorItem CreateColorItem(Color color) {
			return new ColorizerColorItem(color);
		}
	}
}
