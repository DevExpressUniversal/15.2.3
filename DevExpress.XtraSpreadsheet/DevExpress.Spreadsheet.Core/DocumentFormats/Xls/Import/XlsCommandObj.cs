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
using System.IO;
using System.Text;
using System.Reflection;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Export.Xls;
using DevExpress.Export.Xl;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsObjType
	public enum XlsObjType {
		Undefined = -1,
		Group = 0x0000,
		Line = 0x0001,
		Rectangle = 0x0002,
		Oval = 0x0003,
		Arc = 0x0004,
		Chart = 0x0005,
		Text = 0x0006,
		Button = 0x0007,
		Picture = 0x0008,
		Polygon = 0x0009,
		CheckBox = 0x000b,
		RadioButton = 0x000c,
		EditBox = 0x000d,
		Label = 0x000e,
		DialogBox = 0x000f,
		SpinControl = 0x0010,
		ScrollBar = 0x0011,
		List = 0x0012,
		GroupBox = 0x0013,
		DropdownList = 0x0014,
		Note = 0x0019,
		OfficeArtObject = 0x001e
	}
	#endregion
	#region IXlsObjData
	public interface IXlsObjData {
		XlsObjType ObjType { get; }
		int ObjId { get; }
		bool IsActiveX { get; }
		bool InControlStream { get; }
		XlsContentBuilder ContentBuilder { get; }
	}
	#endregion
	#region IXlsObjRecord
	public interface IXlsObjRecord {
		int Size { get; }
		void Read(BinaryReader reader, IXlsObjData owner);
		void Write(BinaryWriter writer, IXlsObjData owner);
		void Execute(XlsContentBuilder contentBuilder);
		int GetSize(IXlsObjData owner);
	}
	#endregion
	#region XlsObjBase
	public abstract class XlsObjBase : IXlsObjRecord {
		const int headerSize = 4;
		#region Properties
		public int Size { get; private set; }
		#endregion
		public virtual void Read(BinaryReader reader, IXlsObjData owner) {
			Size = reader.ReadUInt16();
			ReadCore(reader, owner);
		}
		public virtual void Write(BinaryWriter writer, IXlsObjData owner) {
			writer.Write(XlsObjFactory.GetTypeCodeByType(GetType()));
			writer.Write((ushort)(GetSize(owner) - headerSize));
			WriteCore(writer, owner);
		}
		public virtual void Execute(XlsContentBuilder contentBuilder) {
		}
		public virtual int GetSize(IXlsObjData owner) {
			return headerSize;
		}
		protected virtual void ReadCore(BinaryReader reader, IXlsObjData owner) {
		}
		protected virtual void WriteCore(BinaryWriter writer, IXlsObjData owner) {
		}
	}
	#endregion
	#region XlsObjFactory
	public static class XlsObjFactory {
		#region XlsObjInfo
		internal class XlsObjRecordInfo {
			short typeCode;
			Type recordType;
			public XlsObjRecordInfo(short typeCode, Type recordType) {
				this.typeCode = typeCode;
				this.recordType = recordType;
			}
			public short TypeCode { get { return this.typeCode; } }
			public Type RecordType { get { return this.recordType; } }
		}
		#endregion
		static List<XlsObjRecordInfo> infos;
		static Dictionary<short, ConstructorInfo> recordTypes;
		static Dictionary<Type, short> typeCodes;
		static XlsObjFactory() {
			infos = new List<XlsObjRecordInfo>();
			recordTypes = new Dictionary<short, ConstructorInfo>();
			typeCodes = new Dictionary<Type, short>();
			infos.Add(new XlsObjRecordInfo(-1, typeof(XlsObjUnknown)));
			infos.Add(new XlsObjRecordInfo(0x0000, typeof(XlsObjEnd)));
			infos.Add(new XlsObjRecordInfo(0x0006, typeof(XlsObjGroupProp)));
			infos.Add(new XlsObjRecordInfo(0x0007, typeof(XlsObjPictureFormat)));
			infos.Add(new XlsObjRecordInfo(0x0008, typeof(XlsObjPictureFlags)));
			infos.Add(new XlsObjRecordInfo(0x0009, typeof(XlsObjPictureFormula)));
			infos.Add(new XlsObjRecordInfo(0x000a, typeof(XlsObjCheckbox)));
			infos.Add(new XlsObjRecordInfo(0x000b, typeof(XlsObjRadioButton)));
			infos.Add(new XlsObjRecordInfo(0x000c, typeof(XlsObjScrollable)));
			infos.Add(new XlsObjRecordInfo(0x000d, typeof(XlsObjNote)));
			infos.Add(new XlsObjRecordInfo(0x000f, typeof(XlsObjGroupboxData)));
			infos.Add(new XlsObjRecordInfo(0x0010, typeof(XlsObjEditboxData)));
			infos.Add(new XlsObjRecordInfo(0x0011, typeof(XlsObjRadioButtonData)));
			infos.Add(new XlsObjRecordInfo(0x0012, typeof(XlsObjCheckboxData)));
			infos.Add(new XlsObjRecordInfo(0x0013, typeof(XlsObjListData)));
			infos.Add(new XlsObjRecordInfo(0x0015, typeof(XlsObjCommon)));
			for (int i = 0; i < infos.Count; i++) {
				recordTypes.Add(infos[i].TypeCode, infos[i].RecordType.GetConstructor(new Type[] { }));
				typeCodes.Add(infos[i].RecordType, infos[i].TypeCode);
			}
		}
		public static short GetTypeCodeByType(Type recordType) {
			short typeCode;
			if (!typeCodes.TryGetValue(recordType, out typeCode))
				typeCode = 0x0000;
			return typeCode;
		}
		public static IXlsObjRecord CreateRecord(short typeCode) {
			if (!recordTypes.ContainsKey(typeCode))
				typeCode = -1;
			ConstructorInfo recordConstructor = recordTypes[typeCode];
			IXlsObjRecord recordInstance = recordConstructor.Invoke(new object[] { }) as IXlsObjRecord;
			return recordInstance;
		}
		public static IXlsObjRecord CreateRecord(BinaryReader reader) {
			short typeCode = reader.ReadInt16();
			if (!recordTypes.ContainsKey(typeCode))
				typeCode = -1;
			ConstructorInfo recordConstructor = recordTypes[typeCode];
			IXlsObjRecord recordInstance = recordConstructor.Invoke(new object[] { }) as IXlsObjRecord;
			return recordInstance;
		}
	}
	#endregion
	#region XlsObjUnknown
	public class XlsObjUnknown : XlsObjBase {
		protected override void ReadCore(BinaryReader reader, IXlsObjData owner) {
			reader.BaseStream.Seek(Size, SeekOrigin.Current);
		}
	}
	#endregion
	#region XlsObjEnd
	public class XlsObjEnd : XlsObjBase {
	}
	#endregion
	#region XlsObjCommon
	public class XlsObjCommon : XlsObjBase {
		int objId;
		#region Properties
		public XlsObjType ObjType { get; set; }
		public int ObjId {
			get { return objId; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "ObjId");
				objId = value;
			}
		}
		public bool Locked { get; set; }
		public bool DefaultSize { get; set; }
		public bool Published { get; set; }
		public bool Print { get; set; }
		public bool Disabled { get; set; }
		public bool UIObject { get; set; }
		public bool RecalcObject { get; set; }
		public bool AlwaysRecalcObject { get; set; }
		#endregion
		protected override void ReadCore(BinaryReader reader, IXlsObjData owner) {
			ObjType = (XlsObjType)reader.ReadUInt16();
			ObjId = reader.ReadUInt16();
			ushort bitwiseField = reader.ReadUInt16();
			Locked = Convert.ToBoolean(bitwiseField & 0x0001);
			DefaultSize = Convert.ToBoolean(bitwiseField & 0x0004);
			Published = Convert.ToBoolean(bitwiseField & 0x0008);
			Print = Convert.ToBoolean(bitwiseField & 0x0010);
			Disabled = Convert.ToBoolean(bitwiseField & 0x0080);
			UIObject = Convert.ToBoolean(bitwiseField & 0x0100);
			RecalcObject = Convert.ToBoolean(bitwiseField & 0x0200);
			AlwaysRecalcObject = Convert.ToBoolean(bitwiseField & 0x1000);
			reader.ReadUInt32();
			reader.ReadUInt32();
			reader.ReadUInt32();
		}
		protected override void WriteCore(BinaryWriter writer, IXlsObjData owner) {
			writer.Write((ushort)ObjType);
			writer.Write((ushort)ObjId);
			ushort bitwiseField = 0;
			if(Locked)
				bitwiseField |= 0x0001;
			if(DefaultSize)
				bitwiseField |= 0x0004;
			if(Published)
				bitwiseField |= 0x0008;
			if(Print)
				bitwiseField |= 0x0010;
			if(Disabled)
				bitwiseField |= 0x0080;
			if(UIObject)
				bitwiseField |= 0x0100;
			if(RecalcObject)
				bitwiseField |= 0x0200;
			if(AlwaysRecalcObject)
				bitwiseField |= 0x1000;
			writer.Write(bitwiseField);
			writer.Write((uint)0);
			writer.Write((uint)0);
			writer.Write((uint)0);
		}
		public override int GetSize(IXlsObjData owner) {
			return 22;
		}
	}
	#endregion
	#region XlsObjGroupProp
	public class XlsObjGroupProp : XlsObjBase {
		protected override void ReadCore(BinaryReader reader, IXlsObjData owner) {
			reader.ReadUInt16();
		}
		protected override void WriteCore(BinaryWriter writer, IXlsObjData owner) {
			writer.Write((ushort)0);
		}
		public override int GetSize(IXlsObjData owner) {
			return 6;
		}
	}
	#endregion
	#region XlsObjPictureFormat
	public enum XlsObjPictureType {
		Unspecified = 0xffff,
		EnhancedMetafile = 0x0002,
		Bitmap = 0x0009,
		Native = 0x000e
	}
	public class XlsObjPictureFormat : XlsObjBase {
		public XlsObjPictureType PictureType { get; set; }
		protected override void ReadCore(BinaryReader reader, IXlsObjData owner) {
			if (Size > 0)
				PictureType = (XlsObjPictureType)reader.ReadUInt16();
			else
				PictureType = XlsObjPictureType.Unspecified;
		}
		protected override void WriteCore(BinaryWriter writer, IXlsObjData owner) {
			writer.Write((ushort)PictureType);
		}
		public override int GetSize(IXlsObjData owner) {
			return 6;
		}
	}
	#endregion
	#region XlsObjPictureFlags
	public class XlsObjPictureFlags : XlsObjBase {
		#region Properties
		public bool IsAuto { get; set; }
		public bool IsDdeRef { get; set; }
		public bool PrintCalc { get; set; }
		public bool IconDisplay { get; set; }
		public bool IsActiveX { get; set; }
		public bool InControlStream { get; set; }
		public bool IsCameraPicture { get; set; }
		public bool DefaultSize { get; set; }
		public bool AutoLoad { get; set; }
		#endregion
		protected override void ReadCore(BinaryReader reader, IXlsObjData owner) {
			ushort bitwiseField = 0x0001;
			if (Size > 0)
				bitwiseField = reader.ReadUInt16();
			IsAuto = Convert.ToBoolean(bitwiseField & 0x0001);
			IsDdeRef = Convert.ToBoolean(bitwiseField & 0x0002);
			PrintCalc = Convert.ToBoolean(bitwiseField & 0x0004);
			IconDisplay = Convert.ToBoolean(bitwiseField & 0x0008);
			IsActiveX = Convert.ToBoolean(bitwiseField & 0x0010);
			InControlStream = Convert.ToBoolean(bitwiseField & 0x0020);
			IsCameraPicture = Convert.ToBoolean(bitwiseField & 0x0080);
			DefaultSize = Convert.ToBoolean(bitwiseField & 0x0100);
			AutoLoad = Convert.ToBoolean(bitwiseField & 0x0200);
		}
		protected override void WriteCore(BinaryWriter writer, IXlsObjData owner) {
			ushort bitwiseField = 0;
			if(IsAuto)
				bitwiseField |= 0x0001;
			if(IsDdeRef)
				bitwiseField |= 0x0002;
			if(PrintCalc)
				bitwiseField |= 0x0004;
			if(IconDisplay)
				bitwiseField |= 0x0008;
			if(IsActiveX)
				bitwiseField |= 0x0010;
			if(InControlStream)
				bitwiseField |= 0x0020;
			if(IsCameraPicture)
				bitwiseField |= 0x0080;
			if(DefaultSize)
				bitwiseField |= 0x0100;
			if(AutoLoad)
				bitwiseField |= 0x0200;
			writer.Write(bitwiseField);
		}
		public override int GetSize(IXlsObjData owner) {
			return 6;
		}
	}
	#endregion
	#region XlsObjCheckbox
	public class XlsObjCheckbox : XlsObjBase {
		protected override void ReadCore(BinaryReader reader, IXlsObjData owner) {
			reader.ReadUInt32(); 
			reader.ReadUInt32(); 
			if(Size == 12)
				reader.ReadUInt32(); 
		}
		protected override void WriteCore(BinaryWriter writer, IXlsObjData owner) {
			writer.Write((uint)0); 
			writer.Write((uint)0); 
			writer.Write((uint)0); 
		}
		public override int GetSize(IXlsObjData owner) {
			return 16;
		}
	}
	#endregion
	#region XlsObjRadioButton
	public class XlsObjRadioButton : XlsObjBase {
		protected override void ReadCore(BinaryReader reader, IXlsObjData owner) {
			reader.ReadUInt32(); 
			reader.ReadUInt16(); 
		}
		protected override void WriteCore(BinaryWriter writer, IXlsObjData owner) {
			writer.Write((uint)0); 
			writer.Write((ushort)0); 
		}
		public override int GetSize(IXlsObjData owner) {
			return 10;
		}
	}
	#endregion
	#region XlsObjScrollable
	public class XlsObjScrollable : XlsObjBase {
		#region Fields
		int value;
		int min;
		int max;
		int inc;
		int page;
		int widthInPixels;
		#endregion
		#region Properties
		public int Value {
			get { return value; }
			set {
				ValueChecker.CheckValue(value, short.MinValue, short.MaxValue);
				this.value = value;
			}
		}
		public int Min {
			get { return min; }
			set {
				ValueChecker.CheckValue(value, short.MinValue, short.MaxValue, "Min");
				min = value;
			}
		}
		public int Max {
			get { return max; }
			set {
				ValueChecker.CheckValue(value, short.MinValue, short.MaxValue, "Max");
				max = value;
			}
		}
		public int Inc {
			get { return inc; }
			set {
				ValueChecker.CheckValue(value, 0, short.MaxValue, "Inc");
				inc = value;
			}
		}
		public int Page {
			get { return page; }
			set {
				ValueChecker.CheckValue(value, 0, short.MaxValue, "Page");
				page = value;
			}
		}
		public bool Horizontal { get; set; }
		public int WidthInPixels {
			get { return widthInPixels; }
			set {
				ValueChecker.CheckValue(value, 0, short.MaxValue, "WidthInPixels");
				widthInPixels = value;
			}
		}
		public bool Draw { get; set; }
		public bool DrawSliderOnly { get; set; }
		public bool TrackElevator { get; set; }
		public bool Without3dEffects { get; set; }
		#endregion
		protected override void ReadCore(BinaryReader reader, IXlsObjData owner) {
			reader.ReadUInt32(); 
			Value = reader.ReadInt16();
			Min = reader.ReadInt16();
			Max = reader.ReadInt16();
			Inc = reader.ReadInt16();
			Page = reader.ReadInt16();
			Horizontal = Convert.ToBoolean(reader.ReadInt16());
			WidthInPixels = reader.ReadInt16();
			ushort bitwiseField = reader.ReadUInt16();
			Draw = Convert.ToBoolean(bitwiseField & 0x0001);
			DrawSliderOnly = Convert.ToBoolean(bitwiseField & 0x0002);
			TrackElevator = Convert.ToBoolean(bitwiseField & 0x0004);
			Without3dEffects = Convert.ToBoolean(bitwiseField & 0x0008);
		}
		protected override void WriteCore(BinaryWriter writer, IXlsObjData owner) {
			writer.Write((uint)0); 
			writer.Write((short)Value);
			writer.Write((short)Min);
			writer.Write((short)Max);
			writer.Write((short)Inc);
			writer.Write((short)Page);
			writer.Write((short)(Horizontal ? 1 : 0));
			writer.Write((short)WidthInPixels);
			ushort bitwiseField = 0;
			if(Draw)
				bitwiseField |= 0x0001;
			if(DrawSliderOnly)
				bitwiseField |= 0x0002;
			if(TrackElevator)
				bitwiseField |= 0x0004;
			if(Without3dEffects)
				bitwiseField |= 0x0008;
			writer.Write(bitwiseField);
		}
		public override int GetSize(IXlsObjData owner) {
			return 24;
		}
	}
	#endregion
	#region XlsObjNote
	public class XlsObjNote : XlsObjBase {
		Guid noteGuid;
		#region Properties
		public Guid NoteGuid {
			get { return noteGuid; }
			set { noteGuid = value; } 
		}
		public bool IsSharedNote { get; set; }
		#endregion
		protected override void ReadCore(BinaryReader reader, IXlsObjData owner) {
			noteGuid = new Guid(reader.ReadBytes(16));
			IsSharedNote = Convert.ToBoolean(reader.ReadUInt16());
			reader.ReadUInt32(); 
		}
		protected override void WriteCore(BinaryWriter writer, IXlsObjData owner) {
			writer.Write(noteGuid.ToByteArray());
			writer.Write((ushort)(IsSharedNote ? 1 : 0));
			writer.Write((int)0); 
		}
		public override int GetSize(IXlsObjData owner) {
			return 26;
		}
	}
	#endregion
	#region XlsObjPictureFormula
	public class XlsObjPictureFormula : XlsObjBase {
		#region Fields
		XlsObjFormula formula = new XlsObjFormula() { IsPartOfPictureFormula = true };
		byte[] licenseKey = new byte[0];
		XlsObjFormula linkedCellFormula = new XlsObjFormula() { IsPartOfPictureFormula = true };
		XlsObjFormula listFillRangeFormula = new XlsObjFormula() { IsPartOfPictureFormula = true };
		#endregion
		#region Properties
		public XlsObjFormula Formula { get { return formula; } }
		public int PositionInControlStream { get; set; }
		public int SizeInControlStream { get; set; }
		public byte[] LicenseKey {
			get { return licenseKey; }
			set {
				if(value != null)
					licenseKey = value;
				else
					licenseKey = new byte[0];
			}
		}
		public XlsObjFormula LinkedCellFormula { get { return linkedCellFormula; } }
		public XlsObjFormula ListFillRangeFormula { get { return listFillRangeFormula; } }
		#endregion
		protected override void ReadCore(BinaryReader reader, IXlsObjData owner) {
			this.formula = XlsObjFormula.FromStream(reader, owner.ContentBuilder, true);
			if(this.formula.StartsWith(typeof(ParsedThingDataTable)))
				PositionInControlStream = reader.ReadInt32();
			if(owner.InControlStream)
				SizeInControlStream = reader.ReadInt32();
			if(owner.IsActiveX) {
				int licenseKeySize = reader.ReadInt32();
				if(licenseKeySize > 0)
					this.licenseKey = reader.ReadBytes(licenseKeySize);
				this.linkedCellFormula = XlsObjFormula.FromStream(reader, owner.ContentBuilder, true);
				this.listFillRangeFormula = XlsObjFormula.FromStream(reader, owner.ContentBuilder, true);
			}
		}
		protected override void WriteCore(BinaryWriter writer, IXlsObjData owner) {
			this.formula.Write(writer);
			if(this.formula.StartsWith(typeof(ParsedThingDataTable)))
				writer.Write(PositionInControlStream);
			if(owner.InControlStream)
				writer.Write(SizeInControlStream);
			if(owner.IsActiveX) {
				int licenseKeySize = this.licenseKey.Length;
				writer.Write(licenseKeySize);
				if(licenseKeySize > 0)
					writer.Write(this.licenseKey);
				this.linkedCellFormula.Write(writer);
				this.listFillRangeFormula.Write(writer);
			}
		}
		public override int GetSize(IXlsObjData owner) {
			int dataSize = this.formula.GetSize();
			if(this.formula.StartsWith(typeof(ParsedThingDataTable)))
				dataSize += 4;
			if(owner.InControlStream)
				dataSize += 4;
			if(owner.IsActiveX) {
				dataSize += 4 + this.licenseKey.Length;
				dataSize += this.linkedCellFormula.GetSize();
				dataSize += this.listFillRangeFormula.GetSize();
			}
			return base.GetSize(owner) + dataSize;
		}
	}
	#endregion
	#region XlsObjCheckboxData
	public enum XlsCheckboxState {
		Unchecked = 0x0000,
		Checked = 0x0001,
		Mixed = 0x0002
	}
	public class XlsObjCheckboxData : XlsObjBase {
		int accel;
		#region Properties
		public XlsCheckboxState State { get; set; }
		public int Accel {
			get { return accel; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "Accel");
				accel = value;
			}
		}
		public bool Without3dEffects { get; set; }
		#endregion
		protected override void ReadCore(BinaryReader reader, IXlsObjData owner) {
			State = (XlsCheckboxState)reader.ReadUInt16();
			Accel = reader.ReadUInt16();
			reader.ReadUInt16(); 
			ushort bitwiseField = reader.ReadUInt16();
			Without3dEffects = Convert.ToBoolean(bitwiseField & 0x0001);
		}
		protected override void WriteCore(BinaryWriter writer, IXlsObjData owner) {
			writer.Write((ushort)State);
			writer.Write((ushort)Accel);
			writer.Write((ushort)0); 
			ushort bitwiseField = 0;
			if(Without3dEffects)
				bitwiseField |= 0x0001;
			writer.Write(bitwiseField);
		}
		public override int GetSize(IXlsObjData owner) {
			return 12;
		}
	}
	#endregion
	#region XlsObjRadioButtonData
	public class XlsObjRadioButtonData : XlsObjBase {
		int nextButton;
		#region Properties
		public int NextButton {
			get { return nextButton; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "NextButton");
				nextButton = value;
			}
		}
		public bool IsFirstButton { get; set; }
		#endregion
		protected override void ReadCore(BinaryReader reader, IXlsObjData owner) {
			NextButton = reader.ReadUInt16();
			IsFirstButton = Convert.ToBoolean(reader.ReadUInt16());
		}
		protected override void WriteCore(BinaryWriter writer, IXlsObjData owner) {
			writer.Write((ushort)NextButton);
			writer.Write((ushort)(IsFirstButton ? 1 : 0));
		}
		public override int GetSize(IXlsObjData owner) {
			return 8;
		}
	}
	#endregion
	#region XlsObjEditboxData
	public enum XlsObjDataValidation {
		None = 0x0000,
		Integer = 0x0001,
		Number = 0x0002,
		RangeReference = 0x0003,
		Formula = 0x0004
	}
	public class XlsObjEditboxData : XlsObjBase {
		int associatedList;
		#region Properties
		public XlsObjDataValidation Validation { get; set; }
		public bool Multiline { get; set; }
		public bool ShowScrollbar { get; set; }
		public int AssociatedList {
			get { return associatedList; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "AssociatedList");
				associatedList = value;
			}
		}
		#endregion
		protected override void ReadCore(BinaryReader reader, IXlsObjData owner) {
			Validation = (XlsObjDataValidation)reader.ReadUInt16();
			Multiline = Convert.ToBoolean(reader.ReadUInt16());
			ShowScrollbar = Convert.ToBoolean(reader.ReadUInt16());
			AssociatedList = reader.ReadUInt16();
		}
		protected override void WriteCore(BinaryWriter writer, IXlsObjData owner) {
			writer.Write((ushort)Validation);
			writer.Write((ushort)(Multiline ? 1 : 0));
			writer.Write((ushort)(ShowScrollbar ? 1 : 0));
			writer.Write((ushort)AssociatedList);
		}
		public override int GetSize(IXlsObjData owner) {
			return 12;
		}
	}
	#endregion
	#region XlsObjGroupboxData
	public class XlsObjGroupboxData : XlsObjBase {
		int accel;
		#region Properties
		public int Accel {
			get { return accel; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "Accel");
				accel = value;
			}
		}
		public bool Without3dEffects { get; set; }
		#endregion
		protected override void ReadCore(BinaryReader reader, IXlsObjData owner) {
			Accel = reader.ReadUInt16();
			reader.ReadUInt16(); 
			ushort bitwiseField = reader.ReadUInt16();
			Without3dEffects = Convert.ToBoolean(bitwiseField & 0x0001);
		}
		protected override void WriteCore(BinaryWriter writer, IXlsObjData owner) {
			writer.Write((ushort)Accel);
			writer.Write((ushort)0); 
			ushort bitwiseField = 0;
			if(Without3dEffects)
				bitwiseField |= 0x0001;
			writer.Write(bitwiseField);
		}
		public override int GetSize(IXlsObjData owner) {
			return 10;
		}
	}
	#endregion
	#region XlsObjFormula
	public class XlsObjFormula {
		#region Fields
		byte[] formulaBytes = new byte[0];
		ParsedExpression expression = new ParsedExpression();
		XLUnicodeStringNoCch className = new XLUnicodeStringNoCch();
		#endregion
		#region Properties
		public bool IsPartOfPictureFormula { get; set; }
		public ParsedExpression Expression { get { return expression; } }
		public string EmbeddedControlClassName {
			get { return className.Value; }
			set { className.Value = value; }
		}
		#endregion
		public static XlsObjFormula FromStream(BinaryReader reader, XlsContentBuilder contentBuilder) {
			return FromStream(reader, contentBuilder, false);
		}
		public static XlsObjFormula FromStream(BinaryReader reader, XlsContentBuilder contentBuilder, bool isPartOfPictureFormula) {
			XlsObjFormula result = new XlsObjFormula();
			result.IsPartOfPictureFormula = isPartOfPictureFormula;
			result.Read(reader, contentBuilder);
			return result;
		}
		protected void Read(BinaryReader reader, XlsContentBuilder contentBuilder) {
			this.expression.Clear();
			this.className.Value = string.Empty;
			int count = reader.ReadUInt16();
			if(count > 0) {
				int formulaSize = reader.ReadUInt16() & 0x7fff;
				reader.ReadUInt32(); 
				this.formulaBytes = reader.ReadBytes(formulaSize);
				this.expression = contentBuilder.RPNContext.BinaryToExpression(formulaBytes, formulaSize);
				int paddingSize = count - (formulaSize + 6);
				if(IsPartOfPictureFormula && StartsWith(typeof(ParsedThingDataTable))) {
					reader.ReadByte(); 
					int charCount = reader.ReadByte();
					reader.ReadByte(); 
					if(charCount > 0) {
						this.className = XLUnicodeStringNoCch.FromStream(reader, charCount);
						paddingSize -= this.className.Length;
					}
					paddingSize -= 3;
				}
				if(paddingSize > 0)
					reader.ReadBytes(paddingSize);
			}
		}
		public void Write(BinaryWriter writer) {
			int count = GetSize() - 2;
			writer.Write((ushort)count);
			if(count > 0) {
				int formulaSize = this.formulaBytes.Length;
				writer.Write((ushort)formulaSize);
				writer.Write((uint)0); 
				writer.Write(this.formulaBytes);
				int paddingSize = count - (formulaSize + 6);
				if(IsPartOfPictureFormula && StartsWith(typeof(ParsedThingDataTable))) {
					writer.Write((byte)0x03); 
					int charCount = this.className.Value.Length;
					writer.Write((byte)charCount);
					writer.Write((byte)0); 
					if(charCount > 0) {
						this.className.Write(writer);
						paddingSize -= this.className.Length;
					}
					paddingSize -= 3;
				}
				while(paddingSize > 0) {
					writer.Write((byte)0);
					paddingSize--;
				}
			}
		}
		public int GetSize() {
			int result = 0;
			if(this.formulaBytes.Length > 0) {
				result += this.formulaBytes.Length + 6;
				if(IsPartOfPictureFormula && StartsWith(typeof(ParsedThingDataTable))) {
					result += 3;
					if(this.className.Value.Length > 0)
						result += this.className.Length;
				}
				if(result % 2 != 0)
					result++;
			}
			return result + 2;
		}
		public void SetExpression(ParsedExpression expression, IRPNContext context) {
			this.expression = expression;
			if(expression.Count > 0) {
				byte[] buf = context.ExpressionToBinary(expression);
				int size = BitConverter.ToUInt16(buf, 0);
				this.formulaBytes = new byte[size];
				Array.Copy(buf, 2, this.formulaBytes, 0, size);
			}
			else
				this.formulaBytes = new byte[0];
		}
		protected internal bool StartsWith(Type ptgType) {
			if(this.expression.Count == 0)
				return false;
			return ptgType.IsInstanceOfType(this.expression[0]);
		}
	}
	#endregion
	#region XlsObjDropData
	public enum XlsDropDownStyle {
		Combo = 0,
		ComboEdit = 1,
		Simple = 2
	}
	public class XlsObjDropData {
		int linesDisplayed;
		int minWidthInPixels;
		XLUnicodeString currentValue = new XLUnicodeString();
		#region Properties
		public XlsDropDownStyle DropDownStyle { get; set; }
		public bool IsFiltered { get; set; }
		public int LinesDisplayed {
			get { return linesDisplayed; }
			set {
				ValueChecker.CheckValue(value, 0, 0x7fff, "LinesDisplayed");
				linesDisplayed = value;
			}
		}
		public int MinWidthInPixels {
			get { return minWidthInPixels; }
			set {
				ValueChecker.CheckValue(value, 0, 0x7fff, "MinWidthInPixels");
				minWidthInPixels = value;
			}
		}
		public string CurrentValue {
			get { return this.currentValue.Value; }
			set { this.currentValue.Value = value; }
		}
		#endregion
		public static XlsObjDropData FromStream(BinaryReader reader) {
			XlsObjDropData result = new XlsObjDropData();
			result.Read(reader);
			return result;
		}
		protected void Read(BinaryReader reader) {
			ushort bitwiseField = reader.ReadUInt16();
			DropDownStyle = (XlsDropDownStyle)(bitwiseField & 0x0003);
			IsFiltered = Convert.ToBoolean(bitwiseField & 0x0008);
			this.linesDisplayed = reader.ReadUInt16();
			this.minWidthInPixels = reader.ReadUInt16();
			this.currentValue = XLUnicodeString.FromStream(reader);
			if(this.currentValue.Length % 2 != 0)
				reader.ReadByte();
		}
		public void Write(BinaryWriter writer) {
			ushort bitwiseField = (ushort)DropDownStyle;
			if(IsFiltered)
				bitwiseField |= 0x0008;
			writer.Write(bitwiseField);
			writer.Write((ushort)this.linesDisplayed);
			writer.Write((ushort)this.minWidthInPixels);
			this.currentValue.Write(writer);
			if(this.currentValue.Length % 2 != 0)
				writer.Write((byte)0);
		}
		public int GetSize() {
			return (this.currentValue.Length % 2 != 0) ? this.currentValue.Length + 7 : this.currentValue.Length + 6;
		}
	}
	#endregion
	#region XlsObjListData
	public enum XlsListSelectionType {
		Single = 0,
		Multiple = 1,
		MultipleWithCtrl = 2
	}
	public enum XlsListBehaviour {
		Regular = 0x00,
		PivotTablePage = 0x01,
		AutoFilter = 0x03,
		AutoComplete = 0x05,
		DataValidationList = 0x06,
		PivotTableRowColumn = 0x07,
		TotalRow = 0x09
	}
	public class XlsObjListData : XlsObjBase {
		bool isEmpty = true;
		XlsObjFormula formula = new XlsObjFormula();
		int linesCount;
		int selectedIndex;
		int editBoxId;
		XlsObjDropData dropDownData = new XlsObjDropData();
		List<string> lines = new List<string>();
		List<bool> selected = new List<bool>();
		#region Properties
		public bool IsEmpty { get { return isEmpty; } set { isEmpty = value; } }
		public XlsObjFormula Formula { get { return formula; } }
		public int LinesCount {
			get { return linesCount; }
			set {
				ValueChecker.CheckValue(value, 0, 0x7fff, "LinesCount");
				linesCount = value;
			}
		}
		public int SelectedIndex {
			get { return selectedIndex; }
			set {
				ValueChecker.CheckValue(value, 0, 0x8000, "SelectedIndex");
				selectedIndex = value;
			}
		}
		public bool UseBehaviourClass { get; set; }
		public bool IsValidPlex { get; set; }
		public bool IsValidIds { get; set; }
		public bool Without3dEffects { get; set; }
		public XlsListSelectionType SelectionType { get; set; }
		public XlsListBehaviour ListBehaviour { get; set; }
		public int EditBoxId {
			get { return editBoxId; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "EditBoxId");
				editBoxId = value;
			}
		}
		public XlsObjDropData DropDownData { get { return dropDownData; } }
		public List<string> Lines { get { return lines; } }
		public List<bool> Selected { get { return selected; } }
		#endregion
		public override void Read(BinaryReader reader, IXlsObjData owner) {
			int continued = reader.ReadUInt16(); 
			if(continued == 0) return;
			this.isEmpty = false;
			this.formula = XlsObjFormula.FromStream(reader, owner.ContentBuilder);
			this.linesCount = reader.ReadUInt16();
			SelectedIndex = reader.ReadUInt16();
			byte bitwiseField = reader.ReadByte();
			UseBehaviourClass = Convert.ToBoolean(bitwiseField & 0x01);
			IsValidPlex = Convert.ToBoolean(bitwiseField & 0x02);
			IsValidIds = Convert.ToBoolean(bitwiseField & 0x04);
			Without3dEffects = Convert.ToBoolean(bitwiseField & 0x08);
			SelectionType = (XlsListSelectionType)((bitwiseField & 0x30) >> 4);
			if(UseBehaviourClass)
				ListBehaviour = (XlsListBehaviour)reader.ReadByte();
			else
				reader.ReadByte();
			EditBoxId = reader.ReadUInt16();
			if(owner.ObjType == XlsObjType.DropdownList)
				this.dropDownData = XlsObjDropData.FromStream(reader);
			if(IsValidPlex) {
				for(int i = 0; i < this.linesCount; i++)
					Lines.Add(XLUnicodeString.FromStream(reader).Value);
			}
			if(SelectionType != XlsListSelectionType.Single) {
				for(int i = 0; i < this.linesCount; i++)
					Selected.Add(Convert.ToBoolean(reader.ReadByte()));
			}
		}
		public override void Write(BinaryWriter writer, IXlsObjData owner) {
			writer.Write(XlsObjFactory.GetTypeCodeByType(GetType()));
			if(this.isEmpty) {
				writer.Write((ushort)0);
				return;
			}
			XlsChunkWriter chunkWriter = writer as XlsChunkWriter;
			int continued = GetFullSize(owner.ObjType);
			if(chunkWriter != null) {
				int sizeInCurrentRecord = GetSizeInCurrentRecord(owner.ObjType, chunkWriter.SpaceInCurrentRecord - 2);
				if(sizeInCurrentRecord < continued)
					continued = sizeInCurrentRecord - 1;
			}
			writer.Write((ushort)continued);
			this.formula.Write(writer);
			writer.Write((ushort)LinesCount);
			writer.Write((ushort)SelectedIndex);
			byte bitwiseField = (byte)((byte)SelectionType << 4);
			if(UseBehaviourClass)
				bitwiseField |= 0x01;
			if(IsValidPlex)
				bitwiseField |= 0x02;
			if(IsValidIds)
				bitwiseField |= 0x04;
			if(Without3dEffects)
				bitwiseField |= 0x08;
			writer.Write(bitwiseField);
			if(UseBehaviourClass)
				writer.Write((byte)ListBehaviour);
			else
				writer.Write((byte)0);
			writer.Write((ushort)EditBoxId);
			if(owner.ObjType == XlsObjType.DropdownList)
				this.dropDownData.Write(writer);
			if(LinesCount > 0) {
				if(IsValidPlex) {
					XLUnicodeString str = new XLUnicodeString();
					for(int i = 0; i < LinesCount; i++) {
						if(i < Lines.Count)
							str.Value = Lines[i];
						else
							str.Value = string.Empty;
						if(chunkWriter != null) chunkWriter.BeginRecord(str.Length);
						str.Write(writer);
					}
				}
				if(SelectionType != XlsListSelectionType.Single) {
					byte item;
					for(int i = 0; i < LinesCount; i++) {
						if(i < Selected.Count)
							item = (byte)(Selected[i] ? 1 : 0);
						else
							item = 0;
						if(chunkWriter != null && (i % 8 == 0)) chunkWriter.BeginRecord(8);
						writer.Write(item);
					}
				}
			}
		}
		public override int GetSize(IXlsObjData owner) {
			return base.GetSize(owner) + GetFullSize(owner.ObjType);
		}
		int GetFullSize(XlsObjType objectType) {
			int result = 0;
			if(IsEmpty) return result;
			result += this.formula.GetSize();
			result += 8;
			if(objectType == XlsObjType.DropdownList)
				result += this.dropDownData.GetSize();
			if(LinesCount > 0) {
				if(IsValidPlex) {
					XLUnicodeString str = new XLUnicodeString();
					for(int i = 0; i < LinesCount; i++) {
						if(i < LinesCount)
							str.Value = Lines[i];
						else
							str.Value = string.Empty;
						result += str.Length;
					}
				}
				if(SelectionType != XlsListSelectionType.Single) {
					result += LinesCount;
				}
			}
			return result;
		}
		int GetSizeInCurrentRecord(XlsObjType objectType, int spaceInCurrentRecord) {
			int result = 0;
			if(IsEmpty) return result;
			result += this.formula.GetSize();
			result += 8;
			if(objectType == XlsObjType.DropdownList)
				result += this.dropDownData.GetSize();
			if(LinesCount > 0) {
				if(IsValidPlex) {
					XLUnicodeString str = new XLUnicodeString();
					for(int i = 0; i < LinesCount; i++) {
						if(i < Lines.Count)
							str.Value = Lines[i];
						else
							str.Value = string.Empty;
						int length = str.Length;
						if((result + length) > spaceInCurrentRecord)
							return result;
						result += length;
					}
				}
				if(SelectionType != XlsListSelectionType.Single) {
					int lineCount = LinesCount;
					while(lineCount > 8) {
						if((result + 8) > spaceInCurrentRecord)
							return result;
						result += 8;
						lineCount -= 8;
					}
					if((result + lineCount) > spaceInCurrentRecord)
						return result;
					result += lineCount;
				}
			}
			return result;
		}
	}
	#endregion
	#region XlsObjImageData
	public enum XlsObjImageEnvironment {
		MicrosoftWindows = 1,
		AppleMacintosh = 2
	}
	public class XlsObjImageData {
		#region Fields
		const int fixedPartSize = 8;
		byte[] data = new byte[0];
		#endregion
		public XlsObjImageData() {
			ClipboardFormat = XlsObjPictureType.Bitmap;
			Environment = XlsObjImageEnvironment.MicrosoftWindows;
		}
		#region Properties
		public XlsObjPictureType ClipboardFormat { get; set; }
		public XlsObjImageEnvironment Environment { get; set; }
		public byte[] Data {
			get { return data; }
			set {
				if(value != null)
					data = value;
				else
					data = new byte[0];
			}
		}
		public bool HasData { get { return data.Length > 0; } }
		#endregion
		public void Read(BinaryReader reader) {
			ClipboardFormat = (XlsObjPictureType)reader.ReadUInt16();
			Environment = (XlsObjImageEnvironment)reader.ReadUInt16();
			int size = reader.ReadInt32();
			if(size > 0)
				this.data = reader.ReadBytes(size);
			else
				this.data = new byte[0];
		}
		public void Write(BinaryWriter writer) {
			writer.Write((ushort)ClipboardFormat);
			writer.Write((ushort)Environment);
			writer.Write((int)this.data.Length);
			if(this.data.Length > 0)
				writer.Write(this.data);
		}
		public int GetSize() {
			return this.data.Length + fixedPartSize;
		}
	}
	#endregion
	#region XlsObjData
	public class XlsObjData : IXlsObjData, IEnumerable<IXlsObjRecord> {
		const short embeddedImageDataTypeCode = 0x007f;
		List<IXlsObjRecord> items = new List<IXlsObjRecord>();
		XlsObjImageData embeddedImageData = new XlsObjImageData();
		Chart embeddedChart = null;
		XlsContentBuilder contentBuilder;
		#region Properties
		public XlsObjType ObjType {
			get {
				XlsObjCommon common = CommonProperties;
				if(common == null)
					return XlsObjType.Undefined;
				return common.ObjType;
			}
		}
		public int ObjId {
			get {
				XlsObjCommon common = CommonProperties;
				if(common == null)
					return 0;
				return common.ObjId;
			}
		}
		public bool IsActiveX {
			get {
				XlsObjPictureFlags flags = FindRecord(typeof(XlsObjPictureFlags)) as XlsObjPictureFlags;
				if(flags != null)
					return flags.IsActiveX;
				return false;
			}
		}
		public bool InControlStream {
			get {
				XlsObjPictureFlags flags = FindRecord(typeof(XlsObjPictureFlags)) as XlsObjPictureFlags;
				if(flags != null)
					return flags.InControlStream;
				return false;
			}
		}
		public XlsContentBuilder ContentBuilder { get { return contentBuilder; } }
		public int Count { get { return items.Count; } }
		public IXlsObjRecord this[int index] {
			get { return this.items[index]; }
			set { this.items[index] = value; }
		}
		public XlsObjImageData EmbeddedImageData { get { return embeddedImageData; } }
		public XlsObjCommon CommonProperties {
			get {
				if(items.Count == 0)
					return null;
				return items[0] as XlsObjCommon;
			}
		}
		public Chart EmbeddedChart { get { return embeddedChart; } }
		protected internal XlsChartExporter ChartExporter { get; set; }
		#endregion
		public void Add(IXlsObjRecord item) {
			Guard.ArgumentNotNull(item, "item");
			this.items.Add(item);
		}
		public void Clear() {
			this.items.Clear();
		}
		public int Size {
			get { return GetSize(); }
		}
		public void Read(BinaryReader reader, XlsContentBuilder contentBuilder) {
			Guard.ArgumentNotNull(reader, "reader");
			Guard.ArgumentNotNull(contentBuilder, "contentBuilder");
			this.contentBuilder = contentBuilder;
			while(true) {
				IXlsObjRecord item = XlsObjFactory.CreateRecord(reader);
				item.Read(reader, this);
				if(item is XlsObjUnknown) continue;
				this.items.Add(item);
				if(item is XlsObjEnd) break;
				if(item is XlsObjListData) break;
			}
			XlsCommandStream stream = reader.BaseStream as XlsCommandStream;
			if (stream != null)
				stream.SkipTillEndOfBlock();
			if (ObjType == XlsObjType.Picture) {
				if(stream != null && stream.GetNextTypeCode() == embeddedImageDataTypeCode)
					this.embeddedImageData.Read(reader);
			}
			else if (ObjType == XlsObjType.Chart) {
				this.embeddedChart = new Chart(contentBuilder.CurrentSheet);
				contentBuilder.CurrentChart = this.embeddedChart;
			}
		}
		public void Write(BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			int count = items.Count;
			for(int i = 0; i < count; i++)
				items[i].Write(writer, this);
			if (ObjType == XlsObjType.Chart && ChartExporter != null) {
				writer.Flush(); 
				ChartExporter.WriteContent();
			}
		}
		public void Execute(XlsContentBuilder contentBuilder) {
			Guard.ArgumentNotNull(contentBuilder, "contentBuilder");
			int count = items.Count;
			for(int i = 0; i < count; i++)
				items[i].Execute(contentBuilder);
		}
		public int GetSize() {
			int result = 0;
			int count = items.Count;
			for(int i = 0; i < count; i++)
				result += items[i].GetSize(this);
			return result;
		}
		#region IEnumerable<IXlsObjRecord> Members
		public IEnumerator<IXlsObjRecord> GetEnumerator() {
			return ((IEnumerable<IXlsObjRecord>)this.items).GetEnumerator();
		}
		#endregion
		#region IEnumerable Members
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return ((System.Collections.IEnumerable)this.items).GetEnumerator();
		}
		#endregion
		#region Internals
		IXlsObjRecord FindRecord(Type recordType) {
			int count = items.Count;
			for(int i = 0; i < count; i++) {
				IXlsObjRecord item = items[i];
				if(recordType.IsInstanceOfType(item))
					return item;
			}
			return null;
		}
		#endregion
	}
	#endregion
	#region XlsCommandObject
	public class XlsCommandObject : XlsCommandRecordBase {
		public override IXlsCommand GetInstance() {
			return new XlsCommandObject();
		}
	}
	#endregion
	#region XlsTextObjData
	public enum XlsTextRotation {
		None = 0,
		StackedOrVertical = 1,
		CounterClockwise90 = 2,
		Clockwise90 = 3
	}
	public class XlsTextObjData {
		#region Fields
		XlsObjFormula formula = new XlsObjFormula();
		string text = string.Empty;
		List<XlsFormatRun> formatRuns = new List<XlsFormatRun>();
		#endregion
		#region Properties
		public XlHorizontalAlignment HorizontalAlignment { get; set; }
		public XlVerticalAlignment VerticalAlignment { get; set; }
		public bool IsLocked { get; set; }
		public bool JustifyLast { get; set; }
		public bool IsSecret { get; set; }
		public XlsTextRotation Rotation { get; set; }
		public int FontIndexEmpty { get; set; }
		public XlsObjFormula Formula {
			get { return formula; }
			set {
				Guard.ArgumentNotNull(value, "Formula");
				formula = value;
			}
		}
		public string Text {
			get { return text; }
			set {
				if(string.IsNullOrEmpty(value))
					text = string.Empty;
				else
					text = value;
			}
		}
		public IList<XlsFormatRun> FormatRuns { get { return formatRuns; } }
		#endregion
		public void Read(BinaryReader reader, XlsContentBuilder contentBuilder) {
			ushort bitwiseField = reader.ReadUInt16();
			HorizontalAlignment = XlsTextAlignHelper.HorizontalTextToModelAlign((bitwiseField & 0x000e) >> 1);
			VerticalAlignment = XlsTextAlignHelper.VerticalTextToModelAlign((bitwiseField & 0x0070) >> 4);
			IsLocked = Convert.ToBoolean(bitwiseField & 0x0200);
			JustifyLast = Convert.ToBoolean(bitwiseField & 0x4000);
			IsSecret = Convert.ToBoolean(bitwiseField & 0x8000);
			Rotation = (XlsTextRotation)(reader.ReadUInt16() & 0x03);
			reader.ReadUInt16(); 
			reader.ReadUInt32(); 
			int charCount = reader.ReadUInt16();
			int runSize = reader.ReadUInt16();
			FontIndexEmpty = reader.ReadUInt16();
			this.formula = XlsObjFormula.FromStream(reader, contentBuilder);
			if(charCount > 0) {
				while(charCount > 0) {
					bool hasHighBytes = Convert.ToBoolean(reader.ReadByte());
					int charsInRecord = charCount;
					int length = hasHighBytes ? charCount * 2 : charCount;
					if(length >= XlsDefs.MaxRecordDataSize) {
						length = XlsDefs.MaxRecordDataSize - 1;
						if(hasHighBytes) {
							charsInRecord = length / 2;
							length = charsInRecord * 2;
						}
						else
							charsInRecord = length;
					}
					byte[] buffer = reader.ReadBytes(length);
					this.text += XLStringEncoder.GetEncoding(hasHighBytes).GetString(buffer, 0, length);
					charCount -= charsInRecord;
				}
				int runCount = runSize / 8 - 1;
				for(int i = 0; i < runCount; i++) {
					this.formatRuns.Add(XlsFormatRun.FromStream(reader));
					reader.ReadUInt16(); 
					reader.ReadUInt16(); 
				}
				reader.ReadUInt16();
				reader.ReadUInt16();
				reader.ReadUInt32();
			}
		}
		public void Write(BinaryWriter writer) {
			ushort bitwiseField = (ushort)(XlsTextAlignHelper.ModelAlignToHorizontalText(HorizontalAlignment) << 1);
			bitwiseField |= (ushort)(XlsTextAlignHelper.ModelAlignToVerticalText(VerticalAlignment) << 4);
			if(IsLocked)
				bitwiseField |= 0x0200;
			if(JustifyLast)
				bitwiseField |= 0x4000;
			if(IsSecret)
				bitwiseField |= 0x8000;
			writer.Write(bitwiseField);
			writer.Write((ushort)Rotation);
			writer.Write((ushort)0); 
			writer.Write((uint)0); 
			int charCount = this.text.Length;
			writer.Write((ushort)charCount);
			int runSize = (charCount > 0) ? (this.formatRuns.Count * 8 + 8) : 0;
			writer.Write((ushort)runSize);
			writer.Write((ushort)FontIndexEmpty);
			this.formula.Write(writer);
			if(charCount > 0) {
				writer.Flush(); 
				XlsChunkWriter chunkWriter = writer as XlsChunkWriter;
				bool hasHighBytes = XLStringEncoder.StringHasHighBytes(this.text);
				writer.Write((byte)(hasHighBytes ? 1 : 0));
				if(chunkWriter != null) chunkWriter.BeginStringValue(hasHighBytes);
				writer.Write(XLStringEncoder.GetBytes(this.text, hasHighBytes));
				if(chunkWriter != null) chunkWriter.EndStringValue();
				writer.Flush(); 
				int runCount = this.formatRuns.Count;
				for(int i = 0; i < runCount; i++) {
					this.formatRuns[i].Write(writer);
					writer.Write((ushort)0); 
					writer.Write((ushort)0); 
				}
				writer.Write((ushort)charCount);
				writer.Write((ushort)0);
				writer.Write((uint)0);
			}
		}
	}
	#endregion
	#region XlsCommandTextObject
	public class XlsCommandTextObject : XlsCommandRecordBase {
		public override IXlsCommand GetInstance() {
			return new XlsCommandTextObject();
		}
	}
	#endregion
}
