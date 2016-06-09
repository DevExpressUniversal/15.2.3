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

using DevExpress.Utils;
using DevExpress.XtraMap.Drawing;
using DevExpress.XtraMap.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraMap {
	public abstract class MapPathBase<T> : MapShape, IRenderItemContainer, ISupportSegments  where T : MapSegmentBase {
		MapSegmentCollectionBase<T> segments;
		protected virtual bool RenderItself { get { return false; } }
		protected internal MapSegmentCollectionBase<T> Segments {
			get { return segments; }
			set {
				if (segments == value)
					return;
				((IChangedCallbackOwner)segments).SetParentCallback(null);
				if (value == null)
					value = CreateSegmentCollection();
				segments = value;
				((IChangedCallbackOwner)segments).SetParentCallback(OnSegmentsChanged);
				OnSegmentsChanged();
			}
		}
		protected MapPathBase() {
			this.segments = CreateSegmentCollection();
			((IChangedCallbackOwner)segments).SetParentCallback(OnSegmentsChanged);
		}
		#region IRenderItemContainer implementation
		IEnumerable<IRenderItem> IRenderItemContainer.Items {
			get {
				foreach(MapSegmentBase segment in Segments)
					yield return (IRenderItem)segment;
			}
		}
		bool IRenderItemContainer.RenderItself { get { return RenderItself; } }
		#endregion
		#region ISupportSegments implementation
		MapSegmentBase[] ISupportSegments.Segments {
			get { return segments.ToArray(); }
		}
		#endregion
		void ForEach(Action<MapSegmentBase> action) {
			for (int i = 0; i < Segments.Count; i++) {
				action(Segments[i]);
			}
		}
		void DoUpdateItem(MapSegmentBase segment) {
			segment.UpdateItem();
		}
		protected internal override void ResetColorizerColor() {
			base.ResetColorizerColor();
			ForEach(ResetSegmentColorizerColor);
		}
		void ResetSegmentColorizerColor(MapSegmentBase segment) {
			segment.ResetColorizerColor();
		}
		protected abstract MapSegmentCollectionBase<T> CreateSegmentCollection();
		protected internal virtual void OnSegmentsChanged() {
			UpdateBoundingRect();
			ResetStyle();
			UpdateItem(MapItemUpdateType.Layout);
			Invalidate();
		}
		protected virtual void Invalidate(){
		}
		protected override IMapItemGeometry CreateShapeGeometry() {
			return null;
		}
		protected override MapRect GetHitTestUnitBounds() {
			return MapRect.Empty;
		}
		protected override void ReleaseResourcesInternal() {
			ForEach(delegate(MapSegmentBase segment) {
				segment.ReleaseResourceHolder();
			});
		}
		protected override void UpdateBounds() {
			base.UpdateBounds();
			ForEach(DoUpdateItem);
		}	   
		protected internal override void ResetStyle() {
			base.ResetStyle();
			ForEach(delegate(MapSegmentBase segment) {
				segment.ResetStyle();
			});
		}
		protected internal override void ReleaseHitTestGeometry() {
			ForEach((d) => { d.ReleaseHitTestGeometry(); });
			base.ReleaseHitTestGeometry();
		}
		protected internal override DrawMapItemEventArgs CreateDrawEventArgs() {
			return new DrawMapSegmentableItemEventArgs(this);
		}
		protected override void AfterDrawMapItemEvent(IRenderItemStyle style) {
			DrawMapSegmentableItemEventArgs shapeRenderStyle = style as DrawMapSegmentableItemEventArgs;
			if(shapeRenderStyle != null)
				for(int i = 0; i < Segments.Count; i++)
					segments[i].ApplyStyle(shapeRenderStyle.Segments[i], shapeRenderStyle);
			base.AfterDrawMapItemEvent(style);
		}
	}
}
