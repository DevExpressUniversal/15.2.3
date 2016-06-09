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
#if SILVERLIGHT
using DevExpress.Xpf.Drawing;
#else
using System.Drawing;
#endif
namespace DevExpress.XtraPrinting.Native {
	public class ImagesContainer : ObjectContainer<Image>, IImagesContainer {
		readonly List<IDisposable> storedObjects = new List<IDisposable>();
		public void Add(object key, Image image) {
			if(key == null)
				key = new ImageInfo(image);
			Items.Add(key, image);
		}
		public bool ContainsImage(object key) {
			return Items.ContainsKey(key);
		}
		public Image GetImageByKey(object key) {
			Image image;
			return key != null && Items.TryGetValue(key, out image)
				? (Image)image
				: null;
		}
		public Image GetImage(object key, Image image) {
			if(image == null)
				return null;
			if(key == null)
				key = new ImageInfo(image);
			return (Image)GetObject(key, image);
		}
		public Image GetImage(Image image) {
			return GetImage(null, image);
		}
		public void ResetHash() {
			foreach(IDisposable obj in Items.Values)
				if(!storedObjects.Contains(obj))
					storedObjects.Add(obj);
			Items.Clear();
		}
		public override void Clear() {
			foreach(IDisposable obj in storedObjects)
				obj.Dispose();
			storedObjects.Clear();
			base.Clear();
		}
	}
}
