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
	#region PivotTablePivotHierarchyCollectionDestination
	public class PivotTablePivotHierarchyCollectionDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		readonly Worksheet worksheet;
		readonly PivotHierarchyCollection hierarchies;
		#region Handler
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("pivotHierarchy", OnPivotHierarchy);
			return result;
		}
		#endregion
		public PivotTablePivotHierarchyCollectionDestination(SpreadsheetMLBaseImporter importer, PivotHierarchyCollection hierarchies, Worksheet worksheet)
			: base(importer) {
			this.worksheet = worksheet;
			this.hierarchies = hierarchies;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public PivotHierarchyCollection Hierarchies { get { return hierarchies; } }
		public Worksheet Worksheet { get { return worksheet; } }
		#endregion
		static PivotTablePivotHierarchyCollectionDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotTablePivotHierarchyCollectionDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnPivotHierarchy(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTablePivotHierarchyCollectionDestination self = GetThis(importer);
			return new PivotTablePivotHierarchyDestination(importer, self.Hierarchies, self.Worksheet);
		}
		#endregion
	}
	#endregion
	#region PivotTablePivotHierarchyDestination
	public class PivotTablePivotHierarchyDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		readonly Worksheet worksheet;
		readonly PivotHierarchyCollection hierarchies;
		PivotHierarchy pivotHierarchy;
		#region Handler
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("members", OnMembers);
			result.Add("mps", OnMemberProperties);
			return result;
		}
		#endregion
		public PivotTablePivotHierarchyDestination(SpreadsheetMLBaseImporter importer, PivotHierarchyCollection hierarchies, Worksheet worksheet)
			: base(importer) {
			this.worksheet = worksheet;
			this.hierarchies = hierarchies;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public PivotHierarchyCollection Hierarchies { get { return hierarchies; } }
		public PivotHierarchy PivotHierarchy { get { return pivotHierarchy; } }
		public Worksheet Worksheet { get { return worksheet; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
				pivotHierarchy = new PivotHierarchy(Worksheet.Workbook);
				Hierarchies.AddCore(pivotHierarchy);
				PivotHierarchy.SetOutlineCore(Importer.GetWpSTOnOffValue(reader, "outline", false));
				PivotHierarchy.SetMultipleItemSelectionAllowedCore(Importer.GetWpSTOnOffValue(reader, "multipleItemSelectionAllowed", false));
				PivotHierarchy.SetSubtotalTopCore(Importer.GetWpSTOnOffValue(reader, "subtotalTop", false));
				PivotHierarchy.SetShowInFieldListCore(Importer.GetWpSTOnOffValue(reader, "showInFieldList", true));
				PivotHierarchy.SetDragToRowCore(Importer.GetWpSTOnOffValue(reader, "dragToRow", true));
				PivotHierarchy.SetDragToColCore(Importer.GetWpSTOnOffValue(reader, "dragToCol", true));
				PivotHierarchy.SetDragToPageCore(Importer.GetWpSTOnOffValue(reader, "dragToPage", true));
				PivotHierarchy.SetDragToDataCore(Importer.GetWpSTOnOffValue(reader, "dragToData", false));
				PivotHierarchy.SetDragOffCore(Importer.GetWpSTOnOffValue(reader, "dragOff", true));
				PivotHierarchy.SetIncludeNewItemsInFilterCore(Importer.GetWpSTOnOffValue(reader, "includeNewItemsInFilter", false));
				string caption = Importer.GetWpSTXString(reader, "caption");
				if (!String.IsNullOrEmpty(caption))
					PivotHierarchy.SetCaptionCore(caption);
		}
		static PivotTablePivotHierarchyDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotTablePivotHierarchyDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnMembers(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTablePivotHierarchyDestination self = GetThis(importer);
			return new PivotTablePivotHierarchyMemberCollectionDestination(importer, self.PivotHierarchy, self.Worksheet);
		}
		static Destination OnMemberProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTablePivotHierarchyDestination self = GetThis(importer);
			return new PivotTablePivotHierarchyMemberPropertieCollectionDestination(importer, self.PivotHierarchy, self.Worksheet);
		}
		#endregion
	}
	#endregion
	#region PivotTablePivotHierarchyMemberCollectionDestination
	public class PivotTablePivotHierarchyMemberCollectionDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		readonly Worksheet worksheet;
		readonly PivotHierarchy pivotHierarchy;
		#region Handler
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("member", OnMember);
			return result;
		}
		#endregion
		public PivotTablePivotHierarchyMemberCollectionDestination(SpreadsheetMLBaseImporter importer, PivotHierarchy pivotHierarchy, Worksheet worksheet)
			: base(importer) {
			this.worksheet = worksheet;
			this.pivotHierarchy = pivotHierarchy;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public PivotHierarchy PivotHierarchy { get { return pivotHierarchy; } }
		public Worksheet Worksheet { get { return worksheet; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
				int level = Importer.GetWpSTIntegerValue(reader, "level", -1);
				if (level >= 0)
					PivotHierarchy.SetLevelMembersCore(level);
		}
		static PivotTablePivotHierarchyMemberCollectionDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotTablePivotHierarchyMemberCollectionDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnMember(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTablePivotHierarchyMemberCollectionDestination self = GetThis(importer);
			 return new StringValueDestination(importer, self.ReadingAttribute, "name");
		}
		#endregion
		void ReadingAttribute(string value){
			PivotHierarchy.Members.AddCore(value);
		}
	}
	#endregion
	#region PivotTablePivotHierarchyMemberPropertieCollectionDestination
	public class PivotTablePivotHierarchyMemberPropertieCollectionDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		readonly Worksheet worksheet;
		readonly PivotHierarchy pivotHierarchy;
		#region Handler
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("mp", OnMemberProperty);
			return result;
		}
		#endregion
		public PivotTablePivotHierarchyMemberPropertieCollectionDestination(SpreadsheetMLBaseImporter importer, PivotHierarchy pivotHierarchy, Worksheet worksheet)
			: base(importer) {
			this.worksheet = worksheet;
			this.pivotHierarchy = pivotHierarchy;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public PivotHierarchy PivotHierarchy { get { return pivotHierarchy; } }
		public Worksheet Worksheet { get { return worksheet; } }
		#endregion
		static PivotTablePivotHierarchyMemberPropertieCollectionDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotTablePivotHierarchyMemberPropertieCollectionDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnMemberProperty(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTablePivotHierarchyMemberPropertieCollectionDestination self = GetThis(importer);
			return new PivotTablePivotHierarchyMemberPropertieDestination(importer, self.PivotHierarchy.MemberProperties, self.Worksheet);
		}
		#endregion
	}
	#endregion
	#region PivotTablePivotHierarchyMemberPropertieDestination
	public class PivotTablePivotHierarchyMemberPropertieDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		readonly MemberPropertiesCollection memberProperties;
		readonly Worksheet worksheet;
		#region Handler
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			return result;
		}
		#endregion
		public PivotTablePivotHierarchyMemberPropertieDestination(SpreadsheetMLBaseImporter importer, MemberPropertiesCollection memberProperties, Worksheet worksheet)
			: base(importer) {
				this.worksheet = worksheet;
				this.memberProperties = memberProperties;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public MemberPropertiesCollection MemberProperties { get { return memberProperties; } }
		public Worksheet Worksheet { get { return worksheet; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
				MemberProperty mp = new MemberProperty(Worksheet.Workbook);
				MemberProperties.AddCore(mp);
				string name = Importer.GetWpSTXString(reader, "name");
				if (!String.IsNullOrEmpty(name))
					mp.SetNameCore(name);
				mp.SetShowCellCore(Importer.GetWpSTOnOffValue(reader, "showCell", false));
				mp.SetShowTipCore(Importer.GetWpSTOnOffValue(reader, "showTip", false));
				mp.SetShowAsCaptionCore(Importer.GetWpSTOnOffValue(reader, "showAsCaption", false));
				int value = Importer.GetWpSTIntegerValue(reader, "nameLen", Int32.MinValue);
				if (value != Int32.MinValue)
					mp.SetNameLenCore(value);
				value = Importer.GetWpSTIntegerValue(reader, "pPos", Int32.MinValue);
				if (value != Int32.MinValue)
					mp.SetPropertyNameCharacterIndexCore(value);
				value = Importer.GetWpSTIntegerValue(reader, "pLen", Int32.MinValue);
				if (value != Int32.MinValue)
					mp.SetPropertyNameLengthCore(value);
				value = Importer.GetWpSTIntegerValue(reader, "level", Int32.MinValue);
				if (value != Int32.MinValue)
					mp.SetLevelIndexCore(value);
				mp.SetFieldIndexCore(Importer.GetWpSTIntegerValue(reader, "field"));
		}
	}
	#endregion
}
