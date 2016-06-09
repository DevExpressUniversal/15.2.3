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
namespace DevExpress.XtraPrinting.Export.Pdf {
	public class PdfInfo : PdfDocumentDictionaryObject {
		string author = String.Empty;
		string application = PdfDocumentOptions.Producer;
		string title = String.Empty;
		string subject = String.Empty;
		string keywords = String.Empty;
		DateTime creationDate;
		public string Author { get { return author; } set { this.author = value; } }
		public string Application { get { return application; } set { this.application = value; } }
		public string Title { get { return title; } set { this.title = value; } }
		public string Subject { get { return subject; } set { this.subject = value; } }
		public string Keywords { get { return keywords; } set { this.keywords = value; } }
		public DateTime CreationDate { get { return creationDate; } set { this.creationDate = value; } }
		public PdfInfo(bool compressed) : base(compressed) {
		}
		void FillUpValue(string name, string value) {
			if(value != null && value != String.Empty)
				Dictionary.Add(name, new PdfTextUnicode(value));
		}
		public override void FillUp() {
			FillUpValue("Producer", PdfDocumentOptions.Producer);
			FillUpValue("Author", this.author);
			FillUpValue("Creator", this.application);
			FillUpValue("Title", this.title);
			FillUpValue("Subject", this.subject);
			FillUpValue("Keywords", this.keywords);
#if !DEBUGTEST
			Dictionary.Add("CreationDate", new PdfDate(creationDate));
#endif
		}
	}
}
