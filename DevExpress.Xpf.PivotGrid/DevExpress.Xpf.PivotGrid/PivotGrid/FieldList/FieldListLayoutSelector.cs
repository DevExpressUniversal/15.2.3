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
using System.Windows;
using System.Windows.Controls;
#if SL
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using DevExpress.Xpf.Core.WPFCompatibility;
#else
using DependencyPropertyManager = System.Windows.DependencyProperty;
#endif
namespace DevExpress.Xpf.PivotGrid.Internal {
	public class FieldListLayoutSelector : Control {
		#region static staff
		 public static readonly DependencyProperty LayoutProperty;
		 public static readonly DependencyProperty FieldListAreaContentProperty;
		 public static readonly DependencyProperty ColumnAreaContentProperty;
		 public static readonly DependencyProperty RowAreaContentProperty;
		 public static readonly DependencyProperty FilterAreaContentProperty;
		 public static readonly DependencyProperty DataAreaContentProperty;
		 public static readonly DependencyProperty DefereUpdatesContentProperty;
		 public static readonly DependencyProperty StackedDefaultTemplateProperty;
		 public static readonly DependencyProperty StackedSideBySideTemplateProperty;
		 public static readonly DependencyProperty TopPanelOnlyTemplateProperty;
		 public static readonly DependencyProperty BottomPanelOnly2by2TemplateProperty;
		 public static readonly DependencyProperty BottomPanelOnly1by4TemplateProperty;
		 public static readonly DependencyProperty FieldListSplitterYProperty;
		 public static readonly DependencyProperty FieldListSplitterXProperty;
		 public static readonly DependencyProperty ShowAllProperty;
		public static readonly DependencyProperty ActualWidthCoreProperty;
		public static readonly DependencyProperty ActualHeightCoreProperty;
		static FieldListLayoutSelector() {
			Type ownerType = typeof(FieldListLayoutSelector);
			LayoutProperty = DependencyPropertyManager.Register("Layout", typeof(FieldListLayout),
				ownerType, new PropertyMetadata(FieldListLayout.StackedDefault, OnLayoutChanged));
			FieldListAreaContentProperty = DependencyPropertyManager.Register("FieldListAreaContent", typeof(object), ownerType, new PropertyMetadata(string.Empty));
			ColumnAreaContentProperty = DependencyPropertyManager.Register("ColumnAreaContent", typeof(object), ownerType, new PropertyMetadata(string.Empty));
			RowAreaContentProperty = DependencyPropertyManager.Register("RowAreaContent", typeof(object), ownerType, new PropertyMetadata(string.Empty));
			FilterAreaContentProperty = DependencyPropertyManager.Register("FilterAreaContent", typeof(object), ownerType, new PropertyMetadata(string.Empty));
			DataAreaContentProperty = DependencyPropertyManager.Register("DataAreaContent", typeof(object), ownerType, new PropertyMetadata(string.Empty));
			DefereUpdatesContentProperty = DependencyPropertyManager.Register("DefereUpdatesContent", typeof(object), ownerType, new PropertyMetadata(string.Empty));
			StackedDefaultTemplateProperty = DependencyPropertyManager.Register("StackedDefaultTemplate", typeof(ControlTemplate), ownerType, new PropertyMetadata(null, OnLayoutChanged));
			StackedSideBySideTemplateProperty = DependencyPropertyManager.Register("StackedSideBySideTemplate", typeof(ControlTemplate), ownerType, new PropertyMetadata(null, OnLayoutChanged));
			TopPanelOnlyTemplateProperty = DependencyPropertyManager.Register("TopPanelOnlyTemplate", typeof(ControlTemplate), ownerType, new PropertyMetadata(null, OnLayoutChanged));
			BottomPanelOnly2by2TemplateProperty = DependencyPropertyManager.Register("BottomPanelOnly2by2Template", typeof(ControlTemplate), ownerType, new PropertyMetadata(null, OnLayoutChanged));
			BottomPanelOnly1by4TemplateProperty = DependencyPropertyManager.Register("BottomPanelOnly1by4Template", typeof(ControlTemplate), ownerType, new PropertyMetadata(null, OnLayoutChanged));
			FieldListSplitterYProperty = DependencyPropertyManager.Register("FieldListSplitterY", typeof(double), ownerType, new PropertyMetadata(150d));
			FieldListSplitterXProperty = DependencyPropertyManager.Register("FieldListSplitterX", typeof(double), ownerType, new PropertyMetadata(104d));
			ShowAllProperty = DependencyPropertyManager.Register("ShowAll", typeof(bool), ownerType, new PropertyMetadata(false));
			ActualWidthCoreProperty = DependencyPropertyManager.Register("ActualWidthCore", typeof(double), ownerType, new PropertyMetadata(0d));
			ActualHeightCoreProperty = DependencyPropertyManager.Register("ActualHeightCore", typeof(double), ownerType, new PropertyMetadata(0d));
		}
		static void OnLayoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			FieldListLayoutSelector templateSelector = d as FieldListLayoutSelector;
			if(templateSelector == null) return;
			templateSelector.SetLayout();
		}
		#endregion
		public FieldListLayoutSelector() {
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
		}
		public FieldListLayout Layout {
			get { return (FieldListLayout)GetValue(LayoutProperty); }
			set { SetValue(LayoutProperty, value); }
		}
		public object FieldListAreaContent {
			get { return GetValue(FieldListAreaContentProperty); }
			set { SetValue(FieldListAreaContentProperty, value); }
		}
		public object ColumnAreaContent {
			get { return GetValue(ColumnAreaContentProperty); }
			set { SetValue(ColumnAreaContentProperty, value); }
		}
		public object RowAreaContent {
			get { return GetValue(RowAreaContentProperty); }
			set { SetValue(RowAreaContentProperty, value); }
		}
		public object FilterAreaContent {
			get { return GetValue(FilterAreaContentProperty); }
			set { SetValue(FilterAreaContentProperty, value); }
		}
		public object DataAreaContent {
			get { return GetValue(DataAreaContentProperty); }
			set { SetValue(DataAreaContentProperty, value); }
		}
		public object DefereUpdatesContent {
			get { return GetValue(DefereUpdatesContentProperty); }
			set { SetValue(DefereUpdatesContentProperty, value); }
		}
		public ControlTemplate StackedDefaultTemplate {
			get { return (ControlTemplate)GetValue(StackedDefaultTemplateProperty); }
			set { SetValue(StackedDefaultTemplateProperty, value); }
		}
		public ControlTemplate StackedSideBySideTemplate {
			get { return (ControlTemplate)GetValue(StackedSideBySideTemplateProperty); }
			set { SetValue(StackedSideBySideTemplateProperty, value); }
		}
		public ControlTemplate TopPanelOnlyTemplate {
			get { return (ControlTemplate)GetValue(TopPanelOnlyTemplateProperty); }
			set { SetValue(TopPanelOnlyTemplateProperty, value); }
		}
		public ControlTemplate BottomPanelOnly2by2Template {
			get { return (ControlTemplate)GetValue(BottomPanelOnly2by2TemplateProperty); }
			set { SetValue(BottomPanelOnly2by2TemplateProperty, value); }
		}
		public ControlTemplate BottomPanelOnly1by4Template {
			get { return (ControlTemplate)GetValue(BottomPanelOnly1by4TemplateProperty); }
			set { SetValue(BottomPanelOnly1by4TemplateProperty, value); }
		}
		public double FieldListSplitterY {
			get { return (double)GetValue(FieldListSplitterYProperty); }
			set { SetValue(FieldListSplitterYProperty, value); }
		}
		public double FieldListSplitterX {
			get { return (double)GetValue(FieldListSplitterXProperty); }
			set { SetValue(FieldListSplitterXProperty, value); }
		}
		public bool ShowAll {
			get { return (bool)GetValue(ShowAllProperty); }
			set { SetValue(ShowAllProperty, value); }
		}
		public double ActualWidthCore {
			get { return (double)GetValue(ActualWidthCoreProperty); }
			set { SetValue(ActualWidthCoreProperty, value); }
		}
		public double ActualHeightCore {
			get { return (double)GetValue(ActualHeightCoreProperty); }
			set { SetValue(ActualHeightCoreProperty, value); }
		}
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
			SetLayout();
			SizeChanged += FieldListLayoutSelector_SizeChanged;
			UpdateSizes();
		}
		void OnUnloaded(object sender, RoutedEventArgs e) {
			SizeChanged -= FieldListLayoutSelector_SizeChanged;
			UpdateSizes();
		}
		void FieldListLayoutSelector_SizeChanged(object sender, SizeChangedEventArgs e) {
			UpdateSizes();
		}
		private void UpdateSizes() {
			ActualHeightCore = ActualHeight;
			ActualWidthCore = ActualWidth;
		}
		void SetLayout() {
			ControlTemplate template = GetTemplate(Layout);
			if(template != null)
				Template = template;
		}
		internal ControlTemplate GetTemplate(FieldListLayout layout) {
			switch(layout) {
				case FieldListLayout.StackedDefault:
					return StackedDefaultTemplate;
				case FieldListLayout.StackedSideBySide:
					return StackedSideBySideTemplate;
				case FieldListLayout.TopPanelOnly:
					return TopPanelOnlyTemplate;
				case FieldListLayout.BottomPanelOnly2by2:
					return BottomPanelOnly2by2Template;
				case FieldListLayout.BottomPanelOnly1by4:
					return BottomPanelOnly1by4Template;
				default:
					throw new NotImplementedException();
			}
		}
	}
}
