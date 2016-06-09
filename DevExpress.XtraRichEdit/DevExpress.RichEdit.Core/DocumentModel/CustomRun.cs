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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model.History;
using DevExpress.Compatibility.System.Drawing;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Model {
	#region ICustomRunObject
	public interface ICustomRunObject : ICloneable<ICustomRunObject> {
		void Export(PieceTable pieceTable, DevExpress.Office.Drawing.Painter painter, Rectangle bounds);
#if !DXPORTABLE
		DevExpress.XtraPrinting.VisualBrick GetVisualBrick(PieceTable pieceTable, Rectangle bounds);
#endif
		OfficeImage ExportToImage(PieceTable pieceTable, Rectangle bounds);
		void Measure(BoxInfo boxInfo, IObjectMeasurer measurer, CustomRun run, DocumentModelUnitToLayoutUnitConverter unitConverter);
		string GetText();
		event CustomRunPropertiesChangedEventHandler PropertiesChanged;
	}
#endregion
#region CustomRunPropertiesChangedEventHandler
	public delegate void CustomRunPropertiesChangedEventHandler(object sender, CustomRunPropertiesChangedEventArgs e);
#endregion
#region CustomRunPropertiesChangedEventArgs
	public class CustomRunPropertiesChangedEventArgs : EventArgs {
		public ICustomRunPropertiesChangedInfo ChangedInfo { get; set; }
	}
#endregion
#region ICustomRunPropertiesChangedInfo
	public interface ICustomRunPropertiesChangedInfo {
		void Undo(ICustomRunObject customRunObject);
		void Redo(ICustomRunObject customRunObject);
	}
#endregion
#region CustomRun
	public class CustomRun : TextRunBase, IInlineObjectRun, IDisposable {
		ICustomRunObject customRunObject;
		public CustomRun(Paragraph paragraph)
			: base(paragraph) {
		}
		public CustomRun(Paragraph paragraph, int startIndex, int length)
			: base(paragraph, startIndex, length) {
		}
		public ICustomRunObject CustomRunObject {
			get {
				return customRunObject;
			}
			set {
				if (Object.ReferenceEquals(CustomRunObject, value))
					return;
				UnsubscribeCustomObjectEvents();
				customRunObject = value;
				SubscribeCustomObjectEvents();
			}
		}
		public override bool CanJoinWith(TextRunBase nextRun) {
			return false;
		}
		protected virtual void SubscribeCustomObjectEvents() {
			if (CustomRunObject == null)
				return;
			CustomRunObject.PropertiesChanged += new CustomRunPropertiesChangedEventHandler(OnCustomObjectPropertiesChanged);
		}
		protected virtual void UnsubscribeCustomObjectEvents() {
			if (CustomRunObject == null)
				return;
			CustomRunObject.PropertiesChanged -= new CustomRunPropertiesChangedEventHandler(OnCustomObjectPropertiesChanged);
		}
		void OnCustomObjectPropertiesChanged(object sender, CustomRunPropertiesChangedEventArgs e) {
			RunIndex thisRunIndex = PieceTable.CalculateRunIndex(this);
			if (e.ChangedInfo != null) {
				CustomRunPropertiesChangedHistoryItem historyItem = new CustomRunPropertiesChangedHistoryItem(PieceTable, thisRunIndex, e.ChangedInfo);
				PieceTable.DocumentModel.History.Add(historyItem);
			}
			PieceTable.ApplyChangesCore(InlineObjectChangeActionsCalculator.CalculateChangeActions(InlineObjectChangeType.BatchUpdate), thisRunIndex, thisRunIndex);
		}
		public override void Export(IDocumentModelExporter exporter) {
			exporter.Export(this);
		}
		public override bool CanPlaceCaretBefore {
			get { return true; }
		}
		protected internal override IRectangularObject GetRectangularObject() {
			return CustomRunObject as IRectangularObject;
		}
#region IInlineObjectRun Members
		public virtual Size MeasureRun(IObjectMeasurer measurer) {
			BoxInfo boxInfo = new BoxInfo();
			Measure(boxInfo, measurer);
			return boxInfo.Size;
		}
		public Box CreateBox() {
			return new CustomRunBox();
		}
#endregion
		protected internal override void Measure(BoxInfo boxInfo, IObjectMeasurer measurer) {
			CustomRunObject.Measure(boxInfo, measurer, this, DocumentModel.ToDocumentLayoutUnitConverter);
		}
		protected internal override bool TryAdjustEndPositionToFit(BoxInfo boxInfo, int maxWidth, IObjectMeasurer measurer) {
			return false;
		}
		public override TextRunBase Copy(DocumentModelCopyManager copyManager) {
			PieceTable pieceTable = copyManager.TargetPieceTable;
			DocumentModelPosition targetPosition = copyManager.TargetPosition;
			pieceTable.InsertCustomRun(targetPosition.LogPosition, CustomRunObject.Clone(), false);
			TextRunBase resultRun = pieceTable.Runs[targetPosition.RunIndex];
			resultRun.CopyCharacterPropertiesFrom(copyManager, this);
			return resultRun;
		}
#region IDisposable
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~CustomRun() {
			Dispose(false);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (customRunObject != null) {
					UnsubscribeCustomObjectEvents();
					customRunObject = null;
				}
			}
		}
#endregion
	}
#endregion
}
namespace DevExpress.XtraRichEdit.Model.History {
#region CustomRunPropertiesChangedHistoryItem
	public class CustomRunPropertiesChangedHistoryItem : RichEditHistoryItem {
#region Fields
		RunIndex index;
		ICustomRunPropertiesChangedInfo info;
#endregion
		public CustomRunPropertiesChangedHistoryItem(PieceTable pieceTable, RunIndex index, ICustomRunPropertiesChangedInfo info)
			: base(pieceTable) {
			Guard.ArgumentNotNull(info, "info");
			Debug.Assert(index >= RunIndex.Zero);
			this.index = index;
			this.info = info;
		}
#region Properties
		public RunIndex Index { get { return index; } }
#endregion
		protected override void UndoCore() {
			CustomRun run = PieceTable.Runs[Index] as CustomRun;
#if DEBUG || DEBUGTEST
			Debug.Assert(run != null);
#endif
			if (run.CustomRunObject != null)
				this.info.Undo(run.CustomRunObject);
		}
		protected override void RedoCore() {
			CustomRun run = PieceTable.Runs[Index] as CustomRun;
#if DEBUG || DEBUGTEST
			Debug.Assert(run != null);
#endif
			if (run.CustomRunObject != null)
				this.info.Redo(run.CustomRunObject);
		}
	}
#endregion
}
