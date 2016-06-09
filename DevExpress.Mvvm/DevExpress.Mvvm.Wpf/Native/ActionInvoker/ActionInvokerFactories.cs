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
	public class StrongReferenceActionInvokerFactory : IActionInvokerFactory {
		IActionInvoker IActionInvokerFactory.CreateActionInvoker<TMessage>(object recipient, Action<TMessage> action) {
			return new StrongReferenceActionInvoker<TMessage>(recipient, action);
		}
	}
	public class WeakReferenceActionInvokerFactory : IActionInvokerFactory {
		IActionInvoker IActionInvokerFactory.CreateActionInvoker<TMessage>(object recipient, Action<TMessage> action) {
#if !NETFX_CORE
			if(action.Method.IsStatic)
#else
			if(action.GetMethodInfo().IsStatic)
#endif
				return new StrongReferenceActionInvoker<TMessage>(recipient, action);
#if SILVERLIGHT
			if(ShouldStoreActionItself(action))
				return new SLWeakReferenceActionInvoker<TMessage>(recipient, action);
#endif
			return new WeakReferenceActionInvoker<TMessage>(recipient, action);
		}
#if SILVERLIGHT
		static bool ShouldStoreActionItself(Delegate action) {
			if(!action.Method.IsPublic)
				return true;
			if(action.Target != null && !action.Target.GetType().IsPublic && !action.Target.GetType().IsNestedPublic)
				return true;
			var name = action.Method.Name;
			if(name.Contains("<") && name.Contains(">"))
				return true;
			return false;
		}
#endif
	}
}
