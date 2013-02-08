﻿using System;
using System.Collections.Generic;
using System.Text;
using FiresecAPI;
using XFiresecAPI;

namespace Common.GK
{
	public class FormulaBuilder
	{
		public List<FormulaOperation> FormulaOperations { get; protected set; }

		public FormulaBuilder()
		{
			FormulaOperations = new List<FormulaOperation>();
		}

		public void Add(FormulaOperationType formulaOperationType, byte firstOperand = 0, ushort secondOperand = 0, string comment = null)
		{
			var formulaOperation = new FormulaOperation()
			{
				FormulaOperationType = formulaOperationType,
				FirstOperand = firstOperand,
				SecondOperand = secondOperand,
				Comment = comment
			};
			FormulaOperations.Add(formulaOperation);
		}

		public void AddGetBitOff(XStateType stateType, XBinaryBase binaryBase)
		{
			Add(FormulaOperationType.GETBIT,
				(byte)stateType,
				binaryBase.GetDatabaseNo(DatabaseType.Gk),
				"Проверка состояния " + stateType.ToDescription() + " " + XBinaryBaseToString(binaryBase));
			Add(FormulaOperationType.GETBIT,
				(byte)XStateType.Ignore,
				binaryBase.GetDatabaseNo(DatabaseType.Gk));
			Add(FormulaOperationType.COM);
			Add(FormulaOperationType.AND);
		}

		public void AddGetBit(XStateType stateType, XBinaryBase binaryBase)
		{
			Add(FormulaOperationType.GETBIT,
				(byte)stateType,
				binaryBase.GetDatabaseNo(DatabaseType.Gk),
				"Проверка состояния " + stateType.ToDescription() + " " + XBinaryBaseToString(binaryBase));
		}

		public void AddPutBit(XStateType stateType, XBinaryBase binaryBase)
		{
			Add(FormulaOperationType.PUTBIT,
				(byte)stateType,
				binaryBase.GetDatabaseNo(DatabaseType.Gk),
				"Запись состояния " + stateType.ToDescription() + " " + XBinaryBaseToString(binaryBase));
		}

		public void AddStandardTurning(XBinaryBase binaryBase)
		{
			Add(FormulaOperationType.DUP);
			Add(FormulaOperationType.DUP);
			AddGetBit(XStateType.Norm, binaryBase);
			Add(FormulaOperationType.AND, comment: "Смешивание с битом Дежурный Устройства");
			AddGetBit(XStateType.Save, binaryBase);
			Add(FormulaOperationType.COM);
			Add(FormulaOperationType.AND, comment: "Смешивание с битом 31");
			AddPutBit(XStateType.TurnOn, binaryBase);
			Add(FormulaOperationType.COM);
			AddGetBit(XStateType.Norm, binaryBase);
			Add(FormulaOperationType.AND, comment: "Смешивание с битом Дежурный Устройства");
			AddPutBit(XStateType.TurnOff, binaryBase);
			AddPutBit(XStateType.Save, binaryBase);
		}

		string XBinaryBaseToString(XBinaryBase binaryBase)
		{
			return binaryBase.BinaryInfo.Type + " " + binaryBase.BinaryInfo.Name + " " + binaryBase.BinaryInfo.Address;
		}

		public List<byte> GetBytes()
		{
			var bytes = new List<byte>();
			foreach (var formulaOperation in FormulaOperations)
			{
				bytes.Add((byte)formulaOperation.FormulaOperationType);
				bytes.Add(formulaOperation.FirstOperand);
				bytes.AddRange(BitConverter.GetBytes(formulaOperation.SecondOperand));
			}
			return bytes;
		}

		public string GetStringFomula()
		{
			var stringBuilder = new StringBuilder();
			foreach (var formulaOperation in FormulaOperations)
			{
				stringBuilder.Append(formulaOperation.FormulaOperationType + "\t");
				stringBuilder.Append(formulaOperation.FirstOperand + "\t");
				stringBuilder.Append(formulaOperation.SecondOperand + "\t");
				stringBuilder.Append(formulaOperation.Comment);
				stringBuilder.AppendLine("");
			}
			return stringBuilder.ToString();
		}
	}
}