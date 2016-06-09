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
using System;
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.XtraMap.Native {
	public abstract class StyleMergerBase {
		public void Merge(MapItemStyle target) {
			Initialize();
			MergeColorizerStyle(target);
			MergeShapeTitleStyle(target);
			MergeOwnStyle(target);
			MergeParentStyle(target);
			MergeDefaultStyle(target);
			MergeBackgroundStyle(target);
		}
		protected abstract  void MergeColorizerStyle(MapItemStyle target);
		protected abstract void MergeShapeTitleStyle(MapItemStyle target);
		protected abstract void MergeOwnStyle(MapItemStyle target);
		protected abstract void MergeParentStyle(MapItemStyle target);
		protected abstract void MergeDefaultStyle(MapItemStyle target);
		protected abstract void MergeBackgroundStyle(MapItemStyle target);
		protected MapItemColorStyle GetColorizerStyle(IColorizerElement element) {
			return (element != null) ? MapItemStyleCache.Instance.GetColorizerStyle(element.ColorizerColor) : null;
		}
		protected void MergeSegmentStyles(MapSegmentBase segment, MapItemStyle target) {
			MapItemStyleCache cache = MapItemStyleCache.Instance;
			if(!MapUtils.IsColorEmpty(segment.Fill)) {
				MapItemFillStyle fill = cache.GetColorizerStyle(segment.Fill);
				if (fill.IsPriorityStyleColorEmpty(target))
					fill.ApplyColorTo(target);
			}
			if(!MapUtils.IsColorEmpty(segment.Stroke)) {
				MapItemStrokeStyle stroke = cache.GetStrokeStyle(segment.Stroke);
				if(stroke.IsPriorityStyleColorEmpty(target))
					stroke.ApplyColorTo(target);
			}
			if(segment.StrokeWidth > 0 && target.StrokeWidth == -1)
				target.StrokeWidth = segment.StrokeWidth;
		}
		protected virtual void Initialize() {
		}
	}
	public class MapItemStyleMerger : StyleMergerBase {
		readonly MapItem item;
		protected MapItem Item { get { return item; } }
		MapItemStyle highestPriorityStyle;
		public MapItemStyleMerger(MapItem item) { 
			Guard.ArgumentNotNull(item, "item");
			this.item = item;
		}
		protected override void Initialize() {
			if(highestPriorityStyle == null)
				highestPriorityStyle = new MapItemStyle();
			else
				highestPriorityStyle.Reset();
		}
		protected override void MergeShapeTitleStyle(MapItemStyle target) { 
			MapShape shape = Item as MapShape;
			if (shape == null) return;
			ShapeTitleOptions options = shape.TitleOptions;
			MapItemStyleCache cache = MapItemStyleCache.Instance;
			if(!MapUtils.IsColorEmpty(options.TextColor)) {
				MapItemColorStyle colorStyle = cache.GetTextColorStyle(options.TextColor);
				colorStyle.ApplyColorTo(target);
			}
			if(!MapUtils.IsColorEmpty(options.TextGlowColor)) {
				MapItemColorStyle glowStyle = cache.GetTextGlowColorStyle(options.TextGlowColor);
				glowStyle.ApplyColorTo(target);
			}
		}
		protected override void MergeColorizerStyle(MapItemStyle target) {
			if(Item.IsSelected) {
				MapItemStyle.MergeStyles(Item.Layer.SelectedItemStyle, Item.GetDefaultSelectedItemStyle(), highestPriorityStyle);
				if(!Item.IsSelectedStyleEmpty)
					MapItemStyle.MergeStyles(Item.SelectedStyle, highestPriorityStyle, highestPriorityStyle);
			}
			else if(Item.IsHighlighted) {
				MapItemStyle.MergeStyles(Item.Layer.HighlightedItemStyle, Item.GetDefaultHighlightedItemStyle(), highestPriorityStyle);
				 if (!Item.IsHighlightedStyleEmpty)
					 MapItemStyle.MergeStyles(Item.HighlightedStyle, highestPriorityStyle, highestPriorityStyle);
			}
			MapItemStyle.MergeStyles(highestPriorityStyle, highestPriorityStyle, target);
			if(Item.ShouldUseColorizerColor) {
				MapItemColorStyle colorizerStyle = GetColorizerStyle(Item as IColorizerElement);
				if(colorizerStyle != null)
					MapItemStyle.MergeStyles(highestPriorityStyle, colorizerStyle, target);
			}
		}
		protected override void MergeOwnStyle(MapItemStyle target) {
			if (!Item.IsStyleEmpty)
				MapItemStyle.MergeStyles(target, Item.Style, target);
		}
		protected override void MergeParentStyle(MapItemStyle target) {
			MapItemStyle.MergeStyles(target, Item.Layer.ItemStyle, target);
		}
		protected override void MergeDefaultStyle(MapItemStyle target) {
			MapItemStyle.MergeStyles(target, item.GetDefaultItemStyle(), target);
		}
		protected override void MergeBackgroundStyle(MapItemStyle target) {
			if(Item.Layer.BackgroundStyle != null)
				MapItemStyle.MergeStyles(target, Item.Layer.BackgroundStyle, target);
		}
	}
	public class PieSegmentStyleMerger : StyleMergerBase {
		readonly PieSegment segment;
		public PieSegmentStyleMerger(PieSegment segment) {
			Guard.ArgumentNotNull(segment, "segment");
			this.segment = segment;
		}
		protected override void MergeColorizerStyle(MapItemStyle target) {
			if(segment.ShouldUseColorizerColor) {
				MapItemColorStyle colorizerStyle = GetColorizerStyle(segment as IColorizerElement);
				if(colorizerStyle != null)
					colorizerStyle.ApplyColorTo(target);
			}
		}
		protected override void MergeShapeTitleStyle(MapItemStyle target) {
		}
		protected override void MergeOwnStyle(MapItemStyle target) {
			MergeSegmentStyles(segment, target);
		}
		protected override void MergeParentStyle(MapItemStyle target) {
			if(!segment.MapPie.IsStyleEmpty)
				MapItemStyle.MergeStyles(target, segment.MapPie.Style, target);
		}
		protected override void MergeDefaultStyle(MapItemStyle target) {
			MapItemStyle.MergeStyles(target, segment.GetDefaultStyle(), target);
		}
		protected override void MergeBackgroundStyle(MapItemStyle target) {
		}
	}
	public class MapPathSegmentStyleMerger : StyleMergerBase {
		readonly MapPathSegment segment;
		protected MapPathSegment Segment { get { return segment; } }
		protected MapItem Path { get { return Segment.MapPath; } }
		MapItemStyle highestPriorityStyle;
		public MapPathSegmentStyleMerger(MapPathSegment segment) {
			Guard.ArgumentNotNull(segment, "segment");
			this.segment = segment;
		}
		protected override void Initialize() {
			if(highestPriorityStyle == null)
				highestPriorityStyle = new MapItemStyle();
			else
				highestPriorityStyle.Reset();
		}
		protected override void MergeColorizerStyle(MapItemStyle target) {
			if(Path.IsSelected) {
				MapItemStyle.MergeStyles(Path.Layer.SelectedItemStyle, Path.GetDefaultSelectedItemStyle(), highestPriorityStyle);
				if(!Path.IsSelectedStyleEmpty)
					MapItemStyle.MergeStyles(Path.SelectedStyle, highestPriorityStyle, highestPriorityStyle);
			}
			else if(Path.IsHighlighted) {
				MapItemStyle.MergeStyles(Path.Layer.HighlightedItemStyle, Path.GetDefaultHighlightedItemStyle(), highestPriorityStyle);
				if(!Path.IsHighlightedStyleEmpty)
					MapItemStyle.MergeStyles(Path.HighlightedStyle, highestPriorityStyle, highestPriorityStyle);
			}
			MapItemStyle.MergeStyles(highestPriorityStyle, highestPriorityStyle, target);
			if(Path.ShouldUseColorizerColor) {
				MapItemColorStyle colorizerStyle = GetColorizerStyle(Path as IColorizerElement);
				if(colorizerStyle != null)
					MapItemStyle.MergeStyles(highestPriorityStyle, colorizerStyle, target);
			}
		}
		protected override void MergeShapeTitleStyle(MapItemStyle target) {
		}
		protected override void MergeOwnStyle(MapItemStyle target) {
			MergeSegmentStyles(segment, target);
		}
		protected override void MergeParentStyle(MapItemStyle target) {
			MapItemStyle.MergeStyles(target, Path.Style, target);
			MapItemStyle.MergeStyles(target, Path.Layer.ItemStyle, target);
		}
		protected override void MergeDefaultStyle(MapItemStyle target) {
			MapItemStyle.MergeStyles(target, Segment.DefaultStyle, target);
		}
		protected override void MergeBackgroundStyle(MapItemStyle target) {
			if(Path.Layer.BackgroundStyle != null)
				MapItemStyle.MergeStyles(target, Path.Layer.BackgroundStyle, target);
		}
	}
}
