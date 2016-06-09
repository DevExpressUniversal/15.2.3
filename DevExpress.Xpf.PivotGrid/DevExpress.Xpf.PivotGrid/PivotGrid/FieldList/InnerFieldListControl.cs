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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.PivotGrid;
using DevExpress.Xpf.PivotGrid.Internal;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Localization;
using PivotFieldsObservableCollection = DevExpress.Xpf.Core.ObservableCollectionCore<DevExpress.Xpf.PivotGrid.PivotGridField>;
using PivotFieldsReadOnlyObservableCollection = System.Collections.ObjectModel.ReadOnlyObservableCollection<DevExpress.Xpf.PivotGrid.PivotGridField>;
using DevExpress.Xpf.Core.Commands;
#if SL
using IBindingList = DevExpress.Data.Browsing.IBindingList;
using ApplicationException = System.Exception;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DevExpress.Xpf.Core.WPFCompatibility;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DevExpress.Xpf.Core.Native;
#else
using DependencyPropertyManager = System.Windows.DependencyProperty;
#endif
namespace DevExpress.Xpf.PivotGrid.Internal {
	public class InnerFieldListControl : FieldHeadersBase {
		#region static stuff
		public static readonly DependencyProperty StandartTemplateProperty;
		public static readonly DependencyProperty TreeViewTemplateProperty;
		public static readonly DependencyProperty ShowBorderProperty;
		public static readonly DependencyProperty GroupFieldsProperty;
		public static readonly DependencyProperty ShowAllProperty;
		public static readonly DependencyProperty ActualShowAllProperty;
		static readonly DependencyPropertyKey ActualShowAllPropertyKey;
		static InnerFieldListControl() {
			Type ownerType = typeof(InnerFieldListControl);
			Type collectionType = typeof(PivotFieldsReadOnlyObservableCollection);
			StandartTemplateProperty = DependencyPropertyManager.Register("StandartTemplate", typeof(ControlTemplate), ownerType, new PropertyMetadata((d, e) => ((InnerFieldListControl)d).EnsureTemplate()));
			TreeViewTemplateProperty = DependencyPropertyManager.Register("TreeViewTemplate", typeof(ControlTemplate), ownerType, new PropertyMetadata((d, e) => ((InnerFieldListControl)d).EnsureTemplate()));
			GroupFieldsProperty = DependencyPropertyManager.Register("GroupFields", typeof(bool), ownerType, new PropertyMetadata(false, (d, e) => ((InnerFieldListControl)d).EnsureTemplate()));
			ShowAllProperty = DependencyPropertyManager.Register("ShowAll", typeof(bool), ownerType, new PropertyMetadata(false, (d, e) => ((InnerFieldListControl)d).UpdateActualShowAll()));
			ActualShowAllPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualShowAll", typeof(bool), ownerType, new PropertyMetadata(false, (d, e) => ((InnerFieldListControl)d).OnActualShowAllChanged()));
			ActualShowAllProperty = ActualShowAllPropertyKey.DependencyProperty;
			ShowBorderProperty = DependencyPropertyManager.Register("ShowBorder", typeof(bool), ownerType, new PropertyMetadata(true, (d, e) => ((InnerFieldListControl)d).OnShowBorderChanged((bool)e.NewValue)));
		}
		#endregion
		public InnerFieldListControl() {
			this.SetDefaultStyleKey(typeof(InnerFieldListControl));
#if !SL
			Loaded += new RoutedEventHandler(InnerFieldListControl_Loaded);
#endif
		}
#if !SL
		void InnerFieldListControl_Loaded(object sender, RoutedEventArgs e) {
			FrameworkElement elem = DevExpress.Xpf.Core.Native.LayoutHelper.FindElementByType(this, typeof(DevExpress.Xpf.Editors.Popups.EditorListBox));
			if(elem != null)
				elem.SetValue(ScrollViewer.PanningModeProperty, PanningMode.None);
		}
#endif
		public bool ShowBorder {
			get { return (bool)GetValue(ShowBorderProperty); }
			set { SetValue(ShowBorderProperty, value); }
		}
		public ControlTemplate StandartTemplate {
			get { return (ControlTemplate)GetValue(StandartTemplateProperty); }
			set { SetValue(StandartTemplateProperty, value); }
		}
		public ControlTemplate TreeViewTemplate {
			get { return (ControlTemplate)GetValue(TreeViewTemplateProperty); }
			set { SetValue(TreeViewTemplateProperty, value); }
		}
		public bool GroupFields {
			get { return (bool)GetValue(GroupFieldsProperty); }
			set { SetValue(GroupFieldsProperty, value); }
		}
		public bool ShowAll {
			get { return (bool)GetValue(ShowAllProperty); }
			set { SetValue(ShowAllProperty, value); }
		}
		public bool ActualShowAll {
			get { return (bool)GetValue(ActualShowAllProperty); }
			private set { this.SetValue(ActualShowAllPropertyKey, value); }
		}
		public void EnsureTemplate() {
			PivotGridWpfData data = PivotGridControl.GetDataFromAttachedPivot(this);
			if(data == null || !GroupFields && !ActualShowAll || (Area != FieldListArea.All && Area != FieldListArea.DataArea)) {
				if(Template != StandartTemplate)
					Template = StandartTemplate;
			} else {
				if(Template != TreeViewTemplate)
					Template = TreeViewTemplate;
			}
		}
		void OnShowBorderChanged(bool newValue) {
			if(newValue)
				BorderThickness = new Thickness(1);
			else
				BorderThickness = new Thickness(0);
		}
		private InnerFieldListControl GetVisualStateContainer() {
			return this;
		}
		protected override void OnActualShowAllChanged() {
			base.OnActualShowAllChanged();
			EnsureItems();
			EnsureTemplate();
		}
		void UpdateActualShowAll() {
			ActualShowAll = ShowAll && (Area == FieldListArea.All || Area == FieldListArea.DataArea);
		}
		protected override System.Windows.Controls.Control GetEmtyStateElement() {
#if !SL
			return TemplatedParent as System.Windows.Controls.Control;
#else
			return LayoutHelper.FindParentObject<PivotFieldListControl>(this);
#endif
		}
		protected override void OnIsEmptyChanged() {
		}
		public override bool GetActualShowAll() {
			return ActualShowAll;
		}
		protected override void OnPivotGridChanged() {
			base.OnPivotGridChanged();
			EnsureTemplate();
		}
		protected override void OnAreaChanged() {
			base.OnAreaChanged();
			UpdateActualShowAll();
		}
	}
}
