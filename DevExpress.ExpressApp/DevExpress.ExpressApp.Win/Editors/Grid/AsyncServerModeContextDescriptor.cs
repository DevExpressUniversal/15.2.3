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
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
namespace DevExpress.ExpressApp.Win.Editors {
	public class AsyncServerModeContextDescriptor : EvaluatorContextDescriptor {
		private ColumnView view;
		private IObjectSpace objectSpace;
		private Type type;
		public AsyncServerModeContextDescriptor(ColumnView view, IObjectSpace objectSpace, Type type) {
			this.view = view;
			this.objectSpace = objectSpace;
			this.type = type;
		}
		public override System.Collections.IEnumerable GetCollectionContexts(object source, string collectionName) {
			return objectSpace.GetEvaluatorContextDescriptor(type).GetCollectionContexts(GetObject(source), collectionName);
		}
		public override EvaluatorContext GetNestedContext(object source, string propertyPath) {
			return objectSpace.GetEvaluatorContextDescriptor(type).GetNestedContext(GetObject(source), propertyPath);
		}
		public override object GetPropertyValue(object source, EvaluatorProperty propertyPath) {
			GridColumn column = view.Columns.ColumnByFieldName(propertyPath.PropertyPath);
			if(column == null)
				return objectSpace.GetEvaluatorContextDescriptor(type).GetPropertyValue(GetObject(source), propertyPath);
			else {
				int? rowHandle = (int?)source;
				if(!rowHandle.HasValue)
					return null;
				else
					return view.GetRowCellValue(rowHandle.Value, column);
			}
		}
		public override System.Collections.IEnumerable GetQueryContexts(object source, string queryTypeName, CriteriaOperator condition, int top) {
			return objectSpace.GetEvaluatorContextDescriptor(type).GetQueryContexts(GetObject(source), queryTypeName, condition, top);
		}
		private object GetObject(object source) {
			GridColumn keyColumn = view.Columns.ColumnByFieldName(objectSpace.GetKeyPropertyName(type));
			if(keyColumn == null)
				throw new ArgumentException("keyColumn");
			int? rowHandle = (int?)source;
			if(!rowHandle.HasValue)
				return null;
			else
				return objectSpace.GetObjectByKey(type, view.GetRowCellValue(rowHandle.Value, keyColumn));
		}
	}
}
