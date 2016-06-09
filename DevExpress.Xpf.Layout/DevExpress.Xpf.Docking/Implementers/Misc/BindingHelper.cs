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

using System.Windows;
using System.Windows.Data;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Docking {
	static class BindingHelper {
		public static void SetBinding(DependencyObject target, DependencyProperty property, object source, DependencyProperty path = null, BindingMode mode = BindingMode.OneWay) {
			if(target == null) return;
			if(path == null) path = property;
			BindingOperations.SetBinding(target, property,
 new Binding()
 {
	 Path = new PropertyPath(path),
	 Source = source,
	 Mode = mode
 });
		}
		public static void SetBinding(DependencyObject target, DependencyProperty property, object source, DependencyProperty path, IValueConverter converter) {
			if(target == null) return;
			if(path == null) path = property;
			BindingOperations.SetBinding(target, property,
 new Binding()
 {
	 Path = new PropertyPath(path),
	 Source = source,
	 Converter = converter
 });
		}
		public static void SetBinding(DependencyObject target, DependencyProperty property, object source, string path) {
			if(target == null) return;
			BindingOperations.SetBinding(target, property, new Binding(path) { Source = source });
		}
		public static void ClearBinding(DependencyObject dObj, DependencyProperty property) {
			if(dObj == null) return;
			BindingOperations.ClearBinding(dObj, property);
		}
		public static BindingBase CloneBinding(BindingBase bindingBase, object source) {
			var binding = bindingBase as Binding;
			if(binding != null) {
				var result = new Binding
				{
					Source = source,
					AsyncState = binding.AsyncState,
					BindingGroupName = binding.BindingGroupName,
					XPath = binding.XPath,
					IsAsync = binding.IsAsync,
					NotifyOnSourceUpdated = binding.NotifyOnSourceUpdated,
					NotifyOnTargetUpdated = binding.NotifyOnTargetUpdated,
					UpdateSourceExceptionFilter = binding.UpdateSourceExceptionFilter,
					BindsDirectlyToSource = binding.BindsDirectlyToSource,
					Converter = binding.Converter,
					ConverterCulture = binding.ConverterCulture,
					ConverterParameter = binding.ConverterCulture,
					FallbackValue = binding.FallbackValue,
					Mode = binding.Mode,
					NotifyOnValidationError = binding.NotifyOnValidationError,
					Path = binding.Path,
					StringFormat = binding.StringFormat,
					TargetNullValue = binding.TargetNullValue,
					UpdateSourceTrigger = binding.UpdateSourceTrigger,
					ValidatesOnDataErrors = binding.ValidatesOnDataErrors,
					ValidatesOnExceptions = binding.ValidatesOnExceptions,
				};
				foreach(var validationRule in binding.ValidationRules) {
					result.ValidationRules.Add(validationRule);
				}
				return result;
			}
			return null;
		}
		public static bool IsDataBound(DependencyObject dObj, DependencyProperty property) {
			return BindingOperations.IsDataBound(dObj, property);
		}
	}
}
