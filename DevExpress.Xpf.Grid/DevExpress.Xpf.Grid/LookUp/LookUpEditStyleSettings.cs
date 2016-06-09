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
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Utils;
using System.Linq;
namespace DevExpress.Xpf.Grid.LookUp {
	public class LookUpEditStyleSettings : BaseLookUpStyleSettings {
		public static readonly DependencyProperty ShowTotalSummaryProperty, ShowColumnHeadersProperty, ShowGroupPanelProperty;
		public static readonly DependencyProperty AllowSortingProperty;
		public static readonly DependencyProperty AllowGroupingProperty;
		public static readonly DependencyProperty AllowColumnFilteringProperty;
		public static readonly DependencyProperty SelectionModeProperty;
		static LookUpEditStyleSettings() {
			Type ownerType = typeof(LookUpEditStyleSettings);
			SelectionModeProperty = DependencyPropertyManager.Register("SelectionMode", typeof(SelectionMode), ownerType, new FrameworkPropertyMetadata(SelectionMode.Single));
			ShowColumnHeadersProperty = DependencyPropertyManager.Register("ShowColumnHeaders", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			ShowTotalSummaryProperty = DependencyPropertyManager.Register("ShowTotalSummary", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			ShowGroupPanelProperty = DependencyPropertyManager.Register("ShowGroupPanel", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			AllowGroupingProperty = DependencyPropertyManager.Register("AllowGrouping", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			AllowSortingProperty = DependencyPropertyManager.Register("AllowSorting", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			AllowColumnFilteringProperty = DependencyPropertyManager.Register("AllowColumnFiltering", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
		}
		#region public properties
		public bool ShowTotalSummary {
			get { return (bool)GetValue(ShowTotalSummaryProperty); }
			set { SetValue(ShowTotalSummaryProperty, value); }
		}
		public bool ShowColumnHeaders {
			get { return (bool)GetValue(ShowColumnHeadersProperty); }
			set { SetValue(ShowColumnHeadersProperty, value); }
		}
		public bool ShowGroupPanel {
			get { return (bool)GetValue(ShowGroupPanelProperty); }
			set { SetValue(ShowGroupPanelProperty, value); }
		}
		public bool AllowGrouping {
			get { return (bool)GetValue(AllowGroupingProperty); }
			set { SetValue(AllowGroupingProperty, value); }
		}
		public bool AllowSorting {
			get { return (bool)GetValue(AllowSortingProperty); }
			set { SetValue(AllowSortingProperty, value); }
		}
		public bool AllowColumnFiltering {
			get { return (bool)GetValue(AllowColumnFilteringProperty); }
			set { SetValue(AllowColumnFilteringProperty, value); }
		}
		#endregion
		protected override Style GetItemContainerStyle(LookUpEditBase edit) {
			return edit.ItemContainerStyle;
		}
		public SelectionMode SelectionMode {
			get { return (SelectionMode)GetValue(SelectionModeProperty); }
			set { SetValue(SelectionModeProperty, value); }
		}
		protected override SelectionMode GetSelectionMode(LookUpEditBase editor) {
			return SelectionMode;
		}
		protected override SelectionEventMode GetSelectionEventMode(ISelectorEdit edit) {
			if (!((LookUpEditBase)edit).AllowItemHighlighting)
				return SelectionEventMode.MouseDown;
			return SelectionEventMode.MouseEnter;
		}
		public override PopupFooterButtons GetPopupFooterButtons(PopupBaseEdit editor) {
			return SelectionMode == System.Windows.Controls.SelectionMode.Single ? PopupFooterButtons.None : PopupFooterButtons.OkCancel;
		}
		protected override bool GetShowSizeGrip(PopupBaseEdit editor) {
			return true;
		}
		protected internal virtual bool ShowSearchPanel {
			get { return false; }
		}
		protected override FilterByColumnsMode FilterByColumnsMode {
			get { return FilterByColumnsMode.Custom; }
		}
		protected internal virtual void SyncValues(LookUpEditBase editor, GridControl grid) {
			if (LookUpEditHelper.GetIsServerMode(editor)) {
				if (GetSelectionMode(editor) == SelectionMode.Single) {
					grid.View.SelectRowByValue(editor.ValueMember, LookUpEditHelper.GetEditValue(editor));
					return;
				}
				else {
					grid.View.SelectRowsByValues(editor.ValueMember, (IEnumerable<object>)LookUpEditHelper.GetEditValue(editor));
					return;
				}
			}
			if (GetSelectionMode(editor) == SelectionMode.Single) {
				object selectedItem = LookUpEditHelper.GetSelectedItem(editor);
				if (selectedItem != null)
					grid.SetCurrentItemCore(selectedItem);
			}
			else {				
				var selectedItems = LookUpEditHelper.GetSelectedItems(editor).Cast<object>();
				grid.ResetSelectedItems(selectedItems.ToList());
			}
		}
		protected override bool GetIncrementalFiltering() {
			return true;
		}
	}
	public class TokenLookUpEditStyleSettings : LookUpEditStyleSettings, ITokenStyleSettings {
		public static readonly DependencyProperty ShowTokenButtonsProperty;
		public static readonly DependencyProperty TokenBorderTemplateProperty;
		public static readonly DependencyProperty EnableTokenWrappingProperty;
		public static readonly DependencyProperty NewTokenPositionProperty;
		public static readonly DependencyProperty TokenTextTrimmingProperty;
		public static readonly DependencyProperty TokenMaxWidthProperty;
		public static readonly DependencyProperty AddTokenOnPopupSelectionProperty;
		public static readonly DependencyProperty AllowEditTokensProperty;
		static TokenLookUpEditStyleSettings() {
			Type ownerType = typeof(TokenLookUpEditStyleSettings);
			EnableTokenWrappingProperty = DependencyProperty.Register("EnableTokenWrapping", typeof(bool?), ownerType);
			TokenBorderTemplateProperty = DependencyProperty.Register("TokenBorderTemplate", typeof(ControlTemplate), ownerType);
			ShowTokenButtonsProperty = DependencyProperty.Register("ShowTokenButtons", typeof(bool?), ownerType, new FrameworkPropertyMetadata(true));
			NewTokenPositionProperty = DependencyProperty.Register("NewTokenPosition", typeof(NewTokenPosition?), ownerType, new FrameworkPropertyMetadata(null));
			TokenTextTrimmingProperty = DependencyProperty.Register("TokenTextTrimming", typeof(TextTrimming?), ownerType, new FrameworkPropertyMetadata(null));
			TokenMaxWidthProperty = DependencyProperty.Register("TokenMaxWidth", typeof(double?), ownerType, new FrameworkPropertyMetadata(null));
			AddTokenOnPopupSelectionProperty = DependencyProperty.Register("AddTokenOnPopupSelection", typeof(bool?), ownerType, new FrameworkPropertyMetadata(null));
			AllowEditTokensProperty = DependencyProperty.Register("AllowEditTokens", typeof(bool?), ownerType, new FrameworkPropertyMetadata(null));
		}
		public bool? AllowEditTokens {
			get { return (bool?)GetValue(AllowEditTokensProperty); }
			set { SetValue(AllowEditTokensProperty, value); }
		}
		[Obsolete]
		public bool? AddTokenOnPopupSelection {
			get { return (bool?)GetValue(AddTokenOnPopupSelectionProperty); }
			set { SetValue(AddTokenOnPopupSelectionProperty, value); }
		}
		public double? TokenMaxWidth {
			get { return (double?)GetValue(TokenMaxWidthProperty); }
			set { SetValue(TokenMaxWidthProperty, value); }
		}
		public TextTrimming? TokenTextTrimming {
			get { return (TextTrimming?)GetValue(TokenTextTrimmingProperty); }
			set { SetValue(TokenTextTrimmingProperty, value); }
		}
		public NewTokenPosition? NewTokenPosition {
			get { return (NewTokenPosition?)GetValue(NewTokenPositionProperty); }
			set { SetValue(NewTokenPositionProperty, value); }
		}
		public bool? EnableTokenWrapping {
			get { return (bool?)GetValue(EnableTokenWrappingProperty); }
			set { SetValue(EnableTokenWrappingProperty, value); }
		}
		public ControlTemplate TokenBorderTemplate {
			get { return (ControlTemplate)GetValue(TokenBorderTemplateProperty); }
			set { SetValue(TokenBorderTemplateProperty, value); }
		}
		public bool? ShowTokenButtons {
			get { return (bool?)GetValue(ShowTokenButtonsProperty); }
			set { SetValue(ShowTokenButtonsProperty, value); }
		}
		public override bool IsTokenStyleSettings() {
			return true;
		}
		protected override bool GetActualAllowDefaultButton(ButtonEdit editor) {
			LookUpEditBasePropertyProvider btn = (LookUpEditBasePropertyProvider)ActualPropertyProvider.GetProperties(editor);
			return !btn.EnableTokenWrapping;
		}
	}
}
