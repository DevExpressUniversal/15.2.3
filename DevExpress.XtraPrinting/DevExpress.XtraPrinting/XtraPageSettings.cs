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
using System.Drawing.Printing;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraPrinting {
	public class XtraPageSettings : XtraPageSettingsBase {
		#region Fields & Properties
#if DEBUGTEST
		internal
#endif
		PageSettings pageSettings;
		public PageSettings PageSettings {
			get {
				if(pageSettings == null)
					pageSettings = ps.Extend().PageSettings;
				return pageSettings;
			}
		}
		PrinterSettings PrinterSettings {
			get { return PageSettings.PrinterSettings; }
		}
		#endregion // Fields & Properties
		public XtraPageSettings(PrintingSystemBase ps)
			: base(ps) {
		}
		protected override PageData CreateData() {
			return CreatePageData(PageSettings, PageSettingsHelper.GetMinMargins(PageSettings));
		}
		[EditorBrowsable(EditorBrowsableState.Always)]
		public override void AssignPrinterSettings(string printerName, string paperName, PrinterSettingsUsing settingsUsing) {
			base.AssignPrinterSettings(printerName, paperName, settingsUsing);
		}
		public void Assign(PageSettings pageSettings) {
			try {
				PageData data = CreatePageData(pageSettings, PageSettingsHelper.GetMinMargins(pageSettings));
				Assign(data);
			} catch {
			}
		}
		public void AssignDefaultPrinterSettings() {
			try {
				Assign(PageSettingsHelper.DefaultPageSettings);
			} catch {
			}
		}
		public void AssignDefaultPrinterSettings(PrinterSettingsUsing settingsUsing) {
			ps.Extend().AssignDefaultPrinterSettings(settingsUsing);
		}
		[EditorBrowsable(EditorBrowsableState.Always)]
		public override void Assign(Margins margins, PaperKind paperKind, string paperName, bool landscape) {
			base.Assign(margins, paperKind, paperName, landscape);
		}
	}
}
