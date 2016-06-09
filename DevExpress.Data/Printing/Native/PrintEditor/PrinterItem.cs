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
using System.Printing;
using DevExpress.Printing.Native.PrintEditor;
using DevExpress.XtraPrinting.Localization;
namespace DevExpress.Printing {
	public class PrinterItem : IDisposable {
		PrintQueue printQueue;
		SafeGetter<string> fullName;
		SafeGetter<string> location;
		SafeGetter<string> comment;
		SafeGetter<string> printerDocumentsInQueue;
		SafeGetter<string> displayName;
		SafeStructGetter<PrinterStatus> printerStatus;
		PrinterType printerType;
		public string Location { get { return location.Value; } }
		public string Comment { get { return comment.Value; } }
		public string PrinterDocumentsInQueue { get { return printerDocumentsInQueue.Value; } }
		public string Status { get { return printerStatus.Value.GetString(); } }
		public string DisplayName { get { return displayName.Value; } }
		public string FullName { get { return fullName.Value; } }
		public PrinterType PrinterType {
			get {
				if(printerStatus.Value.HasFlag(PrinterStatus.Offline) || printerStatus.Value.HasFlag(PrinterStatus.ServerOffline))
					printerType |= PrinterType.Offline;
				return printerType;
			}
		}
		public PrinterItem(PrintQueue printQueue, bool isFax, bool isNetwork, bool isDefault, bool isOffline) {
			this.printQueue = printQueue;
			fullName = new SafeGetter<string>(() => this.printQueue.FullName, "Undefined Printer");
			location = new SafeGetter<string>(() => this.printQueue.Location, "Undefined Location");
			comment = new SafeGetter<string>(() => this.printQueue.Comment, string.Empty);
			printerDocumentsInQueue = new SafeGetter<string>(() => this.printQueue.NumberOfJobs.ToString(), "0");
			if(isNetwork) {
				displayName = new SafeGetter<string>(() => {
					string printServerName = this.printQueue.HostingPrintServer.Name.Replace(@"\", string.Empty);
					return string.Format(PreviewStringId.NetworkPrinterFormat.GetString(), this.printQueue.Name, printServerName);
				}, "Undefined Printer");
			} else
				displayName = new SafeGetter<string>(() => this.printQueue.Name, "Undefined Printer");
			printerStatus = new SafeStructGetter<PrinterStatus>(() => (PrinterStatus)this.printQueue.QueueStatus, PrinterStatus.NotAvailable);
			if(isFax)
				printerType |= PrinterType.Fax;
			if(isNetwork)
				printerType |= PrinterType.Network;
			if(isDefault)
				printerType |= PrinterType.Default;
			if(isOffline)
				printerType |= PrinterType.Offline;
		}
		class SafeGetter<T> where T : class {
			Func<T> get;
			T value = null;
			T defaultValue;
			public T Value {
				get {
					if(value == null) {
						try {
							value = get();
						} catch {
							value = defaultValue;
						}
					}
					return value;
				}
			}
			public SafeGetter(Func<T> get, T defaultValue) {
				this.get = get;
				this.defaultValue = defaultValue;
			}
		}
		class SafeStructGetter<T> where T : struct {
			Func<T> get;
			Nullable<T> value = null;
			T defaultValue;
			public T Value {
				get {
					if(value == null) {
						try {
							value = get();
						} catch {
							value = defaultValue;
						}
					}
					return value.Value;
				}
			}
			public SafeStructGetter(Func<T> get, T defaultValue) {
				this.get = get;
				this.defaultValue = defaultValue;
			}
		}
		public virtual void Dispose() {
			if(printQueue != null) {
				printQueue.Dispose();
				printQueue = null;
			}
		}
	}
	class SafeGetter<T> where T : class {
		Func<T> get;
		T value = null;
		T defaultValue;
		public T Value {
			get {
				if(value == null) {
					try {
						value = get();
					} catch {
						value = defaultValue;
					}
				}
				return value;
			}
		}
		public SafeGetter(Func<T> get, T defaultValue) {
			this.get = get;
			this.defaultValue = defaultValue;
		}
	}
	class SafeStructGetter<T> where T : struct {
		Func<T> get;
		Nullable<T> value = null;
		T defaultValue;
		public T Value {
			get {
				if(value == null) {
					try {
						value = get();
					} catch {
						value = defaultValue;
					}
				}
				return value.Value;
			}
		}
		public SafeStructGetter(Func<T> get, T defaultValue) {
			this.get = get;
			this.defaultValue = defaultValue;
		}
	}
}
