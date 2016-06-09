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
using System.Windows;
using DevExpress.Utils;
using DevExpress.Xpf.Core.Native;
using System.Windows.Data;
namespace DevExpress.Xpf.Grid.Native {
	public static class Constants {
		public const int InvalidNavigationIndex = int.MinValue;
	}
	public static class ColumnsLayoutParametersValidator {
#if DEBUGTEST
		internal static double? ForcedDataPresenterWidth = null;
#endif
		public static Size GetLastDataPresenterConstraint(Size constraint) {
#if DEBUGTEST
			if(ForcedDataPresenterWidth.HasValue)
				return new Size(ForcedDataPresenterWidth.Value, constraint.Height);
#endif
			return constraint;
		}
		public static double GetVerticalScrollBarWidth(double width) {
#if DEBUGTEST
			if(ForcedDataPresenterWidth.HasValue)
				return 0;
#endif
			return width;
		}
	}
	public enum CustomServiceSummaryItemType {
		DateTimeAverage,
		SortedList,
	}
	public class ServiceSummaryItem : SummaryItemBase {
		internal override bool? IgnoreNullValues { get { return true; } }
		public CustomServiceSummaryItemType? CustomServiceSummaryItemType { get; set; }
	}
	public class AnonymousEqualityComparer<T> : EqualityComparer<T> {
		Func<T, T, bool> _equals;
		Func<T, int> _getHashCode;
		public AnonymousEqualityComparer(Func<T, T, bool> equals, Func<T, int> getHashCode) {
			Guard.ArgumentNotNull(equals, "equals");
			Guard.ArgumentNotNull(getHashCode, "getHashCode");
			_equals = equals;
			_getHashCode = getHashCode;
		}
		public override bool Equals(T x, T y) {
			return _equals(x, y);
		}
		public override int GetHashCode(T obj) {
			return _getHashCode(obj);
		}
	}
	public class BindingValueEvaluator : FrameworkElement {
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register("Value", typeof(object), typeof(BindingValueEvaluator), null);
		public object Value {
			get {
				Value = DependencyProperty.UnsetValue;
				SetBinding(BindingValueEvaluator.ValueProperty, binding);
				return GetValue(ValueProperty);
			}
			private set { SetValue(ValueProperty, value); }
		}
		BindingBase binding;
		public BindingValueEvaluator(BindingBase binding) {
			this.binding = binding;
		}
	}
	internal static class BindingCloneHelper {
		public static BindingBase Clone(BindingBase sourceBinding, object source) {
			Binding binding = sourceBinding as Binding;
			if(binding != null) {
				Binding resultBinding = new Binding()
				{
					Source = source,
					BindsDirectlyToSource = binding.BindsDirectlyToSource,
					Converter = binding.Converter,
					ConverterCulture = binding.ConverterCulture,
					ConverterParameter = binding.ConverterParameter,
					FallbackValue = binding.FallbackValue,
					Mode = binding.Mode,
					NotifyOnValidationError = binding.NotifyOnValidationError,
					Path = binding.Path,
					StringFormat = binding.StringFormat,
					TargetNullValue = binding.TargetNullValue,
					UpdateSourceTrigger = binding.UpdateSourceTrigger,
					ValidatesOnDataErrors = binding.ValidatesOnDataErrors,
					ValidatesOnExceptions = binding.ValidatesOnExceptions,
					AsyncState = binding.AsyncState,
					BindingGroupName = binding.BindingGroupName,
					IsAsync = binding.IsAsync,
					NotifyOnSourceUpdated = binding.NotifyOnSourceUpdated,
					NotifyOnTargetUpdated = binding.NotifyOnTargetUpdated,
					UpdateSourceExceptionFilter = binding.UpdateSourceExceptionFilter,
					XPath = binding.XPath,
				};
				foreach(var item in binding.ValidationRules)
					resultBinding.ValidationRules.Add(item);
				return resultBinding;
			}
			MultiBinding multiBinding = sourceBinding as MultiBinding;
			if(multiBinding != null) {
				MultiBinding resultBinding = new MultiBinding()
				{
					BindingGroupName = multiBinding.BindingGroupName,
					Converter = multiBinding.Converter,
					ConverterCulture = multiBinding.ConverterCulture,
					ConverterParameter = multiBinding.ConverterParameter,
					FallbackValue = multiBinding.FallbackValue,
					Mode = multiBinding.Mode,
					NotifyOnSourceUpdated = multiBinding.NotifyOnSourceUpdated,
					NotifyOnTargetUpdated = multiBinding.NotifyOnTargetUpdated,
					NotifyOnValidationError = multiBinding.NotifyOnValidationError,
					StringFormat = multiBinding.StringFormat,
					TargetNullValue = multiBinding.TargetNullValue,
					UpdateSourceExceptionFilter = multiBinding.UpdateSourceExceptionFilter,
					UpdateSourceTrigger = multiBinding.UpdateSourceTrigger,
					ValidatesOnDataErrors = multiBinding.ValidatesOnDataErrors,
					ValidatesOnExceptions = multiBinding.ValidatesOnDataErrors,
				};
				foreach(var validationRule in multiBinding.ValidationRules) {
					resultBinding.ValidationRules.Add(validationRule);
				}
				foreach(var childBinding in multiBinding.Bindings) {
					resultBinding.Bindings.Add(Clone(childBinding, source));
				}
				return resultBinding;
			}
			return null;
		}
	}
}
