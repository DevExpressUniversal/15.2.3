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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.Windows.Forms;
using DevExpress.Data.Mask;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors;
namespace DevExpress.XtraEditors.Design {
	public class EditMaskEditorForm : XtraForm {
		RepositoryItemTextEdit _repositoryItem;		
		private DevExpress.XtraEditors.ComboBoxEdit comboBoxAutoComplete;
		private DevExpress.XtraEditors.CheckEdit checkEditBeepOnError;
		private DevExpress.XtraEditors.CheckEdit checkEditIgnoreMaskBlank;
		private DevExpress.XtraEditors.TextEdit textEditPlaceHolder;
		private DevExpress.XtraEditors.CheckEdit checkEditSaveLiteral;
		private DevExpress.XtraEditors.CheckEdit checkEditShowPlaceHolders;
		private System.Windows.Forms.Label labelMaskTypeDescription;
		private System.Windows.Forms.Label labelMaskDescription;
		private System.Windows.Forms.Label labelAutocompleteMode;
		private System.Windows.Forms.Label label2;
		int eventsLocked = 0;
		void LockEvents() {
			eventsLocked++;
		}
		void UnlockEvents() {
			eventsLocked--;
		}
		bool IsEventsLocked { get { return eventsLocked != 0; } }
		public RepositoryItemTextEdit RepositoryItem {
			get {
				return _repositoryItem;
			}
			set {
				_repositoryItem = value;
			}
		}
		public event EditMaskEditorComponentChangedHandler ComponentChanged;
		protected virtual void OnComponentChanged(EditMaskEditorComponentChangedEventArgs e) {
			if(ComponentChanged != null) {
				ComponentChanged(this, e);
			}
		}
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Label maskTypeLabel;
		private DevExpress.XtraEditors.ComboBoxEdit maskTypeCombobox;
		private System.Windows.Forms.Label label1;
		private DevExpress.XtraEditors.SimpleButton btnOk;
		private DevExpress.XtraEditors.SimpleButton btnCancel;
		private System.Windows.Forms.Label lblList;
		private DevExpress.XtraEditors.ListBoxControl listMasks;
		private DevExpress.XtraEditors.TextEdit editEditMask;
		private DevExpress.XtraEditors.TextEdit editTest;
		static ArrayList maskTypesArray;
		static AutoCompleteType[] autocompletetypesArray = (AutoCompleteType[])Enum.GetValues(typeof(AutoCompleteType));
		struct PredefinedMask {
			public string ShortDescription;
			public string FullDescription;
			public string EditMask;
			public PredefinedMask(string shortDescription, string fullDescription, string editMask) {
				this.ShortDescription = shortDescription;
				this.FullDescription = fullDescription;
				this.EditMask = editMask;
			}
			public override string ToString() {
				return ShortDescription;
			}
		}
		static PredefinedMask[] simpleMasksSet = new PredefinedMask[] {
																		  new PredefinedMask(Properties.Resources.Phone, "(213) 144-1756", @"(999) 000-0000"),
																		  new PredefinedMask(Properties.Resources.Extension, "15023", @"99999"),
																		  new PredefinedMask(Properties.Resources.SocialSecurity, "555-55-5555", @"000-00-0000"),
																		  new PredefinedMask(Properties.Resources.ShortZipCode, "90628", @"00000"),
																		  new PredefinedMask(Properties.Resources.LongZipCode, "90628-0000", @"00000-9999"),
																		  new PredefinedMask(Properties.Resources.Date, "03/24/99", @"99/99/00"),
																		  new PredefinedMask(Properties.Resources.LongTime, "04:15:34PM", @"90:00:00>LL"),
																		  new PredefinedMask(Properties.Resources.ShortTime, "21:45", @"90:00"),
		};
		static PredefinedMask[] regularMasksSet = new PredefinedMask[] {
																		   new PredefinedMask(Properties.Resources.Phone, "(213) 144-1756", @"(\d?\d?\d?) \d\d\d-\d\d\d\d"),
																		   new PredefinedMask(Properties.Resources.Extension, "15023", @"\d?\d?\d?\d?\d?"),
																		   new PredefinedMask(Properties.Resources.SocialSecurity, "555-55-5555", @"\d\d\d-\d\d-\d\d\d\d"),
																		   new PredefinedMask(Properties.Resources.ShortZipCode, "90628", @"\d\d\d\d\d"),
																		   new PredefinedMask(Properties.Resources.LongZipCode, "90628-0000", @"\d\d\d\d\d-\d?\d?\d?\d?"),
																		   new PredefinedMask(Properties.Resources.Date, "03/24/99", @"\d?\d?/\d?\d?/\d\d"),
																		   new PredefinedMask(Properties.Resources.LongTime, "04:15:34PM", @"\d?\d:\d\d:\d\d>[AP]M"),
																		   new PredefinedMask(Properties.Resources.ShortTime, "21:45", @"\d?\d:\d\d"),
		};
		static PredefinedMask[] regExMasksSet = new PredefinedMask[] {
																		 new PredefinedMask(Properties.Resources.TimeOfDay, "The 24 hour day time:\n15:25\n2:05\n03:57", @"(0?\d|1\d|2[0-3])\:[0-5]\d"),
																		 new PredefinedMask(Properties.Resources.TimeOfDateWithSeconds, "The 24 hour day time with seconds:\n12:45:10\n3:00:01", @"(0?\d|1\d|2[0-3]):[0-5]\d:[0-5]\d"),
																		 new PredefinedMask(Properties.Resources.TimeOfDayAMPM, "The 12 hour day time:\n1:35PM\n12:45AM", @"(0?[1-9]|1[012]):[0-5]\d(AM|PM)"),
																		 new PredefinedMask(Properties.Resources.TimeOfDateWithSecondsAMPM, "The 12 hour day time with seconds:\n10:03:10AM\n03:00:01PM", @"(0?[1-9]|1[012]):[0-5]\d:[0-5]\d(AM|PM)"),
																		 new PredefinedMask(Properties.Resources.Date, "The MM/dd/yy or MM/dd/yyyy date with year from 1000 to 3999:\n3/12/99\n06/25/1800", @"(0?[1-9]|1[012])/([012]?[1-9]|[123]0|31)/([123][0-9])?[0-9][0-9]"),
																		 new PredefinedMask(Properties.Resources.TelephoneNumber, "The telephone number with or without city code:\n(345) 234-12-07\n(210) 7-17-81\n26-32-22", @"(\(\d\d\d\) )?\d{1,3}-\d\d-\d\d"),
																		 new PredefinedMask(Properties.Resources.Extension, "15450", @"\d{0,5}"),
																		 new PredefinedMask(Properties.Resources.SocialSecurity, "555-55-5555", @"\d\d\d-\d\d-\d\d\d\d"),
																		 new PredefinedMask(Properties.Resources.ShortZipCode, "11200", @"\d\d\d\d\d"),
																		 new PredefinedMask(Properties.Resources.LongZipCode, "11200-0000", @"\d\d\d\d\d-\d\d\d\d"),
																		 new PredefinedMask(Properties.Resources.DecimalNumber, "Any decimal number", @"\d+"),
																		 new PredefinedMask(Properties.Resources.HexadecimalNumber, "Any hexadecimal number", @"[0-9A-Fa-f]+"),
																		 new PredefinedMask(Properties.Resources.OctalNumber, "Any octal number", @"[0-7]+"),
																		 new PredefinedMask(Properties.Resources.BinaryNumber, "Any binary number", @"[01]+"),
																		 new PredefinedMask(Properties.Resources.YesNo, "Yes\nNo", @"Yes|No"),
																		 new PredefinedMask(Properties.Resources.TrueFalse, "True\nFalse", @"True|False"),
																		 new PredefinedMask(Properties.Resources.AnySymbols, "Any symbols", @".+"),
																		 new PredefinedMask(Properties.Resources.LatinLettersOnly, "Any letters of the latin alphabet", @"[a-zA-Z]+"),
																		 new PredefinedMask(Properties.Resources.LettersOnly, "Any letters", @"\p{L}+"),
																		 new PredefinedMask(Properties.Resources.UppercaseLetters, "Any uppercase letters", @"\p{Lu}+"),
																		 new PredefinedMask(Properties.Resources.LowercaseLetters, "Any lowercase letters", @"\p{Ll}+"),
		};
		static PredefinedMask[] dateTimeMasksSet = new PredefinedMask[] {
																			new PredefinedMask(Properties.Resources.ShortDate, DateTime.Now.ToString(@"d"), @"d"),
																			new PredefinedMask(Properties.Resources.LongDate, DateTime.Now.ToString(@"D"), @"D"),
																			new PredefinedMask(Properties.Resources.ShortTime, DateTime.Now.ToString(@"t"), @"t"),
																			new PredefinedMask(Properties.Resources.LongTime, DateTime.Now.ToString(@"T"), @"T"),
																			new PredefinedMask(Properties.Resources.FullDateTimeShortTime, DateTime.Now.ToString(@"f"), @"f"),
																			new PredefinedMask(Properties.Resources.FullDateTimeLongTime, DateTime.Now.ToString(@"F"), @"F"),
																			new PredefinedMask(Properties.Resources.GeneralDateTimeShortTime, DateTime.Now.ToString(@"g"), @"g"),
																			new PredefinedMask(Properties.Resources.GeneralDateTimeLongTime, DateTime.Now.ToString(@"G"), @"G"),
																			new PredefinedMask(Properties.Resources.MonthDay, DateTime.Now.ToString(@"m"), @"m"),
																			new PredefinedMask(Properties.Resources.RFC1123, DateTime.Now.ToString(@"r"), @"r"),
																			new PredefinedMask(Properties.Resources.SortableDateTime, "Conforms to ISO 8601\n" + DateTime.Now.ToString(@"s"), @"s"),
																			new PredefinedMask(Properties.Resources.UniversalSortableDateTime, DateTime.Now.ToString(@"u"), @"u"),
																			new PredefinedMask(Properties.Resources.YearMonth, DateTime.Now.ToString(@"y"), @"y"),
		};
		static string PercentMode1Explanation =  Properties.Resources.PercentMode1Explanation;
		static string PercentMode2Explanation = Properties.Resources.PercentMode2Explanation;
		static PredefinedMask[] numericMasksSet = new PredefinedMask[] {
																		   new PredefinedMask(Properties.Resources.Currency, 123456789.123456789m.ToString(@"c") + "\n" + (-12.345678m).ToString(@"c"), @"c"),
																		   new PredefinedMask(Properties.Resources.Decimal, 123456789.ToString(@"d") + "\n" + (-12).ToString(@"d"), @"d"),
																		   new PredefinedMask(Properties.Resources.FixedPoint, 123456789.123456789m.ToString(@"f") + "\n" + (-12.345678m).ToString(@"f"), @"f"),
																		   new PredefinedMask(Properties.Resources.Number, 123456789.123456789m.ToString(@"n") + "\n" + (-12.345678m).ToString(@"n"), @"n"),
																		   new PredefinedMask(Properties.Resources.PercentMode1, PercentMode1Explanation + "\n" + 0.123456789m.ToString(@"p") + "\n" + (-1.23456789).ToString(@"p"), @"P"),
																		   new PredefinedMask(Properties.Resources.PercentMode2, PercentMode2Explanation + "\n" + 0.123456789m.ToString(@"p") + "\n" + (-1.23456789).ToString(@"p"), @"p"),
																		   new PredefinedMask(Properties.Resources.CurrencyInteger, 123456789.123456789m.ToString(@"c0") + "\n" + (-12.345678m).ToString(@"c0"), @"c0"),
																		   new PredefinedMask(Properties.Resources.FixedPointInteger, 123456789.123456789m.ToString(@"f0") + "\n" + (-12.345678m).ToString(@"f0"), @"f0"),
																		   new PredefinedMask(Properties.Resources.NumberInteger, 123456789.123456789m.ToString(@"n0") + "\n" + (-12.345678m).ToString(@"n0"), @"n0"),
																		   new PredefinedMask(Properties.Resources.PercentMode1Integer, PercentMode1Explanation + "\n" + 0.123456789m.ToString(@"p0") + "\n" + (-1.23456789).ToString(@"p0"), @"P0"),
																		   new PredefinedMask(Properties.Resources.PercentMode2Integer, PercentMode2Explanation + "\n" + 0.123456789m.ToString(@"p0") + "\n" + (-1.23456789).ToString(@"p0"), @"p0"),
																		   new PredefinedMask(Properties.Resources.Currency2DecimalPlaces, 123456789.123456789m.ToString(@"c2") + "\n" + (-12.345678m).ToString(@"c2"), @"c2"),
																		   new PredefinedMask(Properties.Resources.FixedPoint2DecimalPlaces, 123456789.123456789m.ToString(@"f2") + "\n" + (-12.345678m).ToString(@"f2"), @"f2"),
																		   new PredefinedMask(Properties.Resources.Number2DecimalPlaces, 123456789.123456789m.ToString(@"n2") + "\n" + (-12.345678m).ToString(@"n2"), @"n2"),
																		   new PredefinedMask(Properties.Resources.PercentMode1_2DecimalPlaces, PercentMode1Explanation + "\n" + 0.123456789m.ToString(@"p2") + "\n" + (-1.23456789).ToString(@"p2"), @"P2"),
																		   new PredefinedMask(Properties.Resources.PercentMode1_2DecimalPlaces, PercentMode2Explanation + "\n" + 0.123456789m.ToString(@"p2") + "\n" + (-1.23456789).ToString(@"p2"), @"p2"),
																		   new PredefinedMask(Properties.Resources.Currency3DecimalPlaces, 123456789.123456789m.ToString(@"c3") + "\n" + (-12.345678m).ToString(@"c3"), @"c3"),
																		   new PredefinedMask(Properties.Resources.FixedPoint3DecimalPlaces, 123456789.123456789m.ToString(@"f3") + "\n" + (-12.345678m).ToString(@"f3"), @"f3"),
																		   new PredefinedMask(Properties.Resources.Number3DecimalPlaces, 123456789.123456789m.ToString(@"n3") + "\n" + (-12.345678m).ToString(@"n3"), @"n3"),
																		   new PredefinedMask(Properties.Resources.PercentMode1_3DecimalPlaces, PercentMode1Explanation + "\n" + 0.123456789m.ToString(@"p3") + "\n" + (-1.23456789).ToString(@"p3"), @"P3"),
																		   new PredefinedMask(Properties.Resources.PercentMode2_3DecimalPlaces, PercentMode2Explanation + "\n" + 0.123456789m.ToString(@"p3") + "\n" + (-1.23456789).ToString(@"p3"), @"p3"),
					};
		static IDictionary allMasksSet;
		static EditMaskEditorForm() {
			allMasksSet = new System.Collections.Specialized.ListDictionary();
			allMasksSet.Add(MaskType.Simple, simpleMasksSet);
			allMasksSet.Add(MaskType.Regular, regularMasksSet);
			allMasksSet.Add(MaskType.RegEx, regExMasksSet);
			allMasksSet.Add(MaskType.DateTime, dateTimeMasksSet);
			allMasksSet.Add(MaskType.DateTimeAdvancingCaret, dateTimeMasksSet);
			allMasksSet.Add(MaskType.Numeric, numericMasksSet);
			Array allMaskTypes = (MaskType[])Enum.GetValues(typeof(MaskType));
			maskTypesArray = new ArrayList(allMaskTypes.Length);
			foreach(MaskType maskType in allMaskTypes) {
				if(maskType != MaskType.Custom)
					maskTypesArray.Add(maskType);
			}
		}
		public EditMaskEditorForm() {
			this.LookAndFeel.SetSkinStyle(DevExpress.Skins.SkinRegistrator.DesignTimeSkinName);
			InitializeComponent();
			this.textEditPlaceHolder.Properties.Mask.AutoComplete = AutoCompleteType.Optimistic;
			this.textEditPlaceHolder.Properties.Mask.MaskType = MaskType.RegEx;
			foreach(MaskType maskType in maskTypesArray)
				maskTypeCombobox.Properties.Items.Add(maskType.ToString());
			foreach(AutoCompleteType autocompleteType in autocompletetypesArray) {
				comboBoxAutoComplete.Properties.Items.Add(autocompleteType.ToString());
			}
		}
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		MaskType GetSelectedMaskType() {
			return (MaskType)maskTypesArray[maskTypeCombobox.SelectedIndex];
		}
		AutoCompleteType GetSelectedAutocomplete() {
			return (AutoCompleteType)autocompletetypesArray[comboBoxAutoComplete.SelectedIndex];
		}
		static string GetMaskTypeDescription(MaskType maskType) {
			switch(maskType) {
				case MaskType.None: return Properties.Resources.MaskTypeNoneDescription;
				case MaskType.DateTime:
				case MaskType.DateTimeAdvancingCaret:
					return Properties.Resources.MaskTypeDateTimeDescription;
				case MaskType.Numeric: return Properties.Resources.MaskTypeNumericDescription;
				case MaskType.RegEx: return Properties.Resources.MaskTypeRegExDescription;
				case MaskType.Regular: return Properties.Resources.MaskTypeRegularDescription;
				case MaskType.Simple: return Properties.Resources.MaskTypeSimpleDescription;
				default: return string.Empty;
			}
		}
		void DoOnMaskTypeChanged() {
			MaskType selectedType = GetSelectedMaskType();
			labelMaskTypeDescription.Text = GetMaskTypeDescription(selectedType);
			listMasks.BeginUpdate();
			LockEvents();
			try {
				listMasks.DataSource = allMasksSet[selectedType];
				listMasks.SelectedIndex = -1;
				labelMaskDescription.Text = string.Empty;
				switch(selectedType) {
					case MaskType.Simple:
					case MaskType.Regular:
					case MaskType.RegEx:
						checkEditIgnoreMaskBlank.Enabled = true;
						break;
					default:
						checkEditIgnoreMaskBlank.Enabled = false;
						break;
				}
				switch(selectedType) {
					case MaskType.Simple:
					case MaskType.Regular:
						checkEditSaveLiteral.Enabled = true;
						break;
					default:
						checkEditSaveLiteral.Enabled = false;
						break;
				}
				switch(selectedType) {
					case MaskType.RegEx:
						checkEditShowPlaceHolders.Enabled = true;
						break;
					default:
						checkEditShowPlaceHolders.Enabled = false;
						break;
				}
				switch(selectedType) {
					case MaskType.Simple:
					case MaskType.Regular:
					case MaskType.RegEx:
						textEditPlaceHolder.Enabled = true;
						break;
					default:
						textEditPlaceHolder.Enabled = false;
						break;
				}
				switch(selectedType) {
					case MaskType.RegEx:
						comboBoxAutoComplete.Enabled = true;
						break;
					default:
						comboBoxAutoComplete.Enabled = false;
						break;
				}
			} finally {
				listMasks.EndUpdate();
				UnlockEvents();
			}
			editEditMask.SelectAll();
			DoUpdateSample();
		}
		void DoUpdateSample() {
			if(IsEventsLocked)
				return;
			editTest.Properties.Assign(RepositoryItem);
			MaskProperties newProps = GetResultMask();
			MaskManager manager = null;
			try {
				if(newProps.MaskType != MaskType.None) {
					manager = newProps.CreateDefaultMaskManager();
					if(manager == null)
						throw new NotImplementedException(string.Format(CultureInfo.InvariantCulture, MaskExceptionsTexts.CreateManagerReturnsNull, newProps.MaskType, newProps.EditMask));
				}
			} catch(Exception e) {
				editTest.Properties.Mask.MaskType = MaskType.None;
				editTest.Properties.ReadOnly = true;
				editTest.EditValue = e.Message;
				editTest.ErrorText = e.Message;
				btnOk.Enabled = false;
				return;
			}
			editTest.Properties.Mask.Assign(newProps);
			editTest.Properties.ReadOnly = false;
			object editValue = null;
			if(RepositoryItem.OwnerEdit != null)
				editValue = RepositoryItem.OwnerEdit.EditValue;
			if(editValue == null) {
				switch(newProps.MaskType) {
					case MaskType.DateTime:
					case MaskType.DateTimeAdvancingCaret:
						editValue = DateTime.Now;
						break;
					case MaskType.Numeric:
						editValue = Decimal.Zero;
						break;
					default:
						editValue = string.Empty;
						break;
				}
			}
			if(manager == null) {
				editTest.EditValue = editValue;
			} else {
				manager.SetInitialEditValue(editValue);
				editTest.EditValue = manager.GetCurrentEditValue();
			}
			editTest.ErrorText = string.Empty;
			btnOk.Enabled = true;
			return;
		}
		MaskProperties GetResultMask() {
			MaskProperties resultMask = new MaskProperties();
			resultMask.Assign(RepositoryItem.Mask);	
			resultMask.MaskType = GetSelectedMaskType();
			resultMask.EditMask = editEditMask.Text;
			resultMask.BeepOnError = checkEditBeepOnError.Checked;
			resultMask.IgnoreMaskBlank = checkEditIgnoreMaskBlank.Checked;
			resultMask.SaveLiteral = checkEditSaveLiteral.Checked;
			resultMask.ShowPlaceHolders = checkEditShowPlaceHolders.Checked;
			resultMask.PlaceHolder = (textEditPlaceHolder.Text.Length == 0) ? '_' : textEditPlaceHolder.Text[0];
			resultMask.AutoComplete = GetSelectedAutocomplete();
			return resultMask;
		}
		void FireChanged() {
			OnComponentChanged(new EditMaskEditorComponentChangedEventArgs(RepositoryItem));
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditMaskEditorForm));
			this.maskTypeLabel = new System.Windows.Forms.Label();
			this.maskTypeCombobox = new DevExpress.XtraEditors.ComboBoxEdit();
			this.label1 = new System.Windows.Forms.Label();
			this.editEditMask = new DevExpress.XtraEditors.TextEdit();
			this.editTest = new DevExpress.XtraEditors.TextEdit();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.lblList = new System.Windows.Forms.Label();
			this.listMasks = new DevExpress.XtraEditors.ListBoxControl();
			this.labelAutocompleteMode = new System.Windows.Forms.Label();
			this.comboBoxAutoComplete = new DevExpress.XtraEditors.ComboBoxEdit();
			this.checkEditBeepOnError = new DevExpress.XtraEditors.CheckEdit();
			this.checkEditIgnoreMaskBlank = new DevExpress.XtraEditors.CheckEdit();
			this.textEditPlaceHolder = new DevExpress.XtraEditors.TextEdit();
			this.checkEditSaveLiteral = new DevExpress.XtraEditors.CheckEdit();
			this.checkEditShowPlaceHolders = new DevExpress.XtraEditors.CheckEdit();
			this.labelMaskTypeDescription = new System.Windows.Forms.Label();
			this.labelMaskDescription = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.maskTypeCombobox.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.editEditMask.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.editTest.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.listMasks)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.comboBoxAutoComplete.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.checkEditBeepOnError.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.checkEditIgnoreMaskBlank.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.textEditPlaceHolder.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.checkEditSaveLiteral.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.checkEditShowPlaceHolders.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.maskTypeLabel, "maskTypeLabel");
			this.maskTypeLabel.Name = "maskTypeLabel";
			this.maskTypeCombobox.EditValue = "";
			resources.ApplyResources(this.maskTypeCombobox, "maskTypeCombobox");
			this.maskTypeCombobox.Name = "maskTypeCombobox";
			this.maskTypeCombobox.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("maskTypeCombobox.Properties.Buttons"))))});
			this.maskTypeCombobox.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.maskTypeCombobox.SelectedIndexChanged += new System.EventHandler(this.maskTypeCombobox_SelectedIndexChanged);
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			this.editEditMask.EditValue = "";
			resources.ApplyResources(this.editEditMask, "editEditMask");
			this.editEditMask.Name = "editEditMask";
			this.editEditMask.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Buffered;
			this.editEditMask.EditValueChanged += new System.EventHandler(this.editEditMask_EditValueChanged);
			this.editTest.EditValue = "";
			resources.ApplyResources(this.editTest, "editTest");
			this.editTest.Name = "editTest";
			this.editTest.InvalidValue += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.editTest_InvalidValue);
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.lblList, "lblList");
			this.lblList.Name = "lblList";
			this.listMasks.ItemHeight = 16;
			resources.ApplyResources(this.listMasks, "listMasks");
			this.listMasks.Name = "listMasks";
			this.listMasks.SelectedIndexChanged += new System.EventHandler(this.listMasks_SelectedIndexChanged);
			resources.ApplyResources(this.labelAutocompleteMode, "labelAutocompleteMode");
			this.labelAutocompleteMode.Name = "labelAutocompleteMode";
			resources.ApplyResources(this.comboBoxAutoComplete, "comboBoxAutoComplete");
			this.comboBoxAutoComplete.Name = "comboBoxAutoComplete";
			this.comboBoxAutoComplete.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("comboBoxAutoComplete.Properties.Buttons"))))});
			this.comboBoxAutoComplete.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.comboBoxAutoComplete.SelectedIndexChanged += new System.EventHandler(this.comboBoxAutoComplete_SelectedIndexChanged);
			resources.ApplyResources(this.checkEditBeepOnError, "checkEditBeepOnError");
			this.checkEditBeepOnError.Name = "checkEditBeepOnError";
			this.checkEditBeepOnError.Properties.Caption = resources.GetString("checkEditBeepOnError.Properties.Caption");
			this.checkEditBeepOnError.CheckedChanged += new System.EventHandler(this.checkEditBeepOnError_CheckedChanged);
			resources.ApplyResources(this.checkEditIgnoreMaskBlank, "checkEditIgnoreMaskBlank");
			this.checkEditIgnoreMaskBlank.Name = "checkEditIgnoreMaskBlank";
			this.checkEditIgnoreMaskBlank.Properties.Caption = resources.GetString("checkEditIgnoreMaskBlank.Properties.Caption");
			this.checkEditIgnoreMaskBlank.CheckedChanged += new System.EventHandler(this.checkEditIgnoreMaskBlank_CheckedChanged);
			resources.ApplyResources(this.textEditPlaceHolder, "textEditPlaceHolder");
			this.textEditPlaceHolder.Name = "textEditPlaceHolder";
			this.textEditPlaceHolder.Properties.Mask.BeepOnError = ((bool)(resources.GetObject("textEditPlaceHolder.Properties.Mask.BeepOnError")));
			this.textEditPlaceHolder.Properties.Mask.EditMask = resources.GetString("textEditPlaceHolder.Properties.Mask.EditMask");
			this.textEditPlaceHolder.Properties.Mask.ShowPlaceHolders = ((bool)(resources.GetObject("textEditPlaceHolder.Properties.Mask.ShowPlaceHolders")));
			this.textEditPlaceHolder.EditValueChanged += new System.EventHandler(this.textEditPlaceHolder_EditValueChanged);
			resources.ApplyResources(this.checkEditSaveLiteral, "checkEditSaveLiteral");
			this.checkEditSaveLiteral.Name = "checkEditSaveLiteral";
			this.checkEditSaveLiteral.Properties.Caption = resources.GetString("checkEditSaveLiteral.Properties.Caption");
			this.checkEditSaveLiteral.CheckedChanged += new System.EventHandler(this.checkEditSaveLiteral_CheckedChanged);
			resources.ApplyResources(this.checkEditShowPlaceHolders, "checkEditShowPlaceHolders");
			this.checkEditShowPlaceHolders.Name = "checkEditShowPlaceHolders";
			this.checkEditShowPlaceHolders.Properties.Caption = resources.GetString("checkEditShowPlaceHolders.Properties.Caption");
			this.checkEditShowPlaceHolders.CheckedChanged += new System.EventHandler(this.checkEditShowPlaceHolders_CheckedChanged);
			resources.ApplyResources(this.labelMaskTypeDescription, "labelMaskTypeDescription");
			this.labelMaskTypeDescription.Name = "labelMaskTypeDescription";
			resources.ApplyResources(this.labelMaskDescription, "labelMaskDescription");
			this.labelMaskDescription.Name = "labelMaskDescription";
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.editTest);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.labelMaskDescription);
			this.Controls.Add(this.labelMaskTypeDescription);
			this.Controls.Add(this.listMasks);
			this.Controls.Add(this.lblList);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.editEditMask);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.maskTypeCombobox);
			this.Controls.Add(this.maskTypeLabel);
			this.Controls.Add(this.checkEditIgnoreMaskBlank);
			this.Controls.Add(this.checkEditBeepOnError);
			this.Controls.Add(this.checkEditSaveLiteral);
			this.Controls.Add(this.comboBoxAutoComplete);
			this.Controls.Add(this.textEditPlaceHolder);
			this.Controls.Add(this.checkEditShowPlaceHolders);
			this.Controls.Add(this.labelAutocompleteMode);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "EditMaskEditorForm";
			this.ShowInTaskbar = false;
			this.Closed += new System.EventHandler(this.EditMaskEditorForm_Closed);
			this.VisibleChanged += new System.EventHandler(this.EditMaskEditorForm_VisibleChanged);
			((System.ComponentModel.ISupportInitialize)(this.maskTypeCombobox.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.editEditMask.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.editTest.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.listMasks)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.comboBoxAutoComplete.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.checkEditBeepOnError.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.checkEditIgnoreMaskBlank.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.textEditPlaceHolder.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.checkEditSaveLiteral.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.checkEditShowPlaceHolders.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private void maskTypeCombobox_SelectedIndexChanged(object sender, System.EventArgs e) {
			DoOnMaskTypeChanged();
		}
		private void EditMaskEditorForm_VisibleChanged(object sender, System.EventArgs e) {
			if(this.Visible) {
				LockEvents();
				try {
					if(RepositoryItem != null) {
						int masktypeIndex = maskTypesArray.IndexOf(RepositoryItem.Mask.MaskType);
						if(masktypeIndex < 0)
							masktypeIndex = 0;
						maskTypeCombobox.SelectedIndex = masktypeIndex;
						editEditMask.Text = RepositoryItem.Mask.EditMask;
						checkEditBeepOnError.Checked = RepositoryItem.Mask.BeepOnError;
						checkEditIgnoreMaskBlank.Checked = RepositoryItem.Mask.IgnoreMaskBlank;
						checkEditSaveLiteral.Checked = RepositoryItem.Mask.SaveLiteral;
						checkEditShowPlaceHolders.Checked = RepositoryItem.Mask.ShowPlaceHolders;
						textEditPlaceHolder.Text = RepositoryItem.Mask.PlaceHolder.ToString();
						int autocompleteIndex = Array.IndexOf(autocompletetypesArray, RepositoryItem.Mask.AutoComplete);
						if(autocompleteIndex < 0)
							autocompleteIndex = 0;
						comboBoxAutoComplete.SelectedIndex = autocompleteIndex;
					}
				} finally {
					UnlockEvents();
				}
				DoOnMaskTypeChanged();
			}
		}
		private void editEditMask_EditValueChanged(object sender, System.EventArgs e) {
			DoUpdateSample();
		}
		private void listMasks_SelectedIndexChanged(object sender, System.EventArgs e) {
			if(listMasks.SelectedIndex >= 0 && !IsEventsLocked) {
				PredefinedMask mask = ((PredefinedMask[])listMasks.DataSource)[listMasks.SelectedIndex];
				editEditMask.Text = mask.EditMask;
				labelMaskDescription.Text = mask.FullDescription;
			}
			DoUpdateSample();
		}
		private void editTest_InvalidValue(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e) {
			e.ExceptionMode = ExceptionMode.Ignore;
			editTest.ErrorText = e.ErrorText;
		}
		private void EditMaskEditorForm_Closed(object sender, System.EventArgs e) {
			if(this.DialogResult == DialogResult.OK) {
				RepositoryItem.Mask.Assign(GetResultMask());
				FireChanged();
			}
		}
		private void comboBoxAutoComplete_SelectedIndexChanged(object sender, System.EventArgs e) {
			DoUpdateSample();
		}
		private void checkEditBeepOnError_CheckedChanged(object sender, System.EventArgs e) {
			DoUpdateSample();
		}
		private void checkEditIgnoreMaskBlank_CheckedChanged(object sender, System.EventArgs e) {
			DoUpdateSample();
		}
		private void checkEditSaveLiteral_CheckedChanged(object sender, System.EventArgs e) {
			DoUpdateSample();
		}
		private void checkEditShowPlaceHolders_CheckedChanged(object sender, System.EventArgs e) {
			DoUpdateSample();
		}
		private void textEditPlaceHolder_EditValueChanged(object sender, System.EventArgs e) {
			DoUpdateSample();
		}
	}
	public delegate void EditMaskEditorComponentChangedHandler(object sender, EditMaskEditorComponentChangedEventArgs e);
	public class EditMaskEditorComponentChangedEventArgs: EventArgs{
		object component;
		public object Component{ get { return component; } set { component = value; } }
		public EditMaskEditorComponentChangedEventArgs(object component){
			this.component = component;
		}
	}
}
