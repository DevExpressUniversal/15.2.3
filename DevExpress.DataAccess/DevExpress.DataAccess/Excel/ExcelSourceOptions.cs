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
using System.Xml.Linq;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Native;
using DevExpress.SpreadsheetSource;
namespace DevExpress.DataAccess.Excel {
	public sealed class ExcelSourceOptions : ExcelSourceOptionsBase {
		const string xml_Password = "Password";
		const string xml_SkipHiddenColumns = "SkipHiddenColumns";
		const string xml_SkipHiddenRows = "SkipHiddenRows";
		const string xml_ImportSettings = "ImportSettings";
		const string xml_ImportSettingsType = "Type";
		public ExcelSourceOptions() {
			SkipHiddenColumns = true;
			SkipHiddenRows = true;
		}
		public ExcelSourceOptions(ExcelSettingsBase importSettings) {
			ImportSettings = importSettings;
		}
		ExcelSourceOptions(ExcelSourceOptions other) : base(other) {
			Password = !string.IsNullOrEmpty(other.PasswordInternal) ? other.PasswordInternal : other.Password;
			SkipHiddenRows = other.SkipHiddenRows;
			SkipHiddenColumns = other.SkipHiddenColumns;
			UseFirstRowAsHeader = other.UseFirstRowAsHeader;
			if(other.ImportSettings != null)
				ImportSettings = other.ImportSettings.Clone();
		}
		internal string PasswordInternal;
		[DefaultValue(null)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		[LocalizableCategory(DataAccessStringId.PropertyGridDataCategoryName)]
		public ExcelSettingsBase ImportSettings { get; set; }
		[DefaultValue(null)]
		public string Password { get; set; }
		[DefaultValue(true)]
		public bool SkipHiddenColumns { get; set; }
		[DefaultValue(true)]
		public bool SkipHiddenRows { get; set; }
		internal override bool IsDefault { get { return base.IsDefault && string.IsNullOrEmpty(Password) && SkipHiddenRows && SkipHiddenColumns; } }
		protected internal override ExcelSourceOptionsBase Clone() {
			return new ExcelSourceOptions(this);
		}
		protected internal override SpreadsheetSourceOptions GetSpreadsheetSourceOptions() {
			return new SpreadsheetSourceOptions {
				Password = Password,
				SkipEmptyRows = SkipEmptyRows,
				SkipHiddenColumns = SkipHiddenColumns,
				SkipHiddenRows = SkipHiddenRows
			};
		}
		protected internal override ISpreadsheetDataReader CreateReader(ISpreadsheetSource source) {
			return ImportSettings.CreateReader(source);
		}
		protected internal override void SaveToXml(XElement options) {
			base.SaveToXml(options);
			if (!string.IsNullOrEmpty(Password))
				options.Add(new XAttribute(xml_Password, Password));
			options.Add(new XAttribute(xml_SkipHiddenColumns, SkipHiddenColumns));
			options.Add(new XAttribute(xml_SkipHiddenRows, SkipHiddenRows));
			if (ImportSettings != null) {
				var settings = new XElement(xml_ImportSettings);
				settings.Add(new XAttribute(xml_ImportSettingsType, ImportSettings.GetType().FullName));
				ImportSettings.SaveToXml(settings);
				options.Add(settings);
			}
		}
		protected internal override void LoadFromXml(XElement options) {
			base.LoadFromXml(options);
			Password = options.GetAttributeValue(xml_Password);
			SkipHiddenColumns = Convert.ToBoolean(options.GetAttributeValue(xml_SkipHiddenColumns));
			SkipHiddenRows = Convert.ToBoolean(options.GetAttributeValue(xml_SkipHiddenRows));
			var sourceSettings = options.Element(xml_ImportSettings);
			if(sourceSettings != null) {
				var settingsType = sourceSettings.GetAttributeValue(xml_ImportSettingsType);
				ExcelSettingsBase settings = (ExcelSettingsBase) Activator.CreateInstance(Type.GetType(settingsType));
				settings.LoadFromXml(sourceSettings);
				ImportSettings = settings;
			}
		}
		bool Equals(ExcelSourceOptions other) {
			return Password == other.Password
				&& SkipHiddenRows == other.SkipHiddenRows
				&& SkipHiddenColumns == other.SkipHiddenColumns;
		}
		public override bool Equals(object obj) {
			if(!base.Equals(obj))
				return false;
			var other = obj as ExcelSourceOptions;
			return other != null && Equals(other);
		}
		public override int GetHashCode() {
			return 0;
		}
	}
}
