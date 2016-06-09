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
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Commands;
using System.Windows;
namespace DevExpress.XtraRichEdit.Forms {
	public partial class DeleteTableCellsFormControl : UserControl, IDialogContent {
		#region Fields
		readonly DeleteTableCellsFormController controller;
		#endregion
		public DeleteTableCellsFormControl() {
			InitializeComponent();
		}
		public DeleteTableCellsFormControl(DeleteTableCellsFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.controller = CreateController(controllerParameters);
			Loaded += OnLoaded;
			InitializeComponent();
			SubscribeControlsEvents();
		}
		#region Properties
		public DeleteTableCellsFormController Controller { get { return controller; } }
		#endregion
		protected internal virtual DeleteTableCellsFormController CreateController(DeleteTableCellsFormControllerParameters controllerParameters) {
			return new DeleteTableCellsFormController(controllerParameters);
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			UpdateForm();
		}
		protected internal virtual void SubscribeControlsEvents() {
			rbShiftLeft.Checked += ContentChanged;
			rbShiftUp.Checked += ContentChanged;
			rbDeleteRow.Checked += ContentChanged;
			rbDeleteColumn.Checked += ContentChanged;
		}
		protected internal virtual void UnsubscribeControlsEvents() {
			rbShiftLeft.Checked -= ContentChanged;
			rbShiftUp.Checked -= ContentChanged;
			rbDeleteRow.Checked -= ContentChanged;
			rbDeleteColumn.Checked -= ContentChanged;
		}
		protected internal void ContentChanged(object sender, RoutedEventArgs e) {
			if (Controller == null)
				return;
			Controller.CellOperation = (TableCellOperation)GetSelectedIndex();
		}
		int GetSelectedIndex() {
			return rbShiftLeft.IsChecked.Value ? 0 :
				rbShiftUp.IsChecked.Value ? 1 :
				rbDeleteRow.IsChecked.Value ? 2 : 3;
		}
		protected internal virtual void UpdateForm() {
			UnsubscribeControlsEvents();
			try {
				UpdateFormCore();
			} finally {
				SubscribeControlsEvents();
			}
		}
		protected virtual void UpdateFormCore() {
			if (Controller == null)
				return;
			rbShiftLeft.IsChecked = (int)Controller.CellOperation == 0;
			rbShiftUp.IsChecked = (int)Controller.CellOperation == 1;
			rbDeleteRow.IsChecked = (int)Controller.CellOperation == 2;
			rbDeleteColumn.IsChecked = (int)Controller.CellOperation == 3;
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
