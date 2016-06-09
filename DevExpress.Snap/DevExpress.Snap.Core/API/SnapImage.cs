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

namespace DevExpress.Snap.Core.API {
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Linq;
	using System.Text;
	using DevExpress.Snap.Core.Fields;
	using DevExpress.XtraPrinting;
	public interface SnapImage : SnapSingleListItemEntity {
		ImageSizeMode ImageSizeMode { get; set; }		   
		UpdateMergeImageFieldMode UpdateMode { get; set; }  
		float ScaleX { get; set; }						  
		float ScaleY { get; set; }						  
		int Width { get; set; }							 
		int Height { get; set; }							
		Size Size { get; set; }							 
	}
}
namespace DevExpress.Snap.API.Native {
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Linq;
	using System.Text;
	using DevExpress.Snap.Core.Fields;
	using DevExpress.XtraPrinting;
	using DevExpress.Snap.Core.API;
	using ModelField = DevExpress.XtraRichEdit.Model.Field;
	using ApiField = DevExpress.XtraRichEdit.API.Native.Field;
	public class NativeSnapImage : NativeSnapSingleListItemEntity, SnapImage {
		ImageSizeMode imageSizeMode;
		UpdateMergeImageFieldMode updateMode;
		int scaleX; 
		int scaleY; 
		int width;
		int height;
		public NativeSnapImage(SnapNativeDocument document, ApiField field) : base(document, field) { }
		public NativeSnapImage(SnapSubDocument subDocument, SnapNativeDocument document, ApiField field) : base(subDocument, document, field) { }
		protected override void Init() {
			base.Init();
			SNImageField parsedField = GetParsedField<SNImageField>();
			this.imageSizeMode = parsedField.Sizing;
			this.updateMode = parsedField.UpdateMode;
			this.scaleX = GetScale(parsedField, SNImageField.ScaleXSwitch);
			this.scaleY = GetScale(parsedField, SNImageField.ScaleYSwitch);
			this.width = parsedField.Switches.GetInt(SNImageField.WidthSwitch);
			this.height = parsedField.Switches.GetInt(SNImageField.HeightSwitch);
		}
		int GetScale(SNImageField parsedField, string key) { return parsedField.Switches.Switches.ContainsKey('\\' + key) ? parsedField.Switches.GetInt(key) : 100; }
		#region SnapImage Members
		public ImageSizeMode ImageSizeMode {
			get { return imageSizeMode; }
			set {
				EnsureUpdateBegan();
				Controller.SetSwitch(SNImageField.ImageSizeModeSwitch, SNImageField.GetImageSizeModeString(value));
				imageSizeMode = value;
			}
		}
		public UpdateMergeImageFieldMode UpdateMode {
			get { return updateMode; }
			set {
				EnsureUpdateBegan();
				Controller.SetSwitch(SNImageField.UpdateModeSwitch, SNImageField.GetUpdateModeString(value));
				updateMode = value;
			}
		}
		public float ScaleX {
			get { return 0.01f * scaleX; }
			set {
				EnsureUpdateBegan();
				scaleX = (int)(100.0f * value);
				if(scaleX == 100 && scaleY == 100) {
					Controller.RemoveSwitch(SNImageField.ScaleXSwitch);
					Controller.RemoveSwitch(SNImageField.ScaleYSwitch);
				}
				else
					Controller.SetSwitch(SNImageField.ScaleXSwitch, Convert.ToString(scaleX));
			}
		}
		public float ScaleY {
			get { return 0.01f * scaleY; }
			set {
				EnsureUpdateBegan();
				scaleY = (int)(100.0f * value);
				if(scaleX == 100 && scaleY == 100) {
					Controller.RemoveSwitch(SNImageField.ScaleXSwitch);
					Controller.RemoveSwitch(SNImageField.ScaleYSwitch);
				}
				else
					Controller.SetSwitch(SNImageField.ScaleYSwitch, Convert.ToString(scaleY));
			}
		}
		public int Width {
			get { return width; }
			set {
				EnsureUpdateBegan();
				width = value;
				if(width == 0 && height == 0) {
					Controller.RemoveSwitch(SNImageField.WidthSwitch);
					Controller.RemoveSwitch(SNImageField.HeightSwitch);
				}
				else
					Controller.SetSwitch(SNImageField.WidthSwitch, Convert.ToString(value));
			}
		}
		public int Height {
			get { return height;  }
			set {
				EnsureUpdateBegan();
				height = value;
				if(width == 0 && height == 0) {
					Controller.RemoveSwitch(SNImageField.WidthSwitch);
					Controller.RemoveSwitch(SNImageField.HeightSwitch);
				}
				else
					Controller.SetSwitch(SNImageField.HeightSwitch, Convert.ToString(value));
			}
		}
		public Size Size {
			get { return new Size(Width, Height); }
			set {
				Width = value.Width;
				Height = value.Height;
			}
		}
		#endregion
	}
}
