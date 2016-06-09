#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.ComponentModel;
using DevExpress.XtraEditors.Controls;
using DevExpress.DashboardCommon;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public partial class DeltaOptionsControl : DashboardUserControl {
		DeltaOptions options;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DeltaOptions DeltaOptions { get { return options; } }
		public event EventHandler OptionsChanged;
		public DeltaOptionsControl() { 
			InitializeComponent();
			foreach (DeltaValueType valueType in Enum.GetValues(typeof(DeltaValueType)))
				cbValueType.Properties.Items.Add(new DeltaValueTypeItem(valueType));
			foreach (DeltaIndicationMode mode in Enum.GetValues(typeof(DeltaIndicationMode)))
				cbResultIndication.Properties.Items.Add(new DeltaIndicationModeItem(mode));
			foreach (DeltaIndicationThresholdType thresholdType in Enum.GetValues(typeof(DeltaIndicationThresholdType)))
				cbThresholdType.Properties.Items.Add(new DeltaIndicationThresholdTypeItem(thresholdType));
			PrepareOptions(new DeltaOptions());
		}
		void RaiseOptionsChanged() {
			if(OptionsChanged != null)
				OptionsChanged(this, EventArgs.Empty);
		}
		void OnValueTypeChanged(object sender, EventArgs e) {
			options.ValueType = ((DeltaValueTypeItem)cbValueType.SelectedItem).ValueType;
			RaiseOptionsChanged();
		}
		void OnResultIndicationChanged(object sender, EventArgs e) {
			options.ResultIndicationMode = ((DeltaIndicationModeItem)cbResultIndication.SelectedItem).Mode;
			RaiseOptionsChanged();
		}
		void OnThresholdTypeChanged(object sender, EventArgs e) {
			DeltaIndicationThresholdType thresholdType = ((DeltaIndicationThresholdTypeItem)cbThresholdType.SelectedItem).ThresholdType;
			options.ResultIndicationThresholdType = thresholdType;
			if(thresholdType == DeltaIndicationThresholdType.Percent) {
				editThresholdValue.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton() });
				editThresholdValue.Properties.Mask.EditMask = "P";
			}
			else {
				editThresholdValue.Properties.Buttons.Clear();
				editThresholdValue.Properties.Mask.EditMask = String.Empty;
			}
			RaiseOptionsChanged();
		}
		void OnThresholdValueChanged(object sender, EventArgs e) {
			options.ResultIndicationThreshold = (decimal)editThresholdValue.EditValue;
			RaiseOptionsChanged();
		}
		public void PrepareOptions(DeltaOptions options) {
			this.options = options.Clone();
			cbValueType.SelectedItem = new DeltaValueTypeItem(options.ValueType);
			cbResultIndication.SelectedItem = new DeltaIndicationModeItem(options.ResultIndicationMode);
			cbThresholdType.SelectedItem = new DeltaIndicationThresholdTypeItem(options.ResultIndicationThresholdType);
			editThresholdValue.EditValue = options.ResultIndicationThreshold;
		}
	}
}
