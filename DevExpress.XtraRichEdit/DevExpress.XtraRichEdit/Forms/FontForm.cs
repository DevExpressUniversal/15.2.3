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
using DevExpress.XtraRichEdit.Model;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Utils;
#if !SL
using System.Drawing;
using DevExpress.XtraRichEdit.Internal;
#else
using System.Windows.Media;
#endif
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.FontForm.btnCancel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.FontForm.btnOk")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.FontForm.fontControl")]
#endregion
namespace DevExpress.XtraRichEdit.Forms {
	[DXToolboxItem(false)]
	public partial class FontForm : DevExpress.XtraEditors.XtraForm {
		static Point FormPreviousLocation;
		#region Fields
		FontFormController controller;
		bool keepLocation = true;
		#endregion
		FontForm() {
			InitializeComponent();
		}
		public FontForm(FontFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			InitializeComponent();
			this.controller = CreateController(controllerParameters);
			this.fontControl.RichEditControl = (RichEditControl)controllerParameters.Control;
			UpdateForm();
		}
		#region Properties
		public FontFormController Controller { get { return controller; } }
		public virtual bool KeepLocation { get { return keepLocation; } set { keepLocation = value; } }
		#endregion
		protected internal virtual FontFormController CreateController(FontFormControllerParameters controllerParameters) {
			return new FontFormController(controllerParameters);
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
		protected internal virtual void UpdateFormCore() {
			fontControl.BeginUpdate();
			fontControl.AllCaps = Controller.AllCaps;
			fontControl.Script = Controller.Script;
			fontControl.Strikeout = Controller.FontStrikeoutType;
			fontControl.FontName = Controller.FontName;
			fontControl.FontBold = Controller.FontBold;
			fontControl.FontItalic = Controller.FontItalic;
			fontControl.FontSize = Controller.DoubleFontSize.HasValue ? Controller.DoubleFontSize / 2f : null;
			fontControl.FontForeColor = Controller.ForeColor;
			fontControl.FontUnderlineColor = Controller.UnderlineColor;
			fontControl.FontUnderlineType = Controller.FontUnderlineType;
			fontControl.Hidden = Controller.Hidden;
			fontControl.UnderlineWordsOnly = Controller.UnderlineWordsOnly;
			fontControl.EndUpdate();
		}
		protected internal virtual void UnsubscribeControlsEvents() {
			fontControl.FontControlChanged -= new EventHandler(OnFontControlChanged);
		}
		protected internal virtual void SubscribeControlsEvents() {
			fontControl.FontControlChanged += new EventHandler(OnFontControlChanged);
		}
		protected internal virtual void OnFontControlChanged(object sender, EventArgs e) {
			Controller.AllCaps = fontControl.AllCaps;
			Controller.Script = fontControl.Script;
			Controller.FontStrikeoutType = fontControl.Strikeout;
			Controller.FontName = fontControl.FontName;
			Controller.FontBold = fontControl.FontBold;
			Controller.FontItalic = fontControl.FontItalic;
			Controller.DoubleFontSize = fontControl.FontSize.HasValue ? (int?)(fontControl.FontSize * 2) : null;
			Controller.ForeColor = fontControl.FontForeColor;
			if (fontControl.FontUnderlineType == controller.FontUnderlineType && controller.FontUnderlineType != UnderlineType.None)
				Controller.UnderlineColor = fontControl.FontUnderlineColor;
			Controller.Hidden = fontControl.Hidden;
			Controller.SetFontUnderline(fontControl.FontUnderlineType, fontControl.UnderlineWordsOnly);
			UpdateForm();
		}
		protected internal virtual void OnBtnOkClick(object sender, EventArgs e) {
			Controller.ApplyChanges();
			this.DialogResult = DialogResult.OK;
		}
		protected internal void OnFontFormClosed(object sender, FormClosedEventArgs e) {
			if (KeepLocation)
				FontForm.FormPreviousLocation = Location;
		}
		protected internal void OnFontFormLoad(object sender, EventArgs e) {
			if (KeepLocation && FontForm.FormPreviousLocation != Point.Empty)
				Location = FontForm.FormPreviousLocation;
		}
	}
}
