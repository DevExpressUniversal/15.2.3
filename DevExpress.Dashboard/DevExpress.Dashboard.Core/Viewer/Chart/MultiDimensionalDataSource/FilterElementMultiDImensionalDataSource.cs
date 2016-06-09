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
using System.Linq;
namespace DevExpress.DashboardCommon.Viewer {
#if DEBUG
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1039")]
#endif
	public class FilterElementMultiDimensionalDataSource : ReadOnlyTypedList {
		readonly List<AxisPoint> axisPoints = new List<AxisPoint>();
		protected List<AxisPoint> AxisPoints { get { return axisPoints; } }
		public FilterElementMultiDimensionalDataSource(MultiDimensionalData data, bool showAllValue) {
			AxisPoint axisRoot = data.GetAxisRoot(DashboardDataAxisNames.DefaultAxis);
			axisPoints = axisRoot.GetAxisPoints().ToList();
			Properties.Add(new FilterElementValuePropertyDescriptor());
			Properties.Add(new FilterElementDisplayTextPropertyDescriptor());
			if(showAllValue)
				axisPoints.Insert(0, axisRoot);
		}
		protected override object GetItemValue(int index) {
			return axisPoints[index];
		}
		protected override int GetItemsCount() {
			return axisPoints.Count;
		}
	}
	public class FilterElementValuePropertyDescriptor : ReadOnlyPropertyDescriptor {
		public const string Member = "Value";
		public FilterElementValuePropertyDescriptor()
			: base(Member) {
		}
		public override object GetValue(object component) {
			AxisPoint axisPoint = (AxisPoint)component;
			return axisPoint.Index;
		}
		public override Type PropertyType { get { return typeof(int); } }
	}
	public class FilterElementDisplayTextPropertyDescriptor : ReadOnlyPropertyDescriptor {
		public const string Member = "DisplayText";
		public FilterElementDisplayTextPropertyDescriptor()
			: base(Member) {
		}
		public override object GetValue(object component) {
			AxisPoint axisPoint = (AxisPoint)component;
			if(axisPoint.Index == -1)
				return DashboardLocalizer.GetString(DashboardStringId.FilterElementShowAllItem);
			return String.Join(", ", axisPoint.RootPath.Select(p => p.DisplayText));
		}
		public override Type PropertyType { get { return typeof(string); } }
	}
#if DEBUG
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1039")]
#endif
	public class TreeViewMultiDimensionalDataSource : ReadOnlyTypedList {
		readonly IList<AxisPoint> axisPoints;
		public TreeViewMultiDimensionalDataSource(MultiDimensionalData data) {
			AxisPoint axisRoot = data.GetAxisRoot(DashboardDataAxisNames.DefaultAxis);
			axisPoints = axisRoot.GetAllAxisPoints();
			Properties.Add(new TreeViewNodeDisplayTextPropertyDescriptor());
			Properties.Add(new TreeViewUniqueIDPropertyDescriptor());
			Properties.Add(new TreeViewParentIDPropertyDescriptor());
		}
		protected override object GetItemValue(int index) {
			return axisPoints[index];
		}
		protected override int GetItemsCount() {
			return axisPoints.Count;
		}
	}
	public class TreeViewNodeDisplayTextPropertyDescriptor : ReadOnlyPropertyDescriptor {
		public const string Member = "ColumnValue";
		public TreeViewNodeDisplayTextPropertyDescriptor()
			: base(Member) {
		}
		public override object GetValue(object component) {
			AxisPoint axisPoint = (AxisPoint)component;
			if(axisPoint.Index == -1)
				return DashboardLocalizer.GetString(DashboardStringId.FilterElementShowAllItem);
			return axisPoint.DisplayText;
		}
		public override Type PropertyType { get { return typeof(string); } }
	}
	public class TreeViewUniqueIDPropertyDescriptor : ReadOnlyPropertyDescriptor {
		public const string Member = "UniqueID";
		public TreeViewUniqueIDPropertyDescriptor()
			: base(Member) {
		}
		public override object GetValue(object component) {
			AxisPoint axisPoint = (AxisPoint)component;
			return axisPoint.Index;
		}
		public override Type PropertyType { get { return typeof(int); } }
	}
	public class TreeViewParentIDPropertyDescriptor : ReadOnlyPropertyDescriptor {
		public const string Member = "ParentID";
		public TreeViewParentIDPropertyDescriptor()
			: base(Member) {
		}
		public override object GetValue(object component) {
			AxisPoint axisPoint = (AxisPoint)component;
			if(axisPoint.Parent == null)
				return -1;
			if(axisPoint.Parent.Index == -1)
				return axisPoint.Index;
			return axisPoint.Parent.Index;
		}
		public override Type PropertyType { get { return typeof(int); } }
	}
}
