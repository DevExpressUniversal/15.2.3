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
using System.Text;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraReports.Native.Data;
using System.ComponentModel.Design;
using DevExpress.XtraReports.UI;
using DevExpress.Data;
using System.Windows.Forms.Design;
using DevExpress.XtraEditors.Controls;
using System.Windows.Forms;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraReports.Native;
using DevExpress.Data.Filtering;
using DevExpress.Data.Browsing;
using DevExpress.XtraFilterEditor;
using DevExpress.Data.Browsing.Design;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraReports.Design {
	class FilterStringControl : FilterEditorControl, IServiceProvider {
		#region inner classes
		class CustomWinFilterTreeNodeModel : WinFilterTreeNodeModel {
			IServiceProvider Provider { get { return ((CustomFilterControl)Control).ServiceProvider;} }
			IExtensionsProvider ExtensionProvider { get { return Provider.GetService<IExtensionsProvider>(); } }
			public CustomWinFilterTreeNodeModel(FilterControl control)
				: base(control) {
			}
			DataSourceProxy DataSourceProxy {
				get {
					return (DataSourceProxy)this.SourceControl;
				}
			}
			protected override CriteriaOperator CriteriaFromString(string value) {
				if (Provider != null && !string.IsNullOrEmpty(value)) {
					CriteriaOperator result = CriteriaOperator.Parse(value);
					if (!Object.ReferenceEquals(result, null)) {
						IDataContextService dataContextService = (IDataContextService)Provider.GetService(typeof(IDataContextService));
						using (DataContext dataContext = dataContextService.CreateDataContext(new DataContextOptions(true, true))) {
							return (CriteriaOperator)result.Accept(new DeserializationFilterStringVisitor(ExtensionProvider, dataContext, DataSourceProxy.DataSource, DataSourceProxy.DataMember));
						}
					}
				}
				return base.CriteriaFromString(value);
			}
			protected override string CriteriaToString(CriteriaOperator criteria) {
				if (!ReferenceEquals(criteria, null)) {
					CriteriaOperator clone = (CriteriaOperator)CriteriaOperator.Clone(criteria);
					CriteriaOperator result = (CriteriaOperator)clone.Accept(new SerializationVisitor(ExtensionProvider));
					return CriteriaOperator.ToString(result);
				}
				return base.CriteriaToString(criteria);
			}
		}
		class CustomFilterControl : FilterControl {
			IServiceProvider provider;
			internal IServiceProvider ServiceProvider { get { return provider; } }
			protected override WinFilterTreeNodeModel CreateModel() {
				return new CustomWinFilterTreeNodeModel(this);
			}
			public CustomFilterControl(IServiceProvider provider) {
				this.provider = provider;
				this.Model.CreateTree(null);
				this.UseMenuForOperandsAndOperators = false;
				this.AllowAggregateEditing = FilterControlAllowAggregateEditing.AggregateWithCondition;
			}
		}
		class SerializationVisitor : ClientCriteriaVisitorBase {
			IExtensionsProvider rootComponent;
			public SerializationVisitor(IExtensionsProvider rootComponent) {
				this.rootComponent = rootComponent;
			}
			public override CriteriaOperator Visit(BinaryOperator theOperator) {
				BinaryOperator result = base.Visit(theOperator) as BinaryOperator;
				OperandProperty leftOperand = theOperator.LeftOperand as OperandProperty;
				OperandValue rightOperand = theOperator.RightOperand as OperandValue;
				if(!ReferenceEquals(leftOperand, null) && !ReferenceEquals(rightOperand, null)) {
					string value;
					if(SerializationService.SerializeObject(rightOperand.Value, out value, rootComponent))
						rightOperand.Value = value;
				}
				return result;
			}
		}
		#endregion
		IServiceProvider provider;
		IExtensionsProvider extensionProvider;
		DataSourceProxy DataSourceProxy {
			get {
				return (DataSourceProxy)this.SourceControl;
			}
		}
		public void Initialize(IServiceProvider provider, object dataSource, string dataMember, IEnumerable<IParameter> parameters, IExtensionsProvider extensionsProvider) {
			this.provider = provider;
			this.extensionProvider = extensionsProvider;
			this.SourceControl = new DataSourceProxy(provider, dataSource, dataMember, parameters, extensionProvider);
		}
		public void Initialize(IServiceProvider provider, object dataSource, string dataMember, IList<IParameter> parameters) {
			this.provider = provider;
			this.SourceControl = new DataSourceProxy(provider, dataSource, dataMember, parameters, null, true);
		}
		protected override string SerializeFilterValue(FilterColumn column, object value) {
			string result;
			if(column != null && SerializationService.SerializeObject(value, out result, extensionProvider)) {
				return "'" + result + "'";
			}
			return base.SerializeFilterValue(column, value);
		}
		protected override FilterControl CreateTreeControl() {
			return new CustomFilterControl(this);
		}
		protected override object DeserializeFilterValue(FilterColumn column, string constValue, object trialValue) {
			object result;
			if(SerializationService.DeserializeObject(trialValue.ToString(), column.ColumnType, out result, extensionProvider)) {
				return result;
			}
			return base.DeserializeFilterValue(column, constValue, trialValue);
		}
		object IServiceProvider.GetService(Type serviceType) {
			if(serviceType == typeof(IExtensionsProvider))
				return extensionProvider;
			return provider != null ? provider.GetService(serviceType) : null;
		}
#if DEBUGTEST
		public DevExpress.XtraRichEdit.RichEditControl Test_Editor {
			get { return this.Editor; }
		}
#endif
	}
}
