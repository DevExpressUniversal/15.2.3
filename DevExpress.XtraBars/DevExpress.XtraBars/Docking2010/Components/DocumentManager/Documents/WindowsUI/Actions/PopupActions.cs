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
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	public abstract class ContentContainerPopupMenuAction : ContentContainerAction, IContentContainerPopupMenuAction {
		protected sealed override void ExecuteCore(WindowsUIView view, IContentContainer container) { }
		protected sealed override void ExecuteCore(WindowsUIView view, IContentContainer container, object state) { }
		System.Collections.Generic.IList<IUIAction<IContentContainer>> IUIPopupMenuAction<IContentContainer>.Actions { get { return GetActions(); } }
		Orientation IUIPopupMenuAction<IContentContainer>.Orientation { get { return GetOrientation(); } }
		protected abstract System.Collections.Generic.IList<IUIAction<IContentContainer>> GetActions();
		protected abstract Orientation GetOrientation();
		internal sealed class CustomContentContainerPopupMenuAction : ContentContainerPopupMenuAction, IActionLayout, IActionBehavior {
			IContentContainerPopupMenuAction action;
			public CustomContentContainerPopupMenuAction(IContentContainerPopupMenuAction action) {
				this.action = action;
			}
			protected override bool CanExecuteCore(IContentContainer container) { return action.Command.CanExecute(container); }
			protected override string GetCaption() { return action.Caption; }
			protected override string GetDescription() { return action.Description; }
			protected override Image GetImage() { return action.Image; }
			protected override DocumentManagerStringId DescriptionID { get { throw new NotSupportedException(); } }
			protected override DocumentManagerStringId ID { get { throw new NotSupportedException(); } }
			ActionType IActionLayout.Type { get { return GetActionLayout(action).Type; } }
			ActionEdge IActionLayout.Edge { get { return GetActionLayout(action).Edge; } }
			ActionBehavior IActionBehavior.Behavior { get { return GetActionBehavior(action).Behavior; } }
			protected override System.Collections.Generic.IList<IUIAction<IContentContainer>> GetActions() { return action.Actions; }
			protected override Orientation GetOrientation() { return action.Orientation; }
		}
	}
	public abstract class ContentContainerPopupControlAction : ContentContainerAction, IContentContainerPopupControlAction {
		protected sealed override void ExecuteCore(WindowsUIView view, IContentContainer container) { }
		protected sealed override void ExecuteCore(WindowsUIView view, IContentContainer container, object state) { }
		Control IUIPopupControlAction<IContentContainer>.Control { get { return GetContentControl(); } }
		protected abstract Control GetContentControl();
		internal sealed class CustomContentContainerPopupControlAction : ContentContainerPopupControlAction, IActionLayout, IActionBehavior {
			IContentContainerPopupControlAction action;
			public CustomContentContainerPopupControlAction(IContentContainerPopupControlAction action) {
				this.action = action;
			}
			protected override bool CanExecuteCore(IContentContainer container) { return action.Command.CanExecute(container); }
			protected override string GetCaption() { return action.Caption; }
			protected override string GetDescription() { return action.Description; }
			protected override Image GetImage() { return action.Image; }
			protected override DocumentManagerStringId DescriptionID { get { throw new NotSupportedException(); } }
			protected override DocumentManagerStringId ID { get { throw new NotSupportedException(); } }
			ActionType IActionLayout.Type { get { return GetActionLayout(action).Type; } }
			ActionEdge IActionLayout.Edge { get { return GetActionLayout(action).Edge; } }
			ActionBehavior IActionBehavior.Behavior { get { return GetActionBehavior(action).Behavior; } }
			protected override Control GetContentControl() { return action.Control; }
		}
	}
	public abstract class DelegatePopupMenuActionCore<T> : UIActionPropertiesCore, IUIPopupMenuAction<T> {
		public DelegatePopupMenuActionCore(Predicate<T> canExecute) {
			Actions = new List<IUIAction<T>>();
			Command = new DelegateCommand<T>(canExecute, null);
		}
		public IList<IUIAction<T>> Actions { get; private set; }
		public System.Windows.Forms.Orientation Orientation { get; set; }
		public ICommand<T> Command { get; private set; }
	}
	public abstract class DelegatePopupControlActionCore<T> : UIActionPropertiesCore, IUIPopupControlAction<T> {
		public DelegatePopupControlActionCore(Predicate<T> canExecute) {
			Command = new DelegateCommand<T>(canExecute, null);
		}
		public ICommand<T> Command { get; private set; }
		public System.Windows.Forms.Control Control { get; set; }
	}
	[ActionGroup("Delegate", ActionType.Default)]
	public sealed class DelegatePopupMenuAction : DelegatePopupMenuActionCore<IContentContainer>, IContentContainerPopupMenuAction, IActionLayout, IActionBehavior {
		public DelegatePopupMenuAction(Predicate<IContentContainer> canExecute) : base(canExecute) { }
		public ActionType Type { get; set; }
		public ActionEdge Edge { get; set; }
		public ActionBehavior Behavior { get; set; }
	}
	[ActionGroup("Delegate", ActionType.Default)]
	public sealed class DelegatePopupControlAction : DelegatePopupControlActionCore<IContentContainer>, IContentContainerPopupControlAction, IActionLayout, IActionBehavior {
		public DelegatePopupControlAction(Predicate<IContentContainer> canExecute) : base(canExecute) { }
		public ActionType Type { get; set; }
		public ActionEdge Edge { get; set; }
		public ActionBehavior Behavior { get; set; }
	}
}
