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
using System.Collections.Generic;
using System.Drawing;
using DevExpress.Utils;
namespace DevExpress.DashboardWin.Design {
	abstract class MenuItemController {
		readonly ImageController imageController = new ImageController();
		public object ImageCollection { get { return imageController.ImageCollection; } }
		protected abstract int Count { get; }
		protected abstract Type[] TypesList { get; }
		protected abstract string[] NamesList { get; }
		protected MenuItemController() {
			ForEach((name, bitmap, type) => {
				imageController.AddImage(bitmap, name);
			});
		}
		public virtual int GetImageIndex(object item) {
			Type type = item.GetType();
			int typeIndex = Array.IndexOf<Type>(TypesList, type);
			string key = NamesList[typeIndex];
			return imageController.GetImageIndex(key);
		}
		public void ForEach(Action<string, Bitmap, Type> action) {
			for(int i = 0; i < Count; i++) {
				string name = NamesList[i];
				Bitmap bitmap = BitmapStorage.GetBitmap(name);
				Type type = TypesList[i];
				action(name, bitmap, type);
			}
		}
	}
	class ImageController {
		ImageCollection imageCollection = new ImageCollection();
		Dictionary<string, int> imageIndices = new Dictionary<string, int>();
		public ImageCollection ImageCollection {
			get { return imageCollection; }
		}
		public ImageController() {
		}
		public int AddImage(System.Drawing.Image bitmap, string key) {
			imageCollection.AddImage(bitmap);
			imageIndices.Add(key, imageCollection.Images.Count - 1);
			return imageCollection.Images.Count - 1;
		}
		public int GetImageIndex(string key) {
			int result;
			return imageIndices.TryGetValue(key, out result) ? result : -1;
		}
	} 
}
