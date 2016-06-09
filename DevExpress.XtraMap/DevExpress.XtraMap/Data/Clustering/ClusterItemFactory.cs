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
using System;
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.XtraMap {
	public interface IClusterItemFactory  {
		MapItem CreateClusterItem(IList<MapItem> objects);
	}
	public class DefaultClusterItemFactory : IClusterItemFactory {
		static readonly IClusterItemFactory instance = new DefaultClusterItemFactory();
		public static IClusterItemFactory Instance { get { return instance; } }
		MapItem IClusterItemFactory.CreateClusterItem(IList<MapItem> objects) {
			return CreateClusterItem(objects);
		}
		protected MapItem CreateClusterItem(IList<MapItem> objects) {
			MapItem item = CreateItemInstance(objects);
			InitializeItem(item, objects);
			return item;
		}
		protected virtual MapItem CreateItemInstance(IList<MapItem> objects) {
			if(objects != null && objects.Count == 0)
				return null;
			IClusterable item = objects[0] as IClusterable;
			return item != null ? item.CreateInstance() as MapItem : new MapCustomElement();
		}
		protected virtual void InitializeItem(MapItem cluster, IList<MapItem> sourceObj) {
		}
	}
}
namespace DevExpress.XtraMap.Native {
	public class ClusterTypeMismatchException : Exception {
	}
	public class MapClusterFactoryAdapter : IClusterItemFactoryCore {
		IClusterItemFactory WinClusterFactory { get; set; }
		public MapClusterFactoryAdapter(IClusterItemFactory winClusterFactory) {
			WinClusterFactory = winClusterFactory;
		}
		public IClusterItem CreateClusterItem(IList<IClusterable> objects) {
			if(objects != null) {
				IClusterItem clusterItem = WinClusterFactory.CreateClusterItem(objects.OfType<MapItem>().ToList()) as IClusterItem;
				if (clusterItem == null)
					throw new ClusterTypeMismatchException();
				else
					return clusterItem;
			}
			return null;
		}
	}
}
