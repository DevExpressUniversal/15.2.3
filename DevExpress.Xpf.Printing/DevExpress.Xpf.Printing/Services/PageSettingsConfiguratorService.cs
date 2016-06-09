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

using DevExpress.XtraPrinting;
using DevExpress.Xpf.Printing.Native;
using DevExpress.Xpf.Core;
using DevExpress.XtraPrinting.Native;
#if !SL
using System.Printing;
using System.Windows;
using System.Windows.Interop;
using System.Drawing.Printing;
#else
using DevExpress.Xpf.Drawing.Printing;
#endif
namespace DevExpress.Xpf.Printing {
	public class PageSettingsConfiguratorService : IPageSettingsConfiguratorService {
#if !SL
		public bool? Configure(XtraPageSettingsBase pageSettings, Window ownerWindow) {
			try {
				var pageSetupWindow = new PageSetupWindow { Owner = ownerWindow, WindowStartupLocation = WindowStartupLocation.CenterOwner };
				if(ownerWindow != null) {
					pageSetupWindow.FlowDirection = ownerWindow.FlowDirection;
				}
				PageSetupViewModel model = pageSetupWindow.Model;
				AssignPageSettingsToModel(pageSettings, model);
				if(pageSetupWindow.ShowDialog() == true) {
					AssignPageSettingsFromModel(pageSettings, model);
				}
				return pageSetupWindow.DialogResult;
			} catch(PrintServerException) {
				MessageBoxHelper.Show(MessageBoxButton.OK, MessageBoxImage.Error, DevExpress.XtraPrinting.Localization.PreviewStringId.Msg_NeedPrinter);
				return false;
			}
		}
#else
		public void Configure(XtraPageSettingsBase pageSettings) {
			var pageSetupWindow = new PageSetupWindow();
			PageSetupViewModel model = pageSetupWindow.Model;
			pageSetupWindow.Closed += (o, e) => { 
				if(pageSetupWindow.DialogResult == DialogResult.OK) {
					AssignPageSettingsFromModel(pageSettings, model);
				} 
			};
			pageSetupWindow.Tag = pageSettings;			
			AssignPageSettingsToModel(pageSettings, model);
			pageSetupWindow.ShowDialog();
		}
#endif
		static void AssignPageSettingsFromModel(XtraPageSettingsBase pageSettings, PageSetupViewModel model) {
			PageSetupViewModel.Margins margins = model.GetMargins(GraphicsDpi.Document);
			var newMargins = new MarginsF(margins.Left, margins.Right, margins.Top, margins.Bottom);
			var newPageData = new PageData(newMargins, model.PaperKind, model.Landscape);
			if(model.PaperKind == PaperKind.Custom) {
				newPageData.Size = pageSettings.Data.Size;
			}
			pageSettings.Assign(newPageData);
		}
		static void AssignPageSettingsToModel(XtraPageSettingsBase pageSettings, PageSetupViewModel model) {
			var pageData = pageSettings.Data;
			model.Landscape = pageSettings.Landscape;
			model.PaperKind = pageSettings.PaperKind;
			model.SetMargins(
				pageData.MarginsF.Left,
				pageData.MarginsF.Top,
				pageData.MarginsF.Right,
				pageData.MarginsF.Bottom,
				GraphicsDpi.Document);
		}
	}
}
