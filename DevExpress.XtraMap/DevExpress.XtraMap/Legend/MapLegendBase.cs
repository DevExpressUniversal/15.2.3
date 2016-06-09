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
using DevExpress.Utils;
using System.Drawing;
using DevExpress.XtraMap.Native;
using System.ComponentModel;
using System.Collections.Generic;
using DevExpress.XtraMap.Drawing;
using System.Drawing.Design;
using System.Globalization;
namespace DevExpress.XtraMap {
	public enum LegendAlignment {
		TopLeft = 1,
		TopCenter = 2,
		TopRight = 4,
		MiddleLeft = 16,
		MiddleRight = 64,
		BottomLeft = 256,
		BottomCenter = 512,
		BottomRight = 1024
	}
	public enum VisibilityMode {
		Auto,
		Visible,
		Hidden
	}
	public abstract class MapLegendBase : MapDisposableObject, IOwnedElement, IMapStyleOwner {
		public const LegendAlignment DefaultAlignment = LegendAlignment.TopLeft;
		public const VisibilityMode DefaultVisibility = VisibilityMode.Auto;
		const string DefaultHeader = "Legend";
		object owner;
		readonly MapLegendItemCollection innerItems;
		readonly MapLegendItemCollection customItems;
		LegendAlignment alignment = DefaultAlignment;
		VisibilityMode visibility = DefaultVisibility;
		readonly LegendAppearance appearance;
		string rangeStopsFormat = string.Empty;
		string header = DefaultHeader;
		string description = string.Empty;
		NotificationCollectionChangedListener<MapLegendItemBase> listener;
		protected object Owner { get { return owner; } }
		protected internal InnerMap Map { get { return Owner as InnerMap; } }
		protected virtual IMapEventHandler EventHandler { get { return Map != null ? Map.EventHandler : null; } }
		protected internal MapLegendItemCollection InnerItems { get { return innerItems; } }
		protected internal virtual bool ActualVisible {
			get { return Visibility != VisibilityMode.Hidden; }
		}
		protected internal virtual void EnsureItemsLoaded() {
		}
		protected internal abstract IList<MapLegendItemBase> GetItems();
		internal LegendAppearance Appearance { get { return appearance; } }
		protected MapLegendItemCollection CustomItems {
			get { return customItems; }
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapLegendBaseHeader"),
#endif
		Category(SRCategoryNames.Appearance), DefaultValue(DefaultHeader)]
		public string Header {
			get { return header; }
			set {
				if (header == value)
					return;
				header = value;
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapLegendBaseDescription"),
#endif
		Category(SRCategoryNames.Appearance), DefaultValue("")]
		public string Description {
			get { return description; }
			set {
				if (description == value)
					return;
				description = value;
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapLegendBaseAlignment"),
#endif
		Category(SRCategoryNames.Layout), DefaultValue(DefaultAlignment),
		Editor("DevExpress.XtraMap.Design.LegendAlignmentEditor," + "DevExpress.XtraMap" + AssemblyInfo.VSuffixDesign, typeof(UITypeEditor))
		]
		public LegendAlignment Alignment {
			get { return alignment; }
			set {
				if (alignment == value)
					return;
				alignment = value;
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapLegendBaseVisibility"),
#endif
		Category(SRCategoryNames.Behavior), DefaultValue(DefaultVisibility)]
		public VisibilityMode Visibility {
			get { return visibility; }
			set {
				visibility = value;
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapLegendBaseRangeStopsFormat"),
#endif
		Category(SRCategoryNames.Data), DefaultValue("")]
		public string RangeStopsFormat {
			get { return rangeStopsFormat; }
			set {
				if (rangeStopsFormat == value)
					return;
				rangeStopsFormat = value;
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapLegendBaseHeaderStyle"),
#endif
		Category(SRCategoryNames.Appearance), NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof(ExpandableObjectConverter))]
		public TextElementStyle HeaderStyle { get { return appearance.HeaderStyle; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapLegendBaseDescriptionStyle"),
#endif
		Category(SRCategoryNames.Appearance), NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof(ExpandableObjectConverter))]
		public TextElementStyle DescriptionStyle { get { return appearance.DescriptionStyle; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapLegendBaseItemStyle"),
#endif
		Category(SRCategoryNames.Appearance), NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof(ExpandableObjectConverter))]
		public MapElementStyle ItemStyle { get { return appearance.ItemStyle; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapLegendBaseBackgroundStyle"),
#endif
		Category(SRCategoryNames.Appearance), NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof(ExpandableObjectConverter))]
		public BackgroundStyle BackgroundStyle { get { return appearance.BackgroundStyle; } }
		protected MapLegendBase() {
			this.appearance = new LegendAppearance(this);
			this.innerItems = new MapLegendItemCollection();
			this.customItems = new MapLegendItemCollection();
			this.listener = new NotificationCollectionChangedListener<MapLegendItemBase>(customItems);
			SubscribeListenerEvents();
		}
		#region IMapStyleOwner implementation
		void IMapStyleOwner.OnStyleChanged() {
			OnChanged();
		}
		#endregion
		#region IOwnedElement implementation
		object IOwnedElement.Owner {
			get { return owner; }
			set {
				if(owner == value)
					return;
				owner = value;
			}
		}
		#endregion
		void SubscribeListenerEvents() {
			listener.Changed += OnCustomItemsCollectionChanged;
		}
		void OnCustomItemsCollectionChanged(object sender, EventArgs e) {
			OnChanged();
		}
		protected virtual void OnChanged() {
			if(EventHandler != null) EventHandler.OnLegendChanged(this);
		}
		protected internal virtual void Invalidate() { }
		protected internal virtual bool CanDisplayItems() {
			return true; 
		}
	}
	public abstract class MapLegendItemBase : ISupportObjectChanged {
		const double DefaultValue = 0.0;
		static readonly Color DefaultColor = Color.Empty;
		string text = "";
		Color color = DefaultColor;
		double itemValue = DefaultValue;
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapLegendItemBaseText"),
#endif
		DefaultValue("")]
		public string Text {
			get { return text; }
			set {
				if(string.Equals(text, value))
					return;
				text = value;
				RaiseChanged();
			}
		}
#if !SL
	[DevExpressXtraMapLocalizedDescription("MapLegendItemBaseColor")]
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
		bool ShouldSerializeColor() { return Color != DefaultColor; }
		void ResetColor() { Color = DefaultColor; }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapLegendItemBaseValue"),
#endif
		DefaultValue(DefaultValue)]
		public double Value {
			get { return itemValue; }
			set {
				if(itemValue == value)
					return;
				itemValue = value;
				RaiseChanged();
			}
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
		protected internal string GetActualText(DevExpress.XtraMap.Services.IColorizerLegendFormatService formatService, string format) {
			if(formatService != null)
				return formatService.FormatLegendItem(null, this);
			if(!String.IsNullOrEmpty(Text))
				return Text;
			return !double.IsNaN(Value) ? Value.ToString(format, CultureInfo.CurrentCulture) : String.Empty;
		}
	}
	public class ColorLegendItem : MapLegendItemBase {
		internal const int DefaultImageIndex = -1;
		int imageIndex = DefaultImageIndex;
		[DefaultValue(DefaultImageIndex)]
		public int ImageIndex {
			get { return imageIndex; }
			set {
				if (value < 0)
					value = -1;
				if (imageIndex == value)
					return;
				imageIndex = value;
				RaiseChanged();
			}
		}
		public override string ToString() {
			return "(ColorLegendItem)";
		}
	}
	public class SizeLegendItem : MapLegendItemBase {
		const int DefaultMarkerSize = 1;
		int markerSize = DefaultMarkerSize;
		[
		DefaultValue(DefaultMarkerSize)]
		public int MarkerSize {
			get { return markerSize; }
			set {
				if(markerSize == value)
					return;
				if (value <= 0)
					value = DefaultMarkerSize;
				markerSize = value;
				RaiseChanged();
			}
		}
		public override string ToString() {
			return "(SizeLegendItem)";
		}
	}
	public class MapLegendItemCollection : NotificationCollection<MapLegendItemBase>, ISupportSwapItems {
		#region ISupportSwapItems members
		void ISupportSwapItems.Swap(int index1, int index2) {
			if (index1 == index2)
				return;
			MapLegendItemBase swapItem = InnerList[index1];
			InnerList[index1] = InnerList[index2];
			InnerList[index2] = swapItem;
		}
		#endregion
	}
}
