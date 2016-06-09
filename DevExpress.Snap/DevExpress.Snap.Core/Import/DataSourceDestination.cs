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
using System.Xml;
using DevExpress.Office;
using DevExpress.XtraRichEdit.Import.OpenXml;
using DevExpress.DataAccess.Native;
using System.Xml.Linq;
using DevExpress.DataAccess;
using DevExpress.DataAccess.Sql;
using DevExpress.Snap.Native.Services;
using DevExpress.DataAccess.Native.Sql;
using System.ComponentModel.Design;
namespace DevExpress.Snap.Core.Import {
	#region DataSourcesDestination
	public class DataSourcesDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("dataSource", OnDataSource);
			result.Add(DataComponentDestination.RootElementName, OnDataComponent);
			return result;
		}
		static Destination OnDataSource(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DataSourceDestination((SnapImporter)importer);
		}
		static Destination OnDataComponent(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DataComponentDestination((SnapImporter)importer);
		}
		public DataSourcesDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
	#region DataSourceDestination
	public class DataSourceDestination : SnapLeafElementDestination {
		public DataSourceDestination(SnapImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string name = Importer.ReadDxStringAttr("name", reader);
			if (name != null) {
				string data = Importer.ReadDxStringAttr("data", reader);
				if (!String.IsNullOrEmpty(data)) {
					try {
						byte[] bytes = Convert.FromBase64String(data);
						AfterDataSourceImportEventArgs args = new AfterDataSourceImportEventArgs(name, bytes);
						Importer.DocumentModel.RaiseAfterDataSourceImport(args);
					}
					catch {
					}
				}
			}
		}
	}
	#endregion
	#region DataComponentDestination
	public class DataComponentDestination : SnapLeafElementDestination {
		public const string RootElementName = "dataComponent";
		public DataComponentDestination(SnapImporter importer)
			: base(importer) { }
		public override void ProcessElementOpen(XmlReader reader) {
			try {
				XElement element = (XElement)XNode.ReadFrom(reader);
				DataComponentHelper helper = new DataComponentHelper(RootElementName);
				IDataComponent dataComponent = helper.LoadFromXml(element);
				IServiceContainer container = dataComponent as IServiceContainer;
				if (container != null) {
					CompositeDataConnectionParametersService srv = new CompositeDataConnectionParametersService();
					IDataConnectionParametersService oldService = container.GetService(typeof(IDataConnectionParametersService)) as IDataConnectionParametersService;
					if (oldService != null) {
						srv.AddService(oldService);
						container.RemoveService(typeof(IDataConnectionParametersService));
					}
					srv.AddService(DocumentModel.GetService<IDataConnectionParametersService>());
					container.AddService(typeof(IDataConnectionParametersService), srv);
				}
				dataComponent.Fill(Importer.DocumentModel.Parameters);
				Importer.DocumentModel.AddDataSource(dataComponent);
			}
			catch {
			}
		}
	}
	#endregion
}
