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
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts.Localization;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Charts.Designer.Native {
	struct PlaceholderInfo {
		int placeholderStart;
		int placeholderLength;
		int placeholderEnd;
		string placeholder;
		string placeholderWithFormat;
		string format;
		public int PlaceholderStart { get { return placeholderStart; } }
		public int PlaceholderLength { get { return placeholderLength; } }
		public int PlaceholderEnd { get { return placeholderEnd; } }
		public string Placeholder { get { return placeholder; } }
		public string PlaceholderWithFormat { get { return placeholderWithFormat; } }
		public string Format { get { return format; } }
		public PlaceholderInfo(int placeholderStart, int placeholderLength, string placeholder, string placeholderWithFormat) {
			this.placeholderStart = placeholderStart;
			this.placeholderLength = placeholderLength;
			this.placeholder = placeholder;
			this.placeholderWithFormat = placeholderWithFormat;
			placeholderEnd = placeholderStart + placeholderLength;
			format = String.Empty;
			int formatDelimiter = placeholderWithFormat.IndexOf(':');
			if (formatDelimiter >= 0)
				format = placeholderWithFormat.Substring(formatDelimiter + 1);
		}
	}
	struct DisplayFormatGroup {
		readonly DisplayFormatGroupType type;
		readonly List<string> formatStrings;
		public DisplayFormatGroupType Type { get { return type; } }
		public List<string> FormatStrings { get { return formatStrings; } }
		public DisplayFormatGroup(DisplayFormatGroupType type, IEnumerable<DisplayFormatItem> formatGroupSource) {
			this.type = type;
			this.formatStrings = new List<string>();
			if (type == DisplayFormatGroupType.Datetime)
				formatStrings.AddRange(PatternUtils.GetCustomDateTimeFormats());
			foreach (DisplayFormatItem item in formatGroupSource)
				formatStrings.Add(item.Value);
			if (type == DisplayFormatGroupType.Datetime)
				formatStrings.AddRange(CultureInfo.CurrentCulture.DateTimeFormat.GetAllDateTimePatterns());
		}
		public override string ToString() {
			return type.ToString();
		}
	}
	public partial class PatternEditor : DXWindow {
		IPatternValuesSource patternSource;
		string[] availablePlaceholders;
		bool placeholderSelected;
		bool placeholderSelectionChanged;
		List<PlaceholderInfo> placeholdersInfo = new List<PlaceholderInfo>();
		PlaceholderInfo selectedPlaceholder;
		bool supressCarretValidation;
		DisplayFormatGroup[] formatGroups;
		Brush initialSelectionBrush;
		string CurrentPlaceholderDescription { get { return (string)lbPlaceholders.SelectedItem; } }
		public string Pattern { get { return tePattern.Text; } }
		public PatternEditor(Theme currentTheme, string pattern, string[] placeholders, IPatternValuesSource patternSource) {
			InitializeComponent();
			try {
				ThemeManager.SetTheme(this, currentTheme);
			}
			catch (Exception e) {
				ChartDebug.WriteWarning("Unable to set the MetropolisLight theme. Exception occured: " + e.Message);
			}
			tePattern.Text = String.IsNullOrEmpty(pattern) ? null : pattern;
			this.availablePlaceholders = placeholders;
			this.patternSource = patternSource;
			this.initialSelectionBrush = tePattern.SelectionBrush;
			FillStandardItemsListBox();
			InitializeFormatItems();
			UpdatePlaceholdersInfo();
			UpdateSample();
		}
		void FillStandardItemsListBox() {
			foreach (string placeholder in availablePlaceholders)
				lbPlaceholders.Items.Add(PatternEditorUtils.GetDescriptionForPatternPlaceholder(placeholder));
		}
		void InitializeFormatItems() {
			formatGroups = new DisplayFormatGroup[5];
			formatGroups[0] = new DisplayFormatGroup(DisplayFormatGroupType.Datetime, DisplayFormatTextControl.DisplayFormatDateTime);
			formatGroups[1] = new DisplayFormatGroup(DisplayFormatGroupType.Number, DisplayFormatTextControl.DisplayFormatNumber);
			formatGroups[2] = new DisplayFormatGroup(DisplayFormatGroupType.Percent, DisplayFormatTextControl.DisplayFormatPercent);
			formatGroups[3] = new DisplayFormatGroup(DisplayFormatGroupType.Currency, DisplayFormatTextControl.DisplayFormatCurrency);
			formatGroups[4] = new DisplayFormatGroup(DisplayFormatGroupType.Special, DisplayFormatTextControl.DisplayFormatSpecial);
			cbFormat.Items.Clear();
			foreach (DisplayFormatGroup group in formatGroups)
				cbFormat.Items.Add(group);
			cbFormat.SelectedIndex = 0;
		}
		void UpdatePlaceholdersInfo() {
			placeholdersInfo.Clear();
			string pattern = tePattern.Text;
			string[] patternParts = pattern.Split('{');
			if (patternParts.Length > 1) {
				int[] startPositions = new int[patternParts.Length - 1];
				int startPositionCounter = 0;
				for (int i = 0; i < pattern.Length; i++)
					if (pattern[i] == '{')
						startPositions[startPositionCounter++] = i + 1;
				startPositionCounter = startPositions[0] == 0 ? 0 : -1;
				foreach (string patternPart in patternParts) {
					if (patternPart.Contains("}") && startPositionCounter >= 0) {
						int placeholderStart = startPositions[startPositionCounter];
						string placeholderWithFormat = patternPart.Substring(0, patternPart.IndexOf('}'));
						string placeholder = placeholderWithFormat;
						if (placeholderWithFormat.Contains(":"))
							placeholder = patternPart.Substring(0, patternPart.IndexOf(':'));
						int placeholderLength = placeholderWithFormat.Length;
						PlaceholderInfo info = new PlaceholderInfo(placeholderStart, placeholderLength, placeholder, placeholderWithFormat);
						placeholdersInfo.Add(info);
					}
					startPositionCounter++;
				}
			}
		}
		void UpdateSample() {
			string sampleText = ChartLocalizer.GetString(ChartStringId.PatternEditorPreviewCaption);
			if (placeholderSelected)
				sampleText = PatternEditorUtils.GetPatternText("{" + selectedPlaceholder.PlaceholderWithFormat + "}", patternSource);
			tbSample.Text = sampleText;
			tbSample.IsEnabled = placeholderSelected;
		}
		void ValidateCarretPosition() {
			placeholderSelectionChanged = false;
			bool previousPlaceholderSelected = placeholderSelected;
			placeholderSelected = false;
			int carretPosition = tePattern.SelectionStart;
			foreach (PlaceholderInfo placeholder in placeholdersInfo) {
				if (carretPosition >= placeholder.PlaceholderStart && carretPosition <= placeholder.PlaceholderEnd) {
					placeholderSelected = true;
					if (!selectedPlaceholder.Equals(placeholder))
						placeholderSelectionChanged = true;
					selectedPlaceholder = placeholder;
					break;
				}
			}
			if (placeholderSelected != previousPlaceholderSelected)
				placeholderSelectionChanged = true;
			UpdateSample();
		}
		void DeselectAll() {
			supressCarretValidation = true;
			int carretPosition = tePattern.SelectionStart;
			tePattern.SelectionStart = 0;
			tePattern.SelectionLength = tePattern.Text.Length;
			tePattern.SelectionBrush = initialSelectionBrush;
			tePattern.SelectionLength = 0;
			tePattern.SelectionStart = carretPosition;
			supressCarretValidation = false;
		}
		void SelectPlaceholder() {
			DeselectAll();
			supressCarretValidation = true;
			int carretPosition = tePattern.SelectionStart;
			tePattern.SelectionStart = selectedPlaceholder.PlaceholderStart - 1;
			tePattern.SelectionLength = selectedPlaceholder.PlaceholderLength + 2;
			tePattern.SelectionBrush = Brushes.Gray;
			tePattern.SelectionLength = 0;
			tePattern.SelectionStart = carretPosition;
			supressCarretValidation = false;
		}
		void SelectPlaceholderDescription() {
			string descrition = PatternEditorUtils.GetDescriptionForPatternPlaceholder(selectedPlaceholder.Placeholder);
			lbPlaceholders.SelectedItem = descrition;
		}
		void AddFormat() {
			supressCarretValidation = true;
			if (lbStandardTypes.SelectedIndex >= 0 && selectedPlaceholder.Format != lbStandardTypes.SelectedItem.ToString()) {
				tePattern.SelectionStart = selectedPlaceholder.PlaceholderStart;
				tePattern.SelectionLength = selectedPlaceholder.PlaceholderLength;
				tePattern.SelectedText = selectedPlaceholder.Placeholder + ":" + lbStandardTypes.SelectedItem;
			}
			supressCarretValidation = false;
		}
		void AddPlaceholder() {
			if (placeholderSelected)
				MoveCarretOutOfPlaceholder();
			tePattern.SelectedText = "{" + PatternEditorUtils.GetPatternPlaceholderForDescription(CurrentPlaceholderDescription) + "}";
			tePattern.CaretIndex = tePattern.SelectionStart;
			tePattern.Focus();
		}
		void MoveCarretOutOfPlaceholder() {
			tePattern.SelectionStart = selectedPlaceholder.PlaceholderEnd + 1;
		}
		void SelectPatternFormat() {
			supressCarretValidation = true;
			bool isFormatFind = false;
			if (!String.IsNullOrEmpty(selectedPlaceholder.Format)) {
				foreach (DisplayFormatGroup group in formatGroups)
					foreach (string formatString in group.FormatStrings)
						if (formatString == selectedPlaceholder.Format) {
							if (((DisplayFormatGroup)cbFormat.SelectedItem).Type != group.Type)
								cbFormat.SelectedItem = group;
							if ((string)lbStandardTypes.SelectedItem != formatString)
								lbStandardTypes.SelectedItem = formatString;
							lbStandardTypes.ScrollIntoView(formatString);
							isFormatFind = true;
						}
			}
			if (selectedPlaceholder.PlaceholderWithFormat.Contains(":") && !isFormatFind)
				lbStandardTypes.SelectedIndex = -1;
			supressCarretValidation = false;
		}
		void SelectNearestPlaceholder() {
			int carretPosition = tePattern.SelectionStart;
			int minDistance = tePattern.Text.Length + 1;
			int nearestPlaceholderPosition = carretPosition;
			foreach (PlaceholderInfo info in placeholdersInfo) {
				int placeholderStartDistance = Math.Abs(info.PlaceholderStart - carretPosition);
				int placeholderEndDistance = Math.Abs(carretPosition - info.PlaceholderEnd);
				if (placeholderStartDistance < minDistance || placeholderEndDistance <= minDistance) {
					minDistance = Math.Min(placeholderStartDistance, placeholderEndDistance);
					nearestPlaceholderPosition = info.PlaceholderStart;
				}
			}
			tePattern.SelectionStart = nearestPlaceholderPosition;
		}
		bool ValidatePattern() {
			foreach (PlaceholderInfo info in placeholdersInfo) {
				bool isCorrect = false;
				foreach (string checkingPlaceholder in availablePlaceholders)
					if (info.Placeholder == checkingPlaceholder) {
						isCorrect = true;
						break;
					}
				if (!isCorrect) {
					tePattern.SelectionStart = info.PlaceholderStart;
					DXMessageBox.Show(ChartLocalizer.GetString(ChartStringId.InvalidPlaceholder) + ": " + info.Placeholder, ChartLocalizer.GetString(ChartStringId.ErrorTitle),
						MessageBoxButton.OK, MessageBoxImage.Error);
					return false;
				}
			}
			return true;
		}
		void lbPlaceholders_SelectedIndexChanged(object sender, RoutedEventArgs e) {
			tePattern.Focus();
		}
		void lbPlaceholders_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
			AddPlaceholder();
		}
		void btnAdd_Click(object sender, RoutedEventArgs e) {
			AddPlaceholder();
		}
		void btnAddAll_Click(object sender, RoutedEventArgs e) {
			if (placeholderSelected)
				MoveCarretOutOfPlaceholder();
			foreach (string item in lbPlaceholders.Items) {
				tePattern.SelectedText = "{" + PatternEditorUtils.GetPatternPlaceholderForDescription(item) + "}";
				tePattern.CaretIndex = tePattern.SelectionStart;
			}
			tePattern.Focus();
		}
		void tePattern_TextChanged(object sender, TextChangedEventArgs e) {
			UpdatePlaceholdersInfo();
			if (!supressCarretValidation && tePattern.SelectionLength == 0) {
				ValidateCarretPosition();
				if (!placeholderSelected && placeholderSelectionChanged)
					DeselectAll();
			}
		}
		void tePattern_SelectionChanged(object sender, RoutedEventArgs e) {
			UpdatePlaceholdersInfo();
			if (!supressCarretValidation && tePattern.SelectionLength == 0) {
				ValidateCarretPosition();
				if (placeholderSelectionChanged)
					if (placeholderSelected) {
						SelectPlaceholderDescription();
						SelectPatternFormat();
					}
					else
						DeselectAll();
			}
		}
		void cbFormat_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			lbStandardTypes.Items.Clear();
			lbStandardTypes.Items.AddRange(((DisplayFormatGroup)cbFormat.SelectedItem).FormatStrings.ToArray());
		}
		void lbStandardTypes_SelectedIndexChanged(object sender, RoutedEventArgs e) {
			if (placeholderSelected) {
				AddFormat();
				ValidateCarretPosition();
			}
			tePattern.Focus();
		}
		void lbStandardTypes_MouseUp(object sender, MouseButtonEventArgs e) {
			if (!placeholderSelected)
				SelectNearestPlaceholder();
			if (placeholderSelected && string.IsNullOrEmpty(selectedPlaceholder.Format)) {
				AddFormat();
				ValidateCarretPosition();
			}
			tePattern.Focus();
		}
		void btnOk_Click(object sender, RoutedEventArgs e) {
			if (!ValidatePattern())
				tePattern.Focus();
			else {
				DialogResult = true;
				Close();
			}
		}
	}
}
