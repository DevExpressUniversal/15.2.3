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
using System.Drawing;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraTreeList;
namespace DevExpress.XtraCharts.Designer.Native {
	[UserRepositoryItem("Register")]
	public class RepositoryItemImageButtonEdit : RepositoryItemButtonEdit {
		internal const string EditorName = "ImageButtonEdit";
		static RepositoryItemImageButtonEdit() {
			Register();
		}
		public static void Register() {
			EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(EditorName, typeof(ImageButtonEdit),
				typeof(RepositoryItemImageButtonEdit), typeof(ImageButtonEditViewInfo),
				new ButtonEditPainter(), false, null, typeof(DevExpress.Accessibility.ButtonEditAccessible)));
		}
		public override string EditorTypeName {
			get { return EditorName; }
		}
		public RepositoryItemImageButtonEdit() {
		}
	}
	public class ImageButtonEdit : ButtonEdit {
		static ImageButtonEdit() {
			RepositoryItemImageButtonEdit.Register();
		}
		public override string EditorTypeName {
			get { return RepositoryItemImageButtonEdit.EditorName; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemImageButtonEdit Properties {
			get { return base.Properties as RepositoryItemImageButtonEdit; }
		}
		public ImageButtonEdit() {
		}
	}
	public class ImageButtonEditViewInfo : ButtonEditViewInfo {
		public ImageButtonEditViewInfo(RepositoryItem item) : base(item) {
		}
		protected override EditorButtonPainter CreateButtonPainter() {
			return new EditorImageButtonPainter(LookAndFeel);
		}
	}
	public class EditorImageButtonPainter : SkinEditorButtonPainter {
		public EditorImageButtonPainter(ISkinProvider provider)
			: base(provider) {
		}
		float GetImageOpacity(ObjectState state) {
			switch (state) {
				case ObjectState.Normal:
					return 0.65f;
				case ObjectState.Hot:
					return 1.0f;
				case ObjectState.Disabled:
					return 0.35f;
				default:
					return 0.35f;
			}
		}
		Bitmap ColorBitmap(Image original, float opacity, Color newColor) {
			Bitmap bitmap = new Bitmap(original);
			for (int x = 0; x < bitmap.Width; x++)
				for (int y = 0; y < bitmap.Height; y++) {
					Color color = bitmap.GetPixel(x, y);
					color = Color.FromArgb(opacity >= 0 && opacity <= 255 ? (int)(color.A * opacity) : 255, newColor);
					bitmap.SetPixel(x, y, color);
				}
			return bitmap;
		}
		protected override void DrawButton(ObjectInfoArgs e) {
		}
		protected override void DrawKindImage(EditorButtonObjectInfoArgs e, Rectangle rect) {
			if (e.IsImageExists) {
				Rectangle r = new Rectangle(Point.Empty, e.ImageSize);
				Bitmap image = new Bitmap(e.ActualImage);
				ObjectState state = e.State;
				if (e.Button.Tag != null && e.Button.Tag.Equals("Disabled"))
					state = ObjectState.Disabled;
				float opacity = GetImageOpacity(state);
				AppearanceDefault appearance = new AppearanceDefault(SystemColors.HighlightText, SystemColors.Highlight);
				Color color = CommonSkins.GetSkin(Provider).TranslateColor(appearance.ForeColor);
				image = ColorBitmap(image, opacity, color);
				if (e.Button.Image != null)
					e.Cache.Paint.DrawImage(e.Graphics, image, rect, r, e.State != ObjectState.Disabled);
			}
		}
	}
}
