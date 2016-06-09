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
using System.Linq.Expressions;
using System.Windows;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Utils {
	public delegate void DependencyPropertyRegistratorChangedCallback<in TOwnerType, in TPropertyType>(TOwnerType d, TPropertyType oldValue, TPropertyType newValue) where TOwnerType : DependencyObject;
	public delegate object DependencyPropertyRegistratorCoerceCallback<in TOwnerType, in TPropertyType>(TOwnerType d, TPropertyType value) where TOwnerType : DependencyObject;
	public static class DependencyPropertyRegistrator {
		public static DependencyProperty Register<TOwnerType, TPropertyType>(Expression<Func<TOwnerType, TPropertyType>> expression)
			where TOwnerType : DependencyObject {
			return Register(expression, default(TPropertyType));
		}
		public static DependencyProperty Register<TOwnerType, TPropertyType>(Expression<Func<TOwnerType, TPropertyType>> expression, TPropertyType defaultValue)
			where TOwnerType : DependencyObject {
			return DependencyProperty.Register(GetMemberName(expression), typeof(TPropertyType), typeof(TOwnerType),
				new FrameworkPropertyMetadata(defaultValue, FrameworkPropertyMetadataOptions.None));
		}
		public static DependencyProperty Register<TOwnerType, TPropertyType>(Expression<Func<TOwnerType, TPropertyType>> expression, TPropertyType defaultValue, DependencyPropertyRegistratorChangedCallback<TOwnerType, TPropertyType> changedCallback)
			where TOwnerType : DependencyObject {
			return DependencyProperty.Register(GetMemberName(expression), typeof(TPropertyType), typeof(TOwnerType),
				new FrameworkPropertyMetadata(defaultValue, FrameworkPropertyMetadataOptions.None,
					(o, args) => changedCallback.Do(x => x((TOwnerType)o, (TPropertyType)args.OldValue, (TPropertyType)args.NewValue))));
		}
		public static DependencyProperty Register<TOwnerType, TPropertyType>(Expression<Func<TOwnerType, TPropertyType>> expression, TPropertyType defaultValue, DependencyPropertyRegistratorChangedCallback<TOwnerType, TPropertyType> changedCallback, DependencyPropertyRegistratorCoerceCallback<TOwnerType, TPropertyType> coerceCallback)
			where TOwnerType : DependencyObject {
			return DependencyProperty.Register(GetMemberName(expression), typeof(TPropertyType), typeof(TOwnerType),
				new FrameworkPropertyMetadata(defaultValue, FrameworkPropertyMetadataOptions.None,
					(o, args) => changedCallback.Do(x => x((TOwnerType)o, (TPropertyType)args.OldValue, (TPropertyType)args.NewValue)),
					(o, value) => coerceCallback.With(x => x((TOwnerType)o, (TPropertyType)value))));
		}
		static string GetMemberName<TOwner, TProperty>(Expression<Func<TOwner, TProperty>> propertyExpression) {
			var me = propertyExpression.Body as MemberExpression ?? (propertyExpression.Body as UnaryExpression).With(x => x.Operand as MemberExpression);
			return me.With(x => x.Expression as ParameterExpression).If(x => x.Name == propertyExpression.Parameters[0].Name).Return(x => me.Member.Name, null);
		}
		public static DependencyPropertyKey RegisterReadOnly<TOwnerType, TPropertyType>(Expression<Func<TOwnerType, TPropertyType>> expression, TPropertyType defaultValue, DependencyPropertyRegistratorChangedCallback<TOwnerType, TPropertyType> changedCallback = null)
			where TOwnerType : DependencyObject {
			return DependencyProperty.RegisterReadOnly(GetMemberName(expression), typeof(TPropertyType), typeof(TOwnerType),
				new FrameworkPropertyMetadata(defaultValue, FrameworkPropertyMetadataOptions.None,
					(o, args) => changedCallback.Do(x => x((TOwnerType)o, (TPropertyType)args.OldValue, (TPropertyType)args.NewValue))));
		}
	}
}
