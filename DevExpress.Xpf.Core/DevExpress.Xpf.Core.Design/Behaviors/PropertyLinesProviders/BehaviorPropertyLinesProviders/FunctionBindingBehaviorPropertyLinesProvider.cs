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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Mvvm.Native;
using System.Windows;
using System.Reflection;
namespace DevExpress.Xpf.Core.Design.Services.PropertyLinesProviders {
	public abstract class FunctionBindingBehaviorBasePropertyLinesProvider : PropertyLinesProviderBase {
		public IModelItem SelectedItem { get; protected set; }
		public IEnumerable<SmartTagLineViewModelBase> ArgsLines { get; private set; }
		public ObjectPropertyLineViewModel MethodPropertyLine { get; protected set; }
		public Func<MethodInfo, bool> GetMethodsPredicate { get; set; }
		public FunctionBindingBehaviorBasePropertyLinesProvider(Type itemType)
			: base(itemType) {
			if(!typeof(FunctionBindingBehaviorBase).IsAssignableFrom(itemType))
				throw new ArgumentException("itemType");
		}
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SelectedItem = viewModel.RuntimeSelectedItem;
			viewModel.PropertyChanged -= OnViewModelPropertyChanged;
			viewModel.PropertyChanged += OnViewModelPropertyChanged;
			MethodPropertyLine = new ObjectPropertyLineViewModel(viewModel, MethodPropertyName) {
				ItemsSource = GetMethods(GetMethodsPredicate), IsReadOnly = true
			};
			ArgsLines = CreateArgsLines(viewModel);
			UpdateArgsLinesVisibility();
			return base.GetPropertiesImpl(viewModel);
		}
		protected virtual void OnViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
			if(string.Equals(e.PropertyName, FunctionBindingBehaviorBase.SourceProperty.Name))
				OnSourcePropertyChanged();
			if(string.Equals(e.PropertyName, MethodPropertyName))
				UpdateArgsLinesVisibility();
		}
		protected virtual void OnSourcePropertyChanged() {
			if(MethodPropertyLine != null)
				MethodPropertyLine.ItemsSource = GetMethods(GetMethodsPredicate);
		}
		protected IEnumerable<string> GetMethods(Func<MethodInfo, bool> predicate) {
			var from = GetSource();
			if(from == null)
				return Enumerable.Empty<string>();
			var methodInfos = from.GetType().GetMethods();
			return methodInfos.Where(predicate).Select(info => info.Name).Distinct();
		}
		protected object GetSource() {
			if(SelectedItem == null)
				return null;
			var source = SelectedItem.Properties[FunctionBindingBehaviorBase.SourceProperty.Name].ComputedValue;
			if(source == null)
				source = SelectedItem.Parent.With(parent => parent.Properties[FrameworkElement.DataContextProperty.Name].ComputedValue);
			return source;
		}
		protected IEnumerable<SmartTagLineViewModelBase> CreateArgsLines(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			var argsLines = new List<SmartTagLineViewModelBase>(15);
			argsLines.Add(new ObjectPropertyLineViewModel(viewModel, FunctionBindingBehaviorBase.Arg1Property.Name));
			argsLines.Add(new ObjectPropertyLineViewModel(viewModel, FunctionBindingBehaviorBase.Arg2Property.Name));
			argsLines.Add(new ObjectPropertyLineViewModel(viewModel, FunctionBindingBehaviorBase.Arg3Property.Name));
			argsLines.Add(new ObjectPropertyLineViewModel(viewModel, FunctionBindingBehaviorBase.Arg4Property.Name));
			argsLines.Add(new ObjectPropertyLineViewModel(viewModel, FunctionBindingBehaviorBase.Arg5Property.Name));
			argsLines.Add(new ObjectPropertyLineViewModel(viewModel, FunctionBindingBehaviorBase.Arg6Property.Name));
			argsLines.Add(new ObjectPropertyLineViewModel(viewModel, FunctionBindingBehaviorBase.Arg7Property.Name));
			argsLines.Add(new ObjectPropertyLineViewModel(viewModel, FunctionBindingBehaviorBase.Arg8Property.Name));
			argsLines.Add(new ObjectPropertyLineViewModel(viewModel, FunctionBindingBehaviorBase.Arg9Property.Name));
			argsLines.Add(new ObjectPropertyLineViewModel(viewModel, FunctionBindingBehaviorBase.Arg10Property.Name));
			argsLines.Add(new ObjectPropertyLineViewModel(viewModel, FunctionBindingBehaviorBase.Arg11Property.Name));
			argsLines.Add(new ObjectPropertyLineViewModel(viewModel, FunctionBindingBehaviorBase.Arg12Property.Name));
			argsLines.Add(new ObjectPropertyLineViewModel(viewModel, FunctionBindingBehaviorBase.Arg13Property.Name));
			argsLines.Add(new ObjectPropertyLineViewModel(viewModel, FunctionBindingBehaviorBase.Arg14Property.Name));
			argsLines.Add(new ObjectPropertyLineViewModel(viewModel, FunctionBindingBehaviorBase.Arg15Property.Name));
			argsLines.ForEach(item => item.IsVisible = false);
			return argsLines;
		}
		protected void UpdateArgsLinesVisibility() {
			string methodName = (string)SelectedItem.With(item => item.Properties[MethodPropertyName].ComputedValue);
			Type sourceType = GetSource().With(s => s.GetType());
			int paramsCount = 0;
			if(!string.IsNullOrEmpty(methodName) && sourceType != null) {
				var methodInfos = sourceType.GetMethods(BindingFlags.Instance | BindingFlags.Public).Where(method => string.Equals(method.Name, methodName));
				paramsCount = methodInfos.Max(method => method.GetParameters().Length);
			}
			for(int i = 0; i < ArgsLines.Count(); i++) {
				ArgsLines.ElementAt(i).IsVisible = i < paramsCount;
			}
		}
		public abstract string MethodPropertyName { get; }
	}
	public sealed class FunctionBindingBehaviorPropertyLinesProvider : FunctionBindingBehaviorBasePropertyLinesProvider {
		ObjectPropertyLineViewModel PropertyPropertyLineViewModel { get; set; }
		public override string MethodPropertyName {
			get { return FunctionBindingBehavior.FunctionProperty.Name; }
		}
		public FunctionBindingBehaviorPropertyLinesProvider()
			: base(typeof(FunctionBindingBehavior)) {
			GetMethodsPredicate = info => info.ReturnType != typeof(void) && !info.IsSpecialName;
		}
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			base.GetPropertiesImpl(viewModel);
			PropertyPropertyLineViewModel = new ObjectPropertyLineViewModel(viewModel, FunctionBindingBehavior.PropertyProperty.Name) {
				ItemsSource = GetNamesOfProperties(), IsReadOnly = true
			};
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, FunctionBindingBehavior.TargetProperty.Name));
			lines.Add(() => PropertyPropertyLineViewModel);
			lines.Add(() => new SeparatorLineViewModel(viewModel));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, FunctionBindingBehavior.SourceProperty.Name));
			lines.Add(() => MethodPropertyLine);
			foreach(var argLine in ArgsLines) {
				lines.Add(() => argLine);
			}
			return lines;
		}
		protected override void OnViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
			base.OnViewModelPropertyChanged(sender, e);
			if(string.Equals(e.PropertyName, FunctionBindingBehavior.TargetProperty.Name))
				PropertyPropertyLineViewModel.ItemsSource = GetNamesOfProperties();
		}
		IEnumerable<string> GetNamesOfProperties() {
			if(SelectedItem == null)
				return Enumerable.Empty<string>();
			Type type = SelectedItem.Parent.ItemType;
			var from = SelectedItem.Properties[FunctionBindingBehavior.TargetProperty.Name].ComputedValue;
			if(from != null)
				type = from.GetType();
			return type.GetProperties().Where(p => p.GetSetMethod() != null).Select(p => p.Name);
		}
	}
}
