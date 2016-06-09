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

using System.Xml;
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using System;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region ProtectedRangesDestination
	public class ProtectedRangesDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("protectedRange", OnProtectedRange);
			return result;
		}
		#endregion
		public ProtectedRangesDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		static Destination OnProtectedRange(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ProtectedRangeDestination(importer);
		}
	}
	#endregion
	#region ProtectedRangeDestination
	public class ProtectedRangeDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		public ProtectedRangeDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string name = Importer.ReadAttribute(reader, "name");
			if(string.IsNullOrEmpty(name))
				Importer.ThrowInvalidFile("Protected range name is null or empty");
			CellRangeBase cellRange = Importer.GetWpSTSqref(reader, "sqref", Importer.CurrentWorksheet);
			if(cellRange == null)
				Importer.ThrowInvalidFile("Protected range sqref is not specified");
			ModelProtectedRange protectedRange = new ModelProtectedRange(name, cellRange);
			protectedRange.SecurityDescriptor = Importer.ReadAttribute(reader, "securityDescriptor");
			protectedRange.Credentials = Importer.ReadProtection(reader);
			Importer.CurrentWorksheet.ProtectedRanges.Add(protectedRange);
		}
	}
	#endregion
}
