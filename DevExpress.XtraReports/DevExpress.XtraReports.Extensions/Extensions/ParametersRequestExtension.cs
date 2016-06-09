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
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Parameters {
	using DevExpress.XtraEditors;
	public class EditParameterInfo {
		ParameterInfo parameterInfo;
		public Parameter Parameter {
			get { return parameterInfo.Parameter; }
		}
		public BaseEdit Editor {
			get {
				return parameterInfo.Editor as BaseEdit;
			}
			set {
				parameterInfo.Editor = value;
			}
		}
		public EditParameterInfo(ParameterInfo parameterInfo) {
			this.parameterInfo = parameterInfo;
		}
	}
}
namespace DevExpress.XtraReports.Extensions {
	public abstract class ParametersRequestExtension : IParametersRequestListener {
		public static void RegisterExtension(ParametersRequestExtension extension, string contextName) {
			if(string.IsNullOrEmpty(contextName))
				throw new ArgumentException("contextName");
			if(extension == null)
				throw new ArgumentNullException("extension");
			ParametersRequestService.RegisterListener(contextName, extension);
		}
		public static void AssociateReportWithExtension(XtraReport report, string contextName) {
			report.Extensions[ParametersRequestService.Guid] = contextName;
		}
		#region IParameterPanelListener Members
		void IParametersRequestListener.OnEditorValueChanged(IList<ParameterInfo> parametersInfo, ParameterInfo changedInstance, IEditingContext context) {
			OnEditorValueChanged(ToEditParametersInfo(parametersInfo), new EditParameterInfo(changedInstance), context.RootComponent as XtraReport);
		}
		static IList<EditParameterInfo> ToEditParametersInfo(IList<ParameterInfo> parametersInfo) {
			List<EditParameterInfo> result = new List<EditParameterInfo>();
			foreach(ParameterInfo item in parametersInfo)
				result.Add(new EditParameterInfo(item));
			return result;
		}
		protected virtual void OnEditorValueChanged(IList<EditParameterInfo> parametersInfo, EditParameterInfo changedInstance, XtraReport report) {
		}
		void IParametersRequestListener.OnValueChanged(IList<ParameterInfo> parametersInfo, ParameterInfo changedInstance, IEditingContext context) {
			OnValueChanged(ToEditParametersInfo(parametersInfo), new EditParameterInfo(changedInstance), context.RootComponent as XtraReport);
		}
		protected virtual void OnValueChanged(IList<EditParameterInfo> parametersInfo, EditParameterInfo changedInstance, XtraReport report) {
		}
		void IParametersRequestListener.OnBeforeShow(IList<ParameterInfo> parametersInfo, IEditingContext context) {
			OnBeforeShow(ToEditParametersInfo(parametersInfo), context.RootComponent as XtraReport);
		}
		protected virtual void OnBeforeShow(IList<EditParameterInfo> parametersInfo, XtraReport report) {
		}
		void IParametersRequestListener.OnSubmit(IList<ParameterInfo> parametersInfo, IEditingContext context) {
			OnSubmit(ToEditParametersInfo(parametersInfo), context.RootComponent as XtraReport);
		}
		protected virtual void OnSubmit(IList<EditParameterInfo> parametersInfo, XtraReport report) {
		}
		#endregion
	}
}
