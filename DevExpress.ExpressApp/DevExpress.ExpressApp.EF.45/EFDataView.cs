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
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using DevExpress.Xpo;
using DevExpress.Xpo.DB.Exceptions;
using DevExpress.Data.Filtering;
using DevExpress.Data.Linq.Helpers;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.EF {
	public class EFDataView : XafDataView {
		protected override void InitObjects() {
			if(objects == null) {
				objects = new List<XafDataViewRecord>();
				objectsDictionary = new Dictionary<Object, XafDataViewRecord>();
				IQueryable<DbDataRecord> dataRecordsQuery = null;
				if(topReturnedObjectsCount <= 0) {
					dataRecordsQuery = ((EFObjectSpace)objectSpace).GetDataRecordsQuery(objectType, expressions, criteria, sorting);
				}
				else {
					dataRecordsQuery = ((EFObjectSpace)objectSpace).GetDataRecordsQuery(objectType, expressions, criteria, sorting).Take(topReturnedObjectsCount);
				}
				try {
					foreach(DbDataRecord obj in dataRecordsQuery) {
						AddDataViewRecordToObjects(new EFDataViewRecord(this, obj));
					}
				}
				catch(Exception exception) {
					SqlException sqlException = EFObjectSpace.GetSqlException(exception);
					if((sqlException != null) && (sqlException.Number == EFObjectSpace.UnableToOpenDatabaseErrorNumber)) {
						throw new UnableToOpenDatabaseException("", exception);
					}
					else {
						throw;
					}
				}
			}
		}
		public EFDataView(EFObjectSpace objectSpace, Type objectType, IList<DataViewExpression> expressions, CriteriaOperator criteria, IList<SortProperty> sorting)
			: base(objectSpace, objectType, expressions, criteria, sorting) {
		}
		public EFDataView(EFObjectSpace objectSpace, Type objectType, String expressions, CriteriaOperator criteria, IList<SortProperty> sorting)
			: this(objectSpace, objectType, BaseObjectSpace.ConvertExpressionsStringToExpressionsList(expressions), criteria, sorting) {
		}
	}
}
