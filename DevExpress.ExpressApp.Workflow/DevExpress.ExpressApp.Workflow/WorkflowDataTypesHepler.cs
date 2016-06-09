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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using System.Text;
using DevExpress.ExpressApp.Design;
using DevExpress.ExpressApp.Utils.Reflection;
using DevExpress.Utils.Design;
namespace DevExpress.ExpressApp.Workflow {
	internal static class WorkflowDataTypesHepler {
		public static Type FindDefaultWorkflowDataType<T>(ITypeDiscoveryService typeService) {
			Type result = null;
			if(DesignTimeTools.IsDesignMode && typeService != null) {				
				if(typeService != null) {
					ICollection types = DesignHelper.FindBusinessClassDescendants<T>(typeService, true);
					result = types.OfType<Type>().FirstOrDefault(f => f.IsClass);
				}
			}
			else {
				result = FindDefaultWorkflowDataType<T>();
			}
			return result;
		}
		public static Type FindDefaultWorkflowDataType<T>() {
			Type result = null;
			String xpoAssemblyName = "DevExpress.Persistent.BaseImpl" + XafAssemblyInfo.VersionSuffix;
			String efAssemblyName = "DevExpress.Persistent.BaseImpl.EF" + XafAssemblyInfo.VersionSuffix;
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			Assembly targetAssembly = assemblies.SingleOrDefault(assembly => AssemblyHelper.GetName(assembly) == xpoAssemblyName);
			if(targetAssembly == null) {
				targetAssembly = assemblies.SingleOrDefault(assembly => AssemblyHelper.GetName(assembly) == efAssemblyName);
			}
			if(targetAssembly != null) {
				IList<Type> types = AssemblyHelper.GetTypes(targetAssembly, type => typeof(T).IsAssignableFrom(type) && type.IsClass);
				result = types.OfType<Type>().FirstOrDefault(f => DesignHelper.IsValidBusinessClass(f, true));
			}
			return result;
		}
	}
}
