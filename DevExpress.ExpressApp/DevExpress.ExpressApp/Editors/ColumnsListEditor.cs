#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.ComponentModel;
using DevExpress.Data;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
namespace DevExpress.ExpressApp.Editors {
	public class ColumnWrapperModelSynchronizer : ModelSynchronizer<ColumnWrapper, IModelColumn> {
		private CollectionSourceDataAccessMode viewDataAccessMode;
		private bool isProtectedContentColumn;
		public ColumnWrapperModelSynchronizer(ColumnWrapper gridColumn, IModelColumn model, ColumnsListEditor listEditor)
			: base(gridColumn, model) {
			viewDataAccessMode = listEditor.Model.DataAccessMode; 
			isProtectedContentColumn = listEditor.HasProtectedContent(Model.PropertyName); 
		}
		public ColumnWrapperModelSynchronizer(ColumnWrapper gridColumn, IModelColumn model, CollectionSourceDataAccessMode viewDataAccessMode, bool isProtectedContentColumn)
			: base(gridColumn, model) {
			this.viewDataAccessMode = viewDataAccessMode;
			this.isProtectedContentColumn = isProtectedContentColumn;
		}
		private bool GetAllowChange() {
			bool allowChange = true;
			if(viewDataAccessMode == CollectionSourceDataAccessMode.Server) {
				IModelListView listViewModel = Model.ParentView as IModelListView;
				if(listViewModel != null) {
					ITypeInfo typeInfo = listViewModel.ModelClass.TypeInfo;
					IMemberInfo memberInfo = typeInfo.FindMember(Model.PropertyName);
					if(memberInfo != null) {
						foreach(IMemberInfo intermediateMemberInfo in memberInfo.GetPath()) {
							if(!intermediateMemberInfo.IsPersistent && !intermediateMemberInfo.IsAliased) {
								return false;
							}
						}
					}
				}
				IModelMember modelMember = Model.ModelMember;
				if(modelMember == null) {
					throw new InvalidOperationException(String.Format("The {0} column's ModelMember property is null. This may happen if PropertyName isn't set correctly or ListView.ModelClass property isn't set.", Model.Id));
				}
				IMemberInfo displayMember = new ObjectEditorHelperBase(modelMember.MemberInfo.MemberTypeInfo, Model).DisplayMember;
				if(displayMember != null) {
					allowChange = displayMember.IsPersistent || displayMember.IsAliased;
				}
				else {
					allowChange = modelMember.MemberInfo.IsPersistent || modelMember.MemberInfo.IsAliased;
				}
			}
			return allowChange;
		}
		protected override void ApplyModelCore() {
			Control.Caption = Model.Caption;
			if(!string.IsNullOrEmpty(Model.DisplayFormat) &&
				(Model.GroupInterval == GroupInterval.None)) {
				Control.DisplayFormat = Model.DisplayFormat;
			}
			Control.Width = Model.Width;
			bool allowChange = GetAllowChange();
			Control.AllowSortingChange = allowChange;
			Control.AllowGroupingChange = allowChange;
			Control.AllowSummaryChange = allowChange;
			if(isProtectedContentColumn) {
				Control.DisableFeaturesForProtectedContentColumn();
			}
			if(Control.AllowSortingChange) {
				Control.SortIndex = Model.SortIndex;
				Control.SortOrder = Model.SortOrder;
			}
			if(Control.AllowGroupingChange) {
				Control.GroupInterval = (DateTimeGroupInterval)Enum.Parse(typeof(DateTimeGroupInterval), Model.GroupInterval.ToString());
				Control.GroupIndex = Model.GroupIndex;
			}
			if(Control.AllowSummaryChange) {
				List<SummaryType> summaryTypes = new List<SummaryType>();
				foreach(IModelColumnSummaryItem summary in Model.Summary) {
					summaryTypes.Add(summary.SummaryType);
				}
				Control.Summary = summaryTypes;
			}
		}
		public override void SynchronizeModel() {
			SynchronizeControlWidth();
			if(!isProtectedContentColumn) {
				Model.Caption = Control.Caption;
				if(Control.AllowSortingChange) {
					Model.SortIndex = Control.SortIndex;
					Model.SortOrder = Control.SortOrder;
				}
				if(Control.AllowGroupingChange) {
					Model.GroupInterval = (GroupInterval)Enum.Parse(typeof(GroupInterval), Control.GroupInterval.ToString());
					Model.GroupIndex = Control.GroupIndex;
				}
				if(Control.AllowSummaryChange) {
					Model.Summary.ClearNodes();
					for(int i = 0; i < Control.Summary.Count; i++) {
						IModelColumnSummaryItem columnSummary = Model.Summary.AddNode<IModelColumnSummaryItem>("Summary" + i);
						columnSummary.Index = i;
						columnSummary.SummaryType = Control.Summary[i];
					}
				}
			}
		}
		public virtual void SynchronizeControlWidth() {
			Model.Width = Control.Width;
		}
	}
	public class ColumnsListEditorModelSynchronizer : ModelSynchronizer<ColumnsListEditor, IModelListView> {
		public ColumnsListEditorModelSynchronizer(ColumnsListEditor listEditor, IModelListView model)
			: base(listEditor, model) {
		}
		protected override void ApplyModelCore() {
			Dictionary<string, ColumnWrapper> presentedColumns = new Dictionary<string, ColumnWrapper>();
			List<ColumnWrapper> toDelete = new List<ColumnWrapper>();
			foreach(ColumnWrapper column in Control.Columns) {
				presentedColumns.Add(column.Id, column);
				toDelete.Add(column);
			}
			if(Model.Columns is ModelNode) {
				List<ModelNode> unsortedList = ((ModelNode)Model.Columns).GetUnsortedChildren();
				List<ModelNode> list = ((ModelNode)Model.Columns).GetUnsortedChildren();
				list.Sort(new ModelNodesComparer());
				foreach(ModelNode columnNode in unsortedList) {
					IModelColumn column = columnNode as IModelColumn;
					if(column != null && Model.Columns[column.Id] != null) {
						ColumnWrapper gridColumn = null;
						if(presentedColumns.TryGetValue(column.Id, out gridColumn)) {
							gridColumn.ApplyModel(column);
						}
						else {
							gridColumn = Control.AddColumn(column);
							presentedColumns.Add(column.Id, gridColumn);
						}
						toDelete.Remove(gridColumn);
					}
				}
				int index = 0;
				foreach(IModelColumn modelColumn in list) {
					int visibleIndex = -1;
					if(!modelColumn.Index.HasValue || modelColumn.Index.Value >= 0) {
						visibleIndex = index;
						index++;
					}
					Control.FindColumn(modelColumn.Id).VisibleIndex = visibleIndex;
				}
			}
			foreach(ColumnWrapper gridColumn in toDelete) {
				Control.RemoveColumn(gridColumn);
			}
			foreach(IModelColumn column in GetSortedBySortIndex(Model.Columns)) {
				if(column.SortIndex != -1 && !Control.HasProtectedContent(column.PropertyName) && presentedColumns[column.Id].AllowSortingChange) {
					presentedColumns[column.Id].SortIndex = column.SortIndex;
				}
			}
		}
		private int ComparisonSortIndex(IModelColumn c1, IModelColumn c2) {
			return c1.SortIndex - c2.SortIndex;
		}
		private IEnumerable<IModelColumn> GetSortedBySortIndex(IModelColumns iModelColumns) {
			List<IModelColumn> list = new List<IModelColumn>();
			foreach(IModelColumn column in iModelColumns) {
				list.Add(column);
			}
			list.Sort(new Comparison<IModelColumn>(ComparisonSortIndex));
			return list;
		}
		public override void SynchronizeModel() {
			foreach(ColumnWrapper column in Control.Columns) {
				column.SynchronizeModel();
			}
			SynchronizeVisibleIndexes();
		}
		private void SynchronizeVisibleIndexes() {
			List<IModelColumn> visibleNodesFromModel = new List<IModelColumn>(Model.Columns.GetVisibleColumns());
			visibleNodesFromModel.Sort(new ModelNodesComparer());
			List<ColumnWrapper> visibleColumnsFromControl = Control.GetVisibleColumns();
			visibleColumnsFromControl.Sort(delegate(ColumnWrapper column1, ColumnWrapper column2) { return column1.VisibleIndex.CompareTo(column2.VisibleIndex); });
			List<string> visibleColumnsFromControl_ID = new List<string>();
			foreach(ColumnWrapper item in visibleColumnsFromControl) {
				if(!string.IsNullOrEmpty(item.Id)) {
					visibleColumnsFromControl_ID.Add(item.Id);
				}
			}
			for(int i = 0; i < visibleColumnsFromControl_ID.Count; i++) {
				string columnID = visibleColumnsFromControl_ID[i];
				if(!(visibleNodesFromModel.Count > i && visibleNodesFromModel[i].Id == columnID)) {
					IModelColumn modelColumn = Model.Columns[columnID];
					if(modelColumn != null) {
						modelColumn.Index = i;
					}
				}
			}
			foreach(IModelColumn node in Model.Columns) {
				if(!visibleColumnsFromControl_ID.Contains(node.Id)) {
					node.Index = -1;
				}
			}
		}
	}
	public class ColumnWrapper : IAppearanceVisibility {
		public virtual String PropertyName {
			get { return ""; }
		}
		public virtual String Id {
			get { return String.Empty; }
		}
		public virtual int VisibleIndex {
			get { return -1; }
			set { ; }
		}
		public virtual int SortIndex {
			get { return -1; }
			set { ; }
		}
		public virtual ColumnSortOrder SortOrder {
			get { return ColumnSortOrder.None; }
			set { ; }
		}
		public virtual bool AllowSortingChange {
			get { return false; }
			set { ; }
		}
		public virtual int GroupIndex {
			get { return -1; }
			set { ; }
		}
		public virtual DateTimeGroupInterval GroupInterval {
			get { return DateTimeGroupInterval.None; }
			set { ; }
		}
		public virtual bool AllowGroupingChange {
			get { return false; }
			set { ; }
		}
		public virtual IList<SummaryType> Summary {
			get { return null; }
			set { }
		}
		public virtual String SummaryFormat {
			get { return ""; }
			set { ; }
		}
		public virtual bool AllowSummaryChange {
			get { return false; }
			set { ; }
		}
		public virtual string Caption {
			get { return String.Empty; }
			set { }
		}
		public virtual string ToolTip {
			get { return String.Empty; }
			set { }
		}
		public virtual string DisplayFormat {
			get { return String.Empty; }
			set { }
		}
		public virtual int Width {
			get { return Int32.MinValue; }
			set { }
		}
		public virtual void DisableFeaturesForProtectedContentColumn() {
			AllowSortingChange = false;
			AllowGroupingChange = false;
			AllowSummaryChange = false;
		}
		public virtual void ApplyModel(IModelColumn columnInfo) {
		}
		public virtual void SynchronizeModel() {
		}
		public virtual bool AllowVisibleIndexStore {
			get {
				return true;
			}
		}
		public virtual bool ShowInCustomizationForm { get { return true; } set { } }
		public virtual bool Visible { get { return VisibleIndex > 0; } }
		#region IAppearanceVisibility Members
		ViewItemVisibility IAppearanceVisibility.Visibility {
			get { return VisibleIndex >= 0 ? ViewItemVisibility.Show : ViewItemVisibility.Hide; }
			set {
				if(((IAppearanceVisibility)this).Visibility != value) {
					if(value == ViewItemVisibility.Show) {
						VisibleIndex = 500;
					}
					else {
						VisibleIndex = -1;
					}
				}
				ShowInCustomizationForm = ((IAppearanceVisibility)this).Visibility == ViewItemVisibility.Show;
			}
		}
		void IAppearanceVisibility.ResetVisibility() { }
		#endregion
	}
	public abstract class ColumnsListEditor : ListEditor, ISupportToolTip {
		public override void Dispose() {
			base.Dispose();
			ColumnAdded = null;
		}
		protected ColumnsListEditor(IModelListView model)
			: base(model) {
		}
		public abstract IList<ColumnWrapper> Columns {
			get;
		}
		public List<ColumnWrapper> GetVisibleColumns() {
			List<ColumnWrapper> result = new List<ColumnWrapper>(); ;
			foreach(ColumnWrapper column in Columns) {
				if(column.Visible) {
					result.Add(column);
				}
			}
			return result;
		}
		public ColumnWrapper AddColumn(IModelColumn columnInfo) {
			if(FindColumn(columnInfo.Id) != null) {
				throw new ArgumentException(String.Format(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.GridColumnExists), columnInfo.Id), "ColumnInfo");
			}
			IModelColumn frameColumn = Model.Columns[columnInfo.Id];
			if(frameColumn == null) {
				columnInfo.Remove();
			}
			ColumnWrapper column = AddColumnCore(columnInfo);
			((ISupportToolTip)this).SetToolTip(column, columnInfo);
			if(ColumnAdded != null) {
				ColumnAdded(this, EventArgs.Empty);
			}
			return column;
		}
		protected abstract ColumnWrapper AddColumnCore(IModelColumn columnInfo);
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RemoveColumn(ColumnWrapper column) { }
		public ColumnWrapper FindColumn(string id) {
			foreach(ColumnWrapper column in Columns) {
				if(column.Id == id) {
					return column;
				}
			}
			return null;
		}
		protected internal virtual bool HasProtectedContent(string propertyName) {
			return false;
		}
		void ISupportToolTip.SetToolTip(object element, IModelToolTip model) {
			ColumnWrapper column = element as ColumnWrapper;
			if(model != null && column != null) {
				if(!string.IsNullOrEmpty(model.ToolTip)) {
					column.ToolTip = model.ToolTip;
				}
			}
		}
		public event EventHandler ColumnAdded;
	}
}
