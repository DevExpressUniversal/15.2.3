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
using System.Windows;
#if !NETFX_CORE
using System.Windows.Controls;
using System.Windows.Data;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Style = Windows.UI.Xaml.Style;
#endif
namespace DevExpress.Mvvm.UI {
	public abstract class ViewServiceBase : ServiceBase {
		public IViewLocator ViewLocator {
			get { return (IViewLocator)GetValue(ViewLocatorProperty); }
			set { SetValue(ViewLocatorProperty, value); }
		}
		public static readonly DependencyProperty ViewLocatorProperty =
			DependencyProperty.Register("ViewLocator", typeof(IViewLocator), typeof(ViewServiceBase),
			new PropertyMetadata(null, (d, e) => ((ViewServiceBase)d).OnViewLocatorChanged((IViewLocator)e.OldValue, (IViewLocator)e.NewValue)));
		protected virtual void OnViewLocatorChanged(IViewLocator oldValue, IViewLocator newValue) { }
		public DataTemplate ViewTemplate {
			get { return (DataTemplate)GetValue(ViewTemplateProperty); }
			set { SetValue(ViewTemplateProperty, value); }
		}
		public static readonly DependencyProperty ViewTemplateProperty =
			DependencyProperty.Register("ViewTemplate", typeof(DataTemplate), typeof(ViewServiceBase),
			new PropertyMetadata(null, (d, e) => ((ViewServiceBase)d).OnViewTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue)));
		protected virtual void OnViewTemplateChanged(DataTemplate oldValue, DataTemplate newValue) { }
		public DataTemplateSelector ViewTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ViewTemplateSelectorProperty); }
			set { SetValue(ViewTemplateSelectorProperty, value); }
		}
		public static readonly DependencyProperty ViewTemplateSelectorProperty =
			DependencyProperty.Register("ViewTemplateSelector", typeof(DataTemplateSelector), typeof(ViewServiceBase),
			new PropertyMetadata(null, (d, e) => ((ViewServiceBase)d).OnViewTemplateSelectorChanged((DataTemplateSelector)e.OldValue, (DataTemplateSelector)e.NewValue)));
		protected virtual void OnViewTemplateSelectorChanged(DataTemplateSelector oldValue, DataTemplateSelector newValue) { }
		protected object CreateAndInitializeView(string documentType, object viewModel, object parameter, object parentViewModel, IDocumentOwner documentOwner = null) {
			return ViewHelper.CreateAndInitializeView(ViewLocator, documentType, viewModel, parameter, parentViewModel, documentOwner, ViewTemplate, ViewTemplateSelector);
		}
#if !SILVERLIGHT
		protected Style GetDocumentContainerStyle(DependencyObject documentContainer, object view, Style style, StyleSelector styleSelector) {
			return style ?? styleSelector.With(s => s.SelectStyle(ViewHelper.GetViewModelFromView(view), documentContainer));
		}
#endif
		protected void UpdateThemeName(DependencyObject target) {
#if !FREE && !SILVERLIGHT && !NETFX_CORE
			if(DevExpress.Xpf.Core.ThemeManager.GetTreeWalker(target) != null) return;
			var themeTreeWalker = AssociatedObject.With(DevExpress.Xpf.Core.ThemeManager.GetTreeWalker);
			if(themeTreeWalker == null) return;
			string themeName = DevExpress.Xpf.Editors.Helpers.ThemeHelper.GetWindowThemeName(AssociatedObject);
			if(DevExpress.Xpf.Core.ThemeManager.ApplicationThemeName != themeName)
				DevExpress.Xpf.Core.ThemeManager.SetThemeName(target, themeName);
#endif
		}
#if !NETFX_CORE
		protected void InitializeDocumentContainer(FrameworkElement documentContainer, DependencyProperty documentContainerViewProperty, Style documentContainerStyle) {
			ViewHelper.SetBindingToViewModel(documentContainer, FrameworkElement.DataContextProperty, new PropertyPath(documentContainerViewProperty));
			if(documentContainerStyle != null)
				documentContainer.Style = documentContainerStyle;
		}
#endif
	}
}
