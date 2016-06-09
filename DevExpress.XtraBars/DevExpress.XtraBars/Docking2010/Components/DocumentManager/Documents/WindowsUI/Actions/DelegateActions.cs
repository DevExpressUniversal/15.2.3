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
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	public abstract class BaseDelegateAction : DelegateActionCore<IContentContainer>, IContentContainerAction, IActionBehavior {
		public BaseDelegateAction(Predicate<IContentContainer> canExecute, Action<IContentContainer> execute)
			: base(canExecute, execute) {
		}
		public ActionBehavior Behavior { get; set; }
	}
	[ActionGroup("Context", ActionType.Context)]
	public sealed class ContextualDelegateAction : BaseDelegateAction {
		public ContextualDelegateAction(Predicate<IContentContainer> canExecute, Action<IContentContainer> execute)
			: base(canExecute, execute) {
		}
	}
	[ActionGroup("Navigation", ActionType.Navigation)]
	public sealed class NavigationDelegateAction : BaseDelegateAction {
		public NavigationDelegateAction(Predicate<IContentContainer> canExecute, Action<IContentContainer> execute)
			: base(canExecute, execute) {
		}
	}
	[ActionGroup("Delegate", ActionType.Default)]
	public sealed class DelegateAction : BaseDelegateAction, IActionLayout {
		public DelegateAction(Func<bool> canExecute, Action execute)
			: base((c) => canExecute(), (c) => execute()) {
		}
		public ActionType Type { get; set; }
		public ActionEdge Edge { get; set; }
	}
	[ActionGroup("Document", ActionType.Default)]
	public sealed class DocumentAction : DelegateActionCore<Document>, IDocumentAction, IActionStyle {
		public DocumentAction(Predicate<Document> canExecute, Action<Document> execute)
			: base(canExecute, execute) {
		}
		public DocumentAction(Action<Document> execute)
			: base((document) => true, execute) {
		}
		ActionStyle IActionStyle.Style { get { return ActionStyle.PushAction; } }
		object IActionStyle.InitialState { get { return null; } }
	}
	[ActionGroup("Document", ActionType.Default)]
	public sealed class DocumentCheckAction : DelegateActionCore<Document>, IDocumentCheckAction, IActionStyle {
		Func<bool> getState;
		public DocumentCheckAction(Func<bool> getState, Action<Document> toggle)
			: this((document) => true, getState, toggle) {
		}
		public DocumentCheckAction(Predicate<Document> canExecute, Func<bool> getState, Action<Document> toggle)
			: base(canExecute, toggle) {
			Base.AssertionException.IsNotNull(getState);
			this.getState = getState;
			this.CheckedCommand = Command;
			this.UncheckedCommand = Command;
		}
		public DocumentCheckAction(Func<bool> getState, Action<Document> check, Action<Document> uncheck)
			: this((document) => true, getState, check, uncheck) {
		}
		public DocumentCheckAction(Predicate<Document> canExecute, Func<bool> getState, Action<Document> check, Action<Document> uncheck)
			: base(canExecute, (document) => { }) {
			Base.AssertionException.IsNotNull(getState);
			this.getState = getState;
			this.CheckedCommand = new DelegateCommand<Document>(CanCheck, check);
			this.UncheckedCommand = new DelegateCommand<Document>(CanUncheck, uncheck);
		}
		bool CanCheck(Document document) { return !getState(); }
		bool CanUncheck(Document document) { return getState(); }
		public ICommand<Document> CheckedCommand { get; private set; }
		public ICommand<Document> UncheckedCommand { get; private set; }
		ActionStyle IActionStyle.Style { get { return ActionStyle.CheckAction; } }
		object IActionStyle.InitialState { get { return getState(); } }
	}
}
