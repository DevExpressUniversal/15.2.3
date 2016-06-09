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
using System.Collections.Specialized;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
namespace DevExpress.ExpressApp.Web {
	public class BinaryDataRequestProcessorFromCurrentObject : IBinaryDataRequestProcessor {
		private IObjectSpace objectSpace;
		string processorId; 
		object newObject;
		object _currentObject;
		XafDataViewRecord dataViewRecord;
		public BinaryDataRequestProcessorFromCurrentObject(IObjectSpace objectSpace, object currentObject) {
			this.objectSpace = objectSpace;
			this._currentObject = currentObject;
			if(objectSpace.IsNewObject(currentObject)) {
				this.processorId = Guid.NewGuid().ToString();
				newObject = currentObject;
			}
			else {
				this.processorId = objectSpace.GetObjectHandle(currentObject);
			}
			if(currentObject is XafDataViewRecord) {
				dataViewRecord = (XafDataViewRecord)currentObject;
			}
		}
		public NameValueCollection GetParameters(string propertyName, IMemberInfo memberInfo) {
			NameValueCollection result = new NameValueCollection();
			result[BinaryDataRequestProcessorFromDatabase.BinaryDataProcessorKey] = processorId;
			result["PropertyName"] = propertyName;
			RequestProcessorUrlKeyParameterHelper.UpdateParameters(result, _currentObject, memberInfo);
			return result;
		}
		public byte[] GetBinaryDataByParameters(NameValueCollection parameters, out string contentType) {
			byte[] binaryData = null;
			contentType = string.Empty;
			string propertyName = parameters["PropertyName"];
			string objectHandle = parameters[BinaryDataRequestProcessorFromDatabase.BinaryDataProcessorKey];
			object value = null;
			if(dataViewRecord != null) {
				if(dataViewRecord.ContainsMember(propertyName)) {
					value = dataViewRecord[propertyName];
				}
				else {
					Type objectType;
					string objectKeyAsString;
					if(ObjectHandleHelper.TryParseObjectHandle(XafTypesInfo.Instance, objectHandle, out objectType, out objectKeyAsString)) {
						object key = objectSpace.GetObjectKey(objectType, objectKeyAsString);
						BinaryOperator criteria = new BinaryOperator(objectSpace.GetKeyPropertyName(objectType), key);
						XafDataView dataView = (XafDataView)objectSpace.CreateDataView(objectType, propertyName, criteria, null);
						if(dataView.Count > 0) {
							value = dataView[0][propertyName];
						}
					}
				}
			}
			else {
				object currentObject = newObject == null ? objectSpace.GetObjectByHandle(objectHandle) : newObject;
				if(currentObject != null) {
					ITypeInfo typeInfo = objectSpace.TypesInfo.FindTypeInfo(objectSpace.GetObjectType(currentObject));
					value = typeInfo.FindMember(propertyName).GetValue(currentObject);
				}
			}
			binaryData = value as byte[];
			contentType = DevExpress.Web.Internal.BinaryStorage.GetImageMimeType(binaryData);
			return binaryData;
		}
		public string Id {
			get {
				return processorId;
			}
		}
	}
}
