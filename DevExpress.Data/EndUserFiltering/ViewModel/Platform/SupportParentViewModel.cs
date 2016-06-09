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

namespace DevExpress.Utils.Filtering.Internal {
	using System;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	static class @SupportParentViewModel {
		internal static object @GetParentViewModel(this object @this) {
			return @this
					.@Get(x => GetGetParentViewModel(x.GetType()))
					.@Get(getProp => getProp(@this));
		}
		internal static void @SetParentViewModel(this object @this, object parentViewModel) {
			@this
				.@Get(x => GetSetParentViewModel(x.GetType()))
				.@Do(setProp => setProp(@this, parentViewModel));
		}
		static IDictionary<string, Func<object, object>> accessorsCache = new Dictionary<string, Func<object, object>>();
		static Func<object, object> GetGetParentViewModel(Type type) {
			Func<object, object> getProp;
			if(!accessorsCache.TryGetValue(type.FullName, out getProp)) {
				var getMethod = MethodInfoHelper.GetMethodInfo(type, "get_ParentViewModel");
				if(getMethod != null) {
					var pSource = Expression.Parameter(typeof(object), "source");
					getProp = Expression.Lambda<Func<object, object>>(
								Expression.Call(Expression.Convert(pSource, getMethod.DeclaringType), getMethod),
								pSource
						).Compile();
					accessorsCache.Add(type.FullName, getProp);
				}
				else accessorsCache.Add(type.FullName, null);
			}
			return getProp;
		}
		static IDictionary<string, Action<object, object>> mutatorsCache = new Dictionary<string, Action<object, object>>();
		static Action<object, object> GetSetParentViewModel(Type type) {
			Action<object, object> setProp;
			if(!mutatorsCache.TryGetValue(type.FullName, out setProp)) {
				var setMethod = MethodInfoHelper.GetMethodInfo(type, "set_ParentViewModel", new Type[] { typeof(object) });
				if(setMethod != null) {
					var pSource = Expression.Parameter(typeof(object), "source");
					var pValue = Expression.Parameter(typeof(object), "value");
					setProp = Expression.Lambda<Action<object, object>>(
								Expression.Call(Expression.Convert(pSource, setMethod.DeclaringType), setMethod, pValue),
								pSource, pValue
						).Compile();
					mutatorsCache.Add(type.FullName, setProp);
				}
				else mutatorsCache.Add(type.FullName, null);
			}
			return setProp;
		}
	}
}
