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
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraBars.Docking.Design.Frames {
	public class DockPanelLayoutFrame : DevExpress.XtraEditors.Frames.SimpleLayoutFrame {
		IContainer components = null;
		public DockPanelLayoutFrame() {
			InitializeComponent();
		}
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		DockManager Manager { get { return EditingObject as DockManager; } }
		protected override void RestoreLayoutFromXml(string path) {
			if(Manager == null) return;
			TypeDescriptor.Refresh(typeof(DockPanel));
			foreach(var item in Manager.Panels) {
				TypeDescriptor.Refresh(item);
			}
			Manager.RestoreLayoutFromXml(path);
		}
		protected override void SaveLayoutToXml(string path) {
			if(Manager == null) return;
			TypeDescriptor.Refresh(typeof(DockPanel));
			foreach(var item in Manager.Panels) {
				TypeDescriptor.Refresh(item);
			}
			Manager.SaveLayoutToXml(path);
		}
		protected override string GetDescription() {
			return "On this page, you can save and load the DockManager layout to/from the file in a local storage. Click the 'Load Layout...' or 'Save Layout...' button to invoke the open/save file dialog.";
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			components = new System.ComponentModel.Container();
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		}
		#endregion
	}
}
