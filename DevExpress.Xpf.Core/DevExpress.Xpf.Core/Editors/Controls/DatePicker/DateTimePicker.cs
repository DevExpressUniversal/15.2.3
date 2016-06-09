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

using DevExpress.Data.Mask;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Validation.Native;
using DevExpress.Xpf.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using DevExpress.Core;
using DevExpress.Xpf.Editors.Helpers;
using System.ComponentModel;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Editors {
	public enum DateTimePart {
		Day,
		Month,
		Year,
		Hour12,
		Hour24,
		Minute,
		Second,
		Millisecond,
		Period,
		PeriodOfEra,
		AmPm,
		None
	}
	public class DateTimePicker : DateEditCalendarBase {
		public DateEdit OwnerDateEdit { get { return BaseEdit.GetOwnerEdit(this) as DateEdit; } }
		DateTimeMaskManager MaskManager { get; set; }
		DateTimeMaskManager IndexMaskManager { get; set; }
		ItemsControl ItemsControl { get; set; }
		internal IList<DateTimePickerPart> Pickers { get; private set; }
		internal DateTimePickerPart SelectedPicker { get; private set; }
		Locker UpdateValueOnAnimationCompletedLocker { get; set; }
		Locker MaskManagerEditTextChangedLocker { get; set; }
		Locker OnPreviewTextInputLocker { get; set; }
		internal bool IsItemsControlInitialized { get; set; }
		PostponedAction ItemsControlInitializeAction { get; set; }
		IEnumerable<DateTimeMaskFormatElement> Formats { get { return DateTimeMaskManagerHelper.GetFormatInfo(MaskManager); } }
		public event EventHandler<DateTimeChangedEventArgs> DateTimeChanged;
		public DateTimePicker() {
			DefaultStyleKey = typeof(DateTimePicker);
			UpdateValueOnAnimationCompletedLocker = new Locker();
			UpdateValueOnAnimationCompletedLocker.Unlocked += UpdateValueOnAnimationCompleted;
			MaskManagerEditTextChangedLocker = new Locker();
			OnPreviewTextInputLocker = new Locker();
			IsItemsControlInitialized = false;
			ItemsControlInitializeAction = new PostponedAction(() => ItemsControl == null);
		}
		protected override void OnDateTimeChanged() {
			if (!IsInitialized)
				return;
			if (OwnerDateEdit != null && OwnerDateEdit.PopupFooterButtons != PopupFooterButtons.OkCancel)
				OwnerDateEdit.SetDateTime(DateTime, UpdateEditorSource.ValueChanging);
			RaiseDateTimeChanged(DateTime);
			UpdateDaysCount();
			if (SelectedPicker != null)
				SetCursorPosition(Pickers.IndexOf(SelectedPicker));
			UpdateSelectedItems();
			OnPreviewTextInputLocker.Unlock();
		}
		void UpdateSelectedItems() {
			if (!IsItemsControlInitialized)
				return;
			int formatterIndex = 0;
			foreach (var format in Formats.Where(x => x.Editable)) {
				var editableFormat = format as DateTimeMaskFormatElementEditable;
				if (editableFormat == null)
					continue;
				var editor = editableFormat.CreateElementEditor((DateTime)MaskManager.GetCurrentEditValue());
				if (editor == null)
					continue;
				var selectedItem = CreateDateTimePickerData(editor, editableFormat);
				Pickers[formatterIndex].SelectedItem = selectedItem;
				formatterIndex++;
			}
		}
		protected override void OnMaskChanged(string newValue) {
			Initialize();
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			if (OwnerDateEdit != null)
				OwnerDateEdit.OnApplyCalendarTemplate(this);
			ItemsControl = GetTemplateChild("ItemsControl") as ItemsControl;
			ItemsControlInitializeAction.Perform();
		}
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			Initialize();
		}
		protected override void OnPreviewTextInput(TextCompositionEventArgs e) {
			if (SelectedPicker == null || (OwnerDateEdit != null && OwnerDateEdit.IsReadOnly))
				return;
			Pickers.ForEach(x => x.UseTransitions = false);
			if (!OnPreviewTextInputLocker.IsLocked)
				OnPreviewTextInputLocker.Lock();
			int selectedPickerIndex = GetCursorPosition();
			MaskManager.Insert(e.Text);
			int newSelectedPickerIndex = GetCursorPosition();
			DateTime currentDateTime = (DateTime)MaskManager.GetCurrentEditValue();
			UpdatePickerOnInput(selectedPickerIndex);
			SelectPicker(newSelectedPickerIndex);
			if (currentDateTime != DateTime)
				SetDateTime(GetCurrentValueFromMaskManager());
			base.OnPreviewTextInput(e);
		}
		void UpdatePickerOnInput(int pickerIndex) {
			int selectionStart;
			int selectionLength;
			GetSelectionRangeByIndex(pickerIndex, out selectionStart, out selectionLength);
			string pickerValue = MaskManager.DisplayText.Substring(selectionStart, selectionLength);
			var picker = Pickers[pickerIndex];
			int intValue;
			DateTimePickerData newItem = null;
			if (int.TryParse(pickerValue, out intValue)) {
				newItem = picker.Items.SingleOrDefault(x => {
					int textIntValue;
					return int.TryParse(x.Text, out textIntValue) && textIntValue == intValue;
				});
			}
			else {
				newItem = picker.Items.SingleOrDefault(x => x.Text.Equals(pickerValue));
			}
			if (newItem != null)
				picker.SelectedItem = newItem;
		}
		void GetSelectionRangeByIndex(int index, out int selectionStart, out int selectionLength) {
			if (GetCursorPosition() == index) {
				selectionStart = MaskManager.DisplaySelectionStart;
				selectionLength = MaskManager.DisplaySelectionLength;
				return;
			}
			int currentIndex = index;
			IndexMaskManager.SetInitialEditValue(MaskManager.GetCurrentEditValue());
			var format = Formats.Where(x => x.Editable).ElementAt(index);
			IndexMaskManager.CursorHome(false);
			while (currentIndex-- > 0)
				IndexMaskManager.CursorRight(false);
			selectionStart = IndexMaskManager.DisplaySelectionStart;
			selectionLength = IndexMaskManager.DisplaySelectionLength;
		}
		protected virtual DateTimeMaskManager CreateMaskManager(string mask) {
			return new DateTimeMaskManager(mask, true, CultureInfo.CurrentCulture, false);
		}
		void UpdateMaskManager() {
			if (MaskManager != null)
				MaskManager.EditTextChanged -= OnMaskManagerEditTextChanged;
			MaskManager = CreateMaskManager(Mask);
			IndexMaskManager = CreateMaskManager(Mask);
			MaskManager.SetInitialEditValue(DateTime);
			IndexMaskManager.SetInitialEditValue(DateTime);
			MaskManager.EditTextChanged += OnMaskManagerEditTextChanged;
		}
		protected virtual void OnMaskManagerEditTextChanged(object sender, EventArgs e) {
			if (!IsItemsControlInitialized || !OnPreviewTextInputLocker.IsLocked || MaskManagerEditTextChangedLocker.IsLocked)
				return;
			MaskManagerEditTextChangedLocker.Lock();
		}
		protected internal override bool ProcessKeyDown(KeyEventArgs e) {
			OnPreviewKeyDown(e);
			return true;
		}
		protected override void OnPreviewKeyDown(KeyEventArgs e) {
			base.OnPreviewKeyDown(e);
			int index = Pickers.IndexOf(SelectedPicker);
			int itemsCount = SelectedPicker.Return(x => x.VisibleItemsCount / 2, () => 0);
			DateTimePickerSelector currentSelector = null;
			if (index >= 0) {
				int items = -1;
				currentSelector = (DateTimePickerSelector)LayoutHelper.FindElement(ItemsControl, x => {
					if (x is DateTimePickerSelector)
						++items;
					return items == index && x is DateTimePickerSelector;
				});
			}
			switch (e.Key) {
				case Key.Down:
					if (currentSelector == null)
						break;
					currentSelector.Spin();
					e.Handled = true;
					break;
				case Key.Up:
					if (currentSelector == null)
						break;
					currentSelector.Spin(-1);
					e.Handled = true;
					break;
				case Key.Left:
					if (index == -1) {
						SelectedPicker = Pickers.FirstOrDefault();
						SelectedPicker.IsExpanded = true;
						e.Handled = true;
						break;
					}
					index = index == 0 ? Pickers.Count - 1 : index - 1;
					SelectPicker(index);
					e.Handled = true;
					break;
				case Key.Right:
					if (index == -1) {
						SelectedPicker = Pickers.FirstOrDefault();
						SelectedPicker.IsExpanded = true;
						e.Handled = true;
						break;
					}
					index = index == Pickers.Count - 1 ? 0 : index + 1;
					SelectPicker(index);
					e.Handled = true;
					break;
				case Key.PageUp:
					if (currentSelector == null)
						break;
					currentSelector.Spin(-itemsCount);
					e.Handled = true;
					break;
				case Key.PageDown:
					if (currentSelector == null)
						break;
					currentSelector.Spin(itemsCount);
					e.Handled = true;
					break;
				case Key.Home:
					if (currentSelector == null)
						break;
					currentSelector.SpinToIndex(0);
					e.Handled = true;
					break;
				case Key.End:
					if (currentSelector == null)
						break;
					currentSelector.SpinToIndex(currentSelector.GetItemsCount() - 1);
					e.Handled = true;
					break;
			}
		}
		void Initialize() {
			UpdateMaskManager();
			ResetPickers();
			IsItemsControlInitialized = false;
			ItemsControlInitializeAction.PerformPostpone(InitializeItemsControl);
		}
		void InitializeItemsControl() {
			if (ItemsControl == null)
				return;
			int formatterIndex = 0;
			bool shouldFixDateTime = false;
			foreach (var format in Formats.Where(x => x.Editable)) {
				MaskManager.SetInitialEditValue(DateTime);
				var editableFormat = format as DateTimeMaskFormatElementEditable;
				if (editableFormat == null)
					continue;
				var editor = editableFormat.CreateElementEditor((DateTime)MaskManager.GetCurrentEditValue());
				if (editor == null)
					continue;
				UpdatePickerPartFromElementEditor(Pickers[formatterIndex], editor, editableFormat, formatterIndex);
				shouldFixDateTime = shouldFixDateTime || editableFormat.Mask.Equals("yy");
				formatterIndex++;
			}
			MaskManager.SetInitialEditValue(DateTime);
			ItemsControl.ItemsSource = Pickers;
			IsItemsControlInitialized = true;
			if (shouldFixDateTime)
				FixDateTimeByMask();
		}
		void FixDateTimeByMask() {
			DateTimeMaskManager maskManager = CreateMaskManager("yy");
			maskManager.SetInitialEditValue(DateTime);
			maskManager.Insert("00");
			var minYear = ((DateTime)maskManager.GetCurrentEditValue()).Year;
			maskManager.Insert("99");
			var maxYear = ((DateTime)maskManager.GetCurrentEditValue()).Year;
			if (DateTime.Year < minYear)
				DateTime = new DateTime(minYear, DateTime.Month, DateTime.Day, DateTime.Hour, DateTime.Minute, DateTime.Second, DateTime.Millisecond, DateTime.Kind);
			else if (DateTime.Year > maxYear)
				DateTime = new DateTime(maxYear, DateTime.Month, DateTime.Day, DateTime.Hour, DateTime.Minute, DateTime.Second, DateTime.Millisecond, DateTime.Kind);
		}
		void UpdateDaysCount() {
			if (!IsItemsControlInitialized)
				return;
			int formatterIndex = 0;
			foreach (var format in Formats.Where(x => x.Editable)) {
				MaskManager.SetInitialEditValue(DateTime);
				var editableFormat = format as DateTimeMaskFormatElementEditable;
				if (editableFormat == null)
					continue;
				var editor = editableFormat.CreateElementEditor((DateTime)MaskManager.GetCurrentEditValue());
				if (editor == null)
					continue;
				if (GetDateTimePartFromEditor(editableFormat) == DateTimePart.Day) {
					UpdatePickerPartFromElementEditor(Pickers[formatterIndex], editor, editableFormat, formatterIndex);
				}
				formatterIndex++;
			}
			MaskManager.SetInitialEditValue(DateTime);
		}
		void ResetPickers() {
			if (Pickers != null) {
				foreach (var picker in Pickers) {
					picker.IsEnabled = OwnerDateEdit == null || !OwnerDateEdit.IsReadOnly;
					picker.AnimatedChanged -= PickerAnimatedChanged;
					picker.ExpandedChanged -= PickerExpandedChanged;
				}
				Pickers = null;
			}
			GeneratePickers();
		}
		void GeneratePickers() {
			Pickers = new ObservableCollection<DateTimePickerPart>();
			foreach (var format in Formats.Where(format => format.Editable)) {
				DateTimePickerPart part = new DateTimePickerPart();
				part.IsEnabled = OwnerDateEdit == null || !OwnerDateEdit.IsReadOnly;
				part.AnimatedChanged += PickerAnimatedChanged;
				part.ExpandedChanged += PickerExpandedChanged;
				Pickers.Add(part);
			}
		}
		void UpdatePickerPartFromElementEditor(DateTimePickerPart part, DateTimeElementEditor editor, DateTimeMaskFormatElementEditable format, int formatterIndex) {
			ObservableCollection<DateTimePickerData> list = new ObservableCollection<DateTimePickerData>();
			UpdateEditor(part, editor, format, formatterIndex, list);
			part.Items = list.OrderBy(data => data.Value).ToList();
			part.SelectedItem = list.FirstOrDefault(data => data.Value == DateTime);
			part.IsExpanded = object.ReferenceEquals(part, SelectedPicker);
		}
		void UpdateEditor(DateTimePickerPart part, DateTimeElementEditor editor, DateTimeMaskFormatElementEditable format, int formatterIndex, ObservableCollection<DateTimePickerData> list) {
			if (editor is DateTimeNumericRangeElementEditor)
				UpdateNumericRangeEditor(part, editor, format, formatterIndex, list);
			else if (editor is DateTimeElementEditorAmPm)
				UpdateAmPmEditor(part, list);
			else
				UpdateOtherEditors(part, editor, format, formatterIndex, list);
		}
		void UpdateOtherEditors(DateTimePickerPart part, DateTimeElementEditor editor, DateTimeMaskFormatElementEditable format, int formatterIndex, ObservableCollection<DateTimePickerData> list) {
			while (list.Count(item => item.Text == editor.DisplayText) == 0) {
				SetCursorPosition(formatterIndex);
				list.Add(CreateDateTimePickerData(editor, format));
				editor.SpinUp();
			}
			part.IsLooped = true;
		}
		void UpdateAmPmEditor(DateTimePickerPart part, ObservableCollection<DateTimePickerData> list) {
			if (DateTime.Hour >= 12) {
				list.Add(new DateTimePickerData { DateTimePart = DateTimePart.AmPm, Text = "AM", Value = DateTime.AddHours(-12) });
				list.Add(new DateTimePickerData { DateTimePart = DateTimePart.AmPm, Text = "PM", Value = DateTime });
			}
			else {
				list.Add(new DateTimePickerData { DateTimePart = DateTimePart.AmPm, Text = "AM", Value = DateTime });
				list.Add(new DateTimePickerData { DateTimePart = DateTimePart.AmPm, Text = "PM", Value = DateTime.AddHours(12) });
			}
			part.IsLooped = false;
		}
		void UpdateNumericRangeEditor(DateTimePickerPart part, DateTimeElementEditor editor, DateTimeMaskFormatElementEditable format, int formatterIndex, ObservableCollection<DateTimePickerData> list) {
			IList<int> available = DateTimeHelper.GetAvailableValuesByMask(format.Mask, DateTime, DateTime.MinValue, DateTime.MaxValue);
			string mask = format.Mask.Length > 3 ? "D4" : "D2";
			foreach (int value in available) {
				string insertValue = value.ToString(mask);
				MaskManager.SetInitialEditValue(DateTime);
				SetCursorPosition(formatterIndex);
				MaskManager.Insert(insertValue);
				editor.Insert(insertValue);
				list.Add(CreateDateTimePickerData(editor, format));
			}
			editor.Insert(DateTimeHelper.GetValueByMask(format.Mask, DateTime).ToString(mask));
			part.IsLooped = true;
		}
		DateTimePickerData CreateDateTimePickerData(DateTimeElementEditor editor, DateTimeMaskFormatElementEditable format) {
			return new DateTimePickerData {
				Text = editor.DisplayText,
				Value = GetCurrentValueFromMaskManager(),
				DateTimePart = GetDateTimePartFromEditor(format),
			};
		}
		DateTimePart GetDateTimePartFromEditor(DateTimeMaskFormatElementEditable format) {
			if (format is DateTimeMaskFormatElement_Year)
				return DateTimePart.Year;
			if (format is DateTimeMaskFormatElement_Month)
				return DateTimePart.Month;
			if (format is DateTimeMaskFormatElement_d)
				return DateTimePart.Day;
			if (format is DateTimeMaskFormatElement_H24)
				return DateTimePart.Hour24;
			if (format is DateTimeMaskFormatElement_h12)
				return DateTimePart.Hour12;
			if (format is DateTimeMaskFormatElement_Min)
				return DateTimePart.Minute;
			if (format is DateTimeMaskFormatElement_s)
				return DateTimePart.Second;
			if (format is DateTimeMaskFormatElement_Millisecond)
				return DateTimePart.Millisecond;
			if (format is DateTimeMaskFormatElement_AmPm)
				return DateTimePart.AmPm;
			return DateTimePart.None;
		}
		void PickerExpandedChanged(object sender, ExpandedChangedEventArgs e) {
			var pickerPart = (DateTimePickerPart)sender;
			Pickers.Where(x => x != pickerPart).ForEach(x => x.UseTransitions = true);
			if (e.IsExpanded) {
				foreach (var picker in Pickers) {
					if (picker.IsExpanded && !object.Equals(picker, pickerPart))
						picker.IsExpanded = false;
				}
				SelectedPicker = pickerPart;
				SetDateTime(GetCurrentValueFromMaskManager());
				SetCursorPosition(Pickers.IndexOf(SelectedPicker));
			}
		}
		void PickerAnimatedChanged(object sender, AnimatedChangedEventArgs e) {
			var pickerPart = (DateTimePickerPart)sender;
			var data = pickerPart.SelectedItem;
			int cursorPos = Pickers.IndexOf(pickerPart);
			if (e.IsAnimated)
				Pickers.ForEach(x => x.UseTransitions = false);
			if (!e.IsAnimated) {
				SetCursorPosition(cursorPos);
				if (OwnerDateEdit == null || !OwnerDateEdit.IsReadOnly) {
					MaskManager.Insert(data.Text);
					MaskManager.FlushPendingEditActions();
				}
				foreach (var picker in Pickers) {
					if (!picker.IsAnimated && !object.Equals(SelectedPicker, picker))
						picker.IsExpanded = false;
				}
				UpdateValueOnAnimationCompletedLocker.Unlock();
			}
			else {
				SelectedPicker = pickerPart;
				foreach (var picker in Pickers) {
					if (!picker.IsAnimated && !object.Equals(SelectedPicker, picker))
						picker.IsExpanded = false;
				}
				pickerPart.IsExpanded = true;
				UpdateValueOnAnimationCompletedLocker.Lock();
			}
		}
		void SetDateTime(DateTime dateTime) {
			if (OwnerDateEdit == null || OwnerDateEdit != null && !OwnerDateEdit.AllowRoundOutOfRangeValue) {
				if (MinValue.HasValue && dateTime < MinValue)
					dateTime = MinValue.Value;
				if (MaxValue.HasValue && dateTime > MaxValue)
					dateTime = MaxValue.Value;
			}
			DateTime = dateTime;
			MaskManager.SetInitialEditValue(DateTime);
			IndexMaskManager.SetInitialEditValue(DateTime);
		}
		void UpdateValueOnAnimationCompleted(object sender, EventArgs e) {
			SetDateTime((DateTime)MaskManager.GetCurrentEditValue());
			UpdateSelectedItems();
		}
		DateTime GetCurrentValueFromMaskManager() {
			MaskManager.FlushPendingEditActions();
			return (DateTime)MaskManager.GetCurrentEditValue();
		}
		void SetCursorPosition(int rightClickCount) {
			MaskManager.CursorHome(false);
			for (int clickCount = 0; clickCount < rightClickCount; clickCount++)
				MaskManager.CursorRight(false);
		}
		int GetCursorPosition() {
			if (MaskManagerEditTextChangedLocker.IsLocked) {
				IndexMaskManager.SetInitialEditValue(MaskManager.GetCurrentEditValue());
				MaskManagerEditTextChangedLocker.Unlock();
			}
			int selectionStartPosition = MaskManager.DisplaySelectionStart;
			int result = 0;
			IndexMaskManager.CursorHome(false);
			foreach (var format in Formats.Where(x => x.Editable)) {
				if (IndexMaskManager.DisplaySelectionStart == selectionStartPosition)
					return result;
				IndexMaskManager.CursorRight(false);
				result++;
			}
			return -1;
		}
		void SelectPicker(int index) {
			if (SelectedPicker != null && index == Pickers.IndexOf(SelectedPicker))
				return;
			SelectedPicker.Do(x => x.IsExpanded = false);
			SelectedPicker = Pickers[index];
			SelectedPicker.IsExpanded = true;
		}
		void RaiseDateTimeChanged(DateTime value) {
			if (DateTimeChanged != null)
				DateTimeChanged(this, new DateTimeChangedEventArgs(value));
		}
		public static class DateTimeHelper {
			static IDictionary<string, IList<int>> Cache { get; set; }
			static DateTimeHelper() {
				Cache = new Dictionary<string, IList<int>>();
			}
			public static IList<int> GetAvailableDaysInPeriod(DateTime currentDateTime) {
				List<int> result = new List<int>();
				int endValue = GetDaysInMonth(currentDateTime);
				for (int i = 1; i <= endValue; ++i)
					result.Add(i);
				return result;
			}
			public static IList<int> GetAvailableMonths() {
				List<int> result = new List<int>();
				for (int i = 1; i <= 12; ++i)
					result.Add(i);
				return result;
			}
			public static IList<int> GetAvailableYears() {
				List<int> result = new List<int>();
				for (int i = 1; i <= 9999; ++i)
					result.Add(i);
				return result;
			}
			public static IList<int> GetAvailableShortYears() {
				IList<int> items = new List<int>();
				for (int i = 0; i < 100; ++i)
					items.Add(i);
				return items;
			}
			public static IList<int> GetAvailableHours24() {
				List<int> result = new List<int>();
				for (int i = 0; i <= 23; ++i)
					result.Add(i);
				return result;
			}
			public static IList<int> GetAvailableHours12() {
				List<int> result = new List<int>();
				for (int i = 1; i <= 12; ++i)
					result.Add(i);
				return result;
			}
			public static IList<int> GetAvailableMinutes() {
				List<int> result = new List<int>();
				for (int i = 0; i <= 59; ++i)
					result.Add(i);
				return result;
			}
			public static IList<int> GetAvailableSeconds() {
				List<int> result = new List<int>();
				for (int i = 0; i <= 59; ++i)
					result.Add(i);
				return result;
			}
			public static IList<int> GetAvailableMilliseconds() {
				List<int> result = new List<int>();
				for (int i = 0; i <= 999; ++i)
					result.Add(i);
				return result;
			}
			public static IList<int> GetAvailableValuesByMask(string mask, DateTime value, DateTime startPeriod, DateTime endPeriod) {
				string cacheKey = string.Format("{0}_{1}_{2}", mask, GetValueByMask(mask, startPeriod), GetValueByMask(mask, endPeriod));
				switch (GetDateTimePartByMask(mask)) {
					case DateTimePart.Day:
						return GetAvailableDaysInPeriod(value);
					case DateTimePart.Hour12:
						if (!Cache.ContainsKey(cacheKey))
							Cache.Add(cacheKey, GetAvailableHours12());
						break;
					case DateTimePart.Hour24:
						if (!Cache.ContainsKey(cacheKey))
							Cache.Add(cacheKey, GetAvailableHours24());
						break;
					case DateTimePart.Millisecond:
						if (!Cache.ContainsKey(cacheKey))
							Cache.Add(cacheKey, GetAvailableMilliseconds());
						break;
					case DateTimePart.Minute:
						if (!Cache.ContainsKey(cacheKey))
							Cache.Add(cacheKey, GetAvailableMinutes());
						break;
					case DateTimePart.Month:
						if (!Cache.ContainsKey(cacheKey))
							Cache.Add(cacheKey, GetAvailableMonths());
						break;
					case DateTimePart.Period:
						throw new NotImplementedException();
					case DateTimePart.PeriodOfEra:
						throw new NotImplementedException();
					case DateTimePart.Second:
						if (!Cache.ContainsKey(cacheKey))
							Cache.Add(cacheKey, GetAvailableSeconds());
						break;
					case DateTimePart.Year:
						if (!Cache.ContainsKey(cacheKey))
							Cache.Add(cacheKey, mask.Length > 3 ? GetAvailableYears() : GetAvailableShortYears());
						break;
				}
				return Cache[cacheKey];
			}
			public static int GetValueByMask(string mask, DateTime dateTime) {
				switch (GetDateTimePartByMask(mask)) {
					case DateTimePart.Day:
						return dateTime.Day;
					case DateTimePart.Hour12:
						return dateTime.Hour > 12 ? dateTime.Hour - 12 : dateTime.Hour;
					case DateTimePart.Hour24:
						return dateTime.Hour;
					case DateTimePart.Millisecond:
						return dateTime.Millisecond;
					case DateTimePart.Minute:
						return dateTime.Minute;
					case DateTimePart.Month:
						return dateTime.Month;
					case DateTimePart.Period:
						return dateTime.Hour >= 12 ? 1 : 0;
					case DateTimePart.PeriodOfEra:
						throw new NotImplementedException();
					case DateTimePart.Second:
						return dateTime.Second;
					case DateTimePart.Year:
						return dateTime.Year;
				}
				return -1;
			}
			static int GetDaysInMonth(DateTime dateTime) {
				return DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
			}
			static DateTimePart GetDateTimePartByMask(string mask) {
				int maskLength = mask.Length;
				switch (mask[0]) {
					case 'd':
						if (maskLength <= 2)
							return DateTimePart.Day;
						return DateTimePart.None;
					case 'f':
						return DateTimePart.Millisecond;
					case 'g':
						return DateTimePart.PeriodOfEra;
					case 'h':
						return DateTimePart.Hour12;
					case 'H':
						return DateTimePart.Hour24;
					case 'm':
						return DateTimePart.Minute;
					case 'M':
						return DateTimePart.Month;
					case 's':
						return DateTimePart.Second;
					case 't':
						return DateTimePart.Period;
					case 'y':
						return DateTimePart.Year;
					default:
						return DateTimePart.None;
				}
			}
		}
	}
}
