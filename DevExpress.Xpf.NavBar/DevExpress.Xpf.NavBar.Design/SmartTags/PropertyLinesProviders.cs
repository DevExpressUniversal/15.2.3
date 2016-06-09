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

extern alias Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Microsoft.Windows.Design.Model;
using Platform::DevExpress.Xpf.NavBar;
using Platform::DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
#if SL
using Platform::DevExpress.Xpf.Core.Design;
using Platform::DevExpress.Design.SmartTags;
#else
using DevExpress.Design.SmartTags;
using DevExpress.Xpf.Core.Design;
using DevExpress.Xpf.Core.Design.SmartTags;
#endif
namespace DevExpress.Xpf.NavBar.Design.SmartTags {
	sealed class NavBarControlPropertyLinesProvider : PropertyLinesProviderBase {
		public NavBarControlPropertyLinesProvider() : base(typeof(NavBarControl)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(new AddNavBarGroupCommandActionProvider(viewModel)));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => NavBarControl.AllowSelectDisabledItemProperty)));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => NavBarControl.AllowSelectItemProperty)));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => NavBarControl.EachGroupHasSelectedItemProperty)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => NavBarControl.GroupDescriptionProperty)));
#if !SL
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => NavBarControl.ItemsSourceProperty)));
#endif
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => NavBarControl.MaxWidthProperty)));
			lines.Add(() => new ItemListPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => NavBarControl.ViewProperty), typeof(NavBarViewBase), DXTypeInfoInstanceSource.FromTypeList(RegisterMetadata.ViewTypes)));
			lines.Add(() => new NestedPropertyLinesViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => NavBarControl.ViewProperty)));
			return lines;
		}
	}
	class SelectViewCommandActionProvider : CommandActionLineProvider {
		public SelectViewCommandActionProvider(IPropertyLineContext context)
			: base(context) {
			CanExecuteAction = OnCanExecute;
		}
		bool OnCanExecute(object arg) {
			ModelItem navBar = XpfModelItem.ToModelItem(arg as IModelItem ?? Context.ModelItem);
			return navBar != null && navBar.Properties["View"].IsSet;
		}
		protected override string GetCommandText() {
			return "Select View";
		}
		protected override void OnCommandExecute(object param) {
			ModelItem navBar = XpfModelItem.ToModelItem(param as IModelItem ?? Context.ModelItem);
			if(navBar == null) return;
			SelectionHelper.SetSelection(navBar.Context, navBar.Properties["View"].Value);
			var service = navBar.Context.Services.GetService<SmartTagDesignService>();
			if(service != null)
				service.IsSmartTagButtonPressed = true;
		}
	}
	class AddNavBarGroupCommandActionProvider : CommandActionLineProvider {
		public AddNavBarGroupCommandActionProvider(FrameworkElementSmartTagPropertiesViewModel ownerViewModel)
			: base(ownerViewModel) { }
		protected override string GetCommandText() {
			return "Add NavBarGroup";
		}
		protected override void OnCommandExecute(object param) {
			XpfModelItem item = Context.ModelItem as XpfModelItem;
			if(item == null) return;
			NavBarDesignTimeHelper.AddNewNavBarGroup(item.Value);
		}
	}
	sealed class NavBarGroupPropertyLinesProvider : PropertyLinesProviderBase {
		public NavBarGroupPropertyLinesProvider() : base(typeof(NavBarGroup)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(new AddNavBarItemCommandActionProvider(viewModel)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => NavBarGroup.ContentProperty)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => NavBarGroup.HeaderProperty)));
			lines.Add(() => new CommandPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => NavBarGroup.CommandProperty)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => NavBarGroup.CommandParameterProperty)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => NavBarGroup.DisplayModeProperty), typeof(DisplayMode)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => NavBarGroup.DisplaySourceProperty), typeof(DisplaySource)));
#if !SL
			lines.Add(() => new ImageSourcePropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => NavBarGroup.ImageSourceProperty)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => NavBarGroup.ItemsSourceProperty)));
#endif
			return lines;
		}
	}
	class AddNavBarItemCommandActionProvider : CommandActionLineProvider {
		public AddNavBarItemCommandActionProvider(FrameworkElementSmartTagPropertiesViewModel ownerViewModel)
			: base(ownerViewModel) { }
		protected override string GetCommandText() {
			return "Add NavBarItem";
		}
		protected override void OnCommandExecute(object param) {
			XpfModelItem item = Context.ModelItem as XpfModelItem;
			if(item == null) return;
			NavBarDesignTimeHelper.AddNewNavBarItem(item.Value);
		}
	}
	sealed class NavBarItemPropertyLinesProvider : PropertyLinesProviderBase {
		public NavBarItemPropertyLinesProvider() : base(typeof(NavBarItem)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => NavBarItem.ContentProperty)));
#if !SILVERLIGHT
			lines.Add(() => new ImageSourcePropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => NavBarItem.ImageSourceProperty)));
#endif
			lines.Add(() => new CommandPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => NavBarItem.CommandProperty)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => NavBarGroup.CommandParameterProperty)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => NavBarItem.DisplayModeProperty), typeof(DisplayMode)));
			return lines;
		}
	}
	#region ViewsProviders
	abstract class NavBarViewPropertyLinesProviderBase : PropertyLinesProviderBase {
		public NavBarViewPropertyLinesProviderBase(Type itemType) : base(itemType) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => NavBarViewBase.GroupDisplayModeProperty), typeof(DisplayMode)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => NavBarViewBase.ItemDisplayModeProperty), typeof(DisplayMode)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => NavBarViewBase.ItemsPanelOrientationProperty), typeof(Orientation)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => NavBarViewBase.OrientationProperty), typeof(Orientation)));
			return lines;
		}
	}
	sealed class NavigationPaneViewProperyLinesProvider : NavBarViewPropertyLinesProviderBase {
		public NavigationPaneViewProperyLinesProvider() : base(typeof(NavigationPaneView)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => NavigationPaneView.IsExpandButtonVisibleProperty)));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => NavigationPaneView.IsExpandedProperty)));
			return lines;
		}
	}
	sealed class ExplorerBarViewPropertyLinesProvider : NavBarViewPropertyLinesProviderBase {
		public ExplorerBarViewPropertyLinesProvider() : base(typeof(ExplorerBarView)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => ExplorerBarView.AnimateGroupExpansionProperty)));
			return lines;
		}
	}
	sealed class SideBarViewPropertyLinesProvider : NavBarViewPropertyLinesProviderBase {
		public SideBarViewPropertyLinesProvider() : base(typeof(SideBarView)) { }
	}
	#endregion
	static class NavBarPropertyLinesRegistrator {
		public static void RegisterPropertyLines() {
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new NavBarControlPropertyLinesProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new NavBarItemPropertyLinesProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new NavBarGroupPropertyLinesProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new ExplorerBarViewPropertyLinesProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new SideBarViewPropertyLinesProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new NavigationPaneViewProperyLinesProvider());
		}
	}
}
