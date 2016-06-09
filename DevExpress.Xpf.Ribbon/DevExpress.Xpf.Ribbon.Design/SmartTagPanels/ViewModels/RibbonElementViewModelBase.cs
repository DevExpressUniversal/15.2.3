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
using DevExpress.Xpf.Core.Design;
using Microsoft.Windows.Design.Model;
using DevExpress.Design.SmartTags;
using Platform::DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
namespace DevExpress.Xpf.Ribbon.Design.SmartTagPanels.ViewModels {
	abstract class RibbonItemPropertyLinesProvider : PropertyLinesProviderBase {
		public RibbonItemPropertyLinesProvider(Type itemType) : base(itemType) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => RibbonPage.CaptionProperty)));
			return lines;
		}
	}
	sealed class RibbonPropertyLineProvider : PropertyLinesProviderBase {
		public RibbonPropertyLineProvider() : base(typeof(RibbonControl)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			ModelItem ribbon = XpfModelItem.ToModelItem(viewModel.RuntimeSelectedItem);
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(new RibbonControlAddCategoryCommandActionProvider(viewModel)));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(new RibbonControlAddPageCommandActionProvider(viewModel)));
			lines.Add(() => new ActionListPropertyLineViewModel(new RibbonAddToQAToolbarActionListContext(viewModel)) { Text = "Add to Toolbar:" });
			lines.Add(() => new ActionListPropertyLineViewModel(new RibbonAddToPageHeaderActionListContext(viewModel)) { Text = "Add to PageHeader:" });
			lines.Add(() => new ItemListPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => RibbonControl.ApplicationMenuProperty), typeof(object), DXTypeInfoInstanceSource.FromTypeList(new Type[] { typeof(ApplicationMenu), typeof(BackstageViewControl) })));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => RibbonControl.MinimizationButtonVisibilityProperty), typeof(RibbonMinimizationButtonVisibility)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => RibbonControl.PageCategoryAlignmentProperty), typeof(RibbonPageCategoryCaptionAlignment), null, t => new Enum[] { RibbonPageCategoryCaptionAlignment.Left, RibbonPageCategoryCaptionAlignment.Right }));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => RibbonControl.RibbonStyleProperty), typeof(RibbonStyle)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => RibbonControl.ToolbarShowModeProperty), typeof(RibbonQuickAccessToolbarShowMode), null, t => new Enum[] { RibbonQuickAccessToolbarShowMode.ShowAbove, RibbonQuickAccessToolbarShowMode.ShowBelow, RibbonQuickAccessToolbarShowMode.Hide }));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => RibbonControl.ShowApplicationButtonProperty)));
			return lines;
		}
	}
	class RibbonAddToQAToolbarActionListContext : BarsActionListPropertyLineContext {
		public RibbonAddToQAToolbarActionListContext(IPropertyLineContext context) : base(context) { }
		protected override ModelItem GetTargetItem(ModelItem item) {
			return item;
		}
		protected override ModelProperty GetTargetProperty(ModelItem targetItem) {
			return targetItem.Properties["ToolbarItems"];
		}
	}
	class RibbonAddToPageHeaderActionListContext : BarsActionListPropertyLineContext {
		public RibbonAddToPageHeaderActionListContext(IPropertyLineContext context) : base(context) { }
		protected override ModelItem GetTargetItem(ModelItem item) {
			return item;
		}
		protected override ModelProperty GetTargetProperty(ModelItem targetItem) {
			return targetItem.Properties["PageHeaderItems"];
		}
	}
	class RibbonControlAddCategoryCommandActionProvider : CommandActionLineProvider {
		public RibbonControlAddCategoryCommandActionProvider(FrameworkElementSmartTagPropertiesViewModel ownerViewModel)
			: base(ownerViewModel) { }
		protected override string GetCommandText() {
			return "Add RibbonPageCategory";
		}
		protected override void OnCommandExecute(object param) {
			ModelItem ribbon = XpfModelItem.ToModelItem(Context.ModelItem);
			if(ribbon == null) return;
			RibbonDesignTimeHelper.AddRibbonPageCategory(ribbon);
		}
	}
	class RibbonControlAddPageCommandActionProvider : CommandActionLineProvider {
		public RibbonControlAddPageCommandActionProvider(FrameworkElementSmartTagPropertiesViewModel ownerViewModel)
			: base(ownerViewModel) {
			this.CanExecuteAction = CanExecute;
		}
		bool CanExecute(object param) {
			ModelItem ribbon = XpfModelItem.ToModelItem(Context.ModelItem);
			return ribbon != null && RibbonDesignTimeHelper.IsRibbonContainDefaultCategory(ribbon);
		}
		protected override string GetCommandText() {
			return "Add RibbonPage";
		}
		protected override void OnCommandExecute(object param) {
			ModelItem ribbon = XpfModelItem.ToModelItem(Context.ModelItem);
			if(ribbon == null) return;
			ModelItem category = RibbonDesignTimeHelper.GetDefaultPageCategory(ribbon);
			RibbonDesignTimeHelper.AddRibbonPage(category);
		}
	}
	class RibbonControlCreateBarManagerCommandActionProvider : CommandActionLineProvider {
		public RibbonControlCreateBarManagerCommandActionProvider(FrameworkElementSmartTagPropertiesViewModel ownerViewModel)
			: base(ownerViewModel) { }
		protected override string GetCommandText() {
			return "Create BarManager";
		}
		protected override void OnCommandExecute(object param) {
			ModelItem ribbon = XpfModelItem.ToModelItem(Context.ModelItem);
			if(ribbon == null) return;
			RibbonDesignTimeHelper.CreateBarManager(ribbon);
		}
	}
	sealed class DXRibbonWindowPropertyLineProvider : DXWindowPropertyLinesProvider {
		public DXRibbonWindowPropertyLineProvider() : base(typeof(DXRibbonWindow)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			var lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new SeparatorLineViewModel(viewModel) { Text = "DXRibbonWindow Properties" });
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DXRibbonWindow.DisplayShowModeSelectorProperty.Name));
			return lines;
		}
	}
}
