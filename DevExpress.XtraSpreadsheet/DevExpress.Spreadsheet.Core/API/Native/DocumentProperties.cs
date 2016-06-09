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
using System.Collections.Generic;
namespace DevExpress.Spreadsheet {
	[Flags]
	public enum DocumentSecurity {
		None = DevExpress.XtraSpreadsheet.Model.ModelDocumentSecurity.None,
		Password = DevExpress.XtraSpreadsheet.Model.ModelDocumentSecurity.Password,
		ReadonlyRecommended = DevExpress.XtraSpreadsheet.Model.ModelDocumentSecurity.ReadonlyRecommended,
		ReadonlyEnforced = DevExpress.XtraSpreadsheet.Model.ModelDocumentSecurity.ReadonlyEnforced,
		Locked = DevExpress.XtraSpreadsheet.Model.ModelDocumentSecurity.Locked
	}
	public interface DocumentProperties {
		string Application { get; set; }
		string Manager { get; set; }
		string Company { get; set; }
		string Version { get; set; }
		DocumentSecurity Security { get; set; }
		string Title { get; set; }
		string Subject { get; set; }
		string Author { get; set; }
		string Keywords { get; set; }
		string Description { get; set; }
		string LastModifiedBy { get; set; }
		string Category { get; set; }
		DateTime Created { get; set; }
		DateTime Modified { get; set; }
		DateTime Printed { get; set; }
		DocumentCustomProperties Custom { get; }
	}
	public interface DocumentCustomProperties {
		int Count { get; }
		CellValue this[string name] { get; set; }
		IEnumerable<string> Names { get; }
		void Clear();
	}
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Spreadsheet;
	using DevExpress.XtraSpreadsheet.Model;
	partial class NativeWorkbook : DocumentProperties, DocumentCustomProperties {
		public DocumentProperties DocumentProperties { get { return this; } }
		#region DocumentProperties implementation
		ModelDocumentApplicationProperties ApplicationProperties { get { return DocumentModel.DocumentApplicationProperties; } }
		ModelDocumentCoreProperties CoreProperties { get { return DocumentModel.DocumentCoreProperties; } }
		string DocumentProperties.Application { get { return ApplicationProperties.Application; } set { ApplicationProperties.Application = value; } }
		string DocumentProperties.Manager { get { return ApplicationProperties.Manager; } set { ApplicationProperties.Manager = value; } }
		string DocumentProperties.Company { get { return ApplicationProperties.Company; } set { ApplicationProperties.Company = value; } }
		string DocumentProperties.Version { get { return ApplicationProperties.Version; } set { ApplicationProperties.Version = value; } }
		string DocumentProperties.Title { get { return CoreProperties.Title; } set { CoreProperties.Title = value; } }
		string DocumentProperties.Subject { get { return CoreProperties.Subject; } set { CoreProperties.Subject = value; } }
		string DocumentProperties.Author { get { return CoreProperties.Creator; } set { CoreProperties.Creator = value; } }
		string DocumentProperties.Keywords { get { return CoreProperties.Keywords; } set { CoreProperties.Keywords = value; } }
		string DocumentProperties.Description { get { return CoreProperties.Description; } set { CoreProperties.Description = value; } }
		string DocumentProperties.LastModifiedBy { get { return CoreProperties.LastModifiedBy; } set { CoreProperties.LastModifiedBy = value; } }
		string DocumentProperties.Category { get { return CoreProperties.Category; } set { CoreProperties.Category = value; } }
		DateTime DocumentProperties.Created { get { return CoreProperties.Created; } set { CoreProperties.Created = value; } }
		DateTime DocumentProperties.Modified { get { return CoreProperties.Modified; } set { CoreProperties.Modified = value; } }
		DateTime DocumentProperties.Printed { get { return CoreProperties.LastPrinted; } set { CoreProperties.LastPrinted = value; } }
		DocumentCustomProperties DocumentProperties.Custom { get { return this; } }
		DocumentSecurity DocumentProperties.Security { 
			get { return (DocumentSecurity)ApplicationProperties.Security; } 
			set { ApplicationProperties.Security = (ModelDocumentSecurity)value; } 
		}
		#endregion
		#region DocumentCustomProperties
		ModelDocumentCustomProperties CustomProperties { get { return DocumentModel.DocumentCustomProperties; } }
		int DocumentCustomProperties.Count { get { return CustomProperties.Count; } }
		CellValue DocumentCustomProperties.this[string name] { get { return CustomProperties[name]; } set { CustomProperties[name] = value; } }
		IEnumerable<string> DocumentCustomProperties.Names { get { return CustomProperties.Names; } }
		void DocumentCustomProperties.Clear() { CustomProperties.Clear(); }
		#endregion
	}
}
