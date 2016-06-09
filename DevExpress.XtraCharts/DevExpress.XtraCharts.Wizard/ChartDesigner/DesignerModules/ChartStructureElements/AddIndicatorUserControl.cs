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
using System.Reflection;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraCharts.Native;
using System.Linq;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts.Designer.Native {
	public partial class AddIndicatorUserControl : GalleryWithFilterUserControl {
		class TypeComparer : IComparer<Type>{
			public int Compare(Type x, Type y) {
				return string.Compare(x.Name, y.Name);
			}
		}
		IEnumerable<Type> GetIndicatorTypes() {
			var asm = Assembly.GetAssembly(typeof(Chart));
			return asm.GetTypes().Where(t => t.IsSubclassOf(typeof(Indicator)) && !t.IsAbstract);
		}
		protected override void FillGallery(GalleryControl gallery) {
			var group = new GalleryItemGroup();
			gallery.Gallery.Groups.Add(group);
			List<Type> indicatorTypes = GetIndicatorTypes().ToList();
			indicatorTypes.Sort(new TypeComparer());
			foreach (Type indicatorType in indicatorTypes) {
				if (indicatorType == typeof(FibonacciIndicator)) {
					string fibonacciArcs = ChartLocalizer.GetString(ChartStringId.FibonacciArcs);
					GalleryItem arcsGalleryItem = new GalleryItem(null, null, fibonacciArcs, "", 0, 0, FibonacciIndicatorKind.FibonacciArcs, fibonacciArcs);
					group.Items.Add(arcsGalleryItem);
					string fibonacciFans = ChartLocalizer.GetString(ChartStringId.FibonacciFans);
					GalleryItem fansGalleryItem = new GalleryItem(null, null, fibonacciFans, "", 0, 0, FibonacciIndicatorKind.FibonacciFans, fibonacciFans);
					group.Items.Add(fansGalleryItem);
					string fibonacciRetracement = ChartLocalizer.GetString(ChartStringId.FibonacciFans);
					GalleryItem retracementGalleryItem = new GalleryItem(null, null, fibonacciRetracement, "", 0, 0, FibonacciIndicatorKind.FibonacciRetracement, fibonacciRetracement);
					group.Items.Add(retracementGalleryItem);
				}
				else {
					var id = (ChartStringId)Enum.Parse(typeof(ChartStringId), "Ind" + indicatorType.Name);
					string hintAndCaption = ChartLocalizer.GetString(id);
					GalleryItem galleryItem = new GalleryItem(null, null, hintAndCaption, "", 0, 0, indicatorType, hintAndCaption);
					group.Items.Add(galleryItem);
				}
			}
		}
	}
}
