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

using System.Windows;
using System;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core;
using DevExpress.Data.Utils;
namespace DevExpress.Xpf.Editors {
	public class ColumnContentChangedEventArgs : EventArgs {
		public ColumnContentChangedEventArgs(DependencyProperty property) {
			this.property = property;
		}
#if DEBUGTEST
		[IgnoreDependencyPropertiesConsistencyChecker]
#endif
		readonly DependencyProperty property;
		public DependencyProperty Property { get { return property; } }
	}
	public delegate void ColumnContentChangedEventHandler(object sender, ColumnContentChangedEventArgs e);
	public class ColumnContentChangedEventHandler<TOwner> : WeakEventHandler<TOwner, ColumnContentChangedEventArgs, ColumnContentChangedEventHandler> where TOwner : class {
	static Func<WeakEventHandler<TOwner, ColumnContentChangedEventArgs, ColumnContentChangedEventHandler>, ColumnContentChangedEventHandler> action = h => h.OnEvent;
		public ColumnContentChangedEventHandler(TOwner owner, Action<TOwner, object, ColumnContentChangedEventArgs> onEventAction, Action<WeakEventHandler<TOwner, ColumnContentChangedEventArgs, ColumnContentChangedEventHandler>, object> unsubscribe)
			: base(owner, onEventAction, unsubscribe, action) {
		}
	}
	public class InnerContentChangedEventHandler<TOwner> : WeakEventHandler<TOwner, EventArgs, EventHandler> where TOwner : class {
	static Action<WeakEventHandler<TOwner, EventArgs, EventHandler>, object> unsibscribe = (h, o) => ((INotifyContentChanged)o).ContentChanged -= h.Handler;
	static Func<WeakEventHandler<TOwner, EventArgs, EventHandler>, EventHandler> action = h => h.OnEvent;
		public InnerContentChangedEventHandler(TOwner owner, Action<TOwner, object, EventArgs> onEventAction)
			: base(owner, onEventAction, unsibscribe, action) {
		}
	}
}
