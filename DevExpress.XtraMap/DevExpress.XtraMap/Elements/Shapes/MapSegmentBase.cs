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

using DevExpress.Map;
using DevExpress.Utils;
using DevExpress.XtraMap.Drawing;
using DevExpress.XtraMap.Native;
using System;
using System.ComponentModel;
using System.Drawing;
using DevExpress.Map.Native;
using System.Collections.Generic;
namespace DevExpress.XtraMap {
	public abstract class MapSegmentBase : IRenderItem, IOwnedElement, IMapStyleOwner, IChangedCallbackOwner {
		readonly MapItemStyle actualStyle = new MapItemStyle();
		IRenderItemResourceHolder resourceHolder;
		object owner;
		IMapItemGeometry geometry;
		object updateLocker = new object();
		bool forceUpdateResourceHolder = false;
		bool geometryIsValid;
		Color fill = Color.Empty;
		Color stroke = Color.Empty;
		int strokeWidth = MapItemStyle.EmptyStrokeWidth;
		StyleMergerBase styleMerger;
		Action callback;
		protected MapUnitConverter UnitConverter {
			get {
				return MapPath != null ? MapPath.UnitConverter : EmptyUnitConverter.Instance;
			}
		}
		protected bool GeometryIsValid { get { return geometryIsValid; } set { geometryIsValid = value; } }
		protected internal UnitGeometry Geometry { get { return GetGeometry() as UnitGeometry; } }
		protected internal abstract MapItemStyle DefaultStyle { get; }
		protected internal MapItemStyle ActualStyle { get { return actualStyle; } }
		protected StyleMergerBase StyleMerger {
			get {
				if(styleMerger == null)
					this.styleMerger = CreateStyleMerger();
				return styleMerger;
			}
		}
		protected internal abstract MapShape MapPath { get; }
		protected object Owner { get { return owner; } set { owner = value; } }
		protected IRenderItem RenderItem { get { return Owner as IRenderItem; } }
		protected MapItemsLayerBase Layer { get { return MapPath != null ? MapPath.Layer : null; } }
		[Category(SRCategoryNames.Appearance),
#if !SL
	DevExpressXtraMapLocalizedDescription("MapSegmentBaseFill")
#else
	Description("")
#endif
]
		public Color Fill {
			get { return fill; }
			set {
				if(fill != value) {
					fill = value;
					OnStyleChanged();
				}
			}
		}
		void ResetFill() { Fill = Color.Empty; }
		protected bool ShouldSerializeFill() { return Fill != Color.Empty; }
		[Category(SRCategoryNames.Appearance),
#if !SL
	DevExpressXtraMapLocalizedDescription("MapSegmentBaseStroke")
#else
	Description("")
#endif
]
		public Color Stroke {
			get { return stroke; }
			set {
				if(stroke != value) {
					stroke = value;
					OnStyleChanged();
				}
			}
		}
		void ResetStroke() { Stroke = Color.Empty; }
		protected bool ShouldSerializeStroke() { return Stroke != Color.Empty; }
		[Category(SRCategoryNames.Appearance), 
		DefaultValue(MapItemStyle.EmptyStrokeWidth),
#if !SL
	DevExpressXtraMapLocalizedDescription("MapSegmentBaseStrokeWidth")
#else
	Description("")
#endif
]
		public int StrokeWidth {
			get { return strokeWidth; }
			set {
				if(strokeWidth != value) {
					strokeWidth = value;
					OnStyleChanged();
				}
			}
		}
		#region IOwnedElement implementation
		object IOwnedElement.Owner {
			get { return Owner; }
			set {
				Owner = value;
			}
		}
		#endregion
		#region IRenderItem
		IRenderShapeTitle IRenderItem.Title { get { return GetTitle(); } }
		IMapItemGeometry IRenderItem.Geometry { get { return GetGeometry(); } }
		IRenderItemStyle IRenderItem.Style { get { return ActualStyle; } }
		IRenderItemResourceHolder IRenderItem.ResourceHolder {
			get { return resourceHolder != null ? resourceHolder : RenderItemResourceHolder.Empty; }
		}
		protected internal void ReleaseResourceHolder() {
			if(resourceHolder != null) {
				resourceHolder.Dispose();
				resourceHolder = null;
			}
		}
		object IRenderItem.UpdateLocker { get { return updateLocker; } }
		bool IRenderItem.Visible {
			get { return RenderItem != null ? RenderItem.Visible : false; }
		}
		bool IRenderItem.CanExport() { return false; }
		void IRenderItem.OnRender() {
		}
		bool IRenderItem.ForceUpdateResourceHolder { get { return forceUpdateResourceHolder; } set { forceUpdateResourceHolder = value; } }
		bool IRenderItem.UseAntiAliasing { get { return true; } }
		void IRenderItem.SetResourceHolder(IRenderer renderer, IRenderItemProvider provider) {
			lock(updateLocker) {
				ReleaseResourceHolder();
				this.resourceHolder = renderer.CreateResourceHolder(provider, this);
				this.forceUpdateResourceHolder = true;
			}
		}
		void IRenderItem.PrepareGeometry() {
			PrepareGeometry();
		}
		#endregion
		#region IStyleOwner implementation
		void IMapStyleOwner.OnStyleChanged() {
			OnStyleChanged();
		}
		#endregion
		protected void OnStyleChanged() {
			ResetStyle();
			InvalidateRender();
		}
		#region IChangedCallbackOwner members
		void IChangedCallbackOwner.SetParentCallback(Action callback) {
			this.callback = callback;
		}
		#endregion
		IRenderShapeTitle GetTitle() {
			return null;
		}
		protected internal void RaiseChanged() {
			if(callback != null)
				callback();
		}
		protected abstract StyleMergerBase CreateStyleMerger();
		protected internal virtual void ResetStyle() {
			if(Layer == null || MapPath == null)
				return;
			lock(updateLocker) {
				MergeStyles();
			}
		}
		protected internal void ApplyStyle(IRenderItemStyle segmentStyle, IShapeRenderItemStyle parentStyle) {
			ActualStyle.Fill = segmentStyle.Fill;
			ActualStyle.Stroke = segmentStyle.Stroke;
			ActualStyle.StrokeWidth = segmentStyle.StrokeWidth;
			ActualStyle.TextColor = parentStyle.TitleColor;
			ActualStyle.TextGlowColor = parentStyle.TitleGlowColor;
		}
		protected abstract void MergeStyles();
		void OnGeometryChanged() {
			forceUpdateResourceHolder = true;
		}
		void PrepareGeometry() {
			if(!GeometryIsValid)
				EnsureGeometry();
		}
		protected internal virtual void ReleaseHitTestGeometry() {
		}
		protected void InvalidateRender() {
			if(Layer != null)
				Layer.InvalidateRender();
		}
		protected abstract IMapItemGeometry CreateGeometry();
		protected internal void UpdateHitGeometryInPool(IHitTestableElement element) {
			if(MapPath != null) MapPath.UpdateHitGeometryInPool(element);
		}
		internal Color GetActualTextColor() {
			return ActualStyle.TextColor;
		}
		internal Color GetActualTextGlowColor() {
			return ActualStyle.TextGlowColor;
		}
		protected virtual IMapItemGeometry GetGeometry() {
			PrepareGeometry();
			return this.geometry;
		}
		protected void EnsureGeometry() {
			if(Layer == null)
				return;
			lock(updateLocker) {
				UpdateBounds();
				this.geometry = CreateGeometry();
				GeometryIsValid = true;
				ReleaseHitTestGeometry();
				OnGeometryChanged();
			}
		}
		protected internal virtual void UpdateBounds() {
		}
		protected internal void UpdateItem() {
			GeometryIsValid = false;
		}
		protected virtual internal void ResetColorizerColor() {			
		}
	}
}
