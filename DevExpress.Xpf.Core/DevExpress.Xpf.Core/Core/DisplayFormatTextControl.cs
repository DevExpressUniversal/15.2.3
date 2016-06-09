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
using System.Windows.Data;
using System.Windows.Documents;
using System.Collections;
using System.ComponentModel;
using DevExpress.Xpf.Editors;
using System.Collections.ObjectModel;
using System.Globalization;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
#if SILVERLIGHT
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Core {
	public class DisplayFormatTextControl : Control, INotifyPropertyChanged {
		public static readonly DependencyProperty PrefixProperty = DependencyPropertyManager.RegisterAttached("Prefix",
			typeof(string), typeof(DisplayFormatTextControl));
		public static readonly DependencyProperty SuffixProperty = DependencyPropertyManager.RegisterAttached("Suffix",
			typeof(string), typeof(DisplayFormatTextControl));
		public static readonly DependencyProperty DisplayFormatSourceCollectionProperty =
			DependencyPropertyManager.RegisterAttached("DisplayFormatSourceCollection", typeof(ICollectionView),
			typeof(DisplayFormatTextControl));
		public static readonly DependencyProperty ExampleTextProperty =
			DependencyPropertyManager.RegisterAttached("ExampleText", typeof(string),
			typeof(DisplayFormatTextControl));
		public static void ClearCustomDisplayFormat() {
			displayFormatCustomItems.Clear();
		}
		#region DisplayFormatValues
		static DisplayFormatItem emptyDisplayFormat = new DisplayFormatItem(String.Empty, DisplayFormatGroupType.Default);
		static IEnumerable<DisplayFormatItem> displayFormatNumber = new List<DisplayFormatItem>() { 
			new DisplayFormatItem("#.00", DisplayFormatGroupType.Number),
			new DisplayFormatItem("#,#", DisplayFormatGroupType.Number),
			new DisplayFormatItem("E2", DisplayFormatGroupType.Number),
			new DisplayFormatItem("n", DisplayFormatGroupType.Number),
			new DisplayFormatItem("n1", DisplayFormatGroupType.Number),
			new DisplayFormatItem("n2", DisplayFormatGroupType.Number),
			new DisplayFormatItem("e", DisplayFormatGroupType.Number),
			new DisplayFormatItem("e1", DisplayFormatGroupType.Number),
			new DisplayFormatItem("f", DisplayFormatGroupType.Number),
			new DisplayFormatItem("f1", DisplayFormatGroupType.Number),
		};
		static IEnumerable<DisplayFormatItem> displayFormatPercent = new List<DisplayFormatItem>() { 
			new DisplayFormatItem("0.00", DisplayFormatGroupType.Percent),
			new DisplayFormatItem("0%", DisplayFormatGroupType.Percent),
		};
		static IEnumerable<DisplayFormatItem> displayFormatCurrency = new List<DisplayFormatItem>() { 
			new DisplayFormatItem("$0.00", DisplayFormatGroupType.Currency),
			new DisplayFormatItem("$0", DisplayFormatGroupType.Currency),
			new DisplayFormatItem("c", DisplayFormatGroupType.Currency),
			new DisplayFormatItem("c1", DisplayFormatGroupType.Currency),
			new DisplayFormatItem("c2", DisplayFormatGroupType.Currency),
		};
		static IEnumerable<DisplayFormatItem> displayFormatSpecial = new List<DisplayFormatItem>() { 
			new DisplayFormatItem("(###) ###-####", DisplayFormatGroupType.Special),
			new DisplayFormatItem("###-##-####", DisplayFormatGroupType.Special),
		};
		static IEnumerable<DisplayFormatItem> displayFormatDateTime = new List<DisplayFormatItem>() { 
			new DisplayFormatItem("yy/MM/dd", DisplayFormatGroupType.Datetime),
			new DisplayFormatItem("ddMMyy", DisplayFormatGroupType.Datetime),
			new DisplayFormatItem("yy/MM/dd hh:mm:ss", DisplayFormatGroupType.Datetime),
			new DisplayFormatItem("M/d/yyyy", DisplayFormatGroupType.Datetime),
			new DisplayFormatItem("M/d/yy", DisplayFormatGroupType.Datetime),
			new DisplayFormatItem("MM/dd/yy", DisplayFormatGroupType.Datetime),
			new DisplayFormatItem("MM/dd/yyyy", DisplayFormatGroupType.Datetime),
			new DisplayFormatItem("yyyy-MM-dd", DisplayFormatGroupType.Datetime),
			new DisplayFormatItem("dd-MMM-yy", DisplayFormatGroupType.Datetime),
			new DisplayFormatItem("dddd, MMMM dd, yyyy", DisplayFormatGroupType.Datetime),
		};
#if DEBUGTEST
		public
#endif
 static Dictionary<TypeCategory, DisplayFormatItem> displayFormatCustomItems = new Dictionary<TypeCategory, DisplayFormatItem>();
		public static IEnumerable<DisplayFormatItem> DisplayFormatNumber {
			get { return displayFormatNumber; }
		}
		public static IEnumerable<DisplayFormatItem> DisplayFormatPercent {
			get { return displayFormatPercent; }
		}
		public static IEnumerable<DisplayFormatItem> DisplayFormatCurrency {
			get { return displayFormatCurrency; }
		}
		public static IEnumerable<DisplayFormatItem> DisplayFormatSpecial {
			get { return displayFormatSpecial; }
		}
		public static IEnumerable<DisplayFormatItem> DisplayFormatDateTime {
			get { return displayFormatDateTime; }
		}
		#endregion
		ObservableCollection<DisplayFormatItem> displayFormatSource = new ObservableCollection<DisplayFormatItem>();
		string currentDisplayFormat = null;
		Type sourceValueType = typeof(Int32);
		public DisplayFormatTextControl() {
			this.DefaultStyleKey = typeof(DisplayFormatTextControl);
			DisplayFormatSourceCollection = new CollectionViewSource() { Source = DisplayFormatSource }.View;
#if !SL
			DisplayFormatSourceCollection.CollectionChanged += DisplayFormatSourceCollection_CollectionChanged;
#endif
		}
#if !SL
		void DisplayFormatSourceCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) { } 
#endif
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			LoadControls();
			SubscribeEvents();
			InitializeControlsTexts();
		}
		#region controls
		public ComboBoxEdit PART_DisplayFormatComboBox;
		TextEdit PART_PrefixValue;
		TextEdit PART_SuffixValue;
		TextBlock PART_PrefixCaption;
		TextBlock PART_SuffixCaption;
		TextBlock PART_DisplayTextFormat;
		TextBlock PART_ExampleCaption;
		TextBlock PART_ExampleValue;
		void LoadControls() {
			PART_DisplayFormatComboBox = GetTemplateChild("PART_DisplayFormatComboBox") as ComboBoxEdit;
			PART_PrefixValue = GetTemplateChild("PART_PrefixValue") as TextEdit;
			PART_SuffixValue = GetTemplateChild("PART_SuffixValue") as TextEdit;
			PART_PrefixCaption = GetTemplateChild("PART_PrefixCaption") as TextBlock;
			PART_SuffixCaption = GetTemplateChild("PART_SuffixCaption") as TextBlock;
			PART_DisplayTextFormat = GetTemplateChild("PART_DisplayTextFormat") as TextBlock;
			PART_ExampleCaption = GetTemplateChild("PART_ExampleCaption") as TextBlock;
			PART_ExampleValue = GetTemplateChild("PART_ExampleValue") as TextBlock;
		}
		#endregion
		void SubscribeEvents() {
			if(PART_DisplayFormatComboBox != null) {
				PART_DisplayFormatComboBox.Validate += DisplayFormatComboBox_Validate;
				PART_DisplayFormatComboBox.EditValueChanged += DisplayFormatComboBox_EditValueChanged;
				PART_DisplayFormatComboBox.ProcessNewValue += DisplayFormatComboBox_ProcessNewValue;
				PART_DisplayFormatComboBox.Spin += PART_DisplayFormatComboBox_Spin;
			}
			if(PART_PrefixValue != null) {
				PART_PrefixValue.Validate += Suffix_Validate;
				PART_PrefixValue.EditValueChanged += Prefix_EditValueChanged;
			}
			if(PART_SuffixValue != null) {
				PART_SuffixValue.Validate += Suffix_Validate;
				PART_SuffixValue.EditValueChanged += Suffix_EditValueChanged;
			}
		}
		void PART_DisplayFormatComboBox_Spin(object sender, SpinEventArgs e) {
			Dispatcher.BeginInvoke(new Action(() =>
				PART_DisplayFormatComboBox.DoValidate()
			));
		}
		void InitializeControlsTexts() {
			var localizer = EditorLocalizer.Active;
			if(PART_DisplayTextFormat != null)
				PART_DisplayTextFormat.Text = localizer.GetLocalizedString(EditorStringId.DisplayFormatTextControlDisplayFormatText);
			if(PART_ExampleCaption != null)
				PART_ExampleCaption.Text = localizer.GetLocalizedString(EditorStringId.DisplayFormatTextControlExample);
			if(PART_PrefixCaption != null)
				PART_PrefixCaption.Text = localizer.GetLocalizedString(EditorStringId.DisplayFormatTextControlPrefix);
			if(PART_SuffixCaption != null)
				PART_SuffixCaption.Text = localizer.GetLocalizedString(EditorStringId.DisplayFormatTextControlSuffix);
			if(PART_DisplayFormatComboBox != null)
				PART_DisplayFormatComboBox.NullText = localizer.GetLocalizedString(EditorStringId.DisplayFormatNullValue);
		}
		void SetFiltrerByType() {
			DisplayFormatSource.Clear();
			emptyDisplayFormat.Example = GetExample(emptyDisplayFormat.Value);
			DisplayFormatSource.Add(emptyDisplayFormat);
			switch(DisplayFormatExamleHelper.GetTypeCategory(SourceValueType)) {
				case TypeCategory.Numeric:
					AddNumericFormatElements();
					break;
				case TypeCategory.DateTime:
					AddDateTimeFormatElements();
					break;
			}
			AddCustomElement(DisplayFormatSource);
			DisplayFormatSourceCollection.GroupDescriptions.Clear();
			DisplayFormatSourceCollection.GroupDescriptions.Add(new PropertyGroupDescription("Group"));
		}
		void AddElementsToList(IList destination, IEnumerable<DisplayFormatItem> elements, bool generateExample = true) {
			foreach(DisplayFormatItem element in elements) {
				if(generateExample)
					element.Example = GetExample(element.Value);
				destination.Add(element);
			}
		}
		void AddElementsToSource(IEnumerable<DisplayFormatItem> elements) {
			AddElementsToList(DisplayFormatSource, elements);
		}
		void AddNumericFormatElements() {
			AddElementsToSource(displayFormatNumber);
			AddElementsToSource(displayFormatPercent);
			AddElementsToSource(displayFormatCurrency);
			AddElementsToSource(displayFormatSpecial);
		}
		void AddDateTimeFormatElements() {
			AddElementsToSource(displayFormatDateTime);
		}
		void AddCustomElement(IList destination, bool generateExample = true) {
			DisplayFormatItem displayFormatCustomItem = GetCustomElement(generateExample);
			if(displayFormatCustomItem != null)
				destination.Add(displayFormatCustomItem);
		}
		DisplayFormatItem GetCustomElement(bool generateExample = true) {
			DisplayFormatItem displayFormatCustomItem = null;
			if(displayFormatCustomItems.TryGetValue(DisplayFormatExamleHelper.GetTypeCategory(SourceValueType), out displayFormatCustomItem)) {
				if(generateExample)
					displayFormatCustomItem.Example = GetExample(displayFormatCustomItem.Value);
			}
			return displayFormatCustomItem;
		}
		string GetDisplayFormatAndUpdateParts(string value) {
			Prefix = String.Empty;
			Suffix = String.Empty;
			if(value == null || SourceValueType == null) {
				return null;
			}
			ExampleText = null;
			string prefix;
			string suffix;
			string analyzingFormat;
			DisplayFormatHelper.GetDisplayFormatParts(value, out prefix, out analyzingFormat, out suffix, NullValueDisplayFormat);
			Prefix = prefix;
			Suffix = suffix;
			AddNewDisplayFormatIfNeed(analyzingFormat);
			UpdateExample(analyzingFormat);
			return analyzingFormat;
		}
		void SetComboBoxValue() {
			if(currentDisplayFormat == null)
				return;
			PART_DisplayFormatComboBox.SelectedItem = GetItemFromDisplayFormatSource(currentDisplayFormat);
		}
		DisplayFormatItem GetItemFromDisplayFormatSource(string value) {
			foreach(DisplayFormatItem item in DisplayFormatSource) {
				if(item.Value == value) {
					return item;
				}
			}
			return null;
		}
		IEnumerable<DisplayFormatItem> GetAllDisplayFormats() {
			List<DisplayFormatItem> allDisplayFormats = new List<DisplayFormatItem>();
			allDisplayFormats.Add(emptyDisplayFormat);
			switch(DisplayFormatExamleHelper.GetTypeCategory(SourceValueType)) {
				case TypeCategory.Numeric:
					AddElementsToList(allDisplayFormats, displayFormatNumber, false);
					AddElementsToList(allDisplayFormats, displayFormatPercent, false);
					AddElementsToList(allDisplayFormats, displayFormatCurrency, false);
					AddElementsToList(allDisplayFormats, displayFormatSpecial, false);
					break;
				case TypeCategory.DateTime:
					AddElementsToList(allDisplayFormats, displayFormatDateTime, false);
					break;
			}
			AddCustomElement(allDisplayFormats, false);
			return allDisplayFormats;
		}
#if DEBUGTEST
		public
#else
		internal
#endif
 bool IsSourceContainsDisplayFromat(string displayFormat) {
			foreach(DisplayFormatItem displayFormatItem in GetAllDisplayFormats()) {
				if(displayFormatItem.Value == displayFormat)
					return true;
			}
			return false;
		}
		void AddNewDisplayFormatIfNeed(string displayFormat) {
			if(!IsSourceContainsDisplayFromat(displayFormat))
				AddNewDisplayFormat(displayFormat);
		}
		void AddNewDisplayFormat(string displayFormat) {
			DisplayFormatItem displayFormatCustomItem = GetCustomElement();
			if(displayFormatCustomItem == null) {
				displayFormatCustomItem = new DisplayFormatItem(displayFormat, DisplayFormatGroupType.Custom);
				displayFormatCustomItem.Example = GetExample(displayFormatCustomItem.Value);
				displayFormatCustomItems.Add(DisplayFormatExamleHelper.GetTypeCategory(SourceValueType), displayFormatCustomItem);
			}
			else {
				displayFormatCustomItem.Example = GetExample(displayFormat);
				displayFormatCustomItem.Value = displayFormat;
			}
			SetFiltrerByType();
		}
		public void AddDisplayFormatToList(string newDisplayFormat) {
			AddNewDisplayFormatIfNeed(newDisplayFormat);
		}
		void OnCurrentDisplayFormatChanged() {
			if(CurrentDisplayFormatChanged != null)
				CurrentDisplayFormatChanged(this, new EditValueChangedEventArgs(null, CurrentDisplayFormat));
		}
		void UpdateExample(string clearDisplayFormat) {
			ExampleText = GetExample(DisplayFormatHelper.GetDisplayFormatFromParts(Prefix, clearDisplayFormat, Suffix));
		}
#if DEBUGTEST
		public
#endif
 string GetExample(string displayFormat, CultureInfo culture) {
			List<object> args = new List<object>() { DisplayFormatExamleHelper.GetExample(sourceValueType) };
			if(String.IsNullOrEmpty(displayFormat))
				return String.Format(culture, "{0}", args.ToArray());
			if(!displayFormat.Contains("{0"))
				return DisplayFormatHelper.GetDisplayTextFromDisplayFormat(culture, "{0:" + displayFormat + "}", args.ToArray());
			string updatedDisplayFormat = displayFormat;
			if(updatedDisplayFormat.Contains("{1")) {
				if(!String.IsNullOrEmpty(SecondParameterName))
					args.Add(SecondParameterName);
				else
					args.Add(String.Empty);
			}
			return DisplayFormatHelper.GetDisplayTextFromDisplayFormat(culture, displayFormat, args.ToArray());
		}
#if DEBUGTEST
		public
#endif
 string GetExample(string displayFormat) {
			return GetExample(displayFormat, CultureInfo.CurrentCulture);
		}
		void UpdateControlState(string value) {
			IsEnabled = value != null;
		}
		void DisplayFormatComboBox_ProcessNewValue(DependencyObject sender, ProcessNewValueEventArgs e) {
			CleanNotValidDisplayFormat();
			string text = e.DisplayText;
			ErrorParameters err = DisplayFormatValidationHelper.IsDisplayFormatStringValid(ref text);
			if(err == null || err.ErrorType != ErrorType.None) {
				SelectNotValidComboBoxValue(e.DisplayText);
				e.Handled = true;
				return;
			}
			ComboBoxValueChanged(e.DisplayText);
			e.Handled = true;
		}
		DisplayFormatItem notValidItem;
		void CleanNotValidDisplayFormat() {
			if(notValidItem != null && DisplayFormatSource.Contains(notValidItem))
				DisplayFormatSource.Remove(notValidItem);
		}
		void SelectNotValidComboBoxValue(string notValidDisplayFormat) {
			notValidItem = new DisplayFormatItem(notValidDisplayFormat, DisplayFormatGroupType.Custom, String.Empty);
			DisplayFormatSource.Add(notValidItem);
		}
		void DisplayFormatComboBox_EditValueChanged(object sender, EditValueChangedEventArgs e) {
			if(e.NewValue == null)
				return;
			if(e.OldValue == e.NewValue)
				return;
			CleanNotValidDisplayFormat();
			ComboBoxValueChanged((string)e.NewValue);
			e.Handled = true;
		}
		void ComboBoxValueChanged(string actualDisplayFormat) {
			SetCurrentDisplayFormat(actualDisplayFormat, false);
			OnCurrentDisplayFormatChanged();
		}
		void Suffix_EditValueChanged(object sender, EditValueChangedEventArgs e) {
			BorderTextChanged(true, (string)e.NewValue);
		}
		private void Prefix_EditValueChanged(object sender, EditValueChangedEventArgs e) {
			BorderTextChanged(false, (string)e.NewValue);
		}
		void BorderTextChanged(bool suffix, string text) {
			if(PART_DisplayFormatComboBox.SelectedItem == null)
				return;
			if(suffix)
				Suffix = text;
			else
				Prefix = text;
			OnCurrentDisplayFormatChanged();
			UpdateExample(currentDisplayFormat);
			UpdateDisplayFormatSourceExamples();
		}
		void UpdateDisplayFormatSourceExamples() {
			if(DisplayFormatSource == null)
				return;
			foreach(DisplayFormatItem item in DisplayFormatSource)
				item.Example = GetExample(item.Value);
		}
		public IList DisplayFormatSource {
			get { return displayFormatSource; }
		}
		public ICollectionView DisplayFormatSourceCollection {
			get { return (ICollectionView)GetValue(DisplayFormatSourceCollectionProperty); }
			set { SetValue(DisplayFormatSourceCollectionProperty, value); }
		}
		public string CurrentDisplayFormat {
			get {
				string format = DisplayFormatHelper.GetDisplayFormatFromParts(Prefix, currentDisplayFormat, Suffix);
				return format == NullValueDisplayFormat ? String.Empty : format;
			}
			set { SetCurrentDisplayFormat(value, true); }
		}
		void SetCurrentDisplayFormat(string actualDisplayFormat, bool newItemSelected) {
			if(actualDisplayFormat == null && currentDisplayFormat == null)
				return;
			PART_DisplayFormatComboBox.SelectedItem = null;
			currentDisplayFormat = newItemSelected ?
				GetDisplayFormatAndUpdateParts(actualDisplayFormat) :
				GetDisplayFormatAndUpdateParts(DisplayFormatHelper.GetDisplayFormatFromParts(Prefix, actualDisplayFormat, Suffix));
			SetComboBoxValue();
			UpdateControlState(actualDisplayFormat);
		}
		public string NullValueDisplayFormat { get; set; }
		public string SecondParameterName { get; set; }
		public string Prefix {
			get { return (string)GetValue(PrefixProperty); }
			set { SetValue(PrefixProperty, value); }
		}
		public string Suffix {
			get { return (string)GetValue(SuffixProperty); }
			set { SetValue(SuffixProperty, value); }
		}
		public string ExampleText {
			get { return (string)GetValue(ExampleTextProperty); }
			set { SetValue(ExampleTextProperty, value); }
		}
		public Type SourceValueType {
			get { return sourceValueType; }
			set {
				ChangeProperty(ref sourceValueType, value, "SourceValueType");
				SetFiltrerByType();
			}
		}
		protected bool ChangeProperty<T>(ref T property, T value, string propertyName) {
			if(object.Equals(property, value))
				return false;
			property = value;
			RaisePropertyChanged(propertyName);
			return true;
		}
		void RaisePropertyChanged(string name) {
			if(PropertyChanged == null)
				return;
			PropertyChanged(this, new PropertyChangedEventArgs(name));
		}
		#region validation
		void DisplayFormatComboBox_Validate(object sender, ValidationEventArgs e) {
			DisplayFormatValidate(e, '0');
		}
		void Suffix_Validate(object sender, ValidationEventArgs e) {
			DisplayFormatValidate(e, '1');
		}
		void DisplayFormatValidate(ValidationEventArgs e, char inplaceChar) {
			string text = (string)e.Value;
			ErrorParameters param;
			if(inplaceChar == '0')
				param = DisplayFormatValidationHelper.IsDisplayFormatStringValid(ref text);
			else
				param = DisplayFormatValidationHelper.IsSuffixValid(text);
			if(param == null)
				param = DisplayFormatValidationHelper.GetNullErrorParameters();
			e.ErrorType = param.ErrorType;
			e.ErrorContent = param.ErrorContent;
			e.IsValid = param.ErrorType == ErrorType.None;
			e.Handled = true;
		}
		#endregion
		public event PropertyChangedEventHandler PropertyChanged;
		public event EditValueChangedEventHandler CurrentDisplayFormatChanged;
	}
	public class ExampleEventArgs : EventArgs {
		public string Value { get; set; }
		public ExampleEventArgs(string value) {
			Value = value;
		}
	}
	public class DisplayFormatItem : INotifyPropertyChanged {
		string example;
		string value;
		DisplayFormatGroupType group;
		public DisplayFormatItem(string value, DisplayFormatGroupType group, string example) {
			Value = value;
			Group = group;
			Example = value;
		}
		public DisplayFormatItem(string value, DisplayFormatGroupType group)
			: this(value, group, "") { }
		public DisplayFormatItem(string value)
			: this(value, DisplayFormatGroupType.Default, "") { }
		public string Value {
			get { return value; }
			set { ChangeProperty(ref this.value, value, "Value"); }
		}
		public DisplayFormatGroupType Group {
			get { return group; }
			set { ChangeProperty(ref group, value, "Group"); }
		}
		public string Example {
			get { return example; }
			set { ChangeProperty(ref example, value, "Example"); }
		}
		protected bool ChangeProperty<T>(ref T property, T value, string propertyName) {
			if(object.Equals(property, value))
				return false;
			property = value;
			RaisePropertyChanged(propertyName);
			return true;
		}
		void RaisePropertyChanged(string name) {
			if(PropertyChanged == null)
				return;
			PropertyChanged(this, new PropertyChangedEventArgs(name));
		}
		public event PropertyChangedEventHandler PropertyChanged;
	}
	public class NullDisplayFormatToTextConverter : IValueConverter {
		DevExpress.Utils.Localization.XtraLocalizer<EditorStringId> localizer = EditorLocalizer.Active;
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			string actualString = (string)value;
			if(String.IsNullOrEmpty(actualString))
				return localizer.GetLocalizedString(EditorStringId.DisplayFormatNullValue);
			return value;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class GroupNameToCaptionConverter : IValueConverter {
		DevExpress.Utils.Localization.XtraLocalizer<EditorStringId> localizer = EditorLocalizer.Active;
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if(!(value is DisplayFormatGroupType))
				return String.Empty;
			DisplayFormatGroupType groupType = (DisplayFormatGroupType)value;
			switch(groupType) {
				case DisplayFormatGroupType.Currency: return localizer.GetLocalizedString(EditorStringId.DisplayFormatGroupTypeCurrency);
				case DisplayFormatGroupType.Custom: return localizer.GetLocalizedString(EditorStringId.DisplayFormatGroupTypeCustom);
				case DisplayFormatGroupType.Datetime: return localizer.GetLocalizedString(EditorStringId.DisplayFormatGroupTypeDatetime);
				case DisplayFormatGroupType.Default: return localizer.GetLocalizedString(EditorStringId.DisplayFormatGroupTypeDefault);
				case DisplayFormatGroupType.Number: return localizer.GetLocalizedString(EditorStringId.DisplayFormatGroupTypeNumber);
				case DisplayFormatGroupType.Percent: return localizer.GetLocalizedString(EditorStringId.DisplayFormatGroupTypePercent);
				case DisplayFormatGroupType.Special: return localizer.GetLocalizedString(EditorStringId.DisplayFormatGroupTypeSpecial);
				default: return String.Empty;
			}
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
