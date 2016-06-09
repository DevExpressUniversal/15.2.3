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
using System.Windows;
using System.Collections;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Controls;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Utils;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Bars.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Core.Internal;
using DevExpress.Xpf.Bars.Customization;
using System.Collections.Generic;
namespace DevExpress.Xpf.Ribbon {
	public class RibbonCustomizationHelper : DependencyObject {
		public RibbonCustomizationHelper(RibbonControl ribbon) {
			Ribbon = ribbon;
		}
		private void OnIsCustomizationModeChanged(DependencyPropertyChangedEventArgs e) {
			if (IsCustomizationMode == true)
				ShowCustomizationForm();
			else {
				if (Ribbon != null && Ribbon.CategoriesPane != null && Ribbon.CategoriesPane.RibbonItemsPanel != null)
					Ribbon.CategoriesPane.RibbonItemsPanel.InvalidateMeasure();
			}
		}
		private object OnIsCustomizationModeChangingCoerce(object o) {
			if (Ribbon == null || !Ribbon.AllowCustomization || Ribbon.IsItemsSourceModeUsedWithin)
				return false;
			return (bool)o;
		}
		#region static
		public static readonly DependencyProperty IsCustomizationModeProperty;
		public static readonly DependencyProperty RibbonProperty;
		static RibbonCustomizationHelper() {
			IsCustomizationModeProperty = DependencyPropertyManager.Register("IsCustomizationMode", typeof(bool), typeof(RibbonCustomizationHelper), new PropertyMetadata(false, new PropertyChangedCallback((d, e) => { ((RibbonCustomizationHelper)d).OnIsCustomizationModeChanged(e); }), new CoerceValueCallback((d, o) => { return ((RibbonCustomizationHelper)d).OnIsCustomizationModeChangingCoerce(o); })));
			RibbonProperty = DependencyPropertyManager.Register("Ribbon", typeof(RibbonControl), typeof(RibbonCustomizationHelper), new PropertyMetadata(null));
		}
		#endregion
		#region props
		public RibbonControl Ribbon {
			get { return (RibbonControl)GetValue(RibbonProperty); }
			set { SetValue(RibbonProperty, value); }
		}
		public bool IsCustomizationMode {
			get { return (bool)GetValue(IsCustomizationModeProperty); }
			set { SetValue(IsCustomizationModeProperty, value); }
		}
		#endregion        
		#region CustomizationFormLogic
		protected internal FloatingContainer CustomizationForm { get; set; }
		public void ShowCustomizationForm() {
			CustomizationForm = CreateCustomizationForm();
			if (CustomizationForm == null)
				return;
			CustomizeCustomizationForm();
			CustomizationForm.Hidden += new RoutedEventHandler(OnCustomizationFormHidden);
			CustomizationForm.IsOpen = true;
			IsCustomizationMode = true;
			FloatingContainer.SetIsActive(CustomizationForm, true);
		}
		FloatingContainer CreateCustomizationForm() {
			FloatingContainer container = FloatingContainerFactory.Create(FloatingMode.Window);
			container.BeginUpdate();
			Ribbon.AddLogicalChildCore(container);
			if (Ribbon == null)
				return null;
			object obj = DevExpress.Xpf.Core.Native.LayoutHelper.FindRoot(Ribbon);
			if (obj is Window)
				container.Owner = (FrameworkElement)((Window)obj).Content;
			else if (obj is FrameworkElement)
				container.Owner = (FrameworkElement)obj;
			container.Owner = container.Owner ?? (FrameworkElement)Ribbon.With(x => x.Manager) ?? Ribbon;
			container.ShowModal = true;
			container.Content = Ribbon.CreateCustomizationControl();
			container.EndUpdate();
			return container;
		}
		void CustomizeCustomizationForm() {
			CustomizationForm.AllowSizing = true;
			CustomizationForm.CloseOnEscape = true;
			CustomizationForm.ContainerStartupLocation = WindowStartupLocation.CenterOwner;
			CustomizationForm.Caption = RibbonControlLocalizer.GetString(RibbonControlStringId.RibbonCustomizationStrings_CustomizationFormCaption);
			CustomizationForm.FloatSize = new Size(750, 500);
			CustomizationForm.MinHeight = 500;
			CustomizationForm.MinWidth = 750;
			if (Ribbon != null)
				Ribbon.AdjustCustomizationForm(CustomizationForm);
		}
		void OnCustomizationFormHidden(object sender, RoutedEventArgs e) {
			CustomizationForm.Hidden -= OnCustomizationFormHidden;
			CustomizationForm.BeginUpdate();
			CustomizationForm.Content = null;
			CustomizationForm.EndUpdate();
			if (CustomizationForm.Owner != null) {
				var window = DevExpress.Xpf.Core.Native.LayoutHelper.FindParentObject<Window>(CustomizationForm.Owner);
				if (window != null) {
					window.Activate();
				}
			}
			CustomizationForm = null;
		}
		#endregion
		#region HelperLogic
		protected internal static void SetCurrentValueForProperty(PropertyDescriptor prop, object value, DependencyObject target) {
			string propertyName = prop.Name;
			System.Reflection.FieldInfo info = target.GetType().GetField(propertyName + "Property",
						System.Reflection.BindingFlags.FlattenHierarchy
						| System.Reflection.BindingFlags.Public
						| System.Reflection.BindingFlags.Static);
			if (info == null)
				return;
			object targetValue = null;
			if ((info.GetValue(target) as DependencyProperty).PropertyType == typeof(string) || (info.GetValue(target) as DependencyProperty).PropertyType == typeof(object))
				targetValue = value;
			else {
				System.Reflection.MethodInfo mInfo = (info.GetValue(target) as DependencyProperty).PropertyType.GetMethod("Parse", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.FlattenHierarchy | System.Reflection.BindingFlags.Static);
				if (mInfo != null)
					targetValue = mInfo.Invoke(null, new object[] { value });
			}
			if (targetValue == null)
				return;
			target.SetCurrentValue(info.GetValue(target) as DependencyProperty, targetValue);
		}
		protected internal static IList GetCollectionFromDependencyObject(DependencyObject obj) {
			if (obj is RibbonControl)
				return (obj as RibbonControl).Categories;
			if (obj is RibbonPageCategoryBase)
				return (obj as RibbonPageCategoryBase).Pages;
			if (obj is RibbonPage)
				return (obj as RibbonPage).Groups;
			if (obj is RibbonPageGroup)
				return (obj as RibbonPageGroup).ItemLinks;
			return null;
		}
		protected internal static BarItemLinkBase CreateItemLink(string fullTypeName) {
			Type t = null;
			System.Reflection.Assembly entryAssembly = System.Reflection.Assembly.GetEntryAssembly();
			if (entryAssembly != null) {
				foreach (System.Reflection.AssemblyName asm in entryAssembly.GetReferencedAssemblies()) {
					t = System.Reflection.Assembly.Load(asm).GetType(fullTypeName);
					if (t != null)
						break;
				}
			} else {
				foreach (System.Reflection.Assembly asm in new System.Reflection.Assembly[] { typeof(Bar).Assembly, typeof(RibbonControl).Assembly }) {
					t = asm.GetType(fullTypeName);
					if (t != null)
						break;
				}
			}
			if (t == null)
				return null;
			BarItemLinkBase link = (BarItemLinkBase)t.GetConstructor(new Type[] { }).Invoke(new object[] { });
			return link;
		}
		#endregion        
	}
	public class UpDownArrowButton : Control {
		public static readonly DependencyProperty DownTemplateProperty =
			DependencyPropertyManager.Register("DownTemplate", typeof(ControlTemplate), typeof(UpDownArrowButton), new FrameworkPropertyMetadata(null, new PropertyChangedCallback((o, e) => { ((UpDownArrowButton)o).UpdateTemplate(); })));
		public static readonly DependencyProperty IsUpOrientedProperty =
			DependencyPropertyManager.Register("IsUpOriented", typeof(bool), typeof(UpDownArrowButton), new FrameworkPropertyMetadata(true, new PropertyChangedCallback((o, e) => { ((UpDownArrowButton)o).UpdateTemplate(); })));
		public static readonly DependencyProperty UpTemplateProperty =
			DependencyPropertyManager.Register("UpTemplate", typeof(ControlTemplate), typeof(UpDownArrowButton), new FrameworkPropertyMetadata(null, new PropertyChangedCallback((o, e) => { ((UpDownArrowButton)o).UpdateTemplate(); })));
		public UpDownArrowButton() {
			DefaultStyleKey = typeof(UpDownArrowButton);
			Loaded += new RoutedEventHandler(OnLoaded);
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			UpdateTemplate();
		}
		void UpdateTemplate() {
			Template = IsUpOriented ? UpTemplate : DownTemplate;
		}
		public ControlTemplate DownTemplate {
			get { return (ControlTemplate)GetValue(DownTemplateProperty); }
			set { SetValue(DownTemplateProperty, value); }
		}
		public bool IsUpOriented {
			get { return (bool)GetValue(IsUpOrientedProperty); }
			set { SetValue(IsUpOrientedProperty, value); }
		}
		public ControlTemplate UpTemplate {
			get { return (ControlTemplate)GetValue(UpTemplateProperty); }
			set { SetValue(UpTemplateProperty, value); }
		}
	}
	public enum CustomizationArrowControlDirection {
		Up, Down, Left, Right
	}
	public class CustomizationArrowControl : Control {
		public static readonly DependencyProperty DirectionProperty =
			DependencyPropertyManager.Register("Direction", typeof(CustomizationArrowControlDirection), typeof(CustomizationArrowControl), new FrameworkPropertyMetadata(CustomizationArrowControlDirection.Up, new PropertyChangedCallback(OnDirectionPropertyChanged)));
		public CustomizationArrowControl() {
			DefaultStyleKey = typeof(CustomizationArrowControl);
			Loaded += new RoutedEventHandler(OnLoaded);
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			VisualStateManager.GoToState(this, Direction.ToString(), false);
		}
		protected virtual void OnDirectionChanged(CustomizationArrowControlDirection oldValue) {
			VisualStateManager.GoToState(this, Direction.ToString(), false);
		}
		protected static void OnDirectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((CustomizationArrowControl)d).OnDirectionChanged((CustomizationArrowControlDirection)e.OldValue);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			VisualStateManager.GoToState(this, Direction.ToString(), false);
		}
		public CustomizationArrowControlDirection Direction {
			get { return (CustomizationArrowControlDirection)GetValue(DirectionProperty); }
			set { SetValue(DirectionProperty, value); }
		}
	}
	public class CustomizationSeparatorControl : Control {
		public static readonly DependencyProperty OrientationProperty =
			DependencyPropertyManager.Register("Orientation", typeof(Orientation), typeof(CustomizationSeparatorControl), new FrameworkPropertyMetadata(Orientation.Horizontal));
		public CustomizationSeparatorControl() {
			DefaultStyleKey = typeof(CustomizationSeparatorControl);
		}
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
	}
	public class ElementWrapper : DependencyObject {
		[IgnoreDependencyPropertiesConsistencyChecker]
		protected static readonly DependencyProperty InternalItemsProperty =
			DependencyPropertyManager.Register("InternalItems", typeof(IList), typeof(ElementWrapper), new FrameworkPropertyMetadata(null, (d, e) => ((ElementWrapper)d).OnInternalItemsChanged((IList)e.OldValue)));
		[IgnoreDependencyPropertiesConsistencyChecker]
		protected static readonly DependencyProperty TempItemsProperty =
			DependencyProperty.Register("TempItems", typeof(ObservableCollection<ElementWrapper>), typeof(ElementWrapper), new PropertyMetadata(null, (d, e) => ((ElementWrapper)d).OnTempItemsChanged((ObservableCollection<ElementWrapper>)e.OldValue)));
		public static readonly DependencyProperty CanChangeContentProperty =
			DependencyPropertyManager.Register("CanChangeContent", typeof(bool), typeof(ElementWrapper), new FrameworkPropertyMetadata(false, (d, e) => ((ElementWrapper)d).OnCanChangeContentChanged((bool)e.OldValue)));
		public static readonly DependencyProperty CanHideProperty =
   DependencyPropertyManager.Register("CanHide", typeof(bool), typeof(ElementWrapper), new FrameworkPropertyMetadata(false, (d, e) => ((ElementWrapper)d).OnCanHideChanged((bool)e.OldValue)));
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty CanMoveInternalProperty =
			DependencyPropertyManager.Register("CanMoveInternal", typeof(bool), typeof(ElementWrapper), new FrameworkPropertyMetadata(false, (d, e) => ((ElementWrapper)d).OnCanMoveInternalChanged((bool)e.OldValue)));
		public static readonly DependencyProperty ContentProperty =
   DependencyPropertyManager.Register("Content", typeof(object), typeof(ElementWrapper), new FrameworkPropertyMetadata(null, (d, e) => ((ElementWrapper)d).OnContentChanged((object)e.OldValue)));
		public static readonly DependencyProperty ContentTemplateProperty =
   DependencyPropertyManager.Register("ContentTemplate", typeof(DataTemplate), typeof(ElementWrapper), new FrameworkPropertyMetadata(null, (d, e) => ((ElementWrapper)d).OnContentTemplateChanged((DataTemplate)e.OldValue)));
		public static readonly DependencyProperty CreatedByCustomizationDialogProperty =
			DependencyProperty.Register("CreatedByCustomizationDialog", typeof(bool), typeof(ElementWrapper), new PropertyMetadata(false));
		public static readonly DependencyProperty GlyphProperty =
   DependencyPropertyManager.Register("Glyph", typeof(ImageSource), typeof(ElementWrapper), new FrameworkPropertyMetadata(null, (d, e) => ((ElementWrapper)d).OnGlyphChanged((ImageSource)e.OldValue)));
		public static readonly DependencyProperty GlyphTemplateProperty =
   DependencyPropertyManager.Register("GlyphTemplate", typeof(DataTemplate), typeof(ElementWrapper), new FrameworkPropertyMetadata(null, (d, e) => ((ElementWrapper)d).OnGlyphTemplateChanged((DataTemplate)e.OldValue)));
		public static readonly DependencyProperty HasGlyphProperty =
   DependencyPropertyManager.Register("HasGlyph", typeof(bool), typeof(ElementWrapper), new FrameworkPropertyMetadata(false, (d, e) => ((ElementWrapper)d).OnHasGlyphChanged((bool)e.OldValue)));
		public static readonly DependencyProperty HasGlyphTemplateProperty =
   DependencyPropertyManager.Register("HasGlyphTemplate", typeof(bool), typeof(ElementWrapper), new FrameworkPropertyMetadata(false, (d, e) => ((ElementWrapper)d).OnHasGlyphTemplateChanged((bool)e.OldValue)));
		public static readonly DependencyProperty IndexProperty =
			DependencyPropertyManager.Register("Index", typeof(int), typeof(ElementWrapper), new FrameworkPropertyMetadata(0, (d, e) => ((ElementWrapper)d).OnIndexChanged((int)e.OldValue)));
		public static readonly DependencyProperty IsDropDownProperty =
			DependencyPropertyManager.Register("IsDropDown", typeof(bool), typeof(ElementWrapper), new FrameworkPropertyMetadata(false));
		public static readonly DependencyProperty IsEditorProperty =
			DependencyPropertyManager.Register("IsEditor", typeof(bool), typeof(ElementWrapper), new FrameworkPropertyMetadata(false));
		public static readonly DependencyProperty IsMenuProperty =
			DependencyPropertyManager.Register("IsMenu", typeof(bool), typeof(ElementWrapper), new FrameworkPropertyMetadata(false));
		public static readonly DependencyProperty IsRemovedProperty =
			 DependencyPropertyManager.Register("IsRemoved", typeof(bool), typeof(ElementWrapper), new FrameworkPropertyMetadata(false, (d, e) => ((ElementWrapper)d).OnIsRemovedChanged((bool)e.OldValue)));
		public static readonly DependencyProperty IsVisibleProperty =
			DependencyPropertyManager.Register("IsVisible", typeof(bool), typeof(ElementWrapper), new FrameworkPropertyMetadata(true, (d, e) => ((ElementWrapper)d).OnIsVisibleChanged((bool)e.OldValue)));
		public static readonly DependencyProperty ItemsProperty =
			DependencyProperty.Register("Items", typeof(ListCollectionView), typeof(ElementWrapper), new PropertyMetadata(null));
		ICommand addCommand;
		ICommand createNewCommand;
		ICommand moveDownCommand;
		ICommand moveUpCommand;
		ICommand removeCommand;
		ICommand renameCommand;
		IRuntimeCustomizationHost customizationHost;
		DependencyObject ownerObject;
		ElementWrapper parentWrapper;
		bool skipCustomizations;
		public ElementWrapper(DependencyObject ownerObject, ElementWrapper parentWrapper, bool skipCustomizations, IRuntimeCustomizationHost customizationHost) {
			this.ownerObject = ownerObject;
			this.parentWrapper = parentWrapper;
			this.skipCustomizations = skipCustomizations;
			this.customizationHost = customizationHost;
			this.TempItems = new ObservableCollection<ElementWrapper>();
			this.SortItems = true;
		}
		public void Reset() {
			ResetOverride();
		}
		protected virtual void ResetOverride() {
			TempItems.Clear();
		}
		void Remove() {
			if (!this.CreatedByCustomizationDialog) {
				bool match = false;
				foreach (ElementWrapper element in ParentWrapper.Items) {
					if (element == this) {
						match = true;
						continue;
					}
					if (!match)
						continue;
					element.Index--;
					AddCustomization(new RuntimePropertyCustomization(element.OwnerObject) {
						PropertyName = GetIndexPropertyName(),
						NewValue = element.Index,
						OldValue = element.Index + 1,
					}.Do(x => x.AffectedTargets.AddRange(GetAffectedTargetsOnRemove())));
				}
				AddCustomization(new RuntimePropertyCustomization(OwnerObject) {
					PropertyName = "IsRemoved",
					NewValue = true
				}.Do(x => x.AffectedTargets.AddRange(GetAffectedTargetsOnRemove())));				
				IsRemoved = true;
				ParentWrapper.Items.Do(x => x.Refresh());
				return;
			}
			ParentWrapper.RemoveChild(this);
		}
		protected virtual IEnumerable<string> GetAffectedTargetsOnRemove() { return GetParentSerializationNames(); }
		protected virtual IEnumerable<string> GetParentSerializationNames() {
			if (ParentWrapper != null)
				foreach (var name in ParentWrapper.GetParentSerializationNames())
					yield return name;
			yield return OwnerObject.With(BarManagerCustomizationHelper.GetSerializationName);
		}
		protected void AddCustomization(RuntimeCustomization customization) {
			if (CustomizationHost != null)
				CustomizationHost.RuntimeCustomizations.Add(customization);
		}
		protected virtual bool CanAdd(ElementWrapper second) { return false; }
		protected virtual bool CanCreateNew(string param) { return false; }
		protected virtual bool CanMoveChildDown(ElementWrapper elementWrapper) { return elementWrapper.Index < Items.Count - 1; }
		protected virtual bool CanMoveChildUp(ElementWrapper elementWrapper) { return elementWrapper.Index > 0; }
		protected virtual bool CanMoveDown() {
			if (!CanMoveInternal || ParentWrapper == null)
				return false;
			return ParentWrapper.CanMoveChildDown(this);
		}
		protected virtual bool CanMoveUp() {
			if (!CanMoveInternal || ParentWrapper == null)
				return false;
			return ParentWrapper.CanMoveChildUp(this);
		}
		protected virtual bool CanRemove() { return false; }
		protected virtual bool CanRename() { return CanChangeContent; }
		protected virtual void CreateNew(string param) { }
		protected bool GetCaption(ref string result) {
			var captionEditor = new ToolbarCaptionEditor() { ToolbarCaption = result };
			try {
				return new DXDialog() {
					Content = captionEditor,
					SizeToContent = SizeToContent.Height,
					Width = 400,
					WindowStartupLocation = WindowStartupLocation.CenterOwner,
					Owner = Window.GetWindow(this),
					Title = BarsLocalizer.GetString(BarsStringId.ToolbarsCustomizationControl_EditorWindowCaption)
				}.ShowDialog(MessageBoxButton.OKCancel) == MessageBoxResult.OK;
			} finally {
				result = captionEditor.ToolbarCaption;
			}
		}
		protected virtual string GetCaptionPropertyName() { return "Caption"; }
		protected virtual string GetIndexPropertyName() { return "Index"; }
		protected virtual void MoveChild(ElementWrapper elementWrapper, bool up) {
			var ordered = Items.OfType<ElementWrapper>().ToList();
			var childIndex = ordered.IndexOf(elementWrapper);
			ElementWrapper right = null;
			ElementWrapper left = null;
			if (up) {
				right = elementWrapper;
				left = ((ElementWrapper)ordered[childIndex - 1]);
			} else {
				left = elementWrapper;
				right = ((ElementWrapper)ordered[childIndex + 1]);
			}
			left.Index++;
			right.Index--;
			AddCustomization(new RuntimePropertyCustomization(left.OwnerObject) {
				PropertyName = GetIndexPropertyName(),
				NewValue = left.Index,
				OldValue = left.Index - 1
			}.Do(x => x.AffectedTargets.AddRange(GetAffectedTargetsOnMoveChild(left))));
			AddCustomization(new RuntimePropertyCustomization(right.OwnerObject) {
				PropertyName = GetIndexPropertyName(),
				NewValue = right.Index,
				OldValue = right.Index + 1,
			}.Do(x => x.AffectedTargets.AddRange(GetAffectedTargetsOnMoveChild(right))));
			Items.Do(x => x.Refresh());
		}
		protected virtual IEnumerable<string> GetAffectedTargetsOnMoveChild(ElementWrapper elementWrapper) {
			foreach (var name in GetParentSerializationNames())
				yield return name;
			yield return elementWrapper.OwnerObject.With(BarManagerCustomizationHelper.GetSerializationName);
		}
		protected virtual void MoveChildDown(ElementWrapper elementWrapper) {
			MoveChild(elementWrapper, false);
		}
		protected virtual void MoveChildUp(ElementWrapper elementWrapper) {
			MoveChild(elementWrapper, true);
		}
		protected virtual void MoveDown() { ParentWrapper.MoveChildDown(this); }
		protected virtual void MoveUp() { ParentWrapper.MoveChildUp(this); }
		protected virtual void OnCanChangeContentChanged(bool oldValue) { }
		protected virtual void OnCanHideChanged(bool oldValue) { }
		protected virtual void OnCanMoveInternalChanged(bool oldValue) { CommandManager.InvalidateRequerySuggested(); }
		protected virtual void OnContentChanged(object oldValue) { }
		protected virtual void OnContentTemplateChanged(DataTemplate oldValue) { }
		protected virtual void OnGlyphChanged(ImageSource oldValue) { }
		protected virtual void OnGlyphTemplateChanged(DataTemplate oldValue) { }
		protected virtual void OnHasGlyphChanged(bool oldValue) { }
		protected virtual void OnHasGlyphTemplateChanged(bool oldValue) { }
		protected virtual void OnIndexChanged(int oldValue) { CommandManager.InvalidateRequerySuggested(); }
		protected virtual void OnInternalItemsChanged(IList oldValue) {
			UpdateItems();
			var oValue = oldValue as INotifyCollectionChanged;
			var nValue = InternalItems as INotifyCollectionChanged;
			if (oValue != null)
				oValue.CollectionChanged -= OnInternalItemsCollectionChanged;
			if (nValue != null)
				nValue.CollectionChanged += OnInternalItemsCollectionChanged;
		}
		protected virtual void OnInternalItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			Items.Do(x => x.Refresh());
		}
		protected virtual void OnIsRemovedChanged(bool oldValue) {
			ParentWrapper.Do(x => x.Items.Refresh());
		}
		protected virtual void OnIsVisibleChanged(bool oldValue) { }
		protected virtual void OnTempItemsChanged(ObservableCollection<ElementWrapper> oldValue) {
			UpdateItems();
			if (oldValue != null)
				oldValue.CollectionChanged -= OnTempItemsCollectionChanged;
			if (TempItems != null)
				TempItems.CollectionChanged += OnTempItemsCollectionChanged;
		}
		protected virtual void OnTempItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			Items.Do(x => x.Refresh());
		}
		protected virtual void RemoveChild(ElementWrapper child) {
			var elementIndex = 0;
			foreach (ElementWrapper parentElement in ParentWrapper.Items) {
				if ((parentElement as ElementWrapper).InternalItems.Contains(child)) {
					elementIndex = parentElement.Index;
					break;
				}
			}
			var element = ParentWrapper.Items.GetItemAt(elementIndex);
			if (child.OwnerObject is RibbonPage)
				AddCustomization(new RuntimeRemoveRibbonPageCustomization(child.OwnerObject as RibbonPage).Do(x => x.AffectedTargets.AddRange(GetAffectedTargetsOnRemove())));
			if (child.OwnerObject is RibbonPageGroup)
				AddCustomization(new RuntimeRemoveRibbonPageGroupCustomization(child.OwnerObject as RibbonPageGroup).Do(x => x.AffectedTargets.AddRange(GetAffectedTargetsOnRemove())));
			if (child.OwnerObject is BarItemLink)
				AddCustomization(new RuntimeRemoveLinkCustomization(child.OwnerObject as BarItemLink).Do(x => x.AffectedTargets.AddRange(GetAffectedTargetsOnRemove())));
			child.IsRemoved = true;
			for (int i = child.Index + 1; i < (element as ElementWrapper).Items.Count; i++) {
				var item = (element as ElementWrapper).Items.GetItemAt(i) as ElementWrapper;
				AddCustomization(new RuntimePropertyCustomization(item.OwnerObject) {
					PropertyName = GetIndexPropertyName(),
					NewValue = item.Index--,
					OldValue = item.Index + 1,
				}.Do(x => x.AffectedTargets.AddRange(GetAffectedTargetsOnRemove())));
				(element as ElementWrapper).InternalItems.Remove(child);
				ParentWrapper.Items.Do(x => x.Refresh());
			}
		}
		protected virtual void Rename() {
			string newCaption = Convert.ToString(Content);
			string oldCaption = newCaption;
			if (!GetCaption(ref newCaption))
				return;
			this.Content = newCaption;
			AddCustomization(new RuntimePropertyCustomization(OwnerObject) {
				PropertyName = GetCaptionPropertyName(),
				NewValue = newCaption,
				OldValue = oldCaption,
				Overwrite = true,
			}.Do(x => x.AffectedTargets.AddRange(GetAffectedTargetsOnRename())));
		}
		protected virtual IEnumerable<string> GetAffectedTargetsOnRename() { return GetParentSerializationNames(); }
		protected virtual void UpdateItems() {
			if (SkipCustomizations) {
				if (InternalItems != null) {
					Items = new ListCollectionView(InternalItems);
					Items.Filter = x => !((ElementWrapper)x).CreatedByCustomizationDialog;
				}
				return;
			}
			CompositeCollection cCol = new CompositeCollection();
			cCol.Add(new CollectionContainer() { Collection = InternalItems });
			cCol.Add(new CollectionContainer() { Collection = TempItems });
			var wrapped = new ObservableCollectionConverter<object, ElementWrapper>() { Source = ((ICollectionViewFactory)cCol).CreateView(), Selector = x => (ElementWrapper)x };
			ListCollectionView lcv = new ListCollectionView(wrapped);
			if (SortItems) {
				SortDescription sort = new SortDescription("Index", ListSortDirection.Ascending);
				lcv.SortDescriptions.Add(sort);
			}
			lcv.Filter = x => !((ElementWrapper)x).IsRemoved;
			Items = lcv;
		}
		protected bool CanMoveInternal {
			get { return (bool)GetValue(CanMoveInternalProperty); }
			set { SetValue(CanMoveInternalProperty, value); }
		}
		protected IRuntimeCustomizationHost CustomizationHost { get { return customizationHost; } }
		protected IList InternalItems {
			get { return (IList)GetValue(InternalItemsProperty); }
			set { SetValue(InternalItemsProperty, value); }
		}
		protected bool SortItems { get; set; }
		protected ObservableCollection<ElementWrapper> TempItems {
			get { return (ObservableCollection<ElementWrapper>)GetValue(TempItemsProperty); }
			set { SetValue(TempItemsProperty, value); }
		}
		public virtual void Add(ElementWrapper second) { }
		public ICommand AddCommand { get { return addCommand ?? (addCommand = new DelegateCommand<ElementWrapper>(Add, CanAdd, true)); } }
		public bool CanChangeContent {
			get { return (bool)GetValue(CanChangeContentProperty); }
			set { SetValue(CanChangeContentProperty, value); }
		}
		public bool CanHide {
			get { return (bool)GetValue(CanHideProperty); }
			set { SetValue(CanHideProperty, value); }
		}
		public object Content {
			get { return (object)GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
		public DataTemplate ContentTemplate {
			get { return (DataTemplate)GetValue(ContentTemplateProperty); }
			set { SetValue(ContentTemplateProperty, value); }
		}
		public bool CreatedByCustomizationDialog {
			get { return (bool)GetValue(CreatedByCustomizationDialogProperty); }
			set { SetValue(CreatedByCustomizationDialogProperty, value); }
		}
		public ICommand CreateNewCommand { get { return createNewCommand ?? (createNewCommand = new DelegateCommand<string>(CreateNew, CanCreateNew, true)); } }
		public ImageSource Glyph {
			get { return (ImageSource)GetValue(GlyphProperty); }
			set { SetValue(GlyphProperty, value); }
		}
		public DataTemplate GlyphTemplate {
			get { return (DataTemplate)GetValue(GlyphTemplateProperty); }
			set { SetValue(GlyphTemplateProperty, value); }
		}
		public bool HasGlyph {
			get { return (bool)GetValue(HasGlyphProperty); }
			set { SetValue(HasGlyphProperty, value); }
		}
		public bool HasGlyphTemplate {
			get { return (bool)GetValue(HasGlyphTemplateProperty); }
			set { SetValue(HasGlyphTemplateProperty, value); }
		}
		public int Index {
			get { return (int)GetValue(IndexProperty); }
			set { SetValue(IndexProperty, value); }
		}
		public bool IsDropDown {
			get { return (bool)GetValue(IsDropDownProperty); }
			set { SetValue(IsDropDownProperty, value); }
		}
		public bool IsEditor {
			get { return (bool)GetValue(IsEditorProperty); }
			set { SetValue(IsEditorProperty, value); }
		}
		public bool IsMenu {
			get { return (bool)GetValue(IsMenuProperty); }
			set { SetValue(IsMenuProperty, value); }
		}
		public bool IsRemoved {
			get { return (bool)GetValue(IsRemovedProperty); }
			set { SetValue(IsRemovedProperty, value); }
		}
		public bool IsVisible {
			get { return (bool)GetValue(IsVisibleProperty); }
			set { SetValue(IsVisibleProperty, value); }
		}
		public ListCollectionView Items {
			get { return (ListCollectionView)GetValue(ItemsProperty); }
			set { SetValue(ItemsProperty, value); }
		}
		public ICommand MoveDownCommand { get { return moveDownCommand ?? (moveDownCommand = new DelegateCommand(MoveDown, CanMoveDown, true)); } }
		public ICommand MoveUpCommand { get { return moveUpCommand ?? (moveUpCommand = new DelegateCommand(MoveUp, CanMoveUp, true)); } }
		public DependencyObject OwnerObject { get { return ownerObject; } }
		public ElementWrapper ParentWrapper { get { return parentWrapper; } }
		public ICommand RemoveCommand { get { return removeCommand ?? (removeCommand = new DelegateCommand(Remove, CanRemove, true)); } }
		public ICommand RenameCommand { get { return renameCommand ?? (renameCommand = new DelegateCommand(Rename, CanRename, true)); } }
		public bool SkipCustomizations { get { return skipCustomizations; } }
	}
	public class RibbonElementWrapper : ElementWrapper {
		RibbonControl owner;
		bool showMainTabs = true;
		bool showToolTabs = true;
		public RibbonElementWrapper(RibbonControl owner) : this(owner, false) { }
		public RibbonElementWrapper(RibbonControl owner, bool skipCustomizations) : this(owner, skipCustomizations, true, true) { }
		public RibbonElementWrapper(RibbonControl owner, bool skipCustomizations, bool showMainTabs, bool showToolTabs)
			: base(owner, null, skipCustomizations, owner) {
			this.owner = owner;
			Reset();
		}
		protected override void ResetOverride() {
			CreatedByCustomizationDialog = false;
			SortItems = false;
			InternalItems = (IList)new ObservableCollectionConverter<RibbonPageCategoryBase, RibbonPageCategoryElementWrapper>() { Source = owner.Categories, Selector = x => new RibbonPageCategoryElementWrapper(x, this, SkipCustomizations, showMainTabs, showToolTabs, CustomizationHost) };
			if (SkipCustomizations) {
				this.CanChangeContent = false;
				this.CanHide = false;
				this.CanMoveInternal = false;
				this.Content = null;
				this.ContentTemplate = null;
				this.Glyph = null;
				this.GlyphTemplate = null;
				this.HasGlyph = false;
				this.HasGlyphTemplate = false;
				this.Index = 0;
				this.IsDropDown = false;
				this.IsEditor = false;
				this.IsMenu = false;
				this.IsVisible = true;
			}
		}
		protected void UpdateTabsVisibility() {
			foreach (RibbonPageCategoryElementWrapper element in InternalItems) {
				if (element.Owner.IsDefault) {
					element.IsRemoved = !ShowMainTabs;
				} else {
					element.IsRemoved = !ShowToolTabs;
				}
			}
		}
		public bool ShowMainTabs {
			get { return showMainTabs; }
			set {
				if (value == showMainTabs)
					return;
				bool oldValue = showMainTabs;
				showMainTabs = value;
				UpdateTabsVisibility();
			}
		}
		public bool ShowToolTabs {
			get { return showToolTabs; }
			set {
				if (value == showToolTabs)
					return;
				bool oldValue = showToolTabs;
				showToolTabs = value;
				UpdateTabsVisibility();
			}
		}
	}
	public class RibbonPageCategoryElementWrapper : ElementWrapper {
		const string namePrefix = "guidD45041CD530241E8B0B3BA7794F805C2";
		static int index;
		RibbonPageCategoryBase owner;
		RibbonElementWrapper parent;
		public RibbonPageCategoryElementWrapper(RibbonPageCategoryBase owner, RibbonElementWrapper parent, IRuntimeCustomizationHost customizationHost) : this(owner, parent, false, customizationHost) { }
		public RibbonPageCategoryElementWrapper(RibbonPageCategoryBase owner, RibbonElementWrapper parent, bool skipCustomizations, IRuntimeCustomizationHost customizationHost) : this(owner, parent, skipCustomizations, true, true, customizationHost) { }
		bool showMainTabs;
		bool showToolTabs;
		public RibbonPageCategoryElementWrapper(RibbonPageCategoryBase owner, RibbonElementWrapper parent, bool skipCustomizations, bool showMainTabs, bool showToolTabs, IRuntimeCustomizationHost customizationHost)
			: base(owner, parent, skipCustomizations, customizationHost) {
			this.owner = owner;
			this.parent = parent;
			this.showMainTabs = showMainTabs;
			this.showToolTabs = showToolTabs;
			CreatedByCustomizationDialog = false;
			Reset();
		}
		protected override void ResetOverride() {
			base.ResetOverride();
			InternalItems = (IList)new ObservableCollectionConverter<RibbonPage, RibbonPageElementWrapper>() { Source = owner.Pages, Selector = x => new RibbonPageElementWrapper(x, this, SkipCustomizations, CustomizationHost) };
			this.CanChangeContent = false;
			this.CanHide = false;
			this.CanMoveInternal = false;
			if (!owner.IsDefault)
				this.Content = owner.Caption;
			else
				Content = "Default category";
			this.ContentTemplate = null;
			this.Glyph = null;
			this.GlyphTemplate = null;
			this.HasGlyph = false;
			this.HasGlyphTemplate = false;
			this.Index = 0;
			this.IsDropDown = false;
			this.IsEditor = false;
			this.IsMenu = false;
			this.IsVisible = true;
			if (!showMainTabs && owner.IsDefault)
				IsRemoved = true;
			if (!showToolTabs && owner.IsDefault)
				IsRemoved = true;
		}
		protected virtual IEnumerable<string> GetAffectedTargetsOnCreateNewPage(RibbonPage page) {
			foreach (var name in GetParentSerializationNames())
				yield return name;
			yield return page.With(BarManagerCustomizationHelper.GetSerializationName);
		}
		protected virtual IEnumerable<string> GetAffectedTargetsOnClonePage(RibbonPage page) {
			foreach (var name in GetParentSerializationNames())
				yield return name;
			yield return page.With(BarManagerCustomizationHelper.GetSerializationName);
		}
		protected override bool CanAdd(ElementWrapper second) { return second is RibbonPageElementWrapper; }
		protected override bool CanCreateNew(string param) { return String.Equals("Page", param); }
		protected override bool CanRemove() { return false; }
		protected override void CreateNew(string param) {
			var page = new RibbonPage() { CreatedByCustomizationDialog = true, Caption = "New Page", Name = namePrefix + index++, Index = Items.Count };
			var newWrapper = new RibbonPageElementWrapper(page, this, false, CustomizationHost);
			newWrapper.RenameCommand.Execute(null);
			TempItems.Add(newWrapper);
			AddCustomization(new CreateNewPageCustomization(page, Owner).Do(x => x.AffectedTargets.AddRange(GetAffectedTargetsOnCreateNewPage(page))));
		}
		public override void Add(ElementWrapper second) {
			var pGroup = second.OwnerObject as RibbonPage;
			var customization = new ClonePageCustomization(pGroup, OwnerObject as RibbonPageCategoryBase, Items.OfType<ElementWrapper>().LastOrDefault().With(x => x.OwnerObject as RibbonPage));
			var clone = customization.CreateClone();
			clone.Index = Items.Count;
			AddCustomization(customization.Do(x => x.AffectedTargets.AddRange(GetAffectedTargetsOnClonePage(clone))));
			var newWrapper = new RibbonPageElementWrapper(clone, this, CustomizationHost);
			TempItems.Add(newWrapper);
			foreach (ElementWrapper element in second.Items)
				newWrapper.Add(element);
		}
		public RibbonPageCategoryBase Owner { get { return owner; } }
		public RibbonElementWrapper Parent { get { return parent; } }
	}
	public class RibbonPageElementWrapper : ElementWrapper {
		const string namePrefix = "guidD39BDA9E7694420A93EA7E0A1DB90F36";
		static int index;
		readonly Locker initializationLocker;
		private RibbonPage owner;
		RibbonPageCategoryElementWrapper parent;
		public RibbonPageElementWrapper(RibbonPage owner, RibbonPageCategoryElementWrapper parent, IRuntimeCustomizationHost customizationHost) : this(owner, parent, false, customizationHost) { }
		public RibbonPageElementWrapper(RibbonPage owner, RibbonPageCategoryElementWrapper parent, bool skipCustomizations, IRuntimeCustomizationHost customizationHost) : base(owner, parent, skipCustomizations, customizationHost) {
			this.owner = owner;
			this.parent = parent;
			CreatedByCustomizationDialog = owner.CreatedByCustomizationDialog;
			initializationLocker = new Locker();
			Reset();
		}
		protected override void ResetOverride() {
			base.ResetOverride();
			using (initializationLocker.Lock()) {
				InternalItems = (IList)new ObservableCollectionConverter<RibbonPageGroup, RibbonPageGroupElementWrapper>() { Source = owner.Groups, Selector = x => new RibbonPageGroupElementWrapper(x, this, SkipCustomizations, CustomizationHost) };
				this.CanChangeContent = false;
				this.CanHide = false;
				this.CanMoveInternal = false;
				this.Content = owner.Caption;
				this.ContentTemplate = null;
				this.Glyph = null;
				this.GlyphTemplate = null;
				this.HasGlyph = false;
				this.HasGlyphTemplate = false;
				this.Index = 0;
				this.IsDropDown = false;
				this.IsEditor = false;
				this.IsMenu = false;
				this.IsVisible = true;
				if (!SkipCustomizations) {
					this.CanChangeContent = true;
					this.CanHide = true;
					this.CanMoveInternal = true;
					this.Content = owner.Caption;
					this.Index = owner.Index;
					this.IsRemoved = owner.IsRemoved;
					this.IsVisible = owner.IsVisible;
				}
			}
		}
		bool IsInitializing { get { return initializationLocker.IsLocked; } }
		protected override bool CanAdd(ElementWrapper second) { return second is RibbonPageGroupElementWrapper; }
		protected override bool CanCreateNew(string param) { return String.Equals("PageGroup", param); }
		protected override bool CanRemove() { return true; }
		protected override void CreateNew(string param) {
			var group = new RibbonPageGroup() { CreatedByCustomizationDialog = true, Caption = "New Group", Name = namePrefix + index++, Index = Items.Count };
			var newWrapper = new RibbonPageGroupElementWrapper(group, this, false, CustomizationHost);
			newWrapper.RenameCommand.Execute(null);
			TempItems.Add(newWrapper);
			AddCustomization(new CreateNewGroupCustomization(group, OwnerObject as RibbonPage).Do(x => x.AffectedTargets.AddRange(GetAffectedTargetsOnCloneGroup(group))));
		}
		protected virtual IEnumerable<string> GetAffectedTargetsOnCloneGroup(RibbonPageGroup group) {
			foreach (var name in GetParentSerializationNames())
				yield return name;
			yield return group.With(BarManagerCustomizationHelper.GetSerializationName);
		}
		protected virtual IEnumerable<string> GetAffectedTargetsOnIsVisibleChanged() {
			return GetParentSerializationNames();
		}
		protected virtual IEnumerable<string> GetAffectedTargetsOnAddGroup(RibbonPageGroup clone) {
			foreach (var name in GetParentSerializationNames())
				yield return name;
			yield return clone.With(BarManagerCustomizationHelper.GetSerializationName);
		}
		protected override string GetCaptionPropertyName() { return RibbonPage.CaptionProperty.Name; }
		protected override void OnIsVisibleChanged(bool oldValue) {
			base.OnIsVisibleChanged(oldValue);
			if (IsInitializing)
				return;
			AddCustomization(new RuntimePropertyCustomization(OwnerObject) {
				PropertyName = RibbonPage.IsVisibleProperty.Name,
				OldValue = oldValue,
				NewValue = IsVisible,
				Overwrite = true
			}.Do(x => x.AffectedTargets.AddRange(GetAffectedTargetsOnIsVisibleChanged())));
		}
		protected override void RemoveChild(ElementWrapper child) {
			base.RemoveChild(child);
		}
		public override void Add(ElementWrapper second) {
			var pGroup = second.OwnerObject as RibbonPageGroup;
			var customization = new CloneGroupCustomization(pGroup, OwnerObject as RibbonPage, Items.OfType<ElementWrapper>().LastOrDefault().With(x => x.OwnerObject as RibbonPageGroup));
			var clone = customization.CreateClone();
			clone.Index = Items.Count;
			AddCustomization(customization.Do(x => x.AffectedTargets.AddRange(GetAffectedTargetsOnAddGroup(clone))));
			var newWrapper = new RibbonPageGroupElementWrapper(clone, this, CustomizationHost);
			TempItems.Add(newWrapper);
			foreach (ElementWrapper element in second.Items)
				newWrapper.Add(element);
		}
		public RibbonPageCategoryElementWrapper Parent { get { return parent; } }
	}
	public class RibbonPageGroupElementWrapper : ElementWrapper {
		RibbonPageGroup owner;
		RibbonPageElementWrapper parent;
		public RibbonPageGroupElementWrapper(RibbonPageGroup owner, RibbonPageElementWrapper parent, IRuntimeCustomizationHost customizationHost) : this(owner, parent, false, customizationHost) { }
		public RibbonPageGroupElementWrapper(RibbonPageGroup owner, RibbonPageElementWrapper parent, bool skipCustomizations, IRuntimeCustomizationHost customizationHost)
			: base(owner, parent, skipCustomizations, customizationHost) {
			this.owner = owner;
			this.parent = parent;
			CreatedByCustomizationDialog = owner.CreatedByCustomizationDialog;
			Reset();
		}
		protected override void ResetOverride() {
			base.ResetOverride();
			InternalItems = (IList)new ObservableCollectionConverter<BarItemLinkBase, BarItemLinkElementWrapper>() { Source = owner.ItemLinks, Selector = x => new BarItemLinkElementWrapper((BarItemLink)x, this, SkipCustomizations, CustomizationHost) };
			this.CanChangeContent = false;
			this.CanHide = false;
			this.CanMoveInternal = false;
			this.Content = owner.Caption;
			this.ContentTemplate = null;
			this.Glyph = null;
			this.GlyphTemplate = null;
			this.HasGlyph = false;
			this.HasGlyphTemplate = false;
			this.Index = 0;
			this.IsDropDown = false;
			this.IsEditor = false;
			this.IsMenu = false;
			this.IsVisible = true;
			if (!SkipCustomizations) {
				this.CanChangeContent = true;
				this.CanMoveInternal = true;
				this.Content = owner.Caption;
				this.Content = owner.Caption;
				this.Index = owner.Index;
				this.IsRemoved = owner.IsRemoved;
			}
		}
		protected override bool CanAdd(ElementWrapper second) { return second is BarItemElementWrapper || second is BarItemLinkElementWrapper; }
		protected override bool CanRemove() { return true; }
		protected override string GetCaptionPropertyName() { return RibbonPageGroup.CaptionProperty.Name; }
		public override void Add(ElementWrapper second) {
			Add(second.OwnerObject as IBarItem);
		}
		public void Add(IBarItem bItem) {
			if (bItem == null)
				return;
			var cli = new RuntimeCopyLinkCustomization(
				bItem,
				OwnerObject as RibbonPageGroup,
				Items.OfType<ElementWrapper>().LastOrDefault(x => x.OwnerObject is BarItemLink).With(x => x.OwnerObject) as BarItemLink,
				true);
			var clone = cli.CreateLinkClone();
			clone.Index = Items.Count;
			TempItems.Add(new BarItemLinkElementWrapper(clone, this, CustomizationHost));
			AddCustomization(cli.Do(x => x.AffectedTargets.AddRange(GetAffectedTargetsOnAddItem(clone))));
		}
		protected virtual IEnumerable<string> GetAffectedTargetsOnAddItem(BarItemLink clone) {
			foreach (var name in GetParentSerializationNames())
				yield return name;
			yield return clone.With(BarManagerCustomizationHelper.GetSerializationName);
		}
		public RibbonPageElementWrapper Parent { get { return parent; } }
	}
	public class BarItemLinkElementWrapper : ElementWrapper {
		BarItemLinkBase owner;
		ElementWrapper parent;
		public BarItemLinkElementWrapper(BarItemLink owner, ElementWrapper parent, IRuntimeCustomizationHost customizationHost) : this(owner, parent, false, customizationHost) { }
		public BarItemLinkElementWrapper(BarItemLink owner, ElementWrapper parent, bool skipCustomizations, IRuntimeCustomizationHost customizationHost)
			: base(owner, parent, skipCustomizations, customizationHost) {
			this.owner = owner;
			this.parent = parent;
			CreatedByCustomizationDialog = owner.CreatedByCustomizationDialog;
			Reset();
		}
		protected override void ResetOverride() {
			base.ResetOverride();
			var owner = OwnerObject as BarItemLink;
			var holder = owner.Item as ILinksHolder;
			if (holder != null)
				InternalItems = (IList)new ObservableCollectionConverter<BarItemLinkBase, BarItemLinkElementWrapper>() { Source = holder.Links, Selector = x => new BarItemLinkElementWrapper((BarItemLink)x, this, SkipCustomizations, CustomizationHost) };
			var item = owner.Item;
			if (item == null)
				return;
			this.CanChangeContent = false;
			this.CanHide = false;
			this.CanMoveInternal = false;
			this.Content = item.Content;
			if (Content == null) {
				if (item is BarButtonGroup)
					Content = "Bar Button Group";
			}
			this.ContentTemplate = item.ContentTemplate;
			this.Glyph = item.Glyph ?? BarItemElementWrapper.DefaultGlyph;
			this.GlyphTemplate = item.GlyphTemplate;
			this.HasGlyph = GlyphTemplate == null;
			this.HasGlyphTemplate = GlyphTemplate != null;
			this.Index = 0;
			this.IsDropDown = item is BarSplitButtonItem || item is BarSplitCheckItem;
			this.IsEditor = item is BarEditItem;
			this.IsMenu = item is BarSubItem;
			this.IsVisible = true;
			if (!SkipCustomizations) {
				this.CanChangeContent = true;
				this.CanMoveInternal = Parent is RibbonPageGroupElementWrapper;
				this.Index = 0;
				this.Content = owner.UserContent.With(Convert.ToString).IfNot(String.IsNullOrEmpty) ?? item.Content;
				this.Index = owner.Index;
				this.IsRemoved = owner.IsRemoved;
			}
		}
		protected override bool CanRemove() { return ParentGroup != null; }
		protected override string GetCaptionPropertyName() { return BarItemLink.UserContentProperty.Name; }
		public ElementWrapper Parent { get { return parent; } }
		public ElementWrapper ParentGroup { get { return parent as RibbonPageGroupElementWrapper; } }
	}
	public class BarItemElementWrapper : ElementWrapper {
		private BarItem item;
		static BarItemElementWrapper() {
			DefaultGlyph = ReflectionHelper.CreateInstanceMethodHandler<BarItemLinkControl, Func<ImageSource>>(null, "get_DefaultGlyph", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)();
		}
		public BarItemElementWrapper(BarItem item)
			: base(item, null, true, null) {
			this.item = item;
			this.CanChangeContent = false;
			this.CanHide = false;
			this.CanMoveInternal = false;
			this.Content = item.Content;
			if (Content == null) {
				if (item is BarButtonGroup)
					Content = "Bar Button Group";
			}
			this.ContentTemplate = item.ContentTemplate;
			this.Glyph = item.Glyph ?? BarItemElementWrapper.DefaultGlyph;
			this.GlyphTemplate = item.GlyphTemplate;
			this.HasGlyph = GlyphTemplate == null;
			this.HasGlyphTemplate = GlyphTemplate != null;
			this.Index = 0;
			this.IsDropDown = item is BarSplitButtonItem || item is BarSplitCheckItem;
			this.IsEditor = item is BarEditItem;
			this.IsMenu = item is BarSubItem;
			this.IsVisible = true;
		}
		public static ImageSource DefaultGlyph { get; private set; }
	}
	class DelegateConverter : IValueConverter {
		Func<object, Type, object, System.Globalization.CultureInfo, object> convert;
		Func<object, Type, object, System.Globalization.CultureInfo, object> convertBack;
		public DelegateConverter(Func<object, Type, object, System.Globalization.CultureInfo, object> convert) : this(convert, null) { }
		public DelegateConverter(Func<object, Type, object, System.Globalization.CultureInfo, object> convert, Func<object, Type, object, System.Globalization.CultureInfo, object> convertBack) {
			this.convert = convert;
			this.convertBack = convertBack;
		}
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if (convert == null)
				return value;
			return convert(value, targetType, parameter, culture);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if (convertBack == null)
				return value;
			return convertBack(value, targetType, parameter, culture);
		}
	}
	public class RibbonControlSerializationStrategy : RuntimeCustomizationHostSerializationStrategy<RibbonControl> {
		static RibbonControlSerializationStrategy() {
			RuntimeCustomization.RegisterCustomization(() => new CreateNewPageCustomization());
			RuntimeCustomization.RegisterCustomization(() => new ClonePageCustomization());
			RuntimeCustomization.RegisterCustomization(() => new CloneGroupCustomization());
			RuntimeCustomization.RegisterCustomization(() => new CreateNewGroupCustomization());
			RuntimeCustomization.RegisterCustomization(() => new RuntimeRemoveRibbonPageCustomization());
			RuntimeCustomization.RegisterCustomization(() => new RuntimeRemoveRibbonPageGroupCustomization());
		}
		public RibbonControlSerializationStrategy(RibbonControl owner) : base(owner) { }
	}
	public class CreateNewPageCustomization : RuntimeCollectionCustomization {
		public CreateNewPageCustomization() : this(null, null) { }
		public CreateNewPageCustomization(RibbonPage target, RibbonPageCategoryBase container) : base(target, container) { }
		protected override void ApplyOverride() {
			var category = FindContainer() as RibbonPageCategoryBase;
			var page = FindTarget() as RibbonPage;
			if (page == null) {
				page = new RibbonPage();
				page.Name = TargetName;
				page.Caption = PageCaption;
				page.CreatedByCustomizationDialog = true;
			} else {
				PageCaption = Convert.ToString(page.Caption);
			}
			if (category == null || page == null)
				return;
			category.Pages.Add(page);
		}
		protected override void UndoOverride() {
			var category = FindContainer() as RibbonPageCategoryBase;
			var page = FindTarget() as RibbonPage;
			if (category == null || page == null)
				return;
			category.Pages.Remove(page);
		}
		[XtraSerializableProperty]
		public string PageCaption { get; set; }
	}
	public class ClonePageCustomization : RuntimeCollectionCustomization {
		RibbonPage clone;
		public ClonePageCustomization() : this(null, null) { }
		public ClonePageCustomization(RibbonPage source, RibbonPageCategoryBase container) : this(source, container, null) { }
		public ClonePageCustomization(RibbonPage source, RibbonPageCategoryBase container, RibbonPage insertAfter) : base(source, container) {
			InsertAfter = insertAfter.With(BarManagerCustomizationHelper.GetSerializationName);
		}
		protected override void ApplyOverride() {
			var clone = CreateClone();
			var container = FindContainer() as RibbonPageCategoryBase;
			if (clone == null || container == null)
				return;
			var index = (Host.FindTarget(InsertAfter) as RibbonPage).Return(x => container.Pages.IndexOf(x) + 1, () => -1);
			if (index == -1)
				container.Pages.Add(clone);
			else
				container.Pages.Insert(index, clone);
		}
		protected override void UndoOverride() {
			var container = FindContainer() as RibbonPageCategoryBase;
			var clone = Host.FindTarget(ResultName) as RibbonPage;
			if (container == null || clone == null)
				return;
			container.Pages.Remove(clone);
		}
		public RibbonPage CreateClone() {
			if (clone != null)
				return clone;
			var source = FindTarget() as RibbonPage;
			if (source == null)
				return null;
			clone = new RibbonPage();
			clone.Name = CloneNameHelper.GetCloneName(source, clone);
			clone.Caption = source.Caption;
			clone.CreatedByCustomizationDialog = true;
			if (string.IsNullOrEmpty(ResultName))
				ResultName = BarManagerCustomizationHelper.GetSerializationName(clone);
			else {
				clone.Name = ResultName;
				CloneNameHelper.Register(clone.Name);
			}
			return clone;
		}
		[XtraSerializableProperty]
		public string InsertAfter { get; set; }
		[XtraSerializableProperty]
		public string ResultName { get; set; }
	}
	public class CloneGroupCustomization : RuntimeCollectionCustomization {
		RibbonPageGroup clone;
		public CloneGroupCustomization() : this(null, null) { }
		public CloneGroupCustomization(RibbonPageGroup source, RibbonPage container) : this(source, container, null) { }
		public CloneGroupCustomization(RibbonPageGroup source, RibbonPage container, RibbonPageGroup insertAfter) : base(source, container) {
			InsertAfter = insertAfter.With(BarManagerCustomizationHelper.GetSerializationName);
		}
		protected override void ApplyOverride() {
			var clone = CreateClone();
			var container = FindContainer() as RibbonPage;
			if (clone == null || container == null)
				return;
			var index = (Host.FindTarget(InsertAfter) as RibbonPageGroup).Return(x => container.Groups.IndexOf(x) + 1, () => -1);
			if (index == -1)
				container.Groups.Add(clone);
			else
				container.Groups.Insert(index, clone);
		}
		protected override void UndoOverride() {
			var container = FindContainer() as RibbonPage;
			var clone = Host.FindTarget(ResultName) as RibbonPageGroup;
			if (container == null || clone == null)
				return;
			container.Groups.Remove(clone);
		}
		public RibbonPageGroup CreateClone() {
			if (clone != null)
				return clone;
			var source = FindTarget() as RibbonPageGroup;
			if (source == null)
				return null;
			clone = new RibbonPageGroup();
			clone.Name = CloneNameHelper.GetCloneName(source, clone);
			clone.Caption = source.Caption;
			clone.CreatedByCustomizationDialog = true;
			if (string.IsNullOrEmpty(ResultName))
				ResultName = BarManagerCustomizationHelper.GetSerializationName(clone);
			else {
				clone.Name = ResultName;
				CloneNameHelper.Register(clone.Name);
			}
			return clone;
		}
		[XtraSerializableProperty]
		public string InsertAfter { get; set; }
		[XtraSerializableProperty]
		public string ResultName { get; set; }
	}
	public class CreateNewGroupCustomization : RuntimeCollectionCustomization {
		public CreateNewGroupCustomization() : this(null, null) { }
		public CreateNewGroupCustomization(RibbonPageGroup target, RibbonPage container) : base(target, container) { }
		protected override void ApplyOverride() {
			var page = FindContainer() as RibbonPage;
			var group = FindTarget() as RibbonPageGroup;
			if (group == null) {
				group = new RibbonPageGroup();
				group.Name = TargetName;
				group.Caption = GroupCaption;
				group.CreatedByCustomizationDialog = true;
			}
			if (page == null || group == null)
				return;
			page.Groups.Add(group);
		}
		protected override void UndoOverride() {
			var page = FindContainer() as RibbonPage;
			var group = FindTarget() as RibbonPageGroup;
			if (page == null || group == null)
				return;
			page.Groups.Remove(group);
		}
		[XtraSerializableProperty]
		public string GroupCaption { get; set; }
	}
	public class RuntimeRemoveRibbonPageCustomization : RuntimeCollectionCustomization {
		[XtraSerializableProperty]
		public string HolderName { get; set; }
		[XtraSerializableProperty]
		public int Index { get; set; }
		[XtraSerializableProperty]
		public string BasedOn { get; set; }
		public RuntimeRemoveRibbonPageCustomization()
			: this(null) {
		}
		public RuntimeRemoveRibbonPageCustomization(RibbonPage forcedTarget)
			: base(forcedTarget, forcedTarget.With(x => x.PageCategory) as DependencyObject) {
		}
		protected RibbonPage FindRibbonPage() {
			return FindTarget() as RibbonPage;
		}
		protected override void ApplyOverride() {
			var ribbonPage = FindRibbonPage();
			var holder = ribbonPage.PageCategory;
			HolderName = (holder as IFrameworkInputElement).With(x => x.Name);
			Index = holder.Pages.IndexOf(ribbonPage);
			holder.Pages.Remove(ribbonPage);
		}
		protected override void UndoOverride() {
		}
	}
	public class RuntimeRemoveRibbonPageGroupCustomization : RuntimeCollectionCustomization {
		[XtraSerializableProperty]
		public string HolderName { get; set; }
		[XtraSerializableProperty]
		public int Index { get; set; }
		[XtraSerializableProperty]
		public string BasedOn { get; set; }
		public RuntimeRemoveRibbonPageGroupCustomization()
			: this(null) {
		}
		public RuntimeRemoveRibbonPageGroupCustomization(RibbonPageGroup forcedTarget)
			: base(forcedTarget, forcedTarget.With(x => x.Page) as DependencyObject) {
		}
		protected RibbonPageGroup FindRibbonPageGroup() {
			return FindTarget() as RibbonPageGroup;
		}
		protected override void ApplyOverride() {
			var ribbonPageGroup = FindRibbonPageGroup();
			var holder = ribbonPageGroup.Page;
			HolderName = (holder as IFrameworkInputElement).With(x => x.Name);
			Index = holder.Groups.IndexOf(ribbonPageGroup);
			holder.Groups.Remove(ribbonPageGroup);
		}
		protected override void UndoOverride() {
		}
	}
}
