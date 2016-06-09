#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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
using DevExpress.XtraReports.Web.Native.ParametersPanel;
namespace DevExpress.XtraReports.Web.Native {
	public abstract class DictionaryJSON : Dictionary<string, object> {
		protected DictionaryJSON()
			: base(StringComparer.Ordinal) {
		}
		protected DictionaryJSON(int capacity)
			: base(capacity, StringComparer.Ordinal) {
		}
	}
	public class ParameterJSON : DictionaryJSON {
		public ParameterJSON(ASPxParameterInfo parameterInfo)
			: base(4) {
			Add("editorId", parameterInfo.EditorInformation == null ? "" : parameterInfo.EditorInformation.ClientID);
			Add("value", GetParameterInfoValue(parameterInfo));
			Add("useCascadeLookup", parameterInfo.UseCascadeLookup);
			Add("visible", parameterInfo.Parameter.Visible);
		}
		static object GetParameterInfoValue(ASPxParameterInfo parameterInfo) {
			if(parameterInfo.EditorInformation == null) {
				return parameterInfo.Value;
			}
			return ComboboxDateTimeLookupFix.HasMark(parameterInfo.EditorInformation)
				? ComboboxDateTimeLookupFix.ToJSON(parameterInfo.Value)
				: parameterInfo.Value;
		}
	}
	public class ParametersJSON : DictionaryJSON {
		public ParametersJSON(ASPxParameterInfo[] parameters)
			: base(parameters.Length) {
			foreach(var parameterInfo in parameters) {
				var parametersJSON = new ParameterJSON(parameterInfo);
				Add(parameterInfo.Path, parametersJSON);
			}
		}
	}
}
