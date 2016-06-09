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

using DevExpress.XtraEditors;
using DevExpress.Pdf.Drawing;
namespace DevExpress.XtraPdfViewer.Native {
	public class PdfTextEditController : PdfTextEditBasedEditorContoller {
		readonly PdfTextEdit textEdit;
		readonly PdfTextEditSettings settings;
		protected override BaseControl Control { get { return textEdit; } }
		protected override TextEdit Editor { get { return textEdit; } }
		protected override object Value { get { return textEdit.Text; } }
		public PdfTextEditController(PdfViewer viewer, PdfTextEditSettings settings, IPdfViewerValueEditingCallBack callback)
			: base(viewer, settings, callback) {
			this.settings = settings;
			textEdit = new PdfTextEdit(this);
		}
		public override void SetUp() {
			textEdit.Text = settings.InitialText;
			textEdit.Properties.MaxLength = settings.MaxLen;
			base.SetUp();
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if (disposing)
				textEdit.Dispose();
		}
	}
}
