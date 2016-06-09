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

namespace DevExpress.Utils.MVVM.Internal {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.Reflection.Emit;
	sealed class ILReader {
		internal ILReader(MethodInfo mInfo) {
			Instructions = ReadInstructions(mInfo);
		}
		public Instruction[] Instructions { get; private set; }
		Instruction[] ReadInstructions(System.Reflection.MethodInfo method) {
			List<Instruction> results = new List<Instruction>();
			var mBody = method.GetMethodBody();
			if(mBody != null) {
				var reader = new BinaryReader(mBody.GetILAsByteArray());
				var context = new OperandReaderContext(method);
				while(reader.CanRead()) {
					int offset = reader.Offset;
					OpCode opCode = OpCodeReader.ReadOpCode(reader);
					object operand = OperandReader.Read(reader, context, opCode.OperandType);
					results.Add(new Instruction(offset, opCode, operand));
				}
			}
			return results.ToArray();
		}
		#region Patterns & Instructions
		internal sealed class Pattern {
			Func<Instruction[], bool> match;
			public Pattern(params OpCode[] opcodes)
				: this(opcodes.Select(op => new Func<Instruction, bool>(i => i.OpCode == op)).ToArray()) {
			}
			public Pattern(params Func<Instruction, bool>[] matches) {
				match = (instructions) =>
				{
					captures = new Instruction[] { };
					List<Instruction> results = new List<Instruction>();
					int startIndex = 0;
					foreach(var m in matches) {
						int index = Array.FindIndex(instructions, startIndex, i => m(i));
						if(index >= 0) {
							results.Add(instructions[index]);
							startIndex = index;
						}
						else return false;
					}
					captures = results.ToArray();
					return true;
				};
			}
			Instruction[] captures;
			public Instruction[] Captures {
				get { return captures; }
			}
			public bool Match(ILReader reader) {
				return Match(reader.Instructions);
			}
			public bool Match(Instruction[] instructions) {
				return match(instructions);
			}
		}
		internal sealed class Instruction {
			public Instruction(int offset, OpCode opCode, object operand) {
				this.Offset = offset;
				this.OpCode = opCode;
				this.Operand = operand;
			}
			public int Offset { get; private set; }
			public OpCode OpCode { get; private set; }
			public object Operand { get; private set; }
			public override string ToString() {
				if(object.ReferenceEquals(Operand, null))
					return string.Format("{0:X5}:  {1}", Offset, OpCode);
				else
					return string.Format("{0:X5}:  {1} {2}", Offset, OpCode, Operand);
			}
		}
		#endregion
		#region Readers
		sealed class BinaryReader {
			readonly byte[] bytes;
			int ptr;
			public BinaryReader(byte[] bytes) {
				this.bytes = bytes;
			}
			internal bool CanRead() {
				return ptr < bytes.Length;
			}
			internal byte ReadByte() {
				return bytes[ptr++];
			}
			internal bool ReadBoolean() {
				return bytes[ptr++] != 0;
			}
			internal short ReadShort() {
				ptr += 2;
				return System.BitConverter.ToInt16(bytes, ptr - 2);
			}
			internal int ReadInt() {
				ptr += 4;
				return System.BitConverter.ToInt32(bytes, ptr - 4);
			}
			internal float ReadFloat() {
				ptr += 4;
				return System.BitConverter.ToSingle(bytes, ptr - 4);
			}
			internal long ReadLong() {
				ptr += 8;
				return System.BitConverter.ToInt64(bytes, ptr - 8);
			}
			internal double ReadDouble() {
				ptr += 8;
				return System.BitConverter.ToDouble(bytes, ptr - 8);
			}
			internal int Offset {
				get { return ptr; }
			}
		}
		sealed class OperandReaderContext {
			public OperandReaderContext(MethodBase method)
				: this(method, method.GetMethodBody()) {
			}
			public OperandReaderContext(MethodBase method, MethodBody methodBody) {
				module = method.Module;
				variables = methodBody.LocalVariables.ToArray();
				methodArguments = method.IsGenericMethod ? method.GetGenericArguments() : null;
				typeArguments = method.DeclaringType.IsGenericType ? method.DeclaringType.GetGenericArguments() : null;
			}
			readonly Module module;
			readonly LocalVariableInfo[] variables;
			readonly Type[] methodArguments;
			readonly Type[] typeArguments;
			public LocalVariableInfo this[byte variableIndex] {
				get { return variables[variableIndex]; }
			}
			public LocalVariableInfo this[short variableIndex] {
				get { return variables[variableIndex]; }
			}
			public MethodBase ResolveMethod(int methodToken) {
				return module.ResolveMethod(methodToken, typeArguments, methodArguments);
			}
			public FieldInfo ResolveField(int fieldToken) {
				return module.ResolveField(fieldToken, typeArguments, methodArguments);
			}
			public Type ResolveType(int typeToken) {
				return module.ResolveType(typeToken, typeArguments, methodArguments);
			}
			public MemberInfo ResolveMember(int memberToken) {
				return module.ResolveMember(memberToken, typeArguments, methodArguments);
			}
			public string ResolveString(int stringToken) {
				return module.ResolveString(stringToken);
			}
			public byte[] ResolveSignature(int sigToken) {
				return module.ResolveSignature(sigToken);
			}
		}
		static class OpCodeReader {
			#region Initialization
			static OpCodeReader() {
				CreateOpCodes();
			}
			static OpCode[] singleByteOpCode;
			static OpCode[] doubleByteOpCode;
			static void CreateOpCodes() {
				singleByteOpCode = new OpCode[225];
				doubleByteOpCode = new OpCode[31];
				FieldInfo[] fields = GetOpCodeFields();
				for(int i = 0; i < fields.Length; i++) {
					OpCode code = (OpCode)fields[i].GetValue(null);
					if(code.OpCodeType == OpCodeType.Nternal)
						continue;
					if(code.Size == 1)
						singleByteOpCode[code.Value] = code;
					else
						doubleByteOpCode[code.Value & 0xff] = code;
				}
			}
			static FieldInfo[] GetOpCodeFields() {
				return typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static);
			}
			#endregion Initialization
			public static OpCode ReadOpCode(BinaryReader binaryReader) {
				byte instruction = binaryReader.ReadByte();
				if(instruction != 254)
					return singleByteOpCode[instruction];
				else
					return doubleByteOpCode[binaryReader.ReadByte()];
			}
		}
		static class OperandReader {
			#region Initialization
			static IDictionary<OperandType, Func<BinaryReader, OperandReaderContext, object>> cache = new Dictionary<OperandType, Func<BinaryReader, OperandReaderContext, object>>();
			static OperandReader() {
				cache.Add(OperandType.InlineNone, (r, c) => null);
				cache.Add(OperandType.ShortInlineBrTarget, (r, c) => r.ReadByte());
				cache.Add(OperandType.ShortInlineI, (r, c) => r.ReadByte());
				cache.Add(OperandType.ShortInlineVar, (r, c) => c[r.ReadByte()]);
				cache.Add(OperandType.InlineVar, (r, c) => c[r.ReadByte()]);
				cache.Add(OperandType.InlineBrTarget, (r, c) => r.ReadInt());
				cache.Add(OperandType.InlineField, (r, c) => c.ResolveField(r.ReadInt()));
				cache.Add(OperandType.InlineI, (r, c) => r.ReadInt());
				cache.Add(OperandType.InlineMethod, (r, c) => c.ResolveMethod(r.ReadInt()));
				cache.Add(OperandType.InlineSig, (r, c) => c.ResolveSignature(r.ReadInt()));
				cache.Add(OperandType.InlineString, (r, c) => c.ResolveString(r.ReadInt()));
				cache.Add(OperandType.InlineSwitch, (r, c) => r.ReadInt());
				cache.Add(OperandType.InlineTok, (r, c) => c.ResolveMember(r.ReadInt()));
				cache.Add(OperandType.InlineType, (r, c) => c.ResolveType(r.ReadInt()));
				cache.Add(OperandType.ShortInlineR, (r, c) => r.ReadFloat());
				cache.Add(OperandType.InlineI8, (r, c) => r.ReadLong());
				cache.Add(OperandType.InlineR, (r, c) => r.ReadDouble());
			}
			#endregion Initialization
			internal static object Read(BinaryReader reader, OperandReaderContext context, OperandType operandType) {
				Func<BinaryReader, OperandReaderContext, object> read;
				if(cache.TryGetValue(operandType, out read))
					return read(reader, context);
				throw new NotSupportedException(operandType.ToString());
			}
		}
		#endregion Readers
	}
}
#if DEBUGTEST
namespace DevExpress.Utils.MVVM.Tests {
	using System;
	using System.Reflection.Emit;
	using DevExpress.Utils.MVVM.Internal;
	using NUnit.Framework;
	[TestFixture]
	public class ILReader_Tests {
		Action<Foo, Action> subscribe =
			(foo, execute) => foo.Click += (s, e) => execute();
		class Foo {
			public event EventHandler Click {
				add { }
				remove { }
			}
		}
		[Test]
		public void Test_ReadInstructions() {
			var reader = new ILReader(subscribe.Method);
			var l = reader.Instructions.Length;
			Assert.AreEqual(OpCodes.Newobj, reader.Instructions[0].OpCode);
			Assert.AreEqual(OpCodes.Ret, reader.Instructions[l - 1].OpCode);
		}
		[Test]
		public void Test_SubscribePatern() {
			var reader = new ILReader(subscribe.Method);
			Assert.IsTrue(EventInfoHelper.GetSubscribeEventPattern().Match(reader));
		}
		[Test]
		public void Test_SubscribePattern2() {
			Action update;
			Action<Foo, Action> subscribe2 = 
				(foo, execute) => update = () => execute();
			var reader = new ILReader(subscribe2.Method);
			Assert.IsFalse(EventInfoHelper.GetSubscribeEventPattern().Match(reader));
		}
	}
}
#endif
