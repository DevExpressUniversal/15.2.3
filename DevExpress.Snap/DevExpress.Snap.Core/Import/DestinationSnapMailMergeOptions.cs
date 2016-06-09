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
using System.Xml;
using DevExpress.Data;
using DevExpress.Office;
using DevExpress.Snap.Core.API;
using DevExpress.XtraRichEdit.Import.OpenXml;
namespace DevExpress.Snap.Core.Import {
	#region SnapMailMergeOptionsDestination
	public class SnapMailMergeOptionsDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("dataSourceName", OnDataSourceName);
			result.Add("dataMember", OnDataMember);
			result.Add("currentRecordIndex", OnCurrentRecordIndex);
			result.Add("filterString", OnFilterString);
			result.Add("sorting", OnSorting);
			return result;
		}
		public SnapMailMergeOptionsDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static Destination OnDataSourceName(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new SnapMailMergeOptionsDataSourceNameDestination((SnapImporter)importer);
		}
		static Destination OnDataMember(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new SnapMailMergeOptionsDataMemberDestination((SnapImporter)importer);
		}
		static Destination OnCurrentRecordIndex(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new SnapMailMergeOptionsCurrentRecordIndexDestination((SnapImporter)importer);
		}
		static Destination OnFilterString(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new SnapMailMergeOptionsFilterStringDestination((SnapImporter)importer);
		}
		static Destination OnSorting(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new SnapMailMergeOptionsSortingDestination(importer);
		}
	}
	#endregion
	#region SnapMailMergeOptionsDataSourceNameDestination
	public class SnapMailMergeOptionsDataSourceNameDestination : SnapLeafElementDestination {
		public SnapMailMergeOptionsDataSourceNameDestination(SnapImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Importer.DocumentModel.SnapMailMergeVisualOptions.DataSourceName = Importer.ReadDxStringAttr("val", reader);
		}
	}
	#endregion
	#region SnapMailMergeOptionsDataMemberDestination
	public class SnapMailMergeOptionsDataMemberDestination : SnapLeafElementDestination {
		public SnapMailMergeOptionsDataMemberDestination(SnapImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Importer.DocumentModel.SnapMailMergeVisualOptions.DataMember = Importer.ReadDxStringAttr("val", reader);
		}
	}
	#endregion
	#region SnapMailMergeOptionsCurrentRecordIndexDestination
	public class SnapMailMergeOptionsCurrentRecordIndexDestination : SnapLeafElementDestination {
		public SnapMailMergeOptionsCurrentRecordIndexDestination(SnapImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Importer.DocumentModel.SnapMailMergeVisualOptions.CurrentRecordIndex = Importer.ReadDxIntAttr("val", reader);
		}
	}
	#endregion
	#region SnapMailMergeOptionsFilterStringDestination
	public class SnapMailMergeOptionsFilterStringDestination : SnapLeafElementDestination {
		public SnapMailMergeOptionsFilterStringDestination(SnapImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Importer.DocumentModel.SnapMailMergeVisualOptions.FilterString = Importer.ReadDxStringAttr("val", reader);
		}
	}
	#endregion
	#region SnapMailMergeOptionsSortingDestination
	public class SnapMailMergeOptionsSortingDestination : ElementDestination {
		static readonly ElementHandlerTable table = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("sortingItem", OnSortingItem);
			return result;
		}
		public SnapMailMergeOptionsSortingDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return table; } }
		static Destination OnSortingItem(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new SnapMailMergeOptionsSortingItemDestination((SnapImporter)importer);
		}
	}
	#endregion
	#region SnapMailMergeOptionsSortingItemDestination
	public class SnapMailMergeOptionsSortingItemDestination : SnapLeafElementDestination {
		public SnapMailMergeOptionsSortingItemDestination(SnapImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string fieldName = Importer.ReadDxStringAttr("fieldName", reader);
			string sortOrderString = Importer.ReadDxStringAttr("sortOrder", reader);
			ColumnSortOrder sortOrder = (!String.IsNullOrEmpty(sortOrderString)) ? (ColumnSortOrder)Enum.Parse(typeof(ColumnSortOrder), sortOrderString) 
				: ColumnSortOrder.None;
			Importer.DocumentModel.SnapMailMergeVisualOptions.Sorting.Add(new SnapListGroupParam(fieldName, sortOrder));
		}
	}
	#endregion
}
