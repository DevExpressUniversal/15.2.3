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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Xpf.Core.Native;
using DevExpress.XtraSpreadsheet;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Internal;
namespace DevExpress.Xpf.Spreadsheet.Internal {
	public class FormulaBarEditor : XpfCellInplaceEditorBase {
		static readonly DependencyPropertyKey ScrollViewerPropertyKey;
		public static readonly DependencyProperty ScrollViewerProperty;
		static FormulaBarEditor() {
			ScrollViewerPropertyKey = DependencyProperty.RegisterReadOnly("ScrollViewer", typeof(ScrollViewer), typeof(FormulaBarEditor),
				new FrameworkPropertyMetadata(null));
			ScrollViewerProperty = ScrollViewerPropertyKey.DependencyProperty;
		}
		public FormulaBarEditor() {
			DefaultStyleKey = typeof(FormulaBarEditor);
			this.IsEnabled = false;
		}
		public ScrollViewer ScrollViewer {
			get { return (ScrollViewer)GetValue(ScrollViewerProperty); }
			private set { SetValue(ScrollViewerPropertyKey, value); }
		}
		#region Events
		#region ActivateEditor
		EventHandler onActivateEditor;
		public event EventHandler ActivateEditor { add { onActivateEditor += value; } remove { onActivateEditor -= value; } }
		protected internal virtual void RaiseActivateEditor() {
			if (onActivateEditor != null)
				onActivateEditor(this, EventArgs.Empty);
		}
		#endregion
		#region DeactivateEditor
		EventHandler onDeactivateEditor;
		public event EventHandler DeactivateEditor { add { onDeactivateEditor += value; } remove { onDeactivateEditor -= value; } }
		protected internal virtual void RaiseDeactivateEditor() {
			if (onDeactivateEditor != null)
				onDeactivateEditor(this, EventArgs.Empty);
		}
		#endregion
		#region Rollback
		EventHandler onRollback;
		public event EventHandler Rollback { add { onRollback += value; } remove { onRollback -= value; } }
		private void RaiseRollback() {
			if (onRollback != null)
				onRollback(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		protected internal override void Activate() {
			RaiseActivateEditor();
		}
		protected internal override void Deactivate() {
			base.Deactivate();
			RaiseDeactivateEditor();
		}
		public new bool IsVisible { get; set; }
		internal void InitializeSpreadsheet(ISpreadsheetControl spreadsheetControl) {
			this.IsEnabled = true;
			SpreadsheetControl control = spreadsheetControl as SpreadsheetControl;
			if (control != null) control.InnerControlInitialized -= InnerControlInitialized;
			Control = spreadsheetControl;
			control.InnerControlInitialized += InnerControlInitialized;
			WorksheetControl = LayoutHelper.FindElementByType(control, typeof(WorksheetControl)) as WorksheetControl;
			OnFormulaEditorLoaded();
		}
		private void InnerControlInitialized(object sender, EventArgs e) {
			OnFormulaEditorLoaded();
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			ScrollViewer = LayoutHelper.FindElementByType(this, typeof(ScrollViewer)) as ScrollViewer;
		}
		protected override void OnLoaded() {
			base.OnLoaded();
			OnFormulaEditorLoaded();
		}
		private void OnFormulaEditorLoaded() {
			if (Control != null)
				InnerEditor = Control.InnerControl.InplaceEditor;
		}
		protected override void OnTextInputCore(TextCompositionEventArgs e) {
			char[] text = e.Text.ToCharArray();
			if (InnerEditor != null && text.Length > 0) {
				InnerEditor.OnKeyPress(new System.Windows.Forms.KeyPressEventArgs(text[0]));
			}
		}
		protected override void OnRollBack() {
			base.OnRollBack();
			RaiseRollback();
		}
	}
}
