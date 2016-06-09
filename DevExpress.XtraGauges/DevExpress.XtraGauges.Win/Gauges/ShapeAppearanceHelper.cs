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
using DevExpress.Utils;
using DevExpress.XtraGauges.Core.Drawing;
namespace DevExpress.XtraGauges.Win.Gauges {
	public static class ShapeAppearanceHelper {
		public static void Combine(RangeBarAppearance target, RangeBarAppearance main, RangeBarAppearance defaultAppearance) {
			target.BorderBrush = CombineBrushes(main.BorderBrush, defaultAppearance.BorderBrush);
			target.ContentBrush = CombineBrushes(main.ContentBrush, defaultAppearance.ContentBrush);
			target.BackgroundBrush = CombineBrushes(main.BackgroundBrush, defaultAppearance.BackgroundBrush);
		}
		public static void Combine(BaseTextAppearance target, BaseTextAppearance main, BaseTextAppearance defaultAppearance) {
			target.TextBrush = CombineBrushes(main.TextBrush, defaultAppearance.TextBrush);
			target.Font = (main.Font != BaseTextAppearance.DefaultFont) ? main.Font : defaultAppearance.Font;
		}
		private static BrushObject CombineBrushes(BrushObject brushObjectMain, BrushObject brushObjectDefault) {
			return (brushObjectMain == BrushObject.Empty) ? brushObjectDefault : brushObjectMain;
		}
		public static void Init(RangeBarAppearance result, AppearanceDefault defaulAppearance) {
			result.BorderBrush = BrushesCache.GetBrushByColor(defaulAppearance.BorderColor);
			result.ContentBrush = BrushesCache.GetBrushByColor(defaulAppearance.ForeColor);
			result.BackgroundBrush = BrushesCache.GetBrushByColor(defaulAppearance.BackColor);
		}
		public static void Init(BaseTextAppearance result, AppearanceDefault defaulAppearance) {
			if(defaulAppearance != null) {
				result.TextBrush = BrushesCache.GetBrushByColor(defaulAppearance.ForeColor);
				result.Font = defaulAppearance.Font;
			}
			else {
				result.TextBrush = BrushesCache.GetBrushByColor(Color.Black);
				result.Font = new Font("Tahoma", 8);
			}
		}
	}
	static class BrushesCache {
		static IDictionary<Color, SolidBrushObject> brushes = new Dictionary<Color, SolidBrushObject>();
		static void ResetCache(IDictionary<Color, SolidBrushObject> cache) {
			foreach(KeyValuePair<Color, SolidBrushObject> pair in cache) {
				if(pair.Value != null)
					pair.Value.Dispose();
			}
			cache.Clear();
		}
		public static void Reset() {
			ResetCache(brushes);
		}
		public static SolidBrushObject GetBrushByColor(Color color) {
			SolidBrushObject brush;
			if(!brushes.TryGetValue(color, out brush)) {
				brush = new SolidBrushObject(color);
				brushes.Add(color, brush);
			}
			return brushes[color];
		}
	}
}
