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
using DevExpress.PivotGrid.OLAP;
using DevExpress.XtraPivotGrid;
namespace DevExpress.PivotGrid.Xmla {
	enum ResponseFormat { Tabular, Multidimensional, Native }
	interface IResponseFormatProvider {
		ResponseFormat ResponseFormat { get ; }
	}
	class XmlaCommand : IOLAPCommand, IResponseFormatProvider, IOLAPEntity {
		readonly XmlaConnection connection;
		ResponseFormat responseFormat;
		string commandText;
		int commandTimeout;
		public XmlaCommand(XmlaConnection connection, string mdx) {
			this.connection = connection;
			this.commandText = mdx;
			this.responseFormat = ResponseFormat.Multidimensional;
		}
		public string CommandText {
			get { return this.commandText; }
			set { this.commandText = value; }
		}
		public int CommandTimeout {
			get { return this.commandTimeout; }
			set { this.commandTimeout = value; }
		}
		public XmlaConnection Connection {
			get { return this.connection; }
		}
		public ResponseFormat ResponseFormat {
			get { return responseFormat; }
		}
		public IOLAPCellSet ExecuteCellSet(OLAPAreas areas) {
			this.responseFormat = ResponseFormat.Multidimensional;
			XmlaCommandExecutor сommandExecutor = XmlaCommandExecutor.Create(this);
			return сommandExecutor.ExecuteCellSet();
		}
		public IOLAPRowSet ExecuteRowSet() {
			this.responseFormat = ResponseFormat.Tabular;
			XmlaCommandExecutor сommandExecutor = XmlaCommandExecutor.Create(this);
			return сommandExecutor.ExecuteRowSet();
		}
		#region IOLAPCommand Members
		IOLAPConnection IOLAPCommand.Connection {
			get { return this.Connection; }
		}
		#endregion
		#region IDisposable Members
		void IDisposable.Dispose() {
		}
		#endregion
	}
}
