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
using DevExpress.XtraPrinting.Native;
using System.IO;
using DevExpress.Utils.Serializing;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
namespace DevExpress.XtraPrinting.Native {
	public class DeserializedDocument : PSLinkDocument {
		static void AfterBuilderPages() {
			ExceptionHelper.ThrowInvalidOperationException();
		}
		bool deserializing;
		ContinuousExportInfo continuousExportInfo = ContinuousExportInfo.Empty;
		internal override bool CanPerformContinuousExport { get { return !continuousExportInfo.IsEmpty; } }
		public override bool IsEmpty { get { return base.IsEmpty || deserializing; } }
		public DeserializedDocument(PrintingSystemBase ps)
			: base(ps, AfterBuilderPages) {
		}
		protected override void OnEndSerializingCore() {
		}
		protected internal override ContinuousExportInfo GetContinuousExportInfo() {
			return continuousExportInfo;
		}
		internal override ContinuousExportInfo GetDeserializationContinuousExportInfo() {
			continuousExportInfo = base.GetDeserializationContinuousExportInfo();
			return continuousExportInfo;
		}
		protected override object GetObjectByIndexCore(string propertyName, int index) {
			switch(propertyName) {
				case PrintingSystemSerializationNames.Brick:
				case PrintingSystemSerializationNames.InnerBricks:
				case PrintingSystemSerializationNames.SharedBricks:
					return BricksSerializationCache.GetObjectByIndex(index);
				case PrintingSystemSerializationNames.ImageEntry:
					return ImagesSerializationCache.GetObjectByIndex(index);
				case PrintingSystemSerializationNames.Style:
					return StylesSerializationCache.GetObjectByIndex(index);
			}
			return base.GetObjectByIndexCore(propertyName, index);
		}
		protected override void OnStartDeserializingCore() {
			CreateSerializationObjects();
			CanChangePageSettings = false;
			deserializing = true;
		}
		protected override void OnEndDeserializingCore() {
			if(Pages.Count > 0) {
				((System.ComponentModel.ISupportInitialize)PrintingSystem).BeginInit();
				try {
					PrintingSystem.PageSettings.Assign(new PageData(Pages[0].PageData));
				} finally {
					((System.ComponentModel.ISupportInitialize)PrintingSystem).EndInit();
				}
			}
			PrintingSystem.Graph.PageUnit = GraphicsUnit.Document;
			foreach(Brick brick in bricks.Collection) {
				brick.Initialize(ps, brick.Rect, false);
			}
			NullDeserializationCaches();
			ProgressReflector.MaximizeRange();
			deserializing = false;
#if !SL
			PrintingSystem.SetCommandVisibility(PSCommandHelper.ExportCommands, CommandVisibility.All, Priority.Low);
			PrintingSystem.SetCommandVisibility(PSCommandHelper.SendCommands, CommandVisibility.All, Priority.Low);
#endif
			OnContentChanged();
		}
		protected virtual void NullDeserializationCaches() {
			NullCaches();
		}
		internal override void ClearContent() {
			Dispose();
		}
	}
	public class VirtualDocument : DeserializedDocument {
		protected Stream independentPagesStream;
		bool headerLoaded;
		bool disposeStream;
		bool forced;
		public VirtualDocument(PrintingSystemBase ps, Stream independentPagesStream, bool disposeStream) 
			: base(ps) {
			this.independentPagesStream = independentPagesStream;
			DocumentDeserializationCollection collection = new DocumentDeserializationCollection(this, index => false);
			collection.Add(new DocumentSerializationOptions(this));
			collection.Add(XtraSkipObjectInfo.SkipObjectInfoInstance);
			DeserializePart(collection);
			headerLoaded = true;
			this.disposeStream = disposeStream;
		}
		internal override void Serialize(Stream stream, XtraSerializer serializer) {
			ExceptionHelper.ThrowInvalidOperationException();
		}
		protected internal override void LoadPage(int pageIndex) {
			DocumentSerializationCollection collection = new DocumentSerializationCollection();
			collection.Add(XtraSkipObjectInfo.SkipObjectInfoInstance);
			collection.AddRange(Pages, index => index.Equals(pageIndex) && !Pages[index].Loaded); 
			collection.Add(XtraSkipObjectInfo.SkipObjectInfoInstance);
			DeserializePart(collection);
		}
		protected internal override void ForceLoad() {
			if(forced)
				return;
			DocumentSerializationCollection collection = new DocumentSerializationCollection();
			collection.Add(XtraSkipObjectInfo.SkipObjectInfoInstance);
			collection.AddRange(Pages, index => !Pages[index].Loaded);
			collection.Add(GetDeserializationContinuousExportInfo());
			DeserializePart(collection);
			forced = true;
		}
		void DeserializePart(DocumentSerializationCollection collection) {
			lock(independentPagesStream) {
				DeserializeCore(independentPagesStream, PrintingSystemBase.CreateIndependentPagesSerializer(), collection); 
			}
		}
		protected override void NullDeserializationCaches() {
		}
		protected override void CreateSerializationObjects() {
			if(!headerLoaded)
				base.CreateSerializationObjects();
		}
		public override void Dispose() {
			if(disposeStream && independentPagesStream != null) {
				independentPagesStream.Dispose();
			}
			independentPagesStream = null;
			base.Dispose();
		}
	}
}
