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
using DevExpress.Utils;
namespace DevExpress.Office.History {
	#region IndexChangedHistoryItemCore
	public abstract class IndexChangedHistoryItemCore<TActions> : HistoryItem where TActions : struct {
		#region Fields
		int oldIndex;
		int newIndex;
		TActions changeActions;
		#endregion
		protected IndexChangedHistoryItemCore(IDocumentModelPart documentModelPart)
			: base(documentModelPart) {
		}
		#region Properties
		public int OldIndex { get { return oldIndex; } set { oldIndex = value; } }
		public int NewIndex { get { return newIndex; } set { newIndex = value; } }
		public TActions ChangeActions { get { return changeActions; } set { changeActions = value; } }
		#endregion
		protected override void UndoCore() {
			IIndexBasedObject<TActions> obj = GetObject();
			obj.SetIndex(OldIndex, ChangeActions);
		}
		protected override void RedoCore() {
			IIndexBasedObject<TActions> obj = GetObject();
			obj.SetIndex(NewIndex, ChangeActions);
		}
		public abstract IIndexBasedObject<TActions> GetObject();
	}
	#endregion
	#region IndexChangedHistoryItem
	public class IndexChangedHistoryItem<TActions> : IndexChangedHistoryItemCore<TActions> where TActions : struct {
		readonly IIndexBasedObject<TActions> obj;
		public IndexChangedHistoryItem(IDocumentModelPart documentModelPart, IIndexBasedObject<TActions> obj)
			: base(documentModelPart) {
			Guard.ArgumentNotNull(obj, "obj");
			this.obj = obj;
		}
		public IIndexBasedObject<TActions> Object { get { return obj; } }
		public override IIndexBasedObject<TActions> GetObject() {
			return obj;
		}
	}
	#endregion
}
