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
using System.Threading;
using System.Threading.Tasks;
namespace DevExpress.DataAccess.Native {
	public abstract class PresenterBase<TModel, TView, TPresenter> where TView : IView<TPresenter> where TPresenter : PresenterBase<TModel, TView, TPresenter>  {
		readonly TView view;
		readonly TModel model;
		TaskCompletionSource<bool> tcs;
		protected PresenterBase(TModel model, TView view) {
			this.model = model;
			this.view = view;
		}
		protected TModel Model { get { return this.model; } }
		protected TView View { get { return this.view; } }
		public void InitView() {
			this.view.BeginUpdate();
			this.view.RegisterPresenter((TPresenter)this);
			InitViewCore();
			this.view.EndUpdate();
		}
		protected virtual Task<bool> DoAsync(CancellationToken cancellationToken) {
			this.tcs = new TaskCompletionSource<bool>();
			this.view.Ok += OnOk;
			this.view.Cancel += OnCancel;
			cancellationToken.Register(() => {
				StopView();
				this.tcs.TrySetCanceled();
			});
			this.view.Start();
			return this.tcs.Task;
		}
		void OnCancel(object s, EventArgs e) {
			StopView();
			this.tcs.TrySetResult(false);
		}
		void OnOk(object s, EventArgs e) {
			string error = Validate();
			if(error == null) {
				StopView();
				this.tcs.TrySetResult(true);
			}
			else
				this.view.Warning(error);
		}
		void StopView() {
			this.view.Ok -= OnOk;
			this.view.Cancel -= OnCancel;
			this.view.Stop();
		}
		public bool Do() {
			Task<bool> task = DoAsync(CancellationToken.None);
			task.Wait();
			return task.Result;
		}
		protected abstract void InitViewCore();
		protected virtual string Validate() { return null; }
	}
}
