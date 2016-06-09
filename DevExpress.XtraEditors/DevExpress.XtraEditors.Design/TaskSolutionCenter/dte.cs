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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
namespace DevExpress.XtraEditors.Design.TasksSolution {
	[ComImport, Guid("00000000-0000-0000-c000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IUnknown {
		[PreserveSig]
		uint QueryInterface(REFIID riid,
			[MarshalAs(UnmanagedType.Interface)] out object ppv);
		[PreserveSig]
		IntPtr AddRef();
		[PreserveSig]
		IntPtr Release();
	}
	[StructLayout(LayoutKind.Sequential)]
	public struct REFIID {
		public uint x;
		public short s1;
		public short s2;
		[MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray,
			 SizeConst=8)]
		public byte[] chars;
		public REFIID(string guid) {
			string[] data = guid.Split('-');
			if (data.Length != 5)
				throw new Exception ("Invalid guid");
			x = Convert.ToUInt32(data[0], 16);
			s1 = Convert.ToInt16(data[1], 16);
			s2 = Convert.ToInt16(data[2], 16);
			string bytesData = data[3] + data[4];
			chars = new byte[] { Convert.ToByte(bytesData.Substring(0,2), 16),
								   Convert.ToByte(bytesData.Substring(2,2), 16),
								   Convert.ToByte(bytesData.Substring(4,2), 16),
								   Convert.ToByte(bytesData.Substring(6,2), 16),
								   Convert.ToByte(bytesData.Substring(8,2), 16),
								   Convert.ToByte(bytesData.Substring(10,2), 16),
								   Convert.ToByte(bytesData.Substring(12,2), 16),
								   Convert.ToByte(bytesData.Substring(14,2), 16) };
		}
	}
	[Guid("D83C7FAD-BC3A-46BB-9BFC-C0091329626D")]
	public abstract class CodeModelLanguageConstants {
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsCMLanguageCSharp = "{B5E9BD34-6D3E-4B5D-925E-8A43B79820B4}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsCMLanguageIDL = "{B5E9BD35-6D3E-4B5D-925E-8A43B79820B4}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsCMLanguageMC = "{B5E9BD36-6D3E-4B5D-925E-8A43B79820B4}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsCMLanguageVB = "{B5E9BD33-6D3E-4B5D-925E-8A43B79820B4}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsCMLanguageVC = "{B5E9BD32-6D3E-4B5D-925E-8A43B79820B4}";
	}
	public enum vsCMTypeRef {
		vsCMTypeRefArray = 2,
		vsCMTypeRefBool = 15,
		vsCMTypeRefByte = 7,
		vsCMTypeRefChar = 8,
		vsCMTypeRefCodeType = 1,
		vsCMTypeRefDecimal = 14,
		vsCMTypeRefDouble = 13,
		vsCMTypeRefFloat = 12,
		vsCMTypeRefInt = 10,
		vsCMTypeRefLong = 11,
		vsCMTypeRefObject = 6,
		vsCMTypeRefOther = 0,
		vsCMTypeRefPointer = 4,
		vsCMTypeRefShort = 9,
		vsCMTypeRefString = 5,
		vsCMTypeRefVariant = 0x10,
		vsCMTypeRefVoid = 3
	}
	public enum vsCMAccess {
		vsCMAccessAssemblyOrFamily = 0x40,
		vsCMAccessDefault = 0x20,
		vsCMAccessPrivate = 2,
		vsCMAccessProject = 4,
		vsCMAccessProjectOrProtected = 12,
		vsCMAccessProtected = 8,
		vsCMAccessPublic = 1,
		vsCMAccessWithEvents = 0x80
	}
	public enum vsCMFunction {
		vsCMFunctionComMethod = 0x10000,
		vsCMFunctionConstant = 0x2000,
		vsCMFunctionConstructor = 1,
		vsCMFunctionDestructor = 0x200,
		vsCMFunctionFunction = 0x80,
		vsCMFunctionInline = 0x8000,
		vsCMFunctionOperator = 0x400,
		vsCMFunctionOther = 0,
		vsCMFunctionPropertyAssign = 0x20,
		vsCMFunctionPropertyGet = 2,
		vsCMFunctionPropertyLet = 4,
		vsCMFunctionPropertySet = 8,
		vsCMFunctionPure = 0x1000,
		vsCMFunctionPutRef = 0x10,
		vsCMFunctionShared = 0x4000,
		vsCMFunctionSub = 0x40,
		vsCMFunctionTopLevel = 0x100,
		vsCMFunctionVirtual = 0x800
	}
	public enum vsCMPart {
		vsCMPartAttributes = 2,
		vsCMPartAttributesWithDelimiter = 0x44,
		vsCMPartBody = 0x10,
		vsCMPartBodyWithDelimiter = 80,
		vsCMPartHeader = 4,
		vsCMPartHeaderWithAttributes = 6,
		vsCMPartName = 1,
		vsCMPartNavigate = 0x20,
		vsCMPartWhole = 8,
		vsCMPartWholeWithAttributes = 10
	}
	[Guid("B26AC3C2-3981-4A2E-9D6F-559B41CD1CD2")]
	public enum vsPaneShowHow {
		vsPaneShowAsIs = 2,
		vsPaneShowCentered = 0,
		vsPaneShowTop = 1
	}
	[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIDispatch), Guid("7F59E94E-4939-40D2-9F7F-B7651C25905D"), TypeLibType((short) 4160)]
	public interface TextPoint {
		[DispId(1)]
		IUnknown DTE { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(1)] get; }
		[DispId(2)]
		IUnknown Parent { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(2)] get; }
		[DispId(11)]
		int Line { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(11)] get; }
		[DispId(12)]
		int LineCharOffset { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(12)] get; }
		[DispId(13)]
		int AbsoluteCharOffset { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(13)] get; }
		[DispId(14)]
		int DisplayColumn { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(14)] get; }
		[DispId(0x15)]
		bool AtEndOfDocument { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x15)] get; }
		[DispId(0x16)]
		bool AtStartOfDocument { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x16)] get; }
		[DispId(0x17)]
		bool AtEndOfLine { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x17)] get; }
		[DispId(0x18)]
		bool AtStartOfLine { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x18)] get; }
		[DispId(0x19)]
		int LineLength { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x19)] get; }
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x1f)]
		bool EqualTo([In, MarshalAs(UnmanagedType.Interface)] TextPoint Point);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x20)]
		bool LessThan([In, MarshalAs(UnmanagedType.Interface)] TextPoint Point);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x21)]
		bool GreaterThan([In, MarshalAs(UnmanagedType.Interface)] TextPoint Point);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(50)]
		bool TryToShow([In, Optional] int How , [In, Optional, MarshalAs(UnmanagedType.Struct)] object PointOrCount);
		[DispId(0x33)]
		IUnknown this[int Scope] { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x33)] get; }
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x22)]
		EditPoint CreateEditPoint();
	}
	[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIDispatch), TypeLibType((short) 4160), Guid("C1FFE800-028B-4475-A907-14F51F19BB7D")]
	public interface EditPoint  {
		[DispId(1)]
		IUnknown DTE { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(1)] get; }
		[DispId(2)]
		IUnknown Parent { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(2)] get; }
		[DispId(11)]
		int Line { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(11)] get; }
		[DispId(12)]
		int LineCharOffset { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(12)] get; }
		[DispId(13)]
		int AbsoluteCharOffset { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(13)] get; }
		[DispId(14)]
		int DisplayColumn { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(14)] get; }
		[DispId(0x15)]
		bool AtEndOfDocument { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x15)] get; }
		[DispId(0x16)]
		bool AtStartOfDocument { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x16)] get; }
		[DispId(0x17)]
		bool AtEndOfLine { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x17)] get; }
		[DispId(0x18)]
		bool AtStartOfLine { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x18)] get; }
		[DispId(0x19)]
		int LineLength { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x19)] get; }
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x1f)]
		bool EqualTo([In, MarshalAs(UnmanagedType.Interface)] TextPoint Point);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x20)]
		bool LessThan([In, MarshalAs(UnmanagedType.Interface)] TextPoint Point);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x21)]
		bool GreaterThan([In, MarshalAs(UnmanagedType.Interface)] TextPoint Point);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(50)]
		bool TryToShow([In, Optional] vsPaneShowHow How , [In, Optional, MarshalAs(UnmanagedType.Struct)] object PointOrCount);
		[DispId(0x33)]
		CodeElement this[int Scope] { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x33)] get; }
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x22)]
		EditPoint CreateEditPoint();
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x65)]
		void CharLeft([In, Optional] int Count );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x66)]
		void CharRight([In, Optional] int Count );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x67)]
		void EndOfLine();
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x68)]
		void StartOfLine();
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x69)]
		void EndOfDocument();
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x6a)]
		void StartOfDocument();
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x6b)]
		void WordLeft([In, Optional] int Count );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x6c)]
		void WordRight([In, Optional] int Count );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x6d)]
		void LineUp([In, Optional] int Count );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(110)]
		void LineDown([In, Optional] int Count );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(170)]
		void MoveToPoint([In, MarshalAs(UnmanagedType.Interface)] TextPoint Point);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xab)]
		void MoveToLineAndOffset([In] int Line, [In] int Offset);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xac)]
		void MoveToAbsoluteOffset([In] int Offset);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x79)]
		void SetBookmark();
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x7a)]
		void ClearBookmark();
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x7b)]
		bool NextBookmark();
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x7c)]
		bool PreviousBookmark();
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x83)]
		void PadToColumn([In] int Column);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x84)]
		void Insert([In, MarshalAs(UnmanagedType.BStr)] string Text);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x85)]
		void InsertFromFile([In, MarshalAs(UnmanagedType.BStr)] string File);
		[return: MarshalAs(UnmanagedType.BStr)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x86)]
		string GetText([In, MarshalAs(UnmanagedType.Struct)] object PointOrCount);
		[return: MarshalAs(UnmanagedType.BStr)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xad)]
		string GetLines([In] int Start, [In] int ExclusiveEnd);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x88)]
		void Copy([In, MarshalAs(UnmanagedType.Struct)] object PointOrCount, [In, Optional] bool Append );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x89)]
		void Cut([In, MarshalAs(UnmanagedType.Struct)] object PointOrCount, [In, Optional] bool Append );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x87)]
		void Delete([In, MarshalAs(UnmanagedType.Struct)] object PointOrCount);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x8a)]
		void Paste();
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x8b)]
		bool ReadOnly([In, MarshalAs(UnmanagedType.Struct)] object PointOrCount);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x97)]
		bool FindPattern([In, MarshalAs(UnmanagedType.BStr)] string Pattern, [In, Optional] int vsFindOptionsValue , [In, Out, Optional, MarshalAs(UnmanagedType.Interface)] ref EditPoint EndPoint , [In, Out, Optional, MarshalAs(UnmanagedType.Interface)] ref TextRanges Tags );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x98)]
		bool ReplacePattern([In, MarshalAs(UnmanagedType.Interface)] TextPoint Point, [In, MarshalAs(UnmanagedType.BStr)] string Pattern, [In, MarshalAs(UnmanagedType.BStr)] string Replace, [In, Optional] int vsFindOptionsValue , [In, Out, Optional, MarshalAs(UnmanagedType.Interface)] ref TextRanges Tags );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xa1)]
		void Indent([In, Optional, MarshalAs(UnmanagedType.Interface)] TextPoint Point , [In, Optional] int Count );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xa2)]
		void Unindent([In, Optional, MarshalAs(UnmanagedType.Interface)] TextPoint Point , [In, Optional] int Count );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xa3)]
		void SmartFormat([In, MarshalAs(UnmanagedType.Interface)] TextPoint Point);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xa7)]
		void OutlineSection([In, MarshalAs(UnmanagedType.Struct)] object PointOrCount);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xa4)]
		void ReplaceText([In, MarshalAs(UnmanagedType.Struct)] object PointOrCount, [In, MarshalAs(UnmanagedType.BStr)] string Text, [In] int Flags);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xa5)]
		void ChangeCase([In, MarshalAs(UnmanagedType.Struct)] object PointOrCount, [In] int How);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xa6)]
		void DeleteWhitespace([In, Optional] int Direction );
	}
	[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIDispatch), TypeLibType((short) 4160), DefaultMember("Item"), Guid("B6422E9C-9EFD-4F87-BDDC-C7FD8F2FD303")]
	public interface TextRanges : IEnumerable {
		[DispId(1)]
		IUnknown DTE { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(1)] get; }
		[DispId(2)]
		IUnknown Parent { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(2)] get; }
		[DispId(3)]
		int Count { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(3)] get; }
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0)]
		IUnknown Item([In, MarshalAs(UnmanagedType.Struct)] object index);
	}
	[Guid("496B0ABE-CDEE-11d3-88E8-00902754C43A")]
	public interface IEnumerable {
		[DispId(-4)]
		IEnumerator GetEnumerator();
	}
	[Guid("496B0ABF-CDEE-11d3-88E8-00902754C43A")]
	public interface IEnumerator {
		bool MoveNext();
		object Current { get; }
		void Reset();
	}
	[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIDispatch), Guid("0CFBC2B5-0D4E-11D3-8997-00C04F688DDE"), TypeLibType((short) 4160), DefaultMember("Item")]
	public interface CodeElements {
		[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType="", MarshalCookie="")]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 1), DispId(-4)]
		IEnumerator GetEnumerator();
		[DispId(1)]
		IUnknown DTE { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(1)] get; }
		[DispId(2)]
		object Parent { [return: MarshalAs(UnmanagedType.IDispatch)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(2)] get; }
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0)]
		CodeElement Item([MarshalAs(UnmanagedType.Struct)] object index);
		[DispId(3)]
		int Count { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(3)] get; }
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(4), TypeLibFunc((short) 0x41)]
		void Reserved1([MarshalAs(UnmanagedType.Struct)] object Element);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(5)]
		bool CreateUniqueID([In, MarshalAs(UnmanagedType.BStr)] string Prefix, [In, Out, Optional, MarshalAs(UnmanagedType.BStr)] ref string NewName );
	}
	[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIDispatch), TypeLibType((short) 4160), Guid("0CFBC2B6-0D4E-11D3-8997-00C04F688DDE")]
	public interface CodeElement {
		[DispId(1)]
		IUnknown DTE { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x400), DispId(1)] get; }
		[DispId(2)]
		CodeElements Collection { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(2), TypeLibFunc((short) 0x400)] get; }
		[DispId(0)]
		string Name { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0)] get; [param: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0)] set; }
		[DispId(3)]
		string FullName { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(3)] get; }
		[DispId(4)]
		ProjectItem ProjectItem { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(4), TypeLibFunc((short) 0x400)] get; }
		[DispId(5)]
		int Kind { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x400), DispId(5)] get; }
		[DispId(6)]
		bool IsCodeType { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(6), TypeLibFunc((short) 0x400)] get; }
		[DispId(7)]
		int InfoLocation { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(7), TypeLibFunc((short) 0x400)] get; }
		[DispId(8)]
		CodeElements Children { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(8), TypeLibFunc((short) 0x400)] get; }
		[DispId(9)]
		string Language { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x400), DispId(9)] get; }
		[DispId(10)]
		TextPoint StartPoint { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(10), TypeLibFunc((short) 0x400)] get; }
		[DispId(11)]
		TextPoint EndPoint { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(11), TypeLibFunc((short) 0x400)] get; }
		[DispId(12)]
		object ExtenderNames { [return: MarshalAs(UnmanagedType.Struct)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x400), DispId(12)] get; }
		[DispId(13)]
		object this[string ExtenderName] { [return: MarshalAs(UnmanagedType.IDispatch)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(13), TypeLibFunc((short) 0x400)] get; }
		[DispId(14)]
		string ExtenderCATID { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x400), DispId(14)] get; }
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(15), TypeLibFunc((short) 0x400)]
		TextPoint GetStartPoint([In, Optional] int Part );
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x10), TypeLibFunc((short) 0x400)]
		TextPoint GetEndPoint([In, Optional] int Part );
	}
	[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIDispatch), TypeLibType((short) 4160), Guid("0B48100A-473E-433C-AB8F-66B9739AB620")]
	public interface ProjectItem {
		[DispId(10)]
		bool IsDirty { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(10), TypeLibFunc((short) 0x40)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(10), TypeLibFunc((short) 0x40)] set; }
		[DispId(11)]
		string this[short index] { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(11)] get; }
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(12)]
		bool SaveAs([In, MarshalAs(UnmanagedType.BStr)] string NewFileName);
		[DispId(13)]
		short FileCount { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(13)] get; }
		[DispId(0)]
		string Name { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0)] set; }
		[DispId(0x36)]
		IUnknown Collection { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x36)] get; }
		[DispId(0x38)]
		IUnknown Properties { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x38)] get; }
		[DispId(200)]
		IUnknown DTE { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(200)] get; }
		[DispId(0xc9)]
		string Kind { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xc9)] get; }
		[DispId(0xcb)]
		IUnknown ProjectItems { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xcb)] get; }
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xcc)]
		bool get_IsOpen([In, Optional, MarshalAs(UnmanagedType.BStr)] string ViewKind );
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xcd)]
		IUnknown Open([In, Optional, MarshalAs(UnmanagedType.BStr)] string ViewKind );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xce)]
		void Remove();
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x6b)]
		void ExpandView();
		[DispId(0x6c)]
		object Object { [return: MarshalAs(UnmanagedType.IDispatch)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x6c)] get; }
		[return: MarshalAs(UnmanagedType.IDispatch)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x6d), TypeLibFunc((short) 0x400)]
		object get_Extender([In, MarshalAs(UnmanagedType.BStr)] string ExtenderName);
		[DispId(110)]
		object ExtenderNames { [return: MarshalAs(UnmanagedType.Struct)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(110), TypeLibFunc((short) 0x400)] get; }
		[DispId(0x6f)]
		string ExtenderCATID { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x6f), TypeLibFunc((short) 0x400)] get; }
		[DispId(0x71)]
		bool Saved { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x71)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x71)] set; }
		[DispId(0x74)]
		IUnknown ConfigurationManager { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x74)] get; }
		[DispId(0x75)]
		FileCodeModel FileCodeModel { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x75)] get; }
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x76)]
		void Save([Optional, MarshalAs(UnmanagedType.BStr)] string FileName );
		[DispId(0x77)]
		Document Document { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x77)] get; }
		[DispId(120)]
		IUnknown SubProject { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(120)] get; }
		[DispId(0x79)]
		IUnknown ContainingProject { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x79)] get; }
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x7a)]
		void Delete();
	}
	[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIDispatch), TypeLibType((short) 4160), Guid("ED1A3F99-4477-11D3-89BF-00C04F688DDE")]
	public interface FileCodeModel {
		[DispId(1)]
		IUnknown DTE { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(1)] get; }
		[DispId(2)]
		ProjectItem Parent { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(2)] get; }
		[DispId(3)]
		string Language { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(3)] get; }
		[DispId(4)]
		CodeElements CodeElements { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(4)] get; }
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(5)]
		CodeElement CodeElementFromPoint([MarshalAs(UnmanagedType.Interface)] TextPoint Point, int Scope);
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(7)]
		CodeNamespace AddNamespace([MarshalAs(UnmanagedType.BStr)] string Name, [Optional, MarshalAs(UnmanagedType.Struct)] object Position);
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(8)]
		CodeClass AddClass([MarshalAs(UnmanagedType.BStr)] string Name, [Optional, MarshalAs(UnmanagedType.Struct)] object Position, [Optional, MarshalAs(UnmanagedType.Struct)] object Bases, [Optional, MarshalAs(UnmanagedType.Struct)] object ImplementedInterfaces, [Optional] vsCMAccess Access );
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(9)]
		IUnknown AddInterface([MarshalAs(UnmanagedType.BStr)] string Name, [Optional, MarshalAs(UnmanagedType.Struct)] object Position, [Optional, MarshalAs(UnmanagedType.Struct)] object Bases, [Optional] vsCMAccess Access );
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(10)]
		CodeFunction AddFunction([MarshalAs(UnmanagedType.BStr)] string Name, vsCMFunction Kind, [MarshalAs(UnmanagedType.Struct)] object Type, [Optional, MarshalAs(UnmanagedType.Struct)] object Position, [Optional] vsCMAccess Access );
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(11)]
		IUnknown AddVariable([MarshalAs(UnmanagedType.BStr)] string Name, [MarshalAs(UnmanagedType.Struct)] object Type, [Optional, MarshalAs(UnmanagedType.Struct)] object Position, [Optional] vsCMAccess Access );
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(12)]
		IUnknown AddAttribute([MarshalAs(UnmanagedType.BStr)] string Name, [MarshalAs(UnmanagedType.BStr)] string Value, [Optional, MarshalAs(UnmanagedType.Struct)] object Position);
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(13)]
		IUnknown AddStruct([MarshalAs(UnmanagedType.BStr)] string Name, [Optional, MarshalAs(UnmanagedType.Struct)] object Position, [Optional, MarshalAs(UnmanagedType.Struct)] object Bases, [Optional, MarshalAs(UnmanagedType.Struct)] object ImplementedInterfaces, [Optional] vsCMAccess Access );
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(14)]
		IUnknown AddEnum([MarshalAs(UnmanagedType.BStr)] string Name, [Optional, MarshalAs(UnmanagedType.Struct)] object Position, [Optional, MarshalAs(UnmanagedType.Struct)] object Bases, [Optional] vsCMAccess Access );
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(15)]
		IUnknown AddDelegate([MarshalAs(UnmanagedType.BStr)] string Name, [MarshalAs(UnmanagedType.Struct)] object Type, [Optional, MarshalAs(UnmanagedType.Struct)] object Position, [Optional] vsCMAccess Access );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x10)]
		void Remove([MarshalAs(UnmanagedType.Struct)] object Element);
	}
	[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIDispatch), TypeLibType((short) 4160), Guid("0CFBC2B8-0D4E-11D3-8997-00C04F688DDE")]
	public interface CodeNamespace {
		[DispId(1)]
		IUnknown DTE { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(1), TypeLibFunc((short) 0x400)] get; }
		[DispId(2)]
		CodeElements Collection { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(2), TypeLibFunc((short) 0x400)] get; }
		[DispId(0)]
		string Name { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0)] get; [param: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0)] set; }
		[DispId(3)]
		string FullName { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(3)] get; }
		[DispId(4)]
		ProjectItem ProjectItem { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x400), DispId(4)] get; }
		[DispId(5)]
		int Kind { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x400), DispId(5)] get; }
		[DispId(6)]
		bool IsCodeType { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(6), TypeLibFunc((short) 0x400)] get; }
		[DispId(7)]
		int InfoLocation { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x400), DispId(7)] get; }
		[DispId(8)]
		CodeElements Children { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(8), TypeLibFunc((short) 0x400)] get; }
		[DispId(9)]
		string Language { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x400), DispId(9)] get; }
		[DispId(10)]
		TextPoint StartPoint { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x400), DispId(10)] get; }
		[DispId(11)]
		TextPoint EndPoint { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x400), DispId(11)] get; }
		[DispId(12)]
		object ExtenderNames { [return: MarshalAs(UnmanagedType.Struct)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x400), DispId(12)] get; }
		[DispId(13)]
		object this[string ExtenderName] { [return: MarshalAs(UnmanagedType.IDispatch)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(13), TypeLibFunc((short) 0x400)] get; }
		[DispId(14)]
		string ExtenderCATID { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(14), TypeLibFunc((short) 0x400)] get; }
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(15), TypeLibFunc((short) 0x400)]
		TextPoint GetStartPoint([In, Optional] int Part );
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x10), TypeLibFunc((short) 0x400)]
		TextPoint GetEndPoint([In, Optional] int Part );
		[DispId(0x1f)]
		object Parent { [return: MarshalAs(UnmanagedType.IDispatch)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x400), DispId(0x1f)] get; }
		[DispId(0x20)]
		CodeElements Members { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x400), DispId(0x20)] get; }
		[DispId(0x23)]
		string DocComment { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x23), TypeLibFunc((short) 0x400)] get; [param: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x23), TypeLibFunc((short) 0x400)] set; }
		[DispId(0x24)]
		string Comment { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x24), TypeLibFunc((short) 0x400)] get; [param: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x400), DispId(0x24)] set; }
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x25)]
		CodeNamespace AddNamespace([MarshalAs(UnmanagedType.BStr)] string Name, [Optional, MarshalAs(UnmanagedType.Struct)] object Position);
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x26)]
		CodeClass AddClass([MarshalAs(UnmanagedType.BStr)] string Name, [Optional, MarshalAs(UnmanagedType.Struct)] object Position, [Optional, MarshalAs(UnmanagedType.Struct)] object Bases, [Optional, MarshalAs(UnmanagedType.Struct)] object ImplementedInterfaces, [Optional] vsCMAccess Access );
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x27)]
		IUnknown AddInterface([MarshalAs(UnmanagedType.BStr)] string Name, [Optional, MarshalAs(UnmanagedType.Struct)] object Position, [Optional, MarshalAs(UnmanagedType.Struct)] object Bases, [Optional] vsCMAccess Access );
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(40)]
		IUnknown AddStruct([MarshalAs(UnmanagedType.BStr)] string Name, [Optional, MarshalAs(UnmanagedType.Struct)] object Position, [Optional, MarshalAs(UnmanagedType.Struct)] object Bases, [Optional, MarshalAs(UnmanagedType.Struct)] object ImplementedInterfaces, [Optional] vsCMAccess Access );
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x29)]
		IUnknown AddEnum([MarshalAs(UnmanagedType.BStr)] string Name, [Optional, MarshalAs(UnmanagedType.Struct)] object Position, [Optional, MarshalAs(UnmanagedType.Struct)] object Bases, [Optional] vsCMAccess Access );
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x2a)]
		IUnknown AddDelegate([MarshalAs(UnmanagedType.BStr)] string Name, [MarshalAs(UnmanagedType.Struct)] object Type, [Optional, MarshalAs(UnmanagedType.Struct)] object Position, [Optional] vsCMAccess Access );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x2b)]
		void Remove([MarshalAs(UnmanagedType.Struct)] object Element);
	}
	[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIDispatch), Guid("B1F42514-91CD-4D3A-8B25-A317D8032B24"), TypeLibType((short) 4160)]
	public interface CodeClass {
		[DispId(1)]
		IUnknown DTE { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x400), DispId(1)] get; }
		[DispId(2)]
		CodeElements Collection { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(2), TypeLibFunc((short) 0x400)] get; }
		[DispId(0)]
		string Name { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0)] get; [param: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0)] set; }
		[DispId(3)]
		string FullName { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(3)] get; }
		[DispId(4)]
		ProjectItem ProjectItem { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(4), TypeLibFunc((short) 0x400)] get; }
		[DispId(5)]
		int Kind { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(5), TypeLibFunc((short) 0x400)] get; }
		[DispId(6)]
		bool IsCodeType { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x400), DispId(6)] get; }
		[DispId(7)]
		int InfoLocation { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x400), DispId(7)] get; }
		[DispId(8)]
		CodeElements Children { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(8), TypeLibFunc((short) 0x400)] get; }
		[DispId(9)]
		string Language { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(9), TypeLibFunc((short) 0x400)] get; }
		[DispId(10)]
		TextPoint StartPoint { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(10), TypeLibFunc((short) 0x400)] get; }
		[DispId(11)]
		TextPoint EndPoint { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x400), DispId(11)] get; }
		[DispId(12)]
		object ExtenderNames { [return: MarshalAs(UnmanagedType.Struct)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x400), DispId(12)] get; }
		[return: MarshalAs(UnmanagedType.IDispatch)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x400), DispId(13)]
		object get_Extender([MarshalAs(UnmanagedType.BStr)] string ExtenderName);
		[DispId(14)]
		string ExtenderCATID { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x400), DispId(14)] get; }
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(15), TypeLibFunc((short) 0x400)]
		TextPoint GetStartPoint([In, Optional] vsCMPart Part );
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x10), TypeLibFunc((short) 0x400)]
		TextPoint GetEndPoint([In, Optional] vsCMPart Part );
		[DispId(0x1f)]
		object Parent { [return: MarshalAs(UnmanagedType.IDispatch)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x1f), TypeLibFunc((short) 0x400)] get; }
		[DispId(0x20)]
		CodeNamespace Namespace { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x20), TypeLibFunc((short) 0x400)] get; }
		[DispId(0x21)]
		CodeElements Bases { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x400), DispId(0x21)] get; }
		[DispId(0x22)]
		CodeElements Members { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x400), DispId(0x22)] get; }
		[DispId(0x23)]
		vsCMAccess Access { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x23)] get; [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x23)] set; }
		[DispId(0x24)]
		CodeElements Attributes { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x24), TypeLibFunc((short) 0x400)] get; }
		[DispId(0x25)]
		string DocComment { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x25), TypeLibFunc((short) 0x400)] get; [param: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x25), TypeLibFunc((short) 0x400)] set; }
		[DispId(0x26)]
		string Comment { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x26), TypeLibFunc((short) 0x400)] get; [param: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x400), DispId(0x26)] set; }
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x27)]
		CodeElement AddBase([MarshalAs(UnmanagedType.Struct)] object Base, [Optional, MarshalAs(UnmanagedType.Struct)] object Position);
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(40)]
		IUnknown AddAttribute([MarshalAs(UnmanagedType.BStr)] string Name, [MarshalAs(UnmanagedType.BStr)] string Value, [Optional, MarshalAs(UnmanagedType.Struct)] object Position);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x29)]
		void RemoveBase([MarshalAs(UnmanagedType.Struct)] object Element);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x2a)]
		void RemoveMember([MarshalAs(UnmanagedType.Struct)] object Element);
		[DispId(0x2b)]
		bool this[string FullName] { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x2b)] get; }
		[DispId(0x2c)]
		CodeElements DerivedTypes { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x400), DispId(0x2c)] get; }
		[DispId(0x3d)]
		CodeElements ImplementedInterfaces { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x3d), TypeLibFunc((short) 0x400)] get; }
		[DispId(0x3e)]
		bool IsAbstract { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x3e)] get; [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x3e)] set; }
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x3f)]
		IUnknown AddImplementedInterface([MarshalAs(UnmanagedType.Struct)] object Base, [Optional, MarshalAs(UnmanagedType.Struct)] object Position);
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x40)]
		CodeFunction AddFunction([MarshalAs(UnmanagedType.BStr)] string Name, vsCMFunction Kind, [MarshalAs(UnmanagedType.Struct)] object Type, [Optional, MarshalAs(UnmanagedType.Struct)] object Position, [Optional] vsCMAccess Access , [Optional, MarshalAs(UnmanagedType.Struct)] object Location);
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x41)]
		IUnknown AddVariable([MarshalAs(UnmanagedType.BStr)] string Name, [MarshalAs(UnmanagedType.Struct)] object Type, [Optional, MarshalAs(UnmanagedType.Struct)] object Position, [Optional] vsCMAccess Access , [Optional, MarshalAs(UnmanagedType.Struct)] object Location);
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x42)]
		IUnknown AddProperty([MarshalAs(UnmanagedType.BStr)] string GetterName, [MarshalAs(UnmanagedType.BStr)] string PutterName, [MarshalAs(UnmanagedType.Struct)] object Type, [Optional, MarshalAs(UnmanagedType.Struct)] object Position, [Optional] vsCMAccess Access , [Optional, MarshalAs(UnmanagedType.Struct)] object Location);
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x43)]
		CodeClass AddClass([MarshalAs(UnmanagedType.BStr)] string Name, [Optional, MarshalAs(UnmanagedType.Struct)] object Position, [Optional, MarshalAs(UnmanagedType.Struct)] object Bases, [Optional, MarshalAs(UnmanagedType.Struct)] object ImplementedInterfaces, [Optional] vsCMAccess Access );
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x44)]
		IUnknown AddStruct([MarshalAs(UnmanagedType.BStr)] string Name, [Optional, MarshalAs(UnmanagedType.Struct)] object Position, [Optional, MarshalAs(UnmanagedType.Struct)] object Bases, [Optional, MarshalAs(UnmanagedType.Struct)] object ImplementedInterfaces, [Optional] vsCMAccess Access );
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x45)]
		IUnknown AddEnum([MarshalAs(UnmanagedType.BStr)] string Name, [Optional, MarshalAs(UnmanagedType.Struct)] object Position, [Optional, MarshalAs(UnmanagedType.Struct)] object Bases, [Optional] vsCMAccess Access );
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(70)]
		IUnknown AddDelegate([MarshalAs(UnmanagedType.BStr)] string Name, [MarshalAs(UnmanagedType.Struct)] object Type, [Optional, MarshalAs(UnmanagedType.Struct)] object Position, [Optional] vsCMAccess Access );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x47)]
		void RemoveInterface([MarshalAs(UnmanagedType.Struct)] object Element);
	}
	[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIDispatch), TypeLibType((short) 4160), Guid("0CFBC2B9-0D4E-11D3-8997-00C04F688DDE")]
	public interface CodeFunction {
		[DispId(1)]
		IUnknown DTE { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(1), TypeLibFunc((short) 0x400)] get; }
		[DispId(2)]
		CodeElements Collection { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(2), TypeLibFunc((short) 0x400)] get; }
		[DispId(0)]
		string Name { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0)] get; [param: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0)] set; }
		[DispId(3)]
		string FullName { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(3)] get; }
		[DispId(4)]
		ProjectItem ProjectItem { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(4), TypeLibFunc((short) 0x400)] get; }
		[DispId(5)]
		int Kind { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(5), TypeLibFunc((short) 0x400)] get; }
		[DispId(6)]
		bool IsCodeType { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(6), TypeLibFunc((short) 0x400)] get; }
		[DispId(7)]
		int InfoLocation { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x400), DispId(7)] get; }
		[DispId(8)]
		CodeElements Children { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x400), DispId(8)] get; }
		[DispId(9)]
		string Language { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x400), DispId(9)] get; }
		[DispId(10)]
		TextPoint StartPoint { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x400), DispId(10)] get; }
		[DispId(11)]
		TextPoint EndPoint { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x400), DispId(11)] get; }
		[DispId(12)]
		object ExtenderNames { [return: MarshalAs(UnmanagedType.Struct)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(12), TypeLibFunc((short) 0x400)] get; }
		[DispId(13)]
		object this[string ExtenderName] { [return: MarshalAs(UnmanagedType.IDispatch)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x400), DispId(13)] get; }
		[DispId(14)]
		string ExtenderCATID { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(14), TypeLibFunc((short) 0x400)] get; }
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x400), DispId(15)]
		TextPoint GetStartPoint([In, Optional] vsCMPart Part );
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x400), DispId(0x10)]
		TextPoint GetEndPoint([In, Optional] vsCMPart Part );
		[DispId(0x1f)]
		object Parent { [return: MarshalAs(UnmanagedType.IDispatch)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x400), DispId(0x1f)] get; }
		[DispId(0x20)]
		vsCMFunction FunctionKind { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x20), TypeLibFunc((short) 0x400)] get; }
		[DispId(0x22)]
		string this[int Flags] { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x400), DispId(0x22)] get; }
		[DispId(0x23)]
		IUnknown Type { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x23), TypeLibFunc((short) 0x400)] get; [param: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x400), DispId(0x23)] set; }
		[DispId(0x24)]
		CodeElements Parameters { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x400), DispId(0x24)] get; }
		[DispId(0x26)]
		vsCMAccess Access { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x26)] get; [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x26)] set; }
		[DispId(0x27)]
		bool IsOverloaded { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x27)] get; }
		[DispId(40)]
		bool IsShared { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(40)] get; [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(40)] set; }
		[DispId(0x29)]
		bool MustImplement { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x29)] get; [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x29)] set; }
		[DispId(0x2a)]
		CodeElements Overloads { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x2a), TypeLibFunc((short) 0x400)] get; }
		[DispId(0x2b)]
		CodeElements Attributes { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x2b), TypeLibFunc((short) 0x400)] get; }
		[DispId(0x2c)]
		string DocComment { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x400), DispId(0x2c)] get; [param: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x2c), TypeLibFunc((short) 0x400)] set; }
		[DispId(0x2d)]
		string Comment { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x2d), TypeLibFunc((short) 0x400)] get; [param: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x400), DispId(0x2d)] set; }
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x2f)]
		IUnknown AddParameter([MarshalAs(UnmanagedType.BStr)] string Name, [MarshalAs(UnmanagedType.Struct)] object Type, [Optional, MarshalAs(UnmanagedType.Struct)] object Position);
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x30)]
		IUnknown AddAttribute([MarshalAs(UnmanagedType.BStr)] string Name, [MarshalAs(UnmanagedType.BStr)] string Value, [Optional, MarshalAs(UnmanagedType.Struct)] object Position);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x31)]
		void RemoveParameter([MarshalAs(UnmanagedType.Struct)] object Element);
		[DispId(50)]
		bool CanOverride { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(50)] get; [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(50)] set; }
	}
	[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIDispatch), DefaultMember("Name"), Guid("63EB5C39-CA8F-498E-9A66-6DD4A27AC95B"), TypeLibType((short) 4160)]
	public interface Document {
		[DispId(100)]
		object DTE { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(100)] get; }
		[DispId(0x8d)]
		string Kind { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x8d)] get; }
		[DispId(0x65)]
		Documents Collection { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x65)] get; }
		[DispId(0x66)]
		object ActiveWindow { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x66)] get; }
		[DispId(0x67)]
		string FullName { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x67)] get; }
		[DispId(0)]
		string Name { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0)] get; }
		[DispId(0x69)]
		string Path { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x69)] get; }
		[DispId(0x6a)]
		bool ReadOnly { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x6a)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x40), DispId(0x6a)] set; }
		[DispId(0x6b)]
		bool Saved { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x6b)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x6b)] set; }
		[DispId(0x6d)]
		object Windows { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x6d)] get; }
		[DispId(110)]
		ProjectItem ProjectItem { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(110)] get; }
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x79)]
		void Activate();
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x7b)]
		void Close([In, Optional] int Save );
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x7d)]
		object NewWindow();
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x7f)]
		bool Redo();
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x80)]
		bool Undo();
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x81)]
		int Save([In, Optional, MarshalAs(UnmanagedType.BStr)] string FileName );
		[DispId(0x83)]
		object Selection { [return: MarshalAs(UnmanagedType.IDispatch)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x83)] get; }
		[return: MarshalAs(UnmanagedType.IDispatch)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x84)]
		object Object([In, Optional, MarshalAs(UnmanagedType.BStr)] string ModelKind );
		[return: MarshalAs(UnmanagedType.IDispatch)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x85), TypeLibFunc((short) 0x400)]
		object get_Extender([In, MarshalAs(UnmanagedType.BStr)] string ExtenderName);
		[DispId(0x86)]
		object ExtenderNames { [return: MarshalAs(UnmanagedType.Struct)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x400), DispId(0x86)] get; }
		[DispId(0x87)]
		string ExtenderCATID { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x87), TypeLibFunc((short) 0x400)] get; }
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x7e), TypeLibFunc((short) 0x40)]
		void PrintOut();
		[DispId(0x8e)]
		int IndentSize { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x8e), TypeLibFunc((short) 0x40)] get; }
		[DispId(0x90)]
		string Language { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x40), DispId(0x90)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x40), DispId(0x90)] set; }
		[DispId(0x93)]
		int TabSize { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x93), TypeLibFunc((short) 0x40)] get; }
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x7a), TypeLibFunc((short) 0x40)]
		void ClearBookmarks();
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x7c), TypeLibFunc((short) 0x40)]
		bool MarkText([In, MarshalAs(UnmanagedType.BStr)] string Pattern, [In, Optional] int Flags );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x15), TypeLibFunc((short) 0x40)]
		bool ReplaceText([In, MarshalAs(UnmanagedType.BStr)] string FindText, [In, MarshalAs(UnmanagedType.BStr)] string ReplaceText, [In, Optional] int Flags );
		[DispId(0x95)]
		string Type { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x40), DispId(0x95)] get; }
	}
	[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIDispatch), Guid("CB218890-1382-472B-9118-782700C88115"), TypeLibType((short) 4160)]
	public interface TextDocument {
		[DispId(150)]
		IUnknown DTE { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(150)] get; }
		[DispId(0x97)]
		Document Parent { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x97)] get; }
		[DispId(1)]
		TextSelection Selection { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(1)] get; }
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x7a)]
		void ClearBookmarks();
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x7c)]
		bool MarkText([In, MarshalAs(UnmanagedType.BStr)] string Pattern, [In, Optional] int vsFindOptionsValue );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x80)]
		bool ReplacePattern([In, MarshalAs(UnmanagedType.BStr)] string Pattern, [In, MarshalAs(UnmanagedType.BStr)] string Replace, [In, Optional] int vsFindOptionsValue , [In, Out, Optional, MarshalAs(UnmanagedType.Interface)] ref TextRanges Tags );
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x83)]
		EditPoint CreateEditPoint([In, Optional, MarshalAs(UnmanagedType.Interface)] TextPoint TextPoint );
		[DispId(0x84)]
		TextPoint StartPoint { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x84)] get; }
		[DispId(0x85)]
		TextPoint EndPoint { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x85)] get; }
		[DispId(0x89)]
		string Language { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x89), TypeLibFunc((short) 0x40)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x89), TypeLibFunc((short) 0x40)] set; }
		[DispId(0x91)]
		string Type { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x91), TypeLibFunc((short) 0x40)] get; }
		[DispId(0x87)]
		int IndentSize { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x40), DispId(0x87)] get; }
		[DispId(140)]
		int TabSize { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x40), DispId(140)] get; }
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x90), TypeLibFunc((short) 0x40)]
		bool ReplaceText([In, MarshalAs(UnmanagedType.BStr)] string FindText, [In, MarshalAs(UnmanagedType.BStr)] string ReplaceText, [In, Optional] int vsFindOptionsValue );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x86), TypeLibFunc((short) 0x40)]
		void PrintOut();
	}
	[ComImport, TypeLibType((short) 4160), DefaultMember("Text"), Guid("1FA0E135-399A-4D2C-A4FE-D21E2480F921")]
	public interface TextSelection {
		[DispId(1)]
		IUnknown DTE { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(1)] get; }
		[DispId(2)]
		TextDocument Parent { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(2)] get; }
		[DispId(3)]
		IUnknown AnchorPoint { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(3)] get; }
		[DispId(4)]
		IUnknown ActivePoint { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(4)] get; }
		[DispId(5)]
		int AnchorColumn { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x40), DispId(5)] get; }
		[DispId(6)]
		int BottomLine { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(6), TypeLibFunc((short) 0x40)] get; }
		[DispId(7)]
		IUnknown BottomPoint { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(7)] get; }
		[DispId(8)]
		int CurrentColumn { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(8), TypeLibFunc((short) 0x40)] get; }
		[DispId(9)]
		int CurrentLine { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x40), DispId(9)] get; }
		[DispId(10)]
		bool IsEmpty { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(10)] get; }
		[DispId(11)]
		bool IsActiveEndGreater { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(11)] get; }
		[DispId(0)]
		string Text { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0)] set; }
		[DispId(13)]
		int TopLine { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(13), TypeLibFunc((short) 0x40)] get; }
		[DispId(14)]
		IUnknown TopPoint { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(14)] get; }
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(15)]
		void ChangeCase([In] int How);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x10)]
		void CharLeft([In, Optional] bool Extend , [In, Optional] int Count );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x11)]
		void CharRight([In, Optional] bool Extend , [In, Optional] int Count );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x12)]
		void ClearBookmark();
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x13)]
		void Collapse();
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x48)]
		void OutlineSection();
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(20)]
		void Copy();
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x15)]
		void Cut();
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x16)]
		void Paste();
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x17)]
		void Delete([In, Optional] int Count );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x18)]
		void DeleteLeft([In, Optional] int Count );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x19)]
		void DeleteWhitespace([In, Optional] int Direction );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x1a)]
		void EndOfLine([In, Optional] bool Extend );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x1b)]
		void StartOfLine([In, Optional] int Where , [In, Optional] bool Extend );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x1c)]
		void EndOfDocument([In, Optional] bool Extend );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x1d)]
		void StartOfDocument([In, Optional] bool Extend );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(30)]
		bool FindPattern([In, MarshalAs(UnmanagedType.BStr)] string Pattern, [In, Optional] int vsFindOptionsValue , [In, Out, Optional, MarshalAs(UnmanagedType.Interface)] ref TextRanges Tags );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x1f)]
		bool ReplacePattern([In, MarshalAs(UnmanagedType.BStr)] string Pattern, [In, MarshalAs(UnmanagedType.BStr)] string Replace, [In, Optional] int vsFindOptionsValue , [In, Out, Optional, MarshalAs(UnmanagedType.Interface)] ref TextRanges Tags );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(70)]
		bool FindText([In, MarshalAs(UnmanagedType.BStr)] string Pattern, [In, Optional] int vsFindOptionsValue );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x40), DispId(0x47)]
		bool ReplaceText([In, MarshalAs(UnmanagedType.BStr)] string Pattern, [In, MarshalAs(UnmanagedType.BStr)] string Replace, [In, Optional] int vsFindOptionsValue );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x20)]
		void GotoLine([In] int Line, [In, Optional] bool Select );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x21)]
		void Indent([In, Optional] int Count );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x22)]
		void Unindent([In, Optional] int Count );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x23)]
		void Insert([In, MarshalAs(UnmanagedType.BStr)] string Text, [In, Optional] int vsInsertFlagsCollapseToEndValue );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x3d)]
		void InsertFromFile([In, MarshalAs(UnmanagedType.BStr)] string File);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x24)]
		void LineDown([In, Optional] bool Extend , [In, Optional] int Count );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x25)]
		void LineUp([In, Optional] bool Extend , [In, Optional] int Count );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x26)]
		void MoveToPoint([In, MarshalAs(UnmanagedType.Interface)] TextPoint Point, [In, Optional] bool Extend );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x27)]
		void MoveToLineAndOffset([In] int Line, [In] int Offset, [In, Optional] bool Extend );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(40)]
		void MoveToAbsoluteOffset([In] int Offset, [In, Optional] bool Extend );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x29)]
		void NewLine([In, Optional] int Count );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x2a)]
		void SetBookmark();
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x2b)]
		bool NextBookmark();
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x2c)]
		bool PreviousBookmark();
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x2d)]
		void PadToColumn([In] int Column);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x2e)]
		void SmartFormat();
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x2f)]
		void SelectAll();
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x30)]
		void SelectLine();
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x31)]
		void SwapAnchor();
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(50)]
		void Tabify();
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x33)]
		void Untabify();
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x34)]
		void WordLeft([In, Optional] bool Extend , [In, Optional] int Count );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x35)]
		void WordRight([In, Optional] bool Extend , [In, Optional] int Count );
		[DispId(0x36)]
		IUnknown TextPane { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x36)] get; }
		[DispId(0x37)]
		int Mode { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x37)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x37)] set; }
		[DispId(0x38)]
		IUnknown TextRanges { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x38)] get; }
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x40), DispId(80)]
		void Backspace([In, Optional] int Count );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x40), DispId(0x51)]
		void Cancel();
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x39)]
		void DestructiveInsert([In, MarshalAs(UnmanagedType.BStr)] string Text);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x52), TypeLibFunc((short) 0x40)]
		void MoveTo([In] int Line, [In] int Column, [In, Optional] bool Extend );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x3a)]
		void MoveToDisplayColumn([In] int Line, [In] int Column, [In, Optional] bool Extend );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x3b)]
		void PageUp([In, Optional] bool Extend , [In, Optional] int Count );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(60)]
		void PageDown([In, Optional] bool Extend , [In, Optional] int Count );
	}
	[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIDispatch), TypeLibType((short) 4160), Guid("04A72314-32E9-48E2-9B87-A63603454F3E")]
	public interface _DTE {
		[DispId(0)]
		string Name { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0)] get; }
		[DispId(10)]
		string FileName { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x40), DispId(10)] get; }
		[DispId(100)]
		string Version { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(100)] get; }
		[DispId(0x6c)]
		IUnknown CommandBars { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x6c)] get; }
		[DispId(110)]
		IUnknown Windows { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(110)] get; }
		[DispId(0x6f)]
		IUnknown Events { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x6f)] get; }
		[DispId(200)]
		IUnknown AddIns { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(200)] get; }
		[DispId(0xcc)]
		IUnknown MainWindow { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xcc)] get; }
		[DispId(0xcd)]
		IUnknown ActiveWindow { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xcd)] get; }
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xcf)]
		void Quit();
		[DispId(0xd0)]
		int DisplayMode { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xd0)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xd0)] set; }
		[DispId(0xd1)]
		IUnknown Solution { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xd1)] get; }
		[DispId(210)]
		IUnknown Commands { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(210)] get; }
		[return: MarshalAs(UnmanagedType.IDispatch)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xd3)]
		object GetObject([In, MarshalAs(UnmanagedType.BStr)] string Name);
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xd4)]
		IUnknown get_Properties([MarshalAs(UnmanagedType.BStr)] string Category, [MarshalAs(UnmanagedType.BStr)] string Page);
		[DispId(0xd5)]
		IUnknown SelectedItems { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xd5)] get; }
		[DispId(0xd6)]
		string CommandLineArguments { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xd6)] get; }
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xd7), TypeLibFunc((short) 0x40)]
		IUnknown OpenFile([In, MarshalAs(UnmanagedType.BStr)] string ViewKind, [In, MarshalAs(UnmanagedType.BStr)] string FileName);
		[DispId(0xd8)]
		bool this[string ViewKind, string FileName] { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x40), DispId(0xd8)] get; }
		[DispId(0xd9)]
		IUnknown DTE { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xd9)] get; }
		[DispId(0xda)]
		int LocaleID { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xda)] get; }
		[DispId(0xdb)]
		IUnknown WindowConfigurations { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xdb)] get; }
		[DispId(220)]
		IUnknown Documents { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(220)] get; }
		[DispId(0xdd)]
		Document ActiveDocument { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xdd)] get; }
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xde)]
		void ExecuteCommand([In, MarshalAs(UnmanagedType.BStr)] string CommandName, [In, Optional, MarshalAs(UnmanagedType.BStr)] string CommandArgs );
		[DispId(0xdf)]
		IUnknown Globals { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xdf)] get; }
		[DispId(0xe1)]
		IUnknown StatusBar { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xe1)] get; }
		[DispId(0xe2)]
		string FullName { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xe2)] get; }
		[DispId(0xe3)]
		bool UserControl { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xe3)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xe3)] set; }
		[DispId(0xe4)]
		IUnknown ObjectExtenders { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xe4)] get; }
		[DispId(0xe5)]
		IUnknown Find { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xe5)] get; }
		[DispId(230)]
		int Mode { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(230)] get; }
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xe8)]
		int LaunchWizard([In, MarshalAs(UnmanagedType.BStr)] string VSZFile, [In, MarshalAs(UnmanagedType.SafeArray, SafeArraySubType=VarEnum.VT_VARIANT)] ref object[] ContextParams);
		[DispId(0xe9)]
		IUnknown ItemOperations { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xe9)] get; }
		[DispId(0xeb)]
		IUnknown UndoContext { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xeb)] get; }
		[DispId(0xec)]
		IUnknown Macros { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xec)] get; }
		[DispId(0xed)]
		object ActiveSolutionProjects { [return: MarshalAs(UnmanagedType.Struct)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xed)] get; }
		[DispId(0xee)]
		IUnknown MacrosIDE { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xee)] get; }
		[DispId(0xef)]
		string RegistryRoot { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xef)] get; }
		[DispId(240)]
		IUnknown Application { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(240), TypeLibFunc((short) 0x40)] get; }
		[DispId(0xf1)]
		IUnknown ContextAttributes { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xf1)] get; }
		[DispId(0xf2)]
		IUnknown SourceControl { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xf2)] get; }
		[DispId(0xf3)]
		bool SuppressUI { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xf3)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xf3)] set; }
		[DispId(0xf4)]
		IUnknown Debugger { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xf4)] get; }
		[return: MarshalAs(UnmanagedType.BStr)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xf5)]
		string SatelliteDllPath([MarshalAs(UnmanagedType.BStr)] string Path, [MarshalAs(UnmanagedType.BStr)] string Name);
		[DispId(0xf6)]
		string Edition { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0xf6)] get; }
	}
	[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIDispatch), Guid("9E2CF3EA-140F-413E-BD4B-7D46740CD2F4"), TypeLibType((short) 4160), DefaultMember("Item")]
	public interface Documents {
		[DispId(100)]
		IUnknown DTE { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(100)] get; }
		[DispId(0x65)]
		IUnknown Parent { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x65)] get; }
		[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType="",  MarshalCookie="")]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 1), DispId(-4)]
		IEnumerator GetEnumerator();
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0)]
		Document Item([In, MarshalAs(UnmanagedType.Struct)] object index);
		[DispId(3)]
		int Count { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(3)] get; }
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x40), DispId(4)]
		Document Add([In, MarshalAs(UnmanagedType.BStr)] string Kind);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(5)]
		void CloseAll([In, Optional] int Save );
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(6)]
		void SaveAll();
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x40), DispId(7)]
		Document Open([MarshalAs(UnmanagedType.BStr)] string PathName, [Optional, MarshalAs(UnmanagedType.BStr)] string Kind , [Optional] bool ReadOnly );
	}
	[Guid("B1AB3125-0744-4B46-AA7A-8902E36D2E15")]
	public abstract class Constants {
		[TypeLibVar((short) 0x40), MarshalAs(UnmanagedType.LPStr)]
		public const string dsCPP = "C/C++";
		[TypeLibVar((short) 0x40), MarshalAs(UnmanagedType.LPStr)]
		public const string dsFortran_Fixed = "Fortran Fixed";
		[TypeLibVar((short) 0x40), MarshalAs(UnmanagedType.LPStr)]
		public const string dsFortran_Free = "Fortran Free";
		[TypeLibVar((short) 0x40), MarshalAs(UnmanagedType.LPStr)]
		public const string dsHTML_IE3 = "HTML - IE 3.0";
		[TypeLibVar((short) 0x40), MarshalAs(UnmanagedType.LPStr)]
		public const string dsHTML_RFC1866 = "HTML 2.0 (RFC 1866)";
		[TypeLibVar((short) 0x40), MarshalAs(UnmanagedType.LPStr)]
		public const string dsIDL = "ODL/IDL";
		[TypeLibVar((short) 0x40), MarshalAs(UnmanagedType.LPStr)]
		public const string dsJava = "Java";
		[TypeLibVar((short) 0x40), MarshalAs(UnmanagedType.LPStr)]
		public const string dsVBSMacro = "VBS Macro";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsAddInCmdGroup = "{1E58696E-C90F-11D2-AAB2-00C04F688DDE}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsCATIDDocument = "{610d4611-d0d5-11d2-8599-006097c68e81}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsCATIDGenericProject = "{610d4616-d0d5-11d2-8599-006097c68e81}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsCATIDMiscFilesProject = "{610d4612-d0d5-11d2-8599-006097c68e81}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsCATIDMiscFilesProjectItem = "{610d4613-d0d5-11d2-8599-006097c68e81}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsCATIDSolution = "{52AEFF70-BBD8-11d2-8598-006097C68E81}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsCATIDSolutionBrowseObject = "{A2392464-7C22-11d3-BDCA-00C04F688E50}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsContextDebugging = "{ADFC4E61-0397-11D1-9F4E-00A0C911004F}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsContextDesignMode = "{ADFC4E63-0397-11D1-9F4E-00A0C911004F}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsContextEmptySolution = "{ADFC4E65-0397-11D1-9F4E-00A0C911004F}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsContextFullScreenMode = "{ADFC4E62-0397-11D1-9F4E-00A0C911004F}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsContextMacroRecording = "{04BBF6A5-4697-11D2-890E-0060083196C6}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsContextMacroRecordingToolbar = "{85A70471-270A-11D2-88F9-0060083196C6}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsContextNoSolution = "{ADFC4E64-0397-11D1-9F4E-00A0C911004F}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsContextSolutionBuilding = "{ADFC4E60-0397-11D1-9F4E-00A0C911004F}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsContextSolutionHasMultipleProjects = "{93694FA0-0397-11D1-9F4E-00A0C911004F}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsContextSolutionHasSingleProject = "{ADFC4E66-0397-11D1-9F4E-00A0C911004F}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsDocumentKindBinary = "{25834150-CD7E-11D0-92DF-00A0C9138C45}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsDocumentKindHTML = "{C76D83F8-A489-11D0-8195-00A0C91BBEE3}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsDocumentKindResource = "{00000000-0000-0000-0000-000000000000}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsDocumentKindText = "{8E7B96A8-E33D-11D0-A6D5-00C04FB67F6A}";
		[TypeLibVar((short) 0x40), MarshalAs(UnmanagedType.LPStr)]
		public const string vsext_GUID_AddItemWizard = "{0F90E1D1-4999-11D1-B6D1-00A0C90F2744}";
		[TypeLibVar((short) 0x40), MarshalAs(UnmanagedType.LPStr)]
		public const string vsext_GUID_NewProjectWizard = "{0F90E1D0-4999-11D1-B6D1-00A0C90F2744}";
		[TypeLibVar((short) 0x40), MarshalAs(UnmanagedType.LPStr)]
		public const string vsext_vk_Code = "{7651A701-06E5-11D1-8EBD-00A0C90F26EA}";
		[TypeLibVar((short) 0x40), MarshalAs(UnmanagedType.LPStr)]
		public const string vsext_vk_Debugging = "{7651A700-06E5-11D1-8EBD-00A0C90F26EA}";
		[TypeLibVar((short) 0x40), MarshalAs(UnmanagedType.LPStr)]
		public const string vsext_vk_Designer = "{7651A702-06E5-11D1-8EBD-00A0C90F26EA}";
		[TypeLibVar((short) 0x40), MarshalAs(UnmanagedType.LPStr)]
		public const string vsext_vk_Primary = "{00000000-0000-0000-0000-000000000000}";
		[TypeLibVar((short) 0x40), MarshalAs(UnmanagedType.LPStr)]
		public const string vsext_vk_TextView = "{7651A703-06E5-11D1-8EBD-00A0C90F26EA}";
		[TypeLibVar((short) 0x40), MarshalAs(UnmanagedType.LPStr)]
		public const string vsext_wk_AutoLocalsWindow = "{F2E84780-2AF1-11D1-A7FA-00A0C9110051}";
		[TypeLibVar((short) 0x40), MarshalAs(UnmanagedType.LPStr)]
		public const string vsext_wk_CallStackWindow = "{0504FF91-9D61-11D0-A794-00A0C9110051}";
		[TypeLibVar((short) 0x40), MarshalAs(UnmanagedType.LPStr)]
		public const string vsext_wk_ClassView = "{C9C0AE26-AA77-11D2-B3F0-0000F87570EE}";
		[TypeLibVar((short) 0x40), MarshalAs(UnmanagedType.LPStr)]
		public const string vsext_wk_ContextWindow = "{66DBA47C-61DF-11D2-AA79-00C04F990343}";
		[TypeLibVar((short) 0x40), MarshalAs(UnmanagedType.LPStr)]
		public const string vsext_wk_ImmedWindow = "{98731960-965C-11D0-A78F-00A0C9110051}";
		[TypeLibVar((short) 0x40), MarshalAs(UnmanagedType.LPStr)]
		public const string vsext_wk_LocalsWindow = "{4A18F9D0-B838-11D0-93EB-00A0C90F2734}";
		[TypeLibVar((short) 0x40), MarshalAs(UnmanagedType.LPStr)]
		public const string vsext_wk_ObjectBrowser = "{269A02DC-6AF8-11D3-BDC4-00C04F688E50}";
		[TypeLibVar((short) 0x40), MarshalAs(UnmanagedType.LPStr)]
		public const string vsext_wk_OutputWindow = "{34E76E81-EE4A-11D0-AE2E-00A0C90FFFC3}";
		[TypeLibVar((short) 0x40), MarshalAs(UnmanagedType.LPStr)]
		public const string vsext_wk_PropertyBrowser = "{EEFA5220-E298-11D0-8F78-00A0C9110057}";
		[TypeLibVar((short) 0x40), MarshalAs(UnmanagedType.LPStr)]
		public const string vsext_wk_SProjectWindow = "{3AE79031-E1BC-11D0-8F78-00A0C9110057}";
		[TypeLibVar((short) 0x40), MarshalAs(UnmanagedType.LPStr)]
		public const string vsext_wk_TaskList = "{4A9B7E51-AA16-11D0-A8C5-00A0C921A4D2}";
		[TypeLibVar((short) 0x40), MarshalAs(UnmanagedType.LPStr)]
		public const string vsext_wk_ThreadWindow = "{E62CE6A0-B439-11D0-A79D-00A0C9110051}";
		[TypeLibVar((short) 0x40), MarshalAs(UnmanagedType.LPStr)]
		public const string vsext_wk_Toolbox = "{B1E99781-AB81-11D0-B683-00AA00A3EE26}";
		[TypeLibVar((short) 0x40), MarshalAs(UnmanagedType.LPStr)]
		public const string vsext_wk_WatchWindow = "{90243340-BD7A-11D0-93EF-00A0C90F2734}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsMiscFilesProjectUniqueName = "<MiscFiles>";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsProjectItemKindMisc = "{66A2671F-8FB5-11D2-AA7E-00C04F688DDE}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsProjectItemKindPhysicalFile = "{6BB5F8EE-4483-11D3-8BCF-00C04F8EC28C}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsProjectItemKindPhysicalFolder = "{6BB5F8EF-4483-11D3-8BCF-00C04F8EC28C}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsProjectItemKindSolutionItems = "{66A26722-8FB5-11D2-AA7E-00C04F688DDE}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsProjectItemKindSubProject = "{EA6618E8-6E24-4528-94BE-6889FE16485C}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsProjectItemKindVirtualFolder = "{6BB5F8F0-4483-11D3-8BCF-00C04F8EC28C}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsProjectItemsKindMisc = "{66A2671E-8FB5-11D2-AA7E-00C04F688DDE}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsProjectItemsKindSolutionItems = "{66A26721-8FB5-11D2-AA7E-00C04F688DDE}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsProjectKindMisc = "{66A2671D-8FB5-11D2-AA7E-00C04F688DDE}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsProjectKindSolutionItems = "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsProjectKindUnmodeled = "{67294A52-A4F0-11D2-AA88-00C04F688DDE}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsProjectsKindSolution = "{96410B9F-3542-4A14-877F-BC7227B51D3B}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsSolutionItemsProjectUniqueName = "<SolnItems>";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsViewKindAny = "{FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsViewKindCode = "{7651A701-06E5-11D1-8EBD-00A0C90F26EA}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsViewKindDebugging = "{7651A700-06E5-11D1-8EBD-00A0C90F26EA}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsViewKindDesigner = "{7651A702-06E5-11D1-8EBD-00A0C90F26EA}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsViewKindPrimary = "{00000000-0000-0000-0000-000000000000}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsViewKindTextView = "{7651A703-06E5-11D1-8EBD-00A0C90F26EA}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsWindowKindAutoLocals = "{F2E84780-2AF1-11D1-A7FA-00A0C9110051}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsWindowKindCallStack = "{0504FF91-9D61-11D0-A794-00A0C9110051}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsWindowKindClassView = "{C9C0AE26-AA77-11D2-B3F0-0000F87570EE}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsWindowKindCommandWindow = "{28836128-FC2C-11D2-A433-00C04F72D18A}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsWindowKindDocumentOutline = "{25F7E850-FFA1-11D0-B63F-00A0C922E851}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsWindowKindDynamicHelp = "{66DBA47C-61DF-11D2-AA79-00C04F990343}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsWindowKindFindReplace = "{CF2DDC32-8CAD-11D2-9302-005345000000}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsWindowKindFindResults1 = "{0F887920-C2B6-11D2-9375-0080C747D9A0}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsWindowKindFindResults2 = "{0F887921-C2B6-11D2-9375-0080C747D9A0}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsWindowKindFindSymbol = "{53024D34-0EF5-11D3-87E0-00C04F7971A5}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsWindowKindFindSymbolResults = "{68487888-204A-11D3-87EB-00C04F7971A5}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsWindowKindLinkedWindowFrame = "{9DDABE99-1D02-11D3-89A1-00C04F688DDE}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsWindowKindLocals = "{4A18F9D0-B838-11D0-93EB-00A0C90F2734}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsWindowKindMacroExplorer = "{07CD18B4-3BA1-11D2-890A-0060083196C6}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsWindowKindMainWindow = "{9DDABE98-1D02-11D3-89A1-00C04F688DDE}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsWindowKindObjectBrowser = "{269A02DC-6AF8-11D3-BDC4-00C04F688E50}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsWindowKindOutput = "{34E76E81-EE4A-11D0-AE2E-00A0C90FFFC3}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsWindowKindProperties = "{EEFA5220-E298-11D0-8F78-00A0C9110057}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsWindowKindResourceView = "{2D7728C2-DE0A-45b5-99AA-89B609DFDE73}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsWindowKindServerExplorer = "{74946827-37A0-11D2-A273-00C04F8EF4FF}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsWindowKindSolutionExplorer = "{3AE79031-E1BC-11D0-8F78-00A0C9110057}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsWindowKindTaskList = "{4A9B7E51-AA16-11D0-A8C5-00A0C921A4D2}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsWindowKindThread = "{E62CE6A0-B439-11D0-A79D-00A0C9110051}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsWindowKindToolbox = "{B1E99781-AB81-11D0-B683-00AA00A3EE26}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsWindowKindWatch = "{90243340-BD7A-11D0-93EF-00A0C90F2734}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsWindowKindWebBrowser = "{E8B06F52-6D01-11D2-AA7D-00C04F990343}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsWizardAddItem = "{0F90E1D1-4999-11D1-B6D1-00A0C90F2744}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsWizardAddSubProject = "{0F90E1D2-4999-11D1-B6D1-00A0C90F2744}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string vsWizardNewProject = "{0F90E1D0-4999-11D1-B6D1-00A0C90F2744}";
	}
}
