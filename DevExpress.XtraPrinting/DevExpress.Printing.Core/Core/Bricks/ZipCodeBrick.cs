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

using DevExpress.XtraPrinting.Native;
using DevExpress.Utils.Serializing;
using System.ComponentModel;
using DevExpress.XtraPrinting.BrickExporters;
namespace DevExpress.XtraPrinting {
	[BrickExporter(typeof(ZipCodeBrickExporter))]
	public class ZipCodeBrick : TextBrickBase {
		int segmentWidth;
		#region hidden properties
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override object TextValue { get { return base.TextValue; } set { base.TextValue = value; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string TextValueFormatString { get { return base.TextValueFormatString; } set { base.TextValueFormatString = value; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string XlsxFormatString { get { return base.XlsxFormatString; } set { base.XlsxFormatString = value; } }
		#endregion
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("ZipCodeBrickSegmentWidth"),
#endif
		XtraSerializableProperty,
		DefaultValue(0)
		]
		public int SegmentWidth { get { return segmentWidth; } set { segmentWidth = value; } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("ZipCodeBrickBrickType")]
#endif
		public override string BrickType { get { return BrickTypes.ZipCode; } }
		public ZipCodeBrick(IBrickOwner brickOwner) 
			: base(brickOwner) {
		}
		public ZipCodeBrick()
			: this(NullBrickOwner.Instance) {
		}
	}
}
