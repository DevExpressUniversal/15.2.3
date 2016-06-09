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
using System.ComponentModel;
using System.Linq;
using System.Text;
using DevExpress.Data.Utils;
using DevExpress.XtraReports.UI;
namespace DevExpress.Xpf.Reports.UserDesigner.Native {
	public class BandHeightChangedWeakEventHandler<T> : WeakEventHandler<T, BandEventArgs, BandEventHandler> where T : class {
		static Action<WeakEventHandler<T, BandEventArgs, BandEventHandler>, object> onDetachAction = (h, o) => ((XtraReport)o).BandHeightChanged -= h.Handler;
		static Func<WeakEventHandler<T, BandEventArgs, BandEventHandler>, BandEventHandler> createHandlerFunction = h => h.OnEvent;
		public BandHeightChangedWeakEventHandler(T owner, Action<T, object, BandEventArgs> onEventAction)
			: base(owner, onEventAction, onDetachAction, createHandlerFunction) {
		}
	}
	public class XRControlCollectionChangedWeakEventHandler<T> : WeakEventHandler<T, CollectionChangeEventArgs, CollectionChangeEventHandler> where T : class {
		static Action<WeakEventHandler<T, CollectionChangeEventArgs, CollectionChangeEventHandler>, object> onDetachAction = (h, o) => ((XRControlCollection)o).CollectionChanged -= h.Handler;
		static Func<WeakEventHandler<T, CollectionChangeEventArgs, CollectionChangeEventHandler>, CollectionChangeEventHandler> createHandlerFunction = h => h.OnEvent;
		public XRControlCollectionChangedWeakEventHandler(T owner, Action<T, object, CollectionChangeEventArgs> onEventAction)
			: base(owner, onEventAction, onDetachAction, createHandlerFunction) {
		}
	}
	public class XRControlPropertyChangedWeakEventHandler<T> : WeakEventHandler<T, PropertyChangedEventArgs, PropertyChangedEventHandler> where T : class {
		static Action<WeakEventHandler<T, PropertyChangedEventArgs, PropertyChangedEventHandler>, object> onDetachAction = (h, o) => ((IObjectTracker)o).ObjectPropertyChanged -= h.Handler;
		static Func<WeakEventHandler<T, PropertyChangedEventArgs, PropertyChangedEventHandler>, PropertyChangedEventHandler> createHandlerFunction = h => h.OnEvent;
		public XRControlPropertyChangedWeakEventHandler(T owner, Action<T, object, PropertyChangedEventArgs> onEventAction)
			: base(owner, onEventAction, onDetachAction, createHandlerFunction) {
		}
	}
	public class XRControlParentChangedWeakEventHandler<T> : WeakEventHandler<T, ChangeEventArgs, ChangeEventHandler> where T : class {
		static Action<WeakEventHandler<T, ChangeEventArgs, ChangeEventHandler>, object> onDetachAction = (h, o) => ((XRControl)o).ParentChanged -= h.Handler;
		static Func<WeakEventHandler<T, ChangeEventArgs, ChangeEventHandler>, ChangeEventHandler> createHandlerFunction = h => h.OnEvent;
		public XRControlParentChangedWeakEventHandler(T owner, Action<T, object, ChangeEventArgs> onEventAction)
			: base(owner, onEventAction, onDetachAction, createHandlerFunction) {
		}
	}
}
