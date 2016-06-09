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
using System.Text;
using System.ComponentModel;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting.Native;
using DevExpress.Data;
namespace DevExpress.XtraPrinting {
	[
	TypeConverter(typeof(LocalizableObjectConverter)),
	DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.XpsDocumentOptions"),
	]
	public class XpsDocumentOptions : ICloneable {		
		string creator = string.Empty;
		string category = string.Empty;
		string title = string.Empty;
		string subject = string.Empty;
		string keywords = string.Empty;
		string version = string.Empty;
		string description = string.Empty;
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("XpsDocumentOptionsCreator"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.XpsDocumentOptions.Creator"),
		DefaultValue(""),
		Localizable(true),
		XtraSerializableProperty,
		]
		public string Creator { get { return creator; } set { creator = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("XpsDocumentOptionsCategory"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.XpsDocumentOptions.Category"),
		DefaultValue(""),
		Localizable(true),
		XtraSerializableProperty,
		]
		public string Category { get { return category; } set { category = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("XpsDocumentOptionsTitle"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.XpsDocumentOptions.Title"),
		DefaultValue(""),
		Localizable(true),
		XtraSerializableProperty,
		]
		public string Title { get { return title; } set { title = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("XpsDocumentOptionsSubject"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.XpsDocumentOptions.Subject"),
		DefaultValue(""),
		Localizable(true),
		XtraSerializableProperty,
		]
		public string Subject { get { return subject; } set { subject = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("XpsDocumentOptionsKeywords"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.XpsDocumentOptions.Keywords"),
		DefaultValue(""),
		Localizable(true),
		XtraSerializableProperty,
		]
		public string Keywords { get { return keywords; } set { keywords = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("XpsDocumentOptionsVersion"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.XpsDocumentOptions.Version"),
		DefaultValue(""),
		Localizable(true),
		XtraSerializableProperty,
		]
		public string Version { get { return version; } set { version = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("XpsDocumentOptionsDescription"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.XpsDocumentOptions.Description"),
		DefaultValue(""),
		Localizable(true),
		XtraSerializableProperty,
		]
		public string Description { get { return description; } set { description = value; } }
		public void Assign(XpsDocumentOptions xpsDocumentOptions) {
			if(xpsDocumentOptions == null)
				throw new ArgumentNullException("xpsDocumentOptions");
			this.creator = xpsDocumentOptions.creator;
			this.category = xpsDocumentOptions.category;
			this.title = xpsDocumentOptions.title;
			this.subject = xpsDocumentOptions.subject;
			this.keywords = xpsDocumentOptions.keywords;
			this.version = xpsDocumentOptions.version;
			this.description = xpsDocumentOptions.description;
		}
		public object Clone() {
			XpsDocumentOptions documentOptions = new XpsDocumentOptions();
			documentOptions.Assign(this);
			return documentOptions;
		}
		internal bool ShouldSerialize() {
			return creator != string.Empty || category != string.Empty || title != string.Empty ||
				subject != string.Empty || keywords != string.Empty || version != string.Empty ||
				description != string.Empty;
		}
	}
}
