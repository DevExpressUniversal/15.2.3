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
using DevExpress.Utils;
using DevExpress.XtraMap.Native;
using DevExpress.XtraPrinting;
using DevExpress.XtraMap.Printing;
using System.Windows.Forms;
using DevExpress.Services.Implementation;
namespace DevExpress.XtraMap.Services {
	public interface IPrintableService {
		IPrintable Printable { get; }
		PrintImageFormat ImageFormat { get; set; }
	}
	public interface IInnerMapService {
		InnerMap Map { get; }
	}
	public interface IColorizerLegendFormatService {
		string FormatLegendItem(MapLegendBase legend, MapLegendItemBase legendItem);
	}
	public abstract class MapServiceBase {
		InnerMap map;
		protected InnerMap Map { get { return map; } }
		protected MapServiceBase(InnerMap map) {
			Guard.ArgumentNotNull(map, "map");
			this.map = map;
		}
	}
	public class InnerMapService : MapServiceBase, IInnerMapService {
		internal InnerMapService(InnerMap map)
			: base(map) {
		}
		InnerMap IInnerMapService.Map { get { return Map; } }
	}
	public class MapPrintableService : MapServiceBase, IPrintableService {
		internal MapPrintableService(InnerMap map)
			: base(map) {
		}
		public IPrintable Printable { get { return Map.Printer; } }
		PrintImageFormat IPrintableService.ImageFormat {
			get {
				return Map.Printer.ImageFormat;
			}
			set {
				Map.Printer.ImageFormat = value;
			}
		}
	}
	public class MapMouseHandlerService : MouseHandlerService {
		readonly InnerMap map;
		public InnerMap Map { get { return map; } }
		public override MouseHandler Handler { get { return Map.MouseHandler; } }
		internal MapMouseHandlerService(InnerMap map)
			: base(map.MouseHandler) {
			this.map = map;
		}
	}
}
