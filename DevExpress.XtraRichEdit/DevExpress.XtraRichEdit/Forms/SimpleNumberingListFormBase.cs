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
using System.ComponentModel;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Localization;
using System.Windows.Forms;
using DevExpress.Office.NumberConverters;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.SimpleNumberingListFormBase.edtNumberFormat")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.SimpleNumberingListFormBase.lblNumberFormat")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.SimpleNumberingListFormBase.lblNumberStyle")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.SimpleNumberingListFormBase.edtStart")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.SimpleNumberingListFormBase.lblStartAt")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.SimpleNumberingListFormBase.lblNumberPosition")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.SimpleNumberingListFormBase.edtDisplayFormat")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.SimpleNumberingListFormBase.edtNumberingAlignment")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.SimpleNumberingListFormBase.lblTextPosition")]
#endregion
namespace DevExpress.XtraRichEdit.Forms {
	[DXToolboxItem(false)]
	public partial class SimpleNumberingListFormBase : NumberingListFormsBase {
		#region Fields
		DXValidationProvider validationProvider;
		ConditionValidationRule rangeValidationRule;
		bool isCloseForm = true;
		DisplayFormatHelper displayFormatHelper;
		#endregion
		public SimpleNumberingListFormBase() { 
			InitializeComponent();
		}
		public SimpleNumberingListFormBase(ListLevelCollection<ListLevel> levels, int levelIndex, RichEditControl control, IFormOwner formOwner)
			: base(levels, levelIndex, control, formOwner) {
			InitializeComponent();
			InitializeValidationProvider();
			this.displayFormatHelper = new DisplayFormatHelper(edtNumberFormat, Controller.EditedLevels, levelIndex);
		}
		#region Properties
		public new SimpleNumberingListController Controller { get { return (SimpleNumberingListController)base.Controller; } }
		#endregion
		protected override BulletedListFormController CreateController(ListLevelCollection<ListLevel> sourceLevels) {
			return new SimpleNumberingListController(sourceLevels);
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			UpdateNumberFormat();
		}
		public override void ApplyChanges() {
			if (isCloseForm)
				base.ApplyChanges();
		}
		protected override void UpdateFormCore() {
			base.UpdateFormCore();
			this.edtDisplayFormat.DisplayFormat = Controller.Format;
			this.edtNumberingAlignment.NumberingAlignment = Controller.Alignment;
			this.edtStart.Value = Controller.Start;
			UpdateNumberFormat();
		}
		protected internal override void SubscribeControlsEvents() {
			base.SubscribeControlsEvents();
			this.edtDisplayFormat.SelectedIndexChanged += OnFormatValueChanged;
			this.edtNumberingAlignment.SelectedIndexChanged += OnAlignmentValueChanged;
			this.edtStart.ValueChanged += OnStartValueChanged;
			this.edtStart.TextChanged += OnStartTextChanged;
			this.edtNumberFormat.KeyDown += OnNumberFormatKeyDown;
			this.edtNumberFormat.ContentChanged += OnDisplayFormatChanged;
		}
		void OnNumberFormatKeyDown(object sender, KeyEventArgs e) {
			switch (e.KeyCode) {
				case Keys.Tab:
					ChangeFocus();
					break;
				case Keys.Enter:
					ApplyChanges();
					break;
			}
		}
		protected internal virtual void ChangeFocus() {
			btnFont.Focus();
		}
		protected internal override void UnsubscribeControlsEvents() {
			base.UnsubscribeControlsEvents();
			this.edtDisplayFormat.SelectedIndexChanged -= OnFormatValueChanged;
			this.edtNumberingAlignment.SelectedIndexChanged -= OnAlignmentValueChanged;
			this.edtStart.ValueChanged -= OnStartValueChanged;
			this.edtStart.TextChanged -= OnStartTextChanged;
			this.edtNumberFormat.KeyDown -= OnNumberFormatKeyDown;
			this.edtNumberFormat.ContentChanged -= OnDisplayFormatChanged;
		}
		protected internal void InitializeValidationProvider() {
			validationProvider = new DXValidationProvider();
			rangeValidationRule = new ConditionValidationRule();
			rangeValidationRule.ConditionOperator = ConditionOperator.Between;
			LanguageId languageId = CultureInfoToLanguageIdConverter.GetLanguageId(System.Threading.Thread.CurrentThread.CurrentUICulture);
			OrdinalBasedNumberConverter converter = GetOrdinalNumberConverter(Controller.Format, languageId);
			long minValue = converter.MinValue;
			long maxValue = converter.MaxValue;
			UpdateValidationRuleValues(minValue, maxValue);
			validationProvider.SetValidationRule(edtStart, rangeValidationRule);
			UpdateMinAndMaxValueStart(minValue, maxValue);			
		}
		protected internal virtual OrdinalBasedNumberConverter GetOrdinalNumberConverter(NumberingFormat format, LanguageId languageId) {
			return OrdinalBasedNumberConverter.CreateConverter(format, languageId);
		}
		protected internal virtual void UpdateMinAndMaxValueStart(long minValue, long maxValue) {
			edtStart.Properties.MinValue = minValue;
			edtStart.Properties.MaxValue = maxValue;
		}
		protected internal virtual void UpdateValidationRuleValues(long minValue, long maxValue) {
			rangeValidationRule.Value1 = minValue;
			rangeValidationRule.Value2 = maxValue;
			string errorTextFormat = XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_InvalidNumberingListStartAtValue);
			rangeValidationRule.ErrorText = String.Format(errorTextFormat, minValue, maxValue);
			validationProvider.SetValidationRule(edtStart, rangeValidationRule);
		}
		public virtual void OnFormatValueChanged(object sender, EventArgs e) {
			LanguageId languageId = CultureInfoToLanguageIdConverter.GetLanguageId(System.Threading.Thread.CurrentThread.CurrentUICulture);
			Controller.Format = edtDisplayFormat.DisplayFormat;
			OrdinalBasedNumberConverter converter = GetOrdinalNumberConverter(Controller.Format, languageId);
			long minValue = converter.MinValue;
			long maxValue = converter.MaxValue;
			UpdateValidationRuleValues(minValue, maxValue);
			int start = (int)edtStart.Value;
			UpdateMinAndMaxValueStart(minValue, maxValue);
			edtStart.Value = start;
			isCloseForm = validationProvider.Validate(edtStart);
			UpdateNumberFormat();
		}
		protected internal virtual void OnAlignmentValueChanged(object sender, EventArgs e) {
			Controller.Alignment = edtNumberingAlignment.NumberingAlignment;
		}
		protected internal virtual void OnStartValueChanged(object sender, EventArgs e) {
			Controller.Start = Convert.ToInt32(edtStart.Value);
			UpdateNumberFormat();
		}
		protected internal void UpdateNumberFormat() {
			if (this.displayFormatHelper != null)
				this.displayFormatHelper.SetDisplayFormat(Controller.DisplayFormat);
		}
		void OnDisplayFormatChanged(object sender, EventArgs e) {
			Controller.DisplayFormat = displayFormatHelper.GetDisplayFormatString();
		}
		void OnStartTextChanged(object sender, EventArgs e) {
			isCloseForm = validationProvider.Validate(edtStart);
		}
	}
	public class CultureInfoToLanguageIdConverter {
		static System.Collections.Generic.Dictionary<string, LanguageId> mapNameToLanguageId;
		static CultureInfoToLanguageIdConverter() {
			mapNameToLanguageId = new System.Collections.Generic.Dictionary<string, LanguageId>();
			mapNameToLanguageId.Add("en", LanguageId.English);
			mapNameToLanguageId.Add("fr", LanguageId.French);
			mapNameToLanguageId.Add("de", LanguageId.German);
			mapNameToLanguageId.Add("el", LanguageId.Greek);
			mapNameToLanguageId.Add("hi", LanguageId.Hindi);
			mapNameToLanguageId.Add("it", LanguageId.Italian);
			mapNameToLanguageId.Add("pt", LanguageId.Portuguese);
			mapNameToLanguageId.Add("ru", LanguageId.Russian);
			mapNameToLanguageId.Add("es", LanguageId.Spanish);
			mapNameToLanguageId.Add("sv", LanguageId.Swedish);
			mapNameToLanguageId.Add("tr", LanguageId.Turkish);
			mapNameToLanguageId.Add("uk", LanguageId.Ukrainian);
		}
		public static LanguageId GetLanguageId(System.Globalization.CultureInfo info) {
			return GetLanguageId(info, LanguageId.English);
		}
		public static LanguageId GetLanguageId(System.Globalization.CultureInfo info, LanguageId defaultLanguageId) {
			LanguageId result;
			if (mapNameToLanguageId.TryGetValue(info.TwoLetterISOLanguageName.ToLowerInvariant(), out result))
				return result;
			else
				return defaultLanguageId;
		}
	}
}
