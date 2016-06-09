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

using System.Windows;
using System.Windows.Media;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Charts.Native;
namespace DevExpress.Xpf.Charts {
	public interface IColorizerKeyProvider {
		object GetKey(object item);
	}
	public class KeyColorColorizer : LegendSupportColorizerBase {
		public static readonly DependencyProperty KeyProviderProperty = DependencyPropertyManager.Register("KeyProvider", typeof(IColorizerKeyProvider), typeof(KeyColorColorizer));
		public static readonly DependencyProperty KeysProperty = DependencyPropertyManager.Register("Keys", typeof(ColorizerKeysCollection), typeof(KeyColorColorizer), new PropertyMetadata(OnKeysPropertyChanged));
		[Category(Categories.Appearance),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true),
		TypeConverter(typeof(ExpandableObjectConverter))]
		public IColorizerKeyProvider KeyProvider {
			get { return (IColorizerKeyProvider)GetValue(KeyProviderProperty); }
			set { SetValue(KeyProviderProperty, value); }
		}
		[Category(Categories.Appearance),
		XtraSerializableProperty(XtraSerializationVisibility.SimpleCollection)]
		public ColorizerKeysCollection Keys {
			get { return (ColorizerKeysCollection)GetValue(KeysProperty); }
			set { SetValue(KeysProperty, value); }
		}
		static void OnKeysPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			KeyColorColorizer colorizer = d as KeyColorColorizer;
			if (colorizer != null)
				colorizer.OnKeysChanged(e.OldValue as ColorizerKeysCollection, e.NewValue as ColorizerKeysCollection);
		}
		readonly Dictionary<object, int> actualKeyIndex = new Dictionary<object, int>();
		bool UseAutocreatedItems { get { return Keys == null || Keys.Count == 0; } }
		Dictionary<object, int> ActualKeyIndex { get { return actualKeyIndex; } }
		public KeyColorColorizer() {
			SetValue(KeysProperty, new ColorizerKeysCollection());
			SetValue(LegendTextPatternProperty, "{V}");
		}
		void OnKeysChanged(ColorizerKeysCollection oldKeys, ColorizerKeysCollection newKeys) {
			if (oldKeys != null)
				oldKeys.CollectionChanged -= KeysCollectionChanged;
			if (newKeys != null)
				newKeys.CollectionChanged += KeysCollectionChanged;
			ActualKeyIndex.Clear();
		}
		void KeysCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			ActualKeyIndex.Clear();
			NotifyPropertyChanged("Keys");
		}
		protected override ChartDependencyObject CreateObject() {
			return new KeyColorColorizer();
		}
		protected override PatternDataProvider GetPatternDataProvider(PatternConstants patternConstant) {
			return patternConstant == PatternConstants.Value ? new KeyColorColorizerLegendItemDataProvider() : null;
		}
		protected override void UpdateLegendItems(List<ColorizerLegendItem> legendItems, Palette palette) {
			if (palette != null && palette.Count > 0 && ActualKeyIndex.Count > 0) {
				foreach (KeyValuePair<object, int> item in ActualKeyIndex) {
					Color color = palette[item.Value % palette.Count];
					string text = CreateLegendItemText(item.Key);
					legendItems.Add(new ColorizerLegendItem(color, text));
				}
			}
		}
		public override Color? GetPointColor(object argument, object[] values, object colorKey, Palette palette) {
			if (colorKey != null && palette != null && palette.Count > 0) {
				object key = KeyProvider != null ? KeyProvider.GetKey(colorKey) : colorKey;
				if (key != null) {
					int colorIndex;
					if (!ActualKeyIndex.TryGetValue(key, out colorIndex)) {
						colorIndex = UseAutocreatedItems ? ActualKeyIndex.Count : Keys.IndexOf(key);
						if (colorIndex >= 0)
							ActualKeyIndex.Add(key, colorIndex);
					}
					if (colorIndex >= 0)
						return palette[colorIndex % palette.Count];
				}
			}
			return null;
		}
	}
	public class ColorizerKeysCollection : ObservableCollection<object> {
	}
	public class ObjectKeyProvider : ChartDependencyObject, IColorizerKeyProvider {
		object IColorizerKeyProvider.GetKey(object item) {
			return item;
		}
		protected override ChartDependencyObject CreateObject() {
			return new ObjectKeyProvider();
		}
	}
}
