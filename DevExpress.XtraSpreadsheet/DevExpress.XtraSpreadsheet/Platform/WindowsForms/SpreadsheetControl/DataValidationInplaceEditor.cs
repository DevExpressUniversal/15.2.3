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
using DevExpress.XtraSpreadsheet.Internal;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.Utils.Commands;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraSpreadsheet.Model;
using System.Collections;
namespace DevExpress.XtraSpreadsheet {
	public partial class SpreadsheetControl {
		IDataValidationInplaceEditor IInnerSpreadsheetControlOwner.CreateDataValidationInplaceEditor() {
			return new WinFormsDataValidationInplaceEditor(this);
		}
	}
}
namespace DevExpress.XtraSpreadsheet.Internal {
	#region WinFormsDataValidationInplaceEditor
	[DXToolboxItem(false)]
	public class WinFormsDataValidationInplaceEditor : ComboBoxEdit, IDataValidationInplaceEditor {
		#region Fields
		readonly SpreadsheetControl control;
		#endregion
		public WinFormsDataValidationInplaceEditor(SpreadsheetControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			Initialize();
		}
		#region Properties
		public object Value { get { return GetValue(); } }
		#endregion
		void Initialize() {
			this.Visible = false;
			this.Properties.DropDownRows = 8;
			AppearanceObject appearance = this.Properties.AppearanceDropDown;
			appearance.Font = GetActualFont(appearance.Font);
		}
		Font GetActualFont(Font currentFont) {
			float zoomFactor = control.ActiveView.ZoomFactor;
			if (zoomFactor == 1.0f)
				return currentFont;
			return new Font(currentFont.FontFamily, currentFont.Size * zoomFactor, currentFont.Style, currentFont.Unit, currentFont.GdiCharSet, currentFont.GdiVerticalFont);
		}
		object GetValue() {
			if (PopupForm != null)
				SelectedIndex = PopupForm.ListBox.SelectedIndex;
			return SelectedItem;
		}
		void SubscribeEvents() {
			if (PopupForm != null) {
				this.PopupForm.KeyDown += OnKeyDown;
				this.PopupForm.ListBox.MouseUp += OnMouseUp;
			}
		}
		void UnsubscribeEvents() {
			if (PopupForm != null) {
				this.PopupForm.KeyDown -= OnKeyDown;
				this.PopupForm.ListBox.MouseUp -= OnMouseUp;
			}
		}
		void OnKeyDown(object sender, KeyEventArgs e) {
			control.InnerControl.OnKeyDown(e);
		}
		void OnMouseUp(object sender, MouseEventArgs e) {
			Command command = control.CreateCommand(SpreadsheetCommandId.DataValidationInplaceEndEdit);
			command.Execute();
		}
		public void SetBounds(Rectangle bounds) {
			bounds = GetActualBounds(bounds);
			this.Location = new Point(bounds.X, bounds.Y - this.Height);
			this.Width = bounds.Width;
		}
		Rectangle GetActualBounds(Rectangle originalBounds) {
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
		public void SetAllowedValues(List<DataValidationInplaceValue> values, DataValidationInplaceValue activeValue) {
			SetAllowedValuesCore(values, activeValue);
		}
		public void SetAllowedValues(List<string> values, string activeValue) {
			SetAllowedValuesCore(values, activeValue);
		}
		void SetAllowedValuesCore(IList values, object activeValue) {
			ComboBoxItemCollection items = this.Properties.Items;
			items.BeginUpdate();
			try {
				for (int i = 0; i < values.Count; i++) {
					items.Add(values[i]);
				}
			}
			finally {
				items.EndUpdate();
			}
			SelectedItem = activeValue;
		}
		public void SetFocus() {
			if (PopupForm != null)
				this.PopupForm.Focus();
		}
		public void Activate() {
			control.Controls.Add(this);
			this.Parent = control;
			this.ShowPopup();
			SubscribeEvents();
		}
		public void Deactivate() {
			UnsubscribeEvents();
			control.Controls.Remove(this);
			Dispose();
		}
	}
	#endregion
}
