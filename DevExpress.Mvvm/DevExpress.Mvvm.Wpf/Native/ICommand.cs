#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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

using DevExpress.Mvvm.Native;
using System;
using System.Threading;
using System.Windows.Input;
namespace DevExpress.Mvvm.Native {
	public interface IDelegateCommand : ICommand {
		void RaiseCanExecuteChanged();
	}
	public interface IAsyncCommand : IDelegateCommand {
		bool IsExecuting { get; }
		[Obsolete("Use the IsCancellationRequested property instead.")]
		bool ShouldCancel { get; }
		CancellationTokenSource CancellationTokenSource { get; }
		bool IsCancellationRequested { get; }
		ICommand CancelCommand { get; }
		void Wait(TimeSpan timeout);
	}
}
namespace DevExpress.Mvvm {
	public static class IAsyncCommandExtensions {
		public static void Wait(this IAsyncCommand service) {
			VerifyService(service);
			service.Wait(TimeSpan.FromMilliseconds(-1));
		}
		static void VerifyService(IAsyncCommand service) {
			if(service == null) throw new ArgumentNullException("service");
		}
	}
}
