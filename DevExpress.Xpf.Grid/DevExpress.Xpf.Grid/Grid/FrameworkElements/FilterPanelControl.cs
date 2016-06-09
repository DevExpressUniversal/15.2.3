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
using DevExpress.Xpf.Grid.HitTest;
using System.Windows.Input;
using DevExpress.Xpf.Editors.Filtering;
using DevExpress.Xpf.Utils;
using System.Windows.Data;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using System.Collections.Generic;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Grid {
	public class FilterPanelControl : FilterPanelControlBase {
#if DEBUGTEST
		internal ComboBoxEdit MRUListCombo { get { return mruListCombo; } }
#endif
		public FilterPanelControl() {
			SetBinding(ClearFilterCommandProperty, new Binding("Commands.ClearFilter"));
			SetBinding(ShowFilterEditorCommandProperty, new Binding("Commands.ShowFilterEditor"));
			SetBinding(IsFilterEnabledProperty, new Binding("DataControl.IsFilterEnabled") { Mode = BindingMode.TwoWay });
			SetBinding(AllowFilterEditorProperty, new Binding("ShowEditFilterButton"));
			SetBinding(IsCanEnableFilterProperty, new Binding("DataControl.FilterCriteria") { Converter = new ObjectToBooleanConverter() });
			SetBinding(MRUFiltersProperty, new Binding("DataControl.MRUFilters"));
			SetBinding(ActiveFilterInfoProperty, new Binding("DataControl.ActiveFilterInfo"));
			SetBinding(AllowMRUFilterListProperty, new Binding("DataControl.AllowMRUFilterList"));
			GridViewHitInfoBase.SetHitTestAcceptor(this, new FilterPanelTableViewHitTestAcceptor());
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			GridViewHitInfoBase.SetHitTestAcceptorSafe(GetTemplateChild("PART_FilterPanelCloseButton"), new FilterPanelCloseButtonTableViewHitTestAcceptor());
			GridViewHitInfoBase.SetHitTestAcceptorSafe(GetTemplateChild("PART_FilterControlButton"), new FilterPanelCustomizeButtonTableViewHitTestAcceptor());
			GridViewHitInfoBase.SetHitTestAcceptorSafe(GetTemplateChild("PART_FilterPanelIsActiveButton"), new FilterPanelActiveButtonTableViewHitTestAcceptor());
			GridViewHitInfoBase.SetHitTestAcceptorSafe(GetTemplateChild("PART_FilterPanelText"), new FilterPanelTextTableViewHitTestAcceptor());
			GridViewHitInfoBase.SetHitTestAcceptorSafe(GetTemplateChild("PART_FilterPanelMRUComboBox"), new MRUFilterListComboBoxHitTestAcceptor());
		}
		protected override void FilterPanelMRUComboBoxSelectedIndexChanged(object sender, RoutedEventArgs args) {
				CriteriaOperatorInfo selectedFilter=GetSelectedFilter();
				if(selectedFilter != null) {
					DataViewBase view = DataContext as DataViewBase;
					if(view != null && view.DataControl != null) {
						view.DataControl.FilterCriteria = selectedFilter.FilterOperator;
					}
				}
		}
#if !SL
		[System.ComponentModel.Browsable(false)]
		public bool ShouldSerializeMRUFilters(System.Windows.Markup.XamlDesignerSerializationManager manager) {
			return false;
		}
#endif
	}
	public class FilterPanelCaptionConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if(value == null)
				return null;
			return string.Format(GridControlLocalizer.GetString(GridControlStringId.FilterPanelCaptionFormatStringForMasterDetail), value.ToString());
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
