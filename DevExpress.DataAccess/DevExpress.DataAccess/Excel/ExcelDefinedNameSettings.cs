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

using System.ComponentModel;
using System.Xml.Linq;
using DevExpress.DataAccess.Native;
using DevExpress.SpreadsheetSource;
using DevExpress.Utils;
namespace DevExpress.DataAccess.Excel {
	public sealed class ExcelDefinedNameSettings : ExcelSettingsBase {
		const string xml_DefinedName = "DefinedName";
		const string xml_Scope = "Scope";
		public ExcelDefinedNameSettings() {}
		public ExcelDefinedNameSettings(string definedName) : this(definedName, null) {}
		public ExcelDefinedNameSettings(string definedName, string scope) {
			DefinedName = definedName;
			Scope = scope;
		}
		public string DefinedName { get; set; }
		[DefaultValue(null)]
		public string Scope { get; set; }
		#region Equality members
		bool Equals(ExcelDefinedNameSettings other) {
			return string.Equals(DefinedName, other.DefinedName) 
				&& string.Equals(Scope, other.Scope);
		}
		public override bool Equals(object obj) {
			var other = obj as ExcelDefinedNameSettings;
			return other != null && Equals(other);
		}
		public override int GetHashCode() {
			return 0;
		}
		#endregion
		protected internal override ISpreadsheetDataReader CreateReader(ISpreadsheetSource source) {
			return source.GetDataReader(source.DefinedNames.FindBy(DefinedName, Scope));
		}
		protected internal override void SaveToXml(XElement definedNamesSettings) {
			Guard.ArgumentNotNull(definedNamesSettings, "definedNamesSettings");
			if(!string.IsNullOrEmpty(DefinedName))
				definedNamesSettings.Add(new XAttribute(xml_DefinedName, DefinedName));
			if(!string.IsNullOrEmpty(Scope))
				definedNamesSettings.Add(new XAttribute(xml_Scope, Scope));
		}
		protected internal override void LoadFromXml(XElement definedNamesSettings) {
			Guard.ArgumentNotNull(definedNamesSettings, "definedNamesSettings");
			DefinedName = definedNamesSettings.GetAttributeValue(xml_DefinedName);
			Scope = definedNamesSettings.GetAttributeValue(xml_Scope);
		}
		protected internal override ExcelSettingsBase Clone() {
			return new ExcelDefinedNameSettings(DefinedName, Scope);
		}
	}
}
