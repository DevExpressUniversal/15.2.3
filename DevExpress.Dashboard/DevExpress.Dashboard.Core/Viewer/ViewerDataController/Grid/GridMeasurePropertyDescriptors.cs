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
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DataAccess.Native.Data;
using DevExpress.Utils;
using System.Collections.Generic;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardCommon.Viewer {
	public abstract class GridMeasureMDDataPropertyDescriptorBase : ReadOnlyPropertyDescriptor {
		readonly MultiDimensionalData data;
		readonly MeasureDescriptor descriptor;
		Type propertyType;
		protected Type PropertyTypeCore { 
			get { return propertyType; }
			set { propertyType = value; }
		}
		protected GridMeasureMDDataPropertyDescriptorBase(MultiDimensionalData data, string measureId, string name, Type propertyType)
			: base(name) {
			this.data = data;
			descriptor = data.GetMeasureDescriptorByID(measureId);
			this.propertyType = propertyType;
		}
		public override Type PropertyType {
			get { return propertyType; }
		}
		protected MeasureValue GetMeasureValue(object component) {
			AxisPoint point = component as AxisPoint;
			return point != null && descriptor != null ? data.GetValue(point, descriptor) : null;
		}
	}
	public class GridMeasureValueMDDataPropertyDescriptor : GridMeasureMDDataPropertyDescriptorBase {
		public static Type MakeNullable(Type dataType) {
			if(dataType.IsClass() || Nullable.GetUnderlyingType(dataType) != null || dataType.IsInterface())
				return dataType;
			return typeof(Nullable<>).MakeGenericType(dataType);
		}
		public GridMeasureValueMDDataPropertyDescriptor(MultiDimensionalData data, string measureId, DataFieldType dataFieldType, IEnumerable<AxisPoint> rows)
			: base(data, measureId, measureId, typeof(object)) {
			Type dataType;
			if(dataFieldType == DataFieldType.Unknown) {
				dataType = typeof(object);
				if(rows != null) {
					bool flag = false;
					IEnumerator<AxisPoint> enumerator = rows.GetEnumerator();
					while(!flag && enumerator.MoveNext()) {
						MeasureValue val = GetMeasureValue(enumerator.Current);
						if(val != null && val.Value != null) {
							dataType = MakeNullable(val.Value.GetType());
							flag = true;
						}
					}
				}
			} else
				dataType = dataFieldType.ToType();
			PropertyTypeCore = dataType;
		}
		public override object GetValue(object component) {
			MeasureValue value = GetMeasureValue(component);
			return value != null ? value.Value : null;
		}
	}
	public class GridMeasureDisplayTextMDDataPropertyDescriptor : GridMeasureMDDataPropertyDescriptorBase {
		public GridMeasureDisplayTextMDDataPropertyDescriptor(MultiDimensionalData data, string measureId, string name)
			: base(data, measureId, name, typeof(string)) {
		}
		public override object GetValue(object component) {
			MeasureValue value = GetMeasureValue(component);
			return value != null ? value.DisplayText : null;
		}
	}
	public class GridMeasureUniqueNameMDDataPropertyDescriptor : GridMeasureMDDataPropertyDescriptorBase {
		public GridMeasureUniqueNameMDDataPropertyDescriptor(MultiDimensionalData data, string measureId, string name, Type propertyType)
			: base(data, measureId, name, propertyType) {
		}
		public override object GetValue(object component) {
			MeasureValue value = GetMeasureValue(component);
			return value != null ? value.Value : null;
		}
	}
}
