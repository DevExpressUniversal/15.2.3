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
using System.Linq;
using System.Text;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.XtraMap.Drawing;
using System.Drawing;
namespace DevExpress.XtraMap.Native {
	public interface IMapEventHandler {
		void OnLayersChanged(CollectionChangedEventArgs<LayerBase> e);
		void OnLayerChanged(MapItemsLayerBase layer);
		void OnLayerSupportObjectChanged();
		void OnLegendsChanged(CollectionChangedEventArgs<MapLegendBase> e);
		void OnLegendChanged(MapLegendBase legend);
		void OnAppearanceChanged();
		void OnMiniMapChanged();
		void OnOptionsChanged(BaseOptionChangedEventArgs e);
	}
	public class MapEventHandler : IMapEventHandler {
		readonly InnerMap map;
		public MapEventHandler(InnerMap map) {
			Guard.ArgumentNotNull(map, "map");
			this.map = map;
		}
		public InnerMap Map { get { return map; } }
		#region IMapEventHandler Members
		void IMapEventHandler.OnAppearanceChanged() {
			Map.Render();
		}
		void IMapEventHandler.OnMiniMapChanged() {
			Map.Render();
		}
		void IMapEventHandler.OnOptionsChanged(BaseOptionChangedEventArgs e) {
			InvalidateViewInfo();
		}
		void IMapEventHandler.OnLayerChanged(MapItemsLayerBase layer) {
			Map.OperationHelper.ResetEnableHighlighting();
			Map.OperationHelper.ResetEnableSelection();
			UpdateSelection(layer);
			UpdateRenderItems(layer);
		}
		void IMapEventHandler.OnLayersChanged(CollectionChangedEventArgs<LayerBase> e) {
			Map.RenderController.BeginUpdate();
			try {
				UpdateRenderItems(null);
				Map.OperationHelper.ResetErrorPanelVisibility();
				Map.UpdateNavigationPanel(Point.Empty, false);
				Map.InteractionController.UpdateSearchPanel();
				Map.RenderController.RegisterUpdate(UpdateActionType.Render);
			} finally {
				Map.RenderController.EndUpdate();
			}
		}
		void IMapEventHandler.OnLayerSupportObjectChanged() {
			Map.InteractionController.UpdateSearchPanel();
			Map.RenderController.UpdateSearchPanel();
		}
		void IMapEventHandler.OnLegendsChanged(CollectionChangedEventArgs<MapLegendBase> e) {
			InvalidateViewInfo();
		}
		void IMapEventHandler.OnLegendChanged(MapLegendBase legend) {
			InvalidateViewInfo();
		}
		#endregion
		void UpdateSelection(MapItemsLayerBase layer) {
			if (layer.DataItems.Count() == 0) {
				layer.SelectedItems.Clear();
			}
		}
		void UpdateRenderItems(LayerBase updatedLayer) {
			if (!Map.IsDesignMode)
				Map.UpdateLayerRenderItems(updatedLayer);
		}
		void InvalidateViewInfo() {
			Map.InvalidateViewInfo();
		}
	}
}
