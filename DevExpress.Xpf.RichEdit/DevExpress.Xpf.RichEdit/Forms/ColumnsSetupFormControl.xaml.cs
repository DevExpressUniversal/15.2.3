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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Xpf.RichEdit;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.RichEdit.UI;
using System.Windows.Media.Imaging;
using DevExpress.Office;
namespace DevExpress.XtraRichEdit.Forms {
	public partial class ColumnsSetupFormControl : UserControl, IDialogContent {
		readonly ColumnsSetupFormController controller;
		readonly IRichEditControl control;
		readonly List<ColumnsPresetControl> presetControls = new List<ColumnsPresetControl>();
		public ColumnsSetupFormControl() {
			InitializeComponent();
		}
		public ColumnsSetupFormControl(ColumnsSetupFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.control = controllerParameters.Control;
			this.controller = CreateController(controllerParameters);
			Loaded += OnLoaded;
			InitializeComponent();
			SubscribeControlsEvents();
		}
		#region Properties
		protected internal ColumnsSetupFormController Controller { get { return controller; } }
		public IRichEditControl Control { get { return control; } }
		public DocumentModelUnitConverter UnitConverter { get { return Control.InnerControl.DocumentModel.UnitConverter; } }
		#endregion
		public void SubscribeControlsEvents() {
			int count = presetControls.Count;
			for (int i = 0; i < count; i++)
				presetControls[i].CheckedChanged += ColumnsPresetChecked;
			edtColumnCount.EditValueChanged += OnColumnCountChanged;
			chkEqualColumnWidth.Checked += OnEqualColumnWidthChanged;
			chkEqualColumnWidth.Unchecked += OnEqualColumnWidthChanged;
		}
		public void UnsubscribeControlsEvents() {
			int count = presetControls.Count;
			for (int i = 0; i < count; i++)
				presetControls[i].CheckedChanged -= ColumnsPresetChecked;
			edtColumnCount.EditValueChanged -= OnColumnCountChanged;
			chkEqualColumnWidth.Checked -= OnEqualColumnWidthChanged;
			chkEqualColumnWidth.Unchecked -= OnEqualColumnWidthChanged;
		}
		protected internal virtual ColumnsSetupFormController CreateController(ColumnsSetupFormControllerParameters controllerParameters) {
			return new ColumnsSetupFormController(controllerParameters);
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			InitializeForm();
			UpdateForm();
		}
		void InitializeForm() {
			InitPresetControls();
			columnsEdit.DefaultUnitType = Control.InnerControl.UIUnit;
			columnsEdit.ColumnsInfo = controller.ColumnsInfo;
			columnsEdit.ValueUnitConverter = controller.ValueUnitConverter;
			FillApplyToCombo(cbApplyTo);
			SetApplyToComboInitialValue(cbApplyTo);
		}
		protected internal virtual void InitPresetControls() {
			AddPresetControl(columnsPresetControlOne, new SingleColumnsInfoPreset());
			AddPresetControl(columnsPresetControlTwo, new TwoUniformColumnsInfoPreset());
			AddPresetControl(columnsPresetControlThree, new ThreeUniformColumnsInfoPreset());
			AddPresetControl(columnsPresetControlLeft, new LeftNarrowColumnsInfoPreset());
			AddPresetControl(columnsPresetControlRight, new RightNarrowColumnsInfoPreset());
		}
		protected internal virtual void AddPresetControl(ColumnsPresetControl presetControl, ColumnsInfoPreset preset) {
			presetControl.Tag = preset;
#if SL
			presetControl.ImageSource = preset.Image.Source;
#else
			BitmapImage imageSource = new BitmapImage();
			imageSource.BeginInit();
			imageSource.StreamSource = preset.ImageStream;
			imageSource.EndInit();
			imageSource.Freeze();
			presetControl.ImageSource = imageSource;
#endif
			presetControls.Add(presetControl);
		}
		void OnColumnCountChanged(object sender, EventArgs e) {
			Controller.ChangeColumnCount((int)edtColumnCount.Value);
			UpdateForm();
		}
		void OnEqualColumnWidthChanged(object sender, EventArgs e) {
			controller.SetEqualColumnWidth(chkEqualColumnWidth.IsChecked.Value);
			UpdateForm();
		}
		protected internal virtual void ColumnsPresetChecked(object sender, EventArgs e) {
			Action action = delegate() { ColumnsPresetCheckedCore(sender, e); };
			Dispatcher.BeginInvoke(action);
		}
		protected internal virtual void ColumnsPresetCheckedCore(object sender, EventArgs e) {
			UnsubscribeControlsEvents();
			try {
				ColumnsPresetControl control = (ColumnsPresetControl)sender;
				ColumnsInfoPreset preset = (ColumnsInfoPreset)control.Tag;
				Controller.ApplyPreset(preset);
				UpdateFormCore();
			}
			finally {
				SubscribeControlsEvents();
			}
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
		protected virtual void UpdateFormCore() {
			if (Controller == null)
				return;
			chkEqualColumnWidth.IsChecked = controller.ColumnsInfo.EqualColumnWidth;
			edtColumnCount.Value = GetEdtColumnCountValue(controller.ColumnsInfo.ColumnCount);
			columnsEdit.UpdateForm();
			int count = presetControls.Count;
			for (int i = 0; i < count; i++) {
				ColumnsInfoPreset preset = (ColumnsInfoPreset)presetControls[i].Tag;
				presetControls[i].Checked = preset.MatchTo(Controller.ColumnsInfo);
			}
		}
		protected internal virtual int GetEdtColumnCountValue(int? value) {
			return (value.HasValue) ? value.Value : 0;
		}
		protected internal virtual void CommitValuesToController() {
			if (Controller == null)
				return;
			SectionPropertiesApplyTypeListBoxItem applyTypeItem = cbApplyTo.SelectedItem as SectionPropertiesApplyTypeListBoxItem;
			if (applyTypeItem != null)
				controller.ApplyType = applyTypeItem.ApplyType;
		}
		protected internal virtual void ApplyChanges() {
			if (Controller != null) {
				CommitValuesToController();
				Controller.ApplyChanges();
			}
		}
		protected internal virtual void FillApplyToCombo(ComboBoxEdit combo) {
			ListItemCollection items = combo.Items;
			items.BeginUpdate();
			try {
				AddApplyToComboItem(items, SectionPropertiesApplyType.WholeDocument);
				AddApplyToComboItem(items, SectionPropertiesApplyType.CurrentSection);
				AddApplyToComboItem(items, SectionPropertiesApplyType.SelectedSections);
				AddApplyToComboItem(items, SectionPropertiesApplyType.ThisPointForward);
			}
			finally {
				items.EndUpdate();
			}
		}
		protected internal virtual void SetApplyToComboInitialValue(ComboBoxEdit combo) {
			if (Controller == null)
				return;
			combo.EditValue = null;
			ListItemCollection items = combo.Items;
			int count = items.Count;
			for (int i = 0; i < count; i++) {
				SectionPropertiesApplyTypeListBoxItem item = (SectionPropertiesApplyTypeListBoxItem)items[i];
				if (item.ApplyType == controller.ApplyType) {
					combo.SelectedIndex = i;
					break;
				}
			}
		}
		protected internal virtual void AddApplyToComboItem(ListItemCollection items, SectionPropertiesApplyType applyType) {
			if ((Controller.AvailableApplyType & applyType) == applyType)
				items.Add(new SectionPropertiesApplyTypeListBoxItem(applyType));
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
