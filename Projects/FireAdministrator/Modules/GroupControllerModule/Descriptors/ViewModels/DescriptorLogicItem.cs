﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using Controls.Converters;

namespace GKModule.ViewModels
{
	public class DescriptorLogicItem : BaseViewModel
	{
		public FormulaOperation FormulaOperation { get; private set; }
		DescriptorsViewModel DescriptorsViewModel;

		public DescriptorLogicItem(FormulaOperation formulaOperation, DescriptorsViewModel descriptorsViewModel)
		{
			FormulaOperation = formulaOperation;
			DescriptorsViewModel = descriptorsViewModel;

			FirstOperand = FormulaOperation.FirstOperand.ToString();
			SecondOperand = FormulaOperation.SecondOperand.ToString();

			switch (FormulaOperation.FormulaOperationType)
			{
				case FormulaOperationType.ADD:
				case FormulaOperationType.AND:
				case FormulaOperationType.COM:
				case FormulaOperationType.DUP:
				case FormulaOperationType.END:
				case FormulaOperationType.EQ:
				case FormulaOperationType.GE:
				case FormulaOperationType.GT:
				case FormulaOperationType.LE:
				case FormulaOperationType.LT:
				case FormulaOperationType.MUL:
				case FormulaOperationType.NE:
				case FormulaOperationType.NEG:
				case FormulaOperationType.OR:
				case FormulaOperationType.SUB:
				case FormulaOperationType.XOR:
					FirstOperand = "";
					SecondOperand = "";
					break;

				case FormulaOperationType.CONST:
					FirstOperand = "";
					break;

				default:
					IsBold = true;

					var stateTypeToIconConverter = new XStateTypeToIconConverter();
					StateIcon = (string)stateTypeToIconConverter.Convert((XStateBit)FormulaOperation.FirstOperand, null, null, null);

					var stateTypeToStringConverter = new XStateTypeToStringConverter();
					FirstOperand = (string)stateTypeToStringConverter.Convert((XStateBit)FormulaOperation.FirstOperand, null, null, null);

					var descriptorViewModel = DescriptorsViewModel.Descriptors.FirstOrDefault(x => x.Descriptor.XBase.GKDescriptorNo == FormulaOperation.SecondOperand);
					if (descriptorViewModel != null)
					{
						DescriptorIcon = descriptorViewModel.ImageSource;
						SecondOperand = descriptorViewModel.Descriptor.XBase.PresentationName;
					}
					break;
			}
		}

		public string FirstOperand { get; private set; }
		public string SecondOperand { get; private set; }
		public bool IsBold { get; private set; }
		public string StateIcon { get; private set; }
		public string DescriptorIcon { get; private set; }

		public string StackIcon
		{
			get
			{
				switch (FormulaOperation.FormulaOperationType)
				{
					case FormulaOperationType.CONST:
					case FormulaOperationType.DUP:
					case FormulaOperationType.GETBIT:
					case FormulaOperationType.GETBYTE:
					case FormulaOperationType.GETWORD:
						return "/Controls;component/Images/BArrowUp.png";

					case FormulaOperationType.ADD:
					case FormulaOperationType.AND:
					case FormulaOperationType.EQ:
					case FormulaOperationType.NE:
					case FormulaOperationType.GE:
					case FormulaOperationType.GT:
					case FormulaOperationType.LE:
					case FormulaOperationType.LT:
					case FormulaOperationType.MUL:
					case FormulaOperationType.OR:
					case FormulaOperationType.PUTBIT:
					case FormulaOperationType.PUTBYTE:
					case FormulaOperationType.PUTWORD:
					case FormulaOperationType.SUB:
					case FormulaOperationType.XOR:
						return "/Controls;component/Images/BArrowDown.png";

					case FormulaOperationType.COM:
					case FormulaOperationType.END:
					case FormulaOperationType.NEG:
						return null;
				}
				return null;
			}
		}
	}
}