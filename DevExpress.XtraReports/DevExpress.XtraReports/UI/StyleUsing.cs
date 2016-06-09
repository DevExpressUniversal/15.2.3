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
using System.Text;
using System.Collections;
using DevExpress.XtraReports.Serialization;
using System.ComponentModel;
using DevExpress.XtraPrinting;
namespace DevExpress.XtraReports.UI {
	[
	DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.StyleUsing"),
	]
	public class StyleUsing : StyleFlagsBase, IXRSerializable {
		internal static StyleUsing CreateEmptyStyleUsing() {
			StyleUsing styleUsing = new StyleUsing();
			styleUsing.UseFontCore = false;
			styleUsing.UseForeColorCore = false;
			styleUsing.UseBackColorCore = false;
			styleUsing.UseBorderColorCore = false;
			styleUsing.UseBordersCore = false;
			styleUsing.UseBorderWidthCore = false;
			styleUsing.UseBorderDashStyleCore = false;
			styleUsing.UseTextAlignmentCore = false;
			styleUsing.UsePaddingCore = false;
			return styleUsing;
		}
		#region Properties
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("StyleUsingUseFont"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.StyleUsing.UseFont"),
		NotifyParentProperty(true),
		]
		public bool UseFont { get { return UseFontCore; } set { UseFontCore = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("StyleUsingUseForeColor"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.StyleUsing.UseForeColor"),
		NotifyParentProperty(true),
		]
		public bool UseForeColor { get { return UseForeColorCore; } set { UseForeColorCore = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("StyleUsingUseBackColor"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.StyleUsing.UseBackColor"),
		NotifyParentProperty(true),
		]
		public bool UseBackColor { get { return UseBackColorCore; } set { UseBackColorCore = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("StyleUsingUsePadding"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.StyleUsing.UsePadding"),
		NotifyParentProperty(true),
		]
		public bool UsePadding { get { return UsePaddingCore; } set { UsePaddingCore = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("StyleUsingUseBorderColor"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.StyleUsing.UseBorderColor"),
		NotifyParentProperty(true),
		]
		public bool UseBorderColor { get { return UseBorderColorCore; } set { UseBorderColorCore = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("StyleUsingUseBorders"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.StyleUsing.UseBorders"),
		NotifyParentProperty(true),
		]
		public bool UseBorders { get { return UseBordersCore; } set { UseBordersCore = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("StyleUsingUseBorderWidth"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.StyleUsing.UseBorderWidth"),
		NotifyParentProperty(true),
		]
		public bool UseBorderWidth { get { return UseBorderWidthCore; } set { UseBorderWidthCore = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("StyleUsingUseTextAlignment"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.StyleUsing.UseTextAlignment"),
		NotifyParentProperty(true),
		]
		public bool UseTextAlignment { get { return UseTextAlignmentCore; } set { UseTextAlignmentCore = value; } }
		#endregion
		public StyleUsing() {
			UseFontCore = true;
			UseForeColorCore = true;
			UseBackColorCore = true;
			UseBorderColorCore = true;
			UseBordersCore = true;
			UseBorderWidthCore = true;
			UseBorderDashStyleCore = true;
		}
		#region Serialization
		void IXRSerializable.SerializeProperties(XRSerializer serializer) {
#if !SL
			serializer.SerializeBoolean("UseFont", UseFontCore);
			serializer.SerializeBoolean("UseForeColor", UseForeColorCore);
			serializer.SerializeBoolean("UseBackColor", UseBackColorCore);
			serializer.SerializeBoolean("UseBorderColor", UseBorderColorCore);
			serializer.SerializeBoolean("UseBorders", UseBordersCore);
			serializer.SerializeBoolean("UseBorderWidth", UseBorderWidthCore);
			serializer.SerializeBoolean("UseTextAlignment", UseTextAlignmentCore);
#endif
		}
		void IXRSerializable.DeserializeProperties(XRSerializer serializer) {
#if !SL
			UseFontCore = serializer.DeserializeBoolean("UseFont", true);
			UseForeColorCore = serializer.DeserializeBoolean("UseForeColor", true);
			UseBackColorCore = serializer.DeserializeBoolean("UseBackColor", true);
			UseBorderColorCore = serializer.DeserializeBoolean("UseBorderColor", true);
			bool useBorderSide = serializer.DeserializeBoolean("UseBorderSide", true);
			UseBordersCore = serializer.DeserializeBoolean("UseBorders", useBorderSide);
			UseBorderWidthCore = serializer.DeserializeBoolean("UseBorderWidth", true);
			UseTextAlignmentCore = serializer.DeserializeBoolean("UseTextAlignment", true);
#endif
		}
		IList IXRSerializable.SerializableObjects { get { return new object[] { }; } }
		#endregion
		#region ShouldSerializeXXX
		internal bool ShouldSerializeUseFont() {
			return UseFontCore == false;
		}
		internal bool ShouldSerializeUseForeColor() {
			return UseForeColorCore == false;
		}
		internal bool ShouldSerializeUseBackColor() {
			return UseBackColorCore == false;
		}
		internal bool ShouldSerializeUseBorderColor() {
			return UseBorderColorCore == false;
		}
		internal bool ShouldSerializeUseBorders() {
			return UseBordersCore == false;
		}
		internal bool ShouldSerializeUseBorderWidth() {
			return UseBorderWidthCore == false;
		}
		internal bool ShouldSerializeUseBorderDashStyle() {
			return UseBorderDashStyleCore == false;
		}
		internal bool ShouldSerializeUseTextAlignment() {
			return UseTextAlignmentCore == true;
		}
		internal bool ShouldSerializeUsePadding() {
			return UsePaddingCore == true;
		}
		#endregion
		protected override StyleFlagsBase CreateInstance() {
			return new StyleUsing();
		}
	}
}
