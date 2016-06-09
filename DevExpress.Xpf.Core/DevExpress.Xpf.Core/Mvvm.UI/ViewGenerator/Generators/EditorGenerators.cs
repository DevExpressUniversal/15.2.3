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
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using DevExpress.Utils.Filtering.Internal;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
namespace DevExpress.Mvvm.UI.Native.ViewGenerator {
	static class EditorsGeneratorSelector {
		internal static void GenerateEditor(IEdmPropertyInfo property, EditorsGeneratorBase generator, Func<IEdmPropertyInfo, ForeignKeyInfo> getForegnKeyProperty, IEnumerable<TypeNamePropertyPair> collectionProperties = null, bool guessImageProperties = false, bool guessDisplayMembers = false, bool skipIEnumerableProperties = false) {
			if(GenerateEditorBasedEditorTemplates(property, generator, guessImageProperties, skipIEnumerableProperties)) return;
			if(GenerateFilterEditor(property, generator as EditorsGeneratorFilteringBase, guessImageProperties, skipIEnumerableProperties)) return;
			if(GenerateLookUp(property, generator, getForegnKeyProperty, collectionProperties, guessDisplayMembers)) return;
			SelectMethod(generator, true, property, property.PropertyType, guessImageProperties, skipIEnumerableProperties);
		}
		static Lazy<EditorsGeneratorBase.Initializer> SelectMethod(EditorsGeneratorBase generator, bool callGenerateMethod,
			IEdmPropertyInfo property, Type propertyType, bool guessImageProperties, bool skipIEnumerableProperties) {
			if(property == null || propertyType == null) return null;
			propertyType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
			var attributes = property.Attributes;
			PropertyDataType dataType = attributes.PropertyDataType();
			RegExMaskType? regExMaskType = MaskInfo.GetRegExMaskType(attributes.Mask(), dataType == PropertyDataType.PhoneNumber ? (RegExMaskType?)RegExMaskType.Simple : null);
			if(IsNumericType(propertyType)) {
				if(MaskInfo.GetRegExMaskType(attributes.Mask(), null) == null) {
					var rangeAttr = attributes.ReadAttributeProperty<RangeAttribute, RangeAttribute>(x => x).Value;
					if(rangeAttr != null) return generator.Range(callGenerateMethod, property, GetNumericMask(property), rangeAttr.Minimum, rangeAttr.Maximum);
					return generator.Numeric(callGenerateMethod, property, GetNumericMask(property));
				}
				return generator.RegExMaskText(callGenerateMethod, property, GetMaskInfo(property, null, true, regExMaskType));
			}
			if(propertyType == typeof(bool)) return generator.Check(callGenerateMethod, property);
			if(propertyType == typeof(string)) {
				if(dataType == PropertyDataType.Password) return generator.Password(callGenerateMethod, property);
				if(dataType == PropertyDataType.MultilineText) return generator.Text(callGenerateMethod, property, true);
				if(dataType == PropertyDataType.ImageUrl) return generator.Image(callGenerateMethod, property, true);
				if(regExMaskType.HasValue) {
					string defaultMask = dataType == PropertyDataType.PhoneNumber ? EditorsSource.PhoneNumberMask : null;
					return generator.RegExMaskText(callGenerateMethod, property, GetMaskInfo(property, defaultMask, string.IsNullOrEmpty(defaultMask), regExMaskType));
				}
				return generator.Text(callGenerateMethod, property, false);
			}
			if(propertyType == typeof(char)) return generator.Char(callGenerateMethod, property);
			if(propertyType == typeof(DateTime)) return generator.DateTime(callGenerateMethod, property);
			if(propertyType == typeof(byte[]) && guessImageProperties && EditorsSource.IsImagePropertyName(property.Name))
				return generator.Image(callGenerateMethod, property, false);
			if(propertyType.IsEnum) return generator.Enum(callGenerateMethod, property, propertyType);
			if(!skipIEnumerableProperties || string.IsNullOrEmpty(property.Name) || !typeof(IEnumerable).IsAssignableFrom(property.PropertyType) || property.Attributes.AutoGenerateField.GetValueOrDefault(false))
				return generator.Object(callGenerateMethod, property);
			return null;
		}
		static bool GenerateEditorBasedEditorTemplates(IEdmPropertyInfo property, EditorsGeneratorBase generator, bool guessImageProperties, bool skipIEnumerableProperties) {
			object templateKey = null;
			templateKey = GetEditorAttributeTemplateKey(generator.Target,
				x => property.Attributes.ReadAttributeProperty<CommonEditorAttributeBase, CommonEditorAttributeBase>(x, y => y));
			if(templateKey == null) {
				var typeEditorAttributes = MetadataHelper.GetExternalAndFluentAPIAttrbutes(Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType, null).OfType<CommonEditorAttributeBase>();
				templateKey = GetEditorAttributeTemplateKey(generator.Target,
					attrType => new Lazy<CommonEditorAttributeBase>(() => {
						return typeEditorAttributes.FirstOrDefault(attr => {
							Type aType = attr.GetType();
							return aType == attrType || aType.IsSubclassOf(attrType);
						});
					}));
			}
			if(templateKey != null) {
				generator.GenerateEditorFromResources(property, templateKey,
					SelectMethod(generator, false, property, property.PropertyType, guessImageProperties, skipIEnumerableProperties).Value);
				return true;
			}
			return false;
		}
		static object GetEditorAttributeTemplateKey(EditorsGeneratorBase.EditorsGeneratorTarget target, Func<Type, Lazy<CommonEditorAttributeBase>> readEditorAttribute) {
			Lazy<CommonEditorAttributeBase> editorAttr = new Lazy<CommonEditorAttributeBase>(() => null);
			Lazy<CommonEditorAttributeBase> defaultAttr = readEditorAttribute(typeof(DefaultEditorAttribute));
			switch(target) {
				case EditorsGeneratorBase.EditorsGeneratorTarget.GridControl:
					editorAttr = readEditorAttribute(typeof(GridEditorAttribute)); break;
				case EditorsGeneratorBase.EditorsGeneratorTarget.LayoutControl:
					editorAttr = readEditorAttribute(typeof(LayoutControlEditorAttribute)); break;
				case EditorsGeneratorBase.EditorsGeneratorTarget.PropertyGrid:
					editorAttr = readEditorAttribute(typeof(PropertyGridEditorAttribute)); break;
			}
			return editorAttr.Value.With(x => x.TemplateKey) ?? defaultAttr.Value.With(x => x.TemplateKey);
		}
		static bool GenerateFilterEditor(IEdmPropertyInfo property, EditorsGeneratorFilteringBase generator, bool guessImageProperties, bool skipIEnumerableProperties) {
			if(generator == null) return false;
			Func<EditorsGeneratorBase.Initializer> getInitializer = () =>
				SelectMethod(generator, false, property, property.PropertyType, guessImageProperties, skipIEnumerableProperties).Value;
			var editorType = property.Attributes.ReadAttributeProperty<FilterEditorAttribute, FilterEditorAttribute>(x => x).Value;
			if(generator == null || !property.PropertyType.GetInterfaces().Contains(typeof(IValueViewModel))) {
				if(editorType != null) {
					if(editorType.RangeEditorSettings != null)
						generator.FilterRangeProperty(property, editorType.RangeEditorSettings, getInitializer());
					else if(editorType.BooleanEditorSettings != null)
						generator.FilterBooleanChoiceProperty(property, editorType.BooleanEditorSettings, getInitializer());
					else if(editorType.LookupEditorSettings != null)
						generator.FilterLookupProperty(property, editorType.LookupEditorSettings, getInitializer());
					else if(editorType.EnumEditorSettings != null)
						generator.FilterEnumChoiceProperty(property, editorType.EnumEditorSettings, getInitializer());
					return true;
				}
				return false;
			}
			var propType = property.PropertyType.GetGenericTypeDefinition();
			var propUnderlyingType = property.PropertyType.GetGenericArguments().First();
			propUnderlyingType = Nullable.GetUnderlyingType(propUnderlyingType) ?? propUnderlyingType;
			if(propType == typeof(IRangeValueViewModel<>))
				generator.FilterRange(property, editorType.RangeEditorSettings, getInitializer());
			else if(propType == typeof(ICollectionValueViewModel<>))
				generator.FilterLookup(property, editorType.LookupEditorSettings, getInitializer());
			else if(propType == typeof(ISimpleValueViewModel<>))
				generator.FilterBooleanChoice(property, editorType.BooleanEditorSettings, getInitializer());
			else if(propType == typeof(IEnumValueViewModel<>))
				generator.FilterEnumChoice(property, editorType.EnumEditorSettings, getInitializer());
			else throw new NotImplementedException();
			return true;
		}
		static bool GenerateLookUp(IEdmPropertyInfo property, EditorsGeneratorBase generator, Func<IEdmPropertyInfo, ForeignKeyInfo> getForegnKeyProperty, IEnumerable<TypeNamePropertyPair> collectionProperties, bool guessDisplayMembers) {
			var lookUpAttr = property.Attributes.ReadAttributeProperty<LookupBindingPropertiesAttribute, LookupBindingPropertiesAttribute>(x => x).Value;
			if(guessDisplayMembers && property.IsNavigationProperty) {
				string displayMemberPropertyName = EditorsSource.GetDisplayMemberPropertyName(property.PropertyType);
				string propertyTypeName = property.GetUnderlyingClrType().FullName;
				TypeNamePropertyPair pair = collectionProperties != null ? collectionProperties.FirstOrDefault(x => x.TypeFullName == propertyTypeName) : null;
				ForeignKeyInfo foreignKeyInfo = getForegnKeyProperty.With(x => x(property));
				generator.LookUp(property, pair != null ? pair.PropertyName : null, displayMemberPropertyName, foreignKeyInfo);
				return true;
			}
			return false;
		}
		static MaskInfo GetNumericMask(IEdmPropertyInfo property) {
			if(MaskInfo.GetRegExMaskType(property.Attributes.Mask(), null) != null) return null;
			bool isFloatNumericType = EditorsSource.NumericFloatTypes.Contains(property.GetUnderlyingClrType());
			PropertyDataType dataType = property.Attributes.PropertyDataType();
			string defaultMask = property.GetActualMask(dataType == PropertyDataType.Currency ? (isFloatNumericType ? "C" : "C0") : (isFloatNumericType ? string.Empty : "d"));
			return GetMaskInfo(property, defaultMask, string.IsNullOrEmpty(defaultMask), null);
		}
		static MaskInfo GetMaskInfo(IEdmPropertyInfo property, string defaultMask, bool defaultNotUseMaskAsDisplayFormat, RegExMaskType? regExMaskType) {
			return MaskInfo.GetMaskIfo(property.Attributes.Mask(), defaultMask, defaultNotUseMaskAsDisplayFormat, regExMaskType, string.IsNullOrEmpty(property.Attributes.DataFormatString));
		}
		static bool IsNumericType(Type type) {
			return EditorsSource.NumericFloatTypes.Contains(type) || EditorsSource.NumericIntegerTypes.Contains(type);
		}
	}
	public abstract class EditorsGeneratorBase {
		protected enum EditorsGeneratorMode { Edit, EditSettings }
		public enum EditorsGeneratorTarget { GridControl, LayoutControl, PropertyGrid, Unknown }
		public struct Initializer {
			public static Initializer Default = new Initializer();
			readonly Action<IModelItem, IModelItem> SetEditProps;
			readonly Action<IModelItem> SetContainerProps;
			public Initializer(Action<IModelItem, IModelItem> setEditProps = null, Action<IModelItem> setContainerProps = null) {
				SetEditProps = setEditProps;
				SetContainerProps = setContainerProps;
			}
			public void SetEditProperties(IModelItem container, IModelItem edit) {
				SetEditProps.Do(x => x(container, edit));
			}
			public void SetContainerProperties(IModelItem container) {
				SetContainerProps.Do(x => x(container));
			}
			public static Initializer operator +(Initializer a, Initializer b) {
				Action<IModelItem, IModelItem> setEditProps = null;
				Action<IModelItem> setContainerProps = null;
				if(a.SetEditProps != null || b.SetEditProps != null)
					setEditProps = (container, edit) => { a.SetEditProperties(container, edit); b.SetEditProperties(container, edit); };
				if(a.SetContainerProps != null || b.SetContainerProps != null)
					setContainerProps = (container) => { a.SetContainerProperties(container); b.SetContainerProperties(container); };
				return new Initializer(setEditProps, setContainerProps);
			}
		}
		protected static bool GetAllowNullInput(IEdmPropertyInfo property) {
			bool nullable = property.HasNullableType();
			return property.HasNullableType() && !property.Attributes.Required();
		}
		protected static Type GetEditValueType(IEdmPropertyInfo property) {
			return property.HasNullableType() ? property.GetUnderlyingClrType() : null;
		}
		protected static MaskInfo GetDateTimeMask(IEdmPropertyInfo property) {
			PropertyDataType dataType = property.Attributes.PropertyDataType();
			string defaultMask = property.GetActualMask(GetDateTimeMask(dataType));
			return MaskInfo.GetMaskIfo(property.Attributes.Mask(), defaultMask, defaultMask == MaskInfo.DefaultDateTimeMaskValue, null, string.IsNullOrEmpty(property.Attributes.DataFormatString));
		}
		static string GetDateTimeMask(PropertyDataType dataType) {
			switch(dataType) {
				case PropertyDataType.DateTime:
					return "g";
				case PropertyDataType.Time:
					return "t";
				default:
					return "d";
			}
		}
		protected static int GetMaxLength(IEdmPropertyInfo property) {
			return property.Attributes.MaxLength();
		}
		static bool IsEdit(IModelItem edit) {
			return edit.ItemType.IsSubclassOf(typeof(BaseEdit));
		}
		static bool IsEditSettings(IModelItem edit) {
			return edit.ItemType.IsSubclassOf(typeof(BaseEditSettings));
		}
		static IEnumerable<IEdmPropertyInfo> GetFilteredProperties(IEnumerable<IEdmPropertyInfo> properties, bool scaffolding) {
			Func<IEdmPropertyInfo, bool> generateField = x => (!scaffolding || x.Attributes.AllowScaffolding) && !x.IsForeignKey
			   && (x.Attributes.AutoGenerateField == null || x.Attributes.AutoGenerateField.Value);
			return properties.Where(generateField);
		}
		public static IEnumerable<IEdmPropertyInfo> GetFilteredAndSortedProperties(IEnumerable<IEdmPropertyInfo> properties, bool scaffolding, bool movePropertiesWithNegativeOrderToEnd, LayoutType layoutType) {
			Func<IEdmPropertyInfo, bool> isVisible = x => x.Attributes.Order == null || x.Attributes.Order.Value >= 0;
			properties = GetFilteredProperties(properties, scaffolding);
			IEnumerable<IEdmPropertyInfo> visibleProperties = movePropertiesWithNegativeOrderToEnd ? properties.Where(x => isVisible(x)) : properties;
			IEnumerable<IEdmPropertyInfo> invisibleProperties = movePropertiesWithNegativeOrderToEnd ? properties.Where(x => !isVisible(x)) : new IEdmPropertyInfo[0];
			return visibleProperties.OrderBy(x => {
				int? order = x.Attributes.GetOrder(layoutType);
				return order != null ? order.Value : LayoutGroupInfoConstants.LastPropertiesStartIndex;
			}).Concat(invisibleProperties);
		}
		public virtual IEnumerable<IEdmPropertyInfo> FilterProperties(IEnumerable<IEdmPropertyInfo> properties) { return properties; } 
		public abstract EditorsGeneratorTarget Target { get; }
		protected abstract EditorsGeneratorMode Mode { get; }
		protected abstract void GenerateEditor(IEdmPropertyInfo property, Type editType, Initializer initializer);
		protected abstract Type GetLookUpEditType();
		internal Lazy<Initializer> Text(bool callGenerateMethod, IEdmPropertyInfo property, bool multiline) {
			if(callGenerateMethod) Text(property, multiline);
			return new Lazy<Initializer>(() => TextInitializer(property, multiline));
		}
		internal Lazy<Initializer> Check(bool callGenerateMethod, IEdmPropertyInfo property) {
			if(callGenerateMethod) Check(property);
			return new Lazy<Initializer>(() => CheckInitializer(property));
		}
		internal Lazy<Initializer> Password(bool callGenerateMethod, IEdmPropertyInfo property) {
			if(callGenerateMethod) Password(property);
			return new Lazy<Initializer>(() => PasswordInitializer(property));
		}
		internal Lazy<Initializer> Char(bool callGenerateMethod, IEdmPropertyInfo property) {
			if(callGenerateMethod) Char(property);
			return new Lazy<Initializer>(() => CharInitializer(property));
		}
		internal Lazy<Initializer> Object(bool callGenerateMethod, IEdmPropertyInfo property) {
			if(callGenerateMethod) Object(property);
			return new Lazy<Initializer>(() => ObjectInitializer(property));
		}
		internal Lazy<Initializer> Numeric(bool callGenerateMethod, IEdmPropertyInfo property, MaskInfo mask) {
			if(callGenerateMethod) Numeric(property, mask);
			return new Lazy<Initializer>(() => NumericInitializer(property, mask));
		}
		internal Lazy<Initializer> Range(bool callGenerateMethod, IEdmPropertyInfo property, MaskInfo mask, object minimum, object maximum) {
			if(callGenerateMethod) Range(property, mask, minimum, maximum);
			return new Lazy<Initializer>(() => RangeInitializer(property, mask, minimum, maximum));
		}
		internal Lazy<Initializer> RegExMaskText(bool callGenerateMethod, IEdmPropertyInfo property, MaskInfo mask) {
			if(callGenerateMethod) RegExMaskText(property, mask);
			return new Lazy<Initializer>(() => RegExMaskTextInitializer(property, mask));
		}
		internal Lazy<Initializer> DateTime(bool callGenerateMethod, IEdmPropertyInfo property) {
			if(callGenerateMethod) DateTime(property);
			return new Lazy<Initializer>(() => DateTimeInitializer(property));
		}
		internal Lazy<Initializer> Image(bool callGenerateMethod, IEdmPropertyInfo property, bool readOnly) {
			if(callGenerateMethod) Image(property, readOnly);
			return new Lazy<Initializer>(() => ImageInitializer(property, readOnly));
		}
		internal Lazy<Initializer> Enum(bool callGenerateMethod, IEdmPropertyInfo property, Type enumType) {
			if(callGenerateMethod) Enum(property, enumType);
			return new Lazy<Initializer>(() => EnumInitializer(property, enumType));
		}
		internal Lazy<Initializer> LookUp(bool callGenerateMethod, IEdmPropertyInfo property, string itemsSource, string displayMember, ForeignKeyInfo foreignKeyInfo) {
			if(callGenerateMethod) LookUp(property, itemsSource, displayMember, foreignKeyInfo);
			return new Lazy<Initializer>(() => LookUpInitializer(property, itemsSource, displayMember, foreignKeyInfo));
		}
		public virtual void Text(IEdmPropertyInfo property, bool multiline) {
			int maxLength = GetMaxLength(property);
			Type t = Mode == EditorsGeneratorMode.Edit ? typeof(TextEdit) : (maxLength > 0 ? typeof(TextEditSettings) : null);
			if(multiline) t = Mode == EditorsGeneratorMode.Edit ? typeof(TextEdit) : typeof(MemoEditSettings);
			GenerateEditor(property, t, TextInitializer(property, multiline, maxLength));
		}
		public virtual void Check(IEdmPropertyInfo property) {
			GenerateEditor(property, Mode == EditorsGeneratorMode.Edit ? typeof(CheckEdit) : typeof(CheckEditSettings), CheckInitializer(property));
		}
		public virtual void Password(IEdmPropertyInfo property) {
			GenerateEditor(property, Mode == EditorsGeneratorMode.Edit ? typeof(PasswordBoxEdit) : typeof(PasswordBoxEditSettings), PasswordInitializer(property));
		}
		public virtual void Char(IEdmPropertyInfo property) {
			GenerateEditor(property, Mode == EditorsGeneratorMode.Edit ? typeof(TextEdit) : typeof(TextEditSettings), CharInitializer(property));
		}
		public virtual void Object(IEdmPropertyInfo property) {
			if(Mode == EditorsGeneratorMode.Edit) GenerateEditor(property, typeof(TextEdit), ObjectInitializer(property));
			else GenerateEditor(property, null, ObjectInitializer(property));
		}
		public virtual void Numeric(IEdmPropertyInfo property, MaskInfo mask) {
			GenerateEditor(property, Mode == EditorsGeneratorMode.Edit ? typeof(TextEdit) : typeof(TextEditSettings), NumericInitializer(property, mask));
		}
		public virtual void Range(IEdmPropertyInfo property, MaskInfo mask, object minimum, object maximum) {
			GenerateEditor(property, Mode == EditorsGeneratorMode.Edit ? typeof(SpinEdit) : typeof(SpinEditSettings),
				RangeInitializer(property, mask, minimum, maximum));
		} 
		public virtual void RegExMaskText(IEdmPropertyInfo property, MaskInfo mask) {
			GenerateEditor(property, Mode == EditorsGeneratorMode.Edit ? typeof(TextEdit) : typeof(TextEditSettings),
				RegExMaskTextInitializer(property, mask));
		}
		public virtual void DateTime(IEdmPropertyInfo property) {
			GenerateEditor(property, Mode == EditorsGeneratorMode.Edit ? typeof(DateEdit) : typeof(DateEditSettings), DateTimeInitializer(property));
		}
		public virtual void Image(IEdmPropertyInfo property, bool readOnly) {
			Type editType = Mode == EditorsGeneratorMode.Edit ? typeof(PopupImageEdit) : typeof(PopupImageEditSettings);
			if(Target == EditorsGeneratorTarget.LayoutControl) editType = Mode == EditorsGeneratorMode.Edit ? typeof(ImageEdit) : typeof(ImageEditSettings);
			GenerateEditor(property, editType, ImageInitializer(property, readOnly));
		}
		public virtual void Enum(IEdmPropertyInfo property, Type enumType) {
			GenerateEditor(property, Mode == EditorsGeneratorMode.Edit ? typeof(ComboBoxEdit) : typeof(ComboBoxEditSettings), EnumInitializer(property, enumType));
		}
		public virtual void LookUp(IEdmPropertyInfo property, string itemsSource, string displayMember, ForeignKeyInfo foreignKeyInfo) {
			Type editType = null;
			if(string.IsNullOrEmpty(itemsSource)) editType = Mode == EditorsGeneratorMode.Edit ? typeof(TextEdit) : null;
			else editType = GetLookUpEditType();
			GenerateEditor(property, editType, LookUpInitializer(property, itemsSource, displayMember, foreignKeyInfo));
		}
		protected internal virtual Initializer TextInitializer(IEdmPropertyInfo property, bool multiline, int? maxLength = null) {
			int _maxLength = maxLength ?? GetMaxLength(property);
			return new Initializer((container, edit) => {
				if(!multiline) {
					EditorInitializeHelper.InitializeMaxLength(edit, _maxLength);
					return;
				}
				if(IsEdit(edit)) {
					edit.SetValueIfNotSet(TextEditBase.TextWrappingProperty, TextWrapping.Wrap, false);
					edit.SetValueIfNotSet(TextEditBase.AcceptsReturnProperty, true, false);
					edit.SetValue(TextEditBase.VerticalContentAlignmentProperty, VerticalAlignment.Top, false);
					edit.SetValueIfNotSet(TextEditBase.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Auto, false);
					EditorInitializeHelper.InitializeMaxLength(edit, _maxLength);
				} else if(IsEditSettings(edit)) {
					EditorInitializeHelper.InitializeMaxLength(edit, _maxLength);
				}
			}, (container) => {
				if(multiline && Mode == EditorsGeneratorMode.Edit) {
					container.Properties["MinHeight"].SetValueIfNotSet(EditorsSource.MultilineTextMinHeight);
					container.Properties["VerticalAlignment"].SetValue(VerticalAlignment.Stretch);
				}
			});
		}
		protected internal virtual Initializer CheckInitializer(IEdmPropertyInfo property, bool? allowNullInput = null) {
			bool _allowNullInput = allowNullInput ?? GetAllowNullInput(property);
			return new Initializer((container, edit) => { edit.SetValueIfNotSet(CheckEdit.IsThreeStateProperty, _allowNullInput, false); });
		}
		protected internal virtual Initializer PasswordInitializer(IEdmPropertyInfo property) {
			return new Initializer((container, edit) => { EditorInitializeHelper.InitializeMaxLength(edit, GetMaxLength(property)); });
		}
		protected internal virtual Initializer CharInitializer(IEdmPropertyInfo property) {
			return new Initializer((container, edit) => {
				EditorInitializeHelper.InitializeCharEdit(edit);
				edit.SetValueIfNotSet(BaseEdit.AllowNullInputProperty, GetAllowNullInput(property), false);
			});
		}
		protected internal virtual Initializer ObjectInitializer(IEdmPropertyInfo property) {
			return Initializer.Default;
		}
		protected internal virtual Initializer NumericInitializer(IEdmPropertyInfo property, MaskInfo mask, Type editValueType = null, bool? nullable = null, int? maxLength = null) {
			Type _editValueType = editValueType ?? GetEditValueType(property);
			bool _nullable = nullable ?? GetAllowNullInput(property);
			int _maxLength = maxLength ?? GetMaxLength(property);
			return new Initializer((container, edit) => {
				if(IsEdit(edit)) {
					EditorInitializeHelper.InitializeMask(edit, MaskType.Numeric, mask, _maxLength);
					edit.SetValue(TextEdit.HorizontalContentAlignmentProperty, EditorsSource.CurrencyValueAlignment, false);
					edit.SetValueIfNotSet(BaseEdit.EditValueTypeProperty, _editValueType, false);
					edit.SetValueIfNotSet(BaseEdit.AllowNullInputProperty, _nullable, false);
				} else if(IsEditSettings(edit)) {
					EditorInitializeHelper.InitializeMask(edit, MaskType.Numeric, mask, _maxLength);
					edit.SetValueIfNotSet(BaseEditSettings.AllowNullInputProperty, _nullable, false);
				}
			});
		}
		protected internal virtual Initializer RangeInitializer(IEdmPropertyInfo property, MaskInfo mask, object minimum, object maximum, Type editValueType = null) {
			return NumericInitializer(property, mask, editValueType ?? GetEditValueType(property), false);
		}
		protected internal virtual Initializer RegExMaskTextInitializer(IEdmPropertyInfo property, MaskInfo mask) {
			return new Initializer((container, edit) => { EditorInitializeHelper.InitializeRegExMask(edit, mask, GetMaxLength(property)); });
		}
		protected internal virtual Initializer DateTimeInitializer(IEdmPropertyInfo property, MaskInfo mask = null, bool? nullable = null) {
			MaskInfo _mask = mask ?? GetDateTimeMask(property);
			bool _nullable = nullable ?? GetAllowNullInput(property);
			return new Initializer((container, edit) => {
				edit.SetValueIfNotSet(BaseEdit.AllowNullInputProperty, _nullable, false);
				EditorInitializeHelper.InitializeDateTimeMask(edit, _mask);
			});
		}
		protected internal virtual Initializer ImageInitializer(IEdmPropertyInfo property, bool readOnly) {
			return new Initializer(null, (container) => {
				if(readOnly) {
					container.SetValueIfNotSet("IsReadOnly", true, false);
					container.SetValueIfNotSet("ReadOnly", true, false);
				}
			});
		}
		protected internal virtual Initializer EnumInitializer(IEdmPropertyInfo property, Type enumType, bool? useFlags = null, bool? nullable = null) {
			bool _nullable = nullable ?? GetAllowNullInput(property);
			return new Initializer((container, edit) => {
				if(_nullable) edit.SetValueIfNotSet(BaseEdit.AllowNullInputProperty, false, false);
				EditorInitializeHelper.InitializeEnumItemsSource(property, edit, enumType);
			});
		}
		protected internal virtual Initializer LookUpInitializer(IEdmPropertyInfo property, string itemsSource, string displayMember, ForeignKeyInfo foreignKeyInfo) {
			return new Initializer((container, edit) => {
				if(string.IsNullOrEmpty(itemsSource)) return;
				edit.SetValueIfNotSet(LookUpEditSettingsBase.ItemsSourceProperty, new Binding(itemsSource), false);
				edit.SetValueIfNotSet(LookUpEditSettingsBase.DisplayMemberProperty, displayMember, false);
				edit.SetValueIfNotSet(LookUpEditSettingsBase.ValueMemberProperty, foreignKeyInfo.PrimaryKeyPropertyName, false);
			}, (container) => {
				if(string.IsNullOrEmpty(itemsSource)) {
					container.SetValueIfNotSet("IsReadOnly", true, false);
					container.SetValueIfNotSet("ReadOnly", true, false);
					return;
				}
			});
		}
		public abstract void GenerateEditorFromResources(IEdmPropertyInfo property, object resourceKey, Initializer initializer);
		protected static object GetResourceContent<TColumn, TEditSettings, TEditor>(DataTemplate template) {
			object content = template.LoadContent();
			if(content is TColumn) return content;
			if(content is TEditSettings) return content;
			if(content is TEditor) return content;
			ContentControl cc = content as ContentControl;
			if(cc == null) return content;
			if(cc.Content is TColumn || cc.Content is TEditSettings) {
				content = cc.Content;
				cc.Content = null;
			}
			return content;
		}
		protected static DataTemplate GetResourceTemplate(FrameworkElement targetObject, object resourceKey) {
			return (DataTemplate)targetObject.FindResource(resourceKey);
		}
	}
	public abstract class EditorsGeneratorFilteringBase : EditorsGeneratorBase {
		public abstract void FilterRange(IEdmPropertyInfo property, FilterRangeEditorSettings settings, Initializer initializer);
		public abstract void FilterRangeProperty(IEdmPropertyInfo property, FilterRangeEditorSettings settings, Initializer initializer); 
		public abstract void FilterLookup(IEdmPropertyInfo property, FilterLookupEditorSettings settings, Initializer initializer);
		public abstract void FilterLookupProperty(IEdmPropertyInfo property, FilterLookupEditorSettings settings, Initializer initializer);
		public abstract void FilterBooleanChoice(IEdmPropertyInfo property, FilterBooleanEditorSettings settings, Initializer initializer);
		public abstract void FilterBooleanChoiceProperty(IEdmPropertyInfo property, FilterBooleanEditorSettings settings, Initializer initializer);
		public abstract void FilterEnumChoice(IEdmPropertyInfo property, FilterEnumEditorSettings settings, Initializer initializer);
		public abstract void FilterEnumChoiceProperty(IEdmPropertyInfo property, FilterEnumEditorSettings settings, Initializer initializer);
	}
}
