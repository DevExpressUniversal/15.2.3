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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.Persistent.Base;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraFilterEditor;
namespace DevExpress.ExpressApp.Win.Editors {
	public class FilterEditorControlHelper {
		private RepositoryEditorsFactory repositoryEditorsFactory;
		private IObjectSpace objectSpace;
		private void filterControl_CreateCriteriaParseContext(object sender, CreateCriteriaParseContextEventArgs e) {
			if(objectSpace != null) {
				e.Context = objectSpace.CreateParseCriteriaScope();
			}
		}
		private void filterControl_CreateCustomRepositoryItem(object sender, DevExpress.XtraEditors.Filtering.CreateCustomRepositoryItemEventArgs e) {
			if(CreateCustomRepositoryItem != null) {
				CreateCustomRepositoryItem(this, e);
			}
			if(e.RepositoryItem == null && repositoryEditorsFactory != null && e.Column != null && e.Column.ColumnType != null) {
				e.RepositoryItem = repositoryEditorsFactory.CreateStandaloneRepositoryItem(e.Column.ColumnType);
			}
		}
		public FilterEditorControlHelper(RepositoryEditorsFactory repositoryEditorsFactory, IObjectSpace objectSpace) {
			this.repositoryEditorsFactory = repositoryEditorsFactory;
			this.objectSpace = objectSpace;
		}
		public FilterEditorControlHelper() { }
		public FilterEditorControlHelper(XafApplication application, IObjectSpace objectSpace) : this(new RepositoryEditorsFactory(application, objectSpace), objectSpace) { }
		public void Attach(FilterEditorControl filterControl) {
			filterControl.ViewMode = FilterEditorViewMode.VisualAndText;
			filterControl.ShowDateTimeOperators = true;
			filterControl.ShowOperandTypeIcon = true;
			filterControl.ShowFunctions = true;
			filterControl.ShowGroupCommandsIcon = false;
			filterControl.AllowAggregateEditing = FilterControlAllowAggregateEditing.AggregateWithCondition;
			filterControl.UseMenuForOperandsAndOperators = false;
			filterControl.CreateCustomRepositoryItem += new EventHandler<DevExpress.XtraEditors.Filtering.CreateCustomRepositoryItemEventArgs>(filterControl_CreateCustomRepositoryItem);
			filterControl.CreateCriteriaParseContext += new EventHandler<CreateCriteriaParseContextEventArgs>(filterControl_CreateCriteriaParseContext);
		}
		public event EventHandler<DevExpress.XtraEditors.Filtering.CreateCustomRepositoryItemEventArgs> CreateCustomRepositoryItem;
	}
	public class CriteriaPropertyEditor : WinPropertyEditor, IComplexViewItem, IDependentPropertyEditor {
		private CriteriaPropertyEditorHelper helper;
		private XafApplication application;
		private IObjectSpace objectSpace;
		private void filterControl_FilterChanged(object sender, FilterChangedEventArgs e) {
			OnControlValueChanged();
		}
		private void filterControl_FilterTextChanged(object sender, FilterTextChangedEventArgs e) {
			OnControlValueChanged();
		}
		private void RefreshControlDatasource() {
			Guard.ArgumentNotNull(helper, "helper");
			if(Control != null) {
				Control.SourceControl = helper.CreateFilterControlDataSource(CurrentObject, application);
				Type itemType = helper.GetCriteriaObjectType(CurrentObject);
				if(itemType != null) {
					ITypeInfo ti = XafTypesInfo.Instance.FindTypeInfo(itemType);
					if(ti != null && ti.DefaultMember != null) {
						foreach(FilterColumn col in Control.FilterColumns) {
							if(ti.DefaultMember.Name == col.FieldName) {
								Control.SetDefaultColumn(col);
							}
						}
					}
					for(Int32 i = Control.FilterColumns.Count - 1; i >= 0; i--) {
						if(CriteriaPropertyEditorHelper.IgnoredMemberTypes.Contains(Control.FilterColumns[i].ColumnType)) {
							Control.FilterColumns.RemoveAt(i);
						}
					}
				}
			}
		}
		protected override object CreateControlCore() {
			Guard.ArgumentNotNull(helper, "helper");
			FilterEditorControl filterControl = new FilterEditorControl();
			new FilterEditorControlHelper(application, objectSpace).Attach(filterControl);
			filterControl.AllowCreateDefaultClause = false; 
			filterControl.FilterChanged += new FilterChangedEventHandler(filterControl_FilterChanged);
			filterControl.FilterTextChanged += new FilterTextChangedEventHandler(filterControl_FilterTextChanged);
			return filterControl;
		}
		protected override void OnFormatValue(ConvertEventArgs e) {
			Guard.ArgumentNotNull(helper, "helper");
			e.Value = helper.ConvertFromOldFormat((string)e.Value, CurrentObject, objectSpace);
		}
		protected override void OnCurrentObjectChanged() {
			base.OnCurrentObjectChanged();
			RefreshControlDatasource();
		}
		protected override void OnControlCreated() {
			base.OnControlCreated();
			RefreshControlDatasource();
		}
		public CriteriaPropertyEditor(Type objectType, IModelMemberViewItem model)
			: base(objectType, model) {
			ControlBindingProperty = "FilterString";
			CanUpdateControlEnabled = true;
		}
		public override void Refresh() {
			base.Refresh();
			RefreshControlDatasource(); 
		}
		public void Setup(IObjectSpace objectSpace, XafApplication application) {
			this.application = application;
			this.objectSpace = objectSpace;
			this.helper = new CriteriaPropertyEditorHelper(MemberInfo);
		}
		public new FilterEditorControl Control {
			get { return (FilterEditorControl)base.Control; }
		}
		public IList<string> MasterProperties {
			get { return helper.MasterProperties; }
		}
	}
}
