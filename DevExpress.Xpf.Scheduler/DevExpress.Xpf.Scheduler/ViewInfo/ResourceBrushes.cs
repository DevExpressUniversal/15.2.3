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
using System.Windows.Media;
using DevExpress.XtraScheduler;
using DevExpress.Utils;
using DevExpress.Xpf.Core;
using System.Collections.Generic;
#if SL
using PlatformIndependentColor = System.Windows.Media.Color;
#else
using PlatformIndependentColor = System.Drawing.Color;
#endif
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class ResourceBrushes {
		static Dictionary<int, WeakReference> BrushByColorCache;
		static ResourceBrushes() {
			BrushByColorCache = new Dictionary<int, WeakReference>();
		}
		static Brush GetCachedBrushByColor(Color color) {
			int key = color.ToArgb();
			if (BrushByColorCache.ContainsKey(key)) {
				Brush brush = BrushByColorCache[key].Target as Brush;
				if (brush != null)
					return brush;
				BrushByColorCache.Remove(key);
			}
			Brush newBrush = new SolidColorBrush(color);
#if!SILVERLIGHT
			newBrush.Freeze();
#endif
			BrushByColorCache.Add(key, new WeakReference(newBrush));
			return newBrush;
		}
		Brush cell;
		Brush cellBorder;
		Brush cellBorderDark;
		Brush cellLight;
		Brush cellLightBorder;
		Brush cellLightBorderDark;
		public ResourceBrushes(SchedulerColorSchema schema) {
			Cell = GetCachedBrushByColor(schema.Cell);
			CellBorder = GetCachedBrushByColor(schema.CellBorder);
			CellBorderDark = GetCachedBrushByColor(schema.CellBorderDark);
			CellLight = GetCachedBrushByColor(schema.CellLight);
			CellLightBorder = GetCachedBrushByColor(schema.CellLightBorder);
			CellLightBorderDark = GetCachedBrushByColor(schema.CellLightBorderDark);
		}
		public Brush Cell { 
			get { return cell; } 
			private set { cell = value; } 
		}
		public Brush CellBorder { 
			get { return cellBorder; } 
			private set { cellBorder = value; } }
		public Brush CellBorderDark { 
			get { return cellBorderDark; } 
			private set { cellBorderDark = value; } }
		public Brush CellLight { 
			get { return cellLight; } 
			private set { cellLight = value; } }
		public Brush CellLightBorder { 
			get { return cellLightBorder; } 
			private set { cellLightBorder = value; } }
		public Brush CellLightBorderDark { 
			get { return cellLightBorderDark; } 
			private set { cellLightBorderDark = value; } }
	}
}
