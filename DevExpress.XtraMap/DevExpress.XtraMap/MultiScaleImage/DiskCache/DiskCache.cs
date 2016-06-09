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
using System.IO;
using System.Threading;
using DevExpress.Utils;
namespace DevExpress.XtraMap.Native {
	public struct SaveData {
		public byte[] Data { get; set; }
		public string FileName { get; set; }
	}
	public class DiskCache : MapDisposableObject {
		public const string constCacheFileExtension = ".xtramapcache";
		readonly DirectoryInfo root;
		readonly DiskCacheMap cacheMap;
		readonly ManualResetEventSlim watchdogEvent;
		readonly ManualResetEventSlim watchdogStartedEvent;
		Exception watchdogException;
		object watchdogLocker;
		bool watchdogRunning;
		protected internal DirectoryInfo Root { get { return root; } }
		protected DiskCacheMap CacheMap { get { return cacheMap; } }
		bool IsWatchdogRunning {
			get {
				lock(watchdogLocker)
					return watchdogRunning;
			}
			set {
				lock(watchdogLocker)
					watchdogRunning = value;
			}
		}
		public TimeSpan ExpireTime { get; set; }
		public int DiskLimit { get; set; }
		public DiskCache(string folder, TimeSpan expireTime, int diskLimit) {
			Guard.ArgumentIsNotNullOrEmpty(folder, "folder");
			this.root = new DirectoryInfo(folder);
			this.root.Create();
			this.cacheMap = new DiskCacheMap(this);
			ExpireTime = expireTime;
			DiskLimit = diskLimit;
			this.watchdogLocker = new object();
			this.watchdogEvent = new ManualResetEventSlim(true);
			this.watchdogStartedEvent = new ManualResetEventSlim(false);
			ThreadPool.QueueUserWorkItem(new WaitCallback(Watchdog));
			watchdogStartedEvent.Wait();
			watchdogStartedEvent.Dispose();
			watchdogStartedEvent = null;
		}
		void ProcessCache() {
			if((Root != null) && (DiskLimit > 0)) {
				long limitInBytes = (long)DiskLimit << 20;
				long currentSize = CacheMap.Size;
				if(CacheMap.Size > limitInBytes) {
					if(watchdogEvent.IsSet)
						return;
					List<DiskCacheMapRecord> toRemove = new List<DiskCacheMapRecord>();
					for(int index = CacheMap.Timeline.Count - 1; index >= 0; index--) {
						DiskCacheMapRecord record = CacheMap.Timeline[index];
						FileInfo file = null;
						string fullName = String.Format("{0}\\{1}", Root.FullName, record.Name);
						if(File.Exists(fullName))
							file = new FileInfo(fullName);
						else
							file = null;
						if(file != null)
							File.Delete(file.FullName);
						toRemove.Add(record);
						currentSize -= record.Size;
						if(currentSize <= limitInBytes)
							break;
					}
					foreach(var remove in toRemove)
						cacheMap.RemoveRecord(remove);
				}
			}
		}
		void Watchdog(object target) {
			IsWatchdogRunning = true;
			watchdogStartedEvent.Set();
			while(true) {
				watchdogEvent.Wait();
				if(IsDisposed)
					break;
				lock(CacheMap) {
					watchdogEvent.Reset();
					try {
						ProcessCache();
					}
					catch(Exception exc) {
						watchdogException = exc;
						break;
					}
				}
			}
			IsWatchdogRunning = false;
		}
		void CheckWatchdog() {
			if(!IsWatchdogRunning) {
				if(watchdogException != null)
					throw new AggregateException("DiskCache watchdog thread not running", watchdogException);
				else
					throw new Exception("DiskCache watchdog thread not running");
			}
		}
		string GetItemName(TileIndex index, string salt) {
			return string.Format("{0}_{1}_{2}_{3}{4}", salt, index.X, index.Y, index.Level, constCacheFileExtension);
		}
		void SaveToDisk(object obj) {
			if(IsDisposed || obj.GetType() != typeof(SaveData))
				return;
			SaveData saveTask = (SaveData)obj;
			if((saveTask.Data != null) && !string.IsNullOrWhiteSpace(saveTask.FileName)) {
				try {
					File.WriteAllBytes(String.Format("{0}\\{1}", Root.FullName, saveTask.FileName), saveTask.Data);
					lock(CacheMap) {
						CacheMap.PushRecord(saveTask.FileName, saveTask.Data.Length);
					}
				}
				catch(Exception e) {
					e.ToString();
				}
				lock(CacheMap) {
					if(!IsDisposed)
						CheckWatchdog();
					watchdogEvent.Set();
				}
			}
		}
		public void Push(byte[] data, TileIndex index, string salt) {
			if(Root != null) {
				SaveData saveData = new SaveData() { Data = data, FileName = GetItemName(index, salt) };
				ThreadPool.QueueUserWorkItem(new WaitCallback(SaveToDisk), saveData);
			}
		}
		public TileImageSource Retrieve(TileIndex index, string salt) {
			lock(CacheMap) {
				if(!IsDisposed && Root != null) {
					CheckWatchdog();
					FileInfo file = null;
					string fileName = GetItemName(index, salt);
					var record = CacheMap.GetRecord(fileName);
					if(record != null) {
						string fullName = String.Format("{0}\\{1}", Root.FullName, fileName);
						if(File.Exists(fullName)) {
							file = new FileInfo(fullName);
							double fileLives = (DateTime.Now - record.Time).TotalMilliseconds;
							if(ExpireTime != TimeSpan.Zero && fileLives >= ExpireTime.TotalMilliseconds) {
								File.Delete(fullName);
								CacheMap.RemoveRecord(fileName);
								return null;
							}
							CacheMap.UpdateRecord(record);
							return new DiskImageSource(index, file);
						}
						else
							CacheMap.RemoveRecord(fileName);
					}
				}
				return null;
			}
		}
		public bool LocatedAt(string folder) {
			return string.Equals(Root.FullName, folder);
		}
		protected override void DisposeOverride() {
			lock(CacheMap) {
				watchdogEvent.Set();
				watchdogEvent.Dispose();
			}
		}
	}
}
