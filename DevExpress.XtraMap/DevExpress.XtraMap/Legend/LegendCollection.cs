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
using DevExpress.XtraMap.Native;
using System.Collections.Generic;
namespace DevExpress.XtraMap {
	public class LegendCollection : OwnedCollection<MapLegendBase>, ISupportSwapItems {
		protected InnerMap Map { get { return (InnerMap)Owner; } }
		protected internal LegendCollection(InnerMap map)
			: base(map) {
		}
		#region ISupportSwapItems members
		void ISupportSwapItems.Swap(int index1, int index2) {
			if (index1 == index2)
				return;
			MapLegendBase swapLegend = InnerList[index1];
			InnerList[index1] = InnerList[index2];
			InnerList[index2] = swapLegend;
		}
		#endregion
		internal void DetachLayer(LayerBase layerToDetach) {
			foreach(MapLegendBase legend in this) {
				ItemsLayerLegend layerLegend = legend as ItemsLayerLegend;
				if(layerLegend != null) {
					if(layerToDetach == null || Object.Equals(layerToDetach, layerLegend.Layer)) 
						layerLegend.Layer = null;
				}
			}
		}
#if DEBUGTEST
		internal IList<MapLegendBase> GetLegendsByLayer(LayerBase layer) {
			List<MapLegendBase> result = new List<MapLegendBase>();
			foreach(MapLegendBase legend in this) {
				ItemsLayerLegend mapLegend = legend as ItemsLayerLegend;
				if(mapLegend != null &&  object.Equals(mapLegend.Layer, layer))
					result.Add(mapLegend);
			}
			return result;
		}
#endif
	}
}
