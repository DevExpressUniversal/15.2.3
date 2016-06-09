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
using System;
using System.ComponentModel;
using System.Drawing.Design;
using DevExpress.XtraMap.Native;
using DevExpress.Utils;
using DevExpress.Utils.Editors;
using System.Drawing;
namespace DevExpress.XtraMap {
	public class ColorizerKeyItem : ISupportObjectChanged {
		string name;
		object key;
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("ColorizerKeyItemName"),
#endif
		DefaultValue(null)]
		public string Name {
			get { return name; }
			set {
				if(string.Equals(name, value))
					return;
				name = value;
				RaiseChanged();
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("ColorizerKeyItemKey"),
#endif
		DefaultValue(null),
		Editor(typeof(UIObjectEditor), typeof(UITypeEditor)), TypeConverter(typeof(ObjectEditorTypeConverter))]
		public object Key {
			get { return key; }
			set {
				if(object.Equals(key, value))
					return;
				object oldKey = key;
				key = value;
				OnKeyChanged(key, oldKey);
				RaiseChanged();
			}
		}
		public ColorizerKeyItem() {
		}
		#region ISupportObjectChanged implementation
		EventHandler onChanged;
		event EventHandler ISupportObjectChanged.Changed {
			add { onChanged += value; }
			remove { onChanged -= value; }
		}
		protected internal void RaiseChanged() {
			if(onChanged != null) onChanged(this, EventArgs.Empty);
		}
		#endregion
		protected virtual void OnKeyChanged(object key, object oldKey) {
			RaiseKeyChanged(key, oldKey);
		}
		internal event KeyChangedEventHandler KeyChanged;
		protected internal virtual void RaiseKeyChanged(object key, object oldKey) {
			if(KeyChanged != null) {
				KeyChangedEventArgs args = new KeyChangedEventArgs(key, oldKey);
				KeyChanged(this, args);
			}
		}
		internal string GetText() {
			return !string.IsNullOrEmpty(Name) ? Name : Convert.ToString(Key);
		}
		public override string ToString() {
			return "(ColorizerKeyItem)";
		}
	}
	public class ColorizerKeyCollection : NotificationCollection<ColorizerKeyItem> { 
		readonly Dictionary<object, ColorizerKeyItem> keyHash = new Dictionary<object, ColorizerKeyItem>();
		protected internal Dictionary<object, ColorizerKeyItem> KeyHash { get { return keyHash; } }
		public ColorizerKeyCollection() {
		}
		protected internal ColorizerKeyItem GetItemByKey(object key) {
			ColorizerKeyItem result;
			if(KeyHash.TryGetValue(key, out result))
				return result;
			else
				return null;
		}
		protected object GetItemKey(ColorizerKeyItem value) {
			return value != null ? value.Key : null;
		}
		protected override bool OnClear() {
			foreach(ColorizerKeyItem item in this)
				item.KeyChanged -= OnKeyChanged;
			return base.OnClear();
		}
		protected override void OnClearComplete() {
			KeyHash.Clear();
			base.OnClearComplete();
		}
		protected internal virtual void OnKeyChanged(object sender, KeyChangedEventArgs e) {
			if(e.OldKey != null) {
				KeyHash.Remove(e.OldKey);
			}
			if(e.Key != null)
				KeyHash.Add(e.Key, (ColorizerKeyItem)sender);
		}
		protected override void OnInsertComplete(int index, ColorizerKeyItem value) {
			base.OnInsertComplete(index, value);
			object key = GetItemKey(value);
			if(key != null) {
				if(KeyHash.ContainsKey(key))
					KeyHash[key] = value;
				else
					KeyHash.Add(key, value);
			}
			value.KeyChanged += OnKeyChanged;
		}
		protected override void OnRemoveComplete(int index, ColorizerKeyItem value) {
			value.KeyChanged -= OnKeyChanged;
			object key = GetItemKey(value);
			if(key != null) {
				if(KeyHash.ContainsKey(key))
					KeyHash.Remove(key);
			}
			base.OnRemoveComplete(index, value);
		}
	}
	public class KeyColorColorizer : PredefinedColorsColorizer, IColorizerLegendDataProvider {
		const string DefaultCollectionEditor = "DevExpress.Utils.UI." + "CollectionEditor," + AssemblyInfo.SRAssemblyUtilsUI;
		readonly Dictionary<object, int> displayItemIndexHash = new Dictionary<object, int>();
		readonly Dictionary<object, int> keyIndexHash = new Dictionary<object, int>();
		readonly object displayItemsLocker = new object();
		readonly ColorizerKeyCollection keys = new ColorizerKeyCollection();
		ColorCollection colors = new ColorCollection();
		ColorCollection predefinedColors = new ColorCollection();
		NotificationCollectionChangedListener<ColorizerKeyItem> keysListener;
		List<ColoredDisplayItem> createdItems = new List<ColoredDisplayItem>();
		ColoredDisplayItem[] displayItems;
		IColorizerItemKeyProvider itemKeyProvider;
		bool shouldUpdateItems;
		protected bool ShouldAutoCreateKeys { get { return Keys.Count == 0; } }
		protected ColorCollection PredefinedColors { get { return predefinedColors; } }
		protected internal virtual ColorCollection ActualColors {
			get {
				return (PredefinedColorSchema != PredefinedColorSchema.None) ? PredefinedColors : Colors;
			}
		}
		internal object DisplayItemsLocker { get { return displayItemsLocker; } }
		internal List<ColoredDisplayItem> CreatedItems { get { return createdItems; } }
		internal IEnumerable<ColoredDisplayItem> DisplayItems { get { return displayItems != null ? displayItems : new ColoredDisplayItem[0]; } }
		protected Dictionary<object, int> KeyIndexHash { 
			get {
				if(keyIndexHash.Count == 0)
					PopulateKeyIndexHash();
				return keyIndexHash; 
			} 
		}
		void PopulateKeyIndexHash() {
			int count = Keys.Count;
			for(int i = 0; i < count; i++) {
				keyIndexHash.Add(Keys[i].Key, i);
			}
		}
		[Category(SRCategoryNames.Appearance),
		Editor(DefaultCollectionEditor, typeof(UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ColorCollection Colors { get { return colors; } }
		[
		Category(SRCategoryNames.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public ColorizerKeyCollection Keys { get { return keys; } }
		[
		Category(SRCategoryNames.Behavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		TypeConverter("DevExpress.XtraMap.Design.ExpandableNoneStringSupportedTypeConverter," + AssemblyInfo.SRAssemblyMapDesign),
		Editor("DevExpress.XtraMap.Design.ColorizerItemKeyProviderPickerEditor," + AssemblyInfo.SRAssemblyMapDesign, typeof(UITypeEditor)),
		DefaultValue(null),
#if !SL
	DevExpressXtraMapLocalizedDescription("KeyColorColorizerItemKeyProvider")
#else
	Description("")
#endif
		]
		public IColorizerItemKeyProvider ItemKeyProvider {
			get {
				return itemKeyProvider;
			}
			set {
				if(Object.Equals(itemKeyProvider, value))
					return;
				itemKeyProvider = value;
				InvalidateItems();
				InvalidateMap();
			}
		}
		public KeyColorColorizer() {
			this.keysListener = new NotificationCollectionChangedListener<ColorizerKeyItem>(Keys);
		}
		protected internal override void UpdatePredefinedColors(ColorCollection colors) {
			this.predefinedColors = colors;
		}
		protected override void DisposeOverride() {
			Keys.Clear();
			colors.Clear();
			predefinedColors = null;
			base.DisposeOverride();
		}
		protected override void SubscribeEvents() {
			base.SubscribeEvents();
			colors.CollectionChanged += OnColorsChanged;
			SubscribeKeysListenerEvents();
		}
		protected override void UnsubscribeEvents() {
			base.UnsubscribeEvents();
			UnsubscribeKeysListenerEvents();
		}
		void SubscribeKeysListenerEvents() {
			keysListener.Changed += OnKeysListenerChanged;
		}
		void UnsubscribeKeysListenerEvents() {
			if(keysListener != null)
				keysListener.Changed -= OnKeysListenerChanged;
		}
		void OnKeysListenerChanged(object sender, EventArgs e) {
			RecreateKeyIndexHash();
			OnColorizerChanged();
		}
		void RecreateKeyIndexHash() {
			keyIndexHash.Clear();
		}
		void OnColorsChanged(object sender, CollectionChangedEventArgs<Color> e) {
			OnColorizerChanged();
		}
		protected override void OnPredefinedColorSchemaChanged() {
			InvalidateItems();
			base.OnPredefinedColorSchemaChanged();
		}
		void InvalidateItems() {
			shouldUpdateItems = true;
		}
		protected internal override void OnColorizerChanged() {
			InvalidateItems();
			base.OnColorizerChanged();
		}
		void IColorizerLegendDataProvider.StartColorize() {
			if(shouldUpdateItems)
				ClearDisplayItems();
		}
		void IColorizerLegendDataProvider.EndColorize() {
			if(shouldUpdateItems) {
				CopyDisplayItems();
				if(Layer != null)
					Layer.UpdateLegends();
				shouldUpdateItems = false;
			}
		}
		public override void ColorizeElement(IColorizerElement element) {
			if(element == null || ItemKeyProvider == null)
				return;
			object key = ItemKeyProvider.GetItemKey(element);
			if(key == null)
				return;
			ColoredDisplayItem item = GetColoredItem(key);
			if(item != null)
				element.ColorizerColor = item.Color;
		}
		ColorizerKeyItem FindKeyItemByKey(object key) {
			return key != null ? Keys.GetItemByKey(key) : null;
		}
		void RegisterDisplayItem(object key, string text) {
			if(!displayItemIndexHash.ContainsKey(key)) {
				int keyIndex = ShouldAutoCreateKeys ?  createdItems.Count : CalculateKeyIndex(key);
				Color color = CalculateColor(keyIndex);
				createdItems.Add(new ColoredDisplayItem() { Text = text, Color = color });
				displayItemIndexHash[key] = createdItems.Count - 1;
				shouldUpdateItems = true;
			}
		}
		ColoredDisplayItem GetColoredItem(object key) {
			int index = CalculateDisplayItemIndex(key);
			if(index >= 0) {
				ColoredDisplayItem item = createdItems[index];
				return item;
			}
			return null;
		}
		Color CalculateColor(int keyIndex) {
			int count = ActualColors.Count;
			if(count > 0) {
				int index = keyIndex % count;
				return ActualColors[index];
			}
			return Color.Empty;
		}
		int CalculateDisplayItemIndex(object key) {
			int index = -1;
			if(displayItemIndexHash.TryGetValue(key, out index))
				return index;
			if(IsKeyValid(key)) {
				AddNewDisplayItem(key);
				return createdItems.Count - 1;
			} 
			return -1;
		}
		bool IsKeyValid(object key) {
			if(ShouldAutoCreateKeys)
				return true;
			return Keys.GetItemByKey(key) != null;
		}
		int CalculateKeyIndex(object key) {
			return KeyIndexHash[key];
		}
		void AddNewDisplayItem(object key) {
			AddNewDisplayItem(key, GetTextByKey(key));
		}
		internal void AddNewDisplayItem(object key, string text) {
			RegisterDisplayItem(key, text);
		}
		string GetTextByKey(object key) {
			if(!ShouldAutoCreateKeys) {
				ColorizerKeyItem item = FindKeyItemByKey(key);
				if(item != null) return item.GetText();
			}
			return Convert.ToString(key);
		}
		IList<MapLegendItemBase> ILegendDataProvider.CreateItems(MapLegendBase legend) {
			return CreateLegendItems(legend);
		}
		internal void ClearDisplayItems() {
			displayItemIndexHash.Clear();
			createdItems.Clear();
		}
		internal void CopyDisplayItems() {
			lock(DisplayItemsLocker) {
				displayItems = CreatedItems.ToArray();
			}
		}
		protected virtual IList<MapLegendItemBase> CreateLegendItems(MapLegendBase legend) {
			if(displayItemIndexHash.Count == 0)
				return new MapLegendItemBase[0];
			ColorizerLegendItemsBuilderBase builder = CreateLegendItemBuilder();
			return builder != null ? builder.CreateItems() : new List<MapLegendItemBase>();
		}
		protected virtual ColorizerLegendItemsBuilderBase CreateLegendItemBuilder() {
			return new KeyColorColorizerLegendItemsBuilder(this);
		}
		public Color GetColor(object key) {
			if (key == null)
				return Color.Empty;
			ColoredDisplayItem item = GetColoredItem(key);
			return item != null ? item.Color : Color.Empty;
		}
		public override string ToString() {
			return "(KeyColorColorizer)";
		}
	}
}
namespace DevExpress.XtraMap.Native {
	public interface IColorizerLegendDataProvider : ILegendDataProvider {
		void StartColorize();
		void EndColorize();
	}
	public class ColoredDisplayItem {
		public Color Color { get; set; }
		public string Text { get; set; }
	}
	public class KeyColorColorizerLegendItemsBuilder : ColorizerLegendItemsBuilderBase {
		protected new KeyColorColorizer Colorizer { get { return (KeyColorColorizer)base.Colorizer; } }
		public KeyColorColorizerLegendItemsBuilder(KeyColorColorizer colorizer)
			: base(colorizer) {
		}
		public override List<MapLegendItemBase> CreateItems() {
			List<MapLegendItemBase> result = new List<MapLegendItemBase>();
			lock(Colorizer.DisplayItemsLocker) {
				IEnumerable<ColoredDisplayItem> items = Colorizer.DisplayItems;
				foreach(ColoredDisplayItem item in items) {
					MapLegendItemBase legendItem = CreateLegendItem(item.Color, item.Text, double.NaN);
					result.Add(legendItem);
				}
			}
			return result;
		}
	}
}
