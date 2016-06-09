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
using System.ComponentModel;
using System.ServiceModel;
using DevExpress.ExpressApp.DC;
namespace DevExpress.ExpressApp.Workflow {
	#region Obsolete 13.1
	[Obsolete(ObsoleteMessages.TypeIsNotUsedAnymore, true), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class SerializableObjectLayerObjectSpaceProviderBase : IObjectSpaceProvider {
		private SchemaUpdateMode schemaUpdateMode = SchemaUpdateMode.DatabaseAndSchema;
		private CheckCompatibilityType? checkCompatibilityType = null;
		public IObjectSpace CreateObjectSpace() {
			return null;
		}
		public abstract string ConnectionString { get; set; }
		public abstract IObjectSpace CreateUpdatingObjectSpace(bool allowUpdateSchema);
		public void UpdateSchema() {
			throw new NotSupportedException();
		}
		public ITypesInfo TypesInfo { get; private set; }
		public IEntityStore EntityStore { get { return XafTypesInfo.PersistentEntityStore; } }
		public Type ModuleInfoType {
			get { return null; }
		}
		public SchemaUpdateMode SchemaUpdateMode {
			get { return schemaUpdateMode; }
			set { schemaUpdateMode = value; }
		}
		public CheckCompatibilityType? CheckCompatibilityType {
			get { return checkCompatibilityType; }
			set { checkCompatibilityType = value; }
		}
	}
	[Obsolete(ObsoleteMessages.TypeIsNotUsedAnymore, true), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class SerializableObjectLayerObjectSpaceProvider : SerializableObjectLayerObjectSpaceProviderBase {
		public override string ConnectionString {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
		public override IObjectSpace CreateUpdatingObjectSpace(bool allowUpdateSchema) {
			throw new NotImplementedException();
		}
	}
	[Obsolete(ObsoleteMessages.TypeIsNotUsedAnymore, true), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class RemotingSerializableObjectLayerObjectSpaceProvider : SerializableObjectLayerObjectSpaceProviderBase {
		private string connectionString;
		public override IObjectSpace CreateUpdatingObjectSpace(bool allowUpdateSchema) {
			throw new NotImplementedException();
		}
		public override string ConnectionString { get { return connectionString; } set { this.connectionString = value; } }
	}
	[Obsolete(ObsoleteMessages.TypeIsNotUsedAnymore, true), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class ServiceSerializableObjectLayerObjectSpaceProvider : SerializableObjectLayerObjectSpaceProviderBase {
		private string connectionString;
		public override IObjectSpace CreateUpdatingObjectSpace(bool allowUpdateSchema) {
			throw new NotImplementedException();
		}
		public override string ConnectionString { get { return connectionString; } set { this.connectionString = value; } }
	}
	#endregion
}
