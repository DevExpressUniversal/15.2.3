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
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Xpo {
	public class XpoDataView : XafDataView {
		private XPView xpView;
		private CriteriaOperator filter;
		private Boolean isFilterChanged;
		protected override void InitObjects() {
			if(objects == null) {
				objects = new List<XafDataViewRecord>();	
				objectsDictionary = new Dictionary<Object, XafDataViewRecord>();
				Int32 topReturnedRecords = topReturnedObjectsCount;
				if(topReturnedRecords == 0) {
					topReturnedRecords = Int32.MaxValue;
				}
				xpView.Properties.Clear();
				foreach(DataViewExpression dataViewExpression in expressions) {
					ViewProperty viewProperty = new ViewProperty();
					viewProperty.Name = dataViewExpression.Name;
					viewProperty.Property = dataViewExpression.Expression;
					xpView.Properties.Add(viewProperty);
				}
				xpView.Criteria = criteria;
				xpView.Filter = filter;
				xpView.Sorting = XPObjectSpace.CreateSortingCollection(sorting);
				xpView.TopReturnedRecords = topReturnedRecords;
				foreach(ViewRecord obj in xpView) {
					AddDataViewRecordToObjects(new XpoDataViewRecord(this, obj));
				}
			}
			else if(isFilterChanged) {
				objects.Clear();
				objectsDictionary.Clear();
				xpView.Filter = filter;
				foreach(ViewRecord obj in xpView) {
					AddDataViewRecordToObjects(new XpoDataViewRecord(this, obj));
				}
			}
			isFilterChanged = false;
		}
		public XpoDataView(XPObjectSpace objectSpace, Type objectType, IList<DataViewExpression> expressions, CriteriaOperator criteria, IList<SortProperty> sorting)
			: base(objectSpace, objectType, expressions, criteria, sorting) {
			Type type = objectType;
			if(objectType.IsInterface) {
				type = PersistentInterfaceHelper.GetPersistentInterfaceDataType(objectType);
			}
			xpView = new XPView(((XPObjectSpace)objectSpace).Session, type);
		}
		public XpoDataView(XPObjectSpace objectSpace, Type objectType, String expressions, CriteriaOperator criteria, IList<SortProperty> sorting)
			: this(objectSpace, objectType, BaseObjectSpace.ConvertExpressionsStringToExpressionsList(expressions), criteria, sorting) {
		}
		public override void Dispose() {
			base.Dispose();
			if(xpView != null) {
				xpView.Dispose();
				xpView = null;
			}
		}
		public CriteriaOperator Filter {
			get { return filter; }
			set {
				if(!ReferenceEquals(filter, value)) {
					filter = value;
					isFilterChanged = true;
					RaiseListChangedEvent(new ListChangedEventArgs(ListChangedType.Reset, 0));
				}
			}
		}
	}
}
