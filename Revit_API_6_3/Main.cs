// Разработать WPF-приложение, которое равномерно расставляет
// определенное кол-во элементов модели между двумя точками: 
// Запрашиваются начальная и конечная точки. 
// Далее появляется окно со списком типов семейств и кол-вом элементов,
// которое должно быть размещено между указанными точками.

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revit_API_6_3
{
    [Transaction(TransactionMode.Manual)]

    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            MainView mainView = new MainView(commandData);
            mainView.ShowDialog();
            //TaskDialog.Show("Сообщение", "Текст");
            return Result.Succeeded;
        }
    }
}
