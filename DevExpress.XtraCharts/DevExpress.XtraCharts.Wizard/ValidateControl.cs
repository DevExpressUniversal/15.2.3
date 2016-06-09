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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
namespace DevExpress.XtraCharts.Wizard {
	internal partial class ValidateControl : ChartUserControl {
		bool invalidState;
		protected bool InvalidState { get{ return invalidState; } }
		public ValidateControl() {
			InitializeComponent();
		}
		protected void SetInvalidState() {
			this.invalidState = true;
		}
		protected void SetValidState() {
			this.invalidState = false;
		}
		public virtual bool ValidateContent() {
			if (invalidState)
				return false;
			foreach (ValidateControl ctrl in ScanControlsHierarchyForValidateControl(this))
				if (!ctrl.ValidateContent())
					return false;
			return true;
		}
		List<ValidateControl> ScanControlsHierarchyForValidateControl(Control control) {
			List<ValidateControl> list = new List<ValidateControl>();
			foreach (Control ctrl in control.Controls) {
				if (ctrl is ValidateControl)
					list.Add((ValidateControl)ctrl);
				else
					list.AddRange(ScanControlsHierarchyForValidateControl(ctrl));
			}
			return list;
		}
	}
}
