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
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region ChartSeriesType
	public enum ChartSeriesType {
		Area,
		Bar,
		Bubble,
		Line,
		Pie,
		Surface,
		Scatter,
		Radar
	}
	#endregion
	#region ISeries
	public interface ISeries : ISupportsCopyFrom<ISeries> {
		int Index { get; set; }
		int Order { get; set; }
		IChartView View { get; }
		IDataReference Arguments { get; set; }
		IDataReference Values { get; set; }
		ChartType ChartType { get; }
		ChartSeriesType SeriesType { get; }
		IChartText Text { get; set; }
		ShapeProperties ShapeProperties { get; }
		bool IsContained { get; }
		ISeries CloneTo(IChartView view);
		string GetSeriesText();
		bool IsCompatible(IChartView view);
		void Visit(ISeriesVisitor visitor);
		void ResetToStyle();
		void OnDataChanged();
		void OnRangeInserting(InsertRangeNotificationContext context);
		void OnRangeRemoving(RemoveRangeNotificationContext context);
		IEnumerable<IDataReference> GetDataReferences();
	}
	#endregion
	#region ISeriesVisitor
	public interface ISeriesVisitor {
		void Visit(AreaSeries series);
		void Visit(BarSeries series);
		void Visit(BubbleSeries series);
		void Visit(LineSeries series);
		void Visit(PieSeries series);
		void Visit(RadarSeries series);
		void Visit(ScatterSeries series);
		void Visit(SurfaceSeries series);
	}
	#endregion
	#region ISupportsInvertIfNegative
	public interface ISupportsInvertIfNegative {
		bool InvertIfNegative { get; set; }
		void SetInvertIfNegativeCore(bool value);
	}
	#endregion
}
