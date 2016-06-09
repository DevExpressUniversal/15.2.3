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
	public abstract class ActionInvokerBase : IActionInvoker {
		WeakReference targetReference;
		public ActionInvokerBase(object target) {
			targetReference = target == null ? null : new WeakReference(target);
		}
		public object Target { get { return targetReference.With(x => x.Target); } }
		void IActionInvoker.ExecuteIfMatched(Type messageTargetType, object parameter) {
			object target = Target;
			if(target == null) return;
			if(messageTargetType == null || messageTargetType.IsAssignableFrom(target.GetType())) {
				Execute(parameter);
			}
		}
		void IActionInvoker.ClearIfMatched(Delegate action, object recipient) {
			object target = Target;
			if(recipient != target)
				return;
#if !NETFX_CORE
			if(action != null && action.Method.Name != MethodName) 
#else
			if(action != null && action.GetMethodInfo().Name != MethodName)
#endif
				return;
			targetReference = null;
			ClearCore();
		}
		protected abstract string MethodName { get; }
		protected abstract void Execute(object parameter);
		protected abstract void ClearCore();
	}
}
