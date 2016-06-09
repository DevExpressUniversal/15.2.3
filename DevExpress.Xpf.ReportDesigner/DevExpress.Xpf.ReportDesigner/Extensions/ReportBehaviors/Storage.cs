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

using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Reports.UserDesigner.Extensions;
using DevExpress.Xpf.Reports.UserDesigner.Native.ReportExtensions;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
namespace DevExpress.Xpf.Native {
	class StringPairList : List<StringPairList.StringPair> {
		public class StringPair {
			public string Item1 { get; set; }
			public string Item2 { get; set; }
		}
	}
	class FileSystemStore {
		string dir;
		readonly string metadataExt = ".metadata";
		public FileSystemStore(string dir) {
			this.dir = dir;
			Directory.CreateDirectory(dir);
		}
		public string AddItem(byte[] item, byte[] metadata) {
			var key = Guid.NewGuid().ToString();
			UpdateItem(key, item, metadata);
			return key;
		}
		public byte[] GetItem(string key) {
			return File.ReadAllBytes(Path.Combine(dir, key));
		}
		public string[] GetKeys() {
			return Directory.GetFiles(dir)
				.Where(f => !f.EndsWith(metadataExt))
				.Select(Path.GetFileName)
				.ToArray();
		}
		public byte[] GetMetadata(string key) {
			var path = Path.Combine(dir, key + metadataExt);
			if(!File.Exists(path))
				return null;
			return File.ReadAllBytes(path);
		}
		public void RemoveItem(string key) {
			File.Delete(Path.Combine(dir, key));
			File.Delete(Path.Combine(dir, key + metadataExt));
		}
		public void UpdateItem(string key, byte[] item, byte[] metadata) {
			if (item != null) {
				File.WriteAllBytes(Path.Combine(dir, key), item);
			}
			File.WriteAllBytes(Path.Combine(dir, key + metadataExt), metadata);
		}
	}
	class PersistentReportSerializer {
		public XtraReport Load(byte[] bytes) {
			return XtraReportSerializer.Load(bytes);
		}
		public byte[] Save(XtraReport report) {
			return XtraReportSerializer.Save(report);
		}
	}
	class PersistentReportInfoSerializer {
		[DataContract]
		class SerializedReportInfo {
			[DataMember]
			public string Key { get; set; }
			[DataMember]
			public string Name { get; set; }
		}
		public ReportInfo Load(byte[] state) {
			if(state.Length == 0)
				return null;
			try {
				var serializer = new DataContractSerializer(typeof(SerializedReportInfo));
				using(var ms = new MemoryStream(state)) {
					var proxy = (SerializedReportInfo)serializer.ReadObject(ms);
					return new ReportInfo(proxy.Key, false) {
						Name = proxy.Name
					};
				}
			} catch {
				return null;
			}
		}
		public byte[] Save(ReportInfo item) {
			if(item == null)
				return new byte[0];
			var serializer = new DataContractSerializer(typeof(SerializedReportInfo));
			using(var ms = new MemoryStream()) {
				serializer.WriteObject(ms, new SerializedReportInfo {
					Key = item.Key,
					Name = item.Name
				});
				return ms.ToArray();
			}
		}
	}
	class PredefinedReportsStore {
		PredefinedReportCollection collection;
		public PredefinedReportsStore(PredefinedReportCollection collection) {
			this.collection = collection;
		}
		public XtraReport GetItem(string key) {
			return collection.FirstOrDefault(r => r.ReportName == key)
				.With(info => (XtraReport)Activator.CreateInstance(info.Type));
		}
		public string[] GetKeys() {
			return collection.Select(r => r.ReportName).ToArray();
		}
		public ReportInfo GetMetadata(string key) {
			return collection.FirstOrDefault(r => r.ReportName == key)
				.With(m => new ReportInfo(key, true) { Name = key });
		}
	}
}
