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
using System.ComponentModel;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Text;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public class CloudControlStyles : StylesBase {
		public const string ControlSystemStyleName = "dxccControlSys";
		public const string LinkStyleName = "dxccLink";
		public const string ValueStyleName = "dxccValue";
		public const string BeginEndTextStyleName = "dxccBEText";
		protected override bool MakeLinkStyleAttributesImportant {
			get { return true; }
		}
		protected internal override string GetCssClassNamePrefix() {
			return "dxcc";
		}
		public CloudControlStyles(ASPxCloudControl cloudControl)
			: base(cloudControl) {
		}
		public virtual Color GetMaxColor() {
			return Color.Empty;
		}
		public virtual Color GetMinColor() {			
			return Color.Empty;
		}
		public double GetMinRankFontSize() {
			return 10; 
		}
		protected internal double GetFontSizeInPixels(FontUnit fontUnit) {
			double fontValue = fontUnit.Unit.Value;
			UnitType unitType = fontUnit.Unit.Type;
			if(fontUnit.Type != FontSize.NotSet && fontUnit.Type != FontSize.AsUnit) {
				fontValue = UnitUtils.GetLogicalFontValue(fontUnit.Type);
				unitType = UnitType.Point; 
			}
			UnitUtils.ConvertToPixels(ref unitType, ref fontValue);
			if(unitType != UnitType.Pixel) 
				fontValue = GetMinRankFontSize();
			return Math.Round(fontValue);
		}
	}
}
