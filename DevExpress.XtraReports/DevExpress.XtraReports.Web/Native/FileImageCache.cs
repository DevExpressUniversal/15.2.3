#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using DevExpress.Web;
namespace DevExpress.XtraReports.Web.Native {
	public class FileImageCache : ImageCache {
		public const string NamePrefix = "f_";
		readonly string imageDir = XRFileStore.GetTempDirectory();
		public override void Add(string key, XRBinaryStorageData data, ASPxWebControlBase control) {
			try {
				string filename = GetFileName(key);
				if(!File.Exists(filename)) {
					using(Stream stream = File.OpenWrite(filename)) {
						var serializer = new BinaryFormatter();
						serializer.Serialize(stream, data);
					}
				}
				XRFileStore.Start(control.Page.Application);
			} catch {
				base.Add(key, data, control);
			}
		}
		public override BinaryStorageData Get(string key) {
			try {
				string filename = GetFileName(key);
				using(Stream stream = File.OpenRead(filename)) {
					var serializer = new BinaryFormatter();
					var result = serializer.Deserialize(stream) as BinaryStorageData;
					return result != null
						? result
						: base.Get(key);
				}
			} catch {
				return base.Get(key);
			}
		}
		public override string GetName(byte[] content) {
			return NamePrefix + base.GetName(content);
		}
		public override void Remove(string key) {
			string filename = GetFileName(key);
			if(File.Exists(filename)) {
				File.Delete(filename);
			} else {
				base.Remove(key);
			}
		}
		string GetFileName(string name) {
			return Path.Combine(imageDir, name + ".img");
		}
	}
}
