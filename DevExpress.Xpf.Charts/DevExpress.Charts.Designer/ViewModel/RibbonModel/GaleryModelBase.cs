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

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
namespace DevExpress.Charts.Designer.Native {
	public abstract class GalleryViewModelBase : RibbonItemViewModelBase {
		ObservableCollection<GalleryGroupViewModel> galleryGroupModels = new ObservableCollection<GalleryGroupViewModel>();
		public ObservableCollection<GalleryGroupViewModel> Groups {
			get { return galleryGroupModels; }
		}
		public GalleryViewModelBase() {
			galleryGroupModels.CollectionChanged += OnGalleryGroupModelsCollectionChanged;
		}
		void UpdateIsEnabled() {
			bool isEnabledLocal = false;
			foreach (GalleryGroupViewModel group in galleryGroupModels)
				if (!group.AreAllCommandsDisabled) {
					isEnabledLocal = true;
					break;
				}
			IsEnabled = isEnabledLocal;
		}
		void OnGalleryGroupModelsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if (e.NewItems != null)
				foreach (GalleryGroupViewModel item in e.NewItems)
					item.PropertyChanged += OnItemPropertyChanged;
			if (e.OldItems != null)
				foreach (GalleryGroupViewModel item in e.OldItems)
					item.PropertyChanged -= OnItemPropertyChanged;
			UpdateIsEnabled();
		}
		void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e) {
			if (e.PropertyName == "AreAllCommandsDisabled")
				UpdateIsEnabled();
		}
		public override void CleanUp() {
			base.CleanUp();
			foreach (var model in Groups)
				model.CleanUp();
		}
	}
}
