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
using System.Windows.Media;
using DevExpress.Mvvm;
using DevExpress.XtraReports.UI;
using DevExpress.Data.Browsing.Design;
using DevExpress.Xpf.Reports.UserDesigner.Native.ReportExtensions;
using DevExpress.Xpf.Reports.UserDesigner.ReportModel.Native;
using DevExpress.Utils;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Reports.UserDesigner.FieldList {
	public abstract class FieldListNodeBase<TOwner> : BindableBase {
		public abstract TOwner Owner { get; }
		public abstract IEnumerable<FieldListNodeBase<TOwner>> Children { get; }
		public abstract string DataMember { get; }
		public abstract string DisplayName { get; }
		public abstract object DataSource { get; }
		public abstract bool IsDataSourceNode { get; }
		public abstract ImageSource Icon { get; set; }
		public abstract TypeSpecifics TypeSpecifics { get; }
		public BindingData BindingData { get { return DataSource == null ? null : new BindingData(DataSource, DataMember); } }
		public abstract FieldListNodeBase<TOwner> Parent { get; }
		public abstract string FullPath { get; }
	}
}
