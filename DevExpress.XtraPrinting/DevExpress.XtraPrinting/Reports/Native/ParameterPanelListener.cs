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

using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.XtraReports.Parameters;
namespace DevExpress.XtraReports.Native {
	public interface IParametersRequestListener {
		void OnEditorValueChanged(IList<ParameterInfo> parametersInfo, ParameterInfo changedInstance, IEditingContext context);
		void OnValueChanged(IList<ParameterInfo> parametersInfo, ParameterInfo changedInstance, IEditingContext context);
		void OnBeforeShow(IList<ParameterInfo> parametersInfo, IEditingContext context);
		void OnSubmit(IList<ParameterInfo> parametersInfo, IEditingContext context);
	}
	public class ParametersRequestService : InstanceProvider<IParametersRequestListener> {
		public const string Guid = "ParametersRequestExtension";
		static readonly object padlock = new object();
		static ParametersRequestService service = new ParametersRequestService();
		public static void OnEditorValueChanged(IList<ParameterInfo> parametersInfo, ParameterInfo changedInstance, IReport report) {
			lock(padlock) {
				service.InvokeEditorValueChanged(parametersInfo, changedInstance, CreateContext(report));
			}
		}
		static EditingContext CreateContext(IReport report) {
			string value;
			report.Extensions.TryGetValue(ParametersRequestService.Guid, out  value);
			return new EditingContext(value, report);
		}
		public static void OnValueChanged(IList<ParameterInfo> parametersInfo, ParameterInfo changedInstance, IReport report) {
			lock(padlock) {
				service.InvokeValueChanged(parametersInfo, changedInstance, CreateContext(report));
			}
		}
		public static void OnBeforeShow(IList<ParameterInfo> parametersInfo, IReport report) {
			lock(padlock) {
				service.InvokeOnBeforeShow(parametersInfo, CreateContext(report));
			}
		}
		public static void OnSubmit(IList<ParameterInfo> parametersInfo, IReport report) {
			lock(padlock) {
				service.InvokeOnSubmit(parametersInfo, CreateContext(report));
			}
		}
		public static void RegisterListener(string contextName, IParametersRequestListener listener) {
			lock(padlock) {
				service.SetInstance(contextName, listener);
			}
		}
		void InvokeEditorValueChanged(IList<ParameterInfo> parametersInfo, ParameterInfo changedInstance, IEditingContext context) {
			IParametersRequestListener listener = GetInstance(context.Name);
			if(listener != null)
				listener.OnEditorValueChanged(parametersInfo, changedInstance, context);
		}
		void InvokeValueChanged(IList<ParameterInfo> parametersInfo, ParameterInfo changedInstance, IEditingContext context) {
			IParametersRequestListener listener = GetInstance(context.Name);
			if(listener != null)
				listener.OnValueChanged(parametersInfo, changedInstance, context);
		}
		void InvokeOnBeforeShow(IList<ParameterInfo> parametersInfo, IEditingContext context) {
			IParametersRequestListener listener = GetInstance(context.Name);
			if(listener != null)
				listener.OnBeforeShow(parametersInfo, context);
		}
		void InvokeOnSubmit(IList<ParameterInfo> parametersInfo, IEditingContext context) {
			IParametersRequestListener listener = GetInstance(context.Name);
			if(listener != null)
				listener.OnSubmit(parametersInfo, context);
		}
	}
}
