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
namespace DevExpress.Charts.Native {
	public interface ISupportMinMaxValues {
		double Max { get; }
		double Min { get; }
		double GetAbsMinValue();
	}
	public class SeriesComparerByActiveIndex : Comparer<RefinedSeries> {
		public override int Compare(RefinedSeries series1, RefinedSeries series2) {
			if (series1 == null || series2 == null)
				throw new ArgumentNullException();
			return series1.ActiveIndex.CompareTo(series2.ActiveIndex);
		}
	}
	public class InteractionComparerByArgument : Comparer<IPointInteraction> {
		public override int Compare(IPointInteraction interaction1, IPointInteraction interaction2) {
			if (interaction2 == null || interaction1 == null)
				throw new ArgumentNullException();
			return interaction1.Argument.CompareTo(interaction2.Argument);
		}
	}
	public class InteractionComparerByMinValue : Comparer<IPointInteraction> {
		readonly ISeriesView seriesView;
		public InteractionComparerByMinValue(ISeriesView seriesView) {
			this.seriesView = seriesView;
		}
		public override int Compare(IPointInteraction interaction1, IPointInteraction interaction2) {
			if (interaction2 == null || interaction1 == null)
				throw new ArgumentNullException();
			double value1 = interaction1.GetMinValue(seriesView);
			double value2 = interaction2.GetMinValue(seriesView);
			return value1.CompareTo(value2);
		}
	}
	public class InteractionComparerByMaxValue : Comparer<IPointInteraction> {
		readonly ISeriesView seriesView;
		public InteractionComparerByMaxValue(ISeriesView seriesView) {
			this.seriesView = seriesView;
		}
		public override int Compare(IPointInteraction interaction1, IPointInteraction interaction2) {
			if (interaction2 == null || interaction1 == null)
				throw new ArgumentNullException();
			double value1 = interaction1.GetMaxValue(seriesView);
			double value2 = interaction2.GetMaxValue(seriesView);
			return value2.CompareTo(value1);
		}
	}
	public class InteractionComparerByMinAbsValue : Comparer<IPointInteraction> {
		readonly ISeriesView seriesView;
		public InteractionComparerByMinAbsValue(ISeriesView seriesView) {
			this.seriesView = seriesView;
		}
		public override int Compare(IPointInteraction interaction1, IPointInteraction interaction2) {
			if (interaction2 == null || interaction1 == null)
				throw new ArgumentNullException();
			double value1 = interaction1.GetMinAbsValue(seriesView);
			double value2 = interaction2.GetMinAbsValue(seriesView);
			return value1.CompareTo(value2);
		}
	}
	public class StackedGroupKey : List<RefinedSeries> {
		public object GroupKey { get; private set; }
		public StackedGroupKey(object groupKey) {
			GroupKey = groupKey;
		}
	}
	public interface IPointInteraction {
		int Count { get; }
		double Argument { get; }
		bool IsEmpty { get; }
		double GetMinValue(ISeriesView seriesView);
		double GetMaxValue(ISeriesView seriesView);
		double GetMinAbsValue(ISeriesView seriesView);
	}
}
