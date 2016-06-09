#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Linq;
using System.Text;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraGrid.Views.Base;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor.InternalDesigner {
	internal class DesignerDataSourceProvider {
		private List<FakeObject> fakeObjects;
		private IServiceProvider serviceProvider;
		public DesignerDataSourceProvider(IServiceProvider serviceProvider) {
			this.serviceProvider = serviceProvider;
		}
		public IList CreateDataSource(IModelListView model, ColumnView columnView) {
			return CreateDataSource(model, columnView, FakeObjects);
		}
		public IList CreateDataSource(IModelListView model, ColumnView columnView, List<FakeObject> dataSourceObjects) {
			IList dataSource = null;
			Type objectType = model.ModelClass.TypeInfo.Type;
			if(objectType != null && serviceProvider != null) {
				if(model.DataAccessMode == CollectionSourceDataAccessMode.Server) {
					dataSource = new ServerDataSourceProvider(objectType.FullName, "DataSource", serviceProvider, true, columnView, dataSourceObjects);
				}
				else {
					dataSource = new ClientDataSourceProvider(objectType.FullName, "DataSource", serviceProvider, true, dataSourceObjects);
				}
			}
			return dataSource;
		}
		private List<FakeObject> FakeObjects {
			get {
				if(fakeObjects == null) {
					fakeObjects = new List<FakeObject>();
					fakeObjects.Add(new FakeObject(0));
					fakeObjects.Add(new FakeObject(1));
					fakeObjects.Add(new FakeObject(1));
					fakeObjects.Add(new FakeObject(2));
					fakeObjects.Add(new FakeObject(2));
				}
				return fakeObjects;
			}
		}
	}
}
