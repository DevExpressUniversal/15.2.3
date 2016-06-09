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
using System.Linq;
using System.Text;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web;
using DevExpress.Web.ASPxTreeList;
namespace DevExpress.ExpressApp.TreeListEditors.Web {
	public class TreeListDataColumnInfo : IDataColumnInfo {
		private IMemberInfo memberInfo;
		private IModelColumn model;
		private CollectionSourceDataAccessMode viewDataAccessMode;
		private bool isProtectedContentColumn;
		public TreeListDataColumnInfo(IModelColumn model, IMemberInfo memberInfo, CollectionSourceDataAccessMode viewDataAccessMode, bool isProtectedContentColumn)
			: base() {
			Guard.ArgumentNotNull(model, "model");
			Guard.ArgumentNotNull(memberInfo, "memberInfo");
			this.model = model;
			this.memberInfo = memberInfo;
			this.viewDataAccessMode = viewDataAccessMode;
			this.isProtectedContentColumn = isProtectedContentColumn;
		}
		public virtual ModelSynchronizer CreateModelSynchronizer(WebColumnBase column) {
			return new ColumnWrapperModelSynchronizer(CreateColumnWrapper(column), Model, viewDataAccessMode, isProtectedContentColumn);
		}
		public virtual WebColumnBaseColumnWrapper CreateColumnWrapper(WebColumnBase column) {
			return new XafTreeListColumnWrapper((TreeListDataColumn)column, this);
		}
		public void ApplyModel(WebColumnBase column) {
			CreateModelSynchronizer(column).ApplyModel();
		}
		public void SynchronizeModel(WebColumnBase column) {
			CreateModelSynchronizer(column).SynchronizeModel();
		}
		public IModelColumn Model {
			get { return model; }
		}
		public IMemberInfo MemberInfo {
			get { return memberInfo; }
			set { memberInfo = value; }
		}
		IModelLayoutElement IColumnInfo.Model {
			get {
				return model;
			}
		}
	}
}
