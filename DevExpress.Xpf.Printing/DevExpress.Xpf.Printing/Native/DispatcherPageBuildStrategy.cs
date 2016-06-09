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
using System.Windows.Threading;
using DevExpress.Utils;
using DevExpress.XtraPrinting.Native;
using System.Windows;
namespace DevExpress.Xpf.Printing.Native {
	public class DispatcherPageBuildStrategy : BackgroundPageBuildEngineStrategy {
		int subscribersCount;
		event EventHandler tick;
#if SILVERLIGHT
		DispatcherTimer idleTimer;
		public DispatcherPageBuildStrategy() {
			idleTimer = new DispatcherTimer();
			idleTimer.Tick += (o, e) => OnIdle();
		}
#endif
		public override void BeginInvoke(Action0 method) {
			method();		
		}
		public override event EventHandler Tick {
			add {
				tick += value;
				subscribersCount++;
				InvokeOnIdle();
			}
			remove {
				tick -= value;
				if(subscribersCount > 0)
					subscribersCount--;
			}
		}
		void InvokeOnIdle() {
#if !SL
			Dispatcher.CurrentDispatcher.BeginInvoke(new Action(OnIdle), DispatcherPriority.ApplicationIdle);
#else
			idleTimer.Start();
#endif
		}
		void OnIdle() {
#if SILVERLIGHT
			idleTimer.Stop();
#endif
			if(subscribersCount > 0) {
				if(tick != null) {
					tick(this, null);
				}
				InvokeOnIdle();
			}
		}
	}
}
