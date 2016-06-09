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
using DevExpress.Persistent.Base.General;
using System.Data;
using DevExpress.XtraScheduler;
using DevExpress.Web.ASPxScheduler.Internal;
using System.Collections;
using DevExpress.ExpressApp.Web.Core;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Scheduler.Web {
	public class SourceObjectHelperWeb : SourceObjectHelperBase {
		private Type objectType;
		private CollectionSourceBase collectionSource;
		private string GetKeyProperty() {
			string keyProperty = ObjectSpace.GetKeyPropertyName(ObjectType);
			if(String.IsNullOrEmpty(keyProperty)) {
				keyProperty = "AppointmentId";
			}
			return keyProperty;
		}
		private object GetKeyValue(object targetObject) {
			object keyValue = null;
			string keyProperty = GetKeyProperty();
			if(targetObject is DataRowView) {
				DataRowView dataRowView = (DataRowView)targetObject;
				keyValue = dataRowView[keyProperty];
			}
			else if(targetObject is Appointment) {
				keyValue = AppointmentIdHelper.GetAppointmentId((Appointment)targetObject);
			}
			return keyValue;
		}
		public SourceObjectHelperWeb(IObjectSpace objectSpace, CollectionSourceBase collectionSource) {
			Guard.ArgumentNotNull(collectionSource, "collectionSource");
			Guard.ArgumentNotNull(objectSpace, "objectSpace");
			this.objectSpace = objectSpace;
			this.collectionSource = collectionSource;
			this.objectType = collectionSource.ObjectTypeInfo.Type;
		}
		public IObjectSpace ObjectSpace {
			get {
				return objectSpace;
			}
		}
		public Type ObjectType {
			get {
				return objectType;
			}
		}
		public override object GetObjectInCollectionSource(object targetObject) {
			object result = null;
			if(targetObject is XafDataViewRecord) {
				result = targetObject;
			}
			else if(targetObject is DataRowView && collectionSource.DataAccessMode == CollectionSourceDataAccessMode.DataView) {
				object keyValue = GetKeyValue(targetObject);
				foreach(XafDataViewRecord dataRecord in (IList)collectionSource.Collection) {
					if(dataRecord[GetKeyProperty()].Equals(keyValue)) {
						result = dataRecord;
						break;
					}
				}
			}
			else {
				result = GetSourceObject(targetObject);
			}
			return result;
		}
		public override IEvent GetSourceObject(object targetObject) {
			if(targetObject is IEvent) {
				return (IEvent)targetObject;
			}
			if(targetObject is XafDataViewRecord) {
				return objectSpace.GetObject(targetObject) as IEvent; ;
			}
			object keyValue = GetKeyValue(targetObject);
			if(keyValue != null && keyValue != System.DBNull.Value) {
				if(collectionSource.ObjectTypeInfo.IsPersistent) {
					return ObjectSpaceHelper.FindObjectByKey(objectSpace, objectType, keyValue) as IEvent;
				}
				else {
					foreach(object schedulerEvent in (IList)collectionSource.Collection) {
						if(((IEvent)schedulerEvent).AppointmentId.Equals(keyValue)) {
							return (IEvent)schedulerEvent;
						}
					}
				}
			}
			return null;
		}
	}
}
