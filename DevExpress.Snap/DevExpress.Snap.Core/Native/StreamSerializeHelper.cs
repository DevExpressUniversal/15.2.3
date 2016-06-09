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

using DevExpress.Utils.Serializing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DevExpress.Snap.Core.Fields;
namespace DevExpress.Snap.Core.Native {
	public static class StreamSerializeHelper {
		public static string ToBase64String(Action<MemoryStream> serialize) {
			using (MemoryStream stream = new MemoryStream()) {
				serialize(stream);
				return Convert.ToBase64String(stream.ToArray());
			}
		}
		public static string ToBase64String(object obj) {
			return ToBase64String(stream => new XmlXtraSerializer().SerializeObject(obj, stream, string.Empty));
		}
		public static byte[] ToByteArray(Action<MemoryStream> serialize) {
			using(MemoryStream stream = new MemoryStream()) {
				serialize(stream);
				return stream.ToArray();
			}
		}
		public static byte[] ToByteArray(object obj) {
			return ToByteArray(stream => new XmlXtraSerializer().SerializeObject(obj, stream, string.Empty));
		}
		public static void FromBase64String(string data, Action<MemoryStream> deserialize) {
			using(MemoryStream stream = new MemoryStream(Convert.FromBase64String(data)))
				deserialize(stream);
		}
		public static void FromBase64String(string data, object obj) {
			FromBase64String(data, stream => new XmlXtraSerializer().DeserializeObject(obj, stream, string.Empty));
		}
		public static T FromBase64String<T>(string data) where T : new() {
			T result = new T();
			FromBase64String(data, result);
			return result;
		}
		public static void FromBase64StringDataContainer(Base64StringDataContainer data, Action<MemoryStream> deserialize) {
			using(MemoryStream stream = new MemoryStream(data.GetData()))
				deserialize(stream);
		}
		public static void FromBase64StringDataContainer(Base64StringDataContainer data, object obj) {
			FromBase64StringDataContainer(data, stream => new XmlXtraSerializer().DeserializeObject(obj, stream, string.Empty));
		}
		public static T FromBase64StringDataContainer<T>(Base64StringDataContainer data) where T : new() {
			T result = new T();
			FromBase64StringDataContainer(data, result);
			return result;
		}
	}
}
