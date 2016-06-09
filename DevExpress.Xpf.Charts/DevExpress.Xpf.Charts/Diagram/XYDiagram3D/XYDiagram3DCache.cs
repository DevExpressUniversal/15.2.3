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

using System.Windows.Media.Media3D;
using System.Collections.Generic;
using DevExpress.Charts.Native;
using System.Windows.Media;
namespace DevExpress.Xpf.Charts.Native {
	public class XYDiagram3DCache {
		class XYDiagram3DCacheItem {
			readonly Model3D[] models;
			public Model3D[] Models { get { return models; } }
			public XYDiagram3DCacheItem(params Model3D[] models) {
				this.models = models;
			}
		}
		readonly Dictionary<int, AxisCache> axesCache = new Dictionary<int, AxisCache>();
		readonly Dictionary<object, XYDiagram3DCacheItem> modelItems = new Dictionary<object, XYDiagram3DCacheItem>();
		readonly MaterialBrushCache materialBrushes = new MaterialBrushCache();
		Model3D bottom;
		Model3D back;
		Model3D fore;
		Model3D left;
		Model3D right;
		Model3DGroup seriesModel;
		public Model3D Bottom { 
			get { return bottom; } 
			set { bottom = value; } 
		}
		public Model3D Back { 
			get { return back; } 
			set { back = value; } 
		}
		public Model3D Fore { 
			get { return fore; } 
			set { fore = value; } 
		}
		public Model3D Left { 
			get { return left; } 
			set { left = value; } 
		}
		public Model3D Right { 
			get { return right; } 
			set { right = value; } 
		}
		public Model3DGroup SeriesModel {
			get { return seriesModel; }
			set { seriesModel = value; }
		}
		public bool IsDiagramModelValid { get { return bottom != null && back != null && fore != null && left != null && right != null; } }
		public AxisCache GetAxisCache(Axis axis) {
			AxisCache axisCache;
			int hashCode = axis.GetHashCode();
			if (!axesCache.TryGetValue(hashCode, out axisCache)) {
				axisCache = new AxisCache();
				axesCache.Add(hashCode, axisCache);
			}
			return axisCache;
		}
		public void Clear() {
			foreach (AxisCache cache in axesCache.Values)
				cache.Clear();
			axesCache.Clear();
			bottom = null;
			back = null;
			fore = null;
			left = null;
			right = null;
			seriesModel = null;
			modelItems.Clear();
			materialBrushes.Clear();
		}
		public void AddModels(object modelHolder, params Model3D[] models) {
			modelItems.Add(modelHolder, new XYDiagram3DCacheItem(models));
		}
		public void ChangeModelBrush(object modelHolder, SolidColorBrush brush) {
			XYDiagram3DCacheItem cacheItem;
			modelItems.TryGetValue(modelHolder, out cacheItem);
			if (cacheItem != null)
				foreach (Model3D model in cacheItem.Models) {
					ColorUtils.MixBrushes(model, brush, materialBrushes);
				}
		}
	}
}
