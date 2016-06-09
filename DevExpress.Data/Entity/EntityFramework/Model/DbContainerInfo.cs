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
using DevExpress.Entity.ProjectModel;
using DevExpress.Entity.Model.Metadata;
namespace DevExpress.Entity.Model {
	public class DbContainerInfo : DXTypeInfo, IDbContainerInfo {		
		IEntityContainerInfo container;
		public DbContainerInfo(Type type, MetadataWorkspaceInfo metadataWorkspace = null)
			: base(type) {
			if(metadataWorkspace != null) {
				MetadataWorkspace = metadataWorkspace.Value;
			}
		}
		public DbContainerInfo(Type type, IEntityContainerInfo container, MetadataWorkspaceInfo metadataWorkspace = null)
			: this(type, metadataWorkspace) {
			this.container = container;			
		}
		public IEntityContainerInfo EntityContainer {
			get { return container; }
			private set { container = value; }
		}		
		public IEnumerable<IEntitySetInfo> EntitySets {
			get {
				if(EntityContainer == null || EntityContainer.EntitySets == null)
					yield break;
				foreach(IEntitySetInfo info in EntityContainer.EntitySets)
					if(IsValidEntitySet(info))
						yield return info;
			}
		}
		protected virtual bool IsValidEntitySet(IEntitySetInfo info) {
			return info != null;
		}
		public DbContainerType ContainerType {
			get;
			set;
		}
		public string SourceUrl {
			get;
			set;
		}
		public IEnumerable<IEntityFunctionInfo> EntityFunctions {
			get {
				if(EntityContainer == null || EntityContainer.EntityFunctions == null)
					yield break;
				foreach(IEntityFunctionInfo info in EntityContainer.EntityFunctions)
					yield return info;
			}
		}
		public object MetadataWorkspace { get; private set; }
	}
}
