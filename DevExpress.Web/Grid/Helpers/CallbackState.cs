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
using System.Web.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DevExpress.Data.IO;
namespace DevExpress.Web.Internal {
	public class GridCallbackState {
		Dictionary<string, object> Store;
		void EnsureStore() {
			if(Store == null)
				Store = new Dictionary<string, object>();
		}
		public void Clear() {
			if(Store != null)
				Store.Clear();
		}
		public T Get<T>(string key) {
			return Get<T>(key, default(T));
		}
		public T Get<T>(string key, T defaultValue) {
			if(Store != null && Store.ContainsKey(key))
				return (T)Store[key];
			return defaultValue;
		}
		public void Put(string key, object value) {
			EnsureStore();
			Store[key] = value;
		}
		public string Save() {
			EnsureStore();
			using(MemoryStream stream = new MemoryStream())
			using(TypedBinaryWriter writer = new TypedBinaryWriter(stream)) {
				writer.WriteObject(Store.Count);
				writer.WriteObject(CalcBinaryCount());
				SaveBinaryValues(writer);
				SaveOtherValues(writer);
				return Convert.ToBase64String(stream.ToArray());
			}			
		}
		public void Load(string value) {
			Clear();
			if(String.IsNullOrEmpty(value))
				return;
			EnsureStore();			
			using(MemoryStream stream = new MemoryStream(Convert.FromBase64String(value)))
			using(TypedBinaryReader reader = new TypedBinaryReader(stream)) {
				int totalCount = reader.ReadObject<int>();
				int binaryCount = reader.ReadObject<int>();
				LoadBinaryValues(reader, binaryCount);
				LoadOtherValues(reader, totalCount - binaryCount);
			}
		}
		int CalcBinaryCount() {
			int result = 0;
			foreach(string key in Store.Keys) {
				if(Store[key] is byte[])
					result++;
			}
			return result;
		}
		void SaveBinaryValues(TypedBinaryWriter writer) {
			foreach(string key in Store.Keys) {
				byte[] binary = Store[key] as byte[];
				if(binary == null)
					continue;
				writer.WriteObject(key);
				writer.WriteObject(binary.Length);
				writer.Write(binary);
			}
		}
		void SaveOtherValues(TypedBinaryWriter writer) {
			foreach(string key in Store.Keys) {
				if(Store[key] is byte[])
					continue;
				writer.WriteObject(key);
				writer.WriteTypedObject(Store[key]);
			}
		}
		void LoadBinaryValues(TypedBinaryReader reader, int count) {
			while(count-- > 0) {
				string key = reader.ReadObject<string>();
				int length = reader.ReadObject<int>();
				Store[key] = reader.ReadBytes(length);
			}
		}
		void LoadOtherValues(TypedBinaryReader reader, int count) {
			while(count-- > 0) {
				string key = reader.ReadObject<string>();
				Store[key] = reader.ReadTypedObject();
			}
		}
	}	
}
