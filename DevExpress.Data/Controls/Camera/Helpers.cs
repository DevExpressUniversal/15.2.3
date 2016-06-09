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
using System.Threading;
namespace DevExpress.Data.Camera {
	[SecuritySafeCritical]
	public static class DeviceHelper {
		internal static IPin GetPin(this IBaseFilter filter, PinDirection dir, int num) {
			IPin[] pin = new IPin[1];
			IEnumPins pinsEnum = null;
			if(filter.EnumPins(out pinsEnum) == 0) {
				PinDirection pinDir;
				int n;
				while(pinsEnum.Next(1, pin, out n) == 0) {
					pin[0].QueryDirection(out pinDir);
					if(pinDir == dir) {
						if(num == 0) {
							return pin[0];
						}
						num--;
					}
					Marshal.ReleaseComObject(pin[0]);
					pin[0] = null;
				}
			}
			return null;
		}
		public static List<CameraDeviceInfo> GetDevices() {
			List<CameraDeviceInfo> list = DeviceMonikes.ToList();
			return list;
		}
		public static CameraDeviceInfo[] DeviceMonikes {
			get {
				List<CameraDeviceInfo> filters = new List<CameraDeviceInfo>();
				IMoniker[] ms = new IMoniker[1];
				ICreateDevEnum enumD = Activator.CreateInstance(Type.GetTypeFromCLSID(CameraImport.SystemDeviceEnum)) as ICreateDevEnum;
				IEnumMoniker moniker;
				Guid g = CameraImport.VideoInputDevice;
				if(enumD.CreateClassEnumerator(ref g, out moniker, 0) == 0) {
					while(true) {
						int r = moniker.Next(1, ms, IntPtr.Zero);
						if(r != 0 || ms[0] == null) {
							break;
						}
						filters.Add(new CameraDeviceInfo(ms[0]));
						Marshal.ReleaseComObject(ms[0]);
						ms[0] = null;
					}
				}
				return filters.ToArray();
			}
		}
	}
	internal class FrameRateCounter {
		public FrameRateCounter() {
			this.timer = new System.Timers.Timer(1000.0);
			this.timer.Elapsed += new System.Timers.ElapsedEventHandler(this.timer_Elapsed);
		}
		System.Timers.Timer timer;
		int count;
		public int FrameRate {
			get;
			private set;
		}
		void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
			this.FrameRate = this.count;
			Interlocked.Exchange(ref this.count, 0);
		}
		public void Inc() {
			Interlocked.Increment(ref this.count);
		}
		public void Start() {
			this.timer.Start();
		}
		public void Stop() {
			this.timer.Stop();
		}
		public void Free() {
			this.Stop();
			this.timer.Elapsed -= new System.Timers.ElapsedEventHandler(this.timer_Elapsed);
			this.timer.Dispose();
		}
	}
}
