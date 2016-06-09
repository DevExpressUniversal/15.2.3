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

using System.ComponentModel;
using System.ComponentModel.Design;
namespace DevExpress.XtraBars.Design {
	public class BarManagerLayoutFrame : DevExpress.XtraEditors.Frames.SimpleLayoutFrame {
		System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		public BarManagerLayoutFrame() {
			InitializeComponent();
		}
		protected BarManagerDesigner GetDesigner() {
			IComponent comp = EditingComponent as IComponent;
			if(comp == null || comp.Site == null) return null;
			IDesignerHost host = comp.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;
			if(host == null) return null;
			return host.GetDesigner(comp) as BarManagerDesigner;
		}
		protected override void RestoreLayout() {
			var designer = GetDesigner();
			var form = FindForm();
			if(designer != null)
				if(designer.RestoreLayout() && form != null) {
					form.Close();
					designer.Dispose();
				}
		}
		protected override void SaveLayout() {
			var designer = GetDesigner();
			if(designer != null)
				designer.SaveLayout();
		}
		protected override string GetDescription() {
			return "On this page, you can save and load the BarManager layout to/from the file in a local storage. Click the 'Load Layout...' or 'Save Layout...' button to invoke the open/save file dialog.";
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			components = new System.ComponentModel.Container();
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		}
		#endregion
	}
}
