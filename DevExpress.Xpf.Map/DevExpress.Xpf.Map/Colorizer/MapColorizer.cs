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

using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
namespace DevExpress.Xpf.Map {
	public interface IColorizerElement {
		Color ColorizerColor { get; set; }
	}
	public class GenericColorizerItemCollection<T> : MapElementCollection<T>  {
		MapColorizer Colorizer { get { return Owner as MapColorizer; } }
		protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			base.OnCollectionChanged(e);
			if(Colorizer != null)
				Colorizer.Invalidate();
		}
	}
	public class ColorCollection : GenericColorizerItemCollection<Color> {
	}
	public abstract class MapColorizer : MapDependencyObject, IOwnedElement {
		public static readonly DependencyProperty ColorsProperty = DependencyPropertyManager.Register("Colors",
			typeof(ColorCollection), typeof(MapColorizer), new PropertyMetadata(null, OwnedCollectionPropertyChanged));
		protected static void OwnedCollectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapColorizer colorizer = d as MapColorizer;
			if (colorizer != null) {
				CommonUtils.SetOwnerForValues(e.OldValue, e.NewValue, colorizer);
				colorizer.Invalidate();
			}
		}
		protected static void InvalidateColorsAndLegend(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapColorizer colorizer = d as MapColorizer;
			if (colorizer != null)
				colorizer.Invalidate();
		}
		Dictionary<Color, Brush> BrushesCache = new Dictionary<Color, Brush>();
		[Category(Categories.Appearance)]
		public ColorCollection Colors {
			get { return (ColorCollection)GetValue(ColorsProperty); }
			set { SetValue(ColorsProperty, value); }
		}
		internal static readonly Color DefaultColor = System.Windows.Media.Colors.Transparent;
		readonly ColorCollection defaultColors;
		object owner;
		protected ColorCollection DefaultColors { get { return defaultColors; } }
		protected ColorCollection ActualColors { get { return Colors != null && Colors.Count > 0 ? Colors : DefaultColors; } }
		protected internal MapControl Map { get { return ObtainMap(); } }
		protected internal VectorLayerBase Layer { get { return owner as VectorLayerBase; } }
		public MapColorizer() {
			Colors = new ColorCollection();
			this.defaultColors = CreateDefaultColors();
		}
		#region IOwnedElement implementation
		object IOwnedElement.Owner {
			get { return owner; }
			set {
				if (owner != value) {
					owner = value;
					OwnerChanged();
				}
			}
		}
		#endregion
		ColorCollection CreateDefaultColors() {
			return new ColorCollection() {
				Color.FromArgb(0xFF, 0x4F, 0x81, 0xBD),
				Color.FromArgb(0xFF, 0xC0, 0x50, 0x4D),
				Color.FromArgb(0xFF, 0x9B, 0xBB, 0x59),
				Color.FromArgb(0xFF, 0x80, 0x64, 0xA2),
				Color.FromArgb(0xFF, 0x4B, 0xAC, 0xC6),
				Color.FromArgb(0xFF, 0xF7, 0x96, 0x46)
			};
		}
		MapControl ObtainMap() {
			MapControl map = owner as MapControl;
			if (map != null)
				return map;
			LayerBase layer = owner as LayerBase;
			if (layer != null) return layer.Map;
			return null;
		}
		protected virtual void OwnerChanged() {
		}
		protected internal void Invalidate() {
			if (Map != null)
				Map.OnColorizerChanged();
		}
		protected internal virtual void Reset() {
		}
		public abstract Color GetItemColor(IColorizerElement element);
		protected internal Brush GetBrush(Color color) {
			Brush brush = null;
			if(BrushesCache.TryGetValue(color, out brush))
				return brush;
			brush = new SolidColorBrush(color);
			BrushesCache[color] = brush;
			return brush;
		}
	}
}
