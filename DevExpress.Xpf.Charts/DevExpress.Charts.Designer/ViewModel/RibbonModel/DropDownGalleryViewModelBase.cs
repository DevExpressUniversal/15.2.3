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
using DevExpress.Utils;
using DevExpress.Xpf.Bars;
namespace DevExpress.Charts.Designer.Native {
	public abstract class DropDownGalleryViewModelBase : GalleryViewModelBase {
		GallerySizeMode sizeMode = GallerySizeMode.None;
		int initialVisibleColCount = 5;
		int initialVisibleRowCount = 5;
		bool isItemDescriptionVisible = false;
		DefaultBoolean isGroupCaptionVisible = DefaultBoolean.False;
		bool isItemCaptionVisible = false;
		public GallerySizeMode SizeMode {
			get { return sizeMode; }
			set {
				if (sizeMode != value) {
					sizeMode = value;
					OnPropertyChanged("SizeMode");
				}
			}
		}
		public int InitialVisibleColCount {
			get { return initialVisibleColCount; }
			set {
				if (initialVisibleColCount != value) {
					initialVisibleColCount = value;
					OnPropertyChanged("InitialVisibleColCount");
				}
			}
		}
		public int InitialVisibleRowCount {
			get { return initialVisibleRowCount; }
			set {
				if (initialVisibleRowCount != value) {
					initialVisibleRowCount = value;
					OnPropertyChanged("InitialVisibleRowCount");
				}
			}
		}
		public bool IsItemDescriptionVisible {
			get {
				return isItemDescriptionVisible;
			}
			set {
				if (isItemDescriptionVisible != value) {
					isItemDescriptionVisible = value;
					OnPropertyChanged("IsItemDescriptionVisible");
				}
			}
		}
		public bool IsItemCaptionVisible {
			get { return isItemCaptionVisible; }
			set {
				if (isItemCaptionVisible != value) {
					isItemCaptionVisible = value;
					OnPropertyChanged("IsItemCaptionVisible");
				}
			}
		}
		public DefaultBoolean IsGroupCaptionVisible {
			get { return isGroupCaptionVisible; }
			set {
				if (isGroupCaptionVisible != value) {
					isGroupCaptionVisible = value;
					OnPropertyChanged("IsGroupCaptionVisible");
				}
			}
		}
	}
}
