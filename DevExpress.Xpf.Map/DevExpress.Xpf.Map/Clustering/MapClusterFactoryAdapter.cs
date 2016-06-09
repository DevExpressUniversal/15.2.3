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

using DevExpress.Map.Native;
using DevExpress.Xpf.Map;
using DevExpress.Xpf.Map.Native;
using System;
using System.Collections.Generic;
namespace DevExpress.Xpf.Native {
	public class ClusterTypeMismatchException : Exception {
	}
	public class MapClusterFactoryAdapter : IClusterItemFactoryCore {
		MapItemSettingsBase ClusterSettings { get; set; }
		public MapClusterFactoryAdapter(MapItemSettingsBase clusterSettings) {
			ClusterSettings = clusterSettings;
		}
		public IClusterItem CreateClusterItem(IList<IClusterable> objects) {
			if(objects == null || objects.Count == 0)
				return null;
			MapItem item = objects[0] as MapItem;
			IClusterItem result = null;
			if(item != null) {
				if(ClusterSettings != null){
					  MapItem cluster = ClusterSettings.CreateItem();
					  ClusterSettings.ApplySource(cluster, new MapItemInfo(cluster));
					  result = cluster as IClusterItem;
				}
				else{
					IClusterable clusterItem = objects[0] as IClusterable;
					result = clusterItem != null ? clusterItem.CreateInstance() : new MapPushpin() as IClusterItem;
				}
				result.ClusteredItems = objects;
				if(result == null)
					throw new ClusterTypeMismatchException();
				else
					return result;
			}
			return null;
		}
	}
}
