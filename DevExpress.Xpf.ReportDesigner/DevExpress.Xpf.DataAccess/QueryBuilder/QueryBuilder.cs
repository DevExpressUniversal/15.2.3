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
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Native.Sql.QueryBuilder;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.DataAccess.Wizard.Native;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Utils;
using DevExpress.Xpf.DataAccess.WaitForm;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.TreeList;
namespace DevExpress.Xpf.DataAccess.DataSourceWizard {
	public class QueryBuilder : Control {
		public static readonly DependencyProperty EditValueProperty;
		public static readonly DependencyProperty SelectedAvailableItemProperty;
		static readonly DependencyPropertyKey AvailableItemGroupBoxHeaderPropertyKey;
		public static readonly DependencyProperty AvailableItemGroupBoxHeaderProperty;
		public static readonly DependencyProperty PreviewDialogServiceTemplateProperty;
		static readonly Action<QueryBuilder, Action<IDialogService>> PreviewDialogServiceAccessor;
		public static readonly DependencyProperty FilterDialogServiceTemplateProperty;
		static readonly Action<QueryBuilder, Action<IDialogService>> FilterDialogServiceAccessor;
		public static readonly DependencyProperty JoinEditorDialogServiceTemplateProperty;
		static readonly Action<QueryBuilder, Action<IDialogService>> JoinEditorDialogServiceAccessor;
		public static readonly DependencyProperty MessageBoxServiceTemplateProperty;
		static readonly Action<QueryBuilder, Action<IMessageBoxService>> MessageBoxServiceAccessor;
		public static readonly DependencyProperty SplashScreenServiceTemplateProperty;
		static readonly Action<QueryBuilder, Action<ISplashScreenService>> splashScreenServiceAccessor;
		static QueryBuilder() {
			DependencyPropertyRegistrator<QueryBuilder>.New()
				.Register(d => d.EditValue, out EditValueProperty, null, d => d.OnEditValueChanged())
				.Register(d => d.SelectedAvailableItem, out SelectedAvailableItemProperty, null, d => d.OnSelectedAvailableItemChanged())
				.RegisterReadOnly(d => d.AvailableItemGroupBoxHeader, out AvailableItemGroupBoxHeaderPropertyKey, out AvailableItemGroupBoxHeaderProperty, null)
				.RegisterServiceTemplateProperty(d => d.PreviewDialogServiceTemplate, out PreviewDialogServiceTemplateProperty, out PreviewDialogServiceAccessor)
				.RegisterServiceTemplateProperty(d => d.FilterDialogServiceTemplate, out FilterDialogServiceTemplateProperty, out FilterDialogServiceAccessor)
				.RegisterServiceTemplateProperty(d => d.JoinEditorDialogServiceTemplate, out JoinEditorDialogServiceTemplateProperty, out JoinEditorDialogServiceAccessor)
				.RegisterServiceTemplateProperty(d => d.MessageBoxServiceTemplate, out MessageBoxServiceTemplateProperty, out MessageBoxServiceAccessor)
				.RegisterServiceTemplateProperty(d => d.SplashScreenServiceTemplate, out SplashScreenServiceTemplateProperty, out splashScreenServiceAccessor)
				.OverrideDefaultStyleKey()
			;
		}
		readonly IWaitFormActivator waitFormActivator;
		public QueryBuilder() {
			this.previewResultsCommand = DelegateCommandFactory.Create(PreviewResults);
			this.onEditFilterCommand = DelegateCommandFactory.Create(OnEditFilter);
			this.addSelectedItemCommand = DelegateCommandFactory.Create(AddSelectedItem);
			this.joinTableCommand = DelegateCommandFactory.Create<int>(JoinTable);
			this.onEditJoinCommand = DelegateCommandFactory.Create<string>(OnEditJoin);
			this.removeTableCommand = DelegateCommandFactory.Create<string>(RemoveTable);
			this.renameTableCommand = DelegateCommandFactory.Create<TreeListView>(RenameTable);
			this.waitFormActivator = new WaitFormActivator(DoWithSplashScreenService);
		}
		readonly ICommand previewResultsCommand;
		public ICommand PreviewResultsCommand { get { return previewResultsCommand; } }
		readonly ICommand onEditFilterCommand;
		public ICommand OnEditFilterCommand { get { return onEditFilterCommand; } }
		readonly ICommand addSelectedItemCommand;
		public ICommand AddSelectedItemCommand { get { return addSelectedItemCommand; } }
		readonly ICommand joinTableCommand;
		public ICommand JoinTableCommand { get { return joinTableCommand; } }
		readonly ICommand onEditJoinCommand;
		public ICommand OnEditJoinCommand { get { return onEditJoinCommand; } }
		readonly ICommand removeTableCommand;
		public ICommand RemoveTableCommand { get { return removeTableCommand; } }
		readonly ICommand renameTableCommand;
		public ICommand RenameTableCommand { get { return renameTableCommand; } }
		public QueryBuilderViewModel EditValue {
			get { return (QueryBuilderViewModel)GetValue(EditValueProperty); }
			set { SetValue(EditValueProperty, value); }
		}
		void OnEditValueChanged() {
			SelectedAvailableItem = EditValue.Available.FirstOrDefault();
		}
		public AvailableItemData SelectedAvailableItem {
			get { return (AvailableItemData)GetValue(SelectedAvailableItemProperty); }
			set { SetValue(SelectedAvailableItemProperty, value); }
		}
		void OnSelectedAvailableItemChanged() {
			AvailableItemGroupBoxHeader = string.Format(DataAccessUILocalizer.GetString(DataAccessUIStringId.QueryBuilderColumnsOf), SelectedAvailableItem.Name);
			EditValue.OnAvailableExpand(SelectedAvailableItem.Name);
		}
		public string AvailableItemGroupBoxHeader {
			get { return (string)GetValue(AvailableItemGroupBoxHeaderProperty); }
			private set { SetValue(AvailableItemGroupBoxHeaderPropertyKey, value); }
		}
		public void DoWithPreviewDialogService(Action<IDialogService> action) { PreviewDialogServiceAccessor(this, action); }
		public DataTemplate PreviewDialogServiceTemplate {
			get { return (DataTemplate)GetValue(PreviewDialogServiceTemplateProperty); }
			set { SetValue(PreviewDialogServiceTemplateProperty, value); }
		}
		public void DoWithFilterDialogService(Action<IDialogService> action) { FilterDialogServiceAccessor(this, action); }
		public DataTemplate FilterDialogServiceTemplate {
			get { return (DataTemplate)GetValue(FilterDialogServiceTemplateProperty); }
			set { SetValue(FilterDialogServiceTemplateProperty, value); }
		}
		public void DoWithJoinEditorDialogService(Action<IDialogService> action) { JoinEditorDialogServiceAccessor(this, action); }
		public DataTemplate JoinEditorDialogServiceTemplate {
			get { return (DataTemplate)GetValue(JoinEditorDialogServiceTemplateProperty); }
			set { SetValue(JoinEditorDialogServiceTemplateProperty, value); }
		}
		public void DoWithMessageBoxService(Action<IMessageBoxService> action) { MessageBoxServiceAccessor(this, action); }
		public DataTemplate MessageBoxServiceTemplate {
			get { return (DataTemplate)GetValue(MessageBoxServiceTemplateProperty); }
			set { SetValue(MessageBoxServiceTemplateProperty, value); }
		}
		protected void DoWithSplashScreenService(Action<ISplashScreenService> action) { splashScreenServiceAccessor(this, action); }
		public DataTemplate SplashScreenServiceTemplate {
			get { return (DataTemplate)GetValue(SplashScreenServiceTemplateProperty); }
			set { SetValue(SplashScreenServiceTemplateProperty, value); }
		}
		void PreviewResults() {
			SelectedDataEx data = null;
			CancellationTokenSource cts = new CancellationTokenSource();
			CancellationTokenHook hook = new CancellationTokenHook(cts);
			CancellationToken token = cts.Token;
			this.waitFormActivator.ShowWaitForm(true, true, true);
			this.waitFormActivator.SetWaitFormObject(hook);
			Exception exception = null;
			try {
				Task<SelectedDataEx> task = EditValue.GetPreviewDataAsync(token);
				task.Wait(token);
				if(task.Status == TaskStatus.RanToCompletion)
					data = task.Result;
			} catch(Exception ex) {
				exception = ex;
			} finally { this.waitFormActivator.CloseWaitForm(); }
			if(exception != null) {
				AggregateException aex = exception as AggregateException;
				if(aex != null) {
					aex.Flatten();
					if(aex.InnerExceptions.Count == 1)
						exception = aex.GetBaseException();
				}
				IExceptionHandler exceptionHandler = new QueryExceptionHandler(DoWithMessageBoxService);
				exceptionHandler.HandleException(exception);
			}
			if(data == null)
				return;
			DoWithPreviewDialogService(dialog => dialog.ShowDialog(MessageButton.OK, DataAccessUILocalizer.GetString(DataAccessUIStringId.QueryDesignControlDataPreviewCaption), new ResultTable(string.Empty, data)));
		}
		void OnEditFilter() {
			EditValue.OnEditFilter(OpenFilterEditor);
		}
		IFiltersView OpenFilterEditor() {
			return FiltersView.Create(DoWithFilterDialogService);
		}
		void AddSelectedItem() {
			EditValue.OnTableAddToSelection(SelectedAvailableItem.Name, OpenJoinEditor);
		}
		void JoinTable(int id) {
			var row = EditValue.Selection.First(x => x.Id == id);
			var columnName = row.Name;
			var tableName = EditValue.Selection.First(x => x.Id == row.Parent).Name;
			EditValue.OnJoinWithForeignKey(tableName, columnName);
		}
		void OnEditJoin(string name) {
			EditValue.OnEditJoin(name, OpenJoinEditor);
		}
		IJoinEditorView OpenJoinEditor() {
			return new JoinEditorView(DoWithJoinEditorDialogService);
		}
		bool ConfirmTablesRemoving(IEnumerable<string> tables) {
			bool confirm = false;
			string message = string.Format(DataAccessUILocalizer.GetString(DataAccessUIStringId.QueryDesignControlRemoveTables), string.Join(",\n", tables.Select(x => string.Format("\t\u2022 {0}", x))) + ".\n");
			DoWithMessageBoxService(dialog => {
				if(dialog.ShowMessage(message, DataAccessUILocalizer.GetString(DataAccessUIStringId.MessageBoxWarningTitle), MessageButton.OKCancel, MessageIcon.Warning) == MessageResult.OK)
					confirm = true;
			});
			return confirm;
		}
		void RemoveTable(string name) {
			EditValue.OnTableRemoveFromSelection(name, ConfirmTablesRemoving);
		}
		void RenameTable(TreeListView view) {
			view.DataControl.CurrentColumn.AllowEditing = DefaultBoolean.True;
			TreeListEditorEventHandler shownEditor = null;
			shownEditor = (sender_showEditor, event_showEditor) => {
				RoutedEventHandler lostFocus = null;
				lostFocus = (s, e) => {
					view.DataControl.CurrentColumn.AllowEditing = DefaultBoolean.False;
					view.ActiveEditor.LostFocus -= lostFocus;
				};
				view.ActiveEditor.LostFocus += lostFocus;
				view.ShownEditor -= shownEditor;
			};
			view.ShownEditor += shownEditor;
			view.ShowEditor();
		}
		#region Inner classes
		class QueryExceptionHandler : ExceptionHandler {
			public QueryExceptionHandler(Action<Action<IMessageBoxService>> doWithMessageBoxService)
				: base(doWithMessageBoxService, DataAccessUILocalizer.GetString(DataAccessUIStringId.MessageBoxWarningTitle)) { }
			#region Overrides of ExceptionHandler
			protected override string GetMessage(Exception exception) {
				if(exception is NoTablesValidationException || exception is NoColumnsValidationException)
					return DataAccessLocalizer.GetString(DataAccessStringId.QueryBuilderNothingSelected);
				return base.GetMessage(exception);
			}
			#endregion
		}
		#endregion
	}
}
