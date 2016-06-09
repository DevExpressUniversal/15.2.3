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

using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
namespace DevExpress.ExpressApp.Utils {
	public static class UserDifferencesHelper {
		public static IDictionary<string, string> GetUserDifferences(IModelNode modelNode) {
			Guard.ArgumentNotNull(modelNode, "modelNode");
			ModelNode lastLayer = ((ModelNode)modelNode).GetOrCreateWritableLayer();
			IModelApplicationServices modelServices = (IModelApplicationServices)modelNode.Root;
			ModelXmlWriter writer = new ModelXmlWriter();
			IDictionary<string, string> differences = new Dictionary<string, string>();
			for(int i = 0; i < modelServices.AspectCount; ++i) {
				differences.Add(modelServices.GetAspect(i), writer.WriteToString(lastLayer, i));
			}
			return differences;
		}
		public static void SetUserDifferences(IModelNode modelNode, IDictionary<string, string> differences) {
			Guard.ArgumentNotNull(modelNode, "modelNode");
			ModelNode lastLayer = ((ModelNode)modelNode).GetOrCreateWritableLayer();
			lastLayer.Undo();
			if(differences != null) {
				ModelXmlReader reader = new ModelXmlReader();
				foreach(KeyValuePair<string, string> difference in differences) {
					reader.ReadFromString(lastLayer, difference.Key, difference.Value);
				}
			}
		}
	}
}
