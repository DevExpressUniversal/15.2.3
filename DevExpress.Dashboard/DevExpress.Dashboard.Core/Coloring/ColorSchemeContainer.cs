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
using DevExpress.DataAccess;
using DevExpress.DataAccess.Native;
using DevExpress.Utils;
using System.Collections.Generic;
namespace DevExpress.DashboardCommon.Native {
	public class ColorSchemeContainer : IChangeService {
		readonly ColorRepository repository = new ColorRepository();
		readonly ColorScheme colorScheme = new ColorScheme();
		readonly IColorSchemeContext context;
		Locker updateRepositoryLocker = new Locker();
		event EventHandler<ChangedEventArgs> IChangeService.Changed { add { } remove { } }
		public ColorScheme Scheme { get { return colorScheme; } }
		public ColorRepository Repository { get { return repository; } }
		public ColorSchemeContainer(IColorSchemeContext context) {
			Guard.ArgumentNotNull(context, "context");
			this.context = context;
			colorScheme.CollectionChanged += OnColorSchemeCollectionChanged;
		}
		public void UpdateRepository(ColorRepository newRepository, IDataSourceInfoProvider dataInfoProvider) {
			repository.Assign(newRepository);
			updateRepositoryLocker.Lock(); 
			try {
				colorScheme.Clear();
				colorScheme.AddRange(GetEnsuredColorSchemeEntries(dataInfoProvider));
			}
			finally {
				updateRepositoryLocker.Unlock();
			}
			OnChanged();
		}
		public ColorRepository CloneRepository() {
			return Repository.Clone();
		}
		public void OnEndLoading() {
			RefreshRepository();
		}
		public void Reset() {
			colorScheme.Clear();
		}
		public void RefreshRepository() {
			repository.SetColorSchemeEntries(colorScheme);
		}
		void OnColorSchemeCollectionChanged(object sender, NotifyingCollectionChangedEventArgs<ColorSchemeEntry> e) {
			foreach(ColorSchemeEntry entry in e.AddedItems) {
				entry.ChangeService = this;
			}
			foreach(ColorSchemeEntry entry in e.RemovedItems) {
				entry.ChangeService = null;
			}
			if(!context.Loading && !updateRepositoryLocker.IsLocked)
				OnColorSchemeChanged();
		}
		void IChangeService.OnChanged(ChangedEventArgs e) {
			if(!context.Loading)
				OnColorSchemeChanged();
		}
		void OnColorSchemeChanged() {
			RefreshRepository();
			OnChanged();
		}
		void OnChanged() {
			if(!context.IsChangeLocked && context.ChangeService != null)
				context.ChangeService.OnChanged(new ChangedEventArgs(ChangeReason.Coloring, null, null));
		}
		IList<ColorSchemeEntry> GetEnsuredColorSchemeEntries(IDataSourceInfoProvider dataInfoProvider) {
			IList<ColorSchemeEntry> entries = repository.GetColorSchemeEntries();
			if (dataInfoProvider != null) {
				foreach (ColorSchemeEntry entry in entries) {
					if (entry.DataSource == null) {
						DataSourceInfo dataInfo = dataInfoProvider.GetDataSourceInfo(entry.DataSourceName, entry.DataMember);
						if (dataInfo != null)
							entry.DataSource = dataInfo.DataSource;
					}
				}
			}
			return entries;
		}
	}
}
