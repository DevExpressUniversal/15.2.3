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
using System.Linq;
using System.Text;
using DevExpress.Utils.Design;
namespace DevExpress.XtraCharts.Designer.Native {
	[ModelOf(typeof(ChartImage))]
	public class ChartImageModel : DesignerChartElementModelBase {
		readonly ChartImage image;
		protected ChartImage ChartImage { get { return image; } }
		protected internal override ChartElement ChartElement { get { return image; } }
		[PropertyForOptions]
		public Image Image {
			get { return ChartImage.Image; }
			set { SetProperty("Image", value); }
		}
		[PropertyForOptions]
		public string ImageUrl {
			get { return ChartImage.ImageUrl; }
			set { SetProperty("ImageUrl", value); }
		}
		public ChartImageModel(ChartImage image, CommandManager commandManager)
			: base(commandManager) {
			this.image = image;
		}
	}
	[ModelOf(typeof(BackgroundImage))]
	public class BackgroundImageModel : ChartImageModel {
		protected new BackgroundImage ChartImage { get { return (BackgroundImage)base.ChartImage; } }
		[PropertyForOptions, TypeConverter(typeof(BooleanTypeConverter))]
		public bool Stretch {
			get { return ChartImage.Stretch; }
			set { SetProperty("Stretch", value); }
		}
		public BackgroundImageModel(BackgroundImage image, CommandManager commandManager)
			: base(image, commandManager) {
		}
	}
}
