#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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
using System.Linq;
using System;
using DevExpress.XtraPrinting.Localization;
using System.Collections;
using DevExpress.DocumentServices.ServiceModel.Native;
namespace DevExpress.DocumentServices.ServiceModel {
	public class DefaultValueParameterContainer : IParameterContainer {
		readonly List<DefaultValueParameter> parameters = new List<DefaultValueParameter>(0);
		public void CopyFrom(ClientParameterContainer container) {
			foreach(ClientParameter parameter in container) {
				var defaultParameter = new DefaultValueParameter(parameter.Path);
				defaultParameter.CopyFrom(parameter);
				parameters.Add(defaultParameter);
			}
		}
		public bool CopyTo(ClientParameterContainer reportParameters, out Exception error) {
			error = null;
			var badParameterPaths = new List<string>(0);
			foreach(var parameter in parameters) {
				var reportParameter = reportParameters[parameter.Path];
				if(reportParameter == null)
					badParameterPaths.Add(parameter.Path);
				else
					parameter.CopyTo(reportParameter);
			}
			if(badParameterPaths.Count > 0) {
				var joinedParameterPaths = badParameterPaths.Select(path => string.Format("'{0}'", path));
				error = new Exception(string.Format(PreviewLocalizer.GetString(PreviewStringId.Msg_NoParameters), string.Join(", ", joinedParameterPaths)));
			}
			return error == null;
		}
		#region IParameterContainer
		public int Count {
			get { return parameters.Count; }
		}
		public IClientParameter this[string path] {
			get {
				var parameter = parameters.FirstOrDefault(p => p.Path == path);
				if(parameter == null) {
					parameter = new DefaultValueParameter(path);
					parameters.Add(parameter);
				}
				return parameter;
			}
		}
		public IEnumerator<IClientParameter> GetEnumerator() {
			return parameters.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		#endregion
	}
}
