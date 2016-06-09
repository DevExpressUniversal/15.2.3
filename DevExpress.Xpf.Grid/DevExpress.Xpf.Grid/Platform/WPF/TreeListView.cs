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
using System.Linq;
using System.Text;
using DevExpress.Xpf.Printing;
using System.Windows;
using System.Printing;
using System.IO;
using DevExpress.XtraPrinting;
using System.Windows.Controls;
using DevExpress.Xpf.Core.ConditionalFormatting;
using DevExpress.Xpf.Grid.Printing;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
using DevExpress.Xpf.Grid.EditForm;
using System.Windows.Data;
namespace DevExpress.Xpf.Grid {
	public partial class TreeListView {
		#region Printing and Exporting
		#region CSV
		public void ExportToCsv(Stream stream) {
			PrintHelper.ExportToCsv(this, stream);
		}
		public void ExportToCsv(Stream stream, CsvExportOptions options) {
			PrintHelper.ExportToCsv(this, stream, options);
		}
		public void ExportToCsv(string filePath) {
			PrintHelper.ExportToCsv(this, filePath);
		}
		public void ExportToCsv(string filePath, CsvExportOptions options) {
			PrintHelper.ExportToCsv(this, filePath, options);
		}
		#endregion
		#region XLS
		public void ExportToXls(Stream stream) {
			PrintHelper.ExportToXls(this, stream);
		}
		public void ExportToXls(Stream stream, XlsExportOptions options) {
			PrintHelper.ExportToXls(this, stream, options);
		}
		public void ExportToXls(string filePath) {
			PrintHelper.ExportToXls(this, filePath);
		}
		public void ExportToXls(string filePath, XlsExportOptions options) {
			PrintHelper.ExportToXls(this, filePath, options);
		}
		#endregion
		#region XLSX
		public void ExportToXlsx(Stream stream) {
			PrintHelper.ExportToXlsx(this, stream);
		}
		public void ExportToXlsx(Stream stream, XlsxExportOptions options) {
			PrintHelper.ExportToXlsx(this, stream, options);
		}
		public void ExportToXlsx(string filePath) {
			PrintHelper.ExportToXlsx(this, filePath);
		}
		public void ExportToXlsx(string filePath, XlsxExportOptions options) {
			PrintHelper.ExportToXlsx(this, filePath, options);
		}
		#endregion
		#endregion
		#region Lightweight templates
		public static readonly DependencyProperty UseLightweightTemplatesProperty;
		public static readonly DependencyProperty RowDetailsTemplateProperty;
		public static readonly DependencyProperty RowDetailsTemplateSelectorProperty;
		static readonly DependencyPropertyKey ActualRowDetailsTemplateSelectorPropertyKey;
		public static readonly DependencyProperty ActualRowDetailsTemplateSelectorProperty;
		public static readonly DependencyProperty RowDetailsVisibilityModeProperty;
		public UseLightweightTemplates? UseLightweightTemplates {
			get { return (UseLightweightTemplates?)GetValue(UseLightweightTemplatesProperty); }
			set { SetValue(UseLightweightTemplatesProperty, value); }
		}
		public DataTemplate RowDetailsTemplate {
			get { return (DataTemplate)GetValue(RowDetailsTemplateProperty); }
			set { SetValue(RowDetailsTemplateProperty, value); }
		}
		public DataTemplateSelector RowDetailsTemplateSelector {
			get { return (DataTemplateSelector)GetValue(RowDetailsTemplateSelectorProperty); }
			set { SetValue(RowDetailsTemplateSelectorProperty, value); }
		}
		public DataTemplateSelector ActualRowDetailsTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ActualRowDetailsTemplateSelectorProperty); }
		}
		DependencyPropertyKey ITableView.ActualRowDetailsTemplateSelectorPropertyKey { get { return ActualRowDetailsTemplateSelectorPropertyKey; } }
		public RowDetailsVisibilityMode RowDetailsVisibilityMode {
			get { return (RowDetailsVisibilityMode)GetValue(RowDetailsVisibilityModeProperty); }
			set { SetValue(RowDetailsVisibilityModeProperty, value); }
		}
		bool ITableView.UseRowDetailsTemplate(int rowHandle) {
			return TreeListViewBehavior.UseRowDetailsTemplate(rowHandle);
		}
		#endregion
		#region EditForm
		public static readonly DependencyProperty EditFormDialogServiceTemplateProperty = Mvvm.UI.Native.AssignableServiceHelper2<TreeListView, Mvvm.IDialogService>.RegisterServiceTemplateProperty("EditFormDialogServiceTemplate");
		public static readonly DependencyProperty EditFormColumnCountProperty = DependencyProperty.Register("EditFormColumnCount", typeof(int), typeof(TreeListView), new PropertyMetadata(3, (d, e) => ((TreeListView)d).HideEditForm()));
		public static readonly DependencyProperty EditFormPostModeProperty = DependencyProperty.Register("EditFormPostMode", typeof(EditFormPostMode), typeof(TreeListView), new PropertyMetadata(EditFormPostMode.Cached, (d, e) => ((TreeListView)d).HideEditForm()));
		public static readonly DependencyProperty EditFormShowModeProperty = DependencyProperty.Register("EditFormShowMode", typeof(EditFormShowMode), typeof(TreeListView), new PropertyMetadata(EditFormShowMode.None, (d, e) => ((TreeListView)d).HideEditForm()));
		public static readonly DependencyProperty ShowEditFormOnF2KeyProperty = DependencyProperty.Register("ShowEditFormOnF2Key", typeof(bool), typeof(TreeListView), new PropertyMetadata(true));
		public static readonly DependencyProperty ShowEditFormOnEnterKeyProperty = DependencyProperty.Register("ShowEditFormOnEnterKey", typeof(bool), typeof(TreeListView), new PropertyMetadata(true));
		public static readonly DependencyProperty ShowEditFormOnDoubleClickProperty = DependencyProperty.Register("ShowEditFormOnDoubleClick", typeof(bool), typeof(TreeListView), new PropertyMetadata(true));
		public static readonly DependencyProperty EditFormPostConfirmationProperty = DependencyProperty.Register("EditFormPostConfirmation", typeof(PostConfirmationMode), typeof(TreeListView), new PropertyMetadata(PostConfirmationMode.YesNoCancel, (d, e) => ((TreeListView)d).HideEditForm()));
		public static readonly DependencyProperty ShowEditFormUpdateCancelButtonsProperty = DependencyProperty.Register("ShowEditFormUpdateCancelButtons", typeof(bool), typeof(TreeListView), new PropertyMetadata(true, (d, e) => ((TreeListView)d).HideEditForm()));
		public static readonly DependencyProperty EditFormTemplateProperty = DependencyProperty.Register("EditFormTemplate", typeof(DataTemplate), typeof(TreeListView), new PropertyMetadata(null, (d, e) => ((TreeListView)d).HideEditForm()));
		public DataTemplate EditFormDialogServiceTemplate {
			get { return (DataTemplate)GetValue(EditFormDialogServiceTemplateProperty); }
			set { SetValue(EditFormDialogServiceTemplateProperty, value); }
		}
		public int EditFormColumnCount {
			get { return (int)GetValue(EditFormColumnCountProperty); }
			set { SetValue(EditFormColumnCountProperty, value); }
		}
		public EditFormPostMode EditFormPostMode {
			get { return (EditFormPostMode)GetValue(EditFormPostModeProperty); }
			set { SetValue(EditFormPostModeProperty, value); }
		}
		public EditFormShowMode EditFormShowMode {
			get { return (EditFormShowMode)GetValue(EditFormShowModeProperty); }
			set { SetValue(EditFormShowModeProperty, value); }
		}
		public bool ShowEditFormOnF2Key {
			get { return (bool)GetValue(ShowEditFormOnF2KeyProperty); }
			set { SetValue(ShowEditFormOnF2KeyProperty, value); }
		}
		public bool ShowEditFormOnEnterKey {
			get { return (bool)GetValue(ShowEditFormOnEnterKeyProperty); }
			set { SetValue(ShowEditFormOnEnterKeyProperty, value); }
		}
		public bool ShowEditFormOnDoubleClick {
			get { return (bool)GetValue(ShowEditFormOnDoubleClickProperty); }
			set { SetValue(ShowEditFormOnDoubleClickProperty, value); }
		}
		public PostConfirmationMode EditFormPostConfirmation {
			get { return (PostConfirmationMode)GetValue(EditFormPostConfirmationProperty); }
			set { SetValue(EditFormPostConfirmationProperty, value); }
		}
		public BindingBase EditFormCaptionBinding { get; set; }
		public bool ShowEditFormUpdateCancelButtons {
			get { return (bool)GetValue(ShowEditFormUpdateCancelButtonsProperty); }
			set { SetValue(ShowEditFormUpdateCancelButtonsProperty, value); }
		}
		public DataTemplate EditFormTemplate {
			get { return (DataTemplate)GetValue(EditFormTemplateProperty); }
			set { SetValue(EditFormTemplateProperty, value); }
		}
		public void ShowDialogEditForm() {
			TreeListViewEditFormManager.ShowDialogEditForm();
		}
		public void ShowInlineEditForm() {
			TreeListViewEditFormManager.ShowInlineEditForm();
		}
		public void ShowEditForm() {
			TreeListViewEditFormManager.ShowEditForm();
		}
		public void HideEditForm() {
			TreeListViewEditFormManager.HideEditForm();
		}
		public void CloseEditForm() {
			TreeListViewEditFormManager.CloseEditForm();
		}
		internal EditFormManager TreeListViewEditFormManager { get { return EditFormManager as EditFormManager; } }
		internal protected override IEditFormManager CreateEditFormManager() {
			return new EditFormManager(this);
		}
		internal protected override IEditFormOwner CreateEditFormOwner() {
			return new EditFormOwner(this);
		}
		#endregion
	}
}
