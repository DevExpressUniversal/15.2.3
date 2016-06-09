#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Diagnostics.CodeAnalysis;
using DevExpress.XtraCharts;
namespace DevExpress.ExpressApp.Chart {
	public static class ChartControlWithEmptyArgumentsHelper {
		public static string ConvertArgument(object argument) {
			string stringArgument = argument == null ? String.Empty : argument.ToString();
			return String.IsNullOrEmpty(stringArgument) ? "Empty" : stringArgument;
		}
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters")]
		public static SeriesPoint[] CalcMin(Series series, object argument, string[] functionArguments, DataSourceValues[] values, object[] colors) {
			argument = ConvertArgument(argument);
			if(series.ValueScaleType == ScaleType.DateTime) {
				DateTime? min = null;
				foreach(DataSourceValues item in values) {
					object value = item[functionArguments[0]];
					if(value != DBNull.Value && value != null) {
						string str = value.ToString();
						if(!String.IsNullOrEmpty(str)) {
							DateTime data = Convert.ToDateTime(str);
							if(min == null || DateTime.Compare(data, min.Value) < 0)
								min = data;
						}
					}
				}
				return new SeriesPoint[] { min == null ? new SeriesPoint(argument) : new SeriesPoint(argument, min) };
			}
			else {
				double? min = null;
				foreach(DataSourceValues item in values) {
					object value = item[functionArguments[0]];
					if(value != DBNull.Value && value != null) {
						string str = value.ToString();
						if(!String.IsNullOrEmpty(str)) {
							double data = Convert.ToDouble(str);
							if(min == null || data < min.Value)
								min = data;
						}
					}
				}
				return new SeriesPoint[] { min == null ? new SeriesPoint(argument) : new SeriesPoint(argument, min) };
			}
		}
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters")]
		public static SeriesPoint[] CalcMax(Series series, object argument, string[] functionArguments, DataSourceValues[] values, object[] colors) {
			argument = ConvertArgument(argument);
			if(series.ValueScaleType == ScaleType.DateTime) {
				DateTime? max = null;
				foreach(DataSourceValues item in values) {
					object value = item[functionArguments[0]];
					if(value != DBNull.Value && value != null) {
						string str = value.ToString();
						if(!String.IsNullOrEmpty(str)) {
							DateTime data = Convert.ToDateTime(str);
							if(max == null || DateTime.Compare(data, max.Value) > 0)
								max = data;
						}
					}
				}
				return new SeriesPoint[] { max == null ? new SeriesPoint(argument) : new SeriesPoint(argument, max) };
			}
			else {
				double? max = null;
				foreach(DataSourceValues item in values) {
					object value = item[functionArguments[0]];
					if(value != DBNull.Value && value != null) {
						string str = value.ToString();
						if(!String.IsNullOrEmpty(str)) {
							double data = Convert.ToDouble(str);
							if(max == null || data > max.Value)
								max = data;
						}
					}
				}
				return new SeriesPoint[] { max == null ? new SeriesPoint(argument) : new SeriesPoint(argument, max) };
			}
		}
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters")]
		public static SeriesPoint[] CalcSum(Series series, object argument, string[] functionArguments, DataSourceValues[] values, object[] colors) {
			argument = ConvertArgument(argument);
			double sum = 0.0;
			foreach(DataSourceValues item in values) {
				object value = item[functionArguments[0]];
				if(value != DBNull.Value && value != null) {
					string str = value.ToString();
					if(!String.IsNullOrEmpty(str))
						sum += Convert.ToDouble(str);
				}
			}
			return new SeriesPoint[] { new SeriesPoint(argument, sum) };
		}
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters")]
		public static SeriesPoint[] CalcAverage(Series series, object argument, string[] functionArguments, DataSourceValues[] values, object[] colors) {
			argument = ConvertArgument(argument);
			double sum = 0.0;
			int count = 0;
			foreach(DataSourceValues item in values) {
				object value = item[functionArguments[0]];
				if(value != DBNull.Value && value != null) {
					string str = value.ToString();
					if(!String.IsNullOrEmpty(str)) {
						sum += Convert.ToDouble(str);
						count++;
					}
				}
			}
			return new SeriesPoint[] { count == 0 ? new SeriesPoint(argument) : new SeriesPoint(argument, sum / count) };
		}
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters")]
		public static SeriesPoint[] CalcCount(Series series, object argument, string[] functionArguments, DataSourceValues[] values, object[] colors) {
			argument = ConvertArgument(argument);
			return new SeriesPoint[] { new SeriesPoint(argument, values.Length) };
		}
	}
}
