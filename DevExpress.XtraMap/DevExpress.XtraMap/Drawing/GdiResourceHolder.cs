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
using System.Drawing;
using System.Linq;
using DevExpress.Utils;
using DevExpress.Map.Native;
using DevExpress.XtraMap.Native;
namespace DevExpress.XtraMap.Drawing {
	public class ScreenGeometryResourceHolder : MapDisposableObject, IRenderItemResourceHolder {
		readonly IRenderItem owner;
		readonly IRenderItemProvider provider;
		IGeometry screenGeometry;
		IImageGeometry imageGeometry = null;
		IList<MapPoint[]> points = new List<MapPoint[]>();
		protected IMapItemGeometry Geometry { get { return owner.Geometry; } }
		protected IRenderItemProvider Provider { get { return provider; } }
		public IGeometry ScreenGeometry { get { return screenGeometry; } }
		public Image Image { get { return imageGeometry != null ? imageGeometry.Image : null; } }
		public IList<MapPoint[]> Points { get { return points; } }
		public ScreenGeometryResourceHolder(IRenderItemProvider provider, IRenderItem owner) {
			Guard.ArgumentNotNull(owner, "owner");
			Guard.ArgumentNotNull(provider, "provider");
			this.owner = owner;
			this.provider = provider;
		}
		public void Initialize() {
			screenGeometry = CalculateScreenGeometry();
			this.points = screenGeometry.Points;
			imageGeometry = Geometry as IImageGeometry;
		}
		protected override void DisposeOverride() {
			base.DisposeOverride();
			screenGeometry = null;
			this.points.Clear();
		}
		IGeometry CalculateScreenGeometry() {
			IList<MapPoint[]> contours = new List<MapPoint[]>();
			IUnitGeometry unitGeometry = Geometry as UnitGeometry;
			MultiLineUnitGeometry lineGeometry = Geometry as MultiLineUnitGeometry;
			PathUnitGeometry pathGeometry = Geometry as PathUnitGeometry;
			   if(pathGeometry != null) {
				   contours.Add(provider.GeometryToScreenPoints(pathGeometry.Points));
				   foreach(MapUnit[] contour in pathGeometry.InnerContours)
					   contours.Add(provider.GeometryToScreenPoints(contour));
			   } else if(lineGeometry != null) {
				   foreach(MapUnit[] segment in lineGeometry.Segments)
					   contours.Add(provider.GeometryToScreenPoints(segment));
			   } else if(unitGeometry != null) {
				   contours.Add(provider.GeometryToScreenPoints(unitGeometry.Points));
			   }
			   return CreteGeometry(contours, unitGeometry); 
		}
		IGeometry CreteGeometry(IList<MapPoint[]> contours, IUnitGeometry unitGeometry) {
			IGeometry geometry = new PolygonGeometry(new List<MapPoint[]>());
			if(unitGeometry != null && contours.Count > 0) {
				if(unitGeometry.Type == UnitGeometryType.Areal)
					geometry = new PolygonGeometry(contours);
				else if(unitGeometry.Type == UnitGeometryType.Linear)
					geometry = new PolylineGeometry(contours);
			}
			return geometry;
		}
		void IRenderItemResourceHolder.Update() {
			Initialize();
		}
	}
}
