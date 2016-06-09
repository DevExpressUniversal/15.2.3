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
using System.ComponentModel;
using DevExpress.Data;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting.Native;
using DevExpress.Compatibility.System;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraPrinting {
	[TypeConverter(typeof(LocalizableObjectConverter)),
	DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.XlDocumentOptions")]
	public class XlDocumentOptions : ICloneable {
		string title = string.Empty;
		string subject = string.Empty;
		string tags = string.Empty;
		string category = string.Empty;
		string comments = string.Empty;
		string author = string.Empty;
		string application = string.Empty;
		string company = string.Empty;
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.XlDocumentOptions.Title"),
		DefaultValue(""),
		Localizable(true),
		XtraSerializableProperty]
		public string Title { get { return title; } set { title = value; } }
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.XlDocumentOptions.Subject"),
		DefaultValue(""),
		Localizable(true),
		XtraSerializableProperty]
		public string Subject { get { return subject; } set { subject = value; } }
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.XlDocumentOptions.Tags"),
		DefaultValue(""),
		Localizable(true),
		XtraSerializableProperty]
		public string Tags { get { return tags; } set { tags = value; } }
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.XlDocumentOptions.Category"),
		DefaultValue(""),
		Localizable(true),
		XtraSerializableProperty]
		public string Category { get { return category; } set { category = value; } }
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.XlDocumentOptions.Comments"),
		DefaultValue(""),
		Localizable(true),
		XtraSerializableProperty]
		public string Comments { get { return comments; } set { comments = value; } }
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.XlDocumentOptions.Author"),
		DefaultValue(""),
		Localizable(true),
		XtraSerializableProperty]
		public string Author { get { return author; } set { author = value; } }
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.XlDocumentOptions.Application"),
		DefaultValue(""),
		Localizable(true),
		XtraSerializableProperty]
		public string Application { get { return application; } set { application = value; } }
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.XlDocumentOptions.Company"),
		DefaultValue(""),
		Localizable(true),
		XtraSerializableProperty]
		public string Company { get { return company; } set { company = value; } }
		public void Assign(XlDocumentOptions documentOptions) {
			if(documentOptions == null)
				throw new ArgumentNullException("documentOptions");
			this.title = documentOptions.title;
			this.subject = documentOptions.subject;
			this.tags = documentOptions.tags;
			this.category = documentOptions.category;
			this.comments = documentOptions.comments;
			this.author = documentOptions.author;
			this.application = documentOptions.application;
			this.company = documentOptions.company;
		}
		public object Clone() {
			XlDocumentOptions documentOptions = new XlDocumentOptions();
			documentOptions.Assign(this);
			return documentOptions;
		}
		internal bool ShouldSerialize() {
			return title != string.Empty || subject != string.Empty || tags != string.Empty ||
				category != string.Empty || comments != string.Empty || author != string.Empty ||
				application != string.Empty || company != string.Empty;
		}
	}
}
