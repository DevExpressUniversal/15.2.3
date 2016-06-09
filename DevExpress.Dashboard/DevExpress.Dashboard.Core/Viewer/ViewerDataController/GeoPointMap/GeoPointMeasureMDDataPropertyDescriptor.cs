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
namespace DevExpress.DashboardCommon.Viewer {
	public abstract class GeoPointMeasureMDDataPropertyDescriptorBase : ReadOnlyPropertyDescriptor { 
		readonly MultiDimensionalData data;
		readonly MeasureDescriptor descriptor;
		protected GeoPointMeasureMDDataPropertyDescriptorBase(MultiDimensionalData data, string measureId, string name)
			: base(name) {
			this.data = data;
			descriptor = data.GetMeasureDescriptorByID(measureId);
		}
		public override Type PropertyType {
			get { return typeof(object); }
		}
		protected MeasureValue GetMeasureValue(object component) {
			AxisPoint point = component as AxisPoint;
			return point != null ? data.GetValue(point, descriptor) : null;
		}
	}
	public class GeoPointValueMDDataPropertyDescriptor : GeoPointMeasureMDDataPropertyDescriptorBase {
		public GeoPointValueMDDataPropertyDescriptor(MultiDimensionalData data, string measureId)
			: base(data, measureId, measureId) {
		}
		public override object GetValue(object component) {
			MeasureValue measure = GetMeasureValue(component);
			return measure != null ? measure.Value : null;
		}
	}
	public class GeoPointDisplayTextMDDataPropertyDescriptor : GeoPointMeasureMDDataPropertyDescriptorBase {
		public GeoPointDisplayTextMDDataPropertyDescriptor(MultiDimensionalData data, string measureId, string name)
			: base(data, measureId, name) {
		}
		public override object GetValue(object component) {
			MeasureValue measure = GetMeasureValue(component);
			return measure != null ? measure.DisplayText : null;
		}
	}
}
