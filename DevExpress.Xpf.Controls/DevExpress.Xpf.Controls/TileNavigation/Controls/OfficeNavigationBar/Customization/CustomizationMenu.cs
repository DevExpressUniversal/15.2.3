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
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.WindowsUI.Base;
using DevExpress.Xpf.WindowsUI.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
namespace DevExpress.Xpf.Navigation.NavigationBar.Customization {
	class DefaultNavigationBarMenuItemNames {
		public const string CustomizationNavigationOptions = "ItemCustomizationNavigationOptions";
		public const string CustomizationIsCompact = "ItemCustomizationIsCompact";
		public const string CustomizationMaxItemsCount = "ItemCustomizationnMaxItemsCount";
	}
	public abstract class NavigationBarMenuBase : CustomizablePopupMenuBase {
		#region static
		public static readonly DependencyProperty GridMenuInfoProperty;
		static NavigationBarMenuBase() {
			Type ownerType = typeof(NavigationBarMenuBase);
			GridMenuInfoProperty = DependencyPropertyManager.RegisterAttached("GridMenuInfo", typeof(NavigationBarMenuInfoBase),
				ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
		}
		public static void SetGridMenuInfo(DependencyObject element, NavigationBarMenuInfoBase value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(GridMenuInfoProperty, value);
		}
		public static NavigationBarMenuInfoBase GetGridMenuInfo(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (NavigationBarMenuInfoBase)element.GetValue(GridMenuInfoProperty);
		}
		#endregion
		public NavigationBarMenuBase(ILogicalOwner owner)
			: base(owner) {
		}
		protected override void UpdateMenuInfoAttachedProperty(BarManager manager, MenuInfoBase menuInfo) {
			NavigationBarMenuInfoBase navigationMenuInfo = menuInfo as NavigationBarMenuInfoBase;
			foreach(BarItem item in GetItems()) {
				SetGridMenuInfo(item, navigationMenuInfo);
			}
		}
		protected override bool ShouldClearItemsOnClose { get { return true; } }
		public virtual BarEditItem CreateBarSpinEditItem(string name, object content, bool beginGroup, ImageSource glyph, ICommand command, BarItemLinkCollection links, decimal value, decimal minValue, decimal maxValue) {
			BarEditItem barItem = new CommandBarEditItem() { Name = name, Content = content, Glyph = glyph, EditValue = value, Command = command };
			barItem.EditSettings = new SpinEditSettings() { MinValue = minValue, MaxValue = maxValue, };
			AddItemCore(barItem, beginGroup, links);
			return barItem;
		}
		class CommandBarEditItem : BarEditItem {
			protected override void OnEditValueChanged() {
				base.OnEditValueChanged();
				if(Command != null) Command.Execute(EditValue);
			}
		}
	}
	public abstract class NavigationBarMenuInfoBase : MenuInfoBase {
		public new NavigationBarMenuBase Menu { get { return (NavigationBarMenuBase)base.Menu; } }
		protected NavigationBarMenuInfoBase(NavigationBarMenuBase menu)
			: base(menu) {
		}
		string GetLocalizedString(NavigationStringId id) {
			return NavigationLocalizer.GetString(id);
		}
		protected virtual BarButtonItem CreateBarButtonItem(string name, NavigationStringId id, bool beginGroup, ImageSource image, ICommand command) {
			return CreateBarButtonItem(name, GetLocalizedString(id), beginGroup, image, command);
		}
		protected virtual BarEditItem CreateBarSpinEditItem(string name, NavigationStringId id, bool beginGroup, ImageSource image, ICommand command, decimal value, decimal minValue, decimal maxValue) {
			return Menu.CreateBarSpinEditItem(name, GetLocalizedString(id), beginGroup, image, command, Menu.ItemLinks, value, minValue, maxValue);
		}
		protected virtual BarCheckItem CreateBarCheckItem(string name, NavigationStringId id, bool? isChecked, bool beginGroup, ImageSource image, ICommand command) {
			return CreateBarCheckItem(name, GetLocalizedString(id), isChecked, beginGroup, image, command);
		}
		protected virtual BarSubItem CreateBarSubItem(string name, NavigationStringId id, bool beginGroup, ImageSource image, ICommand command) {
			return CreateBarSubItem(name, GetLocalizedString(id), beginGroup, image, command);
		}
		protected virtual BarCheckItem CreateBarCheckItem(string name, object content, bool? isChecked, bool beginGroup, ImageSource image, ICommand command) {
			BarCheckItem item = Menu.CreateBarCheckItem(name, content, isChecked, beginGroup, image, Menu.ItemLinks);
			AssignCommand(item, command, TargetElement);
			return item;
		}
		protected virtual BarButtonItem CreateBarButtonItem(string name, object content, bool beginGroup, ImageSource image, ICommand command) {
			BarButtonItem item = Menu.CreateBarButtonItem(name, content, beginGroup, image, Menu.ItemLinks);
			AssignCommand(item, command, TargetElement);
			return item;
		}
		protected virtual BarSubItem CreateBarSubItem(string name, string content, bool beginGroup, ImageSource image, ICommand command) {
			BarSubItem item = Menu.CreateBarSubItem(name, content, beginGroup, image, Menu.ItemLinks);
			AssignCommand(item, command, TargetElement);
			return item;
		}
		void AssignCommand(BarItem item, ICommand command, IInputElement commandTarget) {
			if(item != null) {
				item.CommandTarget = commandTarget;
				item.Command = command;
			}
		}
		public static BarManagerMenuController CreateMenuController(PopupMenu menu) {
			return new BarManagerMenuController(menu);
		}
	}
	public class NavigationBarMenu : NavigationBarMenuBase {
		public NavigationBarMenu(OfficeNavigationBar owner)
			: base(owner) {
		}
		protected override MenuInfoBase CreateMenuInfo(UIElement placementTarget) {
			return new NavigationBarMenuInfo(this);
		}
	}
	public class NavigationBarMenuInfo : NavigationBarMenuInfoBase {
		public NavigationBarMenuInfo(NavigationBarMenu menu)
			: base(menu) {
		}
		public new NavigationBarMenu Menu { get { return (NavigationBarMenu)base.Menu; } }
		internal OfficeNavigationBar NavigationBar { get { return Menu.Owner as OfficeNavigationBar; } }
		public override bool CanCreateItems {
			get { return true; }
		}
		static int menuItemCounter = 0;
		public static string GetUniqueMenuItemName() {
			return string.Format("MenuItem{0}", ++menuItemCounter);
		}
		protected override void CreateItems() {
			CreateBarButtonItem(DefaultNavigationBarMenuItemNames.CustomizationNavigationOptions, NavigationStringId.CustomizationMenu_NavigationOptions, false, null,
				DelegateCommandFactory.Create(() => { NavigationBar.CustomizationHelper.ShowCustomizationForm(NavigationBar); })
				);
			int actualHiddenItemsCount = NavigationBar.ActualHiddenItemsCount;
			if(actualHiddenItemsCount > 0) {
				int itemsCount = 0;
				for(int i = NavigationBar.Items.Count - actualHiddenItemsCount; i < NavigationBar.Items.Count; i++) {
					NavigationBarItem container = NavigationBar.ItemContainerGenerator.ContainerFromIndex(i) as NavigationBarItem;
					if(container != null) {
						object content = container.CustomizationCaption ?? (container.Content is UIElement ? container.Content.ToString() : container.Content);
						INavigationItem navigationItem = content as INavigationItem;
						if(navigationItem != null) content = navigationItem.Header;
						DataTemplate template = container.CustomizationCaptionTemplate ?? container.ContentTemplate;
						DataTemplateSelector templateSelector = container.CustomizationCaptionTemplateSelector ?? container.ContentTemplateSelector;
						var barItem = CreateBarCheckItem(GetUniqueMenuItemName(), content, container.IsSelected, itemsCount == 0, null,
							DelegateCommandFactory.Create(() => { container.IsSelected = true; }));
						barItem.ContentTemplate = template;
						barItem.ContentTemplateSelector = templateSelector;
						itemsCount++;
					}
				}
			}
		}
		public override BarManagerMenuController MenuController {
			get { return new BarManagerMenuController(Menu); }
		}
	}
}
