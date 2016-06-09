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
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner.Native;
using System.Reflection;
namespace DevExpress.XtraReports.Design {
	[ProvideProperty("Name", typeof(IComponent))]
	public class NameExtender : IExtenderProvider {
		public NameExtender() {
		}
		#region System.ComponentModel.IExtenderProvider interface implementation
		public virtual bool CanExtend(object extendee) {
			if(extendee is IComponent) {
				IComponent component = extendee as IComponent;
				IDictionary properties = new Hashtable();
				properties["Name"] = new object();
				XRComponentDesignerBase.RaiseFilterComponentProperties(component, properties);
				if(!properties.Contains("Name"))
					return false;
			}
			InheritanceAttribute inheritanceAttribute = (InheritanceAttribute)TypeDescriptor.GetAttributes(extendee)[typeof(InheritanceAttribute)];
			return extendee is IComponent && inheritanceAttribute.Equals(InheritanceAttribute.NotInherited);
		}
		#endregion
		[DesignOnly(true),
		SRCategory(DevExpress.XtraReports.Localization.ReportStringId.CatDesign),
		Browsable(true),
		ParenthesizePropertyName(true),
		Description("The name of the component."),
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.XRControl.Name"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public string GetName(IComponent component) {
			ISite site = component.Site;
			if (site != null)
				return site.Name;
			return null;
		}
		public void SetName(IComponent component, string name) {
			ISite site = component.Site;
			if(site == null)
				return;
			IDesignerHost designerHost = (IDesignerHost)site.GetService(typeof(IDesignerHost));
			if(designerHost.Loading) return;
			INameCreationService nameCreationService = (INameCreationService)site.GetService(typeof(INameCreationService));
			if(!string.Equals(component.Site.Name, name, StringComparison.OrdinalIgnoreCase))
				nameCreationService.ValidateName(name);
			if (component is CalculatedField)
				((CalculatedField)component).ValidateNameUniqueness(name);
			Type componentType = component.GetType();
			PropertyInfo[] properties = componentType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			foreach(PropertyInfo property in properties) {
				if(property.Name == name)
					throw new ArgumentException("An incorrect Name value.");
			}			
			IComponentChangeService componentChangeService = site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(componentChangeService != null) {
				PropertyDescriptor pd = TypeDescriptor.GetProperties(component.GetType())["Name"];
				componentChangeService.OnComponentChanging(component, pd);
				string oldName = site.Name;
				site.Name = name;
				componentChangeService.OnComponentChanged(component, pd, oldName, name);
				if(componentChangeService is XRComponentChangeService)
					((XRComponentChangeService)componentChangeService).OnComponentRename(component, oldName, name);
			}
		}
	}
}
