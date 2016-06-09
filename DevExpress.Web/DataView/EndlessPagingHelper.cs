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

using System.Collections;
using System.Globalization;
using System.IO;
using System.Web.UI;
namespace DevExpress.Web.Internal {
	public class DataViewEndlessPagingHelper {
		class ClientTableLayoutSettigns: ClientLayoutSettigns {
			public override Layout Layout { get { return Layout.Table; } }
			public int ColumnCount { get; set; }
			public int RowsPerPage { get; set; }
			public override void LoadClientState(ASPxDataViewBase dataView) {
				ColumnCount = dataView.SettingsTableLayoutInternal.ColumnCount;
				RowsPerPage = dataView.SettingsTableLayoutInternal.RowsPerPage;
			}
			public override bool AreEqualsState(ASPxDataViewBase dataView) {
				var settings = dataView.SettingsTableLayoutInternal;
				return base.AreEqualsState(dataView) && settings.ColumnCount == ColumnCount &&
					settings.RowsPerPage == RowsPerPage;
			}
		}
		class ClientFlowLayoutSettigns: ClientLayoutSettigns {
			public override Layout Layout { get { return Layout.Flow; } }
			public int ItemsPerPage { get; set; }
			public override void LoadClientState(ASPxDataViewBase dataView) {
				ItemsPerPage = dataView.SettingsFlowLayoutInternal.ItemsPerPage;
			}
			public override bool AreEqualsState(ASPxDataViewBase dataView) {
				return base.AreEqualsState(dataView) && dataView.SettingsFlowLayoutInternal.ItemsPerPage == ItemsPerPage;
			}
		}
		abstract class ClientLayoutSettigns {
			public abstract Layout Layout { get; }
			public abstract void LoadClientState(ASPxDataViewBase dataView);
			public virtual bool AreEqualsState(ASPxDataViewBase dataView){
				return Layout == dataView.LayoutInternal;
			}
		}
		public DataViewEndlessPagingHelper(ASPxDataViewBase dataView) {
			DataView = dataView;
		}
		public bool PartialLoad { get; private set; }
		public bool IsEmptyCallback { get; set; }
		protected bool ShouldLoadFirstPage { get; set; }
		protected ASPxDataViewBase DataView { get; private set; }
		int ClientPageIndex { get; set; }
		int ClientPageCount { get; set; }
		ClientLayoutSettigns ClientLayoutSettings { get; set; }
		DataViewEndlessPagingMode ClientEndlessPagingMode { get; set; }
		public void LoadClientState(ASPxDataViewBase dataView) {
			if(dataView == null || !dataView.UseEndlessPaging)
				return;
			ClientPageIndex = dataView.PageIndex;
			ClientPageCount = dataView.PageCount;
			ClientLayoutSettings = CreateLayoutSettings(dataView.LayoutInternal);
			ClientLayoutSettings.LoadClientState(dataView);
			var endlessPagingSettings = dataView.PagerSettings as IDataViewEndlessPagingSettigns;
			if(endlessPagingSettings != null)
				ClientEndlessPagingMode = endlessPagingSettings.EndlessPagingMode;
		}
		ClientLayoutSettigns CreateLayoutSettings(Layout layout) {
			if(layout == Layout.Flow)
				return new ClientFlowLayoutSettigns();
			return new ClientTableLayoutSettigns();
		}
		public void Validate() {
			if(ClientLayoutSettings == null || !ClientLayoutSettings.AreEqualsState(DataView) || ClientPageCount > DataView.PageCount)
				ForceLoadFirstPage();
		}
		public void ProcessCallback() {
			Validate();
			if(!ShouldLoadFirstPage) {
				IsEmptyCallback = ClientPageIndex == DataView.PageIndex;
				PartialLoad = true;
			}
		}
		public void ForceLoadFirstPage() {
			PartialLoad = false;
			ShouldLoadFirstPage = true;
			DataView.PageIndex = 0;
		}
		public void AddParametersToCallbackResult(IDictionary callbackResult) {
			callbackResult[ASPxDataViewBase.EndlessPagingHtmlKey] = string.Empty;
			if(!IsEmptyCallback && DataView.PageIndex < DataView.PageCount - 1)
				callbackResult[ASPxDataViewBase.EndlessPagingHtmlKey] = DataView.MainControl.RenderEndlessPagingContainerContent();
		}
		public string GetCallbackContentControlResult() {
			if(IsEmptyCallback)
				return string.Empty;
			using(var sw = new StringWriter(CultureInfo.InvariantCulture))
			using(var writer = new HtmlTextWriter(sw)) {
				DataView.MainControl.ContentControl.RenderEndlessPagingItems(writer);
				return sw.ToString();
			}
		}
	}
}
