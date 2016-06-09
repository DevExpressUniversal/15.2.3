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

using DevExpress.XtraPivotGrid.Data;
using System;
using System.Linq;
using System.Collections.Generic;
namespace DevExpress.DashboardCommon.Native {
	public class PivotExpandCollapseActionController {
		class ValuesContainer {
			readonly List<object[]> storage = new List<object[]>();
			readonly bool shorterToLongerSortOrder;
			public ValuesContainer(bool shorterToLongerSortOrder) {
				this.shorterToLongerSortOrder = shorterToLongerSortOrder;
			}
			public void Add(object[] value) {
				CheckLength(value);
				if(!storage.Any((v => AreEqual(v, value))))
					storage.Add(value);
			}
			public void Remove(object[] value) {
				CheckLength(value);
				storage.RemoveAll(v => AreEqual(v, value));
			}
			public IList<object[]> RemoveChildren(object[] parent) {
				IList<object[]> childs = storage.Where(v => IsChild(parent, v)).ToList<object[]>();
				storage.RemoveAll(v => childs.Contains(v));
				return childs;
			}
			public IList<object[]> RemoveParents(object[] child) {
				IList<object[]> parents = storage.Where(v => IsChild(v, child)).ToList<object[]>();
				storage.RemoveAll(v => parents.Contains(v));
				return parents;
			}
			public void AddRange(IList<object[]> values) {
				foreach(object[] value in values)
					Add(value);
			}
			public IList<object[]> Values {
				get {
					storage.Sort((value1, value2) => {
						int delta = value1.Length - value2.Length;
						if(!shorterToLongerSortOrder)
							delta *= -1;
						return delta;
					});
					return storage;
				}
				set {
					storage.Clear();
					if(value != null)
						storage.AddRange(value);
				}
			}
			void CheckLength(object[] values) {
				if(values.Length == 0)
					throw new ArgumentException("values length = 0");
			}
			bool AreEqual(object[] values1, object[] values2) {
				int length1 = values1.Length;
				int length2 = values2.Length;
				if(length1 != length2)
					return false;
				for(int i = 0; i < length1; i++) {
					if(!Object.Equals(values1[i], values2[i]))
						return false;
				}
				return true;
			}
			bool IsChild(object[] parent, object[] values) {
				if(values.Length > parent.Length) {
					int length = parent.Length;
					for(int i = 0; i < length; i++) {
						if(!Object.Equals(parent[i], values[i]))
							return false;
					}
					return true;
				}
				return false;
			}
		}
		readonly bool isInitiallyCollapsed;
		readonly ValuesContainer container;
		readonly ValuesContainer hiddenContainer;
		public PivotExpandCollapseActionController(bool isInitiallyCollapsed) {
			this.isInitiallyCollapsed = isInitiallyCollapsed;
			this.container = new ValuesContainer(isInitiallyCollapsed);
			this.hiddenContainer = new ValuesContainer(isInitiallyCollapsed);
		}
		public IList<object[]> PerformChangeAction(object[] values, bool expanded) {
			List<object[]> actionValues = new List<object[]>();
			if(IsPositiveAction(expanded)) {
				container.Add(values);
				actionValues.Add(values);
				IList<object[]> removedHiddenValues = hiddenContainer.RemoveChildren(values);
				container.AddRange(removedHiddenValues);
				actionValues.AddRange(removedHiddenValues);
				hiddenContainer.Remove(values);
			} else {
				container.Remove(values);
				actionValues.Add(values);
				hiddenContainer.AddRange(isInitiallyCollapsed ? container.RemoveChildren(values) : container.RemoveParents(values));
			}
			return actionValues;
		}
		public IList<object[]> GetEntireActionValues() {
			return container.Values;
		}
		public PivotAreaExpandCollapseState GetState() {
			return new PivotAreaExpandCollapseState(container.Values.ToList(), hiddenContainer.Values.ToList());
		}
		public void SetState(PivotAreaExpandCollapseState state) {
			container.Values = state.Values;
			hiddenContainer.Values = state.HiddenValues;
		}
		bool IsPositiveAction(bool expanded) {
			return expanded == isInitiallyCollapsed;
		}
	}
}
