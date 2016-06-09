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
using DevExpress.Data;
using DevExpress.DataAccess.EntityFramework;
using DevExpress.DataAccess.Sql;
namespace DevExpress.DataAccess.Wizard.Native {
	public abstract class StoredProcInfo<T> where T : IParameter {
		protected StoredProcInfo(string name, List<T> parameters) {
			Name = name;
			Parameters = parameters;
		}
		public string Name { get; private set; }
		public List<T> Parameters { get; private set; }
		public abstract T CreateParameter(string name, Type type, object value);
	}
	class SqlStoredProcInfo : StoredProcInfo<QueryParameter> {
		public SqlStoredProcInfo(string name, List<QueryParameter> parameter)
			: base(name, parameter) {
		}
		public override QueryParameter CreateParameter(string name, Type type, object value) {
			return new QueryParameter { Name = name, Type = type, Value = value };
		}
	}
	class EFStoredProcInfo : StoredProcInfo<EFParameter> {
		public EFStoredProcInfo(string name, List<EFParameter> parameters)
			: base(name, parameters) {
		}
		public override EFParameter CreateParameter(string name, Type type, object value) {
			return new EFParameter { Name = name, Type = type, Value = value };
		}
	}
}
