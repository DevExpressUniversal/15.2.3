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
using System.ComponentModel;
using DevExpress.Map.Native;
using DevExpress.XtraMap.Native;
using System.IO;
using System.Net;
namespace DevExpress.XtraMap {
	public abstract class FileDataAdapterBase : CoordinateSystemDataAdapterBase {
		MapLoaderCore<MapItem> innerLoader;
		Uri fileUri;
		List<MapItem> LoadedItems { get; set; }
		protected bool UriChecked { get; set; }
		protected bool UriExists { get; set; }
		protected virtual bool NeedLoadItems { get { return !AreItemsLoaded && UriChecked && UriExists; } }
		protected MapLoaderCore<MapItem> InnerLoader { get { return innerLoader; } }
		protected override bool IsReady { get { return !NeedLoadItems; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("FileDataAdapterBaseFileUri"),
#endif
		DefaultValue(null)]
		public Uri FileUri {
			get { return fileUri; }
			set {
				if (fileUri == value)
					return;
				fileUri = value;
				if(NeedCheckUri()) {
					UriExists = FileUriExists(fileUri);
					if(!UriExists)
						throw new IncorrectUriException(fileUri);
				}
				OnPropertyChanged();
			}
		}
#if !SL
	[DevExpressXtraMapLocalizedDescription("FileDataAdapterBaseItemsLoaded")]
#endif
		public event ItemsLoadedEventHandler ItemsLoaded;
		protected FileDataAdapterBase() {
			InitInnerLoader();
		}
		#region DataAdapter
		protected override object GetItemSourceObject(MapItem item) {
			return item;
		}
		protected override MapItem GetItemBySourceObject(object sourceObject) {
			return null;
		}
		protected internal override MapItemType GetDefaultMapItemType() {
			return MapItemType.Unknown;
		}
		#endregion
		void InitInnerLoader() {
			this.innerLoader = CreateInnerLoader();
			this.innerLoader.ItemsLoaded += OnLoaderItemsLoaded;
			this.innerLoader.BoundsCalculated += OnBoundsCalculated;
		}
		protected internal bool FileUriExists(Uri uri) {
			UriChecked = false;
			try {
				if(uri.IsFile)
					return new FileInfo(uri.LocalPath).Exists;
				else {
					HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;
					request.Method = "HEAD";
					using(HttpWebResponse response = request.GetResponse() as HttpWebResponse) {
						return response.StatusCode == HttpStatusCode.OK;
					}
				}
			} catch {
				return false;
			} finally {
				UriChecked = true;
			}
		}
		protected virtual bool NeedCheckUri() {
			return true;
		}
		protected override void LoadData(IMapItemFactory factory) {
			if (FileUri == null) {
				AreItemsLoaded = true;
				return;
			}
			PrepareInnerLoader();
			innerLoader.Load(FileUri, false);
		}
		protected abstract MapLoaderCore<MapItem> CreateInnerLoader();
		protected override void OnLoaderItemsLoaded(object sender, ItemsLoadedEventArgs<MapItem> e) {
			lock (UpdateLocker) {
				LoadedItems = e.Items;
				RaiseItemsLoadedEvent(LoadedItems);
				base.OnLoaderItemsLoaded(sender, e);
			}
		}
		protected virtual void PrepareInnerLoader() { }
		protected void RaiseItemsLoadedEvent(IList<MapItem> items) {
			if (ItemsLoaded != null)
				ItemsLoaded(this, new ItemsLoadedEventArgs(items));
		}
		protected override void OnLayerChanged() {
			base.OnLayerChanged();
			PrepareDataLoading();
		}
		protected override bool IsCSCompatibleTo(MapCoordinateSystem mapCS) {
			return mapCS.PointType == GetActualCoordinateSystem().GetSourcePointType();
		}
		public override void Load() {
			if (Layer == null || Layer.Map == null) {
				PrepareInnerLoader();
				if (FileUri != null)
					InnerLoader.Load(FileUri, false);
			}
			base.Load();
		}
	}
}
