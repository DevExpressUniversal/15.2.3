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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.Utils.Controls;
using DevExpress.DashboardCommon.ViewerData;
using System.Windows.Forms;
using DevExpress.DashboardCommon.Service;
using DevExpress.XtraMap;
namespace DevExpress.DashboardWin.Native {
	public class MapInteractivityController : InteractivityController {
		MapDashboardItemViewer mapViewer;
		DashboardMapControl MapControl { get { return mapViewer.MapControl; } }
		bool shiftPressed = false;
		public MapInteractivityController(MapDashboardItemViewer viewer)
			: base((IInteractivityControllerClient)viewer) {
				this.mapViewer = viewer;
			MapControl.SelectionChanged += OnSelectionChanged;
		}
		public override void ProcessKeyDown(KeyEventArgs e) {
			if(e.KeyCode == Keys.ShiftKey) {
				EnableShiftMode();
			}
			base.ProcessKeyDown(e);
		}
		public override void ProcessKeyUp(KeyEventArgs e) {
			if(e.KeyCode == Keys.ShiftKey) {
				DisableShiftMode();
			}
			base.ProcessKeyUp(e);
		}
		public override void ProcessLostFocus() {
			DisableShiftMode();
			base.ProcessLostFocus();
		}
		protected override void ProcessMouseClickAction(AxisPointTuple tuple) {
			if(!shiftPressed)
				base.ProcessMouseClickAction(tuple);
		}
		void OnSelectionChanged(object sender, MapSelectionChangedEventArgs e) {
			if(shiftPressed) {
				SetSelection(mapViewer.GetControlSelection());
				ApplySelection();
			}
		}
		void EnableShiftMode() {
			if(SelectionMode == DashboardSelectionMode.Multiple) {
				MapControl.SelectionMode = ElementSelectionMode.Multiple;
				shiftPressed = true;
			}
		}
		void DisableShiftMode() {
			MapControl.SelectionMode = ElementSelectionMode.None;
			shiftPressed = false;
		}
	}
}
