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
using DevExpress.XtraRichEdit.Commands;
using DevExpress.Xpf.Core;
using DevExpress.Utils;
namespace DevExpress.XtraRichEdit.Forms {
	public partial class InsertTableFormControl : UserControl, IDialogContent {
		readonly InsertTableFormController controller;
		public InsertTableFormControl() {
			InitializeComponent();
		}
		public InsertTableFormControl(InsertTableFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.controller = CreateController(controllerParameters);
			Loaded += OnLoaded;
			InitializeComponent();
			SubscribeControlsEvents();
		}
		#region Properties
		public int ColumnsCount { get { return Convert.ToInt32(spnColumns.Value); } set { spnColumns.Value = Convert.ToDecimal(value); } }
		public int RowsCount { get { return Convert.ToInt32(spnRows.Value); } set { spnRows.Value = Convert.ToDecimal(value); } }
		public InsertTableFormController Controller { get { return controller; } }
		#endregion
		protected internal virtual InsertTableFormController CreateController(InsertTableFormControllerParameters controllerParameters) {
			return new InsertTableFormController(controllerParameters);
		}
		public void SubscribeControlsEvents() {
			spnColumns.EditValueChanged += ContentChanged;
			spnRows.EditValueChanged += ContentChanged;
		}
		public void UnsubscribeControlsEvents() {
			spnColumns.EditValueChanged -= ContentChanged;
			spnRows.EditValueChanged -= ContentChanged;
		}
		void ContentChanged(object sender, EventArgs e) {
			AssignValuesToController();
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			UpdateForm();
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
			Controller.ColumnCount = ColumnsCount;
			Controller.RowCount = RowsCount;
		}
		protected virtual void UpdateFormCore() {
			if (Controller == null)
				return;
			ColumnsCount = Controller.ColumnCount;
			RowsCount = Controller.RowCount;
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
