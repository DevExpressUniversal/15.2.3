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
using System.Windows;
using System.Windows.Media;
using DevExpress.Xpf.Map.Native;
namespace DevExpress.Xpf.Map {
	public class GraphColorizer : MapColorizer {
		Color GetColor(MapShapeBase shape) {
			ColorCollection colors = ActualColors;
			Dictionary<Color, int> intersect = new Dictionary<Color, int>();
			Rect targetRect = GetShapeBounds(shape);
			foreach (MapItem item in shape.Layer.DataItems) {
				MapShapeBase shp = item as MapShapeBase;
				if (shp == null || shp.ColorizerColor == DefaultColor)
					continue;
				Rect rect = GetShapeBounds(shp);
				rect.Intersect(targetRect);
				if (!rect.IsEmpty) {
					if (intersect.ContainsKey(shp.ColorizerColor)) {
						intersect[shp.ColorizerColor]++;
					}
					else
						intersect.Add(shp.ColorizerColor, 1);
				}
			}
			int actualUsedColorsCount = colors != null && colors.Count > 1 ? colors.Count - 1 : 0;
			for (int i = 0; i < actualUsedColorsCount; i++) {
				if (!intersect.ContainsKey(colors[i]))
					return colors[i];
			}
			return colors[actualUsedColorsCount];
		}
		Rect GetShapeBounds(MapShapeBase shape) {
			IMapItem item = shape as IMapItem;
			return item != null ? new Rect(item.Location, item.Size) : Rect.Empty;
		}
		protected override MapDependencyObject CreateObject() {
			return new GraphColorizer();
		}
		public override Color GetItemColor(IColorizerElement item) {
			if (item == null)
				return DefaultColor;
			Color actualItemColorizerColor = item.ColorizerColor;
			if (ActualColors == null || ActualColors.Count < 2)
				return actualItemColorizerColor;
			MapShapeBase shape = item as MapShapeBase;
			if (shape == null || shape.ColorizerColor != DefaultColor)
				return actualItemColorizerColor;
			Color color = GetColor(shape);
			if (color != DefaultColor)
				return color;
			else
				return actualItemColorizerColor;
		}
	}
}
