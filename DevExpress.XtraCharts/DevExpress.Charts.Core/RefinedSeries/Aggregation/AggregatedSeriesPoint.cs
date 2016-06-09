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
	public class AggregatedSeriesPoint : ISeriesPoint {
		readonly Scale argumentScaleType;
		readonly DateTime[] dateTimeValues;
		readonly RefinedSeries owner;
		readonly IList<RefinedPoint> sourcePoints;
		readonly double[] values;
		string stringArgument;
		double doubleArgument;
		DateTime dateTimeArgument;
		object argument;
		bool isEmpty = true;
		bool isAuxiliary;
		public ISeries Series { get { return owner != null ? owner.Series : null; } }
		public object Argument { get { return argument; } set { argument = value; } }
		public bool IsEmpty { get { return isEmpty; } }
		public double[] Values { get { return values; } }
		public DateTime[] DateTimeValues { get { return dateTimeValues; } }
		public IList<RefinedPoint> SourcePoints { get { return sourcePoints; } }
		public bool IsAuxiliary { get { return isAuxiliary; } }
		#region ISeriesPoint
		Scale ISeriesPoint.ArgumentScaleType {
			get { return argumentScaleType; }
		}
		object ISeriesPoint.UserArgument {
			get { return argument; }
		}
		string ISeriesPoint.QualitativeArgument {
			get { return stringArgument; }
		}
		double ISeriesPoint.NumericalArgument {
			get { return doubleArgument; }
		}
		DateTime ISeriesPoint.DateTimeArgument {
			get { return dateTimeArgument; }
		}
		double[] ISeriesPoint.UserValues {
			get { return values; }
		}
		double[] ISeriesPoint.AnimatedValues {
			get { return values; }
		}
		DateTime[] ISeriesPoint.DateTimeValues {
			get { return dateTimeValues; }
		}
		double ISeriesPoint.InternalArgument {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		double[] ISeriesPoint.InternalValues {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		object ISeriesPoint.ToolTipHint {
			get { return sourcePoints != null && sourcePoints.Count > 0 ? sourcePoints[0].SeriesPoint.ToolTipHint : string.Empty; } 
		}
		bool ISeriesPoint.IsEmpty(Scale scale) {
			return IsEmpty;
		}
		#endregion       
		AggregatedSeriesPoint(RefinedSeries owner, Scale argumentScaleType, object argument, IList<RefinedPoint> sourcePoints, bool isEmpty, bool isAuxiliary) {
			this.owner = owner;
			this.sourcePoints = sourcePoints;
			this.argumentScaleType = argumentScaleType;
			this.argument = argument;
			this.isEmpty = isEmpty;
			this.isAuxiliary = isAuxiliary;
			InitializeArguments();			
		}
		public AggregatedSeriesPoint(RefinedSeries owner, Scale argumentScaleType, object argument, double[] values, IList<RefinedPoint> sourcePoints, bool isEmpty, bool isAuxiliary) 
			: this(owner, argumentScaleType, argument, sourcePoints, isEmpty, isAuxiliary) {
			if (sourcePoints != null && sourcePoints.Count > 0) {
				int count = sourcePoints[0].SeriesPoint.UserValues.Length;
				this.values = new double[count];
				for (int i = 0; i < count; i++)
					this.values[i] = values[i];
			} else
				this.values = values;
		}
		public AggregatedSeriesPoint(RefinedSeries owner, Scale argumentScaleType, object argument, DateTime[] values, double[] userValues, IList<RefinedPoint> sourcePoints, bool isEmpty) : 
			this(owner, argumentScaleType, argument, sourcePoints, isEmpty, false) {
			this.values = userValues;
			if (sourcePoints != null && sourcePoints.Count > 0) {
				int count = sourcePoints[0].SeriesPoint.UserValues.Length;
				dateTimeValues = new DateTime[count];
				for (int i = 0; i < count; i++)
					dateTimeValues[i] = values[i];
			} else
				dateTimeValues = values;
		}
		void InitializeArguments() {
			switch (argumentScaleType) {
				case Scale.Qualitative:
					stringArgument = argument != null ? argument.ToString() : string.Empty;
					break;
				case Scale.Numerical:
					doubleArgument = DataItemsHelper.ConvertToDouble(argument);
					break;
				case Scale.DateTime:
					dateTimeArgument = DataItemsHelper.ConvertToDateTime(argument);
					break;
				case Scale.Auto:
					break;
				default:
					break;
			}
		}
	}
}
