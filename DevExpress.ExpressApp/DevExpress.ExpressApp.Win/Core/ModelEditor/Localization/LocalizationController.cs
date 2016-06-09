#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Win.Templates.ActionContainers;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor {
	public delegate void ItemUpdaterDelegate(LocalizationItemBind item);
	public delegate bool ReplaceAllItemsDelegate(int itemsCount, string defaultLanguageValue);
	public class LocalizationController : ISupportUpdate {
		internal enum LocalizationExportType { AllValues, ModifiedValues, SelectedValues };
		private string modelEditorInitialAspect;
		private ModelApplicationBase modelApplication;
		private LocalizationForm localizationForm;
		private Dictionary<string, ILocalizationImportExport> localizationImportExports;
		private SettingsStorage settingsStorage;
		private ModelDifferenceStore diffstore;
		private string currentPropertyPath;
		private string defaultLanguage;
		private bool? replaceAlways = null;
		private bool isUpdate = false;
		private bool isModelModified = false;
		private SimpleAction saveAction;
		private SingleChoiceAction exportAction;
		private SimpleAction importAction;
		private SingleChoiceAction translatedLanguageAction;
		private SimpleAction translateAction;
		private SingleChoiceAction predefinedFiltersAction;
		private SimpleAction selectAllAction;
		private SimpleAction undoAction;
		private SimpleAction markAsTranslated;
		private ActionsDXPopupMenu popupMenu;
		private ActionContainerMenuBarItem popupMenuContainer;
		public LocalizationController(ModelApplicationBase modelApplication, LocalizationForm localizationForm, ModelDifferenceStore diffstore) {
			this.modelEditorInitialAspect = ((IModelApplicationServices)modelApplication).CurrentAspect;
			this.modelApplication = modelApplication;
			this.localizationImportExports = new Dictionary<string, ILocalizationImportExport>();
			this.localizationForm = localizationForm;
			this.diffstore = diffstore;
			this.defaultLanguage = string.Empty;
			localizationForm.FormClosing += new System.Windows.Forms.FormClosingEventHandler(localizationForm_FormClosing);
			GridListEditor.Grid.HandleCreated += new EventHandler(Grid_HandleCreated);
			GridListEditorForLocalizationForm _gridListEditor = GridListEditor as GridListEditorForLocalizationForm;
			if(_gridListEditor != null) {
				_gridListEditor.SetObjectTypeInfo(XafTypesInfo.Instance.FindTypeInfo(typeof(DevExpress.ExpressApp.Win.Core.ModelEditor.LocalizationItemBind)));
			}
			popupMenu = new ActionsDXPopupMenu();
			popupMenuContainer = new ActionContainerMenuBarItem();
			LocalizationForm.BarManager.Items.Add(popupMenuContainer);
			CreateActions(localizationForm.DefaultContainer, localizationForm.GetContainers());
			RefreshDataSource();
			UpdateTranslatorProvider();
			UpdateActionState();
		}
		internal LocalizationForm LocalizationForm {
			get {
				return localizationForm;
			}
		}
		private void localizationForm_FormClosing(object sender, FormClosingEventArgs e) {
			if(GridView.ActiveEditor != null) {
				GridView.CloseEditor();
			}
			((IModelApplicationServices)modelApplication).CurrentAspectProvider.CurrentAspect = this.modelEditorInitialAspect;
			SaveSettings();
			if(IsModelModified) {
				localizationForm.DialogResult = DialogResult.Yes;
			}
		}
		#region ISupportUpdate
		public void BeginUpdate() {
			isUpdate = true;
		}
		public void EndUpdate() {
			isUpdate = false;
			UpdateActionState();
			LocalizationItems.UpdateDefaultLanguageValues();
		}
		#endregion
		#region Settings
		public void SetSettingsStorage(SettingsStorage settingsStorage) {
			this.settingsStorage = settingsStorage;
			if(GridListEditor.Grid.IsHandleCreated) {
				LoadSettings();
			}
		}
		private void LoadSettings() {
			if(settingsStorage != null) {
				new FormStateAndBoundsManager().Load(localizationForm, settingsStorage);
				defaultLanguage = settingsStorage.LoadOption("", "SourceLanguageCode");
				if(string.IsNullOrEmpty(defaultLanguage)) {
					defaultLanguage = string.Empty;
				}
				string gridViewSettings = settingsStorage.LoadOption("", "GridView");
				if(!string.IsNullOrEmpty(gridViewSettings)) {
					Stream gridViewSettingsStream = new MemoryStream();
					StreamWriter streamWriter = new StreamWriter(gridViewSettingsStream);
					streamWriter.Write(gridViewSettings);
					streamWriter.Flush();
					gridViewSettingsStream.Seek(0, SeekOrigin.Begin);
					GridView.RestoreLayoutFromStream(gridViewSettingsStream);
				}
				string lastFileName = settingsStorage.LoadOption("", "LastFileName");
				if(!string.IsNullOrEmpty(lastFileName) && diffstore != null && diffstore.Name == lastFileName) {
					string translatedLanguage = settingsStorage.LoadOption("", "TranslatedLanguage");
					ChoiceActionItem languageChoiceActionItem = GetLanguageChoiceActionItem(translatedLanguageAction, translatedLanguage);
					if(languageChoiceActionItem != null) {
						translatedLanguageAction.DoExecute(languageChoiceActionItem);
					}
					currentPropertyPath = settingsStorage.LoadOption("", "CurrentPropertyPath");
					RestoreCurrentProperty();
				}
				UpdateActionState();
			}
		}
		private void RestoreCurrentProperty() {
			foreach(ILocalizationItem item in LocalizationItems) {
				if(item.FullPath == currentPropertyPath) {
					GridListEditor.FocusedObject = item;
					break;
				}
			}
		}
		private void SaveSettings() {
			if(settingsStorage != null) {
				new FormStateAndBoundsManager().Save(localizationForm, settingsStorage);
				MemoryStream gridViewSettingsStream = new MemoryStream();
				GridView.SaveLayoutToStream(gridViewSettingsStream);
				string gridViewSettings = System.Text.Encoding.UTF8.GetString(gridViewSettingsStream.ToArray());
				settingsStorage.SaveOption("", "GridView", gridViewSettings);
				settingsStorage.SaveOption("", "SourceLanguageCode", defaultLanguage);
				if(diffstore != null) {
					settingsStorage.SaveOption("", "LastFileName", diffstore.Name);
					settingsStorage.SaveOption("", "TranslatedLanguage", TranslatedLanguage);
					if(GridListEditor.FocusedObject != null) {
						settingsStorage.SaveOption("", "CurrentPropertyPath", ((LocalizationItemBind)GridListEditor.FocusedObject).FullPath);
					}
				}
			}
		}
		#endregion
		#region GridView
		private GridView GridView {
			get { return localizationForm.GridListEditor.GridView; }
		}
		private GridListEditor GridListEditor {
			get { return localizationForm.GridListEditor; }
		}
		private void InitializeGridView() {
			GridView.OptionsView.ShowFooter = true;
			GridView.OptionsView.ShowIndicator = false;
			GridView.OptionsView.ShowGroupPanel = false;
			GridView.OptionsView.RowAutoHeight = true;
			GridView.OptionsSelection.MultiSelect = true;
			DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit repositoryItemMemoEdit = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
			DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit = new XtraEditors.Repository.RepositoryItemCheckEdit();
			repositoryItemMemoEdit.AcceptsReturn = false;
			repositoryItemMemoEdit.WordWrap = true;
			repositoryItemMemoEdit.ScrollBars = ScrollBars.None;
			GridView.Columns["DefaultLanguageValue"].ColumnEdit = repositoryItemMemoEdit;
			GridView.Columns["TranslatedValue"].ColumnEdit = repositoryItemMemoEdit;
			GridView.Columns["IsTranslated"].ColumnEdit = repositoryItemCheckEdit;
			GridView.Columns["IsTranslated"].OptionsColumn.FixedWidth = true;
			GridView.Columns["IsTranslated"].Width = 120;
			GridView.Columns["IsCalculated"].OptionsColumn.FixedWidth = true;
			GridView.Columns["IsCalculated"].Width = 80;
			GridView.Columns["IsCalculated"].OptionsColumn.AllowFocus = false;
			GridView.Columns["PropertyName"].OptionsColumn.AllowFocus = false;
			GridView.Columns["NodePath"].OptionsColumn.AllowFocus = false;
			GridView.Columns["PropertyName"].OptionsColumn.AllowFocus = false;
			GridView.Columns["PropertyName"].SummaryItem.SummaryType = SummaryItemType.Count;
			GridView.Columns["Description"].OptionsColumn.AllowFocus = false;
			GridView.Columns["Description"].ColumnEdit = repositoryItemMemoEdit;
			GridView.OptionsLayout.StoreAllOptions = false;
			GridView.OptionsLayout.StoreAppearance = false;
			GridView.OptionsLayout.StoreVisualOptions = true;
			GridView.OptionsLayout.StoreDataSettings = true;
			GridView.ActiveFilter.Changed += new EventHandler(ActiveFilter_Changed);
			GridView.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(GridView_CustomDrawCell);
			GridView.SelectionChanged += new SelectionChangedEventHandler(GridView_SelectionChanged);
			GridListEditor.ProcessSelectedItem += new EventHandler(GridListEditor_ProcessSelectedItem);
			GridView.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(GridView_CellValueChanged);
			GridView.CellValueChanging += new XtraGrid.Views.Base.CellValueChangedEventHandler(GridView_CellValueChanging);
		}
		private void Grid_HandleCreated(object sender, EventArgs e) {
			InitializeGridView();
			LoadSettings();
			RestoreCurrentProperty();
		}
		private void GridView_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e) {
			object row = GridView.GetRow(e.RowHandle);
			if(row != null) { 
				if(((ILocalizationItem)row).IsModified) {
					e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
				}
			}
		}
		private void GridView_CellValueChanging(object sender, XtraGrid.Views.Base.CellValueChangedEventArgs e) {
			if(GridView.FocusedColumn.FieldName == "IsTranslated") {
				BeginUpdate();
				bool value = (bool)e.Value;
				ItemUpdaterDelegate updater = delegate(LocalizationItemBind item) {
					item.IsTranslated = value;
				};
				Predicate<LocalizationItemBind> needUpdate = delegate(LocalizationItemBind item) {
					return item.IsTranslated != value;
				};
				ApplyValue(updater, needUpdate, false);
				IsModelModified = true;
				EndUpdate();
			}
		}
		private void GridView_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e) {
			BeginUpdate();
			if(GridView.FocusedColumn.FieldName == "TranslatedValue") {
				ApplyTranslatedValue((string)e.Value, false);
			}
			EndUpdate();
			IsModelModified = true;
		}
		private void GridView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			UpdateActionState();
		}
		private void ActiveFilter_Changed(object sender, EventArgs e) {
			UpdateActionState();
		}
		private void actionContainerBarItem_LinkAdded(object sender, LinkEventArgs e) {
			BarItem barItem = e.Link.Item;
			if(barItem.Caption == "Translation Language") {
				barItem.Width = 70;
			}
			else if(barItem.Caption == "Filters") {
				barItem.Alignment = BarItemLinkAlignment.Right;
				barItem.Width = 170;
			}
		}
		private void GridListEditor_ProcessSelectedItem(object sender, EventArgs e) {
			if(GridView.ActiveEditor == null) {
				GridView.ShowEditor();
			}
		}
		#endregion
		#region ApplyTranslatedValue
		private void ApplyValue(ItemUpdaterDelegate updater, Predicate<LocalizationItemBind> needUpdate, bool moveNext) {
			if(GridListEditor.FocusedObject != null) {
				ApplyValue((LocalizationItemBind)GridListEditor.FocusedObject, updater, needUpdate, moveNext);
			}
		}
		private void ApplyTranslatedValue(string value, bool moveNext) {
			if(GridListEditor.FocusedObject != null) {
				LocalizationItemBind currentItem = (LocalizationItemBind)GridListEditor.FocusedObject;
				ItemUpdaterDelegate updater = delegate(LocalizationItemBind item) {
					item.TranslatedValue = value;
				};
				Predicate<LocalizationItemBind> needUpdate = delegate(LocalizationItemBind item) {
					return !item.IsTranslated;
				};
				ApplyValue(currentItem, updater, needUpdate, moveNext);
			}
		}
		private void UpdateItemsCore(IList<ILocalizationItem> selectedItems, ItemUpdaterDelegate updater, Predicate<LocalizationItemBind> needCollect, ReplaceAllItemsDelegate replaceAllItemsDelegate) {
			if(selectedItems.Count > 0) {
				while(selectedItems.Count > 0) {
					LocalizationItemBind currentItem = (LocalizationItemBind)selectedItems[0];
					List<ILocalizationItem> sameLocalizationItems = FindSameLocalizationItems(currentItem, needCollect);
					if(sameLocalizationItems.Count > 1) {
						if(replaceAllItemsDelegate(sameLocalizationItems.Count, currentItem.DefaultLanguageValue)) {
							foreach(LocalizationItemBind item in sameLocalizationItems) {
								selectedItems.Remove(item);
								updater(item);
							}
						}
						else {
							selectedItems.Remove(currentItem);
							updater(currentItem);
						}
					}
					else {
						selectedItems.Remove(currentItem);
						updater(currentItem);
					}
				}
			}
		}
		private void ApplyValue(LocalizationItemBind currentLocalizationItem, ItemUpdaterDelegate updater, Predicate<LocalizationItemBind> needUpdate, bool moveNext) {
			List<ILocalizationItem> sameLocalizationItems = FindSameLocalizationItems(currentLocalizationItem, needUpdate);
			LocalizationItemBind prevFocusedObject = currentLocalizationItem;
			if(sameLocalizationItems.Count > 1) {
				if(ReplaceAllItems(sameLocalizationItems.Count, currentLocalizationItem.DefaultLanguageValue)) {
					foreach(ILocalizationItem item in sameLocalizationItems) {
						updater((LocalizationItemBind)item);
					}
					do {
						prevFocusedObject = (LocalizationItemBind)GridListEditor.FocusedObject;
						GridView.MoveNext();
					} while(sameLocalizationItems.Contains((LocalizationItemBind)GridListEditor.FocusedObject) &&
							 prevFocusedObject != GridListEditor.FocusedObject);
					localizationItems.Refresh();
				}
			}
			if(moveNext && prevFocusedObject == GridListEditor.FocusedObject) {
				GridView.MoveNext();
			}
		}
		private bool ReplaceAllItems(int itemsCount, string defaultLanguageValue) {
			if(replaceAlways == null) {
				IReplaceQuestion replaceQuestion = ObjectFactory.CreateInstance<IReplaceQuestion, ReplaceQuestionForm>();
				replaceQuestion.SetValues(itemsCount, defaultLanguageValue);
				bool dialogResult = replaceQuestion.ShowDialog() == DialogResult.Yes;
				if(replaceQuestion.ShowDialogAgain) {
					replaceAlways = dialogResult;
				}
				return dialogResult;
			}
			return replaceAlways.Value;
		}
		private List<ILocalizationItem> FindSameLocalizationItems(LocalizationItemBind item, Predicate<LocalizationItemBind> needCollect) {
			List<ILocalizationItem> result = new List<ILocalizationItem>();
			for(int i = 0; i < GridView.RowCount; i++) {
				LocalizationItemBind localizationItem = (LocalizationItemBind)GridView.GetRow(i);
				if(localizationItem.DefaultLanguageValue == item.DefaultLanguageValue && (needCollect == null || needCollect(localizationItem))) {
					result.Add(localizationItem);
				}
			}
			return result;
		}
		#endregion
		#region Actions Create and Update State
		private void CreateActions(IActionContainer defaultActionContainer, ICollection<IActionContainer> actionContainers) {
			ActionContainerBarItem defaultActionContainerBarItem = defaultActionContainer as ActionContainerBarItem;
			defaultActionContainerBarItem.LinkAdded += new LinkEventHandler(actionContainerBarItem_LinkAdded);
			IActionContainer editActionContainer = FindActionContainer(DevExpress.Persistent.Base.PredefinedCategory.Edit.ToString(), actionContainers);
			IActionContainer saceActionContainer = FindActionContainer(DevExpress.Persistent.Base.PredefinedCategory.Save.ToString(), actionContainers);
			saveAction = new SimpleAction();
			saveAction.Caption = "&Save";
			saveAction.Category = DevExpress.Persistent.Base.PredefinedCategory.Save.ToString();
			saveAction.Id = "Save";
			saveAction.ImageName = "MenuBar_Save";
			saveAction.ToolTip = "Save the changes";
			saveAction.Shortcut = "Control+S";
			saveAction.Execute += new SimpleActionExecuteEventHandler(saveAction_Execute);
			saceActionContainer.Register(saveAction);
			importAction = new SimpleAction();
			importAction.Caption = "Import...";
			importAction.Category = "ImportExport";
			importAction.ToolTip = importAction.Caption;
			importAction.Shortcut = "Control+I";
			importAction.ImageName = "Action_LocalizationImport";
			importAction.Execute += new SimpleActionExecuteEventHandler(import_Execute);
			editActionContainer.Register(importAction);
			exportAction = new SingleChoiceAction();
			exportAction.Caption = "Export...";
			exportAction.Category = "ImportExport";
			exportAction.ToolTip = exportAction.Caption;
			exportAction.Shortcut = "Control+E";
			exportAction.ImageName = "Action_LocalizationExport";
			exportAction.ItemType = SingleChoiceActionItemType.ItemIsOperation;
			exportAction.ShowItemsOnClick = true;
			exportAction.Execute += new SingleChoiceActionExecuteEventHandler(export_Execute);
			ChoiceActionItem allExportItem = new ChoiceActionItem();
			allExportItem.Caption = "All records";
			allExportItem.Data = LocalizationExportType.AllValues;
			exportAction.Items.Add(allExportItem);
			ChoiceActionItem modifiedExportItem = new ChoiceActionItem();
			modifiedExportItem.Caption = "Modified records";
			modifiedExportItem.Data = LocalizationExportType.ModifiedValues;
			exportAction.Items.Add(modifiedExportItem);
			ChoiceActionItem selectedExportItem = new ChoiceActionItem();
			selectedExportItem.Caption = "Selected records";
			selectedExportItem.Data = LocalizationExportType.SelectedValues;
			exportAction.Items.Add(selectedExportItem);
			editActionContainer.Register(exportAction);
			translatedLanguageAction = new SingleChoiceAction();
			translatedLanguageAction.Caption = "Translation Language";
			translatedLanguageAction.ToolTip = translatedLanguageAction.Caption;
			translatedLanguageAction.PaintStyle = ActionItemPaintStyle.Caption;
			translatedLanguageAction.Execute += new SingleChoiceActionExecuteEventHandler(aspectAction_Execute);
			defaultActionContainer.Register(translatedLanguageAction);
			selectAllAction = new SimpleAction();
			selectAllAction.Caption = "Select All";
			selectAllAction.Shortcut = "Control+A";
			selectAllAction.Execute += new SimpleActionExecuteEventHandler(selectAllAction_Execute);
			translateAction = new SimpleAction();
			translateAction.Caption = "Translate...";
			translateAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
			translateAction.ToolTip = string.Format("Translate the selected records via the {0} service.", TranslatorProvider.GetProvider() == null ? "internet" : TranslatorProvider.GetProvider().Caption);
			translateAction.Shortcut = "Control+T";
			translateAction.ImageName = "Action_Translate";
			translateAction.Execute += new SimpleActionExecuteEventHandler(translateAction_Execute);
			defaultActionContainer.Register(translateAction);
			predefinedFiltersAction = new SingleChoiceAction();
			predefinedFiltersAction.Caption = "Filters";
			predefinedFiltersAction.ToolTip = predefinedFiltersAction.Caption;
			predefinedFiltersAction.ImageName = "Action_Filter";
			predefinedFiltersAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
			predefinedFiltersAction.ItemType = SingleChoiceActionItemType.ItemIsMode;
			predefinedFiltersAction.Execute += new SingleChoiceActionExecuteEventHandler(predefinedFiltersAction_Execute);
			defaultActionContainer.Register(predefinedFiltersAction);
			CreateFilters();
			undoAction = new SimpleAction();
			undoAction.Caption = "Undo";
			undoAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
			undoAction.Shortcut = "Control+Z";
			undoAction.ImageName = "Action_Cancel";
			undoAction.Execute += new SimpleActionExecuteEventHandler(undoAction_Execute);
			markAsTranslated = new SimpleAction();
			markAsTranslated.Caption = "Mark as translated";
			markAsTranslated.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
			markAsTranslated.Shortcut = "Control+T";
			markAsTranslated.ImageName = "Action_Grant";
			markAsTranslated.Execute += new SimpleActionExecuteEventHandler(markAsTranslated_Execute);
			popupMenuContainer.Register(selectAllAction);
			popupMenuContainer.Register(markAsTranslated);
			popupMenuContainer.Register(undoAction);
			popupMenuContainer.Register(exportAction);
			popupMenuContainer.Register(importAction);
			popupMenuContainer.Register(translateAction);
			popupMenu.CreateActionItems(LocalizationForm, GridListEditor, new IActionContainer[] { popupMenuContainer });
			FillAspectChoiceActionsItems();
			SaveActionUpadate();
		}
		private ChoiceActionItem noneActionItem;
		private IActionContainer FindActionContainer(string actionContainerId, IEnumerable<IActionContainer> actionContainers) {
			foreach(IActionContainer actionContainer in actionContainers) {
				if(actionContainer.ContainerId == actionContainerId) {
					return actionContainer;
				}
			}
			return null;
		}
		private void CreateFilters() {
			UnaryOperator notNullDefaultLanguageValueOperator = new UnaryOperator(UnaryOperatorType.Not, new UnaryOperator(UnaryOperatorType.IsNull, "DefaultLanguageValue"));
			CriteriaOperator notEmptyDefaultLanguageValueOperator = CriteriaOperator.And(notNullDefaultLanguageValueOperator, new BinaryOperator("DefaultLanguageValue", "", BinaryOperatorType.NotEqual));
			BinaryOperator notCalculatedOperator = new BinaryOperator("IsCalculated", false, BinaryOperatorType.Equal);
			BinaryOperator calculatedOperator = new BinaryOperator("IsCalculated", true, BinaryOperatorType.Equal);
			BinaryOperator notChangedOperator = new BinaryOperator("IsTranslated", false, BinaryOperatorType.Equal);
			BinaryOperator changedOperator = new BinaryOperator("IsTranslated", true, BinaryOperatorType.Equal);
			noneActionItem = new ChoiceActionItem();
			noneActionItem.Caption = "None";
			noneActionItem.Data = null;
			predefinedFiltersAction.Items.Add(noneActionItem);
			ChoiceActionItem notChangedNotCalculatedActionItem = new ChoiceActionItem();
			notChangedNotCalculatedActionItem.Caption = "Untranslated non-calculated";
			notChangedNotCalculatedActionItem.Data = CriteriaOperator.And(notEmptyDefaultLanguageValueOperator, notCalculatedOperator, notChangedOperator);
			predefinedFiltersAction.Items.Add(notChangedNotCalculatedActionItem);
			ChoiceActionItem notChangedCalculatedActionItem = new ChoiceActionItem();
			notChangedCalculatedActionItem.Caption = "Untranslated calculated";
			notChangedCalculatedActionItem.Data = CriteriaOperator.And(notEmptyDefaultLanguageValueOperator, calculatedOperator, notChangedOperator);
			predefinedFiltersAction.Items.Add(notChangedCalculatedActionItem);
			ChoiceActionItem notChangedActionItem = new ChoiceActionItem();
			notChangedActionItem.Caption = "Untranslated";
			notChangedActionItem.Data = CriteriaOperator.And(notEmptyDefaultLanguageValueOperator, notChangedOperator);
			predefinedFiltersAction.Items.Add(notChangedActionItem);
			ChoiceActionItem changedActionItem = new ChoiceActionItem();
			changedActionItem.Caption = "Translated";
			changedActionItem.Data = CriteriaOperator.And(notEmptyDefaultLanguageValueOperator, changedOperator);
			predefinedFiltersAction.Items.Add(changedActionItem);
			ChoiceActionItem notCalculatedActionItem = new ChoiceActionItem();
			notCalculatedActionItem.Caption = "Non-calculated";
			notCalculatedActionItem.Data = CriteriaOperator.And(notEmptyDefaultLanguageValueOperator, notCalculatedOperator);
			predefinedFiltersAction.Items.Add(notCalculatedActionItem);
			ChoiceActionItem calculatedActionItem = new ChoiceActionItem();
			calculatedActionItem.Caption = "Calculated";
			calculatedActionItem.Data = CriteriaOperator.And(notEmptyDefaultLanguageValueOperator, calculatedOperator);
			predefinedFiltersAction.Items.Add(calculatedActionItem);
			ChoiceActionItem customActionItem = new ChoiceActionItem();
			customActionItem.Caption = "Custom";
			predefinedFiltersAction.Items.Add(customActionItem);
		}
		private void FillAspectChoiceActionsItems() {
			foreach(string aspect in ModelEditorHelper.GetAspectNames(modelApplication)) {
				ChoiceActionItem actionItem = new ChoiceActionItem(aspect, aspect);
				if(aspect != CaptionHelper.DefaultLanguage) {
					translatedLanguageAction.Items.Add(actionItem);
				}
			}
			translatedLanguageAction.SelectedIndex = 0;
		}
		public void UpdateActionState() {
			if(!isUpdate) {
				translateAction.Enabled["IsExistsTranslatorProvider"] = TranslatorProvider.GetProvider() != null;
				UpdateUndoAction();
				AupdateMarkAsTranslatedAction();
				UpdateSelectedFilter();
			}
		}
		private void UpdateUndoAction() {
			bool result = false;
			foreach(LocalizationItemBind localizationItem in GetSelectedLocalizationItems()) {
				if(localizationItem.IsModified) {
					result = true;
					break;
				}
			}
			undoAction.Enabled.SetItemValue("Item(s) is modefied", result);
		}
		private void AupdateMarkAsTranslatedAction() {
			bool result = false;
			foreach(LocalizationItemBind localizationItem in GetSelectedLocalizationItems()) {
				if(!localizationItem.IsTranslated) {
					result = true;
					break;
				}
			}
			markAsTranslated.Enabled.SetItemValue("Item(s) is not translated", result);
		}
		private void UpdateSelectedFilter() {
			foreach(ChoiceActionItem item in predefinedFiltersAction.Items) {
				if(item.Caption == "Custom" || (object.ReferenceEquals(GridView.ActiveFilterCriteria, null) && item.Data == null) || (item.Data != null && GridView.ActiveFilterCriteria.ToString() == item.Data.ToString())) {
					predefinedFiltersAction.SelectedItem = item;
					break;
				}
			}
		}
		#endregion
		#region Translate Action
		private string GetTranslatorSupportedLanguageCode() {
			string result = null;
			List<string> supportLanguagesList = new List<string>();
			string translatedLanguageSmallCode = GetLanguageCodeByAspect(TranslatedLanguage);
			if(TranslatorProvider.GetProvider() != null && TranslatedLanguage != null) {
				try {
					supportLanguagesList = new List<string>(TranslatorProvider.GetProvider().GetLanguages());
				}
				catch(Exception e) {
					Messaging.GetMessaging(null).Show(e.Message, "Translator error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				if(supportLanguagesList.Contains(TranslatedLanguage)) {
					result = TranslatedLanguage;
				}
				else if(supportLanguagesList.Contains(translatedLanguageSmallCode)) {
					result = translatedLanguageSmallCode;
				}
			}
			if(string.IsNullOrEmpty(result) && supportLanguagesList.Count > 0) {
				Messaging.GetMessaging(null).Show("The target language is not supported.", "Translator error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			return result;
		}
		private void UpdateTranslatorProvider() {
			if(TranslatorProvider.GetProvider() == null) {
				TranslatorProvider.RegisterProvider(new BingTranslatorProvider());
			}
		}
		private void translateAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			string translatedLanguageCode = GetTranslatorSupportedLanguageCode();
			if(!string.IsNullOrEmpty(translatedLanguageCode)) {
				ITranslationOptions translationOptionsForm = ObjectFactory.CreateInstance<ITranslationOptions>(
					delegate() {
						return new TranslationOptionsForm(defaultLanguage, TranslatedLanguage, TranslatorProvider.GetProvider());
					}
				);
				if(translationOptionsForm.ShowDialog() == DialogResult.OK) {
					defaultLanguage = translationOptionsForm.SourceLanguageCode;
					LocalizationForm.Refresh();
					TranslateWorker worker = new TranslateWorker();
					List<string> texts = CollectTextsForTranslate();
					Dictionary<string, string> translation = worker.Start(TranslatorProvider.GetProvider(), localizationForm, defaultLanguage, translatedLanguageCode, texts.ToArray());
					if(translation != null) {
						ApplyTranslation(translation);
					}
				}
			}
		}
		private List<string> CollectTextsForTranslate() {
			List<string> result = new List<string>();
			foreach(ILocalizationItem item in GetSelectedLocalizationItems()) {
				string text = item.DefaultLanguageValue;
				if(text != null && !result.Contains(text)) {
					result.Add(text);
				}
			}
			return result;
		}
		private void ApplyTranslation(Dictionary<string, string> translation) {
			BeginUpdate();
			string translatedText = null;
			List<string> defaultValues = new List<string>();
			Dictionary<string, int> sameSelectedLocalizationItems = new Dictionary<string, int>();
			foreach(ILocalizationItem item in GetSelectedLocalizationItems()) {
				if(!string.IsNullOrEmpty(item.DefaultLanguageValue)) {
					if(!sameSelectedLocalizationItems.ContainsKey(item.DefaultLanguageValue)) {
						sameSelectedLocalizationItems.Add(item.DefaultLanguageValue, 1);
					}
					else {
						sameSelectedLocalizationItems[item.DefaultLanguageValue]++;
					}
				}
			}
			Dictionary<string, int> sameAllLocalizationItems = new Dictionary<string, int>();
			for(int i = 0; i < GridView.RowCount; i++) {
				LocalizationItemBind item = (LocalizationItemBind)GridView.GetRow(i);
				if(!string.IsNullOrEmpty(item.DefaultLanguageValue)) {
					if(!sameAllLocalizationItems.ContainsKey(item.DefaultLanguageValue)) {
						sameAllLocalizationItems.Add(item.DefaultLanguageValue, 1);
					}
					else {
						sameAllLocalizationItems[item.DefaultLanguageValue]++;
					}
				}
			}
			foreach(LocalizationItemBind item in GetSelectedLocalizationItems()) {
				string text = item.DefaultLanguageValue;
				if(!string.IsNullOrEmpty(item.DefaultLanguageValue) && translation.TryGetValue(text, out translatedText)) {
					IsModelModified = true;
					item.TranslatedValue = translatedText;
					int sameAllItemsCount = 0;
					sameAllLocalizationItems.TryGetValue(item.DefaultLanguageValue, out sameAllItemsCount);
					int sameSelectedItemsCount = 0;
					sameSelectedLocalizationItems.TryGetValue(item.DefaultLanguageValue, out sameSelectedItemsCount);
					if(sameAllItemsCount != sameSelectedItemsCount && !defaultValues.Contains(item.DefaultLanguageValue)) {
						GridListEditor.FocusedObject = item;
						ApplyTranslatedValue(item.TranslatedValue, true);
						defaultValues.Add(item.DefaultLanguageValue);
					}
				}
			}
			EndUpdate();
		}
		public void RegisterLocalizationImportExport(ILocalizationImportExport localizationDocument) {
			localizationImportExports.Add(localizationDocument.Extension, localizationDocument);
		}
		#endregion
		#region ImportExport Actions
		private void export_Execute(object sender, SingleChoiceActionExecuteEventArgs e) {
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			List<string> filters = new List<string>();
			foreach(KeyValuePair<string, ILocalizationImportExport> localizationImportExport in localizationImportExports) {
				filters.Add(localizationImportExport.Value.Filter);
			}
			saveFileDialog.Filter = string.Join("|", filters.ToArray());
			if(saveFileDialog.ShowDialog() == DialogResult.OK) {
				string fileName = saveFileDialog.FileName;
				try {
					FileInfo fileInfo = new FileInfo(fileName);
					if(localizationImportExports.ContainsKey(fileInfo.Extension)) {
						ILocalizationImportExport localizationImportExport = localizationImportExports[fileInfo.Extension];
						using(System.IO.FileStream fileStream = new System.IO.FileStream(fileName, System.IO.FileMode.OpenOrCreate)) {
							fileStream.SetLength(0);
							IEnumerable<ILocalizationItem> localizationItems = null;
							switch(((LocalizationExportType)e.SelectedChoiceActionItem.Data)) {
								case LocalizationExportType.AllValues:
									localizationItems = GetAllLocalizationItems();
									break;
								case LocalizationExportType.ModifiedValues:
									localizationItems = LocalizationItems.GetModifiedLocalizationItems();
									break;
								case LocalizationExportType.SelectedValues:
									localizationItems = GetSelectedLocalizationItems();
									break;
							}
							LocalizationDocumentData data = new LocalizationDocumentData(localizationItems, defaultLanguage, TranslatedLanguage);
							localizationImportExport.Export(fileStream, data);
						}
					}
				}
				catch(Exception exception) {
					XtraMessageBox.Show(exception.Message, "Export exception");
				}
			}
		}
		private void import_Execute(object sender, SimpleActionExecuteEventArgs e) {
			OpenFileDialog openFileDialog = new OpenFileDialog();
			List<string> filters = new List<string>();
			foreach(KeyValuePair<string, ILocalizationImportExport> localizationDocument in localizationImportExports) {
				filters.Add(localizationDocument.Value.Filter);
			}
			openFileDialog.Filter = string.Join("|", filters.ToArray());
			if(openFileDialog.ShowDialog() == DialogResult.OK) {
				string fileName = openFileDialog.FileName;
				try {
					FileInfo fileInfo = new FileInfo(fileName);
					if(localizationImportExports.ContainsKey(fileInfo.Extension)) {
						System.IO.FileStream fileStream = new System.IO.FileStream(fileName, System.IO.FileMode.Open);
						ILocalizationImportExport localizationImportExport = localizationImportExports[fileInfo.Extension];
						LocalizationDocumentData data = localizationImportExport.Import(fileStream);
						fileStream.Close();
						ApplyLocalizationDocumentData(data);
					}
				}
				catch(Exception exception) {
					XtraMessageBox.Show(exception.Message, "Import exception");
				}
			}
		}
		private ChoiceActionItem GetLanguageChoiceActionItem(SingleChoiceAction aspectAction, string aspect) {
			foreach(ChoiceActionItem choiceActionItem in aspectAction.Items) {
				if(choiceActionItem.Caption == aspect) {
					return choiceActionItem;
				}
			}
			return null;
		}
		private bool ChangeTranslatedLanguage(string destinationAspect) {
			if(destinationAspect != TranslatedLanguage) {
				ChoiceActionItem destinationActionItem = GetLanguageChoiceActionItem(translatedLanguageAction, destinationAspect);
				if(destinationActionItem == null) {
					Messaging.GetMessaging(null).Show(string.Format("The specified file is designed for the \"{0}\" language", destinationAspect), "Localization", MessageBoxButtons.OK, MessageBoxIcon.Information);
					return false;
				}
				else {
					RefreshDataSource();
					translatedLanguageAction.BeginUpdate();
					translatedLanguageAction.SelectedItem = destinationActionItem;
					translatedLanguageAction.EndUpdate();
				}
			}
			return true;
		}
		private Dictionary<string, LocalizationItemBind> CreateLocalizationItemsHash() {
			Dictionary<string, LocalizationItemBind> localizationItemsHash = new Dictionary<string, LocalizationItemBind>();
			foreach(LocalizationItemBind item in LocalizationItems) {
				if(!localizationItemsHash.ContainsKey(item.FullPath)) {
					localizationItemsHash.Add(item.FullPath, item);
				}
			}
			return localizationItemsHash;
		}
		private void ApplyLocalizationDocumentData(LocalizationDocumentData localizationDocumentData) {
			if(!ChangeTranslatedLanguage(localizationDocumentData.DestinationAspect)) {
				return;
			}
			defaultLanguage = localizationDocumentData.SourceAspect;
			Dictionary<string, LocalizationItemBind> localizationItemsHash = CreateLocalizationItemsHash();
			ChoiceActionItem oldFiltersActionItem = predefinedFiltersAction.SelectedItem;
			IImportQuestion importQuestion = ObjectFactory.CreateInstance<IImportQuestion, ImportQuestionForm>();
			ImportSplashScreenManager importWorker = new ImportSplashScreenManager();
			try {
				GridListEditor.BeginUpdate();
				List<ILocalizationItem> localizationDocumentDataLocalizationItems = new List<ILocalizationItem>(localizationDocumentData.LocalizationItems);
				int remainingElements = localizationDocumentDataLocalizationItems.Count;
				foreach(ILocalizationItem item in localizationDocumentDataLocalizationItems) {
					LocalizationItemBind foundItem = null;
					if(localizationItemsHash.TryGetValue(item.FullPath, out foundItem)) {
						string foundItemDefaultLanguageValue = foundItem.DefaultLanguageValue;
						string itemDefaultLanguageValue = item.DefaultLanguageValue;
						string foundItemTranslatedValue = foundItem.TranslatedValue;
						string itemTranslatedValue = item.TranslatedValue;
						bool isTranslated = item.IsTranslated;
						if(!string.IsNullOrEmpty(foundItemDefaultLanguageValue) && !string.IsNullOrEmpty(itemDefaultLanguageValue) ||
							((!string.IsNullOrEmpty(foundItemTranslatedValue) && !string.IsNullOrEmpty(itemTranslatedValue)) || !string.IsNullOrEmpty(itemTranslatedValue))) {
							if(predefinedFiltersAction.SelectedItem != noneActionItem) {
								predefinedFiltersAction.DoExecute(noneActionItem);
							}
							if(importQuestion.IsYesForAll) {
								importWorker.Start(localizationForm);
							}
							else {
								importQuestion.SetValues(itemDefaultLanguageValue, itemTranslatedValue);
								switch(importQuestion.ShowDialog()) {
									case DialogResult.No:
										continue;
									case DialogResult.Cancel:
										return;
								}
							}
							if(!string.IsNullOrEmpty(itemDefaultLanguageValue)) {
								foundItem.DefaultLanguageValue = itemDefaultLanguageValue;
							}
							if(!string.IsNullOrEmpty(itemTranslatedValue)) {
								foundItem.TranslatedValue = itemTranslatedValue;
							}
							if(!foundItem.IsTranslated && isTranslated) {
								foundItem.IsTranslated = isTranslated;
							}
							IsModelModified = true;
						}
					}
					remainingElements--;
					importWorker.SetProgress(localizationDocumentDataLocalizationItems.Count, remainingElements);
					if(importWorker.Abort) {
						break;
					}
				}
			}
			finally {
				importWorker.Stop();
				GridListEditor.EndUpdate();
				if(predefinedFiltersAction.SelectedItem != oldFiltersActionItem) {
					predefinedFiltersAction.DoExecute(oldFiltersActionItem);
				}
			}
		}
		#endregion
		#region Other Actions
		private void ItemUndo(LocalizationItemBind item) {
			item.Undo();
		}
		private bool NeedUndo(LocalizationItemBind item) {
			return item.IsModified;
		}
		private bool NeedMarkAsTranclated(LocalizationItemBind item) {
			return !item.IsTranslated;
		}
		private void ItemMarkAsTranclated(LocalizationItemBind item) {
			item.IsTranslated = true;
		}
		private void undoAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			BeginUpdate();
			IList<ILocalizationItem> selectedItems = new List<ILocalizationItem>(GetSelectedLocalizationItems());
			UpdateItemsCore(selectedItems, ItemUndo, NeedUndo, ReplaceAllItems);
			EndUpdate();
			IsModelModified = true;
		}
		private void markAsTranslated_Execute(object sender, SimpleActionExecuteEventArgs e) {
			BeginUpdate();
			IList<ILocalizationItem> selectedItems = new List<ILocalizationItem>(GetSelectedLocalizationItems());
			UpdateItemsCore(selectedItems, ItemMarkAsTranclated, NeedMarkAsTranclated, ReplaceAllItems);
			EndUpdate();
			IsModelModified = true;
		}
		private void selectAllAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			GridView.SelectAll();
		}
		private void predefinedFiltersAction_Execute(object sender, SingleChoiceActionExecuteEventArgs e) {
			if(e.SelectedChoiceActionItem.Caption == "Custom") {
				GridView.ShowFilterEditor(null);
			}
			else {
				GridView.ActiveFilterCriteria = (CriteriaOperator)e.SelectedChoiceActionItem.Data;
			}
			UpdateActionState();
		}
		private void aspectAction_Execute(object sender, SingleChoiceActionExecuteEventArgs e) {
			RefreshDataSource();
		}
		#endregion
		LocalizationItems localizationItems = null;
		private void RefreshDataSource() {
			if(GridListEditor.FocusedObject != null) {
				currentPropertyPath = ((LocalizationItemBind)GridListEditor.FocusedObject).FullPath;
			}
			localizationItems = GetAllLocalizationItems();
			GridView.OptionsBehavior.AutoPopulateColumns = true;
			GridListEditor.DataSource = localizationItems;
			GridListEditor.Refresh();
			localizationItems.ListChanged += new ListChangedEventHandler(localizationItems_ListChanged);
			RestoreCurrentProperty();
			UpdateActionState();
		}
		private LocalizationItems LocalizationItems {
			get { return (LocalizationItems)GridListEditor.DataSource; }
		}
		private void localizationItems_ListChanged(object sender, ListChangedEventArgs e) {
			UpdateActionState();
		}
		private LocalizationItems GetAllLocalizationItems() {
			return new LocalizationItems(modelApplication, TranslatedLanguage);
		}
		private IEnumerable<ILocalizationItem> GetSelectedLocalizationItems() {
			foreach(ILocalizationItem item in GridListEditor.GetSelectedObjects()) {
				yield return item;
			}
		}
		private string GetLanguageCodeByAspect(string aspect) {
			return aspect.Split('-')[0];
		}
		private string TranslatedLanguage {
			get {
				string result = null;
				if(translatedLanguageAction.SelectedItem != null) {
					result = (string)translatedLanguageAction.SelectedItem.Data;
				}
				return result;
			}
		}
		public ModelNode CurrentModelNode {
			get {
				if(GridListEditor.FocusedObject != null) {
					string modelNodePath = ((LocalizationItemBind)GridListEditor.FocusedObject).NodePath;
					return ModelEditorHelper.FindNodeByPath(modelNodePath, modelApplication);
				}
				return null;
			}
		}
		public bool IsModelModified {
			get {
				return isModelModified;
			}
			private set {
				bool oldValue = isModelModified;
				isModelModified = value;
				if(isModelModified && isModelModified != oldValue) {
					CancelEventArgs args = new CancelEventArgs(false);
					if(Modifying != null) {
						Modifying(this, args);
					}
					if(!args.Cancel) {
						SaveActionUpadate();
					}
				}
			}
		}
		private void saveAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			if(SaveChanges != null) {
				SaveChanges(this, EventArgs.Empty);
				IsModelModified = false;
				SaveActionUpadate();
			}
		}
		private void SaveActionUpadate() {
			saveAction.Enabled.SetItemValue("CanSaveChanges", SaveChanges != null);
			saveAction.Enabled.SetItemValue("IsModified", IsModelModified);
		}
		public event CancelEventHandler Modifying;
		public event EventHandler SaveChanges;
#if DebugTest
		public void DebugTest_ApplyTranslatedValue(string value) {
			ApplyTranslatedValue(value, true);
		}
		public bool DebugTest_ReplaceAllItems(int itemsCount, string defaultLanguageValue) {
			return ReplaceAllItems(itemsCount, defaultLanguageValue);
		}
		public void DebugTest_ApplyTranslation(Dictionary<string, string> translation) {
			ApplyTranslation(translation);
		}
		public void DebugTest_LoadSettings() {
			LoadSettings();
		}
		public void DebugTest_ApplyLocalizationDocumentData(LocalizationDocumentData localizationDocumentData) {
			ApplyLocalizationDocumentData(localizationDocumentData);
		}
		public SimpleAction DebugTest_TranslateAction {
			get { return translateAction; }
		}
		public SimpleAction DebugTest_SelectAllAction {
			get { return selectAllAction; }
		}
		public SimpleAction DebugTest_UndoAction {
			get { return undoAction; }
		}
		public SimpleAction DebugTest_MarkAsTranslated {
			get { return markAsTranslated; }
		}
		public SingleChoiceAction DebugTest_PredefinedFiltersAction {
			get { return predefinedFiltersAction; }
		}
		public SingleChoiceAction DebugTest_TranslatedLanguageAction {
			get { return translatedLanguageAction; }
		}
		public LocalizationItems DebugTest_LocalizationItems {
			get { return LocalizationItems; }
		}
		public GridView DebugTest_GridView {
			get { return GridView; }
		}
		public GridListEditor DebugTest_GridListEditor {
			get { return GridListEditor; }
		}
		public string DebugTest_TranslatedLanguage {
			get { return TranslatedLanguage; }
		}
		public string DebugTest_DefaultLanguage {
			get { return defaultLanguage; }
		}
		public bool? DebugTest_ReplaceAlways {
			get { return replaceAlways; }
			set { replaceAlways = value; }
		}
		public void DebugTest_UpdateItemsCore(IList<ILocalizationItem> selectedItems, ItemUpdaterDelegate updater, Predicate<LocalizationItemBind> needCollect) {
			UpdateItemsCore(selectedItems, updater, needCollect, ReplaceAllItems);
		}
#endif
	}
	public class LocalizationItems : BindingList<ILocalizationItem> {
		private IModelApplicationServices modelApplicationServices;
		private string translatedLanguage;
		public LocalizationItems(ModelApplicationBase modelApplication, string translatedLanguage) {
			this.modelApplicationServices = modelApplication;
			this.translatedLanguage = translatedLanguage;
			SetCurrentAspect(string.Empty);
			CreateLocalizationItemsFromModel(modelApplication, translatedLanguage);
			SetCurrentAspect(translatedLanguage);
		}
		public IEnumerable<ILocalizationItem> GetModifiedLocalizationItems() {
			foreach(ILocalizationItem item in this) {
				if(item.IsModified) {
					yield return item;
				}
			}
		}
		private void CreateLocalizationItemsFromModel(ModelNode node, string translatedLanguage) {
			foreach(ModelValueInfo valueInfo in node.NodeInfo.ValuesInfo) {
				if(valueInfo.IsLocalizable) {
					Add(new LocalizationItemBind(node, valueInfo.Name, translatedLanguage));
				}
			}
			for(int i = 0; i < node.NodeCount; i++) {
				CreateLocalizationItemsFromModel(node.GetNode(i), translatedLanguage);
			}
		}
		private void SetCurrentAspect(string aspect) {
			modelApplicationServices.CurrentAspectProvider.CurrentAspect = aspect;
		}
		public void UpdateDefaultLanguageValues() {
			SetCurrentAspect(string.Empty);
			foreach(LocalizationItemBind item in this) {
				item.UpdateDefaultLanguageValue();
			}
			SetCurrentAspect(translatedLanguage);
		}
		public void Refresh() {
			OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
		}
	}
}
