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
	public class ModelLoader {
		readonly ChartControl chart;
		public ModelLoader(ChartControl chart) {
			this.chart = chart;
		}
		Exception MakeUnknownTypeException(object value) {
			if (value == null)
				return new Exception("Unexpected null model value");
			return new Exception("Unknown model type: " + value.GetType().Name);
		}
		string GetDataMemberValue(Dictionary<Model.DataMemberType, string> dataMembers, Model.DataMemberType key) {
			string result = null;
			if(dataMembers.TryGetValue(key, out result))
				return result;
			return null;
		}
		ScaleType GetArgumentScaleType(Model.ArgumentScaleType modelType) {
			switch (modelType) {
				case Model.ArgumentScaleType.Auto:
					return ScaleType.Auto;
				case Model.ArgumentScaleType.DateTime:
					return ScaleType.DateTime;
				case Model.ArgumentScaleType.Numerical:
					return ScaleType.Numerical;
				case Model.ArgumentScaleType.Qualitative:
					return ScaleType.Qualitative;
			}
			throw MakeUnknownTypeException(modelType);
		}
		ScaleType GetValueScaleType(Model.ValueScaleType modelType) {
			switch (modelType) {
				case Model.ValueScaleType.DateTime:
					return ScaleType.DateTime;
				case Model.ValueScaleType.Numerical:
					return ScaleType.Numerical;
			}
			throw MakeUnknownTypeException(modelType);
		}
		Diagram CreateDiagram(Model.Chart chartModel) {
			if (chartModel is Model.CartesianChart)
				return new XYDiagram2D();
			else if (chartModel is Model.PieChart)
				return new SimpleDiagram2D();
			throw MakeUnknownTypeException(chartModel);
		}
		Series CreateSeries(Model.SeriesModel seriesModel) {
			Series series = null;
			if (seriesModel is Model.SideBySideBarSeries) {
				var barSeries = new BarSideBySideSeries2D();
				barSeries.ValueDataMember = GetDataMemberValue(seriesModel.DataMembers, Model.DataMemberType.Value);
				series = barSeries;
			} else if (seriesModel is Model.PieSeries) {
				var pieSeries = new PieSeries2D();
				pieSeries.ValueDataMember = GetDataMemberValue(seriesModel.DataMembers, Model.DataMemberType.Value);
				series = pieSeries;
			}
			if (series == null)
				throw MakeUnknownTypeException(seriesModel);
			series.ArgumentDataMember = GetDataMemberValue(seriesModel.DataMembers, Model.DataMemberType.Argument);
			series.ArgumentScaleType = GetArgumentScaleType(seriesModel.ArgumentScaleType);
			series.ValueScaleType = GetValueScaleType(seriesModel.ValueScaleType);
			return series;
		}
		public void LoadModel(Model.Chart chartModel) {
			var diagram = CreateDiagram(chartModel);
			foreach (var seriesModel in chartModel.Series)
				diagram.Series.Add(CreateSeries(seriesModel));
			chart.Diagram = diagram;
		}
	}
}
