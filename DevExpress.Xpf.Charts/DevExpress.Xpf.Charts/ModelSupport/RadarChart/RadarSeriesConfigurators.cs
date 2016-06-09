﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
namespace DevExpress.Xpf.Charts.ModelSupport {
	public class CircularSeriesPropertiesConfigurator : GeneralSeriesPropertiesConfigurator {
		public override void Configure(Series series, Model.SeriesModel seriesModel, Diagram diagram) {
			base.Configure(series, seriesModel, diagram);
			Model.Marker markerModel = ((Model.ISupportMarkerSeries)seriesModel).Marker;
			if (markerModel != null) {
				CircularSeries2D circularSeries = (CircularSeries2D)series;
				circularSeries.MarkerSize = markerModel.Size;
				circularSeries.MarkerModel = GetMarkerModel(markerModel.MarkerType);
			}
		}
	}
	public class CircularLineSeriesPropertiesConfigurator : CircularSeriesPropertiesConfigurator {
		public override void Configure(Series series, Model.SeriesModel seriesModel, Diagram diagram) {
			base.Configure(series, seriesModel, diagram);
			Model.Marker markerModel = ((Model.ISupportMarkerSeries)seriesModel).Marker;
			if (markerModel != null) {
				((CircularLineSeries2D)series).MarkerVisible = markerModel.Visible;
			}
		}
	}
	public class CircularAreaSeriesPropertiesConfigurator : CircularSeriesPropertiesConfigurator {
		public override void Configure(Series series, Model.SeriesModel seriesModel, Diagram diagram) {
			base.Configure(series, seriesModel, diagram);
			Model.Marker markerModel = ((Model.ISupportMarkerSeries)seriesModel).Marker;
			if (markerModel != null) {
				((CircularAreaSeries2D)series).MarkerVisible = markerModel.Visible;
			}
		}
	}
}
