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
using System.ComponentModel;
using DevExpress.Data;
using DevExpress.Data.Helpers;
using DevExpress.Data.Selection;
using DevExpress.Xpf.Core;
#if SL
using DevExpress.Data.Browsing;
using DevExpress.Xpf.Core.WPFCompatibility;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
#endif
namespace DevExpress.Xpf.Data {
	public class AsyncServerModeGridDataController : AsyncServerModeDataController {
		protected override SelectionController CreateSelectionController() { return new RowStateController(this); }
		protected override BaseDataControllerHelper CreateHelper() {
			return new AsyncGridDataControllerHelper(this);
		}
	}
	public class AsyncGridDataControllerHelper : AsyncListDataControllerHelper {
		public AsyncGridDataControllerHelper(AsyncServerModeDataController controller) : base(controller) { }
		protected override PropertyDescriptorCollection GetPropertyDescriptorCollection() {
			 PropertyDescriptorCollection collection = base.GetPropertyDescriptorCollection();
			 if(Controller.ListSource != null) {
			 }
			 return collection;
		}
	}
	internal class GridDataControllerThreadClient : IDataControllerThreadClient {
		GridDataProvider dataProvider;
#if DEBUGTEST
		internal int LoadedRowsCount { get; private set; }
#endif
		public GridDataControllerThreadClient(GridDataProvider dataProvider) {
			this.dataProvider = dataProvider;
		}
		#region IDataControllerThreadClient Members
		public void OnAsyncBegin() {
			dataProvider.IsAsyncOperationInProgress = true; 
		}
		public void OnAsyncEnd() {
			dataProvider.IsAsyncOperationInProgress = false;
		}
		public void OnRowLoaded(int controllerRowHandle) {
			IncrementLoadedRowsCount();
		}
		void IncrementLoadedRowsCount() {
#if DEBUGTEST
			LoadedRowsCount++;
			if(LoadedRowsCount == 1001) {
				LoadedRowsCount = 0;
			}
#endif
		}
		public void OnTotalsReceived() {
			dataProvider.OnAsyncTotalsReceived();
		}
		#endregion
	}
}
