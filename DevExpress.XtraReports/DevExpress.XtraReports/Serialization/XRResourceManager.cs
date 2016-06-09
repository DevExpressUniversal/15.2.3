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
using System.Text;
using System.Resources;
using System.IO;
using System.Collections;
using DevExpress.Data.Utils;
using System.Drawing;
namespace DevExpress.XtraReports.Serialization {
	public class XRResourceManager : ResourceManager {
		[ThreadStatic]
		static Dictionary<string, string> deserializedResources;
		const string imagePostfix = "&System.Drawing.Image";
		public static bool IsImageSpecificName(string name) {
			return !string.IsNullOrEmpty(name) && name.EndsWith(imagePostfix);
		}
		public static string ToImageSpecificName(string name) {
			return name + imagePostfix; 
		}
		public static string FromImageSpecificName(string name) {
			return name.Remove(name.Length - imagePostfix.Length, imagePostfix.Length);
		}
		#region repx resoures 
		public static string GetResourceFor(string reportTypeName) {
			if(deserializedResources != null) {
				return deserializedResources[reportTypeName];
			}
			return null;
		}
		public static void RegisterResourceStrings(Dictionary<string, string> res) {
			deserializedResources = res;
		}
		public static void ClearResourceStrings() {
			deserializedResources = null;
		}
		#endregion
		#region inner classes
		class PatchedResourceSet : ResourceSet {
			[System.Security.SecurityCritical]
			public PatchedResourceSet(Stream stream)
				: base(stream) {
			}
			protected override void ReadResources() {
				IDictionaryEnumerator enumerator = this.Reader.GetEnumerator();
				while(enumerator.MoveNext()) {
					try {
						object obj2 = enumerator.Value;
						if(IsImageSpecificName(enumerator.Key as string) && obj2 is byte[]) {
							Image image = new ImageTool().FromArray((byte[])obj2);
							this.Table.Add(FromImageSpecificName(enumerator.Key as string), image);
						} else
							this.Table.Add(enumerator.Key, obj2);
					} catch { }
				}
			}
		}
		#endregion
		ResourceSet fResourceSet;
		string resources;
		ResourceSet ResourceSet {
			get {
				if(fResourceSet == null)
					fResourceSet = CreateResourceSet(new MemoryStream(Convert.FromBase64String(resources)));
				return fResourceSet;
			}
		}
		[System.Security.SecuritySafeCritical]
		static ResourceSet CreateResourceSet(Stream stream) {
			return new PatchedResourceSet(stream);
		}
		public XRResourceManager(string resources) {
			this.resources = PatchPreviousVersion(resources);
		}
		public override object GetObject(string name) {
			return ResourceSet.GetObject(name);
		}
		public override string GetString(string name) {
			return ResourceSet.GetString(name);
		}
		public override ResourceSet GetResourceSet(System.Globalization.CultureInfo culture, bool createIfNotExists, bool tryParents) {
			return ResourceSet;
		}
		public override void ReleaseAllResources() {
			if(fResourceSet != null) {
				fResourceSet.Close();
				fResourceSet = null;
			}
		}
		static string PatchPreviousVersion(string resources) {
			byte[] bytes = Convert.FromBase64String(resources);
			PatchReportVersion(bytes);
			return Convert.ToBase64String(bytes);
		}
		static void PatchReportVersion(byte[] src) {
			byte[] sampleBytes = System.Text.Encoding.UTF8.GetBytes("DevExpress.XtraReports.v");
			byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes("PublicKeyToken=");
			int index = 0;
			while(true) {
				index = IndexOf(src, sampleBytes, index);
				if(index < 0)
					return;
				index += sampleBytes.Length;
				if(!char.IsDigit((char)src[index]))
					continue;
				index++;
				if(char.IsDigit((char)src[index]))
					index++;
				if(src[index] != '.' || !char.IsDigit((char)src[index + 1]) || src[index + 2] != ',')
					continue;
				index += 2;
				int keyIndex = IndexOf(src, keyBytes, index);
				if(keyIndex < 0)
					continue;
				Clear(src, index, keyIndex + "PublicKeyToken=xxxxxxxxxxxxxxxx".Length);
				index = keyIndex;
			}
		}
		static void Clear(byte[] src, int startIndex, int endIndex) {
			for(int i = startIndex; i < src.Length && i < endIndex; i++)
				src[i] = 0;
		}
		static int IndexOf(byte[] src, byte[] sample, int startIndex) {
			for(int i = startIndex; i < src.Length; i++) {
				if(Equal(src, sample, i))
					return i;
			}
			return -1;
		}
		static bool Equal(byte[] src, byte[] sample, int startIndex) {
			if(src.Length < startIndex + sample.Length)
				return false;
			for(int i = 0; i < sample.Length; i++) {
				if(sample[i] != src[startIndex + i])
					return false;
			}
			return true;
		}
	}
}
