#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardCommon.Server {
	public interface IDashboardActivator {
		Dashboard CreateDashboard();
	}
	internal abstract class DashboardActivatorBase<T> : IDashboardActivator {
		readonly T source;
		protected T Source { get { return source; } }
		public DashboardActivatorBase(T source) {
			this.source = source;
		}
		public abstract Dashboard CreateDashboard();
	}
	internal class DashboardXmlActivator : DashboardActivatorBase<string> {
		public DashboardXmlActivator(string xml) : base(xml) { }
		public override Dashboard CreateDashboard() {
			using(MemoryStream stream = new MemoryStream()) {
				using(StreamWriter writer = new StreamWriter(stream)) {
					writer.Write(Source);
					writer.Flush();
					stream.Position = 0L;
					Dashboard dashboard = new Dashboard();
					dashboard.LoadFromXml(stream, null);
					return dashboard;
				}
			}
		}
	}
	internal class DashboardTypeActivator : DashboardActivatorBase<Type> {
		public DashboardTypeActivator(Type dashboardType) : base(dashboardType) { }
		public override Dashboard CreateDashboard() {
			return Activator.CreateInstance(Source) as Dashboard;
		}
	}
	internal class DashboardInstanceActivator : DashboardActivatorBase<Dashboard> {
		public DashboardInstanceActivator(Dashboard dashboard) : base(dashboard) { }
		public override Dashboard CreateDashboard() {
			return Source;
		}
	}
}
