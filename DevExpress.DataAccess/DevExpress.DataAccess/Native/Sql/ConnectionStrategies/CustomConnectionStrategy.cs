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
namespace DevExpress.DataAccess.Native.Sql.ConnectionStrategies {
	public class CustomConnectionStrategy : IConnectionParametersStrategy {
		#region Implementation of IConnectionParametersStrategy
		public ConnectionParameterEdits GetEditsSet(IConnectionParametersControl control) { return ConnectionParameterEdits.CustomString; }
		public ConnectionParameterEdits GetDisabledEdits(IConnectionParametersControl control) { return ConnectionParameterEdits.None; }
		public ConnectionParameterEdits Subscriptions { get { return ConnectionParameterEdits.None; } }
		public string FileNameFilter { get { return null; } }
		public bool CanGetDatabases { get { return false; } }
		public string GetDefaultText(ConnectionParameterEdits edit) {
			return edit == ConnectionParameterEdits.CustomString ? string.Empty : null;
		}
		public int GetDefaultIndex(ConnectionParameterEdits edit) { return -1; }
		public DataConnectionParametersBase GetConnectionParameters(IConnectionParametersControl control) { return new CustomStringConnectionParameters(control.CustomString); }
		public string GetConnectionName(IConnectionParametersControl control) { return "Connection"; }
		public IEnumerable<string> GetDatabases(IConnectionParametersControl control) { yield break; }
		public void InitializeControl(IConnectionParametersControl control, DataConnectionParametersBase value) {
			CustomStringConnectionParameters p = (CustomStringConnectionParameters)value;
			control.CustomString = p.ConnectionString;
		}
		#endregion
	}
}
