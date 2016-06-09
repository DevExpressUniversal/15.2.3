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
using System.Linq;
using DevExpress.Data;
using DevExpress.DataAccess.EntityFramework;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.Xpo.DB;
namespace DevExpress.DataAccess.Wizard.Native {
	public static class StoredProcParametersHelper {
		public static void SyncParams<T>(StoredProcInfo<T> storedProcInfo, DBSchema schema) where T: IParameter {
			DBStoredProcedure storedProcedure = schema.StoredProcedures.FirstOrDefault(s => s.Name == storedProcInfo.Name);
			if(storedProcedure == null)
				return;
			List<T> storedProcParameters = storedProcedure.Arguments.Where(a => a.Direction != DBStoredProcedureArgumentDirection.Out)
								.Select(p => storedProcInfo.CreateParameter(p.Name, DBColumn.GetType(p.Type), null))
								.ToList();
			for(int i = 0; i < storedProcInfo.Parameters.Count; i++) {
				string paramName = storedProcInfo.Parameters[i].Name;
				if(!storedProcParameters.Any(p => string.Equals(p.Name, paramName)))
					storedProcInfo.Parameters.RemoveAt(i--);
			}
			foreach(T param in storedProcParameters) {
				string paramName = param.Name;
				if(!storedProcInfo.Parameters.Any(p => string.Equals(p.Name, paramName)))
					storedProcInfo.Parameters.Add(param);
			}
		}
		public static List<StoredProcedureViewInfo> SyncProcedures(DBSchema schema, IEnumerable<EFStoredProcedureInfo> existingProcedures) {
			List<StoredProcedureViewInfo> procedures = new List<StoredProcedureViewInfo>();
			if(existingProcedures != null)
				procedures.AddRange(existingProcedures.Where(p => schema.StoredProcedures.Any(n => n.Name == p.Name)).Select(p => new StoredProcedureViewInfo(p, true)));
			procedures.AddRange(schema.StoredProcedures.Where(p => procedures.All(i => i.StoredProcedure.Name != p.Name)).Select(p => new StoredProcedureViewInfo(p.Name)));
			foreach(EFStoredProcedureInfo info in procedures)
				StoredProcParametersHelper.SyncParams(new EFStoredProcInfo(info.Name, info.Parameters), schema);
			return procedures;
		}
	}
}
