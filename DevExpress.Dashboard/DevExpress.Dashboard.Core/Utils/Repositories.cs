#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using DevExpress.DashboardCommon.Localization;
namespace DevExpress.DashboardCommon.Native {
	public class SeriesTypeRepository<T> {
		readonly Dictionary<T, string> dictionary = new Dictionary<T, string>();
		public void Register(T seriesType, string caption) {
			if (String.IsNullOrEmpty(caption))
				throw new ArgumentException("The caption can't be empty");
			if (!(seriesType is Enum))
				throw new ArgumentException("The seriesType must be enum");
			if (dictionary.ContainsKey(seriesType))
				throw new ArgumentException("The repository already contains the " + seriesType);
			if (dictionary.ContainsValue(caption))
				throw new ArgumentException("The repository already contains the caption" + caption);
			dictionary.Add(seriesType, caption);
		}
		public string GetCaption(T seriesType) {
			if(!dictionary.ContainsKey(seriesType))
				throw new ArgumentException("Unknown seriesType");
			return dictionary[seriesType];
		}
	}
	public static class OpenHighLowCloseSeriesTypeRepository {
		static readonly SeriesTypeRepository<OpenHighLowCloseSeriesType> repository = new SeriesTypeRepository<OpenHighLowCloseSeriesType>();
		static OpenHighLowCloseSeriesTypeRepository() {
			Register(OpenHighLowCloseSeriesType.CandleStick, DashboardLocalizer.GetString(DashboardStringId.SeriesTypeCandleStick));
			Register(OpenHighLowCloseSeriesType.Stock, DashboardLocalizer.GetString(DashboardStringId.SeriesTypeStock));
		}
		public static void Register(OpenHighLowCloseSeriesType seriesType, string caption) {
			repository.Register(seriesType, caption);
		}
		public static string GetCaption(OpenHighLowCloseSeriesType seriesType) {
			return repository.GetCaption(seriesType);
		}
	}
	public static class RangeSeriesTypeRepository {
		static readonly SeriesTypeRepository<RangeSeriesType> repository = new SeriesTypeRepository<RangeSeriesType>();
		static RangeSeriesTypeRepository() {
			Register(RangeSeriesType.SideBySideRangeBar, DashboardLocalizer.GetString(DashboardStringId.SeriesTypeSideBySideRangeBar));
			Register(RangeSeriesType.RangeArea, DashboardLocalizer.GetString(DashboardStringId.SeriesTypeRangeArea));
		}
		public static void Register(RangeSeriesType seriesType, string caption) {
			repository.Register(seriesType, caption);
		}
		public static string GetCaption(RangeSeriesType seriesType) {
			return repository.GetCaption(seriesType);
		}
	}
	public static class SimpleSeriesTypeRepository {
		static readonly SeriesTypeRepository<SimpleSeriesType> repository = new SeriesTypeRepository<SimpleSeriesType>();
		static SimpleSeriesTypeRepository() {
			Register(SimpleSeriesType.Bar, DashboardLocalizer.GetString(DashboardStringId.SeriesTypeBar));
			Register(SimpleSeriesType.StackedBar, DashboardLocalizer.GetString(DashboardStringId.SeriesTypeStackedBar));
			Register(SimpleSeriesType.FullStackedBar, DashboardLocalizer.GetString(DashboardStringId.SeriesTypeFullStackedBar));
			Register(SimpleSeriesType.Point, DashboardLocalizer.GetString(DashboardStringId.SeriesTypePoint));
			Register(SimpleSeriesType.Line, DashboardLocalizer.GetString(DashboardStringId.SeriesTypeLine));
			Register(SimpleSeriesType.StackedLine, DashboardLocalizer.GetString(DashboardStringId.SeriesTypeStackedLine));
			Register(SimpleSeriesType.FullStackedLine, DashboardLocalizer.GetString(DashboardStringId.SeriesTypeFullStackedLine));
			Register(SimpleSeriesType.StepLine, DashboardLocalizer.GetString(DashboardStringId.SeriesTypeStepLine));
			Register(SimpleSeriesType.Spline, DashboardLocalizer.GetString(DashboardStringId.SeriesTypeSpline));
			Register(SimpleSeriesType.Area, DashboardLocalizer.GetString(DashboardStringId.SeriesTypeArea));
			Register(SimpleSeriesType.StackedArea, DashboardLocalizer.GetString(DashboardStringId.SeriesTypeStackedArea));
			Register(SimpleSeriesType.FullStackedArea, DashboardLocalizer.GetString(DashboardStringId.SeriesTypeFullStackedArea));
			Register(SimpleSeriesType.StepArea, DashboardLocalizer.GetString(DashboardStringId.SeriesTypeStepArea));
			Register(SimpleSeriesType.SplineArea, DashboardLocalizer.GetString(DashboardStringId.SeriesTypeSplineArea));
			Register(SimpleSeriesType.StackedSplineArea, DashboardLocalizer.GetString(DashboardStringId.SeriesTypeStackedSplineArea));
			Register(SimpleSeriesType.FullStackedSplineArea, DashboardLocalizer.GetString(DashboardStringId.SeriesTypeFullStackedSplineArea));
		}
		public static void Register(SimpleSeriesType seriesType, string caption) {
			repository.Register(seriesType, caption);
		}
		public static string GetCaption(SimpleSeriesType seriesType) {
			return repository.GetCaption(seriesType);
		}
	}
}
