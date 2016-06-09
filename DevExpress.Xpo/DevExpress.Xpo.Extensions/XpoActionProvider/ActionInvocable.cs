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
using System.Data.Services.Providers;
using System.Data.Services;
namespace DevExpress.Xpo {
	public class ActionInvokable : IDataServiceInvokable {
		bool hasRun;
		object result;
		readonly ActionInfo info;
		readonly string actionName;
		readonly object site;
		readonly object[] marshalledParameters;
		public ActionInvokable(DataServiceOperationContext operationContext, ServiceAction serviceAction, object site, object[] parameters, IParameterMarshaller marshaller) {
			actionName = serviceAction.Name;
			this.info = serviceAction.CustomState as ActionInfo;
			this.site = site;
			this.marshalledParameters = marshaller.Marshall(operationContext, serviceAction, parameters);
			info.AssertAvailable(site, marshalledParameters[0], true);
		}
		void CaptureResult(object o) {
			if (hasRun) throw new Exception("Invoke not available. This invokable has already been Invoked.");
			result = o;
			hasRun = true;
		}
		public object GetResult() {
			if (!hasRun) throw new Exception("Results not available. This invokable hasn't been Invoked.");
			return result;
		}
		public void Invoke() {
			try {
				CaptureResult(info.InvokeAction(site, marshalledParameters));
			} catch {
				throw new DataServiceException(
					500,
					string.Format("Exception executing action {0}", actionName)
				);
			}
		}
	}
}
