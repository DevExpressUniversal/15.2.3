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

using System.Windows.Forms;
using DevExpress.Utils.Commands;
using DevExpress.XtraEditors;
using DevExpress.XtraBars;
using DevExpress.XtraPdfViewer.Localization;
namespace DevExpress.XtraPdfViewer.Commands {
	public class PdfCopyCommand : PdfViewerCommand {
		public override XtraPdfViewerStringId MenuCaptionStringId { get { return XtraPdfViewerStringId.CommandCopyCaption; } }
		public override XtraPdfViewerStringId DescriptionStringId { get { return XtraPdfViewerStringId.CommandCopyDescription; } }
		public override PdfViewerCommandId Id { get { return PdfViewerCommandId.Copy; } }
		public override string ImageName { get { return "Copy"; } }
		protected override BarShortcut ItemShortcut { get { return new BarShortcut(Keys.Control | Keys.C); } }
		public PdfCopyCommand(PdfViewer control) : base(control) {
		}
		protected override void ExecuteInternal() {
			PdfViewer viewer = Control;
			try {
				viewer.CopyToClipboard();
			}
			catch {
				XtraMessageBox.Show(viewer.LookAndFeel, viewer.ParentForm, XtraPdfViewerLocalizer.GetString(XtraPdfViewerStringId.MessageClipboardError), 
					XtraPdfViewerLocalizer.GetString(XtraPdfViewerStringId.MessageErrorCaption), MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			PdfViewer viewer = Control;
			state.Enabled = viewer != null && viewer.Document != null && viewer.HasSelection;
		}
	}
}
