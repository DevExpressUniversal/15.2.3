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
using System;
using System.ComponentModel;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting.BrickExporters;
namespace DevExpress.XtraPrinting {
	[BrickExporter(typeof(LabelBrickExporter))]
	public class LabelBrick : TextBrick, ILabelBrick {
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("LabelBrickAngle"),
#endif
		XtraSerializableProperty,
		DefaultValue(0f),
		]
		public float Angle {
			get { return GetValue(BrickAttachedProperties.Angle, 0f); }
			set { SetAttachedValue(BrickAttachedProperties.Angle, value, 0f); }
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("LabelBrickIsVerticalTextMode")]
#endif
		public bool IsVerticalTextMode { get { return Math.Abs(Math.Sin(Math.PI * RealAngle / 180)) == 1; } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("LabelBrickBrickType")]
#endif
		public override string BrickType { get { return BrickTypes.Label; } }
		float RealAngle { get { return -Angle; } }
		public LabelBrick()
			: this(NullBrickOwner.Instance) {
		}
		public LabelBrick(IBrickOwner brickOwner) 
			: base(brickOwner) {
		}
		LabelBrick(LabelBrick labelBrick) : base(labelBrick) {
			Angle = labelBrick.Angle;			
		}
		public override object Clone() {
			return new LabelBrick(this);
		}
	}
}
