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
using System.ComponentModel;
using System.Linq.Expressions;
using DevExpress.Mvvm.Native;
#if NETFX_CORE
using System.Runtime.CompilerServices;
#endif
namespace DevExpress.Mvvm {
	[System.Runtime.Serialization.DataContract]
	public abstract class BindableBase : INotifyPropertyChanged {
		public static string GetPropertyName<T>(Expression<Func<T>> expression) {
			return GetPropertyNameFast(expression);
		}
		internal static string GetPropertyNameFast(LambdaExpression expression) {
			MemberExpression memberExpression = expression.Body as MemberExpression;
			if(memberExpression == null) {
				throw new ArgumentException("MemberExpression is expected in expression.Body", "expression");
			}
			const string vblocalPrefix = "$VB$Local_";
			var member = memberExpression.Member;
			if(
#if !NETFX_CORE
				member.MemberType == System.Reflection.MemberTypes.Field &&
#endif
				member.Name != null &&
				member.Name.StartsWith(vblocalPrefix))
				return member.Name.Substring(vblocalPrefix.Length);
			return member.Name;
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected T GetProperty<T>(Expression<Func<T>> expression) {
			return PropertyManager.GetProperty<T>(GetPropertyName(expression));
		}
		protected bool SetProperty<T>(ref T storage, T value, string propertyName, Action changedCallback) {
			return PropertyManager.SetProperty<T>(ref storage, value, propertyName, () => CallChangedCallBackAndRaisePropertyChanged(propertyName, changedCallback));
		}
		protected bool SetProperty<T>(ref T storage, T value, Expression<Func<T>> expression, Action changedCallback) {
			return this.SetProperty(ref storage, value, GetPropertyName(expression), changedCallback);
		}
		protected bool SetProperty<T>(ref T storage, T value, Expression<Func<T>> expression) {
			return this.SetProperty<T>(ref storage, value, expression, null);
		}
#if !NETFX_CORE
		protected bool SetProperty<T>(ref T storage, T value, string propertyName) {
#else
		protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null) {
#endif
			return this.SetProperty<T>(ref storage, value, propertyName, null);
		}
#if NETFX_CORE
		protected bool SetProperty<T>(ref T storage, T value, Action<T, T> changedCallback, [CallerMemberName] string propertyName = null) {
			if(object.Equals(storage, value)) return false;
			T oldValue = storage;
			storage = value;
			if(changedCallback != null)
				changedCallback(oldValue, value);
			RaisePropertiesChanged(propertyName);
			return true;
		}
#endif
		protected bool SetProperty<T>(Expression<Func<T>> expression, T value) {
			return this.SetProperty(expression, value, null);
		}
		protected bool SetProperty<T>(Expression<Func<T>> expression, T value, Action changedCallback) {
			string propertyName = GetPropertyName(expression);
			return PropertyManager.SetProperty<T>(propertyName, value, () => CallChangedCallBackAndRaisePropertyChanged(propertyName, changedCallback));
		}
		void CallChangedCallBackAndRaisePropertyChanged(string propertyName, Action changedCallback) {
			RaisePropertyChanged(propertyName);
			if(changedCallback != null)
				changedCallback();
		}
#if !NETFX_CORE
		protected void RaisePropertyChanged(string propertyName) {
#else
		protected void RaisePropertyChanged([CallerMemberName] string propertyName = null) {
#endif
			PropertyChanged.Do(x => x(this, new PropertyChangedEventArgs(propertyName)));
		}
#if !NETFX_CORE
		protected void RaisePropertyChanged() {
			RaisePropertiesChanged(null);
		}
#endif
		protected void RaisePropertyChanged<T>(Expression<Func<T>> expression) {
			RaisePropertyChanged(GetPropertyName(expression));
		}
		protected void RaisePropertiesChanged(params string[] propertyNames) {
			if(propertyNames == null) {
				RaisePropertyChanged(string.Empty);
				return;
			}
			foreach(string propertyName in propertyNames) {
				RaisePropertyChanged(propertyName);
			}
		}
		protected void RaisePropertiesChanged<T1, T2>(Expression<Func<T1>> expression1, Expression<Func<T2>> expression2) {
			RaisePropertyChanged(expression1);
			RaisePropertyChanged(expression2);
		}
		protected void RaisePropertiesChanged<T1, T2, T3>(Expression<Func<T1>> expression1, Expression<Func<T2>> expression2, Expression<Func<T3>> expression3) {
			RaisePropertyChanged(expression1);
			RaisePropertyChanged(expression2);
			RaisePropertyChanged(expression3);
		}
		protected void RaisePropertiesChanged<T1, T2, T3, T4>(Expression<Func<T1>> expression1, Expression<Func<T2>> expression2, Expression<Func<T3>> expression3, Expression<Func<T4>> expression4) {
			RaisePropertyChanged(expression1);
			RaisePropertyChanged(expression2);
			RaisePropertyChanged(expression3);
			RaisePropertyChanged(expression4);
		}
		protected void RaisePropertiesChanged<T1, T2, T3, T4, T5>(Expression<Func<T1>> expression1, Expression<Func<T2>> expression2, Expression<Func<T3>> expression3, Expression<Func<T4>> expression4, Expression<Func<T5>> expression5) {
			RaisePropertyChanged(expression1);
			RaisePropertyChanged(expression2);
			RaisePropertyChanged(expression3);
			RaisePropertyChanged(expression4);
			RaisePropertyChanged(expression5);
		}
		PropertyManager propertyManager;
		internal PropertyManager PropertyManager { get { return propertyManager ?? (propertyManager = new PropertyManager()); } }
	}
}
namespace DevExpress.Mvvm.Native {
	public class PropertyManager {
		internal Dictionary<string, object> propertyBag = new Dictionary<string, object>();
		public bool SetProperty<T>(string propertyName, T value, Action changedCallback) {
			T currentValue = default(T);
			object val;
			if(propertyBag.TryGetValue(propertyName, out val))
				currentValue = (T)val;
			if(CompareValues<T>(currentValue, value))
				return false;
			propertyBag[propertyName] = value;
			changedCallback.Do(x => x());
			return true;
		}
		public T GetProperty<T>(string propertyName) {
			object val;
			if(propertyBag.TryGetValue(propertyName, out val))
				return (T)val;
			return default(T);
		}
		public bool SetProperty<T>(ref T storage, T value, string propertyName, Action changedCallback) {
			if(PropertyManager.CompareValues<T>(storage, value))
				return false;
			T oldValue = storage;
			storage = value;
			changedCallback.Do(x => x());
			return true;
		}
		static bool CompareValues<T>(T storage, T value) {
			return object.Equals(storage, value);
		}
	}
}
