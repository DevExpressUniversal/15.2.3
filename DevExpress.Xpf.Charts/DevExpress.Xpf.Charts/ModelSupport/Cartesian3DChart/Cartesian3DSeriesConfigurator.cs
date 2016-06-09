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
using System.Diagnostics;
using System.Reflection;
using DevExpress.Utils;
using Model = DevExpress.Charts.Model;
using DevExpress.Charts.Native;
namespace DevExpress.Xpf.Charts.ModelSupport {
	public class BarSeries3DPropertiesConfigurator : GeneralSeriesPropertiesConfigurator {
		public override void Configure(Series series, Model.SeriesModel seriesModel, Diagram diagram) {
			base.Configure(series, seriesModel, diagram);
			BarSeries3D barSeries = (BarSeries3D)series;
			barSeries.BarWidth = ((Model.ISupportBarWidthSeries)seriesModel).BarWidth;
			switch (((Model.ISupportBar3DModelSeries)seriesModel).Model) {
				case Model.Bar3DModel.Box:
					barSeries.Model = new BoxBar3DModel();
					break;
				case Model.Bar3DModel.Cone:
					barSeries.Model = new ConeBar3DModel();
					break;
				case Model.Bar3DModel.Cylinder:
					barSeries.Model = new CylinderBar3DModel();
					break;
				case Model.Bar3DModel.Pyramid:
					barSeries.Model = new PyramidBar3DModel();
					break;
				default:
					barSeries.Model = new BoxBar3DModel();
					break;
			}
		}
	}
	public class MarkerSeries3DPropertiesConfigurator : GeneralSeriesPropertiesConfigurator {
		new Marker3DModel GetMarkerModel(Model.MarkerType markerType) {
			switch (markerType) {
				case Model.MarkerType.Circle:
					return new SphereMarker3DModel();
				case Model.MarkerType.Cross:
					return new SphereMarker3DModel();
				case Model.MarkerType.Diamond:
					return new SphereMarker3DModel();
				case Model.MarkerType.Hexagon:
					return new HexagonMarker3DModel();
				case Model.MarkerType.InvertedTriangle:
					return new PyramidMarker3DModel();
				case Model.MarkerType.Pentagon:
					return new SphereMarker3DModel();
				case Model.MarkerType.Plus:
					return new SphereMarker3DModel();
				case Model.MarkerType.Square:
					return new CubeMarker3DModel();
				case Model.MarkerType.Star:
					return new StarMarker3DModel();
				case Model.MarkerType.Triangle:
					return new PyramidMarker3DModel();
				default:
					return new SphereMarker3DModel();
			}
		}
		public override void Configure(Series series, Model.SeriesModel seriesModel, Diagram diagram) {
			base.Configure(series, seriesModel, diagram);
			Model.Marker markerModel = ((Model.ISupportMarkerSeries)seriesModel).Marker;
			if (markerModel != null) {
				MarkerSeries3D markerSeries = (MarkerSeries3D)series;
				markerSeries.Model = GetMarkerModel(markerModel.MarkerType);
				if (series is PointSeries3D)
					((PointSeries3D)series).MarkerSize = markerModel.Size;
			}
		}
	}
}
