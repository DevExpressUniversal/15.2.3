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
using System.Xml;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.Utils.Localization;
namespace DevExpress.ExpressApp.Localization {
	public class ControlResourcesLocalizerLogic {
		protected IXafResourceManagerParametersProvider xafResourceManagerParametersProvider;
		protected XafResourceManager resourceManager;
		private IModelApplication modelApplication;
		public ControlResourcesLocalizerLogic(IXafResourceManagerParametersProvider xafResourceManagerParametersProvider) {
			Guard.ArgumentNotNull(xafResourceManagerParametersProvider, "xafResourceManagerParametersProvider");
			this.xafResourceManagerParametersProvider = xafResourceManagerParametersProvider;
			if(resourceManager != null) {
				resourceManager.ReleaseAllResources();
			}
			resourceManager = new XafResourceManager(xafResourceManagerParametersProvider);
		}
		public void Setup(IModelApplication modelApplication) {
			this.modelApplication = modelApplication;
			xafResourceManagerParametersProvider.XafResourceManagerParameters.ModelApplication = modelApplication;
			Reset();
		}
		public void Reset() {
			Manager.ReloadModelData();
		}
		public XafResourceManager Manager {
			get { return resourceManager; }
		}
		public IModelApplication Model {
			get { return modelApplication; }
		}
	}
	public class ControlXmlResourcesLocalizerLogic<T> : ControlResourcesLocalizerLogic where T : struct {
		private bool readDefault;
		public ControlXmlResourcesLocalizerLogic(IXafResourceManagerParametersProvider xafResourceManagerParametersProvider) :
			base(xafResourceManagerParametersProvider) {
		}
		public XmlDocument GetXmlResources() {
			XtraLocalizer<T> xmlResourceLocalizer = (XtraLocalizer<T>)xafResourceManagerParametersProvider;
			readDefault = true;
			XmlDocument xmlDocument = xmlResourceLocalizer.CreateXmlDocument();
			readDefault = false;
			return xmlDocument;
		}
		public string GetResourceItemPrefix() {
			return xafResourceManagerParametersProvider.XafResourceManagerParameters.ResourceItemPrefix;
		}
		public string GetString(T id, string defaultString) {
			if(readDefault) {
				return defaultString;
			}
			return resourceManager.GetString(GetResourceItemPrefix() + id.ToString());
		}
	}
}
