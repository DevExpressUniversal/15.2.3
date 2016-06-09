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
using DevExpress.Workflow;
using DevExpress.ExpressApp.MiddleTier;
namespace DevExpress.ExpressApp.Workflow.ServiceModel {
	[Obsolete("Use the 'DevExpress.ExpressApp.MiddleTier.ServiceBase' instead.")]
	public abstract class ServiceBase : IService {
		protected virtual void OnInitialized() { }
		protected virtual bool OnCustomHandleException(Exception e) {
			CustomHandleExceptionEventArgs args = new CustomHandleExceptionEventArgs(e);
			if(CustomHandleException != null) {
				CustomHandleException(this, args);
			}
			return args.Handled;
		}
		public void Initialize(IServiceProviderEx serviceProvider) {
			ServiceProvider = serviceProvider;
			OnInitialized();
		}
		public IServiceProviderEx ServiceProvider { get; private set; }
		public T FindService<T>() {
			return ServiceProvider.GetService<T>();
		}
		public T GetService<T>() {
			T result = FindService<T>();
			Guard.ArgumentNotNull(result, "GetService(" + typeof(T).FullName + ")");
			return result;
		}
		public event EventHandler<CustomHandleExceptionEventArgs> CustomHandleException;
	}
}
