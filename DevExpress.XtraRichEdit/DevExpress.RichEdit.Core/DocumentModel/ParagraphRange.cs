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
using System.Text;
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Layout.Engine;
namespace DevExpress.XtraRichEdit.Model {
	#region ParagraphRun
	public class ParagraphRun : TextRunBase {
		public ParagraphRun(Paragraph paragraph)
			: base(paragraph) {
		}
		public override int Length {
			get { return base.Length; }
			set {
				if (value != 1) {
					Exceptions.ThrowArgumentException("Length", value);
				}
			}
		}
		public override bool CanPlaceCaretBefore { get { return true; } }
		protected internal override string GetText() {
			return String.Empty;
		}
		protected internal override string GetTextFast(ChunkedStringBuilder growBuffer) {
			return String.Empty;
		}
		protected internal override string GetRawTextFast(ChunkedStringBuilder growBuffer) {
			return "\n";
		}
		public override string GetPlainText(ChunkedStringBuilder growBuffer) {
			return "\r\n";
		}
		protected internal override string GetPlainText(ChunkedStringBuilder growBuffer, int from, int to) {
			return GetPlainText(growBuffer);
		}
		public override bool CanJoinWith(TextRunBase nextRun) {
			Guard.ArgumentNotNull(nextRun, "nextRun");
			return false;
		}
		protected internal override void Measure(BoxInfo boxInfo, IObjectMeasurer measurer) {
			Exceptions.ThrowInternalException();
		}
		protected internal override bool TryAdjustEndPositionToFit(BoxInfo boxInfo, int maxWidth, IObjectMeasurer measurer) {
			return false;
		}
		public override void Export(IDocumentModelExporter exporter) {
			exporter.Export(this);
		}
		public override TextRunBase Copy(DocumentModelCopyManager copyManager) {
			Exceptions.ThrowInternalException();
			return null;
		}
	}
	#endregion
}
