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
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Mvvm.UI.ViewGenerator.Metadata;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Validation;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
namespace DevExpress.Xpf.LayoutControl {
	[DXToolboxBrowsable(DXToolboxItemKind.Free)]
	public class DataLayoutItem : LayoutItem {
		#region static
		public static HorizontalAlignment CurrencyValueAlignment { get { return EditorsSource.CurrencyValueAlignment; } set { EditorsSource.CurrencyValueAlignment = value; } }
		public static double MultilineTextMinHeight { get { return EditorsSource.MultilineTextMinHeight; } set { EditorsSource.MultilineTextMinHeight = value; } }
		public static string PhoneNumberMask { get { return EditorsSource.PhoneNumberMask; } set { EditorsSource.PhoneNumberMask = value; } }
		internal static bool IsPropertyHasDefaultValue(DependencyObject dObj, DependencyProperty property) {
			return System.Windows.DependencyPropertyHelper.GetValueSource(dObj, property).BaseValueSource < BaseValueSource.Inherited;
		}
		static bool IsPropertyHasDefaultOrInheritedValue(DependencyObject dObj, DependencyProperty property) {
			return System.Windows.DependencyPropertyHelper.GetValueSource(dObj, property).BaseValueSource <= BaseValueSource.Inherited;
		}
		#endregion
		#region Properties
		public static readonly DependencyProperty UnderlyingObjectProperty = DependencyProperty.RegisterAttached("UnderlyingObject", typeof(object), typeof(DataLayoutItem), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
		public static readonly DependencyProperty AttributeSettingsProperty = DependencyProperty.RegisterAttached("AttributeSettings", typeof(DataLayoutItemAttributeSettings), typeof(DataLayoutItem), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
		public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(DataLayoutItem),
			new PropertyMetadata((o, e) => ((DataLayoutItem)o).OnIsReadOnlyChanged()));
		public static readonly DependencyProperty ApplySettingsForCustomContentProperty = DependencyProperty.Register("ApplySettingsForCustomContent", typeof(bool), typeof(DataLayoutItem), new PropertyMetadata(false));
		public static object GetUnderlyingObject(DependencyObject obj) { return (object)obj.GetValue(UnderlyingObjectProperty); }
		internal static void SetUnderlyingObject(DependencyObject obj, object value) { obj.SetValue(UnderlyingObjectProperty, value); }
		public static DataLayoutItemAttributeSettings GetAttributeSettings(DependencyObject obj) { return (DataLayoutItemAttributeSettings)obj.GetValue(AttributeSettingsProperty) ?? DataLayoutItemAttributeSettings.Empty; }
		internal static void SetAttributeSettings(DependencyObject obj, DataLayoutItemAttributeSettings value) { obj.SetValue(AttributeSettingsProperty, value); }
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("DataLayoutItemIsReadOnly")]
#endif
		public bool IsReadOnly { get { return (bool)GetValue(IsReadOnlyProperty); } set { SetValue(IsReadOnlyProperty, value); } }
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("DataLayoutItemIsActuallyReadOnly")]
#endif
		public bool IsActuallyReadOnly { get { return GetIsActuallyReadOnly(); } }
		public bool ApplySettingsForCustomContent { get { return (bool)GetValue(ApplySettingsForCustomContentProperty); } set { SetValue(ApplySettingsForCustomContentProperty, value); } }
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("DataLayoutItemBinding")]
#endif
		public Binding Binding {
			get { return _Binding; }
			set {
				if(Binding == value)
					return;
				if(value != null) {
					if(!string.IsNullOrEmpty(value.ElementName))
						throw new NotSupportedException("Binding.ElementName is not supported.");
					if(value.RelativeSource != null)
						throw new NotSupportedException("Binding.RelativeSource is not supported.");
				}
				_Binding = value;
				OnBindingChanged();
			}
		}
		protected Type BindingSourceType { get { return GetBindingSourceType(DataContext); } }
		protected internal DataLayoutControl DataLayoutControl {
			get { return _DataLayoutControl; }
			set {
				if(DataLayoutControl == value)
					return;
				_DataLayoutControl = value;
				OnDataLayoutControlChanged();
			}
		}
		private Binding _Binding;
		private DataLayoutControl _DataLayoutControl;
		private bool _IsActuallyReadOnly;
		WeakReference _DataLayoutControlReference;
		internal DataTemplate editorTemplate;
		#endregion Properties
		public DataLayoutItem() {
			UpdateIsActuallyReadOnly();
			UpdateIsActuallyRequired();
			ShowEmptyItemDesignTimeMessage();
			DataContextChanged += (sender, e) => OnDataContextChanged(e.OldValue, e.NewValue);
			PresentationSource.AddSourceChangedHandler(this, new SourceChangedEventHandler(OnPresentationSourceChanged));
		}
		private void OnPresentationSourceChanged(object sender, SourceChangedEventArgs e) {
			if(e.NewSource != null) {
				ClearDataLayoutControlReference();
				EnsureDataLayoutControlReference();
			}
		}
		protected override void OnVisualParentChanged(DependencyObject oldParent) {
			base.OnVisualParentChanged(oldParent);
			if(VisualParent != null)
				EnsureDataLayoutControlReference();
			else
				ClearDataLayoutControlReference();
		}
		BindingExpressionBase dataContextBindingExpression;
		BindingExpressionBase addColonToItemLabelsBindingExpression;
		void EnsureDataLayoutControlReference() {
			if(GetDataLayoutControl() != null) return;
			var dataLayoutControl = this.GetLayoutControl() as DataLayoutControl;
			if(dataLayoutControl != null) {
				_DataLayoutControlReference = new WeakReference(dataLayoutControl);
				if(IsPropertyHasDefaultOrInheritedValue(this, DataContextProperty)) {
					dataContextBindingExpression = SetBinding(DataContextProperty,
						new Binding("CurrentItem") { Source = dataLayoutControl, Mode = BindingMode.OneWay });
				}
				if(IsPropertyHasDefaultValue(this, AddColonToLabelProperty)) {
					addColonToItemLabelsBindingExpression = SetBinding(AddColonToLabelProperty,
						new Binding("AddColonToItemLabels") { Source = dataLayoutControl, Mode = BindingMode.OneWay });
				}
			}
		}
		void ClearDataLayoutControlReference() {
			if(_DataLayoutControlReference == null) return;
			_DataLayoutControlReference = null;
			if(dataContextBindingExpression == ReadLocalValue(DataContextProperty)) ClearValue(DataContextProperty);
			if(addColonToItemLabelsBindingExpression == ReadLocalValue(AddColonToLabelProperty)) ClearValue(AddColonToLabelProperty);
		}
		internal DataLayoutControl GetDataLayoutControl() {
			return DataLayoutControl ?? _DataLayoutControlReference.With(x => x.Target) as DataLayoutControl;
		}
		protected virtual bool GetIsActuallyReadOnly() {
			if(PropertyInfo == null || !PropertyInfo.PropertyType.IsEditable())
				return true;
			if(GetDataLayoutControl().Return(x => x.IsReadOnly, () => false))
				return true;
			if(IsReadOnly)
				return true;
			ReadOnlyAttribute classReadOnlyAttribute = BindingSourceType != null ? BindingSourceType.GetAttribute<ReadOnlyAttribute>() : null;
			return GetAttributeSettings(this).IsReadOnly ||
				classReadOnlyAttribute != null && classReadOnlyAttribute.IsReadOnly;
		}
		protected void CheckIsActuallyReadOnlyChanged() {
			if (IsActuallyReadOnly == _IsActuallyReadOnly)
				return;
			_IsActuallyReadOnly = IsActuallyReadOnly;
			OnIsActuallyReadOnlyChanged();
		}
		protected virtual void OnIsActuallyReadOnlyChanged() {
			UpdateIsActuallyRequired();
			if(Content == null)
				return;
			UpdateContentValueBinding((FrameworkElement)Content);
			UpdateContentIsReadOnly((FrameworkElement)Content);
		}
		protected virtual void OnIsReadOnlyChanged() {
			CheckIsActuallyReadOnlyChanged();
		}
		protected void UpdateIsActuallyReadOnly() {
			_IsActuallyReadOnly = IsActuallyReadOnly;
		}
		protected virtual PropertyDescriptor GetBindingProperty() {
			return !string.IsNullOrEmpty(GetBindingPropertyPath()) ? TypeDescriptor.GetProperties(GetBindingSource(DataContext))[GetBindingPropertyPath()] : null;
		}
		protected string GetBindingPropertyPath() {
			return Binding.Path != null ? Binding.Path.Path : null;
		}
		protected object GetBindingSource(object dataContext) {
			return (Binding != null ? Binding.Source : null) ?? dataContext;
		}
		protected Type GetBindingSourceType(object dataContext) {
			object bindingSource = GetBindingSource(dataContext);
			return bindingSource != null ? bindingSource.GetType() : null;
		}
		protected void UpdateUI() {
			if(Binding == null || BindingSourceType == null)
				ClearUI();
			else
				GenerateUI();
		}
		protected void GenerateUI() {
			if(Binding == null || BindingSourceType == null)
				return;
			var property = GetBindingProperty();
			PropertyInfo = property != null ?
				(IEdmPropertyInfo)new EdmPropertyInfo(property, DataColumnAttributesProvider.GetAttributes(property, BindingSourceType), false) :
				new EmptyEdmPropertyInfo(BindingSourceType);
			SetAttributeSettings(this, DataLayoutItemAttributeSettings.Create(PropertyInfo));
			UpdateIsActuallyReadOnly();
			UpdateIsActuallyRequired();
			if(IsPropertyHasDefaultValue(this, LabelProperty))
				SetCurrentValue(LabelProperty, GetLabel());
			if(IsPropertyHasDefaultValue(this, ToolTipProperty))
				SetCurrentValue(ToolTipProperty, GetToolTip());
			if(IsPropertyHasDefaultValue(this, ContentProperty)) {
				FrameworkElement content = CreateContentAndInitializeUI();
				UpdateContentValueBinding(content);
				UpdateContentIsReadOnly(content);
				SetCurrentValue(ContentProperty, content);
			} else {
				UpdateUnderlyingObject(Content as FrameworkElement);
				DoApplySettingsForCustomContent();
			}
			if(property != null) {
				PropertyValidator = PropertyValidator.FromAttributes(property.Attributes, property.Name);
				BaseEdit editor = Content as BaseEdit;
				if(PropertyValidator != null && editor != null) {
					editor.Validate -= OnContentValidated;
					editor.Validate += OnContentValidated;
				}
			}
		}
		protected void ClearUI() {
			PropertyInfo = null;
			UpdateIsActuallyReadOnly();
			UpdateIsActuallyRequired();
			using(labelLocker.Lock()) {
				if(IsPropertyHasDefaultValue(this, LabelProperty))
					ClearValue(LabelProperty);
			}
			if(IsPropertyHasDefaultValue(this, ToolTipProperty))
				ClearValue(ToolTipProperty);
			using(contentLocker.Lock()) {
				if(IsPropertyHasDefaultValue(this, ContentProperty)) {
					BaseEdit editor = Content as BaseEdit;
					if(PropertyValidator != null && editor != null) editor.Validate -= OnContentValidated;
					ClearValue(ContentProperty);
				}
				FinalizeUI();
			}
			ShowEmptyItemDesignTimeMessage();
		}
		protected virtual void FinalizeUI() {
			ClearValue(MinHeightProperty);
			ClearValue(VerticalAlignmentProperty);
		}
		protected virtual FrameworkElement CreateContentAndInitializeUI() {
			DataLayoutItemGenerator generator = new DataLayoutItemGenerator(this, true);
			EditorsSource.GenerateEditor(PropertyInfo, generator, null);
			return generator.Content;
		}
		void DoApplySettingsForCustomContent() {
			if(!ApplySettingsForCustomContent) return;
			DataLayoutItemGenerator generator = new DataLayoutItemGenerator(this, false);
			EditorsSource.GenerateEditor(PropertyInfo, generator, null);
		}
		PropertyValidator PropertyValidator;
		void OnContentValidated(object sender, ValidationEventArgs e) {
			BaseEdit editor = sender as BaseEdit;
			if(editor == null) return;
			BaseEditHelper.SetValidationError(editor, null);
			string validationErrorText = PropertyValidator.GetErrorText(e.Value, GetDataLayoutControl().Return(x => x.CurrentItem, () => DataContext));
			if(string.IsNullOrEmpty(validationErrorText)) return;
			BaseEditHelper.SetValidationError(editor, new BaseValidationError(validationErrorText));
		}
		protected virtual DependencyProperty GetContentIsReadOnlyProperty(FrameworkElement content) {
			return content is BaseEdit ? BaseEdit.IsReadOnlyProperty : null;
		}
		protected override bool GetIsActuallyRequired() {
			if (IsActuallyReadOnly)
				return false;
			if (PropertyInfo != null && GetAttributeSettings(this).IsRequired)
				return true;
			return base.GetIsActuallyRequired();
		}
		protected virtual object GetLabel() {
			return GetAttributeSettings(this).Label;
		}
		protected virtual object GetToolTip() {
			return GetAttributeSettings(this).ToolTip;
		}
		protected internal override FrameworkElement GetSelectableElement(Point p) {
			return this;
		}
		protected virtual void OnBindingChanged() {
			UpdateUI();
		}
		protected virtual void OnDataContextChanged(object oldValue, object newValue) {
			if (GetBindingSourceType(newValue) != GetBindingSourceType(oldValue))
				UpdateUI();
		}
		protected virtual void OnDataLayoutControlChanged() {
			CheckIsActuallyReadOnlyChanged();
		}
		protected internal virtual void OnDataLayoutControlIsReadOnlyChanged() {
			CheckIsActuallyReadOnlyChanged();
		}
		protected void UpdateContentIsReadOnly(FrameworkElement content) {
			if (GetContentIsReadOnlyProperty(content) != null)
				content.SetValue(GetContentIsReadOnlyProperty(content), IsActuallyReadOnly);
		}
		protected virtual void UpdateContentValueBinding(FrameworkElement content) {
			UpdateUnderlyingObject(content);
			var prop = GetContentValueProperty(content);
			if(prop != null) content.SetBinding(prop, CreateContentValueBinding(IsActuallyReadOnly));
		}
		void UpdateUnderlyingObject(FrameworkElement content) {
			if(content == null) return;
			content.SetBinding(DataLayoutItem.UnderlyingObjectProperty, CreateContentValueBinding(true));
		}
		protected virtual DependencyProperty GetContentValueProperty(FrameworkElement content) {
			return content is BaseEdit ? BaseEdit.EditValueProperty : null;
		}
		protected virtual Binding CreateContentValueBinding(bool isReadOnly) {
			var result = new Binding();
			if(Binding.Source != null)
				result.Source = Binding.Source;
			if(Binding.Path != null)
				result.Path = Binding.Path;
			result.Mode = isReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;
			result.Converter = Binding.Converter;
			result.ConverterCulture = Binding.ConverterCulture;
			result.ConverterParameter = Binding.ConverterParameter;
			result.FallbackValue = Binding.FallbackValue;
			result.StringFormat = Binding.StringFormat;
			result.TargetNullValue = Binding.TargetNullValue;
			result.UpdateSourceTrigger = Binding.UpdateSourceTrigger;
			result.ValidatesOnDataErrors = true;
			result.ValidatesOnExceptions = true;
			result.NotifyOnValidationError = true;
			result.NotifyOnSourceUpdated = true;
			return result;
		}
		protected IEdmPropertyInfo PropertyInfo { get; private set; }
		Locker labelLocker = new Locker();
		Locker contentLocker = new Locker();
		protected override void OnLabelChanged(object oldValue) {
			base.OnLabelChanged(oldValue);
			if(this.IsInDesignTool()) {
				if(labelLocker)
					return;
				using(labelLocker.Lock()) {
					if(Label == null)
						SetCurrentValue(LabelProperty, GetLabel());
				}
			}
		}
		protected override void OnContentChanged(UIElement oldValue, UIElement newValue) {
			base.OnContentChanged(oldValue, newValue);
			if(this.IsInDesignTool()) {
				if(contentLocker)
					return;
				using(contentLocker.Lock()) {
					if(Content == null) GenerateUI();
					if(Content == null) ShowEmptyItemDesignTimeMessage();
				}
			}
		}
		private void ShowEmptyItemDesignTimeMessage() {
			if(this.IsInDesignTool()) {
				TextBlock tb = new TextBlock() { Text = "No data source: set Binding" };
				SetCurrentValue(ContentProperty, tb);
			}
		}
	}
}
