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
using DevExpress.Design.SmartTags;
using DevExpress.Design.UI;
using Microsoft.Windows.Design.Model;
using Platform::DevExpress.Utils;
using Platform::DevExpress.Xpf.Bars;
using Platform::DevExpress.Xpf.Editors.Settings;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;
#if !SL
using DevExpress.Xpf.Core.Native;
using System.Windows.Media.Imaging;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Core.Design.Services;
#else
using Platform::System.Windows.Media.Imaging;
using DevExpress.Xpf.Core.Design.CoreUtils;
#endif
namespace DevExpress.Xpf.Core.Design {
	public sealed class LinksHolderPropertyLinesProvider : PropertyLinesProviderBase {
		public LinksHolderPropertyLinesProvider() : base(typeof(ILinksHolder)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new ActionListPropertyLineViewModel(new BarsActionListPropertyLineContext(viewModel)) { Text = "Add" });
			return lines;
		}
	}
	public sealed class BarPropertyLinesProvider : PropertyLinesProviderBase {
		public BarPropertyLinesProvider() : base(typeof(Bar)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => Bar.CaptionProperty)));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => Bar.IsMainMenuProperty)));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => Bar.IsStatusBarProperty)));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => Bar.ShowDragWidgetProperty)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => Bar.UseWholeRowProperty), typeof(DefaultBoolean)));
			return lines;
		}
	}
	public abstract class ToolBarControlPropertyLinesProviderBase : PropertyLinesProviderBase {
		public ToolBarControlPropertyLinesProviderBase(Type type) : base(type) {
			if(!typeof(ToolBarControlBase).IsAssignableFrom(type))
				throw new ArgumentException();
		}
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new ActionListPropertyLineViewModel(new BarsActionListPropertyLineContext(viewModel)) { Text = "Add" });
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => ToolBarControlBase.AllowCustomizationMenuProperty)));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => ToolBarControlBase.AllowHideProperty)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => ToolBarControlBase.BarItemDisplayModeProperty), typeof(BarItemDisplayMode)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => ToolBarControlBase.CaptionProperty)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => ToolBarControlBase.ItemsSourceProperty)));
			return lines;
		}
	}
	public sealed class MainMenuControlPropertyLinesProvider : ToolBarControlPropertyLinesProviderBase {
		public MainMenuControlPropertyLinesProvider()
			: base(typeof(MainMenuControl)) {
		}
	}
	public sealed class ToolBarControlPropertyLinesProvider : ToolBarControlPropertyLinesProviderBase {
		public ToolBarControlPropertyLinesProvider()
			: base(typeof(ToolBarControl)) {
		}
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			var lines = base.GetPropertiesImpl(viewModel);
			lines.Insert(1, () => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => ToolBarControl.AllowCollapseProperty)));
			lines.Insert(4, () => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => ToolBarControl.AllowQuickCustomizationProperty)));
			lines.Insert(7, () => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => ToolBarControl.IsCollapsedProperty)));
			lines.Insert(9, () => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => ToolBarControl.OrientationProperty), typeof(Orientation)));
			lines.Insert(10, () => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => ToolBarControl.RotateWhenVerticalProperty)));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => ToolBarControl.ShowDragWidgetProperty)));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => ToolBarControl.UseWholeRowProperty)));
			return lines;
		}
	}
	public sealed class StatusBarControlPropertyLinesProvider : ToolBarControlPropertyLinesProviderBase {
		public StatusBarControlPropertyLinesProvider()
			: base(typeof(StatusBarControl)) {
		}
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			var lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => StatusBarControl.ShowSizeGripProperty)));
			return lines;
		}
	}
	public class BarsActionListPropertyLineContext : ActionListPropertyLineContext {
		public static event EventHandler<LinksHolderItemTypeEventArgs> BarItemAdding;
		ModelItem SelectedModelItem { get { return XpfModelItem.ToModelItem(Context.ModelItem); } }
		MenuItemInfo EditInfo { get; set; }
		MenuItemInfo AvailableItemLinks { get; set; }
		ICommand CreateNewBarItemLink { get; set; }
		static BarsActionListPropertyLineContext() {
		}
		public BarsActionListPropertyLineContext(IPropertyLineContext context)
			: base(context) {
		}
		protected override void InitializeCommands() {
			base.InitializeCommands();
			CreateNewBarItemLink = (ICommand)new WpfDelegateCommand<object>(CreateNewBarItemLinkAction, false);
		}
		protected override void InitializeItems() {
			ObservableCollection<MenuItemInfo> items = new ObservableCollection<MenuItemInfo>();
			foreach(Type itemType in TypeLists.BarItemTypes) {
				if(SkipItem(itemType)) continue;
				items.Add(new MenuItemInfo() { Command = SetSelectedItemCommand, Caption = itemType.Name, Tag = itemType });
			}
			EditInfo = new MenuItemInfo() { Command = SetSelectedItemCommand, Caption = typeof(BarEditItem).Name, Tag = typeof(BarEditItem) };
			InitializeEditInfo(EditInfo);
			items.Add(EditInfo);
			InitializeExistingItems();
			items.Add(AvailableItemLinks);
			Items = items;
			SelectedItem = items.Count > 0 ? items[0] : null;
		}
		protected override void OnSelectedItemExecute(MenuItemInfo param) {
			ModelItem targetItem = XpfModelItem.ToModelItem(Context.ModelItem);
			ModelItem item = typeof(BaseEditSettings).IsAssignableFrom((Type)SelectedItem.Tag) ?
				BarManagerDesignTimeHelper.CreateBarEditItem(SelectedModelItem.Context, (Type)SelectedItem.Tag) :
				BarManagerDesignTimeHelper.CreateBarItem(SelectedModelItem.Context, (Type)SelectedItem.Tag);
			using(ModelEditingScope scope = targetItem.BeginEdit(string.Format("Add {0}", item.ItemType.Name))) {
				var property = GetTargetProperty(targetItem);
				if(property != null)
					property.Collection.Add(item);
				scope.Complete();
			}
			UpdateItemLinks();
		}
		protected override void OnSelectedItemChanged(MenuItemInfo oldValue) {
			base.OnSelectedItemChanged(oldValue);
			UpdateIsChecked();
		}
		protected virtual ModelItem GetTargetItem(ModelItem selectedItem) {
			return BarManagerDesignTimeHelper.FindParentByType<ILinksHolder>(selectedItem);
		}
		protected virtual ModelProperty GetTargetProperty(ModelItem targetItem) {
			if(targetItem.Content.IsCollection && targetItem.Content.IsPropertyOfType(typeof(CommonBarItemCollection)))
				return targetItem.Content;
			string propertyName = "Items";
			if(targetItem.ItemType.Name.Equals("RibbonGalleryBarItem")) {
				propertyName = "DropDownItems";
			}
			var property = targetItem.Properties.Find(propertyName);
			return property;
		}
		protected virtual void InitializeExistingItems() {
			AvailableItemLinks = new MenuItemInfo() { Caption = "Existing Items" };
			UpdateItemLinks();
		}
		protected virtual void CreateNewBarItemLinkAction(object param) {
			MenuItemInfo itemInfo = param as MenuItemInfo;
			ModelItem barItem = itemInfo == null ? null : itemInfo.Tag as ModelItem;
			if(barItem == null) return;
			ModelItem link = BarManagerDesignTimeHelper.CreateBarItemLink(barItem);
			BarManagerDesignTimeHelper.AddBarItemLink(GetTargetProperty(XpfModelItem.ToModelItem(Context.ModelItem)), link);
		}
		protected virtual void InitializeEditInfo(MenuItemInfo editInfo) {
			editInfo.SubItems = new ObservableCollection<MenuItemInfo>();
			foreach(Type editSettingsType in TypeLists.EditSettingsTypes.Where(t => (t.Item2 & PropertyTarget.Bar) != 0).Select(t => t.Item1)) {
				string name = GetBarItemTypeName(editSettingsType);
				editInfo.SubItems.Add(new MenuItemInfo() { Caption = name, Command = SetSelectedItemCommand, Tag = editSettingsType, Image = GetImage(editSettingsType.Name) });
			}
		}
		protected Image GetImage(string imageUri) {
			var uri = GetImageUri(imageUri);
			return BehaviorInfoHelper.Resources.Any(res => uri.ToString().ToLower().Contains(res)) ? new Image() { Source = new BitmapImage(uri) } : null;
		}
		protected virtual Uri GetImageUri(string imageUri) {
			return new Uri(string.Format("pack://application:,,,/{0}.Design;component/Images/EditSettings/{1}.png", AssemblyInfo.SRAssemblyXpfCore, imageUri));
		}
		protected void UpdateItemLinks() {
			if(AvailableItemLinks.SubItems != null)
				AvailableItemLinks.SubItems.Clear();
			var collection = new ObservableCollection<MenuItemInfo>();
			ModelItem barManager = BarManagerDesignTimeHelper.FindBarManagerInParent(SelectedModelItem);
			if(barManager != null) {
				foreach(ModelItem barItem in barManager.Properties["Items"].Collection) {
					if(barItem.Equals(SelectedModelItem) || (!string.Equals(SelectedModelItem.ItemType.Name, "RibbonPageGroup") && string.Equals(barItem.ItemType.Name, "RibbonGalleryBarItem")))
						continue;
					var img = (barItem.Properties["Glyph"].ComputedValue ?? barItem.Properties["LargeGlyph"].ComputedValue) as BitmapImage;
					var menuItem = new MenuItemInfo() {
						Command = CreateNewBarItemLink,
						Caption = barItem.Name, Tag = barItem,
					};
					menuItem.ImageSource = img == null ? null : img.UriSource;
					collection.Add(menuItem);
				}
			}
			AvailableItemLinks.SubItems = collection;
			AvailableItemLinks.IsVisible = AvailableItemLinks.SubItems.Count > 0;
		}
		protected string GetBarItemTypeName(Type newValue) {
			string name = string.Empty;
			if(typeof(BarItem).IsAssignableFrom(newValue)) {
				name = newValue.Name;
			} else if(typeof(BaseEditSettings).IsAssignableFrom(newValue))
				name = string.Format("Bar{0}Item", newValue.Name.Replace("Settings", "")); ;
			return name;
		}
		bool SkipItem(Type itemType) {
			if(BarItemAdding != null) {
				LinksHolderItemTypeEventArgs args = new LinksHolderItemTypeEventArgs(this.Context, itemType);
				BarItemAdding(this, args);
				return args.SkipItem;
			}
			return false;
		}
		void UpdateIsChecked() {
			foreach(var item in Items) {
				item.IsChecked = item == SelectedItem;
			}
			bool isEditItemSelected = SelectedItem != null && SelectedItem.Tag != null && typeof(BaseEditSettings).IsAssignableFrom(((Type)SelectedItem.Tag));
			EditInfo.IsChecked = isEditItemSelected;
			foreach(var item in EditInfo.SubItems) {
				item.IsChecked = item == SelectedItem;
			}
		}
	}
	public class LinksHolderItemTypeEventArgs : EventArgs {
		public IPropertyLineContext Context { get; private set; }
		public Type ItemType { get; private set; }
		public bool SkipItem { get; set; }
		public LinksHolderItemTypeEventArgs(IPropertyLineContext context, Type itemType) {
			Context = context;
			ItemType = itemType;
		}
	}
	sealed class BarManagerPropertyLinesProvider : PropertyLinesProviderBase {
		const string CreateBarText = "Create Bar";
		const string CreateStatusBarText = "Create Status Bar";
		const string CreateMainMenuText = "Create Main Menu";
		public BarManagerPropertyLinesProvider()
			: base(typeof(BarManager)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(new CreateMainMenuActionLineContext(viewModel)));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(new CreateBarActionLineContext(viewModel)));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(new CreateStatusBarActionLineContext(viewModel)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => BarManager.BarsSourceProperty)));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => BarManager.CreateStandardLayoutProperty)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => BarManager.MenuAnimationTypeProperty), typeof(PopupAnimation)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => BarManager.MenuGlyphSizeProperty), typeof(GlyphSize)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => BarManager.ToolbarGlyphSizeProperty), typeof(GlyphSize)));
			return lines;
		}
		class CreateBarActionLineContext : CommandActionLineProvider {
			public CreateBarActionLineContext(IPropertyLineContext context) :
				base(context) { }
			protected override string GetCommandText() {
				return CreateBarText;
			}
			protected override void OnCommandExecute(object param) {
				var barManager = XpfModelItem.ToModelItem(Context.ModelItem);
				if(barManager == null) return;
				BarManagerDesignTimeHelper.AddBar(barManager, ModelFactory.CreateItem(barManager.Context, typeof(Bar)));
			}
		}
		class CreateStatusBarActionLineContext : CommandActionLineProvider {
			public CreateStatusBarActionLineContext(IPropertyLineContext context)
				: base(context) {
				CanExecuteAction = CanExecute;
			}
			bool CanExecute(object param) {
				var barManager = XpfModelItem.ToModelItem(Context.ModelItem);
				return BarManagerDesignTimeHelper.IsStatusBarExist(barManager);
			}
			protected override string GetCommandText() {
				return CreateStatusBarText;
			}
			protected override void OnCommandExecute(object param) {
				var barManager = XpfModelItem.ToModelItem(Context.ModelItem);
				if(barManager == null) return;
				ModelItem bar = ModelFactory.CreateItem(barManager.Context, typeof(Bar));
				bar = BarManagerDesignTimeHelper.ConvertToStatusBar(bar);
				BarManagerDesignTimeHelper.AddBar(barManager, bar);
			}
		}
		class CreateMainMenuActionLineContext : CommandActionLineProvider {
			public CreateMainMenuActionLineContext(IPropertyLineContext context)
				: base(context) {
				CanExecuteAction = CanExecute;
			}
			bool CanExecute(object param) {
				var barManager = XpfModelItem.ToModelItem(Context.ModelItem);
				return BarManagerDesignTimeHelper.IsMainMenuExist(barManager);
			}
			protected override string GetCommandText() {
				return CreateMainMenuText;
			}
			protected override void OnCommandExecute(object param) {
				var barManager = XpfModelItem.ToModelItem(Context.ModelItem);
				if(barManager == null) return;
				ModelItem bar = ModelFactory.CreateItem(barManager.Context, typeof(Bar));
				bar = BarManagerDesignTimeHelper.ConvertToMainMenuBar(bar);
				BarManagerDesignTimeHelper.AddBar(barManager, bar);
			}
		}
	}
}
