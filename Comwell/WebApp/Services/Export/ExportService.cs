using Core.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace WebApp.Services.Export
{
    public class ExportService : IExportService
    {
        public async Task<byte[]> ExportToExcelAsync(List<User> users, List<StudentPlan> plans, string selectedLocation = "", string selectedGoalTitle = "")
        {
            // Set the EPPlus license context
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();

            // Filter users based on location if specified
            var filteredUsers = string.IsNullOrWhiteSpace(selectedLocation)
                ? users
                : users.Where(u => u.Hotel == selectedLocation).ToList();

            // Get unique roles, with "Elev" first
            var roles = filteredUsers
                .Select(u => u.Role)
                .Distinct()
                .OrderByDescending(r => r == "Elev")
                .ToList();

            // Create overview sheet
            await CreateOverviewSheet(package, filteredUsers, roles, plans, selectedLocation, selectedGoalTitle);

            // Create detailed sheets for each role
            foreach (var role in roles)
            {
                var usersInRole = filteredUsers.Where(u => u.Role == role).ToList();
                if (usersInRole.Any())
                {
                    await CreateRoleSheet(package, usersInRole, plans, role, selectedGoalTitle);
                }
            }

            return await Task.FromResult(package.GetAsByteArray());
        }

        private async Task CreateOverviewSheet(ExcelPackage package, List<User> filteredUsers, List<string> roles, List<StudentPlan> plans, string selectedLocation, string selectedGoalTitle)
        {
            var worksheet = package.Workbook.Worksheets.Add("Oversigt");

            // Title
            worksheet.Cells["A1"].Value = "HR Rapport - Medarbejderoversigt";
            worksheet.Cells["A1"].Style.Font.Size = 16;
            worksheet.Cells["A1"].Style.Font.Bold = true;
            worksheet.Cells["A1:E1"].Merge = true;

            // Export info
            var row = 3;
            worksheet.Cells[$"A{row}"].Value = "Eksporteret:";
            worksheet.Cells[$"B{row}"].Value = DateTime.Now.ToString("dd-MM-yyyy HH:mm");
            
            row++;
            worksheet.Cells[$"A{row}"].Value = "Filtreret efter lokation:";
            worksheet.Cells[$"B{row}"].Value = string.IsNullOrWhiteSpace(selectedLocation) ? "Alle lokationer" : selectedLocation;
            
            row++;
            worksheet.Cells[$"A{row}"].Value = "Filtreret efter mål:";
            worksheet.Cells[$"B{row}"].Value = string.IsNullOrWhiteSpace(selectedGoalTitle) ? "Alle mål" : selectedGoalTitle;

            // Summary table
            row += 2;
            var startRow = row;
            
            // Headers
            worksheet.Cells[$"A{row}"].Value = "Rolle";
            worksheet.Cells[$"B{row}"].Value = "Antal medarbejdere";
            worksheet.Cells[$"C{row}"].Value = "Procent af total";
            worksheet.Cells[$"D{row}"].Value = "Gennemsnitlig fuldførelse (%)";

            // Style headers
            using (var range = worksheet.Cells[$"A{row}:D{row}"])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }

            row++;

            // Data rows
            var totalUsers = filteredUsers.Count;
            foreach (var role in roles)
            {
                var count = filteredUsers.Count(u => u.Role == role);
                if (count > 0)
                {
                    var percentage = totalUsers > 0 ? (double)count / totalUsers * 100 : 0;
                    var avgCompletion = role == "Elev" ? CalculateRoleAverageCompletion(filteredUsers.Where(u => u.Role == role).ToList(), plans, selectedGoalTitle) : 0;

                    worksheet.Cells[$"A{row}"].Value = role;
                    worksheet.Cells[$"B{row}"].Value = count;
                    worksheet.Cells[$"C{row}"].Value = $"{percentage:F1}%";
                    worksheet.Cells[$"D{row}"].Value = role == "Elev" ? $"{avgCompletion}%" : "N/A";

                    row++;
                }
            }

            // Style data table
            using (var range = worksheet.Cells[$"A{startRow}:D{row - 1}"])
            {
                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                range.AutoFitColumns();
            }

            await Task.CompletedTask;
        }

        private async Task CreateRoleSheet(ExcelPackage package, List<User> usersInRole, List<StudentPlan> plans, string role, string selectedGoalTitle)
        {
            var worksheet = package.Workbook.Worksheets.Add($"{role} Medarbejdere");

            // Title
            worksheet.Cells["A1"].Value = $"{role} Medarbejdere - Detaljeret oversigt";
            worksheet.Cells["A1"].Style.Font.Size = 14;
            worksheet.Cells["A1"].Style.Font.Bold = true;
            worksheet.Cells["A1:F1"].Merge = true;

            // Headers
            var row = 3;
            worksheet.Cells[$"A{row}"].Value = "Navn";
            worksheet.Cells[$"B{row}"].Value = "Email";
            worksheet.Cells[$"C{row}"].Value = "Hotel/Lokation";
            worksheet.Cells[$"D{row}"].Value = "Medarbejder ID";
            
            var columnCount = 4;
            if (role == "Elev")
            {
                worksheet.Cells[$"E{row}"].Value = "Fuldførelse (%)";
                worksheet.Cells[$"F{row}"].Value = "Status";
                columnCount = 6;
            }

            // Style headers
            using (var range = worksheet.Cells[$"A{row}:{GetColumnLetter(columnCount)}{row}"])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }

            row++;

            // Data rows
            foreach (var user in usersInRole.OrderBy(u => u.Name))
            {
                worksheet.Cells[$"A{row}"].Value = user.Name;
                worksheet.Cells[$"B{row}"].Value = user.Email;
                worksheet.Cells[$"C{row}"].Value = user.Hotel;
                worksheet.Cells[$"D{row}"].Value = user.Id;

                if (role == "Elev")
                {
                    var userPlans = plans.Where(p => p.StudentId == user.Id).ToList();
                    var allGoals = userPlans.SelectMany(p => p.Goals).ToList();
                    var percent = CalculateCompletionPercentage(allGoals, selectedGoalTitle);
                    
                    worksheet.Cells[$"E{row}"].Value = $"{percent}%";
                    
                    var status = percent switch
                    {
                        >= 90 => "Fremragende",
                        >= 70 => "God progression",
                        >= 40 => "I gang",
                        _ => "Behøver opmærksomhed"
                    };
                    
                    worksheet.Cells[$"F{row}"].Value = status;

                    // Color code the status
                    var statusCell = worksheet.Cells[$"F{row}"];
                    statusCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    statusCell.Style.Fill.BackgroundColor.SetColor(percent switch
                    {
                        >= 90 => Color.LightGreen,
                        >= 70 => Color.LightBlue,
                        >= 40 => Color.LightYellow,
                        _ => Color.LightPink
                    });
                }

                row++;
            }

            // Style data table and autofit columns
            using (var range = worksheet.Cells[$"A3:{GetColumnLetter(columnCount)}{row - 1}"])
            {
                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                range.AutoFitColumns();
            }

            await Task.CompletedTask;
        }

        private int CalculateCompletionPercentage(List<Goal> goals, string selectedGoalTitle)
        {
            if (string.IsNullOrWhiteSpace(selectedGoalTitle))
            {
                int total = goals.Count;
                int done = goals.Count(g => g.Subtasks.All(s => s.Status == "Fuldført"));
                return total > 0 ? (int)Math.Round(done * 100.0 / total) : 0;
            }
            else
            {
                var selectedGoals = goals.Where(g => g.Title == selectedGoalTitle).ToList();
                int total = selectedGoals.Count;
                int done = selectedGoals.Count(g => g.Subtasks.All(s => s.Status == "Fuldført"));
                return total > 0 ? (int)Math.Round(done * 100.0 / total) : 0;
            }
        }

        private int CalculateRoleAverageCompletion(List<User> usersInRole, List<StudentPlan> plans, string selectedGoalTitle)
        {
            if (!usersInRole.Any() || usersInRole.First().Role != "Elev") 
                return 0;

            var completions = usersInRole.Select(student =>
            {
                var userPlans = plans.Where(p => p.StudentId == student.Id).ToList();
                var allGoals = userPlans.SelectMany(p => p.Goals).ToList();
                return CalculateCompletionPercentage(allGoals, selectedGoalTitle);
            }).ToList();

            return completions.Any() ? (int)completions.Average() : 0;
        }

        private static string GetColumnLetter(int column)
        {
            string letter = "";
            while (column > 0)
            {
                column--;
                letter = (char)(65 + column % 26) + letter;
                column /= 26;
            }
            return letter;
        }
    }
} 