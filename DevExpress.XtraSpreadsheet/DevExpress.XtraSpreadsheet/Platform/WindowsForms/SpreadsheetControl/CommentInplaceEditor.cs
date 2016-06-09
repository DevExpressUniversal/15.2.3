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

using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.XtraSpreadsheet {
	public partial class SpreadsheetControl {
		ICommentInplaceEditor IInnerSpreadsheetControlOwner.CreateCommentInplaceEditor() {
			return new WinFormsCommentInplaceEditor(this);
		}
	}
}
namespace DevExpress.XtraSpreadsheet.Internal {
	#region WinFormsCommentInplaceEditor
	[DXToolboxItem(false)]
	public class WinFormsCommentInplaceEditor : Panel, ICommentInplaceEditor {
		#region Fields
		readonly SpreadsheetControl control;
		DevExpress.XtraEditors.MemoEdit textBox;
		#endregion
		public WinFormsCommentInplaceEditor(SpreadsheetControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			Initialize();
			SubscribeEvents();
		}
		#region Properties
		string ICommentInplaceEditor.Text { get { return textBox.Text; } set { textBox.Text = value; } }
		#endregion
		void Initialize() {
			this.BorderStyle = BorderStyle.FixedSingle;
			InitializeTextControl();
		}
		void InitializeTextControl() {
			this.textBox = new XtraEditors.MemoEdit();
			this.textBox.ImeMode = control.ImeMode;
			this.textBox.BorderStyle = XtraEditors.Controls.BorderStyles.NoBorder;
			this.textBox.Properties.ScrollBars = ScrollBars.None;
			this.textBox.Properties.Appearance.Font = GetActualFont();
			this.textBox.Font = GetActualFont();
			this.textBox.ForeColor = Comment.DefaultForeColor;
		}
		void SubscribeEvents() {
			this.textBox.KeyDown += OnKeyDown;
		}
		void UnsubscribeEvents() {
			this.textBox.KeyDown -= OnKeyDown;
		}
		void OnKeyDown(object sender, KeyEventArgs e) {
			control.InnerControl.OnKeyDown(e);
		}
		void ICommentInplaceEditor.Activate() {
			this.Controls.Add(textBox);
			textBox.Parent = this;
			control.Controls.Add(this);
			this.Parent = control;
		}
		void ICommentInplaceEditor.Deactivate() {
			control.Controls.Remove(this);
			Dispose();
		}
		void ICommentInplaceEditor.SetBackColor(Color color) {
			this.BackColor = color;
			this.textBox.BackColor = color;
		}
		void ICommentInplaceEditor.SetBounds(Rectangle bounds) {
			this.Bounds = GetActualBounds(bounds);
			this.textBox.Bounds = GetActualBounds(new Rectangle(1, 0, bounds.Width - 4, bounds.Height)); 
		}
		Rectangle GetActualBounds(Rectangle originalBounds) {
			originalBounds = control.DocumentModel.LayoutUnitConverter.LayoutUnitsToPixels(originalBounds);
			float zoomFactor = control.ActiveView.ZoomFactor;
			if (zoomFactor == 1.0f)
				return originalBounds;
			int x = ApplyZoomFactor(originalBounds.X, zoomFactor);
			int y = ApplyZoomFactor(originalBounds.Y, zoomFactor);
			int width = ApplyZoomFactor(originalBounds.Width, zoomFactor);
			int height = ApplyZoomFactor(originalBounds.Height, zoomFactor);
			return new Rectangle(x, y, width, height);
		}
		int ApplyZoomFactor(float value, float zoomFactor) {
			return (int)Math.Round(value * zoomFactor);
		}
		void ICommentInplaceEditor.SetFocus() {
			this.textBox.Focus();
		}
		void ICommentInplaceEditor.SetSelection() {
			this.textBox.SelectionStart = this.textBox.Text.Length;
			this.textBox.ScrollToCaret();
		}
		Font GetActualFont() {
			float zoomFactor = control.ActiveView.ZoomFactor;
			Font font = Comment.DefaultFont;
			if (zoomFactor == 1.0f)
				return font;
			return new Font(font.FontFamily, font.Size * zoomFactor, font.Style, font.Unit, font.GdiCharSet, font.GdiVerticalFont);
		}
		protected override void Dispose(bool disposing) {
			try {
				if (textBox != null) {
					UnsubscribeEvents();
					textBox.Dispose();
					this.textBox = null;
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
	}
	#endregion
}
