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
using DevExpress.Persistent.Base;
using System.Drawing;
using DevExpress.ExpressApp.Xpo;
using System.ComponentModel;
using System.Collections.Generic;
using DevExpress.Xpo.Metadata;
using DevExpress.ExpressApp.Updating;
namespace DevExpress.ExpressApp.MiddleTier {
	[ToolboxItem(false)]
	[ToolboxBitmap(typeof(ServerApplication), "Resources.Toolbox_ServerApplication.ico")]
	public class ServerApplication : XafApplication {
		protected override DevExpress.ExpressApp.Layout.LayoutManager CreateLayoutManagerCore(bool simple) {
			throw new NotImplementedException();
		}
		public ServerApplication() {
			Modules.Add(new ServerUpdateDatabaseModule());
		}
	}
	[ToolboxItem(false)]
	public class ServerUpdateDatabaseModule : ModuleBase {
		public class ServerApplicationUpdater : ModuleUpdater {
			public ServerApplicationUpdater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion) { }
			public override void UpdateDatabaseAfterUpdateSchema() {
				List<XPClassInfo> list = new List<XPClassInfo>();
				foreach (XPClassInfo ci in ((XPObjectSpace)this.ObjectSpace).Session.Dictionary.Classes) {
					if (ci.IsPersistent) {
						list.Add(ci);
					}
				}
				((XPObjectSpace)this.ObjectSpace).Session.CreateObjectTypeRecords(list.ToArray());
			}
		}
		public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
			return new ModuleUpdater[] { new ServerApplicationUpdater(objectSpace, versionFromDB) };
		}
		protected override IEnumerable<Type> GetRegularTypes() {
			return null;
		}
		protected override IEnumerable<Type> GetDeclaredExportedTypes() {
			return Type.EmptyTypes;
		}
		protected override IEnumerable<Type> GetDeclaredControllerTypes() {
			return Type.EmptyTypes;
		}
	}
}
