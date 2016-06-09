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

using DevExpress.Web.ASPxRichEdit.Internal;
using DevExpress.Web.Office.Internal;
using DevExpress.Web.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
namespace DevExpress.Web.Office {
	public class RichEditDocumentInfo : OfficeDocumentBase<RichEditWorkSession> {
		public RichEditDocumentInfo(RichEditWorkSession workSession) : base(workSession) { }
		protected internal Guid WorkSessionGuid { get { return WorkSession.ID; } }
		public DevExpress.XtraRichEdit.DocumentFormat DocumentFormat {
			get { return WorkSession.CalculateDocumentFormat(); }
		}
		public override bool Modified {
			get { return WorkSession.Modified; }
		}
		public override DateTime? LastModifyTime {
			get {
				return WorkSession.LastModifyTime;
			}
		}
		public override void SaveCopy(string filePath) {
			SaveCopyCore(() => WorkSession.RichEdit.SaveDocument(filePath, WorkSession.CalculateDocumentFormat()));
		}
		public override byte[] SaveCopy() {
			return SaveCopy(WorkSession.CalculateDocumentFormat());
		}
		public byte[] SaveCopy(DevExpress.XtraRichEdit.DocumentFormat documentFormat) {
			using(Stream stream = new MemoryStream()) {
				SaveCopyCore(() => WorkSession.RichEdit.SaveDocument(stream, documentFormat));
				return CommonUtils.GetBytesFromStream(stream);
			}
		}
		public override void SaveCopy(Stream stream) {
			SaveCopy(stream, WorkSession.CalculateDocumentFormat());
		}
		public void SaveCopy(Stream stream, DevExpress.XtraRichEdit.DocumentFormat documentFormat) {
			SaveCopyCore(() => WorkSession.RichEdit.SaveDocument(stream, documentFormat));
		}
		void SaveCopyCore(Action saveAction) {
			var currentFileName = WorkSession.RichEdit.Options.DocumentSaveOptions.CurrentFileName;
			var currentFormat = WorkSession.RichEdit.Options.DocumentSaveOptions.CurrentFormat;
			saveAction();
			WorkSession.RichEdit.Options.DocumentSaveOptions.CurrentFileName = currentFileName;
			WorkSession.RichEdit.Options.DocumentSaveOptions.CurrentFormat = currentFormat;
		}
	}
}
