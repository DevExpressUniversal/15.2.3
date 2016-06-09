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

using System.Collections.Generic;
using System.Reflection;
using System.Resources;
using DevExpress.Utils;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
using System.Globalization;
using System;
namespace DevExpress.XtraSpreadsheet.Localization {
	#region XtraSpreadsheetCellErrorNameStringId
	public enum XtraSpreadsheetCellErrorNameStringId {
		ValueNotAvailable = 0,
		Number = 1,
		Name = 2,
		Reference = 3,
		Value = 4,
		DivisionByZero = 5,
		NullIntersection = 6,
	}
	#endregion
	#region XtraSpreadsheetCellErrorNameLocalizer
	public class XtraSpreadsheetCellErrorNameLocalizer : XtraLocalizer<XtraSpreadsheetCellErrorNameStringId> {
		static XtraSpreadsheetCellErrorNameLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<XtraSpreadsheetCellErrorNameStringId>(CreateDefaultLocalizer()));
		}
		#region PopulateStringTable
		protected override void PopulateStringTable() {
			AddString(XtraSpreadsheetCellErrorNameStringId.Value, "#VALUE!");
			AddString(XtraSpreadsheetCellErrorNameStringId.DivisionByZero, "#DIV/0!");
			AddString(XtraSpreadsheetCellErrorNameStringId.Number, "#NUM!");
			AddString(XtraSpreadsheetCellErrorNameStringId.Name, "#NAME?");
			AddString(XtraSpreadsheetCellErrorNameStringId.Reference, "#REF!");
			AddString(XtraSpreadsheetCellErrorNameStringId.ValueNotAvailable, "#N/A");
			AddString(XtraSpreadsheetCellErrorNameStringId.NullIntersection, "#NULL!");
		}
		#endregion
		public static XtraLocalizer<XtraSpreadsheetCellErrorNameStringId> CreateDefaultLocalizer() {
			return new XtraSpreadsheetCellErrorNameResLocalizer();
		}
		public static string GetString(XtraSpreadsheetCellErrorNameStringId id) {
			return Active.GetLocalizedString(id);
		}
		public override XtraLocalizer<XtraSpreadsheetCellErrorNameStringId> CreateResXLocalizer() {
			return new XtraSpreadsheetCellErrorNameResLocalizer();
		}
		protected override void AddString(XtraSpreadsheetCellErrorNameStringId id, string str) {
			Dictionary<XtraSpreadsheetCellErrorNameStringId, string> table = XtraLocalizierHelper<XtraSpreadsheetCellErrorNameStringId>.GetStringTable(this);
			table[id] = str;
		}
	}
	#endregion
	#region XtraSpreadsheetCellErrorNameResLocalizer
	public class XtraSpreadsheetCellErrorNameResLocalizer : XtraResXLocalizer<XtraSpreadsheetCellErrorNameStringId> {
		static XtraSpreadsheetCellErrorNameResLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<XtraSpreadsheetCellErrorNameStringId>(CreateDefaultLocalizer()));
		}
		public XtraSpreadsheetCellErrorNameResLocalizer()
			: base(new XtraSpreadsheetCellErrorNameLocalizer()) {
		}
		public static XtraLocalizer<XtraSpreadsheetCellErrorNameStringId> CreateDefaultLocalizer() {
			return new XtraSpreadsheetCellErrorNameResLocalizer();
		}
		protected override ResourceManager CreateResourceManagerCore() {
#if DXPORTABLE
			return new ResourceManager("DevExpress.Spreadsheet.Core.LocalizationCellErrorNamesRes", typeof(XtraSpreadsheetCellErrorNameResLocalizer).GetAssembly());
#else
			return new ResourceManager("DevExpress.XtraSpreadsheet.LocalizationCellErrorNamesRes", typeof(XtraSpreadsheetCellErrorNameResLocalizer).GetAssembly());
#endif
		}
#if DXPORTABLE
		public static string GetString(XtraSpreadsheetCellErrorNameStringId id, CultureInfo culture) {
			return XtraResXLocalizer<XtraSpreadsheetCellErrorNameStringId>.GetLocalizedStringFromResources<XtraSpreadsheetCellErrorNameStringId, XtraSpreadsheetCellErrorNameResLocalizer>(
				id,
				culture,
				() => Active as XtraSpreadsheetCellErrorNameResLocalizer,
				(stringId) => XtraSpreadsheetCellErrorNameLocalizer.GetString(stringId)
			);
		}
#else
		[ThreadStatic] static CultureInfo lastCulture;
		[ThreadStatic] static ResourceSet lastSet;
		public static string GetString(XtraSpreadsheetCellErrorNameStringId id, CultureInfo culture) {
			if (culture == lastCulture)
				return GetString(id, lastSet);
			XtraSpreadsheetCellErrorNameLocalizer.GetString(id); 
			lastCulture = culture;
			lastSet = null;
			XtraSpreadsheetCellErrorNameResLocalizer localizer = XtraSpreadsheetCellErrorNameLocalizer.Active as XtraSpreadsheetCellErrorNameResLocalizer;
			if (localizer == null)
				return XtraSpreadsheetCellErrorNameLocalizer.GetString(id);
			lastSet = localizer.Manager.GetResourceSet(culture, true, true);
			return GetString(id, lastSet);
		}
		static string GetString(XtraSpreadsheetCellErrorNameStringId id, ResourceSet set) {
			if (set == null)
				return XtraSpreadsheetCellErrorNameLocalizer.GetString(id);
			string resStr = String.Format("{0}.{1}", typeof(XtraSpreadsheetCellErrorNameStringId).Name, id.ToString());
			string result = set.GetString(resStr);
			if (!String.IsNullOrEmpty(result))
				return result;
			return XtraSpreadsheetCellErrorNameLocalizer.GetString(id);
		}
#endif
	}
#endregion
}
