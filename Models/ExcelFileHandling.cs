using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.Charts;
using mystap.Models;
namespace FileUploadInMVC.Models
{
    public class ExcelFileHandling
    {
        //This Method will Create an Excel Sheet and Store it in the Memory Stream Object
        //And return thar Memory Stream Object
        public MemoryStream CreateExcelFile(List<Notifikasi> notifikasi)
        {
            //Create an Instance of Workbook, i.e., Creates a new Excel workbook
            var workbook = new XLWorkbook();

            //Add a Worksheets with the workbook
            //Worksheets name is Employees
            IXLWorksheet worksheet = workbook.Worksheets.Add("Notifikasi");

            //Create the Cell
            //First Row is going to be Header 

            worksheet.Cell(1, 1).Value = "id"; //First Row and First Column
            worksheet.Cell(1, 2).Value = "notification_type"; //First Row and First Column
            worksheet.Cell(1, 3).Value = "notifikasi"; //First Row and Second Column
            worksheet.Cell(1, 4).Value = "order"; //First Row and Third Column
            worksheet.Cell(1, 5).Value = "notification_date"; //First Row and Fourth Column
            worksheet.Cell(1, 6).Value = "created_by"; //First Row and Fifth Column
            worksheet.Cell(1, 7).Value = "created_on"; //First Row and Sixth Column
            worksheet.Cell(1, 8).Value = "change_by"; //First Row and Sixth Column
            worksheet.Cell(1, 9).Value = "change_on"; //First Row and Sixth Column
            worksheet.Cell(1, 10).Value = "planner_group"; //First Row and Sixth Column
            worksheet.Cell(1, 11).Value = "description"; //First Row and Sixth Column
            worksheet.Cell(1, 12).Value = "user_status"; //First Row and Sixth Column
            worksheet.Cell(1, 13).Value = "system_status"; //First Row and Sixth Column
            worksheet.Cell(1, 14).Value = "maintenance_plant"; //First Row and Sixth Column
            worksheet.Cell(1, 15).Value = "functional_location"; //First Row and Sixth Column
            worksheet.Cell(1, 16).Value = "equipment"; //First Row and Sixth Column
            worksheet.Cell(1, 17).Value = "required_start"; //First Row and Sixth Column
            worksheet.Cell(1, 18).Value = "required_end"; //First Row and Sixth Column
            worksheet.Cell(1, 19).Value = "location"; //First Row and Sixth Column
            worksheet.Cell(1, 20).Value = "main_work_center"; //First Row and Sixth Column
            worksheet.Cell(1, 21).Value = "maintenance_item"; //First Row and Sixth Column
            worksheet.Cell(1, 22).Value = "maintenance_plan"; //First Row and Sixth Column
            worksheet.Cell(1, 23).Value = "rekomendasi"; //First Row and Sixth Column

            //Data is going to stored from Row 2
            int row = 2;

            //Loop Through Each Employees and Populate the worksheet
            //For Each Employee increase row by 1
            foreach (var not in notifikasi)
            {
                worksheet.Cell(row, 1).Value = not.id;
                worksheet.Cell(row, 2).Value = not.notification_type;
                worksheet.Cell(row, 3).Value = not.notifikasi;
                worksheet.Cell(row, 4).Value = not.order;
                worksheet.Cell(row, 5).Value = not.notification_date;
                worksheet.Cell(row, 6).Value = not.created_by;
                worksheet.Cell(row, 7).Value = not.created_on;
                worksheet.Cell(row, 8).Value = not.change_by;
                worksheet.Cell(row, 9).Value = not.change_on;
                worksheet.Cell(row, 10).Value = not.planner_group;
                worksheet.Cell(row, 11).Value = not.description;
                worksheet.Cell(row, 12).Value = not.user_status;
                worksheet.Cell(row, 13).Value = not.system_status;
                worksheet.Cell(row, 14).Value = not.maintenance_plant;
                worksheet.Cell(row, 15).Value = not.functional_location;
                worksheet.Cell(row, 16).Value = not.equipment;
                worksheet.Cell(row, 17).Value = not.required_start;
                worksheet.Cell(row, 18).Value = not.required_end;
                worksheet.Cell(row, 19).Value = not.location;
                worksheet.Cell(row, 20).Value = not.main_work_center;
                worksheet.Cell(row, 21).Value = not.maintenance_item;
                worksheet.Cell(row, 22).Value = not.maintenance_plan;
                worksheet.Cell(row, 23).Value = not.rekomendasi;
                row++; //Increasing the Data Row by 1
            }

            //Create an Memory Stream Object
            var stream = new MemoryStream();

            //Saves the current workbook to the Memory Stream Object.
            workbook.SaveAs(stream);

            //The Position property gets or sets the current position within the stream.
            //This is the next position a read, write, or seek operation will occur from.
            stream.Position = 0;

            return stream;
        }
        public List<Notifikasi> ParseExcelFile(Stream stream)
        {
            var notifikasis = new List<Notifikasi>();
            //Create a workbook instance
            //Opens an existing workbook from a stream.
            using (var workbook = new XLWorkbook(stream))
            {
                //Lets assume the First Worksheet contains the data
                var worksheet = workbook.Worksheet(1);
                //Lets assume first row contains the header, so skip the first row
                var rows = worksheet.RangeUsed().RowsUsed().Skip(1);
                //Loop Through all the Rows except the first row which contains the header data
                foreach (var row in rows)
                {
                    //Create an Instance of Employee object and populate it with the Excel Data Row
                    var notifikasi = new Notifikasi
                    {
                        notification_type = row.Cell(1).GetValue<string>(),
                        notifikasi = row.Cell(2).GetValue<string>(),
                        order = row.Cell(3).GetValue<string>(),
                        notification_date = row.Cell(4).GetValue<DateTime>(),
                        created_by = row.Cell(5).GetValue<string>(),
                        created_on = row.Cell(6).GetValue<DateTime>(),
                        change_by = row.Cell(7).GetValue<string>(),
                        change_on = row.Cell(8).GetValue<DateTime>(),
                        planner_group = row.Cell(9).GetValue<string>(),
                        description = row.Cell(10).GetValue<string>(),
                        user_status = row.Cell(11).GetValue<int>(),
                        system_status = row.Cell(12).GetValue<string>(),
                        maintenance_plant = row.Cell(13).GetValue<string>(),
                        functional_location = row.Cell(14).GetValue<string>(),
                        equipment = row.Cell(15).GetValue<string>(),
                        required_start = row.Cell(16).GetValue<DateTime>(),
                        required_end = row.Cell(17).GetValue<DateTime>(),
                        location = row.Cell(18).GetValue<string>(),
                        main_work_center = row.Cell(19).GetValue<string>(),
                        maintenance_item = row.Cell(20).GetValue<string>(),
                        maintenance_plan = row.Cell(21).GetValue<string>(),
                        rekomendasi = row.Cell(22).GetValue<string>(),
                    };
                    //Add the Employee to the List of Employees
                    notifikasis.Add(notifikasi);
                }
            }
            //Finally return the List of Employees
            return notifikasis;
        }

    }
}