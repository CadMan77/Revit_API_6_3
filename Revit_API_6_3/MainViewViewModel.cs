using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Revit_API_6_3
{
    internal class MainViewViewModel
    {
        private ExternalCommandData _commandData;
        private UIDocument uidoc;
        private Document doc;

        public List<FamilySymbol> ProjectFamilySymbols { get; } = new List<FamilySymbol>();
        public FamilySymbol SelectedFamilySymbol { get; set; }

        public DelegateCommand ApplyCommand { get; }

        XYZ point1 = null;
        XYZ point2 = null;

        static readonly string posIntMask = @"^\d+$";
        readonly Regex intRGX = new Regex(posIntMask);

        private int elementQTY;
        public int ELementQTY
        {
            get => elementQTY;
            set
            {
                if (value != 0 && intRGX.IsMatch(value.ToString()))
                {
                    elementQTY = value;
                }
            }
        }

        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            uidoc = _commandData.Application.ActiveUIDocument;
            doc = uidoc.Document;

            ApplyCommand = new DelegateCommand(OnApplyCommand);

            point1 = uidoc.Selection.PickPoint("Выберите 1-ую точку:");
            point2 = uidoc.Selection.PickPoint("Выберите 2-ую точку:");

            List<FamilySymbol> projectFurnitureTypes = new FilteredElementCollector(doc)
                .WhereElementIsElementType()
                .OfCategory(BuiltInCategory.OST_Entourage)
                .Cast<FamilySymbol>()
                .ToList();

            ProjectFamilySymbols = projectFurnitureTypes;
        }

        private void OnApplyCommand()
        {
            if ((point1 == null || point2 == null) || SelectedFamilySymbol == null || elementQTY < 1)
                return;

            List<XYZ> points = new List<XYZ>();

            double pX = (point2.X - point1.X);
            double pY = (point2.Y - point1.Y);
            double pZ = (point2.Z - point1.Z);
            double k = 0.0;

            for (int i = 0; i < elementQTY; i++)
            {
                k = (double)(i)/(double)(elementQTY-1);
                points.Add(new XYZ(point1.X+k*pX, point1.Y+k*pY, point1.Z+k*pZ));
            }

            FamilyInstance fi = null;

            using (Transaction ts = new Transaction(doc, "Place Family Instance Transaction"))
            {
                ts.Start();

                if (!SelectedFamilySymbol.IsActive)
                    SelectedFamilySymbol.Activate();
                foreach (XYZ point in points)
                {
                    fi = doc.Create.NewFamilyInstance(point, SelectedFamilySymbol, StructuralType.NonStructural);
                }

                ts.Commit();
            }
            RaiseCloseRequest();
        }

        public EventHandler CloseRequest;

        public void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }
    }
}