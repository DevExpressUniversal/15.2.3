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
using System.ComponentModel;
using System.Linq.Expressions;
#if !FREE && !NETFX_CORE
using System.Windows.Threading;
#endif
namespace DevExpress.Mvvm.Native {
#if !FREE && !NETFX_CORE
	public static class PropertyChangedTracker {
		public static PropertyChangedTracker<T, TProperty> GetPropertyChangedTracker<T, TProperty>(this T obj, Expression<Func<T, TProperty>> propertyExpression, Action changedCallBack)
			where T : class {
			return new PropertyChangedTracker<T, TProperty>(obj, propertyExpression, changedCallBack);
		}
	}
	public class PropertyChangedTracker<T, TProperty> where T : class {
		readonly T obj;
		readonly Func<T, TProperty> propertyAccessor;
		readonly Dispatcher dispatcher;
		readonly string propertyName;
		readonly Action changedCallBack;
		public PropertyChangedTracker(T obj, Expression<Func<T, TProperty>> propertyExpression, Action changedCallBack) {
			this.obj = obj;
			this.propertyName = BindableBase.GetPropertyNameFast(propertyExpression);
			this.propertyAccessor = propertyExpression.Compile();
			this.dispatcher = Dispatcher.CurrentDispatcher;
			this.changedCallBack = changedCallBack;
			((INotifyPropertyChanged)obj).PropertyChanged += OnPropertyChanged;
			UpdateValue();
		}
		public TProperty Value { get; private set; }
		public int ChangeCount { get; private set; }
		void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
			if(propertyName != e.PropertyName)
				return;
			dispatcher.VerifyAccess();
			ChangeCount++;
			UpdateValue();
			changedCallBack();
		}
		void UpdateValue() {
			Value = propertyAccessor(obj);
		}
	}
#endif
}
