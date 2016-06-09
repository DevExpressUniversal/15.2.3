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

using DevExpress.Data.Browsing;
using DevExpress.Snap.Core.Native.Data;
using DevExpress.Snap.Core.Native.Services;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Native.Data;
namespace DevExpress.Snap.Core.Native {
	public class DataContextService : DataContextServiceBase {
		readonly SnapDocumentModel documentModel;
		readonly IDataSourceDisplayNameProvider dataSourceDisplayNameProvider;
		public DataContextService(SnapDocumentModel documentModel) {
			this.documentModel = documentModel;
			this.documentModel.DataSourceDispatcher.DataSourceChanged += OnDataSourceChanged;
			this.documentModel.DataSourceDispatcher.CollectionChanged += OnDataSourceChanged;
			this.dataSourceDisplayNameProvider = this.documentModel.GetService<IDataSourceDisplayNameProvider>();
		}
		void OnDataSourceChanged(object sender, System.EventArgs e) {
			DisposeDataContext();
		}
		protected override System.Collections.Generic.IEnumerable<ICalculatedField> CalculatedFields {
			get {
				return this.documentModel.DataSourceDispatcher.GetCalculatedFields();
			}
		}
		protected override DataContext CreateDataContextInternal(DataContextOptions options) {
			return options.UseCalculatedFields ? new SnapDataContext(CalculatedFields, documentModel.Parameters, dataSourceDisplayNameProvider) : new SnapDataContext(dataSourceDisplayNameProvider);
		}
		public override void Dispose() {
			this.documentModel.DataSourceDispatcher.DataSourceChanged -= OnDataSourceChanged;
			this.documentModel.DataSourceDispatcher.CollectionChanged -= OnDataSourceChanged;
			base.Dispose();
		}
	}
}
