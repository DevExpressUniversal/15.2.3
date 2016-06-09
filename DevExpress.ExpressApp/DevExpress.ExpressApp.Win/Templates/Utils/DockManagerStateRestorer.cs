#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.IO;
using System.Windows.Forms;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.Utils;
using DevExpress.XtraBars.Docking;
namespace DevExpress.ExpressApp.Win.Templates.Utils {
	public class DockManagerStateRestorer {
		IModelTemplateWin templateModel;
		DockManager dockManager;
		private void MainForm_Load(object sender, EventArgs e) {
			LoadDockManagerSettings();
		}
		private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
			SaveDockManagerSettings();
		}
		private void LoadDockManagerSettings() {
			if(!string.IsNullOrEmpty(templateModel.DockManagerSettings)) {
				MemoryStream stream = new MemoryStream(Convert.FromBase64String(templateModel.DockManagerSettings));
				dockManager.SerializationOptions.RestoreDockPanelsText = false;
				dockManager.RestoreLayoutFromStream(stream);
			}
		}
		private void SaveDockManagerSettings() {
			MemoryStream stream = new MemoryStream();
			dockManager.SaveLayoutToStream(stream);
			templateModel.DockManagerSettings = Convert.ToBase64String(stream.ToArray());
		}
		public void Attach(Form form, IModelTemplateWin templateModel, DockManager dockManager) {
			Guard.ArgumentNotNull(form, "form");
			Guard.ArgumentNotNull(templateModel, "templateModel");
			Guard.ArgumentNotNull(dockManager, "dockManager");
			this.templateModel = templateModel;
			this.dockManager = dockManager;
			form.Load += MainForm_Load;
			form.FormClosing += MainForm_FormClosing;
		}
	}
}
