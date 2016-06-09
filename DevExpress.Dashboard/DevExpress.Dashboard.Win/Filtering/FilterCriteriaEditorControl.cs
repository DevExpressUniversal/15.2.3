#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.LookAndFeel;
using DevExpress.Utils.UI;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Filtering;
using DevExpress.DashboardCommon;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public partial class FilterCriteriaEditorControl : XtraUserControl {
		class FilterCriteriaEditorContext : IFilterParametersOwner, IFilteredComponent {
			readonly IFilterParametersOwner parametersOwner;
			readonly IFilteredComponent filteredComponent;
			public FilterCriteriaEditorContext(FilterFormParametersOwner parametersOwner, IFilteredComponent filteredComponent) {
				this.parametersOwner = parametersOwner;
				this.filteredComponent = filteredComponent;
			}
			#region interface methods
			void IFilterParametersOwner.AddParameter(string name, Type type) {
				this.parametersOwner.AddParameter(name, type);
			}
			bool IFilterParametersOwner.CanAddParameters { get { return this.parametersOwner.CanAddParameters; } }
			IList<IFilterParameter> IFilterParametersOwner.Parameters { get { return this.parametersOwner.Parameters; } }
			IBoundPropertyCollection IFilteredComponent.CreateFilterColumnCollection() {
				return this.filteredComponent.CreateFilterColumnCollection();
			}
			CriteriaOperator IFilteredComponentBase.RowCriteria { get { return this.filteredComponent.RowCriteria; } set { this.filteredComponent.RowCriteria = value; } }
			public event EventHandler PropertiesChanged { add { } remove { } }
			public event EventHandler RowFilterChanged { add { } remove { } }
			#endregion
		}
		public static string GetFilterString(CriteriaOperator filterCriteria) {
			return ReferenceEquals(filterCriteria, null) ? String.Empty : filterCriteria.ToString();
		}
		static readonly object criteriaChanged = new object();
		static readonly object beforeShowValueEditor = new object();
		readonly IList<IParameter> parameters;
		readonly IFilteredComponent filteredComponent;
		public event FilterCriteriaEditorCriteriaChangedEventHandler CriteriaChanged { add { Events.AddHandler(criteriaChanged, value); } remove { Events.RemoveHandler(criteriaChanged, value); } }
		public event ShowValueEditorEventHandler BeforeShowValueEditor { add { Events.AddHandler(beforeShowValueEditor, value); } remove { Events.RemoveHandler(beforeShowValueEditor, value); } }
		public IList<IParameter> Parameters { get { return this.parameters; } }
		public bool IsFilterCriteriaChanged { get { return GetFilterString(PreviousFilterCriteria) != GetFilterString(NextFilterCriteria); } }
		public CriteriaOperator FilterCriteria { get { return this.filterControl.FilterCriteria; } set { this.filterControl.FilterCriteria = value; } }
		public string FilterCriteriaString { get { return !Equals(FilterCriteria, null) ? FilterCriteria.ToString() : string.Empty; } set { FilterCriteria = CriteriaOperator.Parse(value); } }
		protected CriteriaOperator PreviousFilterCriteria { get { return this.filteredComponent.RowCriteria; } }
		protected CriteriaOperator NextFilterCriteria { get { return FilterCriteria; } }
		public FilterCriteriaEditorControl() {
			InitializeComponent();
		}
		public FilterCriteriaEditorControl(IParameterCreator parameterCreator, IFilteredComponent filteredComponent, IList<IParameter> oldParameters, UserLookAndFeel lookAndFeel)
			: this(parameterCreator, filteredComponent, oldParameters, lookAndFeel, null) {
		}
		public FilterCriteriaEditorControl(IParameterCreator parameterCreator, IFilteredComponent filteredComponent, IList<IParameter> oldParameters, string defaultDataMember)
			: this(parameterCreator, filteredComponent, oldParameters, null, defaultDataMember) {
		}
		protected FilterCriteriaEditorControl(IParameterCreator parameterCreator, IFilteredComponent filteredComponent, IList<IParameter> oldParameters, UserLookAndFeel lookAndFeel, string defaultItem)
			: this() {
			this.filterControl.SourceControl = new FilterCriteriaEditorContext(new FilterFormParametersOwner(parameterCreator, oldParameters, parameterCreator != null), filteredComponent);
			if(this.filterControl.FilterColumns != null && this.filterControl.FilterColumns.Count > 0)
				this.filterControl.SetDefaultColumn(GetFirstSelectableNode(this.filterControl.FilterColumns, defaultItem));
			this.parameters = oldParameters;
			this.filteredComponent = filteredComponent;
			if(lookAndFeel != null)
				DataSourceWizardWinHelper.SetParentLookAndFeel(this, lookAndFeel);
		}
		public void ApplyFilterCriteria() {
			if(IsFilterCriteriaChanged)
				this.filteredComponent.RowCriteria = NextFilterCriteria;
		}
		void OnBeforeShowValueEditor(object sender, ShowValueEditorEventArgs e) {
			RaiseBeforeShowValueEditor(e);
		}
		void OnFilterControlFilterChanged(object sender, FilterChangedEventArgs e) {
			RaiseFilterCriteriaChanged(PreviousFilterCriteria, NextFilterCriteria);
		}
		void RaiseBeforeShowValueEditor(ShowValueEditorEventArgs e) {
			ShowValueEditorEventHandler handler = (ShowValueEditorEventHandler)Events[beforeShowValueEditor];
			if(handler != null) {
				handler(this, e);
			}
		}
		void RaiseFilterCriteriaChanged(CriteriaOperator previous, CriteriaOperator next) {
			FilterCriteriaEditorCriteriaChangedEventHandler handler = (FilterCriteriaEditorCriteriaChangedEventHandler)Events[criteriaChanged];
			if(handler != null) {
				handler(this, new FilterCriteriaEditorCriteriaChangedEventArgs(previous, next));
			}
		}
		static FilterColumn GetFirstSelectableNode(IEnumerable list, string defaultItem) {
			foreach(FilterColumn parentColumn in list) {
				if(parentColumn.Children == null) {
					if(defaultItem == null || parentColumn.FieldName == defaultItem)
						return parentColumn;
				}
				else {
					FilterColumn column = GetFirstSelectableNode(parentColumn.Children, defaultItem);
					if(column != null)
						return column;
				}
			}
			return null;
		}
	}
	public delegate void FilterCriteriaEditorCriteriaChangedEventHandler(object sender, FilterCriteriaEditorCriteriaChangedEventArgs e);
	public class FilterCriteriaEditorCriteriaChangedEventArgs : EventArgs {
		readonly CriteriaOperator oldCriteria;
		readonly CriteriaOperator newCriteria;
		public CriteriaOperator OldCriteria { get { return this.oldCriteria; } }
		public CriteriaOperator NewCriteria { get { return this.newCriteria; } }
		public bool IsChanged { get { return FilterCriteriaEditorControl.GetFilterString(OldCriteria) != FilterCriteriaEditorControl.GetFilterString(NewCriteria); } }
		public FilterCriteriaEditorCriteriaChangedEventArgs(CriteriaOperator previous, CriteriaOperator next) {
			this.oldCriteria = previous;
			this.newCriteria = next;
		}
	}
	public static class DataSourceWizardWinHelper {
		public static void SetParentLookAndFeel(object obj, UserLookAndFeel parentLookAndFeel) {
			if(obj == null)
				return;
			ISupportLookAndFeel supportLookAndFeel = obj as ISupportLookAndFeel;
			if(supportLookAndFeel != null)
				supportLookAndFeel.LookAndFeel.ParentLookAndFeel = parentLookAndFeel;
			else {
				PropertyInfo property = obj.GetType().GetProperty("LookAndFeel", BindingFlags.Public | BindingFlags.Instance);
				if(property == null)
					return;
				UserLookAndFeel lookAndFeel = property.GetValue(obj, null) as UserLookAndFeel;
				if(lookAndFeel != null)
					lookAndFeel.ParentLookAndFeel = parentLookAndFeel;
			}
		}
	}
	[DXToolboxItem(false)]
	public class DashboardFilterControl : FilterControl {
		public DashboardFilterControl() : base() { }
		protected override bool CheckAdditionalOperandParameterAllowed(FilterControlFocusInfo FocusInfo, IFilterParameter parameter) {
			if(!((DashboardParameter)parameter).AllowMultiselect)
				return true;
			else if(((ClauseNode)FocusInfo.Node).Operation == Data.Filtering.Helpers.ClauseType.AnyOf || ((ClauseNode)FocusInfo.Node).Operation == Data.Filtering.Helpers.ClauseType.NoneOf)
				return true;
			else
				return false;
		}
	}
}
