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
using System.IO;
using System.ComponentModel;
using System.Reflection;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.XtraScheduler.Exchange {
	#region AppointmentExchanger
	public abstract class AppointmentExchanger {
		int sourceObjectCount = -1;
		ISchedulerStorageBase storage;
		bool isTermination;
		protected AppointmentExchanger(ISchedulerStorageBase storage) {
			if (storage == null)
				Exceptions.ThrowArgumentException("storage", storage);
			this.storage = storage;
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentExchangerStorage")]
#endif
		public ISchedulerStorageBase Storage { get { return storage; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentExchangerSourceObjectCount")]
#endif
		public int SourceObjectCount {
			get {
				if (sourceObjectCount < 0)
					sourceObjectCount = CalculateSourceObjectCount();
				return sourceObjectCount;
			}
		}
		#region Events
		ExchangeAppointmentEventHandler onGetAppointmentForeignId;
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentExchangerGetAppointmentForeignId")]
#endif
		public event ExchangeAppointmentEventHandler GetAppointmentForeignId { add { onGetAppointmentForeignId += value; } remove { onGetAppointmentForeignId -= value; } }
		protected internal virtual void RaiseGetAppointmentForeignId(ExchangeAppointmentEventArgs e) {
			if (onGetAppointmentForeignId != null) onGetAppointmentForeignId(this, e);
		}
		ExchangeExceptionEventHandler onException;
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentExchangerOnException")]
#endif
		public event ExchangeExceptionEventHandler OnException { add { onException += value; } remove { onException -= value; } }
		protected internal virtual void RaiseOnException(ExchangeExceptionEventArgs e) {
			if (onException != null) onException(this, e);
		}
		#endregion
		protected internal abstract int CalculateSourceObjectCount();
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentExchangerIsTermination")]
#endif
		public bool IsTermination { get { return isTermination; } }
		protected internal void ResetTermination() {
			this.isTermination = false;
		}
		public virtual void Terminate() {
			this.isTermination = true;
		}
	}
	#endregion
	#region AppointmentImporter
	public abstract class AppointmentImporter : AppointmentExchanger {
		protected AppointmentImporter(ISchedulerStorageBase storage)
			: base(storage) {
		}
		#region Events
		AppointmentImportingEventHandler onAppointmentImporting;
		AppointmentImportedEventHandler onAppointmentImported;
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentImporterAppointmentImporting")]
#endif
		public event AppointmentImportingEventHandler AppointmentImporting { add { onAppointmentImporting += value; } remove { onAppointmentImporting -= value; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentImporterAppointmentImported")]
#endif
		public event AppointmentImportedEventHandler AppointmentImported { add { onAppointmentImported += value; } remove { onAppointmentImported -= value; } }
		protected internal virtual void RaiseOnAppointmentImporting(AppointmentImportingEventArgs e) {
			if (onAppointmentImporting != null) onAppointmentImporting(this, e);
		}
		protected internal virtual void RaiseOnAppointmentImported(AppointmentImportedEventArgs e) {
			if (onAppointmentImported != null) onAppointmentImported(this, e);
		}
		#endregion
		public virtual void Import(string path) {
			using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)) {
				Import(fs);
			}
		}
		public virtual void Import(Stream stream) {
			Storage.BeginUpdate();
			try {
				ResetTermination();
				ImportCore(stream);
			}
			finally {
				Storage.EndUpdate();
			}
		}
		protected internal abstract void ImportCore(Stream stream);
		protected internal MemoryStream CreateStreamCopy(Stream stream) {
			if (stream == null || stream == Stream.Null)
				return null;
			long pos = stream.Position;
			long count = stream.Length - pos;
			byte[] bytes = new byte[count];
			stream.Read(bytes, 0, (int)count);
			return new MemoryStream(bytes, false);
		}
	}
	#endregion
	#region AppointmentExporter
	public abstract class AppointmentExporter : AppointmentExchanger {
		protected AppointmentExporter(ISchedulerStorageBase storage)
			: base(storage) {
		}
		#region Events
		AppointmentExportingEventHandler onAppointmentExporting;
		AppointmentExportedEventHandler onAppointmentExported;
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentExporterAppointmentExporting")]
#endif
		public event AppointmentExportingEventHandler AppointmentExporting { add { onAppointmentExporting += value; } remove { onAppointmentExporting -= value; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentExporterAppointmentExported")]
#endif
		public event AppointmentExportedEventHandler AppointmentExported { add { onAppointmentExported += value; } remove { onAppointmentExported -= value; } }
		protected internal virtual void RaiseOnAppointmentExporting(AppointmentExportingEventArgs e) {
			if (onAppointmentExporting != null) onAppointmentExporting(this, e);
		}
		protected internal virtual void RaiseOnAppointmentExported(AppointmentExportedEventArgs e) {
			if (onAppointmentExported != null) onAppointmentExported(this, e);
		}
		#endregion
		public virtual void Export(string path) {
			using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read)) {
				Export(fs);
			}
		}
		public virtual void Export(Stream stream) {
			Storage.BeginUpdate();
			try {
				ResetTermination();
				ExportCore(stream);
			}
			finally {
				Storage.EndUpdate();
			}
		}
		protected internal override int CalculateSourceObjectCount() {
			return ((IInternalSchedulerStorageBase)Storage).CalcTotalAppointmentCountForExchange();
		}
		protected internal abstract void ExportCore(Stream stream);
	}
	#endregion
	#region AppointmentSynchronizer
	public abstract class AppointmentSynchronizer : AppointmentExchanger {
		string foreignIdFieldName = string.Empty;
		protected AppointmentSynchronizer(ISchedulerStorageBase storage)
			: base(storage) {
		}
		#region Events
		AppointmentSynchronizingEventHandler onAppointmentSynchronizing;
		AppointmentSynchronizedEventHandler onAppointmentSynchronized;
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentSynchronizerAppointmentSynchronizing")]
#endif
		public event AppointmentSynchronizingEventHandler AppointmentSynchronizing { add { onAppointmentSynchronizing += value; } remove { onAppointmentSynchronizing -= value; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentSynchronizerAppointmentSynchronized")]
#endif
		public event AppointmentSynchronizedEventHandler AppointmentSynchronized { add { onAppointmentSynchronized += value; } remove { onAppointmentSynchronized -= value; } }
		protected internal virtual void RaiseOnAppointmentSynchronizing(AppointmentSynchronizingEventArgs e) {
			if (onAppointmentSynchronizing != null) onAppointmentSynchronizing(this, e);
		}
		protected internal virtual void RaiseOnAppointmentSynchronized(AppointmentSynchronizedEventArgs e) {
			if (onAppointmentSynchronized != null) onAppointmentSynchronized(this, e);
		}
		#endregion
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentSynchronizerForeignIdFieldName")]
#endif
		public string ForeignIdFieldName {
			get { return foreignIdFieldName; }
			set {
				if (value == null) value = string.Empty;
				foreignIdFieldName = value;
			}
		}
		public virtual void Synchronize() {
			Storage.BeginUpdate();
			try {
				ResetTermination();
				SynchronizeCore();
			}
			finally {
				Storage.EndUpdate();
			}
		}
		protected internal abstract void SynchronizeCore();
	}
	#endregion
	#region AppointmentImportSynchronizer
	public abstract class AppointmentImportSynchronizer : AppointmentSynchronizer {
		protected AppointmentImportSynchronizer(ISchedulerStorageBase storage)
			: base(storage) {
		}
	}
	#endregion
	#region AppointmentExportSynchronizer
	public abstract class AppointmentExportSynchronizer : AppointmentSynchronizer {
		protected AppointmentExportSynchronizer(ISchedulerStorageBase storage)
			: base(storage) {
		}
		protected internal override int CalculateSourceObjectCount() {
			return ((IInternalSchedulerStorageBase)Storage).CalcTotalAppointmentCountForExchange();
		}
	}
	#endregion
	public static class ExchangeHelper {
		internal static string CreateValidAssemblyName(string shortAssemblyName) {
			Assembly assembly = Assembly.GetExecutingAssembly();
			string assemblyName = assembly.FullName.Split(',')[0]; 
			return String.Format(shortAssemblyName + assembly.FullName.Substring(assemblyName.Length));
		}
		internal static string GetICalendarAssemblyName() {
			return CreateValidAssemblyName(AssemblyInfo.SRAssemblySchedulerCore);
		}
		public static AppointmentImporter CreateImporter(ISchedulerStorageBase storage, string assemblyName, string fullClassName) {
			return CreateExchanger(storage, assemblyName, fullClassName, typeof(AppointmentImporter)) as AppointmentImporter;
		}
		public static AppointmentExporter CreateExporter(ISchedulerStorageBase storage, string assemblyName, string fullClassName) {
			return CreateExchanger(storage, assemblyName, fullClassName, typeof(AppointmentExporter)) as AppointmentExporter;
		}
		internal static bool CanCreateExporter(string assemblyName, string fullClassName) {
			return CanCreateExchanger(assemblyName, fullClassName, typeof(AppointmentExporter));
		}
		internal static bool CanCreateExchanger(string assemblyName, string fullClassName, Type baseType) {
			try {
				Assembly asm = Assembly.Load(assemblyName);
				if (asm == null)
					return false;
				Type importerClass = asm.GetType(fullClassName);
				if (importerClass == null)
					return false;
				return true;
			}
			catch {
				return false;
			}
		}
		internal static AppointmentExchanger CreateExchanger(ISchedulerStorageBase storage, string assemblyName, string fullClassName, Type baseType) {
			try {
				Assembly asm = Assembly.Load(assemblyName);
				if (asm == null)
					return null;
				Type importerClass = asm.GetType(fullClassName);
				if (importerClass == null)
					return null;
				if (!baseType.IsAssignableFrom(importerClass))
					return null;
				ConstructorInfo ci = importerClass.GetConstructor(new Type[] { typeof(ISchedulerStorageBase) });
				if (ci == null)
					return null;
				return ci.Invoke(new object[] { storage }) as AppointmentExchanger;
			}
			catch {
				return null;
			}
		}
#if !SL
		internal static string GetOutlookAssemblyName() {
			return CreateValidAssemblyName(AssemblyInfo.SRAssemblySchedulerCore);
		}
		internal static string GetVCalendarAssemblyName() {
			return CreateValidAssemblyName(AssemblyInfo.SRAssemblySchedulerCore);
		}
		public static AppointmentImportSynchronizer CreateImportSynchronizer(ISchedulerStorageBase storage, string assemblyName, string fullClassName) {
			return CreateExchanger(storage, assemblyName, fullClassName, typeof(AppointmentImportSynchronizer)) as AppointmentImportSynchronizer;
		}
		public static AppointmentExportSynchronizer CreateExportSynchronizer(ISchedulerStorageBase storage, string assemblyName, string fullClassName) {
			return CreateExchanger(storage, assemblyName, fullClassName, typeof(AppointmentExportSynchronizer)) as AppointmentExportSynchronizer;
		}
#endif
	}
}
