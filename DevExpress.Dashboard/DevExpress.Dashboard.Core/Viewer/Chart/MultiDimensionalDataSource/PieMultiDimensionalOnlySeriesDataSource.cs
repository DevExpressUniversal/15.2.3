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

using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DataAccess.Native.Data;
using System;
namespace DevExpress.DashboardCommon.Viewer {
#if DEBUG
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1039")]
#endif
	public class PieMultiDimensionalOnlySeriesDataSource : ChartMultiDimensionalDataSource {
		readonly string[] measureIds;
		public PieMultiDimensionalOnlySeriesDataSource(MultiDimensionalData data, string[] measureIds, int argumentThreshold, ChartSeriesPointColorHelper colorHelper)
			: base(data, argumentThreshold) {
			this.measureIds = measureIds;
			Properties.Add(new PieValuesAsArgumentsPropertyDescriptor(data, measureIds));
			Properties.Add(new PieValuePropertyDescriptor(data, measureIds));
			Properties.Add(new PieValuesAsArgumentsColorPropertyDescriptor(colorHelper, data.GetAxisRoot(DashboardDataAxisNames.ChartArgumentAxis)));
		}
		protected override object GetItemValue(int index) {
			return index;
		}
		protected override int GetItemsCount() {
			return measureIds.Length;
		}
	}
	public class PieValuesAsArgumentsPropertyDescriptor : ChartArgumentMultiDimensionalDataPropertyDescriptor {
		readonly MultiDimensionalData data;
		readonly string[] measureIds;
		public PieValuesAsArgumentsPropertyDescriptor(MultiDimensionalData data, string[] measureIds)
			: base(typeof(string)) {
			this.data = data;
			this.measureIds = measureIds;
		}
		public override object GetValue(object component) {
			if(component != null) {
				int index = (int)component;
				return data.GetMeasureDescriptorByID(measureIds[index]).Name;
			}
			return null;
		}
	}
	public class PieValuePropertyDescriptor : ReadOnlyPropertyDescriptor {
		public const string ValueDataMember = "Value";
		readonly MultiDimensionalData data;
		readonly string[] measureIds;
		public PieValuePropertyDescriptor(MultiDimensionalData data, string[] measureIds)
			: base(ValueDataMember) {
			this.data = data;
			this.measureIds = measureIds;
		}
		public override object GetValue(object component) {
			if(component != null) {
				int index = (int)component;
				MeasureDescriptor descriptor = data.GetMeasureDescriptorByID(measureIds[index]);
				object value = data.GetValue(descriptor).Value;
				if(!DashboardSpecialValues.IsErrorValue(value))
					return value;
			}
			return null;
		}
		public override Type PropertyType {
			get { return typeof(decimal); }
		}
	}
	public class PieValuesAsArgumentsColorPropertyDescriptor : ChartColorMultiDimensionalDataPropertyDescriptor {
		readonly AxisPoint argumentRoot;
		public override Type PropertyType { get { return typeof(int); } }
		public PieValuesAsArgumentsColorPropertyDescriptor(ChartSeriesPointColorHelper colorHelper, AxisPoint argumentRoot)
			: base(colorHelper) {
			this.argumentRoot = argumentRoot;
		}
		public override object GetValue(object component) {
			if(component != null) {
				int index = (int)component;
				return ColorHelper.GetColorValue(index, argumentRoot);
			}
			return null;
		}
	}
}
