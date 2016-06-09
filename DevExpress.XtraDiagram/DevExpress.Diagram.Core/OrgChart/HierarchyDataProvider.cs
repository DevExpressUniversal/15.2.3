#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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

using DevExpress.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Diagram.Core {
	public interface IChildrenSelector {
		IEnumerable<object> GetChildren(object parent);
	}
	public sealed class DelegateChildrenSelector : IChildrenSelector {
		readonly Func<object, IEnumerable<object>> getChildren;
		public DelegateChildrenSelector(Func<object, IEnumerable<object>> getChildren) {
			this.getChildren = getChildren;
		}
		IEnumerable<object> IChildrenSelector.GetChildren(object parent) {
			return getChildren(parent);
		}
	}
	public interface IHierarchyDataProvider {
		IEnumerable<object> AllItems { get; }
		IEnumerable<object> GetChildren(object node);
	}
	sealed class PlainListDataProvider : IHierarchyDataProvider {
		readonly IEnumerable ItemsSource;
		public PlainListDataProvider(IEnumerable itemsSource) {
			ItemsSource = itemsSource;
		}
		IEnumerable<object> IHierarchyDataProvider.AllItems { get { return ItemsSource.Cast<object>(); } }
		IEnumerable<object> IHierarchyDataProvider.GetChildren(object node) {
			return EmptyArray<object>.Instance;
		}
	}
	sealed class KeyParentModeDataProvider : IHierarchyDataProvider {
		readonly IEnumerable itemsSource;
		readonly string keyMember, parentMember;
		readonly Dictionary<object, object[]> itemToChildrenMap;
		public KeyParentModeDataProvider(IEnumerable itemsSource, string keyMember, string parentMember) {
			this.itemsSource = itemsSource;
			this.keyMember = keyMember;
			this.parentMember = parentMember;
			itemToChildrenMap = itemsSource.Cast<object>()
				.GroupBy(x => PropertyDescriptorHelper.GetValue(x, parentMember))
				.ToDictionary(x => x.Key, x => x.ToArray());
		}
		IEnumerable<object> IHierarchyDataProvider.AllItems { get { return itemsSource.Cast<object>(); } }
		IEnumerable<object> IHierarchyDataProvider.GetChildren(object node) {
			var id = PropertyDescriptorHelper.GetValue(node, keyMember);
			return itemToChildrenMap.GetValueOrDefault(id, EmptyArray<object>.Instance);
		}
	}
	sealed class ChildrenSelectorDataProvider : IHierarchyDataProvider {
		readonly IEnumerable itemsSource;
		readonly Func<object, IEnumerable> getChildren;
		public ChildrenSelectorDataProvider(IEnumerable itemsSource, Func<object, IEnumerable> getChildren) {
			this.itemsSource = itemsSource;
			this.getChildren = getChildren;
		}
		IEnumerable<object> IHierarchyDataProvider.AllItems { get { return itemsSource.Cast<object>().Flatten(GetChildren); } }
		IEnumerable<object> IHierarchyDataProvider.GetChildren(object node) {
			return GetChildren(node);
		}
		IEnumerable<object> GetChildren(object node) {
			return getChildren(node).Cast<object>();
		}
	}
}
