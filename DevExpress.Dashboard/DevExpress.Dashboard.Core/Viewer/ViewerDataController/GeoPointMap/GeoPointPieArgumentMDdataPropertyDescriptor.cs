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
	public class GeoPointPieArgumentUniqueValueMDdataPropertyDescriptor : ReadOnlyPropertyDescriptor {
		public GeoPointPieArgumentUniqueValueMDdataPropertyDescriptor(string name)
			: base(name) {
		}
		public override object GetValue(object component) {
			AxisPoint axisPoint = (AxisPoint)component;
			return axisPoint.UniqueValue;
		}
		public override Type PropertyType {
			get { return typeof(object); }
		}
	}
	public class GeoPointPieArgumentDisplayTextMDdataPropertyDescriptor : ReadOnlyPropertyDescriptor {
		public GeoPointPieArgumentDisplayTextMDdataPropertyDescriptor(string name)
			: base(name) {
		}
		public override object GetValue(object component) {
			AxisPoint axisPoint = (AxisPoint)component;
			return axisPoint.DisplayText;
		}
		public override Type PropertyType {
			get { return typeof(object); }
		}
	}
	public class GeoPointPieArgumentNameMDdataPropertyDescriptor : ReadOnlyPropertyDescriptor {
		readonly string argumentName;
		public GeoPointPieArgumentNameMDdataPropertyDescriptor(MultiDimensionalData data, string measureId, string name)
			: base(name) {
			argumentName = data.GetMeasureDescriptorByID(measureId).Name;
		}
		public override object GetValue(object component) {
			return argumentName;
		}
		public override Type PropertyType {
			get { return typeof(object); }
		}
	}
}
