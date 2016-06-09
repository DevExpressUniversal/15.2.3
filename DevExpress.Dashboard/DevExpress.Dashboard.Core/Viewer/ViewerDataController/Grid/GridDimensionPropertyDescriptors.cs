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
	public abstract class GridDimensionMDDataPropertyDescriptorBase : ReadOnlyPropertyDescriptor {
		readonly string dimensionId;
		readonly Type propertyType;
		protected GridDimensionMDDataPropertyDescriptorBase(string dimensionId, string name, Type propertyType)
			: base(name) {
			this.dimensionId = dimensionId;
			this.propertyType = propertyType;
		}
		public override Type PropertyType {
			get { return propertyType; }
		}
		public AxisPoint GetAxisPoint(object component) {
			AxisPoint point = component as AxisPoint;
			if(point != null) {
				AxisPoint current = point;
				while(current.Dimension != null && current.Dimension.ID != dimensionId)
					current = current.Parent;
				if(current.Parent != null)
					return current;
			}
			return null;
		}
	}
	public class GridDimensionValueMDDataPropertyDescriptor : GridDimensionMDDataPropertyDescriptorBase {
		public GridDimensionValueMDDataPropertyDescriptor(string dimensionId, Type propertyType)
			: base(dimensionId, dimensionId, propertyType) {
		}
		public override object GetValue(object component) {
			AxisPoint point = GetAxisPoint(component);
			if(point == null || DashboardSpecialValues.IsNullValue(point.Value))
				return null;
			return point.Value;
		}
	}
	public class GridDimensionDisplayTextMDDataPropertyDescriptor : GridDimensionMDDataPropertyDescriptorBase {
		public GridDimensionDisplayTextMDDataPropertyDescriptor(string dimensionId, string name)
			: base(dimensionId, name, typeof(string)) {
		}
		public override object GetValue(object component) {
			AxisPoint point = GetAxisPoint(component);
			return point != null ? point.DisplayText : null;
		}
	}
	public class GridDimensionUniqueNameMDDataPropertyDescriptor: GridDimensionMDDataPropertyDescriptorBase {
		public GridDimensionUniqueNameMDDataPropertyDescriptor(string dimensionId, string name, Type propertyType)
			: base(dimensionId, name, propertyType) {
		}
		public override object GetValue(object component) {
			AxisPoint point = GetAxisPoint(component);
			return point != null ? point.UniqueValue : DashboardSpecialValues.OlapNullValue;
		}
	}
}
