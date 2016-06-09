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

using DevExpress.Data.Filtering;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Utils.Filtering.Internal;
using DevExpress.Xpf.Core.Native;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Reflection.Emit;
using System.Windows;
using System.Windows.Controls;
namespace DevExpress.Xpf.Core {
	[TargetType(typeof(Window))]
	[TargetType(typeof(UserControl))]
	public class FilteringBehavior : Behavior<FrameworkElement> {
		public static readonly DependencyProperty SourceTypeProperty = DependencyProperty.Register("SourceType", typeof(Type), typeof(FilteringBehavior),
			new PropertyMetadata(null, (d, e) => ((FilteringBehavior)d).OnSourceTypeChanged()));
		static readonly DependencyPropertyKey FilteringViewModelPropertyKey = DependencyProperty.RegisterReadOnly("FilteringViewModel", typeof(object), typeof(FilteringBehavior), new PropertyMetadata(null));
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty FilteringViewModelProperty = FilteringViewModelPropertyKey.DependencyProperty;
		static readonly DependencyPropertyKey FilterCriteriaPropertyKey = DependencyProperty.RegisterReadOnly("FilterCriteria", typeof(CriteriaOperator), typeof(FilteringBehavior), new PropertyMetadata(null));
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty FilterCriteriaProperty = FilterCriteriaPropertyKey.DependencyProperty;
		public object FilteringViewModel { get { return (object)GetValue(FilteringViewModelProperty); } private set { SetValue(FilteringViewModelPropertyKey, value); } }
		public Type SourceType { get { return (Type)GetValue(SourceTypeProperty); } set { SetValue(SourceTypeProperty, value); } }
		public CriteriaOperator FilterCriteria { get { return (CriteriaOperator)GetValue(FilterCriteriaProperty); } set { SetValue(FilterCriteriaPropertyKey, value); } }
		protected override bool AllowAttachInDesignMode { get { return true; } }
		IEndUserFilteringViewModelProvider filteringViewModelProvider;
		protected override void OnAttached() {
			base.OnAttached();
			Initialize();
			AssociatedObject.Loaded += OnLoaded;
			AssociatedObject.Unloaded += OnUnloaded;
			AssociatedObject.DataContextChanged += OnDataContextChanged;
		}
		protected override void OnDetaching() {
			AssociatedObject.DataContextChanged -= OnDataContextChanged;
			AssociatedObject.Loaded -= OnLoaded;
			AssociatedObject.Unloaded -= OnUnloaded;
			Uninitialize();
			base.OnDetaching();
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			if(filteringViewModelProvider != null) return;
			Initialize();
		}
		void OnUnloaded(object sender, RoutedEventArgs e) {
			Uninitialize();
		}
		void Initialize() {
			filteringViewModelProvider = GetFilteringViewModelProvider(SourceType, AssociatedObject.DataContext);
			((INotifyPropertyChanged)filteringViewModelProvider).PropertyChanged += OnFilteringViewModelProviderPropertyChanged;
			FilterCriteria = filteringViewModelProvider.FilterCriteria;
			FilteringViewModel = filteringViewModelProvider.ViewModel;
		}
		void Uninitialize() {
			((INotifyPropertyChanged)filteringViewModelProvider).PropertyChanged -= OnFilteringViewModelProviderPropertyChanged;
			FilterCriteria = null;
			FilteringViewModel = null;
			filteringViewModelProvider.SourceType = null;
			filteringViewModelProvider.ParentViewModel = null;
			filteringViewModelProvider.Reset();
			filteringViewModelProvider = null;
		}
		void OnFilteringViewModelProviderPropertyChanged(object sender, PropertyChangedEventArgs e) {
			if(e.PropertyName == BindableBase.GetPropertyName(() => filteringViewModelProvider.FilterCriteria))
				FilterCriteria = filteringViewModelProvider.FilterCriteria;
			if(e.PropertyName == BindableBase.GetPropertyName(() => filteringViewModelProvider.ViewModel))
				FilteringViewModel = filteringViewModelProvider.ViewModel;
		}
		void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
			filteringViewModelProvider.Do(x => x.ParentViewModel = ((FrameworkElement)sender).DataContext);
		}
		void OnSourceTypeChanged() {
			filteringViewModelProvider.Do(x => x.SourceType = SourceType);
		}
		internal static IEndUserFilteringViewModelProvider GetFilteringViewModelProvider(Type sourceType, object parentVM) {
			var factory = new FilteringViewModelProviderFactory();
			var vmProvider = factory.CreateViewModelProvider();
			vmProvider.ParentViewModel = parentVM;
			vmProvider.SourceType = sourceType;
			return vmProvider;
		}
		class FilteringViewModelProviderFactory : BaseEndUserFilteringViewModelProviderFactory {
			protected override IViewModelFactory GetViewModelFactory() {
				return new ViewModelFactory();
			}
		}
		class ViewModelFactory : IViewModelFactory {
			public object Create(Type viewModelType, IViewModelBuilder builder) {
				return Activator.CreateInstance(ViewModelSource.GetPOCOType(viewModelType, new ViewModelSourceBuilder(builder)));
			}
		}
		class ViewModelSourceBuilder : ViewModelSourceBuilderBase {
			readonly IViewModelBuilder underlyingBuilder;
			public ViewModelSourceBuilder(IViewModelBuilder underlyingBuilder) {
				this.underlyingBuilder = underlyingBuilder;
			}
			protected override void BuildBindablePropertyAttributes(PropertyInfo property, PropertyBuilder builder) {
				base.BuildBindablePropertyAttributes(property, builder);
				if(underlyingBuilder != null)
					underlyingBuilder.BuildBindablePropertyAttributes(property, builder);
			}
			protected override bool ForceOverrideProperty(PropertyInfo property) {
				return underlyingBuilder.ForceBindableProperty(property);
			}
		}
	}
} 
