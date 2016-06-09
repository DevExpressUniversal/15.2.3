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

using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Interactivity;
using System.Linq;
using System.Windows;
#if NETFX_CORE
using Windows.UI.Xaml;
#endif
namespace DevExpress.Mvvm.UI {
	public static class ViewModelExtensions {
		public static readonly object NotSetParameter = new object();
		public static readonly DependencyProperty ParameterProperty =
			DependencyProperty.RegisterAttached("Parameter", typeof(object), typeof(ViewModelExtensions),
			new PropertyMetadata(NotSetParameter, (d, e) => OnParameterChanged(d, e.NewValue)));
		public static readonly DependencyProperty ParentViewModelProperty =
			DependencyProperty.RegisterAttached("ParentViewModel", typeof(object), typeof(ViewModelExtensions), 
			new PropertyMetadata(null, (d,e) => OnParentViewModelChanged(d, e.NewValue)));
		public static readonly DependencyProperty DocumentOwnerProperty =
			DependencyProperty.RegisterAttached("DocumentOwner", typeof(IDocumentOwner), typeof(ViewModelExtensions),
			new PropertyMetadata(null, (d, e) => OnDocumentOwnerChanged(d, e.NewValue as IDocumentOwner)));
#if !SILVERLIGHT && !NETFX_CORE
		public static readonly DependencyProperty DocumentTitleProperty =
			DependencyProperty.RegisterAttached("DocumentTitle", typeof(object), typeof(ViewModelExtensions), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
#endif
		public static object GetParameter(DependencyObject obj) {
			return obj.GetValue(ParameterProperty) ?? GetParameterCore(obj);
		}
		public static object GetParentViewModel(DependencyObject obj) {
			return obj.GetValue(ParentViewModelProperty) ?? GetParentViewModelCore(obj);
		}
		public static IDocumentOwner GetDocumentOwner(DependencyObject obj) {
			return (IDocumentOwner)obj.GetValue(DocumentOwnerProperty) ?? GetDocumentOwnerCore(obj);
		}
		public static void SetParameter(DependencyObject obj, object value) {
			obj.SetValue(ParameterProperty, value);
		}
		public static void SetParentViewModel(DependencyObject obj, object value) {
			obj.SetValue(ParentViewModelProperty, value);
		}
		public static void SetDocumentOwner(DependencyObject obj, IDocumentOwner value) {
			obj.SetValue(DocumentOwnerProperty, value);
		}
#if !SILVERLIGHT && !NETFX_CORE
		public static object GetDocumentTitle(DependencyObject d) { return d.GetValue(DocumentTitleProperty); }
		public static void SetDocumentTitle(DependencyObject d, object value) { d.SetValue(DocumentTitleProperty, value); }
#endif
		static void OnParameterChanged(DependencyObject d, object newValue) {
			if(NotSetParameter == newValue) return;
			SetParameterCore(d, newValue);
			ParameterAndParentViewModelSyncBehavior.AttachTo(d);
		}
		static void OnParentViewModelChanged(DependencyObject d, object newValue) {
			SetParentViewModelCore(d, newValue);
			ParameterAndParentViewModelSyncBehavior.AttachTo(d);
		}
		static void OnDocumentOwnerChanged(DependencyObject d, IDocumentOwner newValue) {
			SetDocumentOwnerCore(d, newValue);
			ParameterAndParentViewModelSyncBehavior.AttachTo(d);
		}
		static void SetParameterCore(DependencyObject d, object parameter) {
			ViewHelper.GetViewModelFromView(d).With(x => x as ISupportParameter).Do(x => x.Parameter = parameter);
		}
		static void SetParentViewModelCore(DependencyObject d, object parentViewModel) {
			ViewHelper.GetViewModelFromView(d).With(x => x as ISupportParentViewModel).Do(x => x.ParentViewModel = parentViewModel);
		}
		static void SetDocumentOwnerCore(DependencyObject d, IDocumentOwner documentOwner) {
			ViewHelper.GetViewModelFromView(d).With(x => x as IDocumentContent).Do(x => x.DocumentOwner = documentOwner);
		}
		static object GetParameterCore(DependencyObject d) {
			return ViewHelper.GetViewModelFromView(d).With(x => x as ISupportParameter).Return(x => x.Parameter, null);
		}
		static object GetParentViewModelCore(DependencyObject d) {
			return ViewHelper.GetViewModelFromView(d).With(x => x as ISupportParentViewModel).Return(x => x.ParentViewModel, null);
		}
		static IDocumentOwner GetDocumentOwnerCore(DependencyObject d) {
			return ViewHelper.GetViewModelFromView(d).With(x => x as IDocumentContent).Return(x => x.DocumentOwner, null);
		}
		class ParameterAndParentViewModelSyncBehavior : Behavior<FrameworkElement> {
			public static void AttachTo(DependencyObject obj) {
				FrameworkElement el = obj as FrameworkElement;
				if(el == null) return;
				BehaviorCollection bCol = Interaction.GetBehaviors(el);
				ParameterAndParentViewModelSyncBehavior b = (ParameterAndParentViewModelSyncBehavior)bCol.FirstOrDefault(x => x is ParameterAndParentViewModelSyncBehavior);
				if(b != null) return;
				bCol.Add(new ParameterAndParentViewModelSyncBehavior());
			}
			ParameterAndParentViewModelSyncBehavior() { }
			protected override void OnAttached() {
				base.OnAttached();
				Subscribe();
			}
			protected override void OnDetaching() {
				Unsubscribe();
				base.OnDetaching();
			}
			void Subscribe() {
				Unsubscribe();
				AssociatedObject.Unloaded += OnAssociatedObjectUnloaded;
				AssociatedObject.DataContextChanged += OnAssociatedObjectDataContextChanged;
			}
			void Unsubscribe() {
				AssociatedObject.Unloaded -= OnAssociatedObjectUnloaded;
				AssociatedObject.DataContextChanged -= OnAssociatedObjectDataContextChanged;
			}
			void OnAssociatedObjectUnloaded(object sender, RoutedEventArgs e) {
				Unsubscribe();
			}
#if NETFX_CORE
			void OnAssociatedObjectDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs e) {
#else
			void OnAssociatedObjectDataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
#endif
				SetParameterCore(AssociatedObject, GetParameter(AssociatedObject));
				SetParentViewModelCore(AssociatedObject, GetParentViewModel(AssociatedObject));
				SetDocumentOwnerCore(AssociatedObject, GetDocumentOwner(AssociatedObject));
			}
		}
	}
}
