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
using System.Linq;
using DevExpress.DataAccess.EntityFramework;
namespace DevExpress.DataAccess.Wizard.Views {
	public interface IConfigureEFStoredProceduresPageView {
		StoredProcedureViewInfo SelectedItem { get; }
		void Initialize(IEnumerable<StoredProcedureViewInfo> procedures, Func<object> getPreviewDataFunc);
		void AddToList(IEnumerable<StoredProcedureViewInfo> procedures);
		void RemoveFromList(StoredProcedureViewInfo procedure);
		IEnumerable<StoredProcedureViewInfo> ChooseProceduresToAdd(IEnumerable<StoredProcedureViewInfo> available);
		void SetAddEnabled(bool value);
		event EventHandler AddClick;
		event EventHandler RemoveClick;
	}
	public sealed class StoredProcedureViewInfo {
		readonly EFStoredProcedureInfo storedProcedure;
		string displayName;
		public EFStoredProcedureInfo StoredProcedure {
			get { return storedProcedure; }
		}
		public bool Checked { get; set; }
		string DisplayName {
			get {
				return storedProcedure == null
					? null
					: displayName ??
					  (displayName =
						  string.Format("{0} ({1})", storedProcedure.Name,
							  string.Join(", ", storedProcedure.Parameters.Select(p => p.Name))));
			}
		}
		public StoredProcedureViewInfo(string procedureName) {
			storedProcedure = new EFStoredProcedureInfo(procedureName, null);
		}
		public StoredProcedureViewInfo(EFStoredProcedureInfo procedure) {
			storedProcedure = procedure;
		}
		public StoredProcedureViewInfo(EFStoredProcedureInfo procedure, bool isChecked) : this(procedure) {
			Checked = isChecked;
		}
		public static implicit operator EFStoredProcedureInfo(StoredProcedureViewInfo value) {
			return value != null ? value.StoredProcedure : null;
		}
		public override string ToString() {
			return storedProcedure != null ? DisplayName : base.ToString();
		}
	}
}
