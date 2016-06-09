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
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Map {
	public class KeyColorColorizer : MapColorizer, ILegendDataProvider {
		public static readonly DependencyProperty ItemKeyProviderProperty = DependencyPropertyManager.Register("ItemKeyProvider",
			typeof(IColorizerItemKeyProvider), typeof(KeyColorColorizer), new PropertyMetadata(null, InvalidateColorsAndLegend));
		public static readonly DependencyProperty KeysProperty = DependencyPropertyManager.Register("Keys",
			typeof(ColorizerKeyItemCollection), typeof(KeyColorColorizer), new PropertyMetadata(null, OwnedCollectionPropertyChanged));
		[Category(Categories.Appearance), TypeConverter(typeof(ExpandableObjectConverter))]
		public IColorizerItemKeyProvider ItemKeyProvider {
			get { return (IColorizerItemKeyProvider)GetValue(ItemKeyProviderProperty); }
			set { SetValue(ItemKeyProviderProperty, value); }
		}
		[Category(Categories.Appearance)]
		public ColorizerKeyItemCollection Keys {
			get { return (ColorizerKeyItemCollection)GetValue(KeysProperty); }
			set { SetValue(KeysProperty, value); }
		}
		readonly Dictionary<object, ColorizerKeyItem> actualKeyItems = new Dictionary<object, ColorizerKeyItem>();
		bool UseAutocreatedItems { get { return Keys == null || Keys.Count == 0; } }
		internal IEnumerable<ColorizerKeyItem> ActualKeyItems { get { return actualKeyItems.Values; } }
		public KeyColorColorizer() {
			SetValue(KeysProperty, new ColorizerKeyItemCollection());
		}
		#region ILegendDataProvider Members
		IList<MapLegendItemBase> ILegendDataProvider.CreateItems(MapLegendBase legend) {
			return CreateLegendItems(legend);
		}
		#endregion
		ColorizerKeyItem FindItemByKey(object key, out int itemIndex) {
			itemIndex = -1;
			for (int i = 0; i < Keys.Count; i++) {
				ColorizerKeyItem item = Keys[i];
				if (Object.Equals(item.Key, key)) {
					itemIndex = i;
					return item;
				}
			}
			return null;
		}
		ColorizerKeyItem GetKeyItem(object key) {
			ColorizerKeyItem keyItem = null;
			int itemIndex;
			if (UseAutocreatedItems) {
				keyItem = new ColorizerKeyItem() { Key = key };
				itemIndex = actualKeyItems.Count;
			}
			else
				keyItem = FindItemByKey(key, out itemIndex);
			if (keyItem != null) {
				int itemColorIndex = itemIndex % ActualColors.Count;
				keyItem.Color = ActualColors[itemColorIndex];
			}
			return keyItem;
		}
		protected internal override void Reset() {
			base.Reset();
			actualKeyItems.Clear();
		}
		protected virtual IList<MapLegendItemBase> CreateLegendItems(MapLegendBase legend) {
			ColorizerLegendItemsBuilderBase builder = CreateLegendItemBuilder(legend);
			return builder != null ? builder.CreateItems() : new List<MapLegendItemBase>();
		}
		protected virtual ColorizerLegendItemsBuilderBase CreateLegendItemBuilder(MapLegendBase legend) {
			return new KeyValueColorizerLegendItemsBuilder(legend, this);
		}
		protected override MapDependencyObject CreateObject() {
			return new KeyColorColorizer();
		}
		public override Color GetItemColor(IColorizerElement item) {
			if (item == null || ItemKeyProvider == null)
				return MapColorizer.DefaultColor;
			object key = ItemKeyProvider.GetItemKey(item);
			if (key == null)
				return MapColorizer.DefaultColor;
			ColorizerKeyItem keyItem = null;
			if (!actualKeyItems.TryGetValue(key, out keyItem)) {
				keyItem = GetKeyItem(key);
				if (keyItem != null)
					actualKeyItems.Add(key, keyItem);
			}
			return keyItem != null ? keyItem.Color : MapColorizer.DefaultColor;
		}
	}
}
