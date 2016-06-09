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

using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Forms;
using System;
using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Internal {
	#region InnerPivotTableFieldsPanel
	public class InnerPivotTableFieldsPanel {
		static Point InvalidLocation = new Point(Int32.MinValue, Int32.MaxValue);
		static Size InvalidSize = new Size(-1, -1);
		readonly InnerSpreadsheetControl innerControl;
		IPivotTableFieldsPanel panel;
		Point location = InvalidLocation;
		Size size = InvalidSize;
		bool isVisible;
		bool isInternalClosing;
		public InnerPivotTableFieldsPanel(InnerSpreadsheetControl innerControl) {
			Guard.ArgumentNotNull(innerControl, "innerControl");
			this.innerControl = innerControl;
		}
		public void ShowPanel(FieldListPanelPivotTableViewModel viewModel) {
			if (isVisible) {
				panel.ChangeViewModel(viewModel);
				return;
			}
			panel = CreatePanel();
			if (panel == null)
				return;
			SpreadsheetPivotTableFieldListOptions options = innerControl.Options.InnerPivotTableFieldList;
			if (location == InvalidLocation)
				panel.SetStartPosition(options.StartPosition, options.StartLocation);
			else
				panel.Location = location;
			if (size == InvalidSize) {
				if (options.StartSize != SpreadsheetPivotTableFieldListOptions.defaultStartSize)
					panel.Size = options.StartSize;
			}
			else
				panel.Size = size;
			panel.Closing += OnPanelClosing;
			panel.Show(viewModel);
			this.isVisible = true;
			this.isInternalClosing = false;
		}
		void OnPanelClosing(object sender, EventArgs e) {
			if (panel != null) {
				location = panel.Location;
				size = panel.Size;
				panel.Closing -= OnPanelClosing;
				panel = null;
			}
			TryHidePanelInModel();
			this.isVisible = false;
		}
		void TryHidePanelInModel() {
			if (!isInternalClosing)
				innerControl.DocumentModel.Properties.HidePivotFieldList = true;
		}
		public void HidePanel() {
			if (panel != null) {
				this.isInternalClosing = true;
				panel.Hide();
			}
		}
		IPivotTableFieldsPanel CreatePanel() {
			return innerControl.CreatePivotTableFieldsPanel();
		}
	}
	#endregion
}
