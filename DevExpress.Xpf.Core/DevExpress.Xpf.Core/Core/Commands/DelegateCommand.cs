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
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
#if SLDESIGN
namespace DevExpress.Xpf.Core.Design.CoreUtils.Commands {
#else
namespace DevExpress.Xpf.Core.Commands {
#endif
#if !SL || SLDESIGN
	internal interface IDispatcherInfo {
		Dispatcher Dispatcher { get; }
	}
	public partial class DelegateCommand<T> : IDispatcherInfo {
		readonly Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
		#region IDispatcherInfo Members
		public Dispatcher Dispatcher {
			get { return dispatcher; }
		}
		#endregion
	}
#endif
	[Obsolete("This class is obsolete. Use the DevExpress.Mvvm.DelegateCommand instead.")]
	public partial class DelegateCommand<T> : ICommand {
		private readonly Action<T> executeMethod = null;
		private readonly Func<T, bool> canExecuteMethod = null;
		public DelegateCommand(Action<T> executeMethod)
			: this(executeMethod, null) {
		}
		public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod) {
			if(executeMethod == null && canExecuteMethod == null)
				throw new ArgumentNullException("executeMethod");
			this.executeMethod = executeMethod;
			this.canExecuteMethod = canExecuteMethod;
		}
		public bool CanExecute(T parameter) {
			if(canExecuteMethod == null) return true;
			return canExecuteMethod(parameter);
		}
		public void Execute(T parameter) {
			if(executeMethod == null) return;
			executeMethod(parameter);
		}
		bool ICommand.CanExecute(object parameter) {
			return CanExecute((T)parameter);
		}
#if SL
		public event EventHandler CanExecuteChanged;
#else
		private List<WeakReference> _canExecuteChangedHandlers;
		public event EventHandler CanExecuteChanged {
			add {
				WeakEventHandlerManager.AddWeakReferenceHandler(ref _canExecuteChangedHandlers, value, 2);
			}
			remove {
				WeakEventHandlerManager.RemoveWeakReferenceHandler(_canExecuteChangedHandlers, value);
			}
		}
#endif
		void ICommand.Execute(object parameter) {
			Execute((T)parameter);
		}
		protected virtual void OnCanExecuteChanged() {
#if SL
			if(CanExecuteChanged != null)
				CanExecuteChanged(this, EventArgs.Empty);
#else
			WeakEventHandlerManager.CallWeakReferenceHandlers(this, _canExecuteChangedHandlers);
#endif
		}
		public void RaiseCanExecuteChanged() {
			OnCanExecuteChanged();
		}
	}
}
