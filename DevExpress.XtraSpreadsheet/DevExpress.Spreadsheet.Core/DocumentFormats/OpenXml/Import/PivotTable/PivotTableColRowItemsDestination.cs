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

using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using System;
using System.Collections.Generic;
using System.Xml;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region PivotTableColRowItemsDestination
	public class PivotTableColRowItemsDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		readonly PivotLayoutItems items;
		#region Handler
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("i", OnRowItems);
			return result;
		}
		#endregion
		public PivotTableColRowItemsDestination(SpreadsheetMLBaseImporter importer, PivotLayoutItems items)
			: base(importer) {
				this.items = items;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		static PivotTableColRowItemsDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotTableColRowItemsDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnRowItems(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTableColRowItemsDestination self = GetThis(importer);
			return new PivotTableRowItemsDestination(importer, self.items);
		}
		#endregion
	}
	#endregion
	#region PivotTableRowItemsDestination
	public class PivotTableRowItemsDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		readonly PivotLayoutItems items;
		int dataFieldIndex;
		int repeatedItemsCount;
		PivotFieldItemType itemType;
		List<int> memberPropertyIndex;
		#region Handler
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("x", OnMemberPropertyIndex);
			return result;
		}
		#endregion
		public PivotTableRowItemsDestination(SpreadsheetMLBaseImporter importer, PivotLayoutItems items)
			: base(importer) {
			this.items = items;
			memberPropertyIndex = new List<int>();
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public List<int> MemberPropertyIndex { get { return memberPropertyIndex; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
				itemType = Importer.GetWpEnumValue<PivotFieldItemType>(reader, "t", PivotTablePivotItemCollectionDestination.reversePivoItemTypeTable, PivotFieldItemType.Data);
				repeatedItemsCount = Importer.GetWpSTIntegerValue(reader, "r", 0);
				dataFieldIndex = Importer.GetWpSTIntegerValue(reader, "i", 0);
		}
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
			items.Add(PivotLayoutItemFactory.CreateInstance(MemberPropertyIndex.ToArray(), repeatedItemsCount, dataFieldIndex, itemType));
		}
		static PivotTableRowItemsDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotTableRowItemsDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnMemberPropertyIndex(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTableRowItemsDestination self = GetThis(importer);
			return new IntegerValueDestination(importer, self.SetMemberPropertyIndex, "v", 0);
		}
		void SetMemberPropertyIndex(int value) {
			MemberPropertyIndex.Add(value);
		}
		#endregion
	}
	#endregion
}
