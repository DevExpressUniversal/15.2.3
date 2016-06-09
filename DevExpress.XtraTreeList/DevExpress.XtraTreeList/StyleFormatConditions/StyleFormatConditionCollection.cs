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

using System;
using System.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.Utils;
using DevExpress.XtraTreeList.Columns;
using DevExpress.Data;
namespace DevExpress.XtraTreeList.StyleFormatConditions {
	[ListBindable(false)]
	public class StyleFormatConditionCollection : FormatConditionCollectionBase {
		TreeList treeList; 
		DataController dataController;
		public StyleFormatConditionCollection(TreeList treeList) {
			this.treeList = treeList;
			dataController = new DataController();
		}
		public void Add(StyleFormatCondition condition) {
			base.Add(condition);
		}
		public void AddRange(StyleFormatCondition[] conditions) {
			base.AddRange(conditions);
		}
		public override int CompareValues(object val1, object val2) {
			return dataController.ValueComparer.Compare(val1, val2);
		}
		public new StyleFormatCondition this[int index] {
			get {
				return (base.List[index] as StyleFormatCondition);
			}
		}
		public new StyleFormatCondition this[object tag] {
			get {
				foreach(StyleFormatCondition condition in base.List) {
					if(condition.Tag == tag) 
						return condition;
				}
				return null;
			}
		}
		public TreeList TreeListControl {
			get {
				return ((this.treeList != null) ? treeList : null);
			}
		}
		public override bool IsLoading {
			get {
				return ((this.treeList == null) || this.treeList.IsLoading);
			}
		}
		protected override object CreateItem() {
			return new StyleFormatCondition();
		}
		protected internal virtual void OnColumnRemoved(TreeListColumn column) {
			if(IsLoading) return;
			for(int n = Count - 1; n >= 0; n--)
				this[n].OnColumnRemoved(column);
		}
	}
}
