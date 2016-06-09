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
using System.Linq.Expressions;
using DevExpress.Utils;
namespace DevExpress.XtraReports.Service {
	public class DelegateExtensionsResolver : IExtensionsResolver {
		static readonly Type[] GetAllInstancesMethodArguments = { typeof(Type) };
		public static DelegateExtensionsResolver FromServiceLocator(object serviceLocator) {
			var methodGetAllInstances = serviceLocator.GetType().GetMethod("GetAllInstances", GetAllInstancesMethodArguments);
			Guard.ArgumentNotNull(methodGetAllInstances, "serviceLocator.GetAllInstances");
			var parameter = Expression.Parameter(typeof(Type));
			var instance = Expression.Constant(serviceLocator);
			var call = Expression.Call(instance, methodGetAllInstances, parameter);
			var funcGetAllInstances = Expression.Lambda<Func<Type, IEnumerable<object>>>(call, parameter).Compile();
			return new DelegateExtensionsResolver(funcGetAllInstances);
		}
		readonly Func<Type, IEnumerable<object>> resolve;
		public DelegateExtensionsResolver(Func<Type, IEnumerable<object>> resolve) {
			Guard.ArgumentNotNull(resolve, "resolve");
			this.resolve = resolve;
		}
		#region IExtensionsResolver Members
		public IEnumerable<T> GetExtensions<T>() {
			return resolve(typeof(T)).Cast<T>();
		}
		#endregion
	}
}
