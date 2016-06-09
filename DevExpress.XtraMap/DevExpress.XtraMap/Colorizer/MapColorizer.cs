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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraMap.Native;
namespace DevExpress.XtraMap {
	public class DoubleCollection : NotificationCollection<double> {
	}
	public class ColorCollection : NotificationCollection<Color> {
	}
	public class ColorizerColorItem : ISupportObjectChanged {
		static readonly Color DefaultColor = Color.Empty;
		Color color;
#if !SL
	[DevExpressXtraMapLocalizedDescription("ColorizerColorItemColor")]
#endif
		public Color Color {
			get { return color; }
			set {
				if(color == value)
					return;
				color = value;
				RaiseChanged();
			}
		}
		void ResetColor() { Color = DefaultColor; }
		bool ShouldSerializeColor() { return Color != DefaultColor; }
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
		public ColorizerColorItem()
			: this(Color.Empty) {
		}
		public ColorizerColorItem(Color color) {
			this.color = color;
		}
		public override string ToString() {
			return "(ColorizerColorItem)";
		}
	}
	public class GenericColorizerItemCollection<T> : NotificationCollection<T> where T : ColorizerColorItem {
		public ColorCollection ToColors() {
			ColorCollection result = new ColorCollection();
			lock (InnerList) { 
				ForEach((d) => result.Add(d.Color));
			}
			return result;
		}
	}
	public abstract class ColorizerBase<T> : MapDisposableObject where T : IColorizerElement {
		public abstract void ColorizeElement(T element);
	}
	public abstract class MapColorizer : ColorizerBase<IColorizerElement>, IOwnedElement {
		object owner;
		MapItemsLayerBase layer;
		protected object Owner { get { return owner; } }
		protected MapItemsLayerBase Layer {
			get {
				if(layer == null) layer = GetLayer();
				return layer;
			}
		}
		protected MapColorizer() {
		}
		protected virtual MapItemsLayerBase GetLayer() {
			MapItemsLayerBase layer = Owner as MapItemsLayerBase;
			if(layer == null) {
				IOwnedElement owned = Owner as IOwnedElement;
				return owned != null ? owned.Owner as MapItemsLayerBase : null;
			}
			return layer;
		}
		#region IOwnedElement implementation
		object IOwnedElement.Owner {
			get { return owner; }
			set {
				if(owner == value)
					return;
				owner = value;
				OwnerChanged();
			}
		}
		#endregion
		protected virtual void OwnerChanged() {
			if(Layer == null) {
				UnsubscribeEvents();
				layer = null;
			} else
				SubscribeEvents();
		}
		protected virtual void OnPredefinedColorSchemaChanged() {
			OnColorizerChanged();
		}
		protected virtual void UnsubscribeEvents() {
		}
		protected virtual void SubscribeEvents() {
		}
		protected void InvalidateMap() {
			if(Layer != null) Layer.InvalidateRender();
		}
		protected internal virtual void OnColorizerChanged() {
			if(Layer != null) {
				Layer.OnColorizerChanged();
				Layer.InvalidateRender();
			}
		}
	}
	public abstract class PredefinedColorsColorizer :  MapColorizer {
		PredefinedColorSchema predefinedColorSchema = PredefinedColorSchema.None;
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("PredefinedColorsColorizerPredefinedColorSchema"),
#endif
		Category(SRCategoryNames.Appearance), DefaultValue(PredefinedColorSchema.None)]
		public PredefinedColorSchema PredefinedColorSchema {
			get {
				return predefinedColorSchema;
			}
			set {
				if(predefinedColorSchema == value)
					return;
				predefinedColorSchema = value;
				OnPredefinedColorSchemaChanged();
			}
		}
		protected internal abstract void UpdatePredefinedColors(ColorCollection colors);
	}
	public abstract class GenericColorizer<T> : PredefinedColorsColorizer where T : ColorizerColorItem {
		readonly GenericColorizerItemCollection<T> colorItems;
		readonly GenericColorizerItemCollection<T> predefinedColorItems;
		NotificationCollectionChangedListener<T> listener;
		protected internal GenericColorizerItemCollection<T> ColorItems { get { return colorItems; } }
		protected internal GenericColorizerItemCollection<T> PredefinedColorItems { get { return predefinedColorItems; } }
		protected internal virtual GenericColorizerItemCollection<T> ActualColorItems {
			get {
				return (PredefinedColorSchema != PredefinedColorSchema.None) ? PredefinedColorItems : ColorItems;
			}
		}
		protected GenericColorizer() {
			this.colorItems = new GenericColorizerItemCollection<T>();
			this.predefinedColorItems = new GenericColorizerItemCollection<T>();
			this.listener = new NotificationCollectionChangedListener<T>(ColorItems);
			SubscribeListenerEvents();
		}
		protected ColorCollection GetColorCollection(GenericColorizerItemCollection<T> coloredItems) {
			return coloredItems.ToColors();
		}
		protected internal override void UpdatePredefinedColors(ColorCollection colors) {
			PredefinedColorItems.BeginUpdate();
			try {
				PredefinedColorItems.Clear();
				foreach(Color color in colors) {
					PredefinedColorItems.Add(CreateColorItem(color));
				}
			} finally {
				PredefinedColorItems.EndUpdate();
			}
		}
		protected abstract T CreateColorItem(Color color);
		protected override void SubscribeEvents() {
			base.SubscribeEvents();
			SubscribeListenerEvents();
		}
		protected override void UnsubscribeEvents() {
			base.UnsubscribeEvents();
			UnsubscribeListenerEvents();
		}
		protected virtual void OnListenerChanged(object sender, EventArgs e) {
			OnColorizerChanged();
		}
		void SubscribeListenerEvents() {
			listener.Changed += OnListenerChanged;
		}
		void UnsubscribeListenerEvents() {
			if(listener != null)
				listener.Changed -= OnListenerChanged;
		}
	}
}
namespace DevExpress.XtraMap.Native {
	public abstract class ColorizerLegendItemsBuilderBase {
		MapColorizer colorizer;
		protected MapColorizer Colorizer { get { return colorizer; } }
		protected ColorizerLegendItemsBuilderBase(MapColorizer colorizer) {
			Guard.ArgumentNotNull(colorizer, "colorizer");
			this.colorizer = colorizer;
		}
		public abstract List<MapLegendItemBase> CreateItems();
		protected MapLegendItemBase CreateLegendItem(Color color, string text, double value) {
			MapLegendItemBase item = new ColorLegendItem() { Color = color, Text = text, Value = value };
			return item;
		}
	}
}
