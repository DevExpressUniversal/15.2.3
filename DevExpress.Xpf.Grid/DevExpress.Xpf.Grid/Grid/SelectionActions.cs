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
using DevExpress.Xpf.Editors;
namespace DevExpress.Xpf.Grid.Native {
	public abstract class SelectionActionBase : IAction {
		protected readonly int oldRowHandle;
		protected readonly GridViewBase view;
		protected GridControl GridControl { get { return (GridControl)view.DataControl; } }
		protected GridControl FocusedGrid { get { return (GridControl)FocusedView.DataControl; } }
		protected GridViewBase FocusedView { get { return (GridViewBase)view.FocusedView; } }
		protected SelectionActionBase(GridViewBase view) {
			this.view = view;
			oldRowHandle = view.GlobalSelectionAnchor.RowHandle;
			CanFocusChangeDeleteAction = true;
		}
		public bool CanFocusChangeDeleteAction { get; set; }
		#region IAction Members
		public abstract void Execute();
		#endregion
	}
	public class AddRowsToSelectionAction : SelectionActionBase {
		readonly int startCommonVisibleIndex;
		public AddRowsToSelectionAction(GridViewBase view) : base(view) {
			view.EditorSetInactiveAfterClick = true;
			var selectionView = view.GlobalSelectionAnchor.View;
			startCommonVisibleIndex = selectionView.DataControl.GetCommonVisibleIndex(view.GlobalSelectionAnchor.RowHandle);
		}
		public override void Execute() {
			int endCommonlVisibleIndex = FocusedView.DataControl.GetCommonVisibleIndex(FocusedView.FocusedRowHandle);
			FocusedView.SelectionStrategy.SelectOnlyThisMasterDetailRange(startCommonVisibleIndex, endCommonlVisibleIndex);
		}
	}
	public class AddOneRowToSelectionAction : SelectionActionBase {
		public AddOneRowToSelectionAction(GridViewBase view) : base(view) {
			view.EditorSetInactiveAfterClick = true;
		}
		public override void Execute() {
			FocusedGrid.SelectItem(FocusedView.FocusedRowHandle);
			FocusedView.SetSelectionAnchor();
		}
	}
	public class OnlyThisSelectionAction : AddOneRowToSelectionAction {
		public OnlyThisSelectionAction(GridViewBase view) : base(view) {
			view.EditorSetInactiveAfterClick = false;
		}
		public override void Execute() {
			view.SelectionStrategy.DoMasterDetailSelectionAction(() => {
				GridControl.ClearMasterDetailSelection();
				if(FocusedView.GetActualSelectionMode() != MultiSelectMode.MultipleRow)
					base.Execute();
			});
		}
	}
	public class OnlyThisSelectionMouseUpAction : OnlyThisSelectionAction {
		public OnlyThisSelectionMouseUpAction(GridViewBase view)
			: base(view) {
			view.EditorSetInactiveAfterClick = true;
			CanFocusChangeDeleteAction = false;
		}
	}
	public class InvertSelectionOnMouseUpAction : SelectionActionBase {
		public InvertSelectionOnMouseUpAction(GridViewBase view)
			: base(view) {
			view.EditorSetInactiveAfterClick = true;
			CanFocusChangeDeleteAction = false;
		}
		public override void Execute() {
			view.SelectionStrategy.OnInvertSelection();
		}
	}
}
