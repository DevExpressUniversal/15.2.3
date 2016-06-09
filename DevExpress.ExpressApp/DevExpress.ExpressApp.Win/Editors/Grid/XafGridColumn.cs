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
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraGrid.Columns;
namespace DevExpress.ExpressApp.Win.Editors {
	#region Obsolete 14.2
	[Obsolete(ObsoleteMessages.TypeIsNotUsedAnymore + " Instead, either use the GridColumn object or iterate through the ColumnsListEditor.Columns collection and cast its element to XafGridColumnWrapper to access its information.", true)]
	public class XafGridColumn : GridColumn {
		private ITypeInfo typeInfo;
		private bool allowSummaryChange = true;
		private IModelColumn model;
		private GridListEditor gridListEditor;
		private bool isProtectedContentColumn;
		public XafGridColumn(ITypeInfo typeInfo, GridListEditor gridListEditor)
			: this(typeInfo) {
			this.gridListEditor = gridListEditor;
		}
		public XafGridColumn(ITypeInfo typeInfo, bool isProtectedColumn)
			: this(typeInfo) {
			this.isProtectedContentColumn = isProtectedColumn;
		}
		protected XafGridColumn(ITypeInfo typeInfo) {
			this.typeInfo = typeInfo;
		}
		public bool IsProtectedContentColumn {
			get {
				if(gridListEditor != null) {
					return gridListEditor.HasProtectedContentInternal(model.PropertyName);
				}
				else {
					return isProtectedContentColumn;
				}
			}
		}
		public CollectionSourceDataAccessMode DataAccessMode {
			get { return ((IModelListView)model.ParentView).DataAccessMode; }
		}
		public void SetModel(IModelColumn columnInfo) {
			model = columnInfo;
		}
		public void ApplyModel(IModelColumn columnInfo) {
			SetModel(columnInfo);
			CreateModelSynchronizer().ApplyModel();
		}
		public void SynchronizeModel() {
			CreateModelSynchronizer().SynchronizeModel();
		}
		public ITypeInfo TypeInfo {
			get { return typeInfo; }
		}
		public string PropertyName {
			get {
				if(model != null)
					return model.PropertyName;
				return string.Empty;
			}
		}
		public bool AllowSummaryChange {
			get { return allowSummaryChange; }
			set { allowSummaryChange = value; }
		}
		public IModelColumn Model {
			get { return model; }
		}
		private ModelSynchronizer CreateModelSynchronizer() {
			throw new NotImplementedException();
		}
	}
	#endregion
}
