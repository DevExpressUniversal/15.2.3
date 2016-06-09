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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Data;
using DevExpress.Xpf.Reports.UserDesigner.Native;
using DevExpress.Xpf.Reports.UserDesigner.Native.ReportExtensions;
using DevExpress.XtraReports.UI;
namespace DevExpress.Xpf.Reports.UserDesigner {
	public class ReportDesignerDocumentSourceConverter : IValueConverter {
		public static XtraReport ToReport(object reportSource) {
			return ToReport(reportSource, null, null);
		}
		public static XtraReport ToReport(object reportSource, IReportStorage storage) {
			return ToReport(reportSource, null, storage);
		}
		public static XtraReport ToReport(object reportSource, IReportSerializer serializer) {
			return ToReport(reportSource, serializer, null);
		}
		public static XtraReport ToReport(object reportSource, IReportSerializer serializer, IReportStorage storage) {
			if(reportSource == null) return null;
			byte[] data = reportSource as byte[];
			if(data != null)
				return (serializer ?? new DefaultReportSerializer()).Load(data);
			return ToReportCore(reportSource, serializer, storage);
		}
		public static byte[] ToByteArray(object reportSource) {
			return ToByteArray(reportSource, null, null);
		}
		public static byte[] ToByteArray(object reportSource, IReportStorage storage) {
			return ToByteArray(reportSource, null, storage);
		}
		public static byte[] ToByteArray(object reportSource, IReportSerializer serializer) {
			return ToByteArray(reportSource, serializer, null);
		}
		public static byte[] ToByteArray(object reportSource, IReportSerializer serializer, IReportStorage storage) {
			if(reportSource == null) return null;
			byte[] data = reportSource as byte[];
			if(data != null)
				return data;
			return serializer.Save(ToReportCore(reportSource, serializer, storage));
		}
		static XtraReport ToReportCore(object reportSource, IReportSerializer serializer, IReportStorage storage) {
			Stream stream = reportSource as Stream;
			if(stream != null)
				return (serializer ?? new DefaultReportSerializer()).Load(stream);
			XtraReport report = reportSource as XtraReport;
			if(report != null)
				return report;
			string filePath = reportSource as string;
			if(filePath != null)
				return (storage ?? new DefaultReportStorage()).Load(filePath, serializer ?? new DefaultReportSerializer());
			throw new ArgumentException("", "reportSource"); 
		}
		IReportSerializer reportSerializer = new DefaultReportSerializer();
		public IReportSerializer ReportSerializer {
			get { return reportSerializer; }
			set { reportSerializer = value; }
		}
		IReportStorage storage = new DefaultReportStorage();
		public IReportStorage ReportStorage {
			get { return storage; }
			set { storage = value; }
		}
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return ConvertCore(value, targetType);
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return ConvertCore(value, targetType);
		}
		object ConvertCore(object value, Type targetType) {
			if(value == null || targetType == null || targetType.IsAssignableFrom(value.GetType())) return value;
			if(targetType == typeof(byte[]))
				return ToByteArray(value, ReportSerializer, ReportStorage);
			if(targetType.IsAssignableFrom(typeof(XtraReport)))
				return ToReport(value, ReportSerializer, ReportStorage);
			return value;
		}
	}
}
