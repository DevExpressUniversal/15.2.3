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
using System.Collections.ObjectModel;
using System.Linq;
namespace DevExpress.DashboardCommon.Native {
	public class DimensionValueSet {
		readonly OrderedDictionary<Dimension, IList<object>> valueStore;
		public IList<Dimension> Dimensions { get { return valueStore.Keys.ToList<Dimension>(); } }
		public int Count {
			get {
				int result = 0;
				foreach(Dimension dimension in valueStore.Keys)
					result = Math.Max(result, valueStore[dimension].Count);
				return result;
			}
		}
		public int FilterLength { get { 
			int count = 0;
			foreach(Dimension dim in valueStore.Keys)
				if(valueStore[dim].Count>0)
					count++;
			return count; 
		} }
#if DEBUG
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006")]
#endif
		public DimensionValueSet(OrderedDictionary<Dimension, IList<object>> valueStore) {
			this.valueStore = valueStore;
		}
		public object GetValue(Dimension dimension, int valueIndex) {
			IList<object> dimensionFilters;
			if(valueStore.TryGetValue(dimension, out dimensionFilters) && dimensionFilters.Count > 0)
				return dimensionFilters.Count > valueIndex ? dimensionFilters[valueIndex] : dimensionFilters[dimensionFilters.Count - 1];
			return null;
		}
		public IValuesSet GetValuesSet(IEnumerable<Dimension> dimensionList) {
			IEnumerable<Dimension> dimensions = dimensionList == null ? Dimensions : dimensionList;
			IEnumerable<ISelectionColumn> values = dimensions.Select(d => {
				IList<object> value;
				if (valueStore.TryGetValue(d, out value))
					return value.AsSelectionColumn();
				else
					return null;
			}).Where(x => x != null);
			return values.AsValuesSet();
		}
		public OrderedDictionary<Dimension, object> GetSingleMasterFilterDictionary() {
			OrderedDictionary<Dimension, object> result = new OrderedDictionary<Dimension, object>();
			ReadOnlyCollection<Dimension> dimensions = valueStore.Keys;
			for(int i = 0; i < dimensions.Count; i++) {
				result.Add(dimensions[i], valueStore[dimensions[i]][0]);
			}
			return result;
		}
	}
	public class MasterFilterNode {
		IList<MasterFilterNode> nodeCollection = new List<MasterFilterNode>();
		object value;
		public MasterFilterNode(object value) {
			this.value = value;
		}
		public MasterFilterNode GetChildNode(object value) {
			foreach(MasterFilterNode node in nodeCollection) {
				if(Object.Equals(node.Value, value))
					return node;
			}
			MasterFilterNode newNode = new MasterFilterNode(value);
			nodeCollection.Add(newNode);
			return newNode;
		}
		public object Value {
			get { return value; }
		}
		public IList<MasterFilterNode> ChildNodes { get { return nodeCollection; } }
	}
}
