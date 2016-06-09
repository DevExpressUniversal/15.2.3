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

using DevExpress.Mvvm.UI;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.NavBar;
using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
namespace DevExpress.Mvvm.UI.ViewInjection {
	public class NavBarControlWrapper : ISelectorWrapper<NavBarControl> {
		public NavBarControl Target { get; set; }
		public object ItemsSource {
			get { return Target.ItemsSource; }
			set { Target.ItemsSource = (IEnumerable)value; }
		}
		public object SelectedItem {
			get { return Target.SelectedGroup; }
			set { Target.SelectedGroup = value; }
		}
		public DataTemplate ItemTemplate {
			get { return Target.ItemTemplate; }
			set { Target.ItemTemplate = value; }
		}
		public DataTemplateSelector ItemTemplateSelector {
			get { return Target.ItemTemplateSelector; }
			set { Target.ItemTemplateSelector = value; }
		}
		public event EventHandler SelectionChanged {
			add { DependencyPropertyDescriptor.FromProperty(NavBarControl.SelectedGroupProperty, typeof(NavBarControl)).AddValueChanged(Target, value); }
			remove { DependencyPropertyDescriptor.FromProperty(NavBarControl.SelectedGroupProperty, typeof(NavBarControl)).RemoveValueChanged(Target, value); }
		}
	}
	public class NavBarControlStrategy : SelectorStrategy<NavBarControl, NavBarControlWrapper> {
		const string Exception2 = "ViewInjectionService cannot create view by name or type, because the target control has the ItemTemplate/ItemTemplateSelector property set.";
		class InjectionNavBarGroupContentControl : ContentControl {
			public InjectionNavBarGroupContentControl() {
				Content = new InjectionNavBarGroup();
			}
		}
		class InjectionNavBarGroup : NavBarGroup {
			protected internal override void UpdateActualLayoutSettings() {
				base.UpdateActualLayoutSettings();
				if(NavBar == null) return;
				if(Content == null && !BindingOperations.IsDataBound(this, ContentProperty)) {
					Content = DataContext;
					ContentTemplate = GetStrategy(NavBar).ViewSelector.SelectTemplate(Content, this);
					DisplaySource = DevExpress.Xpf.NavBar.DisplaySource.Content;
				}
			}
		}
		static DataTemplate _navBarGroupItemTemplate;
		static DataTemplate NavBarGroupItemTemplate {
			get {
				if(_navBarGroupItemTemplate == null) {
					_navBarGroupItemTemplate = new DataTemplate() { VisualTree = new FrameworkElementFactory(typeof(InjectionNavBarGroupContentControl)) };
					_navBarGroupItemTemplate.Seal();
				}
				return _navBarGroupItemTemplate;
			}
		}
		static NavBarControlStrategy GetStrategy(NavBarControl navBar) {
			var service = Interaction.GetBehaviors(navBar).OfType<ViewInjectionService>().First();
			return (NavBarControlStrategy)service.Strategy;
		}
		protected override void InitItemTemplate() {
			if(Target.ItemTemplate == null && Target.ItemTemplateSelector == null)
				Target.ItemTemplate = NavBarGroupItemTemplate;
		}
		protected override void CheckInjectionProcedure(object viewModel, string viewName, Type viewType) {
			if(Wrapper.ItemTemplate != NavBarGroupItemTemplate && (!string.IsNullOrEmpty(viewName) || viewType != null))
				throw new InvalidOperationException(Exception2);
		}
		public static void RegisterStrategy() {
			StrategyManager.Default.RegisterStrategy<NavBarControl, NavBarControlStrategy>();
		}
	}
}
