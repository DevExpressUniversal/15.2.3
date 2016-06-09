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
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using DevExpress.Design.SmartTags;
using DevExpress.Entity.Model;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Native.ViewGenerator;
using DevExpress.Xpf.Core.Design;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Model;
using Platform::DevExpress.Xpf.Editors;
using Platform::DevExpress.Xpf.LayoutControl;
using Platform::System.Windows;
using Platform::System.Windows.Controls;
using DependencyPropertyHelper = DevExpress.Xpf.Core.Design.DependencyPropertyHelper;
namespace DevExpress.Xpf.LayoutControl.Design {
	sealed class DataLayoutControlPropertyLinesProvider : IPropertyLineProvider {
		IEnumerable<SmartTagLineViewModelBase> IPropertyLineProvider.GetProperties(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			return new SmartTagLineViewModelBase[] {
				new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(()=>DataLayoutControl.CurrentItemProperty)),
			};
		}
		Type IPropertyLineProvider.ItemType { get { return typeof(DataLayoutControl); } }
	}
	sealed class LayoutControlPropertyLinesProvider : IPropertyLineProvider {
		IEnumerable<SmartTagLineViewModelBase> IPropertyLineProvider.GetProperties(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			return new SmartTagLineViewModelBase[] { };
		}
		Type IPropertyLineProvider.ItemType { get { return typeof(Platform::DevExpress.Xpf.LayoutControl.LayoutControl); } }
	}
	abstract class CreateItemCommandProviderBase : CommandActionLineProvider {
		public CreateItemCommandProviderBase(IPropertyLineContext context)
			: base(context) { }
		protected override void OnCommandExecute(object param) {
			ModelItem layoutControl = param is XpfModelItem ? XpfModelItem.ToModelItem((XpfModelItem)param) : null;
			if(layoutControl == null) return;
			using(var scope = layoutControl.BeginEdit(GetCommandText())) {
				ModelItem item = CreateItem(layoutControl.Context);
				AddItem(layoutControl, item);
				scope.Complete();
			}
		}
		protected virtual void AddItem(ModelItem layoutControl, ModelItem item) {
			layoutControl.Content.Collection.Add(item);
		}
		protected abstract ModelItem CreateItem(EditingContext context);
	}
	class CreateGroupCommandProvider : CreateItemCommandProviderBase {
		public CreateGroupCommandProvider(IPropertyLineContext context)
			: base(context) { }
		protected override string GetCommandText() {
			return "Add a new group";
		}
		protected override ModelItem CreateItem(EditingContext context) {
			ModelItem group = ModelFactory.CreateItem(context, typeof(LayoutGroup));
			group.Properties["Header"].SetValue("LayoutGroup");
			group.Properties["View"].SetValue(LayoutGroupView.GroupBox);
			return group;
		}
	}
	class CreateItemCommandProvider : CreateItemCommandProviderBase {
		public CreateItemCommandProvider(IPropertyLineContext context)
			: base(context) { }
		protected override string GetCommandText() {
			return "Add a new item";
		}
		protected override ModelItem CreateItem(EditingContext context) {
			ModelItem item = ModelFactory.CreateItem(context, typeof(LayoutItem));
			item.Properties["Label"].SetValue("LayoutItem");
			item.Content.SetValue(ModelFactory.CreateItem(context, typeof(TextEdit)));
			return item;
		}
	}
	class CreateTabbedItemCommandProvider : CreateItemCommandProviderBase {
		public CreateTabbedItemCommandProvider(IPropertyLineContext context)
			: base(context) { }
		protected override string GetCommandText() {
			return "Add a new tabbed group";
		}
		protected override ModelItem CreateItem(EditingContext context) {
			ModelItem group = ModelFactory.CreateItem(context, typeof(LayoutGroup), CreateOptions.InitializeDefaults);
			group.Properties["Header"].SetValue("LayoutGroup");
			group.Properties["View"].SetValue(LayoutGroupView.Tabs);
			ModelItem childGroup = ModelFactory.CreateItem(context, typeof(LayoutGroup));
			childGroup.Properties["Header"].SetValue("Tab");
			group.Content.Collection.Add(childGroup);
			return group;
		}
	}
	class GenerateItemsCommandProvider : CommandActionLineProvider {
		public GenerateItemsCommandProvider(IPropertyLineContext context)
			: base(context) {				
				this.CanExecuteAction = CanExecute;
		}
		bool CanExecute(object param) {
			ModelItem model = XpfModelItem.ToModelItem(Context.ModelItem);
			if(model == null) return false;
			var dataLayoutControl = model.GetCurrentValue() as DataLayoutControl;
			return dataLayoutControl != null && dataLayoutControl.CurrentItem != null;
		}
		protected override void OnCommandExecute(object param) {
			ModelItem model = XpfModelItem.ToModelItem(Context.ModelItem);
			using(var scope = model.BeginEdit(GetCommandText())) {				
				model.Properties["AutoGenerateItems"].SetValue(false);
				var dataLayoutControl = model.GetCurrentValue() as DataLayoutControl;
				Type objectType = dataLayoutControl.CurrentItem.GetType();
				if(objectType.IsSimple()) {
					AddItem(model, CreateItem(model.Context, new Binding()));
				} else {
					var propInfos = EditorsGeneratorBase.GetFilteredAndSortedProperties(GetCurrentItemTypeSupportedProperties(dataLayoutControl.CurrentItem), false, false, LayoutType.DataForm);
					foreach(var prop in propInfos) {
						AddItem(model, CreateItem(model.Context, new Binding(prop.Name)));
					}
				}
				scope.Complete();
			}
		}
		protected virtual IEnumerable<IEdmPropertyInfo> GetCurrentItemTypeSupportedProperties(object currentItem) {
			IEntityProperties typeInfo = new ReflectionEntityProperties(TypeDescriptor.GetProperties(currentItem).Cast<PropertyDescriptor>(), currentItem.GetType(), true);
			return typeInfo.AllProperties;
		}
		protected override string GetCommandText() {
			return "Generate items";
		}
		protected virtual void AddItem(ModelItem layoutControl, ModelItem item) {
			layoutControl.Content.Collection.Add(item);
		}
		protected virtual ModelItem CreateItem(EditingContext context, Binding binding) {
			ModelItem item = ModelFactory.CreateItem(context, typeof(DataLayoutItem));
			item.Properties["Binding"].SetValue(binding);			
			return item;
		}
	}
	sealed class LayoutItemPropertyLinesProvider : PropertyLinesProviderBase {
		public LayoutItemPropertyLinesProvider() : base(typeof(LayoutItem)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			Type[] editorTypes = new Type[] { 
				typeof(TextEdit), 
				typeof(SpinEdit), 
				typeof(CheckEdit), 
				typeof(ComboBoxEdit), 
				typeof(DateEdit), 
				typeof(ImageEdit),
				typeof(ButtonEdit), 
				typeof(MemoEdit),
				typeof(TrackBarEdit),
				typeof(ProgressBarEdit),
				typeof(PopupImageEdit),
				typeof(PopupCalcEdit),
				typeof(PopupColorEdit),
				typeof(ListBoxEdit),
				typeof(PasswordBoxEdit), 
			};
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => LayoutItem.LabelProperty)));
			lines.Add(() => new ItemListPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => LayoutItem.ContentProperty), typeof(object), DXTypeInfoInstanceSource.FromTypeList(editorTypes)));
			return lines;
		}
	}
	sealed class LayoutGroupPropertyLinesProvider : PropertyLinesProviderBase {
		public LayoutGroupPropertyLinesProvider() : base(typeof(LayoutGroup)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			Type selectedItemType = viewModel.RuntimeSelectedItem.ItemType;
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => LayoutGroup.DataContextProperty)));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(new CreateGroupCommandProvider(viewModel)));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(new CreateItemCommandProvider(viewModel)));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(new CreateTabbedItemCommandProvider(viewModel)));
			if(typeof(Platform::DevExpress.Xpf.LayoutControl.DataLayoutControl).IsAssignableFrom(selectedItemType)) {
				lines.Add(() => new SeparatorLineViewModel(viewModel));
				lines.Add(() => ActionPropertyLineViewModel.CreateLine(new GenerateItemsCommandProvider(viewModel)));
			}
			if(!typeof(Platform::DevExpress.Xpf.LayoutControl.LayoutControl).IsAssignableFrom(selectedItemType)) {
				lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => LayoutGroup.HeaderProperty)));
				lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => LayoutGroup.HorizontalAlignmentProperty), typeof(HorizontalAlignment)));
				lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => LayoutGroup.VerticalAlignmentProperty), typeof(VerticalAlignment)));
				lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => LayoutGroup.OrientationProperty), typeof(Orientation)));
				lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => LayoutGroup.ViewProperty), typeof(LayoutGroupView)));
			}
			return lines;
		}
	}
	static class DataLayoutControlPropertyLinesRegistrator {
		public static void RegisterPropertyLines() {
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new DataLayoutControlPropertyLinesProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new LayoutControlPropertyLinesProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new LayoutItemPropertyLinesProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new LayoutGroupPropertyLinesProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new TileLayoutControlPropertyLinesProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new TilePropertyLinesProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new DockLayoutControlPropertyLinesProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new FlowLayoutControlPropertyLinesProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new GroupBoxPropertyLinesProvider());
		}
	}
}
