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

using DevExpress.XtraMap.Drawing;
using DevExpress.XtraMap.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraMap.Drawing {
	[CLSCompliant(false)]
	public class MapItemStyleCache {
		static readonly MapItemStyleCache instance = new MapItemStyleCache();
		public static MapItemStyleCache Instance { get { return instance; } }
		static readonly MapItemStyle readOnlyPointerStyle = new MapItemStyle() { Fill = Color.Transparent, Stroke = Color.Transparent, StrokeWidth = 0 };
		public static IRenderItemStyle ReadOnlyPointerStyle { get { return readOnlyPointerStyle; } }
		SimpleColorStyleCache<MapItemFillStyle> fillStyles = new SimpleColorStyleCache<MapItemFillStyle>();
		SimpleColorStyleCache<MapItemStrokeStyle> strokeStyles = new SimpleColorStyleCache<MapItemStrokeStyle>();
		SimpleColorStyleCache<MapItemTextColorStyle> textStyles = new SimpleColorStyleCache<MapItemTextColorStyle>();
		SimpleColorStyleCache<MapItemTextGlowColorStyle> textGlowStyles = new SimpleColorStyleCache<MapItemTextGlowColorStyle>();
		protected internal SimpleColorStyleCache<MapItemFillStyle> FillStyles { get { return fillStyles; } }
		protected internal SimpleColorStyleCache<MapItemStrokeStyle> StrokeStyles { get { return strokeStyles; } }
		protected internal SimpleColorStyleCache<MapItemTextColorStyle> TextStyles { get { return textStyles; } }
		protected internal SimpleColorStyleCache<MapItemTextGlowColorStyle> TextGlowStyles { get { return textGlowStyles; } }
		public MapItemFillStyle GetColorizerStyle(Color color) {
			MapItemFillStyle style = FillStyles.GetStyle(color);
			if (style == null) {
				style = new MapItemFillStyle() { Fill = color };
				FillStyles.RegisterStyle(style);
			}
			return style;
		}
		public MapItemTextColorStyle GetTextColorStyle(Color color) {
			MapItemTextColorStyle style = TextStyles.GetStyle(color);
			if(style == null) {
				style = new MapItemTextColorStyle() { TextColor = color };
				TextStyles.RegisterStyle(style);
			}
			return style;
		}
		public MapItemTextGlowColorStyle GetTextGlowColorStyle(Color color) {
			MapItemTextGlowColorStyle style = TextGlowStyles.GetStyle(color);
			if(style == null) {
				style = new MapItemTextGlowColorStyle() { TextGlowColor = color };
				TextGlowStyles.RegisterStyle(style);
			}
			return style;
		}
		public MapItemStrokeStyle GetStrokeStyle(Color color) {
			MapItemStrokeStyle style = StrokeStyles.GetStyle(color);
			if(style == null) {
				style = new MapItemStrokeStyle() { Stroke = color };
				StrokeStyles.RegisterStyle(style);
			}
			return style;
		}
	}
	[CLSCompliant(false)]
	public class SimpleColorStyleCache<T> where T : MapItemColorStyle {
		Dictionary<UInt32, T> colorizerStyles = new Dictionary<UInt32, T>();
		protected Dictionary<uint, T> Styles { get { return colorizerStyles; } }
		public int Count { get { return Styles.Count; } }
		public void Reset() {
			Styles.Clear();
		}
		protected UInt32 GetStyleKey(Color color) {
			return (UInt32)MapUtils.ColorToUInt(color);
		}
		public void RegisterStyle(T style) {
			UInt32 key = GetStyleKey(style.Color);
			if(!Styles.ContainsKey(key))
				Styles.Add(key, style);
		}
		public T GetStyle(Color color) {
			UInt32 key = MapUtils.ColorToUInt(color);
			T style;
			if(Styles.TryGetValue(key, out style))
				return style;
			return null;
		}
	}
}
