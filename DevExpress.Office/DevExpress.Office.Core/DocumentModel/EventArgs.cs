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
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Office.Model {
	#region UndoableCollection
	#region UndoableCollectionAdd
	public delegate void UndoableCollectionAddEventHandler(object sender, EventArgs e);
	public class UndoableCollectionAddEventArgs<T> : EventArgs {
		readonly T item;
		public UndoableCollectionAddEventArgs(T item) {
			this.item = item;
		}
		public T Item { get { return item; } }
	}
	#endregion
	#region UndoableCollectionRemoveAt
	public delegate void UndoableCollectionRemoveAtEventHandler(object sender, UndoableCollectionRemoveAtEventArgs e);
	public class UndoableCollectionRemoveAtEventArgs : EventArgs {
		readonly int index;
		public UndoableCollectionRemoveAtEventArgs(int index) {
			this.index = index;
		}
		public int Index { get { return index; } }
	}
	#endregion
	#region UndoableCollectionInsert
	public delegate void UndoableCollectionInsertEventHandler(object sender, EventArgs e);
	public class UndoableCollectionInsertEventArgs<T> : UndoableCollectionRemoveAtEventArgs {
		readonly T item;
		public UndoableCollectionInsertEventArgs(int index, T item)
			: base(index) {
			this.item = item;
		}
		public T Item { get { return item; } }
	}
	#endregion
	#region UndoableCollectionClear
	public delegate void UndoableCollectionClearEventHandler(object sender);
	#endregion
	#region UndoableCollectionAddRange
	public delegate void UndoableCollectionAddRangeEventHandler(object sender, EventArgs e);
	public class UndoableCollectionAddRangeEventArgs<T> : EventArgs {
		readonly IEnumerable<T> collection;
		public UndoableCollectionAddRangeEventArgs(IEnumerable<T> collection) {
			this.collection = collection;
		}
		public IEnumerable<T> Collection { get { return collection; } }
	}
	#endregion
	#region UndoableCollectionMove
	public delegate void UndoableCollectionMoveEventHandler(object sender, EventArgs e);
	public class UndoableCollectionMoveEventArgs : EventArgs {
		readonly int sourceIndex;
		readonly int targetIndex;
		public UndoableCollectionMoveEventArgs(int sourceIndex, int targetIndex) {
			this.sourceIndex = sourceIndex;
			this.targetIndex = targetIndex;
		}
		public int SourceIndex { get { return sourceIndex; } }
		public int TargetIndex { get { return targetIndex; } }
	}
	#endregion
	#endregion
}
