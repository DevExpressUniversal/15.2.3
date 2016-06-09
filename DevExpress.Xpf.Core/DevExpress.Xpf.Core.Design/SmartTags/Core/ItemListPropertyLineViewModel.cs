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
using DevExpress.Design.UI;
using Platform::DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.Design.SmartTags {
	public class ItemListPropertyLineViewModel : PropertyLineViewModelBase {
		InstanceSourceBase[] items;
		Func<IEnumerable<InstanceSourceBase>> itemsGetter;
		public virtual InstanceSourceBase[] Items { 
			get {
				if (itemsGetter == null)
					return items;
				return itemsGetter.Invoke().ToArray();
			} 
		}
		public Action<IModelItem, IModelItem> SelectedTypeSourceChanged { get; set; }
		public AutoCompleteInfo AutoCompleteInfo {
			get { return autoCompleteInfo; }
			set { SetProperty(ref autoCompleteInfo, value, () => AutoCompleteInfo); }
		}
		InstanceSourceBase selectedTypeSource;
		public InstanceSourceBase SelectedTypeSource {
			get { return selectedTypeSource; }
			set { SetProperty(ref selectedTypeSource, value, () => SelectedTypeSource, () => OnSelectedTypeSourceChanged()); }
		}
		Dictionary<string, Func<IModelItem, IModelItem, bool>> skipCopyingProperty = new Dictionary<string, Func<IModelItem, IModelItem, bool>>();
		public Dictionary<string, Func<IModelItem, IModelItem, bool>> SkipCopyingProperty { get { return skipCopyingProperty; } }
		bool CanCopyProperty(IModelItem oldValue, IModelItem currentValue, IModelProperty property) {
			if(SkipCopyingProperty == null || SkipCopyingProperty.Count == 0 || !SkipCopyingProperty.ContainsKey(property.Name))
				return true;
			return !SkipCopyingProperty[property.Name](oldValue, currentValue);
		}
		void OnSelectedTypeSourceChanged() {
			if(SelectedTypeSource != null) {
				var oldValue = ModelPropertyValue;
				PropertyValue = SelectedTypeSource.Resolve(SelectedItem);
				CopyPropertyValues(oldValue, ModelPropertyValue);
				if(SelectedTypeSourceChanged != null)
					SelectedTypeSourceChanged(oldValue, ModelPropertyValue);
			}
		}
		void CopyPropertyValues(IModelItem oldValue, IModelItem currentValue) {
			if(oldValue == null || currentValue == null) return;
			using(var scope = currentValue.BeginEdit("")) {
				foreach(IModelProperty property in oldValue.Properties) {
					if(!property.IsSet || property.IsReadOnly) continue;
					var newProperty = currentValue.Properties.Find(property.Name);
					if(newProperty != null && CanCopyProperty(oldValue, currentValue, property))
						newProperty.SetValue(property.Value);
				}
				scope.Complete();
			}
		}
		public ItemListPropertyLineViewModel(IPropertyLineContext context, string propertyName, Type propertyType, IEnumerable<InstanceSourceBase> items, Type propertyOwnerType = null)
			: base(context, propertyName, propertyType, propertyOwnerType, context.PlatformInfoFactory.Default()) {
			this.items = items.ToArray();
			UpdateSelectedTypeSource();
		}
		public ItemListPropertyLineViewModel(IPropertyLineContext context, string propertyName, Type propertyType, Func<IEnumerable<InstanceSourceBase>> itemsGetter, Type propertyOwnerType = null)
			: base(context, propertyName, propertyType, propertyOwnerType, context.PlatformInfoFactory.Default()) {
				this.itemsGetter = itemsGetter;
			UpdateSelectedTypeSource();
		}
		void UpdateSelectedTypeSource() {
			if (ModelProperty == null)
				return;
			InstanceSourceBase source = !ModelProperty.IsSet ? null : Items.FirstOrDefault(s => s.IsMatchedObject(ModelProperty.Value));
			SetProperty(ref selectedTypeSource, source, () => SelectedTypeSource);
		}
		public override void OnSelectedItemPropertyChanged(string propertyName) {
			base.OnSelectedItemPropertyChanged(propertyName);
			UpdateSelectedTypeSource();
		}
		AutoCompleteInfo autoCompleteInfo;
	}
	public class AutoCompleteInfo : WpfBindableBase {
		public bool StaysOpenOnEdit {
			get { return staysOpenOnEdit; }
			set { SetProperty(ref staysOpenOnEdit, value, () => StaysOpenOnEdit); }
		}
		public bool IsTextSearchCaseSensitive {
			get { return isTextSearchCaseSensitive; }
			set { SetProperty(ref isTextSearchCaseSensitive, value, () => IsTextSearchCaseSensitive); }
		}
		public string TextSearchText {
			get { return textSearchText; }
			set { SetProperty(ref textSearchText, value, () => TextSearchText); }
		}
		bool staysOpenOnEdit, isTextSearchCaseSensitive;
		string textSearchText;
	}
}
