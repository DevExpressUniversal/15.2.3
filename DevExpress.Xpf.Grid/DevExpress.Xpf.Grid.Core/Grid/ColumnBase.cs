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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using DevExpress.Core;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Helpers;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core.Serialization;
using DevExpress.Xpf.Data;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Xpf.Utils;
using DevExpress.XtraGrid;
using System.Linq;
using DevExpress.Xpf.GridData;
using DevExpress.Xpf.Core.ConditionalFormatting;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Grid {
	[
	ContentProperty("Header"), Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
	public abstract class GridColumnBase : ColumnBase {
	}
	public abstract partial class BaseColumn : DXFrameworkContentElement {
		internal const double DefaultMinWidth = 5;
		public static readonly DependencyProperty HeaderProperty;
		internal const string HeaderCaptionPropertyName = "HeaderCaption";
		protected internal static readonly DependencyPropertyKey HeaderCaptionPropertyKey;
		public static readonly DependencyProperty HeaderCaptionProperty;
		public static readonly DependencyProperty HeaderTemplateProperty;
		public static readonly DependencyProperty HeaderTemplateSelectorProperty;
		protected static readonly DependencyPropertyKey ActualHeaderTemplateSelectorPropertyKey;
		public static readonly DependencyProperty ActualHeaderTemplateSelectorProperty;
		public static readonly DependencyProperty HeaderToolTipProperty;
		public static readonly DependencyProperty HeaderToolTipTemplateProperty;
		static readonly DependencyPropertyKey ActualHeaderToolTipTemplatePropertyKey;
		public static readonly DependencyProperty ActualHeaderToolTipTemplateProperty;
		public static readonly DependencyProperty ColumnPositionProperty;
		public static readonly DependencyProperty HasTopElementProperty;
		public static readonly DependencyProperty HasBottomElementProperty;
		public static readonly DependencyProperty VisibleProperty;
		public static readonly DependencyProperty FixedWidthProperty;
#if !SL
		public static readonly DependencyProperty MinWidthProperty;
#else
		public new static readonly DependencyProperty MinWidthProperty;
#endif
		protected static readonly DependencyPropertyKey ActualWidthPropertyKey;
#if !SL
		public static readonly DependencyProperty ActualWidthProperty;
#else
		public new static readonly DependencyProperty ActualWidthProperty;
#endif
		static readonly DependencyPropertyKey ActualHeaderWidthPropertyKey;
		public static readonly DependencyProperty ActualHeaderWidthProperty;
#if !SL
		public static readonly DependencyProperty WidthProperty;
#else
		public new static readonly DependencyProperty WidthProperty;
#endif
		public static readonly DependencyProperty HasRightSiblingProperty;
		public static readonly DependencyProperty HasLeftSiblingProperty;
		internal const string ActualAllowResizingPropertyName = "ActualAllowResizing";
		protected static readonly DependencyPropertyKey ActualAllowResizingPropertyKey;
		public static readonly DependencyProperty ActualAllowResizingProperty;
		public static readonly DependencyProperty AllowResizingProperty;
		public static readonly DependencyProperty AllowMovingProperty;
		static readonly DependencyPropertyKey ActualAllowMovingPropertyKey;
		public static readonly DependencyProperty ActualAllowMovingProperty;
		public static readonly DependencyProperty VisibleIndexProperty;
		public static readonly DependencyProperty HorizontalHeaderContentAlignmentProperty;
		public static readonly DependencyProperty IsAutoGeneratedProperty;
		static readonly DependencyPropertyKey IsAutoGeneratedPropertyKey;
		public static readonly DependencyProperty FixedProperty;
		public static readonly DependencyProperty AllowPrintingProperty;
		static readonly DependencyPropertyKey ParentBandPropertyKey;
		public static readonly DependencyProperty ParentBandProperty;
		public static readonly DependencyProperty AllowSearchHeaderHighlightingProperty;
		public static readonly DependencyProperty HeaderStyleProperty;
		static BaseColumn() {
			Type ownerType = typeof(BaseColumn);
			HeaderProperty = DependencyPropertyManager.Register("Header", typeof(object), ownerType, new FrameworkPropertyMetadata(OnHeaderChanged));
			HeaderCaptionPropertyKey = DependencyPropertyManager.RegisterReadOnly(HeaderCaptionPropertyName, typeof(object), ownerType, new PropertyMetadata("", (d, e) => ((BaseColumn)d).HeaderCaptionChanged()));
			HeaderCaptionProperty = HeaderCaptionPropertyKey.DependencyProperty;
			HeaderTemplateProperty = DependencyPropertyManager.Register("HeaderTemplate", typeof(DataTemplate), ownerType, new PropertyMetadata(null, (d, e) => ((BaseColumn)d).UpdateActualHeaderTemplateSelector()));
			HeaderTemplateSelectorProperty = DependencyPropertyManager.Register("HeaderTemplateSelector", typeof(DataTemplateSelector), ownerType, new PropertyMetadata(null, (d, e) => ((BaseColumn)d).UpdateActualHeaderTemplateSelector()));
			ActualHeaderTemplateSelectorPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualHeaderTemplateSelector", typeof(DataTemplateSelector), ownerType, new PropertyMetadata((d, e) => ((BaseColumn)d).RaiseContentChanged(ActualHeaderTemplateSelectorProperty)));
			ActualHeaderTemplateSelectorProperty = ActualHeaderTemplateSelectorPropertyKey.DependencyProperty;
			HeaderToolTipProperty = DependencyPropertyManager.Register("HeaderToolTip", typeof(object), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((BaseColumn)d).RaiseContentChanged(HeaderToolTipProperty)));
			HeaderToolTipTemplateProperty = DependencyPropertyManager.Register("HeaderToolTipTemplate", typeof(DataTemplate), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((BaseColumn)d).UpdateActualHeaderToolTipTemplate()));
			ActualHeaderToolTipTemplatePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualHeaderToolTipTemplate", typeof(DataTemplate), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((BaseColumn)d).RaiseContentChanged(ActualHeaderToolTipTemplateProperty)));
			ActualHeaderToolTipTemplateProperty = ActualHeaderToolTipTemplatePropertyKey.DependencyProperty;
			ColumnPositionProperty = DependencyPropertyManager.Register("ColumnPosition", typeof(ColumnPosition), ownerType, new FrameworkPropertyMetadata(ColumnPosition.Middle, (d, e) => ((BaseColumn)d).RaiseContentChanged(ColumnPositionProperty)));
			HasTopElementProperty = DependencyPropertyManager.Register("HasTopElement", typeof(bool), ownerType, new PropertyMetadata(false, (d, e) => ((BaseColumn)d).RaiseHasTopElementChanged()));
			HasBottomElementProperty = DependencyPropertyManager.Register("HasBottomElement", typeof(bool), ownerType, new PropertyMetadata(false, (d, e) => ((BaseColumn)d).RaiseContentChanged(BaseColumn.HasBottomElementProperty)));
			VisibleProperty = DependencyPropertyManager.Register("Visible", typeof(bool), ownerType, new PropertyMetadata(true, (d, e) => ((BaseColumn)d).OnVisibleChanged()));
			HasRightSiblingProperty = DependencyPropertyManager.Register("HasRightSibling", typeof(bool), ownerType, new FrameworkPropertyMetadata(true, (d, e) => ((BaseColumn)d).OnHasRightSiblingChanged()));
			HasLeftSiblingProperty = DependencyPropertyManager.Register("HasLeftSibling", typeof(bool), ownerType, new FrameworkPropertyMetadata(true, (d, e) => ((BaseColumn)d).OnHasLeftSiblingChanged()));
			FixedWidthProperty = DependencyPropertyManager.Register("FixedWidth", typeof(bool), ownerType, new PropertyMetadata(false, (d, e) => ((BaseColumn)d).OnLayoutPropertyChanged()));
			MinWidthProperty = DependencyPropertyManager.Register("MinWidth", typeof(double), ownerType, new PropertyMetadata(DefaultMinWidth, (d, e) => ((BaseColumn)d).OnLayoutPropertyChanged(), OnMinWidthChanging));
			ActualWidthPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualWidth", typeof(double), ownerType, new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure));
			ActualWidthProperty = ActualWidthPropertyKey.DependencyProperty;
			ActualHeaderWidthPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualHeaderWidth", typeof(double), ownerType, new FrameworkPropertyMetadata(double.NaN, (d, e) => ((BaseColumn)d).OnActualHeaderWidthChanged(), (d, baseValue) => Math.Max(0, (double)baseValue)));
			ActualHeaderWidthProperty = ActualHeaderWidthPropertyKey.DependencyProperty;
			WidthProperty = DependencyPropertyManager.Register("Width", typeof(double), ownerType, new FrameworkPropertyMetadata(double.NaN, (d, e) => ((BaseColumn)d).OnWidthChanged(), (d, e) => ((BaseColumn)d).OnWidthChanging((double)e)));
			AllowResizingProperty = DependencyPropertyManager.Register("AllowResizing", typeof(DefaultBoolean), ownerType, new PropertyMetadata(DefaultBoolean.Default, (d, e) => ((BaseColumn)d).OnLayoutPropertyChanged()));
			ActualAllowResizingPropertyKey = DependencyPropertyManager.RegisterReadOnly(ActualAllowResizingPropertyName, typeof(bool), ownerType, new PropertyMetadata(true, (d, e) => ((BaseColumn)d).RaiseContentChanged(ActualAllowResizingProperty)));
			ActualAllowResizingProperty = ActualAllowResizingPropertyKey.DependencyProperty;
			AllowMovingProperty = DependencyPropertyManager.Register("AllowMoving", typeof(DefaultBoolean), ownerType, new FrameworkPropertyMetadata(DefaultBoolean.Default, (d, e) => ((BaseColumn)d).UpdateActualAllowMoving()));
			ActualAllowMovingPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualAllowMoving", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			ActualAllowMovingProperty = ActualAllowMovingPropertyKey.DependencyProperty;
			VisibleIndexProperty = DependencyPropertyManager.RegisterAttached("VisibleIndex", typeof(int), ownerType, new PropertyMetadata(-1, OnVisibleIndexChanged));
			HorizontalHeaderContentAlignmentProperty = DependencyPropertyManager.Register("HorizontalHeaderContentAlignment", typeof(HorizontalAlignment), ownerType, new FrameworkPropertyMetadata(HorizontalAlignment.Left, (d, e) => ((BaseColumn)d).RaiseContentChanged(HorizontalHeaderContentAlignmentProperty), new CoerceValueCallback(CoerceHorizontalHeaderContentAlignment)));
			FixedProperty = DependencyPropertyManager.Register("Fixed", typeof(FixedStyle), ownerType, new PropertyMetadata(FixedStyle.None, (d, e) => ((BaseColumn)d).OnFixedChanged()));
			CloneDetailHelper.RegisterKnownPropertyKeys(ownerType, ActualHeaderWidthPropertyKey, ActualWidthPropertyKey);
			IsAutoGeneratedPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsAutoGenerated", typeof(bool), ownerType, new PropertyMetadata(false));
			IsAutoGeneratedProperty = IsAutoGeneratedPropertyKey.DependencyProperty;
			AllowPrintingProperty = DependencyProperty.Register("AllowPrinting", typeof(bool), ownerType, new PropertyMetadata(true));
			ParentBandPropertyKey = DependencyProperty.RegisterReadOnly("ParentBand", typeof(BandBase), ownerType, new PropertyMetadata(null));
			ParentBandProperty = ParentBandPropertyKey.DependencyProperty;
			AllowSearchHeaderHighlightingProperty = DependencyProperty.Register("AllowSearchHeaderHighlighting", typeof(bool), ownerType, new PropertyMetadata(false, (d, e) => ((BaseColumn)d).OnAllowSearchHeaderHighlightingChanged()));
			HeaderStyleProperty = DependencyPropertyManager.Register("HeaderStyle", typeof(Style), ownerType, new PropertyMetadata(null, (d, e) => ((BaseColumn)d).OnHeaderStyleChanged(e)));
		}
		static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BaseColumn)d).SetHeaderCaption();
		}
		static void OnVisibleIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			BaseColumn column = d as BaseColumn;
			if(column != null)
				column.OnVisibleIndexChanged();
		}
		public static void SetVisibleIndex(DependencyObject element, int index) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(VisibleIndexProperty, index);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public static int GetVisibleIndex(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (int)element.GetValue(VisibleIndexProperty);
		}
		static object OnMinWidthChanging(DependencyObject d, object value) {
			if((double)value < DefaultMinWidth) return DefaultMinWidth;
			return value;
		}
		double OnWidthChanging(double value) {
			if(double.IsNaN(value)) return value;
			if(value < MinWidth) return MinWidth;
#if !SL
			if(value == Width && Width != ActualWidth)
				OnWidthChanged();
#endif
			return value;
		}
		void OnWidthChanged() {
			SetActualWidth(Width);
			OnLayoutPropertyChanged();
		}
		static object CoerceHorizontalHeaderContentAlignment(DependencyObject d, object value) {
			BaseColumn column = (BaseColumn)d;
			if(column.Visible) {
				return value;
			}
			return HorizontalAlignment.Left;
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("BaseColumnName"),
#endif
 XtraSerializableProperty, XtraResetProperty(ResetPropertyMode.None), GridStoreAlwaysProperty]
		public new string Name { get { return base.Name; } set { base.Name = value; } }
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("BaseColumnHeader"),
#endif
 Category(Categories.Data), TypeConverter(typeof(ObjectConverter)), XtraSerializableProperty]
		public object Header {
			get { return GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("BaseColumnHeaderCaption")]
#endif
		public object HeaderCaption {
			get { return GetValue(HeaderCaptionProperty); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("BaseColumnHeaderTemplate"),
#endif
 Category(Categories.Appearance)]
		public DataTemplate HeaderTemplate {
			get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
			set { SetValue(HeaderTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("BaseColumnHeaderTemplateSelector"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Category(Categories.Appearance)]
		public DataTemplateSelector HeaderTemplateSelector {
			get { return (DataTemplateSelector)GetValue(HeaderTemplateSelectorProperty); }
			set { SetValue(HeaderTemplateSelectorProperty, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("BaseColumnActualHeaderTemplateSelector")]
#endif
		public DataTemplateSelector ActualHeaderTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ActualHeaderTemplateSelectorProperty); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), CloneDetailMode(CloneDetailMode.Skip)]
		public bool AllowSearchHeaderHighlighting {
			get { return (bool)GetValue(AllowSearchHeaderHighlightingProperty); }
			set { SetValue(AllowSearchHeaderHighlightingProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("BaseColumnHeaderToolTip"),
#endif
 Category(Categories.Data), XtraSerializableProperty]
		public object HeaderToolTip {
			get { return GetValue(HeaderToolTipProperty); }
			set { SetValue(HeaderToolTipProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("BaseColumnHeaderToolTipTemplate"),
#endif
 Category(Categories.Appearance)]
		public DataTemplate HeaderToolTipTemplate {
			get { return (DataTemplate)GetValue(HeaderToolTipTemplateProperty); }
			set { SetValue(HeaderToolTipTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("BaseColumnActualHeaderToolTipTemplate")]
#endif
		public DataTemplate ActualHeaderToolTipTemplate {
			get { return (DataTemplate)GetValue(ActualHeaderToolTipTemplateProperty); }
			internal set { this.SetValue(ActualHeaderToolTipTemplatePropertyKey, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), CloneDetailMode(CloneDetailMode.Skip)]
		public ColumnPosition ColumnPosition {
			get { return (ColumnPosition)GetValue(ColumnPositionProperty); }
			set { SetValue(ColumnPositionProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), CloneDetailMode(CloneDetailMode.Skip)]
		public bool HasTopElement {
			get { return (bool)GetValue(HasTopElementProperty); }
			set { SetValue(HasTopElementProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), CloneDetailMode(CloneDetailMode.Skip)]
		public bool HasBottomElement {
			get { return (bool)GetValue(HasBottomElementProperty); }
			set { SetValue(HasBottomElementProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("BaseColumnVisible"),
#endif
 Category(Categories.Layout), XtraSerializableProperty, GridUIProperty]
		public bool Visible {
			get { return (bool)GetValue(VisibleProperty); }
			set { SetValue(VisibleProperty, value); }
		}
		public bool AllowPrinting {
			get { return (bool)GetValue(AllowPrintingProperty); }
			set { SetValue(AllowPrintingProperty, value); }
		}
		[
		Browsable(false),
		XtraSerializableProperty,
		XtraResetProperty(ResetPropertyMode.None),
		GridUIProperty,
		]
		public int VisibleIndex {
			get { return GetVisibleIndex(this); }
			set { SetVisibleIndex(this, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("BaseColumnHorizontalHeaderContentAlignment"),
#endif
 Category(Categories.Layout)]
		public HorizontalAlignment HorizontalHeaderContentAlignment {
			get { return (HorizontalAlignment)GetValue(HorizontalHeaderContentAlignmentProperty); }
			set { SetValue(HorizontalHeaderContentAlignmentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("BaseColumnFixed"),
#endif
 Category(Categories.Layout), XtraSerializableProperty, GridUIProperty]
		public FixedStyle Fixed {
			get { return (FixedStyle)GetValue(FixedProperty); }
			set { SetValue(FixedProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("BaseColumnFixedWidth"),
#endif
 DefaultValue(false), Category(Categories.Layout), XtraSerializableProperty, XtraResetProperty]
		public bool FixedWidth {
			get { return (bool)GetValue(FixedWidthProperty); }
			set { SetValue(FixedWidthProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("BaseColumnMinWidth"),
#endif
#if !SL
 TypeConverter(typeof(LengthConverter)),
#endif
 Category(Categories.Layout), XtraSerializableProperty]
#if !SL
		public double MinWidth {
#else
		public new double MinWidth {
#endif
			get { return (double)GetValue(MinWidthProperty); }
			set { SetValue(MinWidthProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("BaseColumnActualWidth"),
#endif
 XtraSerializableProperty(SerializationOrders.GridColumn_ActualWidth), GridUIProperty, XtraResetProperty(ResetPropertyMode.None), CloneDetailMode(CloneDetailMode.Force)]
#if !SL
		public double ActualWidth {
#else
		public new double ActualWidth {
#endif
			get { return (double)GetValue(ActualWidthProperty); }
		}
		protected internal void SetActualWidth(double value) {
			this.SetValue(ActualWidthPropertyKey, value);
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("BaseColumnActualHeaderWidth"),
#endif
 CloneDetailMode(CloneDetailMode.Force)]
		public double ActualHeaderWidth {
			get { return (double)GetValue(ActualHeaderWidthProperty); }
			internal set { this.SetValue(ActualHeaderWidthPropertyKey, value); }
		}
		internal double HeaderWidth { get; set; }
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("BaseColumnWidth"),
#endif
#if !SL
 TypeConverter(typeof(LengthConverter)),
#endif
 Category(Categories.Layout), XtraSerializableProperty(SerializationOrders.GridColumn_Width), GridUIProperty]
#if !SL
		public double Width {
#else
		public new double Width {
#endif
			get { return (double)GetValue(WidthProperty); }
			set { SetValue(WidthProperty, value); }
		}
		[Browsable(false), CloneDetailMode(CloneDetailMode.Skip)]
		public bool HasRightSibling {
			get { return (bool)GetValue(HasRightSiblingProperty); }
			set { this.SetValue(HasRightSiblingProperty, value); }
		}
		[Browsable(false), CloneDetailMode(CloneDetailMode.Skip)]
		public bool HasLeftSibling {
			get { return (bool)GetValue(HasLeftSiblingProperty); }
			set { this.SetValue(HasLeftSiblingProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("BaseColumnAllowResizing"),
#endif
 Category(Categories.Layout), XtraSerializableProperty]
		public DefaultBoolean AllowResizing {
			get { return (DefaultBoolean)GetValue(AllowResizingProperty); }
			set { SetValue(AllowResizingProperty, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("BaseColumnActualAllowResizing")]
#endif
		public bool ActualAllowResizing {
			get { return (bool)GetValue(ActualAllowResizingProperty); }
		}
		[ Category(Categories.Layout), XtraSerializableProperty]
		public DefaultBoolean AllowMoving {
			get { return (DefaultBoolean)GetValue(AllowMovingProperty); }
			set { SetValue(AllowMovingProperty, value); }
		}
		public bool ActualAllowMoving {
			get { return (bool)GetValue(ActualAllowMovingProperty); }
			private set { this.SetValue(ActualAllowMovingPropertyKey, value); }
		}
		public bool IsAutoGenerated {
			get { return (bool)GetValue(IsAutoGeneratedProperty); }
			internal set { this.SetValue(IsAutoGeneratedPropertyKey, value); }
		}
		[CloneDetailMode(CloneDetailMode.Skip)]
		public BandBase ParentBand {
			get { return (BandBase)GetValue(ParentBandProperty); }
			internal set { SetValue(ParentBandPropertyKey, value); }
		}
		[ Category(Categories.OptionsLayout)]
		public Style HeaderStyle {
			get { return (Style)GetValue(HeaderStyleProperty); }
			set { SetValue(HeaderStyleProperty, value); }
		}
		protected virtual void SetHeaderCaption() {
			this.SetValue(HeaderCaptionPropertyKey, Header);
		}
		protected virtual void HeaderCaptionChanged() {
			RaiseContentChanged(HeaderCaptionProperty);
		}
		internal protected virtual DataTemplate GetActualTemplate() {
			return HeaderTemplate;
		}
		internal protected virtual DataTemplateSelector GetActualTemplateSelector() {
			return HeaderTemplateSelector;
		}
		internal protected void UpdateActualHeaderTemplateSelector() {
			UpdateActualTemplateSelector(ActualHeaderTemplateSelectorPropertyKey, GetActualTemplateSelector(), GetActualTemplate());
		}
		internal protected virtual void UpdateActualHeaderToolTipTemplate() {
			ActualHeaderToolTipTemplate = HeaderToolTipTemplate;
		}
		protected virtual void OnVisibleChanged() {
			this.CoerceValue(ColumnBase.HorizontalHeaderContentAlignmentProperty);
			UpdateSearchInfo();
			OnPropertyChanged();
		}
		protected void UpdateSearchInfo() {
			if(View == null)
				return;
			View.UpdateColumnAllowSearchPanel(this);
		}
		protected void OnPropertyChanged() {
			if(ResizeOwner != null) {
				ResizeOwner.CalcColumnsLayout();
				ResizeOwner.UpdateContentLayout();
			}
		}
		protected virtual void OnVisibleIndexChanged() {
			if(ResizeOwner != null)
				ResizeOwner.ApplyColumnVisibleIndex(this);
		}
		protected virtual void OnActualHeaderWidthChanged() {
			RaiseContentChanged(ActualHeaderWidthProperty);
		}
		protected virtual void OnAllowSearchHeaderHighlightingChanged() {
			RaiseContentChanged(AllowSearchHeaderHighlightingProperty);
		}
		protected void UpdateContentLayout() {
			if(ResizeOwner != null)
				ResizeOwner.UpdateContentLayout();
		}
		void OnHeaderStyleChanged(DependencyPropertyChangedEventArgs e) {
			RaiseContentChanged(HeaderStyleProperty);
		}
		public event ColumnContentChangedEventHandler ContentChanged;
		protected void RaiseContentChanged(DependencyProperty property) {
			if(ContentChanged != null)
				ContentChanged(this, new ColumnContentChangedEventArgs(property));
		}
		internal protected void RaiseHasTopElementChanged() {
			RaiseContentChanged(BaseColumn.HasTopElementProperty);
		}
		protected BaseColumn GetEventTargetColumn() {
			return GetOriginationColumn() ?? this;
		}
		protected virtual BaseColumn GetOriginationColumn() {
			return null;
		}
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			base.OnPropertyChanged(e);
			DataControlBase dataControl = GetNotifySourceControl();
			if(dataControl != null && !dataControl.LockUpdateLayout) {
				Func<DataControlBase, DependencyObject> getTarget = CreateCloneAccessor();
				dataControl.GetDataControlOriginationElement().NotifyPropertyChanged(dataControl, e.Property, getTarget, typeof(BaseColumn));
			}
		}
		internal abstract DataControlBase GetNotifySourceControl();
		internal abstract Func<DataControlBase, BaseColumn> CreateCloneAccessor();
		internal protected void UpdateActualTemplateSelector(DependencyPropertyKey propertyKey, DataTemplateSelector selector, DataTemplate template, Func<DataTemplateSelector, DataTemplate, DataTemplateSelector> createWrapper = null) {
			DataControlOriginationElementHelper.UpdateActualTemplateSelector(this, GetOriginationColumn(), propertyKey, selector, template, createWrapper);
		}
		protected virtual void OnLayoutPropertyChanged() { }
		protected virtual void OnHasRightSiblingChanged() {
			RaiseContentChanged(HasRightSiblingProperty);
		}
		protected virtual void OnHasLeftSiblingChanged() {
			RaiseContentChanged(HasLeftSiblingProperty);
		}
		protected virtual void OnFixedChanged() {
			if(IsServiceColumn())
				return;
			OnPropertyChanged();
			RaiseContentChanged(FixedProperty);
		}
		internal virtual bool IsServiceColumn() { return false; }
		protected internal virtual IColumnOwnerBase ResizeOwner { get { return null; } }
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("BaseColumnView")]
#endif
		public DataViewBase View {
			get { return ResizeOwner as DataViewBase; }
		}
		internal protected virtual bool ActualAllowGroupingCore {
			get { return false; }
		}
		protected internal virtual bool GetAllowResizing() {
			return AllowResizing.GetValue(OwnerAllowResizing);
		}
		protected virtual bool OwnerAllowResizing { get { return ResizeOwner.AllowResizing; } }
		protected virtual bool OwnerAllowMoving { get { return ResizeOwner.AllowColumnMoving; } }
		internal abstract BandBase ParentBandInternal { get; }
		internal BandRow BandRow { get; set; }
		internal void UpdateActualAllowResizing() {
			this.SetValue(ActualAllowResizingPropertyKey, GetActualAllowResizing(ResizeOwner.AutoWidth));
		}
		void UpdateActualAllowMoving() {
			ActualAllowMoving = DesignerHelper.GetValue(this, AllowMoving.GetValue(OwnerAllowMoving), true);
		}
		protected virtual bool GetActualAllowResizing(bool autoWidth) {
			bool allowResizing = GetAllowResizing();
			if(allowResizing && autoWidth)
				return HasRightSibling;
			return allowResizing;
		}
		internal virtual void UpdateViewInfo(bool updateDataPropertiesOnly = false) {
			if(updateDataPropertiesOnly) return;
			UpdateActualAllowResizing();
			UpdateActualAllowMoving();
		}
		protected internal virtual bool CanStartDragSingleColumn {
			get {
				if(View.DataControl.BandsLayoutCore != null && View.DataControl.BandsLayoutCore.ShowBandsPanel)
					return true;
				return !View.IsLastVisibleColumn(this) || View.CanStartDragSingleColumn();
			}
		}
		internal int index = -1;
		protected internal virtual bool IsBand { get { return false; } }
		protected internal virtual bool AllowChangeParent { get { return View.DataControl.BandsLayoutCore.AllowChangeColumnParent; } }
		protected internal virtual bool CanDropTo(BaseColumn target) { return true; }
	}
	public abstract partial class ColumnBase : BaseColumn, IColumnInfo, IDefaultEditorViewInfo, IBestFitColumn, IDataColumnInfo, IInplaceEditorColumn, INotifyPropertyChanged {
		#region DependencyProperties
		protected internal static readonly DependencyPropertyKey IsSortedPropertyKey;
		public static readonly DependencyProperty IsSortedProperty, SortOrderProperty;
		static readonly DependencyPropertyKey IsSortedBySummaryPropertyKey;
		public static readonly DependencyProperty IsSortedBySummaryProperty;
		internal const string FieldNamePropertyName = "FieldName";
		public static readonly DependencyProperty FieldNameProperty;
		static readonly DependencyPropertyKey FieldTypePropertyKey;
		public static readonly DependencyProperty FieldTypeProperty;
		public static readonly DependencyProperty UnboundTypeProperty;
		public static readonly DependencyProperty UnboundExpressionProperty;
		public static readonly DependencyProperty ReadOnlyProperty;
		public static readonly DependencyProperty AllowEditingProperty;
		public static readonly DependencyProperty EditSettingsProperty;
		public static readonly DependencyProperty DisplayTemplateProperty;
		public static readonly DependencyProperty EditTemplateProperty;
		public static readonly DependencyProperty SortModeProperty;
		public static readonly DependencyProperty SortIndexProperty;
		public static readonly DependencyProperty CellTemplateProperty;
		public static readonly DependencyProperty CellTemplateSelectorProperty;
		static readonly DependencyPropertyKey ActualCellTemplateSelectorPropertyKey;
		public static readonly DependencyProperty ActualCellTemplateSelectorProperty;
		public static readonly DependencyProperty HeaderCustomizationAreaTemplateProperty;
		public static readonly DependencyProperty HeaderCustomizationAreaTemplateSelectorProperty;
		static readonly DependencyPropertyKey ActualHeaderCustomizationAreaTemplateSelectorPropertyKey;
		public static readonly DependencyProperty ActualHeaderCustomizationAreaTemplateSelectorProperty;
		public static readonly DependencyProperty FilterEditorHeaderTemplateProperty;
		public static readonly DependencyProperty HeaderPresenterTypeProperty;
		static readonly DependencyPropertyKey ActualDataWidthPropertyKey;
		public static readonly DependencyProperty ActualDataWidthProperty;
		static readonly DependencyPropertyKey ActualAdditionalRowDataWidthPropertyKey;
		public static readonly DependencyProperty ActualAdditionalRowDataWidthProperty;
		static readonly DependencyPropertyKey TotalSummaryTextPropertyKey;
		public static readonly DependencyProperty TotalSummaryTextProperty;
		static readonly DependencyPropertyKey HasTotalSummariesPropertyKey;
		public static readonly DependencyProperty HasTotalSummariesProperty;
		static readonly DependencyPropertyKey TotalSummariesPropertyKey;
		public static readonly DependencyProperty TotalSummariesProperty;
		public static readonly DependencyProperty AllowSortingProperty;
		static readonly DependencyPropertyKey ActualAllowSortingPropertyKey;
		public static readonly DependencyProperty ActualAllowSortingProperty;
		static readonly DependencyPropertyKey ActualEditSettingsPropertyKey;
		public static readonly DependencyProperty ActualEditSettingsProperty;
		static readonly DependencyPropertyKey ActualHorizontalContentAlignmentPropertyKey;
		public static readonly DependencyProperty ActualHorizontalContentAlignmentProperty;
		public static readonly DependencyProperty NavigationIndexProperty;
		public static readonly DependencyProperty CellStyleProperty;
		public static readonly DependencyProperty AutoFilterRowCellStyleProperty;
		public static readonly DependencyProperty NewItemRowCellStyleProperty;
		public static readonly DependencyProperty ColumnHeaderContentStyleProperty;
		public static readonly DependencyProperty TotalSummaryContentStyleProperty;
		public static readonly DependencyProperty ActualCellStyleProperty;
		public static readonly DependencyProperty ActualAutoFilterRowCellStyleProperty;
		public static readonly DependencyProperty ActualNewItemRowCellStyleProperty;
		public static readonly DependencyProperty ActualColumnHeaderContentStyleProperty;
		public static readonly DependencyProperty ActualTotalSummaryContentStyleProperty;
		static readonly DependencyPropertyKey ActualCellStylePropertyKey;
		static readonly DependencyPropertyKey ActualAutoFilterRowCellStylePropertyKey;
		static readonly DependencyPropertyKey ActualNewItemRowCellStylePropertyKey;
		static readonly DependencyPropertyKey ActualColumnHeaderContentStylePropertyKey;
		static readonly DependencyPropertyKey ActualTotalSummaryContentStylePropertyKey;
		internal const string ActualAllowColumnFilteringPropertyName = "ActualAllowColumnFiltering";
		protected static readonly DependencyPropertyKey ActualAllowColumnFilteringPropertyKey;
		public static readonly DependencyProperty ActualAllowColumnFilteringProperty;
		internal const string IsFilteredPropertyName = "IsFiltered";
		protected static readonly DependencyPropertyKey IsFilteredPropertyKey;
		public static readonly DependencyProperty IsFilteredProperty;
		public static readonly DependencyProperty AutoFilterValueProperty;
		public static readonly DependencyProperty CustomColumnFilterPopupTemplateProperty;
		public static readonly DependencyProperty AutoFilterConditionProperty;
		public static readonly DependencyProperty AllowAutoFilterProperty;
		public static readonly DependencyProperty ImmediateUpdateAutoFilterProperty;
		public static readonly DependencyProperty AllowColumnFilteringProperty;
		public static readonly DependencyProperty FilterPopupModeProperty;
		public static readonly DependencyProperty ColumnFilterModeProperty;
		public static readonly DependencyProperty ImmediateUpdateColumnFilterProperty;
		public static readonly DependencyProperty ColumnFilterPopupMaxRecordsCountProperty;
		public static readonly DependencyProperty ShowEmptyDateFilterProperty;
		public static readonly DependencyProperty AutoFilterRowDisplayTemplateProperty;
		public static readonly DependencyProperty AutoFilterRowEditTemplateProperty;
		public static readonly DependencyProperty RoundDateTimeForColumnFilterProperty;
		public static readonly DependencyProperty ShowAllTableValuesInCheckedFilterPopupProperty;
		public static readonly DependencyProperty ShowAllTableValuesInFilterPopupProperty;
		public static readonly DependencyProperty ShowInColumnChooserProperty;
		public static readonly DependencyProperty ColumnChooserHeaderCaptionProperty;
		protected static readonly DependencyPropertyKey ActualColumnChooserHeaderCaptionPropertyKey;
		internal const string ActualColumnChooserHeaderCaptionPropertyName = "ActualColumnChooserHeaderCaption";
		public static readonly DependencyProperty ActualColumnChooserHeaderCaptionProperty;
		public static readonly DependencyProperty BestFitWidthProperty;
		public static readonly DependencyProperty BestFitMaxRowCountProperty;
		public static readonly DependencyProperty BestFitModeProperty;
		public static readonly DependencyProperty BestFitAreaProperty;
		public static readonly DependencyProperty AllowBestFitProperty;
		static readonly DependencyPropertyKey IsLastPropertyKey;
		public static readonly DependencyProperty IsLastProperty;
		static readonly DependencyPropertyKey IsFirstPropertyKey;
		public static readonly DependencyProperty IsFirstProperty;
		public static readonly DependencyProperty AllowUnboundExpressionEditorProperty;
		public static readonly DependencyProperty PrintCellStyleProperty;
		public static readonly DependencyProperty PrintColumnHeaderStyleProperty;
		public static readonly DependencyProperty PrintTotalSummaryStyleProperty;
		public static readonly DependencyProperty ActualPrintCellStyleProperty;
		public static readonly DependencyProperty ActualPrintColumnHeaderStyleProperty;
		public static readonly DependencyProperty ActualPrintTotalSummaryStyleProperty;
		static readonly DependencyPropertyKey ActualPrintCellStylePropertyKey;
		static readonly DependencyPropertyKey ActualPrintColumnHeaderStylePropertyKey;
		static readonly DependencyPropertyKey ActualPrintTotalSummaryStylePropertyKey;
		public static readonly DependencyProperty AllowFocusProperty;
		public static readonly DependencyProperty TabStopProperty;
		public static readonly DependencyProperty ShowValidationAttributeErrorsProperty;
		static readonly DependencyPropertyKey ActualShowValidationAttributeErrorsPropertyKey;
		public static readonly DependencyProperty ActualShowValidationAttributeErrorsProperty;
		public static readonly DependencyProperty AllowSearchPanelProperty;
		public static readonly DependencyProperty CopyValueAsDisplayTextProperty;
		public static readonly DependencyProperty IsSmartProperty;
		public static readonly DependencyProperty AllowCellMergeProperty;
		public static readonly DependencyProperty AllowConditionalFormattingMenuProperty;
		public static readonly DependencyProperty EditFormCaptionProperty;
		public static readonly DependencyProperty EditFormColumnSpanProperty;
		public static readonly DependencyProperty EditFormRowSpanProperty;
		public static readonly DependencyProperty EditFormStartNewRowProperty;
		public static readonly DependencyProperty EditFormVisibleProperty;
		public static readonly DependencyProperty EditFormVisibleIndexProperty;
		public static readonly DependencyProperty EditFormTemplateProperty;
		static ColumnBase() {
			Type ownerType = typeof(ColumnBase);
			IsSortedPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsSorted", typeof(bool), ownerType, new PropertyMetadata(false));
			IsSortedProperty = IsSortedPropertyKey.DependencyProperty;
			IsSortedBySummaryPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsSortedBySummary", typeof(bool), ownerType, new PropertyMetadata(false));
			IsSortedBySummaryProperty = IsSortedBySummaryPropertyKey.DependencyProperty;
			SortOrderProperty = DependencyPropertyManager.Register("SortOrder", typeof(ColumnSortOrder), ownerType, new PropertyMetadata(ColumnSortOrder.None, OnSortOrderChanged));
			FieldTypePropertyKey = DependencyPropertyManager.RegisterReadOnly("FieldType", typeof(Type), ownerType, new PropertyMetadata(typeof(Object), (d, e) => ((ColumnBase)d).OnFieldTypeChanged()));
			FieldTypeProperty = FieldTypePropertyKey.DependencyProperty;
			FieldNameProperty = DependencyPropertyManager.Register(FieldNamePropertyName, typeof(string), ownerType, new FrameworkPropertyMetadata("", new PropertyChangedCallback(OnFieldNameChanged), (column, fieldName) => ((ColumnBase)column).CoerceFieldName(fieldName)));
			UnboundTypeProperty = DependencyPropertyManager.Register("UnboundType", typeof(UnboundColumnType), ownerType, new FrameworkPropertyMetadata(UnboundColumnType.Bound, new PropertyChangedCallback(ColumnBase.OnUnboundTypeChanged)));
			UnboundExpressionProperty = DependencyPropertyManager.Register("UnboundExpression", typeof(string), ownerType, new FrameworkPropertyMetadata(string.Empty, (d, e) => ((ColumnBase)d).OnUnboundChanged()));
			ReadOnlyProperty = DependencyPropertyManager.Register("ReadOnly", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			AllowEditingProperty = DependencyPropertyManager.Register("AllowEditing", typeof(DefaultBoolean), ownerType, new FrameworkPropertyMetadata(DefaultBoolean.Default, (d, e) => ((ColumnBase)d).UpdateEditorButtonVisibilities()));
			EditSettingsProperty = DependencyPropertyManager.Register("EditSettings", typeof(BaseEditSettings), ownerType, new FrameworkPropertyMetadata(null, OnEditSettingsChanged));
			EditTemplateProperty = DependencyPropertyManager.Register("EditTemplate", typeof(ControlTemplate), ownerType, new FrameworkPropertyMetadata(null));
			DisplayTemplateProperty = DependencyPropertyManager.Register("DisplayTemplate", typeof(ControlTemplate), ownerType, new FrameworkPropertyMetadata(null, OnDisplayTemplateChanged));
			SortModeProperty = DependencyPropertyManager.Register("SortMode", typeof(ColumnSortMode), ownerType, new FrameworkPropertyMetadata(ColumnSortMode.Default, new PropertyChangedCallback(ColumnBase.OnDataPropertyChanged)));
			SortIndexProperty = DependencyPropertyManager.Register("SortIndex", typeof(int), ownerType, new PropertyMetadata(-1, OnSortIndexChanged));
			CellTemplateProperty = DependencyPropertyManager.Register("CellTemplate", typeof(DataTemplate), ownerType, new PropertyMetadata(null, (d, e) => ((ColumnBase)d).UpdateActualCellTemplateSelector()));
			CellTemplateSelectorProperty = DependencyPropertyManager.Register("CellTemplateSelector", typeof(DataTemplateSelector), ownerType, new PropertyMetadata(null, (d, e) => ((ColumnBase)d).UpdateActualCellTemplateSelector()));
			ActualCellTemplateSelectorPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualCellTemplateSelector", typeof(DataTemplateSelector), ownerType, new FrameworkPropertyMetadata(null, (d, _) => ((ColumnBase)d).UpdateContentLayout()));
			ActualCellTemplateSelectorProperty = ActualCellTemplateSelectorPropertyKey.DependencyProperty;
			HeaderCustomizationAreaTemplateProperty = DependencyPropertyManager.Register("HeaderCustomizationAreaTemplate", typeof(DataTemplate), ownerType, new PropertyMetadata(null, (d, e) => ((ColumnBase)d).UpdateActualHeaderCustomizationAreaTemplateSelector()));
			HeaderCustomizationAreaTemplateSelectorProperty = DependencyPropertyManager.Register("HeaderCustomizationAreaTemplateSelector", typeof(DataTemplateSelector), ownerType, new PropertyMetadata(null, (d, e) => ((ColumnBase)d).UpdateActualHeaderCustomizationAreaTemplateSelector()));
			ActualHeaderCustomizationAreaTemplateSelectorPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualHeaderCustomizationAreaTemplateSelector", typeof(DataTemplateSelector), ownerType, new PropertyMetadata(null, (d, e) => ((ColumnBase)d).RaiseContentChanged(ActualHeaderCustomizationAreaTemplateSelectorProperty)));
			ActualHeaderCustomizationAreaTemplateSelectorProperty = ActualHeaderCustomizationAreaTemplateSelectorPropertyKey.DependencyProperty;
			FilterEditorHeaderTemplateProperty = DependencyPropertyManager.Register("FilterEditorHeaderTemplate", typeof(DataTemplate), ownerType);
			HeaderPresenterTypeProperty = DependencyPropertyManager.RegisterAttached("HeaderPresenterType", typeof(HeaderPresenterType), ownerType, new FrameworkPropertyMetadata(HeaderPresenterType.Headers, FrameworkPropertyMetadataOptions.Inherits));
			ActualDataWidthPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualDataWidth", typeof(double), ownerType, new FrameworkPropertyMetadata(double.NaN, (d, _) => ((ColumnBase)d).UpdateContentLayout(), (d, baseValue) => Math.Max(0, (double)baseValue)));
			ActualDataWidthProperty = ActualDataWidthPropertyKey.DependencyProperty;
			ActualAdditionalRowDataWidthPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualAdditionalRowDataWidth", typeof(double), ownerType, new FrameworkPropertyMetadata(double.NaN, null, (d, baseValue) => Math.Max(0, (double)baseValue)));
			ActualAdditionalRowDataWidthProperty = ActualAdditionalRowDataWidthPropertyKey.DependencyProperty;
			TotalSummaryTextPropertyKey = DependencyPropertyManager.RegisterReadOnly("TotalSummaryText", typeof(string), ownerType, new PropertyMetadata(" "));
			TotalSummaryTextProperty = TotalSummaryTextPropertyKey.DependencyProperty;
			HasTotalSummariesPropertyKey = DependencyPropertyManager.RegisterReadOnly("HasTotalSummaries", typeof(bool), ownerType, new PropertyMetadata(false));
			HasTotalSummariesProperty = HasTotalSummariesPropertyKey.DependencyProperty;
			TotalSummariesPropertyKey = DependencyPropertyManager.RegisterReadOnly("TotalSummaries", typeof(IList<GridTotalSummaryData>), ownerType, new PropertyMetadata(null));
			TotalSummariesProperty = TotalSummariesPropertyKey.DependencyProperty;
			NavigationIndexProperty = DependencyPropertyManager.RegisterAttached("NavigationIndex", typeof(int), ownerType, new PropertyMetadata(Constants.InvalidNavigationIndex, OnNavigationIndexChanged));
			AllowSortingProperty = DependencyPropertyManager.Register("AllowSorting", typeof(DefaultBoolean), ownerType,
				new FrameworkPropertyMetadata(DefaultBoolean.Default, (d, e) => ((ColumnBase)d).UpdateActualAllowSorting(), (d, value) => ((ColumnBase)d).CoerceAllowSorting((DefaultBoolean)value)));
			ActualAllowSortingPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualAllowSorting", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			ActualAllowSortingProperty = ActualAllowSortingPropertyKey.DependencyProperty;
			ActualEditSettingsPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualEditSettings", typeof(BaseEditSettings), ownerType, new FrameworkPropertyMetadata(null, OnActualEditSettingsChanged));
			ActualEditSettingsProperty = ActualEditSettingsPropertyKey.DependencyProperty;
			ActualHorizontalContentAlignmentPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualHorizontalContentAlignment", typeof(HorizontalAlignment), ownerType, new PropertyMetadata(HorizontalAlignment.Right));
			ActualHorizontalContentAlignmentProperty = ActualHorizontalContentAlignmentPropertyKey.DependencyProperty;
			CellStyleProperty = DependencyPropertyManager.Register("CellStyle", typeof(Style), ownerType, new PropertyMetadata(null, OnAppearanceChanged));
			AutoFilterRowCellStyleProperty = DependencyPropertyManager.Register("AutoFilterRowCellStyle", typeof(Style), ownerType, new PropertyMetadata(null, OnAppearanceChanged));
			NewItemRowCellStyleProperty = DependencyPropertyManager.Register("NewItemRowCellStyle", typeof(Style), ownerType, new PropertyMetadata(null, OnAppearanceChanged));
			ColumnHeaderContentStyleProperty = DependencyPropertyManager.Register("ColumnHeaderContentStyle", typeof(Style), ownerType, new PropertyMetadata(null, OnAppearanceChanged));
			ActualCellStylePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualCellStyle", typeof(Style), ownerType, new FrameworkPropertyMetadata(null, (d, _) => ((ColumnBase)d).OnActualCellStyleCahnged()));
			ActualCellStyleProperty = ActualCellStylePropertyKey.DependencyProperty;
			ActualAutoFilterRowCellStylePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualAutoFilterRowCellStyle", typeof(Style), ownerType, new FrameworkPropertyMetadata(null));
			ActualAutoFilterRowCellStyleProperty = ActualAutoFilterRowCellStylePropertyKey.DependencyProperty;
			ActualNewItemRowCellStylePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualNewItemRowCellStyle", typeof(Style), ownerType, new FrameworkPropertyMetadata(null));
			ActualNewItemRowCellStyleProperty = ActualNewItemRowCellStylePropertyKey.DependencyProperty;
			ActualColumnHeaderContentStylePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualColumnHeaderContentStyle", typeof(Style), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((ColumnBase)d).RaiseContentChanged(ActualColumnHeaderContentStyleProperty)));
			ActualColumnHeaderContentStyleProperty = ActualColumnHeaderContentStylePropertyKey.DependencyProperty;
			PrintCellStyleProperty = DependencyPropertyManager.Register("PrintCellStyle", typeof(Style), ownerType, new PropertyMetadata(null, OnAppearanceChanged));
			PrintColumnHeaderStyleProperty = DependencyPropertyManager.Register("PrintColumnHeaderStyle", typeof(Style), ownerType, new PropertyMetadata(null, OnAppearanceChanged));
			PrintTotalSummaryStyleProperty = DependencyPropertyManager.Register("PrintTotalSummaryStyle", typeof(Style), ownerType, new PropertyMetadata(null, OnAppearanceChanged));
			ActualPrintCellStylePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualPrintCellStyle", typeof(Style), ownerType, new FrameworkPropertyMetadata(null));
			ActualPrintCellStyleProperty = ActualPrintCellStylePropertyKey.DependencyProperty;
			ActualPrintColumnHeaderStylePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualPrintColumnHeaderStyle", typeof(Style), ownerType, new FrameworkPropertyMetadata(null));
			ActualPrintColumnHeaderStyleProperty = ActualPrintColumnHeaderStylePropertyKey.DependencyProperty;
			ActualPrintTotalSummaryStylePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualPrintTotalSummaryStyle", typeof(Style), ownerType, new FrameworkPropertyMetadata(null));
			ActualPrintTotalSummaryStyleProperty = ActualPrintTotalSummaryStylePropertyKey.DependencyProperty;
			ActualAllowColumnFilteringPropertyKey = DependencyPropertyManager.RegisterReadOnly(ActualAllowColumnFilteringPropertyName, typeof(bool), ownerType, new PropertyMetadata(false, (d, e) => ((ColumnBase)d).RaiseContentChanged(ActualAllowColumnFilteringProperty)));
			ActualAllowColumnFilteringProperty = ActualAllowColumnFilteringPropertyKey.DependencyProperty;
			IsFilteredPropertyKey = DependencyPropertyManager.RegisterReadOnly(IsFilteredPropertyName, typeof(bool), ownerType, new PropertyMetadata(false, (d, e) => ((ColumnBase)d).RaiseContentChanged(IsFilteredProperty)));
			IsFilteredProperty = IsFilteredPropertyKey.DependencyProperty;
			AutoFilterValueProperty = DependencyPropertyManager.Register("AutoFilterValue", typeof(object), ownerType, new PropertyMetadata(null, OnAutoFilterValueChanged));
			CustomColumnFilterPopupTemplateProperty = DependencyPropertyManager.Register("CustomColumnFilterPopupTemplate", typeof(DataTemplate), ownerType, new PropertyMetadata(null));
			AutoFilterConditionProperty = DependencyPropertyManager.Register("AutoFilterCondition", typeof(AutoFilterCondition), ownerType, new PropertyMetadata(AutoFilterCondition.Default, OnAutoFilterConditionChanged));
			AllowAutoFilterProperty = DependencyPropertyManager.Register("AllowAutoFilter", typeof(bool), ownerType, new PropertyMetadata(true));
			ImmediateUpdateAutoFilterProperty = DependencyPropertyManager.Register("ImmediateUpdateAutoFilter", typeof(bool), ownerType, new PropertyMetadata(true));
			AllowColumnFilteringProperty = DependencyPropertyManager.Register("AllowColumnFiltering", typeof(DefaultBoolean), ownerType, new PropertyMetadata(DefaultBoolean.Default, OnAllowColumnFilteringChanged));
			FilterPopupModeProperty = DependencyPropertyManager.Register("FilterPopupMode", typeof(FilterPopupMode), ownerType, new PropertyMetadata(FilterPopupMode.Default, OnFilterPopupModeChanged));
			ColumnFilterModeProperty = DependencyPropertyManager.Register("ColumnFilterMode", typeof(ColumnFilterMode), ownerType, new PropertyMetadata(ColumnFilterMode.Value, OnColumnFilterModeChanged));
			ImmediateUpdateColumnFilterProperty = DependencyPropertyManager.Register("ImmediateUpdateColumnFilter", typeof(bool), ownerType, new PropertyMetadata(true));
			ColumnFilterPopupMaxRecordsCountProperty = DependencyPropertyManager.Register("ColumnFilterPopupMaxRecordsCount", typeof(int), ownerType, new PropertyMetadata(-1));
			ShowEmptyDateFilterProperty = DependencyPropertyManager.Register("ShowEmptyDateFilter", typeof(bool), ownerType, new PropertyMetadata(false));
			AutoFilterRowDisplayTemplateProperty = DependencyPropertyManager.Register("AutoFilterRowDisplayTemplate", typeof(ControlTemplate), ownerType, new FrameworkPropertyMetadata(null, OnAutoFilterRowDisplayTemplateChanged));
			AutoFilterRowEditTemplateProperty = DependencyPropertyManager.Register("AutoFilterRowEditTemplate", typeof(ControlTemplate), ownerType, new FrameworkPropertyMetadata(null));
			RoundDateTimeForColumnFilterProperty = DependencyPropertyManager.Register("RoundDateTimeForColumnFilter", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			ShowAllTableValuesInFilterPopupProperty = DependencyPropertyManager.Register("ShowAllTableValuesInFilterPopup", typeof(DefaultBoolean), ownerType, new PropertyMetadata(DefaultBoolean.Default));
			ShowAllTableValuesInCheckedFilterPopupProperty = DependencyPropertyManager.Register("ShowAllTableValuesInCheckedFilterPopup", typeof(DefaultBoolean), ownerType, new PropertyMetadata(DefaultBoolean.Default));
			ShowInColumnChooserProperty = DependencyPropertyManager.Register("ShowInColumnChooser", typeof(bool), ownerType, new FrameworkPropertyMetadata(true, (d, e) => ((ColumnBase)d).RebuildColumnChooserColumns()));
			ActualColumnChooserHeaderCaptionPropertyKey = DependencyPropertyManager.RegisterReadOnly(ActualColumnChooserHeaderCaptionPropertyName, typeof(object), ownerType, new PropertyMetadata((d, e) => ((ColumnBase)d).RaiseContentChanged(ActualColumnChooserHeaderCaptionProperty)));
			ActualColumnChooserHeaderCaptionProperty = ActualColumnChooserHeaderCaptionPropertyKey.DependencyProperty;
			ColumnChooserHeaderCaptionProperty = DependencyPropertyManager.Register("ColumnChooserHeaderCaption", typeof(object), ownerType, new FrameworkPropertyMetadata(null, OnColumnChooserHeaderCaptionChanged));
			TotalSummaryContentStyleProperty = DependencyPropertyManager.Register("TotalSummaryContentStyle", typeof(Style), ownerType, new PropertyMetadata(null, OnAppearanceChanged));
			ActualTotalSummaryContentStylePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualTotalSummaryContentStyle", typeof(Style), ownerType, new FrameworkPropertyMetadata(null));
			ActualTotalSummaryContentStyleProperty = ActualTotalSummaryContentStylePropertyKey.DependencyProperty;
			BestFitWidthProperty = DependencyPropertyManager.Register("BestFitWidth", typeof(double), ownerType, new FrameworkPropertyMetadata(double.NaN, null, (d, baseValue) => CoerceBestFitWidth(Convert.ToDouble(baseValue))));
			BestFitMaxRowCountProperty = DependencyPropertyManager.Register("BestFitMaxRowCount", typeof(int), ownerType, new FrameworkPropertyMetadata(-1, null, (d, baseValue) => DataViewBase.CoerceBestFitMaxRowCount(Convert.ToInt32(baseValue))));
			BestFitModeProperty = DependencyPropertyManager.Register("BestFitMode", typeof(BestFitMode), ownerType, new FrameworkPropertyMetadata(BestFitMode.Default));
			BestFitAreaProperty = DependencyPropertyManager.Register("BestFitArea", typeof(BestFitArea), ownerType, new FrameworkPropertyMetadata(BestFitArea.None));
			AllowBestFitProperty = DependencyPropertyManager.Register("AllowBestFit", typeof(DefaultBoolean), ownerType, new FrameworkPropertyMetadata(DefaultBoolean.Default));
			AllowUnboundExpressionEditorProperty = DependencyPropertyManager.Register("AllowUnboundExpressionEditor", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			EventManager.RegisterClassHandler(ownerType, DXSerializer.AllowPropertyEvent, new AllowPropertyEventHandler(OnDeserializeAllowProperty));
			EventManager.RegisterClassHandler(ownerType, DXSerializer.DeserializePropertyEvent, new XtraPropertyInfoEventHandler(OnDeserializeProperty));
			EventManager.RegisterClassHandler(ownerType, DXSerializer.CustomShouldSerializePropertyEvent, new CustomShouldSerializePropertyEventHandler(OnCustomShouldSerializeProperty));
			IsLastPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsLast", typeof(bool), ownerType, new PropertyMetadata(false));
			IsLastProperty = IsLastPropertyKey.DependencyProperty;
			IsFirstPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsFirst", typeof(bool), ownerType, new PropertyMetadata(false));
			IsFirstProperty = IsFirstPropertyKey.DependencyProperty;
			AllowFocusProperty = DependencyPropertyManager.Register("AllowFocus", typeof(bool), ownerType, new PropertyMetadata(true, OnAllowFocusChanged));
			TabStopProperty = DependencyPropertyManager.Register("TabStop", typeof(bool), ownerType, new PropertyMetadata(true));
			ShowValidationAttributeErrorsProperty = DependencyPropertyManager.Register("ShowValidationAttributeErrors", typeof(DefaultBoolean), ownerType, new FrameworkPropertyMetadata(DefaultBoolean.Default, (d, e) => ((ColumnBase)d).UpdateShowValidationAttributeErrors()));
			ActualShowValidationAttributeErrorsPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualShowValidationAttributeErrors", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			ActualShowValidationAttributeErrorsProperty = ActualShowValidationAttributeErrorsPropertyKey.DependencyProperty;
			AllowSearchPanelProperty = DependencyPropertyManager.Register("AllowSearchPanel", typeof(DefaultBoolean), ownerType, new FrameworkPropertyMetadata(DefaultBoolean.Default, (d, e) => ((ColumnBase)d).OnAllowSearchPanelChanged()));
			CopyValueAsDisplayTextProperty = DependencyPropertyManager.Register("CopyValueAsDisplayText", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			IsSmartProperty = DependencyPropertyManager.Register("IsSmart", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			AllowCellMergeProperty = DependencyPropertyManager.Register("AllowCellMerge", typeof(bool?), ownerType, new PropertyMetadata(null, (d, e) => ((ColumnBase)d).OnAllowCellMergeChanged((bool?)e.OldValue)));
			AllowConditionalFormattingMenuProperty = DependencyPropertyManager.Register("AllowConditionalFormattingMenu", typeof(bool?), typeof(ColumnBase), new PropertyMetadata(null));
			EditFormCaptionProperty = DependencyProperty.Register("EditFormCaption", typeof(object), ownerType, new PropertyMetadata(null));
			EditFormColumnSpanProperty = DependencyProperty.Register("EditFormColumnSpan", typeof(int?), ownerType, new PropertyMetadata(null));
			EditFormRowSpanProperty = DependencyProperty.Register("EditFormRowSpan", typeof(int?), ownerType, new PropertyMetadata(null));
			EditFormStartNewRowProperty = DependencyProperty.Register("EditFormStartNewRow", typeof(bool), ownerType, new PropertyMetadata(false));
			EditFormVisibleProperty = DependencyProperty.Register("EditFormVisible", typeof(bool?), ownerType, new PropertyMetadata(null));
			EditFormVisibleIndexProperty = DependencyProperty.Register("EditFormVisibleIndex", typeof(int), ownerType, new PropertyMetadata(0));
			EditFormTemplateProperty = DependencyProperty.Register("EditFormTemplate", typeof(DataTemplate), ownerType, new PropertyMetadata(null));
			CloneDetailHelper.RegisterKnownPropertyKeys(ownerType, ActualAdditionalRowDataWidthPropertyKey, ActualDataWidthPropertyKey);
		}
		protected override void OnVisibleChanged() {
			base.OnVisibleChanged();
			ReInitializeFocusedColumnIfNeeded();
		}
		static void OnAllowFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ColumnBase column = (ColumnBase)d;
			column.ReInitializeFocusedColumnIfNeeded();
		}
		static void OnNavigationIndexChanged(DependencyObject dObject, DependencyPropertyChangedEventArgs e) {
			INotifyNavigationIndexChanged notifyNavigationIndexChanged = dObject as INotifyNavigationIndexChanged;
			if(notifyNavigationIndexChanged != null) {
				notifyNavigationIndexChanged.OnNavigationIndexChanged();
			}
		}
		DefaultBoolean CoerceAllowSorting(DefaultBoolean value) {
			if(OwnerControl == null || !OwnerControl.IsDissalowSortingColumn(this))
				return value;
			return DefaultBoolean.False;
		}
		static double CoerceBestFitWidth(double baseValue) {
			return Math.Max(baseValue, 0);
		}
		static void OnDeserializeAllowProperty(object sender, AllowPropertyEventArgs e) {
			((ColumnBase)sender).OnDeserializeAllowPropertyInternal(e);
		}
		static void OnDeserializeProperty(object sender, XtraPropertyInfoEventArgs e) {
			((ColumnBase)sender).OnDeserializeProperty(e);
		}
		static void OnCustomShouldSerializeProperty(object sender, CustomShouldSerializePropertyEventArgs e) {
			((ColumnBase)sender).OnCustomShouldSerializeProperty(e);
		}
		static void OnSortIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ColumnBase column = d as ColumnBase;
			if(column != null)
				column.OnSortIndexChanged();
		}
		static void OnSortOrderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ColumnBase column = d as ColumnBase;
			if(column != null)
				column.OnSortOrderChanged();
		}
		static void OnActualEditSettingsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ColumnBase column = d as ColumnBase;
			if(column != null)
				column.OnActualEditSettingsChanged((BaseEditSettings)e.NewValue, (BaseEditSettings)e.OldValue);
		}
		void OnActualEditSettingsChanged(BaseEditSettings newValue, BaseEditSettings oldValue) {
			if(oldValue != null && oldValue.Parent == this)
				RemoveLogicalChild(oldValue);
			if(newValue != null && newValue.Parent == null)
				AddLogicalChild(newValue);
			ActualEditSettingsCore = newValue;
			UpdateActualCellTemplateSelector();
			if(oldValue != null)
				EditSettingsChangedEventHandler.Unsubscribe(oldValue);
			if(newValue != null)
				EditSettingsChangedEventHandler.Subscribe(newValue);
			OnEditSettingsContentChanged();
		}
		static void OnAppearanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ColumnBase)d).UpdateAppearance();
		}
		static void OnEditSettingsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ColumnBase)d).OnEditSettingsChanged((BaseEditSettings)e.OldValue);
		}
		static void OnDisplayTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ColumnBase)d).OnDisplayTemplateChanged();
		}
		static void OnColumnChooserHeaderCaptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ColumnBase)d).SetActualColumnChooserHeaderCaption();
		}
		static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ColumnBase)d).SetHeaderCaption();
		}
		static void OnFieldNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ColumnBase)d).OnFieldNameChanged(e.OldValue as string);
		}
		static void OnUnboundTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ColumnBase)d).OnUnboundTypeChanged();
		}
		protected static void OnDataPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ColumnBase)d).OnDataPropertyChanged();
		}
		static void OnAutoFilterValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ColumnBase)d).OnAutoFilterValueChanged();
		}
		static void OnAutoFilterConditionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ColumnBase)d).UpdateAutoFilter();
		}
		static void OnAllowColumnFilteringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ColumnBase)d).UpdateActualAllowColumnFiltering();
			((ColumnBase)d).UpdateShowEditFilterButton(e);
		}
		static void OnFilterPopupModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ColumnBase)d).OnFilterPopupModeChanged();
		}
		static void OnColumnFilterModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ColumnBase)d).OnColumnFilterModeChanged();
		}
		static void OnAutoFilterRowDisplayTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ColumnBase)d).OnAutoFilterRowDisplayTemplateChanged();
		}
		public static int GetNavigationIndex(DependencyObject dependencyObject) {
			return (int)dependencyObject.GetValue(NavigationIndexProperty);
		}
		public static void SetNavigationIndex(DependencyObject dependencyObject, int index) {
			dependencyObject.SetValue(NavigationIndexProperty, index);
		}
		public static HeaderPresenterType GetHeaderPresenterType(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (HeaderPresenterType)element.GetValue(HeaderPresenterTypeProperty);
		}
		public static void SetHeaderPresenterType(DependencyObject element, HeaderPresenterType value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(HeaderPresenterTypeProperty, value);
		}
		#endregion
		internal static string GetSummaryDisplayName(ColumnBase column, DevExpress.Xpf.Grid.SummaryItemBase summaryItem) {
			return GetDisplayName(column, summaryItem.FieldName);
		}
		internal static string GetDisplayName(ColumnBase column, string fieldName) {
			return column != null ? column.HeaderCaption.ToString() : fieldName;
		}
		BindingBase displayMemberBinding;
		internal override BandBase ParentBandInternal {
			get { return ParentBand; }
		}
		internal virtual int GroupIndexCore { get { return -1; } set { } }
		protected internal override IColumnOwnerBase ResizeOwner { get { return Owner; } }
		protected virtual void OnOwnerChanged() {
			UpdateActualHeaderCustomizationAreaTemplateSelector();
			UpdateActualHeaderTemplateSelector();
			UpdateActualHeaderToolTipTemplate();
			UpdateActualCellTemplateSelector();
			UpdateActualEditSettings();
			ValidateSimpleBinding(SimpleBindingState.Data);
			SetUnboundType();
		}
		internal IDesignTimeAdornerBase DesignTimeGridAdorner { get { return OwnerControl != null ? OwnerControl.DesignTimeAdorner : EmptyDesignTimeAdornerBase.Instance; } }
		protected internal IColumnOwnerBase Owner { get { return (OwnerControl != null ? OwnerControl.viewCore : null) ?? EmptyColumnOwnerBase.Instance; } }
		protected virtual IColumnCollection ParentCollection { get { return OwnerControl != null ? OwnerControl.ColumnsCore : null; } }
		DataControlBase ownerControlCore;
		internal DataControlBase OwnerControl {
			get { return ownerControlCore; }
			set {
				if(ownerControlCore != value) {
					ownerControlCore = value;
					ChangeOwner();
				}
			}
		}
		internal void ChangeOwner() {
			OnPropertyChanged("View");
			OnOwnerChanged();
		}
		internal bool IsUnbound { get { return UnboundType != UnboundColumnType.Bound; } }
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("ColumnBaseActualShowValidationAttributeErrors")]
#endif
		public bool ActualShowValidationAttributeErrors {
			get { return (bool)GetValue(ActualShowValidationAttributeErrorsProperty); }
			private set { this.SetValue(ActualShowValidationAttributeErrorsPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseShowValidationAttributeErrors"),
#endif
 Category(Categories.Layout), XtraSerializableProperty]
		public DefaultBoolean ShowValidationAttributeErrors {
			get { return (DefaultBoolean)GetValue(ShowValidationAttributeErrorsProperty); }
			set { SetValue(ShowValidationAttributeErrorsProperty, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("ColumnBaseIsSorted")]
#endif
		public bool IsSorted { get { return (bool)GetValue(IsSortedProperty); } }
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseAllowFocus"),
#endif
 Category(Categories.OptionsBehavior)]
		public bool AllowFocus {
			get { return (bool)GetValue(AllowFocusProperty); }
			set { SetValue(AllowFocusProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseTabStop"),
#endif
 Category(Categories.OptionsBehavior)]
		public bool TabStop {
			get { return (bool)GetValue(TabStopProperty); }
			set { SetValue(TabStopProperty, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("ColumnBaseIsSortedBySummary")]
#endif
		public bool IsSortedBySummary {
			get { return (bool)GetValue(IsSortedBySummaryProperty); }
			internal set { this.SetValue(IsSortedBySummaryPropertyKey, value); }
		}
		[Browsable(false)]
		public ColumnSortOrder SortOrder {
			get { return (ColumnSortOrder)GetValue(SortOrderProperty); }
			set { SetValue(SortOrderProperty, value); }
		}
		[Browsable(false)]
		public int SortIndex {
			get { return (int)GetValue(SortIndexProperty); }
			set { SetValue(SortIndexProperty, value); }
		}
		[Browsable(false), XtraSerializableProperty, DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XtraResetProperty(ResetPropertyMode.None), GridUIProperty]
		public int GridRow {
			get { return BandBase.GetGridRow(this); }
			set { BandBase.SetGridRow(this, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseFieldName"),
#endif
 DefaultValue(""), Category(Categories.Data), XtraSerializableProperty, XtraResetProperty(ResetPropertyMode.None), GridSerializeAlwaysPropertyAttribute]
		public virtual string FieldName {
			get { return (string)GetValue(FieldNameProperty); }
			set { SetValue(FieldNameProperty, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Type FieldType {
			get { return (Type)GetValue(FieldTypeProperty); }
			internal set { this.SetValue(FieldTypePropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseUnboundType"),
#endif
 DefaultValue(UnboundColumnType.Bound), Category(Categories.Data), XtraSerializableProperty]
		public UnboundColumnType UnboundType {
			get { return (UnboundColumnType)GetValue(UnboundTypeProperty); }
			set { SetValue(UnboundTypeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseUnboundExpression"),
#endif
 DefaultValue(""), Category(Categories.Data), XtraSerializableProperty, GridUIProperty]
		public string UnboundExpression {
			get { return (string)GetValue(UnboundExpressionProperty); }
			set { SetValue(UnboundExpressionProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseReadOnly"),
#endif
 DefaultValue(false), Category(Categories.Editing), XtraSerializableProperty]
		public bool ReadOnly {
			get { return (bool)GetValue(ReadOnlyProperty); }
			set { SetValue(ReadOnlyProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseAllowEditing"),
#endif
 Category(Categories.Editing), XtraSerializableProperty]
		public DefaultBoolean AllowEditing {
			get { return (DefaultBoolean)GetValue(AllowEditingProperty); }
			set { SetValue(AllowEditingProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseEditSettings"),
#endif
 Category(Categories.Editing)]
		public BaseEditSettings EditSettings {
			get { return (BaseEditSettings)GetValue(EditSettingsProperty); }
			set { SetValue(EditSettingsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseEditTemplate"),
#endif
 Category(Categories.Editing)]
		public ControlTemplate EditTemplate {
			get { return (ControlTemplate)GetValue(EditTemplateProperty); }
			set { SetValue(EditTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseDisplayTemplate"),
#endif
 Category(Categories.Editing)]
		public ControlTemplate DisplayTemplate {
			get { return (ControlTemplate)GetValue(DisplayTemplateProperty); }
			set { SetValue(DisplayTemplateProperty, value); }
		}
		[Obsolete("Use the Binding property instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DefaultValue(null)]
		public BindingBase DisplayMemberBinding {
			get { return Binding; }
			set { Binding = value; }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseBinding"),
#endif
 DefaultValue(null), Category(Categories.Data)]
		public BindingBase Binding {
			get { return displayMemberBinding; }
			set {
				if(displayMemberBinding == value)
					return;
				displayMemberBinding = value;
				OnDisplayMemberBindingChanged();
			}
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseTotalSummaryContentStyle"),
#endif
 Category(Categories.Appearance)]
		public Style TotalSummaryContentStyle {
			get { return (Style)GetValue(TotalSummaryContentStyleProperty); }
			set { SetValue(TotalSummaryContentStyleProperty, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("ColumnBaseActualTotalSummaryContentStyle")]
#endif
		public Style ActualTotalSummaryContentStyle {
			get { return (Style)GetValue(ActualTotalSummaryContentStyleProperty); }
			private set { this.SetValue(ActualTotalSummaryContentStylePropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseSortMode"),
#endif
 DefaultValue(ColumnSortMode.Default), Category(Categories.Data), XtraSerializableProperty]
		public ColumnSortMode SortMode {
			get { return (ColumnSortMode)GetValue(SortModeProperty); }
			set { SetValue(SortModeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseCellStyle"),
#endif
 Category(Categories.Appearance)]
		public Style CellStyle {
			get { return (Style)GetValue(CellStyleProperty); }
			set { SetValue(CellStyleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseAutoFilterRowCellStyle"),
#endif
 Category(Categories.Appearance)]
		public Style AutoFilterRowCellStyle {
			get { return (Style)GetValue(AutoFilterRowCellStyleProperty); }
			set { SetValue(AutoFilterRowCellStyleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseNewItemRowCellStyle"),
#endif
 Category(Categories.Appearance)]
		public Style NewItemRowCellStyle {
			get { return (Style)GetValue(NewItemRowCellStyleProperty); }
			set { SetValue(NewItemRowCellStyleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseColumnHeaderContentStyle"),
#endif
 Category(Categories.Appearance)]
		public Style ColumnHeaderContentStyle {
			get { return (Style)GetValue(ColumnHeaderContentStyleProperty); }
			set { SetValue(ColumnHeaderContentStyleProperty, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("ColumnBaseActualCellStyle")]
#endif
		public Style ActualCellStyle {
			get { return (Style)GetValue(ActualCellStyleProperty); }
			private set { this.SetValue(ActualCellStylePropertyKey, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("ColumnBaseActualAutoFilterRowCellStyle")]
#endif
		public Style ActualAutoFilterRowCellStyle {
			get { return (Style)GetValue(ActualAutoFilterRowCellStyleProperty); }
			private set { this.SetValue(ActualAutoFilterRowCellStylePropertyKey, value); }
		}
		public Style ActualNewItemRowCellStyle {
			get { return (Style)GetValue(ActualNewItemRowCellStyleProperty); }
			private set { this.SetValue(ActualNewItemRowCellStylePropertyKey, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("ColumnBaseActualColumnHeaderContentStyle")]
#endif
		public Style ActualColumnHeaderContentStyle {
			get { return (Style)GetValue(ActualColumnHeaderContentStyleProperty); }
			private set { this.SetValue(ActualColumnHeaderContentStylePropertyKey, value); }
		}
		[Browsable(false)]
		public bool ActualAllowColumnFiltering {
			get { return (bool)GetValue(ActualAllowColumnFilteringProperty); }
			private set { this.SetValue(ActualAllowColumnFilteringPropertyKey, value); }
		}
		[Browsable(false)]
		public bool IsFiltered {
			get { return (bool)GetValue(IsFilteredProperty); }
			private set { this.SetValue(IsFilteredPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseBestFitWidth"),
#endif
 Category(Categories.BestFit), XtraSerializableProperty]
		public double BestFitWidth {
			get { return (double)GetValue(BestFitWidthProperty); }
			set { SetValue(BestFitWidthProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseBestFitMaxRowCount"),
#endif
 Category(Categories.BestFit), XtraSerializableProperty]
		public int BestFitMaxRowCount {
			get { return (int)GetValue(BestFitMaxRowCountProperty); }
			set { SetValue(BestFitMaxRowCountProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseBestFitMode"),
#endif
 Category(Categories.BestFit), XtraSerializableProperty]
		public BestFitMode BestFitMode {
			get { return (BestFitMode)GetValue(BestFitModeProperty); }
			set { SetValue(BestFitModeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseAllowBestFit"),
#endif
 Category(Categories.BestFit), XtraSerializableProperty]
		public DefaultBoolean AllowBestFit {
			get { return (DefaultBoolean)GetValue(AllowBestFitProperty); }
			set { SetValue(AllowBestFitProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseAllowUnboundExpressionEditor"),
#endif
Category(Categories.Data), XtraSerializableProperty]
		public bool AllowUnboundExpressionEditor {
			get { return (bool)GetValue(AllowUnboundExpressionEditorProperty); }
			set { SetValue(AllowUnboundExpressionEditorProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseBestFitArea"),
#endif
 Category(Categories.BestFit), XtraSerializableProperty]
		public BestFitArea BestFitArea {
			get { return (BestFitArea)GetValue(BestFitAreaProperty); }
			set { SetValue(BestFitAreaProperty, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("ColumnBaseIsLast")]
#endif
		public bool IsLast {
			get { return (bool)GetValue(IsLastProperty); }
			internal set { this.SetValue(IsLastPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("ColumnBaseIsFirst")]
#endif
		public bool IsFirst {
			get { return (bool)GetValue(IsFirstProperty); }
			internal set { this.SetValue(IsFirstPropertyKey, value); }
		}
		protected internal ColumnSortMode GetSortMode() {
			if(SortMode == ColumnSortMode.Default)
				return BaseEditHelper.GetRequireDisplayTextSorting(ActualEditSettings) ? ColumnSortMode.DisplayText : ColumnSortMode.Value;
			return SortMode;
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBasePrintCellStyle"),
#endif
 Category(Categories.AppearancePrint)]
		public Style PrintCellStyle {
			get { return (Style)GetValue(PrintCellStyleProperty); }
			set { SetValue(PrintCellStyleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBasePrintColumnHeaderStyle"),
#endif
 Category(Categories.AppearancePrint)]
		public Style PrintColumnHeaderStyle {
			get { return (Style)GetValue(PrintColumnHeaderStyleProperty); }
			set { SetValue(PrintColumnHeaderStyleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBasePrintTotalSummaryStyle"),
#endif
 Category(Categories.AppearancePrint)]
		public Style PrintTotalSummaryStyle {
			get { return (Style)GetValue(PrintTotalSummaryStyleProperty); }
			set { SetValue(PrintTotalSummaryStyleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseActualPrintTotalSummaryStyle"),
#endif
 Category(Categories.AppearancePrint)]
		public Style ActualPrintTotalSummaryStyle {
			get { return (Style)GetValue(ActualPrintTotalSummaryStyleProperty); }
			protected set { this.SetValue(ActualPrintTotalSummaryStylePropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseActualPrintCellStyle"),
#endif
 Category(Categories.AppearancePrint)]
		public Style ActualPrintCellStyle {
			get { return (Style)GetValue(ActualPrintCellStyleProperty); }
			protected set { this.SetValue(ActualPrintCellStylePropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseActualPrintColumnHeaderStyle"),
#endif
 Category(Categories.AppearancePrint)]
		public Style ActualPrintColumnHeaderStyle {
			get { return (Style)GetValue(ActualPrintColumnHeaderStyleProperty); }
			protected set { this.SetValue(ActualPrintColumnHeaderStylePropertyKey, value); }
		}
		public bool CopyValueAsDisplayText {
			get { return (bool)GetValue(CopyValueAsDisplayTextProperty); }
			set { SetValue(CopyValueAsDisplayTextProperty, value); }
		}
		[XtraSerializableProperty]
		public new object Tag {
			get { return GetValue(TagProperty); }
			set { SetValue(TagProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseIsSmart"),
#endif
 Category(Categories.Data), XtraSerializableProperty]
		public bool IsSmart {
			get { return (bool)GetValue(IsSmartProperty); }
			set { SetValue(IsSmartProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseAllowCellMerge"),
#endif
 Category(Categories.OptionsLayout), XtraSerializableProperty]
		public bool? AllowCellMerge {
			get { return (bool?)GetValue(AllowCellMergeProperty); }
			set { SetValue(AllowCellMergeProperty, value); }
		}
		public bool? AllowConditionalFormattingMenu {
			get { return (bool?)GetValue(AllowConditionalFormattingMenuProperty); }
			set { SetValue(AllowConditionalFormattingMenuProperty, value); }
		}
		public object EditFormCaption {
			get { return GetValue(EditFormCaptionProperty); }
			set { SetValue(EditFormCaptionProperty, value); }
		}
		public int? EditFormColumnSpan {
			get { return (int?)GetValue(EditFormColumnSpanProperty); }
			set { SetValue(EditFormColumnSpanProperty, value); }
		}
		public int? EditFormRowSpan {
			get { return (int?)GetValue(EditFormRowSpanProperty); }
			set { SetValue(EditFormRowSpanProperty, value); }
		}
		public bool EditFormStartNewRow {
			get { return (bool)GetValue(EditFormStartNewRowProperty); }
			set { SetValue(EditFormStartNewRowProperty, value); }
		}
		public bool? EditFormVisible {
			get { return (bool?)GetValue(EditFormVisibleProperty); }
			set { SetValue(EditFormVisibleProperty, value); }
		}
		public int EditFormVisibleIndex {
			get { return (int)GetValue(EditFormVisibleIndexProperty); }
			set { SetValue(EditFormVisibleIndexProperty, value); }
		}
		public DataTemplate EditFormTemplate {
			get { return (DataTemplate)GetValue(EditFormTemplateProperty); }
			set { SetValue(EditFormTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("ColumnBaseCommands")]
#endif
		public GridColumnCommands Commands { get; private set; }
		readonly NamePropertyChangeListener namePropertyChangeListener;
		EditSettingsChangedEventHandler<ColumnBase> EditSettingsChangedEventHandler { get; set; }
		public ColumnBase() {
			namePropertyChangeListener = NamePropertyChangeListener.CreateDesignTimeOnly(this, () => DesignTimeGridAdorner.UpdateDesignTimeInfo());
			EditSettingsChangedEventHandler = new EditSettingsChangedEventHandler<ColumnBase>(this, (owner, o, e) => owner.OnEditSettingsContentChanged());
			Commands = new GridColumnCommands(this);
			TotalSummariesCore = new List<SummaryItemBase>();
			GroupSummariesCore = new List<SummaryItemBase>();
		}
		protected override void HeaderCaptionChanged() {
			base.HeaderCaptionChanged();
			if(OwnerControl == null || OwnerControl.IsLoading || OwnerControl.IsDeserializing || View == null)
				return;
			View.UpdateRowData(data => data.OnHeaderCaptionChanged(), false, false);
			View.UpdateFilterPanel();
		}
		ColumnFilterInfoBase columnFilterInfo;
		internal ColumnFilterInfoBase ColumnFilterInfo {
			get {
				if(columnFilterInfo == null)
					columnFilterInfo = CreateColumnFilterInfo();
				return columnFilterInfo;
			}
		}
		internal void OnFilterPopupModeChanged() {
			columnFilterInfo = null;
			RaiseContentChanged(FilterPopupModeProperty);
		}
		protected void OnColumnFilterModeChanged() {
			if(OwnerControl != null) OwnerControl.DestroyFilterData();
			RaiseContentChanged(ColumnFilterModeProperty);
		}
		void OnAutoFilterRowDisplayTemplateChanged() {
			RaiseContentChanged(AutoFilterRowDisplayTemplateProperty);
		}
		void ReInitializeFocusedColumnIfNeeded() {
			if((OwnerControl != null) && (OwnerControl.CurrentColumn == this) && (!AllowFocus || !Visible)) {
				OwnerControl.ReInitializeCurrentColumn();
			}
		}
		protected virtual ColumnFilterInfoBase CreateColumnFilterInfo() {
			switch(FilterPopupMode) {
				case FilterPopupMode.Default:
				case FilterPopupMode.List:
					return new ListColumnFilterInfo(this);
				case FilterPopupMode.CheckedList:
					return new CheckedListColumnFilterInfo(this);
				case FilterPopupMode.Custom:
					return new CustomColumnFilterInfo(this);
			}
			return null;
		}
		protected virtual void OnUnboundTypeChanged() {
			if(Binding != null && UnboundType == UnboundColumnType.Bound && (OwnerControl == null || !OwnerControl.IsDeserializing))
				throw new ArgumentException("The UnboundType property cannot be set to UnboundColumnType.Bound if the Binding property has been specified.");
			OnDataPropertyChanged();
			OnUnboundChanged();
			if(!IsCloned)
				isAutoDetectedUnboundType = false;
		}
		void OnUnboundChanged() {
			if(OwnerControl != null) {
				OwnerControl.UpdateUnboundColumnAllowSorting(this);
				OwnerControl.OnColumnUnboundChangedPosponed();
			}
		}
		protected virtual void OnDataPropertyChanged() {
			if(ParentCollection != null)
				ParentCollection.OnColumnsChanged();
		}
		protected internal virtual void SetSortInfo(ColumnSortOrder sortOrder, bool isGrouped) {
			if(!IsSortedBySummary) {
				SortOrder = sortOrder;
			}
			this.SetValue(IsSortedPropertyKey, sortOrder != ColumnSortOrder.None);
		}
		void RebuildColumnChooserColumns() {
			Owner.RebuildColumnChooserColumns();
		}
		protected override void SetHeaderCaption() {
			object value = Header;
			if(value == null) {
				value = SplitStringHelper.SplitPascalCaseString(FieldName);
			}
			this.SetValue(HeaderCaptionPropertyKey, value);
			SetActualColumnChooserHeaderCaption();
			RebuildColumnChooserColumns();
		}
		void SetActualColumnChooserHeaderCaption() {
			ActualColumnChooserHeaderCaption = ColumnChooserHeaderCaption ?? HeaderCaption;
		}
		void UpdateCellDataValues() {
			Owner.UpdateCellDataValues();
		}
		protected internal void ClearBindingValues() {
			if(Binding != null)
				Owner.ClearBindingValues(this);
		}
		void UpdateSorting() {
			if(OwnerControl != null) {
				if(OwnerControl.InvalidSortCache.ContainsKey(FieldName)) {
					OwnerControl.UpdateSortingFromInvalidSortCache(this);
					return;
				}
				OwnerControl.ApplyColumnSortOrder(this);
			}
		}
		void UpdateGrouping(string oldFieldName) {
			if(OwnerControl != null)
				OwnerControl.UpdateGroupingFromInvalidGroupCache(this);
			UpdateGroupingCore(oldFieldName);
		}
		protected virtual void UpdateGroupingCore(string oldFieldName) { }
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseActualDataWidth"),
#endif
 CloneDetailMode(CloneDetailMode.Force)]
		public double ActualDataWidth {
			get { return (double)GetValue(ActualDataWidthProperty); }
			internal set { this.SetValue(ActualDataWidthPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseActualAdditionalRowDataWidth"),
#endif
 CloneDetailMode(CloneDetailMode.Force)]
		public double ActualAdditionalRowDataWidth {
			get { return (double)GetValue(ActualAdditionalRowDataWidthProperty); }
			internal set { this.SetValue(ActualAdditionalRowDataWidthPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseAllowSorting"),
#endif
 Category(Categories.Layout), XtraSerializableProperty]
		public DefaultBoolean AllowSorting {
			get { return (DefaultBoolean)GetValue(AllowSortingProperty); }
			set { SetValue(AllowSortingProperty, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("ColumnBaseActualAllowSorting")]
#endif
		public bool ActualAllowSorting {
			get { return (bool)GetValue(ActualAllowSortingProperty); }
			private set { this.SetValue(ActualAllowSortingPropertyKey, value); }
		}
		protected bool CanReduce {
			get {
				return GetAllowResizing() && (ActualDataWidth - MinWidth > 1.0);
			}
		}
		protected virtual bool HasSizeableColumnsFromRight() {
			if(Owner.VisibleColumns == null) return false;
			int currIndex = Owner.VisibleColumns.IndexOf(this);
			for(int i = currIndex + 1; i < Owner.VisibleColumns.Count; i++) {
				if(!CanReduce && !Owner.VisibleColumns[i].CanReduce) continue;
				if(Owner.VisibleColumns[i].GetAllowResizing() && !Owner.VisibleColumns[i].FixedWidth) return true;
			}
			if(currIndex < Owner.VisibleColumns.Count - 1 && Owner.VisibleColumns[currIndex + 1].CanReduce) return true;
			return false;
		}
		protected override bool GetActualAllowResizing(bool autoWidth) {
			if(!DesignerHelper.GetValue(this, GetAllowResizing() && Owner.AllowColumnsResizing, true) || Owner.VisibleColumns == null) return false;
			if(!autoWidth)
				return true;
			if(OwnerControl != null && OwnerControl.BandsLayoutCore != null) {
				return HasRightSibling;
			}
			if((Owner.VisibleColumns.IndexOf(this) == Owner.VisibleColumns.Count - 1) || !HasSizeableColumnsFromRight())
				return false;
			return CanReduce || HasSizeableColumnsFromRight();
		}
		protected internal virtual bool GetAllowEditing() {
			return DesignerHelper.GetValue(this, AllowEditing.GetValue(Owner.AllowEditing), false);
		}
		internal bool IsDisplayMemberBindingEditable {
			get {
				if(Binding == null)
					return true;
				Binding binding = Binding as Binding;
				if(binding != null && binding.Mode == BindingMode.TwoWay)
					return true;
#if !SL
				MultiBinding multiBinding = Binding as MultiBinding;
				if(multiBinding != null && multiBinding.Mode == BindingMode.TwoWay)
					return true;
#endif
				return false;
			}
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseCellTemplate"),
#endif
 Category(Categories.Appearance)]
		public DataTemplate CellTemplate {
			get { return (DataTemplate)GetValue(CellTemplateProperty); }
			set { SetValue(CellTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseCellTemplateSelector"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Category(Categories.Appearance)]
		public DataTemplateSelector CellTemplateSelector {
			get { return (DataTemplateSelector)GetValue(CellTemplateSelectorProperty); }
			set { SetValue(CellTemplateSelectorProperty, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("ColumnBaseActualCellTemplateSelector")]
#endif
		public DataTemplateSelector ActualCellTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ActualCellTemplateSelectorProperty); }
			private set { this.SetValue(ActualCellTemplateSelectorPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseFilterEditorHeaderTemplate"),
#endif
Category(Categories.Appearance)]
		public DataTemplate FilterEditorHeaderTemplate {
			get { return (DataTemplate)GetValue(FilterEditorHeaderTemplateProperty); }
			set { SetValue(FilterEditorHeaderTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseHeaderCustomizationAreaTemplate"),
#endif
 Category(Categories.Appearance)]
		public DataTemplate HeaderCustomizationAreaTemplate {
			get { return (DataTemplate)GetValue(HeaderCustomizationAreaTemplateProperty); }
			set { SetValue(HeaderCustomizationAreaTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseHeaderCustomizationAreaTemplateSelector"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Category(Categories.Appearance)]
		public DataTemplateSelector HeaderCustomizationAreaTemplateSelector {
			get { return (DataTemplateSelector)GetValue(HeaderCustomizationAreaTemplateSelectorProperty); }
			set { SetValue(HeaderCustomizationAreaTemplateSelectorProperty, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("ColumnBaseActualHeaderCustomizationAreaTemplateSelector")]
#endif
		public DataTemplateSelector ActualHeaderCustomizationAreaTemplateSelector {
			get { return (DataTemplateSelector)base.GetValue(ActualHeaderCustomizationAreaTemplateSelectorProperty); }
			private set { this.SetValue(ActualHeaderCustomizationAreaTemplateSelectorPropertyKey, value); }
		}
		protected internal IList<SummaryItemBase> TotalSummariesCore { get; set; }
		protected internal IList<SummaryItemBase> GroupSummariesCore { get; set; }
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("ColumnBaseHasTotalSummaries")]
#endif
		public bool HasTotalSummaries {
			get { return (bool)GetValue(HasTotalSummariesProperty); }
			private set { this.SetValue(HasTotalSummariesPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("ColumnBaseTotalSummaries")]
#endif
		public IList<GridTotalSummaryData> TotalSummaries {
			get { return (IList<GridTotalSummaryData>)GetValue(TotalSummariesProperty); }
			private set { this.SetValue(TotalSummariesPropertyKey, value); }
		}
		[Browsable(false)]
		public bool ShouldSerializeTotalSummaries(XamlDesignerSerializationManager manager) {
			return false;
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("ColumnBaseTotalSummaryText")]
#endif
		public string TotalSummaryText {
			get { return (string)GetValue(TotalSummaryTextProperty); }
			private set { this.SetValue(TotalSummaryTextPropertyKey, value); }
		}
		[Category(Categories.OptionsFilter), Browsable(false), CloneDetailMode(CloneDetailMode.Skip)]
		public object AutoFilterValue {
			get { return GetValue(AutoFilterValueProperty); }
			set { SetValue(AutoFilterValueProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseAutoFilterCondition"),
#endif
 Category(Categories.OptionsFilter), XtraSerializableProperty]
		public AutoFilterCondition AutoFilterCondition {
			get { return (AutoFilterCondition)GetValue(AutoFilterConditionProperty); }
			set { SetValue(AutoFilterConditionProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseAllowAutoFilter"),
#endif
 Category(Categories.OptionsFilter), XtraSerializableProperty]
		public bool AllowAutoFilter {
			get { return (bool)GetValue(AllowAutoFilterProperty); }
			set { SetValue(AllowAutoFilterProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseImmediateUpdateAutoFilter"),
#endif
 Category(Categories.OptionsFilter), XtraSerializableProperty]
		public bool ImmediateUpdateAutoFilter {
			get { return (bool)GetValue(ImmediateUpdateAutoFilterProperty); }
			set { SetValue(ImmediateUpdateAutoFilterProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseAllowColumnFiltering"),
#endif
 Category(Categories.OptionsFilter), XtraSerializableProperty]
		public DefaultBoolean AllowColumnFiltering {
			get { return (DefaultBoolean)GetValue(AllowColumnFilteringProperty); }
			set { SetValue(AllowColumnFilteringProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseFilterPopupMode"),
#endif
 Category(Categories.OptionsFilter), XtraSerializableProperty]
		public FilterPopupMode FilterPopupMode {
			get { return (FilterPopupMode)GetValue(FilterPopupModeProperty); }
			set { SetValue(FilterPopupModeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseColumnFilterMode"),
#endif
 Category(Categories.OptionsFilter), XtraSerializableProperty]
		public ColumnFilterMode ColumnFilterMode {
			get { return (ColumnFilterMode)GetValue(ColumnFilterModeProperty); }
			set { SetValue(ColumnFilterModeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseImmediateUpdateColumnFilter"),
#endif
 Category(Categories.OptionsFilter), XtraSerializableProperty]
		public bool ImmediateUpdateColumnFilter {
			get { return (bool)GetValue(ImmediateUpdateColumnFilterProperty); }
			set { SetValue(ImmediateUpdateColumnFilterProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseColumnFilterPopupMaxRecordsCount"),
#endif
 Category(Categories.OptionsFilter), XtraSerializableProperty]
		public int ColumnFilterPopupMaxRecordsCount {
			get { return (int)GetValue(ColumnFilterPopupMaxRecordsCountProperty); }
			set { SetValue(ColumnFilterPopupMaxRecordsCountProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseShowEmptyDateFilter"),
#endif
 Category(Categories.OptionsFilter), XtraSerializableProperty]
		public bool ShowEmptyDateFilter {
			get { return (bool)GetValue(ShowEmptyDateFilterProperty); }
			set { SetValue(ShowEmptyDateFilterProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseAutoFilterRowDisplayTemplate"),
#endif
 Category(Categories.Appearance)]
		public ControlTemplate AutoFilterRowDisplayTemplate {
			get { return (ControlTemplate)GetValue(AutoFilterRowDisplayTemplateProperty); }
			set { this.SetValue(AutoFilterRowDisplayTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseAutoFilterRowEditTemplate"),
#endif
 Category(Categories.Appearance)]
		public ControlTemplate AutoFilterRowEditTemplate {
			get { return (ControlTemplate)GetValue(AutoFilterRowEditTemplateProperty); }
			set { this.SetValue(AutoFilterRowEditTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("ColumnBaseRoundDateTimeForColumnFilter")]
#endif
		public bool RoundDateTimeForColumnFilter {
			get { return (bool)GetValue(RoundDateTimeForColumnFilterProperty); }
			set { SetValue(RoundDateTimeForColumnFilterProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseCustomColumnFilterPopupTemplate"),
#endif
 Category(Categories.OptionsFilter)]
		public DataTemplate CustomColumnFilterPopupTemplate {
			get { return (DataTemplate)GetValue(CustomColumnFilterPopupTemplateProperty); }
			set { SetValue(CustomColumnFilterPopupTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseShowAllTableValuesInCheckedFilterPopup"),
#endif
 XtraSerializableProperty, Category(Categories.OptionsFilter)]
		public DefaultBoolean ShowAllTableValuesInCheckedFilterPopup {
			get { return (DefaultBoolean)GetValue(ShowAllTableValuesInCheckedFilterPopupProperty); }
			set { SetValue(ShowAllTableValuesInCheckedFilterPopupProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseShowAllTableValuesInFilterPopup"),
#endif
 XtraSerializableProperty, Category(Categories.OptionsFilter)]
		public DefaultBoolean ShowAllTableValuesInFilterPopup {
			get { return (DefaultBoolean)GetValue(ShowAllTableValuesInFilterPopupProperty); }
			set { SetValue(ShowAllTableValuesInFilterPopupProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseShowInColumnChooser"),
#endif
 Category(Categories.OptionsColumn), XtraSerializableProperty]
		public bool ShowInColumnChooser {
			get { return (bool)GetValue(ShowInColumnChooserProperty); }
			set { SetValue(ShowInColumnChooserProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseColumnChooserHeaderCaption"),
#endif
 Category(Categories.Data), TypeConverter(typeof(ObjectConverter)), XtraSerializableProperty]
		public object ColumnChooserHeaderCaption {
			get { return GetValue(ColumnChooserHeaderCaptionProperty); }
			set { SetValue(ColumnChooserHeaderCaptionProperty, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("ColumnBaseActualColumnChooserHeaderCaption")]
#endif
		public object ActualColumnChooserHeaderCaption {
			get { return GetValue(ActualColumnChooserHeaderCaptionProperty); }
			private set { this.SetValue(ActualColumnChooserHeaderCaptionPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("ColumnBaseAllowSearchPanel"),
#endif
 CloneDetailMode(CloneDetailMode.Skip)]
		public DefaultBoolean AllowSearchPanel {
			get { return (DefaultBoolean)GetValue(AllowSearchPanelProperty); }
			set { SetValue(AllowSearchPanelProperty, value); }
		}
		internal bool ActualAllowSearchPanel { get { return AllowSearchPanel.ToBoolean(Visible); } }
		internal BaseEditSettings ActualEditSettingsCore { get; private set; }
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("ColumnBaseActualEditSettings")]
#endif
		public BaseEditSettings ActualEditSettings {
			get { return (BaseEditSettings)GetValue(ActualEditSettingsProperty); }
			private set { this.SetValue(ActualEditSettingsPropertyKey, value); }
		}
		[DXBrowsable(false)]
		public HorizontalAlignment ActualHorizontalContentAlignment {
			get { return (HorizontalAlignment)GetValue(ActualHorizontalContentAlignmentProperty); }
			private set { this.SetValue(ActualHorizontalContentAlignmentPropertyKey, value); }
		}
		internal override void UpdateViewInfo(bool updateDataPropertiesOnly = false) {
			base.UpdateViewInfo(updateDataPropertiesOnly);
			UpdateColumnTypeProperties(null);
			UpdateActualHorizontalContentAlignment();
			UpdateTotalSummaries();
			UpdateActualAllowSorting();
			if(updateDataPropertiesOnly) return;
			UpdateActualShowValidationAttributeErrors();
			UpdateAppearance();
			UpdateActualAllowColumnFiltering();
		}
		internal void UpdateColumnTypeProperties(DataProviderBase dataProvider) {
			UpdateFieldType(dataProvider);
			UpdateActualEditSettings();
		}
		void UpdateActualHorizontalContentAlignment() {
			if(ActualEditSettings == null)
				return;
			ActualHorizontalContentAlignment = DevExpress.Xpf.Editors.Native.EditSettingsHorizontalAlignmentHelper.GetHorizontalAlignment(ActualEditSettings.HorizontalContentAlignment, Owner.GetDefaultColumnAlignment(this));
		}
		Locker filterUpdateLocker = new Locker();
		void OnAutoFilterValueChanged() {
			RaiseContentChanged(AutoFilterValueProperty);
			filterUpdateLocker.DoIfNotLocked(() => UpdateAutoFilter());
		}
		internal void SetAutoFilterValue(object value) {
			if(AutoFilterValue == value) return;
			filterUpdateLocker.DoLockedAction(() => AutoFilterValue = value);
			View.EnqueueImmediateAction(() => UpdateAutoFilter());
		}
		internal void ApplyColumnFilter(CriteriaOperator op) {
			if(OwnerControl == null)
				return;
			if(!object.ReferenceEquals(op, null))
				OwnerControl.MergeColumnFilters(op);
			else
				OwnerControl.ClearColumnFilter(this);
		}
		void UpdateFieldType(DataProviderBase dataProvider) {
			Type type = Owner.GetColumnType(this, dataProvider);
			FieldType = type ?? typeof(object);
		}
		bool lockAutoFilterUpdate;
		internal void UpdateAutoFilter() {
			if(!IsInitialized) return;
			Owner.LockEditorClose = true;
			lockAutoFilterUpdate = true;
			AutoFilterCondition condition = ResolveAutoFilterCondition();
			try {
				ApplyColumnFilter(View != null && AutoFilterValue != null && !string.IsNullOrEmpty(AutoFilterValue.ToString()) ?
					(Owner as DataViewBase).CreateAutoFilterCriteria(FieldName, condition, AutoFilterValue) : null);
			}
			finally {
				Owner.LockEditorClose = false;
				lockAutoFilterUpdate = false;
			}
		}
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			if(AutoFilterValue != null)
				UpdateAutoFilter();
			else
				UpdateAutoFilterValue();
		}
		protected AutoFilterCondition ResolveAutoFilterCondition() {
			if(ActualEditSettings is CheckEditSettings) {
				return AutoFilterCondition.Equals;
			}
			if(AutoFilterCondition != AutoFilterCondition.Default)
				return AutoFilterCondition;
			if(ColumnFilterMode == ColumnFilterMode.DisplayText) {
				return AutoFilterCondition.Like;
			}
			if(ActualEditSettings is DateEditSettings ||
				ActualEditSettings is ImageEditSettings ||
#if !SL
 ActualEditSettings is ProgressBarEditSettings ||
#endif
 ActualEditSettings is TrackBarEditSettings ||
				ActualEditSettings is LookUpEditSettingsBase) {
				return AutoFilterCondition.Equals;
			}
			return AutoFilterCondition.Like;
		}
		internal void UpdateAutoFilterValue() {
			if(!IsInitialized || View == null || View.DataControl == null) return;
			CriteriaOperator op = View.DataControl.GetColumnFilterCriteria(this);
			IsFiltered = !object.ReferenceEquals(op, null);
			if(lockAutoFilterUpdate) {
				return;
			}
			filterUpdateLocker.DoLockedAction(() => {
				if (Owner is DataViewBase)
					AutoFilterValue = ((DataViewBase)Owner).GetAutoFilterValue(this, op);
				}
			);
		}
		internal object GetAutoFilterValue(CriteriaOperator op) {
			BinaryOperator binaryOp = op as BinaryOperator;
			if(!ReferenceEquals(null, binaryOp)) {
				OperandValue ov = binaryOp.RightOperand as OperandValue;
				if(object.ReferenceEquals(ov, null))
					return null;
				object value = ov.Value;
				if(value == null)
					return null;
				AutoFilterCondition condition = ResolveAutoFilterCondition();
				if(condition == AutoFilterCondition.Equals && binaryOp.OperatorType == BinaryOperatorType.Equal)
					return value;
				return null;
			}
			FunctionOperator fop = op as FunctionOperator;
			if(!ReferenceEquals(null, fop)) {
				if(fop.Operands.Count != 2)
					return null;
				OperandValue ov = fop.Operands[1] as OperandValue;
				if(object.ReferenceEquals(ov, null))
					return null;
				string value = ov.Value as string;
				if(string.IsNullOrEmpty(value))
					return null;
				AutoFilterCondition condition = ResolveAutoFilterCondition();
				if(condition == AutoFilterCondition.Contains && fop.OperatorType == FunctionOperatorType.Contains) {
					return value;
				}
				else if(condition == Grid.AutoFilterCondition.Like && fop.OperatorType == FunctionOperatorType.Contains) {
					return '%' + value;
				}
				else if(condition == Grid.AutoFilterCondition.Like && fop.OperatorType == FunctionOperatorType.StartsWith) {
					return value;
				}
			}
			GroupOperator gop = op as GroupOperator;
			if(!ReferenceEquals(null, gop)) {
				if(gop.Operands.Count != 2)
					return null;
				BinaryOperator binOp = gop.Operands[0] as BinaryOperator;
				if(object.ReferenceEquals(binOp, null))
					return null;
				OperandValue ov = binOp.RightOperand as OperandValue;
				if(object.ReferenceEquals(ov, null))
					return null;
				if(ov.Value is DateTime) {
					BinaryOperator binOp2 = gop.Operands[1] as BinaryOperator;
					if(object.ReferenceEquals(binOp2, null))
						return null;
					OperandValue ov2 = binOp2.RightOperand as OperandValue;
					if(object.ReferenceEquals(ov2, null))
						return null;
					if(binOp.OperatorType == BinaryOperatorType.GreaterOrEqual && binOp2.OperatorType == BinaryOperatorType.Less) {
						if((DateTime)ov.Value == ((DateTime)ov2.Value).AddDays(-1))
							return ov.Value;
					}
				}
				return null;
			}
			return null;
		}
		internal bool GetShowAllTableValuesInCheckedFilterPopup() {
			if(ShowAllTableValuesInCheckedFilterPopup == DefaultBoolean.Default)
				return Owner.ShowAllTableValuesInCheckedFilterPopup;
			return ShowAllTableValuesInCheckedFilterPopup == DefaultBoolean.True;
		}
		internal bool GetShowAllTableValuesInFilterPopup() {
			if(ShowAllTableValuesInFilterPopup == DefaultBoolean.Default)
				return Owner.ShowAllTableValuesInFilterPopup;
			return ShowAllTableValuesInFilterPopup == DefaultBoolean.True;
		}
		bool CalcActualAllowSorting() {
			if(!Owner.AllowSortColumn(this))
				return false;
			return GetActualAllowSorting();
		}
		internal bool GetActualAllowSorting() {
			return AllowSorting.GetValue(Owner.AllowSorting);
		}
		internal virtual bool GetActualAllowGroupingCore() {
			return GetActualAllowSorting();
		}
		protected virtual void UpdateActualAllowSorting() {
			ActualAllowSorting = CalcActualAllowSorting();
		}
		internal virtual void UpdateAppearance() {
			ActualCellStyle = Owner.GetActualCellStyle(this);
			ActualAutoFilterRowCellStyle = AutoFilterRowCellStyle == null ? Owner.AutoFilterRowCellStyle : AutoFilterRowCellStyle;
			ActualNewItemRowCellStyle = NewItemRowCellStyle == null ? Owner.NewItemRowCellStyle : NewItemRowCellStyle;
			ActualColumnHeaderContentStyle = ColumnHeaderContentStyle == null ? Owner.ColumnHeaderContentStyle : ColumnHeaderContentStyle;
			ActualTotalSummaryContentStyle = TotalSummaryContentStyle == null ? Owner.TotalSummaryContentStyle : TotalSummaryContentStyle;
			UpdatePrintAppearance();
		}
		partial void UpdatePrintAppearance();
		internal void UpdateActualAllowColumnFiltering() {
			ActualAllowColumnFiltering = AllowColumnFiltering.GetValue(Owner.AllowColumnFiltering);
		}
		void UpdateShowEditFilterButton(DependencyPropertyChangedEventArgs e) {
			Owner.UpdateShowEditFilterButton((DefaultBoolean)e.NewValue, (DefaultBoolean)e.OldValue);
		}
		internal void UpdateTotalSummaries() {
			TotalSummaryText = GetTotalSummaryText(GetValue);
		}
		internal string GetTotalSummaryText(Func<SummaryItemBase, object> getSummaryValue) {
			if(!HasTotalSummaries && TotalSummariesCore.Count == 0) return " " ;
			HasTotalSummaries = TotalSummariesCore.Count > 0;
			List<GridTotalSummaryData> summaryData = new List<GridTotalSummaryData>();
			foreach(DevExpress.Xpf.Grid.SummaryItemBase item in TotalSummariesCore) {
				object summaryValue = getSummaryValue(item);
				summaryData.Add(new GridTotalSummaryData(item, summaryValue, Owner.GetColumn(item.FieldName)));
			}
			StringBuilder sb = new StringBuilder();
			for(int i = 0; i < summaryData.Count; i++) {
				if(i > 0)
					sb.AppendLine();
				sb.Append(GetSummaryText(summaryData[i]));
			}
			TotalSummaries = summaryData;
			string totalSummariesCaption = sb.ToString();
			return totalSummariesCaption == string.Empty ? " " : totalSummariesCaption;
		}
		internal static string GetSummaryText(GridTotalSummaryData data, bool forceShowColumnName) {
			return GetSummaryText(data, forceShowColumnName, data.Column != null ? data.Column.DisplayFormat : string.Empty, false);
		}
		internal string DisplayFormat { get { return ActualEditSettings != null ? FormatStringConverter.GetDisplayFormat(ActualEditSettings.DisplayFormat) : string.Empty; } }
		internal static string GetSummaryText(GridTotalSummaryData data) {
			bool forceShowColumnName = false;
			data.Column.With(column => column.View).Do(view => forceShowColumnName = view.ForceShowTotalSummaryColumnName);
			return GetSummaryText(data, forceShowColumnName);
		}
		internal static string GetSummaryText(GridTotalSummaryData data, string forceDisplayFormat) {
			return GetSummaryText(data, false, forceDisplayFormat, true);
		}
		internal static string GetSummaryText(GridTotalSummaryData data, bool forceShowColumnName, string forceDisplayFormat, bool useForceDisplayFormat) {
			if(forceShowColumnName)
				return data.Item.GetFooterDisplayTextWithColumnName(CultureInfo.CurrentCulture, GetSummaryDisplayName(data.Column, data.Item), data.Value, forceDisplayFormat);
			return data.Item.GetFooterDisplayText(CultureInfo.CurrentCulture, GetSummaryDisplayName(data.Column, data.Item), data.Value, forceDisplayFormat, useForceDisplayFormat);
		}
		protected object GetValue(DevExpress.Xpf.Grid.SummaryItemBase summary) { return Owner.GetTotalSummaryValue(summary); }
		internal bool ShouldRepopulateColumns { get { return UnboundType != UnboundColumnType.Bound || FieldName.Contains("."); } }
		protected internal bool IsNameOrFieldOrCaption(string fieldName) {
			return FieldName == fieldName;
		}
		internal protected override DataTemplate GetActualTemplate() {
			return HeaderTemplate ?? Owner.ColumnHeaderTemplate;
		}
		internal protected override DataTemplateSelector GetActualTemplateSelector() {
			return HeaderTemplateSelector ?? Owner.ColumnHeaderTemplateSelector;
		}
		internal void UpdateActualHeaderCustomizationAreaTemplateSelector() {
			ActualHeaderCustomizationAreaTemplateSelector = new ActualTemplateSelectorWrapper(
				HeaderCustomizationAreaTemplateSelector ?? Owner.ColumnHeaderCustomizationAreaTemplateSelector,
				HeaderCustomizationAreaTemplate ?? Owner.ColumnHeaderCustomizationAreaTemplate);
		}
		internal void UpdateActualCellTemplateSelector() {
			DataTemplate template = CellTemplate != null ? CellTemplate : Owner.GetActualCellTemplate();
			ActualCellTemplateSelector = new ActualTemplateSelectorWrapper(
				CellTemplateSelector != null ? CellTemplateSelector : Owner.CellTemplateSelector,
				template
			);
			RaiseContentChanged(null);
		}
		internal void UpdateHasTopElement() {
			bool hasTopElement = false;
			if(ParentBand != null)
				hasTopElement = ParentBand.ActualRows.IndexOf(BandRow) > 0;
			if(HasTopElement != hasTopElement)
				HasTopElement = hasTopElement;
			else
				RaiseHasTopElementChanged();
		}
		internal void UpdateHasBottomElement() {
			if(ParentBand != null && ParentBand.ActualRows.Count > 0)
				HasBottomElement = ParentBand.ActualRows.IndexOf(BandRow) != ParentBand.ActualRows.Count - 1;
			else HasBottomElement = false;
		}
		void OnAllowSearchPanelChanged() {
			UpdateSearchInfo();
		}
		protected override IEnumerator LogicalChildren {
			get {
				if(EditSettings != null && EditSettings.Parent == this)
					return (new object[] { EditSettings }).GetEnumerator();
				return (new object[0]).GetEnumerator();
			}
		}
		protected virtual void OnEditSettingsChanged(BaseEditSettings oldValue) {
			UpdateActualEditSettings();
			UpdateActualCellTemplateSelector();
			RaiseContentChanged(EditSettingsProperty);
			DesignTimeGridAdorner.UpdateDesignTimeInfo();
		}
		protected void OnEditSettingsContentChanged() {
			UpdateActualHorizontalContentAlignment();
		}
		protected virtual void OnDisplayTemplateChanged() {
			RaiseContentChanged(DisplayTemplateProperty);
		}
		protected override void OnHasRightSiblingChanged() {
			UpdateContentLayout();
			base.OnHasRightSiblingChanged();
		}
		protected override void OnHasLeftSiblingChanged() {
			UpdateContentLayout();
			base.OnHasLeftSiblingChanged();
		}
		BaseEditSettings defaultSettings;
		void UpdateActualEditSettings() {
			if(EditSettings == null) {
				BaseEditSettings newSettings = Owner.CreateDefaultEditSettings(this);
				if(defaultSettings == null || newSettings.GetType() != defaultSettings.GetType()) {
					defaultSettings = newSettings;
				}
				UpdateActualEditSettingsAndRaiseContentChanged(defaultSettings);
			}
			else {
				UpdateActualEditSettingsAndRaiseContentChanged(EditSettings);
			}
		}
		void UpdateActualEditSettingsAndRaiseContentChanged(BaseEditSettings editSettings) {
			if(ActualEditSettings == editSettings)
				return;
			ActualEditSettings = editSettings;
			TestActualEditSettingsOnSynchronizeCurrentItem();
#if SILVERLIGHT
#pragma warning disable 618
				UpdateActualEditSettingsInResources();
#pragma warning restore 618
#endif
			RaiseContentChanged(null);
		}
		protected internal void TestActualEditSettingsOnSynchronizeCurrentItem() {
			IItemsProviderOwner owner = ActualEditSettings as IItemsProviderOwner;
			if(owner != null && owner.IsSynchronizedWithCurrentItem)
				throw new NotSupportedException("The IsSynchronizedWithCurrentItem property cannot be set to True if an editor is used as an in-place editor.");
		}
		#region IColumnInfo Members
		string IColumnInfo.FieldName { get { return FieldName; } }
		ColumnSortOrder IColumnInfo.SortOrder { get { return SortOrder; } }
		UnboundColumnType IColumnInfo.UnboundType { get { return UnboundType; } }
		bool IColumnInfo.ReadOnly { get { return ReadOnly; } }
		#endregion
		void OnSortIndexChanged() {
			if(OwnerControl != null)
				OwnerControl.ApplyColumnSortIndex(this);
		}
		void OnSortOrderChanged() {
			if(OwnerControl != null)
				OwnerControl.ApplyColumnSortOrder(this);
			RaiseContentChanged(SortOrderProperty);
		}
		void OnFieldTypeChanged() {
			if(FilterPopupMode == FilterPopupMode.Default)
				OnFilterPopupModeChanged();
		}
		void OnDeserializeAllowPropertyInternal(AllowPropertyEventArgs e) {
			e.Allow = OnDeserializeAllowProperty(e);
		}
		void OnCustomShouldSerializeProperty(CustomShouldSerializePropertyEventArgs e) {
			if(e.DependencyProperty == ActualWidthProperty) {
				e.CustomShouldSerialize = true;
				return;
			}
			if(e.DependencyProperty == FieldNameProperty) {
				e.CustomShouldSerialize = true;
				return;
			}
		}
		protected virtual bool OnDeserializeAllowProperty(AllowPropertyEventArgs e) {
			return OwnerControl != null ? OwnerControl.OnDeserializeAllowProperty(e) : false;
		}
		protected virtual void OnDeserializeProperty(XtraPropertyInfoEventArgs e) {
			if(e.DependencyProperty == ActualWidthProperty) {
				if(e.Info != null) {
					SetActualWidth(Convert.ToDouble(e.Info.Value, CultureInfo.InvariantCulture));
				}
				e.Handled = true;
			}
		}
		protected internal virtual string GetValidationAttributesErrorText(object value, int rowHandle) {
			return OwnerControl == null ? null : OwnerControl.GetValidationAttributesErrorText(value, FieldName, rowHandle);
		}
		partial void UpdatePrintAppearance() {
			UpdatePrintAppearanceCore();
		}
		void UpdateShowValidationAttributeErrors() {
			UpdateActualShowValidationAttributeErrors();
			if(View != null)
				View.UpdateCellDataErrors();
		}
		internal void UpdateActualShowValidationAttributeErrors() {
			ActualShowValidationAttributeErrors = CalcActualShowValidationAttributeErrors();
		}
		bool CalcActualShowValidationAttributeErrors() {
			return ShowValidationAttributeErrors.GetValue(Owner.ShowValidationAttributeErrors);
		}
		internal protected override void UpdateActualHeaderToolTipTemplate() {
			if(HeaderToolTipTemplate != null) {
				ActualHeaderToolTipTemplate = HeaderToolTipTemplate;
			} else {
				ActualHeaderToolTipTemplate = View == null ? null : View.ColumnHeaderToolTipTemplate;
			}
		}
		protected virtual void UpdatePrintAppearanceCore() {
			UpdateActualPrintProperties(Owner as DataViewBase);
		}
		internal void UpdateActualPrintProperties(DataViewBase view) {
			if(view == null) return;
			ActualPrintCellStyle = PrintCellStyle == null ? view.PrintCellStyle : PrintCellStyle;
			ActualPrintColumnHeaderStyle = PrintColumnHeaderStyle == null ? view.With(v => v as ITableView).With(tb => tb.PrintColumnHeaderStyle) : PrintColumnHeaderStyle;
			ActualPrintTotalSummaryStyle = PrintTotalSummaryStyle == null ? view.PrintTotalSummaryStyle : PrintTotalSummaryStyle;
		}
		void UpdateEditorButtonVisibilities() {
			if(View != null)
				View.UpdateEditorButtonVisibilities();
		}
		#region IDefaultEditorViewInfo Members
		HorizontalAlignment IDefaultEditorViewInfo.DefaultHorizontalAlignment {
			get { return Owner.GetDefaultColumnAlignment(this); }
		}
		bool IDefaultEditorViewInfo.HasTextDecorations {
			get {
				var tableView = View as ITableView;
				return tableView != null && ((ConditionalFormattingMaskHelper.GetConditionsMask(tableView.FormatConditions.GetInfoByFieldName(FieldName)) | ConditionalFormattingMaskHelper.GetConditionsMask(tableView.FormatConditions.GetInfoByFieldName(string.Empty))) & ConditionalFormatMask.TextDecorations) > 0;
			}
		}
		#endregion
		#region IDataColumnInfo
		string IDataColumnInfo.Caption { get { return Convert.ToString(HeaderCaption); } }
		string IDataColumnInfo.FieldName { get { return FieldName; } }
		string IDataColumnInfo.Name { get { return Name; } }
		string IDataColumnInfo.UnboundExpression { get { return UnboundExpression; } }
		DataControllerBase IDataColumnInfo.Controller {
			get {
				return View != null ? View.GetDataControllerForUnboundColumnsCore() : null;
			}
		}		
		List<IDataColumnInfo> IDataColumnInfo.Columns {
			get {
				List<IDataColumnInfo> list = new List<IDataColumnInfo>();
				if(ParentCollection != null) {
					foreach(ColumnBase colum in ParentCollection) {
						if(colum != this)
							list.Add(colum);
					}
				}
				return list;
			}
		}
		#endregion
		#region IInplaceEditorColumn Members
		BaseEditSettings IInplaceEditorColumn.EditSettings { get { return ActualEditSettingsCore; } }
		DataTemplateSelector IInplaceEditorColumn.EditorTemplateSelector { get { return ActualCellTemplateSelector; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		#endregion
		protected internal virtual object GetWaitIndicator() { return null; }
		#region DisplayMemberBinding
		DisplayMemberBindingCalculator displayMemberBindingCalculator = null;
		internal DisplayMemberBindingCalculator DisplayMemberBindingCalculator {
			get {
				if(displayMemberBindingCalculator != null && displayMemberBindingCalculator.GridView == View) {
					return displayMemberBindingCalculator;
				}
				return CreateDisplayMemberBindingCalculator();
			}
		}
		DisplayMemberBindingCalculator CreateDisplayMemberBindingCalculator() {
			if(View == null || Binding == null)
				return null;
			displayMemberBindingCalculator = new DisplayMemberBindingCalculator(View, this);
			return displayMemberBindingCalculator;
		}
#if DEBUGTEST
		public static bool AllowOptimizedDisplayMemberBinding = true;
#endif
		SimpleBindingProcessor simpleBindingProcessor;
		internal ISimpleBindingProcessor SimpleBindingProcessor { get { return simpleBindingProcessor; } }
		internal bool IsSimpleBindingEnabled { get { return simpleBindingProcessor != null && simpleBindingProcessor.IsEnabled; } }
		internal BindingBase ActualBinding { get { return IsSimpleBindingEnabled ? null : Binding; } }
		internal bool IsCloned { get; set; }
		protected virtual void OnDisplayMemberBindingChanged() {
			if(Binding == null)
				SetUnboundType(UnboundColumnType.Bound);
			if(!IsCloned)
				DisplayMemberBindingCalculator.ValidateBinding(Binding);
#if DEBUGTEST
			if(AllowOptimizedDisplayMemberBinding)
#endif
			simpleBindingProcessor = new SimpleBindingProcessor(this);
			ValidateSimpleBinding(SimpleBindingState.Bidning);
			SetUnboundType();
			if(string.IsNullOrEmpty(FieldName))
				FieldName = PatchBindingName(DisplayMemberBindingCalculator.GetBindingName(Binding));
			else
				OnFieldNameChanged(FieldName);
		}
		string PatchBindingName(string name, int uid = 0) {
			if(ParentCollection == null || string.IsNullOrEmpty(name))
				return name;
			string patchedName = uid > 0 ? name + uid : name;
			if(ParentCollection[patchedName] == null)
				return patchedName;
			return PatchBindingName(name, uid + 1);
		}
		protected virtual void OnFieldNameChanged(string oldValue) {
			SetHeaderCaption();
			if(ShouldRepopulateColumns)
				OnUnboundChanged();
			UpdateCellDataValues();
			UpdateSorting();
			UpdateGrouping(oldValue);
			OnDataPropertyChanged();
			ValidateSimpleBinding(SimpleBindingState.Field);
		}
		internal void SetUnboundType() {
			displayMemberBindingCalculator = null;
			if(Binding != null && DisplayMemberBindingCalculator != null) {
				SetUnboundType(DisplayMemberBindingCalculator.GetUnboundColumnType());
			}
		}
		internal bool isAutoDetectedUnboundType = true;
		internal void SetUnboundType(UnboundColumnType unboundType) {
			if(!isAutoDetectedUnboundType) return;
			UnboundType = unboundType;
			isAutoDetectedUnboundType = true;
		}
		void ValidateSimpleBinding(SimpleBindingState state) {
			if(simpleBindingProcessor != null && !IsCloned) {
				bool oldValue = simpleBindingProcessor.IsEnabled;
				simpleBindingProcessor.Validate(state);
				if(oldValue != simpleBindingProcessor.IsEnabled)
					UpdateCellDataValues();
			}
		}
		internal void UpdateDisplayMemberBindingData() {
			ValidateSimpleBinding(SimpleBindingState.Data);
		}
		internal void UpdateSimpleBindingLanguage() {
			if(simpleBindingProcessor != null)
				simpleBindingProcessor.ResetLanguage();
		}
		#endregion
		public event PropertyChangedEventHandler PropertyChanged;
		void OnPropertyChanged(string propertyName) {
			if(PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#region master detail
		protected override BaseColumn GetOriginationColumn() {
			return OwnerControl == null || OwnerControl.IsOriginationDataControl() ?
				null :
				CloneDetailHelper.SafeGetDependentCollectionItem<ColumnBase>(this, OwnerControl.ColumnsCore, OwnerControl.GetOriginationDataControl().ColumnsCore);
		}
		internal override Func<DataControlBase, BaseColumn> CreateCloneAccessor() {
			if(ParentBand != null)
				return BandWalker.CreateColumnCloneAccessor(this);
			return dc => CloneDetailHelper.SafeGetDependentCollectionItem<ColumnBase>(this, OwnerControl.ColumnsCore, dc.ColumnsCore);
		}
		internal override DataControlBase GetNotifySourceControl() {
			return OwnerControl;
		}
		#endregion
		protected override void OnFixedChanged() {
			base.OnFixedChanged();
			DesignTimeGridAdorner.OnColumnsLayoutChanged();
		}
		protected override void OnLayoutPropertyChanged() {
			Owner.CalcColumnsLayout();
		}
		void OnActualCellStyleCahnged() {
			UpdateContentLayout();
			RaiseContentChanged(ActualCellStyleProperty);
		}
		internal protected abstract void OnValidation(GridRowValidationEventArgs e);
		void OnAllowCellMergeChanged(bool? oldValue) {
			if(OwnerControl == null) return;
			OwnerControl.UpdateColumnCellMergeCounter(oldValue, AllowCellMerge);
		}
		object CoerceFieldName(object fieldName) {
			if(fieldName == null)
				return string.Empty;
			return fieldName;
		}
	}
}
