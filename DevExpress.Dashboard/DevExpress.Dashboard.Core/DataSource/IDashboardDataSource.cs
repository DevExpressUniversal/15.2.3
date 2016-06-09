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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.Data;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Data;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.DashboardCommon.DataProcessing;
namespace DevExpress.DashboardCommon {
	public interface IDashboardDataSource : IDashboardComponent, ISupportPrefix {
		[Browsable(false)]
		CalculatedFieldCollection CalculatedFields { get; }
		[Browsable(false)]
		string Filter { get; set; }
		[Browsable(false)]
		object Data { get; set; }
		[Browsable(false)]
		bool HasDataProvider { get; }
		[Browsable(false)]
		bool IsServerModeSupported { get; }
		[Browsable(false)]
		DataProcessingMode DataProcessingMode { get; set; }
#if !DXPORTABLE
		[Browsable(false)]
		IDataProvider DataProvider { get; set; }
#pragma warning disable 612, 618
		[Browsable(false)]
		SqlDataProvider SqlDataProvider { get; }
		[Browsable(false)]
		OlapDataProvider OlapDataProvider { get; }
#pragma warning restore 612, 618
#endif
		[Browsable(false)]
		IEnumerable<IParameter> Parameters { get; }
		[Browsable(false)]
		bool IsConnected { get; }
		IDashboardDataSourceInternal GetDataSourceInternal();
		ICalculatedFieldsController GetCalculatedFieldsController();
		IDataSourceSchema GetDataSourceSchema(string dataMember);
	}
}
