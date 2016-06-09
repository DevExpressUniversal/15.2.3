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
using DevExpress.XtraEditors;
using DevExpress.Pdf.Drawing;
namespace DevExpress.XtraPdfViewer.Native {
	public class PdfMemoEditController : PdfTextEditBasedEditorContoller {
		readonly PdfMemoEdit memoEdit;
		readonly PdfTextEditSettings settings;
		protected override BaseControl Control { get { return memoEdit; } }
		protected override TextEdit Editor { get { return memoEdit; } }
		protected override object Value {
			get {
				string text = memoEdit.Text;
				int lineCount = memoEdit.MaskBox.GetLineFromCharIndex(text.Length) + 1;
				string result = String.Empty;
				for (int i = 0; i < lineCount; i++) {
					int start = memoEdit.MaskBox.GetFirstCharIndexFromLine(i);
					int end = i < lineCount - 1 ? memoEdit.MaskBox.GetFirstCharIndexFromLine(i + 1) : text.Length;
					string line = text.Substring(start, end - start);
					result = line.Contains(Environment.NewLine) || i == lineCount - 1 ? result + line : result + line + Environment.NewLine;
				}
				return result;
			}
		}
		public override void SetUp() {
			memoEdit.Properties.MaxLength = settings.MaxLen;
			base.SetUp();
		}
		public PdfMemoEditController(PdfViewer viewer, PdfTextEditSettings settings, IPdfViewerValueEditingCallBack callback)
			: base(viewer, settings, callback) {
			memoEdit = new PdfMemoEdit(this);
			this.settings = settings;
			memoEdit.Text = settings.InitialText;
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if (disposing)
				memoEdit.Dispose();
		}
	}
}
