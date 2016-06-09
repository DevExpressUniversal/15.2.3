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

using DevExpress.Snap;
using DevExpress.Snap.Core.Commands;
using DevExpress.Snap.Extensions.Native;
using DevExpress.Utils.Commands;
using DevExpress.Utils.UI;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands;
using System;
using System.ComponentModel;
using System.Drawing;
namespace DevExpress.Snap.Extensions.UI {
	public class PropertiesBarButtonItem : ControlCommandBarButtonItem<RichEditControl, RichEditCommandId> {
		SNPopupControlContainer container;
		SNSmartTagService frSmartTagService;
		public PropertiesBarButtonItem() {
		}
		public PropertiesBarButtonItem(BarManager manager)
			: base(manager) {
		}
		public PropertiesBarButtonItem(string caption)
			: base(caption) {
		}
		public PropertiesBarButtonItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		#region Properties
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BarButtonStyle ButtonStyle { get { return BarButtonStyle.DropDown; } set { } }
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override PopupControl DropDownControl { get { return container; } set { } }
		protected override RichEditCommandId CommandId { get { return SnapCommandId.Properties; } }
		#endregion
		protected override Command CreateCommand() {
			return Control != null ? new PropertiesCommand(Control) : null;
		}
		protected override void Initialize() {
			base.Initialize();
			frSmartTagService = new SNSmartTagService();
			InitializePopupControl();
		}
		protected virtual void InitializePopupControl() {
			this.container = new SNPopupControlContainer();
			container.CloseOnOuterMouseClick = false;
			container.AutoSize = true;
			container.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			container.MinimumSize = new Size(10, 10);			
			container.Popup += new EventHandler(OnContainerPopup);
			frSmartTagService.Init(container);
		}
		protected override void OnControlChanged() {
			base.OnControlChanged();
			frSmartTagService.Control = (SnapControl)Control;
		} 
		protected virtual void OnContainerPopup(object sender, EventArgs e) {
			frSmartTagService.UpdatePopup();
		}
	}
}
