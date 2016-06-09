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
	public class BinaryDataRequestProcessorFromDatabase : IBinaryDataRequestProcessor {
		public const string BinaryDataProcessorKey = "BinaryDPId";
		public const string BinaryDataRequestProcessorFromDatabaseId = "BinaryDataRDBProcessor";
		public NameValueCollection GetParameters(IObjectSpace objectSpace, object currentObject, string propertyName, IMemberInfo memberInfo) {
			NameValueCollection result = new NameValueCollection();
			result[BinaryDataProcessorKey] = Id;
			if(currentObject != null) {
				result["ObjectHandle"] = objectSpace.GetObjectHandle(currentObject);
			}
			result["PropertyName"] = propertyName;
			RequestProcessorUrlKeyParameterHelper.UpdateParameters(result, currentObject, memberInfo);
			return result;
		}
		public byte[] GetBinaryDataByParameters(NameValueCollection parameters, out string contentType) {
			byte[] binaryData = null;
			contentType = string.Empty;
			Type objectType;
			IObjectSpace objectSpace;
			string objectKeyAsString;
			string objectHandle = parameters["ObjectHandle"];
			string propertyName = parameters["PropertyName"];
			ObjectHandleHelper.TryParseObjectHandle(XafTypesInfo.Instance, objectHandle, out objectType, out objectKeyAsString);
#if DebugTest
			DebugTest_QueryObjectSpaceEventArgs queryObjectSpaceEventArgs = new DebugTest_QueryObjectSpaceEventArgs();
			if(DebugTest_QueryObjectSpace != null) {
				DebugTest_QueryObjectSpace(this, queryObjectSpaceEventArgs);
			}
			objectSpace = queryObjectSpaceEventArgs.ObjectSpace;
			if(objectSpace == null) {
				objectSpace = WebApplication.Instance.CreateObjectSpace(objectType);
			}
#else
			objectSpace = WebApplication.Instance.CreateObjectSpace(objectType);
#endif
			if(objectType != null && !String.IsNullOrEmpty(objectKeyAsString)) {
				object key = objectSpace.GetObjectKey(objectType, objectKeyAsString);
				BinaryOperator criteria = new BinaryOperator(objectSpace.GetKeyPropertyName(objectType), key);
				XafDataView dataView = (XafDataView)objectSpace.CreateDataView(objectType, propertyName, criteria, null);
				if(dataView.Count > 0) {
					binaryData = dataView[0][propertyName] as byte[];
				}
			}
			return binaryData;
		}
		public string Id {
			get {
				return BinaryDataRequestProcessorFromDatabaseId;
			}
		}
#if DebugTest
		public event EventHandler<DebugTest_QueryObjectSpaceEventArgs> DebugTest_QueryObjectSpace;
#endif
	}
}
