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
using DevExpress.Xpf.Bars.Native;
using DevExpress.Xpf.Core.Mvvm.UI.ViewGenerator.Metadata;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
namespace DevExpress.Xpf.PropertyGrid.Internal {
	public class StandardDefinitionsProvider {
		class CacheRecord {
			public CacheRecord(PropertyDefinitionBase value) {
				Value = value;
				NeedsValidation = false;
			}
			public PropertyDefinitionBase Value { get; private set; }
			public PropertyDefinitionBase ValidValue { get { return NeedsValidation ? null : Value; } }
			public bool NeedsValidation { get; set; }
			public bool Validate(PropertyDefinitionBase value) {
				NeedsValidation = !Value.Equals(value);
				return !NeedsValidation;
			}
		}
		readonly PropertyBuilder builder;
		readonly Dictionary<RowHandle, CacheRecord> cache;
		readonly Stack<RowHandle> currentHandle = new Stack<RowHandle>().Do(x => x.Push(null));
		readonly PropertyGridControl propertyGrid;
		public RowHandle CurrentHandle {
			get { return currentHandle.Peek(); }
			set { if (value == null && CurrentHandle != null) currentHandle.Pop(); else currentHandle.Push(value); }
		}
		public PropertyDefinitionBase CurrentDefinition {
			get {
				if (CurrentHandle == null || !cache.ContainsKey(CurrentHandle))
					return null;
				return cache[CurrentHandle].ValidValue;
			}
			set {
				if (CurrentHandle == null)
					return;
				CacheRecord record;
				if (!cache.TryGetValue(CurrentHandle, out record) || !record.Validate(value)) {
					if (record != null) {
						RemoveLogicalChild(record.Value);
					}
					cache[CurrentHandle] = new CacheRecord(value);
				}
			}
		}
		public IEnumerator<PropertyDefinitionBase> CreatedDefinitions {
			get {
				foreach (var record in cache.Values) {
					if (record.NeedsValidation)
						continue;
					yield return record.ValidValue;
				}
			}
		}
#if DEBUGTEST
		public IEnumerable<PropertyDefinitionBase> DefinitionsForTest { get { return cache.Values.Select(x => x.ValidValue).Where(x => x != null); } }
#endif
		public StandardDefinitionsProvider(PropertyBuilder builder) {
			this.builder = builder;
			this.propertyGrid = builder.PropertyGrid;
			this.cache = new Dictionary<RowHandle, CacheRecord>();
		}
		public void Invalidate(DataViewBase view, RowHandle handle) {
			InvalidateImpl(view, handle, cache.Keys.ToList());
		}
		public void ChangeHandle(RowHandle oldHandle, RowHandle newHandle) {
			CacheRecord cr = null;
			if (cache.TryGetValue(oldHandle, out cr)) {
				cache.Remove(oldHandle);
				cache[newHandle] = cr;
			}
		}
		public void ClearUnlinkedDefinitions() {
			foreach (var key in cache.Keys.ToList()) {
				cache[key].If(x => x.NeedsValidation).Do(x => { cache.Remove(key); RemoveLogicalChild(x.Value); });
			}
		}
		public PropertyDefinitionBase GetStandardDefinition(DataViewBase view, RowHandle handle) {
			if (view.IsGroupRowHandle(handle))
				return GetStandardDefinitionForGroupHandle(view, handle);
			EnsureCacheExists(view, handle);
			CurrentHandle = handle;
			var definition = CurrentDefinition;
			CurrentHandle = null;
			return definition;
		}
		void InvalidateImpl(DataViewBase view, RowHandle handle, List<RowHandle> list) {
			foreach (var childHandle in list) {
				if (GetParentHandle(view, childHandle) == handle)
					InvalidateImpl(view, childHandle, list);
			}
			if (!CacheExists(handle))
				return;
			CurrentHandle = handle;
			var element = CurrentDefinition;
			CurrentHandle = null;
			cache[handle].NeedsValidation = true;
		}
		bool CacheExists(RowHandle handle) {
			return cache.ContainsKey(handle) && !cache[handle].NeedsValidation;
		}
		RowHandle GetParentHandle(DataViewBase view, RowHandle handle) {
			var parentHandle = view.GetParent(handle);
			return view.IsGroupRowHandle(handle) ? view.GetParent(parentHandle) : parentHandle;
		}
		void EnsureCacheExists(DataViewBase view, RowHandle handle) {
			if (handle == null || CacheExists(handle))
				return;
			CurrentHandle = handle;
			var descriptor = view.GetDescriptor(handle);			
			if (descriptor != null) {
				var propertyInfo = new EdmPropertyInfo(descriptor, DataColumnAttributesProvider.GetAttributes(descriptor), false);
				var options = GenerateEditorOptions.ForRuntime();
				var context = new RuntimeEditingContext(this);
				PropertyDefinitionsGenerator generator = new PropertyDefinitionsGenerator(context.GetRoot(), propertyGrid, view, handle);
				EditorsSource.GenerateEditor(propertyInfo, generator, null, options.CollectionProperties, options.GuessImageProperties, options.GuessDisplayMembers);
				var cd = CurrentDefinition;
				if (cd == null) {
				}
				CurrentDefinition.isStandardDefinition = true;
				AddLogicalChild(CurrentDefinition);
			}
			CurrentHandle = null;
		}
		PropertyDefinitionBase GetStandardDefinitionForGroupHandle(DataViewBase view, RowHandle handle) {
			var definition = new CollectionDefinition();
			definition.isStandardDefinition = true;
			definition.HeaderShowMode = HeaderShowMode.OnlyHeader;
			cache[handle] = new CacheRecord(definition);
			AddLogicalChild(definition);
			return definition;
		}
		void AddLogicalChild(PropertyDefinitionBase definition) {
			builder.InternalAddLogicalChild(definition);
		}
		void RemoveLogicalChild(PropertyDefinitionBase definition) {
			builder.InternalRemoveLogicalChild(definition);
		}
	}
	sealed class StandardValuesSource : ObservableCollectionConverter<object, object> {
		bool useTypeConverterToStringConversion;
		readonly DataViewBase view;
		readonly RowHandle handle;
		public bool UseTypeConverterToStringConversion {
			get { return useTypeConverterToStringConversion;  }
			set { useTypeConverterToStringConversion = value;
				UpdateSelector();
			}
		}		
		public StandardValuesSource(DataViewBase view, RowHandle handle) {
			this.view = view;
			this.handle = handle;
			Source = view.GetStandardValues(handle);
			UpdateSelector();
		}
		void UpdateSelector() {
			if (useTypeConverterToStringConversion)
				Selector = x => new CustomItem() { EditValue = x, DisplayText = view.ConvertToString(handle, x) };
			else
				Selector = x => x;
		}
	}
	class PropertyDefinitionsGenerator : EditorsGeneratorBase {
		readonly IModelItem definitionsProvider;
		readonly DataViewBase dataView;
		readonly RowHandle handle;
		readonly PropertyGridControl owner;
		public PropertyDefinitionsGenerator(IModelItem definitionsProvider, PropertyGridControl owner, DataViewBase dataView, RowHandle handle) {
			this.definitionsProvider = definitionsProvider;
			this.owner = owner;
			this.dataView = dataView;
			this.handle = handle;
		}
		protected override EditorsGeneratorMode Mode { get { return EditorsGeneratorMode.EditSettings; } }
		public override EditorsGeneratorTarget Target { get { return EditorsGeneratorTarget.PropertyGrid; } }
		protected override Type GetLookUpEditType() { return null; }
		void GenerateEditor(IEdmPropertyInfo property, IModelItem propertyDefinition, IModelItem editSettings, Initializer initializer) {
			propertyDefinition = propertyDefinition ?? definitionsProvider.Properties["CurrentDefinition"].If(x => x.IsSet).With(x => x.Value);
			propertyDefinition = propertyDefinition ?? definitionsProvider.Context.CreateItem(typeof(PropertyDefinition));
			AttributesApplier.ApplyBaseAttributes(property,
				setDisplayMember: x => propertyDefinition.SetValueIfNotSet(PropertyDefinitionBase.PathProperty, x),
				setDisplayName: x => propertyDefinition.SetValueIfNotSet(PropertyDefinitionBase.HeaderProperty, x),
				setDisplayShortName: null,
				setDescription: x => propertyDefinition.SetValueIfNotSet(PropertyDefinitionBase.DescriptionProperty, x),
				setReadOnly: () => propertyDefinition.SetValueIfNotSet(PropertyDefinitionBase.IsReadOnlyProperty, true),
				setEditable: x => propertyDefinition.SetValueIfNotSet(PropertyDefinitionBase.IsReadOnlyProperty, x),
				setHidden: null,
				setInvisible: null,
				setRequired: null);
			AttributesApplier.ApplyDisplayFormatAttributesForEditSettings(property,
				() => editSettings ?? (editSettings = propertyDefinition.Context.CreateItem(typeof(TextEditSettings))));
			initializer.SetContainerProperties(propertyDefinition);
			if(editSettings != null) {
				initializer.SetEditProperties(propertyDefinition, editSettings);
				propertyDefinition.SetValue(PropertyDefinition.EditSettingsProperty, editSettings);
			}
			definitionsProvider.Properties["CurrentDefinition"].SetValue(propertyDefinition);
		}
		protected override void GenerateEditor(IEdmPropertyInfo property, Type editType, Initializer initializer) {
			IModelItem propertyDefinition = definitionsProvider.Properties["CurrentDefinition"].If(x => x.IsSet).With(x => x.Value);
			IModelItem editSettings = null;
			if(propertyDefinition != null) {
				editSettings = propertyDefinition.Properties["EditSettings"].If(x => x.IsSet).With(x => x.Value);
				if(!Equals(editSettings.With(x => x.ItemType), (editType ?? typeof(TextEditSettings))))
					editSettings = null;
			} else propertyDefinition = definitionsProvider.Context.CreateItem(typeof(PropertyDefinition));
			editSettings = editSettings ?? (editType != null ? definitionsProvider.Context.CreateItem(editType) : null);
			GenerateEditor(property, propertyDefinition, editSettings, initializer);
		}
		bool InitializeStandardValuesEditor(IEdmPropertyInfo property) {
			if (dataView.GetIsStandardValuesSupported(handle)) {
				GenerateEditor(property, typeof(ComboBoxEditSettings), StandardValuesEditorInitializer(property).Value);
				return true;
			}
			return false;
		}
		Initializer? StandardValuesEditorInitializer(IEdmPropertyInfo property) {
			if (dataView.GetIsStandardValuesSupported(handle)) {
				return new Initializer((container, settings) => {
					settings.SetValueIfNotSet(LookUpEditSettingsBase.ItemsSourceProperty, new StandardValuesSource(dataView, handle), false);
					if (dataView.GetIsStandardValuesExclusive(handle)) {
						settings.SetValueIfNotSet(ButtonEditSettings.IsTextEditableProperty, false, false);
						settings.SetValueIfNotSet(LookUpEditSettingsBase.AllowRejectUnknownValuesProperty, true, false);
					}					
					if (property.HasNullableType())
						settings.SetValueIfNotSet(ButtonEditSettings.NullValueButtonPlacementProperty, EditorPlacement.EditBox, false);
				});
			}
			return null;
		}
		public override void Text(IEdmPropertyInfo property, bool multiline) {
			if (!InitializeStandardValuesEditor(property))
				base.Text(property, multiline);
		}
		protected override Initializer TextInitializer(IEdmPropertyInfo property, bool multiline, int? maxLength = default(int?)) {
			return StandardValuesEditorInitializer(property) ?? base.TextInitializer(property, multiline, maxLength);
		}
		public override void Numeric(IEdmPropertyInfo property, MaskInfo mask) {
			if (!InitializeStandardValuesEditor(property))
				base.Numeric(property, mask);
		}
		protected override Initializer NumericInitializer(IEdmPropertyInfo property, MaskInfo mask, Type editValueType = null, bool? nullable = default(bool?), int? maxLength = default(int?)) {
			return StandardValuesEditorInitializer(property) ?? base.NumericInitializer(property, mask, editValueType, nullable, maxLength);
		}		
		public override void Object(IEdmPropertyInfo property) {			
			Type editType = null;
			if (property.PropertyType.If(x => typeof(Brush).IsAssignableFrom(x)).ReturnSuccess())
				editType = typeof(PopupBrushEditSettings);
			else if (property.PropertyType.If(x => typeof(Color).IsAssignableFrom(x)).ReturnSuccess())
				editType = typeof(PopupColorEditSettings);
			else if (InitializeStandardValuesEditor(property))
				return;
			GenerateEditor(property, editType, ObjectInitializer(property));
		}
		protected override Initializer ObjectInitializer(IEdmPropertyInfo property) {
			if(property.PropertyType.If(x => typeof(Brush).IsAssignableFrom(x)).ReturnSuccess())
				return new Initializer((container, edit) => {
					((DependencyObject)edit.GetCurrentValue()).SetValue(PropertyGridEditSettingsHelper.PostOnEditValueChangedProperty, true); });
			if(property.PropertyType.If(x => typeof(Color).IsAssignableFrom(x)).ReturnSuccess())
				return new Initializer((container, edit) => {
					((DependencyObject)edit.GetCurrentValue()).SetValue(PropertyGridEditSettingsHelper.PostOnEditValueChangedProperty, true); });
			return base.ObjectInitializer(property);
		}
		public override void GenerateEditorFromResources(IEdmPropertyInfo property, object resourceKey, Initializer initializer) {
			var template = GetResourceTemplate(owner, resourceKey);
			var content = GetResourceContent<PropertyDefinition, BaseEditSettings, BaseEdit>(template);
			EditorFromResourcesCore(property, content, template, initializer);
		}
		void EditorFromResourcesCore(IEdmPropertyInfo property, object content, DataTemplate template, Initializer initializer) {
			if(content is PropertyDefinition) {
				var definition = content as PropertyDefinition;
				IModelItem editSettingsModel = (definition).EditSettings != null ?
					new RuntimeEditingContext((definition).EditSettings).GetRoot() : null;
				definition.isResourceGeneratedDefinition = true;
				GenerateEditor(property, new RuntimeEditingContext(content).GetRoot(), editSettingsModel, initializer);
				return;
			}
			if(content is BaseEditSettings) {
				GenerateEditor(property, null, new RuntimeEditingContext(content).GetRoot(), initializer);
				return;
			}
			Initializer resInitializer = initializer + new Initializer(null, (container) => container.SetValueIfNotSet(PropertyDefinition.CellTemplateProperty, template));
			GenerateEditor(property, null, null, resInitializer);
		}
	}
}
