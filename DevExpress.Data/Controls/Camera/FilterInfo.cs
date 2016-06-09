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

using DevExpress.Data.Camera.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;
using System.Text;
namespace DevExpress.Data.Camera {
	[SecuritySafeCritical]
	public class CameraDeviceInfo : IComparable {
		public readonly string Name;
		public readonly string MonikerString;
		public CameraDeviceInfo(string monikerString) {
			this.MonikerString = monikerString;
			this.Name = this.GetName(monikerString);
		}
		public CameraDeviceInfo(string monikerString, string name) {
			this.MonikerString = monikerString;
			this.Name = name;
		}
		internal CameraDeviceInfo(IMoniker moniker) {
			this.MonikerString = this.GetMonikerString(moniker);
			this.Name = this.GetName(moniker);
		}
		public int CompareTo(object value) {
			CameraDeviceInfo f = (CameraDeviceInfo)value;
			if(f == null) {
				return 1;
			}
			return this.Name.CompareTo(f.Name);
		}
		internal static IBaseFilter CreateFilter(string filterMoniker) {
			object filterObject = null;
			IBindCtx bindCtx = null;
			IMoniker moniker = null;
			int n = 0;
			if(CameraImport.CreateBindCtx(0, out bindCtx) == 0) {
				if(CameraImport.MkParseDisplayName(bindCtx, filterMoniker, ref n, out moniker) == 0) {
					Guid filterId = typeof(IBaseFilter).GUID;
					moniker.BindToObject(null, null, ref filterId, out filterObject);
					Marshal.ReleaseComObject(moniker);
				}
				Marshal.ReleaseComObject(bindCtx);
			}
			return filterObject as IBaseFilter;
		}
		internal static IMoniker GetMoniker(string filterMoniker) {
			IBindCtx bindCtx = null;
			IMoniker moniker = null;
			int n = 0;
			if(CameraImport.CreateBindCtx(0, out bindCtx) == 0) {
				if(CameraImport.MkParseDisplayName(bindCtx, filterMoniker, ref n, out moniker) == 0) {
				}
				Marshal.ReleaseComObject(bindCtx);
			}
			return moniker;
		}
		string GetMonikerString(IMoniker moniker) {
			string str;
			moniker.GetDisplayName(null, null, out str);
			return str;
		}
		string GetName(IMoniker moniker) {
			object bagObj = null;
			IPropertyBag bag = null;
			try {
				Guid bagId = typeof(IPropertyBag).GUID;
				moniker.BindToStorage(null, null, ref bagId, out bagObj);
				bag = (IPropertyBag)bagObj;
				object val = string.Empty;
				int hr = bag.Read("FriendlyName", ref val, IntPtr.Zero);
				if(hr != 0) {
					Marshal.ThrowExceptionForHR(hr);
				}
				string ret = (string)val;
				if((ret == null) || (ret.Length < 1)) {
					throw new ApplicationException();
				}
				return ret;
			}
			catch(Exception) {
				return string.Empty;
			}
			finally {
				bag = null;
				if(bagObj != null) {
					Marshal.ReleaseComObject(bagObj);
					bagObj = null;
				}
			}
		}
		string GetName(string monikerString) {
			IBindCtx bindCtx = null;
			IMoniker moniker = null;
			string name = string.Empty;
			int n = 0;
			if(CameraImport.CreateBindCtx(0, out bindCtx) == 0) {
				if(CameraImport.MkParseDisplayName(bindCtx, monikerString, ref n, out moniker) == 0) {
					name = this.GetName(moniker);
					Marshal.ReleaseComObject(moniker);
					moniker = null;
				}
				Marshal.ReleaseComObject(bindCtx);
				bindCtx = null;
			}
			return name;
		}
		public override string ToString() {
			return Name;
		}
	}
}
