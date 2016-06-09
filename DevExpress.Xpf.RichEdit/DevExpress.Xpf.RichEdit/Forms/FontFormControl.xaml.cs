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
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office.Internal;
#if SL
using PlatformIndependentColor = System.Windows.Media.Color;
#else
using PlatformIndependentColor = System.Drawing.Color;
#endif
namespace DevExpress.XtraRichEdit.Forms {
	public partial class RichEditFontControl : UserControl, IDialogContent {
		readonly FontFormController controller;
		public RichEditFontControl() {
			InitializeComponent();
			SubscribeControlsEvents();
		}
		public RichEditFontControl(FontFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.controller = CreateController(controllerParameters);
			InitializeComponent();
			SubscribeControlsEvents();
		}
		#region Properties
		public string FontName { get { return lbFontName.Text; } set { lbFontName.Text = value; } }
		public bool? FontBold {
			get { return lbFontStyle.SelectedIndex == 3 || lbFontStyle.SelectedIndex == 1; }
			set {
				if (value == null)
					lbFontStyle.SelectedIndex = -1;
				else if (lbFontStyle.SelectedIndex != 2)
					lbFontStyle.SelectedIndex = value.Value ? 1 : 0;
				else
					lbFontStyle.SelectedIndex = value.Value ? 3 : 2;
			}
		}
		public bool? FontItalic {
			get { return lbFontStyle.SelectedIndex == 3 || lbFontStyle.SelectedIndex == 2; }
			set {
				if (value == null)
					lbFontStyle.SelectedIndex = -1;
				else if (lbFontStyle.SelectedIndex != 1)
					lbFontStyle.SelectedIndex = value.Value ? 2 : 0;
				else
					lbFontStyle.SelectedIndex = value.Value ? 3 : 1;
			}
		}
		public Color? FontForeColor {
			get {
				if (cbForeColor.EditValue is Color)
					return (Color)cbForeColor.EditValue;
				return null;
			}
			set {
				if (value.HasValue)
					cbForeColor.Color = value.Value;
				else
					cbForeColor.EditValue = null;
			}
		}
		public Color? FontUnderlineColor {
			get {
				if (cbUnderlineColor.EditValue is Color)
					return (Color)cbUnderlineColor.EditValue;
				return null;
			}
			set {
				if (value.HasValue)
					cbUnderlineColor.Color = value.Value;
				else
					cbUnderlineColor.EditValue = null;
			}
		}
		public UnderlineType? FontUnderlineType { get { return cbUnderline.Value; } set { cbUnderline.Value = value; } }
		public int? SelectedFontSize {
			get {
				if (lbFontSize.Value == -1)
					return null;
				else
					return lbFontSize.Value;
			}
			set { lbFontSize.Value = value.HasValue ? value.Value : -1; }
		}
		public bool? AllCaps {
			get {
				return cbAllCaps.IsChecked;
			}
			set {
				cbAllCaps.IsChecked = value;
				cbAllCaps.IsThreeState = value == null;
			}
		}
		public bool? Hidden {
			get {
				return cbHidden.IsChecked;
			}
			set {
				cbHidden.IsChecked = value;
				cbHidden.IsThreeState = value == null;
			}
		}
		public bool? UnderlineWordsOnly {
			get {
				return cbUnderlineWordsOnly.IsChecked;
			}
			set {
				cbUnderlineWordsOnly.IsChecked = value;
				cbUnderlineWordsOnly.IsThreeState = value == null;
			}
		}
		public StrikeoutType? Strikeout {
			get {
				if (!cbStrikeNormal.IsChecked.Value && !cbDoubleStrikeout.IsChecked.Value && !cbStrikeout.IsChecked.Value) return null;
				if (cbStrikeNormal.IsChecked.Value) return StrikeoutType.None;
				if (cbStrikeout.IsChecked.Value) return StrikeoutType.Single;
				if (cbDoubleStrikeout.IsChecked.Value) return StrikeoutType.Double;
				return StrikeoutType.None;
			}
			set {
				if (!value.HasValue) {
					cbStrikeNormal.IsChecked = false;
					cbStrikeout.IsChecked = false;
					cbDoubleStrikeout.IsChecked = false;
					return;
				}
				cbStrikeNormal.IsChecked = value.Value == StrikeoutType.None;
				cbStrikeout.IsChecked = value.Value == StrikeoutType.Single;
				cbDoubleStrikeout.IsChecked = value.Value == StrikeoutType.Double;
			}
		}
		public CharacterFormattingScript? Script {
			get {
				if (!cbSuperscript.IsChecked.HasValue || !cbSubscript.IsChecked.HasValue)
					return null;
				if(cbSuperscript.IsChecked.Value)
					return CharacterFormattingScript.Superscript;
				if (cbSubscript.IsChecked.Value)
					return CharacterFormattingScript.Subscript;
				return CharacterFormattingScript.Normal;
			}
			set {
				if (!value.HasValue) {
					cbSubscript.IsChecked = null;
					cbSuperscript.IsChecked = null;
					return;
				}
				cbSubscript.IsChecked = value.Value == CharacterFormattingScript.Subscript;
				cbSuperscript.IsChecked = value.Value == CharacterFormattingScript.Superscript;
			}
		}
		public FontFormController Controller { get { return controller; } }
		#endregion
		protected internal virtual FontFormController CreateController(FontFormControllerParameters controllerParameters) {
			return new FontFormController(controllerParameters);
		}
		public void SubscribeControlsEvents() {
			lbFontName.SelectionChanged += ContentChanged;
			lbFontSize.ValueChanged += ContentChanged;
			lbFontStyle.SelectionChanged += ContentChanged;
			cbForeColor.EditValueChanged += ContentChanged;
			cbUnderline.EditValueChanged += ContentChanged;
			cbUnderlineColor.EditValueChanged += ContentChanged;
			cbAllCaps.EditValueChanged += ContentChanged;
			cbHidden.EditValueChanged += ContentChanged;
			cbUnderlineWordsOnly.EditValueChanged += ContentChanged;
			cbStrikeNormal.Click += ContentChanged;
			cbDoubleStrikeout.Click += ContentChanged;
			cbStrikeout.Click += ContentChanged;
			cbSubscript.EditValueChanged += ScriptValueChanged;
			cbSuperscript.EditValueChanged += ScriptValueChanged;
		}
		public void UnsubscribeControlsEvents() {
			lbFontName.SelectionChanged -= ContentChanged;
			lbFontSize.ValueChanged -= ContentChanged;
			lbFontStyle.SelectionChanged -= ContentChanged;
			cbForeColor.EditValueChanged -= ContentChanged;
			cbUnderline.EditValueChanged -= ContentChanged;
			cbUnderlineColor.EditValueChanged -= ContentChanged;
			cbAllCaps.EditValueChanged -= ContentChanged;
			cbHidden.EditValueChanged -= ContentChanged;
			cbUnderlineWordsOnly.EditValueChanged -= ContentChanged;
			cbStrikeNormal.Click -= ContentChanged;
			cbDoubleStrikeout.Click -= ContentChanged;
			cbStrikeout.Click -= ContentChanged;
			cbSubscript.EditValueChanged -= ScriptValueChanged;
			cbSuperscript.EditValueChanged -= ScriptValueChanged;
		}
		void ContentChanged(object sender, EditValueChangedEventArgs e) {
			AssignValuesToController();
		}
		void ContentChanged(object sender, EventArgs e) {
			AssignValuesToController();
		}
		void ScriptValueChanged(object sender, EditValueChangedEventArgs e) {
			UnsubscribeControlsEvents();
			try {
				if (sender == cbSubscript)
					cbSuperscript.IsChecked = false;
				else
					cbSubscript.IsChecked = false;
			}
			finally {
				SubscribeControlsEvents();
			}
			AssignValuesToController();
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			UpdateForm();
			lbFontName.Focus();
		}
		protected internal virtual void UpdateForm() {
			UnsubscribeControlsEvents();
			try {
				UpdateFormCore();
			}
			finally {
				SubscribeControlsEvents();
			}
		}
		protected internal virtual void AssignValuesToController() {
			if (Controller == null)
				return;
			Controller.AllCaps = AllCaps;
			Controller.Hidden = Hidden;
			Controller.SetFontUnderline(FontUnderlineType, UnderlineWordsOnly);
			Controller.Script = Script;
			Controller.DoubleFontSize = SelectedFontSize;
			Controller.FontStrikeoutType = Strikeout;
			Controller.FontBold = FontBold;
			Controller.FontItalic = FontItalic;
			Controller.FontName = FontName;
			if (FontForeColor.HasValue)
				Controller.ForeColor = XpfTypeConverter.ToPlatformIndependentColor(FontForeColor.Value);
			else
				Controller.ForeColor = null;
			if (FontUnderlineColor.HasValue)
				Controller.UnderlineColor = XpfTypeConverter.ToPlatformIndependentColor(FontUnderlineColor.Value);
			else
				Controller.UnderlineColor = null;
			UnsubscribeControlsEvents();
			FontUnderlineType = Controller.FontUnderlineType;
			UnderlineWordsOnly = Controller.UnderlineWordsOnly;
			SubscribeControlsEvents();
		}
		protected virtual void UpdateFormCore() {
			if (Controller == null)
				return;
			AllCaps = Controller.AllCaps;
			Hidden = Controller.Hidden;
			UnderlineWordsOnly = Controller.UnderlineWordsOnly;
			Script = Controller.Script;
			Strikeout = Controller.FontStrikeoutType;
			FontName = Controller.FontName;
			FontBold = Controller.FontBold;
			FontItalic = Controller.FontItalic;
			SelectedFontSize = Controller.DoubleFontSize;
			if (Controller.ForeColor.HasValue)
				FontForeColor = XpfTypeConverter.ToPlatformColor(Controller.ForeColor.Value);
			else
				FontForeColor = null;
			if (Controller.UnderlineColor.HasValue)
				FontUnderlineColor = XpfTypeConverter.ToPlatformColor(Controller.UnderlineColor.Value);
			else
				FontUnderlineColor = null;
			FontUnderlineType = Controller.FontUnderlineType;
		}
		protected internal virtual void ApplyChanges() {
			if (Controller != null)
				Controller.ApplyChanges();
		}
		#region IDialogContent Members
		bool IDialogContent.CanCloseWithOKResult() {
			return true;
		}
		void IDialogContent.OnApply() {
			ApplyChanges();
		}
		void IDialogContent.OnOk() {
			ApplyChanges();
		}
		#endregion
	}
}
