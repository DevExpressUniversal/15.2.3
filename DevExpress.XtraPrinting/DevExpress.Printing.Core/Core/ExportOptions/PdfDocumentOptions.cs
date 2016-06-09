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
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Data;
using DevExpress.Compatibility.System;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraPrinting {
	[
	TypeConverter(typeof(LocalizableObjectConverter)),
	DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.PdfDocumentOptions"),
	]
	public class PdfDocumentOptions : ICloneable {
		const string productName = "Developer Express Inc. DXperience (tm)";
		public static readonly string Producer;
		static PdfDocumentOptions() {
#if DEBUGTEST
			Producer = productName;
#else
			string version = AssemblyInfo.Version;
			version = "v" + version.Remove(version.Length - 2, 2);
			Producer = productName + " " + version;
#endif
		}
		string author = string.Empty;
		string application = string.Empty;
		string title = string.Empty;
		string subject = string.Empty;
		string keywords = string.Empty;
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PdfDocumentOptionsAuthor"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.PdfDocumentOptions.Author"),
		DefaultValue(""),
		Localizable(true),
		XtraSerializableProperty,
		]
		public string Author { get { return author; } set { author = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PdfDocumentOptionsApplication"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.PdfDocumentOptions.Application"),
		DefaultValue(""),
		Localizable(true),
		XtraSerializableProperty,
		]
		public string Application { get { return application; } set { application = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PdfDocumentOptionsTitle"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.PdfDocumentOptions.Title"),
		DefaultValue(""),
		Localizable(true),
		XtraSerializableProperty,
		]
		public string Title { get { return title; } set { title = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PdfDocumentOptionsSubject"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.PdfDocumentOptions.Subject"),
		DefaultValue(""),
		Localizable(true),
		XtraSerializableProperty,
		]
		public string Subject { get { return subject; } set { subject = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PdfDocumentOptionsKeywords"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.PdfDocumentOptions.Keywords"),
		DefaultValue(""),
		Localizable(true),
		XtraSerializableProperty,
		]
		public string Keywords { get { return keywords; } set { keywords = value; } }
		public void Assign(PdfDocumentOptions documentOptions) {
			if(documentOptions == null)
				throw new ArgumentNullException("documentOptions");
			this.author = documentOptions.author;
			this.application = documentOptions.application;
			this.title = documentOptions.title;
			this.subject = documentOptions.subject;
			this.keywords = documentOptions.keywords;
		}
		public object Clone() {
			PdfDocumentOptions documentOptions = new PdfDocumentOptions();
			documentOptions.Assign(this);
			return documentOptions;
		}
		internal bool ShouldSerialize() {
			return author != string.Empty || application != string.Empty || title != string.Empty ||
				subject != string.Empty || keywords != string.Empty;
		}
	}
}
