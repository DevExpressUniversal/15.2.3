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
namespace DevExpress.XtraSpreadsheet.Model {
	#region VbaProjectItem
	public class VbaProjectItem {
		#region Fields
		string streamName = string.Empty;
		byte[] data = new byte[0];
		#endregion
		public VbaProjectItem() {
		}
		public VbaProjectItem(string streamName) {
			StreamName = streamName;
		}
		public VbaProjectItem(string streamName, byte[] data) {
			StreamName = streamName;
			Data = data;
		}
		#region Properties
		public string StreamName {
			get { return streamName; }
			set {
				if(value == null)
					streamName = string.Empty;
				else
					streamName = value;
			}
		}
		public byte[] Data {
			get { return data; }
			set {
				if(value == null)
					data = new byte[0];
				else
					data = value;
			}
		}
		public bool IsEmpty { get { return this.data.Length == 0; } }
		#endregion
	}
	#endregion
	#region VbaProject
	public class VbaProject {
		#region Fields
		public const string ProjectStreamName = "PROJECT";
		public const string ProjectLicenseStreamName = "PROJECTlk";
		public const string ProjectNamesStreamName = "PROJECTwm";
		public const string VbaStorageName = "VBA\\";
		public const string VbaProjectStreamName = "VBA\\_VBA_PROJECT";
		public const string DirStreamName = "VBA\\dir";
		readonly List<VbaProjectItem> items = new List<VbaProjectItem>();
		#endregion
		#region Properties
		public IList<VbaProjectItem> Items { get { return items; } }
		public bool IsEmpty { get { return Items.Count == 0; } }
		public bool HasNoMacros { get; set; }
		#endregion
		public void Clear() {
			Items.Clear();
			HasNoMacros = false;
		}
		public VbaProjectItem FindItem(string path) {
			foreach(VbaProjectItem item in Items) {
				if(string.Compare(item.StreamName, path, StringComparison.CurrentCultureIgnoreCase) == 0)
					return item;
			}
			return null;
		}
	}
	#endregion
}
