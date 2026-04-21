using System.Linq;
using TaskManager.Core.Interfaces;

namespace TaskManager.Core.Services
{
    public class ReportService
    {
        private readonly ITaskReader _reader;

        // Injectam doar ITaskReader. Acest serviciu nu are acces la Delete() sau Add()
        public ReportService(ITaskReader reader)
        {
            _reader = reader;
        }

        public string GenerateSummary()
        {
            var tasks = _reader.GetAll();
            int total = tasks.Count;

            // Ajusteaza "Done" in functie de cum ai numit tu statusul in clasa/enum-ul tau
            int doneCount = tasks.Count(t => t.Status.ToString() == "Done");

            return $"Total sarcini: {total}, Finalizate: {doneCount}";
        }
    }
}