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
using System.Linq;
using System.Text;
using DevExpress.XtraBars;
using System.ComponentModel;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraCharts.Native;
using DevExpress.Utils.UI;
namespace DevExpress.Snap.Extensions.Native {
	[DXToolboxItem(false)]
	public class SNPopupControlContainer : PopupControlContainer {
		protected override PopupContainerBarControl CreatePopupContainerBarControl(BarManager manager) {
			return new SNPopupContainerBarControl(manager, this);
		}
	}
	public class SNPopupContainerBarControl : PopupContainerBarControl {
		public SNPopupContainerBarControl(BarManager manager, PopupControlContainer popupControl)
			: base(manager, popupControl) {
		}
		public override bool ShouldCloseOnOuterClick(System.Windows.Forms.Control control, MouseInfoArgs e) {
			foreach (System.Windows.Forms.Form form in System.Windows.Forms.Application.OpenForms) {
				if ((form is SNSummaryEditorForm) || (form is SNFormatStringEditorForm) || (form is CollectionEditorFormBase) || (form is WizardFormBase))
					return false;
			}
			return true;
		}
	}
}
