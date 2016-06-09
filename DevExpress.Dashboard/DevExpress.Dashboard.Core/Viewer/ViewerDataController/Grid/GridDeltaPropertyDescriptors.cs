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
using DevExpress.DashboardCommon.Native;
using System.Collections.Generic;
namespace DevExpress.DashboardCommon.Viewer {
	public class GridDeltaMDDataPropertyDescriptor : ReadOnlyPropertyDescriptor {
		readonly MultiDimensionalData data;
		readonly DeltaDescriptor descriptor;
		public bool Initialized { get { return descriptor != null; } }
		protected GridDeltaMDDataPropertyDescriptor(MultiDimensionalData data, string name)
			: this(data, name, name) {
		}
		public GridDeltaMDDataPropertyDescriptor(MultiDimensionalData data, string deltaId, string name)
			: base(name) {
			this.data = data;
			descriptor = data.GetDeltaDescriptorById(deltaId);
		}
		public override object GetValue(object component) {
			return GetValueInternal(component);
		}
		public override Type PropertyType {
			get { return typeof(DeltaValue); }
		}
		protected MeasureValue GetMeasureValue(object component, DeltaValueType deltaType) {
			DeltaValue deltaValue = GetValueInternal(component);
			if(deltaValue != null) {
				switch(deltaType) {
					case DeltaValueType.AbsoluteVariation:
						return deltaValue.AbsoluteVariation;
					case DeltaValueType.PercentVariation:
						return deltaValue.PercentVariation;
					case DeltaValueType.PercentOfTarget:
						return deltaValue.PercentOfTarget;
					case DeltaValueType.ActualValue:
					default:
						return deltaValue.ActualValue;
				}
			}
			return null;
		}
		DeltaValue GetValueInternal(object component) {
			AxisPoint point = component as AxisPoint;
			return point != null && descriptor != null ? data.GetDeltaValue(point, descriptor) : null;
		}
	}
	public class GridDeltaValueMDDataPropertyDescriptor : GridDeltaMDDataPropertyDescriptor {
		readonly DeltaValueType deltaType;
		readonly Type propertyType;
		public override Type PropertyType {
			get { return propertyType; }
		}
		public GridDeltaValueMDDataPropertyDescriptor(MultiDimensionalData data, string deltaId, DeltaValueType deltaType, DataFieldType dataFieldType, IEnumerable<AxisPoint> rows)
			: base(data, deltaId) {
			this.deltaType = deltaType;
			if(dataFieldType == DataFieldType.Unknown) {
				propertyType = typeof(object);
				if(rows != null) {
					bool flag = false;
					IEnumerator<AxisPoint> enumerator = rows.GetEnumerator();
					while(!flag && enumerator.MoveNext()) {
						MeasureValue val = GetMeasureValue(enumerator.Current, deltaType);
						if(val != null && val.Value != null) {
							propertyType = GridMeasureValueMDDataPropertyDescriptor.MakeNullable(val.Value.GetType());
							flag = true;
						}
					}
				}
			} else
				propertyType = dataFieldType.ToType();
		}
		public override object GetValue(object component) {
			MeasureValue value = GetMeasureValue(component, deltaType);
			return value != null ? value.Value : null;
		}
	}
	public class GridDeltaDisplayTextMDDataPropertyDescriptor : GridDeltaMDDataPropertyDescriptor {
		readonly DeltaValueType deltaType;
		public override Type PropertyType {
			get { return typeof(string); }
		}
		public GridDeltaDisplayTextMDDataPropertyDescriptor(MultiDimensionalData data, string deltaId, string name, DeltaValueType deltaType)
			: base(data, deltaId, name) {
			this.deltaType = deltaType;
		}
		public override object GetValue(object component) {
			MeasureValue value = GetMeasureValue(component, deltaType);
			return value != null ? value.DisplayText : null;
		}
	}
}
