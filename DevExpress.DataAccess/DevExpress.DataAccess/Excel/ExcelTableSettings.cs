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

using System.Xml.Linq;
using DevExpress.DataAccess.Native;
using DevExpress.SpreadsheetSource;
using DevExpress.Utils;
namespace DevExpress.DataAccess.Excel {
	public sealed class ExcelTableSettings : ExcelSettingsBase {
		const string xml_TableName = "TableName";
		public ExcelTableSettings() {}
		public ExcelTableSettings(string tableName) {
			TableName = tableName;
		}
		public string TableName { get; set; }
		#region Equality members
		bool Equals(ExcelTableSettings other) {
			return string.Equals(TableName, other.TableName);
		}
		public override bool Equals(object obj) {
			var other = obj as ExcelTableSettings;
			return other != null && Equals(other);
		}
		public override int GetHashCode() {
			return 0;
		}
		#endregion
		protected internal override ISpreadsheetDataReader CreateReader(ISpreadsheetSource source) {
			return source.GetDataReader(source.Tables[TableName].Range);
		}
		protected internal override void SaveToXml(XElement tableSettings) {
			Guard.ArgumentNotNull(tableSettings, "tableSettings");
			if(!string.IsNullOrEmpty(TableName))
				tableSettings.Add(new XAttribute(xml_TableName, TableName));
		}
		protected internal override void LoadFromXml(XElement tableSettings) {
			Guard.ArgumentNotNull(tableSettings, "tableSettings");
			TableName = tableSettings.GetAttributeValue(xml_TableName);
		}
		protected internal override ExcelSettingsBase Clone() {
			return new ExcelTableSettings(TableName);
		}
	}
}
