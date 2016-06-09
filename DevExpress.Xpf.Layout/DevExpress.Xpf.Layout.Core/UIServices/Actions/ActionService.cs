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

namespace DevExpress.Xpf.Layout.Core.Actions {
	class ActionService : UIService, IActionService {
		int actionLockCounter;
		public bool InAction {
			get { return actionLockCounter > 0; }
		}
		public void Hide(IView view, bool immediatelly) {
			if(IsDisposing || InAction || view == null) return;
			actionLockCounter++;
			IActionServiceListener listener = view.GetUIServiceListener<IActionServiceListener>();
			if(listener != null) listener.OnHide(immediatelly);
			actionLockCounter--;
		}
		public void Expand(IView view) {
			if(IsDisposing || InAction || view == null) return;
			actionLockCounter++;
			IActionServiceListener listener = view.GetUIServiceListener<IActionServiceListener>();
			if(listener != null) listener.OnExpand();
			actionLockCounter--;
		}
		public void Collapse(IView view) {
			if(IsDisposing || InAction || view == null) return;
			actionLockCounter++;
			IActionServiceListener listener = view.GetUIServiceListener<IActionServiceListener>();
			if(listener != null) listener.OnCollapse();
			actionLockCounter--;
		}
		public void ShowSelection(IView view) {
			if(IsDisposing || InAction || view == null) return;
			actionLockCounter++;
			IActionServiceListener listener = view.GetUIServiceListener<IActionServiceListener>();
			if(listener != null) listener.OnShowSelection();
			actionLockCounter--;
		}
		public void HideSelection(IView view) {
			if(IsDisposing || InAction || view == null) return;
			actionLockCounter++;
			IActionServiceListener listener = view.GetUIServiceListener<IActionServiceListener>();
			if(listener != null) listener.OnHideSelection();
			actionLockCounter--;
		}
	}
	class ContextActionService : UIService, IContextActionService {
		public bool DoContextAction(IView view, ILayoutElement element, ContextAction action) {
			if(IsDisposing || view == null || element == null) return false;
			IContextActionServiceListener listener = view.GetUIServiceListener<IContextActionServiceListener>();
			return listener != null ? listener.OnContextAction(element, action) : false;
		}
	}
}
