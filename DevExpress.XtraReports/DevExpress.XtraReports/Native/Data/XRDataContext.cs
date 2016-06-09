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
using System.Text;
using System.Data;
using System.Collections;
using DevExpress.Data.Browsing;
using DevExpress.Data;
using System.ComponentModel;
using DevExpress.Data.Native;
using DevExpress.Data.Browsing.Design;
using System.Data.Common;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Native.Data {
	public class XRDataContext : XRDataContextBase {
		#region static
		public static string ValidateDataMember(object dataSource, string dataMember) {
			if(dataSource is DevExpress.Data.IDisplayNameProvider)
				return dataMember;
			using(DataContext dataContext = new DataContext(true)) {
				ListBrowser listBrowser = dataContext.GetDataBrowser(dataSource, dataMember, true) as ListBrowser;
				if(listBrowser == null)
					return string.IsNullOrEmpty(dataMember) ? string.Empty : null;
				else
					return dataMember;
			}
		}
		public static ObjectName CreateObjectName(object dataSource, string dataMember, string name) {
			if(dataMember == null)
				dataMember = String.Empty;
			using(DataContext dataContext = new DataContext(true)) {
				string displayName = dataContext.GetDataMemberDisplayName(dataSource, dataMember, BindingHelper.JoinStrings(".", dataMember, name));
				return string.IsNullOrEmpty(displayName) ? null : new ObjectName(name, displayName, dataMember);
			}
		}
		static bool BrowserHasRelation(DataBrowser browser, DataBrowser parent) {
			while(browser != null && parent != null) {
				if(Object.Equals(browser.Parent, parent))
					return true;
				browser = browser.Parent;
			}
			return false;
		}
		static object ValidateDataSource(object dataSource, ref string dataMember) {
			string[] items = dataMember.Split(new char[] { '.' }, 2);
			if(dataSource is DataView) {
				DataTable viewDataTable = ((DataView)dataSource).Table;
				if(viewDataTable != null && Object.Equals(viewDataTable.TableName, items[0]))
					dataMember = items.Length > 1 ? items[1] : "";
				return dataSource;
			}
			if(!(dataSource is DataSet))
				return dataSource;
			DataSet dataSet = (DataSet)dataSource;
			if(dataSet.Tables.Count == 0)
				return null;
			DataTable dataTable = dataSet.Tables[items[0]];
			if(dataTable != null) {
				dataMember = items.Length > 1 ? items[1] : "";
				return dataTable;
			}
			return dataSet.Tables[0];
		}
		#endregion
		EmptyListBrowser emptyListBrowser = new EmptyListBrowser();
		Dictionary<HashObj, DataPair> dataPairHT = new Dictionary<HashObj, DataPair>();
		public XRDataContext() {
		}
		public XRDataContext(IEnumerable<ICalculatedField> calculatedFields)
			: this(calculatedFields, false) {
		}
		public XRDataContext(IEnumerable<ICalculatedField> calculatedFields, bool suppressListFilling)
			: base(calculatedFields, suppressListFilling) {
		}
		public override void Clear() {
			base.Clear();
			dataPairHT.Clear();
		}
		public bool DataEqual(object dataSource1, string dataMember1, object dataSource2, string dataMember2) {
			DataBrowser dataBrowser1 = GetDataBrowser(dataSource1, dataMember1, true);
			DataBrowser dataBrowser2 = GetDataBrowser(dataSource2, dataMember2, true);
			return Object.Equals(dataBrowser1, dataBrowser2);
		}
		public bool DataHaveRelation(object dataSource, string dataMember, object relatedDataSource, string relatedDataMember) {
			try {
				return BrowserHasRelation(this[relatedDataSource, relatedDataMember], this[dataSource, dataMember]);
			} catch {
				return false;
			}
		}
		protected override DataBrowser GetDataBrowserInternal(object dataSource, string dataMember, bool suppressException) {
			if(dataSource == null || dataMember == null)
				return emptyListBrowser;
			DataPair dataPair;
			HashObj hashObj = new HashObj(dataSource, dataMember);
			if(!dataPairHT.TryGetValue(hashObj, out dataPair)) {
				object validDataSource = ValidateDataSource(dataSource, ref dataMember);
				string validDataMember = ValidateDataMember(validDataSource as DataTable, dataMember);
				dataPair = new DataPair(validDataSource, validDataMember);
#if DEBUGTEST
				DataPair dataPair2;
				if(dataPairHT.TryGetValue(hashObj, out dataPair2)) {
					System.Diagnostics.Debug.Assert(ReferenceEquals(dataPair.Source, dataPair2.Source));
					System.Diagnostics.Debug.Assert(dataPair.Member == dataPair2.Member);
				}
#endif
				dataPairHT[hashObj] = dataPair;
			}
			return dataPair.IsEmpty ? emptyListBrowser : base.GetDataBrowserInternal(dataPair.Source, dataPair.Member, suppressException);
		}
		string ValidateDataMember(DataTable dataTable, string dataMember) {
			if(dataTable == null || dataMember.Length == 0)
				return dataMember;
			if(base.GetDataBrowserInternal(dataTable, dataMember, true) != null)
				return dataMember;
			string[] names = dataMember.Split(new char[] { '.' }, 2);
			if(names.Length > 1 && base.GetDataBrowserInternal(dataTable, names[1], true) != null)
				return names[1];
			return "";
		}
	}
}
