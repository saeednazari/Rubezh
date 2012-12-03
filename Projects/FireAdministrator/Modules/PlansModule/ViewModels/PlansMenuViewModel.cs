﻿
using System.Collections.ObjectModel;
using Controls.Menu.ViewModels;
using Infrastructure.Common.Windows.ViewModels;
namespace PlansModule.ViewModels
{
	//public class PlansMenuViewModel : BaseViewModel
	//{
	//    public PlansMenuViewModel(PlansViewModel context)
	//    {
	//        Context = context;
	//    }

	//    public PlansViewModel Context { get; private set; }
	//}


	public class PlansMenuViewModel : MenuViewModel
	{
		public PlansMenuViewModel(PlansViewModel context)
		{
			Items = new ObservableCollection<BaseViewModel>()
	        {
	            new MenuButtonViewModel(context.AddCommand, "/Controls;component/Images/Add.png" , "Добавить"),
	            new MenuButtonViewModel(context.EditCommand, "/Controls;component/Images/Edit.png" , "Редактировать"),
	            new MenuButtonViewModel(context.RemoveCommand, "/Controls;component/Images/Delete.png" , "Удалить"),
				//new MenuButtonViewModel(context.AddSubPlanCommand, "/Controls;component/Images/AddChild.png" , "Добавить дочерний план"),
				//new MenuSeparatorViewModel(),
	            new MenuButtonViewModel(context.CopyCommand, "/Controls;component/Images/Copy.png" , "Копировать"),
				new MenuButtonViewModel(context.CutCommand, "/Controls;component/Images/Cut.png" , "Вырезать"),
				new MenuButtonViewModel(context.PasteCommand, "/Controls;component/Images/Paste.png" , "Вставить"),
				new MenuButtonViewModel(context.UndoCommand, "/Controls;component/Images/Undo.png" , "Отменить"),
				new MenuButtonViewModel(context.RedoCommand, "/Controls;component/Images/Redo.png" , "Применить"),
				//new MenuSeparatorViewModel(),
	            new MenuButtonViewModel(context.PlanDesignerViewModel.MoveToFrontCommand, "/Controls;component/Images/MoveForward.png" , "Вверх"),
	            new MenuButtonViewModel(context.PlanDesignerViewModel.SendToBackCommand, "/Controls;component/Images/MoveBackward.png" , "Вниз"),
	            new MenuButtonViewModel(context.PlanDesignerViewModel.MoveForwardCommand, "/Controls;component/Images/MoveFront.png" , "Выше"),
	            new MenuButtonViewModel(context.PlanDesignerViewModel.MoveBackwardCommand, "/Controls;component/Images/MoveBack.png" , "Ниже"),
				//new MenuSeparatorViewModel(),
				//context.DesignerCanvas.Toolbox
				new MenuButtonViewModel(context.PlanDesignerViewModel.AlignHorizontalLeftCommand, "/Controls;component/Images/shapes-align-hori-left.png" , "Выровнить по левому краю"),
	            new MenuButtonViewModel(context.PlanDesignerViewModel.AlignHorizontalCenterCommand, "/Controls;component/Images/shapes-align-hori-center.png" , "Выровнить по горизонтали"),
	            new MenuButtonViewModel(context.PlanDesignerViewModel.AlignHorizontalRightCommand, "/Controls;component/Images/shapes-align-hori-right.png" , "Выровнить по правому краю"),
	            new MenuButtonViewModel(context.PlanDesignerViewModel.AlignVerticalTopCommand, "/Controls;component/Images/shapes-align-verti-top.png" , "Выровнить по верхнему краю"),
	            new MenuButtonViewModel(context.PlanDesignerViewModel.AlignVerticalCenterCommand, "/Controls;component/Images/shapes-align-verti-middle.png" , "Выровнить по вертикали"),
	            new MenuButtonViewModel(context.PlanDesignerViewModel.AlignVerticalBottomCommand, "/Controls;component/Images/shapes-align-verti-bottom.png" , "Выровнить по нижнему краю"),
				//new MenuButtonViewModel(context.PlanDesignerViewModel., "/Controls;component/Images/shapes-flip-horizontal.png" , "Ниже"),
				//new MenuButtonViewModel(context.PlanDesignerViewModel., "/Controls;component/Images/shapes-flip-vertical.png" , "Ниже"),
				//new MenuButtonViewModel(context.PlanDesignerViewModel., "/Controls;component/Images/shapes-rotate-anticlockwise.png" , "Ниже"),
				//new MenuButtonViewModel(context.PlanDesignerViewModel., "/Controls;component/Images/shapes-rotate-clockwise.png" , "Ниже"),
	        };
		}
	}
}
