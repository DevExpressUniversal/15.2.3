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
	public partial class SeriesConfigurator {
		readonly Dictionary<Type, Type> modelSeriesMapping3D = new Dictionary<Type, Type>();
		void FillModelSeriesMapping() {
			FillModelSeriesMapping2D();
			FillModelSeriesMapping3D();
		}
		void FillSeriesConfiguratorsMapping() {
			FillSeriesConfiguratorsMapping2D();
			FillSeriesConfiguratorsMapping3D();
		}
		void FillModelSeriesMapping3D() {
			modelSeriesMapping3D.Add(typeof(Model.SideBySideBarSeries), typeof(BarSideBySideSeries3D));
			modelSeriesMapping3D.Add(typeof(Model.ManhattanBarSeries), typeof(BarSeries3D));
			modelSeriesMapping3D.Add(typeof(Model.PointSeries), typeof(PointSeries3D));
			modelSeriesMapping3D.Add(typeof(Model.BubbleSeries), typeof(BubbleSeries3D));
			modelSeriesMapping3D.Add(typeof(Model.AreaSeries), typeof(AreaSeries3D));
			modelSeriesMapping3D.Add(typeof(Model.StackedAreaSeries), typeof(AreaStackedSeries3D));
			modelSeriesMapping3D.Add(typeof(Model.FullStackedAreaSeries), typeof(AreaFullStackedSeries3D));
			modelSeriesMapping3D.Add(typeof(Model.PieSeries), typeof(PieSeries3D));
			modelSeriesMapping3D.Add(typeof(Model.DonutSeries), typeof(PieSeries3D));
		}
		void FillSeriesConfiguratorsMapping3D() {
			seriesConfiguratorsMapping.Add(typeof(BarSeries3D), new SeriesPropertiesConfiguratorBase[] { new BarSeries3DPropertiesConfigurator(), new ColorEachSeriesPropertiesConfigurator() });
			seriesConfiguratorsMapping.Add(typeof(BarSideBySideSeries3D), new SeriesPropertiesConfiguratorBase[] { new BarSeries3DPropertiesConfigurator(), new ColorEachSeriesPropertiesConfigurator() });
			seriesConfiguratorsMapping.Add(typeof(PointSeries3D), new SeriesPropertiesConfiguratorBase[] { new MarkerSeries3DPropertiesConfigurator(), new ColorEachSeriesPropertiesConfigurator() });
			seriesConfiguratorsMapping.Add(typeof(BubbleSeries3D), new SeriesPropertiesConfiguratorBase[] { new MarkerSeries3DPropertiesConfigurator(), new ColorEachSeriesPropertiesConfigurator() });
			seriesConfiguratorsMapping.Add(typeof(AreaSeries3D), new SeriesPropertiesConfiguratorBase[] { new GeneralSeriesPropertiesConfigurator() });
			seriesConfiguratorsMapping.Add(typeof(AreaStackedSeries3D), new SeriesPropertiesConfiguratorBase[] { new GeneralSeriesPropertiesConfigurator() });
			seriesConfiguratorsMapping.Add(typeof(AreaFullStackedSeries3D), new SeriesPropertiesConfiguratorBase[] { new GeneralSeriesPropertiesConfigurator() });
			seriesConfiguratorsMapping.Add(typeof(PieSeries3D), new SeriesPropertiesConfiguratorBase[] { new PieSeries3DPropertiesConfigurator() });
		}
		public Series CreateSeries(Model.SeriesModel seriesModel, bool is3D) {
			Type seriesType;
			if (is3D) {
				if (modelSeriesMapping3D.TryGetValue(seriesModel.GetType(), out seriesType))
					return (Series)Activator.CreateInstance(seriesType);
			}
			else
				if (modelSeriesMapping2D.TryGetValue(seriesModel.GetType(), out seriesType))
					return (Series)Activator.CreateInstance(seriesType);
			return null;
		}
	}
}
