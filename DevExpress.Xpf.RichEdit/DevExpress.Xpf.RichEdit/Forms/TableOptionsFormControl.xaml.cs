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
using DevExpress.XtraRichEdit.Model;
using DevExpress.Xpf.RichEdit;
using DevExpress.Utils;
using DevExpress.Office;
namespace DevExpress.XtraRichEdit.Forms {
	#region TableOptionsForm
	public partial class TableOptionsFormControl : UserControl, IDialogContent {
		#region Fields
		readonly IRichEditControl control;
		TableOptionsFormController controller;
		#endregion
		public TableOptionsFormControl() {
			InitializeComponent();
		}
		public TableOptionsFormControl(TableOptionsFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.control = controllerParameters.Control;
			this.controller = CreateController(controllerParameters);
			InitializeComponent();
			Loaded += OnLoaded;
			SubscribeControlsEvents();
		}
		#region Properties
		public IRichEditControl Control { get { return control; } }
		public TableOptionsFormController Controller { get { return controller; } }
		#endregion
		protected internal virtual TableOptionsFormController CreateController(TableOptionsFormControllerParameters controllerParameters) {
			return new TableOptionsFormController(controllerParameters);
		}
		private void InitializeForm() {
			DocumentUnit UIUnit = Control.InnerControl.UIUnit;
			this.spnTopMargin.DefaultUnitType = UIUnit;
			this.spnRightMargin.DefaultUnitType = UIUnit;
			this.spnBottomMargin.DefaultUnitType = UIUnit;
			this.spnLeftMargin.DefaultUnitType = UIUnit;
			this.spnSpacingBetweenCells.DefaultUnitType = UIUnit;
			DocumentModelUnitConverter unitConverter = Control.InnerControl.DocumentModel.UnitConverter;
			this.spnTopMargin.ValueUnitConverter = unitConverter;
			this.spnRightMargin.ValueUnitConverter = unitConverter;
			this.spnBottomMargin.ValueUnitConverter = unitConverter;
			this.spnLeftMargin.ValueUnitConverter = unitConverter;
			this.spnSpacingBetweenCells.ValueUnitConverter = unitConverter;
			this.spnTopMargin.MaxValue = TableOptionsFormDefaults.MaxTopMarginByDefault;
			this.spnTopMargin.MinValue = TableOptionsFormDefaults.MinTopMarginByDefault;
			this.spnRightMargin.MaxValue = TableOptionsFormDefaults.MaxRightMarginByDefault;
			this.spnRightMargin.MinValue = TableOptionsFormDefaults.MinRightMarginByDefault;
			this.spnBottomMargin.MaxValue = TableOptionsFormDefaults.MaxBottomMarginByDefault;
			this.spnBottomMargin.MinValue = TableOptionsFormDefaults.MinBottomMarginByDefault;
			this.spnLeftMargin.MaxValue = TableOptionsFormDefaults.MaxLeftMarginByDefault;
			this.spnLeftMargin.MinValue = TableOptionsFormDefaults.MinLeftMarginByDefault;
			this.spnSpacingBetweenCells.MaxValue = TableOptionsFormDefaults.MaxCellSpacingByDefault;
			this.spnSpacingBetweenCells.MinValue = TableOptionsFormDefaults.MinCellSpacingByDefault;
		}
		protected internal virtual void SubscribeControlsEvents() {
			spnTopMargin.ValueChanged += OnSpnTopMarginValueChanged;
			spnRightMargin.ValueChanged += OnSpnRightMarginValueChanged;
			spnBottomMargin.ValueChanged += OnSpnBottomMarginValueChanged;
			spnLeftMargin.ValueChanged += OnSpnLeftMarginValueChanged;
			spnSpacingBetweenCells.ValueChanged += OnSpnSpacingBetweenCellsValueChanged;
			cbAllowCellSpacing.EditValueChanged += OnAllowCellSpacingChanged;
			chkResizeToFitContent.EditValueChanged += OnResizeToFitContentChanged;
		}
		protected internal virtual void UnsubscribeControlsEvents() {
			spnTopMargin.ValueChanged -= OnSpnTopMarginValueChanged;
			spnRightMargin.ValueChanged -= OnSpnRightMarginValueChanged;
			spnBottomMargin.ValueChanged -= OnSpnBottomMarginValueChanged;
			spnLeftMargin.ValueChanged -= OnSpnLeftMarginValueChanged;
			spnSpacingBetweenCells.ValueChanged -= OnSpnSpacingBetweenCellsValueChanged;
			cbAllowCellSpacing.EditValueChanged -= OnAllowCellSpacingChanged;
			chkResizeToFitContent.EditValueChanged -= OnResizeToFitContentChanged;
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			InitializeForm();
			UpdateForm();
		}
		protected internal virtual void UpdateForm() {
			UnsubscribeControlsEvents();
			try {
				UpdateFormCore();
			} finally {
				SubscribeControlsEvents();
			}
		}
		protected internal virtual void UpdateFormCore() {
			spnTopMargin.Value = Controller.TopMargin;
			spnRightMargin.Value = Controller.RightMargin;
			spnBottomMargin.Value = Controller.BottomMargin;
			spnLeftMargin.Value = Controller.LeftMargin;
			spnSpacingBetweenCells.Value = Controller.CellSpacing;
			spnSpacingBetweenCells.IsEnabled = Controller.AllowCellSpacing;
			cbAllowCellSpacing.IsChecked = Controller.AllowCellSpacing;
			chkResizeToFitContent.IsChecked = Controller.ResizeToFitContent;
		}
		void OnSpnTopMarginValueChanged(object sender, EventArgs e) {
			Controller.TopMargin = spnTopMargin.Value;
		}
		void OnSpnRightMarginValueChanged(object sender, EventArgs e) {
			Controller.RightMargin = spnRightMargin.Value;
		}
		void OnSpnBottomMarginValueChanged(object sender, EventArgs e) {
			Controller.BottomMargin = spnBottomMargin.Value;
		}
		void OnSpnLeftMarginValueChanged(object sender, EventArgs e) {
			Controller.LeftMargin = spnLeftMargin.Value;
		}
		void OnSpnSpacingBetweenCellsValueChanged(object sender, EventArgs e) {
			Controller.CellSpacing = spnSpacingBetweenCells.Value;
		}
		void OnAllowCellSpacingChanged(object sender, EventArgs e) {
			bool cellSpacingChecked = cbAllowCellSpacing.IsChecked.Value;
			if (cellSpacingChecked && spnSpacingBetweenCells.Value == 0)
				spnSpacingBetweenCells.Value = TableOptionsFormDefaults.MinCellSpacing;
			Controller.AllowCellSpacing = cellSpacingChecked;
			spnSpacingBetweenCells.IsEnabled = cellSpacingChecked;
		}
		void OnResizeToFitContentChanged(object sender, EventArgs e) {
			Controller.ResizeToFitContent = chkResizeToFitContent.IsChecked.Value;
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
	#endregion
}
