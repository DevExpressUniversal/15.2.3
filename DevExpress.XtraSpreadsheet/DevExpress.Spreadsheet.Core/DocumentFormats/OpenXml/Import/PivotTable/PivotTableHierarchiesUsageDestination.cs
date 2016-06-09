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
using DevExpress.XtraSpreadsheet.Model.History;
using System;
using System.Collections.Generic;
using System.Xml;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region PivotTableHierarchiesUsageDestination
	public class PivotTableHierarchiesUsageDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		readonly UndoableCollection<int> array;
		#region Handler
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("rowHierarchyUsage", OnHierarchyUsage);
			result.Add("colHierarchyUsage", OnHierarchyUsage);
			return result;
		}
		#endregion
		public PivotTableHierarchiesUsageDestination(SpreadsheetMLBaseImporter importer, UndoableCollection<int> array)
			: base(importer) {
			this.array = array;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public UndoableCollection<int> Array { get { return array; } }
		#endregion
		static PivotTableHierarchiesUsageDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotTableHierarchiesUsageDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnHierarchyUsage(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTableHierarchiesUsageDestination self = GetThis(importer);
			return new PivotTableHierarchyUsageDestination(importer, self.Array);
		}
		#endregion
	}
	#endregion
	#region PivotTableHierarchiesUsageDestination
	public class PivotTableHierarchyUsageDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly UndoableCollection<int> array;
		public PivotTableHierarchyUsageDestination(SpreadsheetMLBaseImporter importer, UndoableCollection<int> array)
			: base(importer) {
			this.array = array;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			array.AddCore(Importer.GetWpSTIntegerValue(reader, "hierarchyUsage"));
		}
	}
	#endregion
}
