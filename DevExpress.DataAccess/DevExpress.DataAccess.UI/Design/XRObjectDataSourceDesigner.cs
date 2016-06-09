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

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using DevExpress.DataAccess.Native.EntityFramework;
using DevExpress.DataAccess.ObjectBinding;
using DevExpress.Entity.ProjectModel;
namespace DevExpress.DataAccess.UI.Design {
	public class XRObjectDataSourceDesigner : ComponentDesigner {
		ISolutionTypesProvider solutionTypesProvider;
		IDesignerHost designerHost;
		protected ObjectDataSource DataSource { get { return (ObjectDataSource)Component; } }
		#region Overrides of ComponentDesigner
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			CreateServices();
			AddServicesToDesignerHost();
		}
		#endregion
		void CreateServices() {
			designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
			solutionTypesProvider = CreateSolutionTypesProvider();
		}
		protected virtual ISolutionTypesProvider CreateSolutionTypesProvider() {
			return EntityServiceHelper.GetRuntimeSolutionProvider(Assembly.GetEntryAssembly());
		}
		void AddServicesToDesignerHost() {
			AddServiceToDesignerHost(solutionTypesProvider);
		}
		void AddServiceToDesignerHost<T>(T service) {
			if(designerHost.GetService(typeof(T)) == null)
				designerHost.AddService(typeof(T), service);
		}
	}
}
