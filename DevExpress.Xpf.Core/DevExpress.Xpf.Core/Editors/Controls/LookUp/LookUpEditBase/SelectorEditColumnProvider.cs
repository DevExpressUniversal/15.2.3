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

#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Windows.Markup;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Windows.Automation.Peers;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Editors.Internal;
#if !SL
using DevExpress.Data.Access;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Themes;
using DevExpress.Data.Mask;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Automation;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
#else
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.WPFCompatibility;
using DevExpress.Xpf.Editors.WPFCompatibility.Extensions;
using DevExpress.Data.Mask;
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Editors.Automation;
#endif
#if SL
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using SelectionChangedEventArgs = DevExpress.Xpf.Editors.WPFCompatibility.SLSelectionChangedEventArgs;
using SelectionChangedEventHandler = DevExpress.Xpf.Editors.WPFCompatibility.SLSelectionChangedEventHandler;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
using System.Windows.Data;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
#endif
#endregion
namespace DevExpress.Xpf.Editors {
	public class SelectorEditColumnProvider : Decorator, ISearchPanelColumnProvider {
		public static readonly DependencyProperty OwnerEditProperty;
		internal static readonly DependencyPropertyKey ColumnsPropertyKey;
		public static readonly DependencyProperty ColumnsProperty;
		public static readonly DependencyProperty AllowFilterProperty;
		internal static readonly DependencyPropertyKey AvailableColumnsPropertyKey;
		public static readonly DependencyProperty AvailableColumnsProperty;
		public static readonly DependencyProperty CustomColumnsProperty;
		public static readonly DependencyProperty ItemsSourceTypeProperty;
		static SelectorEditColumnProvider() {
			Type ownerType = typeof(SelectorEditColumnProvider);
			OwnerEditProperty = DependencyPropertyManager.Register("OwnerEdit", typeof(ISelectorEdit), ownerType,
				new FrameworkPropertyMetadata(null, (d, e) => ((SelectorEditColumnProvider)d).OwnerEditChanged((ISelectorEdit)e.NewValue)));
			AllowFilterProperty = DependencyPropertyManager.Register("AllowFilter", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, (d, e) => ((SelectorEditColumnProvider)d).AllowFilterChanged((bool)e.NewValue)));
			ColumnsPropertyKey = DependencyPropertyManager.RegisterReadOnly("Columns", typeof(ReadOnlyObservableCollection<string>), ownerType, new FrameworkPropertyMetadata(null));
			ColumnsProperty = ColumnsPropertyKey.DependencyProperty;
			AvailableColumnsPropertyKey = DependencyPropertyManager.RegisterReadOnly("AvailableColumns", typeof(ReadOnlyObservableCollection<string>), ownerType, new FrameworkPropertyMetadata(null));
			AvailableColumnsProperty = AvailableColumnsPropertyKey.DependencyProperty;
			CustomColumnsProperty = DependencyPropertyManager.Register("CustomColumns", typeof(ObservableCollection<string>), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((SelectorEditColumnProvider)d).CustomColumnsChanged((ObservableCollection<string>)e.OldValue, (ObservableCollection<string>)e.NewValue)));
			ItemsSourceTypeProperty = DependencyProperty.Register("ItemsSourceType", typeof(Type), ownerType, new PropertyMetadata(null, (d, e) => ((SelectorEditColumnProvider)d).ItemsSourceTypeChanged()));
		}
		void ItemsSourceTypeChanged() {
			var customColumns = new ObservableCollection<string>();
			if(ItemsSourceType == null) {
				CustomColumns = customColumns;
				return;
			}
			foreach(PropertyDescriptor prop in TypeDescriptor.GetProperties(ItemsSourceType))
				customColumns.Add(prop.Name);
			CustomColumns = customColumns;
		}
		public SelectorEditColumnProvider() {
			Columns = new ReadOnlyObservableCollection<string>(new ObservableCollection<string>());
			CustomColumns = new ObservableCollection<string>();
		}
		public ObservableCollection<string> CustomColumns {
			get { return (ObservableCollection<string>)GetValue(CustomColumnsProperty); }
			set { this.SetValue(CustomColumnsProperty, value); }
		}
		public ReadOnlyObservableCollection<string> AvailableColumns {
			get { return (ReadOnlyObservableCollection<string>)GetValue(AvailableColumnsProperty); }
			internal set { this.SetValue(AvailableColumnsPropertyKey, value); }
		}
		public ReadOnlyObservableCollection<string> Columns {
			get { return (ReadOnlyObservableCollection<string>)GetValue(ColumnsProperty); }
			internal set { this.SetValue(ColumnsPropertyKey, value); }
		}
		public ISelectorEdit OwnerEdit {
			get { return (ISelectorEdit)GetValue(OwnerEditProperty); }
			set { SetValue(OwnerEditProperty, value); }
		}
		public bool AllowFilter {
			get { return (bool)GetValue(AllowFilterProperty); }
			set { SetValue(AllowFilterProperty, value); }
		}
		public Type ItemsSourceType {
			get { return (Type)GetValue(ItemsSourceTypeProperty); }
			set { SetValue(ItemsSourceTypeProperty, value); }
		}
		CriteriaOperator FilterCriteria { get; set; }
		FilterCondition FilterCondition { get; set; }
		string SearchText { get; set; }
		FilterByColumnsMode FilterByColumnsMode { get; set; }
		public void UpdateColumns(FilterByColumnsMode mode) {
			FilterByColumnsMode = mode;
			AvailableColumns = new ReadOnlyObservableCollection<string>(GetAvailableColumns());
			Columns = new ReadOnlyObservableCollection<string>(GetColumns(mode));
		}
		ObservableCollection<string> GetAvailableColumns() {
			if (OwnerEdit == null)
				return new ObservableCollection<string>();
			return new ObservableCollection<string>(OwnerEdit.ItemsProvider.AvailableColumns);
		}
		ObservableCollection<string> GetCustomColumns() {
			if (CustomColumns == null)
				return new ObservableCollection<string>();
			return CustomColumns;
		}
		ObservableCollection<string> GetColumns(FilterByColumnsMode mode) {
			switch(mode){
				case Editors.FilterByColumnsMode.Default:
					return new ObservableCollection<string> { GetDisplayColumn() };
				case Editors.FilterByColumnsMode.Custom:
					return new ObservableCollection<string>(GetCustomColumns());
			}
			return new ObservableCollection<string>();
		}
		string GetDisplayColumn() {
			if (OwnerEdit == null)
				return string.Empty;
			return string.IsNullOrEmpty(OwnerEdit.DisplayMember) ? DataControllerData.DisplayColumn : OwnerEdit.DisplayMember;
		}
		protected virtual void CustomColumnsChanged(ObservableCollection<string> oldCollection, ObservableCollection<string> newCollection) {
			if (oldCollection != null)
				oldCollection.CollectionChanged -= CustomColumnsCollectionChanged;
			if (newCollection != null)
				newCollection.CollectionChanged += CustomColumnsCollectionChanged;
			PerformUpdate();
		}
		protected virtual void CustomColumnsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			PerformUpdate();
		}
		protected virtual void OwnerEditChanged(ISelectorEdit editor) {
			if (editor != null)
				PerformUpdate();
		}
		protected virtual void AllowFilterChanged(bool value) {
			if (value)
				PerformUpdate();
			else
				OwnerEdit.FilterCriteria = null;
		}
		protected virtual void PerformUpdate() {
			if(OwnerEdit != null && AllowFilter)
				OwnerEdit.FilterCriteria = FilterCriteria;
			UpdateColumns(FilterByColumnsMode);
		}
		#region ISearchPanelColumnSource Members
		IEnumerable<string> ISearchPanelColumnProvider.Columns {
			get { return Columns; }
		}
		bool ISearchPanelColumnProviderBase.UpdateFilter(string searchText, FilterCondition filterCondition, CriteriaOperator filterCriteria) {
			FilterCriteria = filterCriteria;
			FilterCondition = filterCondition;
			SearchText = searchText;
			if (AllowFilter)
				PerformUpdate();
			return true;
		}
		string ISearchPanelColumnProviderBase.GetSearchText() {
			return SearchText;
		}
		#endregion
	}
}
