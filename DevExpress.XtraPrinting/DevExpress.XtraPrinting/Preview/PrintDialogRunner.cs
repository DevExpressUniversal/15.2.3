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

using System.Drawing.Printing;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Printing.Native;
namespace DevExpress.XtraPrinting.Preview {
	public abstract class PrintDialogRunner {
		static PrintDialogRunner intstance;
		static public PrintDialogRunner Instance {
			get {
				if(intstance == null)
					intstance = new DefaultPrintDialogRunner();
				return intstance;
			}
			set { intstance = value; }
		}
		public abstract DialogResult Run(PrintDocument document, UserLookAndFeel lookAndFeel, IWin32Window owner, PrintDialogAllowFlags flags);
	}
	public class SystemPrintDialogRunner : PrintDialogRunner {
		public override DialogResult Run(PrintDocument document, UserLookAndFeel lookAndFeel, IWin32Window owner, PrintDialogAllowFlags flags){
			using(PrintDialog dialog = CreatePrintDialog(document, flags)) {
				DialogResult result = DevExpress.XtraPrinting.Native.DialogRunner.ShowDialog(dialog, owner);
				if(DialogResult.OK == result) {
					if(document is IPrintDocumentExtension)
						(document as IPrintDocumentExtension).PageRange = new PageScope(dialog.PrinterSettings.FromPage, dialog.PrinterSettings.ToPage).PageRange;
				}
				return result;
			}
		}
		protected virtual PrintDialog CreatePrintDialog(PrintDocument document, PrintDialogAllowFlags flags) {
			PrintDialog dialog = new PrintDialog();
			dialog.Document = document;
			dialog.AllowSomePages = flags.HasFlag(PrintDialogAllowFlags.AllowSomePages);
			dialog.AllowCurrentPage = flags.HasFlag(PrintDialogAllowFlags.AllowCurrentPage);
			dialog.AllowSelection = flags.HasFlag(PrintDialogAllowFlags.AllowSelection);
			dialog.AllowPrintToFile = flags.HasFlag(PrintDialogAllowFlags.AllowPrintToFile);
			dialog.UseEXDialog = true; 
			return dialog;
		}
	}
	public class DefaultPrintDialogRunner : PrintDialogRunner {
		public override DialogResult Run(PrintDocument document, UserLookAndFeel lookAndFeel, IWin32Window owner, PrintDialogAllowFlags flags){
			using(PrintEditorForm dialog = CreatePrintDialog(document, lookAndFeel, flags)) {
				return DevExpress.XtraPrinting.Native.DialogRunner.ShowDialog(dialog, owner);
			}
		}
		protected virtual PrintEditorForm CreatePrintDialog(PrintDocument document, UserLookAndFeel lookAndFeel, PrintDialogAllowFlags flags) {
			PrintEditorForm dialog = new PrintEditorForm();
			dialog.Document = document;
			dialog.AllowAllPages = flags.HasFlag(PrintDialogAllowFlags.AllowAllPages);
			dialog.AllowSomePages = flags.HasFlag(PrintDialogAllowFlags.AllowSomePages);
			dialog.AllowCurrentPage = flags.HasFlag(PrintDialogAllowFlags.AllowCurrentPage);
			dialog.AllowSelection = flags.HasFlag(PrintDialogAllowFlags.AllowSelection);
			dialog.AllowPrintToFile = flags.HasFlag(PrintDialogAllowFlags.AllowPrintToFile);
			dialog.LookAndFeel.ParentLookAndFeel = lookAndFeel;
			return dialog;
		}
	}
}
