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

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
namespace DevExpress.DataAccess.EntityFramework {
	[TypeConverter("DevExpress.DataAccess.UI.Native.EntityFramework.EFStoredProcedureInfoCollectionTypeConverter," + AssemblyInfo.SRAssemblyDataAccessUI)]
	[Editor("DevExpress.DataAccess.UI.Native.EntityFramework.EFStoredProcedureInfoCollectionEditor, " + AssemblyInfo.SRAssemblyDataAccessUI, typeof(UITypeEditor))]
	public sealed class EFStoredProcedureInfoCollection : List<EFStoredProcedureInfo> {
		readonly EFDataSource dataSource;
		internal EFDataSource DataSource {
			get {
				return dataSource;
			}
		}
		internal EFStoredProcedureInfoCollection(EFDataSource dataSource) {
			this.dataSource = dataSource;
		}
		public new void AddRange(IEnumerable<EFStoredProcedureInfo> storedProcedureInfos) {
			foreach(EFStoredProcedureInfo efStoredProcedureInfo in storedProcedureInfos) {
				efStoredProcedureInfo.ParentCollection = this;
			}
			base.AddRange(storedProcedureInfos);
		}
		public void AddRange(EFStoredProcedureInfo[] storedProcedureInfos) {
			AddRange(storedProcedureInfos.AsEnumerable());
		}
		public new void Add(EFStoredProcedureInfo storedProcedure) {
			storedProcedure.ParentCollection = this;
			base.Add(storedProcedure);
		}
	}
}
