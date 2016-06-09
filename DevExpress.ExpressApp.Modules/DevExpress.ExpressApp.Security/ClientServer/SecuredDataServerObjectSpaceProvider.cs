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
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.DC.Xpo;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Xpo;
using DevExpress.ExpressApp.Xpo.Updating;
using DevExpress.Xpo;
using System.ComponentModel;
namespace DevExpress.ExpressApp.Security.ClientServer {
	public class DataServerObjectSpaceProvider : IObjectSpaceProvider { 
		private IClientInfoProvider currentClientInfoProvider;
		private ISecuredSerializableObjectLayer securedObjectLayer;
		private SchemaUpdateMode schemaUpdateMode = SchemaUpdateMode.DatabaseAndSchema;
		private CheckCompatibilityType? checkCompatibilityType = null;
		private void AsyncServerModeSourceResolveSession(ResolveSessionEventArgs args) {
			throw new NotImplementedException();
		}
		private void AsyncServerModeSourceDismissSession(ResolveSessionEventArgs args) {
			IDisposable toDispose = args.Session as IDisposable;
			if(toDispose != null) {
				toDispose.Dispose();
			}
		}
		public DataServerObjectSpaceProvider(ISecuredSerializableObjectLayer securedObjectLayer, IClientInfoProvider currentClientInfoProvider) {
			Guard.ArgumentNotNull(securedObjectLayer, "securedObjectLayer");
			Guard.ArgumentNotNull(currentClientInfoProvider, "currentClientInfoProvider");
			XpoTypesInfoHelper.ForceInitialize();
			this.securedObjectLayer = securedObjectLayer;
			this.currentClientInfoProvider = currentClientInfoProvider;
		}
		private UnitOfWork CreateUnitOfWork() {
			SecuredSerializableObjectLayerClient securedObjectLayerClient = new SecuredSerializableObjectLayerClient(currentClientInfoProvider.CreateClientInfo(), securedObjectLayer);
			XpoTypeInfoSource xpoTypeInfoSource = (XpoTypeInfoSource)EntityStore;
			SerializableObjectLayerClient objectLayer = new SerializableObjectLayerClient(securedObjectLayerClient, xpoTypeInfoSource.XPDictionary);
			return new UnitOfWork(objectLayer);
		}
		#region IObjectSpaceProvider Members
		public IObjectSpace CreateObjectSpace() {
			XPObjectSpace objectSpace = new XPObjectSpace(TypesInfo, (XpoTypeInfoSource)EntityStore, CreateUnitOfWork);
			objectSpace.AsyncServerModeSourceResolveSession = AsyncServerModeSourceResolveSession;
			objectSpace.AsyncServerModeSourceDismissSession = AsyncServerModeSourceDismissSession;
			return objectSpace;
		}
		public IObjectSpace CreateUpdatingObjectSpace(bool allowUpdateSchema) {
			if(allowUpdateSchema) {
				throw new NotSupportedException();
			}
			return CreateObjectSpace();
		}
		public void UpdateSchema() {
			throw new NotSupportedException();
		}
		public ITypesInfo TypesInfo {
			get { return XafTypesInfo.Instance; }
		}
		public IEntityStore EntityStore {
			get { return ((TypesInfo)XafTypesInfo.Instance).FindEntityStore(typeof(XpoTypeInfoSource)); }
		}
		public string ConnectionString {
			get { return ""; }
			set { }
		}
		public Type ModuleInfoType {
			get { return typeof(ModuleInfo); }
		}
		public SchemaUpdateMode SchemaUpdateMode {
			get { return schemaUpdateMode; }
			set { schemaUpdateMode = value; } 
		}
		public CheckCompatibilityType? CheckCompatibilityType {
			get { return checkCompatibilityType; }
			set { checkCompatibilityType = value; }
		}
		#endregion
	}
}
