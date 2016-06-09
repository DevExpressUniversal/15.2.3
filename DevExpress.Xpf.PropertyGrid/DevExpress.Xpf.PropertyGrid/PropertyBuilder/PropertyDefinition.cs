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
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.PropertyGrid.Internal;
using DevExpress.Xpf.Utils;
using System.Windows.Controls;
using DevExpress.Xpf.Bars;
using System.Collections.ObjectModel;
using System.Collections;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core;
using System.Windows.Markup;
using System.Windows.Input;
using DevExpress.Mvvm.Native;
using DevExpress.XtraVerticalGrid.Data;
using DevExpress.Xpf.Bars.Native;
namespace DevExpress.Xpf.PropertyGrid {
	public enum TypeMatchMode {
		Direct,
		Extended
	}
	public enum ApplyingMode {
		WhenGrouping = 0x1,
		WhenNoGrouping = 0x2,
		Always = WhenGrouping | WhenNoGrouping
	}
	public enum AllowExpandingMode {
		Default,
		Never,
		ForceIfNoTypeConverter,
		Force
	}
	public enum HeaderShowMode {
		Left,
		Top,
		Hidden,
		OnlyHeader
	}
	[Flags]
	public enum HeaderHighlightingMode {
		None = 0x0,
		OnlyHeader = 0x1,
		OnlyContent = 0x2,
		HeaderAndContent = OnlyHeader | OnlyContent
	}
	[ContentProperty("PropertyDefinitions")]
	public abstract class PropertyDefinitionBase : FrameworkElement {
		protected static readonly DependencyPropertyKey ParentDefinitionPropertyKey;
		protected static readonly DependencyPropertyKey BuilderPropertyKey;
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		protected internal static readonly DependencyProperty BuilderProperty;
		public static readonly DependencyProperty PathProperty;
		public static readonly DependencyProperty PropertyDefinitionsProperty;
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		protected internal static readonly DependencyProperty ParentDefinitionProperty;
		public static readonly DependencyProperty ApplyingModeProperty;
		public static readonly DependencyProperty ChildrenSortModeProperty;
		public static readonly DependencyProperty IsReadOnlyProperty;
		public static readonly DependencyProperty HeaderTemplateSelectorProperty;
		public static readonly DependencyProperty HeaderTemplateProperty;
		public static readonly DependencyProperty RowTemplateProperty;
		public static readonly DependencyProperty HeaderProperty;
		public static readonly DependencyProperty DescriptionProperty;
		public static readonly DependencyProperty DescriptionContainerStyleSelectorProperty;
		public static readonly DependencyProperty DescriptionContainerStyleProperty;
		public static readonly DependencyProperty DescriptionTemplateSelectorProperty;
		public static readonly DependencyProperty DescriptionTemplateProperty;
		public static readonly DependencyProperty InstanceInitializerProperty;
		public static readonly DependencyProperty HeaderShowModeProperty;
		public static readonly DependencyProperty ContentTemplateProperty;
		public static readonly DependencyProperty ContentTemplateSelectorProperty;
		public static readonly DependencyProperty ShowChildrenProperty;
		public static readonly DependencyProperty HighlightingModeProperty;
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		static readonly DependencyProperty PropertyDefinitionsAttachedBehaviorProperty;
		public static readonly DependencyProperty PropertyDefinitionsSourceProperty;
		public static readonly DependencyProperty PropertyDefinitionStyleProperty;
		public static readonly DependencyProperty PropertyDefinitionTemplateProperty;
		public static readonly DependencyProperty PropertyDefinitionTemplateSelectorProperty;		
		static PropertyDefinitionBase() {
			var ownerType = typeof(PropertyDefinitionBase);
			VisibilityProperty.OverrideMetadata(typeof(PropertyDefinitionBase), new FrameworkPropertyMetadata(Visibility.Visible, (d, e) => ((PropertyDefinitionBase)d).OnVisibilityChanged((Visibility)e.OldValue, (Visibility)e.NewValue), (d, v) => ((Visibility)v) == Visibility.Hidden ? Visibility.Collapsed : (Visibility)v));
			ParentDefinitionPropertyKey = DependencyPropertyManager.RegisterReadOnly("ParentDefinition", ownerType, ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((PropertyDefinitionBase)d).OnParentDefinitionChanged((PropertyDefinitionBase)e.OldValue, (PropertyDefinitionBase)e.NewValue)));
			BuilderPropertyKey = DependencyPropertyManager.RegisterReadOnly("Builder", typeof(PropertyBuilder), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((PropertyDefinitionBase)d).OnBuilderChanged((PropertyBuilder)e.OldValue)));
			BuilderProperty = BuilderPropertyKey.DependencyProperty;
			PathProperty = DependencyPropertyManager.Register("Path", typeof(string), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((PropertyDefinitionBase)d).OnPathChanged((string)e.OldValue), (d, v) => ((PropertyDefinitionBase)d).CoercePath((string)v)));
			HeaderTemplateSelectorProperty = DependencyPropertyManager.Register("HeaderTemplateSelector", typeof(DataTemplateSelector), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((PropertyDefinitionBase)d).OnHeaderTemplateSelectorChanged((DataTemplateSelector)e.OldValue, (DataTemplateSelector)e.NewValue)));
			HeaderTemplateProperty = DependencyPropertyManager.Register("HeaderTemplate", typeof(DataTemplate), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((PropertyDefinitionBase)d).OnHeaderTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue)));
			RowTemplateProperty = DependencyPropertyManager.Register("RowTemplate", typeof(ControlTemplate), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((PropertyDefinitionBase)d).OnRowTemplateChanged((ControlTemplate)e.OldValue, (ControlTemplate)e.NewValue)));
			PropertyDefinitionsProperty = DependencyPropertyManager.Register("PropertyDefinitions", typeof(PropertyDefinitionCollection), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((PropertyDefinitionBase)d).OnPropertyDefinitionsChanged((PropertyDefinitionCollection)e.OldValue, (PropertyDefinitionCollection)e.NewValue)));
			HeaderProperty = DependencyPropertyManager.Register("Header", typeof(string), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((PropertyDefinitionBase)d).OnHeaderChanged((string)e.OldValue)));
			InstanceInitializerProperty = DependencyPropertyManager.Register("InstanceInitializer", typeof(IInstanceInitializer), typeof(PropertyDefinitionBase), new FrameworkPropertyMetadata(null, (d, e) => ((PropertyDefinitionBase)d).RaiseChanged(PropertyBuilderChangeKind.VisualClientProperties)));
			IsReadOnlyProperty = DependencyPropertyManager.Register("IsReadOnly", typeof(bool?), typeof(PropertyDefinitionBase), new FrameworkPropertyMetadata(null, (d, e) => ((PropertyDefinitionBase)d).OnIsReadOnlyChanged((bool?)e.OldValue, (bool?)e.NewValue)));
			DescriptionProperty = DependencyPropertyManager.Register("Description", typeof(object), typeof(PropertyDefinitionBase), new FrameworkPropertyMetadata(null, (d, e) => ((PropertyDefinitionBase)d).OnDescriptionChanged((object)e.OldValue, (object)e.NewValue)));
			DescriptionContainerStyleSelectorProperty = DependencyPropertyManager.Register("DescriptionContainerStyleSelector", typeof(StyleSelector), typeof(PropertyDefinitionBase), new FrameworkPropertyMetadata(null, (d, e) => ((PropertyDefinitionBase)d).DescriptionContainerStyleSelectorChanged((StyleSelector)e.OldValue, (StyleSelector)e.NewValue)));
			DescriptionContainerStyleProperty = DependencyPropertyManager.Register("DescriptionContainerStyle", typeof(Style), typeof(PropertyDefinitionBase), new FrameworkPropertyMetadata(null, (d, e) => ((PropertyDefinitionBase)d).DescriptionContainerStyleChanged((Style)e.OldValue, (Style)e.NewValue)));
			DescriptionTemplateSelectorProperty = DependencyPropertyManager.Register("DescriptionTemplateSelector", typeof(DataTemplateSelector), typeof(PropertyDefinitionBase), new FrameworkPropertyMetadata(null, (d, e) => ((PropertyDefinitionBase)d).OnDescriptionTemplateSelectorChanged((DataTemplateSelector)e.OldValue, (DataTemplateSelector)e.NewValue)));
			DescriptionTemplateProperty = DependencyPropertyManager.Register("DescriptionTemplate", typeof(DataTemplate), typeof(PropertyDefinitionBase), new FrameworkPropertyMetadata(null, (d, e) => ((PropertyDefinitionBase)d).OnDescriptionTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue)));
			PropertyDefinitionsSourceProperty = DependencyPropertyManager.Register("PropertyDefinitionsSource", typeof(IEnumerable), typeof(PropertyDefinitionBase), new FrameworkPropertyMetadata(null, (d, e) => ((PropertyDefinitionBase)d).OnPropertyDefinitionsSourceChanged(e)));
			PropertyDefinitionStyleProperty = DependencyPropertyManager.Register("PropertyDefinitionStyle", typeof(Style), typeof(PropertyDefinitionBase), new FrameworkPropertyMetadata(null, (d, e) => ((PropertyDefinitionBase)d).OnPropertyDefinitionStyleChanged(e)));
			PropertyDefinitionTemplateProperty = DependencyPropertyManager.Register("PropertyDefinitionTemplate", typeof(DataTemplate), typeof(PropertyDefinitionBase), new FrameworkPropertyMetadata(null, (d, e) => ((PropertyDefinitionBase)d).OnPropertyDefinitionTemplateChanged(e)));
			PropertyDefinitionTemplateSelectorProperty = DependencyPropertyManager.Register("PropertyDefinitionTemplateSelector", typeof(DataTemplateSelector), typeof(PropertyDefinitionBase), new FrameworkPropertyMetadata(null, (d, e) => ((PropertyDefinitionBase)d).OnPropertyDefinitionTemplateSelectorChanged(e)));
			PropertyDefinitionsAttachedBehaviorProperty = DependencyPropertyManager.RegisterAttached("PropertyDefinitiosAttachedBehavior", typeof(ItemsAttachedBehaviorCore<PropertyDefinitionBase, PropertyDefinitionBase>), typeof(PropertyDefinitionBase), new PropertyMetadata(null));
			ChildrenSortModeProperty = DependencyPropertyManager.Register("ChildrenSortMode", typeof(PropertyGridSortMode), typeof(PropertyDefinitionBase), new FrameworkPropertyMetadata(PropertyGridSortMode.Unspecified, (d, e) => ((PropertyDefinitionBase)d).OnSortModeChanged((PropertyGridSortMode)e.OldValue, (PropertyGridSortMode)e.NewValue)));
			ApplyingModeProperty = DependencyPropertyManager.Register("ApplyingMode", typeof(ApplyingMode), typeof(PropertyDefinitionBase), new FrameworkPropertyMetadata(ApplyingMode.Always, (d, e) => ((PropertyDefinitionBase)d).OnApplyingModeChanged((ApplyingMode)e.OldValue, (ApplyingMode)e.NewValue)));
			HeaderShowModeProperty = DependencyPropertyManager.Register("HeaderShowMode", typeof(HeaderShowMode), typeof(PropertyDefinitionBase), new FrameworkPropertyMetadata(HeaderShowMode.Left));
			HighlightingModeProperty = DependencyPropertyManager.Register("HighlightingMode", typeof(HeaderHighlightingMode), typeof(PropertyDefinitionBase), new FrameworkPropertyMetadata(HeaderHighlightingMode.HeaderAndContent));
			ContentTemplateProperty = DependencyPropertyManager.Register("ContentTemplate", typeof(DataTemplate), typeof(PropertyDefinitionBase), new FrameworkPropertyMetadata(null, (d, e) => ((PropertyDefinitionBase)d).OnContentTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue)));
			ContentTemplateSelectorProperty = DependencyPropertyManager.Register("ContentTemplateSelector", typeof(DataTemplateSelector), typeof(PropertyDefinitionBase), new FrameworkPropertyMetadata(null, (d, e) => ((PropertyDefinitionBase)d).OnContentTemplateSelectorChanged((DataTemplateSelector)e.OldValue, (DataTemplateSelector)e.NewValue)));
			ShowChildrenProperty = DependencyPropertyManager.Register("ShowChildren", typeof(bool), typeof(PropertyDefinitionBase), new FrameworkPropertyMetadata(true, (d, e) => ((PropertyDefinitionBase)d).OnShowChildrenChanged((bool)e.OldValue)));
			ParentDefinitionProperty = ParentDefinitionPropertyKey.DependencyProperty;
		}
		internal int typeHashCode = 0;
		internal int pathHashCode = 0;
		internal int scopeHashCode = 0;
		internal int ownTypeHashCode = 0;
		internal bool isStandardDefinition = false;
		internal bool isResourceGeneratedDefinition = false;
		ControlTemplate rowTemplate = null;
		DataTemplate headerTemplate = null;
		DataTemplate contentTemplate = null;
		DataTemplateSelector headerTemplateSelector = null;
		DataTemplateSelector contentTemplateSelector = null;
		public DataTemplate HeaderTemplate {
			get { return headerTemplate; }
			set { SetValue(HeaderTemplateProperty, value); }
		}
		public DataTemplateSelector HeaderTemplateSelector {
			get { return headerTemplateSelector; }
			set { SetValue(HeaderTemplateSelectorProperty, value); }
		}
		public ControlTemplate RowTemplate {
			get { return rowTemplate; }
			set { SetValue(RowTemplateProperty, value); }
		}
		public DataTemplate ContentTemplate {
			get { return contentTemplate; }
			set { SetValue(ContentTemplateProperty, value); }
		}
		public DataTemplateSelector ContentTemplateSelector {
			get { return contentTemplateSelector; }
			set { SetValue(ContentTemplateSelectorProperty, value); }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PropertyDefinitionCollection PropertyDefinitions {
			get { return (PropertyDefinitionCollection)GetValue(PropertyDefinitionsProperty); }
		}
		public IEnumerable PropertyDefinitionsSource {
			get { return (IEnumerable)GetValue(PropertyDefinitionsSourceProperty); }
			set { SetValue(PropertyDefinitionsSourceProperty, value); }
		}
		public Style PropertyDefinitionStyle {
			get { return (Style)GetValue(PropertyDefinitionStyleProperty); }
			set { SetValue(PropertyDefinitionStyleProperty, value); }
		}
		public DataTemplate PropertyDefinitionTemplate {
			get { return (DataTemplate)GetValue(PropertyDefinitionTemplateProperty); }
			set { SetValue(PropertyDefinitionTemplateProperty, value); }
		}
		public DataTemplateSelector PropertyDefinitionTemplateSelector {
			get { return (DataTemplateSelector)GetValue(PropertyDefinitionTemplateSelectorProperty); }
			set { SetValue(PropertyDefinitionTemplateSelectorProperty, value); }
		}
		public string Header {
			get { return (string)GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}
		string path = null;
		string[] splittedPath = null;
		protected internal string[] SplittedPath {
			get { return splittedPath ?? (splittedPath = GetSplittedPath()); }
		}
		protected void InvalidateSplittedPath() { splittedPath = null; }
		protected virtual string[] GetSplittedPath() { return Path.WithString(FieldNameHelper.GetPath); }
		public string Path {
			get { return path; }
			set { SetValue(PathProperty, value); }
		}
		PropertyDefinitionBase parentDefinition;
		protected internal PropertyDefinitionBase ParentDefinition {
			get { return parentDefinition; }
			set { this.SetValue(ParentDefinitionPropertyKey, value); }
		}		
		protected internal PropertyBuilder Builder {
			get { return (PropertyBuilder)GetValue(BuilderProperty); }
			set { this.SetValue(BuilderPropertyKey, value); }
		}
		protected internal virtual bool IsCategory {
			get { return false; }
		}
		public ApplyingMode ApplyingMode {
			get { return (ApplyingMode)GetValue(ApplyingModeProperty); }
			set { SetValue(ApplyingModeProperty, value); }
		}
		public PropertyGridSortMode ChildrenSortMode {
			get { return (PropertyGridSortMode)GetValue(ChildrenSortModeProperty); }
			set { SetValue(ChildrenSortModeProperty, value); }
		}
		public bool ShowChildren {
			get { return (bool)GetValue(ShowChildrenProperty); }
			set { SetValue(ShowChildrenProperty, value); }
		}
		[TypeConverter(typeof(StringConverter))]
		public object Description {
			get { return (object)GetValue(DescriptionProperty); }
			set { SetValue(DescriptionProperty, value); }
		}
		public IInstanceInitializer InstanceInitializer {
			get { return (IInstanceInitializer)GetValue(InstanceInitializerProperty); }
			set { SetValue(InstanceInitializerProperty, value); }
		}
		public DataTemplate DescriptionTemplate {
			get { return (DataTemplate)GetValue(DescriptionTemplateProperty); }
			set { SetValue(DescriptionTemplateProperty, value); }
		}
		public DataTemplateSelector DescriptionTemplateSelector {
			get { return (DataTemplateSelector)GetValue(DescriptionTemplateSelectorProperty); }
			set { SetValue(DescriptionTemplateSelectorProperty, value); }
		}
		public Style DescriptionContainerStyle {
			get { return (Style)GetValue(DescriptionContainerStyleProperty); }
			set { SetValue(DescriptionContainerStyleProperty, value); }
		}
		public StyleSelector DescriptionContainerStyleSelector {
			get { return (StyleSelector)GetValue(DescriptionContainerStyleSelectorProperty); }
			set { SetValue(DescriptionContainerStyleSelectorProperty, value); }
		}
		public bool? IsReadOnly {
			get { return (bool?)GetValue(IsReadOnlyProperty); }
			set { SetValue(IsReadOnlyProperty, value); }
		}
		internal PropertyDefinitionBase() {
			this.SetValue(PropertyDefinitionsProperty, new PropertyDefinitionCollection(this));
			this.ownTypeHashCode = this.GetType().GetHashCode();
		}		
		protected virtual string CoercePath(string path) {
			return path;
		}
		public HeaderShowMode HeaderShowMode {
			get { return (HeaderShowMode)GetValue(HeaderShowModeProperty); }
			set { SetValue(HeaderShowModeProperty, value); }
		}
		public HeaderHighlightingMode HighlightingMode {
			get { return (HeaderHighlightingMode)GetValue(HighlightingModeProperty); }
			set { SetValue(HighlightingModeProperty, value); }
		}
		protected internal virtual void ExecuteMenuCustomizations(DependencyObject context) { }
		internal event PropertyChangedEventHandler NotifyPropertyChanged;
		internal void RaiseNotifyPropertyChanged(string propertyName) {
			if (NotifyPropertyChanged == null)
				return;
			NotifyPropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		DataTemplateSelector actualHeaderTemplateSelector;
		protected internal DataTemplateSelector ActualHeaderTemplateSelector {
			get { return actualHeaderTemplateSelector ?? (actualHeaderTemplateSelector = (HeaderTemplate == null && HeaderTemplateSelector == null) ? null : new PrioritizedDataTemplateSelector() { DefaultTemplate = HeaderTemplate, DefaultTemplateSelector = HeaderTemplateSelector }); }
		}
		[ThreadStatic]
		static DataTemplate defaultPresenterTemplate;
		[ThreadStatic]
		static DataTemplateSelector defaultContentTemplateSelector;
		static DataTemplate DefaultPresenterTemplate { get { return defaultPresenterTemplate ?? (defaultPresenterTemplate = new DataTemplate() { VisualTree = new FrameworkElementFactory(typeof(CellEditorPresenter)) }.Do(x => x.Seal())); } }
		static DataTemplateSelector DefaultContentTemplateSelector {
			get { return defaultContentTemplateSelector ?? (defaultContentTemplateSelector = new PrioritizedDataTemplateSelector() { DefaultTemplate = DefaultPresenterTemplate }); }
		}
		DataTemplateSelector actualContentTemplateSelector;
		protected internal DataTemplateSelector ActualContentTemplateSelector {
			get { return actualContentTemplateSelector ?? (actualContentTemplateSelector = (ContentTemplate == null && ContentTemplateSelector == null) ? DefaultContentTemplateSelector : new PrioritizedDataTemplateSelector() { DefaultTemplate = ContentTemplate, DefaultTemplateSelector = ContentTemplateSelector }); }
		}
		protected void UpdateActualHeaderTemplateSelector() {
			actualHeaderTemplateSelector = null;
			RaiseNotifyPropertyChanged("ActualHeaderTemplateSelector");
		}
		protected void UpdateActualContentTemplateSelector() {
			actualContentTemplateSelector = null;
			RaiseNotifyPropertyChanged("ActualContentTemplateSelector");
		}
		protected virtual void OnPropertyDefinitionTemplateSelectorChanged(DependencyPropertyChangedEventArgs args) {
			ItemsAttachedBehaviorCore<PropertyDefinitionBase, PropertyDefinitionBase>.OnItemsGeneratorTemplatePropertyChanged(this, args, PropertyDefinitionsAttachedBehaviorProperty);
		}
		protected virtual void OnPropertyDefinitionTemplateChanged(DependencyPropertyChangedEventArgs args) {
			ItemsAttachedBehaviorCore<PropertyDefinitionBase, PropertyDefinitionBase>.OnItemsGeneratorTemplatePropertyChanged(this, args, PropertyDefinitionsAttachedBehaviorProperty);
		}
		protected virtual void OnPropertyDefinitionStyleChanged(DependencyPropertyChangedEventArgs args) {
			ItemsAttachedBehaviorCore<PropertyDefinitionBase, PropertyDefinitionBase>.OnItemsGeneratorTemplatePropertyChanged(this, args, PropertyDefinitionsAttachedBehaviorProperty);
		}
		protected virtual void OnPropertyDefinitionsSourceChanged(DependencyPropertyChangedEventArgs args) {
			ItemsAttachedBehaviorCore<PropertyDefinitionBase, PropertyDefinitionBase>.OnItemsSourcePropertyChanged(this,
			args,
			PropertyDefinitionsAttachedBehaviorProperty,
			PropertyDefinitionTemplateProperty,
			PropertyDefinitionTemplateSelectorProperty,
			PropertyDefinitionStyleProperty,
			propertyGrid => propertyGrid.PropertyDefinitions,
			propertyGrid => new PropertyDefinition());
		}		
		protected virtual void OnShowChildrenChanged(bool oldValue) {
		}
		protected virtual void OnHeaderTemplateChanged(DataTemplate oldValue, DataTemplate newValue) { headerTemplate = newValue; UpdateActualHeaderTemplateSelector(); }
		protected virtual void OnHeaderTemplateSelectorChanged(DataTemplateSelector oldValue, DataTemplateSelector newValue) { headerTemplateSelector = newValue; UpdateActualHeaderTemplateSelector(); }
		protected virtual void OnContentTemplateSelectorChanged(DataTemplateSelector oldValue, DataTemplateSelector newValue) { contentTemplateSelector = newValue; UpdateActualContentTemplateSelector(); }
		protected virtual void OnContentTemplateChanged(DataTemplate oldValue, DataTemplate newValue) { contentTemplate = newValue; UpdateActualContentTemplateSelector(); }
		protected virtual void OnDescriptionTemplateChanged(DataTemplate oldValue, DataTemplate newValue) {
		}
		protected virtual void OnDescriptionTemplateSelectorChanged(DataTemplateSelector oldValue, DataTemplateSelector newValue) {
		}
		protected virtual void DescriptionContainerStyleChanged(Style oldValue, Style newValue) {
		}
		protected virtual void DescriptionContainerStyleSelectorChanged(StyleSelector oldValue, StyleSelector newValue) {
		}
		protected virtual void OnApplyingModeChanged(ApplyingMode oldValue, ApplyingMode newValue) {
			RaiseChanged(PropertyBuilderChangeKind.CoreProperties);
		}
		protected virtual void OnVisibilityChanged(Visibility oldValue, Visibility newValue) {
			RaiseChanged(PropertyBuilderChangeKind.CoreProperties);
		}		
		protected virtual void OnRowTemplateChanged(ControlTemplate oldValue, ControlTemplate newValue) {
			rowTemplate = newValue;
			RaiseNotifyPropertyChanged("RowTemplate");
		}
		protected virtual void OnDescriptionChanged(object oldValue, object newValue) {
		}
		protected virtual void OnPropertyDefinitionsChanged(PropertyDefinitionCollection oldValue, PropertyDefinitionCollection newValue) {
			if (oldValue != null) {
				oldValue.CollectionChanged -= OnPropertyDefinitionsCollectionChanged;
				ClearDefinitions(oldValue);
			}
			if (newValue != null) {
				newValue.CollectionChanged += OnPropertyDefinitionsCollectionChanged;
				SetUpDefinitions(newValue);
			}
			RaiseChanged(PropertyBuilderChangeKind.CoreProperties);
		}
		protected virtual void OnIsReadOnlyChanged(bool? oldValue, bool? newValue) {
		}
		protected virtual void OnPropertyDefinitionsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			ClearDefinitions(e.OldItems);
			SetUpDefinitions(PropertyDefinitions);
			RaiseChanged(PropertyBuilderChangeKind.CoreProperties);
		}
		protected virtual void OnHeaderChanged(string oldValue) {
		}
		protected virtual void OnPathChanged(string oldValue) {
			InvalidateSplittedPath();
			path = (string)this.GetValue(PropertyDefinitionBase.PathProperty);
			this.pathHashCode = path == null ? 0 : path.GetHashCode();
			RaiseChanged(PropertyBuilderChangeKind.CoreProperties);
		}
		protected virtual void OnParentDefinitionChanged(PropertyDefinitionBase oldValue, PropertyDefinitionBase newValue) {
			parentDefinition = newValue;
			if (oldValue != null) {
				oldValue.CheckRemoveCustomDefinition(this);
			}
			if (newValue != null) {
				newValue.CheckAddCustomDefinition(this);
			}
			RaiseChanged(PropertyBuilderChangeKind.CoreProperties);
		}		
		protected virtual void OnBuilderChanged(PropertyBuilder oldValue) {
			ActOnDefinitions(PropertyDefinitions, element => element.Builder = Builder);
			if (oldValue != null) {
				oldValue.CheckRemoveCustomDefinition(this);
			}
			if (Builder != null) {
				Builder.CheckAddCustomDefinition(this);
			}
			RaiseChanged(PropertyBuilderChangeKind.CoreProperties);
		}
		protected virtual void OnSortModeChanged(PropertyGridSortMode oldValue, PropertyGridSortMode newValue) {
			RaiseChanged(PropertyBuilderChangeKind.CoreProperties);
		}
		protected virtual void ClearDefinitions(IList collection) {
			ActOnDefinitions(collection, element => element.Builder = null, element => element.ParentDefinition = null);
		}
		protected virtual void SetUpDefinitions(IList collection) {
			ActOnDefinitions(collection, element => element.Builder = Builder, element => element.ParentDefinition = this);
		}
		protected internal void ActOnDefinitions(IList collection, params Action<PropertyDefinitionBase>[] actions) {
			if (collection == null)
				return;
			foreach (var element in collection.OfType<PropertyDefinitionBase>()) {
				foreach (var action in actions)
					action(element);
			}
		}				
		protected internal void Apply(RowDataBase data, bool mergeWithStandard = true) {
			MergeDefaultProperties(data);
			PropertyDefinitionBase standardDefinition = data.RowDataGenerator.GetStandardDefinition(data.Handle);
			MergeWithDefinition(data, standardDefinition, mergeWithStandard);
			if (data.Definition.Style == null)
				data.Definition.Style = standardDefinition.Style;
		}
		protected virtual void MergeWithDefinition(RowDataBase data, PropertyDefinitionBase standard, bool mergeWithStandard) {
			MergeDefaultPropertiesFromStandardDefinition(data, standard);
			if (mergeWithStandard)
				MergeOtherPropertiesFromStandardDefinition(data, standard);
		}
		protected virtual void MergeDefaultPropertiesFromStandardDefinition(RowDataBase data, PropertyDefinitionBase standard) {
			data.Header = GetMergedValue(HeaderProperty, standard, () => data.RowDataGenerator.DataView.GetDisplayName(data.Handle));
			data.Description = GetMergedValue<object>(DescriptionProperty, standard, () => data.RowDataGenerator.DataView.GetDescription(data.Handle));
			data.IsReadOnly = GetMergedValue(IsReadOnlyProperty, standard, () => data.RowDataGenerator.View.PropertyGrid.ReadOnly || data.RowDataGenerator.DataView.IsReadOnly(data.Handle));
			data.RenderReadOnly = data.RowDataGenerator.View.PropertyGrid.ReadOnly || data.IsReadOnly && data.RowDataGenerator.DataView.ShouldRenderReadOnly(data.Handle);
		}
		protected virtual void MergeOtherPropertiesFromStandardDefinition(RowDataBase data, PropertyDefinitionBase standard) {
		}
		T GetMergedValue<T>(DependencyProperty property, PropertyDefinitionBase standard, Func<T> getBaseValueHandler) {
			if (this.IsPropertySet(property)) {
				object value = GetValue(property);
				if (value is T)
					return (T)value;
			}
			if (standard.IsPropertySet(property)) {
				object value = standard.GetValue(property);
				if (value is T)
					return (T)value;
			}
			return getBaseValueHandler();
		}
		protected virtual void MergeDefaultProperties(RowDataBase data) {
			if (data == null)
				return;
			data.Definition = this;
			if (IsReadOnly.HasValue) {
				data.IsReadOnly = IsReadOnly.Value;
				data.RenderReadOnly = IsReadOnly.Value;
			}
			data.CanExpand &= ShowChildren;
		}
		readonly Locker linkLocker = new Locker();
		protected internal bool HasLinks {
			get { return linkLocker.IsLocked; }
		}		
		protected override IEnumerator LogicalChildren {
			get { return PropertyDefinitions.GetEnumerator(); }
		}
		protected virtual void CheckAddCustomDefinition(PropertyDefinitionBase definition) {
			if (definition == null)
				return;
			if (PropertyDefinitions.Return(x => x.Contains(definition), () => false))
				AddLogicalChild(definition);
		}
		protected internal virtual void CheckRemoveCustomDefinition(PropertyDefinitionBase definition) {
			if (definition == null)
				return;
			if (definition.Parent == this)
				RemoveLogicalChild(definition);
		}
		bool isInitializing = false;
		public override void BeginInit() {
			isInitializing = true;
			base.BeginInit();
		}
		public override void EndInit() {
			base.EndInit();
			isInitializing = false;
			RaiseChanged(PropertyBuilderChangeKind.Reset);
		}
		internal void RaiseChanged(PropertyBuilderChangeKind propertyBuilderChangeKind) {
			if (Builder == null || isInitializing)
				return;
			Builder.RaiseChanged(this, propertyBuilderChangeKind);
		}
		internal static int GetIndexInCollection(PropertyDefinitionBase definition) {
			if (definition == null || definition.isStandardDefinition)
				return Int32.MaxValue;
			if (definition.ParentDefinition != null) {
				if (definition.ParentDefinition.PropertyDefinitions.Contains(definition))
					return definition.ParentDefinition.PropertyDefinitions.IndexOf(definition);
			}
			else if (definition.Builder != null) {
				if (definition.Builder.Definitions.Contains(definition))
					return definition.Builder.Definitions.IndexOf(definition);
			}
			return Int32.MaxValue;
		}		
	}
	public class PropertyDefinition : PropertyDefinitionBase {
		public static readonly DependencyProperty TypeProperty;
		public static readonly DependencyProperty InsertDefinitionsFromProperty;
		public static readonly DependencyProperty ScopeProperty;
		public static readonly DependencyProperty CellTemplateProperty;
		public static readonly DependencyProperty CellTemplateSelectorProperty;
		public static readonly DependencyProperty EditSettingsProperty;
		public static readonly DependencyProperty AllowInstanceInitializerProperty;
		public static readonly DependencyProperty PostOnEditValueChangedProperty;
		public static readonly DependencyProperty AllowExpandingProperty;
		public static readonly DependencyProperty CommandParameterProperty;
		public static readonly DependencyProperty CommandProperty;
		public static readonly DependencyProperty ShowMenuButtonProperty;
		public static readonly DependencyProperty UseTypeConverterToStringConversionProperty;
		public static readonly DependencyProperty TypeMatchModeProperty;		
		static PropertyDefinition() {
			var ownerType = typeof(PropertyDefinition);
			CellTemplateProperty = DependencyPropertyManager.Register("CellTemplate", typeof(DataTemplate), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((PropertyDefinition)d).OnCellTemplateChanged((DataTemplate)e.OldValue)));
			CellTemplateSelectorProperty = DependencyPropertyManager.Register("CellTemplateSelector", typeof(DataTemplateSelector), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((PropertyDefinition)d).OnCellTemplateSelectorChanged((DataTemplateSelector)e.OldValue)));
			EditSettingsProperty = DependencyPropertyManager.Register("EditSettings", typeof(BaseEditSettings), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((PropertyDefinition)d).OnEditSettingsChanged((BaseEditSettings)e.OldValue)));
			TypeProperty = DependencyPropertyManager.Register("Type", typeof(Type), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((PropertyDefinition)d).OnTypeChanged((Type)e.OldValue)));
			InsertDefinitionsFromProperty = DependencyPropertyManager.Register("InsertDefinitionsFrom", ownerType, typeof(PropertyDefinition), new FrameworkPropertyMetadata(null, (d, e) => ((PropertyDefinition)d).OnInsertDefinitionsFromChanged((PropertyDefinition)e.OldValue, (PropertyDefinition)e.NewValue)));
			ScopeProperty = DependencyPropertyManager.Register("Scope", typeof(string), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((PropertyDefinition)d).OnScopeChanged((string)e.OldValue, (string)e.NewValue)));
			AllowInstanceInitializerProperty = DependencyPropertyManager.Register("AllowInstanceInitializer", typeof(bool?), typeof(PropertyDefinition), new FrameworkPropertyMetadata(null, (d, e) => ((PropertyDefinition)d).OnAllowInstanceInitializerChanged((bool?)e.OldValue, (bool?)e.NewValue)));
			PostOnEditValueChangedProperty = DependencyPropertyManager.Register("PostOnEditValueChanged", typeof(bool), typeof(PropertyDefinition), new FrameworkPropertyMetadata(false));
			AllowExpandingProperty = DependencyPropertyManager.Register("AllowExpanding", typeof(AllowExpandingMode?), typeof(PropertyDefinition), new FrameworkPropertyMetadata(null, (d, e) => ((PropertyDefinition)d).OnAllowExpandingChanged((AllowExpandingMode?)e.OldValue)));
			CommandParameterProperty = DependencyPropertyManager.Register("CommandParameter", typeof(object), typeof(PropertyDefinition), new FrameworkPropertyMetadata(null, (d, e) => ((PropertyDefinition)d).OnCommandParameterChanged((object)e.OldValue)));
			CommandProperty = DependencyPropertyManager.Register("Command", typeof(ICommand), typeof(PropertyDefinition), new FrameworkPropertyMetadata(null, (d, e) => ((PropertyDefinition)d).OnCommandChanged((ICommand)e.OldValue)));
			ShowMenuButtonProperty = DependencyPropertyManager.Register("ShowMenuButton", typeof(bool?), typeof(PropertyDefinition), new FrameworkPropertyMetadata(null, (d, e) => ((PropertyDefinition)d).OnShowMenuButtonChanged()));
			UseTypeConverterToStringConversionProperty = DependencyPropertyManager.Register("UseTypeConverterToStringConversion", typeof(bool), typeof(PropertyDefinition), new FrameworkPropertyMetadata(false, (d,e)=>((PropertyDefinition)d).OnUseTypeConverterToStringConversionChanged()));
			TypeMatchModeProperty = DependencyProperty.Register("TypeMatchMode", typeof(TypeMatchMode), typeof(PropertyDefinition), new PropertyMetadata(TypeMatchMode.Direct));
		}		
		public TypeMatchMode TypeMatchMode {
			get { return (TypeMatchMode)GetValue(TypeMatchModeProperty); }
			set { SetValue(TypeMatchModeProperty, value); }
		}
		public bool UseTypeConverterToStringConversion {
			get { return (bool)GetValue(UseTypeConverterToStringConversionProperty); }
			set { SetValue(UseTypeConverterToStringConversionProperty, value); }
		}
		public bool? ShowMenuButton {
			get { return (bool?)GetValue(ShowMenuButtonProperty); }
			set { SetValue(ShowMenuButtonProperty, value); }
		}
		public DataTemplate CellTemplate {
			get { return (DataTemplate)GetValue(CellTemplateProperty); }
			set { SetValue(CellTemplateProperty, value); }
		}
		public DataTemplateSelector CellTemplateSelector {
			get { return (DataTemplateSelector)GetValue(CellTemplateSelectorProperty); }
			set { SetValue(CellTemplateSelectorProperty, value); }
		}
		readonly CustomizationActionsField menuCustomizations;
		public IList<IControllerAction> MenuCustomizations { get { return menuCustomizations.Value; } }
		protected internal override void ExecuteMenuCustomizations(DependencyObject context) { menuCustomizations.Execute(context); }
		public BaseEditSettings EditSettings {
			get { return (BaseEditSettings)GetValue(EditSettingsProperty); }
			set { SetValue(EditSettingsProperty, value); }
		}
		public bool PostOnEditValueChanged {
			get { return (bool)GetValue(PostOnEditValueChangedProperty); }
			set { SetValue(PostOnEditValueChangedProperty, value); }
		}
		public AllowExpandingMode? AllowExpanding {
			get { return (AllowExpandingMode?)GetValue(AllowExpandingProperty); }
			set { SetValue(AllowExpandingProperty, value); }
		}
		public ICommand Command {
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}
		public object CommandParameter {
			get { return (object)GetValue(CommandParameterProperty); }
			set { SetValue(CommandParameterProperty, value); }
		}
		Type type = null;
		public Type Type {
			get { return type; }
			set { SetValue(TypeProperty, value); }
		}
		string scope = null;
		protected override string[] GetSplittedPath() {
			if (Scope == null && Path == null)
				return null;
			if (string.IsNullOrEmpty(Scope))
				return base.GetSplittedPath();
			return FieldNameHelper.GetPath(String.Format("{0}.{1}", Scope, Path.IfNot(String.IsNullOrEmpty) ?? "*"));
		}
		public string Scope {
			get { return scope; }
			set { SetValue(ScopeProperty, value); }
		}
		public PropertyDefinition InsertDefinitionsFrom {
			get { return (PropertyDefinition)GetValue(InsertDefinitionsFromProperty); }
			set { SetValue(InsertDefinitionsFromProperty, value); }
		}
		protected internal override sealed bool IsCategory {
			get { return false; }
		}
		PrioritizedDataTemplateSelector selectorWrapper;
		protected internal PrioritizedDataTemplateSelector SelectorWrapper {
			get { return selectorWrapper; }
		}
		public bool? AllowInstanceInitializer {
			get { return (bool?)GetValue(AllowInstanceInitializerProperty); }
			set { SetValue(AllowInstanceInitializerProperty, value); }
		}
		public PropertyDefinition() {
			menuCustomizations = new CustomizationActionsField(this, true);
		}
		internal event EventHandler ShowMenuButtonChanged;
		protected virtual void OnShowMenuButtonChanged() { if (ShowMenuButtonChanged != null) ShowMenuButtonChanged(this, null); }
		protected virtual void OnCellTemplateChanged(DataTemplate oldValue) {
			selectorWrapper = new PrioritizedDataTemplateSelector() { CustomTemplateSelector = CellTemplateSelector, CustomTemplate = CellTemplate };
		}
		protected virtual void OnCellTemplateSelectorChanged(DataTemplateSelector oldValue) {
			selectorWrapper = new PrioritizedDataTemplateSelector() { CustomTemplateSelector = CellTemplateSelector, CustomTemplate = CellTemplate };
		}
		protected virtual void OnAllowExpandingChanged(AllowExpandingMode? oldValue) {
			RaiseChanged(PropertyBuilderChangeKind.VisualClientProperties);
		}
		protected virtual void OnCommandParameterChanged(object oldValue) {
		}
		protected virtual void OnCommandChanged(ICommand oldValue) {
		}
		protected virtual void OnUseTypeConverterToStringConversionChanged() {
		}
		protected virtual void OnEditSettingsChanged(BaseEditSettings oldValue) {
			if (oldValue != null)
				RemoveLogicalChild(oldValue);
			if (EditSettings != null)
				AddLogicalChild(EditSettings);
		}
		protected virtual void OnTypeChanged(Type oldValue) {
			type = (Type)this.GetValue(PropertyDefinition.TypeProperty);
			this.typeHashCode = type == null ? 0 : type.GetHashCode();
			RaiseChanged(PropertyBuilderChangeKind.CoreProperties);
		}
		protected virtual void OnInsertDefinitionsFromChanged(PropertyDefinition oldValue, PropertyDefinition newValue) { RaiseChanged(PropertyBuilderChangeKind.CoreProperties); }
		protected virtual void OnScopeChanged(string oldValue, string newValue) {
			InvalidateSplittedPath();
			scope = (string)this.GetValue(PropertyDefinition.ScopeProperty);
			this.scopeHashCode = scope == null ? 0 : scope.GetHashCode();
			RaiseChanged(PropertyBuilderChangeKind.CoreProperties);
		}
		protected virtual void OnAllowInstanceInitializerChanged(bool? oldValue, bool? newValue) { RaiseChanged(PropertyBuilderChangeKind.VisualClientProperties); }
		protected override IEnumerator LogicalChildren {
			get {
				var lst = PropertyDefinitions.ToList<object>();
				if (EditSettings != null)
					lst.Add(EditSettings);
				return lst.GetEnumerator();
			}
		}
		protected override void MergeDefaultProperties(RowDataBase data) {
			base.MergeDefaultProperties(data);
			if (data == null)
				return;
			data.Command = Command;
			data.CommandParameter = CommandParameter;
			data.IsReadOnly |= PropertyGridEditSettingsHelper.GetIsReadOnly(data.EditSettings);
		}
		protected override void MergeOtherPropertiesFromStandardDefinition(RowDataBase data, PropertyDefinitionBase standardDefinition) {
			base.MergeOtherPropertiesFromStandardDefinition(data, standardDefinition);
			var sd = standardDefinition as PropertyDefinition;
			if (sd != null) 
				MergeSettings(data, sd);
		}
		void MergeSettings(RowDataBase data, PropertyDefinition standardDefinition) {
			var settings = EditSettings;
			if (settings == null)
				settings = standardDefinition.With(x => x.EditSettings);
			PreprocessEditSettings(settings);
			data.EditSettings = settings;
		}
		protected virtual void PreprocessEditSettings(BaseEditSettings settings) {
			(settings as PopupBaseEditSettings).Do(x => PropertyGridEditSettingsHelper.SetPostOnPopupClosed(x, true));
			(settings as CheckEditSettings).Do(x => PropertyGridEditSettingsHelper.SetPostOnEditValueChanged(x, true));
		}
	}
	public class RootPropertyDefinitionExtension : MarkupExtension {
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return new RootPropertyDefinition();
		}
	}
	[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
	public sealed class RootPropertyDefinition : PropertyDefinition {
		internal RootPropertyDefinition() { }
	}
	public class CategoryDefinition : PropertyDefinitionBase {		
		static CategoryDefinition() {
			var ownerType = typeof(CategoryDefinition);
			HeaderShowModeProperty.OverrideMetadata(typeof(CategoryDefinition), new FrameworkPropertyMetadata(HeaderShowMode.OnlyHeader));
		}		
		protected internal override sealed bool IsCategory {
			get {
				return true;
			}
		}
		public CategoryDefinition() { }
		protected override string CoercePath(string path) {
			if (path == null)
				return path;
			if (path.StartsWith("<") && path.EndsWith(">"))
				return path;
			else
				return String.Format("<{0}>", path);
		}
		protected override void CheckAddCustomDefinition(PropertyDefinitionBase definition) {
			if (definition == null)
				return;
			if (PropertyDefinitions.Contains(definition))
				AddLogicalChild(definition);
		}
	}
	[ContentProperty("PropertyDefinitions")]
	public class CollectionDefinition : PropertyDefinition {
		public static readonly DependencyProperty NewItemInitializerProperty;
		public static readonly DependencyProperty UseCollectionEditorProperty;
		public static readonly DependencyProperty AllowNewItemInitializerProperty;
		static CollectionDefinition() {
			NewItemInitializerProperty = DependencyPropertyManager.Register("NewItemInitializer", typeof(IInstanceInitializer), typeof(CollectionDefinition), new FrameworkPropertyMetadata(null, (d, e) => ((CollectionDefinition)d).RaiseChanged(PropertyBuilderChangeKind.VisualClientProperties)));
			UseCollectionEditorProperty = DependencyPropertyManager.Register("UseCollectionEditor", typeof(bool?), typeof(CollectionDefinition), new FrameworkPropertyMetadata(null, (d, e) => ((CollectionDefinition)d).OnUseCollectionEditorChanged((bool?)e.OldValue, (bool?)e.NewValue)));
			AllowNewItemInitializerProperty = DependencyPropertyManager.Register("AllowNewItemInitializer", typeof(bool?), typeof(CollectionDefinition), new FrameworkPropertyMetadata(null, (d, e) => ((CollectionDefinition)d).OnAllowNewItemInitializerChanged((bool?)e.OldValue, (bool?)e.NewValue)));
		}
		public CollectionDefinition() {
		}
		public IInstanceInitializer NewItemInitializer {
			get { return (IInstanceInitializer)GetValue(NewItemInitializerProperty); }
			set { SetValue(NewItemInitializerProperty, value); }
		}
		public bool? UseCollectionEditor {
			get { return (bool?)GetValue(UseCollectionEditorProperty); }
			set { SetValue(UseCollectionEditorProperty, value); }
		}
		public bool? AllowNewItemInitializer {
			get { return (bool?)GetValue(AllowNewItemInitializerProperty); }
			set { SetValue(AllowNewItemInitializerProperty, value); }
		}
		protected virtual void OnUseCollectionEditorChanged(bool? oldValue, bool? newValue) {
			RaiseChanged(PropertyBuilderChangeKind.CoreProperties);
		}
		protected virtual void OnAllowNewItemInitializerChanged(bool? oldValue, bool? newValue) {
			RaiseChanged(PropertyBuilderChangeKind.CoreProperties);
		}
	}
	[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
	public class PropertyDefinitionCollection : ObservableCollection<PropertyDefinitionBase> {
		readonly PropertyDefinitionBase owner;
		public PropertyDefinitionCollection(PropertyDefinitionBase owner) {
			this.owner = owner;
		}
		protected override void InsertItem(int index, PropertyDefinitionBase item) {			
			base.InsertItem(index, item);
			item.ParentDefinition = owner;
		}
		protected override void RemoveItem(int index) {
			var element = this[index];
			element.ParentDefinition = null;
			base.RemoveItem(index);
		}
		protected override void ClearItems() {
			while (Count != 0)
				RemoveAt(0);
		}
	}
}
