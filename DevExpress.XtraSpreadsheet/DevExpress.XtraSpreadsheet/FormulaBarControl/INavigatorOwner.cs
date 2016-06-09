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
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.Office.Internal;
using DevExpress.XtraSpreadsheet;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraEditors.NavigatorButtons;
using System.ComponentModel;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraEditors.ViewInfo;
#if !SL
using System.Drawing;
using Imaging = System.Drawing.Imaging;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Localization;
using System.Reflection;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
#else
using System.Windows.Media;
using Imaging = System.Windows.Media.Imaging;
#endif
namespace DevExpress.XtraSpreadsheet {
	public partial class SpreadsheetFormulaBarControl {
		public bool IsResourceNavigatorActionEnabled(NavigatorButtonType type) {
			switch (type) {
				case NavigatorButtonType.CancelEdit:
					return buttons.Buttons.CancelEdit.Enabled;
				case NavigatorButtonType.EndEdit:
					return buttons.Buttons.EndEdit.Enabled;
				case NavigatorButtonType.Custom:
					return buttons.Buttons.Function.Enabled;
				default:
					return false;
			}
		}
		public bool ExecuteResourceNavigatorAction(NavigatorButtonType type) {
			ExecuteResourceNavigatorActionCore(type);
			return true;
		}
		void ExecuteResourceNavigatorActionCore(NavigatorButtonType type) {
			switch (type) {
				case NavigatorButtonType.CancelEdit:
					RaiseCancelButtonClick();
					break;
				case NavigatorButtonType.EndEdit:
					RaiseOkButtonClick();
					break;
				case NavigatorButtonType.Custom:
					RaiseInsertFunctionButtonClick();
					break;
			}
		}
	}
	#region FormulaBarControlButtons
	[TypeConverter("System.ComponentModel.ExpandableObjectConverter, System")]
	public class FormulaBarControlButtons : NavigatorButtonsBase {
		List<string> imagePaths;
		public FormulaBarControlButtons(INavigatorOwner navigator)
			: base(navigator) {
			this.imagePaths = CreateImagePaths();
		}
		#region Properties
		[Browsable(false)]
		public INavigatableControl Control {
			get {
				if (Owner is IGetNavigatableControl)
					return ((IGetNavigatableControl)Owner).Control;
				else
					return null;
			}
		}
		protected virtual bool ShouldSerializeEndEdit() { return EndEdit.ShouldSerialize; }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual NavigatorButton EndEdit {
			get {
				NavigatorButton button = ButtonByButtonType(NavigatorButtonType.EndEdit);
				button.Hint = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Tooltip_FormulaBarOkButton);
				return button;
			}
		}
		protected virtual bool ShouldSerializeCancelEdit() { return CancelEdit.ShouldSerialize; }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual NavigatorButton CancelEdit {
			get {
				NavigatorButton button = ButtonByButtonType(NavigatorButtonType.CancelEdit);
				button.Hint = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Tooltip_FormulaBarCancelButton);
				return button;
			}
		}
		protected virtual bool ShouldSerializeFunction() { return Function.ShouldSerialize; }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual NavigatorButton Function {
			get {
				NavigatorButton button = ButtonByButtonType(NavigatorButtonType.Custom);
				button.Hint = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Tooltip_FormulaBarFunctionButton);
				return button;
			}
		}
		#endregion
		protected override NavigatorButtonCollectionBase CreateNavigatorButtonCollection() {
			return new FormulaBarButtonCollection(this);
		}
		protected override NavigatorButtonsViewInfo CreateViewInfo() {
			return new FormulaBarButtonsViewInfo(this);
		}
		List<string> CreateImagePaths() {
			List<string> result = new List<string>();
			result.Add("FormulaBarCancel_16x16.png");
			result.Add("FormulaBarOk_16x16.png");
			result.Add("FormulaBarInsertFunction_16x16.png");
			return result;
		}
		protected override object GetDefaultImages() {
			object skinImages = base.GetDefaultImages();
			ImageCollection imageCollection = skinImages as ImageCollection;
			if (imageCollection != null)
				AddFormulaBarDefaultImages(imageCollection);
			return skinImages;
		}
		void AddFormulaBarDefaultImages(ImageCollection imageCollection) {
			Color pixel55 = ((Bitmap)imageCollection.Images[0]).GetPixel(5, 5); 
			Imaging.ImageAttributes imageAttributes = DevExpress.Utils.Drawing.ImageColorizer.GetColoredAttributes(pixel55);
			for (int i = 0; i < imagePaths.Count; i++) {
				Bitmap coloredButtonImage = SkinImageDecorator.GetColoredButtonImage(imageAttributes, imagePaths[i], GetType().Assembly);
				imageCollection.AddImage(coloredButtonImage);
			}
		}
	}
	#endregion
	#region FormulaBarButtonsViewInfo
	public class FormulaBarButtonsViewInfo : NavigatorButtonsViewInfo {
		public FormulaBarButtonsViewInfo(NavigatorButtonsBase buttons)
			: base(buttons) {
		}
		protected override NavigatorButtonViewInfo CreateButtonViewInfo(NavigatorButtonBase button) {
			return new FormulaBarNavigatorButtonViewInfo(this, button);
		}
	}
	#endregion
	#region FormulaBarNavigatorButtonViewInfo
	public class FormulaBarNavigatorButtonViewInfo : NavigatorButtonViewInfo {
		public FormulaBarNavigatorButtonViewInfo(NavigatorButtonsViewInfo viewInfo, NavigatorButtonBase button)
			: base(viewInfo, button) {
		}
		public override Size MinSize { get { return new Size(0, 0); } }
	}
	#endregion
	#region FormulaBarButtonCollection
	public class FormulaBarButtonCollection : NavigatorButtonCollectionBase {
		public FormulaBarButtonCollection(FormulaBarControlButtons buttons)
			: base(buttons) {
		}
		protected override void CreateButtons(NavigatorButtonsBase buttons) {
			AddButton(new ControlCancelEditButtonHelper(buttons));
			AddButton(new ControlOkEditButtonHelper(buttons));
			AddButton(new ControlFunctionButtonHelper(buttons));
		}
	}
	#endregion
	#region ControlButtonHelperBase
	public abstract class ControlButtonHelperBase : NavigatorButtonHelper {
		protected ControlButtonHelperBase(NavigatorButtonsBase buttons) : base(buttons) { }
		protected FormulaBarControlButtons ControlButtons { get { return Buttons as FormulaBarControlButtons; } }
		public INavigatableControl Control { get { return ControlButtons != null ? ControlButtons.Control : null; } }
		public override void DoClick() {
			if (Enabled)
				DoDataClick();
		}
		protected virtual void DoDataClick() {
			Control.DoAction(ButtonType);
		}
		public override bool Enabled { get { return (Control != null) && Control.IsActionEnabled(ButtonType); } }
	}
	#endregion
	#region ControlCancelEditButtonHelper
	public class ControlCancelEditButtonHelper : ControlButtonHelperBase {
		public ControlCancelEditButtonHelper(NavigatorButtonsBase buttons) : base(buttons) { }
		public override NavigatorButtonType ButtonType { get { return NavigatorButtonType.CancelEdit; } }
		public override int DefaultIndex {
			get {
				return 22;
			}
		}
	}
	#endregion
	#region ControlOkEditButtonHelper
	public class ControlOkEditButtonHelper : ControlButtonHelperBase {
		public ControlOkEditButtonHelper(NavigatorButtonsBase buttons) : base(buttons) { }
		public override NavigatorButtonType ButtonType { get { return NavigatorButtonType.EndEdit; } }
		public override int DefaultIndex {
			get {
				return 20;
			}
		}
	}
	#endregion
	#region ControlNavigatorFunctionButtonHelper
	public class ControlFunctionButtonHelper : ControlButtonHelperBase {
		public ControlFunctionButtonHelper(NavigatorButtonsBase buttons) : base(buttons) { }
		public override NavigatorButtonType ButtonType { get { return NavigatorButtonType.Custom; } }
		public override int DefaultIndex {
			get {
				return 21;
			}
		}
	}
	#endregion
	#region ResourceNavigator
	public class ResourceNavigator : INavigatableControl, IDisposable {
		SpreadsheetFormulaBarControl formulaBarControl;
		public ResourceNavigator(SpreadsheetFormulaBarControl formulaBarControl) {
			Guard.ArgumentNotNull(formulaBarControl, "formulaBarControl");
			this.formulaBarControl = formulaBarControl;
		}
		#region INavigatableControl implementation
		void INavigatableControl.AddNavigator(INavigatorOwner owner) {
		}
		void INavigatableControl.RemoveNavigator(INavigatorOwner owner) {
		}
		int INavigatableControl.RecordCount { get { return 0; } }
		int INavigatableControl.Position { get { return 0; } }
		bool INavigatableControl.IsActionEnabled(NavigatorButtonType type) {
			return formulaBarControl.IsResourceNavigatorActionEnabled(type);
		}
		void INavigatableControl.DoAction(NavigatorButtonType type) {
			formulaBarControl.ExecuteResourceNavigatorAction(type);
		}
		#endregion
		public void Dispose() {
			formulaBarControl = null;
		}
	}
	#endregion
	#region IGetNavigatableControl
	interface IGetNavigatableControl {
		INavigatableControl Control { get; }
	}
	#endregion
	public static class SkinImageDecorator {
		public static Bitmap GetColoredButtonImage(Imaging.ImageAttributes imageAttributes, string imageName, Assembly assembly) {
			Bitmap buttonImage = (Bitmap)ResourceImageHelper.CreateImageFromResources("DevExpress.XtraSpreadsheet.Images." + imageName, assembly);
			return GetColoredImage(imageAttributes, buttonImage);
		}
		public static Bitmap GetColoredImage(Imaging.ImageAttributes imageAttributes, Image image) {
			int width = image.Width;
			int height = image.Height;
			Bitmap result = new Bitmap(width, height);
			using (Graphics graphics = Graphics.FromImage(result)) {
				graphics.DrawImage(image, new Rectangle(0, 0, width, height), 0, 0, width, height, GraphicsUnit.Pixel, imageAttributes);
			}
			return result;
		}
		public static Bitmap CreateImageColoredToButtonForeColor(Image image, UserLookAndFeel lookAndFeel) {
			Skin currentSkin = EditorsSkins.GetSkin(lookAndFeel);
			SkinElement element = currentSkin[EditorsSkins.SkinEditorButton];
			Color color = element.Color.ForeColor;
			if (color.IsSystemColor)
				color = element.Color.Owner.GetSystemColor(color);
			Imaging.ImageAttributes imageAttributes = DevExpress.Utils.Drawing.ImageColorizer.GetColoredAttributes(color);
			return GetColoredImage(imageAttributes, image);
		}
	}
}
