﻿using System.ComponentModel;

namespace Infrastructure.Models
{
	public enum ArchiveDefaultStateType
	{
		[DescriptionAttribute("за указанное число последних часов")]
		LastHours,

		[DescriptionAttribute("за указанное число последних дней")]
		LastDays,

		[DescriptionAttribute("из всего архива")]
		All,

		[DescriptionAttribute("начиная с указанной даты")]
		FromDate,

		[DescriptionAttribute("согласно указанному дипазону дат")]
		RangeDate
	}
}