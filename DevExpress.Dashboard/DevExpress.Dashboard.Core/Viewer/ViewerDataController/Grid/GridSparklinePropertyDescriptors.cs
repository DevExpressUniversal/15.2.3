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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DataAccess.Native.Data;
namespace DevExpress.DashboardCommon.Viewer {
	public abstract class GridSparklineValuesPropertyDescriptorBase : ReadOnlyPropertyDescriptor {
		readonly MultiDimensionalData data;
		readonly IList<AxisPoint> sparklineAxisPoints;
		readonly MeasureDescriptor descriptor;
		protected GridSparklineValuesPropertyDescriptorBase(MultiDimensionalData data, string sparklineId, string name, string axisName)
			: base(name) {
			this.data = data;
			sparklineAxisPoints = !String.IsNullOrEmpty(axisName) ? data.GetAxisPoints(axisName) : null;
			descriptor = data.GetMeasureDescriptorByID(sparklineId);
		}
		public override Type PropertyType {
			get { return typeof(object); }
		}
		protected IList<MeasureValue> GetMeasureValues(object component) {
			AxisPoint point = component as AxisPoint;
			if(point != null) {
				IList<MeasureValue> res = new List<MeasureValue>();
				if(sparklineAxisPoints != null) {
					for(int i = 0; i < sparklineAxisPoints.Count; i++)
						res.Add(data.GetValue(point, sparklineAxisPoints[i], descriptor));
				}
				else
					res.Add(data.GetValue(point, descriptor));
				return res;
			}
			return null;
		}
		protected string GetDisplayText(MeasureValue value) {
			return value.Value != null ? value.DisplayText : descriptor.Format(0);
		}
	}
	public class GridSparklineValuesPropertyDescriptor : GridSparklineValuesPropertyDescriptorBase {
		public GridSparklineValuesPropertyDescriptor(MultiDimensionalData data, string sparklineId, string axisName)
			: base(data, sparklineId, sparklineId, axisName) {
		}
		public override object GetValue(object component) {
			IList<MeasureValue> values = GetMeasureValues(component);
			return values != null ? values.Select(val => Helper.ConvertToDouble(val.Value)).ToList() : null;
		}
	}
	public class GridSparklineUniqueNamePropertyDescriptor : GridSparklineValuesPropertyDescriptorBase {
		public GridSparklineUniqueNamePropertyDescriptor(MultiDimensionalData data, string sparklineId, string name, string axisName)
			: base(data, sparklineId, name, axisName) {
		}
		public override object GetValue(object component) {
			IList<MeasureValue> values = GetMeasureValues(component);
			return values != null ? values.Select(val => Helper.ConvertToDouble(val.Value)).ToList() : null;
		}
	}
	public abstract class GridSparklinePropertyDescriptor : ReadOnlyPropertyDescriptor {
		readonly MultiDimensionalData data;
		readonly AxisPoint sparklineAxisPoint;
		readonly MeasureDescriptor descriptor;
		protected MultiDimensionalData Data { get { return data; } }
		protected GridSparklinePropertyDescriptor(MultiDimensionalData data, string sparklineId, string name)
			: base(name) {
			this.data = data;
			sparklineAxisPoint = GetSparklineAxisPoint();
			descriptor = data.GetMeasureDescriptorByID(sparklineId);
		}
		protected GridSparklinePropertyDescriptor(MultiDimensionalData data, string sparklineId)
			: base(sparklineId) {
		}
		public override Type PropertyType {
			get { return typeof(object); }
		}
		protected abstract AxisPoint GetSparklineAxisPoint();
		protected object GetValueInternal(object component) {
			MeasureValue value = GetMeasureValue(component);
			return value != null ? value.Value : null;
		}
		protected string GetDisplayText(object component) {
			MeasureValue value = GetMeasureValue(component);
			if(value == null)
				return null;
			if(value.Value == null)
				return descriptor.Format(0);
			return value.DisplayText;
		}
		MeasureValue GetMeasureValue(object component) {
			AxisPoint point = component as AxisPoint;
			return point != null ? data.GetValue(point, sparklineAxisPoint, descriptor) : null;
		}
	}
	public abstract class GridSparklineStartPropertyDescriptor : GridSparklinePropertyDescriptor {
		protected GridSparklineStartPropertyDescriptor(MultiDimensionalData data, string sparklineId, string name)
			: base(data, sparklineId, name) {
		}
		protected override AxisPoint GetSparklineAxisPoint() {
			return Data.Axes.ContainsKey(DashboardDataAxisNames.SparklineAxis) ? Data.GetAxisPoints(DashboardDataAxisNames.SparklineAxis).FirstOrDefault() : null;
		}
	}
	public class GridSparklineStartValuePropertyDescriptor : GridSparklineStartPropertyDescriptor {
		public GridSparklineStartValuePropertyDescriptor(MultiDimensionalData data, string sparklineId, string name)
			: base(data, sparklineId, name) {
		}
		public override object GetValue(object component) {
			return GetValueInternal(component);
		}
	}
	public class GridSparklineStartDisplayTextPropertyDescriptor : GridSparklineStartPropertyDescriptor {
		public GridSparklineStartDisplayTextPropertyDescriptor(MultiDimensionalData data, string sparklineId, string name)
			: base(data, sparklineId, name) {
		}
		public override object GetValue(object component) {
			return GetDisplayText(component);
		}
	}
	public abstract class GridSparklineEndPropertyDescriptor : GridSparklinePropertyDescriptor {
		protected GridSparklineEndPropertyDescriptor(MultiDimensionalData data, string sparklineId, string name)
			: base(data, sparklineId, name) {
		}
		protected override AxisPoint GetSparklineAxisPoint() {
			return Data.Axes.ContainsKey(DashboardDataAxisNames.SparklineAxis) ? Data.GetAxisPoints(DashboardDataAxisNames.SparklineAxis).LastOrDefault() : null;
		}
	}
	public class GridSparklineEndValuePropertyDescriptor : GridSparklineEndPropertyDescriptor {
		public GridSparklineEndValuePropertyDescriptor(MultiDimensionalData data, string sparklineId, string name)
			: base(data, sparklineId, name) {
		}
		public override object GetValue(object component) {
			return GetValueInternal(component);
		}
	}
	public class GridSparklineEndDisplayTextPropertyDescriptor : GridSparklineEndPropertyDescriptor {
		public GridSparklineEndDisplayTextPropertyDescriptor(MultiDimensionalData data, string sparklineId, string name)
			: base(data, sparklineId, name) {
		}
		public override object GetValue(object component) {
			return GetDisplayText(component);
		}
	}
	public class GridSparklineMinDisplayTextPropertyDescriptor : GridSparklineValuesPropertyDescriptorBase {
		public GridSparklineMinDisplayTextPropertyDescriptor(MultiDimensionalData data, string sparklineId, string name, string axisName)
			: base(data, sparklineId, name, axisName) {
		}
		public override object GetValue(object component) {
			IList<MeasureValue> values = GetMeasureValues(component);
			string res = null;
			if(values != null) {
				double min = Double.MaxValue;
				foreach(MeasureValue value in values) {
					double current = Helper.ConvertToDouble(value.Value);
					if(current < min) {
						min = current;
						res = GetDisplayText(value);
					}
				}
			}
			return res;
		}
	}
	public class GridSparklineMaxDisplayTextPropertyDescriptor : GridSparklineValuesPropertyDescriptorBase {
		public GridSparklineMaxDisplayTextPropertyDescriptor(MultiDimensionalData data, string sparklineId, string name, string axisName)
			: base(data, sparklineId, name, axisName) {
		}
		public override object GetValue(object component) {
			IList<MeasureValue> values = GetMeasureValues(component);
			string res = null;
			if(values != null) {
				double max = Double.MinValue;
				foreach(MeasureValue value in values) {
					double current = Helper.ConvertToDouble(value.Value);
					if(current > max) {
						max = current;
						res = GetDisplayText(value);
					}
				}
			}
			return res;
		}
	}
}
