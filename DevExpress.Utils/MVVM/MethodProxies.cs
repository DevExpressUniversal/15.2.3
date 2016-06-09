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

namespace DevExpress.Utils.MVVM {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	class ProxyBase {
		ParameterInfo[] mInfoParameters;
		public ProxyBase(ParameterInfo[] mInfoParameters) {
			this.mInfoParameters = mInfoParameters;
		}
		protected Expression[] CreateCallParameters(ParameterExpression paramsExpression) {
			Expression[] callParameters = new Expression[mInfoParameters.Length];
			for(int i = 0; i < callParameters.Length; i++) {
				var p = mInfoParameters[i];
				if(!p.IsOptional) {
					var ppi = Expression.ArrayIndex(paramsExpression, Expression.Constant(i));
					callParameters[i] = Expression.Convert(ppi, mInfoParameters[i].ParameterType);
				}
				else callParameters[i] = Expression.Constant(p.DefaultValue);
			}
			return callParameters;
		}
		protected bool MatchCore(object[] parameters) {
			int length = mInfoParameters.Length;
			var parameterTypes = GetParameterTypes(length);
			while(true) {
				if(parameters.SequenceEqual(parameterTypes, ParameterTypesComparer))
					return true;
				if(length > 0)
					parameterTypes = GetParameterTypes(--length);
				else break;
			}
			return false;
		}
		IEnumerable<Type> GetParameterTypes(int length) {
			return mInfoParameters.Where((p, index) => !p.IsOptional || index < length).Select(p => p.ParameterType);
		}
		internal static IEqualityComparer<object> ParameterTypesComparer {
			get { return DefalultParameterTypesComparer.Instance; }
		}
		sealed class DefalultParameterTypesComparer : IEqualityComparer<object> {
			internal static IEqualityComparer<object> Instance = new DefalultParameterTypesComparer();
			bool IEqualityComparer<object>.Equals(object parameter, object type) {
				return parameter != null ? ((Type)type).IsAssignableFrom(parameter.GetType()) : ((Type)type).IsClass;
			}
			int IEqualityComparer<object>.GetHashCode(object obj) {
				throw new NotImplementedException();
			}
		}
		internal static T[] Reduce<T>(T[] parameters) {
			T[] result = new T[parameters.Length - 1];
			Array.Copy(parameters, result, result.Length);
			return result;
		}
	}
}
