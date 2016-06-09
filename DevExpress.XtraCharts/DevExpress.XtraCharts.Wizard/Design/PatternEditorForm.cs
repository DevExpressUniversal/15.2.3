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
using System.Windows.Forms;
using DevExpress.Charts.Native;
using DevExpress.XtraEditors;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraReports.Design;
using DevExpress.XtraTab;
using DevExpress.Utils.FormatStrings;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts.Design {
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
	public class PatternEditorForm : XtraForm {
		IPatternValuesSource patternSource;
		string[] availablePlaceholders;
		bool placeholderSelected;
		bool placeholderSelectionChanged;
		List<PlaceholderInfo> placeholdersInfo = new List<PlaceholderInfo>();
		PlaceholderInfo selectedPlaceholder;
		bool supressCarretValidation;
		CategoryItem[] categoryItems;
		private SimpleButton btnOk;
		private SimpleButton btnCancel;
		private DevExpress.Utils.Frames.PropertyGridEx propertyGrid;
		private ListBoxControl lbPlaceholders;
		private RichTextBox tbPattern;
		private ListBoxControl lbStandardTypes;
		private ComboBoxEdit cbFormat;
		private LabelControl lblPlaceholders;
		private LabelControl lblPattern;
		private LabelControl lblFormat;
		private SimpleButton btnAdd;
		private PanelControl pnlPatternBorder;
		private TextEdit teSample;
		private SimpleButton btnAddAll;
		bool IsPointPattern {
			get {
				switch (selectedPlaceholder.Placeholder) {
					case ToolTipPatternUtils.SeriesNamePattern:
						return false;
					case ToolTipPatternUtils.StackedGroupPattern:
						return false;
					default:
						return true;
				}
			}
		}
		string CurrentPlaceholderDescription { get { return (string)lbPlaceholders.SelectedItem; } }
		public string Pattern { get { return MergeLinesToPattern(tbPattern.Lines); } }
		PatternEditorForm() {
			InitializeComponent();
		}
		public PatternEditorForm(string pattern, string[] placeholders, IPatternValuesSource patternSource) : this() {
			tbPattern.Lines = String.IsNullOrEmpty(pattern) ? null : SplitPatternToLines((string)pattern.Clone());
			this.availablePlaceholders = placeholders;
			this.patternSource = patternSource;
			FillStandardItemsListBox();
			InitializeFormatItems();
			UpdatePlaceholdersInfo();
			UpdateSample();
		}
		string[] SplitPatternToLines(string pattern) {
			return pattern.Split('\n');
		}
		string MergeLinesToPattern(string[] lines) {
			string pattern = "";
			for (int i = 0; i < lines.Length; i++)
				pattern = pattern + lines[i] + (i != lines.Length - 1 ? "\n" : "");
			return pattern;
		}
		void FillStandardItemsListBox() {
			foreach (string placeholder in availablePlaceholders)
				lbPlaceholders.Items.Add(PatternEditorUtils.GetDescriptionForPatternPlaceholder(placeholder));
		}
		void InitializeFormatItems() {
			XmlDataLoader xmlDataLoader = new XmlDataLoader();
			cbFormat.Properties.Items.Clear();
			categoryItems = xmlDataLoader.CreateCategoryItems();
			categoryItems = FilterCategoryItems(categoryItems);
			foreach(CategoryItem item in categoryItems)
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
			if(placeholderSelected != previousPlaceholderSelected)
				placeholderSelectionChanged = true;
			UpdateSample();
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
		void AddFormat() {
			supressCarretValidation = true;
			if (lbStandardTypes.SelectedIndex >= 0 && selectedPlaceholder.Format != lbStandardTypes.SelectedItem.ToString()) {
				tbPattern.SelectionStart = selectedPlaceholder.PlaceholderStart;
				tbPattern.SelectionLength = selectedPlaceholder.PlaceholderLength;
				tbPattern.SelectedText = selectedPlaceholder.Placeholder + ":" + lbStandardTypes.SelectedItem;
			}
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
		bool ValidatePattern() {
			foreach (PlaceholderInfo info in placeholdersInfo) {
				bool isCorrect = false;
				foreach (string checkingPlaceholder in availablePlaceholders)
					if (info.Placeholder == checkingPlaceholder) {
						isCorrect = true;
						break;
					}
				if (!isCorrect) {
					tbPattern.SelectionStart = info.PlaceholderStart;
					XtraMessageBox.Show(ChartLocalizer.GetString(ChartStringId.InvalidPlaceholder) + info.Placeholder, ChartLocalizer.GetString(ChartStringId.ErrorTitle),
						MessageBoxButtons.OK, MessageBoxIcon.Error);
					return false;
				}
			}
			return true;
		}
		void UpdateSample() {
			string sampleText = ChartLocalizer.GetString(ChartStringId.PatternEditorPreviewCaption);
			if (placeholderSelected)
				sampleText = PatternEditorUtils.GetPatternText("{" + selectedPlaceholder.PlaceholderWithFormat + "}", patternSource);
			teSample.Text = sampleText;
			teSample.Enabled = placeholderSelected;
		}
		#region Windows Form Designer generated code
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PatternEditorForm));
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.lbPlaceholders = new DevExpress.XtraEditors.ListBoxControl();
			this.propertyGrid = new DevExpress.Utils.Frames.PropertyGridEx();
			this.tbPattern = new System.Windows.Forms.RichTextBox();
			this.lbStandardTypes = new DevExpress.XtraEditors.ListBoxControl();
			this.cbFormat = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblPlaceholders = new DevExpress.XtraEditors.LabelControl();
			this.lblPattern = new DevExpress.XtraEditors.LabelControl();
			this.lblFormat = new DevExpress.XtraEditors.LabelControl();
			this.btnAdd = new DevExpress.XtraEditors.SimpleButton();
			this.btnAddAll = new DevExpress.XtraEditors.SimpleButton();
			this.pnlPatternBorder = new DevExpress.XtraEditors.PanelControl();
			this.teSample = new DevExpress.XtraEditors.TextEdit();
			((System.ComponentModel.ISupportInitialize)(this.lbPlaceholders)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lbStandardTypes)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbFormat.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlPatternBorder)).BeginInit();
			this.pnlPatternBorder.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.teSample.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.lbPlaceholders, "lbPlaceholders");
			this.lbPlaceholders.HotTrackSelectMode = DevExpress.XtraEditors.HotTrackSelectMode.SelectItemOnClick;
			this.lbPlaceholders.Name = "lbPlaceholders";
			this.lbPlaceholders.SelectedIndexChanged += new System.EventHandler(this.lbPlaceholders_SelectedIndexChanged);
			this.lbPlaceholders.DoubleClick += new System.EventHandler(this.lbPlaceholders_DoubleClick);
			this.propertyGrid.CommandsActiveLinkColor = System.Drawing.SystemColors.ActiveCaption;
			this.propertyGrid.CommandsDisabledLinkColor = System.Drawing.SystemColors.ControlDark;
			this.propertyGrid.CommandsLinkColor = System.Drawing.SystemColors.ActiveCaption;
			resources.ApplyResources(this.propertyGrid, "propertyGrid");
			this.propertyGrid.DrawFlat = true;
			this.propertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.ToolbarVisible = false;
			this.tbPattern.AcceptsTab = true;
			resources.ApplyResources(this.tbPattern, "tbPattern");
			this.tbPattern.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.tbPattern.Name = "tbPattern";
			this.tbPattern.SelectionChanged += new System.EventHandler(this.tbPattern_SelectionChanged);
			this.tbPattern.TextChanged += new System.EventHandler(this.tbPattern_TextChanged);
			resources.ApplyResources(this.lbStandardTypes, "lbStandardTypes");
			this.lbStandardTypes.HotTrackSelectMode = DevExpress.XtraEditors.HotTrackSelectMode.SelectItemOnClick;
			this.lbStandardTypes.Items.AddRange(new object[] {
			resources.GetString("lbStandardTypes.Items"),
			resources.GetString("lbStandardTypes.Items1"),
			resources.GetString("lbStandardTypes.Items2"),
			resources.GetString("lbStandardTypes.Items3"),
			resources.GetString("lbStandardTypes.Items4"),
			resources.GetString("lbStandardTypes.Items5"),
			resources.GetString("lbStandardTypes.Items6"),
			resources.GetString("lbStandardTypes.Items7"),
			resources.GetString("lbStandardTypes.Items8"),
			resources.GetString("lbStandardTypes.Items9"),
			resources.GetString("lbStandardTypes.Items10")});
			this.lbStandardTypes.Name = "lbStandardTypes";
			this.lbStandardTypes.SelectedIndexChanged += new System.EventHandler(this.lbStandardTypes_SelectedIndexChanged);
			this.lbStandardTypes.Click += new System.EventHandler(this.lbStandardTypes_Click);
			resources.ApplyResources(this.cbFormat, "cbFormat");
			this.cbFormat.Name = "cbFormat";
			this.cbFormat.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbFormat.Properties.Buttons"))))});
			this.cbFormat.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbFormat.SelectedIndexChanged += new System.EventHandler(this.cbFormat_SelectedIndexChanged);
			resources.ApplyResources(this.lblPlaceholders, "lblPlaceholders");
			this.lblPlaceholders.Cursor = System.Windows.Forms.Cursors.Default;
			this.lblPlaceholders.Name = "lblPlaceholders";
			resources.ApplyResources(this.lblPattern, "lblPattern");
			this.lblPattern.Cursor = System.Windows.Forms.Cursors.Default;
			this.lblPattern.Name = "lblPattern";
			resources.ApplyResources(this.lblFormat, "lblFormat");
			this.lblFormat.Cursor = System.Windows.Forms.Cursors.Default;
			this.lblFormat.Name = "lblFormat";
			this.btnAdd.Image = ((System.Drawing.Image)(resources.GetObject("btnAdd.Image")));
			this.btnAdd.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			resources.ApplyResources(this.btnAdd, "btnAdd");
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			this.btnAddAll.Image = ((System.Drawing.Image)(resources.GetObject("btnAddAll.Image")));
			this.btnAddAll.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			resources.ApplyResources(this.btnAddAll, "btnAddAll");
			this.btnAddAll.Name = "btnAddAll";
			this.btnAddAll.Click += new System.EventHandler(this.btnAddAll_Click);
			resources.ApplyResources(this.pnlPatternBorder, "pnlPatternBorder");
			this.pnlPatternBorder.Controls.Add(this.tbPattern);
			this.pnlPatternBorder.Name = "pnlPatternBorder";
			resources.ApplyResources(this.teSample, "teSample");
			this.teSample.Name = "teSample";
			this.teSample.Properties.Appearance.Options.UseTextOptions = true;
			this.teSample.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.teSample.Properties.AutoHeight = ((bool)(resources.GetObject("teSample.Properties.AutoHeight")));
			this.teSample.Properties.ReadOnly = true;
			this.AcceptButton = this.btnOk;
			this.CancelButton = this.btnCancel;
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.pnlPatternBorder);
			this.Controls.Add(this.cbFormat);
			this.Controls.Add(this.lbPlaceholders);
			this.Controls.Add(this.btnAddAll);
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.lblFormat);
			this.Controls.Add(this.lblPattern);
			this.Controls.Add(this.lblPlaceholders);
			this.Controls.Add(this.lbStandardTypes);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.teSample);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PatternEditorForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			((System.ComponentModel.ISupportInitialize)(this.lbPlaceholders)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lbStandardTypes)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbFormat.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlPatternBorder)).EndInit();
			this.pnlPatternBorder.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.teSample.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		void tbPattern_TextChanged(object sender, EventArgs e) {
			UpdatePlaceholdersInfo();
			if (!supressCarretValidation && tbPattern.SelectionLength == 0) {
				ValidateCarretPosition();
				if (!placeholderSelected && placeholderSelectionChanged)
						DeselectAll();
			}
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
		void lbPlaceholders_SelectedIndexChanged(object sender, EventArgs e) {
			tbPattern.Focus();
		}
		void lbPlaceholders_DoubleClick(object sender, EventArgs e) {
			AddPlaceholder();
		}
		void btnAdd_Click(object sender, EventArgs e) {
			AddPlaceholder();
		}
		void btnAddAll_Click(object sender, EventArgs e) {
			if (placeholderSelected)
				MoveCarretOutOfPlaceholder();
			foreach (string item in lbPlaceholders.Items)
				tbPattern.SelectedText = "{" + PatternEditorUtils.GetPatternPlaceholderForDescription(item) + "}";
			tbPattern.Focus();
		}
		void cbFormat_SelectedIndexChanged(object sender, EventArgs e) {
			lbStandardTypes.Items.Clear();
			lbStandardTypes.Items.AddRange(((CategoryItem)cbFormat.SelectedItem).FormatStrings);
		}
		void lbStandardTypes_SelectedIndexChanged(object sender, EventArgs e) {
			if (placeholderSelected) {
				AddFormat();
				ValidateCarretPosition();
			}
			tbPattern.Focus();
		}
		void lbStandardTypes_Click(object sender, EventArgs e) {
			if (!placeholderSelected)
				SelectNearestPlaceholder();
		}
		void btnOk_Click(object sender, EventArgs e) {
			if (!ValidatePattern()) {
				DialogResult = DialogResult.None;
				tbPattern.Focus();
			}
		}
	}
}
