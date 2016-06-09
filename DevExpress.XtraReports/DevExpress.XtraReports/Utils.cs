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
using System.Collections;
using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.UI;
using System.ComponentModel;
using DevExpress.Utils;
using System.Collections.Generic;
using DevExpress.XtraReports.Serialization;
using DevExpress.XtraReports.Localization;
using System.Security;
using DevExpress.XtraReports.Design;
using System.Diagnostics;
using DevExpress.XtraReports.Native.Data;
using System.IO;
namespace DevExpress.XtraReports.Native {
	public class ReportMessageBoxException : ApplicationException {
		public ReportMessageBoxException() { }
		public ReportMessageBoxException(string message)
			: base(message) { }
		public ReportMessageBoxException(string message, Exception innerException)
			: base(message, innerException) { }
		protected ReportMessageBoxException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}
	public static class DesignImageHelper {
		public static ImageList CreateVoidImageList() {
			ImageList imageList = new ImageList();
			imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			imageList.ImageSize = new System.Drawing.Size(16, 16);
			imageList.TransparentColor = System.Drawing.Color.Fuchsia;
			return imageList;
		}
		public static ImageList CreateImageListFromResources(string ResourceBmpName) {
			ImageList imageList = CreateVoidImageList();
			ResourceImageHelper.FillImageListFromResources(imageList, ResourceBmpName, typeof(ResFinder));
			return imageList;
		}
		public static string Get256ColorBitmapName(string typeName) {
			return string.Format("Bitmaps256.{0}.bmp", typeName);
		}
		public static Bitmap Get256ColorBitmap(Type type) {
			return ResourceImageHelper.CreateBitmapFromResources(DesignImageHelper.Get256ColorBitmapName(type.Name), typeof(ResFinder));
		}
	}
	public class UniqueItemList : ArrayList {
		public override void AddRange(ICollection components) {
			foreach(System.ComponentModel.IComponent component in components)
				Add(component);
		}
		public override int Add(object value) {
			return CanAdd(value) ? base.Add(value) : -1;
		}
		bool CanAdd(object value) {
			return value != null && !Contains(value);
		}
	}
	public class DesignItemList : UniqueItemList {
		XtraReport report;
		public DesignItemList(XtraReport report) {
			this.report = report;
		}
		public override int Add(object value) {
			int index = base.Add(value);
			if(value is BindingSource && ((BindingSource)value).DataSource is IComponent)
				index = base.Add(((BindingSource)value).DataSource);
			return index;
		}
		public void AddDataSource(IComponent dataSource) {
			if(ShouldSerializeDataSource(dataSource))
				Add(dataSource);
		}
		bool ShouldSerializeDataSource(IComponent dataSource) {
			return dataSource != null && (dataSource.GetType() != typeof(System.Data.DataSet) || DesignTool.IsEndUserDesigner(report.Site));
		}
		public void AddDataAdapter(object dataAdapter) {
			Add(dataAdapter);
			AddIDbDataAdapterComponents(dataAdapter as System.Data.IDbDataAdapter);
		}
		public void AddIDbDataAdapterComponents(System.Data.IDbDataAdapter dbDataAdapter) {
			if(dbDataAdapter != null) {
				Add(dbDataAdapter.InsertCommand);
				Add(dbDataAdapter.SelectCommand);
				Add(dbDataAdapter.DeleteCommand);
				Add(dbDataAdapter.UpdateCommand);
				if(dbDataAdapter.SelectCommand != null)
					Add(dbDataAdapter.SelectCommand.Connection);
			}
		}
	}
	public static class ReportHelper {
		public static ObjectNameCollection GetColumnNames(XtraReportBase report) {
			using(XRDataContext dataContext = new XRDataContext(null, true)) {
				return dataContext.GetItemDisplayNames(GetEffectiveDataSource(report), report.DataMember);
			}
		}
		public static object GetEffectiveDataSource(XtraReportBase report) {
			if(report == null)
				throw new ArgumentNullException("report");
			object dataSource = report.GetEffectiveDataSource();
			return dataSource != null ? dataSource : report.RootReport.SchemaDataSource;
		}
		public static XtraReport LoadReportLayout(XtraReport report, System.IO.Stream stream) {
			XtraReport source = null;
			report.LoadLayoutInternal(stream, ref source, false);
			return source;
		}
	}
	public class ReportGroupsSynchronizer {
		XtraReportBase report;
		public ReportGroupsSynchronizer(XtraReportBase report) {
			this.report = report;
		}
		bool IsReportValid {
			get { return report.Groups != null && report.Bands != null; }
		}
		public void HandleCollectionChanged(CollectionChangeEventArgs e) {
			if(!IsReportValid)
				return;
			GroupBand groupBand = e.Element as GroupBand;
			switch(e.Action) {
				case CollectionChangeAction.Add:
					if(groupBand != null)
						report.Groups.OnAddBand(groupBand);
					break;
				case CollectionChangeAction.Remove:
					if(groupBand != null)
						report.Groups.OnRemoveBand(groupBand);
					break;
				case CollectionChangeAction.Refresh:
					EnforceGroupsUpdate();
					break;
			}
		}
		public void EnforceGroupsUpdate() {
			if(!IsReportValid)
				return;
			foreach(Band band in report.Bands) {
				GroupBand groupBand = band as GroupBand;
				if(groupBand != null)
					report.Groups.OnAddBand(groupBand, groupBand.Level);
			}
		}
	}
}
