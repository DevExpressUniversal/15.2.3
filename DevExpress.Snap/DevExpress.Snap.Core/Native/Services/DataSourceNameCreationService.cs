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
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Snap.Core.API;
namespace DevExpress.Snap.Core.Native.Services {
	public class DataSourceNameCreationService : IDataSourceNameCreationService {
		const string prefix = "Data Source ";
		readonly SnapDocumentModel documentModel;
		public DataSourceNameCreationService(SnapDocumentModel documentModel) {
			this.documentModel = documentModel;
		}
		public string CreateName() {
			List<string> names = new List<string>();
			foreach (DataSourceInfo info in this.documentModel.DataSourceDispatcher.GetInfos())
				if (!this.documentModel.DataSourceDispatcher.IsDefaultDataSource(info.DataSource))
					names.Add(info.DataSourceName);
			int index = 1;
			string name;
			do {
				name = string.Format("{0}{1}", prefix, index);
				index++;
			} while (names.Contains(name));
			return name;
		}
		public bool ValidateName(string name) {
			foreach (DataSourceInfo info in this.documentModel.DataSourceDispatcher.GetInfos())
				if (!this.documentModel.DataSourceDispatcher.IsDefaultDataSource(info.DataSource) && string.Equals(info.DataSourceName, name))
					return false;
			return true;
		}
	}
}
