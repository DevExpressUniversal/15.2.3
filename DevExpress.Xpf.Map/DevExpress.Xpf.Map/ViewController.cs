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
using System.Windows.Input;
using System.Collections.Generic;
using DevExpress.Map.Native;
namespace DevExpress.Xpf.Map.Native {
	public class MapViewController {
		MapControl map;
		HitTestController hitTestController;
		protected MapControl Map { get { return map; } }
		protected ToolTipInfo ToolTipInfo { get { return Map.ToolTipInfo; } }
		public MapViewController(MapControl map) {
			this.map = map;
			this.hitTestController = new HitTestController(Map);
		}
		void UpdateToolTip(MapItem mapItem, Point hitPoint) {
			HideToolTip();
			if(mapItem != null && mapItem.Layer != null && mapItem.Layer.ActualToolTipEnable) {
				ToolTipInfo.ToolTipText = CreateToolTipText(mapItem);
				ToolTipInfo.Item = mapItem.Info.Source;
				ToolTipInfo.ContentTemplate = mapItem.Layer.ToolTipContentTemplate;
				ToolTipInfo.GeoPoint = mapItem.Layer.ScreenToGeoPoint(hitPoint, true);
				ToolTipInfo.Layer = mapItem.Layer;
				ToolTipInfo.Visibility = Visibility.Visible;
				ToolTipInfo.UpdatePosition();
			}
		}
		internal void UpdateToolTipPosition() {
			ToolTipInfo.UpdatePosition();
		}
		internal string CreateToolTipText(MapItem mapItem) {
			ToolTipPatternParser parser = mapItem.CreateToolTipPatternParser();
			parser.AddContextRange(mapItem.ActualAttributes);
			return parser.GetText();
		}
		void UpdateSelection(MapItem mapItem, ModifierKeys keyModifiers) {
			if(Map.SelectionMode != ElementSelectionMode.None) {
				VectorLayerBase layer = ((IOwnedElement)mapItem).Owner as VectorLayerBase;
				if(layer != null && layer.EnableSelection)
					layer.UpdateItemSelection(Map.SelectionMode, keyModifiers, mapItem.Info);
			}
		}
		internal void UpdateToolTipAndSelection(Point hitPoint, ModifierKeys modifiers) {
			HideToolTip();
			List<IHitTestableElement> items = hitTestController.FindElements(hitPoint);
			IHitTestableElement item = items.Find((element) => { return (element is MapItemPresenter) && element.IsHitTestVisible; });
			if(item != null) {
				MapItem mapItem = (MapItem)item.Element;
				UpdateSelection(mapItem, modifiers);
				UpdateToolTip(mapItem, hitPoint);
			} else {
				if(modifiers == ModifierKeys.None)
					ClearAllSelectedItems();
			}
		}
		internal void HideToolTip() {
			ToolTipInfo.Visibility = Visibility.Collapsed;
		}
		internal void SelectItemsByRegion(Rect region) {
			HideToolTip();
			ClearAllSelectedItems();
			List<IHitTestableElement> items = hitTestController.FindElemensByRegion(region);
			foreach(IHitTestableElement element in items) {
				MapItem mapItem = element.Element as MapItem;
				if(mapItem != null)
					UpdateSelection(mapItem, ModifierKeys.Shift);
			}
		}
		internal void ClearAllSelectedItems() {
			foreach(LayerBase item in map.Layers) {
				VectorLayerBase layer = item as VectorLayerBase;
				if(layer != null && layer.Visibility == Visibility.Visible && layer.AllowResetSelection)
					layer.SelectionController.ClearSelectedItems();
			}
		}
	}
}
