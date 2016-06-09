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
using System.Windows;
using System.Windows.Data;
using DevExpress.Mvvm.Native;
using System.Globalization;
namespace DevExpress.Mvvm.UI {
	public class FunctionBindingBehavior : FunctionBindingBehaviorBase {
		#region Dependency Properties
		public static readonly DependencyProperty PropertyProperty = 
			 DependencyProperty.Register("Property", typeof(string), typeof(FunctionBindingBehavior),
			 new PropertyMetadata(null, (d, e) => ((FunctionBindingBehavior)d).OnResultAffectedPropertyChanged()));
		public static readonly DependencyProperty ConverterProperty = 
			DependencyProperty.Register("Converter", typeof(IValueConverter), typeof(FunctionBindingBehavior),
			new PropertyMetadata(null, (d, e) => ((FunctionBindingBehavior)d).OnResultAffectedPropertyChanged()));
		public static readonly DependencyProperty ConverterParameterProperty = 
			DependencyProperty.Register("ConverterParameter", typeof(object), typeof(FunctionBindingBehavior),
			new PropertyMetadata(null, (d, e) => ((FunctionBindingBehavior)d).OnResultAffectedPropertyChanged()));
		public static readonly DependencyProperty FunctionProperty = 
			DependencyProperty.Register("Function", typeof(string), typeof(FunctionBindingBehavior),
			new PropertyMetadata(null, (d, e) => ((FunctionBindingBehavior)d).OnResultAffectedPropertyChanged()));
		public string Property {
			get { return (string)GetValue(PropertyProperty); }
			set { SetValue(PropertyProperty, value); }
		}
		public IValueConverter Converter {
			get { return (IValueConverter)GetValue(ConverterProperty); }
			set { SetValue(ConverterProperty, value); }
		}
		public object ConverterParameter {
			get { return GetValue(ConverterParameterProperty); }
			set { SetValue(ConverterParameterProperty, value); }
		}
		public string Function {
			get { return (string)GetValue(FunctionProperty); }
			set { SetValue(FunctionProperty, value); }
		}
		#endregion
		protected override string ActualFunction { get { return Function; } }
		protected object GetSourceMethodValue() {
			object result = InvokeSourceFunction(ActualSource, ActualFunction, GetArgsInfo(this), DefaultMethodInfoChecker);
			return Converter.Return(x => x.Convert(result, null, ConverterParameter, CultureInfo.InvariantCulture), () => result);
		}
		protected override void OnResultAffectedPropertyChanged() {
			if(ActualTarget == null || ActualSource == null || string.IsNullOrEmpty(ActualFunction) || string.IsNullOrEmpty(Property) || !IsAttached)
				return;
			Action<object> propertySetter = GetObjectPropertySetter(ActualTarget, Property, false);
			if(propertySetter == null)
				return;
			object value = GetSourceMethodValue();
			if(value == DependencyProperty.UnsetValue)
				return;
			propertySetter.Invoke(value);
		}
	}
}
