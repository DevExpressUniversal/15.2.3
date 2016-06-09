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
using System.Drawing;
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	public interface ICommand<T> {
		bool CanExecute(T parameter);
		void Execute(T parameter);
	}
	public interface IUIActionProperties {
		Image Image { get; }
		string Caption { get; }
		string Description { get; }
	}
	public interface IUIAction<T> : IUIActionProperties {
		ICommand<T> Command { get; }
	}
	public interface IUICheckAction<T> : IUIAction<T> {
		ICommand<T> CheckedCommand { get; }
		ICommand<T> UncheckedCommand { get; }
	}
	public interface IUIActionList<T> : IUIActionProperties {
		IEnumerable<ICommand<T>> Commands { get; }
	}
	public interface IDocumentAction :
		IUIAction<Document> {
	}
	public interface IDocumentCheckAction : IDocumentAction,
		IUICheckAction<Document> {
	}
	public interface IContentContainerAction :
		IUIAction<IContentContainer> {
	}
	public interface IContentContainerActionList :
		IUIActionList<IContentContainer> {
	}
	public interface IUIPopupControlAction<T> : IUIAction<T>, IUIActionProperties {
		System.Windows.Forms.Control Control { get; }
	}
	public interface IUIPopupMenuAction<T> : IUIAction<T>, IUIActionProperties {
		IList<IUIAction<T>> Actions { get; }
		System.Windows.Forms.Orientation Orientation { get; }
	}
	public interface IContentContainerPopupMenuAction : IUIPopupMenuAction<IContentContainer>, IContentContainerAction { }
	public interface IContentContainerPopupControlAction : IUIPopupControlAction<IContentContainer>, IContentContainerAction { }
	public sealed class DelegateCommand<T> : ICommand<T> {
		Predicate<T> canExecuteCore;
		Action<T> executeCore;
		public DelegateCommand(Predicate<T> canExecute, Action<T> execute) {
			canExecuteCore = canExecute;
			executeCore = execute;
		}
		public bool CanExecute(T parameter) {
			return (canExecuteCore != null) && canExecuteCore(parameter);
		}
		public void Execute(T parameter) {
			if(executeCore != null)
				executeCore(parameter);
		}
	}
	public abstract class UIActionPropertiesCore : IUIActionProperties {
		public string Caption { get; set; }
		public Image Image { get; set; }
		public string Description { get; set; }
	}
	public abstract class DelegateActionCore<T> : UIActionPropertiesCore, IUIAction<T> {
		public DelegateActionCore(Predicate<T> canExecute, Action<T> execute) {
			Command = new DelegateCommand<T>(canExecute, execute);
		}
		public ICommand<T> Command { get; private set; }
	}
}
