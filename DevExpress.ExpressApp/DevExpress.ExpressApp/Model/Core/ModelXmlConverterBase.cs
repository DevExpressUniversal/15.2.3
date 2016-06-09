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
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Model.Core {
	public abstract class ModelXmlConverterBase {
		private ConvertXmlParameters convertXmlParameters;
		public void Convert(ConvertXmlParameters convertXmlParameters) {
			Guard.ArgumentNotNull(convertXmlParameters, "convertXmlParameters");
			this.convertXmlParameters = convertXmlParameters;
			ConvertCore();
			this.convertXmlParameters = null;
		}
		protected abstract void ConvertCore();
		protected Version GetModuleXmlVersion(string moduleName) {
			IModelApplicationServices services = convertXmlParameters.Node.Root as IModelApplicationServices;
			return services != null ? services.GetModuleVersion(moduleName) : null;
		}
		protected bool ContainsValue(string valueName) {
			return convertXmlParameters.Values.ContainsKey(valueName);
		}
		protected bool ContainsValueIgnoreCase(string valueName, out string actualValueName) {
			if(ContainsValue(valueName)) {
				actualValueName = valueName;
				return true;
			}
			foreach(string key in convertXmlParameters.Values.Keys) {
				if(string.Compare(key, valueName, true) == 0) {
					actualValueName = key;
					return true;
				}
			}
			actualValueName = null;
			return false;
		}
		protected string GetValue(string valueName) {
			return convertXmlParameters.Values[valueName];
		}
		protected bool TryGetValue(string valueName, out string value) {
			return convertXmlParameters.Values.TryGetValue(valueName, out value);
		}
		protected void SetValue(string valueName, string newValue) {
			convertXmlParameters.Values[valueName] = newValue;
		}
		protected IEnumerable<Tuple<string, ModelValueInfo>> GetXmlValueNamesWithModelValueInfo() {
			List<Tuple<string, ModelValueInfo>> result = new List<Tuple<string, ModelValueInfo>>();
			Type nodeType = convertXmlParameters.NodeType;
			if(nodeType != null && convertXmlParameters.Values.Count > 0) {
				ModelNodeInfo nodeInfo = convertXmlParameters.Node.CreatorInstance.GetNodeInfo(nodeType);
				foreach(string xmlValueName in new List<string>(convertXmlParameters.Values.Keys)) {
					string valueName = nodeInfo.GetValueNameByXmlName(xmlValueName);
					ModelValueInfo valueInfo = nodeInfo.GetValueInfo(valueName);
					if(valueInfo != null) {
						result.Add(new Tuple<string, ModelValueInfo>(xmlValueName, valueInfo));
					}
				}
			}
			return result;
		}
		protected ConvertXmlParameters ConvertXmlParameters {
			get { return convertXmlParameters; }
		}
		protected IModelSources ModelSources {
			get { return convertXmlParameters.Node.Root as IModelSources; }
		}
	}
}
