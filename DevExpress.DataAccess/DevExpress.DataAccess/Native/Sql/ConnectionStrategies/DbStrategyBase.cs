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
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.Xpo.DB;
namespace DevExpress.DataAccess.Native.Sql.ConnectionStrategies {
	public abstract class DbStrategyBase : IConnectionParametersStrategy {
		public abstract ConnectionParameterEdits GetEditsSet(IConnectionParametersControl control);
		public virtual ConnectionParameterEdits GetDisabledEdits(IConnectionParametersControl control) { return ConnectionParameterEdits.None; }
		public virtual ConnectionParameterEdits Subscriptions { get { return ConnectionParameterEdits.None; } }
		public virtual string FileNameFilter { get { return Factory.FileFilter; } }
		public virtual bool CanGetDatabases { get { return false; } }
		protected abstract ProviderFactory Factory { get; }
		public abstract string GetDefaultText(ConnectionParameterEdits edit);
		public virtual int GetDefaultIndex(ConnectionParameterEdits edit) { return -1; }
		public abstract DataConnectionParametersBase GetConnectionParameters(IConnectionParametersControl control);
		public abstract string GetConnectionName(IConnectionParametersControl control);
		public virtual IEnumerable<string> GetDatabases(IConnectionParametersControl control) { yield break; }
		public abstract void InitializeControl(IConnectionParametersControl control, DataConnectionParametersBase value);
	}
}
