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
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Reflection;
namespace DevExpress.Utils.Design {
	#region AutomaticBindHelper
	#if DXSchedulerReports
	static
	#else
	public 
	#endif
		class AutomaticBindHelper {
		static IDesignerHost GetHost(ComponentDesigner designer) {
			if (designer == null)
				return null;
			if (designer.Component == null)
				return null;
			ISite site = designer.Component.Site;
			if (site == null)
				return null;
			return (IDesignerHost)site.GetService(typeof(IDesignerHost));
		}
		public static bool BindToComponent(ComponentDesigner designer, string propertyName, Type baseType) {
			IDesignerHost host = GetHost(designer);
			if (host == null)
				return false;
			PropertyInfo prop = GetPropertyInfo(designer, propertyName, baseType);
			if (prop == null)
				return false;
			if (prop.GetValue(designer.Component, null) != null)
				return false;
			foreach (IComponent comp in host.Container.Components) {
				if (baseType.IsAssignableFrom(comp.GetType())) {
					prop.SetValue(designer.Component, comp, null);
					return true;
				}
			}
			return false;
		}
		public static void UnbindFromRemovedComponent(ComponentDesigner designer, IComponent componentGoingToRemove, string propertyName, Type baseType) {
			if (componentGoingToRemove == null || designer == null)
				return;
			if (baseType.IsAssignableFrom(componentGoingToRemove.GetType())) {
				PropertyInfo prop = AutomaticBindHelper.GetPropertyInfo(designer, propertyName, baseType);
				if (prop != null) {
					if (componentGoingToRemove.Equals(prop.GetValue(designer.Component, null)))
						prop.SetValue(designer.Component, null, null);
				}
			}
		}
		public static PropertyInfo GetPropertyInfo(ComponentDesigner designer, string name, Type baseType) {
			if (designer.Component == null)
				return null;
			PropertyInfo prop = designer.Component.GetType().GetProperty(name, baseType);
			if (prop == null)
				return null;
			if (!baseType.IsAssignableFrom(prop.PropertyType))
				return null;
			return prop;
		}
	}
	#endregion
}
