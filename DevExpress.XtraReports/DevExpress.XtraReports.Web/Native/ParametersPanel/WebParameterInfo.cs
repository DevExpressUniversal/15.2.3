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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Utils;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Parameters;
namespace DevExpress.XtraReports.Web.Native.ParametersPanel {
	public class WebParameterInfo<T> {
		IExtensionsProvider extensionsProvider;
		ParameterPath parameterPath;
		object value;
		public string Path {
			get { return parameterPath.Path; }
		}
		public Parameter Parameter {
			get { return parameterPath.Parameter; }
		}
		public T EditorInformation { get; set; }
		public object Value {
			get {
				if(value == null) {
					value = GetValue();
				}
				return value;
			}
		}
		public string Caption {
			get {
				return string.IsNullOrEmpty(Parameter.Description)
					? Parameter.Name
					: Parameter.Description;
			}
		}
		public bool SupportCascadeLookup {
			get {
				var lookUpSettings = Parameter.LookUpSettings;
				return lookUpSettings != null && !string.IsNullOrEmpty(lookUpSettings.FilterString);
			}
		}
		public bool UseCascadeLookup { get; set; }
		public WebParameterInfo(Parameter parameter, string path, T editorInformation, IExtensionsProvider extensionsProvider)
			: this(new ParameterPath(parameter, path), editorInformation, extensionsProvider) {
		}
		public WebParameterInfo(ParameterPath parameterPath, IExtensionsProvider extensionsProvider) {
			Guard.ArgumentNotNull(parameterPath, "parameterPath");
			this.parameterPath = parameterPath;
			this.extensionsProvider = extensionsProvider;
			EditorInformation = default(T);
		}
		public WebParameterInfo(ParameterPath parameterPathArg, T editorInformationArg, IExtensionsProvider extensionsProvider)
			: this(parameterPathArg, extensionsProvider) {
			EditorInformation = editorInformationArg;
		}
		public void Update(IDictionary<string, Parameter> parametersByPaths, IExtensionsProvider extensionsProvider) {
			this.extensionsProvider = extensionsProvider;
			var path = parameterPath.Path;
			Parameter updatedParameter;
			if(parametersByPaths.TryGetValue(path, out updatedParameter)) {
				parameterPath = new ParameterPath(updatedParameter, path);
			}
		}
		object GetValue() {
			var typeCode = Type.GetTypeCode(Parameter.Type);
			if(Parameter.Owner == null || typeCode != TypeCode.Object || Parameter.Type == typeof(Guid) || Parameter.Value == null) {
				return Parameter.Value;
			}
			if(string.IsNullOrEmpty(Parameter.ValueInfo) && Parameter.Owner != null) {
				return Parameter.MultiValue
					? (object)GetMultiValueViaSerializationService(Parameter.Value as IEnumerable, extensionsProvider)
					: GetValueViaSerializationService(Parameter.Value, extensionsProvider);
			}
			return Parameter.ValueInfo;
		}
		static string GetValueViaSerializationService(object value, IExtensionsProvider extensionsProvider) {
			string result;
			SerializationService.SerializeObject(value, out result, extensionsProvider);
			return result;
		}
		static string[] GetMultiValueViaSerializationService(IEnumerable values, IExtensionsProvider extensionsProvider) {
			if(values == null) {
				return new string[0];
			}
			return values
				.Cast<object>()
				.Select(x => GetValueViaSerializationService(x, extensionsProvider))
				.ToArray();
		}
	}
}
