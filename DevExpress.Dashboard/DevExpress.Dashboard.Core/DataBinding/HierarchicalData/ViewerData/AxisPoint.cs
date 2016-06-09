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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.DataProcessing;
using DevExpress.DashboardCommon.DataBinding.HierarchicalData;
namespace DevExpress.DashboardCommon.ViewerData {
	public class AxisPoint {
		DimensionDescriptor dimension;
		DimensionValue dimensionValue;
		LevelInfo level;
		List<AxisPoint> childItems = new List<AxisPoint>();
		AxisPoint parent;
		ReadOnlyCollection<AxisPoint> readOnlyChildren = null;
		int index;
		string axisName;
		internal AxisPoint(string axisName, DimensionDescriptor dimension, ServerDimensionValue serverValue, LevelInfo level) {
			this.axisName = axisName;
			this.dimension = dimension;
			this.dimensionValue = new DimensionValue(serverValue, dimension != null ? GetDisplayText(dimension, serverValue) : string.Empty);
			this.level = level;
		}
		string GetDisplayText(DimensionDescriptor dimension, ServerDimensionValue serverValue) {
			if(serverValue.DisplayText != null)
				return serverValue.DisplayText;
			if(DashboardSpecialValues.IsNullValue(serverValue.UniqueValue) || DashboardSpecialValues.IsOlapNullValue(serverValue.UniqueValue))
				return dimension.Format(serverValue.UniqueValue);
			return dimension.Format(serverValue.Value);
		}
		public DimensionDescriptor Dimension { get { return dimension; } }
		public DimensionValue DimensionValue { get { return dimensionValue; } }
		public AxisPoint Parent { get { return parent; } internal set { parent = value; } }
		public ReadOnlyCollection<AxisPoint> ChildItems {
			get {
				if(readOnlyChildren == null)
					readOnlyChildren = new ReadOnlyCollection<AxisPoint>(childItems);
				return readOnlyChildren;
			}
		}
		public string AxisName { get { return axisName; } }
		public object Value { get { return dimensionValue.Value; } }
		public object UniqueValue { get { return dimensionValue.UniqueValue; } }
		public string DisplayText { get { return dimensionValue.DisplayText; } }
		internal List<AxisPoint> ChildItemsInternal { get { return childItems; } }
		internal int Index { get { return index; } set { index = value; } }
		internal LevelInfo Level { get { return level; } }
		public ReadOnlyCollection<DimensionDescriptor> GetDimensions() {
			List<DimensionDescriptor> dimensions = new List<DimensionDescriptor>();
			if(parent != null)
				dimensions.AddRange(parent.GetDimensions());
			if(dimension != null)
				dimensions.Add(dimension);
			return new ReadOnlyCollection<DimensionDescriptor>(dimensions);
		}
		public DimensionValue GetDimensionValue(DimensionDescriptor dimension) {
			if(this.dimension.Equals(dimension))
				return dimensionValue;
			if(parent != null)
				return parent.GetDimensionValue(dimension);
			else
				return null;
		}
		public override bool Equals(object obj) {
			AxisPoint axisPoint = obj as AxisPoint;
			if(axisPoint != null && String.Equals(AxisName, axisPoint.AxisName) && EqualsInternal(axisPoint))
				return AxisPoint.Equals(parent, axisPoint.parent);
			return false;
		}
		public override int GetHashCode() {
			int hash = 0;
			foreach(DimensionValue value in RootPath.Select(p => p.dimensionValue))
				hash ^= value.GetHashCode();
			return hash;
		}
		internal int GetInstanceHashCode() {
			return base.GetHashCode();
		}
		internal DimensionValue GetDimensionValue(string id) {
			if(!string.IsNullOrEmpty(id) && this.dimension != null) {
				if(this.dimension.ID.Equals(id))
					return dimensionValue;
				if(parent != null)
					return parent.GetDimensionValue(id);
			}
			return null;
		}
		internal DimensionDescriptorCollection GetChildDimensions() {
			List<DimensionDescriptor> dimensions = new List<DimensionDescriptor>();
			if(dimension != null)
				dimensions.Add(dimension);
			if(childItems.Count > 0)
				dimensions.AddRange(childItems[0].GetChildDimensions());
			return new DimensionDescriptorCollection(dimensions);
		}
		internal IList<AxisPoint> GetAllAxisPoints() {
			return GetAllAxisPoints(null);
		}
		internal IList<AxisPoint> GetAllAxisPoints(bool includeGrandTotal) {
			IList<AxisPoint> result = GetAllAxisPoints();
			if(!includeGrandTotal)
				result = result.Where(p => p.Parent != null).ToList();
			return result;
		}
		internal IList<AxisPoint> GetAllAxisPoints(Func<AxisPoint, bool> selector) {
			List<AxisPoint> axisPoints = new List<AxisPoint>();
			if(selector == null || selector(this)) {
				axisPoints.Add(this);
				foreach(AxisPoint child in childItems)
					axisPoints.AddRange(child.GetAllAxisPoints(selector));
			}
			return axisPoints;
		}
		internal IEnumerable<AxisPoint> GetAxisPoints() {
			List<AxisPoint> axisPoints = new List<AxisPoint>();
			if(childItems.Count == 0 && parent != null)
				axisPoints.Add(this);
			else {
				foreach(AxisPoint child in childItems) {
					axisPoints.AddRange(child.GetAxisPoints());
				}
			}
			return axisPoints;
		}
		internal AxisPoint FindPointByIndex(int index) {
			if(this.index == index)
				return this;
			AxisPoint res = null;
			foreach(AxisPoint child in childItems) {
				res = child.FindPointByIndex(index);
				if(res != null)
					break;
			}
			return res;
		}
		internal IEnumerable<AxisPoint> FindPointsByIndices(IEnumerable<int> indices) {
			if(indices.Contains(index))
				yield return this;
			foreach(AxisPoint point in childItems) {
				foreach(AxisPoint child in point.FindPointsByIndices(indices))
					yield return child;
			}
		}
		internal AxisPoint GetAxisPointByUniqueValues(object[] values) {
			if(values != null && values.Length > 0) {
				if(dimension == null || Object.Equals(values[0], dimensionValue.UniqueValue)) {
					object[] nextValues = dimension == null ? values : values.Skip(1).ToArray();
					if(nextValues.Length == 0)
						return this;
					AxisPoint res = null;
					foreach(AxisPoint child in childItems) {
						res = child.GetAxisPointByUniqueValues(nextValues);
						if(res != null)
							return res;
					}
				}
			}
			return null;
		}
		internal IEnumerable<AxisPoint> GetAxisPointsByUniqueValues(List<KeyValuePair<string, object>> values) {
			if(values.Count == 0)
				return new List<AxisPoint> { this };
			IEnumerable<AxisPoint> targetPoints = new List<AxisPoint> { this };
			foreach(KeyValuePair<string, object> pair in values) {
				List<AxisPoint> levelPoints = new List<AxisPoint>();
				List<AxisPoint> childItems = targetPoints.SelectMany(p => p.ChildItems).ToList();
				if(!DashboardSpecialValues.IsOlapNullValue(pair.Value) || childItems.Count > 0) {
					foreach(AxisPoint point in childItems)
						levelPoints.AddRange(point.GetAxisPoints(pair.Key, pair.Value).Where(p => p != null));
					targetPoints = levelPoints;
				}
			}
			return targetPoints;
		}
		IEnumerable<AxisPoint> GetAxisPoints(string dimensionID, object value) {
			if(dimension != null && dimension.ID == dimensionID) {
				if(Object.Equals(value, dimensionValue.UniqueValue))
					yield return this;
				yield return null;
			}
			else {
				if(DashboardSpecialValues.IsOlapNullValue(value) && childItems.Count == 0)
					yield return this;
				foreach(AxisPoint child in childItems) {
					foreach(AxisPoint foundChild in child.GetAxisPoints(dimensionID, value))
						yield return foundChild;
				}
			}
		}
		internal List<AxisPoint> GetAxisPointsByDimension(DimensionDescriptor dimension, bool includeRaggedHierarchy) {
			if(this.dimension == dimension || includeRaggedHierarchy && childItems.Count == 0 && Parent != null)
				return new List<AxisPoint>() { this };
			List<AxisPoint> children = new List<AxisPoint>();
			foreach(AxisPoint child in childItems) {
				children.AddRange(child.GetAxisPointsByDimension(dimension, includeRaggedHierarchy));
			}
			return children;
		}
		internal List<object> GetAllDimensionValues() {
			return RootPath.Select(p => p.dimensionValue.UniqueValue).ToList();
		}
		internal IEnumerable<AxisPoint> RootPath {
			get {
				if(Parent == null)
					yield break;
				foreach(AxisPoint point in Parent.RootPath)
					yield return point;
				yield return this;
			}
		}
		internal AxisPoint GetParentByDimensionID(string dimensionID) {
			AxisPoint current = this;
			while(current.Parent != null) {
				if(current.Dimension != null && current.Dimension.ID == dimensionID)
					return current;
				current = current.Parent;
			}
			return dimensionID != null ? this : current;
		}
		internal bool DimensionValueEquals(AxisPoint axisPoint) {
			if(axisPoint != null && DimensionValueEqualsInternal(axisPoint))
				return parent.DimensionValueEqualsInternal(axisPoint.parent);
			return false;
		}
		bool DimensionValueEqualsInternal(AxisPoint axisPoint) {
			return this.dimensionValue.Equals(axisPoint.dimensionValue);
		}
		bool EqualsInternal(AxisPoint axisPoint) {
			return this.dimensionValue.Equals(axisPoint.dimensionValue);
		}
	}
}
