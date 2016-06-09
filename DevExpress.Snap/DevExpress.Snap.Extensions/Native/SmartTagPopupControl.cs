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

using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraPrinting.Native.Lines;
using DevExpress.XtraEditors;
using DevExpress.Utils;
using DevExpress.XtraPrinting.Native;
using System.ComponentModel;
namespace DevExpress.Snap.Extensions.Native {
	[DXToolboxItem(false)]
	public partial class ActionListPopupControl : XtraUserControl {
		readonly SNSmartTagService frsmartTagService;
		ActionListPopupControl() {
			InitializeComponent();
		}
		public ActionListPopupControl(SNSmartTagService frsmartTagService) {
			this.frsmartTagService = frsmartTagService;
			InitializeComponent();
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlConstants.DoubleBuffer, true);			
			this.SuspendLayout();
		}
		public void SetLineControllers(BaseLineController[] controllers) {
			this.SuspendLayout();
			this.Controls.Clear();
			SmartTagServiceBase.PopupHelper popupHelper = new SmartTagServiceBase.PopupHelper(frsmartTagService, this);			
			Size = popupHelper.FillLinesController(controllers, new Point(0, 0));
			this.ResumeLayout();
			this.PerformLayout();
			Form form = Parent.FindForm();
			if(form != null)
				form.Size = Size;
		}
	}
}
