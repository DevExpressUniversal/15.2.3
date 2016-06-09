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
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts.Wizard {
	internal partial class RectangleIndentsControl : ChartUserControl {
		bool inUpdate;
		RectangleIndents indents;
		int minValue;
		Action0 changedCallback;
		public RectangleIndentsControl() {
			InitializeComponent();
		}
		void UpdateControls() {
			inUpdate = true;
			txtLeft.Properties.MinValue = minValue;
			txtTop.Properties.MinValue = minValue;
			txtRight.Properties.MinValue = minValue;
			txtBottom.Properties.MinValue = minValue;
			txtLeft.EditValue = indents.Left;
			txtTop.EditValue = indents.Top;
			txtRight.EditValue = indents.Right;
			txtBottom.EditValue = indents.Bottom;
			if (indents.All == RectangleIndents.Undefined)
				txtAll.EditValue = 0;
			txtAll.EditValue = indents.All;
			inUpdate = false;
			if (changedCallback != null)
				changedCallback();
		}
		void txtAll_CustomDisplayText(object sender, CustomDisplayTextEventArgs e) {
			if (Convert.ToInt32(e.Value) == RectangleIndents.Undefined) {
				ChartStringId id = (indents.Left == indents.Right && indents.Top == indents.Bottom && indents.Left == indents.Top) ? 
					ChartStringId.WizIndentDefault : ChartStringId.WizIndentUndefined;
				e.DisplayText = ChartLocalizer.GetString(id);
			}
		}
		void txtAll_EditValueChanged(object sender, EventArgs e) {
			if (!inUpdate) {
				int value = Convert.ToInt32(txtAll.EditValue);
				indents.All = value < minValue ? minValue : value;
				UpdateControls();
			}
		}
		void CustomDisplayText(object sender, CustomDisplayTextEventArgs e) {
			if (Convert.ToInt32(e.Value) == RectangleIndents.Undefined)
				e.DisplayText = ChartLocalizer.GetString(ChartStringId.WizIndentDefault);
		}
		void txtLeft_EditValueChanged(object sender, EventArgs e) {
			if (!inUpdate) {
				indents.Left = Convert.ToInt32(txtLeft.EditValue);
				UpdateControls();
			}
		}
		void txtTop_EditValueChanged(object sender, EventArgs e) {
			if (!inUpdate) {
				indents.Top = Convert.ToInt32(txtTop.EditValue);
				UpdateControls();
			}
		}
		void txtRight_EditValueChanged(object sender, EventArgs e) {
			if (!inUpdate) {
				indents.Right = Convert.ToInt32(txtRight.EditValue);
				UpdateControls();
			}
		}
		void txtBottom_EditValueChanged(object sender, EventArgs e) {
			if (!inUpdate) {
				indents.Bottom = Convert.ToInt32(txtBottom.EditValue);
				UpdateControls();
			}
		}
		void Initialize(RectangleIndents indents, int minValue, Action0 changedCallback) {
			this.indents = indents;
			this.minValue = minValue;
			this.changedCallback = changedCallback;
			UpdateControls();
		}
		public void Initialize(RectangleIndents indents) {
			Initialize(indents, 0, null);
		}
		public void Initialize(RectangleIndents indents, int minValue) {
			Initialize(indents, minValue, null);
		}
		public void Initialize(RectangleIndents indents, Action0 changedCallback) {
			Initialize(indents, 0, changedCallback);
		}
	}
}
