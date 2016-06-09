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
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
namespace DevExpress.XtraPivotGrid.Design {
	public partial class AllowedLocationsControl : UserControl {
		readonly IWindowsFormsEditorService edSvc;
		AllowedLocationArea startArea;
		AllowedLocationArea area;
		AllowedLocationArea currentArea;
		Color allowedAreaColor = Color.LightCyan;
		Color currentAreaColor = Color.GreenYellow;
		public AllowedLocationArea Area { get { return area; } }
		public AllowedLocationsControl(IWindowsFormsEditorService edSvcm, AllowedLocationArea allowed, AllowedLocationArea current)
			: this() {
			this.edSvc = edSvcm;
			area = allowed;
			startArea = allowed;
			currentArea = current;
			UpdatePanelsAppearance();
		}
		public AllowedLocationsControl() {
			InitializeComponent();
		}
		void UpdatePanelsAppearance() {
			pcColumnArea.Appearance.BackColor = Color.Empty;
			pcRowArea.Appearance.BackColor = Color.Empty;
			pcDataArea.Appearance.BackColor = Color.Empty;
			pcFilterArea.Appearance.BackColor = Color.Empty;
			pcHiddenFields.Appearance.BackColor = Color.Empty;
			if((area & AllowedLocationArea.HiddenArea) != 0)
				pcHiddenFields.Appearance.BackColor = allowedAreaColor;
			if((area & AllowedLocationArea.ColumnArea) != 0)
				pcColumnArea.Appearance.BackColor = allowedAreaColor;
			if((area & AllowedLocationArea.RowArea) != 0)
				pcRowArea.Appearance.BackColor = allowedAreaColor;
			if((area & AllowedLocationArea.DataArea) != 0)
				pcDataArea.Appearance.BackColor = allowedAreaColor;
			if((area & AllowedLocationArea.FilterArea) != 0)
				pcFilterArea.Appearance.BackColor = allowedAreaColor;
			switch(currentArea) {
				case AllowedLocationArea.HiddenArea:
					pcHiddenFields.Appearance.BackColor = Color.Lime;
					break;
				case AllowedLocationArea.ColumnArea:
					pcColumnArea.Appearance.BackColor = currentAreaColor;
					break;
				case AllowedLocationArea.RowArea:
					pcRowArea.Appearance.BackColor = currentAreaColor;
					break;
				case AllowedLocationArea.DataArea:
					pcDataArea.Appearance.BackColor = currentAreaColor;
					break;
				case AllowedLocationArea.FilterArea:
					pcFilterArea.Appearance.BackColor = currentAreaColor;
					break;
			}
		}
		void ProcessClick(AllowedLocationArea newArea) {
			if((currentArea & newArea) != 0)
				return;
			area ^= newArea;
			UpdatePanelsAppearance();
		}
		void pcColumnArea_Click(object sender, EventArgs e) {
			ProcessClick(AllowedLocationArea.ColumnArea);
		}
		void pcDataArea_Click(object sender, EventArgs e) {
			ProcessClick(AllowedLocationArea.DataArea);
		}
		void pcFilterArea_Click(object sender, EventArgs e) {
			ProcessClick(AllowedLocationArea.FilterArea);
		}
		void pcHiddenFields_Click(object sender, EventArgs e) {
			ProcessClick(AllowedLocationArea.HiddenArea);
		}
		void pcRowArea_Click(object sender, EventArgs e) {
			ProcessClick(AllowedLocationArea.RowArea);
		}
		void btnOK_Click(object sender, EventArgs e) {
			edSvc.CloseDropDown();
		}
		void btnCancel_Click(object sender, EventArgs e) {
			area = startArea;
			edSvc.CloseDropDown();
		}
	}
	[Flags]
	public enum AllowedLocationArea {
		All = 31,
		RowArea = 1,
		ColumnArea = 2,
		FilterArea = 4,
		DataArea = 8,
		HiddenArea = 16
	}
}
