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
using DevExpress.Xpf.Grid;
using System.Collections;
using DevExpress.Xpf.Data;
using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Grid.Native {
	public class GridRowsEnumerator : IEnumerator {
		protected VirtualItemsEnumerator en;
		public RowDataBase CurrentRowData { get { return CurrentNode.GetRowData(); } }
		public FrameworkElement CurrentRow { get { return CurrentNode.GetRowElement(); } }
		public RowNode CurrentNode { get { return en.Current; } }
		protected DataViewBase view;
		public GridRowsEnumerator(DataViewBase view, NodeContainer containerItem) {
			this.view = view;
			en = CreateInnerEnumerator(containerItem);
		}
		public virtual bool MoveNext() {
			while(en.MoveNext()) {
				if(CurrentRow != null && IsCurrentRowInTree())
					return true;
			}
			return false;
		}
		protected bool IsCurrentRowInTree() {
			return LayoutHelper.FindParentObject<DataPresenterBase>(CurrentRow) != null;
		}
		public void Reset() {
			en.Reset();
		}
		object IEnumerator.Current { get { return en.Current; } }
		protected virtual VirtualItemsEnumerator CreateInnerEnumerator(NodeContainer containerItem) {
			return new SkipCollapsedGroupVirtualItemsEnumerator(containerItem);
		}
	}
	public class SkipInvisibleGridRowsEnumerator : GridRowsEnumerator {
		public SkipInvisibleGridRowsEnumerator(DataViewBase view, NodeContainer containerItem)
			: base(view, containerItem) {
		}
		public override bool MoveNext() {
			bool result = en.MoveNext();
			if(!result)
				return false;
			bool exitLoop = false;
			do {
				if(en.Current is RowNode) {
					RowNode data = (RowNode)en.Current;
					if(!data.IsRowVisible) {
						result = en.MoveNext();
						if(!result)
							return false;
						continue;
					}
				}
				exitLoop = true;
			}
			while(!exitLoop);
			return IsCurrentRowInTree();
		}
	}
}
