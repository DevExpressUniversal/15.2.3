﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
using DevExpress.Pdf;
using DevExpress.Utils.Commands;
using DevExpress.XtraBars;
using DevExpress.XtraPdfViewer.Localization;
namespace DevExpress.XtraPdfViewer.Commands {
	public class PdfSelectAllCommand : PdfViewerCommand {
		public override PdfViewerCommandId Id { get { return PdfViewerCommandId.SelectAll; } }
		public override XtraPdfViewerStringId MenuCaptionStringId { get { return XtraPdfViewerStringId.CommandSelectAllCaption; } }
		public override XtraPdfViewerStringId DescriptionStringId { get { return XtraPdfViewerStringId.CommandSelectAllDescription; } }
		public override string ImageName { get { return "SelectAll"; } }
		protected override BarShortcut ItemShortcut { get { return new BarShortcut(Keys.Control | Keys.A); } }
		public PdfSelectAllCommand(PdfViewer control) : base(control) {
		}
		public override void UpdateUIState(ICommandUIState state) {
			PdfDocument document = Control.Document;
			state.Enabled = document != null && document.Pages.Count > 0;
		}
		protected override void ExecuteInternal() {
			Control.SelectAllText();
		}
	}
}
