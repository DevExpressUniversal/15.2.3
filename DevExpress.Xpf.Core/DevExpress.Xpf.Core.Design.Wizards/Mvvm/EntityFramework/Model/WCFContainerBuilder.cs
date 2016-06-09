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
using System.IO;
using System.Xml;
using System.Linq;
using System.Xml.Linq;
using System.Data.Metadata.Edm;
using System.Collections.Generic;
using DevExpress.Design.Mvvm;
using DevExpress.Design.Mvvm.Wizards;
using DevExpress.Mvvm.UI.Native.ViewGenerator;
using DevExpress.Xpf.Core.Mvvm.UI.ViewGenerator.Metadata;
using DevExpress.Entity.Model.Metadata;
using DevExpress.Entity.Model;
using DevExpress.Entity.ProjectModel;
namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm.EntityFramework {
	public class WCFContainerBuilder : ContainerBuilder {
		readonly IWcfEdmInfoProvider provider;
		public WCFContainerBuilder(IWcfEdmInfoProvider provider) {
			this.provider = provider;
		}
		public override DbContainerType BuilderType {
			get {
				return DbContainerType.WCF;
			}
		}
		protected override IDataColumnAttributesProvider CreateDataColumnAttributesProvider() {
			return new DataColumnAttributesProvider();
		}
		void ClearFromFunctionImport(XElement element) {
			XNode functionImport = GetXNodeRecursively(element.Nodes(), node => {
				XElement xElement = node as XElement;
				return xElement != null && xElement.Name.LocalName == "FunctionImport";
			});
			if (functionImport != null)
				functionImport.Remove();
		}
		IEnumerable<XmlReader> ExtractCsdlContent(Stream stream) {
			XNamespace edmxns = "http://schemas.microsoft.com/ado/2007/06/edmx";
			XDocument edmxDoc = XDocument.Load(stream);
			if (edmxDoc == null)
				return null;
			XElement edmxNode = edmxDoc.Element(edmxns + "Edmx");
			if (edmxNode == null)
				return null;
			XElement dataServices = edmxNode.Element(edmxns + "DataServices");
			if (dataServices == null)
				return null;
			List<XmlReader> result = new List<XmlReader>();
			IEnumerable<XElement> elements = dataServices.Elements();
			foreach (XElement element in elements) {
				ClearFromFunctionImport(element);
				XmlReader reader = element.CreateReader();
				result.Add(reader);
			}
			return result;
		}
		XNode GetXNodeRecursively(IEnumerable<XNode> nodes, Predicate<XNode> predicate) {
			if (nodes == null || predicate == null)
				return null;
			foreach (XNode item in nodes) {
				if (predicate(item))
					return item;
				XElement element = item as XElement;
				if (element != null) {
					XNode result = GetXNodeRecursively(element.Nodes(), predicate);
					if (result != null)
						return result;
				}
			}
			return null;
		}
		protected override DbContainerInfo GetDbContainerInfo(IDXTypeInfo type, MetadataWorkspaceInfo mw, IMapper mapper) {
			DbContainerInfo result = base.GetDbContainerInfo(type, mw, mapper);
			if (result != null) {
				result.ContainerType = DbContainerType.WCF;
				result.SourceUrl = provider.GetSourceUrl(type.ResolveType());
			}
			return result;
		}
		public override IDbContainerInfo Build(IDXTypeInfo type, ISolutionTypesProvider typesProvider) {
			try {
				MetadataWorkspace mw = new MetadataWorkspace();
				Type clrType = type.ResolveType();
				EdmItemCollection ec = new EdmItemCollection(ExtractCsdlContent(provider.GetEdmItemCollectionStream(clrType)));
				if (ec == null)
					return null;
				mw.RegisterItemCollection(ec);
				return GetDbContainerInfo(type, new MetadataWorkspaceInfo(mw), new WcfMapper(type));
			}
			catch (Exception ex) {
				Log.SendException(ex);
				return null;
			}
		}
		protected override DbContainerInfo CreateDbContainerInfo(IDXTypeInfo type, EntityContainerInfo result, MetadataWorkspaceInfo mw) {
			return new ScaffoldDbContainerInfo(type.ResolveType(), result, mw) { Assembly = type.Assembly };
		}
	}
}
