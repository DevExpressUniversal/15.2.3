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
using System.Text;
using System.Collections.Generic;
namespace DevExpress.ExpressApp.Security {
	[Obsolete("Use 'ServerPermissionRequestProcessor' Instead")]
	public class ClientPermissionRequestProcessor : IPermissionRequestProcessor {
		private List<IObjectSpace> nonSecuredObjectSpaces;
		private Dictionary<Type, IObjectSpace> objectSpacesMap;
		private IDictionary<Type, IPermissionRequestProcessor> processors;
		private IObjectSpace GetObjectSpace(Type objectType) {
			IObjectSpace result = null;
			if(nonSecuredObjectSpaces.Count == 1) {
				result = nonSecuredObjectSpaces[0];
			}
			else {
				if(!objectSpacesMap.TryGetValue(objectType, out result)) {
					foreach(IObjectSpace objectSpace in nonSecuredObjectSpaces) {
						if(objectSpace.CanInstantiate(objectType)) {
							result = objectSpace;
							objectSpacesMap[objectType] = objectSpace;
							break;
						}
					}
				}
			}
			return result;
		}
		public ClientPermissionRequestProcessor(IList<IObjectSpace> nonSecuredObjectSpaces, IDictionary<Type, IPermissionRequestProcessor> processors) {
			this.nonSecuredObjectSpaces = new List<IObjectSpace>();
			this.nonSecuredObjectSpaces.AddRange(nonSecuredObjectSpaces);
			this.processors = processors;
			objectSpacesMap = new Dictionary<Type, IObjectSpace>();
		}
		public bool IsGranted(ClientPermissionRequest clientPermissionRequest) {
			IObjectSpace nonSecuredObjectSpace = GetObjectSpace(clientPermissionRequest.ObjectType);
			object targetObject = !string.IsNullOrEmpty(clientPermissionRequest.TargetObjectHandle) ? nonSecuredObjectSpace.GetObjectByHandle(clientPermissionRequest.TargetObjectHandle) : null;
			ServerPermissionRequest request = new ServerPermissionRequest(clientPermissionRequest.ObjectType, targetObject,
					clientPermissionRequest.MemberName, clientPermissionRequest.Operation, new SecurityExpressionEvaluator(nonSecuredObjectSpace));
			return processors[request.GetType()].IsGranted(request);
		}
		bool IPermissionRequestProcessor.IsGranted(IPermissionRequest permissionRequest) {
			return IsGranted((ClientPermissionRequest)permissionRequest);
		}
	}
}
