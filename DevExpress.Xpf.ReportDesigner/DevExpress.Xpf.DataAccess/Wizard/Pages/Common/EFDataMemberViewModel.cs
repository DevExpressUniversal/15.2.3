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

using DevExpress.Mvvm.Native;
using DevExpress.Xpo.DB;
using DevExpress.XtraPrinting.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Xpf.DataAccess.DataSourceWizard {
	public abstract class EFDataMemberViewModelBase {
		public enum EFDataMemberType {
			Table,
			View,
			StoredProcedure
		}
		public abstract string DataMember { get; }
		public abstract EFDataMemberType DataMemberType { get; }
		public string Name {
			get { return string.IsNullOrEmpty(DataMember) ? PreviewLocalizer.GetString(PreviewStringId.NoneString) : DataMember; }
		}
	}
	public class EFDBTableViewModel : EFDataMemberViewModelBase {
		readonly DBTable table;
		public EFDBTableViewModel(DBTable table) {
			GuardHelper.ArgumentNotNull(table, "table");
			this.table = table;
		}
		public override EFDataMemberType DataMemberType {
			get { return table.IsView ? EFDataMemberType.View : EFDataMemberType.Table; }
		}
		public override string DataMember {
			get { return table.Name; }
		}
	}
	public class EFDBStoredProcedureViewModel : EFDataMemberViewModelBase {
		readonly DBStoredProcedure procedure;
		public EFDBStoredProcedureViewModel(DBStoredProcedure procedure) {
			GuardHelper.ArgumentNotNull(procedure, "procedure");
			this.procedure = procedure;
		}
		public override EFDataMemberType DataMemberType {
			get { return EFDataMemberType.StoredProcedure; }
		}
		public override string DataMember {
			get { return procedure.Name; }
		}
	}
}
