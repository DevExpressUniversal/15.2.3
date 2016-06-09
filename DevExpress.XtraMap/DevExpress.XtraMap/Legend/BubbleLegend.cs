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

using System.ComponentModel;
using DevExpress.XtraMap.Native;
using System.Drawing.Design;
namespace DevExpress.XtraMap {
	public enum SizeLegendType {
		Inline,
		Nested
	}
	public class SizeLegend : ItemsLayerLegend {
		public const SizeLegendType DefaultSizeLegendType = SizeLegendType.Inline;
		SizeLegendType type = DefaultSizeLegendType;
		bool showTickMarks = true;
		[
		Category(SRCategoryNames.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TypeConverter(typeof(CollectionConverter)),
		Editor("DevExpress.XtraMap.Design.MapSizeLegendItemCollectionEditor," + "DevExpress.XtraMap" + AssemblyInfo.VSuffixDesign, typeof(UITypeEditor)),
#if !SL
	DevExpressXtraMapLocalizedDescription("SizeLegendCustomItems")
#else
	Description("")
#endif
		]
		public new MapLegendItemCollection CustomItems {
			get { return base.CustomItems; }
		}
		[Category(SRCategoryNames.Layout), 
		DefaultValue(DefaultSizeLegendType),
#if !SL
	DevExpressXtraMapLocalizedDescription("SizeLegendType")
#else
	Description("")
#endif
]
		public SizeLegendType Type {
			get { return type; }
			set {
				if(type == value)
					return;
				type = value;
				OnPropertiesChanged();
			}
		}
		[Category(SRCategoryNames.Layout), 
		DefaultValue(true),
#if !SL
	DevExpressXtraMapLocalizedDescription("SizeLegendShowTickMarks")
#else
	Description("")
#endif
]
		public bool ShowTickMarks {
			get { return showTickMarks; }
			set {
				if(showTickMarks == value)
					return;
				showTickMarks = value;
				OnPropertiesChanged();
			}
		}
		void OnPropertiesChanged() {
			if(Map != null) Map.InvalidateViewInfo();
		}
		public override string ToString() {
			return "(SizeLegend)";
		}
	}
}
