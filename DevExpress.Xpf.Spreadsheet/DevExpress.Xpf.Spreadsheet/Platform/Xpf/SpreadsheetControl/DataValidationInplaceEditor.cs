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
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Xpf.Spreadsheet.Extensions.Internal;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Collections;
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.Xpf.Core.Native;
using DrawingRectangle = System.Drawing.Rectangle;
using System.Windows.Input;
namespace DevExpress.Xpf.Spreadsheet {
	public partial class SpreadsheetControl {
		IDataValidationInplaceEditor IInnerSpreadsheetControlOwner.CreateDataValidationInplaceEditor() {
			return ViewControl.WorksheetControl.DataValidationInplaceEditor;
		}
	}
}
namespace DevExpress.Xpf.Spreadsheet.Internal {
	#region XpfCommentInplaceEditor
	public class XpfDataValidationInplaceEditor : ListBoxEdit, IDataValidationInplaceEditor {
		#region Fields
		SpreadsheetControl control;
		#endregion
		public XpfDataValidationInplaceEditor() {
			MaxHeight = 8 * 22 + 4; 
			SetVisible(false);
			EditValuePostMode = PostMode.Immediate;
		}
		public object Value { get { return SelectedItem; } }
		void SetVisible(bool visible) {
			Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
		}
		public void Activate() {
			SetVisible(true);
		}
		public void Deactivate() {
			SetVisible(false);
		}
		public void SetAllowedValues(List<string> values, string activeValue) {
			SetAllowedValuesCore(values, activeValue);
		}
		public void SetAllowedValues(List<DataValidationInplaceValue> values, DataValidationInplaceValue activeValue) {
			SetAllowedValuesCore(values, activeValue);
		}
		void SetAllowedValuesCore(IList values, object activeValue) {
			Items.BeginUpdate();
			try {
				Items.Clear();
				for (int i = 0; i < values.Count; i++) {
					Items.Add(values[i]);
				}
				SelectedItem = activeValue;
			}
			finally {
				Items.EndUpdate();
			}
		}
		public void SetBounds(DrawingRectangle bounds) {
			Rect rect = bounds.ToRect();
			rect.Offset(-1, -1);
			SetValue(Canvas.LeftProperty, rect.Left);
			SetValue(Canvas.TopProperty, rect.Top);
			Width = rect.Width;
		}
		public void SetFocus() {
			Focus();
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			this.control = LayoutHelper.FindParentObject<SpreadsheetControl>(this);
		}
		protected override void OnMouseUp(System.Windows.Input.MouseButtonEventArgs e) {
			base.OnMouseUp(e);
			Command command = control.CreateCommand(SpreadsheetCommandId.DataValidationInplaceEndEdit);
			command.Execute();
		}
		protected override void OnPreviewKeyDown(KeyEventArgs e) {
			base.OnPreviewKeyDown(e);
			if (e.Key == Key.Down) {
				this.SelectedIndex = Math.Min(SelectedIndex + 1, Items.Count - 1);
				HandlePreviewKeyDown(e);
			}
			if (e.Key == Key.Up) {
				this.SelectedIndex = Math.Max(0, SelectedIndex - 1);
				HandlePreviewKeyDown(e);
			}
		}
		void HandlePreviewKeyDown(KeyEventArgs e) {
			EditorListBox editor = EditCore as EditorListBox;
			editor.FocusSelectedItem();
			e.Handled = true;
		}
	}
	#endregion
}
