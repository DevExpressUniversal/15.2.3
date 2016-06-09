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

using DevExpress.Entity.Model;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Native.ViewGenerator;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using DevExpress.Utils.Filtering;
using DevExpress.Utils.Filtering.Internal;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Settings;
using System;
using System.Windows;
namespace DevExpress.Xpf.LayoutControl {
	public abstract class DataLayoutControlEditorsGeneratorBase : EditorsGeneratorFilteringBase {
		public override EditorsGeneratorTarget Target { get { return EditorsGeneratorTarget.LayoutControl; } }
		protected override EditorsGeneratorMode Mode { get { return EditorsGeneratorMode.Edit; } }
		protected override Type GetLookUpEditType() { return null; }
		protected void ApplyProperties(IEdmPropertyInfo property, IModelItem layoutItem, IModelItem edit, Initializer initializer, bool customizeEditor) {
			initializer.SetEditProperties(layoutItem, edit);
			initializer.SetContainerProperties(layoutItem);
			AttributesApplier.ApplyBaseAttributesForLayoutItem(property, layoutItem, customizeEditor ? edit : null);
			AttributesApplier.ApplyDisplayFormatAttributesForEditor(property, () => customizeEditor ? edit : null);
		}
	}
	public class DataLayoutControlGenerator : DataLayoutControlEditorsGeneratorBase {
		static readonly double LookupLayoutItemMaxHeight = SystemParameters.PrimaryScreenHeight / 3d;
		readonly DataLayoutControl control;
		readonly Action<FrameworkElement> addItemAction;
		public DataLayoutControlGenerator(DataLayoutControl control, Action<FrameworkElement> addItemAction) {
			this.control = control;
			this.addItemAction = addItemAction;
		}
		protected override void GenerateEditor(IEdmPropertyInfo property, Type editType, Initializer initializer) {
			CreateDataLayoutItem(property, null);
		}
		DataLayoutItem CreateDataLayoutItem(IEdmPropertyInfo property, Func<DataLayoutItem> createItem) {
			var res = control.GenerateItem(property.PropertyType, property, createItem);
			res.Do(addItemAction);
			return res;
		}
		public override void FilterRange(IEdmPropertyInfo property, FilterRangeEditorSettings settings, Initializer initializer) {
			CreateDataLayoutItem(property, null);
		}
		public override void FilterRangeProperty(IEdmPropertyInfo property, FilterRangeEditorSettings settings, Initializer initializer) {
			throw new NotImplementedException();
		}
		public override void FilterLookup(IEdmPropertyInfo property, FilterLookupEditorSettings settings, Initializer initializer) {
			var res = CreateDataLayoutItem(property, null);
			if((settings.EditorType == LookupUIEditorType.Default || settings.EditorType == LookupUIEditorType.List)
				&& DataLayoutItem.IsPropertyHasDefaultValue(res, DataLayoutItem.MaxHeightProperty)) {
				res.MaxHeight = LookupLayoutItemMaxHeight;
			}
		}
		public override void FilterLookupProperty(IEdmPropertyInfo property, FilterLookupEditorSettings settings, Initializer initializer) {
			throw new NotImplementedException();
		}
		public override void FilterBooleanChoice(IEdmPropertyInfo property, FilterBooleanEditorSettings settings, Initializer initializer) {
			CreateDataLayoutItem(property, null);
		}
		public override void FilterBooleanChoiceProperty(IEdmPropertyInfo property, FilterBooleanEditorSettings settings, Initializer initializer) {
			throw new NotImplementedException();
		}
		public override void FilterEnumChoice(IEdmPropertyInfo property, FilterEnumEditorSettings settings, Initializer initializer) {
			CreateDataLayoutItem(property, null);
		}
		public override void FilterEnumChoiceProperty(IEdmPropertyInfo property, FilterEnumEditorSettings settings, Initializer initializer) {
			throw new NotImplementedException();
		}
		public override void GenerateEditorFromResources(IEdmPropertyInfo property, object resourceKey, Initializer initializer) {
			DataTemplate template = GetResourceTemplate(control, resourceKey);
			object content = GetResourceContent<DataLayoutItem, BaseEditSettings, BaseEdit>(template);
			if(content is DataLayoutItem) {
				var dataLayoutItem = (DataLayoutItem)content;
				if(dataLayoutItem.Content != null) {
					ApplyProperties(property,
							new RuntimeEditingContext(dataLayoutItem).GetRoot(),
							new RuntimeEditingContext(dataLayoutItem.Content).GetRoot(), initializer, dataLayoutItem.Content is BaseEdit);
				}
				CreateDataLayoutItem(property, () => dataLayoutItem);
				return;
			}
			CreateDataLayoutItem(property, () => {
				var res = control.CreateItem();
				res.editorTemplate = template;
				return res;
			});
		}
	}
	public class DataLayoutItemGenerator : DataLayoutControlEditorsGeneratorBase {
		public FrameworkElement Content { get; private set; }
		public DataLayoutItem Owner { get; private set; }
		bool allowGenerateContent;
		public DataLayoutItemGenerator(DataLayoutItem owner, bool allowGenerateContent) {
			Owner = owner;
			this.allowGenerateContent = allowGenerateContent;
			if(!this.allowGenerateContent) Content = owner.Content as FrameworkElement;
		}
		protected override void GenerateEditor(IEdmPropertyInfo property, Type editType, Initializer initializer) {
			IEditingContext context = new RuntimeEditingContext(Owner);
			GenerateEditor(property, context.CreateItem(editType), initializer, true);
		}
		void GenerateEditor(IEdmPropertyInfo property, IModelItem edit, Initializer initializer, bool customizeEditor) {
			if(this.allowGenerateContent) {
				ApplyProperties(property, new RuntimeEditingContext(Owner).GetRoot(), edit, initializer, customizeEditor);
				Content = (FrameworkElement)edit.GetCurrentValue();
				return;
			}
			ApplyProperties(property, new RuntimeEditingContext(Owner).GetRoot(), new RuntimeEditingContext(Content).GetRoot(), initializer, Content is BaseEdit);
		}
		public override void FilterRange(IEdmPropertyInfo property, FilterRangeEditorSettings settings, Initializer initializer) {
			DataTemplate template;
			if(settings.NumericEditorType != null) {
				switch(settings.NumericEditorType) {
					case RangeUIEditorType.Default: template = GetFilteringTemplate(FilteringUIGeneratorThemeKeys.RangeDefault); break;
					case RangeUIEditorType.Text: template = GetFilteringTemplate(FilteringUIGeneratorThemeKeys.RangeText); break;
					case RangeUIEditorType.Spin: template = GetFilteringTemplate(FilteringUIGeneratorThemeKeys.RangeSpin); break;
					case RangeUIEditorType.Range: template = GetFilteringTemplate(FilteringUIGeneratorThemeKeys.RangeTrack); break;
					default: throw new NotImplementedException();
				}
			} else if(settings.DateTimeEditorType != null) {
				switch(settings.DateTimeEditorType) {
					case DateTimeRangeUIEditorType.Calendar:
					case DateTimeRangeUIEditorType.Range:
					case DateTimeRangeUIEditorType.Default: template = GetFilteringTemplate(FilteringUIGeneratorThemeKeys.DateTimeRangeDefault); break;
					case DateTimeRangeUIEditorType.Picker: template = GetFilteringTemplate(FilteringUIGeneratorThemeKeys.DateTimeRangePicker); break;
					default: throw new NotImplementedException();
				}
			} else throw new InvalidOperationException();
			GenerateEditorFromResourcesCore(property, template, CustomizeInitializer(initializer, true, true));
		}
		public override void FilterRangeProperty(IEdmPropertyInfo property, FilterRangeEditorSettings settings, Initializer initializer) {
			GenerateEditor(property, null, initializer, true);
		}
		public override void FilterLookup(IEdmPropertyInfo property, FilterLookupEditorSettings settings, Initializer initializer) {
			DataTemplate template;
			switch(settings.EditorType) {
				case LookupUIEditorType.Default:
				case LookupUIEditorType.List:
					template = settings.UseFlags
						? GetFilteringTemplate(FilteringUIGeneratorThemeKeys.LookupCheckedList)
						: GetFilteringTemplate(FilteringUIGeneratorThemeKeys.LookupList); break;
				case LookupUIEditorType.DropDown:
					template = settings.UseFlags
						? GetFilteringTemplate(FilteringUIGeneratorThemeKeys.LookupCheckedDropDown)
						: GetFilteringTemplate(FilteringUIGeneratorThemeKeys.LookupDropDown); break;
				case LookupUIEditorType.TokenBox:
					template = settings.UseFlags
						? GetFilteringTemplate(FilteringUIGeneratorThemeKeys.LookupToken)
						: GetFilteringTemplate(FilteringUIGeneratorThemeKeys.LookupList); break;
				default: throw new NotImplementedException();
			}
			GenerateEditorFromResourcesCore(property, template, CustomizeInitializer(initializer, true, true));
		}
		public override void FilterLookupProperty(IEdmPropertyInfo property, FilterLookupEditorSettings settings, Initializer initializer) {
			throw new NotImplementedException();
		}
		public override void FilterBooleanChoice(IEdmPropertyInfo property, FilterBooleanEditorSettings settings, Initializer initializer) {
			DataTemplate template;
			switch(settings.EditorType) {
				case BooleanUIEditorType.Check:
				case BooleanUIEditorType.Default:
					template = GetFilteringTemplate(FilteringUIGeneratorThemeKeys.BooleanCheck);
					initializer = CustomizeInitializer(initializer, true, true);
					break;
				case BooleanUIEditorType.Toggle:
					template = GetFilteringTemplate(FilteringUIGeneratorThemeKeys.BooleanToggle);
					initializer = CustomizeInitializer(initializer, true, true);
					break;
				case BooleanUIEditorType.DropDown:
					template = GetFilteringTemplate(FilteringUIGeneratorThemeKeys.BooleanDropDown);
					initializer = CustomizeInitializer(initializer, true, true);
					break;
				case BooleanUIEditorType.List:
					template = GetFilteringTemplate(FilteringUIGeneratorThemeKeys.BooleanList);
					initializer = CustomizeInitializer(initializer, true, true);
					break;
				default: throw new NotImplementedException();
			}
			GenerateEditorFromResourcesCore(property, template, initializer);
		}
		public override void FilterBooleanChoiceProperty(IEdmPropertyInfo property, FilterBooleanEditorSettings settings, Initializer initializer) {
			throw new NotImplementedException();
		}
		public override void FilterEnumChoice(IEdmPropertyInfo property, FilterEnumEditorSettings settings, Initializer initializer) {
			DataTemplate template;
			switch(settings.EditorType) {
				case LookupUIEditorType.Default:
				case LookupUIEditorType.List:
					template = settings.UseFlags 
						? GetFilteringTemplate(FilteringUIGeneratorThemeKeys.EnumCheckedList) 
						: GetFilteringTemplate(FilteringUIGeneratorThemeKeys.EnumList); break;
				case LookupUIEditorType.DropDown:
					template = settings.UseFlags 
						? GetFilteringTemplate(FilteringUIGeneratorThemeKeys.EnumCheckedDropDown) 
						: GetFilteringTemplate(FilteringUIGeneratorThemeKeys.EnumDropDown); break;
				case LookupUIEditorType.TokenBox:
					template = settings.UseFlags 
						? GetFilteringTemplate(FilteringUIGeneratorThemeKeys.EnumToken) 
						: GetFilteringTemplate(FilteringUIGeneratorThemeKeys.EnumList); break;
				default: throw new NotImplementedException();
			}
			GenerateEditorFromResourcesCore(property, template, CustomizeInitializer(initializer, true, true));
		}
		public override void FilterEnumChoiceProperty(IEdmPropertyInfo property, FilterEnumEditorSettings settings, Initializer initializer) {
			throw new NotImplementedException();
		}
		public override void GenerateEditorFromResources(IEdmPropertyInfo property, object resourceKey, Initializer initializer) {
			var template = GetResourceTemplate(resourceKey) ?? Owner.editorTemplate;
			GenerateEditorFromResourcesCore(property, template, initializer);
		}
		void GenerateEditorFromResourcesCore(IEdmPropertyInfo property, DataTemplate resource, Initializer initializer) {
			if(resource == null) return;
			var content = GetResourceContent<DataLayoutItem, BaseEditSettings, BaseEdit>(resource);
			if(content is BaseEditSettings) {
				var editor = ((BaseEditSettings)content).CreateEditor();
				GenerateEditor(property, new RuntimeEditingContext(editor).GetRoot(), initializer, true);
				return;
			}
			if(content is BaseEdit) {
				GenerateEditor(property, new RuntimeEditingContext(content).GetRoot(), initializer, true);
				return;
			}
			GenerateEditor(property, new RuntimeEditingContext(content).GetRoot(), initializer, false);
		}
		Initializer CustomizeInitializer(Initializer initializer, bool clearLabel, bool clearToolTip) {
			return initializer + new Initializer(null, (container) => {
				if(clearLabel) container.SetValue(DataLayoutItem.LabelProperty, null);
				if(clearToolTip) container.SetValue(DataLayoutItem.ToolTipProperty, null);
			});
		}
		DataTemplate GetFilteringTemplate(FilteringUIGeneratorThemeKeys key) {
			return GetResourceTemplate(new FilteringUIGeneratorThemeKeyExtension() {
				IsThemeIndependent = true, ResourceKey = key });
		}
		DataTemplate GetResourceTemplate(object key) {
			FrameworkElement resourceElement = (FrameworkElement)Owner.GetDataLayoutControl() ?? Owner;
			return (DataTemplate)resourceElement.TryFindResource(key);
		}
	}
	public sealed class DataLayoutItemAttributeSettings {
		public static DataLayoutItemAttributeSettings Create(IEdmPropertyInfo propertyInfo) {
			var settings = new Settings();
			var context = new RuntimeEditingContext(settings);
			AttributesApplier.ApplyBaseAttributesForLayoutItem(propertyInfo, context.GetRoot(), context.GetRoot());
			AttributesApplier.ApplyDisplayFormatAttributesForEditor(propertyInfo, () => context.GetRoot());
			return new DataLayoutItemAttributeSettings(settings);
		}
		static DataLayoutItemAttributeSettings empty;
		public static DataLayoutItemAttributeSettings Empty {
			get { return empty ?? (empty = new DataLayoutItemAttributeSettings(null)); }
		}
		public string Label { get; private set; }
		public string ToolTip { get; private set; }
		public bool IsReadOnly { get; private set; }
		public bool IsRequired { get; private set; }
		public string NullText { get; private set; }
		public bool ShowNullTextForEmptyValue { get; private set; }
		public string DisplayFormatString { get; private set; }
		DataLayoutItemAttributeSettings(Settings settings) {
			if(settings == null) {
				Label = null;
				ToolTip = null;
				IsReadOnly = false;
				IsRequired = false;
				NullText = null;
				ShowNullTextForEmptyValue = false;
				DisplayFormatString = null;
				return;
			}
			Label = settings.Label;
			ToolTip = settings.ToolTip;
			IsReadOnly = settings.IsReadOnly;
			IsRequired = settings.IsRequired;
			NullText = settings.NullText;
			ShowNullTextForEmptyValue = settings.ShowNullTextForEmptyValue;
			DisplayFormatString = settings.DisplayFormatString;
		}
		class Settings {
			public string Label { get; set; }
			public string ToolTip { get; set; }
			public bool IsReadOnly { get; set; }
			public bool IsRequired { get; set; }
			public string NullText { get; set; }
			public bool ShowNullTextForEmptyValue { get; set; }
			public string DisplayFormatString { get; set; }
		}
	}
}
