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
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using System.Reflection;
using System.ComponentModel.Design.Serialization;
using DevExpress.Serialization.Services;
using DevExpress.XtraPrinting.Native;
using System.Collections.Generic;
namespace DevExpress.XtraReports.UserDesigner.Native
{
	public class XRDesignComponentContainer : Container {
		public class ComponentSite : ISite, IDictionaryService {
			IComponent fComponent;
			string fName = String.Empty;
			SDictionaryService dictionaryService;
			XRDesignComponentContainer container;
			public ComponentSite(IComponent component, XRDesignComponentContainer container, string name) {
				this.container = container;
				this.fComponent = component;
				SetName(name);
				dictionaryService = new SDictionaryService();
			}
			#region IDictionaryService implementation
			object IDictionaryService.GetKey(object value) {
				return dictionaryService.GetKey(value);
			}
			void IDictionaryService.SetValue(object key, object value) {
				dictionaryService.SetValue(key, value);
			}
			object IDictionaryService.GetValue(object key) {
				return dictionaryService.GetValue(key);
			}
			#endregion
			public IComponent Component {
				get { return fComponent; }
			}
			public IContainer Container {
				get { return container; }
			}
			public bool DesignMode {
				get { return true; }
			}
			public string Name {
				get { return fName; }
				set {
					SetName(value);
				}
			}
			void SetName(string value) {
				if(value == null)
					throw new ArgumentException();
				if(fName != value) {
					bool differentComponents = true;
					if(value.Length > 0) {
						IComponent component = container.Components[value];
						differentComponents = this.fComponent != component;
						if(component != null && differentComponents) {
							throw new Exception("Duplicate name " + value);
						}
					}
					if(differentComponents) {
						INameCreationService service = (INameCreationService)((IServiceProvider)this).GetService(typeof(INameCreationService));
						if(service != null) {
							service.ValidateName(value);
						}
					}
					fName = value;
					PropertyInfo propInfo = fComponent.GetType().GetProperty("Name");
					if(propInfo != null)
						propInfo.SetValue(fComponent, value, null);
				}
			}
			public object GetService(Type service) {
				if(service == null) 
					throw new ArgumentNullException("service");
				if(service.Equals(typeof(IDictionaryService)))
					return (IDictionaryService)this;
				if(service != typeof(ISite)) 
					return this.container.GetService(service);
				return this;
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				while(Components.Count > 0)
					Remove(Components[0]);
			}
			base.Dispose(disposing);
		}
		protected override ISite CreateSite(IComponent component, string name) {
			return new ComponentSite(component, this, name);
		}
	}
}
