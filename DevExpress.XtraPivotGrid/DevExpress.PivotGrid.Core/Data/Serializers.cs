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
using System.IO;
using System.IO.Compression;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Compatibility.System.IO;
namespace DevExpress.Data.PivotGrid {
	public class PivotXtraSerializer : BinaryXtraSerializer {
		protected static string AppName { get { return "ASPxPivotGrid"; } }
		public bool Serialize(object obj, Stream stream, OptionsLayoutBase options) {
			return base.SerializeObject(obj, stream, AppName, options);
		}
		public void Deserialize(object obj, Stream stream, OptionsLayoutBase options) {
			base.DeserializeObject(obj, stream, AppName, options);
		}
	}
	public class Base64XtraSerializer : PivotXtraSerializer {
		bool resetProperties;
		public string Serialize(IXtraPropertyCollection properties) {
			using(MemoryStream stream = new MemoryStream()) {
				SerializeCore(stream, properties);
				return Convert.ToBase64String(stream.ToArray());
			}
		}
		bool SerializeCore(Stream stream, IXtraPropertyCollection properties) {
			using(DeflateStream compressor = new DeflateStream(stream, CompressionMode.Compress, true)) {
				using(BufferedStream buffered = new BufferedStream(compressor)) {
					return Serialize(buffered, properties, AppName);
				}
			}
		}
		public void Deserialize(object obj, string base64String) {
			Deserialize(obj, base64String, true);
		}
		public void Deserialize(object obj, string base64String, bool resetProperties) {
			using(MemoryStream compressed = new MemoryStream(Convert.FromBase64String(base64String))) {
				Deserialize(obj, compressed, resetProperties);
			}
		}
		public void Deserialize(object obj, Stream stream, bool resetProperties) {
			this.resetProperties = resetProperties;
			using(DeflateStream decompressor = new DeflateStream(stream, CompressionMode.Decompress)) {
				using(MemoryStream bufferedStream = new MemoryStream()) {
					byte[] buffer = new byte[0x1000];
					while(true) {
						int bytesRead = decompressor.Read(buffer, 0, buffer.Length);
						if(bytesRead == 0)
							break;
						bufferedStream.Write(buffer, 0, bytesRead);
					}
					bufferedStream.Position = 0;
					DeserializeObject(obj, bufferedStream, AppName);
				}
			}
		}
		protected override void DeserializeObject(object obj, IXtraPropertyCollection store, OptionsLayoutBase options) {
			if(options == null)
				options = OptionsLayoutBase.FullLayout;
			if(store == null)
				return;
			XtraPropertyCollection coll = new XtraPropertyCollection();
			coll.AddRange(store);
			DeserializeHelper helper = new DeserializeHelper(obj, this.resetProperties);
			helper.ObjectConverterImpl = base.ObjectConverterImpl;
			helper.DeserializeObject(obj, coll, options);
		}
	}
}
