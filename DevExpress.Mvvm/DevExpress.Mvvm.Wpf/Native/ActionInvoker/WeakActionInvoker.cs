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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DevExpress.Mvvm.Native;
#if !NETFX_CORE
using System.Windows.Threading;
#endif
namespace DevExpress.Mvvm.Native {
	public abstract class WeakReferenceActionInvokerBase : ActionInvokerBase {
		public WeakReferenceActionInvokerBase(object target, Delegate action)
			: base(target) {
#if !NETFX_CORE
				ActionMethod = action.Method;
#else
				ActionMethod = action.GetMethodInfo();
#endif
				ActionTargetReference = new WeakReference(action.Target);
		}
		protected MethodInfo ActionMethod { get; private set; }
		protected WeakReference ActionTargetReference { get; private set; }
		protected override string MethodName { get { return ActionMethod.Name; } }
		protected override void ClearCore() {
			ActionTargetReference = null;
			ActionMethod = null;
		}
	}
	public class WeakReferenceActionInvoker<T> : WeakReferenceActionInvokerBase {
		public WeakReferenceActionInvoker(object target, Action<T> action)
			: base(target, action) {
		}
		protected override void Execute(object parameter) {
			if(ActionMethod != null && ActionTargetReference.IsAlive) {
				ActionMethod.Invoke(ActionTargetReference.Target, new object[] { (T)parameter });
			}
		}
	}
	public class WeakReferenceActionInvoker : WeakReferenceActionInvokerBase {
		public WeakReferenceActionInvoker(object target, Action action)
			: base(target, action) {
		}
		protected override void Execute(object parameter) {
			if(ActionMethod != null && ActionTargetReference.IsAlive) {
				ActionMethod.Invoke(ActionTargetReference.Target, null);
			}
		}
	}
}
