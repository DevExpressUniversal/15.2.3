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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
namespace DevExpress.Xpf.Core.Design {
	public static class TaskHelper {
		public static Task<T> FromResult<T>(T result) { return Task<T>.Factory.StartNew(() => result); }
		public static void SetAsyncResult<T>(this TaskCompletionSource<T> taskSource, Exception exception, Func<T> result) {
			if(exception == null)
				taskSource.SetResult(result());
			else
				taskSource.SetException(exception);
		}
		public static Task<TResult> WithFunc<TSource, TResult>(this Task<TSource> task, Func<TSource, TResult> evaluator) {
			return ContinueTaskWithScheduler(task, t => evaluator(t.Result));
		}
		public static Task WithFunc<TSource>(this Task<TSource> task, Action<TSource> evaluator) {
			return ContinueTaskWithScheduler(task, t => evaluator(t.Result));
		}
		public static Task<TResult> WithTask<TSource, TResult>(this Task<TSource> task, Func<TSource, Task<TResult>> evaluator) {
			TaskCompletionSource<TResult> resultTaskSource = new TaskCompletionSource<TResult>();
			ContinueTaskWithScheduler(task, t => {
				if(t.Exception != null)
					resultTaskSource.SetException(t.Exception);
				else {
					ContinueTaskWithScheduler(evaluator(t.Result), a => resultTaskSource.SetAsyncResult(a.Exception, () => a.Result));
				}
			});
			return resultTaskSource.Task;
		}
		public static TResult WaitResult<TResult>(this Task<TResult> task) {
			task.Wait();
			return task.Result;
		}
		static Task<TResult> ContinueTaskWithScheduler<TSource, TResult>(Task<TSource> task, Func<Task<TSource>, TResult> continuationFunc) {
			return SynchronizationContext.Current == null ? task.ContinueWith(continuationFunc) : task.ContinueWith(continuationFunc, TaskScheduler.FromCurrentSynchronizationContext());
		}
		static Task ContinueTaskWithScheduler<TSource>(Task<TSource> task, Action<Task<TSource>> continuationAction) {
			return SynchronizationContext.Current == null ? task.ContinueWith(continuationAction) : task.ContinueWith(continuationAction, TaskScheduler.FromCurrentSynchronizationContext());
		}
	}
}
