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
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.Pdf;
using DevExpress.Pdf.Drawing;
using DevExpress.Pdf.Native;
namespace DevExpress.XtraPdfViewer.Native {
	public abstract class PdfEditorController : PdfDisposableObject {
		protected static HorzAlignment GetAlignment(PdfTextJustification textJustification) {
			switch (textJustification) {
				case PdfTextJustification.Centered:
					return HorzAlignment.Center;
				case PdfTextJustification.LeftJustified:
					return HorzAlignment.Near;
				case PdfTextJustification.RightJustified:
					return HorzAlignment.Far;
				default:
					return HorzAlignment.Default;
			}
		}
		public static PdfEditorController Create(PdfViewer viewer, PdfEditorSettings settings, IPdfViewerValueEditingCallBack callback) {
			switch (settings.EditorType) {
				case PdfEditorType.TextEdit:
					PdfTextEditSettings textEditSettings = (PdfTextEditSettings)settings;
					return textEditSettings.Multiline ? (PdfEditorController)new PdfMemoEditController(viewer, textEditSettings, callback)
													  : (PdfEditorController)new PdfTextEditController(viewer, textEditSettings, callback);
				case PdfEditorType.ComboBox:
					return new PdfComboBoxController(viewer, (PdfComboBoxSettings)settings, callback);
				case PdfEditorType.ListBox:
					return new PdfListBoxController(viewer, (PdfListBoxSettings)settings, callback);
			}
			return null;
		}
		readonly PdfEditorSettings settings;
		readonly IPdfViewerValueEditingCallBack callback;
		readonly PdfViewer viewer;
		readonly Font unscaledFont;
		float scale = PdfRenderingCommandInterpreter.DpiFactor;
		public PdfEditorSettings Settings { get { return settings; } }
		public float Scale { get { return scale; } }
		protected abstract object Value { get; }
		protected abstract BaseControl Control { get; }
		protected PdfEditorController(PdfViewer viewer, PdfEditorSettings settings, IPdfViewerValueEditingCallBack callback) {
			this.settings = settings;
			this.callback = callback;
			this.viewer = viewer;
			PdfEditableFontData fontData = settings.FontData;
			unscaledFont = new Font(fontData.FontFamily, (float)settings.FontSize, fontData.FontStyle);
		}
		protected virtual void SetFonts(Font scaledFont) {
			Control.Font = scaledFont;
		}
		protected virtual void DisposeFonts() {
			Control.Font.Dispose();
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				BaseControl control = Control;
				control.Leave -= TryValidateAndPost;
				control.KeyDown -= OnEditorKeyDown;
				control.Hide();
				viewer.Viewer.ViewControl.Invalidated -= UpdateParameters;
				viewer.Viewer.ViewControl.Controls.Remove(control);
				unscaledFont.Dispose();
			}
		}
		void OnEditorKeyDown(object sender, KeyEventArgs e) {
			switch (e.KeyCode) {
				case Keys.Escape:
					callback.HideEditor();
					break;
				case Keys.Enter:
					if (!(Control is PdfMemoEdit))
						PostValue();
					break;
			}
		}
		void TryValidateAndPost(object sender, EventArgs e) {
			object value = Value;
			if (callback != null) {
				PdfValidationError error = callback.ValidateEditor(value);
				if (error == null) {
					callback.PostEditor(value);
					callback.HideEditor();
					return;
				}
				Control.Focus();
			}
		}
		void UpdateParameters(object sender = null, EventArgs e = null) {
			PdfDocumentArea area = settings.DocumentArea;
			PdfRectangle rect = area.Area;
			int pageNumber = area.PageNumber;
			DisposeFonts();
			PerformFontUpdate();
			PointF pt1 = viewer.Viewer.DocumentToClient(new PdfDocumentPosition(pageNumber, rect.TopLeft));
			PointF pt2 = viewer.Viewer.DocumentToClient(new PdfDocumentPosition(pageNumber, rect.BottomRight));
			float left = Math.Min(pt1.X, pt2.X);
			float top = Math.Min(pt1.Y, pt2.Y);
			float right = Math.Max(pt1.X, pt2.X);
			float bottom = Math.Max(pt1.Y, pt2.Y);
			int width = (int)Math.Round(right - left);
			int height = (int)Math.Round(bottom - top);
			BaseControl control = Control;
			control.Size = new Size(width, height);
			Point location = new Point((int)Math.Round(left), (int)Math.Round(top));
			control.Visible = viewer.ClientRectangle.IntersectsWith(new Rectangle(location, control.Size));
			control.Location = location;
		}
		void PerformFontUpdate() {
			scale = viewer.ZoomFactor / 100 * PdfRenderingCommandInterpreter.DpiFactor;
			Font scaledFont = new Font(settings.FontData.FontFamily, unscaledFont.Size * scale, unscaledFont.Style, GraphicsUnit.Pixel);
			SetFonts(scaledFont);
		}
		public void PostValue() {
			TryValidateAndPost(null, null);
		}
		public virtual void SetUp() {
			BaseControl control = Control;
			control.ForeColor = settings.FontColor;
			control.BackColor = settings.BackgroundColor;
			control.KeyDown += OnEditorKeyDown;
			control.Leave += TryValidateAndPost;
			PerformFontUpdate();
			UpdateParameters();
			viewer.Viewer.ViewControl.Controls.Add(control);
			viewer.Viewer.ViewControl.Invalidated += UpdateParameters;
			control.Visible = true;
			control.Focus();
		}
	}
}
