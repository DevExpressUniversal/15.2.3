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

using DevExpress.Design.SmartTags;
using DevExpress.Mvvm.UI;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
namespace DevExpress.Xpf.Core.Design.Services.PropertyLinesProviders {
	public sealed class MethodToCommandBehaviorPropertyLinesProvider : FunctionBindingBehaviorBasePropertyLinesProvider {
		ObjectPropertyLineViewModel CanExecuteFunctionPropertyLine { get; set; }
		public override string MethodPropertyName {
			get { return MethodToCommandBehavior.MethodProperty.Name; }
		}
		public MethodToCommandBehaviorPropertyLinesProvider()
			: base(typeof(MethodToCommandBehavior)) {
				GetMethodsPredicate = info => info.ReturnType == typeof(void) && !info.IsSpecialName && !info.Name.StartsWith("set_");
		}
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			base.GetPropertiesImpl(viewModel);
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, FunctionBindingBehavior.TargetProperty.Name));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, MethodToCommandBehavior.CommandProperty.Name) { ItemsSource = GetCommands(SelectedItem.Parent), IsReadOnly = true });
			lines.Add(() => new SeparatorLineViewModel(viewModel));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, MethodToCommandBehavior.SourceProperty.Name));
			lines.Add(() => MethodPropertyLine);
			CanExecuteFunctionPropertyLine = new ObjectPropertyLineViewModel(viewModel, MethodToCommandBehavior.CanExecuteFunctionProperty.Name) { ItemsSource = GetMethods(CanExecuteFunctionPredicate), IsReadOnly = true };
			lines.Add(() => CanExecuteFunctionPropertyLine);
			foreach(var argLine in ArgsLines) {
				lines.Add(() => argLine);
			}
			return lines;
		}
		protected override void OnSourcePropertyChanged() {
			base.OnSourcePropertyChanged();
			if(CanExecuteFunctionPropertyLine != null)
				CanExecuteFunctionPropertyLine.ItemsSource = GetMethods(CanExecuteFunctionPredicate);
		}
		IEnumerable<string> GetCommands(IModelItem from) {
			var propertyInfos = from.ItemType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
			return propertyInfos.Where(info => typeof(ICommand).IsAssignableFrom(info.PropertyType)).Select(info => info.Name);
		}
		bool CanExecuteFunctionPredicate(MethodInfo methodInfo) {
			return methodInfo.ReturnType == typeof(bool) && methodInfo.GetParameters().Length == 1;
		}
	}
}
