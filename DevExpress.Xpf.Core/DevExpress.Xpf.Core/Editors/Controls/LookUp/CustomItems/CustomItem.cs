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

using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Editors.Themes;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Editors {
	public class CustomItem : DependencyObject, ICloneable, ICustomItem {
		public static IEnumerable<object> FilterCustomItems(IEnumerable<object> items) {
			return items.With(x => x.Where(item => FilterCustomItem(item) != null));
		}
		public static object FilterCustomItem(object item) {
			CustomItem customItem = item as CustomItem;
			return customItem == null || !customItem.ShouldFilter ? item : null;
		}
		public static readonly DependencyProperty DisplayTextProperty;
		public static readonly DependencyProperty ItemTemplateProperty;
		public static readonly DependencyProperty ItemContainerStyleProperty;
		public static readonly DependencyProperty EditValueProperty;
		static CustomItem() {
			Type ownerType = typeof(CustomItem);
			DisplayTextProperty = DependencyPropertyManager.Register("DisplayText", typeof(string), ownerType,
				new FrameworkPropertyMetadata(null, (d, e) => ((CustomItem)d).UpdateProperties()));
			EditValueProperty = DependencyPropertyManager.Register("EditValue", typeof(object), ownerType,
				new FrameworkPropertyMetadata(null, (d, e) => ((CustomItem)d).UpdateProperties()));
			ItemTemplateProperty = DependencyPropertyManager.Register("ItemTemplate", typeof(DataTemplate), ownerType,
				new FrameworkPropertyMetadata(null, (d, e) => ((CustomItem)d).UpdateProperties()));
			ItemContainerStyleProperty = DependencyPropertyManager.Register("ItemContainerStyle", typeof(Style), ownerType,
				new FrameworkPropertyMetadata(null, (d, e) => ((CustomItem)d).UpdateProperties()));
		}
		protected internal virtual bool ShouldFilter { get { return false; } }
		public string DisplayText {
			get { return (string)GetValue(DisplayTextProperty); }
			set { SetValue(DisplayTextProperty, value); }
		}
		public DataTemplate ItemTemplate {
			get { return (DataTemplate)GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}
		public object EditValue {
			get { return GetValue(EditValueProperty); }
			set { SetValue(EditValueProperty, value); }
		}
		public Style ItemContainerStyle {
			get { return (Style)GetValue(ItemContainerStyleProperty); }
			set { SetValue(ItemContainerStyleProperty, value); }
		}
		WeakReference ownerEditReference;
		protected internal ISelectorEdit OwnerEdit {
			get { return ownerEditReference != null ? (ISelectorEdit)ownerEditReference.Target : null; }
			set { ownerEditReference = new WeakReference(value); }
		}
		internal void SetOwnerEdit(ISelectorEdit ownerEdit) {
			if (OwnerEdit != ownerEdit) {
				OwnerEdit = ownerEdit;
				UpdateProperties();
			}
		}
		Locker UpdatePropertiesLocker { get; set; }
		public CustomItem() {
			UpdatePropertiesLocker = new Locker();
		}
		protected virtual Style GetItemStyle() {
			if (ItemContainerStyle != null)
				return ItemContainerStyle;
			return GetItemStyleInternal();
		}
		protected virtual Style GetItemStyleInternal() {
			ISelectorEdit editor = OwnerEdit;
			if (editor == null)
				return null;
			ISelectorEditStyleSettings settings = (ISelectorEditStyleSettings)((BaseEdit)editor).PropertyProvider.StyleSettings;
			return settings.GetItemContainerStyle(editor);
		}
		protected virtual DataTemplate GetTemplate() {
			if (ItemTemplate != null)
				return ItemTemplate;
			FrameworkElement editor = (FrameworkElement)OwnerEdit;
			if (editor == null)
				return null;
			return (DataTemplate)editor.FindResource(new CustomItemThemeKeyExtension {
				ResourceKey = CustomItemThemeKeys.DefaultTemplate,
			});
		}
		protected virtual ICustomItem GetCustomItem() {
			if (this.IsPropertyAssigned(EditValueProperty))
				return new EditorCustomItem { EditValue = EditValue, DisplayValue = DisplayText };
			return null;
		}
		protected virtual void UpdateProperties() {
			ISelectorEdit editor = OwnerEdit;
			if (editor == null)
				return;
			UpdatePropertiesLocker.DoLockedActionIfNotLocked(() => {
				DisplayText = GetDisplayText();
				EditValue = GetEditValue();
				ItemTemplate = GetTemplate();
				ItemContainerStyle = GetItemStyle();
			});
		}
		protected string GetDisplayText() {
			return DisplayText ?? GetDisplayTextInternal();
		}
		protected object GetEditValue() {
			return EditValue ?? GetEditValueInternal();
		}
		protected virtual string GetDisplayTextInternal() {
			return string.Empty;
		}
		protected virtual object GetEditValueInternal() {
			return null;
		}
		#region Implementation of ICloneable
		object ICloneable.Clone() {
			var item = (CustomItem)Activator.CreateInstance(GetType());
			Assign(item);
			return item;
		}
		protected virtual void Assign(CustomItem item) {
			item.DisplayText = DisplayText;
			item.EditValue = EditValue;
			item.ItemContainerStyle = ItemContainerStyle;
			item.ItemTemplate = ItemTemplate;
		}
		#endregion
		#region ICustomItem Members
		object ICustomItem.DisplayValue {
			get { return DisplayText; }
			set { DisplayText = Convert.ToString(value); }
		}
		#endregion
	}
}
