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
using DevExpress.Xpf.Core;
using DevExpress.Utils;
using DevExpress.Xpf.Editors;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Office;
namespace DevExpress.XtraRichEdit.Forms {
	public partial class LineNumberingFormControl : UserControl, IDialogContent {
		readonly LineNumberingFormController controller;
		readonly IRichEditControl control;
		public LineNumberingFormControl() {
			InitializeComponent();
		}
		public LineNumberingFormControl(LineNumberingFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.control = controllerParameters.Control;
			this.controller = CreateController(controllerParameters);
			Loaded += OnLoaded;
			InitializeComponent();
			SubscribeControlsEvents();
		}
		#region Properties
		protected internal LineNumberingFormController Controller { get { return controller; } }
		public IRichEditControl Control { get { return control; } }
		public DocumentModelUnitConverter UnitConverter { get { return Control.InnerControl.DocumentModel.UnitConverter; } }
		#endregion
		public void SubscribeControlsEvents() {
			chkAddLineNumbering.EditValueChanged += chkAddLineNumbering_EditValueChanged;
		}
		public void UnsubscribeControlsEvents() {
			chkAddLineNumbering.EditValueChanged -= chkAddLineNumbering_EditValueChanged;
		}
		protected internal virtual LineNumberingFormController CreateController(LineNumberingFormControllerParameters controllerParameters) {
			return new LineNumberingFormController(controllerParameters);
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			InitializeForm();
			UpdateForm();
		}
		void InitializeForm() {
			edtFromText.ValueUnitConverter = UnitConverter;
			edtFromText.DefaultUnitType = Control.InnerControl.UIUnit;
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
		void chkAddLineNumbering_EditValueChanged(object sender, EditValueChangedEventArgs e) {
			bool? allowAddLineNumbering = chkAddLineNumbering.IsChecked;
			EnableControls(allowAddLineNumbering);
			if (allowAddLineNumbering == true) {
				if (Convert.ToInt32(edtCountBy.EditValue) <= 0)
					edtCountBy.EditValue = 1;
				CommitValuesToController();
			}
		}
		protected virtual void UpdateFormCore() {
			if (Controller == null)
				return;
			chkAddLineNumbering.IsChecked = Controller.Step > 0;
			edtStartAt.EditValue = Controller.StartingLineNumber;
			edtCountBy.EditValue = Math.Max(1, Controller.Step);
			edtFromText.Value = Controller.Distance;
			switch (Controller.NumberingRestartType) {
				case LineNumberingRestart.NewPage:
					rbtnRestartEachPage.IsChecked = true;
					break;
				case LineNumberingRestart.NewSection:
					rbtnRestartEachSection.IsChecked = true;
					break;
				case LineNumberingRestart.Continuous:
					rbtnContinuous.IsChecked = true;
					break;
			}
			EnableControls(chkAddLineNumbering.IsChecked);
		}
		protected internal virtual void EnableControls(bool? isChecked) {
			bool enable = isChecked.HasValue ? isChecked.Value : false;
#if !SL
			lblStartAt.IsEnabled = enable;
			lblFromText.IsEnabled = enable;
			lblCountBy.IsEnabled = enable;
			lblNumbering.IsEnabled = enable;
#endif
			edtStartAt.IsEnabled = enable;
			edtFromText.IsEnabled = enable;
			edtCountBy.IsEnabled = enable;
			rbtnRestartEachPage.IsEnabled = enable;
			rbtnRestartEachSection.IsEnabled = enable;
			rbtnContinuous.IsEnabled = enable;
		}
		protected internal virtual void CommitValuesToController() {
			if (Controller == null)
				return;
			if (chkAddLineNumbering.IsChecked == false)
				Controller.Step = 0;
			else
				Controller.Step = Convert.ToInt32(edtCountBy.EditValue);
			if (rbtnRestartEachPage.IsChecked.Value)
				Controller.NumberingRestartType = LineNumberingRestart.NewPage;
			if (rbtnRestartEachSection.IsChecked.Value)
				Controller.NumberingRestartType = LineNumberingRestart.NewSection;
			if (rbtnContinuous.IsChecked.Value)
				Controller.NumberingRestartType = LineNumberingRestart.Continuous;
			Controller.StartingLineNumber = Convert.ToInt32(edtStartAt.EditValue);
			Controller.Distance = edtFromText.Value.HasValue ? edtFromText.Value.Value : 0;
		}
		protected internal virtual void ApplyChanges() {
			if (Controller != null) {
				CommitValuesToController();
				Controller.ApplyChanges();
			}
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
