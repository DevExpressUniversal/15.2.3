#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Xml.Linq;
namespace DevExpress.DashboardCommon.Native {
	public class DashboardImage {
		const string xmlUrl = "Url";
		static readonly byte[] ErrorData = new byte[0];
		string url;
		byte[] data;
		public bool HasImage { get { return data != null || !string.IsNullOrEmpty(url); } }
		public string Url {
			get { return url; }
			set {
				if(value != url) {
					url = value;
					data = null;
					OnChanged();
				}
			}
		}
		public byte[] Data {
			get { return data; }
			set {
				if(value != data) {
					data = value;
					url = null;
					OnChanged();
				}
			}
		}
		public string Base64Data { 
			get { return Data != null ? Convert.ToBase64String(Data) : null; } 
			set { Data = !string.IsNullOrEmpty(value) ? Convert.FromBase64String(value) : null; } 
		}
		public event EventHandler Changed;
		public void SaveToXml(XElement element) {
			if(ShouldSerializeData())
				element.Value = Base64Data;
			if(ShouldSerializeUrl())
				element.Add(new XAttribute(xmlUrl, url));
		}
		public void LoadFromXml(XElement element) {
			string value = element.Value;
			if(!string.IsNullOrEmpty(value)) {
				try {
					Base64Data = value;
				}
				catch {
					Data = ErrorData;
				}
			}
			string urlAttr = XmlHelper.GetAttributeValue(element, xmlUrl);
			if(!string.IsNullOrEmpty(urlAttr))
				Url = urlAttr;
		}
		public bool ShouldSerializeData() {
			return data != null;
		}
		public bool ShouldSerializeUrl() {
			return !string.IsNullOrEmpty(url);
		}
		void OnChanged() {
			if(Changed != null)
				Changed(this, EventArgs.Empty);
		}
	}
}
