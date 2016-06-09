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
using System.Drawing;
using System.Text;
namespace DevExpress.Web {
	public class ImageGalleryItemEventArgs : EventArgs {
		private ImageGalleryItem item = null;
		public ImageGalleryItem Item {
			get { return item; }
		}
		public ImageGalleryItemEventArgs(ImageGalleryItem item)
			: base() {
			this.item = item;
		}
	}
	public class ImageGalleryCustomImageProcessingEventArgs : EventArgs {
		public Bitmap Image { get; private set; }
		public Graphics Graphics { get; private set; }
		public ImageGalleryImageLocation ImageLocation { get; private set; }
		public ImageGalleryCustomImageProcessingEventArgs(Graphics graphics, Bitmap image, ImageGalleryImageLocation imageLocation)
			: base() {
			Graphics = graphics;
			ImageLocation = imageLocation;
			Image = image;
		}
	}
	public delegate void ImageGalleryItemEventHandler(object source, ImageGalleryItemEventArgs e);
	public delegate void ImageGalleryCustomImageProcessingEventHandler(object source, ImageGalleryCustomImageProcessingEventArgs e);
}
