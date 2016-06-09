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
using System.Drawing;
using System.Globalization;
using DevExpress.Charts.Native;
using DevExpress.Utils.FormatStrings;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraReports.Design;
namespace DevExpress.XtraCharts.Wizard {
	internal partial class TextPatternControl : ChartUserControl {
		SeriesBase series;
		Locker updateLocker = new Locker();
		string[] availablePlaceholders;
		CategoryItem[] categoryItems;
		List<PlaceholderInfo> placeholdersInfo = new List<PlaceholderInfo>();
		bool placeholderSelected;
		PlaceholderInfo selectedPlaceholder;
		bool supressCarretValidation;
		bool placeholderSelectionChanged;
		string CurrentPlaceholderDescription { get { return (string)lbPlaceholders.SelectedItem; } }
		protected virtual bool IsLegendPointOptions { get { return true; } }
		protected SeriesBase Series { get { return series; } }
		protected virtual string TextPattern {
			get { return series.LegendTextPattern; }
			set { series.LegendTextPattern = value; }
		}
		public TextPatternControl() {
			InitializeComponent();
		}
		void InitControls() {
			this.availablePlaceholders = GetAvailablePlaceholders();
			InitializePlaceholders();
			InitializeFormatItems();
			UpdatePlaceholdersInfo();
		}
		void InitializePlaceholders() {
			lbPlaceholders.Items.Clear();
			foreach (string placeholder in availablePlaceholders)
				lbPlaceholders.Items.Add(PatternEditorUtils.GetDescriptionForPatternPlaceholder(placeholder));
		}
		void InitializeFormatItems() {
			XmlDataLoader xmlDataLoader = new XmlDataLoader();
			cbFormat.Properties.Items.Clear();
			categoryItems = xmlDataLoader.CreateCategoryItems();
			categoryItems = FilterCategoryItems(categoryItems);
			foreach (CategoryItem item in categoryItems)
				if (item.Type == typeof(DateTime)) {
					List<string> formatStrings = new List<string>(PatternUtils.GetCustomDateTimeFormats());
					formatStrings.AddRange(CultureInfo.CurrentCulture.DateTimeFormat.GetAllDateTimePatterns());
					item.SetFormatStrings(formatStrings.ToArray());
					break;
				}
			cbFormat.Properties.Items.AddRange(categoryItems);
			cbFormat.SelectedIndex = 0;
		}
		CategoryItem[] FilterCategoryItems(CategoryItem[] items) {
			System.Collections.Generic.List<CategoryItem> result = new System.Collections.Generic.List<CategoryItem>();
			foreach (CategoryItem categoryItem in items) {
				if (categoryItem.ToString() != CategoryItem.GetDisplayName(FormatStringEditorForm.general))
					result.Add(categoryItem);
			}
			return result.ToArray();
		}
		void UpdatePlaceholdersInfo() {
			placeholdersInfo.Clear();
			string pattern = tbPattern.Text;
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
		void chSynchronize_CheckedChanged(object sender, EventArgs e) {
			if (!updateLocker.IsLocked) {
				if (chSynchronize.Checked)
					series.LegendTextPattern = String.Empty;
				else
					SeriesBaseWizardHelper.BreakPointOptionsRelations(series);
				UpdateControls();
			}
		}
		void tbPattern_TextChanged(object sender, EventArgs e) {
			UpdatePlaceholdersInfo();
			if (!supressCarretValidation && tbPattern.SelectionLength == 0) {
				ValidateCarretPosition();
				if (!placeholderSelected && placeholderSelectionChanged)
					DeselectAll();
			}
			if (!updateLocker.IsLocked)
				TextPattern = tbPattern.Text;
		}
		void lbPlaceholders_SelectedIndexChanged(object sender, EventArgs e) {
		}
		void lbPlaceholders_DoubleClick(object sender, EventArgs e) {
			AddPlaceholder();
		}
		void cbAdd_Click(object sender, EventArgs e) {
			AddPlaceholder();
		}
		void cbFormat_SelectedIndexChanged(object sender, EventArgs e) {
			lbStandardTypes.Items.Clear();
			lbStandardTypes.Items.AddRange(((CategoryItem)cbFormat.SelectedItem).FormatStrings);
		}
		void lbStandardTypes_Click(object sender, EventArgs e) {
			if (!placeholderSelected)
				SelectNearestPlaceholder();
		}
		void lbStandardTypes_SelectedIndexChanged(object sender, EventArgs e) {
			if (placeholderSelected) {
				AddFormat();
				ValidateCarretPosition();
			}
			tbPattern.Focus();
		}
		void tbPattern_SelectionChanged(object sender, EventArgs e) {
			UpdatePlaceholdersInfo();
			if (!supressCarretValidation && tbPattern.SelectionLength == 0) {
				ValidateCarretPosition();
				if (placeholderSelectionChanged)
					if (placeholderSelected) {
						SelectPlaceholder();
						SelectPlaceholderDescription();
						SelectPatternFormat();
					}
					else
						DeselectAll();
			}
		}
		void SelectPlaceholder() {
			DeselectAll();
			supressCarretValidation = true;
			int carretPosition = tbPattern.SelectionStart;
			tbPattern.SelectionStart = selectedPlaceholder.PlaceholderStart - 1;
			tbPattern.SelectionLength = selectedPlaceholder.PlaceholderLength + 2;
			tbPattern.SelectionBackColor = Color.LightGray;
			tbPattern.SelectionLength = 0;
			tbPattern.SelectionStart = carretPosition;
			supressCarretValidation = false;
		}
		void SelectPlaceholderDescription() {
			string descrition = PatternEditorUtils.GetDescriptionForPatternPlaceholder(selectedPlaceholder.Placeholder);
			lbPlaceholders.SelectedItem = descrition;
		}
		void SelectPatternFormat() {
			supressCarretValidation = true;
			bool isFormatFind = false;
			if (!String.IsNullOrEmpty(selectedPlaceholder.Format)) {
				foreach (CategoryItem item in categoryItems)
					foreach (string formatString in item.FormatStrings)
						if (formatString == selectedPlaceholder.Format) {
							if (cbFormat.SelectedItem != item)
								cbFormat.SelectedItem = item;
							if ((string)lbStandardTypes.SelectedItem != formatString)
								lbStandardTypes.SelectedItem = formatString;
							isFormatFind = true;
						}
			}
			if (selectedPlaceholder.PlaceholderWithFormat.Contains(":") && !isFormatFind)
				lbStandardTypes.SelectedIndex = -1;
			supressCarretValidation = false;
		}
		void AddPlaceholder() {
			if (placeholderSelected)
				MoveCarretOutOfPlaceholder();
			tbPattern.SelectedText = "{" + PatternEditorUtils.GetPatternPlaceholderForDescription(CurrentPlaceholderDescription) + "}";
			tbPattern.Focus();
		}
		void MoveCarretOutOfPlaceholder() {
			tbPattern.SelectionStart = selectedPlaceholder.PlaceholderEnd + 1;
		}
		void SelectNearestPlaceholder() {
			int carretPosition = tbPattern.SelectionStart;
			int minDistance = tbPattern.Text.Length + 1;
			int nearestPlaceholderPosition = carretPosition;
			foreach (PlaceholderInfo info in placeholdersInfo) {
				int placeholderStartDistance = Math.Abs(info.PlaceholderStart - carretPosition);
				int placeholderEndDistance = Math.Abs(carretPosition - info.PlaceholderEnd);
				if (placeholderStartDistance < minDistance || placeholderEndDistance <= minDistance) {
					minDistance = Math.Min(placeholderStartDistance, placeholderEndDistance);
					nearestPlaceholderPosition = info.PlaceholderStart;
				}
			}
			tbPattern.SelectionStart = nearestPlaceholderPosition;
		}
		void AddFormat() {
			supressCarretValidation = true;
			if (lbStandardTypes.SelectedIndex >= 0 && selectedPlaceholder.Format != lbStandardTypes.SelectedItem.ToString()) {
				tbPattern.SelectionStart = selectedPlaceholder.PlaceholderStart;
				tbPattern.SelectionLength = selectedPlaceholder.PlaceholderLength;
				tbPattern.SelectedText = selectedPlaceholder.Placeholder + ":" + lbStandardTypes.SelectedItem;
			}
			supressCarretValidation = false;
		}
		void ValidateCarretPosition() {
			placeholderSelectionChanged = false;
			bool previousPlaceholderSelected = placeholderSelected;
			placeholderSelected = false;
			int carretPosition = tbPattern.SelectionStart;
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
		}
		void DeselectAll() {
			supressCarretValidation = true;
			int carretPosition = tbPattern.SelectionStart;
			tbPattern.SelectionStart = 0;
			tbPattern.SelectionLength = tbPattern.Text.Length;
			tbPattern.SelectionBackColor = Color.White;
			tbPattern.SelectionLength = 0;
			tbPattern.SelectionStart = carretPosition;
			supressCarretValidation = false;
		}
		void UpdateControls() {
			updateLocker.Lock();
			try {
				pnlPointOptionsControls.Enabled = !chSynchronize.Checked;
				if (!pnlPointOptionsControls.Enabled)
					tbPattern.Text = PatternEditorUtils.GetActualPattern(series);
				else
					tbPattern.Text = TextPattern;
			}
			finally {
				updateLocker.Unlock();
			}
		}
		protected virtual string[] GetAvailablePlaceholders() {
			return PatternEditorUtils.GetAvailablePlaceholdersForPoint(series);
		}
		public void Initialize(SeriesBase series) {
			this.series = series;
			InitControls();
			pnlSynchronize.Visible = IsLegendPointOptions;
			chSynchronize.Checked = pnlSynchronize.Visible && String.IsNullOrEmpty(series.LegendTextPattern);
			UpdateControls();
		}
	}
	internal partial class SeriesLabelTextPatternControl : TextPatternControl {
		protected override bool IsLegendPointOptions { get { return false; } }
		protected override string TextPattern {
			get { return Series.Label.TextPattern; }
			set { Series.Label.TextPattern = value; }
		}
	}
	internal partial class AxisLabelTextPatternControl : TextPatternControl {
		AxisLabel label;
		protected override bool IsLegendPointOptions { get { return false; } }
		protected override string TextPattern {
			get { return label.TextPattern; }
			set { label.TextPattern = value; }
		}
		public void Initialize(AxisLabel label) {
			this.label = label;
			base.Initialize(null);
		}
		protected override string[] GetAvailablePlaceholders() {
			return PatternEditorUtils.GetAvailablePlaceholdersForAxisLabel(label);
		}
	}
}
