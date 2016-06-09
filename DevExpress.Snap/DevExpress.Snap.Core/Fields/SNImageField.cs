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
using System.IO;
using System.Security;
using DevExpress.Data.Utils;
using DevExpress.Utils.Design;
using DevExpress.Office.Utils;
using DevExpress.XtraPrinting;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Fields;
namespace DevExpress.Snap.Core.Fields {
	[TypeConverter(typeof(EnumTypeConverter)), ResourceFinder(typeof(Data.ResFinder), DXDisplayNameAttribute.DefaultResourceFile)]
	public enum UpdateMergeImageFieldMode {
		KeepSize,
		KeepScale
	}
	[ActionList("DevExpress.Snap.Extensions.Native.ActionLists.ContentTypeActionList," + AssemblyInfo.SRAssemblySnapExtensions, 0)]
	[ActionList("DevExpress.Snap.Extensions.Native.ActionLists.DataFieldNameActionList," + AssemblyInfo.SRAssemblySnapExtensions, 1)]
	[ActionList("DevExpress.Snap.Extensions.Native.ActionLists.SNImageActionList," + AssemblyInfo.SRAssemblySnapExtensions, 2)]
	public class SNImageField : SNMergeFieldSupportsEmptyFieldDataAlias {
		ImageSizeMode sizing = ImageSizeMode.StretchImage;
		UpdateMergeImageFieldMode updateMode = UpdateMergeImageFieldMode.KeepScale;
		bool suppressStoreGraphicsDataWithDocument;
		#region static
		public static new readonly string FieldType = "SNIMAGE";
		public static readonly string ImageSizeModeSwitch = "s";
		public static readonly string UpdateModeSwitch = "k";
		public static readonly string ScaleXSwitch = "sx";
		public static readonly string ScaleYSwitch = "sy";
		public static readonly string HeightSwitch = "h";
		public static readonly string WidthSwitch = "w";
		static readonly Dictionary<string, bool> mergeImageSwitchesWithArgument;
		static readonly Dictionary<string, ImageSizeMode> imageSizeModes = CreateImageSizeModes();
		static readonly Dictionary<string, UpdateMergeImageFieldMode> updateFieldModes = CreateUpdateMergeImageFieldModes();
		static readonly Dictionary<ImageSizeMode, string> mapImageSizeModeToString = CreateInverseMappingDictionary(imageSizeModes);
		static readonly Dictionary<UpdateMergeImageFieldMode, string> mapUpdateModeToString = CreateInverseMappingDictionary(updateFieldModes);
		internal static OfficeImage ImagePlaceHolder;
		[SecuritySafeCritical]
		static SNImageField() {
			Stream stream = typeof(MergefieldField).Assembly.GetManifestResourceStream("DevExpress.XtraRichEdit.Images.ImagePlaceHolder.png");
			ImagePlaceHolder = OfficeImage.CreateImage(stream);
			mergeImageSwitchesWithArgument = CreateSwitchesWithArgument(ImageSizeModeSwitch, UpdateModeSwitch, ScaleXSwitch, ScaleYSwitch, HeightSwitch, WidthSwitch,EmptyFieldDataAliasSwitch);
			foreach (KeyValuePair<string, bool> sw in MergefieldField.SwitchesWithArgument)
				mergeImageSwitchesWithArgument.Add(sw.Key, sw.Value);
		}
		static Dictionary<string, ImageSizeMode> CreateImageSizeModes() {
			Dictionary<string, ImageSizeMode> result = new Dictionary<string, ImageSizeMode>();
			result.Add("autosize", ImageSizeMode.AutoSize);
			result.Add("a", ImageSizeMode.AutoSize);
			result.Add("centerimage", ImageSizeMode.CenterImage);
			result.Add("c", ImageSizeMode.CenterImage);
			result.Add("n", ImageSizeMode.Normal);
			result.Add("normal", ImageSizeMode.Normal);
			result.Add("squeeze", ImageSizeMode.Squeeze);
			result.Add("sq", ImageSizeMode.Squeeze);
			result.Add("stretchimage", ImageSizeMode.StretchImage);
			result.Add("st", ImageSizeMode.StretchImage);
			result.Add("zoomimage", ImageSizeMode.ZoomImage);
			result.Add("z", ImageSizeMode.ZoomImage);
			result.Add("tile", ImageSizeMode.Tile);
			return result;
		}
		static Dictionary<string, UpdateMergeImageFieldMode> CreateUpdateMergeImageFieldModes() {
			Dictionary<string, UpdateMergeImageFieldMode> result = new Dictionary<string, UpdateMergeImageFieldMode>();
			result.Add("scale", UpdateMergeImageFieldMode.KeepScale);
			result.Add("size", UpdateMergeImageFieldMode.KeepSize);
			return result;
		}
		protected static Dictionary<U, T> CreateInverseMappingDictionary<T,U>(Dictionary<T,U> mapping) {
			Dictionary<U, T> result = new Dictionary<U,T>();
			foreach (KeyValuePair<T, U> pair in mapping) {
				if(!result.ContainsKey(pair.Value))
					result.Add(pair.Value, pair.Key);				
			}
			return result;
		}
		public static new CalculatedFieldBase Create() {
			return new SNImageField();
		}
		public static string GetImageSizeModeString(ImageSizeMode sizeMode) {
			return mapImageSizeModeToString[sizeMode];
		}
		public static string GetUpdateModeString(UpdateMergeImageFieldMode updateMode) {
			return mapUpdateModeToString[updateMode];
		}
		#endregion
		public ImageSizeMode Sizing { get { return sizing; } }
		public UpdateMergeImageFieldMode UpdateMode { get { return updateMode; } }
		protected override Dictionary<string, bool> SwitchesWithArguments { get { return mergeImageSwitchesWithArgument; } }
		protected override string FieldTypeName { get { return FieldType; } }
		public override void Initialize(PieceTable pieceTable, InstructionCollection switches) {
			base.Initialize(pieceTable, switches);
			suppressStoreGraphicsDataWithDocument = switches.GetBool("d");
			SetSizeMode(switches);
			SetUpdateMode(switches);
		}
		void SetSizeMode(InstructionCollection switches) {
			string sizingString = switches.GetString(ImageSizeModeSwitch);
			if (String.IsNullOrEmpty(sizingString))
				return;
			ImageSizeMode sizeMode;
			if (imageSizeModes.TryGetValue(sizingString.ToLower(), out sizeMode))
				this.sizing = sizeMode;
		}
		void SetUpdateMode(InstructionCollection switches) {
			string updateString = switches.GetString(UpdateModeSwitch);
			if (String.IsNullOrEmpty(updateString))
				return;
			UpdateMergeImageFieldMode updateMergeImageFieldMode;
			if (updateFieldModes.TryGetValue(updateString, out updateMergeImageFieldMode))
				this.updateMode = updateMergeImageFieldMode;
		}
		protected override bool SholdApplyFormating {
			get { return false; }
		}
		public override CalculatedFieldValue GetCalculatedValueCore(PieceTable sourcePieceTable, MailMergeDataMode mailMergeDataMode, Field documentField) {
			CalculatedFieldValue result = base.GetCalculatedValueCore(sourcePieceTable, mailMergeDataMode, documentField);
			if (result == CalculatedFieldValue.Null) {
				return new CalculatedFieldValue(EmptyFieldDataAlias);
			}
			OfficeImage image = result.RawValue != FieldNull.Value ? TryCreateImage(result) : ImagePlaceHolder;
			bool suppressStore = suppressStoreGraphicsDataWithDocument && mailMergeDataMode != MailMergeDataMode.FinalMerging;
			DocumentModel targetModel = sourcePieceTable.DocumentModel.GetFieldResultModel();
			InsertInlinePictureInTargetModel(image, targetModel, suppressStore);
			return new CalculatedFieldValue(targetModel);
		}
		protected internal DocumentModel InsertInlinePictureInTargetModel(OfficeImage image, DocumentModel targetModel, bool suppressStore) {
			targetModel.MainPieceTable.InsertInlinePicture(DocumentLogPosition.Zero, image);
			InlinePictureRun run = (InlinePictureRun)targetModel.MainPieceTable.Runs[new RunIndex(0)];
			run.Sizing = Sizing;
			UpdateRunSize(run);
			return targetModel;
		}
		void UpdateRunSize(InlinePictureRun run) {
			if (Sizing == ImageSizeMode.AutoSize) {
				run.ScaleX = 100.0f;
				run.ScaleY = 100.0f;
				return;
			}
			if (UpdateMode == UpdateMergeImageFieldMode.KeepScale) {
				float scaleX = Switches.GetInt(ScaleXSwitch) / 100.0f;
				if (scaleX != 0.0)
					run.ScaleX = scaleX;
				float scaleY = Switches.GetInt(ScaleYSwitch) / 100.0f;
				if (scaleY != 0.0)
					run.ScaleY = scaleY;
			}
			else {
				int width = Switches.GetInt(WidthSwitch);
				if (width != 0)
					run.ScaleX = (width / (float)run.ActualSize.Width) * 100;
				int height = Switches.GetInt(HeightSwitch);
				if (height != 0)
					run.ScaleY = (height / (float)run.ActualSize.Height) * 100;
			}
		}
		public virtual OfficeImage TryCreateImage(CalculatedFieldValue value) {
			if(value.RawValue is Image) 
				return OfficeImage.CreateImage((Image)value.RawValue);
			byte[] imageBytes = value.RawValue as byte[];
			if (imageBytes == null)
				return null;
			Image nativeImage = new ImageTool().FromArray(imageBytes);
			return OfficeImage.CreateImage(nativeImage);
		}
		protected internal override string[] GetNativeSwithes() {
			return new string[] {
				ImageSizeModeSwitch,
				UpdateModeSwitch,
				ScaleXSwitch,
				ScaleYSwitch,
				HeightSwitch,
				WidthSwitch,
				EmptyFieldDataAliasSwitch,
				EnableEmptyFieldDataAliasSwitch
			};
		}
		protected internal override string[] GetInvariableSwitches() {
			return new string[] {
				EmptyFieldDataAliasSwitch,
				EnableEmptyFieldDataAliasSwitch
			};
		}
		public override bool CanResize { get { return Sizing != ImageSizeMode.AutoSize; } }
	}
	public class SNImageFieldController : SizeAndScaleFieldController<SNImageField> {
		public SNImageFieldController(InstructionController controller)
			: base(controller, GetRectangularObject(controller)) {
		}
		static IRectangularObject GetRectangularObject(InstructionController controller) {
			return controller.PieceTable.Runs[controller.Field.Result.Start] as IRectangularObject;
		}
		public void SetImageSizeInfo(UpdateMergeImageFieldMode mode) {
			Controller.SuppressFieldsUpdateAfterUpdateInstruction = true;
			SetImageSizeInfoCore(mode);
			Controller.ApplyDeferredActions();
		}
		protected override void SetImageSizeInfoCore() {
			SetImageSizeInfoCore(GetField().UpdateMode);
		}
		void SetImageSizeInfoCore(UpdateMergeImageFieldMode mode) {
			if (mode == UpdateMergeImageFieldMode.KeepScale) {
				SetScale();
				ClearSize();
			}
			else {
				SetSize();
				ClearScale();
			}
		}
	}
}
