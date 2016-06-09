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
using DevExpress.Web.Internal;
namespace DevExpress.Web.Office.Internal {
	public interface IWorkSessionHibernateProvider : IWorkSessionAutoProcessingProvider {
		void Hibernate(IWorkSession workSession);
		void WakeUp(IWorkSession workSession);
		void OnDisposingSettingChanged();
	}
	public abstract class WorkSessionHibernationProviderBase : IWorkSessionHibernateProvider {
		static TimeSpan TimeSpanCorrection = TimeSpan.FromMinutes(1);
		public WorkSessionHibernationProviderBase() {
			ASPxHttpHandlerModule.Subscribe(new DocumentWorkSessionManagerSubscriber(), true);
		}
		protected virtual bool HibernationEnabled { 
			get {
				return WorkSessionProcessing.EnableHibernation && HibernationSettings.HibernateTimeout != TimeSpan.Zero && IsHibernationStorageValid(WorkSessionProcessing.HibernationStoragePath);
			}
		}
		protected abstract bool IsHibernationStorageValid(string hibernationStorage);
		internal bool StorageMaintainingProcessed { get; set; }
		public bool ShouldBeProcessed(IWorkSession workSession, DateTime now) {
			return ShouldBeHibernated(workSession, now) || ShouldBeDisposed(workSession, now);
		}
		bool ShouldBeHibernated(IWorkSession workSession, DateTime now) {
			bool shouldBeHibernated = CanBeHibernated(workSession.State) && HibernationTimeoutExpired(workSession, now);
			return shouldBeHibernated;
		}
		bool ShouldBeDisposed(IWorkSession workSession, DateTime now) {
			bool shouldBeDisposed = CanBeDisposed(workSession.State) && DisposingTimeoutExpired(workSession, now);
			return shouldBeDisposed;
		}
		bool CanBeHibernated(WorkSessionState state) {
			return HibernationEnabled && state == WorkSessionState.Loaded;
		}
		static bool HibernationTimeoutExpired(IWorkSession workSession, DateTime now) {
			return now - workSession.LastTimeActivity > GetHibernateTimeout(workSession);
		}
		bool CanBeDisposed(WorkSessionState state) {
			return HibernationEnabled && ( state == WorkSessionState.Loaded || state == WorkSessionState.Hibernated);
		}
		static bool DisposingTimeoutExpired(IWorkSession workSession, DateTime now) {
			return now - workSession.LastTimeActivity > GetHibernatedDisposeTimeout(workSession);
		}
		public void Process(IWorkSession workSession) {
			var now = DateTime.Now;
			if(ShouldBeHibernated(workSession, now))
				Hibernate(workSession);
			if(ShouldBeDisposed(workSession, now))
				Dispose(workSession);
		}
		void Dispose(IWorkSession workSession) {
			if(workSession != null)
				Dispose(workSession.ID);
		}
		void Dispose(Guid workSessionId) {
			WorkSessions.CloseWorkSession(workSessionId);
			DisposeHibernationContainer(workSessionId);
		}
		internal static TimeSpan GetHibernateTimeout(IWorkSession workSession) {
			TimeSpan autoSaveTimout = workSession.EnableAutoSave ? workSession.AutoSaveTimeout : TimeSpan.FromMilliseconds(0);
			TimeSpan correctedHibernateTimeout = autoSaveTimout > WorkSessionProcessing.HibernateTimeout ? (autoSaveTimout + TimeSpanCorrection) : WorkSessionProcessing.HibernateTimeout;
			return correctedHibernateTimeout;
		}
		internal static TimeSpan GetHibernatedDisposeTimeout(IWorkSession workSession) {
			TimeSpan hibernateTimout = GetHibernateTimeout(workSession);
			TimeSpan correctedDisposeTimeout = hibernateTimout > WorkSessionProcessing.HibernatedDocumentsDisposeTimeout ? (hibernateTimout + TimeSpanCorrection) : WorkSessionProcessing.HibernatedDocumentsDisposeTimeout;
			return correctedDisposeTimeout;
		}
		public void Hibernate(IWorkSession workSession) {
			var hibernationContainer = workSession.GetHibernationContainer();
			if(hibernationContainer != null) {
				PersistHibernationContainer(hibernationContainer, workSession.ID);
				workSession.OnHibernated();
			}
		}
		public void WakeUp(IWorkSession workSession) {
			var hibernationContainer = LoadHibernationContainer(workSession.ID);
			if(hibernationContainer != null)
				workSession.WakeUp(hibernationContainer);
		}
		public void OnDisposingSettingChanged() {
			if(HibernationEnabled && !StorageMaintainingProcessed){
				StorageMaintainingProcessed = true;
				StartStorageMaintaining();
			}
		}
		void StartStorageMaintaining() {
			DisposeOutdatedHibernationContainers();
			CreateReadyToWakeUpWorksessionsInternal();
		}
		void CreateReadyToWakeUpWorksessionsInternal() {
			EnsureOfficeProducts();
			CreateReadyToWakeUpWorkSessions();
		}
		void EnsureOfficeProducts() {
		   WorkSessionFactoryRegistrator.RegisterViaOfficeAssemblyAttributes();
		}
		protected abstract void PersistHibernationContainer(HibernationContainer hibernationContainer, Guid workSessionId);
		protected abstract HibernationContainer LoadHibernationContainer(Guid workSessionId);
		protected abstract void DisposeHibernationContainer(Guid workSessionId);
		protected abstract void DisposeOutdatedHibernationContainers();
		protected abstract void CreateReadyToWakeUpWorkSessions();
	}
	class WorkSessionFactoryRegistrator {
		public static void RegisterViaOfficeAssemblyAttributes() {
			var assemblyFullNames = new List<string>() {
				AssemblyInfo.SRAssemblyWebSpreadsheetFull,
				AssemblyInfo.SRAssemblyWebRichEditFull
			};
			foreach(var assemblyFullName in assemblyFullNames) {
				try {
					var assembly = AppDomain.CurrentDomain.Load(assemblyFullName);
					RegisterWorkSessionViaOfficeAssemblyAttribute(assembly);
				} catch(Exception e) {
					CommonUtils.RaiseCallbackErrorInternal(null, e);
				}
			}
		}
		private static void RegisterWorkSessionViaOfficeAssemblyAttribute(System.Reflection.Assembly assembly) {
			assembly.GetCustomAttributes(false);
		}
	}
}
