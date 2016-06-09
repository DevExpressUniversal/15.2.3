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
using DevExpress.Snap.Core.Native.Data;
using ApiField = DevExpress.XtraRichEdit.API.Native.Field;
using ModelTable = DevExpress.XtraRichEdit.Model.Table;
using ModelTableCellStyle = DevExpress.XtraRichEdit.Model.TableCellStyle;
using DevExpress.Office.Utils;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.Snap.Core.API {
	public interface SnapList : SnapEntity {
		string Name { get; set; }			   
		SnapDocument RowTemplate { get; }	   
		bool KeepLastSeparator { get; set; }	
		int EditorRowLimit { get; set; }		
		SnapDocument ListHeader { get; }		
		SnapDocument ListFooter { get; }		
		SnapDocument Separator { get; }		 
		string DataSourceName { get; set; }
		string DataMember { get; set; }
		SnapListGroups Groups { get; }
		SnapListSorting Sorting { get; }
		SnapListFilters Filters { get; }
		void ApplyTableStyles();
		void ApplyTableStyles(int level);
	}
}
namespace DevExpress.Snap.API.Native {
	using DevExpress.Office.Utils;
	using DevExpress.Snap.Core.API;
	using DevExpress.Snap.Core.Fields;
	using DevExpress.Snap.Core.Native;
	using DevExpress.XtraRichEdit.API.Native;
	using DevExpress.XtraRichEdit.Fields;
	using DevExpress.XtraRichEdit.Model;
	using System.Diagnostics;
	using Debug = System.Diagnostics.Debug;
	public class NativeSnapList : NativeSnapEntityBase, SnapList {
		#region Fields
		string name;
		string dataSourceName;
		string dataMember;
		bool dataSourceModified;
		Dictionary<string, DocumentRange> switches;
		NativeSnapListGroups groups;
		NativeSnapListSorting sorting;
		NativeSnapListFilters filters;
		int erl;
		SnapNativeDocument rowTemplate;
		SnapNativeDocument listHeader;
		SnapNativeDocument listFooter;
		SnapNativeDocument separator;
		bool rowTemplateModified;
		bool listHeaderModified;
		bool listFooterModified;
		bool separatorModified;
		IFieldDataAccessService dataAccessService;		
		#endregion
		public NativeSnapList(SnapNativeDocument document, ApiField field) : this(document, document, field) { }
		public NativeSnapList(SnapSubDocument subDocument, SnapNativeDocument document, ApiField field) : base(subDocument, document, field) {
#if DEBUGTEST
			Debug.Assert(object.ReferenceEquals(((DevExpress.XtraRichEdit.API.Native.Implementation.NativeSubDocument)subDocument).DocumentModel, document.DocumentModel));
#endif
			SNListField parsedField = GetParsedField<SNListField>();
			if (parsedField.Name == null)
				this.name = String.Empty;
			else
				this.name = parsedField.Name;
			this.dataAccessService = document.DocumentModel.GetService<IFieldDataAccessService>();
			if (dataAccessService == null)
				Exceptions.ThrowInternalException();
			FieldPathInfo pathInfo = dataAccessService.FieldPathService.FromString(parsedField.DataSourceName);
			this.dataSourceName = FieldDataSourceInfoToString(pathInfo.DataSourceInfo);
			this.dataMember = pathInfo.GetFullPath();
			List<string> filtersData;
			if (pathInfo.DataMemberInfo.LastItem == null || pathInfo.DataMemberInfo.LastItem.FilterProperties == null)
				filtersData = new List<string>();
			else
				filtersData = pathInfo.DataMemberInfo.LastItem.FilterProperties.Filters;
			this.filters = new NativeSnapListFilters(this, filtersData);
		}
		public DocumentRange GetSwitchValue(string key) {
			if(switches == null)
				UpdateSwitches();
			DocumentRange range;
			if(switches.TryGetValue(key, out range))
				return range;
			return null;
		}
		void UpdateSwitches() {
			switches = new Dictionary<string, DocumentRange>();
			SNListField parsedField = GetParsedField<SNListField>();
			foreach(string key in parsedField.Switches.GetAllSwitchKeys()) {
				DocumentLogInterval interval = parsedField.Switches.GetSwitchArgumentDocumentInterval(key);
				switches.Add(key, Document.CreateRange(interval.Start, interval.Length));
			}
			erl = SNListField.DefaultEditorRowLimit;
			if(switches.ContainsKey(SNListField.EditorRowLimitSwitch)) {
				Int32.TryParse(parsedField.Switches.GetString(SNListField.EditorRowLimitSwitch), out erl);
			}
		}
		void UpdateRowTemplate() {
			InternalSnapDocumentServer server = GetTemplate(SNListField.TemplateSwitch);
			this.rowTemplate = (SnapNativeDocument)server.Document;
			server.ModifiedChanged += RowTemplateDocumentModel_ModifiedChanged;
		}
		void UpdateListHeader() {
			InternalSnapDocumentServer server = GetTemplate(SNListField.ListHeaderTemplateSwitch);
			this.listHeader = (SnapNativeDocument)server.Document;
			server.ModifiedChanged += ListHeaderDocumentModel_ModifiedChanged;
		}
		void UpdateListFooter() {
			InternalSnapDocumentServer server = GetTemplate(SNListField.ListFooterTemplateSwitch);
			this.listFooter = (SnapNativeDocument)server.Document;
			server.ModifiedChanged += ListFooterDocumentModel_ModifiedChanged;
		}
		void UpdateSeparator() {
			InternalSnapDocumentServer server = GetTemplate(SNListField.SeparatorTemplateSwitch);
			this.separator = (SnapNativeDocument)server.Document;
			server.ModifiedChanged += SeparatorDocumentModel_ModifiedChanged;
		}
		InternalSnapDocumentServer GetTemplate(string switchKey) {
			if(this.switches == null)
				UpdateSwitches();
			SnapDocumentModel existingModel = Document.DocumentModel;
			DocumentModel model = existingModel.CreateNew();
			DocumentModelCopyCommand.ReplaceDefaultProperties(model, existingModel);
			DocumentModelCopyCommand.CopyStyles(model, existingModel);
			model.FieldOptions.CopyFrom(existingModel.FieldOptions);
			if(this.switches.ContainsKey(switchKey)) {
				DocumentRange range = this.switches[switchKey];
				DevExpress.XtraRichEdit.Internal.CopyHelper.CopyCore(Document.PieceTable, model.MainPieceTable, new DocumentLogInterval(range.Start.LogPosition, range.Length), DocumentLogPosition.Zero);
			}
			InternalSnapDocumentServer server = new InternalSnapDocumentServer((SnapDocumentModel)model);
			server.DocumentModel.Modified = false;
			return server;
		}
		void RowTemplateDocumentModel_ModifiedChanged(object sender, EventArgs e) {
			EnsureUpdateBegan();
			this.rowTemplateModified = true;
			this.rowTemplate.ModifiedChanged -= RowTemplateDocumentModel_ModifiedChanged;
		}
		void ListHeaderDocumentModel_ModifiedChanged(object sender, EventArgs e) {
			EnsureUpdateBegan();
			this.listHeaderModified = true;
			this.listHeader.ModifiedChanged -= ListHeaderDocumentModel_ModifiedChanged;
		}
		void ListFooterDocumentModel_ModifiedChanged(object sender, EventArgs e) {
			EnsureUpdateBegan();
			this.listFooterModified = true;
			this.listFooter.ModifiedChanged -= ListFooterDocumentModel_ModifiedChanged;
		}
		void SeparatorDocumentModel_ModifiedChanged(object sender, EventArgs e) {
			EnsureUpdateBegan();
			this.separatorModified = true;
			this.separator.ModifiedChanged -= SeparatorDocumentModel_ModifiedChanged;
		}
		void ApplyGroupsAndSorting() {
			if((this.groups == null) && (this.sorting == null)) {
				if(this.filters.Modified || this.dataSourceModified)
					InitGroupsAndSorting();
				else
					return;
			}
			FieldPathInfo ds = new FieldPathInfo() { DataSourceInfo = FieldDataSourceInfoFromString(this.dataSourceName), DataMemberInfo = new FieldPathDataMemberInfo() };
			ds.DataMemberInfo.Items.Add(new FieldDataMemberInfoItem(this.dataMember));
			foreach(SnapListGroupParam param in this.sorting) {
				this.groups.Add(this.groups.CreateSnapListGroupInfo(param));
			}
			foreach(SnapListGroupInfo group in this.groups) {
				ds.DataMemberInfo.LastItem.AddGroup(((NativeSnapListGroupInfo)group).Group);
				((NativeSnapListGroupInfo)group).EndUpdate();
			}
			foreach(string filter in this.filters) {
				ds.DataMemberInfo.LastItem.AddFilter(filter);
			}
			Controller.SetArgument(0, dataAccessService.FieldPathService.GetStringPath(ds));
			this.groups.SetObsolete();
			this.sorting.SetObsolete();
			this.groups = null; this.sorting = null;
			this.filters.Modified = false;
			this.dataSourceModified = false;
		}
		void InitGroupsAndSorting() {
			FieldDataMemberInfoItem memberInfo = dataAccessService.FieldPathService.FromString(GetParsedField<SNListField>().DataSourceName).DataMemberInfo.LastItem;
			this.groups = new NativeSnapListGroups(this, memberInfo);
			this.sorting = new NativeSnapListSorting(this, memberInfo);
		}
		public DocumentModel CreateSwitch(out string key) {
			EnsureUpdateBegan();
			key = Controller.GetNonUsedSwitch();
			Controller.SetSwitch(key, String.Empty);
			DocumentModel templateDocumentModel = Document.DocumentModel.CreateNew();
			return templateDocumentModel;
		}
		protected override void Reset() {
			base.Reset();
			groups = null;
			sorting = null;
			switches = null;			
		}
		enum ElementStyle { 
			None = 0, 
			Header = 1, 
			Footer = 2, 
			GroupHeader = 3, 
			GroupFooter = 4 
		}
		static readonly string[] ElementStyleNames = new[] { 
			SnapDocumentModel.DefaultListStyleName,
			SnapDocumentModel.DefaultListHeaderStyleName,
			SnapDocumentModel.DefaultListFooterStyleName,
			SnapDocumentModel.DefaultGroupHeaderStyleName,
			SnapDocumentModel.DefaultGroupFooterStyleName
		};
		void ApplyTableStylesCore(SnapNativeDocument template, ElementStyle baseStyle, int listLevel) { ApplyTableStylesCore(template, baseStyle, listLevel, 0); }
		void ApplyTableStylesCore(SnapNativeDocument template, ElementStyle baseStyle, int listLevel, int groupLevel) {
			template.BeginUpdate();
			try {
				template.DocumentModel.BeginUpdate();
				try {
					SnapDocumentModel styleSourceModel = Document.DocumentModel;
					SnapDocumentModel result = template.DocumentModel;
					string styleName = StyleHelper.GetStyleName(ElementStyleNames[(int)ElementStyle.None], listLevel, styleSourceModel);
					DevExpress.XtraRichEdit.Model.TableStyle tableStyleList = styleSourceModel.TableStyles.GetStyleByName(styleName);
					if(tableStyleList != null) {
						DevExpress.XtraRichEdit.Model.TableStyle newStyle = tableStyleList.CopyTo(result);
						foreach(ModelTable table in result.MainPieceTable.Tables) {
							table.TableProperties.AvoidDoubleBorders = true;
							if(table.NestedLevel != 0)
								continue;
							table.StyleIndex = table.DocumentModel.TableStyles.GetStyleIndexByName(newStyle.StyleName);
						}
					}
					switch(baseStyle) {
						case ElementStyle.Header:
						case ElementStyle.Footer:
							styleName = StyleHelper.GetCellStyleName(ElementStyleNames[(int)baseStyle], listLevel, styleSourceModel);
							break;
						case ElementStyle.GroupHeader:
						case ElementStyle.GroupFooter:
							styleName = StyleHelper.GetGroupStyleName(ElementStyleNames[(int)baseStyle], listLevel, groupLevel, styleSourceModel);
							break;
						case ElementStyle.None:
						default:							
							return;
					}
					ModelTableCellStyle tableStyleList1 = styleSourceModel.TableCellStyles.GetStyleByName(styleName);
					if(tableStyleList1 != null) {
						ModelTableCellStyle newStyle = tableStyleList1.CopyTo(result);
						foreach(var table in result.MainPieceTable.Tables) {
							table.Rows.ForEach(row => row.Cells.ForEach(cell => StyleHelper.ApplyTableCellStyleCore(newStyle.StyleName, cell)));
						}
					}
				}
				finally {
					template.DocumentModel.EndUpdate();
				}
			}
			finally {
				template.EndUpdate();
			}
		}
		#region SnapList Members
		public string Name {
			get {
				return name;
			}
			set {
				EnsureUpdateBegan();
				if(value == null)
					name = String.Empty;
				name = value;
				Controller.SetSwitch("name", value);
			}
		}
		public SnapDocument RowTemplate { 
			get {
				if(rowTemplate == null)
					UpdateRowTemplate();
				return rowTemplate;
			} 
		}
		public SnapDocument ListHeader {
			get {
				if(listHeader == null)
					UpdateListHeader();
				return listHeader;
			}
		}
		public SnapDocument ListFooter {
			get {
				if(listFooter == null)
					UpdateListFooter();
				return listFooter;
			}
		}
		public SnapDocument Separator {
			get {
				if (separator == null)
					UpdateSeparator();
				return separator;
			}
		}
		public bool KeepLastSeparator { 
			get {
				if(Active)
					return Controller.ContainsSwitch(SNListField.KeepLastSeparatorSwitch) && ! Controller.IsSwitchRemoving(SNListField.KeepLastSeparatorSwitch);
				SNListField parsedField = GetParsedField<SNListField>();
				return parsedField.Switches.GetBool(SNListField.KeepLastSeparatorSwitch);
			}
			set {
				EnsureUpdateBegan();
				if(value)
					Controller.SetSwitch(SNListField.KeepLastSeparatorSwitch);
				else
					Controller.RemoveSwitch(SNListField.KeepLastSeparatorSwitch);
			}
		}
		public int EditorRowLimit { 
			get {
				if(switches == null)
					UpdateSwitches();
				return erl;
			}
			set {
				EnsureUpdateBegan();
				if(EditorRowLimit == value)
					return;
				erl = (value < 0) ? 0 : value;
				Controller.SetSwitch(SNListField.EditorRowLimitSwitch, erl.ToString());
			}
		}
		public string DataSourceName {
			get {
				return dataSourceName;
			}
			set {
				EnsureUpdateBegan();
				this.dataSourceModified = true;
				this.dataSourceName = value;
			}
		}
		public string DataMember {
			get {
				return dataMember;
			}
			set {
				EnsureUpdateBegan();
				this.dataSourceModified = true;
				this.dataMember = value;
			}
		}
		public SnapListGroups Groups {
			get {
				if(groups == null)
					InitGroupsAndSorting();
				return groups;
			}
		}
		public SnapListSorting Sorting {
			get {
				if(sorting == null)
					InitGroupsAndSorting();
				return sorting;
			}
		}
		public SnapListFilters Filters { get { return filters; } }
		public void ApplyTableStyles() { ApplyTableStyles(1); }
		public void ApplyTableStyles(int level) {
			ApplyTableStylesCore((SnapNativeDocument)RowTemplate, ElementStyle.None, level);
			ApplyTableStylesCore((SnapNativeDocument)ListHeader, ElementStyle.Header, level);
			ApplyTableStylesCore((SnapNativeDocument)ListFooter, ElementStyle.Footer, level);
			int groupLevel = 1;
			foreach(SnapListGroupInfo group in Groups) {
				if(group.Header != null)
					ApplyTableStylesCore((SnapNativeDocument)group.Header, ElementStyle.GroupHeader, level, groupLevel);
				if(group.Footer != null)
					ApplyTableStylesCore((SnapNativeDocument)group.Footer, ElementStyle.GroupFooter, level, groupLevel);
				groupLevel ++;
			}
		}
		#endregion
		#region SnapEntity Members
		public override void EndUpdate() {
			if (!Active) return;
			ApplyGroupsAndSorting();
			if (this.rowTemplateModified) {
				UpdateSwitch(SNListField.TemplateSwitch, this.rowTemplate.DocumentModel);
				this.rowTemplate = null;
				this.rowTemplateModified = false;
			}
			if (this.listHeaderModified) {
				UpdateSwitch(SNListField.ListHeaderTemplateSwitch, this.listHeader.DocumentModel);
				this.listHeader = null;
				this.listHeaderModified = false;
			}
			if (this.listFooterModified) {
				UpdateSwitch(SNListField.ListFooterTemplateSwitch, this.listFooter.DocumentModel);
				this.listFooter = null;
				this.listFooterModified = false;
			}
			if (this.separatorModified) {
				UpdateSwitch(SNListField.SeparatorTemplateSwitch, this.separator.DocumentModel);
				this.separator = null;
				this.separatorModified = false;
			}
			base.EndUpdate();
		}
		void UpdateSwitch(string fieldSwitch, SnapDocumentModel model) {
			if (model.IsEmpty)
				Controller.RemoveSwitch(fieldSwitch);
			else
				Controller.SetSwitch(fieldSwitch, model);
		}
		#endregion
		internal static FieldDataSourceInfo FieldDataSourceInfoFromString(string value) {
			return !String.IsNullOrEmpty(value) ? ((FieldDataSourceInfo) new RootFieldDataSourceInfo(value)) : ((FieldDataSourceInfo) new RelativeFieldDataSourceInfo(0));
		}
		internal static string FieldDataSourceInfoToString(FieldDataSourceInfo value) {
			RootFieldDataSourceInfo ds = value as RootFieldDataSourceInfo;
			return !Object.ReferenceEquals(ds, null) ? ds.Name : String.Empty;
		}
	}
}
