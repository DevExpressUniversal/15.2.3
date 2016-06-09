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

using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DataAccess.Native.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
namespace DevExpress.DashboardCommon.Viewer {
#if DEBUG
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1039")]
#endif
	public abstract class ChartMultiDimensionalDataSourceBase : ReadOnlyTypedList {
		readonly MultiDimensionalData data;
		readonly IList<AxisPoint> argumentPoints;
		readonly int argumentThreshold;
		readonly ChartColorMultiDimensionalDataPropertyDescriptor colorDescriptor;
		protected MultiDimensionalData Data { get { return data; } }
		protected ChartMultiDimensionalDataSourceBase(MultiDimensionalData data, int argumentThreshold) {
			this.argumentThreshold = argumentThreshold;
			this.data = data;
		}
		protected ChartMultiDimensionalDataSourceBase(MultiDimensionalData data, string argumentName, int argumentThreshold)
			: this(data, argumentThreshold) {
			argumentPoints = GetArgumentAxisPoints(argumentName);
			Properties.Add(new ChartSortingPropertyDescriptor());
		}
		protected ChartMultiDimensionalDataSourceBase(MultiDimensionalData data, string argumentName, string[] measureIDs, int argumentThreshold, ChartSeriesPointColorHelper colorHelper)
			: this(data, argumentThreshold) {
			argumentPoints = GetArgumentAxisPoints(argumentName);
			foreach(string measureID in measureIDs)
				Properties.Add(new ChartValueMultiDimensionalDataPropertyDescriptor(measureID, data));
			colorDescriptor = new ChartColorMultiDimensionalDataPropertyDescriptor(colorHelper);
			Properties.Add(colorDescriptor);
		}
		public Color GetFirstArgumentColor() {
			if(colorDescriptor != null) {
				foreach(AxisPoint axisPoint in argumentPoints) {
					object colorValue = colorDescriptor.GetValue(axisPoint);
					if(colorValue != null)
						return Color.FromArgb(Convert.ToInt32(colorValue));
				}
			}
			return Color.Empty;
		}
		protected override object GetItemValue(int index) {
			return argumentPoints[index];
		}
		protected override int GetItemsCount() {
			return argumentPoints.Count;
		}
		IList<AxisPoint> GetArgumentAxisPoints(string argumentName) {
			AxisPoint axisRoot = data.GetAxisRoot(DashboardDataAxisNames.ChartArgumentAxis);
			DimensionDescriptor summaryArgument = data.GetDimensionDescriptorByID(DashboardDataAxisNames.ChartArgumentAxis, argumentName);
			if(summaryArgument == null)
				return new List<AxisPoint>() { axisRoot };
			IList<AxisPoint> allPoints = axisRoot.GetAxisPointsByDimension(summaryArgument, true);
			return argumentThreshold > 0 ?
				allPoints.Take(argumentThreshold).ToList() :
				allPoints;
		}
	}
#if DEBUG
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1039")]
#endif
	public class ChartMultiDimensionalDataSource : ChartMultiDimensionalDataSourceBase {
		public ChartMultiDimensionalDataSource(MultiDimensionalData data, int argumentThreshold)
			: base(data, argumentThreshold) {
		}
		public ChartMultiDimensionalDataSource(MultiDimensionalData data, string argumentName, Type argumentType, int argumentThreshold)
			: base(data, argumentName, argumentThreshold) {
			Properties.Add(new ChartArgumentMultiDimensionalDataPropertyDescriptor(argumentName, argumentType));
		}
		public ChartMultiDimensionalDataSource(MultiDimensionalData data, string argumentName, Type argumentType, string[] measureIDs, int argumentThreshold, ChartSeriesPointColorHelper colorHelper)
			: base(data, argumentName, measureIDs, argumentThreshold, colorHelper) {
			Properties.Add(new ChartArgumentMultiDimensionalDataPropertyDescriptor(argumentName, argumentType));
		}
	}
#if DEBUG
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1039")]
#endif
	public class ScatterChartMultiDimensionalDataSource : ChartMultiDimensionalDataSourceBase {
		public ScatterChartMultiDimensionalDataSource(MultiDimensionalData data, string argumentName, string argumentMeasureID, int argumentThreshold)
			: base(data, argumentName, argumentThreshold) {
			Properties.Add(new ChartValueMultiDimensionalDataPropertyDescriptor(argumentName ?? ChartArgumentMultiDimensionalDataPropertyDescriptor.EmptyArgumentMember, argumentMeasureID, data));
		}
		public ScatterChartMultiDimensionalDataSource(MultiDimensionalData data, string argumentName, string argumentMeasureID, string[] measureIDs, int argumentThreshold, ChartSeriesPointColorHelper colorHelper)
			: base(data, argumentName, measureIDs, argumentThreshold, colorHelper) {
			Properties.Add(new ChartValueMultiDimensionalDataPropertyDescriptor(argumentName ?? ChartArgumentMultiDimensionalDataPropertyDescriptor.EmptyArgumentMember, argumentMeasureID, data));
		}
	}
	public class ChartArgumentMultiDimensionalDataPropertyDescriptor : ReadOnlyPropertyDescriptor {
		public const string EmptyArgumentMember = "Argument";
		readonly Type argumentType;
		public override Type PropertyType { get { return argumentType; } }
		public ChartArgumentMultiDimensionalDataPropertyDescriptor(string name, Type argumentType)
			: base(name ?? EmptyArgumentMember) {
			this.argumentType = argumentType;
		}
		protected ChartArgumentMultiDimensionalDataPropertyDescriptor(Type argumentType)
			: this(EmptyArgumentMember, argumentType) {
		}
		public override object GetValue(object component) {
			AxisPoint point = component as AxisPoint;
			if(point != null && point.Parent != null)
				return argumentType == typeof(string) ? point.Index : point.UniqueValue;
			return DashboardLocalizer.GetString(DashboardStringId.ChartTotalValue);
		}
	}
	public class ChartValueMultiDimensionalDataPropertyDescriptor : ReadOnlyPropertyDescriptor {
		readonly MultiDimensionalData data;
		readonly MeasureDescriptor measureDescriptor;
		public ChartValueMultiDimensionalDataPropertyDescriptor(string name, MultiDimensionalData data)
			: this(name, name, data) {
		}
		public ChartValueMultiDimensionalDataPropertyDescriptor(string name, string measureId, MultiDimensionalData data)
			: base(name) {
			this.data = data;
			this.measureDescriptor = data.GetMeasureDescriptorByID(measureId);
		}
		public override object GetValue(object component) {
			AxisPoint point = component as AxisPoint;
			if(point != null && measureDescriptor != null) {
				object value = data.GetSlice(point).GetValue(measureDescriptor).Value;
				if(!DashboardSpecialValues.IsErrorValue(value))
					return value;
			}
			return null;
		}
		public override Type PropertyType {
			get { return typeof(decimal); }
		}
	}
	public class ChartColorMultiDimensionalDataPropertyDescriptor : ReadOnlyPropertyDescriptor {
		public const string ColorMember = "Color";
		readonly ChartSeriesPointColorHelper colorHelper;
		public override Type PropertyType { get { return typeof(int); } }
		protected ChartSeriesPointColorHelper ColorHelper { get { return colorHelper; } }
		public ChartColorMultiDimensionalDataPropertyDescriptor(ChartSeriesPointColorHelper colorHelper)
			: base(ColorMember) {
			this.colorHelper = colorHelper;
		}
		public override object GetValue(object component) {
			AxisPoint point = component as AxisPoint;
			if(point != null) {
				return ColorHelper.GetColorValue(point);
			}
			return null;
		}
	}
	public class ChartSortingPropertyDescriptor : ReadOnlyPropertyDescriptor {
		public const string PropertyName = "SortingDataMember";
		public ChartSortingPropertyDescriptor()
			: base(PropertyName) {
		}
		public override object GetValue(object component) {
			return null;
		}
		public override Type PropertyType {
			get { return typeof(decimal); }
		}
	}
}
