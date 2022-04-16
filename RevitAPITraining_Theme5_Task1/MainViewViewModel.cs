using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPITraining_Theme5_Task1
{
   public class MainViewViewModel
    {
        private ExternalCommandData _commandData;

        public DelegateCommand PipeComand { get; }
        public DelegateCommand WallComand { get; }
        public DelegateCommand DoorCommand { get; }

        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            PipeComand = new DelegateCommand(OnSelectCommand);
            WallComand = new DelegateCommand(OnSelectCommand1);
            DoorCommand = new DelegateCommand(OnSelectCommand2);
        }

        private void OnSelectCommand1()
        {
            RaiseHideRequest();

            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            var walls = new FilteredElementCollector(doc)
                                               .OfCategory(BuiltInCategory.OST_Walls)
                                               .WhereElementIsNotElementType()
                                               .Cast<Wall>()
                                               .ToList();
            var elementList = new List<Double>();

            foreach (var wall in walls)
            {
                //Element element = doc.GetElement(wall);
                Parameter volumeParameter = wall.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED);
                if (volumeParameter.StorageType == StorageType.Double)
                {
                    double volumeValue = UnitUtils.ConvertFromInternalUnits(volumeParameter.AsDouble(), UnitTypeId.CubicMeters);
                    elementList.Add(volumeValue);
                }
            }
            double sum = elementList.ToArray().Sum();
            TaskDialog.Show("Суммарный объем", $"Суммарный объем стен: {sum} м²");

            RaiseShowRequest();
        }

        private void OnSelectCommand2()
        {
            RaiseHideRequest();

            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            var doors = new FilteredElementCollector(doc)
                                               .OfCategory(BuiltInCategory.OST_Doors)
                                               .WhereElementIsNotElementType()
                                               .Cast<FamilyInstance>()
                                               .ToList();

            TaskDialog.Show("Колличество дверей в проекте", $"Дверей в проекте: {doors.Count}");

            RaiseShowRequest();
        }

        public event EventHandler HideRequest;

        private void RaiseHideRequest()
        {
            HideRequest?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler ShowRequest;

        private void RaiseShowRequest()
        {
            ShowRequest?.Invoke(this, EventArgs.Empty);
        }

        private void OnSelectCommand()
        {
            RaiseHideRequest();

            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc=uidoc.Document;

            var pipes = new FilteredElementCollector(doc)
                                               .OfCategory(BuiltInCategory.OST_PipeCurves)
                                               .WhereElementIsNotElementType()
                                               .Cast<Pipe>()
                                               .ToList();

            TaskDialog.Show("Колличество труб в проекте", $"Труб в проекте: {pipes.Count}");

            RaiseShowRequest();
        }
    }
}
