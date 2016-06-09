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
using System.ComponentModel;
using DevExpress.XtraBars.Docking;
using DevExpress.Snap.Extensions.Localization;
using System.Windows.Forms;
using DevExpress.Snap.Extensions.Native;
using DevExpress.Snap.Native;
namespace DevExpress.Snap.Extensions.UI {
	[DXToolboxItem(false)]
	public class SnapDockPanelBase : DockPanel {
		SnapControl snapControl;
		protected IDesignControl DesignControl { get; set; }
		public SnapDockPanelBase() {
			InitializeControl();
			Text = DefaultText;
		}
		public SnapDockPanelBase(DockManager dockManager, DockingStyle dock)
			: base(true, dock, dockManager) {
			InitializeControl();
			Text = DefaultText;
		}
		public override string Text {
			get { return base.Text; }
			set {
				base.Text = value;
			}
		}
		protected virtual string DefaultText { get { return SnapExtensionsLocalizer.Active.GetLocalizedString(SnapExtensionsStringId.FieldListDockPanel_Text); } }
		protected virtual bool ShouldSerializeText() {
			return Text != DefaultText;
		}		
		public override void ResetText() {
			Text = DefaultText;
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public SnapControl SnapControl {
			get { return snapControl; }
			set {
				snapControl = value;
				if (DesignControl != null)
					DesignControl.SnapControl = value;
			}
		}
		protected override void OnControlAdded(ControlEventArgs e) {
			base.OnControlAdded(e);
			Control control = DesignControl as Control;
			if (control != null && control.Parent == null && ControlContainer != null)
				ControlContainer.Controls.Add(control);
		}
		protected override void CreateControlContainer() {
			InitializeControl();
			base.CreateControlContainer();
		}
		protected virtual void InitializeControl() {
		}
	}
}
