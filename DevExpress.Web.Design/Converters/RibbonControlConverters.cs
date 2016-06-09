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

using DevExpress.Web.Design;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.Design;
namespace DevExpress.Web.Design {
	public class RibbonControlIDConverter : TypeConverter {
		public RibbonControlIDConverter() {
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return sourceType == typeof(string);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if(value == null)
				return string.Empty;
			if(value is string)
				return (string)value;
			throw base.GetConvertFromException(value);
		}
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			List<string> result = new List<string>();
			if(context != null) {
				IComponent component = context.Instance as IComponent;
				if(component == null && context.Instance is DesignerActionList)
					component = ((DesignerActionList)context.Instance).Component as IComponent;
				if(component != null) {
					Dictionary<string, ASPxRibbon> dictionary = GetRibbonControlsDictionary(component);
					foreach(string id in dictionary.Keys)
						result.Add(id);
				}
			}
			return new TypeConverter.StandardValuesCollection(result);
		}
		public static MasterPage LookupFakeMasterPage(DesignerCollection designers, WebFormsRootDesigner rootDesigner) {
			foreach(IDesignerHost host in designers) {
				foreach(IComponent comp in host.Container.Components) {
					MasterPage masterPage = comp as MasterPage;
					if(masterPage != null) {
						WebFormsRootDesigner designer = host.GetDesigner(masterPage) as WebFormsRootDesigner;
						if(designer != null && designer.DocumentUrl == rootDesigner.DocumentUrl)
							return masterPage;
					}
				}
			}
			return null;
		}
		public static MasterPage LookupMasterPage(DesignerCollection designers, MasterPage fakaMasterPage) {
			foreach(IDesignerHost host in designers) {
				MasterPage result = host.Container.Components[0] as MasterPage;
				if(result == null || result == fakaMasterPage)
					continue;
				foreach(IComponent comp in fakaMasterPage.Site.Container.Components) {
					if((comp as Page) != null || (comp as MasterPage) != null)
						continue;
					if(GetComponentByName(host.Container.Components, comp.Site.Name) == null) {
						result = null;
						break;
					}
				}
				if(result != null)
					return result;
			}
			return null;
		}
		public static IComponent GetComponentByName(ComponentCollection components, string name) {
			foreach(IComponent comp in components) {
				if(comp.Site.Name == name)
					return comp;
			}
			return null;
		}
		public static List<ASPxRibbon> GetRibbonControls(ComponentCollection components) {
			List<ASPxRibbon> controls = new List<ASPxRibbon>();
			foreach(IComponent comp in components) {
				ASPxRibbon ribbon = comp as ASPxRibbon;
				if(ribbon != null)
					controls.Add(ribbon);
			}
			return controls;
		}
		public static Dictionary<string, ASPxRibbon> GetRibbonControlsDictionary(IComponent component) {
			Dictionary<string, ASPxRibbon> result = new Dictionary<string, ASPxRibbon>();
			ISite site = component.Site;
			if(site != null) {
				IDesignerHost host = (IDesignerHost)site.GetService(typeof(IDesignerHost));
				if(host != null) {
					List<ASPxRibbon> controls = GetRibbonControls(host.Container.Components);
					foreach(ASPxRibbon control in controls)
						result.Add(control.ID, control);
				}
				IDesignerEventService eventService = (IDesignerEventService)site.GetService(typeof(IDesignerEventService));
				if(eventService != null) {
					WebFormsRootDesigner rootDesigner = (host.GetDesigner(component) as ASPxWebControlDesigner).DesignerRoot;
					MasterPage fakeMasterPage = LookupFakeMasterPage(eventService.Designers, rootDesigner);
					if(fakeMasterPage != null) {
						MasterPage masterPage = LookupMasterPage(eventService.Designers, fakeMasterPage);
						if(masterPage != null) {
							IDesignerHost designerHost = (IDesignerHost)masterPage.Site.GetService(typeof(IDesignerHost));
							List<ASPxRibbon> controls = GetRibbonControls(masterPage.Site.Container.Components);
							foreach(ASPxRibbon control in controls)
								result.Add(string.Format("{0}.{1}", designerHost.RootComponentClassName, control.ID), control);
						}
					}
				}
			}
			return result;
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return false;
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return context.Instance is IComponent || context.Instance is DesignerActionList;
		}
	}
}
