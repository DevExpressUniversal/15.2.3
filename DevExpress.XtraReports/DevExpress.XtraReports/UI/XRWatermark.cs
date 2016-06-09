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
using System.Drawing;
using System.Collections;
using DevExpress.XtraReports.Serialization;
using DevExpress.XtraPrinting.Drawing;
using System.Collections.Generic;
namespace DevExpress.XtraReports.UI {
	public class XRWatermark : Watermark, IXRSerializable {
		void IXRSerializable.SerializeProperties(XRSerializer serializer) {
			serializer.SerializeString("Text", Text);
			serializer.SerializeFont("Font", Font);
			serializer.SerializeColor("ForeColor", ForeColor);
			serializer.SerializeInteger("Transparency", TextTransparency);
			serializer.SerializeEnum("TextDirection", TextDirection);
			serializer.SerializeValue("Image", Image);
			serializer.SerializeEnum("ImageAlign", ImageAlign);
			serializer.SerializeEnum("ImageViewMode", ImageViewMode);
			serializer.SerializeBoolean("ImageTiling", ImageTiling);
			serializer.SerializeString("PageRange", PageRange);
			serializer.SerializeBoolean("ShowBehind", ShowBehind);
		}
		void IXRSerializable.DeserializeProperties(XRSerializer serializer) {
			Text = serializer.DeserializeString("Text", String.Empty);
			using(Font defaultFont = CreateDefaultFont()) {
				Font = serializer.DeserializeFont("Font", defaultFont);
			}
			ForeColor = serializer.DeserializeColor("ForeColor", Color.Red);
			TextTransparency = serializer.DeserializeInteger("Transparency", 50);
			TextDirection = (DevExpress.XtraPrinting.Drawing.DirectionMode)serializer.DeserializeEnum("TextDirection", typeof(DevExpress.XtraPrinting.Drawing.DirectionMode),
				DevExpress.XtraPrinting.Drawing.DirectionMode.ForwardDiagonal);
			Image = (Image)serializer.DeserializeValue("Image", typeof(Image), null);
			ImageAlign = (ContentAlignment)serializer.DeserializeEnum("ImageAlign", typeof(ContentAlignment), ContentAlignment.MiddleCenter);
			ImageViewMode = (ImageViewMode)serializer.DeserializeEnum("ImageViewMode", typeof(ImageViewMode), ImageViewMode.Clip);
			ImageTiling = serializer.DeserializeBoolean("ImageTiling", false);
			PageRange = serializer.DeserializeString("PageRange", String.Empty);
			ShowBehind = serializer.DeserializeBoolean("ShowBehind", true);
		}
		IList IXRSerializable.SerializableObjects {
			get { return null; }
		}
		[System.ComponentModel.Editor("DevExpress.XtraReports.Design.ImageEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor))]
		public override Image Image {
			get { return base.Image; }
			set { base.Image = value; }
		}
	}
}
