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

using System.Collections.Generic;
using System.Windows.Media;
namespace DevExpress.XtraPrinting.Native {
	public static class ImageRepository {
		static Dictionary<string, Dictionary<string, ImageSource>> items;
		static ImageRepository() {
			items = new Dictionary<string, Dictionary<string, ImageSource>>();
		}
		public static void RegisterImageSource(string psId, string imageId, ImageSource imageSource) {
			if(!items.ContainsKey(psId))
				items.Add(psId, new Dictionary<string, ImageSource>());
			if(items[psId].ContainsKey(imageId))
				items[psId].Remove(imageId);
			items[psId].Add(imageId, imageSource);
		}
		public static ImageSource GetImageSource(string psId, string imageId) {
			if(!items.ContainsKey(psId) || !items[psId].ContainsKey(imageId))
				throw new KeyNotFoundException("ImageSource with such ID doesn't registered in ImageRepository");
			return items[psId][imageId];
		}
		public static void Clear(string psId) {
			if(!items.ContainsKey(psId) || items[psId].Count == 0)
				return;
			items[psId].Clear();
			items.Remove(psId);
		}
	}
}
