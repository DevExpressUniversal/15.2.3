#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.DashboardWin.Localization;
using DevExpress.DataAccess.Native;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
namespace DevExpress.DashboardWin.Native {
	public partial class FormatRuleCustomStyleForm : DashboardForm, IStyleContainerProvider {
		static Color DefaultBorderColor = Color.Black;
		readonly Locker locker = new Locker();
		StyleMode styleMode;
		public StyleSettingsContainer Style {
			get {
				Color? backColor = this.edtBackground.IsAutomaticColorSelected ? StyleSettingsContainer.DefaultBackColor : (Color?)this.edtBackground.Color;
				if(styleMode == StyleMode.Bar || styleMode == StyleMode.BarGradientNonemptyStop || styleMode == StyleMode.BarGradientStop) {
					return new StyleSettingsContainer(styleMode, backColor);
				}
				else {
					FontStyle fontStyle = FontStyle.Regular;
					if(this.cbBold.Checked)
						fontStyle |= FontStyle.Bold;
					if(this.cbItalic.Checked)
						fontStyle |= FontStyle.Italic;
					if(this.cbUnderline.Checked)
						fontStyle |= FontStyle.Underline;
					return new StyleSettingsContainer(styleMode,
						backColor,
						this.edtColor.IsAutomaticColorSelected ? StyleSettingsContainer.DefaultForeColor : (Color?)this.edtColor.Color,
						this.lbFontFamily.SelectedItem == AutomaticListBoxItem.Empty ? StyleSettingsContainer.DefaultFontFamily : Convert.ToString(this.lbFontFamily.SelectedItem),
						StyleSettingsContainer.DefaultFontSize, 
						fontStyle != FontStyle.Regular ? (FontStyle?)fontStyle : null
					);
				}
			}
			set {
				this.styleMode = value.Mode;
				locker.Lock();
				try {
					if(value.BackColor.HasValue)
						this.edtBackground.Color = value.BackColor.Value;
					if(value.ForeColor.HasValue)
						this.edtColor.Color = value.ForeColor.Value;
					if(!value.IsFontFamilyDefault) {
						string fontFamily = value.FontFamily;
						if(!this.lbFontFamily.Items.Contains(fontFamily)) {
							this.lbFontFamily.Items.Add(fontFamily);
						}
						this.lbFontFamily.SelectedItem = fontFamily;
					} else {
						this.lbFontFamily.SelectedItem = AutomaticListBoxItem.Empty;
					}
					if(value.FontStyle.HasValue) {
						if(value.FontStyle.Value.HasFlag(FontStyle.Bold))
							this.cbBold.Checked = true;
						if(value.FontStyle.Value.HasFlag(FontStyle.Italic))
							this.cbItalic.Checked = true;
						if(value.FontStyle.Value.HasFlag(FontStyle.Underline))
							this.cbUnderline.Checked = true;
					}
				} finally {
					locker.Unlock();
				}
			}
		}
		public FormatRuleCustomStyleForm() {
			InitializeComponent();
		}
		public FormatRuleCustomStyleForm(UserLookAndFeel lookAndFeel, StyleSettingsContainer style)
			: base(lookAndFeel) {
			InitializeComponent();
			locker.Lock();
			layoutControl.BeginUpdate();
			try {
				this.edtColor.Properties.AutomaticColorButtonCaption = this.edtBackground.Properties.AutomaticColorButtonCaption = CaptionManager.AutomaticCaption;
				this.edtColor.Properties.AutomaticBorderColor = this.edtBackground.Properties.AutomaticBorderColor = DefaultBorderColor;
				this.edtBackground.Properties.AutomaticColor = StyleSettingsContainerPainter.GetDefaultBackColor(LookAndFeel);
				this.edtBackground.Color = this.edtBackground.Properties.AutomaticColor;
				this.edtColor.Properties.AutomaticColor = StyleSettingsContainerPainter.GetDefaultForeColor(LookAndFeel);
				this.edtColor.Color = this.edtColor.Properties.AutomaticColor;
				this.btnOK.Enabled = this.btnCreate.Enabled = false;
				this.lbFontFamily.Items.AddRange(GetFontFamilies());
				SubscribeEvents();
				this.Style = style;
				this.lciCreate.Visibility = style.IsEmpty ? XtraLayout.Utils.LayoutVisibility.Always : XtraLayout.Utils.LayoutVisibility.OnlyInCustomization;
				this.lciOK.Visibility = style.IsEmpty ? XtraLayout.Utils.LayoutVisibility.OnlyInCustomization : XtraLayout.Utils.LayoutVisibility.Always;
			} finally {
				layoutControl.EndUpdate();
				locker.Unlock();
			}
#if DEBUGTEST
			layoutControl.AllowCustomization = true;
#endif
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			this.pctPreview.Initialize(this, LookAndFeel);
		}
		protected override void DisposeInternal(bool disposing) {
			base.DisposeInternal(disposing);
			if(disposing) {
				UnsubscribeEvents();
			}
		}
		void SubscribeEvents() {
			this.edtColor.ColorChanged += OnStateChanged;
			this.edtBackground.ColorChanged += OnStateChanged;
			this.lbFontFamily.SelectedIndexChanged += OnStateChanged;
			this.cbBold.CheckedChanged += OnStateChanged;
			this.cbItalic.CheckedChanged += OnStateChanged;
			this.cbUnderline.CheckedChanged += OnStateChanged;
		}
		void UnsubscribeEvents() {
			this.edtColor.ColorChanged -= OnStateChanged;
			this.edtBackground.ColorChanged -= OnStateChanged;
			this.lbFontFamily.SelectedIndexChanged -= OnStateChanged;
			this.cbBold.CheckedChanged -= OnStateChanged;
			this.cbItalic.CheckedChanged -= OnStateChanged;
			this.cbUnderline.CheckedChanged -= OnStateChanged;
		}
		void OnStateChanged(object sender, EventArgs e) {
			if(!locker.IsLocked) {
				StyleSettingsContainer style = Style;
				btnOK.Enabled = btnCreate.Enabled = style.Mode.IsGradient() ? !style.IsBackColorDefault : !style.IsEmpty;
				pctPreview.Refresh();
			}
		}
		object[] GetFontFamilies() {
			ArrayList result = new ArrayList();
			result.Add(AutomaticListBoxItem.Empty);
			foreach(FontFamily family in FontFamily.Families)
				if(family.IsStyleAvailable(FontStyle.Regular))
					result.Add(family.Name);
			return result.ToArray();
		}
		void OnApplyClick(object sender, EventArgs e) {
			DialogResult = DialogResult.OK;
		}
		void OnCancelClick(object sender, EventArgs e) {
			DialogResult = DialogResult.Cancel;
		}
	}
	static class CaptionManager {
		public static string AutomaticCaption {
			get { return DashboardWinLocalizer.GetString(DashboardWinStringId.EditorAutomaticValue); }
		}
	}
	class AutomaticListBoxItem {
		public static AutomaticListBoxItem Empty = new AutomaticListBoxItem();
		public override string ToString() {
			return CaptionManager.AutomaticCaption;
		}
	}
}
