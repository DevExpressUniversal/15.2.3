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
namespace DevExpress.XtraScheduler.Native {
	public class SchedulerCancellationTokenSource : IDisposable {
		CancellationTokenSource ts;
		public SchedulerCancellationTokenSource() {
			ts = CreateSource();
		}
		public virtual bool IsCancellationRequested {
			get { return ts.IsCancellationRequested; }
		}
		public virtual CancellationToken Token {
			get { return ts.Token; }
		}
		public virtual void Cancel() {
			ts.Cancel();
		}
		public virtual void Dispose() {
			ts.Dispose();
		}
		protected virtual CancellationTokenSource CreateSource() {
			return new CancellationTokenSource();
		}
	}
	public class NullSchedulerCancellationTokenSource : SchedulerCancellationTokenSource {
		CancellationToken t = new CancellationToken(true);
		public override bool IsCancellationRequested {
			get { return true; }
		}
		public override CancellationToken Token {
			get { return t; }
		}
		public override void Cancel() {
		}
		public override void Dispose() {
		}
		protected override CancellationTokenSource CreateSource() {
			return null;
		}
	}
}
