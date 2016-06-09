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
using DevExpress.ExpressApp.Utils;
using DevExpress.Printing.Core.PdfExport.Metafile;
using System.Runtime.Serialization;
using System.Security;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Security.ClientServer {
	[Serializable]
	public class SerializablePermissionRequest : IPermissionRequest, ISerializable, IObjectTypePermissionRequst {
		public SerializablePermissionRequest() { 
		}
		public SerializablePermissionRequest(Type objectType, string memberName, string targetObjectHandle, string operation) {
			Guard.ArgumentNotNull(objectType, "objectType");
			MemberName = memberName;
			TargetObjectHandle = targetObjectHandle;
			ObjectType = objectType;
			Operation = operation;
		}
		public SerializablePermissionRequest(SerializationInfo information, StreamingContext context) {
			Operation = information.GetString("Operation");
			MemberName = information.GetString("MemberName");
			string typeName = information.GetString("ObjectType");
			Guard.ArgumentNotNullOrEmpty(typeName, "information.GetString(\"ObjectType\")");
			ObjectType = ReflectionHelper.GetType(typeName);
			TargetObjectHandle = information.GetString("TargetObjectHandle");
		}
		public object GetHashObject() {		  
				StringBuilder key = new StringBuilder();
				key.Append(GetType().Name);
				key.Append(" - ");
				key.Append(ObjectType.FullName);
				key.Append(" - ");
				key.Append(Operation);
				key.Append(" - ");
				key.Append(TargetObjectHandle);
				key.Append(" - ");
				key.Append(MemberName);
				return key.ToString();
		}
		[SecurityCritical]
		public void GetObjectData(SerializationInfo info, StreamingContext context) {
			info.AddValue("Operation", Operation);
			info.AddValue("MemberName", MemberName);
			info.AddValue("ObjectType", ObjectType.FullName);
			info.AddValue("TargetObjectHandle", TargetObjectHandle);
		}
		public string MemberName { get; set; }
		public string TargetObjectHandle { get; set; }
		public Type ObjectType { get;  set; }
		public string Operation { get; set; }
	}
}
