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
using System.Linq;
using System.Windows.Forms;
using DevExpress.Utils.Filtering;
using DevExpress.Utils.Filtering.Internal;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Filtering.Repository;
using DevExpress.XtraEditors.Repository;
namespace DevExpress.XtraEditors.Filtering {
	[ToolboxItem(false)]
	public class FilterUIEditorTemplateContainer : IDisposable {
		public FilterUIEditorTemplateContainer(RepositoryItemFilterUIEditorContainerEdit ownerEditProperties) {
			this.ownerEditProperties = ownerEditProperties;
		}
		public FilterUIEditorTemplateContainer(RepositoryItemFilterUIEditorPopupContainerEdit ownerEditProperties) {
			this.ownerPopupEditProperties = ownerEditProperties;
		}
		public void Dispose() {
			npcActions.Clear();
			UnsubscribeValueViewModelEvents();
			if(templateCore != null) {
				templateCore.HandleCreated -= templateCore_HandleCreated;
				templateCore.Dispose();
			}
			GC.SuppressFinalize(this);
		}
		IValueViewModel valueViewModelCore;
		public IValueViewModel ValueViewModel {
			get { return valueViewModelCore; }
			set {
				if(valueViewModelCore == value) return;
				UnsubscribeValueViewModelEvents();
				valueViewModelCore = value;
				SubscribeValueViewModelEvents();
				OnValueViewModelChanged();
			}
		}
		void SubscribeValueViewModelEvents() {
			if(ValueViewModel != null)
				((INotifyPropertyChanged)ValueViewModel).PropertyChanged += ValueViewModel_PropertyChanged;
		}
		void UnsubscribeValueViewModelEvents() {
			if(ValueViewModel != null)
				((INotifyPropertyChanged)ValueViewModel).PropertyChanged -= ValueViewModel_PropertyChanged;
		}
		RepositoryItemFilterUIEditorContainerEdit ownerEditProperties;
		protected RepositoryItemFilterUIEditorContainerEdit Properties {
			get { return ownerEditProperties; }
		}
		RepositoryItemFilterUIEditorPopupContainerEdit ownerPopupEditProperties;
		protected RepositoryItemFilterUIEditorPopupContainerEdit PopupEditProperties {
			get { return ownerPopupEditProperties; }
		}
		protected bool IsActive {
			get { return (ownerEditProperties != null || ownerPopupEditProperties != null) && (ValueViewModel != null); }
		}
		protected virtual void OnValueViewModelChanged() {
			if(ValueViewModel == null)
				return;
			if(IsActive) ApplyTemplate();
		}
		void ValueViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e) {
			Action updateEditor;
			if(npcActions.TryGetValue(e.PropertyName, out updateEditor))
				updateEditor();
		}
		protected virtual System.Type GetValueViewModelType() {
			return ValueViewModel.GetType().BaseType;
		}
		Control templateCore;
		public Control Template {
			get { return templateCore; }
		}
		protected bool IsPopupEdit {
			get { return PopupEditProperties != null; }
		}
		void ApplyTemplate() {
			if(templateCore != null) return;
			if(IsPopupEdit)
				templateCore = GetPopupTemplate();
			else
				templateCore = GetTemplate();
			if(templateCore != null) {
				templateCore.Dock = DockStyle.Fill;
				templateCore.HandleCreated += templateCore_HandleCreated;
				if(!IsPopupEdit) {
					templateCore.Parent = Properties.OwnerEdit;
					Properties.OwnerEdit.OnPropertiesChanged(true);
				}
			}
		}
		void templateCore_HandleCreated(object sender, EventArgs e) {
			OnApplyTemplate();
		}
		protected virtual void OnApplyTemplate() {
			var properties = TypeDescriptor.GetProperties(ValueViewModel.GetType());
			var fromValueProperty = properties["FromValue"];
			var toValueProperty = properties["ToValue"];
			if(fromValueProperty != null && toValueProperty != null)
				SetupRangeEditor(properties, fromValueProperty, toValueProperty);
			var valueProperty = properties["Value"];
			if(valueProperty != null)
				SetupValueEditor(valueProperty);
			var valuesProperty = properties["Values"];
			if(valuesProperty != null)
				SetupLookupEditor(valuesProperty);
		}
		void SetupRangeEditor(PropertyDescriptorCollection properties, PropertyDescriptor fromValueProperty, PropertyDescriptor toValueProperty) {
			var minimumProperty = properties["Minimum"];
			var maximumProperty = properties["Maximum"];
			var averageProperty = properties["Average"];
			Action updateTrackBar = null;
			Action updateTrackBarMinMax = null;
			var Part_TrackBar = GetTemplatedEditor<TrackBarControl>("Part_TrackBar");
			if(Part_TrackBar != null) {
				updateTrackBarMinMax = () =>
				{
					Part_TrackBar.Properties.BeginInit();
					int minValue = Convert.ToInt32(minimumProperty.GetValue(ValueViewModel));
					int maxValue = Convert.ToInt32(maximumProperty.GetValue(ValueViewModel));
					object avg = averageProperty.GetValue(ValueViewModel);
					Part_TrackBar.Properties.Minimum = minValue;
					Part_TrackBar.Properties.Maximum = maxValue;
					Part_TrackBar.Properties.TickFrequency =
						(Part_TrackBar.Properties.Maximum - Part_TrackBar.Properties.Minimum) / 10;
					Part_TrackBar.Properties.Labels[0].Label = Convert.ToString(minValue);
					Part_TrackBar.Properties.Labels[0].Value = minValue;
					Part_TrackBar.Properties.Labels[1].Label = Convert.ToString(maxValue);
					Part_TrackBar.Properties.Labels[1].Value = maxValue;
					Part_TrackBar.Properties.Labels[2].Label = Convert.ToString(avg);
					Part_TrackBar.Properties.Labels[2].Visible = (avg != null);
					Part_TrackBar.Properties.Labels[2].Value = Convert.ToInt32(avg);
					Part_TrackBar.Properties.EndInit();
				};
				updateTrackBarMinMax();
				updateTrackBar = () =>
				{
					Part_TrackBar.EditValue = new DevExpress.XtraEditors.Repository.TrackBarRange(
								Convert.ToInt32(fromValueProperty.GetValue(ValueViewModel)),
								Convert.ToInt32(toValueProperty.GetValue(ValueViewModel))
						);
				};
				updateTrackBar();
				Part_TrackBar.EditValueChanged += (s, e) =>
				{
					if(Part_TrackBar.EditValue is DevExpress.XtraEditors.Repository.TrackBarRange) {
						var range = (DevExpress.XtraEditors.Repository.TrackBarRange)Part_TrackBar.EditValue;
						if(range != null) {
							Type valueType = Nullable.GetUnderlyingType(fromValueProperty.PropertyType);
							fromValueProperty.SetValue(ValueViewModel, Convert.ChangeType(range.Minimum, valueType));
							toValueProperty.SetValue(ValueViewModel, Convert.ChangeType(range.Maximum, valueType));
						}
					}
				};
			}
			Action updateFromSpinMinMax = null;
			var spinPart_FromValue = GetTemplatedChild<SpinEdit>("Part_FromValue");
			if(spinPart_FromValue != null) {
				updateFromSpinMinMax = () =>
				{
					spinPart_FromValue.Properties.MaxValue = Convert.ToDecimal(maximumProperty.GetValue(ValueViewModel));
					spinPart_FromValue.Properties.MinValue = Convert.ToDecimal(minimumProperty.GetValue(ValueViewModel));
				};
				updateFromSpinMinMax();
			}
			Action updateToSpinMinMax = null;
			var spinPart_ToValue = GetTemplatedChild<SpinEdit>("Part_ToValue");
			if(spinPart_ToValue != null) {
				updateToSpinMinMax = () =>
				{
					spinPart_ToValue.Properties.MaxValue = Convert.ToDecimal(maximumProperty.GetValue(ValueViewModel));
					spinPart_ToValue.Properties.MinValue = Convert.ToDecimal(minimumProperty.GetValue(ValueViewModel));
				};
				updateToSpinMinMax();
			}
			SetupValueEditor(fromValueProperty, "Part_FromValue", "Part_FromLabel", false, updateTrackBar);
			SetupValueEditor(toValueProperty, "Part_ToValue", "Part_ToLabel", false, updateTrackBar);
			Action updateMinMax = () =>
			{
				if(updateTrackBarMinMax != null)
					updateTrackBarMinMax();
				if(updateFromSpinMinMax != null)
					updateFromSpinMinMax();
				if(updateToSpinMinMax != null)
					updateToSpinMinMax();
			};
			npcActions.Add("Minimum", updateMinMax);
			npcActions.Add("Maximum", updateMinMax);
			npcActions.Add("Average", updateMinMax);
		}
		void SetupLookupEditor(PropertyDescriptor valuesProperty) {
			Type elementType = valuesProperty.PropertyType.GetGenericArguments()[0];
			var cbPart_Values = GetTemplatedEditor<CheckedComboBoxEdit>("Part_Values");
			if(cbPart_Values != null)
				SetupDropDownEditor(cbPart_Values, valuesProperty, elementType);
			var tokenPart_Values = GetTemplatedChild<TokenEdit>("Part_Values");
			if(tokenPart_Values != null)
				SetupTokenEditor(tokenPart_Values, valuesProperty, elementType);
			var lbPart_Values = GetTemplatedChild<CheckedListBoxControl>("Part_Values");
			if(lbPart_Values != null)
				SetupListEditor(lbPart_Values, valuesProperty, elementType);
			SetupLoadButtons();
		}
		void SetupLoadButtons() {
			SetupLoadMoreButton();
			SetupLoadFewerButton();
		}
		void SetupLoadMoreButton() {
			var Part_MoreButton = GetTemplatedChild<SimpleButton>("Part_MoreButton");
			if(Part_MoreButton != null) {
				var collectionValueViewModel = ValueViewModel as ICollectionValueViewModel;
				if(collectionValueViewModel != null) {
					Part_MoreButton.BindCommand<ICollectionValueViewModel>(x => x.LoadMore(), collectionValueViewModel);
					Action updateButtonVisibility = () =>
					{
						FilterUIEditorContainerEdit templatedParent = Template.Parent as FilterUIEditorContainerEdit;
						if(templatedParent != null)
							templatedParent.SizeableChanged();
						Part_MoreButton.Visible = collectionValueViewModel.IsLoadMoreAvailable;
						if(templatedParent != null && templatedParent.Parent != null)
							templatedParent.Parent.PerformLayout();
					};
					npcActions.Add("IsLoadMoreAvailable", updateButtonVisibility);
					updateButtonVisibility();
				}
				else Part_MoreButton.Visible = false;
			}
		}
		void SetupLoadFewerButton() {
			var Part_FewerButton = GetTemplatedChild<SimpleButton>("Part_FewerButton");
			if(Part_FewerButton != null) {
				var collectionValueViewModel = ValueViewModel as ICollectionValueViewModel;
				if(collectionValueViewModel != null) {
					Part_FewerButton.BindCommand<ICollectionValueViewModel>(x => x.LoadFewer(), collectionValueViewModel);
					Action updateButtonVisibility = () =>
					{
						FilterUIEditorContainerEdit templatedParent = Template.Parent as FilterUIEditorContainerEdit;
						if(templatedParent != null)
							templatedParent.SizeableChanged();
						Part_FewerButton.Visible = collectionValueViewModel.IsLoadFewerAvailable;
						if(templatedParent != null && templatedParent.Parent != null)
							templatedParent.Parent.PerformLayout();
					};
					npcActions.Add("IsLoadFewerAvailable", updateButtonVisibility);
					updateButtonVisibility();
				}
				else Part_FewerButton.Visible = false;
			}
		}
		void SetupListEditor(CheckedListBoxControl lbPart_Values, PropertyDescriptor valuesProperty, Type elementType) {
			Action updateCheckedItems = () =>
			{
				lbPart_Values.BeginUpdate();
				var values = valuesProperty.GetValue(ValueViewModel) as IEnumerable;
				object[] valuesArr = (values != null) ?
					values.OfType<object>().ToArray() : new object[] { };
				CheckedListBoxItem lbAllItem = null;
				ShowAllElement showAllElement = null;
				foreach(CheckedListBoxItem item in lbPart_Values.Items) {
					if(item.Value is ShowAllElement) {
						showAllElement = item.Value as ShowAllElement;
						lbAllItem = item;
						continue;
					}
					item.CheckStateInternal = Array.IndexOf(valuesArr, item.Value) != -1 ?
						CheckState.Checked : CheckState.Unchecked;
				}
				if(showAllElement != null && lbAllItem != null)
					lbAllItem.CheckStateInternal = showAllElement.ItemsCheckStyle();
				lbPart_Values.CheckedIndices.Clear();
				lbPart_Values.EndUpdate();
			};
			npcActions.Add(valuesProperty.Name, updateCheckedItems);
			updateCheckedItems();
			bool useSelectAll = (ValueViewModel is IBaseCollectionValueViewModel) && ((IBaseCollectionValueViewModel)ValueViewModel).UseSelectAll;
			int lockItemCheck = 0;
			lbPart_Values.ItemCheck += (s, e) =>
			{
				if(lockItemCheck > 0) return;
				var showAllElement = useSelectAll ? (lbPart_Values.Items[0].Value as ShowAllElement) : null;
				if(showAllElement != null) {
					lockItemCheck++;
					if(e.Index == 0)
						showAllElement.UpdateCheckStateItems();
					else
						showAllElement.UpdateCheckStateElement();
					lbPart_Values.CheckedIndices.Clear();
					lockItemCheck--;
				}
				valuesProperty.SetValue(ValueViewModel, lbPart_Values.CheckedItems.ToArray(elementType));
			};
			var collectionValueViewModel = ValueViewModel as ICollectionValueViewModel;
			if(collectionValueViewModel != null) {
				Action updateLBDataSource = () =>
				{
					lbPart_Values.BeginUpdate();
					var checkedItems = lbPart_Values.CheckedItems.ToArray();
					lbPart_Values.Items.Clear();
					CheckedListBoxItem lbAllItem = null;
					ShowAllElement showAllElement = null;
					if(collectionValueViewModel.UseSelectAll) {
						lbAllItem = checkedItems.FirstOrDefault(i => i.Value is ShowAllElement) ??
							new CheckedListBoxItem(new ShowAllElement(lbPart_Values, collectionValueViewModel.SelectAllName));
						showAllElement = lbAllItem.Value as ShowAllElement;
						lbPart_Values.Items.Add(lbAllItem);
					}
					if(collectionValueViewModel.LookupDataSource != null) {
						foreach(var item in collectionValueViewModel.LookupDataSource) {
							var lbItem = checkedItems.FirstOrDefault(i => Equals(i.Value, item.Key)) ??
								new CheckedListBoxItem(item.Key, CheckState.Unchecked);
							lbItem.Description = item.Value;
							lbPart_Values.Items.Add(lbItem);
						}
					}
					if(showAllElement != null && lbAllItem != null)
						lbAllItem.CheckStateInternal = showAllElement.ItemsCheckStyle();
					lbPart_Values.MultiColumn = lbPart_Values.ItemCount > Templates.Lookup.ListAutoSizeHelper.MaxRowCountForSingleColumnMode;
					lbPart_Values.CheckedIndices.Clear();
					lbPart_Values.EndUpdate();
				};
				npcActions.Add("DataSource", updateLBDataSource);
				updateLBDataSource();
			}
			var enumValueViewModel = ValueViewModel as IEnumValueViewModel;
			if(enumValueViewModel != null) {
				lbPart_Values.BeginUpdate();
				if(useSelectAll)
					lbPart_Values.Items.Add(new CheckedListBoxItem(new ShowAllElement(lbPart_Values, enumValueViewModel.SelectAllName)));
				lbPart_Values.AddEnum(enumValueViewModel.EnumType, elementType != enumValueViewModel.EnumType);
				lbPart_Values.EndUpdate();
			}
		}
		void SetupTokenEditor(TokenEdit tokenPart_Values, PropertyDescriptor valuesProperty, Type elementType) {
			var token = (RepositoryItemTokenEdit)tokenPart_Values.Properties;
			token.SelectedItemsChanged += (s, e) =>
			{
				Array values = Array.CreateInstance(elementType, token.SelectedItems.Count);
				for(int i = 0; i < values.Length; i++)
					values.SetValue(token.SelectedItems[i].Value, i);
				valuesProperty.SetValue(ValueViewModel, values);
			};
			var collectionValueViewModel = ValueViewModel as ICollectionValueViewModel;
			if(collectionValueViewModel != null) {
				Action updateTokenDataSource = () =>
				{
					token.BeginUpdate();
					token.Tokens.Clear();
					if(collectionValueViewModel.LookupDataSource != null) {
						foreach(var item in collectionValueViewModel.LookupDataSource)
							token.Tokens.Add(new TokenEditToken(item.Value, item.Key));
					}
					token.EndUpdate();
				};
				npcActions.Add("DataSource", updateTokenDataSource);
				updateTokenDataSource();
			}
			var enumValueViewModel = ValueViewModel as IEnumValueViewModel;
			if(enumValueViewModel != null)
				token.Tokens.AddEnum(enumValueViewModel.EnumType, elementType != enumValueViewModel.EnumType);
		}
		void SetupDropDownEditor(CheckedComboBoxEdit cbPart_Values, PropertyDescriptor valuesProperty, Type elementType) {
			var cb = (RepositoryItemCheckedComboBoxEdit)cbPart_Values.Properties;
			int lockUpdate = 0;
			Action updateCheckedValues = () =>
			{
				if(lockUpdate == 0) cbPart_Values.EditValue = valuesProperty.GetValue(ValueViewModel);
			};
			npcActions.Add(valuesProperty.Name, updateCheckedValues);
			updateCheckedValues();
			cb.EditValueChanged += (s, e) =>
			{
				lockUpdate++;
				var editValueList = ((BaseEdit)s).EditValue as System.Collections.IList;
				Array values = Array.CreateInstance(elementType, editValueList.Count);
				for(int i = 0; i < values.Length; i++)
					values.SetValue(editValueList[i], i);
				valuesProperty.SetValue(ValueViewModel, values);
				lockUpdate--;
			};
			var collectionValueViewModel = ValueViewModel as ICollectionValueViewModel;
			if(collectionValueViewModel != null) {
				Action updateCBDataSource = () =>
				{
					cb.DataSource = collectionValueViewModel.DataSource;
				};
				npcActions.Add("DataSource", updateCBDataSource);
				updateCBDataSource();
				cb.ValueMember = collectionValueViewModel.ValueMember;
				cb.DisplayMember = collectionValueViewModel.DisplayMember;
				cb.SelectAllItemCaption = collectionValueViewModel.SelectAllName;
				cb.SelectAllItemVisible = collectionValueViewModel.UseSelectAll;
				if(collectionValueViewModel.UseSelectAll) {
					cb.CustomDisplayText += (s, e) =>
					{
						if(cb.OwnerCheckedComboBoxEdit != null && cb.OwnerCheckedComboBoxEdit.IsAllSelectedItemsChecked())
							e.DisplayText = cb.SelectAllItemCaption;
					};
				}
			}
			var enumValueViewModel = ValueViewModel as IEnumValueViewModel;
			if(enumValueViewModel != null) {
				cb.AddEnum(enumValueViewModel.EnumType, elementType != enumValueViewModel.EnumType);
				cb.SelectAllItemCaption = enumValueViewModel.SelectAllName;
				cb.SelectAllItemVisible = enumValueViewModel.UseSelectAll;
			}
		}
		void SetupValueEditor(PropertyDescriptor valueProperty, string editorPartName = "Part_Value", string labelPartName = "Part_ValueLabel", bool merge = true, Action update = null) {
			var Part_Editor = GetTemplatedEditor<BaseEdit>(editorPartName);
			if(Part_Editor != null) {
				Action updateValueEditor = () =>
				{
					if(update != null) update();
					Part_Editor.EditValue = valueProperty.GetValue(ValueViewModel);
				};
				npcActions.Add(valueProperty.Name, updateValueEditor);
				updateValueEditor();
				Type valueType = Nullable.GetUnderlyingType(valueProperty.PropertyType);
				Part_Editor.EditValueChanged += (s, e) =>
				{
					object editValue = ((BaseEdit)s).EditValue;
					object value = !ReferenceEquals(editValue, null) ?
						Convert.ChangeType(editValue, valueType) : null;
					valueProperty.SetValue(ValueViewModel, value);
				};
				var valueAttributes = new Data.Utils.AnnotationAttributes(valueProperty);
				if(DefaultEditorsRepository.CanUseMaskForType(valueAttributes)) {
					RepositoryItemTextEdit textEditItem = Part_Editor.Properties as RepositoryItemTextEdit;
					if(textEditItem != null) {
						Type defaultNumericType;
						if(DefaultEditorsRepository.IsNumeric(valueType, out defaultNumericType))
							DefaultEditorsRepository.InitializeDefaultEditMaskForNumericType(textEditItem, defaultNumericType);
						var dataType = valueAttributes.GetActualDataType();
						if(dataType.HasValue && dataType.Value == System.ComponentModel.DataAnnotations.DataType.Currency)
							DefaultEditorsRepository.InitializeDefaultEditMaskForCurrencyType(textEditItem);
					}
				}
				DefaultEditorsRepository.InitializeDisplayFormatFromDataFormatString(Part_Editor.Properties, valueType, valueAttributes);
				DefaultEditorsRepository.InitializeNullValueRelatedProperties(Part_Editor.Properties, valueAttributes);
				var booleanValueViewModel = ValueViewModel as IBooleanValueViewModel;
				if(booleanValueViewModel != null) {
					var check = Part_Editor.Properties as RepositoryItemCheckEdit;
					if(check != null && booleanValueViewModel.DefaultValue.HasValue) {
						check.AllowGrayed = false;
					}
					var cb = Part_Editor.Properties as RepositoryItemImageComboBox;
					if(cb != null) {
						cb.Items[0].Description = booleanValueViewModel.DefaultName;
						cb.Items[1].Description = booleanValueViewModel.TrueName;
						cb.Items[2].Description = booleanValueViewModel.FalseName;
						if(booleanValueViewModel.DefaultValue.HasValue) {
							if(booleanValueViewModel.DefaultValue.Value)
								cb.Items.RemoveAt(1);
							else
								cb.Items.RemoveAt(2);
							cb.Items[0].Value = booleanValueViewModel.DefaultValue.Value;
						}
					}
					var radio = Part_Editor.Properties as RepositoryItemRadioGroup;
					if(radio != null) {
						radio.Items[0].Description = booleanValueViewModel.DefaultName;
						radio.Items[1].Description = booleanValueViewModel.TrueName;
						radio.Items[2].Description = booleanValueViewModel.FalseName;
						if(booleanValueViewModel.DefaultValue.HasValue) {
							if(booleanValueViewModel.DefaultValue.Value)
								radio.Items.RemoveAt(1);
							else
								radio.Items.RemoveAt(2);
							radio.Items[0].Value = booleanValueViewModel.DefaultValue.Value;
						}
					}
				}
			}
			var Part_Label = GetTemplatedChild<Control>(labelPartName);
			if(Part_Label != null) {
				var valueAttributes = new Data.Utils.AnnotationAttributes(valueProperty);
				if(merge)
					valueAttributes = valueAttributes.Merge(IsPopupEdit ? PopupEditProperties.AnnotationAttributes : Properties.AnnotationAttributes);
				Part_Label.Text = Data.Utils.AnnotationAttributes.GetColumnCaption(valueAttributes);
				TextEdit tEdit = Part_Editor as TextEdit;
				if(tEdit != null) {
					Part_Label.MouseClick += (s, e) =>
					{
						if(e.Clicks == 1)
							tEdit.Focus();
						if(e.Clicks == 2)
							tEdit.SelectAll();
					};
				}
				CheckEdit check = Part_Editor as CheckEdit;
				if(check != null) {
					Part_Label.MouseClick += (s, e) =>
						check.Toggle();
					LabelControl label = Part_Label as LabelControl;
					if(label != null)
						label.ToolTip = Data.Utils.AnnotationAttributes.GetColumnDescription(valueAttributes);
				}
			}
		}
		IDictionary<string, Action> npcActions = new Dictionary<string, Action>();
		#region Templates Registration
		readonly static IDictionary<object, Func<Control>> templatesMap = new Dictionary<object, Func<Control>>(){
			{ RangeUIEditorType.Default, () => new Templates.Range.DefaultTemplate() },
			{ RangeUIEditorType.Range, () => new Templates.Range.RangeTemplate() },
			{ RangeUIEditorType.Text, () => new Templates.Range.TextTemplate() },
			{ RangeUIEditorType.Spin, () => new Templates.Range.SpinTemplate() },
			{ DateTimeRangeUIEditorType.Default, () => new Templates.DateTimeRange.DefaultTemplate() },
			{ DateTimeRangeUIEditorType.Picker, () => new Templates.DateTimeRange.PickerTemplate() },
			{ DateTimeRangeUIEditorType.Range, () => new Templates.DateTimeRange.RangeTemplate() },
			{ DateTimeRangeUIEditorType.Calendar, () => new Templates.DateTimeRange.CalendarTemplate() },
			{ BooleanUIEditorType.Default, () => new Templates.Choice.DefaultTemplate() },
			{ BooleanUIEditorType.Check, () => new Templates.Choice.CheckTemplate() },
			{ BooleanUIEditorType.Toggle, () => new Templates.Choice.ToggleTemplate() },
			{ BooleanUIEditorType.List, () => new Templates.Choice.ListTemplate() },
			{ BooleanUIEditorType.DropDown, () => new Templates.Choice.DropDownTemplate() },
			{ LookupUIEditorType.Default, () => new Templates.Lookup.DefaultTemplate() },
			{ LookupUIEditorType.List, () => new Templates.Lookup.ListTemplate() },
			{ LookupUIEditorType.DropDown, () => new Templates.Lookup.DropDownTemplate() },
			{ LookupUIEditorType.TokenBox, () => new Templates.Lookup.TokenTemplate() },
		};
		public static void RegisterTemplate<TTemplate>(RangeUIEditorType editorType)
			where TTemplate : XtraUserControl, new() {
			RegisterTemplate<TTemplate>(editorType, () => new TTemplate());
		}
		public static void RegisterTemplate<TTemplate>(RangeUIEditorType editorType, Func<TTemplate> factoryMethod)
			where TTemplate : XtraUserControl {
			RegisterTemplateCore(editorType, factoryMethod);
		}
		public static void RegisterTemplate<TTemplate>(DateTimeRangeUIEditorType editorType)
			where TTemplate : XtraUserControl, new() {
			RegisterTemplate<TTemplate>(editorType, () => new TTemplate());
		}
		public static void RegisterTemplate<TTemplate>(DateTimeRangeUIEditorType editorType, Func<TTemplate> factoryMethod)
			where TTemplate : XtraUserControl {
			RegisterTemplateCore(editorType, factoryMethod);
		}
		public static void RegisterTemplate<TTemplate>(BooleanUIEditorType editorType)
			where TTemplate : XtraUserControl, new() {
			RegisterTemplate<TTemplate>(editorType, () => new TTemplate());
		}
		public static void RegisterTemplate<TTemplate>(BooleanUIEditorType editorType, Func<TTemplate> factoryMethod)
			where TTemplate : XtraUserControl {
			RegisterTemplateCore(editorType, factoryMethod);
		}
		public static void RegisterTemplate<TTemplate>(LookupUIEditorType editorType)
			where TTemplate : XtraUserControl, new() {
			RegisterTemplate<TTemplate>(editorType, () => new TTemplate());
		}
		public static void RegisterTemplate<TTemplate>(LookupUIEditorType editorType, Func<TTemplate> factoryMethod)
			where TTemplate : XtraUserControl {
			RegisterTemplateCore(editorType, factoryMethod);
		}
		static void RegisterTemplateCore<TTemplate>(object editorType, Func<TTemplate> factoryMethod)
			where TTemplate : XtraUserControl {
			templatesMap[editorType] = factoryMethod;
		}
		#endregion Templates Registration
		protected Control GetTemplate() {
			object key = null;
			if(Properties.RangeUIEditorType.HasValue)
				key = Properties.RangeUIEditorType.Value;
			if(Properties.DateTimeRangeUIEditorType.HasValue)
				key = Properties.DateTimeRangeUIEditorType.Value;
			if(Properties.BooleanUIEditorType.HasValue)
				key = Properties.BooleanUIEditorType.Value;
			if(Properties.LookupUIEditorType.HasValue)
				key = Properties.LookupUIEditorType.Value;
			Func<Control> template;
			if(key != null && templatesMap.TryGetValue(key, out template))
				return template();
			return new XtraUserControl();
		}
		protected Control GetPopupTemplate() {
			object key = null;
			if(PopupEditProperties.RangeUIEditorType.HasValue)
				key = Properties.RangeUIEditorType.Value;
			if(PopupEditProperties.DateTimeRangeUIEditorType.HasValue)
				key = Properties.DateTimeRangeUIEditorType.Value;
			if(PopupEditProperties.BooleanUIEditorType.HasValue)
				key = Properties.BooleanUIEditorType.Value;
			if(PopupEditProperties.LookupUIEditorType.HasValue)
				key = Properties.LookupUIEditorType.Value;
			Func<Control> template;
			if(key != null && templatesMap.TryGetValue(key, out template))
				return template();
			return new XtraUserControl();
		}
		protected TEditor GetTemplatedEditor<TEditor>(string name) where TEditor : BaseEdit {
			return GetTemplatedChild<TEditor>(name);
		}
		protected TChild GetTemplatedChild<TChild>(string name) where TChild : Control {
			return Template.Controls.Find(name, true).FirstOrDefault(c => c is TChild) as TChild;
		}
	}
}
