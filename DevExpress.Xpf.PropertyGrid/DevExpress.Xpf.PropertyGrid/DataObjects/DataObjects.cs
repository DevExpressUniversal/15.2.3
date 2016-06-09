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

using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Validation;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.PropertyGrid.Internal;
using DevExpress.Xpf.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
namespace DevExpress.Xpf.PropertyGrid {
#if DEBUGTEST    
	[System.Diagnostics.DebuggerDisplay("RowDataBase::{GetHashCode()}::{FullPath}")]
#endif
	public class RowDataBase : EditableDataObject, ISupportInitialize {
		public static readonly DependencyProperty CommandProperty;
		public static readonly DependencyProperty CommandParameterProperty;
		public static readonly DependencyProperty CanExpandProperty;
		public static readonly DependencyProperty IsExpandedProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		protected static readonly DependencyPropertyKey ForcedChildrenPropertyKey;
		[IgnoreDependencyPropertiesConsistencyChecker]
		protected static readonly DependencyProperty ForcedChildrenProperty;		
		public static readonly DependencyProperty ParentProperty;
		public static readonly DependencyProperty HeaderProperty;
		public static readonly DependencyProperty ValueTypeProperty;
		public static readonly DependencyProperty DescriptionProperty;
		public static readonly DependencyProperty IsSelectedProperty;
		public static readonly DependencyProperty LevelProperty;
		public static readonly DependencyProperty HandleProperty;
		public static readonly DependencyProperty RowDataGeneratorProperty;
		public static readonly DependencyProperty IsReadOnlyProperty;
		public static readonly DependencyProperty EditSettingsProperty;
		public static readonly DependencyProperty RenderReadOnlyProperty;
		protected static readonly DependencyPropertyKey DefinitionPropertyKey;
		protected static readonly DependencyPropertyKey FullPathPropertyKey;
		public static readonly DependencyProperty EditableObjectProperty;
		public static readonly DependencyProperty FullPathProperty;
		public static readonly DependencyProperty DefinitionProperty;
		public static readonly DependencyProperty StandardValuesProperty;
		public static readonly DependencyProperty ShouldHighlightValueProperty;
		public static readonly DependencyProperty IsModifiableCollectionItemProperty;
		public static readonly DependencyProperty IsCollectionRowProperty;
		public static readonly DependencyProperty ActualTypeInfoProperty;
		public static readonly DependencyProperty ValidationErrorProperty;
		static RowDataBase() {
			Type ownerType = typeof(RowDataBase);
			ForcedChildrenPropertyKey = DependencyPropertyManager.RegisterReadOnly("ForcedChildren", typeof(RowDataBaseCollection), typeof(RowDataBase), new FrameworkPropertyMetadata(null));
			ForcedChildrenProperty = ForcedChildrenPropertyKey.DependencyProperty;
			CommandProperty = DependencyPropertyManager.Register("Command", typeof(ICommand), ownerType, new FrameworkPropertyMetadata(null));
			CanExpandProperty = DependencyPropertyManager.Register("CanExpand", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			IsExpandedProperty = DependencyPropertyManager.Register("IsExpanded", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, (d, e) => ((RowDataBase)d).OnIsExpandedChanged((bool)e.OldValue), (d, e) => ((RowDataBase)d).CoerceIsExpanded((bool)e)));			
			ParentProperty = DependencyPropertyManager.Register("Parent", ownerType, ownerType,
				new FrameworkPropertyMetadata(null, (d, e) => ((RowDataBase)d).OnParentChanged(((RowDataBase)e.OldValue))));
			HeaderProperty = DependencyPropertyManager.Register("Header", typeof(string), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((RowDataBase)d).RaiseHeaderChanged()));
			LevelProperty = DependencyPropertyManager.Register("Level", typeof(int), ownerType, new FrameworkPropertyMetadata(0));
			ValueTypeProperty = DependencyPropertyManager.Register("ValueType", typeof(Type), ownerType,
				new FrameworkPropertyMetadata(typeof(object), (d, e) => ((RowDataBase)d).OnValueTypeChanged(((Type)e.OldValue))));
			DescriptionProperty = DependencyPropertyManager.Register("Description", typeof(object), ownerType, new FrameworkPropertyMetadata(null));
			IsSelectedProperty = DependencyPropertyManager.Register("IsSelected", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, (d, e) => ((RowDataBase)d).OnIsSelectedChanged((bool)e.OldValue)));
			HandleProperty = DependencyPropertyManager.Register("handle", typeof(RowHandle), ownerType,
				new FrameworkPropertyMetadata(null, (d, e) => ((RowDataBase)d).OnHandleChanged((RowHandle)e.OldValue)));
			RowDataGeneratorProperty = DependencyPropertyManager.Register("RowDataGenerator", typeof(RowDataGenerator), ownerType,
				new FrameworkPropertyMetadata(null, (d, e) => ((RowDataBase)d).OnRowDataGeneratorChanged((RowDataGenerator)e.OldValue)));			
			IsReadOnlyProperty = DependencyPropertyManager.Register("IsReadOnly", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, (d, e) => ((RowDataBase)d).OnIsReadOnlyChanged((bool)e.OldValue)));
			EditSettingsProperty = DependencyPropertyManager.Register("EditSettings", typeof(BaseEditSettings), ownerType,
				new FrameworkPropertyMetadata(null, (d, e) => ((RowDataBase)d).OnEditSettingsChanged((BaseEditSettings)e.OldValue)));
			RenderReadOnlyProperty = DependencyPropertyManager.Register("RenderReadOnly", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(true, (d, e) => ((RowDataBase)d).OnRenderReadOnlyChanged((bool)e.OldValue)));
			DefinitionPropertyKey = DependencyPropertyManager.RegisterReadOnly("Definition", typeof(PropertyDefinitionBase), ownerType,
				new FrameworkPropertyMetadata(null, (d, e) => ((RowDataBase)d).OnDefinitionChanged((PropertyDefinitionBase)e.OldValue, (PropertyDefinitionBase)e.NewValue)));
			StandardValuesProperty = DependencyPropertyManager.Register("StandardValues", typeof(IEnumerable), ownerType,
				new FrameworkPropertyMetadata(null, (d, e) => ((RowDataBase)d).OnStandardValuesChanged((IEnumerable)e.OldValue, (IEnumerable)e.NewValue)));
			ShouldHighlightValueProperty = DependencyPropertyManager.Register("ShouldHighlightValue", typeof(bool?), typeof(RowDataBase), new FrameworkPropertyMetadata(null));
			FullPathPropertyKey = DependencyPropertyManager.RegisterReadOnly("FullPath", typeof(string), typeof(RowDataBase), new FrameworkPropertyMetadata(null));
			EditableObjectProperty = DependencyPropertyManager.Register("EditableObject", typeof(object), typeof(RowDataBase),
				new FrameworkPropertyMetadata(null, (d, e) => ((RowDataBase)d).EditableObjectChanged(e.OldValue, e.NewValue)));
			IsModifiableCollectionItemProperty = DependencyPropertyManager.Register("IsModifiableCollectionItem", typeof(bool), typeof(RowDataBase), new FrameworkPropertyMetadata(false, (d,e)=>((RowDataBase)d).OnIsModifiableCollectionItemChanged()));
			IsCollectionRowProperty = DependencyPropertyManager.Register("IsCollectionRow", typeof(bool), typeof(RowDataBase), new FrameworkPropertyMetadata(false, (d,e)=>((RowDataBase)d).OnIsCollectionRowChanged()));
			CommandParameterProperty = DependencyPropertyManager.Register("CommandParameter", typeof(object), typeof(RowDataBase), new FrameworkPropertyMetadata(null));
			FullPathProperty = FullPathPropertyKey.DependencyProperty;
			DefinitionProperty = DefinitionPropertyKey.DependencyProperty;
			ActualTypeInfoProperty = DependencyPropertyManager.Register("ActualTypeInfo", typeof(TypeInfo), typeof(RowDataBase), new FrameworkPropertyMetadata(null));
			ValidationErrorProperty = DependencyPropertyManager.Register("ValidationError", typeof(BaseValidationError), typeof(RowDataBase),
				new FrameworkPropertyMetadata(null, (d, e) => ((RowDataBase)d).OnValidationErrorChanged((BaseValidationError)e.OldValue)));
		}
		protected virtual void OnIsModifiableCollectionItemChanged() {
			RaiseIsModifiableCollectionItemChanged();
		}
		protected virtual void OnIsCollectionRowChanged() {
			RaiseIsCollectionRowChanged();
		}
		public TypeInfo ActualTypeInfo {
			get { return (TypeInfo)GetValue(ActualTypeInfoProperty); }
			set { SetValue(ActualTypeInfoProperty, value); }
		}
		public bool IsCollectionRow {
			get { return (bool)GetValue(IsCollectionRowProperty); }
			set { SetValue(IsCollectionRowProperty, value); }
		}
		public bool IsModifiableCollectionItem {
			get { return (bool)GetValue(IsModifiableCollectionItemProperty); }
			set { SetValue(IsModifiableCollectionItemProperty, value); }
		}
		public BaseValidationError ValidationError {
			get { return (BaseValidationError)GetValue(ValidationErrorProperty); }
			set { SetValue(ValidationErrorProperty, value); }
		}
		public string Header {
			get { return (string)GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}
		public bool? ShouldHighlightValue {
			get { return (bool?)GetValue(ShouldHighlightValueProperty); }
			set { SetValue(ShouldHighlightValueProperty, value); }
		}
		public RowDataBase Parent {
			get { return (RowDataBase)GetValue(ParentProperty); }
			set { SetValue(ParentProperty, value); }
		}
		public RowDataBase Root {
			get { return RowDataGenerator.With(x => x.RowDataFromHandle(RowHandle.Root)); }
		}
		protected internal RowDataBaseCollection ForcedChildren {
			get { return (RowDataBaseCollection)GetValue(ForcedChildrenProperty); }
			private set { this.SetValue(ForcedChildrenPropertyKey, value); }
		}		
		public bool IsExpanded {
			get { return (bool)GetValue(IsExpandedProperty); }
			set { SetValue(IsExpandedProperty, value); }
		}
		public bool CanExpand {
			get { return (bool)GetValue(CanExpandProperty); }
			set { SetValue(CanExpandProperty, value); }
		}
		public int Level {
			get { return (int)GetValue(LevelProperty); }
			set { SetValue(LevelProperty, value); }
		}
		public ICommand Command {
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}
		public object CommandParameter {
			get { return (object)GetValue(CommandParameterProperty); }
			set { SetValue(CommandParameterProperty, value); }
		}		
		public IList<IControllerAction> Actions {
			get { return actionGroup.Actions; }
		}
		protected internal void ExecuteMenuCustomizations(DependencyObject context) {
			((IControllerAction)actionGroup).Execute(context);
		}
		protected internal EditorColumn Column { get; private set; }
		public Type ValueType {
			get { return (Type)GetValue(ValueTypeProperty); }
			set { SetValue(ValueTypeProperty, value); }
		}
		public object Description {
			get { return (object)GetValue(DescriptionProperty); }
			set { SetValue(DescriptionProperty, value); }
		}
		public bool IsSelected {
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		public RowHandle Handle {
			get { return (RowHandle)GetValue(HandleProperty); }
			set { SetValue(HandleProperty, value); }
		}
		public RowDataGenerator RowDataGenerator {
			get { return (RowDataGenerator)GetValue(RowDataGeneratorProperty); }
			set { SetValue(RowDataGeneratorProperty, value); }
		}
		public bool IsReadOnly {
			get { return (bool)GetValue(IsReadOnlyProperty); }
			set { SetValue(IsReadOnlyProperty, value); }
		}		
		public BaseEditSettings EditSettings {
			get { return (BaseEditSettings)GetValue(EditSettingsProperty); }
			set { SetValue(EditSettingsProperty, value); }
		}
		public bool RenderReadOnly {
			get { return (bool)GetValue(RenderReadOnlyProperty); }
			set { SetValue(RenderReadOnlyProperty, value); }
		}
		public PropertyDefinitionBase Definition {
			get { return (PropertyDefinitionBase)GetValue(DefinitionProperty); }
			protected internal set { this.SetValue(DefinitionPropertyKey, value); }
		}
		public IEnumerable StandardValues {
			get { return (IEnumerable)GetValue(StandardValuesProperty); }
			set { SetValue(StandardValuesProperty, value); }
		}
		public string FullPath {
			get { return (string)GetValue(FullPathProperty); }
			protected internal set { this.SetValue(FullPathPropertyKey, value); }
		}
		public object EditableObject {
			get { return (object)GetValue(EditableObjectProperty); }
			set { SetValue(EditableObjectProperty, value); }
		}
		internal event EventHandler IsReadOnlyChanged;
		void RaiseIsReadOnlyChanged() { if (IsReadOnlyChanged != null) IsReadOnlyChanged(this, null); }
		internal event EventHandler IsExpandedChanged;
		void RaiseIsExpandedChanged() { if (IsExpandedChanged != null) IsExpandedChanged(this, null); }
		internal event EventHandler IsSelectedChanged;
		void RaiseIsSelectedChanged() { if (IsSelectedChanged != null) IsSelectedChanged(this, null); }
		internal event EventHandler IsCollectionRowChanged;
		void RaiseIsCollectionRowChanged() { if (IsCollectionRowChanged != null) IsCollectionRowChanged(this, null); }
		internal event EventHandler IsModifiableCollectionItemChanged;
		void RaiseIsModifiableCollectionItemChanged() { if (IsModifiableCollectionItemChanged != null) IsModifiableCollectionItemChanged(this, null); }
		internal event EventHandler ShowMenuButtonChanged;
		void RaiseShowMenuButtonChanged() { if (ShowMenuButtonChanged != null) ShowMenuButtonChanged(this, null); }
		internal event EventHandler RenderReadOnlyChanged;
		void RaiseRenderReadOnlyChanged() { if (RenderReadOnlyChanged != null) RenderReadOnlyChanged(this, null); }
		internal event EventHandler HeaderChanged;
		void RaiseHeaderChanged() { if (HeaderChanged != null) HeaderChanged(this, null); }
		internal bool IsDirty = false;
		readonly ActionGroup actionGroup;
		protected internal bool IsReady { get; set; }
		public RowDataBase() {
			actionGroup = new ActionGroup();
			ForcedChildren = new RowDataBaseCollection(this);
			Column = new EditorColumn(this);
			IsReady = false;
		}
		protected virtual void EditableObjectChanged(object oldValue, object newValue) {
			if (newValue is EditableObjectWrapper || valueChangedLocker.IsLocked)
				return;
			RowDataGenerator.SetValue(Handle, newValue);
		}
		public object CoerceIsExpanded(bool value) {
			return value && CanExpand;
		}
		protected virtual void OnHandleChanged(RowHandle oldValue) {
			UpdateParent();
		}
		protected virtual void UpdateParent() {
		}		
		protected virtual void OnIsExpandedChanged(bool oldValue) {
			if (RowDataGenerator != null)
				RowDataGenerator.SetExpanded(Handle, IsExpanded);
			RaiseIsExpandedChanged();
		}
		protected virtual void OnRowDataGeneratorChanged(RowDataGenerator oldValue) {
		}		
		protected virtual void OnIsEditorFocusedChanged(bool oldValue) {
		}
		public void OnIsSelectedChanged(bool oldValue) {
			RaiseIsSelectedChanged();
		}
		protected virtual void OnIndentChanged(double p) {
		}
		protected virtual void OnParentChanged(RowDataBase rowDataBase) {
		}		
		protected virtual void OnValidationErrorChanged(BaseValidationError oldValue) {
			RowDataGenerator.SetViewValidationError(Handle, ValidationError);
		}
		protected virtual void OnValueTypeChanged(Type p) { }
		readonly Locker valueChangedLocker = new Locker();
		protected internal virtual void SetValueInternal(object value) {
			valueChangedLocker.DoLockedAction(() => Value = value);
		}
		protected override object CoerceValue(object newValue) {
			var propertyGrid = RowDataGenerator.With(x => x.View).With(x => x.PropertyGrid);
			if (!valueChangedLocker.IsLocked && propertyGrid != null && !IsDirty && Handle != null && RowDataGenerator != null && RowDataGenerator.DataView != null)
				if (!propertyGrid.RaiseCellValueChanging(this, Value, newValue))
					return Value;
			return newValue;
		}
		protected override void OnValueChanged(object oldValue) {
			if (ReferenceEquals(oldValue, Value))
				return;
			RaiseContentChanged();
			var propertyGrid = RowDataGenerator.With(x => x.View).With(x => x.PropertyGrid);
			if (!valueChangedLocker.IsLocked && propertyGrid != null && !IsDirty && Handle != null && RowDataGenerator != null && RowDataGenerator.DataView != null) {
				Exception exception = propertyGrid == null ? null : propertyGrid.RaiseValidateCell(this, oldValue, Value);
				exception = exception ?? RowDataGenerator.DataView.SetValue(Handle, Value);
				string message = exception == null ? null : exception.Message;
				message = propertyGrid.RaiseInvalidCellException(this, exception);
				if (exception == null) {
					RowDataGenerator.DataView.Update();
					propertyGrid.RaiseCellValueChanged(this, oldValue, RowDataGenerator.DataView.GetValue(Handle));
				}
				else
					SetValueInternal(RowDataGenerator.DataView.GetValue(Handle));
			}
		}
		protected virtual void OnRowControlChanged(RowControlBase oldValue) {
		}
		protected virtual void OnEditSettingsChanged(BaseEditSettings oldValue) {
			UpdateEditSettingsSource();
			if (Definition == null || !Definition.isStandardDefinition)
				Column.RaiseContentChanged(null);
		}
		void UpdateEditSettingsSource() {
			var utc = (Definition as PropertyDefinition).Return(x => x.UseTypeConverterToStringConversion, () => false);
			var source = (EditSettings as LookUpEditSettingsBase).With(x => x.ItemsSource as StandardValuesSource);
			if (source == null)
				return;
			source.UseTypeConverterToStringConversion = utc;
		}
		protected internal bool ShouldConvertValue {
			get {
				var shouldConvert = Definition.With(x => x as PropertyDefinition).Return(x => x.UseTypeConverterToStringConversion, () => false);
				return shouldConvert && RowDataGenerator.With(x => x.DataView).Return(x => x.CanConvertToString(Handle), () => false);
			}
		}
		protected virtual void OnRenderReadOnlyChanged(bool oldValue) {
			RaiseRenderReadOnlyChanged();
		}		
		protected internal bool IsCategory { get; set; }
		protected internal virtual bool? CanShowMenu { get { return IsCategory ? false : (Definition as PropertyDefinition).Return(x => x.ShowMenuButton, () => null); } }
		protected virtual void OnActualIsReadOnlyChanged(bool oldValue) {
		}
		protected virtual void OnDefinitionChanged(PropertyDefinitionBase oldValue, PropertyDefinitionBase newValue) {			
			if (Initializing || IsDirty)
				return;
			UpdateEditSettingsSource();
			if ((newValue == null || !newValue.isStandardDefinition) && RowDataGenerator != null && RowDataGenerator.DataView != null && Handle != null)
				RowDataGenerator.DataView.Invalidate(Handle);
			bool? oldsmb = null;
			bool? newsmb = null;
			if (oldValue is PropertyDefinition) {
				((PropertyDefinition)oldValue).ShowMenuButtonChanged -= OnDefinitionShowMenuButtonChanged;
				oldsmb = ((PropertyDefinition)oldValue).ShowMenuButton;
			}				
			if (newValue is PropertyDefinition) {
				((PropertyDefinition)newValue).ShowMenuButtonChanged += OnDefinitionShowMenuButtonChanged;
				newsmb = ((PropertyDefinition)oldValue).ShowMenuButton;
			}
			if ((!oldsmb.HasValue || !newsmb.HasValue) || oldsmb != newsmb)
				RaiseShowMenuButtonChanged();
		}
		void OnDefinitionShowMenuButtonChanged(object sender, EventArgs args) {
			RaiseShowMenuButtonChanged();
		}
		protected virtual void OnStandardValuesChanged(IEnumerable oldValue, IEnumerable newValue) {
		}
		protected virtual void OnIsReadOnlyChanged(bool oldValue) {
			RaiseIsReadOnlyChanged();
		}
		#region ISupportInitialize Members
		Locker initializationLocker = new Locker();
		protected internal bool Initializing {
			get { return initializationLocker.IsLocked; }
		}
		public void BeginInit() {
			initializationLocker.Lock();
		}
		public void EndInit() {
			initializationLocker.Unlock();
			UpdateParent();
		}
		#endregion
		public void SetEditableObject(EditableObjectWrapper wrapper) {
			valueChangedLocker.DoLockedAction(() => EditableObject = wrapper);
		}
#if DEBUGTEST
		public override string ToString() {
			return String.Format("RowDataBase::{0}::{1}", GetHashCode(), FullPath);
		}
#endif
	}
	public class EditorColumn : IInplaceEditorColumn {
		[ThreadStatic]
		static BaseEditSettings cachedEditSettings;
		WeakReference ownerWR = new WeakReference(null);
		public RowDataBase Owner {
			get { return (RowDataBase)ownerWR.Target; }
			set { ownerWR = new WeakReference(value); }
		}
		public EditorColumn(RowDataBase owner) {
			this.Owner = owner;
		}
		#region IInplaceEditorColumnImplementation
		ColumnContentChangedEventHandler contentChanged;
		event ColumnContentChangedEventHandler IInplaceEditorColumn.ContentChanged {
			add { contentChanged += value; }
			remove { contentChanged -= value; }
		}
		protected internal void RaiseContentChanged(DependencyProperty property) {
			if (contentChanged != null)
				contentChanged(this, new ColumnContentChangedEventArgs(property));
		}
		ControlTemplate IInplaceEditorColumn.DisplayTemplate {
			get { return null; }
		}
		BaseEditSettings IInplaceEditorColumn.EditSettings {
			get { return Owner.With(x => x.EditSettings) ?? (cachedEditSettings ?? (cachedEditSettings = new TextEditSettings())); }
		}
		ControlTemplate IInplaceEditorColumn.EditTemplate {
			get { return null; }
		}
		DataTemplateSelector IInplaceEditorColumn.EditorTemplateSelector {
			get {
				if (Owner.With(x => x.Definition) as PropertyDefinition == null)
					return null;
				return ((PropertyDefinition)Owner.With(x => x.Definition)).SelectorWrapper;
			}
		}
		HorizontalAlignment IDefaultEditorViewInfo.DefaultHorizontalAlignment {
			get { return HorizontalAlignment.Stretch; }
		}
		bool IDefaultEditorViewInfo.HasTextDecorations { get { return false; } }
		#endregion
	}
	[Obsolete]
	public class RowData : RowDataBase {
	}
	[Obsolete]
	public class CategoryData : RowDataBase {		
	}
	class DisconnectedRowData : RowDataBase {
	}
}
