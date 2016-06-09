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
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Drawing.Design;
namespace DevExpress.XtraMap {
	public class CacheOptions : INotifyPropertyChanged {
		const int defaultMemoryLimit = 64;
		const int defaultDiskLimit = -1;
		static string defaultDiskFolder = "";
		static TimeSpan defaultDiskExpireTime = TimeSpan.Zero;
		int memoryLimit;
		int diskLimit;
		TimeSpan diskExpireTime;
		string diskFolder;
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("CacheOptionsMemoryLimit"),
#endif
		DefaultValue(defaultMemoryLimit)]
		public int MemoryLimit {
			get {
				return memoryLimit;
			}
			set {
				if (memoryLimit != value) {
					memoryLimit = value;
					RaisePropertyChanged("MemoryLimit");
				}
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("CacheOptionsDiskLimit"),
#endif
		DefaultValue(defaultDiskLimit)]
		public int DiskLimit {
			get {
				return diskLimit;
			}
			set {
				if (diskLimit != value) {
					diskLimit = value;
					RaisePropertyChanged("DiskLimit");
				}
			}
		}
#if !SL
	[DevExpressXtraMapLocalizedDescription("CacheOptionsDiskExpireTime")]
#endif
		public TimeSpan DiskExpireTime {
			get {
				return diskExpireTime;
			}
			set {
				if(diskExpireTime != value) {
					diskExpireTime = value;
					RaisePropertyChanged("DiskExpireTime");
				}
			}
		}
		void ResetDiskExpireTime() { diskExpireTime = defaultDiskExpireTime; }
		bool ShouldSerializeDiskExpireTime() { return diskExpireTime != defaultDiskExpireTime; }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("CacheOptionsDiskFolder"),
#endif
		DefaultValue(""),
		 Editor("DevExpress.XtraMap.Design.DirectoryUrlEditor," + AssemblyInfo.SRAssemblyMapDesign, typeof(UITypeEditor))]
		public string DiskFolder {
			get {
				return diskFolder;
			}
			set {
				diskFolder = value;
				RaisePropertyChanged("DiskFolder");
			}
		}
		public CacheOptions() {
			this.memoryLimit = defaultMemoryLimit;
			this.diskLimit = defaultDiskLimit;
			this.diskExpireTime = defaultDiskExpireTime;
			this.diskFolder = defaultDiskFolder;
		}
		#region INotifyPropertyChanged Members
#if !SL
	[DevExpressXtraMapLocalizedDescription("CacheOptionsPropertyChanged")]
#endif
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion
		void RaisePropertyChanged(string propertyName) {
			if(PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		public override string ToString() {
			return "(CacheOptions)";
		}
	}
}
